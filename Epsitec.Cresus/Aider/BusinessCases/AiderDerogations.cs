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
		public static void CheckPrerequisiteBeforeDerogate(AiderPersonEntity person, AiderGroupEntity currentParishGroup,AiderGroupEntity derogationParishGroup)
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

		public static void AddParishGroupParticipations(BusinessContext businessContext, AiderPersonEntity person, AiderGroupEntity derogationParishGroup, Date date)
		{
			derogationParishGroup.AddParticipations (businessContext, person.MainContact, date, FormattedText.Null);
		}

		public static void RemoveParishGroupPartiticipations(BusinessContext businessContext, AiderPersonEntity person, AiderGroupEntity currentParishGroup)
		{
			currentParishGroup.RemoveParticipations (businessContext, currentParishGroup.FindParticipations (businessContext, person.MainContact));
		}

		public static void AddParticipationToDerogationIn(BusinessContext businessContext, AiderPersonEntity person, AiderGroupEntity derogationInGroup, Date date)
		{
			if (derogationInGroup.GroupDef.Classification == GroupClassification.DerogationIn)
			{
				derogationInGroup.AddParticipations (businessContext, person.MainContact, date, new FormattedText ("Dérogation entrante"));
			}
			else
			{
				var message = "Opération impossible, ce groupe n'est pas classifié : Derogations Entrantes";
				throw new BusinessRuleException (message);
			}
		}

		public static void AddParticipationToDerogationOut(BusinessContext businessContext, AiderPersonEntity person, AiderGroupEntity derogationOutGroup, Date date)
		{
			if (derogationOutGroup.GroupDef.Classification == GroupClassification.DerogationOut)
			{
				derogationOutGroup.AddParticipations (businessContext, person.MainContact, date, new FormattedText ("Dérogation sortante"));
			}
			else
			{
				var message = "Opération impossible, ce groupe n'est pas classifié : Derogations Sortantes";
				throw new BusinessRuleException (message);
			}
		}

		public static AiderGroupEntity GetDerogationInForParishGroup(BusinessContext businessContext, AiderGroupEntity parishGroup)
		{
			return parishGroup.Subgroups.Single (g => g.GroupDef.Classification == GroupClassification.DerogationIn);
		}

		public static AiderGroupEntity GetDerogationOutForParishGroup(BusinessContext businessContext, AiderGroupEntity parishGroup)
		{
			return parishGroup.Subgroups.Single (g => g.GroupDef.Classification == GroupClassification.DerogationOut);
		}

		public static void RemoveDerogationInParticipations(BusinessContext businessContext, AiderGroupEntity parishGroup, AiderPersonEntity person)
		{
			var oldDerogationInGroup = parishGroup.Subgroups.Single (g => g.GroupDef.Classification == GroupClassification.DerogationIn);
			if (oldDerogationInGroup != null)
			{
				oldDerogationInGroup.RemoveParticipations (businessContext, oldDerogationInGroup.FindParticipations (businessContext, person.MainContact));
			}
		}

		public static void RemoveDerogationOutParticipations(BusinessContext businessContext, AiderGroupEntity parishGroup, AiderPersonEntity person)
		{
			var oldDerogationInGroup = parishGroup.Subgroups.Single (g => g.GroupDef.Classification == GroupClassification.DerogationIn);
			if (oldDerogationInGroup != null)
			{
				oldDerogationInGroup.RemoveParticipations (businessContext, oldDerogationInGroup.FindParticipations (businessContext, person.MainContact));
			}
		}

		public static AiderOfficeLetterReportEntity CreateDerogationLetter(	BusinessContext businessContext,AiderPersonEntity person,
			/**/															AiderOfficeSenderEntity sender, AiderGroupEntity destParish, AiderGroupEntity origineParish)
		{

			var office			= AiderOfficeManagementEntity.Find (businessContext, destParish);
			var recipient		= person.MainContact;
			var documentName	= "Confirmation dérogation " + recipient.DisplayName;

			if (sender.IsNull ())
			{
				var message = "Vous devez d'abord associer votre utilisateur à un secrétariat; la création de la dérogation est impossible.";
				throw new BusinessRuleException (message);
			}

			if (CoreContext.HasExperimentalFeature ("OfficeManagement") == false)
			{
				throw new BusinessRuleException ("Cette fonction n'est pas encore disponible.");
			}

			var greetings = (person.eCH_Person.PersonSex == PersonSex.Male) ? "Monsieur" : "Madame";
			var fullName  = sender.OfficialContact.Person.GetFullName ();
			var content   = FormattedContent.Escape (greetings, destParish.Name, origineParish.Name, destParish.Name, fullName);

			return AiderOfficeLetterReportEntity.Create (businessContext, recipient, sender, documentName, "template-letter-derogation", content);
		}

		public static void WarnEndOfDerogation(BusinessContext businessContext, AiderPersonEntity person, string parishPathCache)
		{
			var message = "Fin de dérogation.";
			AiderPersonWarningEntity.Create (businessContext, person, parishPathCache, WarningType.ParishDeparture, message);
		}

		public static void WarnEndOfDerogationForRelocation(BusinessContext businessContext, AiderPersonEntity person, string parishPathCache)
		{
			var message = "Fin de dérogation suite à un déménagement.";
			AiderPersonWarningEntity.Create (businessContext, person, parishPathCache, WarningType.ParishDeparture, message);
		}

		public static void WarnEndOfDerogationForRelocationAsChange(BusinessContext businessContext, AiderPersonEntity person, string parishPathCache)
		{
			var message = "Fin de dérogation suite à un déménagement.";
			AiderPersonWarningEntity.Create (businessContext, person, parishPathCache, WarningType.DerogationChange, message);
		}

		public static void WarnGeoParishAboutChange(BusinessContext businessContext, AiderPersonEntity person, AiderGroupEntity destParish)
		{
			var message = "Personne dérogée vers la\n" + destParish.Name + ".";
			AiderPersonWarningEntity.Create (businessContext, person, person.GeoParishGroupPathCache, WarningType.ParishDeparture, message);
		}

		public static void WarnReturnToOriginalParish(BusinessContext businessContext, AiderPersonEntity person, AiderGroupEntity destParish)
		{
			var message = "Fin de dérogation.";
			AiderPersonWarningEntity.Create (businessContext, person, destParish.Path, WarningType.ParishArrival, message);
		}


		public static void WarnArrivalInParish(BusinessContext businessContext, AiderPersonEntity person, AiderGroupEntity destParish)
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
