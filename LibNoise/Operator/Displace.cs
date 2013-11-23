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
	/// Provides a noise module that uses three source modules to displace each
	/// coordinate of the input value before returning the output value from
	/// a source module. [OPERATOR]
	/// </summary>
	public class Displace : ModuleBase
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of Displace.
		/// </summary>
		public Displace()
			: base(4)
		{
		}

		/// <summary>
		/// Initializes a new instance of Displace.
		/// </summary>
		/// <param name="input">The input module.</param>
		/// <param name="x">The displacement module of the x-axis.</param>
		/// <param name="y">The displacement module of the y-axis.</param>
		/// <param name="z">The displacement module of the z-axis.</param>
		public Displace(ModuleBase input, ModuleBase x, ModuleBase y, ModuleBase z)
			: base(4)
		{
			this.m_modules[0] = input;
			this.m_modules[1] = x;
			this.m_modules[2] = y;
			this.m_modules[3] = z;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the controlling module on the x-axis.
		/// </summary>
		public ModuleBase X
		{
			get { return this.m_modules[1]; }
			set
			{
				System.Diagnostics.Debug.Assert(value != null);
				this.m_modules[1] = value;
			}
		}

		/// <summary>
		/// Gets or sets the controlling module on the z-axis.
		/// </summary>
		public ModuleBase Y
		{
			get { return this.m_modules[2]; }
			set
			{
				System.Diagnostics.Debug.Assert(value != null);
				this.m_modules[2] = value;
			}
		}

		/// <summary>
		/// Gets or sets the controlling module on the z-axis.
		/// </summary>
		public ModuleBase Z
		{
			get { return this.m_modules[3]; }
			set
			{
				System.Diagnostics.Debug.Assert(value != null);
				this.m_modules[3] = value;
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
			System.Diagnostics.Debug.Assert(this.m_modules[3] != null);
			double dx = x + this.m_modules[1].GetValue(x, y, z);
			double dy = y + this.m_modules[1].GetValue(x, y, z);
			double dz = z + this.m_modules[1].GetValue(x, y, z);
			return this.m_modules[0].GetValue(dx, dy, dz);
		}

		#endregion
	}
}