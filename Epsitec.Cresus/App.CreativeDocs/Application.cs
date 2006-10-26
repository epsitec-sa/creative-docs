//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.App.CreativeDocs
{
	public class Application
	{
		[System.STAThread]
		public static void Main() 
		{
			Epsitec.Common.Support.Resources.SetTwoLetterISOLanguageNameOverride ("en");
			
			Epsitec.App.DocumentEditor.Application.Start ("F");
		}
	}
}
