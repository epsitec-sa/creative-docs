//	Copyright � 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// L'interface ISqlBuilder est utilis�e pour construire des commandes SQL
	/// � partir d'une description plus abstraite.
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
		//	Manipulation du buffer interne, permettant de repr�senter une ou
		//	plusieurs commandes SQL. Le buffer interne est vraisemblablement
		//	impl�ment� au moyen d'une instance de System.Text.StringBuilder.
		//
		//	Si AutoClear = true, l'insertion d'une commande �crase automatique-
		//	ment la pr�c�dente. Dans le cas contraire, il faut appeler Clear
		//	explicitement, ou une exception est g�n�r�e.
		
		
		
		
		//	Manipulation de tables :
		
		
		
		//	Cr�ation de requ�tes standard :
		
		
		//	Cr�ation d'une requ�te d'ex�cution de proc�dure SQL :
		
		
		//	Manipule les param�tres d'un appel de proc�dure SQL :
		
		
		//	Divers :
		
	}
}
