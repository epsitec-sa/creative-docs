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
			
			while (num > 0 && (list[num-1] == ""))
			{
				num--;
			}
			
			return (num == 0) ? "" : string.Join ("_", list, 0, num);
		}
	}
}
