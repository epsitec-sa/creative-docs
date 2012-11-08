//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Aider.Entities;
using Epsitec.Aider.Tools;

using Epsitec.Common.Types;

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
		}

		public override void ApplyUpdateRule(AiderPersonEntity person)
		{
			if (string.IsNullOrWhiteSpace (person.CallName))
			{
				person.CallName = eCH_PersonEntity.GetDefaultFirstName (person.eCH_Person);
			}

			person.DisplayName = AiderPersonEntity.GetDisplayName (person);

			this.VerifyParish (person);
			
			if (person.eCH_Person.DataSource != Enumerations.DataSource.Government)
			{
				switch (person.MrMrs)
				{
					case Enumerations.PersonMrMrs.Madame:
					case Enumerations.PersonMrMrs.Mademoiselle:
						person.eCH_Person.PersonSex = Enumerations.PersonSex.Female;
						break;

					case Enumerations.PersonMrMrs.Monsieur:
						person.eCH_Person.PersonSex = Enumerations.PersonSex.Male;
						break;
				}
			}
		}

		public override void ApplyValidateRule(AiderPersonEntity entity)
		{
			//	TODO: ensure that person's MrMrs is compatible with person's sex
		}

		private void VerifyParish(AiderPersonEntity person)
		{
			if (person.Parish.IsNull ())
			{
				return;
			}

			var businessContext = this.GetBusinessContext ();

			var parish1 = ParishLocator.FindParish (businessContext, person.Household1.Address);
			var parish2 = ParishLocator.FindParish (businessContext, person.Household2.Address);

			if ((parish1 == person.Parish.Group) ||
				(parish2 == person.Parish.Group))
			{
				return;
			}
			
			if (person.Warnings.Any (x => x.WarningType == Enumerations.WarningType.ParishMismatch))
			{
				return;
			}

			var warning = businessContext.CreateAndRegisterEntity<AiderPersonWarningEntity> ();

			warning.Title       = new FormattedText ("La paroisse ne correspond pas à l'adresse principale");
			warning.WarningType = Enumerations.WarningType.ParishMismatch;

			if (businessContext.AcquireLock ())
			{
				var warnings = person.Warnings;

				warnings.Add (warning);

				businessContext.SaveChanges (LockingPolicy.ReleaseLock);
			}
		}
	}
}
