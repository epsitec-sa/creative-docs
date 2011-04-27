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
				() => DisposableWrapper.CreateDisposable (null)
			);
		}


		[TestMethod]
		public void SimpleTest()
		{
			int nbFinalizerCalls = 0;

			Action finalizer = () => nbFinalizerCalls++;

			Assert.AreEqual (0, nbFinalizerCalls);

			IDisposable disposableWrapper = DisposableWrapper.CreateDisposable (finalizer);

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

			IDisposable disposableWrapper = DisposableWrapper.CreateDisposable (finalizer);

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
			IDisposable disposableWrapper = DisposableWrapper.CreateDisposable (() => { });

			ExceptionAssert.Throw<ArgumentNullException>
			(
				() => DisposableWrapper.CombineDisposables (null)
			);

			ExceptionAssert.Throw<ArgumentException>
			(
				() => DisposableWrapper.CombineDisposables (null, disposableWrapper)
			);

			ExceptionAssert.Throw<ArgumentException>
			(
				() => DisposableWrapper.CombineDisposables (disposableWrapper, null)
			);
		}


		[TestMethod]
		public void CombineSimpleTest1()
		{
			int nbFinalizerCalls1 = 0;
			int nbFinalizerCalls2 = 0;

			Action finalizer1 = () => nbFinalizerCalls1++;
			Action finalizer2 = () => nbFinalizerCalls2++;

			IDisposable disposableWrapper1 = DisposableWrapper.CreateDisposable (finalizer1);
			IDisposable disposableWrapper2 = DisposableWrapper.CreateDisposable (finalizer2);

			Assert.AreEqual (0, nbFinalizerCalls1);
			Assert.AreEqual (0, nbFinalizerCalls2);

			IDisposable disposableWrapper3 = DisposableWrapper.CombineDisposables (disposableWrapper1, disposableWrapper2);

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

			IDisposable disposableWrapper1 = DisposableWrapper.CreateDisposable (finalizer1);
			IDisposable disposableWrapper2 = DisposableWrapper.CreateDisposable (finalizer2);

			Assert.AreEqual (0, nbFinalizerCalls1);
			Assert.AreEqual (0, nbFinalizerCalls2);

			IDisposable disposableWrapper3 = DisposableWrapper.CombineDisposables (disposableWrapper1, disposableWrapper2);
			
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


		[TestMethod]
		public void CombineExceptionTest1()
		{
			int nbFinalizerCalls1 = 0;
			int nbFinalizerCalls2 = 0;

			var exception = new System.InvalidTimeZoneException ();

			Action finalizer1 = () =>
			{
				nbFinalizerCalls1++;
				throw exception;
			};

			Action finalizer2 = () => nbFinalizerCalls2++;

			IDisposable disposableWrapper1 = DisposableWrapper.CreateDisposable (finalizer1);
			IDisposable disposableWrapper2 = DisposableWrapper.CreateDisposable (finalizer2);

			Assert.AreEqual (0, nbFinalizerCalls1);
			Assert.AreEqual (0, nbFinalizerCalls2);

			IDisposable disposableWrapper3 = DisposableWrapper.CombineDisposables (disposableWrapper1, disposableWrapper2);

			Assert.AreEqual (0, nbFinalizerCalls1);
			Assert.AreEqual (0, nbFinalizerCalls2);

			System.Exception thrown = null;

			try
			{
				disposableWrapper3.Dispose ();
			}
			catch (System.InvalidTimeZoneException e)
			{
				thrown = e;
			}

			Assert.IsNotNull (thrown);
			Assert.AreSame (exception, thrown);

			Assert.AreEqual (1, nbFinalizerCalls1);
			Assert.AreEqual (1, nbFinalizerCalls2);
		}


		[TestMethod]
		public void CombineExceptionTest2()
		{
			int nbFinalizerCalls1 = 0;
			int nbFinalizerCalls2 = 0;
			int nbFinalizerCalls3 = 0;

			var exception1 = new System.InvalidTimeZoneException ();
			var exception2 = new System.InvalidTimeZoneException ();

			Action finalizer1 = () =>
			{
				nbFinalizerCalls1++;
				throw exception1;
			};

			Action finalizer2 = () =>
			{
				nbFinalizerCalls2++;
				throw exception2;
			};

			Action finalizer3 = () =>
			{
				nbFinalizerCalls3++;
			};

			IDisposable disposableWrapper1 = DisposableWrapper.CreateDisposable (finalizer1);
			IDisposable disposableWrapper2 = DisposableWrapper.CreateDisposable (finalizer2);
			IDisposable disposableWrapper3 = DisposableWrapper.CreateDisposable (finalizer3);

			Assert.AreEqual (0, nbFinalizerCalls1);
			Assert.AreEqual (0, nbFinalizerCalls2);
			Assert.AreEqual (0, nbFinalizerCalls3);

			IDisposable disposableWrapper4 = DisposableWrapper.CombineDisposables (disposableWrapper1, disposableWrapper2, disposableWrapper3);

			Assert.AreEqual (0, nbFinalizerCalls1);
			Assert.AreEqual (0, nbFinalizerCalls2);
			Assert.AreEqual (0, nbFinalizerCalls3);

			GroupedException thrown = null;

			try
			{
				disposableWrapper4.Dispose ();
			}
			catch (GroupedException e)
			{
				thrown = e;
			}

			Assert.IsNotNull (thrown);
			Assert.AreSame (exception1, thrown.Exceptions[0]);
			Assert.AreSame (exception2, thrown.Exceptions[1]);

			Assert.AreEqual (1, nbFinalizerCalls1);
			Assert.AreEqual (1, nbFinalizerCalls2);
			Assert.AreEqual (1, nbFinalizerCalls3);
		}

	}


}
