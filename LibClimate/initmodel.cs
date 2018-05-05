using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibClimate
{
    public class initmodel
    {
        public static void initModelParameters()
        {
            TMdlConst.Init();
            TInitCond.Init();
            TSimConst.Init();
            TPhysConst.Init();
            TSpecialParam.Init();
        }

        public static void initTime(TTime t, TSolarSurface sSurface)
        {
            initTimeHDY(0, 1, 2000, t, sSurface);
        }

        public static void initTimeHDY(long hour, long day, long year, TTime t, TSolarSurface sSurface)
        {
            if ((hour < 0) || (hour > 23)) throw new Exception("Hour has to lie between 0 and 23");
            if ((day < 1) || (day > 365)) throw new Exception("Day has to lie between 1 and 365");
            if ((year < 0) || (year > 10000)) throw new Exception("Year has to lie between 0 and 10000");

            t.hour = hour;
            t.day = day;
            t.year = year;
        }

        public static void stepTime(TTime t, TSolarSurface sSurface)
        {
            long j;
            t.hour = t.hour + TSimConst.degree_step / 15;
            if (t.hour >= 24)
            {
                t.hour = 0;
                t.day = t.day + 1;
                if (t.day > 365)
                {
                    t.day = 1;
                    t.year = t.year + 1;
                }
            }

            if (TMdlConst.rotation)
            {
                for (j = 90; j < 180; j++)
                {
                    if (sSurface.degstart[j] == Constants.FULL_NIGHT) continue;

                    sSurface.degstart[j] = sSurface.degstart[j] - TSimConst.degree_step * TMdlConst.inverse_rotation;
                    if (sSurface.degstart[j] < 0) sSurface.degstart[j] = sSurface.degstart[j] + 360;
                    if (sSurface.degstart[j] >= 360) sSurface.degstart[j] = sSurface.degstart[j] - 360;

                    sSurface.degend[j] = sSurface.degend[j] - TSimConst.degree_step * TMdlConst.inverse_rotation;
                    if (sSurface.degend[j] < 0) sSurface.degend[j] = sSurface.degend[j] + 360;
                    if (sSurface.degend[j] >= 360) sSurface.degend[j] = sSurface.degend[j] - 360;
                }
            }
        }


        public static void initWorld(TWorld w, String filePath = null)
        {
            string AppPath;

            initModelParameters();
            Conversion.InitConversion(false); // default to linear grid

            if (string.IsNullOrWhiteSpace(filePath))
                AppPath = @"config\planet-elevation.txt"; //ExtractFilePath(ParamStr(0));
            else AppPath = filePath;

            using (StreamReader file = new System.IO.StreamReader(AppPath))
            {
                for (int j = 0; j < 180; j++)
                    for (int i = 0; i < 360; i++)
                    {
                        string str = file.ReadLine();
                        if (!string.IsNullOrWhiteSpace(str.Trim()))
                        {
                            w.elevation[i, j] = double.Parse(str, CultureInfo.InvariantCulture);
                            w.isOcean[i, j] = w.elevation[i, j] <= 0;
                        }
                        else
                            throw new Exception("Problem in file planet-elevation.txt");

                    }
                file.Close();
            }
        }


        public static void initClima(TWorld w, TClima clima, double T_atm, double T_ocean_terr, String filePath = null)
        {
            long x, y, population;
            double kT_atm,
            kT_ocean_terr,
            weight,
            altitude,
            lat,
            lon;

            double h, hStart, hEnd,
            startLat, stopLat,
            area;

            string AppPath, str;

            if (string.IsNullOrWhiteSpace(filePath))
                AppPath = @"config\planet-population-1990.txt"; //ExtractFilePath(ParamStr(0)); 
            else AppPath = filePath;

            // load population
            for (int j = 0; j < 180; j++)
                for (int i = 0; i < 360; i++)
                    clima.population[i, j] = 0;
            using (StreamReader file = new System.IO.StreamReader(AppPath))
            {
                while ((str = file.ReadLine()) != null)
                {
                    if (!string.IsNullOrWhiteSpace(str))
                    {
                        string[] vals = str.Split(";".ToCharArray());
                        // parse string
                        y = Conversion.LatToY(90 - int.Parse(vals[0]));
                        x = Conversion.LonToX(int.Parse(vals[1]) - 180);
                        population = int.Parse(vals[2]);
                        clima.population[x, y] = clima.population[x, y] + population;
                    }
                }
                file.Close();
            }

            co2cycle.increasePopulation(clima, 3650); // the dataset is of 1990, simulation {s in 2000


            // temperature
            // this initialization is very simplistic
            // we assume the earth has only one temperature in atmosphere and one in terrain
            // and distribute the energy over ocean, terrain, atmosphere and ice

            // compute the area of a degree squared
            // a zone on a sphere has area 2*Pi*r*h
            // we divide the zone in 360 degrees
            for (int j = 0; j < 180; j++)
            {
                startLat = Math.Abs(Conversion.YtoLat(j));
                stopLat = Math.Abs(Conversion.YtoLat(j + 1));

                hStart = Math.Cos((90 - startLat) / 360.0 * 2 * Math.PI) * TPhysConst.earth_radius;
                hEnd = Math.Cos((90 - stopLat) / 360.0 * 2 * Math.PI) * TPhysConst.earth_radius;

                h = Math.Abs(hStart - hEnd);
                area = (2 * Math.PI * TPhysConst.earth_radius * h);
                w.area_of_degree_squared[j] = area / 360.0;
                w.length_of_degree[j] = Math.Sqrt(w.area_of_degree_squared[j]);
            }

            for (int j = 0; j < 180; j++)
                for (int i = 0; i < 360; i++)
                {
                    lat = Conversion.YtoLat(j);
                    lon = Conversion.XtoLon(i);

                    clima.T_ocean_terr[i, j] = 15.5 + TPhysConst.absolutezero;
                    clima.T_atmosphere[0, i, j] = 15.5 + TPhysConst.absolutezero;


                    kT_atm = Conversion.CtoK(T_atm + TInitCond.thermic_excursion * Math.Cos(Math.Abs(lat) / 90 * Math.PI / 2));
                    kT_ocean_terr = Conversion.CtoK(T_ocean_terr + TInitCond.thermic_excursion * Math.Cos(Math.Abs(lat) / 90 * Math.PI / 2));

                    // desert belt
                    if ((Math.Abs(lat) <= TInitCond.desert_belt_lat + TInitCond.desert_belt_ext) &&
                           (Math.Abs(lat) >= TInitCond.desert_belt_lat) && (!w.isOcean[i, j]))
                        kT_ocean_terr = kT_ocean_terr + TInitCond.desert_belt_delta_T * Math.Sin(Math.Abs(TInitCond.desert_belt_lat - lat / (TInitCond.desert_belt_ext)) * Math.PI);


                    if (!w.isOcean[i, j])
                    {
                        if (Math.Abs(lat) < 60)
                            kT_ocean_terr = kT_ocean_terr + TInitCond.surface_shift;

                        // thermic gradient adjustment on surface and atmosphere
                        kT_ocean_terr = kT_ocean_terr - TInitCond.thermic_gradient_avg * Math.Abs(w.elevation[i, j] / 100.0);
                        kT_atm = kT_atm - TInitCond.thermic_gradient_avg * Math.Abs(w.elevation[i, j] / 100.0);

                    }
                    else
                    {
                        // thermic gradient adjustment on sea depth and atmopshere
                        kT_ocean_terr = kT_ocean_terr - TInitCond.thermic_gradient_sea * Math.Abs(w.elevation[i, j] / 1000.0);
                        kT_atm = kT_atm - TInitCond.thermic_gradient_sea * Math.Abs(w.elevation[i, j] / 1000.0);
                    }

                    // insulation
                    if (lon > -180 && lon <= 0)
                    {
                        // heat terrain up and cool ocean down
                        if (w.isOcean[i, j])
                            kT_ocean_terr = kT_ocean_terr - TInitCond.ocean_shift;
                        else
                            kT_ocean_terr = kT_ocean_terr + TInitCond.terrain_shift;

                    }
                    else
                    {
                        // heat ocean up, cool terrain down
                        if (w.isOcean[i, j])
                            kT_ocean_terr = kT_ocean_terr + TInitCond.ocean_shift;
                        else
                            kT_ocean_terr = kT_ocean_terr - TInitCond.terrain_shift;

                    }
                    clima.T_ocean_terr[i, j] = kT_ocean_terr;
                    clima.T_atmosphere[0, i, j] = kT_atm;

                    // steam and rain
                    for (int l = 0; l < TMdlConst.atmospheric_layers; l++)
                        clima.steam[l, i, j] = 0;
                    clima.rain[i, j] = false;
                    clima.water_surface[i, j] = 0;
                    clima.rain_times[i, j] = 0;

                    clima.humidity[i, j] = 0;
                    clima.rain_times[i, j] = 0;

                    // ashes and CO2
                    clima.ashes_pct[i, j] = 0;
                    clima.co2_tons[i, j] = 0;

                    // atmospheric layers
                    for (int l = 1; l < TMdlConst.atmospheric_layers; l++)
                        clima.T_atmosphere[l, i, j] = statechanges.thermicGradient(w, clima, l * TMdlConst.distance_atm_layers, clima.T_atmosphere[0, i, j], i, j);


                    clima.isIce[i, j] = (clima.T_ocean_terr[i, j] <= TPhysConst.kT_ice);


                    // update energies
                    // deltaQ = cp * m * deltaT
                    if (w.isOcean[i, j])
                        altitude = 0;
                    else
                        altitude = w.elevation[i, j];

                    // energies
                    weight = statechanges.weightOnAltitudeProQuadrateMeter(altitude, i, j, w);
                    clima.energy_atmosphere[i, j] = clima.T_atmosphere[0, i, j] * TPhysConst.cp_air * weight * w.area_of_degree_squared[j];
                    // terrain
                    clima.energy_ocean_terr[i, j] = clima.T_ocean_terr[i, j] * TPhysConst.cp_earth * (w.elevation[i, j] + TSimConst.earth_crust_height) * w.area_of_degree_squared[j] * TPhysConst.density_earth;

                    if (w.isOcean[i, j])
                        clima.energy_ocean_terr[i, j] = clima.energy_ocean_terr[i, j] + clima.T_ocean_terr[i, j] * TPhysConst.cp_water * Math.Abs(w.elevation[i, j]) * w.area_of_degree_squared[j] * TPhysConst.density_water;


                }

        }
    }
}