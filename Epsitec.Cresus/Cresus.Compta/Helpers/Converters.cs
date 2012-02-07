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
			switch (Converters.PreparingForSearh (text))
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
