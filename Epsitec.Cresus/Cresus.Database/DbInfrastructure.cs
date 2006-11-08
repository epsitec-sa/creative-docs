//	Copyright © 2003-2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Cresus.Database
{
	using InvariantConverter = Epsitec.Common.Types.InvariantConverter;
	
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
			this.localisations     = new string[] { "", "FR" };
			this.live_transactions = new System.Collections.Hashtable (20, 0.2f);
			this.release_requested = new System.Collections.ArrayList ();
		}
		
		
		public CallbackDisplayDataSet			DisplayDataSet
		{
			get
			{
				return this.displayDataSet;
			}
			set
			{
				this.displayDataSet = value;
			}
		}
		
		public ISqlBuilder						DefaultSqlBuilder
		{
			get
			{
				return this.db_abstraction.SqlBuilder;
			}
		}
		
		public ISqlEngine						DefaultSqlEngine
		{
			get
			{
				return this.sql_engine;
			}
		}
		
		public IDbAbstraction					DefaultDbAbstraction
		{
			get
			{
				return this.db_abstraction;
			}
		}
		
		public ITypeConverter					TypeConverter
		{
			get
			{
				return this.type_converter;
			}
		}
		
		public DbTransaction[]					LiveTransactions
		{
			get
			{
				lock (this.live_transactions)
				{
					DbTransaction[] transactions = new DbTransaction[this.live_transactions.Count];
					this.live_transactions.Values.CopyTo (transactions, 0);
					return transactions;
				}
			}
		}
		
		public DbLogger							Logger
		{
			get
			{
				return this.logger;
			}
		}
		
		public DbClientManager					ClientManager
		{
			get
			{
				if (this.client_manager == null)
				{
					if (this.LocalSettings.IsServer)
					{
						this.client_manager = new DbClientManager ();
						
						using (DbTransaction transaction = this.BeginTransaction (DbTransactionMode.ReadOnly))
						{
							this.client_manager.Attach (this, this.ResolveDbTable (transaction, Tags.TableClientDef));
							this.client_manager.LoadFromBase (transaction);
							transaction.Commit ();
						}
					}
				}
				
				return this.client_manager;
			}
		}
		
		public bool								IsInGlobalLock
		{
			get
			{
				return this.global_lock.IsWriterLockHeld;
			}
		}
		
		public Settings.Globals					GlobalSettings
		{
			get
			{
				return this.globals;
			}
		}
		
		public Settings.Locals					LocalSettings
		{
			get
			{
				return this.locals;
			}
		}
		
		
		public void CreateDatabase(DbAccess db_access)
		{
//-			System.Diagnostics.Debug.WriteLine ("Connect to DEBUGGER now");
//-			System.Threading.Thread.Sleep (30*1000);
			
			//	Crée une base de données avec les structures de gestion requises par Crésus
			//	(tables de description, etc.).
			
			if (this.db_access.IsValid)
			{
				throw new Exceptions.GenericException (this.db_access, "A database already exists for this DbInfrastructure.");
			}
			
			this.db_access = db_access;
			this.db_access.CreateDatabase = true;

			this.InitializeDatabaseAbstraction ();
			
			this.types.RegisterTypes ();
			
			//	La base de données vient d'être créée. Elle est donc toute vide (aucune
			//	table n'est encore définie).
			
			System.Diagnostics.Debug.Assert (this.db_abstraction.QueryUserTableNames ().Length == 0);
			
			//	Il faut créer les tables internes utilisées pour la gestion des méta-données.
			
			using (DbTransaction transaction = this.BeginTransaction (DbTransactionMode.ReadWrite))
			{
				BootHelper helper = new BootHelper (this, transaction);
				
				helper.CreateTableTableDef ();
				helper.CreateTableColumnDef ();
				helper.CreateTableTypeDef ();
				helper.CreateTableEnumValDef ();
				helper.CreateTableLog ();
				helper.CreateTableRequestQueue ();
				helper.CreateTableClientDef ();
				
				//	Valide la création de toutes ces tables avant de commencer à peupler
				//	les tables. Firebird requiert ce mode de fonctionnement.
				
				transaction.Commit ();
			}
			
			//	Les tables de description ont toutes été créées. Il faut maintenant les remplir
			//	avec les informations de initiales.
			
			using (DbTransaction transaction = this.BeginTransaction (DbTransactionMode.ReadWrite))
			{
				//	TODO: gérer client_id
				
				this.client_id = 1;
				
				this.SetupTables (transaction);
				
				//	Crée aussi les réglages globaux et locaux, ce qui va créer des tables dans
				//	la base, donc nécessiter une validation de transaction avant que celles-ci
				//	ne soient accessibles en écriture :
				
				Settings.Globals.CreateTable (this, transaction, Settings.Globals.Name, DbElementCat.Internal, DbRevisionMode.Disabled, DbReplicationMode.Shared);
				Settings.Locals.CreateTable (this, transaction, Settings.Locals.Name, DbElementCat.Internal, DbRevisionMode.Disabled, DbReplicationMode.Private);
				
				transaction.Commit ();
			}
			
			//	Crée les valeurs par défaut dans les réglages (Globals et Locals) :
			
			using (DbTransaction transaction = this.BeginTransaction (DbTransactionMode.ReadWrite))
			{
				Settings.Globals globals = new Settings.Globals (this, transaction);
				Settings.Locals  locals  = new Settings.Locals (this, transaction);
				
				globals.PersistToBase (transaction);
				
				locals.ClientId = this.client_id;
				locals.IsServer = false;			//	TODO: gérer IsServer
				locals.PersistToBase (transaction);
				
				globals = null;
				locals  = null;

				transaction.Commit ();
			}
			
			this.StartUsingDatabase ();
		}
		
		public void AttachDatabase(DbAccess db_access)
		{
			if (this.db_access.IsValid)
			{
				throw new Exceptions.GenericException (this.db_access, "Database already attached");
			}
			
			this.db_access = db_access;
			this.db_access.CreateDatabase = false;

			this.InitializeDatabaseAbstraction ();
			
			System.Diagnostics.Debug.Assert (this.db_abstraction.QueryUserTableNames ().Length > 0);
			
			using (DbTransaction transaction = this.BeginTransaction (DbTransactionMode.ReadOnly))
			{
				this.internal_tables.Add (this.ResolveDbTable (transaction, Tags.TableLog));
				this.internal_tables.Add (this.ResolveDbTable (transaction, Tags.TableTableDef));
				this.internal_tables.Add (this.ResolveDbTable (transaction, Tags.TableColumnDef));
				this.internal_tables.Add (this.ResolveDbTable (transaction, Tags.TableTypeDef));
				this.internal_tables.Add (this.ResolveDbTable (transaction, Tags.TableEnumValDef));
				this.internal_tables.Add (this.ResolveDbTable (transaction, Tags.TableClientDef));
				this.internal_tables.Add (this.ResolveDbTable (transaction, Tags.TableRequestQueue));
				
				this.types.ResolveTypes (transaction);
				
				transaction.Commit ();
			}
			
			this.StartUsingDatabase ();
		}
		
		public void SetupRoamingDatabase(int client_id)
		{
			if (this.client_id != client_id)
			{
				using (DbTransaction transaction = this.BeginTransaction ())
				{
					//	Prend note du dernier ID stocké dans le LOG; il sert à définir le ID actif
					//	au moment de la synchronisation (puisqu'on part d'une 'image' de la base,
					//	ça revient à considérer qu'on a fait une synchronisation).
					
					DbId last_server_id = this.logger.CurrentId;
					
					//	Les prochains IDs affectés aux diverses lignes des diverses tables vont
					//	tous commencer à 1, combiné avec notre ID de client.
					
					this.ResetIdColumn (transaction, this.internal_tables[Tags.TableTableDef], Tags.ColumnNextId, DbId.CreateId (1, client_id));
					
					//	Vide les tables des requêtes et des clients, qui ne sont normalement pas
					//	répliquées :
					
					this.ClearTable (transaction, this.internal_tables[Tags.TableRequestQueue]);
					this.ClearTable (transaction, this.internal_tables[Tags.TableClientDef]);
					
					//	Met à jour le Logger afin d'utiliser dès à présent notre ID de client :
					
					this.logger.Detach ();
					
					this.logger = new DbLogger ();
					this.logger.DefineClientId (client_id);
					this.logger.DefineLogId (1);
					this.logger.Attach (this, this.internal_tables[Tags.TableLog]);
					this.logger.CreateInitialEntry (transaction);
					
					//	Adapte les divers réglages locaux en fonction du client :
					
					this.LocalSettings.ClientId  = client_id;
					this.LocalSettings.IsServer  = false;
					this.LocalSettings.SyncLogId = last_server_id.Value;
					
					this.LocalSettings.PersistToBase (transaction);
					
					//	...et dès maintenant, nous sommes prêts à travailler !
					
					transaction.Commit ();
					
					this.client_id = client_id;
				}
			}
		}
		
		protected void ClearTable(DbTransaction transaction, DbTable table)
		{
			transaction.SqlBuilder.RemoveData (table.CreateSqlName (), null);
			this.ExecuteNonQuery (transaction);
		}
		
		protected void ResetIdColumn(DbTransaction transaction, DbTable table, string columnName, DbId id)
		{
			DbColumn column     = table.Columns[columnName];
			string   sql_column = column.CreateSqlName ();
			
			Collections.SqlFields fields = new Collections.SqlFields ();
			
			fields.Add (sql_column, SqlField.CreateConstant (id.Value, DbKey.RawTypeForId));
			
			transaction.SqlBuilder.UpdateData (table.CreateSqlName (), fields, null);
			this.ExecuteNonQuery (transaction);
		}
		
		public void ReleaseConnection()
		{
			this.ReleaseConnection (this.db_abstraction);
		}
		
		public void ReleaseConnection(IDbAbstraction db_abstraction)
		{
			lock (this.live_transactions)
			{
				if (this.live_transactions.ContainsKey (db_abstraction))
				{
					this.release_requested.Add (db_abstraction);
				}
				else
				{
					db_abstraction.ReleaseConnection ();
				}
			}
		}


		public static DbAccess CreateDbAccess(string name)
		{
			return new DbAccess ("Firebird", name, "localhost", "sysdba", "masterkey", false);
		}

		public static DbAccess CreateDbAccess(string provider, string name)
		{
			return new DbAccess (provider, name, "localhost", "sysdba", "masterkey", false);
		}
		
		
		public IDbAbstraction CreateDbAbstraction()
		{
			IDbAbstraction db_abstraction = DbFactory.FindDbAbstraction (this.db_access);
			
			db_abstraction.SqlBuilder.AutoClear = true;
			
			return db_abstraction;
		}
		
		
		public DbTransaction BeginTransaction()
		{
			return this.BeginTransaction (DbTransactionMode.ReadWrite);
		}
		
		public DbTransaction BeginTransaction(DbTransactionMode mode)
		{
			return this.BeginTransaction (mode, this.db_abstraction);
		}
		
		public DbTransaction BeginTransaction(DbTransactionMode mode, IDbAbstraction db_abstraction)
		{
			System.Diagnostics.Debug.Assert (db_abstraction != null);
			
			//	Débute une nouvelle transaction. Ceci n'est possible que si aucune
			//	autre transaction n'est actuellement en cours sur cette connexion.
			
			DbTransaction transaction = null;
			
			this.DatabaseLock (db_abstraction);
			
			try
			{
				switch (mode)
				{
					case DbTransactionMode.ReadOnly:
						transaction = new DbTransaction (db_abstraction.BeginReadOnlyTransaction (), db_abstraction, this, mode);
						break;
					
					case DbTransactionMode.ReadWrite:
						transaction = new DbTransaction (db_abstraction.BeginReadWriteTransaction (), db_abstraction, this, mode);
						break;
					
					default:
						throw new System.ArgumentOutOfRangeException ("mode", mode, string.Format ("Transaction mode {0} not accepted.", mode.ToString ()));
				}
			}
			catch
			{
				this.DatabaseUnlock (db_abstraction);
				throw;
			}
			
			return transaction;
		}
		
		
		public DbTable   CreateDbTable(string name, DbElementCat category, DbRevisionMode revision_mode)
		{
			//	Crée la description d'une table qui ne contient que le strict minimum nécessaire au fonctionnement
			//	de Crésus (tuple pour la clef primaire, statut). Il faudra compléter les colonnes en fonction des
			//	besoins, puis appeler la méthode RegisterNewDbTable.
			
			switch (category)
			{
				case DbElementCat.Internal:
					throw new Exceptions.GenericException (this.db_access, string.Format ("User may not create internal table. Table '{0}'.", name));
				
				case DbElementCat.UserDataManaged:
					return this.CreateTable(name, category, revision_mode, DbReplicationMode.Shared);
				
				default:
					throw new Exceptions.GenericException (this.db_access, string.Format ("Unsupported category {0} specified. Table '{1}'.", category, name));
			}
		}
		
		public void      RegisterNewDbTable(DbTransaction transaction, DbTable table)
		{
			this.RegisterDbTable (transaction, table, false);
		}
		
		public void      RegisterKnownDbTable(DbTransaction transaction, DbTable table)
		{
			this.RegisterDbTable (transaction, table, true);
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
			
			DbKey old_key = table.Key;
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
			if (key.IsEmpty)
			{
				return null;
			}

			lock (this.cache_db_tables)
			{
				DbTable table = this.cache_db_tables[key];
				
				if (table == null)
				{
					List<DbTable> tables = this.LoadDbTable (transaction, key, DbRowSearchMode.LiveActive);
					
					if (tables.Count > 0)
					{
						table = tables[0];
						
//-						System.Diagnostics.Debug.WriteLine (string.Format ("Loaded {0} {1} from database.", table.GetType ().Name, table.Name));
						System.Diagnostics.Debug.Assert (tables.Count == 1);
						System.Diagnostics.Debug.Assert (table.Key == key);
					}
				}
				
				return table;
			}
		}
		
		public DbTable[] FindDbTables(DbTransaction transaction, DbElementCat category)
		{
			return this.FindDbTables (transaction, category, DbRowSearchMode.LiveActive);
		}
		
		public DbTable[] FindDbTables(DbTransaction transaction, DbElementCat category, DbRowSearchMode row_search_mode)
		{
			//	Liste toutes les tables appartenant à la catégorie spécifiée.
			
			List<DbTable> list = this.LoadDbTable (transaction, DbKey.Empty, row_search_mode);
			
			if (category != DbElementCat.Any)
			{
				for (int i = 0; i < list.Count; )
				{
					DbTable table = list[i];
					
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
		
		
		public void ClearCaches()
		{
			lock (this.cache_db_tables)
			{
				this.cache_db_tables.ClearCache ();
			}
			lock (this.cache_db_types)
			{
				this.cache_db_types.ClearCache ();
			}
		}
		
		public DbColumn CreateColumn(string columnName, DbTypeDef typeDef)
		{
			return new DbColumn (columnName, typeDef, DbColumnClass.Data, DbElementCat.UserDataManaged);
		}
		
		public DbColumn[] CreateLocalisedColumns(string columnName, DbTypeDef typeDef)
		{
			//	TODO: crée la ou les colonnes localisées
			
			//	Note: utilise DbColumnClass.Data et DbElementCat.UserDataManaged pour ces
			//	colonnes, puisqu'elles appartiennent à l'utilisateur.
			
			throw new System.NotImplementedException ("CreateLocalisedColumns not implemented.");
		}
		
		public DbColumn[] CreateRefColumns(string columnName, string targetTableName)
		{
			//	Crée la ou les colonnes nécessaires à la définition d'une référence à une autre
			//	table.
			
			DbTypeDef typeDef = this.internal_types[Tags.TypeKeyId];
			return new DbColumn[] { this.CreateRefColumn (columnName, targetTableName, typeDef) };
		}

		public DbColumn CreateRefColumn(string columnName, string targetTableName, DbTypeDef type)
		{
			System.Diagnostics.Debug.Assert (type != null);
			System.Diagnostics.Debug.Assert (!string.IsNullOrEmpty (targetTableName));

			DbColumn column = new DbColumn (columnName, type, DbColumnClass.RefId, DbElementCat.UserDataManaged);

			column.DefineTargetTableName (targetTableName);

			return column;
		}

		public DbColumn CreateUserDataColumn(string columnName, DbTypeDef type)
		{
			System.Diagnostics.Debug.Assert (type != null);

			return new DbColumn (columnName, type, DbColumnClass.Data, DbElementCat.UserDataManaged);
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
					case DbColumnClass.RefId:
						
						string parent_name = column.TargetTableName;
						
						if (parent_name != null)
						{
							DbTable parent_table = this.ResolveDbTable (transaction, parent_name);
							
							if (parent_table == null)
							{
								string message = string.Format ("Table '{0}' referenced from '{1}.{2}' not found in database.", parent_name, table.Name, column.Name);
								throw new Exceptions.GenericException (this.db_access, message);
							}
							
							DbKey source_table_key  = table.Key;
							DbKey source_column_key = column.Key;
							DbKey parent_table_key  = parent_table.Key;
							
							if (source_table_key.IsEmpty)
							{
								string message = string.Format ("Reference of '{0}' from '{1}.{2}' specifies unregistered table '{1}'.", parent_name, table.Name, column.Name);
								throw new Exceptions.GenericException (this.db_access, message);
							}

							if (source_column_key.IsEmpty)
							{
								string message = string.Format ("Reference of '{0}' from '{1}.{2}' specifies unregistered column '{2}'.", parent_name, table.Name, column.Name);
								throw new Exceptions.GenericException (this.db_access, message);
							}

							if (parent_table_key.IsEmpty)
							{
								string message = string.Format ("Reference of '{0}' from '{1}.{2}' specifies unregistered table '{0}'.", parent_name, table.Name, column.Name);
								throw new Exceptions.GenericException (this.db_access, message);
							}
							
							this.UpdateColumnRelation (transaction, source_table_key, source_column_key, parent_table_key);
						}
						break;
					
					default:
						break;
				}
			}
		}


		public void      RegisterNewDbType(DbTransaction transaction, DbTypeDef typeDef)
		{
			//	Enregistre un nouveau type dans la base de données. Ceci va attribuer au
			//	type une clef DbKey et vérifier qu'il n'y a pas de collision avec un
			//	éventuel type déjà existant.
			
			if (transaction == null)
			{
				using (transaction = this.BeginTransaction ())
				{
					this.RegisterNewDbType (transaction, typeDef);
					transaction.Commit ();
					return;
				}
			}
			
			this.CheckForUnknownType (transaction, typeDef);

			long table_id = this.NewRowIdInTable (transaction, this.internal_tables[Tags.TableTypeDef].Key, 1);

#if false
			//	TODO: handle enumeration types here:
			
			DbTypeEnum type_enum = typeDef as DbTypeEnum;
			
			long enum_id  = (type_enum == null) ? 0 : this.NewRowIdInTable (transaction, this.internal_tables[Tags.TableEnumValDef].Key, type_enum.Count);
#endif

			//	Crée la ligne de description du type :
			
			typeDef.DefineKey (new DbKey (table_id));
			this.InsertTypeDefRow (transaction, typeDef);
#if false
			if (type_enum != null)
			{
				//	Crée les lignes de description des valeurs de l'énumération :
				
				int i = 0;
				
				foreach (DbEnumValue value in type_enum.Values)
				{
					value.DefineInternalKey (new DbKey (enum_id + i));
					this.InsertEnumValueDefRow (transaction, type, value);
					i++;
				}
			}
#endif
		}
		
		public void      UnregisterDbType(DbTransaction transaction, DbTypeDef typeDef)
		{
			//	Supprime la description du type de la base. Pour des raisons de sécurité,
			//	le type SQL n'est pas réellement supprimé.
			
			if (transaction == null)
			{
				using (transaction = this.BeginTransaction ())
				{
					this.UnregisterDbType (transaction, typeDef);
					transaction.Commit ();
					return;
				}
			}
			
			this.CheckForKnownType (transaction, typeDef);
			
			DbKey old_key = typeDef.Key;
			DbKey new_key = new DbKey (old_key.Id, DbRowStatus.Deleted);
			
			this.UpdateKeyInRow (transaction, Tags.TableTypeDef, old_key, new_key);
		}
		
		public DbTypeDef ResolveDbType(DbTransaction transaction, string type_name)
		{
			DbKey key = this.FindDbTypeKey (transaction, type_name);
			return this.ResolveDbType (transaction, key);
		}
		
		public DbTypeDef  ResolveDbType(DbTransaction transaction, DbKey key)
		{
			if (key.IsEmpty)
			{
				return null;
			}

			//	Trouve le type corredpondant à une clef spécifique.
			
			lock (this.cache_db_types)
			{
				DbTypeDef typeDef = this.cache_db_types[key];
				
				if (typeDef == null)
				{
					List<DbTypeDef> types = this.LoadDbType (transaction, key, DbRowSearchMode.LiveActive);
					
					if (types.Count > 0)
					{
						typeDef = types[0] as DbTypeDef;
						
//-						System.Diagnostics.Debug.WriteLine (string.Format ("Loaded {0} {1} from database.", type.GetType ().Name, type.Name));
						System.Diagnostics.Debug.Assert (types.Count == 1);
						System.Diagnostics.Debug.Assert (typeDef.Key == key);
						
						this.cache_db_types[key] = typeDef;
					}
				}
				
				return typeDef;
			}
		}
		
		public DbTypeDef[]  FindDbTypes(DbTransaction transaction)
		{
			return this.FindDbTypes (transaction, DbRowSearchMode.LiveActive);
		}
		
		public DbTypeDef[]  FindDbTypes(DbTransaction transaction, DbRowSearchMode row_search_mode)
		{
			//	Liste tous les types.
			
			return this.LoadDbType (transaction, DbKey.Empty, row_search_mode).ToArray ();
		}
		
		
		
		internal DbTable CreateTable(string name, DbElementCat category, DbRevisionMode revision_mode, DbReplicationMode replication_mode)
		{
			System.Diagnostics.Debug.Assert (revision_mode != DbRevisionMode.Unknown);
			
			DbTable table = new DbTable (name);
			
			DbTypeDef typeDef = this.internal_types[Tags.TypeKeyId];
			
			DbColumn col_id   = new DbColumn (Tags.ColumnId,     this.internal_types[Tags.TypeKeyId],     DbColumnClass.KeyId, DbElementCat.Internal);
			DbColumn col_stat = new DbColumn (Tags.ColumnStatus, this.internal_types[Tags.TypeKeyStatus], DbColumnClass.KeyStatus, DbElementCat.Internal);
			DbColumn col_log  = new DbColumn (Tags.ColumnRefLog, this.internal_types[Tags.TypeKeyId],     DbColumnClass.RefInternal, DbElementCat.Internal);
			
			table.DefineCategory (category);
			table.DefineRevisionMode (revision_mode);
			table.DefineReplicationMode (replication_mode);
			
			table.Columns.Add (col_id);
			table.Columns.Add (col_stat);
			table.Columns.Add (col_log);
			
			table.PrimaryKeys.Add (col_id);
			
			return table;
		}
		
		
		protected void RegisterDbTable(DbTransaction transaction, DbTable table, bool check_for_known)
		{
			//	Enregistre une nouvelle table dans la base de données. Ceci va attribuer à
			//	la table une clef DbKey et vérifier qu'il n'y a pas de collision avec une
			//	éventuelle table déjà existante. Cela va aussi attribuer des colonnes pour
			//	la nouvelle table.
			
			if (transaction == null)
			{
				using (transaction = this.BeginTransaction ())
				{
					this.RegisterDbTable (transaction, table, check_for_known);
					transaction.Commit ();
					return;
				}
			}
			
			this.CheckForRegisteredTypes (transaction, table);
			
			if (check_for_known)
			{
				this.CheckForKnownTable (transaction, table);
			}
			else
			{
				this.CheckForUnknownTable (transaction, table);
				
				long table_id  = this.NewRowIdInTable (transaction, this.internal_tables[Tags.TableTableDef] .Key, 1);
				long column_id = this.NewRowIdInTable (transaction, this.internal_tables[Tags.TableColumnDef].Key, table.Columns.Count);
				
				//	Crée la ligne de description de la table :
				
				table.DefineKey (new DbKey (table_id));
				table.UpdatePrimaryKeyInfo ();
				
				this.InsertTableDefRow (transaction, table);
				
				//	Crée les lignes de description des colonnes :
				
				for (int i = 0; i < table.Columns.Count; i++)
				{
					table.Columns[i].DefineKey (new DbKey (column_id + i));
					this.InsertColumnDefRow (transaction, table, table.Columns[i]);
				}
			}
			
			//	Finalement, il faut créer la table elle-même :
			
			SqlTable sql_table = table.CreateSqlTable (this.type_converter);
			
			transaction.SqlBuilder.InsertTable (sql_table);
			this.ExecuteSilent (transaction);
		}
		
		
		protected void CheckForRegisteredTypes(DbTransaction transaction, DbTable table)
		{
			//	Vérifie que tous les types utilisés dans la définition des colonnes sont bien
			//	connus (on vérifie qu'ils ont une clef valide).
			
			Collections.DbColumns columns = table.Columns;
			
			for (int i = 0; i < columns.Count; i++)
			{
				DbTypeDef typeDef = columns[i].Type;
				
				System.Diagnostics.Debug.Assert (typeDef != null);
				
				if (typeDef.Key.IsEmpty)
				{
					string message = string.Format ("Unregistered type '{0}' used in table '{1}', column '{2}'.",
						/**/						typeDef.Name, table.Name, columns[i].Name);
					
					throw new Exceptions.GenericException (this.db_access, message);
				}
			}
		}
		
		protected void CheckForUnknownType(DbTransaction transaction, DbTypeDef typeDef)
		{
			System.Diagnostics.Debug.Assert (typeDef != null);
			
			if (this.CountMatchingRows (transaction, Tags.TableTypeDef, Tags.ColumnName, typeDef.Name) > 0)
			{
				string message = string.Format ("Type {0} already exists in database.", typeDef.Name);
				throw new Exceptions.GenericException (this.db_access, message);
			}
		}
		
		protected void CheckForKnownType(DbTransaction transaction, DbTypeDef typeDef)
		{
			System.Diagnostics.Debug.Assert (typeDef != null);
			
			if (this.CountMatchingRows (transaction, Tags.TableTypeDef, Tags.ColumnName, typeDef.Name) == 0)
			{
				string message = string.Format ("Type {0} does not exist in database.", typeDef.Name);
				throw new Exceptions.GenericException (this.db_access, message);
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
			
			if (this.CountMatchingRows (transaction, Tags.TableTableDef, Tags.ColumnName, table.Name) > 0)
			{
				string message = string.Format ("Table {0} already exists in database.", table.Name);
				throw new Exceptions.GenericException (this.db_access, message);
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
			
			if (this.CountMatchingRows (transaction, Tags.TableTableDef, Tags.ColumnName, table.Name) == 0)
			{
				string message = string.Format ("Table {0} does not exist in database.", table.Name);
				throw new Exceptions.GenericException (this.db_access, message);
			}
		}
		
		
		public void GlobalLock()
		{
			this.global_lock.AcquireWriterLock (this.lock_timeout);
		}
		
		public void GlobalUnlock()
		{
			this.global_lock.ReleaseWriterLock ();
		}
		
		
		internal void DatabaseLock(IDbAbstraction database)
		{
			this.global_lock.AcquireReaderLock (this.lock_timeout);
			
			if (System.Threading.Monitor.TryEnter (database, this.lock_timeout) == false)
			{
				this.global_lock.ReleaseReaderLock ();
				throw new Exceptions.DeadLockException (this.db_access, "Cannot lock database.");
			}
		}
		
		internal void DatabaseUnlock(IDbAbstraction database)
		{
			this.global_lock.ReleaseReaderLock ();
			System.Threading.Monitor.Exit (database);
		}
		
		
		internal void NotifyBeginTransaction(DbTransaction transaction)
		{
			IDbAbstraction db_abstraction = transaction.Database;
			
			lock (this.live_transactions)
			{
				if (this.live_transactions.ContainsKey (db_abstraction))
				{
					throw new Exceptions.GenericException (this.db_access, string.Format ("Nested transactions not supported."));
				}
				
				this.live_transactions[db_abstraction] = transaction;
			}
		}
		
		internal void NotifyEndTransaction(DbTransaction transaction)
		{
			IDbAbstraction db_abstraction = transaction.Database;
			
			this.DatabaseUnlock (db_abstraction);
			
			lock (this.live_transactions)
			{
				if (this.live_transactions[db_abstraction] != transaction)
				{
					throw new Exceptions.GenericException (this.db_access, string.Format ("Ending wrong transaction."));
				}
				
				this.live_transactions.Remove (db_abstraction);
				
				if (this.release_requested.Contains (db_abstraction))
				{
					this.release_requested.Remove (db_abstraction);
					this.ReleaseConnection (db_abstraction);
				}
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
			
			this.sql_engine.Execute (command, this, transaction.Transaction);
		}
		
		
		public int ExecuteSilent(DbTransaction transaction)
		{
			if (transaction == null)
			{
				using (transaction = this.BeginTransaction ())
				{
					int result = this.ExecuteSilent (transaction, transaction.SqlBuilder);
					transaction.Commit ();
					return result;
				}
			}
			else
			{
				return this.ExecuteSilent (transaction, transaction.SqlBuilder);
			}
		}
		
		public int ExecuteSilent(DbTransaction transaction, ISqlBuilder builder)
		{
			System.Diagnostics.Debug.Assert (transaction != null);
			System.Diagnostics.Debug.Assert (builder != null);
			
			int count = builder.CommandCount;
			
			if (count < 1)
			{
				return 0;
			}
			
			using (System.Data.IDbCommand command = builder.CreateCommand (transaction.Transaction))
			{
				int result;
				this.sql_engine.Execute (command, DbCommandType.Silent, count, out result);
				return result;
			}
		}
		
		public object ExecuteScalar(DbTransaction transaction)
		{
			if (transaction == null)
			{
				using (transaction = this.BeginTransaction ())
				{
					object value = this.ExecuteScalar (transaction, transaction.SqlBuilder);
					transaction.Commit ();
					return value;
				}
			}
			else
			{
				return this.ExecuteScalar (transaction, transaction.SqlBuilder);
			}
		}
		
		public object ExecuteScalar(DbTransaction transaction, ISqlBuilder builder)
		{
			System.Diagnostics.Debug.Assert (transaction != null);
			System.Diagnostics.Debug.Assert (builder != null);
			
			int count = builder.CommandCount;
			
			if (count < 1)
			{
				return null;
			}
			
			using (System.Data.IDbCommand command = builder.CreateCommand (transaction.Transaction))
			{
				object data;
				
				this.sql_engine.Execute (command, DbCommandType.ReturningData, count, out data);
				
				return data;
			}
		}
		
		public object ExecuteNonQuery(DbTransaction transaction)
		{
			if (transaction == null)
			{
				using (transaction = this.BeginTransaction ())
				{
					object value = this.ExecuteNonQuery (transaction, transaction.SqlBuilder);
					transaction.Commit ();
					return value;
				}
			}
			else
			{
				return this.ExecuteNonQuery (transaction, transaction.SqlBuilder);
			}
		}
		
		public object ExecuteNonQuery(DbTransaction transaction, ISqlBuilder builder)
		{
			System.Diagnostics.Debug.Assert (transaction != null);
			System.Diagnostics.Debug.Assert (builder != null);
			
			int count = builder.CommandCount;
			
			if (count < 1)
			{
				return null;
			}
			
			using (System.Data.IDbCommand command = builder.CreateCommand (transaction.Transaction))
			{
				object data;
				
				this.sql_engine.Execute (command, DbCommandType.NonQuery, count, out data);
				
				return data;
			}
		}
		
		public System.Data.DataSet ExecuteRetData(DbTransaction transaction)
		{
			if (transaction == null)
			{
				using (transaction = this.BeginTransaction ())
				{
					System.Data.DataSet value = this.ExecuteRetData (transaction, transaction.SqlBuilder);
					transaction.Commit ();
					return value;
				}
			}
			else
			{
				return this.ExecuteRetData (transaction, transaction.SqlBuilder);
			}
		}
		
		public System.Data.DataSet ExecuteRetData(DbTransaction transaction, ISqlBuilder builder)
		{
			System.Diagnostics.Debug.Assert (transaction != null);
			System.Diagnostics.Debug.Assert (builder != null);
			
			int count = builder.CommandCount;
			
			if (count < 1)
			{
				return null;
			}
			
			using (System.Data.IDbCommand command = builder.CreateCommand (transaction.Transaction))
			{
				System.Data.DataSet data;
				
				this.sql_engine.Execute (command, DbCommandType.ReturningData, count, out data);
				
				return data;
			}
		}
		
		public System.Data.DataTable ExecuteSqlSelect(DbTransaction transaction, SqlSelect query, int min_rows)
		{
			if (transaction == null)
			{
				using (transaction = this.BeginTransaction (DbTransactionMode.ReadOnly))
				{
					System.Data.DataTable value = this.ExecuteSqlSelect (transaction, transaction.SqlBuilder, query, min_rows);
					transaction.Commit ();
					return value;
				}
			}
			else
			{
				return this.ExecuteSqlSelect (transaction, transaction.SqlBuilder, query, min_rows);
			}
		}
		
		public System.Data.DataTable ExecuteSqlSelect(DbTransaction transaction, ISqlBuilder builder, SqlSelect query, int min_rows)
		{
			System.Diagnostics.Debug.Assert (transaction != null);
			System.Diagnostics.Debug.Assert (builder != null);
			
			builder.SelectData (query);
			
			System.Data.DataSet data_set;
			System.Data.DataTable data_table;
			
			data_set = this.ExecuteRetData (transaction);
			
			if ((data_set == null) ||
				(data_set.Tables.Count != 1))
			{
				throw new Exceptions.GenericException (this.db_access, string.Format ("Query failed."));
			}
			
			data_table = data_set.Tables[0];
			
			if (data_table.Rows.Count < min_rows)
			{
				throw new Exceptions.GenericException (this.db_access, string.Format ("Query returned to few rows; expected {0}, found {1}.", min_rows, data_table.Rows.Count));
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
			
			return DbKey.Empty;
		}
		
		internal DbKey[] FindDbKeys(DbTransaction transaction, string table_name, string row_name)
		{
			//	Trouve la (ou les) clefs des lignes de la table 'table_name', pour lesquelles le
			//	contenu de la colonne CR_NAME correspond au nom défini par 'row_name'.
			
			SqlSelect query = new SqlSelect ();
			
			query.Fields.Add ("T_ID",   SqlField.CreateName ("T", Tags.ColumnId));
			query.Fields.Add ("T_STAT",	SqlField.CreateName ("T", Tags.ColumnStatus));
			
			query.Tables.Add ("T", SqlField.CreateName (table_name));
			
			query.Conditions.Add (new SqlFunction (SqlFunctionType.CompareEqual, SqlField.CreateName ("T", Tags.ColumnName), SqlField.CreateConstant (row_name, DbRawType.String)));
			
			System.Data.DataTable data_table = this.ExecuteSqlSelect (transaction, query, 0);
			
			if (this.displayDataSet != null)
			{
				this.displayDataSet (this, table_name, data_table);
			}
			
			DbKey[] keys = new DbKey[data_table.Rows.Count];
			
			for (int i = 0; i < data_table.Rows.Count; i++)
			{
				System.Data.DataRow row = data_table.Rows[i];
				
				long id;
				int  status;
				
				InvariantConverter.Convert (row["T_ID"],   out id);
				InvariantConverter.Convert (row["T_STAT"], out status);
				
				keys[i] = new DbKey (id, DbKey.ConvertFromIntStatus (status));
			}
			
			return keys;
		}
		
		
		public int CountMatchingRows(DbTransaction transaction, string table_name, string name_column, string value)
		{
			int count = 0;
			
			//	Compte combien de lignes dans la table ont le texte spécifié dans la colonne spécifiée.
			//	Ne considère que les lignes actives.
			
			if (transaction == null)
			{
				using (transaction = this.BeginTransaction (DbTransactionMode.ReadOnly))
				{
					count = this.CountMatchingRows (transaction, table_name, name_column, value);
					transaction.Commit ();
					return count;
				}
			}
			
			SqlSelect query = new SqlSelect ();
			
			query.Fields.Add ("N", new SqlAggregate (SqlAggregateType.Count, SqlField.CreateAll ()));
			query.Tables.Add ("T", SqlField.CreateName (table_name));
			
			query.Conditions.Add (new SqlFunction (SqlFunctionType.CompareEqual, SqlField.CreateName ("T", name_column), SqlField.CreateConstant (value, DbRawType.String)));

			DbInfrastructure.AddKeyExtraction (query.Conditions, "T", DbRowSearchMode.LiveActive);
			
			transaction.SqlBuilder.SelectData (query);
			
			InvariantConverter.Convert (this.ExecuteScalar (transaction), out count);
			
			return count;
		}
		
		
		public void UpdateKeyInRow(DbTransaction transaction, string table_name, DbKey old_key, DbKey new_key)
		{
			//	Met à jour la clef de la ligne spécifiée. Ceci est utile pour mettre à jour
			//	le champ DbRowStatus.
			
			if (transaction == null)
			{
				using (transaction = this.BeginTransaction (DbTransactionMode.ReadOnly))
				{
					this.UpdateKeyInRow (transaction, table_name, old_key, new_key);
					transaction.Commit ();
					return;
				}
			}
			
			Collections.SqlFields fields = new Collections.SqlFields ();
			Collections.SqlFields conds  = new Collections.SqlFields ();
			
			fields.Add (Tags.ColumnId,     SqlField.CreateConstant (new_key.Id,            DbKey.RawTypeForId));
			fields.Add (Tags.ColumnStatus, SqlField.CreateConstant (new_key.IntStatus,     DbKey.RawTypeForStatus));
			fields.Add (Tags.ColumnRefLog, SqlField.CreateConstant (this.logger.CurrentId, DbKey.RawTypeForId));
			
			DbInfrastructure.AddKeyExtraction (conds, table_name, old_key);
			
			transaction.SqlBuilder.UpdateData (table_name, fields, conds);
			
			int num_rows_affected;
			
			InvariantConverter.Convert (this.ExecuteNonQuery (transaction), out num_rows_affected);
			
			if (num_rows_affected != 1)
			{
				throw new Exceptions.GenericException (this.db_access, string.Format ("Update of row {0} in table {1} produced {2} updates.", old_key, table_name, num_rows_affected));
			}
		}
		
		public void UpdateTableNextId(DbTransaction transaction, DbKey key, DbId next_id)
		{
			if (transaction == null)
			{
				using (transaction = this.BeginTransaction (DbTransactionMode.ReadOnly))
				{
					this.UpdateTableNextId (transaction, key, next_id);
					transaction.Commit ();
					return;
				}
			}
			
			Collections.SqlFields fields = new Collections.SqlFields ();
			Collections.SqlFields conds  = new Collections.SqlFields ();
			
			fields.Add (Tags.ColumnNextId, SqlField.CreateConstant (next_id, DbKey.RawTypeForId));
			
			DbInfrastructure.AddKeyExtraction (conds, Tags.TableTableDef, key);
			
			transaction.SqlBuilder.UpdateData (Tags.TableTableDef, fields, conds);
			this.ExecuteSilent (transaction);
		}
		
		
		public List<DbTable> LoadDbTable(DbTransaction transaction, DbKey key, DbRowSearchMode rowSearchMode)
		{
			//	Charge les définitions pour la table au moyen d'une requête unique qui va
			//	aussi retourner les diverses définitions de colonnes.
			
			SqlSelect query = new SqlSelect ();
			
			//	Ce qui est propre à la table :
			
			query.Fields.Add ("T_ID",   SqlField.CreateName ("T_TABLE", Tags.ColumnId));
			query.Fields.Add ("T_NAME", SqlField.CreateName ("T_TABLE", Tags.ColumnName));
			query.Fields.Add ("T_INFO", SqlField.CreateName ("T_TABLE", Tags.ColumnInfoXml));
			
			//	Ce qui est propre aux colonnes :
			
			query.Fields.Add ("C_ID",     SqlField.CreateName ("T_COLUMN", Tags.ColumnId));
			query.Fields.Add ("C_NAME",   SqlField.CreateName ("T_COLUMN", Tags.ColumnName));
			query.Fields.Add ("C_INFO",   SqlField.CreateName ("T_COLUMN", Tags.ColumnInfoXml));
			query.Fields.Add ("C_TYPE",   SqlField.CreateName ("T_COLUMN", Tags.ColumnRefType));
			query.Fields.Add ("C_PARENT", SqlField.CreateName ("T_COLUMN", Tags.ColumnRefParent));
			
			//	Les deux tables utilisées pour l'extraction :
			
			query.Tables.Add ("T_TABLE",  SqlField.CreateName (Tags.TableTableDef));
			query.Tables.Add ("T_COLUMN", SqlField.CreateName (Tags.TableColumnDef));
			
			if (key.IsEmpty)
			{
				//	On extrait toutes les définitions de tables qui correspondent à une version
				//	'active' (ignore les versions archivées et détruites). Extrait aussi les colonnes
				//	correspondantes.
				
				DbInfrastructure.AddKeyExtraction (query.Conditions, "T_TABLE", rowSearchMode);
				DbInfrastructure.AddKeyExtraction (query.Conditions, "T_COLUMN", Tags.ColumnRefTable, "T_TABLE");
			}
			else
			{
				//	On extrait toutes les lignes de T_TABLE qui ont un CR_ID = key, ainsi que
				//	les lignes correspondantes de T_COLUMN qui ont un CREF_TABLE = key.
				
				DbInfrastructure.AddKeyExtraction (query.Conditions, "T_TABLE", key);
				DbInfrastructure.AddKeyExtraction (query.Conditions, "T_COLUMN", Tags.ColumnRefTable, key);
			}
			
			System.Data.DataTable dataTable = this.ExecuteSqlSelect (transaction, query, 1);
			
			if (this.displayDataSet != null)
			{
				this.displayDataSet (this, string.Format ("DbTable.{0}", key), dataTable);
			}
			
			long          rowId   = -1;
			List<DbTable> tables  = new List<DbTable> ();
			DbTable		  dbTable = null;
			bool          recycle = false;
			
			
			//	Analyse toutes les lignes retournées. On suppose que les lignes sont groupées
			//	logiquement par tables.
			
			foreach (System.Data.DataRow row in dataTable.Rows)
			{
				long currentRowId = InvariantConverter.ToLong (row["T_ID"]);
				
				if (rowId != currentRowId)
				{
					rowId   = currentRowId;
					dbTable = null;
					
					string tableInfo = InvariantConverter.ToString (row["T_INFO"]);
					string tableName = InvariantConverter.ToString (row["T_NAME"]);
					DbKey  tableKey  = key.IsEmpty ? new DbKey (rowId) : key;
					
					dbTable = this.cache_db_tables[tableKey];
					
					if (dbTable == null)
					{
						dbTable = DbTools.DeserializeFromXml<DbTable> (tableInfo);
						recycle = false;

						dbTable.DefineName (tableName);
						dbTable.DefineKey (tableKey);
						
						//	Afin d'éviter de recharger cette table plus tard, on va en prendre note tout de suite; ça permet
						//	aussi d'éviter des boucles sans fin dans le cas de tables qui ont des références circulaires, car
						//	la prochaine recherche avec ResolveDbTable s'appliquant à cette table se terminera avec succès.
						
						if ((tableKey.Status != DbRowStatus.Live) ||
							(tableKey.Status == DbRowStatus.Copied))
						{
							this.cache_db_tables[tableKey] = dbTable;
						}
					}
					else
					{
						System.Diagnostics.Debug.WriteLine (string.Format ("Recycling known table {0}", dbTable.Name));
						recycle = true;
					}
					
					tables.Add (dbTable);
				}
				
				if (recycle)
				{
					continue;
				}
				
				//	Chaque ligne contient une définition de colonne.

				long   typeDefId  = InvariantConverter.ToLong (row["C_TYPE"]);
				long   columnId   = InvariantConverter.ToLong (row["C_ID"]);
				string columnName = InvariantConverter.ToString (row["C_NAME"]);
				string columnInfo = InvariantConverter.ToString (row["C_INFO"]);
				string targetName = null;

				if (InvariantConverter.IsNotNull (row["C_PARENT"]))
				{
					DbKey   parentKey   = new DbKey (InvariantConverter.ToLong (row["C_PARENT"]));
					DbTable parentTable = this.ResolveDbTable (transaction, parentKey);

					targetName = parentTable.Name;
				}

				DbKey     typeDefKey = new DbKey (typeDefId);
				DbTypeDef typeDef    = this.ResolveDbType (transaction, typeDefKey);
				
				if (typeDef == null)
				{
					throw new Exceptions.GenericException (this.db_access, string.Format ("Missing type for column '{0}' in table '{1}'", columnName, dbTable.Name));
				}

				System.Diagnostics.Debug.Assert (typeDef.Key == typeDefKey);

				DbColumn dbColumn = DbTools.DeserializeFromXml<DbColumn> (columnInfo);

				dbColumn.DefineName (columnName);
				dbColumn.DefineKey (new DbKey (columnId));
				dbColumn.DefineType (typeDef);
				dbColumn.DefineTargetTableName (targetName);
				
				dbTable.Columns.Add (dbColumn);
				
				if (dbColumn.IsPrimaryKey)
				{
					dbTable.PrimaryKeys.Add (dbColumn);
				}
			}
			
			//	TODO: il faut encore initialiser les champs TargetTableName des diverses colonnes
			//	qui établissent une relation avec une autre table. Pour cela, il faudra faire un
			//	SELECT dans Tags.TableRelationDef pour les colonnes dont DbColumnClass est parmi
			//	RefSimpleId/RefLiveId/RefTupleId/RefTupleRevision et déterminer le nom des tables
			//	cibles, puis appeler DbColumn.DefineTargetTableName...
			
			return tables;
		}
		
		public List<DbTypeDef> LoadDbType(DbTransaction transaction, DbKey key, DbRowSearchMode rowSearchMode)
		{
			SqlSelect query = new SqlSelect ();
			
			query.Fields.Add ("T_ID",   SqlField.CreateName ("T_TYPE", Tags.ColumnId));
			query.Fields.Add ("T_NAME", SqlField.CreateName ("T_TYPE", Tags.ColumnName));
			query.Fields.Add ("T_INFO", SqlField.CreateName ("T_TYPE", Tags.ColumnInfoXml));
			
			query.Tables.Add ("T_TYPE", SqlField.CreateName (Tags.TableTypeDef));
			
			if (key.IsEmpty)
			{
				//	On extrait toutes les définitions de types qui correspondent à la version
				//	'active'.
				
				DbInfrastructure.AddKeyExtraction (query.Conditions, "T_TYPE", rowSearchMode);
			}
			else
			{
				//	Cherche la ligne de la table dont 'CR_ID = key'.
				
				DbInfrastructure.AddKeyExtraction (query.Conditions, "T_TYPE", key);
			}
			
			System.Data.DataTable dataTable = this.ExecuteSqlSelect (transaction, query, 1);
			List<DbTypeDef> types = new List<DbTypeDef> ();
			
			foreach (System.Data.DataRow row in dataTable.Rows)
			{
				long   typeId   = InvariantConverter.ToLong (row["T_ID"]);
				string typeName = InvariantConverter.ToString (row["T_NAME"]);
				string typeInfo = InvariantConverter.ToString (row["T_INFO"]);
				
				//	A partir de l'information trouvée dans la base, génère l'objet DbTypeDef
				//	qui correspond.

				DbTypeDef typeDef = DbTools.DeserializeFromXml<DbTypeDef> (typeInfo);

				typeDef.DefineName (typeName);
				typeDef.DefineKey (new DbKey (typeId));
				
				//	TODO: handle enumeration types
#if false
				if (type is DbTypeEnum)
				{
					DbTypeEnum type_enum = type as DbTypeEnum;
					DbEnumValue[] values = this.LoadEnumValues (transaction, type_enum);
					
					type_enum.DefineValues (values);
				}
#endif
				
				types.Add (typeDef);
			}
			
			return types;
		}
		
		
		public long NextRowIdInTable(DbTransaction transaction, DbKey key)
		{
			return this.NewRowIdInTable (transaction, key, 0);
		}
		
		public long NewRowIdInTable(DbTransaction transaction, DbKey key, int numKeys)
		{
			//	Attribue 'numKeys' clef consécutives dans la table spécifiée.
			
			System.Diagnostics.Debug.Assert (numKeys >= 0);
			
			if (transaction == null)
			{
				using (transaction = this.BeginTransaction ())
				{
					long id = this.NewRowIdInTable (transaction, key, numKeys);
					transaction.Commit ();
					return id;
				}
			}
			
			Collections.SqlFields fields = new Collections.SqlFields ();
			Collections.SqlFields conds  = new Collections.SqlFields ();
			
			SqlField fieldNextId = SqlField.CreateName (Tags.ColumnNextId);
			SqlField fieldConstN = SqlField.CreateConstant (numKeys, DbRawType.Int32);
			
			fields.Add (Tags.ColumnNextId, new SqlFunction (SqlFunctionType.MathAdd, fieldNextId, fieldConstN));
			
			DbInfrastructure.AddKeyExtraction (conds, Tags.TableTableDef, key);
			
			if (numKeys > 0)
			{
				transaction.SqlBuilder.UpdateData (Tags.TableTableDef, fields, conds);
				this.ExecuteSilent (transaction);
			}
			
			SqlSelect query = new SqlSelect ();

			System.Diagnostics.Debug.Assert (conds.Count == 1);

			query.Fields.Add (fieldNextId);
			query.Tables.Add (SqlField.CreateName (Tags.TableTableDef));
			query.Conditions.Add (conds[0]);
			
			transaction.SqlBuilder.SelectData (query);
			
			return InvariantConverter.ToLong (this.ExecuteScalar (transaction)) - numKeys;
		}
		
#if false
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
			
			//	Cherche les lignes de la table dont la colonne CREF_TYPE correspond à l'ID du type.
			
			DbInfrastructure.AddKeyExtraction (query.Conditions, "T_ENUM", Tags.ColumnRefType, type_enum.InternalKey);
			
			System.Data.DataTable data_table = this.ExecuteSqlSelect (transaction, query, 1);
			
			DbEnumValue[] values = new DbEnumValue[data_table.Rows.Count];
			
			for (int i = 0; i < data_table.Rows.Count; i++)
			{
				System.Data.DataRow data_row = data_table.Rows[i];
				
				//	Pour chaque valeur retournée dans la table, il y a une ligne. Cette ligne
				//	contient toute l'information nécessaire à la création d'une instance de la
				//	class DbEnumValue :
				
				string val_name = InvariantConverter.ToString (data_row["E_NAME"]);
				string val_id   = InvariantConverter.ToString (data_row["E_ID"]);
				string val_info = InvariantConverter.ToString (data_row["E_INFO"]);
				
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
#endif
		
#if false
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
				
				if (InvariantConverter.Convert (row[index], out value))
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
#endif
		
		protected static void AddKeyExtraction(Collections.SqlFields conditions, string table_name, DbKey key)
		{
			SqlField name_col_id = SqlField.CreateName (table_name, Tags.ColumnId);
			SqlField constant_id = SqlField.CreateConstant (key.Id, DbKey.RawTypeForId);
			
			conditions.Add (new SqlFunction (SqlFunctionType.CompareEqual, name_col_id, constant_id));
		}

		protected static void AddKeyExtraction(Collections.SqlFields conditions, string sourceTableName, string sourceColumnName, string parentTableName)
		{
			SqlField parentColumnId = SqlField.CreateName (parentTableName, Tags.ColumnId);
			SqlField sourceColumnId = SqlField.CreateName (sourceTableName, sourceColumnName);
			
			conditions.Add (new SqlFunction (SqlFunctionType.CompareEqual, sourceColumnId, parentColumnId));
		}
		
		protected static void AddKeyExtraction(Collections.SqlFields conditions, string source_table_name, string source_col_name, DbKey key)
		{
			SqlField source_col_id = SqlField.CreateName (source_table_name, source_col_name);
			SqlField constant_id   = SqlField.CreateConstant (key.Id, DbKey.RawTypeForId);
			
			conditions.Add (new SqlFunction (SqlFunctionType.CompareEqual, source_col_id, constant_id));
		}

		protected static void AddKeyExtraction(Collections.SqlFields conditions, string table_name, DbRowSearchMode search_mode)
		{
			//	Génère la condition d'extraction pour une recherche selon le statut des lignes
			//	(voir aussi la définition de DbRowStatus).
			
			SqlFunctionType function;
			DbRowStatus     status;
			
			switch (search_mode)
			{
				case DbRowSearchMode.Copied:		status = DbRowStatus.Copied;		function = SqlFunctionType.CompareEqual;	break;
				case DbRowSearchMode.Live:			status = DbRowStatus.Live;			function = SqlFunctionType.CompareEqual;	break;
				case DbRowSearchMode.LiveActive:	status = DbRowStatus.ArchiveCopy;	function = SqlFunctionType.CompareLessThan;	break;
				case DbRowSearchMode.ArchiveCopy:	status = DbRowStatus.ArchiveCopy;	function = SqlFunctionType.CompareEqual;	break;
				case DbRowSearchMode.LiveAll:		status = DbRowStatus.Deleted;		function = SqlFunctionType.CompareLessThan;	break;
				case DbRowSearchMode.Deleted:		status = DbRowStatus.Deleted;		function = SqlFunctionType.CompareEqual;	break;
				
				case DbRowSearchMode.All:
					return;
				
				default:
					throw new System.ArgumentException (string.Format ("Search mode {0} not supported.", search_mode), "search_mode");
			}
			
			SqlField name_status  = SqlField.CreateName (table_name, Tags.ColumnStatus);
			SqlField const_status = SqlField.CreateConstant (DbKey.ConvertToIntStatus (status), DbKey.RawTypeForStatus);
			
			conditions.Add (new SqlFunction (function, name_status, const_status));
		}


		protected virtual void StartUsingDatabase()
		{
			using (DbTransaction transaction = this.BeginTransaction (DbTransactionMode.ReadOnly))
			{
				this.globals = new Settings.Globals (this, transaction);
				this.locals  = new Settings.Locals (this, transaction);
				
				this.client_id = this.locals.ClientId;
				this.SetupLogger (transaction);
				
				transaction.Commit ();
			}
		}
		
		
		private void SetupLogger(DbTransaction transaction)
		{
			this.logger = new DbLogger ();
			this.logger.DefineClientId (this.client_id);
			this.logger.Attach (this, this.internal_tables[Tags.TableLog]);
			this.logger.ResetCurrentLogId (transaction);
		}
		
		private void SetupTables(DbTransaction transaction)
		{
			//	Remplit les tables de gestion (CR_*) avec les valeurs par défaut et
			//	les définitions initiales de la structure interne de la base vide.
			
			long type_key_id     = DbId.CreateId (1, this.client_id);
			long table_key_id    = DbId.CreateId (1, this.client_id);
			long column_key_id   = DbId.CreateId (1, this.client_id);
			long enum_val_key_id = DbId.CreateId (1, this.client_id);
			long client_key_id   = DbId.CreateId (1, this.client_id);
			
			//	Initialisation partielle de DbLogger (juste ce qu'il faut pour pouvoir
			//	accéder à this.logger.CurrentId) :
			
			this.logger = new DbLogger ();
			this.logger.DefineClientId (this.client_id);
			this.logger.DefineLogId (1);
			
			System.Diagnostics.Debug.Assert (this.logger.CurrentId.LocalId == 1);
			
			//	Il faut commencer par finir d'initialiser les descriptions des types, parce
			//	que les description des colonnes doivent y faire référence.
			
			foreach (DbTypeDef typeDef in this.internal_types)
			{
				//	Attribue à chaque type interne une clef unique et établit les informations de base
				//	dans la table de définition des types.
				
				typeDef.DefineKey (new DbKey (type_key_id++));
				this.InsertTypeDefRow (transaction, typeDef);
			}
			
			foreach (DbTable table in this.internal_tables)
			{
				//	Attribue à chaque table interne une clef unique et établit les informations de base
				//	dans la table de définition des tables.
				
				table.DefineKey (new DbKey (table_key_id++));
				table.UpdatePrimaryKeyInfo ();
				
				this.InsertTableDefRow (transaction, table);
				
				foreach (DbColumn column in table.Columns)
				{
					//	Pour chaque colonne de la table, établit les informations de base dans la table de
					//	définition des colonnes.
					
					column.DefineKey (new DbKey (column_key_id++));
					this.InsertColumnDefRow (transaction, table, column);
				}
			}
			
			//	Complète encore les informations au sujet des relations :
			//
			//	- La description d'une colonne fait référence à la table et à un type.
			//	- La description d'une valeur d'enum fait référence à un type.
			//	- La description d'une référence fait elle-même référence à la table
			//	  source et destination, ainsi qu'à la colonne.
			
			this.UpdateColumnRelation (transaction, Tags.TableColumnDef,   Tags.ColumnRefTable,  Tags.TableTableDef);
			this.UpdateColumnRelation (transaction, Tags.TableColumnDef,   Tags.ColumnRefType,   Tags.TableTypeDef);
			this.UpdateColumnRelation (transaction, Tags.TableColumnDef,   Tags.ColumnRefParent, Tags.TableTypeDef);
			this.UpdateColumnRelation (transaction, Tags.TableEnumValDef,  Tags.ColumnRefType,   Tags.TableTypeDef);
			
			this.UpdateTableNextId (transaction, this.internal_tables[Tags.TableTableDef].Key, table_key_id);
			this.UpdateTableNextId (transaction, this.internal_tables[Tags.TableColumnDef].Key, column_key_id);
			this.UpdateTableNextId (transaction, this.internal_tables[Tags.TableTypeDef].Key, type_key_id);
			this.UpdateTableNextId (transaction, this.internal_tables[Tags.TableEnumValDef].Key, enum_val_key_id);
			this.UpdateTableNextId (transaction, this.internal_tables[Tags.TableClientDef].Key, client_key_id);
			
			//	On ne peut attacher le DbLogger qu'ici, car avant, les tables et les clefs
			//	indispensables ne sont pas encore utilisables :
			
			this.logger.Attach (this, this.internal_tables[Tags.TableLog]);
			this.logger.CreateInitialEntry (transaction);
			
			System.Diagnostics.Debug.Assert (this.logger.CurrentId.LocalId == 1);
			System.Diagnostics.Debug.Assert (this.NextRowIdInTable (transaction, this.internal_tables[Tags.TableLog].Key) == DbId.CreateId (2, this.client_id));
		}
		
		
		protected static void SetCategory(DbColumn[] columns, DbElementCat cat)
		{
			for (int i = 0; i < columns.Length; i++)
			{
				columns[i].DefineCategory (cat);
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

			DbInfrastructure.AddKeyExtraction (conds, Tags.TableColumnDef, source_column_key);
			
			transaction.SqlBuilder.UpdateData (Tags.TableColumnDef, fields, conds);
			this.ExecuteSilent (transaction);
		}

		protected void UpdateColumnRelation(DbTransaction transaction, string src_table_name, string src_columnName, string targetTableName)
		{
			DbTable  source = this.internal_tables[src_table_name];
			DbTable parent = this.internal_tables[targetTableName];
			DbColumn column = source.Columns[src_columnName];
			
			DbKey src_table_key    = source.Key;
			DbKey src_column_key   = column.Key;
			DbKey parent_table_key = parent.Key;
			
			this.UpdateColumnRelation (transaction, src_table_key, src_column_key, parent_table_key);
		}
				
		
		protected void InsertTypeDefRow(DbTransaction transaction, DbTypeDef typeDef)
		{
			System.Diagnostics.Debug.Assert (this.logger.CurrentId.LocalId > 0);
			
			DbTable type_def_table = this.internal_tables[Tags.TableTypeDef];
			
			//	Insère une ligne dans la table de définition des types.
			
			Collections.SqlFields fields = new Collections.SqlFields ();
			
			fields.Add (type_def_table.Columns[Tags.ColumnId]      .CreateSqlField (this.type_converter, typeDef.Key.Id));
			fields.Add (type_def_table.Columns[Tags.ColumnStatus]  .CreateSqlField (this.type_converter, typeDef.Key.IntStatus));
			fields.Add (type_def_table.Columns[Tags.ColumnRefLog]  .CreateSqlField (this.type_converter, this.logger.CurrentId));
			fields.Add (type_def_table.Columns[Tags.ColumnName]    .CreateSqlField (this.type_converter, typeDef.Name));
			fields.Add (type_def_table.Columns[Tags.ColumnInfoXml] .CreateSqlField (this.type_converter, DbTools.GetCompactXml (typeDef)));
			
			//	TODO: Initializer les colonnes descriptives
			
			transaction.SqlBuilder.InsertData (type_def_table.CreateSqlName (), fields);
			this.ExecuteSilent (transaction);
		}

#if false
		protected void InsertEnumValueDefRow(DbTransaction transaction, DbType type, DbEnumValue value)
		{
			System.Diagnostics.Debug.Assert (this.logger.CurrentId.LocalId > 0);
			System.Diagnostics.Debug.Assert (transaction != null);
			
			DbTable enum_def = this.internal_tables[Tags.TableEnumValDef];
			
			//	Insère une ligne dans la table de définition des énumérations.
			
			Collections.SqlFields fields = new Collections.SqlFields ();
			
			fields.Add (enum_def.Columns[Tags.ColumnId]	     .CreateSqlField (this.type_converter, value.InternalKey.Id));
			fields.Add (enum_def.Columns[Tags.ColumnStatus]  .CreateSqlField (this.type_converter, value.InternalKey.IntStatus));
			fields.Add (enum_def.Columns[Tags.ColumnRefLog]  .CreateSqlField (this.type_converter, this.logger.CurrentId));
			fields.Add (enum_def.Columns[Tags.ColumnName]    .CreateSqlField (this.type_converter, value.Name));
			fields.Add (enum_def.Columns[Tags.ColumnInfoXml] .CreateSqlField (this.type_converter, DbEnumValue.SerializeToXml (value, false)));
			fields.Add (enum_def.Columns[Tags.ColumnRefType] .CreateSqlField (this.type_converter, type.InternalKey.Id));
			
			//	TODO: Initialiser les colonnes descriptives
			
			transaction.SqlBuilder.InsertData (enum_def.CreateSqlName (), fields);
			this.ExecuteSilent (transaction);
		}
#endif
		
		protected void InsertTableDefRow(DbTransaction transaction, DbTable table)
		{
			System.Diagnostics.Debug.Assert (this.logger.CurrentId.LocalId > 0);
			System.Diagnostics.Debug.Assert (transaction != null);
			
			DbTable table_def = this.internal_tables[Tags.TableTableDef];
			
			//	Insère une ligne dans la table de définition des tables.
			
			Collections.SqlFields fields = new Collections.SqlFields ();
			
			fields.Add (table_def.Columns[Tags.ColumnId]      .CreateSqlField (this.type_converter, table.Key.Id));
			fields.Add (table_def.Columns[Tags.ColumnStatus]  .CreateSqlField (this.type_converter, table.Key.IntStatus));
			fields.Add (table_def.Columns[Tags.ColumnRefLog]  .CreateSqlField (this.type_converter, this.logger.CurrentId));
			fields.Add (table_def.Columns[Tags.ColumnName]    .CreateSqlField (this.type_converter, table.Name));
			fields.Add (table_def.Columns[Tags.ColumnInfoXml] .CreateSqlField (this.type_converter, DbTools.GetCompactXml (table)));
			fields.Add (table_def.Columns[Tags.ColumnNextId]  .CreateSqlField (this.type_converter, DbId.CreateId (1, this.client_id)));
			
			//	TODO: Initialiser les colonnes descriptives
			
			transaction.SqlBuilder.InsertData (table_def.CreateSqlName (), fields);
			this.ExecuteSilent (transaction);
		}
		
		protected void InsertColumnDefRow(DbTransaction transaction, DbTable table, DbColumn column)
		{
			System.Diagnostics.Debug.Assert (this.logger.CurrentId.LocalId > 0);
			System.Diagnostics.Debug.Assert (transaction != null);
			
			DbTable column_def = this.internal_tables[Tags.TableColumnDef];
			
			//	Insère une ligne dans la table de définition des colonnes.
			
			Collections.SqlFields fields = new Collections.SqlFields ();
			
			fields.Add (column_def.Columns[Tags.ColumnId]      .CreateSqlField (this.type_converter, column.Key.Id));
			fields.Add (column_def.Columns[Tags.ColumnStatus]  .CreateSqlField (this.type_converter, column.Key.IntStatus));
			fields.Add (column_def.Columns[Tags.ColumnRefLog]  .CreateSqlField (this.type_converter, this.logger.CurrentId));
			fields.Add (column_def.Columns[Tags.ColumnName]    .CreateSqlField (this.type_converter, column.Name));
			fields.Add (column_def.Columns[Tags.ColumnInfoXml] .CreateSqlField (this.type_converter, DbTools.GetCompactXml (column)));
			fields.Add (column_def.Columns[Tags.ColumnRefTable].CreateSqlField (this.type_converter, table.Key.Id));
			fields.Add (column_def.Columns[Tags.ColumnRefType] .CreateSqlField (this.type_converter, column.Type.Key.Id));
			
			//	TODO: Initialiser les colonnes descriptives
			
			transaction.SqlBuilder.InsertData (column_def.CreateSqlName (), fields);
			this.ExecuteSilent (transaction);
		}
		
		
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.global_lock != null)
				{
					this.global_lock.ReleaseLock ();
					this.global_lock = null;
				}
				
				if (this.logger != null)
				{
					this.logger.Detach ();
					this.logger = null;
				}
				
				if (this.db_abstraction != null)
				{
					this.db_abstraction.Dispose ();
					
					System.Diagnostics.Debug.Assert (this.db_abstraction.IsConnectionOpen == false);
					
					this.db_abstraction = null;
					this.sql_engine     = null;
					this.type_converter = null;
				}
				
				System.Diagnostics.Debug.Assert (this.sql_engine == null);
				System.Diagnostics.Debug.Assert (this.type_converter == null);
			}
		}
		
		
		#region Initialisation
		protected void InitializeDatabaseAbstraction()
		{
			this.types = new TypeHelper (this);
			
			this.db_abstraction = this.CreateDbAbstraction ();
			
			this.sql_engine  = this.db_abstraction.SqlEngine;
			
			System.Diagnostics.Debug.Assert (this.sql_engine != null);
			
			this.type_converter = this.db_abstraction.Factory.TypeConverter;
			
			System.Diagnostics.Debug.Assert (this.type_converter != null);
			
			this.db_abstraction.SqlBuilder.AutoClear = true;
		}
		#endregion
		
		#region IDisposable Members
		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (this);
		}
		#endregion
		
		#region BootHelper Class
		public class BootHelper
		{
			public BootHelper(DbInfrastructure infrastructure, DbTransaction transaction)
			{
				this.infrastructure = infrastructure;
				this.transaction    = transaction;

				System.Diagnostics.Debug.Assert (this.infrastructure.types.KeyId.IsNullable == false);
				System.Diagnostics.Debug.Assert (this.infrastructure.types.KeyStatus.IsNullable == false);
				System.Diagnostics.Debug.Assert (this.infrastructure.types.NullableKeyId.IsNullable == true);
				System.Diagnostics.Debug.Assert (this.infrastructure.types.Name.IsNullable == false);
				System.Diagnostics.Debug.Assert (this.infrastructure.types.InfoXml.IsNullable == false);
			}
			
			
			public void CreateTableTableDef()
			{
				TypeHelper types   = this.infrastructure.types;
				DbTable    table   = new DbTable (Tags.TableTableDef);
				DbColumn[] columns = new DbColumn[]
					{
						new DbColumn (Tags.ColumnId,		  types.KeyId,		 DbColumnClass.KeyId),
						new DbColumn (Tags.ColumnStatus,	  types.KeyStatus,	 DbColumnClass.KeyStatus),
						new DbColumn (Tags.ColumnRefLog,	  types.KeyId,		 DbColumnClass.RefInternal),
						new DbColumn (Tags.ColumnName,		  types.Name,		 DbColumnClass.Data),
						new DbColumn (Tags.ColumnInfoXml,	  types.InfoXml,	 DbColumnClass.Data),
						new DbColumn (Tags.ColumnNextId,	  types.KeyId,		 DbColumnClass.RefInternal)
					};

				this.CreateTable (table, columns, DbReplicationMode.Manual);
			}
			
			public void CreateTableColumnDef()
			{
				TypeHelper types   = this.infrastructure.types;
				DbTable    table   = new DbTable (Tags.TableColumnDef);
				DbColumn[] columns = new DbColumn[]
					{
						new DbColumn (Tags.ColumnId,		  types.KeyId,		   DbColumnClass.KeyId),
						new DbColumn (Tags.ColumnStatus,	  types.KeyStatus,	   DbColumnClass.KeyStatus),
						new DbColumn (Tags.ColumnRefLog,	  types.KeyId,		   DbColumnClass.RefInternal),
						new DbColumn (Tags.ColumnName,		  types.Name,		   DbColumnClass.Data),
						new DbColumn (Tags.ColumnInfoXml,	  types.InfoXml,	   DbColumnClass.Data),
						new DbColumn (Tags.ColumnRefTable,	  types.KeyId,         DbColumnClass.RefId),
						new DbColumn (Tags.ColumnRefType,	  types.KeyId,         DbColumnClass.RefId),
						new DbColumn (Tags.ColumnRefParent,	  types.NullableKeyId, DbColumnClass.RefId)
					};
				
				this.CreateTable (table, columns, DbReplicationMode.Manual);
			}
			
			public void CreateTableTypeDef()
			{
				TypeHelper types   = this.infrastructure.types;
				DbTable    table   = new DbTable (Tags.TableTypeDef);
				DbColumn[] columns = new DbColumn[]
					{
						new DbColumn (Tags.ColumnId,		  types.KeyId,		 DbColumnClass.KeyId),
						new DbColumn (Tags.ColumnStatus,	  types.KeyStatus,	 DbColumnClass.KeyStatus),
						new DbColumn (Tags.ColumnRefLog,	  types.KeyId,		 DbColumnClass.RefInternal),
						new DbColumn (Tags.ColumnName,		  types.Name,		 DbColumnClass.Data),
						new DbColumn (Tags.ColumnInfoXml,	  types.InfoXml,	 DbColumnClass.Data)
					};
				
				this.CreateTable (table, columns, DbReplicationMode.Manual);
			}
			
			public void CreateTableEnumValDef()
			{
				TypeHelper types   = this.infrastructure.types;
				DbTable    table   = new DbTable (Tags.TableEnumValDef);
				DbColumn[] columns = new DbColumn[]
					{
						new DbColumn (Tags.ColumnId,		  types.KeyId,		DbColumnClass.KeyId),
						new DbColumn (Tags.ColumnStatus,	  types.KeyStatus,	DbColumnClass.KeyStatus),
						new DbColumn (Tags.ColumnRefLog,	  types.KeyId,		DbColumnClass.RefInternal),
						new DbColumn (Tags.ColumnName,		  types.Name,		DbColumnClass.Data),
						new DbColumn (Tags.ColumnInfoXml,	  types.InfoXml,	DbColumnClass.Data),
						new DbColumn (Tags.ColumnRefType,	  types.KeyId,      DbColumnClass.RefId)
					};
				
				this.CreateTable (table, columns, DbReplicationMode.Manual);
			}
			
			public void CreateTableLog()
			{
				TypeHelper types   = this.infrastructure.types;
				DbTable    table   = new DbTable (Tags.TableLog);
				DbColumn[] columns = new DbColumn[]
					{
						new DbColumn (Tags.ColumnId,		  types.KeyId,		DbColumnClass.KeyId),
						new DbColumn (Tags.ColumnDateTime,	  types.DateTime,	DbColumnClass.Data)
					};
				
				//	TODO: ajouter ici une colonne définissant la nature du changement (et l'utilisateur
				//	qui en est la cause).
				
				this.CreateTable (table, columns, DbReplicationMode.Manual);
			}
			
			public void CreateTableRequestQueue()
			{
				TypeHelper types   = this.infrastructure.types;
				DbTable    table   = new DbTable (Tags.TableRequestQueue);
				DbColumn[] columns = new DbColumn[]
					{
						new DbColumn (Tags.ColumnId,		  types.KeyId,		DbColumnClass.KeyId),
						new DbColumn (Tags.ColumnStatus,	  types.KeyStatus,	DbColumnClass.KeyStatus),
						new DbColumn (Tags.ColumnRefLog,	  types.KeyId,		DbColumnClass.RefInternal),
						new DbColumn (Tags.ColumnReqExState,  types.ReqExecState, DbColumnClass.Data),
						new DbColumn (Tags.ColumnReqData,	  types.ReqData,	DbColumnClass.Data),
						new DbColumn (Tags.ColumnDateTime,    types.DateTime,   DbColumnClass.Data)
					};
				
				this.CreateTable (table, columns, DbReplicationMode.Private);
			}
			
			public void CreateTableClientDef()
			{
				TypeHelper types   = this.infrastructure.types;
				DbTable    table   = new DbTable (Tags.TableClientDef);
				DbColumn[] columns = new DbColumn[]
					{
						new DbColumn (Tags.ColumnId,			types.KeyId,	 DbColumnClass.KeyId),
						new DbColumn (Tags.ColumnStatus,		types.KeyStatus, DbColumnClass.KeyStatus),
						new DbColumn (Tags.ColumnRefLog,		types.KeyId,	 DbColumnClass.RefInternal),
						new DbColumn (Tags.ColumnClientId,		types.KeyId,	 DbColumnClass.Data),
						new DbColumn (Tags.ColumnClientName,	types.Name,      DbColumnClass.Data),
						new DbColumn (Tags.ColumnClientSync,	types.KeyId,	 DbColumnClass.Data),
						new DbColumn (Tags.ColumnClientCreDate,	types.DateTime,  DbColumnClass.Data),
						new DbColumn (Tags.ColumnClientConDate,	types.DateTime,  DbColumnClass.Data)
					};
				
				this.CreateTable (table, columns, DbReplicationMode.Private);
			}
			
			
			private void CreateTable(DbTable table, DbColumn[] columns, DbReplicationMode replication_mode)
			{
				DbInfrastructure.SetCategory (columns, DbElementCat.Internal);
				
				table.Columns.AddRange (columns);
				
				table.DefineCategory (DbElementCat.Internal);
				table.DefinePrimaryKey (columns[0]);
				table.DefineReplicationMode (replication_mode);
				
				this.infrastructure.internal_tables.Add (table);
				
				SqlTable sql_table = table.CreateSqlTable (this.infrastructure.type_converter);
				this.infrastructure.DefaultSqlBuilder.InsertTable (sql_table);
				this.infrastructure.ExecuteSilent (this.transaction);
			}
		
			
			private DbInfrastructure			infrastructure;
			private DbTransaction				transaction;
		}
		#endregion
		
		#region TypeHelper Class
		protected class TypeHelper
		{
			public TypeHelper(DbInfrastructure infrastructure)
			{
				this.infrastructure = infrastructure;
			}
			
			
			public void RegisterTypes()
			{
				this.InitializeNumTypes ();
				this.InitializeStrTypes ();
				this.InitializeOtherTypes ();
				
				this.AssertAllTypesReady ();
			}
			
			public void ResolveTypes(DbTransaction transaction)
			{
				this.num_type_key_id          = this.infrastructure.ResolveDbType (transaction, Tags.TypeKeyId);
				this.num_type_nullable_key_id = this.infrastructure.ResolveDbType (transaction, Tags.TypeNullableKeyId);
				this.num_type_key_status      = this.infrastructure.ResolveDbType (transaction, Tags.TypeKeyStatus);
				this.num_type_req_ex_state    = this.infrastructure.ResolveDbType (transaction, Tags.TypeReqExState);
				
				this.str_type_name        = this.infrastructure.ResolveDbType (transaction, Tags.TypeName);
				this.str_type_info_xml    = this.infrastructure.ResolveDbType (transaction, Tags.TypeInfoXml);
				this.str_type_dict_key    = this.infrastructure.ResolveDbType (transaction, Tags.TypeDictKey);
				this.str_type_dict_value  = this.infrastructure.ResolveDbType (transaction, Tags.TypeDictValue);
				
				this.d_t_type_datetime    = this.infrastructure.ResolveDbType (transaction, Tags.TypeDateTime);
				this.bin_type_req_data    = this.infrastructure.ResolveDbType (transaction, Tags.TypeReqData);
				
				this.infrastructure.internal_types.Add (this.num_type_key_id);
				this.infrastructure.internal_types.Add (this.num_type_nullable_key_id);
				this.infrastructure.internal_types.Add (this.num_type_key_status);
				this.infrastructure.internal_types.Add (this.num_type_req_ex_state);
				
				this.infrastructure.internal_types.Add (this.str_type_name);
				this.infrastructure.internal_types.Add (this.str_type_info_xml);
				this.infrastructure.internal_types.Add (this.str_type_dict_key);
				this.infrastructure.internal_types.Add (this.str_type_dict_value);
				
				this.infrastructure.internal_types.Add (this.d_t_type_datetime);
				this.infrastructure.internal_types.Add (this.bin_type_req_data);
				
				this.AssertAllTypesReady ();
			}


			void InitializeNumTypes()
			{
				this.num_type_key_id          = new DbTypeDef (Res.Types.Num.KeyId);
				this.num_type_nullable_key_id = new DbTypeDef (Res.Types.Num.NullableKeyId);
				this.num_type_key_status      = new DbTypeDef (Res.Types.Num.KeyStatus);
				this.num_type_req_ex_state    = new DbTypeDef (Res.Types.Num.ReqExecState);
			
				this.infrastructure.internal_types.Add (this.num_type_key_id);
				this.infrastructure.internal_types.Add (this.num_type_nullable_key_id);
				this.infrastructure.internal_types.Add (this.num_type_key_status);
				this.infrastructure.internal_types.Add (this.num_type_req_ex_state);
			}

			void InitializeOtherTypes()
			{
				this.d_t_type_datetime   = new DbTypeDef (Res.Types.Other.DateTime);
				this.bin_type_req_data   = new DbTypeDef (Res.Types.Other.ReqData);
		
				this.infrastructure.internal_types.Add (this.d_t_type_datetime);
				this.infrastructure.internal_types.Add (this.bin_type_req_data);
			}

			void InitializeStrTypes()
			{
				this.str_type_name        = new DbTypeDef (Res.Types.Str.Name);
				this.str_type_info_xml    = new DbTypeDef (Res.Types.Str.InfoXml);
				this.str_type_dict_key    = new DbTypeDef (Res.Types.Str.Dict.Key);
				this.str_type_dict_value  = new DbTypeDef (Res.Types.Str.Dict.Value);
			
				this.infrastructure.internal_types.Add (this.str_type_name);
				this.infrastructure.internal_types.Add (this.str_type_info_xml);
				this.infrastructure.internal_types.Add (this.str_type_dict_key);
				this.infrastructure.internal_types.Add (this.str_type_dict_value);
			}
			
			
			void AssertAllTypesReady ()
			{
				System.Diagnostics.Debug.Assert (this.num_type_key_id != null);
				System.Diagnostics.Debug.Assert (this.num_type_nullable_key_id != null);
				System.Diagnostics.Debug.Assert (this.num_type_key_status != null);
				System.Diagnostics.Debug.Assert (this.num_type_req_ex_state != null);
				
				System.Diagnostics.Debug.Assert (this.str_type_name != null);
				System.Diagnostics.Debug.Assert (this.str_type_info_xml != null);
				System.Diagnostics.Debug.Assert (this.str_type_dict_key != null);
				System.Diagnostics.Debug.Assert (this.str_type_dict_value != null);
				
				System.Diagnostics.Debug.Assert (this.d_t_type_datetime != null);
				System.Diagnostics.Debug.Assert (this.bin_type_req_data != null);
			}
			
			
			public DbTypeDef					KeyId
			{
				get
				{
					return this.num_type_key_id;
				}
			}

			public DbTypeDef					NullableKeyId
			{
				get
				{
					return this.num_type_nullable_key_id;
				}
			}

			public DbTypeDef					KeyStatus
			{
				get
				{
					return this.num_type_key_status;
				}
			}

			public DbTypeDef ReqExecState
			{
				get
				{
					return this.num_type_req_ex_state;
				}
			}

			public DbTypeDef					DateTime
			{
				get
				{
					return this.d_t_type_datetime;
				}
			}

			public DbTypeDef ReqData
			{
				get
				{
					return this.bin_type_req_data;
				}
			}

			public DbTypeDef Name
			{
				get
				{
					return this.str_type_name;
				}
			}

			public DbTypeDef InfoXml
			{
				get
				{
					return this.str_type_info_xml;
				}
			}

			public DbTypeDef DictKey
			{
				get
				{
					return this.str_type_dict_key;
				}
			}

			public DbTypeDef DictValue
			{
				get
				{
					return this.str_type_dict_value;
				}
			}
			
			
			protected DbInfrastructure			infrastructure;

			protected DbTypeDef num_type_key_id;
			protected DbTypeDef num_type_nullable_key_id;
			protected DbTypeDef num_type_key_status;
			protected DbTypeDef num_type_req_ex_state;

			protected DbTypeDef d_t_type_datetime;
			protected DbTypeDef bin_type_req_data;

			protected DbTypeDef str_type_name;
			protected DbTypeDef str_type_info_xml;
			protected DbTypeDef str_type_dict_key;
			protected DbTypeDef str_type_dict_value;
		}
		#endregion
		
		
		protected DbAccess						db_access;
		protected IDbAbstraction				db_abstraction;
		
		protected ISqlEngine					sql_engine;
		protected ITypeConverter				type_converter;
		
		private TypeHelper						types;
		private DbLogger						logger;
		private DbClientManager					client_manager;
		
		private Settings.Globals				globals;
		private Settings.Locals					locals;
		
		protected Collections.DbTables			internal_tables = new Collections.DbTables ();
		protected Collections.DbTypeDefs		internal_types  = new Collections.DbTypeDefs ();
		
		protected int							client_id;
		
		CallbackDisplayDataSet					displayDataSet;
		string[]								localisations;
		
		Cache.DbTypeDefs						cache_db_types = new Cache.DbTypeDefs ();
		Cache.DbTables							cache_db_tables = new Cache.DbTables ();
		
		protected System.Collections.Hashtable	live_transactions;
		protected System.Collections.ArrayList	release_requested;
		protected int							lock_timeout = 15000;
		System.Threading.ReaderWriterLock		global_lock = new System.Threading.ReaderWriterLock ();
	}
}
