using Epsitec.Common.Support;

using Epsitec.Common.UnitTesting;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;

using System.Collections.Generic;

using System.Diagnostics;

using System.Threading;


namespace Epsitec.Common.Tests.Vs.Support
{


	[TestClass]
	public class UnitTestWorkerThread
	{


		[TestMethod]
		public void UseAfterDispose()
		{
			var workerThread = new WorkerThread ();

			workerThread.Dispose ();

			ExceptionAssert.Throw<InvalidOperationException>
			(
				() => workerThread.ExecuteAsynchronously (() => { })
			);

			ExceptionAssert.Throw<InvalidOperationException>
			(
				() => workerThread.ExecuteSynchronously (() => { })
			);
		}


		[TestMethod]
		public void ExecuteSynchronously()
		{
			using (var workerThread = new WorkerThread())
			{
				bool done = false;

				workerThread.ExecuteSynchronously (() =>
				{
					Thread.Sleep (100);

					done = true;
				});

				Assert.IsTrue (done);
			}
		}


		[TestMethod]
		public void ExecuteAsynchronously()
		{
			using (var workerThread = new WorkerThread ())
			{
				bool done = false;

				workerThread.ExecuteAsynchronously (() =>
				{
					Thread.Sleep (100);

					done = true;
				});

				Assert.IsFalse (done);

				Thread.Sleep (100);

				Assert.IsTrue (done);
			}
		}


		[TestMethod]
		public void ExecuteSequentially()
		{
			var times = new Dictionary<int, DateTime> ();

			using (var workerThread = new WorkerThread ())
			{
				for (int i = 0; i < 10; i++)
				{
					var capturedI = i;

					workerThread.ExecuteAsynchronously (() =>
					{
						Thread.Sleep (100);

						times[capturedI] = DateTime.Now;
					});
				}
			}

			for (int i = 0; i < 9; i++)
			{
				Assert.IsTrue (times[i + 1] - times[i] >= TimeSpan.FromMilliseconds (100));
			}
		}


		[TestMethod]
		public void WaitForDispose()
		{
			using (var workerThread = new WorkerThread ())
			{
				bool done1 = false;
				bool done2 = false;

				var t1 = new Thread (() =>
				{
					workerThread.ExecuteAsynchronously (() =>
					{
						Thread.Sleep (200);

						done1 = true;
					});
					
					workerThread.ExecuteSynchronously (() =>
					{
						Thread.Sleep (200);

						done2 = true;
					});
				});

				var t2 = new Thread (() =>
				{
					Thread.Sleep (100);

					Assert.IsFalse (done1);
					Assert.IsFalse (done2);

					var watch = Stopwatch.StartNew ();

					workerThread.Dispose ();

					watch.Stop ();

					Assert.IsTrue (watch.Elapsed >= TimeSpan.FromMilliseconds (300));

					Assert.IsTrue (done1);
					Assert.IsTrue (done2);
				});

				t1.Start ();
				t2.Start ();

				t1.Join ();
				t2.Join ();
			}
		}


	}


}
