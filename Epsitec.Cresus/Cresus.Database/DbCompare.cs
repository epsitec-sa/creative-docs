//	Copyright © 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>DbCompare</c> enumeration defines the comparison operation
	/// which has to be applied to two fields.
	/// </summary>
	public enum DbCompare : byte
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
		/// Returns <c>true</c> if both fields are alike (this uses SQL pattern based string comparison).
		/// </summary>
		Like,
		
		/// <summary>
		/// Returns <c>true</c> if both fields are not alike (this uses SQL pattern based string comparison).
		/// </summary>
		NotLike
	}
}
