﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noise
{
    /// Enumerates the noise quality.
    public enum NoiseQuality
    {

        /// Generates coherent noise quickly.  When a coherent-noise function with
        /// this quality setting is used to generate a bump-map image, there are
        /// noticeable "creasing" artifacts in the resulting image.  This is
        /// because the derivative of that function is discontinuous at integer
        /// boundaries.
        QUALITY_FAST = 0,

        /// Generates standard-quality coherent noise.  When a coherent-noise
        /// function with this quality setting is used to generate a bump-map
        /// image, there are some minor "creasing" artifacts in the resulting
        /// image.  This is because the second derivative of that function is
        /// discontinuous at integer boundaries.
        QUALITY_STD = 1,

        /// Generates the best-quality coherent noise.  When a coherent-noise
        /// function with this quality setting is used to generate a bump-map
        /// image, there are no "creasing" artifacts in the resulting image.  This
        /// is because the first and second derivatives of that function are
        /// continuous at integer boundaries.
        QUALITY_BEST = 2

    }


    public static class NoiseGen
    {
        /// Generates a gradient-coherent-noise value from the coordinates of a
        /// three-dimensional input value.
        ///
        /// @param x The @a x coordinate of the input value.
        /// @param y The @a y coordinate of the input value.
        /// @param z The @a z coordinate of the input value.
        /// @param seed The random number seed.
        /// @param noiseQuality The quality of the coherent-noise.
        ///
        /// @returns The generated gradient-coherent-noise value.
        ///
        /// The return value ranges from -1.0 to +1.0.
        ///
        /// For an explanation of the difference between <i>gradient</i> noise and
        /// <i>value</i> noise, see the comments for the GradientNoise3D() function.
        public static double GradientCoherentNoise3D(double x, double y, double z, int seed = 0,
          NoiseQuality noiseQuality = NoiseQuality.QUALITY_STD)
        {
            // Create a unit-length cube aligned along an integer boundary.  This cube
            // surrounds the input point.
            int x0 = (x > 0.0 ? (int)x : (int)x - 1);
            int x1 = x0 + 1;
            int y0 = (y > 0.0 ? (int)y : (int)y - 1);
            int y1 = y0 + 1;
            int z0 = (z > 0.0 ? (int)z : (int)z - 1);
            int z1 = z0 + 1;

            // Map the difference between the coordinates of the input value and the
            // coordinates of the cube's outer-lower-left vertex onto an S-curve.
            double xs = 0, ys = 0, zs = 0;
            switch (noiseQuality)
            {
                case NoiseQuality.QUALITY_FAST:
                    xs = (x - (double)x0);
                    ys = (y - (double)y0);
                    zs = (z - (double)z0);
                    break;
                case NoiseQuality.QUALITY_STD:
                    xs = Interp.SCurve3(x - (double)x0);
                    ys = Interp.SCurve3(y - (double)y0);
                    zs = Interp.SCurve3(z - (double)z0);
                    break;
                case NoiseQuality.QUALITY_BEST:
                    xs = Interp.SCurve5(x - (double)x0);
                    ys = Interp.SCurve5(y - (double)y0);
                    zs = Interp.SCurve5(z - (double)z0);
                    break;
            }
            // Now calculate the noise values at each vertex of the cube.  To generate
            // the coherent-noise value at the input point, interpolate these eight
            // noise values using the S-curve value as the interpolant (trilinear
            // interpolation.)
            double n0, n1, ix0, ix1, iy0, iy1;
            n0 = GradientNoise3D(x, y, z, x0, y0, z0, seed);
            n1 = GradientNoise3D(x, y, z, x1, y0, z0, seed);
            ix0 = Interp.LinearInterp(n0, n1, xs);
            n0 = GradientNoise3D(x, y, z, x0, y1, z0, seed);
            n1 = GradientNoise3D(x, y, z, x1, y1, z0, seed);
            ix1 = Interp.LinearInterp(n0, n1, xs);
            iy0 = Interp.LinearInterp(ix0, ix1, ys);
            n0 = GradientNoise3D(x, y, z, x0, y0, z1, seed);
            n1 = GradientNoise3D(x, y, z, x1, y0, z1, seed);
            ix0 = Interp.LinearInterp(n0, n1, xs);
            n0 = GradientNoise3D(x, y, z, x0, y1, z1, seed);
            n1 = GradientNoise3D(x, y, z, x1, y1, z1, seed);
            ix1 = Interp.LinearInterp(n0, n1, xs);
            iy1 = Interp.LinearInterp(ix0, ix1, ys);

            return Interp.LinearInterp(iy0, iy1, zs);
        }

        /// Generates a gradient-noise value from the coordinates of a
        /// three-dimensional input value and the integer coordinates of a
        /// nearby three-dimensional value.
        ///
        /// @param fx The floating-point @a x coordinate of the input value.
        /// @param fy The floating-point @a y coordinate of the input value.
        /// @param fz The floating-point @a z coordinate of the input value.
        /// @param ix The integer @a x coordinate of a nearby value.
        /// @param iy The integer @a y coordinate of a nearby value.
        /// @param iz The integer @a z coordinate of a nearby value.
        /// @param seed The random number seed.
        ///
        /// @returns The generated gradient-noise value.
        ///
        /// @pre The difference between @a fx and @a ix must be less than or equal
        /// to one.
        ///
        /// @pre The difference between @a fy and @a iy must be less than or equal
        /// to one.
        ///
        /// @pre The difference between @a fz and @a iz must be less than or equal
        /// to one.
        ///
        /// A <i>gradient</i>-noise function generates better-quality noise than a
        /// <i>value</i>-noise function.  Most noise modules use gradient noise for
        /// this reason, although it takes much longer to calculate.
        ///
        /// The return value ranges from -1.0 to +1.0.
        ///
        /// This function generates a gradient-noise value by performing the
        /// following steps:
        /// - It first calculates a random normalized vector based on the
        ///   nearby integer value passed to this function.
        /// - It then calculates a new value by adding this vector to the
        ///   nearby integer value passed to this function.
        /// - It then calculates the dot product of the above-generated value
        ///   and the floating-point input value passed to this function.
        ///
        /// A noise function differs from a random-number generator because it
        /// always returns the same output value if the same input value is passed
        /// to it.
        public static double GradientNoise3D(double fx, double fy, double fz, int ix, int iy,
          int iz, int seed = 0)
        {
            // Randomly generate a gradient vector given the integer coordinates of the
            // input value.  This implementation generates a random number and uses it
            // as an index into a normalized-vector lookup table.
            int vectorIndex = (
                X_NOISE_GEN * ix
              + Y_NOISE_GEN * iy
              + Z_NOISE_GEN * iz
              + SEED_NOISE_GEN * seed)
              & 0xffffff;
            vectorIndex ^= (vectorIndex >> SHIFT_NOISE_GEN);
            vectorIndex &= 0xff;

            double xvGradient = VectorTbl.g_randomVectors[(vectorIndex << 2)];
            double yvGradient = VectorTbl.g_randomVectors[(vectorIndex << 2) + 1];
            double zvGradient = VectorTbl.g_randomVectors[(vectorIndex << 2) + 2];

            // Set up us another vector equal to the distance between the two vectors
            // passed to this function.
            double xvPoint = (fx - (double)ix);
            double yvPoint = (fy - (double)iy);
            double zvPoint = (fz - (double)iz);

            // Now compute the dot product of the gradient vector with the distance
            // vector.  The resulting value is gradient noise.  Apply a scaling value
            // so that this noise value ranges from -1.0 to 1.0.
            return ((xvGradient * xvPoint)
              + (yvGradient * yvPoint)
              + (zvGradient * zvPoint)) * 2.12;
        }

        /// Generates an integer-noise value from the coordinates of a
        /// three-dimensional input value.
        ///
        /// @param x The integer @a x coordinate of the input value.
        /// @param y The integer @a y coordinate of the input value.
        /// @param z The integer @a z coordinate of the input value.
        /// @param seed A random number seed.
        ///
        /// @returns The generated integer-noise value.
        ///
        /// The return value ranges from 0 to 2147483647.
        ///
        /// A noise function differs from a random-number generator because it
        /// always returns the same output value if the same input value is passed
        /// to it.
        public static int IntValueNoise3D(int x, int y, int z, int seed = 0)
        {
            // All constants are primes and must remain prime in order for this noise
            // function to work correctly.
            int n = (
                X_NOISE_GEN * x
              + Y_NOISE_GEN * y
              + Z_NOISE_GEN * z
              + SEED_NOISE_GEN * seed)
              & 0x7fffffff;
            n = (n >> 13) ^ n;
            return (n * (n * n * 60493 + 19990303) + 1376312589) & 0x7fffffff;
        }

        /// Modifies a floating-point value so that it can be stored in a
        /// noise::int32 variable.
        ///
        /// @param n A floating-point number.
        ///
        /// @returns The modified floating-point number.
        ///
        /// This function does not modify @a n.
        ///
        /// In libnoise, the noise-generating algorithms are all integer-based;
        /// they use variables of type noise::int32.  Before calling a noise
        /// function, pass the @a x, @a y, and @a z coordinates to this function to
        /// ensure that these coordinates can be cast to a noise::int32 value.
        ///
        /// Although you could do a straight cast from double to noise::int32, the
        /// resulting value may differ between platforms.  By using this function,
        /// you ensure that the resulting value is identical between platforms.
        public static double MakeInt32Range(double n)
        {
            if (n >= 1073741824.0)
            {
                return (2.0 * (n % 1073741824.0)) - 1073741824.0;
            }
            else if (n <= -1073741824.0)
            {
                return (2.0 * (n % 1073741824.0)) + 1073741824.0;
            }
            else
            {
                return n;
            }
        }

        /// Generates a value-coherent-noise value from the coordinates of a
        /// three-dimensional input value.
        ///
        /// @param x The @a x coordinate of the input value.
        /// @param y The @a y coordinate of the input value.
        /// @param z The @a z coordinate of the input value.
        /// @param seed The random number seed.
        /// @param noiseQuality The quality of the coherent-noise.
        ///
        /// @returns The generated value-coherent-noise value.
        ///
        /// The return value ranges from -1.0 to +1.0.
        ///
        /// For an explanation of the difference between <i>gradient</i> noise and
        /// <i>value</i> noise, see the comments for the GradientNoise3D() function.
        public static double ValueCoherentNoise3D(double x, double y, double z, int seed = 0,
          NoiseQuality noiseQuality = NoiseQuality.QUALITY_STD)
        {
            // Create a unit-length cube aligned along an integer boundary.  This cube
            // surrounds the input point.
            int x0 = (x > 0.0 ? (int)x : (int)x - 1);
            int x1 = x0 + 1;
            int y0 = (y > 0.0 ? (int)y : (int)y - 1);
            int y1 = y0 + 1;
            int z0 = (z > 0.0 ? (int)z : (int)z - 1);
            int z1 = z0 + 1;

            // Map the difference between the coordinates of the input value and the
            // coordinates of the cube's outer-lower-left vertex onto an S-curve.
            double xs = 0, ys = 0, zs = 0;
            switch (noiseQuality)
            {
                case NoiseQuality.QUALITY_FAST:
                    xs = (x - (double)x0);
                    ys = (y - (double)y0);
                    zs = (z - (double)z0);
                    break;
                case NoiseQuality.QUALITY_STD:
                    xs = Interp.SCurve3(x - (double)x0);
                    ys = Interp.SCurve3(y - (double)y0);
                    zs = Interp.SCurve3(z - (double)z0);
                    break;
                case NoiseQuality.QUALITY_BEST:
                    xs = Interp.SCurve5(x - (double)x0);
                    ys = Interp.SCurve5(y - (double)y0);
                    zs = Interp.SCurve5(z - (double)z0);
                    break;
            }

            // Now calculate the noise values at each vertex of the cube.  To generate
            // the coherent-noise value at the input point, interpolate these eight
            // noise values using the S-curve value as the interpolant (trilinear
            // interpolation.)
            double n0, n1, ix0, ix1, iy0, iy1;
            n0 = ValueNoise3D(x0, y0, z0, seed);
            n1 = ValueNoise3D(x1, y0, z0, seed);
            ix0 = Interp.LinearInterp(n0, n1, xs);
            n0 = ValueNoise3D(x0, y1, z0, seed);
            n1 = ValueNoise3D(x1, y1, z0, seed);
            ix1 = Interp.LinearInterp(n0, n1, xs);
            iy0 = Interp.LinearInterp(ix0, ix1, ys);
            n0 = ValueNoise3D(x0, y0, z1, seed);
            n1 = ValueNoise3D(x1, y0, z1, seed);
            ix0 = Interp.LinearInterp(n0, n1, xs);
            n0 = ValueNoise3D(x0, y1, z1, seed);
            n1 = ValueNoise3D(x1, y1, z1, seed);
            ix1 = Interp.LinearInterp(n0, n1, xs);
            iy1 = Interp.LinearInterp(ix0, ix1, ys);
            return Interp.LinearInterp(iy0, iy1, zs);
        }

        /// Generates a value-noise value from the coordinates of a
        /// three-dimensional input value.
        ///
        /// @param x The @a x coordinate of the input value.
        /// @param y The @a y coordinate of the input value.
        /// @param z The @a z coordinate of the input value.
        /// @param seed A random number seed.
        ///
        /// @returns The generated value-noise value.
        ///
        /// The return value ranges from -1.0 to +1.0.
        ///
        /// A noise function differs from a random-number generator because it
        /// always returns the same output value if the same input value is passed
        /// to it.
        public static double ValueNoise3D(int x, int y, int z, int seed = 0)
        {
            return 1.0 - ((double)IntValueNoise3D(x, y, z, seed) / 1073741824.0);
        }

        private const int X_NOISE_GEN = 1619;
        private const int Y_NOISE_GEN = 31337;
        private const int Z_NOISE_GEN = 6971;
        private const int SEED_NOISE_GEN = 1013;
        private const int SHIFT_NOISE_GEN = 8;
    }
}
