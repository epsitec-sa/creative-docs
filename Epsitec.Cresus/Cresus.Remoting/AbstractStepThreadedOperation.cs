//	Copyright © 2004-2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Cresus.Remoting
{
	/// <summary>
	/// The <c>AbstractStepThreadedOperation</c> class executes a multi step
	/// operation on a dedicated thread.
	/// </summary>
	public abstract class AbstractStepThreadedOperation : AbstractThreadedOperation
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AbstractStepThreadedOperation"/> class.
		/// </summary>
		protected AbstractStepThreadedOperation()
		{
			this.steps = new List<System.Action> ();
		}


		/// <summary>
		/// Adds the specified step to the list of steps executed by this
		/// operation.
		/// </summary>
		/// <param name="step">The step.</param>
		protected void Add(System.Action step)
		{
			lock (this.exclusion)
			{
				this.steps.Add (step);
			}
		}


		/// <summary>
		/// Does the real work for this operation, by executing one step after the
		/// other.
		/// </summary>
		protected override void ProcessOperation()
		{
			try
			{
				this.SetupSteps ();

				for (;;)
				{
					System.Action step;

					lock (this.exclusion)
					{
						if (this.stepIndex < this.steps.Count)
						{
							step = this.steps[this.stepIndex];
						}
						else
						{
							break;
						}
					}
					
					this.IncrementStep ();
					
					if (this.IsCancelRequested)
					{
						this.SetCanceled ();
						return;
					}
					
					step ();
				}
				
				this.SetProgress (100);
			}
			catch (System.Exception exception)
			{
				this.SetFailed (string.Concat (exception.Message, "\n", exception.StackTrace));
			}
		}


		/// <summary>
		/// Prepares the operation for the current known number of steps.
		/// </summary>
		private void SetupSteps()
		{
			int n;

			lock (this.exclusion)
			{
				this.stepIndex = 0;
				n = this.steps.Count;
			}

			this.SetLastExpectedStep (n);
		}

		/// <summary>
		/// Increments the step and notifies the abstract operation class of the
		/// progress we have made so far.
		/// </summary>
		private void IncrementStep()
		{
			int n;
			int progress;

			lock (this.exclusion)
			{
				this.stepIndex++;
				n = this.stepIndex;
				progress = 100 * this.stepIndex / (this.steps.Count + 1);
			}

			this.SetCurrentStep (n, progress);
		}
		
		private int								stepIndex;
		private List<System.Action>				steps;
	}
}
