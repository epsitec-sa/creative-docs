//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

namespace Epsitec.Cresus.Database
{
	using Tags = Epsitec.Common.Support.Tags;
	using Converter = Epsitec.Common.Support.Data.Converter;
	
	public delegate void CallbackDisplayDataSet(DbInfrastructure infrastructure, string name, System.Data.DataTable table);
	
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
			
			this.internal_tables.Add (this.ResolveDbTable (DbTable.TagTableDef));
			this.internal_tables.Add (this.ResolveDbTable (DbTable.TagColumnDef));
			this.internal_tables.Add (this.ResolveDbTable (DbTable.TagTypeDef));
			this.internal_tables.Add (this.ResolveDbTable (DbTable.TagRefDef));
			this.internal_tables.Add (this.ResolveDbTable (DbTable.TagEnumValDef));
			
			this.internal_types.Add (this.ResolveDbType ("CR.KeyId"));
			this.internal_types.Add (this.ResolveDbType ("CR.KeyRevision"));
			this.internal_types.Add (this.ResolveDbType ("CR.KeyStatus"));
			this.internal_types.Add (this.ResolveDbType ("CR.Name"));
			this.internal_types.Add (this.ResolveDbType ("CR.Caption"));
			this.internal_types.Add (this.ResolveDbType ("CR.Description"));
			this.internal_types.Add (this.ResolveDbType ("CR.InfoXml"));
		}
		
		
		public CallbackDisplayDataSet	DisplayDataSet
		{
			set { this.display_data_set = value; }
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
		
		public object ExecuteScalar(System.Data.IDbTransaction transaction)
		{
			using (System.Data.IDbCommand command = this.sql_builder.Command)
			{
				command.Transaction = transaction;
				
				object data;
				
				System.Console.Out.WriteLine ("SQL Command: {0}", command.CommandText);
				this.sql_engine.Execute (command, DbCommandType.ReturningData, out data);
				
				return data;
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
						System.Diagnostics.Debug.WriteLine ("DbInfrastructure.ExecuteReturningData: Roll back transaction.");
						
						transaction.Rollback ();
						throw;
					}
				}
				
				transaction.Commit ();
			}
		}
		
		
		public System.Data.DataTable ExecuteSqlSelect(SqlSelect query, int min_rows)
		{
			this.sql_builder.SelectData (query);
			
			System.Data.DataSet data_set;
			System.Data.DataTable data_table;
			
			this.ExecuteReturningData (out data_set);
			
			if ((data_set == null) ||
				(data_set.Tables.Count != 1))
			{
				throw new DbException (this.db_access, string.Format ("Query failed."));
			}
			
			data_table = data_set.Tables[0];
			
			if (data_table.Rows.Count < min_rows)
			{
				throw new DbException (this.db_access, string.Format ("Query returned to few rows; expected {0}, found {1}.", min_rows, data_table.Rows.Count));
			}
			
			return data_table;
		}
		
		
		public DbKey FindDbTableKey(string name)
		{
			return this.FindLiveKey (this.FindDbKeys (DbTable.TagTableDef, name));
		}
		
		public DbKey FindDbTypeKey(string name)
		{
			return this.FindLiveKey (this.FindDbKeys (DbTable.TagTypeDef, name));
		}
		
		
		internal DbKey FindLiveKey(DbKey[] keys)
		{
			for (int i = 0; i < keys.Length; i++)
			{
				if (keys[i].Revision == 0)
				{
					return keys[i];
				}
			}
			
			return null;
		}
		
		internal DbKey[] FindDbKeys(string table_name, string row_name)
		{
			//	Trouve la (ou les) clefs.
			
			SqlSelect query = new SqlSelect ();
			
			query.Fields.Add ("T_ID",   SqlField.CreateName ("T", DbColumn.TagId));
			query.Fields.Add ("T_REV",	SqlField.CreateName ("T", DbColumn.TagRevision));
			query.Fields.Add ("T_STAT",	SqlField.CreateName ("T", DbColumn.TagStatus));
			
			query.Tables.Add ("T", SqlField.CreateName (table_name));
			
			query.Conditions.Add (new SqlFunction (SqlFunctionType.CompareEqual, SqlField.CreateName ("T", DbColumn.TagName), SqlField.CreateConstant (row_name, DbRawType.String)));
			
			System.Data.DataTable data_table = this.ExecuteSqlSelect (query, 0);
			
			if (this.display_data_set != null)
			{
				this.display_data_set (this, table_name, data_table);
			}
			
			DbKey[] keys = new DbKey[data_table.Rows.Count];
			
			for (int i = 0; i < data_table.Rows.Count; i++)
			{
				System.Data.DataRow row = data_table.Rows[i];
				
				long id;
				int  revision;
				int  status;
				
				Converter.Convert (row["T_ID"],   out id);
				Converter.Convert (row["T_REV"],  out revision);
				Converter.Convert (row["T_STAT"], out status);
				
				keys[i] = new DbKey (id, revision, status);
			}
			
			return keys;
		}
		
		
		public void RegisterNewDbTable(DbTable table)
		{
			//	Enregistre une nouvelle table dans la base de données. Ceci va attribuer à
			//	la table une clef DbKey et vérifier qu'il n'y a pas de collision avec une
			//	éventuelle table déjà existante.
			
			using (System.Data.IDbTransaction transaction = this.db_abstraction.BeginTransaction ())
			{
				try
				{
					int col_num = table.Columns.Count;
					
					long table_id  = this.NewRowIdInTable (transaction, this.internal_tables[DbTable.TagTableDef].InternalKey, 1);
					long column_id = this.NewRowIdInTable (transaction, this.internal_tables[DbTable.TagColumnDef].InternalKey, col_num);
				}
				catch
				{
					transaction.Rollback ();
					throw;
				}
				
				transaction.Commit ();
			}
		}
		
		
		public DbTable ResolveDbTable(string table_name)
		{
			return this.ResolveDbTable (this.FindDbTableKey (table_name));
		}
		
		public DbTable ResolveDbTable(DbKey key)
		{
			if (key == null)
			{
				return null;
			}

			lock (this.cache_db_tables)
			{
				DbTable table = this.cache_db_tables[key];
				
				if (table == null)
				{
					table = this.LoadDbTable (key);
					
					if (table != null)
					{
						System.Diagnostics.Debug.WriteLine (string.Format ("Loaded {0} {1} from database.", table.GetType ().Name, table.Name));
						
						this.cache_db_tables[key] = table;
					}
				}
				
				return table;
			}
		}
		
		public DbType  ResolveDbType(string type_name)
		{
			return this.ResolveDbType (this.FindDbTypeKey (type_name));
		}
		
		public DbType  ResolveDbType(DbKey key)
		{
			if (key == null)
			{
				return null;
			}

			//	Trouve le type corredpondant à une clef spécifique.
			
			lock (this.cache_db_types)
			{
				DbType type = this.cache_db_types[key];
				
				if (type == null)
				{
					type = this.LoadDbType (key);
					
					if (type != null)
					{
						System.Diagnostics.Debug.WriteLine (string.Format ("Loaded {0} {1} from database.", type.GetType ().Name, type.Name));
						
						this.cache_db_types[key] = type;
					}
				}
				
				return type;
			}
		}
		
		
		public DbTable LoadDbTable(DbKey key)
		{
			//	Charge les définitions pour la table au moyen d'une requête unique qui va
			//	aussi retourner les diverses définitions de colonnes.
			
			SqlSelect query = new SqlSelect ();
			
			//	Ce qui est propre à la table :
			
			query.Fields.Add ("T_NAME", SqlField.CreateName ("T_TABLE", DbColumn.TagName));
			query.Fields.Add ("T_INFO", SqlField.CreateName ("T_TABLE", DbColumn.TagInfoXml));
			
			this.AddLocalisedColumns (query, "TABLE_CAPTION", "T_TABLE", DbColumn.TagCaption);
			this.AddLocalisedColumns (query, "TABLE_DESCRIPTION", "T_TABLE", DbColumn.TagDescription);
			
			//	Ce qui est propre aux colonnes :
			
			query.Fields.Add ("C_ID",   SqlField.CreateName ("T_COLUMN", DbColumn.TagId));
			query.Fields.Add ("C_NAME", SqlField.CreateName ("T_COLUMN", DbColumn.TagName));
			query.Fields.Add ("C_INFO", SqlField.CreateName ("T_COLUMN", DbColumn.TagInfoXml));
			query.Fields.Add ("C_TYPE", SqlField.CreateName ("T_COLUMN", DbColumn.TagRefType));
			
			this.AddLocalisedColumns (query, "COLUMN_CAPTION", "T_COLUMN", DbColumn.TagCaption);
			this.AddLocalisedColumns (query, "COLUMN_DESCRIPTION", "T_COLUMN", DbColumn.TagDescription);
			
			//	Les deux tables utilisées pour l'extraction :
			
			query.Tables.Add ("T_TABLE",  SqlField.CreateName (DbTable.TagTableDef));
			query.Tables.Add ("T_COLUMN", SqlField.CreateName (DbTable.TagColumnDef));
			
			//	On extrait toutes les lignes de T_TABLE qui ont un CR_ID = key, ainsi que
			//	les lignes correspondantes de T_COLUMN qui ont un CREF_TABLE = key.
			
			this.AddKeyExtraction (query, key, "T_TABLE", DbKeyMatchMode.ExactRevisionId);
			this.AddKeyExtraction (query, "T_COLUMN", DbColumn.TagRefTable, key);
			
			System.Data.DataTable data_table = this.ExecuteSqlSelect (query, 1);
			
			if (this.display_data_set != null)
			{
				this.display_data_set (this, string.Format ("DbTable.{0}", key), data_table);
			}
			
			System.Data.DataRow row_zero = data_table.Rows[0];
			
			DbTable db_table = DbTable.NewTable (Converter.ToString (row_zero["T_INFO"]));
			
			db_table.Attributes.SetAttribute (Tags.Name, Converter.ToString (row_zero["T_NAME"]));
			db_table.DefineInternalKey (key);
			
			this.DefineLocalisedAttributes (row_zero, "TABLE_CAPTION", DbColumn.TagCaption, db_table.Attributes, Tags.Caption);
			this.DefineLocalisedAttributes (row_zero, "TABLE_DESCRIPTION", DbColumn.TagDescription, db_table.Attributes, Tags.Description);
			
			foreach (System.Data.DataRow data_row in data_table.Rows)
			{
				long type_ref_id;
				long column_id;
				
				DbColumn db_column = DbColumn.NewColumn (Converter.ToString (data_row["C_INFO"]));
				
				Converter.Convert (data_row["C_ID"], out column_id);
				Converter.Convert (data_row["C_TYPE"], out type_ref_id);
				
				db_column.Attributes.SetAttribute (Tags.Name, Converter.ToString (data_row["C_NAME"]));
				db_column.DefineInternalKey (new DbKey (column_id));
				
				this.DefineLocalisedAttributes (data_row, "COLUMN_CAPTION", DbColumn.TagCaption, db_column.Attributes, Tags.Caption);
				this.DefineLocalisedAttributes (data_row, "COLUMN_DESCRIPTION", DbColumn.TagDescription, db_column.Attributes, Tags.Description);
				
				DbType db_type = this.ResolveDbType (new DbKey (type_ref_id));
				
				if (db_type == null)
				{
					throw new DbException (this.db_access, string.Format ("Missing type for column {0} in table {1}.", db_column.Name, db_table.Name));
				}
				
				db_column.SetType (db_type);
				db_table.Columns.Add (db_column);
			}
			
			return db_table;
		}
		
		public DbType  LoadDbType(DbKey key)
		{
			SqlSelect query = new SqlSelect ();
			
			query.Fields.Add ("T_NAME", SqlField.CreateName ("T_TYPE", DbColumn.TagName));
			query.Fields.Add ("T_INFO", SqlField.CreateName ("T_TYPE", DbColumn.TagInfoXml));
			
			this.AddLocalisedColumns (query, "TYPE_CAPTION", "T_TYPE", DbColumn.TagCaption);
			this.AddLocalisedColumns (query, "TYPE_DESCRIPTION", "T_TYPE", DbColumn.TagDescription);
			
			query.Tables.Add ("T_TYPE", SqlField.CreateName (DbTable.TagTypeDef));
			
			//	Cherche la ligne de la table dont 'CR_ID = key' en ne considérant que
			//	l'ID et dont la révision = 0.
			
			this.AddKeyExtraction (query, key, "T_TYPE", DbKeyMatchMode.LiveId);
			
			System.Data.DataTable data_table = this.ExecuteSqlSelect (query, 1);
			System.Data.DataRow   data_row   = data_table.Rows[0];
			
			string type_name = data_row["T_NAME"] as string;
			string type_info = data_row["T_INFO"] as string;
			
			//	A partir de l'information trouvée dans la base, génère l'objet DbType
			//	qui correspond.
			
			DbType type = DbTypeFactory.NewType (type_info);
			
			type.DefineName (type_name);
			type.DefineInternalKey (key);
			
			this.DefineLocalisedAttributes (data_row, "TYPE_CAPTION", DbColumn.TagCaption, type.Attributes, Tags.Caption);
			this.DefineLocalisedAttributes (data_row, "TYPE_DESCRIPTION", DbColumn.TagDescription, type.Attributes, Tags.Description);
			
			if (type is DbTypeEnum)
			{
				DbTypeEnum type_enum = type as DbTypeEnum;
				DbEnumValue[] values = this.LoadEnumValues (type_enum);
				
				type_enum.Initialise (values);
			}
			
			return type;
		}
		
		
		public long NewRowIdInTable(DbKey key, int num_keys)
		{
			using (System.Data.IDbTransaction transaction = this.db_abstraction.BeginTransaction ())
			{
				long id;
				
				try
				{
					id = this.NewRowIdInTable (transaction, key, num_keys);
				}
				catch
				{
					transaction.Rollback ();
					throw;
				}
				
				transaction.Commit ();
				
				return id;
			}
		}
		
		public long NewRowIdInTable(System.Data.IDbTransaction transaction, DbKey key, int num_keys)
		{
			System.Diagnostics.Debug.Assert (num_keys >= 0);
			
			SqlFieldCollection fields = new SqlFieldCollection ();
			SqlFieldCollection conds  = new SqlFieldCollection ();
			
			SqlField field_next_id  = SqlField.CreateName (DbColumn.TagNextId);
			SqlField field_const_n  = SqlField.CreateConstant (num_keys, DbRawType.Int32);
			
			fields.Add (DbColumn.TagNextId, new SqlFunction (SqlFunctionType.MathAdd, field_next_id, field_const_n));
			
			conds.Add (new SqlFunction (SqlFunctionType.CompareEqual, SqlField.CreateName (DbColumn.TagId), SqlField.CreateConstant (key.Id, DbRawType.Int64)));
			conds.Add (new SqlFunction (SqlFunctionType.CompareEqual, SqlField.CreateName (DbColumn.TagRevision), SqlField.CreateConstant (key.Revision, DbRawType.Int32)));
			
			if (num_keys != 0)
			{
				this.sql_builder.UpdateData (DbTable.TagTableDef, fields, conds);
				
#if false // PA: en attendant le support de UpdateData...
				this.ExecuteSilent ();
#endif
			}
			
			SqlSelect query = new SqlSelect ();
			
			query.Fields.Add (field_next_id);
			query.Tables.Add (SqlField.CreateName (DbTable.TagTableDef));
			query.Conditions.Add (conds[0]);
			query.Conditions.Add (conds[1]);
			
			this.sql_builder.SelectData (query);
			
			long new_row_id;
			
			Converter.Convert (this.ExecuteScalar (transaction), out new_row_id);
			
			return new_row_id - num_keys;
		}
		
		
		protected DbEnumValue[] LoadEnumValues(DbTypeEnum type_enum)
		{
			System.Diagnostics.Debug.Assert (type_enum != null);
			System.Diagnostics.Debug.Assert (type_enum.Count == 0);
			
			SqlSelect query = new SqlSelect ();
			
			query.Fields.Add ("E_ID",   SqlField.CreateName ("T_ENUM", DbColumn.TagId));
			query.Fields.Add ("E_NAME", SqlField.CreateName ("T_ENUM", DbColumn.TagName));
			query.Fields.Add ("E_INFO", SqlField.CreateName ("T_ENUM", DbColumn.TagInfoXml));
			
			this.AddLocalisedColumns (query, "ENUM_CAPTION", "T_ENUM", DbColumn.TagCaption);
			this.AddLocalisedColumns (query, "ENUM_DESCRIPTION", "T_ENUM", DbColumn.TagDescription);
			
			query.Tables.Add ("T_ENUM", SqlField.CreateName (DbTable.TagEnumValDef));
			
			//	Cherche les lignes de la table dont la colonne CREF_TYPE correspond à l'ID du type.
			
			this.AddKeyExtraction (query, "T_ENUM", DbColumn.TagRefType, type_enum.InternalKey);
			
			System.Data.DataTable data_table = this.ExecuteSqlSelect (query, 1);
			
			DbEnumValue[] values = new DbEnumValue[data_table.Rows.Count];
			
			for (int i = 0; i < data_table.Rows.Count; i++)
			{
				System.Data.DataRow data_row = data_table.Rows[i];
				
				//	Pour chaque valeur retournée dans la table, il y a une ligne. Cette ligne
				//	contient toute l'information nécessaire à la création d'une instance de la
				//	class DbEnumValue :
				
				values[i] = new DbEnumValue ();
				
				values[i].Attributes.SetAttribute (Tags.Name,	 Converter.ToString (data_row["E_NAME"]));
				values[i].Attributes.SetAttribute (Tags.InfoXml, Converter.ToString (data_row["E_INFO"]));
				values[i].Attributes.SetAttribute (Tags.Id,		 Converter.ToString (data_row["E_ID"]));
				
				this.DefineLocalisedAttributes (data_row, "ENUM_CAPTION", DbColumn.TagCaption, values[i].Attributes, Tags.Caption);
				this.DefineLocalisedAttributes (data_row, "ENUM_DESCRIPTION", DbColumn.TagDescription, values[i].Attributes, Tags.Description);
			}
			
			return values;
		}
		
		
		protected void DefineLocalisedAttributes(System.Data.DataRow row, string prefix, string column, DbAttributes attributes, string tag)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			buffer.Append ("LOC_");
			buffer.Append (prefix);
			
			int initial_length = buffer.Length;
			
			string value;
			
			for (int i = 0; i < this.localisations.Length; i++)
			{
				string locale = this.localisations[i];
				buffer.Length = initial_length;
				
				if (locale != "")
				{
					buffer.Append ("_");
					buffer.Append (locale);
				}
				
				string index = buffer.ToString ();
				
				if (Converter.Convert (row[index], out value))
				{
					attributes.SetAttribute (tag, value, locale);
				}
			}
		}
		
		protected void AddLocalisedColumns(SqlSelect query, string prefix, string table, string column)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			buffer.Append ("LOC_");
			buffer.Append (prefix);
			
			int initial_length = buffer.Length;
			
			for (int i = 0; i < this.localisations.Length; i++)
			{
				string locale = this.localisations[i];
				buffer.Length = initial_length;
				
				if (locale != "")
				{
					buffer.Append ("_");
					buffer.Append (locale);
				}
				
				string index = buffer.ToString ();
				
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
		
		
		protected void SetCategory(DbColumn[] columns, DbElementCat cat)
		{
			for (int i = 0; i < columns.Length; i++)
			{
				columns[i].DefineCategory (cat);
			}
		}
		
		
		#region Bootstrapping
		protected void BootCreateTableTableDef()
		{
			DbTable    table   = new DbTable (DbTable.TagTableDef);
			DbColumn[] columns = new DbColumn[8];
			
			columns[0] = new DbColumn (DbColumn.TagId,			this.num_type_id);
			columns[1] = new DbColumn (DbColumn.TagRevision,	this.num_type_revision);
			columns[2] = new DbColumn (DbColumn.TagStatus,		this.num_type_status);
			columns[3] = new DbColumn (DbColumn.TagName,		this.str_type_name, Nullable.No);
			columns[4] = new DbColumn (DbColumn.TagCaption,		this.str_type_caption, Nullable.Yes);
			columns[5] = new DbColumn (DbColumn.TagDescription,	this.str_type_description, Nullable.Yes);
			columns[6] = new DbColumn (DbColumn.TagInfoXml,		this.str_type_info_xml, Nullable.No);
			columns[7] = new DbColumn (DbColumn.TagNextId,		this.num_type_id);
			
			columns[0].DefineColumnClass (DbColumnClass.KeyId);
			columns[1].DefineColumnClass (DbColumnClass.KeyRevision);
			columns[4].DefineColumnLocalisation (DbColumnLocalisation.Default);
			columns[5].DefineColumnLocalisation (DbColumnLocalisation.Default);
			
			this.SetCategory (columns, DbElementCat.Internal);
			
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
			
			columns[0].DefineColumnClass (DbColumnClass.KeyId);
			columns[1].DefineColumnClass (DbColumnClass.KeyRevision);
			columns[4].DefineColumnLocalisation (DbColumnLocalisation.Default);
			columns[5].DefineColumnLocalisation (DbColumnLocalisation.Default);
			columns[7].DefineColumnClass (DbColumnClass.Ref);
			columns[8].DefineColumnClass (DbColumnClass.Ref);
			
			this.SetCategory (columns, DbElementCat.Internal);
			
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
			
			columns[0].DefineColumnClass (DbColumnClass.KeyId);
			columns[1].DefineColumnClass (DbColumnClass.KeyRevision);
			columns[4].DefineColumnLocalisation (DbColumnLocalisation.Default);
			columns[5].DefineColumnLocalisation (DbColumnLocalisation.Default);
			
			this.SetCategory (columns, DbElementCat.Internal);
			
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
			
			columns[0].DefineColumnClass (DbColumnClass.KeyId);
			columns[1].DefineColumnClass (DbColumnClass.KeyRevision);
			columns[4].DefineColumnLocalisation (DbColumnLocalisation.Default);
			columns[5].DefineColumnLocalisation (DbColumnLocalisation.Default);
			columns[7].DefineColumnClass (DbColumnClass.Ref);
			
			this.SetCategory (columns, DbElementCat.Internal);
			
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
			
			columns[0].DefineColumnClass (DbColumnClass.KeyId);
			columns[1].DefineColumnClass (DbColumnClass.Ref);
			columns[2].DefineColumnClass (DbColumnClass.Ref);
			columns[3].DefineColumnClass (DbColumnClass.Ref);
			
			this.SetCategory (columns, DbElementCat.Internal);
			
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
			fields.Add (table_def.Columns[DbColumn.TagNextId]	.CreateSqlField (this.type_converter, 0));
			
			this.sql_builder.InsertData (table_def.Name, fields);
			
			this.ExecuteSilent ();
		}
		
		protected void BootUpdateTableNextId(DbKey key, long next_id)
		{
			SqlFieldCollection fields = new SqlFieldCollection ();
			SqlFieldCollection conds  = new SqlFieldCollection ();
			
			fields.Add (DbColumn.TagNextId, SqlField.CreateConstant (next_id, DbRawType.Int64));
			
			conds.Add (new SqlFunction (SqlFunctionType.CompareEqual, SqlField.CreateName (DbColumn.TagId), SqlField.CreateConstant (key.Id, DbRawType.Int64)));
			conds.Add (new SqlFunction (SqlFunctionType.CompareEqual, SqlField.CreateName (DbColumn.TagRevision), SqlField.CreateConstant (key.Revision, DbRawType.Int32)));
			
			System.Diagnostics.Debug.WriteLine (string.Format ("Table {0}, next ID will be {1}.", key, next_id));
			
			this.sql_builder.UpdateData (DbTable.TagTableDef, fields, conds);
			
#if false // PA: en attendant le support de UpdateData...
			this.ExecuteSilent ();
#endif
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
			int type_key_id     = 1;
			int table_key_id    = 1;
			int column_key_id   = 1;
			int ref_key_id      = 1;
			int enum_val_key_id = 1;
			
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
			
			this.BootInsertRefDefRow (ref_key_id++, DbTable.TagColumnDef,  DbColumn.TagRefTable,  DbTable.TagTableDef);
			this.BootInsertRefDefRow (ref_key_id++, DbTable.TagColumnDef,  DbColumn.TagRefType,   DbTable.TagTypeDef);
			this.BootInsertRefDefRow (ref_key_id++, DbTable.TagEnumValDef, DbColumn.TagRefType,   DbTable.TagTypeDef);
			this.BootInsertRefDefRow (ref_key_id++, DbTable.TagRefDef,     DbColumn.TagRefColumn, DbTable.TagColumnDef);
			this.BootInsertRefDefRow (ref_key_id++, DbTable.TagRefDef,     DbColumn.TagRefSource, DbTable.TagTableDef);
			this.BootInsertRefDefRow (ref_key_id++, DbTable.TagRefDef,     DbColumn.TagRefTarget, DbTable.TagTableDef);
			
			this.BootUpdateTableNextId (this.internal_tables[DbTable.TagTableDef].InternalKey, table_key_id);
			this.BootUpdateTableNextId (this.internal_tables[DbTable.TagColumnDef].InternalKey, column_key_id);
			this.BootUpdateTableNextId (this.internal_tables[DbTable.TagTypeDef].InternalKey, type_key_id);
			this.BootUpdateTableNextId (this.internal_tables[DbTable.TagRefDef].InternalKey, ref_key_id);
			this.BootUpdateTableNextId (this.internal_tables[DbTable.TagEnumValDef].InternalKey, enum_val_key_id);
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
		
		CallbackDisplayDataSet			display_data_set;
		string[]						localisations;
		
		Cache.DbTypes					cache_db_types = new Cache.DbTypes ();
		Cache.DbTables					cache_db_tables = new Cache.DbTables ();
	}
}
