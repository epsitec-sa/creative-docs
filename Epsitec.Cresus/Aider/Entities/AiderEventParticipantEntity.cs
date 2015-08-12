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

namespace Epsitec.Aider.Entities
{
	public partial class AiderEventParticipantEntity
	{
		public override FormattedText GetSummary()
		{
			var role = this.GetRoleCaption ();
			if (this.IsExternal == true)
			{
				var personSummary = this.FirstName + " " + this.LastName;
				return TextFormatter.FormatText (role + ": " + personSummary);
			}
			else
			{
				return TextFormatter.FormatText (role + ": " + this.Person.GetSummary ());
			}
			
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

		public static AiderEventParticipantEntity CreateForExternal(BusinessContext context, AiderEventEntity targetEvent, string firstName, string lastName, string town, Enumerations.EventParticipantRole role)
		{
			var newParticipant         = context.CreateAndRegisterEntity<AiderEventParticipantEntity> ();
			newParticipant.Event       = targetEvent;
			newParticipant.Role        = role;
			newParticipant.IsExternal  = true;
			newParticipant.FirstName   = firstName;
			newParticipant.LastName    = lastName;
			newParticipant.Town        = town;
			return newParticipant;
		}

		public void Delete(BusinessContext context)
		{
			context.DeleteEntity (this);
		}

		private string GetRoleCaption()
		{
			return Res.Types.Enum.EventParticipantRole.FindValueFromEnumValue (this.Role).Caption.DefaultLabel;
		}
	}
}

