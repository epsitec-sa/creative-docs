//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Business.Finance;
using Epsitec.Cresus.Core.Business.Finance.PriceCalculators;
using Epsitec.Cresus.Core.Business.Finance.PriceCalculators.ItemPriceCalculators;
using Epsitec.Cresus.Core.Controllers.TabIds;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class ArticleDocumentItemEntity : ICopyableEntity<ArticleDocumentItemEntity>
	{
		public decimal GetQuantity(ArticleQuantityType quantityType)
		{
			var quantities = this.ArticleQuantities.Where (x => x.QuantityColumn.QuantityType == quantityType);

			return quantities.Sum (x => this.ArticleDefinition.ConvertToBillingUnit (x.Quantity, x.Unit));
		}


		public bool ArticlePriceIncludesTaxes
		{
			get
			{
				return this.ArticleAttributes.HasFlag (ArticleDocumentItemAttributes.ArticlePriceIncludesTaxes);
			}
			set
			{
				if (value)
				{
					this.ArticleAttributes = this.ArticleAttributes | ArticleDocumentItemAttributes.ArticlePriceIncludesTaxes;
				}
				else
				{
					this.ArticleAttributes = this.ArticleAttributes & ~ArticleDocumentItemAttributes.ArticlePriceIncludesTaxes;
				}
			}
		}

		public bool NeverApplyDiscount
		{
			get
			{
				return this.ArticleAttributes.HasFlag (ArticleDocumentItemAttributes.NeverApplyDiscount);
			}
			set
			{
				if (value)
				{
					this.ArticleAttributes = this.ArticleAttributes | ArticleDocumentItemAttributes.NeverApplyDiscount;
				}
				else
				{
					this.ArticleAttributes = this.ArticleAttributes & ~ArticleDocumentItemAttributes.NeverApplyDiscount;
				}
			}
		}

		public bool IsDiscountable
		{
			get
			{
				if ((this.ArticleAttributes.HasFlag (ArticleDocumentItemAttributes.NeverApplyDiscount)) ||
					(this.ArticleAttributes.HasFlag (ArticleDocumentItemAttributes.ArticleNotDiscountable)))
				{
					return false;
				}
				else
				{
					return true;
				}
			}
		}

		public bool HasPartialQuantities
		{
			get
			{
				return this.ArticleAttributes.HasFlag (ArticleDocumentItemAttributes.PartialQuantities);
			}
			set
			{
				if (value)
				{
					this.ArticleAttributes = this.ArticleAttributes | ArticleDocumentItemAttributes.PartialQuantities;
				}
				else
				{
					this.ArticleAttributes = this.ArticleAttributes & ~ArticleDocumentItemAttributes.PartialQuantities;
				}
			}
		}

		public decimal? TotalRevenueBeforeTax
		{
			get
			{
				if (this.TotalRevenueAfterTax.HasValue)
				{
					var vatRate = this.VatRateA * this.VatRatio + this.VatRateB * (1 - this.VatRatio);
					var vatMult = 1 + vatRate;

					return this.TotalRevenueAfterTax.Value / vatMult;
				}
				else
				{
					return null;
				}
			}
		}

		public override FormattedText GetCompactSummary()
		{
			if (this.GetEntityStatus () == EntityStatus.Empty)
			{
				return null;
			}

			var quantity = Helpers.ArticleDocumentItemHelper.GetArticleQuantityAndUnit (this);
			var desc     = this.ArticleDescriptionCache.Lines.FirstOrDefault ().ToSimpleText ();
			var price    = Misc.PriceToString (this.ArticlePriceIncludesTaxes ? this.LinePriceAfterTax2 : this.LinePriceBeforeTax2);

			if (string.IsNullOrEmpty (desc))
			{
				desc = "?";
			}

			return string.Concat (quantity, " ", desc, " ", price);
		}

		public override EntityStatus GetEntityStatus ()
		{
			using (var a = new EntityStatusAccumulator ())
			{
				a.Accumulate (this.ArticleDefinition.GetEntityStatus ());
				a.Accumulate (this.ArticleParameters.GetEntityStatus ().TreatAsOptional ());
				a.Accumulate (this.ArticleQuantities.Select (x => x.GetEntityStatus ()));

				return a.EntityStatus;
			}
		}

		public override void Process(IDocumentPriceCalculator priceCalculator)
		{
			priceCalculator.Process (new ArticleItemPriceCalculator (priceCalculator, this));
		}

		#region ICloneable<ArticleDocumentItemEntity> Members

		void ICopyableEntity<ArticleDocumentItemEntity>.CopyTo(IBusinessContext businessContext, ArticleDocumentItemEntity copy)
		{
			copy.Attributes              = this.Attributes;
			copy.GroupIndex              = this.GroupIndex;
			
			copy.BeginDate               = this.BeginDate;
			copy.EndDate                 = this.EndDate;
			copy.ArticleDefinition       = this.ArticleDefinition;
			copy.ArticleParameters       = this.ArticleParameters;
			copy.ArticleAttributes       = this.ArticleAttributes;

			//	TODO: clone ArticleTraceabilityDetails

			copy.ArticleQuantities.AddRange (this.ArticleQuantities.Select (x => x.CloneEntity (businessContext)));
			copy.Discounts.AddRange (this.Discounts.Select (x => x.CloneEntity (businessContext)));

			copy.ArticleAccountingDefinition = this.ArticleAccountingDefinition;
			copy.VatRateA                    = this.VatRateA;
			copy.VatRateB                    = this.VatRateB;
			copy.VatRatio                    = this.VatRatio;
			copy.UnitPriceBeforeTax1         = this.UnitPriceBeforeTax1;
			copy.UnitPriceBeforeTax2         = this.UnitPriceBeforeTax2;
			copy.UnitPriceAfterTax1          = this.UnitPriceAfterTax1;
			copy.UnitPriceAfterTax2          = this.UnitPriceAfterTax2;
			copy.LinePriceBeforeTax1         = this.LinePriceBeforeTax1;
			copy.LinePriceBeforeTax2         = this.LinePriceBeforeTax2;
			copy.LinePriceAfterTax1          = this.LinePriceAfterTax1;
			copy.LinePriceAfterTax2          = this.LinePriceAfterTax2;
			copy.TotalRevenueAfterTax                = this.TotalRevenueAfterTax;
			copy.TotalRevenueAccounted       = this.TotalRevenueAccounted;
			copy.ArticleNameCache            = this.ArticleNameCache;
			copy.ArticleDescriptionCache     = this.ArticleDescriptionCache;
			copy.ReplacementName             = this.ReplacementName;
			copy.ReplacementDescription      = this.ReplacementDescription;
		}

		#endregion
	}
}
