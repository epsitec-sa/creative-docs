//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
				return this.Person.eCH_Person.PersonOfficialName;
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
				return this.Person.eCH_Person.PersonFirstNames;
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
				return this.Person.GetFullName ();
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
				return this.Sex;
			}
			else
			{
				return this.Person.eCH_Person.PersonSex;
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
						return person.MainContact.GetAddress ().Town.Name;
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
				return this.Person.ParishGroup.Name;
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
				return this.Person.Confession;
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

