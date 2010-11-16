﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Rules
{
	[BusinessRule (RuleType.Setup)]
	internal class BusinessDocumentSetupRule : GenericBusinessRule<BusinessDocumentEntity>
	{
		protected override void Apply(BusinessDocumentEntity entity)
		{
			var businessContext = Logic.Current.BusinessContext;

			entity.BillingCurrencyCode = Finance.CurrencyCode.Chf;
			entity.BillingDate = Date.Today;
			entity.BillingStatus = Finance.BillingStatus.NotAnInvoice;
			entity.PriceRefDate = Date.Today;
		}
	}
}
