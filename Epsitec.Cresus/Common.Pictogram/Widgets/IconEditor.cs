using Epsitec.Common.Widgets;
using Epsitec.Common.Dialogs;
using Epsitec.Common.Support;
using Epsitec.Common.Pictogram.Data;

namespace Epsitec.Common.Pictogram.Widgets
{
	/// <summary>
	/// La classe IconEditor représente l'éditeur d'icône complet.
	/// </summary>
	public class IconEditor : Epsitec.Common.Widgets.Widget
	{
		public IconEditor()
		{
			Epsitec.Common.Support.ImageProvider.RegisterAssembly("Epsitec.Common.Pictogram", this.GetType().Assembly);
			
			this.CreateLayout();
			this.UpdatePanels();
		}

		public IconEditor(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.allWidgets = false;

				this.toolTip = null;
				this.menu = null;
				this.hToolBar = null;
				this.vToolBar = null;
				this.root = null;
				this.info = null;
				this.pane = null;
				this.leftPane = null;
				this.rightPane = null;
				this.separator = null;
				this.panel = null;
				this.drawer = null;
				this.hScroller = null;
				this.vScroller = null;
				this.lister = null;
				this.frame1 = null;
				this.frame2 = null;
			}
			
			base.Dispose(disposing);
		}
		
		public override CommandDispatcher CommandDispatcher
		{
			get
			{
				if ( this.commandDispatcher == null )
				{
					// On crée son propre dispatcher, pour éviter de marcher sur les autres commandes.
					this.commandDispatcher = new Support.CommandDispatcher("IconEditor");
					this.commandDispatcher.RegisterController(this);
				}
				
				return this.commandDispatcher;
			}
		}


		protected void CreateLayout()
		{
			this.toolTip = new ToolTip();
			this.toolTip.Behaviour = ToolTipBehaviour.Normal;

			this.menu = new HMenu();
			this.menu.Host = this;
			this.menu.Items.Add(new MenuItem("", "Fichier"));
			this.menu.Items.Add(new MenuItem("", "Edition"));
			this.menu.Items.Add(new MenuItem("", "Objets"));
			this.menu.Items.Add(new MenuItem("", "Affichage"));
			this.menu.Items.Add(new MenuItem("", "Aide"));

			VMenu fileMenu = new VMenu();
			fileMenu.Name = "File";
			fileMenu.Host = this;
			this.MenuAdd(fileMenu, @"file:images/new1.icon", "New", "Nouveau", "Ctrl+N");
			this.MenuAdd(fileMenu, @"file:images/open1.icon", "Open", "Ouvrir...", "Ctrl+O");
			this.MenuAdd(fileMenu, @"file:images/save1.icon", "Save", "Enregistrer sous...", "Ctrl+S");
			this.MenuAdd(fileMenu, @"", "", "", "");
			this.MenuAdd(fileMenu, @"", "Quit", "Quitter", "");
			fileMenu.AdjustSize();
			this.menu.Items[0].Submenu = fileMenu;

			VMenu editMenu = new VMenu();
			editMenu.Name = "Edit";
			editMenu.Host = this;
			this.MenuAdd(editMenu, @"file:images/undo1.icon", "Undo", "Annuler", "Ctrl+Z");
			this.MenuAdd(editMenu, @"file:images/redo1.icon", "Redo", "Refaire", "Ctrl+Y");
			this.MenuAdd(editMenu, @"", "", "", "");
			this.MenuAdd(editMenu, @"file:images/cut1.icon", "Cut", "Couper", "Ctrl+X");
			this.MenuAdd(editMenu, @"file:images/copy1.icon", "Copy", "Copier", "Ctrl+C");
			this.MenuAdd(editMenu, @"file:images/paste1.icon", "Paste", "Coller", "Ctrl+V");
			this.MenuAdd(editMenu, @"", "", "", "");
			this.MenuAdd(editMenu, @"file:images/delete1.icon", "Delete", "Supprimer", "Del");
			this.MenuAdd(editMenu, @"file:images/duplicate1.icon", "Duplicate", "Dupliquer", "");
			editMenu.AdjustSize();
			this.menu.Items[1].Submenu = editMenu;

			VMenu objMenu = new VMenu();
			objMenu.Name = "Obj";
			objMenu.Host = this;
			this.MenuAdd(objMenu, @"file:images/deselect1.icon", "Deselect", "Désélectionner tout", "");
			this.MenuAdd(objMenu, @"file:images/selectall1.icon", "SelectAll", "Tout sélectionner", "");
			this.MenuAdd(objMenu, @"file:images/selectinvert1.icon", "SelectInvert", "Inverser la sélection", "");
			this.MenuAdd(objMenu, @"", "", "", "");
			this.MenuAdd(objMenu, @"file:images/orderup1.icon", "OrderUp", "Dessus", "");
			this.MenuAdd(objMenu, @"file:images/orderdown1.icon", "OrderDown", "Dessous", "");
			this.MenuAdd(objMenu, @"", "", "", "");
			this.MenuAdd(objMenu, @"file:images/groupempty1.icon", "", "Groupe", "");
			objMenu.AdjustSize();
			this.menu.Items[2].Submenu = objMenu;

			VMenu groupMenu = new VMenu();
			groupMenu.Name = "Group";
			groupMenu.Host = this;
			this.MenuAdd(groupMenu, @"file:images/merge1.icon", "Merge", "Fusionner", "");
			this.MenuAdd(groupMenu, @"file:images/group1.icon", "Group", "Associer", "");
			this.MenuAdd(groupMenu, @"file:images/ungroup1.icon", "Ungroup", "Dissocier", "");
			this.MenuAdd(groupMenu, @"", "", "", "");
			this.MenuAdd(groupMenu, @"file:images/inside1.icon", "Inside", "Entrer dans le groupe", "");
			this.MenuAdd(groupMenu, @"file:images/outside1.icon", "Outside", "Sortir du groupe", "");
			groupMenu.AdjustSize();
			objMenu.Items[7].Submenu = groupMenu;

			VMenu showMenu = new VMenu();
			showMenu.Name = "Show";
			showMenu.Host = this;
			this.MenuAdd(showMenu, @"file:images/selectmode1.icon", "SelectMode", "Sélection partielle", "");
			this.MenuAdd(showMenu, @"file:images/grid1.icon", "Grid", "Grille magnétique", "");
			this.MenuAdd(showMenu, @"file:images/mode1.icon", "Mode", "Tableau des objets", "");
			this.MenuAdd(showMenu, @"", "", "", "");
			this.MenuAdd(showMenu, @"file:images/zoommenu1.icon", "", "Zoom", "");
			this.MenuAdd(showMenu, @"", "", "", "");
			this.MenuAdd(showMenu, @"", "", "Apparence", "");
			showMenu.AdjustSize();
			this.menu.Items[3].Submenu = showMenu;

			VMenu zoomMenu = new VMenu();
			zoomMenu.Name = "Zoom";
			zoomMenu.Host = this;
			this.MenuAdd(zoomMenu, @"file:images/zoommin1.icon", "ZoomMin", "Zoom minimal", "");
			this.MenuAdd(zoomMenu, @"file:images/zoomdefault1.icon", "ZoomDefault", "Zoom 100%", "");
			this.MenuAdd(zoomMenu, @"file:images/zoomsel1.icon", "ZoomSel", "Zoom sélection", "");
			this.MenuAdd(zoomMenu, @"file:images/zoomprev1.icon", "ZoomPrev", "Zoom précédent", "");
			this.MenuAdd(zoomMenu, @"", "", "", "");
			this.MenuAdd(zoomMenu, @"file:images/zoomsub1.icon", "ZoomSub", "Réduction", "");
			this.MenuAdd(zoomMenu, @"file:images/zoomadd1.icon", "ZoomAdd", "Agrandissement", "");
			zoomMenu.AdjustSize();
			showMenu.Items[4].Submenu = zoomMenu;

			VMenu lookMenu = new VMenu();
			lookMenu.Name = "Look";
			lookMenu.Host = this;
			string[] list = Epsitec.Common.Widgets.Adorner.Factory.AdornerNames;
			foreach ( string name in list )
			{
				this.MenuAdd(lookMenu, @"y/n", "SelectLook (this.Name)", name, "", name);
			}
			lookMenu.AdjustSize();
			showMenu.Items[6].Submenu = lookMenu;

			VMenu helpMenu = new VMenu();
			helpMenu.Name = "Help";
			helpMenu.Host = this;
			helpMenu.Items.Add(new MenuItem("help", "", "Aide", "F1"));
			helpMenu.Items.Add(new MenuItem("ctxhelp", "", "Aide contextuelle", ""));
			helpMenu.Items.Add(new MenuItem("about", "", "A propos de...", ""));
			helpMenu.AdjustSize();
			this.menu.Items[4].Submenu = helpMenu;

			this.hToolBar = new HToolBar();
			this.hToolBar.Parent = this;
			this.HToolBarAdd(@"file:images/new1.icon", "New", "Nouveau");
			this.HToolBarAdd(@"file:images/open1.icon", "Open", "Ouvrir");
			this.HToolBarAdd(@"file:images/save1.icon", "Save", "Enregistrer");
			this.HToolBarAdd("", "", "");
			this.HToolBarAdd(@"file:images/delete1.icon", "Delete", "Supprimer");
			this.HToolBarAdd(@"file:images/duplicate1.icon", "Duplicate", "Dupliquer");
			this.HToolBarAdd("", "", "");
			this.HToolBarAdd(@"file:images/undo1.icon", "Undo", "Annuler");
			this.HToolBarAdd(@"file:images/redo1.icon", "Redo", "Refaire");
			this.HToolBarAdd("", "", "");
			this.HToolBarAdd(@"file:images/orderup1.icon", "OrderUp", "Dessus");
			this.HToolBarAdd(@"file:images/orderdown1.icon", "OrderDown", "Dessous");
			this.HToolBarAdd("", "", "");
			this.HToolBarAdd(@"file:images/merge1.icon", "Merge", "Fusionner");
			this.HToolBarAdd(@"file:images/group1.icon", "Group", "Associer");
			this.HToolBarAdd(@"file:images/ungroup1.icon", "Ungroup", "Dissocier");
			this.HToolBarAdd(@"file:images/inside1.icon", "Inside", "Entrer dans le groupe");
			this.HToolBarAdd(@"file:images/outside1.icon", "Outside", "Sortir du groupe");
			this.HToolBarAdd("", "", "");
			this.HToolBarAdd(@"file:images/selectmode1.icon", "SelectMode", "Sélection partielle");
			this.HToolBarAdd(@"file:images/grid1.icon", "Grid", "Grille magnétique");
			this.HToolBarAdd(@"file:images/mode1.icon", "Mode", "Tableau des objets");
			this.HToolBarAdd("", "", "");

			this.vToolBar = new VToolBar();
			this.vToolBar.Parent = this;
			this.VToolBarAdd(@"file:images/select1.icon", "SelectTool", "Sélectionner", "Select");
			this.VToolBarAdd(@"file:images/zoom1.icon", "SelectTool", "Agrandir", "Zoom");
			this.VToolBarAdd(@"file:images/hand1.icon", "SelectTool", "Déplacer", "Hand");
			this.VToolBarAdd(@"file:images/picker1.icon", "SelectTool", "Pipette", "Picker");
			this.VToolBarAdd("", "", "");
			this.VToolBarAdd(@"file:images/line1.icon", "SelectTool", "Segment de ligne", "ObjectLine");
			this.VToolBarAdd(@"file:images/arrow1.icon", "SelectTool", "Flèche", "ObjectArrow");
			this.VToolBarAdd(@"file:images/rectangle1.icon", "SelectTool", "Rectangle", "ObjectRectangle");
			this.VToolBarAdd(@"file:images/circle1.icon", "SelectTool", "Cercle", "ObjectCircle");
			this.VToolBarAdd(@"file:images/ellipse1.icon", "SelectTool", "Ellipse", "ObjectEllipse");
			this.VToolBarAdd(@"file:images/regular1.icon", "SelectTool", "Polygone régulier", "ObjectRegular");
			this.VToolBarAdd(@"file:images/poly1.icon", "SelectTool", "Polygone quelconque", "ObjectPoly");
			this.VToolBarAdd(@"file:images/bezier1.icon", "SelectTool", "Courbes de Bézier", "ObjectBezier");
			this.VToolBarAdd(@"file:images/text1.icon", "SelectTool", "Texte", "ObjectText");
			this.VToolBarAdd("", "", "");
			
			this.root = new Widget();
			this.root.Parent = this;
			
			this.pane = new PaneBook();
			this.pane.PaneBookStyle = PaneBookStyle.LeftRight;
			this.pane.PaneBehaviour = PaneBookBehaviour.FollowMe;
			this.pane.SizeChanged += new EventHandler(this.HandlePaneSizeChanged);
			this.pane.Parent = root;

			this.leftPane = new PanePage();
			this.leftPane.PaneRelativeSize = 10;
			this.leftPane.PaneElasticity = 1;
			this.leftPane.PaneMinSize = 100;
			this.pane.Items.Add(this.leftPane);

			this.rightPane = new PanePage();
			this.rightPane.PaneAbsoluteSize = 40;
			this.rightPane.PaneElasticity = 0;
			this.rightPane.PaneMinSize = 40;
			this.rightPane.PaneMaxSize = 200;
			this.pane.Items.Add(this.rightPane);

			this.separator = new Separator();
			this.separator.Parent = this;

			this.panel = new Widget();
			this.panel.Parent = this;

			this.drawer = new Drawer();
			this.drawer.InitCommands(this.CommandDispatcher);
			this.CommandDispatcher.RegisterController(this.drawer);
			this.drawer.IsEditable = true;
			this.drawer.SelectedTool = "Select";
			this.drawer.PanelChanged += new EventHandler(this.HandleDrawerPanelChanged);
			this.drawer.CommandChanged += new EventHandler(this.HandleDrawerCommandChanged);
			this.drawer.AllChanged += new EventHandler(this.HandleDrawerAllChanged);
			this.drawer.ScrollerChanged += new EventHandler(this.HandleDrawerScrollerChanged);
			this.drawer.InfoObjectChanged += new EventHandler(this.HandleDrawerInfoObjectChanged);
			this.drawer.InfoMouseChanged += new EventHandler(this.HandleDrawerInfoMouseChanged);
			this.drawer.InfoZoomChanged += new EventHandler(this.HandleDrawerInfoZoomChanged);
			this.drawer.Parent = this.leftPane;

			this.lister = new Lister();
			this.lister.PanelChanged += new EventHandler(this.HandleDrawerAllChanged);
			this.lister.IconObjects = this.drawer.IconObjects;
			this.lister.Parent = this.leftPane;
			this.lister.Hide();

			this.frame1 = new SampleButton();
			this.frame1.ButtonStyle = ButtonStyle.ToolItem;
			this.frame1.ActiveState = WidgetState.ActiveYes;
			this.frame1.IconObjects.Objects = this.drawer.Objects;
			this.frame1.Parent = this.rightPane;

			this.frame2 = new SampleButton();
			this.frame2.ButtonStyle = ButtonStyle.ToolItem;
			this.frame2.ActiveState = WidgetState.ActiveYes;
			this.frame2.SetEnabled(false);
			this.frame2.IconObjects.Objects = this.drawer.Objects;
			this.frame2.Parent = this.rightPane;

			this.drawer.AddClone(this.frame1);
			this.drawer.AddClone(this.frame2);
			this.drawer.AddClone(this.lister);

			this.hScroller = new HScroller();
			this.hScroller.ValueChanged += new EventHandler(this.HandleHScrollerValueChanged);
			this.hScroller.Parent = this.leftPane;

			this.vScroller = new VScroller();
			this.vScroller.ValueChanged += new EventHandler(this.HandleVScrollerValueChanged);
			this.vScroller.Parent = this.leftPane;

			this.info = new StatusBar();
			this.info.Parent = this;
			this.InfoAdd("", 200, "StatusDocument", "");
			this.InfoAdd(@"file:images/deselect1.icon", 0, "Deselect", "Désélectionner tout");
			this.InfoAdd(@"file:images/selectall1.icon", 0, "SelectAll", "Tout sélectionner");
			this.InfoAdd(@"file:images/selectinvert1.icon", 0, "SelectInvert", "Inverser la sélection");
			this.InfoAdd("", 120, "StatusObject", "");
			this.InfoAdd(@"file:images/zoommin1.icon", 0, "ZoomMin", "Zoom minimal");
			this.InfoAdd(@"file:images/zoomdefault1.icon", 0, "ZoomDefault", "Zoom 100%");
			this.InfoAdd(@"file:images/zoomsel1.icon", 0, "ZoomSel", "Zoom sélection");
			this.InfoAdd(@"file:images/zoomprev1.icon", 0, "ZoomPrev", "Zoom précédent");
			this.InfoAdd(@"file:images/zoomsub1.icon", 0, "ZoomSub", "Réduction");
			this.InfoAdd(@"file:images/zoomadd1.icon", 0, "ZoomAdd", "Agrandissement");
			this.InfoAdd("", 90, "StatusZoom", "");
			this.InfoAdd("", 120, "StatusMouse", "");

			this.allWidgets = true;
			this.ResizeLayout();
			this.drawer.UpdateCommands();
		}

		public HMenu GetMenu()
		{
			return this.menu;
		}

		private void HandleDrawerPanelChanged(object sender)
		{
			this.UpdatePanels();
		}

		private void HandleDrawerCommandChanged(object sender)
		{
			this.drawer.UpdateCommands();
		}

		private void HandleDrawerAllChanged(object sender)
		{
			this.drawer.UpdateCommands();
			this.UpdatePanels();
		}

		private void HandleDrawerScrollerChanged(object sender)
		{
			IconObjects icon = this.drawer.IconObjects;
			Drawing.Size size = icon.Size;
			Drawing.Size area = icon.SizeArea;

			this.hScroller.Minimum = (size.Width-area.Width)/2;
			this.hScroller.Maximum = (area.Width+this.hScroller.Minimum) - size.Width/this.drawer.Zoom;
			this.hScroller.VisibleRangeRatio = (size.Width/area.Width)/this.drawer.Zoom;
			this.hScroller.Value = -this.drawer.OriginX;
			this.drawer.OriginX = -this.hScroller.Value;

			this.vScroller.Minimum = (size.Height-area.Height)/2;
			this.vScroller.Maximum = (area.Height+this.vScroller.Minimum) - size.Height/this.drawer.Zoom;
			this.vScroller.VisibleRangeRatio = (size.Height/area.Height)/this.drawer.Zoom;
			this.vScroller.Value = -this.drawer.OriginY;
			this.drawer.OriginY = -this.vScroller.Value;
		}

		// Ajoute une icône.
		protected void MenuAdd(VMenu vmenu, string icon, string command, string text, string shortcut)
		{
			this.MenuAdd(vmenu, icon, command, text, shortcut, command);
		}
		
		protected void MenuAdd(VMenu vmenu, string icon, string command, string text, string shortcut, string name)
		{
			if ( text == "" )
			{
				vmenu.Items.Add(new MenuSeparator());
			}
			else
			{
				string iconNo  = "";
				string iconYes = "";

				if ( icon == "y/n" )
				{
					icon    = @"";
					iconNo  = @"file:images/activeno1.icon";
					iconYes = @"file:images/activeyes1.icon";
				}

				MenuItem item = new MenuItem(command, icon, text, shortcut, name);
				if ( iconNo  != "" )  item.IconNameActiveNo  = iconNo;
				if ( iconYes != "" )  item.IconNameActiveYes = iconYes;
				vmenu.Items.Add(item);
			}
		}

		// Ajoute une icône.
		protected void HToolBarAdd(string icon, string command, string tooltip)
		{
			if ( icon == "" )
			{
				IconSeparator sep = new IconSeparator();
				sep.IsHorizontal = true;
				this.hToolBar.Items.Add(sep);
			}
			else
			{
				IconButton button = new IconButton(command, icon, command);
				this.hToolBar.Items.Add(button);
				this.toolTip.SetToolTip(button, tooltip);
			}
		}

		// Ajoute une icône.
		protected void VToolBarAdd(string icon, string command, string tooltip)
		{
			this.VToolBarAdd(icon, command, tooltip, command);
		}

		protected void VToolBarAdd(string icon, string command, string tooltip, string name)
		{
			if ( icon == "" )
			{
				IconSeparator sep = new IconSeparator();
				sep.IsHorizontal = false;
				this.vToolBar.Items.Add(sep);
			}
			else
			{
				IconButton button = new IconButton(command, icon, name);
				this.vToolBar.Items.Add(button);
				this.toolTip.SetToolTip(button, tooltip);
			}
		}

		// Ajoute une icône.
		protected void InfoAdd(string icon, double width, string command, string tooltip)
		{
			this.InfoAdd(icon, width, command, tooltip, command);
		}
		
		protected void InfoAdd(string icon, double width, string command, string tooltip, string name)
		{
			if ( icon == "" )
			{
				StatusField field = new StatusField();
				field.Width = width;
				this.info.Items.Add(field);

				int i = this.info.Children.Count-1;
				this.info.Items[i].Name = name;
			}
			else
			{
				IconButton button = new IconButton(command, icon, name);
				double h = this.info.DefaultHeight-3;
				button.Size = new Drawing.Size(h, h);
				this.info.Items.Add(button);
				this.toolTip.SetToolTip(button, tooltip);
			}
		}

		// Extrait le nom de fichier, en ignorant les noms de dossiers et l'extension.
		// "c:\rep\abc.txt" devient "abc".
		public string ExtractName(string filename)
		{
			int i = filename.LastIndexOf("\\")+1;
			if ( i < 0 )  i = 0;
			int j = filename.IndexOf(".", i);
			if ( j < 0 )  j = filename.Length;
			if ( j <= i )  return "";
			return filename.Substring(i, j-i);
		}

		private void UpdateInfoDocument()
		{
			StatusField field = this.info.Items["StatusDocument"] as StatusField;
			string name = this.ExtractName(this.filename);
			Drawing.Size size = this.drawer.IconObjects.Size;
			field.Text = string.Format("{0} ({1}x{2})", name, size.Width, size.Height);
			field.Invalidate();
		}

		private void HandleDrawerInfoObjectChanged(object sender)
		{
			StatusField field = this.info.Items["StatusObject"] as StatusField;
			field.Text = this.drawer.TextInfoObject;
			field.Invalidate();
		}

		private void HandleDrawerInfoMouseChanged(object sender)
		{
			StatusField field = this.info.Items["StatusMouse"] as StatusField;
			field.Text = this.drawer.TextInfoMouse;
			field.Invalidate();
		}

		private void HandleDrawerInfoZoomChanged(object sender)
		{
			StatusField field = this.info.Items["StatusZoom"] as StatusField;
			field.Text = this.drawer.TextInfoZoom;
			field.Invalidate();
		}

		private void HandleHScrollerValueChanged(object sender)
		{
			this.drawer.OriginX = -this.hScroller.Value;
		}

		private void HandleVScrollerValueChanged(object sender)
		{
			this.drawer.OriginY = -this.vScroller.Value;
		}

		// Met à jour les panneaux de gauche en fonction des propriétés de l'objet.
		protected void UpdatePanels()
		{
			this.panel.Children.Clear();

			Drawing.Rectangle rect = new Drawing.Rectangle();

			System.Collections.ArrayList list = this.drawer.PropertiesList();
			double posy = this.panel.Height;
			Widget originColorLastPanel = null;
			int index = 0;
			foreach ( AbstractProperty property in list )
			{
				AbstractPanel panel = property.CreatePanel();

				AbstractProperty p = this.drawer.GetProperty(property.Type);
				panel.SetProperty(p);
				panel.Multi = p.Multi;

				rect.Left   = 0;
				rect.Right  = this.panel.Width;
				rect.Bottom = posy-panel.DefaultHeight;
				rect.Top    = posy;
				panel.Bounds = rect;
				panel.Anchor = AnchorStyles.LeftAndRight|AnchorStyles.Top;
				panel.Changed += new EventHandler(this.HandlePanelChanged);
				panel.ExtendedChanged += new EventHandler(this.HandleExtendedChanged);
				panel.OriginColorChanged += new EventHandler(this.HandleOriginColorChanged);
				panel.TabIndex = index++;
				panel.TabNavigation = Widget.TabNavigationMode.ActivateOnTab | Widget.TabNavigationMode.ForwardToChildren | Widget.TabNavigationMode.ForwardOnly;
				panel.Parent = this.panel;

				if ( panel.PropertyType == this.originColorType )
				{
					originColorLastPanel = panel;
				}

				posy -= rect.Height;
			}
			this.leftHeightUsed = this.panel.Height-posy;

			if ( this.colorSelector == null )
			{
				this.colorSelector = new ColorSelector();
			}
			rect.Left   = 0;
			rect.Right  = this.panel.Width;
			rect.Bottom = 0;
			rect.Top    = System.Math.Min(this.colorSelector.DefaultHeight, this.panel.Height-this.leftHeightUsed);
			this.colorSelector.Bounds = rect;
			this.colorSelector.Anchor = AnchorStyles.LeftAndRight|AnchorStyles.Bottom;
			this.colorSelector.Changed += new EventHandler(this.HandleColorSelectorChanged);
			this.colorSelector.TabIndex = index++;
			this.colorSelector.TabNavigation = Widget.TabNavigationMode.ActivateOnTab | Widget.TabNavigationMode.ForwardToChildren | Widget.TabNavigationMode.ForwardOnly;
			this.colorSelector.Parent = this.panel;

			this.HandleOriginColorChanged(originColorLastPanel, true);
			this.HandleDrawerScrollerChanged(null);
			this.HandleDrawerInfoObjectChanged(null);
			this.HandleDrawerInfoMouseChanged(null);
			this.HandleDrawerInfoZoomChanged(null);
			this.UpdateInfoDocument();
		}

		// Le contenu d'un panneau a été changé.
		private void HandlePanelChanged(object sender)
		{
			AbstractPanel panel = sender as AbstractPanel;
			AbstractProperty property = panel.GetProperty();
			this.drawer.SetProperty(property);
			panel.Multi = false;
		}

		// La hauteur d'un panneau a été changée.
		private void HandleExtendedChanged(object sender)
		{
			AbstractPanel panel = sender as AbstractPanel;
			AbstractProperty property = panel.GetProperty();
			this.drawer.SetPropertyExtended(property);
			this.UpdatePanels();
		}

		// Le widget qui détermine la couleur d'origine a changé.
		private void HandleOriginColorChanged(object sender)
		{
			this.HandleOriginColorChanged(sender, false);
		}

		private void HandleOriginColorChanged(object sender, bool lastOrigin)
		{
			this.originColorPanel = null;

			foreach ( Widget widget in this.panel.Children )
			{
				AbstractPanel panel = widget as AbstractPanel;
				if ( panel == null )  continue;
				Widget wSender = sender as Widget;
				if ( panel == wSender )
				{
					this.originColorPanel = panel;
					panel.OriginColorSelect( lastOrigin ? this.originColorRank : -1 );
				}
				else
				{
					panel.OriginColorDeselect();
				}
			}

			if ( this.originColorPanel == null )
			{
				this.colorSelector.SetEnabled(false);
			}
			else
			{
				this.colorSelector.SetEnabled(true);
				this.ignoreColorChanged = true;
				this.colorSelector.Color = this.originColorPanel.OriginColorGet();
				this.ignoreColorChanged = false;
				this.originColorType = this.originColorPanel.PropertyType;
				this.originColorRank = this.originColorPanel.OriginColorRank();
			}
		}

		// Couleur d'origine changée dans la roue.
		private void HandleColorSelectorChanged(object sender)
		{
			if ( this.ignoreColorChanged || this.originColorPanel == null )  return;
			this.originColorPanel.OriginColorChange(this.colorSelector.Color);

			AbstractProperty property = this.originColorPanel.GetProperty();
			this.drawer.SetProperty(property);
			this.originColorPanel.Multi = false;
		}


		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();
			this.ResizeLayout();
		}

		protected void ResizeLayout()
		{
			if ( !this.allWidgets )  return;

			Drawing.Rectangle rect = new Drawing.Rectangle(0, 0, this.Client.Width, this.Client.Height);

			this.hToolBar.Location = new Drawing.Point(0, rect.Height-this.hToolBar.DefaultHeight);
			this.hToolBar.Size = new Drawing.Size(rect.Width, this.hToolBar.DefaultHeight);

			this.vToolBar.Location = new Drawing.Point(0, this.info.Height);
			this.vToolBar.Size = new Drawing.Size(this.vToolBar.DefaultWidth, rect.Height-this.info.Height-this.hToolBar.DefaultHeight);

			this.info.Location = new Drawing.Point(0, 0);
			this.info.Size = new Drawing.Size(rect.Width, this.info.DefaultHeight);

			this.separator.Location = new Drawing.Point(rect.Right-200-1, this.info.Height);
			this.separator.Size = new Drawing.Size(1, rect.Height-this.info.Height-this.hToolBar.DefaultHeight);

			this.panel.Location = new Drawing.Point(rect.Right-200, this.info.Height);
			this.panel.Size = new Drawing.Size(200, rect.Height-this.info.Height-this.hToolBar.DefaultHeight);

			this.root.Location = new Drawing.Point(this.vToolBar.DefaultWidth, this.info.Height);
			this.root.Size = new Drawing.Size(rect.Width-this.vToolBar.DefaultWidth-200-1, rect.Height-this.info.Height-this.hToolBar.DefaultHeight);
			//this.root.SetClientAngle(0);
			//this.root.SetClientZoom(1.0);

			this.pane.Location = new Drawing.Point(0, 0);
			this.pane.Size = this.root.Size;
			this.rightPane.PaneMinSize = 20+this.drawer.IconObjects.Size.Width;

			if ( this.colorSelector != null )
			{
				rect.Left   = 0;
				rect.Right  = this.panel.Width;
				rect.Bottom = 0;
				rect.Top    = System.Math.Min(this.colorSelector.DefaultHeight, this.panel.Height-this.leftHeightUsed);
				this.colorSelector.Bounds = rect;
			}

			Drawing.Size iconSize = this.drawer.IconObjects.Size;
			double dimx = this.leftPane.Width-20;
			double dimy = dimx*iconSize.Height/iconSize.Width;
			if ( dimy > this.leftPane.Height-20 )
			{
				dimy = this.leftPane.Height-20;
				dimx = dimy*iconSize.Width/iconSize.Height;
			}
			dimx -= this.vScroller.DefaultWidth;
			dimy -= this.hScroller.DefaultHeight;
			this.drawer.Location = new Drawing.Point(10, this.leftPane.Height-10-dimy-1);
			this.drawer.Size = new Drawing.Size(dimx+1, dimy+1);

			rect.Left   = 10;
			rect.Width  = dimx+1;
			rect.Bottom = this.leftPane.Height-10-dimy-this.hScroller.DefaultHeight;
			rect.Height = this.hScroller.DefaultHeight;
			this.hScroller.Bounds = rect;

			rect.Left   = 10+dimx;
			rect.Width  = this.vScroller.DefaultWidth;
			rect.Bottom = this.leftPane.Height-10-dimy-1;
			rect.Height = dimy+1;
			this.vScroller.Bounds = rect;

			this.lister.Location = new Drawing.Point(10, 10);
			this.lister.Size = new Drawing.Size(this.leftPane.Width-20, this.leftPane.Height-20);

			dimx = this.rightPane.Width-20;
			dimy = dimx*iconSize.Height/iconSize.Width;
			rect.Left   = 10;
			rect.Bottom = this.rightPane.Height-10-dimy-1;
			rect.Width  = dimx;
			rect.Height = dimy;
			rect.Inflate(1, 1);
			this.frame1.Bounds = rect;
			rect.Inflate(-1, -1);

			rect.Offset(0, -dimy-10);
			rect.Inflate(1, 1);
			this.frame2.Bounds = rect;
			rect.Inflate(-1, -1);
		}

		private void HandlePaneSizeChanged(object sender)
		{
			PaneBook pane = (PaneBook)sender;

			if ( pane == this.pane )
			{
			}
			this.ResizeLayout();
		}


		[Command ("SelectTool")]
		void CommandActivateTool(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			if ( this.drawer.IsVisible )
			{
				Widget widget = e.Source as Widget;
				this.drawer.SelectedTool = widget.Name;
			}
			else
			{
				this.drawer.SelectedTool = "Select";
			}
			
			this.drawer.UpdateCommands();
			this.UpdatePanels();
		}

		[Command ("SelectLook")]
		void CommandSelectLook(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			Epsitec.Common.Widgets.Adorner.Factory.SetActive(e.CommandArgs[0]);
			this.drawer.UpdateCommands();
		}
		
		[Command ("New")]
		void CommandNew(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.drawer.CommandNew();
			this.filename = "";
			this.UpdateInfoDocument();
		}

		[Command ("Open")]
		void CommandOpen(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			FileOpen dialog = new FileOpen();
		
			dialog.Title = "Ouvrir une icone";
			dialog.FileName = this.filename;
			dialog.Filters.Add("icon", "Icônes", "*.icon");
			dialog.Show();

			this.filename = dialog.FileName;
			this.UpdateInfoDocument();

			this.drawer.CommandOpen(this.filename);

			this.ResizeLayout();
			this.Invalidate();
			this.UpdateInfoDocument();
		}

		[Command ("Save")]
		void CommandSave(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			FileSave dialog = new FileSave();
			
			dialog.Title = "Enregisrter une icone";
			dialog.FileName = this.filename;
			dialog.Filters.Add("icon", "Icônes", "*.icon");
			dialog.Show();

			this.filename = dialog.FileName;
			this.UpdateInfoDocument();

			this.drawer.CommandSave(this.filename);
		}

		[Command ("Mode")]
		void CommandMode(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			if ( this.drawer.IsActive )
			{
				this.drawer.IsActive = false;
				this.drawer.Hide();
				this.hScroller.Hide();
				this.vScroller.Hide();
				this.lister.Show();

				this.drawer.SelectedTool = "Select";
				this.UpdatePanels();
			}
			else
			{
				this.drawer.IsActive = true;
				this.drawer.Show();
				this.hScroller.Show();
				this.vScroller.Show();
				this.lister.Hide();
			}
			this.drawer.UpdateCommands();
		}


		protected CommandDispatcher				commandDispatcher;
		protected bool							allWidgets = false;
		protected ToolTip						toolTip;
		protected HMenu							menu;
		protected HToolBar						hToolBar;
		protected VToolBar						vToolBar;
		protected Widget						root;
		protected StatusBar						info;
		protected PaneBook						pane;
		protected PanePage						leftPane;
		protected PanePage						rightPane;
		protected Separator						separator;
		protected Widget						panel;
		protected ColorWheel					circle;
		protected Drawer						drawer;
		protected HScroller						hScroller;
		protected VScroller						vScroller;
		protected Lister						lister;
		protected SampleButton					frame1;
		protected SampleButton					frame2;
		protected ColorSelector					colorSelector;
		protected AbstractPanel					originColorPanel = null;
		protected PropertyType					originColorType = PropertyType.None;
		protected int							originColorRank = -1;
		protected double						leftHeightUsed = 0;
		protected string						filename = "";
		protected bool							ignoreColorChanged = false;
	}
}
