using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document
{
	/// <summary>
	/// La classe Misc contient quelques routines générales.
	/// </summary>
	public class Misc
	{
		// Donne le nom complet du fichier.
		// Si le nom n'existe pas, donne "sans titre".
		// Si le fichier doit être sérialisé, donne le nom en gras.
		static public string FullName(string filename, bool dirtySerialize)
		{
			string name = "";
			if ( dirtySerialize )  name += "<b>";

			if ( filename == "" )
			{
				name += "<i>sans titre</i>";
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
				name += "<i>sans titre</i>";
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
			return Misc.CopyName(name, "Copie", "de");
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
