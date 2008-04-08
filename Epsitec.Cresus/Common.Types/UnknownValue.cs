//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>UnknownValue</c> represents the item for a property which does
	/// not exist. Compare with <see cref="UndefinedValue"/>.
	/// </summary>
	public sealed class UnknownValue
	{
		private UnknownValue()
		{
		}


		/// <summary>
		/// Determines whether the object maps to an unknown item.
		/// </summary>
		/// <param name="value">The object which should be tested.</param>
		/// <returns><c>true</c> if the object is an unknown item; otherwise,
		/// <c>false</c>.</returns>
		[System.Diagnostics.DebuggerStepThrough]
		public static bool IsUnknownValue(object value)
		{
			return (value == UnknownValue.Value);
		}

		/// <summary>
		/// Returns a <see cref="string"/> that represents the unknown item.
		/// </summary>
		/// <returns>A <see cref="string"/> that represents the unknown item.</returns>
		public override string ToString()
		{
			return "<UnknownValue>";
		}

		public static readonly UnknownValue Value = new UnknownValue ();
	}
}
