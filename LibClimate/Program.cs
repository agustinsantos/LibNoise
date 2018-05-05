using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace LibClimate
{
    class Program
    {
        public delegate Color ToColorDelegate(double val, double scale = 1);
        public delegate Color ToBoolColorDelegate(bool val);

        /// <summary>
        ///  Data from http://hypertextbook.com/facts/2007/LilyLi.shtml
        /// </summary>
        public static readonly double[] seaSurfaceMeanTemp = new double[] {
            15.8, // J	
            15.9, // F
            15.9, // M
            16.0, // A	
            16.3, // M
            16.4, // J
            16.4, // J
            16.4, // A
            16.2, // S
            15.9, // O
            15.8, // N
            15.7  // D
        };

        /// <summary>
        /// Data from http://hypertextbook.com/facts/2000/MichaelLevin.shtml
        /// </summary>
        public static readonly double[] atmSurfaceMeanTemp = new double[] {
            13.6, // J	
            14.3, // F
            16.0, // M
            18.7, // A
            21.5, // M
            24.7, // J
            26.4, // J	
            27.5, // A	
            25.6, // S	
            22.4, // O	
            19.0, // N	
            14.7  // D	
         };

        static void Main(string[] args)
        {
            TestInitWorld();
            //TestDataStructure();
            //TestConversion();
            //TestEnergyFunctions();
            //TestEnergyFlow();

            Console.WriteLine("Enter to end");
            Console.ReadLine();
        }

        static void TestInitWorld()
        {
            TWorld world = new TWorld();
            Debug.WriteLine("Init World");
            initmodel.initWorld(world);
            WriteGridToImage(world.elevation, "elevation.bmp", TerrainGradient);

            TClima clima = new TClima();
            Debug.WriteLine("Init Clima");
            for (int month = 0; month < 12; month++)
            {
                initmodel.initClima(world, clima, atmSurfaceMeanTemp[month], seaSurfaceMeanTemp[month]);
                WriteGridToImage(clima.T_atmosphere[0], "T_atmosphere-" + month + ".bmp", TemperatureGradient);
                WriteGridToImage(clima.T_ocean_terr, "T_ocean_terr-" + month + ".bmp", TemperatureGradient);
                WriteBoolGridToImage(clima.isIce, "isIce" + month + ".bmp", BoolColor);
            }
            initmodel.initClima(world, clima, 16, 16);
            WriteGridToImage(clima.T_ocean_terr, "T_ocean_terr.bmp", TemperatureGradient);
            WriteBoolGridToImage(clima.isIce, "isIce.bmp", BoolColor);

        }

        static void TestTwoDays()
        {
            double[,] tmpGrid = new double[360, 180];
            short[,] wind = new short[360, 180];
            double[,] T_atmosphere = new double[360, 180];

            TWorld world = new TWorld();
            Debug.WriteLine("Init World");
            initmodel.initWorld(world);

            TClima clima = new TClima();
            Debug.WriteLine("Init Clima");
            initmodel.initClima(world, clima, 16, 16);

            TTime t = new TTime();
            TSolarSurface s = new TSolarSurface();
            Debug.WriteLine("Init time and solar surface");
            initmodel.initTime(t, s);
            Debug.WriteLine("Initial conditions created");

            Debug.WriteLine("Simulating 3 days of weather including energy cycle, winds,");
            Debug.WriteLine("marine currents, steam generation and rain");
            for (int day = 170; day < 173; day++)
            {
                Debug.Write("Simulating day = " + day + " ");

                for (int hour = 0; hour < 24; hour++)
                {
                    Debug.Write(".");
                    riverandlakes.clearRain(clima);
                    double earthInclination = energyfunctions.computeEarthDeclination(day);
                    for (int j = 0; j < 180; j++)
                        for (int i = 0; i < 360; i++)
                        {
                            energyfunctions.updateIncomingEnergyOnCellGrid(clima, world, s, earthInclination, i, j);
                            watercycle.formSteam(clima, world, s, i, j, t.day);
                        }

                    flux.moveEnergy(clima, clima.energy_atmosphere, tmpGrid, T_atmosphere, wind, flux.WIND, world, true);
#if TODO
                    flux.moveEnergy(clima, clima.energy_ocean_terr, tmpGrid, clima.T_ocean_terr, clima.surfaceTransfer, flux.SURFACE_AND_MARINE_CURRENT, world, true);
                    //printPlanet(2000, day, hour, clima, world, true, false);
                    watercycle.moveSteam(clima.wind, clima.steam, tmpGrid);
#endif
                }
            }
        }


        public static void WriteGridToImage(Grid grid, string destFilename, ToColorDelegate ToColor, bool mustScale = false)
        {
            int Width = grid.Width;
            int Height = grid.Height;
            double scale = 1;
            if (mustScale)
            {
                grid.ComputeStatistics();
                scale = 1 / grid.Maximum;
            }

            //Here create the Bitmap to the know height, width and format
            Bitmap bmp = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);
            //First Create the instance of Stopwatch Class
            Stopwatch sw = new Stopwatch();

            // Start The StopWatch ...From 000
            sw.Start();
            // get source bitmap pixel format size
            int Depth = System.Drawing.Bitmap.GetPixelFormatSize(bmp.PixelFormat) / 8;
            byte[] data = new byte[Width * Height * Depth];

            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                {
                    // Get start index of the specified pixel
                    int i = ((y * Width) + x) * Depth;
                    Color sColor = ToColor(grid[x, y], scale);
                    data[i + 3] = sColor.A;
                    data[i + 2] = sColor.R;
                    data[i + 1] = sColor.G;
                    data[i + 0] = sColor.B;
                }

            //Create a BitmapData and Lock all pixels to be written 
            BitmapData bmpData = bmp.LockBits(
                                 new Rectangle(0, 0, bmp.Width, bmp.Height),
                                 ImageLockMode.WriteOnly, bmp.PixelFormat);

            //Copy the data from the byte array into BitmapData.Scan0
            Marshal.Copy(data, 0, bmpData.Scan0, data.Length);

            //Unlock the pixels
            bmp.UnlockBits(bmpData);
            bmp.Save(destFilename);
            sw.Stop();
            //Writing Execution Time
            string ExecutionTimeTaken = string.Format("Minutes :{0}\tSeconds :{1}\t Mili seconds :{2}", sw.Elapsed.Minutes, sw.Elapsed.Seconds, sw.Elapsed.TotalMilliseconds);
            Debug.WriteLine("WriterBMP: " + ExecutionTimeTaken);
        }

        public static Color TerrainGradient(double val, double scale = 1)
        {
            if (val < 5000 * -1.0000) return Color.FromArgb(255, 0, 0, 128); // deeps
            else if (val < 5000 * -0.2500) return Color.FromArgb(255, 0, 0, 255); // shallow
            else if (val < 0.0000) return Color.FromArgb(255, 0, 128, 255); // shore
            else if (val < 5000 * 0.0625) return Color.FromArgb(255, 240, 240, 64); // sand
            else if (val < 5000 * 0.1250) return Color.FromArgb(255, 32, 160, 0); // grass
            else if (val < 5000 * 0.3750) return Color.FromArgb(255, 224, 224, 0); // dirt
            else if (val < 5000 * 0.7500) return Color.FromArgb(255, 128, 128, 128); // rock
            else return Color.FromArgb(255, 255, 255, 255); // snow
        }

        public static Color TemperatureGradient(double val, double scale = 1)
        {
            if (val < 270) return Color.FromArgb(255, 7, 30, 70);
            else if (val < 275) return Color.FromArgb(255, 7, 47, 107);
            else if (val < 280) return Color.FromArgb(255, 8, 82, 156);
            else if (val < 285) return Color.FromArgb(255, 33, 113, 181);
            else if (val < 290) return Color.FromArgb(255, 66, 146, 199);
            else if (val < 295) return Color.FromArgb(255, 90, 160, 205);
            else if (val < 300) return Color.FromArgb(255, 120, 191, 214);
            else if (val < 305) return Color.FromArgb(255, 170, 220, 230);
            else if (val < 310) return Color.FromArgb(255, 219, 245, 255);
            else if (val < 315) return Color.FromArgb(255, 240, 252, 255);
            else if (val < 320) return Color.FromArgb(255, 255, 240, 245);
            else if (val < 325) return Color.FromArgb(255, 255, 224, 224);
            else if (val < 330) return Color.FromArgb(255, 252, 187, 170);
            else if (val < 335) return Color.FromArgb(255, 252, 146, 114);
            else if (val < 340) return Color.FromArgb(255, 251, 106, 74);
            else if (val < 345) return Color.FromArgb(255, 240, 60, 43);
            else if (val < 350) return Color.FromArgb(255, 204, 24, 30);
            else if (val < 355) return Color.FromArgb(255, 166, 15, 20);
            else if (val < 360) return Color.FromArgb(255, 120, 10, 15);
            else return Color.FromArgb(255, 95, 0, 0);
        }

        public static Color ScaleGradient(double origval, double scale = 1)
        {
            double val = origval * scale;
            if (val < 0.05) return Color.FromArgb(255, 7, 30, 70);
            else if (val < 0.1) return Color.FromArgb(255, 7, 47, 107);
            else if (val < 0.15) return Color.FromArgb(255, 8, 82, 156);
            else if (val < 0.2) return Color.FromArgb(255, 33, 113, 181);
            else if (val < 0.25) return Color.FromArgb(255, 66, 146, 199);
            else if (val < 0.3) return Color.FromArgb(255, 90, 160, 205);
            else if (val < 0.35) return Color.FromArgb(255, 120, 191, 214);
            else if (val < 0.4) return Color.FromArgb(255, 170, 220, 230);
            else if (val < 0.45) return Color.FromArgb(255, 219, 245, 255);
            else if (val < 0.5) return Color.FromArgb(255, 240, 252, 255);
            else if (val < 0.55) return Color.FromArgb(255, 255, 240, 245);
            else if (val < 0.6) return Color.FromArgb(255, 255, 224, 224);
            else if (val < 0.65) return Color.FromArgb(255, 252, 187, 170);
            else if (val < 0.7) return Color.FromArgb(255, 252, 146, 114);
            else if (val < 0.75) return Color.FromArgb(255, 251, 106, 74);
            else if (val < 0.8) return Color.FromArgb(255, 240, 60, 43);
            else if (val < 0.85) return Color.FromArgb(255, 204, 24, 30);
            else if (val < 0.9) return Color.FromArgb(255, 166, 15, 20);
            else if (val < 0.95) return Color.FromArgb(255, 120, 10, 15);
            else return Color.FromArgb(255, 95, 0, 0);
        }

        public static void WriteBoolGridToImage(BoolGrid grid, string destFilename, ToBoolColorDelegate ToColor)
        {
            int Width = grid.Width;
            int Height = grid.Height;

            //Here create the Bitmap to the know height, width and format
            Bitmap bmp = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);
            //First Create the instance of Stopwatch Class
            Stopwatch sw = new Stopwatch();

            // Start The StopWatch ...From 000
            sw.Start();
            // get source bitmap pixel format size
            int Depth = System.Drawing.Bitmap.GetPixelFormatSize(bmp.PixelFormat) / 8;
            byte[] data = new byte[Width * Height * Depth];

            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                {
                    // Get start index of the specified pixel
                    int i = ((y * Width) + x) * Depth;
                    Color sColor = ToColor(grid[x, y]);
                    data[i + 3] = sColor.A;
                    data[i + 2] = sColor.R;
                    data[i + 1] = sColor.G;
                    data[i + 0] = sColor.B;
                }

            //Create a BitmapData and Lock all pixels to be written 
            BitmapData bmpData = bmp.LockBits(
                                 new Rectangle(0, 0, bmp.Width, bmp.Height),
                                 ImageLockMode.WriteOnly, bmp.PixelFormat);

            //Copy the data from the byte array into BitmapData.Scan0
            Marshal.Copy(data, 0, bmpData.Scan0, data.Length);

            //Unlock the pixels
            bmp.UnlockBits(bmpData);
            bmp.Save(destFilename);
            sw.Stop();
            //Writing Execution Time
            string ExecutionTimeTaken = string.Format("Minutes :{0}\tSeconds :{1}\t Mili seconds :{2}", sw.Elapsed.Minutes, sw.Elapsed.Seconds, sw.Elapsed.TotalMilliseconds);
            Debug.WriteLine("WriterBMP: " + ExecutionTimeTaken);
        }

        public static Color BoolColor(bool val)
        {
            if (val)
                return Color.FromArgb(255, 255, 255, 255);
            else
                return Color.FromArgb(255, 10, 10, 10);
        }

        private static void TestDataStructure()
        {
            TClima clima = new TClima();
            TWorld world = new TWorld();
            double sum = 0;

            Console.WriteLine("Init world");
            initmodel.initWorld(world, "");
            Console.WriteLine("Init clima");
            initmodel.initClima(world, clima, 16, 16, "");
            Console.WriteLine("");
            Console.WriteLine("Test of data structure");
            Console.WriteLine("");
            Console.WriteLine("Test of area of a degree squared");
            for (int j = 0; j < 180; j++)
                sum = sum + world.area_of_degree_squared[j];

            sum = sum * 360;
            Console.WriteLine("Sum of surface of degree squared across one intitude * 360 is " + sum);
            Console.WriteLine("Total surface of a sphere is " + (4 * Math.PI * TPhysConst.earth_radius * TPhysConst.earth_radius));
            Console.WriteLine("");

            for (int j = 90; j > 0; j--)
                Console.WriteLine("Area of degree squared lat +" + j + " :" + world.area_of_degree_squared[Conversion.LatToY(j)]);

            Console.WriteLine("Area of degree squared lat +89 " + world.area_of_degree_squared[Conversion.LatToY(89)]);
            Console.WriteLine("Area of degree squared lat +88 " + world.area_of_degree_squared[Conversion.LatToY(88)]);
            Console.WriteLine("Area of degree squared lat +01 " + world.area_of_degree_squared[Conversion.LatToY(1)]);
            Console.WriteLine("Area of degree squared lat +00 " + world.area_of_degree_squared[Conversion.LatToY(0)]);
            Console.WriteLine("Area of degree squared lat -01 " + world.area_of_degree_squared[Conversion.LatToY(-1)]);
            Console.WriteLine("Area of degree squared lat -88 " + world.area_of_degree_squared[Conversion.LatToY(-88)]);
            Console.WriteLine("Area of degree squared lat -89 " + world.area_of_degree_squared[Conversion.LatToY(-89)]);
            Console.WriteLine("Area of degree squared lat -90 " + world.area_of_degree_squared[Conversion.LatToY(-90)]);
        }

        #region TestConversion
        public static void TestConversion()
        {
            TClima clima = new TClima();
            TWorld world = new TWorld();

            Console.WriteLine("Init world");
            initmodel.initWorld(world, "");
            Console.WriteLine("Init clima");
            initmodel.initClima(world, clima, 16, 16);

            Console.WriteLine("Test of Conversion routines");
            Console.WriteLine();
            Console.WriteLine("0 grad Celsius in Kelvin " + Conversion.CtoK(0));
            Console.WriteLine("-273.15 grad Celsius in Kelvin " + Conversion.CtoK(-273.15));
            Console.WriteLine();
            Console.WriteLine("0      grad Kelvin in Celsius " + Conversion.KtoC(0));
            Console.WriteLine("273.16 grad Kelvin in Celsius " + Conversion.KtoC(273.16));
            Console.WriteLine("373.16 grad Kelvin in Celsius " + Conversion.KtoC(373.16));
            Console.WriteLine();

            Console.WriteLine("Longitude 180 deg W on grid " + Conversion.LonToX(-180));
            Console.WriteLine("Longitude 179 deg W on grid " + Conversion.LonToX(-179));
            Console.WriteLine("Longitude 1 deg W on grid " + Conversion.LonToX(-1));
            Console.WriteLine("Longitude 0.3 deg W on grid " + Conversion.LonToX(-0.3));
            Console.WriteLine("Longitude Greenwich on grid " + Conversion.LonToX(0));
            Console.WriteLine("Longitude 0.3 deg E on grid " + Conversion.LonToX(0.3));
            Console.WriteLine("Longitude 1 deg E on grid " + Conversion.LonToX(1));
            Console.WriteLine("Longitude 179 deg E on grid " + Conversion.LonToX(179));
            Console.WriteLine("Longitude 180 deg E on grid " + Conversion.LonToX(180));
            Console.WriteLine();
            Console.WriteLine("Latitudine North Pole on grid " + Conversion.LatToY(90));
            Console.WriteLine("Latitudine 89.5 deg on grid " + Conversion.LatToY(89.3));
            Console.WriteLine("Latitudine 89 deg on grid " + Conversion.LatToY(89));
            Console.WriteLine("Latitudine 88.3 deg on grid " + Conversion.LatToY(88.3));
            Console.WriteLine("Latitudine 88 deg on grid " + Conversion.LatToY(88));
            Console.WriteLine("Latitudine Poschiavo on grid " + Conversion.LatToY(45));
            Console.WriteLine("Latitudine 0.3 deg on grid " + Conversion.LatToY(0.3));
            Console.WriteLine("Latitudine equator on grid " + Conversion.LatToY(0));
            Console.WriteLine("Latitudine -0.3 deg on grid " + Conversion.LatToY(-0.3));
            Console.WriteLine("Latitudine -88 deg on grid " + Conversion.LatToY(-88));
            Console.WriteLine("Latitudine -89 deg on grid " + Conversion.LatToY(-89));
            Console.WriteLine("Latitudine -89.5 deg on grid " + Conversion.LatToY(-89.3));
            Console.WriteLine("Latitudine south pole on grid " + Conversion.LatToY(-90));
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Converting twice 90 deg lat N : " + Conversion.YtoLat(Conversion.LatToY(90)));
            Console.WriteLine("Converting twice 0 deg lat : " + Conversion.YtoLat(Conversion.LatToY(0)));
            Console.WriteLine("Converting twice 90 deg lat S: " + Conversion.YtoLat(Conversion.LatToY(-90)));
            Console.WriteLine();
            Console.WriteLine("Converting twice 180 deg lat W : " + Conversion.XtoLon(Conversion.LonToX(-180)));
            Console.WriteLine("Converting twice 0 deg lat : " + Conversion.XtoLon(Conversion.LonToX(0)));
            Console.WriteLine("Converting twice 180 deg lat E: " + Conversion.XtoLon(Conversion.LonToX(180)));
        }
        #endregion


        public static void plotTemperature(TClima clima, TWorld w, int i, int j, bool output)
        {
            energyfunctions.updateTemperature(clima, w, i, j);
            if (output)
            {
                Console.WriteLine("T atmosphere    : " + Conversion.KtoC(clima.T_atmosphere[0, i, j]));
                Console.WriteLine("T ocean/terrain : " + Conversion.KtoC(clima.T_ocean_terr[i, j]));
                Console.WriteLine();
            }
        }

        public static void testEnergyFlowOnSquare(double lat, double lon, int day, TClima clima, TWorld w, bool output)
        {
            double earthInclination, energyIn;
            int i, j;

            i = Conversion.LonToX(lon);
            j = Conversion.LatToY(lat);

            if (output)
            {
                Console.WriteLine("Energy flow on a degree squared at latitude " + lat + " on day " + day);
                Console.Write("Initial conditions on ");
                if (clima.isIce[i, j]) Console.WriteLine("ICE square:");
                else
                    if (w.isOcean[i, j]) Console.WriteLine("OCEAN square:");
                    else
                        Console.WriteLine("TERRAIN square:");
            }

            plotTemperature(clima, w, i, j, output);
            earthInclination = energyfunctions.computeEarthDeclination(day);
            energyIn = energyfunctions.computeEnergyFromSunOnSquare(i, j, earthInclination, clima, w);
            if (output) Console.WriteLine("Energy from Sun entering the square: " + energyIn);
            if (output) Console.WriteLine("Distributing energy between atmosphere and terrain...");
            energyfunctions.spreadEnergyOnAtmosphereAndTerrain(clima, energyIn, i, j);
            if (output) Console.WriteLine("Temperature after insulation: ");
            plotTemperature(clima, w, i, j, output);
            if (output) Console.WriteLine("Exchanging energy between atmosphere and terrain...");
            energyfunctions.exchangeEnergyBetweenAtmAndTerrain(clima, w, i, j);
            if (output) Console.WriteLine("Temperature after exchange: ");
            plotTemperature(clima, w, i, j, output);
            if (output) Console.WriteLine("Radiating energy back into space");
            energyfunctions.radiateEnergyIntoSpace(clima, w, i, j);
            if (output) Console.WriteLine("Temperature after loss into space: ");
            plotTemperature(clima, w, i, j, output);
        }

        public static void testLossOfEnergyDuringNight(double lat, double lon, int day, TClima clima, TWorld w, bool output)
        {
            int i, j;

            i = Conversion.LonToX(lon);
            j = Conversion.LatToY(lat);

            if (output) Console.WriteLine("Exchanging energy between atmosphere and terrain...");
            energyfunctions.exchangeEnergyBetweenAtmAndTerrain(clima, w, i, j);
            if (output) Console.WriteLine("Temperature after exchange: ");
            plotTemperature(clima, w, i, j, output);
            if (output) Console.WriteLine("Radiating energy back into space");
            energyfunctions.radiateEnergyIntoSpace(clima, w, i, j);
            if (output) Console.WriteLine("Temperature after loss into space: ");
            plotTemperature(clima, w, i, j, output);
        }

        public static void TestEnergyFlow()
        {
            TClima clima = new TClima();
            TWorld world = new TWorld();


            Console.WriteLine("Init world");
            initmodel.initWorld(world, "");
            Console.WriteLine("Init clima");
            initmodel.initClima(world, clima, 16, 16);

            Console.WriteLine("Test of energy flow at 22 March Giubiasco (45 deg N, 6 deg E)");
            testEnergyFlowOnSquare(45, 6, 80, clima, world, true);
            Console.WriteLine("-------------------------------------------------");

            Console.WriteLine("Test of energy flow at 22 March Equator Ocean(0 deg S, 3 deg W)");
            testEnergyFlowOnSquare(0, -3, 80, clima, world, true);
            Console.WriteLine("-------------------------------------------------");

            Console.WriteLine("Test of energy flow at 22 March Equator Terrain (3 deg S, 12 deg E)");
            testEnergyFlowOnSquare(-3, 12, 80, clima, world, true);
            Console.WriteLine("-------------------------------------------------");

            Console.WriteLine("Test of energy flow at 22 March North pole (89 deg N, 3 deg W)");
            testEnergyFlowOnSquare(89, -3, 80, clima, world, true);
            Console.WriteLine("-------------------------------------------------");

            Console.WriteLine("Test of energy flow at 22 March South pole (89 deg S, 3 deg W)");
            testEnergyFlowOnSquare(-89, -3, 80, clima, world, true);
            Console.WriteLine("-------------------------------------------------");
            Console.WriteLine();
            Console.WriteLine();

            Console.WriteLine("Six months of full insulation with no nights");
            for (int countDay = 1; countDay <= 180; countDay++)
                for (int countHour = 0; countHour < 24; countHour++)
                {
                    testEnergyFlowOnSquare(0, -3, 80 + countDay, clima, world, false);
                    testEnergyFlowOnSquare(-3, 12, 80 + countDay, clima, world, false);
                    testEnergyFlowOnSquare(45, 6, 80 + countDay, clima, world, false);
                    testEnergyFlowOnSquare(89, -3, 80 + countDay, clima, world, false);
                    testEnergyFlowOnSquare(-89, -3, 80 + countDay, clima, world, false);
                }

            Console.WriteLine("In Giubiasco:");
            plotTemperature(clima, world, Conversion.LonToX(6), Conversion.LatToY(45), true);
            Console.WriteLine("At equator (ocean):");
            plotTemperature(clima, world, Conversion.LonToX(-3), Conversion.LatToY(0), true);
            Console.WriteLine("At equator (terrain):");
            plotTemperature(clima, world, Conversion.LonToX(12), Conversion.LatToY(-3), true);
            Console.WriteLine("North Pole:");
            plotTemperature(clima, world, Conversion.LonToX(-3), Conversion.LatToY(89), true);
            Console.WriteLine("South Pole:");
            plotTemperature(clima, world, Conversion.LonToX(-3), Conversion.LatToY(-89), true);

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Init world");
            initmodel.initWorld(world, "");
            Console.WriteLine("Init clima");
            initmodel.initClima(world, clima, 16, 16);

            Console.WriteLine("Six months of deep night without sun");
            for (int countDay = 1; countDay <= 180; countDay++)
                for (int countHour = 0; countHour < 24; countHour++)
                {
                    testLossOfEnergyDuringNight(0, -3, 80 + countDay, clima, world, false);
                    testLossOfEnergyDuringNight(-3, 12, 80 + countDay, clima, world, false);
                    testLossOfEnergyDuringNight(45, 6, 80 + countDay, clima, world, false);
                    testLossOfEnergyDuringNight(89, -3, 80 + countDay, clima, world, false);
                    testLossOfEnergyDuringNight(-89, -3, 80 + countDay, clima, world, false);
                }

            Console.WriteLine("In Giubiasco:");
            plotTemperature(clima, world, Conversion.LonToX(6), Conversion.LatToY(45), true);
            Console.WriteLine("At equator (ocean):");
            plotTemperature(clima, world, Conversion.LonToX(-3), Conversion.LatToY(0), true);
            Console.WriteLine("At equator (terrain):");
            plotTemperature(clima, world, Conversion.LonToX(12), Conversion.LatToY(-3), true);
            Console.WriteLine("North Pole:");
            plotTemperature(clima, world, Conversion.LonToX(-3), Conversion.LatToY(89), true);
            Console.WriteLine("South Pole:");
            plotTemperature(clima, world, Conversion.LonToX(-3), Conversion.LatToY(-89), true);

        }

        public static void TestEnergyFunctions()
        {
            TClima clima = new TClima();
            TWorld world = new TWorld();

            Console.WriteLine("Init world");
            initmodel.initWorld(world, "");
            Console.WriteLine("Init clima");
            initmodel.initClima(world, clima, 16, 16);

            Console.WriteLine("Test of routines in unit energyfunctions");
            Console.WriteLine();
            Console.WriteLine("Earth inclination (1 January) " + energyfunctions.computeEarthDeclination(1));
            Console.WriteLine();
            Console.WriteLine("Earth inclination (22 March) " + energyfunctions.computeEarthDeclination(80));
            Console.WriteLine("Earth inclination (22 June) " + energyfunctions.computeEarthDeclination(172));
            Console.WriteLine();
            Console.WriteLine("Earth inclination mid of year: " + energyfunctions.computeEarthDeclination(182));
            Console.WriteLine();
            Console.WriteLine("Earth inclination (21 September) " + energyfunctions.computeEarthDeclination(264));
            Console.WriteLine("Earth inclination (22 December) " + energyfunctions.computeEarthDeclination(356));
            Console.WriteLine();
            Console.WriteLine("Earth inclination (31 December) " + energyfunctions.computeEarthDeclination(365));
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();

            Console.WriteLine("To compare the energies, we multiply with the area of degree squared");
            Console.WriteLine("Energy from sun at 22 June at North Pole " +
                      energyfunctions.computeEnergyFactorWithAngle(1, Conversion.LatToY(90), 23.45));
            Console.WriteLine("Energy from sun at 22 June at 45 deg lat N " +
                       energyfunctions.computeEnergyFactorWithAngle(1, Conversion.LatToY(45), 23.45));
            Console.WriteLine("Energy from sun at 22 June at 23.45 deg lat N " +
                       energyfunctions.computeEnergyFactorWithAngle(1, Conversion.LatToY(23.45), 23.45));
            Console.WriteLine("Energy from sun at 22 June at equator  " +
                       energyfunctions.computeEnergyFactorWithAngle(1, Conversion.LatToY(0), 23.45));
            Console.WriteLine("Energy from sun at 22 June at 23.45 deg lat S " +
                       energyfunctions.computeEnergyFactorWithAngle(1, Conversion.LatToY(-23.45), 23.45));
            Console.WriteLine("Energy from sun at 22 June at 45 deg lat S " +
                       energyfunctions.computeEnergyFactorWithAngle(1, Conversion.LatToY(-45), 23.45));
            Console.WriteLine("Energy from sun at 22 June at South Pole " +
                       energyfunctions.computeEnergyFactorWithAngle(1, Conversion.LatToY(-90), 23.45));

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Energy from sun at 22 December at North Pole " +
                       energyfunctions.computeEnergyFactorWithAngle(1, Conversion.LatToY(90), -23.45));
            Console.WriteLine("Energy from sun at 22 December at 45 deg lat N " +
                       energyfunctions.computeEnergyFactorWithAngle(1, Conversion.LatToY(45), -23.45));
            Console.WriteLine("Energy from sun at 22 December at 23.45 deg lat N " +
                       energyfunctions.computeEnergyFactorWithAngle(1, Conversion.LatToY(23.45), -23.45));
            Console.WriteLine("Energy from sun at 22 December at equator  " +
                       energyfunctions.computeEnergyFactorWithAngle(1, Conversion.LatToY(0), -23.45));
            Console.WriteLine("Energy from sun at 22 December at 23.45 deg lat S " +
                       energyfunctions.computeEnergyFactorWithAngle(1, Conversion.LatToY(-23.45), -23.45));
            Console.WriteLine("Energy from sun at 22 December at 45 deg lat S " +
                       energyfunctions.computeEnergyFactorWithAngle(1, Conversion.LatToY(-45), -23.45));
            Console.WriteLine("Energy from sun at 22 December at South Pole " +
                       energyfunctions.computeEnergyFactorWithAngle(1, Conversion.LatToY(-90), -23.45));

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Energy from sun at 22 March/21 September at North Pole " +
                //world.area_of_degree_squared[LatToY(90)] *
                                 energyfunctions.computeEnergyFactorWithAngle(1, Conversion.LatToY(90), 0));
            Console.WriteLine("Energy from sun at 22 March/21 September at 45 deg lat N " +
                //world.area_of_degree_squared[LatToY(45)] *
                                 energyfunctions.computeEnergyFactorWithAngle(1, Conversion.LatToY(45), 0));
            Console.WriteLine("Energy from sun at 22 March/21 September at 23.45 deg lat N " +
                //world.arenergyfunctions.ea_of_degree_squared[LatToY(23.45)] *
                                 energyfunctions.computeEnergyFactorWithAngle(1, Conversion.LatToY(23.45), 0));
            Console.WriteLine("Energy from sun at 22 March/21 September at equator  " +
                //world.area_of_degree_squared[LatToY(0)] *
                                 energyfunctions.computeEnergyFactorWithAngle(1, Conversion.LatToY(0), 0));
            Console.WriteLine("Energy from sun at 22 March/21 September at 23.45 deg lat S " +
                //world.area_of_degree_squared[LatToY(-23.45)] *
                                 energyfunctions.computeEnergyFactorWithAngle(1, Conversion.LatToY(-23.45), 0));
            Console.WriteLine("Energy from sun at 22 March/21 September at 45 deg lat S " +
                //world.area_of_degree_squared[LatToY(-45)] *
                                 energyfunctions.computeEnergyFactorWithAngle(1, Conversion.LatToY(-45), 0));
            Console.WriteLine("Energy from sun at 22 March/21 September at South Pole " +
                //world.area_of_degree_squared[LatToY(-90)] *
                                 energyfunctions.computeEnergyFactorWithAngle(1, Conversion.LatToY(-90), 0));
        }
    }
}