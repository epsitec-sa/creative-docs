//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 01/12/2003

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// La classe DbRichCommand.
	/// </summary>
	public class DbRichCommand
	{
		public DbRichCommand(ISqlBuilder sql_builder, System.Data.IDbTransaction transaction)
		{
			this.command_object = sql_builder.Command;
			this.command_count  = sql_builder.CommandCount;
			this.command_object.Transaction = transaction;
			
			this.tables = new DbTableCollection ();
		}
		
		public void FillDataSet(DbAccess db_access, System.Data.IDataAdapter adapter)
		{
			//	Définit et remplit le DataSet en se basant sur les données fournies
			//	par l'objet 'adapter' (ADO.NET).
			
			this.data_set = new System.Data.DataSet ();
			this.data_adapter = adapter;
			
			for (int i = 0; i < this.tables.Count; i++)
			{
				DbTable db_table = this.tables[i];
				
				string  ado_name_table = (i == 0) ? "Table" : string.Format (System.Globalization.CultureInfo.InvariantCulture, "Table{0}", i);
				string  db_name_table  = db_table.Name;
				
				System.Data.ITableMapping mapping = adapter.TableMappings.Add (ado_name_table, db_name_table);
				
				for (int c = 0; c < db_table.Columns.Count; c++)
				{
					DbColumn db_column = db_table.Columns[c];
					
					string db_name_column  = db_column.Name;
					string ado_name_column = db_column.CreateSqlName ();
					
					mapping.ColumnMappings.Add (ado_name_column, db_name_column);
				}
			}
			
			adapter.Fill (this.data_set);
		}
		
		public void CreateEmptyDataSet(ITypeConverter type_converter)
		{
			//	Crée un DataSet vide, en ajoutant le schéma correspondant aux tables
			//	définies.
			
			this.data_set = new System.Data.DataSet ();
			
			for (int i = 0; i < this.tables.Count; i++)
			{
				DbTable db_table = this.tables[i];
				
				System.Data.DataTable ado_table = this.data_set.Tables.Add (db_table.Name);
				
				for (int c = 0; c < db_table.Columns.Count; c++)
				{
					DbColumn db_column = db_table.Columns[c];
					System.Type native_type = TypeConverter.MapToNativeType (db_column.CreateSqlColumn (type_converter).Type);
					
					ado_table.Columns.Add (db_column.Name, native_type);
				}
			}
		}
		
		public void UpdateTables()
		{
			if (this.data_adapter != null)
			{
				this.data_adapter.Update (this.data_set);
			}
		}
		
		public System.Data.IDbCommand		Command
		{
			get { return this.command_object; }
		}
		
		public int							CommandCount
		{
			get { return this.command_count; }
		}
		
		public DbTableCollection			Tables
		{
			get { return this.tables; }
		}
		
		public System.Data.DataSet			DataSet
		{
			get { return this.data_set; }
		}
		
		
		protected DbCommandCollection		commands;
		protected DbTableCollection			tables;
		protected System.Data.DataSet		data_set;
		protected System.Data.IDataAdapter	data_adapter;
		
		protected System.Data.IDbCommand	command_object;
		protected int						command_count;
	}
}
