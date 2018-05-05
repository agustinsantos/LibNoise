using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibClimate
{
    public class co2cycle
    {
        public static void formCO2(TClima clima, TWorld w, long i, long j)
        {
            if (!TSimConst.population) return;
            if (!TSimConst.energy_source_oil) return;
            clima.co2_tons[i, j] = clima.co2_tons[i, j] +
                                    TSimConst.co2_production_per_human_per_year / (365 / 24 / 15 * TSimConst.degree_step) *
                                    clima.population[i, j];
        }

        public static void moveCO2(short[,] wind, double[,] co2, double[,] copyCO2)
        {
            flux.moveParticlesInAtm(wind, co2, copyCO2);
        }

        public static void absorbCO2(TClima clima, TWorld w, TSolarSurface s, long i, long j)
        {
            double absorption, rain_times;

            if (clima.isIce[i, j] || clima.co2_tons[i, j] == 0 || !statechanges.isInSunlight(i, j, s)) return;

            if (w.isOcean[i, j])
                // absorption scaled over 12 h per day and over 3 quarters of earth surface
                absorption = TSimConst.co2_absorption_ocean / (365 / 12 / 15 * TSimConst.degree_step) / (360 * 180 * 3 / 4);
            else
            {
                // absorption scaled over 12 h per day and over 1 quarter of earth surface
                absorption = TSimConst.co2_absorption_vegetation / (365 / 12 / 15 * TSimConst.degree_step) / (360 * 180 * 1 / 4);
                // jungle absorbs more than desert
                rain_times = clima.rain_times[i, j];
                if (rain_times > 5) rain_times = 5;

                absorption = absorption * rain_times;
            }

            clima.co2_tons[i, j] = clima.co2_tons[i, j] - absorption;
            if (clima.co2_tons[i, j] < 0) clima.co2_tons[i, j] = 0;
        }

        public static void increasePopulation(TClima clima, long days)
        {
            double base_, exp, factor;

            base_ = 1;
            exp = 1 + (TSimConst.population_increase_pct * days / 365);
            factor = (double)Math.Pow((double)base_, (double)exp);
            for (int j = 0; j < 180; j++)
                for (int i = 0; i < 360; i++)
                    clima.population[i, j] = clima.population[i, j] * factor;
        }
    }
}
