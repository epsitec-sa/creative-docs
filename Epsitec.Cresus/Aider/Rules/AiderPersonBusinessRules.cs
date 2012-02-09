//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Aider.Entities;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;


namespace Epsitec.Aider.Rules
{
	[BusinessRule]
	internal class AiderPersonBusinessRules : GenericBusinessRule<AiderPersonEntity>
	{
		public override void ApplySetupRule(AiderPersonEntity person)
		{
			var businessContext = Logic.Current.GetComponent<BusinessContext> ();

			var echPerson = businessContext.CreateEntity<eCH_PersonEntity> ();
			
			echPerson.CreationDate = Date.Today;
			echPerson.DataSource   = Enumerations.DataSource.Undefined;

			person.eCH_Person = echPerson;
		}

		public override void ApplyUpdateRule(AiderPersonEntity person)
		{
			if (string.IsNullOrWhiteSpace (person.CallName))
			{
				person.CallName = eCH_PersonEntity.GetDefaultFirstName (person.eCH_Person);
			}

			person.DisplayName = AiderPersonEntity.GetDisplayName (person);
		}

	}
}
