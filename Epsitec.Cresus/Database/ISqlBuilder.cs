namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// L'interface ISqlBuilder est utilisée pour construire des commandes SQL
	/// à partir d'une description plus abstraite.
	/// </summary>
	public interface ISqlBuilder
	{
		//	Manipulation du buffer interne, permettant de représenter une ou
		//	plusieurs commandes SQL. Le buffer interne est vraisemblablement
		//	implémenté au moyen d'une instance de System.Text.StringBuilder.
		//
		//	Si AutoClear = true, la lecture de la propriété CommandString fera
		//	aussi automatiquement un Clear.
		
		bool			AutoClear				{ get; set; }
		string			CommandString			{ get; }
		
		void Clear();
		
		//	Vérification de la validité de noms et de valeurs SQL :
		
		bool ValidateName(string name);
		bool ValidateString(string name);
		bool ValidateNumber(string name);
		
		//	Manipulation de tables :
		
		void AddTable(DbTable table);
		void RemoveTable(string table_name);
		
		void AddTableColumns(string table_name, DbColumn[] columns);
		void UpdateTableColumns(string table_name, DbColumn[] columns);
		void RemoveTableColumns(string table_name, DbColumn[] columns);
		
		//	Création de requêtes standard :
		
		void SelectData(SqlSelect query);
		void InsertData(SqlTable table, SqlFieldCollection fields);
		void UpdateData(SqlTable table, SqlFieldCollection fields);
	}
}
