//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 27/11/2003

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// L'interface ISqlBuilder est utilisée pour construire des commandes SQL
	/// à partir d'une description plus abstraite.
	/// </summary>
	public interface ISqlBuilder : ISqlValidator
	{
		//	Manipulation du buffer interne, permettant de représenter une ou
		//	plusieurs commandes SQL. Le buffer interne est vraisemblablement
		//	implémenté au moyen d'une instance de System.Text.StringBuilder.
		//
		//	Si AutoClear = true, l'insertion d'une commande écrase automatique-
		//	ment la précédente. Dans le cas contraire, il faut appeler Clear
		//	explicitement, ou une exception est générée.
		
		bool					AutoClear		{ get; set; }
		DbCommandType			CommandType		{ get; }
		System.Data.IDbCommand	Command			{ get; }
		int						CommandCount	{ get; }
		
		System.Data.IDbCommand	CreateCommand(System.Data.IDbTransaction transaction);
		System.Data.IDbCommand  CreateCommand(System.Data.IDbTransaction transaction, string text);
		
		void Clear();
		void AppendMore();
		
		//	Manipulation de tables :
		
		void InsertTable(SqlTable table);
		void RemoveTable(string table_name);
		
		void InsertTableColumns(string table_name, SqlColumn[] columns);
		void UpdateTableColumns(string table_name, SqlColumn[] columns);
		void RemoveTableColumns(string table_name, SqlColumn[] columns);
		
		//	Création de requêtes standard :
		
		void SelectData(SqlSelect query);
		void InsertData(string table_name, Collections.SqlFields fields);
		void UpdateData(string table_name, Collections.SqlFields fields, Collections.SqlFields conditions);
		void RemoveData(string table_name, Collections.SqlFields conditions);
		
		//	Création d'une requête d'exécution de procédure SQL :
		
		void ExecuteProcedure(string procedure_name, Collections.SqlFields fields);
		
		//	Manipule les paramètres d'un appel de procédure SQL :
		
		void SetSqlParameters(System.Data.IDbCommand command, Collections.SqlFields fields);
		void GetSqlParameters(System.Data.IDbCommand command, Collections.SqlFields fields);
	}
}
