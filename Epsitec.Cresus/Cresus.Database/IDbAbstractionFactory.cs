//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 07/10/2003

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// L'interface IDbAbstractionFactory est utilisée pour crées des instances
	/// du gestionnaire de données spécifique.
	/// </summary>
	public interface IDbAbstractionFactory
	{
		string						ProviderName	{ get; }
		ITypeConverter				TypeConverter	{ get; }
		
		IDbAbstraction NewDbAbstraction(DbAccess db_access);
	}
}
