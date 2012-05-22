//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Entities;

namespace Epsitec.Cresus.Core.Library.Address
{
	/// <summary>
	/// The <c>StreetAddressConverter</c> class is dedicated to converting street names and
	/// house numbers to full street names, and the other way round.
	/// </summary>
	public static class StreetAddressConverter
	{
		public static StreetAddressFormat Classify(MailContactEntity address)
		{
			if ((address.IsNull ()) ||
				(address.Location.IsNull ()) ||
				(address.Location.Country.IsNull ()))
			{
				return StreetAddressFormat.Default;
			}

			var countryCode = address.Location.Country.CountryCode;
			var regionCode  = address.Location.Region.RegionCode ?? "";

			if (countryCode == "FR")
			{
				return StreetAddressFormat.HouseNumberBeforeStreetName;
			}
			if ((countryCode == "CH") &&
				(regionCode == "GE"))
			{
				return StreetAddressFormat.HouseNumberBeforeStreetName;
			}

			return StreetAddressFormat.Default;
		}

		public static System.Tuple<string, int?, string> SplitHouseNumber(string value)
		{
			var extractor = new CharacterExtractor (value);

			string prefix = extractor.GetNextText ();
			int?   number = extractor.GetNextDigits ();
			string suffix = extractor.GetNextText ();

			return new System.Tuple<string, int?, string> (prefix, number, suffix);
		}

		public static System.Tuple<string, string> SplitStreetAndHouseNumber(string value, StreetAddressFormat format)
		{
			if (value.IsNullOrWhiteSpace ())
			{
				return new System.Tuple<string, string> ("", "");
			}

			switch (format)
			{
				case StreetAddressFormat.Default:
					return StreetAddressConverter.SplitDefault (value.Trim ());

				case StreetAddressFormat.HouseNumberBeforeStreetName:
					return StreetAddressConverter.SplitHouseNumberBeforeStreetName (value.Trim ());

				default:
					throw StreetAddressConverter.GetNotSupportedException (format);
			}
		}

		public static string MergeHouseNumber(string prefix, int? number, string suffix)
		{
			var value = InvariantConverter.ConvertToString (number);
			return string.Concat (prefix, value, suffix);
		}

		public static string MergeStreetAndHouseNumber(string streetName, string houseNumber, StreetAddressFormat format)
		{
			switch (format)
			{
				case StreetAddressFormat.Default:
					return StringExtensions.JoinNonEmpty (" ", streetName, houseNumber);

				case StreetAddressFormat.HouseNumberBeforeStreetName:
					return StringExtensions.JoinNonEmpty (", ", streetName, houseNumber);

				default:
					throw StreetAddressConverter.GetNotSupportedException (format);
			}
		}

		public static IValidationResult Validate(string value, StreetAddressFormat format)
		{
			switch (format)
			{
				case StreetAddressFormat.Default:
					return StreetAddressConverter.ValidateDefault (value);

				case StreetAddressFormat.HouseNumberBeforeStreetName:
					return StreetAddressConverter.ValidateHouseNumberBeforeStreetName (value);

				default:
					throw StreetAddressConverter.GetNotSupportedException (format);
			}
		}

		
		private static System.Tuple<string, string> SplitDefault(string value)
		{
			int pos = value.LastIndexOf (' ');

			if (pos < 0)
			{
				return new System.Tuple<string, string> (value, "");
			}

			System.Diagnostics.Debug.Assert (pos+1 < value.Length);

			var house  = value.Substring (pos+1);
			var street = value.Substring (0, pos).TrimEnd (' ');

			if (char.IsDigit (house[0]))
			{
				return new System.Tuple<string, string> (street, house);
			}
			else
			{
				return new System.Tuple<string, string> (value, "");
			}
		}

		private static System.Tuple<string, string> SplitHouseNumberBeforeStreetName(string value)
		{
			int pos = value.IndexOf (',');

			if (pos <= 0)
			{
				return new System.Tuple<string, string> (value, "");
			}

			var street = value.Substring (pos+1).TrimStart (' ');
			var house  = value.Substring (0, pos).TrimEnd (' ');

			return new System.Tuple<string, string> (street, house);
		}

		private static IValidationResult ValidateDefault(string value)
		{
			if (string.IsNullOrEmpty (value))
			{
				return ValidationResult.Ok;
			}
			if (value.Contains (","))
			{
				return ValidationResult.CreateError ("Le caractère virgule (,) n'est pas admis dans une adresse<br/>" +
					/**/							 "de rue telle que <i>rue de la Gare 23</i>");
			}
			else
			{
				return ValidationResult.Ok;
			}
		}

		private static IValidationResult ValidateHouseNumberBeforeStreetName(string value)
		{
			if (string.IsNullOrEmpty (value))
			{
				return ValidationResult.Ok;
			}

			int pos = value.IndexOf (',');
			
			if (pos > 0)
			{
				if (value.CountOccurences (",") > 1)
				{
					return ValidationResult.CreateError ("Le caractère virgule (,) ne peut apparaître qu'une seule fois<br/>" +
						/**/							 "dans une adresse de rue telle que <i>23, rue de la Gare</i>");
				}

				var house = value.Substring (0, pos);
				int num   = InvariantConverter.ParseInt (house);

				if ((house.Length > 8) ||
					(num == 0))
				{
					return ValidationResult.CreateWarning ("L'élément <b>{0}</b> ne semble pas être un numéro de maison<br/>" +
						/**/							   "valide dans une adresse de rue telle que <i>23, rue de la Gare</i>", house.TruncateAndAddEllipsis (5));
				}
			}

			return ValidationResult.Ok;
		}
		
		private static System.NotSupportedException GetNotSupportedException(StreetAddressFormat format)
		{
			return new System.NotSupportedException (string.Format ("{0} is not supported", format.GetQualifiedName ()));
		}
	}
}
