namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// L'interface ISqlBuilder est utilis�e pour construire des commandes SQL
	/// � partir d'une description plus abstraite.
	/// </summary>
	public interface ISqlBuilder
	{
		//	Manipulation du buffer interne, permettant de repr�senter une ou
		//	plusieurs commandes SQL. Le buffer interne est vraisemblablement
		//	impl�ment� au moyen d'une instance de System.Text.StringBuilder.
		//
		//	Si AutoClear = true, la lecture de la propri�t� Command fera aussi
		//	automatiquement un Clear.
		
		bool					AutoClear		{ get; set; }
		System.Data.IDbCommand	Command			{ get; }
		
		void Clear();
		
		//	V�rification de la validit� de noms et de valeurs SQL :
		
		bool ValidateName(string value);
		bool ValidateString(string value);
		bool ValidateNumber(string value);
		
		//	Manipulation de tables :
		
		void InsertTable(SqlTable table);
		void RemoveTable(string table_name);
		
		void InsertTableColumns(string table_name, SqlColumn[] columns);
		void UpdateTableColumns(string table_name, SqlColumn[] columns);
		void RemoveTableColumns(string table_name, SqlColumn[] columns);
		
		//	Cr�ation de requ�tes standard :
		
		void SelectData(SqlSelect query);
		void InsertData(string table_name, SqlFieldCollection fields);
		void UpdateData(string table_name, SqlFieldCollection fields, SqlFieldCollection conditions);
		void RemoveData(string table_name, SqlFieldCollection conditions);
		
		//	Cr�ation d'une requ�te d'ex�cution de proc�dure SQL :
		
		void ExecuteProcedure(string procedure_name, SqlFieldCollection fields);
		
		//	Manipule les param�tres d'un appel de proc�dure SQL :
		
		void SetSqlParameters(System.Data.IDbCommand command, SqlFieldCollection fields);
		void GetSqlParameters(System.Data.IDbCommand command, SqlFieldCollection fields);
	}
}
