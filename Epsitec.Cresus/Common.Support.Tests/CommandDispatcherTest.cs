using NUnit.Framework;

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
			
			//	V�rifie que le dispatch se fait correctement.
			
			dispatcher.Dispatch ("private-base-a", null);
			dispatcher.Dispatch ("private-base-x", null);
			dispatcher.Dispatch ("protected-base-b", null);
			dispatcher.Dispatch ("public-base-virtual-c", null);
			dispatcher.Dispatch ("public-base-d", null);
			dispatcher.Dispatch ("public-base-virtual-e", null);	//	n'existe pas
			
			Assertion.AssertEquals ("ba/bx/bb/bc/bd/", CommandDispatcherTest.buffer.ToString ());
		}
		
		[Test] public void CheckDispatchDerived()
		{
			CommandDispatcher     dispatcher = new CommandDispatcher ();
			DerivedTestController derived    = new DerivedTestController ();
			
			dispatcher.RegisterController (derived);
			
			CommandDispatcherTest.buffer.Length = 0;
			
			//	V�rifie que le dispatch se fait correctement, et que la visibilit� des m�thodes
			//	est bien respect�e.
			
			dispatcher.Dispatch ("private-base-a", null);			//	priv�, pas accessible dans la version d�riv�e
			dispatcher.Dispatch ("private-base-x", null);			//	priv�, pas accessible dans la version d�riv�e
			dispatcher.Dispatch ("protected-base-b", null);			//	accessible et surcharg� par 'new' -> visible
			dispatcher.Dispatch ("public-base-virtual-c", null);	//	accessible et surcharg� par 'override' -> pas visible
			dispatcher.Dispatch ("public-base-d", null);			//	accessible et non surcharg� -> visible
			dispatcher.Dispatch ("public-base-virtual-e", null);	//	accessible mais non marqu� :-)
			dispatcher.Dispatch ("private-derived-a", null);		//	priv�, accessible localement -> visible
			dispatcher.Dispatch ("protected-new-b", null);			//	accessible, surcharge la base -> visible
			dispatcher.Dispatch ("public-override-c", null);		//	accessible, surcharge la base -> visible
			dispatcher.Dispatch ("public-override-e", null);		//	accessible, surcharge la base -> visible
			
			Assertion.AssertEquals ("bb/bd/da/db/dc/de/", CommandDispatcherTest.buffer.ToString ());
		}
		
		[Test] public void CheckCommandState()
		{
			CommandDispatcher dispatcher = new CommandDispatcher ();
			
			CommandDispatcherTest.buffer.Length = 0;
			
			CommandState s1 = new CommandState ("s1", dispatcher);
			CommandState s2 = new CommandState ("s2", dispatcher);
			CommandState s3 = new CommandState ("s3", dispatcher);
			
			dispatcher.SynchroniseCommandStates ();
			
			Assertion.AssertEquals ("s1/s2/s3/", CommandDispatcherTest.buffer.ToString ());
		}
		
		[Test] public void CheckExtractCommandArgs()
		{
			string cmd1 = "foo";
			string cmd2 = "foo ()";
			string cmd3 = "foo (a)";
			string cmd4 = "foo (this.Name, \"Hello, world !\", 123)";
			string cmd5 = "foo ( this.Name , 'Hello, world !' , 123 ) ";
			
			Assertion.Assert (CommandDispatcher.IsSimpleCommand (cmd1));
			Assertion.Assert (CommandDispatcher.IsSimpleCommand (cmd2) == false);
			
			Assertion.AssertEquals ("foo", CommandDispatcher.ExtractCommandName (cmd1));
			Assertion.AssertEquals ("foo", CommandDispatcher.ExtractCommandName (cmd2));
			Assertion.AssertEquals ("foo", CommandDispatcher.ExtractCommandName (cmd3));
			Assertion.AssertEquals ("foo", CommandDispatcher.ExtractCommandName (cmd4));
			Assertion.AssertEquals ("foo", CommandDispatcher.ExtractCommandName (cmd5));
			
			Assertion.AssertEquals (0, CommandDispatcher.ExtractCommandArgs (cmd1).Length);
			Assertion.AssertEquals (0, CommandDispatcher.ExtractCommandArgs (cmd2).Length);
			Assertion.AssertEquals (1, CommandDispatcher.ExtractCommandArgs (cmd3).Length);
			Assertion.AssertEquals (3, CommandDispatcher.ExtractCommandArgs (cmd4).Length);
			Assertion.AssertEquals (3, CommandDispatcher.ExtractCommandArgs (cmd5).Length);

			Assertion.AssertEquals ("a", CommandDispatcher.ExtractCommandArgs (cmd3)[0]);
			Assertion.AssertEquals ("this.Name",          CommandDispatcher.ExtractCommandArgs (cmd4)[0]);
			Assertion.AssertEquals ("\"Hello, world !\"", CommandDispatcher.ExtractCommandArgs (cmd4)[1]);
			Assertion.AssertEquals ("123",                CommandDispatcher.ExtractCommandArgs (cmd4)[2]);
			Assertion.AssertEquals ("this.Name",          CommandDispatcher.ExtractCommandArgs (cmd5)[0]);
			Assertion.AssertEquals ("'Hello, world !'",   CommandDispatcher.ExtractCommandArgs (cmd5)[1]);
			Assertion.AssertEquals ("123",                CommandDispatcher.ExtractCommandArgs (cmd5)[2]);
		}
		
		class CommandState : CommandDispatcher.CommandState
		{
			public CommandState(string name, CommandDispatcher dispatcher) : base (name, dispatcher)
			{
			}
			
			public override void Synchronise()
			{
				CommandDispatcherTest.buffer.Append (this.Name);
				CommandDispatcherTest.buffer.Append ("/");
			}

		}
		
		static System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
		
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
	}
}
