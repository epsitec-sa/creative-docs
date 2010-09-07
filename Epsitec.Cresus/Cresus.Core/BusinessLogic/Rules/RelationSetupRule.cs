//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.BusinessLogic.Rules
{
	[BusinessRule (RuleType=RuleType.Setup)]
	internal class RelationSetupRule : GenericBusinessRule<RelationEntity>
	{
		protected override void Apply(RelationEntity relation)
		{
			relation.IdA = "1000";
			relation.FirstContactDate = Date.Today;
			relation.TaxMode = Business.Finance.TaxMode.LiableForVat;
			relation.DefaultBillingMode =	Business.Finance.BillingMode.IncludingTax;
			relation.DefaultCurrencyCode = Business.Finance.CurrencyCode.Chf;
		}
	}
}
