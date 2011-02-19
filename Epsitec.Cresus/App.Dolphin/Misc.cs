//	Copyright © 2003-2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

namespace Epsitec.App.Dolphin
{
	/// <summary>
	/// La classe Misc contient quelques routines générales.
	/// </summary>
	public class Misc
	{
		static public double Norm(double value)
		{
			if (value < 0)
			{
				return 0;
			}
			else if (value > 1)
			{
				return 1;
			}
			else
			{
				return value;
			}
		}


		static public string RemoveSpaces(string text)
		{
			//	Supprime tous les espaces dans un texte.
			while (true)
			{
				int start = text.IndexOf(" ");
				if (start == -1)
				{
					break;
				}

				text = text.Remove(start, 1);
			}

			return text;
		}

		static public string RemoveTags(string text)
		{
			//	Supprime tous les tags xml <...> dans un texte.
			while (true)
			{
				int start = text.IndexOf("<");
				if (start == -1)
				{
					break;
				}

				int end = text.IndexOf(">", start);
				if (end == -1)
				{
					break;
				}

				text = text.Remove(start, end-start+1);
			}

			return text;
		}


		static public int ParseHexa(string hexa)
		{
			//	Analyse une chaîne hexadécimale et retourne sa valeur.
			return Misc.ParseHexa(hexa, 0, 0);
		}

		static public int ParseHexa(string hexa, int defaultValue, int errorValue)
		{
			//	Analyse une chaîne hexadécimale et retourne sa valeur.
			if (string.IsNullOrEmpty(hexa))
			{
				return defaultValue;
			}

			int result;
			if (System.Int32.TryParse(hexa, System.Globalization.NumberStyles.AllowHexSpecifier, System.Globalization.CultureInfo.CurrentCulture, out result))
			{
				return result;
			}
			else
			{
				return errorValue;
			}
		}

		static public int ParseBin(string bin)
		{
			//	Analyse une chaîne binaire et retourne sa valeur.
			return Misc.ParseBin(bin, 0, 0);
		}

		static public int ParseBin(string bin, int defaultValue, int errorValue)
		{
			//	Analyse une chaîne binaire et retourne sa valeur.
			if (string.IsNullOrEmpty(bin))
			{
				return defaultValue;
			}

			int result = 0;
			foreach (char c in bin)
			{
				result = result << 1;

				if (c == '0')
				{
				}
				else if (c == '1')
				{
					result |= 1;
				}
				else
				{
					return errorValue;
				}
			}
			return result;
		}


		static public string GetVersion()
		{
			//	Donne le numéro de version.
			string version = typeof(DolphinApplication).Assembly.FullName.Split(',')[1].Split('=')[1];
			if ( version.EndsWith(".0") )
			{
				version = version.Substring(0, version.Length-2);
			}
			return version;
		}

		static public int CompareVersions(string v1, string v2)
		{
			//	Compare deux numéros de version.
			if (v1 == v2)
			{
				return 0;
			}

			string[] sv1 = v1.Split('.');
			string[] sv2 = v2.Split('.');

			int count = System.Math.Min(sv1.Length, sv2.Length);
			for (int i=0; i<count; i++)
			{
				int c = sv1[i].CompareTo(sv2[i]);
				if (c != 0)
				{
					return c;
				}
			}

			return 0;
		}


		static public Path GetHatchPath(Rectangle rect, double distance, Point reference)
		{
			//	Retourne des hachures à 45 degrés remplissant sans déborder un rectangle.
			//	Une hachure passe toujours par le point de référence.
			Path path = new Path();

			//	Déplace le point de référence sur le bord gauche du rectangle.
			reference.Y += rect.Left - reference.X;
			reference.X = rect.Left;
			double d = reference.Y - rect.Bottom;

			double v = System.Math.Ceiling(rect.Width/distance) * distance;
			v -= d % distance;

			for (double y=rect.Bottom-v; y<rect.Top; y+=distance)
			{
				double x1 = rect.Left;
				double y1 = y;
				double x2 = rect.Right;
				double y2 = y+rect.Width;

				if (y1 < rect.Bottom)
				{
					x1 += rect.Bottom-y1;
					y1 = rect.Bottom;
				}

				if (y2 > rect.Top)
				{
					x2 -= y2-rect.Top;
					y2 = rect.Top;
				}

				if (x1 < x2)
				{
					path.MoveTo(x1, y1);
					path.LineTo(x2, y2);
				}
			}

			return path;
		}

		static public void DrawPathDash(Graphics graphics, Path path, double width, double dash, double gap, Color color)
		{
			//	Dessine un traitillé simple (dash/gap) le long d'un chemin.
			if (path.IsEmpty)  return;

			DashedPath dp = new DashedPath();
			dp.Append(path);

			if (dash == 0.0)  // juste un point ?
			{
				dash = 0.00001;
				gap -= dash;
			}
			dp.AddDash(dash, gap);

			using (Path temp = dp.GenerateDashedPath())
			{
				graphics.Rasterizer.AddOutline(temp, width, CapStyle.Square, JoinStyle.Round, 5.0);
				graphics.RenderSolid(color);
			}
		}


		static public string FontSize(string text, int size)
		{
			//	Retourne le texte dans une autre taille.
			return string.Format("<font size=\"{1}%\">{0}</font>", text, size.ToString(System.Globalization.CultureInfo.InvariantCulture));
		}

		static public string Bold(string text)
		{
			//	Retourne le texte en gras.
			return string.Format("<b>{0}</b>", text);
		}

		static public string Italic(string text)
		{
			//	Retourne le texte en italique.
			return string.Format("<i>{0}</i>", text);
		}


		static public string ImageFull(string fullName)
		{
			//	Retourne le texte pour mettre une image dans un texte.
			return string.Format(@"<img src=""{0}""/>", fullName);
		}

		static public string ImageFull(string fullName, double verticalOffset)
		{
			//	Retourne le texte pour mettre une image dans un texte.
			return string.Format(@"<img src=""{0}"" voff=""{1}""/>", fullName, verticalOffset.ToString(System.Globalization.CultureInfo.InvariantCulture));
		}

		static public string Image(string icon)
		{
			//	Retourne le texte pour mettre une image dans un texte.
			return string.Format(@"<img src=""{0}""/>", Misc.Icon(icon));
		}

		static public string Image(string icon, double verticalOffset)
		{
			//	Retourne le texte pour mettre une image dans un texte.
			return string.Format(@"<img src=""{0}"" voff=""{1}""/>", Misc.Icon(icon), verticalOffset.ToString(System.Globalization.CultureInfo.InvariantCulture));
		}

		static public string ImageDyn(string name, string parameter)
		{
			//	Retourne le texte pour mettre une image dynamique dans un texte.
			return string.Format(@"<img src=""{0}""/>", Misc.IconDyn(name, parameter));
		}

		static public string ImageDyn(string name, string parameter, double verticalOffset)
		{
			//	Retourne le texte pour mettre une image dynamique dans un texte.
			return string.Format(@"<img src=""{0}"" voff=""{1}""/>", Misc.IconDyn(name, parameter), verticalOffset.ToString(System.Globalization.CultureInfo.InvariantCulture));
		}

		static public Size IconPreferredSize(string iconSize)
		{
			//	Retourne la taille préférée pour une icône. Si la taille réelle de l'icône n'est
			//	pas exactement identique, ce n'est pas important. Drawing.Canvas cherche au mieux.
			if ( iconSize == "Small" )  return new Size(14, 14);
			if ( iconSize == "Large" )  return new Size(31, 31);
			return new Size(20, 20);
		}

		static public string Icon(string icon)
		{
			//	Retourne le nom complet d'une icône.
			return string.Format("manifest:Epsitec.App.Dolphin.Images.{0}.icon", icon);
		}

		static public string IconDyn(string name, string parameter)
		{
			//	Retourne le nom complet d'une icône dynamique.
			return string.Format("dyn:{0}/{1}", name, parameter);
		}


		static public void Swap(ref bool a, ref bool b)
		{
			//	Permute deux variables.
			bool t = a;
			a = b;
			b = t;
		}

		static public void Swap(ref int a, ref int b)
		{
			int t = a;
			a = b;
			b = t;
		}

		static public void Swap(ref double a, ref double b)
		{
			double t = a;
			a = b;
			b = t;
		}

		static public void Swap(ref Point a, ref Point b)
		{
			Point t = a;
			a = b;
			b = t;
		}

		static public void Swap(ref Size a, ref Size b)
		{
			Size t = a;
			a = b;
			b = t;
		}


		static public readonly int undefined = int.MinValue;
	}
}
