﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
		public static bool IsAdministratorUser()
		{
			return User.IsAdministratorUser (User.CurrentUser);
		}

		public static bool IsAdministratorUser(SoftwareUserEntity user)
		{
			if (user.IsNull ())
			{
				return false;
			}
			else
			{
				return user.UserGroups.Where (group => group.UserPowerLevel == UserPowerLevel.Administrator).Count () > 0;
			}
		}


		public static bool IsDeveloperUser()
		{
			return User.IsDeveloperUser (User.CurrentUser);
		}

		public static bool IsDeveloperUser(SoftwareUserEntity user)
		{
			if (user.IsNull ())
			{
				return false;
			}
			else
			{
				return user.UserGroups.Where (group => group.UserPowerLevel == UserPowerLevel.Developer).Count () > 0;
			}
		}


		public static bool IsPowerUserUser()
		{
			return User.IsPowerUserUser (User.CurrentUser);
		}

		public static bool IsPowerUserUser(SoftwareUserEntity user)
		{
			if (user.IsNull ())
			{
				return false;
			}
			else
			{
				return user.UserGroups.Where (group => group.UserPowerLevel == UserPowerLevel.PowerUser).Count () > 0;
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
