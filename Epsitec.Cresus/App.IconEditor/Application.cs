using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using Epsitec.Common.Pictogram.Widgets;
 
namespace Epsitec.Icons
{
	/// <summary>
	/// La classe Application démarre l'éditeur d'icônes.
	/// </summary>
	public class Application
	{
		public Application()
		{
			Epsitec.Common.Widgets.Adorner.Factory.SetActive ("LookMetal");
			
			this.main_window = new Window ();
			
			main_window.ClientSize = new Size (700, 500);
			main_window.Text       = "Icon Editor";
			
			this.icon_editor = new IconEditor ();
			
			this.icon_editor.Size   = this.main_window.ClientSize;
			this.icon_editor.Dock   = DockStyle.Fill;
			this.icon_editor.Parent = this.main_window.Root;
			
			this.main_window.Show ();
		}
		
		
		#region Application Startup
		[System.STAThread]
		static void Main() 
		{
			Widget.Initialise ();
			Epsitec.Common.Pictogram.Engine.Initialise();
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
		private IconEditor				icon_editor;
	}
}
