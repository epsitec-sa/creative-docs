using NUnit.Framework;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;

namespace Epsitec.Common.Widgets
{
	[TestFixture] public class CommandTest
	{
		[SetUp]
		public void Initialise()
		{
			Epsitec.Common.Document.Engine.Initialise ();
			Epsitec.Common.Widgets.Adorners.Factory.SetActive ("LookMetal");
		}

		[Test]
		public void CheckCommandFromDruid()
		{
			Command c1 = Command.Find (Druid.Parse ("[0]"));
			Command c2 = Command.Get (Druid.Parse ("[0]"));

			Assert.IsNull (c1);
			Assert.IsNotNull (c2);
			Assert.AreEqual ("[0]", c2.Name);
		}
	}
}