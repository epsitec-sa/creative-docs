//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Rules
{
	[BusinessRule (RuleType.Update)]
	internal class BusinessDocumentUpdateRule : GenericBusinessRule<BusinessDocumentEntity>
	{
		protected override void Apply(BusinessDocumentEntity entity)
		{
			var businessContext = Logic.Current.BusinessContext;
            
			Epsitec.Cresus.Core.Business.Finance.PriceCalculator.UpdatePrices (businessContext, entity);
		}
	}
}
