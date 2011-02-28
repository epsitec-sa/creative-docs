//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.CorePlugIn.WorkflowDesigner
{
	/// <summary>
	/// La classe Misc contient quelques routines générales.
	/// </summary>
	public class Misc
	{
		static public double Truncate(double value, int numberOfDecimal=1)
		{
			//	Retourne un nombre tronqué à un certain nombre de décimales.
			double factor = System.Math.Pow (10, numberOfDecimal);
			return System.Math.Floor (value*factor) / factor;
		}


		static public string Bold(string text)
		{
			//	Retourne le texte en gras.
			return string.Format ("<b>{0}</b>", text);
		}

		static public string Italic(string text)
		{
			//	Retourne le texte en italique.
			return string.Format ("<i>{0}</i>", text);
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
			if (string.IsNullOrEmpty (icon))
			{
				return null;
			}
			else
			{
				return string.Format ("manifest:Epsitec.Cresus.CorePlugIn.WorkflowDesigner.Images.{0}.icon", icon);
			}
		}

		static public string IconDyn(string name, string parameter)
		{
			//	Retourne le nom complet d'une icône dynamique.
			return string.Format("dyn:{0}/{1}", name, parameter);
		}
	}
}
