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
			this.hidden = new List<States.CoreState> ();
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
				if (this.history.Count > this.hidden.Count)
				{
					States.CoreState state;
					
					int i = 0;
					
					do
					{
						state = this.history[i++];
					}
					while (this.hidden.Contains (state));
					
					return state;
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
				this.hidden.Remove (state);

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

		public void Show(States.CoreState state)
		{
			if (state == null)
			{
				return;
			}

			if (this.hidden.Remove (state))
			{
				this.history.Remove (state);
				this.history.Rotate (1);
				this.history.Reverse ();
				this.history.Insert (0, state);

				this.zOrder.Remove (state);
				this.zOrder.Add (state);

				this.ActivateCurrentState ();
				this.OnStateStackChanged (new StateStackChangedEventArgs (StateStackChange.Visibility, state));
			}
			else
			{
				this.Push (state);
			}
		}

		public void Hide(States.CoreState state)
		{
			if (state == null)
			{
				return;
			}

			if (this.hidden.Contains (state))
			{
				throw new System.InvalidOperationException ();
			}

			this.hidden.Add (state);
			this.OnStateStackChanged (new StateStackChangedEventArgs (StateStackChange.Visibility, state));
		}

		public bool NavigateHistoryPrev()
		{
			if (this.history.Count > this.hidden.Count)
			{
				States.CoreState state;

				do
				{
					this.history.Rotate (1);
					state = this.history[0];
				}
				while (this.hidden.Contains (state));

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
			if (this.history.Count > this.hidden.Count)
			{
				States.CoreState state;

				do
				{
					this.history.Rotate (-1);
					state = this.history[0];
				}
				while (this.hidden.Contains (state));

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
				   where state.StateDeck == deck && this.hidden.Contains (state) == false
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

			return from state in history
				   where this.hidden.Contains (state) == false
				   select state;
		}

		public IEnumerable<States.CoreState> GetHistoryStates(States.StateDeck deck, HistorySortMode sortMode)
		{
			switch (sortMode)
			{
				case HistorySortMode.NewestFirst:
					return from state in this.history
						   where state.StateDeck == deck && this.hidden.Contains (state) == false
						   select state;
				
				case HistorySortMode.NewestLast:
					return from state in CircularList<States.CoreState>.Reverse (this.history)
						   where state.StateDeck == deck && this.hidden.Contains (state) == false
						   select state;

				default:
					throw new System.ArgumentException ();
			}
		}



		/// <summary>
		/// Saves all the states as an XML tree.
		/// </summary>
		/// <param name="xmlNodeName">Name of the root XML element.</param>
		/// <returns>The root XML element.</returns>
		public XElement SaveStates(string xmlNodeName)
		{
			StateSerializationContext context = new StateSerializationContext (this.states);

			System.Text.StringBuilder historyTags = new System.Text.StringBuilder ();
			System.Text.StringBuilder hiddenTags  = new System.Text.StringBuilder ();
			System.Text.StringBuilder zOrderTags  = new System.Text.StringBuilder ();

			foreach (var state in this.history)
			{
				if (historyTags.Length > 0)
				{
					historyTags.Append (" ");
				}
				historyTags.Append (context.GetTag (state));
			}

			foreach (var state in this.zOrder)
			{
				if (zOrderTags.Length > 0)
				{
					zOrderTags.Append (" ");
				}
				zOrderTags.AppendFormat (context.GetTag (state));
			}

			foreach (var state in this.hidden)
			{
				if (hiddenTags.Length > 0)
				{
					hiddenTags.Append (" ");
				}
				hiddenTags.AppendFormat (context.GetTag (state));
			}

			var xmlStateElements =
				from state in this.states
				select state.Serialize (new XElement (Strings.XmlState, new XAttribute (Strings.XmlClass, States.StateFactory.GetClassName (state))), context);

			return new XElement (xmlNodeName,
				new XElement (Strings.XmlStates,  xmlStateElements),
				new XElement (Strings.XmlHistory, historyTags.ToString ()),
				new XElement (Strings.XmlZOrder,  zOrderTags.ToString ()),
				new XElement (Strings.XmlHidden,  hiddenTags.ToString ()));
		}

		/// <summary>
		/// Restores all the states from the XML tree.
		/// </summary>
		/// <param name="xml">The root XML element.</param>
		public void RestoreStates(XElement xml)
		{
			List<States.CoreState> states = new	List<States.CoreState> (
				from state in xml.Descendants (Strings.XmlState)
				select States.StateFactory.CreateState (this, state, (string) state.Attribute (Strings.XmlClass)));

			System.Diagnostics.Debug.Assert (this.states.Count == 0);

			this.states.AddRange (states);
			
			StateSerializationContext context = new StateSerializationContext (this.states);

			string history = xml.Element (Strings.XmlHistory).Value;
			string zOrder  = xml.Element (Strings.XmlZOrder).Value;
			string hidden  = xml.Element (Strings.XmlHidden).Value;

			if (string.IsNullOrEmpty (history) == false)
			{
				foreach (string tag in history.Split (' '))
				{
					this.history.Add (context.GetState (tag));
				}
			}
			
			if (string.IsNullOrEmpty (zOrder) == false)
			{
				foreach (string tag in zOrder.Split (' '))
				{
					this.zOrder.Add (context.GetState (tag));
				}
			}
			
			if (string.IsNullOrEmpty (hidden) == false)
			{
				foreach (string tag in hidden.Split (' '))
				{
					this.hidden.Add (context.GetState (tag));
				}
			}

			//	Now that the states have been deserialized and that the history, z-order and
			//	hidden lists are up to date, let the states handle any post-deserialization
			//	logic :

			foreach (var state in states)
			{
				state.NotifyDeserialized (context);
			}

			this.ActivateCurrentState ();
			this.OnStateStackChanged (new StateStackChangedEventArgs (StateStackChange.Load, null));
		}


		/// <summary>
		/// Registers a box for the specified container, assigning it a numeric
		/// identifier, which uniquely identifies it.
		/// </summary>
		/// <param name="container">The container.</param>
		/// <returns>Identifier for the box.</returns>
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


		/// <summary>
		/// Activates the state found on the top of the history stack.
		/// </summary>
		private void ActivateCurrentState()
		{
			if ((this.boxes.Count > 0) &&
				(this.history.Count > 0))
			{
				this.Attach (this.history[0]);

				//	If there are any free boxes, attach the next available states to them.
				
				foreach (var state in this.history)
				{
					Box box = this.boxes[state.BoxId];

					if (box.State == null)
					{
						this.Attach (state);
					}
				}
			}
		}

		/// <summary>
		/// Attaches the specified state to the user interface.
		/// </summary>
		/// <param name="state">The state.</param>
		private void Attach(States.CoreState state)
		{
			if ((this.boxes.Count == 0) ||
				(state == null))
			{
				return;
			}

			System.Diagnostics.Debug.Assert (state.BoxId != 0);
			System.Diagnostics.Debug.Assert (this.boxes.ContainsKey (state.BoxId));

			Box box = this.boxes[state.BoxId];

			if (box.State == state)
			{
				return;
			}
			
			//	If there was another active state bound to the specified box, then we
			//	first unbind it.

			this.Detach (box.State);

			System.Diagnostics.Debug.Assert (box.State == null);

			box.State = state;
			box.State.Bind (box.Container);
		}

		/// <summary>
		/// Detaches the specified state from the user interface.
		/// </summary>
		/// <param name="state">The state.</param>
		private void Detach(States.CoreState state)
		{
			if ((this.boxes.Count == 0) ||
				(state == null))
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
		/// Determines whether the specified state is bound with a box, i.e.
		/// whether it has a visible user interface.
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
			if (this.application != null)
			{
				this.application.AsyncSaveApplicationState ();
			}
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

			public Widget						Container
			{
				get;
				set;
			}

			public States.CoreState				State
			{
				get;
				set;
			}

			public int							Id
			{
				get
				{
					return this.Id;
				}
			}

			private readonly int id;
		}

		#endregion

		#region Private Strings

		/// <summary>
		/// The <c>Strings</c> class defines the constants used for XML serialization
		/// of the states.
		/// </summary>
		private static class Strings
		{
			public static readonly string XmlStates = "states";
			public static readonly string XmlState = "state";
			public static readonly string XmlHistory = "history";
			public static readonly string XmlZOrder = "z_order";
			public static readonly string XmlHidden = "hidden";
			public static readonly string XmlClass = "class";
		}

		#endregion


		public event System.EventHandler<StateStackChangedEventArgs>	StackChanged;


		private readonly CoreApplication application;
		private readonly List<States.CoreState> states;
		private readonly List<States.CoreState> zOrder;
		private readonly List<States.CoreState> hidden;
		private readonly CircularList<States.CoreState> history;
		private readonly Dictionary<int, Box> boxes;
	}
}
