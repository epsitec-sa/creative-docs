//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Rules
{
	[BusinessRule]
	internal class BusinessDocumentBusinessRules : GenericBusinessRule<BusinessDocumentEntity>
	{
		public override void ApplySetupRule(BusinessDocumentEntity entity)
		{
			var businessContext = Logic.Current.GetComponent<BusinessContext> ();

			entity.CurrencyCode = Finance.CurrencyCode.Chf;
			entity.BillingDate = Date.Today;
			entity.BillingStatus = Finance.BillingStatus.NotAnInvoice;
			entity.PriceRefDate = Date.Today;
		}
		
		public override void ApplyUpdateRule(BusinessDocumentEntity entity)
		{
			var businessContext = Logic.Current.GetComponent<BusinessContext> ();

			Epsitec.Cresus.Core.Business.Finance.PriceCalculator.UpdatePrices (businessContext, entity);
		}
	}
}