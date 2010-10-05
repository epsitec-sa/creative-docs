//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Helpers;

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
					this.ShortDescription
				);
		}

		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.IdA, "~-", this.ShortDescription);
		}

		public override string[] GetEntityKeywords()
		{
			return new string[] { this.IdA, this.ShortDescription.ToSimpleText () };
		}

		public override EntityStatus GetEntityStatus()
		{
			using (var a = new EntityStatusAccumulator ())
			{
				a.Accumulate (this.IdA.GetEntityStatus ());
				a.Accumulate (this.IdB.GetEntityStatus ().TreatAsOptional ());
				a.Accumulate (this.IdC.GetEntityStatus ().TreatAsOptional ());

				a.Accumulate (this.ShortDescription.GetEntityStatus ());
				a.Accumulate (this.LongDescription.GetEntityStatus ().TreatAsOptional ());
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
	}
}
