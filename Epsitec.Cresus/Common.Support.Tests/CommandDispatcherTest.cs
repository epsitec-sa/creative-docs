using NUnit.Framework;

namespace Epsitec.Common.Support
{
	[TestFixture] public class CommandDispatcherTest
	{
		[Test] public void CheckRegisterController()
		{
			CommandDispatcher     dispatcher = new CommandDispatcher ();
			DerivedTestController derived    = new DerivedTestController ();
			
			dispatcher.RegisterController (derived);
			
			CommandDispatcherTest.buffer.Length = 0;
			
			//	Vérifie que le dispatch se fait correctement, et que la visibilité des méthodes
			//	soit respectée.
			
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
		
		[Test] [ExpectedException (typeof (System.FormatException))] public void CheckRegisterControllerEx1()
		{
			CommandDispatcher    dispatcher = new CommandDispatcher ();
			BrokenTestController broken     = new BrokenTestController ();
			
			dispatcher.RegisterController (broken);
		}
		
		static System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
		
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
	}
}
