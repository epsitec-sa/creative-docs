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

		public bool CanViewOfficeEvents()
		{
			var canViewDetails = this.CanViewOfficeDetails ();
			var ministerBypass = false;
			if (this.Contact.IsNotNull ())
			{
				if (this.Contact.Person.Employee.IsNotNull ())
				{
					var employee = this.Contact.Person.Employee;
					switch (employee.EmployeeType)
					{
						case Enumerations.EmployeeType.Diacre:
						case Enumerations.EmployeeType.Pasteur:
							ministerBypass = true;
						break;
					}
				}
			}

			return  canViewDetails || ministerBypass;
		}

		public bool IsOfficeManager()
		{
			var isOfficeManager = false;
			if (this.Contact.IsNotNull ())
			{
				if (this.Contact.Person.Employee.IsNotNull ())
				{
					var employee = this.Contact.Person.Employee;
					isOfficeManager = employee.IsOfficeManager ();
				}
			}

			return isOfficeManager;
		}

		public IEnumerable<AiderGroupEntity> GetGroupsUnderManagement(BusinessContext businessContext)
		{
			if (!this.IsOfficeManager ())
			{
				return Enumerable.Empty<AiderGroupEntity> ();
			}
			var managerJobs = this.Contact.Person.Employee.EmployeeJobs.Where (j => j.EmployeeJobFunction == Enumerations.EmployeeJobFunction.GestionnaireAIDER);
			var offices     = managerJobs.Select (j => j.Office).Distinct ();
			
			return offices.SelectMany (o => AiderGroupEntity.FindGroupsAndSubGroupsFromPathPrefix (businessContext, o.ParishGroupPathCache)).Distinct ();
		}

		public bool CanValidateEvents ()
		{
			var isMinister = false;
			if (this.Contact.IsNotNull ())
			{
				if (this.Contact.Person.Employee.IsNotNull ())
				{
					var employee = this.Contact.Person.Employee;
					isMinister = employee.IsMinister ();
				}
			}

			return isMinister;
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

		public bool CanDerogateTo(AiderGroupEntity derogationParishGroup)
		{
			if ((this.Role.Name == AiderUserRoleEntity.AleRole) || this.HasPowerLevel (UserPowerLevel.Administrator))
			{
				return true;
			}
			else
			{
				if (this.Office.IsNotNull ())
				{
					if (derogationParishGroup == this.Office.ParishGroup)
					{
						return true;
					}
					else
					{
						return false;
					}
				}
				else
				{
					return false;
				}
			}
		}

		public bool CanDoTaskInOffice(AiderOfficeManagementEntity office)
		{
			var canViewDetails = this.CanViewOfficeDetails ();
			if (this.Contact.IsNotNull ())
			{
				if (this.Contact.Person.Employee.IsNotNull ())
				{
					var employee = this.Contact.Person.Employee;
					return employee.EmployeeJobs.Any (j => j.Office.ParishGroupPathCache == office.ParishGroupPathCache);
				}
			}

			return canViewDetails;
		}

		public bool IsParishLevelUser()
		{
			var isNotHighLevelEditor = !(this.EnableGroupEditionRegion || this.EnableGroupEditionCanton);
			var isNotAdmin           = !this.HasPowerLevel (UserPowerLevel.Administrator);
			var isParishLevel        = (this.Role.Name == AiderUserRoleEntity.ParishRole);
			return isNotHighLevelEditor && isNotAdmin && isParishLevel;
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
				var oldOffice = AiderOfficeManagementEntity.Find (businessContext, currentParish);
				if(oldOffice.IsNotNull ())
				{
					AiderOfficeManagementEntity.LeaveOfficeUsers (businessContext, oldOffice, this);
				}

				var office = AiderOfficeManagementEntity.Find (businessContext, group);
				if (office.IsNotNull ())
				{
					AiderOfficeManagementEntity.JoinOfficeUsers (businessContext, office, this, false);
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
			AiderContactEntity contact, 
			AiderUserRoleEntity role)
		{
			var login = AiderUserEntity.BuildLoginName (businessContext, contact.Person);
			var user = businessContext.CreateAndRegisterEntity<AiderUserEntity> ();

			user.LoginName = login;
			user.DisplayName = AiderUserEntity.BuildDisplayName (contact.Person);
			user.Role = role;
			user.Parish = contact.Person.ParishGroup;
			user.Contact = contact;

			return user;
		}

		public static string BuildLoginName (BusinessContext businessContext, AiderPersonEntity person)
		{
			var initial = person.eCH_Person.PersonFirstNames.Substring (0, 1).ToLower ();
			var name    = person.eCH_Person.PersonOfficialName.ToLower ();
			var desiredUsername = initial + "." + name;

			var checkExample = new AiderUserEntity ()
			{
				LoginName = desiredUsername
			};
			var count = businessContext.GetByExample<AiderUserEntity> (checkExample).Count;
			if(count == 0)
			{
				return desiredUsername;
			}
			else
			{
				var next = count + 1;
				return desiredUsername = initial + "." + name + next.ToString ();
			}
		}

		public static string BuildDisplayName(AiderPersonEntity person)
		{
			return person.eCH_Person.PersonFirstNames + " " + person.eCH_Person.PersonOfficialName;
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
