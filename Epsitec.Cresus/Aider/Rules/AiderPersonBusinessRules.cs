//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Aider.Data;
using Epsitec.Aider.Data.Eerv;
using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;

using Epsitec.Common.Types;
using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Business;

using System.Collections.Generic;
using System.Linq;

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

		public override void ApplyUpdateRule(AiderPersonEntity person)
		{
			AiderPersonBusinessRules.UpdatePersonOfficialName (person);
			AiderPersonBusinessRules.UpdateBirthday (person);
			AiderPersonBusinessRules.UpdateCallName (person);
			AiderPersonBusinessRules.UpdateDisplayName (person);
			AiderPersonBusinessRules.UpdatePersonSex (person);
			AiderPersonBusinessRules.UpdateVisibility (person);

			this.VerifyParish (person);
		}

		private static void UpdatePersonOfficialName(AiderPersonEntity person)
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

		private static void UpdateVisibility(AiderPersonEntity person)
		{
			if (person.eCH_Person.IsDeceased)
			{
				person.Visibility = PersonVisibilityStatus.Deceased;
			}
		}
		private static void UpdateBirthday(AiderPersonEntity person)
		{
			var date = person.eCH_Person.PersonDateOfBirth;

			if (date.HasValue == false)
			{
				person.BirthdayDay   = 0;
				person.BirthdayMonth = 0;
				person.BirthdayYear  = 0;
			}
			else
			{
				person.BirthdayDay   = date.Value.Day;
				person.BirthdayMonth = date.Value.Month;
				person.BirthdayYear  = date.Value.Year;
			}
		}

		public override void ApplyValidateRule(AiderPersonEntity person)
		{
			AiderPersonBusinessRules.ValidateMrMrs (person);
		}

		private static void UpdateCallName(AiderPersonEntity person)
		{
			if (string.IsNullOrWhiteSpace (person.CallName))
			{
				person.CallName = eCH_PersonEntity.GetDefaultFirstName (person.eCH_Person);
			}
		}

		private static void UpdateDisplayName(AiderPersonEntity person)
		{
			person.DisplayName = AiderPersonEntity.GetDisplayName (person);
		}

		private static void UpdatePersonSex(AiderPersonEntity person)
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

		private static void ValidateMrMrs(AiderPersonEntity person)
		{
			var eCH = person.eCH_Person;

			if ((eCH.IsNull ()) ||
				(eCH.PersonSex == PersonSex.Unknown))
			{
				Logic.BusinessRuleException (person, Resources.Text ("Cette personne n'a pas de sexe défini."));
				return;
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

		private void VerifyParish(AiderPersonEntity person)
		{
			if (person.Parish.IsNull ())
			{
				this.AssignParish (person);
			}
			else
			{
				this.CheckCurrentParish (person);
			}
		}

		private void CheckCurrentParish(AiderPersonEntity person)
		{
			var businessContext = this.GetBusinessContext ();

			if (!ParishAssigner.IsParishGroup (person.Parish.Group))
			{
				Logic.BusinessRuleException (person, Resources.Text ("Vous devez sélectionner un groupe 'paroisse' pour la paroisse."));

				return;
			}

			if (ParishAssigner.IsInValidParish (ParishAddressRepository.Current, businessContext, person))
			{
				return;
			}

			if (person.Warnings.Any (x => x.WarningType == WarningType.ParishMismatch))
			{
				return;
			}

			var title = Resources.Text ("La paroisse ne correspond pas à l'adresse principale");
			var type = WarningType.ParishMismatch;

			AiderPersonWarningEntity.Create (businessContext, person, title, type);
		}

		private void AssignParish(AiderPersonEntity person)
		{
			if (ParishAssigner.IsInNoParishGroup (person))
			{
				return;
			}

			var parishRepository = ParishAddressRepository.Current;
			var businessContext = this.GetBusinessContext ();

			ParishAssigner.AssignToParish (parishRepository, businessContext, person);
		}
	}
}
