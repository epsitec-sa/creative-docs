//	Copyright © 2012-2019, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Enumerations;
using Epsitec.Aider.Override;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Business.UserManagement;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.DataLayer.Context;
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
                TextFormatter.FormatText ("Privilège: ").ApplyBold (), this.PowerLevel, "\n",
//				TextFormatter.FormatText ("administrateur: ").ApplyBold (), this.HasPowerLevel (UserPowerLevel.Administrator).ToYesOrNo (), "\n",
				TextFormatter.FormatText ("Actif: ").ApplyBold (), this.IsActive.ToYesOrNo (), "\n",
				TextFormatter.FormatText ("Dernier login: ").ApplyBold (), this.LastLoginDate.ToLocalTime (), "\n",
				TextFormatter.FormatText ("Dernier accès: ").ApplyBold (), this.LastActivityDate.ToLocalTime (), "\n"
			);
		}

		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.DisplayName);
		}

        public override EntityStatus GetEntityStatus()
        {
            using (var a = new EntityStatusAccumulator ())
            {
                a.Accumulate (this.Contact.GetEntityStatus ());
                a.Accumulate (this.Role.GetEntityStatus ());
                a.Accumulate (this.Email.GetEntityStatus ());

                return a.EntityStatus;
            }
        }


        public FormattedText GetSenderAddressLabelText ()
		{
			var sender = this.OfficeSender;
			if (sender.IsNull ())
			{
				var office = this.Office;
				if (office.IsNull ())
				{
					throw new BusinessRuleException ("Votre utilisateur n'est pas associé à une gestion, impossible d'utiliser un expéditeur");
				}
				else
				{
					var mainContactText = office.OfficeMainContact.GetAddressLabelText ();
					if (mainContactText.IsNullOrEmpty ())
					{
						throw new BusinessRuleException (
							string.Format ("Le contact principal de votre gestion ({0}) est mal configuré, impossible d'utiliser cet expéditeur", office.OfficeName)
						);
					}
					return mainContactText;
				}

			}
			var senderText = sender.OfficialContact.GetAddressLabelText ();
			if (senderText.IsNullOrEmpty ())
			{
				throw new BusinessRuleException ("Le contact pour votre expéditeur est mal configuré, impossible d'utiliser cet expéditeur");
			}
			return senderText;
		}


        public void AssignUserGroups(BusinessContext businessContext, UserPowerLevel powerLevel)
        {
            this.AssignUserGroups (businessContext.DataContext, powerLevel);
        }

        public void AssignUserGroups(DataContext dataContext, UserPowerLevel powerLevel)
        {
            //	Setting the power level picks the proper user group and associates it with
            //	the user; unless the level is more restricted than "standard", we should
            //	always include the standard user level too.

            var groups = AiderUserEntity
                .GetSoftwareUserGroups (dataContext, powerLevel)
                .Where (x => x != null)
                .ToList ();

            this.UserGroups.Clear ();
            this.UserGroups.AddRange (groups);
		}

		public IEnumerable<AiderOfficeManagementEntity> GetOfficesByJobs()
		{
			if (this.Contact.IsNotNull ())
			{
				if (this.Contact.Person.Employee.IsNotNull ())
				{
					var employee = this.Contact.Person.Employee;
					return employee.EmployeeJobs.Where (j => j.Office.IsNotNull ()).Select (j => j.Office);
				}
			}

			return Enumerable.Empty<AiderOfficeManagementEntity> ();
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
                        case Enumerations.EmployeeType.AnimateurEglise:
//                      case Enumerations.EmployeeType.AnimateurParoisse:
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

		public bool CanViewConfidentialAddress()
		{
			return (this.IsPowerUser () || this.IsAdmin ());
		}

		public bool CanViewOfficeDetails()
		{
			return (this.IsPowerUser () || this.IsAdmin ());
		}

		public bool CanRemoveMailing()
		{
			return (this.IsPowerUser () || this.IsAdmin ());
		}

		public bool CanEditEmployee()
		{
			return (this.IsPowerUser () || this.IsAdmin ());
		}

		public bool CanBypassSubscriptionCheck()
		{
			return (this.IsPowerUser () || this.IsAdmin ());
		}

		public bool CanEditReferee()
		{
			return ((this.Role.Name == AiderUserRoleEntity.RegionRole) || 
					this.IsAdmin ())
					&& this.IsOfficeDefined ();
		}

		public bool CanDerogateTo(AiderGroupEntity derogationParishGroup)
		{
			if ((this.IsPowerUser ()) || this.HasPowerLevel (UserPowerLevel.Administrator))
			{
				return true;
			}
			else
			{
				if (this.Office.IsNotNull ())
				{
					if (derogationParishGroup.Name == this.Office.ParishGroup.Name)
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
			if (canViewDetails)
			{
				return true;
			}

			if (this.Contact.IsNotNull ())
			{
				if (this.Contact.Person.Employee.IsNotNull ())
				{
					var employee = this.Contact.Person.Employee;
					return employee.EmployeeJobs.Any (j => j.Office.ParishGroupPathCache == office.ParishGroupPathCache);
				}
			}

			return false;
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
            return this.HasPowerLevel (UserPowerLevel.AdminUser);
        }

        public bool IsSysAdmin()
        {
            return this.HasPowerLevel (UserPowerLevel.Administrator);
        }

        public bool IsPowerUser()
		{
			return (this.Role.Name == AiderUserRoleEntity.AleRole)
                || (this.Role.Name == AiderUserRoleEntity.PowerRole)
                || (this.Role.Name == AiderUserRoleEntity.AdminRole)
                || (this.IsAdmin ());
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

		public void SetAdministrator(BusinessContext businessContext, bool admin)
		{
			var powerLevel = UserPowerLevel.Administrator;

			var isAdmin = this.HasPowerLevel (powerLevel);

			if (!isAdmin && admin)
			{
				this.AssignUserGroups (businessContext, powerLevel);
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
            var contact = this.Contact;

            if (contact.IsNotNull ())
            {
                var employee = contact.Person.Employee;

                if (employee.IsNotNull ())
                {
                    var userJobsToRemove = employee
                        .EmployeeJobs
                        .Where (j => j.EmployeeJobFunction == EmployeeJobFunction.UtilisateurAIDER)
                        .ToList ();

                    var suppleantJobsToRemove = employee
                        .EmployeeJobs
                        .Where (j => j.EmployeeJobFunction == EmployeeJobFunction.SuppléantAIDER)
                        .ToList ();

                    var officeManagerJobsToRemove = employee
                        .EmployeeJobs
                        .Where (j => j.EmployeeJobFunction == EmployeeJobFunction.GestionnaireAIDER)
                        .ToList ();

                    var jobsToRemove = userJobsToRemove
                        .Concat (suppleantJobsToRemove)
                        .Concat (officeManagerJobsToRemove)
                        .ToList ();
                    
                    foreach (var userJob in userJobsToRemove)
                    {
                        AiderOfficeManagementEntity.LeaveOfficeUsers (businessContext, userJob.Office, this);
                    }

                    foreach (var suppleantJob in suppleantJobsToRemove)
                    {
                        AiderOfficeManagementEntity.LeaveOfficeSuppleants (businessContext, suppleantJob.Office, this);
                    }

                    foreach (var managerJob in officeManagerJobsToRemove)
                    {
                        AiderOfficeManagementEntity.LeaveOfficeManagement (businessContext, managerJob.Office, this, true);
                    }

                    foreach (var job in employee.EmployeeJobs)
                    {
                        job.Delete (businessContext);
                    }

                    businessContext.DeleteEntity (employee);
                }
            }


            this.CustomUISettings.Delete (businessContext);

			businessContext.DeleteEntity (this);
		}

		public static AiderUserEntity Create(BusinessContext businessContext, AiderContactEntity contact, AiderUserRoleEntity role)
		{
            var person = contact.Person;
            var login  = AiderUserEntity.BuildLoginName (businessContext, person);
			var user   = businessContext.CreateAndRegisterEntity<AiderUserEntity> ();

            user.LoginName   = login;
			user.DisplayName = AiderUserEntity.BuildDisplayName (person);
			user.Role        = role;
			user.Parish      = person.ParishGroup;
			user.Contact     = contact;
            user.Email       = person.MainEmail;

            AiderUserEntity.CreateEmployee (businessContext, user);

			return user;
		}
        
        private static void CreateEmployee(BusinessContext businessContext, AiderUserEntity user)
        {
            var contact = user.Contact;
            var person  = contact.Person;

            if (person.Employee.IsNotNull ())
            {
                return;
            }

            var employee = AiderEmployeeEntity
                .Create (businessContext,
                    person, user, Enumerations.EmployeeType.BenevoleAIDER,
                    function: "", Enumerations.EmployeeActivity.None, navs13: "");

            if (user.Parish.IsNotNull ())
            {
                var officeExemple = new AiderOfficeManagementEntity
                {
                    ParishGroup = user.Parish
                };

                var office = businessContext
                    .GetByExample<AiderOfficeManagementEntity> (officeExemple)
                    .FirstOrDefault ();

                if ((office.IsNotNull ()) &&
                    (office.UserJobExistFor (user)))
                {
                    AiderEmployeeJobEntity.CreateOfficeUser (businessContext, employee, office, detail: "");
                }
            }
        }
        
        private static string BuildLoginName (BusinessContext businessContext, AiderPersonEntity person)
		{
			var initial = person.eCH_Person.PersonFirstNames.Substring (0, 1).ToLower ();
			var name    = person.eCH_Person.PersonOfficialName.ToLower ();
            var index   = 0;

            while (true)
            {
                var loginName = index == 0 ? $"{initial}.{name}" : $"{initial}.{name}{index}";

                var checkExample = new AiderUserEntity ()
                {
                    LoginName = loginName
                };

                if (businessContext.GetByExample<AiderUserEntity> (checkExample).Any ())
                {
                    index++;
                    continue;
                }
                else
                {
                    return loginName;
                }
			}
		}

		private static string BuildDisplayName(AiderPersonEntity person)
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
            var dataContext = this.GetDataContext ();
            var callingUser = AiderUserManager.Current.AuthenticatedUser;

            if (dataContext == null)
            {
                throw new System.Exception ("Don't set power level of user for example queries; it won't work as this is a synthetic property");
            }

            if ((callingUser.IsNotNull ()) &&
                (callingUser.IsSysAdmin () == false))
            {
                if ((callingUser.IsAdmin ()) &&
                    (this.PowerLevel >= UserPowerLevel.AdminUser) &&
                    (value >= UserPowerLevel.AdminUser))
                {
                    //  OK, accept that the caller changes the value...
                }
                else
                {
                    return;
                }
            }

            this.AssignUserGroups (dataContext, value);
		}
		
		
		
		private static IEnumerable<SoftwareUserGroupEntity> GetSoftwareUserGroups(DataContext dataContext, UserPowerLevel powerLevel)
		{
            //	Tthe power level picks the proper user group; unless the level is
            //  more restricted than "standard", we always include the standard user
            //  level too.

            if (powerLevel == UserPowerLevel.None)
            {
                yield break;
            }

            if (powerLevel < UserPowerLevel.Restricted)
            {
                var example = new SoftwareUserGroupEntity
                {
                    UserPowerLevel = UserPowerLevel.Standard
                };

                yield return dataContext
                    .GetByExample (example)
                    .Single ();
            }

            if (powerLevel != UserPowerLevel.Standard)
            {
                var example = new SoftwareUserGroupEntity
                {
                    UserPowerLevel = powerLevel
                };

                var result = dataContext
                    .GetByExample (example)
                    .FirstOrDefault ();

                if (result == null)
                {
                    //  TODO: fix the database, missing power level description found
                }

                yield return result;
            }
		}
	}
}
