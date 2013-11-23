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


namespace LibNoise
{
	#region Enumerations

	/// <summary>
	/// Defines a collection of quality modes.
	/// </summary>
	public enum QualityMode
	{
		Low,
		Medium,
		High,
	}

	#endregion

	/// <summary>
	/// Base class for noise modules.
	/// </summary>
	public abstract class ModuleBase : IDisposable
	{
		#region Fields

		protected ModuleBase[] m_modules = null;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of Helpers.
		/// </summary>
		/// <param name="count">The number of source modules.</param>
		protected ModuleBase(int count)
		{
			if (count > 0)
			{
				this.m_modules = new ModuleBase[count];
			}
		}

		#endregion

		#region Indexers

		/// <summary>
		/// Gets or sets a source module by index.
		/// </summary>
		/// <param name="index">The index of the source module to aquire.</param>
		/// <returns>The requested source module.</returns>
		public virtual ModuleBase this[int index]
		{
			get
			{
				System.Diagnostics.Debug.Assert(this.m_modules != null);
				System.Diagnostics.Debug.Assert(this.m_modules.Length > 0);
				if (index < 0 || index >= this.m_modules.Length)
				{
					throw new ArgumentOutOfRangeException();
				}
				if (this.m_modules[index] == null)
				{
					throw new ArgumentNullException();
				}
				return this.m_modules[index];
			}
			set
			{
				System.Diagnostics.Debug.Assert(this.m_modules.Length > 0);
				if (index < 0 || index >= this.m_modules.Length)
				{
					throw new ArgumentOutOfRangeException();
				}
				if (value == null)
				{
					throw new ArgumentNullException();
				}
				this.m_modules[index] = value;
			}
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the number of source modules required by this noise module.
		/// </summary>
		public int SourceModuleCount
		{
			get { return (this.m_modules == null) ? 0 : this.m_modules.Length; }
		}

		#endregion

		#region Methods

		/// <summary>
		/// Returns the output value for the given input coordinates.
		/// </summary>
		/// <param name="x">The input coordinate on the x-axis.</param>
		/// <param name="y">The input coordinate on the y-axis.</param>
		/// <param name="z">The input coordinate on the z-axis.</param>
		/// <returns>The resulting output value.</returns>
		public abstract double GetValue(double x, double y, double z);

		#endregion

		#region IDisposable Members

		[System.Xml.Serialization.XmlIgnore]
#if !XBOX360 && !ZUNE
		[NonSerialized]
#endif
		private bool m_disposed = false;

		/// <summary>
		/// Gets a value whether the object is disposed.
		/// </summary>
		public bool IsDisposed
		{
			get { return this.m_disposed; }
		}

		/// <summary>
		/// Immediately releases the unmanaged resources used by this object.
		/// </summary>
		public void Dispose()
		{
			if (!this.m_disposed) { this.m_disposed = this.Disposing(); }
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Immediately releases the unmanaged resources used by this object.
		/// </summary>
		/// <returns>True if the object is completely disposed.</returns>
		protected virtual bool Disposing()
		{
			if (this.m_modules != null)
			{
				for (int i = 0; i < this.m_modules.Length; i++)
				{
					this.m_modules[i].Dispose();
					this.m_modules[i] = null;
				}
				this.m_modules = null;
			}
			return true;
		}

		#endregion
	}
}