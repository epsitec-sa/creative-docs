//	Copyright © 2011-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using Epsitec.Common.IO;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using Epsitec.Data.Platform;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Entities
{
	public partial class AiderAddressEntity
	{
		public string StreetRoot
		{
			get
			{
				var street = this.Street;

				if (string.IsNullOrEmpty (street))
				{
					return street;
				}

				int pos = street.IndexOf (',');

				if (pos < 0)
				{
					return street;
				}
				else
				{
					return street.Substring (0, pos);
				}
			}
		}

		
		public string GetHtmlForLocationWebServices(string name = null)
		{
			var street  = string.Concat (this.StreetUserFriendly, " ", this.HouseNumberAndComplement);
			var town    = string.Concat (this.Town.ZipCode, " ", this.Town.Name);
			var address = string.Concat (street, ", ", town);

			var what  = UriBuilder.ConvertToAlphaNumericQueryArgument (name);
			var where = UriBuilder.ConvertToAlphaNumericQueryArgument (address);

			var link1 = string.Format (@"&nbsp;&nbsp;<a href=""http://tel.local.ch/fr/q?what={0}&amp;where={1}&amp;typeref=res"" class=""tile-local-header-button"" target=""_blank""></a>", what, where);
			var link2 = string.Format (@"&nbsp;&nbsp;<a href=""http://maps.google.ch/maps?f=q&q={0}"" class=""tile-gmaps-header-button"" target=""_blank""></a>", where);
			
			return link1 + link2;
		}



		public FormattedText GetPostalAddress()
		{
			return TextFormatter.FormatText (
				this.AddressLine1, "\n",
				this.StreetUserFriendly.CapitalizeFirstLetter (), this.HouseNumberAndComplement, "\n",
				this.PostBox, "\n",
				this.Town.ZipCode, this.Town.Name, "\n",
				TextFormatter.Command.Mark, this.Town.Country.Name, this.Town.Country.IsoCode, "CH", TextFormatter.Command.ClearToMarkIfEqual);
		}

		public FormattedText GetShortPostalAddress()
		{
			return TextFormatter.FormatText (
				this.StreetUserFriendly.CapitalizeFirstLetter (), this.HouseNumberAndComplement, "\n",
				this.PostBox, "\n",
				this.Town.ZipCode, this.Town.Name, "\n",
				TextFormatter.Command.Mark, this.Town.Country.Name, this.Town.Country.IsoCode, "CH", TextFormatter.Command.ClearToMarkIfEqual);
		}

		public FormattedText GetDisplayAddress()
		{
			return TextFormatter.FormatText (this.Town.Name, "~,~", this.StreetRoot, this.HouseNumberAndComplement);
		}

		public FormattedText GetDisplayZipCode()
		{
			var town = this.Town;

			if (town.IsNull ())
			{
				return "";
			}

			var country = town.Country;
			var countryCode = country.IsoCode == "CH"
				? null
				: country.IsoCode;

			return TextFormatter.FormatText (countryCode, town.ZipCode);
		}

		public FormattedText GetShortStreetAddress()
		{
			return TextFormatter.FormatText (this.StreetRoot, this.HouseNumberAndComplement);
		}

		public FormattedText GetStreetZipAndTownAddress()
		{
			return TextFormatter.FormatText (this.GetShortStreetAddress (), "~,", this.Town.ZipCode, this.Town.Name);
		}


		public FormattedText GetPhoneSummary()
		{
			return TextFormatter.FormatText (
				TextFormatter.FormatField (() => this.Phone1), "\n",
				TextFormatter.FormatField (() => this.Phone2), "\n",
				TextFormatter.FormatField (() => this.Mobile), "\n",
				TextFormatter.FormatField (() => this.Fax), "~(fax)"
			);
		}

		public FormattedText GetWebEmailSummary()
		{
			return TextFormatter.FormatText (
				UriFormatter.ToFormattedText (this.Email), "\n",
				UriFormatter.ToFormattedText (this.Web, "_blank")
			);
		}


		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText (
				this.GetPostalAddress (), "\n",
				this.GetPhoneSummary (), "\n",
				this.GetWebEmailSummary ()
			);
		}

		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.Town.ZipCode, this.Town.Name, "~,~", this.StreetRoot);
		}

		public override EntityStatus GetEntityStatus()
		{
			using (var a = new EntityStatusAccumulator ())
			{
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
				a.Accumulate (this.Email.GetEntityStatus ().TreatAsOptional ());

				return a.EntityStatus;
			}
		}


		public string GetCleanHouseNumberAndComplement()
		{
			var tuple = this.HouseNumberAndComplement.SplitAfter (x => char.IsDigit (x));

			var houseNumber = InvariantConverter.ParseNullableInt (tuple.Item1);
			var complement  = AiderAddressEntity.ParseHouseNumberComplement (tuple.Item2) ?? "";

			return AiderAddressEntity.GetCleanHouseNumberAndComplement (houseNumber, complement);
		}


		public static AiderAddressEntity Create(BusinessContext context, AiderAddressEntity templateAddress = null)
		{
			var address = context.CreateAndRegisterEntity<AiderAddressEntity> ();

			if (templateAddress.IsNotNull ())
			{
				address.AddressLine1          = templateAddress.AddressLine1;
				address.Street                = templateAddress.Street;
				address.HouseNumber           = templateAddress.HouseNumber;
				address.HouseNumberComplement = templateAddress.HouseNumberComplement;
				address.PostBox               = templateAddress.PostBox;
				address.Town                  = templateAddress.Town;

				address.Web    = templateAddress.Web;
				address.Email  = templateAddress.Email;
				address.Phone1 = templateAddress.Phone1;
				address.Phone2 = templateAddress.Phone2;
				address.Mobile = templateAddress.Mobile;
				address.Fax    = templateAddress.Fax;
			}

			return address;
		}


		partial void GetStreetUserFriendly(ref string value)
		{
			value = SwissPostStreet.ConvertToUserFriendlyStreetName (this.Street);
		}

		partial void SetStreetUserFriendly(string value)
		{
			this.Street = this.ResolveStreetFromUserFriendlyStreetName (value) ?? value;
		}

		private string ResolveStreetFromUserFriendlyStreetName(string value)
		{
			var town = this.Town;

			if ((town.IsNull ()) ||
				(town.SwissZipCode == null) ||
				(town.SwissZipCodeAddOn == null))
			{
				return value;
			}
			else
			{
				int zipCode  = town.SwissZipCode.Value;
				int zipAddOn = town.SwissZipCodeAddOn.Value;

				return SwissPostStreet.ConvertFromUserFriendlyStreetName (zipCode, zipAddOn, value);
			}
		}

		partial void GetHouseNumberAndComplement(ref string value)
		{
			value = AiderAddressEntity.GetCleanHouseNumberAndComplement (this.HouseNumber, this.HouseNumberComplement);
		}

		public static string GetCleanHouseNumberAndComplement(int? houseNumberValue, string houseNumberComplement)
		{
			var houseNumber = houseNumberValue.HasValue ? InvariantConverter.ToString (houseNumberValue) : "";
			var complement  = houseNumberComplement ?? "";

			if (houseNumber.Length == 0)
			{
				//	Degenerate case of a house number + complement might be "B2" where there is no house number
				//	per se (the complement encodes the full "B2").
				
				return complement;
			}

			switch (complement.Length)
			{
				case 0:  return houseNumber;
				case 1:  return houseNumber+complement;
				
				default:
					if (complement.Any (c => char.IsDigit (c)))
					{
						return houseNumber+complement;
					}
					else
					{
						return string.Concat (houseNumber, " ", complement);
					}
			}
		}

		partial void SetHouseNumberAndComplement(string value)
		{
			if (string.IsNullOrWhiteSpace (value))
			{
				this.HouseNumber           = null;
				this.HouseNumberComplement = null;
			}
			else
			{
				var tuple = value.SplitAfter (x => char.IsDigit (x));

				var number = tuple.Item1;
				var compl  = tuple.Item2;

				this.HouseNumber           = AiderAddressEntity.ParseHouseNumber (number);
				this.HouseNumberComplement = AiderAddressEntity.ParseHouseNumberComplement (compl);
			}
		}

		partial void GetStreetHouseNumberAndComplement(ref string value)
		{
			var street = this.StreetUserFriendly;
			var number = this.HouseNumberAndComplement;

			if (string.IsNullOrEmpty (street))
			{
				value = number;
			}
			else
			{
				value = string.Concat (street, " ", number);
				value = value.Trim ();
			}
		}

		partial void SetStreetHouseNumberAndComplement(string value)
		{
			if (string.IsNullOrEmpty (value))
			{
				this.StreetUserFriendly       = "";
				this.HouseNumberAndComplement = "";
			}
			else
			{
				int pos = value.LastIndexOfAny (AiderAddressEntity.digits);

				if (pos < 0)
				{
					this.StreetUserFriendly       = value.Trim ();
					this.HouseNumberAndComplement = "";
				}
				else
				{
					//	We found a digit somewhere in the address. Now, rewind until we reach a
					//	space...

					int numberStart = pos;

					while (--numberStart > 0)
					{
						if (value[numberStart] == ' ')
						{
							if ((numberStart > 2) &&
								(value[numberStart-2] == ' '))
							{
								continue;
							}

							break;
						}
					}

					pos = numberStart+1;

					var streetName  = value.Substring (0, pos).Trim ();
					var houseNumber = new string (value.Substring (pos).Where (c => c != ' ').ToArray ());
					var houseLetter = houseNumber[0];

					if (char.IsLetter (houseLetter))
					{
						var streetNameSuffix = " " + houseLetter;
						var streetNameWithLetter = streetName + streetNameSuffix;
						
						var attempt1 = this.ResolveStreetFromUserFriendlyStreetName (streetName);
						var attempt2 = this.ResolveStreetFromUserFriendlyStreetName (streetNameWithLetter);

						//	Handle special cases such as a street named "Marjovet C"

						if ((attempt2 == null) ||
							(attempt2.Split (',')[0].EndsWith (streetNameSuffix) == false))
						{
							//	OK
						}
						else
						{
							streetName  = streetNameWithLetter;
							houseNumber = houseNumber.Substring (1);
						}
					}

					this.StreetUserFriendly       = streetName;
					this.HouseNumberAndComplement = houseNumber;
				}
			}
		}


		private static int? ParseHouseNumber(string number)
		{
			if (string.IsNullOrEmpty (number))
			{
				return null;
			}
			else
			{
				return InvariantConverter.ToInt (number);
			}
		}

		private static string ParseHouseNumberComplement(string value)
		{
			if (string.IsNullOrWhiteSpace (value))
			{
				return null;
			}

			//	"a" will become "A" (as in "1A", but "1" is encoded as the house number)
			//	"c1" will become "C1" (the full "C1" is the complement of a possibly missing house number)
			//	"Bis" will become "bis" (multiple characters = "bis"/"ter"/...)

			value = value.Trim ();

			int letterCount = value.Count (c => char.IsLetter (c));

			if (letterCount == 1)
			{
				return value.ToUpperInvariant ();
			}
			else
			{
				return value.ToLowerInvariant ();
			}
		}


		partial void GetAddressTextSingleLine(ref string value)
		{
			this.GetAddressTextMultiLine (ref value);

			value = value.Replace ("\n", ", ");
		}


		partial void SetAddressTextSingleLine(string value)
		{
			throw new System.NotImplementedException ("Do not use this method");
		}


		partial void GetAddressTextMultiLine(ref string value)
		{
			value = this.GetPostalAddress ().ToSimpleText ();
		}


		partial void SetAddressTextMultiLine(string value)
		{
			throw new System.NotImplementedException ("Do not use this method");
		}


		private static char[] digits = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
	}
}