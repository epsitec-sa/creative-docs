//	Copyright � 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
			//	Parse une date situ�e dans n'importe quelle p�riode.
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
			switch (Converters.PreparingForSearh (text))
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
			switch (Converters.PreparingForSearh (text))
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
