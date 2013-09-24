//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Drawing;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	public static class ColorExtensions
	{
		public static bool IsBackground(this Color color)
		{
			//	Indique s'il s'agit de la couleur de fond.
			return color == ColorManager.GetBackgroundColor ();
		}

		public static bool IsGray(this Color color)
		{
			//	Indique s'il s'agit d'une nuance de gris.
			return color.R == color.G && color.R == color.B;
		}

		public static Color Complement(this Color color)
		{
			//	Retourne la couleur complémentaire.
			double h,s,v;

			double r = color.R;
			double g = color.G;
			double b = color.B;

			ColorExtensions.ConvertRgbToHsv (r, g, b, out h, out s, out v);

			h += 180.0;
			while (h > 360.0)
			{
				h -= 360.0;
			}

			ColorExtensions.ConvertHsvToRgb (h, s, v, out r, out g, out b);

			return Color.FromAlphaRgb (color.A, r, g, b);
		}

		public static Color Alpha(this Color color, double alpha)
		{
			//	Retourne une couleur avec une transparence forcée.
			return Color.FromAlphaRgb (alpha, color.R, color.G, color.B);
		}

		public static Color Delta(this Color color, double delta)
		{
			//	Retourne une couleur plus claire/foncée en appliquant un delta linéairement
			//	sur les 3 composantes.
			//	Si delta > 0  ->  plus clair
			//	Si delta < 0  ->  plus foncé
			double r = ColorExtensions.ClipColor (color.R + delta);
			double g = ColorExtensions.ClipColor (color.G + delta);
			double b = ColorExtensions.ClipColor (color.B + delta);

			return Color.FromRgb (r, g, b);
		}

		public static Color ForceV(this Color color, double value)
		{
			//	Retourne une couleur plus foncée, en forçant sa luminosité.
			double h,s,v;

			double r = color.R;
			double g = color.G;
			double b = color.B;

			ColorExtensions.ConvertRgbToHsv (r, g, b, out h, out s, out v);

			if (s == 0)  // gris ?
			{
				r = value;
				g = value;
				b = value;
			}
			else
			{
				v = value;
				ColorExtensions.ConvertHsvToRgb (h, s, v, out r, out g, out b);
			}

			return Color.FromAlphaRgb (color.A, r, g, b);
		}

		public static Color ForceSV(this Color color, double saturation, double value)
		{
			//	Retourne une couleur pastelle, en forçant sa saturation et sa luminosité.
			double h,s,v;

			double r = color.R;
			double g = color.G;
			double b = color.B;

			ColorExtensions.ConvertRgbToHsv (r, g, b, out h, out s, out v);

			if (s == 0)  // gris ?
			{
				r = value;
				g = value;
				b = value;
			}
			else
			{
				s = saturation;
				v = value;
				ColorExtensions.ConvertHsvToRgb (h, s, v, out r, out g, out b);
			}

			return Color.FromAlphaRgb (color.A, r, g, b);
		}

		private static double ClipColor(double value)
		{
			value = System.Math.Max (value, 0.0);
			value = System.Math.Min (value, 1.0);

			return value;
		}


		private static void ConvertRgbToHsv(double r, double g, double b, out double h, out double s, out double v)
		{
			//	R = [0..1]
			//	G = [0..1]
			//	B = [0..1]

			//	H = [0..360]
			//	S = [0..1]
			//	V = [0..1]

			double min = System.Math.Min (r, System.Math.Min (g, b));
			v = System.Math.Max (r, System.Math.Max (g, b));
			double delta = v-min;

			if (v == 0)
			{
				s = 0;
			}
			else
			{
				s = delta/v;
			}

			if (s == 0)  // achromatic ?
			{
				h = 0;
			}
			else	// chromatic ?
			{
				if (r == v)  // between yellow and magenta ?
				{
					h = 60*(g-b)/delta;
				}
				else if (g == v)  // between cyan and yellow ?
				{
					h = 120+60*(b-r)/delta;
				}
				else	// between magenta and cyan ?
				{
					h = 240+60*(r-g)/delta;
				}
				if (h < 0)
				{
					h += 360;
				}
			}
		}

		private static void ConvertHsvToRgb(double h, double s, double v, out double r, out double g, out double b)
		{
			r = g = b = v;

			if (s == 0)
			{
				//	Unsaturated color: this is a gray color.
				return;
			}

			while (h < 0)
			{
				h += 360;
			}
			while (h >= 360)
			{
				h -= 360;
			}

			h /= 60;  // 0..5

			double f = h-System.Math.Floor (h);
			double p = v*(1-s);
			double q = v*(1-s*f);
			double t = v*(1-s*(1-f));

			switch ((int) h)
			{
				case 0:
					r=v;
					g=t;
					b=p;
					break;
				case 1:
					r=q;
					g=v;
					b=p;
					break;
				case 2:
					r=p;
					g=v;
					b=t;
					break;
				case 3:
					r=p;
					g=q;
					b=v;
					break;
				case 4:
					r=t;
					g=p;
					b=v;
					break;
				case 5:
					r=v;
					g=p;
					b=q;
					break;
			}
		}
	}
}
