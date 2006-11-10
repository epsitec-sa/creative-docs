//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>SqlJoinCode</c> enumeration defines all SQL joins.
	/// </summary>
	public enum SqlJoinCode
	{
		Unknown,
		
		Inner,									//	A.a, B.b -> A INNER JOIN B ON A.a = B.b
		
		OuterLeft,
		OuterRight
	}
}
