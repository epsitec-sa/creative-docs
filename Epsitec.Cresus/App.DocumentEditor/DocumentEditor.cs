using Epsitec.Common.Document;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using System.IO;

namespace Epsitec.App.DocumentEditor
{
	/// <summary>
	/// La classe DocumentEditor représente l'éditeur de document complet.
	/// </summary>
	
	[SuppressBundleSupport]
	
	public class DocumentEditor : Epsitec.Common.Widgets.Widget
	{
		public DocumentEditor(DocumentType type)
		{
			this.type = type;

			this.clipboard = new Document(this.type, DocumentMode.Clipboard);
			this.clipboard.Name = "Clipboard";

			this.document = new Document(this.type, DocumentMode.Modify);
			this.document.Name = "Document";
			this.document.Clipboard = this.clipboard;

			this.CreateLayout();
			this.InitCommands();
			this.ConnectEvents();
			this.document.Modifier.New();
			this.document.Notifier.NotifyAllChanged();
			this.document.Notifier.GenerateEvents();
		}

		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.allWidgets = false;
			}
			
			base.Dispose(disposing);
		}

		public DocumentType Type
		{
			get { return this.type; }
		}
		
		public override CommandDispatcher CommandDispatcher
		{
			get
			{
				if ( this.commandDispatcher == null )
				{
					// On crée son propre dispatcher, pour éviter de marcher sur les autres commandes.
					this.commandDispatcher = new Common.Support.CommandDispatcher("DocumentEditor");
					this.commandDispatcher.CommandDispatched += new EventHandler(this.HandleCommandDispatched);
					this.commandDispatcher.RegisterController(this);
				}
				
				return this.commandDispatcher;
			}
		}

		private void HandleCommandDispatched(object sender)
		{
			//?this.document.Notifier.GenerateEvents();
		}

		public void AsyncNotify()
		{
			this.document.Notifier.GenerateEvents();
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
			this.menu.Items.Add(new MenuItem("", "Document"));
#if DEBUG
			this.menu.Items.Add(new MenuItem("", "Debug"));
#endif
			this.menu.Items.Add(new MenuItem("", "Aide"));

			int i = 0;

			VMenu fileMenu = new VMenu();
			fileMenu.Name = "File";
			fileMenu.Host = this;
			this.MenuAdd(fileMenu, @"file:images/new.icon", "New", "Nouveau", "Ctrl+N");
			this.MenuAdd(fileMenu, @"file:images/open.icon", "Open", "Ouvrir...", "Ctrl+O");
			this.MenuAdd(fileMenu, @"file:images/save.icon", "Save", "Enregistrer", "Ctrl+S");
			this.MenuAdd(fileMenu, @"file:images/saveas.icon", "SaveAs", "Enregistrer sous...", "");
			this.MenuAdd(fileMenu, @"", "", "", "");
			this.MenuAdd(fileMenu, @"file:images/print.icon", "Print", "Imprimer...", "Ctrl+P");
			this.MenuAdd(fileMenu, @"", "", "", "");
			this.MenuAdd(fileMenu, @"", "QuitApplication", "Quitter", "");
			fileMenu.AdjustSize();
			this.menu.Items[i++].Submenu = fileMenu;

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
			this.menu.Items[i++].Submenu = editMenu;

			VMenu objMenu = new VMenu();
			objMenu.Name = "Obj";
			objMenu.Host = this;
			this.MenuAdd(objMenu, @"file:images/deselect.icon", "Deselect", "Désélectionner tout", "");
			this.MenuAdd(objMenu, @"file:images/selectall.icon", "SelectAll", "Tout sélectionner", "");
			this.MenuAdd(objMenu, @"file:images/selectinvert.icon", "SelectInvert", "Inverser la sélection", "");
			this.MenuAdd(objMenu, @"file:images/selectpartial.icon", "SelectPartial", "Sélection partielle", "");
			this.MenuAdd(objMenu, @"file:images/selectglobal.icon", "SelectGlobal", "Sélection groupée", "");
			this.MenuAdd(objMenu, @"", "", "", "");
			this.MenuAdd(objMenu, @"y/n", "HideHalf", "Mode estompé", "");
			this.MenuAdd(objMenu, @"file:images/hidesel.icon", "HideSel", "Cacher la sélection", "");
			this.MenuAdd(objMenu, @"file:images/hiderest.icon", "HideRest", "Cacher le reste", "");
			this.MenuAdd(objMenu, @"file:images/hidecancel.icon", "HideCancel", "Montrer tout", "");
			this.MenuAdd(objMenu, @"", "", "", "");
			this.MenuAdd(objMenu, @"file:images/orderup.icon", "OrderUp", "Dessus", "");
			this.MenuAdd(objMenu, @"file:images/orderdown.icon", "OrderDown", "Dessous", "");
			this.MenuAdd(objMenu, @"", "", "", "");
			this.MenuAdd(objMenu, @"", "", "Opérations", "");
			this.MenuAdd(objMenu, @"file:images/groupempty.icon", "", "Groupe", "");
			objMenu.AdjustSize();
			this.menu.Items[i++].Submenu = objMenu;

			VMenu operMenu = new VMenu();
			operMenu.Name = "Oper";
			operMenu.Host = this;
			this.MenuAdd(operMenu, @"", "Rotate90", "Quart de tour à gauche", "");
			this.MenuAdd(operMenu, @"", "Rotate180", "Demi-tour", "");
			this.MenuAdd(operMenu, @"", "Rotate270", "Quart de tour à droite", "");
			this.MenuAdd(operMenu, @"", "", "", "");
			this.MenuAdd(operMenu, @"", "MirrorH", "Miroir horizontal", "");
			this.MenuAdd(operMenu, @"", "MirrorV", "Miroir vertical", "");
			this.MenuAdd(operMenu, @"", "", "", "");
			this.MenuAdd(operMenu, @"", "ZoomMul2", "Zoom x2", "");
			this.MenuAdd(operMenu, @"", "ZoomDiv2", "Zoom /2", "");
			operMenu.AdjustSize();
			objMenu.Items[14].Submenu = operMenu;

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
			objMenu.Items[15].Submenu = groupMenu;

			VMenu showMenu = new VMenu();
			showMenu.Name = "Show";
			showMenu.Host = this;
			this.MenuAdd(showMenu, @"file:images/preview.icon", "Preview", "Aperçu avant impression", "");
			this.MenuAdd(showMenu, @"file:images/grid.icon", "Grid", "Grille magnétique", "");
			this.MenuAdd(showMenu, @"file:images/mode.icon", "Mode", "Tableau des objets", "");
			this.MenuAdd(showMenu, @"", "", "", "");
			this.MenuAdd(showMenu, @"file:images/zoommenu.icon", "", "Zoom", "");
			this.MenuAdd(showMenu, @"", "", "", "");
			this.MenuAdd(showMenu, @"", "", "Apparence", "");
			showMenu.AdjustSize();
			this.menu.Items[i++].Submenu = showMenu;

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
			showMenu.Items[4].Submenu = zoomMenu;

			VMenu lookMenu = new VMenu();
			lookMenu.Name = "Look";
			lookMenu.Host = this;
			string[] list = Epsitec.Common.Widgets.Adorner.Factory.AdornerNames;
			foreach ( string name in list )
			{
				this.MenuAdd(lookMenu, @"y/n", "SelectLook(this.Name)", name, "", name);
			}
			lookMenu.AdjustSize();
			showMenu.Items[6].Submenu = lookMenu;

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
			this.menu.Items[i++].Submenu = arrayMenu;

			VMenu arrayLookMenu = new VMenu();
			arrayLookMenu.Name = "ArrayLook";
			arrayLookMenu.Host = this;
			for ( int j=0 ; j<100 ; j++ )
			{
				string text, name;
				if ( !ObjectArray.CommandLook(j, out text, out name) )  break;
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

			VMenu docMenu = new VMenu();
			docMenu.Name = "Document";
			docMenu.Host = this;
			this.MenuAdd(docMenu, @"", "PageCreate", "Nouvelle page", "");
			this.MenuAdd(docMenu, @"", "PageDelete", "Supprimer la page", "");
			this.MenuAdd(docMenu, @"", "PageUp", "Reculer la page", "");
			this.MenuAdd(docMenu, @"", "PageDown", "Avancer la page", "");
			this.MenuAdd(docMenu, @"", "", "", "");
			this.MenuAdd(docMenu, @"", "LayerCreate", "Nouveau calque", "");
			this.MenuAdd(docMenu, @"", "LayerDelete", "Supprimer le calque", "");
			this.MenuAdd(docMenu, @"", "LayerDown", "Monter le calque", "");
			this.MenuAdd(docMenu, @"", "LayerUp", "Descendre le calque", "");
			docMenu.AdjustSize();
			this.menu.Items[i++].Submenu = docMenu;

#if DEBUG
			VMenu debugMenu = new VMenu();
			debugMenu.Name = "Debug";
			debugMenu.Host = this;
			this.MenuAdd(debugMenu, @"y/n", "DebugBboxThin", "BBoxThin", "");
			this.MenuAdd(debugMenu, @"y/n", "DebugBboxGeom", "BBoxGeom", "");
			this.MenuAdd(debugMenu, @"y/n", "DebugBboxFull", "BBoxFull", "");
			this.MenuAdd(debugMenu, @"", "", "", "");
			this.MenuAdd(debugMenu, @"", "DebugDirty", "Salir", "F12");
			debugMenu.AdjustSize();
			this.menu.Items[i++].Submenu = debugMenu;
#endif

			VMenu helpMenu = new VMenu();
			helpMenu.Name = "Help";
			helpMenu.Host = this;
			helpMenu.Items.Add(new MenuItem("help", "", "Aide", "F1"));
			helpMenu.Items.Add(new MenuItem("ctxhelp", "", "Aide contextuelle", ""));
			helpMenu.Items.Add(new MenuItem("about", "", "A propos de...", ""));
			helpMenu.AdjustSize();
			this.menu.Items[i++].Submenu = helpMenu;

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
			this.VToolBarAdd(@"file:images/global.icon", "SelectTool(this.Name)", "Rectangle de sélection", "Global");
			this.VToolBarAdd(@"file:images/edit.icon", "SelectTool(this.Name)", "Editer", "Edit");
			this.VToolBarAdd(@"file:images/zoom.icon", "SelectTool(this.Name)", "Agrandir", "Zoom");
			this.VToolBarAdd(@"file:images/hand.icon", "SelectTool(this.Name)", "Déplacer", "Hand");
			this.VToolBarAdd(@"file:images/picker.icon", "SelectTool(this.Name)", "Pipette", "Picker");
			if ( this.document.Type == DocumentType.Pictogram )
			{
				this.VToolBarAdd(@"file:images/hotspot.icon", "SelectTool(this.Name)", "Point chaud", "HotSpot");
			}
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
			
			if ( this.document.Type == DocumentType.Pictogram )
			{
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
			}

			this.book = new TabBook();
			this.book.Arrows = TabBookArrows.Stretch;
			this.book.Parent = this;

			this.bookProperties = new TabPage();
			this.bookProperties.TabTitle = "Attributs";
			this.book.Items.Add(this.bookProperties);

			this.bookStyles = new TabPage();
			this.bookStyles.TabTitle = "Styles";
			this.book.Items.Add(this.bookStyles);

#if DEBUG
			this.bookAutos = new TabPage();
			this.bookAutos.TabTitle = "Auto";
			this.book.Items.Add(this.bookAutos);
#endif

			this.bookPages = new TabPage();
			this.bookPages.TabTitle = "Pages";
			this.book.Items.Add(this.bookPages);

			this.bookLayers = new TabPage();
			this.bookLayers.TabTitle = "Calques";
			this.book.Items.Add(this.bookLayers);

			this.bookOper = new TabPage();
			//?this.bookOper.TabTitle = "Opérations";
			this.bookOper.TabTitle = "Op";
			this.book.Items.Add(this.bookOper);

			this.book.ActivePage = this.bookProperties;

			this.containerProperties = new ContainerProperties(this.document);
			this.containerProperties.Dock = DockStyle.Fill;
			this.containerProperties.DockMargins = new Margins(4, 4, 10, 4);
			this.containerProperties.Parent = this.bookProperties;
			this.document.Modifier.AttachContainer(this.containerProperties);

			this.containerStyles = new ContainerStyles(this.document);
			this.containerStyles.Dock = DockStyle.Fill;
			this.containerStyles.DockMargins = new Margins(4, 4, 10, 4);
			this.containerStyles.Parent = this.bookStyles;
			this.document.Modifier.AttachContainer(this.containerStyles);

#if DEBUG
			this.containerAutos = new ContainerAutos(this.document);
			this.containerAutos.Dock = DockStyle.Fill;
			this.containerAutos.DockMargins = new Margins(4, 4, 10, 4);
			this.containerAutos.Parent = this.bookAutos;
			this.document.Modifier.AttachContainer(this.containerAutos);
#endif

			this.containerPages = new ContainerPages(this.document);
			this.containerPages.Dock = DockStyle.Fill;
			this.containerPages.DockMargins = new Margins(4, 4, 10, 4);
			this.containerPages.Parent = this.bookPages;
			this.document.Modifier.AttachContainer(this.containerPages);

			this.containerLayers = new ContainerLayers(this.document);
			this.containerLayers.Dock = DockStyle.Fill;
			this.containerLayers.DockMargins = new Margins(4, 4, 10, 4);
			this.containerLayers.Parent = this.bookLayers;
			this.document.Modifier.AttachContainer(this.containerLayers);

			this.containerOper = new ContainerOper(this.document);
			this.containerOper.Dock = DockStyle.Fill;
			this.containerOper.DockMargins = new Margins(4, 4, 10, 4);
			this.containerOper.Parent = this.bookOper;
			this.document.Modifier.AttachContainer(this.containerOper);

			Widget mainViewParent;
			if ( this.document.Type == DocumentType.Pictogram )
			{
				mainViewParent = this.leftPane;
				this.viewer = new Viewer(this.document);
				this.viewer.Parent = mainViewParent;
				this.document.Modifier.ActiveViewer = this.viewer;
				this.document.Modifier.AttachViewer(this.viewer);

				this.frame1 = new Viewer(this.document);
				this.frame1.Parent = this.rightPane;
				this.frame1.DrawingContext.LayerDrawingMode = LayerDrawingMode.ShowInactive;
				this.document.Modifier.AttachViewer(this.frame1);

				this.frame2 = new Viewer(this.document);
				this.frame2.Parent = this.rightPane;
				this.frame2.DrawingContext.LayerDrawingMode = LayerDrawingMode.ShowInactive;
				this.document.Modifier.AttachViewer(this.frame2);
			}
			else
			{
				mainViewParent = root;
				this.viewer = new Viewer(this.document);
				this.viewer.Parent = mainViewParent;
				this.document.Modifier.ActiveViewer = this.viewer;
				this.document.Modifier.AttachViewer(this.viewer);
			}

			this.quickPagePrev = new GlyphButton("PagePrev");
			this.quickPagePrev.GlyphShape = GlyphShape.ArrowLeft;
			this.quickPagePrev.Parent = mainViewParent;
			ToolTip.Default.SetToolTip(this.quickPagePrev, "Page précédente");

			this.quickPageMenu = new Button();
			this.quickPageMenu.Command = "PageMenu";
			//?this.quickPageMenu.Text = "1";
			this.quickPageMenu.Clicked += new MessageEventHandler(this.HandleQuickPageMenu);
			this.quickPageMenu.Parent = mainViewParent;
			ToolTip.Default.SetToolTip(this.quickPageMenu, "Choix d'une page");

			this.quickPageNext = new GlyphButton("PageNext");
			this.quickPageNext.GlyphShape = GlyphShape.ArrowRight;
			this.quickPageNext.Parent = mainViewParent;
			ToolTip.Default.SetToolTip(this.quickPageNext, "Page suivante");

			this.hScroller = new HScroller();
			this.hScroller.ValueChanged += new EventHandler(this.HandleHScrollerValueChanged);
			this.hScroller.Parent = mainViewParent;

			this.quickLayerNext = new GlyphButton("LayerNext");
			this.quickLayerNext.GlyphShape = GlyphShape.ArrowUp;
			this.quickLayerNext.Parent = mainViewParent;
			ToolTip.Default.SetToolTip(this.quickLayerNext, "Calque suivant");

			this.quickLayerMenu = new Button();
			this.quickLayerMenu.Command = "LayerMenu";
			//?this.quickLayerMenu.Text = "A";
			this.quickLayerMenu.Clicked += new MessageEventHandler(this.HandleQuickLayerMenu);
			this.quickLayerMenu.Parent = mainViewParent;
			ToolTip.Default.SetToolTip(this.quickLayerMenu, "Choix d'un calque");

			this.quickLayerPrev = new GlyphButton("LayerPrev");
			this.quickLayerPrev.GlyphShape = GlyphShape.ArrowDown;
			this.quickLayerPrev.Parent = mainViewParent;
			ToolTip.Default.SetToolTip(this.quickLayerPrev, "Calque précédent");

			this.vScroller = new VScroller();
			this.vScroller.ValueChanged += new EventHandler(this.HandleVScrollerValueChanged);
			this.vScroller.Parent = mainViewParent;

			this.info = new StatusBar();
			this.info.Parent = this;
			this.InfoAdd("", 200, "StatusDocument", "");
			this.InfoAdd(@"file:images/deselect.icon", 0, "Deselect", "Désélectionner tout");
			this.InfoAdd(@"file:images/selectall.icon", 0, "SelectAll", "Tout sélectionner");
			this.InfoAdd(@"file:images/selectinvert.icon", 0, "SelectInvert", "Inverser la sélection");
			this.InfoAdd(@"file:images/selectpartial.icon", 0, "SelectPartial", "Sélection partielle");
			this.InfoAdd(@"file:images/selectglobal.icon", 0, "SelectGlobal", "Sélection groupée");
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
		}

		public HMenu GetMenu()
		{
			return this.menu;
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
				button.Size = new Size(h, h);
				this.info.Items.Add(button);
				ToolTip.Default.SetToolTip(button, tooltip);
			}
		}


		private void HandleHScrollerValueChanged(object sender)
		{
			Viewer viewer = this.document.Modifier.ActiveViewer;
			viewer.DrawingContext.OriginX = (double) -this.hScroller.Value;
			//?this.document.Notifier.GenerateEvents();
		}

		private void HandleVScrollerValueChanged(object sender)
		{
			Viewer viewer = this.document.Modifier.ActiveViewer;
			viewer.DrawingContext.OriginY = (double) -this.vScroller.Value;
			//?this.document.Notifier.GenerateEvents();
		}


		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();
			this.ResizeLayout();
		}

		protected void ResizeLayout()
		{
			if ( !this.allWidgets )  return;

			Rectangle rect = new Rectangle(0, 0, this.Client.Width, this.Client.Height);

			this.hToolBar.Location = new Point(0, rect.Height-this.hToolBar.DefaultHeight);
			this.hToolBar.Size = new Size(rect.Width, this.hToolBar.DefaultHeight);

			this.vToolBar.Location = new Point(0, this.info.Height);
			this.vToolBar.Size = new Size(this.vToolBar.DefaultWidth, rect.Height-this.info.Height-this.hToolBar.DefaultHeight);

			this.info.Location = new Point(0, 0);
			this.info.Size = new Size(rect.Width, this.info.DefaultHeight);

			double pw = this.panelsWidth+12+1;

			this.book.Location = new Point(rect.Right-pw, this.info.Height+1);
			this.book.Size = new Size(pw-1, rect.Height-this.info.Height-this.hToolBar.DefaultHeight-2);
			Rectangle inside = this.book.InnerBounds;
			this.bookProperties.Bounds = inside;
			this.bookStyles.Bounds = inside;
#if DEBUG
			this.bookAutos.Bounds = inside;
#endif
			this.bookPages.Bounds = inside;
			this.bookLayers.Bounds = inside;
			this.bookOper.Bounds = inside;

			this.root.Location = new Point(this.vToolBar.DefaultWidth, this.info.Height);
			this.root.Size = new Size(rect.Width-this.vToolBar.DefaultWidth-pw, rect.Height-this.info.Height-this.hToolBar.DefaultHeight);

			if ( this.document.Type == DocumentType.Pictogram )
			{
				this.pane.Location = new Point(0, 0);
				this.pane.Size = this.root.Size;
				this.rightPane.PaneMinSize = 20+this.document.Size.Width;
			}

			Widget mainViewParent;
			if ( this.document.Type == DocumentType.Pictogram )
			{
				mainViewParent = this.leftPane;
			}
			else
			{
				mainViewParent = root;
			}

			Size docSize = this.document.Size;
			double dimx = mainViewParent.Width-20;
			double dimy = mainViewParent.Height-20;

			if ( this.document.Type == DocumentType.Pictogram )
			{
				dimy = dimx*docSize.Height/docSize.Width;
				if ( dimy > this.leftPane.Height-20 )
				{
					dimy = mainViewParent.Height-20;
					dimx = dimy*docSize.Width/docSize.Height;
				}
			}
			dimx -= this.vScroller.DefaultWidth;
			dimy -= this.hScroller.DefaultHeight;
			this.viewer.Location = new Point(10, mainViewParent.Height-10-dimy+1);
			this.viewer.Size = new Size(dimx-1, dimy-1);

			rect.Bottom = mainViewParent.Height-10-dimy-this.hScroller.DefaultHeight;
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
			rect.Bottom = mainViewParent.Height-10-this.vScroller.DefaultWidth;
			rect.Height = this.vScroller.DefaultWidth;
			this.quickLayerNext.Bounds = rect;
			rect.Offset(0, -rect.Height);
			this.quickLayerMenu.Bounds = rect;
			rect.Offset(0, -rect.Height);
			this.quickLayerPrev.Bounds = rect;
			rect.Bottom = mainViewParent.Height-10-dimy+1;
			rect.Height = dimy-1-this.vScroller.DefaultWidth*3.2;
			this.vScroller.Bounds = rect;

			if ( this.document.Type == DocumentType.Pictogram )
			{
				dimx = this.rightPane.Width-20;
				dimy = dimx*docSize.Height/docSize.Width;
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
			}

			//?this.GeometryPanels();
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
			this.document.Modifier.Tool = e.CommandArgs[0];
		}

		[Command ("SelectLook")]
		void CommandSelectLook(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			Epsitec.Common.Widgets.Adorner.Factory.SetActive(e.CommandArgs[0]);
			this.UpdateLookCommand();
		}
		
		// Affiche le dialogue pour demander s'il faut enregistrer le
		// document modifié, avant de passer à un autre document.
		protected bool DialogSave(CommandDispatcher dispatcher, string cmdYes, string cmdNo)
		{
			if ( !this.document.IsDirtySerialize ||
				 this.document.Modifier.StatisticTotalObjects() == 0 )
			{
				return false;
			}

			string title = "Crésus";
			string icon = "manifest:Epsitec.Common.Dialogs.Images.Warning.icon";

			string shortFilename = "<i>sans titre</i>";
			if ( this.document.Filename != "" )
			{
				shortFilename = string.Format("<b>{0}</b>", Misc.ExtractName(this.document.Filename));
			}

			string statistic = string.Format("<font size=\"80%\">{0}</font><br/>", this.document.Modifier.Statistic());
			string message = string.Format("<font size=\"100%\">Le fichier {0} a été modifié.</font><br/><br/>{1}Voulez-vous enregistrer les modifications ?", shortFilename, statistic);
			Common.Dialogs.IDialog dialog = Common.Dialogs.Message.CreateYesNoCancel(title, icon, message, cmdYes, cmdNo, dispatcher);
			dialog.Owner = this.Window;
			dialog.OpenDialog();
			return true;
		}

		// Retourne un texte multi-lignes de statistiques sur le fichier.
		protected static string StatisticFilename(string filename)
		{
			System.DateTime date1 = File.GetCreationTime(filename);
			System.DateTime date2 = File.GetLastAccessTime(filename);

			long size = 0;
			try
			{
				using ( Stream stream = File.OpenRead(filename) )
				{
					size = stream.Length;
				}
			}
			catch ( System.Exception )
			{
				size = 999999;
			}

			string attributes = "";
			FileAttributes att = File.GetAttributes(filename);
			if ( (att & FileAttributes.ReadOnly) != 0 )
			{
				attributes += "Lecture seule";
			}
			if ( (att & FileAttributes.Hidden) != 0 )
			{
				if ( attributes.Length > 0 )  attributes += ", ";
				attributes += "Fichier caché";
			}

			string chip = "<list type=\"fix\" width=\"1.5\"/>";
			string info1 = string.Format("{0}Nom complet: {1}<br/>", chip, filename);
			string info2 = string.Format("{0}Taille: {1}<br/>", chip, size);
			string info3 = string.Format("{0}Date de création: {1}<br/>", chip, date1.ToString());
			string info4 = string.Format("{0}Date de modification: {1}<br/>", chip, date2.ToString());
			string info5 = string.Format("{0}Attributs: {1}<br/>", chip, attributes);
			return string.Format("{0}{1}{2}{3}{4}", info1, info2, info3, info4, info5);
		}

		// Affiche le dialogue pour demander s'il faut effacer le
		// fichier existant.
		protected void DialogDelete(CommandDispatcher dispatcher, string cmdYes)
		{
			string title = "Crésus";
			string icon = "manifest:Epsitec.Common.Dialogs.Images.Warning.icon";

			string shortFilename = string.Format("<b>{0}</b>", Misc.ExtractName(this.saveAsFilename));
			string statistic = string.Format("<font size=\"80%\">{0}</font><br/>", DocumentEditor.StatisticFilename(this.saveAsFilename));
			string message = string.Format("<font size=\"100%\">Le fichier {0} existe déjà.</font><br/><br/>{1}Voulez-vous le remplacer ?", shortFilename, statistic);

			Common.Dialogs.IDialog dialog = Common.Dialogs.Message.CreateOkCancel(title, icon, message, cmdYes, dispatcher);
			dialog.Owner = this.Window;
			dialog.OpenDialog();
		}

		// Affiche le dialogue pour signaler une erreur.
		protected void DialogError(CommandDispatcher dispatcher, string error)
		{
			if ( error == "" )  return;

			string title = "Crésus";
			string icon = "manifest:Epsitec.Common.Dialogs.Images.Warning.icon";
			string message = error;

			Common.Dialogs.IDialog dialog = Common.Dialogs.Message.CreateOkCancel(title, icon, message, "", dispatcher);
			dialog.Owner = this.Window;
			dialog.OpenDialog();
		}

		[Command ("New")]
		void CommandNew(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			if ( this.DialogSave(dispatcher, "NewYes", "NewNo") )  return;
			this.document.Modifier.New();
		}

		[Command ("NewYes")]
		void CommandNewYes(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			if ( this.document.Filename == "" )
			{
				this.CommandSaveAs(dispatcher, e);
			}
			else
			{
				this.CommandSave(dispatcher, e);
			}

			this.CommandNew(dispatcher, e);
		}

		[Command ("NewNo")]
		void CommandNewNo(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.document.IsDirtySerialize = false;
			this.CommandNew(dispatcher, e);
		}

		[Command ("Open")]
		void CommandOpen(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			if ( this.DialogSave(dispatcher, "SaveYes", "SaveNo") )  return;

			Common.Dialogs.FileOpen dialog = new Common.Dialogs.FileOpen();
		
			dialog.FileName = "";
			if ( this.document.Type == DocumentType.Graphic )
			{
				dialog.Title = "Ouvrir un document";
				dialog.Filters.Add("crdoc", "Document", "*.crdoc");
			}
			else
			{
				dialog.Title = "Ouvrir une icône";
				dialog.Filters.Add("icon", "Icônes", "*.icon");
			}
			dialog.Owner = this.Window;
			dialog.OpenDialog();
			if ( dialog.Result != Common.Dialogs.DialogResult.Accept )  return;

			string err = this.document.Read(dialog.FileName);
			this.DialogError(dispatcher, err);
		}

		[Command ("SaveYes")]
		void CommandSaveYes(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			if ( this.document.Filename == "" )
			{
				this.CommandSaveAs(dispatcher, e);
			}
			else
			{
				this.CommandSave(dispatcher, e);
			}

			this.CommandOpen(dispatcher, e);
		}

		[Command ("SaveNo")]
		void CommandSaveNo(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.document.IsDirtySerialize = false;
			this.CommandOpen(dispatcher, e);
		}

		[Command ("Save")]
		void CommandSave(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			if ( !this.document.IsDirtySerialize )  return;

			if ( this.document.Filename == "" )
			{
				this.CommandSaveAs(dispatcher, e);
			}
			else
			{
				string err = this.document.Write(this.document.Filename);
				this.DialogError(dispatcher, err);
			}
		}
		
		[Command ("SaveAs")]
		void CommandSaveAs(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			Common.Dialogs.FileSave dialog = new Common.Dialogs.FileSave();
			
			dialog.FileName = this.document.Filename;
			if ( this.document.Type == DocumentType.Graphic )
			{
				dialog.Title = "Enregistrer un document";
				dialog.Filters.Add("crdoc", "Document", "*.crdoc");
			}
			else
			{
				dialog.Title = "Enregistrer une icône";
				dialog.Filters.Add("icon", "Icônes", "*.icon");
			}
			dialog.PromptForOverwriting = true;
			dialog.Owner = this.Window;
			dialog.OpenDialog();
			if ( dialog.Result != Common.Dialogs.DialogResult.Accept )  return;

#if false
			if ( File.Exists(dialog.FileName) )
			{
				this.saveAsFilename = dialog.FileName;
				this.DialogDelete(dispatcher, "SaveAsYes");
				return;
			}
#endif

			string err = this.document.Write(dialog.FileName);
			this.DialogError(dispatcher, err);
		}
		
		[Command ("SaveAsYes")]
		void CommandSaveAsYes(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			string err = this.document.Write(this.saveAsFilename);
			this.DialogError(dispatcher, err);
		}

		[Command ("Print")]
		void CommandPrint(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			Common.Dialogs.Print dialog = new Common.Dialogs.Print();
			
			dialog.AllowFromPageToPage = false;
			dialog.AllowSelectedPages  = false;
			
			string[] printers = Common.Printing.PrinterSettings.InstalledPrinters;
			
			dialog.Document.PrinterSettings.MinimumPage = 1;
			dialog.Document.PrinterSettings.MaximumPage = 1;
			dialog.Document.PrinterSettings.FromPage = 1;
			dialog.Document.PrinterSettings.ToPage = 1;
			dialog.Document.PrinterSettings.PrintRange = Common.Printing.PrintRange.AllPages;
			dialog.Document.PrinterSettings.Collate = false;
			dialog.Owner = this.Window;
			dialog.OpenDialog();
			if ( dialog.Result != Common.Dialogs.DialogResult.Accept )  return;

			this.document.Print(dialog);
		}
		
		[Command ("QuitApplication")]
		void CommandQuitApplication(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			if ( this.DialogSave(dispatcher, "QuitYes", "QuitNo") )  return;
			this.QuitApplication();
		}

		[Command ("QuitYes")]
		void CommandQuitYes(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			if ( this.document.Filename == "" )
			{
				this.CommandSaveAs(dispatcher, e);
			}
			else
			{
				this.CommandSave(dispatcher, e);
			}

			this.CommandQuitApplication(dispatcher, e);
		}

		[Command ("QuitNo")]
		void CommandQuitNo(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.document.IsDirtySerialize = false;
			this.CommandQuitApplication(dispatcher, e);
		}

		[Command ("Delete")]
		void CommandDelete()
		{
			this.document.Modifier.DeleteSelection();
		}

		[Command ("Duplicate")]
		void CommandDuplicate(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.document.Modifier.DuplicateSelection(new Point(1,1));
		}

		[Command ("Cut")]
		void CommandCut(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.document.Modifier.CutSelection();
		}

		[Command ("Copy")]
		void CommandCopy(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.document.Modifier.CopySelection();
		}

		[Command ("Paste")]
		void CommandPaste(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.document.Modifier.Paste();
		}

		[Command ("Undo")]
		void CommandUndo(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.document.Modifier.Undo();
		}

		[Command ("Redo")]
		void CommandRedo(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.document.Modifier.Redo();
		}

		[Command ("OrderUp")]
		void CommandOrderUp(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.document.Modifier.OrderUpSelection();
		}

		[Command ("OrderDown")]
		void CommandOrderDown(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.document.Modifier.OrderDownSelection();
		}

		[Command ("Rotate90")]
		void CommandRotate90(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.document.Modifier.RotateSelection(90);
		}

		[Command ("Rotate180")]
		void CommandRotate180(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.document.Modifier.RotateSelection(180);
		}

		[Command ("Rotate270")]
		void CommandRotate270(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.document.Modifier.RotateSelection(270);
		}

		[Command ("MirrorH")]
		void CommandMirrorH(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.document.Modifier.MirrorSelection(true);
		}

		[Command ("MirrorV")]
		void CommandMirrorV(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.document.Modifier.MirrorSelection(false);
		}

		[Command ("ZoomMul2")]
		void CommandZoomMul2(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.document.Modifier.ZoomSelection(2.0);
		}

		[Command ("ZoomDiv2")]
		void CommandZoomDiv2(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.document.Modifier.ZoomSelection(0.5);
		}

		[Command ("Merge")]
		void CommandMerge(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.document.Modifier.MergeSelection();
		}

		[Command ("Group")]
		void CommandGroup(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.document.Modifier.GroupSelection();
		}

		[Command ("Ungroup")]
		void CommandUngroup(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.document.Modifier.UngroupSelection();
		}

		[Command ("Inside")]
		void CommandInside(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.document.Modifier.InsideSelection();
		}

		[Command ("Outside")]
		void CommandOutside(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.document.Modifier.OutsideSelection();
		}

		[Command ("Grid")]
		void CommandGrid(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			context.GridActive = !context.GridActive;
		}

		[Command ("Preview")]
		void CommandPreview(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			context.PreviewActive = !context.PreviewActive;
		}

		[Command ("Deselect")]
		void CommandDeselect(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.document.Modifier.DeselectAll();
		}

		[Command ("SelectAll")]
		void CommandSelectAll(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.document.Modifier.SelectAll();
		}

		[Command ("SelectInvert")]
		void CommandSelectInvert(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.document.Modifier.InvertSelection();
		}

		[Command ("SelectGlobal")]
		void CommandSelectGlobal(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.document.Modifier.Tool = "Select";
			Viewer viewer = this.document.Modifier.ActiveViewer;
			viewer.GlobalSelect = !viewer.GlobalSelect;
		}

		[Command ("SelectPartial")]
		void CommandSelectPartial(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.document.Modifier.Tool = "Select";
			Viewer viewer = this.document.Modifier.ActiveViewer;
			viewer.PartialSelect = !viewer.PartialSelect;
		}

		[Command ("HideHalf")]
		void CommandHideHalf(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.document.Modifier.Tool = "Select";
			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			context.HideHalfActive = !context.HideHalfActive;
		}

		[Command ("HideSel")]
		void CommandHideSel(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.document.Modifier.HideSelection();
		}

		[Command ("HideRest")]
		void CommandHideRest(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.document.Modifier.HideRest();
		}

		[Command ("HideCancel")]
		void CommandHideCancel(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.document.Modifier.HideCancel();
		}

		[Command ("ZoomMin")]
		void CommandZoomMin(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.document.Modifier.ZoomChange(0.0001);
		}

		[Command ("ZoomDefault")]
		void CommandZoomDefault(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			this.document.Modifier.ZoomMemorize();
			context.ZoomAndOrigin(1, 0,0);
		}

		[Command ("ZoomSel")]
		void CommandZoomSel(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.document.Modifier.ZoomSel();
		}

		[Command ("ZoomPrev")]
		void CommandZoomPrev(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.document.Modifier.ZoomPrev();
		}

		[Command ("ZoomSub")]
		void CommandZoomSub(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.document.Modifier.ZoomChange(0.5);
		}

		[Command ("ZoomAdd")]
		void CommandZoomAdd(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.document.Modifier.ZoomChange(2.0);
		}

		// Exécute une commande locale à un objet.
		[Command ("Object")]
		void CommandObject(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			Widget widget = e.Source as Widget;
			this.document.Modifier.ActiveViewer.CommandObject(widget.Name);
		}

		[Command ("ArrayOutlineFrame")]
		[Command ("ArrayOutlineHoriz")]
		[Command ("ArrayOutlineVerti")]
		[Command ("ArrayAddColumnLeft")]
		[Command ("ArrayAddColumnRight")]
		[Command ("ArrayAddRowTop")]
		[Command ("ArrayAddRowBottom")]
		[Command ("ArrayDelColumn")]
		[Command ("ArrayDelRow")]
		[Command ("ArrayAlignColumn")]
		[Command ("ArrayAlignRow")]
		[Command ("ArraySwapColumn")]
		[Command ("ArraySwapRow")]
		[Command ("ArrayLook")]
		void CommandArray(CommandDispatcher dispatcher, CommandEventArgs e)
		{
		}


		[Command ("PagePrev")]
		void CommandPagePrev(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			if ( context.CurrentPage > 0 )
			{
				context.CurrentPage = context.CurrentPage-1;
			}
		}

		[Command ("PageNext")]
		void CommandPageNext(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			if ( context.CurrentPage < context.TotalPages()-1 )
			{
				context.CurrentPage = context.CurrentPage+1;
			}
		}

		// Bouton "menu des pages" cliqué.
		private void HandleQuickPageMenu(object sender, MessageEventArgs e)
		{
			Button button = sender as Button;
			if ( button == null )  return;
			Point pos = button.MapClientToScreen(new Point(0, button.Height));
			VMenu menu = this.CreatePagesMenu();
			menu.Host = this;
			pos.Y += menu.Height;
			menu.ShowAsContextMenu(this.Window, pos);
		}

		[Command ("PageSelect")]
		void CommandPageSelect(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			int sel = System.Convert.ToInt32(e.CommandArgs[0]);
			context.CurrentPage = sel;
		}

		[Command ("PageCreate")]
		void CommandPageCreate(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			int rank = context.CurrentPage;
			this.document.Modifier.PageCreate(rank+1, "");
		}

		[Command ("PageDelete")]
		void CommandPageDelete(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			int rank = context.CurrentPage;
			this.document.Modifier.PageDelete(rank);
		}

		[Command ("PageUp")]
		void CommandPageUp(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			int rank = context.CurrentPage;
			this.document.Modifier.PageSwap(rank, rank-1);
			context.CurrentPage = rank-1;
		}

		[Command ("PageDown")]
		void CommandPageDown(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			int rank = context.CurrentPage;
			this.document.Modifier.PageSwap(rank, rank+1);
			context.CurrentPage = rank+1;
		}


		[Command ("LayerPrev")]
		void CommandLayerPrev(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			if ( context.CurrentLayer > 0 )
			{
				context.CurrentLayer = context.CurrentLayer-1;
			}
		}

		[Command ("LayerNext")]
		void CommandLayerNext(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			if ( context.CurrentLayer < context.TotalLayers()-1 )
			{
				context.CurrentLayer = context.CurrentLayer+1;
			}
		}

		// Bouton "menu des calques" cliqué.
		private void HandleQuickLayerMenu(object sender, MessageEventArgs e)
		{
			Button button = sender as Button;
			if ( button == null )  return;
			Point pos = button.MapClientToScreen(new Point(0, button.Height));
			VMenu menu = this.CreateLayersMenu();
			menu.Host = this;
			pos.X -= menu.Width;
			menu.ShowAsContextMenu(this.Window, pos);
		}

		[Command ("LayerSelect")]
		void CommandLayerSelect(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			int sel = System.Convert.ToInt32(e.CommandArgs[0]);
			context.CurrentLayer = sel;
		}

		[Command ("LayerCreate")]
		void CommandLayerCreate(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			int rank = context.CurrentLayer;
			this.document.Modifier.LayerCreate(rank+1, "");
		}

		[Command ("LayerDelete")]
		void CommandLayerDelete(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			int rank = context.CurrentLayer;
			this.document.Modifier.LayerDelete(rank);
		}

		[Command ("LayerUp")]
		void CommandLayerUp(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			int rank = context.CurrentLayer;
			this.document.Modifier.LayerSwap(rank, rank-1);
			context.CurrentLayer = rank-1;
		}

		[Command ("LayerDown")]
		void CommandLayerDown(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			int rank = context.CurrentLayer;
			this.document.Modifier.LayerSwap(rank, rank+1);
			context.CurrentLayer = rank+1;
		}


		[Command ("DebugBboxThin")]
		void CommandDebugBboxThin(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			context.IsDrawBoxThin = !context.IsDrawBoxThin;
		}

		[Command ("DebugBboxGeom")]
		void CommandDebugBboxGeom(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			context.IsDrawBoxGeom = !context.IsDrawBoxGeom;
		}

		[Command ("DebugBboxFull")]
		void CommandDebugBboxFull(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			context.IsDrawBoxFull = !context.IsDrawBoxFull;
		}

		[Command ("DebugDirty")]
		void CommandDebugDirty(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.document.Modifier.ActiveViewer.DirtyAllViews();
		}


		// Quitte l'application.
		public void QuitApplication()
		{
			Window.Quit();
		}


		// Construit le menu pour choisir une page.
		public VMenu CreatePagesMenu()
		{
			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			UndoableList pages = this.document.Objects;  // liste des pages
			int total = pages.Count;
			VMenu menu = new VMenu();
			for ( int i=0 ; i<total ; i++ )
			{
				ObjectPage page = pages[i] as ObjectPage;

				string name = string.Format("{0}: {1}", (i+1).ToString(), page.Name);

				string icon = @"file:images/activeno.icon";
				if ( i == context.CurrentPage )
				{
					icon = @"file:images/activeyes.icon";
				}

				MenuItem item = new MenuItem("PageSelect(this.Name)", icon, name, "", i.ToString());
				menu.Items.Add(item);
			}
			menu.AdjustSize();
			return menu;
		}

		// Construit le menu pour choisir un calque.
		public VMenu CreateLayersMenu()
		{
			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			AbstractObject page = context.RootObject(1);
			UndoableList layers = page.Objects;  // liste des calques
			int total = layers.Count;
			VMenu menu = new VMenu();
			for ( int i=0 ; i<total ; i++ )
			{
				int ii = total-i-1;
				ObjectLayer layer = layers[i] as ObjectLayer;

				string name = "";
				if ( layer.Name == "" )
				{
					name = string.Format("{0}: {1}", ((char)('A'+ii)).ToString(), ObjectLayer.LayerPositionName(ii, total));
				}
				else
				{
					name = string.Format("{0}: {1}", ((char)('A'+ii)).ToString(), layer.Name);
				}

				string icon = @"file:images/activeno.icon";
				if ( ii == context.CurrentLayer )
				{
					icon = @"file:images/activeyes.icon";
				}

				MenuItem item = new MenuItem("LayerSelect(this.Name)", icon, name, "", ii.ToString());
				menu.Items.Add(item);
			}
			menu.AdjustSize();
			return menu;
		}



		// Initialise toutes les commandes.
		protected void InitCommands()
		{
			CommandDispatcher foo = this.CommandDispatcher;

			this.newState = new CommandState("New", this.commandDispatcher);
			this.openState = new CommandState("Open", this.commandDispatcher);
			this.saveState = new CommandState("Save", this.commandDispatcher);
			this.saveAsState = new CommandState("SaveAs", this.commandDispatcher);
			this.deleteState = new CommandState("Delete", this.commandDispatcher);
			this.duplicateState = new CommandState("Duplicate", this.commandDispatcher);
			this.cutState = new CommandState("Cut", this.commandDispatcher);
			this.copyState = new CommandState("Copy", this.commandDispatcher);
			this.pasteState = new CommandState("Paste", this.commandDispatcher);
			this.orderUpState = new CommandState("OrderUp", this.commandDispatcher);
			this.orderDownState = new CommandState("OrderDown", this.commandDispatcher);
			this.rotate90State = new CommandState("Rotate90", this.commandDispatcher);
			this.rotate180State = new CommandState("Rotate180", this.commandDispatcher);
			this.rotate270State = new CommandState("Rotate270", this.commandDispatcher);
			this.mirrorHState = new CommandState("MirrorH", this.commandDispatcher);
			this.mirrorVState = new CommandState("MirrorV", this.commandDispatcher);
			this.zoomMul2State = new CommandState("ZoomMul2", this.commandDispatcher);
			this.zoomDiv2State = new CommandState("ZoomDiv2", this.commandDispatcher);
			this.mergeState = new CommandState("Merge", this.commandDispatcher);
			this.groupState = new CommandState("Group", this.commandDispatcher);
			this.ungroupState = new CommandState("Ungroup", this.commandDispatcher);
			this.insideState = new CommandState("Inside", this.commandDispatcher);
			this.outsideState = new CommandState("Outside", this.commandDispatcher);
			this.undoState = new CommandState("Undo", this.commandDispatcher);
			this.redoState = new CommandState("Redo", this.commandDispatcher);
			this.deselectState = new CommandState("Deselect", this.commandDispatcher);
			this.selectAllState = new CommandState("SelectAll", this.commandDispatcher);
			this.selectInvertState = new CommandState("SelectInvert", this.commandDispatcher);
			this.selectGlobalState = new CommandState("SelectGlobal", this.commandDispatcher);
			this.selectPartialState = new CommandState("SelectPartial", this.commandDispatcher);
			this.hideHalfState = new CommandState("HideHalf", this.commandDispatcher);
			this.hideSelState = new CommandState("HideSel", this.commandDispatcher);
			this.hideRestState = new CommandState("HideRest", this.commandDispatcher);
			this.hideCancelState = new CommandState("HideCancel", this.commandDispatcher);
			this.zoomMinState = new CommandState("ZoomMin", this.commandDispatcher);
			this.zoomDefaultState = new CommandState("ZoomDefault", this.commandDispatcher);
			this.zoomSelState = new CommandState("ZoomSel", this.commandDispatcher);
			this.zoomPrevState = new CommandState("ZoomPrev", this.commandDispatcher);
			this.zoomSubState = new CommandState("ZoomSub", this.commandDispatcher);
			this.zoomAddState = new CommandState("ZoomAdd", this.commandDispatcher);
			this.previewState = new CommandState("Preview", this.commandDispatcher);
			this.gridState = new CommandState("Grid", this.commandDispatcher);
			this.modeState = new CommandState("Mode", this.commandDispatcher);

			this.arrayOutlineFrameState = new CommandState("ArrayOutlineFrame", this.commandDispatcher);
			this.arrayOutlineHorizState = new CommandState("ArrayOutlineHoriz", this.commandDispatcher);
			this.arrayOutlineVertiState = new CommandState("ArrayOutlineVerti", this.commandDispatcher);
			this.arrayAddColumnLeftState = new CommandState("ArrayAddColumnLeft", this.commandDispatcher);
			this.arrayAddColumnRightState = new CommandState("ArrayAddColumnRight", this.commandDispatcher);
			this.arrayAddRowTopState = new CommandState("ArrayAddRowTop", this.commandDispatcher);
			this.arrayAddRowBottomState = new CommandState("ArrayAddRowBottom", this.commandDispatcher);
			this.arrayDelColumnState = new CommandState("ArrayDelColumn", this.commandDispatcher);
			this.arrayDelRowState = new CommandState("ArrayDelRow", this.commandDispatcher);
			this.arrayAlignColumnState = new CommandState("ArrayAlignColumn", this.commandDispatcher);
			this.arrayAlignRowState = new CommandState("ArrayAlignRow", this.commandDispatcher);
			this.arraySwapColumnState = new CommandState("ArraySwapColumn", this.commandDispatcher);
			this.arraySwapRowState = new CommandState("ArraySwapRow", this.commandDispatcher);
			this.arrayLookState = new CommandState("ArrayLook", this.commandDispatcher);

			this.debugBboxThinState = new CommandState("DebugBboxThin", this.commandDispatcher);
			this.debugBboxGeomState = new CommandState("DebugBboxGeom", this.commandDispatcher);
			this.debugBboxFullState = new CommandState("DebugBboxFull", this.commandDispatcher);

			this.pagePrev = new CommandState("PagePrev", this.commandDispatcher);
			this.pageNext = new CommandState("PageNext", this.commandDispatcher);
			this.pageMenu = new CommandState("PageMenu", this.commandDispatcher);
			this.pageCreate = new CommandState("PageCreate", this.commandDispatcher);
			this.pageDelete = new CommandState("PageDelete", this.commandDispatcher);
			this.pageUp = new CommandState("PageUp", this.commandDispatcher);
			this.pageDown = new CommandState("PageDown", this.commandDispatcher);

			this.layerPrev = new CommandState("LayerPrev", this.commandDispatcher);
			this.layerNext = new CommandState("LayerNext", this.commandDispatcher);
			this.layerMenu = new CommandState("LayerMenu", this.commandDispatcher);
			this.layerCreate = new CommandState("LayerCreate", this.commandDispatcher);
			this.layerDelete = new CommandState("LayerDelete", this.commandDispatcher);
			this.layerUp = new CommandState("LayerUp", this.commandDispatcher);
			this.layerDown = new CommandState("LayerDown", this.commandDispatcher);
		}


		// On s'engistre auprès du document pour tous les événements.
		protected void ConnectEvents()
		{
			this.document.Notifier.DocumentChanged  += new SimpleEventHandler(this.HandleDocumentChanged);
			this.document.Notifier.MouseChanged     += new SimpleEventHandler(this.HandleMouseChanged);
			this.document.Notifier.OriginChanged    += new SimpleEventHandler(this.HandleOriginChanged);
			this.document.Notifier.ZoomChanged      += new SimpleEventHandler(this.HandleZoomChanged);
			this.document.Notifier.ToolChanged      += new SimpleEventHandler(this.HandleToolChanged);
			this.document.Notifier.SaveChanged      += new SimpleEventHandler(this.HandleSaveChanged);
			this.document.Notifier.SelectionChanged += new SimpleEventHandler(this.HandleSelectionChanged);
			this.document.Notifier.CreateChanged    += new SimpleEventHandler(this.HandleCreateChanged);
			this.document.Notifier.StyleChanged     += new SimpleEventHandler(this.HandleStyleChanged);
			this.document.Notifier.PagesChanged     += new SimpleEventHandler(this.HandlePagesChanged);
			this.document.Notifier.LayersChanged    += new SimpleEventHandler(this.HandleLayersChanged);
			this.document.Notifier.PageChanged      += new ObjectEventHandler(this.HandlePageChanged);
			this.document.Notifier.LayerChanged     += new ObjectEventHandler(this.HandleLayerChanged);
			this.document.Notifier.UndoRedoChanged  += new SimpleEventHandler(this.HandleUndoRedoChanged);
			this.document.Notifier.GridChanged      += new SimpleEventHandler(this.HandleGridChanged);
			this.document.Notifier.PreviewChanged   += new SimpleEventHandler(this.HandlePreviewChanged);
			this.document.Notifier.HideHalfChanged  += new SimpleEventHandler(this.HandleHideHalfChanged);
			this.document.Notifier.DebugChanged     += new SimpleEventHandler(this.HandleDebugChanged);
			this.document.Notifier.PropertyChanged  += new PropertyEventHandler(this.HandlePropertyChanged);
			this.document.Notifier.DrawChanged      += new RedrawEventHandler(this.HandleDrawChanged);
		}

		// Appelé par le document lorsque les informations sur le document ont changé.
		private void HandleDocumentChanged()
		{
			StatusField field = this.info.Items["StatusDocument"] as StatusField;
			field.Text = this.TextDocument;
			field.Invalidate();
		}

		// Appelé par le document lorsque la position de la souris a changé.
		private void HandleMouseChanged()
		{
			// TODO: [PA] Parfois, this.info.Items est nul après avoir cliqué la case de fermeture de la fenêtre !
			if ( this.info.Items == null )  return;

			StatusField field = this.info.Items["StatusMouse"] as StatusField;
			field.Text = this.TextInfoMouse;
			field.Invalidate();
		}

		// Appelé par le document lorsque l'origine a changé.
		private void HandleOriginChanged()
		{
			this.UpdateScroller();

			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			this.zoomDefaultState.Enabled = ( context.Zoom != 1 || context.OriginX != 0 || context.OriginY != 0 );
		}

		// Appelé par le document lorsque le zoom a changé.
		private void HandleZoomChanged()
		{
			this.UpdateScroller();

			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			this.zoomMinState.Enabled = ( context.Zoom > this.document.Modifier.ZoomMin );
			this.zoomDefaultState.Enabled = ( context.Zoom != 1 || context.OriginX != 0 || context.OriginY != 0 );
			this.zoomPrevState.Enabled = ( this.document.Modifier.ZoomMemorizeCount > 0 );
			this.zoomSubState.Enabled = ( context.Zoom > this.document.Modifier.ZoomMin );
			this.zoomAddState.Enabled = ( context.Zoom < this.document.Modifier.ZoomMax );

			StatusField field = this.info.Items["StatusZoom"] as StatusField;
			field.Text = this.TextInfoZoom;
			field.Invalidate();
		}

		// Appelé par le document lorsque l'outil a changé.
		private void HandleToolChanged()
		{
			bool isCreating   = this.document.Modifier.ActiveViewer.IsCreating;
			string tool = this.document.Modifier.Tool;
			Widget[] toolWidgets = Widget.FindAllCommandWidgets("SelectTool", this.commandDispatcher);
			foreach ( Widget widget in toolWidgets )
			{
				widget.ActiveState = ( widget.Name == tool ) ? WidgetState.ActiveYes : WidgetState.ActiveNo;
				widget.SetEnabled(widget.Name == tool || !isCreating);
			}
		}

		// Appelé par le document lorsque l'état "enregistrer" a changé.
		private void HandleSaveChanged()
		{
			this.saveState.Enabled = this.document.IsDirtySerialize;
			this.saveAsState.Enabled = true;
		}

		// Appelé par le document lorsque la sélection a changé.
		private void HandleSelectionChanged()
		{
			viewer = this.document.Modifier.ActiveViewer;

			this.containerProperties.SetDirtyContent();
#if DEBUG
			this.containerAutos.SetDirtyContent();
#endif
			this.containerOper.SetDirtyContent();

			int totalSelected  = this.document.Modifier.TotalSelected;
			int totalHide      = this.document.Modifier.TotalHide;
			int totalPageHide  = this.document.Modifier.TotalPageHide;
			int totalObjects   = this.document.Modifier.TotalObjects;
			bool isCreating    = this.document.Modifier.ActiveViewer.IsCreating;
			bool isBase        = viewer.DrawingContext.RootStackIsBase;
			AbstractObject one = this.document.Modifier.RetOnlySelectedObject();

			this.newState.Enabled = true;
			this.openState.Enabled = true;
			this.deleteState.Enabled = ( totalSelected > 0 || isCreating );
			this.duplicateState.Enabled = ( totalSelected > 0 && !isCreating );
			this.cutState.Enabled = ( totalSelected > 0 && !isCreating );
			this.copyState.Enabled = ( totalSelected > 0 && !isCreating );
			this.pasteState.Enabled = ( !this.document.Modifier.IsClipboardEmpty() && !isCreating );
			this.orderUpState.Enabled = ( totalObjects > 1 && totalSelected > 0 && !isCreating );
			this.orderDownState.Enabled = ( totalObjects > 1 && totalSelected > 0 && !isCreating );
			this.rotate90State.Enabled = ( totalSelected > 0 && !isCreating );
			this.rotate180State.Enabled = ( totalSelected > 0 && !isCreating );
			this.rotate270State.Enabled = ( totalSelected > 0 && !isCreating );
			this.mirrorHState.Enabled = ( totalSelected > 0 && !isCreating );
			this.mirrorVState.Enabled = ( totalSelected > 0 && !isCreating );
			this.zoomMul2State.Enabled = ( totalSelected > 0 && !isCreating );
			this.zoomDiv2State.Enabled = ( totalSelected > 0 && !isCreating );
			this.mergeState.Enabled = ( totalSelected > 1 && !isCreating );
			this.groupState.Enabled = ( totalSelected > 0 && !isCreating );
			this.ungroupState.Enabled = ( totalSelected == 1 && one is ObjectGroup && !isCreating );
			this.insideState.Enabled = ( totalSelected == 1 && one is ObjectGroup && !isCreating );
			this.outsideState.Enabled = ( !isBase && !isCreating );

			this.hideSelState.Enabled = ( totalSelected > 0 && !isCreating );
			this.hideRestState.Enabled = ( totalObjects-totalSelected-totalHide > 0 && !isCreating );
			this.hideCancelState.Enabled = ( totalPageHide > 0 && !isCreating );

			this.zoomSelState.Enabled = ( totalSelected > 0 );

			this.deselectState.Enabled = ( totalSelected > 0 );
			this.selectAllState.Enabled = ( totalSelected < totalObjects-totalHide );
			this.selectInvertState.Enabled = ( totalObjects > 0 );

			this.selectGlobalState.Enabled = ( totalSelected > 0 );
			this.selectGlobalState.ActiveState = ( totalSelected > 0 && viewer.GlobalSelect ) ? WidgetState.ActiveYes : WidgetState.ActiveNo;
			
			this.selectPartialState.ActiveState = viewer.PartialSelect ? WidgetState.ActiveYes : WidgetState.ActiveNo;

			StatusField field = this.info.Items["StatusObject"] as StatusField;
			field.Text = this.TextInfoObject;
			field.Invalidate();
		}

		// Appelé lorsque la création d'un objet à débuté ou s'est terminée.
		private void HandleCreateChanged()
		{
			this.HandleSelectionChanged();
			this.HandlePagesChanged();
			this.HandleLayersChanged();
			this.HandleUndoRedoChanged();
			this.HandleToolChanged();
		}

		// Appelé par le document lorsqu'un style a changé.
		private void HandleStyleChanged()
		{
			this.containerStyles.SetDirtyContent();
		}

		// Appelé par le document lorsque les pages ont changé.
		private void HandlePagesChanged()
		{
			this.containerPages.SetDirtyContent();

			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			int cp = context.CurrentPage;
			int tp = context.TotalPages();

			bool isCreating = this.document.Modifier.ActiveViewer.IsCreating;

			this.pagePrev.Enabled = (cp > 0 && !isCreating );
			this.pageNext.Enabled = (cp < tp-1 && !isCreating );
			this.pageMenu.Enabled = (tp > 1 && !isCreating );
			this.pageCreate.Enabled = !isCreating;
			this.pageDelete.Enabled = (tp > 1 && !isCreating );
			this.pageUp.Enabled = (cp > 0 && !isCreating );
			this.pageDown.Enabled = (cp < tp-1 && !isCreating );

			this.quickPageMenu.Text = (cp+1).ToString();
		}

		// Appelé par le document lorsque les calques ont changé.
		private void HandleLayersChanged()
		{
			this.containerLayers.SetDirtyContent();

			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			int cl = context.CurrentLayer;
			int tl = context.TotalLayers();

			bool isCreating = this.document.Modifier.ActiveViewer.IsCreating;

			this.layerPrev.Enabled = (cl > 0 && !isCreating );
			this.layerNext.Enabled = (cl < tl-1 && !isCreating );
			this.layerMenu.Enabled = (tl > 1 && !isCreating );
			this.layerCreate.Enabled = !isCreating ;
			this.layerDelete.Enabled = (tl > 1 && !isCreating );
			this.layerUp.Enabled = (cl > 0 && !isCreating );
			this.layerDown.Enabled = (cl < tl-1 && !isCreating );

			this.quickLayerMenu.Text = ((char)('A'+cl)).ToString();
		}

		// Appelé par le document lorsqu'un nom de page a changé.
		private void HandlePageChanged(AbstractObject page)
		{
			this.containerPages.SetDirtyObject(page);
		}

		// Appelé par le document lorsqu'un nom de calque a changé.
		private void HandleLayerChanged(AbstractObject layer)
		{
			this.containerLayers.SetDirtyObject(layer);
		}

		// Appelé par le document lorsque l'état des commande undo/redo a changé.
		private void HandleUndoRedoChanged()
		{
			bool isCreating = this.document.Modifier.ActiveViewer.IsCreating;
			this.undoState.Enabled = ( this.document.Modifier.OpletQueue.CanUndo && !isCreating );
			this.redoState.Enabled = ( this.document.Modifier.OpletQueue.CanRedo && !isCreating );
		}

		// Appelé par le document lorsque l'état de la grille a changé.
		private void HandleGridChanged()
		{
			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			this.gridState.ActiveState = context.GridActive ? WidgetState.ActiveYes : WidgetState.ActiveNo;
		}

		// Appelé par le document lorsque l'état de l'aperçu a changé.
		private void HandlePreviewChanged()
		{
			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			this.previewState.ActiveState = context.PreviewActive ? WidgetState.ActiveYes : WidgetState.ActiveNo;
		}

		// Appelé par le document lorsque l'état de la commande "hide half" a changé.
		private void HandleHideHalfChanged()
		{
			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			this.hideHalfState.ActiveState = context.HideHalfActive ? WidgetState.ActiveYes : WidgetState.ActiveNo;
		}

		// Appelé par le document lorsque l'état des commande de debug a changé.
		private void HandleDebugChanged()
		{
			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			this.debugBboxThinState.ActiveState = context.IsDrawBoxThin ? WidgetState.ActiveYes : WidgetState.ActiveNo;
			this.debugBboxGeomState.ActiveState = context.IsDrawBoxGeom ? WidgetState.ActiveYes : WidgetState.ActiveNo;
			this.debugBboxFullState.ActiveState = context.IsDrawBoxFull ? WidgetState.ActiveYes : WidgetState.ActiveNo;
		}

		// Appelé lorsqu'une propriété a changé.
		private void HandlePropertyChanged(System.Collections.ArrayList propertyList)
		{
			this.containerProperties.SetDirtyProperties(propertyList);
			this.containerStyles.SetDirtyProperties(propertyList);
		}

		// Appelé par le document lorsque le dessin a changé.
		private void HandleDrawChanged(Viewer viewer, Rectangle rect)
		{
			Rectangle box = rect;

			if ( viewer.DrawingContext.IsActive )
			{
				box.Inflate(viewer.DrawingContext.HandleSize/2);
			}

			box = viewer.InternalToScreen(box);
			this.InvalidateDraw(viewer, box);
		}


		// Invalide une partie de la zone de dessin d'un visualisateur.
		protected void InvalidateDraw(Viewer viewer, Rectangle bbox)
		{
			if ( bbox.IsEmpty )  return;

			if ( bbox.IsInfinite )
			{
				viewer.SetSyncPaint(true);
				viewer.Invalidate();
				viewer.SetSyncPaint(false);
			}
			else
			{
				viewer.SetSyncPaint(true);
				viewer.Invalidate(bbox);
				viewer.SetSyncPaint(false);
			}
		}

		// Met à jour les ascenseurs.
		protected void UpdateScroller()
		{
			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			Size size = this.document.Size;
			Size area = this.document.Modifier.SizeArea;

			this.hScroller.MinValue = (decimal) context.MinOriginX;
			this.hScroller.MaxValue = (decimal) context.MaxOriginX;
			this.hScroller.VisibleRangeRatio = (decimal) ((size.Width/area.Width)/context.Zoom);
			this.hScroller.Value = (decimal) (-context.OriginX);
			//?context.OriginX = (double) (-this.hScroller.Value);

			this.vScroller.MinValue = (decimal) context.MinOriginY;
			this.vScroller.MaxValue = (decimal) context.MaxOriginY;
			this.vScroller.VisibleRangeRatio = (decimal) ((size.Height/area.Height)/context.Zoom);
			this.vScroller.Value = (decimal) (-context.OriginY);
			//?context.OriginY = (double) (-this.vScroller.Value);
		}


		// Appelé lorsque le look a changé.
		protected void UpdateLookCommand()
		{
			string lookName = Epsitec.Common.Widgets.Adorner.Factory.ActiveName;
			Widget[] lookWidgets = Widget.FindAllCommandWidgets("SelectLook", this.commandDispatcher);
			foreach ( Widget widget in lookWidgets )
			{
				widget.ActiveState = ( widget.Name == lookName ) ? WidgetState.ActiveYes : WidgetState.ActiveNo;
			}
		}


		// Texte pour le document.
		protected string TextDocument
		{
			get
			{
				string name = Misc.ExtractName(this.document.Filename);
				Size size = this.document.Size;
				return string.Format("{0} ({1}x{2})", name, size.Width, size.Height);
			}
		}

		// Texte pour les informations.
		protected string TextInfoObject
		{
			get
			{
				int sel   = this.document.Modifier.TotalSelected;
				int hide  = this.document.Modifier.TotalHide;
				int total = this.document.Modifier.TotalObjects;
				int deep  = this.document.Modifier.ActiveViewer.DrawingContext.RootStackDeep;

				string sDeep = "Sélection";
				if ( deep > 2 )
				{
					sDeep = string.Format("Niveau {0}", deep-2);
				}

				string sHide = "";
				if ( hide > 0 )
				{
					sHide = string.Format("-{0}", hide);
				}

				return string.Format("{0}: {1}/{2}{3}", sDeep, sel, total, sHide);
			}
		}

		// Texte pour les informations.
		protected string TextInfoMouse
		{
			get
			{
				Point mouse = this.document.Modifier.ActiveViewer.MousePos();
				return string.Format("(x:{0} y:{1})", mouse.X.ToString("F2"), mouse.Y.ToString("F2"));
			}
		}

		// Texte pour les informations.
		protected string TextInfoZoom
		{
			get
			{
				DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
				double zoom = context.Zoom;
				return string.Format("Zoom {0}%", (zoom*100).ToString("F0"));
			}
		}


		protected DocumentType			type;
		protected Document				clipboard;
		protected Document				document;

		protected CommandDispatcher		commandDispatcher;
		protected bool					allWidgets = false;

		protected HMenu					menu;
		protected HToolBar				hToolBar;
		protected VToolBar				vToolBar;
		protected Widget				root;
		protected StatusBar				info;
		protected PaneBook				pane;
		protected PanePage				leftPane;
		protected PanePage				rightPane;
		protected TabBook				book;
		protected TabPage				bookProperties;
		protected TabPage				bookStyles;
		protected TabPage				bookAutos;
		protected TabPage				bookPages;
		protected TabPage				bookLayers;
		protected TabPage				bookOper;
		protected ContainerProperties	containerProperties;
		protected ContainerStyles		containerStyles;
		protected ContainerAutos		containerAutos;
		protected ContainerPages		containerPages;
		protected ContainerLayers		containerLayers;
		protected ContainerOper			containerOper;
		protected Viewer				viewer;
		protected Viewer				frame1;
		protected Viewer				frame2;
		protected Viewer				clipboardViewer;

		protected GlyphButton			quickPagePrev;
		protected Button				quickPageMenu;
		protected GlyphButton			quickPageNext;
		protected HScroller				hScroller;

		protected GlyphButton			quickLayerPrev;
		protected Button				quickLayerMenu;
		protected GlyphButton			quickLayerNext;
		protected VScroller				vScroller;

		protected double				panelsWidth = 235;

		protected string				saveAsFilename;

		protected CommandState			newState;
		protected CommandState			openState;
		protected CommandState			saveState;
		protected CommandState			saveAsState;
		protected CommandState			deleteState;
		protected CommandState			duplicateState;
		protected CommandState			cutState;
		protected CommandState			copyState;
		protected CommandState			pasteState;
		protected CommandState			orderUpState;
		protected CommandState			orderDownState;
		protected CommandState			rotate90State;
		protected CommandState			rotate180State;
		protected CommandState			rotate270State;
		protected CommandState			mirrorHState;
		protected CommandState			mirrorVState;
		protected CommandState			zoomMul2State;
		protected CommandState			zoomDiv2State;
		protected CommandState			mergeState;
		protected CommandState			groupState;
		protected CommandState			ungroupState;
		protected CommandState			insideState;
		protected CommandState			outsideState;
		protected CommandState			undoState;
		protected CommandState			redoState;
		protected CommandState			deselectState;
		protected CommandState			selectAllState;
		protected CommandState			selectInvertState;
		protected CommandState			selectGlobalState;
		protected CommandState			selectPartialState;
		protected CommandState			hideHalfState;
		protected CommandState			hideSelState;
		protected CommandState			hideRestState;
		protected CommandState			hideCancelState;
		protected CommandState			zoomMinState;
		protected CommandState			zoomDefaultState;
		protected CommandState			zoomSelState;
		protected CommandState			zoomPrevState;
		protected CommandState			zoomSubState;
		protected CommandState			zoomAddState;
		protected CommandState			previewState;
		protected CommandState			gridState;
		protected CommandState			modeState;
		protected CommandState			arrayOutlineFrameState;
		protected CommandState			arrayOutlineHorizState;
		protected CommandState			arrayOutlineVertiState;
		protected CommandState			arrayAddColumnLeftState;
		protected CommandState			arrayAddColumnRightState;
		protected CommandState			arrayAddRowTopState;
		protected CommandState			arrayAddRowBottomState;
		protected CommandState			arrayDelColumnState;
		protected CommandState			arrayDelRowState;
		protected CommandState			arrayAlignColumnState;
		protected CommandState			arrayAlignRowState;
		protected CommandState			arraySwapColumnState;
		protected CommandState			arraySwapRowState;
		protected CommandState			arrayLookState;
		protected CommandState			debugBboxThinState;
		protected CommandState			debugBboxGeomState;
		protected CommandState			debugBboxFullState;
		protected CommandState			pagePrev;
		protected CommandState			pageNext;
		protected CommandState			pageMenu;
		protected CommandState			pageCreate;
		protected CommandState			pageDelete;
		protected CommandState			pageUp;
		protected CommandState			pageDown;
		protected CommandState			layerPrev;
		protected CommandState			layerNext;
		protected CommandState			layerMenu;
		protected CommandState			layerCreate;
		protected CommandState			layerDelete;
		protected CommandState			layerUp;
		protected CommandState			layerDown;
	}
}
