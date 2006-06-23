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
		public Panels(Module module, PanelsContext context) : base(module, context)
		{
			int tabIndex = 0;

			Widget left = new Widget(this);
			left.MinWidth = 80;
			left.MaxWidth = 400;
			left.PreferredWidth = 200;
			left.Dock = DockStyle.Left;
			left.Padding = new Margins(10, 10, 10, 10);

			this.labelEdit = new TextFieldEx(left);
			this.labelEdit.Name = "LabelEdit";
			this.labelEdit.Margins = new Margins(0, 0, 10, 0);
			this.labelEdit.Dock = DockStyle.Bottom;
			this.labelEdit.EditionAccepted += new EventHandler(this.HandleTextChanged);
			this.labelEdit.KeyboardFocusChanged += new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);
			this.labelEdit.TabIndex = tabIndex++;
			this.labelEdit.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.labelEdit.Visibility = (this.module.Mode == DesignerMode.Build);

			this.array = new MyWidgets.StringArray(left);
			this.array.Columns = 1;
			this.array.SetColumnsRelativeWidth(0, 1.00);
			this.array.SetDynamicsToolTips(0, true);
			this.array.Margins = new Margins(0, 0, 0, 0);
			this.array.Dock = DockStyle.Fill;
			this.array.CellCountChanged += new EventHandler (this.HandleArrayCellCountChanged);
			this.array.SelectedRowChanged += new EventHandler(this.HandleArraySelectedRowChanged);
			this.array.TabIndex = tabIndex++;
			this.array.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			VSplitter splitter1 = new VSplitter(this);
			splitter1.Dock = DockStyle.Left;
			VSplitter.SetAutoCollapseEnable(left, true);

			this.toolBar = new VToolBar(this);
			this.toolBar.Margins = new Margins(0, 0, 0, 0);
			this.toolBar.Dock = DockStyle.Left;
			this.ToolBarAdd(Widgets.Command.Get("ToolSelect"));
			this.ToolBarAdd(Widgets.Command.Get("ToolGlobal"));
			this.ToolBarAdd(Widgets.Command.Get("ToolEdit"));
			//?this.ToolBarAdd(Widgets.Command.Get("ToolZoom"));
			//?this.ToolBarAdd(Widgets.Command.Get("ToolHand"));
			this.ToolBarAdd(null);
			this.ToolBarAdd(Widgets.Command.Get("ObjectVLine"));
			this.ToolBarAdd(Widgets.Command.Get("ObjectHLine"));
			this.ToolBarAdd(Widgets.Command.Get("ObjectStatic"));
			this.ToolBarAdd(Widgets.Command.Get("ObjectButton"));
			this.ToolBarAdd(Widgets.Command.Get("ObjectText"));
			this.ToolBarAdd(Widgets.Command.Get("ObjectGroup"));

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

			this.panelContainer = this.CreateEmptyPanel();
			this.panelContainer.SetParent(container);

			//	Le PanelEditor est par-dessus le UI.Panel.
			this.panelEditor = new MyWidgets.PanelEditor(container);
			this.panelEditor.Initialise(this.module, this.context, this.panelContainer);
			this.panelEditor.MinWidth = 100;
			this.panelEditor.MinHeight = 100;
			this.panelEditor.Anchor = AnchorStyles.All;
			this.panelEditor.ChildrenAdded += new EventHandler(this.HandlePanelEditorChildrenAdded);
			this.panelEditor.ChildrenSelected += new EventHandler(this.HandlePanelEditorChildrenSelected);
			this.panelEditor.ChildrenGeometryChanged += new EventHandler(this.HandlePanelEditorChildrenGeometryChanged);
			this.panelEditor.UpdateCommands += new EventHandler(this.HandlePanelEditorUpdateCommands);

			this.tabBook = new TabBook(this);
			this.tabBook.MinWidth = 150;
			this.tabBook.PreferredWidth = 260;
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

			VSplitter splitter2 = new VSplitter(this);
			splitter2.Dock = DockStyle.Right;

			this.module.PanelsRead();

			this.UpdateDruidsIndex("", Searcher.SearchingMode.None);
			this.UpdateArray();
			this.UpdateEdit();
		}

		private UI.Panel CreateEmptyPanel()
		{
			UI.Panel panel;
			
			panel = new UI.Panel ();
			panel.ChildrenLayoutMode = Widgets.Layouts.LayoutMode.Anchored;
			//?panel.ChildrenLayoutMode = Widgets.Layouts.LayoutMode.Docked;
			//?panel.ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow;
			panel.ContainerLayoutMode = ContainerLayoutMode.VerticalFlow;
			panel.PreferredSize = new Size (200, 200);
			panel.Anchor = AnchorStyles.BottomLeft;
			panel.Padding = new Margins (20, 20, 20, 20);
			
			return panel;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.array.CellCountChanged -= new EventHandler (this.HandleArrayCellCountChanged);
				this.array.SelectedRowChanged -= new EventHandler(this.HandleArraySelectedRowChanged);

				this.labelEdit.EditionAccepted -= new EventHandler(this.HandleTextChanged);
				this.labelEdit.KeyboardFocusChanged -= new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);
			}

			base.Dispose(disposing);
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


		public override void DoSearch(string search, Searcher.SearchingMode mode)
		{
			//	Effectue une recherche.
		}

		public override void DoCount(string search, Searcher.SearchingMode mode)
		{
			//	Effectue un comptage.
		}

		public override void DoReplace(string search, string replace, Searcher.SearchingMode mode)
		{
			//	Effectue un remplacement.
		}

		public override void DoReplaceAll(string search, string replace, Searcher.SearchingMode mode)
		{
			//	Effectue un 'remplacer tout'.
		}

		public override void DoModification(string name)
		{
			//	Change la ressource modifiée visible.
		}

		public override void DoDelete()
		{
			//	Supprime la ressource sélectionnée.
			int sel = this.array.SelectedRow;
			if ( sel == -1 )  return;

			Druid druid = this.druidsIndex[sel];
			this.module.PanelDelete(druid);

			this.druidsIndex.RemoveAt(sel);
			this.UpdateArray();

			sel = System.Math.Min(sel, this.druidsIndex.Count-1);
			this.array.SelectedRow = sel;
			this.array.ShowSelectedRow();
			this.UpdateCommands();
			this.module.Modifier.IsDirty = true;
		}

		public override void DoDuplicate(bool duplicate)
		{
			//	Duplique la ressource sélectionnée.
			int sel = this.array.SelectedRow;
			if ( sel == -1 )  return;
			int newSel = sel+1;

			Druid druid = this.druidsIndex[sel];
			int index = this.module.PanelIndex(druid);
			string newName = this.GetDuplicateName(this.module.PanelName(index));
			Druid newDruid = this.module.PanelCreate(newName, index+1);

			this.druidsIndex.Insert(newSel, newDruid);
			this.UpdateArray();

			this.array.SelectedRow = newSel;
			this.array.ShowSelectedRow();
			this.UpdateCommands();
			this.module.Modifier.IsDirty = true;
		}

		public override void DoMove(int direction)
		{
			//	Déplace la ressource sélectionnée.
			int sel = this.array.SelectedRow;
			if ( sel == -1 )  return;

			int newSel = sel+direction;
			System.Diagnostics.Debug.Assert(newSel >= 0 && newSel < this.druidsIndex.Count);

			Druid druid1 = this.druidsIndex[sel];
			Druid druid2 = this.druidsIndex[newSel];

			this.module.PanelMove(druid1, this.module.PanelIndex(druid2));

			this.druidsIndex.RemoveAt(sel);
			this.druidsIndex.Insert(newSel, druid1);
			this.UpdateArray();

			this.array.SelectedRow = newSel;
			this.array.ShowSelectedRow();
			this.UpdateCommands();
			this.module.Modifier.IsDirty = true;
		}

		public override void DoNewCulture()
		{
			//	Crée une nouvelle culture.
		}

		public override void DoDeleteCulture()
		{
			//	Supprime la culture courante.
		}

		public override void DoClipboard(string name)
		{
			//	Effectue une action avec le bloc-notes.
		}

		public override void DoFont(string name)
		{
			//	Effectue une modification de typographie.
		}

		public override void DoTool(string name)
		{
			//	Choix de l'outil.
			base.DoTool(name);

			if (this.context.Tool == "ToolSelect" || this.context.Tool == "ToolGlobal")
			{
				this.panelEditor.SelectLastCreatedObject();
			}

			if (this.context.Tool.StartsWith("Object"))
			{
				this.panelEditor.DeselectAll();
			}

			this.panelEditor.SizeMarkDeselect();
			this.panelEditor.Invalidate();
		}

		public override void DoCommand(string name)
		{
			//	Exécute une commande.
			if (name == "PanelRun")
			{
				this.module.RunPanel(this.array.SelectedRow);
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


		protected override void UpdateDruidsIndex(string filter, Searcher.SearchingMode mode)
		{
			//	Construit l'index en fonction des ressources primaires.
			this.druidsIndex.Clear();

			if ((mode&Searcher.SearchingMode.CaseSensitive) == 0)
			{
				filter = Searcher.RemoveAccent(filter.ToLower());
			}

			Regex regex = null;
			if ((mode&Searcher.SearchingMode.Jocker) != 0)
			{
				regex = RegexFactory.FromSimpleJoker(filter, RegexFactory.Options.None);
			}

			for (int i=0; i<this.module.PanelsCount; i++)
			{
				string name = this.module.PanelName(i);

				if (filter != "")
				{
					if ((mode&Searcher.SearchingMode.Jocker) != 0)
					{
						string text = name;
						if ((mode&Searcher.SearchingMode.CaseSensitive) == 0)
						{
							text = Searcher.RemoveAccent(text.ToLower());
						}
						if (!regex.IsMatch(text))  continue;
					}
					else
					{
						int index = Searcher.IndexOf(name, filter, 0, mode);
						if (index == -1)  continue;
						if ((mode&Searcher.SearchingMode.AtBeginning) != 0 && index != 0)  continue;
					}
				}

				this.druidsIndex.Add(this.module.PanelDruid(i));
			}
		}

		protected override void UpdateArray()
		{
			//	Met à jour tout le contenu du tableau.
			this.array.TotalRows = this.druidsIndex.Count;

			int first = this.array.FirstVisibleRow;
			for (int i=0; i<this.array.LineCount; i++)
			{
				if (first+i < this.druidsIndex.Count)
				{
					Druid druid = this.druidsIndex[first+i];
					int index = this.module.PanelIndex(druid);

					this.array.SetLineString(0, first+i, this.module.PanelName(index));
					this.array.SetLineState(0, first+i, MyWidgets.StringList.CellState.Normal);
				}
				else
				{
					this.array.SetLineString(0, first+i, "");
					this.array.SetLineState(0, first+i, MyWidgets.StringList.CellState.Disabled);
				}
			}
		}

		protected void UpdateEdit()
		{
			//	Met à jour les lignes éditables en fonction de la sélection dans le tableau.
			bool iic = this.ignoreChange;
			this.ignoreChange = true;

			int sel = this.array.SelectedRow;

			if (sel >= this.druidsIndex.Count)
			{
				sel = -1;
			}

			if (this.panelContainer != null)
			{
				this.panelContainer.SetParent (null);
				this.panelContainer = null;
			}
			
			if (sel == -1)
			{
				this.labelEdit.Enable = false;
				this.labelEdit.Text = "";
			}
			else
			{
				Druid druid = this.druidsIndex[sel];
				int index = this.module.PanelIndex(druid);
				string label = this.module.PanelName(index);
				ResourceBundle bundle = this.module.PanelBundle(index);
				UI.Panel newPanel = Panels.GetPanel(bundle);

				if (newPanel == null)
				{
					newPanel = this.CreateEmptyPanel ();
					Panels.SetPanel(bundle, newPanel);
				}

				this.panelContainer = newPanel;
				this.panelContainer.SetParent(this.panelEditor.Parent);
				this.panelContainer.ZOrder = this.panelEditor.ZOrder+1;
				this.panelEditor.Panel = this.panelContainer;
				
				this.labelEdit.Enable = true;
				this.labelEdit.Text = label;
				this.labelEdit.Focus();
				this.labelEdit.SelectAll();
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

		public override void UpdateCommands()
		{
			//	Met à jour les commandes en fonction de la ressource sélectionnée.
			base.UpdateCommands();

			int sel = this.array.SelectedRow;
			int count = this.druidsIndex.Count;
			bool build = (this.module.Mode == DesignerMode.Build);

			this.GetCommandState("NewCulture").Enable = false;
			this.GetCommandState("DeleteCulture").Enable = false;

			this.GetCommandState("Search").Enable = false;
			this.GetCommandState("SearchPrev").Enable = false;
			this.GetCommandState("SearchNext").Enable = false;

			this.GetCommandState("ModificationPrev").Enable = false;
			this.GetCommandState("ModificationNext").Enable = false;
			this.GetCommandState("ModificationAll").Enable = false;
			this.GetCommandState("ModificationClear").Enable = false;

			this.GetCommandState("FontBold").Enable = false;
			this.GetCommandState("FontItalic").Enable = false;
			this.GetCommandState("FontUnderlined").Enable = false;
			this.GetCommandState("Glyphs").Enable = false;

			int objSelected, objCount;
			this.panelEditor.GetSelectionInfo(out objSelected, out objCount);

			this.GetCommandState("PanelDelete").Enable = (objSelected != 0);
			this.GetCommandState("PanelDuplicate").Enable = (objSelected != 0);
			this.GetCommandState("PanelDeselectAll").Enable = (objSelected != 0);
			this.GetCommandState("PanelSelectAll").Enable = (objSelected < objCount);
			this.GetCommandState("PanelSelectInvert").Enable = (objCount > 0);

			this.GetCommandState("PanelShowGrid").Enable = true;
			this.GetCommandState("PanelShowConstrain").Enable = true;
			this.GetCommandState("PanelShowAttachment").Enable = true;
			this.GetCommandState("PanelShowExpand").Enable = true;
			this.GetCommandState("PanelShowZOrder").Enable = true;
			this.GetCommandState("PanelShowTabIndex").Enable = true;
			this.GetCommandState("PanelRun").Enable = (sel != -1);
			this.GetCommandState("PanelShowGrid").ActiveState = this.context.ShowGrid ? ActiveState.Yes : ActiveState.No;
			this.GetCommandState("PanelShowConstrain").ActiveState = this.context.ShowConstrain ? ActiveState.Yes : ActiveState.No;
			this.GetCommandState("PanelShowAttachment").ActiveState = this.context.ShowAttachment ? ActiveState.Yes : ActiveState.No;
			this.GetCommandState("PanelShowExpand").ActiveState = this.context.ShowExpand ? ActiveState.Yes : ActiveState.No;
			this.GetCommandState("PanelShowZOrder").ActiveState = this.context.ShowZOrder ? ActiveState.Yes : ActiveState.No;
			this.GetCommandState("PanelShowTabIndex").ActiveState = this.context.ShowTabIndex ? ActiveState.Yes : ActiveState.No;

			this.GetCommandState("MoveLeft").Enable = (objSelected != 0);
			this.GetCommandState("MoveRight").Enable = (objSelected != 0);
			this.GetCommandState("MoveDown").Enable = (objSelected != 0);
			this.GetCommandState("MoveUp").Enable = (objSelected != 0);

			this.GetCommandState("AlignLeft").Enable = (objSelected >= 2);
			this.GetCommandState("AlignCenterX").Enable = (objSelected >= 2);
			this.GetCommandState("AlignRight").Enable = (objSelected >= 2);
			this.GetCommandState("AlignTop").Enable = (objSelected >= 2);
			this.GetCommandState("AlignCenterY").Enable = (objSelected >= 2);
			this.GetCommandState("AlignBottom").Enable = (objSelected >= 2);
			this.GetCommandState("AlignBaseLine").Enable = (objSelected >= 2);
			this.GetCommandState("AdjustWidth").Enable = (objSelected >= 2);
			this.GetCommandState("AdjustHeight").Enable = (objSelected >= 2);
			this.GetCommandState("AlignGrid").Enable = (objSelected != 0);

			this.GetCommandState("OrderUpAll").Enable = (objSelected != 0 && objCount >= 2);
			this.GetCommandState("OrderDownAll").Enable = (objSelected != 0 && objCount >= 2);
			this.GetCommandState("OrderUpOne").Enable = (objSelected != 0 && objCount >= 2);
			this.GetCommandState("OrderDownOne").Enable = (objSelected != 0 && objCount >= 2);

			this.GetCommandState("TabIndexClear").Enable = (objSelected != 0);
			this.GetCommandState("TabIndexRenum").Enable = (objCount != 0);
			this.GetCommandState("TabIndexLast").Enable = (objSelected != 0);
			this.GetCommandState("TabIndexPrev").Enable = (objSelected != 0);
			this.GetCommandState("TabIndexNext").Enable = (objSelected != 0);
			this.GetCommandState("TabIndexFirst").Enable = (objSelected != 0);

			this.module.MainWindow.UpdateInfoCurrentModule();
			this.module.MainWindow.UpdateInfoAccess();
			this.module.MainWindow.UpdateInfoViewer();
		}


		public override string InfoAccessText
		{
			//	Donne le texte d'information sur l'accès en cours.
			get
			{
				System.Text.StringBuilder builder = new System.Text.StringBuilder();

				int sel = this.array.SelectedRow;
				if (sel == -1)
				{
					builder.Append("-");
				}
				else
				{
					int index = this.module.PanelIndex(this.druidsIndex[sel]);
					builder.Append(this.module.PanelName(index));
					builder.Append(": ");
					builder.Append((sel+1).ToString());
				}

				builder.Append("/");
				builder.Append(this.druidsIndex.Count.ToString());

				if (this.druidsIndex.Count < this.InfoAccessTotalCount)
				{
					builder.Append(" (");
					builder.Append(this.InfoAccessTotalCount.ToString());
					builder.Append(")");
				}

				return builder.ToString();
			}
		}

		protected override int InfoAccessTotalCount
		{
			get
			{
				return this.module.PanelsCount;
			}
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
				button.Pressed += new MessageEventHandler(this.HandleCultureButtonPressed);
				ToolTip.Default.SetToolTip(button, Misc.CultureLongName(culture));

				this.cultureButtonList.Add(button);
			}

			this.UpdateCultureButtons();
		}

		void HandleCultureButtonPressed(object sender, MessageEventArgs e)
		{
			//	Un bouton pour changer de culture a été cliqué.
			IconButton button = sender as IconButton;
			this.module.ResourceManager.ActiveCulture = Resources.FindSpecificCultureInfo(button.Name);
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
				IconButton button = new IconButton(command.Name, Misc.Icon(command.IconName), command.Name);
				this.toolBar.Items.Add(button);
				ToolTip.Default.SetToolTip(button, Misc.GetTextWithShortcut(command));
				return button;
			}
		}


		protected string GetDuplicateName(string baseName)
		{
			//	Retourne le nom à utiliser lorsqu'un nom existant est dupliqué.
			int numberLength = 0;
			while (baseName.Length > 0)
			{
				char last = baseName[baseName.Length-1-numberLength];
				if (last >= '0' && last <= '9')
				{
					numberLength ++;
				}
				else
				{
					break;
				}
			}

			int nextNumber = 2;
			if (numberLength > 0)
			{
				nextNumber = int.Parse(baseName.Substring(baseName.Length-numberLength))+1;
				baseName = baseName.Substring(0, baseName.Length-numberLength);
			}

			string newName = baseName;
			for (int i=nextNumber; i<nextNumber+100; i++)
			{
				newName = string.Concat(baseName, i.ToString(System.Globalization.CultureInfo.InvariantCulture));
				if ( !this.IsExistingName(newName) )  break;
			}

			return newName;
		}

		protected bool IsExistingName(string baseName)
		{
			//	Indique si un nom existe.
			int total = this.module.PanelsCount;
			for (int i=0; i<total; i++)
			{
				if (baseName == this.module.PanelName(i))
				{
					return true;
				}
			}
			return false;
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
			this.propertiesScrollable.Panel.Children.Clear();
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


		void HandleArrayCellCountChanged(object sender)
		{
			//	Le nombre de lignes a changé.
			this.UpdateArray();
		}

		void HandleArraySelectedRowChanged(object sender)
		{
			//	La ligne sélectionnée a changé.
			this.UpdateEdit();
			this.UpdateCommands();
			this.panelEditor.IsEditEnabled = (this.array.SelectedRow != -1);
			this.DefineProxies(this.panelEditor.SelectedObjects);
		}

		void HandleTextChanged(object sender)
		{
			//	Un texte éditable a changé.
			if ( this.ignoreChange )  return;

			AbstractTextField edit = sender as AbstractTextField;
			string newName = edit.Text;
			int sel = this.array.SelectedRow;
			Druid druid = this.druidsIndex[sel];

			this.module.PanelRename(druid, newName);

			this.UpdateArray();

			this.module.Modifier.IsDirty = true;
		}

		void HandlePanelEditorChildrenAdded(object sender)
		{
			this.UpdateCommands();
		}

		void HandlePanelEditorChildrenSelected(object sender)
		{
			this.UpdateCommands();
			this.DefineProxies(this.panelEditor.SelectedObjects);
		}

		void HandlePanelEditorChildrenGeometryChanged(object sender)
		{
			this.UpdateProxies();
		}

		void HandlePanelEditorUpdateCommands(object sender)
		{
			this.UpdateCommands();
		}


		public static void SetPanel(DependencyObject obj, UI.Panel panel)
		{
			obj.SetValue (Panels.PanelProperty, panel);
		}

		public static UI.Panel GetPanel(DependencyObject obj)
		{
			return (UI.Panel) obj.GetValue(Panels.PanelProperty);
		}
		
		public static readonly DependencyProperty PanelProperty = DependencyProperty.RegisterAttached("Panel", typeof(UI.Panel), typeof(Panels));


		protected ProxyManager				proxyManager;
		protected TextFieldEx				labelEdit;
		protected VToolBar					toolBar;
		protected Scrollable				scrollable;
		protected UI.Panel					panelContainer;
		protected MyWidgets.PanelEditor		panelEditor;
		protected TabBook					tabBook;

		protected TabPage					tabPageProperties;
		protected Scrollable				propertiesScrollable;

		protected TabPage					tabPageObjects;

		protected TabPage					tabPageCultures;
		protected List<IconButton>			cultureButtonList;
	}
}
