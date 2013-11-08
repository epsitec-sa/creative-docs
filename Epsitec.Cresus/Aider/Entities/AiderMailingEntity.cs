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
			return TextFormatter.FormatText (this.Name,"\n",
											 this.Description,"\n",
											 this.GetReadyText (),"\n",
											 "Dernière mise à jour: ",this.LastUpdate.Value.ToString ());
		}


		public FormattedText GetRecipientsOverview()
		{
			return TextFormatter.FormatText ("Destinataires:", "\n",
											 this.RecipientContacts.Count(),"contacts individuel(s)", "\n",
											 this.RecipientGroups.Count (), "groupe(s)", "\n",
											 this.RecipientHouseholds.Count (), "ménage(s)", "\n",
											 "Dernière mise à jour: ", this.LastUpdate.Value.ToString ());
		}

		public string GetReadyText()
		{
			if (this.IsReady)
				return "Prêt pour l'envoi";
			else
				return "En préparation";
		}

		public void RefreshCache()
		{
		
			
		}

		public void RecreateFromScratch(BusinessContext businessContext)
		{
			this.LastUpdate = System.DateTime.Today;

			AiderMailingParticipantEntity.DeleteByMailing (businessContext, this);

			foreach (var contact in this.RecipientContacts)
			{
				AiderMailingParticipantEntity.Create (businessContext, this, contact);
			}

			foreach (var group in this.RecipientGroups)
			{
				AiderMailingParticipantEntity.Create (businessContext, this, group);
			}

			foreach (var household in this.RecipientHouseholds)
			{
				AiderMailingParticipantEntity.Create (businessContext, this, household);
			}

			foreach (var excluded in this.Exclusions)
			{
				AiderMailingParticipantEntity.ExcludeContact (businessContext, this, excluded);
			}
		}

		/// <summary>
		/// Sync groups changes that affect participants dataset
		/// </summary>
		/// <param name="businessContext"></param>
		public void SyncParticipants(BusinessContext businessContext)
		{
			this.LastUpdate = System.DateTime.Today;

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
					AiderMailingParticipantEntity.CreateForGroup (businessContext, this, contact);
				}
			}

			businessContext.SaveChanges (LockingPolicy.KeepLock);
		}

		public void AddHousehold(BusinessContext businessContext, AiderHouseholdEntity householdToAdd)
		{
			if (!this.RecipientHouseholds.Contains (householdToAdd))
			{
				this.LastUpdate = System.DateTime.Today;
				this.RecipientHouseholds.Add (householdToAdd);
				AiderMailingParticipantEntity.Create (businessContext, this, householdToAdd);
			}
		}

		public void AddGroup(BusinessContext businessContext, AiderGroupEntity groupToAdd)
		{
			if (!this.RecipientGroups.Contains (groupToAdd))
			{
				this.LastUpdate = System.DateTime.Today;
				this.RecipientGroups.Add (groupToAdd);
				AiderMailingParticipantEntity.Create (businessContext, this, groupToAdd);
			}
		}

		public void AddContact(BusinessContext businessContext, AiderContactEntity contactToAdd)
		{
			if(!this.RecipientContacts.Contains(contactToAdd))
			{
				this.LastUpdate = System.DateTime.Today;
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
					this.LastUpdate = System.DateTime.Today;
					this.RecipientContacts.Add (contact);
					AiderMailingParticipantEntity.Create (businessContext, this, contact);
				}
			}
		}

		public void RemoveContact(BusinessContext businessContext, AiderContactEntity contactToRemove)
		{
			if (!this.Exclusions.Contains (contactToRemove))
			{
				this.LastUpdate = System.DateTime.Today;
				this.RecipientContacts.Remove (contactToRemove);
				AiderMailingParticipantEntity.FindAndRemove (businessContext, this, contactToRemove);
			}
		}

		public void RemoveHousehold(BusinessContext businessContext, AiderContactEntity contactToRemove,AiderHouseholdEntity householdToRemove)
		{
			if (!this.Exclusions.Contains (contactToRemove))
			{
				this.LastUpdate = System.DateTime.Today;
				this.RecipientHouseholds.Remove (householdToRemove);
				AiderMailingParticipantEntity.FindAndRemove (businessContext, this, contactToRemove);
			}
		}

		public void ExludeContact(BusinessContext businessContext, AiderContactEntity contactToExclude)
		{
			if (!this.Exclusions.Contains (contactToExclude))
			{
				this.LastUpdate = System.DateTime.Today;
				this.Exclusions.Add (contactToExclude);
				AiderMailingParticipantEntity.ExcludeContact (businessContext, this, contactToExclude);
			}
		}

		public void ExludeContacts(BusinessContext businessContext, IEnumerable<AiderContactEntity> contactsToExclude)
		{		
			foreach (var contact in contactsToExclude)
			{
				if (!this.Exclusions.Contains (contact))
				{
					this.LastUpdate = System.DateTime.Today;
					this.Exclusions.Add(contact);
					AiderMailingParticipantEntity.ExcludeContact (businessContext, this, contact);
				}
			}
		}

		public void UnExludeContacts(BusinessContext businessContext, IEnumerable<AiderContactEntity> contactsToUnExclude)
		{
			this.LastUpdate = System.DateTime.Today;
			this.Exclusions.RemoveAll(r => contactsToUnExclude.Contains(r));
			foreach (var contact in contactsToUnExclude)
			{
				AiderMailingParticipantEntity.UnExcludeContact (businessContext, this, contact);
			}
		}

		public FormattedText GetRecipientsTitleSummary()
		{
			return FormattedText.FromSimpleText ("Destinataires (",this.GetRecipients ().Count ().ToString (),")");
		}

		public FormattedText GetRecipientsSummary()
		{
			var recipients = this.GetRecipients ()
				.Select (r => r.GetCompactSummary ())
				.CreateSummarySequence (10, "...");

			return FormattedText.Join (FormattedText.FromSimpleText ("\n"), recipients);
		}

		public FormattedText GetExclusionsTitleSummary()
		{
			return FormattedText.FromSimpleText ("Exclusions (", this.Exclusions.Count ().ToString (), ")");
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

				foreach (var household in this.RecipientHouseholds)
				{
					contacts.Add (household.Contacts[0]);
				}

				contacts.RemoveAll (c => this.Exclusions.Contains (c));

				this.recipientsCache = contacts.Distinct().ToList();
			}

			return this.recipientsCache;
		}

		public static AiderMailingEntity Create(BusinessContext context, AiderUserEntity aiderUser, string name, string desc, AiderMailingCategoryEntity cat, bool isReady)
		{
			var mailing = context.CreateAndRegisterEntity<AiderMailingEntity> ();
			mailing.Name = name;
			mailing.Category = cat;
			mailing.Description = desc;
			mailing.IsReady = isReady;
			mailing.CreatedBy = aiderUser;
			mailing.ParishGroupPathCache = aiderUser.ParishGroupPathCache;
			mailing.LastUpdate = System.DateTime.Today;

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
