//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Remoting
{
	/// <summary>
	/// La classe ThreadJoinProgress permet d'attendre la fin de l'exécution
	/// d'un processus.
	/// </summary>
	public sealed class ThreadJoinProgress : AbstractProgress
	{
		public ThreadJoinProgress(System.Threading.Thread thread)
		{
			this.thread = thread;
		}
		
		
		public override int						ProgressPercent
		{
			get
			{
				this.PollState ();
				
				return base.ProgressPercent;
			}
		}

		
		
		public override bool WaitForProgress(int minimum_progress, System.TimeSpan timeout)
		{
			if (minimum_progress <= 0)
			{
				return true;
			}
			if (minimum_progress > 100)
			{
				System.Threading.Thread.Sleep (timeout);
				return false;
			}
			
			//	En attendant directement sur l'événement de fin d'exécution du processus, on
			//	se simplifie énormément la vie par rapport à l'implémentation par défaut :
			
			if (this.thread.Join (timeout))
			{
				//	Le processus a fini de s'exécuter, donc c'est OK.
				
				return true;
			}
			
			return false;
		}

		
		private void PollState()
		{
			if ((this.thread == null) ||
				(this.thread.IsAlive == false))
			{
				this.SetProgress (100);
			}
		}
		
		
		private System.Threading.Thread			thread;
	}
}
