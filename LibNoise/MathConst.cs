using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noise
{
    public static class MathConst
    {
        /// Square root of 2.
        public const double SQRT_2 = 1.4142135623730950488;

        /// Square root of 3.
        public const double SQRT_3 = 1.7320508075688772935;

        /// Converts an angle from degrees to radians.
        public const double DEG_TO_RAD = Math.PI / 180.0;

        /// Converts an angle from radians to degrees.
        public const double RAD_TO_DEG = 1.0 / DEG_TO_RAD;
    }
}
