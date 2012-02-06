//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Aider.Entities;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;


namespace Epsitec.Aider.Rules
{
	[BusinessRule]
	internal class AiderPersonBusinessRules : GenericBusinessRule<AiderPersonEntity>
	{
		public override void ApplySetupRule(AiderPersonEntity aiderPerson)
		{
			var businessContext = Logic.Current.GetComponent<BusinessContext> ();

			var echPerson = businessContext.CreateEntity<eCH_PersonEntity> ();
			echPerson.CreationDate = Date.Today;
			echPerson.DataSource = Enumerations.DataSource.Undefined;

			aiderPerson.eCH_Person = echPerson;
#if false
			var additionalAddress1 = businessContext.CreateEntity<AiderAddressEntity> ();
			aiderPerson.AdditionalAddress1 = additionalAddress1;

			var additionalAddress2 = businessContext.CreateEntity<AiderAddressEntity> ();
			aiderPerson.AdditionalAddress2 = additionalAddress2;

			var additionalAddress3 = businessContext.CreateEntity<AiderAddressEntity> ();
			aiderPerson.AdditionalAddress3 = additionalAddress3;

			var additionalAddress4 = businessContext.CreateEntity<AiderAddressEntity> ();
			aiderPerson.AdditionalAddress4 = additionalAddress4;
#endif
		}
	}
}
