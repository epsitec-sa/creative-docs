//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
		public static decimal RoundMontant(decimal montant)
		{
			//	Retourne un montant en francs arrondi selon les réglages.
			return System.Math.Floor ((montant / Converters.roundMontantValue) + 0.5m) * Converters.roundMontantValue;
		}

		public static decimal? ParseMontant(FormattedText text)
		{
			return Converters.ParseMontant (text.ToSimpleText ());
		}

		public static decimal? ParseMontant(string text)
		{
			//	Parse un montant en francs, selon les réglages.
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

		public static string MontantToString(decimal? montant)
		{
			//	Conversion d'un montant en francs en chaîne, selon les réglages.
			if (montant.HasValue)
			{
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
			//	Parse une date située dans n'importe quelle période.
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


		#region CatégorieDeCompte
		public static string CatégoriesToString(CatégorieDeCompte catégorie)
		{
			//	Conversion de plusieurs catégories en une chaîne où elles sont séparées par des "|".
			var list = new List<string> ();

			foreach (var c in Converters.Catégories)
			{
				if ((catégorie & c) != 0)
				{
					list.Add (Converters.CatégorieToString (c));
				}
			}

			return string.Join ("|", list);
		}

		public static CatégorieDeCompte StringToCatégories(string text)
		{
			//	Conversion de plusieurs catégories séparées par des "|".
			var catégorie = CatégorieDeCompte.Inconnu;

			if (!string.IsNullOrEmpty (text))
			{
				var words = text.Split ('|');

				foreach (var word in words)
				{
					catégorie |= Converters.StringToCatégorie (word);
				}
			}

			return catégorie;
		}


		public static string CatégorieToString(CatégorieDeCompte catégorie)
		{
			//	Conversion d'une catégorie en chaîne.
			switch (catégorie)
			{
				case CatégorieDeCompte.Actif:
					return "Actif";

				case CatégorieDeCompte.Passif:
					return "Passif";

				case CatégorieDeCompte.Charge:
					return "Charge";

				case CatégorieDeCompte.Produit:
					return "Produit";

				case CatégorieDeCompte.Exploitation:
					return "Exploitation";

				default:
					return "?";
			}
		}

		public static CatégorieDeCompte StringToCatégorie(string text)
		{
			//	Conversion d'une chaîne en catégorie.
			switch (Strings.PreparingForSearh (text))
			{
				case "actif":
				case "actifs":
					return CatégorieDeCompte.Actif;

				case "passif":
				case "passifs":
					return CatégorieDeCompte.Passif;

				case "charge":
				case "charges":
					return CatégorieDeCompte.Charge;

				case "produit":
				case "produits":
					return CatégorieDeCompte.Produit;

				case "exploitation":
				case "exploitations":
					return CatégorieDeCompte.Exploitation;

				default:
					return CatégorieDeCompte.Inconnu;
			}
		}


		public static IEnumerable<FormattedText> CatégorieDescriptions
		{
			//	Retourne une liste de tous les noms de catégories possibles.
			get
			{
				foreach (var catégorie in Converters.Catégories)
				{
					yield return Converters.CatégorieToString (catégorie);
				}
			}
		}

		private static IEnumerable<CatégorieDeCompte> Catégories
		{
			get
			{
				yield return CatégorieDeCompte.Actif;
				yield return CatégorieDeCompte.Passif;
				yield return CatégorieDeCompte.Charge;
				yield return CatégorieDeCompte.Produit;
				yield return CatégorieDeCompte.Exploitation;
			}
		}
		#endregion


		#region TypeDeCompte
		public static string TypeToString(TypeDeCompte type)
		{
			//	Conversion d'un type de compte en chaîne.
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
			//	Conversion d'une chaîne en type de compte.
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


		#region RaccourciModèle
		public static string RaccourciToString(RaccourciModèle raccourci)
		{
			//	Conversion d'un raccourci en chaîne.
			switch (raccourci)
			{
				case RaccourciModèle.Ctrl0:
					return "Ctrl+0";

				case RaccourciModèle.Ctrl1:
					return "Ctrl+1";

				case RaccourciModèle.Ctrl2:
					return "Ctrl+2";

				case RaccourciModèle.Ctrl3:
					return "Ctrl+3";

				case RaccourciModèle.Ctrl4:
					return "Ctrl+4";

				case RaccourciModèle.Ctrl5:
					return "Ctrl+5";

				case RaccourciModèle.Ctrl6:
					return "Ctrl+6";

				case RaccourciModèle.Ctrl7:
					return "Ctrl+7";

				case RaccourciModèle.Ctrl8:
					return "Ctrl+8";

				case RaccourciModèle.Ctrl9:
					return "Ctrl+9";

				default:
					return "Aucun";
			}
		}

		public static RaccourciModèle StringToRaccourci(string text)
		{
			//	Conversion d'une chaîne en raccourci.
			switch (Strings.PreparingForSearh (text))
			{
				case "ctrl+0":
					return RaccourciModèle.Ctrl0;

				case "ctrl+1":
					return RaccourciModèle.Ctrl1;

				case "ctrl+2":
					return RaccourciModèle.Ctrl2;

				case "ctrl+3":
					return RaccourciModèle.Ctrl3;

				case "ctrl+4":
					return RaccourciModèle.Ctrl4;

				case "ctrl+5":
					return RaccourciModèle.Ctrl5;

				case "ctrl+6":
					return RaccourciModèle.Ctrl6;

				case "ctrl+7":
					return RaccourciModèle.Ctrl7;

				case "ctrl+8":
					return RaccourciModèle.Ctrl8;

				case "ctrl+9":
					return RaccourciModèle.Ctrl9;

				default:
					return RaccourciModèle.None;
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

		private static IEnumerable<RaccourciModèle> Raccourcis
		{
			get
			{
				yield return RaccourciModèle.None;
				yield return RaccourciModèle.Ctrl0;
				yield return RaccourciModèle.Ctrl1;
				yield return RaccourciModèle.Ctrl2;
				yield return RaccourciModèle.Ctrl3;
				yield return RaccourciModèle.Ctrl4;
				yield return RaccourciModèle.Ctrl5;
				yield return RaccourciModèle.Ctrl6;
				yield return RaccourciModèle.Ctrl7;
				yield return RaccourciModèle.Ctrl8;
				yield return RaccourciModèle.Ctrl9;
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
			//	"Période précédente, Budget au prorata et Budget futur"
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
				case ComparisonShowed.PériodePénultième:
					return "Période pénultième";

				case ComparisonShowed.PériodePrécédente:
					return "Période précédente";

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
				yield return ComparisonShowed.PériodePénultième;
				yield return ComparisonShowed.PériodePrécédente;
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

				case ComparisonDisplayMode.Différence:
					return "Différence en francs";

				case ComparisonDisplayMode.Pourcentage:
					return "Comparaison en %";

				case ComparisonDisplayMode.PourcentageMontant:
					return "Comparaison en % avec montant";

				case ComparisonDisplayMode.Graphique:
					return "Graphique";

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

		#region Présentations
		public static FormattedText GetPrésentationCommandDescription(Command cmd)
		{
			var s = Converters.PrésentationCommandToString (cmd);
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

				case "Modèles":
					return "Ecritures modèles";

				case "Libellés":
					return "Libellés usuels";

				case "Périodes":
					return "Périodes comptables";

				case "Journaux":
					return "Journaux";

				case "Journal":
					return "Journal des écritures";

				case "PlanComptable":
					return "Plan comptable";

				case "Balance":
					return "Balance de vérification";

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

				case "RésuméPériodique":
					return "Résumé périodique";

				case "RésuméTVA":
					return "Résumé TVA";

				case "DécompteTVA":
					return "Décompte TVA";

				case "PiècesGenerator":
					return "Générateur de pièces";

				case "Utilisateurs":
					return "Utilisateurs";

				case "Réglages":
					return "Réglages";

				default:
					return s;
			}
		}

		public static void SetPrésentationCommand(ref string list, Command cmd, bool state)
		{
			//	Ajoute ou enlève une présentation dans une liste.
			var s = Converters.PrésentationCommandToString (cmd);

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

		public static bool ContainsPrésentationCommand(string list, Command cmd)
		{
			//	Indique si une liste contient une présentation.
			if (string.IsNullOrEmpty (list))
			{
				return false;
			}
			else
			{
				var words = list.Split (',');
				return words.Contains (Converters.PrésentationCommandToString (cmd));
			}
		}

		public static int PrésentationCommandCount(string list)
		{
			//	Retourne le nombre de présentations contenues dans une liste.
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

		public static Command StringToPrésentationCommand(string text)
		{
			//	Nom de commande court "xxx" -> Command
			foreach (var cmd in Converters.PrésentationCommands)
			{
				if (Converters.PrésentationCommandToString (cmd) == text)
				{
					return cmd;
				}
			}

			return null;
		}

		public static string PrésentationCommandToString(Command cmd)
		{
			//	Command -> nom de commande court "xxx"
			string s = "Présentation.";
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

		public static IEnumerable<Command> PrésentationCommands
		{
			//	Liste de toutes les présentations existantes.
			//	L'ordre est utilisé lors du choix d'un utilisateur, pour afficher les boutons à cocher.
			get
			{
				yield return Res.Commands.Présentation.Open;
				yield return Res.Commands.Présentation.Save;
				yield return Res.Commands.Présentation.Login;
				yield return Res.Commands.Présentation.Print;

				yield return Res.Commands.Présentation.Journal;
				yield return Res.Commands.Présentation.Balance;
				yield return Res.Commands.Présentation.Extrait;
				yield return Res.Commands.Présentation.Bilan;
				yield return Res.Commands.Présentation.PP;
				yield return Res.Commands.Présentation.Exploitation;
				yield return Res.Commands.Présentation.Budgets;
				yield return Res.Commands.Présentation.Change;
				yield return Res.Commands.Présentation.RésuméPériodique;
				yield return Res.Commands.Présentation.RésuméTVA;
				yield return Res.Commands.Présentation.DécompteTVA;

				yield return Res.Commands.Présentation.PlanComptable;
				yield return Res.Commands.Présentation.CodesTVA;
				yield return Res.Commands.Présentation.ListeTVA;
				yield return Res.Commands.Présentation.Libellés;
				yield return Res.Commands.Présentation.Modèles;
				yield return Res.Commands.Présentation.Journaux;
				yield return Res.Commands.Présentation.Périodes;
				yield return Res.Commands.Présentation.PiècesGenerator;
				yield return Res.Commands.Présentation.Utilisateurs;
				yield return Res.Commands.Présentation.Réglages;
			}
		}

		public static IEnumerable<Command> MenuPrésentationCommands
		{
			//	Présentations rarement utilisées, accessible par un menu.
			//	L'ordre est utilisé pour construire le menu.
			get
			{
				yield return Res.Commands.Présentation.Libellés;
				yield return Res.Commands.Présentation.Modèles;
				yield return Res.Commands.Présentation.Journaux;
				yield return Res.Commands.Présentation.Périodes;
				yield return Res.Commands.Présentation.PiècesGenerator;
				yield return Res.Commands.Présentation.CodesTVA;
				yield return Res.Commands.Présentation.Utilisateurs;
				yield return Res.Commands.Présentation.Réglages;
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
			settingsList.SetInt  (SettingsType.PriceDecimalDigits,    Converters.numberFormatMontant.CurrencyDecimalDigits);
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

			int? i = settingsList.GetInt (SettingsType.PriceDecimalDigits);
			if (i.HasValue)
			{
				Converters.numberFormatMontant.CurrencyDecimalDigits = i.Value;
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
			Converters.numberFormatNegative  = settingsList.GetEnum (SettingsType.PriceNegativeFormat);

			Converters.roundMontantValue = settingsList.GetDecimal (SettingsType.EcritureArrondiTVA).GetValueOrDefault (0.01m);

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
			Converters.numberFormatMontant.CurrencyDecimalDigits    = 2;
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
		private static decimal      roundMontantValue       = 0.01m;
		private static SettingsEnum percentDecimalSeparator = SettingsEnum.SeparatorDot;
		private static SettingsEnum percentFracFormat       = SettingsEnum.PercentFrac1;
	}
}
