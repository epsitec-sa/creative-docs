//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class MailContactEntity
	{
		public FormattedText GetTitle()
		{
			return TextFormatter.FormatText ("Adresse", "(", FormattedText.Join (", ", this.Roles.Select (role => role.Name).ToArray ()), ")");
		}

		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText
				(
					this.LegalPerson.Name, "\n",
					this.LegalPerson.Complement, "\n",
					TextFormatter.FormatText (this.NaturalPerson.Firstname, this.NaturalPerson.Lastname, "\n"),
					this.Complement, "\n",
					this.Address.Street.StreetName, "\n",
					this.Address.Street.Complement, "\n",
					this.Address.PostBox.Number, "\n",
					TextFormatter.FormatText (this.Address.Location.Country.Code, "~-", this.Address.Location.PostalCode),
					this.Address.Location.Name
				);
		}

		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText
				(
					this.LegalPerson.Name, "~,",
					TextFormatter.FormatText (this.NaturalPerson.Firstname, this.NaturalPerson.Lastname), "~,",
					this.Address.Street.StreetName, "~,",
					this.Address.Location.PostalCode, this.Address.Location.Name
				);
		}

		public override string[] GetTextArray()
		{
			return new string[]
			{
				this.LegalPerson.Name.ToSimpleText (),
				this.NaturalPerson.Firstname.ToSimpleText (),
				this.NaturalPerson.Lastname.ToSimpleText (),
				this.Address.Street.StreetName.ToSimpleText (),
				this.Address.Location.PostalCode.ToSimpleText (),
				this.Address.Location.Name.ToSimpleText ()
			};
		}
	}
}
