namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// L'interface IDbAbstraction est utilisée pour accéder aux mécanismes ADO.NET
	/// dépendants d'un provider spécifique.
	/// </summary>
	public interface IDbAbstraction
	{
		IDbAbstractionFactory		Factory			{ get; }
		System.Data.IDbConnection	Connection		{ get; }
		ISqlBuilder					SqlBuilder		{ get; }
		
		System.Data.IDbCommand NewDbCommand();
		System.Data.IDataAdapter NewDataAdapter(System.Data.IDbCommand command);
		
		string[] QueryTableNames();
		SqlTable QueryTableSchema(string table_name);
	}
}
