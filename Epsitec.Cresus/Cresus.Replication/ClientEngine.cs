//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Cresus.Database;

namespace Epsitec.Cresus.Replication
{
	/// <summary>
	/// Summary description for ClientEngine.
	/// </summary>
	public class ClientEngine
	{
		public ClientEngine(DbInfrastructure infrastructure)
		{
			this.infrastructure = infrastructure;
		}
		
		public void RequestRollback()
		{
			//	TODO: Demande au serveur de répliquer tout ce qui a été modifié depuis
			//	l'instant spécifié.
		}
		
		public void ApplyChanges(IDbAbstraction database, byte[] compressed_data)
		{
			this.ApplyChanges (database, Common.IO.Serialization.DeserializeAndDecompressFromMemory (compressed_data) as ReplicationData);
		}
		
		public void ApplyChanges(IDbAbstraction database, ReplicationData data)
		{
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			
			list.AddRange (data.TableData);
			
			PackedTableData def_table   = ClientEngine.FindPackedTable (list, Tags.TableTableDef);
			PackedTableData def_column  = ClientEngine.FindPackedTable (list, Tags.TableColumnDef);
			PackedTableData def_type    = ClientEngine.FindPackedTable (list, Tags.TableTypeDef);
			PackedTableData def_enumval = ClientEngine.FindPackedTable (list, Tags.TableEnumValDef);
			
			//	Il faut appliquer les changements concernant les tables de gestion internes
			//	(s'il y en a) avant de pouvoir appliquer les autres changements :
			
			if ((def_table != null) ||
				(def_column != null) ||
				(def_type != null) ||
				(def_enumval != null))
			{
				//	Ces opérations ne sont possibles qu'au sein d'un bloc d'exclusion global au
				//	niveau des accès à la base de données :
				
				this.infrastructure.GlobalLock ();
				
				try
				{
					using (DbTransaction transaction = infrastructure.BeginTransaction (DbTransactionMode.ReadWrite, database))
					{
						if (def_table  != null)  this.ApplyChanges (transaction, def_table);
						if (def_column != null)  this.ApplyChanges (transaction, def_column);
						if (def_type   != null)  this.ApplyChanges (transaction, def_type);
						if (def_enumval != null) this.ApplyChanges (transaction, def_enumval);
						
						//	Il est indispensable de valider la transaction à ce stade, car on va peut-être
						//	modifier la structure interne de la base de données, et cela ne sera visible
						//	qu'après validation :
						
						this.infrastructure.ClearCaches ();
						
						transaction.Commit ();
					}
					
					list.Remove (def_table);
					list.Remove (def_column);
					list.Remove (def_type);
					list.Remove (def_enumval);
					
					//	Met à jour la structure de la base de données selon les nouvelles descriptions de
					//	tables/colonnes/types :
					
					this.ApplyStructuralChanges (database, def_table, def_column, def_type);
					
					using (DbTransaction transaction = infrastructure.BeginTransaction (DbTransactionMode.ReadWrite, database))
					{
						this.ApplyChanges (transaction, list);
						transaction.Commit ();
					}
				}
				finally
				{
					this.infrastructure.GlobalUnlock ();
				}
			}
			else
			{
				using (DbTransaction transaction = infrastructure.BeginTransaction (DbTransactionMode.ReadWrite, database))
				{
					this.ApplyChanges (transaction, list);
					transaction.Commit ();
				}
			}
		}
		
		protected void ApplyChanges(DbTransaction transaction, System.Collections.ArrayList list)
		{
			//	Applique les modifications pour toutes les tables de la liste :
			
			foreach (PackedTableData data in list)
			{
				this.ApplyChanges (transaction, data);
			}
		}
		
		protected void ApplyChanges(DbTransaction transaction, PackedTableData data)
		{
			//	Applique les modifications décrites pour la table spécifiée. Pour ce faire,
			//	on remplit une table avec les lignes à répliquer et on utiliser un 'REPLACE'
			//	de toutes celles-ci :
			
			DbTable table = this.infrastructure.ResolveDbTable (transaction, new DbKey (data.Key));
			
			using (DbRichCommand command = DbRichCommand.CreateFromTable (this.infrastructure, transaction, table))
			{
				System.Diagnostics.Debug.Assert (command.DataSet != null);
				System.Diagnostics.Debug.Assert (command.DataSet.Tables.Count == 1);
				
				System.Data.DataTable data_table = command.DataSet.Tables[0];
				
				data.FillTable (data_table);
				
				System.Diagnostics.Debug.Assert (data_table.Rows.Count > 0);
				
				command.ReplaceTables (transaction);
			}
		}
		
		protected void ApplyStructuralChanges(IDbAbstraction database, PackedTableData def_table, PackedTableData def_column, PackedTableData def_type)
		{
			//	Applique des modifications structurelles (tables, colonnes, types...)
			//	Pour l'instant, seule la création d'une nouvelle table est gérée.
			
			//	TODO: gérer les mises à jour de tables existantes (modification des types et colonnes).
			
			if (def_table != null)
			{
				object[][] def_table_rows = def_table.GetValuesArray ();
				
				//	Passe en revue toutes les lignes qui ont changé dans CR_TABLE :
				
				for (int i = 0; i < def_table_rows.Length; i++)
				{
					DbKey   def_table_row_key = new DbKey (def_table_rows[i]);
					DbTable def_table_runtime = this.infrastructure.ResolveDbTable (null, def_table_row_key);
					
					System.Diagnostics.Debug.Assert (def_table_runtime != null);
					
					if (def_table_row_key.Status == DbRowStatus.Deleted)
					{
						//	La table a été supprimée. Dans les faits, on ne la supprime jamais
						//	de table dans la base de données. Il n'y a donc rien à faire...
						
						System.Diagnostics.Debug.WriteLine (string.Format ("Replication: table {0} was deleted.", def_table_runtime.Name));
					}
					else
					{
						//	La table a été modifiée (peut-être créée).
						
						string   find_sql_name    = def_table_runtime.CreateSqlName ();
						string[] known_sql_tables = database.UserTableNames;
						
						bool found = false;
						
						for (int j = 0; j < known_sql_tables.Length; j++)
						{
							if (known_sql_tables[j] == find_sql_name)
							{
								found = true;
								break;
							}
						}
						
						if (found)
						{
							System.Diagnostics.Debug.WriteLine (string.Format ("Replication: table {0} was modified. SQL Name is {1}.", def_table_runtime.Name, find_sql_name));
							
							//	TODO: gérer la mise à jour de la table...
						}
						else
						{
							System.Diagnostics.Debug.WriteLine (string.Format ("Replication: table {0} was created. SQL Name is {1}.", def_table_runtime.Name, find_sql_name));
							
							//	La table doit être créée. On va simplement créer la table dans la base
							//	de données, sans créer les informations dans CR_TABLE/CR_COLUMN, car
							//	celles-ci sont déjà présentes :
							
							using (DbTransaction transaction = this.infrastructure.BeginTransaction (DbTransactionMode.ReadWrite, database))
							{
								this.infrastructure.RegisterKnownDbTable (transaction, def_table_runtime);
								
								transaction.Commit ();
							}
						}
					}
				}
			}
		}
		
		
		private static PackedTableData FindPackedTable(System.Collections.ArrayList list, string name)
		{
			foreach (PackedTableData packed_table in list)
			{
				if (packed_table.Name == name)
				{
					return packed_table;
				}
			}
			
			return null;
		}
		
		
		private DbInfrastructure				infrastructure;
	}
}
