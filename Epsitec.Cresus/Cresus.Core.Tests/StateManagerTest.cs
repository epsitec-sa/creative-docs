using System.Collections.Generic;

using NUnit.Framework;

namespace Epsitec.Cresus.Core
{
	[TestFixture]
	public class StateManagerTest
	{
		[TestFixtureSetUp]
		public void Initialize()
		{
			Epsitec.Common.Document.Engine.Initialize();
			Epsitec.Common.Widgets.Adorners.Factory.SetActive("LookMetal");
			Epsitec.Common.Widgets.Widget.Initialize ();
		}

		[Test]
		public void Check01SaveState()
		{
			using (System.IO.TextWriter writer = new System.IO.StreamWriter (this.path))
			{
				StateManager.Write (writer, new States.AbstractState[0]);
			}
		}

		[Test]
		public void Check02LoadState()
		{
			foreach (var item in StateManager.Read (this.path))
			{
				System.Diagnostics.Debug.WriteLine (item == null ? "<null>" : item.ToString ());
			}
		}


		private string path = System.IO.Path.GetTempFileName ();
	}
}
