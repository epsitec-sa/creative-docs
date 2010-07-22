using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

namespace Epsitec.Cresus.DataLayer.Expressions
{


	/// <summary>
	///  The <c>Constant</c> class represents a constant value that can be used in an
	///  <see cref="Expression"/>.
	/// </summary>
	public sealed class Constant
	{


		/// <summary>
		/// Builds a new <c>Constant</c> of type <see cref="short"/>.
		/// </summary>
		/// <param name="value">The value of the new <c>Constant</c></param>
		public Constant(short value)
			: this (Type.Int16, value)
		{
		}


		/// <summary>
		/// Builds a new <c>Constant</c> of type <see cref="int"/>.
		/// </summary>
		/// <param name="value">The value of the new <c>Constant</c></param>
		public Constant(int value)
			: this (Type.Int32, value)
		{
		}


		/// <summary>
		/// Builds a new <c>Constant</c> of type <see cref="long"/>.
		/// </summary>
		/// <param name="value">The value of the new <c>Constant</c></param>
		public Constant(long value)
			: this (Type.Int64, value)
		{
		}


		/// <summary>
		/// Builds a new <c>Constant</c> of type <see cref="decimal"/>. Note that the decimal will
		/// be converted to an <see cref="double"/>, which might cause a loss of precision.
		/// </summary>
		/// <param name="value">The value of the new <c>Constant</c></param>
		public Constant(decimal value)
			: this (Type.Double, (double) value)
		{
		}


		/// <summary>
		/// Builds a new <c>Constant</c> of type <see cref="float"/>.
		/// </summary>
		/// <param name="value">The value of the new <c>Constant</c></param>
		public Constant(float value)
			: this (Type.Double, value)
		{
		}


		/// <summary>
		/// Builds a new <c>Constant</c> of type <see cref="double"/>.
		/// </summary>
		/// <param name="value">The value of the new <c>Constant</c></param>
		public Constant(double value)
			: this (Type.Double, value)
		{
		}


		/// <summary>
		/// Builds a new <c>Constant</c> of type <see cref="bool"/>.
		/// </summary>
		/// <param name="value">The value of the new <c>Constant</c></param>
		public Constant(bool value)
			: this (Type.Boolean, value)
		{
		}


		/// <summary>
		/// Builds a new <c>Constant</c> of type <see cref="string"/>.
		/// </summary>
		/// <param name="value">The value of the new <c>Constant</c></param>
		/// <exception cref="System.ArgumentNullException">If <paramref name="value"/> is null.</exception>
		public Constant(string value)
			: this (Type.String, value)
		{
			value.ThrowIfNull ("value");
		}


		/// <summary>
		/// Builds a new <c>Constant</c> of type <see cref="Date"/>.
		/// </summary>
		/// <param name="value">The value of the new <c>Constant</c></param>
		public Constant(Date value)
			: this (Type.Date, value)
		{
		}


		/// <summary>
		/// Builds a new <c>Constant</c> of type <see cref="Time"/>.
		/// </summary>
		/// <param name="value">The value of the new <c>Constant</c></param>
		public Constant(Time value)
			: this (Type.Time, value)
		{
		}


		/// <summary>
		/// Builds a new <c>Constant</c> of type <see cref="System.DateTime"/>.
		/// </summary>
		/// <param name="value">The value of the new <c>Constant</c></param>
		public Constant(System.DateTime value)
			: this (Type.DateTime, value)
		{
		}

		
		/// <summary>
		/// Builds a new <c>Constant</c>.
		/// </summary>
		/// <param name="type">The type of the <c>Constant</c>.</param>
		/// <param name="value">The value of the <c>Constant</c>.</param>
		private Constant(Type type, object value)
		{
			this.Type = type;
			this.Value = value;
		}


		/// <summary>
		/// The type of the <c>Constant</c>.
		/// </summary>
		internal Type Type
		{
			get;
			private set;
		}
		
		
		/// <summary>
		/// The value of the <c>Constant</c>.
		/// </summary>
		public object Value
		{
			get;
			private set;
		}


	}


}
