//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Cresus.Core.Library;
using Nancy.Helpers;
using Epsitec.Common.Types;

namespace Epsitec.Cresus.WebCore.Server.Core
{
	public sealed class CoreJob
	{
		public CoreJob(string username, string sessionId, string taskId, string title, EntityBagManager bag, StatusBarManager bar, bool enableCancelation)
		{
			var now = System.DateTime.Now;
			
			this.Username  = username;
			this.SessionId = sessionId;
			this.rawTitle  = title;
			this.Status    = CoreJobStatus.Ordered;
			this.taskId    = taskId;
			this.createdAt = now;
			this.entityBag = bag;
			this.statusBar = bar;
			
			this.cancellationEnabled = enableCancelation;
		}

		
		public string Id
		{
			get 
			{ 
				return this.taskId;
			}
		}

		public FormattedText SummaryView
		{
			get
			{
				var desc = "";

				if ((this.Status == CoreJobStatus.Ordered) || (this.Status == CoreJobStatus.Waiting))
				{
					desc = desc + "<br><input type='button' onclick='Epsitec.Cresus.Core.app.cancelJob(\"" + this.Id + "\");' value='Annuler' />";
				}
				if (this.Status == CoreJobStatus.Ended)
				{
					desc = desc + string.Format("<br>Durée: {0}",this.GetRunningTime());
				}
				if (this.Status == CoreJobStatus.Cancelled)
				{
					desc = desc + string.Format ("<br><b>Annulée</b><br>Durée: {0}", this.GetRunningTime ());
				}

				desc = desc + "<br>" + this.Metadata;

				return new FormattedText (desc);
			}
		}

		public string Title
		{
			get
			{
				return this.rawTitle + " (" + this.Status.Value + ")";
			}
		}

		public string StatusView
		{
			get
			{
				var desc = "[" + this.Username + "] (" + this.Status.Value + ")" + this.Metadata;
				return desc;
			}
		}

		public CoreJobStatus Status
		{
			get;
			set;
		}

		public string Metadata
		{
			get;
			set;
		}

		public string Username
		{
			get;
			set;
		}

		public string SessionId
		{
			get;
			set;
		}

		public System.DateTime QueuedAt
		{
			get
			{
				return this.queuedAt;
			}
		}

		public void Enqueue()
		{
			this.queuedAt = System.DateTime.Now;
			this.Status = CoreJobStatus.Waiting;
			this.AddTaskStatus ();
			if (this.cancellationEnabled)
			{
				this.AddTaskStatusInBag ();
			}
			
		}

		public void Start()
		{		
			this.startedAt = System.DateTime.Now;
			this.Status = CoreJobStatus.Running;
			if (this.cancellationEnabled)
			{
				this.RemoveTaskInStatusInBag ();
			}			
			this.UpdateTaskStatus ();
		}

		public void Start(string metaData)
		{
			this.startedAt = System.DateTime.Now;
			this.Status = CoreJobStatus.Running;
			this.Metadata = metaData;
			this.UpdateTaskStatus ();
			this.UpdateTaskStatusInBag ();
		}

		public void Progress(string metaData)
		{
			this.startedAt = System.DateTime.Now;
			this.Status = CoreJobStatus.Running;
			this.Metadata = metaData;
			this.UpdateTaskStatus ();
			this.UpdateTaskStatusInBag ();
		}

		public void Finish()
		{
			this.finishedAt = System.DateTime.Now;
			this.Status = CoreJobStatus.Ended;
			this.RemoveTaskStatus ();
		}

		public void Finish(string metaData)
		{
			this.finishedAt = System.DateTime.Now;
			this.Status = CoreJobStatus.Ended;
			this.Metadata = metaData;
			this.UpdateTaskStatusInBag ();
			this.RemoveTaskStatus ();
		}

		public void Cancel()
		{
			this.finishedAt = System.DateTime.Now;
			this.Status = CoreJobStatus.Cancelled;
			this.RemoveTaskStatus ();
			this.RemoveTaskInStatusInBag ();
		}

		public System.TimeSpan GetRunningTime()
		{
			return this.finishedAt.Subtract (this.startedAt);
		}

		private void AddTaskStatusInBag()
		{
			this.entityBag.AddToBag (this.Username, this.Title, this.SummaryView, this.Id, When.Now);
		}

		private void AddTaskStatus()
		{
			this.statusBar.AddToBar ("text", this.StatusView, "", this.Id, When.Now);
		}


		private void UpdateTaskStatusInBag()
		{
			this.entityBag.RemoveFromBag (this.Username, this.Id, When.Now);
			this.entityBag.AddToBag (this.Username, this.Title, this.SummaryView, this.Id, When.Now);
		}

		private void UpdateTaskStatus()
		{
			this.statusBar.RemoveFromBar (this.Id, When.Now);
			this.statusBar.AddToBar ("text", this.StatusView, "", this.Id, When.Now);
		}

		private void RemoveTaskInStatusInBag()
		{
			this.entityBag.RemoveFromBag (this.Username, this.Id, When.Now);
		}
		private void RemoveTaskStatus()
		{
			this.statusBar.RemoveFromBar (this.Id, When.Now);
		}

		private readonly string taskId;

		private System.DateTime createdAt;
		private System.DateTime queuedAt;
		private System.DateTime startedAt;
		private System.DateTime finishedAt;

		private string rawTitle;
		private StatusBarManager statusBar;
		private EntityBagManager entityBag;

		private bool cancellationEnabled;
	}
}