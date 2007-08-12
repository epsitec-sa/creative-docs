//	Copyright � 2003-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

namespace Epsitec.App.Dolphin
{
	/// <summary>
	/// La classe Misc contient quelques routines g�n�rales.
	/// </summary>
	public class Misc
	{
		static public int ParseHexa(string hexa)
		{
			//	Analyse une cha�ne hexad�cimale et retourne sa valeur.
			if (string.IsNullOrEmpty(hexa))
			{
				return 0;
			}

			int result;
			if (System.Int32.TryParse(hexa, System.Globalization.NumberStyles.AllowHexSpecifier, System.Globalization.CultureInfo.CurrentCulture, out result))
			{
				return result;
			}
			else
			{
				return 0;
			}
		}


		static public string GetVersion()
		{
			//	Donne le num�ro de version.
			string version = typeof(DolphinApplication).Assembly.FullName.Split(',')[1].Split('=')[1];
			if ( version.EndsWith(".0") )
			{
				version = version.Substring(0, version.Length-2);
			}
			return version;
		}

		static public int CompareVersions(string v1, string v2)
		{
			//	Compare deux num�ros de version.
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
			//	Retourne des hachures � 45 degr�s remplissant sans d�border un rectangle.
			//	Une hachure passe toujours par le point de r�f�rence.
			Path path = new Path();

			//	D�place le point de r�f�rence sur le bord gauche du rectangle.
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
			//	Dessine un traitill� simple (dash/gap) le long d'un chemin.
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
			//	Retourne la taille pr�f�r�e pour une ic�ne. Si la taille r�elle de l'ic�ne n'est
			//	pas exactement identique, ce n'est pas important. Drawing.Canvas cherche au mieux.
			if ( iconSize == "Small" )  return new Size(14, 14);
			if ( iconSize == "Large" )  return new Size(31, 31);
			return new Size(20, 20);
		}

		static public string Icon(string icon)
		{
			//	Retourne le nom complet d'une ic�ne.
			return string.Format("manifest:Epsitec.App.Dolphin.Images.{0}.icon", icon);
		}

		static public string IconDyn(string name, string parameter)
		{
			//	Retourne le nom complet d'une ic�ne dynamique.
			return string.Format("dyn:{0}/{1}", name, parameter);
		}
	}
}
