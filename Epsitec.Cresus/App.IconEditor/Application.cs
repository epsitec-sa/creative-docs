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
			this.main_window.PreventAutoClose = true;
			
			this.main_window.ClientSize = new Size (800, 580);
			this.main_window.Text       = "App.Icons";
			this.main_window.Name       = "Application";
			this.main_window.Root.ClientGeometryUpdated += new EventHandler (this.HandleRootClientGeometryUpdated);
			
			this.icon_editor = new IconEditor ();
			this.icon_menu   = this.icon_editor.GetMenu();
			
			this.icon_menu.Parent   = this.main_window.Root;
			this.icon_menu.Dock     = DockStyle.Top;
			
			this.icon_editor.Size   = this.main_window.ClientSize;
			this.icon_editor.Dock   = DockStyle.Fill;
			this.icon_editor.Parent = this.main_window.Root;
			
			this.main_window.CommandDispatcher = this.icon_editor.CommandDispatcher;
			
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
		private HMenu					icon_menu;
		private int						in_geom_update;

		private void HandleRootClientGeometryUpdated(object sender)
		{
			if (this.in_geom_update == 0)
			{
				this.in_geom_update++;
				this.main_window.Root.SetClientZoom (this.main_window.Root.Width / 800);
				this.in_geom_update--;
			}
		}
	}
}
