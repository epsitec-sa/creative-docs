//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class MailContactEntity
	{
		public FormattedText GetSummary()
		{
			return TextFormatter.FormatText (this.LegalPerson.Name, "\n",
											 this.LegalPerson.Complement, "\n",
											 this.Complement, "\n",
											 this.Address.Street.StreetName, "\n",
											 this.Address.Street.Complement, "\n",
											 this.Address.PostBox.Number, "\n",
											 TextFormatter.FormatText (this.Address.Location.Country.Code, "~-", this.Address.Location.PostalCode),
											 this.Address.Location.Name);
		}

		public FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.Address.Street.StreetName, "~,", this.Address.Location.PostalCode, this.Address.Location.Name);
		}
	}
}
