//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 12/11/2003

namespace Epsitec.Cresus.DataLayer
{
	/// <summary>
	/// La classe DbTools fournit des outils en relation avec la base de
	/// données.
	/// </summary>
	public class DbTools
	{
		private DbTools()
		{
		}
		
		
		public static string BuildCompositeName(params string[] list)
		{
			return string.Join ("_", list);
		}
	}
}
