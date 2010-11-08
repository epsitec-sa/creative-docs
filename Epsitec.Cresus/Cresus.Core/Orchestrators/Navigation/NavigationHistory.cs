//	Copyright � 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
			this.UpdateNavigationCommands ();
		}


		public void Record(NavigationPath fullPath)
		{
			if (this.suspendCounter > 0)
            {
				if ((this.state == State.Neutral) ||
					(this.state == State.NavigateInPlace))
				{
					return;
				}

				throw new System.InvalidOperationException ("Navigation is forbidden when NavigationHistory is suspended");
            }

			if (fullPath.IsReadOnly == false)
			{
				fullPath = new NavigationPath (fullPath);
				fullPath.Freeze ();
			}

			switch (this.state)
			{
				case State.Neutral:
					if (NavigationHistory.RecordOnlyIfDifferent (this.backwardHistory, fullPath))
					{
						this.forwardHistory.Clear ();
					}
					break;

				case State.NavigateBackward:
					this.forwardHistory.Push (fullPath);
					break;

				case State.NavigateForward:
					this.backwardHistory.Push (fullPath);
					break;

				case State.NavigateInPlace:
					return;
			}

			this.UpdateNavigationCommands ();
			System.Diagnostics.Debug.WriteLine ("Recorded full path " + fullPath.ToString ());
//			this.DebugDump ();
		}

		/// <summary>
		/// Suspends recording. As long as recording is suspended, nothing will be recorded
		/// by method <see cref="Record"/>.
		/// </summary>
		/// <returns>The object which must be used in a <c>using</c> block.</returns>
		public System.IDisposable SuspendRecording()
		{
			return new Suspender (this, "SuspendRecording");
		}

		public bool NavigateBackward()
		{
			if (this.backwardHistory.Count > 0)
			{
				using (new StatePreserver (this, State.NavigateBackward))
				{
					this.navigator.NotifyAboutToNavigateHistory ();
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
					this.navigator.NotifyAboutToNavigateHistory ();
					return this.forwardHistory.Pop ().Navigate (this.navigator);
				}
			}

			return false;
		}

		public bool NavigateInPlace(NavigationPath fullPath)
		{
			using (new StatePreserver (this, State.NavigateInPlace))
			{
				return fullPath.Navigate (this.navigator);
			}
		}
		
		private void UpdateNavigationCommands()
		{
			var commandContext = this.navigator.MainViewController.CommandContext;

			commandContext.GetCommandState (Res.Commands.History.NavigateBackward).Enable = this.backwardHistory.Count > 0;
			commandContext.GetCommandState (Res.Commands.History.NavigateForward).Enable  = this.forwardHistory.Count > 0;
		}

		private static bool RecordOnlyIfDifferent(Stack<NavigationPath> stack, NavigationPath item)
		{
			if ((stack.Count > 0) &&
				(stack.Peek ().Equals (item)))
			{
				return false;
			}
			else
			{
				stack.Push (item);
				return true;
			}
		}



		private void DebugDump()
		{
			System.Diagnostics.Debug.WriteLine ("[-----------------------------------------------------------");
			System.Diagnostics.Debug.WriteLine ("Past :");
			System.Diagnostics.Debug.WriteLine (string.Join ("\r\n", this.backwardHistory.Select (x => x.ToString ()).ToArray ()));
			System.Diagnostics.Debug.WriteLine ("------------------------------------------------------------");
			System.Diagnostics.Debug.WriteLine ("Future :");
			System.Diagnostics.Debug.WriteLine (string.Join ("\r\n", this.forwardHistory.Select (x => x.ToString ()).ToArray ()));
			System.Diagnostics.Debug.WriteLine ("-----------------------------------------------------------]");
		}
		
		private enum State
		{
			Neutral,
			NavigateBackward,
			NavigateForward,
			NavigateInPlace,
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
				this.history.UpdateNavigationCommands ();
				this.history.DebugDump ();
			}

			#endregion

			private readonly NavigationHistory history;
			private readonly State oldState;
		}

		private class Suspender : System.IDisposable
		{
			public Suspender(NavigationHistory history, string name)
			{
				this.name = name;
				this.history = history;
				this.history.suspendCounter++;
			}

			~Suspender()
			{
				throw new System.InvalidOperationException (string.Format ("The object returned by {0}.{1} was not properly disposed. Call {1} in a using block.", typeof (NavigationHistory).Name, this.name));
			}

			#region IDisposable Members

			void System.IDisposable.Dispose()
			{
				this.history.suspendCounter--;
				System.GC.SuppressFinalize (this);
			}

			#endregion
			
			private readonly string name;
			private readonly NavigationHistory history;
		}

		private readonly NavigationOrchestrator navigator;
		private readonly Stack<NavigationPath> backwardHistory;
		private readonly Stack<NavigationPath> forwardHistory;
		
		private int suspendCounter;
		private State state;
	}
}
