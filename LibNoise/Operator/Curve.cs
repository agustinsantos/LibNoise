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

using System.Collections.Generic;


namespace LibNoise.Operator
{
	/// <summary>
	/// Provides a noise module that maps the output value from a source module onto an
	/// arbitrary function curve. [OPERATOR]
	/// </summary>
	public class Curve : ModuleBase
	{
		#region Fields

		private List<KeyValuePair<double, double>> m_data = new List<KeyValuePair<double,double>>();

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of Curve.
		/// </summary>
		public Curve()
			: base(1)
		{
		}

		/// <summary>
		/// Initializes a new instance of Curve.
		/// </summary>
		/// <param name="input">The input module.</param>
		public Curve(ModuleBase input)
			: base(1)
		{
			this.m_modules[0] = input;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the number of control points.
		/// </summary>
		public int ControlPointCount
		{
			get { return this.m_data.Count; }
		}

		/// <summary>
		/// Gets the list of control points.
		/// </summary>
		public List<KeyValuePair<double, double>> ControlPoints
		{
			get { return this.m_data; }
		}

		#endregion

		#region Methods

		/// <summary>
		/// Adds a control point to the curve.
		/// </summary>
		/// <param name="input">The curves input value.</param>
		/// <param name="output">The curves output value.</param>
		public void Add(double input, double output)
		{
			KeyValuePair<double, double> kvp = new KeyValuePair<double, double>(input, output);
			if (!this.m_data.Contains(kvp))
			{
				this.m_data.Add(kvp);
			}
			this.m_data.Sort(delegate(KeyValuePair<double, double> lhs, KeyValuePair<double, double> rhs)
				{ return lhs.Key.CompareTo(rhs.Key); });
		}

		/// <summary>
		/// Clears the control points.
		/// </summary>
		public void Clear()
		{
			this.m_data.Clear();
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
			System.Diagnostics.Debug.Assert(this.ControlPointCount >= 4);
			double smv = this.m_modules[0].GetValue(x, y, z);
			int ip;
			for (ip = 0; ip < this.m_data.Count; ip++)
			{
				if (smv < this.m_data[ip].Key)
				{
					break;
				}
			}
			int i0 = (int)MathHelper.Clamp(ip - 2, 0, this.m_data.Count - 1);
			int i1 = (int)MathHelper.Clamp(ip - 1, 0, this.m_data.Count - 1);
			int i2 = (int)MathHelper.Clamp(ip, 0, this.m_data.Count - 1);
			int i3 = (int)MathHelper.Clamp(ip + 1, 0, this.m_data.Count - 1);
			if (i1 == i2)
			{
				return this.m_data[i1].Value;
			}
			double ip0 = this.m_data[i1].Key;
			double ip1 = this.m_data[i2].Key;
			double a = (smv - ip0) / (ip1 - ip0);
			return Utils.InterpolateCubic(this.m_data[i0].Value, this.m_data[i1].Value, this.m_data[i2].Value, this.m_data[i3].Value, a);
		}

		#endregion
	}
}