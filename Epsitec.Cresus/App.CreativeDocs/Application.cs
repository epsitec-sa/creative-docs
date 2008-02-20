//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.App.CreativeDocs
{
	public class Application
	{
		[System.STAThread]
		public static void Main() 
		{
			Epsitec.Common.Support.Resources.OverrideDefaultTwoLetterISOLanguageName ("en");
			Epsitec.Common.DocumentEditor.Application.Start ("F");
		}
	}
}
