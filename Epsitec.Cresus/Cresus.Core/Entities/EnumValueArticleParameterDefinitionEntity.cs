//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Controllers.TabIds;
using Epsitec.Common.Support.EntityEngine;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class EnumValueArticleParameterDefinitionEntity
	{
		public override ArticleParameterTabId TabId
		{
			get
			{
				return ArticleParameterTabId.Enum;
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

			if (!string.IsNullOrWhiteSpace (this.DefaultValue) &&
				!string.IsNullOrWhiteSpace (this.Values))
			{
				builder.Append (this.DefaultValue);
				builder.Append (" (");
				builder.Append (Epsitec.Cresus.Core.Controllers.EditionControllers.Common.EnumInternalToSingleLine (this.Values));
				builder.Append (")");
			}
			else
			{
				builder.Append ("<i>Vide</i>");
			}
		}
	}
}
