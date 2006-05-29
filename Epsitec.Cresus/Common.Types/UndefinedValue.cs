//	Copyright © 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>UndefinedValue</c> represents the value for a property which has
	/// not been set. Compare with <see cref="T:InvalidValue"/>.
	/// </summary>
	public sealed class UndefinedValue
	{
		private UndefinedValue()
		{
		}


		/// <summary>
		/// Determines whether the object maps to an undefined value.
		/// </summary>
		/// <param name="value">The object which should be tested.</param>
		/// <returns><c>true</c> if the object is an undefined value; otherwise,
		/// <c>false</c>.</returns>
		public static bool IsValueUndefined(object value)
		{
			return (value == UndefinedValue.Instance);
		}

		/// <summary>
		/// Returns a <see cref="T:string"/> that represents the undefined value.
		/// </summary>
		/// <returns>A <see cref="T:string"/> that represents the undefined value.</returns>
		public override string ToString()
		{
			return "<UndefinedValue>";
		}
		
		public static readonly UndefinedValue	Instance = new UndefinedValue();
	}
}
