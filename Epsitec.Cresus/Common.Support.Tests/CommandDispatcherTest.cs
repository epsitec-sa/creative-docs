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
			
			//	Vérifie que le dispatch se fait correctement.
			
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
			
			//	Vérifie que le dispatch se fait correctement, et que la visibilité des méthodes
			//	est bien respectée.
			
			dispatcher.Dispatch ("private-base-a", null);			//	privé, pas accessible dans la version dérivée
			dispatcher.Dispatch ("private-base-x", null);			//	privé, pas accessible dans la version dérivée
			dispatcher.Dispatch ("protected-base-b", null);			//	accessible et surchargé par 'new' -> visible
			dispatcher.Dispatch ("public-base-virtual-c", null);	//	accessible et surchargé par 'override' -> pas visible
			dispatcher.Dispatch ("public-base-d", null);			//	accessible et non surchargé -> visible
			dispatcher.Dispatch ("public-base-virtual-e", null);	//	accessible mais non marqué :-)
			dispatcher.Dispatch ("private-derived-a", null);		//	privé, accessible localement -> visible
			dispatcher.Dispatch ("protected-new-b", null);			//	accessible, surcharge la base -> visible
			dispatcher.Dispatch ("public-override-c", null);		//	accessible, surcharge la base -> visible
			dispatcher.Dispatch ("public-override-e", null);		//	accessible, surcharge la base -> visible
			
			Assertion.AssertEquals ("bb/bd/da/db/dc/de/", CommandDispatcherTest.buffer.ToString ());
		}
		
		[Test] public void CheckDispatchMultipart()
		{
			CommandDispatcher       dispatcher = new CommandDispatcher ();
			MultipartTestController multipart  = new MultipartTestController ();
			
			dispatcher.RegisterController (multipart);
			
			//	Vérifie que le dispatch se fait correctement et que les sous-parties des commandes sont
			//	correctement interprétées.
			
			CommandDispatcherTest.buffer.Length = 0;
			dispatcher.Dispatch ("a.b.c", null);
			Assertion.AssertEquals ("a.b.c/*.b.c/*.c/", CommandDispatcherTest.buffer.ToString ());
			
			CommandDispatcherTest.buffer.Length = 0;
			dispatcher.Dispatch ("x.b.c", null);
			Assertion.AssertEquals ("*.b.c/*.c/", CommandDispatcherTest.buffer.ToString ());
			
			CommandDispatcherTest.buffer.Length = 0;
			dispatcher.Dispatch ("x.y.c", null);
			Assertion.AssertEquals ("*.c/", CommandDispatcherTest.buffer.ToString ());
			
			CommandDispatcherTest.buffer.Length = 0;
			dispatcher.Dispatch ("q.r.s.b.c", null);
			Assertion.AssertEquals ("*.b.c/*.c/", CommandDispatcherTest.buffer.ToString ());
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
		
		public class MultipartTestController
		{
			public MultipartTestController()
			{
			}
			
			[Command ("a.b.c")] public void Method1()
			{
				CommandDispatcherTest.buffer.Append ("a.b.c/");
			}
			
			[Command ("*.b.c")] public void Method2()
			{
				CommandDispatcherTest.buffer.Append ("*.b.c/");
			}
			
			[Command ("*.c")] public void Method3()
			{
				CommandDispatcherTest.buffer.Append ("*.c/");
			}
		}
		#endregion
	}
}
