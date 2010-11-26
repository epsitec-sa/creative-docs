//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Business.UserManagement;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core
{
	public static class User
	{
		public static bool HasLevelUser(UserPowerLevel level)
		{
			return User.HasLevelUser (User.CurrentUser, level);
		}

		public static bool HasLevelUser(SoftwareUserEntity user, UserPowerLevel level)
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


		public static SoftwareUserEntity CurrentUser
		{
			get
			{
				return CoreProgram.Application.UserManager.AuthenticatedUser;
			}
		}
	}
}
