//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

namespace Epsitec.Cresus.Database
{
	using Tags = Epsitec.Common.Support.Tags;
	
	public delegate void CallbackDebugDisplayDataSet(DbInfrastructure infrastructure, string name, System.Data.DataTable table);
	
	/// <summary>
	/// La classe DbInfrastructure offre le support pour l'infrastructure
	/// nécessaire à toute base de données "Crésus" (tables internes, méta-
	/// données, etc.)
	/// </summary>
	public class DbInfrastructure : System.IDisposable
	{
		public DbInfrastructure()
		{
			this.localisations = new string[1] { "" };
			
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
						System.Console.Out.WriteLine ("SQL Command: {0}", command.CommandText);
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
		
		public void ExecuteReturningData(out System.Data.DataSet data)
		{
			using (System.Data.IDbTransaction transaction = this.db_abstraction.BeginTransaction ())
			{
				using (System.Data.IDbCommand command = this.sql_builder.Command)
				{
					command.Transaction = transaction;
					
					try
					{
						System.Console.Out.WriteLine ("SQL Command: {0}", command.CommandText);
						this.sql_engine.Execute (command, DbCommandType.ReturningData, out data);
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
		
		
		public DbTable ReadDbTableMeta(string table_name)
		{
			SqlSelect query = new SqlSelect ();
			
			query.Fields.Add ("T_ID",   SqlField.CreateName ("T_TABLE", DbColumn.TagId));
			query.Fields.Add ("T_NAME", SqlField.CreateName ("T_TABLE", DbColumn.TagName));
			query.Fields.Add ("T_INFO", SqlField.CreateName ("T_TABLE", DbColumn.TagInfoXml));
			query.Fields.Add ("C_ID",   SqlField.CreateName ("T_COLUMN", DbColumn.TagId));
			query.Fields.Add ("C_NAME", SqlField.CreateName ("T_COLUMN", DbColumn.TagName));
			query.Fields.Add ("C_INFO", SqlField.CreateName ("T_COLUMN", DbColumn.TagInfoXml));
			query.Fields.Add ("C_TYPE", SqlField.CreateName ("T_COLUMN", DbColumn.TagRefType));
			
			query.Tables.Add ("T_TABLE",  SqlField.CreateName (DbTable.TagTableDef));
			query.Tables.Add ("T_COLUMN", SqlField.CreateName (DbTable.TagColumnDef));
			
			query.Conditions.Add (new SqlFunction (SqlFunctionType.CompareEqual, SqlField.CreateName ("T_TABLE", DbColumn.TagName), SqlField.CreateConstant (table_name, DbRawType.String)));
			query.Conditions.Add (new SqlFunction (SqlFunctionType.CompareEqual, SqlField.CreateName ("T_TABLE", DbColumn.TagId), SqlField.CreateName ("T_COLUMN", DbColumn.TagRefTable)));
			
			this.sql_builder.SelectData (query);
			
			System.Data.DataSet   data_set;
			System.Data.DataTable data_table;
			
			this.ExecuteReturningData (out data_set);
			
			if (data_set.Tables.Count == 0)
			{
				//	La table n'existe pas (on n'a pas trouvé de description dans la base),
				//	alors on retourne simplement null plutôt que de lever une exception.
				
				return null;
			}
			
			System.Diagnostics.Debug.Assert (data_set.Tables.Count == 1);
			
			data_table = data_set.Tables[0];
			
			if (this.debug_display_data_set != null)
			{
				this.debug_display_data_set (this, table_name, data_table);
			}
			
			foreach (System.Data.DataRow data_row in data_table.Rows)
			{
				long type_ref_id = long.Parse (data_row["C_TYPE"].ToString ());
				this.ResolveType (new DbKey (type_ref_id));
			}
			
			return null;
		}
		
		
		public DbType ResolveType(DbKey type_ref)
		{
			lock (this.cache_db_types)
			{
				DbType type = this.cache_db_types[type_ref];
				if (type == null)
				{
					type = this.LoadType (type_ref);
					this.cache_db_types[type_ref] = type;
				}
				return type;
			}
		}
		
		public DbType LoadType(DbKey type_ref)
		{
			SqlSelect query = new SqlSelect ();
			
			query.Fields.Add ("T_NAME", SqlField.CreateName ("T_TYPE", DbColumn.TagName));
			query.Fields.Add ("T_INFO", SqlField.CreateName ("T_TYPE", DbColumn.TagInfoXml));
			
			this.AddLocalisedColumns (query, "T_TYPE", DbColumn.TagCaption);
			this.AddLocalisedColumns (query, "T_TYPE", DbColumn.TagDescription);
			
			query.Tables.Add ("T_TYPE", SqlField.CreateName (DbTable.TagTypeDef));
			
			//	Cherche la ligne de la table dont 'CR_ID = type_ref' en ne considérant que
			//	l'ID et dont la révision = 0.
			
			this.AddKeyExtraction (query, type_ref, "T_TYPE", DbKeyMatchMode.LiveId);
			
			this.sql_builder.SelectData (query);
			
			System.Data.DataSet data_set;
			System.Data.DataRow data_row;
			
			this.ExecuteReturningData (out data_set);
			
			if ((data_set.Tables.Count != 1) ||
				(data_set.Tables[0].Rows.Count != 1))
			{
				//	On devrait toujours avoir exactement une table avec exactement une
				//	ligne pour cette requête. Le contraire serait une erreur grave !
				
				throw new DbException (this.db_access, string.Format ("DbType: query returned garbage on ID={0}.", type_ref));
			}
			
			data_row = data_set.Tables[0].Rows[0];
			
			string type_name = data_row["T_NAME"] as string;
			string type_info = data_row["T_INFO"] as string;
			
			//	A partir de l'information trouvée dans la base, génère l'objet DbType
			//	qui correspond.
			
			DbType type = DbTypeFactory.NewType (type_info);
			
			type.DefineName (type_name);
			type.DefineInternalKey (type_ref);
			
			this.DefineLocalisedAttributes (data_row, DbColumn.TagCaption,     type.Attributes, Tags.Caption);
			this.DefineLocalisedAttributes (data_row, DbColumn.TagDescription, type.Attributes, Tags.Description);
			
			if (type is DbTypeEnum)
			{
				this.LoadEnumValues (type as DbTypeEnum);
			}
			
			data_set.Dispose ();
			
			return type;
		}
		
		
		protected void LoadEnumValues(DbTypeEnum type_enum)
		{
			System.Diagnostics.Debug.Assert (type_enum != null);
			System.Diagnostics.Debug.Assert (type_enum.Count == 0);
			
			SqlSelect query = new SqlSelect ();
			
			query.Fields.Add ("E_ID",   SqlField.CreateName ("T_ENUM", DbColumn.TagId));
			query.Fields.Add ("E_NAME", SqlField.CreateName ("T_ENUM", DbColumn.TagName));
			query.Fields.Add ("E_INFO", SqlField.CreateName ("T_ENUM", DbColumn.TagInfoXml));
			
			this.AddLocalisedColumns (query, "T_ENUM", DbColumn.TagCaption);
			this.AddLocalisedColumns (query, "T_ENUM", DbColumn.TagDescription);
			
			query.Tables.Add ("T_ENUM", SqlField.CreateName (DbTable.TagEnumValDef));
			
			//	Cherche les lignes de la table dont la colonne CREF_TYPE correspond à l'ID du type.
			
			this.AddKeyExtraction (query, "T_ENUM", DbColumn.TagRefType, type_enum.InternalKey);
			
			this.sql_builder.SelectData (query);
			
			System.Data.DataSet data_set;
			
			this.ExecuteReturningData (out data_set);
			
			if (data_set.Tables.Count == 0)
			{
				//	Si aucune information n'existe pour ce type, c'est une erreur fatale,
				//	car on avait une référence d'énumération valide.
				
				throw new System.ArgumentException (string.Format ("Type {0} has no DbEnumValues defined.", type_enum.Name));
			}
			
			System.Diagnostics.Debug.Assert (data_set.Tables.Count == 1);
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			
			foreach (System.Data.DataRow data_row in data_set.Tables[0].Rows)
			{
				//	Pour chaque valeur retournée dans la table, il y a une ligne. Cette ligne
				//	contient toute l'information nécessaire à la création d'une instance de la
				//	class DbEnumValue :
				
				DbEnumValue value = new DbEnumValue ();
				
				string arg_name;
				string arg_info;
				string arg_id;
				
				Epsitec.Common.Support.Data.Converter.Convert (data_row["E_NAME"], out arg_name);
				Epsitec.Common.Support.Data.Converter.Convert (data_row["E_INFO"], out arg_info);
				Epsitec.Common.Support.Data.Converter.Convert (data_row["E_ID"],   out arg_id);
				
				value.Attributes.SetAttribute (Tags.Name,	 arg_name);
				value.Attributes.SetAttribute (Tags.InfoXml, arg_info);
				value.Attributes.SetAttribute (Tags.Id,		 arg_id);
				
				this.DefineLocalisedAttributes (data_row, DbColumn.TagCaption,     value.Attributes, Tags.Caption);
				this.DefineLocalisedAttributes (data_row, DbColumn.TagDescription, value.Attributes, Tags.Description);
				
				list.Add (value);
			}
			
			//	Initialise le type avec la liste des valeurs de l'énumération.
			
			type_enum.Initialise (list);
			
			data_set.Dispose ();
		}
		
		protected void DefineLocalisedAttributes(System.Data.DataRow row, string column, DbAttributes attributes, string tag)
		{
			string alias = "LOC_" + column;
			string value;
			
			for (int i = 0; i < this.localisations.Length; i++)
			{
				string locale = this.localisations[i];
				string index  = locale == "" ? alias : alias + "_" + locale;
				
				if (Epsitec.Common.Support.Data.Converter.Convert (row[index], out value))
				{
					attributes.SetAttribute (tag, value, locale);
				}
			}
		}
		
		protected void AddLocalisedColumns(SqlSelect query, string table, string column)
		{
			string alias = "LOC_" + column;
			
			for (int i = 0; i < this.localisations.Length; i++)
			{
				string locale = this.localisations[i];
				string index  = locale == "" ? alias : alias + "_" + locale;
				
				query.Fields.Add (index, SqlField.CreateName (table, column));
			}
		}
		
		
		protected void AddKeyExtraction(SqlSelect query, DbKey key, string target_table_name, DbKeyMatchMode mode)
		{
			SqlField name_col_id = SqlField.CreateName (target_table_name, DbColumn.TagId);
			SqlField constant_id = SqlField.CreateConstant (key.Id, DbRawType.Int64);
			
			query.Conditions.Add (new SqlFunction (SqlFunctionType.CompareEqual, name_col_id, constant_id));
			
			int revision = 0;
			
			if (mode == DbKeyMatchMode.ExactRevisionId)
			{
				revision = key.Revision;
			}
			
			if ((mode == DbKeyMatchMode.LiveId) ||
				(mode == DbKeyMatchMode.ExactRevisionId))
			{
				SqlField name_col_rev = SqlField.CreateName (target_table_name, DbColumn.TagRevision);
				SqlField constant_rev = SqlField.CreateConstant (revision, DbRawType.Int32);
			
				query.Conditions.Add (new SqlFunction (SqlFunctionType.CompareEqual, name_col_rev, constant_rev));
			}
		}
		
		protected void AddKeyExtraction(SqlSelect query, string source_table_name, string source_col_id, string target_table_name)
		{
			SqlField target_col = SqlField.CreateName (target_table_name, DbColumn.TagId);
			SqlField source_col = SqlField.CreateName (source_table_name, source_col_id);
			
			query.Conditions.Add (new SqlFunction (SqlFunctionType.CompareEqual, source_col, target_col));
		}
		
		protected void AddKeyExtraction(SqlSelect query, string source_table_name, string source_col_id, DbKey key)
		{
			SqlField source_col  = SqlField.CreateName (source_table_name, source_col_id);
			SqlField constant_id = SqlField.CreateConstant (key.Id, DbRawType.Int64);
			
			query.Conditions.Add (new SqlFunction (SqlFunctionType.CompareEqual, source_col, constant_id));
		}
		
		public CallbackDebugDisplayDataSet DebugDisplayDataSet
		{
			set { this.debug_display_data_set = value; }
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
		
		CallbackDebugDisplayDataSet		debug_display_data_set;
		string[]						localisations;
		
		Cache.DbTypes					cache_db_types = new Cache.DbTypes ();
		
	}
}
