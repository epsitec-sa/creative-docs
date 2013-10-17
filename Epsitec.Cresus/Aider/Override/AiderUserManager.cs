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
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support;
using Epsitec.Cresus.Core.Library;

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

			var notif = NotificationManager.GetCurrentNotificationManager ();

			this.NotifyUserLogin (user as AiderUserEntity, notif);
			this.NotifyMissingEMail (user as AiderUserEntity, notif);
			base.NotifySusccessfulLogin (user);
		}

		public override void NotifyChangePassword(SoftwareUserEntity user)
		{
			var notif = NotificationManager.GetCurrentNotificationManager ();

			this.NotifyChangePassword (user as AiderUserEntity, notif);
			base.NotifyChangePassword (user);
		}

		protected override void ChangeAuthenticatedUser(SoftwareUserEntity user)
		{
			var aiderUser = user as AiderUserEntity;

			if (aiderUser != null)
			{
				AiderActivityLogger.Current.RecordAccess (aiderUser);

				var notif = NotificationManager.GetCurrentNotificationManager ();

				var now  = System.DateTime.UtcNow;
				var last = aiderUser.LastActivityDate;
				var date = aiderUser.LastSoftwareReleaseDate;

				if ((last == null) ||
					(date == null) ||
					((now-last.Value).Seconds > 10))
				{
					this.UpdateUser (user.Code,
						u =>
						{
							u.LastActivityDate = now;
							u.LastSoftwareReleaseDate = CoreContext.SoftwareReleaseDate;
						});
				}
			}

			base.ChangeAuthenticatedUser (user);
		}


		private void NotifyUserLogin(AiderUserEntity user, NotificationManager notif)
		{
			notif.Notify (user.LoginName,
				new NotificationMessage ()
				{
					Title = "Information AIDER",
					Body = "Bienvenue..."
				},
				NotificationTime.OnConnect);

			notif.NotifyAll (
				new NotificationMessage ()
				{
					Title = "Information AIDER",
					Body = user.DisplayName + " vient de se connecter."
				},
				NotificationTime.Now);
		}

		private void NotifyChangePassword(AiderUserEntity user, NotificationManager notif)
		{
			var message = new NotificationMessage ()
			{
				Title     = "Attention AIDER",
				Body      = "Merci de changer votre mot de passe! Cliquez sur ce message pour accéder à votre profil...",
				Dataset   = Res.CommandIds.Base.ShowAiderUser,
				EntityKey = this.BusinessContext.DataContext.GetNormalizedEntityKey (user).Value,
				
				HeaderErrorMessage = "Réinitialisez votre mot de passe à l'aide du bouton d'action"
			};

			notif.WarnUser (user.LoginName, message, NotificationTime.OnConnect);
		}

		private void NotifyMissingEMail(AiderUserEntity user, NotificationManager notif)
		{
			if (string.IsNullOrEmpty (user.Email))
			{
				var message = new NotificationMessage ()
				{
					Title     = "Attention AIDER",
					Body      = "Merci de saisir votre adresse e-mail. Cliquez sur ce message pour accéder à votre profil...",
					Dataset   = Res.CommandIds.Base.ShowAiderUser,
					EntityKey = this.BusinessContext.DataContext.GetNormalizedEntityKey (user).Value,
					
					HeaderErrorMessage = "Adresse e-mail manquante",
					
					ErrorField        = LambdaUtils.Convert ((AiderUserEntity e) => e.Email),
					ErrorFieldMessage = "votre adresse e-mail"
				};

				notif.WarnUser (user.LoginName, message, NotificationTime.OnConnect);
			}
		}


		private void UpdateUser(string userCode, System.Action<AiderUserEntity> update)
		{
#if false
			var notif = Epsitec.Cresus.Core.Library.NotificationManager.GetCurrentNotificationManager ();
			
			notif.NotifyAll (new Cresus.Core.Library.NotificationMessage ()
			{
				Title = "Welcome back",
				Body = "C'est un plaisir de vous revoir parmi nous..."
			});
#endif

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
