using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Document;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Soap;

namespace Epsitec.App.DocumentEditor
{
	using Drawing        = Common.Drawing;
	using Widgets        = Common.Widgets;
	using Containers     = Common.Document.Containers;
	using Objects        = Common.Document.Objects;
	using Settings       = Common.Document.Settings;
	using GlobalSettings = Common.Document.Settings.GlobalSettings;

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

			if ( this.type == DocumentType.Pictogram )
			{
				this.installType = InstallType.Full;
			}
			else
			{
				string key = Common.Support.SerialAlgorithm.ReadSerial();
				this.askKey = (key == null);

				if ( key == null || key == "demo" )
				{
					this.installType = InstallType.Demo;
				}
				else if ( Common.Support.SerialAlgorithm.CheckSerial(key) )
				{
					this.installType = InstallType.Full;
				}
				else
				{
					this.installType = InstallType.Expired;
				}
			}

			if ( !this.ReadGlobalSettings() )
			{
				this.globalSettings = new GlobalSettings();
				this.globalSettings.Initialise(this.type);
			}

			Epsitec.Common.Widgets.Adorner.Factory.SetActive(this.globalSettings.Adorner);

			this.dlgSplash   = new Dialogs.Splash(this);
			this.dlgKey      = new Dialogs.Key(this);
			this.dlgUpdate   = new Dialogs.Update(this);
			this.dlgAbout    = new Dialogs.About(this);
			this.dlgInfos    = new Dialogs.Infos(this);
			this.dlgExport   = new Dialogs.Export(this);
			this.dlgSettings = new Dialogs.Settings(this);

			if ( this.type != DocumentType.Pictogram &&
				 this.globalSettings.SplashScreen )
			{
				this.dlgSplash.Show();

				// Donne l'occasion aux événements d'affichage d'être traités:
				Window.PumpEvents();
			}
			this.CreateLayout();
			this.InitCommands();

			this.clipboard = new Document(this.type, DocumentMode.Clipboard, this.installType, this.globalSettings, this.CommandDispatcher);
			this.clipboard.Name = "Clipboard";

			this.documents = new System.Collections.ArrayList();
			this.currentDocument = -1;

			string[] args = System.Environment.GetCommandLineArgs();
			if ( args.Length >= 2 )
			{
				this.CreateDocument();
				string filename = args[1];
				string err = "";
				if ( filename.ToLower().EndsWith(".crcolors") )
				{
					err = this.PaletteRead(filename);
				}
				else
				{
					err = this.CurrentDocument.Read(filename);
				}
				this.UpdateRulers();
				if ( err == "" )
				{
					this.DialogWarnings(this.commandDispatcher, this.CurrentDocument.ReadWarnings);
				}
				this.DialogError(this.commandDispatcher, err);
			}
			else
			{
				if ( this.globalSettings.FirstAction == Settings.FirstAction.OpenNewDocument )
				{
					this.CreateDocument();
				}

				if ( this.globalSettings.FirstAction == Settings.FirstAction.OpenLastFile &&
					 this.globalSettings.LastFilenameCount > 0 )
				{
					this.CreateDocument();
					string filename = this.globalSettings.LastFilenameGet(0);
					string err = this.CurrentDocument.Read(filename);
					this.UpdateRulers();
					if ( err == "" )
					{
						this.DialogWarnings(this.commandDispatcher, this.CurrentDocument.ReadWarnings);
					}
				}
			}

			if ( this.IsCurrentDocument )
			{
				this.firstInitialise = true;
				this.CurrentDocument.Notifier.NotifyAllChanged();
				this.CurrentDocument.Notifier.GenerateEvents();
			}
			else
			{
				this.UseDocument(-1);
				this.UpdateCloseCommand();
			}
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}

		public DocumentType Type
		{
			get { return this.type; }
		}
		
		public InstallType InstallType
		{
			get
			{
				return this.installType;
			}

			set
			{
				this.installType = value;

				int total = this.documents.Count;
				for ( int i=0 ; i<total ; i++ )
				{
					DocumentInfo di = this.documents[i] as DocumentInfo;
					di.document.InstallType = this.installType;
				}
			}
		}
		
		public GlobalSettings GlobalSettings
		{
			get { return this.globalSettings; }
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


		// Appelé lorsque l'application a fini de démarrer.
		public void Finalize()
		{
			if ( this.askKey )
			{
				this.askKey = false;
				this.dlgSplash.Hide();
				this.dlgKey.Show();
			}
		}

		public void AsyncNotify()
		{
			if ( this.currentDocument < 0 )  return;
			this.CurrentDocument.Notifier.GenerateEvents();

			if ( this.firstInitialise )
			{
				this.firstInitialise = false;
				if ( this.IsCurrentDocument )
				{
					DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
					context.ZoomPageAndCenter();
					this.CurrentDocument.Modifier.ActiveViewer.Focus();
				}
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
			this.MenuAdd(fileMenu, "manifest:Epsitec.App.DocumentEditor.Images.New.icon", "New", "Nouveau", "Ctrl+N");
			this.MenuAdd(fileMenu, "manifest:Epsitec.App.DocumentEditor.Images.Open.icon", "Open", "Ouvrir...", "Ctrl+O");
			this.MenuAdd(fileMenu, "", "Close", "Fermer", "");
			this.MenuAdd(fileMenu, "", "CloseAll", "Fermer tout", "");
			this.MenuAdd(fileMenu, "", "", "", "");
			this.MenuAdd(fileMenu, "manifest:Epsitec.App.DocumentEditor.Images.Save.icon", "Save", "Enregistrer", "Ctrl+S");
			this.MenuAdd(fileMenu, "manifest:Epsitec.App.DocumentEditor.Images.SaveAs.icon", "SaveAs", "Enregistrer sous...", "");
			this.MenuAdd(fileMenu, "", "", "", "");
			this.MenuAdd(fileMenu, "manifest:Epsitec.App.DocumentEditor.Images.Print.icon", "Print", "Imprimer...", "Ctrl+P");
			this.MenuAdd(fileMenu, "manifest:Epsitec.App.DocumentEditor.Images.Export.icon", "Export", "Exporter...", "");
			this.MenuAdd(fileMenu, "", "", "", "");
			this.MenuAdd(fileMenu, "", "", "Fichiers récents", "");
			this.MenuAdd(fileMenu, "", "", "", "");
			this.MenuAdd(fileMenu, "", "QuitApplication", "Quitter", "");
			fileMenu.AdjustSize();
			this.menu.Items[i++].Submenu = fileMenu;

			this.fileMenu = fileMenu;
			this.BuildLastFilenamesMenu();

			VMenu editMenu = new VMenu();
			editMenu.Name = "Edit";
			editMenu.Host = this;
			this.MenuAdd(editMenu, "manifest:Epsitec.App.DocumentEditor.Images.Undo.icon", "Undo", "Annuler", "Ctrl+Z");
			this.MenuAdd(editMenu, "manifest:Epsitec.App.DocumentEditor.Images.Redo.icon", "Redo", "Refaire", "Ctrl+Y");
			this.MenuAdd(editMenu, "", "", "", "");
			this.MenuAdd(editMenu, "manifest:Epsitec.App.DocumentEditor.Images.Cut.icon", "Cut", "Couper", "Ctrl+X");
			this.MenuAdd(editMenu, "manifest:Epsitec.App.DocumentEditor.Images.Copy.icon", "Copy", "Copier", "Ctrl+C");
			this.MenuAdd(editMenu, "manifest:Epsitec.App.DocumentEditor.Images.Paste.icon", "Paste", "Coller", "Ctrl+V");
			this.MenuAdd(editMenu, "", "", "", "");
			this.MenuAdd(editMenu, "manifest:Epsitec.App.DocumentEditor.Images.Delete.icon", "Delete", "Supprimer", "Del");
			this.MenuAdd(editMenu, "manifest:Epsitec.App.DocumentEditor.Images.Duplicate.icon", "Duplicate", "Dupliquer", "Ctrl+D");
			editMenu.AdjustSize();
			this.menu.Items[i++].Submenu = editMenu;

			VMenu objMenu = new VMenu();
			objMenu.Name = "Obj";
			objMenu.Host = this;
			this.MenuAdd(objMenu, "manifest:Epsitec.App.DocumentEditor.Images.Deselect.icon", "Deselect", "Désélectionner tout", "Esc");
			this.MenuAdd(objMenu, "manifest:Epsitec.App.DocumentEditor.Images.SelectAll.icon", "SelectAll", "Tout sélectionner", "Ctrl+A");
			this.MenuAdd(objMenu, "manifest:Epsitec.App.DocumentEditor.Images.SelectInvert.icon", "SelectInvert", "Inverser la sélection", "");
			this.MenuAdd(objMenu, "", "", "", "");
			this.MenuAdd(objMenu, "y/n", "HideHalf", "Mode estompé", "");
			this.MenuAdd(objMenu, "manifest:Epsitec.App.DocumentEditor.Images.HideSel.icon", "HideSel", "Cacher la sélection", "");
			this.MenuAdd(objMenu, "manifest:Epsitec.App.DocumentEditor.Images.HideRest.icon", "HideRest", "Cacher le reste", "");
			this.MenuAdd(objMenu, "manifest:Epsitec.App.DocumentEditor.Images.HideCancel.icon", "HideCancel", "Montrer tout", "");
			this.MenuAdd(objMenu, "", "", "", "");
			this.MenuAdd(objMenu, "manifest:Epsitec.App.DocumentEditor.Images.OrderUp.icon", "OrderUp", "Dessus", "");
			this.MenuAdd(objMenu, "manifest:Epsitec.App.DocumentEditor.Images.OrderDown.icon", "OrderDown", "Dessous", "");
			this.MenuAdd(objMenu, "", "", "", "");
			this.MenuAdd(objMenu, "manifest:Epsitec.App.DocumentEditor.Images.OperMoveH.icon", "", "Opérations", "");
			this.MenuAdd(objMenu, "manifest:Epsitec.App.DocumentEditor.Images.GroupEmpty.icon", "", "Groupe", "");
			this.MenuAdd(objMenu, "manifest:Epsitec.App.DocumentEditor.Images.Combine.icon", "", "Géométrie", "");
			this.MenuAdd(objMenu, "manifest:Epsitec.App.DocumentEditor.Images.BooleanOr.icon", "", "Booléen", "");
			objMenu.AdjustSize();
			this.menu.Items[i++].Submenu = objMenu;

			VMenu operMenu = new VMenu();
			operMenu.Name = "Oper";
			operMenu.Host = this;
			this.MenuAdd(operMenu, "manifest:Epsitec.App.DocumentEditor.Images.OperRot90.icon", "Rotate90", "Quart de tour à gauche", "");
			this.MenuAdd(operMenu, "manifest:Epsitec.App.DocumentEditor.Images.OperRot180.icon", "Rotate180", "Demi-tour", "");
			this.MenuAdd(operMenu, "manifest:Epsitec.App.DocumentEditor.Images.OperRot270.icon", "Rotate270", "Quart de tour à droite", "");
			this.MenuAdd(operMenu, "", "", "", "");
			this.MenuAdd(operMenu, "manifest:Epsitec.App.DocumentEditor.Images.OperMirrorH.icon", "MirrorH", "Miroir horizontal", "");
			this.MenuAdd(operMenu, "manifest:Epsitec.App.DocumentEditor.Images.OperMirrorV.icon", "MirrorV", "Miroir vertical", "");
			this.MenuAdd(operMenu, "", "", "", "");
			this.MenuAdd(operMenu, "manifest:Epsitec.App.DocumentEditor.Images.OperZoomDiv2.icon", "ZoomDiv2", "Réduction /2", "");
			this.MenuAdd(operMenu, "manifest:Epsitec.App.DocumentEditor.Images.OperZoomMul2.icon", "ZoomMul2", "Agrandissement x2", "");
			operMenu.AdjustSize();
			objMenu.Items[12].Submenu = operMenu;

			VMenu groupMenu = new VMenu();
			groupMenu.Name = "Group";
			groupMenu.Host = this;
			this.MenuAdd(groupMenu, "manifest:Epsitec.App.DocumentEditor.Images.Merge.icon", "Merge", "Fusionner", "");
			this.MenuAdd(groupMenu, "manifest:Epsitec.App.DocumentEditor.Images.Group.icon", "Group", "Associer", "");
			this.MenuAdd(groupMenu, "manifest:Epsitec.App.DocumentEditor.Images.Ungroup.icon", "Ungroup", "Dissocier", "");
			this.MenuAdd(groupMenu, "", "", "", "");
			this.MenuAdd(groupMenu, "manifest:Epsitec.App.DocumentEditor.Images.Inside.icon", "Inside", "Entrer dans le groupe", "");
			this.MenuAdd(groupMenu, "manifest:Epsitec.App.DocumentEditor.Images.Outside.icon", "Outside", "Sortir du groupe", "");
			groupMenu.AdjustSize();
			objMenu.Items[13].Submenu = groupMenu;

			VMenu geomMenu = new VMenu();
			geomMenu.Name = "Geom";
			geomMenu.Host = this;
			this.MenuAdd(geomMenu, "manifest:Epsitec.App.DocumentEditor.Images.Combine.icon", "Combine", "Combiner", "");
			this.MenuAdd(geomMenu, "manifest:Epsitec.App.DocumentEditor.Images.Uncombine.icon", "Uncombine", "Scinder", "");
			this.MenuAdd(geomMenu, "", "", "", "");
			this.MenuAdd(geomMenu, "manifest:Epsitec.App.DocumentEditor.Images.ToBezier.icon", "ToBezier", "Convertir en courbes", "");
			this.MenuAdd(geomMenu, "manifest:Epsitec.App.DocumentEditor.Images.ToPoly.icon", "ToPoly", "Convertir en droites", "");
			this.MenuAdd(geomMenu, "", "", "", "");
			this.MenuAdd(geomMenu, "manifest:Epsitec.App.DocumentEditor.Images.Fragment.icon", "Fragment", "Fragmenter", "");
#if DEBUG
			this.MenuAdd(geomMenu, "", "ToSimplest", "Simplifie (debug)", "");
#endif
			geomMenu.AdjustSize();
			objMenu.Items[14].Submenu = geomMenu;

			VMenu boolMenu = new VMenu();
			boolMenu.Name = "Bool";
			boolMenu.Host = this;
			this.MenuAdd(boolMenu, "manifest:Epsitec.App.DocumentEditor.Images.BooleanOr.icon", "BooleanOr", "Union", "");
			this.MenuAdd(boolMenu, "manifest:Epsitec.App.DocumentEditor.Images.BooleanAnd.icon", "BooleanAnd", "Intersection", "");
			this.MenuAdd(boolMenu, "manifest:Epsitec.App.DocumentEditor.Images.BooleanXor.icon", "BooleanXor", "Exclusion", "");
			this.MenuAdd(boolMenu, "manifest:Epsitec.App.DocumentEditor.Images.BooleanFrontMinus.icon", "BooleanFrontMinus", "Avant moins arrières", "");
			this.MenuAdd(boolMenu, "manifest:Epsitec.App.DocumentEditor.Images.BooleanBackMinus.icon", "BooleanBackMinus", "Arrière moins avants", "");
			boolMenu.AdjustSize();
			objMenu.Items[15].Submenu = boolMenu;

			VMenu showMenu = new VMenu();
			showMenu.Name = "Show";
			showMenu.Host = this;
			this.MenuAdd(showMenu, "manifest:Epsitec.App.DocumentEditor.Images.Preview.icon", "Preview", "Comme imprimé", "");
			this.MenuAdd(showMenu, "manifest:Epsitec.App.DocumentEditor.Images.Grid.icon", "Grid", "Grille magnétique", "");
			if ( this.type != DocumentType.Pictogram )
			{
				this.MenuAdd(showMenu, "manifest:Epsitec.App.DocumentEditor.Images.Rulers.icon", "Rulers", "Règles graduées", "");
				this.MenuAdd(showMenu, "manifest:Epsitec.App.DocumentEditor.Images.Labels.icon", "Labels", "Noms des objets", "");
			}
			this.MenuAdd(showMenu, "", "", "", "");
			this.MenuAdd(showMenu, "manifest:Epsitec.App.DocumentEditor.Images.ZoomMin.icon", "ZoomMin", "Zoom minimal", "");
			if ( this.type != DocumentType.Pictogram )
			{
				this.MenuAdd(showMenu, "manifest:Epsitec.App.DocumentEditor.Images.ZoomPage.icon", "ZoomPage", "Zoom pleine page", "");
				this.MenuAdd(showMenu, "manifest:Epsitec.App.DocumentEditor.Images.ZoomPageWidth.icon", "ZoomPageWidth", "Zoom largeur page", "");
			}
			this.MenuAdd(showMenu, "manifest:Epsitec.App.DocumentEditor.Images.ZoomDefault.icon", "ZoomDefault", "Zoom 100%", "");
			this.MenuAdd(showMenu, "manifest:Epsitec.App.DocumentEditor.Images.ZoomSel.icon", "ZoomSel", "Zoom sélection", "");
			if ( this.type != DocumentType.Pictogram )
			{
				this.MenuAdd(showMenu, "manifest:Epsitec.App.DocumentEditor.Images.ZoomSelWidth.icon", "ZoomSelWidth", "Zoom largeur sélection", "");
			}
			this.MenuAdd(showMenu, "manifest:Epsitec.App.DocumentEditor.Images.ZoomPrev.icon", "ZoomPrev", "Zoom précédent", "");
			this.MenuAdd(showMenu, "manifest:Epsitec.App.DocumentEditor.Images.ZoomSub.icon", "ZoomSub", "Réduction", "NumPad -");
			this.MenuAdd(showMenu, "manifest:Epsitec.App.DocumentEditor.Images.ZoomAdd.icon", "ZoomAdd", "Agrandissement", "NumPad +");
			showMenu.AdjustSize();
			this.menu.Items[i++].Submenu = showMenu;

			if ( this.useArray )
			{
				VMenu arrayMenu = new VMenu();
				arrayMenu.Name = "Array";
				arrayMenu.Host = this;
				this.MenuAdd(arrayMenu, "manifest:Epsitec.App.DocumentEditor.Images.ArrayFrame.icon", "ArrayOutlineFrame", "Modifie le cadre", "");
				this.MenuAdd(arrayMenu, "manifest:Epsitec.App.DocumentEditor.Images.ArrayHoriz.icon", "ArrayOutlineHoriz", "Modifie l'intérieur horizontal", "");
				this.MenuAdd(arrayMenu, "manifest:Epsitec.App.DocumentEditor.Images.ArrayVerti.icon", "ArrayOutlineVerti", "Modifie l'intérieur vertical", "");
				this.MenuAdd(arrayMenu, "", "", "", "");
				this.MenuAdd(arrayMenu, "", "", "Assistants", "");
				this.MenuAdd(arrayMenu, "", "", "", "");
				this.MenuAdd(arrayMenu, "", "ArrayAddColumnLeft", "Insérer des colonnes à gauche", "");
				this.MenuAdd(arrayMenu, "", "ArrayAddColumnRight", "Insérer des colonnes à droite", "");
				this.MenuAdd(arrayMenu, "", "ArrayAddRowTop", "Insérer des lignes en dessus", "");
				this.MenuAdd(arrayMenu, "", "ArrayAddRowBottom", "Insérer des lignes en dessous", "");
				this.MenuAdd(arrayMenu, "", "", "", "");
				this.MenuAdd(arrayMenu, "", "ArrayDelColumn", "Supprimer les colonnes", "");
				this.MenuAdd(arrayMenu, "", "ArrayDelRow", "Supprimer les lignes", "");
				this.MenuAdd(arrayMenu, "", "", "", "");
				this.MenuAdd(arrayMenu, "", "ArrayAlignColumn", "Egaliser les largeurs de colonne", "");
				this.MenuAdd(arrayMenu, "", "ArrayAlignRow", "Egaliser les hauteurs de ligne", "");
				this.MenuAdd(arrayMenu, "", "", "", "");
				this.MenuAdd(arrayMenu, "", "ArraySwapColumn", "Permuter le contenu des colonnes", "");
				this.MenuAdd(arrayMenu, "", "ArraySwapRow", "Permuter le contenu des lignes", "");
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
						this.MenuAdd(arrayLookMenu, "", "", "", "");
					}
					else
					{
						this.MenuAdd(arrayLookMenu, "", "ArrayLook(this.Name)", text, "", name);
					}
				}
				arrayLookMenu.AdjustSize();
				arrayMenu.Items[4].Submenu = arrayLookMenu;
			}

			VMenu docMenu = new VMenu();
			docMenu.Name = "Document";
			docMenu.Host = this;
			this.MenuAdd(docMenu, "manifest:Epsitec.App.DocumentEditor.Images.Settings.icon", "Settings", "Réglages...", "F5");
			this.MenuAdd(docMenu, "manifest:Epsitec.App.DocumentEditor.Images.Infos.icon", "Infos", "Informations...", "");
			this.MenuAdd(docMenu, "", "", "", "");
			this.MenuAdd(docMenu, "manifest:Epsitec.App.DocumentEditor.Images.PageNew.icon", "PageCreate", "Nouvelle page", "");
			this.MenuAdd(docMenu, "manifest:Epsitec.App.DocumentEditor.Images.Up.icon", "PageUp", "Reculer la page", "");
			this.MenuAdd(docMenu, "manifest:Epsitec.App.DocumentEditor.Images.Down.icon", "PageDown", "Avancer la page", "");
			this.MenuAdd(docMenu, "manifest:Epsitec.App.DocumentEditor.Images.DeleteItem.icon", "PageDelete", "Supprimer la page", "");
			this.MenuAdd(docMenu, "", "", "", "");
			this.MenuAdd(docMenu, "manifest:Epsitec.App.DocumentEditor.Images.LayerNew.icon", "LayerCreate", "Nouveau calque", "");
			this.MenuAdd(docMenu, "manifest:Epsitec.App.DocumentEditor.Images.Up.icon", "LayerDown", "Monter le calque", "");
			this.MenuAdd(docMenu, "manifest:Epsitec.App.DocumentEditor.Images.Down.icon", "LayerUp", "Descendre le calque", "");
			this.MenuAdd(docMenu, "manifest:Epsitec.App.DocumentEditor.Images.DeleteItem.icon", "LayerDelete", "Supprimer le calque", "");
			docMenu.AdjustSize();
			this.menu.Items[i++].Submenu = docMenu;

#if DEBUG
			VMenu debugMenu = new VMenu();
			debugMenu.Name = "Debug";
			debugMenu.Host = this;
			this.MenuAdd(debugMenu, "y/n", "DebugBboxThin", "BBoxThin", "");
			this.MenuAdd(debugMenu, "y/n", "DebugBboxGeom", "BBoxGeom", "");
			this.MenuAdd(debugMenu, "y/n", "DebugBboxFull", "BBoxFull", "");
			this.MenuAdd(debugMenu, "", "", "", "");
			this.MenuAdd(debugMenu, "", "DebugDirty", "Salir", "F12");
			this.MenuAdd(debugMenu, "", "", "", "");
			this.MenuAdd(debugMenu, "manifest:Epsitec.App.DocumentEditor.Images.SelectTotal.icon", "SelectTotal", "Sélection totale requise", "");
			this.MenuAdd(debugMenu, "manifest:Epsitec.App.DocumentEditor.Images.SelectPartial.icon", "SelectPartial", "Sélection partielle autorisée", "");
			debugMenu.AdjustSize();
			this.menu.Items[i++].Submenu = debugMenu;
#endif

			VMenu helpMenu = new VMenu();
			helpMenu.Name = "Help";
			helpMenu.Host = this;
			this.MenuAdd(helpMenu, "manifest:Epsitec.App.DocumentEditor.Images.Key.icon", "KeyApplication", "Clé...", "");
			this.MenuAdd(helpMenu, "manifest:Epsitec.App.DocumentEditor.Images.Update.icon", "UpdateApplication", "Mise à jour...", "");
			this.MenuAdd(helpMenu, "", "", "", "");
			this.MenuAdd(helpMenu, "manifest:Epsitec.App.DocumentEditor.Images.About.icon", "AboutApplication", "A propos de...", "");
			helpMenu.AdjustSize();
			this.menu.Items[i++].Submenu = helpMenu;

			this.hToolBar = new HToolBar(this);
			this.hToolBar.Anchor = AnchorStyles.LeftAndRight | AnchorStyles.Top;
			this.HToolBarAdd("manifest:Epsitec.App.DocumentEditor.Images.New.icon", "New", "Nouveau");
			this.HToolBarAdd("manifest:Epsitec.App.DocumentEditor.Images.Open.icon", "Open", "Ouvrir");
			this.HToolBarAdd("manifest:Epsitec.App.DocumentEditor.Images.Save.icon", "Save", "Enregistrer");
			this.HToolBarAdd("manifest:Epsitec.App.DocumentEditor.Images.SaveAs.icon", "SaveAs", "Enregistrer sous");
			this.HToolBarAdd("manifest:Epsitec.App.DocumentEditor.Images.Print.icon", "Print", "Imprimer");
			this.HToolBarAdd("manifest:Epsitec.App.DocumentEditor.Images.Export.icon", "Export", "Exporter");
			this.HToolBarAdd("", "", "");
			this.HToolBarAdd("manifest:Epsitec.App.DocumentEditor.Images.Delete.icon", "Delete", "Supprimer");
			this.HToolBarAdd("manifest:Epsitec.App.DocumentEditor.Images.Duplicate.icon", "Duplicate", "Dupliquer");
			this.HToolBarAdd("", "", "");
			this.HToolBarAdd("manifest:Epsitec.App.DocumentEditor.Images.Cut.icon", "Cut", "Couper");
			this.HToolBarAdd("manifest:Epsitec.App.DocumentEditor.Images.Copy.icon", "Copy", "Copier");
			this.HToolBarAdd("manifest:Epsitec.App.DocumentEditor.Images.Paste.icon", "Paste", "Coller");
			this.HToolBarAdd("", "", "");
			this.HToolBarAdd("manifest:Epsitec.App.DocumentEditor.Images.Undo.icon", "Undo", "Annuler");
			this.HToolBarAdd("manifest:Epsitec.App.DocumentEditor.Images.Redo.icon", "Redo", "Refaire");
			this.HToolBarAdd("", "", "");
			this.HToolBarAdd("manifest:Epsitec.App.DocumentEditor.Images.OrderUp.icon", "OrderUp", "Dessus");
			this.HToolBarAdd("manifest:Epsitec.App.DocumentEditor.Images.OrderDown.icon", "OrderDown", "Dessous");
			this.HToolBarAdd("", "", "");
			this.HToolBarAdd("manifest:Epsitec.App.DocumentEditor.Images.Merge.icon", "Merge", "Fusionner");
			this.HToolBarAdd("manifest:Epsitec.App.DocumentEditor.Images.Group.icon", "Group", "Associer");
			this.HToolBarAdd("manifest:Epsitec.App.DocumentEditor.Images.Ungroup.icon", "Ungroup", "Dissocier");
			this.HToolBarAdd("manifest:Epsitec.App.DocumentEditor.Images.Inside.icon", "Inside", "Entrer dans le groupe");
			this.HToolBarAdd("manifest:Epsitec.App.DocumentEditor.Images.Outside.icon", "Outside", "Sortir du groupe");
			this.HToolBarAdd("", "", "");
			this.HToolBarAdd("manifest:Epsitec.App.DocumentEditor.Images.Preview.icon", "Preview", "Comme imprimé");
			this.HToolBarAdd("manifest:Epsitec.App.DocumentEditor.Images.Grid.icon", "Grid", "Grille magnétique");
			this.HToolBarAdd("manifest:Epsitec.App.DocumentEditor.Images.Labels.icon", "Labels", "Noms des objets");
			this.HToolBarAdd("manifest:Epsitec.App.DocumentEditor.Images.Settings.icon", "Settings", "Réglages...");
			this.HToolBarAdd("manifest:Epsitec.App.DocumentEditor.Images.Infos.icon", "Infos", "Informations...");
			this.HToolBarAdd("", "", "");
			if ( this.useArray )
			{
				this.HToolBarAdd("manifest:Epsitec.App.DocumentEditor.Images.ArrayFrame.icon", "ArrayOutlineFrame", "Modifie le cadre");
				this.HToolBarAdd("manifest:Epsitec.App.DocumentEditor.Images.ArrayHoriz.icon", "ArrayOutlineHoriz", "Modifie l'intérieur horizontal");
				this.HToolBarAdd("manifest:Epsitec.App.DocumentEditor.Images.ArrayVerti.icon", "ArrayOutlineVerti", "Modifie l'intérieur vertical");
				this.HToolBarAdd("", "", "");
			}

			this.info = new StatusBar(this);
			this.info.Anchor = AnchorStyles.LeftAndRight | AnchorStyles.Bottom;
			this.InfoAdd("", 120, "StatusDocument", "");
			this.InfoAdd("manifest:Epsitec.App.DocumentEditor.Images.Deselect.icon", 0, "Deselect", "Désélectionner tout");
			this.InfoAdd("manifest:Epsitec.App.DocumentEditor.Images.SelectAll.icon", 0, "SelectAll", "Tout sélectionner");
			this.InfoAdd("manifest:Epsitec.App.DocumentEditor.Images.SelectInvert.icon", 0, "SelectInvert", "Inverser la sélection");
			this.InfoAdd("manifest:Epsitec.App.DocumentEditor.Images.HideSel.icon", 0, "HideSel", "Cacher la sélection");
			this.InfoAdd("manifest:Epsitec.App.DocumentEditor.Images.HideRest.icon", 0, "HideRest", "Cacher le reste");
			this.InfoAdd("manifest:Epsitec.App.DocumentEditor.Images.HideCancel.icon", 0, "HideCancel", "Montrer tout");
			this.InfoAdd("", 120, "StatusObject", "");
			this.InfoAdd("manifest:Epsitec.App.DocumentEditor.Images.ZoomMin.icon", 0, "ZoomMin", "Zoom minimal");
			if ( this.type != DocumentType.Pictogram )
			{
				this.InfoAdd("manifest:Epsitec.App.DocumentEditor.Images.ZoomPage.icon", 0, "ZoomPage", "Zoom pleine page");
				this.InfoAdd("manifest:Epsitec.App.DocumentEditor.Images.ZoomPageWidth.icon", 0, "ZoomPageWidth", "Zoom largeur page");
			}
			this.InfoAdd("manifest:Epsitec.App.DocumentEditor.Images.ZoomDefault.icon", 0, "ZoomDefault", "Zoom 100%");
			this.InfoAdd("manifest:Epsitec.App.DocumentEditor.Images.ZoomSel.icon", 0, "ZoomSel", "Zoom sélection");
			if ( this.type != DocumentType.Pictogram )
			{
				this.InfoAdd("manifest:Epsitec.App.DocumentEditor.Images.ZoomSelWidth.icon", 0, "ZoomSelWidth", "Zoom largeur sélection");
			}
			this.InfoAdd("manifest:Epsitec.App.DocumentEditor.Images.ZoomPrev.icon", 0, "ZoomPrev", "Zoom précédent");
			this.InfoAdd("manifest:Epsitec.App.DocumentEditor.Images.ZoomSub.icon", 0, "ZoomSub", "Réduction");
			this.InfoAdd("manifest:Epsitec.App.DocumentEditor.Images.ZoomAdd.icon", 0, "ZoomAdd", "Agrandissement");
			this.InfoAdd("", 90, "StatusZoom", "");
			this.InfoAdd("", 110, "StatusMouse", "");
			this.InfoAdd("", 220, "StatusModif", "");

			this.vToolBar = new VToolBar(this);
			this.vToolBar.Anchor = AnchorStyles.TopAndBottom | AnchorStyles.Left;
			this.vToolBar.AnchorMargins = new Margins(0, 0, this.hToolBar.Height, this.info.Height);
			this.VToolBarAdd("manifest:Epsitec.App.DocumentEditor.Images.Select.icon", "SelectTool(this.Name)", "Sélectionner", "Select");
			this.VToolBarAdd("manifest:Epsitec.App.DocumentEditor.Images.Global.icon", "SelectTool(this.Name)", "Rectangle de sélection", "Global");
			this.VToolBarAdd("manifest:Epsitec.App.DocumentEditor.Images.Edit.icon", "SelectTool(this.Name)", "Editer", "Edit");
			this.VToolBarAdd("manifest:Epsitec.App.DocumentEditor.Images.Zoom.icon", "SelectTool(this.Name)", "Agrandir", "Zoom");
			this.VToolBarAdd("manifest:Epsitec.App.DocumentEditor.Images.Hand.icon", "SelectTool(this.Name)", "Déplacer", "Hand");
			this.VToolBarAdd("manifest:Epsitec.App.DocumentEditor.Images.Picker.icon", "SelectTool(this.Name)", "Pipette", "Picker");
			if ( this.type == DocumentType.Pictogram )
			{
				this.VToolBarAdd("manifest:Epsitec.App.DocumentEditor.Images.HotSpot.icon", "SelectTool(this.Name)", "Point chaud", "HotSpot");
			}
			this.VToolBarAdd("", "", "");
			this.VToolBarAdd("manifest:Epsitec.App.DocumentEditor.Images.Line.icon", "SelectTool(this.Name)", "Segment de ligne", "ObjectLine");
			this.VToolBarAdd("manifest:Epsitec.App.DocumentEditor.Images.Rectangle.icon", "SelectTool(this.Name)", "Rectangle", "ObjectRectangle");
			this.VToolBarAdd("manifest:Epsitec.App.DocumentEditor.Images.Circle.icon", "SelectTool(this.Name)", "Cercle", "ObjectCircle");
			this.VToolBarAdd("manifest:Epsitec.App.DocumentEditor.Images.Ellipse.icon", "SelectTool(this.Name)", "Ellipse", "ObjectEllipse");
			this.VToolBarAdd("manifest:Epsitec.App.DocumentEditor.Images.Poly.icon", "SelectTool(this.Name)", "Polygone quelconque", "ObjectPoly");
			this.VToolBarAdd("manifest:Epsitec.App.DocumentEditor.Images.Bezier.icon", "SelectTool(this.Name)", "Courbes de Bézier", "ObjectBezier");
			this.VToolBarAdd("manifest:Epsitec.App.DocumentEditor.Images.Regular.icon", "SelectTool(this.Name)", "Polygone régulier", "ObjectRegular");
			this.VToolBarAdd("manifest:Epsitec.App.DocumentEditor.Images.Surface.icon", "SelectTool(this.Name)", "Surfaces 2d", "ObjectSurface");
			this.VToolBarAdd("manifest:Epsitec.App.DocumentEditor.Images.Volume.icon", "SelectTool(this.Name)", "Volumes 3d", "ObjectVolume");
			this.VToolBarAdd("manifest:Epsitec.App.DocumentEditor.Images.TextLine.icon", "SelectTool(this.Name)", "Ligne de texte", "ObjectTextLine");
			this.VToolBarAdd("manifest:Epsitec.App.DocumentEditor.Images.TextBox.icon", "SelectTool(this.Name)", "Pavé de texte", "ObjectTextBox");
			if ( this.useArray )
			{
				this.VToolBarAdd("manifest:Epsitec.App.DocumentEditor.Images.Array.icon", "SelectTool(this.Name)", "Tableau", "ObjectArray");
			}
			this.VToolBarAdd("manifest:Epsitec.App.DocumentEditor.Images.Image.icon", "SelectTool(this.Name)", "Image bitmap", "ObjectImage");
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
			double sr = 11;  // largeur d'une règle
			double wm = 4;  // marges autour du viewer
			
			di.tabPage = new TabPage();
			this.bookDocuments.Items.Insert(this.currentDocument, di.tabPage);

			Widget mainViewParent;
			double lm = 0;
			double tm = 0;
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
				viewer.AnchorMargins = new Margins(wm, wm+sw+1, 6+wm, wm+sw+1);
				document.Modifier.ActiveViewer = viewer;
				document.Modifier.AttachViewer(viewer);

				Viewer frame1 = new Viewer(document);
				frame1.Parent = rightPane;
				frame1.Anchor = AnchorStyles.LeftAndRight | AnchorStyles.Top;
				frame1.AnchorMargins = new Margins(wm, wm, 6+wm, wm);
				frame1.DrawingContext.LayerDrawingMode = LayerDrawingMode.ShowInactive;
				document.Modifier.AttachViewer(frame1);

				Viewer frame2 = new Viewer(document);
				frame2.Parent = rightPane;
				frame2.Anchor = AnchorStyles.LeftAndRight | AnchorStyles.Top;
				frame2.AnchorMargins = new Margins(wm, wm, 6+wm+30, wm);
				frame2.DrawingContext.LayerDrawingMode = LayerDrawingMode.ShowInactive;
				document.Modifier.AttachViewer(frame2);
			}
			else
			{
				lm = sr-1;
				tm = sr-1;

				mainViewParent = di.tabPage;
				Viewer viewer = new Viewer(document);
				viewer.Parent = mainViewParent;
				viewer.Anchor = AnchorStyles.All;
				viewer.AnchorMargins = new Margins(wm+lm, wm+sw+1, 6+wm+tm, wm+sw+1);
				document.Modifier.ActiveViewer = viewer;
				document.Modifier.AttachViewer(viewer);

				di.hRuler = new HRuler(mainViewParent);
				di.hRuler.AutoCapture = false;
				di.hRuler.Pressed += new MessageEventHandler(this.HandleHRulerPressed);
				di.hRuler.Released += new MessageEventHandler(this.HandleHRulerReleased);
				di.hRuler.Anchor = AnchorStyles.LeftAndRight | AnchorStyles.Top;
				di.hRuler.AnchorMargins = new Margins(wm+lm, wm+sw+1, 6+wm, 0);
				ToolTip.Default.SetToolTip(di.hRuler, "Tirer avec la souris pour créer un repère");

				di.vRuler = new VRuler(mainViewParent);
				di.vRuler.AutoCapture = false;
				di.vRuler.Pressed += new MessageEventHandler(this.HandleVRulerPressed);
				di.vRuler.Released += new MessageEventHandler(this.HandleHRulerReleased);
				di.vRuler.Anchor = AnchorStyles.TopAndBottom | AnchorStyles.Left;
				di.vRuler.AnchorMargins = new Margins(wm, 0, 6+wm+tm, wm+sw+1);
				ToolTip.Default.SetToolTip(di.vRuler, "Tirer avec la souris pour créer un repère");
			}

			// Bande horizontale qui contient les boutons des pages et l'ascenseur.
			Widget hBand = new Widget(mainViewParent);
			hBand.Height = sw;
			hBand.Anchor = AnchorStyles.LeftAndRight | AnchorStyles.Bottom;
			hBand.AnchorMargins = new Margins(wm+lm, wm+sw+1, 0, wm);

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
			vBand.AnchorMargins = new Margins(0, wm, 6+wm+tm, wm+sw+1);

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

		#region LastFilenames
		// Construit le sous-menu des derniers fichiers ouverts.
		protected void BuildLastFilenamesMenu()
		{
			VMenu lastMenu = new VMenu();
			lastMenu.Name = "LastFilenames";
			lastMenu.Host = this;

			int total = this.globalSettings.LastFilenameCount;
			if ( total == 0 )
			{
				this.MenuAdd(lastMenu, "", "", "<i>Aucun</i>", "");
			}
			else
			{
				for ( int i=0 ; i<total ; i++ )
				{
					string filename = string.Format("{0} {1}", (i+1)%10, this.globalSettings.LastFilenameGetShort(i));
					this.MenuAdd(lastMenu, "", "LastFile(this.Name)", filename, "", this.globalSettings.LastFilenameGet(i));
				}
			}

			lastMenu.AdjustSize();
			this.fileMenu.Items[11].Submenu = lastMenu;
		}
		#endregion

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

		private void HandleHRulerPressed(object sender, MessageEventArgs e)
		{
			this.CurrentDocument.Modifier.ActiveViewer.GuideInteractiveStart(true);
		}

		private void HandleHRulerReleased(object sender, MessageEventArgs e)
		{
			this.CurrentDocument.Modifier.ActiveViewer.GuideInteractiveEnd();
		}

		private void HandleVRulerPressed(object sender, MessageEventArgs e)
		{
			this.CurrentDocument.Modifier.ActiveViewer.GuideInteractiveStart(false);
		}

		private void HandleVRulerReleased(object sender, MessageEventArgs e)
		{
			this.CurrentDocument.Modifier.ActiveViewer.GuideInteractiveEnd();
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

		#region IO
		// Affiche le dialogue pour demander s'il faut enregistrer le
		// document modifié, avant de passer à un autre document.
		protected Common.Dialogs.DialogResult DialogSave(CommandDispatcher dispatcher)
		{
			if ( !this.CurrentDocument.IsDirtySerialize ||
				 this.CurrentDocument.Modifier.StatisticTotalObjects() == 0 )
			{
				return Common.Dialogs.DialogResult.None;
			}

			string title = "Crésus";
			string icon = "manifest:Epsitec.Common.Dialogs.Images.Warning.icon";

			string shortFilename = Misc.ExtractName(this.CurrentDocument.Filename, this.CurrentDocument.IsDirtySerialize);
			string statistic = string.Format("<font size=\"80%\">{0}</font><br/>", this.CurrentDocument.Modifier.Statistic(false, false));
			string message = string.Format("<font size=\"100%\">Le fichier {0} a été modifié.</font><br/><br/>{1}Voulez-vous enregistrer les modifications ?", shortFilename, statistic);
			Common.Dialogs.IDialog dialog = Common.Dialogs.Message.CreateYesNoCancel(title, icon, message, "", "", dispatcher);
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
		protected Common.Dialogs.DialogResult DialogDelete(CommandDispatcher dispatcher, string filename)
		{
			string title = "Crésus";
			string icon = "manifest:Epsitec.Common.Dialogs.Images.Warning.icon";

			string shortFilename = string.Format("<b>{0}</b>", Misc.ExtractName(filename));
			string statistic = string.Format("<font size=\"80%\">{0}</font><br/>", DocumentEditor.StatisticFilename(filename));
			string message = string.Format("<font size=\"100%\">Le fichier {0} existe déjà.</font><br/><br/>{1}Voulez-vous le remplacer ?", shortFilename, statistic);

			Common.Dialogs.IDialog dialog = Common.Dialogs.Message.CreateYesNo(title, icon, message, "", "", dispatcher);
			dialog.Owner = this.Window;
			dialog.OpenDialog();
			return dialog.Result;
		}

		// Affiche le dialogue pour signaler la liste de tous les problèmes.
		protected Common.Dialogs.DialogResult DialogWarnings(CommandDispatcher dispatcher, System.Collections.ArrayList warnings)
		{
			if ( warnings == null || warnings.Count == 0 )  return Common.Dialogs.DialogResult.None;
			warnings.Sort();

			string title = "Crésus";
			string icon = "manifest:Epsitec.Common.Dialogs.Images.Warning.icon";

			string chip = "<list type=\"fix\" width=\"1.5\"/>";
			System.Text.StringBuilder builder = new System.Text.StringBuilder();
			builder.Append("Les problèmes suivants ont été rencontrés:<br/>");
			builder.Append("<br/>");
			foreach ( string s in warnings )
			{
				builder.Append(chip);
				builder.Append(s);
				builder.Append("<br/>");
			}
			builder.Append("<br/>");
			builder.Append("Malgré ces problèmes, le fichier a été ouvert le mieux possible.<br/>");
			builder.Append("Son aspect est toutefois différent de ce qu'il devrait être.<br/>");
			string message = builder.ToString();

			Common.Dialogs.IDialog dialog = Common.Dialogs.Message.CreateOk(title, icon, message, "", dispatcher);
			dialog.Owner = this.Window;
			dialog.OpenDialog();
			return dialog.Result;
		}

		// Affiche le dialogue pour signaler une erreur.
		public Common.Dialogs.DialogResult DialogError(CommandDispatcher dispatcher, string error)
		{
			if ( error == "" )  return Common.Dialogs.DialogResult.None;

			string title = "Crésus";
			string icon = "manifest:Epsitec.Common.Dialogs.Images.Warning.icon";
			string message = error;

			Common.Dialogs.IDialog dialog = Common.Dialogs.Message.CreateOk(title, icon, message, "", dispatcher);
			dialog.Owner = this.Window;
			dialog.OpenDialog();
			return dialog.Result;
		}

		// Si on a tapé "toto", mais qu'il existe le fichier "Toto",
		// met le "vrai" nom dans filename.
		protected static string AdjustFilename(string filename)
		{
			string path = System.IO.Path.GetDirectoryName(filename);
			string name = System.IO.Path.GetFileName(filename);
			string[] s;
			try
			{
				s = System.IO.Directory.GetFiles(path, name);
			}
			catch
			{
				return filename;
			}
			if ( s.Length == 1 )
			{
				filename = s[0];
			}
			return filename;
		}

		// Demande un nom de fichier puis ouvre le fichier.
		// Affiche l'erreur éventuelle.
		// Retourne false si le fichier n'a pas été ouvert.
		protected bool Open(CommandDispatcher dispatcher)
		{
			Common.Dialogs.FileOpen dialog = new Common.Dialogs.FileOpen();

			dialog.InitialDirectory = this.globalSettings.InitialDirectory;
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
			dialog.AcceptMultipleSelection = true;
			dialog.Owner = this.Window;
			dialog.OpenDialog();
			if ( dialog.Result != Common.Dialogs.DialogResult.Accept )  return false;

			string[] names = dialog.FileNames;
			if ( names.Length >= 1 )
			{
				this.globalSettings.InitialDirectory = System.IO.Path.GetDirectoryName(names[0]);
			}
			for ( int i=0 ; i<names.Length ; i++ )
			{
				this.Open(names[i]);
			}
			return true;
		}

		// Ouvre un ficher d'après son nom.
		// Affiche l'erreur éventuelle.
		// Retourne false si le fichier n'a pas été ouvert.
		public bool Open(string filename)
		{
			string err = "";
			if ( filename.ToLower().EndsWith("crcolors") )
			{
				if ( !this.IsCurrentDocument )
				{
					this.CreateDocument();
				}
				err = this.PaletteRead(filename);
			}
			else
			{
				// Cherche si ce nom de fichier est déjà ouvert ?
				int total = this.bookDocuments.PageCount;
				for ( int i=0 ; i<total ; i++ )
				{
					DocumentInfo di = this.documents[i] as DocumentInfo;
					if ( di.document.Filename == filename )
					{
						this.globalSettings.LastFilenameAdd(filename);
						this.BuildLastFilenamesMenu();
						this.UseDocument(i);
						return true;
					}
				}

				filename = DocumentEditor.AdjustFilename(filename);

				this.CreateDocument();
				err = this.CurrentDocument.Read(filename);
				this.UpdateRulers();
				if ( err == "" )
				{
					this.UpdateBookDocuments();
					this.DialogWarnings(this.commandDispatcher, this.CurrentDocument.ReadWarnings);
				}
			}
			this.DialogError(this.commandDispatcher, err);
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
				Common.Dialogs.FileSave dialog = new Common.Dialogs.FileSave();
			
				dialog.InitialDirectory = this.globalSettings.InitialDirectory;
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
				if ( dialog.Result != Common.Dialogs.DialogResult.Accept )  return false;
				filename = dialog.FileName;
				filename = DocumentEditor.AdjustFilename(filename);
			}
			else
			{
				filename = this.CurrentDocument.Filename;
			}

			string err = this.CurrentDocument.Write(filename);
			if ( err == "" )
			{
				this.globalSettings.InitialDirectory = System.IO.Path.GetDirectoryName(filename);
				this.globalSettings.LastFilenameAdd(filename);
				this.BuildLastFilenamesMenu();
			}
			this.DialogError(dispatcher, err);
			return (err == "");
		}

		// Fait tout ce qu'il faut pour éventuellement sauvegarder le document
		// avant de passer à autre chose.
		// Retourne false si on ne peut pas continuer.
		protected bool AutoSave(CommandDispatcher dispatcher)
		{
			Common.Dialogs.DialogResult result = this.DialogSave(dispatcher);
			if ( result == Common.Dialogs.DialogResult.Yes )
			{
				return this.Save(dispatcher, false);
			}
			if ( result == Common.Dialogs.DialogResult.Cancel )
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
			this.CurrentDocument.Modifier.ActiveViewer.Focus();
		}

		[Command ("Open")]
		void CommandOpen(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.Open(dispatcher);
			if ( this.IsCurrentDocument )
			{
				this.CurrentDocument.Modifier.ActiveViewer.Focus();
			}
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

		[Command ("LastFile")]
		void CommandLastFile(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			string filename = e.CommandArgs[0];
			this.Open(filename);
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
			Document document = this.CurrentDocument;
			Common.Dialogs.Print dialog = document.PrintDialog;
			document.Printer.RestoreSettings(dialog.Document.PrinterSettings);
			
			dialog.Owner = this.Window;
			dialog.OpenDialog();
			
			if ( dialog.Result == Common.Dialogs.DialogResult.Accept )
			{
				document.Printer.SaveSettings(dialog.Document.PrinterSettings);
				document.Print(dialog);
			}
		}
		
		[Command ("Export")]
		void CommandExport(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			Common.Dialogs.FileSave dialog = new Common.Dialogs.FileSave();
			
			if ( this.CurrentDocument.ExportDirectory == "" )
			{
				if ( this.CurrentDocument.Filename == "" )
				{
					dialog.InitialDirectory = this.globalSettings.InitialDirectory;
				}
				else
				{
					dialog.InitialDirectory = System.IO.Path.GetDirectoryName(this.CurrentDocument.Filename);
				}
			}
			else
			{
				dialog.InitialDirectory = this.CurrentDocument.ExportDirectory;
			}

			dialog.FileName = this.CurrentDocument.ExportFilename;
			dialog.Title = "Exporter la page courante";
			dialog.Filters.Add("bmp", "Bitmap Window BMP", "*.bmp");
			dialog.Filters.Add("tif", "Tagged Image TIFF", "*.tif");
			dialog.Filters.Add("jpg", "Image compressée JPEG", "*.jpg");
			dialog.Filters.Add("png", "Portable Network Graphics PNG", "*.png");
			dialog.Filters.Add("gif", "Graphics Interchange Format GIF", "*.gif");
			//dialog.Filters.Add("exf", "Image EXIF", "*.exif");
			//dialog.Filters.Add("emf", "Image EMF", "*.emf");
			//dialog.Filters.Add("wmf", "Image WMF", "*.wmf");
			dialog.FilterIndex = this.CurrentDocument.ExportFilter;
			dialog.PromptForOverwriting = true;
			dialog.Owner = this.Window;
			dialog.OpenDialog();
			if ( dialog.Result != Common.Dialogs.DialogResult.Accept )  return;

			this.CurrentDocument.ExportDirectory = System.IO.Path.GetDirectoryName(dialog.FileName);
			this.CurrentDocument.ExportFilename = System.IO.Path.GetFileName(dialog.FileName);
			this.CurrentDocument.ExportFilter = dialog.FilterIndex;

#if false
			string dir = this.CurrentDocument.ExportDirectory;
			string name = System.IO.Path.GetFileNameWithoutExtension(this.CurrentDocument.ExportFilename);
			string ext = dialog.Filters[this.CurrentDocument.ExportFilter].Name;
			this.CurrentDocument.ExportFilename = string.Format("{0}\\{1}.{2}", dir, name, ext);
#endif

			string ext = dialog.Filters[this.CurrentDocument.ExportFilter].Name;
			this.CurrentDocument.Printer.ImageFormat = Printer.GetImageFormat(ext);

			this.dlgExport.Show(this.CurrentDocument.ExportFilename);
		}
		
		#region PaletteIO
		[Command ("NewPaletteDefault")]
		void CommandNewPaletteDefault()
		{
			this.CurrentDocument.GlobalSettings.ColorCollection.Initialise(ColorCollectionType.Default);
		}

		[Command ("NewPaletteRainbow")]
		void NewPaletteRainbow()
		{
			this.CurrentDocument.GlobalSettings.ColorCollection.Initialise(ColorCollectionType.Rainbow);
		}

		[Command ("NewPaletteLight")]
		void NewPaletteLight()
		{
			this.CurrentDocument.GlobalSettings.ColorCollection.Initialise(ColorCollectionType.Light);
		}

		[Command ("NewPaletteDark")]
		void NewPaletteDark()
		{
			this.CurrentDocument.GlobalSettings.ColorCollection.Initialise(ColorCollectionType.Dark);
		}

		[Command ("NewPaletteGray")]
		void CommandNewPaletteGray()
		{
			this.CurrentDocument.GlobalSettings.ColorCollection.Initialise(ColorCollectionType.Gray);
		}

		[Command ("OpenPalette")]
		void CommandOpenPalette()
		{
			Common.Dialogs.FileOpen dialog = new Common.Dialogs.FileOpen();
			
			dialog.InitialDirectory = this.CurrentDocument.GlobalSettings.ColorCollectionDirectory;
			dialog.FileName = this.CurrentDocument.GlobalSettings.ColorCollectionFilename;
			dialog.Title = "Ouvrir une palette de couleurs";
			dialog.Filters.Add("crcolors", "Palette de couleurs", "*.crcolors");
			dialog.FilterIndex = this.CurrentDocument.ExportFilter;
			dialog.Owner = this.Window;
			dialog.OpenDialog();
			if ( dialog.Result != Common.Dialogs.DialogResult.Accept )  return;

			this.CurrentDocument.GlobalSettings.ColorCollectionDirectory = System.IO.Path.GetDirectoryName(dialog.FileName);
			this.CurrentDocument.GlobalSettings.ColorCollectionFilename = System.IO.Path.GetFileName(dialog.FileName);

			string err = this.PaletteRead(dialog.FileName);
			this.DialogError(this.commandDispatcher, err);
		}

		[Command ("SavePalette")]
		void CommandSavePalette()
		{
			Common.Dialogs.FileSave dialog = new Common.Dialogs.FileSave();
			
			dialog.InitialDirectory = this.CurrentDocument.GlobalSettings.ColorCollectionDirectory;
			dialog.FileName = this.CurrentDocument.GlobalSettings.ColorCollectionFilename;
			dialog.Title = "Enregistrer la palette de couleurs";
			dialog.Filters.Add("crcolors", "Palette de couleurs", "*.crcolors");
			dialog.FilterIndex = this.CurrentDocument.ExportFilter;
			dialog.PromptForOverwriting = true;
			dialog.Owner = this.Window;
			dialog.OpenDialog();
			if ( dialog.Result != Common.Dialogs.DialogResult.Accept )  return;

			this.CurrentDocument.GlobalSettings.ColorCollectionDirectory = System.IO.Path.GetDirectoryName(dialog.FileName);
			this.CurrentDocument.GlobalSettings.ColorCollectionFilename = System.IO.Path.GetFileName(dialog.FileName);

			string err = this.PaletteWrite(dialog.FileName);
			this.DialogError(this.commandDispatcher, err);
		}

		// Lit la collection de couleurs à partir d'un fichier.
		public string PaletteRead(string filename)
		{
			try
			{
				using ( Stream stream = File.OpenRead(filename) )
				{
					SoapFormatter formatter = new SoapFormatter();
					ColorCollection cc = (ColorCollection) formatter.Deserialize(stream);
					cc.CopyTo(this.CurrentDocument.GlobalSettings.ColorCollection);
				}
			}
			catch ( System.Exception e )
			{
				return e.Message;
			}

			return "";  // ok
		}

		// Ecrit la collection de couleurs dans un fichier.
		public string PaletteWrite(string filename)
		{
			if ( File.Exists(filename) )
			{
				File.Delete(filename);
			}

			try
			{
				using ( Stream stream = File.OpenWrite(filename) )
				{
					SoapFormatter formatter = new SoapFormatter();
					formatter.Serialize(stream, this.CurrentDocument.GlobalSettings.ColorCollection);
				}
			}
			catch ( System.Exception e )
			{
				return e.Message;
			}

			return "";  // ok
		}
		#endregion
		

		[Command ("Delete")]
		void CommandDelete()
		{
			this.CurrentDocument.Modifier.DeleteSelection();
		}

		[Command ("Duplicate")]
		void CommandDuplicate(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			Point move = this.CurrentDocument.Modifier.EffectiveDuplicateMove;
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

		[Command ("TextBold")]
		void CommandTextBold(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.TextBold();
		}

		[Command ("TextItalic")]
		void CommandTextItalic(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.TextItalic();
		}

		[Command ("TextUnderlined")]
		void CommandTextUnderlined(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.TextUnderlined();
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

		[Command ("Combine")]
		void CommandCombine(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.CombineSelection();
		}

		[Command ("Uncombine")]
		void CommandUncombine(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.UncombineSelection();
		}

		[Command ("ToBezier")]
		void CommandToBezier(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.ToBezierSelection();
		}

		[Command ("ToPoly")]
		void CommandToPoly(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.ToPolySelection();
		}

		[Command ("ToSimplest")]
		void CommandToSimplest(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.ToSimplestSelection();
		}

		[Command ("Fragment")]
		void CommandFragment(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.FragmentSelection();
		}

		[Command ("BooleanOr")]
		void CommandBooleanOr(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.BooleanSelection(Drawing.PathOperation.Or);
		}

		[Command ("BooleanAnd")]
		void CommandBooleanAnd(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.BooleanSelection(Drawing.PathOperation.And);
		}

		[Command ("BooleanXor")]
		void CommandBooleanXor(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.BooleanSelection(Drawing.PathOperation.Xor);
		}

		[Command ("BooleanFrontMinus")]
		void CommandBooleanFrontMinus(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.BooleanSelection(Drawing.PathOperation.AMinusB);
		}

		[Command ("BooleanBackMinus")]
		void CommandBooleanBackMinus(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.BooleanSelection(Drawing.PathOperation.BMinusA);
		}

		[Command ("Grid")]
		void CommandGrid(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
			context.GridActive = !context.GridActive;
			context.GridShow = context.GridActive;
		}

		[Command ("Rulers")]
		void CommandRulers(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
			context.RulersShow = !context.RulersShow;
			this.UpdateRulers();
		}

		[Command ("Labels")]
		void CommandLabels(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
			context.LabelsShow = !context.LabelsShow;
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

		[Command ("SelectTotal")]
		void CommandSelectTotal(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.Tool = "Select";
			Viewer viewer = this.CurrentDocument.Modifier.ActiveViewer;
			viewer.PartialSelect = false;
		}

		[Command ("SelectPartial")]
		void CommandSelectPartial(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.Tool = "Select";
			Viewer viewer = this.CurrentDocument.Modifier.ActiveViewer;
			viewer.PartialSelect = true;
		}

		[Command ("SelectorAuto")]
		void CommandSelectorAuto(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.Tool = "Select";
			Viewer viewer = this.CurrentDocument.Modifier.ActiveViewer;
			viewer.SelectorType = SelectorType.Auto;
		}

		[Command ("SelectorIndividual")]
		void CommandSelectorIndividual(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.Tool = "Select";
			Viewer viewer = this.CurrentDocument.Modifier.ActiveViewer;
			viewer.SelectorType = SelectorType.Individual;
		}

		[Command ("SelectorZoom")]
		void CommandSelectorZoom(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.Tool = "Select";
			Viewer viewer = this.CurrentDocument.Modifier.ActiveViewer;
			viewer.SelectorType = SelectorType.Zoomer;
		}

		[Command ("SelectorStretch")]
		void CommandSelectorStretch(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.Tool = "Select";
			Viewer viewer = this.CurrentDocument.Modifier.ActiveViewer;
			viewer.SelectorType = SelectorType.Stretcher;
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

		[Command ("ZoomPage")]
		void CommandZoomPage(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
			this.CurrentDocument.Modifier.ZoomMemorize();
			context.ZoomPageAndCenter();
		}

		[Command ("ZoomPageWidth")]
		void CommandZoomPageWidth(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
			this.CurrentDocument.Modifier.ZoomMemorize();
			context.ZoomPageWidthAndCenter();
		}

		[Command ("ZoomDefault")]
		void CommandZoomDefault(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
			this.CurrentDocument.Modifier.ZoomMemorize();
			context.ZoomDefaultAndCenter();
		}

		[Command ("ZoomSel")]
		void CommandZoomSel(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.ZoomSel();
		}

		[Command ("ZoomSelWidth")]
		void CommandZoomSelWidth(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.ZoomSelWidth();
		}

		[Command ("ZoomPrev")]
		void CommandZoomPrev(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.ZoomPrev();
		}

		[Command ("ZoomSub")]
		void CommandZoomSub(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.ZoomChange(1.0/1.2);
		}

		[Command ("ZoomAdd")]
		void CommandZoomAdd(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.ZoomChange(1.2);
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
			this.dlgSettings.Show();
		}

		[Command ("Infos")]
		void CommandInfos(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.dlgInfos.Show();
		}

		[Command ("KeyApplication")]
		void CommandKeyApplication(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.dlgSplash.Hide();
			this.dlgKey.Show();
		}

		[Command ("UpdateApplication")]
		void CommandUpdateApplication(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.dlgSplash.Hide();
			this.dlgUpdate.Show();
		}

		[Command ("AboutApplication")]
		void CommandAboutApplication(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.dlgSplash.Hide();
			this.dlgAbout.Show();
		}


		[Command ("MoveLeftNorm")]
		void CommandMoveLeftNorm(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.MoveSelection(new Point(-1,0), 0);
		}

		[Command ("MoveRightNorm")]
		void CommandMoveRightNorm(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.MoveSelection(new Point(1,0), 0);
		}

		[Command ("MoveUpNorm")]
		void CommandMoveUpNorm(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.MoveSelection(new Point(0,1), 0);
		}

		[Command ("MoveDownNorm")]
		void CommandMoveDownNorm(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.MoveSelection(new Point(0,-1), 0);
		}

		[Command ("MoveLeftCtrl")]
		void CommandMoveLeftCtrl(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.MoveSelection(new Point(-1,0), -1);
		}

		[Command ("MoveRightCtrl")]
		void CommandMoveRightCtrl(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.MoveSelection(new Point(1,0), -1);
		}

		[Command ("MoveUpCtrl")]
		void CommandMoveUpCtrl(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.MoveSelection(new Point(0,1), -1);
		}

		[Command ("MoveDownCtrl")]
		void CommandMoveDownCtrl(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.MoveSelection(new Point(0,-1), -1);
		}

		[Command ("MoveLeftShift")]
		void CommandMoveLeftShift(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.MoveSelection(new Point(-1,0), 1);
		}

		[Command ("MoveRightShift")]
		void CommandMoveRightShift(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.MoveSelection(new Point(1,0), 1);
		}

		[Command ("MoveUpShift")]
		void CommandMoveUpShift(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.MoveSelection(new Point(0,1), 1);
		}

		[Command ("MoveDownShift")]
		void CommandMoveDownShift(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.MoveSelection(new Point(0,-1), 1);
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
			this.WritedGlobalSettings();
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

				string icon = "manifest:Epsitec.App.DocumentEditor.Images.ActiveNo.icon";
				if ( i == context.CurrentPage )
				{
					icon = "manifest:Epsitec.App.DocumentEditor.Images.ActiveYes.icon";
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
				Objects.Layer layer = layers[ii] as Objects.Layer;

				string name = "";
				if ( layer.Name == "" )
				{
					name = string.Format("{0}: {1}", ((char)('A'+ii)).ToString(), Objects.Layer.LayerPositionName(ii, total));
				}
				else
				{
					name = string.Format("{0}: {1}", ((char)('A'+ii)).ToString(), layer.Name);
				}

				string icon = "manifest:Epsitec.App.DocumentEditor.Images.ActiveNo.icon";
				if ( ii == context.CurrentLayer )
				{
					icon = "manifest:Epsitec.App.DocumentEditor.Images.ActiveYes.icon";
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
			this.openState = new CommandState("Open", this.commandDispatcher, KeyCode.ModifierControl|KeyCode.AlphaO);
			this.saveState = new CommandState("Save", this.commandDispatcher, KeyCode.ModifierControl|KeyCode.AlphaS);
			this.saveAsState = new CommandState("SaveAs", this.commandDispatcher);
			this.closeState = new CommandState("Close", this.commandDispatcher);
			this.closeAllState = new CommandState("CloseAll", this.commandDispatcher);
			this.printState = new CommandState("Print", this.commandDispatcher, KeyCode.ModifierControl|KeyCode.AlphaP);
			this.exportState = new CommandState("Export", this.commandDispatcher);
			this.deleteState = new CommandState("Delete", this.commandDispatcher, KeyCode.Delete);
			this.duplicateState = new CommandState("Duplicate", this.commandDispatcher, KeyCode.ModifierControl|KeyCode.AlphaD);
			this.cutState = new CommandState("Cut", this.commandDispatcher, KeyCode.ModifierControl|KeyCode.AlphaX);
			this.copyState = new CommandState("Copy", this.commandDispatcher, KeyCode.ModifierControl|KeyCode.AlphaC);
			this.pasteState = new CommandState("Paste", this.commandDispatcher, KeyCode.ModifierControl|KeyCode.AlphaV);
			this.textBoldState = new CommandState("TextBold", this.commandDispatcher, KeyCode.ModifierControl|KeyCode.AlphaB);
			this.textItalicState = new CommandState("TextItalic", this.commandDispatcher, KeyCode.ModifierControl|KeyCode.AlphaI);
			this.textUnderlinedState = new CommandState("TextUnderlined", this.commandDispatcher, KeyCode.ModifierControl|KeyCode.AlphaU);
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
			this.combineState = new CommandState("Combine", this.commandDispatcher);
			this.uncombineState = new CommandState("Uncombine", this.commandDispatcher);
			this.toBezierState = new CommandState("ToBezier", this.commandDispatcher);
			this.toPolyState = new CommandState("ToPoly", this.commandDispatcher);
			this.fragmentState = new CommandState("Fragment", this.commandDispatcher);
			this.booleanAndState = new CommandState("BooleanAnd", this.commandDispatcher);
			this.booleanOrState = new CommandState("BooleanOr", this.commandDispatcher);
			this.booleanXorState = new CommandState("BooleanXor", this.commandDispatcher);
			this.booleanFrontMinusState = new CommandState("BooleanFrontMinus", this.commandDispatcher);
			this.booleanBackMinusState = new CommandState("BooleanBackMinus", this.commandDispatcher);
			this.undoState = new CommandState("Undo", this.commandDispatcher, KeyCode.ModifierControl|KeyCode.AlphaZ);
			this.redoState = new CommandState("Redo", this.commandDispatcher, KeyCode.ModifierControl|KeyCode.AlphaY);
			this.deselectState = new CommandState("Deselect", this.commandDispatcher, KeyCode.Escape);
			this.selectAllState = new CommandState("SelectAll", this.commandDispatcher, KeyCode.ModifierControl|KeyCode.AlphaA);
			this.selectInvertState = new CommandState("SelectInvert", this.commandDispatcher);
			this.selectorAutoState = new CommandState("SelectorAuto", this.commandDispatcher);
			this.selectorIndividualState = new CommandState("SelectorIndividual", this.commandDispatcher);
			this.selectorZoomState = new CommandState("SelectorZoom", this.commandDispatcher);
			this.selectorStretchState = new CommandState("SelectorStretch", this.commandDispatcher);
			this.selectTotalState = new CommandState("SelectTotal", this.commandDispatcher);
			this.selectPartialState = new CommandState("SelectPartial", this.commandDispatcher);
			this.hideHalfState = new CommandState("HideHalf", this.commandDispatcher);
			this.hideSelState = new CommandState("HideSel", this.commandDispatcher);
			this.hideRestState = new CommandState("HideRest", this.commandDispatcher);
			this.hideCancelState = new CommandState("HideCancel", this.commandDispatcher);
			this.zoomMinState = new CommandState("ZoomMin", this.commandDispatcher);
			this.zoomPageState = new CommandState("ZoomPage", this.commandDispatcher);
			this.zoomPageWidthState = new CommandState("ZoomPageWidth", this.commandDispatcher);
			this.zoomDefaultState = new CommandState("ZoomDefault", this.commandDispatcher);
			this.zoomSelState = new CommandState("ZoomSel", this.commandDispatcher);
			this.zoomSelWidthState = new CommandState("ZoomSelWidth", this.commandDispatcher);
			this.zoomPrevState = new CommandState("ZoomPrev", this.commandDispatcher);
			this.zoomSubState = new CommandState("ZoomSub", this.commandDispatcher, KeyCode.Substract);
			this.zoomAddState = new CommandState("ZoomAdd", this.commandDispatcher, KeyCode.Add);
			this.previewState = new CommandState("Preview", this.commandDispatcher);
			this.gridState = new CommandState("Grid", this.commandDispatcher);
			this.rulersState = new CommandState("Rulers", this.commandDispatcher);
			this.labelsState = new CommandState("Labels", this.commandDispatcher);

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

			this.settingsState = new CommandState("Settings", this.commandDispatcher, KeyCode.FuncF5);
			this.infosState = new CommandState("Infos", this.commandDispatcher);
			this.aboutState = new CommandState("AboutApplication", this.commandDispatcher);
			this.updateState = new CommandState("UpdateApplication", this.commandDispatcher);
			this.keyState = new CommandState("KeyApplication", this.commandDispatcher);

			this.moveLeftNormState   = new CommandState("MoveLeftNorm",   this.commandDispatcher, KeyCode.ArrowLeft);
			this.moveRightNormState  = new CommandState("MoveRightNorm",  this.commandDispatcher, KeyCode.ArrowRight);
			this.moveUpNormState     = new CommandState("MoveUpNorm",     this.commandDispatcher, KeyCode.ArrowUp);
			this.moveDownNormState   = new CommandState("MoveDownNorm",   this.commandDispatcher, KeyCode.ArrowDown);
			this.moveLeftCtrlState   = new CommandState("MoveLeftCtrl",   this.commandDispatcher, KeyCode.ModifierControl|KeyCode.ArrowLeft);
			this.moveRightCtrlState  = new CommandState("MoveRightCtrl",  this.commandDispatcher, KeyCode.ModifierControl|KeyCode.ArrowRight);
			this.moveUpCtrlState     = new CommandState("MoveUpCtrl",     this.commandDispatcher, KeyCode.ModifierControl|KeyCode.ArrowUp);
			this.moveDownCtrlState   = new CommandState("MoveDownCtrl",   this.commandDispatcher, KeyCode.ModifierControl|KeyCode.ArrowDown);
			this.moveLeftShiftState  = new CommandState("MoveLeftShift",  this.commandDispatcher, KeyCode.ModifierShift|KeyCode.ArrowLeft);
			this.moveRightShiftState = new CommandState("MoveRightShift", this.commandDispatcher, KeyCode.ModifierShift|KeyCode.ArrowRight);
			this.moveUpShiftState    = new CommandState("MoveUpShift",    this.commandDispatcher, KeyCode.ModifierShift|KeyCode.ArrowUp);
			this.moveDownShiftState  = new CommandState("MoveDownShift",  this.commandDispatcher, KeyCode.ModifierShift|KeyCode.ArrowDown);
		}


		// On s'enregistre auprès du document pour tous les événements.
		protected void ConnectEvents()
		{
			this.CurrentDocument.Notifier.DocumentChanged  += new SimpleEventHandler(this.HandleDocumentChanged);
			this.CurrentDocument.Notifier.MouseChanged     += new SimpleEventHandler(this.HandleMouseChanged);
			this.CurrentDocument.Notifier.ModifChanged     += new SimpleEventHandler(this.HandleModifChanged);
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
			this.CurrentDocument.Notifier.SelNamesChanged  += new SimpleEventHandler(this.HandleSelNamesChanged);
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
				this.exportState.Enabled = true;
				this.infosState.Enabled = true;

				this.CurrentDocument.Dialogs.UpdateInfos();
				this.UpdateBookDocuments();
			}
			else
			{
				this.printState.Enabled = false;
				this.exportState.Enabled = false;
				this.infosState.Enabled = false;
			}

			this.BuildLastFilenamesMenu();
		}

		// Appelé par le document lorsque la position de la souris a changé.
		private void HandleMouseChanged()
		{
			// TODO: [PA] Parfois, this.info.Items est nul après avoir cliqué la case de fermeture de la fenêtre !
			if ( this.info.Items == null )  return;

			StatusField field = this.info.Items["StatusMouse"] as StatusField;
			field.Text = this.TextInfoMouse;
			field.Invalidate();

			if ( this.IsCurrentDocument )
			{
				DocumentInfo di = this.CurrentDocumentInfo;
				if ( di.hRuler != null && di.hRuler.IsVisible )
				{
					Point mouse;
					if ( this.CurrentDocument.Modifier.ActiveViewer.MousePos(out mouse) )
					{
						di.hRuler.MarkerVisible = true;
						di.hRuler.Marker = mouse.X;

						di.vRuler.MarkerVisible = true;
						di.vRuler.Marker = mouse.Y;
					}
					else
					{
						di.hRuler.MarkerVisible = false;
						di.vRuler.MarkerVisible = false;
					}
				}
			}
		}

		// Appelé par le document lorsque le texte des modifications a changé.
		private void HandleModifChanged()
		{
			// TODO: [PA] Parfois, this.info.Items est nul après avoir cliqué la case de fermeture de la fenêtre !
			if ( this.info.Items == null )  return;

			StatusField field = this.info.Items["StatusModif"] as StatusField;
			field.Text = this.TextInfoModif;
			field.Invalidate();
		}

		// Appelé par le document lorsque l'origine a changé.
		private void HandleOriginChanged()
		{
			this.UpdateScroller();

			if ( this.IsCurrentDocument )
			{
				DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
				this.zoomPageState.Enabled = !context.IsZoomPage;
				this.zoomPageWidthState.Enabled = !context.IsZoomPageWidth;
				this.zoomDefaultState.Enabled = !context.IsZoomDefault;
			}
			else
			{
				this.zoomPageState.Enabled = false;
				this.zoomPageWidthState.Enabled = false;
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
				this.zoomPageState.Enabled = !context.IsZoomPage;
				this.zoomPageWidthState.Enabled = !context.IsZoomPageWidth;
				this.zoomDefaultState.Enabled = !context.IsZoomDefault;
				this.zoomPrevState.Enabled = ( this.CurrentDocument.Modifier.ZoomMemorizeCount > 0 );
				this.zoomSubState.Enabled = ( context.Zoom > this.CurrentDocument.Modifier.ZoomMin );
				this.zoomAddState.Enabled = ( context.Zoom < this.CurrentDocument.Modifier.ZoomMax );
			}
			else
			{
				this.zoomMinState.Enabled = false;
				this.zoomPageState.Enabled = false;
				this.zoomPageWidthState.Enabled = false;
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
				bool isEdit        = this.CurrentDocument.Modifier.Tool == "Edit";
				SelectorType sType = viewer.SelectorType;
				Objects.Abstract one = this.CurrentDocument.Modifier.RetOnlySelectedObject();

				this.newState.Enabled = true;
				this.openState.Enabled = true;
				this.deleteState.Enabled = ( totalSelected > 0 || isCreating );
				this.duplicateState.Enabled = ( totalSelected > 0 && !isCreating );
				this.orderUpState.Enabled = ( totalObjects > 1 && totalSelected > 0 && !isCreating && !isEdit );
				this.orderDownState.Enabled = ( totalObjects > 1 && totalSelected > 0 && !isCreating && !isEdit );
				this.rotate90State.Enabled = ( totalSelected > 0 && !isCreating && !isEdit );
				this.rotate180State.Enabled = ( totalSelected > 0 && !isCreating && !isEdit );
				this.rotate270State.Enabled = ( totalSelected > 0 && !isCreating && !isEdit );
				this.mirrorHState.Enabled = ( totalSelected > 0 && !isCreating && !isEdit );
				this.mirrorVState.Enabled = ( totalSelected > 0 && !isCreating && !isEdit );
				this.zoomMul2State.Enabled = ( totalSelected > 0 && !isCreating && !isEdit );
				this.zoomDiv2State.Enabled = ( totalSelected > 0 && !isCreating && !isEdit );
				this.mergeState.Enabled = ( totalSelected > 1 && !isCreating && !isEdit );
				this.groupState.Enabled = ( totalSelected > 0 && !isCreating && !isEdit );
				this.ungroupState.Enabled = ( totalSelected == 1 && one is Objects.Group && !isCreating && !isEdit );
				this.insideState.Enabled = ( totalSelected == 1 && one is Objects.Group && !isCreating && !isEdit );
				this.outsideState.Enabled = ( !isBase && !isCreating && !isEdit );
				this.combineState.Enabled = ( totalSelected > 1 && !isCreating && !isEdit );
				this.uncombineState.Enabled = ( totalSelected > 0 && !isCreating && !isEdit );
				this.toBezierState.Enabled = ( totalSelected > 0 && !isCreating && !isEdit );
				this.toPolyState.Enabled = ( totalSelected > 0 && !isCreating && !isEdit );
				this.fragmentState.Enabled = ( totalSelected > 0 && !isCreating && !isEdit );
				this.booleanAndState.Enabled = ( totalSelected > 1 && !isCreating && !isEdit );
				this.booleanOrState.Enabled = ( totalSelected > 1 && !isCreating && !isEdit );
				this.booleanXorState.Enabled = ( totalSelected > 1 && !isCreating && !isEdit );
				this.booleanFrontMinusState.Enabled = ( totalSelected > 1 && !isCreating && !isEdit );
				this.booleanBackMinusState.Enabled = ( totalSelected > 1 && !isCreating && !isEdit );

				this.hideSelState.Enabled = ( totalSelected > 0 && !isCreating );
				this.hideRestState.Enabled = ( totalObjects-totalSelected-totalHide > 0 && !isCreating );
				this.hideCancelState.Enabled = ( totalPageHide > 0 && !isCreating );

				this.zoomSelState.Enabled = ( totalSelected > 0 );
				this.zoomSelWidthState.Enabled = ( totalSelected > 0 );

				this.deselectState.Enabled = ( totalSelected > 0 );
				this.selectAllState.Enabled = ( totalSelected < totalObjects-totalHide );
				this.selectInvertState.Enabled = ( totalSelected > 0 && totalSelected < totalObjects-totalHide );

				this.selectorAutoState.Enabled       = true;
				this.selectorIndividualState.Enabled = true;
				this.selectorZoomState.Enabled       = true;
				this.selectorStretchState.Enabled    = true;
				this.selectorAutoState.ActiveState =       (sType == SelectorType.Auto      ) ? WidgetState.ActiveYes : WidgetState.ActiveNo;
				this.selectorIndividualState.ActiveState = (sType == SelectorType.Individual) ? WidgetState.ActiveYes : WidgetState.ActiveNo;
				this.selectorZoomState.ActiveState =       (sType == SelectorType.Zoomer    ) ? WidgetState.ActiveYes : WidgetState.ActiveNo;
				this.selectorStretchState.ActiveState =    (sType == SelectorType.Stretcher ) ? WidgetState.ActiveYes : WidgetState.ActiveNo;

				this.selectTotalState.Enabled   = true;
				this.selectPartialState.Enabled = true;
				this.selectTotalState.ActiveState   = !viewer.PartialSelect ? WidgetState.ActiveYes : WidgetState.ActiveNo;
				this.selectPartialState.ActiveState =  viewer.PartialSelect ? WidgetState.ActiveYes : WidgetState.ActiveNo;

				Objects.Abstract edit = this.CurrentDocument.Modifier.RetEditObject();
				if ( edit == null )
				{
					this.cutState.Enabled = ( totalSelected > 0 && !isCreating );
					this.copyState.Enabled = ( totalSelected > 0 && !isCreating );
					this.pasteState.Enabled = ( !this.CurrentDocument.Modifier.IsClipboardEmpty() && !isCreating );
					this.textBoldState.Enabled = false;
					this.textItalicState.Enabled = false;
					this.textUnderlinedState.Enabled = false;
				}
				else
				{
					this.cutState.Enabled = true;
					this.copyState.Enabled = true;
					this.pasteState.Enabled = true;
					this.textBoldState.Enabled = true;
					this.textItalicState.Enabled = true;
					this.textUnderlinedState.Enabled = true;
				}

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
				this.textBoldState.Enabled = false;
				this.textItalicState.Enabled = false;
				this.textUnderlinedState.Enabled = false;
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
				this.combineState.Enabled = false;
				this.uncombineState.Enabled = false;
				this.toBezierState.Enabled = false;
				this.toPolyState.Enabled = false;
				this.fragmentState.Enabled = false;
				this.booleanAndState.Enabled = false;
				this.booleanOrState.Enabled = false;
				this.booleanXorState.Enabled = false;
				this.booleanFrontMinusState.Enabled = false;
				this.booleanBackMinusState.Enabled = false;

				this.hideSelState.Enabled = false;
				this.hideRestState.Enabled = false;
				this.hideCancelState.Enabled = false;

				this.zoomSelState.Enabled = false;
				this.zoomSelWidthState.Enabled = false;

				this.deselectState.Enabled = false;
				this.selectAllState.Enabled = false;
				this.selectInvertState.Enabled = false;

				this.selectorAutoState.Enabled       = false;
				this.selectorIndividualState.Enabled = false;
				this.selectorZoomState.Enabled       = false;
				this.selectorStretchState.Enabled    = false;
				this.selectorAutoState.ActiveState       = WidgetState.ActiveNo;
				this.selectorIndividualState.ActiveState = WidgetState.ActiveNo;
				this.selectorZoomState.ActiveState       = WidgetState.ActiveNo;
				this.selectorStretchState.ActiveState    = WidgetState.ActiveNo;

				this.selectTotalState.Enabled   = false;
				this.selectPartialState.Enabled = false;
				this.selectTotalState.ActiveState   = WidgetState.ActiveNo;
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
				this.rulersState.Enabled = true;
				this.rulersState.ActiveState = context.RulersShow ? WidgetState.ActiveYes : WidgetState.ActiveNo;
				this.labelsState.Enabled = true;
				this.labelsState.ActiveState = context.LabelsShow ? WidgetState.ActiveYes : WidgetState.ActiveNo;
			}
			else
			{
				this.gridState.Enabled = false;
				this.gridState.ActiveState = WidgetState.ActiveNo;
				this.rulersState.Enabled = false;
				this.rulersState.ActiveState = WidgetState.ActiveNo;
				this.labelsState.Enabled = false;
				this.labelsState.ActiveState = WidgetState.ActiveNo;
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

		// Appelé lorsque la sélection par noms a changé.
		private void HandleSelNamesChanged()
		{
			if ( this.IsCurrentDocument )
			{
				DocumentInfo di = this.CurrentDocumentInfo;
				di.containerPrincipal.SetDirtySelNames();
			}
		}

		// Appelé par le document lorsque le dessin a changé.
		private void HandleDrawChanged(Viewer viewer, Drawing.Rectangle rect)
		{
			Drawing.Rectangle box = rect;

			if ( viewer.DrawingContext.IsActive )
			{
				box.Inflate(viewer.DrawingContext.HandleSize/2 + 1);
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
			Size cs = context.ContainerSize;
			if ( cs.Width < 0.0 || cs.Height < 0.0 )  return;
			double ratioH = cs.Width/scale.X/area.Width;
			double ratioV = cs.Height/scale.Y/area.Height;
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
			di.hScroller.SmallChange = (decimal) ((cs.Width*0.1)/scale.X);
			di.hScroller.LargeChange = (decimal) ((cs.Width*0.9)/scale.X);

			min = context.MinOriginY;
			max = context.MaxOriginY;
			max = System.Math.Max(min, max);
			di.vScroller.MinValue = (decimal) min;
			di.vScroller.MaxValue = (decimal) max;
			di.vScroller.VisibleRangeRatio = (decimal) ratioV;
			di.vScroller.Value = (decimal) (-context.OriginY);
			di.vScroller.SmallChange = (decimal) ((cs.Height*0.1)/scale.Y);
			di.vScroller.LargeChange = (decimal) ((cs.Height*0.9)/scale.Y);

			if ( di.hRuler != null && di.hRuler.IsVisible )
			{
				di.hRuler.PPM = this.CurrentDocument.Modifier.RealScale;
				di.vRuler.PPM = this.CurrentDocument.Modifier.RealScale;

				Rectangle rect = this.CurrentDocument.Modifier.ActiveViewer.RectangleDisplayed;

				di.hRuler.Starting = rect.Left;
				di.hRuler.Ending   = rect.Right;

				di.vRuler.Starting = rect.Bottom;
				di.vRuler.Ending   = rect.Top;
			}
		}

		// Met à jour les règles, après les avoir montrées ou cachées.
		protected void UpdateRulers()
		{
			if ( !this.IsCurrentDocument )  return;

			Viewer viewer = this.CurrentDocument.Modifier.ActiveViewer;
			DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
			DocumentInfo di = this.CurrentDocumentInfo;
			if ( di.hRuler == null )  return;

			di.hRuler.SetVisible(context.RulersShow);
			di.vRuler.SetVisible(context.RulersShow);

			double sw = 17;  // largeur d'un ascenseur
			double sr = 11;  // largeur d'une règle
			double wm = 4;  // marges autour du viewer
			double lm = 0;
			double tm = 0;
			if ( context.RulersShow )
			{
				lm = sr-1;
				tm = sr-1;
			}
			viewer.AnchorMargins = new Margins(wm+lm, wm+sw+1, 6+wm+tm, wm+sw+1);
		}


		// Texte pour le document.
		protected string TextDocument
		{
			get
			{
				Document doc = this.CurrentDocument;
				if ( doc == null )
				{
					return " ";
				}
				else
				{
					Size size = doc.Size;
					return string.Format("Format {0}x{1}", doc.Modifier.RealToString(size.Width), doc.Modifier.RealToString(size.Height));
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
					return " ";
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
					return " ";
				}
				else
				{
					Point mouse;
					if ( doc.Modifier.ActiveViewer.MousePos(out mouse) )
					{
						return string.Format("x:{0} y:{1}", doc.Modifier.RealToString(mouse.X), doc.Modifier.RealToString(mouse.Y));
					}
					else
					{
						return " ";
					}
				}
			}
		}

		// Texte pour les informations.
		protected string TextInfoModif
		{
			get
			{
				Document doc = this.CurrentDocument;
				if ( doc == null )
				{
					return " ";
				}
				else
				{
					if ( doc.Modifier.TextInfoModif == "" )  return " ";
					return doc.Modifier.TextInfoModif;
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
					return " ";
				}
				else
				{
					DrawingContext context = doc.Modifier.ActiveViewer.DrawingContext;
					double zoom = context.Zoom;
					return string.Format("Zoom {0}%", (zoom*100).ToString("F0"));
				}
			}
		}


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
		public bool IsCurrentDocument
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
		protected void CreateDocument()
		{
			this.PrepareCloseDocument();

			Document doc = new Document(this.type, DocumentMode.Modify, this.installType, this.globalSettings, this.CommandDispatcher);
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
			this.closeAllState.Enabled = (this.bookDocuments.PageCount > 0);
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
			this.dlgSettings.Rebuild();
			this.dlgInfos.Rebuild();
			this.dlgExport.Rebuild();
		}

		// Secoue un CommandState pour le forcer à se remettre à jour.
		protected void CommandStateShake(CommandState state)
		{
			state.Enabled = !state.Enabled;
			state.Enabled = !state.Enabled;
		}
		#endregion


		#region GlobalSettings
		// Lit le fichier des réglages de l'application.
		protected bool ReadGlobalSettings()
		{
			try
			{
				using ( Stream stream = File.OpenRead(this.GlobalSettingsFilename) )
				{
					// Initialise la variable statique utilisée par VersionDeserializationBinder.
					// Exemple de contenu:
					// "Common.Document, Version=1.0.1777.18519, Culture=neutral, PublicKeyToken=7344997cc606b490"
					System.Reflection.Assembly ass = System.Reflection.Assembly.GetAssembly(typeof(GlobalSettings));
					DocumentEditor.AssemblyFullName = ass.FullName;

					SoapFormatter formatter = new SoapFormatter();
					formatter.Binder = new VersionDeserializationBinder();

					try
					{
						this.globalSettings = (GlobalSettings) formatter.Deserialize(stream);
					}
					catch
					{
						return false;
					}
				}
			}
			catch
			{
				return false;
			}
			return true;
		}

		sealed class VersionDeserializationBinder : SerializationBinder 
		{
			// Retourne un type correspondant à l'application courante, afin
			// d'accepter de désérialiser un fichier généré par une application
			// ayant un autre numéro de révision.
			// Application courante: Version=1.0.1777.18519
			// Version dans le fichier: Version=1.0.1777.11504
			public override System.Type BindToType(string assemblyName, string typeName) 
			{
				System.Type typeToDeserialize;
				typeToDeserialize = System.Type.GetType(string.Format("{0}, {1}", typeName, DocumentEditor.AssemblyFullName));
				return typeToDeserialize;
			}
		}

		protected static string AssemblyFullName = "";

		// Ecrit le fichier des réglages de l'application.
		protected bool WritedGlobalSettings()
		{
			this.globalSettings.IsFullScreen = this.Window.IsFullScreen;
			this.globalSettings.WindowLocation = this.Window.WindowPlacementNormalBounds.Location;
			this.globalSettings.WindowSize = this.Window.WindowPlacementNormalBounds.Size;

			this.dlgAbout.Save();
			this.dlgInfos.Save();
			this.dlgExport.Save();
			this.dlgSettings.Save();

			this.globalSettings.Adorner = Epsitec.Common.Widgets.Adorner.Factory.ActiveName;

			try
			{
				using ( Stream stream = File.OpenWrite(this.GlobalSettingsFilename) )
				{
					SoapFormatter formatter = new SoapFormatter();
					formatter.Serialize(stream, this.globalSettings);
				}
			}
			catch
			{
				return false;
			}
			return true;
		}

		// Retourne le nom du fichier des réglages de l'application.
		// Le dossier est qq chose du genre:
		// C:\Documents and Settings\Daniel Roux\Application Data\Epsitec\Crésus documents\1.0.0.0
		protected string GlobalSettingsFilename
		{
			get
			{
				string dir = Common.Support.Globals.Directories.UserAppData;
				int i = dir.LastIndexOf("\\");
				if ( i > 0 )
				{
					dir = dir.Substring(0, i);
				}

				if ( this.type == DocumentType.Pictogram )
				{
					return string.Format("{0}\\{1}", dir, "CresusPicto.data");
				}
				else
				{
					return string.Format("{0}\\{1}", dir, "CresusDoc.data");
				}
			}
		}
		#endregion


		protected DocumentType					type;
		protected InstallType					installType;
		protected bool							useArray;
		protected bool							firstInitialise;
		protected Document						clipboard;
		protected int							currentDocument;
		protected System.Collections.ArrayList	documents;
		protected GlobalSettings				globalSettings;
		protected bool							askKey = false;

		protected CommandDispatcher				commandDispatcher;

		protected HMenu							menu;
		protected VMenu							fileMenu;
		protected HToolBar						hToolBar;
		protected VToolBar						vToolBar;
		protected StatusBar						info;
		protected TabBook						bookDocuments;
		protected double						panelsWidth = 247;
		protected bool							ignoreChange;
		protected int							tabIndex;

		protected Dialogs.Splash				dlgSplash;
		protected Dialogs.Key					dlgKey;
		protected Dialogs.Update				dlgUpdate;
		protected Dialogs.About					dlgAbout;
		protected Dialogs.Infos					dlgInfos;
		protected Dialogs.Export				dlgExport;
		protected Dialogs.Settings				dlgSettings;

		protected CommandState					newState;
		protected CommandState					openState;
		protected CommandState					saveState;
		protected CommandState					saveAsState;
		protected CommandState					closeState;
		protected CommandState					closeAllState;
		protected CommandState					printState;
		protected CommandState					exportState;
		protected CommandState					deleteState;
		protected CommandState					duplicateState;
		protected CommandState					cutState;
		protected CommandState					copyState;
		protected CommandState					pasteState;
		protected CommandState					textBoldState;
		protected CommandState					textItalicState;
		protected CommandState					textUnderlinedState;
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
		protected CommandState					combineState;
		protected CommandState					uncombineState;
		protected CommandState					toBezierState;
		protected CommandState					toPolyState;
		protected CommandState					fragmentState;
		protected CommandState					booleanAndState;
		protected CommandState					booleanOrState;
		protected CommandState					booleanXorState;
		protected CommandState					booleanFrontMinusState;
		protected CommandState					booleanBackMinusState;
		protected CommandState					undoState;
		protected CommandState					redoState;
		protected CommandState					deselectState;
		protected CommandState					selectAllState;
		protected CommandState					selectInvertState;
		protected CommandState					selectorAutoState;
		protected CommandState					selectorIndividualState;
		protected CommandState					selectorZoomState;
		protected CommandState					selectorStretchState;
		protected CommandState					selectTotalState;
		protected CommandState					selectPartialState;
		protected CommandState					hideHalfState;
		protected CommandState					hideSelState;
		protected CommandState					hideRestState;
		protected CommandState					hideCancelState;
		protected CommandState					zoomMinState;
		protected CommandState					zoomPageState;
		protected CommandState					zoomPageWidthState;
		protected CommandState					zoomDefaultState;
		protected CommandState					zoomSelState;
		protected CommandState					zoomSelWidthState;
		protected CommandState					zoomPrevState;
		protected CommandState					zoomSubState;
		protected CommandState					zoomAddState;
		protected CommandState					previewState;
		protected CommandState					gridState;
		protected CommandState					rulersState;
		protected CommandState					labelsState;
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
		protected CommandState					updateState;
		protected CommandState					keyState;
		protected CommandState					moveLeftNormState;
		protected CommandState					moveRightNormState;
		protected CommandState					moveUpNormState;
		protected CommandState					moveDownNormState;
		protected CommandState					moveLeftCtrlState;
		protected CommandState					moveRightCtrlState;
		protected CommandState					moveUpCtrlState;
		protected CommandState					moveDownCtrlState;
		protected CommandState					moveLeftShiftState;
		protected CommandState					moveRightShiftState;
		protected CommandState					moveUpShiftState;
		protected CommandState					moveDownShiftState;


		protected class DocumentInfo
		{
			public Document						document;
			public TabPage						tabPage;
			public HRuler						hRuler;
			public VRuler						vRuler;
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
				if ( this.hRuler != null )  this.hRuler.Dispose();
				if ( this.vRuler != null )  this.vRuler.Dispose();
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
