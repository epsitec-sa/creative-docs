//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// L'interface ISqlBuilder est utilisée pour construire des commandes SQL
	/// à partir d'une description plus abstraite.
	/// </summary>
	public interface ISqlBuilder : ISqlValidator, System.IDisposable
	{
		ISqlBuilder NewSqlBuilder();
		
		bool					AutoClear		{ get; set; }
		DbCommandType			CommandType		{ get; }
		System.Data.IDbCommand	Command			{ get; }
		int						CommandCount	{ get; }
		System.Data.IDbCommand	CreateCommand(System.Data.IDbTransaction transaction);
		System.Data.IDbCommand  CreateCommand(System.Data.IDbTransaction transaction, string text);
		void Clear();
		void AppendMore();
		void InsertTable(SqlTable table);
		void RemoveTable(string table_name);
		void InsertTableColumns(string table_name, SqlColumn[] columns);
		void UpdateTableColumns(string table_name, SqlColumn[] columns);
		void RemoveTableColumns(string table_name, SqlColumn[] columns);
		void SelectData(SqlSelect query);
		void InsertData(string table_name, Collections.SqlFields fields);
		void UpdateData(string table_name, Collections.SqlFields fields, Collections.SqlFields conditions);
		void RemoveData(string table_name, Collections.SqlFields conditions);
		void ExecuteProcedure(string procedure_name, Collections.SqlFields fields);
		void SetSqlParameters(System.Data.IDbCommand command, Collections.SqlFields fields);
		void GetSqlParameters(System.Data.IDbCommand command, Collections.SqlFields fields);
		void SetCommandParameterValue(System.Data.IDbCommand command, int index, object value);
		void GetCommandParameterValue(System.Data.IDbCommand command, int index, out object value);
		//	Manipulation du buffer interne, permettant de représenter une ou
		//	plusieurs commandes SQL. Le buffer interne est vraisemblablement
		//	implémenté au moyen d'une instance de System.Text.StringBuilder.
		//
		//	Si AutoClear = true, l'insertion d'une commande écrase automatique-
		//	ment la précédente. Dans le cas contraire, il faut appeler Clear
		//	explicitement, ou une exception est générée.
		
		
		
		
		//	Manipulation de tables :
		
		
		
		//	Création de requêtes standard :
		
		
		//	Création d'une requête d'exécution de procédure SQL :
		
		
		//	Manipule les paramètres d'un appel de procédure SQL :
		
		
		//	Divers :
		
	}
}
