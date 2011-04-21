using Epsitec.Common.Support;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;


namespace Epsitec.Common.Tests.Vs.Support
{


	[TestClass]
	public sealed class UnitTestSafeCounter
	{
		

		
		[TestMethod]
		public void SimpleTest()
		{
			SafeCounter counter = new SafeCounter ();

			Assert.IsTrue (counter.IsZero);

			using (System.IDisposable disposable = counter.Enter ())
			{
				Assert.IsNotNull (disposable);

				Assert.IsFalse (counter.IsZero);
			}

			Assert.IsTrue (counter.IsZero);
		}



		[TestMethod]
		public void ComplexeTest()
		{
			SafeCounter counter = new SafeCounter ();

			Assert.IsTrue (counter.IsZero);

			List<System.IDisposable> disposables = new List<System.IDisposable>();

			for (int i = 0; i < 10; i++)
			{
				var disposable = counter.Enter ();

				Assert.IsNotNull (disposable);

				disposables.Add (disposable);

				Assert.IsFalse (counter.IsZero);
			}

			for (int i = 9; i >= 0; i--)
			{
				Assert.IsFalse (counter.IsZero);

				disposables[i].Dispose ();
			}

			Assert.IsTrue (counter.IsZero);
		}


		[TestMethod]
		public void IfZeroTest()
		{
			SafeCounter counter = new SafeCounter ();

			int nbCalls = 0;

			System.Action action = () => nbCalls++;

			Assert.AreEqual (0, nbCalls);

			Assert.IsTrue(counter.IfZero (action));

			Assert.AreEqual (1, nbCalls);

			using (counter.Enter ())
			{
				Assert.IsFalse(counter.IfZero (action));

				Assert.AreEqual (1, nbCalls);

				using (counter.Enter())
				{
					Assert.IsFalse (counter.IfZero (action));

					Assert.AreEqual (1, nbCalls);
				}
			}

			Assert.AreEqual (1, nbCalls);

			Assert.IsTrue (counter.IfZero (action));

			Assert.AreEqual (2, nbCalls);
		}


	}


}
