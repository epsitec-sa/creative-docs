namespace Epsitec.Common.Pictogram.Data
{
	/// <summary>
	/// La classe Misc contient quelques routines générales.
	/// </summary>
	public class Misc
	{
		static public string ExtractName(string filename)
		{
			//	Extrait le nom de fichier, en ignorant les noms de dossiers et l'extension.
			//	"c:\rep\abc.txt" devient "abc".
			int i = filename.LastIndexOf("\\")+1;
			if ( i < 0 )  i = 0;
			int j = filename.IndexOf(".", i);
			if ( j < 0 )  j = filename.Length;
			if ( j <= i )  return "";
			return filename.Substring(i, j-i);
		}

		static public string CopyName(string name)
		{
			//	Retourne la copie d'un nom.
			//	"Bidon"              ->  "Copie de Bidon"
			//	"Copie de Bidon"     ->  "Copie (2) de Bidon"
			//	"Copie (2) de Bidon" ->  "Copie (3) de Bidon"
			if ( name == "" )
			{
				return "Copie";
			}

			if ( name.StartsWith("Copie de ") )
			{
				return "Copie (2) de " + name.Substring(9);
			}

			if ( name.StartsWith("Copie (") )
			{
				int num = 0;
				int i = 7;
				while ( name[i] >= '0' && name[i] <= '9' )
				{
					num *= 10;
					num += name[i++]-'0';
				}
				num ++;
				return "Copie (" + num.ToString() + name.Substring(i);
			}

			return "Copie de " + name;
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
	}
}
