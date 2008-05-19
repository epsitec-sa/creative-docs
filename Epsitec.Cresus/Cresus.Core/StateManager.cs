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
			System.Diagnostics.Debug.Assert (state != null);
			System.Diagnostics.Debug.Assert (state.StateManager == this);

			if (this.states.Contains (state))
			{
				this.history.Remove (state);
				this.history.Rotate (1);
				this.history.Reverse ();
				this.history.Insert (0, state);

				this.zOrder.Remove (state);
				this.zOrder.Add (state);

				this.OnStateStackChanged (new StateStackChangedEventArgs (StateStackChange.Promotion, state));
			}
			else
			{
				this.states.Add (state);
				this.history.Insert (0, state);
				this.zOrder.Add (state);
				this.Attach (state);

				this.OnStateStackChanged (new StateStackChangedEventArgs (StateStackChange.Push, state));
			}
		}

		public bool Pop(States.CoreState state)
		{
			System.Diagnostics.Debug.Assert (state != null);
			System.Diagnostics.Debug.Assert (state.StateManager == this);

			if (this.states.Remove (state))
			{
				this.history.Remove (state);
				this.zOrder.Remove (state);

				this.Detach (state);
				this.OnStateStackChanged (new StateStackChangedEventArgs (StateStackChange.Pop, state));
				
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

				this.Attach (state);
				this.zOrder.Remove (state);
				this.zOrder.Add (state);

				this.OnStateStackChanged (new StateStackChangedEventArgs (StateStackChange.Navigation, state));

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

				this.Attach (state);
				this.zOrder.Remove (state);
				this.zOrder.Add (state);

				this.OnStateStackChanged (new StateStackChangedEventArgs (StateStackChange.Navigation, state));

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

		public IEnumerable<States.CoreState> GetHistoryStates(ListSortDirection direction)
		{
			switch (direction)
			{
				case ListSortDirection.Ascending:
					return this.history;
				case ListSortDirection.Descending:
					return CircularList<States.CoreState>.Reverse (this.history);
			}

			throw new System.ArgumentException ();
		}

		public IEnumerable<States.CoreState> GetHistoryStates(States.StateDeck deck, ListSortDirection direction)
		{
			switch (direction)
			{
				case ListSortDirection.Ascending:
					return from state in this.history
						   where state.StateDeck == deck
						   select state;
				case ListSortDirection.Descending:
					return from state in CircularList<States.CoreState>.Reverse (this.history)
						   where state.StateDeck == deck
						   select state;
			}

			throw new System.ArgumentException ();
		}


		public void ReloadStates(string path)
		{
			XDocument doc = XDocument.Load (path);
			XElement store = doc.Element ("store");

			var states = from state in doc.Descendants ("state")
						 select States.StateFactory.CreateState (this, state);

			Dictionary<string, States.CoreState> taggedStates = new Dictionary<string, States.CoreState> ();
			
			foreach (var state in states)
			{
				string tag = this.states.Count.ToString (System.Globalization.CultureInfo.InvariantCulture);
				taggedStates[tag] = state;
				this.states.Add (state);
			}
			foreach (string tag in store.Element ("history").Value.Split (' '))
			{
				this.history.Add (taggedStates[tag]);
			}
			foreach (string tag in store.Element ("z_order").Value.Split (' '))
			{
				this.zOrder.Add (taggedStates[tag]);
			}

			for (int i = this.states.Count-1; i >= 0; i--)
			{
				States.CoreState state = this.states[i];
				Box box = this.boxes[state.BoxId];

				if (box.State == null)
				{
					box.State = state;
					box.State.Bind (box.Container);
				}
			}

			this.OnStateStackChanged (new StateStackChangedEventArgs (StateStackChange.Load, null));
		}

		public IEnumerable<States.CoreState> ReadStates(string path)
		{
			using (System.IO.StreamReader reader = System.IO.File.OpenText (path))
			{
				foreach (var item in this.ReadStates (reader))
				{
					yield return item;
				}
			}
		}

		public IEnumerable<States.CoreState> ReadStates(System.IO.TextReader reader)
		{
			XDocument doc = XDocument.Load (reader);

			return from state in doc.Descendants ("state")
				   select States.StateFactory.CreateState (this, state);
		}

		
		public void WriteStates(string path)
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
			
			IEnumerable<XElement> additionalElements = new XElement[]
			{
				new XElement ("history", historyTags.ToString ()),
				new XElement ("z_order", zOrderTags.ToString ())
			};

			using (System.IO.TextWriter writer = new System.IO.StreamWriter (path))
			{
				this.WriteStates (writer, this.states, additionalElements);
			}
		}

		public void WriteStates(string path, IEnumerable<States.CoreState> states)
		{
			using (System.IO.TextWriter writer = new System.IO.StreamWriter (path))
			{
				this.WriteStates (writer, states, new XElement[0]);
			}
		}
		
		public void WriteStates(System.IO.TextWriter writer, IEnumerable<States.CoreState> states, IEnumerable<XElement> additionalElements)
		{
			System.DateTime now = System.DateTime.Now.ToUniversalTime ();
			string timeStamp = string.Concat (now.ToShortDateString (), " ", now.ToShortTimeString (), " UTC");

			XDocument doc = new XDocument (
				new XDeclaration ("1.0", "utf-8", "yes"),
				new XComment ("Saved on " + timeStamp),
				new XElement ("store",
					new XAttribute ("version", "1.0"),
					new XAttribute ("content", "Epsitec.Cresus.Core.States"),
					new XElement ("states",
						from state in states
						select state.Serialize (new XElement ("state",
							new XAttribute ("class", States.StateFactory.GetClassName (state))))),
					additionalElements));

			doc.Save (writer);
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


		private void Attach(States.CoreState state)
		{
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
