//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
