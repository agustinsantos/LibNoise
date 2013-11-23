using System.Diagnostics;

namespace Noise.Module
{
    /// Noise module that outputs the sum of the two output values from two
    /// source modules.
    ///
    /// @image html moduleadd.png
    ///
    /// This noise module requires two source modules.
    public class Add : Module
    {
        /// Constructor.
        public Add()
            : base(2)
        {
        }

        public override int GetSourceModuleCount()
        {
            return 2;
        }

        public override double GetValue(double x, double y, double z)
        {
            Debug.Assert(m_pSourceModule[0] != null);
            Debug.Assert(m_pSourceModule[1] != null);

            return m_pSourceModule[0].GetValue(x, y, z)
                 + m_pSourceModule[1].GetValue(x, y, z);
        }
    }
}
