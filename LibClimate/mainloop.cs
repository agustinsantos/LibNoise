using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibClimate
{
    class mainloop
    {
        public static void mainSimTimestepLoop(TWorld world, TClima clima, TSolarSurface s, TTime t, double[,] tmpGrid, double arthDeclination)
        {
            long l, i, j;
#if TODO
            clearRain(clima);
            for (j = 0; j < 180; j++)
                for (i = 0; i < 360; i++)
                {
                    updateIncomingEnergyOnCellGrid(clima, world, s, earthDeclination, i, j);
                    formSteam(clima, world, s, i, j, t.day); // add steam to layer 0
                    for (l = 1; l < TMdlConst.atmospheric_layers; l++)   // move steam from layer l-1 to layer l
                        moveSteamUp(clima, world, s, l, i, j);
                    formCO2(clima, world, i, j);
                }

            moveEnergy(clima, clima.energy_ocean_terr, tmpGrid, clima.T_ocean_terr, clima.surfaceTransfer, SURFACE_AND_MARINE_CURRENT, world, true);
            moveEnergy(clima, clima.energy_atmosphere, tmpGrid, clima.T_atmosphere[0, 0, 0], clima.wind[0, 0, 0], WIND, world, true);

            flux.applyCoriolis(clima.wind[0], clima.wind[0], TMdlConst.rotation);
            for (l = 1; l < TMdlConst.atmospheric_layers; l++)
                flux.applyCoriolis(clima.wind[l - 1], clima.wind[l], TMdlConst.rotation);

            followSurface(world, clima.wind[0]);

            for (l = 0; l < TMdlConst.atmospheric_layers; l++)
                moveSteam(clima.wind[l], clima.steam[l], tmpGrid);

            moveCO2(clima.wind[0], clima.co2_tons[0], tmpGrid);

            vulcanoEruption(world, clima);
            singleNuclearBomb(world, clima);
            nuclearWar(world, clima);
            moveAshes(clima.wind[0, 0, 0], clima.ashes_pct[0, 0], tmpGrid);
            ashesFallDown(world, clima);

            for (j = 0; j < 180; j++)
                for (i = 0; i < 360; i++)
                {
                    for (l = TMdlConst.atmospheric_layers - 1; l > 0; l--)    // move steam from layer l to layer l-1
                        moveSteamDown(clima, world, s, l, i, j);
                    rainSteam(clima, world, i, j);
                    waterOrIce(clima, world, i, j);
                    absorbCO2(clima, world, s, i, j);
                    updateOutgoingEnergyOnCellGrid(clima, world, s, earthDeclination, i, j);
                }

            moveWaterDownToOcean(world, clima, clima.water_surface, tmpGrid);
            stepTime(t, s);
#endif
            throw new NotImplementedException();
        }


        public static void mainSimDayLoop(TWorld world, TClima clima, TSolarSurface s, TTime t, double[,] tmpGrid, double earthDeclination)
        {
#if TODO
            computeSolarSurface(s, t, earthDeclination);
            increasePopulation(clima, 1);
            printEarthStatus(t.year, t.day, Round(t.hour), clima, world);
            printPlanet(t.year, t.day, Round(t.hour), clima, world, false, true);
            if (t.day % TSimConst.decrease_rain_times == 0) decreaseRainTimes(clima);
#endif
            throw new NotImplementedException();
        }
    }
}
