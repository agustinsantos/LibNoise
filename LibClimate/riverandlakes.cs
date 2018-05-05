using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibClimate
{
    public class riverandlakes
    {

        public static void clearRain(TClima clima)
        {
            long i, j;

            for (j = 0; j < 180; j++)
                for (i = 0; i < 360; i++)
                    clima.rain[i, j] = false;
        }

        public static void transferWater(long i, long j, long i_target, long j_target, TWorld w, TClima clima, double[,] water_surface, double[,] water_grid)
        {
            double transferredWater;

            transferredWater = water_grid[i, j] * TSimConst.riverandlakes_pct;
            water_surface[i, j] = water_surface[i, j] - transferredWater;
            if (water_surface[i, j] < 0) water_surface[i, j] = 0;

            if (clima.isIce[i, j] || w.isOcean[i, j])
                return; // water reached its target


            // water continues to flow down...
            water_surface[i_target, j_target] = water_surface[i_target, j_target] + transferredWater;
            if (water_surface[i_target, j_target] > double.MaxValue) //raise Exception.Create('Too much water!');
                water_surface[i_target, j_target] = double.MaxValue; // we limit the quantity of water in this way
        }

        public static void moveWaterDownToOcean(TWorld w, TClima clima, double[,] water_surface, double[,] water_grid)
        {
            long
              i,
              j,
              check_north,
              check_south,
              check_east,
              check_west;

            double E_own,
             E_north,
             E_south,
             E_west,
             E_east,
             E_north_west,
             E_north_east,
             E_south_west,
             E_south_east;

            double E_lowestCardinal,
             E_lowestDiagonal,
             E_lowest;

            // we need a local copy of the energy grid
            for (j = 0; j < 180; j++)
                for (i = 0; i < 360; i++)
                    water_grid[i, j] = water_surface[i, j];

            for (j = 0; j < 180; j++)
                for (i = 0; i < 360; i++)
                {
                    if (w.isOcean[i, j] || clima.isIce[i, j]) continue;

                    check_north = j - 1;
                    check_south = j + 1;
                    check_west = i - 1;
                    check_east = i + 1;

                    // we live on a sphere
                    if (check_north < 0) check_north = 179;
                    if (check_south > 179) check_south = 0;
                    if (check_west < 0) check_west = 359;
                    if (check_east > 359) check_east = 0;

                    E_north = w.elevation[i, check_north];
                    E_south = w.elevation[i, check_south];
                    E_west = w.elevation[check_west, j];
                    E_east = w.elevation[check_east, j];
                    E_north_west = w.elevation[check_west, check_north];
                    E_north_east = w.elevation[check_east, check_north];
                    E_south_west = w.elevation[check_west, check_south];
                    E_south_east = w.elevation[check_east, check_south];
                    E_own = w.elevation[i, j];

                    E_lowestCardinal = Math.Min(Math.Min(E_north, E_south), Math.Min(E_west, E_east));
                    E_lowestDiagonal = Math.Min(Math.Min(E_north_east, E_south_west), Math.Min(E_north_west, E_south_east));
                    E_lowest = Math.Min(E_lowestCardinal, E_lowestDiagonal);

                    if (E_own == E_lowest)
                        transferWater(i, j, i, j, w, clima,   water_surface,   water_grid);
                    else
                        if (E_west == E_lowest)
                            transferWater(i, j, check_west, j, w, clima, water_surface, water_grid);
                        else
                            if (E_east == E_lowest)
                                transferWater(i, j, check_east, j, w, clima, water_surface, water_grid);
                            else
                                if (E_north == E_lowest)
                                    transferWater(i, j, i, check_north, w, clima, water_surface, water_grid);
                                else
                                    if (E_south == E_lowest)
                                        transferWater(i, j, i, check_south, w, clima, water_surface, water_grid);
                                    else
                                        if (E_north_west == E_lowest)
                                            transferWater(i, j, check_west, check_north, w, clima, water_surface, water_grid);
                                        else
                                            if (E_north_east == E_lowest)
                                                transferWater(i, j, check_east, check_north, w, clima, water_surface, water_grid);
                                            else
                                                if (E_south_east == E_lowest)
                                                    transferWater(i, j, check_east, check_south, w, clima, water_surface, water_grid);
                                                else
                                                    if (E_south_west == E_lowest)
                                                        transferWater(i, j, check_west, check_south, w, clima, water_surface, water_grid);
                }

            averages.computeAvgWaterSurface(w, clima);
        }

        public static bool isRiver(TClima clima, long i, long j)
        {
            return clima.water_surface[i, j] > TSimConst.paint_river_pct * clima.avgWaterSurface[j];
        }

        public static bool isGrass(TWorld w, TClima clima, long i, long j)
        {
            return (clima.water_surface[i, j] >= 1);
        }

        public static bool isForest(TWorld w, TClima clima, long i, long j)
        {
            double T;

            T = Conversion.KtoC(clima.T_ocean_terr[i, j]);
            return (clima.water_surface[i, j] <= TSimConst.paint_river_pct * clima.avgWaterSurface[j]) && (T > 0) && (T < 25) &&
                      (clima.rain_times[i, j] > 1);
        }

        public static bool isJungle(TWorld w, TClima clima, long i, long j)
        {
            double T;

            T = Conversion.KtoC(clima.T_ocean_terr[i, j]);
            return (clima.water_surface[i, j] <= TSimConst.paint_river_pct * clima.avgWaterSurface[j])
                      && (T >= 25) && (T < 50) && (clima.rain_times[i, j] > 2);
        }

        public static bool isDesert(TWorld w, TClima clima, long i, long j)
        {
            double T;

            T = Conversion.KtoC(clima.T_ocean_terr[i, j]);
            return (T >= 50) && (clima.water_surface[i, j] < 1);
        }
    }
}