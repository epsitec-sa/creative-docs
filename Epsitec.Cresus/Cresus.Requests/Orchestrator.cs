//	Copyright � 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Cresus.Database;

namespace Epsitec.Cresus.Requests
{
	/// <summary>
	/// La classe Orchestrator g�re l'arriv�e de requ�tes, leur mise en queue et
	/// leur traitement.
	/// </summary>
	public class Orchestrator
	{
		public Orchestrator(DbInfrastructure infrastructure)
		{
			this.infrastructure   = infrastructure;
			this.database         = this.infrastructure.CreateDbAbstraction ();
			this.execution_engine = new ExecutionEngine (this.infrastructure);
			this.execution_queue  = new ExecutionQueue (this.infrastructure, this.database);
		}
		
		
		public void WorkerThread()
		{
			System.Threading.WaitHandle[] handles = new System.Threading.WaitHandle[2];
			
			handles[0] = this.execution_queue.EnqueueEvent;
			handles[1] = Common.Support.Globals.AbortEvent;
			
			for (;;)
			{
				int handle_index = System.Threading.WaitHandle.WaitAny (handles);
				
				if (handle_index >= 128)
				{
					handle_index -= 128;
				}
				
				if (handle_index != 0)
				{
					break;
				}
				
				this.ProcessQueue ();
			}
		}
		
		public void ProcessQueue()
		{
			//	Passe en revue la queue � la recherche de requ�tes en attente d'ex�cution.
			
			System.Data.DataRowCollection rows = this.execution_queue.Rows;
			
			//	Prend note du nombre de lignes dans la queue; si des nouvelles lignes sont
			//	rajout�es pendant notre ex�cution, on les ignore. Elles seront trait�es au
			//	prochain tour.
			
			int n = rows.Count;
			
			for (int i = 0; i < n; i++)
			{
				ExecutionState state = this.execution_queue.GetRequestExecutionState (rows[i]);
				
				switch (state)
				{
					case ExecutionState.Pending:
					case ExecutionState.ConflictResolved:
						this.ProcessPendingRequest (i, this.execution_queue.GetRequest (rows[i]));
						break;
					
					default:
						break;
				}
			}
		}
		
		protected virtual void ProcessPendingRequest(int row_index, AbstractRequest request)
		{
		}
		
		
		protected DbInfrastructure				infrastructure;
		protected ExecutionQueue				execution_queue;
		protected ExecutionEngine				execution_engine;
		protected IDbAbstraction				database;
	}
}
