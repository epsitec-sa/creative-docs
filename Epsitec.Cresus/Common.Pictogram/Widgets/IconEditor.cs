using Epsitec.Common.Widgets;
using Epsitec.Common.Dialogs;
using Epsitec.Common.Support;
using Epsitec.Common.Pictogram.Data;

namespace Epsitec.Common.Pictogram.Widgets
{
	/// <summary>
	/// La classe IconEditor représente l'éditeur d'icône complet.
	/// </summary>
	
	[SuppressBundleSupport]
	
	public class IconEditor : Epsitec.Common.Widgets.Widget
	{
		public IconEditor()
		{
			this.CreateLayout();
			this.UpdatePanels();
			this.panelPatterns.Update();
			this.panelPages.Update();
			this.panelLayers.Update();
			this.panelStyles.UpdateAll(-1);
			this.UpdatePagesLayers();
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

				this.menu = null;
				this.hToolBar = null;
				this.vToolBar = null;
				this.root = null;
				this.info = null;
				this.pane = null;
				this.leftPane = null;
				this.rightPane = null;
				this.book = null;
				this.bookPanel = null;
				this.bookStyles = null;
				this.bookPatterns = null;
				this.bookPages = null;
				this.bookLayers = null;
				this.panel = null;
				this.drawer = null;
				this.quickPagePrev = null;
				this.quickPageMenu = null;
				this.quickPageNext = null;
				this.hScroller = null;
				this.quickLayerPrev = null;
				this.quickLayerMenu = null;
				this.quickLayerNext = null;
				this.vScroller = null;
				this.lister = null;
				this.frame1 = null;
				this.frame2 = null;
				this.panelStyles = null;
				this.panelPatterns = null;
				this.panelPages = null;
				this.panelLayers = null;
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
			ToolTip.Default.Behaviour = ToolTipBehaviour.Normal;

			this.menu = new HMenu();
			this.menu.Host = this;
			this.menu.Items.Add(new MenuItem("", "Fichier"));
			this.menu.Items.Add(new MenuItem("", "Edition"));
			this.menu.Items.Add(new MenuItem("", "Objets"));
			this.menu.Items.Add(new MenuItem("", "Affichage"));
			this.menu.Items.Add(new MenuItem("", "Tableau"));
			this.menu.Items.Add(new MenuItem("", "Debug"));
			this.menu.Items.Add(new MenuItem("", "Aide"));

			VMenu fileMenu = new VMenu();
			fileMenu.Name = "File";
			fileMenu.Host = this;
			this.MenuAdd(fileMenu, @"file:images/new.icon", "New", "Nouveau", "Ctrl+N");
			this.MenuAdd(fileMenu, @"file:images/open.icon", "Open", "Ouvrir...", "Ctrl+O");
			this.MenuAdd(fileMenu, @"file:images/save.icon", "Save", "Enregistrer sous...", "Ctrl+S");
			this.MenuAdd(fileMenu, @"", "", "", "");
			this.MenuAdd(fileMenu, @"file:images/print.icon", "Print", "Imprimer...", "Ctrl+P");
			this.MenuAdd(fileMenu, @"", "", "", "");
			this.MenuAdd(fileMenu, @"", "QuitApplication", "Quitter", "");
			fileMenu.AdjustSize();
			this.menu.Items[0].Submenu = fileMenu;

			VMenu editMenu = new VMenu();
			editMenu.Name = "Edit";
			editMenu.Host = this;
			this.MenuAdd(editMenu, @"file:images/undo.icon", "Undo", "Annuler", "Ctrl+Z");
			this.MenuAdd(editMenu, @"file:images/redo.icon", "Redo", "Refaire", "Ctrl+Y");
			this.MenuAdd(editMenu, @"", "", "", "");
			this.MenuAdd(editMenu, @"file:images/cut.icon", "Cut", "Couper", "Ctrl+X");
			this.MenuAdd(editMenu, @"file:images/copy.icon", "Copy", "Copier", "Ctrl+C");
			this.MenuAdd(editMenu, @"file:images/paste.icon", "Paste", "Coller", "Ctrl+V");
			this.MenuAdd(editMenu, @"", "", "", "");
			this.MenuAdd(editMenu, @"file:images/delete.icon", "Delete", "Supprimer", "Del");
			this.MenuAdd(editMenu, @"file:images/duplicate.icon", "Duplicate", "Dupliquer", "");
			editMenu.AdjustSize();
			this.menu.Items[1].Submenu = editMenu;

			VMenu objMenu = new VMenu();
			objMenu.Name = "Obj";
			objMenu.Host = this;
			this.MenuAdd(objMenu, @"file:images/deselect.icon", "Deselect", "Désélectionner tout", "");
			this.MenuAdd(objMenu, @"file:images/selectall.icon", "SelectAll", "Tout sélectionner", "");
			this.MenuAdd(objMenu, @"file:images/selectinvert.icon", "SelectInvert", "Inverser la sélection", "");
			this.MenuAdd(objMenu, @"file:images/selectglobal.icon", "SelectGlobal", "Changer le mode de sélection", "");
			this.MenuAdd(objMenu, @"", "", "", "");
			this.MenuAdd(objMenu, @"y/n", "HideHalf", "Mode estompé", "");
			this.MenuAdd(objMenu, @"file:images/hidesel.icon", "HideSel", "Cacher la sélection", "");
			this.MenuAdd(objMenu, @"file:images/hiderest.icon", "HideRest", "Cacher le reste", "");
			this.MenuAdd(objMenu, @"file:images/hidecancel.icon", "HideCancel", "Montrer tout", "");
			this.MenuAdd(objMenu, @"", "", "", "");
			this.MenuAdd(objMenu, @"file:images/orderup.icon", "OrderUp", "Dessus", "");
			this.MenuAdd(objMenu, @"file:images/orderdown.icon", "OrderDown", "Dessous", "");
			this.MenuAdd(objMenu, @"", "", "", "");
			this.MenuAdd(objMenu, @"file:images/groupempty.icon", "", "Groupe", "");
			objMenu.AdjustSize();
			this.menu.Items[2].Submenu = objMenu;

			VMenu groupMenu = new VMenu();
			groupMenu.Name = "Group";
			groupMenu.Host = this;
			this.MenuAdd(groupMenu, @"file:images/merge.icon", "Merge", "Fusionner", "");
			this.MenuAdd(groupMenu, @"file:images/group.icon", "Group", "Associer", "");
			this.MenuAdd(groupMenu, @"file:images/ungroup.icon", "Ungroup", "Dissocier", "");
			this.MenuAdd(groupMenu, @"", "", "", "");
			this.MenuAdd(groupMenu, @"file:images/inside.icon", "Inside", "Entrer dans le groupe", "");
			this.MenuAdd(groupMenu, @"file:images/outside.icon", "Outside", "Sortir du groupe", "");
			groupMenu.AdjustSize();
			objMenu.Items[13].Submenu = groupMenu;

			VMenu showMenu = new VMenu();
			showMenu.Name = "Show";
			showMenu.Host = this;
			this.MenuAdd(showMenu, @"file:images/selectmode.icon", "SelectMode", "Sélection partielle", "");
			this.MenuAdd(showMenu, @"file:images/preview.icon", "Preview", "Aperçu avant impression", "");
			this.MenuAdd(showMenu, @"file:images/grid.icon", "Grid", "Grille magnétique", "");
			this.MenuAdd(showMenu, @"file:images/mode.icon", "Mode", "Tableau des objets", "");
			this.MenuAdd(showMenu, @"", "", "", "");
			this.MenuAdd(showMenu, @"file:images/zoommenu.icon", "", "Zoom", "");
			this.MenuAdd(showMenu, @"", "", "", "");
			this.MenuAdd(showMenu, @"", "", "Apparence", "");
			showMenu.AdjustSize();
			this.menu.Items[3].Submenu = showMenu;

			VMenu zoomMenu = new VMenu();
			zoomMenu.Name = "Zoom";
			zoomMenu.Host = this;
			this.MenuAdd(zoomMenu, @"file:images/zoommin.icon", "ZoomMin", "Zoom minimal", "");
			this.MenuAdd(zoomMenu, @"file:images/zoomdefault.icon", "ZoomDefault", "Zoom 100%", "");
			this.MenuAdd(zoomMenu, @"file:images/zoomsel.icon", "ZoomSel", "Zoom sélection", "");
			this.MenuAdd(zoomMenu, @"file:images/zoomprev.icon", "ZoomPrev", "Zoom précédent", "");
			this.MenuAdd(zoomMenu, @"", "", "", "");
			this.MenuAdd(zoomMenu, @"file:images/zoomsub.icon", "ZoomSub", "Réduction", "");
			this.MenuAdd(zoomMenu, @"file:images/zoomadd.icon", "ZoomAdd", "Agrandissement", "");
			zoomMenu.AdjustSize();
			showMenu.Items[5].Submenu = zoomMenu;

			VMenu lookMenu = new VMenu();
			lookMenu.Name = "Look";
			lookMenu.Host = this;
			string[] list = Epsitec.Common.Widgets.Adorner.Factory.AdornerNames;
			foreach ( string name in list )
			{
				this.MenuAdd(lookMenu, @"y/n", "SelectLook(this.Name)", name, "", name);
			}
			lookMenu.AdjustSize();
			showMenu.Items[7].Submenu = lookMenu;

			VMenu arrayMenu = new VMenu();
			arrayMenu.Name = "Array";
			arrayMenu.Host = this;
			this.MenuAdd(arrayMenu, @"file:images/arrayframe.icon", "ArrayOutlineFrame", "Modifie le cadre", "");
			this.MenuAdd(arrayMenu, @"file:images/arrayhoriz.icon", "ArrayOutlineHoriz", "Modifie l'intérieur horizontal", "");
			this.MenuAdd(arrayMenu, @"file:images/arrayverti.icon", "ArrayOutlineVerti", "Modifie l'intérieur vertical", "");
			this.MenuAdd(arrayMenu, @"", "", "", "");
			this.MenuAdd(arrayMenu, @"", "", "Assistants", "");
			this.MenuAdd(arrayMenu, @"", "", "", "");
			this.MenuAdd(arrayMenu, @"", "ArrayAddColumnLeft", "Insérer des colonnes à gauche", "");
			this.MenuAdd(arrayMenu, @"", "ArrayAddColumnRight", "Insérer des colonnes à droite", "");
			this.MenuAdd(arrayMenu, @"", "ArrayAddRowTop", "Insérer des lignes en dessus", "");
			this.MenuAdd(arrayMenu, @"", "ArrayAddRowBottom", "Insérer des lignes en dessous", "");
			this.MenuAdd(arrayMenu, @"", "", "", "");
			this.MenuAdd(arrayMenu, @"", "ArrayDelColumn", "Supprimer les colonnes", "");
			this.MenuAdd(arrayMenu, @"", "ArrayDelRow", "Supprimer les lignes", "");
			this.MenuAdd(arrayMenu, @"", "", "", "");
			this.MenuAdd(arrayMenu, @"", "ArrayAlignColumn", "Egaliser les largeurs de colonne", "");
			this.MenuAdd(arrayMenu, @"", "ArrayAlignRow", "Egaliser les hauteurs de ligne", "");
			this.MenuAdd(arrayMenu, @"", "", "", "");
			this.MenuAdd(arrayMenu, @"", "ArraySwapColumn", "Permuter le contenu des colonnes", "");
			this.MenuAdd(arrayMenu, @"", "ArraySwapRow", "Permuter le contenu des lignes", "");
			arrayMenu.AdjustSize();
			this.menu.Items[4].Submenu = arrayMenu;

			VMenu arrayLookMenu = new VMenu();
			arrayLookMenu.Name = "ArrayLook";
			arrayLookMenu.Host = this;
			for ( int i=0 ; i<100 ; i++ )
			{
				string text, name;
				if ( !ObjectArray.CommandLook(i, out text, out name) )  break;
				if ( name == "" )
				{
					this.MenuAdd(arrayLookMenu, @"", "", "", "");
				}
				else
				{
					this.MenuAdd(arrayLookMenu, @"", "ArrayLook(this.Name)", text, "", name);
				}
			}
			arrayLookMenu.AdjustSize();
			arrayMenu.Items[4].Submenu = arrayLookMenu;

			VMenu debugMenu = new VMenu();
			debugMenu.Name = "Debug";
			debugMenu.Host = this;
			this.MenuAdd(debugMenu, @"y/n", "DebugBboxThin", "BBoxThin", "");
			this.MenuAdd(debugMenu, @"y/n", "DebugBboxGeom", "BBoxGeom", "");
			this.MenuAdd(debugMenu, @"y/n", "DebugBboxFull", "BBoxFull", "");
			debugMenu.AdjustSize();
			this.menu.Items[5].Submenu = debugMenu;

			VMenu helpMenu = new VMenu();
			helpMenu.Name = "Help";
			helpMenu.Host = this;
			helpMenu.Items.Add(new MenuItem("help", "", "Aide", "F1"));
			helpMenu.Items.Add(new MenuItem("ctxhelp", "", "Aide contextuelle", ""));
			helpMenu.Items.Add(new MenuItem("about", "", "A propos de...", ""));
			helpMenu.AdjustSize();
			this.menu.Items[6].Submenu = helpMenu;

			this.hToolBar = new HToolBar();
			this.hToolBar.Parent = this;
			this.HToolBarAdd(@"file:images/new.icon", "New", "Nouveau");
			this.HToolBarAdd(@"file:images/open.icon", "Open", "Ouvrir");
			this.HToolBarAdd(@"file:images/save.icon", "Save", "Enregistrer");
			this.HToolBarAdd(@"file:images/print.icon", "Print", "Imprimer");
			this.HToolBarAdd("", "", "");
			this.HToolBarAdd(@"file:images/delete.icon", "Delete", "Supprimer");
			this.HToolBarAdd(@"file:images/duplicate.icon", "Duplicate", "Dupliquer");
			this.HToolBarAdd("", "", "");
			this.HToolBarAdd(@"file:images/cut.icon", "Cut", "Couper");
			this.HToolBarAdd(@"file:images/copy.icon", "Copy", "Copier");
			this.HToolBarAdd(@"file:images/paste.icon", "Paste", "Coller");
			this.HToolBarAdd("", "", "");
			this.HToolBarAdd(@"file:images/undo.icon", "Undo", "Annuler");
			this.HToolBarAdd(@"file:images/redo.icon", "Redo", "Refaire");
			this.HToolBarAdd("", "", "");
			this.HToolBarAdd(@"file:images/orderup.icon", "OrderUp", "Dessus");
			this.HToolBarAdd(@"file:images/orderdown.icon", "OrderDown", "Dessous");
			this.HToolBarAdd("", "", "");
			this.HToolBarAdd(@"file:images/merge.icon", "Merge", "Fusionner");
			this.HToolBarAdd(@"file:images/group.icon", "Group", "Associer");
			this.HToolBarAdd(@"file:images/ungroup.icon", "Ungroup", "Dissocier");
			this.HToolBarAdd(@"file:images/inside.icon", "Inside", "Entrer dans le groupe");
			this.HToolBarAdd(@"file:images/outside.icon", "Outside", "Sortir du groupe");
			this.HToolBarAdd("", "", "");
			this.HToolBarAdd(@"file:images/selectmode.icon", "SelectMode", "Sélection partielle");
			this.HToolBarAdd(@"file:images/preview.icon", "Preview", "Aperçu avant impression");
			this.HToolBarAdd(@"file:images/grid.icon", "Grid", "Grille magnétique");
			this.HToolBarAdd(@"file:images/mode.icon", "Mode", "Tableau des objets");
			this.HToolBarAdd("", "", "");
			this.HToolBarAdd(@"file:images/arrayframe.icon", "ArrayOutlineFrame", "Modifie le cadre");
			this.HToolBarAdd(@"file:images/arrayhoriz.icon", "ArrayOutlineHoriz", "Modifie l'intérieur horizontal");
			this.HToolBarAdd(@"file:images/arrayverti.icon", "ArrayOutlineVerti", "Modifie l'intérieur vertical");
			this.HToolBarAdd("", "", "");

			this.vToolBar = new VToolBar();
			this.vToolBar.Parent = this;
			this.VToolBarAdd(@"file:images/select.icon", "SelectTool(this.Name)", "Sélectionner", "Select");
			this.VToolBarAdd(@"file:images/edit.icon", "SelectTool(this.Name)", "Editer", "Edit");
			this.VToolBarAdd(@"file:images/zoom.icon", "SelectTool(this.Name)", "Agrandir", "Zoom");
			this.VToolBarAdd(@"file:images/hand.icon", "SelectTool(this.Name)", "Déplacer", "Hand");
			this.VToolBarAdd(@"file:images/picker.icon", "SelectTool(this.Name)", "Pipette", "Picker");
			this.VToolBarAdd("", "", "");
			this.VToolBarAdd(@"file:images/line.icon", "SelectTool(this.Name)", "Segment de ligne", "ObjectLine");
			this.VToolBarAdd(@"file:images/rectangle.icon", "SelectTool(this.Name)", "Rectangle", "ObjectRectangle");
			this.VToolBarAdd(@"file:images/circle.icon", "SelectTool(this.Name)", "Cercle", "ObjectCircle");
			this.VToolBarAdd(@"file:images/ellipse.icon", "SelectTool(this.Name)", "Ellipse", "ObjectEllipse");
			this.VToolBarAdd(@"file:images/regular.icon", "SelectTool(this.Name)", "Polygone régulier", "ObjectRegular");
			this.VToolBarAdd(@"file:images/poly.icon", "SelectTool(this.Name)", "Polygone quelconque", "ObjectPoly");
			this.VToolBarAdd(@"file:images/bezier.icon", "SelectTool(this.Name)", "Courbes de Bézier", "ObjectBezier");
			this.VToolBarAdd(@"file:images/textline.icon", "SelectTool(this.Name)", "Ligne de texte", "ObjectTextLine");
			this.VToolBarAdd(@"file:images/textbox.icon", "SelectTool(this.Name)", "Pavé de texte", "ObjectTextBox");
			this.VToolBarAdd(@"file:images/array.icon", "SelectTool(this.Name)", "Tableau", "ObjectArray");
			this.VToolBarAdd(@"file:images/image.icon", "SelectTool(this.Name)", "Image bitmap", "ObjectImage");
			this.VToolBarAdd("", "", "");
			
			this.root = new Widget();
			this.root.Parent = this;
			
			this.pane = new PaneBook();
			this.pane.PaneBookStyle = PaneBookStyle.LeftRight;
			this.pane.PaneBehaviour = PaneBookBehaviour.FollowMe;
			this.pane.PaneSizeChanged += new EventHandler(this.HandlePaneSizeChanged);
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

			this.book = new TabBook();
			this.book.Arrows = TabBookArrows.Stretch;
			this.book.ActivePageChanged += new EventHandler(this.HandleActivePageChanged);
			this.book.Parent = this;

			this.bookPanel = new TabPage();
			this.bookPanel.TabTitle = "Attributs";
			this.book.Items.Add(this.bookPanel);

			this.bookStyles = new TabPage();
			this.bookStyles.TabTitle = "Styles";
			this.book.Items.Add(this.bookStyles);

			this.bookPatterns = new TabPage();
			this.bookPatterns.TabTitle = "Motifs";
			this.book.Items.Add(this.bookPatterns);

			this.bookPages = new TabPage();
			this.bookPages.TabTitle = "Pages";
			this.book.Items.Add(this.bookPages);

			this.bookLayers = new TabPage();
			this.bookLayers.TabTitle = "Calques";
			this.book.Items.Add(this.bookLayers);

			this.book.ActivePage = this.bookPanel;

			this.panel = new Widget();
			this.panel.Parent = this.bookPanel;

			this.drawer = new Drawer();
			this.drawer.InitCommands(this.CommandDispatcher);
			this.CommandDispatcher.RegisterController(this.drawer);
			this.drawer.IsIconEditable = true;
			this.drawer.SelectedTool = "Select";
			this.drawer.PanelChanged += new EventHandler(this.HandleDrawerPanelChanged);
			this.drawer.CommandChanged += new EventHandler(this.HandleDrawerCommandChanged);
			this.drawer.AllChanged += new EventHandler(this.HandleDrawerAllChanged);
			this.drawer.ScrollerChanged += new EventHandler(this.HandleDrawerScrollerChanged);
			this.drawer.InfoDocumentChanged += new EventHandler(this.HandleDrawerInfoDocumentChanged);
			this.drawer.InfoObjectChanged += new EventHandler(this.HandleDrawerInfoObjectChanged);
			this.drawer.InfoMouseChanged += new EventHandler(this.HandleDrawerInfoMouseChanged);
			this.drawer.InfoZoomChanged += new EventHandler(this.HandleDrawerInfoZoomChanged);
			this.drawer.UsePropertiesPanel += new EventHandler(this.HandleDrawerUsePropertiesPanel);
			this.drawer.PageLayerChanged += new EventHandler(this.HandleDrawerPageLayerChanged);
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

			this.quickPagePrev = new GlyphButton();
			this.quickPagePrev.GlyphShape = GlyphShape.ArrowLeft;
			this.quickPagePrev.Clicked += new MessageEventHandler(this.HandleQuickPagePrev);
			this.quickPagePrev.Parent = this.leftPane;
			ToolTip.Default.SetToolTip(this.quickPagePrev, "Page précédente");

			this.quickPageMenu = new Button();
			this.quickPageMenu.Text = "1";
			this.quickPageMenu.Clicked += new MessageEventHandler(this.HandleQuickPageMenu);
			this.quickPageMenu.Parent = this.leftPane;
			ToolTip.Default.SetToolTip(this.quickPageMenu, "Choix d'une page");

			this.quickPageNext = new GlyphButton();
			this.quickPageNext.GlyphShape = GlyphShape.ArrowRight;
			this.quickPageNext.Clicked += new MessageEventHandler(this.HandleQuickPageNext);
			this.quickPageNext.Parent = this.leftPane;
			ToolTip.Default.SetToolTip(this.quickPageNext, "Page suivante");

			this.hScroller = new HScroller();
			this.hScroller.ValueChanged += new EventHandler(this.HandleHScrollerValueChanged);
			this.hScroller.Parent = this.leftPane;

			this.quickLayerNext = new GlyphButton();
			this.quickLayerNext.GlyphShape = GlyphShape.ArrowUp;
			this.quickLayerNext.Clicked += new MessageEventHandler(this.HandleQuickLayerNext);
			this.quickLayerNext.Parent = this.leftPane;
			ToolTip.Default.SetToolTip(this.quickLayerNext, "Calque suivant");

			this.quickLayerMenu = new Button();
			this.quickLayerMenu.Text = "A";
			this.quickLayerMenu.Clicked += new MessageEventHandler(this.HandleQuickLayerMenu);
			this.quickLayerMenu.Parent = this.leftPane;
			ToolTip.Default.SetToolTip(this.quickLayerMenu, "Choix d'un calque");

			this.quickLayerPrev = new GlyphButton();
			this.quickLayerPrev.GlyphShape = GlyphShape.ArrowDown;
			this.quickLayerPrev.Clicked += new MessageEventHandler(this.HandleQuickLayerPrev);
			this.quickLayerPrev.Parent = this.leftPane;
			ToolTip.Default.SetToolTip(this.quickLayerPrev, "Calque précédent");

			this.vScroller = new VScroller();
			this.vScroller.ValueChanged += new EventHandler(this.HandleVScrollerValueChanged);
			this.vScroller.Parent = this.leftPane;

			this.panelStyles = new PanelStyles(this.drawer);
			this.panelStyles.Parent = this.bookStyles;

			this.panelPatterns = new PanelPatterns(this.drawer);
			this.panelPatterns.ObjectsChanged += new EventHandler(this.HandlePanelPatternsObjectsChanged);
			this.panelPatterns.Parent = this.bookPatterns;

			this.panelPages = new PanelPages(this.drawer);
			this.panelPages.ObjectsChanged += new EventHandler(this.HandlePanelPagesObjectsChanged);
			this.panelPages.Parent = this.bookPages;

			this.panelLayers = new PanelLayers(this.drawer);
			this.panelLayers.ObjectsChanged += new EventHandler(this.HandlePanelLayersObjectsChanged);
			this.panelLayers.Parent = this.bookLayers;

			this.info = new StatusBar();
			this.info.Parent = this;
			this.InfoAdd("", 200, "StatusDocument", "");
			this.InfoAdd(@"file:images/deselect.icon", 0, "Deselect", "Désélectionner tout");
			this.InfoAdd(@"file:images/selectall.icon", 0, "SelectAll", "Tout sélectionner");
			this.InfoAdd(@"file:images/selectinvert.icon", 0, "SelectInvert", "Inverser la sélection");
			this.InfoAdd(@"file:images/selectglobal.icon", 0, "SelectGlobal", "Changer le mode de sélection");
			this.InfoAdd(@"file:images/hidesel.icon", 0, "HideSel", "Cacher la sélection");
			this.InfoAdd(@"file:images/hiderest.icon", 0, "HideRest", "Cacher le reste");
			this.InfoAdd(@"file:images/hidecancel.icon", 0, "HideCancel", "Montrer tout");
			this.InfoAdd("", 120, "StatusObject", "");
			this.InfoAdd(@"file:images/zoommin.icon", 0, "ZoomMin", "Zoom minimal");
			this.InfoAdd(@"file:images/zoomdefault.icon", 0, "ZoomDefault", "Zoom 100%");
			this.InfoAdd(@"file:images/zoomsel.icon", 0, "ZoomSel", "Zoom sélection");
			this.InfoAdd(@"file:images/zoomprev.icon", 0, "ZoomPrev", "Zoom précédent");
			this.InfoAdd(@"file:images/zoomsub.icon", 0, "ZoomSub", "Réduction");
			this.InfoAdd(@"file:images/zoomadd.icon", 0, "ZoomAdd", "Agrandissement");
			this.InfoAdd("", 90, "StatusZoom", "");
			this.InfoAdd("", 120, "StatusMouse", "");
			StatusField infoField = this.info.Items["StatusMouse"] as StatusField;

			this.allWidgets = true;
			this.ResizeLayout();
			this.drawer.UpdateCommands();
		}

		public HMenu GetMenu()
		{
			return this.menu;
		}

		private void HandleActivePageChanged(object sender)
		{
			if ( this.drawer != null )
			{
				this.drawer.CreateEnding();
			}
			this.UpdatePanels();
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

			this.hScroller.MinValue = (decimal) ((size.Width-area.Width)/2);
			this.hScroller.MaxValue = (decimal) ((area.Width+(double)this.hScroller.MinValue) - size.Width/this.drawer.Zoom);
			this.hScroller.VisibleRangeRatio = (decimal) ((size.Width/area.Width)/this.drawer.Zoom);
			this.hScroller.Value = (decimal) (-this.drawer.OriginX);
			this.drawer.OriginX = (double) (-this.hScroller.Value);

			this.vScroller.MinValue = (decimal) ((size.Height-area.Height)/2);
			this.vScroller.MaxValue = (decimal) ((area.Height+(double)this.vScroller.MinValue) - size.Height/this.drawer.Zoom);
			this.vScroller.VisibleRangeRatio = (decimal) ((size.Height/area.Height)/this.drawer.Zoom);
			this.vScroller.Value = (decimal) (-this.drawer.OriginY);
			this.drawer.OriginY = (double) (-this.vScroller.Value);
		}

		// Bouton "page précédente" cliqué.
		private void HandleQuickPagePrev(object sender, MessageEventArgs e)
		{
			if ( this.drawer.IconObjects.CurrentPage > 0 )
			{
				this.panelPages.PageSelect(this.drawer.IconObjects.CurrentPage-1);
			}
		}

		// Bouton "page suivante" cliqué.
		private void HandleQuickPageNext(object sender, MessageEventArgs e)
		{
			if ( this.drawer.IconObjects.CurrentPage < this.drawer.IconObjects.TotalPages()-1 )
			{
				this.panelPages.PageSelect(this.drawer.IconObjects.CurrentPage+1);
			}
		}

		// Bouton "menu des pages" cliqué.
		private void HandleQuickPageMenu(object sender, MessageEventArgs e)
		{
			Button button = sender as Button;
			Drawing.Point pos = button.MapClientToScreen(new Drawing.Point(0, button.Height));
			VMenu menu = this.panelPages.CreateMenu();
			menu.Host = this;
			pos.Y += menu.Height;
			menu.ShowAsContextMenu(this.Window, pos);
		}

		[Command ("SelectPage")]
		void CommandSelectPage(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			int sel = System.Convert.ToInt32(e.CommandArgs[0]);
			this.panelPages.PageSelect(sel);
		}

		// Bouton "calque précédent" cliqué.
		private void HandleQuickLayerPrev(object sender, MessageEventArgs e)
		{
			if ( this.drawer.IconObjects.CurrentLayer > 0 )
			{
				this.panelLayers.LayerSelect(this.drawer.IconObjects.CurrentLayer-1);
			}
		}

		// Bouton "calque suivant" cliqué.
		private void HandleQuickLayerNext(object sender, MessageEventArgs e)
		{
			if ( this.drawer.IconObjects.CurrentLayer < this.drawer.IconObjects.TotalLayers()-1 )
			{
				this.panelLayers.LayerSelect(this.drawer.IconObjects.CurrentLayer+1);
			}
		}

		// Bouton "menu des calques" cliqué.
		private void HandleQuickLayerMenu(object sender, MessageEventArgs e)
		{
			Button button = sender as Button;
			Drawing.Point pos = button.MapClientToScreen(new Drawing.Point(0, button.Height));
			VMenu menu = this.panelLayers.CreateMenu();
			menu.Host = this;
			pos.X -= menu.Width;
			menu.ShowAsContextMenu(this.Window, pos);
		}

		[Command ("SelectLayer")]
		void CommandSelectLayer(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			int sel = System.Convert.ToInt32(e.CommandArgs[0]);
			this.panelLayers.LayerSelect(sel);
		}

		// Met à jour les boutons pour les pages et les calques.
		protected void UpdatePagesLayers()
		{
			int cp = this.drawer.IconObjects.CurrentPage;
			int tp = this.drawer.IconObjects.TotalPages();
			this.quickPagePrev.SetEnabled(cp > 0);
			this.quickPageNext.SetEnabled(cp < tp-1);
			this.quickPageMenu.SetEnabled(tp > 1);
			this.quickPageMenu.Text = (cp+1).ToString();

			int cl = this.drawer.IconObjects.CurrentLayer;
			int tl = this.drawer.IconObjects.TotalLayers();
			this.quickLayerPrev.SetEnabled(cl > 0);
			this.quickLayerNext.SetEnabled(cl < tl-1);
			this.quickLayerMenu.SetEnabled(tl > 1);
			this.quickLayerMenu.Text = ((char)('A'+(tl-cl-1))).ToString();
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
				MenuItem item;
				
				if ( icon == "y/n" )
				{
					item = MenuItem.CreateYesNo(command, text, shortcut, name);
				}
				else
				{
					item = new MenuItem(command, icon, text, shortcut, name);
				}
				
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
				ToolTip.Default.SetToolTip(button, tooltip);
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
				ToolTip.Default.SetToolTip(button, tooltip);
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
				ToolTip.Default.SetToolTip(button, tooltip);
			}
		}

		private void HandleDrawerInfoDocumentChanged(object sender)
		{
			StatusField field = this.info.Items["StatusDocument"] as StatusField;
			string name = Misc.ExtractName(this.filename);
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

		private void HandleDrawerUsePropertiesPanel(object sender)
		{
			this.book.ActivePage = this.bookPanel;
		}

		private void HandleDrawerPageLayerChanged(object sender)
		{
			this.panelPatterns.Update();
			this.panelPages.Update();
			this.panelLayers.Update();
			this.UpdatePagesLayers();
		}

		private void HandleHScrollerValueChanged(object sender)
		{
			this.drawer.OriginX = (double) -this.hScroller.Value;
		}

		private void HandleVScrollerValueChanged(object sender)
		{
			this.drawer.OriginY = (double) -this.vScroller.Value;
		}

		private void HandlePanelPatternsObjectsChanged(object sender)
		{
			this.drawer.PageOrLayerChanged();
			this.drawer.InvalidateAll();
			this.drawer.UpdateCommands();
			this.HandleDrawerInfoObjectChanged(null);
			this.panelPages.Update();
			this.panelLayers.Update();
			this.UpdatePagesLayers();
			this.bookStyles.SetEnabled(this.drawer.IconObjects.CurrentPattern == 0);
		}

		private void HandlePanelPagesObjectsChanged(object sender)
		{
			this.drawer.PageOrLayerChanged();
			this.drawer.InvalidateAll();
			this.drawer.UpdateCommands();
			this.HandleDrawerInfoObjectChanged(null);
			this.panelLayers.Update();
			this.UpdatePagesLayers();
		}

		private void HandlePanelLayersObjectsChanged(object sender)
		{
			this.drawer.PageOrLayerChanged();
			this.drawer.InvalidateAll();
			this.drawer.UpdateCommands();
			this.HandleDrawerInfoObjectChanged(null);
			this.UpdatePagesLayers();
		}

		// Met à jour les panneaux des propriétés de droite en fonction de l'objet.
		protected void UpdatePanels()
		{
			if ( this.panel == null )  return;

			// Supprime tous les panneaux, sauf le ColorSelector.
			AbstractPanel panel;
			int i = 0;
			while ( i < this.panel.Children.Count )
			{
				if ( this.panel.Children[i] is ColorSelector )
				{
					i ++;
				}
				else
				{
					panel = this.panel.Children[i] as AbstractPanel;
					if ( panel != null )
					{
						panel.Changed -= new EventHandler(this.HandlePanelChanged);
						panel.ExtendedChanged -= new EventHandler(this.HandleExtendedChanged);
						panel.OriginColorChanged -= new EventHandler(this.HandleOriginColorChanged);
					}
					this.panel.Children[i].Dispose();
				}
			}

			// Crée une fois pour toutes le ColorSelector.
			if ( this.colorSelector == null )
			{
				this.colorSelector = new ColorSelector();
				this.colorSelector.Anchor = AnchorStyles.LeftAndRight|AnchorStyles.Bottom;
				this.colorSelector.Changed += new EventHandler(this.HandleColorSelectorChanged);
				this.colorSelector.TabIndex = 100;
				this.colorSelector.TabNavigation = Widget.TabNavigationMode.ActivateOnTab | Widget.TabNavigationMode.ForwardToChildren | Widget.TabNavigationMode.ForwardOnly;
				this.colorSelector.Parent = this.panel;
			}

			// Crée tous les panneaux.
			Widget originColorLastPanel = null;

			AbstractObject creatingObject = this.drawer.CreatingObject;
			if ( creatingObject == null )
			{
				System.Collections.ArrayList list = this.drawer.PropertiesList();
				int index = 0;
				foreach ( AbstractProperty property in list )
				{
					panel = property.CreatePanel(this.drawer);
					panel.ExtendedSize = this.drawer.GetPropertyExtended(property.Type);
					panel.Multi = property.Multi;
					panel.LayoutDirect = (property.Type == PropertyType.Name);
					panel.PatternDirect = (this.drawer.IconObjects.CurrentPattern != 0);

					AbstractProperty p = this.drawer.GetProperty(property.Type);
					panel.SetProperty(p);

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
				}

				foreach ( Widget widget in this.panel.Children )
				{
					panel = widget as AbstractPanel;
					if ( panel == null ) continue;
					if ( panel.DefaultFocus() )  break;;
				}
			}
			else	// objet en cours de création ?
			{
				for ( i=0 ; i<100 ; i++ )
				{
					string cmd, name, text;
					if ( !creatingObject.CreateAction(i, out cmd, out name, out text) )  break;
					Button button = new Button();
					button.Command = cmd;
					button.Name = name;
					button.Text = text;
					button.Parent = this.panel;
				}
			}
			this.GeometryPanels();

			this.HandleOriginColorChanged(originColorLastPanel, true);
			this.HandleDrawerScrollerChanged(null);
			this.HandleDrawerInfoDocumentChanged(null);
			this.HandleDrawerInfoObjectChanged(null);
			this.HandleDrawerInfoMouseChanged(null);
			this.HandleDrawerInfoZoomChanged(null);
		}

		// Repositionne tous les panels.
		protected void GeometryPanels()
		{
			if ( this.colorSelector == null )  return;

			Drawing.Rectangle rect = new Drawing.Rectangle();
			double posy = this.panel.Height-1;

			AbstractObject creatingObject = this.drawer.CreatingObject;
			if ( creatingObject == null )
			{
				double lastBack = -1;
				foreach ( Widget widget in this.panel.Children )
				{
					AbstractPanel panel = widget as AbstractPanel;
					if ( panel == null )  continue;

					AbstractProperty property = panel.GetProperty();

					if ( lastBack != -1 && lastBack != property.BackgroundIntensity )
					{
						posy -= 5;
					}
					lastBack = property.BackgroundIntensity;

					rect.Left   = 1;
					rect.Right  = this.panel.Width-1;
					rect.Bottom = posy-panel.DefaultHeight;
					rect.Top    = posy;
					panel.Bounds = rect;

					posy -= rect.Height;
				}
			}
			else
			{
				posy -= 50;
				foreach ( Widget widget in this.panel.Children )
				{
					Button button = widget as Button;
					if ( button == null )  continue;

					rect.Left   = 1;
					rect.Right  = this.panel.Width-1;
					rect.Bottom = posy-button.DefaultHeight;
					rect.Top    = posy;
					button.Bounds = rect;

					posy -= rect.Height+10;
				}
			}
			this.leftHeightUsed = this.panel.Height-posy;

			// Positionne le ColorSelector.
			rect.Left   = 0;
			rect.Right  = this.panel.Width;
			rect.Bottom = 0;
			rect.Top    = System.Math.Min(this.colorSelector.DefaultHeight, this.panel.Height-this.leftHeightUsed);
			this.colorSelector.Bounds = rect;
		}

		// Le contenu d'un panneau a été changé.
		private void HandlePanelChanged(object sender)
		{
			AbstractPanel panel = sender as AbstractPanel;
			AbstractProperty property = panel.GetProperty();
			this.drawer.SetProperty(property, true);
			panel.Multi = false;
		}

		// La hauteur d'un panneau a été changée.
		private void HandleExtendedChanged(object sender)
		{
			AbstractPanel panel = sender as AbstractPanel;
			AbstractProperty property = panel.GetProperty();
			this.drawer.SetPropertyExtended(property.Type, panel.ExtendedSize);
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
			Drawing.Color backColor = Drawing.Color.Empty;

			foreach ( Widget widget in this.panel.Children )
			{
				AbstractPanel panel = widget as AbstractPanel;
				if ( panel == null )  continue;
				Widget wSender = sender as Widget;
				if ( panel == wSender )
				{
					this.originColorPanel = panel;
					panel.OriginColorSelect( lastOrigin ? this.originColorRank : -1 );
					if ( panel.GetProperty().StyleID != 0 )
					{
						backColor = IconContext.ColorStyleBack;
					}
				}
				else
				{
					panel.OriginColorDeselect();
				}
			}

			if ( this.originColorPanel == null )
			{
				this.colorSelector.SetEnabled(false);
				this.colorSelector.BackColor = Drawing.Color.Empty;
			}
			else
			{
				this.colorSelector.SetEnabled(true);
				this.colorSelector.BackColor = backColor;
				this.ignoreColorChanged = true;
				this.colorSelector.Color = this.originColorPanel.OriginColorGet();
				this.ignoreColorChanged = false;
				this.originColorType = this.originColorPanel.PropertyType;
				this.originColorRank = this.originColorPanel.OriginColorRank();
			}
		}

		// Couleur changée dans la roue.
		private void HandleColorSelectorChanged(object sender)
		{
			if ( this.ignoreColorChanged || this.originColorPanel == null )  return;
			this.originColorPanel.OriginColorChange(this.colorSelector.Color);

			AbstractProperty property = this.originColorPanel.GetProperty();
			this.drawer.SetProperty(property, true);
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

			double pw = this.panelsWidth+12+1;

			this.book.Location = new Drawing.Point(rect.Right-pw, this.info.Height+1);
			this.book.Size = new Drawing.Size(pw-1, rect.Height-this.info.Height-this.hToolBar.DefaultHeight-2);
			Drawing.Rectangle inside = this.book.InnerBounds;
			this.bookPanel.Bounds = inside;
			this.bookStyles.Bounds = inside;
			inside.Left += 1;
			inside.Width = this.panelsWidth+2;
			inside.Top -= 10;
			this.panel.Bounds = inside;
			this.panelStyles.Bounds = inside;
			this.panelPatterns.Bounds = inside;
			this.panelPages.Bounds = inside;
			this.panelLayers.Bounds = inside;

			this.root.Location = new Drawing.Point(this.vToolBar.DefaultWidth, this.info.Height);
			this.root.Size = new Drawing.Size(rect.Width-this.vToolBar.DefaultWidth-pw, rect.Height-this.info.Height-this.hToolBar.DefaultHeight);

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
			this.drawer.Location = new Drawing.Point(10, this.leftPane.Height-10-dimy+1);
			this.drawer.Size = new Drawing.Size(dimx-1, dimy-1);

			rect.Bottom = this.leftPane.Height-10-dimy-this.hScroller.DefaultHeight;
			rect.Height = this.hScroller.DefaultHeight;
			rect.Left   = 10;
			rect.Width  = this.hScroller.DefaultHeight;
			this.quickPagePrev.Bounds = rect;
			rect.Offset(rect.Width, 0);
			rect.Width += this.hScroller.DefaultHeight*0.5;
			this.quickPageMenu.Bounds = rect;
			rect.Offset(rect.Width, 0);
			rect.Width -= this.hScroller.DefaultHeight*0.5;
			this.quickPageNext.Bounds = rect;
			rect.Left   = 10+this.hScroller.DefaultHeight*3.7;
			rect.Width  = dimx-1-this.hScroller.DefaultHeight*3.7;
			this.hScroller.Bounds = rect;

			rect.Left   = 10+dimx;
			rect.Width  = this.vScroller.DefaultWidth;
			rect.Bottom = this.leftPane.Height-10-this.vScroller.DefaultWidth;
			rect.Height = this.vScroller.DefaultWidth;
			this.quickLayerNext.Bounds = rect;
			rect.Offset(0, -rect.Height);
			this.quickLayerMenu.Bounds = rect;
			rect.Offset(0, -rect.Height);
			this.quickLayerPrev.Bounds = rect;
			rect.Bottom = this.leftPane.Height-10-dimy+1;
			rect.Height = dimy-1-this.vScroller.DefaultWidth*3.2;
			this.vScroller.Bounds = rect;

			this.lister.Location = new Drawing.Point(10, 10);
			this.lister.Size = new Drawing.Size(this.leftPane.Width-20, this.leftPane.Height-20);

			dimx = this.rightPane.Width-20;
			dimy = dimx*iconSize.Height/iconSize.Width;
			rect.Left   = 10;
			rect.Bottom = this.rightPane.Height-10-dimy-1;
			rect.Width  = dimx;
			rect.Height = dimy;
			rect.Inflate(1);
			this.frame1.Bounds = rect;
			rect.Deflate(1);

			rect.Offset(0, -dimy-10);
			rect.Inflate(1);
			this.frame2.Bounds = rect;
			rect.Deflate(1);

			this.GeometryPanels();
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
				this.drawer.SelectedTool = e.CommandArgs[0];
			}
			else
			{
				this.drawer.SelectedTool = "Select";
			}
			
			this.book.ActivePage = this.bookPanel;
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
			this.HandleDrawerInfoDocumentChanged(null);
			this.panelPatterns.Update();
			this.panelPages.Update();
			this.panelLayers.Update();
			this.UpdatePagesLayers();
		}

		[Command ("Open")]
		void CommandOpen(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			FileOpen dialog = new FileOpen();
		
			dialog.Title = "Ouvrir une icone";
			dialog.FileName = this.filename;
			dialog.Filters.Add("icon", "Icônes", "*.icon");
			dialog.Show();
			if ( dialog.Result != Dialogs.DialogResult.Accept )  return;

			this.filename = dialog.FileName;
			this.HandleDrawerInfoDocumentChanged(null);

			this.drawer.CommandOpen(this.filename);

			this.ResizeLayout();
			this.Invalidate();
			this.HandleDrawerInfoDocumentChanged(null);
			this.panelPatterns.Update();
			this.panelPages.Update();
			this.panelLayers.Update();
			this.UpdatePagesLayers();
		}

		[Command ("Save")]
		void CommandSave(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			FileSave dialog = new FileSave();
			
			dialog.Title = "Enregisrter une icone";
			dialog.FileName = this.filename;
			dialog.Filters.Add("icon", "Icônes", "*.icon");
			dialog.Show();
			if ( dialog.Result != Dialogs.DialogResult.Accept )  return;

			this.filename = dialog.FileName;
			this.HandleDrawerInfoDocumentChanged(null);

			this.drawer.CommandSave(this.filename);
		}
		
		[Command ("Print")]
		void CommandPrint(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			Print dialog = new Print();
			
			dialog.AllowFromPageToPage = false;
			dialog.AllowSelectedPages  = false;
			
			string[] printers = Printing.PrinterSettings.InstalledPrinters;
			
			dialog.Document.PrinterSettings.MinimumPage = 1;
			dialog.Document.PrinterSettings.MaximumPage = 1;
			dialog.Document.PrinterSettings.FromPage = 1;
			dialog.Document.PrinterSettings.ToPage = 1;
			dialog.Document.PrinterSettings.PrintRange = Printing.PrintRange.AllPages;
			dialog.Document.PrinterSettings.Collate = false;
			dialog.Show();
			if ( dialog.Result != Dialogs.DialogResult.Accept )  return;

			this.drawer.Print(dialog);
		}
		
		[Command ("QuitApplication")]
		void CommandQuitApplication(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			Window.Quit();
		}

		[Command ("Mode")]
		void CommandMode(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			if ( this.drawer.IsIconActive )
			{
				this.drawer.IsIconActive = false;
				this.drawer.Hide();
				this.quickPageNext.Hide();
				this.quickPageMenu.Hide();
				this.quickPagePrev.Hide();
				this.hScroller.Hide();
				this.quickLayerPrev.Hide();
				this.quickLayerMenu.Hide();
				this.quickLayerNext.Hide();
				this.vScroller.Hide();
				this.lister.Show();

				this.drawer.SelectedTool = "Select";
				this.UpdatePanels();
			}
			else
			{
				this.drawer.IsIconActive = true;
				this.drawer.Show();
				this.quickPageNext.Show();
				this.quickPageMenu.Show();
				this.quickPagePrev.Show();
				this.hScroller.Show();
				this.quickLayerPrev.Show();
				this.quickLayerMenu.Show();
				this.quickLayerNext.Show();
				this.vScroller.Show();
				this.lister.Hide();
			}
			this.drawer.UpdateCommands();
			this.Invalidate();
		}

		// TODO: pourquoi cette méthode ne peut pas être directement dans PanelStyles ?
		[Command ("StyleCreate")]
		void CommandStyleCreate(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.drawer.UndoBeginning("StyleCreate");
			this.drawer.UndoSelectionWillBeChanged();
			this.drawer.UndoValidate();
			PropertyType type = AbstractProperty.TypeName(e.CommandArgs[0]);
			this.panelStyles.CommandStyleCreate(type);
		}

		[Command ("StyleMake")]
		void CommandStyleMake(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.drawer.UndoBeginning("StyleMake");
			this.drawer.UndoSelectionWillBeChanged();
			this.drawer.UndoValidate();
			PropertyType type = AbstractProperty.TypeName(e.CommandArgs[0]);
			int sel = this.drawer.StyleMake(type);
			if ( sel == -1 )  return;
			this.panelStyles.UpdateAll(sel);
			this.UpdatePanels();
		}

		[Command ("StyleFree")]
		void CommandStyleFree(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.drawer.UndoBeginning("StyleFree");
			this.drawer.UndoSelectionWillBeChanged();
			this.drawer.UndoValidate();
			PropertyType type = AbstractProperty.TypeName(e.CommandArgs[0]);
			this.drawer.StyleFree(type);
			this.UpdatePanels();
		}

		[Command ("StyleUse")]
		void CommandStyleUse(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.drawer.UndoBeginning("StyleUse");
			this.drawer.UndoSelectionWillBeChanged();
			this.drawer.UndoValidate();
			int rank = System.Convert.ToInt32(e.CommandArgs[0]);
			this.drawer.StyleUse(rank);
			this.UpdatePanels();
		}


		protected CommandDispatcher				commandDispatcher;
		protected bool							allWidgets = false;
		protected HMenu							menu;
		protected HToolBar						hToolBar;
		protected VToolBar						vToolBar;
		protected Widget						root;
		protected StatusBar						info;
		protected PaneBook						pane;
		protected PanePage						leftPane;
		protected PanePage						rightPane;
		protected TabBook						book;
		protected TabPage						bookPanel;
		protected TabPage						bookStyles;
		protected TabPage						bookPatterns;
		protected TabPage						bookPages;
		protected TabPage						bookLayers;
		protected Widget						panel;
		protected Drawer						drawer;
		protected GlyphButton					quickPagePrev;
		protected Button						quickPageMenu;
		protected GlyphButton					quickPageNext;
		protected HScroller						hScroller;
		protected GlyphButton					quickLayerPrev;
		protected Button						quickLayerMenu;
		protected GlyphButton					quickLayerNext;
		protected VScroller						vScroller;
		protected Lister						lister;
		protected SampleButton					frame1;
		protected SampleButton					frame2;
		protected ColorSelector					colorSelector;
		protected PanelStyles					panelStyles;
		protected PanelPatterns					panelPatterns;
		protected PanelPages					panelPages;
		protected PanelLayers					panelLayers;
		protected AbstractPanel					originColorPanel = null;
		protected PropertyType					originColorType = PropertyType.None;
		protected int							originColorRank = -1;
		protected double						leftHeightUsed = 0;
		protected string						filename = "";
		protected bool							ignoreColorChanged = false;
		protected double						panelsWidth = 215;

		public static new OpletQueue			OpletQueue = new OpletQueue();
	}
}
