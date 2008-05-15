//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Cresus.Core;

using System.Collections.Generic;
using System.Xml.Linq;

using NUnit.Framework;

[assembly: State (Type = typeof (StateManagerTest.DummyState), Name = "Dummy")]

namespace Epsitec.Cresus.Core
{
	[TestFixture]
	public class StateManagerTest
	{
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

			StateManager manager = new StateManager ();

			using (System.IO.TextWriter writer = new System.IO.StreamWriter (this.path))
			{
				manager.Write (writer,
					new DummyState[] {
						new DummyState (manager, "A"),
						new DummyState (manager, "B")
					});
			}
		}

		[Test]
		public void Check02LoadState()
		{
			StateManager manager = new StateManager ();

			List<States.AbstractState> states = new List<States.AbstractState> (manager.Read (this.path));

			Assert.AreEqual (2, states.Count);
			Assert.AreEqual (typeof (DummyState), states[0].GetType ());
			Assert.AreEqual (typeof (DummyState), states[1].GetType ());
			Assert.AreEqual ("A", states[0].ToString ());
			Assert.AreEqual ("B", states[1].ToString ());
			Assert.AreEqual (manager, states[0].StateManager);
			Assert.AreEqual (manager, states[1].StateManager);
		}


		[Test]
		public void Check03SaveDialogData()
		{
		}

		#region DummyState Class

		internal class DummyState : States.AbstractState
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

			public override System.Xml.Linq.XElement Serialize(System.Xml.Linq.XElement element)
			{
				element.Add (
					new XElement ("dummy",
						new XAttribute ("value", this.value)));

				return element;
			}

			public override States.AbstractState Deserialize(XElement element)
			{
				this.value = (string) element.Element ("dummy").Attribute ("value");
				return this;
			}

			public override string ToString()
			{
				return this.value;
			}

			private string value;
		}

		#endregion

		private readonly string path = System.IO.Path.Combine (System.IO.Path.GetTempPath (), "core.tests.statemanager.xml");
	}
}
