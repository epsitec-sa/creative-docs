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
			this.dispatcher       = new Support.CommandDispatcher ("InterfaceDesigner");
			this.edit_window_list = new System.Collections.ArrayList ();
		}
		
		
		public void Initialise()
		{
			if (this.is_initialised)
			{
				return;
			}
			
			//	Crée toute l'infrastructure nécessaire au designer de GUI :
			
			this.CreateCreationWindow ();
			
			this.state_delete_selection = new CommandState ("DeleteActiveSelection", this.dispatcher);
			
			//	Enregistre cette classe comme un contrôleur sachant exécuter des commandes
			//	interactives :
			
			this.dispatcher.RegisterController (this);
			
			//	Définit l'état initial des commandes :
			
			this.StateDeleteActiveSelection.Enabled = false;
			
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
		
		
		public CommandState						StateDeleteActiveSelection
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
		
		
		protected AbstractWidgetEdit FindEditor(Window window)
		{
			return window == null ? null : window.GetProperty ("$editor") as AbstractWidgetEdit;
		}
		
		
		private void HandleSourceDragBegin(object sender)
		{
			System.Diagnostics.Debug.Assert (this.widget_palette == sender);
			
			//	Quand on commence une opération de drag & drop depuis la palette des widgets, on commence
			//	par dé-sélectionner tous les widgets de toutes les fenêtres :
			
			foreach (Window window in this.edit_window_list)
			{
				AbstractWidgetEdit editor = this.FindEditor (window);
				
				editor.SelectedWidgets.Clear ();
			}
		}
		
		private void HandleEditorSelected(object sender, object o)
		{
			AbstractWidgetEdit editor = sender as AbstractWidgetEdit;
			Widget             widget = o as Widget;
			
			System.Diagnostics.Debug.Assert (editor != null);
			System.Diagnostics.Debug.Assert (widget != null);
			System.Diagnostics.Debug.Assert (editor.SelectedWidgets.Count > 0);
			System.Diagnostics.Debug.Assert (editor.SelectedWidgets.Contains (o));
			
			//	Quand un widget est sélectionné dans un éditeur, on vérifie que l'éditeur actif
			//	est bien à jour, puis on met à jour les informations sur la sélection et sur le
			//	widget actif :
			
			this.SetActiveEditor (editor);
			this.UpdateSelectionState ();
			this.UpdateActiveWidget ();
		}
		
		private void HandleEditorDeselected(object sender, object o)
		{
			AbstractWidgetEdit editor = sender as AbstractWidgetEdit;
			Widget             widget = o as Widget;
			
			System.Diagnostics.Debug.Assert (editor != null);
			System.Diagnostics.Debug.Assert (widget != null);
			
			//	Quand un widget est dé-sélectionné dans un éditeur, on met à jour les informations
			//	de sélection ainsi que celles sur le widget actif :
			
			this.UpdateSelectionState ();
			this.UpdateActiveWidget ();
		}
		
		private void HandleEditWindowWindowActivated(object sender)
		{
			Window             window = sender as Window;
			AbstractWidgetEdit editor = this.FindEditor (window);
			
			System.Diagnostics.Debug.Assert (window != null);
			System.Diagnostics.Debug.Assert (editor != null);
			
			//	Quand une fenêtre contenant un éditeur est activée, on prend note de l'éditeur comme
			//	nouvel éditeur actif :
			
			this.SetActiveEditor (editor);
		}
		
		
		protected void UpdateSelectionState()
		{
			//	Détermine s'il y a une sélection :
			
			bool has_selection = false;
			
			if (this.active_editor != null)
			{
				if (this.active_editor.SelectedWidgets.Count > 0)
				{
					has_selection = true;
				}
			}
			
			//	S'il y a une sélection, il faut mettre à jour l'état des commandes qui s'y
			//	rapportent :
			
			this.StateDeleteActiveSelection.Enabled = has_selection;
		}
		
		protected void UpdateActiveWidget ()
		{
			//	Détermine s'il y a un widget actif pour l'édition. Pour cela, il faut qu'un
			//	seul widget soit sélectionné. Si aucun widget n'est sélectionné, mais qu'un
			//	éditeur est actif, alors on utilise la racine de l'éditeur comme widget
			//	actif (cela permet d'éditer les propriétés du conteneur = fenêtre) :
			
			Widget widget = null;
			
			if (this.active_editor != null)
			{
				if (this.active_editor.SelectedWidgets.Count == 1)
				{
					widget = this.active_editor.SelectedWidgets[0];
				}
				else if (this.active_editor.SelectedWidgets.Count == 0)
				{
					widget = this.active_editor.Root;
				}
			}
			
			this.SetActiveWidget (widget);
		}
		
		protected void SetActiveEditor(AbstractWidgetEdit editor)
		{
			//	Change d'éditeur actif. Si on change d'éditeur, on s'assure qu'aucune
			//	sélection n'est encore active dans un autre éditeur :
			
			if (this.active_editor != editor)
			{
				this.active_editor = editor;
				
				foreach (Window window in this.edit_window_list)
				{
					editor = this.FindEditor (window);
					
					if (editor != this.active_editor)
					{
						editor.SelectedWidgets.Clear ();
					}
				}
			}
		}
		
		protected void SetActiveWidget(Widget widget)
		{
			//	Change de widget actif (= en cours d'édition).
			
			if (this.active_widget != widget)
			{
				this.active_widget = widget;
			}
		}
		
		
		
		[Command ("CreateNewWindow")]			void CommandCreateNewWindow()
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
			editor.Root  = window.Root;
			
			editor.Selected   += new SelectionEventHandler (this.HandleEditorSelected);
			editor.Deselected += new SelectionEventHandler (this.HandleEditorDeselected);
			
			window.WindowActivated += new EventHandler (this.HandleEditWindowWindowActivated);
			
			window.SetProperty ("$editor", editor);
			window.Show ();
		}
		
		[Command ("DeleteActiveSelection")]		void CommandDeleteActiveSelection()
		{
			System.Diagnostics.Debug.Assert (this.active_editor != null);
			System.Diagnostics.Debug.Assert (this.active_editor.SelectedWidgets.Count > 0);
			
			Widget[] widgets = new Widget[this.active_editor.SelectedWidgets.Count];
			this.active_editor.SelectedWidgets.CopyTo (widgets, 0);
			
			this.CommandDeselectAll ();
			
			for (int i = 0; i < widgets.Length; i++)
			{
				widgets[i].Dispose ();
			}
		}
		
		[Command ("DeselectAll")]				void CommandDeselectAll()
		{
			System.Diagnostics.Debug.Assert (this.active_editor != null);
			System.Diagnostics.Debug.Assert (this.active_editor.SelectedWidgets.Count > 0);
			
			this.active_editor.SelectedWidgets.Clear ();
		}
		
		
		
		
		protected Support.CommandDispatcher		dispatcher;
		protected bool							is_initialised;
		
		protected Panels.WidgetPalette			widget_palette;
		protected Panels.EditToolsBar			edit_tools_bar;
		
		protected Window						creation_window;
		protected System.Collections.ArrayList	edit_window_list;
		
		protected Window						active_window;
		protected AbstractWidgetEdit			active_editor;
		protected Widget						active_widget;
		
		protected CommandState					state_delete_selection;
	}
}
