//	Copyright � 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// L'interface IPersistable permet de s�rialiser/d�s�rialiser des donn�es �
	/// partir de la base de donn�es (objets persistants).
	/// </summary>
	public interface IPersistable
	{
		void Attach(DbInfrastructure infrastructure, DbTable table);
		void Detach();
		void SerializeToBase(DbTransaction transaction);
		void RestoreFromBase(DbTransaction transaction);
	}
}
