using System.Collections.Generic;
using System.Text.RegularExpressions;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

namespace Epsitec.Common.Designer.Viewers
{
	/// <summary>
	/// Permet de représenter les ressources d'un module.
	/// </summary>
	public class Panels : Abstract
	{
		public Panels(Module module, PanelsContext context, ResourceAccess access, MainWindow mainWindow) : base(module, context, access, mainWindow)
		{
			int tabIndex = 0;

			this.left = new Widget(this);
			this.left.MinWidth = 80;
			this.left.MaxWidth = 400;
			this.left.PreferredWidth = Abstract.leftArrayWidth;
			this.left.Dock = DockStyle.Left;
			this.left.Padding = new Margins(10, 10, 10, 10);

			this.labelEdit = new MyWidgets.TextFieldExName(this.left);
			this.labelEdit.Margins = new Margins(0, 0, 10, 0);
			this.labelEdit.Dock = DockStyle.Bottom;
			this.labelEdit.ButtonShowCondition = ShowCondition.WhenModified;
			this.labelEdit.EditionAccepted += new EventHandler(this.HandleTextChanged);
			this.labelEdit.KeyboardFocusChanged += new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleLabelKeyboardFocusChanged);
			this.labelEdit.TabIndex = tabIndex++;
			this.labelEdit.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.labelEdit.Visibility = (this.module.Mode == DesignerMode.Build);

			this.array = new MyWidgets.StringArray(this.left);
			this.array.Columns = 1;
			this.array.SetColumnsRelativeWidth(0, 1.00);
			this.array.SetColumnBreakMode(0, TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine);
			this.array.SetDynamicToolTips(0, true);
			this.array.Margins = new Margins(0, 0, 0, 0);
			this.array.Dock = DockStyle.Fill;
			this.array.CellCountChanged += new EventHandler (this.HandleArrayCellCountChanged);
			this.array.SelectedRowChanged += new EventHandler(this.HandleArraySelectedRowChanged);
			this.array.TabIndex = tabIndex++;
			this.array.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.splitter1 = new VSplitter(this);
			this.splitter1.Dock = DockStyle.Left;
			this.splitter1.SplitterDragged += new EventHandler(this.HandleSplitterDragged);
			VSplitter.SetAutoCollapseEnable(this.left, true);

			this.toolBar = new VToolBar(this);
			this.toolBar.Margins = new Margins(0, 0, 0, 0);
			this.toolBar.Dock = DockStyle.Left;
			this.ToolBarAdd(Widgets.Command.Get("ToolSelect"));
			this.ToolBarAdd(Widgets.Command.Get("ToolGlobal"));
			this.ToolBarAdd(Widgets.Command.Get("ToolGrid"));
			//?this.ToolBarAdd(Widgets.Command.Get("ToolEdit"));
			//?this.ToolBarAdd(Widgets.Command.Get("ToolZoom"));
			//?this.ToolBarAdd(Widgets.Command.Get("ToolHand"));
			this.ToolBarAdd(null);
			this.ToolBarAdd(Widgets.Command.Get("ObjectVLine"));
			this.ToolBarAdd(Widgets.Command.Get("ObjectHLine"));
			this.ToolBarAdd(Widgets.Command.Get("ObjectStatic"));
			this.ToolBarAdd(Widgets.Command.Get("ObjectSquareButton"));
			this.ToolBarAdd(Widgets.Command.Get("ObjectRectButton"));
			this.ToolBarAdd(Widgets.Command.Get("ObjectText"));
			this.ToolBarAdd(Widgets.Command.Get("ObjectGroup"));
			this.ToolBarAdd(Widgets.Command.Get("ObjectGroupBox"));

			this.scrollable = new Scrollable(this);
			this.scrollable.MinWidth = 100;
			this.scrollable.MinHeight = 100;
			this.scrollable.Margins = new Margins(1, 1, 1, 1);
			this.scrollable.Dock = DockStyle.Fill;
			this.scrollable.HorizontalScrollerMode = ScrollableScrollerMode.ShowAlways;
			this.scrollable.VerticalScrollerMode = ScrollableScrollerMode.ShowAlways;
			this.scrollable.Panel.IsAutoFitting = true;
			this.scrollable.IsForegroundFrame = true;
			//?this.scrollable.ForegroundFrameMargins = new Margins(0, 1, 0, 1);

			Widget container = new Widget(this.scrollable.Panel);
			container.MinWidth = 100;
			container.Dock = DockStyle.Fill;

			this.panelContainer = this.access.CreateEmptyPanel();
			this.panelContainer.SetParent(container);

			//	Le PanelEditor est par-dessus le UI.Panel.
			this.panelEditor = new MyWidgets.PanelEditor(container);
			this.panelEditor.Initialize(this.module, this.context, this.panelContainer);
			this.panelEditor.MinWidth = 100;
			this.panelEditor.MinHeight = 100;
			this.panelEditor.Anchor = AnchorStyles.All;
			this.panelEditor.ChildrenAdded += new EventHandler(this.HandlePanelEditorChildrenAdded);
			this.panelEditor.ChildrenSelected += new EventHandler(this.HandlePanelEditorChildrenSelected);
			this.panelEditor.ChildrenGeometryChanged += new EventHandler(this.HandlePanelEditorChildrenGeometryChanged);
			this.panelEditor.UpdateCommands += new EventHandler(this.HandlePanelEditorUpdateCommands);

			this.tabBook = new TabBook(this);
			this.tabBook.MinWidth = 150;
			this.tabBook.PreferredWidth = 240;
			this.tabBook.MaxWidth = 400;
			this.tabBook.Arrows = TabBookArrows.Stretch;
			this.tabBook.Margins = new Margins(1, 1, 1, 1);
			this.tabBook.Dock = DockStyle.Right;

			//	Crée l'onglet 'propriétés'.
			this.tabPageProperties = new TabPage();
			this.tabPageProperties.TabTitle = Res.Strings.Viewers.Panels.TabProperties;
			this.tabPageProperties.Padding = new Margins(4, 4, 4, 4);
			this.tabBook.Items.Add(this.tabPageProperties);

			this.proxyManager = new ProxyManager(this);

			this.propertiesScrollable = new Scrollable(this.tabPageProperties);
			this.propertiesScrollable.Dock = DockStyle.Fill;
			this.propertiesScrollable.HorizontalScrollerMode = ScrollableScrollerMode.ShowAlways;
			this.propertiesScrollable.VerticalScrollerMode = ScrollableScrollerMode.ShowAlways;
			this.propertiesScrollable.Panel.IsAutoFitting = true;
			this.propertiesScrollable.Panel.Margins = new Margins(10, 10, 10, 10);
			this.propertiesScrollable.IsForegroundFrame = true;
			//?this.propertiesScrollable.ForegroundFrameMargins = new Margins(0, 1, 0, 0);

			//	Crée l'onglet 'objets'.
			this.tabPageObjects = new TabPage();
			this.tabPageObjects.TabTitle = Res.Strings.Viewers.Panels.TabObjects;
			this.tabPageObjects.Padding = new Margins(10, 10, 10, 10);
			this.tabBook.Items.Add(this.tabPageObjects);

			//	Crée l'onglet 'cultures'.
			this.tabPageCultures = new TabPage();
			this.tabPageCultures.TabTitle = Res.Strings.Viewers.Panels.TabCultures;
			this.tabPageCultures.Padding = new Margins(10, 10, 10, 10);
			this.tabBook.Items.Add(this.tabPageCultures);

			this.CreateCultureButtons();

			this.tabBook.ActivePage = this.tabPageProperties;

			this.splitter2 = new VSplitter(this);
			this.splitter2.Dock = DockStyle.Right;

			this.UpdateEdit();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.splitter1.SplitterDragged -= new EventHandler(this.HandleSplitterDragged);

				this.array.CellCountChanged -= new EventHandler(this.HandleArrayCellCountChanged);
				this.array.SelectedRowChanged -= new EventHandler(this.HandleArraySelectedRowChanged);

				this.labelEdit.EditionAccepted -= new EventHandler(this.HandleTextChanged);
				this.labelEdit.KeyboardFocusChanged -= new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleLabelKeyboardFocusChanged);
			}

			base.Dispose(disposing);
		}


		public override ResourceAccess.Type ResourceType
		{
			get
			{
				return ResourceAccess.Type.Panels;
			}
		}


		public MyWidgets.PanelEditor PanelEditor
		{
			get
			{
				return this.panelEditor;
			}
		}


		public override void PaintHandler(Graphics graphics, Rectangle repaint, IPaintFilter paintFilter)
		{
			if (paintFilter == null)
			{
				paintFilter = this.panelEditor;
			}
			
			base.PaintHandler(graphics, repaint, paintFilter);
		}


		public override void DoTool(string name)
		{
			//	Choix de l'outil.
			base.DoTool(name);
			this.panelEditor.AdaptAfterToolChanged();
			this.RegenerateProxies();
		}

		public override void DoCommand(string name)
		{
			//	Exécute une commande.
			if (name == "PanelRun")
			{
				this.module.MainWindow.ActiveButton("PanelRun", true);
				this.module.RunPanel(this.access.AccessIndex);
				this.module.MainWindow.ActiveButton("PanelRun", false);
				return;
			}

			this.panelEditor.DoCommand(name);
			base.DoCommand(name);
		}

		public override string InfoViewerText
		{
			//	Donne le texte d'information sur le visualisateur en cours.
			get
			{
				return this.panelEditor.SelectionInfo;
			}
		}


		protected override void UpdateEdit()
		{
			//	Met à jour les lignes éditables en fonction de la sélection dans le tableau.
			bool iic = this.ignoreChange;
			this.ignoreChange = true;

			int sel = this.access.AccessIndex;

			if (sel >= this.access.AccessCount)
			{
				sel = -1;
			}

			if (this.panelContainer != null)
			{
				this.panelContainer.SetParent(null);
				this.panelContainer = null;
			}
			
			if (sel == -1)
			{
				this.SetTextField(this.labelEdit, 0, null, ResourceAccess.FieldType.None);
			}
			else
			{
				this.panelContainer = this.access.NewPanel(sel);
				this.panelContainer.SetParent(this.panelEditor.Parent);
				this.panelContainer.ZOrder = this.panelEditor.ZOrder+1;
				this.panelEditor.Panel = this.panelContainer;

				int index = this.access.AccessIndex;
				this.SetTextField(this.labelEdit, index, null, ResourceAccess.FieldType.Name);
				this.labelEdit.SelectAll();
				this.labelEdit.Focus();
			}

			this.ignoreChange = iic;

			this.UpdateCommands();
		}


		public override void Update()
		{
			//	Met à jour le contenu du Viewer.
			this.UpdateArray();
			this.UpdateEdit();
			this.UpdateCommands();
		}


		protected void CreateCultureButtons()
		{
			//	Crée tous les boutons pour les cultures.
			this.cultureButtonList = new List<IconButton>();

			int tabIndex = 0;
			foreach (string name in Misc.Cultures)
			{
				System.Globalization.CultureInfo culture = Resources.FindSpecificCultureInfo(name);

				IconButton button = new IconButton(this.tabPageCultures);
				button.Name = name;
				button.Text = Misc.CultureName(culture);
				button.ButtonStyle = ButtonStyle.ActivableIcon;
				button.Dock = DockStyle.Top;
				button.Margins = new Margins(0, 0, 0, 2);
				button.TabIndex = tabIndex++;
				button.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				button.Clicked += new MessageEventHandler(this.HandleCultureButtonClicked);
				ToolTip.Default.SetToolTip(button, Misc.CultureLongName(culture));

				this.cultureButtonList.Add(button);
			}

			this.UpdateCultureButtons();
		}

		void HandleCultureButtonClicked(object sender, MessageEventArgs e)
		{
			//	Un bouton pour changer de culture a été cliqué.
			IconButton button = sender as IconButton;
			this.module.ResourceManager.ActiveCulture = Resources.FindSpecificCultureInfo(button.Name);
			this.panelContainer.UpdateDisplayCaptions ();
			this.UpdateCultureButtons();
		}

		protected void UpdateCultureButtons()
		{
			//	Met à jour les boutons pour les cultures.
			string culture = Misc.CultureBaseName(this.module.ResourceManager.ActiveCulture);
			foreach (IconButton button in this.cultureButtonList)
			{
				bool active = (button.Name == culture);
				button.ActiveState = active ? ActiveState.Yes : ActiveState.No;
			}
		}


		protected Widget ToolBarAdd(Command command)
		{
			//	Ajoute une icône.
			if (command == null)
			{
				IconSeparator sep = new IconSeparator();
				sep.IsHorizontal = false;
				this.toolBar.Items.Add(sep);
				return sep;
			}
			else
			{
				IconButton button = new IconButton(command);
				this.toolBar.Items.Add(button);
				ToolTip.Default.SetToolTip(button, Misc.GetTextWithShortcut(command));
				return button;
			}
		}


		#region Proxies
		protected void DefineProxies(IEnumerable<Widget> widgets)
		{
			//	Crée les proxies et l'interface utilisateur pour les widgets sélectionnés.
			this.ClearProxies();
			this.proxyManager.SetSelection(widgets);
			this.proxyManager.CreateUserInterface(this.propertiesScrollable.Panel);
		}

		protected void ClearProxies()
		{
			//	Supprime l'interface utilisateur pour les widgets sélectionnés.
			this.proxyManager.ClearUserInterface(this.propertiesScrollable.Panel);
		}

		protected void UpdateProxies()
		{
			//	Met à jour les proxies et l'interface utilisateur (panneaux), sans changer
			//	le nombre de propriétés visibles par panneau.
			this.proxyManager.UpdateUserInterface();
		}

		public void RegenerateProxies()
		{
			//	Régénère la liste des proxies et met à jour les panneaux de l'interface
			//	utilisateur s'il y a eu un changement dans le nombre de propriétés visibles
			//	par panneau.
			if (this.proxyManager.RegenerateProxies())
			{
				this.ClearProxies();
				this.proxyManager.CreateUserInterface(this.propertiesScrollable.Panel);
			}
		}
		#endregion


		protected override void TextFieldToIndex(AbstractTextField textField, out int field, out int subfield)
		{
			//	Cherche les index correspondant à un texte éditable.
			field = -1;
			subfield = -1;

			if (textField == this.labelEdit)
			{
				field = 0;
				subfield = 0;
			}
		}

		protected override AbstractTextField IndexToTextField(int field, int subfield)
		{
			//	Cherche le TextField permettant d'éditer des index.
			if (field == 0 && subfield == 0)
			{
				return this.labelEdit;
			}

			return null;
		}


		private void HandleSplitterDragged(object sender)
		{
			//	Le splitter a été bougé.
			Abstract.leftArrayWidth = this.left.ActualWidth;
		}

		private void HandleArrayCellCountChanged(object sender)
		{
			//	Le nombre de lignes a changé.
			this.UpdateArray();
		}

		private void HandleArraySelectedRowChanged(object sender)
		{
			//	La ligne sélectionnée a changé.
			this.access.AccessIndex = this.array.SelectedRow;
			this.UpdateEdit();
			this.UpdateCommands();
			this.panelEditor.IsEditEnabled = (this.access.AccessIndex != -1);
			this.DefineProxies(this.panelEditor.SelectedObjects);
		}

		private void HandleLabelKeyboardFocusChanged(object sender, Epsitec.Common.Types.DependencyPropertyChangedEventArgs e)
		{
			//	Appelé lorsque la ligne éditable pour le label voit son focus changer.
			TextFieldEx field = sender as TextFieldEx;
			field.AcceptEdition();
			this.HandleEditKeyboardFocusChanged(sender, e);
		}

		private void HandleTextChanged(object sender)
		{
			//	Un texte éditable a changé.
			if ( this.ignoreChange )  return;

			AbstractTextField edit = sender as AbstractTextField;
			int sel = this.access.AccessIndex;
			this.UpdateFieldName(edit, sel);

			this.UpdateArray();
		}

		private void HandlePanelEditorChildrenAdded(object sender)
		{
			this.UpdateCommands();
		}

		private void HandlePanelEditorChildrenSelected(object sender)
		{
			this.UpdateCommands();
			this.DefineProxies(this.panelEditor.SelectedObjects);
		}

		private void HandlePanelEditorChildrenGeometryChanged(object sender)
		{
			this.UpdateProxies();
		}

		private void HandlePanelEditorUpdateCommands(object sender)
		{
			this.UpdateCommands();
		}


		public static void SetPanel(DependencyObject obj, UI.Panel panel)
		{
			if (panel == null)
			{
				obj.ClearValue (Panels.PanelProperty);
			}
			else
			{
				obj.SetValue (Panels.PanelProperty, panel);
			}
		}

		public static UI.Panel GetPanel(DependencyObject obj)
		{
			return (UI.Panel) obj.GetValue(Panels.PanelProperty);
		}
		
		public static readonly DependencyProperty PanelProperty = DependencyProperty.RegisterAttached("Panel", typeof(UI.Panel), typeof(Panels));


		protected ProxyManager					proxyManager;
		protected Widget						left;
		protected VSplitter						splitter1;
		protected VSplitter						splitter2;
		protected MyWidgets.TextFieldExName		labelEdit;
		protected VToolBar						toolBar;
		protected Scrollable					scrollable;
		protected UI.Panel						panelContainer;
		protected MyWidgets.PanelEditor			panelEditor;
		protected TabBook						tabBook;

		protected TabPage						tabPageProperties;
		protected Scrollable					propertiesScrollable;

		protected TabPage						tabPageObjects;

		protected TabPage						tabPageCultures;
		protected List<IconButton>				cultureButtonList;
	}
}
