//	Copyright � 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Remoting
{
	/// <summary>
	/// La classe ThreadJoinProgress permet d'attendre la fin de l'ex�cution
	/// d'un processus.
	/// </summary>
	public sealed class ThreadJoinProgress : AbstractOperation
	{
		public ThreadJoinProgress(System.Threading.Thread thread)
		{
			this.thread = thread;
		}


		protected override void RefreshProgressInformation()
		{
			this.PollState ();
			base.RefreshProgressInformation ();
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
			
			//	En attendant directement sur l'�v�nement de fin d'ex�cution du processus, on
			//	se simplifie �norm�ment la vie par rapport � l'impl�mentation par d�faut :
			
			if (this.thread.Join (timeout))
			{
				//	Le processus a fini de s'ex�cuter, donc c'est OK.
				
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
