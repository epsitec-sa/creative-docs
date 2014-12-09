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
			return TextFormatter.FormatText (this.Person.GetSummary ());
		}

		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.Person.GetCompactSummary ());
		}

		public AiderEventParticipantEntity Create(BusinessContext context, AiderPersonEntity person, Enumerations.EventParticipantRole role)
		{
			var newParticipant = context.CreateAndRegisterEntity<AiderEventParticipantEntity> ();
			newParticipant.Role = role;
			newParticipant.Person = person;
			return newParticipant;
		}

		public void Delete(BusinessContext context)
		{
			context.DeleteEntity (this);
		}
	}
}

