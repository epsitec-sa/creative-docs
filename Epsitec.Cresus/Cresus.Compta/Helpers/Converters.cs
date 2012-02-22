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
		#region Montant
		public static decimal? ParseMontant(FormattedText text)
		{
			return Converters.ParseMontant (text.ToSimpleText ());
		}

		public static decimal? ParseMontant(string text)
		{
			//	Parse un montant en francs.
			if (string.IsNullOrEmpty (text))
			{
				return null;
			}

			text = text.Replace ("-", "0");
			text = text.Replace ("�", "0");

			decimal d;
			if (decimal.TryParse (text, out d))
			{
				return d;
			}

			return null;
		}

		public static string MontantToString(decimal? montant)
		{
			if (montant.HasValue)
			{
				string s = montant.Value.ToString ("C", Converters.numberFormatMontant);

				if (Converters.numberFormatNullParts == SettingsEnum.NullPartsDashZero ||
					Converters.numberFormatNullParts == SettingsEnum.NullPartsDashDash)  // commence par un tiret ?
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

				return s;
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

				case TypeDeCompte.Titre:
					return "Titre";

				case TypeDeCompte.Groupe:
					return "Groupe";

				case TypeDeCompte.Bloqu�:
					return "Bloqu�";

				default:
					return "?";
			}
		}

		public static TypeDeCompte StringToType(string text)
		{
			//	Conversion d'une cha�ne en type de compte.
			switch (Strings.PreparingForSearh (text))
			{
				case "titre":
					return TypeDeCompte.Titre;

				case "groupe":
					return TypeDeCompte.Groupe;

				case "bloque":
					return TypeDeCompte.Bloqu�;

				default:
					return TypeDeCompte.Normal;
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
				yield return TypeDeCompte.Titre;
				yield return TypeDeCompte.Groupe;
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
			//	"Plusieurs"
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
				return "Plusieurs";
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

				case ComparisonDisplayMode.Pourcent:
					return "Comparaison en %";

				case ComparisonDisplayMode.PourcentMontant:
					return "Comparaison en % avec montant";

				case ComparisonDisplayMode.Graphique:
					return "Graphique";

				default:
					return "?";
			}
		}
		#endregion


		#region Pr�sentations
		public static FormattedText GetPr�sentationCommandDescription(Command cmd)
		{
			var s = Converters.Pr�sentationCommandToString (cmd);
			switch (s)
			{
				case "Open":
					return "Ouverture";

				case "Save":
					return "Enregistrement";

				case "Print":
					return "Impression";

				case "Login":
					return "Indentification";

				case "Mod�les":
					return "Ecritures mod�les";

				case "Libell�s":
					return "Libell�s usuels";

				case "P�riodes":
					return "P�riodes comptables";

				case "Journaux":
					return "Journaux";

				case "Journal":
					return "Journal des �critures";

				case "PlanComptable":
					return "Plan comptable";

				case "Balance":
					return "Balance de v�rification";

				case "Extrait":
					return "Extrait de compte";

				case "Bilan":
					return "Bilan";

				case "PP":
					return "Pertes et Profits";

				case "Exploitation":
					return "Compte d'exploitation";

				case "Budgets":
					return "Budgets";

				case "Change":
					return "Taux de change";

				case "R�sum�P�riodique":
					return "R�sum� p�riodique";

				case "R�sum�TVA":
					return "R�sum� TVA";

				case "D�compteTVA":
					return "D�compte TVA";

				case "Pi�cesGenerator":
					return "G�n�rateur de pi�ces";

				case "Utilisateurs":
					return "Utilisateurs";

				case "R�glages":
					return "R�glages";

				default:
					return s;
			}
		}

		public static void SetPr�sentationCommand(ref string list, Command cmd, bool state)
		{
			//	Ajoute ou enl�ve une pr�sentation dans une liste.
			var s = Converters.Pr�sentationCommandToString (cmd);

			if (string.IsNullOrEmpty (list))
			{
				if (state)
				{
					list = s;
				}
			}
			else
			{
				var words = list.Split (',').ToList ();
				int i = words.IndexOf (s);

				if (i == -1)
				{
					if (state)
					{
						list += "," + s;
					}
				}
				else
				{
					if (!state)
					{
						words.RemoveAt (i);
						list = string.Join (",", words);
					}
				}
			}
		}

		public static bool ContainsPr�sentationCommand(string list, Command cmd)
		{
			//	Indique si une liste contient une pr�sentation.
			if (string.IsNullOrEmpty (list))
			{
				return false;
			}
			else
			{
				var words = list.Split (',');
				return words.Contains (Converters.Pr�sentationCommandToString (cmd));
			}
		}

		public static int Pr�sentationCommandCount(string list)
		{
			//	Retourne le nombre de pr�sentations contenues dans une liste.
			if (string.IsNullOrEmpty (list))
			{
				return 0;
			}
			else
			{
				var words = list.Split (',');
				return words.Length;
			}
		}

		public static Command StringToPr�sentationCommand(string text)
		{
			//	Nom de commande court "xxx" -> Command
			foreach (var cmd in Converters.Pr�sentationCommands)
			{
				if (Converters.Pr�sentationCommandToString (cmd) == text)
				{
					return cmd;
				}
			}

			return null;
		}

		public static string Pr�sentationCommandToString(Command cmd)
		{
			//	Command -> nom de commande court "xxx"
			string s = "Pr�sentation.";
			int i = cmd.Name.IndexOf (s);

			if (i == -1)
			{
				return null;
			}
			else
			{
				return cmd.Name.Substring (i+s.Length);  // "xxx"
			}
		}

		public static IEnumerable<Command> Pr�sentationCommands
		{
			get
			{
				yield return Res.Commands.Pr�sentation.Open;
				yield return Res.Commands.Pr�sentation.Save;
				yield return Res.Commands.Pr�sentation.Login;
				yield return Res.Commands.Pr�sentation.Print;

				yield return Res.Commands.Pr�sentation.Journal;
				yield return Res.Commands.Pr�sentation.Balance;
				yield return Res.Commands.Pr�sentation.Extrait;
				yield return Res.Commands.Pr�sentation.Bilan;
				yield return Res.Commands.Pr�sentation.PP;
				yield return Res.Commands.Pr�sentation.Exploitation;
				yield return Res.Commands.Pr�sentation.Budgets;
				yield return Res.Commands.Pr�sentation.Change;
				yield return Res.Commands.Pr�sentation.R�sum�P�riodique;
				yield return Res.Commands.Pr�sentation.R�sum�TVA;
				yield return Res.Commands.Pr�sentation.D�compteTVA;

				yield return Res.Commands.Pr�sentation.PlanComptable;
				yield return Res.Commands.Pr�sentation.Libell�s;
				yield return Res.Commands.Pr�sentation.Mod�les;
				yield return Res.Commands.Pr�sentation.Journaux;
				yield return Res.Commands.Pr�sentation.P�riodes;
				yield return Res.Commands.Pr�sentation.Pi�cesGenerator;
				yield return Res.Commands.Pr�sentation.Utilisateurs;
				yield return Res.Commands.Pr�sentation.R�glages;
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
			settingsList.SetEnum (SettingsType.PriceDecimalDigits,    SettingsEnum.DecimalDigits0 + Converters.numberFormatMontant.CurrencyDecimalDigits);
			settingsList.SetEnum (SettingsType.PriceDecimalSeparator, Converters.CharToSettingsEnum (Converters.numberFormatMontant.CurrencyDecimalSeparator));
			settingsList.SetEnum (SettingsType.PriceGroupSeparator,   Converters.CharToSettingsEnum (Converters.numberFormatMontant.CurrencyGroupSeparator));
			settingsList.SetEnum (SettingsType.PriceNullParts,        Converters.numberFormatNullParts);

			if (Converters.numberFormatMontant.CurrencyNegativePattern == 1)
			{
				settingsList.SetEnum (SettingsType.PriceNegativeFormat, SettingsEnum.NegativeMinus);
			}
			if (Converters.numberFormatMontant.CurrencyNegativePattern == 0)
			{
				settingsList.SetEnum (SettingsType.PriceNegativeFormat, SettingsEnum.NegativeParentheses);
			}

			//	Dates.
			settingsList.SetEnum (SettingsType.DateSeparator, Converters.dateFormatSeparator);
			settingsList.SetEnum (SettingsType.DateYear,      Converters.dateFormatYear);
			settingsList.SetEnum (SettingsType.DateOrder,     Converters.dateFormatOrder);
		}

		public static void ImportSettings(SettingsList settingsList)
		{
			//	Settings -> Converters
			SettingsEnum e;

			e = settingsList.GetEnum (SettingsType.PriceDecimalDigits);
			if (e >= SettingsEnum.DecimalDigits0 && e <= SettingsEnum.DecimalDigits5)
			{
				Converters.numberFormatMontant.CurrencyDecimalDigits = e - SettingsEnum.DecimalDigits0;
			}

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

			e = settingsList.GetEnum (SettingsType.PriceNegativeFormat);
			if (e == SettingsEnum.NegativeMinus)
			{
				Converters.numberFormatMontant.CurrencyNegativePattern = 1;  // -$
			}
			if (e == SettingsEnum.NegativeParentheses)
			{
				Converters.numberFormatMontant.CurrencyNegativePattern = 0;  // ($n)
			}

			//	Dates.
			Converters.dateFormatSeparator = settingsList.GetEnum (SettingsType.DateSeparator);
			Converters.dateFormatYear      = settingsList.GetEnum (SettingsType.DateYear);
			Converters.dateFormatOrder     = settingsList.GetEnum (SettingsType.DateOrder);
		}
		#endregion


		static Converters()
		{
			Converters.numberFormatMontant = new System.Globalization.CultureInfo ("fr-CH").NumberFormat;

			Converters.numberFormatMontant.CurrencySymbol           = "";
			Converters.numberFormatMontant.CurrencyDecimalDigits    = 2;
			Converters.numberFormatMontant.CurrencyDecimalSeparator = ".";
			Converters.numberFormatMontant.CurrencyGroupSeparator   = "'";
			Converters.numberFormatMontant.CurrencyGroupSizes       = new int[] { 3 };
			Converters.numberFormatMontant.CurrencyPositivePattern  = 1;  // $n
			Converters.numberFormatMontant.CurrencyNegativePattern  = 1;  // -$n
		}

		private static readonly System.Globalization.NumberFormatInfo numberFormatMontant;

		private static SettingsEnum numberFormatNullParts = SettingsEnum.NullPartsZeroZero;
		private static SettingsEnum dateFormatSeparator   = SettingsEnum.SeparatorDot;
		private static SettingsEnum dateFormatYear        = SettingsEnum.YearDigits4;
		private static SettingsEnum dateFormatOrder       = SettingsEnum.YearDMY;
	}
}
