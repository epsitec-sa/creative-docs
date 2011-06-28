//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Data;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Library.Settings;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.UserManagement
{
	/// <summary>
	/// The <c>UserSummary</c> class is used to summarize information about a user,
	/// by collapsing the data found in the various user groups and roles.
	/// </summary>
	public sealed class UserSummary
	{
		public UserSummary(SoftwareUserEntity user)
		{
			this.userCode = user.Code;
			this.disabled = user.Disabled || user.UserGroups.Any (x => x.Disabled);
			this.powerLevels = new HashSet<UserPowerLevel> (user.UserGroups.Select (x => x.UserPowerLevel));
			this.roles = new HashSet<string> (user.UserGroups.SelectMany (x => x.Roles).Select (x => x.Code));
		}

		
		public string							UserCode
		{
			get
			{
				return this.userCode;
			}
		}

		public bool								Disabled
		{
			get
			{
				return this.disabled;
			}
		}


		public bool HasPowerLevel(UserPowerLevel level)
		{
			return this.powerLevels.Contains (level);
		}

		public bool HasRole(string role)
		{
			return this.roles.Contains (role);
		}

		public bool Matches(IUserIdentity user)
		{
			if (user == null)
			{
				throw new System.ArgumentNullException ("user");
			}

			switch (user.UserCategory)
			{
				case UserCategory.Any:
					return true;

				case UserCategory.User:
					return this.UserCode == user.UserIdentity;

				case UserCategory.Group:
					return this.HasPowerLevel ((UserPowerLevel) InvariantConverter.ToInt (user.UserIdentity));

				case UserCategory.Role:
					return this.HasRole (user.UserIdentity);

				default:
					throw new System.NotSupportedException (string.Format ("UserCategory.{0} not supported", user.UserCategory));
			}
		}

		
		private readonly string					userCode;
		private readonly bool					disabled;
		private readonly HashSet<UserPowerLevel> powerLevels;
		private readonly HashSet<string>		roles;
	}
}
