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
