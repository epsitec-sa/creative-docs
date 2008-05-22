//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Epsitec.Cresus.Core
{
	/// <summary>
	/// The <c>StateManager</c> class is used to manage a collection of states.
	/// </summary>
	public sealed class StateManager
	{
		public StateManager(CoreApplication application)
		{
			this.application = application;
			this.states = new List<States.CoreState> ();
			this.zOrder = new List<States.CoreState> ();
			this.history = new CircularList<States.CoreState> ();
			this.boxes = new Dictionary<int, Box> ();
		}


		public CoreApplication					Application
		{
			get
			{
				return this.application;
			}
		}

		public int								Depth
		{
			get
			{
				return this.states.Count;
			}
		}

		public States.CoreState					ActiveState
		{
			get
			{
				if (this.history.Count > 0)
				{
					return this.history[0];
				}
				else
				{
					return null;
				}
			}
		}


		public void Push(States.CoreState state)
		{
			if (state == null)
			{
				return;
			}
			
			System.Diagnostics.Debug.Assert (state.StateManager == this);

			if (this.states.Contains (state))
			{
				this.history.Remove (state);
				this.history.Rotate (1);
				this.history.Reverse ();
				this.history.Insert (0, state);

				this.zOrder.Remove (state);
				this.zOrder.Add (state);

				this.ActivateCurrentState ();
				this.OnStateStackChanged (new StateStackChangedEventArgs (StateStackChange.Promotion, state));
				this.OnNavigated ();
			}
			else
			{
				this.states.Add (state);
				this.history.Insert (0, state);
				this.zOrder.Add (state);

				this.ActivateCurrentState ();
				this.OnStateStackChanged (new StateStackChangedEventArgs (StateStackChange.Push, state));
				this.OnNavigated ();
			}
		}

		public bool Pop(States.CoreState state)
		{
			if (state == null)
			{
				return false;
			}

			System.Diagnostics.Debug.Assert (state.StateManager == this);

			if (this.states.Remove (state))
			{
				this.history.Remove (state);
				this.zOrder.Remove (state);

				this.Detach (state);

				this.ActivateCurrentState ();
				this.OnStateStackChanged (new StateStackChangedEventArgs (StateStackChange.Pop, state));
				this.OnNavigated ();
				
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool NavigateHistoryPrev()
		{
			if (this.history.Count > 0)
			{
				this.history.Rotate (1);

				States.CoreState state = this.history[0];

				this.zOrder.Remove (state);
				this.zOrder.Add (state);

				this.ActivateCurrentState ();
				this.OnStateStackChanged (new StateStackChangedEventArgs (StateStackChange.Navigation, state));
				this.OnNavigated ();

				return true;
			}
			else
			{
				return false;
			}
		}

		public bool NavigateHistoryNext()
		{
			if (this.history.Count > 0)
			{
				this.history.Rotate (-1);

				States.CoreState state = this.history[0];

				this.zOrder.Remove (state);
				this.zOrder.Add (state);

				this.ActivateCurrentState ();
				this.OnStateStackChanged (new StateStackChangedEventArgs (StateStackChange.Navigation, state));
				this.OnNavigated ();

				return true;
			}
			else
			{
				return false;
			}
		}


		public IEnumerable<States.CoreState> GetAllStates()
		{
			return this.states;
		}

		public IEnumerable<States.CoreState> GetAllStates(States.StateDeck deck)
		{
			return from state in this.states
				   where state.StateDeck == deck
				   select state;
		}

		public IEnumerable<States.CoreState> GetZOrderStates(States.StateDeck deck)
		{
			return from state in this.zOrder
				   where state.StateDeck == deck
				   select state;
		}

		public IEnumerable<States.CoreState> GetHistoryStates(HistorySortMode sortMode)
		{
			IEnumerable<States.CoreState> history;

			switch (sortMode)
			{
				case HistorySortMode.NewestFirst:
					history = this.history;
					break;
				
				case HistorySortMode.NewestLast:
					history = CircularList<States.CoreState>.Reverse (this.history);
					break;

				default:
					throw new System.ArgumentException ();
			}

			return history;
		}

		public IEnumerable<States.CoreState> GetHistoryStates(States.StateDeck deck, HistorySortMode sortMode)
		{
			switch (sortMode)
			{
				case HistorySortMode.NewestFirst:
					return from state in this.history
						   where state.StateDeck == deck
						   select state;
				
				case HistorySortMode.NewestLast:
					return from state in CircularList<States.CoreState>.Reverse (this.history)
						   where state.StateDeck == deck
						   select state;

				default:
					throw new System.ArgumentException ();
			}
		}


		public XElement SaveStates(string xmlNodeName)
		{
			int tag = 0;

			foreach (States.CoreState state in this.states)
			{
				state.Tag = tag.ToString (System.Globalization.CultureInfo.InvariantCulture);
				tag++;
			}

			System.Text.StringBuilder historyTags = new System.Text.StringBuilder ();
			System.Text.StringBuilder zOrderTags  = new System.Text.StringBuilder ();

			foreach (var state in this.history)
			{
				if (historyTags.Length > 0)
				{
					historyTags.Append (" ");
				}
				historyTags.Append (state.Tag);
			}

			foreach (var state in this.zOrder)
			{
				if (zOrderTags.Length > 0)
				{
					zOrderTags.Append (" ");
				}
				zOrderTags.AppendFormat (state.Tag);
			}

			return new XElement (xmlNodeName,
				new XElement ("states",
					from state in this.states
					select state.Serialize (new XElement ("state",
						new XAttribute ("class", States.StateFactory.GetClassName (state))))),
				new XElement ("history", historyTags.ToString ()),
				new XElement ("z_order", zOrderTags.ToString ()));
		}

		public void RestoreStates(XElement xml)
		{
			var states = from state in xml.Descendants ("state")
						 select States.StateFactory.CreateState (this, state);

			Dictionary<string, States.CoreState> taggedStates = new Dictionary<string, States.CoreState> ();

			foreach (var state in states)
			{
				string tag = this.states.Count.ToString (System.Globalization.CultureInfo.InvariantCulture);
				taggedStates[tag] = state;
				this.states.Add (state);
				state.Tag = tag;
			}

			string history = xml.Element ("history").Value;
			string zOrder  = xml.Element ("z_order").Value;

			if (history.Length > 0)
			{
				foreach (string tag in history.Split (' '))
				{
					this.history.Add (taggedStates[tag]);
				}
			}
			if (zOrder.Length > 0)
			{
				foreach (string tag in zOrder.Split (' '))
				{
					this.zOrder.Add (taggedStates[tag]);
				}
			}

			foreach (var state in states)
			{
				state.NotifyDeserialized (taggedStates);
			}

			if (this.boxes.Count > 0)
			{
				foreach (var state in this.history)
				{
					Box box = this.boxes[state.BoxId];

					if (box.State == null)
					{
						box.State = state;
						box.State.Bind (box.Container);
					}
				}
			}

			this.OnStateStackChanged (new StateStackChangedEventArgs (StateStackChange.Load, null));
		}


		public int RegisterBox(Widget container)
		{
			int id = 1;

			//	If there are empty slots in the boxes dictionary, reuse them,
			//	otherwise allocate a slot with a new id.
			
			foreach (var item in this.boxes)
			{
				if (item.Value == null)
				{
					id = item.Key;
					break;
				}

				if (item.Key >= id)
				{
					id = item.Key + 1;
				}
			}

			this.boxes[id] = new Box (id)
			{
				Container = container
			};
			
			return id;
		}


		private void ActivateCurrentState()
		{
			if (this.history.Count > 0)
			{
				this.Attach (this.history[0]);
			}
		}

		private void Attach(States.CoreState state)
		{
			if (this.boxes.Count == 0)
			{
				return;
			}

			System.Diagnostics.Debug.Assert (state.BoxId != 0);
			System.Diagnostics.Debug.Assert (this.boxes.ContainsKey (state.BoxId));

			Box box = this.boxes[state.BoxId];

			//	If there was another active state bound to the specified box, then we
			//	first unbind it :
			
			if ((box.State != state) &&
				(box.State != null))
			{
				box.State.Unbind ();
				box.State = null;
			}

			box.State = state;
			box.State.Bind (box.Container);
		}

		private void Detach(States.CoreState state)
		{
			if (this.boxes.Count == 0)
			{
				return;
			}

			System.Diagnostics.Debug.Assert (state.BoxId != 0);
			System.Diagnostics.Debug.Assert (this.boxes.ContainsKey (state.BoxId));

			Box box = this.boxes[state.BoxId];

			if (box.State == state)
			{
				box.State.Unbind ();
				box.State = null;
			}
		}
		

		/// <summary>
		/// Determines whether the specified state is bound with a box.
		/// </summary>
		/// <param name="state">The state.</param>
		/// <returns>
		/// 	<c>true</c> if the specified state is bound with a box; otherwise, <c>false</c>.
		/// </returns>
		internal bool IsBound(States.CoreState state)
		{
			if (state.BoxId == 0)
			{
				return false;
			}

			System.Diagnostics.Debug.Assert (state.BoxId != 0);
			System.Diagnostics.Debug.Assert (this.boxes.ContainsKey (state.BoxId));

			return this.boxes[state.BoxId].State == state;
		}


		private void OnStateStackChanged(StateStackChangedEventArgs e)
		{
			if (this.StackChanged != null)
			{
				this.StackChanged (this, e);
			}
		}

		private void OnNavigated()
		{
			this.application.SaveApplicationState ();
		}


		#region Box Class

		/// <summary>
		/// The <c>Box</c> class associates a state with a UI container.
		/// </summary>
		private class Box
		{
			public Box(int id)
			{
				this.id = id;
			}

			public Widget Container
			{
				get;
				set;
			}

			public States.CoreState State
			{
				get;
				set;
			}

			public int Id
			{
				get
				{
					return this.Id;
				}
			}

			private readonly int id;
		}

		#endregion


		public event System.EventHandler<StateStackChangedEventArgs>	StackChanged;


		private readonly CoreApplication application;
		private readonly List<States.CoreState> states;
		private readonly List<States.CoreState> zOrder;
		private readonly CircularList<States.CoreState> history;
		private readonly Dictionary<int, Box> boxes;
	}
}
