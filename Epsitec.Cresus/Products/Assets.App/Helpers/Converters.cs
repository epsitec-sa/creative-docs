﻿//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Types;

namespace Epsitec.Cresus.Assets.App.Helpers
{
	public static class Converters
	{
		#region Pourcentage
		public static decimal? ParsePercent(FormattedText text)
		{
			return Converters.ParsePercent (text.ToSimpleText ());
		}

		public static decimal? ParsePercent(string text)
		{
			if (string.IsNullOrEmpty (text))
			{
				return null;
			}

			if (text.Contains ('%'))
			{
				text = text.Replace ("%", "");
			}

			decimal d;
			if (decimal.TryParse (text, out d))
			{
				return d / 100;
			}
			else
			{
				return null;
			}
		}

		public static string PercentToString(decimal? percent)
		{
			if (percent.HasValue)
			{
				string s;

				if (Converters.percentFracFormat == SettingsEnum.PercentFloating)
				{
					s = (percent.Value*100).ToString (System.Globalization.CultureInfo.InvariantCulture);

					while (s.Length > 1 && s.EndsWith ("0"))
					{
						s = s.Substring (0, s.Length-1);  // TODO: on doit pouvoir faire plus simple...
					}

					if (s.EndsWith ("."))
					{
						s = s.Substring (0, s.Length-1);
					}
				}
				else
				{
					string format = null;

					switch (Converters.percentFracFormat)
					{
						case SettingsEnum.PercentFrac1:
							format = "0.0";
							break;

						case SettingsEnum.PercentFrac2:
							format = "0.00";
							break;

						case SettingsEnum.PercentFrac3:
							format = "0.000";
							break;
					}

					s = (percent.Value*100).ToString (format);
				}

				if (Converters.percentDecimalSeparator == SettingsEnum.SeparatorComma)
				{
					s = s.Replace (".", ",");
				}

				return s + "%";
			}
			else
			{
				return null;
			}
		}
		#endregion


		#region Montant
		public static decimal? ParseMontant(FormattedText text)
		{
			return Converters.ParseMontant (text.ToSimpleText ());
		}

		public static decimal? ParseMontant(string text)
		{
			//	Parse un montant, selon les réglages.
			if (string.IsNullOrEmpty (text))
			{
				return null;
			}

			decimal neg = 1;

			if (text.StartsWith ("("))
			{
				neg = -1;
				text = text.Substring (1).Replace (")", "");
			}

			if (Converters.numberFormatNullParts != SettingsEnum.NullPartsDashZero &&
				Converters.numberFormatNullParts != SettingsEnum.NullPartsDashDash)  // ne commence pas par un tiret si zéro ?
			{
				if (text.StartsWith ("-"))
				{
					neg = -1;
					text = text.Substring (1);
				}
			}

			text = text.Replace ("-", "0");
			text = text.Replace ("—", "0");

			decimal d;
			if (decimal.TryParse (text, out d))
			{
				return d*neg;
			}

			return null;
		}

		public static string MontantToString(decimal? montant, int decimalDigits = 2)
		{
			//	Conversion d'un montant, selon les réglages.
			if (montant.HasValue)
			{
				Converters.numberFormatMontant.CurrencyDecimalDigits = decimalDigits;

				bool neg = false;
				if (montant < 0)
				{
					neg = true;
					montant = -montant;
				}

				string s = montant.Value.ToString ("C", Converters.numberFormatMontant);

				if (Converters.numberFormatNullParts == SettingsEnum.NullPartsDashZero ||
					Converters.numberFormatNullParts == SettingsEnum.NullPartsDashDash)  // commence par un tiret si zéro ?
				{
					string pattern = "0" + Converters.numberFormatMontant.CurrencyDecimalSeparator;  // "0."
					if (s.StartsWith (pattern))
					{
						s = "-" + s.Substring (1);
					}
				}

				if (Converters.numberFormatNullParts == SettingsEnum.NullPartsZeroDash ||
					Converters.numberFormatNullParts == SettingsEnum.NullPartsDashDash)  // termine par un tiret long ?
				{
					string pattern = Converters.numberFormatMontant.CurrencyDecimalSeparator + new string ('0', Converters.numberFormatMontant.CurrencyDecimalDigits);  // ".00"
					if (s.EndsWith (pattern))
					{
						s = s.Substring (0, s.Length-pattern.Length+1) + "—";  // tiret long
					}
				}

				if (neg)
				{
					if (Converters.numberFormatNegative == SettingsEnum.NegativeParentheses)
					{
						s = "(" + s + ")";
					}
					else
					{
						s = "-" + s;
					}
				}

				return s;
			}
			else
			{
				return null;
			}
		}
		#endregion


		#region Decimal
		public static decimal? ParseDecimal(FormattedText text)
		{
			return Converters.ParseMontant (text.ToSimpleText ());
		}

		public static decimal? ParseDecimal(string text)
		{
			//	Parse un nombre réel.
			if (string.IsNullOrEmpty (text))
			{
				return null;
			}

			decimal i;
			if (decimal.TryParse (text, out i))
			{
				return i;
			}

			return null;
		}

		public static string DecimalToString(decimal? value, int? fracCount)
		{
			//	Conversion d'un nombre réel en chaîne.
			if (value.HasValue)
			{
				if (fracCount.HasValue)
				{
					string format = string.Format ("F{0}", fracCount.Value.ToString ());
					return value.Value.ToString (format);
				}
				else
				{
					return value.Value.ToString ();
				}
			}
			else
			{
				return null;
			}
		}
		#endregion


		#region Int
		public static int? ParseInt(FormattedText text)
		{
			return Converters.ParseInt (text.ToSimpleText ());
		}

		public static int? ParseInt(string text)
		{
			//	Parse un entier.
			int i;
			if (int.TryParse (text, out i))
			{
				return i;
			}

			return null;
		}

		public static string IntToString(int? value)
		{
			if (value.HasValue)
			{
				return value.Value.ToString ();
			}
			else
			{
				return null;
			}
		}
		#endregion


		#region Date
		public static System.DateTime? ParseDate(FormattedText text)
		{
			return Converters.ParseDate (text.ToSimpleText ());
		}

		public static System.DateTime? ParseDate(string text)
		{
			//	Parse une date située dans n'importe quelle période.
			System.DateTime? date;
			Converters.ParseDate (text, System.DateTime.Today, null, null, out date);
			return date;
		}

		public static bool ParseDate(string text, System.DateTime defaultDate, System.DateTime? minDate, System.DateTime? maxDate, out System.DateTime? date)
		{
			if (string.IsNullOrEmpty (text))
			{
				date = null;
				return true;
			}

			text = text.Replace (".", " ");
			text = text.Replace (",", " ");
			text = text.Replace ("/", " ");
			text = text.Replace ("-", " ");
			text = text.Replace (":", " ");
			text = text.Replace (";", " ");
			text = text.Replace ("  ", " ");
			text = text.Replace ("  ", " ");

			var words = text.Split (' ');

			int? n1 = null;
			int? n2 = null;
			int? n3 = null;

			if (words.Length >= 1)
			{
				n1 = Converters.ParseInt (words[0]);
			}

			if (words.Length >= 2)
			{
				n2 = Converters.ParseInt (words[1]);
			}

			if (words.Length >= 3)
			{
				n3 = Converters.ParseInt (words[2]);
			}

			int y, m, d;

			if (Converters.dateFormatOrder == SettingsEnum.YearYMD)
			{
				if (n1.HasValue)
				{
					y = n1.Value;
				}
				else
				{
					y = defaultDate.Year;
				}

				if (n2.HasValue)
				{
					m = n2.Value;
				}
				else
				{
					m = defaultDate.Month;
				}

				if (n3.HasValue)
				{
					d = n3.Value;
				}
				else
				{
					d = defaultDate.Day;
				}
			}
			else
			{
				if (n1.HasValue)
				{
					d = n1.Value;
				}
				else
				{
					d = defaultDate.Day;
				}

				if (n2.HasValue)
				{
					m = n2.Value;
				}
				else
				{
					m = defaultDate.Month;
				}

				if (n3.HasValue)
				{
					y = n3.Value;
				}
				else
				{
					y = defaultDate.Year;
				}
			}

			if (y < 1000)
			{
				y += 2000;
			}

			if (d < 1 || d > 31 || m < 1 || m > 12)
			{
				date = null;
				return true;
			}

			try
			{
				date = new System.DateTime (y, m, d);
			}
			catch
			{
				date = null;
				return true;
			}

			bool ok = true;

			if (minDate.HasValue && date < minDate.Value)
			{
				date = minDate.Value;
				ok = false;
			}

			if (maxDate.HasValue && date > maxDate.Value)
			{
				date = maxDate.Value;
				ok = false;
			}

			return ok;
		}

		public static string DateToString(System.DateTime? date)
		{
			if (date.HasValue)
			{
				string d, m, y;

				d = date.Value.Day.ToString ("00");
				m = date.Value.Month.ToString ("00");

				if (Converters.dateFormatYear == SettingsEnum.YearDigits2)
				{
					y = (date.Value.Year % 100).ToString ("00");
				}
				else
				{
					y = date.Value.Year.ToString ("0000");
				}

				string s = Converters.SettingsEnumToChar (Converters.dateFormatSeparator);

				if (Converters.dateFormatOrder == SettingsEnum.YearYMD)
				{
					return y+s+m+s+d;
				}
				else
				{
					return d+s+m+s+y;
				}
			}
			else
			{
				return null;
			}
		}
		#endregion


		#region Settings
		private static SettingsEnum CharToSettingsEnum(string c)
		{
			switch (c)
			{
				case "":
					return SettingsEnum.SeparatorNone;

				case " ":
					return SettingsEnum.SeparatorSpace;

				case ".":
					return SettingsEnum.SeparatorDot;

				case ",":
					return SettingsEnum.SeparatorComma;

				case "/":
					return SettingsEnum.SeparatorSlash;

				case "-":
					return SettingsEnum.SeparatorDash;

				case "'":
					return SettingsEnum.SeparatorApostrophe;

				default:
					return SettingsEnum.Unknown;

			}
		}

		public static string SettingsEnumToChar(SettingsEnum type)
		{
			switch (type)
			{
				case SettingsEnum.SeparatorNone:
					return "";

				case SettingsEnum.SeparatorSpace:
					return " ";

				case SettingsEnum.SeparatorDot:
					return ".";

				case SettingsEnum.SeparatorComma:
					return ",";

				case SettingsEnum.SeparatorSlash:
					return "/";

				case SettingsEnum.SeparatorDash:
					return "-";

				case SettingsEnum.SeparatorApostrophe:
					return "'";

				default:
					return "?";
			}
		}
		#endregion


		public enum SettingsEnum
		{
			Unknown,

			YearDigits2,
			YearDigits4,

			YearDMY,
			YearYMD,

			SeparatorNone,
			SeparatorSpace,
			SeparatorDot,
			SeparatorComma,
			SeparatorSlash,
			SeparatorDash,
			SeparatorApostrophe,

			NegativeMinus,
			NegativeParentheses,

			NullPartsZeroZero,
			NullPartsDashZero,
			NullPartsZeroDash,
			NullPartsDashDash,

			PercentFloating,
			PercentFrac1,
			PercentFrac2,
			PercentFrac3,
		}


		static Converters()
		{
			//	Constructeur statique.
			Converters.numberFormatMontant = new System.Globalization.CultureInfo ("fr-CH").NumberFormat;

			Converters.numberFormatMontant.CurrencySymbol           = "";
			Converters.numberFormatMontant.CurrencyDecimalSeparator = ".";
			Converters.numberFormatMontant.CurrencyGroupSeparator   = " ";
			Converters.numberFormatMontant.CurrencyGroupSizes       = new int[] { 3 };
			Converters.numberFormatMontant.CurrencyPositivePattern  = 1;  // $n
			Converters.numberFormatMontant.CurrencyNegativePattern  = 1;  // -$n
		}

		private static readonly System.Globalization.NumberFormatInfo numberFormatMontant;

		private static SettingsEnum numberFormatNullParts   = SettingsEnum.NullPartsZeroZero;
		private static SettingsEnum numberFormatNegative    = SettingsEnum.NegativeMinus;
		private static SettingsEnum dateFormatSeparator     = SettingsEnum.SeparatorDot;
		private static SettingsEnum dateFormatYear          = SettingsEnum.YearDigits4;
		private static SettingsEnum dateFormatOrder         = SettingsEnum.YearDMY;
		private static SettingsEnum percentDecimalSeparator = SettingsEnum.SeparatorDot;
		private static SettingsEnum percentFracFormat       = SettingsEnum.PercentFrac1;
	}
}