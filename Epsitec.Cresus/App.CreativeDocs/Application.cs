namespace Epsitec.App.CreativeDocs
{
	public class Application
	{
		[System.STAThread]
		public static void Main() 
		{
			System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.GetCultureInfo (1033);
			System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.GetCultureInfo (1033);
			
			Epsitec.App.DocumentEditor.Application.Start ("F");
		}
	}
}
