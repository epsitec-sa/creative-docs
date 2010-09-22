//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Support;

namespace Epsitec.Cresus.Core.Business.UserManagement
{
	/// <summary>
	/// The <c>UserManager</c> class is used to authenticate users, based on their name
	/// and on their credentials (which might be a password).
	/// </summary>
	public class UserManager
	{
		public UserManager(CoreData data)
		{
			this.data = data;
		}


		/// <summary>
		/// Gets the authenticated user.
		/// </summary>
		/// <value>The authenticated user (or <c>null</c>).</value>
		public SoftwareUserEntity AuthenticatedUser
		{
			get
			{
				return this.authenticatedUser;
			}
		}


		/// <summary>
		/// Authenticates the specified user. This will display a dialog to query for the
		/// user name and/or password.
		/// </summary>
		/// <param name="user">The user (or <c>null</c> if it must be selected interactively).</param>
		/// <returns><c>true</c> if the user was successfully authenticated; otherwise, <c>false</c>.</returns>
		public bool Authenticate(SoftwareUserEntity user = null)
		{
			user = null;  // TODO: provisoire
			string defaultPassword = null;

			for (int i = 0; i < 3; i++)
			{
				if (user == null)
				{
					var dialog = new Dialogs.UserManagerDialog (CoreProgram.Application);
					dialog.IsModal = true;
					dialog.OpenDialog ();

					if (dialog.Result == Common.Dialogs.DialogResult.Cancel)
					{
						return false;
					}

					user            = dialog.SelectedUser;
					defaultPassword = dialog.DefaultPassword;
				}

				var result = this.CheckUserAuthentication (user, defaultPassword);

				if (result == CheckResult.OK)
				{
					this.OnAuthenticatedUserChanging ();
					this.authenticatedUser = user;
					this.OnAuthenticatedUserChanged ();

					return true;
				}
				else if (result == CheckResult.Error)
				{
					return false;
				}

				user = null;  // on redemande l'utilisateur
			}

			return false;
		}


		/// <summary>
		/// Gets the active users.
		/// </summary>
		/// <returns>The collection of active users.</returns>
		public IEnumerable<SoftwareUserEntity> GetActiveUsers()
		{
			return this.data.GetAllEntities<SoftwareUserEntity> ().Where (user => user.IsActive);
		}

		public string CreateNewUser(System.Action<BusinessLogic.BusinessContext, SoftwareUserEntity> initializer)
		{
			using (var context = this.data.CreateBusinessContext ())
			{
				var user = context.CreateEntity<SoftwareUserEntity> ();

				initializer (context, user);
				context.SaveChanges ();

				return user.Code;
			}
		}

		/// <summary>
		/// Finds the active system user, based on the user currently logged in to Windows.
		/// </summary>
		/// <returns>The user or <c>null</c>.</returns>
		public SoftwareUserEntity FindActiveSystemUser()
		{
			var users = this.GetActiveUsers ();
			var login = System.Environment.UserName;

			return users.FirstOrDefault (user => user.AuthenticationMethod == UserAuthenticationMethod.System && user.LoginName == login);
		}

		public SoftwareUserEntity FindActiveUser(string userCode)
		{
			return this.GetActiveUsers ().FirstOrDefault (user => user.Code == userCode);
		}

		private CheckResult CheckUserAuthentication(SoftwareUserEntity user, string defaultPassword)
		{
			switch (user.AuthenticationMethod)
            {
            	case UserAuthenticationMethod.None:
					return CheckResult.OK;

				case UserAuthenticationMethod.System:
					return this.CheckSystemUserAuthentication (user);

				case UserAuthenticationMethod.Password:
					return this.CheckPasswordUserAuthentication (user, defaultPassword);

				default:
					return CheckResult.Error;
            }
		}

		private CheckResult CheckSystemUserAuthentication(SoftwareUserEntity user)
		{
			return (user.LoginName == System.Environment.UserName) ? CheckResult.OK : CheckResult.Error;
		}

		private CheckResult CheckPasswordUserAuthentication(SoftwareUserEntity user, string defaultPassword)
		{
			if (defaultPassword == null)
			{
				//	TODO: display a dialog box to request the password for the specified
				//	user...
			}

			for (int i = 0; i < 3; i++)
			{
				if (user.CheckPassword (defaultPassword))
				{
					return CheckResult.OK;
				}

				//	TODO: display dialog box to request the password (the one provided was not valid).
				var dialog = new Dialogs.PasswordDialog (CoreProgram.Application, user);
				dialog.IsModal = true;
				dialog.OpenDialog ();

				if (dialog.Result == Common.Dialogs.DialogResult.Cancel)
				{
					return CheckResult.Cancel;
				}

				defaultPassword = dialog.DefaultPassword;
			}

			return CheckResult.Error;
		}

		private enum CheckResult
		{
			OK,
			Error,
			Cancel,
		}


		private void OnAuthenticatedUserChanging()
		{
			var handler = this.AuthenticatedUserChanging;

			if (handler != null)
			{
				handler (this);
			}
		}
		
		private void OnAuthenticatedUserChanged()
		{
			var handler = this.AuthenticatedUserChanged;

			if (handler != null)
			{
				handler (this);
			}
		}
		
		public event EventHandler AuthenticatedUserChanging;
        public event EventHandler AuthenticatedUserChanged;


		private readonly CoreData data;
		private SoftwareUserEntity authenticatedUser;
	}
}
