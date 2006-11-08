//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// L'interface IAttachable permet de s'attacher/détacher d'une table d'une
	/// base de données.
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
