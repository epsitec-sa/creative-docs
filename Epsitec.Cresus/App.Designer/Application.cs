using Epsitec.Common.Designer;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;

namespace Epsitec.Designer
{
	/// <summary>
	/// La classe Application démarre le designer.
	/// </summary>
	public class Application : Epsitec.Common.Designer.Application
	{
		public Application()
		{
		}
		
		#region Application Commands
		[Command ("QuitDesigner")] void CommandQuitDesigner()
		{
			this.MainWindow.Quit ();
		}
		#endregion
		
		#region Application Startup
		[System.STAThread] static void Main() 
		{
			Application.application = new Application ();
			Application.application.MainWindow.Show ();
			Application.application.MainWindow.Run ();
		}
		#endregion
		
		static Designer.Application		application;
	}
}
