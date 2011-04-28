//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;

using Epsitec.Cresus.Core.Business.Finance.PriceCalculators.ItemPriceCalculators;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Finance.PriceCalculators
{
	public interface IDocumentPriceCalculator
	{
		BusinessDocumentEntity Document
		{
			get;
		}

		CoreData Data
		{
			get;
		}
		
		void Process(ArticleItemPriceCalculator calculator);
		void Process(SubTotalItemPriceCalculator calculator);
	}
}
