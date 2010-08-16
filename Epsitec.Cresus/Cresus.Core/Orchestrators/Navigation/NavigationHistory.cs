//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Orchestrators.Navigation
{
	public class NavigationHistory
	{
		public NavigationHistory(NavigationOrchestrator navigator)
		{
			this.navigator = navigator;
			this.backwardHistory = new Stack<NavigationPath> ();
			this.forwardHistory  = new Stack<NavigationPath> ();
		}


		public void Record(NavigationPath fullPath)
		{
			switch (this.state)
			{
				case State.Neutral:
					this.backwardHistory.Push (fullPath);
					this.forwardHistory.Clear ();
					break;

				case State.NavigateBackward:
					this.forwardHistory.Push (fullPath);
					break;

				case State.NavigateForward:
					this.backwardHistory.Push (fullPath);
					break;
			}

			System.Diagnostics.Debug.WriteLine (fullPath.ToString ());
		}

		public bool NavigateBackward()
		{
			if (this.backwardHistory.Count > 0)
            {
				using (new StatePreserver (this, State.NavigateBackward))
				{
					return this.backwardHistory.Pop ().Navigate (this.navigator);
				}
            }

			return false;
		}

		public bool NavigateForward()
		{
			if (this.forwardHistory.Count > 0)
			{
				using (new StatePreserver (this, State.NavigateForward))
				{
					return this.forwardHistory.Pop ().Navigate (this.navigator);
				}
			}

			return false;
		}



		private void NotifyNavigation()
		{
			System.Diagnostics.Debug.WriteLine ("------------------------------------------------------------");
			System.Diagnostics.Debug.WriteLine ("Past :");
			System.Diagnostics.Debug.WriteLine (string.Join ("\r\n", this.backwardHistory.Select (x => x.ToString ()).ToArray ()));
			System.Diagnostics.Debug.WriteLine ("------------------------------------------------------------");
			System.Diagnostics.Debug.WriteLine ("Future :");
			System.Diagnostics.Debug.WriteLine (string.Join ("\r\n", this.forwardHistory.Select (x => x.ToString ()).ToArray ()));
		}
		
		private enum State
		{
			Neutral,
			NavigateBackward,
			NavigateForward,
		}

		private class StatePreserver : System.IDisposable
		{
			public StatePreserver(NavigationHistory history, State newState)
			{
				this.history = history;
				this.oldState = this.history.state;
				this.history.state = newState;
			}

			#region IDisposable Members

			public void Dispose()
			{
				this.history.state = this.oldState;
				this.history.NotifyNavigation ();
			}

			#endregion

			private readonly NavigationHistory history;
			private readonly State oldState;
		}


		private readonly NavigationOrchestrator navigator;
		private readonly Stack<NavigationPath> backwardHistory;
		private readonly Stack<NavigationPath> forwardHistory;
		
		private State state;
	}
}
