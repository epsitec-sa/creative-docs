//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// L'interface IPersistable permet de sérialiser/désérialiser des données à
	/// partir de la base de données (objets persistants).
	/// </summary>
	public interface IPersistable
	{
		/// <summary>
		/// Saves the instance data to the database.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		void PersistToBase(DbTransaction transaction);
		
		/// <summary>
		/// Loads the instance data from the database.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		void LoadFromBase(DbTransaction transaction);
	}
}
