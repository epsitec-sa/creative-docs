//	Copyright � 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	using Converter = Epsitec.Common.Types.Converter;
	
	public delegate void CallbackDisplayDataSet(DbInfrastructure infrastructure, string name, System.Data.DataTable table);
	
	/// <summary>
	/// La classe DbInfrastructure offre le support pour l'infrastructure
	/// n�cessaire � toute base de donn�es "Cr�sus" (tables internes, m�ta-
	/// donn�es, etc.)
	/// </summary>
	public class DbInfrastructure : System.IDisposable
	{
		public DbInfrastructure()
		{
			this.localisations = new string[] { "", "FR" };
		}
		
		
		public CallbackDisplayDataSet			DisplayDataSet
		{
			get
			{
				return this.display_data_set;
			}
			set
			{
				this.display_data_set = value;
			}
		}
		
		public ISqlBuilder						SqlBuilder
		{
			get
			{
				return this.sql_builder;
			}
		}
		
		public ISqlEngine						SqlEngine
		{
			get
			{
				return this.sql_engine;
			}
		}
		
		public ITypeConverter					TypeConverter
		{
			get
			{
				return this.type_converter;
			}
		}
		
		public DbTransaction					LiveTransaction
		{
			get
			{
				return this.live_transaction;
			}
		}
		
		
		public void CreateDatabase(DbAccess db_access)
		{
			//	Cr�e une base de donn�es avec les structures de gestion requises par Cr�sus
			//	(tables de description, etc.).
			
			if (this.db_access.IsValid)
			{
				throw new DbException (this.db_access, "A database already exists for this DbInfrastructure.");
			}
			
			this.db_access = db_access;
			this.db_access.Create = true;
			
			this.InitialiseDatabaseAbstraction ();
			
			this.types.RegisterTypes ();
			
			//	La base de donn�es vient d'�tre cr��e. Elle est donc toute vide (aucune
			//	table n'est encore d�finie).
			
			System.Diagnostics.Debug.Assert (this.db_abstraction.UserTableNames.Length == 0);
			
			//	Il faut cr�er les tables internes utilis�es pour la gestion des m�ta-donn�es.
			
			using (DbTransaction transaction = this.BeginTransaction ())
			{
				BootHelper helper = new BootHelper (this, transaction);
				
				helper.CreateTableTableDef ();
				helper.CreateTableColumnDef ();
				helper.CreateTableTypeDef ();
				helper.CreateTableEnumValDef ();
				helper.CreateTableLog ();
				
				//	Valide la cr�ation de toutes ces tables avant de commencer � peupler
				//	les tables. Firebird requiert ce mode de fonctionnement.
				
				transaction.Commit ();
			}
			
			//	Les tables ont toutes �t� cr��es. Il faut maintenant les remplir avec
			//	les informations de d�part.
				
			using (DbTransaction transaction = this.BeginTransaction ())
			{
				this.BootSetupTables (transaction);
				
				transaction.Commit ();
			}
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
			
			using (DbTransaction transaction = this.BeginTransaction ())
			{
				this.internal_tables.Add (this.ResolveDbTable (transaction, Tags.TableLog));
				this.internal_tables.Add (this.ResolveDbTable (transaction, Tags.TableTableDef));
				this.internal_tables.Add (this.ResolveDbTable (transaction, Tags.TableColumnDef));
				this.internal_tables.Add (this.ResolveDbTable (transaction, Tags.TableTypeDef));
				this.internal_tables.Add (this.ResolveDbTable (transaction, Tags.TableEnumValDef));
				
				this.types.ResolveTypes (transaction);
				
				//	TODO: log_id doit �tre initialis� autrement (voir DbLogger)
				
				long log_id = this.NextRowIdInTable (transaction, this.internal_tables[Tags.TableLog].InternalKey);
				
				this.client_id      = 0;
				this.current_log_id = DbID.CreateID (log_id, this.client_id);
				
				transaction.Commit ();
			}
		}
		
		public void ReleaseConnection()
		{
			if (this.live_transaction == null)
			{
				this.db_abstraction.ReleaseConnection ();
			}
			else
			{
				this.release_connection_requested = true;
			}
		}
		
		
		public static DbAccess CreateDbAccess(string name)
		{
			DbAccess db_access = new DbAccess ();
			
			db_access.Provider		= "Firebird";
			db_access.LoginName		= "sysdba";
			db_access.LoginPassword = "masterkey";
			db_access.Database		= name;
			db_access.Server		= "localhost";
			db_access.Create		= false;
			
			return db_access;
		}
		
		
		public DbTransaction BeginTransaction()
		{
			//	D�bute une nouvelle transaction. Ceci n'est possible que si aucune
			//	autre transaction n'est actuellement en cours sur cette connexion.
			
			return new DbTransaction (this.db_abstraction.BeginTransaction (), this);
		}
		
		
		public DbTable   CreateDbTable(string name, DbElementCat category, DbRevisionMode revision_mode)
		{
			//	Cr�e la description d'une table qui ne contient que le strict minimum n�cessaire au fonctionnement
			//	de Cr�sus (tuple pour la clef primaire, statut). Il faudra compl�ter les colonnes en fonction des
			//	besoins, puis appeler la m�thode RegisterNewDbTable.
			
			switch (category)
			{
				case DbElementCat.Internal:
					throw new DbException (this.db_access, string.Format ("User may not create internal table. Table '{0}'.", name));
				
				case DbElementCat.UserDataManaged:
					return this.CreateUserTable(name, revision_mode);
				
				default:
					throw new DbException (this.db_access, string.Format ("Unsupported category {0} specified. Table '{1}'.", category, name));
			}
		}
		
		public void      RegisterNewDbTable(DbTransaction transaction, DbTable table)
		{
			//	Enregistre une nouvelle table dans la base de donn�es. Ceci va attribuer �
			//	la table une clef DbKey et v�rifier qu'il n'y a pas de collision avec une
			//	�ventuelle table d�j� existante. Cela va aussi attribuer des colonnes pour
			//	la nouvelle table.
			
			if (transaction == null)
			{
				using (transaction = this.BeginTransaction ())
				{
					this.RegisterNewDbTable (transaction, table);
					transaction.Commit ();
					return;
				}
			}
			
			this.CheckForRegisteredTypes (transaction, table);
			this.CheckForUnknownTable (transaction, table);
			
			long table_id  = this.NewRowIdInTable (transaction, this.internal_tables[Tags.TableTableDef] .InternalKey, 1);
			long column_id = this.NewRowIdInTable (transaction, this.internal_tables[Tags.TableColumnDef].InternalKey, table.Columns.Count);
			
			//	Cr�e la ligne de description de la table :
			
			table.DefineInternalKey (new DbKey (table_id));
			table.UpdatePrimaryKeyInfo ();
			
			this.BootInsertTableDefRow (transaction, table);
			
			//	Cr�e les lignes de description des colonnes :
			
			for (int i = 0; i < table.Columns.Count; i++)
			{
				table.Columns[i].DefineInternalKey (new DbKey (column_id + i));
				this.BootInsertColumnDefRow (transaction, table, table.Columns[i]);
			}
			
			//	Finalement, il faut cr�er la table elle-m�me :
			
			SqlTable sql_table = table.CreateSqlTable (this.type_converter);
			
			this.sql_builder.InsertTable (sql_table);
			this.ExecuteSilent (transaction);
		}
		
		public void      UnregisterDbTable(DbTransaction transaction, DbTable table)
		{
			//	Supprime la description de la table de la base. Pour des raisons de s�curit�,
			//	la table SQL n'est pas r�ellement supprim�e.
			
			if (transaction == null)
			{
				using (transaction = this.BeginTransaction ())
				{
					this.UnregisterDbTable (transaction, table);
					transaction.Commit ();
					return;
				}
			}
			
			this.CheckForKnownTable (transaction, table);
			
			DbKey old_key = table.InternalKey;
			DbKey new_key = new DbKey (old_key.Id, DbRowStatus.Deleted);
			
			this.UpdateKeyInRow (transaction, Tags.TableTableDef, old_key, new_key);
		}
		
		public DbTable   ResolveDbTable(DbTransaction transaction, string table_name)
		{
			DbKey key = this.FindDbTableKey (transaction, table_name);
			return this.ResolveDbTable (transaction, key);
		}
		
		public DbTable   ResolveDbTable(DbTransaction transaction, DbKey key)
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
					System.Collections.ArrayList tables = this.LoadDbTable (transaction, key);
					
					if (tables.Count > 0)
					{
						table = tables[0] as DbTable;
						
						System.Diagnostics.Debug.WriteLine (string.Format ("Loaded {0} {1} from database.", table.GetType ().Name, table.Name));
						System.Diagnostics.Debug.Assert (tables.Count == 1);
					}
				}
				
				return table;
			}
		}
		
		public DbTable[] FindDbTables(DbTransaction transaction, DbElementCat category)
		{
			//	Liste toutes les tables appartenant � la cat�gorie sp�cifi�e.
			
			System.Collections.ArrayList list = this.LoadDbTable (transaction, null);
			
			if (category != DbElementCat.Any)
			{
				for (int i = 0; i < list.Count; )
				{
					DbTable table = list[i] as DbTable;
					
					if (table.Category != category)
					{
						list.RemoveAt (i);
					}
					else
					{
						i++;
					}
				}
			}
			
			DbTable[] tables = new DbTable[list.Count];
			list.CopyTo (tables, 0);
			
			return tables;
		}
		
		
		public DbColumn   CreateColumn(string column_name, DbType type)
		{
			return new DbColumn (column_name, type, DbColumnClass.Data, DbElementCat.UserDataManaged);
		}
		
		public DbColumn   CreateColumn(string column_name, DbType type, Nullable nullable)
		{
			return new DbColumn (column_name, type, nullable, DbColumnClass.Data, DbElementCat.UserDataManaged);
		}
		
		public DbColumn[] CreateLocalisedColumns(string column_name, DbType type)
		{
			//	TODO: cr�e la ou les colonnes localis�es
			
			//	Note: utilise DbColumnClass.Data et DbElementCat.UserDataManaged pour ces
			//	colonnes, puisqu'elles appartiennent � l'utilisateur.
			
			throw new System.NotImplementedException ("CreateLocalisedColumns not implemented.");
		}
		
		public DbColumn[] CreateRefColumns(string column_name, string parent_table_name)
		{
			return this.CreateRefColumns (column_name, parent_table_name, Nullable.Undefined);
		}
		
		public DbColumn[] CreateRefColumns(string column_name, string parent_table_name, Nullable nullable)
		{
			//	Cr�e la ou les colonnes n�cessaires � la d�finition d'une r�f�rence � une autre
			//	table.
			
			DbType type_id = this.internal_types[Tags.TypeKeyId];
			return new DbColumn[] { DbColumn.CreateRefColumn (column_name, parent_table_name, DbColumnClass.RefId, type_id, nullable) };
		}
		
		
		public void RegisterColumnRelations(DbTransaction transaction, DbTable table)
		{
			if (transaction == null)
			{
				using (transaction = this.BeginTransaction ())
				{
					this.RegisterColumnRelations (transaction, table);
					transaction.Commit ();
					return;
				}
			}
			
			//	Passe en revue toutes les colonnes de type ID qui font r�f�rence � une table
			//	et enregistre l'information dans la table de d�finition des r�f�rences.
			//
			//	Note: il faut que les tables aient �t� enregistr�es aupr�s de Cr�sus pour
			//	que cette m�thode fonctionne (on a besoin des IDs des tables et des colonnes
			//	concern�es).
			
			System.Collections.Hashtable ref_columns = new System.Collections.Hashtable ();
			
			for (int i = 0; i < table.Columns.Count; i++)
			{
				DbColumn column = table.Columns[i];
				
				switch (column.ColumnClass)
				{
					case DbColumnClass.RefId:
						
						string parent_name = column.ParentTableName;
						
						if (parent_name != null)
						{
							DbTable parent_table = this.ResolveDbTable (transaction, parent_name);
							
							if (parent_table == null)
							{
								string message = string.Format ("Table '{0}' referenced from '{1}.{2}' not found in database.", parent_name, table.Name, column.Name);
								throw new DbException (this.db_access, message);
							}
							
							DbKey source_table_key  = table.InternalKey;
							DbKey source_column_key = column.InternalKey;
							DbKey parent_table_key  = parent_table.InternalKey;
							
							if (source_table_key == null)
							{
								string message = string.Format ("Reference of '{0}' from '{1}.{2}' specifies unregistered table '{1}'.", parent_name, table.Name, column.Name);
								throw new DbException (this.db_access, message);
							}
							
							if (source_column_key == null)
							{
								string message = string.Format ("Reference of '{0}' from '{1}.{2}' specifies unregistered column '{2}'.", parent_name, table.Name, column.Name);
								throw new DbException (this.db_access, message);
							}
							
							if (parent_table_key == null)
							{
								string message = string.Format ("Reference of '{0}' from '{1}.{2}' specifies unregistered table '{0}'.", parent_name, table.Name, column.Name);
								throw new DbException (this.db_access, message);
							}
							
							this.UpdateColumnRelation (transaction, source_table_key, source_column_key, parent_table_key);
						}
						break;
					
					default:
						break;
				}
			}
		}
		
		protected void UpdateColumnRelation(DbTransaction transaction, DbKey source_table_key, DbKey source_column_key, DbKey parent_table_key)
		{
			System.Diagnostics.Debug.Assert (transaction != null);
			
			System.Diagnostics.Debug.Assert (source_table_key  != null);
			System.Diagnostics.Debug.Assert (source_column_key != null);
			System.Diagnostics.Debug.Assert (parent_table_key  != null);
			
			Collections.SqlFields fields = new Collections.SqlFields ();
			Collections.SqlFields conds  = new Collections.SqlFields ();
			
			fields.Add (Tags.ColumnRefParent, SqlField.CreateConstant (parent_table_key.Id, DbKey.RawTypeForId));

			this.AddKeyExtraction (conds, source_column_key);
			
			this.sql_builder.UpdateData (Tags.TableColumnDef, fields, conds);
			this.ExecuteSilent (transaction);
		}
		
		
		public DbType    CreateDbType(string name, int length, bool is_fixed)
		{
			DbTypeString type = new DbTypeString (length, is_fixed);
			type.DefineName (name);
			return type;
		}
		
		public DbType    CreateDbType(string name, DbNumDef num_def)
		{
			DbTypeNum type = new DbTypeNum (num_def);
			type.DefineName (name);
			return type;
		}
		
		public DbType    CreateDbType(string name, DbEnumValue[] values)
		{
			DbTypeEnum type = new DbTypeEnum (values);
			type.DefineName (name);
			return type;
		}
		
		public DbType    CreateDbTypeByteArray(string name)
		{
			DbTypeByteArray type = new DbTypeByteArray ();
			type.DefineName (name);
			return type;
		}
		
		public void      RegisterNewDbType(DbTransaction transaction, DbType type)
		{
			//	Enregistre un nouveau type dans la base de donn�es. Ceci va attribuer au
			//	type une clef DbKey et v�rifier qu'il n'y a pas de collision avec un
			//	�ventuel type d�j� existant.
			
			if (transaction == null)
			{
				using (transaction = this.BeginTransaction ())
				{
					this.RegisterNewDbType (transaction, type);
					transaction.Commit ();
					return;
				}
			}
			
			this.CheckForUnknownType (transaction, type);
			
			DbTypeEnum type_enum = type as DbTypeEnum;
			
			long table_id = this.NewRowIdInTable (transaction, this.internal_tables[Tags.TableTypeDef].InternalKey, 1);
			long enum_id  = (type_enum == null) ? 0 : this.NewRowIdInTable (transaction, this.internal_tables[Tags.TableEnumValDef].InternalKey, type_enum.Count);
			
			//	Cr�e la ligne de description du type :
			
			type.DefineInternalKey (new DbKey (table_id));
			
			this.BootInsertTypeDefRow (transaction, type);
			
			if (type_enum != null)
			{
				//	Cr�e les lignes de description des valeurs de l'�num�ration :
				
				DbEnumValue[] enum_values = type_enum.Values;
				
				for (int i = 0; i < enum_values.Length; i++)
				{
					enum_values[i].DefineInternalKey (new DbKey (enum_id + i));
					this.BootInsertEnumValueDefRow (transaction, type, enum_values[i]);
				}
			}
		}
		
		public void      UnregisterDbType(DbTransaction transaction, DbType type)
		{
			//	Supprime la description du type de la base. Pour des raisons de s�curit�,
			//	le type SQL n'est pas r�ellement supprim�.
			
			if (transaction == null)
			{
				using (transaction = this.BeginTransaction ())
				{
					this.UnregisterDbType (transaction, type);
					transaction.Commit ();
					return;
				}
			}
			
			this.CheckForKnownType (transaction, type);
			
			DbKey old_key = type.InternalKey;
			DbKey new_key = new DbKey (old_key.Id, DbRowStatus.Deleted);
			
			this.UpdateKeyInRow (transaction, Tags.TableTypeDef, old_key, new_key);
		}
		
		public DbType    ResolveDbType(DbTransaction transaction, string type_name)
		{
			DbKey key = this.FindDbTypeKey (transaction, type_name);
			return this.ResolveDbType (transaction, key);
		}
		
		public DbType    ResolveDbType(DbTransaction transaction, DbKey key)
		{
			if (key == null)
			{
				return null;
			}

			//	Trouve le type corredpondant � une clef sp�cifique.
			
			lock (this.cache_db_types)
			{
				DbType type = this.cache_db_types[key];
				
				if (type == null)
				{
					System.Collections.ArrayList types = this.LoadDbType (transaction, key);
					
					if (types.Count > 0)
					{
						type = types[0] as DbType;
						
						System.Diagnostics.Debug.WriteLine (string.Format ("Loaded {0} {1} from database.", type.GetType ().Name, type.Name));
						System.Diagnostics.Debug.Assert (types.Count == 1);
						
						this.cache_db_types[key] = type;
					}
				}
				
				return type;
			}
		}
		
		public DbType[]  FindDbTypes(DbTransaction transaction)
		{
			//	Liste tous les types.
			
			System.Collections.ArrayList list = this.LoadDbType (transaction, null);
			
			DbType[] types = new DbType[list.Count];
			list.CopyTo (types, 0);
			
			return types;
		}
		
		
		
		internal DbTable CreateUserTable(string name, DbRevisionMode revision_mode)
		{
			System.Diagnostics.Debug.Assert (revision_mode != DbRevisionMode.Unknown);
			
			DbTable table = new DbTable (name);
			
			DbType type = this.internal_types[Tags.TypeKeyId];
			
			DbColumn col_id   = new DbColumn (Tags.ColumnId,       this.internal_types[Tags.TypeKeyId]);
			DbColumn col_stat = new DbColumn (Tags.ColumnStatus,   this.internal_types[Tags.TypeKeyStatus]);
			
			col_id.DefineCategory (DbElementCat.Internal);
			col_id.DefineColumnClass (DbColumnClass.KeyId);
			
			col_stat.DefineCategory (DbElementCat.Internal);
			col_stat.DefineColumnClass (DbColumnClass.KeyStatus);
			
			table.DefineCategory (DbElementCat.UserDataManaged);
			table.DefineRevisionMode (revision_mode);
			
			table.Columns.Add (col_id);
			table.Columns.Add (col_stat);
			
			table.PrimaryKeys.Add (col_id);
			
			return table;
		}
		
		
		protected void CheckForRegisteredTypes(DbTransaction transaction, DbTable table)
		{
			//	V�rifie que tous les types utilis�s dans la d�finition des colonnes sont bien
			//	connus (on v�rifie qu'ils ont une clef valide).
			
			Collections.DbColumns columns = table.Columns;
			
			for (int i = 0; i < columns.Count; i++)
			{
				DbType type = columns[i].Type;
				
				System.Diagnostics.Debug.Assert (type != null);
				
				if (type.InternalKey == null)
				{
					string message = string.Format ("Unregistered type '{0}' used in table '{1}', column '{2}'.",
						type.Name, table.Name, columns[i].Name);
					
					throw new DbException (this.db_access, message);
				}
			}
		}
		
		protected void CheckForUnknownType(DbTransaction transaction, DbType type)
		{
			System.Diagnostics.Debug.Assert (type != null);
			
			if (this.CountMatchingRows (transaction, Tags.TableTypeDef, Tags.ColumnName, type.Name) > 0)
			{
				string message = string.Format ("Type {0} already exists in database.", type.Name);
				throw new DbException (this.db_access, message);
			}
		}
		
		protected void CheckForKnownType(DbTransaction transaction, DbType type)
		{
			System.Diagnostics.Debug.Assert (type != null);
			
			if (this.CountMatchingRows (transaction, Tags.TableTypeDef, Tags.ColumnName, type.Name) == 0)
			{
				string message = string.Format ("Type {0} does not exist in database.", type.Name);
				throw new DbException (this.db_access, message);
			}
		}
		
		protected void CheckForUnknownTable(DbTransaction transaction, DbTable table)
		{
			//	Cherche si une table avec ce nom existe dans la base. Si c'est le cas,
			//	g�n�re une exception.
			//
			//	NOTE:
			//
			//	On cherche les lignes dans CR_TABLE_DEF dont la colonne CR_NAME contient le nom
			//	sp�cifi� et dont CR_REV = 0. Cette seconde condition est n�cessaire, car une table
			//	d�truite figure encore dans CR_TABLE_DEF avec CR_REV > 0, et elle ne doit pas �tre
			//	compt�e.
			
			if (this.CountMatchingRows (transaction, Tags.TableTableDef, Tags.ColumnName, table.Name) > 0)
			{
				string message = string.Format ("Table {0} already exists in database.", table.Name);
				throw new DbException (this.db_access, message);
			}
		}
		
		protected void CheckForKnownTable(DbTransaction transaction, DbTable table)
		{
			//	Cherche si une table avec ce nom existe dans la base. Si ce n'est pas le cas,
			//	g�n�re une exception.
			//
			//	NOTE:
			//
			//	On cherche les lignes dans CR_TABLE_DEF dont la colonne CR_NAME contient le nom
			//	sp�cifi� et dont CR_REV = 0. Cette seconde condition est n�cessaire, car une table
			//	d�truite figure encore dans CR_TABLE_DEF avec CR_REV > 0, et elle ne doit pas �tre
			//	compt�e.
			
			if (this.CountMatchingRows (transaction, Tags.TableTableDef, Tags.ColumnName, table.Name) == 0)
			{
				string message = string.Format ("Table {0} does not exist in database.", table.Name);
				throw new DbException (this.db_access, message);
			}
		}
		
		
		internal void NotifyBeginTransaction(DbTransaction transaction)
		{
			if (this.live_transaction != null)
			{
				throw new DbException (this.db_access, string.Format ("Nested transactions not supported."));
			}
			
			this.live_transaction = transaction;
		}
		
		internal void NotifyEndTransaction(DbTransaction transaction)
		{
			if (this.live_transaction != transaction)
			{
				throw new DbException (this.db_access, string.Format ("Ending wrong transaction."));
			}
			
			this.live_transaction = null;
			
			if (this.release_connection_requested)
			{
				this.release_connection_requested = false;
				this.ReleaseConnection ();
			}
		}
		
		
		
		public void Execute(DbTransaction transaction, DbRichCommand command)
		{
			if (transaction == null)
			{
				using (transaction = this.BeginTransaction ())
				{
					this.Execute (transaction, command);
					transaction.Commit ();
					return;
				}
			}
			
			this.sql_engine.Execute (this, transaction.Transaction, command);
		}
		
		
		public void					ExecuteSilent(DbTransaction transaction)
		{
			int count = this.sql_builder.CommandCount;
			
			if (count < 1)
			{
				return;
			}
			
			if (transaction == null)
			{
				using (transaction = this.BeginTransaction ())
				{
					this.ExecuteSilent (transaction);
					transaction.Commit ();
					return;
				}
			}
			
			using (System.Data.IDbCommand command = this.sql_builder.CreateCommand (transaction.Transaction))
			{
				this.sql_engine.Execute (command, DbCommandType.Silent, count);
			}
		}
		
		public object				ExecuteScalar(DbTransaction transaction)
		{
			int count = this.sql_builder.CommandCount;
			
			if (count < 1)
			{
				return null;
			}
			
			if (transaction == null)
			{
				using (transaction = this.BeginTransaction ())
				{
					object value = this.ExecuteScalar (transaction);
					transaction.Commit ();
					return value;
				}
			}
			
			using (System.Data.IDbCommand command = this.sql_builder.CreateCommand (transaction.Transaction))
			{
				object data;
				
				this.sql_engine.Execute (command, DbCommandType.ReturningData, count, out data);
				
				return data;
			}
		}
		
		public object				ExecuteNonQuery(DbTransaction transaction)
		{
			int count = this.sql_builder.CommandCount;
			
			if (count < 1)
			{
				return null;
			}
			
			if (transaction == null)
			{
				using (transaction = this.BeginTransaction ())
				{
					object value = this.ExecuteNonQuery (transaction);
					transaction.Commit ();
					return value;
				}
			}
			
			using (System.Data.IDbCommand command = this.sql_builder.CreateCommand (transaction.Transaction))
			{
				object data;
				
				this.sql_engine.Execute (command, DbCommandType.NonQuery, count, out data);
				
				return data;
			}
		}
		
		public System.Data.DataSet	ExecuteRetData(DbTransaction transaction)
		{
			int count = this.sql_builder.CommandCount;
			
			if (count < 1)
			{
				return null;
			}
			
			if (transaction == null)
			{
				using (transaction = this.BeginTransaction ())
				{
					System.Data.DataSet value = this.ExecuteRetData (transaction);
					transaction.Commit ();
					return value;
				}
			}
			
			using (System.Data.IDbCommand command = this.sql_builder.CreateCommand (transaction.Transaction))
			{
				System.Data.DataSet data;
				
				this.sql_engine.Execute (command, DbCommandType.ReturningData, count, out data);
				
				return data;
			}
		}
		
		
		public System.Data.DataTable ExecuteSqlSelect(DbTransaction transaction, SqlSelect query, int min_rows)
		{
			this.sql_builder.SelectData (query);
			
			System.Data.DataSet data_set;
			System.Data.DataTable data_table;
			
			data_set = this.ExecuteRetData (transaction);
			
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
		
		
		public DbKey FindDbTableKey(DbTransaction transaction, string name)
		{
			return this.FindLiveKey (this.FindDbKeys (transaction, Tags.TableTableDef, name));
		}
		
		public DbKey FindDbTypeKey(DbTransaction transaction, string name)
		{
			return this.FindLiveKey (this.FindDbKeys (transaction, Tags.TableTypeDef, name));
		}
		
		
		internal DbKey FindLiveKey(DbKey[] keys)
		{
			for (int i = 0; i < keys.Length; i++)
			{
				switch (keys[i].Status)
				{
					case DbRowStatus.Live:
					case DbRowStatus.Copied:
						return keys[i];
				}
			}
			
			return null;
		}
		
		internal DbKey[] FindDbKeys(DbTransaction transaction, string table_name, string row_name)
		{
			//	Trouve la (ou les) clefs.
			
			SqlSelect query = new SqlSelect ();
			
			query.Fields.Add ("T_ID",   SqlField.CreateName ("T", Tags.ColumnId));
			query.Fields.Add ("T_STAT",	SqlField.CreateName ("T", Tags.ColumnStatus));
			
			query.Tables.Add ("T", SqlField.CreateName (table_name));
			
			query.Conditions.Add (new SqlFunction (SqlFunctionType.CompareEqual, SqlField.CreateName ("T", Tags.ColumnName), SqlField.CreateConstant (row_name, DbRawType.String)));
			
			System.Data.DataTable data_table = this.ExecuteSqlSelect (transaction, query, 0);
			
			if (this.display_data_set != null)
			{
				this.display_data_set (this, table_name, data_table);
			}
			
			DbKey[] keys = new DbKey[data_table.Rows.Count];
			
			for (int i = 0; i < data_table.Rows.Count; i++)
			{
				System.Data.DataRow row = data_table.Rows[i];
				
				long id;
				int  status;
				
				Converter.Convert (row["T_ID"],   out id);
				Converter.Convert (row["T_STAT"], out status);
				
				keys[i] = new DbKey (id, DbKey.ConvertFromIntStatus (status));
			}
			
			return keys;
		}
		
		
		public int CountMatchingRows(DbTransaction transaction, string table, string name_column, string value)
		{
			//	Compte combien de lignes dans la table ont le texte sp�cifi� dans la colonne sp�cifi�e.
			//	Ne consid�re que les lignes actives.
			
			SqlSelect query = new SqlSelect ();
			
			query.Fields.Add ("N", new SqlAggregate (SqlAggregateType.Count, SqlField.CreateAll ()));
			query.Tables.Add ("T", SqlField.CreateName (table));
			
			query.Conditions.Add (new SqlFunction (SqlFunctionType.CompareEqual, SqlField.CreateName ("T", name_column), SqlField.CreateConstant (value, DbRawType.String)));

			this.AddKeyExtraction (query, "T", DbRowSearchMode.LiveActive);
			
			this.sql_builder.SelectData (query);
			
			int count;
			
			Converter.Convert (this.ExecuteScalar (transaction), out count);
			
			return count;
		}
		
		
		public void UpdateKeyInRow(DbTransaction transaction, string table, DbKey old_key, DbKey new_key)
		{
			//	Met � jour la clef de la ligne sp�cifi�e.
			
			Collections.SqlFields fields = new Collections.SqlFields ();
			Collections.SqlFields conds  = new Collections.SqlFields ();
			
			fields.Add (Tags.ColumnId,     SqlField.CreateConstant (new_key.Id,        DbKey.RawTypeForId));
			fields.Add (Tags.ColumnStatus, SqlField.CreateConstant (new_key.IntStatus, DbKey.RawTypeForStatus));

			this.AddKeyExtraction (conds, old_key);
			
			this.sql_builder.UpdateData (table, fields, conds);
			
			int num_rows_affected;
			
			Converter.Convert (this.ExecuteNonQuery (transaction), out num_rows_affected);
			
			if (num_rows_affected != 1)
			{
				throw new DbException (this.db_access, string.Format ("Update of row {0} in table {1} produced {2} updates.", old_key, table, num_rows_affected));
			}
		}
		
		
		public System.Collections.ArrayList LoadDbTable(DbTransaction transaction, DbKey key)
		{
			//	Charge les d�finitions pour la table au moyen d'une requ�te unique qui va
			//	aussi retourner les diverses d�finitions de colonnes.
			
			SqlSelect query = new SqlSelect ();
			
			//	Ce qui est propre � la table :
			
			query.Fields.Add ("T_ID",   SqlField.CreateName ("T_TABLE", Tags.ColumnId));
			query.Fields.Add ("T_NAME", SqlField.CreateName ("T_TABLE", Tags.ColumnName));
			query.Fields.Add ("T_INFO", SqlField.CreateName ("T_TABLE", Tags.ColumnInfoXml));
			
			this.AddLocalisedColumns (query, "TABLE_CAPTION", "T_TABLE", Tags.ColumnCaption);
			this.AddLocalisedColumns (query, "TABLE_DESCRIPTION", "T_TABLE", Tags.ColumnDescription);
			
			//	Ce qui est propre aux colonnes :
			
			query.Fields.Add ("C_ID",     SqlField.CreateName ("T_COLUMN", Tags.ColumnId));
			query.Fields.Add ("C_NAME",   SqlField.CreateName ("T_COLUMN", Tags.ColumnName));
			query.Fields.Add ("C_INFO",   SqlField.CreateName ("T_COLUMN", Tags.ColumnInfoXml));
			query.Fields.Add ("C_TYPE",   SqlField.CreateName ("T_COLUMN", Tags.ColumnRefType));
			query.Fields.Add ("C_PARENT", SqlField.CreateName ("T_COLUMN", Tags.ColumnRefParent));
			
			this.AddLocalisedColumns (query, "COLUMN_CAPTION", "T_COLUMN", Tags.ColumnCaption);
			this.AddLocalisedColumns (query, "COLUMN_DESCRIPTION", "T_COLUMN", Tags.ColumnDescription);
			
			//	Les deux tables utilis�es pour l'extraction :
			
			query.Tables.Add ("T_TABLE",  SqlField.CreateName (Tags.TableTableDef));
			query.Tables.Add ("T_COLUMN", SqlField.CreateName (Tags.TableColumnDef));
			
			if (key == null)
			{
				//	On extrait toutes les d�finitions de tables qui correspondent � une version
				//	'active' (ignore les versions archiv�es et d�truites). Extrait aussi les colonnes
				//	correspondantes.
				
				this.AddKeyExtraction (query, "T_TABLE", DbRowSearchMode.LiveActive);
				this.AddKeyExtraction (query, "T_COLUMN", Tags.ColumnRefTable, "T_TABLE");
			}
			else
			{
				//	On extrait toutes les lignes de T_TABLE qui ont un CR_ID = key, ainsi que
				//	les lignes correspondantes de T_COLUMN qui ont un CREF_TABLE = key.
				
				this.AddKeyExtraction (query, "T_TABLE", key);
				this.AddKeyExtraction (query, "T_COLUMN", Tags.ColumnRefTable, key);
			}
			
			System.Data.DataTable data_table = this.ExecuteSqlSelect (transaction, query, 1);
			
			if (this.display_data_set != null)
			{
				this.display_data_set (this, string.Format ("DbTable.{0}", key), data_table);
			}
			
			System.Data.DataRowCollection rows     = data_table.Rows;
			long                          row_id   = -1;
			System.Collections.ArrayList  tables   = new System.Collections.ArrayList ();
			DbTable						  db_table = null;
			bool                          recycle  = false;
			
			
			//	Analyse toutes les lignes retourn�es. On suppose que les lignes sont group�es
			//	logiquement par tables.
			
			for (int i = 0; i < rows.Count; i++)
			{
				long current_row_id;
				System.Data.DataRow data_row = rows[i];
				
				Converter.Convert (data_row["T_ID"], out current_row_id);
				
				if (row_id != current_row_id)
				{
					row_id   = current_row_id;
					db_table = null;
					
					string table_info = Converter.ToString (data_row["T_INFO"]);
					DbKey  table_key  = (key == null) ? new DbKey (row_id) : key;
					
					db_table = this.cache_db_tables[table_key];
					
					if (db_table == null)
					{
						db_table = DbTable.CreateTable (table_info);
						recycle  = false;
						
						db_table.Attributes.SetAttribute (Tags.Name, Converter.ToString (data_row["T_NAME"]));
						db_table.DefineInternalKey (table_key);
						
						this.DefineLocalisedAttributes (data_row, "TABLE_CAPTION", Tags.ColumnCaption, db_table.Attributes, Tags.Caption);
						this.DefineLocalisedAttributes (data_row, "TABLE_DESCRIPTION", Tags.ColumnDescription, db_table.Attributes, Tags.Description);
						
						//	Afin d'�viter de recharger cette table plus tard, on va en prendre note tout de suite; �a permet
						//	aussi d'�viter des boucles sans fin dans le cas de tables qui ont des r�f�rences circulaires, car
						//	la prochaine recherche avec ResolveDbTable s'appliquant � cette table se terminera avec succ�s.
						
						this.cache_db_tables[table_key] = db_table;
					}
					else
					{
						System.Diagnostics.Debug.WriteLine (string.Format ("Recycling known table {0}.", db_table.Name));
						recycle = true;
					}
					
					tables.Add (db_table);
				}
				
				if (recycle)
				{
					continue;
				}
				
				//	Chaque ligne contient une d�finition de colonne.
				
				long type_ref_id;
				long parent_table_ref_id;
				long column_id;
				
				DbColumn db_column = DbColumn.CreateColumn (Converter.ToString (data_row["C_INFO"]));
				
				Converter.Convert (data_row["C_ID"], out column_id);
				Converter.Convert (data_row["C_TYPE"], out type_ref_id);
				
				bool has_parent_table = Converter.Convert (data_row["C_PARENT"], out parent_table_ref_id);
				
				db_column.Attributes.SetAttribute (Tags.Name, Converter.ToString (data_row["C_NAME"]));
				db_column.DefineInternalKey (new DbKey (column_id));
				
				this.DefineLocalisedAttributes (data_row, "COLUMN_CAPTION", Tags.ColumnCaption, db_column.Attributes, Tags.Caption);
				this.DefineLocalisedAttributes (data_row, "COLUMN_DESCRIPTION", Tags.ColumnDescription, db_column.Attributes, Tags.Description);
				
				if (has_parent_table)
				{
					DbKey   parent_key   = new DbKey (parent_table_ref_id);
					DbTable parent_table = this.ResolveDbTable (transaction, parent_key);
					
					System.Diagnostics.Debug.WriteLine (string.Format ("Column {0}.{1} ({4}) refers to table {3} (ID {2}).", db_table.Name, db_column.Name, parent_key.Id, parent_table.Name, db_column.ColumnClass));
					
					db_column.DefineParentTableName (parent_table.Name);
				}
				
				DbType db_type = this.ResolveDbType (transaction, new DbKey (type_ref_id));
				
				if (db_type == null)
				{
					throw new DbException (this.db_access, string.Format ("Missing type for column {0} in table {1}.", db_column.Name, db_table.Name));
				}
				
				db_column.SetType (db_type);
				db_table.Columns.Add (db_column);
				
				if (db_column.IsPrimaryKey)
				{
					db_table.PrimaryKeys.Add (db_column);
				}
			}
			
			//	TODO: il faut encore initialiser les champs ParentTableName des diverses colonnes
			//	qui �tablissent une relation avec une autre table. Pour cela, il faudra faire un
			//	SELECT dans Tags.TableRelationDef pour les colonnes dont DbColumnClass est parmi
			//	RefSimpleId/RefLiveId/RefTupleId/RefTupleRevision et d�terminer le nom des tables
			//	cibles, puis appeler DbColumn.DefineParentTableName...
			
			return tables;
		}
		
		public System.Collections.ArrayList LoadDbType(DbTransaction transaction, DbKey key)
		{
			SqlSelect query = new SqlSelect ();
			
			query.Fields.Add ("T_ID",   SqlField.CreateName ("T_TYPE", Tags.ColumnId));
			query.Fields.Add ("T_NAME", SqlField.CreateName ("T_TYPE", Tags.ColumnName));
			query.Fields.Add ("T_INFO", SqlField.CreateName ("T_TYPE", Tags.ColumnInfoXml));
			
			this.AddLocalisedColumns (query, "TYPE_CAPTION", "T_TYPE", Tags.ColumnCaption);
			this.AddLocalisedColumns (query, "TYPE_DESCRIPTION", "T_TYPE", Tags.ColumnDescription);
			
			query.Tables.Add ("T_TYPE", SqlField.CreateName (Tags.TableTypeDef));
			
			if (key == null)
			{
				//	On extrait toutes les d�finitions de types qui correspondent � la version
				//	'active'.
				
				this.AddKeyExtraction (query, "T_TYPE", DbRowSearchMode.LiveActive);
			}
			else
			{
				//	Cherche la ligne de la table dont 'CR_ID = key'.
				
				this.AddKeyExtraction (query, "T_TYPE", key);
			}
			
			System.Data.DataTable        data_table = this.ExecuteSqlSelect (transaction, query, 1);
			System.Collections.ArrayList types      = new System.Collections.ArrayList ();
			
			foreach (System.Data.DataRow data_row in data_table.Rows)
			{
				long type_id;
				
				Converter.Convert (data_row["T_ID"], out type_id);
				
				string type_name = data_row["T_NAME"] as string;
				string type_info = data_row["T_INFO"] as string;
				
				//	A partir de l'information trouv�e dans la base, g�n�re l'objet DbType
				//	qui correspond.
				
				DbType type = DbTypeFactory.CreateType (type_info);
				
				type.DefineName (type_name);
				type.DefineInternalKey (new DbKey (type_id));
				
				this.DefineLocalisedAttributes (data_row, "TYPE_CAPTION", Tags.ColumnCaption, type.Attributes, Tags.Caption);
				this.DefineLocalisedAttributes (data_row, "TYPE_DESCRIPTION", Tags.ColumnDescription, type.Attributes, Tags.Description);
				
				if (type is DbTypeEnum)
				{
					DbTypeEnum type_enum = type as DbTypeEnum;
					DbEnumValue[] values = this.LoadEnumValues (transaction, type_enum);
					
					type_enum.DefineValues (values);
				}
				
				types.Add (type);
			}
			
			return types;
		}
		
		
		public long NextRowIdInTable(DbTransaction transaction, DbKey key)
		{
			return this.NewRowIdInTable (transaction, key, 0);
		}
		
		public long NewRowIdInTable(DbTransaction transaction, DbKey key, int num_keys)
		{
			//	Attribue 'num_keys' clef cons�cutives dans la table sp�cifi�e.
			
			System.Diagnostics.Debug.Assert (num_keys >= 0);
			
			if (transaction == null)
			{
				using (transaction = this.BeginTransaction ())
				{
					long id = this.NewRowIdInTable (transaction, key, num_keys);
					transaction.Commit ();
					return id;
				}
			}
			
			Collections.SqlFields fields = new Collections.SqlFields ();
			Collections.SqlFields conds  = new Collections.SqlFields ();
			
			SqlField field_next_id  = SqlField.CreateName (Tags.ColumnNextId);
			SqlField field_const_n  = SqlField.CreateConstant (num_keys, DbRawType.Int32);
			
			fields.Add (Tags.ColumnNextId, new SqlFunction (SqlFunctionType.MathAdd, field_next_id, field_const_n));
			
			this.AddKeyExtraction (conds, key);
			
			if (num_keys != 0)
			{
				this.sql_builder.UpdateData (Tags.TableTableDef, fields, conds);
				this.ExecuteSilent (transaction);
			}
			
			SqlSelect query = new SqlSelect ();

			System.Diagnostics.Debug.Assert (conds.Count == 1);

			query.Fields.Add (field_next_id);
			query.Tables.Add (SqlField.CreateName (Tags.TableTableDef));
			query.Conditions.Add (conds[0]);
			
			this.sql_builder.SelectData (query);
			
			long new_row_id;
			
			Converter.Convert (this.ExecuteScalar (transaction), out new_row_id);
			
			return new_row_id - num_keys;
		}
		
		
		protected DbEnumValue[] LoadEnumValues(DbTransaction transaction, DbTypeEnum type_enum)
		{
			System.Diagnostics.Debug.Assert (type_enum != null);
			System.Diagnostics.Debug.Assert (type_enum.Count == 0);
			
			SqlSelect query = new SqlSelect ();
			
			query.Fields.Add ("E_ID",   SqlField.CreateName ("T_ENUM", Tags.ColumnId));
			query.Fields.Add ("E_NAME", SqlField.CreateName ("T_ENUM", Tags.ColumnName));
			query.Fields.Add ("E_INFO", SqlField.CreateName ("T_ENUM", Tags.ColumnInfoXml));
			
			this.AddLocalisedColumns (query, "ENUM_CAPTION", "T_ENUM", Tags.ColumnCaption);
			this.AddLocalisedColumns (query, "ENUM_DESCRIPTION", "T_ENUM", Tags.ColumnDescription);
			
			query.Tables.Add ("T_ENUM", SqlField.CreateName (Tags.TableEnumValDef));
			
			//	Cherche les lignes de la table dont la colonne CREF_TYPE correspond � l'ID du type.
			
			this.AddKeyExtraction (query, "T_ENUM", Tags.ColumnRefType, type_enum.InternalKey);
			
			System.Data.DataTable data_table = this.ExecuteSqlSelect (transaction, query, 1);
			
			DbEnumValue[] values = new DbEnumValue[data_table.Rows.Count];
			
			for (int i = 0; i < data_table.Rows.Count; i++)
			{
				System.Data.DataRow data_row = data_table.Rows[i];
				
				//	Pour chaque valeur retourn�e dans la table, il y a une ligne. Cette ligne
				//	contient toute l'information n�cessaire � la cr�ation d'une instance de la
				//	class DbEnumValue :
				
				string val_name = Converter.ToString (data_row["E_NAME"]);
				string val_id   = Converter.ToString (data_row["E_ID"]);
				string val_info = Converter.ToString (data_row["E_INFO"]);
				
				System.Xml.XmlDocument xml = new System.Xml.XmlDocument ();
				xml.LoadXml (val_info);
				
				values[i] = new DbEnumValue (xml.DocumentElement);
				
				values[i].Attributes.SetAttribute (Tags.Name, val_name);
				values[i].Attributes.SetAttribute (Tags.Id, val_id);
				
				this.DefineLocalisedAttributes (data_row, "ENUM_CAPTION", Tags.ColumnCaption, values[i].Attributes, Tags.Caption);
				this.DefineLocalisedAttributes (data_row, "ENUM_DESCRIPTION", Tags.ColumnDescription, values[i].Attributes, Tags.Description);
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
				
				if (locale.Length > 0)
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
				
				if (locale.Length > 0)
				{
					buffer.Append ("_");
					buffer.Append (locale);
				}
				
				string index = buffer.ToString ();
				
				//	TODO: adapte le nom de la colonne en fonction de la localisation
				
				query.Fields.Add (index, SqlField.CreateName (table, column));
			}
		}

		protected void AddKeyExtraction(Collections.SqlFields conditions, DbKey key)
		{
			SqlField name_col_id = SqlField.CreateName (Tags.ColumnId);
			SqlField constant_id = SqlField.CreateConstant (key.Id, DbKey.RawTypeForId);
			
			conditions.Add (new SqlFunction (SqlFunctionType.CompareEqual, name_col_id, constant_id));
		}

		protected void AddKeyExtraction(SqlSelect query, string table_name, DbKey key)
		{
			SqlField name_col_id = SqlField.CreateName (table_name, Tags.ColumnId);
			SqlField constant_id = SqlField.CreateConstant (key.Id, DbKey.RawTypeForId);
			
			query.Conditions.Add (new SqlFunction (SqlFunctionType.CompareEqual, name_col_id, constant_id));
		}
		
		protected void AddKeyExtraction(SqlSelect query, string source_table_name, string source_col_id, string parent_table_name)
		{
			SqlField parent_col = SqlField.CreateName (parent_table_name, Tags.ColumnId);
			SqlField source_col = SqlField.CreateName (source_table_name, source_col_id);
			
			query.Conditions.Add (new SqlFunction (SqlFunctionType.CompareEqual, source_col, parent_col));
		}
		
		protected void AddKeyExtraction(SqlSelect query, string source_table_name, string source_col_id, DbKey key)
		{
			SqlField source_col  = SqlField.CreateName (source_table_name, source_col_id);
			SqlField constant_id = SqlField.CreateConstant (key.Id, DbKey.RawTypeForId);
			
			query.Conditions.Add (new SqlFunction (SqlFunctionType.CompareEqual, source_col, constant_id));
		}

		protected void AddKeyExtraction(SqlSelect query, string table_name, DbRowSearchMode search_mode)
		{
			SqlFunctionType function;
			DbRowStatus status;
			
			switch (search_mode)
			{
				case DbRowSearchMode.Copied:
					status = DbRowStatus.Copied;
					function = SqlFunctionType.CompareEqual;
					break;
				
				case DbRowSearchMode.Live:
					status = DbRowStatus.Live;
					function = SqlFunctionType.CompareEqual;
					break;
				
				case DbRowSearchMode.LiveActive:
					status = DbRowStatus.ArchiveCopy;
					function = SqlFunctionType.CompareLessThan;
					break;
				
				case DbRowSearchMode.All:
					return;
				
				case DbRowSearchMode.ArchiveCopy:
					status = DbRowStatus.ArchiveCopy;
					function = SqlFunctionType.CompareEqual;
					break;

				case DbRowSearchMode.LiveAll:
					status = DbRowStatus.Deleted;
					function = SqlFunctionType.CompareLessThan;
					break;

				case DbRowSearchMode.Deleted:
					status = DbRowStatus.Deleted;
					function = SqlFunctionType.CompareEqual;
					break;
				default:
					throw new System.ArgumentException (string.Format ("Search mode {0} not supported.", search_mode), "search_mode");
			}

			SqlField name_status = SqlField.CreateName (table_name, Tags.ColumnStatus);
			SqlField const_status = SqlField.CreateConstant (DbKey.ConvertToIntStatus (status), DbKey.RawTypeForStatus);

			query.Conditions.Add (new SqlFunction (function, name_status, const_status));
		}


		protected static void SetCategory(DbColumn[] columns, DbElementCat cat)
		{
			for (int i = 0; i < columns.Length; i++)
			{
				columns[i].DefineCategory (cat);
			}
		}
		
		
		public class BootHelper
		{
			public BootHelper(DbInfrastructure infrastructure, DbTransaction transaction)
			{
				this.infrastructure = infrastructure;
				this.transaction    = transaction;
			}
			
			
			public void CreateTableTableDef()
			{
				DbTypeCache types   = this.infrastructure.types;
				DbTable     table   = new DbTable (Tags.TableTableDef);
				DbColumn[]  columns = new DbColumn[]
					{
						new DbColumn (Tags.ColumnId,		  types.KeyId,		 Nullable.No,  DbColumnClass.KeyId),
						new DbColumn (Tags.ColumnStatus,	  types.KeyStatus,	 Nullable.No,  DbColumnClass.KeyStatus),
						new DbColumn (Tags.ColumnRefLog,	  types.KeyId,		 Nullable.No,  DbColumnClass.RefId),
						new DbColumn (Tags.ColumnName,		  types.Name,		 Nullable.No,  DbColumnClass.Data),
						new DbColumn (Tags.ColumnCaption,	  types.Caption,	 Nullable.Yes, DbColumnClass.Data, DbColumnLocalisation.Default),
						new DbColumn (Tags.ColumnDescription, types.Description, Nullable.Yes, DbColumnClass.Data, DbColumnLocalisation.Default),
						new DbColumn (Tags.ColumnInfoXml,	  types.InfoXml,	 Nullable.No,  DbColumnClass.Data),
						new DbColumn (Tags.ColumnNextId,	  types.KeyId,		 Nullable.No,  DbColumnClass.RefId)
					};
				
				this.CreateTable (table, columns);
			}
			
			public void CreateTableColumnDef()
			{
				DbTypeCache types   = this.infrastructure.types;
				DbTable     table   = new DbTable (Tags.TableColumnDef);
				DbColumn[]  columns = new DbColumn[]
					{
						new DbColumn (Tags.ColumnId,		  types.KeyId,		 Nullable.No,  DbColumnClass.KeyId),
						new DbColumn (Tags.ColumnStatus,	  types.KeyStatus,	 Nullable.No,  DbColumnClass.KeyStatus),
						new DbColumn (Tags.ColumnRefLog,	  types.KeyId,		 Nullable.No,  DbColumnClass.RefId),
						new DbColumn (Tags.ColumnName,		  types.Name,		 Nullable.No,  DbColumnClass.Data),
						new DbColumn (Tags.ColumnCaption,	  types.Caption,	 Nullable.Yes, DbColumnClass.Data, DbColumnLocalisation.Default),
						new DbColumn (Tags.ColumnDescription, types.Description, Nullable.Yes, DbColumnClass.Data, DbColumnLocalisation.Default),
						new DbColumn (Tags.ColumnInfoXml,	  types.InfoXml,	 Nullable.No,  DbColumnClass.Data),
						new DbColumn (Tags.ColumnRefTable,	  types.KeyId,       Nullable.No,  DbColumnClass.RefId),
						new DbColumn (Tags.ColumnRefType,	  types.KeyId,       Nullable.No,  DbColumnClass.RefId),
						new DbColumn (Tags.ColumnRefParent,	  types.KeyId,       Nullable.Yes, DbColumnClass.RefId)
					};
				
				this.CreateTable (table, columns);
			}
			
			public void CreateTableTypeDef()
			{
				DbTypeCache types   = this.infrastructure.types;
				DbTable     table   = new DbTable (Tags.TableTypeDef);
				DbColumn[]  columns = new DbColumn[]
					{
						new DbColumn (Tags.ColumnId,		  types.KeyId,		 Nullable.No,  DbColumnClass.KeyId),
						new DbColumn (Tags.ColumnStatus,	  types.KeyStatus,	 Nullable.No,  DbColumnClass.KeyStatus),
						new DbColumn (Tags.ColumnRefLog,	  types.KeyId,		 Nullable.No,  DbColumnClass.RefId),
						new DbColumn (Tags.ColumnName,		  types.Name,		 Nullable.No,  DbColumnClass.Data),
						new DbColumn (Tags.ColumnCaption,	  types.Caption,	 Nullable.Yes, DbColumnClass.Data, DbColumnLocalisation.Default),
						new DbColumn (Tags.ColumnDescription, types.Description, Nullable.Yes, DbColumnClass.Data, DbColumnLocalisation.Default),
						new DbColumn (Tags.ColumnInfoXml,	  types.InfoXml,	 Nullable.No,  DbColumnClass.Data)
					};
				
				this.CreateTable (table, columns);
			}
			
			public void CreateTableEnumValDef()
			{
				DbTypeCache types   = this.infrastructure.types;
				DbTable     table   = new DbTable (Tags.TableEnumValDef);
				DbColumn[]  columns = new DbColumn[]
					{
						new DbColumn (Tags.ColumnId,		  types.KeyId,		 Nullable.No,  DbColumnClass.KeyId),
						new DbColumn (Tags.ColumnStatus,	  types.KeyStatus,	 Nullable.No,  DbColumnClass.KeyStatus),
						new DbColumn (Tags.ColumnRefLog,	  types.KeyId,		 Nullable.No,  DbColumnClass.RefId),
						new DbColumn (Tags.ColumnName,		  types.Name,		 Nullable.No,  DbColumnClass.Data),
						new DbColumn (Tags.ColumnCaption,	  types.Caption,	 Nullable.Yes, DbColumnClass.Data, DbColumnLocalisation.Default),
						new DbColumn (Tags.ColumnDescription, types.Description, Nullable.Yes, DbColumnClass.Data, DbColumnLocalisation.Default),
						new DbColumn (Tags.ColumnInfoXml,	  types.InfoXml,	 Nullable.No,  DbColumnClass.Data),
						new DbColumn (Tags.ColumnRefType,	  types.KeyId,       Nullable.No,  DbColumnClass.RefId)
					};
				
				this.CreateTable (table, columns);
			}
			
			public void CreateTableLog()
			{
				DbTypeCache types   = this.infrastructure.types;
				DbTable     table   = new DbTable (Tags.TableLog);
				DbColumn[]  columns = new DbColumn[]
					{
						new DbColumn (Tags.ColumnId,		  types.KeyId,		 Nullable.No,  DbColumnClass.KeyId),
						new DbColumn (Tags.ColumnDateTime,	  types.DateTime,	 Nullable.No,  DbColumnClass.Data)
					};
				
				//	TODO: ajouter ici une colonne d�finissant la nature du changement (et l'utilisateur
				//	qui en est la cause).
				
				this.CreateTable (table, columns);
			}
			
			
			private void CreateTable(DbTable table, DbColumn[] columns)
			{
				DbInfrastructure.SetCategory (columns, DbElementCat.Internal);
				
				table.Columns.AddRange (columns);
				
				table.DefineCategory (DbElementCat.Internal);
				table.DefinePrimaryKey (columns[0]);
				
				this.infrastructure.internal_tables.Add (table);
				
				SqlTable sql_table = table.CreateSqlTable (this.infrastructure.type_converter);
				this.infrastructure.sql_builder.InsertTable (sql_table);
				this.infrastructure.ExecuteSilent (this.transaction);
			}
		
			
			private DbInfrastructure			infrastructure;
			private DbTransaction				transaction;
		}
		
		
		
		
		
		protected void BootInsertLogRow(DbTransaction transaction, DbID id, long date_time)
		{
			DbTable log_table = this.internal_tables[Tags.TableLog];
			
			//	Phase d'initialisation de la base : ins�re une ligne dans la table de logging.
			
			Collections.SqlFields fields = new Collections.SqlFields ();
			
			fields.Add (log_table.Columns[Tags.ColumnId]      .CreateSqlField (this.type_converter, id));
			fields.Add (log_table.Columns[Tags.ColumnDateTime].CreateSqlField (this.type_converter, date_time));
			
			this.sql_builder.InsertData (log_table.CreateSqlName (), fields);
			this.ExecuteSilent (transaction);
		}
		
		protected void BootInsertTypeDefRow(DbTransaction transaction, DbType type)
		{
			System.Diagnostics.Debug.Assert (this.current_log_id.LocalID > 0);
			
			DbTable type_def = this.internal_tables[Tags.TableTypeDef];
			
			//	Phase d'initialisation de la base : ins�re une ligne dans la table de d�finition des
			//	types. Les colonnes descriptives (pour l'utilisateur) ne sont pas initialis�es.
			
			Collections.SqlFields fields = new Collections.SqlFields ();
			
			fields.Add (type_def.Columns[Tags.ColumnId]      .CreateSqlField (this.type_converter, type.InternalKey.Id));
			fields.Add (type_def.Columns[Tags.ColumnStatus]  .CreateSqlField (this.type_converter, type.InternalKey.IntStatus));
			fields.Add (type_def.Columns[Tags.ColumnRefLog]  .CreateSqlField (this.type_converter, this.current_log_id));
			fields.Add (type_def.Columns[Tags.ColumnName]    .CreateSqlField (this.type_converter, type.Name));
			fields.Add (type_def.Columns[Tags.ColumnInfoXml] .CreateSqlField (this.type_converter, DbTypeFactory.SerializeToXml (type, false)));
			
			this.sql_builder.InsertData (type_def.CreateSqlName (), fields);
			this.ExecuteSilent (transaction);
		}
		
		protected void BootInsertEnumValueDefRow(DbTransaction transaction, DbType type, DbEnumValue value)
		{
			System.Diagnostics.Debug.Assert (this.current_log_id.LocalID > 0);
			
			DbTable enum_def = this.internal_tables[Tags.TableEnumValDef];
			
			//	Phase d'initialisation de la base : ins�re une ligne dans la table de d�finition des
			//	�num�rations. Les colonnes descriptives (pour l'utilisateur) ne sont pas initialis�es.
			
			Collections.SqlFields fields = new Collections.SqlFields ();
			
			fields.Add (enum_def.Columns[Tags.ColumnId]	     .CreateSqlField (this.type_converter, value.InternalKey.Id));
			fields.Add (enum_def.Columns[Tags.ColumnStatus]  .CreateSqlField (this.type_converter, value.InternalKey.IntStatus));
			fields.Add (enum_def.Columns[Tags.ColumnRefLog]  .CreateSqlField (this.type_converter, this.current_log_id));
			fields.Add (enum_def.Columns[Tags.ColumnName]    .CreateSqlField (this.type_converter, value.Name));
			fields.Add (enum_def.Columns[Tags.ColumnInfoXml] .CreateSqlField (this.type_converter, DbEnumValue.SerializeToXml (value, false)));
			fields.Add (enum_def.Columns[Tags.ColumnRefType] .CreateSqlField (this.type_converter, type.InternalKey.Id));
			
			this.sql_builder.InsertData (enum_def.CreateSqlName (), fields);
			this.ExecuteSilent (transaction);
		}
		
		protected void BootInsertTableDefRow(DbTransaction transaction, DbTable table)
		{
			System.Diagnostics.Debug.Assert (this.current_log_id.LocalID > 0);
			
			DbTable table_def = this.internal_tables[Tags.TableTableDef];
			
			//	Phase d'initialisation de la base : ins�re une ligne dans la table de d�finition des
			//	tables. Les colonnes descriptives (pour l'utilisateur) ne sont pas initialis�es.
			
			Collections.SqlFields fields = new Collections.SqlFields ();
			
			fields.Add (table_def.Columns[Tags.ColumnId]      .CreateSqlField (this.type_converter, table.InternalKey.Id));
			fields.Add (table_def.Columns[Tags.ColumnStatus]  .CreateSqlField (this.type_converter, table.InternalKey.IntStatus));
			fields.Add (table_def.Columns[Tags.ColumnRefLog]  .CreateSqlField (this.type_converter, this.current_log_id));
			fields.Add (table_def.Columns[Tags.ColumnName]    .CreateSqlField (this.type_converter, table.Name));
			fields.Add (table_def.Columns[Tags.ColumnInfoXml] .CreateSqlField (this.type_converter, DbTable.SerializeToXml (table, false)));
			fields.Add (table_def.Columns[Tags.ColumnNextId]  .CreateSqlField (this.type_converter, 0));
			
			this.sql_builder.InsertData (table_def.CreateSqlName (), fields);
			this.ExecuteSilent (transaction);
		}
		
		protected void BootUpdateTableNextId(DbTransaction transaction, DbKey key, long next_id)
		{
			Collections.SqlFields fields = new Collections.SqlFields ();
			Collections.SqlFields conds  = new Collections.SqlFields ();
			
			fields.Add (Tags.ColumnNextId, SqlField.CreateConstant (next_id, DbRawType.Int64));

			this.AddKeyExtraction (conds, key);
			
			System.Diagnostics.Debug.WriteLine (string.Format ("Table {0}, next ID will be {1}.", key, next_id));
			
			this.sql_builder.UpdateData (Tags.TableTableDef, fields, conds);
			this.ExecuteSilent (transaction);
		}
		
		protected void BootInsertColumnDefRow(DbTransaction transaction, DbTable table, DbColumn column)
		{
			System.Diagnostics.Debug.Assert (this.current_log_id.LocalID > 0);
			
			DbTable column_def = this.internal_tables[Tags.TableColumnDef];
			
			//	Phase d'initialisation de la base : ins�re une ligne dans la table de d�finition des
			//	colonnes. Les colonnes descriptives (pour l'utilisateur) ne sont pas initialis�es.
			
			Collections.SqlFields fields = new Collections.SqlFields ();
			
			fields.Add (column_def.Columns[Tags.ColumnId]      .CreateSqlField (this.type_converter, column.InternalKey.Id));
			fields.Add (column_def.Columns[Tags.ColumnStatus]  .CreateSqlField (this.type_converter, column.InternalKey.IntStatus));
			fields.Add (column_def.Columns[Tags.ColumnRefLog]  .CreateSqlField (this.type_converter, this.current_log_id));
			fields.Add (column_def.Columns[Tags.ColumnName]    .CreateSqlField (this.type_converter, column.Name));
			fields.Add (column_def.Columns[Tags.ColumnInfoXml] .CreateSqlField (this.type_converter, DbColumn.SerializeToXml (column, false)));
			fields.Add (column_def.Columns[Tags.ColumnRefTable].CreateSqlField (this.type_converter, table.InternalKey.Id));
			fields.Add (column_def.Columns[Tags.ColumnRefType] .CreateSqlField (this.type_converter, column.Type.InternalKey.Id));
			
			this.sql_builder.InsertData (column_def.CreateSqlName (), fields);
			this.ExecuteSilent (transaction);
		}
		
		protected void BootUpdateColumnRelation(DbTransaction transaction, string src_table_name, string src_column_name, string parent_table_name)
		{
			Collections.SqlFields fields = new Collections.SqlFields ();
			Collections.SqlFields conds  = new Collections.SqlFields ();
			
			DbTable  source = this.internal_tables[src_table_name];
			DbTable  parent = this.internal_tables[parent_table_name];
			DbColumn column = source.Columns[src_column_name];
			
			fields.Add (Tags.ColumnRefParent, SqlField.CreateConstant (parent.InternalKey.Id, DbRawType.Int64));

			this.AddKeyExtraction (conds, column.InternalKey);
			
			this.sql_builder.UpdateData (Tags.TableColumnDef, fields, conds);
			this.ExecuteSilent (transaction);
		}
		
		
		protected void BootSetupTables(DbTransaction transaction)
		{
			int log_key_id		= 1;
			int type_key_id     = 1;
			int table_key_id    = 1;
			int column_key_id   = 1;
			int enum_val_key_id = 1;
			
			this.current_log_id = DbID.CreateID (log_key_id++, this.client_id);
			
			this.BootInsertLogRow (transaction, this.current_log_id, 0);
			
			//	Il faut commencer par finir d'initialiser les descriptions des types, parce
			//	que les description des colonnes doivent y faire r�f�rence.
			
			foreach (DbType type in this.internal_types)
			{
				//	Attribue � chaque type interne une clef unique et �tablit les informations de base
				//	dans la table de d�finition des types.
				
				type.DefineInternalKey (new DbKey (type_key_id++));
				this.BootInsertTypeDefRow (transaction, type);
			}
			
			foreach (DbTable table in this.internal_tables)
			{
				//	Attribue � chaque table interne une clef unique et �tablit les informations de base
				//	dans la table de d�finition des tables.
				
				table.DefineInternalKey (new DbKey (table_key_id++));
				table.UpdatePrimaryKeyInfo ();
				
				this.BootInsertTableDefRow (transaction, table);
				
				foreach (DbColumn column in table.Columns)
				{
					//	Pour chaque colonne de la table, �tablit les informations de base dans la table de
					//	d�finition des colonnes.
					
					column.DefineInternalKey (new DbKey (column_key_id++));
					this.BootInsertColumnDefRow (transaction, table, column);
				}
			}
			
			//	Compl�te encore les informations au sujet des relations :
			//
			//	- La description d'une colonne fait r�f�rence � la table et � un type.
			//	- La description d'une valeur d'enum fait r�f�rence � un type.
			//	- La description d'une r�f�rence fait elle-m�me r�f�rence � la table
			//	  source et destination, ainsi qu'� la colonne.
			
			this.BootUpdateColumnRelation (transaction, Tags.TableColumnDef,   Tags.ColumnRefTable,  Tags.TableTableDef);
			this.BootUpdateColumnRelation (transaction, Tags.TableColumnDef,   Tags.ColumnRefType,   Tags.TableTypeDef);
			this.BootUpdateColumnRelation (transaction, Tags.TableColumnDef,   Tags.ColumnRefParent, Tags.TableTypeDef);
			this.BootUpdateColumnRelation (transaction, Tags.TableEnumValDef,  Tags.ColumnRefType,   Tags.TableTypeDef);
			
			this.BootUpdateTableNextId (transaction, this.internal_tables[Tags.TableLog].InternalKey, log_key_id);
			this.BootUpdateTableNextId (transaction, this.internal_tables[Tags.TableTableDef].InternalKey, table_key_id);
			this.BootUpdateTableNextId (transaction, this.internal_tables[Tags.TableColumnDef].InternalKey, column_key_id);
			this.BootUpdateTableNextId (transaction, this.internal_tables[Tags.TableTypeDef].InternalKey, type_key_id);
			this.BootUpdateTableNextId (transaction, this.internal_tables[Tags.TableEnumValDef].InternalKey, enum_val_key_id);
		}
		
		
		#region Initialisation
		protected void InitialiseDatabaseAbstraction()
		{
			this.types = new DbTypeCache (this);
			
			this.db_abstraction = DbFactory.FindDbAbstraction (this.db_access);
			
			this.sql_builder = this.db_abstraction.SqlBuilder;
			this.sql_engine  = this.db_abstraction.SqlEngine;
			
			System.Diagnostics.Debug.Assert (this.sql_builder != null);
			System.Diagnostics.Debug.Assert (this.sql_engine != null);
			
			this.type_converter = this.db_abstraction.Factory.TypeConverter;
			
			System.Diagnostics.Debug.Assert (this.type_converter != null);
			
			this.sql_builder.AutoClear = true;
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
					
					System.Diagnostics.Debug.Assert (this.db_abstraction.IsConnectionOpen == false);
					
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
		
		
		protected class DbTypeCache
		{
			public DbTypeCache(DbInfrastructure infrastructure)
			{
				this.infrastructure = infrastructure;
			}
			
			
			public void RegisterTypes()
			{
				this.InitialiseNumDefs ();
				this.InitialiseStrTypes ();
				
				this.AssertAllTypesReady ();
			}
			
			public void ResolveTypes(DbTransaction transaction)
			{
				this.num_type_key_id      = this.infrastructure.ResolveDbType (transaction, Tags.TypeKeyId) as DbTypeNum;
				this.num_type_key_status  = this.infrastructure.ResolveDbType (transaction, Tags.TypeKeyStatus) as DbTypeNum;
				this.str_type_name        = this.infrastructure.ResolveDbType (transaction, Tags.TypeName) as DbTypeString;
				this.str_type_caption     = this.infrastructure.ResolveDbType (transaction, Tags.TypeCaption) as DbTypeString;
				this.str_type_description = this.infrastructure.ResolveDbType (transaction, Tags.TypeDescription) as DbTypeString;
				this.str_type_info_xml    = this.infrastructure.ResolveDbType (transaction, Tags.TypeInfoXml) as DbTypeString;
				this.str_type_dict_key    = this.infrastructure.ResolveDbType (transaction, Tags.TypeDictKey) as DbTypeString;
				this.str_type_dict_value  = this.infrastructure.ResolveDbType (transaction, Tags.TypeDictValue) as DbTypeString;
				this.num_type_datetime    = this.infrastructure.ResolveDbType (transaction, Tags.TypeDateTime) as DbTypeNum;
				
				this.infrastructure.internal_types.Add (this.num_type_key_id);
				this.infrastructure.internal_types.Add (this.num_type_key_status);
				this.infrastructure.internal_types.Add (this.str_type_name);
				this.infrastructure.internal_types.Add (this.str_type_caption);
				this.infrastructure.internal_types.Add (this.str_type_description);
				this.infrastructure.internal_types.Add (this.str_type_info_xml);
				this.infrastructure.internal_types.Add (this.str_type_dict_key);
				this.infrastructure.internal_types.Add (this.str_type_dict_value);
				this.infrastructure.internal_types.Add (this.num_type_datetime);
				
				this.AssertAllTypesReady ();
			}
			
			
			void InitialiseNumDefs()
			{
				this.num_type_key_id     = new DbTypeNum (DbNumDef.FromRawType (DbKey.RawTypeForId),     Tags.Name + "=" + Tags.TypeKeyId);
				this.num_type_key_status = new DbTypeNum (DbNumDef.FromRawType (DbKey.RawTypeForStatus), Tags.Name + "=" + Tags.TypeKeyStatus);
				this.num_type_datetime   = new DbTypeNum (DbNumDef.FromRawType (DbRawType.Int64),        Tags.Name + "=" + Tags.TypeDateTime);
			
				this.infrastructure.internal_types.Add (this.num_type_key_id);
				this.infrastructure.internal_types.Add (this.num_type_key_status);
				this.infrastructure.internal_types.Add (this.num_type_datetime);
			}
			
			void InitialiseStrTypes()
			{
				this.str_type_name        = new DbTypeString (DbColumn.MaxNameLength, false,		Tags.Name + "=" + Tags.TypeName);
				this.str_type_caption     = new DbTypeString (DbColumn.MaxCaptionLength, false,		Tags.Name + "=" + Tags.TypeCaption);
				this.str_type_description = new DbTypeString (DbColumn.MaxDescriptionLength, false, Tags.Name + "=" + Tags.TypeDescription);
				this.str_type_info_xml    = new DbTypeString (DbColumn.MaxInfoXmlLength, false,		Tags.Name + "=" + Tags.TypeInfoXml);
				this.str_type_dict_key    = new DbTypeString (DbColumn.MaxDictKeyLength, false,		Tags.Name + "=" + Tags.TypeDictKey);
				this.str_type_dict_value  = new DbTypeString (DbColumn.MaxDictValueLength, false,	Tags.Name + "=" + Tags.TypeDictValue);
			
				this.infrastructure.internal_types.Add (this.str_type_name);
				this.infrastructure.internal_types.Add (this.str_type_caption);
				this.infrastructure.internal_types.Add (this.str_type_description);
				this.infrastructure.internal_types.Add (this.str_type_info_xml);
				this.infrastructure.internal_types.Add (this.str_type_dict_key);
				this.infrastructure.internal_types.Add (this.str_type_dict_value);
			}
			
			
			void AssertAllTypesReady ()
			{
				System.Diagnostics.Debug.Assert (this.num_type_key_id != null);
				System.Diagnostics.Debug.Assert (this.num_type_key_status != null);
				System.Diagnostics.Debug.Assert (this.num_type_datetime != null);
				
				System.Diagnostics.Debug.Assert (this.str_type_name != null);
				System.Diagnostics.Debug.Assert (this.str_type_caption != null);
				System.Diagnostics.Debug.Assert (this.str_type_description != null);
				System.Diagnostics.Debug.Assert (this.str_type_info_xml != null);
				System.Diagnostics.Debug.Assert (this.str_type_dict_key != null);
				System.Diagnostics.Debug.Assert (this.str_type_dict_value != null);
			}
			
			
			public DbTypeNum					KeyId
			{
				get
				{
					return this.num_type_key_id;
				}
			}
			
			public DbTypeNum					KeyStatus
			{
				get
				{
					return this.num_type_key_status;
				}
			}
			
			public DbTypeNum					DateTime
			{
				get
				{
					return this.num_type_datetime;
				}
			}
			
			public DbTypeString					Name
			{
				get
				{
					return this.str_type_name;
				}
			}
			
			public DbTypeString					Caption
			{
				get
				{
					return this.str_type_caption;
				}
			}
			
			public DbTypeString					Description
			{
				get
				{
					return this.str_type_description;
				}
			}
			
			public DbTypeString					InfoXml
			{
				get
				{
					return this.str_type_info_xml;
				}
			}
			
			public DbTypeString					DictKey
			{
				get
				{
					return this.str_type_dict_key;
				}
			}
			
			public DbTypeString					DictValue
			{
				get
				{
					return this.str_type_dict_value;
				}
			}
			
			
			protected DbInfrastructure			infrastructure;
			
			protected DbTypeNum					num_type_key_id;
			protected DbTypeNum					num_type_key_status;
			protected DbTypeNum					num_type_datetime;
			
			protected DbTypeString				str_type_name;
			protected DbTypeString				str_type_caption;
			protected DbTypeString				str_type_description;
			protected DbTypeString				str_type_info_xml;
			protected DbTypeString				str_type_dict_key;
			protected DbTypeString				str_type_dict_value;
		}
		
		
		protected DbAccess						db_access;
		protected IDbAbstraction				db_abstraction;
		
		protected ISqlBuilder					sql_builder;
		protected ISqlEngine					sql_engine;
		protected ITypeConverter				type_converter;
		
		protected DbTypeCache					types;
		
		protected Collections.DbTables			internal_tables = new Collections.DbTables ();
		protected Collections.DbTypes			internal_types  = new Collections.DbTypes ();
		
		protected int							client_id;
		protected DbID							current_log_id;
		
		CallbackDisplayDataSet					display_data_set;
		string[]								localisations;
		
		Cache.DbTypes							cache_db_types = new Cache.DbTypes ();
		Cache.DbTables							cache_db_tables = new Cache.DbTables ();
		
		protected DbTransaction					live_transaction;
		protected bool							release_connection_requested;
	}
}
