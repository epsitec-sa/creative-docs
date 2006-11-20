//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>SqlJoinCode</c> enumeration defines all SQL joins.
	/// </summary>
	public enum SqlJoinCode
	{
		/// <summary>
		/// Unknown join.
		/// </summary>
		Unknown,
		
		/// <summary>
		/// Inner join; A.a, B.b -&gt; A INNER JOIN B ON A.a = B.b.
		/// </summary>
		Inner,
		
		/// <summary>
		/// Outer left join.
		/// </summary>
		OuterLeft,

		/// <summary>
		/// Outer right join.
		/// </summary>
		OuterRight
	}
}
