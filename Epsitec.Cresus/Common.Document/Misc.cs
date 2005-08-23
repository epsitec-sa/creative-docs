using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.Document
{
	/// <summary>
	/// La classe Misc contient quelques routines générales.
	/// </summary>
	public class Misc
	{
		// Ajoute la liste des fontes dans la liste d'un TextFieldCombo.
		static public void AddFontList(TextFieldCombo combo, bool all)
		{
			Font.FaceInfo[] list = Font.Faces;
			foreach ( Font.FaceInfo info in list )
			{
				if ( all || info.IsLatin )
				{
					combo.Items.Add(info.Name);
				}
			}
		}

		// Donne une fonte d'après son nom.
		static public Font GetFont(string fontName)
		{
			Font font = Font.GetFont(fontName, "Regular");

			if ( font == null )
			{
				font = Font.GetFontFallback(fontName);
			}
			
			return font;
		}

		// Retourne le nom d'un caractère Unicode.
		static public string GetUnicodeName(int code)
		{
			if ( code == 0 )  return "";

			string text = TextBreak.GetUnicodeName(code);

			bool minus = false;
			for( int i=0 ; i<text.Length ; i++ )
			{
				if ( text[i] >= 'a' && text[i] <= 'z' )
				{
					minus = true;  // contient une lettre minuscule
					break;
				}
			}

			if ( !minus )  // aucune minuscule dans le texte ?
			{
				// Première lettre en majuscule, le reste en minuscules.
				text = string.Format("{0}{1}", text.Substring(0, 1).ToUpper(), text.Substring(1, text.Length-1).ToLower());
			}

			return text;
		}


		// Retourne le texte en gras.
		static public string Bold(string text)
		{
			return string.Format("<b>{0}</b>", text);
		}

		// Retourne le texte en italique.
		static public string Italic(string text)
		{
			return string.Format("<i>{0}</i>", text);
		}

		// Retourne le texte pour mettre une image dans un texte.
		static public string Image(string icon)
		{
			return string.Format(@"<img src=""{0}""/>", Misc.Icon(icon));
		}

		// Retourne le nom complet d'une icône.
		static public string Icon(string icon)
		{
			return string.Format("manifest:Epsitec.App.DocumentEditor.Images.{0}.icon", icon);
		}

		// Donne le nom complet du fichier.
		// Si le nom n'existe pas, donne "sans titre".
		// Si le fichier doit être sérialisé, donne le nom en gras.
		static public string FullName(string filename, bool dirtySerialize)
		{
			string name = "";
			if ( dirtySerialize )  name += "<b>";

			if ( filename == "" )
			{
				name += Res.Strings.Misc.NoTitle;
			}
			else
			{
				name += filename;
			}

			if ( dirtySerialize )  name += "</b>";
			return name;
		}

		// Extrait le nom de fichier, en ignorant les noms de dossiers et l'extension.
		// Si le nom n'existe pas, donne "sans titre".
		// Si le fichier doit être sérialisé, donne le nom en gras.
		static public string ExtractName(string filename, bool dirtySerialize)
		{
			string name = "";
			if ( dirtySerialize )  name += "<b>";

			if ( filename == "" )
			{
				name += Res.Strings.Misc.NoTitle;
			}
			else
			{
				name += ExtractName(filename);
			}

			if ( dirtySerialize )  name += "</b>";
			return name;
		}

		// Extrait le nom de fichier, en ignorant les noms de dossiers et l'extension.
		// "c:\rep\abc.txt" devient "abc".
		static public string ExtractName(string filename)
		{
			return System.IO.Path.GetFileNameWithoutExtension(filename);
		}

		// Indique si un fichier utilise une extension donnée.
		static public bool IsExtension(string filename, string ext)
		{
			return filename.ToLower().EndsWith(ext);
		}

		// Retourne la copie d'un nom.
		// "Bidon"              ->  "Copie de Bidon"
		// "Copie de Bidon"     ->  "Copie (2) de Bidon"
		// "Copie (2) de Bidon" ->  "Copie (3) de Bidon"
		static public string CopyName(string name)
		{
			return Misc.CopyName(name, Res.Strings.Misc.Copy, Res.Strings.Misc.CopyOf);
		}

		// Retourne la copie d'un nom.
		// copy = "Copie" ou "Copy"
		// of = "de" ou "of"
		static public string CopyName(string name, string copy, string of)
		{
			if ( name == "" )
			{
				return copy;
			}

			if ( name.StartsWith(string.Concat(copy, " ", of, " ")) )
			{
				return string.Concat(copy, " (2) ", of, " ", name.Substring(copy.Length+of.Length+2));
			}

			if ( name.StartsWith(string.Concat(copy, " (")) )
			{
				int num = 0;
				int i = copy.Length+2;
				while ( name[i] >= '0' && name[i] <= '9' )
				{
					num *= 10;
					num += name[i++]-'0';
				}
				num ++;
				return string.Concat(copy, " (", num.ToString(), name.Substring(i));
			}

			return string.Concat(copy, " ", of, " ", name);
		}

		// Permute deux variables.
		static public void Swap(ref bool a, ref bool b)
		{
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
	}
}
