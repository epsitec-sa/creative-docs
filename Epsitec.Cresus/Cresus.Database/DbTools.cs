//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 26/11/2003

namespace Epsitec.Cresus.Database
{
	using Converter = Epsitec.Common.Converters.Converter;
	
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
			
			while ((num > 0) && (list[num-1].Length == 0))
			{
				num--;
			}
			
			return (num == 0) ? "" : string.Join ("_", list, 0, num);
		}
		
		public static DbElementCat ParseElementCategory(string text)
		{
			if ((text == null) ||
				(text.Length == 0))
			{
				return DbElementCat.Unknown;
			}
			
			int cat;
			Converter.Convert (text, out cat);
			return (DbElementCat) cat;
		}
		
		public static string ElementCategoryToString(DbElementCat cat)
		{
			if (cat == DbElementCat.Unknown)
			{
				return null;
			}
			
			return Converter.ToString ((int) cat);
		}
	}
}
