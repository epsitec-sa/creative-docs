using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Document;
using System.IO;

namespace Epsitec.App.DocumentEditor
{
	using Drawing    = Common.Drawing;
	using Widgets    = Common.Widgets;
	using Dialogs    = Common.Dialogs;
	using Containers = Common.Document.Containers;
	using Objects    = Common.Document.Objects;

	/// <summary>
	/// La classe DocumentEditor représente l'éditeur de document complet.
	/// </summary>
	
	[SuppressBundleSupport]
	
	public class DocumentEditor : Widgets.Widget
	{
		public DocumentEditor(DocumentType type)
		{
			this.type = type;
			this.useArray = false;

			this.CreateLayout();
			this.InitCommands();

			this.clipboard = new Document(this.type, DocumentMode.Clipboard);
			this.clipboard.Name = "Clipboard";

			this.documents = new System.Collections.ArrayList();
			this.currentDocument = -1;
			this.CreateDocument();

			string[] args = System.Environment.GetCommandLineArgs();
			if ( args.Length >= 2 )
			{
				string err = this.CurrentDocument.Read(args[1]);
				this.DialogError(this.commandDispatcher, err);
			}

			this.CurrentDocument.Notifier.NotifyAllChanged();
			this.CurrentDocument.Notifier.GenerateEvents();
		}

		protected override void Dispose(bool disposing)
		{
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
					this.commandDispatcher.RegisterController(this);
				}
				
				return this.commandDispatcher;
			}
		}

		public void AsyncNotify()
		{
			if ( this.currentDocument < 0 )  return;
			this.CurrentDocument.Notifier.GenerateEvents();
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
			if ( this.useArray )
			{
				this.menu.Items.Add(new MenuItem("", "Tableau"));
			}
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
			this.MenuAdd(fileMenu, @"", "Close", "Fermer", "");
			this.MenuAdd(fileMenu, @"", "CloseAll", "Fermer tout", "");
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
			this.MenuAdd(operMenu, @"", "ZoomDiv2", "Réduction /2", "");
			this.MenuAdd(operMenu, @"", "ZoomMul2", "Agrandissement x2", "");
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
			showMenu.Items[3].Submenu = zoomMenu;

			VMenu lookMenu = new VMenu();
			lookMenu.Name = "Look";
			lookMenu.Host = this;
			string[] list = Widgets.Adorner.Factory.AdornerNames;
			foreach ( string name in list )
			{
				this.MenuAdd(lookMenu, @"y/n", "SelectLook(this.Name)", name, "", name);
			}
			lookMenu.AdjustSize();
			showMenu.Items[5].Submenu = lookMenu;

			if ( this.useArray )
			{
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
					if ( !Objects.Array.CommandLook(j, out text, out name) )  break;
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
			}

			VMenu docMenu = new VMenu();
			docMenu.Name = "Document";
			docMenu.Host = this;
			this.MenuAdd(docMenu, @"file:images/settings.icon", "Settings", "Réglages...", "");
			this.MenuAdd(docMenu, @"file:images/infos.icon", "Infos", "Informations...", "");
			this.MenuAdd(docMenu, @"", "", "", "");
			this.MenuAdd(docMenu, @"file:images/pagenew.icon", "PageCreate", "Nouvelle page", "");
			this.MenuAdd(docMenu, @"file:images/delete.icon", "PageDelete", "Supprimer la page", "");
			this.MenuAdd(docMenu, @"file:images/up.icon", "PageUp", "Reculer la page", "");
			this.MenuAdd(docMenu, @"file:images/down.icon", "PageDown", "Avancer la page", "");
			this.MenuAdd(docMenu, @"", "", "", "");
			this.MenuAdd(docMenu, @"file:images/layernew.icon", "LayerCreate", "Nouveau calque", "");
			this.MenuAdd(docMenu, @"file:images/delete.icon", "LayerDelete", "Supprimer le calque", "");
			this.MenuAdd(docMenu, @"file:images/up.icon", "LayerDown", "Monter le calque", "");
			this.MenuAdd(docMenu, @"file:images/down.icon", "LayerUp", "Descendre le calque", "");
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
			this.MenuAdd(helpMenu, @"", "AboutApplication", "A propos de...", "");
			helpMenu.AdjustSize();
			this.menu.Items[i++].Submenu = helpMenu;

			this.hToolBar = new HToolBar(this);
			this.hToolBar.Anchor = AnchorStyles.LeftAndRight | AnchorStyles.Top;
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
			this.HToolBarAdd(@"file:images/settings.icon", "Settings", "Réglages...");
			this.HToolBarAdd(@"file:images/infos.icon", "Infos", "Informations...");
			this.HToolBarAdd("", "", "");
			if ( this.useArray )
			{
				this.HToolBarAdd(@"file:images/arrayframe.icon", "ArrayOutlineFrame", "Modifie le cadre");
				this.HToolBarAdd(@"file:images/arrayhoriz.icon", "ArrayOutlineHoriz", "Modifie l'intérieur horizontal");
				this.HToolBarAdd(@"file:images/arrayverti.icon", "ArrayOutlineVerti", "Modifie l'intérieur vertical");
				this.HToolBarAdd("", "", "");
			}

			this.info = new StatusBar(this);
			this.info.Anchor = AnchorStyles.LeftAndRight | AnchorStyles.Bottom;
			this.InfoAdd("", 120, "StatusDocument", "");
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

			this.vToolBar = new VToolBar(this);
			this.vToolBar.Anchor = AnchorStyles.TopAndBottom | AnchorStyles.Left;
			this.vToolBar.AnchorMargins = new Margins(0, 0, this.hToolBar.Height, this.info.Height);
			this.VToolBarAdd(@"file:images/select.icon", "SelectTool(this.Name)", "Sélectionner", "Select");
			this.VToolBarAdd(@"file:images/global.icon", "SelectTool(this.Name)", "Rectangle de sélection", "Global");
			this.VToolBarAdd(@"file:images/edit.icon", "SelectTool(this.Name)", "Editer", "Edit");
			this.VToolBarAdd(@"file:images/zoom.icon", "SelectTool(this.Name)", "Agrandir", "Zoom");
			this.VToolBarAdd(@"file:images/hand.icon", "SelectTool(this.Name)", "Déplacer", "Hand");
			this.VToolBarAdd(@"file:images/picker.icon", "SelectTool(this.Name)", "Pipette", "Picker");
			if ( this.type == DocumentType.Pictogram )
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
			if ( this.useArray )
			{
				this.VToolBarAdd(@"file:images/array.icon", "SelectTool(this.Name)", "Tableau", "ObjectArray");
			}
			this.VToolBarAdd(@"file:images/image.icon", "SelectTool(this.Name)", "Image bitmap", "ObjectImage");
			this.VToolBarAdd("", "", "");

			this.bookDocuments = new TabBook(this);
			this.bookDocuments.Width = this.panelsWidth;
			this.bookDocuments.Anchor = AnchorStyles.All;
			this.bookDocuments.AnchorMargins = new Margins(this.vToolBar.Width+1, this.panelsWidth+2, this.hToolBar.Height+1, this.info.Height+1);
			this.bookDocuments.Arrows = TabBookArrows.Right;
			this.bookDocuments.HasCloseButton = true;
			this.bookDocuments.CloseButton.Command = "Close";
			this.bookDocuments.ActivePageChanged += new EventHandler(this.HandleBookDocumentsActivePageChanged);
			ToolTip.Default.SetToolTip(this.bookDocuments.CloseButton, "Fermer le document");
		}

		protected void CreateDocumentLayout(Document document)
		{
			DocumentInfo di = this.CurrentDocumentInfo;
			double sw = 17;  // largeur d'un ascenseur
			double wm = 4;  // marges autour du viewer
			
			di.tabPage = new TabPage();
			this.bookDocuments.Items.Insert(this.currentDocument, di.tabPage);

			Widget mainViewParent;
			if ( document.Type == DocumentType.Pictogram )
			{
				PaneBook pane = new PaneBook(di.tabPage);
				pane.Anchor = AnchorStyles.All;
				pane.PaneBookStyle = PaneBookStyle.LeftRight;
				pane.PaneBehaviour = PaneBookBehaviour.FollowMe;

				PanePage leftPane = new PanePage();
				leftPane.PaneRelativeSize = 10;
				leftPane.PaneElasticity = 1;
				leftPane.PaneMinSize = 100;
				pane.Items.Add(leftPane);

				PanePage rightPane = new PanePage();
				rightPane.PaneAbsoluteSize = 40;
				rightPane.PaneElasticity = 0;
				rightPane.PaneMinSize = 40;
				rightPane.PaneMaxSize = 200;
				pane.Items.Add(rightPane);

				mainViewParent = leftPane;
				Viewer viewer = new Viewer(document);
				viewer.Parent = mainViewParent;
				viewer.Anchor = AnchorStyles.All;
				viewer.AnchorMargins = new Margins(wm, wm+sw+1, 10, wm+sw+1);
				document.Modifier.ActiveViewer = viewer;
				document.Modifier.AttachViewer(viewer);

				Viewer frame1 = new Viewer(document);
				frame1.Parent = rightPane;
				frame1.Anchor = AnchorStyles.LeftAndRight | AnchorStyles.Top;
				frame1.AnchorMargins = new Margins(wm, wm, 10, wm);
				frame1.DrawingContext.LayerDrawingMode = LayerDrawingMode.ShowInactive;
				document.Modifier.AttachViewer(frame1);

				Viewer frame2 = new Viewer(document);
				frame2.Parent = rightPane;
				frame2.Anchor = AnchorStyles.LeftAndRight | AnchorStyles.Top;
				frame2.AnchorMargins = new Margins(wm, wm, 10+30, wm);
				frame2.DrawingContext.LayerDrawingMode = LayerDrawingMode.ShowInactive;
				document.Modifier.AttachViewer(frame2);
			}
			else
			{
				mainViewParent = di.tabPage;
				Viewer viewer = new Viewer(document);
				viewer.Parent = mainViewParent;
				viewer.Anchor = AnchorStyles.All;
				viewer.AnchorMargins = new Margins(wm, wm+sw+1, 10, wm+sw+1);
				document.Modifier.ActiveViewer = viewer;
				document.Modifier.AttachViewer(viewer);
			}

			// Bande horizontale qui contient les boutons des pages et l'ascenseur.
			Widget hBand = new Widget(mainViewParent);
			hBand.Height = sw;
			hBand.Anchor = AnchorStyles.LeftAndRight | AnchorStyles.Bottom;
			hBand.AnchorMargins = new Margins(wm, wm+sw+1, 0, wm);

			GlyphButton quickPagePrev = new GlyphButton("PagePrev");
			quickPagePrev.Parent = hBand;
			quickPagePrev.GlyphShape = GlyphShape.ArrowLeft;
			quickPagePrev.Width = sw;
			quickPagePrev.Height = sw;
			quickPagePrev.Dock = DockStyle.Left;
			ToolTip.Default.SetToolTip(quickPagePrev, "Page précédente");

			di.quickPageMenu = new Button(hBand);
			di.quickPageMenu.Command = "PageMenu";
			di.quickPageMenu.Clicked += new MessageEventHandler(this.HandleQuickPageMenu);
			di.quickPageMenu.Width = System.Math.Floor(sw*1.5);
			di.quickPageMenu.Height = sw;
			di.quickPageMenu.Dock = DockStyle.Left;
			ToolTip.Default.SetToolTip(di.quickPageMenu, "Choix d'une page");

			GlyphButton quickPageNext = new GlyphButton("PageNext");
			quickPageNext.Parent = hBand;
			quickPageNext.GlyphShape = GlyphShape.ArrowRight;
			quickPageNext.Width = sw;
			quickPageNext.Height = sw;
			quickPageNext.Dock = DockStyle.Left;
			ToolTip.Default.SetToolTip(quickPageNext, "Page suivante");

			di.hScroller = new HScroller(hBand);
			di.hScroller.ValueChanged += new EventHandler(this.HandleHScrollerValueChanged);
			di.hScroller.Dock = DockStyle.Fill;
			di.hScroller.DockMargins = new Margins(2, 0, 0, 0);

			// Bande verticale qui contient les boutons des calques et l'ascenseur.
			Widget vBand = new Widget(mainViewParent);
			vBand.Width = sw;
			vBand.Anchor = AnchorStyles.TopAndBottom | AnchorStyles.Right;
			vBand.AnchorMargins = new Margins(0, wm, 10, wm+sw+1);

			GlyphButton quickLayerNext = new GlyphButton("LayerNext");
			quickLayerNext.Parent = vBand;
			quickLayerNext.GlyphShape = GlyphShape.ArrowUp;
			quickLayerNext.Width = sw;
			quickLayerNext.Height = sw;
			quickLayerNext.Dock = DockStyle.Top;
			ToolTip.Default.SetToolTip(quickLayerNext, "Calque suivant");

			di.quickLayerMenu = new Button(vBand);
			di.quickLayerMenu.Command = "LayerMenu";
			di.quickLayerMenu.Clicked += new MessageEventHandler(this.HandleQuickLayerMenu);
			di.quickLayerMenu.Width = sw;
			di.quickLayerMenu.Height = sw;
			di.quickLayerMenu.Dock = DockStyle.Top;
			ToolTip.Default.SetToolTip(di.quickLayerMenu, "Choix d'un calque");

			GlyphButton quickLayerPrev = new GlyphButton("LayerPrev");
			quickLayerPrev.Parent = vBand;
			quickLayerPrev.GlyphShape = GlyphShape.ArrowDown;
			quickLayerPrev.Width = sw;
			quickLayerPrev.Height = sw;
			quickLayerPrev.Dock = DockStyle.Top;
			ToolTip.Default.SetToolTip(quickLayerPrev, "Calque précédent");

			di.vScroller = new VScroller(vBand);
			di.vScroller.ValueChanged += new EventHandler(this.HandleVScrollerValueChanged);
			di.vScroller.Dock = DockStyle.Fill;
			di.vScroller.DockMargins = new Margins(0, 0, 2, 0);

			di.bookPanels = new TabBook(this);
			di.bookPanels.Width = this.panelsWidth;
			di.bookPanels.Anchor = AnchorStyles.TopAndBottom | AnchorStyles.Right;
			di.bookPanels.AnchorMargins = new Margins(1, 1, this.hToolBar.Height+1, this.info.Height+1);
			di.bookPanels.Arrows = TabBookArrows.Stretch;

			TabPage bookPrincipal = new TabPage();
			bookPrincipal.TabTitle = "Attributs";
			di.bookPanels.Items.Add(bookPrincipal);

			TabPage bookStyles = new TabPage();
			bookStyles.TabTitle = "Styles";
			di.bookPanels.Items.Add(bookStyles);

#if DEBUG
			TabPage bookAutos = new TabPage();
			bookAutos.TabTitle = "Auto";
			di.bookPanels.Items.Add(bookAutos);
#endif

			TabPage bookPages = new TabPage();
			bookPages.TabTitle = "Pages";
			di.bookPanels.Items.Add(bookPages);

			TabPage bookLayers = new TabPage();
			bookLayers.TabTitle = "Calques";
			di.bookPanels.Items.Add(bookLayers);

			TabPage bookOper = new TabPage();
			bookOper.TabTitle = "Op";
			di.bookPanels.Items.Add(bookOper);

			di.bookPanels.ActivePage = bookPrincipal;

			di.containerPrincipal = new Containers.Principal(document);
			di.containerPrincipal.Parent = bookPrincipal;
			di.containerPrincipal.Dock = DockStyle.Fill;
			di.containerPrincipal.DockMargins = new Margins(4, 4, 10, 4);
			document.Modifier.AttachContainer(di.containerPrincipal);

			di.containerStyles = new Containers.Styles(document);
			di.containerStyles.Parent = bookStyles;
			di.containerStyles.Dock = DockStyle.Fill;
			di.containerStyles.DockMargins = new Margins(4, 4, 10, 4);
			document.Modifier.AttachContainer(di.containerStyles);

#if DEBUG
			di.containerAutos = new Containers.Autos(document);
			di.containerAutos.Parent = bookAutos;
			di.containerAutos.Dock = DockStyle.Fill;
			di.containerAutos.DockMargins = new Margins(4, 4, 10, 4);
			document.Modifier.AttachContainer(di.containerAutos);
#endif

			di.containerPages = new Containers.Pages(document);
			di.containerPages.Parent = bookPages;
			di.containerPages.Dock = DockStyle.Fill;
			di.containerPages.DockMargins = new Margins(4, 4, 10, 4);
			document.Modifier.AttachContainer(di.containerPages);

			di.containerLayers = new Containers.Layers(document);
			di.containerLayers.Parent = bookLayers;
			di.containerLayers.Dock = DockStyle.Fill;
			di.containerLayers.DockMargins = new Margins(4, 4, 10, 4);
			document.Modifier.AttachContainer(di.containerLayers);

			di.containerOperations = new Containers.Operations(document);
			di.containerOperations.Parent = bookOper;
			di.containerOperations.Dock = DockStyle.Fill;
			di.containerOperations.DockMargins = new Margins(4, 4, 10, 4);
			document.Modifier.AttachContainer(di.containerOperations);

			this.bookDocuments.ActivePage = di.tabPage;
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
			HScroller scroller = sender as HScroller;
			Viewer viewer = this.CurrentDocument.Modifier.ActiveViewer;
			viewer.DrawingContext.OriginX = (double) -scroller.Value;
		}

		private void HandleVScrollerValueChanged(object sender)
		{
			VScroller scroller = sender as VScroller;
			Viewer viewer = this.CurrentDocument.Modifier.ActiveViewer;
			viewer.DrawingContext.OriginY = (double) -scroller.Value;
		}


		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();
		}


		[Command ("SelectTool")]
		void CommandActivateTool(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.Tool = e.CommandArgs[0];
		}

		[Command ("SelectLook")]
		void CommandSelectLook(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			Widgets.Adorner.Factory.SetActive(e.CommandArgs[0]);
			this.UpdateLookCommand();
		}
		
		#region IO
		// Affiche le dialogue pour demander s'il faut enregistrer le
		// document modifié, avant de passer à un autre document.
		protected Dialogs.DialogResult DialogSave(CommandDispatcher dispatcher)
		{
			if ( !this.CurrentDocument.IsDirtySerialize ||
				 this.CurrentDocument.Modifier.StatisticTotalObjects() == 0 )
			{
				return Dialogs.DialogResult.None;
			}

			string title = "Crésus";
			string icon = "manifest:Epsitec.Common.Dialogs.Images.Warning.icon";

			string shortFilename = Misc.ExtractName(this.CurrentDocument.Filename, this.CurrentDocument.IsDirtySerialize);
			string statistic = string.Format("<font size=\"80%\">{0}</font><br/>", this.CurrentDocument.Modifier.Statistic(false, false));
			string message = string.Format("<font size=\"100%\">Le fichier {0} a été modifié.</font><br/><br/>{1}Voulez-vous enregistrer les modifications ?", shortFilename, statistic);
			Dialogs.IDialog dialog = Dialogs.Message.CreateYesNoCancel(title, icon, message, "", "", dispatcher);
			dialog.Owner = this.Window;
			dialog.OpenDialog();
			return dialog.Result;
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
		protected Dialogs.DialogResult DialogDelete(CommandDispatcher dispatcher, string filename)
		{
			string title = "Crésus";
			string icon = "manifest:Epsitec.Common.Dialogs.Images.Warning.icon";

			string shortFilename = string.Format("<b>{0}</b>", Misc.ExtractName(filename));
			string statistic = string.Format("<font size=\"80%\">{0}</font><br/>", DocumentEditor.StatisticFilename(filename));
			string message = string.Format("<font size=\"100%\">Le fichier {0} existe déjà.</font><br/><br/>{1}Voulez-vous le remplacer ?", shortFilename, statistic);

			Dialogs.IDialog dialog = Dialogs.Message.CreateYesNo(title, icon, message, "", "", dispatcher);
			dialog.Owner = this.Window;
			dialog.OpenDialog();
			return dialog.Result;
		}

		// Affiche le dialogue pour signaler une erreur.
		protected Dialogs.DialogResult DialogError(CommandDispatcher dispatcher, string error)
		{
			if ( error == "" )  return Dialogs.DialogResult.None;

			string title = "Crésus";
			string icon = "manifest:Epsitec.Common.Dialogs.Images.Warning.icon";
			string message = error;

			Dialogs.IDialog dialog = Dialogs.Message.CreateOk(title, icon, message, "", dispatcher);
			dialog.Owner = this.Window;
			dialog.OpenDialog();
			return dialog.Result;
		}

		// Demande un nom de fichier puis ouvre le fichier.
		// Affiche l'erreur éventuelle.
		// Retourne false si le fichier n'a pas été ouvert.
		protected bool Open(CommandDispatcher dispatcher)
		{
			Dialogs.FileOpen dialog = new Dialogs.FileOpen();
		
			dialog.FileName = "";
			if ( this.type == DocumentType.Graphic )
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
			if ( dialog.Result != Dialogs.DialogResult.Accept )  return false;

			this.CreateDocument();
			string err = this.CurrentDocument.Read(dialog.FileName);
			this.DialogError(dispatcher, err);
			return (err == "");
		}

		// Demande un nom de fichier puis enregistre le fichier.
		// Si le document a déjà un nom de fichier et que ask=false,
		// l'enregistrement est fait directement avec le nom connu.
		// Affiche l'erreur éventuelle.
		// Retourne false si le fichier n'a pas été enregistré.
		protected bool Save(CommandDispatcher dispatcher, bool ask)
		{
			string filename;

			if ( this.CurrentDocument.Filename == "" || ask )
			{
				Dialogs.FileSave dialog = new Dialogs.FileSave();
			
				dialog.FileName = this.CurrentDocument.Filename;
				if ( this.type == DocumentType.Graphic )
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
				if ( dialog.Result != Dialogs.DialogResult.Accept )  return false;
				filename = dialog.FileName;
			}
			else
			{
				filename = this.CurrentDocument.Filename;
			}

			string err = this.CurrentDocument.Write(filename);
			this.DialogError(dispatcher, err);
			return (err == "");
		}

		// Fait tout ce qu'il faut pour éventuellement sauvegarder le document
		// avant de passer à autre chose.
		// Retourne false si on ne peut pas continuer.
		protected bool AutoSave(CommandDispatcher dispatcher)
		{
			Dialogs.DialogResult result = this.DialogSave(dispatcher);
			if ( result == Dialogs.DialogResult.Yes )
			{
				return this.Save(dispatcher, false);
			}
			if ( result == Dialogs.DialogResult.Cancel )
			{
				return false;
			}
			return true;
		}

		// Fait tout ce qu'il faut pour éventuellement sauvegarder tous les
		// documents avant de passer à autre chose.
		// Retourne false si on ne peut pas continuer.
		protected bool AutoSaveAll(CommandDispatcher dispatcher)
		{
			int cd = this.currentDocument;

			int total = this.bookDocuments.PageCount;
			for ( int i=0 ; i<total ; i++ )
			{
				this.currentDocument = i;
				if ( !this.AutoSave(dispatcher) )
				{
					this.currentDocument = cd;
					return false;
				}
			}

			this.currentDocument = cd;
			return true;
		}
		#endregion

		[Command ("New")]
		void CommandNew(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CreateDocument();
			this.CurrentDocument.Modifier.New();
		}

		[Command ("Open")]
		void CommandOpen(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.Open(dispatcher);
		}

		[Command ("Save")]
		void CommandSave(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.Save(dispatcher, false);
		}
		
		[Command ("SaveAs")]
		void CommandSaveAs(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.Save(dispatcher, true);
		}
		
		[Command ("Close")]
		void CommandClose(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			if ( !this.AutoSave(dispatcher) )  return;
			this.CloseDocument();
		}

		[Command ("CloseAll")]
		void CommandCloseAll(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			if ( !this.AutoSaveAll(dispatcher) )  return;
			while ( this.IsCurrentDocument )
			{
				this.CloseDocument();
			}
		}

		[Command ("QuitApplication")]
		void CommandQuitApplication(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			if ( !this.AutoSaveAll(dispatcher) )  return;
			this.QuitApplication();
		}

		[Command ("Print")]
		void CommandPrint(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			Dialogs.Print dialog = new Dialogs.Print();
			dialog.Document.SelectPrinter(this.printerName);
			dialog.AllowFromPageToPage = true;
			dialog.AllowSelectedPages  = false;
			int total = this.CurrentDocument.Modifier.StatisticTotalPages();
			dialog.Document.PrinterSettings.MinimumPage = 1;
			dialog.Document.PrinterSettings.MaximumPage = total;
			dialog.Document.PrinterSettings.FromPage = 1;
			dialog.Document.PrinterSettings.ToPage = total;
			dialog.Document.PrinterSettings.PrintRange = Common.Printing.PrintRange.AllPages;
			dialog.Document.PrinterSettings.Collate = false;
			dialog.Owner = this.Window;
			dialog.OpenDialog();
			if ( dialog.Result != Dialogs.DialogResult.Accept )  return;

			this.printerName = dialog.Document.PrinterSettings.PrinterName;
			this.CurrentDocument.Print(dialog);
		}
		

		[Command ("Delete")]
		void CommandDelete()
		{
			this.CurrentDocument.Modifier.DeleteSelection();
		}

		[Command ("Duplicate")]
		void CommandDuplicate(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			Point move = this.CurrentDocument.Modifier.DuplicateMove;
			this.CurrentDocument.Modifier.DuplicateSelection(move);
		}

		[Command ("Cut")]
		void CommandCut(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.CutSelection();
		}

		[Command ("Copy")]
		void CommandCopy(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.CopySelection();
		}

		[Command ("Paste")]
		void CommandPaste(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.Paste();
		}

		[Command ("Undo")]
		void CommandUndo(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.Undo();
		}

		[Command ("Redo")]
		void CommandRedo(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.Redo();
		}

		[Command ("OrderUp")]
		void CommandOrderUp(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.OrderUpSelection();
		}

		[Command ("OrderDown")]
		void CommandOrderDown(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.OrderDownSelection();
		}

		[Command ("Rotate90")]
		void CommandRotate90(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.RotateSelection(90);
		}

		[Command ("Rotate180")]
		void CommandRotate180(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.RotateSelection(180);
		}

		[Command ("Rotate270")]
		void CommandRotate270(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.RotateSelection(270);
		}

		[Command ("MirrorH")]
		void CommandMirrorH(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.MirrorSelection(true);
		}

		[Command ("MirrorV")]
		void CommandMirrorV(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.MirrorSelection(false);
		}

		[Command ("ZoomMul2")]
		void CommandZoomMul2(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.ZoomSelection(2.0);
		}

		[Command ("ZoomDiv2")]
		void CommandZoomDiv2(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.ZoomSelection(0.5);
		}

		[Command ("Merge")]
		void CommandMerge(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.MergeSelection();
		}

		[Command ("Group")]
		void CommandGroup(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.GroupSelection();
		}

		[Command ("Ungroup")]
		void CommandUngroup(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.UngroupSelection();
		}

		[Command ("Inside")]
		void CommandInside(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.InsideSelection();
		}

		[Command ("Outside")]
		void CommandOutside(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.OutsideSelection();
		}

		[Command ("Grid")]
		void CommandGrid(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
			context.GridActive = !context.GridActive;
			context.GridShow = context.GridActive;
		}

		[Command ("Preview")]
		void CommandPreview(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
			context.PreviewActive = !context.PreviewActive;
		}

		[Command ("Deselect")]
		void CommandDeselect(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.DeselectAll();
		}

		[Command ("SelectAll")]
		void CommandSelectAll(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.SelectAll();
		}

		[Command ("SelectInvert")]
		void CommandSelectInvert(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.InvertSelection();
		}

		[Command ("SelectGlobal")]
		void CommandSelectGlobal(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.Tool = "Select";
			Viewer viewer = this.CurrentDocument.Modifier.ActiveViewer;
			viewer.GlobalSelect = !viewer.GlobalSelect;
		}

		[Command ("SelectPartial")]
		void CommandSelectPartial(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.Tool = "Select";
			Viewer viewer = this.CurrentDocument.Modifier.ActiveViewer;
			viewer.PartialSelect = !viewer.PartialSelect;
		}

		[Command ("HideHalf")]
		void CommandHideHalf(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.Tool = "Select";
			DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
			context.HideHalfActive = !context.HideHalfActive;
		}

		[Command ("HideSel")]
		void CommandHideSel(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.HideSelection();
		}

		[Command ("HideRest")]
		void CommandHideRest(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.HideRest();
		}

		[Command ("HideCancel")]
		void CommandHideCancel(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.HideCancel();
		}

		[Command ("ZoomMin")]
		void CommandZoomMin(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.ZoomChange(0.0001);
		}

		[Command ("ZoomDefault")]
		void CommandZoomDefault(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
			this.CurrentDocument.Modifier.ZoomMemorize();
			context.ZoomAndCenter();
		}

		[Command ("ZoomSel")]
		void CommandZoomSel(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.ZoomSel();
		}

		[Command ("ZoomPrev")]
		void CommandZoomPrev(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.ZoomPrev();
		}

		[Command ("ZoomSub")]
		void CommandZoomSub(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.ZoomChange(0.5);
		}

		[Command ("ZoomAdd")]
		void CommandZoomAdd(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.ZoomChange(2.0);
		}

		// Exécute une commande locale à un objet.
		[Command ("Object")]
		void CommandObject(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			Widget widget = e.Source as Widget;
			this.CurrentDocument.Modifier.ActiveViewer.CommandObject(widget.Name);
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


		[Command ("Settings")]
		void CommandSettings(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CreateWindowSettings();
		}

		[Command ("Infos")]
		void CommandInfos(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CreateWindowInfos();
		}

		[Command ("AboutApplication")]
		void CommandAboutApplication(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CreateWindowAbout();
		}


		[Command ("PagePrev")]
		void CommandPagePrev(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
			if ( context.CurrentPage > 0 )
			{
				context.CurrentPage = context.CurrentPage-1;
			}
		}

		[Command ("PageNext")]
		void CommandPageNext(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
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
			DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
			int sel = System.Convert.ToInt32(e.CommandArgs[0]);
			context.CurrentPage = sel;
		}

		[Command ("PageCreate")]
		void CommandPageCreate(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
			int rank = context.CurrentPage;
			this.CurrentDocument.Modifier.PageCreate(rank+1, "");
		}

		[Command ("PageDelete")]
		void CommandPageDelete(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
			int rank = context.CurrentPage;
			this.CurrentDocument.Modifier.PageDelete(rank);
		}

		[Command ("PageUp")]
		void CommandPageUp(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
			int rank = context.CurrentPage;
			this.CurrentDocument.Modifier.PageSwap(rank, rank-1);
			context.CurrentPage = rank-1;
		}

		[Command ("PageDown")]
		void CommandPageDown(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
			int rank = context.CurrentPage;
			this.CurrentDocument.Modifier.PageSwap(rank, rank+1);
			context.CurrentPage = rank+1;
		}


		[Command ("LayerPrev")]
		void CommandLayerPrev(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
			if ( context.CurrentLayer > 0 )
			{
				context.CurrentLayer = context.CurrentLayer-1;
			}
		}

		[Command ("LayerNext")]
		void CommandLayerNext(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
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
			DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
			int sel = System.Convert.ToInt32(e.CommandArgs[0]);
			context.CurrentLayer = sel;
		}

		[Command ("LayerCreate")]
		void CommandLayerCreate(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
			int rank = context.CurrentLayer;
			this.CurrentDocument.Modifier.LayerCreate(rank+1, "");
		}

		[Command ("LayerDelete")]
		void CommandLayerDelete(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
			int rank = context.CurrentLayer;
			this.CurrentDocument.Modifier.LayerDelete(rank);
		}

		[Command ("LayerUp")]
		void CommandLayerUp(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
			int rank = context.CurrentLayer;
			this.CurrentDocument.Modifier.LayerSwap(rank, rank-1);
			context.CurrentLayer = rank-1;
		}

		[Command ("LayerDown")]
		void CommandLayerDown(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
			int rank = context.CurrentLayer;
			this.CurrentDocument.Modifier.LayerSwap(rank, rank+1);
			context.CurrentLayer = rank+1;
		}


		[Command ("DebugBboxThin")]
		void CommandDebugBboxThin(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
			context.IsDrawBoxThin = !context.IsDrawBoxThin;
		}

		[Command ("DebugBboxGeom")]
		void CommandDebugBboxGeom(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
			context.IsDrawBoxGeom = !context.IsDrawBoxGeom;
		}

		[Command ("DebugBboxFull")]
		void CommandDebugBboxFull(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
			context.IsDrawBoxFull = !context.IsDrawBoxFull;
		}

		[Command ("DebugDirty")]
		void CommandDebugDirty(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.ActiveViewer.DirtyAllViews();
		}


		// Quitte l'application.
		public void QuitApplication()
		{
			Window.Quit();
		}


		// Construit le menu pour choisir une page.
		public VMenu CreatePagesMenu()
		{
			DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
			UndoableList pages = this.CurrentDocument.GetObjects;  // liste des pages
			int total = pages.Count;
			VMenu menu = new VMenu();
			for ( int i=0 ; i<total ; i++ )
			{
				Objects.Page page = pages[i] as Objects.Page;

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
			DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
			Objects.Abstract page = context.RootObject(1);
			UndoableList layers = page.Objects;  // liste des calques
			int total = layers.Count;
			VMenu menu = new VMenu();
			for ( int i=0 ; i<total ; i++ )
			{
				int ii = total-i-1;
				Objects.Layer layer = layers[i] as Objects.Layer;

				string name = "";
				if ( layer.Name == "" )
				{
					name = string.Format("{0}: {1}", ((char)('A'+ii)).ToString(), Objects.Layer.LayerPositionName(ii, total));
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
			this.closeState = new CommandState("Close", this.commandDispatcher);
			this.printState = new CommandState("Print", this.commandDispatcher);
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
			this.debugDirtyState = new CommandState("DebugDirty", this.commandDispatcher);

			this.pagePrevState = new CommandState("PagePrev", this.commandDispatcher);
			this.pageNextState = new CommandState("PageNext", this.commandDispatcher);
			this.pageMenuState = new CommandState("PageMenu", this.commandDispatcher);
			this.pageCreateState = new CommandState("PageCreate", this.commandDispatcher);
			this.pageDeleteState = new CommandState("PageDelete", this.commandDispatcher);
			this.pageUpState = new CommandState("PageUp", this.commandDispatcher);
			this.pageDownState = new CommandState("PageDown", this.commandDispatcher);

			this.layerPrevState = new CommandState("LayerPrev", this.commandDispatcher);
			this.layerNextState = new CommandState("LayerNext", this.commandDispatcher);
			this.layerMenuState = new CommandState("LayerMenu", this.commandDispatcher);
			this.layerCreateState = new CommandState("LayerCreate", this.commandDispatcher);
			this.layerDeleteState = new CommandState("LayerDelete", this.commandDispatcher);
			this.layerUpState = new CommandState("LayerUp", this.commandDispatcher);
			this.layerDownState = new CommandState("LayerDown", this.commandDispatcher);

			this.settingsState = new CommandState("Settings", this.commandDispatcher);
			this.infosState = new CommandState("Infos", this.commandDispatcher);
			this.aboutState = new CommandState("AboutApplication", this.commandDispatcher);
		}


		// On s'enregistre auprès du document pour tous les événements.
		protected void ConnectEvents()
		{
			this.CurrentDocument.Notifier.DocumentChanged  += new SimpleEventHandler(this.HandleDocumentChanged);
			this.CurrentDocument.Notifier.MouseChanged     += new SimpleEventHandler(this.HandleMouseChanged);
			this.CurrentDocument.Notifier.OriginChanged    += new SimpleEventHandler(this.HandleOriginChanged);
			this.CurrentDocument.Notifier.ZoomChanged      += new SimpleEventHandler(this.HandleZoomChanged);
			this.CurrentDocument.Notifier.ToolChanged      += new SimpleEventHandler(this.HandleToolChanged);
			this.CurrentDocument.Notifier.SaveChanged      += new SimpleEventHandler(this.HandleSaveChanged);
			this.CurrentDocument.Notifier.SelectionChanged += new SimpleEventHandler(this.HandleSelectionChanged);
			this.CurrentDocument.Notifier.CreateChanged    += new SimpleEventHandler(this.HandleCreateChanged);
			this.CurrentDocument.Notifier.StyleChanged     += new SimpleEventHandler(this.HandleStyleChanged);
			this.CurrentDocument.Notifier.PagesChanged     += new SimpleEventHandler(this.HandlePagesChanged);
			this.CurrentDocument.Notifier.LayersChanged    += new SimpleEventHandler(this.HandleLayersChanged);
			this.CurrentDocument.Notifier.PageChanged      += new ObjectEventHandler(this.HandlePageChanged);
			this.CurrentDocument.Notifier.LayerChanged     += new ObjectEventHandler(this.HandleLayerChanged);
			this.CurrentDocument.Notifier.UndoRedoChanged  += new SimpleEventHandler(this.HandleUndoRedoChanged);
			this.CurrentDocument.Notifier.GridChanged      += new SimpleEventHandler(this.HandleGridChanged);
			this.CurrentDocument.Notifier.PreviewChanged   += new SimpleEventHandler(this.HandlePreviewChanged);
			this.CurrentDocument.Notifier.SettingsChanged  += new SimpleEventHandler(this.HandleSettingsChanged);
			this.CurrentDocument.Notifier.GuidesChanged    += new SimpleEventHandler(this.HandleGuidesChanged);
			this.CurrentDocument.Notifier.HideHalfChanged  += new SimpleEventHandler(this.HandleHideHalfChanged);
			this.CurrentDocument.Notifier.DebugChanged     += new SimpleEventHandler(this.HandleDebugChanged);
			this.CurrentDocument.Notifier.PropertyChanged  += new PropertyEventHandler(this.HandlePropertyChanged);
			this.CurrentDocument.Notifier.DrawChanged      += new RedrawEventHandler(this.HandleDrawChanged);
		}

		// Appelé par le document lorsque les informations sur le document ont changé.
		private void HandleDocumentChanged()
		{
			StatusField field = this.info.Items["StatusDocument"] as StatusField;
			field.Text = this.TextDocument;
			field.Invalidate();

			if ( this.IsCurrentDocument )
			{
				this.printState.Enabled = true;
				this.selectPartialState.Enabled = true;
				this.settingsState.Enabled = true;
				this.infosState.Enabled = true;

				this.CurrentDocument.Dialogs.UpdateInfos();
				this.UpdateBookDocuments();
			}
			else
			{
				this.printState.Enabled = false;
				this.selectPartialState.Enabled = false;
				this.settingsState.Enabled = false;
				this.infosState.Enabled = false;
			}
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

			if ( this.IsCurrentDocument )
			{
				DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
				this.zoomDefaultState.Enabled = !context.IsZoomDefault;
			}
			else
			{
				this.zoomDefaultState.Enabled = false;
			}
		}

		// Appelé par le document lorsque le zoom a changé.
		private void HandleZoomChanged()
		{
			this.UpdateScroller();

			if ( this.IsCurrentDocument )
			{
				DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
				this.zoomMinState.Enabled = ( context.Zoom > this.CurrentDocument.Modifier.ZoomMin );
				this.zoomDefaultState.Enabled = !context.IsZoomDefault;
				this.zoomPrevState.Enabled = ( this.CurrentDocument.Modifier.ZoomMemorizeCount > 0 );
				this.zoomSubState.Enabled = ( context.Zoom > this.CurrentDocument.Modifier.ZoomMin );
				this.zoomAddState.Enabled = ( context.Zoom < this.CurrentDocument.Modifier.ZoomMax );
			}
			else
			{
				this.zoomMinState.Enabled = false;
				this.zoomDefaultState.Enabled = false;
				this.zoomPrevState.Enabled = false;
				this.zoomSubState.Enabled = false;
				this.zoomAddState.Enabled = false;
			}

			StatusField field = this.info.Items["StatusZoom"] as StatusField;
			field.Text = this.TextInfoZoom;
			field.Invalidate();
		}

		// Appelé par le document lorsque l'outil a changé.
		private void HandleToolChanged()
		{
			if ( this.IsCurrentDocument )
			{
				bool isCreating = this.CurrentDocument.Modifier.ActiveViewer.IsCreating;
				string tool = this.CurrentDocument.Modifier.Tool;
				Widget[] toolWidgets = Widget.FindAllCommandWidgets("SelectTool", this.commandDispatcher);
				foreach ( Widget widget in toolWidgets )
				{
					widget.ActiveState = ( widget.Name == tool ) ? WidgetState.ActiveYes : WidgetState.ActiveNo;
					widget.SetEnabled(widget.Name == tool || widget.Name == "Select" || !isCreating);
				}
			}
			else
			{
				Widget[] toolWidgets = Widget.FindAllCommandWidgets("SelectTool", this.commandDispatcher);
				foreach ( Widget widget in toolWidgets )
				{
					widget.ActiveState = WidgetState.ActiveNo;
					widget.SetEnabled(false);
				}
			}
		}

		// Appelé par le document lorsque l'état "enregistrer" a changé.
		private void HandleSaveChanged()
		{
			if ( this.IsCurrentDocument )
			{
				this.saveState.Enabled = this.CurrentDocument.IsDirtySerialize;
				this.saveAsState.Enabled = true;
				this.UpdateBookDocuments();
			}
			else
			{
				this.saveState.Enabled = false;
				this.saveAsState.Enabled = false;
			}
		}

		// Appelé par le document lorsque la sélection a changé.
		private void HandleSelectionChanged()
		{
			if ( this.IsCurrentDocument )
			{
				DocumentInfo di = this.CurrentDocumentInfo;
				di.containerPrincipal.SetDirtyContent();
#if DEBUG
				di.containerAutos.SetDirtyContent();
#endif
				di.containerOperations.SetDirtyContent();

				Viewer viewer = this.CurrentDocument.Modifier.ActiveViewer;
				int totalSelected  = this.CurrentDocument.Modifier.TotalSelected;
				int totalHide      = this.CurrentDocument.Modifier.TotalHide;
				int totalPageHide  = this.CurrentDocument.Modifier.TotalPageHide;
				int totalObjects   = this.CurrentDocument.Modifier.TotalObjects;
				bool isCreating    = this.CurrentDocument.Modifier.ActiveViewer.IsCreating;
				bool isBase        = viewer.DrawingContext.RootStackIsBase;
				Objects.Abstract one = this.CurrentDocument.Modifier.RetOnlySelectedObject();

				this.newState.Enabled = true;
				this.openState.Enabled = true;
				this.deleteState.Enabled = ( totalSelected > 0 || isCreating );
				this.duplicateState.Enabled = ( totalSelected > 0 && !isCreating );
				this.cutState.Enabled = ( totalSelected > 0 && !isCreating );
				this.copyState.Enabled = ( totalSelected > 0 && !isCreating );
				this.pasteState.Enabled = ( !this.CurrentDocument.Modifier.IsClipboardEmpty() && !isCreating );
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
				this.ungroupState.Enabled = ( totalSelected == 1 && one is Objects.Group && !isCreating );
				this.insideState.Enabled = ( totalSelected == 1 && one is Objects.Group && !isCreating );
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

				this.CurrentDocument.Dialogs.UpdateInfos();
			}
			else
			{
				this.newState.Enabled = true;
				this.openState.Enabled = true;
				this.deleteState.Enabled = false;
				this.duplicateState.Enabled = false;
				this.cutState.Enabled = false;
				this.copyState.Enabled = false;
				this.pasteState.Enabled = false;
				this.orderUpState.Enabled = false;
				this.orderDownState.Enabled = false;
				this.rotate90State.Enabled = false;
				this.rotate180State.Enabled = false;
				this.rotate270State.Enabled = false;
				this.mirrorHState.Enabled = false;
				this.mirrorVState.Enabled = false;
				this.zoomMul2State.Enabled = false;
				this.zoomDiv2State.Enabled = false;
				this.mergeState.Enabled = false;
				this.groupState.Enabled = false;
				this.ungroupState.Enabled = false;
				this.insideState.Enabled = false;
				this.outsideState.Enabled = false;

				this.hideSelState.Enabled = false;
				this.hideRestState.Enabled = false;
				this.hideCancelState.Enabled = false;

				this.zoomSelState.Enabled = false;

				this.deselectState.Enabled = false;
				this.selectAllState.Enabled = false;
				this.selectInvertState.Enabled = false;

				this.selectGlobalState.Enabled = false;
				this.selectGlobalState.ActiveState = WidgetState.ActiveNo;
			
				this.selectPartialState.ActiveState = WidgetState.ActiveNo;
			}

			StatusField field = this.info.Items["StatusObject"] as StatusField;
			field.Text = this.TextInfoObject;
			field.Invalidate();
		}

		// Appelé lorsque la création d'un objet à débuté ou s'est terminée.
		private void HandleCreateChanged()
		{
			if ( !this.IsCurrentDocument )  return;
			this.HandleSelectionChanged();
			this.HandlePagesChanged();
			this.HandleLayersChanged();
			this.HandleUndoRedoChanged();
			this.HandleToolChanged();
			this.CurrentDocument.Dialogs.UpdateInfos();
		}

		// Appelé par le document lorsqu'un style a changé.
		private void HandleStyleChanged()
		{
			if ( !this.IsCurrentDocument )  return;
			DocumentInfo di = this.CurrentDocumentInfo;
			di.containerStyles.SetDirtyContent();
		}

		// Appelé par le document lorsque les pages ont changé.
		private void HandlePagesChanged()
		{
			if ( this.IsCurrentDocument )
			{
				DocumentInfo di = this.CurrentDocumentInfo;
				di.containerPages.SetDirtyContent();

				DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
				int cp = context.CurrentPage;
				int tp = context.TotalPages();

				bool isCreating = this.CurrentDocument.Modifier.ActiveViewer.IsCreating;

				this.pagePrevState.Enabled = (cp > 0 && !isCreating );
				this.pageNextState.Enabled = (cp < tp-1 && !isCreating );
				this.pageMenuState.Enabled = (tp > 1 && !isCreating );
				this.pageCreateState.Enabled = !isCreating;
				this.pageDeleteState.Enabled = (tp > 1 && !isCreating );
				this.pageUpState.Enabled = (cp > 0 && !isCreating );
				this.pageDownState.Enabled = (cp < tp-1 && !isCreating );

				this.CurrentDocumentInfo.quickPageMenu.Text = (cp+1).ToString();
			}
			else
			{
				this.pagePrevState.Enabled = false;
				this.pageNextState.Enabled = false;
				this.pageMenuState.Enabled = false;
				this.pageCreateState.Enabled = false;
				this.pageDeleteState.Enabled = false;
				this.pageUpState.Enabled = false;
				this.pageDownState.Enabled = false;
			}
		}

		// Appelé par le document lorsque les calques ont changé.
		private void HandleLayersChanged()
		{
			if ( this.IsCurrentDocument )
			{
				DocumentInfo di = this.CurrentDocumentInfo;
				di.containerLayers.SetDirtyContent();

				DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
				int cl = context.CurrentLayer;
				int tl = context.TotalLayers();

				bool isCreating = this.CurrentDocument.Modifier.ActiveViewer.IsCreating;

				this.layerPrevState.Enabled = (cl > 0 && !isCreating );
				this.layerNextState.Enabled = (cl < tl-1 && !isCreating );
				this.layerMenuState.Enabled = (tl > 1 && !isCreating );
				this.layerCreateState.Enabled = !isCreating ;
				this.layerDeleteState.Enabled = (tl > 1 && !isCreating );
				this.layerUpState.Enabled = (cl > 0 && !isCreating );
				this.layerDownState.Enabled = (cl < tl-1 && !isCreating );

				this.CurrentDocumentInfo.quickLayerMenu.Text = ((char)('A'+cl)).ToString();
			}
			else
			{
				this.layerPrevState.Enabled = false;
				this.layerNextState.Enabled = false;
				this.layerMenuState.Enabled = false;
				this.layerCreateState.Enabled = false;
				this.layerDeleteState.Enabled = false;
				this.layerUpState.Enabled = false;
				this.layerDownState.Enabled = false;
			}
		}

		// Appelé par le document lorsqu'un nom de page a changé.
		private void HandlePageChanged(Objects.Abstract page)
		{
			if ( !this.IsCurrentDocument )  return;
			DocumentInfo di = this.CurrentDocumentInfo;
			di.containerPages.SetDirtyObject(page);
		}

		// Appelé par le document lorsqu'un nom de calque a changé.
		private void HandleLayerChanged(Objects.Abstract layer)
		{
			if ( !this.IsCurrentDocument )  return;
			DocumentInfo di = this.CurrentDocumentInfo;
			di.containerLayers.SetDirtyObject(layer);
		}

		// Appelé par le document lorsque l'état des commande undo/redo a changé.
		private void HandleUndoRedoChanged()
		{
			if ( this.IsCurrentDocument )
			{
				bool isCreating = this.CurrentDocument.Modifier.ActiveViewer.IsCreating;
				this.undoState.Enabled = ( this.CurrentDocument.Modifier.OpletQueue.CanUndo && !isCreating );
				this.redoState.Enabled = ( this.CurrentDocument.Modifier.OpletQueue.CanRedo && !isCreating );
			}
			else
			{
				this.undoState.Enabled = false;
				this.redoState.Enabled = false;
			}
		}

		// Appelé par le document lorsque l'état de la grille a changé.
		private void HandleGridChanged()
		{
			if ( this.IsCurrentDocument )
			{
				DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
				this.gridState.Enabled = true;
				this.gridState.ActiveState = context.GridActive ? WidgetState.ActiveYes : WidgetState.ActiveNo;
			}
			else
			{
				this.gridState.Enabled = false;
				this.gridState.ActiveState = WidgetState.ActiveNo;
			}
		}

		// Appelé par le document lorsque l'état de l'aperçu a changé.
		private void HandlePreviewChanged()
		{
			if ( this.IsCurrentDocument )
			{
				DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
				this.previewState.Enabled = true;
				this.previewState.ActiveState = context.PreviewActive ? WidgetState.ActiveYes : WidgetState.ActiveNo;
			}
			else
			{
				this.previewState.Enabled = false;
				this.previewState.ActiveState = WidgetState.ActiveNo;
			}
		}

		// Appelé par le document lorsque les réglages ont changé.
		private void HandleSettingsChanged()
		{
			if ( this.IsCurrentDocument )
			{
				this.CurrentDocument.Dialogs.UpdateSettings();
			}
		}

		// Appelé par le document lorsque les repères ont changé.
		private void HandleGuidesChanged()
		{
			if ( this.IsCurrentDocument )
			{
				this.CurrentDocument.Dialogs.UpdateGuides();
			}
		}

		// Appelé par le document lorsque l'état de la commande "hide half" a changé.
		private void HandleHideHalfChanged()
		{
			if ( this.IsCurrentDocument )
			{
				DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
				this.hideHalfState.Enabled = true;
				this.hideHalfState.ActiveState = context.HideHalfActive ? WidgetState.ActiveYes : WidgetState.ActiveNo;
			}
			else
			{
				this.hideHalfState.Enabled = false;
				this.hideHalfState.ActiveState = WidgetState.ActiveNo;
			}
		}

		// Appelé par le document lorsque l'état des commande de debug a changé.
		private void HandleDebugChanged()
		{
			if ( this.IsCurrentDocument )
			{
				DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
				this.debugBboxThinState.Enabled = true;
				this.debugBboxGeomState.Enabled = true;
				this.debugBboxFullState.Enabled = true;
				this.debugBboxThinState.ActiveState = context.IsDrawBoxThin ? WidgetState.ActiveYes : WidgetState.ActiveNo;
				this.debugBboxGeomState.ActiveState = context.IsDrawBoxGeom ? WidgetState.ActiveYes : WidgetState.ActiveNo;
				this.debugBboxFullState.ActiveState = context.IsDrawBoxFull ? WidgetState.ActiveYes : WidgetState.ActiveNo;
				this.debugDirtyState.Enabled = true;
			}
			else
			{
				this.debugBboxThinState.Enabled = false;
				this.debugBboxGeomState.Enabled = false;
				this.debugBboxFullState.Enabled = false;
				this.debugBboxThinState.ActiveState = WidgetState.ActiveNo;
				this.debugBboxGeomState.ActiveState = WidgetState.ActiveNo;
				this.debugBboxFullState.ActiveState = WidgetState.ActiveNo;
				this.debugDirtyState.Enabled = false;
			}
		}

		// Appelé lorsqu'une propriété a changé.
		private void HandlePropertyChanged(System.Collections.ArrayList propertyList)
		{
			if ( this.IsCurrentDocument )
			{
				DocumentInfo di = this.CurrentDocumentInfo;
				di.containerPrincipal.SetDirtyProperties(propertyList);
				di.containerStyles.SetDirtyProperties(propertyList);
			}
		}

		// Appelé par le document lorsque le dessin a changé.
		private void HandleDrawChanged(Viewer viewer, Drawing.Rectangle rect)
		{
			Drawing.Rectangle box = rect;

			if ( viewer.DrawingContext.IsActive )
			{
				box.Inflate(viewer.DrawingContext.HandleSize/2);
			}

			box = viewer.InternalToScreen(box);
			this.InvalidateDraw(viewer, box);
		}


		// Invalide une partie de la zone de dessin d'un visualisateur.
		protected void InvalidateDraw(Viewer viewer, Drawing.Rectangle bbox)
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
			if ( !this.IsCurrentDocument )  return;

			DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
			Size area = this.CurrentDocument.Modifier.SizeArea;
			Point scale = context.Scale;
			double ratioH = context.ContainerSize.Width/scale.X/area.Width;
			double ratioV = context.ContainerSize.Height/scale.Y/area.Height;
			ratioH = System.Math.Min(ratioH, 1.0);
			ratioV = System.Math.Min(ratioV, 1.0);

			DocumentInfo di = this.CurrentDocumentInfo;
			double min, max;

			min = context.MinOriginX;
			max = context.MaxOriginX;
			max = System.Math.Max(min, max);
			di.hScroller.MinValue = (decimal) min;
			di.hScroller.MaxValue = (decimal) max;
			di.hScroller.VisibleRangeRatio = (decimal) ratioH;
			di.hScroller.Value = (decimal) (-context.OriginX);
			di.hScroller.SmallChange = (decimal) (10.0/scale.X);
			di.hScroller.LargeChange = (decimal) (100.0/scale.X);

			min = context.MinOriginY;
			max = context.MaxOriginY;
			max = System.Math.Max(min, max);
			di.vScroller.MinValue = (decimal) min;
			di.vScroller.MaxValue = (decimal) max;
			di.vScroller.VisibleRangeRatio = (decimal) ratioV;
			di.vScroller.Value = (decimal) (-context.OriginY);
			di.vScroller.SmallChange = (decimal) (10.0/scale.Y);
			di.vScroller.LargeChange = (decimal) (100.0/scale.Y);
		}


		// Appelé lorsque le look a changé.
		protected void UpdateLookCommand()
		{
			string lookName = Widgets.Adorner.Factory.ActiveName;
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
				Document doc = this.CurrentDocument;
				if ( doc == null )
				{
					return "";
				}
				else
				{
					Size size = doc.Size / doc.Modifier.RealScale;
					return string.Format("Format {0}x{1}", size.Width, size.Height);
				}
			}
		}

		// Texte pour les informations.
		protected string TextInfoObject
		{
			get
			{
				Document doc = this.CurrentDocument;
				if ( doc == null )
				{
					return "";
				}
				else
				{
					int sel   = doc.Modifier.TotalSelected;
					int hide  = doc.Modifier.TotalHide;
					int total = doc.Modifier.TotalObjects;
					int deep  = doc.Modifier.ActiveViewer.DrawingContext.RootStackDeep;

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
		}

		// Texte pour les informations.
		protected string TextInfoMouse
		{
			get
			{
				Document doc = this.CurrentDocument;
				if ( doc == null )
				{
					return "";
				}
				else
				{
					Point mouse = doc.Modifier.ActiveViewer.MousePos();
					mouse /= doc.Modifier.RealScale;
					return string.Format("(x:{0} y:{1})", mouse.X.ToString("F1"), mouse.Y.ToString("F1"));
				}
			}
		}

		// Texte pour les informations.
		protected string TextInfoZoom
		{
			get
			{
				Document doc = this.CurrentDocument;
				if ( doc == null )
				{
					return "";
				}
				else
				{
					DrawingContext context = doc.Modifier.ActiveViewer.DrawingContext;
					double zoom = context.Zoom;
					return string.Format("Zoom {0}%", (zoom*100).ToString("F0"));
				}
			}
		}


		#region Dialogs
		// Crée la fenêtre pour les réglages.
		protected void CreateWindowSettings()
		{
			if ( this.windowSettings == null )
			{
				this.windowSettings = new Window();
				this.windowSettings.ClientSize = new Size(300, 350);
				this.windowSettings.Text = "Réglages";
				this.windowSettings.MakeSecondaryWindow();
				this.windowSettings.MakeFixedSizeWindow();
				this.windowSettings.MakeToolWindow();
				this.windowSettings.PreventAutoClose = true;
				this.windowSettings.Owner = this.Window;
				this.windowSettings.WindowCloseClicked += new EventHandler(this.HandleWindowSettingsCloseClicked);

				// Crée les onglets.
				TabBook book = new TabBook(this.windowSettings.Root);
				book.Arrows = TabBookArrows.Stretch;
				book.Anchor = AnchorStyles.LeftAndRight | AnchorStyles.TopAndBottom;
				book.AnchorMargins = new Margins(6, 6, 6, 34);

				TabPage bookFormat = new TabPage();
				bookFormat.Name = "Format";
				bookFormat.TabTitle = "Format";
				book.Items.Add(bookFormat);

				TabPage bookGrid = new TabPage();
				bookGrid.Name = "Grid";
				bookGrid.TabTitle = "Grille";
				book.Items.Add(bookGrid);

				TabPage bookGuides = new TabPage();
				bookGuides.Name = "Guides";
				bookGuides.TabTitle = "Repères";
				book.Items.Add(bookGuides);

				TabPage bookPrint = new TabPage();
				bookPrint.Name = "Print";
				bookPrint.TabTitle = "Impression";
				book.Items.Add(bookPrint);

				TabPage bookMisc = new TabPage();
				bookMisc.Name = "Misc";
				bookMisc.TabTitle = "Divers";
				book.Items.Add(bookMisc);

				book.ActivePage = bookFormat;

				// Bouton de fermeture.
				Button buttonClose = new Button(this.windowSettings.Root);
				buttonClose.Width = 75;
				buttonClose.Text = "Fermer";
				buttonClose.ButtonStyle = ButtonStyle.DefaultAccept;
				buttonClose.Anchor = AnchorStyles.BottomLeft;
				buttonClose.AnchorMargins = new Margins(6, 0, 0, 6);
				buttonClose.Clicked += new MessageEventHandler(this.HandleSettingsButtonCloseClicked);
				buttonClose.TabIndex = 1000;
				buttonClose.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				ToolTip.Default.SetToolTip(buttonClose, "Fermer les réglages");
			}

			this.CurrentDocument.Dialogs.BuildSettings(this.windowSettings);
			this.windowSettings.Show();
		}

		private void HandleWindowSettingsCloseClicked(object sender)
		{
			this.windowSettings.Hide();
		}

		private void HandleSettingsButtonCloseClicked(object sender, MessageEventArgs e)
		{
			this.windowSettings.Hide();
		}

		protected void RebuildWindowSettings()
		{
			if ( !this.IsCurrentDocument )  return;
			if ( this.windowSettings == null )  return;
			this.CurrentDocument.Dialogs.BuildSettings(this.windowSettings);
		}

		// Crée la fenêtre pour les informations.
		protected void CreateWindowInfos()
		{
			if ( this.windowInfos == null )
			{
				this.windowInfos = new Window();
				this.windowInfos.ClientSize = new Size(300, 250);
				this.windowInfos.Text = "Informations";
				//?this.windowInfos.MakeFixedSizeWindow();
				this.windowInfos.MakeSecondaryWindow();
				this.windowInfos.PreventAutoClose = true;
				this.windowInfos.Owner = this.Window;
				this.windowInfos.WindowCloseClicked += new EventHandler(this.HandleWindowInfosCloseClicked);
				this.windowInfos.Root.MinSize = new Size(200, 100);

				TextFieldMulti multi = new TextFieldMulti(this.windowInfos.Root);
				multi.Name = "Infos";
				multi.IsReadOnly = true;
				multi.Dock = DockStyle.Fill;
				multi.DockMargins = new Margins(10, 10, 10, 40);

				// Bouton de fermeture.
				Button buttonClose = new Button(this.windowInfos.Root);
				buttonClose.Width = 75;
				buttonClose.Text = "Fermer";
				buttonClose.ButtonStyle = ButtonStyle.DefaultAccept;
				buttonClose.Anchor = AnchorStyles.BottomLeft;
				buttonClose.AnchorMargins = new Margins(10, 0, 0, 10);
				buttonClose.Clicked += new MessageEventHandler(this.HandleInfosButtonCloseClicked);
				buttonClose.TabIndex = 1000;
				buttonClose.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				ToolTip.Default.SetToolTip(buttonClose, "Fermer ce dialogue");
			}

			this.windowInfos.Show();
			this.CurrentDocument.Dialogs.BuildInfos(this.windowInfos);
		}

		private void HandleWindowInfosCloseClicked(object sender)
		{
			this.windowInfos.Hide();
		}

		private void HandleInfosButtonCloseClicked(object sender, MessageEventArgs e)
		{
			this.windowInfos.Hide();
		}

		protected void RebuildWindowInfos()
		{
			if ( !this.IsCurrentDocument )  return;
			if ( this.windowInfos == null )  return;
			this.CurrentDocument.Dialogs.BuildInfos(this.windowInfos);
		}

		// Crée la fenêtre "à propos de".
		protected void CreateWindowAbout()
		{
			if ( this.windowAbout == null )
			{
				this.windowAbout = new Window();
				this.windowAbout.ClientSize = new Size(300, 150);
				this.windowAbout.Text = "A propos de...";
				this.windowAbout.MakeFixedSizeWindow();
				this.windowAbout.MakeSecondaryWindow();
				this.windowAbout.PreventAutoClose = true;
				this.windowAbout.Owner = this.Window;
				this.windowAbout.WindowCloseClicked += new EventHandler(this.HandleWindowAboutCloseClicked);

				if ( this.type == DocumentType.Pictogram )
				{
					Common.Document.Dialogs.CreateTitle(this.windowAbout.Root, "Crésus pictogramme");
				}
				else
				{
					Common.Document.Dialogs.CreateTitle(this.windowAbout.Root, "Crésus document");
				}

				string version = typeof(Document).Assembly.FullName.Split(',')[1].Split('=')[1];
				Common.Document.Dialogs.CreateLabel(this.windowAbout.Root, "Version", version);
				Common.Document.Dialogs.CreateLabel(this.windowAbout.Root, "Identificateur", "58421-75001-63244-80751");
				Common.Document.Dialogs.CreateLabel(this.windowAbout.Root, "Créé par", "EPSITEC SA");
				Common.Document.Dialogs.CreateLabel(this.windowAbout.Root, "Site web", @"<a href=""http://www.epsitec.ch"">www.epsitec.ch</a>");
				Common.Document.Dialogs.CreateSeparator(this.windowAbout.Root);

				// Bouton de fermeture.
				Button buttonClose = new Button(this.windowAbout.Root);
				buttonClose.Width = 75;
				buttonClose.Text = "Fermer";
				buttonClose.ButtonStyle = ButtonStyle.DefaultAccept;
				buttonClose.Anchor = AnchorStyles.BottomLeft;
				buttonClose.AnchorMargins = new Margins(10, 0, 0, 10);
				buttonClose.Clicked += new MessageEventHandler(this.HandleAboutButtonCloseClicked);
				buttonClose.TabIndex = 1000;
				buttonClose.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				ToolTip.Default.SetToolTip(buttonClose, "Fermer ce dialogue");
			}

			this.windowAbout.Show();
		}

		private void HandleWindowAboutCloseClicked(object sender)
		{
			this.windowAbout.Hide();
		}

		private void HandleAboutButtonCloseClicked(object sender, MessageEventArgs e)
		{
			this.windowAbout.Hide();
		}
		#endregion


		#region TabBook
		// L'onglet pour le document courant a été cliqué.
		private void HandleBookDocumentsActivePageChanged(object sender)
		{
			if ( this.ignoreChange )  return;

			int total = this.bookDocuments.PageCount;
			for ( int i=0 ; i<total ; i++ )
			{
				DocumentInfo di = this.documents[i] as DocumentInfo;
				if ( di.tabPage == this.bookDocuments.ActivePage )
				{
					this.UseDocument(i);
					return;
				}
			}
		}

		// Indique s'il existe un document courant.
		protected bool IsCurrentDocument
		{
			get
			{
				return ( this.currentDocument >= 0 );
			}
		}

		// Retourne le DocumentInfo courant.
		protected DocumentInfo CurrentDocumentInfo
		{
			get
			{
				if ( this.currentDocument < 0 )  return null;
				return this.documents[this.currentDocument] as DocumentInfo;
			}
		}

		// Retourne le Document courant.
		public Document CurrentDocument
		{
			get
			{
				if ( this.currentDocument < 0 )  return null;
				return this.CurrentDocumentInfo.document;
			}
		}

		// Crée un nouveau document.
		public void CreateDocument()
		{
			this.PrepareCloseDocument();

			Document doc = new Document(this.type, DocumentMode.Modify);
			doc.Name = "Document";
			doc.Clipboard = this.clipboard;

			DocumentInfo di = new DocumentInfo();
			di.document = doc;
			this.documents.Insert(++this.currentDocument, di);

			this.CreateDocumentLayout(this.CurrentDocument);

			this.ConnectEvents();
			this.CurrentDocument.Modifier.New();
			this.UpdateCloseCommand();
			this.PrepareOpenDocument();
		}

		// Utilise un document ouvert.
		protected void UseDocument(int rank)
		{
			if ( this.ignoreChange )  return;

			this.PrepareCloseDocument();
			this.currentDocument = rank;
			this.PrepareOpenDocument();

			if ( rank >= 0 )
			{
				this.ignoreChange = true;
				this.bookDocuments.ActivePage = this.CurrentDocumentInfo.tabPage;
				this.ignoreChange = false;

				int total = this.bookDocuments.PageCount;
				for ( int i=0 ; i<total ; i++ )
				{
					DocumentInfo di = this.documents[i] as DocumentInfo;
					di.bookPanels.SetVisible(i == this.currentDocument);
				}

				this.CommandStateShake(this.pageNextState);
				this.CommandStateShake(this.pagePrevState);
				this.CommandStateShake(this.pageMenuState);
				this.CommandStateShake(this.layerNextState);
				this.CommandStateShake(this.layerPrevState);
				this.CommandStateShake(this.layerMenuState);

				this.CurrentDocument.Notifier.NotifyAllChanged();
			}
			else
			{
				this.HandleDocumentChanged();
				this.HandleMouseChanged();
				this.HandleOriginChanged();
				this.HandleZoomChanged();
				this.HandleToolChanged();
				this.HandleSaveChanged();
				this.HandleSelectionChanged();
				this.HandleCreateChanged();
				this.HandleStyleChanged();
				this.HandlePagesChanged();
				this.HandleLayersChanged();
				this.HandleUndoRedoChanged();
				this.HandleGridChanged();
				this.HandlePreviewChanged();
				this.HandleSettingsChanged();
				this.HandleGuidesChanged();
				this.HandleHideHalfChanged();
				this.HandleDebugChanged();
			}
		}

		// Ferme le document courant.
		protected void CloseDocument()
		{
			this.PrepareCloseDocument();
			int rank = this.currentDocument;
			if ( rank < 0 )  return;

			DocumentInfo di = this.CurrentDocumentInfo;
			this.documents.RemoveAt(rank);
			this.ignoreChange = true;
			this.bookDocuments.Items.RemoveAt(rank);
			this.ignoreChange = false;
			di.Dispose();

			if ( rank >= this.bookDocuments.PageCount )
			{
				rank = this.bookDocuments.PageCount-1;
			}
			this.currentDocument = -1;
			this.UseDocument(rank);
			this.UpdateCloseCommand();
		}

		// Met à jour l'état de la commande de fermeture.
		protected void UpdateCloseCommand()
		{
			this.closeState.Enabled = (this.bookDocuments.PageCount > 0);
		}

		// Met à jour le nom de l'onglet des documents.
		protected void UpdateBookDocuments()
		{
			if ( !this.IsCurrentDocument )  return;
			TabPage tab = this.bookDocuments.Items[this.currentDocument] as TabPage;
			tab.TabTitle = Misc.ExtractName(this.CurrentDocument.Filename, this.CurrentDocument.IsDirtySerialize);
		}

		// Préparation avant la fermeture d'un document.
		protected void PrepareCloseDocument()
		{
			if ( !this.IsCurrentDocument )  return;
			this.CurrentDocument.Dialogs.FlushAll();
		}

		// Préparation après l'ouverture d'un document.
		protected void PrepareOpenDocument()
		{
			this.RebuildWindowSettings();
			this.RebuildWindowInfos();
		}

		// Secoue un CommandState pour le forcer à se remettre à jour.
		protected void CommandStateShake(CommandState state)
		{
			state.Enabled = !state.Enabled;
			state.Enabled = !state.Enabled;
		}
		#endregion


		protected DocumentType					type;
		protected bool							useArray;
		protected Document						clipboard;
		protected int							currentDocument;
		protected System.Collections.ArrayList	documents;

		protected CommandDispatcher				commandDispatcher;

		protected HMenu							menu;
		protected HToolBar						hToolBar;
		protected VToolBar						vToolBar;
		protected StatusBar						info;
		protected TabBook						bookDocuments;
		protected double						panelsWidth = 247;
		protected string						printerName = "";
		protected bool							ignoreChange;

		protected Window						windowAbout;
		protected Window						windowInfos;
		protected Window						windowSettings;

		protected CommandState					newState;
		protected CommandState					openState;
		protected CommandState					saveState;
		protected CommandState					saveAsState;
		protected CommandState					closeState;
		protected CommandState					printState;
		protected CommandState					deleteState;
		protected CommandState					duplicateState;
		protected CommandState					cutState;
		protected CommandState					copyState;
		protected CommandState					pasteState;
		protected CommandState					orderUpState;
		protected CommandState					orderDownState;
		protected CommandState					rotate90State;
		protected CommandState					rotate180State;
		protected CommandState					rotate270State;
		protected CommandState					mirrorHState;
		protected CommandState					mirrorVState;
		protected CommandState					zoomMul2State;
		protected CommandState					zoomDiv2State;
		protected CommandState					mergeState;
		protected CommandState					groupState;
		protected CommandState					ungroupState;
		protected CommandState					insideState;
		protected CommandState					outsideState;
		protected CommandState					undoState;
		protected CommandState					redoState;
		protected CommandState					deselectState;
		protected CommandState					selectAllState;
		protected CommandState					selectInvertState;
		protected CommandState					selectGlobalState;
		protected CommandState					selectPartialState;
		protected CommandState					hideHalfState;
		protected CommandState					hideSelState;
		protected CommandState					hideRestState;
		protected CommandState					hideCancelState;
		protected CommandState					zoomMinState;
		protected CommandState					zoomDefaultState;
		protected CommandState					zoomSelState;
		protected CommandState					zoomPrevState;
		protected CommandState					zoomSubState;
		protected CommandState					zoomAddState;
		protected CommandState					previewState;
		protected CommandState					gridState;
		protected CommandState					arrayOutlineFrameState;
		protected CommandState					arrayOutlineHorizState;
		protected CommandState					arrayOutlineVertiState;
		protected CommandState					arrayAddColumnLeftState;
		protected CommandState					arrayAddColumnRightState;
		protected CommandState					arrayAddRowTopState;
		protected CommandState					arrayAddRowBottomState;
		protected CommandState					arrayDelColumnState;
		protected CommandState					arrayDelRowState;
		protected CommandState					arrayAlignColumnState;
		protected CommandState					arrayAlignRowState;
		protected CommandState					arraySwapColumnState;
		protected CommandState					arraySwapRowState;
		protected CommandState					arrayLookState;
		protected CommandState					debugBboxThinState;
		protected CommandState					debugBboxGeomState;
		protected CommandState					debugBboxFullState;
		protected CommandState					debugDirtyState;
		protected CommandState					pagePrevState;
		protected CommandState					pageNextState;
		protected CommandState					pageMenuState;
		protected CommandState					pageCreateState;
		protected CommandState					pageDeleteState;
		protected CommandState					pageUpState;
		protected CommandState					pageDownState;
		protected CommandState					layerPrevState;
		protected CommandState					layerNextState;
		protected CommandState					layerMenuState;
		protected CommandState					layerCreateState;
		protected CommandState					layerDeleteState;
		protected CommandState					layerUpState;
		protected CommandState					layerDownState;
		protected CommandState					settingsState;
		protected CommandState					infosState;
		protected CommandState					aboutState;


		protected class DocumentInfo
		{
			public Document						document;
			public TabPage						tabPage;
			public HScroller					hScroller;
			public VScroller					vScroller;
			public Button						quickPageMenu;
			public Button						quickLayerMenu;
			public TabBook						bookPanels;
			public Containers.Principal			containerPrincipal;
			public Containers.Styles			containerStyles;
			public Containers.Autos				containerAutos;
			public Containers.Pages				containerPages;
			public Containers.Layers			containerLayers;
			public Containers.Operations		containerOperations;

			public void Dispose()
			{
				if ( this.tabPage != null )  this.tabPage.Dispose();
				if ( this.hScroller != null )  this.hScroller.Dispose();
				if ( this.vScroller != null )  this.vScroller.Dispose();
				if ( this.quickPageMenu != null )  this.quickPageMenu.Dispose();
				if ( this.quickLayerMenu != null )  this.quickLayerMenu.Dispose();
				if ( this.bookPanels != null )  this.bookPanels.Dispose();
				if ( this.containerPrincipal != null )  this.containerPrincipal.Dispose();
				if ( this.containerStyles != null )  this.containerStyles.Dispose();
				if ( this.containerAutos != null )  this.containerAutos.Dispose();
				if ( this.containerPages != null )  this.containerPages.Dispose();
				if ( this.containerLayers != null )  this.containerLayers.Dispose();
				if ( this.containerOperations != null )  this.containerOperations.Dispose();
			}
		}
	}
}
