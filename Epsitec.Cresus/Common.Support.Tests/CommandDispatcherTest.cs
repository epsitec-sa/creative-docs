using NUnit.Framework;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.Support
{
	[TestFixture] public class CommandDispatcherTest
	{
		[Test] public void CheckRegisterController()
		{
			CommandDispatcher     dispatcher = new CommandDispatcher ();
			BaseTestController    controller = new BaseTestController ();
			DerivedTestController derived    = new DerivedTestController ();
			
			dispatcher.RegisterController (controller);
			dispatcher.RegisterController (derived);
		}
		
		[Test] [ExpectedException (typeof (System.FormatException))] public void CheckRegisterControllerEx1()
		{
			CommandDispatcher    dispatcher = new CommandDispatcher ();
			BrokenTestController broken     = new BrokenTestController ();
			
			dispatcher.RegisterController (broken);
		}
		
		
		[Test] public void CheckDispatch()
		{
			CommandDispatcher  dispatcher = new CommandDispatcher ();
			BaseTestController controller = new BaseTestController ();
			
			dispatcher.RegisterController (controller);
			
			CommandDispatcherTest.buffer.Length = 0;
			
			//	Vérifie que le dispatch se fait correctement.
			
			dispatcher.InternalDispatch ("private-base-a", null);
			dispatcher.InternalDispatch ("private-base-x", null);
			dispatcher.InternalDispatch ("protected-base-b", null);
			dispatcher.InternalDispatch ("public-base-virtual-c", null);
			dispatcher.InternalDispatch ("public-base-d", null);
			dispatcher.InternalDispatch ("public-base-virtual-e", null);	//	n'existe pas
			
			Assert.AreEqual ("ba/bx/bb/bc/bd/", CommandDispatcherTest.buffer.ToString ());
		}
		
		[Test] [Ignore ("Not implemented - broken")] public void CheckDispatchMultipleCommands()
		{
			CommandDispatcher  dispatcher = new CommandDispatcher ();
			BaseTestController controller = new BaseTestController ();
			
			dispatcher.RegisterController (controller);
			
			CommandDispatcherTest.buffer.Length = 0;
			
			//	Vérifie que le dispatch se fait correctement.
			
			dispatcher.InternalDispatch ("private-base-a -> protected-base-b -> public-base-virtual-c", null);
			dispatcher.InternalDispatch ("private-base-a -> cancel-multiple", null);
			dispatcher.InternalDispatch ("private-base-a -> cancel-multiple -> protected-base-b -> public-base-virtual-c", null);
			
			Assert.AreEqual ("ba/bb/bc/ba/cm/ba/CM/", CommandDispatcherTest.buffer.ToString ());
		}
		
		[Test] public void CheckDispatchDerived()
		{
			CommandDispatcher     dispatcher = new CommandDispatcher ();
			DerivedTestController derived    = new DerivedTestController ();
			
			dispatcher.RegisterController (derived);
			
			CommandDispatcherTest.buffer.Length = 0;
			
			//	Vérifie que le dispatch se fait correctement, et que la visibilité des méthodes
			//	est bien respectée.
			
			dispatcher.InternalDispatch ("private-base-a", null);			//	privé, pas accessible dans la version dérivée
			dispatcher.InternalDispatch ("private-base-x", null);			//	privé, pas accessible dans la version dérivée
			dispatcher.InternalDispatch ("protected-base-b", null);			//	accessible et surchargé par 'new' -> visible
			dispatcher.InternalDispatch ("public-base-virtual-c", null);	//	accessible et surchargé par 'override' -> pas visible
			dispatcher.InternalDispatch ("public-base-d", null);			//	accessible et non surchargé -> visible
			dispatcher.InternalDispatch ("public-base-virtual-e", null);	//	accessible mais non marqué :-)
			dispatcher.InternalDispatch ("private-derived-a", null);		//	privé, accessible localement -> visible
			dispatcher.InternalDispatch ("protected-new-b", null);			//	accessible, surcharge la base -> visible
			dispatcher.InternalDispatch ("public-override-c", null);		//	accessible, surcharge la base -> visible
			dispatcher.InternalDispatch ("public-override-e", null);		//	accessible, surcharge la base -> visible
			
			Assert.AreEqual ("bb/bd/da/db/dc/de/", CommandDispatcherTest.buffer.ToString ());
		}
		
		[Test] public void CheckExtractCommandArgs()
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
			Assert.AreEqual ("this.Name",          CommandDispatcher.ExtractCommandArgs (cmd4)[0]);
			Assert.AreEqual ("\"Hello, world !\"", CommandDispatcher.ExtractCommandArgs (cmd4)[1]);
			Assert.AreEqual ("123",                CommandDispatcher.ExtractCommandArgs (cmd4)[2]);
			Assert.AreEqual ("this.Name",          CommandDispatcher.ExtractCommandArgs (cmd5)[0]);
			Assert.AreEqual ("'Hello, world !'",   CommandDispatcher.ExtractCommandArgs (cmd5)[1]);
			Assert.AreEqual ("123",                CommandDispatcher.ExtractCommandArgs (cmd5)[2]);
			Assert.AreEqual ("\"Hello, world !\"", CommandDispatcher.ExtractCommandArgs (cmd6)[1]);
			Assert.AreEqual ("123",                CommandDispatcher.ExtractCommandArgs (cmd6)[2]);
		}
		
		[Test] [ExpectedException (typeof (System.FormatException))] public void CheckExtractCommandArgsEx1()
		{
			string cmd = "foo (123x)";
			CommandDispatcher.ExtractCommandArgs (cmd);
		}
		
		[Test] [ExpectedException (typeof (System.FormatException))] public void CheckExtractCommandArgsEx2()
		{
			string cmd = "foo..bar (1)";
			CommandDispatcher.ExtractCommandArgs (cmd);
		}
		
		[Test] [ExpectedException (typeof (System.FormatException))] public void CheckExtractCommandArgsEx3()
		{
			string cmd = "foo ('x'x')";
			CommandDispatcher.ExtractCommandArgs (cmd);
		}
		
		[Test] [ExpectedException (typeof (System.FormatException))] public void CheckExtractCommandArgsEx4()
		{
			string cmd = "foo (a.)";
			CommandDispatcher.ExtractCommandArgs (cmd);
		}
		
		[Test] [ExpectedException (typeof (System.FormatException))] public void CheckExtractCommandArgsEx5()
		{
			string cmd = "foo (a,)";
			CommandDispatcher.ExtractCommandArgs (cmd);
		}
		
		[Test] [ExpectedException (typeof (System.FormatException))] public void CheckExtractCommandArgsEx6()
		{
			string cmd = "foo (a";
			CommandDispatcher.ExtractCommandArgs (cmd);
		}
		
		[Test] public void CheckExtractAndParseCommandArgs()
		{
			string   cmd  = "foo (this.Name, 'abc', 123.456, bar, this.Mode)";
			string[] args = CommandDispatcher.ExtractCommandArgs (cmd);
			string[] vals = CommandDispatcher.ExtractAndParseCommandArgs (cmd, this);
			string[] expect = new string[] { "test", "abc", "123.456", "bar", "Funny" };
			
			for (int i = 0; i < args.Length; i++)
			{
				Assert.AreEqual (expect[i], vals[i], string.Format ("{0} mismatched;", args[i]));
			}
		}
		
		[Test] [ExpectedException (typeof (System.FieldAccessException))] public void CheckExtractAndParseCommandArgsEx1()
		{
			string   cmd  = "foo (this.Name, 'abc', 123.456, bar, this.DoesNotExist)";
			string[] args = CommandDispatcher.ExtractCommandArgs (cmd);
			string[] vals = CommandDispatcher.ExtractAndParseCommandArgs (cmd, this);
		}
		
		
		#region Test Properties used by ExtractAndParseCommandArgs
		public string						Name
		{
			get { return "test"; }
		}
		
		public TestMode						Mode
		{
			get { return TestMode.Funny; }
		}
		
		public enum TestMode
		{
			Boring, Funny, Strange
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
			
			[Command ("private-base-a")] private void PrivateA()
			{
				CommandDispatcherTest.buffer.Append ("ba/");
			}
			
			[Command ("private-base-x")] private void PrivateX()
			{
				CommandDispatcherTest.buffer.Append ("bx/");
			}
			
			[Command ("protected-base-b")] protected void ProtectedB()
			{
				CommandDispatcherTest.buffer.Append ("bb/");
			}
			
			[Command ("public-base-virtual-c")] public virtual void PublicC()
			{
				CommandDispatcherTest.buffer.Append ("bc/");
			}
			
			[Command ("public-base-d")] public void PublicD(CommandDispatcher dispatcher, CommandEventArgs e)
			{
				CommandDispatcherTest.buffer.Append ("bd/");
			}
			
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
			
			[Command ("private-derived-a")] private void PrivateA(CommandDispatcher dispatcher)
			{
				CommandDispatcherTest.buffer.Append ("da/");
			}
			
			[Command ("protected-new-b")] protected new void ProtectedB()
			{
				CommandDispatcherTest.buffer.Append ("db/");
			}
			
			[Command ("public-override-c")] public override void PublicC()
			{
				CommandDispatcherTest.buffer.Append ("dc/");
			}
			
			[Command ("public-override-e")] public override void PublicE()
			{
				CommandDispatcherTest.buffer.Append ("de/");
			}
		}
		
		public class BrokenTestController
		{
			public BrokenTestController()
			{
			}
			
			[Command ("broken")] public void BrokenMethod(string command)
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
}
