using NUnit.Framework;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;

namespace Epsitec.Common.Widgets
{
	[TestFixture] public class CommandTest
	{
		[SetUp]
		public void Initialise()
		{
			Epsitec.Common.Document.Engine.Initialise ();
			Epsitec.Common.Widgets.Adorners.Factory.SetActive ("LookMetal");

			Resources.DefaultManager.ActiveCulture = Resources.FindSpecificCultureInfo ("fr");
		}
		
		[Test]
		public void CheckApplicationCommands()
		{
			Assert.AreEqual ("Couper", ApplicationCommands.Cut.Description);
			Assert.AreEqual ("Copier", ApplicationCommands.Copy.Description);
			Assert.AreEqual ("Coller", ApplicationCommands.Paste.Description);
			Assert.AreEqual ("Supprimer", ApplicationCommands.Delete.Description);
			Assert.AreEqual ("Sélectionner tout", ApplicationCommands.SelectAll.Description);
		}

		[Test]
		public void CheckCommandFromDruid()
		{
			Command command = Command.Get (Druid.Parse ("[0005]"));

			Assert.IsNotNull (command);
			Assert.AreEqual ("[0005]", command.Name);

			Assert.AreEqual ("Sélectionner tout", command.Description);
			Assert.AreEqual (ApplicationCommands.SelectAll, command);
		}

		[Test]
		public void CheckCommandShortcut()
		{
			Command command = new Command ("Test");
			command.Shortcuts.Add (new Shortcut ('O', ModifierKeys.Alt));
			command.Shortcuts.Add (new Shortcut (KeyCode.FuncF10 | KeyCode.ModifierShift));
			command.Shortcuts.Add (new Shortcut ('O', ModifierKeys.Alt));

			Assert.AreEqual (2, command.Shortcuts.Count);

			string xml = Types.Serialization.SimpleSerialization.SerializeToString (command);

			System.Console.Out.WriteLine (xml);

			Command restored = Types.Serialization.SimpleSerialization.DeserializeFromString (xml) as Command;

			Assert.AreEqual (restored.Shortcuts.Count, 2);
			Assert.AreEqual (command.Shortcuts[0], restored.Shortcuts[0]);
			Assert.AreEqual (command.Shortcuts[1], restored.Shortcuts[1]);
		}
	}
}