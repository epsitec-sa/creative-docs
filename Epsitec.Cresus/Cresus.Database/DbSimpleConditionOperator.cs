//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>DbCompare</c> enumeration defines the comparison operation
	/// which has to be applied to two fields.
	/// </summary>
	public enum DbSimpleConditionOperator : byte
	{
		/// <summary>
		/// Returns <c>true</c> if both fields are equal.
		/// </summary>
		Equal,
		
		/// <summary>
		/// Returns <c>true</c> if both fields are not equal.
		/// </summary>
		NotEqual,

		/// <summary>
		/// Returns <c>true</c> if first field is less than second field.
		/// </summary>
		LessThan,
		
		/// <summary>
		/// Returns <c>true</c> if first field is less than or equal to second field.
		/// </summary>
		LessThanOrEqual,

		/// <summary>
		/// Returns <c>true</c> if first field is greater than second field.
		/// </summary>
		GreaterThan,
		
		/// <summary>
		/// Returns <c>true</c> if first field is less than or equal to second field.
		/// </summary>
		GreaterThanOrEqual,

		/// <summary>
		/// Returns <c>true</c> if both fields are alike (this uses SQL pattern based
		/// string comparison).
		/// </summary>
		Like,

		/// <summary>
		/// Returns <c>true</c> if both fields are not alike (this uses SQL pattern based
		/// string comparison).
		/// </summary>
		NotLike,

		/// <summary>
		/// Returns <c>true</c> if both fields are alike (this uses SQL pattern based
		/// string comparison, including support for special character escaping).
		/// </summary>
		LikeEscape,

		/// <summary>
		/// Returns <c>true</c> if both fields are not alike (this uses SQL pattern based
		/// string comparison, including support for special character escaping).
		/// </summary>
		NotLikeEscape,


		IsNull,
		IsNotNull,
	}
}
