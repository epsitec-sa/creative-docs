//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 19/11/2003

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// La classe DbTools fournit quelques fonctions utilitaires qui n'ont pas
	/// de rapport direct avec SQL, mais sont utiles aux classes DbXyz.
	/// </summary>
	public class DbTools
	{
		private DbTools()
		{
		}
		
		
		public static string BuildCompositeName(params string[] list)
		{
			int num = list.Length;
			
			//	Retire de la liste toutes les chaînes vides qui pourraient s'être
			//	accumulées à la fin; ça évite que la composition de "a" avec "" ne
			//	donne "a_" en fin de compte (le résultat sera "a", simplement).
			
			while (num > 0 && (list[num-1] == ""))
			{
				num--;
			}
			
			return (num == 0) ? "" : string.Join ("_", list, 0, num);
		}
		
		public static DbElementCat ParseElementCategory(string text)
		{
			if ((text == null) ||
				(text == ""))
			{
				return DbElementCat.Unknown;
			}
			
			return (DbElementCat) System.Enum.Parse (typeof (DbElementCat), text, true);
		}
		
		public static string ElementCategoryToString(DbElementCat cat)
		{
			if (cat == DbElementCat.Unknown)
			{
				return null;
			}
			
			return cat.ToString ();
		}
	}
}
