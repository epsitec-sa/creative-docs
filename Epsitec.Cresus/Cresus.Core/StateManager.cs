//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Cresus.Core
{
	/// <summary>
	/// The <c>StateManager</c> class is used to manage a collection of states.
	/// </summary>
	public sealed class StateManager
	{
		public StateManager()
		{
			this.states = new List<States.AbstractState> ();
		}


		public int Depth
		{
			get
			{
				return this.states.Count;
			}
		}


		public void Push(States.AbstractState state)
		{
			if (this.states.Remove (state))
			{
				this.states.Add (state);
				this.OnStateStackChanged (new StateStackChangedEventArgs (StateStackChange.Promotion, state));
			}
			else
			{
				this.states.Add (state);
				this.OnStateStackChanged (new StateStackChangedEventArgs (StateStackChange.Push, state));
			}
		}

		public bool Pop(States.AbstractState state)
		{
			if (this.states.Remove (state))
			{
				this.OnStateStackChanged (new StateStackChangedEventArgs (StateStackChange.Pop, state));
				return true;
			}
			else
			{
				return false;
			}
		}

		public States.AbstractState Peek()
		{
			int n = this.states.Count;

			if (n > 0)
			{
				return this.states[n-1];
			}
			else
			{
				return null;
			}
		}


		private void OnStateStackChanged(StateStackChangedEventArgs e)
		{
			if (this.StackChanged != null)
			{
				this.StackChanged (this, e);
			}
		}

		public event System.EventHandler<StateStackChangedEventArgs>	StackChanged;

		private readonly List<States.AbstractState> states;
	}
}
