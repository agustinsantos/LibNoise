using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace LibClimate
{
    public class World
    {
        private string worldName = "Earth";

        public string WorldName
        {
            get { return worldName; }
            set { worldName = value; }
        }

        public const int NumLongitudes = 360;
        public const int NumLatitudes = 180;

        public Grid elevation = new Grid(360, 180);
        public BoolGrid isOcean = new BoolGrid(360, 180);
        public double[] area_of_degree_squared = new double[180];
        public double[] length_of_degree = new double[180];

        public void InitWorld(TWorld w, String filePath = null)
        {
            string AppPath;

            InitModelParameters();
            Conversion.InitConversion(false); // default to linear grid

            if (string.IsNullOrWhiteSpace(filePath))
                AppPath = @"config\planet-elevation.txt"; //ExtractFilePath(ParamStr(0));
            else 
                AppPath = filePath;

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
        
        public static void InitModelParameters()
        {
            TMdlConst.Init();
            TInitCond.Init();
            TSimConst.Init();
            TPhysConst.Init();
            TSpecialParam.Init();
        }
    }
}
