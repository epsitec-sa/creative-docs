//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// La classe DbInfrastructure offre le support pour l'infrastructure
	/// nécessaire à toute base de données "Crésus" (tables internes, méta-
	/// données, etc.)
	/// </summary>
	public class DbInfrastructure : System.IDisposable
	{
		public DbInfrastructure()
		{
			this.InitialiseNumDefs ();
			this.InitialiseStrTypes ();
		}
		
		
		public void CreateDatabase(DbAccess db_access)
		{
			if (this.db_access.IsValid)
			{
				throw new DbException (this.db_access, "Database already exists");
			}
			
			this.db_access = db_access;
			this.db_access.Create = true;
			
			this.SetupDatabaseAbstraction ();
			
			//	La base de données vient d'être créée. Elle est donc toute vide (aucune
			//	table n'est encore définie).
			
			System.Diagnostics.Debug.Assert (this.db_abstraction.UserTableNames.Length == 0);
			
			//	Il faut créer les tables internes utilisées pour la gestion des méta-données.
			
			this.CreateTableTableDef ();
			this.CreateTableColumnDef ();
			this.CreateTableTypeDef ();
			this.CreateTableEnumValDef ();
			this.CreateTableRefDef ();
			
			this.FillTableTableDef ();
		}
		
		public void AttachDatabase(DbAccess db_access)
		{
			if (this.db_access.IsValid)
			{
				throw new DbException (this.db_access, "Database already attached");
			}
			
			this.db_access = db_access;
			this.db_access.Create = false;
			
			this.SetupDatabaseAbstraction ();
			
			System.Diagnostics.Debug.Assert (this.db_abstraction.UserTableNames.Length > 0);
		}
		
		
		public void ExecuteSilent()
		{
			using (System.Data.IDbTransaction transaction = this.db_abstraction.BeginTransaction ())
			{
				using (System.Data.IDbCommand command = this.sql_builder.Command)
				{
					command.Transaction = transaction;
					
					try
					{
						this.sql_engine.Execute (command, DbCommandType.Silent);
					}
					catch
					{
						System.Diagnostics.Debug.WriteLine ("DbInfrastructure.ExecuteSilent: Roll back transaction.");
						
						transaction.Rollback ();
						throw;
					}
				}
				
				transaction.Commit ();
			}
		}
		
		
		#region Internal Management Table Creation
		protected void CreateTableTableDef()
		{
			DbTable  db_table  = this.GetTableDefDbTable ();
			SqlTable sql_table = db_table.CreateSqlTable (this.type_converter);
			
			this.sql_builder.InsertTable (sql_table);
			this.ExecuteSilent ();
		}
		
		protected DbTable GetTableDefDbTable()
		{
			DbTable    table   = new DbTable (DbTable.TagTableDef);
			DbColumn[] columns = new DbColumn[7];
			
			columns[0] = new DbColumn (DbColumn.TagId,			this.num_def_id);
			columns[1] = new DbColumn (DbColumn.TagRevision,	this.num_def_revision);
			columns[2] = new DbColumn (DbColumn.TagStatus,		this.num_def_status);
			columns[3] = new DbColumn (DbColumn.TagName,		this.str_type_name, Nullable.No);
			columns[4] = new DbColumn (DbColumn.TagCaption,		this.str_type_caption, Nullable.Yes);
			columns[5] = new DbColumn (DbColumn.TagDescription,	this.str_type_description, Nullable.Yes);
			columns[6] = new DbColumn (DbColumn.TagInfoXml,		this.str_type_info_xml, Nullable.No);
			
			table.Columns.AddRange (columns);
			table.PrimaryKey = new DbColumn[] { columns[0], columns[1] };
			
			return table;
		}
		
		protected virtual void CreateTableColumnDef()
		{
			SqlTable    table   = new SqlTable (DbTable.TagColumnDef);
			SqlColumn[] columns = new SqlColumn[8];
			
			columns[0] = new SqlColumn (DbColumn.TagId,				DbRawType.Int32);
			columns[1] = new SqlColumn (DbColumn.TagRevision,		DbRawType.Int32);
			columns[2] = new SqlColumn (DbColumn.TagStatus,			DbRawType.Int32);
			columns[3] = new SqlColumn (DbColumn.TagName,			DbRawType.String, DbColumn.MaxNameLength,			false, Nullable.No);
			columns[4] = new SqlColumn (DbColumn.TagCaption,		DbRawType.String, DbColumn.MaxCaptionLength,		false, Nullable.Yes);
			columns[5] = new SqlColumn (DbColumn.TagDescription,	DbRawType.String, DbColumn.MaxDescriptionLength,	false, Nullable.Yes);
			columns[6] = new SqlColumn (DbColumn.TagInfoXml,		DbRawType.String, DbColumn.MaxInfoXmlLength,	false, Nullable.No);
			columns[7] = new SqlColumn (DbColumn.TagRefTable,		DbRawType.Int32);
			
			table.Columns.AddRange (columns);
			table.PrimaryKey = new SqlColumn[] { columns[0], columns[1] };
			
			this.sql_builder.InsertTable (table);
			this.ExecuteSilent ();
		}
		
		protected virtual void CreateTableTypeDef()
		{
			SqlTable    table   = new SqlTable (DbTable.TagTypeDef);
			SqlColumn[] columns = new SqlColumn[7];
			
			columns[0] = new SqlColumn (DbColumn.TagId,				DbRawType.Int32);
			columns[1] = new SqlColumn (DbColumn.TagRevision,		DbRawType.Int32);
			columns[2] = new SqlColumn (DbColumn.TagStatus,			DbRawType.Int32);
			columns[3] = new SqlColumn (DbColumn.TagName,			DbRawType.String, DbColumn.MaxNameLength,			false, Nullable.No);
			columns[4] = new SqlColumn (DbColumn.TagCaption,		DbRawType.String, DbColumn.MaxCaptionLength,		false, Nullable.Yes);
			columns[5] = new SqlColumn (DbColumn.TagDescription,	DbRawType.String, DbColumn.MaxDescriptionLength,	false, Nullable.Yes);
			columns[6] = new SqlColumn (DbColumn.TagInfoXml,		DbRawType.String, DbColumn.MaxInfoXmlLength,		false, Nullable.No);
			
			table.Columns.AddRange (columns);
			table.PrimaryKey = new SqlColumn[] { columns[0], columns[1] };
			
			this.sql_builder.InsertTable (table);
			this.ExecuteSilent ();
		}
		
		protected virtual void CreateTableEnumValDef()
		{
			SqlTable    table   = new SqlTable (DbTable.TagEnumValDef);
			SqlColumn[] columns = new SqlColumn[7];
			
			columns[0] = new SqlColumn (DbColumn.TagId,				DbRawType.Int32);
			columns[1] = new SqlColumn (DbColumn.TagRevision,		DbRawType.Int32);
			columns[2] = new SqlColumn (DbColumn.TagStatus,			DbRawType.Int32);
			columns[3] = new SqlColumn (DbColumn.TagName,			DbRawType.String, DbColumn.MaxNameLength,			false, Nullable.No);
			columns[4] = new SqlColumn (DbColumn.TagCaption,		DbRawType.String, DbColumn.MaxCaptionLength,		false, Nullable.Yes);
			columns[5] = new SqlColumn (DbColumn.TagDescription,	DbRawType.String, DbColumn.MaxDescriptionLength,	false, Nullable.Yes);
			columns[6] = new SqlColumn (DbColumn.TagRefType,		DbRawType.Int32);
			
			table.Columns.AddRange (columns);
			table.PrimaryKey = new SqlColumn[] { columns[0], columns[1] };
			
			this.sql_builder.InsertTable (table);
			this.ExecuteSilent ();
		}
		
		protected virtual void CreateTableRefDef()
		{
			SqlTable    table   = new SqlTable (DbTable.TagRefDef);
			SqlColumn[] columns = new SqlColumn[4];
			
			columns[0] = new SqlColumn (DbColumn.TagId,		DbRawType.Int32);
			columns[1] = new SqlColumn (DbColumn.TagName,	DbRawType.String, DbColumn.MaxNameLength, false, Nullable.No);
			columns[2] = new SqlColumn (DbColumn.TagSource,	DbRawType.Int32);
			columns[3] = new SqlColumn (DbColumn.TagTarget,	DbRawType.Int32);
			
			table.Columns.AddRange (columns);
			table.PrimaryKey = new SqlColumn[] { columns[0] };
			
			this.sql_builder.InsertTable (table);
			this.ExecuteSilent ();
		}
		#endregion
		
		protected SqlField CreateField(DbColumn column, int value)
		{
			return column.CreateSqlField (this.type_converter, value);
		}
		
		protected SqlField CreateField(DbColumn column, long value)
		{
			return column.CreateSqlField (this.type_converter, value);
		}
		
		protected SqlField CreateField(DbColumn column, string value)
		{
			return column.CreateSqlField (this.type_converter, value);
		}
		
		protected void InsertTableDef(DbTable table, DbKey key, string name, string info_xml)
		{
			SqlFieldCollection fields = new SqlFieldCollection ();
			
			fields.Add (this.CreateField (table.Columns[DbColumn.TagId], key.Id));
			fields.Add (this.CreateField (table.Columns[DbColumn.TagRevision], key.Revision));
			fields.Add (this.CreateField (table.Columns[DbColumn.TagStatus], key.RawStatus));
			fields.Add (this.CreateField (table.Columns[DbColumn.TagName], name));
			fields.Add (this.CreateField (table.Columns[DbColumn.TagInfoXml], info_xml));
			
			this.sql_builder.InsertData (table.Name, fields);
			
			this.ExecuteSilent ();
		}
		
		protected virtual void FillTableTableDef()
		{
			DbTable table = this.GetTableDefDbTable ();
			
			this.InsertTableDef (table, new DbKey (1), DbTable.TagTableDef,		"<table/>");
			this.InsertTableDef (table, new DbKey (2), DbTable.TagColumnDef,	"<table/>");
			this.InsertTableDef (table, new DbKey (3), DbTable.TagTypeDef,		"<table/>");
			this.InsertTableDef (table, new DbKey (4), DbTable.TagEnumValDef,	"<table/>");
			this.InsertTableDef (table, new DbKey (5), DbTable.TagRefDef,		"<table/>");
		}
		
		protected virtual void InitTableEnumValDef()
		{
			
		}
		
		protected virtual void AddRefDef(DbKey source, DbKey target, string name)
		{
		}
		
		#region IDisposable Members
		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (this);
		}
		#endregion
		
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.db_abstraction != null)
				{
					this.db_abstraction.Dispose ();
					this.db_abstraction = null;
					this.sql_builder = null;
					this.sql_engine = null;
					this.type_converter = null;
				}
				
				System.Diagnostics.Debug.Assert (this.sql_builder == null);
				System.Diagnostics.Debug.Assert (this.sql_engine == null);
				System.Diagnostics.Debug.Assert (this.type_converter == null);
			}
		}
		
		protected virtual void SetupDatabaseAbstraction()
		{
			this.db_abstraction = DbFactory.FindDbAbstraction (this.db_access);
			
			this.sql_builder = this.db_abstraction.SqlBuilder;
			this.sql_engine  = this.db_abstraction.SqlEngine;
			
			this.type_converter = this.db_abstraction.Factory.TypeConverter;
			
			System.Diagnostics.Debug.Assert (this.sql_builder != null);
			System.Diagnostics.Debug.Assert (this.sql_engine != null);
			
			System.Diagnostics.Debug.Assert (this.type_converter != null);
			
			this.sql_builder.AutoClear = true;
		}
		
		protected void InitialiseNumDefs()
		{
			//	Faut-il plutôt utiliser Int64 pour l'ID ???
			
			this.num_def_id       = DbNumDef.FromRawType (DbRawType.Int32);
			this.num_def_revision = DbNumDef.FromRawType (DbRawType.Int32);
			this.num_def_status   = DbNumDef.FromRawType (DbRawType.Int32);
		}
		
		protected void InitialiseStrTypes()
		{
			this.str_type_name        = new DbTypeString (DbColumn.MaxNameLength, false);
			this.str_type_caption     = new DbTypeString (DbColumn.MaxCaptionLength, false);
			this.str_type_description = new DbTypeString (DbColumn.MaxDescriptionLength, false);
			this.str_type_info_xml    = new DbTypeString (DbColumn.MaxInfoXmlLength, false);
		}
		
		
		protected DbAccess				db_access;
		protected IDbAbstraction		db_abstraction;
		
		protected ISqlBuilder			sql_builder;
		protected ISqlEngine			sql_engine;
		protected ITypeConverter		type_converter;
		
		protected DbNumDef				num_def_id;
		protected DbNumDef				num_def_revision;
		protected DbNumDef				num_def_status;
		
		protected DbTypeString			str_type_name;
		protected DbTypeString			str_type_caption;
		protected DbTypeString			str_type_description;
		protected DbTypeString			str_type_info_xml;
	}
}
