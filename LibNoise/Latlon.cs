
using System;
namespace Noise
{
    public static class Latlon
    {
        /// Converts latitude/longitude coordinates on a unit sphere into 3D
        /// Cartesian coordinates.
        ///
        /// @param lat The latitude, in degrees.
        /// @param lon The longitude, in degrees.
        /// @param x On exit, this parameter contains the @a x coordinate.
        /// @param y On exit, this parameter contains the @a y coordinate.
        /// @param z On exit, this parameter contains the @a z coordinate.
        ///
        /// @pre lat must range from @b -90 to @b +90.
        /// @pre lon must range from @b -180 to @b +180.
        public static void LatLonToXYZ(double lat, double lon, out double x, out double y, out double z)
        {
            double r = Math.Cos(MathConst.DEG_TO_RAD * lat);
            x = r * Math.Cos(MathConst.DEG_TO_RAD * lon);
            y = Math.Sin(MathConst.DEG_TO_RAD * lat);
            z = r * Math.Sin(MathConst.DEG_TO_RAD * lon);
        }
    }
}
