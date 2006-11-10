//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>SqlSelectSetOp</c> enumeration defines the SELECT set operations.
	/// </summary>
	public enum SqlSelectSetOp
	{
		/// <summary>
		/// No combination for a single SELECT command.
		/// </summary>
		None,
		
		/// <summary>
		/// SELECT ... UNION [ALL|DISTINCT] SELECT ...
		/// </summary>
		Union,
		
		/// <summary>
		/// SELECT ... EXCEPT [ALL|DISTINCT] SELECT ...
		/// </summary>
		Except,
		
		/// <summary>
		/// SELECT ... INTERSECT [ALL|DISTINCT] SELECT ...
		/// </summary>
		Intersect,
	}
}
