//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Dialogs;

namespace Epsitec.Common.Designer
{
	/// <summary>
	/// La classe InterfaceEditController implémente la logique de l'application de design des
	/// interfaces graphiques.
	/// </summary>
	public class InterfaceEditController : AbstractMainPanelController
	{
		public InterfaceEditController(Application application) : base (application)
		{
		}
		
		
		public Editors.WidgetEditor[]			WidgetEditors
		{
			get
			{
				this.UpdateEditorArray ();
				return this.editors;
			}
		}
		
		
		public Panels.DataSourcePanel			DataSourcePalette
		{
			get
			{
				return this.data_palette;
			}
		}
		
		public Panels.WidgetSourcePanel			WidgetSourcePalette
		{
			get
			{
				return this.widget_palette;
			}
		}
		
		public Panels.WidgetAttributePanel		WidgetAttributePalette
		{
			get
			{
				return this.attribute_palette;
			}
		}
		
		
		public DialogDesigner					ActiveDialogDesigner
		{
			get
			{
				if (this.active_editor != null)
				{
					return this.active_editor.DialogDesigner;
				}
				
				return null;
			}
		}
		
		
		public override void Initialise()
		{
			System.Diagnostics.Debug.Assert (this.is_initialised == false);
			
			//	Crée toute l'infrastructure nécessaire au designer de GUI :
			
			this.CreateMainPanel ();
			this.CreateCreationPanel ();
			this.CreateAttributePanel ();
			
			//	Définit l'état initial des commandes :
			
			this.SetTabIndexSetter (false);
			this.UpdateTabIndexIcons ();
			this.UpdateCommandStates ();
			
			this.main_panel.Size = this.main_panel.MinSize;
			
			this.is_initialised = true;
		}
		
		public override void FillToolBar(AbstractToolBar tool_bar)
		{
			Common.UI.InterfaceType type = Common.UI.InterfaceType.Any;

			if ((this.active_editor != null) &&
				(this.active_editor.DialogDesigner != null))
			{
				type = this.active_editor.DialogDesigner.InterfaceType;
			}

			tool_bar.Items.Add (IconButton.CreateSimple (Command.CreateNewInterface, "manifest:Epsitec.Common.Designer.Images.New.icon"));
			
			switch (type)
			{
				case Common.UI.InterfaceType.Any:
					tool_bar.Items.Add (IconButton.CreateSimple (Command.OpenLoadInterface, "manifest:Epsitec.Common.Designer.Images.Open.icon"));
					tool_bar.Items.Add (IconButton.CreateSimple (Command.SaveActiveInterface, "manifest:Epsitec.Common.Designer.Images.Save.icon"));
					break;
				
				case Common.UI.InterfaceType.DialogWindow:
					tool_bar.Items.Add (IconButton.CreateSimple (Command.SaveActiveInterface, "manifest:Epsitec.Common.Designer.Images.Save.icon"));
					break;
				
				case Common.UI.InterfaceType.Panel:
					break;
			}
			
			this.SyncCommandStates ();
		}
		
		
		internal void NotifyActiveEditionWidgetChanged(Widget widget, bool restart_edition)
		{
			if (widget == null)
			{
				widget = this.active_editor.Root;
			}
			
			this.attribute_palette.NotifyActiveEditionWidgetChanged (widget, restart_edition);
		}
		
		class SavedWindowParams
		{
			public SavedWindowParams(Window window)
			{
				this.is_edition_enabled = window.Root.IsEditionEnabled;
				this.prevent_auto_close = window.PreventAutoClose;
				this.prevent_auto_quit  = window.PreventAutoQuit;
				this.owner              = window.Owner;
			}
			
			
			public static void Restore(Window window)
			{
				SavedWindowParams saved = window.GetProperty (InterfaceEditController.prop_saved_window_params) as SavedWindowParams;
				
				if (saved != null)
				{
					window.Root.IsEditionEnabled = saved.is_edition_enabled;
					window.PreventAutoClose      = saved.prevent_auto_close;
					window.PreventAutoQuit       = saved.prevent_auto_quit;
					window.Owner                 = saved.owner;
					
					window.ClearProperty (InterfaceEditController.prop_saved_window_params);
				}
			}
			
			
			bool				is_edition_enabled;
			bool				prevent_auto_close;
			bool				prevent_auto_quit;
			Window				owner;
		}
		
		internal void CreateEditorForWindow(Window window, string resource_name)
		{
			//	Crée un éditeur pour la fenêtre spécifiée.
			
			window.SetProperty (InterfaceEditController.prop_saved_window_params, new SavedWindowParams (window));
			
			window.Root.IsEditionEnabled = true;
			window.PreventAutoClose      = true;
			window.PreventAutoQuit       = true;
			window.Owner                 = this.application.MainWindow;
			
//-			window.MakeFloatingWindow ();
			
			this.edit_window_list.Add (window);
			this.editors = null;
			
			Editors.WidgetEditor editor   = new Editors.WidgetEditor (this);
			DialogDesigner       designer = DialogDesigner.FromWindow (window);
			
			if (designer == null)
			{
				//	Comme c'est pour une fenêtre que l'on crée un designer, on va choisir
				//	le type d'interface dialogue/fenêtre :
				
				Common.UI.InterfaceType interface_type = Common.UI.InterfaceType.DialogWindow;
				
				designer = new DialogDesigner (this.application, interface_type);
				
				designer.DialogWindow = window;
				designer.DialogData   = null;
				designer.ResourceName = resource_name;
			}
			
			editor.DialogDesigner = designer;
			editor.Root           = window.Root;
			
			editor.Selected   += new SelectionEventHandler (this.HandleEditorSelected);
			editor.Deselected += new SelectionEventHandler (this.HandleEditorDeselected);
			
			editor.DragSelectionBegin += new EventHandler (this.HandleEditorDragSelectionBegin);
			editor.DragSelectionEnd   += new EventHandler (this.HandleEditorDragSelectionEnd);
			
			window.WindowActivated += new Support.EventHandler (this.HandleEditWindowWindowActivated);
			window.Show ();
			
			this.SetActiveEditor (editor);
			this.UpdateActiveWidget ();
		}
		
		
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.DeselectAllWidgets ();
				this.DisposeAllEditors ();
				
				this.attribute_palette.Dispose ();	this.attribute_palette = null;
				this.widget_palette.Dispose ();		this.widget_palette    = null;
				this.data_palette.Dispose ();		this.data_palette      = null;
				
				this.active_editor = null;
				this.active_widget = null;
				
				this.edit_window_list.Clear ();
				this.editors = null;
				
				this.creation_book     = null;
				
				this.attribute_panel   = null;
				this.creation_panel    = null;
			}
			
			base.Dispose (disposing);
		}
		
		protected void DisposeAllEditors()
		{
			foreach (Editors.WidgetEditor editor in this.WidgetEditors)
			{
				Widget root   = editor.Root;
				Window window = root.Window;
				
				editor.Selected   -= new SelectionEventHandler (this.HandleEditorSelected);
				editor.Deselected -= new SelectionEventHandler (this.HandleEditorDeselected);
			
				editor.DragSelectionBegin -= new EventHandler (this.HandleEditorDragSelectionBegin);
				editor.DragSelectionEnd   -= new EventHandler (this.HandleEditorDragSelectionEnd);
			
				window.WindowActivated -= new Support.EventHandler (this.HandleEditWindowWindowActivated);
				
				SavedWindowParams.Restore (window);
				
				editor.Root = null;
				editor.Dispose ();
			}
		}
		
		protected override void CreateMainPanel()
		{
			this.main_panel = new Widget ();
		}
		
		protected void CreateCreationPanel()
		{
			//	Crée le panneau contenant la palette des widgets qui peuvent être utilisés
			//	pour construire une interface par drag & drop.
			
			this.creation_panel = this.CreatePanel ("ToolKitWindow", "Boîte à outils");
			this.creation_book  = new TabBook (this.creation_panel);
			this.widget_palette = new Panels.WidgetSourcePanel ();
			this.data_palette   = new Panels.DataSourcePanel ();
			
			double dx = System.Math.Max (this.widget_palette.Size.Width,  this.data_palette.Size.Width);
			double dy = System.Math.Max (this.widget_palette.Size.Height, this.data_palette.Size.Height);
			
			Drawing.Size book_frame_size = this.creation_book.Client.Size - this.creation_book.InnerBounds.Size;
			
			dx += book_frame_size.Width;
			dy += book_frame_size.Height;
			
			this.creation_panel.Size       = new Drawing.Size (dx, dy);
			this.creation_panel.MinSize    = new Drawing.Size (dx, dy);
			this.creation_book.Dock        = DockStyle.Fill;
			this.creation_book.DockMargins = new Drawing.Margins (4, 4, 4, 4);
			
			TabPage page_1 = new TabPage ();
			TabPage page_2 = new TabPage ();
			
			page_1.TabTitle = "Widgets";
			page_2.TabTitle = "Données";
			
			this.creation_book.Items.Add (page_1);
			this.creation_book.Items.Add (page_2);
			
			this.creation_book.ActivePage = page_1;
			
			//	Initialisation des palettes :
			
			Widget widget_panel = this.widget_palette.Widget;
			Widget data_panel   = this.data_palette.Widget;
			
			widget_panel.SetEmbedder (page_1);
			widget_panel.Dock = DockStyle.Fill;
			
			data_panel.SetEmbedder (page_2);
			data_panel.Dock = DockStyle.Fill;
			
			this.widget_palette.DragBegin += new Support.EventHandler (this.HandleSourceDragBegin);
			this.data_palette.DragBegin   += new Support.EventHandler (this.HandleSourceDragBegin);
			this.widget_palette.DragEnd   += new Support.EventHandler (this.HandleSourceDragEnd);
			this.data_palette.DragEnd     += new Support.EventHandler (this.HandleSourceDragEnd);
			
			this.creation_panel.Parent = this.MainPanel;
		}
		
		protected void CreateAttributePanel()
		{
			//	Crée la fenêtre contenant les attributs.
			
			this.attribute_panel   = this.CreatePanel ("AttributesWindow", "Attributs");
			this.attribute_palette = new Panels.WidgetAttributePanel (this.application);
			this.tool_bar          = new HToolBar ();
			
			double dx = this.attribute_palette.Size.Width;
			double dy = this.attribute_palette.Size.Height + this.tool_bar.DefaultHeight;
			
			this.attribute_panel.Size    = new Drawing.Size (dx, dy);
			this.attribute_panel.MinSize = new Drawing.Size (dx, dy);
			
			Widget root   = this.attribute_panel;
			Widget widget;
			
			//	Initialisation de la palette des attributs :
			
			widget = this.attribute_palette.Widget;
			
			widget.Parent      = root;
			widget.Dock        = DockStyle.Fill;
			widget.DockMargins = new Drawing.Margins (4, 4, 4, 4);
			
			//	Initialisation de la barre d'outils pour l'édition :
			
			this.tool_bar.Items.Add (IconButton.CreateToggle (Command.TabIndexSetter.ToString () + "(this.IsActive)", "manifest:Epsitec.Common.Designer.Images.NumTabIndex.icon"));
			this.tool_bar.Items.Add (IconButton.CreateSimple (Command.TabIndexResetSeq,				                  "manifest:Epsitec.Common.Designer.Images.NumOne.icon"));
			this.tool_bar.Items.Add (IconButton.CreateToggle (Command.TabIndexPicker.ToString () + "(this.IsActive)", "manifest:Epsitec.Common.Designer.Images.NumPicker.icon"));
			this.tool_bar.Items.Add (new IconSeparator ());
			this.tool_bar.Items.Add (IconButton.CreateSimple (Command.DeleteActiveSelection, "manifest:Epsitec.Common.Designer.Images.Delete.icon"));
			this.tool_bar.Items.Add (new IconSeparator ());
			this.tool_bar.Items.Add (IconButton.CreateSimple (Command.ZTopActiveSelection,    "manifest:Epsitec.Common.Designer.Images.ZTop.icon"));
			this.tool_bar.Items.Add (IconButton.CreateSimple (Command.ZBottomActiveSelection, "manifest:Epsitec.Common.Designer.Images.ZBottom.icon"));
			this.tool_bar.Items.Add (IconButton.CreateSimple (Command.ZUpActiveSelection,     "manifest:Epsitec.Common.Designer.Images.ZUp.icon"));
			this.tool_bar.Items.Add (IconButton.CreateSimple (Command.ZDownActiveSelection,   "manifest:Epsitec.Common.Designer.Images.ZDown.icon"));
			
			this.tool_bar.Size   = new Drawing.Size (dx, this.tool_bar.DefaultHeight);
			this.tool_bar.Parent = root;
			this.tool_bar.Dock   = DockStyle.Top;
			
			this.attribute_panel.Parent = this.MainPanel;
		}
		
		
		protected CommandState GetCommandState(Command command)
		{
			return CommandState.Find (command.ToString (), this.dispatcher);
		}
		
		protected Widget CreatePanel(string name, string caption)
		{
			Widget panel = new Widget ();
			
			panel.Dock = DockStyle.Top;
			panel.Name = name;
			panel.Text = caption;
			panel.CommandDispatcher = this.dispatcher;
			
			return panel;
		}
		
		
		protected void UpdateEditorArray()
		{
			if (this.editors == null)
			{
				int n = this.edit_window_list.Count;
				
				Window[]               windows = new Window[n];
				Editors.WidgetEditor[] editors = new Editors.WidgetEditor[n];
				
				this.edit_window_list.CopyTo (windows);
				
				for (int i = 0; i < n; i++)
				{
					editors[i] = Editors.WidgetEditor.FromWindow (windows[i]);
				}
				
				this.editors = editors;
			}
		}
		
		
		private void HandleSourceDragBegin(object sender)
		{
			System.Diagnostics.Debug.Assert ((this.widget_palette == sender) || (this.data_palette == sender));
			
			this.SetTabIndexSetter (false);
			this.SetTabIndexPicker (false);
			
			//	Quand on commence une opération de drag & drop depuis la palette des widgets, on commence
			//	par dé-sélectionner tous les widgets de toutes les fenêtres :
			
			Editors.WidgetEditor[] editors = this.WidgetEditors;
			
			for (int i = 0; i < editors.Length; i++)
			{
				editors[i].SelectedWidgets.Clear ();
			}
		}
		
		private void HandleSourceDragEnd(object sender)
		{
			System.Diagnostics.Debug.Assert ((this.widget_palette == sender) || (this.data_palette == sender));
			
			Panels.IDropSource drop_source = sender as Panels.IDropSource;
			
			//	Le drag & drop vient de se terminer.
			
			Widget widget = drop_source.DroppedWidget;
			
			if (widget != null)
			{
				Editors.WidgetEditor editor = Editors.WidgetEditor.FromWidget (widget);
				
				System.Diagnostics.Debug.Assert (editor != null);
				System.Diagnostics.Debug.Assert (editor.SelectedWidgets.Count == 0);
				
				editor.SelectedWidgets.Add (widget);
				
				this.NotifyActiveEditionWidgetChanged (widget, true);
			}
		}
		
		private void HandleEditorSelected(object sender, object o)
		{
			Editors.WidgetEditor editor = sender as Editors.WidgetEditor;
			Widget			     widget = o as Widget;
			
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
			Editors.WidgetEditor editor = sender as Editors.WidgetEditor;
			Widget			     widget = o as Widget;
			
			System.Diagnostics.Debug.Assert (editor != null);
			System.Diagnostics.Debug.Assert (widget != null);
			
			//	Quand un widget est dé-sélectionné dans un éditeur, on met à jour les informations
			//	de sélection ainsi que celles sur le widget actif :
			
			this.UpdateSelectionState ();
			this.UpdateActiveWidget ();
		}
		
		private void HandleEditorDragSelectionBegin(object sender)
		{
		}
		
		private void HandleEditorDragSelectionEnd(object sender)
		{
			this.UpdateCommandStates ();
		}
		
		private void HandleEditWindowWindowActivated(object sender)
		{
			Window			     window = sender as Window;
			Editors.WidgetEditor editor = Editors.WidgetEditor.FromWindow (window);
			
			System.Diagnostics.Debug.Assert (window != null);
			System.Diagnostics.Debug.Assert (editor != null);
			
			//	Quand une fenêtre contenant un éditeur est activée, on prend note de l'éditeur comme
			//	nouvel éditeur actif :
			
			this.SetActiveEditor (editor);
			this.UpdateActiveWidget ();
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
			
			this.GetCommandState (Command.DeleteActiveSelection).Enabled  = has_selection;
			this.GetCommandState (Command.ZBottomActiveSelection).Enabled = has_selection;
			this.GetCommandState (Command.ZTopActiveSelection).Enabled    = has_selection;
			this.GetCommandState (Command.ZUpActiveSelection).Enabled     = has_selection;
			this.GetCommandState (Command.ZDownActiveSelection).Enabled   = has_selection;
		}
		
		protected void UpdateCommandStates()
		{
			CommandState state = this.GetCommandState (Command.TabIndexSetter);
			
			if (this.active_editor != null)
			{
				this.active_editor.CommandDispatcher = this.CommandDispatcher;
				
				state.Enabled = true;
				
				this.active_editor.SetTabIndexSetterMode (this.is_tool_tab_setter_active);
				this.active_editor.SetTabIndexPickerMode (this.is_tool_tab_picker_active);
				this.active_editor.IsActiveEditor = true;
			}
			else
			{
				state.Enabled = false;
			}
			
			this.UpdateSelectionState ();
			
			bool is_dirty = false;
			
			if (this.active_editor != null)
			{
				is_dirty = this.active_editor.IsDirty;
			}
			
			this.GetCommandState (Command.SaveActiveInterface).Enabled = is_dirty;
		}
		
		protected void SyncCommandStates()
		{
			this.GetCommandState (Command.SaveActiveInterface).Synchronise ();
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
		
		protected void UpdateTabIndexIcons()
		{
			bool enable = this.GetCommandState (Command.TabIndexSetter).ActiveState == WidgetState.ActiveYes;
			
			this.tool_bar.FindChild ("TabIndexResetSeq").SetVisible (enable);
			this.tool_bar.FindChild ("TabIndexPicker").SetVisible (enable);
		}
		
		
		
		protected void SetActiveEditor(Editors.WidgetEditor editor)
		{
			//	Change d'éditeur actif. Si on change d'éditeur, on s'assure qu'aucune
			//	sélection n'est encore active dans un autre éditeur :
			
			if (this.active_editor != editor)
			{
				if (this.active_editor != null)
				{
					this.active_editor.IsActiveEditor = false;
				}
				
				this.active_editor = editor;
				this.UpdateEditorArray ();
				
				for (int i = 0; i < this.editors.Length; i++)
				{
					if (this.editors[i] != this.active_editor)
					{
						this.editors[i].SelectedWidgets.Clear ();
					}
				}
				
				this.UpdateCommandStates ();
				
				//	L'éditeur actif a changé; on doit donc aussi donner l'occasion aux diverses
				//	sous-palettes de se modifier :
				
				this.widget_palette.NotifyActiveEditorChanged (this.active_editor);
				this.data_palette.NotifyActiveEditorChanged (this.active_editor);
				this.attribute_palette.NotifyActiveEditorChanged (this.active_editor);
				
				this.OnActiveEditorChanged ();
			}
		}
		
		protected void SetActiveWidget(Widget widget)
		{
			//	Change de widget actif (= en cours d'édition).
			
			if (this.active_widget != widget)
			{
				this.active_widget = widget;
				this.attribute_palette.ActiveObject = widget;
			}
		}
		
		protected void SetTabIndexSetter(bool enable)
		{
			if ((this.is_tool_tab_setter_active != enable) ||
				(this.is_initialised == false))
			{
				CommandState state = this.GetCommandState (Command.TabIndexSetter);
				
				state.ActiveState = enable ? WidgetState.ActiveYes : WidgetState.ActiveNo;
				
				this.is_tool_tab_setter_active = enable;
				
				this.SetTabIndexSetter (this.WidgetEditors);
				this.SetTabIndexPicker (false);
				this.UpdateTabIndexIcons ();
			}
		}
		
		protected void SetTabIndexPicker(bool enable)
		{
			if ((this.is_tool_tab_picker_active != enable) ||
				(this.is_initialised == false))
			{
				this.GetCommandState (Command.TabIndexPicker).ActiveState = enable ? WidgetState.ActiveYes : WidgetState.ActiveNo;
				
				this.is_tool_tab_picker_active = enable;
				
				this.SetTabIndexPicker (this.WidgetEditors);
				this.UpdateTabIndexIcons ();
			}
		}
				
		protected void SetTabIndexSetter(Editors.WidgetEditor[] editors)
		{
			for (int i = 0; i < editors.Length; i++)
			{
				editors[i].SetTabIndexSetterMode (this.is_tool_tab_setter_active);
			}
		}
		
		protected void SetTabIndexPicker(Editors.WidgetEditor[] editors)
		{
			for (int i = 0; i < editors.Length; i++)
			{
				editors[i].SetTabIndexPickerMode (this.is_tool_tab_picker_active);
			}
		}
		
		protected void ResetTabIndexSeq(Editors.WidgetEditor[] editors)
		{
			for (int i = 0; i < editors.Length; i++)
			{
				editors[i].ResetTabIndexSeq ();
			}
		}
		
		
		protected Widget[] GetSelectedWidgets()
		{
			Widget[] widgets = new Widget[this.active_editor.SelectedWidgets.Count];
			this.active_editor.SelectedWidgets.CopyTo (widgets, 0);
			return widgets;
		}
		
		
		protected void DeselectAllWidgets()
		{
			if (this.active_editor != null)
			{
				this.active_editor.SelectedWidgets.Clear ();
			}
		}
		
		protected void ReselectWidgets(Widget[] widgets)
		{
			for (int i = 0; i < widgets.Length; i++)
			{
				Widget               widget = widgets[i];
				Editors.WidgetEditor editor = Editors.WidgetEditor.FromWidget (widget);
				
				editor.SelectedWidgets.Add (widget);
			}
		}
		
		protected void ChangeZOrder(Widget[] widgets, int delta)
		{
			int   n = widgets.Length;
			int[] z = new int[n];
			
			//	Trie les widgets de manière à toujours obtenir le même comportement, indépendamment
			//	de l'ordre dans lequel ils ont été sélectionnés.
			
			for (int i = 0; i < n; i++)
			{
				z[i] = widgets[i].ZOrder * delta;
			}
			
			System.Array.Sort (z, widgets);
			
			for (int i = 0; i < n; i++)
			{
				int z_min = 0;
				int z_max = widgets[i].Parent == null ? 1 : widgets[i].Parent.Children.Count;
				
				z[i] = System.Math.Max (System.Math.Min (widgets[i].ZOrder + delta, z_max-1), z_min);
			}
			
			for (int i = 0; i < n; i++)
			{
				widgets[i].ZOrder = z[i];
			}
			
			this.active_editor.GripsOverlay.ZOrder = 0;
		}
		
		
		[Command ("CreateNewInterface")]		void CommandCreateNewInterface()
		{
			Window window = new Window ();
			
			window.ClientSize = new Drawing.Size (400, 300);
			
			this.CreateEditorForWindow (window, null);
		}
		
		[Command ("SaveActiveInterface")]		void CommandSaveActiveInterface()
		{
			System.Diagnostics.Debug.Assert (this.active_editor != null);
			System.Diagnostics.Debug.Assert (this.active_editor.Root != null);
			
			Widget[] selected = this.GetSelectedWidgets ();
			Widget   root     = this.active_editor.Root;
			
			this.DeselectAllWidgets ();
			
			this.application.StringEditController.SaveAllBundles ();
			this.active_editor.Save ();
			
			this.ReselectWidgets (selected);
			this.UpdateCommandStates ();
		}
		
		[Command ("OpenLoadInterface")]			void CommandOpenLoadInterface()
		{
			Support.ObjectBundler  bundler = new Support.ObjectBundler ();
			Support.ResourceBundle bundle  = Support.Resources.GetBundle ("file:test");
			
			bundler.EnableMapping ();
			
			Widget root   = bundler.CreateFromBundle (bundle) as Widget;
			Window window = root.Window;
			
			this.CreateEditorForWindow (window, bundle.PrefixedName);
		}
		
		
		[Command ("DeleteActiveSelection")]		void CommandDeleteActiveSelection()
		{
			System.Diagnostics.Debug.Assert (this.active_editor != null);
			System.Diagnostics.Debug.Assert (this.active_editor.SelectedWidgets.Count > 0);
			
			Widget[] widgets = this.GetSelectedWidgets ();
			this.DeselectAllWidgets ();
			
			foreach (Widget widget in widgets)
			{
				widget.Dispose ();
			}
		}
		
		
		[Command ("ZTopActiveSelection")]		void CommandZTopActiveSelection()
		{
			System.Diagnostics.Debug.Assert (this.active_editor != null);
			System.Diagnostics.Debug.Assert (this.active_editor.SelectedWidgets.Count > 0);
			
			Widget[] widgets = this.GetSelectedWidgets ();
			
			this.ChangeZOrder (widgets, -1000);
		}
		
		[Command ("ZBottomActiveSelection")]	void CommandZBottomActiveSelection()
		{
			System.Diagnostics.Debug.Assert (this.active_editor != null);
			System.Diagnostics.Debug.Assert (this.active_editor.SelectedWidgets.Count > 0);
			
			Widget[] widgets = this.GetSelectedWidgets ();
			
			this.ChangeZOrder (widgets, 1000);
		}
		
		[Command ("ZUpActiveSelection")]		void CommandZUpActiveSelection()
		{
			System.Diagnostics.Debug.Assert (this.active_editor != null);
			System.Diagnostics.Debug.Assert (this.active_editor.SelectedWidgets.Count > 0);
			
			Widget[] widgets = this.GetSelectedWidgets ();
			
			this.ChangeZOrder (widgets, -1);
		}
		
		[Command ("ZDownActiveSelection")]		void CommandZDownActiveSelection()
		{
			System.Diagnostics.Debug.Assert (this.active_editor != null);
			System.Diagnostics.Debug.Assert (this.active_editor.SelectedWidgets.Count > 0);
			
			Widget[] widgets = this.GetSelectedWidgets ();
			
			this.ChangeZOrder (widgets, 1);
		}
		
		
		[Command ("DeselectAll")]				void CommandDeselectAll()
		{
			System.Diagnostics.Debug.Assert (this.active_editor != null);
			System.Diagnostics.Debug.Assert (this.active_editor.SelectedWidgets.Count > 0);
			
			this.DeselectAllWidgets ();
		}
		
		[Command ("ReselectActiveSelection")]	void CommandReselectActiveSelection()
		{
			System.Diagnostics.Debug.Assert (this.active_editor != null);
			System.Diagnostics.Debug.Assert (this.active_editor.SelectedWidgets.Count > 0);
			
			Widget[] selected = this.GetSelectedWidgets ();
			
			this.DeselectAllWidgets ();
			
			this.ReselectWidgets (selected);
		}
		
		[Command ("TabIndexSetter")]			void CommandTabIndexSetter(CommandDispatcher d, CommandEventArgs e)
		{
			System.Diagnostics.Debug.Assert (this.active_editor != null);
			
			bool enable;
			Types.Converter.Convert (e.CommandArgs[0], out enable);
			
			this.active_editor.SelectedWidgets.Clear ();
			this.SetTabIndexSetter (enable);
		}
		
		[Command ("TabIndexPicker")]			void CommandTabIndexPicker(CommandDispatcher d, CommandEventArgs e)
		{
			System.Diagnostics.Debug.Assert (this.active_editor != null);
			
			bool enable;
			Types.Converter.Convert (e.CommandArgs[0], out enable);
			
			this.active_editor.SelectedWidgets.Clear ();
			this.SetTabIndexPicker (enable);
		}
		
		[Command ("TabIndexResetSeq")]			void CommandTabIndexResetSeq()
		{
			this.SetTabIndexPicker (false);
			this.ResetTabIndexSeq (this.WidgetEditors);
		}
		
		[Command ("TabIndexStartSeq")]			void CommandTabIndexStartSeq()
		{
			System.Diagnostics.Debug.Assert (this.active_editor != null);
			
			this.SetTabIndexPicker (false);
			this.ResetTabIndexSeq (this.WidgetEditors);
			this.active_editor.StartTabIndexSeq ();
		}
		
		[Command ("TabIndexDefine")]			void CommandTabIndexDefine(CommandDispatcher d, CommandEventArgs e)
		{
			System.Diagnostics.Debug.Assert (this.active_editor != null);
			
			int index;
			Types.Converter.Convert (e.CommandArgs[0], out index);
			
			this.active_editor.DefineTabIndex (index);
		}
		
		
		
		protected enum Command
		{
			CreateNewInterface,
			OpenLoadInterface,
			SaveActiveInterface,
			
			DeleteActiveSelection,
			ZTopActiveSelection,
			ZBottomActiveSelection,
			ZUpActiveSelection,
			ZDownActiveSelection,
			
			TabIndexSetter,
			TabIndexPicker,
			TabIndexResetSeq,
		}


		protected virtual void OnActiveEditorChanged()
		{
			if (this.ActiveEditorChanged != null)
			{
				this.ActiveEditorChanged (this);
			}
		}
		
		
		
		private const string					prop_saved_window_params = "$designer$saved window params$";
		
		
		public event Support.EventHandler		ActiveEditorChanged;
		
		
		private bool							is_initialised;
		
		protected Widget						creation_panel;
		protected TabBook						creation_book;
		protected Panels.WidgetSourcePanel	widget_palette;
		protected Panels.DataSourcePanel		data_palette;
		
		protected AbstractToolBar				tool_bar;
		protected bool							is_tool_tab_setter_active;
		protected bool							is_tool_tab_picker_active;
		
		protected Widget						attribute_panel;
		protected Panels.WidgetAttributePanel	attribute_palette;
		
		protected System.Collections.ArrayList	edit_window_list	= new System.Collections.ArrayList ();
		protected Editors.WidgetEditor[]		editors;
		
		protected Editors.WidgetEditor			active_editor;
		protected Widget						active_widget;
	}
}
