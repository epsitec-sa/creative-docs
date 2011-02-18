//	Copyright © 2007-2008, OPaC bright ideas, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Drawing
{
	/// <summary>
	/// The <c>CustomThreadPool</c> class implements a specialized thread pool
	/// which is used by the image manager to execute asynchronous threads.
	/// </summary>
	internal sealed class CustomThreadPool : System.IDisposable
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CustomThreadPool"/> class.
		/// </summary>
		public CustomThreadPool()
		{
		}

		/// <summary>
		/// Defines the memory limit which specified when too much memory is used.
		/// </summary>
		/// <param name="limit">The limit.</param>
		public void DefineMemoryLimit(long limit)
		{
			this.memoryLimit = limit;
		}

		/// <summary>
		/// Queues a work item; it will be executed by one of the thread pool
		/// threads.
		/// </summary>
		/// <param name="callback">The callback.</param>
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
				if (this.threads.Count >= System.Environment.ProcessorCount + 1)
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
					lock (this.threads)
					{
						if (this.exitRequested)
						{
							break;
						}
					}

					if (this.semaphore.WaitOne (this.lifeTimeout, false))
					{
						bool doSomeWork = true;
						bool ignoreWork = false;

						while (doSomeWork)
						{
							System.Threading.Interlocked.Increment (ref this.busyThreadCount);

							if (ignoreWork == false)
							{
								try
								{
									this.ProcessWorkItem ();
								}
								finally
								{
									System.Threading.Interlocked.Decrement (ref this.busyThreadCount);
								}
							}

							if (this.UsesTooMuchMemory ())
							{
								if (isFirstThread)
								{
									if (gcExecuted)
									{
										gcExecuted = false;
									}
									else
									{
										//System.Diagnostics.Debug.WriteLine ("Low memory, GC collection.");
										System.GC.Collect ();
										gcExecuted = true;
									}
								}
								else
								{
									System.Threading.Thread.Sleep (100);
									ignoreWork = true;
								}
							}
							else
							{
								ignoreWork = false;
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

				if (this.UsesTooMuchMemory ())
				{
					break;
				}
			}
		}

		private bool UsesTooMuchMemory()
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

		private readonly object					exclusion	= new object ();
		private long							memoryLimit = 200*1024*1024L;
		private int								lifeTimeout = 20*1000;
		private int								busyThreadCount;
		private bool							exitRequested;
		readonly Queue<Callback>				workItems	= new Queue<Callback> ();
		readonly List<System.Threading.Thread>	threads		= new List<System.Threading.Thread> ();
		readonly System.Threading.Semaphore		semaphore	= new System.Threading.Semaphore (0, 0x7fffffff);
	}
}
