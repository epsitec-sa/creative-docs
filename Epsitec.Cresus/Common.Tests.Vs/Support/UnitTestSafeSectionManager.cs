using Epsitec.Common.Support;
using Epsitec.Common.UnitTesting;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;

using System.Collections.Generic;

using System.Diagnostics;

using System.Linq;

using System.Threading;


namespace Epsitec.Common.Tests.Vs.Support
{
	
	
	[TestClass]
	public class UnitTestSafeSectionManager
	{


		[TestMethod]
		public void CreateAfterDispose()
		{
			var safeSectionManager = new SafeSectionManager ();

			safeSectionManager.Dispose ();

			Assert.IsNull (safeSectionManager.TryCreate ());

			ExceptionAssert.Throw<ObjectDisposedException>
			(
				() => safeSectionManager.Create ()
			);
		}


		[TestMethod]
		public void ConcurentCreations()
		{
			int nbThreads = 10;
			var startTime = DateTime.Now;

			var times = new List<List<DateTime>> ();
			var threads = new List<Thread> ();	

			using (var safeSectionManager = new SafeSectionManager ())
			{
				Action<int> threadAction = i =>
				{
					using (safeSectionManager.Create ())
					{
						while (DateTime.Now - startTime < TimeSpan.FromSeconds (1))
						{
							times[i].Add (DateTime.Now);

							Thread.Sleep (100);
						}
					}
				};

				for (int i = 0; i < nbThreads; i++)
				{
					var capturedI = i;

					threads.Add(new Thread (() => threadAction (capturedI)));
					times.Add (new List<DateTime> ());
				}

				foreach (var thread in threads)
				{
					thread.Start ();
				}

				foreach (var thread in threads)
				{
					thread.Join ();
				}
			}

			for (int i = 0; i < nbThreads; i++)
			{
				var myTimes = times[i];
				var nextTimes = times[(i + 1) % nbThreads];

				Assert.IsTrue (myTimes.Any (mt => nextTimes.Any (ot => ot > mt) && nextTimes.Any (ot => ot < mt)));
			}
		}


		[TestMethod]
		public void WaitForDisposeTest()
		{
			using (var safeSectionManager = new SafeSectionManager ())
			{
				bool done = false;

				var t1 = new Thread (() =>
				{
					using (safeSectionManager.Create ())
					{
						Thread.Sleep (200);

						done = true;
					}

					Thread.Sleep (100);

					Assert.IsNull (safeSectionManager.TryCreate ());
				});

				var t2 = new Thread (() =>
				{
					Thread.Sleep (100);

					Assert.IsFalse (done);

					var watch = Stopwatch.StartNew ();

					safeSectionManager.Dispose ();

					watch.Stop ();

					Assert.IsTrue (watch.Elapsed >= TimeSpan.FromMilliseconds (100));

					Assert.IsTrue (done);
				});

				t1.Start ();
				t2.Start ();

				t1.Join ();
				t2.Join ();
			}
		}


	}


}
