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
	/// Provides a noise module that outputs a weighted blend of the output values from
	/// two source modules given the output value supplied by a control module. [OPERATOR]
	/// </summary>
	public class Blend : ModuleBase
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of Blend.
		/// </summary>
		public Blend()
			: base(3)
		{
		}

		/// <summary>
		/// Initializes a new instance of Blend.
		/// </summary>
		/// <param name="lhs">The left hand input module.</param>
		/// <param name="rhs">The right hand input module.</param>
		/// <param name="controller">The controller of the operator.</param>
		public Blend(ModuleBase lhs, ModuleBase rhs, ModuleBase controller)
			: base(3)
		{
			this.m_modules[0] = lhs;
			this.m_modules[1] = rhs;
			this.m_modules[2] = controller;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the controlling module.
		/// </summary>
		public ModuleBase Controller
		{
			get { return this.m_modules[2]; }
			set
			{
				System.Diagnostics.Debug.Assert(value != null);
				this.m_modules[2] = value;
			}
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
			System.Diagnostics.Debug.Assert(this.m_modules[1] != null);
			System.Diagnostics.Debug.Assert(this.m_modules[2] != null);
			double a = this.m_modules[0].GetValue(x, y, z);
			double b = this.m_modules[1].GetValue(x, y, z);
			double c = (this.m_modules[2].GetValue(x, y, z) + 1.0) / 2.0;
			return Utils.InterpolateLinear(a, b, c);
		}

		#endregion
	}
}