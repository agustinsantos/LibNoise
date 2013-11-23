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
	/// Provides a noise module that outputs a constant value. [GENERATOR]
	/// </summary>
	public class Const : ModuleBase
	{
		#region Fields

		private double m_value = 0.0;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of Const.
		/// </summary>
		public Const()
			: base(0)
		{
		}

		/// <summary>
		/// Initializes a new instance of Const.
		/// </summary>
		/// <param name="value">The constant value.</param>
		public Const(double value)
			: base(0)
		{
			this.Value = value;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the constant value.
		/// </summary>
		public double Value
		{
			get { return this.m_value; }
			set { this.m_value = value; }
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
			return this.m_value;
		}

		#endregion
	}
}