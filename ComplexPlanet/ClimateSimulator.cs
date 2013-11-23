using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplexPlanet
{
    public enum Clim : byte
    {
        C_OCEAN = 0,
        C_BARELAND = 1,
        C_MOUNTAIN = 2,
        C_OCEANICE = 3,
        C_TUNDRA = 4,
        C_STEPPE = 5,
        C_DESERT = 6,
        C_SAVANA = 7,
        C_DECIDUOUS = 8,
        C_JUNGLE = 9,
        C_SWAMP = 10,
        C_LANDICE = 11,
        C_BOREAL = 12

        //C_OCEAN = 0,
        //C_OCEANICE,
        //C_SWAMP,
        //C_BARELAND,
        //C_LAKERIVER,
        //C_MOUNTAIN,
        //C_JUNGLE, // Af
        //C_DECIDUOUS, // Cf
        //C_BOREAL, // Df
        //C_TROPICWETDRY, // Aw
        //C_MONSOON, // Am
        //C_GRASSLAND, // Cw
        //C_MEDITERRANEAN, // Cs
        //C_COLDDRYWINTER, // Dw
        //C_SAVANA,
        //C_DESERT, // Bw
        //C_STEPPE,
        //C_TUNDRA, // Et
        //C_LANDICE
    }

    public enum BIOME 
    {
        // WATER
        OCEAN = 0,
        OCEANICE,
        COOL_AQUATIC,
        WARM_AQUATIC,
        TROPICAL_AQUATIC,
        //LAND, based on Holdridge Life Zones
        //                             biotemperature(C) precipitation (cm)                                                
        POLAR_DESERT,               // <1.5              6.25-75
        SUBPOLAR_DRY_TUNDRA,        // 1.5-3             6.25-12.5
        SUBPOLAR_MOIST_TUNDRA,      // 1.5-3             12.5-25
        SUBPOLAR_WET_TUNDRA,        // 1.5-3             25-50
        SUBPOLAR_RAIN_TUNDRA,       // 1.5-3             50-100
        BOREAL_DESERT,              // 3-6               6.25-12.5
        BOREAL_DRY_SCRUB,           // 3-6               12.5-25
        BOREAL_MOIST_FOREST,        // 3-6               25-50
        BOREAL_WET_FOREST,          // 3-6               50-100
        BOREAL_RAIN_FOREST,         // 3-6               100-200
        COOL_TEMPERATE_DESERT,      // 6-12              6.25-12.5
        COOL_TEMPERATE_DESERT_SCRUB,// 6-12              12.5-25
        COOL_TEMPERATE_STEPPE,      // 6-12              25-50
        COOL_TEMPERATE_MOIST_FOREST,// 6-12              50-100
        COOL_TEMPERATE_WET_FOREST,  // 6-12              100-200
        COOL_TEMPERATE_RAIN_FOREST, // 6-12              200-400
        WARM_TEMPERATE_DESERT,      // 12-18             6.25-12.5
        WARM_TEMPERATE_DESERT_SCRUB,// 12-18             12.5-25
        WARM_TEMPERATE_THORN_SCRUB, // 12-18             25-50
        WARM_TEMPERATE_DRY_FOREST,  // 12-18             50-100
        WARM_TEMPERATE_MOIST_FOREST,// 12-18             100-200
        WARM_TEMPERATE_WET_FOREST,  // 12-18             200-400
        WARM_TEMPERATE_RAIN_FOREST, // 12-18             400-800
        SUBTROPICAL_DESERT,         // 18-24             6.25-12.5
        SUBTROPICAL_DESERT_SCRUB,   // 18-24             12.5-25
        SUBTROPICAL_THORN_WOODLAND, // 18-24             25-50
        SUBTROPICAL_DRY_FOREST,     // 18-24             50-100
        SUBTROPICAL_MOIST_FOREST,   // 18-24             100-200
        SUBTROPICAL_WET_FOREST,     // 18-24             200-400
        SUBTROPICAL_RAIN_FOREST,    // 18-24             400-800
        TROPICAL_DESERT,            // >24               6.25-12.5
        TROPICAL_DESERT_SCRUB,      // >24               12.5-25
        TROPICAL_THORN_WOODLAND,    // >24               25-50
        TROPICAL_VERY_DRY_FOREST,   // >24               50-100
        TROPICAL_DRY_FOREST,        // >24               100-200
        TROPICAL_MOIST_FOREST,      // >24               200-400
        TROPICAL_WET_FOREST,        // >24               400-800
        TROPICAL_RAIN_FOREST        // >24                +800
    }

    public enum DNDCLIMATE_E
    {
        D_WARM_SWAMP = 0,
        D_WARM_FOREST,
        D_WARM_PLAIN,
        D_WARM_DESERT,
        D_WARM_HILL,
        D_WARM_MOUNTAIN,
        D_WARM_AQUATIC,
        D_TEMP_SWAMP,
        D_TEMP_FOREST,
        D_TEMP_PLAIN,
        D_TEMP_DESERT,
        D_TEMP_HILL,
        D_TEMP_MOUNTAIN,
        D_TEMP_AQUATIC,
        D_COLD_SWAMP,
        D_COLD_FOREST,
        D_COLD_PLAIN,
        D_COLD_DESERT,
        D_COLD_HILL,
        D_COLD_MOUNTAIN,
        D_COLD_AQUATIC,
        D_OCEANICE
    }

    public class ClimateSimulator
    {
        /// <summary>
         private enum Pressure
        {
            PR_NORMAL = 0,
            PR_LOW = 1,
            PR_HIGH = 2,
            PR_HEQ = 3
        }

        [Flags]
        private enum WindDir : byte
        {
            N = 1,
            S = 2,
            E = 4,
            W = 8
        }

        private struct Wind
        {
            public WindDir dir;
            public float intensity;
        }

        private float[,] heightmap;
        private float[,] countland;
        private float[,] heatmap;
        private float[,] pressuremap;
        private float[,] rainfallmap;
        private float[,] windmap;
        private Wind[,] wind;
        private float[,] cl;
        private float[,] dndclim;
        private float[,] debug;
        private int width;
        private int height;
        private float factor;

        public ClimateSimulator(float[,] hm)
        {
            heightmap = hm;
            width = heightmap.GetLength(0);
            height = heightmap.GetLength(1);
            factor = (float)width / (float)height;
        }

        public void ComputeClimate()
        {
            ComputeCountLand();
            ComputeHeatmap(0);
            ComputePressureMap();
            ComputeWind();
            ComputeRain();
            ComputeBiomes();
        }

        public float[,] CountLandMap
        {
            get { return countland; }
        }

        public float[,] PressureMap
        {
            get { return pressuremap; }
        }

        public float[,] HeatMap
        {
            get { return heatmap; }
        }

        public float[,] RainfallMap
        {
            get { return rainfallmap; }
        }

        public float[,] WindMap
        {
            get { return windmap; }
        }

        public float[,] BiomeMap
        {
            get { return cl; }
        }
        public float[,] BiomeMap2
        {
            get { return dndclim; }
        }

        public float[,] DebugMap
        {
            get { return debug; }
        }
        #region BIOME CLASIFICATION
        /// <summary>
        /// Parameters affecting climate determination
        /// 
        /// ICEBERGK  - Default 263.  If an ocean square is below this temperature
        ///             (measured in deg Kelvin) all year round, then the ocean square
        ///             is icebergs.
        /// TEMPCUT   - Default is the vector (0 40 90 120).  The climate array found
        ///             in climate.c/climkey is 4 x 5; the first index is based on
        ///             average annual temperature.  The temperature is relative, based
        ///             on the range 0..255; this vector determines the cutoff points.
        ///             For example, with the default vector, a scaled temperature of 20
        ///             falls into the first "bin" and 121 falls into the fourth.
        /// RAINCUT   - Default is the vector (40 60 110 160 180).  The second index of
        ///             the climate array is based on average annual rainfall, scaled into
        ///             the range 0..255.  This vector determines the cutoff points.  For
        ///             example, rainfall of 35 falls into the first "bin".
        /// MTDELTA   - Default 20.  This is the amount, in degrees Farenheit, by which
        ///             temperature in the mountains is decreased before the climate 
        ///             lookup is performed.
        /// </summary>
        public static int[] tempcut = { -40, 10, 40, 100, 120 }, raincut = { 40, 65, 110, 160, 180 };
        /// This array is the heart of the climate routine; temperature increases
        /// going down the array, and rainfall increases going from left to right. 
        /// </summary>
        private static Clim[,] climkey = { { Clim.C_OCEANICE, Clim.C_OCEANICE, Clim.C_OCEANICE, Clim.C_OCEANICE, Clim.C_OCEANICE },
                                           { Clim.C_TUNDRA, Clim.C_TUNDRA, Clim.C_TUNDRA, Clim.C_TUNDRA, Clim.C_TUNDRA },
								           { Clim.C_STEPPE, Clim.C_STEPPE, Clim.C_DECIDUOUS,  Clim.C_DECIDUOUS,  Clim.C_DECIDUOUS  },
								           { Clim.C_DESERT, Clim.C_SAVANA, Clim.C_JUNGLE,  Clim.C_JUNGLE, Clim.C_SWAMP  },
								           { Clim.C_DESERT, Clim.C_SAVANA, Clim.C_JUNGLE, Clim.C_SWAMP,  Clim.C_SWAMP  } };

        public const int  MTDELTA = 20, TCSIZE = 4, RCSIZE = 5;
        public const double ICEBERGK = 263.0;

        /// <summary>
        ///  The outer loop looks at each square.  If it is ocean, the climate will
        ///  be ocean unless the temperature is below ICEBREGK degrees all year round.
        ///  If it is land, then the average rainfall and temperature (in Farenheit) are
        ///  computed for the square.  If the square is mountain, it is colder; the
        ///  temperature is decreased.  These two figures are then turned into array
        ///  indices by using the tempcut and raincut parameter vectors.  The climate
        ///  for the square is then simply a table lookup.  Finally, the array is printed
        ///  if desired. 
        /// </summary>
        private void ComputeBiomes()
        {
            cl = new float[width, height];
            int buf;
            double avetemp, averain;
            int ttt, r;
            bool noice;
            for (int j = 0; j < height; j++)
                for (int i = 0; i < width; i++)
                {
                    if (heightmap[i, j] <= 0)
                    { /* ocean */
                        noice = false;
                        for (buf = 0; buf < BSIZE; buf++)
                            noice |= (tt[buf, i, j] > TEMPSCALE * ICEBERGK);
                        cl[i, j] = (float)(noice ? Clim.C_OCEAN : Clim.C_OCEANICE);
                    }
                    else if (heightmap[i, j] >= 3500)
                        cl[i, j] = (float)Clim.C_MOUNTAIN;
                    else
                    { /* land or mountain */
                        for (averain = 0, avetemp = 0, buf = 0; buf < BSIZE; buf++)
                        {
                            //averain += rn[buf, i, j];
                            avetemp += tt[buf, i, j];
                        }
                        averain = rainfallmap[i, j]; // /= BSIZE;
                        avetemp /= BSIZE;
                        avetemp = ((double)(avetemp / TEMPSCALE) - 273.0) * 1.8 +32.0;
                        if (heightmap[i, j] >= 100)
                            avetemp -= MTDELTA;
                        ttt = 0;
                        while ((avetemp > tempcut[ttt]) && (ttt < TCSIZE - 1))
                            ttt++;
                        r = 0;
                        while ((averain > raincut[r]) && (r < RCSIZE - 1))
                            r++;
                        cl[i, j] = (float)climkey[ttt, r];
                    }
                }
        }

        private void ComputeBiomes2()
        {
            float minRain = float.MinValue, maxRain = float.MinValue;
            dndclim = new float[width, height];
            cl = new float[width, height];

            for (int j = 0; j < height; j++)
                for (int i = 0; i < width; i++)
                {
                    if (heightmap[i, j] <= 0)
                    { /* ocean */
                        float avetemp = 0;
                        float MaxTemp_C = float.MinValue;
                        float MinTemp_C = float.MaxValue;
                        for (int buf = 0; buf <= BSIZE - 1; buf++)
                        {
                            avetemp = avetemp + tt[buf, i, j] / TEMPSCALE;
                            if (MaxTemp_C < tt[buf, i, j] / TEMPSCALE)
                                MaxTemp_C = tt[buf, i, j] / TEMPSCALE;
                            if (MinTemp_C > tt[buf, i, j] / TEMPSCALE)
                                MinTemp_C = tt[buf, i, j] / TEMPSCALE;
                        }
                        avetemp = avetemp / BSIZE;

                        if (avetemp <= ICEBERGK || MaxTemp_C <= FREEZING)
                        {
                            cl[i, j] = (float)Clim.C_OCEANICE;
                            dndclim[i, j] = (float)DNDCLIMATE_E.D_OCEANICE;
                        }
                        else
                        {
                            cl[i, j] = (float)Clim.C_OCEAN;
                            dndclim[i, j] = (float)DNDCLIMATE_E.D_TEMP_AQUATIC;
                            if (MinTemp_C > FREEZING + 18)
                                dndclim[i, j] = (float)DNDCLIMATE_E.D_WARM_AQUATIC;
                        }
                        if (MinTemp_C <= -5 + FREEZING & MaxTemp_C >= 10 + FREEZING)
                        {
                            dndclim[i, j] = (float)DNDCLIMATE_E.D_COLD_AQUATIC;
                        }
                    }
                    else
                    { /* land or mountain */
                        // calculate the temperature and rainfall stats for this spot
                        float avetemp = 0;
                        float MaxTemp_C = float.MinValue;
                        float MinTemp_C = float.MaxValue;
                        float MinRain_cm = 32768;
                        float MaxRain_cm = 0;
                        float RainRange_cm = 0;
                        float AnnualRain_cm = 0;
                        int DryestMonth = 0;
                        int WettestMonth = 0;
                        for (int buf = 0; buf <= BSIZE - 1; buf++)
                        {
                            //fahrenheit = (9 / 5) * (tt(buf, i, j) / TEMPSCALE - FREEZING#) + 32
                            float Celsius = (float)(tt[buf, i, j] / TEMPSCALE - FREEZING);

                            if (Celsius < MinTemp_C)
                                MinTemp_C = Celsius;
                            if (Celsius > MaxTemp_C)
                                MaxTemp_C = Celsius;
                            AnnualRain_cm = AnnualRain_cm + rn[buf, i, j] * 12 / BSIZE ;

                            if (rn[buf, i, j] * 12 / BSIZE < MinRain_cm)
                            {
                                MinRain_cm = rn[buf, i, j] * 12 / BSIZE;
                                DryestMonth = buf;
                            }
                            if (rn[buf, i, j] * 12 / BSIZE > MaxRain_cm)
                            {
                                MaxRain_cm = rn[buf, i, j] * 12 / BSIZE;
                                WettestMonth = buf;
                            }
                            avetemp = avetemp + Celsius;
                        }
                        float TempRange_C = MaxTemp_C - MinTemp_C;
                        float RainFact = 0;
                        RainRange_cm = MaxRain_cm - MinRain_cm;
                        avetemp = avetemp / BSIZE;

                        if (maxRain < AnnualRain_cm) maxRain = AnnualRain_cm;
                        if (minRain > AnnualRain_cm) minRain = AnnualRain_cm;
                        if (MaxRain_cm > 0)
                        {
                            RainFact = 100.0f * RainRange_cm / MaxRain_cm;
                        }
                        else
                        {
                            RainFact = 0;
                        }
                        // A moist tropical climates
                        // A
                        if (MinTemp_C >= 18)
                        {
                            if (AnnualRain_cm < 635) // 25 in
                            {
                                // B climates
                                cl[i, j] = (float)Clim.C_DESERT;
                                dndclim[i, j] = (float)DNDCLIMATE_E.D_WARM_DESERT;
                            }
                            else if (AnnualRain_cm < 1143) // 45 in
                            {
                                // plains
                                cl[i, j] = (float)Clim.C_SAVANA;
                                dndclim[i, j] = (float)DNDCLIMATE_E.D_WARM_PLAIN;
                                if (CheckForDnDHills(i, j) == true)
                                    dndclim[i, j] = (float)DNDCLIMATE_E.D_WARM_HILL;
                                if (heightmap[i, j] >= 4000)
                                    dndclim[i, j] = (float)DNDCLIMATE_E.D_WARM_MOUNTAIN;
                                else
                                {
                                    // lots of rain.  Either forest or monsoon
                                    //Select Case RainFact
                                    //    Case Is < 60
                                    cl[i, j] = (float)Clim.C_JUNGLE;
                                    dndclim[i, j] = (float)DNDCLIMATE_E.D_WARM_FOREST;
                                    //    Case Else
                                    //        If MinRain_cm > 0.3 Then
                                    //            cl[i,j] = Climate_E.C_TROPICWETDRY
                                    //            dndclim[i,j] = modTecGlobals.DNDCLIMATE_E.D_WARM_FOREST
                                    //        Else
                                    //            cl[i,j] = Climate_E.C_MONSOON
                                    //            dndclim[i,j] = modTecGlobals.DNDCLIMATE_E.D_WARM_FOREST
                                    //        End If
                                    //End Select

                                    if (heightmap[i, j] >= 0 && heightmap[i, j] < 3000)
                                    {
                                        cl[i, j] = (float)Clim.C_SWAMP;
                                        dndclim[i, j] = (float)DNDCLIMATE_E.D_WARM_SWAMP;
                                    }
                                }
                            }
                            else if (MinTemp_C >= -5 && MaxTemp_C >= 10)
                            {
                                if (AnnualRain_cm < 635)
                                {
                                    // desert
                                    cl[i, j] = (float)Clim.C_DESERT;
                                    dndclim[i, j] = (float)DNDCLIMATE_E.D_TEMP_DESERT;
                                }
                                else if (AnnualRain_cm < 1016) // 40 in
                                {
                                    // steppes and plains
                                    cl[i, j] = (float)Clim.C_STEPPE;
                                    dndclim[i, j] = (float)DNDCLIMATE_E.D_TEMP_PLAIN;
                                    if (CheckForDnDHills(i, j) == true)
                                        dndclim[i, j] = (float)DNDCLIMATE_E.D_TEMP_HILL;
                                    if (heightmap[i, j] >= 4000)
                                        dndclim[i, j] = (float)DNDCLIMATE_E.D_TEMP_MOUNTAIN;
                                }
                                else
                                {
                                    //   Case Is < 60
                                    cl[i, j] = (float)Clim.C_DECIDUOUS;
                                    dndclim[i, j] = (float)DNDCLIMATE_E.D_TEMP_FOREST;
                                    //Case Else '60 To 88
                                    //Select Case DryestMonth
                                    //Case MONTH_E.JANUARY To MONTH_E.MARCH
                                    //cl[i,j] = Climate_E.C_GRASSLAND ' Dry winter?
                                    //dndclim[i,j] = modTecGlobals.DNDCLIMATE_E.D_TEMP_PLAIN
                                    //Case MONTH_E.APRIL To MONTH_E.SEPTEMBER
                                    //    cl[i,j] = Climate_E.C_MEDITERRANEAN ' Dry summer?
                                    //    dndclim[i,j] = modTecGlobals.DNDCLIMATE_E.D_TEMP_PLAIN
                                    //Case MONTH_E.OCTOBER To MONTH_E.DECEMBER
                                    //    cl[i,j] = Climate_E.C_GRASSLAND ' Dry winter?
                                    //    dndclim[i,j] = modTecGlobals.DNDCLIMATE_E.D_TEMP_PLAIN
                                    //End Select
                                    //End Select

                                    if (heightmap[i, j] >= 0 && heightmap[i, j] < 3000)
                                    {
                                        dndclim[i, j] = (float)DNDCLIMATE_E.D_TEMP_SWAMP;
                                        cl[i, j] = (float)Clim.C_SWAMP;
                                    }
                                }

                            }
                            else if (MinTemp_C <= -5 & MaxTemp_C >= 10)
                            {
                                // D moist continental mid-latitude climates, cold winter
                                if (AnnualRain_cm < 635)
                                {
                                    cl[i, j] = (float)Clim.C_DESERT;
                                    dndclim[i, j] = (float)DNDCLIMATE_E.D_COLD_DESERT;
                                }
                                else if (AnnualRain_cm < 1016 )
                                {
                                    cl[i, j] = (float)Clim.C_STEPPE;
                                    dndclim[i, j] = (float)DNDCLIMATE_E.D_COLD_PLAIN;
                                    if (CheckForDnDHills(i, j) == true)
                                        dndclim[i, j] = (float)DNDCLIMATE_E.D_COLD_HILL;
                                    if (heightmap[i, j] >= 4000)
                                        dndclim[i, j] = (float)DNDCLIMATE_E.D_COLD_MOUNTAIN;
                                }
                                else
                                {
                                    //    Case Is < 75
                                    cl[i, j] = (float)Clim.C_BOREAL;
                                    dndclim[i, j] = (float)DNDCLIMATE_E.D_COLD_FOREST;
                                    //    Case Else
                                    //        cl[i,j] = Climate_E.C_COLDDRYWINTER
                                    //        dndclim[i,j] = modTecGlobals.DNDCLIMATE_E.D_COLD_FOREST
                                    //End Select

                                    if (heightmap[i, j] >= 0 && heightmap[i, j] < 3000)
                                    {
                                        dndclim[i, j] = (float)DNDCLIMATE_E.D_COLD_SWAMP;
                                        cl[i, j] = (float)Clim.C_SWAMP;
                                    }
                                    break;
                                }
                            }
                            else if (MaxTemp_C >= 0 & MaxTemp_C <= 10)
                            {
                                // Tundra
                                cl[i, j] = (float)Clim.C_TUNDRA;
                                dndclim[i, j] = (float)DNDCLIMATE_E.D_COLD_DESERT;

                                if (heightmap[i, j] >= 4000)
                                    dndclim[i, j] = (float)DNDCLIMATE_E.D_COLD_MOUNTAIN;
                                if (CheckForDnDHills(i, j) == true)
                                    dndclim[i, j] = (float)DNDCLIMATE_E.D_COLD_HILL;

                            }
                            else if (MaxTemp_C < 0)
                            {
                                // Ice cap
                                cl[i, j] = (float)Clim.C_LANDICE;
                                dndclim[i, j] = (float)DNDCLIMATE_E.D_COLD_PLAIN;
                                if (CheckForDnDHills(i, j) == true)
                                    dndclim[i, j] = (float)DNDCLIMATE_E.D_COLD_HILL;
                                if (heightmap[i, j] >= 4000)
                                    dndclim[i, j] = (float)DNDCLIMATE_E.D_COLD_MOUNTAIN;
                            }
                            else
                            {
                                throw new Exception("Cannot compute climate: T=[" + MinTemp_C + ":" + MaxTemp_C + "] P=" + AnnualRain_cm);
                            }

                        }
                    }
                }
        }


        /// <summary>
        ///  Implementtion of the described by Holdridge at the paper http://cct.or.cr/publicaciones/Life-Zone-Ecology.pdf
        ///  
        /// Latitudinal regions (mean annual biotemperature at sea level 0) (Cesius)
        /// Polar:      0 - 1.5
        /// SubPolar:   1.5 - 3
        /// Boreal:     3 - 6
        /// Cool temp:  6 - 12
        /// Warn temp:  12 - 18
        /// Subtropical: 18 - 24
        /// Tropical:   24 - 30+
        /// 
        /// Biotemperature =  t – [3 * (latitud/100) * (t – 24)2] for t > 24
        /// Elevation in meters
        /// 0
        /// 500
        /// 1000
        /// 1500
        /// 2000
        /// 2500
        /// 3000
        /// 3500
        /// 4000
        /// 4500
        /// 4750+
        /// 
        /// Annual precipitation classification (mm)
        /// Super-Arid:     0-125
        /// Perarid:        125-250
        /// Arid:           250-500
        /// SemiArid:       500-1000
        /// Sub-Arid:       1000-2000
        /// Humid:          2000-4000
        /// PerHumid:       4000-8000
        /// SuperHumid:     8000-16000
        /// 
        /// 
        /// 			biotemperature 	Average total annual precipitation (cm)
        /// Polar			< 1.5		6.25-75
        /// Subpolar 
        /// </summary>
        private void ComputeBiomes3()
        { 
        }

        private bool CheckForDnDHills(int i, int j)
        {
            bool functionReturnValue = false;

            functionReturnValue = false;
            if (heightmap[i, j] >= 2000 && heightmap[i, j] <= 4000)
            {
                functionReturnValue = true;
            }
            else if (heightmap[i, j] < 2000)
            {// TODO && i <= Information.UBound(mvarGradientMagnitude, 1) && j <= Information.UBound(mvarGradientMagnitude, 2)) {
                //TODO if (mvarGradientMagnitude(i, j) >= 2000)
                //TODO 	functionReturnValue = true;
            }
            return functionReturnValue;
        }

        #endregion

        #region TEMPERATURE
        /// <summary>
        /// Parameters affecting temperature
        ///TILT      - Default 23.0.  This is the tilt of the planet with respect to
        ///            its plane of orbit, in degrees.  Smaller numbers produce less 
        ///            seasonality; numbers above 45 violate some of the assumptions
        ///            of the models used.
        ///ECCENT    - Default 0.0.  The eccentricity of the planet's orbit; this
        ///            parameter affects seasonality as well.  Numbers above 0.5 are
        ///            probably unrealistic.
        ///ECCPHASE  - Default 0.0.  This parameter describes the phase offset of the
        ///            eccentricity with respect to the tilt, in radians.  You can
        ///            produce climates with complicated seasonality by varying this.
        ///LCONST    - Default 275.0.  The basic temperature for land squares, assuming
        ///            no tilt, eccentricity, or nearby ocean.
        ///LCOS      - Default 45.0.  The amount by which land temperatures should vary
        ///            from north pole to equator.  Land temperature, ignoring ocean
        ///            effects, varies from LCONST - LCOS/2 at the poles to LCONST +
        ///            LCOS/2 at the equator.
        ///LTILT     - Default 1.0.  The fraction of the tilt parameter that should be
        ///            applied to temperature adjustment for land.  Typically, land
        ///            temperatures vary more from season to season than the ocean
        ///            temperatures do, so LTILT should be higher than OTILT.
        ///LSMOOTH   - Default 0.6.  One equation governs the effect of land on ocean
        ///            temperatures and vice versa.  The equation involves LSMOOTH,
        ///            LDIV, OSMOOTH and ODIV.  Given the land and sea temperatures, and
        ///            the number of land squares in a 11 x 5 box around the square,
        ///            the final temperature is a weighted sum of the two temperatures.
        ///            The weights are related to LSMOOTH and OSMOOTH, and the importance
        ///            of nearby land is diminished by increasing LDIV or ODIV.
        ///LDIV      - Default 180.0.  See above.
        ///OCONST    - Default 275.0.  Same as LCONST, only for the ocean.
        ///OCOS      - Default 30.0.   Same as LCOS, only for the ocean.
        ///OTILT     - Default 0.2.    See LTILT.
        ///OSMOOTH   - Default 0.2.    See LSMOOTH.
        ///ODIV      - Default 250.0.  See LSMOOTH.
        /// </summary>
        private const double TILT = 23.0, ECCENT = 0.0, ECCPHASE = 0.0;
        private const double LCOS = 45.0, LCONST = 275.0, LTILT = 1.0, LSMOOTH = 0.6, LDIV = 180.0;
        private const double OCOS = 30.0, OCONST = 275.0, OTILT = 0.2, OSMOOTH = 0.2, ODIV = 250.0;
        private const int BSIZE = 4; //Number of seasons in computation.
        private const double FREEZING = 273.15; // In Kelvin scale
        private const double DEG2RAD = (Math.PI / 180);
        private const float TEMPSCALE = 1;
        private float[, ,] tt;

        /// <summary>
        /// This function does all the work for computing temperatures.  The outermost
        /// loop goes through each row of the output array once, computing all buffers
        /// at the same time.  The loop has two inner loops: first, tland and tsea are
        /// filled with the temperatures found far inland and far at sea for each buffer.
        /// In the second loop, the weight function for each point in the latitude line
        /// is computed and the temperature is found for each buffer.
        /// </summary>
        public void ComputeHeatmap(int season)
        {
            int buf;
            double lat, lscl, sscl, x, fact, theta, delth, phase;
            double[] tland = new double[BSIZE], tsea = new double[BSIZE];
            tt = new float[BSIZE, width, height];
            lscl = DEG2RAD * 180.0 / (90.0 + LTILT * TILT);
            sscl = DEG2RAD * 180.0 / (90.0 + OTILT * TILT);
            delth = 2.0 * Math.PI / (double)BSIZE;
            float tmin = float.MaxValue, tmax = float.MinValue;
            for (int j = 0; j < height; j++)
            {
                lat = 90.0 - 180.0 * (double)j / (double)height;
                for (buf = 0, theta = 0; buf < BSIZE; buf++, theta += delth)
                {
                    phase = theta + ECCPHASE;
                    if (phase > 2.0 * Math.PI)
                        phase -= (2 * Math.PI);
                    fact = (1.0 + ECCENT * Math.Cos(phase)) * TEMPSCALE;
                    // x is the effective latitude in radians due to the tilt
                    x = (lat + Math.Cos(theta) * TILT * LTILT) * lscl;
                    tland[buf] = (LCONST + LCOS * Math.Cos(x)) * fact;
                    x = (lat + Math.Cos(theta) * TILT * OTILT) * sscl;
                    tsea[buf] = (OCONST + OCOS * Math.Cos(x)) * fact;
                }

                // Calculate the smoothing
                for (int i = 0; i < width; i++)
                {
                    for (buf = 0; buf < BSIZE; buf++)
                    {
                        float lngLand = countland[i, j];
                        tt[buf, i, j] = (float)(tsea[buf] * (1 - lngLand / 198) + tland[buf] * (lngLand / 198));

                        // factor in altitude
                        // The Standard Atmosphere (SA) which is what all instruments, and especially aviation instruments, 
                        // are calibrated for is like this
                        // Average temperature: 15 degrees Celsius.
                        // Average pressure at sea level: 1,013.25 hectoPascals.
                        // Average temperature drop per 100 m of altitude: 0.65 degrees.
                        // Dry air temperatue drops with about 1 degrees C per 100 meter and
                        // saturated moist air, at about 0.5 degrees C per 100 meter.
                        if (heightmap[i, j] >= 0)
                        {
                            double temperature = tt[buf, i, j] / TEMPSCALE;
                            // do the elevation math
                            temperature = temperature - 0.65 * (heightmap[i, j] / 100);
                            tt[buf, i, j] = (float)(temperature * TEMPSCALE);
                        }

                        if ((tt[buf, i, j] < tmin))
                            tmin = tt[buf, i, j];
                        if ((tt[buf, i, j] > tmax))
                            tmax = tt[buf, i, j];
                    }
                }
            }
            heatmap = new float[width, height];
            for (int j = 0; j < height; j++)
                for (int i = 0; i < width; i++)
                {
                    heatmap[i, j] = (float)(tt[season, i, j] / TEMPSCALE);
                }
        }

        private void ComputeCountLand()
        {
            countland = new float[width, height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; ++x)
                {
                    countland[x, y] = CountLand(x, y);
                }
            }
        }

        /// <summary>
        /// Called by ComputeCountLand() for each square, this function looks in a box
        /// and counts the number of land squares found there.  
        /// It compensates for y values off the map, and wraps x values around. 
        /// The answer is returned.
        /// </summary>
        /// <param name="x"> x coord</param>
        /// <param name="y"> y coord</param>
        /// <returns></returns>
        private float CountLand(int x, int y)
        {
            const int boxSize = 5;
            int xbox = (int)(boxSize * factor);

            float sum = 0;
            int jmin, jmax, i1;

            jmin = y - boxSize;
            if (jmin < 0)
                jmin = 0;
            jmax = y + boxSize;
            if (jmax >= height)
                jmax = height - 1;
            for (int j1 = jmin; j1 <= jmax; j1++)
                for (int i0 = -xbox; i0 < xbox; i0++)
                {
                    i1 = i0 + x;
                    if (i1 < 0)
                        i1 += width;
                    if (i1 >= width)
                        i1 -= width;
                    sum += (heightmap[i1, j1] > 0 ? 1 : 0);
                }
            return sum;
        }
        #endregion

        #region PRESSURE
        /// <summary>
        /// Parameters affecting pressure
        /// OLTHRESH  - Default 1.  Ocean pressure zones essentially ignore land masses
        ///             whose radius is equal to or less than this number, like islands.
        /// OOTHRESH  - Default 5.  Ocean pressure zones must be at least this many 
        ///             squares away from the nearest (non-ignored) land.
        /// OLMIN     - Default 40.  If the unscaled temperature of an ocean square is
        ///             greater than OLMIN and less than OLMAX, then that square is a
        ///             low pressure zone.
        /// OLMAX     - Default 65.  See above.
        /// OHMIN     - Default 130.  If the unscaled temperature of an ocean square is
        ///             greater than OHMIN and less than OHMAX, then that square is a
        ///             high pressure zone.
        /// OHMAX     - Default 180.  See above.
        /// LOTHRESH  - Default 3.  Land pressure zones essentially ignore ocean bodies
        ///             whose radius is less than or equal to this number, like lakes.
        /// LLTHRESH  - Default 7.  Land pressure zones must be at least this many 
        ///             squares away from the nearest (non-ignored) ocean.
        /// LLMIN     - Default 220.  If the unscaled temperature of a land square is
        ///             greater than LLMIN and less than LLMAX, then that square is a
        ///             low pressure zone.
        /// LLMAX     - Default 255.  See above.
        /// LHMIN     - Default 0.  If the unscaled temperature of a land square is
        ///             greater than LHMIN and less than LHMAX, then that square is a
        ///             high pressure zone.
        /// LHMAX     - Default 20.  See above.
        /// </summary>
        private int maxRange = 15;
        private const int MAXPRESS = 255;
        private int OOTHRESH = 5, OLTHRESH = 1, LOTHRESH = 3, LLTHRESH = 7;
        private int OHMIN = 293, OHMAX = 353, OLMIN = 273, OLMAX = 283; // High is between 20-80 degree Celsius, Low  is between 0-10
        private int LHMIN = 293, LHMAX = 353, LLMIN = 220, LLMAX = 255; // High is between 30-80 degree Celsius, Low  is between 0-10
        private int SMOOTH = 1;

        /* Input arrays are pr, from pressure.c, and l, from main.c.  The array
        hl is used as temporary storage for highs and lows, while p is used
        to store the smoothed pressure map.  The output array, wd, contains
        an edge map with wind directions. */

        private Pressure[, ,] pr;
        private WindDir[, ,] wd;
        private int[,] hl0;
        private int[,] hl1;
        private int[,] r;

        public void ComputePressureMap()
        {
            r = new int[width, height];
            pr = new Pressure[BSIZE, width, height];
            wd = new WindDir[BSIZE, width, height];
            hl0 = new int[width, height];
            hl1 = new int[width, height];

            pressuremap = new float[width, height];
            maxRange = Math.Max(width, height) / 60 * 15;
            OOTHRESH = Math.Max(width, height) / 60 * 5;
            SMOOTH = Math.Max(width, height) * 3 / 400;

            /* The main routine for this file.  It just calls the four routines
            which do all the work.  PressureInOcean() finds pressure extremes on the ocean;
            PressureInLand() does the same for land; ComputeHeatEquator() defines the heat equator, and
            ComputePressure() computes pressuremap[, ] from pr[, ]. */

            PressureInOcean();
            PressureInLand();
            ComputeHeatEquator();
            ComputePressure();
        }

        /// <summary>
        /// This function takes the high and low markings from pressure.c and creates
        /// a smoothed function.  Highs turn into MAXPRESS and lows turn into 0.
        /// </summary>
        /// <param name="season">the season to be calculated</param>
        private void ComputePressure()
        {
            int i, j;
            int equator = 0;
            for (int season = 0; season < BSIZE; season++)
            {
                for (i = 0; i < width; i++)
                {
                    for (j = 0; j < height; j++)
                    {
                        /* Zero out the arrays to be used */
                        wd[season, i, j] = 0;
                        hl0[i, j] = 0;
                        hl1[i, j] = 0;

                        /* Fill hl[0] with the low pressure zones, and hl[1] with highs */
                        if (pr[season, i, j] == Pressure.PR_LOW)
                            hl0[i, j] = -1;
                        else if (pr[season, i, j] == Pressure.PR_HIGH)
                            hl1[i, j] = -1;
                        else if (pr[season, i, j] == Pressure.PR_HEQ)
                        {
                            hl0[i, j] = -1;
                            equator = j;
                        }
                    }
                    for (int k = -OOTHRESH; k <= OOTHRESH; k++)
                        hl1[i, equator + k] = 0;
                }
                /* Set each square in hl[0] to the distance from that square to the */
                /* nearest low, and each square in hl[1] to the distance to a high. */
                Range(hl0);
                Range(hl1);

                /* The final pressure, in array p, is zero if a low is there and */
                /* MAXPRESS if a high is there.  Otherwise, the pressure in a square is */
                /* proportional to the ratio of (distance from the square to the nearest */
                /* low) to (total of distance from nearest high and nearest low).  This */
                /* gives a smooth curve between the extremes. */
                for (j = 0; j < height; j++)
                    for (i = 0; i < width; i++)
                    {
                        if (hl1[i, j] == -1)
                            pressuremap[i, j] = MAXPRESS;
                        else if (hl0[i, j] == -1)
                            pressuremap[i, j] = 0;
                        else
                            pressuremap[i, j] = (float)(MAXPRESS * (hl0[i, j])) / (float)(hl0[i, j] + hl1[i, j]);
                    }
            }
            Smooth(pressuremap, SMOOTH, MAXPRESS);
        }

        /// <summary>
        /// Determine ocean highs and lows.  An ocean high or low must occur over
        /// ocean, far away from major land masses.  Two calls to range() are made
        /// to find the qualifying ocean areas; then temperature criteria are used
        /// to select the actual pressure zones.
        /// </summary>
        /// <param name="season"></param>
        private void PressureInOcean()
        {
            int i, j;
            float x;

            /* Set r to the distance on land from the coast. */
            for (j = 0; j < height; j++)
                for (i = 0; i < width; i++)
                    r[i, j] = heightmap[i, j] > 0 ? (sbyte)0 : (sbyte)-1;
            Range(r);

            /* Initialize r to contain blobs on land which are at least OLTHRESH squares
               away from the coast.  Then set r to the distance from these.  The result
               in r is the distance from the nearest big piece of land (ignoring
               islands). */
            for (j = 0; j < height; j++)
                for (i = 0; i < width; i++)
                    r[i, j] = (r[i, j] > OLTHRESH) ? (sbyte)-1 : (sbyte)0;
            Range(r);

            /* For each array element, if it is at least OOTHRESH squares from the
               nearest big piece of land, it might be the center of an ocean pressure
               zone.  The pressure zones are defined by temperature ranges; if the
               temperature in ts is between OLMIN and OLMAX, a low is recorded, while
               if the temperature is between OHMIN and OHMAX, a high is recorded. */

            for (int season = 0; season < BSIZE; season++)
            {
                for (j = 0; j < height; j++)
                    for (i = 0; i < width; i++)
                    {
                        pr[season, i, j] = 0;
                        x = (float)(tt[season, i, j] / TEMPSCALE);
                        if (r[i, j] > OOTHRESH)
                        {
                            if ((x >= OLMIN) && (x <= OLMAX))
                                pr[season, i, j] = Pressure.PR_LOW;
                            if ((x >= OHMIN) && (x <= OHMAX))
                                pr[season, i, j] = Pressure.PR_HIGH;
                        }
                    }
            }
        }

        /// <summary>
        /// This function is simply the complement of ocean(): it finds land highs
        /// and lows.  A land high or low must occur over land, far from major oceans.
        /// Two calls to range() are made to find the qualifying land areas; then
        /// temperature criteria are used to select the actual pressure zones.
        /// </summary>
        /// <param name="season"></param>
        private void PressureInLand()
        {
            int i, j;
            float x;

            /* Set r to distance on water from coast. */
            for (j = 0; j < height; j++)
                for (i = 0; i < width; i++)
                    r[i, j] = (sbyte)(heightmap[i, j] > 0 ? -1 : 0);
            Range(r);

            /* Initialize r to contain blobs on ocean which are at least LOTHRESH
               squares away from the coast.  Then set r to the distance from these.  The
               result in r is the distance from the nearest ocean, ignoring lakes. */
            for (j = 0; j < height; j++)
                for (i = 0; i < width; i++)
                    r[i, j] = (sbyte)((r[i, j] > LOTHRESH) ? -1 : 0);
            Range(r);

            /* For each array element, if it is at least LLTHRESH squares from the
               nearest large ocean, it might be the center of a land pressure zone.
               The pressure zones are defined by temperature ranges; if the temperature
               in ts is between LLMIN and LLMAX, a low is recorded, while if the
               temperature is between LHMIN and LHMAX, a high is recorded. */
            for (int season = 0; season < BSIZE; season++)
            {
                for (j = 0; j < height; j++)
                    for (i = 0; i < width; i++)
                    {
                        x = (float)(tt[season, i, j] / TEMPSCALE);
                        if (r[i, j] > LLTHRESH)
                        {
                            if ((x >= LLMIN) && (x <= LLMAX))
                                pr[season, i, j] = Pressure.PR_LOW;
                            if ((x >= LHMIN) && (x <= LHMAX))
                                pr[season, i, j] = Pressure.PR_HIGH;
                        }
                    }
            }

        }

        /// <summary>
        /// This function finds the heat equator and marks it in pr.  For each
        /// vertical column of ts, the median position is found and marked.  To
        /// make the heat equator continuous, jlast is set to the position of the
        /// heat equator in the previous column; a connection is made in the present
        /// column to ensure continuity.
        /// </summary>
        /// <param name="season"></param>
        private void ComputeHeatEquator()
        {
            int i, j;
            int sum;
            int jlast = 0, jnext;

            for (int season = 0; season < BSIZE; season++)
            {
                for (i = 0; i < width; i++)
                {
                    /* Find the total of the temperatures in this column */
                    for (sum = 0, j = 0; j < height; j++)
                        sum += (int)(tt[season, i, j] / TEMPSCALE);

                    /* Step through the column again until the total so far is exactly
                       half the total for the column.  This is the median position. */
                    for (sum >>= 1, j = 0; j < height && sum > 0; j++)
                        sum -= (int)(tt[season, i, j] / TEMPSCALE);

                    /* Mark this position and remember it with jnext */
                    pr[season, i, j] = Pressure.PR_HEQ;
                    jnext = j;

                    /* For each column except the first (where i = 0), if the last heat
                       equator is above this one, move upwards to it, marking each square,
                       to ensure continuity; if below this one, move downwards to it. */

                    if (i != 0 && (j > jlast))
                        for (; j >= jlast; j--)
                            pr[season, i, j] = Pressure.PR_HEQ;
                    else if (i != 0 && (j < jlast))
                        for (; j <= jlast; j++)
                            pr[season, i, j] = Pressure.PR_HEQ;

                    /* Remember this position for the next column.  Note that no check is
                       done to ensure continuity at the wraparound point; this is bad. */
                    jlast = jnext;
                }
            }

        }

        #endregion

        #region WIND

        /// <summary>
        /// BARSEP    - Default 16.  Winds are determined from pressure; a smooth
        ///             pressure map ranging from 0..255 is built by interpolating between
        ///             highs and lows.  Wind lines are contour lines on this map, and
        ///             BARSEP indicates the pressure difference between lines on the map.
        /// </summary>
        private int BARSEP = 16;

        /// <summary>
        /// This is the main function in this file; it calls getpress() to create
        /// a smoothed pressure map, then getwind() to put isobars (wind lines) on
        /// the output map.  The last step makes sure that contradictory winds are
        /// removed, such as N and S winds in the same square.
        /// </summary>
        public void ComputeWind()
        {
            windmap = new float[width, height];
            wind = SobelOperator(pressuremap);

            //WindDir x;
            //BARSEP = (int)(16.0 * 60 / Math.Max(width, height) * 0.5);
            //for (int season = 0; season < BSIZE; season++)
            //{
            //    GetWind(season);
            //    for (int j = 0; j < height; j++)
            //        for (int i = 0; i < width; i++)
            //        {
            //            x = wd[season, i, j];
            //            if ((x & WindDir.N) != 0)
            //                x &= (~WindDir.S);
            //            if ((x & WindDir.E) != 0)
            //                x &= (~WindDir.W);
            //            wd[season, i, j] = x;
            //            wind[i, j].dir = x;
            //        }
            //}

            for (int j = 0; j < height; j++)
                for (int i = 0; i < width; i++)
                    windmap[i, j] = (int)wind[i, j].dir;
        }

        /// <summary>
        /// This function draws isobars around the pressure map created above.  These
        /// isobars are the directions of wind flow.  The isobars are given a direction
        /// depending on whether the square is above or below the heat equator; north of
        /// the heat equator, the winds blow counterclockwise out from a low, while
        /// south of it, the opposite is true.
        /// </summary>
        /// <param name="season"></param>
        public void GetWind(int season)
        {
            float a, b;
            bool e = false;

            /* Step from 0 to MAXPRESS by BARSEP; bar is the pressure for which this */
            /* isobar will be drawn. */
            for (int bar = BARSEP; bar <= MAXPRESS; bar += BARSEP)
            {
                for (int i = 0; i < width; i++)
                    for (int j = 0; j < height; j++)
                    {
                        e = false;
                        /* Set e if this square is south of the heat equator */
                        a = pressuremap[i, j];
                        if (pr[season, i, j] == Pressure.PR_HEQ)
                            e = true;

                        /* Provided the square is not at the top of the array, compare the */
                        /* pressure here to the pressure one square up.  This gives the */
                        /* direction of the wind in terms of east / west flow. */
                        if (j != 0)
                        {
                            b = pressuremap[i, j - 1];
                            if ((a < bar) && (b >= bar))
                                wd[season, i, j] |= (e ? WindDir.E : WindDir.W);
                            if ((a >= bar) && (b < bar))
                                wd[season, i, j] |= (e ? WindDir.W : WindDir.E);
                        }

                        /* Compare the pressure here to the pressure one square to the left */
                        /* (including wraparound); this gives the wind direction in terms */
                        /* of north / south flow. */
                        b = i != 0 ? pressuremap[i - 1, j] : pressuremap[width - 1, j];
                        if ((a < bar) && (b >= bar))
                            wd[season, i, j] |= (e ? WindDir.N : WindDir.S);
                        if ((a >= bar) && (b < bar))
                            wd[season, i, j] |= (e ? WindDir.S : WindDir.N);
                    }
            }
        }

        #endregion


        #region REAINFALL

        /// <summary>
        /// Parameters affecting rainfall
        /// 
        /// MAXFETCH  - Default 5.  Fetch is the term that describes how many squares a
        ///             given wind line travels over water.  A high fetch indicates a
        ///             moist wind.  This number is the maximum depth for the tree walking
        ///             algorithm which finds fetch; the effect of wind in one square
        ///             can travel at most this number of squares before stopping.
        /// RAINCONST - Default 32.  This is the base amount of rainfall in each square.
        /// LANDEL    - Default -10.  This is the amount by which rainfall is increased
        ///             in every land or mountain square; that is, rainfall goes down.
        /// MOUNTDEL  - Default 32.  For each unit of fetch which is stopped by a mountain,
        ///             rainfall in the mountain square increases by this amount.
        /// FETCHDEL  - Default 4.  The amount of rainfall in a square is increased by
        ///             this number for each unit of fetch in the square.
        /// HEQDEL    - Default 32.  The amount of rainfall in a square is increased by
        ///             this amount if the square is on the heat equator.
        /// NRHEQDEL  - Default 24.  The amount of rainfall in a square is increased by
        ///             this amount if the square is next to a square on the heat equator.
        /// FLANKDEL  - Default -24.  The amount of rainfall in a square is increased by
        ///             this amount if the square is on the "flank" of a circular wind
        ///             pattern.  This happens when the wind blows south.
        /// NRFDEL    - Default -3.  The amount of rainfall in a square is increased by
        ///             this amount for each adjacent square which is on a "flank".
        /// </summary>
        private int MAXFETCH = 5, RAINCONST = 28, LANDEL = 10, MOUNTDEL = 32, FETCHDEL = 4;
        private int NRFDEL = 3, HEQDEL = 0, NRHEQDEL = 0, FLANKDEL = -6;
        private int MAXRAIN = 198;
        private bool[, ,] fr;
        private float[,] fs;
        private float[, ,] rn;


        /// <summary>
        /// This is the main rain computation function.  It calls the functions
        /// getfetch () and getrain () to do all the work for each buffer, then
        /// prints out the results if needed. 
        /// </summary>
        private void ComputeRain()
        {
            MAXFETCH = (int)(Math.Max(width, height) / 60.0f * MAXFETCH) * 3;
            rn = new float[BSIZE, width, height];
            fr = new bool[2, width, height];
            fs = new float[width, height];
            rainfallmap = new float[width, height];
            for (int season = 0; season < BSIZE; season++)
            {
                GetFetch(season);
                GetRain(season);
                for (int i = 0; i < width; i++)
                    for (int j = 0; j < height; j++)
                    {
                        rainfallmap[i, j] += rn[season, i, j];
                    }
            }
            Normalize(rainfallmap);
            Smooth(rainfallmap, 3, MAXRAIN);

            float max = float.MinValue;
            float min = float.MaxValue;
            debug = new float[width, height];
            for (int j = 0; j < height; j++)
                for (int i = 0; i < width; i++)
                {
                    //if (heightmap[i, j] > 0)
                        debug[i, j] = rn[0, i, j];
                    if (max < debug[i, j]) max = debug[i, j];
                    if (min > debug[i, j] && heightmap[i, j] > 0) min = debug[i, j];
                }
            for (int j = 0; j < height; j++)
                for (int i = 0; i < width; i++)
                {
                    if (heightmap[i, j] > 0)
                        debug[i, j] = (debug[i, j] - min)* 900 / max; //cm
                    else
                        debug[i, j] = -1;
                }
        }

        /// <summary>
        /// This is the workhorse function for getfetch(), below.  It is called
        /// several times per square.  It changes x to account for wraparound, so it
        ///  won't work as a macro.  If y is out of range it does nothing, else it
        /// "marks" the new square in fr[dest] and increments fs to record the number
        /// of times the square has been marked.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="dest"></param>
        private void FetchInc(int x, int y, int dest)
        {
            if (x == -1)
                x = width - 1;
            else if (x == width)
                x = 0;
            if ((y == -1) || (y == height))
                return;
            fr[dest, x, y] = true;
            fs[x, y]++;
        }

        /// <summary>
        /// "Fetch" is the term that describes how many squares a given wind line
        /// travels over water.  It measures how moist the wind is.  The algorithm to
        /// measure fetch looks like many simultaneous tree walks, where each water
        /// square is a root square, and every wind edge is a tree edge.  A counter
        /// for each square determines how many times that square is reached during
        /// the tree walks; that is the fetch.
        /// </summary>
        /// <param name="season"></param>
        private void GetFetch(int season)
        {
            int src, dest;

            /* Initialize the counter fs to zero.  Array fr, which records the */
            /* list of active edges in the walks, is set so that all ocean squares */
            /* are active.  Also, the result array rn is cleared. */
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                {
                    fr[0, i, j] = (heightmap[i, j] <= 0);
                    fs[i, j] = 0;
                    rn[season, i, j] = 0;
                }

            /* Each time through the loop, each square is examined.  If it's */
            /* active, disable the mark in the current time step (thus ensuring */
            /* that when the buffers are flipped, the new destination is empty). */
            /* If the square is a mountain, don't pass the mark, but instead add */
            /* some amount to the square -- implementing rain shadows and rainy */
            /* mountain squares.  Finally, for each of the eight cardinal */
            /* directions, if there is wind blowing in that direction, carry a */
            /* marker to that square using fetchinc(), above. */

            for (int k = 0; k < MAXFETCH; k++)
            {
                src = k % 2; dest = 1 - src;
                for (int i = 0; i < width; i++)
                    for (int j = 0; j < height; j++)
                        if (fr[src, i, j])
                        {
                            fr[src, i, j] = false;
                            // Unwin (1969) found that the linear model
                            // P = 1064 + 4.57(H in meter) + error term, where P is rainfall in mm
                            if (heightmap[i, j] < 3000)
                            switch (wind[i, j].dir)//wd[season, i, j])
                                {
                                    case WindDir.N | WindDir.E: FetchInc(i + 1, j - 1, dest); break;
                                    case WindDir.N | WindDir.W: FetchInc(i - 1, j - 1, dest); break;
                                    case WindDir.S | WindDir.E: FetchInc(i + 1, j + 1, dest); break;
                                    case WindDir.S | WindDir.W: FetchInc(i - 1, j + 1, dest); break;
                                    case WindDir.N: FetchInc(i, j - 1, dest); break;
                                    case WindDir.S: FetchInc(i, j + 1, dest); break;
                                    case WindDir.E: FetchInc(i + 1, j, dest); break;
                                    case WindDir.W: FetchInc(i - 1, j, dest); break;
                                }
                        }
            }

            Smooth(fs, 5, 1);
        }


        /// <summary>
        /// This macro is called several times per square by getrain(), below.  It
        /// simply tests the square for several conditions: if the square is on the
        /// heat equator, itcz is set to one; if the wind blows south in this square,
        /// it is on the flank of a circular wind zone (and thus less rainy); the local
        /// rain sum, x, is increased according to the fetch sum in the square. 
        /// </summary>
        /// <param name="buf"></param>
        /// <param name="xx"></param>
        /// <param name="yy"></param>
        /// <param name="itcz"></param>
        /// <param name="flank"></param>
        /// <param name="x"></param>
        private void RAINTEST(int buf, int xx, int yy, ref bool itcz, ref bool flank, ref float x)
        {
            if (pr[buf, xx, yy] == Pressure.PR_HEQ)
                itcz = true;
            if ((wind[xx, yy].dir & WindDir.S) != 0)
                flank = true;
            x += (fs[xx, yy] * NRFDEL);
        }

        /// <summary>
        /// Once the fetch array is computed, this function looks at each square to
        /// determine the amount of rainfall there.  The above macro is called five
        /// times, once for the square and each of its four neighbors; this determines
        /// whether the square is near the ITCZ or the flank of an air cycle.  The
        /// sum of fetches for the neighbors is also determined.   Finally, each of the
        /// factors is weighted and added to the rainfall value:  the local fetch value,
        /// a land factor, the nearness of the heat equator, and the nearness of a
        /// flank.  Note that while rn is zeroed in getfetch(), it may be increased by
        /// rain falling on mountains, so it is nonzero when this function is called.
        /// </summary>
        /// <param name="season"></param>
        private void GetRain(int season)
        {
            int i, j;
            float x;
            bool itcz, flank;

            for (i = 0; i < width; i++)
                for (j = 0; j < height; j++)
                {
                    flank = false;
                    itcz = false;
                    // Unwin (1969) found that the linear model
                    // P = 1064 + 4.57(H in meter) + error term, where P is rainfall in mm
                    if (heightmap[i, j] >= 50)
                        rn[season, i, j] += (int)(MOUNTDEL * (heightmap[i, j] / 9000));
                    x = rn[season, i, j];
                    if (i < width - 1) { RAINTEST(season, i + 1, j, ref itcz, ref flank, ref x); }
                    else { RAINTEST(season, 0, j, ref itcz, ref flank, ref x); }
                    if (i != 0) { RAINTEST(season, i - 1, j, ref itcz, ref flank, ref x); }
                    else { RAINTEST(season, width - 1, j, ref itcz, ref flank, ref x); }
                    if (j < height - 1) { RAINTEST(season, i, j + 1, ref itcz, ref flank, ref x); }
                    if (j != 0) { RAINTEST(season, i, j - 1, ref itcz, ref flank, ref x); }
                    RAINTEST(season, i, j, ref itcz, ref flank, ref x);

                    x += (RAINCONST + FETCHDEL * fs[i, j]);
                    if (heightmap[i, j] > 0) x += LANDEL;
                    if (pr[season, i, j] == Pressure.PR_HEQ) x += HEQDEL;
                    if (itcz) x += NRHEQDEL;
                    if (flank) x += FLANKDEL;
                    //if (x < 0) x = 0;
                    //if (x > 255) 
                    //    x = 255;
                    rn[season, i, j] = x;
                }

        }
        #endregion

        /// <summary>
        /// This function is called by a number of climate routines.  It takes an
        /// input array with blobs of -1's on a background of 0's.  The function winds
        /// up replacing each 0 with the distance from that square to the nearest -1.
        /// The function onerange() does all the work, but it will not compute ranges
        /// greater than maxRange.  Therefore, after onerange() is called, any remaining
        /// 0 values must be replaced with maxRange, indicating that that square is
        /// "very far" from any -1 value.
        /// </summary>
        /// <param name="rr"></param>
        private void Range(int[,] rr)
        {
            OneRange(rr);
            for (int j = 0; j < height; j++)
                for (int i = 0; i < width; i++)
                    if ((rr[i, j]) == 0)
                        rr[i, j] = maxRange;
        }


        /// <summary>
        /// This routine consists of a loop.  Each time through the loop, every
        /// square is checked.  If the square is zero, it has not yet been updated.
        /// In that case, look to see if any adjacent squares were previously updated
        /// (or if they were initialized to -1).  If so, set the square to the current
        /// distance value, which happens to be identical to the outer loop variable.
        /// If, after one loop iteration, no squares have been updated, the matrix
        /// must be completely updated.  Stop.  To keep down run-time, a maximum
        /// distance value, maxRange, is used as the terminating loop value. 
        /// </summary>
        /// <param name="rr"></param>
        private void OneRange(int[,] rr)
        {
            int x, k;
            bool keepgo;
            for (k = 1; k < maxRange; k++)
            {
                keepgo = false;
                for (int j = 0; j < height; j++)
                    for (int i = 0; i < width; i++)
                        if ((rr[i, j]) == 0)
                        {
                            keepgo = true;
                            x = rr[i != 0 ? i - 1 : width - 1, j];
                            if (x != 0 && (x != k))
                                rr[i, j] = k;
                            x = rr[(i < width - 1) ? i + 1 : 0, j];
                            if (x != 0 && (x != k))
                                rr[i, j] = k;
                            if (j < height - 1)
                            {
                                x = rr[i, j + 1];
                                if (x != 0 && (x != k))
                                    rr[i, j] = k;
                            }
                            if (j != 0)
                            {
                                x = rr[i, j - 1];
                                if (x != 0 && (x != k))
                                    rr[i, j] = k;
                            }
                        }
                if (!keepgo)
                    return;
            }
            return;
        }

        private void Normalize(float[,] array)
        {
            float min = float.MaxValue, max = float.MinValue;
            for (int w = 0; w < width; w++)
            {
                for (int h = 0; h < height; h++)
                {
                    float curr = array[w, h];
                    min = min < curr ? min : curr;
                    max = max > curr ? max : curr;
                }
            }
            float span = max - min;
            Parallel.For(0, width, w =>
            {
                for (int h = 0; h < height; h++)
                {
                    array[w, h] = (array[w, h] - min) / span;
                }
            });
        }

        private void Smooth(float[,] arr, int d = 5, float maxVal = 255)
        {
            //arr=Gaussian.GaussianConvolution(arr, d);
            //return;
            float[,] mean = new float[width, height];
            float max = float.MinValue;
            for (int w = 0; w < width; w++)
            {
                for (int h = 0; h < height; h++)
                {
                    mean[w, h] = 0;
                    for (int w1 = -d; w1 <= d; w1++)
                    {
                        for (int h1 = -d; h1 <= d; h1++)
                        {
                            int i = w + w1, j = h + h1;
                            if (i < 0) i = width + i;
                            if (j < 0) j = height + j;
                            if (i >= width) i = i - width;
                            if (j >= height) j = j - height;
                            mean[w, h] += arr[i, j];
                        }
                    }
                    mean[w, h] /= d * d;
                    if (max < mean[w, h]) max = mean[w, h];
                }
            }
            float coef = maxVal / max;
            for (int w = 0; w < width; w++)
            {
                for (int h = 0; h < height; h++)
                {
                    arr[w, h] = mean[w, h] * coef;
                }
            }

        }

        private Wind[,] SobelOperator(float[,] b)
        {
            int[,] gx = new int[,] { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } };   //  The matrix Gx
            int[,] gy = new int[,] { { 1, 2, 1 }, { 0, 0, 0 }, { -1, -2, -1 } };  //  The matrix Gy
            Wind[,] b1 = new Wind[width, height];
            bool isSouth;
            for (int j = 1; j < width - 1; j++) // loop for image pixels width    
            {
                isSouth = false;
                for (int i = 1; i < height - 1; i++)   // loop for the image pixels height
                {
                    if (pr[0, j, i] == Pressure.PR_HEQ)
                        isSouth = true;
                    float new_x = 0, new_y = 0;
                    float c;
                    for (int hw = -1; hw < 2; hw++)  //loop for cov matrix
                    {
                        for (int wi = -1; wi < 2; wi++)
                        {
                            c = b[j + wi, i + hw];
                            new_x += gx[hw + 1, wi + 1] * c;
                            new_y += gy[hw + 1, wi + 1] * c;
                        }
                    }
                    b1[j, i].intensity = (float)Math.Sqrt(new_x * new_x + new_y * new_y);
                    if (isSouth)
                        new_x -= 10;
                    else
                        new_x += 10;
                    if (new_x > 0)
                        b1[j, i].dir = WindDir.E;
                    else
                        b1[j, i].dir = WindDir.W;
                    if (new_y > 0)
                        b1[j, i].dir |= WindDir.N;
                    else
                        b1[j, i].dir |= WindDir.S;


                }
            }
            return b1;
        }
    }

    public class Gaussian
    {
        public static float[,] GaussianConvolution(float[,] matrix, float deviation)
        {
            float[,] kernel = CalculateNormalized1DSampleKernel(deviation);
            float[,] res1 = new float[matrix.GetLength(0), matrix.GetLength(1)];
            float[,] res2 = new float[matrix.GetLength(0), matrix.GetLength(1)];
            //x-direction
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                    res1[i, j] = processPoint(matrix, i, j, kernel, 0);
            }
            //y-direction
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                    res2[i, j] = processPoint(res1, i, j, kernel, 1);
            }
            return res2;
        }
        private static float processPoint(float[,] matrix, int x, int y, float[,] kernel, int direction)
        {
            float res = 0;
            int half = kernel.GetLength(0) / 2;
            for (int i = 0; i < kernel.GetLength(0); i++)
            {
                int cox = direction == 0 ? x + i - half : x;
                int coy = direction == 1 ? y + i - half : y;
                if (cox >= 0 && cox < matrix.GetLength(0) && coy >= 0 && coy < matrix.GetLength(1))
                {
                    res += matrix[cox, coy] * kernel[i, 0];
                }
            }
            return res;
        }
        public static float[,] Calculate1DSampleKernel(float deviation, int size)
        {
            float[,] ret = new float[size, 1];
            double sum = 0;
            int half = size / 2;
            for (int i = 0; i < size; i++)
            {
                ret[i, 0] = (float)(1 / (Math.Sqrt(2 * Math.PI) * deviation) * Math.Exp(-(i - half) * (i - half) / (2 * deviation * deviation)));
                sum += ret[i, 0];
            }
            return ret;
        }
        public static float[,] Calculate1DSampleKernel(float deviation)
        {
            int size = (int)Math.Ceiling(deviation * 3) * 2 + 1;
            return Calculate1DSampleKernel(deviation, size);
        }
        public static float[,] CalculateNormalized1DSampleKernel(float deviation)
        {
            return NormalizeMatrix(Calculate1DSampleKernel(deviation));
        }
        public static float[,] NormalizeMatrix(float[,] matrix)
        {
            float[,] ret = new float[matrix.GetLength(0), matrix.GetLength(1)];
            float sum = 0;
            for (int i = 0; i < ret.GetLength(0); i++)
            {
                for (int j = 0; j < ret.GetLength(1); j++)
                    sum += matrix[i, j];
            }
            if (sum != 0)
            {
                for (int i = 0; i < ret.GetLength(0); i++)
                {
                    for (int j = 0; j < ret.GetLength(1); j++)
                        ret[i, j] = matrix[i, j] / sum;
                }
            }
            return ret;
        }
    }
}
