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
			this.ApplyChanges (database, DataCruncher.DeserializeAndDecompressFromMemory (compressed_data) as ReplicationData);
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
					
					this.ApplyStructuralChanges (def_table, def_column, def_type);
					
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
			foreach (PackedTableData data in list)
			{
				this.ApplyChanges (transaction, data);
			}
		}
		
		protected void ApplyChanges(DbTransaction transaction, PackedTableData data)
		{
			//	Applique les modifications décrites pour la table spécifiée.
			
			System.Diagnostics.Debug.WriteLine (string.Format ("Applying changes in table {0}.", data.Name));
			
			
		}
		
		protected void ApplyStructuralChanges(PackedTableData def_table, PackedTableData def_column, PackedTableData def_type)
		{
			if (def_table != null)
			{
				object[][] def_table_rows = def_table.GetValuesArray ();
				
				for (int i = 0; i < def_table_rows.Length; i++)
				{
					DbKey   def_table_row_key = new DbKey (def_table_rows[i]);
					DbTable def_table_runtime = this.infrastructure.ResolveDbTable (null, def_table_row_key);
					
					System.Diagnostics.Debug.Assert (def_table_runtime != null);
					
					if (def_table_row_key.Status == DbRowStatus.Deleted)
					{
						//	La table a été supprimée. Dans les faits, on ne la supprime pas de la base
						//	de données.
						
						System.Diagnostics.Debug.WriteLine ("Replication: table {0} was deleted.", def_table_runtime.Name);
					}
					else
					{
						//	La table a été modifiée et peut-être même créée...
						
						System.Diagnostics.Debug.WriteLine ("Replication: table {0} was modified/created.", def_table_runtime.Name);
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
