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
		public static void DrawHorizontalGradient(Graphics graphics, Rectangle rect, Color leftColor, Color rightColor)
		{
			//	Peint la surface avec un dégradé horizontal.
			graphics.FillMode = FillMode.NonZero;
			graphics.GradientRenderer.Fill = GradientFill.X;
			graphics.GradientRenderer.SetColors (leftColor, rightColor);
			graphics.GradientRenderer.SetParameters (-100, 100);

			Transform ot = graphics.GradientRenderer.Transform;
			Transform t = Transform.Identity;
			Point center = rect.Center;
			t = t.Scale (rect.Width/100/2, rect.Height/100/2);
			t = t.Translate (center);
			graphics.GradientRenderer.Transform = t;
			graphics.RenderGradient ();
			graphics.GradientRenderer.Transform = ot;
		}

		public static void DrawVerticalGradient(Graphics graphics, Rectangle rect, Color bottomColor, Color topColor)
		{
			//	Peint la surface avec un dégradé vertical.
			graphics.FillMode = FillMode.NonZero;
			graphics.GradientRenderer.Fill = GradientFill.Y;
			graphics.GradientRenderer.SetColors (bottomColor, topColor);
			graphics.GradientRenderer.SetParameters (-100, 100);

			Transform ot = graphics.GradientRenderer.Transform;
			Transform t = Transform.Identity;
			Point center = rect.Center;
			t = t.Scale (rect.Width/100/2, rect.Height/100/2);
			t = t.Translate (center);
			graphics.GradientRenderer.Transform = t;
			graphics.RenderGradient ();
			graphics.GradientRenderer.Transform = ot;
		}

		public static void DrawPathDash(Graphics graphics, Path path, double width, double dash, double gap, bool round, Color color)
		{
			//	Dessine un traitillé simple (dash/gap) le long d'un chemin.
			if (path.IsEmpty)
				return;

			DashedPath dp = new DashedPath ();
			dp.Append (path);

			if (dash == 0.0)  // juste un point ?
			{
				dash = 0.00001;
				gap -= dash;
			}
			dp.AddDash (dash, gap);

			using (Path temp = dp.GenerateDashedPath ())
			{
				graphics.Rasterizer.AddOutline (temp, width, round ? CapStyle.Round : CapStyle.Square, JoinStyle.Round, 5.0);
				graphics.RenderSolid (color);
			}
		}

	
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


		public static double GetEstimatedHeight(string text, double lineHeight = 16)
		{
			//	Retourne la hauteur estimée pour un texte, en fonction du nombre de lignes.
			var lines = text.Split (new string[] { FormattedText.HtmlBreak }, System.StringSplitOptions.None);
			return lines.Length*lineHeight;
		}


		public static FormattedText FontSize(FormattedText text, double size)
		{
			return FormattedText.Concat ("<font size=\"", size.ToString (System.Globalization.CultureInfo.InvariantCulture), "\">", TextFormatter.FormatText (text), "</font>");
		}

		public static FormattedText FontColorize(FormattedText text, Color color)
		{
			return FormattedText.Concat ("<font color=\"#", Color.ToHexa (color), "\">", TextFormatter.FormatText (text), "</font>");
		}

		public static FormattedText Bold(FormattedText text)
		{
			return Misc.Tagged (text, "b");
		}

		public static FormattedText Italic(FormattedText text)
		{
			return Misc.Tagged (text, "i");
		}

		private static FormattedText Tagged(FormattedText text, string tag)
		{
			return FormattedText.Concat ("<", tag, ">", TextFormatter.FormatText (text), "</", tag, ">");
		}


		public static FormattedText FirstLine(FormattedText text)
		{
			string t = TextFormatter.FormatText (text).ToString ();

			if (!string.IsNullOrEmpty (t))
			{
				int i = t.IndexOf (FormattedText.HtmlBreak);

				if (i != -1)
				{
					return t.Substring (0, i);
				}
			}

			return text;
		}

		public static FormattedText AppendLine(FormattedText current, FormattedText text)
		{
			current = TextFormatter.FormatText (current);
			text    = TextFormatter.FormatText (text);

			if (current.IsNullOrEmpty)
			{
				return text;
			}
			else
			{
				return FormattedText.Concat (current, FormattedText.HtmlBreak, text);
			}
		}


		/// <summary>
		/// Retourne les dimensions pour un bouton contenant une icône ou une image.
		/// </summary>
		/// <param name="large"></param>
		/// <returns></returns>
		public static double GetButtonWidth(bool large = true)
		{
			int dx = large ? Misc.buttonLargeWidth : Misc.buttonSmallWidth;
			return 2 * ((dx + 1) / 2 + 5);
		}

		public static readonly int buttonSmallWidth = 14;
		public static readonly int buttonLargeWidth = 31;


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



		private static readonly decimal maxValue = 1000000000;  // en francs, 1'000'000'000.-, soit 1 milliard

		private static readonly DecimalRange decimalRange001 = new DecimalRange (-Misc.maxValue, Misc.maxValue, 0.01M);
		private static readonly DecimalRange decimalRange005 = new DecimalRange (-Misc.maxValue, Misc.maxValue, 0.05M);
		private static readonly DecimalRange decimalRange100 = new DecimalRange (-Misc.maxValue, Misc.maxValue, 1.00M);
	}
}
