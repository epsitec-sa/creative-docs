//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Core.Business.Finance;
using Epsitec.Cresus.Core.Extensions;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class ArticleDefinitionEntity
	{
		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText
				(
					"N°~", this.IdA, "\n",
					this.Name
				);
		}

		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.IdA, "~-~", this.Name);
		}

		public override string[] GetEntityKeywords()
		{
			return new string[] { this.IdA, this.Name.ToSimpleText () };
		}

		public override EntityStatus GetEntityStatus()
		{
			using (var a = new EntityStatusAccumulator ())
			{
				a.Accumulate (this.IdA.GetEntityStatus ());
				a.Accumulate (this.IdB.GetEntityStatus ().TreatAsOptional ());
				a.Accumulate (this.IdC.GetEntityStatus ().TreatAsOptional ());

				a.Accumulate (this.Name.GetEntityStatus ());
				a.Accumulate (this.Description.GetEntityStatus ().TreatAsOptional ());
				a.Accumulate (this.ArticleGroups.Select (x => x.GetEntityStatus ()));
				a.Accumulate (this.ArticleCategory.GetEntityStatus ());
				a.Accumulate (this.ArticlePrices.Select (x => x.GetEntityStatus ()));
				a.Accumulate (this.Accounting.Select (x => x.GetEntityStatus ()));
				a.Accumulate (this.BillingUnit.GetEntityStatus ());
				a.Accumulate (this.Units.GetEntityStatus ());
				a.Accumulate (this.Comments.Select (x => x.GetEntityStatus ()));

				return a.EntityStatus;
			}
		}

		public Business.Finance.VatCode GetOutputVatCode()
		{
			if ((this.OutputVatCode.HasValue == false) &&
				(this.ArticleCategory.IsNotNull ()))
			{
				return this.ArticleCategory.DefaultOutputVatCode.GetValueOrDefault (VatCode.None);
			}
			else
			{
				return this.OutputVatCode.GetValueOrDefault ();
			}
		}

		public Business.Finance.VatCode GetInputVatCode()
		{
			if ((this.InputVatCode.HasValue == false) &&
				(this.ArticleCategory.IsNotNull ()))
			{
				return this.ArticleCategory.DefaultInputVatCode.GetValueOrDefault (VatCode.None);
			}
			else
			{
				return this.InputVatCode.GetValueOrDefault ();
			}
		}
		
		public IEnumerable<ArticlePriceEntity> GetArticlePrices(decimal quantity, System.DateTime date, CurrencyCode currencyCode, PriceGroupEntity priceGroup = null)
		{
			var prices = from price in this.ArticlePrices
						 where price.CurrencyCode == currencyCode
						 where date.InRange (price)
						 where quantity.InRange (price.MinQuantity, price.MaxQuantity)
						 select price;

			if (priceGroup.IsNull ())
			{
				return prices;
			}

			//	Find specific article prices which match the specified price group.
			//	If none can be found, return the generic article prices instead.
			
			var match = new List<ArticlePriceEntity> ();

			foreach (var price in prices)
			{
				if (price.PriceGroups.Contains (priceGroup))
				{
					match.Add (price);
				}
			}

			if (match.Count == 0)
			{
				return prices.Where (x => x.PriceGroups.Count == 0);
			}
			else
			{
				return match;
			}
		}
	}
}
