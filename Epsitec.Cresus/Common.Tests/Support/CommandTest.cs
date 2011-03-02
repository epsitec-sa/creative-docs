using NUnit.Framework;

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Types;

namespace Epsitec.Common.Tests.Support
{
	[TestFixture]
	public class CommandTest
	{
		[SetUp]
		public void Initialize()
		{
			Epsitec.Common.Widgets.Widget.Initialize ();
		}
		
		[Test]
		public void CheckCommandContext()
		{
			Command command = Command.Get ("TestSave");

			command.ManuallyDefineCommand ("Enregistre le document ouvert", "save.icon", null, false);
			command.Shortcuts.Add (new Shortcut ('S', ModifierKeys.Control));

			Assert.AreEqual ("TestSave", command.CommandId);
			Assert.IsTrue (command.IsReadWrite);
			Assert.IsFalse (command.IsReadOnly);

			CommandContext contextA = new CommandContext ();
			CommandContext contextB = new CommandContext ();

			Visual v1 = new Visual ();
			Visual v2 = new Visual ();
			Visual v3 = new Visual ();

			v1.Children.Add (v2);
			v2.Children.Add (v3);

			CommandContext.SetContext (v1, contextA);
			CommandContext.SetContext (v2, contextB);

			CommandContextChain chain;

			chain = CommandContextChain.BuildChain (v1);

			Assert.AreEqual (1, Types.Collection.Count (chain.Contexts));
			Assert.AreEqual (contextA, Types.Collection.ToArray (chain.Contexts)[0]);

			chain = CommandContextChain.BuildChain (v2);

			Assert.AreEqual (2, Types.Collection.Count (chain.Contexts));
			Assert.AreEqual (contextB, Types.Collection.ToArray (chain.Contexts)[0]);
			Assert.AreEqual (contextA, Types.Collection.ToArray (chain.Contexts)[1]);

			chain = CommandContextChain.BuildChain (v3);

			Assert.AreEqual (2, Types.Collection.Count (chain.Contexts));
			Assert.AreEqual (contextB, Types.Collection.ToArray (chain.Contexts)[0]);
			Assert.AreEqual (contextA, Types.Collection.ToArray (chain.Contexts)[1]);

			v3.CommandObject = command;

			Assert.AreEqual ("TestSave", v3.CommandName);

			CommandCache.Instance.Synchronize ();

			Assert.IsTrue (v3.Enable);

			CommandState stateA;
			CommandState stateB;

			stateA = contextA.GetCommandState (command);

			Assert.IsNotNull (stateA);
			Assert.AreEqual ("SimpleState", stateA.GetType ().Name);

			stateA.Enable = false;

			Assert.IsTrue (v3.Enable);
			CommandCache.Instance.Synchronize ();
			Assert.IsFalse (v3.Enable);
			Assert.AreEqual (stateA, chain.GetCommandState (command.CommandId));

			//	En créant un stateB dans contextB, on va se trouver plus près de v3
			//	dans la chaîne des contextes des commandes. Du coup, c'est l'état de
			//	stateB (enabled) qui l'emportera sur stateA (disabled) :

			stateB = contextB.GetCommandState (command);

			Assert.AreNotEqual (stateA, stateB);

			Assert.IsFalse (v3.Enable);
			CommandCache.Instance.Synchronize ();
			Assert.IsTrue (v3.Enable);
			Assert.AreEqual (stateB, chain.GetCommandState (command.CommandId));

			contextB.ClearCommandState (command);

			Assert.IsTrue (v3.Enable);
			CommandCache.Instance.Synchronize ();
			Assert.IsFalse (v3.Enable);
			Assert.AreEqual (stateA, chain.GetCommandState (command.CommandId));
		}
	}
}
