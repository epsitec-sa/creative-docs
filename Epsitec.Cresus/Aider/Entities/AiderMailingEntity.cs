//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

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
	public partial class AiderMailingEntity
	{
		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText
			(
				this.Name
			);
		}


		public override FormattedText GetSummary()
		{
			FormattedText summary = new FormattedText (this.Name);
			if (this.IsReady)
			{
				summary += ("\nActif");
			}
			else
			{
				summary += ("\nInactif");
			}

			return summary;
		}

		public void RefreshCache()
		{
			
		}

		public void SyncParticipants(BusinessContext businessContext)
		{

			var participants = AiderMailingParticipantEntity.GetAllParticipants (businessContext, this).Select( p => p.Contact);
			var recipientsDict = this.GetRecipients().ToDictionary(k => k);
			//Remove missing
			foreach (var contact in participants)
			{
				if (!recipientsDict.ContainsKey (contact))
				{
					AiderMailingParticipantEntity.FindAndRemove (businessContext, this, contact);
				}
			}

			var participantsDict = AiderMailingParticipantEntity.GetAllParticipants (businessContext, this).ToDictionary(k => k.Contact);
			//Add missing
			foreach (var contact in this.GetRecipients ())
			{
				if (!participantsDict.ContainsKey (contact))
				{
					AiderMailingParticipantEntity.Create (businessContext, this, contact);
				}
			}

			businessContext.SaveChanges (LockingPolicy.KeepLock);
		}

		public void AddGroup(BusinessContext businessContext, AiderGroupEntity groupToAdd)
		{
			if (!this.RecipientGroups.Contains (groupToAdd))
			{
				this.RecipientGroups.Add (groupToAdd);
				AiderMailingParticipantEntity.Create (businessContext, this, groupToAdd);
			}
		}

		public void AddContact(BusinessContext businessContext, AiderContactEntity contactToAdd)
		{
			if(!this.RecipientContacts.Contains(contactToAdd))
			{
				this.RecipientContacts.Add (contactToAdd);
				AiderMailingParticipantEntity.Create (businessContext, this, contactToAdd);
			}
		}

		public void AddContacts(BusinessContext businessContext, IEnumerable<AiderContactEntity> contactsToAdd)
		{
			foreach (var contact in contactsToAdd)
			{
				if (!this.RecipientContacts.Contains (contact))
				{
					this.RecipientContacts.Add (contact);
					AiderMailingParticipantEntity.Create (businessContext, this, contact);
				}
			}
		}

		public void ExludeContact(BusinessContext businessContext, AiderContactEntity contactToExclude)
		{
			if (!this.Exclusions.Contains (contactToExclude))
			{
				this.Exclusions.Add (contactToExclude);
				AiderMailingParticipantEntity.FindAndRemove (businessContext, this, contactToExclude);
			}
		}

		public void ExludeContacts(BusinessContext businessContext, IEnumerable<AiderContactEntity> contactsToExclude)
		{		
			foreach (var contact in contactsToExclude)
			{
				if (!this.Exclusions.Contains (contact))
				{
					this.Exclusions.Add(contact);
					AiderMailingParticipantEntity.FindAndRemove (businessContext, this, contact);
				}
			}
		}

		public void UnExludeContacts(BusinessContext businessContext, IEnumerable<AiderContactEntity> contactsToUnExclude)
		{
			this.Exclusions.RemoveAll(r => contactsToUnExclude.Contains(r));
			foreach (var contact in contactsToUnExclude)
			{
				AiderMailingParticipantEntity.Create (businessContext, this, contact);
			}
		}

		public FormattedText GetRecipientsSummary()
		{
			var recipients = this.GetRecipients ()
				.Select (r => r.GetCompactSummary ())
				.CreateSummarySequence (10, "...");

			return FormattedText.Join (FormattedText.FromSimpleText ("\n"), recipients);
		}

		public FormattedText GetExclusionsSummary()
		{
			var recipients = this.Exclusions
				.Select (r => r.GetCompactSummary ())
				.CreateSummarySequence (10, "...");

			return FormattedText.Join (FormattedText.FromSimpleText ("\n"), recipients);
		}

		public IList<AiderContactEntity> GetRecipients()
		{
			if (this.recipientsCache == null)
			{
				var contacts = this.RecipientContacts;

				foreach (var group in this.RecipientGroups)
				{
					contacts.AddRange (group.GetAllGroupAndSubGroupParticipants ());
				}

				contacts.RemoveAll (c => this.Exclusions.Contains (c));

				this.recipientsCache = contacts.Distinct().ToList();
			}

			return this.recipientsCache;
		}

		public static AiderMailingEntity Create(BusinessContext context, SoftwareUserEntity creator, string name, bool isReady)
		{
			var mailing = context.CreateAndRegisterEntity<AiderMailingEntity> ();

			var aiderUserExample = new AiderUserEntity()
			{
				People = creator.People
			};
			mailing.Name = name;
			mailing.IsReady = isReady;
			mailing.CreatedBy = context.DataContext.GetByExample<AiderUserEntity> (aiderUserExample).FirstOrDefault ();

			return mailing;
		}


		public static void Delete(BusinessContext businessContext, AiderMailingEntity mailing)
		{
			AiderMailingParticipantEntity.DeleteByMailing (businessContext, mailing);
			businessContext.DeleteEntity (mailing);
		}

		//	These properties are only meant as an in memory cache of the members of the household.
		//	They will never be saved to the database:
		private IList<AiderContactEntity>		recipientsCache;
	}
}
