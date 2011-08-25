//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Data;
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
			var businessContext = Logic.Current.GetComponent<BusinessContext> ();
			var generatorPool   = Logic.Current.GetComponent<RefIdGeneratorPool> ();


			var generator = generatorPool.GetGenerator<CustomerEntity> ();
			var nextId    = generator.GetNextId ();

			customer.IdA = string.Format ("{0:000000}", nextId);
			customer.DefaultBillingMode  = Business.Finance.BillingMode.IncludingTax;
			customer.Workflow = WorkflowFactory.CreateDefaultWorkflow<CustomerEntity> (businessContext);
			customer.Code = (string) ItemCodeGenerator.NewCode ();
		}

		public override void ApplyUpdateRule(CustomerEntity customer)
		{
			customer.Affairs.ForEach (affair => affair.Customer = customer);
		}
	}
}
