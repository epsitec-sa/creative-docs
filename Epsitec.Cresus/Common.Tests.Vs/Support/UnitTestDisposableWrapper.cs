using Epsitec.Common.Support;

using Epsitec.Common.UnitTesting;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;


namespace Epsitec.Common.Tests.Vs.Support
{


	[TestClass]
	public class UnitTestDisposableWrapper
	{


		[TestMethod]
		public void GetArgumentCheck()
		{
			ExceptionAssert.Throw<ArgumentNullException>
			(
				() => DisposableWrapper.Get (null)
			);
		}


		[TestMethod]
		public void SimpleTest()
		{
			int nbFinalizerCalls = 0;

			Action finalizer = () => nbFinalizerCalls++;

			Assert.AreEqual (0, nbFinalizerCalls);

			IDisposable disposableWrapper = DisposableWrapper.Get (finalizer);

			Assert.IsNotNull (disposableWrapper);

			Assert.AreEqual (0, nbFinalizerCalls);

			disposableWrapper.Dispose ();

			Assert.AreEqual (1, nbFinalizerCalls);

			disposableWrapper.Dispose ();

			Assert.AreEqual (1, nbFinalizerCalls);

			GC.WaitForPendingFinalizers ();
		}


		[TestMethod]
		public void NoCallToDisposeTest()
		{
			int nbFinalizerCalls = 0;

			Action finalizer = () => nbFinalizerCalls++;

			Assert.AreEqual (0, nbFinalizerCalls);

			IDisposable disposableWrapper = DisposableWrapper.Get (finalizer);

			Assert.IsNotNull (disposableWrapper);

			Assert.AreEqual (0, nbFinalizerCalls);

			GC.WaitForPendingFinalizers ();

			// NOTE Here an exception has probably been thrown somewhere by the garbage collector. I
			// believe that the dev express unit test runner does handle it so it is normal that it
			// is normal that it is not thrown here.
			// Marc

			Assert.AreEqual (0, nbFinalizerCalls);		
		}


		[TestMethod]
		public void CombineArgumentCheck()
		{
			IDisposable disposableWrapper = DisposableWrapper.Get (() => { });

			ExceptionAssert.Throw<ArgumentNullException>
			(
				() => DisposableWrapper.Combine (null, disposableWrapper)
			);

			ExceptionAssert.Throw<ArgumentNullException>
			(
				() => DisposableWrapper.Combine (disposableWrapper, null)
			);
		}


		[TestMethod]
		public void CombineSimpleTest1()
		{
			int nbFinalizerCalls1 = 0;
			int nbFinalizerCalls2 = 0;

			Action finalizer1 = () => nbFinalizerCalls1++;
			Action finalizer2 = () => nbFinalizerCalls2++;

			IDisposable disposableWrapper1 = DisposableWrapper.Get (finalizer1);
			IDisposable disposableWrapper2 = DisposableWrapper.Get (finalizer2);

			Assert.AreEqual (0, nbFinalizerCalls1);
			Assert.AreEqual (0, nbFinalizerCalls2);

			IDisposable disposableWrapper3 = DisposableWrapper.Combine (disposableWrapper1, disposableWrapper2);

			Assert.AreEqual (0, nbFinalizerCalls1);
			Assert.AreEqual (0, nbFinalizerCalls2);

			disposableWrapper3.Dispose ();

			Assert.AreEqual (1, nbFinalizerCalls1);
			Assert.AreEqual (1, nbFinalizerCalls2);

			disposableWrapper3.Dispose ();

			Assert.AreEqual (1, nbFinalizerCalls1);
			Assert.AreEqual (1, nbFinalizerCalls2);

			GC.WaitForPendingFinalizers ();
		}


		[TestMethod]
		public void CombineSimpleTest2()
		{
			int nbFinalizerCalls1 = 0;
			int nbFinalizerCalls2 = 0;

			Action finalizer1 = () => nbFinalizerCalls1++;
			Action finalizer2 = () => nbFinalizerCalls2++;

			IDisposable disposableWrapper1 = DisposableWrapper.Get (finalizer1);
			IDisposable disposableWrapper2 = DisposableWrapper.Get (finalizer2);

			Assert.AreEqual (0, nbFinalizerCalls1);
			Assert.AreEqual (0, nbFinalizerCalls2);

			IDisposable disposableWrapper3 = DisposableWrapper.Combine (disposableWrapper1, disposableWrapper2);
			
			Assert.AreEqual (0, nbFinalizerCalls1);
			Assert.AreEqual (0, nbFinalizerCalls2);

			disposableWrapper1.Dispose ();

			Assert.AreEqual (1, nbFinalizerCalls1);
			Assert.AreEqual (0, nbFinalizerCalls2);

			disposableWrapper3.Dispose ();

			Assert.AreEqual (1, nbFinalizerCalls1);
			Assert.AreEqual (1, nbFinalizerCalls2);

			disposableWrapper3.Dispose ();

			Assert.AreEqual (1, nbFinalizerCalls1);
			Assert.AreEqual (1, nbFinalizerCalls2);

			GC.WaitForPendingFinalizers ();
		}


	}


}
