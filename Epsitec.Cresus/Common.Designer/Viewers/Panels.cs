using System.Collections.Generic;
using System.Text.RegularExpressions;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

namespace Epsitec.Common.Designer.Viewers
{
	/// <summary>
	/// Permet de repr�senter les ressources d'un module.
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
			this.labelEdit.TabNavigationMode = TabNavigationMode.ActivateOnTab;
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

			this.splitter1 = new VSplitter(this);
			this.splitter1.Dock = DockStyle.Left;
			this.splitter1.SplitterDragged += new EventHandler(this.HandleSplitterDragged);
			VSplitter.SetAutoCollapseEnable(this.left, true);

			this.vToolBar = new VToolBar(this);
			this.vToolBar.Margins = new Margins(0, 0, 0, 0);
			this.vToolBar.Dock = DockStyle.Left;
			this.VToolBarAdd(Widgets.Command.Get("ToolSelect"));
			this.VToolBarAdd(Widgets.Command.Get("ToolGlobal"));
			this.VToolBarAdd(Widgets.Command.Get("ToolGrid"));
			//?this.ToolBarAdd(Widgets.Command.Get("ToolEdit"));
			//?this.ToolBarAdd(Widgets.Command.Get("ToolZoom"));
			//?this.ToolBarAdd(Widgets.Command.Get("ToolHand"));
			this.VToolBarAdd(null);
			this.VToolBarAdd(Widgets.Command.Get("ObjectVLine"));
			this.VToolBarAdd(Widgets.Command.Get("ObjectHLine"));
			this.VToolBarAdd(Widgets.Command.Get("ObjectStatic"));
			this.VToolBarAdd(Widgets.Command.Get("ObjectSquareButton"));
			this.VToolBarAdd(Widgets.Command.Get("ObjectRectButton"));
			this.VToolBarAdd(Widgets.Command.Get("ObjectText"));
			this.VToolBarAdd(Widgets.Command.Get("ObjectGroup"));
			this.VToolBarAdd(Widgets.Command.Get("ObjectGroupBox"));
			this.VToolBarAdd(Widgets.Command.Get("ObjectPanel"));

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

			//	Cr�e le groupe droite.
			this.right = new Widget(this);
			this.right.MinWidth = 150;
			this.right.PreferredWidth = 240;
			this.right.MaxWidth = 400;
			this.right.Dock = DockStyle.Right;

			//	Cr�e la toolbar horizontale, au dessus des onglets.
			this.hToolBar = new HToolBar(this.right);
			this.hToolBar.Margins = new Margins(0, 0, 0, 5);
			this.hToolBar.Dock = DockStyle.Top;
			this.hButtonDefault = this.HToolBarAdd(Res.Captions.PanelMode.Default.Id);
			this.hButtonEdition = this.HToolBarAdd(Res.Captions.PanelMode.Edition.Id);
			this.hButtonSearch  = this.HToolBarAdd(Res.Captions.PanelMode.Search.Id);

			//	Cr�e le tabbook pour les onglets.
			this.tabBook = new TabBook(this.right);
			this.tabBook.Arrows = TabBookArrows.Stretch;
			this.tabBook.Dock = DockStyle.Fill;
			this.tabBook.Padding = new Margins(1, 1, 1, 1);

			//	Cr�e l'onglet 'propri�t�s'.
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

			//	Cr�e l'onglet 'objets'.
			this.tabPageObjects = new TabPage();
			this.tabPageObjects.TabTitle = Res.Strings.Viewers.Panels.TabObjects;
			this.tabPageObjects.Padding = new Margins(10, 10, 10, 10);
			this.tabBook.Items.Add(this.tabPageObjects);

			//	Cr�e l'onglet 'cultures'.
			this.tabPageCultures = new TabPage();
			this.tabPageCultures.TabTitle = Res.Strings.Viewers.Panels.TabCultures;
			this.tabPageCultures.Padding = new Margins(10, 10, 10, 10);
			this.tabBook.Items.Add(this.tabPageCultures);

			this.CreateCultureButtons();

			this.tabBook.ActivePage = this.tabPageProperties;

			this.splitter2 = new VSplitter(this);
			this.splitter2.Dock = DockStyle.Right;

			this.UpdateEdit();
			this.UpdateButtons();
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

				this.hButtonDefault.Clicked -= new MessageEventHandler(HandleHbuttonClicked);
				this.hButtonEdition.Clicked -= new MessageEventHandler(HandleHbuttonClicked);
				this.hButtonSearch.Clicked -= new MessageEventHandler(HandleHbuttonClicked);
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
			//	Ex�cute une commande.
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
			//	Met � jour les lignes �ditables en fonction de la s�lection dans le tableau.
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
				this.panelContainer = this.access.GetPanel(sel).GetPanel(this.panelMode);
				this.panelContainer.SetParent(this.panelEditor.Parent);
				this.panelContainer.ZOrder = this.panelEditor.ZOrder+1;
				this.panelContainer.DrawDesignerFrame = true;
				this.panelEditor.Panel = this.panelContainer;
				this.panelEditor.Druid = this.access.AccessDruid(sel);

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
			//	Met � jour le contenu du Viewer.
			this.UpdateArray();
			this.UpdateEdit();
			this.UpdateCommands();
		}

		protected void UpdateButtons()
		{
			//	Met � jour les boutons de la toolbar horizontale.
			this.hButtonDefault.ActiveState = (this.panelMode == UI.PanelMode.Default) ? ActiveState.Yes : ActiveState.No;
			this.hButtonEdition.ActiveState = (this.panelMode == UI.PanelMode.Edition) ? ActiveState.Yes : ActiveState.No;
			this.hButtonSearch.ActiveState = (this.panelMode == UI.PanelMode.Search) ? ActiveState.Yes : ActiveState.No;
		}


		protected void CreateCultureButtons()
		{
			//	Cr�e tous les boutons pour les cultures.
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
				button.TabNavigationMode = TabNavigationMode.ActivateOnTab;
				button.Clicked += new MessageEventHandler(this.HandleCultureButtonClicked);
				ToolTip.Default.SetToolTip(button, Misc.CultureLongName(culture));

				this.cultureButtonList.Add(button);
			}

			this.UpdateCultureButtons();
		}

		void HandleCultureButtonClicked(object sender, MessageEventArgs e)
		{
			//	Un bouton pour changer de culture a �t� cliqu�.
			IconButton button = sender as IconButton;
			this.module.ResourceManager.ActiveCulture = Resources.FindSpecificCultureInfo(button.Name);
			this.panelContainer.UpdateDisplayCaptions();
			this.UpdateCultureButtons();
		}

		protected void UpdateCultureButtons()
		{
			//	Met � jour les boutons pour les cultures.
			string culture = Misc.CultureBaseName(this.module.ResourceManager.ActiveCulture);
			foreach (IconButton button in this.cultureButtonList)
			{
				bool active = (button.Name == culture);
				button.ActiveState = active ? ActiveState.Yes : ActiveState.No;
			}
		}


		protected Widget VToolBarAdd(Command command)
		{
			//	Ajoute une ic�ne dans la toolbar verticale.
			if (command == null)
			{
				IconSeparator sep = new IconSeparator();
				sep.IsHorizontal = false;
				this.vToolBar.Items.Add(sep);
				return sep;
			}
			else
			{
				IconButton button = new IconButton(command);
				this.vToolBar.Items.Add(button);
				ToolTip.Default.SetToolTip(button, Misc.GetTextWithShortcut(command));
				return button;
			}
		}

		protected IconButton HToolBarAdd(Druid caption)
		{
			//	Ajoute un bouton dans la toolbar horizontale.
			IconButton button = new IconButton();
			button.CaptionId = caption;
			button.ButtonStyle = ButtonStyle.ActivableIcon;
			button.Clicked += new MessageEventHandler(HandleHbuttonClicked);

			this.hToolBar.Items.Add(button);
			
			return button;
		}


		#region Proxies
		protected void DefineProxies(IEnumerable<Widget> widgets)
		{
			//	Cr�e les proxies et l'interface utilisateur pour les widgets s�lectionn�s.
			this.ClearProxies();
			this.proxyManager.SetSelection(widgets);
			this.proxyManager.CreateUserInterface(this.propertiesScrollable.Panel);
		}

		protected void ClearProxies()
		{
			//	Supprime l'interface utilisateur pour les widgets s�lectionn�s.
			this.proxyManager.ClearUserInterface(this.propertiesScrollable.Panel);
		}

		protected void UpdateProxies()
		{
			//	Met � jour les proxies et l'interface utilisateur (panneaux), sans changer
			//	le nombre de propri�t�s visibles par panneau.
			this.proxyManager.UpdateUserInterface();
		}

		public void RegenerateProxies()
		{
			//	R�g�n�re la liste des proxies et met � jour les panneaux de l'interface
			//	utilisateur s'il y a eu un changement dans le nombre de propri�t�s visibles
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
			//	Cherche les index correspondant � un texte �ditable.
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
			//	Cherche le TextField permettant d'�diter des index.
			if (field == 0 && subfield == 0)
			{
				return this.labelEdit;
			}

			return null;
		}


		private void HandleSplitterDragged(object sender)
		{
			//	Le splitter a �t� boug�.
			Abstract.leftArrayWidth = this.left.ActualWidth;
		}

		private void HandleArrayCellCountChanged(object sender)
		{
			//	Le nombre de lignes a chang�.
			this.UpdateArray();
		}

		private void HandleArraySelectedRowChanged(object sender)
		{
			//	La ligne s�lectionn�e a chang�.
			this.access.AccessIndex = this.array.SelectedRow;
			this.UpdateEdit();
			this.UpdateCommands();
			this.panelEditor.IsEditEnabled = (this.access.AccessIndex != -1);
			this.DefineProxies(this.panelEditor.SelectedObjects);
		}

		private void HandleLabelKeyboardFocusChanged(object sender, Epsitec.Common.Types.DependencyPropertyChangedEventArgs e)
		{
			//	Appel� lorsque la ligne �ditable pour le label voit son focus changer.
			TextFieldEx field = sender as TextFieldEx;
			field.AcceptEdition();
			this.HandleEditKeyboardFocusChanged(sender, e);
		}

		private void HandleTextChanged(object sender)
		{
			//	Un texte �ditable a chang�.
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

		private void HandleHbuttonClicked(object sender, MessageEventArgs e)
		{
			if (sender == this.hButtonDefault)
			{
				this.panelMode = UI.PanelMode.Default;
			}

			if (sender == this.hButtonEdition)
			{
				this.panelMode = UI.PanelMode.Edition;
			}
			
			if (sender == this.hButtonSearch)
			{
				this.panelMode = UI.PanelMode.Search;
			}

			this.panelEditor.DeselectAll();
			this.UpdateButtons();
			this.UpdateEdit();
		}


		protected ProxyManager					proxyManager;
		protected Widget						left;
		protected VSplitter						splitter1;
		protected VSplitter						splitter2;
		protected MyWidgets.TextFieldExName		labelEdit;
		protected VToolBar						vToolBar;
		protected Scrollable					scrollable;
		protected UI.Panel						panelContainer;
		protected MyWidgets.PanelEditor			panelEditor;
		protected Widget						right;
		protected HToolBar						hToolBar;
		protected IconButton					hButtonDefault;
		protected IconButton					hButtonEdition;
		protected IconButton					hButtonSearch;
		protected TabBook						tabBook;

		protected TabPage						tabPageProperties;
		protected Scrollable					propertiesScrollable;

		protected TabPage						tabPageObjects;

		protected TabPage						tabPageCultures;
		protected List<IconButton>				cultureButtonList;

		protected UI.PanelMode					panelMode = UI.PanelMode.Default;
	}
}
