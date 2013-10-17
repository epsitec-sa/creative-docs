//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Business.UserManagement;

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;

using System.Linq;

namespace Epsitec.Aider.Entities
{
	public partial class AiderUserEntity
	{
		public UserPowerLevel					PowerLevel
		{
			get
			{
				UserPowerLevel level = UserPowerLevel.None;

				foreach (var x in this.UserGroups.Select (x => x.UserPowerLevel).Where (x => x != UserPowerLevel.None))
				{
					if ((level == UserPowerLevel.None) ||
						(level > x))
					{
						level = x;
					}
				}

				return level;
			}
		}

		
		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText
			(
				this.DisplayName, "(", this.LoginName, ")", "\n",
				TextFormatter.FormatText ("E-mail: ").ApplyBold (), this.Email, "\n",
				TextFormatter.FormatText ("Groupe: ").ApplyBold (), this.Parish.Name, "\n",
				TextFormatter.FormatText ("Rôle: ").ApplyBold (), this.Role.Name, "\n",
				TextFormatter.FormatText ("Administrateur: ").ApplyBold (), this.HasPowerLevel (UserPowerLevel.Administrator).ToYesOrNo (), "\n",
				TextFormatter.FormatText ("Actif: ").ApplyBold (), this.IsActive.ToYesOrNo (), "\n",
				TextFormatter.FormatText ("Dernier login: ").ApplyBold (), this.LastLoginDate.ToLocalTime (), "\n",
				TextFormatter.FormatText ("Dernier accès: ").ApplyBold (), this.LastActivityDate.ToLocalTime (), "\n"
			);
		}

		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.DisplayName);
		}


		public void AssignGroup(BusinessContext businessContext, UserPowerLevel powerLevel)
		{
			var group = AiderUserEntity.GetSoftwareUserGroup (businessContext, powerLevel);

			if (group != null)
			{
				this.UserGroups.Add (group);
			}
		}

		public bool CanViewConfidentialAddress()
		{
			return (this.Role.Name == AiderUserRoleEntity.AleRole) || this.HasPowerLevel (UserPowerLevel.Administrator);
		}

		public void SetPassword(string password, string confirmation)
		{
			if (password == null)
			{
				var message = Res.Strings.AiderUserPasswordEmpty.ToString ();

				throw new BusinessRuleException (this, message);
			}

			if (password.Length < 8)
			{
				var message = Res.Strings.AiderUserPasswordTooShort.ToString ();

				throw new BusinessRuleException (this, message);
			}

			if (password != confirmation)
			{
				var message = Res.Strings.AiderUserPasswordMismatch.ToString ();

				throw new BusinessRuleException (this, message);
			}

			this.SetPassword (password);
		}

		public void SetAdmininistrator(BusinessContext businessContext, bool admin)
		{
			var powerLevel = UserPowerLevel.Administrator;

			var isAdmin = this.HasPowerLevel (powerLevel);

			if (!isAdmin && admin)
			{
				this.AssignGroup (businessContext, powerLevel);
			}
			else if (isAdmin && !admin)
			{
				this.UserGroups.RemoveAll
				(
					g => g.UserPowerLevel != UserPowerLevel.None && g.UserPowerLevel <= powerLevel
				);
			}
		}

		public void Delete(BusinessContext businessContext)
		{
			this.CustomUISettings.Delete (businessContext);

			businessContext.DeleteEntity (this);
		}


		partial void OnParishChanging(AiderGroupEntity oldValue, AiderGroupEntity newValue)
		{
			this.ParishGroupPathCache = AiderGroupEntity.GetPath (newValue);
		}
		
		private static SoftwareUserGroupEntity GetSoftwareUserGroup(BusinessContext businessContext, UserPowerLevel powerLevel)
		{
			var example = new SoftwareUserGroupEntity ()
			{
				UserPowerLevel = powerLevel
			};

			var dataContext = businessContext.DataContext;

			return dataContext.GetByExample (example).FirstOrDefault ();
		}
	}
}
