//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Support
{
	/// <summary>
	/// The <c>TaskletJob</c> associates an action with a run mode.
	/// </summary>
	public sealed class TaskletJob
	{
		public TaskletJob(System.Action action, TaskletRunMode runMode)
		{
			this.action = action;
			this.runMode = runMode;
		}

		
		public TaskletRunMode RunMode
		{
			get
			{
				return this.runMode;
			}
		}

		public bool IsBefore
		{
			get
			{
				return this.RunMode == TaskletRunMode.Before || this.RunMode == TaskletRunMode.BeforeAndAfter;
			}
		}

		public bool IsAsync
		{
			get
			{
				return this.RunMode == TaskletRunMode.Async;
			}
		}

		public bool IsAfter
		{
			get
			{
				return this.RunMode == TaskletRunMode.After || this.RunMode == TaskletRunMode.BeforeAndAfter;
			}
		}

		public System.Action Action
		{
			get
			{
				return this.action;
			}
		}

		
		private readonly System.Action action;
		private readonly TaskletRunMode runMode;
	}
}
