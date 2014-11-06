//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Aider.Enumerations;

using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Text;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Aider.Helpers;
using Epsitec.Cresus.Core.Metadata;
using Epsitec.Cresus.DataLayer.Loader;


namespace Epsitec.Aider.Entities
{
	public partial class AiderMailingParticipantEntity
	{

		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText
			(
				this.Contact.DisplayName
			);
		}

		public override FormattedText GetSummary()
		{

			return TextFormatter.FormatText (this.Contact.DisplayName, "\n", this.Contact.DisplayZipCode, this.Contact.DisplayAddress);
		}

		public static IEnumerable<AiderMailingParticipantEntity> GetAllParticipants(DataContext context, AiderMailingEntity mailing)
		{
			var participantExample = new AiderMailingParticipantEntity ()
			{
				Mailing    = mailing,
			};

			return context.GetByExample<AiderMailingParticipantEntity> (participantExample);
		}

		public static IEnumerable<AiderMailingParticipantEntity> GetAllUnExcludedParticipants(DataContext context, AiderMailingEntity mailing)
		{
			var participantExample = new AiderMailingParticipantEntity ()
			{
				Mailing    = mailing,
				IsExcluded = false,
			};

			return context.GetByExample<AiderMailingParticipantEntity> (participantExample);
		}

		public static IEnumerable<AiderMailingParticipantEntity> GetAllExcludedParticipants(DataContext context, AiderMailingEntity mailing)
		{
			var participantExample = new AiderMailingParticipantEntity ()
			{
				Mailing    = mailing,
				IsExcluded = true,
			};

			return context.GetByExample<AiderMailingParticipantEntity> (participantExample);
		}

		public static AiderMailingParticipantEntity Create(BusinessContext context, AiderMailingEntity mailing, AiderContactEntity contact)
		{
			var participant = context.CreateAndRegisterEntity<AiderMailingParticipantEntity> ();

			participant.Contact = contact;
			participant.Mailing = mailing;

			participant.ParticipantType = MailingParticipantType.Contact;
			

			return participant;
		}

		public static AiderMailingParticipantEntity CreateForGroup(BusinessContext context, AiderMailingEntity mailing, AiderGroupParticipantEntity participation, string role)
		{
			var participant = context.CreateAndRegisterEntity<AiderMailingParticipantEntity> ();

			participant.Mailing            = mailing;
			participant.Contact			   = participation.Contact;
			participant.GroupParticipation = participation;
			participant.ParticipantType    = MailingParticipantType.Group;
			

			return participant;
		}

		public static AiderMailingParticipantEntity Create(BusinessContext context, AiderMailingEntity mailing, AiderHouseholdEntity household)
		{
			if (!household.Contacts.Any ())
			{
				AiderHouseholdEntity.DeleteEmptyHouseholds (context, household);
				return null;
			}

			var participant = context.CreateAndRegisterEntity<AiderMailingParticipantEntity> ();

			participant.Mailing  = mailing;
			participant.Contact  = household.Contacts.First ();
			participant.Houshold = household;

			participant.ParticipantType = MailingParticipantType.Household;

			return participant;
		}

		public static IEnumerable<AiderMailingParticipantEntity> Create(BusinessContext context, AiderMailingEntity mailing, AiderGroupEntity group)
		{
			var created = new List<AiderMailingParticipantEntity> ();
			foreach (var participation in group.GetAllGroupAndSubGroupParticipations ().Distinct())
			{
				var participant = context.CreateAndRegisterEntity<AiderMailingParticipantEntity> ();
				participant.Mailing = mailing;
				participant.Contact = participation.Contact;
				participant.GroupParticipation = participation;
				participant.ParticipantType = MailingParticipantType.Group;

				created.Add (participant);
			}


			return created;
		}

		public static IEnumerable<AiderMailingParticipantEntity> Create(BusinessContext context, AiderMailingEntity mailing, AiderGroupExtractionEntity group)
		{
			var created = new List<AiderMailingParticipantEntity> ();
			foreach (var participation in group.GetAllParticipations (context.DataContext).Distinct ())
			{
				var participant = context.CreateAndRegisterEntity<AiderMailingParticipantEntity> ();

				participant.Mailing			   = mailing;
				participant.Contact			   = participation.Contact;
				participant.GroupParticipation = participation;
				participant.ParticipantType    = MailingParticipantType.GroupExtraction;
				created.Add (participant);
			}

			return created;
		}

		public static IEnumerable<AiderMailingParticipantEntity> FindGroupsByContact(BusinessContext businessContext, AiderMailingEntity mailing, AiderContactEntity contact)
		{
			var participantExample = new AiderMailingParticipantEntity ();
			participantExample.Mailing = mailing;
			participantExample.Contact = contact;
			participantExample.ParticipantType = MailingParticipantType.Group;

			return businessContext.DataContext.GetByExample<AiderMailingParticipantEntity> (participantExample);
		}

		public static void FindAndRemove(BusinessContext businessContext, AiderMailingEntity mailing, AiderContactEntity contact)
		{
			var participantExample = new AiderMailingParticipantEntity ();
			participantExample.Mailing = mailing;
			participantExample.Contact = contact;

			var results = businessContext.DataContext.GetByExample<AiderMailingParticipantEntity> (participantExample);
			foreach (var participant in results)
			{
				businessContext.DeleteEntity (participant);
			}
		}

		public static void FindAndRemove(BusinessContext businessContext, AiderMailingEntity mailing, AiderContactEntity contact,MailingParticipantType type)
		{
			var participantExample = new AiderMailingParticipantEntity ();
			participantExample.Mailing = mailing;
			participantExample.Contact = contact;
			participantExample.ParticipantType = type;

			var results = businessContext.DataContext.GetByExample<AiderMailingParticipantEntity> (participantExample);
			foreach(var participant in results)
			{
				businessContext.DeleteEntity (participant);
			}
		}

		public static void FindAndRemove(BusinessContext businessContext, AiderMailingEntity mailing, AiderHouseholdEntity household)
		{
			var participantExample = new AiderMailingParticipantEntity ()
			{
				Mailing = mailing,
				Houshold = household,
			};

			foreach (var participant in businessContext.DataContext.GetByExample<AiderMailingParticipantEntity> (participantExample))
			{
				businessContext.DeleteEntity (participant);
			}
		}

		public static void ExcludeContact(BusinessContext businessContext, AiderMailingEntity mailing, AiderContactEntity contact)
		{
			var participantExample = new AiderMailingParticipantEntity ()
			{
				Mailing = mailing,
				Contact = contact,
			};

			foreach (var item in businessContext.DataContext.GetByExample<AiderMailingParticipantEntity> (participantExample))
			{
				item.IsExcluded = true;
			}
		}

		public static void UnExcludeContact(BusinessContext businessContext, AiderMailingEntity mailing, AiderContactEntity contact)
		{
			var participantExample = new AiderMailingParticipantEntity ()
			{
				Mailing = mailing,
				Contact = contact,
			};

			foreach (var item in businessContext.DataContext.GetByExample<AiderMailingParticipantEntity> (participantExample))
			{
				item.IsExcluded = false;
			}
		}

		public static void DeleteByMailing(BusinessContext businessContext, AiderMailingEntity mailing)
		{
			var participantExample = new AiderMailingParticipantEntity ()
			{
				Mailing = mailing
			};

			foreach (var participant in businessContext.DataContext.GetByExample<AiderMailingParticipantEntity> (participantExample))
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
