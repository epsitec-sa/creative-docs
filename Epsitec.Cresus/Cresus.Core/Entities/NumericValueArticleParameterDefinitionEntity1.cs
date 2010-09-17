//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class NumericValueArticleParameterDefinitionEntity
	{
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
