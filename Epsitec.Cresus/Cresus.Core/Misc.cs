//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core
{
	public static class Misc
	{
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

	}
}
