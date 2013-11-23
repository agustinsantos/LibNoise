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
	/// Provides a noise module that maps the output value from a source module onto an
	/// exponential curve. [OPERATOR]
	/// </summary>
	public class Exponent : ModuleBase
	{
		#region Fields

		private double m_exponent = 1.0;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of Exponent.
		/// </summary>
		public Exponent()
			: base(1)
		{
		}

		/// <summary>
		/// Initializes a new instance of Exponent.
		/// </summary>
		/// <param name="exponent">The exponent to use.</param>
		/// <param name="input">The input module.</param>
		public Exponent(double exponent, ModuleBase input)
			: base(1)
		{
			this.m_modules[0] = input;
			this.Value = exponent;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the exponent.
		/// </summary>
		public double Value
		{
			get { return this.m_exponent; }
			set { this.m_exponent = value; }
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
			double v = this.m_modules[0].GetValue(x, y, z);
			return (Math.Pow(Math.Abs((v + 1.0) / 2.0), this.m_exponent) * 2.0 - 1.0);
		}

		#endregion
	}
}