//	Copyright © 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>DbCompareCombiner</c> enumeration specifies how comparisons
	/// are combined together (logical and/logical or).
	/// </summary>
	public enum DbCompareCombiner : byte
	{
		/// <summary>
		/// No combination specified.
		/// </summary>
		None,

		/// <summary>
		/// Combine using logical and.
		/// </summary>
		And,
		
		/// <summary>
		/// Combine using logical or.
		/// </summary>
		Or
	}
}
