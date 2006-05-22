using NUnit.Framework;

[assembly: Epsitec.Common.Widgets.Controller (typeof (Epsitec.Common.Widgets.PlaceholderTest.TestController1))]

namespace Epsitec.Common.Widgets
{
	[TestFixture] public class PlaceholderTest
	{
		[Test]
		public void AutomatedTestEnvironment()
		{
			Epsitec.Common.Widgets.Window.RunningInAutomatedTestEnvironment = true;
		}

		[Test]
		public void CheckControllerCreation()
		{
			IController c1 = Controllers.Factory.CreateController ("TestController1", "x");
			IController c2 = Controllers.Factory.CreateController ("TestController1", "y");

			Assert.IsNotNull (c1);
			Assert.IsNotNull (c2);

			Assert.AreEqual (typeof (TestController1), c1.GetType ());
			Assert.AreEqual (typeof (TestController1), c2.GetType ());

			TestController1 tc1 = c1 as TestController1;
			TestController1 tc2 = c2 as TestController1;

			Assert.AreEqual ("x", tc1.Parameter);
			Assert.AreEqual ("y", tc2.Parameter);
		}

		internal class TestController1 : Controllers.AbstractController
		{
			public TestController1(string parameter)
			{
				this.parameter = parameter;
			}

			public string Parameter
			{
				get
				{
					return this.parameter;
				}
			}
			
			protected override void CreateUserInterface(object value)
			{
				throw new System.Exception ("The method or operation is not implemented.");
			}

			private string parameter;
		}
	}
}
