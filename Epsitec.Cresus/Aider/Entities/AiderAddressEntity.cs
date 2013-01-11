//	Copyright © 2011-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using Epsitec.Common.IO;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using Epsitec.Data.Platform;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Entities
{
	public partial class AiderAddressEntity
	{
		public FormattedText GetPostalAddress()
		{
			return TextFormatter.FormatText (
				this.AddressLine1, "\n",
				this.StreetUserFriendly, this.HouseNumber, this.HouseNumberComplement, "\n",
				this.Town.ZipCode, this.Town.Name, "\n",
				TextFormatter.Command.Mark, this.Town.Country.Name, this.Town.Country.IsoCode, "CH", TextFormatter.Command.ClearToMarkIfEqual);
		}

		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText (
				new FormattedText ("<b>Adresse</b>\n~"),
				this.GetPostalAddress (), "~\n",
				TextFormatter.FormatField (() => this.Phone1), "~(fixe 1)\n",
				TextFormatter.FormatField (() => this.Phone2), "~(fixe 2)\n",
				TextFormatter.FormatField (() => this.Mobile), "~(mobile)\n",
				TextFormatter.FormatField (() => this.Fax), "~(fax)\n",
				UriFormatter.ToFormattedText (this.Email), "~\n",
				UriFormatter.ToFormattedText (this.Web));
		}

		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.Type);
		}

		public override EntityStatus GetEntityStatus()
		{
			using (var a = new EntityStatusAccumulator ())
			{
				a.Accumulate (this.Type == Enumerations.AddressType.None ? EntityStatus.Empty : EntityStatus.Valid);
				a.Accumulate (this.AddressLine1.GetEntityStatus ().TreatAsOptional ());

				if (string.IsNullOrWhiteSpace (this.Street))
				{
					//	If no street is specified, this will be considered to be valid only if
					//	no house number is specified :

					if ((this.HouseNumber.HasValue) ||
						(string.IsNullOrWhiteSpace (this.HouseNumberComplement) == false))
					{
						a.Accumulate (EntityStatus.Empty);
					}
					else
					{
						a.Accumulate (EntityStatus.Empty | EntityStatus.Valid);
					}
				}

				a.Accumulate (this.PostBox.GetEntityStatus ().TreatAsOptional ());
				a.Accumulate (this.Town);
				a.Accumulate (this.Phone1.GetEntityStatus ().TreatAsOptional ());
				a.Accumulate (this.Phone2.GetEntityStatus ().TreatAsOptional ());
				a.Accumulate (this.Mobile.GetEntityStatus ().TreatAsOptional ());
				a.Accumulate (this.Fax.GetEntityStatus ().TreatAsOptional ());

				return a.EntityStatus;
			}
		}


		public IEnumerable<FormattedText> GetAddressLines()
		{
			yield return this.AddressLine1;
			yield return StringUtils.Join (" ", this.StreetUserFriendly, this.HouseNumber, this.HouseNumberComplement);
			yield return this.PostBox;
			yield return StringUtils.Join (" ", StringUtils.Join ("-", this.Town.Country.IsoCode, this.Town.ZipCode), this.Town.Name);
			yield return this.Town.Country.Name;
		}


		public IEnumerable<FormattedText> GetPhones()
		{
			if (!this.Phone1.IsNullOrWhiteSpace ())
			{
				yield return TextFormatter.FormatField (() => this.Phone1);
			}

			if (!this.Phone2.IsNullOrWhiteSpace ())
			{
				yield return TextFormatter.FormatField (() => this.Phone2);
			}

			if (!this.Mobile.IsNullOrWhiteSpace ())
			{
				yield return TextFormatter.FormatField (() => this.Mobile);
			}

			if (!this.Fax.IsNullOrWhiteSpace ())
			{
				yield return TextFormatter.FormatField (() => this.Fax);
			}
		}
		
		partial void GetStreetUserFriendly(ref string value)
		{
			value = SwissPostStreet.ConvertToUserFriendlyStreetName (this.Street);
		}

		partial void SetStreetUserFriendly(string value)
		{
			var town = this.Town;

			if ((town.IsNull ()) ||
				(town.SwissZipCode == null))
			{
				this.Street = value;
			}
			else
			{
				int zipCode = town.SwissZipCode.Value;
				this.Street = SwissPostStreet.ConvertFromUserFriendlyStreetName (zipCode, value) ?? value;
			}
		}
	}
}