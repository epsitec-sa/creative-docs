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
		void SerializeToBase(DbTransaction transaction);
		void RestoreFromBase(DbTransaction transaction);
	}
}
