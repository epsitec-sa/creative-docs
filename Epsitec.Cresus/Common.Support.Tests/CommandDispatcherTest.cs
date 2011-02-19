using NUnit.Framework;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.Support
{
#if false
	[TestFixture]
	public class CommandDispatcherTest
	{
		[Test]
		public void CheckRegisterController()
		{
			CommandDispatcher dispatcher = new CommandDispatcher ();
			BaseTestController controller = new BaseTestController ();
			DerivedTestController derived    = new DerivedTestController ();

			dispatcher.RegisterController (controller);
			dispatcher.RegisterController (derived);
		}

		[Test]
		[ExpectedException (typeof (System.FormatException))]
		public void CheckRegisterControllerEx1()
		{
			CommandDispatcher dispatcher = new CommandDispatcher ();
			BrokenTestController broken     = new BrokenTestController ();

			dispatcher.RegisterController (broken);
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

			v3.CommandLine = "TestSave";

			Assert.AreEqual ("TestSave", v3.CommandLine);
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

		[Test]
		public void CheckCommandMultiCommand()
		{
			Command command = Command.Get ("TestMulti");

			Command.SetCommandType (command.Caption, CommandType.Multiple);

			Command commandA = Command.Get ("TestCmdA");
			Command commandB = Command.Get ("TestCmdB");
			Command commandC = Command.Get ("TestCmdC");

			command.MultiCommands.Add (commandA);
			command.MultiCommands.Add (commandB);
			command.MultiCommands.Add (commandC);

			CommandContext context = new CommandContext ();
			CommandDispatcher dispatcher = new CommandDispatcher ();

			dispatcher.Register (command.CommandId, this.HandleCommandTestMulti);
			dispatcher.Register (commandA.CommandId, this.HandleCommandTestMulti);
			dispatcher.Register (commandB.CommandId, this.HandleCommandTestMulti);
			dispatcher.Register (commandC.CommandId, this.HandleCommandTestMulti);

			Visual v1 = new Visual ();

			CommandContext.SetContext (v1, context);
			CommandDispatcher.SetDispatcher (v1, dispatcher);

			CommandContextChain contextChain;
			CommandDispatcherChain dispatcherChain;

			contextChain = CommandContextChain.BuildChain (v1);
			dispatcherChain = CommandDispatcherChain.BuildChain (v1);

			CommandState state = context.GetCommandState (command);
			CommandState stateA = context.GetCommandState (commandA);
			CommandState stateB = context.GetCommandState (commandB);
			CommandState stateC = context.GetCommandState (commandC);

			state.Enable = true;

			CommandDispatcherTest.buffer.Length = 0;
			CommandDispatcher.Dispatch (dispatcherChain, contextChain, command.CommandId, this);
			Assert.AreEqual ("<CommandMulti:TestMulti-null>", CommandDispatcherTest.buffer.ToString ());

			MultiCommand.SetSelectedCommand (state, commandA);

			Assert.AreEqual (ActiveState.Yes, stateA.ActiveState);
			Assert.AreEqual (ActiveState.No, stateB.ActiveState);
			Assert.AreEqual (ActiveState.No, stateC.ActiveState);
			
			CommandDispatcherTest.buffer.Length = 0;
			CommandDispatcher.Dispatch (dispatcherChain, contextChain, command.CommandId, this);
			Assert.AreEqual ("<CommandMulti:TestMulti-TestCmdA>", CommandDispatcherTest.buffer.ToString ());

			dispatcher.Unregister (command.CommandId, this.HandleCommandTestMulti);

			CommandDispatcherTest.buffer.Length = 0;
			CommandDispatcher.Dispatch (dispatcherChain, contextChain, command.CommandId, this);
			Assert.AreEqual ("<CommandMulti:TestCmdA-null>", CommandDispatcherTest.buffer.ToString ());

			MultiCommand.SetSelectedCommand (state, commandB);
			
			Assert.AreEqual (ActiveState.No, stateA.ActiveState);
			Assert.AreEqual (ActiveState.Yes, stateB.ActiveState);
			Assert.AreEqual (ActiveState.No, stateC.ActiveState);

			CommandDispatcherTest.buffer.Length = 0;
			CommandDispatcher.Dispatch (dispatcherChain, contextChain, command.CommandId, this);
			Assert.AreEqual ("<CommandMulti:TestCmdB-null>", CommandDispatcherTest.buffer.ToString ());

			MultiCommand.SetSelectedCommand (state, null);

			Assert.AreEqual (ActiveState.No, stateA.ActiveState);
			Assert.AreEqual (ActiveState.No, stateB.ActiveState);
			Assert.AreEqual (ActiveState.No, stateC.ActiveState);
			
			CommandDispatcherTest.buffer.Length = 0;
			CommandDispatcher.Dispatch (dispatcherChain, contextChain, command.CommandId, this);
			Assert.AreEqual ("", CommandDispatcherTest.buffer.ToString ());
		}

		[Test]
		[ExpectedException (typeof (System.ArgumentException))]
		public void CheckCommandMultiCommandEx1()
		{
			Command command = Command.Get ("TestCmdA");

			CommandContext context = new CommandContext ();
			CommandDispatcher dispatcher = new CommandDispatcher ();

			dispatcher.Register (command.CommandId, this.HandleCommandTestMulti);

			Visual v1 = new Visual ();

			CommandContext.SetContext (v1, context);
			CommandDispatcher.SetDispatcher (v1, dispatcher);

			CommandContextChain contextChain;
			CommandDispatcherChain dispatcherChain;

			contextChain = CommandContextChain.BuildChain (v1);
			dispatcherChain = CommandDispatcherChain.BuildChain (v1);

			CommandState state = context.GetCommandState (command);

			MultiCommand.SetSelectedCommand (state, null);
		}

		[Test]
		[ExpectedException (typeof (Widgets.Exceptions.InfiniteCommandLoopException))]
		public void CheckCommandMultiCommandEx2()
		{
			Command command = Command.Get ("TestMultiX");
			Command.SetCommandType (command.Caption, CommandType.Multiple);

			command.MultiCommands.Add (command);

			CommandContext context = new CommandContext ();
			CommandDispatcher dispatcher = new CommandDispatcher ();

			Visual v1 = new Visual ();

			CommandContext.SetContext (v1, context);
			CommandDispatcher.SetDispatcher (v1, dispatcher);

			CommandContextChain contextChain;
			CommandDispatcherChain dispatcherChain;

			contextChain = CommandContextChain.BuildChain (v1);
			dispatcherChain = CommandDispatcherChain.BuildChain (v1);

			CommandState state = context.GetCommandState (command);

			MultiCommand.SetSelectedCommand (state, command);

			CommandDispatcher.Dispatch (dispatcherChain, contextChain, command.CommandId, this);
		}

		[Test]
		[ExpectedException (typeof (System.ArgumentException))]
		public void CheckCommandMultiCommandEx3()
		{
			Command command = Command.Get ("TestMultiY");
			Command.SetCommandType (command.Caption, CommandType.Multiple);

			Command commandA = Command.Get ("TestCmdA");
			Command commandB = Command.Get ("TestCmdB");
			Command commandC = Command.Get ("TestCmdC");

			command.MultiCommands.Add (commandA);
			command.MultiCommands.Add (commandB);

			CommandContext context = new CommandContext ();
			CommandDispatcher dispatcher = new CommandDispatcher ();

			Visual v1 = new Visual ();

			CommandContext.SetContext (v1, context);
			CommandDispatcher.SetDispatcher (v1, dispatcher);

			CommandContextChain contextChain;
			CommandDispatcherChain dispatcherChain;

			contextChain = CommandContextChain.BuildChain (v1);
			dispatcherChain = CommandDispatcherChain.BuildChain (v1);

			CommandState state = context.GetCommandState (command);

			MultiCommand.SetSelectedCommand (state, commandC);
		}

		[Test]
		[Ignore ("Broken - Command does not implement IEnumType")]
		public void CheckCommandMultiCommandEnum()
		{
			Command command = Command.Get ("TestMultiY");
			Command.SetCommandType (command.Caption, CommandType.Multiple);

			Command commandA = Command.Get ("TestCmdA");
			Command commandB = Command.Get ("TestCmdB");
			Command commandC = Command.Get ("TestCmdC");

			command.MultiCommands.AddRange (new Command[] { commandA, commandB, commandC });

#if false
			Types.IEnumType type = command;
			Types.IEnumValue[] values = Types.Collection.ToArray (type.Values);

			Assert.IsNotNull (type);
			Assert.AreEqual (3, values.Length);
			Assert.AreEqual (0, type[0].Rank);
			Assert.AreEqual (0, type["TestCmdA"].Rank);
			Assert.AreEqual (1, type["TestCmdB"].Rank);
			Assert.AreEqual (2, type["TestCmdC"].Rank);
			Assert.AreEqual ("TestCmdA", type[0].Name);
			Assert.IsFalse (type.IsCustomizable);
			Assert.IsFalse (type.IsDefinedAsFlags);

			CommandContext context = new CommandContext ();
			CommandState state = context.GetCommandState (command);

			Assert.AreEqual (command, Types.TypeRosetta.GetTypeObjectFromValue (state));
#endif
		}

		private void HandleCommandTestMulti(CommandDispatcher sender, CommandEventArgs e)
		{
			Assert.AreEqual (this, e.Source);
			Assert.IsNotNull (e.CommandState);

			Command command = e.CommandState.Command;
			command = command.CommandType == CommandType.Multiple ? MultiCommand.GetSelectedCommand (e.CommandState) : null;

			e.Executed = true;
			CommandDispatcherTest.buffer.Append ("<CommandMulti:");
			CommandDispatcherTest.buffer.Append (e.CommandName);
			CommandDispatcherTest.buffer.Append ("-");
			CommandDispatcherTest.buffer.Append (command == null ? "null" : command.CommandId);
			CommandDispatcherTest.buffer.Append (">");
		}

		[Test]
		public void CheckCommandStructuredCommand()
		{
			CommandContext context = new CommandContext ();
			CommandDispatcher dispatcher = new CommandDispatcher ();

			Visual v1 = new Visual ();

			CommandContext.SetContext (v1, context);
			CommandDispatcher.SetDispatcher (v1, dispatcher);

			Command command = Command.Get ("TestSetFontSize");
			Command.SetCommandType (command.Caption, CommandType.Structured);
			
			Types.StructuredType type = command.StructuredType;
			
			type.AddField ("Size", new Types.DecimalType (0.1M, 999.9M, 0.1M));
			type.AddField ("Units", new Types.EnumType (typeof (Text.Properties.SizeUnits)));

			CommandState state = context.GetCommandState (command);

			StructuredCommand.SetFieldValue (state, "Size", 12.0M);
			StructuredCommand.SetFieldValue (state, "Units", Text.Properties.SizeUnits.Points);

			Slider slider = new Slider ();

			slider.MinValue = 0.1M;
			slider.MaxValue = 99.9M;
			slider.Resolution = 0.1M;
			
			slider.SetBinding (Slider.ValueProperty, new Types.Binding (Types.BindingMode.TwoWay, state, "Size"));

			Assert.AreEqual (12.0M, slider.Value);

			slider.Value = 10.0M;

			Assert.AreEqual (10.0M, StructuredCommand.GetFieldValue (state, "Size"));
		}

		[Test]
		public void CheckDispatch()
		{
			CommandDispatcher dispatcher = new CommandDispatcher ();
			BaseTestController controller = new BaseTestController ();

			dispatcher.RegisterController (controller);

			CommandDispatcherTest.buffer.Length = 0;

			//	Vérifie que le dispatch se fait correctement.

			dispatcher.InternalDispatch (null, "private-base-a", null);
			dispatcher.InternalDispatch (null, "private-base-x", null);
			dispatcher.InternalDispatch (null, "protected-base-b", null);
			dispatcher.InternalDispatch (null, "public-base-virtual-c", null);
			dispatcher.InternalDispatch (null, "public-base-d", null);
			dispatcher.InternalDispatch (null, "public-base-virtual-e", null);	//	n'existe pas

			Assert.AreEqual ("ba/bx/bb/bc/bd/", CommandDispatcherTest.buffer.ToString ());
		}

		[Test]
		[Ignore ("Not implemented - broken")]
		public void CheckDispatchMultipleCommands()
		{
			CommandDispatcher dispatcher = new CommandDispatcher ();
			BaseTestController controller = new BaseTestController ();

			dispatcher.RegisterController (controller);

			CommandDispatcherTest.buffer.Length = 0;

			//	Vérifie que le dispatch se fait correctement.

			dispatcher.InternalDispatch (null, "private-base-a -> protected-base-b -> public-base-virtual-c", null);
			dispatcher.InternalDispatch (null, "private-base-a -> cancel-multiple", null);
			dispatcher.InternalDispatch (null, "private-base-a -> cancel-multiple -> protected-base-b -> public-base-virtual-c", null);

			Assert.AreEqual ("ba/bb/bc/ba/cm/ba/CM/", CommandDispatcherTest.buffer.ToString ());
		}

		[Test]
		public void CheckDispatchDerived()
		{
			CommandDispatcher dispatcher = new CommandDispatcher ();
			DerivedTestController derived    = new DerivedTestController ();

			dispatcher.RegisterController (derived);

			CommandDispatcherTest.buffer.Length = 0;

			//	Vérifie que le dispatch se fait correctement, et que la visibilité des méthodes
			//	est bien respectée.

			dispatcher.InternalDispatch (null, "private-base-a", null);			//	privé, pas accessible dans la version dérivée
			dispatcher.InternalDispatch (null, "private-base-x", null);			//	privé, pas accessible dans la version dérivée
			dispatcher.InternalDispatch (null, "protected-base-b", null);		//	accessible et surchargé par 'new' -> visible
			dispatcher.InternalDispatch (null, "public-base-virtual-c", null);	//	accessible et surchargé par 'override' -> pas visible
			dispatcher.InternalDispatch (null, "public-base-d", null);			//	accessible et non surchargé -> visible
			dispatcher.InternalDispatch (null, "public-base-virtual-e", null);	//	accessible mais non marqué :-)
			dispatcher.InternalDispatch (null, "private-derived-a", null);		//	privé, accessible localement -> visible
			dispatcher.InternalDispatch (null, "protected-new-b", null);		//	accessible, surcharge la base -> visible
			dispatcher.InternalDispatch (null, "public-override-c", null);		//	accessible, surcharge la base -> visible
			dispatcher.InternalDispatch (null, "public-override-e", null);		//	accessible, surcharge la base -> visible

			Assert.AreEqual ("bb/bd/da/db/dc/de/", CommandDispatcherTest.buffer.ToString ());
		}

		[Test]
		public void CheckExtractCommandArgs()
		{
			string cmd1 = "foo";
			string cmd2 = "foo ()";
			string cmd3 = "foo (a)";
			string cmd4 = "foo (this.Name, \"Hello, world !\", 123)";
			string cmd5 = "foo ( this.Name , 'Hello, world !' , 123 ) ";
			string cmd6 = "foo(this.Name,\"Hello, world !\",123)";

			Assert.IsTrue (CommandDispatcher.IsSimpleCommand (cmd1));
			Assert.IsTrue (CommandDispatcher.IsSimpleCommand (cmd2) == false);

			Assert.AreEqual ("foo", CommandDispatcher.ExtractCommandName (cmd1));
			Assert.AreEqual ("foo", CommandDispatcher.ExtractCommandName (cmd2));
			Assert.AreEqual ("foo", CommandDispatcher.ExtractCommandName (cmd3));
			Assert.AreEqual ("foo", CommandDispatcher.ExtractCommandName (cmd4));
			Assert.AreEqual ("foo", CommandDispatcher.ExtractCommandName (cmd5));
			Assert.AreEqual ("foo", CommandDispatcher.ExtractCommandName (cmd6));

			Assert.AreEqual (0, CommandDispatcher.ExtractCommandArgs (cmd1).Length);
			Assert.AreEqual (0, CommandDispatcher.ExtractCommandArgs (cmd2).Length);
			Assert.AreEqual (1, CommandDispatcher.ExtractCommandArgs (cmd3).Length);
			Assert.AreEqual (3, CommandDispatcher.ExtractCommandArgs (cmd4).Length);
			Assert.AreEqual (3, CommandDispatcher.ExtractCommandArgs (cmd5).Length);
			Assert.AreEqual (3, CommandDispatcher.ExtractCommandArgs (cmd6).Length);

			Assert.AreEqual ("a", CommandDispatcher.ExtractCommandArgs (cmd3)[0]);
			Assert.AreEqual ("this.Name", CommandDispatcher.ExtractCommandArgs (cmd4)[0]);
			Assert.AreEqual ("\"Hello, world !\"", CommandDispatcher.ExtractCommandArgs (cmd4)[1]);
			Assert.AreEqual ("123", CommandDispatcher.ExtractCommandArgs (cmd4)[2]);
			Assert.AreEqual ("this.Name", CommandDispatcher.ExtractCommandArgs (cmd5)[0]);
			Assert.AreEqual ("'Hello, world !'", CommandDispatcher.ExtractCommandArgs (cmd5)[1]);
			Assert.AreEqual ("123", CommandDispatcher.ExtractCommandArgs (cmd5)[2]);
			Assert.AreEqual ("\"Hello, world !\"", CommandDispatcher.ExtractCommandArgs (cmd6)[1]);
			Assert.AreEqual ("123", CommandDispatcher.ExtractCommandArgs (cmd6)[2]);
		}

		[Test]
		[ExpectedException (typeof (System.FormatException))]
		public void CheckExtractCommandArgsEx1()
		{
			string cmd = "foo (123x)";
			CommandDispatcher.ExtractCommandArgs (cmd);
		}

		[Test]
		[ExpectedException (typeof (System.FormatException))]
		public void CheckExtractCommandArgsEx2()
		{
			string cmd = "foo..bar (1)";
			CommandDispatcher.ExtractCommandArgs (cmd);
		}

		[Test]
		[ExpectedException (typeof (System.FormatException))]
		public void CheckExtractCommandArgsEx3()
		{
			string cmd = "foo ('x'x')";
			CommandDispatcher.ExtractCommandArgs (cmd);
		}

		[Test]
		[ExpectedException (typeof (System.FormatException))]
		public void CheckExtractCommandArgsEx4()
		{
			string cmd = "foo (a.)";
			CommandDispatcher.ExtractCommandArgs (cmd);
		}

		[Test]
		[ExpectedException (typeof (System.FormatException))]
		public void CheckExtractCommandArgsEx5()
		{
			string cmd = "foo (a,)";
			CommandDispatcher.ExtractCommandArgs (cmd);
		}

		[Test]
		[ExpectedException (typeof (System.FormatException))]
		public void CheckExtractCommandArgsEx6()
		{
			string cmd = "foo (a";
			CommandDispatcher.ExtractCommandArgs (cmd);
		}

		[Test]
		public void CheckExtractAndParseCommandArgs()
		{
			string cmd  = "foo (this.Name, 'abc', 123.456, bar, this.Mode)";
			string[] args = CommandDispatcher.ExtractCommandArgs (cmd);
			string[] vals = CommandDispatcher.ExtractAndParseCommandArgs (cmd, this);
			string[] expect = new string[] { "test", "abc", "123.456", "bar", "Funny" };

			for (int i = 0; i < args.Length; i++)
			{
				Assert.AreEqual (expect[i], vals[i], string.Format ("{0} mismatched;", args[i]));
			}
		}

		[Test]
		[ExpectedException (typeof (System.FieldAccessException))]
		public void CheckExtractAndParseCommandArgsEx1()
		{
			string cmd  = "foo (this.Name, 'abc', 123.456, bar, this.DoesNotExist)";
			string[] args = CommandDispatcher.ExtractCommandArgs (cmd);
			string[] vals = CommandDispatcher.ExtractAndParseCommandArgs (cmd, this);
		}


		#region Test Properties used by ExtractAndParseCommandArgs
		public string Name
		{
			get
			{
				return "test";
			}
		}

		public TestMode Mode
		{
			get
			{
				return TestMode.Funny;
			}
		}

		public enum TestMode
		{
			Boring,
			Funny,
			Strange
		}
		#endregion

#if false
		#region MyCommandState Class
		class MyCommandState : CommandState
		{
			public MyCommandState(string name, CommandDispatcher dispatcher) : base (name, dispatcher)
			{
			}
			
			protected override void Synchronize()
			{
				CommandDispatcherTest.buffer.Append (this.Name);
				CommandDispatcherTest.buffer.Append ("/");
			}

			public override bool Enable
			{
				get
				{
					return false;
				}
				set
				{
				}
			}

		}
		#endregion
#endif

		#region XyzTestController classes
		public class BaseTestController
		{
			public BaseTestController()
			{
			}

			[Command ("private-base-a")]
			private void PrivateA()
			{
				CommandDispatcherTest.buffer.Append ("ba/");
			}

			[Command ("private-base-x")]
			private void PrivateX()
			{
				CommandDispatcherTest.buffer.Append ("bx/");
			}

			[Command ("protected-base-b")]
			protected void ProtectedB()
			{
				CommandDispatcherTest.buffer.Append ("bb/");
			}

			[Command ("public-base-virtual-c")]
			public virtual void PublicC()
			{
				CommandDispatcherTest.buffer.Append ("bc/");
			}

			[Command ("public-base-d")]
			public void PublicD(CommandDispatcher dispatcher, CommandEventArgs e)
			{
				CommandDispatcherTest.buffer.Append ("bd/");
			}

#if false //#fix
			[Command ("cancel-multiple")] public void CancelMultiple(CommandDispatcher dispatcher, CommandEventArgs e)
			{
				if (dispatcher.HasPendingMultipleCommands)
				{
					CommandDispatcherTest.buffer.Append ("CM/");
					dispatcher.InternalCancelTopPendingMultipleCommands ();
				}
				else
				{
					CommandDispatcherTest.buffer.Append ("cm/");
				}
			}
#endif

			public virtual void PublicE()
			{
				CommandDispatcherTest.buffer.Append ("be/");
			}
		}

		public class DerivedTestController : BaseTestController
		{
			public DerivedTestController()
			{
			}

			[Command ("private-derived-a")]
			private void PrivateA(CommandDispatcher dispatcher)
			{
				CommandDispatcherTest.buffer.Append ("da/");
			}

			[Command ("protected-new-b")]
			protected new void ProtectedB()
			{
				CommandDispatcherTest.buffer.Append ("db/");
			}

			[Command ("public-override-c")]
			public override void PublicC()
			{
				CommandDispatcherTest.buffer.Append ("dc/");
			}

			[Command ("public-override-e")]
			public override void PublicE()
			{
				CommandDispatcherTest.buffer.Append ("de/");
			}
		}

		public class BrokenTestController
		{
			public BrokenTestController()
			{
			}

			[Command ("broken")]
			public void BrokenMethod(string command)
			{
			}
		}
		#endregion

#if false
		[Test] public void CheckValidators()
		{
			CommandDispatcher dispatcher = new CommandDispatcher ();
			
			ValidationRule v1  = new ValidationRule ("v1");
			ValidationRule v11 = new ValidationRule ("v11");
			ValidationRule v2  = new ValidationRule ("v2");
			
			v1.Validators.Add (v11);
			
			dispatcher.ValidationRule.Validators.Add (v1);
			dispatcher.ValidationRule.Validators.Add (v2);
			
			Assert.AreEqual (ValidationState.Dirty, v1.State);
			Assert.AreEqual (ValidationState.Dirty, v11.State);
			Assert.AreEqual (ValidationState.Dirty, v2.State);
			
			dispatcher.SyncValidationRule ();
			
			Assert.AreEqual (ValidationState.Ok, v1.State);
			Assert.AreEqual (ValidationState.Ok, v11.State);
			Assert.AreEqual (ValidationState.Ok, v2.State);
			
			v1.MakeDirty (false);
			
			Assert.AreEqual (ValidationState.Dirty, v1.State);
			Assert.AreEqual (ValidationState.Ok, v11.State);
			Assert.AreEqual (ValidationState.Ok, v2.State);
			
			v1.MakeDirty (true);
			
			Assert.AreEqual (ValidationState.Dirty, v1.State);
			Assert.AreEqual (ValidationState.Dirty, v11.State);
			Assert.AreEqual (ValidationState.Ok, v2.State);
			
			dispatcher.SyncValidationRule ();
			
			v2.MakeDirty (false);
			
			Assert.AreEqual (ValidationState.Ok, v1.State);
			Assert.AreEqual (ValidationState.Ok, v11.State);
			Assert.AreEqual (ValidationState.Dirty, v2.State);
			
			v11.MakeDirty (false);
			
			Assert.AreEqual (ValidationState.Dirty, v1.State);
			Assert.AreEqual (ValidationState.Dirty, v11.State);
			Assert.AreEqual (ValidationState.Dirty, v2.State);
		}
#endif

		static System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
	}
#endif
}
