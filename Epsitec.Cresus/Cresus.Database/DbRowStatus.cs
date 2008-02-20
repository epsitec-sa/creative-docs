//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>DbRowStatus</c> enumeration defines the possible row status.
	/// </summary>
	public enum DbRowStatus : short
	{
		/// <summary>
		/// Live row (default).
		/// </summary>
		Live = 0,

		/// <summary>
		/// Copied row, still clean and unmodified since the copy was done.
		/// </summary>
		Copied = 1,
		
		/// <summary>
		/// Archived row.
		/// </summary>
		ArchiveCopy = 2,

		/// <summary>
		/// Deleted row.
		/// </summary>
		Deleted = 3,
	}
}
