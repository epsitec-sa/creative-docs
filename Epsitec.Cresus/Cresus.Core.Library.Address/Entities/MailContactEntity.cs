//	Copyright © 2010-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Core.Library.Address;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class MailContactEntity
	{
		public override FormattedText GetTitle()
		{
			return TextFormatter.FormatText ("Adresse", "(", FormattedText.Join (", ", this.ContactGroups.Select (role => role.Name).ToArray ()), ")");
		}

		public void ResetPersonAddress(LegalPersonEntity person)
		{
			FormattedText text = FormattedText.Empty;

			if (person.IsNotNull ())
			{
				text = TextFormatter.FormatText (person.Name);
			}

			this.PersonAddress = text;
		}

		public void ResetPersonAddress(NaturalPersonEntity person)
		{
			FormattedText text = FormattedText.Empty;

			if (person.IsNotNull ())
			{
				text = TextFormatter.FormatText (person.Title.Name, "\n", person.Firstname, person.Lastname);
			}

			this.PersonAddress = text;
		}

		public override FormattedText GetSummary()
		{
			if (this.Location.Country.CountryCode == this.GetBusinessCountryCode ())
			{
				return this.GetLocalSummary ();
			}
			else
			{
				return this.GetForeignSummary ();
			}
		}

		private string GetBusinessCountryCode()
		{
			return "CH";
			// TODO: aller chercher ce code dans les BusinessSettings !
		}

		private FormattedText GetLocalSummary()
		{
			return TextFormatter.FormatText (this.PersonAddress, "\n", this.Complement, "\n", this.StreetAndHouseNumber, "\n", this.PostBoxNumber, "\n", this.Location.PostalCode, this.Location.Name);
		}

		private FormattedText GetForeignSummary()
		{
			return TextFormatter.FormatText (this.GetLocalSummary (), "\n", this.Location.Country.Name);
		}


		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText
				(
					TextFormatter.FormatText (this.PersonAddress).ToSimpleText ().Split ('\n').Last (), "~,",
					this.StreetAndHouseNumber, "~,",
					this.Location.PostalCode, this.Location.Name
				);
		}

		public override IEnumerable<FormattedText> GetFormattedEntityKeywords()
		{
			yield return TextFormatter.FormatText (this.PersonAddress).Lines.Last ();
			yield return TextFormatter.FormatText (this.StreetAndHouseNumber);
			yield return TextFormatter.FormatText (this.Location.PostalCode);
			yield return TextFormatter.FormatText (this.Location.Name);
		}

		public override EntityStatus GetEntityStatus()
		{
			using (var a = new EntityStatusAccumulator ())
			{
				a.Accumulate (this.PersonAddress.GetEntityStatus ().TreatAsOptional ());
				a.Accumulate (this.Complement.GetEntityStatus ().TreatAsOptional ());
				a.Accumulate (this.StreetName.GetEntityStatus ().TreatAsOptional ());
				a.Accumulate (this.HouseNumber.GetEntityStatus ().TreatAsOptional ());
				a.Accumulate (this.PostBoxNumber.GetEntityStatus ().TreatAsOptional ());
				a.Accumulate (this.Location.GetEntityStatus ().TreatAsOptional ());
				a.Accumulate (this.ContactGroups.Select (x => x.GetEntityStatus ()));
				a.Accumulate (this.Comments.Select (x => x.GetEntityStatus ()));

				return a.EntityStatus;
			}
		}
		

		[Action (ActionClasses.Output, Library.Address.Res.CaptionIds.ActionButton.Print)]
		public void Print()
		{
		}


		partial void GetStreetAndHouseNumber(ref string value)
		{
			var format = StreetAddressConverter.Classify (this);

			var street = this.StreetName;
			var house  = this.HouseNumber;

			value = StreetAddressConverter.MergeStreetAndHouseNumber (street, house, format);
		}

		partial void OnStreetAndHouseNumberChanged(string oldValue, string newValue)
		{
			var format = StreetAddressConverter.Classify (this);
			var split  = StreetAddressConverter.SplitStreetAndHouseNumber (newValue, format);

			this.StreetName  = split.Item1;
			this.HouseNumber = split.Item2;
		}
	}
}
