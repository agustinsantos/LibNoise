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
	/// Provides a noise module that applies a scaling factor and a bias to the output
	/// value from a source module. [OPERATOR]
	/// </summary>
	public class ScaleBias : ModuleBase
	{
		#region Fields

		private double m_scale = 1.0;
		private double m_bias = 0.0;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of ScaleBias.
		/// </summary>
		public ScaleBias()
			: base(1)
		{
		}

		/// <summary>
		/// Initializes a new instance of ScaleBias.
		/// </summary>
		/// <param name="scale">The scaling factor to apply to the output value from the source module.</param>
		/// <param name="bias">The bias to apply to the scaled output value from the source module.</param>
		/// <param name="input">The input module.</param>
		public ScaleBias(double scale, double bias, ModuleBase input)
			: base(1)
		{
			this.m_modules[0] = input;
			this.Bias = bias;
			this.Scale = scale;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the bias to apply to the scaled output value from the source module.
		/// </summary>
		public double Bias
		{
			get { return this.m_bias; }
			set { this.m_bias = value; }
		}

		/// <summary>
		/// Gets or sets the scaling factor to apply to the output value from the source module.
		/// </summary>
		public double Scale
		{
			get { return this.m_scale; }
			set { this.m_scale = value; }
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
			return this.m_modules[0].GetValue(x, y, z) * this.m_scale + this.m_bias;
		}

		#endregion
	}
}