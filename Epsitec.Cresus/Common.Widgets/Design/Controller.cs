namespace Epsitec.Common.Widgets.Design
{
	/// <summary>
	/// La classe Controller implémente la logique de l'application de design des
	/// interfaces graphiques.
	/// </summary>
	public class Controller
	{
		public Controller()
		{
			this.command_dispatcher = new Support.CommandDispatcher ("design dispatcher");
		}
		
		public void Initialise()
		{
			if (this.is_initialised)
			{
				return;
			}
			
			//	Crée toute l'infrastructure nécessaire au designer de GUI.
			
			this.CreateWidgetPaletteWindow ();
			
			this.is_initialised = true;
		}
		
		public Support.CommandDispatcher		CommandDispatcher
		{
			get { return this.command_dispatcher; }
		}
		
		public Window							WidgetPaletteWindow
		{
			get { return this.w_widget_palette; }
		}
		
		
		public Window CreateWindow(string name)
		{
			Window window = new Window ();
			
			window.Name = name;
			window.CommandDispatcher = this.command_dispatcher;
			
			return window;
		}
		
		
		protected virtual void CreateWidgetPaletteWindow()
		{
			//	Crée la fenêtre contenant la palette des widgets qui peuvent être utilisés
			//	pour construire une interface par drag & drop.
			
			this.w_widget_palette = this.CreateWindow ("widget palette");
			this.w_widget_palette.MakeFixedSizeWindow ();
			this.w_widget_palette.MakeSecondaryWindow ();
			
			this.p_widget_palette = new Panels.WidgetPalette (Panels.PreferredLayout.Block);
			
			this.w_widget_palette.ClientSize = this.p_widget_palette.Size + new Drawing.Size (20, 20);
			this.w_widget_palette.Root.IsEditionDisabled = true;
			
			this.p_widget_palette.CreateWidgets (this.w_widget_palette.Root, new Drawing.Point (10, 10));
		}
		
		
		protected Support.CommandDispatcher		command_dispatcher;
		protected bool							is_initialised;
		
		protected Panels.WidgetPalette			p_widget_palette;
		
		protected Window						w_widget_palette;
	}
}
