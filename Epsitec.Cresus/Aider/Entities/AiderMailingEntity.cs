//	Copyright © 2012-2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

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
using Epsitec.Aider.Helpers;


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
											 this.LastUpdate.Value.ToLocalTime ().ToString (),
											 "\nCode interne~", this.ParishGroupPathCache);
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
				/**/						 this.LastUpdate.Value.ToLocalTime ().ToString ());
		}

		public string GetReadyText()
		{
			return this.IsReady ? "Prêt pour l'envoi" : "En préparation";
		}

		/// <summary>
		/// Sync groups changes that affect participants dataset
		/// </summary>
		/// <param name="businessContext"></param>
		public void UpdateMailingParticipants(BusinessContext businessContext)
		{
			var dataContext = businessContext.DataContext;

			this.UpdateLastUpdateDate ();

			AiderMailingParticipantEntity.DeleteByMailing (businessContext, this);


			if(this.IsGroupedByHousehold)
			{
				var contactsHouseholds	 = this.RecipientContacts.Select (c => c.Household);

				var groupsHouseholds	 = this.RecipientGroups
												.SelectMany (g => g.GetAllGroupAndSubGroupParticipations ().Distinct ()
													.Select (p => p.Contact)
													.Select (c => c.Household)
												);

				var extractionsHouseholds = this.RecipientGroupExtractions
												.SelectMany (t => t.GetAllParticipations (businessContext.DataContext)
													.Select (p => p.Contact)
													.Select (c => c.Household)
												);

				var households			 = this.RecipientHouseholds
											.Union (extractionsHouseholds)
											.Union (groupsHouseholds)
											.Union (contactsHouseholds)
											.Distinct ();

				foreach(var household in households)
				{
					if(household.Contacts.Any ())
					{
					
						var participation = AiderMailingParticipantEntity.Create (businessContext, this, household);

						if (this.Exclusions.Contains (household.Contacts[0]))
						{
							participation.IsExcluded = true;
						}
					}
				}
			}
			else
			{
				foreach (var contact in this.RecipientContacts)
				{
					var participation = AiderMailingParticipantEntity.Create (businessContext, this, contact);
					if (this.Exclusions.Contains (contact))
					{
						participation.IsExcluded = true;
					}
				}

				foreach (var group in this.RecipientGroups)
				{
					var participations = AiderMailingParticipantEntity.Create (businessContext, this, group);

					foreach (var participation in participations)
					{
						if (this.Exclusions.Contains (participation.Contact))
						{
							participation.IsExcluded = true;
						}
					}
				}

				foreach (var group in this.RecipientGroupExtractions)
				{
					var participations = AiderMailingParticipantEntity.Create (businessContext, this, group);

					foreach (var participation in participations)
					{
						if (this.Exclusions.Contains (participation.Contact))
						{
							participation.IsExcluded = true;
						}
					}
				}

				foreach (var household in this.RecipientHouseholds)
				{
					if(household.Contacts.Any ())
					{
						var participation = AiderMailingParticipantEntity.Create (businessContext, this, household);

						if (this.Exclusions.Contains (household.Contacts[0]))
						{
							participation.IsExcluded = true;
						}
					}			
				}
			}

			businessContext.SaveChanges (LockingPolicy.KeepLock);
		}

		public void UpdateMailingExclusions(BusinessContext businessContext)
		{
			var dataContext = businessContext.DataContext;

			this.UpdateLastUpdateDate ();

			var participants = new HashSet<AiderContactEntity> (AiderMailingParticipantEntity.GetAllParticipants (dataContext, this).Select (p => p.Contact));
			var excludedParticipants = new HashSet<AiderContactEntity> (AiderMailingParticipantEntity.GetAllExcludedParticipants (dataContext, this).Select (p => p.Contact));
			var excludedContacts     = new HashSet<AiderContactEntity> (this.GetExcludedRecipients (dataContext));

			//	Remove exclusions which no longer belong to the current exclusions:
			foreach (var contact in excludedParticipants)
			{
				if (!excludedContacts.Contains (contact))
				{
					AiderMailingParticipantEntity.UnExcludeContact (businessContext, this, contact);
				}
			}

			//	Add exclusions which are not yet defined for the current exclusions:
			foreach (var contact in excludedContacts)
			{
				if (participants.Contains (contact))
				{
					AiderMailingParticipantEntity.ExcludeContact (businessContext, this, contact);
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
			foreach (var contact in groupToRemove.GetAllGroupAndSubGroupParticipantContacts ().Distinct ())
			{
				this.Exclusions.RemoveAll (r => r == contact);
				AiderMailingParticipantEntity.FindAndRemove (businessContext, this, contact, MailingParticipantType.Group);
			}			
		}

		public void RemoveGroupExtraction(BusinessContext businessContext, AiderGroupExtractionEntity groupExtractionToRemove)
		{
			this.UpdateLastUpdateDate ();
			this.RecipientGroupExtractions.Remove (groupExtractionToRemove);

			foreach (var contact in groupExtractionToRemove.GetAllContacts (businessContext.DataContext).Distinct ())
			{
				this.Exclusions.RemoveAll (r => r == contact);
				AiderMailingParticipantEntity.FindAndRemove (businessContext, this, contact, MailingParticipantType.GroupExtraction);
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

		public void ExcludeContact(BusinessContext businessContext, AiderContactEntity contactToExclude)
		{
			if (!this.Exclusions.Contains (contactToExclude))
			{
				this.UpdateLastUpdateDate ();
				this.Exclusions.Add (contactToExclude);
				AiderMailingParticipantEntity.ExcludeContact (businessContext, this, contactToExclude);
			}
		}

		public void ExcludeGroup(BusinessContext businessContext, AiderGroupEntity groupToExclude)
		{
			if (!this.GroupExclusions.Contains (groupToExclude))
			{
				this.UpdateLastUpdateDate ();
				this.GroupExclusions.Add (groupToExclude);

				foreach (var contactToExclude in groupToExclude.GetAllGroupAndSubGroupParticipantContacts ())
				{
					AiderMailingParticipantEntity.ExcludeContact (businessContext, this, contactToExclude);
				}
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

		public void UnExludeGroup(BusinessContext businessContext, AiderGroupEntity groupToUnExclude)
		{
			this.UpdateLastUpdateDate ();
			this.GroupExclusions.RemoveAll (r => r == groupToUnExclude);
			foreach (var contact in groupToUnExclude.GetAllGroupAndSubGroupParticipantContacts())
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
			return FormattedText.FromSimpleText ("Exclusions (C:", this.Exclusions.Count ().ToString (), ",G:",this.GroupExclusions.Count ().ToString (),")");
		}

		public FormattedText GetExclusionsSummary()
		{
			var recipients = this.GetExcludedRecipients ()
				.Select (r => r.GetCompactSummary ())
				.CreateSummarySequence (20, "...");

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
				contacts.UnionWith (this.RecipientGroups.SelectMany (x => x.GetAllGroupAndSubGroupParticipantContacts ()));
				contacts.UnionWith (this.RecipientGroupExtractions.SelectMany (x => x.GetAllContacts (context)));
				contacts.UnionWith (this.RecipientHouseholds.Select (x => x.Contacts.First ()));
				
				contacts.ExceptWith (this.Exclusions);
				contacts.ExceptWith (this.GroupExclusions.SelectMany (x => x.GetAllGroupAndSubGroupParticipantContacts ()));

				this.recipientsCache = contacts.OrderBy (x => x.DisplayName).ToList ();
			}

			return this.recipientsCache;
		}

		public IList<AiderContactEntity> GetExcludedRecipients(DataContext context = null)
		{
			if (context == null)
			{
				context = DataContextPool.GetDataContext (this);
			}

			var exclusions = new HashSet<AiderContactEntity> ();

			exclusions.UnionWith (this.Exclusions);
			exclusions.UnionWith (this.GroupExclusions.SelectMany (x => x.GetAllGroupAndSubGroupParticipantContacts ()));

			return exclusions.OrderBy (x => x.DisplayName).ToList ();		
		}

		public static AiderMailingEntity Create(BusinessContext context, AiderUserEntity aiderUser, string name, string desc, AiderMailingCategoryEntity cat, bool isReady)
		{
			var mailing = context.CreateAndRegisterEntity<AiderMailingEntity> ();

			mailing.Name        = name;
			mailing.Category    = cat;
			mailing.Description = desc;
			mailing.IsReady     = isReady;
			mailing.CreatedBy   = aiderUser;
			
			mailing.ParishGroupPathCache = cat.GroupPathCache;

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
			this.LastUpdate = System.DateTime.UtcNow;
		}

		//	These properties are only meant as an in memory cache of the members of the household.
		//	They will never be saved to the database:
		private IList<AiderContactEntity>		recipientsCache;
	}
}
