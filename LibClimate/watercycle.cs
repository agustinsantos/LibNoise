using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibClimate
{
    class watercycle
    {
        public static void formSteam(TClima clima, TWorld w, TSolarSurface s, long i, long j, long day)
        {
            double energyToBoil, evaporationPct, evaporationQty;

            if (clima.isIce[i, j] || statechanges.isInSunlight(i, j, s))
                return;

            evaporationPct = statechanges.evaporatePercentage(clima, clima.T_ocean_terr[i, j], i, j);
            if (evaporationPct == 0)
                return;

            // steam over ocean
            if (w.isOcean[i, j])
            {
                // check how much steam we could form
                evaporationQty = (statechanges.maxWaterInSteam(clima, w, 0, i, j) - clima.steam[0, i, j]) * (1 / TSimConst.steam_hours)
                                   * evaporationPct;
            }
            else
            // steam produced by river and lakes
            {
                if (clima.water_surface[i, j] == 0)
                    return;

                // vegetation (where it rains more than one time, slows down evaporation)
                if (clima.rain_times[i, j] > 0)
                    evaporationQty = clima.water_surface[i, j] / clima.rain_times[i, j] * evaporationPct * (1 / TSimConst.steam_hours);
                else
                    evaporationQty = clima.water_surface[i, j] * evaporationPct * (1 / TSimConst.steam_hours);

                if (day % TSimConst.decrease_rain_times == 0)
                {
                    clima.rain_times[i, j]--;
                    if (clima.rain_times[i, j] < 0)
                        clima.rain_times[i, j] = 0;
                }

            }


            if (evaporationQty > 0) // here we will rain in a later public static void  call
            {
                // energy to boil the steam
                if (TPhysConst.kT_boil - clima.T_atmosphere[0, i, j] > 0)
                    energyToBoil = TPhysConst.cp_steam * evaporationQty * (TPhysConst.kT_boil - clima.T_atmosphere[0, i, j]);
                else
                    energyToBoil = 0;


                if (clima.energy_ocean_terr[i, j] >= energyToBoil)
                // the atmosphere has enough energy to carry the steam
                {
                    clima.steam[0, i, j] = clima.steam[0, i, j] + evaporationQty;
                    clima.energy_ocean_terr[i, j] = clima.energy_ocean_terr[i, j] - energyToBoil;

                    if (!w.isOcean[i, j])
                        clima.water_surface[i, j] = clima.water_surface[i, j] - evaporationQty;
                }

            }
        }

        public static void moveSteamUp(TClima clima, TWorld w, TSolarSurface s, int l, long i, long j)
        {
            double availableSteam,
            maxSteam,
            transferEnergy,
            evaporationPct;

            // let's compute first the temperature on the upper layer based on the thermic gradient
            clima.T_atmosphere[l, i, j] = statechanges.thermicGradient(w, clima, l * TMdlConst.distance_atm_layers, clima.T_atmosphere[0, i, j], i, j);
            // we do not add altitude here, as altitude is already in clima.T_atmosphere[0, i, j]
            // if there is ice on ground or no steam to push up we exit
            if (clima.isIce[i, j] || clima.steam[l - 1, i, j] == 0)
                return;
            evaporationPct = statechanges.evaporatePercentage(clima, clima.T_atmosphere[l - 1, i, j], i, j);
            if (evaporationPct == 0) return;

            // how much steam could stay in the upper layer l?
            maxSteam = statechanges.maxWaterInSteam(clima, w, l, i, j);
            // how much steam is available for transfer
            availableSteam = Math.Min(clima.steam[l - 1, i, j], maxSteam) * (1 / TSimConst.steam_hours) * evaporationPct;
            // is there enough energy to perform the transfer to the upper layer?
            transferEnergy = availableSteam * TPhysConst.grav_acc * TMdlConst.distance_atm_layers;
            if (clima.energy_atmosphere[i, j] > transferEnergy)
            {
                // let's move it up
                clima.energy_atmosphere[i, j] = clima.energy_atmosphere[i, j] - transferEnergy;
                clima.steam[l - 1, i, j] = clima.steam[l - 1, i, j] - availableSteam;
                clima.steam[l, i, j] = clima.steam[l, i, j] + availableSteam;
            }

        }


        public static void moveSteamDown(TClima clima, TWorld w, TSolarSurface s, int l, long i, long j)
        {
            double
              transferSteam,
              maxSteam,
              transferEnergy;

            // recalculate temperature
            clima.T_atmosphere[l, i, j] = statechanges.thermicGradient(w, clima, l * TMdlConst.distance_atm_layers, clima.T_atmosphere[0, i, j], i, j);
            if (clima.steam[l, i, j] == 0) return;

            // how much steam could stay in the upper layer l
            maxSteam = statechanges.maxWaterInSteam(clima, w, l, i, j);
            // how much steam has to be transferred down
            transferSteam = (clima.steam[l, i, j] - maxSteam) * (1 / TSimConst.steam_hours);
            if (transferSteam < 0) return;
            // energy which is given back to atmosphere
            transferEnergy = transferSteam * TPhysConst.grav_acc * TMdlConst.distance_atm_layers;

            // let's move it dowm
            clima.energy_atmosphere[i, j] = clima.energy_atmosphere[i, j] + transferEnergy;
            clima.steam[l, i, j] = clima.steam[l, i, j] - transferSteam;
            clima.steam[l - 1, i, j] = clima.steam[l - 1, i, j] + transferSteam;
        }

        public static void moveSteam(short[,] wind, double[,] steam, double[,] copySteam)
        {
            flux.moveParticlesInAtm(wind, steam, copySteam);
        }

        public static void rainSteam(TClima clima, TWorld w, long i, long j)
        {
            double availableSteam,
        maxWaterInAir,
        thermicEnergy;

            maxWaterInAir = statechanges.maxWaterInSteam(clima, w, 0, i, j);

            availableSteam = (clima.steam[0, i, j] - maxWaterInAir) * (1 / TSimConst.rain_hours);
            if (availableSteam < 0)
            {
                clima.rain[i, j] = false;
            }
            else
            {

                // drop the exceeding steam in rain
                if (!w.isOcean[i, j])
                    clima.water_surface[i, j] = clima.water_surface[i, j] + availableSteam;
                clima.rain_times[i, j]++;
                clima.rain[i, j] = true;
                clima.steam[0, i, j] = clima.steam[0, i, j] - availableSteam;


                // assumption: thermic energy and potential energy of clouds
                // are given back to terrain as energy of movement
                if ((TPhysConst.kT_boil - clima.T_atmosphere[0, i, j]) > 0)
                    thermicEnergy = availableSteam * TPhysConst.cp_steam * (TPhysConst.kT_boil - clima.T_atmosphere[0, i, j]);
                else
                    thermicEnergy = 0;

                // give the thermic energy and potential energy to the terrain and atmosphere
                // clima.energy_atmosphere[i, j] = clima.energy_atmosphere[i, j] + potEnergy;
                clima.energy_ocean_terr[i, j] = clima.energy_ocean_terr[i, j] +
                                                 thermicEnergy;

            }
            // compute humidity
            statechanges.computeHumidity(clima, w, i, j);
        }

        public static void decreaseRainTimes(TClima clima)
        {
            long i, j;

            for (j = 0; j < 180; j++)
                for (i = 0; i < 360; i++)
                    if (clima.rain_times[i, j] > 0)
                        clima.rain_times[i, j]--;
        }
    }
}
