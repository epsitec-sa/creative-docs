//	Copyright � 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// L'interface IDbServiceTools permet d'acc�der � des fonctions de service
	/// de la base de donn�es (pas forc�ment disponibles pour toutes les bases).
	/// </summary>
	public interface IDbServiceTools
	{
		void Backup(string file_name);
	}
}
