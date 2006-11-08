//	Copyright © 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
		Disabled = 1,
		
		/// <summary>
		/// With revision history.
		/// </summary>
		Enabled = 2,
	}
}
