using System.Collections.Generic;
using System.Text.RegularExpressions;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Viewers
{
	/// <summary>
	/// Permet de représenter les ressources d'un module.
	/// </summary>
	public class Panels : Abstract
	{
		protected enum MouseCursorType
		{
			Arrow,
			ArrowPlus,
			Global,
			Hand,
			Edit,
			Pen,
			Zoom,
		}

		public Panels(Module module) : base(module)
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
			this.array.CellsQuantityChanged += new EventHandler(this.HandleArrayCellsQuantityChanged);
			this.array.SelectedRowChanged += new EventHandler(this.HandleArraySelectedRowChanged);
			this.array.TabIndex = tabIndex++;
			this.array.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			VSplitter splitter = new VSplitter(this);
			splitter.Dock = DockStyle.Left;
			VSplitter.SetAutoCollapseEnable(left, true);

			this.toolBar = new VToolBar(this);
			this.toolBar.Margins = new Margins(0, 0, 0, 0);
			this.toolBar.Dock = DockStyle.Left;
			this.ToolBarAdd(Widgets.Command.Get("ToolSelect"));
			this.ToolBarAdd(Widgets.Command.Get("ToolGlobal"));
			this.ToolBarAdd(Widgets.Command.Get("ToolEdit"));
			this.ToolBarAdd(Widgets.Command.Get("ToolZoom"));
			this.ToolBarAdd(Widgets.Command.Get("ToolHand"));
			this.ToolBarAdd(null);
			this.ToolBarAdd(Widgets.Command.Get("ObjectLine"));
			this.ToolBarAdd(Widgets.Command.Get("ObjectButton"));
			this.ToolBarAdd(Widgets.Command.Get("ObjectText"));

			this.container = new MyWidgets.Frame(this);
			this.container.MinWidth = 100;
			this.container.MinHeight = 100;
			this.container.ChildrenLayoutMode = Widgets.Layouts.LayoutMode.Anchored;
			this.container.Margins = new Margins(1, 1, 1, 1);
			this.container.Dock = DockStyle.Fill;
			this.container.MouseDown += new EventHandler<Point>(this.HandleContainerMouseDown);
			this.container.MouseMove += new EventHandler<Point>(this.HandleContainerMouseMove);
			this.container.MouseUp += new EventHandler<Point>(this.HandleContainerMouseUp);

			this.tabBook = new TabBook(this);
			this.tabBook.PreferredWidth = 150;
			this.tabBook.Arrows = TabBookArrows.Stretch;
			this.tabBook.Margins = new Margins(0, 1, 1, 1);
			this.tabBook.Dock = DockStyle.Right;

			this.tabPageProperties = new TabPage();
			this.tabPageProperties.TabTitle = Res.Strings.Viewers.Panels.TabProperties;
			this.tabBook.Items.Add(this.tabPageProperties);

			this.tabPageObjects = new TabPage();
			this.tabPageObjects.TabTitle = Res.Strings.Viewers.Panels.TabObjects;
			this.tabBook.Items.Add(this.tabPageObjects);

			this.tabBook.ActivePage = this.tabPageProperties;

			this.module.PanelsRead();

			this.UpdateLabelsIndex("", Searcher.SearchingMode.None);
			this.UpdateArray();
			this.UpdateEdit();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.array.CellsQuantityChanged -= new EventHandler(this.HandleArrayCellsQuantityChanged);
				this.array.SelectedRowChanged -= new EventHandler(this.HandleArraySelectedRowChanged);

				this.labelEdit.EditionAccepted -= new EventHandler(this.HandleTextChanged);
				this.labelEdit.KeyboardFocusChanged -= new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleEditKeyboardFocusChanged);
			}

			base.Dispose(disposing);
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

			string name = this.labelsIndex[sel];
			this.module.PanelDelete(name);

			this.labelsIndex.RemoveAt(sel);
			this.UpdateArray();

			sel = System.Math.Min(sel, this.labelsIndex.Count-1);
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

			string name = this.labelsIndex[sel];
			string newName = this.GetDuplicateName(name);
			this.module.PanelCreate(newName, this.module.PanelIndex(name)+1);

			this.labelsIndex.Insert(newSel, newName);
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
			System.Diagnostics.Debug.Assert(newSel >= 0 && newSel < this.labelsIndex.Count);

			string name1 = this.labelsIndex[sel];
			string name2 = this.labelsIndex[newSel];

			this.module.PanelMove(name1, this.module.PanelIndex(name2));

			this.labelsIndex.RemoveAt(sel);
			this.labelsIndex.Insert(newSel, name1);
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


		protected override void UpdateLabelsIndex(string filter, Searcher.SearchingMode mode)
		{
			//	Construit l'index en fonction des ressources primaires.
			this.labelsIndex.Clear();

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

				this.labelsIndex.Add(name);
			}
		}

		protected override void UpdateArray()
		{
			//	Met à jour tout le contenu du tableau.
			this.array.TotalRows = this.labelsIndex.Count;

			int first = this.array.FirstVisibleRow;
			for (int i=0; i<this.array.LineCount; i++)
			{
				if (first+i < this.labelsIndex.Count)
				{
					string label = this.labelsIndex[first+i];
					int index = this.module.PanelIndex(label);
					bool newest = this.module.PanelNewest(index);

					if (newest)
					{
						label = Misc.Italic(label);
					}
					
					this.array.SetLineString(0, first+i, label);
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

			if (sel >= this.labelsIndex.Count)
			{
				sel = -1;
			}

			if ( sel == -1 )
			{
				this.labelEdit.Enable = false;
				this.labelEdit.Text = "";
			}
			else
			{
				string label = this.labelsIndex[sel];
				int index = this.module.PanelIndex(label);
				bool newest = this.module.PanelNewest(index);

				this.labelEdit.Enable = newest;
				this.labelEdit.Text = label;

				if (newest)
				{
					this.labelEdit.Focus();
					this.labelEdit.SelectAll();
				}
				else
				{
					this.labelEdit.Cursor = 100000;
				}
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
			int count = this.labelsIndex.Count;
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

			this.module.MainWindow.UpdateInfoCurrentModule();
			this.module.MainWindow.UpdateInfoAccess();
		}


		protected override int InfoAccessTotalCount
		{
			get
			{
				return this.module.PanelsCount;
			}
		}

		
		protected Widget ToolBarAdd(Command cs)
		{
			//	Ajoute une icône.
			if (cs == null)
			{
				IconSeparator sep = new IconSeparator();
				sep.IsHorizontal = false;
				this.toolBar.Items.Add(sep);
				return sep;
			}
			else
			{
				IconButton button = new IconButton(cs.Name, Misc.Icon(cs.IconName), cs.Name);
				this.toolBar.Items.Add(button);
				ToolTip.Default.SetToolTip(button, Misc.GetTextWithShortcut(cs));
				return button;
			}
		}


		protected bool IsExistingName(string baseName)
		{
			//	Indique si un nom existe.
			return (this.module.PanelIndex(baseName) != -1);
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


		#region Drawing select
		protected void SelectDown(Point pos)
		{
			//	Sélection ponctuelle, souris pressée.
		}

		protected void SelectMove(Point pos)
		{
			//	Sélection ponctuelle, souris déplacée.
			this.ChangeMouseCursor(MouseCursorType.Arrow);
		}

		protected void SelectUp(Point pos)
		{
			//	Sélection ponctuelle, souris relâchée.
		}
		#endregion

		#region Drawing global
		protected void GlobalDown(Point pos)
		{
			//	Sélection rectangulaire, souris pressée.
		}

		protected void GlobalMove(Point pos)
		{
			//	Sélection rectangulaire, souris déplacée.
			this.ChangeMouseCursor(MouseCursorType.Global);
		}

		protected void GlobalUp(Point pos)
		{
			//	Sélection rectangulaire, souris relâchée.
		}
		#endregion

		#region Drawing edit
		protected void EditDown(Point pos)
		{
			//	Edition, souris pressée.
		}

		protected void EditMove(Point pos)
		{
			//	Edition, souris déplacée.
			this.ChangeMouseCursor(MouseCursorType.Edit);
		}

		protected void EditUp(Point pos)
		{
			//	Edition, souris relâchée.
		}
		#endregion

		#region Drawing zoom
		protected void ZoomDown(Point pos)
		{
			//	Loupe, souris pressée.
		}

		protected void ZoomMove(Point pos)
		{
			//	Loupe, souris déplacée.
			this.ChangeMouseCursor(MouseCursorType.Zoom);
		}

		protected void ZoomUp(Point pos)
		{
			//	Loupe, souris relâchée.
		}
		#endregion

		#region Drawing hand
		protected void HandDown(Point pos)
		{
			//	Main, souris pressée.
		}

		protected void HandMove(Point pos)
		{
			//	Main, souris déplacée.
			this.ChangeMouseCursor(MouseCursorType.Hand);
		}

		protected void HandUp(Point pos)
		{
			//	Main, souris relâchée.
		}
		#endregion

		#region Drawing object
		protected void ObjectDown(string tool, Point pos)
		{
			//	Dessin d'un objet, souris pressée.
			this.ContainerLock();

			if (tool == "ObjectLine")
			{
				this.creatingObject = new Separator(this.container);
				this.creatingObject.PreferredHeight = 1;
			}

			if (tool == "ObjectButton")
			{
				this.creatingObject = new Button(this.container);
				this.creatingObject.Text = "Button";
			}
			
			if (tool == "ObjectText")
			{
				this.creatingObject = new TextField(this.container);
				this.creatingObject.Text = "TextField";
			}

			this.creatingObject.Anchor = AnchorStyles.BottomLeft;
			this.creatingObject.Margins = new Margins(pos.X, 0, 0, pos.Y);
		}

		protected void ObjectMove(string tool, Point pos)
		{
			//	Dessin d'un objet, souris déplacée.
			this.ChangeMouseCursor(MouseCursorType.Pen);

			if (this.creatingObject != null)
			{
				this.creatingObject.Margins = new Margins(pos.X, 0, 0, pos.Y);
			}
		}

		protected void ObjectUp(string tool, Point pos)
		{
			//	Dessin d'un objet, souris relâchée.
			this.creatingObject = null;
			this.ContainerUnlock();
		}
		#endregion

		protected void ContainerLock()
		{
#if false  // TODO: génère des asserts 'dirty layout' !
			this.container.MinSize = this.container.ActualSize;
			this.container.MaxSize = this.container.ActualSize;
#endif
		}

		protected void ContainerUnlock()
		{
#if false
			this.container.MinSize = new Size(100, 100);
			this.container.MaxSize = new Size(10000, 10000);
#endif
		}


		void HandleArrayCellsQuantityChanged(object sender)
		{
			//	Le nombre de lignes a changé.
			this.UpdateArray();
		}

		void HandleArraySelectedRowChanged(object sender)
		{
			//	La ligne sélectionnée a changé.
			this.UpdateEdit();
			this.UpdateCommands();
		}

		void HandleTextChanged(object sender)
		{
			//	Un texte éditable a changé.
			if ( this.ignoreChange )  return;

			AbstractTextField edit = sender as AbstractTextField;
			string newName = edit.Text;
			int sel = this.array.SelectedRow;
			string actualName = this.labelsIndex[sel];

			this.module.PanelRename(actualName, newName);

			this.labelsIndex[sel] = newName;
			this.UpdateArray();

			this.module.Modifier.IsDirty = true;
		}

		void HandleContainerMouseDown(object sender, Point pos)
		{
			//	La souris a été pressée dans l'objet 'container'.
			if (this.tool == "ToolSelect")
			{
				this.SelectDown(pos);
			}

			if (this.tool == "ToolGlobal")
			{
				this.GlobalDown(pos);
			}

			if (this.tool == "ToolEdit")
			{
				this.EditDown(pos);
			}

			if (this.tool == "ToolZoom")
			{
				this.ZoomDown(pos);
			}

			if (this.tool == "ToolHand")
			{
				this.HandDown(pos);
			}

			if (this.tool.StartsWith("Object"))
			{
				this.ObjectDown(this.tool, pos);
			}
		}

		void HandleContainerMouseMove(object sender, Point pos)
		{
			//	La souris a été déplacée dans l'objet 'container'.
			if (this.tool == "ToolSelect")
			{
				this.SelectMove(pos);
			}

			if (this.tool == "ToolGlobal")
			{
				this.GlobalMove(pos);
			}

			if (this.tool == "ToolEdit")
			{
				this.EditMove(pos);
			}

			if (this.tool == "ToolZoom")
			{
				this.ZoomMove(pos);
			}

			if (this.tool == "ToolHand")
			{
				this.HandMove(pos);
			}

			if (this.tool.StartsWith("Object"))
			{
				this.ObjectMove(this.tool, pos);
			}
		}

		void HandleContainerMouseUp(object sender, Point pos)
		{
			//	La souris a été relâchée dans l'objet 'container'.
			if (this.tool == "ToolSelect")
			{
				this.SelectUp(pos);
			}

			if (this.tool == "ToolGlobal")
			{
				this.GlobalUp(pos);
			}

			if (this.tool == "ToolEdit")
			{
				this.EditUp(pos);
			}

			if (this.tool == "ToolZoom")
			{
				this.ZoomUp(pos);
			}

			if (this.tool == "ToolHand")
			{
				this.HandUp(pos);
			}

			if (this.tool.StartsWith("Object"))
			{
				this.ObjectUp(this.tool, pos);
			}
		}


		#region MouseCursor
		protected void ChangeMouseCursor(MouseCursorType cursor)
		{
			//	Change le sprite de la souris.
			switch (cursor)
			{
				case MouseCursorType.Arrow:
					this.MouseCursorImage(ref this.mouseCursorArrow, Misc.Icon("CursorArrow"));
					break;

				case MouseCursorType.ArrowPlus:
					this.MouseCursorImage(ref this.mouseCursorArrowPlus, Misc.Icon("CursorArrowPlus"));
					break;

				case MouseCursorType.Global:
					this.MouseCursorImage(ref this.mouseCursorGlobal, Misc.Icon("CursorGlobal"));
					break;

				case MouseCursorType.Edit:
					this.MouseCursorImage(ref this.mouseCursorEdit, Misc.Icon("CursorEdit"));
					break;

				case MouseCursorType.Hand:
					this.MouseCursorImage(ref this.mouseCursorHand, Misc.Icon("CursorHand"));
					break;

				case MouseCursorType.Pen:
					this.MouseCursorImage(ref this.mouseCursorPen, Misc.Icon("CursorPen"));
					break;

				case MouseCursorType.Zoom:
					this.MouseCursorImage(ref this.mouseCursorZoom, Misc.Icon("CursorZoom"));
					break;

				default:
					this.MouseCursor = MouseCursor.AsArrow;
					break;
			}

			this.Window.MouseCursor = this.MouseCursor;
		}

		protected void MouseCursorImage(ref Image image, string name)
		{
			//	Choix du sprite de la souris.
			if (image == null)
			{
				image = Support.Resources.DefaultManager.GetImage(name);
			}

			this.MouseCursor = MouseCursor.FromImage(image);
		}
		#endregion


		protected TextFieldEx				labelEdit;
		protected VToolBar					toolBar;
		protected MyWidgets.Frame			container;
		protected TabBook					tabBook;
		protected TabPage					tabPageProperties;
		protected TabPage					tabPageObjects;
		protected Widget					creatingObject;

		protected Image						mouseCursorArrow = null;
		protected Image						mouseCursorArrowPlus = null;
		protected Image						mouseCursorGlobal = null;
		protected Image						mouseCursorEdit = null;
		protected Image						mouseCursorPen = null;
		protected Image						mouseCursorZoom = null;
		protected Image						mouseCursorHand = null;
	}
}
