//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

namespace Epsitec.Cresus.Database
{
	using Tags = Epsitec.Common.Support.Tags;
	
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
			
			this.InitialiseDatabaseAbstraction ();
			
			//	La base de données vient d'être créée. Elle est donc toute vide (aucune
			//	table n'est encore définie).
			
			System.Diagnostics.Debug.Assert (this.db_abstraction.UserTableNames.Length == 0);
			
			//	Il faut créer les tables internes utilisées pour la gestion des méta-données.
			
			this.BootCreateTableTableDef ();
			this.BootCreateTableColumnDef ();
			this.BootCreateTableTypeDef ();
			this.BootCreateTableEnumValDef ();
			this.BootCreateTableRefDef ();
			
			this.BootSetupTables ();
		}
		
		public void AttachDatabase(DbAccess db_access)
		{
			if (this.db_access.IsValid)
			{
				throw new DbException (this.db_access, "Database already attached");
			}
			
			this.db_access = db_access;
			this.db_access.Create = false;
			
			this.InitialiseDatabaseAbstraction ();
			
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
		
		
		#region Bootstrapping
		protected void BootCreateTableTableDef()
		{
			DbTable    table   = new DbTable (DbTable.TagTableDef);
			DbColumn[] columns = new DbColumn[7];
			
			columns[0] = new DbColumn (DbColumn.TagId,			this.num_type_id);
			columns[1] = new DbColumn (DbColumn.TagRevision,	this.num_type_revision);
			columns[2] = new DbColumn (DbColumn.TagStatus,		this.num_type_status);
			columns[3] = new DbColumn (DbColumn.TagName,		this.str_type_name, Nullable.No);
			columns[4] = new DbColumn (DbColumn.TagCaption,		this.str_type_caption, Nullable.Yes);
			columns[5] = new DbColumn (DbColumn.TagDescription,	this.str_type_description, Nullable.Yes);
			columns[6] = new DbColumn (DbColumn.TagInfoXml,		this.str_type_info_xml, Nullable.No);
			
			table.Columns.AddRange (columns);
			table.PrimaryKey = new DbColumn[] { columns[0], columns[1] };
			
			this.internal_tables.Add (table);
			
			SqlTable sql_table = table.CreateSqlTable (this.type_converter);
			this.sql_builder.InsertTable (sql_table);
			this.ExecuteSilent ();
		}
		
		protected void BootCreateTableColumnDef()
		{
			DbTable    table   = new DbTable (DbTable.TagColumnDef);
			DbColumn[] columns = new DbColumn[9];
			
			columns[0] = new DbColumn (DbColumn.TagId,			this.num_type_id);
			columns[1] = new DbColumn (DbColumn.TagRevision,	this.num_type_revision);
			columns[2] = new DbColumn (DbColumn.TagStatus,		this.num_type_status);
			columns[3] = new DbColumn (DbColumn.TagName,		this.str_type_name, Nullable.No);
			columns[4] = new DbColumn (DbColumn.TagCaption,		this.str_type_caption, Nullable.Yes);
			columns[5] = new DbColumn (DbColumn.TagDescription,	this.str_type_description, Nullable.Yes);
			columns[6] = new DbColumn (DbColumn.TagInfoXml,		this.str_type_info_xml, Nullable.No);
			columns[7] = new DbColumn (DbColumn.TagRefTable,	this.num_type_id);
			columns[8] = new DbColumn (DbColumn.TagRefType,		this.num_type_id);
			
			table.Columns.AddRange (columns);
			table.PrimaryKey = new DbColumn[] { columns[0], columns[1] };
			
			this.internal_tables.Add (table);
			
			SqlTable sql_table = table.CreateSqlTable (this.type_converter);
			this.sql_builder.InsertTable (sql_table);
			this.ExecuteSilent ();
		}
		
		protected void BootCreateTableTypeDef()
		{
			DbTable    table   = new DbTable (DbTable.TagTypeDef);
			DbColumn[] columns = new DbColumn[7];
			
			columns[0] = new DbColumn (DbColumn.TagId,			this.num_type_id);
			columns[1] = new DbColumn (DbColumn.TagRevision,	this.num_type_revision);
			columns[2] = new DbColumn (DbColumn.TagStatus,		this.num_type_status);
			columns[3] = new DbColumn (DbColumn.TagName,		this.str_type_name, Nullable.No);
			columns[4] = new DbColumn (DbColumn.TagCaption,		this.str_type_caption, Nullable.Yes);
			columns[5] = new DbColumn (DbColumn.TagDescription,	this.str_type_description, Nullable.Yes);
			columns[6] = new DbColumn (DbColumn.TagInfoXml,		this.str_type_info_xml, Nullable.No);
			
			table.Columns.AddRange (columns);
			table.PrimaryKey = new DbColumn[] { columns[0], columns[1] };
			
			this.internal_tables.Add (table);
			
			SqlTable sql_table = table.CreateSqlTable (this.type_converter);
			this.sql_builder.InsertTable (sql_table);
			this.ExecuteSilent ();
		}
		
		protected void BootCreateTableEnumValDef()
		{
			DbTable    table   = new DbTable (DbTable.TagEnumValDef);
			DbColumn[] columns = new DbColumn[8];
			
			columns[0] = new DbColumn (DbColumn.TagId,			this.num_type_id);
			columns[1] = new DbColumn (DbColumn.TagRevision,	this.num_type_revision);
			columns[2] = new DbColumn (DbColumn.TagStatus,		this.num_type_status);
			columns[3] = new DbColumn (DbColumn.TagName,		this.str_type_name, Nullable.No);
			columns[4] = new DbColumn (DbColumn.TagCaption,		this.str_type_caption, Nullable.Yes);
			columns[5] = new DbColumn (DbColumn.TagDescription,	this.str_type_description, Nullable.Yes);
			columns[6] = new DbColumn (DbColumn.TagInfoXml,		this.str_type_info_xml, Nullable.No);
			columns[7] = new DbColumn (DbColumn.TagRefType,		this.num_type_id);
			
			table.Columns.AddRange (columns);
			table.PrimaryKey = new DbColumn[] { columns[0], columns[1] };
			
			this.internal_tables.Add (table);
			
			SqlTable sql_table = table.CreateSqlTable (this.type_converter);
			this.sql_builder.InsertTable (sql_table);
			this.ExecuteSilent ();
		}
		
		protected void BootCreateTableRefDef()
		{
			DbTable    table   = new DbTable (DbTable.TagRefDef);
			DbColumn[] columns = new DbColumn[4];
			
			columns[0] = new DbColumn (DbColumn.TagId,			this.num_type_id);
			columns[1] = new DbColumn (DbColumn.TagRefColumn,	this.num_type_id);
			columns[2] = new DbColumn (DbColumn.TagRefSource,	this.num_type_id);
			columns[3] = new DbColumn (DbColumn.TagRefTarget,	this.num_type_id);
			
			table.Columns.AddRange (columns);
			table.PrimaryKey = new DbColumn[] { columns[0] };
			
			this.internal_tables.Add (table);
			
			SqlTable sql_table = table.CreateSqlTable (this.type_converter);
			this.sql_builder.InsertTable (sql_table);
			this.ExecuteSilent ();
		}
		
		
		protected void BootInsertTypeDefRow(DbType type)
		{
			DbTable type_def = this.internal_tables[DbTable.TagTypeDef];
			
			//	Phase d'initialisation de la base : insère une ligne dans la table de définition des
			//	types. Les colonnes descriptives (pour l'utilisateur) ne sont pas initialisées.
			
			SqlFieldCollection fields = new SqlFieldCollection ();
			
			fields.Add (type_def.Columns[DbColumn.TagId]		.CreateSqlField (this.type_converter, type.InternalKey.Id));
			fields.Add (type_def.Columns[DbColumn.TagRevision]	.CreateSqlField (this.type_converter, type.InternalKey.Revision));
			fields.Add (type_def.Columns[DbColumn.TagStatus]	.CreateSqlField (this.type_converter, type.InternalKey.RawStatus));
			fields.Add (type_def.Columns[DbColumn.TagName]		.CreateSqlField (this.type_converter, type.Name));
			fields.Add (type_def.Columns[DbColumn.TagInfoXml]	.CreateSqlField (this.type_converter, DbTypeFactory.ConvertTypeToXml (type)));
			
			this.sql_builder.InsertData (type_def.Name, fields);
			
			this.ExecuteSilent ();
		}
		
		protected void BootInsertTableDefRow(DbTable table)
		{
			DbTable table_def = this.internal_tables[DbTable.TagTableDef];
			
			//	Phase d'initialisation de la base : insère une ligne dans la table de définition des
			//	tables. Les colonnes descriptives (pour l'utilisateur) ne sont pas initialisées.
			
			SqlFieldCollection fields = new SqlFieldCollection ();
			
			fields.Add (table_def.Columns[DbColumn.TagId]		.CreateSqlField (this.type_converter, table.InternalKey.Id));
			fields.Add (table_def.Columns[DbColumn.TagRevision]	.CreateSqlField (this.type_converter, table.InternalKey.Revision));
			fields.Add (table_def.Columns[DbColumn.TagStatus]	.CreateSqlField (this.type_converter, table.InternalKey.RawStatus));
			fields.Add (table_def.Columns[DbColumn.TagName]		.CreateSqlField (this.type_converter, table.Name));
			fields.Add (table_def.Columns[DbColumn.TagInfoXml]	.CreateSqlField (this.type_converter, "<table/>"));
			
			this.sql_builder.InsertData (table_def.Name, fields);
			
			this.ExecuteSilent ();
		}
		
		protected void BootInsertColumnDefRow(DbTable table, DbColumn column)
		{
			DbTable column_def = this.internal_tables[DbTable.TagColumnDef];
			
			//	Phase d'initialisation de la base : insère une ligne dans la table de définition des
			//	colonnes. Les colonnes descriptives (pour l'utilisateur) ne sont pas initialisées.
			
			SqlFieldCollection fields = new SqlFieldCollection ();
			
			fields.Add (column_def.Columns[DbColumn.TagId]		.CreateSqlField (this.type_converter, column.InternalKey.Id));
			fields.Add (column_def.Columns[DbColumn.TagRevision].CreateSqlField (this.type_converter, column.InternalKey.Revision));
			fields.Add (column_def.Columns[DbColumn.TagStatus]	.CreateSqlField (this.type_converter, column.InternalKey.RawStatus));
			fields.Add (column_def.Columns[DbColumn.TagName]	.CreateSqlField (this.type_converter, column.Name));
			fields.Add (column_def.Columns[DbColumn.TagInfoXml]	.CreateSqlField (this.type_converter, DbColumn.ConvertColumnToXml (column)));
			fields.Add (column_def.Columns[DbColumn.TagRefTable].CreateSqlField (this.type_converter, table.InternalKey.Id));
			fields.Add (column_def.Columns[DbColumn.TagRefType]	.CreateSqlField (this.type_converter, column.Type.InternalKey.Id));
			
			this.sql_builder.InsertData (column_def.Name, fields);
			
			this.ExecuteSilent ();
		}
		
		protected void BootInsertRefDefRow(int ref_id, string src_table_name, string src_column_name, string target_table_name)
		{
			DbTable ref_def = this.internal_tables[DbTable.TagRefDef];
			
			//	Phase d'initialisation de la base : insère une ligne dans la table de définition des
			//	références.
			
			SqlFieldCollection fields = new SqlFieldCollection ();
			
			DbTable  source = this.internal_tables[src_table_name];
			DbTable  target = this.internal_tables[target_table_name];
			DbColumn column = source.Columns[src_column_name];
			
			fields.Add (ref_def.Columns[DbColumn.TagId].CreateSqlField (this.type_converter, ref_id));
			fields.Add (ref_def.Columns[DbColumn.TagRefColumn].CreateSqlField (this.type_converter, column.InternalKey.Id));
			fields.Add (ref_def.Columns[DbColumn.TagRefSource].CreateSqlField (this.type_converter, source.InternalKey.Id));
			fields.Add (ref_def.Columns[DbColumn.TagRefTarget].CreateSqlField (this.type_converter, target.InternalKey.Id));
			
			this.sql_builder.InsertData (ref_def.Name, fields);
			
			this.ExecuteSilent ();
		}
		
		
		protected void BootSetupTables()
		{
			int type_key_id   = 1;
			int table_key_id  = 1;
			int column_key_id = 1;
			
			//	Il faut commencer par finir d'initialiser les descriptions des types, parce
			//	que les description des colonnes doivent y faire référence.
			
			foreach (DbType type in this.internal_types)
			{
				//	Attribue à chaque type interne une clef unique et établit les informations de base
				//	dans la table de définition des types.
				
				type.DefineInternalKey (new DbKey (type_key_id++));
				this.BootInsertTypeDefRow (type);
			}
			
			foreach (DbTable table in this.internal_tables)
			{
				//	Attribue à chaque table interne une clef unique et établit les informations de base
				//	dans la table de définition des tables.
				
				table.DefineInternalKey (new DbKey (table_key_id++));
				this.BootInsertTableDefRow (table);
				
				foreach (DbColumn column in table.Columns)
				{
					//	Pour chaque colonne de la table, établit les informations de base dans la table de
					//	définition des colonnes.
					
					column.DefineInternalKey (new DbKey (column_key_id++));
					this.BootInsertColumnDefRow (table, column);
				}
			}
			
			//	Complète encore les informations au sujet des relations :
			//
			//	- La description d'une colonne fait référence à la table et à un type.
			//	- La description d'une valeur d'enum fait référence à un type.
			//	- La description d'une référence fait elle-même référence à la table
			//	  source et destination, ainsi qu'à la colonne.
			
			this.BootInsertRefDefRow (1, DbTable.TagColumnDef,  DbColumn.TagRefTable,  DbTable.TagTableDef);
			this.BootInsertRefDefRow (2, DbTable.TagColumnDef,  DbColumn.TagRefType,   DbTable.TagTypeDef);
			this.BootInsertRefDefRow (3, DbTable.TagEnumValDef, DbColumn.TagRefType,   DbTable.TagTypeDef);
			this.BootInsertRefDefRow (4, DbTable.TagRefDef,     DbColumn.TagRefColumn, DbTable.TagColumnDef);
			this.BootInsertRefDefRow (5, DbTable.TagRefDef,     DbColumn.TagRefSource, DbTable.TagTableDef);
			this.BootInsertRefDefRow (6, DbTable.TagRefDef,     DbColumn.TagRefTarget, DbTable.TagTableDef);
		}
		#endregion
		
		#region Initialisation
		protected void InitialiseDatabaseAbstraction()
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
			
			this.num_type_id       = new DbTypeNum (DbNumDef.FromRawType (DbRawType.Int32), Tags.Name + "=CR.KeyId");
			this.num_type_revision = new DbTypeNum (DbNumDef.FromRawType (DbRawType.Int32), Tags.Name + "=CR.KeyRevision");
			this.num_type_status   = new DbTypeNum (DbNumDef.FromRawType (DbRawType.Int32), Tags.Name + "=CR.KeyStatus");
			
			this.internal_types.Add (this.num_type_id);
			this.internal_types.Add (this.num_type_revision);
			this.internal_types.Add (this.num_type_status);
		}
		
		protected void InitialiseStrTypes()
		{
			this.str_type_name        = new DbTypeString (DbColumn.MaxNameLength, false,		Tags.Name + "=CR.Name");
			this.str_type_caption     = new DbTypeString (DbColumn.MaxCaptionLength, false,		Tags.Name + "=CR.Caption");
			this.str_type_description = new DbTypeString (DbColumn.MaxDescriptionLength, false, Tags.Name + "=CR.Description");
			this.str_type_info_xml    = new DbTypeString (DbColumn.MaxInfoXmlLength, false,		Tags.Name + "=CR.InfoXml");
			
			this.internal_types.Add (this.str_type_name);
			this.internal_types.Add (this.str_type_caption);
			this.internal_types.Add (this.str_type_description);
			this.internal_types.Add (this.str_type_info_xml);
		}
		#endregion
		
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
		
		
		
		
		protected DbAccess				db_access;
		protected IDbAbstraction		db_abstraction;
		
		protected ISqlBuilder			sql_builder;
		protected ISqlEngine			sql_engine;
		protected ITypeConverter		type_converter;
		
		protected DbTypeNum				num_type_id;
		protected DbTypeNum				num_type_revision;
		protected DbTypeNum				num_type_status;
		protected DbTypeString			str_type_name;
		protected DbTypeString			str_type_caption;
		protected DbTypeString			str_type_description;
		protected DbTypeString			str_type_info_xml;
		
		protected DbTableCollection		internal_tables = new DbTableCollection ();
		protected DbTypeCollection		internal_types  = new DbTypeCollection ();
	}
}
