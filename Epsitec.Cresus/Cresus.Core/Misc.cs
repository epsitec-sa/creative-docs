//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core
{
	public static class Misc
	{
		public static decimal? PriceConstrain(decimal? value, decimal resolution=0.01M)
		{
			if (!value.HasValue)
			{
				return null;
			}

			return Misc.PriceConstrain (value.Value, resolution);
		}

		public static decimal PriceConstrain(decimal value, decimal resolution=0.01M)
		{
			if (resolution == 0.01M)
			{
				return Misc.decimalRange001.Constrain (value);
			}
			else if (resolution == 0.05M)
			{
				return Misc.decimalRange005.Constrain (value);
			}
			else if (resolution == 1.00M)
			{
				return Misc.decimalRange100.Constrain (value);
			}
			else
			{
				DecimalRange dr = new DecimalRange (0, 1000000000, resolution);
				return dr.Constrain (value);
			}
		}

		public static string PercentToString(decimal? value)
		{
			if (!value.HasValue)
			{
				return null;
			}

			int i = (int) (value*100);
			return string.Concat (i.ToString (), "%");
		}

		public static string PriceToString(decimal? value)
		{
			if (!value.HasValue)
			{
				return null;
			}

			return Misc.decimalRange001.ConvertToString (value.Value);
		}

		public static decimal? StringToDecimal(string text)
		{
			decimal value;
			if (decimal.TryParse (text, out value))
			{
				return value;
			}
			else
			{
				return null;
			}
		}


		public static string GetDateTimeShortDescription(System.DateTime dateTime)
		{
			if (dateTime == null)
			{
				return null;
			}
			else
			{
				return dateTime.ToString ("dd.MM.yyyy");  // par exemple 31.03.2010
			}
		}

		public static string GetDateTimeDescription(System.DateTime dateTime)
		{
			if (dateTime == null)
			{
				return null;
			}
			else
			{
				return dateTime.ToString ("dd MMMM yyyy");  // par exemple 31 mars 2010
			}
		}


		public static bool ColorsCompare(IEnumerable<Color> colors1, IEnumerable<Color> colors2)
		{
			if (colors1 == null && colors2 == null)
			{
				return true;
			}

			if (colors1 == null || colors2 == null)
			{
				return false;
			}

			int count1 = colors1.Count ();
			int count2 = colors2.Count ();

			if (count1 != count2)
			{
				return false;
			}

			for (int i = 0; i < count1; i++)
			{
				if (colors1.ElementAt (i) != colors2.ElementAt (i))
				{
					return false;
				}
			}

			return true;
		}


		public static bool IsPunctuationMark(char c)
		{
			// Exclu le caractère '/', pour permettre de numéroter une facture "1000 / 45 / bg" (par exemple).
			switch (c)
			{
				case ',':
				case ';':
				case '.':
				case ':':
				case ')':
					return true;

				default:
					return false;
			}
		}


		public static string FormatUnit(decimal quantity, string unit)
		{
			//	1, "pce"		-> "1 pce"
			//	2, "pce"		-> "2 pces"
			//	3, "km"			-> "3 km"
			//	1.5, "Litre"	-> "0.5 litre"

			//	Régle intéressante:
			//	Un euro et soixante centimes : "1,60 euro" ? ou "euros" ?
			//	Non! Le pluriel commence à 2.
			//	Source: http://orthonet.sdv.fr/pages/informations_p11.html

			if (string.IsNullOrEmpty (unit))
			{
				return quantity.ToString ();
			}
			else
			{
				//	Si l'unité a 1 ou 2 caractères, on n'y touche pas ("m", "cm", "m2", "kg", "t", etc.).
				//	TODO: Faire mieux et gérer les pluriels en "x" !
				if (System.Math.Abs (quantity) >= 2 && unit.Length > 2)
				{
					unit = string.Concat (unit.ToLower(), "s");
				}

				return string.Concat (quantity.ToString (), " ", unit);
			}
		}


		public static string FirstLine(string text)
		{
			if (!string.IsNullOrEmpty (text))
			{
				int i = text.IndexOf ("<br/>");

				if (i != -1)
				{
					return text.Substring (0, i);
				}
			}

			return text;
		}

		public static string AppendLine(string current, string text)
		{
			if (string.IsNullOrEmpty (current))
			{
				return text;
			}
			else
			{
				return string.Concat (current, "<br/>", text);
			}
		}


		/// <summary>
		/// Explose une chaîne avec un séparateur à choix en une liste.
		/// </summary>
		/// <param name="text">The text.</param>
		/// <param name="separator">The separator.</param>
		/// <returns></returns>
		public static List<string> Split(string text, string separator)
		{
			List<string> list = new List<string> ();

			if (!string.IsNullOrEmpty (text))
			{
				list.AddRange (text.Split (new string[] { separator }, System.StringSplitOptions.RemoveEmptyEntries));
			}

			return list;
		}

		/// <summary>
		/// Combine une liste en une seule chaîne avec un séparateur à choix.
		/// C'est un peu l'inverse de Misc.Split().
		/// </summary>
		/// <param name="separator">The separator.</param>
		/// <param name="list">The list.</param>
		/// <returns></returns>
		public static string Join(string separator, IEnumerable<string> list)
		{
			if (list == null)
			{
				return null;
			}

			return string.Join (separator, list);
		}

	
		/// <summary>
		/// Encapsule un texte au milieu d'une préface et d'une postface, mais seulement s'il existe.
		/// </summary>
		/// <param name="preface">The preface.</param>
		/// <param name="text">The text.</param>
		/// <param name="postface">The postface.</param>
		/// <returns></returns>
		public static string Encapsulate(string preface, string text, string postface)
		{
			if (string.IsNullOrEmpty (text))
			{
				return text;
			}
			else
			{
				return string.Concat (preface, text, postface);
			}
		}

		/// <summary>
		/// Supprime l'éventuel "<br/>" qui termine un texte.
		/// </summary>
		/// <param name="text">The text.</param>
		/// <returns></returns>
		public static string RemoveLastLineBreak(string text)
		{
			if (!string.IsNullOrEmpty(text) && text.EndsWith ("<br/>"))
			{
				return text.Substring (0, text.Length-5);
			}

			return text;
		}


		/// <summary>
		/// Appond trois chaînes avec des espaces intercalaires.
		/// </summary>
		/// <param name="s1">The s1.</param>
		/// <param name="s2">The s2.</param>
		/// <param name="s3">The s3.</param>
		/// <returns></returns>
		public static string SpacingAppend(string s1, string s2, string s3)
		{
			return Misc.SpacingAppend (s1, Misc.SpacingAppend (s2, s3));
		}

		/// <summary>
		/// Appond deux chaînes avec un espace intercalaire.
		/// </summary>
		/// <param name="s1">The s1.</param>
		/// <param name="s2">The s2.</param>
		/// <returns></returns>
		public static string SpacingAppend(string s1, string s2)
		{
			if (!string.IsNullOrEmpty (s1) && !string.IsNullOrEmpty (s2))
			{
				return string.Concat (s1, " ", s2);
			}

			if (!string.IsNullOrEmpty (s1))
			{
				return s1;
			}

			if (!string.IsNullOrEmpty (s2))
			{
				return s2;
			}

			return "";
		}

	
		/// <summary>
		/// Retourne le tag permettant de mettre une icône sous forme d'image dans un texte html.
		/// </summary>
		/// <param name="icon">Nom brut de l'icône, sans extension.</param>
		/// <returns></returns>
		public static string GetResourceIconImageTag(string icon)
		{
			return string.Format (@"<img src=""{0}""/>", Misc.GetResourceIconUri (icon));
		}

		/// <summary>
		/// Retourne le tag permettant de mettre une icône sous forme d'image dans un texte html.
		/// </summary>
		/// <param name="icon">Nom brut de l'icône, sans extension.</param>
		/// <param name="verticalOffset">Offset vertical.</param>
		/// <returns></returns>
		public static string GetResourceIconImageTag(string icon, double verticalOffset)
		{
			return string.Format (@"<img src=""{0}"" voff=""{1}""/>", Misc.GetResourceIconUri (icon), verticalOffset.ToString (System.Globalization.CultureInfo.InvariantCulture));
		}

		/// <summary>
		/// Retourne le nom complet d'une icône, à utiliser pour la propriété IconButton.IconUri.
		/// </summary>
		/// <param name="icon">Nom brut de l'icône, sans extension.</param>
		/// <returns></returns>
		public static string GetResourceIconUri(string icon)
		{
			return string.Format ("manifest:Epsitec.Cresus.Core.Images.{0}.icon", icon);
		}

		/// <summary>
		/// Retourne le nom complet d'une image contenue dans les ressources.
		/// </summary>
		/// <param name="icon">Nom de l'image, avec extension.</param>
		/// <returns></returns>
		public static string GetResourceImage(string filename)
		{
			return string.Format ("manifest:Epsitec.Cresus.Core.Images.{0}", filename);
		}


		public static string RemoveAccentsToLower(string text)
		{
			return Epsitec.Common.Types.Converters.TextConverter.ConvertToLowerAndStripAccents (text);
		}


		private static DecimalRange decimalRange001 = new DecimalRange (0, 1000000000, 0.01M);
		private static DecimalRange decimalRange005 = new DecimalRange (0, 1000000000, 0.05M);
		private static DecimalRange decimalRange100 = new DecimalRange (0, 1000000000, 1.00M);
	}
}
