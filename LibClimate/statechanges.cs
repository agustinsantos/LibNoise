using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibClimate
{
    public class statechanges
    {
        public static void WaterOrIce(TClima clima, TWorld w, long i, long j)
        {
            clima.isIce[i, j] = (clima.T_ocean_terr[i, j] <= TPhysConst.kT_ice);
            if (!w.isOcean[i, j]) clima.isIce[i, j] = clima.isIce[i, j] && (clima.water_surface[i, j] > 0);
        }


        public static double evaporatePercentage(TClima clima, double T_param, long i, long j)
        {
            double T;

            // a simple linear relation between 10 and 100 degree
            T = Conversion.KtoC(T_param);
            if (T <= TSimConst.evaporation_start_temp) return 0;
            else
                if (T >= TSimConst.full_evaporation_temp) return 1;
                else
                    return (T - TSimConst.evaporation_start_temp) / (TSimConst.full_evaporation_temp - TSimConst.evaporation_start_temp);
        }

        public static bool isInSunlight(long i, long j, TSolarSurface sSurface)
        {
            if (sSurface.degstart[j] == Constants.FULL_NIGHT)
            {
                return false;
            }

            if (sSurface.degend[j] > sSurface.degstart[j])
                return (i >= sSurface.degstart[j]) && (i <= sSurface.degend[j]);
            else
                return (i <= sSurface.degend[j]) || (i >= sSurface.degstart[j]);
        }



        public static double weightOnAltitudeProQuadrateMeter(double altitude, long i, long j, TWorld w)
        {
            double p;

            if (altitude < 0) throw new Exception("weightOnAltitude(...) called with negative altitude");
            p = (double)(100 * Math.Pow((44331.514 - (double)altitude) / 11880.516, 1 / 0.1902632));
            return p / TPhysConst.grav_acc;
        }

        public static double thermicGradient(TWorld w, TClima clima, double elevation, double T_initial, long i, long j)
        {
            double altitude;
            double Result;

            if (elevation <= 0) Result = T_initial;
            if (elevation > TSimConst.max_atmosphere_height) elevation = TSimConst.max_atmosphere_height;
            if (clima.humidity[i, j] < 0) throw new Exception("Humidity is negative in thermicGradient(...)");
            Result = T_initial - Math.Abs(elevation) / 100 * (1 - clima.humidity[i, j] * (TSimConst.thermic_gradient_dry - TSimConst.thermic_gradient_wet));

            // outer atmospheric layers might reach zero absolute
            if (Result < 0) Result = 0;

            return Result;
        }

        public static double computeHumidity(TClima clima, TWorld w, long i, long j)
        {
            double maxWater;

            maxWater = 0;
            clima.humidity[i, j] = 0;
            for (int l = 0; l < TMdlConst.atmospheric_layers; l++)
            {
                clima.humidity[i, j] = clima.humidity[i, j] + clima.steam[l, i, j];
                maxWater = maxWater + maxWaterInSteam(clima, w, l, i, j);
            }

            clima.humidity[i, j] = clima.humidity[i, j] / maxWater;
            if (clima.humidity[i, j] > 1) clima.humidity[i, j] = 1;

            return clima.humidity[i, j];
        }



        public static double maxWaterInSteam(TClima clima, TWorld w, int l, long i, long j)
        {
            double
            altitude,
            tCelsius,
            density;

            tCelsius = Conversion.KtoC(clima.T_atmosphere[l, i, j]);

            //Formeln und Tafeln pg 174
            if (clima.isIce[i, j])
            {
                if (tCelsius < -25)
                    density = 0.00035;
                else if (tCelsius < -20)
                    density = 0.00057;
                else if (tCelsius < -15)
                    density = 0.00091;
                else if (tCelsius < -10)
                    density = 0.00139;
                else if (tCelsius < -5)
                    density = 0.00215;
                else
                    density = 0.00325;
            }
            else
            {
                // here approximating with a public static   might speed up
                if (tCelsius <= 16)
                {
                    if (tCelsius <= -10)
                        density = 0.00236;
                    else if (tCelsius <= -5)
                        density = 0.00332;
                    else if (tCelsius <= 0)
                        density = 0.00485;
                    else if (tCelsius <= 2)
                        density = 0.00557;
                    else if (tCelsius <= 4)
                        density = 0.00637;
                    else if (tCelsius <= 6)
                        density = 0.00727;
                    else if (tCelsius <= 8)
                        density = 0.00828;
                    else if (tCelsius <= 10)
                        density = 0.00941;
                    else if (tCelsius <= 12)
                        density = 0.01067;
                    else if (tCelsius <= 14)
                        density = 0.01208;
                    else                                                            //{if (tCelsius<= 16)}  { } 
                        density = 0.01365;
                }
                else
                {
                    if (tCelsius <= 18)
                        density = 0.01539;
                    else if (tCelsius <= 20)
                        density = 0.01732;
                    else if (tCelsius <= 22)
                        density = 0.01944;
                    else if (tCelsius <= 24)
                        density = 0.02181;
                    else if (tCelsius <= 26)
                        density = 0.02440;
                    else if (tCelsius <= 28)
                        density = 0.02726;
                    else if (tCelsius <= 30)
                        density = 0.03039;
                    else if (tCelsius <= 35)
                        density = 0.03963;
                    else if (tCelsius <= 40)
                        density = 0.05017;
                    else if (tCelsius <= 45)
                        density = 0.06545;
                    else if (tCelsius <= 50)
                        density = 0.08300;
                    else if (tCelsius <= 60)
                        density = 0.13000;
                    else if (tCelsius <= 70)
                        density = 0.19800;
                    else if (tCelsius <= 80)
                        density = 0.29300;
                    else if (tCelsius <= 90)
                        density = 0.42400;
                    else if (tCelsius <= 95)
                        density = 0.50450;
                    else if (tCelsius <= 100)
                        density = 0.5977;
                    else if (tCelsius <= 120)
                        density = 1.1220;
                    else if (tCelsius <= 140)
                        density = 1.9670;
                    else if (tCelsius <= 170)
                        density = 4.1220;
                    else if (tCelsius <= 200)
                        density = 7.8570;
                    else if (tCelsius <= 250)
                        density = 19.980;
                    else if (tCelsius <= 300)
                        density = 46.240;
                    else if (tCelsius <= 350)
                        density = 113.60;
                    else
                        density = 328;
                }
            }

            if (w.isOcean[i, j]) altitude = 0; else altitude = w.elevation[i, j];
            return density * weightOnAltitudeProQuadrateMeter(altitude + l * TMdlConst.distance_atm_layers, i, j, w) * w.area_of_degree_squared[j];
        }
    }
}
