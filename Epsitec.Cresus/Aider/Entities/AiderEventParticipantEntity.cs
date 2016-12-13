//	Copyright � 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP,

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;
using System.Linq;
using System.Collections.Generic;
using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Common.Support;

namespace Epsitec.Aider.Entities
{
	public partial class AiderEventParticipantEntity
	{
		public override FormattedText GetSummary()
		{
			var role = this.GetRoleCaption ();
			var personSummary = new FormattedText (
				this.GetFullName () + "\n" +
				this.GetBirthDate () + "\n" +
				this.GetTown () + "\n" +
				this.GetParishName ());
			return TextFormatter.FormatText (role, "\n", personSummary);
			
		}

		public override FormattedText GetCompactSummary()
		{
			var role = this.GetRoleCaption ();
			if (this.IsExternal == true)
			{
				return this.GetSummary ();
			}
			else
			{
				return TextFormatter.FormatText (role + ": " + this.Person.GetCompactSummary ());
			}		
		}

		public static AiderEventParticipantEntity Create(BusinessContext context, AiderEventEntity targetEvent, AiderPersonEntity person, Enumerations.EventParticipantRole role)
		{
			var newParticipant = context.CreateAndRegisterEntity<AiderEventParticipantEntity> ();
			newParticipant.Event = targetEvent;
			newParticipant.Role = role;
			newParticipant.Person = person;
			newParticipant.ParishGroupPathCache = targetEvent.ParishGroupPathCache;
            newParticipant.UpdateActDataFromModel ();
			return newParticipant;
		}

		public static AiderEventParticipantEntity CreateForExternal(
			BusinessContext context, 
			AiderEventEntity targetEvent, 
			string firstName, 
			string lastName, 
			Date? birthDate,
			string town, 
			string parishName, 
			Enumerations.PersonConfession confession,
			Enumerations.EventParticipantRole role)
		{
			var newParticipant         = context.CreateAndRegisterEntity<AiderEventParticipantEntity> ();
			newParticipant.Event       = targetEvent;
			newParticipant.Role        = role;
			newParticipant.IsExternal  = true;
			newParticipant.FirstName   = firstName;
			newParticipant.LastName    = lastName;
			newParticipant.Town        = town;
			newParticipant.ParishName  = parishName;
			newParticipant.BirthDate   = birthDate;
			newParticipant.Confession  = confession;
			newParticipant.ParishGroupPathCache = targetEvent.ParishGroupPathCache;
			return newParticipant;
		}

		public void Delete(BusinessContext context)
		{
			context.DeleteEntity (this);
		}

		public string GetLastName(bool fromModel = false)
		{
			if (this.IsExternal || (this.Event.State == Enumerations.EventState.Validated && fromModel == false))
			{
				return this.LastName;
			}
			else
			{
                if (this.LastName.IsNullOrWhiteSpace ())
                {
                    return this.Person.eCH_Person.PersonOfficialName;
                }
                else
                {
                    return this.LastName;
                }
			}
		}

		public string GetFirstName(bool fromModel = false)
		{
			if (this.IsExternal || (this.Event.State == Enumerations.EventState.Validated && fromModel == false))
			{
				return this.FirstName;
			}
			else
			{
                if (this.FirstName.IsNullOrWhiteSpace ())
                {
                    return this.Person.eCH_Person.PersonFirstNames;
                }
                else
                {
                    return this.FirstName;
                }
				
			}
		}

		public string GetFullName(bool fromModel = false)
		{
			if (this.IsExternal || (this.Event.State == Enumerations.EventState.Validated && fromModel == false))
			{
				return StringUtils.Join (" ", this.FirstName, this.LastName);
			}
			else
			{
                if (this.FirstName.IsNullOrWhiteSpace () || this.LastName.IsNullOrWhiteSpace ())
                {
                    return this.Person.GetFullName ();
                }
                else
                {
                    return StringUtils.Join (" ", this.FirstName, this.LastName);
                }
				
			}
		}

		public Date? GetBirthDate(bool fromModel = false)
		{
			if (this.IsExternal || (this.Event.State == Enumerations.EventState.Validated && fromModel == false))
			{
				return this.BirthDate;
			}
			else
			{
                return this.Person.eCH_Person.PersonDateOfBirth;
			}
			
		}

		public Enumerations.PersonSex GetSex(bool fromModel = false)
		{
			if (this.IsExternal || (this.Event.State == Enumerations.EventState.Validated && fromModel == false))
			{
				if (this.Sex == Enumerations.PersonSex.Unknown)
				{
					return this.DetermineSexFromRole (this.Sex);
				}
				else
				{
					return this.Sex;
				}
			}
			else
			{
				if (this.Person.eCH_Person.PersonSex == Enumerations.PersonSex.Unknown)
				{
					return this.DetermineSexFromRole (this.Sex);
				}
				else
				{
					return this.Person.eCH_Person.PersonSex;
				}
				
			}
		}

		public Enumerations.PersonSex DetermineSexFromRole (Enumerations.PersonSex defaultValue)
		{
			switch (this.Role)
			{
				case Enumerations.EventParticipantRole.Spouse:
				case Enumerations.EventParticipantRole.SpouseMother:
				case Enumerations.EventParticipantRole.PartnerBMother:
				case Enumerations.EventParticipantRole.PartnerAMother:
				case Enumerations.EventParticipantRole.Mother:
				case Enumerations.EventParticipantRole.HusbandMother:
				case Enumerations.EventParticipantRole.GodMother:
					return Enumerations.PersonSex.Female;
				case Enumerations.EventParticipantRole.SpouseFather:
				case Enumerations.EventParticipantRole.PartnerBFather:
				case Enumerations.EventParticipantRole.PartnerAFather:
				case Enumerations.EventParticipantRole.HusbandFather:
				case Enumerations.EventParticipantRole.GodFather:
				case Enumerations.EventParticipantRole.Father:
				case Enumerations.EventParticipantRole.Husband:
					return Enumerations.PersonSex.Male;
				default:
					return defaultValue;
			}
		}

		public string GetTown(bool fromModel = false)
		{
			if (this.IsExternal || (this.Event.State == Enumerations.EventState.Validated && fromModel == false))
			{
				return this.Town;
			}
			else
			{
                var person = this.Person;
                if (person.IsGovernmentDefined && person.IsDeclared)
                {
                    return person.eCH_Person.GetAddress ().Town;
                }
                else
                {
                    if (person.MainContact.IsNotNull ())
                    {
                        if (person.MainContact.GetAddress ().Town.IsNotNull ())
                        {
                            return person.MainContact.GetAddress ().Town.Name;
                        }
                        else
                        {
                            return person.MainContact.Address.Town.Name;
                        }
                    }
                    else
                    {
                        return "";
                    }
                }
            }			
		}

		public string GetParishName(bool fromModel = false)
		{
			if (this.IsExternal || (this.Event.State == Enumerations.EventState.Validated && fromModel == false))
			{
				return this.ParishName;
			}
			else
			{
                if (this.ParishName.IsNullOrWhiteSpace ())
                {
                    return this.Person.ParishGroup.Name;
                }
                else
                {
                    return this.ParishName;
                }
			}
		}

		public Enumerations.PersonConfession GetConfession(bool fromModel = false)
		{
			if (this.IsExternal || (this.Event.State == Enumerations.EventState.Validated && fromModel == false))
			{
				return this.Confession;
			}
			else
			{
                if (this.Confession == Enumerations.PersonConfession.Unknown)
                {
                    return this.Person.Confession;
                }
				else
                {
                    return this.Confession;
                }
			}
		}

		public void UpdateActDataFromModel()
		{
			this.FirstName  = this.GetFirstName (true);
			this.LastName   = this.GetLastName (true);
			this.Sex        = this.GetSex (true);
			this.BirthDate  = this.GetBirthDate (true);
			this.Town       = this.GetTown (true);
			this.ParishName = this.GetParishName (true);
			this.Confession = this.GetConfession (true);
		}

		public string GetRoleCaption()
		{
			return Res.Types.Enum.EventParticipantRole.FindValueFromEnumValue (this.Role).Caption.DefaultLabel;
		}
	}
}

