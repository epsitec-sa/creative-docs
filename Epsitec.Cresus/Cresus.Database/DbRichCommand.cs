//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// La classe DbRichCommand permet de regrouper plusieurs commandes pour
	/// n'en faire qu'une qui peut ensuite être exécutée au moyen de ISqlEngine,
	/// avec récupération des données dans les tables ad hoc.
	/// </summary>
	public class DbRichCommand
	{
		public DbRichCommand()
		{
			this.commands = new Collections.DbCommands ();
			this.tables   = new Collections.DbTables ();
		}
		
		
		public Collections.DbCommands			Commands
		{
			get
			{
				return this.commands;
			}
		}
		
		public Collections.DbTables				Tables
		{
			get
			{
				return this.tables;
			}
		}
		
		public System.Data.IDbTransaction		Transaction
		{
			get
			{
				return this.transaction;
			}
			set
			{
				if (this.transaction != value)
				{
					this.transaction = value;
					this.SetCommandTransaction ();
				}
			}
		}
		
		public System.Data.DataSet				DataSet
		{
			get
			{
				return this.data_set;
			}
		}
		
		
		public void FillDataSet(System.Data.IDbDataAdapter[] adapters)
		{
			//	Définit et remplit le DataSet en se basant sur les données fournies
			//	par l'objet 'adapter' (ADO.NET).
			
			this.data_set = new System.Data.DataSet ();
			this.adapters = adapters;
			
			this.SetCommandTransaction ();
			
			for (int i = 0; i < this.tables.Count; i++)
			{
				DbTable db_table = this.tables[i];
				
				string  ado_name_table = "Table";
				string  db_name_table  = db_table.Name;
				
				//	Il faut (re)nommer les tables afin d'avoir les noms qui correspondent
				//	à ce que définit DbTable, et faire pareil pour les colonnes.
				
				System.Data.ITableMapping mapping = this.adapters[i].TableMappings.Add (ado_name_table, db_name_table);
				
				for (int c = 0; c < db_table.Columns.Count; c++)
				{
					DbColumn db_column = db_table.Columns[c];
					
					string db_name_column  = db_column.CreateDisplayName ();
					string ado_name_column = db_column.CreateSqlName ();
					
					mapping.ColumnMappings.Add (ado_name_column, db_name_column);
				}
				
				this.adapters[i].Fill (this.data_set);
			}
			
			this.CreateDataRelations ();
		}
		
		public void UpdateTables()
		{
			if (this.adapters != null)
			{
				for (int i = 0; i < this.adapters.Length; i++)
				{
					this.adapters[i].Update (this.data_set);
				}
			}
		}
		
		
		public void CreateEmptyDataSet(DbInfrastructure infrastructure)
		{
			if (this.data_set != null)
			{
				throw new System.InvalidOperationException ("DataSet already exists.");
			}
			
			ITypeConverter type_converter = infrastructure.TypeConverter;
			
			//	Crée un DataSet vide, en ajoutant le schéma correspondant aux tables
			//	définies.
			
			this.data_set = new System.Data.DataSet ();
			
			for (int i = 0; i < this.tables.Count; i++)
			{
				DbTable db_table      = this.tables[i];
				string  db_name_table = db_table.Name;
				
				System.Data.DataTable ado_table = this.data_set.Tables.Add (db_name_table);
				
				for (int c = 0; c < db_table.Columns.Count; c++)
				{
					DbColumn  db_column  = db_table.Columns[c];
					SqlColumn sql_column = db_column.CreateSqlColumn (type_converter);
					
					string db_name_column  = db_column.CreateDisplayName ();
					string ado_name_column = db_column.CreateSqlName ();
					
					System.Type native_type = TypeConverter.MapToNativeType (sql_column.Type);
					
					ado_table.Columns.Add (db_name_column, native_type);
				}
			}
			
			this.CreateDataRelations ();
		}
		
		public void CreateNewRow(string table_name, out System.Data.DataRow data_row)
		{
			this.CheckValidDataSet ();
			
			System.Data.DataTable table = this.data_set.Tables[table_name];
			
			if (table == null)
			{
				throw new System.ArgumentException (string.Format ("Table {0} not found.", table_name), "table_name");
			}
			
			data_row = table.NewRow ();
			
			DbKey key = new DbKey (DbKey.CreateTemporaryId (), 0, DbRowStatus.Live);
			
			data_row[Tags.ColumnId]       = key.Id;
			data_row[Tags.ColumnRevision] = key.Revision;
			data_row[Tags.ColumnStatus]   = key.IntStatus;
			
			table.Rows.Add (data_row);
		}
		
		
		protected void CreateDataRelations ()
		{
			//	Crée pour le DataSet actuel les relations entre les diverses colonnes,
			//	en s'appuyant sur les propriétés de DbTable/DbColumn.
			
			for (int i = 0; i < this.tables.Count; i++)
			{
				DbTable               db_child_table  = this.tables[i];
				System.Data.DataTable ado_child_table = this.data_set.Tables[db_child_table.Name];
				DbForeignKey[]        db_foreign_keys = db_child_table.ForeignKeys;
				
				foreach (DbForeignKey fk in db_foreign_keys)
				{
					int n = fk.Columns.Length;
					
					System.Data.DataTable ado_parent_table = this.data_set.Tables[fk.ParentTableName];
					
					if (ado_parent_table == null)
					{
						//	La table parent n'est pas chargée dans le DataSet, ce qui veut dire
						//	que l'on doit ignorer la relation.
						
						continue;
					}
					
					System.Data.DataColumn[] ado_parent_cols = new System.Data.DataColumn[n];
					System.Data.DataColumn[] ado_child_cols  = new System.Data.DataColumn[n];
					
					for (int j = 0; j < n; j++)
					{
						ado_child_cols[j]  = ado_child_table.Columns[fk.Columns[j].CreateDisplayName ()];
						ado_parent_cols[j] = ado_parent_table.Columns[fk.Columns[j].ParentColumnName];
					}
					
					System.Data.DataRelation relation = new System.Data.DataRelation (null, ado_parent_cols, ado_child_cols);
					this.data_set.Relations.Add (relation);
					
					System.Data.ForeignKeyConstraint constraint = relation.ChildKeyConstraint;
					
					System.Diagnostics.Debug.Assert (constraint != null);
					System.Diagnostics.Debug.Assert (constraint.UpdateRule == System.Data.Rule.Cascade);
				}
			}
		}
		
		protected void SetCommandTransaction()
		{
			for (int i = 0; i < this.commands.Count; i++)
			{
				this.commands[i].Transaction = this.transaction;
			}
		}
		
		protected void CheckValidDataSet()
		{
			if (this.data_set == null)
			{
				throw new System.InvalidOperationException ("No data set defined.");
			}
		}
		
		
		protected Collections.DbCommands		commands;
		protected System.Data.IDbTransaction	transaction;
		protected Collections.DbTables			tables;
		protected System.Data.DataSet			data_set;
		protected System.Data.IDataAdapter[]	adapters;
	}
}
