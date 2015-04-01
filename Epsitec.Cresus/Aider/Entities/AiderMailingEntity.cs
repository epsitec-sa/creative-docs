//	Copyright © 2012-2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Pierre ARNAUD

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
using Epsitec.Cresus.Core.Metadata;
using Epsitec.Cresus.DataLayer.Loader;
using Epsitec.Cresus.DataLayer.Expressions;


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
			int totalCount           = this.GetParticipantCount ();
			int contactCount         = this.RecipientContacts.Count;
			int groupCount           = this.RecipientGroups.Count;
			int groupExtractionCount = this.RecipientGroupExtractions.Count;
			int householdCount       = this.RecipientHouseholds.Count;
			int queriesCount         = this.Queries.Count;
			return TextFormatter.FormatText (totalCount, "destinataires", "\n",
				/**/						 contactCount, contactCount > 1 ? "contacts individuels" : "contact individuel", "\n",
				/**/						 groupCount, groupCount > 1 ? "groupes" : "groupe", "\n",
				/**/						 groupExtractionCount, groupExtractionCount > 1 ? " groupes transversaux" : "groupe transversal", "\n",
				/**/						 householdCount, householdCount > 1 ? "ménages" : "ménage", "\n",
				/**/						 queriesCount, queriesCount > 1 ? "requêtes" : "requête", "\n",
				/**/						 "dernière maj. " + this.LastUpdate.Value.ToLocalTime ().ToString ());
		}

		public string GetReadyText()
		{
			return this.IsReady ? "Prêt pour l'envoi" : "En préparation";
		}

		public void AddQuery (BusinessContext businessContext, AiderMailingQueryEntity queryToAdd)
		{
			if (!this.Queries.Contains (queryToAdd))
			{
				this.UpdateLastUpdateDate ();
				this.Queries.Add (queryToAdd);
				this.UpdateMailingParticipants (businessContext);
			}
		}

		public void RemoveQuery(BusinessContext businessContext, AiderMailingQueryEntity queryToRemove)
		{
			if (this.Queries.Contains (queryToRemove))
			{
				this.UpdateLastUpdateDate ();
				this.UpdateLastUpdateDate ();
				this.Queries.Remove (queryToRemove);
				this.UpdateMailingParticipants (businessContext);
			}
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

		public void RemoveContacts(BusinessContext businessContext, IEnumerable<AiderContactEntity> contactsToRemove)
		{
			this.UpdateLastUpdateDate ();
			foreach (var contact in contactsToRemove)
			{
				this.RecipientContacts.Remove (contact);
				AiderMailingParticipantEntity.FindAndRemove (businessContext, this, contact);
			}
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
			return FormattedText.FromSimpleText ("Destinataires");
		}

		public FormattedText GetRecipientsSummary()
		{
			var count =  this.GetParticipantCount ();
			var type  = "contact";

			if (this.IsGroupedByHousehold)
			{
				type = "ménage";
			}

			if (count > 0)
			{
				if (count == 1)
				{
					return String.Format ("Un {0}", type);
				}
				else
				{
					return String.Format ("{0} {1}s", count, type);
				}
			}
			else
			{
				return String.Format ("Aucun {0}", type);
			}
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
			mailing.CreatedBy   = aiderUser.DisplayName;
			
			mailing.ParishGroupPathCache = cat.GroupPathCache;

			mailing.UpdateLastUpdateDate ();

			return mailing;
		}


		public static void Delete(BusinessContext businessContext, AiderMailingEntity mailing)
		{
			AiderMailingParticipantEntity.DeleteByMailing (businessContext, mailing);
			businessContext.DeleteEntity (mailing);
		}

		public Request GetContactRequestFromQuery(DataContext dataContext, AiderMailingQueryEntity query)
		{
			var queryFilterXml     = DataSetUISettingsEntity.ByteArrayToXml (query.Query);
			var queryFilter		   = Filter.Restore (queryFilterXml);
			var request = new Request ();

			if(query.CommandId == Res.Commands.Base.ShowAiderContactFiltered.CommandId)
			{
				var example			   = new AiderContactEntity ();
				request.RootEntity = example;
				request.AddCondition (dataContext, example, queryFilter);
				return request;
			}

			if (query.CommandId == Res.Commands.Base.ShowAiderEmployeeJob.CommandId)
			{
				var contact             = new AiderContactEntity ();

				var employee            = new AiderEmployeeEntity ()
				{
					PersonContact = contact
				};
				var example			    = new AiderEmployeeJobEntity ()
				{
					Employee = employee
				};

				request.RootEntity      = example;
				request.RequestedEntity = contact;

				request.AddCondition (dataContext, example, queryFilter);
				request.AddCondition (dataContext, example, j => j.EmployeeJobFunction != EmployeeJobFunction.GestionnaireAIDER &&
					                                             j.EmployeeJobFunction != EmployeeJobFunction.UtilisateurAIDER && 
																 j.EmployeeJobFunction != EmployeeJobFunction.SuppléantAIDER); 

				return request;
			}

			if (query.CommandId == Res.Commands.Base.ShowAiderEmployeeJobAIDERUsersOnly.CommandId)
			{
				var contact             = new AiderContactEntity ();

				var employee            = new AiderEmployeeEntity ()
				{
					PersonContact = contact
				};
				var example			    = new AiderEmployeeJobEntity ()
				{
					Employee = employee
				};

				request.RootEntity      = example;
				request.RequestedEntity = contact;

				request.AddCondition (dataContext, example, queryFilter);
				request.AddCondition (dataContext, example, j => j.EmployeeJobFunction == EmployeeJobFunction.GestionnaireAIDER || 
																 j.EmployeeJobFunction == EmployeeJobFunction.UtilisateurAIDER ||
																 j.EmployeeJobFunction == EmployeeJobFunction.SuppléantAIDER);
				return request;
			}

			if (query.CommandId == Res.Commands.Base.ShowAiderEmployee.CommandId)
			{
				var contact             = new AiderContactEntity ();

				var example            = new AiderEmployeeEntity ()
				{
					PersonContact = contact
				};

				request.RootEntity      = example;
				request.RequestedEntity = contact;

				request.AddCondition (dataContext, example, queryFilter);
				return request;
			}

			if (query.CommandId == Res.Commands.Base.ShowAiderReferee.CommandId)
			{
				var contact             = new AiderContactEntity ();

				var employee            = new AiderEmployeeEntity ()
				{
					PersonContact = contact
				};

				var example            = new AiderRefereeEntity ()
				{
					Employee = employee
				};

				request.RootEntity      = example;
				request.RequestedEntity = contact;

				request.AddCondition (dataContext, example, queryFilter);
				return request;
			}

			if (query.CommandId == Res.Commands.Base.ShowAiderHousehold.CommandId)
			{
				var example             = new AiderHouseholdEntity ();

				var contact             = new AiderContactEntity ()
				{
					Household = example
				};

				request.RootEntity      = contact;
				request.RequestedEntity = contact;
				
				request.AddCondition (dataContext, example, queryFilter);
				return request;
			}

			if (query.CommandId == Res.Commands.Base.ShowAiderLegalPerson.CommandId)
			{
				var example			   = new AiderLegalPersonEntity ();

				var contact			   = new AiderContactEntity ()
				{
					LegalPerson = example
				};

				request.RootEntity      = contact;
				request.RequestedEntity = contact;
				request.AddCondition (dataContext, example, queryFilter);
				return request;
			}

			throw new NotImplementedException ("CommandId for this query is not implemented");
		}

		public void UpdateMailing(BusinessContext businessContext)
		{
			// Set participants
			this.UpdateMailingParticipants (businessContext);
			// Then exclude
			this.UpdateMailingExclusions (businessContext);
		}

		/// <summary>
		/// Recreate participations
		/// </summary>
		/// <param name="businessContext"></param>
		private void UpdateMailingParticipants(BusinessContext businessContext)
		{
			var dataContext = businessContext.DataContext;

			this.UpdateLastUpdateDate ();

			AiderMailingParticipantEntity.DeleteByMailing (businessContext, this);


			if (this.IsGroupedByHousehold)
			{
				var households = this.GetParticipantsByHousehold (businessContext.DataContext);

				foreach (var household in households)
				{
					if (household.Contacts.Any ())
					{
						AiderMailingParticipantEntity.Create (businessContext, this, household);
					}
				}
			}
			else
			{
				foreach (var query in this.Queries)
				{
					var created			   = new List<AiderMailingParticipantEntity> ();
					var request			   = this.GetContactRequestFromQuery (businessContext.DataContext, query);

					var contactsFromQuery = businessContext.GetByRequest<AiderContactEntity> (request);
					foreach (var contact in contactsFromQuery)
					{
						AiderMailingParticipantEntity.Create (businessContext, this, contact);
					}
				}

				foreach (var contact in this.RecipientContacts)
				{
					AiderMailingParticipantEntity.Create (businessContext, this, contact);
				}

				foreach (var group in this.RecipientGroups)
				{
					AiderMailingParticipantEntity.Create (businessContext, this, group);
				}

				foreach (var group in this.RecipientGroupExtractions)
				{
					AiderMailingParticipantEntity.Create (businessContext, this, group);
				}

				foreach (var household in this.RecipientHouseholds)
				{
					if (household.Contacts.Any ())
					{
						AiderMailingParticipantEntity.Create (businessContext, this, household);
					}
				}
			}

			businessContext.SaveChanges (LockingPolicy.KeepLock);
		}

		/// <summary>
		/// Apply exclusions on participations 
		/// </summary>
		/// <param name="businessContext"></param>
		private void UpdateMailingExclusions(BusinessContext businessContext)
		{
			var dataContext          = businessContext.DataContext;
			var excludedContacts     = new HashSet<AiderContactEntity> (this.GetExcludedRecipients (dataContext));
			var participants         = new HashSet<AiderContactEntity> (AiderMailingParticipantEntity.GetAllParticipants (dataContext, this).Select (p => p.Contact));

			foreach (var exclude in excludedContacts)
			{
				if (this.IsGroupedByHousehold)
				{
					var households = this.GetParticipantsByHousehold (businessContext.DataContext);
					var excludedHouseholds = households.Where (h => exclude.Household == h);
					foreach (var household in excludedHouseholds)
					{
						// Exclude Contact[0] (the same used when grouped by household)
						AiderMailingParticipantEntity.ExcludeContact (businessContext, this, household.Contacts[0]);
					}
				}
				else
				{
					if (participants.Contains (exclude))
					{
						AiderMailingParticipantEntity.ExcludeContact (businessContext, this, exclude);
					}
				}
			}

			this.UpdateLastUpdateDate ();
			businessContext.SaveChanges (LockingPolicy.KeepLock);
		}

		private void UpdateLastUpdateDate()
		{
			this.LastUpdate = System.DateTime.UtcNow;
		}

		private int GetParticipantCount()
		{
			return this.ExecuteWithDataContext (c => this.CountParticipants (c), () => 0);		
		}

		private int CountParticipants (DataContext dataContext)
		{
			var example = new AiderMailingParticipantEntity ()
			{
				Mailing = this,
				IsExcluded = false
			};

			return dataContext.GetCount (example);
		}

		private IEnumerable<AiderHouseholdEntity> GetParticipantsByHousehold (DataContext dataContext)
		{
			IEnumerable<AiderHouseholdEntity> contactsHouseholdsFromQuery = Enumerable.Empty<AiderHouseholdEntity> ();
			foreach (var query in this.Queries)
			{
				var created			   = new List<AiderMailingParticipantEntity> ();
				var request			   = this.GetContactRequestFromQuery (dataContext, query);

				contactsHouseholdsFromQuery = dataContext.GetByRequest<AiderContactEntity> (request)
											.Select (c => c.Household);
			}

			var contactsHouseholds	 = this.RecipientContacts.Select (c => c.Household);

			var groupsHouseholds	 = this.RecipientGroups
											.SelectMany (g => g.GetAllGroupAndSubGroupParticipations ().Distinct ()
												.Select (p => p.Contact)
												.Select (c => c.Household)
											);

			var extractionsHouseholds = this.RecipientGroupExtractions
											.SelectMany (t => t.GetAllParticipations (dataContext)
												.Select (p => p.Contact)
												.Select (c => c.Household)
											);

			return this.RecipientHouseholds
										.Union (extractionsHouseholds)
										.Union (groupsHouseholds)
										.Union (contactsHouseholds)
										.Union (contactsHouseholdsFromQuery)
										.Distinct ();
		}
	}
}
