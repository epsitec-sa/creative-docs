//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Support
{
	/// <summary>
	/// The <c>Tasklet</c> class provides support for delayed (and possibly
	/// asynchronous) execution.
	/// </summary>
	public class Tasklet
	{
		private Tasklet()
		{
			this.jobs = new List<TaskletJob> ();
		}

		
		public bool ContainsPendingJobs
		{
			get
			{
				return this.jobs.Count > 0;
			}
		}

		public bool IsExecutingJobs
		{
			get
			{
				return this.executeRecursionCount > 0;
			}
		}

		public string BatchName
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the current <c>Tasklet</c> (usually, the one which is currently
		/// executing).
		/// </summary>
		/// <value>The current <c>Tasklet</c> or <c>null</c>.</value>
		public static Tasklet				Current
		{
			get
			{
				return Tasklet.current;
			}
			private set
			{
				Tasklet.current = value;
			}
		}


		/// <summary>
		/// Executes all pending jobs.
		/// </summary>
		public void ExecuteAllJobs()
		{
			foreach (var job in this.GetExecutableJobs ())
			{
				var action = job.Action;
				var owner  = job.Owner;

				if ((owner == null) ||
					(owner.IsDisposed == false))
				{
					action ();
				}
			}
		}

		/// <summary>
		/// Gets the executable jobs and automatically makes the <c>Tasklet</c>
		/// active while iterating.
		/// </summary>
		/// <returns>Collection of executable jobs.</returns>
		public IEnumerable<TaskletJob> GetExecutableJobs()
		{
			var activeTasklet = Tasklet.Current;
			
			this.executeRecursionCount++;
			Tasklet.Current = this;
			
			while (this.ContainsPendingJobs)
			{
				yield return this.DequeueJob ();
			}
			
			Tasklet.Current = activeTasklet;
			this.executeRecursionCount--;
		}


		/// <summary>
		/// Queues a batch of <see cref="TaskletJob"/> jobs for immediate
		/// or deferred execution.
		/// </summary>
		/// <param name="name">The batch name.</param>
		/// <param name="jobs">The jobs to run.</param>
		/// <returns>The <c>Tasklet</c> where the jobs were queued.</returns>
		public static Tasklet QueueBatch(string name, params TaskletJob[] jobs)
		{
			return Tasklet.QueueBatch (name, (IEnumerable<TaskletJob>) jobs);
		}

		public static Tasklet QueueBatch(string name, IEnumerable<TaskletJob> jobs)
		{
			var  tasklet = Tasklet.Current;
			bool asyncMode;

			if (tasklet == null)
			{
				tasklet   = new Tasklet ();
				asyncMode = true;
			}
			else
			{
				asyncMode = ! tasklet.IsExecutingJobs;
			}

			tasklet.Enqueue (name, jobs, asyncMode);

			return tasklet;
		}


		private void Enqueue(string batchName, IEnumerable<TaskletJob> jobs, bool asyncMode)
		{
			Tasklet.ValidateJobRunModes (jobs);

			var pending    = new List<TaskletJob> ();
			var activeName = this.BatchName;

			if (asyncMode)
			{
				jobs.Where (x => x.IsBefore).ForEach (x => x.Action ());

				pending.Add (new TaskletJob (() => this.BatchName = batchName, TaskletRunMode.Sync));
				pending.AddRange (jobs.Where (x => x.IsAsync));
				pending.AddRange (jobs.Where (x => x.IsAfter));
				pending.Add (new TaskletJob (() => this.BatchName = activeName, TaskletRunMode.Sync));
				pending.AddRange (this.jobs);

				this.jobs = pending;
			}
			else
			{
				//	TaskletJobs are already in execution; we will therefore simply execute
				//	synchronously all jobs before returning:

				this.BatchName = batchName;
				jobs.Where (x => x.IsAsync).ForEach (x => x.Action ());
				jobs.Where (x => x.IsAfter).ForEach (x => x.Action ());
				this.BatchName = activeName;
			}
		}

		private TaskletJob DequeueJob()
		{
			var job = this.jobs[0];
			this.jobs.RemoveAt (0);
			return job;
		}

		private static void ValidateJobRunModes(IEnumerable<TaskletJob> jobs)
		{
			foreach (var job in jobs)
			{
				switch (job.RunMode)
				{
					case TaskletRunMode.Async:
					case TaskletRunMode.Before:
					case TaskletRunMode.BeforeAndAfter:
					case TaskletRunMode.After:
						break;

					default:
						throw new System.NotSupportedException (string.Format ("TaskletRunMode.{0} is not supported", job.RunMode));
				}
			}
		}


		[System.ThreadStatic]
		private static Tasklet					current;

		private List<TaskletJob>				jobs;
		private int								executeRecursionCount;
	}
}
