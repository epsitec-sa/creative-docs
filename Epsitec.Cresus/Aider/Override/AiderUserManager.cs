//	Copyright © 2012-2020, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;

using Epsitec.Common.Support;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Business.UserManagement;
using Epsitec.Cresus.Core.Data;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Library;

using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Epsitec.Aider.Override
{
	public sealed class AiderUserManager : UserManager
	{
		public AiderUserManager(CoreData data, bool enableReload)
			: base (data, enableReload)
		{
            this.pending2FApins = new Dictionary<string, Pending2FA> ();
			this.randomGenerator = new System.Random ();
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


        public override bool Start2FALogin(string loginName)
        {
            var user = this.FindUser (loginName) as AiderUserEntity;

            if ((user != null) &&
                (user.SecondFactorMode == Enumerations.SecondFactorMode.PinByMobile) &&
				(string.IsNullOrEmpty (user.Mobile) == false))
            {
				var rnd = this.GetRandomNumber (0, 9999);
                var pin = rnd.ToString ("0000");
                this.AddPin (loginName, pin);

				if ((user.Mobile.StartsWith ("+41")) &&
					(user.Mobile.Length == 12))
				{
					//	Fire & forget task...
					var task = this.Send2FAMessage (pin, user.Mobile.Substring (1));

					return true;
				}
            }

            return false; 
        }

        public override bool CheckUserPin(string loginName, string pin)
        {
            return (string.IsNullOrEmpty (loginName) == false)
                && (string.IsNullOrEmpty (pin) == false)
                && (this.CheckPin (loginName, pin));
        }

        public override void NotifySusccessfulLogin(SoftwareUserEntity user)
		{
            this.UpdateUser (user.Code,
                u =>
                {
                    u.LastLoginDate = u.LastActivityDate = System.DateTime.UtcNow;
                    u.LastSoftwareReleaseDate = CoreContext.SoftwareReleaseDate;
                });

			var notif = NotificationManager.GetCurrentNotificationManager ();

			this.NotifyUserLogin (user as AiderUserEntity, notif);
			this.NotifyMissingEMail (user as AiderUserEntity, notif);
			this.NotifyInvalidContact (user as AiderUserEntity, notif);
			this.NotifyMissingOffice (user as AiderUserEntity, notif);

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

				if ((last == null) ||
					((now-last.Value).TotalMinutes > 10))
				{
					this.UpdateUser (user.Code, u => u.LastActivityDate = now);
				}
			}

			base.ChangeAuthenticatedUser (user);
		}


		public static void NotifyBusinessRuleOverride(string messageText)
		{
			var user  = UserManager.Current.AuthenticatedUser;
			var notif = NotificationManager.GetCurrentNotificationManager ();
			
			var message = new NotificationMessage ()
			{
				Title = Resources.Text ("Avertissement – Règle métier non respectée"),
				Body = messageText
			};

			notif.WarnUser (user.LoginName, message, When.Now);
		}


		private int GetRandomNumber(int min, int max)
		{
			return this.randomGenerator.Next (min, max + 1);
		}

		private async Task Send2FAMessage(string pin, string destination)
		{
			var message = System.Uri.EscapeDataString ($"Code d'accès AIDER:\n{pin}");
			var uri = new System.Uri ($"https://partout.cresus.ch/sms/v1/send/e37f7c4d-ad62-4f60-b520-1f054c038c04/{destination}/{message}");
			
			using var client = new HttpClient ();

			System.Console.WriteLine ($"PIN {pin} for {destination}");

			try
			{
				var result = await client.GetStringAsync (uri);
			}
			catch (System.Exception ex)
			{
				System.Console.WriteLine (ex.Message);
				System.Console.WriteLine (ex.StackTrace);
			}
		}

		private void NotifyUserLogin(AiderUserEntity user, NotificationManager notif)
		{
			if (user.Office.IsNotNull ())
			{
				var office = user.Office;
				notif.Notify (user.LoginName,
				new NotificationMessage ()
				{
					Title = office.OfficeName + ", bienvenue...",
					Body = office.OfficeUsersLoginMessage
				},
				When.OnConnect);
			}

			var offices = user.GetOfficesByJobs ();
			foreach (var office in offices)
			{
				if (office.Tasks.Count (t => t.IsDone == false) > 0)
				{
					var nbTasks = office.Tasks.Count (t => t.IsDone == false);
					var message = new NotificationMessage ()
					{
						Title     = office.OfficeShortName,
						Body      = "Il y a " + nbTasks + " tâches en suspens",
						Dataset   = Res.CommandIds.Base.ShowAiderOfficeManagement,
						EntityKey = this.BusinessContext.DataContext.GetNormalizedEntityKey (office).Value
					};

					notif.Notify (user.LoginName, message, When.OnConnect);
				}
			}

			notif.NotifyAll (
				new NotificationMessage ()
				{
					Title = "Information AIDER",
					Body = user.DisplayName + " vient de se connecter."
				},
				When.Now);
		}

		public static IEnumerable<DataSetUISettingsEntity> GetCurrentDataSetUISettings(string dataSetCommandId)
		{
			return AiderUserManager.Current
								   .AuthenticatedUser
								   .CustomUISettings
								   .DataSetUISettings
								   .Where 
								   (
										d => 
										d.DataSetCommandId == dataSetCommandId
								   );
		}

        private void AddPin(string loginName, string pin)
        {
            lock (this.pending2FApins)
            {
                this.pending2FApins[loginName] = new Pending2FA (loginName, pin);
            }
        }

        private void RemovePin(string loginName, string pin)
        {
            lock (this.pending2FApins)
            {
                this.pending2FApins.Remove (loginName);
            }
        }

        private bool CheckPin(string loginName, string pin)
        {
            lock (this.pending2FApins)
            {
                if ((this.pending2FApins.TryGetValue (loginName, out var info)) &&
                    (info.Pin == pin))
                {
                    this.pending2FApins.Remove (loginName);
                    return true;
                }
            }

            return false;
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

			notif.WarnUser (user.LoginName, message, When.OnConnect);
		}

		private void NotifyMissingEMail(AiderUserEntity user, NotificationManager notif)
		{
            if (string.IsNullOrEmpty (user.Email))
            {
                var message = new NotificationMessage ()
                {
                    Title = "Attention AIDER",
                    Body = "Merci de saisir votre adresse e-mail. Cliquez sur ce message pour accéder à votre profil...",
                    Dataset = Res.CommandIds.Base.ShowAiderUser,
                    EntityKey = this.BusinessContext.DataContext.GetNormalizedEntityKey (user).Value,

                    HeaderErrorMessage = "Adresse e-mail manquante",

                    ErrorField = LambdaUtils.Convert ((AiderUserEntity e) => e.Email),
                    ErrorFieldMessage = "votre adresse e-mail"
                };

                notif.WarnUser (user.LoginName, message, When.OnConnect);
            }

            if (string.IsNullOrEmpty (user.Mobile))
            {
                var message = new NotificationMessage ()
                {
                    Title = "Attention AIDER",
                    Body = "Merci de saisir votre numéro de téléphone mobile. Cliquez sur ce message pour accéder à votre profil...",
                    Dataset = Res.CommandIds.Base.ShowAiderUser,
                    EntityKey = this.BusinessContext.DataContext.GetNormalizedEntityKey (user).Value,

                    HeaderErrorMessage = "Téléphone mobile inconnu",

                    ErrorField = LambdaUtils.Convert ((AiderUserEntity e) => e.Mobile),
                    ErrorFieldMessage = "votre numéro de mobile"
                };

                notif.WarnUser (user.LoginName, message, When.OnConnect);
            }
        }

        private void NotifyInvalidContact(AiderUserEntity user, NotificationManager notif)
		{
			if (user.Contact.IsNull ())
			{
				var message = new NotificationMessage ()
				{
					Title     = "Attention AIDER",
					Body      = "Merci d'associer votre contact à votre profil. Cliquez sur ce message pour accéder à votre profil...",
					Dataset   = Res.CommandIds.Base.ShowAiderUser,
					EntityKey = this.BusinessContext.DataContext.GetNormalizedEntityKey (user).Value,

					HeaderErrorMessage = "Contact non défini",

					ErrorField        = LambdaUtils.Convert ((AiderUserEntity e) => e.Contact),
					ErrorFieldMessage = "votre contact"
				};

				notif.WarnUser (user.LoginName, message, When.OnConnect);
				return;
			}

			if (user.Contact.Person.IsNull ())
			{
				var message = new NotificationMessage ()
				{
					Title     = "Attention AIDER",
					Body      = "Votre contact doit être associé à votre personne physique. Cliquez sur ce message pour accéder à votre profil...",
					Dataset   = Res.CommandIds.Base.ShowAiderUser,
					EntityKey = this.BusinessContext.DataContext.GetNormalizedEntityKey (user).Value,

					HeaderErrorMessage = "Contact associé de manière incorrecte",

					ErrorField        = LambdaUtils.Convert ((AiderUserEntity e) => e.Contact),
					ErrorFieldMessage = "votre contact"
				};

				notif.WarnUser (user.LoginName, message, When.OnConnect);
				return;
			}
		}

		private void NotifyMissingOffice(AiderUserEntity user, NotificationManager notif)
		{
			if ((user.Office.IsNull ()) &&
				(user.EnableGroupEditionCanton == false) &&
				(user.EnableGroupEditionRegion == false))
			{
				var message = new NotificationMessage ()
				{
					Title     = "Attention AIDER",
					Body      = "Merci de rejoindre votre paroisse. Cliquez sur <b>Gestion</b>, sélectionnez votre paroisse, puis choisissez <b>Rejoindre</b>.",
					Dataset   = Res.CommandIds.Base.ShowAiderOfficeManagement,
				};

//-				notif.WarnUser (user.LoginName, message, When.OnConnect);
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


        private readonly Dictionary<string, Pending2FA> pending2FApins;
		private readonly System.Random randomGenerator;
    }
}
