//	Copyright � 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Cresus.Core.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.BusinessLogic.Rules
{
	[BusinessRule (RuleType.Setup)]
	internal class ArticlePriceSetupRule : GenericBusinessRule<ArticlePriceEntity>
	{
		protected override void Apply(ArticlePriceEntity price)
		{
			price.CurrencyCode = Business.Finance.CurrencyCode.Chf;
			price.MinQuantity = 1;
		}
	}
}
