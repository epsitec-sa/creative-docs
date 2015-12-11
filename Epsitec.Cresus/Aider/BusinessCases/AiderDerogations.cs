//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using System.Collections.Generic;
using System.Linq;

using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;
using Epsitec.Aider.Reporting;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Common.Types;

namespace Epsitec.Aider.BusinessCases
{
	/// <summary>
	/// Derogations Business Case
	/// </summary>
	public static class AiderDerogations
	{
		public static bool DerogatePerson(BusinessContext businessContext, AiderPersonEntity person, AiderGroupEntity currentParishGroup, AiderGroupEntity derogationParishGroup, Date date)
		{
			var needDerogationLetter = false;

			AiderDerogations.CheckPrerequisiteBeforeDerogate (person, currentParishGroup, derogationParishGroup);
			
			//Check if a previous derogation is in place ?
			if (person.HasDerogation)
			{
				//Yes, existing derogation in place:
				//Remove old derogation in
				AiderDerogations.RemoveDerogationInParticipations (businessContext, currentParishGroup, person);

				//Check for a "return to home"
				if (person.GeoParishGroupPathCache == derogationParishGroup.Path)
				{
					AiderDerogations.ProcessReturnToHome (businessContext, person, derogationParishGroup);
				}
				else
				{
					AiderDerogations.ProcessDerogationChange (businessContext, person, derogationParishGroup, date);
					needDerogationLetter = true;
				}
			}
			else
			{
				AiderDerogations.ProcessFirstDerogation (businessContext, person, currentParishGroup, derogationParishGroup, date);
				needDerogationLetter = true;
			}

			// X Remove parish participations  
			// this case is now handled by a ParishChangePersonProcess triggered by 
			// parish departure warning
			//AiderDerogations.RemoveParishGroupPartiticipations (businessContext, person, currentParishGroup);


			//Add participation to the destination parish
			AiderDerogations.AddParishGroupParticipations (businessContext, person, derogationParishGroup, date);

			//Check for new subscription
			AiderDerogations.CheckForNewSubscription (businessContext, person, derogationParishGroup);
			//!Trigg business rules!
			person.ParishGroup = derogationParishGroup;

			return needDerogationLetter;
		}

		public static void RemoveDerogation(BusinessContext businessContext, AiderPersonEntity person, AiderGroupEntity oldParishGroup,bool enableWarning = true)
		{
			//Remove old derogation in
			AiderDerogations.RemoveDerogationInParticipations (businessContext, oldParishGroup, person);

			//Remove old derogation out
			var geoParishGroup = person.GetDerogationGeoParishGroup (businessContext);
			AiderDerogations.RemoveDerogationOutParticipations (businessContext, geoParishGroup, person);

			if (enableWarning)
			{
				//Warn old derogated parish
				AiderDerogations.WarnEndOfDerogationForRelocation (businessContext, person, oldParishGroup.Path);

				//Warn GeoParish for derogation end
				AiderDerogations.WarnEndOfDerogationForRelocationAsChange (businessContext, person, person.GeoParishGroupPathCache);
			}
			
			person.ClearDerogation ();
		}

		public static AiderOfficeLetterReportEntity CreateDerogationLetter(BusinessContext businessContext, AiderPersonEntity person,
			/**/														   AiderOfficeSenderEntity sender, AiderUserEntity user,
			/**/														   AiderGroupEntity newParish, AiderGroupEntity oldParish)
		{
			var office		 = AiderOfficeManagementEntity.Find (businessContext, newParish);
			var recipient	 = person.MainContact;
			var documentName = "Confirmation dérogation " + recipient.DisplayName;

			if (sender.IsNull ())
			{
				var message = "Vous devez d'abord associer votre utilisateur à une gestion; la création de la dérogation est impossible.";
				throw new BusinessRuleException (message);
			}

			if (CoreContext.HasExperimentalFeature ("OfficeManagement") == false)
			{
				throw new BusinessRuleException ("Cette fonction n'est pas encore disponible.");
			}

			var greetings = (person.eCH_Person.PersonSex == PersonSex.Male) ? "Monsieur" : "Madame";
			var fullName  = sender.OfficialContact.Person.GetFullName ();

			if (string.IsNullOrEmpty (fullName))
			{
				fullName = user.DisplayName;
			}

			var content   = FormattedContent.Escape (greetings, newParish.Name, oldParish.Name, newParish.Name, fullName);
			var template  = "template-letter-derogation";

			if (oldParish.IsNoParish ())
			{
				template += "-noparish";
			}

			return AiderOfficeLetterReportEntity.Create (businessContext, recipient, sender, documentName, template, content);
		}

		private static void CheckForNewSubscription(BusinessContext businessContext, AiderPersonEntity person, AiderGroupEntity derogationParishGroup)
		{
			var currentSubscriptions = AiderSubscriptionEntity.FindSubscriptions (businessContext, person);
			if (currentSubscriptions.Any ())
			{
				foreach (var subscription in currentSubscriptions)
				{
					var currentEdition		= subscription.RegionalEdition;
					var derogationEdition	= AiderDerogations.GetSubscriptionEdition (businessContext, derogationParishGroup);
					if (currentEdition != derogationEdition)
					{
						AiderSubscriptionEntity.Create (businessContext, person.HouseholdContact.Household, derogationEdition, 1);
						break;
					}
				}
			}
			else
			{
				var derogationEdition	= AiderDerogations.GetSubscriptionEdition (businessContext, derogationParishGroup);
				AiderSubscriptionEntity.Create (businessContext, person.HouseholdContact.Household, derogationEdition, 1);
			}
		}

		private static AiderGroupEntity GetSubscriptionEdition(BusinessContext businessContext,AiderGroupEntity parishGroup)
		{
			var parishRepository = Epsitec.Aider.Data.Common.ParishAddressRepository.Current;

			var regionGroup = parishGroup.Parent;
			var regionCode	= regionGroup.GetRegionId ();

			if(regionCode == 12)
				return Epsitec.Aider.Data.Common.ParishAssigner.FindGroup (businessContext, "PLA", GroupClassification.Region);
			else
				return Epsitec.Aider.Data.Common.ParishAssigner.FindRegionGroup (businessContext, regionCode);
		}

		private static void CheckPrerequisiteBeforeDerogate(AiderPersonEntity person, AiderGroupEntity currentParishGroup, AiderGroupEntity derogationParishGroup)
		{
			if (person.Age.HasValue)
			{
				if (person.Age.Value <16)
				{
					var message = "Une personne de moins de 16 ans ne peut pas être au bénéfice d'une dérogation";
					throw new BusinessRuleException (message);
				}
			}
			if (derogationParishGroup.IsNull ())
			{
				var message = "Vous n'avez pas sélectionné de paroisse.";
				throw new BusinessRuleException (message);
			}
			if (!derogationParishGroup.IsParish ())
			{
				var message = "Le groupe sélectionné n'est pas une paroisse.";
				throw new BusinessRuleException (message);
			}

			if (currentParishGroup == derogationParishGroup)
			{
				var message = "La personne est déjà associée à cette paroisse.";
				throw new BusinessRuleException (message);
			}
		}

		private static void ProcessFirstDerogation(BusinessContext businessContext, AiderPersonEntity person, AiderGroupEntity currentParishGroup, AiderGroupEntity derogationParishGroup, Date date)
		{
			var derogationOutGroup	= AiderDerogations.GetDerogationOutForParishGroup (businessContext, currentParishGroup);
			var derogationInGroup	= AiderDerogations.GetDerogationInForParishGroup (businessContext, derogationParishGroup);

			//No, first derogation:
			//Backup initial parish cache
			person.GeoParishGroupPathCache = person.ParishGroupPathCache;

			//	Add derogation out participation, but not if the person was in the
			//	'no parish' group before...

			if (derogationOutGroup != null)
			{
				AiderDerogations.AddParticipationToDerogationOut (businessContext, person, derogationOutGroup, date);

				AiderDerogations.WarnGeoParishAboutChange (businessContext, person, derogationParishGroup);
			}

			//Add derogation in participation
			AiderDerogations.AddParticipationToDerogationIn (businessContext, person, derogationInGroup, date);

			AiderDerogations.WarnArrivalInParish (businessContext, person, derogationParishGroup);
		}

		private static void ProcessReturnToHome(BusinessContext businessContext, AiderPersonEntity person, AiderGroupEntity derogationParishGroup)
		{
			//Reset state
			person.ClearDerogation ();

			//Remove old derogation out
			AiderDerogations.RemoveDerogationOutParticipations (businessContext, derogationParishGroup, person);

			AiderDerogations.WarnEndOfDerogation (businessContext, person, person.ParishGroupPathCache);
			AiderDerogations.WarnReturnToOriginalParish (businessContext, person, derogationParishGroup);
		}

		private static void ProcessDerogationChange(BusinessContext businessContext, AiderPersonEntity person, AiderGroupEntity derogationParishGroup, Date date)
		{
			var derogationInGroup	= AiderDerogations.GetDerogationInForParishGroup (businessContext, derogationParishGroup);

			//Add derogation in participation
			AiderDerogations.AddParticipationToDerogationIn (businessContext, person, derogationInGroup, date);

			AiderDerogations.WarnEndOfDerogation (businessContext, person, person.ParishGroupPathCache);
			AiderDerogations.WarnArrivalInParish (businessContext, person, derogationParishGroup);
			//Warn GeoParish for a Derogation Change
			AiderPersonWarningEntity.Create (businessContext, person, person.GeoParishGroupPathCache,
				WarningType.DerogationChange, "Changement de dérogation vers la\n" + derogationParishGroup.Name + ".");
		}

		private static void AddParishGroupParticipations(BusinessContext businessContext, AiderPersonEntity person, AiderGroupEntity derogationParishGroup, Date date)
		{
			derogationParishGroup.AddParticipations (businessContext, person.MainContact, date, FormattedText.Null);
		}

		private static void RemoveParishGroupPartiticipations(BusinessContext businessContext, AiderPersonEntity person, AiderGroupEntity currentParishGroup)
		{
			AiderGroupEntity.RemoveParticipations (businessContext, currentParishGroup.FindParticipationsByGroup (businessContext, person, currentParishGroup));
		}

		private static void AddParticipationToDerogationIn(BusinessContext businessContext, AiderPersonEntity person, AiderGroupEntity derogationInGroup, Date date)
		{
			if (derogationInGroup.GroupDef.Classification == GroupClassification.DerogationIn)
			{
				derogationInGroup.AddParticipations (businessContext, person.MainContact, date, new FormattedText ("Dérogation entrante"));
			}
			else
			{
				var message = "Opération impossible, ce groupe n'est pas classifié : Dérogations Entrantes";
				throw new BusinessRuleException (message);
			}
		}

		private static void AddParticipationToDerogationOut(BusinessContext businessContext, AiderPersonEntity person, AiderGroupEntity derogationOutGroup, Date date)
		{
			if (derogationOutGroup.GroupDef.Classification == GroupClassification.DerogationOut)
			{
				derogationOutGroup.AddParticipations (businessContext, person.MainContact, date, new FormattedText ("Dérogation sortante"));
			}
			else
			{
				var message = "Opération impossible, ce groupe n'est pas classifié : Dérogations Sortantes";
				throw new BusinessRuleException (message);
			}
		}

		private static AiderGroupEntity GetDerogationInForParishGroup(BusinessContext businessContext, AiderGroupEntity parishGroup)
		{
			if (parishGroup.Subgroups.Any ())
			{
				return parishGroup.Subgroups.SingleOrDefault (g => g.GroupDef.Classification == GroupClassification.DerogationIn);
			}
			else
				return null;
		}

		private static AiderGroupEntity GetDerogationOutForParishGroup(BusinessContext businessContext, AiderGroupEntity parishGroup)
		{
			if (parishGroup.Subgroups.Any ())
			{
				return parishGroup.Subgroups.SingleOrDefault (g => g.GroupDef.Classification == GroupClassification.DerogationOut);
			}
			else
				return null;
		}

		private static void RemoveDerogationInParticipations(BusinessContext businessContext, AiderGroupEntity parishGroup, AiderPersonEntity person)
		{
			var oldDerogationInGroup = parishGroup.Subgroups.SingleOrDefault (g => g.GroupDef.Classification == GroupClassification.DerogationIn);
			if (oldDerogationInGroup != null)
			{
				AiderGroupEntity.RemoveParticipations (businessContext, oldDerogationInGroup.FindParticipationsByGroup (businessContext, person, oldDerogationInGroup));
			}
		}

		private static void RemoveDerogationOutParticipations(BusinessContext businessContext, AiderGroupEntity parishGroup, AiderPersonEntity person)
		{
			var oldDerogationOutGroup = parishGroup.Subgroups.SingleOrDefault (g => g.GroupDef.Classification == GroupClassification.DerogationOut);
			if (oldDerogationOutGroup != null)
			{
				AiderGroupEntity.RemoveParticipations (businessContext, oldDerogationOutGroup.FindParticipationsByGroup (businessContext, person, oldDerogationOutGroup));
			}
		}

		public static void WarnEndOfDerogation(BusinessContext businessContext, AiderPersonEntity person, string parishPathCache)
		{
			var message = "Fin de dérogation.";
			AiderPersonWarningEntity.Create (businessContext, person, parishPathCache, WarningType.ParishDeparture, message);
		}

		private static void WarnEndOfDerogationForRelocation(BusinessContext businessContext, AiderPersonEntity person, string parishPathCache)
		{
			var message = "Fin de dérogation suite à un déménagement.";
			AiderPersonWarningEntity.Create (businessContext, person, parishPathCache, WarningType.ParishDeparture, message);
		}

		private static void WarnEndOfDerogationForRelocationAsChange(BusinessContext businessContext, AiderPersonEntity person, string parishPathCache)
		{
			var message = "Fin de dérogation suite à un déménagement.";
			AiderPersonWarningEntity.Create (businessContext, person, parishPathCache, WarningType.DerogationChange, message);
		}

		private static void WarnGeoParishAboutChange(BusinessContext businessContext, AiderPersonEntity person, AiderGroupEntity destParish)
		{
			var message = "Personne dérogée vers la\n" + destParish.Name + ".";
			AiderPersonWarningEntity.Create (businessContext, person, person.GeoParishGroupPathCache, WarningType.ParishDeparture, message);
		}

		private static void WarnReturnToOriginalParish(BusinessContext businessContext, AiderPersonEntity person, AiderGroupEntity destParish)
		{
			var message = "Fin de dérogation.";
			AiderPersonWarningEntity.Create (businessContext, person, destParish.Path, WarningType.ParishArrival, message);
		}


		private static void WarnArrivalInParish(BusinessContext businessContext, AiderPersonEntity person, AiderGroupEntity destParish)
		{
			string message;

			if (person.ParishGroup.IsNoParish ())
			{
				message = "Personne dérogée.";
			}
			else
			{
				message = "Personne dérogée en provenance de la\n" + person.ParishGroup.Name + ".";
			}

			AiderPersonWarningEntity.Create (businessContext, person, destParish.Path, WarningType.ParishArrival, message);
		}
	}
}
