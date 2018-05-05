using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibClimate
{
    public class Grid : IGrid<double>, IStatistics
    {
        private double[,] val;

        public Grid(int w, int h)
        {
            val = new double[w, h];
            Width = w;
            Height = h;
        }

        public double this[long row, long column]
        {
            get
            {
                return val[row, column];
            }
            set
            {
                val[row, column] = value;
            }
        }

        public int Width
        {
            get;
            private set;
        }

        public int Height
        {
            get;
            private set;
        }

        public double Maximum
        {
            get;
            private set;
        }

        public double Mean
        {
            get;
            private set;
        }

        public double Minimum
        {
            get;
            private set;
        }

        public void ComputeStatistics()
        {
            double max = double.MinValue;
            double min = double.MaxValue;
            double sum = 0;

            for (int i = 0; i < Width; i++)
                for (int j = 0; j < Height; j++)
                {
                    if (max < val[i, j]) max = val[i, j];
                    if (min > val[i, j]) min = val[i, j];
                    sum += val[i, j];
                }
            Maximum = max;
            Minimum = min;
            Mean = sum / (Width * Height);
        }
    }

    public class BoolGrid : IGrid<bool>
    {
        private bool[,] val;

        public BoolGrid(int w, int h)
        {
            val = new bool[w, h];
            Width = w;
            Height = h;
        }

        public bool this[long row, long column]
        {
            get
            {
                return val[row, column];
            }
            set
            {
                val[row, column] = value;
            }
        }

        public int Width
        {
            get;
            private set;
        }

        public int Height
        {
            get;
            private set;
        }
    }

    public class LayersGrid
    {
        private Grid[] val;

        public LayersGrid(int numLayers, int w, int h)
        {
            val = new Grid[numLayers];
            for (int i = 0; i < numLayers; i++)
                val[i] = new Grid(w, h);
            NumLayers = numLayers;
            Width = w;
            Height = h;
        }
        public Grid this[int nl]
        {
            get
            {
                return val[nl];
            }
        }
        public double this[int nl, long row, long column]
        {
            get
            {
                return val[nl][row, column];
            }
            set
            {
                val[nl][row, column] = value;
            }
        }
        public int NumLayers
        {
            get;
            private set;
        }

        public int Width
        {
            get;
            private set;
        }

        public int Height
        {
            get;
            private set;
        }
    }
}
