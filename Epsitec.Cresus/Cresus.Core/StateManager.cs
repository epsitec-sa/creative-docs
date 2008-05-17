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
		public StateManager()
		{
			this.states = new List<States.CoreState> ();
			this.history = new CircularList<States.CoreState> ();
			this.boxes = new Dictionary<int, Box> ();
		}


		public CoreApplication					Application
		{
			get
			{
				return this.application;
			}
			internal set
			{
				this.DefineCoreApplication (value);
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
			if (this.states.Contains (state))
			{
				this.history.Remove (state);
				this.history.Rotate (1);
				this.history.Reverse ();
				this.history.Insert (0, state);

				this.OnStateStackChanged (new StateStackChangedEventArgs (StateStackChange.Promotion, state));
			}
			else
			{
				this.states.Add (state);
				this.history.Insert (0, state);

				this.OnStateStackChanged (new StateStackChangedEventArgs (StateStackChange.Push, state));
			}
		}

		public bool Pop(States.CoreState state)
		{
			if (this.states.Remove (state))
			{
				this.history.Remove (state);

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

				this.OnStateStackChanged (new StateStackChangedEventArgs (StateStackChange.Navigation, this.history[0]));

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

				this.OnStateStackChanged (new StateStackChangedEventArgs (StateStackChange.Navigation, this.history[0]));

				return true;
			}
			else
			{
				return false;
			}
		}

		public IEnumerable<States.CoreState> GetAllStates(States.StateDeck deck)
		{
			return from state in this.states
				   where state.StateDeck == deck
				   select state;
		}

		public IEnumerable<States.CoreState> GetDeckStates(States.StateDeck deck)
		{
			return from state in CircularList<States.CoreState>.Reverse (this.history)
				   where state.StateDeck == deck
				   select state;
		}

		public IEnumerable<States.CoreState> Read(string path)
		{
			using (System.IO.StreamReader reader = System.IO.File.OpenText (path))
			{
				foreach (var item in this.Read (reader))
				{
					yield return item;
				}
			}
		}

		public IEnumerable<States.CoreState> Read(System.IO.TextReader reader)
		{
			XDocument doc = XDocument.Load (reader);

			return from state in doc.Descendants ("state")
				   select States.StateFactory.CreateState (this, state);
		}

		public void Write(string path, IEnumerable<States.CoreState> states)
		{
			using (System.IO.TextWriter writer = new System.IO.StreamWriter (path))
			{
				this.Write (writer, states);
			}
		}
		
		public void Write(System.IO.TextWriter writer, IEnumerable<States.CoreState> states)
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
							new XAttribute ("class", States.StateFactory.GetClassName (state)))))));

			doc.Save (writer);
		}


		internal int DefineBox(Widget container)
		{
			int id = 1;

			foreach (var item in this.boxes)
			{
				//	If there are empty slots in the boxes dictionary, reuse them.

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

			this.boxes[id] = new Box ()
			{
				Container = container
			};
			
			return id;
		}

		internal Widget Attach(States.CoreState state)
		{
			System.Diagnostics.Debug.Assert (state.BoxId != 0);
			System.Diagnostics.Debug.Assert (this.boxes.ContainsKey (state.BoxId));

			Box box = this.boxes[state.BoxId];

			if ((box.State != state) &&
				(box.State != null))
			{
				box.State.SoftDetach ();
			}

			box.State = state;

			return box.Container;
		}

		internal void Detach(States.CoreState state)
		{
			System.Diagnostics.Debug.Assert (state.BoxId != 0);
			
			Box box = this.boxes[state.BoxId];
			
			if (box.State == state)
			{
				box.State = null;
			}
		}
		
		private void DefineCoreApplication(CoreApplication coreApplication)
		{
			System.Diagnostics.Debug.Assert (coreApplication != null);
			System.Diagnostics.Debug.Assert (this.application == null);

			this.application = coreApplication;
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
			for (int i = 0; i < this.history.Count; i++)
			{
				this.history[i].ZOrder = i;
			}
		}


		private class Box
		{
			public Box()
			{
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
		}

		
		public event System.EventHandler<StateStackChangedEventArgs>	StackChanged;

		
		private readonly List<States.CoreState> states;
		private readonly CircularList<States.CoreState> history;
		private readonly Dictionary<int, Box> boxes;
		private CoreApplication application;
	}
}
