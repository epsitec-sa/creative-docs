//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class MailContactEntity
	{
		public FormattedText GetTitle()
		{
			return TextFormatter.FormatText ("Adresse", "(", FormattedText.Join (", ", this.ContactGroups.Select (role => role.Name).ToArray ()), ")");
		}

		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText
				(
					this.LegalPerson.Name, "\n",
					this.LegalPerson.Complement, "\n",
					this.NaturalPerson.Firstname, this.NaturalPerson.Lastname, "\n",
					this.Complement, "\n",
					this.Address.Street.StreetName, "\n",
					this.Address.Street.Complement, "\n",
					this.Address.PostBox.Number, "\n",
					TextFormatter.Command.Mark,
					this.Address.Location.Country.CountryCode, "~-", this.Address.Location.PostalCode, TextFormatter.Command.ClearToMarkIfEmpty,
					TextFormatter.Command.Mark,
					this.Address.Location.Name
				);
		}

		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText
				(
					this.LegalPerson.Name, ",",
					this.NaturalPerson.Firstname, this.NaturalPerson.Lastname, ",",
					this.Address.Street.StreetName, ",",
					this.Address.Location.PostalCode, this.Address.Location.Name
				);
		}

		public override string[] GetEntityKeywords()
		{
			return new string[]
			{
				this.LegalPerson.Name.ToSimpleText (),
				this.NaturalPerson.Firstname,
				this.NaturalPerson.Lastname,
				this.Address.Street.StreetName.ToSimpleText (),
				this.Address.Location.PostalCode.ToSimpleText (),
				this.Address.Location.Name.ToSimpleText ()
			};
		}

		public override EntityStatus GetEntityStatus()
		{
			using (var a = new EntityStatusAccumulator ())
			{
				a.Accumulate (this.Address.GetEntityStatus ());
				a.Accumulate (this.Complement.GetEntityStatus ().TreatAsOptional ());
				a.Accumulate (this.ContactGroups.Select (x => x.GetEntityStatus ()));
				a.Accumulate (this.Comments.Select (x => x.GetEntityStatus ()));

				return a.EntityStatus;
			}
		}
	}
}
