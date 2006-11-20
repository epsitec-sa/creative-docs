//	Copyright © 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>DbRowSearchMode</c> bitset defines what rows should be returned
	/// when a <c>SELECT</c> is applied to a data table. See also <see cref="DbRowStatus"/>.
	/// </summary>
	[System.Flags]
	public enum DbRowSearchMode
	{
		/// <summary>
		/// Returns all rows.
		/// </summary>
		All				= Live | Copied | ArchiveCopy | Deleted,
		
		/// <summary>
		/// Returns only live rows.
		/// </summary>
		Live			= (1 << DbRowStatus.Live),

		/// <summary>
		/// Returns only copied rows.
		/// </summary>
		Copied			= (1 << DbRowStatus.Copied),

		/// <summary>
		/// Returns only archive rows.
		/// </summary>
		ArchiveCopy		= (1 << DbRowStatus.ArchiveCopy),
		
		/// <summary>
		/// Returns only deleted rows.
		/// </summary>
		Deleted			= (1 << DbRowStatus.Deleted),
		
		/// <summary>
		/// Returns all live and copied rows.
		/// </summary>
		LiveActive		= Live | Copied,
		
		/// <summary>
		/// Returns all rows, excluding the deleted ones.
		/// </summary>
		LiveAll			= Live | Copied | ArchiveCopy,
	}
}
