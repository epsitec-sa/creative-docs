//	Copyright © 2012-2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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

		public bool CanViewOfficeDetails()
		{
			return (this.Role.Name == AiderUserRoleEntity.AleRole) || this.HasPowerLevel (UserPowerLevel.Administrator);
		}

		public bool CanRemoveMailing()
		{
			return (this.Role.Name == AiderUserRoleEntity.AleRole) || this.HasPowerLevel (UserPowerLevel.Administrator);
		}

		public bool CanEditEmployee()
		{
			return (this.Role.Name == AiderUserRoleEntity.AleRole) || this.HasPowerLevel (UserPowerLevel.Administrator);
		}

		public bool CanEditReferee()
		{
			return ((this.Role.Name == AiderUserRoleEntity.RegionRole)  || 
					this.HasPowerLevel (UserPowerLevel.Administrator))
					&& this.IsOfficeDefined ();
		}

		public bool IsOfficeDefined()
		{
			return this.Office.IsNotNull ();
		}

		public bool IsAdmin()
		{
			return this.HasPowerLevel (UserPowerLevel.Administrator);
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

		public void SetParishOrRegion(BusinessContext businessContext, AiderGroupEntity group)
		{
			var currentParish = this.Parish;

			if (this.Contact.IsNotNull ())
			{
				if (this.Office.IsNotNull ())
				{
					var office = AiderOfficeManagementEntity.Find (businessContext,group);
					if(office.IsNotNull ())
					{
						AiderOfficeManagementEntity.JoinOfficeManagement (businessContext, office, this);
					}
				}
				else
				{
					//Stop old usergroup participation
					var currentUserGroup = currentParish.Subgroups.Single (s => s.GroupDef.Classification == Enumerations.GroupClassification.Users);
					if (currentUserGroup.IsNotNull ())
					{
						AiderGroupEntity.RemoveParticipations (businessContext, currentUserGroup.FindParticipationsByGroup (businessContext, this.Contact, currentUserGroup));
					}
					//Create usergroup participation
					var newUserGroup = group.Subgroups.Single (s => s.GroupDef.Classification == Enumerations.GroupClassification.Users);
					if (newUserGroup.IsNotNull ())
					{
						var participationData = new List<ParticipationData> ();
						participationData.Add (new ParticipationData (this.Contact));
						newUserGroup.AddParticipations (businessContext, participationData, Date.Today, FormattedText.Null);
					}
				}
			}

			//set new parish
			this.Parish = group;
		}

		public void Delete(BusinessContext businessContext)
		{
			this.CustomUISettings.Delete (businessContext);

			businessContext.DeleteEntity (this);
		}

		public static AiderUserEntity Create(
			BusinessContext businessContext, 
			string username, 
			string displayname,  
			AiderContactEntity contact, 
			AiderUserRoleEntity role, 
			AiderGroupEntity parish)
		{
			var user = businessContext.CreateAndRegisterEntity<AiderUserEntity> ();

			user.LoginName = username;
			user.DisplayName = displayname;
			user.Role = role;
			user.Parish = parish;
			user.Contact = contact;

			return user;
		}


		partial void OnParishChanging(AiderGroupEntity oldValue, AiderGroupEntity newValue)
		{
			var path = AiderGroupEntity.GetPath (newValue);

			//	Setting ParishGroupPathCache with null, while it had already the null value
			//	before, has a side effect: the field 'ParishGroupPathCache' will no longer
			//	be considered to be undefined or in its default state.
			//
			//	This would cause DataSetAccessor.CreateRequestView to produce an invalid
			//	query (looking for entities where ParishGroupPathCache is null, whereas
			//	the field should simply be ignored in the request).
			//
			//	So we make sure we don't change the path if its value was already the
			//	same as before...
			
			if (this.ParishGroupPathCache != path)
			{
				this.ParishGroupPathCache = path;
			}
		}
		
		partial void GetPowerLevel(ref UserPowerLevel value)
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

			value = level;
		}

		partial void SetPowerLevel(UserPowerLevel value)
		{
			this.UserGroups.Clear ();

			if (value == UserPowerLevel.None)
			{
				return;
			}
			
			var dataContext = this.GetDataContext ();
			
			//	Setting the power level picks the proper user group and associates it with
			//	the user; unless the level is more restricted than "standard", we should
			//	always include the standard user level too.
			
			if (value < UserPowerLevel.Restricted)
			{
				var example = new SoftwareUserGroupEntity
				{
					UserPowerLevel = UserPowerLevel.Standard
				};

				var std = dataContext.GetByExample (example).Single ();
				
				this.UserGroups.Add (std);
			}

			if (value != UserPowerLevel.Standard)
			{
				var example = new SoftwareUserGroupEntity
				{
					UserPowerLevel = value
				};

				var group = dataContext.GetByExample (example).FirstOrDefault ();

				if (group != null)
				{
					this.UserGroups.Add (group);
				}
			}
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
