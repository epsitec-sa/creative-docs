//	Copyright � 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 07/10/2003

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// L'interface IDbAbstractionFactory est utilis�e pour cr�es des instances
	/// du gestionnaire de donn�es sp�cifique.
	/// </summary>
	public interface IDbAbstractionFactory
	{
		string						ProviderName	{ get; }
		ITypeConverter				TypeConverter	{ get; }
		
		IDbAbstraction NewDbAbstraction(DbAccess db_access);
	}
}
