//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Cresus.Core;

using System.Collections.Generic;
using System.Xml.Linq;

using NUnit.Framework;

//- [assembly: State (Type = typeof (StateManagerTest.DummyState), Name = "Dummy")]

namespace Epsitec.Cresus.Core
{
	[TestFixture]
	public class StateManagerTest
	{

#if false
		[TestFixtureSetUp]
		public void Initialize()
		{
			Epsitec.Cresus.Core.States.StateFactory.Setup ();
#if false
			Epsitec.Common.Document.Engine.Initialize();
			Epsitec.Common.Widgets.Adorners.Factory.SetActive("LookMetal");
			Epsitec.Common.Widgets.Widget.Initialize ();
#endif
		}

		[Test]
		public void Check01SaveState()
		{
			System.Console.Out.WriteLine ("Using {0} for the serialization", this.path);

			StateManager manager = new StateManager (null);

			manager.Push (new DummyState (manager, "A"));
			manager.Push (new DummyState (manager, "B"));

			XDocument doc = new XDocument (
				new XDeclaration ("1.0", "utf-8", "yes"),
				new XElement ("store",
					manager.SaveStates ("stateManager")));

			doc.Save (this.path);
		}

		[Test]
		public void Check02LoadState()
		{
			StateManager manager = new StateManager (null);

			XDocument doc = XDocument.Load (path);
			XElement store = doc.Element ("store");

			manager.RestoreStates (store.Element ("stateManager"));

			List<States.CoreState> states = new List<States.CoreState> (manager.GetAllStates ());

			Assert.AreEqual (2, states.Count);
			Assert.AreEqual (typeof (DummyState), states[0].GetType ());
			Assert.AreEqual (typeof (DummyState), states[1].GetType ());
			Assert.AreEqual ("A", states[0].ToString ());
			Assert.AreEqual ("B", states[1].ToString ());
			Assert.AreEqual (manager, states[0].StateManager);
			Assert.AreEqual (manager, states[1].StateManager);
		}

		[Test]
		public void Check03SaveEmptyState()
		{
			System.Console.Out.WriteLine ("Using {0} for the serialization", this.path);

			StateManager manager = new StateManager (null);

			XDocument doc = new XDocument (
				new XDeclaration ("1.0", "utf-8", "yes"),
				new XElement ("store",
					manager.SaveStates ("stateManager")));

			doc.Save (this.path);
		}

		[Test]
		public void Check04LoadEmptyState()
		{
			StateManager manager = new StateManager (null);

			XDocument doc = XDocument.Load (path);
			XElement store = doc.Element ("store");

			manager.RestoreStates (store.Element ("stateManager"));

			List<States.CoreState> states = new List<States.CoreState> (manager.GetAllStates ());

			Assert.AreEqual (0, states.Count);
		}



		#region DummyState Class

		internal class DummyState : States.CoreState
		{
			public DummyState(StateManager manager)
				: base (manager)
			{
			}

			public DummyState(StateManager manager, string value)
				: base (manager)
			{
				this.value = value;
			}

			public override System.Xml.Linq.XElement Serialize(System.Xml.Linq.XElement element, StateSerializationContext context)
			{
				element.Add (
					new XElement ("dummy",
						new XAttribute ("value", this.value)));

				return element;
			}

			public override States.CoreState Deserialize(XElement element)
			{
				this.value = (string) element.Element ("dummy").Attribute ("value");
				return this;
			}

			public override string ToString()
			{
				return this.value;
			}

			protected override void AttachState(Epsitec.Common.Widgets.Widget container)
			{
			}

			protected override void DetachState()
			{
			}

			protected override Epsitec.Common.Widgets.AbstractGroup CreateUserInterface()
			{
				throw new System.NotImplementedException ();
			}

			protected override void EnableWorkspace()
			{
				throw new System.NotImplementedException ();
			}

			protected override void DisableWorkspace()
			{
				throw new System.NotImplementedException ();
			}

			protected override void StoreWorkspace(XElement workspaceElement, StateSerializationContext context)
			{
				throw new System.NotImplementedException ();
			}

			protected override void RestoreWorkspace(XElement workspaceElement)
			{
				throw new System.NotImplementedException ();
			}

			private string value;
		}

		#endregion

		private readonly string path = System.IO.Path.Combine (System.IO.Path.GetTempPath (), "core.tests.statemanager.xml");
#endif
	}
}
