//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Remoting
{
	/// <summary>
	/// Summary description for AbstractThreadedOperation.
	/// </summary>
	public abstract class AbstractThreadedOperation : AbstractOperation
	{
		public AbstractThreadedOperation()
		{
		}
		
		
		protected System.Threading.Thread		Thread
		{
			get
			{
				return this.thread;
			}
		}
		
		protected bool							IsCancelRequested
		{
			get
			{
				return this.is_thread_cancel_requested;
			}
		}
		
		
		public void Start()
		{
			if (this.thread != null)
			{
				new System.InvalidOperationException ("Thread cannot be started twice.");
			}
			
			this.thread = new System.Threading.Thread (new System.Threading.ThreadStart (this.ProcessOperation));
			this.thread.Start ();
		}
		
		
		public override void CancelOperation(out IProgressInformation progress_information)
		{
			this.is_thread_cancel_requested = true;
			
			if ((this.thread != null) &&
				(this.thread.IsAlive))
			{
				//	Il y a un processus en cours d'exécution pour cette opération; on va par
				//	conséquent demander "poliment" au processus de bien vouloir se terminer,
				//	via un événement plutôt que via un Thread.Abort (trop brutal).
				
				this.GetThreadCancelEvent (true).Set ();
				
				//	Signale l'état de l'avancement de l'opération en surveillant simplement
				//	la fin du processus :
				
				progress_information = new ThreadJoinProgress (this.Thread);
			}
			else
			{
				progress_information = new ImmediateProgress ();
			}
		}
		
		
		protected abstract void ProcessOperation();
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.thread != null)
				{
					//	Demande la fin de l'exécution du processus; poliment, d'abord, puis
					//	en cas d'échec après la durée de sursis, brutalement...
					
					IProgressInformation info;
					
					this.CancelOperation (out info);
					
					if (info.WaitForProgress (100, new System.TimeSpan (0, 0, 0, 0, this.wait_before_abort)) == false)
					{
						this.thread.Abort ();
					}
					
					this.thread.Join ();
					this.thread = null;
				}
			}
			
			base.Dispose (disposing);
		}
		
		
		System.Threading.AutoResetEvent GetThreadCancelEvent(bool create)
		{
			lock (this)
			{
				if ((this.thread_cancel == null) &&
					(create))
				{
					this.thread_cancel = new System.Threading.AutoResetEvent (false);
				}
				
				return this.thread_cancel;
			}
		}
		
		
		protected void InterruptIfCancelRequested()
		{
			if (this.IsCancelRequested)
			{
				throw new Exceptions.InterruptedException ();
			}
		}
		
		
		private System.Threading.Thread			thread;
		private System.Threading.AutoResetEvent	thread_cancel;
		private volatile bool					is_thread_cancel_requested;
		
		protected int							wait_before_abort = 100;
	}
}
