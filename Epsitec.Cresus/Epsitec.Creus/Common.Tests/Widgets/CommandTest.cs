using NUnit.Framework;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types.Serialization;
using Epsitec.Common.Widgets.Collections;

[assembly: Epsitec.Common.Types.DependencyClass (typeof (Epsitec.Common.Tests.Widgets.CommandTest.MyCommandTest))]

namespace Epsitec.Common.Tests.Widgets
{
	[TestFixture] public class CommandTest
	{
		[SetUp]
		public void Initialize()
		{
			Epsitec.Common.Document.Engine.Initialize ();
			Epsitec.Common.Widgets.Adorners.Factory.SetActive ("LookMetal");

			Epsitec.Common.Support.Resources.DefaultManager.ActiveCulture = Epsitec.Common.Support.Resources.FindSpecificCultureInfo ("fr");
		}

		[Test]
		public void Check0TemporaryCommand()
		{
			Caption caption1 = Epsitec.Common.Support.Resources.DefaultManager.GetCaption (Druid.Parse ("[0005]"));
			Caption caption2 = Epsitec.Common.Support.Resources.DefaultManager.GetCaption (Druid.Parse ("[0004]"));
			Caption caption3 = Epsitec.Common.Support.Resources.DefaultManager.GetCaption (Druid.Parse ("[0004]"));

			Command command = Command.Get (Druid.Parse ("[0005]"));

			Command tempCmd1 = Command.CreateTemporary (caption1, Epsitec.Common.Support.Resources.DefaultManager);
			Command tempCmd2 = Command.CreateTemporary (caption2, Epsitec.Common.Support.Resources.DefaultManager);

			Assert.AreEqual (caption1, command.Caption);
			Assert.AreEqual (caption2, caption3);
			Assert.AreNotEqual (command, tempCmd1);

			Assert.IsNotNull (Command.Find ("[0005]"));
			//Assert.IsNull (Command.Find ("[0004]"));
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
			Command command = Command.Get ("Test.CheckCommandSerialization1");
			command.Shortcuts.Add (new Shortcut ('O', ModifierKeys.Alt));
			command.Shortcuts.Add (new Shortcut (KeyCode.FuncF10 | KeyCode.ModifierShift));
			command.Shortcuts.Add (new Shortcut ('O', ModifierKeys.Alt));

			Assert.AreEqual (2, command.Shortcuts.Count);

			MyCommandTest t1 = new MyCommandTest ();
			MyCommandTest t2;

			t1.Command = command;

			string xml = SimpleSerialization.SerializeToString (t1);

			System.Console.Out.WriteLine (xml);

			t2 = SimpleSerialization.DeserializeFromString (xml) as MyCommandTest;

			Command restored = t2.Command;

			Assert.AreEqual (restored.Shortcuts.Count, 2);
			Assert.AreEqual (command.Shortcuts[0], restored.Shortcuts[0]);
			Assert.AreEqual (command.Shortcuts[1], restored.Shortcuts[1]);

			Assert.IsTrue (object.ReferenceEquals (command, restored));
		}

		[Test]
		public void CheckCommandSerialization2()
		{
			ShortcutCollection shortcuts;
			
			Command command = Command.Get ("Test.CheckCommandSerialization2");
			command.Shortcuts.Add (new Shortcut ('O', ModifierKeys.Alt));
			command.Shortcuts.Add (new Shortcut (KeyCode.FuncF10 | KeyCode.ModifierShift));

			command.ManuallyDefineCommand ("Description", "icon", "testgroup", true);
			
			shortcuts = Shortcut.GetShortcuts (command.Caption);
			
			Assert.AreEqual ("Test.CheckCommandSerialization2", command.Caption.Name);
			Assert.AreEqual ("Description", command.Caption.Description);
			Assert.AreEqual ("icon", command.Caption.Icon);
			Assert.AreEqual ("testgroup", command.Group);

			Assert.IsNotNull (shortcuts);
			
			Assert.AreEqual (shortcuts.Count, 2);
			Assert.AreEqual (command.Shortcuts[0], shortcuts[0]);
			Assert.AreEqual (command.Shortcuts[1], shortcuts[1]);

			string xml = SimpleSerialization.SerializeToString (command.Caption);

			System.Console.Out.WriteLine (xml);
			Assert.AreEqual (Caption.CompressXml (xml), command.Caption.SerializeToString ());

			Caption caption1 = SimpleSerialization.DeserializeFromString (xml) as Caption;
			Caption caption2 = new Caption ();

			caption2.DeserializeFromString (xml);

			shortcuts = Shortcut.GetShortcuts (caption1);

			Assert.AreEqual ("Test.CheckCommandSerialization2", caption1.Name);
			Assert.AreEqual ("Description", caption1.Description);
			Assert.AreEqual ("icon", caption1.Icon);
			Assert.AreEqual ("testgroup", Command.GetGroup (caption1));

			Assert.IsNotNull (shortcuts);

			Assert.AreEqual (shortcuts.Count, 2);
			Assert.AreEqual (command.Shortcuts[0], shortcuts[0]);
			Assert.AreEqual (command.Shortcuts[1], shortcuts[1]);

			
			shortcuts = Shortcut.GetShortcuts (caption2);

			Assert.AreEqual ("Test.CheckCommandSerialization2", caption2.Name);
			Assert.AreEqual ("Description", caption2.Description);
			Assert.AreEqual ("icon", caption2.Icon);
			Assert.AreEqual ("testgroup", Command.GetGroup (caption2));

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