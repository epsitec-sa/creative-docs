//	Copyright � 2010-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Data;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Core.Factories;
using Epsitec.Cresus.Core.Library;

namespace Epsitec.Cresus.Core.Business.UserManagement
{
	/// <summary>
	/// The <c>UserManager</c> class is used to authenticate users, based on their name
	/// and on their credentials (which might be a password).
	/// </summary>
	public sealed class UserManager : CoreDataComponent
	{
		private UserManager(CoreData data)
			: base (data)
		{
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
		public IBusinessContext					BusinessContext
		{
			get
			{
				if (this.businessContext == null)
				{
					this.businessContext = InterfaceImplementationResolver<IBusinessContext>.CreateInstance (this.Host);
					this.businessContext.GlobalLock = GlobalLocks.UserManagement;
				}

				return this.businessContext;
			}
		}

		
		/// <summary>
		/// Gets the user manager instance for the executing thread.
		/// </summary>
		public static UserManager				Current
		{
			get
			{
				var coreData = CoreApp.FindCurrentAppSessionComponent<CoreData> ();
				var userManager = coreData.GetComponent<UserManager> ();
				return userManager;
			}
		}

		/// <summary>
		/// Authenticates the specified user. This will display a dialog to query for the
		/// user name and/or password.
		/// </summary>
		/// <param name="application">The application.</param>
		/// <param name="user">The user (or <c>null</c> if it must be selected interactively).</param>
		/// <param name="softwareStartup">if set to <c>true</c> [software startup].</param>
		/// <returns>
		///   <c>true</c> if the user was successfully authenticated; otherwise, <c>false</c>.
		/// </returns>
		public bool Authenticate(CoreApp application, SoftwareUserEntity user = null, bool softwareStartup = false)
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
			if ((softwareStartup) &&
				(user != null) &&
				(user == this.FindActiveSystemUser ()))
			{
				this.SetAuthenticatedUser (user, NotificationMode.OnChange);
				return true;
			}

			var dialog = new Dialogs.LoginDialog (application, user, softwareStartup);
			
			dialog.IsModal = true;
			dialog.OpenDialog ();

			if (dialog.Result == Common.Dialogs.DialogResult.Cancel)
			{
				return false;
			}

			this.SetAuthenticatedUser (dialog.SelectedUser, NotificationMode.OnChange);
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
				this.SetAuthenticatedUser (user.Code, NotificationMode.Always);
			}
		}

		/// <summary>
		/// Sets the authenticated user, based on the user code.
		/// </summary>
		/// <param name="userCode">The user code.</param>
		public void SetAuthenticatedUser(string userCode)
		{
			this.SetAuthenticatedUser (userCode, NotificationMode.OnChange);
		}

		private void SetAuthenticatedUser(string userCode, NotificationMode notificationMode)
		{
			var user = this.FindActiveUser (userCode);
			this.SetAuthenticatedUser (user, notificationMode);
		}

		/// <summary>
		/// Sets the authenticated user, based on the user entity (which must belong to the user
		/// manager's business context).
		/// </summary>
		/// <param name="user">The user.</param>
		private void SetAuthenticatedUser(SoftwareUserEntity user, NotificationMode notificationMode)
		{
			if ((this.authenticatedUser == user) &&
				(notificationMode == NotificationMode.OnChange))
			{
				return;
			}

			this.OnAuthenticatedUserChanging ();
			this.authenticatedUser = user;
			this.OnAuthenticatedUserChanged ();
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
				return user.UserGroups.Any (group => group.UserPowerLevel == level);
			}
		}

		/// <summary>
		/// Gets all users.
		/// </summary>
		/// <returns>The complete collection of users.</returns>
		public IEnumerable<SoftwareUserEntity> GetAllUsers()
		{
			return this.Host.GetAllEntities<SoftwareUserEntity> (dataContext: this.BusinessContext.DataContext).Where (user => user.IsArchive == false);
		}

		/// <summary>
		/// Gets the active users.
		/// </summary>
		/// <returns>The collection of active users.</returns>
		public IEnumerable<SoftwareUserEntity> GetActiveUsers()
		{
			return this.Host.GetAllEntities<SoftwareUserEntity> (dataContext: this.BusinessContext.DataContext).Where (user => user.IsActive);
		}

		public IEnumerable<SoftwareUserGroupEntity> GetAllUserGroups()
		{
			return this.Host.GetAllEntities<SoftwareUserGroupEntity> (dataContext: this.BusinessContext.DataContext).Where (group => group.IsArchive == false);
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
		public SoftwareUserEntity FindActiveSystemUser()
		{
			var users = this.GetActiveUsers ().Where (user => user.Disabled == false && user.AuthenticationMethod == UserAuthenticationMethod.System);
			var login = System.Environment.UserName;

			return users.FirstOrDefault (user => string.Equals (user.LoginName, login, System.StringComparison.InvariantCultureIgnoreCase));
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


		/// <summary>
		/// Finds the user based on his login name.
		/// </summary>
		/// <param name="loginName">The user name.</param>
		/// <returns></returns>
		public SoftwareUserEntity FindUser(string loginName)
		{
			var softwareUserRepository = this.Host.GetRepository<SoftwareUserEntity> ();

			var example = softwareUserRepository.CreateExample();
			example.LoginName = loginName;

			return softwareUserRepository.GetByExample (example).FirstOrDefault ();
		}

		/// <summary>
		/// Gets the user summary for the authenticated user.
		/// </summary>
		/// <returns>The <see cref="UserSummary"/>.</returns>
		public UserSummary GetUserSummary()
		{
			return new UserSummary (this.AuthenticatedUser);
		}


		#region NotificationMode Enumeration

		private enum NotificationMode
		{
			OnChange,
			Always,
		}

		#endregion

		
		private SoftwareUserEntity FindActiveUser(string userCode)
		{
			return this.GetActiveUsers ().FirstOrDefault (user => user.Code == userCode);
		}


		public bool CheckUserAuthentication(string loginName, string password)
		{
			bool isValid = false;

			if (!string.IsNullOrEmpty (loginName) && !string.IsNullOrEmpty (password))
			{
				var user = this.FindUser (loginName);

				if (user != null)
				{
					isValid = this.CheckUserAuthentication (user, password);
				}
			}

			return isValid;
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
			if (user.IsNull ())
			{
				return false;
			}

			if (user.IsPasswordRequired == false)
			{
				return true;
			}

			return user.CheckPassword (password);
		}

		private bool CheckPasswordUserAuthentication(SoftwareUserEntity user, string password)
		{
			return user.CheckPassword (password);
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

		#region Factory Class

		private sealed class Factory : ICoreDataComponentFactory
		{
			#region ICoreDataComponentFactory Members

			public bool CanCreate(CoreData data)
			{
				return data.IsReady;
			}

			public CoreDataComponent Create(CoreData data)
			{
				return new UserManager (data);
			}

			public System.Type GetComponentType()
			{
				return typeof (UserManager);
			}

			#endregion
		}

		#endregion

		public event EventHandler				AuthenticatedUserChanging;
		public event EventHandler				AuthenticatedUserChanged;


		private SoftwareUserEntity				authenticatedUser;
		private IBusinessContext				businessContext;
	}
}
