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

namespace LibNoise.Operator
{
	/// <summary>
	/// Provides a noise module that clamps the output value from a source module to a
	/// range of values. [OPERATOR]
	/// </summary>
	public class Clamp : ModuleBase
	{
		#region Fields

		private double m_min = -1.0;
		private double m_max = 1.0;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of Clamp.
		/// </summary>
		public Clamp()
			: base(1)
		{
		}

		/// <summary>
		/// Initializes a new instance of Clamp.
		/// </summary>
		/// <param name="input">The input module.</param>
		/// <param name="min">The minimum value.</param>
		/// <param name="max">The maximum value.</param>
		public Clamp(double min, double max, ModuleBase input)
			: base(1)
		{
			this.Minimum = min;
			this.Maximum = max;
			this.m_modules[0] = input;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the maximum to clamp to.
		/// </summary>
		public double Maximum
		{
			get { return this.m_max; }
			set { this.m_max = value; }
		}

		/// <summary>
		/// Gets or sets the minimum to clamp to.
		/// </summary>
		public double Minimum
		{
			get { return this.m_min; }
			set { this.m_min = value; }
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
			System.Diagnostics.Debug.Assert(this.m_modules[0] != null);
			if (this.m_min > this.m_max)
			{
				double t = this.m_min;
				this.m_min = this.m_max;
				this.m_max = t;
			}
			double v = this.m_modules[0].GetValue(x, y, z);
			if (v < this.m_min) { return this.m_min; }
			else if (v > this.m_max) { return this.m_max; }
			return v;
		}

		#endregion
	}
}