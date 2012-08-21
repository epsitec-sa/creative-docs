﻿//	Copyright © 2010-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;

using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Library;
//using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Text;

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

			return TextFormatterConverter.ReplaceMinusSign (string.Concat (Misc.decimalRange01.ConvertToString (value.Value*100), "%"));
		}

		public static string PriceToString(decimal? value)
		{
			if (value == null)
			{
				return null;
			}

			return TextFormatterConverter.ReplaceMinusSign (Misc.decimalRange001.ConvertToString (value.Value));
		}

		public static decimal? StringToDecimal(string text)
		{
			if (string.IsNullOrEmpty (text))
			{
				return null;
			}

			if (text[0] == (char) Unicode.Code.MinusSign)
			{
				return -Misc.StringToDecimal (text.Substring (1));
			}

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
				return dateTime.Value.ToLocalTime ().ToString ("dd.MM.yyyy");  // par exemple 06.03.2011
			}
		}

		public static string GetDateTimeDescription(Date? date)
		{
			System.DateTime? dateTime;

			if (date.HasValue)
			{
				dateTime = date.Value.ToDateTime ();
			}
			else
			{
				dateTime = null;
			}

			return Misc.GetDateTimeDescription (dateTime);
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
				return dateTime.Value.ToLocalTime ().ToString ("d MMMM yyyy");  // par exemple 6 mars 2011
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
			return string.Format (@"<img src=""{0}"" voff=""{1}""/>", Misc.IconProvider.GetResourceIconUri (icon), verticalOffset.ToString (System.Globalization.CultureInfo.InvariantCulture));
		}

		/// <summary>
		/// Retourne le tag permettant de mettre une icône sous forme d'image dans un texte html.
		/// </summary>
		/// <param name="icon">Nom brut de l'icône, sans extension.</param>
		/// <param name="verticalOffset">Offset vertical.</param>
		/// <returns></returns>
		public static string GetResourceIconImageTag(string icon, double verticalOffset, Size iconSize)
		{
			return string.Format (@"<img src=""{0}"" voff=""{1}"" dx=""{2}"" dy=""{3}""/>", Misc.IconProvider.GetResourceIconUri (icon), verticalOffset.ToString (System.Globalization.CultureInfo.InvariantCulture), iconSize.Width.ToString (System.Globalization.CultureInfo.InvariantCulture), iconSize.Height.ToString (System.Globalization.CultureInfo.InvariantCulture));
		}

		/// <summary>
		/// Retourne le nom complet d'une icône, à utiliser pour la propriété IconButton.IconUri.
		/// </summary>
		/// <param name="icon">Nom brut de l'icône, sans extension; accepte aussi un nom complet avec procole et extension.</param>
		/// <returns></returns>
		public static string XYZFOOBAR(string icon)
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

		public static string GetResourceIconUri(string icon, Epsitec.Common.Types.Converters.Marshaler marshaler)
		{
			if (marshaler == null)
			{
				return Misc.IconProvider.GetResourceIconUri (icon);
			}
			else
			{
				return Misc.GetResourceIconUri (icon, marshaler.MarshaledType);
			}
		}

		public static string GetResourceIconUri(string icon, System.Type type)
		{
			if (type == null)
			{
				return Misc.IconProvider.GetResourceIconUri (icon);
			}

			var typeName    = type.FullName;
			int entitiesPos = typeName.IndexOf (".Entities.");

			if (entitiesPos < 0)
			{
				return Misc.IconProvider.GetResourceIconUri (icon);
			}

			if (icon.Contains (':'))
			{
				return FormattedText.Escape (icon);
			}
			else
			{
				return string.Format ("manifest:{0}.Images.{1}.icon", typeName.Substring (0, entitiesPos), FormattedText.Escape (icon));
			}
		}


		public static readonly IconProvider		IconProvider = new IconProvider ("Epsitec.Cresus.Core");

		private static readonly decimal maxValue = 1000000000;  // en francs, 1'000'000'000.-, soit 1 milliard

		private static readonly DecimalRange decimalRange01  = new DecimalRange (-Misc.maxValue, Misc.maxValue, 0.1M);
		private static readonly DecimalRange decimalRange001 = new DecimalRange (-Misc.maxValue, Misc.maxValue, 0.01M);
		private static readonly DecimalRange decimalRange005 = new DecimalRange (-Misc.maxValue, Misc.maxValue, 0.05M);
		private static readonly DecimalRange decimalRange100 = new DecimalRange (-Misc.maxValue, Misc.maxValue, 1.00M);
	}
}
