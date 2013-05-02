//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Business.UserManagement;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Data;

namespace Epsitec.Aider.Override
{
	public sealed class AiderUserManager : UserManager
	{
		public AiderUserManager(CoreData data, bool enableReload)
			: base (data, enableReload)
		{
		}


		public new AiderUserEntity				AuthenticatedUser
		{
			get
			{
				return base.AuthenticatedUser as AiderUserEntity;
			}
		}

		public new AiderUserSession				ActiveSession
		{
			get
			{
				return base.ActiveSession as AiderUserSession;
			}
		}

		public static new AiderUserManager		Current
		{
			get
			{
				return UserManager.Current as AiderUserManager;
			}
		}


		public override void NotifySusccessfulLogin(SoftwareUserEntity user)
		{
			this.UpdateUser (user.Code, u => u.LastLoginDate = System.DateTime.UtcNow);
			
			base.NotifySusccessfulLogin (user);
		}

		protected override void ChangeAuthenticatedUser(SoftwareUserEntity user)
		{
			var aiderUser = user as AiderUserEntity;

			if (aiderUser != null)
			{
				var now  = System.DateTime.UtcNow;
				var last = aiderUser.LastActivityDate;

				if ((last == null) ||
					((now-last.Value).Seconds > 10))
				{
					this.UpdateUser (user.Code, u => u.LastActivityDate = now);
				}
			}

			base.ChangeAuthenticatedUser (user);
		}


		private void UpdateUser(string userCode, System.Action<AiderUserEntity> update)
		{
			using (var context = new BusinessContext (this.Host, false))
			{
				context.GlobalLock = GlobalLocks.UserManagement;
				
				var example = new AiderUserEntity ()
				{
					Code = userCode,
					Disabled = false,
				};

				var repo = context.GetRepository<AiderUserEntity> ();
				var user = repo.GetByExample (example).FirstOrDefault ();

				if (user != null)
				{
					update (user);
					context.SaveChanges (LockingPolicy.ReleaseLock);
				}
			}
		}
	}
}
