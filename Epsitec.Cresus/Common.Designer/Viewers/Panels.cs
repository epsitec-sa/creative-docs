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

			//	Crée le groupe central.
			this.middle = new Widget(this);
			this.middle.Dock = DockStyle.Fill;

			this.statusBar = new HToolBar(this.middle);
			this.statusBar.Dock = DockStyle.Top;

			Widget drawing = new Widget(this.middle);  // conteneur pour vToolBar et scrollable
			drawing.Dock = DockStyle.Fill;

			this.vToolBar = new VToolBar(drawing);
			this.vToolBar.Dock = DockStyle.Left;
			this.VToolBarAdd(Widgets.Command.Get("ToolSelect"));
			this.VToolBarAdd(Widgets.Command.Get("ToolGlobal"));
			//?this.VToolBarAdd(Widgets.Command.Get("ToolGrid"));
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
			this.VToolBarAdd(Widgets.Command.Get("ObjectTable"));
			this.VToolBarAdd(Widgets.Command.Get("ObjectGroup"));
			this.VToolBarAdd(Widgets.Command.Get("ObjectGroupFrame"));
			this.VToolBarAdd(Widgets.Command.Get("ObjectGroupBox"));
			this.VToolBarAdd(Widgets.Command.Get("ObjectPanel"));

			this.scrollable = new Scrollable(drawing);
			this.scrollable.MinWidth = 100;
			this.scrollable.MinHeight = 100;
			this.scrollable.Margins = new Margins(1, 1, 1, 1);
			this.scrollable.Dock = DockStyle.Fill;
			this.scrollable.HorizontalScrollerMode = ScrollableScrollerMode.ShowAlways;
			this.scrollable.VerticalScrollerMode = ScrollableScrollerMode.ShowAlways;
			this.scrollable.Panel.IsAutoFitting = true;
			this.scrollable.PaintForegroundFrame = true;
			//?this.scrollable.ForegroundFrameMargins = new Margins(0, 1, 0, 1);

			Widget container = new Widget(this.scrollable.Panel);
			container.MinWidth = 100;
			container.Dock = DockStyle.Fill;

			//	Sous-conteneur qui a des marges, pour permettre de voir les cotes (Dimension*)
			//	du PanelEditor qui s'affiche par-dessus.
			this.panelContainerParent = new Widget(container);
			this.panelContainerParent.Margins = new Margins(Dimension.margin, Dimension.margin, Dimension.margin, Dimension.margin);
			this.panelContainerParent.Dock = DockStyle.Fill;

			//	Le UI.Panel est dans le sous-contenur qui a des marges.
			this.panelContainer = this.access.CreateEmptyPanel();
			this.panelContainer.SetParent(this.panelContainerParent);

			//	Le PanelEditor est par-dessus le UI.Panel. Il occupe toute la surface (il déborde
			//	donc des marges) et tient compte lui-même du décalage. C'est le seul moyen pour
			//	pouvoir dessiner dans les marges ET y détecter les événements souris.
			this.panelEditor = new MyWidgets.PanelEditor(container);
			this.panelEditor.Initialize(this.module, this.context, this.panelContainer);
			this.panelEditor.MinWidth = 100;
			this.panelEditor.MinHeight = 100;
			this.panelEditor.Anchor = AnchorStyles.All;
			this.panelEditor.ChildrenAdded += new EventHandler(this.HandlePanelEditorChildrenAdded);
			this.panelEditor.ChildrenSelected += new EventHandler(this.HandlePanelEditorChildrenSelected);
			this.panelEditor.ChildrenGeometryChanged += new EventHandler(this.HandlePanelEditorChildrenGeometryChanged);
			this.panelEditor.UpdateCommands += new EventHandler(this.HandlePanelEditorUpdateCommands);

			//	Crée le groupe droite.
			this.right = new Widget(this);
			this.right.MinWidth = 150;
			this.right.PreferredWidth = 240;
			this.right.MaxWidth = 400;
			this.right.Dock = DockStyle.Right;

			//	Crée la toolbar horizontale, au dessus des onglets.
			this.hToolBar = new HToolBar(this.right);
			this.hToolBar.Margins = new Margins(0, 0, 0, 5);
			this.hToolBar.Dock = DockStyle.Top;
			this.hButtonDefault = this.HToolBarAdd(Res.Captions.PanelMode.Default.Id);
			this.hButtonEdition = this.HToolBarAdd(Res.Captions.PanelMode.Edition.Id);
			this.hButtonSearch  = this.HToolBarAdd(Res.Captions.PanelMode.Search.Id);
			//?this.hToolBar.Items.Add(new IconSeparator());
			//?this.hButtonType    = this.HToolBarAdd(Res.Captions.PanelMode.Type.Id);

			this.staticType = new StaticText();
			this.staticType.ContentAlignment = ContentAlignment.MiddleCenter;
			this.staticType.Dock = DockStyle.Fill;
			this.staticType.Margins = new Margins(8, 0, 0, 0);
			this.hToolBar.Items.Add(this.staticType);

			//	Crée le tabbook pour les onglets.
			this.tabBook = new TabBook(this.right);
			this.tabBook.Arrows = TabBookArrows.Stretch;
			this.tabBook.Dock = DockStyle.Fill;
			this.tabBook.Padding = new Margins(1, 1, 1, 1);
			this.tabBook.ActivePageChanged += new EventHandler(this.HandleTabBookActivePageChanged);

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
			this.propertiesScrollable.PaintForegroundFrame = true;
			//?this.propertiesScrollable.ForegroundFrameMargins = new Margins(0, 1, 0, 0);

			//	Crée l'onglet 'objets'.
			this.tabPageObjects = new TabPage();
			this.tabPageObjects.TabTitle = Res.Strings.Viewers.Panels.TabObjects;
			this.tabPageObjects.Padding = new Margins(4, 4, 4, 4);
			this.tabBook.Items.Add(this.tabPageObjects);

			Widget header = new Widget(this.tabPageObjects);
			header.PreferredHeight = 14;
			header.Dock = DockStyle.Top;
			header.Margins = new Margins(0, 0, 4, 8);

			this.objectsSlider = new HSlider(header);
			this.objectsSlider.PreferredWidth = 100;
			this.objectsSlider.Dock = DockStyle.Left;
			this.objectsSlider.TabIndex = tabIndex++;
			this.objectsSlider.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			this.objectsSlider.MinValue = 15.0M;
			this.objectsSlider.MaxValue = 60.0M;
			this.objectsSlider.SmallChange = 5.0M;
			this.objectsSlider.LargeChange = 10.0M;
			this.objectsSlider.Resolution = 1.0M;
			this.objectsSlider.Value = (decimal) Panels.treeBranchesHeight;
			this.objectsSlider.ValueChanged += new EventHandler(this.HandleObjectsSliderChanged);

			this.objectsScrollable = new Scrollable(this.tabPageObjects);
			this.objectsScrollable.Dock = DockStyle.Fill;
			this.objectsScrollable.HorizontalScrollerMode = ScrollableScrollerMode.ShowAlways;
			this.objectsScrollable.VerticalScrollerMode = ScrollableScrollerMode.ShowAlways;
			this.objectsScrollable.Panel.IsAutoFitting = true;
			this.objectsScrollable.Panel.Padding = new Margins(6, 6, 6, 6);
			this.objectsScrollable.PaintForegroundFrame = true;

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
			this.UpdateType();
			this.UpdateButtons();
			this.UpdateViewer(MyWidgets.PanelEditor.Changing.Show);
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

				this.objectsSlider.ValueChanged -= new EventHandler(this.HandleObjectsSliderChanged);

				this.tabBook.ActivePageChanged -= new EventHandler(this.HandleTabBookActivePageChanged);

				this.StatusBarDispose();
				this.TreeDispose();
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


		protected override bool IsDeleteOrDuplicateForViewer
		{
			//	Indique s'il faut aiguiller ici une opération delete ou duplicate.
			get
			{
				return (this.panelEditor.SelectedObjects.Count != 0);
			}
		}

		protected override void PrepareForDelete()
		{
			//	Préparation en vue de la suppression de l'interface.
			this.panelEditor.PrepareForDelete();
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
				this.panelContainer = this.access.GetPanel(sel).GetPanel(this.panelMode);
				this.panelContainer.SetParent(this.panelContainerParent);
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
			//	Met à jour le contenu du Viewer.
			this.UpdateArray();
			this.UpdateEdit();
			this.UpdateType();
			this.UpdateCommands();
		}

		protected void UpdateButtons()
		{
			//	Met à jour les boutons de la toolbar horizontale.
			this.hButtonDefault.ActiveState = (this.panelMode == UI.PanelMode.Default) ? ActiveState.Yes : ActiveState.No;
			this.hButtonEdition.ActiveState = (this.panelMode == UI.PanelMode.Edition) ? ActiveState.Yes : ActiveState.No;
			this.hButtonSearch.ActiveState = (this.panelMode == UI.PanelMode.Search) ? ActiveState.Yes : ActiveState.No;
		}

		protected void UpdateType()
		{
			//	Met à jour le nom de la structure de données à droite du bouton 'link'.
			string text = "";

			if (this.panelContainer != null)
			{
				StructuredType type = ObjectModifier.GetStructuredType(this.panelContainer);
				if (type != null)
				{
					text = string.Concat("<font size=\"120%\"><b>", type.Caption.Name, "</b></font>");
				}
			}

			this.staticType.Text = text;
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
				button.TabNavigationMode = TabNavigationMode.ActivateOnTab;
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
			this.panelContainer.UpdateDisplayCaptions();
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


		protected Widget VToolBarAdd(Command command)
		{
			//	Ajoute une icône dans la toolbar verticale.
			if (command == null)
			{
				//	N'utilise pas un IconSeparator, afin d'éviter les confusions
				//	avec les objets HSeparator et VSeparator !
				Widget sep = new Widget();
				sep.PreferredHeight = 20;
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


		public override void UpdateViewer(MyWidgets.PanelEditor.Changing oper)
		{
			//	Met à jour le statut du visualisateur en cours, en fonction de la sélection.
			//	Met également à jour l'arbre des objets, s'il est visible.
			this.UpdateStatusViewer();

			if (this.tabPageObjects.Visibility)  // onglet 'Objets' visible ?
			{
				this.TreeUpdate(oper);
			}
		}

		#region StatusBar
		protected void UpdateStatusViewer()
		{
			//	Met à jour le statut du visualisateur en cours, en fonction de la sélection.
			this.StatusBarDispose();
			this.statusBar.Children.Clear();  // supprime tous les boutons

			List<Widget> selection = this.panelEditor.SelectedObjects;
			Widget obj;

			if (selection.Count == 0)
			{
				this.StatusBarButton(this.panelEditor.Panel, "Root", 0, Res.Strings.Viewers.Panels.StatusBar.Root);
				this.statusBar.Items.Add(new IconSeparator());
			}
			else
			{
				//	Crée une liste de tous les parents.
				this.StatusBarButton(null, "DeselectAll", 0, Res.Strings.Viewers.Panels.StatusBar.DeselectAll);

				if (ObjectModifier.GetObjectType(selection[0]) != ObjectModifier.ObjectType.MainPanel)
				{
					this.StatusBarButton(null, "SelectParent", 0, Res.Strings.Action.PanelSelectParent);
				}

				this.statusBar.Items.Add(new IconSeparator());

				List<Widget> parents = new List<Widget>();
				obj = selection[0].Parent;  // parent du premier objet sélectionné
				while (obj != null)
				{
					ObjectModifier.ObjectType type = ObjectModifier.GetObjectType(obj);
					if (type == ObjectModifier.ObjectType.Unknow)
					{
						break;
					}

					parents.Add(obj);
					obj = obj.Parent;
				}

				//	Crée la chaîne de widgets pour les parents.
				for (int i=parents.Count-1; i>=0; i--)
				{
					this.StatusBarButton(parents[i], "Parent", i+1, (i==parents.Count-1) ? Res.Strings.Viewers.Panels.StatusBar.Root : Res.Strings.Viewers.Panels.StatusBar.Parent);
					this.StatusBarArrow("Next", i, Res.Strings.Viewers.Panels.StatusBar.Next);
				}

				//	Crée la série des widgets sélectionnés.
				for (int i=0; i<selection.Count; i++)
				{
					this.StatusBarButton(selection[i], "Selected", i, Res.Strings.Viewers.Panels.StatusBar.This);
				}

				//	Crée la série des enfants.
				obj = selection[0];  // premier objet sélectionné
				if (selection.Count == 1 && obj.Children.Count > 0 && ObjectModifier.IsAbstractGroup(obj))
				{
					this.StatusBarArrow("All", 0, Res.Strings.Viewers.Panels.StatusBar.All);

					int rank = 0;
					foreach (Widget children in obj.Children)
					{
						if (rank >= 10)
						{
							this.StatusBarOverflow();
							break;
						}

						this.StatusBarButton(children, "Children", rank++, Res.Strings.Viewers.Panels.StatusBar.Children);
					}
				}
			}
		}

		protected void StatusBarButton(Widget obj, string type, int rank, string tooltip)
		{
			//	Ajoute un bouton représentant un objet.
			string name = string.Concat(type, ".Level", rank.ToString(System.Globalization.CultureInfo.InvariantCulture));
			string icon;

			if (type == "Root")
			{
				icon = "PanelSelectRoot";
			}
			else if (type == "SelectParent")
			{
				icon = "PanelSelectParent";
			}
			else if (type == "DeselectAll")
			{
				icon = "PanelDeselectAll";
			}
			else
			{
				System.Diagnostics.Debug.Assert(obj != null);
				icon = ObjectModifier.GetObjectIcon(obj);
				System.Diagnostics.Debug.Assert(!string.IsNullOrEmpty(icon));
			}

			IconButton button = new IconButton(this.statusBar);
			button.IconName = Misc.Icon(icon);
			button.Name = name;

			if (type == "Selected")
			{
				button.ButtonStyle = ButtonStyle.ActivableIcon;
				button.ActiveState = ActiveState.Yes;
			}
			else if (type == "Children")
			{
				button.ButtonStyle = ButtonStyle.ActivableIcon;
			}
			else
			{
				button.ButtonStyle = ButtonStyle.ToolItem;
			}

			button.Dock = DockStyle.Left;
			button.Clicked += new MessageEventHandler(this.HandleStatusBarButtonClicked);
			button.Entered += new MessageEventHandler(this.HandleStatusBarButtonEntered);
			button.Exited += new MessageEventHandler(this.HandleStatusBarButtonExited);
			ToolTip.Default.SetToolTip(button, tooltip);
		}

		protected void StatusBarArrow(string type, int rank, string tooltip)
		{
			//	Ajoute une flèche de séparation ">".
			string name = string.Concat(type, ".Level", rank.ToString(System.Globalization.CultureInfo.InvariantCulture));

			GlyphButton arrow = new GlyphButton(this.statusBar);
			arrow.Name = name;
			arrow.GlyphShape = (type == "All") ? GlyphShape.Plus : GlyphShape.ArrowRight;
			arrow.ButtonStyle = ButtonStyle.ToolItem;
			arrow.Dock = DockStyle.Left;
			arrow.Margins = new Margins((type == "All") ? 10:0, 0, 0, 0);
			arrow.Clicked += new MessageEventHandler(this.HandleStatusBarButtonClicked);
			arrow.Entered += new MessageEventHandler(this.HandleStatusBarButtonEntered);
			arrow.Exited += new MessageEventHandler(this.HandleStatusBarButtonExited);
			ToolTip.Default.SetToolTip(arrow, tooltip);
		}

		protected void StatusBarOverflow()
		{
			//	Ajoute un marqueur de débordement "...".
			StaticText overflow = new StaticText(this.statusBar);
			overflow.Text = "...";
			overflow.PreferredWidth = 20;
			overflow.Dock = DockStyle.Left;
		}

		protected void StatusBarDispose()
		{
			//	Supprime tous les widgets existants dans la barre de statut.
			foreach (Widget children in this.statusBar.Children)
			{
				if (children is IconButton)
				{
					AbstractButton button = children as AbstractButton;
					button.Clicked -= new MessageEventHandler(this.HandleStatusBarButtonClicked);
					button.Entered -= new MessageEventHandler(this.HandleStatusBarButtonEntered);
					button.Exited -= new MessageEventHandler(this.HandleStatusBarButtonExited);
				}
			}
		}

		protected void StatusBarEntered(string type, int rank, bool entered)
		{
			if (entered)
			{
				List<Widget> list = this.StatusBarSearch(type, rank);
				this.panelEditor.SetEnteredObjects(list);
			}
			else
			{
				this.panelEditor.SetEnteredObjects(null);
			}
		}

		protected void StatusBarSelect(string type, int rank)
		{
			//	Effectue une opération de sélection.
			List<Widget> list = this.StatusBarSearch(type, rank);
			if (list.Count == 0)
			{
				this.panelEditor.DeselectAll();
			}
			else
			{
				this.panelEditor.SelectListObject(list);
			}
		}

		protected List<Widget> StatusBarSearch(string type, int rank)
		{
			//	Cherche le widget correspondant à une opération de sélection.
			List<Widget> list = new List<Widget>();

			if (type == "Parent")
			{
				List<Widget> selection = this.panelEditor.SelectedObjects;
				Widget obj = selection[0];
				for (int i=0; i<rank; i++)
				{
					obj = obj.Parent;
				}
				list.Add(obj);
			}

			if (type == "Selected")
			{
				List<Widget> selection = this.panelEditor.SelectedObjects;
				list.Add(selection[rank]);
			}

			if (type == "Children")
			{
				List<Widget> selection = this.panelEditor.SelectedObjects;
				Widget obj = selection[0];
				list.Add(obj.Children[rank] as Widget);
			}

			if (type == "Next")
			{
				List<Widget> selection = this.panelEditor.SelectedObjects;
				Widget obj = selection[0];
				for (int i=0; i<rank; i++)
				{
					obj = obj.Parent;
				}

				int next = obj.Parent.Children.IndexOf(obj);
				next++;
				if (next >= obj.Parent.Children.Count)
				{
					next = 0;
				}
				list.Add(obj.Parent.Children[next] as Widget);
			}

			if (type == "Root")
			{
				list.Add(this.panelEditor.Panel);
			}

			if (type == "All")
			{
				List<Widget> selection = this.panelEditor.SelectedObjects;
				Widget obj = selection[0];
				foreach (Widget children in obj.Children)
				{
					list.Add(children);
				}
			}

			if (type == "SelectParent")
			{
				List<Widget> selection = this.panelEditor.SelectedObjects;
				Widget obj = selection[0];
				list.Add(obj.Parent);
			}

			return list;
		}
		#endregion


		#region Tree
		protected void TreeUpdate(MyWidgets.PanelEditor.Changing oper)
		{
			//	Construit l'arbre des objets.
			if (oper == MyWidgets.PanelEditor.Changing.Selection)
			{
				this.TreeUpdateSelection(this.objectsScrollable.Panel.Children);
			}
			else
			{
				this.TreeClear();

				List<Widget> bands = new List<Widget>();
				List<MyWidgets.TreeBranches> branches = new List<MyWidgets.TreeBranches>();
				List<double> positions = new List<double>();
				this.TreeCreateChildren(bands, branches, positions, 0, this.panelContainer);
			}
		}

		protected void TreeClear()
		{
			//	Supprime et libère l'arbre des objets.
			this.TreeDispose();
			this.objectsScrollable.Panel.Children.Clear();  // supprime tous les boutons
		}

		protected void TreeUpdateSelection(Widgets.Collections.FlatChildrenCollection childrens)
		{
			//	Met à jour les objets sélectionnés dans l'arbre.
			foreach (Widget children in childrens)
			{
				if (children is IconButton)
				{
					AbstractButton button = children as AbstractButton;
					Widget obj = button.GetValue(Panels.TreeObjectProperty) as Widget;
					bool sel = this.panelEditor.SelectedObjects.Contains(obj);
					button.ActiveState = sel ? ActiveState.Yes : ActiveState.No;
				}

				this.TreeUpdateSelection(children.Children);
			}
		}

		protected void TreeCreateChildren(List<Widget> bands, List<MyWidgets.TreeBranches> branches, List<double> positions, int deep, Widget obj)
		{
			//	Crée le bouton d'un objet, puis appelle de nouveau cette méthode récursivement
			//	pour les boutons des enfants.
			if (deep >= bands.Count)
			{
				//	Crée le conteneur horizonal pour les boutons.
				Widget band = new Widget(this.objectsScrollable.Panel);
				band.Dock = DockStyle.Top;
				bands.Add(band);

				//	Crée le conteneur horizontal pour les liaisons.
				MyWidgets.TreeBranches tb = new MyWidgets.TreeBranches(this.objectsScrollable.Panel);
				tb.PreferredHeight = Panels.treeBranchesHeight;
				tb.Dock = DockStyle.Top;
				branches.Add(tb);

				//	Crée le remplissage en X du contenur horizontal.
				positions.Add(0);
			}

			if (deep > 0)
			{
				double shift = positions[deep-1] - positions[deep] - Panels.treeButtonWidth;
				if (shift > 0)  // parent en retrait à gauche ?
				{
					//	Crée un séparateur pour aligner le premier enfant sur le parent.
					Widget sep = new Widget(bands[deep]);
					sep.PreferredWidth = shift;
					sep.Dock = DockStyle.Left;
					positions[deep] += shift;
				}
			}

			//	Crée le bouton pour l'objet.
			this.TreeCreateButton(obj, bands[deep]);
			positions[deep] += Panels.treeButtonWidth;

			if (ObjectModifier.IsAbstractGroup(obj))  // conteneur d'autres objets ?
			{
				for (int i=0; i<obj.Children.Count; i++)
				{
					Widget children = obj.Children[i] as Widget;
					this.TreeCreateChildren(bands, branches, positions, deep+1, children);

					branches[deep].AddBranche(positions[deep]-Panels.treeButtonWidth/2, positions[deep+1]-Panels.treeButtonWidth/2);
				}

				if (obj.Children.Count > 0)
				{
					//	Crée le séparateur entre des enfants de parents différents.
					Widget sep = new Widget(bands[deep+1]);
					sep.PreferredWidth = 8;
					sep.Dock = DockStyle.Left;
					positions[deep+1] += sep.PreferredWidth;
				}
			}
		}

		protected void TreeCreateButton(Widget obj, Widget parent)
		{
			//	Ajoute un bouton représentant un objet.
			string icon = ObjectModifier.GetObjectIcon(obj);
			System.Diagnostics.Debug.Assert(icon != null);

			bool sel = this.panelEditor.SelectedObjects.Contains(obj);

			IconButton button = new IconButton(parent);
			button.IconName = Misc.Icon(icon);
			button.ButtonStyle = ButtonStyle.ActivableIcon;
			button.ActiveState = sel ? ActiveState.Yes : ActiveState.No;
			button.PreferredWidth = Panels.treeButtonWidth;
			button.Dock = DockStyle.Left;
			button.Clicked += new MessageEventHandler(this.HandleTreeButtonClicked);
			button.Entered += new MessageEventHandler(this.HandleTreeButtonEntered);
			button.Exited += new MessageEventHandler(this.HandleTreeButtonExited);
			button.SetValue(Panels.TreeObjectProperty, obj);
		}

		private void HandleTreeButtonClicked(object sender, MessageEventArgs e)
		{
			//	Le bouton d'un objet dans l'arbre a été cliqué.
			IconButton button = sender as IconButton;
			Widget obj = button.GetValue(Panels.TreeObjectProperty) as Widget;
			this.panelEditor.SelectOneObject(obj);
		}

		private void HandleTreeButtonEntered(object sender, MessageEventArgs e)
		{
			//	La souris est entrée (survol) dans un bouton d'un objet de l'arbre.
			IconButton button = sender as IconButton;
			Widget obj = button.GetValue(Panels.TreeObjectProperty) as Widget;
			List<Widget> list = new List<Widget>();
			list.Add(obj);
			this.panelEditor.SetEnteredObjects(list);
		}

		private void HandleTreeButtonExited(object sender, MessageEventArgs e)
		{
			//	La souris est sortie (survol) d'un bouton d'un objet de l'arbre.
			this.panelEditor.SetEnteredObjects(null);
		}

		private void HandleObjectsSliderChanged(object sender)
		{
			HSlider slider = sender as HSlider;
			Panels.treeBranchesHeight = (double) slider.Value;

			foreach (Widget widget in this.objectsScrollable.Panel.Children)
			{
				if (widget is MyWidgets.TreeBranches)
				{
					widget.PreferredHeight = Panels.treeBranchesHeight;
				}
			}
		}

		protected void TreeDispose()
		{
			//	Libère tout l'arbre des objets.
			this.TreeDispose(this.objectsScrollable.Panel.Children);
		}

		protected void TreeDispose(Widgets.Collections.FlatChildrenCollection childrens)
		{
			foreach (Widget children in childrens)
			{
				if (children is IconButton)
				{
					AbstractButton button = children as AbstractButton;
					button.Clicked -= new MessageEventHandler(this.HandleTreeButtonClicked);
					button.Entered -= new MessageEventHandler(this.HandleTreeButtonEntered);
					button.Exited -= new MessageEventHandler(this.HandleTreeButtonExited);
					button.ClearValue(Panels.TreeObjectProperty);
				}

				this.TreeDispose(children.Children);
			}
		}

		protected static readonly DependencyProperty TreeObjectProperty = DependencyProperty.RegisterAttached("TreeObject", typeof(Widget), typeof(Panels), new DependencyPropertyMetadata().MakeNotSerializable());
		#endregion


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

		public void RegenerateDimensions()
		{
			//	Régénère les cotes s'il y a eu un changement.
			this.panelEditor.RegenerateDimensions();
		}

		public void RegenerateProxiesAndDimensions()
		{
			//	Régénère les proxies et les cotes.
			if (this.proxyManager.RegenerateProxies())
			{
				this.ClearProxies();
				this.proxyManager.CreateUserInterface(this.propertiesScrollable.Panel);
			}

			this.panelEditor.RegenerateDimensions();
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
			this.UpdateType();
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

		private void HandleStatusBarButtonClicked(object sender, MessageEventArgs e)
		{
			AbstractButton button = sender as AbstractButton;
			int p = button.Name.IndexOf(".Level");
			int i = System.Int32.Parse(button.Name.Substring(p+6));
			string type = button.Name.Substring(0, p);
			this.StatusBarSelect(type, i);
		}

		private void HandleStatusBarButtonEntered(object sender, MessageEventArgs e)
		{
			AbstractButton button = sender as AbstractButton;
			int p = button.Name.IndexOf(".Level");
			int i = System.Int32.Parse(button.Name.Substring(p+6));
			string type = button.Name.Substring(0, p);
			this.StatusBarEntered(type, i, true);
		}

		private void HandleStatusBarButtonExited(object sender, MessageEventArgs e)
		{
			AbstractButton button = sender as AbstractButton;
			int p = button.Name.IndexOf(".Level");
			int i = System.Int32.Parse(button.Name.Substring(p+6));
			string type = button.Name.Substring(0, p);
			this.StatusBarEntered(type, i, false);
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

			if (sender == this.hButtonType)
			{
				Druid druid = Druid.Empty;

				StructuredType type = ObjectModifier.GetStructuredType(this.panelContainer);
				if (type != null)
				{
					druid = type.CaptionId;
				}

				//	Choix d'une ressource type de type 'Types', mais uniquement parmi les TypeCode.Structured.
				druid = this.mainWindow.DlgResourceSelector(this.module, ResourceAccess.Type.Types, TypeCode.Structured, druid, null, null);
				if (!druid.IsEmpty)  // d'accord ?
				{
					AbstractType at = this.module.AccessCaptions.DirectGetAbstractType(druid);
					System.Diagnostics.Debug.Assert(at is StructuredType);
					type = at as StructuredType;
					ObjectModifier.SetStructuredType(this.panelContainer, type);

					this.UpdateType();
				}

				return;
			}

			this.panelEditor.DeselectAll();
			this.UpdateButtons();
			this.UpdateEdit();
		}

		private void HandleTabBookActivePageChanged(object sender)
		{
			//	Changement de l'onglet visible.
			if (this.tabPageObjects.Visibility)  // arbre des objets visible ?
			{
				this.TreeUpdate(MyWidgets.PanelEditor.Changing.Show);  // met à jour l'arbre
			}
			else
			{
				this.TreeClear();  // supprime l'arbre
			}
		}


		protected static readonly double		treeButtonWidth = 22;
		protected static double					treeBranchesHeight = 30;

		protected ProxyManager					proxyManager;
		protected Widget						left;
		protected VSplitter						splitter1;
		protected VSplitter						splitter2;
		protected MyWidgets.TextFieldExName		labelEdit;
		protected Widget						middle;
		protected VToolBar						vToolBar;
		protected HToolBar						statusBar;
		protected Scrollable					scrollable;
		protected Widget						panelContainerParent;
		protected UI.Panel						panelContainer;
		protected MyWidgets.PanelEditor			panelEditor;
		protected Widget						right;
		protected HToolBar						hToolBar;
		protected IconButton					hButtonDefault;
		protected IconButton					hButtonEdition;
		protected IconButton					hButtonSearch;
		protected IconButton					hButtonType;
		protected StaticText					staticType;
		protected TabBook						tabBook;

		protected TabPage						tabPageProperties;
		protected Scrollable					propertiesScrollable;

		protected TabPage						tabPageObjects;
		protected HSlider						objectsSlider;
		protected Scrollable					objectsScrollable;

		protected TabPage						tabPageCultures;
		protected List<IconButton>				cultureButtonList;

		protected UI.PanelMode					panelMode = UI.PanelMode.Default;
	}
}
