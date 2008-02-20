//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>IDataConstraint</c> interface can be used to verify that a
	/// constraint is satisfied.
	/// </summary>
	public interface IDataConstraint
	{
		/// <summary>
		/// Determines whether the specified value is valid according to the
		/// constraint.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// 	<c>true</c> if the specified value is valid; otherwise, <c>false</c>.
		/// </returns>
		bool IsValidValue(object value);
	}
}
