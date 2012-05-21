//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class OptionValueEntity
	{
		public override FormattedText GetSummary()
		{
			if (this.ArticleDefinition.IsNull ())
			{
				return TextFormatter.FormatText (this.Name);
			}
			else
			{
				return TextFormatter.FormatText (this.Name, "(", this.Quantity.ToString (), this.ArticleDefinition.Name, ")");
			}
		}

		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.Name);
		}

		public override EntityStatus GetEntityStatus()
		{
			using (var a = new EntityStatusAccumulator ())
			{
				a.Accumulate (this.Name.GetEntityStatus ());
				a.Accumulate (this.Description.GetEntityStatus ().TreatAsOptional ());

				return a.EntityStatus;
			}
		}
	}
}
