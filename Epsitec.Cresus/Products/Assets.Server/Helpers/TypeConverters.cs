//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Types;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.Helpers
{
	public static class TypeConverters
	{
		#region Pourcentage
		public static decimal? ParseRate(FormattedText text)
		{
			return TypeConverters.ParseRate (text.ToSimpleText ());
		}

		public static decimal? ParseRate(string text)
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

		public static string RateToString(decimal? rate)
		{
			if (rate.HasValue)
			{
				string s;

				if (TypeConverters.rateFracFormat == SettingsEnum.RateFloating)
				{
					s = (rate.Value*100).ToString (System.Globalization.CultureInfo.InvariantCulture);

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

					switch (TypeConverters.rateFracFormat)
					{
						case SettingsEnum.RateFrac1:
							format = "0.0";
							break;

						case SettingsEnum.RateFrac2:
							format = "0.00";
							break;

						case SettingsEnum.RateFrac3:
							format = "0.000";
							break;
					}

					s = (rate.Value*100).ToString (format);
				}

				if (TypeConverters.rateDecimalSeparator == SettingsEnum.SeparatorComma)
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


		#region ComputedAmount
		public static string ComputedAmountToString(ComputedAmount? ca)
		{
			if (ca.HasValue)
			{
				if (ca.Value.Computed)
				{
					string o = ca.Value.Substract ? "− " : "+ ";  // 2212: signe moins

					string a;
					if (ca.Value.Rate)
					{
						a = TypeConverters.RateToString (ca.Value.ArgumentAmount);
					}
					else
					{
						a = TypeConverters.AmountToString (ca.Value.ArgumentAmount);
					}

					string f = TypeConverters.AmountToString (ca.Value.FinalAmount);

					if (!string.IsNullOrEmpty (a) && ca.Value.ArgumentDefined)
					{
						a = string.Concat ("<b>", a, "</b>");
					}

					if (!string.IsNullOrEmpty (f) && !ca.Value.ArgumentDefined)
					{
						f = string.Concat ("<b>", f, "</b>");
					}

					return string.Concat (o, a, " = ", f);
				}
				else
				{
					var a = TypeConverters.AmountToString (ca.Value.FinalAmount);
					return string.Concat ("<b>", a, "</b>");
				}
			}
			else
			{
				return null;
			}
		}
		#endregion


		#region Amount
		public static decimal? ParseAmount(FormattedText text)
		{
			return TypeConverters.ParseAmount (text.ToSimpleText ());
		}

		public static decimal? ParseAmount(string text)
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

			if (TypeConverters.numberFormatNullParts != SettingsEnum.NullPartsDashZero &&
				TypeConverters.numberFormatNullParts != SettingsEnum.NullPartsDashDash)  // ne commence pas par un tiret si zéro ?
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

		public static string AmountToString(decimal? amount, int decimalDigits = 2)
		{
			//	Conversion d'un montant, selon les réglages.
			if (amount.HasValue)
			{
				TypeConverters.numberFormatAmount.CurrencyDecimalDigits = decimalDigits;

				bool neg = false;
				if (amount < 0)
				{
					neg = true;
					amount = -amount;
				}

				string s = amount.Value.ToString ("C", TypeConverters.numberFormatAmount);

				if (TypeConverters.numberFormatNullParts == SettingsEnum.NullPartsDashZero ||
					TypeConverters.numberFormatNullParts == SettingsEnum.NullPartsDashDash)  // commence par un tiret si zéro ?
				{
					string pattern = "0" + TypeConverters.numberFormatAmount.CurrencyDecimalSeparator;  // "0."
					if (s.StartsWith (pattern))
					{
						s = "-" + s.Substring (1);
					}
				}

				if (TypeConverters.numberFormatNullParts == SettingsEnum.NullPartsZeroDash ||
					TypeConverters.numberFormatNullParts == SettingsEnum.NullPartsDashDash)  // termine par un tiret long ?
				{
					string pattern = TypeConverters.numberFormatAmount.CurrencyDecimalSeparator + new string ('0', TypeConverters.numberFormatAmount.CurrencyDecimalDigits);  // ".00"
					if (s.EndsWith (pattern))
					{
						s = s.Substring (0, s.Length-pattern.Length+1) + "—";  // tiret long
					}
				}

				if (neg)
				{
					if (TypeConverters.numberFormatNegative == SettingsEnum.NegativeParentheses)
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
			return TypeConverters.ParseAmount (text.ToSimpleText ());
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

		public static string DecimalToString(decimal? value, int? fracCount = null)
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
			return TypeConverters.ParseInt (text.ToSimpleText ());
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
			return TypeConverters.ParseDate (text.ToSimpleText ());
		}

		public static System.DateTime? ParseDate(string text)
		{
			//	Parse une date située dans n'importe quelle période.
			System.DateTime? date;
			TypeConverters.ParseDate (text, System.DateTime.Today, null, null, out date);
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
				n1 = TypeConverters.ParseInt (words[0]);
			}

			if (words.Length >= 2)
			{
				n2 = TypeConverters.ParseInt (words[1]);
			}

			if (words.Length >= 3)
			{
				n3 = TypeConverters.ParseInt (words[2]);
			}

			int y, m, d;

			if (TypeConverters.dateFormatOrder == SettingsEnum.YearYMD)
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

				if (TypeConverters.dateFormatYear == SettingsEnum.YearDigits2)
				{
					y = (date.Value.Year % 100).ToString ("00");
				}
				else
				{
					y = date.Value.Year.ToString ("0000");
				}

				string s = TypeConverters.SettingsEnumToChar (TypeConverters.dateFormatSeparator);

				if (TypeConverters.dateFormatOrder == SettingsEnum.YearYMD)
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

			RateFloating,
			RateFrac1,
			RateFrac2,
			RateFrac3,
		}


		static TypeConverters()
		{
			//	Constructeur statique.
			TypeConverters.numberFormatAmount = new System.Globalization.CultureInfo ("fr-CH").NumberFormat;

			TypeConverters.numberFormatAmount.CurrencySymbol           = "";
			TypeConverters.numberFormatAmount.CurrencyDecimalSeparator = ".";
			TypeConverters.numberFormatAmount.CurrencyGroupSeparator   = " ";
			TypeConverters.numberFormatAmount.CurrencyGroupSizes       = new int[] { 3 };
			TypeConverters.numberFormatAmount.CurrencyPositivePattern  = 1;  // $n
			TypeConverters.numberFormatAmount.CurrencyNegativePattern  = 1;  // -$n
		}

		private static readonly System.Globalization.NumberFormatInfo numberFormatAmount;

		private static SettingsEnum numberFormatNullParts = SettingsEnum.NullPartsZeroZero;
		private static SettingsEnum numberFormatNegative  = SettingsEnum.NegativeMinus;
		private static SettingsEnum dateFormatSeparator   = SettingsEnum.SeparatorDot;
		private static SettingsEnum dateFormatYear        = SettingsEnum.YearDigits4;
		private static SettingsEnum dateFormatOrder       = SettingsEnum.YearDMY;
		private static SettingsEnum rateDecimalSeparator  = SettingsEnum.SeparatorDot;
		private static SettingsEnum rateFracFormat        = SettingsEnum.RateFrac1;
	}
}
