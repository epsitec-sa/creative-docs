//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

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

		DocumentMetadataEntity Metadata
		{
			get;
		}

		CoreData Data
		{
			get;
		}

		IBusinessContext BusinessContext
		{
			get;
		}
		
		void Process(ArticleItemPriceCalculator calculator);
		void Process(SubTotalItemPriceCalculator calculator);
	}
}
