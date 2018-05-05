using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibClimate
{
    public class Constants
    {
        public const int MAX_ATM_LAYERS = 10;    // {maximum number of atmospheric layers }

        public const int ATMOSPHERE = 100;
        public const int OCEAN_TERR = 200;
        public const int AVERAGE = 300;
        public const int OCEAN = 400;
        public const int TERRAIN = 500;
        public const int AIR_OVER_OCEAN = 600;
        public const int AIR_OVER_TERRAIN = 700;

        public const int FULL_NIGHT = -1; // to identify FULL_NIGHT

        public const int NONE = 0;
        public const int NORTH = 1;
        public const int SOUTH = -1;
        public const int WEST = -2;
        public const int EAST = 2;
        public const int NORTH_WEST = -3;
        public const int NORTH_EAST = 4;
        public const int SOUTH_WEST = -4;
        public const int SOUTH_EAST = 3;
    }

    public struct TTime
    {
        public long year;
        public long day;
        public double hour;
    }
    //type PTime = ^TTime;

    public class TSolarSurface
    {
        public long[] degstart = new long[180];
        public long[] degend = new long[180];
    }
    //type PSolarSurface = ^TSolarSurface;

    public class TWorld
    {
        public Grid elevation = new Grid(360, 180);
        public BoolGrid isOcean = new BoolGrid(360, 180);
        public double[] area_of_degree_squared = new double[180];
        public double[] length_of_degree = new double[180];
    }

    //type PWorld = ^TWorld;
    //type  TColor = Longint;
    //type  double = Extended;
    //type  TLatitude = Array[0..179] of double;
    //type  TGrid = Array [0..359] of Array [0..179] of double;
    //type  TLayersGrid = Array [0..MAX_ATM_LAYERS-1] of TGrid;
    //type  TGridBoolean = Array [0..359] of Array [0..179] of Boolean;
    //type  TGridShortInt = Array [0..359] of Array [0..179] of Shortint;
    //type  TLayersGridShortInt = Array [0..MAX_ATM_LAYERS-1] of TGridShortInt;
    //type  TGridLongint = Array [0..359] of Array [0..179] of Longint;
    //type  TGridColor = Array [0..359] of Array [0..179] of TColor;
    //type  PGrid = ^TGrid;
    //type  PGridShortInt = ^TGridShortInt;
    //type  PGridColor = ^TGridColor;
    public class TClima
    {
        public double[,] energy_atmosphere = new double[360, 180];
        public double[,] energy_ocean_terr = new double[360, 180];

        public LayersGrid T_atmosphere = new LayersGrid(Constants.MAX_ATM_LAYERS, 360, 180); //(in Kelvin)!
        public Grid T_ocean_terr = new Grid(360, 180); //(in Kelvin)!

        public short[, ,] wind = new short[Constants.MAX_ATM_LAYERS, 360, 180];
        public double[,] surfaceTransfer = new double[360, 180];

        public double[, ,] steam = new double[Constants.MAX_ATM_LAYERS, 360, 180];
        public Grid humidity = new Grid(360, 180);

        public bool[,] rain = new bool[360, 180];
        public double[,] water_surface = new double[360, 180];
        public long[,] rain_times = new long[360, 180];

        public double[] avgWaterSurface = new double[180];

        public Grid population = new Grid(360, 180);
        public double[,] co2_tons = new double[360, 180];
        public double[,] ashes_pct = new double[360, 180];

        public BoolGrid isIce = new BoolGrid(360, 180);
    }
    //type PClima = ^TClima;
}
