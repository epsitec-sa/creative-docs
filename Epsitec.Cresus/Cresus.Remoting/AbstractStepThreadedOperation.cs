//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Remoting
{
	/// <summary>
	/// Summary description for AbstractStepThreadedOperation.
	/// </summary>
	public abstract class AbstractStepThreadedOperation : AbstractThreadedOperation
	{
		public AbstractStepThreadedOperation()
		{
			this.steps = new System.Collections.ArrayList ();
		}
		
		
		protected void Add(Step step)
		{
			this.steps.Add (step);
		}
		
		
		protected override void ProcessOperation()
		{
			try
			{
				this.SetupSteps ();
				
				foreach (Step step in this.steps)
				{
					this.IncrementStep ();
					this.InterruptIfCancelRequested ();
					
					step ();
				}
				
				this.SetProgress (100);
			}
			catch (Remoting.Exceptions.InterruptedException)
			{
				this.SetCancelled ();
			}
			catch (System.Exception exception)
			{
				this.SetFailed (exception.ToString ());
			}
			finally
			{
			}
		}
		
		
		private void SetupSteps()
		{
			this.step_index = 0;
			this.SetLastStep (this.steps.Count);
		}
		
		private void IncrementStep()
		{
			this.step_index++;
			this.SetCurrentStep (this.step_index);
		}
		
		protected delegate void Step();
			
		
		private int								step_index;
		private System.Collections.ArrayList	steps;
	}
}
