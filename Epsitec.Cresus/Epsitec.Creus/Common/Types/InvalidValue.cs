//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>InvalidValue</c> class represents an invalid value. Do not confuse
	/// this with an undefined value which is represented by an instance of
	/// class <see cref="UndefinedValue"/>.
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
		[System.Diagnostics.DebuggerStepThrough]
		public static bool IsInvalidValue(object value)
		{
			return (value == InvalidValue.Value);
		}

		/// <summary>
		/// Returns a <see cref="string"/> that represents the invalid value.
		/// </summary>
		/// <returns>A <see cref="string"/> that represents the invalid value.</returns>
		public override string ToString()
		{
			return "<InvalidValue>";
		}
		
		public static readonly InvalidValue Value = new InvalidValue ();
	}
}
