//	Copyright © 2010-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Controllers.TabIds;
using Epsitec.Common.Support.EntityEngine;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class FreeTextValueArticleParameterDefinitionEntity
	{
		public override ArticleParameterTabId TabId
		{
			get
			{
				return ArticleParameterTabId.FreeText;
			}
		}

		public override EntityStatus GetEntityStatus()
		{
			using (var a = new EntityStatusAccumulator ())
			{
				a.Accumulate (this.Code.GetEntityStatus ());
				a.Accumulate (this.Name.GetEntityStatus ());
				a.Accumulate (this.Description.GetEntityStatus ().TreatAsOptional ());

				return a.EntityStatus;
			}
		}


		protected override void AppendSummary(TextBuilder builder)
		{
			base.AppendSummary (builder);
			builder.Append ("~: ");

			if (!this.ShortText.IsNullOrWhiteSpace)
			{
				builder.Append (this.ShortText);
			}
			else
			{
				builder.Append ("<i>Vide</i>");
			}
		}
	}
}
