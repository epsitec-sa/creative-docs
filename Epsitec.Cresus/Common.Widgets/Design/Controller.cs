namespace Epsitec.Common.Widgets.Design
{
	using Command = Support.CommandAttribute;
	
	/// <summary>
	/// La classe Controller implémente la logique de l'application de design des
	/// interfaces graphiques.
	/// </summary>
	public class Controller
	{
		public Controller()
		{
			this.dispatcher       = new Support.CommandDispatcher ("design dispatcher");
			this.edit_window_list = new System.Collections.ArrayList ();
			this.edit_selected    = new System.Collections.ArrayList ();
		}
		
		
		public void Initialise()
		{
			if (this.is_initialised)
			{
				return;
			}
			
			//	Crée toute l'infrastructure nécessaire au designer de GUI :
			
			this.CreateCreationWindow ();
			
			this.state_delete_selection = new CommandState ("*.delete selection", this.dispatcher);
			
			//	Enregistre cette classe comme un contrôleur sachant exécuter des commandes
			//	interactives :
			
			this.dispatcher.RegisterController (this);
			
			this.is_initialised = true;
		}
		
		
		public Support.CommandDispatcher		CommandDispatcher
		{
			get { return this.dispatcher; }
		}
		
		public Window							CreationWindow
		{
			get { return this.creation_window; }
		}
		
		
		public CommandState						StateDeleteSelection
		{
			get
			{
				return this.state_delete_selection;
			}
		}
		
		
		public Window CreateWindow(string name)
		{
			Window window = new Window ();
			
			window.Name = name;
			window.CommandDispatcher = this.dispatcher;
			
			return window;
		}
		
		
		protected void CreateCreationWindow()
		{
			//	Crée la fenêtre contenant la palette des widgets qui peuvent être utilisés
			//	pour construire une interface par drag & drop.
			
			this.creation_window = this.CreateWindow ("widget palette");
			this.creation_window.MakeFixedSizeWindow ();
			this.creation_window.MakeSecondaryWindow ();
			
			this.widget_palette = new Panels.WidgetPalette (Panels.PreferredLayout.Block);
			this.edit_tools_bar = new Panels.EditToolsBar ();
			
			double dx = this.widget_palette.Size.Width;
			double dy = this.widget_palette.Size.Height;
			
			dx  = System.Math.Max (dx, this.edit_tools_bar.Size.Width);
			dy += this.edit_tools_bar.Size.Height;
			
			this.creation_window.ClientSize = new Drawing.Size (dx, dy);
			
			Widget root   = this.creation_window.Root;
			Widget widget;
			
			root.IsEditionDisabled = true;
			
			//	Initialisation de la palette des widgets drag-ables :
			
			widget = this.widget_palette.Widget;
			
			widget.Parent = root;
			widget.Dock   = DockStyle.Fill;
			
			this.widget_palette.DragBegin += new EventHandler(this.HandleSourceDragBegin);
			
			//	Initialisation de la barre d'outils pour l'édition :
			
			widget = this.edit_tools_bar.Widget;
			
			widget.Parent = root;
			widget.Dock   = DockStyle.Top;
		}
		
		
		protected void SetWidgetEnable(string name, bool enabled)
		{
			System.Text.RegularExpressions.Regex regex = Support.RegexFactory.FromSimpleJoker (name);
			
			Widget[] widgets = Widget.FindAllCommandWidgets (regex, this.dispatcher);
			
			for (int i = 0; i < widgets.Length; i++)
			{
				widgets[i].SetEnabled (enabled);
			}
		}
		
		
		private void HandleSourceDragBegin(object sender)
		{
			System.Diagnostics.Debug.Assert (this.widget_palette == sender);
			
			foreach (Window window in this.edit_window_list)
			{
				AbstractWidgetEdit editor = window.GetProperty ("editor") as AbstractWidgetEdit;
				
				editor.SelectedWidgets.Clear ();
			}
		}
		
		private void HandleEditorSelected(object sender, object o)
		{
			AbstractWidgetEdit editor = sender as AbstractWidgetEdit;
			
			System.Diagnostics.Debug.Assert (editor != null);
			System.Diagnostics.Debug.Assert (editor.SelectedWidgets.Count > 0);
			System.Diagnostics.Debug.Assert (editor.SelectedWidgets.Contains (o));
			
			this.edit_selected.Add (o);
			
			//	Du coup, on désélectionne tous les autres widgets dans toutes les autres fenêtres.
			
			foreach (Window window in this.edit_window_list)
			{
				AbstractWidgetEdit other = window.GetProperty ("editor") as AbstractWidgetEdit;
				
				if (other != editor)
				{
					other.SelectedWidgets.Clear ();
				}
			}
			
			this.StateDeleteSelection.Enabled = this.edit_selected.Count > 0;
		}
		
		private void HandleEditorDeselecting(object sender, object o)
		{
			AbstractWidgetEdit editor = sender as AbstractWidgetEdit;
			
			System.Diagnostics.Debug.Assert (editor != null);
			System.Diagnostics.Debug.Assert (editor.SelectedWidgets.Count > 0);
			System.Diagnostics.Debug.Assert (editor.SelectedWidgets.Contains (o));
			
			this.edit_selected.Remove (o);
			
			this.StateDeleteSelection.Enabled = this.edit_selected.Count > 0;
		}
		
		
		[Command ("*.new window")] void CommandNewWindow()
		{
			Window          window  = new Window ();
			ScrollablePanel surface = new ScrollablePanel ();
			
			window.ClientSize = new Drawing.Size (400, 300);
			
			surface.Size   = window.ClientSize;
			surface.Dock   = DockStyle.Fill;
			surface.Parent = window.Root;
			
			this.edit_window_list.Add (window);
			
			Design.AbsPosWidgetEdit editor = new AbsPosWidgetEdit ();
			
			editor.Panel = surface.Panel;
			editor.Selected    += new SelectionEventHandler(this.HandleEditorSelected);
			editor.Deselecting += new SelectionEventHandler(this.HandleEditorDeselecting);
			
			window.SetProperty ("editor", editor);
			window.Show ();
		}
		
		[Command ("*.delete selection")] void CommandDeleteSelection()
		{
			foreach (Widget widget in this.edit_selected)
			{
				//	TODO: effacer le widget
				System.Diagnostics.Debug.WriteLine ("Delete " + widget.ToString ());
			}
		}
		
		
		protected Support.CommandDispatcher		dispatcher;
		protected bool							is_initialised;
		
		protected Panels.WidgetPalette			widget_palette;
		protected Panels.EditToolsBar			edit_tools_bar;
		
		protected Window						creation_window;
		protected System.Collections.ArrayList	edit_window_list;
		protected System.Collections.ArrayList	edit_selected;
		
		protected CommandState					state_delete_selection;
	}
}
