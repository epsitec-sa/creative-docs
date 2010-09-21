//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

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
		public UserManager (CoreData data)
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
			this.authenticatedUser = null;

			string defaultPassword = null;

			if (user == null)
			{
				//	TODO: display dialog box with list of users and let the user
				//	pick a user (and maybe also type in a password, if the user
				//	authentication mode is set to 'Password')
			}

			if (this.CheckUserAuthentication (user, defaultPassword))
			{
				this.authenticatedUser = user;
				return true;
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

		public SoftwareUserEntity FindActiveUser(string userCode)
		{
			return this.GetActiveUsers ().FirstOrDefault (user => user.Code == userCode);
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


		private bool CheckUserAuthentication(SoftwareUserEntity user, string defaultPassword)
		{
			switch (user.AuthenticationMethod)
            {
            	case UserAuthenticationMethod.None:
            		return true;

				case UserAuthenticationMethod.System:
					return this.CheckSystemUserAuthentication (user);

				case UserAuthenticationMethod.Password:
					return this.CheckPasswordUserAuthentication (user, defaultPassword);

				default:
					return false;
            }
		}

		private bool CheckSystemUserAuthentication(SoftwareUserEntity user)
		{
			return (user.LoginName == System.Environment.UserName);
		}

		private bool CheckPasswordUserAuthentication(SoftwareUserEntity user, string defaultPassword)
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
					return true;
				}

				//	TODO: display dialog box to request the password (the one provided was not valid).
			}

			return false;
		}


		private readonly CoreData data;
		private SoftwareUserEntity authenticatedUser;
	}
}
