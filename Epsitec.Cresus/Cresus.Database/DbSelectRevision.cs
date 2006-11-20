//	Copyright © 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>DbSelectRevision</c> enumeration defines which revisions of a
	/// row should be selected.
	/// </summary>
	public enum DbSelectRevision
	{
		/// <summary>
		/// Select all revisions.
		/// </summary>
		All,
		
		/// <summary>
		/// Select only active revisions (this maps to <c>DbRowStatus.Live</c> and <c>DbRowStatus.Copied</c>)
		/// </summary>
		LiveActive,

		/// <summary>
		/// Select only live revisions (this maps to <c>DbRowStatus.Live</c>, <c>DbRowStatus.Copied</c> and
		/// <c>DbRowStatus.ArchiveCopy</c>)
		/// </summary>
		LiveAll,
	}
}
