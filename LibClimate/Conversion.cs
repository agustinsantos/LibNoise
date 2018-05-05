using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibClimate
{
    public static class Conversion
    {
        public static int[] LatitudeToY = new int[91];
        public static double[] YtoLatitude = new double[91];
        public static bool nonlinear_;

        public static bool IsNonLinearConversion()
        {
            return nonlinear_;
        }


        public static int LonToX(double lon)
        {
            if ((lon < -180) || (lon > 180))
                throw new Exception("Longitude has to be between -180 (W) and +180 (E) but was " + lon);

            if (lon == 180)
                return 0;     // 180 deg E = 180 deg W
            else
                return (int)lon + 180;
        }

        public static double XtoLon(int x)
        {
            if ((x < 0) || (x > 359))
                throw new Exception("x on array has to be between 0 and 359 but was " + x);

            return x - 180;
        }

        public static int LatToY_lin(double lat)
        {
            return (int)(90 - lat);
        }

        public static int LatToY_nonlin(double lat)
        {
            int trn;
            double prt;

            trn = (int)lat;
            prt = Math.Abs((int)(lat));

            if (Math.Abs(lat) < 0.5)
                return 90;
            else
                if (lat < 0)
                {
                    if (prt == 0.5)
                        return (int)(((180 - LatitudeToY[Math.Abs(trn)]) + (180 - LatitudeToY[Math.Abs(trn) + 1])) / 2);
                    else
                        if (prt < 0.5)
                            return 180 - LatitudeToY[Math.Abs(trn)];
                        else
                            return 180 - LatitudeToY[Math.Abs(trn) + 1];
                }
                else
                {
                    // lat > 0;
                    if (prt == 0.5)
                        return (int)((LatitudeToY[trn + 1] + LatitudeToY[trn]) / 2);
                    else
                        if (prt < 0.5)
                            return LatitudeToY[trn];
                        else
                            return LatitudeToY[trn + 1];
                }

        }


        public static double YtoLat_lin(int y)
        {
            return 90 - y;
        }

        public static double YtoLat_nonlin(int y)
        {
            if (y <= 90)
                return YtoLatitude[y];
            else
                return -YtoLatitude[180 - y];
        }

        public static int LatToY(double lat)
        {
            int result;

            if ((lat < -90) || (lat > 90))
                throw new Exception("Latitude has to be between -90 (S) and +90 (N) but was " + lat);

            if (nonlinear_)
                result = LatToY_nonlin(lat);
            else
                result = LatToY_lin(lat);

            if (result == 180) return 179; // Latitude -90 maps to 179

            return result;
        }

        public static double YtoLat(int y)
        {
            if ((y < 0) || (y > 180))
                throw new Exception("y on array has to be between 0 and 180 but was " + y);

            if (nonlinear_)
                return YtoLat_nonlin(y);
            else
                return YtoLat_lin(y);
        }


        public static double KtoC(double k)
        {
            if (k < 0)
                throw new Exception("Temperature in Kelvin can not be negative but was " + k);

            return k - 273.16;
        }

        public static double CtoK(double c)
        {
            if (c < -273.16)
                throw new Exception("Temperature in Celsius has to be higher or equal than absolute zero (-273.16) but was " + c);
            return c + 273.16;
        }

        public static void InitConversion(bool nonlinear)
        {
            int j;
            nonlinear_ = nonlinear;

            // init nonlinear merges and splits tables
            YtoLatitude[0] = 90;
            LatitudeToY[90] = 0;
            LatitudeToY[89] = 0;

            YtoLatitude[1] = 88;
            LatitudeToY[88] = 1;
            LatitudeToY[87] = 1;

            YtoLatitude[2] = 86;
            LatitudeToY[86] = 2;
            LatitudeToY[85] = 2;

            YtoLatitude[3] = 84;
            LatitudeToY[84] = 3;
            LatitudeToY[83] = 3;

            YtoLatitude[4] = 82;
            LatitudeToY[82] = 4;
            LatitudeToY[81] = 4;

            // bijection
            for (j = 5; j < 79; j++)
            {
                YtoLatitude[j] = 85 - j; LatitudeToY[85 - j] = j;
            }

            // splits
            YtoLatitude[80] = 5; LatitudeToY[5] = 80;
            YtoLatitude[81] = 4.5;

            YtoLatitude[82] = 4; LatitudeToY[4] = 82;
            YtoLatitude[83] = 3.5;

            YtoLatitude[84] = 3; LatitudeToY[3] = 84;
            YtoLatitude[85] = 2.5;

            YtoLatitude[86] = 2; LatitudeToY[2] = 86;
            YtoLatitude[87] = 1.5;

            YtoLatitude[88] = 1; LatitudeToY[1] = 88;
            YtoLatitude[89] = 0.5;

            YtoLatitude[90] = 0; LatitudeToY[0] = 90;
        }

    }
}
