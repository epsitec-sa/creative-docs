//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Cresus.Database;

namespace Epsitec.Cresus.Requests
{
	/// <summary>
	/// La classe Orchestrator gère l'arrivée de requêtes, leur mise en queue et
	/// leur traitement.
	/// </summary>
	public class Orchestrator
	{
		public Orchestrator(DbInfrastructure infrastructure)
		{
			this.infrastructure   = infrastructure;
			this.execution_engine = new ExecutionEngine (this.infrastructure);
			this.execution_queue  = new ExecutionQueue (this.infrastructure);
			
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
			//	TODO: gère le contenu de la queue.
		}
		
		
		protected DbInfrastructure				infrastructure;
		protected ExecutionQueue				execution_queue;
		protected ExecutionEngine				execution_engine;
	}
}
