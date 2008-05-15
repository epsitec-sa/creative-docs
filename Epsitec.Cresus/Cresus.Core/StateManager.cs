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
			this.zOrder = new List<States.AbstractState> ();
		}


		public int Depth
		{
			get
			{
				return this.states.Count;
			}
		}

		public States.AbstractState ActiveState
		{
			get
			{
				if (this.zOrder.Count > 0)
				{
					return this.zOrder[0];
				}
				else
				{
					return null;
				}
			}
		}


		public void Push(States.AbstractState state)
		{
			if (this.states.Contains (state))
			{
				this.zOrder.Remove (state);
				this.zOrder.Insert (0, state);

				this.OnStateStackChanged (new StateStackChangedEventArgs (StateStackChange.Promotion, state));
			}
			else
			{
				this.states.Add (state);
				this.zOrder.Insert (0, state);

				this.OnStateStackChanged (new StateStackChangedEventArgs (StateStackChange.Push, state));
			}
		}

		public bool Pop(States.AbstractState state)
		{
			if (this.states.Remove (state))
			{
				this.zOrder.Remove (state);

				this.OnStateStackChanged (new StateStackChangedEventArgs (StateStackChange.Pop, state));
				
				return true;
			}
			else
			{
				return false;
			}
		}

		

		private void OnStateStackChanged(StateStackChangedEventArgs e)
		{
			this.UpdateStateZOrder ();

			if (this.StackChanged != null)
			{
				this.StackChanged (this, e);
			}
		}

		private void UpdateStateZOrder()
		{
			for (int i = 0; i < this.zOrder.Count; i++)
			{
				this.zOrder[i].ZOrder = i;
			}
		}

		public event System.EventHandler<StateStackChangedEventArgs>	StackChanged;

		private readonly List<States.AbstractState> states;
		private readonly List<States.AbstractState> zOrder;
	}
}
