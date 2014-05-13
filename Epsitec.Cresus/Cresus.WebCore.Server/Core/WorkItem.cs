//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.IO;
using Epsitec.Common.Support;
using Epsitec.Common.Types.Collections.Concurrent;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Business.UserManagement;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Epsitec.Cresus.Core.Library;

namespace Epsitec.Cresus.WebCore.Server.Core
{
	public abstract class WorkItem
	{
		protected WorkItem(string workItemId, string userName, string sessionId)
		{
			this.enqueueDateTime = System.DateTime.Now;
			
			this.workItemId = workItemId;
			this.userName   = userName;
			this.sessionId  = sessionId;
		}

		
		public string							WorkItemId
		{
			get
			{
				return this.workItemId;
			}
		}

		public string							UserName
		{
			get
			{
				return this.userName;
			}
		}

		public System.DateTime					EnqueueDateTime
		{
			get
			{
				return this.enqueueDateTime;
			}
		}

		public bool								Cancelled
		{
			get;
			private set;
		}

		
		public abstract void Execute(CoreWorker worker);

		public void Cancel()
		{
			this.Cancelled = true;
		}

		
		public override string ToString()
		{
			return string.Format ("{0}: {1} queued by {2} on session {3}", this.enqueueDateTime, this.workItemId, this.userName, this.sessionId);
		}

		
		protected readonly System.DateTime		enqueueDateTime;
		protected readonly string				workItemId;
		protected readonly string				userName;
		protected readonly string				sessionId;
	}
}

