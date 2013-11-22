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
											 this.LastUpdate.Value.ToString ());
		}


		public FormattedText GetRecipientsOverview()
		{
			int contactCount         = this.RecipientContacts.Count;
			int groupCount           = this.RecipientGroups.Count;
			int groupExtractionCount = this.RecipientGroupExtractions.Count;
			int householdCount       = this.RecipientHouseholds.Count;

			return TextFormatter.FormatText (contactCount, contactCount > 1 ? "contacts individuels" : "contact individuel", "\n",
				/**/						 groupCount, groupCount > 1 ? "groupes" : "groupe", "\n",
				/**/						 groupExtractionCount, groupExtractionCount > 1 ? " groupes transversaux" : "groupe transversal", "\n",
				/**/						 householdCount, householdCount > 1 ? "ménages" : "ménage", "\n",
				/**/						 this.LastUpdate.Value.ToString ());
		}

		public string GetReadyText()
		{
			return this.IsReady ? "Prêt pour l'envoi" : "En préparation";
		}

		public void RecreateFromScratch(BusinessContext businessContext)
		{
			this.UpdateLastUpdateDate ();

			AiderMailingParticipantEntity.DeleteByMailing (businessContext, this);

			foreach (var contact in this.RecipientContacts)
			{
				AiderMailingParticipantEntity.Create (businessContext, this, contact);
			}

			foreach (var group in this.RecipientGroups)
			{
				AiderMailingParticipantEntity.Create (businessContext, this, group);
			}

			foreach (var groupExtraction in this.RecipientGroupExtractions)
			{
				AiderMailingParticipantEntity.Create (businessContext, this, groupExtraction);
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
		public void UpdateMailingParticipants(BusinessContext businessContext)
		{
			var dataContext = businessContext.DataContext;

			this.UpdateLastUpdateDate ();

			var participants = new HashSet<AiderContactEntity> (AiderMailingParticipantEntity.GetAllParticipants (dataContext, this).Select (p => p.Contact));
			var contacts     = new HashSet<AiderContactEntity> (this.GetRecipients (dataContext));
			
			//	Remove participants which no longer belong to the current contacts:
			foreach (var contact in participants)
			{
				if (!contacts.Contains (contact))
				{
					AiderMailingParticipantEntity.FindAndRemove (businessContext, this, contact);
				}
			}

			//	Add participants which are not yet defined for the current contacts:
			foreach (var contact in contacts)
			{
				if (!participants.Contains (contact))
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
				this.UpdateLastUpdateDate ();
				this.RecipientHouseholds.Add (householdToAdd);
				AiderMailingParticipantEntity.Create (businessContext, this, householdToAdd);
			}
		}

		public void AddGroup(BusinessContext businessContext, AiderGroupEntity groupToAdd)
		{
			if (!this.RecipientGroups.Contains (groupToAdd))
			{
				this.UpdateLastUpdateDate ();
				this.RecipientGroups.Add (groupToAdd);
				AiderMailingParticipantEntity.Create (businessContext, this, groupToAdd);
			}
		}

		public void AddGroupExtraction(BusinessContext businessContext, AiderGroupExtractionEntity groupExtractionToAdd)
		{
			if (!this.RecipientGroupExtractions.Contains (groupExtractionToAdd))
			{
				this.UpdateLastUpdateDate ();
				this.RecipientGroupExtractions.Add (groupExtractionToAdd);
				AiderMailingParticipantEntity.Create (businessContext, this, groupExtractionToAdd);
			}
		}

		public void AddContact(BusinessContext businessContext, AiderContactEntity contactToAdd)
		{
			if(!this.RecipientContacts.Contains(contactToAdd))
			{
				this.UpdateLastUpdateDate ();
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
					this.UpdateLastUpdateDate ();
					this.RecipientContacts.Add (contact);
					AiderMailingParticipantEntity.Create (businessContext, this, contact);
				}
			}
		}

		public void RemoveContact(BusinessContext businessContext, AiderContactEntity contactToRemove)
		{
			this.UpdateLastUpdateDate ();
			this.RecipientContacts.Remove (contactToRemove);
			AiderMailingParticipantEntity.FindAndRemove (businessContext, this, contactToRemove);	
		}

		public void RemoveGroup(BusinessContext businessContext, AiderGroupEntity groupToRemove)
		{
			this.UpdateLastUpdateDate ();
			this.RecipientGroups.Remove (groupToRemove);
			foreach (var contact in groupToRemove.GetAllGroupAndSubGroupParticipants ().Distinct ())
			{
				this.Exclusions.RemoveAll (r => r == contact);
				AiderMailingParticipantEntity.FindAndRemove (businessContext, this, contact);
			}			
		}

		public void RemoveGroupExtraction(BusinessContext businessContext, AiderGroupExtractionEntity groupExtractionToRemove)
		{
			this.UpdateLastUpdateDate ();
			this.RecipientGroupExtractions.Remove (groupExtractionToRemove);

			foreach (var contact in groupExtractionToRemove.GetAllContacts (businessContext.DataContext).Distinct ())
			{
				this.Exclusions.RemoveAll (r => r == contact);
				AiderMailingParticipantEntity.FindAndRemove (businessContext, this, contact);
			}
		}

		public void RemoveHousehold(BusinessContext businessContext, AiderContactEntity contactToRemove, AiderHouseholdEntity householdToRemove)
		{
			this.UpdateLastUpdateDate ();
			this.RecipientHouseholds.Remove (householdToRemove);
			AiderMailingParticipantEntity.FindAndRemove (businessContext, this, contactToRemove);		
		}

		public void RemoveHousehold(BusinessContext businessContext, AiderHouseholdEntity householdToRemove)
		{
			this.UpdateLastUpdateDate ();
			this.RecipientHouseholds.Remove (householdToRemove);
			AiderMailingParticipantEntity.FindAndRemove (businessContext, this, householdToRemove);
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
					this.UpdateLastUpdateDate ();
					this.Exclusions.Add(contact);
					AiderMailingParticipantEntity.ExcludeContact (businessContext, this, contact);
				}
			}
		}

		public void UnExludeContacts(BusinessContext businessContext, IEnumerable<AiderContactEntity> contactsToUnExclude)
		{
			this.UpdateLastUpdateDate ();
			this.Exclusions.RemoveAll(r => contactsToUnExclude.Contains(r));
			foreach (var contact in contactsToUnExclude)
			{
				AiderMailingParticipantEntity.UnExcludeContact (businessContext, this, contact);
			}
		}

		public FormattedText GetRecipientsTitleSummary()
		{
			return FormattedText.FromSimpleText ("Destinataires (", this.GetRecipients ().Count ().ToString (), ")");
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

		public FormattedText GetGroupsTitleSummary()
		{
			return FormattedText.FromSimpleText ("Groupes (", this.RecipientGroups.Count ().ToString (), ")");
		}

		public FormattedText GetGroupsSummary()
		{
			var recipients = this.RecipientGroups
				.Select (r => r.GetCompactSummary ())
				.CreateSummarySequence (10, "...");

			return FormattedText.Join (FormattedText.FromSimpleText ("\n"), recipients);
		}

		public IList<AiderContactEntity> GetRecipients(DataContext context = null)
		{
			if (this.recipientsCache == null)
			{
				if (context == null)
				{
					context = DataContextPool.GetDataContext (this);
				}

				var contacts = new HashSet<AiderContactEntity> ();

				contacts.UnionWith (this.RecipientContacts);
				contacts.UnionWith (this.RecipientGroups.SelectMany (x => x.GetAllGroupAndSubGroupParticipants ()));
				contacts.UnionWith (this.RecipientGroupExtractions.SelectMany (x => x.GetAllContacts (context)));
				contacts.UnionWith (this.RecipientHouseholds.Select (x => x.Contacts.First ()));
				
				contacts.ExceptWith (this.Exclusions);

				this.recipientsCache = contacts.OrderBy (x => x.DisplayName).ToList ();
			}

			return this.recipientsCache;
		}

		public static AiderMailingEntity Create(BusinessContext context, AiderUserEntity aiderUser, string name, string desc, AiderMailingCategoryEntity cat, bool isReady)
		{
			var mailing = context.CreateAndRegisterEntity<AiderMailingEntity> ();

			mailing.Name        = name;
			mailing.Category    = cat;
			mailing.Description = desc;
			mailing.IsReady     = isReady;
			mailing.CreatedBy   = aiderUser;
			
			mailing.ParishGroupPathCache = aiderUser.ParishGroupPathCache;

			mailing.UpdateLastUpdateDate ();

			return mailing;
		}


		public static void Delete(BusinessContext businessContext, AiderMailingEntity mailing)
		{
			AiderMailingParticipantEntity.DeleteByMailing (businessContext, mailing);
			businessContext.DeleteEntity (mailing);
		}

		private void UpdateLastUpdateDate()
		{
			this.LastUpdate = System.DateTime.Now;
		}

		//	These properties are only meant as an in memory cache of the members of the household.
		//	They will never be saved to the database:
		private IList<AiderContactEntity>		recipientsCache;
	}
}
