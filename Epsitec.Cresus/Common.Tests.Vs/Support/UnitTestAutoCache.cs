using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;

using Epsitec.Common.UnitTesting;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;

using System.Collections.Generic;

using System.Threading;


namespace Epsitec.Common.Tests.Vs.Support
{
	
	
	[TestClass]
	public class UnitTestAutoCache
	{


		[TestMethod]
		public void ConstructorArgumentCheck()
		{
			ExceptionAssert.Throw<ArgumentNullException>
			(
				() => new AutoCache<double, int> (null)
			);
		}


		[TestMethod]
		public void ConstructorTest()
		{
			using (new AutoCache<double, int> (d => (int) d))
			{
			}
		}


		[TestMethod]
		public void SimpleTest()
		{
			Func<double, int> function = (e) => (int) e;
			
			using (var cache = new AutoCache<double, int> (function))
			{
				for (double d = 0; d < 10; d += 0.1)
				{
					Assert.AreEqual (function (d), cache[d]);
				}
			}
		}


		[TestMethod]
		public void NbCallsTest()
		{
			int nbCall = 0;

			Func<double, int> function = (d) => { nbCall++; return (int) d; };

			using (var cache = new AutoCache<double, int> (function))
			{
				for (int i = 0; i < 10; i++)
				{
					for (double d = 0; d < 10; d += 0.1)
					{
						var x = cache[d];
					}
				}

				Assert.AreEqual (101, nbCall);

				cache.Clear ();

				for (int i = 0; i < 10; i++)
				{
					for (double d = 0; d < 10; d += 0.1)
					{
						var x = cache[d];
					}
				}
			}

			Assert.AreEqual (202, nbCall);
		}


		[TestMethod]
		public void DefaultValueTest()
		{
			int nbCall = 0;

			Func<double, int> function = (d) => { nbCall++; return (int) d; };
			
			using (var cache = new AutoCache<double, int> (function))
			{
				Assert.AreEqual (0, cache[0]);
				Assert.AreEqual (0, cache[0]);
			}

			Assert.AreEqual (1, nbCall);
		}


		[TestMethod]
		public void NullValueTest()
		{
			int nbCall = 0;

			Func<object, object> function = (o) => { nbCall++; return o; };

			using (var cache = new AutoCache<object, object> (function))
			{
				Assert.IsNull (cache[null]);
				Assert.IsNull (cache[null]);
			}

			Assert.AreEqual (1, nbCall);
		}


		[TestMethod]
		public void ThreadSafetytest()
		{
			int nbCall = 0;

			Func<object, object> function = (o) => { Interlocked.Increment (ref nbCall); Thread.Sleep (500); return o; };

			using (AutoCache<object, object> cache = new AutoCache<object, object> (function))
			{
				List<object> objects = new List<object> ();
				
				for (int i = 0; i < 10; i++)
				{
					objects.Add (new object ());
				}
				
				objects.Add (null);
				
				List<Thread> threads = new List<Thread> ();
				
				DateTime time = DateTime.Now;
				
				for (int i = 0; i < 100; i++)
				{
					Thread thread = new Thread (() =>
					{
						while (DateTime.Now - time < TimeSpan.FromSeconds (15))
						{
							object key = objects.GetRandomElement ();
							object result = cache[key];
							Assert.AreEqual (key, result);
						}
					});
					threads.Add (thread);
				}

				foreach (var thread in threads)
				{
					thread.Start ();
				}

				foreach (var thread in threads)
				{
					thread.Join ();
				}

				Assert.AreEqual (nbCall, objects.Count);
			}
		}


	}


}
