//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>DbElementCat</c> enumeration defines the categories to which
	/// database elements (such as tables or columns) belong.
	/// </summary>
	public enum DbElementCat : byte
	{
		/// <summary>
		/// Unknown category.
		/// </summary>
		Unknown = 0,
		
		/// <summary>
		/// Internal element; this is used by the database infrastructure and
		/// should never be manipulated directly by the user code.
		/// </summary>
		Internal = 1,
		
		/// <summary>
		/// User data, managed by the Cresus database code.
		/// </summary>
		ManagedUserData = 2,
		
		/// <summary>
		/// External user data, not managed by the Cresus database code.
		/// </summary>
		ExternalUserData = 3,
		
		/// <summary>
		/// Synthetic element; it has no real existence.
		/// </summary>
		Synthetic = 4,
		
		/// <summary>
		/// Any element; this is only valid as an extraction criterion.
		/// </summary>
		Any	= 5,

		/// <summary>
		/// Revision history; used to describe revision history tables and not
		/// intended for public use.
		/// </summary>
		RevisionHistory = 6,
	}
}
