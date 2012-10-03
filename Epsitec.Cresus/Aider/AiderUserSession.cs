//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business.UserManagement;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider
{
	public sealed class AiderUserSession : UserSession
	{
		public AiderUserSession(UserManager manager, string sessionId)
			: base (manager, sessionId)
		{
		}


		public new AiderUserManager UserManager
		{
			get
			{
				return base.UserManager as AiderUserManager;
			}
		}

		
		public AiderUserScopeEntity GetActiveScope()
		{
			if ((this.activeScope == null) &&
				(this.UserManager.AuthenticatedUser != null))
			{
				this.activeScope = this.ResolveActiveScope ();
			}

			return this.activeScope;
		}

		
		private AiderUserScopeEntity ResolveActiveScope()
		{
			var user = this.UserManager.AuthenticatedUser;

			//	TODO: resolve active scope

			return null;
		}


		private AiderUserScopeEntity activeScope;
	}
}
