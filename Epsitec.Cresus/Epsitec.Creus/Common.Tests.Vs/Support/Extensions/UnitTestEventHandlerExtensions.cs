using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;


using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Epsitec.Common.Tests.Vs.Support.Extensions
{


	[TestClass]
	public sealed class UnitTestEventHandlerExtensions
	{


		[TestMethod]
		public void TestNullHandler()
		{
			EventHandler<EventArgs> eventHandler = null;

			eventHandler.Raise (null, null);
		}


		[TestMethod]
		public void TestNormalHandler()
		{
			int i = 0;

			var sender = new object();
			var args = new EventArgs();

			EventHandler<EventArgs> handler = (s, e) =>
			{
				Assert.AreSame (sender, s);
				Assert.AreSame (args, e);

				i = i + 1;
			};

			handler.Raise (sender, args);

			Assert.AreEqual (1, i);
		}


	}


}
