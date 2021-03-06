﻿using System;

namespace Noise.Module
{


    /// Noise module that outputs Voronoi cells.
    ///
    /// @image html modulevoronoi.png
    ///
    /// In mathematics, a <i>Voronoi cell</i> is a region containing all the
    /// points that are closer to a specific <i>seed point</i> than to any
    /// other seed point.  These cells mesh with one another, producing
    /// polygon-like formations.
    ///
    /// By default, this noise module randomly places a seed point within
    /// each unit cube.  By modifying the <i>frequency</i> of the seed points,
    /// an application can change the distance between seed points.  The
    /// higher the frequency, the closer together this noise module places
    /// the seed points, which reduces the size of the cells.  To specify the
    /// frequency of the cells, call the SetFrequency() method.
    ///
    /// This noise module assigns each Voronoi cell with a random constant
    /// value from a coherent-noise function.  The <i>displacement value</i>
    /// controls the range of random values to assign to each cell.  The
    /// range of random values is +/- the displacement value.  Call the
    /// SetDisplacement() method to specify the displacement value.
    ///
    /// To modify the random positions of the seed points, call the SetSeed()
    /// method.
    ///
    /// This noise module can optionally add the distance from the nearest
    /// seed to the output value.  To enable this feature, call the
    /// EnableDistance() method.  This causes the points in the Voronoi cells
    /// to increase in value the further away that point is from the nearest
    /// seed point.
    ///
    /// Voronoi cells are often used to generate cracked-mud terrain
    /// formations or crystal-like textures
    ///
    /// This noise module requires no source modules.
    public class Voronoi : Module
    {
        /// Default displacement to apply to each cell for the
        /// noise::module::Voronoi noise module.
        public const double DEFAULT_VORONOI_DISPLACEMENT = 1.0;

        /// Default frequency of the seed points for the noise::module::Voronoi
        /// noise module.
        public const double DEFAULT_VORONOI_FREQUENCY = 1.0;

        /// Default seed of the noise function for the noise::module::Voronoi
        /// noise module.
        public const int DEFAULT_VORONOI_SEED = 0;
        /// Constructor.
        ///
        /// The default displacement value is set to
        /// noise::module::DEFAULT_VORONOI_DISPLACEMENT.
        ///
        /// The default frequency is set to
        /// noise::module::DEFAULT_VORONOI_FREQUENCY.
        ///
        /// The default seed value is set to
        /// noise::module::DEFAULT_VORONOI_SEED.
        public Voronoi()
            : base(0)
        {
            m_displacement = DEFAULT_VORONOI_DISPLACEMENT;
            m_enableDistance = false;
            m_frequency = DEFAULT_VORONOI_FREQUENCY;
            m_seed = DEFAULT_VORONOI_SEED;

        }

        /// Enables or disables applying the distance from the nearest seed
        /// point to the output value.
        ///
        /// @param enable Specifies whether to apply the distance to the
        /// output value or not.
        ///
        /// Applying the distance from the nearest seed point to the output
        /// value causes the points in the Voronoi cells to increase in value
        /// the further away that point is from the nearest seed point.
        /// Setting this value to @a true (and setting the displacement to a
        /// near-zero value) causes this noise module to generate cracked mud
        /// formations.
        public void EnableDistance(bool enable = true)
        {
            m_enableDistance = enable;
        }

        /// Returns the displacement value of the Voronoi cells.
        ///
        /// @returns The displacement value of the Voronoi cells.
        ///
        /// This noise module assigns each Voronoi cell with a random constant
        /// value from a coherent-noise function.  The <i>displacement
        /// value</i> controls the range of random values to assign to each
        /// cell.  The range of random values is +/- the displacement value.
        public double GetDisplacement()
        {
            return m_displacement;
        }

        /// Returns the frequency of the seed points.
        ///
        /// @returns The frequency of the seed points.
        ///
        /// The frequency determines the size of the Voronoi cells and the
        /// distance between these cells.
        public double GetFrequency()
        {
            return m_frequency;
        }

        public override int GetSourceModuleCount()
        {
            return 0;
        }

        /// Returns the seed value used by the Voronoi cells
        ///
        /// @returns The seed value.
        ///
        /// The positions of the seed values are calculated by a
        /// coherent-noise function.  By modifying the seed value, the output
        /// of that function changes.
        public int GetSeed()
        {
            return m_seed;
        }

        /// Determines if the distance from the nearest seed point is applied
        /// to the output value.
        ///
        /// @returns
        /// - @a true if the distance is applied to the output value.
        /// - @a false if not.
        ///
        /// Applying the distance from the nearest seed point to the output
        /// value causes the points in the Voronoi cells to increase in value
        /// the further away that point is from the nearest seed point.
        public bool IsDistanceEnabled()
        {
            return m_enableDistance;
        }

        public override double GetValue(double x, double y, double z)
        {
            // This method could be more efficient by caching the seed values.  Fix
            // later.

            x *= m_frequency;
            y *= m_frequency;
            z *= m_frequency;

            int xInt = (x > 0.0 ? (int)x : (int)x - 1);
            int yInt = (y > 0.0 ? (int)y : (int)y - 1);
            int zInt = (z > 0.0 ? (int)z : (int)z - 1);

            double minDist = 2147483647.0;
            double xCandidate = 0;
            double yCandidate = 0;
            double zCandidate = 0;

            // Inside each unit cube, there is a seed point at a random position.  Go
            // through each of the nearby cubes until we find a cube with a seed point
            // that is closest to the specified position.
            for (int zCur = zInt - 2; zCur <= zInt + 2; zCur++)
            {
                for (int yCur = yInt - 2; yCur <= yInt + 2; yCur++)
                {
                    for (int xCur = xInt - 2; xCur <= xInt + 2; xCur++)
                    {

                        // Calculate the position and distance to the seed point inside of
                        // this unit cube.
                        double xPos = xCur + NoiseGen.ValueNoise3D(xCur, yCur, zCur, m_seed);
                        double yPos = yCur + NoiseGen.ValueNoise3D(xCur, yCur, zCur, m_seed + 1);
                        double zPos = zCur + NoiseGen.ValueNoise3D(xCur, yCur, zCur, m_seed + 2);
                        double xDist = xPos - x;
                        double yDist = yPos - y;
                        double zDist = zPos - z;
                        double dist = xDist * xDist + yDist * yDist + zDist * zDist;

                        if (dist < minDist)
                        {
                            // This seed point is closer to any others found so far, so record
                            // this seed point.
                            minDist = dist;
                            xCandidate = xPos;
                            yCandidate = yPos;
                            zCandidate = zPos;
                        }
                    }
                }
            }

            double value;
            if (m_enableDistance)
            {
                // Determine the distance to the nearest seed point.
                double xDist = xCandidate - x;
                double yDist = yCandidate - y;
                double zDist = zCandidate - z;
                value = (Math.Sqrt(xDist * xDist + yDist * yDist + zDist * zDist)
                  ) * MathConst.SQRT_3 - 1.0;
            }
            else
            {
                value = 0.0;
            }

            // Return the calculated distance with the displacement value applied.
            return value + (m_displacement * (double)NoiseGen.ValueNoise3D(
              (int)(Math.Floor(xCandidate)),
              (int)(Math.Floor(yCandidate)),
              (int)(Math.Floor(zCandidate))));
        }

        /// Sets the displacement value of the Voronoi cells.
        ///
        /// @param displacement The displacement value of the Voronoi cells.
        ///
        /// This noise module assigns each Voronoi cell with a random constant
        /// value from a coherent-noise function.  The <i>displacement
        /// value</i> controls the range of random values to assign to each
        /// cell.  The range of random values is +/- the displacement value.
        public void SetDisplacement(double displacement)
        {
            m_displacement = displacement;
        }

        /// Sets the frequency of the seed points.
        ///
        /// @param frequency The frequency of the seed points.
        ///
        /// The frequency determines the size of the Voronoi cells and the
        /// distance between these cells.
        public void SetFrequency(double frequency)
        {
            m_frequency = frequency;
        }

        /// Sets the seed value used by the Voronoi cells
        ///
        /// @param seed The seed value.
        ///
        /// The positions of the seed values are calculated by a
        /// coherent-noise function.  By modifying the seed value, the output
        /// of that function changes.
        public void SetSeed(int seed)
        {
            m_seed = seed;
        }



        /// Scale of the random displacement to apply to each Voronoi cell.
        protected double m_displacement;

        /// Determines if the distance from the nearest seed point is applied to
        /// the output value.
        protected bool m_enableDistance;

        /// Frequency of the seed points.
        protected double m_frequency;

        /// Seed value used by the coherent-noise function to determine the
        /// positions of the seed points.
        protected int m_seed;
    }
}
