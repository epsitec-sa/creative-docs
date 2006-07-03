using NUnit.Framework;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Types;

[assembly: Epsitec.Common.Types.DependencyClass (typeof (Epsitec.Common.Widgets.CommandTest.MyCommandTest))]

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
			Assert.AreEqual ("[0005]", command.CommandId);

			Assert.AreEqual ("Sélectionner tout", command.Description);
			Assert.AreEqual (ApplicationCommands.SelectAll, command);
		}

		[Test]
		public void CheckCommandSerialization1()
		{
			Command command = new Command ("Test.CheckCommandSerialization1");
			command.Shortcuts.Add (new Shortcut ('O', ModifierKeys.Alt));
			command.Shortcuts.Add (new Shortcut (KeyCode.FuncF10 | KeyCode.ModifierShift));
			command.Shortcuts.Add (new Shortcut ('O', ModifierKeys.Alt));

			Assert.AreEqual (2, command.Shortcuts.Count);

			MyCommandTest t1 = new MyCommandTest ();
			MyCommandTest t2;

			t1.Command = command;

			string xml = Types.Serialization.SimpleSerialization.SerializeToString (t1);

			System.Console.Out.WriteLine (xml);

			t2 = Types.Serialization.SimpleSerialization.DeserializeFromString (xml) as MyCommandTest;

			Command restored = t2.Command;

			Assert.AreEqual (restored.Shortcuts.Count, 2);
			Assert.AreEqual (command.Shortcuts[0], restored.Shortcuts[0]);
			Assert.AreEqual (command.Shortcuts[1], restored.Shortcuts[1]);

			Assert.IsTrue (object.ReferenceEquals (command, restored));
		}

		[Test]
		public void CheckCommandSerialization2()
		{
			Collections.ShortcutCollection shortcuts;
			
			Command command = new Command ("Test.CheckCommandSerialization2");
			command.Shortcuts.Add (new Shortcut ('O', ModifierKeys.Alt));
			command.Shortcuts.Add (new Shortcut (KeyCode.FuncF10 | KeyCode.ModifierShift));

			command.ManuallyDefineCommand ("Description", "icon", true);
			
			shortcuts = Shortcut.GetShortcuts (command.Caption);
			
			Assert.AreEqual ("Test.CheckCommandSerialization2", command.Caption.Name);
			Assert.AreEqual ("Description", command.Caption.Description);
			Assert.AreEqual ("icon", command.Caption.Icon);

			Assert.IsNotNull (shortcuts);
			
			Assert.AreEqual (shortcuts.Count, 2);
			Assert.AreEqual (command.Shortcuts[0], shortcuts[0]);
			Assert.AreEqual (command.Shortcuts[1], shortcuts[1]);

			string xml = Types.Serialization.SimpleSerialization.SerializeToString (command.Caption);

			System.Console.Out.WriteLine (xml);
			System.Console.Out.WriteLine (command.Caption.SerializeToString ());

			Caption caption = Types.Serialization.SimpleSerialization.DeserializeFromString (xml) as Caption;

			shortcuts = Shortcut.GetShortcuts (caption);

			Assert.AreEqual ("Test.CheckCommandSerialization2", caption.Name);
			Assert.AreEqual ("Description", caption.Description);
			Assert.AreEqual ("icon", caption.Icon);
			
			Assert.IsNotNull (shortcuts);
			
			Assert.AreEqual (shortcuts.Count, 2);
			Assert.AreEqual (command.Shortcuts[0], shortcuts[0]);
			Assert.AreEqual (command.Shortcuts[1], shortcuts[1]);
		}


		public class MyCommandTest : DependencyObject
		{
			public Command Command
			{
				get
				{
					return (Command) this.GetValue (MyCommandTest.CommandProperty);
				}
				set
				{
					this.SetValue (MyCommandTest.CommandProperty, value);
				}
			}

			public static readonly DependencyProperty CommandProperty = DependencyProperty.Register ("Command", typeof (Command), typeof (MyCommandTest));
		}
	}
}