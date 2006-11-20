//	Copyright © 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>DbReplicationMode</c> enumeration defines the replication
	/// modes supported by the database.
	/// </summary>
	public enum DbReplicationMode : byte
	{
		/// <summary>
		/// Unknown replication mode.
		/// </summary>
		Unknown = 0,
		
		/// <summary>
		/// Private information without any replication.
		/// </summary>
		None = 1,
		
		/// <summary>
		/// Shared information with automatic replication.
		/// </summary>
		Automatic = 2,
		
		/// <summary>
		/// Shared information with manual repliaction.
		/// </summary>
		Manual = 3,
	}
}
