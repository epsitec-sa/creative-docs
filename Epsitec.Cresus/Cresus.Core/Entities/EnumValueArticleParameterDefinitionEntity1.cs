//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class EnumValueArticleParameterDefinitionEntity
	{
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
