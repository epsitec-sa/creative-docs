//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

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


		public static IEnumerable<States.AbstractState> Read(string path)
		{
			using (System.IO.StreamReader reader = System.IO.File.OpenText (path))
			{
				foreach (var item in StateManager.Read (reader))
				{
					yield return item;
				}
			}
		}

		public static IEnumerable<States.AbstractState> Read(System.IO.TextReader reader)
		{
			XDocument doc = XDocument.Load (reader);

			return from state in doc.Descendants ("state")
				   select States.StateFactory.CreateState (state);
		}

		public static void Write(System.IO.TextWriter writer, IEnumerable<States.AbstractState> states)
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
