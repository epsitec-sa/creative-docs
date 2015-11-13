//	Copyright © 2012-2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Data.Common;
using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;

using Epsitec.Common.Types;
using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Business;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Aider.BusinessCases;

namespace Epsitec.Aider.Rules
{
	[BusinessRule]
	internal class AiderPersonBusinessRules : GenericBusinessRule<AiderPersonEntity>
	{
		public override void ApplySetupRule(AiderPersonEntity person)
		{
			var businessContext = this.GetBusinessContext ();

			var eChPerson = businessContext.CreateAndRegisterEntity<eCH_PersonEntity> ();

			eChPerson.CreationDate = Date.Today;
			eChPerson.DataSource   = Enumerations.DataSource.Undefined;

			person.eCH_Person = eChPerson;
			person.Visibility = PersonVisibilityStatus.Default;

			//	TODO#PA
		}

		public override void ApplyBindRule(AiderPersonEntity entity)
		{
			//	Registering the contacts will also register the households, as they are registered
			//	by the contacts.

			this.GetBusinessContext ().Register (entity.Contacts);
		}

		public override void ApplyUpdateRule(AiderPersonEntity person)
		{
			var context = this.GetBusinessContext ();

			AiderPersonBusinessRules.UpdatePersonOfficialName (person);
			AiderPersonBusinessRules.UpdatePersonSex (person);
			AiderPersonBusinessRules.UpdateVisibility (person);

			AiderPersonBusinessRules.VerifyParish (context, person);
			
			person.RefreshCache ();
		}

		public override void ApplyValidateRule(AiderPersonEntity person)
		{
			AiderPersonBusinessRules.ValidateMrMrs (person);
		}


		public static void UpdatePersonOfficialName(AiderPersonEntity person)
		{
			//	There might be a problem with this official name, since it is mixing
			//	spaces and dashes... We have seen things such as "X- Y" or "X - Y",
			//	so we make sure that spaces are removed around dashes.

			string name = person.eCH_Person.PersonOfficialName;
			string trim = name.TrimSpacesAndDashes ();

			if (name != trim)
			{
				person.eCH_Person.PersonOfficialName = trim;
			}
		}

		public static void UpdateVisibility(AiderPersonEntity person)
		{
			if (person.eCH_Person.IsDeceased)
			{
				person.Visibility = PersonVisibilityStatus.Deceased;
			}
		}

		public static void UpdatePersonSex(AiderPersonEntity person)
		{
			if (person.eCH_Person.DataSource != Enumerations.DataSource.Government)
			{
				switch (person.MrMrs)
				{
					case PersonMrMrs.Madame:
					case PersonMrMrs.Mademoiselle:
						person.eCH_Person.PersonSex = PersonSex.Female;
						break;

					case PersonMrMrs.Monsieur:
						person.eCH_Person.PersonSex = PersonSex.Male;
						break;
				}
			}
		}

		public static void FixMrMrsBasedOnSex(AiderPersonEntity person, eCH_PersonEntity eCH)
		{
			if (eCH.PersonSex == PersonSex.Male)
			{
				person.MrMrs = PersonMrMrs.Monsieur;
			}
			else
			{
				if (person.MrMrs == PersonMrMrs.Mademoiselle)
				{
					if ((eCH.AdultMaritalStatus == PersonMaritalStatus.Single) ||
						(eCH.AdultMaritalStatus == PersonMaritalStatus.Unmarried) ||
						(eCH.AdultMaritalStatus == PersonMaritalStatus.None))
					{
						return;
					}
				}

				person.MrMrs = PersonMrMrs.Madame;
			}
		}

		
		private static void ValidateMrMrs(AiderPersonEntity person)
		{
			var eCH = person.eCH_Person;

			if (eCH.IsNull ())
			{
				Logic.BusinessRuleException (person, Resources.Text ("Cette personne n'a pas de données eCH associées. Cela ne devrait jamais arriver..."));

				//	TODO: log this fatal error (as it is probably caused by a database corruption)

				return;
			}

			if (eCH.PersonSex == PersonSex.Unknown)
			{
				return;
			}
			
			if (eCH.DataSource == Enumerations.DataSource.Government)
			{
				AiderPersonBusinessRules.FixMrMrsBasedOnSex (person, eCH);
			}

			switch (person.MrMrs)
			{
				case PersonMrMrs.Monsieur:
					if (eCH.PersonSex == PersonSex.Male)
					{
						return;
					}
					break;

				case PersonMrMrs.Mademoiselle:
					if (eCH.PersonSex == PersonSex.Female)
					{
						if ((eCH.AdultMaritalStatus == PersonMaritalStatus.Single) ||
							(eCH.AdultMaritalStatus == PersonMaritalStatus.Unmarried) ||
							(eCH.AdultMaritalStatus == PersonMaritalStatus.None))
						{
							//	OK
						}
						else
						{
							Logic.BusinessRuleException (person, Resources.Text ("Cette femme n'est pas célibataire. L'appellation 'Mademoiselle' n'est donc pas appropriée dans son cas."));
						}

						return;
					}
					break;

				case PersonMrMrs.Madame:
					if (eCH.PersonSex == PersonSex.Female)
					{
						return;
					}
					break;

				case PersonMrMrs.None:
					return;
			}

			Logic.BusinessRuleException (person, Resources.Text ("Vérifiez l'appellation: elle ne correspond pas au sexe de la personne."));
		}
		
		private static void VerifyParish(BusinessContext context, AiderPersonEntity person)
		{
			if (person.IsDeceased)
			{
				return;
			}

			if (person.ParishGroup.IsNull ())
			{
				AiderPersonBusinessRules.AssignParish (context, person);
			}
			
			if ((person.ParishGroup.IsParish () == false) &&
				(person.ParishGroup.IsNoParish () == false))
			{
				Logic.BusinessRuleException (person, Resources.Text ("Vous devez sélectionner un groupe 'paroisse' pour la paroisse."));
				return;
			}
			
			if (AiderPersonBusinessRules.IsReassignNeeded (context, person))
			{
				AiderPersonBusinessRules.ReassignParish (context, person);
			}
		}

		private static bool IsReassignNeeded(BusinessContext context, AiderPersonEntity person)
		{
			if (ParishAssigner.IsInNoParishGroup (person))
			{
				return true;
			}
			if (ParishAssigner.IsInValidParish (context,ParishAddressRepository.Current, person))
			{
				return false;
			}
			if (person.Warnings.Any (x => x.WarningType == WarningType.ParishMismatch))
			{
				return false;
			}
			return true;
		}

		public static bool ReassignParish(BusinessContext context, AiderPersonEntity person)
		{
			var oldParishGroup	        = person.ParishGroup;
			var oldParishName           = person.ParishGroup.Name;
			var oldParishGroupPath      = AiderGroupIds.DefaultToNoParish (person.ParishGroupPathCache);
			var oldParishParticipations = person.GetOtherParishLevelParticipations ().ToList (); 
			

			AiderPersonBusinessRules.AssignParish (context, person);

			var newParish		   = person.ParishGroup;
			var newParishName      = person.ParishGroup.Name;
			var newParishGroupPath = AiderGroupIds.DefaultToNoParish (person.ParishGroupPathCache);

			if (oldParishGroupPath == newParishGroupPath)
			{
				return false;
			}

			var notifyOldParish = true;

			var removedParticipations = new List<string> ();
			oldParishParticipations.ForEach (p =>
			{
				removedParticipations.Add (p.GetRolePathOrHierarchicalName ());
				person.MainContact.RemoveParticipationInternal (p);
				context.DeleteEntity (p);
			});


			if (person.HasDerogation)
			{
				AiderPersonBusinessRules.RemoveDerogation (context, person, oldParishGroup, oldParishGroupPath);
				notifyOldParish = false;
			}

			var title = Resources.Text ("Nouvelle paroisse de domicile");
			
			var description = TextFormatter.FormatText ("La paroisse ne correspondait pas à l'adresse\n",
														"principale du ménage. La correction suivante\n",
														"a été appliquée:\n \n",
														oldParishName, "\n->\n", newParishName,
														"\nParticipations supprimée:\n \n",
														removedParticipations.Join ("\n"));

			if (notifyOldParish)
			{
				AiderPersonWarningEntity.Create (context, person, oldParishGroupPath, WarningType.ParishDeparture, title, description);
			}

			AiderPersonWarningEntity.Create (context, person, newParishGroupPath, WarningType.ParishArrival, title, description);

			AiderParishChangeProcess.StartProcess (context, person);

			return true;
		}

		public static void RemoveDerogation(BusinessContext context, AiderPersonEntity person, AiderGroupEntity oldParishGroup, string oldParishGroupPath)
		{
			AiderDerogations.RemoveDerogation (context, person, oldParishGroup, oldParishGroupPath);
		}
		
		
		private static void AssignParish(BusinessContext context, AiderPersonEntity person)
		{
			ParishAssigner.AssignToParish (ParishAddressRepository.Current, context, person);
		}
	}
}
