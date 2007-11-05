//	Copyright © 2004-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>DbRevisionMode</c> enumeration defines the revision modes
	/// used by database tables.
	/// </summary>
	public enum DbRevisionMode : byte
	{
		/// <summary>
		/// Unknown revision mode.
		/// </summary>
		Unknown = 0,
		
		/// <summary>
		/// Without revision history.
		/// </summary>
		IgnoreChanges = 1,
		
		/// <summary>
		/// With revision history.
		/// </summary>
		TrackChanges = 2,

		/// <summary>
		/// The data is immutable. No revision history is needed.
		/// </summary>
		Immutable = 3,
	}
}
