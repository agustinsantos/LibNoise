using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibClimate
{
    public class averages
    {
        public static double Avg(double one, double two)
        {
            return (one + two) / 2;
        }

        public static double computeAvgKTemperature(TWorld w, TClima clima, long tType)
        {
            long i, j, count;
            double temp;

            temp = 0;
            count = 0;
            for (j = 0; j < 180; j++)
                for (i = 0; i < 360; i++)
                {
                    if (tType == Constants.ATMOSPHERE)
                        temp = clima.T_atmosphere[0, i, j] + temp;
                    else
                        if (tType == Constants.OCEAN_TERR)
                            temp = clima.T_ocean_terr[i, j] + temp;
                        else
                            if (tType == Constants.AVERAGE)
                                temp = clima.T_atmosphere[0, i, j] + clima.T_ocean_terr[i, j] + temp;
                            else
                                if (((tType == Constants.OCEAN) && w.isOcean[i, j]))
                                {
                                    temp = clima.T_ocean_terr[i, j] + temp;
                                    count++;
                                }
                                else
                                    if ((tType == Constants.TERRAIN) && (!w.isOcean[i, j]))
                                    {
                                        temp = clima.T_ocean_terr[i, j] + temp;
                                         count++;
                                    }
                                    else
                                        if ((tType == Constants.AIR_OVER_OCEAN) && w.isOcean[i, j])
                                        {
                                            temp = clima.T_atmosphere[0, i, j] + temp;
                                            count++;
                                        }
                                        else
                                            if ((tType == Constants.AIR_OVER_TERRAIN) && (!w.isOcean[i, j]))
                                            {
                                                temp = clima.T_atmosphere[0, i, j] + temp;
                                                count++;
                                            }
                }

            if (tType == Constants.AVERAGE)
                return temp / (180 * 360 * 2);
            else
                if (count > 0)
                    return temp / count;
                else
                    return temp / (180 * 360);
        }

        public static double computeAvgSteamOnAir(TClima clima)
        {
            long l, i, j;
            double steam;

            steam = 0;
            for (l = 0; l <= TMdlConst.atmospheric_layers - 1; l++)
                for (j = 0; j < 180; j++)
                    for (i = 0; i < 360; i++)
                        steam = steam + clima.steam[l, i, j];
            return steam / (360 * 180 * TMdlConst.atmospheric_layers);
        }

        public static double computeAvgRain(TClima clima)
        {
            long i, j;
            long rainC;

            rainC = 0;
            for (j = 0; j < 180; j++)
                for (i = 0; i < 360; i++)
                    if (clima.rain[i, j])
                         rainC++;
            return rainC / (360 * 180);
        }

        public static double computeAvgHumidity(TWorld w, TClima clima)
        {
            long i, j;
            double humidity;
            humidity = 0;
            for (j = 0; j < 180; j++)
                for (i = 0; i < 360; i++)
                    humidity = humidity + statechanges.computeHumidity(clima, w, i, j);

            return humidity / (360 * 180);
        }

        public static double computeAvgWindMovements(TClima clima)
        {
            long i, j;
            long movements;

            movements = 0;
            for (j = 0; j < 180; j++)
                for (i = 0; i < 360; i++)
                    if (clima.wind[0, i, j] != Constants.NONE)
                        movements = movements + 1;

            return movements / (360 * 180);
        }

        public static double computeAvgCurrentMovements(TWorld w, TClima clima)
        {
            long i, j;
            long movements, count;


            movements = 0;
            count = 0;
            for (j = 0; j < 180; j++)
                for (i = 0; i < 360; i++)
                    if ((clima.surfaceTransfer[i, j] != Constants.NONE) && (w.isOcean[i, j]))
                    {
                        movements = movements + 1;
                        count = count + 1;
                    }
            if (count != 0)
                return movements / count;
            else
                return 0;
        }

        public static long computeIceCoverage(TClima clima, short direction)
        {
            long i, j;
            long iceCoverage;

            iceCoverage = 0;
            for (j = 0; j < 180; j++)
                for (i = 0; i < 360; i++)
                    if (clima.isIce[i, j])
                    {
                        if ((direction == Constants.NORTH) && (j < 90))
                            iceCoverage = iceCoverage + 1;
                        else
                            if ((direction == Constants.SOUTH) && (j >= 90))
                                iceCoverage = iceCoverage + 1;
                            else
                                if (direction == Constants.NONE)
                                    iceCoverage = iceCoverage + 1;
                    }

            return iceCoverage;
        }


        public static void computeAvgWaterSurface(TWorld w, TClima clima)
        {
            long i, j, count;
            double avg;

            for (j = 0; j < 180; j++)
            {
                avg = 0;
                count = 0;

                for (i = 0; i < 360; i++)
                {
                    if ((!w.isOcean[i, j]) && (clima.water_surface[i, j] > 0))
                    {
                        avg = avg + clima.water_surface[i, j];
                        count++;
                    }
                }

                if (count != 0)
                    clima.avgWaterSurface[j] = avg / count;
                else clima.avgWaterSurface[j] = 0;
            }
        }
    }
}
