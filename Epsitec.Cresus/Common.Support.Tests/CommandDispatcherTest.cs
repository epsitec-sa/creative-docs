using NUnit.Framework;

namespace Epsitec.Common.Support
{
	[TestFixture] public class CommandDispatcherTest
	{
		[Test] public void CheckRegisterController()
		{
			CommandDispatcher dispatcher = new CommandDispatcher ();
			TestController    controller = new TestController ();
			
			dispatcher.RegisterController (controller);
		}
		
		
		public class TestController
		{
			public TestController()
			{
			}
			
			[Command ("a")] private void PrivateA()
			{
			}
			
			[Command ("b")] protected void ProtectedB()
			{
			}
			
			[Command ("c")] public void PublicC()
			{
			}
		}
	}
}
