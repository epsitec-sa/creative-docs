//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Controllers.TabIds;

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

		public static string EnumInternalToSingleLine(string value)
		{
			if (string.IsNullOrEmpty (value))
			{
				return null;
			}
			else
			{
				return value.Replace (AbstractArticleParameterDefinitionEntity.Separator, ", ");
			}
		}

		public static string EnumInternalToMultiLine(string value)
		{
			if (string.IsNullOrEmpty (value))
			{
				return null;
			}
			else
			{
				return value.Replace (AbstractArticleParameterDefinitionEntity.Separator, FormattedText.HtmlBreak);
			}
		}

		public static string EnumSingleLineToInternal(string value)
		{
			if (string.IsNullOrEmpty (value))
			{
				return null;
			}
			else
			{
				return value.Replace (", ", AbstractArticleParameterDefinitionEntity.Separator);
			}
		}

		public static string EnumMultiLineToInternal(string value)
		{
			if (string.IsNullOrEmpty (value))
			{
				return null;
			}
			else
			{
				return value.Replace (FormattedText.HtmlBreak, AbstractArticleParameterDefinitionEntity.Separator);
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
				builder.Append (EnumValueArticleParameterDefinitionEntity.EnumInternalToSingleLine (this.Values));
				builder.Append (")");
			}
			else
			{
				builder.Append ("<i>Vide</i>");
			}
		}
	}
}
