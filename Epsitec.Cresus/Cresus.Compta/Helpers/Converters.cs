//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Controllers;

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
				return montant.Value.ToString ("C", Converters.numberFormatMontant);
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
			System.DateTime d;
			if (System.DateTime.TryParse (text, out d))
			{
				return new Date (d);
			}

			return null;
		}

		public static string DateToString(Date? date)
		{
			if (date.HasValue)
			{
				return date.Value.ToString ();
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
			switch (Converters.PreparingForSearh (text))
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

				case TypeDeCompte.Titre:
					return "Titre";

				case TypeDeCompte.Groupe:
					return "Groupe";

				case TypeDeCompte.Bloqué:
					return "Bloqué";

				default:
					return "?";
			}
		}

		public static TypeDeCompte StringToType(string text)
		{
			//	Conversion d'une chaîne en type de compte.
			switch (Converters.PreparingForSearh (text))
			{
				case "titre":
					return TypeDeCompte.Titre;

				case "groupe":
					return TypeDeCompte.Groupe;

				case "bloque":
					return TypeDeCompte.Bloqué;

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
			switch (Converters.PreparingForSearh (text))
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

		public static string GetComparisonShowedDescription(ComparisonShowed mode)
		{
			switch (mode)
			{
				case ComparisonShowed.Budget:
					return "Budget";

				case ComparisonShowed.BudgetProrata:
					return "Budget au prorata";

				case ComparisonShowed.BudgetFutur:
					return "Budget futur";

				case ComparisonShowed.BudgetFuturProrata:
					return "Budget futur au prorata";

				case ComparisonShowed.PériodePrécédente:
					return "Période précédente";

				case ComparisonShowed.PériodePénultième:
					return "Période pénultième";

				default:
					return "?";
			}
		}

		public static IEnumerable<ComparisonShowed> ComparisonsShowed
		{
			get
			{
				yield return ComparisonShowed.Budget;
				yield return ComparisonShowed.BudgetProrata;
				yield return ComparisonShowed.BudgetFutur;
				yield return ComparisonShowed.BudgetFuturProrata;
				yield return ComparisonShowed.PériodePrécédente;
				yield return ComparisonShowed.PériodePénultième;
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


		#region String conversions
		public static string PreparingForSearh(FormattedText text)
		{
			return Converters.PreparingForSearh (text.ToSimpleText ());
		}

		public static string PreparingForSearh(string text)
		{
			if (!string.IsNullOrEmpty (text))
			{
				return Converters.RemoveDiacritics (text).ToLower ();
			}

			return text;
		}

		private static string RemoveDiacritics(string text)
		{
			string norm = text.Normalize (System.Text.NormalizationForm.FormD);
			var builder = new System.Text.StringBuilder ();

			for (int i = 0; i < norm.Length; i++)
			{
				var uc = System.Globalization.CharUnicodeInfo.GetUnicodeCategory (norm[i]);
				if (uc != System.Globalization.UnicodeCategory.NonSpacingMark)
				{
					builder.Append (norm[i]);
				}
			}

			return builder.ToString ().Normalize (System.Text.NormalizationForm.FormC);
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
			//Converters.numberFormatMontant.CurrencyNegativePattern  = 0;  // ($n)
		}

		private static readonly System.Globalization.NumberFormatInfo numberFormatMontant;
	}
}
