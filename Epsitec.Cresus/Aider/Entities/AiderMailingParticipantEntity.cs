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

			return context.DataContext.GetByExample<AiderMailingParticipantEntity> (participantExample).Where(p => !p.IsExclude);
		}

		public static AiderMailingParticipantEntity Create(BusinessContext context, AiderMailingEntity mailing, AiderContactEntity contact)
		{
			var participant = context.CreateAndRegisterEntity<AiderMailingParticipantEntity> ();

			participant.Contact = contact;
			participant.Mailing = mailing;
			participant.ParticipantLetterCode = "C";

			return participant;
		}

		public static AiderMailingParticipantEntity CreateForGroup(BusinessContext context, AiderMailingEntity mailing, AiderContactEntity contact)
		{
			var participant = context.CreateAndRegisterEntity<AiderMailingParticipantEntity> ();

			participant.Contact = contact;
			participant.Mailing = mailing;
			participant.ParticipantLetterCode = "G";

			return participant;
		}

		public static void Create(BusinessContext context, AiderMailingEntity mailing, AiderHouseholdEntity household)
		{
				var participant = context.CreateAndRegisterEntity<AiderMailingParticipantEntity> ();
				participant.Contact = household.Contacts[0];
				participant.Houshold = household;
				participant.Mailing = mailing;
				participant.ParticipantLetterCode = "M";
		}

		public static void Create(BusinessContext context, AiderMailingEntity mailing, AiderGroupEntity group)
		{		
			foreach (var contact in group.GetAllGroupAndSubGroupParticipants ().Distinct())
			{
				var participant = context.CreateAndRegisterEntity<AiderMailingParticipantEntity> ();
				participant.Contact = contact;
				participant.Mailing = mailing;
				participant.ParticipantLetterCode = "G";
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

		public static void ExcludeContact(BusinessContext businessContext, AiderMailingEntity mailing, AiderContactEntity contact)
		{
			var participantExample = new AiderMailingParticipantEntity ();
			participantExample.Mailing = mailing;
			participantExample.Contact = contact;

			var result = businessContext.DataContext.GetByExample<AiderMailingParticipantEntity> (participantExample).First ();
			result.IsExclude = true;
			
		}

		public static void UnExcludeContact(BusinessContext businessContext, AiderMailingEntity mailing, AiderContactEntity contact)
		{
			var participantExample = new AiderMailingParticipantEntity ();
			participantExample.Mailing = mailing;
			participantExample.Contact = contact;

			var result = businessContext.DataContext.GetByExample<AiderMailingParticipantEntity> (participantExample).First ();
			result.IsExclude = false;

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
