//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Dialogs
{
	/// <summary>
	/// La classe Dialog permet d'ouvrir et de gérer un dialogue à partir d'une
	/// ressource et d'une source de données.
	/// </summary>
	public class Dialog
	{
		public Dialog()
		{
		}
		
		public static bool LoadDesigner()
		{
			System.Reflection.Assembly assembly = System.Reflection.Assembly.LoadWithPartialName ("App.Designer");
			
			if (assembly != null)
			{
				System.Type type = assembly.GetType ("Epsitec.Designer.Application");
				
				type.InvokeMember ("StartAsPlugIn", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.InvokeMethod, null, null, null);
				
				return true;
			}
			
			return false;
		}
	}
}
