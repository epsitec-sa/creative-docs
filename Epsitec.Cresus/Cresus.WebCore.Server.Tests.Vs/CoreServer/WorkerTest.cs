using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.WebCore.Server.CoreServer;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;

using System.Threading;


namespace Epsitec.Cresus.WebCore.Server.Tests.Vs.CoreServer
{


	[TestClass]
	public sealed class WorkerTest
	{


		[TestMethod]
		public void SimpleTest()
		{
			using (var worker = new Worker ())
			{
				var t = new DateTime[5];
				var b = new bool[5];

				for (int i = 0; i < 5; i++)
				{
					int index = i;

					Action action = () =>
					{
						Thread.Sleep (250);
						t[index] = DateTime.Now;
						b[index] = true;
					};

					worker.Execute (action);
				}

				foreach (var boolean in b)
				{
					Assert.IsTrue (boolean);
				}

				for (int i = 0; i < 4; i++)
				{
					Assert.IsTrue (t[i + 1] - t[i] >= TimeSpan.FromMilliseconds (250));
				}
			}
		}


		[TestMethod]
		public void MultiThreadTest()
		{
			var threads = new Thread[5];

			var t = new DateTime[5][];
			var b = new bool[5][];
			var a = new int?[5];

			for (int i = 0; i < 5; i++)
			{
				t[i] = new DateTime[5];
				b[i] = new bool[5];
			}

			using (var worker = new Worker())
			{
				for (int i = 0; i < 5; i++)
				{
					int capturedI = i;

					ThreadStart threadAction = () =>
					{
						for (int j = 0; j < 5; j++)
						{
							int capturedJ = j;

							Action action = () =>
							{
								Thread.Sleep (250);
								t[capturedI][capturedJ] = DateTime.Now;
								b[capturedI][capturedJ] = true;
							};

							try
							{
								worker.Execute (action);
							}
							catch (OperationCanceledException)
							{
								a[capturedI] = capturedJ;

								break;
							}
						}
					};

					threads[i] = new Thread (threadAction);
					threads[i].Start ();
				}

				Thread.Sleep (2500);
			}

			for (int i = 0; i < 5; i++)
			{
				threads[i].Join ();
			}

			for (int i = 0; i < 5; i++)
			{
				for (int j = 0; j < 5; j++)
				{
					Assert.IsTrue (b[i][j] || a[i] <= j);
				}
			}

			for (int i = 0; i < 5; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					if (b[i][j] && b[i][j + 1])
					{
						Assert.IsTrue (t[i][j + 1] - t[i][j] >= TimeSpan.FromMilliseconds (250));
					}
				}
			}
		}


		[TestMethod]
		public void DisposedTest()
		{
			var worker = new Worker ();

			worker.Dispose ();

			bool b = false;

			ExceptionAssert.Throw<OperationCanceledException>
			(
				() => worker.Execute (() => b = true)
			);

			Assert.IsFalse (b);
		}


	}


}
