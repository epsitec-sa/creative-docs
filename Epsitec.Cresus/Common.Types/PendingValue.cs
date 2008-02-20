//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>PendingValue</c> represents the value for an asynchronous binding
	/// which has not yet returned a result.
	/// </summary>
	public sealed class PendingValue
	{
		private PendingValue()
		{
		}


		/// <summary>
		/// Determines whether the object maps to a pending value.
		/// </summary>
		/// <param name="value">The object which should be tested.</param>
		/// <returns><c>true</c> if the object is a pending value; otherwise,
		/// <c>false</c>.</returns>
		public static bool IsPendingValue(object value)
		{
			return (value == PendingValue.Value);
		}

		/// <summary>
		/// Returns a <see cref="string"/> that represents the pending value.
		/// </summary>
		/// <returns>A <see cref="string"/> that represents the pending value.</returns>
		public override string ToString()
		{
			return "<PendingValue>";
		}

		public static readonly PendingValue Value = new PendingValue ();
	}
}
