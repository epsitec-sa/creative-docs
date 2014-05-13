//	Copyright © 2011-2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Epsitec.Cresus.WebCore.Server.Core
{
	public sealed class CoreJob
	{
		public string Id
		{
			get 
			{ 
				return this.taskId;
			}
		}

		public string HtmlView
		{
			get
			{
				var desc = this.Id;

				if(this.Status == CoreJobStatus.Waiting)
				{
					desc = desc + "<br><a href='/proxy/jobs/cancel/"+ this.Id +"'>Annuler</a>";
				}
				if (this.Status == CoreJobStatus.Ended)
				{
					desc = desc + string.Format("<br>Durée: {0}",this.GetRunningTime());
				}
				if (this.Status == CoreJobStatus.Canceled)
				{
					desc = desc + string.Format ("<br><b>Annulée</b><br>Durée: {0}", this.GetRunningTime ());
				}

				desc = desc + "<br>" + this.Metadata;
				return desc;
			}
		}

		public string Title
		{
			get
			{
				return this.title + " (" + this.Status.Value + ")";
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

		public CoreJob(string taskId, string title)
		{
			var now = System.DateTime.Now;
			this.title = title;
			this.Status = CoreJobStatus.Started;
			this.taskId = taskId;
			this.createdAt = now;
		}

		public void Enqueue()
		{
			this.queuedAt = System.DateTime.Now;
			this.Status = CoreJobStatus.Waiting;
		}

		public void Start()
		{
			this.startedAt = System.DateTime.Now;
			this.Status = CoreJobStatus.Running;
		}

		public void Finish()
		{
			this.finisedAt = System.DateTime.Now;
			this.Status = CoreJobStatus.Ended;
		}

		public void Cancel()
		{
			this.finisedAt = System.DateTime.Now;
			this.Status = CoreJobStatus.Canceled;
		}

		public System.TimeSpan GetRunningTime()
		{
			return this.finisedAt.Subtract (this.startedAt);
		}

		private readonly string taskId;

		private System.DateTime createdAt;
		private System.DateTime queuedAt;
		private System.DateTime startedAt;
		private System.DateTime finisedAt;

		private string title;
		
	}

	public sealed class CoreJobStatus
	{
		public static readonly CoreJobStatus Started = new CoreJobStatus ("En attente");
		public static readonly CoreJobStatus Waiting = new CoreJobStatus ("En attente");
		public static readonly CoreJobStatus Running = new CoreJobStatus ("En cours");
		public static readonly CoreJobStatus Ended = new CoreJobStatus ("Terminée");
		public static readonly CoreJobStatus Canceled = new CoreJobStatus ("Annulée");

		private CoreJobStatus(string value)
		{
			Value = value;
		}

		public string Value
		{
			get;
			private set;
		}
	}
}
