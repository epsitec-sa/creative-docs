//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Controllers.TabIds;
using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class ArticleDocumentItemEntity
	{
		public override DocumentItemTabId TabId
		{
			get
			{
				return DocumentItemTabId.Article;
			}
		}

		public override FormattedText GetCompactSummary()
		{
			var quantity = Helpers.ArticleDocumentItemHelper.GetArticleQuantityAndUnit (this);
			var desc = Misc.FirstLine (Helpers.ArticleDocumentItemHelper.GetArticleDescription (this, shortDescription: true));
			var price = Misc.PriceToString (this.PrimaryLinePriceBeforeTax);

			FormattedText text = TextFormatter.FormatText (quantity, desc, price);

			if (text.IsNullOrEmpty)
			{
				return "<i>Article</i>";
			}
			else
			{
				return text;
			}
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
	}
}
