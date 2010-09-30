//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class PaymentModeEntity
	{
		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText
				(
					"Code: ",        this.Code,        "\n",
					"Résumé: ",      this.Name,        "\n",
					"Description: ", this.Description, "\n",
					"Compte: ",      this.BookAccount
				);
		}

		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.Name);
		}

		public override string[] GetTextArray()
		{
			return new string[] { this.Name.ToSimpleText () };
		}
	}
}
