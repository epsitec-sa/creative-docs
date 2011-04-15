using Epsitec.Common.Support;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;


namespace Epsitec.Common.Tests.Vs.Support
{


	[TestClass]
	public sealed class UnitTestInterlockedSafeCounter
	{
		

		
		[TestMethod]
		public void SimpleTest()
		{
			InterlockedSafeCounter counter = new InterlockedSafeCounter ();

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
			InterlockedSafeCounter counter = new InterlockedSafeCounter ();

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


	}


}
