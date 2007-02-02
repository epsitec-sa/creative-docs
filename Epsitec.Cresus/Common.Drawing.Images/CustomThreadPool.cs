//	Copyright © 2007, OPaC bright ideas, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Drawing
{
	internal sealed class CustomThreadPool : System.IDisposable
	{
		public CustomThreadPool()
		{
		}

		public void DefineMemoryLimit(long limit)
		{
			this.memoryLimit = limit;
		}

		public void QueueWorkItem(Callback callback)
		{
			int countThreads = 0;
			int countWorkItems = 0;

			lock (this.exclusion)
			{
				this.workItems.Enqueue (callback);

				countThreads = this.threads.Count;
				countWorkItems = this.workItems.Count;
			}

			if (countThreads < countWorkItems)
			{
				this.AddThread ();
			}

			this.semaphore.Release ();
		}

		#region IDisposable Members

		public void Dispose()
		{
			System.Threading.Thread[] threads;

			lock (this.threads)
			{
				this.exitRequested = true;
				threads = this.threads.ToArray ();
			}

			if (threads.Length > 0)
			{
				this.semaphore.Release (threads.Length);

				for (int i = 0; i < threads.Length; i++)
				{
					threads[i].Join ();
				}
			}

			System.Diagnostics.Debug.Assert (this.threads.Count == 0);
		}

		#endregion

		private void AddThread()
		{
			System.Threading.Thread thread = null;

			lock (this.exclusion)
			{
				if (this.threads.Count >= 2*System.Environment.ProcessorCount)
				{
					return;
				}

				thread = new System.Threading.Thread (this.ProcessingLoop);
				thread.Priority = System.Threading.ThreadPriority.BelowNormal;
				thread.Name = string.Format ("CustomThreadPool Thread #{0}", this.threads.Count);
				this.threads.Add (thread);
			}

			if (thread != null)
			{
				thread.Start ();
			}
		}

		private void ProcessingLoop()
		{
			try
			{
				bool gcExecuted = false;
				bool isFirstThread = false;

				lock (this.threads)
				{
					if (this.threads[0] == System.Threading.Thread.CurrentThread)
					{
						isFirstThread = true;
					}
				}
				
				while (true)
				{
					if (this.exitRequested)
					{
						break;
					}

					if (this.semaphore.WaitOne (this.lifeTimeout, false))
					{

						bool doSomeWork = true;

						while (doSomeWork)
						{
							System.Threading.Interlocked.Increment (ref this.busyThreadCount);

							try
							{
								this.ProcessWorkItem ();
							}
							finally
							{
								System.Threading.Interlocked.Decrement (ref this.busyThreadCount);
							}

							if (this.UsesToMuchMemory ())
							{
								if (isFirstThread)
								{
									if (gcExecuted)
									{
										gcExecuted = false;
									}
									else
									{
										System.Diagnostics.Debug.WriteLine ("Low memory, GC collection.");
										System.GC.Collect ();
										gcExecuted = true;
									}
								}
								else
								{
									System.Diagnostics.Debug.WriteLine ("Low memory, wait.");
									System.Threading.Thread.Sleep (100);
								}
							}

							lock (this.exclusion)
							{
								doSomeWork = this.workItems.Count > 0;
							}
						}
					}
					else
					{
						lock (this.threads)
						{
							if (!isFirstThread)
							{
								this.threads.Remove (System.Threading.Thread.CurrentThread);
								break;
							}
						}
					}
				}
			}
			catch (System.Threading.ThreadInterruptedException)
			{
				//	Exit gracefully...
			}
			finally
			{
				lock (this.threads)
				{
					this.threads.Remove (System.Threading.Thread.CurrentThread);
				}
			}
		}

		private void ProcessWorkItem()
		{
			while (true)
			{
				Callback callback = null;

				lock (this.exclusion)
				{
					if (this.workItems.Count > 0)
					{
						callback = this.workItems.Dequeue ();
					}
				}

				if (callback == null)
				{
					break;
				}

				callback ();

				if (this.UsesToMuchMemory ())
				{
					break;
				}
			}
		}

		private bool UsesToMuchMemory()
		{
			if (System.Diagnostics.Process.GetCurrentProcess ().VirtualMemorySize64 > this.memoryLimit)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		private long memoryLimit = 200*1024*1024L;
		private int lifeTimeout = 20*1000;
		private int busyThreadCount;
		private bool exitRequested;
		private readonly object exclusion = new object ();
		private Queue<Callback> workItems = new Queue<Callback> ();
		private List<System.Threading.Thread> threads = new List<System.Threading.Thread> ();
		private System.Threading.Semaphore semaphore = new System.Threading.Semaphore (0, 0x7fffffff);
	}
}
