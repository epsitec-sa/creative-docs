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
		
		void InsertTable(SqlTable table);
		void RemoveTable(string table_name);
		
		void InsertTableColumns(string table_name, SqlColumn[] columns);
		void UpdateTableColumns(string table_name, SqlColumn[] columns);
		void RemoveTableColumns(string table_name, SqlColumn[] columns);
		
		//	Création de requêtes standard :
		
		void SelectData(SqlSelect query);
		void InsertData(string table_name, SqlFieldCollection fields);
		void UpdateData(string table_name, SqlFieldCollection fields, SqlFieldCollection conditions);
		void RemoveData(string table_name, SqlFieldCollection conditions);
	}
}
