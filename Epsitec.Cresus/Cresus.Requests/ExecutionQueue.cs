//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Cresus.Database;
using System.Runtime.Serialization.Formatters.Binary;


namespace Epsitec.Cresus.Requests
{
	/// <summary>
	/// La classe ExecutionQueue permet de représenter la queue des requêtes
	/// en attente d'exécution.
	/// </summary>
	public class ExecutionQueue : IAttachable, IPersistable
	{
		public ExecutionQueue(DbInfrastructure infrastructure)
		{
			this.Setup (infrastructure);
		}
		
		
		
		public System.Data.DataRowCollection	Rows
		{
			get
			{
				return this.queue_data_set.Tables[Tags.TableRequestQueue].Rows;
			}
		}
		
		
		public void Add(Base request)
		{
			BinaryFormatter formatter = new BinaryFormatter ();
			System.IO.MemoryStream stream = new System.IO.MemoryStream ();
			formatter.Serialize (stream, request);
			stream.Close ();
			
			byte[] buffer = stream.ToArray ();
			int    length = buffer.Length;
			
			System.Diagnostics.Debug.WriteLine ("Buffer written, contains " + length + " bytes.");
			
			System.Data.DataRow data_row;
			
			this.queue_command.CreateNewRow (Tags.TableRequestQueue, out data_row);
			
			data_row.BeginEdit ();
			data_row[Tags.ColumnQueueData] = buffer;
			data_row.EndEdit ();
		}
		
		
		
		#region IAttachable Members
		public void Attach(DbInfrastructure infrastructure, DbTable table)
		{
			this.infrastructure = infrastructure;
			this.queue_db_table = table;
		}
		
		public void Detach()
		{
			this.infrastructure = null;
			this.queue_db_table = null;
		}
		#endregion
		
		#region IPersistable Members
		public void RestoreFromBase(DbTransaction transaction)
		{
			System.Diagnostics.Debug.Assert (transaction != null);
			System.Diagnostics.Debug.Assert (this.infrastructure != null);
			
			this.queue_command  = DbRichCommand.CreateFromTable (this.infrastructure, transaction, this.queue_db_table, DbSelectRevision.LiveActive);
			this.queue_data_set = this.queue_command.DataSet;
		}
		
		public void SerializeToBase(DbTransaction transaction)
		{
			System.Diagnostics.Debug.Assert (transaction != null);
			System.Diagnostics.Debug.Assert (this.queue_command != null);
			
			this.queue_command.UpdateRealIds (transaction);
			this.queue_command.UpdateTables (transaction);
			
			this.queue_command.AcceptChanges ();
		}
		#endregion
		
		private void Setup(DbInfrastructure infrastructure)
		{
			using (DbTransaction transaction = infrastructure.BeginTransaction (DbTransactionMode.ReadOnly))
			{
				DbTable table = infrastructure.ResolveDbTable (transaction, Tags.TableRequestQueue);
				
				this.Attach (infrastructure, table);
				this.RestoreFromBase (transaction);
				
				transaction.Commit ();
			}
		}
		
		
		private DbInfrastructure				infrastructure;
		private DbTable							queue_db_table;
		private DbRichCommand					queue_command;
		private System.Data.DataSet				queue_data_set;
	}
}
