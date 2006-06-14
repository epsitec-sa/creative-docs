//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>InvalidValue</c> class represents an invalid value. Do not confuse
	/// this with an undefined value which is represented by an instance of
	/// class <see cref="T:UndefinedValue"/>.
	/// </summary>
	public sealed class InvalidValue
	{
		private InvalidValue()
		{
		}

		/// <summary>
		/// Determines whether the object is the an invalid value.
		/// </summary>
		/// <param name="value">The object which should be tested.</param>
		/// <returns><c>true</c> if the object is an invalid value; otherwise,
		/// <c>false</c>.</returns>
		public static bool IsInvalidValue(object value)
		{
			return (value == InvalidValue.Instance);
		}

		/// <summary>
		/// Returns a <see cref="T:string"/> that represents the invalid value.
		/// </summary>
		/// <returns>A <see cref="T:string"/> that represents the invalid value.</returns>
		public override string ToString()
		{
			return "<InvalidValue>";
		}
		
		public static readonly InvalidValue Instance = new InvalidValue ();
	}
}
