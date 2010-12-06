//	Copyright � 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Data;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

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

		public CoreData							CoreData
		{
			get
			{
				return this.data;
			}
		}

	
		/// <summary>
		/// Gets the authenticated user.
		/// </summary>
		/// <value>The authenticated user (or <c>null</c>).</value>
		public SoftwareUserEntity				AuthenticatedUser
		{
			get
			{
				return this.authenticatedUser;
			}
		}

		/// <summary>
		/// Gets the associated business context.
		/// </summary>
		/// <value>The business context.</value>
		public BusinessContext					BusinessContext
		{
			get
			{
				if (this.businessContext == null)
                {
					this.businessContext = this.data.CreateBusinessContext ();
					this.businessContext.GlobalLock = Data.GlobalLocks.UserManagement;
                }

				return this.businessContext;
			}
		}


		/// <summary>
		/// Authenticates the specified user. This will display a dialog to query for the
		/// user name and/or password.
		/// </summary>
		/// <param name="user">The user (or <c>null</c> if it must be selected interactively).</param>
		/// <returns><c>true</c> if the user was successfully authenticated; otherwise, <c>false</c>.</returns>
		public bool Authenticate(SoftwareUserEntity user = null, bool softwareStartup = false)
		{
			//	Make sure the user entity belongs to our data context; the only way to know for sure
			//	is to retrieve the user based on its 'code':
			if (user != null)
			{
				user = this.FindActiveUser (user.Code);
			}

			//	Si on est dans le dialogue initial (celui qui s'affiche � l'ex�cution du logiciel),
			//	et que l'utilisateur correspond � celui de la session Windows, on effectue le login
			//	sans afficher le dialogue.
			if (softwareStartup && user != null && user == this.FindActiveUser ())
			{
				this.OnAuthenticatedUserChanging ();
				this.authenticatedUser = user;
				this.OnAuthenticatedUserChanged ();

				return true;
			}

			var dialog = new Dialogs.LoginDialog (CoreProgram.Application, user, softwareStartup);
			dialog.IsModal = true;
			dialog.OpenDialog ();

			if (dialog.Result == Common.Dialogs.DialogResult.Cancel)
			{
				return false;
			}

			this.OnAuthenticatedUserChanging ();
			this.authenticatedUser = dialog.SelectedUser;
			this.OnAuthenticatedUserChanged ();

			return true;
		}

		/// <summary>
		/// Updates the authenticated user after some change in its settings.
		/// </summary>
		public void UpdateAuthenticatedUser()
		{
			var user = this.authenticatedUser;

			if (user != null)
			{
				user = this.FindActiveUser (user.Code);

				this.OnAuthenticatedUserChanging ();
				this.authenticatedUser = user;
				this.OnAuthenticatedUserChanged ();
			}
		}


		/// <summary>
		/// Indicates whether the authenticated user has the required power level.
		/// </summary>
		/// <param name="level">The required power level</param>
		/// <returns><c>true</c> if the user has the required power level; otherwise, <c>false</c>.</returns>
		public bool IsAuthenticatedUserAtPowerLevel(UserPowerLevel level)
		{
			return this.IsUserAtPowerLevel (this.AuthenticatedUser, level);
		}

		/// <summary>
		/// Indicates whether a user has the required power level.
		/// </summary>
		/// <param name="user">The user entitry.</param>
		/// <param name="level">The required power level.</param>
		/// <returns><c>true</c> if the user has the required power level; otherwise, <c>false</c>.</returns>
		public bool IsUserAtPowerLevel(SoftwareUserEntity user, UserPowerLevel level)
		{
			if (user.IsNull ())
			{
				return false;
			}
			else
			{
				return user.UserGroups.Where (group => group.UserPowerLevel == level).Count () > 0;
			}
		}

		/// <summary>
		/// Gets all users.
		/// </summary>
		/// <returns>The complete collection of users.</returns>
		public IEnumerable<SoftwareUserEntity> GetAllUsers()
		{
			return this.data.GetAllEntities<SoftwareUserEntity> (dataContext: this.BusinessContext.DataContext).Where (user => user.IsArchive == false);
		}

		/// <summary>
		/// Gets the active users.
		/// </summary>
		/// <returns>The collection of active users.</returns>
		public IEnumerable<SoftwareUserEntity> GetActiveUsers()
		{
			return this.data.GetAllEntities<SoftwareUserEntity> (dataContext: this.BusinessContext.DataContext).Where (user => user.IsActive);
		}

		public IEnumerable<SoftwareUserGroupEntity> GetAllUserGroups()
		{
			return this.data.GetAllEntities<SoftwareUserGroupEntity> (dataContext: this.BusinessContext.DataContext).Where (group => group.IsArchive == false);
		}

		/// <summary>
		/// Creates a new user.
		/// </summary>
		/// <returns>The new user.</returns>
		public SoftwareUserEntity CreateNewUser()
		{
			return this.BusinessContext.CreateEntity<SoftwareUserEntity> ();
		}

		/// <summary>
		/// Creates a new user group.
		/// </summary>
		/// <returns>The new user group.</returns>
		public SoftwareUserGroupEntity CreateNewUserGroup()
		{
			return this.BusinessContext.CreateEntity<SoftwareUserGroupEntity> ();
		}

		/// <summary>
		/// Saves changes done to the users and user groups stored in the associated
		/// business context.
		/// </summary>
		public void SaveChangesAndDisposeBusinessContext()
		{
			if (this.businessContext != null)
			{
				this.businessContext.SaveChanges ();
				this.businessContext.Dispose ();
				this.businessContext = null;
			}
		}

		/// <summary>
		/// Discards all changes done to the users and user groups stored in the associated
		/// business context.
		/// </summary>
		public void DiscardChangesAndDisposeBusinessContext()
		{
			if (this.businessContext != null)
			{
				this.businessContext.Discard ();
				this.businessContext.Dispose ();
				this.businessContext = null;
			}
		}

		
		/// <summary>
		/// Finds the active user, based on the user currently logged in to Windows.
		/// </summary>
		/// <returns>The user or <c>null</c>.</returns>
		public SoftwareUserEntity FindActiveUser()
		{
			var users = this.GetActiveUsers ();
			var login = System.Environment.UserName;

			return users.FirstOrDefault (user => user.LoginName == login && user.AuthenticationMethod == UserAuthenticationMethod.System && user.Disabled == false);
		}

		/// <summary>
		/// Finds the active user matching the specified <see cref="ItemCode"/>.
		/// </summary>
		/// <param name="userCode">The user code.</param>
		/// <returns>The matching user or <c>null</c>.</returns>
		public SoftwareUserEntity FindActiveUser(ItemCode userCode)
		{
			return this.FindActiveUser (userCode.Code);
		}

		private SoftwareUserEntity FindActiveUser(string userCode)
		{
			return this.GetActiveUsers ().FirstOrDefault (user => user.Code == userCode);
		}

		public bool CheckUserAuthentication(SoftwareUserEntity user, string password)
		{
			switch (user.AuthenticationMethod)
            {
            	case UserAuthenticationMethod.None:
					return true;

				case UserAuthenticationMethod.System:
					return this.CheckSystemUserAuthentication (user, password);

				case UserAuthenticationMethod.Password:
					return this.CheckPasswordUserAuthentication (user, password);

				default:
					return false;
            }
		}

		private bool CheckSystemUserAuthentication(SoftwareUserEntity user, string password)
		{
			if (!UserManager.IsPasswordRequired (user))
			{
				return true;
			}

			return user.CheckPassword (password);
		}

		private bool CheckPasswordUserAuthentication(SoftwareUserEntity user, string password)
		{
			return user.CheckPassword (password);
		}

		public static bool IsPasswordRequired(SoftwareUserEntity user)
		{
			if (user == null)
			{
				return false;
			}

			if (user.AuthenticationMethod == UserAuthenticationMethod.Password)
			{
				return true;
			}

			if (user.AuthenticationMethod == UserAuthenticationMethod.None)
			{
				return false;
			}

			if (user.AuthenticationMethod == UserAuthenticationMethod.System)
			{
				return string.Compare (user.LoginName, System.Environment.UserName, System.StringComparison.CurrentCultureIgnoreCase) != 0;
			}

			return false;
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


		private readonly CoreData			data;

		private SoftwareUserEntity			authenticatedUser;
		private BusinessContext				businessContext;
	}
}
