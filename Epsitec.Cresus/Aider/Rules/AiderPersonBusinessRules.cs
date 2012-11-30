//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;
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

			warning.Title       = new FormattedText ("La paroisse ne correspond pas à l'adresse principale");
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
