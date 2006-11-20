//	Copyright © 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>IAttachable</c> interface can be used to attach and detach an
	/// object to a database table.
	/// </summary>
	public interface IAttachable
	{
		/// <summary>
		/// Attaches this instance to the specified database table.
		/// </summary>
		/// <param name="infrastructure">The infrastructure.</param>
		/// <param name="table">The database table.</param>
		void Attach(DbInfrastructure infrastructure, DbTable table);
		
		/// <summary>
		/// Detaches this instance from the database.
		/// </summary>
		void Detach();
	}
}
