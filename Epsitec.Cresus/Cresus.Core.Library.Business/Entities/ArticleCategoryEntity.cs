//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Extensions;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class ArticleCategoryEntity
	{
		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText (this.Name);
		}

		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.Name);
		}

		public override string[] GetEntityKeywords()
		{
			return new string[] { this.Name.ToSimpleText () };
		}

		public override EntityStatus GetEntityStatus()
		{
			using (var a = new EntityStatusAccumulator ())
			{
				a.Accumulate (this.Name.GetEntityStatus ());
				a.Accumulate (this.UnitOfMeasureCategory == Business.UnitOfMeasureCategory.None ? EntityStatus.Empty : EntityStatus.Valid);
				a.Accumulate (this.DefaultPictures);
				a.Accumulate (this.Accounting, EntityStatusAccumulationMode.NoneIsInvalid);

				return a.EntityStatus;
			}
		}

		public ArticleAccountingDefinitionEntity GetArticleAccountingDefinition(System.DateTime date)
		{
			var matches = from accounting in this.Accounting
						  where date.InRange (accounting)
						  select accounting;

			return matches.FirstOrDefault ();
		}
	}
}
