using Epsitec.Common.Support;
using Epsitec.Common.Widgets;

namespace Epsitec.Fiche
{
	/// <summary>
	/// La classe Application gère tout le fonctionnement du mini-fiche.
	/// </summary>
	public class Application
	{
		public Application()
		{
			ResourceBundle bundle = Resources.GetBundle ("file:MainWindow");
			ObjectBundler bundler = new ObjectBundler ();
			
			WindowRoot root = bundler.CreateFromBundle (bundle) as WindowRoot;
			
			this.main_window = root.Window;
			this.main_window.PreventAutoClose = true;
			
			this.RegisterCommands ();
		}
		
		private void RegisterCommands()
		{
			CommandDispatcher cd = this.main_window.CommandDispatcher;
			
			cd.RegisterController (this);
		}
		
		#region Application Commands
		[Command ("Quit")] void CommandQuit()
		{
			this.main_window.Quit ();
		}
		#endregion
		
		#region Application Startup
		[System.STAThread]
		static void Main() 
		{
			Widget.Initialise ();
			Application.application = new Application ();
			Application.application.MainWindow.Run ();
		}
		#endregion
		
		
		public Window					MainWindow
		{
			get { return this.main_window; }
		}
		
		
		public static Application		Current
		{
			get { return Application.application; }
		}
		
		
		
		private static Application		application;
		
		private Window					main_window;
	}
}
