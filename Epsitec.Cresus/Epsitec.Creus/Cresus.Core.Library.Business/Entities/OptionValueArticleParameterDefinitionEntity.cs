//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Controllers.TabIds;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class OptionValueArticleParameterDefinitionEntity
	{
		public override ArticleParameterTabId TabId
		{
			get
			{
				return ArticleParameterTabId.Option;
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

			int count = this.Options.Count;

			if (count == 0)
			{
				builder.Append ("Aucune option");
			}
			else if (count == 1)
			{
				builder.Append ("1 option");
			}
			else
			{
				builder.Append (string.Format ("{0} options", count.ToString ()));
			}
		}
	}
}
