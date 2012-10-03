//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Data;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Factories;
using Epsitec.Cresus.Core.Library;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.UserManagement
{
	public class UserSession : System.IDisposable
	{
		public UserSession(UserManager manager, string sessionId)
		{
			this.userManager = manager;
			this.sessionId = sessionId;
		}


		public UserManager UserManager
		{
			get
			{
				return this.userManager;
			}
		}
		
		public string SessionId
		{
			get
			{
				return this.sessionId;
			}
		}


		#region IDisposable Members

		public void Dispose()
		{
			this.Dispose (true);
		}

		#endregion

		protected virtual void Dispose(bool disposing)
		{
		}


		private readonly UserManager userManager;
		private readonly string sessionId;
	}
}
