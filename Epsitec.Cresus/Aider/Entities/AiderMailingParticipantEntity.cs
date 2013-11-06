//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Aider.Enumerations;

using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Text;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.DataLayer.Context;

using System;
using System.Collections.Generic;
using System.Linq;


namespace Epsitec.Aider.Entities
{
	public partial class AiderMailingParticipantEntity
	{


		public static IEnumerable<AiderMailingParticipantEntity> GetAllParticipants(BusinessContext context, AiderMailingEntity mailing)
		{
			var participantExample = new AiderMailingParticipantEntity ();
			participantExample.Mailing = mailing;

			return context.DataContext.GetByExample<AiderMailingParticipantEntity> (participantExample);
		}

		public static AiderMailingParticipantEntity Create(BusinessContext context, AiderMailingEntity mailing, AiderContactEntity contact)
		{
			var participant = context.CreateAndRegisterEntity<AiderMailingParticipantEntity> ();

			participant.Contact = contact;
			participant.Mailing = mailing;

			return participant;
		}

		public static void Create(BusinessContext context, AiderMailingEntity mailing, AiderGroupEntity group)
		{		
			foreach (var contact in group.GetAllGroupAndSubGroupParticipants ().Distinct())
			{
				var participant = context.CreateAndRegisterEntity<AiderMailingParticipantEntity> ();
				participant.Contact = contact;
				participant.Mailing = mailing;
			}
		}

		public static void FindAndRemove(BusinessContext businessContext, AiderMailingEntity mailing, AiderContactEntity contact)
		{
			var participantExample = new AiderMailingParticipantEntity ();
			participantExample.Mailing = mailing;
			participantExample.Contact = contact;

			var results = businessContext.DataContext.GetByExample<AiderMailingParticipantEntity> (participantExample);
			foreach(var participant in results)
			{
				businessContext.DeleteEntity (participant);
			}
		}

		public static void DeleteByMailing(BusinessContext businessContext, AiderMailingEntity mailing)
		{
			var participantExample = new AiderMailingParticipantEntity ();
			participantExample.Mailing = mailing;

			var results = businessContext.DataContext.GetByExample<AiderMailingParticipantEntity> (participantExample);
			foreach (var participant in results)
			{
				businessContext.DeleteEntity (participant);
			}
		}

		public static void Delete(BusinessContext businessContext, AiderMailingParticipantEntity participant)
		{
			//TODO BEFORE DELETE?

			businessContext.DeleteEntity (participant);
		}
	}
}
