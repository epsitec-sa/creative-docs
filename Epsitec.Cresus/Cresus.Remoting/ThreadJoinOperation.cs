//	Copyright © 2004-2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Remoting
{
	/// <summary>
	/// The <c>ThreadJoinOperation</c> class implements an operation which waits on
	/// a (single) thread join.
	/// </summary>
	public sealed class ThreadJoinOperation : AbstractOperation
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ThreadJoinOperation"/> class.
		/// </summary>
		/// <param name="thread">The thread.</param>
		public ThreadJoinOperation(System.Threading.Thread thread)
		{
			this.thread = thread;
		}


		protected override void RefreshProgressInformation()
		{
			this.PollState ();
			base.RefreshProgressInformation ();
		}

		
		public override bool WaitForProgress(int minimumProgress, System.TimeSpan timeout)
		{
			if (minimumProgress <= 0)
			{
				return true;
			}

			if (minimumProgress > 100)
			{
				System.Threading.Thread.Sleep (timeout);
				return false;
			}
			
			//	Simply wait on the join, using the specified timeout value.
			return this.thread.Join (timeout);
		}

		
		private void PollState()
		{
			if (this.thread == null || !this.thread.IsAlive)
			{
				this.SetProgress (100);
			}
		}
		
		
		private readonly System.Threading.Thread thread;
	}
}
