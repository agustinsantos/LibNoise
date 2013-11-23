// 
// Copyright (c) 2013 Agustin Santos
// 
// Permission is hereby granted, free of charge, to any person obtaining a 
// copy of this software and associated documentation files (the "Software"), 
// to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, 
// and/or sell copies of the Software, and to permit persons to whom the 
// Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included 
// in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS 
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibNoise.Generator
{
	/// <summary>
	/// Provides a noise module that outputs concentric cylinders. [GENERATOR]
	/// </summary>
	public class Cylinders : ModuleBase
	{
		#region Fields

		private double m_frequency = 1.0;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of Cylinders.
		/// </summary>
		public Cylinders()
			: base(0)
		{
		}

		/// <summary>
		/// Initializes a new instance of Cylinders.
		/// </summary>
		/// <param name="frequency">The frequency of the concentric cylinders.</param>
		public Cylinders(double frequency)
			: base(0)
		{
			this.Frequency = frequency;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the frequency of the concentric cylinders.
		/// </summary>
		public double Frequency
		{
			get { return this.m_frequency; }
			set { this.m_frequency = value; }
		}

		#endregion

		#region ModuleBase Members

		/// <summary>
		/// Returns the output value for the given input coordinates.
		/// </summary>
		/// <param name="x">The input coordinate on the x-axis.</param>
		/// <param name="y">The input coordinate on the y-axis.</param>
		/// <param name="z">The input coordinate on the z-axis.</param>
		/// <returns>The resulting output value.</returns>
		public override double GetValue(double x, double y, double z)
		{
			x *= this.m_frequency;
			z *= this.m_frequency;
			double dfc = Math.Sqrt(x * x + z * z);
			double dfss = dfc - Math.Floor(dfc);
			double dfls = 1.0 - dfss;
			double nd = Math.Min(dfss, dfls);
			return 1.0 - (nd * 4.0);
		}

		#endregion
	}
}