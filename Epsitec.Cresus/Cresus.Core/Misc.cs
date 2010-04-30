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
		static Misc()
		{
			//	Constructeur statique.
			Misc.RemoveAccentsToLowerInitialize ();
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
				string[] words = text.Split (new string[] { separator }, System.StringSplitOptions.RemoveEmptyEntries);

				foreach (string word in words)
				{
					if (!string.IsNullOrEmpty (word))
					{
						list.Add (word);
					}
				}
			}

			return list;
		}

		/// <summary>
		/// Combine une liste en une seule chaîne avec un séparateur à choix.
		///	C'est un peu l'inverse de Misc.Split().
		/// </summary>
		/// <param name="list">The list.</param>
		/// <param name="separator">The separator.</param>
		/// <returns></returns>
		public static string Combine(List<string> list, string separator)
		{
			if (list == null)
			{
				return null;
			}

			var builder = new System.Text.StringBuilder ();

			for (int i=0; i<list.Count; i++)
			{
				if (i > 0)
				{
					builder.Append (separator);
				}

				builder.Append (list[i]);
			}

			return builder.ToString ();
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
		public static string RemoveLastBreakLine(string text)
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


		#region RemoveAccentsToLower
		public static string RemoveAccentsToLower(string text)
		{
			//	Retourne le texte en minuscules et sans les accents.
			//	Cette méthode est optimisée pour une exécution la plus rapide possible, vu son
			//	emploi très fréquent. Il faut par exemple éviter l'usage de string.ToLower() qui
			//	ralenti beaucoup.
			if (string.IsNullOrEmpty (text))
			{
				return text;
			}

			char[] chars = null;  // table de caractères pas encore générée

			for (int i=0; i<text.Length; i++)
			{
				char c1 = text[i];
				char c2 = (c1 < Misc.removeAccentsToLowerTable.Length) ? Misc.removeAccentsToLowerTable[c1] : c1;

				if (c2 != c1)  // caractère changé ?
				{
					if (chars == null)  // table de caractères pas encore générée ?
					{
						chars = text.ToCharArray ();  // génère la table
					}

					chars[i] = c2;  // modifie le caractère dans la table
				}
			}

			if (chars == null)  // table de caractères pas générée ?
			{
				return text;  // le texte n'a subi aucun changement
			}
			else
			{
				return new string (chars);  // retourne le texte transformé
			}
		}

		private static void RemoveAccentsToLowerInitialize()
		{
			//	Initialise une fois pour toutes la table de conversion.
			//	Cela n'a aucune importance si ce n'est pas très rapide.
			for (int i=0; i<Misc.removeAccentsToLowerTable.Length; i++)
			{
				char c = (char) i;
				c = Misc.RemoveAccents (c);
				c = char.ToLower (c);

				Misc.removeAccentsToLowerTable[i] = c;
			}
		}

		private static char[] removeAccentsToLowerTable = new char[512];

		private static char RemoveAccents(char c)
		{
			//	Retourne le caractère sans accent.
			//	On ne devrait utiliser cette méthode que lors de la construction de la table avec RemoveAccentsInitialize().
			switch (c)
			{
				case 'â':
					return 'a';
				case 'ä':
					return 'a';
				case 'á':
					return 'a';
				case 'à':
					return 'a';
				case 'ê':
					return 'e';
				case 'ë':
					return 'e';
				case 'é':
					return 'e';
				case 'è':
					return 'e';
				case 'î':
					return 'i';
				case 'ï':
					return 'i';
				case 'í':
					return 'i';
				case 'ì':
					return 'i';
				case 'ô':
					return 'o';
				case 'ö':
					return 'o';
				case 'ó':
					return 'o';
				case 'ò':
					return 'o';
				case 'û':
					return 'u';
				case 'ü':
					return 'u';
				case 'ú':
					return 'u';
				case 'ù':
					return 'u';
				case 'ç':
					return 'c';

				case 'Â':
					return 'A';
				case 'Ä':
					return 'A';
				case 'Á':
					return 'A';
				case 'À':
					return 'A';
				case 'Ê':
					return 'E';
				case 'Ë':
					return 'E';
				case 'É':
					return 'E';
				case 'È':
					return 'E';
				case 'Î':
					return 'I';
				case 'Ï':
					return 'I';
				case 'Í':
					return 'I';
				case 'Ì':
					return 'I';
				case 'Ô':
					return 'O';
				case 'Ö':
					return 'O';
				case 'Ó':
					return 'O';
				case 'Ò':
					return 'O';
				case 'Û':
					return 'U';
				case 'Ü':
					return 'U';
				case 'Ú':
					return 'U';
				case 'Ù':
					return 'U';
				case 'Ç':
					return 'C';

				case '°':
					return 'o';

				default:
					return c;
			}
		}
		#endregion
	}
}
