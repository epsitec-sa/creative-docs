//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Aider.Data;
using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;
using Epsitec.Aider.Tools;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Business;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Support;

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

			var aiderHousehold = businessContext.CreateAndRegisterEntity<AiderHouseholdEntity> ();
			person.Household1 = aiderHousehold;
		}

		public override void ApplyUpdateRule(AiderPersonEntity person)
		{
			AiderPersonBusinessRules.UpdateCallName (person);
			AiderPersonBusinessRules.UpdateDisplayName (person);
			AiderPersonBusinessRules.UpdatePersonSex (person);

			this.VerifyParish (person);
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
				return;
			}

			var businessContext = this.GetBusinessContext ();

			if (person.Parish.Group.GroupDef.PathTemplate != AiderGroupIds.Parish)
			{
				var message = Resources.Text ("Vous devez sélectionner un groupe de paroisse pour la paroisse.");

				Logic.BusinessRuleException (person, message);
			}

			var parishes = new List<AiderGroupEntity> ();

			if (person.Household1.IsNotNull ())
			{
				parishes.Add (ParishLocator.FindParish (businessContext, person.Household1.Address));
			}

			if (person.Household2.IsNotNull ())
			{
				parishes.Add (ParishLocator.FindParish (businessContext, person.Household2.Address));
			}

			if (parishes.Contains (person.Parish.Group))
			{
				return;
			}

			if (person.Warnings.Any (x => x.WarningType == WarningType.ParishMismatch))
			{
				return;
			}

			var warning = businessContext.CreateAndRegisterEntity<AiderPersonWarningEntity> ();

			warning.Title       = Resources.Text ("La paroisse ne correspond pas à l'adresse principale");
			warning.WarningType = WarningType.ParishMismatch;

			if (businessContext.AcquireLock ())
			{
				var warnings = person.Warnings;

				warnings.Add (warning);

				businessContext.SaveChanges (LockingPolicy.ReleaseLock);
			}
		}
	}
}
