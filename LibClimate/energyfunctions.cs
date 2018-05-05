using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibClimate
{
    public class energyfunctions
    {
        public static double computeEarthDeclination(int day)
        {
            double eta, d;

            if (day < 1 || day > 365)
                throw new Exception("Day has to be between 1 and 365 but was " + day);

            if (TMdlConst.revolution)
            {
                // from
                // http://mathforum.org/library/drmath/view/56478.html
                // and http://www.analemma.com/Pages/framesPage.html
                // angle of mean sun
                //{
                ////TODO: check deg/rad problem here
                //eta = 360/365.24 * (day-80);
                //if (eta>=270 ) eta=eta-360;
                //if (eta>=90 ) eta = eta -180;
                //d = 0.917408 * sin(eta*Math.PI/180);
                //return d*180/Math.PI;
                //}
                // Old version
                // +10.5 is to reach the maximum inclination at the 22 of June, the summer solstice
                return -Math.Sin(day / 365.0 * 2.0 * Math.PI + Math.PI / 2.0 + 10.5 / 365.0 * 2.0 * Math.PI) * TPhysConst.earth_inclination_on_ecliptic;
            }
            else
                return TPhysConst.earth_inclination_on_ecliptic;
        }


        public static double computeDayLenghtDuration(double earthDeclination, int j, TTime t)
        {
            double lat, latdeg, A, B, C, d, minusboverc;

            // from
            // http://mathforum.org/library/drmath/view/56478.html
            //Date: 09/22/98 at 12:40:09
            //From: Doctor Rick
            //Subject: Re: Geometry, hours of daylight

            //Hi, Hilde. What you"re asking for isn"t simple. I can show you the
            //geometry of the hours of daylight, but it won"t be complete, and
            //sunrise and sunset times involve additional complications.

            //The Analemma Web site gives explanations and math for the declination
            //of the sun (which you will need for my calculation below) and for the
            //"equation of time," which tells when noon really occurs. Sunrise and
            //sunset will be equal time intervals before and after noon, but noon is
            //not at 12:00:

            //   http://www.analemma.com/Pages/framesPage.html

            //Here is a brief explanation of the length of the day.

            //On any given day, the sun is at a particular declination (which is the
            //latitude of the point on earth where the sun is directly overhead).
            //The terminator (the line connecting all places where the sun is
            //setting or rising) is a circle that is tilted from a north-south plane
            //by an angle equal to the declination. Here is a side view:

            //                 ****|****   Terminator      SUN
            //              *\*    |    ***
            //            **  \ dec|       **
            //          **     \ B |    C    **
            //         *--------\--+-----------o You
            //        *          \ |A      /    *
            //       *            \|   /   lat   *
            //       *             +--------------
            //       *             |\            *
            //        *            | \          *
            //         *           |  \        *
            //          **         |   \     **
            //            **       |    \  **
            //              ***    |    *\*
            //                 ****|****

            //The terminator is shown edge-on, at the angle of declination from the
            //vertical. Your latitude is shown as a horizontal line. We are
            //interested in the point of intersection of the terminator and your
            //latitude, because that is where you experience sunrise or sunset.

            //If we call the radius of the earth 1 unit, then distance A is sin(lat).
            //Therefore distance B is sin(lat)*tan(dec). Distance C, the radius of
            //the latitude circle, is cos(lat).

            //Now look down from the north on your latitude circle:

            //                 oooo|oooo
            //              **o    |    ooo
            //            **| \    |       oo
            //          **  |  \C  |         oo
            //         *    |   \  |           o
            //        *     |    \ |            o
            //       *      |  B  \|             o
            //       * night+------+      day    o
            //       *      |     /|             o
            //        *     |    / |            o
            //         *    |   /  |           o
            //          **  |  /   |         oo
            //            **| /    |       oo
            //              **o    |    ooo
            //                 oooo|oooo

            //The angle between sunrise and sunset (on the day side of the angle) is

            //  d = 2 cos^-1(-B/C)
            //    = 2 cos^-1(-tan(dec)tan(lat))

            //The length of the day is 24 hours * d/360. But remember to consult the
            //Web site above to see why this isn"t quite right.

            //- Doctor Rick, The Math Forum
            //  http://mathforum.org/dr.math/


            lat = Conversion.YtoLat(j);
            latdeg = lat * Math.PI / 180;

            try
            {
                d = 2 * Math.Acos(-Math.Tan(earthDeclination * Math.PI / 180) * Math.Tan(latdeg));
                d = d / Math.PI * 180;
                return 24 * d / 360;

            }
            catch (Exception e)
            {
                // TODO: could be done at the hourly level here
                if (t.day >= 79 && t.day <= 262)
                {
                    // Winter in South Hemisphere
                    if (lat > 0)
                        return 0;  // whole day sun
                    else
                        return Constants.FULL_NIGHT; //  to trick it into deep night
                }
                else
                {
                    // Winter in North Hemisphere
                    if (lat > 0)
                        return Constants.FULL_NIGHT;
                    else
                        return 0; // whole day sun
                }
            }
        }



        public static void computeSolarSurface(TSolarSurface s, TTime t, double earthDeclination)
        {
            int j, shift;
            double daylengthhour;

            for (j = 0; j < 180; j++)
            {
                daylengthhour = computeDayLenghtDuration(earthDeclination, j, t); // for fix daylength 12;
                if (daylengthhour == Constants.FULL_NIGHT)
                {
                    s.degstart[j] = Constants.FULL_NIGHT;
                    s.degend[j] = Constants.FULL_NIGHT;
                }
                else
                {
                    shift = (int)Math.Round(daylengthhour * 15 / 2);   // each hour are 15 degrees (360/24)
                    s.degstart[j] = 179 - shift;
                    s.degend[j] = 179 + shift;

                    s.degstart[j] = s.degstart[j] + TMdlConst.initDegreeSunlight;
                    s.degend[j] = s.degend[j] + TMdlConst.initDegreeSunlight; // 12 hours which are 15 degrees a part
                    if (s.degstart[j] >= 360) s.degstart[j] = s.degstart[j] - 360;
                    if (s.degend[j] >= 360) s.degend[j] = s.degend[j] - 360;
                }  // else FULL_NIGHT
            } // for
        }

        public static double computeEnergyFactorWithAngle(int i, int j, double earthInclination)
        {
            double angle, factor;

            angle = Math.Abs(Conversion.YtoLat(j) - earthInclination);
            factor = Math.Sin((90 - angle) / 90 * Math.PI / 2);
            if (factor < 0) factor = 0;
            return factor;
        }

        public static double computeEnergyFromSunOnSquare(int i, int j, double earthInclination, TClima clima, TWorld w)
        {
            double reflection, angle;

            reflection = TMdlConst.albedo + clima.humidity[i, j] * TSimConst.cloud_reflection_pct + clima.ashes_pct[i, j];
            if (reflection > 1) reflection = 1;
            return TPhysConst.SolarConstant * (1 - reflection) *
                      computeEnergyFactorWithAngle(i, j, earthInclination)
                      / Math.Pow(TMdlConst.distanceFromSun, 2)
                      * TSimConst.hour_step
                      * w.area_of_degree_squared[j];

        }

        public static void spreadEnergyOnAtmosphereAndTerrain(TClima clima, double energy, int i, int j)
        {
            clima.energy_atmosphere[i, j] = energy * (TMdlConst.tau_visible) + clima.energy_atmosphere[i, j];
            clima.energy_ocean_terr[i, j] = energy * (1 - TMdlConst.tau_visible) + clima.energy_ocean_terr[i, j];
        }

        public static void updateTemperature(TClima clima, TWorld w, int i, int j)
        {
            double divAtmosphere,
            divOcean,
            divTerrain,
            weight;

            if (w.isOcean[i, j])
                weight = TPhysConst.weight_on_surface_at_sea_level;
            else
                weight = statechanges.weightOnAltitudeProQuadrateMeter(w.elevation[i, j], i, j, w);

            divAtmosphere = (TPhysConst.cp_air * weight * w.area_of_degree_squared[j]);
            if (divAtmosphere != 0)
                clima.T_atmosphere[0, i, j] = clima.energy_atmosphere[i, j] / divAtmosphere;
            else throw new Exception("divAtmosphere is zero!");

            if (w.isOcean[i, j])
            {
                divOcean = (TPhysConst.cp_water * Math.Abs(w.elevation[i, j]) * w.area_of_degree_squared[j] * TPhysConst.density_water) +
                            (TPhysConst.cp_earth * (w.elevation[i, j] + TSimConst.earth_crust_height) * w.area_of_degree_squared[j] * TPhysConst.density_earth);
                if (divOcean != 0)
                    clima.T_ocean_terr[i, j] = clima.energy_ocean_terr[i, j] / divOcean;
                else clima.T_ocean_terr[i, j] = 0;
            }
            else
            {
                // terrain
                divTerrain = TPhysConst.cp_earth * (w.elevation[i, j] + TSimConst.earth_crust_height) * w.area_of_degree_squared[j] * TPhysConst.density_earth;
                if (divTerrain != 0)
                    clima.T_ocean_terr[i, j] = clima.energy_ocean_terr[i, j] / divTerrain;
                else throw new Exception("divTerrain is zero!");

            }
        }


        public static void radiateTerrestrialEnergy(TClima clima, TWorld w, int i, int j)
        {
            double
              multiple_earth,
              multiple_ocean;

            // earth constantly radiates gravitational energy to the terrain
            clima.T_ocean_terr[i, j] = clima.T_ocean_terr[i, j] + TSimConst.deltaTterrestrialEnergy * (1 - TMdlConst.tau_infrared) * (TSimConst.degree_step / 15);

            // earth part
            multiple_earth = (TPhysConst.cp_earth * (w.elevation[i, j] + TSimConst.earth_crust_height) * w.area_of_degree_squared[j] * TPhysConst.density_earth);

            if (w.isOcean[i, j])
                multiple_ocean = (TPhysConst.cp_water * Math.Abs(w.elevation[i, j]) * w.area_of_degree_squared[j] * TPhysConst.density_water);
            else
                multiple_ocean = 0;

            clima.energy_ocean_terr[i, j] = clima.T_ocean_terr[i, j] * (multiple_earth + multiple_ocean);
        }

        public static void exchangeEnergyBetweenAtmAndTerrain(TClima clima, TWorld w, int i, int j)
        {
            double energy_moved,
                finalEnergy;

            if (clima.T_ocean_terr[i, j] > clima.T_atmosphere[0, i, j])
            {
                // radiate energy from terrain to atmosphere
                energy_moved = TPhysConst.stefan_boltzmann * Math.Pow(clima.T_ocean_terr[i, j], 4)
                                * w.area_of_degree_squared[j] * TSimConst.hour_step * (1 / TSimConst.exchange_atm_terr);
                if (energy_moved < 0) throw new Exception("Energy radiated from terrain to atmosphere is negative");
                finalEnergy = clima.energy_ocean_terr[i, j] - energy_moved;
                if (finalEnergy < 0) return;

                clima.energy_ocean_terr[i, j] = finalEnergy;
                clima.energy_atmosphere[i, j] = clima.energy_atmosphere[i, j] + energy_moved;
            }
            else
            {
                // radiate energy from atmosphere to terrain
                energy_moved = TPhysConst.stefan_boltzmann * Math.Pow(clima.T_atmosphere[0, i, j], 4)
                * w.area_of_degree_squared[j] * TSimConst.hour_step * (1 / TSimConst.exchange_atm_terr);
                if (energy_moved < 0) throw new Exception("Energy radiated from atmosphere to terrain is negative");

                finalEnergy = clima.energy_atmosphere[i, j] - energy_moved;
                if (finalEnergy < 0) return;

                clima.energy_atmosphere[i, j] = finalEnergy;
                clima.energy_ocean_terr[i, j] = clima.energy_ocean_terr[i, j] + energy_moved;
            }
        }



        public static double computeRadiatedEnergyIntoSpace(TClima clima, TWorld w, int i, int j)
        {
            double
             isolation,
             co2_isolation;

            co2_isolation = clima.co2_tons[i, j] / 1E7; // where it is red on plot it is 1
            if (co2_isolation > 1) co2_isolation = 1;

            isolation = clima.humidity[i, j] * TSimConst.cloud_isolation_pct + TSimConst.co2_isolation_pct * co2_isolation;
            if (isolation > 1) isolation = 1;
            return (1 - isolation) * TPhysConst.stefan_boltzmann * Math.Pow(clima.T_atmosphere[0, i, j], 4)
                               * w.area_of_degree_squared[j] * TSimConst.hour_step * (1 / TSimConst.radiation_hours);
        }

        public static void radiateEnergyIntoSpace(TClima clima, TWorld w, int i, int j)
        {
            double energy_radiated,
            finalEnergy;


            energy_radiated = computeRadiatedEnergyIntoSpace(clima, w, i, j);
            if (energy_radiated < 0) throw new Exception("Energy radiated into space is negative");
            finalEnergy = clima.energy_atmosphere[i, j] - energy_radiated;
            if (finalEnergy < 0) return;
            clima.energy_atmosphere[i, j] = finalEnergy;
        }


        public static void updateIncomingEnergyOnCellGrid(TClima clima, TWorld w, TSolarSurface sSurface, double earthInclination, int i, int j)
        {
            double energyIn;

            if (statechanges.isInSunlight(i, j, sSurface))
            {
                energyIn = computeEnergyFromSunOnSquare(i, j, earthInclination, clima, w);
                spreadEnergyOnAtmosphereAndTerrain(clima, energyIn, i, j);
                updateTemperature(clima, w, i, j);
            }

            radiateTerrestrialEnergy(clima, w, i, j);
            updateTemperature(clima, w, i, j);
        }

        public static void updateOutgoingEnergyOnCellGrid(TClima clima, TWorld w, TSolarSurface sSurface, double earthInclination, int i, int j)
        {
            updateTemperature(clima, w, i, j);
            exchangeEnergyBetweenAtmAndTerrain(clima, w, i, j);
            updateTemperature(clima, w, i, j);
            radiateEnergyIntoSpace(clima, w, i, j);
            updateTemperature(clima, w, i, j);
        }
    }
}
