//	Copyright � 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class CountryEntity
	{
		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText
				(
					"Pays: ", this.Name, "\n",
					"Code: ", this.Code
				);
		}

		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.Name, "(", this.Code, ")");
		}

		public override string[] GetEntityKeywords()
		{
			return new string[] { this.Name.ToSimpleText (), this.Code };
		}
	}
}
