//	Copyright © 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>IDbServiceTools</c> interface gives access to service functions
	/// available to some database engines (backup, restore, etc.)
	/// </summary>
	public interface IDbServiceTools
	{
		/// <summary>
		/// Backs up the database at the specified path.
		/// </summary>
		/// <param name="path">The path to the database.</param>
		void Backup(string path);

		/// <summary>
		/// Restores the database at the specified path.
		/// </summary>
		/// <param name="path">The path to the database.</param>
		void Restore(string path);

		/// <summary>
		/// Gets the path for the associated database files.
		/// </summary>
		/// <returns>The path to the database files.</returns>
		string GetDatabasePath();
	}
}
