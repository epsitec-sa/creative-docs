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
		}
		
		
		public void CreateDatabase(DbAccess db_access)
		{
			if (this.db_access.IsValid)
			{
				throw new DbException (this.db_access, "Database already exists");
			}
			
			this.db_access = db_access;
			this.db_access.Create = true;
			
			this.InitialiseNumDefs ();
			this.InitialiseStrTypes ();
			this.InitialiseDatabaseAbstraction ();
			
			//	La base de données vient d'être créée. Elle est donc toute vide (aucune
			//	table n'est encore définie).
			
			System.Diagnostics.Debug.Assert (this.db_abstraction.UserTableNames.Length == 0);
			
			//	Il faut créer les tables internes utilisées pour la gestion des méta-données.
			
			using (DbTransaction transaction = this.BeginTransaction ())
			{
				this.BootCreateTableTableDef (transaction);
				this.BootCreateTableColumnDef (transaction);
				this.BootCreateTableTypeDef (transaction);
				this.BootCreateTableEnumValDef (transaction);
				this.BootCreateTableRelationDef (transaction);
				
				//	Valide la création de toutes ces tables avant de commencer à peupler
				//	les tables. Firebird requiert ce mode de fonctionnement.
				
				transaction.Commit ();
			}
			
			//	Les tables ont toutes été créées. Il faut maintenant les remplir avec
			//	les informations de départ.
				
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
				this.internal_tables.Add (this.ResolveDbTable (transaction, DbTable.TagTableDef));
				this.internal_tables.Add (this.ResolveDbTable (transaction, DbTable.TagColumnDef));
				this.internal_tables.Add (this.ResolveDbTable (transaction, DbTable.TagTypeDef));
				this.internal_tables.Add (this.ResolveDbTable (transaction, DbTable.TagRelationDef));
				this.internal_tables.Add (this.ResolveDbTable (transaction, DbTable.TagEnumValDef));
			
				this.internal_types.Add (this.ResolveDbType (transaction, DbType.TagKeyId));
				this.internal_types.Add (this.ResolveDbType (transaction, DbType.TagKeyRevision));
				this.internal_types.Add (this.ResolveDbType (transaction, DbType.TagKeyStatus));
				this.internal_types.Add (this.ResolveDbType (transaction, DbType.TagName));
				this.internal_types.Add (this.ResolveDbType (transaction, DbType.TagCaption));
				this.internal_types.Add (this.ResolveDbType (transaction, DbType.TagDescription));
				this.internal_types.Add (this.ResolveDbType (transaction, DbType.TagInfoXml));
				
				transaction.Commit ();
			}
		}
		
		
		public CallbackDisplayDataSet		DisplayDataSet
		{
			get { return this.display_data_set; }
			set { this.display_data_set = value; }
		}
		
		public ISqlBuilder					SqlBuilder
		{
			get { return this.sql_builder; }
		}
		
		public ISqlEngine					SqlEngine
		{
			get { return this.sql_engine; }
		}
		
		public DbTransaction				LiveTransaction
		{
			get { return this.live_transaction; }
		}
		
		
		public DbTransaction BeginTransaction()
		{
			//	Débute une nouvelle transaction. Ceci n'est possible que si aucune
			//	autre transaction n'est actuellement en cours sur cette connexion.
			
			return new DbTransaction (this.db_abstraction.BeginTransaction (), this);
		}
		
		
		public DbTable   CreateDbTable(string name, DbElementCat category)
		{
			//	Crée la description d'une table qui ne contient que le strict minimum nécessaire au fonctionnement
			//	de Crésus (tuple pour la clef primaire, statut). Il faudra compléter les colonnes en fonction des
			//	besoins, puis appeler la méthode RegisterNewDbTable.
			
			switch (category)
			{
				case DbElementCat.Internal:
					throw new DbException (this.db_access, string.Format ("User may not create internal table. Table '{0}'.", name));
				
				case DbElementCat.UserDataManaged:
					return this.CreateUserTable(name);
				
				default:
					throw new DbException (this.db_access, string.Format ("Unsupported category {0} specified. Table '{1}'.", category, name));
			}
		}
		
		public void      RegisterNewDbTable(DbTransaction transaction, DbTable table)
		{
			//	Enregistre une nouvelle table dans la base de données. Ceci va attribuer à
			//	la table une clef DbKey et vérifier qu'il n'y a pas de collision avec une
			//	éventuelle table déjà existante. Cela va aussi attribuer des colonnes pour
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
			
			long table_id  = this.NewRowIdInTable (transaction, this.internal_tables[DbTable.TagTableDef].InternalKey, 1);
			long column_id = this.NewRowIdInTable (transaction, this.internal_tables[DbTable.TagColumnDef].InternalKey, table.Columns.Count);
			
			//	Crée la ligne de description de la table :
			
			table.DefineInternalKey (new DbKey (table_id));
			table.UpdatePrimaryKeyInfo ();
			
			this.BootInsertTableDefRow (transaction, table);
			
			//	Crée les lignes de description des colonnes :
			
			for (int i = 0; i < table.Columns.Count; i++)
			{
				table.Columns[i].DefineInternalKey (new DbKey (column_id + i));
				this.BootInsertColumnDefRow (transaction, table, table.Columns[i]);
			}
			
			//	Finalement, il faut créer la table elle-même :
			
			SqlTable sql_table = table.CreateSqlTable (this.type_converter);
			
			this.sql_builder.InsertTable (sql_table);
			this.ExecuteSilent (transaction);
		}
		
		public void      UnregisterDbTable(DbTransaction transaction, DbTable table)
		{
			//	Supprime la description de la table de la base. Pour des raisons de sécurité,
			//	la table SQL n'est pas réellement supprimée.
			
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
			
			int revision = this.FindHighestRowRevision (transaction, DbTable.TagTableDef, table.InternalKey.Id) + 1;
			
			System.Diagnostics.Debug.Assert (revision > 0);
			
			DbKey old_key = table.InternalKey;
			DbKey new_key = new DbKey (old_key.Id, revision, revision);
			
			this.UpdateKeyInRow (transaction, DbTable.TagTableDef, old_key, new_key);
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
						
						this.cache_db_tables[key] = table;
					}
				}
				
				return table;
			}
		}
		
		public DbTable[] FindDbTables(DbTransaction transaction, DbElementCat category)
		{
			//	Liste toutes les tables appartenant à la catégorie spécifiée.
			
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
		
		
		public DbColumn[] CreateRefColumns(string column_name, string target_table_name, DbKeyMatchMode match_mode)
		{
			//	Crée la ou les colonnes nécessaires à la définition d'une référence à une autre
			//	table.
			
			DbType type_id;
			DbType type_rev;
			
			switch (match_mode)
			{
				case DbKeyMatchMode.SimpleId:
					type_id = this.internal_types[DbType.TagKeyId];
					
					return new DbColumn[] { DbColumn.NewRefColumn (column_name, target_table_name, DbColumnClass.RefSimpleId, type_id) };
				
				case DbKeyMatchMode.LiveId:
					type_id = this.internal_types[DbType.TagKeyId];
					
					return new DbColumn[] { DbColumn.NewRefColumn (column_name, target_table_name, DbColumnClass.RefLiveId, type_id) };
				
				case DbKeyMatchMode.ExactIdRevision:
					type_id  = this.internal_types[DbType.TagKeyId];
					type_rev = this.internal_types[DbType.TagKeyRevision];
					
					return new DbColumn[] { DbColumn.NewRefColumn (column_name, target_table_name, DbColumnClass.RefTupleId, type_id),
						/**/				DbColumn.NewRefColumn (column_name, target_table_name, DbColumnClass.RefTupleRevision, type_rev) };
			}
			
			return null;
		}
		
		
		public void RegisterCrossTableReferences(DbTransaction transaction, DbTable table)
		{
			if (transaction == null)
			{
				using (transaction = this.BeginTransaction ())
				{
					this.RegisterCrossTableReferences (transaction, table);
					transaction.Commit ();
					return;
				}
			}
			
			//	Passe en revue toutes les colonnes de type ID qui font référence à une table
			//	et enregistre l'information dans la table de définition des références.
			//
			//	Note: il faut que les tables aient été enregistrées auprès de Crésus pour
			//	que cette méthode fonctionne (on a besoin des IDs des tables et des colonnes
			//	concernées).
			
			System.Collections.Hashtable ref_columns = new System.Collections.Hashtable ();
			
			for (int i = 0; i < table.Columns.Count; i++)
			{
				DbColumn column = table.Columns[i];
				
				switch (column.ColumnClass)
				{
					case DbColumnClass.RefLiveId:
					case DbColumnClass.RefSimpleId:
					case DbColumnClass.RefTupleId:
						string target_name = column.TargetTableName;
						
						if (target_name != null)
						{
							DbTable target_table = this.ResolveDbTable (transaction, target_name);
							
							if (target_table == null)
							{
								string message = string.Format ("Table '{0}' referenced from '{1}.{2}' not found in database.", target_name, table.Name, column.Name);
								throw new DbException (this.db_access, message);
							}
							
							DbKey source_table_key  = table.InternalKey;
							DbKey source_column_key = column.InternalKey;
							DbKey target_table_key  = target_table.InternalKey;
							
							if (source_table_key == null)
							{
								string message = string.Format ("Reference of '{0}' from '{1}.{2}' specifies unregistered table '{1}'.", target_name, table.Name, column.Name);
								throw new DbException (this.db_access, message);
							}
							
							if (source_column_key == null)
							{
								string message = string.Format ("Reference of '{0}' from '{1}.{2}' specifies unregistered column '{2}'.", target_name, table.Name, column.Name);
								throw new DbException (this.db_access, message);
							}
							
							if (target_table_key == null)
							{
								string message = string.Format ("Reference of '{0}' from '{1}.{2}' specifies unregistered table '{0}'.", target_name, table.Name, column.Name);
								throw new DbException (this.db_access, message);
							}
							
							this.RegisterCrossTableReferences (transaction, source_table_key, source_column_key, target_table_key);
						}
						break;
					
					default:
						break;
				}
			}
		}
		
		protected void RegisterCrossTableReferences(DbTransaction transaction, DbKey source_table_key, DbKey source_column_key, DbKey target_table_key)
		{
			System.Diagnostics.Debug.Assert (source_table_key  != null);
			System.Diagnostics.Debug.Assert (source_column_key != null);
			System.Diagnostics.Debug.Assert (target_table_key  != null);
			
			//	TODO: enregistre la relation dans la table DbTable.TagRefTable
			//
			//	(1) vérifie que la relation n'est pas encore connue; si elle l'est, génère
			//		une exception;
			//	(2) trouve un ID pour la nouvelle ligne qui va être créée dans TagRefTable;
			//	(3) crée la ligne dans la table;
			//
			//	Voir aussi RegisterNewDbTable comme modèle, ainsi que BootInsertRelationDefRow
			//	pour l'insertion de la ligne. Il faudra créer les méthodes suivantes :
			//
			//	- CheckForUnknownRelation/CheckForKnownRelation (prenant 3 DbKey comme arg.)
			//	- InsertRelationDefRow, à moins de modifier BootInsertRelationDefRow pour
			//	  accepter des DbKey comme arguments; voir si c'est faisable facilement.
			
			throw new System.NotImplementedException ("RegisterCrossTableReferences not implemented.");
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
		
		public void      RegisterNewDbType(DbTransaction transaction, DbType type)
		{
			//	Enregistre un nouveau type dans la base de données. Ceci va attribuer au
			//	type une clef DbKey et vérifier qu'il n'y a pas de collision avec un
			//	éventuel type déjà existant.
			
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
			
			long table_id = this.NewRowIdInTable (transaction, this.internal_tables[DbTable.TagTableDef].InternalKey, 1);
			long enum_id  = (type_enum == null) ? 0 : this.NewRowIdInTable (transaction, this.internal_tables[DbTable.TagEnumValDef].InternalKey, type_enum.Count);
			
			//	Crée la ligne de description du type :
			
			type.DefineInternalKey (new DbKey (table_id));
			
			this.BootInsertTypeDefRow (transaction, type);
			
			if (type_enum != null)
			{
				//	Crée les lignes de description des valeurs de l'énumération :
				
				for (int i = 0; i < type_enum.Count; i++)
				{
					type_enum[i].DefineInternalKey (new DbKey (enum_id + i));
					this.BootInsertEnumValueDefRow (transaction, type, type_enum[i]);
				}
			}
		}
		
		public void      UnregisterDbType(DbTransaction transaction, DbType type)
		{
			//	Supprime la description du type de la base. Pour des raisons de sécurité,
			//	le type SQL n'est pas réellement supprimé.
			
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
			
			int revision = this.FindHighestRowRevision (transaction, DbTable.TagTypeDef, type.InternalKey.Id) + 1;
			
			System.Diagnostics.Debug.Assert (revision > 0);
			
			DbKey old_key = type.InternalKey;
			DbKey new_key = new DbKey (old_key.Id, revision, revision);
			
			this.UpdateKeyInRow (transaction, DbTable.TagTypeDef, old_key, new_key);
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

			//	Trouve le type corredpondant à une clef spécifique.
			
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
		
		
		
		internal DbTable CreateUserTable(string name)
		{
			DbTable table = new DbTable (name);
			
			DbType type = this.internal_types[DbType.TagKeyId];
			
			DbColumn col_id   = new DbColumn (DbColumn.TagId,       this.internal_types[DbType.TagKeyId]);
			DbColumn col_rev  = new DbColumn (DbColumn.TagRevision, this.internal_types[DbType.TagKeyRevision]);
			DbColumn col_stat = new DbColumn (DbColumn.TagStatus,   this.internal_types[DbType.TagKeyStatus]);
			
			col_id.DefineCategory (DbElementCat.Internal);
			col_id.DefineColumnClass (DbColumnClass.KeyId);
			
			col_rev.DefineCategory (DbElementCat.Internal);
			col_rev.DefineColumnClass (DbColumnClass.KeyRevision);
			
			col_stat.DefineCategory (DbElementCat.Internal);
			col_stat.DefineColumnClass (DbColumnClass.KeyStatus);
			
			table.DefineCategory (DbElementCat.UserDataManaged);
			
			table.Columns.Add (col_id);
			table.Columns.Add (col_rev);
			table.Columns.Add (col_stat);
			
			table.PrimaryKeys.Add (col_id);
			table.PrimaryKeys.Add (col_rev);
			
			return table;
		}
		
		
		protected void CheckForRegisteredTypes(DbTransaction transaction, DbTable table)
		{
			//	Vérifie que tous les types utilisés dans la définition des colonnes sont bien
			//	connus (on vérifie qu'ils ont une clef valide).
			
			DbColumnCollection columns = table.Columns;
			
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
			
			if (this.CountMatchingRows (transaction, DbTable.TagTypeDef, DbColumn.TagName, type.Name) > 0)
			{
				string message = string.Format ("Type {0} already exists in database.", type.Name);
				throw new DbException (this.db_access, message);
			}
		}
		
		protected void CheckForKnownType(DbTransaction transaction, DbType type)
		{
			System.Diagnostics.Debug.Assert (type != null);
			
			if (this.CountMatchingRows (transaction, DbTable.TagTypeDef, DbColumn.TagName, type.Name) == 0)
			{
				string message = string.Format ("Type {0} does not exist in database.", type.Name);
				throw new DbException (this.db_access, message);
			}
		}
		
		protected void CheckForUnknownTable(DbTransaction transaction, DbTable table)
		{
			//	Cherche si une table avec ce nom existe dans la base. Si c'est le cas,
			//	génère une exception.
			//
			//	NOTE:
			//
			//	On cherche les lignes dans CR_TABLE_DEF dont la colonne CR_NAME contient le nom
			//	spécifié et dont CR_REV = 0. Cette seconde condition est nécessaire, car une table
			//	détruite figure encore dans CR_TABLE_DEF avec CR_REV > 0, et elle ne doit pas être
			//	comptée.
			
			if (this.CountMatchingRows (transaction, DbTable.TagTableDef, DbColumn.TagName, table.Name) > 0)
			{
				string message = string.Format ("Table {0} already exists in database.", table.Name);
				throw new DbException (this.db_access, message);
			}
		}
		
		protected void CheckForKnownTable(DbTransaction transaction, DbTable table)
		{
			//	Cherche si une table avec ce nom existe dans la base. Si ce n'est pas le cas,
			//	génère une exception.
			//
			//	NOTE:
			//
			//	On cherche les lignes dans CR_TABLE_DEF dont la colonne CR_NAME contient le nom
			//	spécifié et dont CR_REV = 0. Cette seconde condition est nécessaire, car une table
			//	détruite figure encore dans CR_TABLE_DEF avec CR_REV > 0, et elle ne doit pas être
			//	comptée.
			
			if (this.CountMatchingRows (transaction, DbTable.TagTableDef, DbColumn.TagName, table.Name) == 0)
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
				System.Console.Out.WriteLine ("SQL Command: {0}", command.CommandText);
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
				
				System.Console.Out.WriteLine ("SQL Command: {0}", command.CommandText);
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
				
				System.Console.Out.WriteLine ("SQL Command: {0}", command.CommandText);
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
				
				System.Console.Out.WriteLine ("SQL Command: {0}", command.CommandText);
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
			return this.FindLiveKey (this.FindDbKeys (transaction, DbTable.TagTableDef, name));
		}
		
		public DbKey FindDbTypeKey(DbTransaction transaction, string name)
		{
			return this.FindLiveKey (this.FindDbKeys (transaction, DbTable.TagTypeDef, name));
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
		
		internal DbKey[] FindDbKeys(DbTransaction transaction, string table_name, string row_name)
		{
			//	Trouve la (ou les) clefs.
			
			SqlSelect query = new SqlSelect ();
			
			query.Fields.Add ("T_ID",   SqlField.CreateName ("T", DbColumn.TagId));
			query.Fields.Add ("T_REV",	SqlField.CreateName ("T", DbColumn.TagRevision));
			query.Fields.Add ("T_STAT",	SqlField.CreateName ("T", DbColumn.TagStatus));
			
			query.Tables.Add ("T", SqlField.CreateName (table_name));
			
			query.Conditions.Add (new SqlFunction (SqlFunctionType.CompareEqual, SqlField.CreateName ("T", DbColumn.TagName), SqlField.CreateConstant (row_name, DbRawType.String)));
			
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
				int  revision;
				int  status;
				
				Converter.Convert (row["T_ID"],   out id);
				Converter.Convert (row["T_REV"],  out revision);
				Converter.Convert (row["T_STAT"], out status);
				
				keys[i] = new DbKey (id, revision, status);
			}
			
			return keys;
		}
		
		
		public int CountMatchingRows(DbTransaction transaction, string table, string name_column, string value)
		{
			//	Compte combien de lignes dans la table ont le texte spécifié dans la colonne spécifiée.
			//	Ne considère que les lignes dont la révision est zéro.
			
			SqlSelect query = new SqlSelect ();
			
			query.Fields.Add ("N", new SqlAggregate (SqlAggregateType.Count, SqlField.CreateAll ()));
			query.Tables.Add ("T", SqlField.CreateName (table));
			
			query.Conditions.Add (new SqlFunction (SqlFunctionType.CompareEqual, SqlField.CreateName (name_column), SqlField.CreateConstant (value, DbRawType.String)));
			query.Conditions.Add (new SqlFunction (SqlFunctionType.CompareEqual, SqlField.CreateName (DbColumn.TagRevision), SqlField.CreateConstant (0, DbRawType.Int32)));
			
			this.sql_builder.SelectData (query);
			
			int count;
			
			Converter.Convert (this.ExecuteScalar (transaction), out count);
			
			return count;
		}
		
		
		public int FindHighestRowRevision(DbTransaction transaction, string table, long id)
		{
			//	Trouve la révision la plus élevée (-1 si aucune n'est trouvée) pour une clef
			//	donnée.
			
			SqlSelect query = new SqlSelect ();
			
			query.Fields.Add ("R", new SqlAggregate (SqlAggregateType.Max, SqlField.CreateName (DbColumn.TagRevision)));
			query.Tables.Add ("T", SqlField.CreateName (table));
			
			query.Conditions.Add (new SqlFunction (SqlFunctionType.CompareEqual, SqlField.CreateName (DbColumn.TagId), SqlField.CreateConstant (id, DbRawType.Int64)));
			
			this.sql_builder.SelectData (query);
			
			object result  = this.ExecuteScalar (transaction);
			int    max_rev = -1;
			
			if (Converter.IsNotNull (result))
			{
				Converter.Convert (result, out max_rev);
			}
			
			return max_rev;
		}
		
		public void UpdateKeyInRow(DbTransaction transaction, string table, DbKey old_key, DbKey new_key)
		{
			//	Met à jour la clef de la ligne spécifiée.
			
			SqlFieldCollection fields = new SqlFieldCollection ();
			SqlFieldCollection conds  = new SqlFieldCollection ();
			
			fields.Add (DbColumn.TagId,       SqlField.CreateConstant (new_key.Id,       DbRawType.Int64));
			fields.Add (DbColumn.TagRevision, SqlField.CreateConstant (new_key.Revision, DbRawType.Int32));
			
			conds.Add (new SqlFunction (SqlFunctionType.CompareEqual, SqlField.CreateName (DbColumn.TagId),       SqlField.CreateConstant (old_key.Id,       DbRawType.Int64)));
			conds.Add (new SqlFunction (SqlFunctionType.CompareEqual, SqlField.CreateName (DbColumn.TagRevision), SqlField.CreateConstant (old_key.Revision, DbRawType.Int32)));
			
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
			//	Charge les définitions pour la table au moyen d'une requête unique qui va
			//	aussi retourner les diverses définitions de colonnes.
			
			SqlSelect query = new SqlSelect ();
			
			//	Ce qui est propre à la table :
			
			query.Fields.Add ("T_ID",   SqlField.CreateName ("T_TABLE", DbColumn.TagId));
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
			
			if (key == null)
			{
				//	On extrait toutes les définitions de tables qui correspondent à la version
				//	'active' (CR_REV = 0). Extrait aussi les colonnes correspondantes.
				
				SqlField table_rev_name = SqlField.CreateName ("T_TABLE", DbColumn.TagRevision);
				SqlField table_rev_zero = SqlField.CreateConstant (0, DbRawType.Int32);
				
				query.Conditions.Add (new SqlFunction (SqlFunctionType.CompareEqual, table_rev_name, table_rev_zero));
				
				this.AddKeyExtraction (query, "T_COLUMN", DbColumn.TagRefTable, "T_TABLE");
			}
			else
			{
				//	On extrait toutes les lignes de T_TABLE qui ont un CR_ID = key, ainsi que
				//	les lignes correspondantes de T_COLUMN qui ont un CREF_TABLE = key.
				
				this.AddKeyExtraction (query, key, "T_TABLE", DbKeyMatchMode.ExactIdRevision);
				this.AddKeyExtraction (query, "T_COLUMN", DbColumn.TagRefTable, key);
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
			
			for (int i = 0; i < rows.Count; i++)
			{
				long current_row_id;
				System.Data.DataRow data_row = rows[i];
				
				Converter.Convert (data_row["T_ID"], out current_row_id);
				
				if (row_id != current_row_id)
				{
					row_id   = current_row_id;
					db_table = DbTable.NewTable (Converter.ToString (data_row["T_INFO"]));
					
					db_table.Attributes.SetAttribute (Tags.Name, Converter.ToString (data_row["T_NAME"]));
					db_table.DefineInternalKey (key);
					
					this.DefineLocalisedAttributes (data_row, "TABLE_CAPTION", DbColumn.TagCaption, db_table.Attributes, Tags.Caption);
					this.DefineLocalisedAttributes (data_row, "TABLE_DESCRIPTION", DbColumn.TagDescription, db_table.Attributes, Tags.Description);
					
					tables.Add (db_table);
				}
				
				//	Chaque ligne contient une définition de colonne.
				
				long type_ref_id;
				long column_id;
				
				DbColumn db_column = DbColumn.NewColumn (Converter.ToString (data_row["C_INFO"]));
				
				Converter.Convert (data_row["C_ID"], out column_id);
				Converter.Convert (data_row["C_TYPE"], out type_ref_id);
				
				db_column.Attributes.SetAttribute (Tags.Name, Converter.ToString (data_row["C_NAME"]));
				db_column.DefineInternalKey (new DbKey (column_id));
				
				this.DefineLocalisedAttributes (data_row, "COLUMN_CAPTION", DbColumn.TagCaption, db_column.Attributes, Tags.Caption);
				this.DefineLocalisedAttributes (data_row, "COLUMN_DESCRIPTION", DbColumn.TagDescription, db_column.Attributes, Tags.Description);
				
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
			
			return tables;
		}
		
		public System.Collections.ArrayList LoadDbType(DbTransaction transaction, DbKey key)
		{
			SqlSelect query = new SqlSelect ();
			
			query.Fields.Add ("T_ID",   SqlField.CreateName ("T_TYPE", DbColumn.TagId));
			query.Fields.Add ("T_NAME", SqlField.CreateName ("T_TYPE", DbColumn.TagName));
			query.Fields.Add ("T_INFO", SqlField.CreateName ("T_TYPE", DbColumn.TagInfoXml));
			
			this.AddLocalisedColumns (query, "TYPE_CAPTION", "T_TYPE", DbColumn.TagCaption);
			this.AddLocalisedColumns (query, "TYPE_DESCRIPTION", "T_TYPE", DbColumn.TagDescription);
			
			query.Tables.Add ("T_TYPE", SqlField.CreateName (DbTable.TagTypeDef));
			
			if (key == null)
			{
				//	On extrait toutes les définitions de types qui correspondent à la version
				//	'active' (CR_REV = 0).
				
				SqlField type_rev_name = SqlField.CreateName ("T_TYPE", DbColumn.TagRevision);
				SqlField type_rev_zero = SqlField.CreateConstant (0, DbRawType.Int32);
				
				query.Conditions.Add (new SqlFunction (SqlFunctionType.CompareEqual, type_rev_name, type_rev_zero));
			}
			else
			{
				//	Cherche la ligne de la table dont 'CR_ID = key' en ne considérant que
				//	l'ID et dont la révision = 0.
				
				this.AddKeyExtraction (query, key, "T_TYPE", DbKeyMatchMode.LiveId);
			}
			
			System.Data.DataTable        data_table = this.ExecuteSqlSelect (transaction, query, 1);
			System.Collections.ArrayList types      = new System.Collections.ArrayList ();
			
			foreach (System.Data.DataRow data_row in data_table.Rows)
			{
				long type_id;
				
				Converter.Convert (data_row["T_ID"], out type_id);
				
				string type_name = data_row["T_NAME"] as string;
				string type_info = data_row["T_INFO"] as string;
				
				//	A partir de l'information trouvée dans la base, génère l'objet DbType
				//	qui correspond.
				
				DbType type = DbTypeFactory.NewType (type_info);
				
				type.DefineName (type_name);
				type.DefineInternalKey (new DbKey (type_id));
				
				this.DefineLocalisedAttributes (data_row, "TYPE_CAPTION", DbColumn.TagCaption, type.Attributes, Tags.Caption);
				this.DefineLocalisedAttributes (data_row, "TYPE_DESCRIPTION", DbColumn.TagDescription, type.Attributes, Tags.Description);
				
				if (type is DbTypeEnum)
				{
					DbTypeEnum type_enum = type as DbTypeEnum;
					DbEnumValue[] values = this.LoadEnumValues (transaction, type_enum);
					
					type_enum.Initialise (values);
				}
				
				types.Add (type);
			}
			
			return types;
		}
		
		
		public long NewRowIdInTable(DbTransaction transaction, DbKey key, int num_keys)
		{
			//	Attribue 'num_keys' clef consécutives dans la table spécifiée.
			
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
				this.ExecuteSilent (transaction);
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
		
		
		protected DbEnumValue[] LoadEnumValues(DbTransaction transaction, DbTypeEnum type_enum)
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
			
			System.Data.DataTable data_table = this.ExecuteSqlSelect (transaction, query, 1);
			
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
				
				query.Fields.Add (index, SqlField.CreateName (table, column));
			}
		}
		
		
		protected void AddKeyExtraction(SqlSelect query, DbKey key, string target_table_name, DbKeyMatchMode mode)
		{
			SqlField name_col_id = SqlField.CreateName (target_table_name, DbColumn.TagId);
			SqlField constant_id = SqlField.CreateConstant (key.Id, DbRawType.Int64);
			
			query.Conditions.Add (new SqlFunction (SqlFunctionType.CompareEqual, name_col_id, constant_id));
			
			int revision = 0;
			
			if (mode == DbKeyMatchMode.ExactIdRevision)
			{
				revision = key.Revision;
			}
			
			if ((mode == DbKeyMatchMode.LiveId) ||
				(mode == DbKeyMatchMode.ExactIdRevision))
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
		protected void BootCreateTableTableDef(DbTransaction transaction)
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
			columns[2].DefineColumnClass (DbColumnClass.KeyStatus);
			columns[4].DefineColumnLocalisation (DbColumnLocalisation.Default);
			columns[5].DefineColumnLocalisation (DbColumnLocalisation.Default);
			
			this.SetCategory (columns, DbElementCat.Internal);
			
			table.DefineCategory (DbElementCat.Internal);
			table.Columns.AddRange (columns);
			
			table.PrimaryKeys.Add (columns[0]);
			table.PrimaryKeys.Add (columns[1]);
			
			this.internal_tables.Add (table);
			
			SqlTable sql_table = table.CreateSqlTable (this.type_converter);
			this.sql_builder.InsertTable (sql_table);
			this.ExecuteSilent (transaction);
		}
		
		protected void BootCreateTableColumnDef(DbTransaction transaction)
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
			columns[2].DefineColumnClass (DbColumnClass.KeyStatus);
			columns[4].DefineColumnLocalisation (DbColumnLocalisation.Default);
			columns[5].DefineColumnLocalisation (DbColumnLocalisation.Default);
			columns[7].DefineColumnClass (DbColumnClass.RefLiveId);
			columns[8].DefineColumnClass (DbColumnClass.RefLiveId);
			
			this.SetCategory (columns, DbElementCat.Internal);
			
			table.DefineCategory (DbElementCat.Internal);
			table.Columns.AddRange (columns);
			
			table.PrimaryKeys.Add (columns[0]);
			table.PrimaryKeys.Add (columns[1]);
			
			this.internal_tables.Add (table);
			
			SqlTable sql_table = table.CreateSqlTable (this.type_converter);
			this.sql_builder.InsertTable (sql_table);
			this.ExecuteSilent (transaction);
		}
		
		protected void BootCreateTableTypeDef(DbTransaction transaction)
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
			columns[2].DefineColumnClass (DbColumnClass.KeyStatus);
			columns[4].DefineColumnLocalisation (DbColumnLocalisation.Default);
			columns[5].DefineColumnLocalisation (DbColumnLocalisation.Default);
			
			this.SetCategory (columns, DbElementCat.Internal);
			
			table.DefineCategory (DbElementCat.Internal);
			table.Columns.AddRange (columns);
			
			table.PrimaryKeys.Add (columns[0]);
			table.PrimaryKeys.Add (columns[1]);
			
			this.internal_tables.Add (table);
			
			SqlTable sql_table = table.CreateSqlTable (this.type_converter);
			this.sql_builder.InsertTable (sql_table);
			this.ExecuteSilent (transaction);
		}
		
		protected void BootCreateTableEnumValDef(DbTransaction transaction)
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
			columns[2].DefineColumnClass (DbColumnClass.KeyStatus);
			columns[4].DefineColumnLocalisation (DbColumnLocalisation.Default);
			columns[5].DefineColumnLocalisation (DbColumnLocalisation.Default);
			columns[7].DefineColumnClass (DbColumnClass.RefLiveId);
			
			this.SetCategory (columns, DbElementCat.Internal);
			
			table.DefineCategory (DbElementCat.Internal);
			table.Columns.AddRange (columns);
			
			table.PrimaryKeys.Add (columns[0]);
			table.PrimaryKeys.Add (columns[1]);
			
			this.internal_tables.Add (table);
			
			SqlTable sql_table = table.CreateSqlTable (this.type_converter);
			this.sql_builder.InsertTable (sql_table);
			this.ExecuteSilent (transaction);
		}
		
		protected void BootCreateTableRelationDef(DbTransaction transaction)
		{
			DbTable    table   = new DbTable (DbTable.TagRelationDef);
			DbColumn[] columns = new DbColumn[4];
			
			columns[0] = new DbColumn (DbColumn.TagId,			this.num_type_id);
			columns[1] = new DbColumn (DbColumn.TagRefColumn,	this.num_type_id);
			columns[2] = new DbColumn (DbColumn.TagRefSource,	this.num_type_id);
			columns[3] = new DbColumn (DbColumn.TagRefTarget,	this.num_type_id);
			
			columns[0].DefineColumnClass (DbColumnClass.KeyId);
			columns[1].DefineColumnClass (DbColumnClass.RefLiveId);
			columns[2].DefineColumnClass (DbColumnClass.RefLiveId);
			columns[3].DefineColumnClass (DbColumnClass.RefLiveId);
			
			this.SetCategory (columns, DbElementCat.Internal);
			
			table.DefineCategory (DbElementCat.Internal);
			table.Columns.AddRange (columns);
			
			table.PrimaryKeys.Add (columns[0]);
			
			this.internal_tables.Add (table);
			
			SqlTable sql_table = table.CreateSqlTable (this.type_converter);
			this.sql_builder.InsertTable (sql_table);
			this.ExecuteSilent (transaction);
		}
		
		
		protected void BootInsertTypeDefRow(DbTransaction transaction, DbType type)
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
			
			this.sql_builder.InsertData (type_def.CreateSqlName (), fields);
			this.ExecuteSilent (transaction);
		}
		
		protected void BootInsertEnumValueDefRow(DbTransaction transaction, DbType type, DbEnumValue value)
		{
			DbTable enum_def = this.internal_tables[DbTable.TagEnumValDef];
			
			//	Phase d'initialisation de la base : insère une ligne dans la table de définition des
			//	énumérations. Les colonnes descriptives (pour l'utilisateur) ne sont pas initialisées.
			
			SqlFieldCollection fields = new SqlFieldCollection ();
			
			fields.Add (enum_def.Columns[DbColumn.TagId]		.CreateSqlField (this.type_converter, value.InternalKey.Id));
			fields.Add (enum_def.Columns[DbColumn.TagRevision]	.CreateSqlField (this.type_converter, value.InternalKey.Revision));
			fields.Add (enum_def.Columns[DbColumn.TagStatus]	.CreateSqlField (this.type_converter, value.InternalKey.RawStatus));
			fields.Add (enum_def.Columns[DbColumn.TagName]		.CreateSqlField (this.type_converter, value.Name));
			fields.Add (enum_def.Columns[DbColumn.TagInfoXml]	.CreateSqlField (this.type_converter, DbEnumValue.ConvertValueToXml (value)));
			fields.Add (enum_def.Columns[DbColumn.TagRefType]	.CreateSqlField (this.type_converter, type.InternalKey.Id));
			
			this.sql_builder.InsertData (enum_def.CreateSqlName (), fields);
			this.ExecuteSilent (transaction);
		}
		
		protected void BootInsertTableDefRow(DbTransaction transaction, DbTable table)
		{
			DbTable table_def = this.internal_tables[DbTable.TagTableDef];
			
			//	Phase d'initialisation de la base : insère une ligne dans la table de définition des
			//	tables. Les colonnes descriptives (pour l'utilisateur) ne sont pas initialisées.
			
			SqlFieldCollection fields = new SqlFieldCollection ();
			
			fields.Add (table_def.Columns[DbColumn.TagId]		.CreateSqlField (this.type_converter, table.InternalKey.Id));
			fields.Add (table_def.Columns[DbColumn.TagRevision]	.CreateSqlField (this.type_converter, table.InternalKey.Revision));
			fields.Add (table_def.Columns[DbColumn.TagStatus]	.CreateSqlField (this.type_converter, table.InternalKey.RawStatus));
			fields.Add (table_def.Columns[DbColumn.TagName]		.CreateSqlField (this.type_converter, table.Name));
			fields.Add (table_def.Columns[DbColumn.TagInfoXml]	.CreateSqlField (this.type_converter, DbTable.ConvertTableToXml (table)));
			fields.Add (table_def.Columns[DbColumn.TagNextId]	.CreateSqlField (this.type_converter, 0));
			
			this.sql_builder.InsertData (table_def.CreateSqlName (), fields);
			this.ExecuteSilent (transaction);
		}
		
		protected void BootUpdateTableNextId(DbTransaction transaction, DbKey key, long next_id)
		{
			SqlFieldCollection fields = new SqlFieldCollection ();
			SqlFieldCollection conds  = new SqlFieldCollection ();
			
			fields.Add (DbColumn.TagNextId, SqlField.CreateConstant (next_id, DbRawType.Int64));
			
			conds.Add (new SqlFunction (SqlFunctionType.CompareEqual, SqlField.CreateName (DbColumn.TagId), SqlField.CreateConstant (key.Id, DbRawType.Int64)));
			conds.Add (new SqlFunction (SqlFunctionType.CompareEqual, SqlField.CreateName (DbColumn.TagRevision), SqlField.CreateConstant (key.Revision, DbRawType.Int32)));
			
			System.Diagnostics.Debug.WriteLine (string.Format ("Table {0}, next ID will be {1}.", key, next_id));
			
			this.sql_builder.UpdateData (DbTable.TagTableDef, fields, conds);
			this.ExecuteSilent (transaction);
		}
		
		protected void BootInsertColumnDefRow(DbTransaction transaction, DbTable table, DbColumn column)
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
			
			this.sql_builder.InsertData (column_def.CreateSqlName (), fields);
			this.ExecuteSilent (transaction);
		}
		
		protected void BootInsertRelationDefRow(DbTransaction transaction, int ref_id, string src_table_name, string src_column_name, string target_table_name)
		{
			DbTable ref_def = this.internal_tables[DbTable.TagRelationDef];
			
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
			
			this.sql_builder.InsertData (ref_def.CreateSqlName (), fields);
			this.ExecuteSilent (transaction);
		}
		
		
		protected void BootSetupTables(DbTransaction transaction)
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
				this.BootInsertTypeDefRow (transaction, type);
			}
			
			foreach (DbTable table in this.internal_tables)
			{
				//	Attribue à chaque table interne une clef unique et établit les informations de base
				//	dans la table de définition des tables.
				
				table.DefineInternalKey (new DbKey (table_key_id++));
				table.UpdatePrimaryKeyInfo ();
				
				this.BootInsertTableDefRow (transaction, table);
				
				foreach (DbColumn column in table.Columns)
				{
					//	Pour chaque colonne de la table, établit les informations de base dans la table de
					//	définition des colonnes.
					
					column.DefineInternalKey (new DbKey (column_key_id++));
					this.BootInsertColumnDefRow (transaction, table, column);
				}
			}
			
			//	Complète encore les informations au sujet des relations :
			//
			//	- La description d'une colonne fait référence à la table et à un type.
			//	- La description d'une valeur d'enum fait référence à un type.
			//	- La description d'une référence fait elle-même référence à la table
			//	  source et destination, ainsi qu'à la colonne.
			
			this.BootInsertRelationDefRow (transaction, ref_key_id++, DbTable.TagColumnDef,   DbColumn.TagRefTable,  DbTable.TagTableDef);
			this.BootInsertRelationDefRow (transaction, ref_key_id++, DbTable.TagColumnDef,   DbColumn.TagRefType,   DbTable.TagTypeDef);
			this.BootInsertRelationDefRow (transaction, ref_key_id++, DbTable.TagEnumValDef,  DbColumn.TagRefType,   DbTable.TagTypeDef);
			this.BootInsertRelationDefRow (transaction, ref_key_id++, DbTable.TagRelationDef, DbColumn.TagRefColumn, DbTable.TagColumnDef);
			this.BootInsertRelationDefRow (transaction, ref_key_id++, DbTable.TagRelationDef, DbColumn.TagRefSource, DbTable.TagTableDef);
			this.BootInsertRelationDefRow (transaction, ref_key_id++, DbTable.TagRelationDef, DbColumn.TagRefTarget, DbTable.TagTableDef);
			
			this.BootUpdateTableNextId (transaction, this.internal_tables[DbTable.TagTableDef].InternalKey, table_key_id);
			this.BootUpdateTableNextId (transaction, this.internal_tables[DbTable.TagColumnDef].InternalKey, column_key_id);
			this.BootUpdateTableNextId (transaction, this.internal_tables[DbTable.TagTypeDef].InternalKey, type_key_id);
			this.BootUpdateTableNextId (transaction, this.internal_tables[DbTable.TagRelationDef].InternalKey, ref_key_id);
			this.BootUpdateTableNextId (transaction, this.internal_tables[DbTable.TagEnumValDef].InternalKey, enum_val_key_id);
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
			
			this.num_type_id       = new DbTypeNum (DbNumDef.FromRawType (DbRawType.Int32), Tags.Name + "=" + DbType.TagKeyId);
			this.num_type_revision = new DbTypeNum (DbNumDef.FromRawType (DbRawType.Int32), Tags.Name + "=" + DbType.TagKeyRevision);
			this.num_type_status   = new DbTypeNum (DbNumDef.FromRawType (DbRawType.Int32), Tags.Name + "=" + DbType.TagKeyStatus);
			
			this.internal_types.Add (this.num_type_id);
			this.internal_types.Add (this.num_type_revision);
			this.internal_types.Add (this.num_type_status);
		}
		
		protected void InitialiseStrTypes()
		{
			this.str_type_name        = new DbTypeString (DbColumn.MaxNameLength, false,		Tags.Name + "=" + DbType.TagName);
			this.str_type_caption     = new DbTypeString (DbColumn.MaxCaptionLength, false,		Tags.Name + "=" + DbType.TagCaption);
			this.str_type_description = new DbTypeString (DbColumn.MaxDescriptionLength, false, Tags.Name + "=" + DbType.TagDescription);
			this.str_type_info_xml    = new DbTypeString (DbColumn.MaxInfoXmlLength, false,		Tags.Name + "=" + DbType.TagInfoXml);
			
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
		
		protected DbTransaction			live_transaction;
	}
}
