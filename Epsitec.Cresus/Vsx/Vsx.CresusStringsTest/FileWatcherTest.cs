using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime.Serialization;
using System.Threading;
using Epsitec.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Epsitec
{
	[TestClass]
	public class FileWatcherTest
	{
		[TestMethod]
		public void SingleFile()
		{
			Trace.WriteLine (string.Format ("[{0}] Started...", Thread.CurrentThread.ManagedThreadId));

			var folder = Path.GetFullPath (TestFolder);
			using (new FileMonitor (folder, TestFileName1).Watch ().Subscribe (e => Trace.WriteLine (string.Format ("[{0}] {1}", Thread.CurrentThread.ManagedThreadId, e)), TraceError, TraceCompleted))
			{
				for (int i = 0; i < 10; ++i)
				{
					using (var stream = File.CreateText (TestFilePath1))
					{
						stream.WriteLine (Guid.NewGuid ().ToString ());
					}
				}
				File.Delete (TestFilePath1);

				Thread.Sleep (1000);
			}
			Trace.WriteLine (string.Format ("[{0}] Finished", Thread.CurrentThread.ManagedThreadId));
		}
		[TestMethod]
		public void MultipleFiles()
		{
			Trace.WriteLine (string.Format ("[{0}] Started...", Thread.CurrentThread.ManagedThreadId));

			var folder = Path.GetFullPath (TestFolder);
			var allEvents = Observable.Merge (new FileMonitor (folder, TestFileName1).Watch (), new FileMonitor (folder, TestFileName2).Watch ());

			using (allEvents.Subscribe (e => Trace.WriteLine (string.Format ("[{0}] {1}", Thread.CurrentThread.ManagedThreadId, e)), TraceError, TraceCompleted))
			{
				for (int i = 0; i < 10; ++i)
				{
					using (var stream1 = File.CreateText (TestFilePath1))
					{
						stream1.WriteLine (Guid.NewGuid ().ToString ());
					}
					using (var stream2 = File.CreateText (TestFilePath2))
					{
						stream2.WriteLine (Guid.NewGuid ().ToString ());
					}
				}
				File.Delete (TestFilePath1);
				File.Delete (TestFilePath2);

				Thread.Sleep (1000);
			}
			Trace.WriteLine (string.Format ("[{0}] Finished", Thread.CurrentThread.ManagedThreadId));
		}

		private static void Dump()
		{
		}

		private static void TraceCompleted()
		{
			Trace.WriteLine ("Completed");
		}

		private static void TraceError(Exception ex)
		{
			Trace.WriteLine (string.Format("{0} : {1}", ex.GetType().Name, ex.Message));
		}

		private static void TraceArgs(FileSystemNotification e)
		{
			Trace.WriteLine (string.Format("[{0}] {1}", Thread.CurrentThread.ManagedThreadId, e));
		}

		private const string TestFolder = ".\\";
		private const string TestFileName1 = "Test1.tmp";
		private const string TestFileName2 = "Test2.tmp";
		private const string TestFilePath1 = TestFolder + TestFileName1;
		private const string TestFilePath2 = TestFolder + TestFileName2;
	}
}
