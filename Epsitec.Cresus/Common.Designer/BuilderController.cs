//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.Designer
{
	/// <summary>
	/// La classe BuilderController implémente la logique de l'application de design des
	/// interfaces graphiques.
	/// </summary>
	public class BuilderController : Support.ICommandDispatcherHost
	{
		public BuilderController(Support.CommandDispatcher base_dispatcher)
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
			this.CreateAttributeWindow ();
			
			//	Enregistre cette classe comme un contrôleur sachant exécuter des commandes
			//	interactives :
			
			this.dispatcher.RegisterController (this);
			
			//	Définit l'état initial des commandes :
			
			this.StateDeleteActiveSelection.Enabled = false;
			this.StateTabIndexSetter.Enabled        = false;
			
			this.UpdateTabIndexIcons ();
			
			this.is_initialised = true;
		}
		
		
		public Support.CommandDispatcher		CommandDispatcher
		{
			get
			{
				return this.dispatcher;
			}
			set
			{
				if (this.dispatcher != value)
				{
					throw new System.InvalidOperationException ("CommandDispatcher may not be changed.");
				}
			}
		}
		
		public Window							CreationWindow
		{
			get { return this.creation_window; }
		}
		
		public Window							AttributeWindow
		{
			get { return this.attribute_window; }
		}
		
		
		public Editors.WidgetEditor[]			WidgetEditors
		{
			get
			{
				Editors.WidgetEditor[] editors = new Editors.WidgetEditor[this.edit_window_list.Count];
				int i = 0;
				
				foreach (Window window in this.edit_window_list)
				{
					editors[i++] = Editors.WidgetEditor.FromWindow (window);
				}
				
				return editors;
			}
		}
		
		
		public CommandState						StateDeleteActiveSelection
		{
			get
			{
				return CommandState.Find ("DeleteActiveSelection", this.dispatcher);
			}
		}
		
		public CommandState						StateTabIndexSetter
		{
			get
			{
				return CommandState.Find ("TabIndexSetter", this.dispatcher);
			}
		}
		
		public CommandState						StateTabIndexPicker
		{
			get
			{
				return CommandState.Find ("TabIndexPicker", this.dispatcher);
			}
		}
		
		
		public Window CreateWindow(string name)
		{
			Window window = new Window ();
			
			window.Name = name;
			window.CommandDispatcher = this.dispatcher;
			window.MakeToolWindow ();
			
			return window;
		}
		
		
		protected void CreateCreationWindow()
		{
			//	Crée la fenêtre contenant la palette des widgets qui peuvent être utilisés
			//	pour construire une interface par drag & drop.
			
			this.creation_window = this.CreateWindow ("widget palette");
			this.creation_window.Text = "Widgets...";
			
			this.widget_palette = new Panels.WidgetSourcePalette ();
			
			double dx = this.widget_palette.Size.Width;
			double dy = this.widget_palette.Size.Height;
			
			this.creation_window.ClientSize = new Drawing.Size (dx, dy);
			
			Widget root   = this.creation_window.Root;
			Widget widget;
			
			//	Initialisation de la palette des widgets drag-ables :
			
			widget = this.widget_palette.Widget;
			
			widget.Parent = root;
			widget.Dock   = DockStyle.Fill;
			
			this.widget_palette.DragBegin += new Support.EventHandler (this.HandleSourceDragBegin);
			this.widget_palette.DragEnd   += new Support.EventHandler (this.HandleSourceDragEnd);
		}
		
		protected void CreateAttributeWindow()
		{
			//	Crée la fenêtre contenant les attributs.
			
			this.attribute_window = this.CreateWindow ("widget palette");
			this.attribute_window.Text = "Attributes";
			
			this.attribute_palette = new Panels.WidgetAttributePalette ();
			this.tool_bar          = new HToolBar ();
			
			double dx = this.attribute_palette.Size.Width;
			double dy = this.attribute_palette.Size.Height + this.tool_bar.DefaultHeight;
			
			this.attribute_window.ClientSize = new Drawing.Size (dx, dy);
			
			Widget root   = this.attribute_window.Root;
			Widget widget;
			
			//	Initialisation de la palette des attributs :
			
			widget = this.attribute_palette.Widget;
			
			widget.Parent = root;
			widget.Dock   = DockStyle.Fill;
			
			//	Initialisation de la barre d'outils pour l'édition :
			
			this.tool_bar.Items.Add (IconButton.CreateSimple ("CreateNewWindow",  "file:images/new.icon"));
			this.tool_bar.Items.Add (IconButton.CreateSimple ("OpenLoadWindow",   "file:images/open.icon"));
			this.tool_bar.Items.Add (IconButton.CreateSimple ("SaveActiveWindow", "file:images/save.icon"));
			this.tool_bar.Items.Add (new IconSeparator ());
			this.tool_bar.Items.Add (IconButton.CreateToggle ("TabIndexSetter(this.IsActive)",	"file:images/numtabindex.icon"));
			this.tool_bar.Items.Add (IconButton.CreateSimple ("TabIndexResetSeq",				"file:images/numone.icon"));
			this.tool_bar.Items.Add (IconButton.CreateToggle ("TabIndexPicker(this.IsActive)",	"file:images/numpicker.icon"));
			this.tool_bar.Items.Add (new IconSeparator ());
			this.tool_bar.Items.Add (IconButton.CreateSimple ("DeleteActiveSelection", "file:images/delete.icon"));
			
			this.tool_bar.Size   = new Drawing.Size (dx, this.tool_bar.DefaultHeight);
			this.tool_bar.Parent = root;
			this.tool_bar.Dock   = DockStyle.Top;
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
			System.Diagnostics.Debug.Assert (this.widget_palette == sender);
			
			//	Le drag & drop vient de se terminer.
			
			Widget widget = this.widget_palette.DroppedWidget;
			
			if (widget != null)
			{
				Editors.WidgetEditor editor = Editors.WidgetEditor.FromWidget (widget);
				
				System.Diagnostics.Debug.Assert (editor != null);
				System.Diagnostics.Debug.Assert (editor.SelectedWidgets.Count == 0);
				
				editor.SelectedWidgets.Add (widget);
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
		
		protected void UpdateTabIndexIcons()
		{
			bool enable = this.StateTabIndexSetter.ActiveState == WidgetState.ActiveYes;
			
			this.tool_bar.FindChild ("TabIndexResetSeq").SetVisible (enable);
			this.tool_bar.FindChild ("TabIndexPicker").SetVisible (enable);
		}
		
		
		
		protected void SetActiveEditor(Editors.WidgetEditor editor)
		{
			//	Change d'éditeur actif. Si on change d'éditeur, on s'assure qu'aucune
			//	sélection n'est encore active dans un autre éditeur :
			
			if (this.active_editor != editor)
			{
				this.active_editor = editor;
				
				foreach (Window window in this.edit_window_list)
				{
					editor = Editors.WidgetEditor.FromWindow (window);
					
					if (editor != this.active_editor)
					{
						editor.SelectedWidgets.Clear ();
					}
				}
				
				if (this.active_editor != null)
				{
					this.active_editor.CommandDispatcher = this.CommandDispatcher;
					this.StateTabIndexSetter.Enabled     = true;
					
					this.active_editor.SetTabIndexSetterMode (this.tool_tab_setter_active);
					this.active_editor.SetTabIndexPickerMode (this.tool_tab_picker_active);
				}
				else
				{
					this.StateTabIndexSetter.Enabled = false;
				}
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
			if (this.tool_tab_setter_active != enable)
			{
				this.StateTabIndexSetter.ActiveState = enable ? WidgetState.ActiveYes : WidgetState.ActiveNo;
				this.tool_tab_setter_active          = enable;
				
				this.SetTabIndexSetter (this.WidgetEditors);
				this.SetTabIndexPicker (false);
				this.UpdateTabIndexIcons ();
			}
		}
		
		protected void SetTabIndexPicker(bool enable)
		{
			if (this.tool_tab_picker_active != enable)
			{
				this.StateTabIndexPicker.ActiveState = enable ? WidgetState.ActiveYes : WidgetState.ActiveNo;
				this.tool_tab_picker_active          = enable;
				
				this.SetTabIndexPicker (this.WidgetEditors);
				this.UpdateTabIndexIcons ();
			}
		}
				
		protected void SetTabIndexSetter(Editors.WidgetEditor[] editors)
		{
			for (int i = 0; i < editors.Length; i++)
			{
				editors[i].SetTabIndexSetterMode (this.tool_tab_setter_active);
			}
		}
		
		protected void SetTabIndexPicker(Editors.WidgetEditor[] editors)
		{
			for (int i = 0; i < editors.Length; i++)
			{
				editors[i].SetTabIndexPickerMode (this.tool_tab_picker_active);
			}
		}
		
		protected void ResetTabIndexSeq(Editors.WidgetEditor[] editors)
		{
			for (int i = 0; i < editors.Length; i++)
			{
				editors[i].ResetTabIndexSeq ();
			}
		}
		
		
		[Command ("CreateNewWindow")]			void CommandCreateNewWindow()
		{
			Window window = new Window ();
			
			window.ClientSize = new Drawing.Size (400, 300);
			window.Root.IsEditionEnabled = true;
			
			this.edit_window_list.Add (window);
			
			Editors.WidgetEditor editor = new Editors.WidgetEditor ();
			
			editor.Root  = window.Root;
			
			editor.Selected   += new SelectionEventHandler (this.HandleEditorSelected);
			editor.Deselected += new SelectionEventHandler (this.HandleEditorDeselected);
			
			window.WindowActivated += new Support.EventHandler (this.HandleEditWindowWindowActivated);
			window.Show ();
		}
		
		[Command ("SaveActiveWindow")]			void CommandSaveActiveWindow()
		{
			System.Diagnostics.Debug.Assert (this.active_editor != null);
			System.Diagnostics.Debug.Assert (this.active_editor.Root != null);
			
			Widget root = this.active_editor.Root;
			
			Support.ObjectBundler  bundler = new Support.ObjectBundler ();
			Support.ResourceBundle bundle  = Support.ResourceBundle.Create (root.Name, "file", ResourceLevel.Default, System.Globalization.CultureInfo.CurrentCulture);
			
			bundler.SetupPrefix ("file");
			bundler.FillBundleFromObject (bundle, root);
			
			bundle.CreateXmlDocument (false).Save (@"resources\test.00.resource");
		}
		
		[Command ("OpenLoadWindow")]			void CommandOpenLoadWindow()
		{
			Support.ObjectBundler  bundler = new Support.ObjectBundler ();
			Support.ResourceBundle bundle  = Support.Resources.GetBundle ("file:test");
			
			Widget root   = bundler.CreateFromBundle (bundle) as Widget;
			Window window = root.Window;
			
			root.IsEditionEnabled = true;
			
			this.edit_window_list.Add (window);
			
			Editors.WidgetEditor editor = new Editors.WidgetEditor ();
			
			editor.Root  = window.Root;
			
			editor.Selected   += new SelectionEventHandler (this.HandleEditorSelected);
			editor.Deselected += new SelectionEventHandler (this.HandleEditorDeselected);
			
			window.WindowActivated += new Support.EventHandler (this.HandleEditWindowWindowActivated);
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
		
		
		
		
		protected Support.CommandDispatcher		dispatcher;
		protected bool							is_initialised;
		
		protected Panels.WidgetSourcePalette	widget_palette;
		protected Panels.WidgetAttributePalette	attribute_palette;
		protected AbstractToolBar				tool_bar;
		protected bool							tool_tab_setter_active;
		protected bool							tool_tab_picker_active;
		
		protected Window						creation_window;
		protected Window						attribute_window;
		protected System.Collections.ArrayList	edit_window_list;
		
		protected Window						active_window;
		protected Editors.WidgetEditor			active_editor;
		protected Widget						active_widget;
	}
}
