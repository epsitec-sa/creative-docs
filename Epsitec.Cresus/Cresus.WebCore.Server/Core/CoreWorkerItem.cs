//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.IO;
using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;
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
	public abstract class CoreWorkItem
	{
		protected CoreWorkItem(string workItemId, string username, string sessionId, dynamic bag)
		{
			this.enqueueDateTime = System.DateTime.Now;
			this.workItemId      = workItemId;
			this.username        = username;
			this.sessionId       = sessionId;
			this.bag             = bag;
		}


		public bool							Cancelled
		{
			get;
			private set;
		}

		public string						WorkItemId
		{
			get
			{
				return this.workItemId;
			}
		}

		public string						UserName
		{
			get
			{
				return this.username;
			}
		}

		public string						SessionId
		{
			get
			{
				return this.sessionId;
			}
		}

		public System.DateTime				EnqueueDateTime
		{
			get
			{
				return this.enqueueDateTime;
			}
		}

		public dynamic						Bag
		{
			get
			{
				return this.bag;
			}
		}


		public void Cancel()
		{
			this.Cancelled = true;
		}

		public abstract void Execute(CoreWorker worker);

		public override string ToString()
		{
			return string.Format ("{0}: {1} queued by {2} on session {3}", this.enqueueDateTime, this.workItemId, this.username, this.sessionId);
		}

		
		private readonly System.DateTime	enqueueDateTime;
		private readonly string				workItemId;
		private readonly string				username;
		private readonly string				sessionId;
		private readonly dynamic			bag;
	}

}
