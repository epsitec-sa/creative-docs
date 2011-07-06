//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;

using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

//using Epsitec.Cresus.Core.Entities;

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
				DecimalRange dr = new DecimalRange (-Misc.maxValue, Misc.maxValue, resolution);
				return dr.Constrain (value);
			}
		}

		public static string PercentToString(decimal? value)
		{
			if (!value.HasValue)
			{
				return null;
			}

#if false
			int i = (int) (value*100);
			return string.Concat (i.ToString (), "%");
#else
			DecimalRange dr = new DecimalRange (-Misc.maxValue, Misc.maxValue, 0.1M);
			return string.Concat (dr.ConvertToString (value.Value*100), "%");
#endif
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


		public static Date? GetDateFromString(string text)
		{
			//	Conversion d'une chaîne de la forme "jj.mm.aaaa" en une date.
			//	Les séparateurs sont très libres, mais l'ordre jour/mois/année doit être respecté.
			//	Si le mois ou l'année sont omis, utilise les valeurs du jour.
			//	Exemples acceptés:
			//		"31.3.11"
			//		"31.12"
			//		"1"
			//		"17/4/2011"
			//		"25 12"
			if (string.IsNullOrWhiteSpace (text))
			{
				return null;
			}

			text = text.Trim ();
			text = text.Replace ("   ", " ");
			text = text.Replace ("  ", " ");
			text = text.Replace (".", " ");
			text = text.Replace (":", " ");
			text = text.Replace (",", " ");
			text = text.Replace (";", " ");
			text = text.Replace ("/", " ");
			text = text.Replace ("-", " ");

			string[] parts = text.Split (' ');

			if (parts.Length == 0 || parts.Length > 3)
			{
				return null;
			}

			int year  = Date.Today.Year;
			int month = Date.Today.Month;
			int day   = Date.Today.Day;

			if (parts.Length >= 1)
			{
				if (!int.TryParse (parts[0], out day))
				{
					return null;
				}
			}

			if (parts.Length >= 2)
			{
				if (!int.TryParse (parts[1], out month))
				{
					return null;
				}
			}

			if (parts.Length >= 3)
			{
				if (!int.TryParse (parts[2], out year))
				{
					return null;
				}
			}

			if (year < 100)
			{
				year += 2000;
			}

			try
			{
				return new Date (year, month, day);
			}
			catch
			{
				return null;
			}
		}

		public static string GetDateShortDescription(Date? from, Date? to)
		{
			if (from == null)
			{
				return null;
			}
			else
			{
				if (to == null)
				{
					return Misc.GetDateShortDescription (from);
				}
				else
				{
					return string.Concat (Misc.GetDateShortDescription (from), " au ", Misc.GetDateShortDescription (to));
				}
			}
		}

		public static string GetDateShortDescription(Date? date)
		{
			if (date == null)
			{
				return null;
			}
			else
			{
				return string.Concat (date.Value.Day.ToString ("D2"), ".", date.Value.Month.ToString ("D2"), ".", date.Value.Year.ToString ("D4"));
			}
		}

		public static string GetDateTimeShortDescription(System.DateTime? dateTime)
		{
			if (dateTime == null)
			{
				return null;
			}
			else
			{
				return dateTime.Value.ToString ("dd.MM.yyyy");  // par exemple 06.03.2011
			}
		}

		public static string GetDateTimeDescription(Date date)
		{
			return Misc.GetDateTimeDescription (date.ToDateTime ());
		}

		public static string GetDateTimeDescription(System.DateTime? dateTime)
		{
			if (dateTime == null)
			{
				return null;
			}
			else
			{
				return dateTime.Value.ToString ("d MMMM yyyy");  // par exemple 6 mars 2011
			}
		}



		public static string FormatUnit(decimal quantity, string unit)
		{
			//	1, "pce"		-> "1 pce"
			//	2, "pce"		-> "2 pces"
			//	3, "km"			-> "3 km"
			//	1.5, "Litre"	-> "1.5 litre"

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


		public static double GetEstimatedHeight(FormattedText text, double lineHeight = 16)
		{
			//	Retourne la hauteur estimée pour un texte, en fonction du nombre de lignes.
			var lines = text.ToString ().Split (new string[] { FormattedText.HtmlBreak }, System.StringSplitOptions.None);
			return lines.Length*lineHeight;
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
		/// <param name="icon">Nom brut de l'icône, sans extension; accepte aussi un nom complet avec procole et extension.</param>
		/// <returns></returns>
		public static string GetResourceIconUri(string icon)
		{
			if (icon.Contains (':'))
			{
				return FormattedText.Escape (icon);
			}
			else
			{
				return string.Format ("manifest:Epsitec.Cresus.Core.Images.{0}.icon", FormattedText.Escape (icon));
			}
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




		private static readonly decimal maxValue = 1000000000;  // en francs, 1'000'000'000.-, soit 1 milliard

		private static readonly DecimalRange decimalRange001 = new DecimalRange (-Misc.maxValue, Misc.maxValue, 0.01M);
		private static readonly DecimalRange decimalRange005 = new DecimalRange (-Misc.maxValue, Misc.maxValue, 0.05M);
		private static readonly DecimalRange decimalRange100 = new DecimalRange (-Misc.maxValue, Misc.maxValue, 1.00M);
	}
}
