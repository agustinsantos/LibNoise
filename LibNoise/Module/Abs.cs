using System;
using System.Diagnostics;

namespace Noise.Module
{
    /// Noise module that outputs the absolute value of the output value from
    /// a source module.
    ///
    /// @image html moduleabs.png
    ///
    /// This noise module requires one source module.
    public class Abs : Module
    {
        /// Constructor.
        public Abs()
            : base(1)
        {
        }

        public override int GetSourceModuleCount()
        {
            return 1;
        }

        public override double GetValue(double x, double y, double z)
        {
            Debug.Assert(m_pSourceModule[0] != null);

            return Math.Abs(m_pSourceModule[0].GetValue(x, y, z));
        }
    }
}
