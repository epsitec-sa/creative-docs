//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Finance
{
	public class PriceCalculator
	{
		public PriceCalculator(BusinessDocumentEntity document, ArticleDocumentItemEntity articleItem)
		{
			this.document     = document;
			this.articleItem  = articleItem;
			this.articleDef   = this.articleItem.ArticleDefinition;
			this.currencyCode = this.document.BillingCurrencyCode;
			this.date         = this.document.PriceRefDate.GetValueOrDefault (Date.Today).ToDateTime ();
		}


		public IEnumerable<ArticlePriceEntity> GetArticlePrices(decimal quantity)
		{
			var prices = from price in this.articleDef.ArticlePrices
						 where price.CurrencyCode == this.currencyCode
						 where this.date.InRange (price)
						 where quantity.InRange (price.MinQuantity, price.MaxQuantity)
						 select price;

			return prices;
		}

		private readonly BusinessDocumentEntity		document;
		private readonly ArticleDocumentItemEntity	articleItem;
		private readonly ArticleDefinitionEntity	articleDef;
		private readonly CurrencyCode				currencyCode;
		private readonly System.DateTime			date;

	}
}
