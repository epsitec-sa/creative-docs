//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Entities;

namespace Epsitec.Cresus.Core.Business.Rules
{
	[BusinessRule]
	internal class CustomerBusinessRules : GenericBusinessRule<CustomerEntity>
	{
		public override void ApplyBindRule(CustomerEntity customer)
		{
			var businessContext = Logic.Current.GetComponent<BusinessContext> ();
			businessContext.Register (customer.Relation);
		}

		public override void ApplySetupRule(CustomerEntity customer)
		{
			var generatorPool   = Logic.Current.GetComponent<RefIdGeneratorPool> ();

			var generator = generatorPool.GetGenerator<CustomerEntity> ();
			var nextId    = generator.GetNextId ();

			customer.IdA = string.Format ("{0:000000}", nextId);
		}
	}
}
