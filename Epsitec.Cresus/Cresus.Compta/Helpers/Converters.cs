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
	}
}
