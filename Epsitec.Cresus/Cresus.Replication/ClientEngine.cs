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
			
			this.infrastructure.GlobalLock ();
			
			try
			{
				this.ApplyChanges (database, list);
			}
			finally
			{
				this.infrastructure.GlobalUnlock ();
			}
		}
		
		protected void ApplyChanges(IDbAbstraction database, System.Collections.ArrayList list)
		{
			//	Cette méthode doit s'exécuter dans un bloc verrouillé (aucune exécution concurrente
			//	n'est tolérée) :
			
			System.Diagnostics.Debug.Assert (this.infrastructure.IsInGlobalLock);
			
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
				using (DbTransaction transaction = infrastructure.BeginTransaction (DbTransactionMode.ReadWrite, database))
				{
					this.ApplyChanges (transaction, def_table);
					this.ApplyChanges (transaction, def_column);
					this.ApplyChanges (transaction, def_type);
					this.ApplyChanges (transaction, def_enumval);
					
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
			}
			
			//	A ce stade, les informations "run-time" sont à jour, mais pas les tables réelles
			//	dans la base de données. Peut-être faudra-t-il procéder à des modifications...
		}
		
		protected void ApplyChanges(DbTransaction transaction, PackedTableData data)
		{
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
