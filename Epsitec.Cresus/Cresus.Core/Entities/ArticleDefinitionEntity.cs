//	Copyright � 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

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
					"N�~", this.IdA, "\n",
					this.ShortDescription
				);
		}

		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.IdA, "~-", this.ShortDescription);
		}

		public override string[] GetTextArray()
		{
			return new string[] { this.IdA, this.ShortDescription.ToSimpleText () };
		}
	}
}
