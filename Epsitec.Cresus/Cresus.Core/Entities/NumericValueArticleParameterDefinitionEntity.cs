//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Controllers.TabIds;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class NumericValueArticleParameterDefinitionEntity
	{
		public override ArticleParameterTabId TabId
		{
			get
			{
				return ArticleParameterTabId.Numeric;
			}
		}

		public override EntityStatus GetEntityStatus()
		{
			var s1 = this.Code.GetEntityStatus ();
			var s2 = this.Name.GetEntityStatus ();
			var s3 = this.PreferredValues.GetEntityStatus ().TreatAsOptional ();

			return EntityStatusHelper.CombineStatus (StatusHelperCardinality.All, s1, s2, s3);
		}


		protected override void AppendSummary(TextBuilder builder)
		{
			base.AppendSummary (builder);
			builder.Append ("~: ");

			if (this.DefaultValue.HasValue ||
				this.MinValue.HasValue     ||
				this.MaxValue.HasValue)
			{
				builder.Append (this.DefaultValue);
				builder.Append (" (");
				builder.Append (this.MinValue);
				builder.Append ("..");
				builder.Append (this.MaxValue);
				builder.Append (")");
			}
			else
			{
				builder.Append ("<i>Vide</i>");
			}
		}
	}
}
