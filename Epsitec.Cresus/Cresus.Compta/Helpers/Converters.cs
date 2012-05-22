//	Copyright � 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Settings.Data;
using Epsitec.Cresus.Compta.Settings.Controllers;
using Epsitec.Cresus.Compta.Options.Data;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Helpers
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
		public static decimal RoundMontant(decimal montant, ComptaMonnaieEntity monnaie)
		{
			//	Retourne un montant arrondi selon une monnaie.
			decimal arrondi = 0.01m;

			if (monnaie != null)
			{
				arrondi = monnaie.Arrondi;
			}

			return System.Math.Floor ((montant / arrondi) + 0.5m) * arrondi;
		}

		public static decimal? ParseMontant(FormattedText text)
		{
			return Converters.ParseMontant (text.ToSimpleText ());
		}

		public static decimal? ParseMontant(string text)
		{
			//	Parse un montant, selon les r�glages.
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
				Converters.numberFormatNullParts != SettingsEnum.NullPartsDashDash)  // ne commence pas par un tiret si z�ro ?
			{
				if (text.StartsWith ("-"))
				{
					neg = -1;
					text = text.Substring (1);
				}
			}
			
			text = text.Replace ("-", "0");
			text = text.Replace ("�", "0");

			decimal d;
			if (decimal.TryParse (text, out d))
			{
				return d*neg;
			}

			return null;
		}

		public static string MontantToString(decimal? montant, ComptaMonnaieEntity monnaie)
		{
			//	Conversion d'un montant, selon les r�glages.
			int decimalDigits = (monnaie == null) ? 2 : monnaie.D�cimales;
			return Converters.MontantToString (montant, decimalDigits);
		}

		public static string MontantToString(decimal? montant, int decimalDigits)
		{
			//	Conversion d'un montant, selon les r�glages.
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
					Converters.numberFormatNullParts == SettingsEnum.NullPartsDashDash)  // commence par un tiret si z�ro ?
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
						s = s.Substring (0, s.Length-pattern.Length+1) + "�";  // tiret long
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
			//	Parse un nombre r�el.
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
			//	Conversion d'un nombre r�el en cha�ne.
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
		public static Date? ParseDate(FormattedText text)
		{
			return Converters.ParseDate (text.ToSimpleText ());
		}

		public static Date? ParseDate(string text)
		{
			//	Parse une date situ�e dans n'importe quelle p�riode.
			Date? date;
			Converters.ParseDate (text, Date.Today, null, null, out date);
			return date;
		}

		public static bool ParseDate(string text, Date defaultDate, Date? minDate, Date? maxDate, out Date? date)
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

			date = new Date (y, m, d);
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

		public static string DateToString(Date? date)
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


		#region Cat�gorieDeCompte
		public static string Cat�goriesToString(Cat�gorieDeCompte cat�gorie)
		{
			//	Conversion de plusieurs cat�gories en une cha�ne o� elles sont s�par�es par des "|".
			var list = new List<string> ();

			foreach (var c in Converters.Cat�gories)
			{
				if ((cat�gorie & c) != 0)
				{
					list.Add (Converters.Cat�gorieToString (c));
				}
			}

			return string.Join ("|", list);
		}

		public static Cat�gorieDeCompte StringToCat�gories(string text)
		{
			//	Conversion de plusieurs cat�gories s�par�es par des "|".
			var cat�gorie = Cat�gorieDeCompte.Inconnu;

			if (!string.IsNullOrEmpty (text))
			{
				var words = text.Split ('|');

				foreach (var word in words)
				{
					cat�gorie |= Converters.StringToCat�gorie (word);
				}
			}

			return cat�gorie;
		}


		public static string Cat�gorieToString(Cat�gorieDeCompte cat�gorie)
		{
			//	Conversion d'une cat�gorie en cha�ne.
			switch (cat�gorie)
			{
				case Cat�gorieDeCompte.Actif:
					return "Actif";

				case Cat�gorieDeCompte.Passif:
					return "Passif";

				case Cat�gorieDeCompte.Charge:
					return "Charge";

				case Cat�gorieDeCompte.Produit:
					return "Produit";

				case Cat�gorieDeCompte.Exploitation:
					return "Exploitation";

				default:
					return "?";
			}
		}

		public static Cat�gorieDeCompte StringToCat�gorie(string text)
		{
			//	Conversion d'une cha�ne en cat�gorie.
			switch (Strings.PreparingForSearh (text))
			{
				case "actif":
				case "actifs":
					return Cat�gorieDeCompte.Actif;

				case "passif":
				case "passifs":
					return Cat�gorieDeCompte.Passif;

				case "charge":
				case "charges":
					return Cat�gorieDeCompte.Charge;

				case "produit":
				case "produits":
					return Cat�gorieDeCompte.Produit;

				case "exploitation":
				case "exploitations":
					return Cat�gorieDeCompte.Exploitation;

				default:
					return Cat�gorieDeCompte.Inconnu;
			}
		}


		public static IEnumerable<FormattedText> Cat�gorieDescriptions
		{
			//	Retourne une liste de tous les noms de cat�gories possibles.
			get
			{
				foreach (var cat�gorie in Converters.Cat�gories)
				{
					yield return Converters.Cat�gorieToString (cat�gorie);
				}
			}
		}

		private static IEnumerable<Cat�gorieDeCompte> Cat�gories
		{
			get
			{
				yield return Cat�gorieDeCompte.Actif;
				yield return Cat�gorieDeCompte.Passif;
				yield return Cat�gorieDeCompte.Charge;
				yield return Cat�gorieDeCompte.Produit;
				yield return Cat�gorieDeCompte.Exploitation;
			}
		}
		#endregion


		#region TypeDeCompte
		public static string TypeToString(TypeDeCompte type)
		{
			//	Conversion d'un type de compte en cha�ne.
			switch (type)
			{
				case TypeDeCompte.Normal:
					return "Normal";

				case TypeDeCompte.Groupe:
					return "Groupe";

				case TypeDeCompte.TVA:
					return "TVA";

				default:
					return "?";
			}
		}

		public static TypeDeCompte StringToType(string text)
		{
			//	Conversion d'une cha�ne en type de compte.
			switch (Strings.PreparingForSearh (text))
			{
				case "normal":
					return TypeDeCompte.Normal;

				case "groupe":
					return TypeDeCompte.Groupe;

				case "tva":
					return TypeDeCompte.TVA;

				default:
					return TypeDeCompte.Inconnu;
			}
		}


		public static IEnumerable<FormattedText> TypeDescriptions
		{
			//	Retourne une liste de tous les types de comptes possibles.
			get
			{
				foreach (var type in Converters.Types)
				{
					yield return Converters.TypeToString (type);
				}
			}
		}

		private static IEnumerable<TypeDeCompte> Types
		{
			get
			{
				yield return TypeDeCompte.Normal;
				yield return TypeDeCompte.Groupe;
				yield return TypeDeCompte.TVA;
			}
		}
		#endregion


		#region RaccourciMod�le
		public static string RaccourciToString(RaccourciMod�le raccourci)
		{
			//	Conversion d'un raccourci en cha�ne.
			switch (raccourci)
			{
				case RaccourciMod�le.Ctrl0:
					return "Ctrl+0";

				case RaccourciMod�le.Ctrl1:
					return "Ctrl+1";

				case RaccourciMod�le.Ctrl2:
					return "Ctrl+2";

				case RaccourciMod�le.Ctrl3:
					return "Ctrl+3";

				case RaccourciMod�le.Ctrl4:
					return "Ctrl+4";

				case RaccourciMod�le.Ctrl5:
					return "Ctrl+5";

				case RaccourciMod�le.Ctrl6:
					return "Ctrl+6";

				case RaccourciMod�le.Ctrl7:
					return "Ctrl+7";

				case RaccourciMod�le.Ctrl8:
					return "Ctrl+8";

				case RaccourciMod�le.Ctrl9:
					return "Ctrl+9";

				default:
					return "Aucun";
			}
		}

		public static RaccourciMod�le StringToRaccourci(string text)
		{
			//	Conversion d'une cha�ne en raccourci.
			switch (Strings.PreparingForSearh (text))
			{
				case "ctrl+0":
					return RaccourciMod�le.Ctrl0;

				case "ctrl+1":
					return RaccourciMod�le.Ctrl1;

				case "ctrl+2":
					return RaccourciMod�le.Ctrl2;

				case "ctrl+3":
					return RaccourciMod�le.Ctrl3;

				case "ctrl+4":
					return RaccourciMod�le.Ctrl4;

				case "ctrl+5":
					return RaccourciMod�le.Ctrl5;

				case "ctrl+6":
					return RaccourciMod�le.Ctrl6;

				case "ctrl+7":
					return RaccourciMod�le.Ctrl7;

				case "ctrl+8":
					return RaccourciMod�le.Ctrl8;

				case "ctrl+9":
					return RaccourciMod�le.Ctrl9;

				default:
					return RaccourciMod�le.None;
			}
		}


		public static IEnumerable<FormattedText> RaccourciDescriptions
		{
			//	Retourne une liste de tous les raccourcis possibles.
			get
			{
				foreach (var raccourci in Converters.Raccourcis)
				{
					yield return Converters.RaccourciToString (raccourci);
				}
			}
		}

		private static IEnumerable<RaccourciMod�le> Raccourcis
		{
			get
			{
				yield return RaccourciMod�le.None;
				yield return RaccourciMod�le.Ctrl0;
				yield return RaccourciMod�le.Ctrl1;
				yield return RaccourciMod�le.Ctrl2;
				yield return RaccourciMod�le.Ctrl3;
				yield return RaccourciMod�le.Ctrl4;
				yield return RaccourciMod�le.Ctrl5;
				yield return RaccourciMod�le.Ctrl6;
				yield return RaccourciMod�le.Ctrl7;
				yield return RaccourciMod�le.Ctrl8;
				yield return RaccourciMod�le.Ctrl9;
			}
		}
		#endregion


		#region ComparisonShowed
		public static string GetComparisonShowedListDescription(ComparisonShowed mode)
		{
			//	Retourne une description courte de ce genre:
			//	"Aucun"
			//	"Budget au prorata"
			//	"Plusieurs..."
			int n = Converters.ComparisonsShowed.Where (x => (mode & x) != 0).Count ();

			if (n == 0)
			{
				return "Aucun";
			}
			else if (n == 1)
			{
				return Converters.GetComparisonShowedDescription (mode);
			}
			else
			{
				return "Plusieurs...";
			}
		}

		public static string GetComparisonShowedNiceDescription(ComparisonShowed mode)
		{
			//	Retourne une description de ce genre:
			//	"Budget"
			//	"Budget au prorata et Budget futur"
			//	"P�riode pr�c�dente, Budget au prorata et Budget futur"
			var list = new List<string> ();

			foreach (var m in Converters.ComparisonsShowed)
			{
				if ((mode & m) != 0)
				{
					list.Add (Converters.GetComparisonShowedDescription (m));
				}
			}

			return Strings.SentenceConcat (list);
		}

		public static string GetComparisonShowedDescription(ComparisonShowed mode)
		{
			switch (mode)
			{
				case ComparisonShowed.P�riodeP�nulti�me:
					return "P�riode p�nulti�me";

				case ComparisonShowed.P�riodePr�c�dente:
					return "P�riode pr�c�dente";

				case ComparisonShowed.Budget:
					return "Budget";

				case ComparisonShowed.BudgetProrata:
					return "Budget au prorata";

				case ComparisonShowed.BudgetFutur:
					return "Budget futur";

				case ComparisonShowed.BudgetFuturProrata:
					return "Budget futur au prorata";

				default:
					return "?";
			}
		}

		public static IEnumerable<ComparisonShowed> ComparisonsShowed
		{
			get
			{
				yield return ComparisonShowed.P�riodeP�nulti�me;
				yield return ComparisonShowed.P�riodePr�c�dente;
				yield return ComparisonShowed.Budget;
				yield return ComparisonShowed.BudgetProrata;
				yield return ComparisonShowed.BudgetFutur;
				yield return ComparisonShowed.BudgetFuturProrata;
			}
		}
		#endregion


		#region ComparisonDisplayMode
		public static string GetComparisonDisplayModeDescription(ComparisonDisplayMode mode)
		{
			switch (mode)
			{
				case ComparisonDisplayMode.Montant:
					return "Montant";

				case ComparisonDisplayMode.Diff�rence:
					return "Diff�rence en francs";

				case ComparisonDisplayMode.Pourcentage:
					return "Comparaison en %";

				case ComparisonDisplayMode.PourcentageMontant:
					return "Comparaison en % avec montant";

				default:
					return "?";
			}
		}
		#endregion


		#region TypeEcriture
		public static string TypeEcritureToString(int type)
		{
			return Converters.TypeEcritureToString ((TypeEcriture) type);
		}

		public static string TypeEcritureToString(TypeEcriture type)
		{
			return type.ToString ();
		}

		public static TypeEcriture StringToTypeEcriture(FormattedText text)
		{
			return Converters.StringToTypeEcriture (text.ToString ());
		}

		public static TypeEcriture StringToTypeEcriture(string text)
		{
			TypeEcriture type;
			if (System.Enum.TryParse<TypeEcriture> (text, out type))
			{
				return type;
			}
			else
			{
				return TypeEcriture.Normal;
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

		public static void ExportSettings(SettingsList settingsList)
		{
			//	Converters -> Settings
			settingsList.SetEnum (SettingsType.PriceDecimalSeparator, Converters.CharToSettingsEnum (Converters.numberFormatMontant.CurrencyDecimalSeparator));
			settingsList.SetEnum (SettingsType.PriceGroupSeparator,   Converters.CharToSettingsEnum (Converters.numberFormatMontant.CurrencyGroupSeparator));
			settingsList.SetEnum (SettingsType.PriceNullParts,        Converters.numberFormatNullParts);
			settingsList.SetEnum (SettingsType.PriceNegativeFormat,   Converters.numberFormatNegative);

			//	Dates.
			settingsList.SetEnum (SettingsType.DateSeparator, Converters.dateFormatSeparator);
			settingsList.SetEnum (SettingsType.DateYear,      Converters.dateFormatYear);
			settingsList.SetEnum (SettingsType.DateOrder,     Converters.dateFormatOrder);
		}

		public static void ImportSettings(SettingsList settingsList)
		{
			//	Settings -> Converters
			SettingsEnum e;

			e = settingsList.GetEnum (SettingsType.PriceDecimalSeparator);
			if (e != SettingsEnum.Unknown)
			{
				Converters.numberFormatMontant.CurrencyDecimalSeparator = Converters.SettingsEnumToChar (e);
			}

			e = settingsList.GetEnum (SettingsType.PriceGroupSeparator);
			if (e != SettingsEnum.Unknown)
			{
				Converters.numberFormatMontant.CurrencyGroupSeparator = Converters.SettingsEnumToChar (e);
			}

			Converters.numberFormatNullParts = settingsList.GetEnum (SettingsType.PriceNullParts);
			Converters.numberFormatNegative  = settingsList.GetEnum (SettingsType.PriceNegativeFormat);

			//	Pourcentages.
			Converters.percentDecimalSeparator = settingsList.GetEnum (SettingsType.PercentDecimalSeparator);
			Converters.percentFracFormat       = settingsList.GetEnum (SettingsType.PercentFracFormat);

			//	Dates.
			Converters.dateFormatSeparator = settingsList.GetEnum (SettingsType.DateSeparator);
			Converters.dateFormatYear      = settingsList.GetEnum (SettingsType.DateYear);
			Converters.dateFormatOrder     = settingsList.GetEnum (SettingsType.DateOrder);
		}
		#endregion


		static Converters()
		{
			//	Constructeur statique.
			Converters.numberFormatMontant = new System.Globalization.CultureInfo ("fr-CH").NumberFormat;

			Converters.numberFormatMontant.CurrencySymbol           = "";
			Converters.numberFormatMontant.CurrencyDecimalSeparator = ".";
			Converters.numberFormatMontant.CurrencyGroupSeparator   = "'";
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
