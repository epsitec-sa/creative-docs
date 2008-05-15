//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
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

			using (System.IO.TextWriter writer = new System.IO.StreamWriter (this.path))
			{
				StateManager.Write (writer,
					new DummyState[] {
						new DummyState ("A"),
						new DummyState ("B")
					});
			}
		}

		[Test]
		public void Check02LoadState()
		{
			foreach (var item in StateManager.Read (this.path))
			{
				System.Console.Out.WriteLine ("Element: {0}", item == null ? "<null>" : item.ToString ());
			}
		}


		internal class DummyState : States.AbstractState
		{
			public DummyState()
			{
			}

			public DummyState(string value)
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

		private string path = System.IO.Path.Combine (System.IO.Path.GetTempPath (), "core.tests.statemanager.xml");
	}
}
