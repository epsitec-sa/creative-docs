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
	using DocWidgets     = Common.Document.Widgets;
	using Ribbons        = Common.Document.Ribbons;
	using Containers     = Common.Document.Containers;
	using Objects        = Common.Document.Objects;
	using Settings       = Common.Document.Settings;
	using GlobalSettings = Common.Document.Settings.GlobalSettings;
	using Menus          = Common.Document.Menus;

	/// <summary>
	/// La classe DocumentEditor représente l'éditeur de document complet.
	/// </summary>
	[SuppressBundleSupport]
	public class DocumentEditor : Widgets.Widget
	{
		public DocumentEditor(DocumentType type)
		{
			//	On crée son propre dispatcher, pour éviter de marcher sur les autres commandes.
			
			System.Diagnostics.Debug.WriteLine("*** Created Primary Command Dispatcher ***");
			this.commandDispatcher = new CommandDispatcher("DocumentEditor", CommandDispatcherLevel.Primary);
			this.commandDispatcher.RegisterController(this);
			this.commandDispatcher.Focus();
			this.AttachCommandDispatcher(this.commandDispatcher);
			
			this.type = type;
			this.useArray = false;

			if ( this.type == DocumentType.Pictogram )
			{
				this.installType = InstallType.Full;
			}
			else
			{
				if ( Application.Mode == "F" )
				{
					this.askKey = false;
					this.installType = InstallType.Freeware;
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
			}

#if DEBUG
			this.debugMode = DebugMode.DebugCommands;
#else
			this.debugMode = DebugMode.Release;
#endif
			
			this.debugMode = DebugMode.DebugCommands;	//	commenter cette ligne pour le code de production 1.8.1

			if ( !this.ReadGlobalSettings() )
			{
				this.globalSettings = new GlobalSettings();
				this.globalSettings.Initialise(this.type);
			}

			Epsitec.Common.Widgets.Adorners.Factory.SetActive(this.globalSettings.Adorner);

			this.dlgSplash = new Dialogs.Splash(this);
			
			if ( this.type != DocumentType.Pictogram &&
				 this.globalSettings.SplashScreen )
			{
				this.dlgSplash.Show();
			}
			
			this.dlgAbout     = new Dialogs.About(this);
			this.dlgDownload  = new Dialogs.Download(this);
			this.dlgExport    = new Dialogs.Export(this);
			this.dlgExportPDF = new Dialogs.ExportPDF(this);
			this.dlgGlyphs    = new Dialogs.Glyphs(this);
			this.dlgInfos     = new Dialogs.Infos(this);
			this.dlgKey       = new Dialogs.Key(this);
			this.dlgPageStack = new Dialogs.PageStack(this);
			this.dlgPrint     = new Dialogs.Print(this);
			this.dlgReplace   = new Dialogs.Replace(this);
			this.dlgSettings  = new Dialogs.Settings(this);
			this.dlgUpdate    = new Dialogs.Update(this);

			this.dlgGlyphs.Closed    += new EventHandler(this.HandleDlgClosed);
			this.dlgInfos.Closed     += new EventHandler(this.HandleDlgClosed);
			this.dlgPageStack.Closed += new EventHandler(this.HandleDlgClosed);
			this.dlgReplace.Closed   += new EventHandler(this.HandleDlgClosed);
			this.dlgSettings.Closed  += new EventHandler(this.HandleDlgClosed);

			this.StartCheck(false);
			this.InitCommands();
			this.CreateLayout();

			Misc.GetFontList(false);  // mise en cache des polices
			
			this.dlgSplash.StartTimer();

			this.clipboard = new Document(this.type, DocumentMode.Clipboard, this.installType, this.debugMode, this.globalSettings, this.CommandDispatcher);
			this.clipboard.Name = "Clipboard";

			this.documents = new System.Collections.ArrayList();
			this.currentDocument = -1;

			string[] args = System.Environment.GetCommandLineArgs();
			if ( args.Length >= 2 )
			{
				this.CreateDocument();
				string filename = args[1];
				string err = "";
				if ( Misc.IsExtension(filename, ".crcolors") )
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
					if ( this.globalSettings.NewDocument == "" )
					{
						this.CreateDocument();
					}
					else
					{
						this.Open(this.globalSettings.NewDocument);
						this.CurrentDocument.IsDirtySerialize = false;
					}
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

		private void HandleSizeChanged()
		{
			if ( this.resize == null || this.Window == null )  return;
			this.resize.Enable = !this.Window.IsFullScreen;
		}
		
		protected override void OnSizeChanged(Epsitec.Common.Types.DependencyPropertyChangedEventArgs e)
		{
			base.OnSizeChanged(e);
			this.HandleSizeChanged();
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
		
		public DebugMode DebugMode
		{
			get
			{
				return this.debugMode;
			}

			set
			{
				this.debugMode = value;

				int total = this.documents.Count;
				for ( int i=0 ; i<total ; i++ )
				{
					DocumentInfo di = this.documents[i] as DocumentInfo;
					di.document.DebugMode = this.debugMode;
				}
			}
		}
		
		public GlobalSettings GlobalSettings
		{
			get { return this.globalSettings; }
		}
		
		public CommandDispatcher CommandDispatcher
		{
			get { return this.commandDispatcher; }
		}

#if false //#fix
		static DocumentEditor()
		{
			//	Toute modification de la propriété BackColor doit être répercutée
			//	sur le slider. Le plus simple est d'utiliser un override callback
			//	sur la propriété BackColor :
			
			Epsitec.Common.Types.PropertyMetadata metadata = new Epsitec.Common.Types.PropertyMetadata ();
			
			metadata.GetValueOverride = new Epsitec.Common.Types.GetValueOverrideCallback (DocumentEditor.GetCommandDispatchersValue);
			
			Visual.CommandDispatcherProperty.OverrideMetadata (typeof (DocumentEditor), metadata);
		}
		
		
		private static object GetCommandDispatchersValue(Epsitec.Common.Types.Object o)
		{
			DocumentEditor that = o as DocumentEditor;
			
			if ( that.commandDispatcher == null )
			{
				System.Diagnostics.Debug.WriteLine("*** Created Primary Command Dispatcher ***");
				//	On crée son propre dispatcher, pour éviter de marcher sur les autres commandes.
				that.commandDispatcher = new CommandDispatcher("DocumentEditor", CommandDispatcherLevel.Primary);
				that.commandDispatcher.RegisterController(that);
			}
			
			return that.commandDispatcher;
		}
#endif


		public void MakeReadyToRun()
		{
			//	Appelé lorsque l'application a fini de démarrer.
			if ( this.askKey )
			{
				this.askKey = false;
				this.dlgSplash.Hide();
				this.dlgKey.Show();
			}

			if ( this.IsCurrentDocument )
			{
				this.CurrentDocument.Notifier.NotifyOriginChanged();
			}

			this.EndCheck(false);
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

#if false
			this.menu = new HMenu();
			this.menu.Visibility = false;
			this.menu.Host = this;
			this.menu.Items.Add(new MenuItem("", Res.Strings.Menu.Main.File));
			this.menu.Items.Add(new MenuItem("", Res.Strings.Menu.Main.Edit));
			this.menu.Items.Add(new MenuItem("", Res.Strings.Menu.Main.Objects));
			this.menu.Items.Add(new MenuItem("", Res.Strings.Menu.Main.Show));
			if ( this.useArray )
			{
				this.menu.Items.Add(new MenuItem("", Res.Strings.Menu.Main.Array));
			}
			this.menu.Items.Add(new MenuItem("", Res.Strings.Menu.Main.Document));
#if DEBUG
			this.menu.Items.Add(new MenuItem("", Res.Strings.Menu.Main.Debug));
#endif
			this.menu.Items.Add(new MenuItem("", Res.Strings.Menu.Main.Help));

			int i = 0;

			VMenu fileMenu = new VMenu();
			fileMenu.Name = "File";
			fileMenu.Host = this;
			this.MenuAdd(fileMenu, "New");
			this.MenuAdd(fileMenu, "Open");
			if ( this.type != DocumentType.Pictogram )
			{
				this.MenuAdd(fileMenu, "OpenModel");
			}
			this.MenuAdd(fileMenu, "Close");
			this.MenuAdd(fileMenu, "CloseAll");
			this.MenuAdd(fileMenu, null);
			this.MenuAdd(fileMenu, "Save");
			this.MenuAdd(fileMenu, "SaveAs");
			if ( this.type != DocumentType.Pictogram )
			{
				this.MenuAdd(fileMenu, "SaveModel");
			}
			this.MenuAdd(fileMenu, null);
			this.MenuAdd(fileMenu, "Print");
			this.MenuAdd(fileMenu, "Export");
			this.MenuAdd(fileMenu, null);
			this.MenuAdd(fileMenu, "", "SubmenuLastFiles", DocumentEditor.GetRes("Action.LastFiles"), "");
			this.MenuAdd(fileMenu, null);
			this.MenuAdd(fileMenu, "", "QuitApplication", DocumentEditor.GetRes("Action.Quit"), "");
			fileMenu.AdjustSize();
			this.menu.Items[i++].Submenu = fileMenu;

			this.fileMenu = fileMenu;
			this.BuildLastFilenamesMenu();

			VMenu editMenu = new VMenu();
			editMenu.Name = "Edit";
			editMenu.Host = this;
			this.MenuAdd(editMenu, "Undo");
			this.MenuAdd(editMenu, "Redo");
			this.MenuAdd(editMenu, null);
			this.MenuAdd(editMenu, "Cut");
			this.MenuAdd(editMenu, "Copy");
			this.MenuAdd(editMenu, "Paste");
			this.MenuAdd(editMenu, null);
			this.MenuAdd(editMenu, "Delete");
			this.MenuAdd(editMenu, "Duplicate");
			editMenu.AdjustSize();
			this.menu.Items[i++].Submenu = editMenu;

			VMenu objMenu = new VMenu();
			objMenu.Name = "Obj";
			objMenu.Host = this;
			this.MenuAdd(objMenu, "DeselectAll");
			this.MenuAdd(objMenu, "SelectAll");
			this.MenuAdd(objMenu, "SelectInvert");
			this.MenuAdd(objMenu, null);
			this.MenuAdd(objMenu, "HideSel");
			this.MenuAdd(objMenu, "HideRest");
			this.MenuAdd(objMenu, "HideCancel");
			this.MenuAdd(objMenu, "y/n", "HideHalf", DocumentEditor.GetRes("Action.HideHalf"), "");
			this.MenuAdd(objMenu, null);
			this.MenuAdd(objMenu, Misc.Icon("OrderUpAll"), "SubmenuOrder", DocumentEditor.GetRes("Action.OrderMain"), "");
			this.MenuAdd(objMenu, Misc.Icon("MoveH"), "SubmenuOper", DocumentEditor.GetRes("Action.OperationMain"), "");
			this.MenuAdd(objMenu, Misc.Icon("GroupEmpty"), "SubmenuGroup", DocumentEditor.GetRes("Action.GroupMain"), "");
			this.MenuAdd(objMenu, Misc.Icon("Combine"), "SubmenuGeom", DocumentEditor.GetRes("Action.GeometryMain"), "");
			this.MenuAdd(objMenu, Misc.Icon("BooleanOr"), "SubmenuBool", DocumentEditor.GetRes("Action.BooleanMain"), "");
			objMenu.AdjustSize();
			this.menu.Items[i++].Submenu = objMenu;

			VMenu orderMenu = new VMenu();
			orderMenu.Name = "Order";
			orderMenu.Host = this;
			this.MenuAdd(orderMenu, "OrderUpAll");
			this.MenuAdd(orderMenu, "OrderUpOne");
			this.MenuAdd(orderMenu, "OrderDownOne");
			this.MenuAdd(orderMenu, "OrderDownAll");
			orderMenu.AdjustSize();
			this.MenuAddSub(objMenu, orderMenu, "SubmenuOrder");

			VMenu operMenu = new VMenu();
			operMenu.Name = "Oper";
			operMenu.Host = this;
			this.MenuAdd(operMenu, "Rotate90");
			this.MenuAdd(operMenu, "Rotate180");
			this.MenuAdd(operMenu, "Rotate270");
			this.MenuAdd(operMenu, null);
			this.MenuAdd(operMenu, "MirrorH");
			this.MenuAdd(operMenu, "MirrorV");
			this.MenuAdd(operMenu, null);
			this.MenuAdd(operMenu, "ScaleDiv2");
			this.MenuAdd(operMenu, "ScaleMul2");
			operMenu.AdjustSize();
			this.MenuAddSub(objMenu, operMenu, "SubmenuOper");

			VMenu groupMenu = new VMenu();
			groupMenu.Name = "Group";
			groupMenu.Host = this;
			this.MenuAdd(groupMenu, "Group");
			this.MenuAdd(groupMenu, "Merge");
			this.MenuAdd(groupMenu, "Extract");
			this.MenuAdd(groupMenu, "Ungroup");
			this.MenuAdd(groupMenu, null);
			this.MenuAdd(groupMenu, "Inside");
			this.MenuAdd(groupMenu, "Outside");
			groupMenu.AdjustSize();
			this.MenuAddSub(objMenu, groupMenu, "SubmenuGroup");

			VMenu geomMenu = new VMenu();
			geomMenu.Name = "Geom";
			geomMenu.Host = this;
			this.MenuAdd(geomMenu, "Combine");
			this.MenuAdd(geomMenu, "Uncombine");
			this.MenuAdd(geomMenu, null);
			this.MenuAdd(geomMenu, "ToBezier");
			this.MenuAdd(geomMenu, "ToPoly");
			this.MenuAdd(geomMenu, null);
			this.MenuAdd(geomMenu, "Fragment");
#if DEBUG
			this.MenuAdd(geomMenu, "", "ToSimplest", DocumentEditor.GetRes("Action.ToSimplest"), "");
#endif
			geomMenu.AdjustSize();
			this.MenuAddSub(objMenu, geomMenu, "SubmenuGeom");

			VMenu boolMenu = new VMenu();
			boolMenu.Name = "Bool";
			boolMenu.Host = this;
			this.MenuAdd(boolMenu, "BooleanOr");
			this.MenuAdd(boolMenu, "BooleanAnd");
			this.MenuAdd(boolMenu, "BooleanXor");
			this.MenuAdd(boolMenu, "BooleanFrontMinus");
			this.MenuAdd(boolMenu, "BooleanBackMinus");
			boolMenu.AdjustSize();
			this.MenuAddSub(objMenu, boolMenu, "SubmenuBool");

			VMenu showMenu = new VMenu();
			showMenu.Name = "Show";
			showMenu.Host = this;
			this.MenuAdd(showMenu, "Preview");
			this.MenuAdd(showMenu, "Grid");
			this.MenuAdd(showMenu, "Magnet");
			if ( this.type != DocumentType.Pictogram )
			{
				this.MenuAdd(showMenu, "Rulers");
				this.MenuAdd(showMenu, "Labels");
				this.MenuAdd(showMenu, "Aggregates");
			}
			this.MenuAdd(showMenu, null);
			this.MenuAdd(showMenu, "ZoomMin");
			if ( this.type != DocumentType.Pictogram )
			{
				this.MenuAdd(showMenu, "ZoomPage");
				this.MenuAdd(showMenu, "ZoomPageWidth");
			}
			this.MenuAdd(showMenu, "ZoomDefault");
			this.MenuAdd(showMenu, "ZoomSel");
			if ( this.type != DocumentType.Pictogram )
			{
				this.MenuAdd(showMenu, "ZoomSelWidth");
			}
			this.MenuAdd(showMenu, "ZoomPrev");
			this.MenuAdd(showMenu, "ZoomSub");
			this.MenuAdd(showMenu, "ZoomAdd");
			showMenu.AdjustSize();
			this.menu.Items[i++].Submenu = showMenu;

#if false
			if ( this.useArray )
			{
				VMenu arrayMenu = new VMenu();
				arrayMenu.Name = "Array";
				arrayMenu.Host = this;
				this.MenuAdd(arrayMenu, "ArrayFrame"), ";
				this.MenuAdd(arrayMenu, "ArrayHoriz"), ";
				this.MenuAdd(arrayMenu, "ArrayVerti"), ";
				this.MenuAdd(arrayMenu, null);
				this.MenuAdd(arrayMenu, "", "", "Assistants", "");
				this.MenuAdd(arrayMenu, null);
				this.MenuAdd(arrayMenu, "", "ArrayAddColumnLeft", "Insérer des colonnes à gauche", "");
				this.MenuAdd(arrayMenu, "", "ArrayAddColumnRight", "Insérer des colonnes à droite", "");
				this.MenuAdd(arrayMenu, "", "ArrayAddRowTop", "Insérer des lignes en dessus", "");
				this.MenuAdd(arrayMenu, "", "ArrayAddRowBottom", "Insérer des lignes en dessous", "");
				this.MenuAdd(arrayMenu, null);
				this.MenuAdd(arrayMenu, "", "ArrayDelColumn", "Supprimer les colonnes", "");
				this.MenuAdd(arrayMenu, "", "ArrayDelRow", "Supprimer les lignes", "");
				this.MenuAdd(arrayMenu, null);
				this.MenuAdd(arrayMenu, "", "ArrayAlignColumn", "Egaliser les largeurs de colonne", "");
				this.MenuAdd(arrayMenu, "", "ArrayAlignRow", "Egaliser les hauteurs de ligne", "");
				this.MenuAdd(arrayMenu, null);
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
						this.MenuAdd(arrayLookMenu, null);
					}
					else
					{
						this.MenuAdd(arrayLookMenu, "", "ArrayLook(this.Name)", text, "", name);
					}
				}
				arrayLookMenu.AdjustSize();
				arrayMenu.Items[4].Submenu = arrayLookMenu;
			}
#endif

			VMenu docMenu = new VMenu();
			docMenu.Name = "Document";
			docMenu.Host = this;
			this.MenuAdd(docMenu, "Settings");
			this.MenuAdd(docMenu, "Infos");
			this.MenuAdd(docMenu, "PageStack");
			this.MenuAdd(docMenu, "Glyphs");
			this.MenuAdd(docMenu, null);
			this.MenuAdd(docMenu, "PageNew");
			this.MenuAdd(docMenu, "Up");
			this.MenuAdd(docMenu, "Down");
			this.MenuAdd(docMenu, "DeleteItem");
			this.MenuAdd(docMenu, null);
			this.MenuAdd(docMenu, "LayerNew");
			this.MenuAdd(docMenu, "Up");
			this.MenuAdd(docMenu, "Down");
			this.MenuAdd(docMenu, "DeleteItem");
			docMenu.AdjustSize();
			this.menu.Items[i++].Submenu = docMenu;

#if DEBUG
			VMenu debugMenu = new VMenu();
			debugMenu.Name = "Debug";
			debugMenu.Host = this;
			this.MenuAdd(debugMenu, "y/n", "DebugBboxThin", "Show BBoxThin", Misc.GetShortCut(this.debugBboxThinState));
			this.MenuAdd(debugMenu, "y/n", "DebugBboxGeom", "Show BBoxGeom", Misc.GetShortCut(this.debugBboxGeomState));
			this.MenuAdd(debugMenu, "y/n", "DebugBboxFull", "Show BBoxFull", Misc.GetShortCut(this.debugBboxFullState));
			this.MenuAdd(debugMenu, null);
			this.MenuAdd(debugMenu, "", "DebugDirty", "Make dirty", Misc.GetShortCut(this.debugDirtyState));
			this.MenuAdd(debugMenu, null);
			this.MenuAdd(debugMenu, "SelectTotal");
			this.MenuAdd(debugMenu, "SelectPartial");
			this.MenuAdd(debugMenu, null);
			this.MenuAdd(debugMenu, "", "ForceSaveAll", "Save and overwrite all", Misc.GetShortCut(this.forceSaveAllState));
			debugMenu.AdjustSize();
			this.menu.Items[i++].Submenu = debugMenu;
#endif

			VMenu helpMenu = new VMenu();
			helpMenu.Name = "Help";
			helpMenu.Host = this;
			if ( this.installType != InstallType.Freeware )
			{
				this.MenuAdd(helpMenu, "Key");
			}
			this.MenuAdd(helpMenu, "Update");
			this.MenuAdd(helpMenu, null);
			this.MenuAdd(helpMenu, "About");
			helpMenu.AdjustSize();
			this.menu.Items[i++].Submenu = helpMenu;
#endif

			this.hToolBar = new HToolBar(this);
			this.hToolBar.Anchor = AnchorStyles.LeftAndRight | AnchorStyles.Top;
#if false
			this.HToolBarAdd("New");
			this.HToolBarAdd("Open");
			this.HToolBarAdd("Save");
			this.HToolBarAdd("SaveAs");
			this.HToolBarAdd("Print");
			this.HToolBarAdd("Export");
			this.HToolBarAdd(null);
			this.HToolBarAdd("Delete");
			this.HToolBarAdd("Duplicate");
			this.HToolBarAdd(null);
			this.HToolBarAdd("Cut");
			this.HToolBarAdd("Copy");
			this.HToolBarAdd("Paste");
			this.HToolBarAdd(null);

			this.HToolBarAdd("Undo");

			GlyphButton undoRedoList = new GlyphButton("UndoRedoList");
			undoRedoList.Name = "UndoRedoList";
			undoRedoList.GlyphShape = GlyphShape.ArrowDown;
			undoRedoList.ButtonStyle = ButtonStyle.ToolItem;
			undoRedoList.Width = 14;
			undoRedoList.Margins = new Margins(-1, 0, 0, 0);
			ToolTip.Default.SetToolTip(undoRedoList, DocumentEditor.GetRes("Action.UndoRedoList"));
			this.hToolBar.Items.Add(undoRedoList);

			Widget buttonRedo = this.HToolBarAdd("Redo");
			buttonRedo.Margins = new Margins(-1, 0, 0, 0);

			this.HToolBarAdd(null);

			this.HToolBarAdd("OrderDownAll");
			this.HToolBarAdd("OrderDownOne");
			this.HToolBarAdd("OrderUpOne");
			this.HToolBarAdd("OrderUpAll");
			this.HToolBarAdd(null);
			this.HToolBarAdd("Group");
			this.HToolBarAdd("Merge");
			this.HToolBarAdd("Extract");
			this.HToolBarAdd("Ungroup");
			this.HToolBarAdd("Inside");
			this.HToolBarAdd("Outside");
			this.HToolBarAdd(null);
			this.HToolBarAdd("Preview");
			this.HToolBarAdd("Grid");
			this.HToolBarAdd("Magnet");
			this.HToolBarAdd("Labels");
			this.HToolBarAdd("Aggregates");
			this.HToolBarAdd("Settings");
			this.HToolBarAdd("Infos");
			this.HToolBarAdd("Glyphs");
			this.HToolBarAdd(null);
			if ( this.useArray )
			{
				this.HToolBarAdd("ArrayFrame");
				this.HToolBarAdd("ArrayHoriz");
				this.HToolBarAdd("ArrayVerti");
				this.HToolBarAdd(null);
			}
#endif

			this.ribbonMainButton = new RibbonButton("", Res.Strings.Ribbon.Main);
			this.ribbonMainButton.Size = this.ribbonMainButton.RequiredSize;
			this.ribbonMainButton.Pressed += new MessageEventHandler (this.HandleRibbonPressed);
			this.hToolBar.Items.Add(this.ribbonMainButton);

			this.ribbonGeomButton = new RibbonButton("", Res.Strings.Ribbon.Geom);
			this.ribbonGeomButton.Size = this.ribbonGeomButton.RequiredSize;
			this.ribbonGeomButton.Pressed += new MessageEventHandler (this.HandleRibbonPressed);
			this.hToolBar.Items.Add(this.ribbonGeomButton);

			this.ribbonOperButton = new RibbonButton("", Res.Strings.Ribbon.Oper);
			this.ribbonOperButton.Size = this.ribbonOperButton.RequiredSize;
			this.ribbonOperButton.Pressed += new MessageEventHandler (this.HandleRibbonPressed);
			this.hToolBar.Items.Add(this.ribbonOperButton);

			this.ribbonTextButton = new RibbonButton("", Res.Strings.Ribbon.Text);
			this.ribbonTextButton.Size = this.ribbonTextButton.RequiredSize;
			this.ribbonTextButton.Pressed += new MessageEventHandler (this.HandleRibbonPressed);
			this.hToolBar.Items.Add(this.ribbonTextButton);

			this.UpdateQuickCommands();

			this.ribbonMain = new Ribbons.RibbonContainer(this);
			this.ribbonMain.Name = "Main";
			this.ribbonMain.Height = this.ribbonHeight;
			this.ribbonMain.Anchor = AnchorStyles.LeftAndRight | AnchorStyles.Top;
			this.ribbonMain.Margins = new Margins(0, 0, this.hToolBar.Height, 0);
			this.ribbonMain.Visibility = true;
			this.ribbonMain.Items.Add(new Ribbons.File());
			this.ribbonMain.Items.Add(new Ribbons.Clipboard());
			this.ribbonMain.Items.Add(new Ribbons.Undo());
			this.ribbonMain.Items.Add(new Ribbons.Select());
			this.ribbonMain.Items.Add(new Ribbons.View());
			this.ribbonMain.Items.Add(new Ribbons.Action());
			if ( this.debugMode == DebugMode.DebugCommands )
			{
				this.ribbonMain.Items.Add(new Ribbons.Debug());
			}

			this.ribbonGeom = new Ribbons.RibbonContainer(this);
			this.ribbonGeom.Name = "Geom";
			this.ribbonGeom.Height = this.ribbonHeight;
			this.ribbonGeom.Anchor = AnchorStyles.LeftAndRight | AnchorStyles.Top;
			this.ribbonGeom.Margins = new Margins(0, 0, this.hToolBar.Height, 0);
			this.ribbonGeom.Visibility = false;
			this.ribbonGeom.Items.Add(new Ribbons.Move());
			this.ribbonGeom.Items.Add(new Ribbons.Rotate());
			this.ribbonGeom.Items.Add(new Ribbons.Scale());
			this.ribbonGeom.Items.Add(new Ribbons.Align());
			this.ribbonGeom.Items.Add(new Ribbons.Bool());
			this.ribbonGeom.Items.Add(new Ribbons.Geom());

			this.ribbonOper = new Ribbons.RibbonContainer(this);
			this.ribbonOper.Name = "Oper";
			this.ribbonOper.Height = this.ribbonHeight;
			this.ribbonOper.Anchor = AnchorStyles.LeftAndRight | AnchorStyles.Top;
			this.ribbonOper.Margins = new Margins(0, 0, this.hToolBar.Height, 0);
			this.ribbonOper.Visibility = false;
			this.ribbonOper.Items.Add(new Ribbons.Order());
			this.ribbonOper.Items.Add(new Ribbons.Group());
			this.ribbonOper.Items.Add(new Ribbons.Color());

			this.ribbonText = new Ribbons.RibbonContainer(this);
			this.ribbonText.Name = "Text";
			this.ribbonText.Height = this.ribbonHeight;
			this.ribbonText.Anchor = AnchorStyles.LeftAndRight | AnchorStyles.Top;
			this.ribbonText.Margins = new Margins(0, 0, this.hToolBar.Height, 0);
			this.ribbonText.Visibility = false;
			this.ribbonText.Items.Add(new Ribbons.TextStyles());
			this.ribbonText.Items.Add(new Ribbons.Paragraph());
			this.ribbonText.Items.Add(new Ribbons.Font());
			this.ribbonText.Items.Add(new Ribbons.Clipboard());
			this.ribbonText.Items.Add(new Ribbons.Undo());
			this.ribbonText.Items.Add(new Ribbons.Replace());
			this.ribbonText.Items.Add(new Ribbons.Insert());

			this.ribbonActive = this.ribbonMain;

			this.info = new StatusBar(this);
			this.info.Anchor = AnchorStyles.LeftAndRight | AnchorStyles.Bottom;
			this.info.Margins = new Margins(0, 0, 0, 0);

			this.InfoAdd("DeselectAll");
			this.InfoAdd("SelectAll");
			this.InfoAdd("SelectInvert");
			this.InfoAdd("HideSel");
			this.InfoAdd("HideRest");
			this.InfoAdd("HideCancel");

			this.InfoAdd("StatusObject", 120);

			//?this.InfoAdd("ZoomMin");
			if ( this.type != DocumentType.Pictogram )
			{
				this.InfoAdd("ZoomPage");
				this.InfoAdd("ZoomPageWidth");
			}
			this.InfoAdd("ZoomDefault");
			this.InfoAdd("ZoomSel");
			if ( this.type != DocumentType.Pictogram )
			{
				this.InfoAdd("ZoomSelWidth");
			}
			this.InfoAdd("ZoomPrev");

			StatusField sf = this.InfoAdd("StatusZoom", 55);
			sf.Clicked += new MessageEventHandler(this.HandleStatusZoomClicked);
			ToolTip.Default.SetToolTip(sf, Res.Strings.Status.Zoom.Menu);

			Widgets.HSlider slider = new HSlider();
			slider.Name = "StatusZoomSlider";
			slider.Width = 100;
			slider.Margins = new Margins(1, 1, 1, 1);
			slider.MinValue = 0.1M;
			slider.MaxValue = 16.0M;
			slider.SmallChange = 0.1M;
			slider.LargeChange = 0.1M;
			slider.Resolution = 0.0M;
			slider.Logarithmic = 3.0M;
			slider.ValueChanged += new EventHandler(this.HandleSliderZoomChanged);
			this.info.Items.Add(slider);
			ToolTip.Default.SetToolTip(slider, Res.Strings.Status.Zoom.Slider);

			this.InfoAdd("StatusMouse", 110);
			this.InfoAdd("StatusModif", 300);

			this.info.Items["StatusModif"].Dock = DockStyle.Fill;
			
			this.resize = new ResizeKnob();
			this.resize.Margins = new Margins (2, 0, 0, 0);
			this.info.Items.Add (this.resize);
			this.resize.Dock = DockStyle.Right;
			ToolTip.Default.SetToolTip(this.resize, Res.Strings.Dialog.Tooltip.Resize);

			this.vToolBar = new VToolBar(this);
			this.vToolBar.Anchor = AnchorStyles.TopAndBottom | AnchorStyles.Left;
			this.vToolBar.Margins = new Margins(0, 0, this.hToolBar.Height+this.RibbonHeight, this.info.Height);
			this.VToolBarAdd(this.toolSelectState);
			this.VToolBarAdd(this.toolGlobalState);
			this.VToolBarAdd(this.toolShaperState);
			this.VToolBarAdd(this.toolEditState);
			this.VToolBarAdd(this.toolZoomState);
			this.VToolBarAdd(this.toolHandState);
			this.VToolBarAdd(this.toolPickerState);
			if ( this.type == DocumentType.Pictogram )
			{
				this.VToolBarAdd(this.toolHotSpotState);
			}
			this.VToolBarAdd(null);
			this.VToolBarAdd(this.toolLineState);
			this.VToolBarAdd(this.toolRectangleState);
			this.VToolBarAdd(this.toolCircleState);
			this.VToolBarAdd(this.toolEllipseState);
			this.VToolBarAdd(this.toolPolyState);
			this.VToolBarAdd(this.toolBezierState);
			this.VToolBarAdd(this.toolRegularState);
			this.VToolBarAdd(this.toolSurfaceState);
			this.VToolBarAdd(this.toolVolumeState);
			//this.VToolBarAdd(this.toolTextLineState);
			//this.VToolBarAdd(this.toolTextBoxState);
			this.VToolBarAdd(this.toolTextLine2State);
			this.VToolBarAdd(this.toolTextBox2State);
			if ( this.useArray )
			{
				this.VToolBarAdd(this.toolArrayState);
			}
			this.VToolBarAdd(this.toolImageState);
			if ( this.type != DocumentType.Pictogram )
			{
				this.VToolBarAdd(this.toolDimensionState);
			}
			this.VToolBarAdd(null);

			this.bookDocuments = new TabBook(this);
			this.bookDocuments.Width = this.panelsWidth;
			this.bookDocuments.Anchor = AnchorStyles.All;
			this.bookDocuments.Margins = new Margins(this.vToolBar.Width+this.RibbonHeight+1, this.panelsWidth+2, this.hToolBar.Height+1, this.info.Height+1);
			this.bookDocuments.Arrows = TabBookArrows.Right;
			this.bookDocuments.HasCloseButton = true;
			this.bookDocuments.CloseButton.Command = "Close";
			this.bookDocuments.ActivePageChanged += new EventHandler(this.HandleBookDocumentsActivePageChanged);
			ToolTip.Default.SetToolTip(this.bookDocuments.CloseButton, Res.Strings.Tooltip.TabBook.Close);

			this.ActiveRibbon(this.ribbonMain);
		}

		public void UpdateQuickCommands()
		{
			//	Met à jour toutes les icônes de la partie rapide, à droite des choix du ruban actif.
			//	Supprime tous les IconButtons.
			foreach ( Widget widget in this.hToolBar.Children.Widgets )
			{
				if ( widget is RibbonButton )  continue;
				widget.Dispose();
			}

			bool first = true;
			foreach ( string xcmd in this.globalSettings.QuickCommands )
			{
				bool   used = GlobalSettings.QuickUsed(xcmd);
				bool   sep  = GlobalSettings.QuickSep(xcmd);
				string cmd  = GlobalSettings.QuickCmd(xcmd);

				if ( used )
				{
					if ( first )  // première icône ?
					{
						this.HToolBarAdd(null);  // séparateur au début
						first = false;
					}

					CommandState cs = this.commandDispatcher.GetCommandState(cmd);
					this.HToolBarAdd(cs);

					if ( sep )
					{
						this.HToolBarAdd(null);  // séparateur après l'icône
					}
				}
			}
		}

		protected void CreateDocumentLayout(Document document)
		{
			DocumentInfo di = this.CurrentDocumentInfo;
			double sw = 17;  // largeur d'un ascenseur
			double sr = 13;  // largeur d'une règle
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
				viewer.SetParent(mainViewParent);
				viewer.Anchor = AnchorStyles.All;
				viewer.Margins = new Margins(wm, wm+sw+1, 6+wm, wm+sw+1);
				document.Modifier.ActiveViewer = viewer;
				document.Modifier.AttachViewer(viewer);

#if false
				Viewer frame1 = new Viewer(document);
				frame1.SetParent(rightPane);
				frame1.Anchor = AnchorStyles.LeftAndRight | AnchorStyles.Top;
				frame1.Margins = new Margins(wm, wm, 6+wm, wm);
				frame1.DrawingContext.LayerDrawingMode = LayerDrawingMode.ShowInactive;
				document.Modifier.AttachViewer(frame1);

				Viewer frame2 = new Viewer(document);
				frame2.SetParent(rightPane);
				frame2.Anchor = AnchorStyles.LeftAndRight | AnchorStyles.Top;
				frame2.Margins = new Margins(wm, wm, 6+wm+30, wm);
				frame2.DrawingContext.LayerDrawingMode = LayerDrawingMode.ShowInactive;
				document.Modifier.AttachViewer(frame2);
#endif
			}
			else
			{
				lm = sr-1;
				tm = sr-1;

				mainViewParent = di.tabPage;
				Viewer viewer = new Viewer(document);
				viewer.SetParent(mainViewParent);
				viewer.Anchor = AnchorStyles.All;
				viewer.Margins = new Margins(wm+lm, wm+sw+1, 6+wm+tm, wm+sw+1);
				document.Modifier.ActiveViewer = viewer;
				document.Modifier.AttachViewer(viewer);

				di.hRuler = new DocWidgets.HRuler(mainViewParent);
				di.hRuler.Document = document;
				di.hRuler.Anchor = AnchorStyles.LeftAndRight | AnchorStyles.Top;
				di.hRuler.Margins = new Margins(wm+lm, wm+sw+1, 6+wm, 0);
				ToolTip.Default.SetToolTip(di.hRuler, "*");

				di.vRuler = new DocWidgets.VRuler(mainViewParent);
				di.vRuler.Document = document;
				di.vRuler.Anchor = AnchorStyles.TopAndBottom | AnchorStyles.Left;
				di.vRuler.Margins = new Margins(wm, 0, 6+wm+tm, wm+sw+1);
				ToolTip.Default.SetToolTip(di.vRuler, "*");
			}

			//	Bande horizontale qui contient les boutons des pages et l'ascenseur.
			Widget hBand = new Widget(mainViewParent);
			hBand.Height = sw;
			hBand.Anchor = AnchorStyles.LeftAndRight | AnchorStyles.Bottom;
			hBand.Margins = new Margins(wm+lm, wm+sw+1, 0, wm);

			GlyphButton quickPagePrev = new GlyphButton("PagePrev");
			quickPagePrev.SetParent(hBand);
			quickPagePrev.GlyphShape = GlyphShape.ArrowLeft;
			quickPagePrev.Width = sw;
			quickPagePrev.Height = sw;
			quickPagePrev.Dock = DockStyle.Left;
			ToolTip.Default.SetToolTip(quickPagePrev, DocumentEditor.GetRes("Action.PagePrev"));

			di.quickPageMenu = new Button(hBand);
			di.quickPageMenu.Command = "PageMenu";
			di.quickPageMenu.Clicked += new MessageEventHandler(this.HandleQuickPageMenu);
			di.quickPageMenu.Width = System.Math.Floor(sw*2.0);
			di.quickPageMenu.Height = sw;
			di.quickPageMenu.Dock = DockStyle.Left;
			ToolTip.Default.SetToolTip(di.quickPageMenu, DocumentEditor.GetRes("Action.PageMenu"));

			GlyphButton quickPageNext = new GlyphButton("PageNext");
			quickPageNext.SetParent(hBand);
			quickPageNext.GlyphShape = GlyphShape.ArrowRight;
			quickPageNext.Width = sw;
			quickPageNext.Height = sw;
			quickPageNext.Dock = DockStyle.Left;
			ToolTip.Default.SetToolTip(quickPageNext, DocumentEditor.GetRes("Action.PageNext"));

			Button quickPageNew = new Button(hBand);
			quickPageNew.Command = "PageNew";
			quickPageNew.Text = "<b>+</b>";
			quickPageNew.Width = sw;
			quickPageNew.Height = sw;
			quickPageNew.Dock = DockStyle.Left;
			quickPageNew.Margins = new Margins(2, 0, 0, 0);
			ToolTip.Default.SetToolTip(quickPageNew, DocumentEditor.GetRes("Action.PageNew"));

			di.hScroller = new HScroller(hBand);
			di.hScroller.ValueChanged += new EventHandler(this.HandleHScrollerValueChanged);
			di.hScroller.Dock = DockStyle.Fill;
			di.hScroller.Margins = new Margins(2, 0, 0, 0);

			//	Bande verticale qui contient les boutons des calques et l'ascenseur.
			Widget vBand = new Widget(mainViewParent);
			vBand.Width = sw;
			vBand.Anchor = AnchorStyles.TopAndBottom | AnchorStyles.Right;
			vBand.Margins = new Margins(0, wm, 6+wm+tm, wm+sw+1);

			Button quickLayerNew = new Button(vBand);
			quickLayerNew.Command = "LayerNew";
			quickLayerNew.Text = "<b>+</b>";
			quickLayerNew.Width = sw;
			quickLayerNew.Height = sw;
			quickLayerNew.Dock = DockStyle.Top;
			quickLayerNew.Margins = new Margins(0, 0, 0, 2);
			ToolTip.Default.SetToolTip(quickLayerNew, DocumentEditor.GetRes("Action.LayerNew"));

			GlyphButton quickLayerNext = new GlyphButton("LayerNext");
			quickLayerNext.SetParent(vBand);
			quickLayerNext.GlyphShape = GlyphShape.ArrowUp;
			quickLayerNext.Width = sw;
			quickLayerNext.Height = sw;
			quickLayerNext.Dock = DockStyle.Top;
			ToolTip.Default.SetToolTip(quickLayerNext, DocumentEditor.GetRes("Action.LayerNext"));

			di.quickLayerMenu = new Button(vBand);
			di.quickLayerMenu.Command = "LayerMenu";
			di.quickLayerMenu.Clicked += new MessageEventHandler(this.HandleQuickLayerMenu);
			di.quickLayerMenu.Width = sw;
			di.quickLayerMenu.Height = sw;
			di.quickLayerMenu.Dock = DockStyle.Top;
			ToolTip.Default.SetToolTip(di.quickLayerMenu, DocumentEditor.GetRes("Action.LayerMenu"));

			GlyphButton quickLayerPrev = new GlyphButton("LayerPrev");
			quickLayerPrev.SetParent(vBand);
			quickLayerPrev.GlyphShape = GlyphShape.ArrowDown;
			quickLayerPrev.Width = sw;
			quickLayerPrev.Height = sw;
			quickLayerPrev.Dock = DockStyle.Top;
			ToolTip.Default.SetToolTip(quickLayerPrev, DocumentEditor.GetRes("Action.LayerPrev"));

			di.vScroller = new VScroller(vBand);
			di.vScroller.ValueChanged += new EventHandler(this.HandleVScrollerValueChanged);
			di.vScroller.Dock = DockStyle.Fill;
			di.vScroller.Margins = new Margins(0, 0, 2, 0);

			di.bookPanels = new TabBook(this);
			di.bookPanels.Width = this.panelsWidth;
			di.bookPanels.Anchor = AnchorStyles.TopAndBottom | AnchorStyles.Right;
			di.bookPanels.Margins = new Margins(1, 1, this.hToolBar.Height+this.RibbonHeight+1, this.info.Height+1);
			di.bookPanels.Arrows = TabBookArrows.Stretch;
			di.bookPanels.ActivePageChanged += new EventHandler(this.HandleBookPanelsActivePageChanged);

			TabPage bookPrincipal = new TabPage();
			bookPrincipal.TabTitle = Res.Strings.TabPage.Principal;
			bookPrincipal.Name = "Principal";
			di.bookPanels.Items.Add(bookPrincipal);

			TabPage bookStyles = new TabPage();
			bookStyles.TabTitle = Res.Strings.TabPage.Styles;
			bookStyles.Name = "Styles";
			di.bookPanels.Items.Add(bookStyles);

			TabPage bookAutos = null;
			if ( this.debugMode == DebugMode.DebugCommands )
			{
				bookAutos = new TabPage();
				bookAutos.TabTitle = Res.Strings.TabPage.Autos;
				bookAutos.Name = "Autos";
				di.bookPanels.Items.Add(bookAutos);
			}

			TabPage bookPages = new TabPage();
			bookPages.TabTitle = Res.Strings.TabPage.Pages;
			bookPages.Name = "Pages";
			di.bookPanels.Items.Add(bookPages);

			TabPage bookLayers = new TabPage();
			bookLayers.TabTitle = Res.Strings.TabPage.Layers;
			bookLayers.Name = "Layers";
			di.bookPanels.Items.Add(bookLayers);

#if false
			TabPage bookOper = new TabPage();
			bookOper.TabTitle = Res.Strings.TabPage.Operations;
			bookOper.Name = "Operations";
			di.bookPanels.Items.Add(bookOper);
#endif

			di.bookPanels.ActivePage = bookPrincipal;

			di.containerPrincipal = new Containers.Principal(document);
			di.containerPrincipal.SetParent(bookPrincipal);
			di.containerPrincipal.Dock = DockStyle.Fill;
			di.containerPrincipal.Margins = new Margins(4, 4, 10, 4);
			document.Modifier.AttachContainer(di.containerPrincipal);

			di.containerStyles = new Containers.Styles(document);
			di.containerStyles.SetParent(bookStyles);
			di.containerStyles.Dock = DockStyle.Fill;
			di.containerStyles.Margins = new Margins(4, 4, 10, 4);
			document.Modifier.AttachContainer(di.containerStyles);

			if ( bookAutos != null )
			{
				di.containerAutos = new Containers.Autos(document);
				di.containerAutos.SetParent(bookAutos);
				di.containerAutos.Dock = DockStyle.Fill;
				di.containerAutos.Margins = new Margins(4, 4, 10, 4);
				document.Modifier.AttachContainer(di.containerAutos);
			}

			di.containerPages = new Containers.Pages(document);
			di.containerPages.SetParent(bookPages);
			di.containerPages.Dock = DockStyle.Fill;
			di.containerPages.Margins = new Margins(4, 4, 10, 4);
			document.Modifier.AttachContainer(di.containerPages);

			di.containerLayers = new Containers.Layers(document);
			di.containerLayers.SetParent(bookLayers);
			di.containerLayers.Dock = DockStyle.Fill;
			di.containerLayers.Margins = new Margins(4, 4, 10, 4);
			document.Modifier.AttachContainer(di.containerLayers);

#if false
			di.containerOperations = new Containers.Operations(document);
			di.containerOperations.Parent = bookOper;
			di.containerOperations.Dock = DockStyle.Fill;
			di.containerOperations.Margins = new Margins(4, 4, 10, 4);
			document.Modifier.AttachContainer(di.containerOperations);
#endif
		}

		#region LastFilenames
		protected void BuildLastFilenamesMenu()
		{
			//	Construit le sous-menu des derniers fichiers ouverts.
			if ( this.menu == null )  return;

			VMenu lastMenu = new VMenu();
			lastMenu.Name = "LastFilenames";
			lastMenu.Host = this;

			int total = this.globalSettings.LastFilenameCount;
			if ( total == 0 )
			{
				this.MenuAdd(lastMenu, "", "", Res.Strings.LastFiles.None, "");
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

			this.MenuAddSub(this.fileMenu, lastMenu, "SubmenuLastFiles");
		}
		#endregion

		public HMenu GetMenu()
		{
			return this.menu;
		}

		protected void MenuAddSub(VMenu menu, VMenu sub, string cmd)
		{
			//	Ajoute un sous-menu dans un menu.
			for ( int i=0 ; i<menu.Items.Count ; i++ )
			{
				MenuItem item = menu.Items[i] as MenuItem;
				if ( item.Command == cmd )
				{
					item.Submenu = sub;
					item.Command = cmd;
					return;
				}
			}
			System.Diagnostics.Debug.Assert(false, "MenuAddSub: submenu not found");
		}

		protected void MenuAdd(VMenu vmenu, string command)
		{
			//	Ajoute une icône.
			if ( command == null )
			{
				vmenu.Items.Add(new MenuSeparator());
			}
			else
			{
				CommandState cs = this.commandDispatcher.GetCommandState(command);

				MenuItem item = new MenuItem(cs.Name, Misc.Icon(cs.IconName), cs.LongCaption, Misc.GetShortCut(cs), cs.Name);
				vmenu.Items.Add(item);
			}
		}
		
		protected void MenuAdd(VMenu vmenu, string icon, string command, string text, string shortcut)
		{
			//	Ajoute une icône.
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

		protected Widget HToolBarAdd(CommandState cs)
		{
			//	Ajoute une icône.
			if ( cs == null )
			{
				IconSeparator sep = new IconSeparator();
				sep.IsHorizontal = true;
				this.hToolBar.Items.Add(sep);
				return sep;
			}
			else
			{
				IconButton button = new IconButton(cs.Name, Misc.Icon(cs.IconName), cs.Name);
				this.hToolBar.Items.Add(button);
				ToolTip.Default.SetToolTip(button, Misc.GetTextWithShortcut(cs));
				return button;
			}
		}

		protected Widget VToolBarAdd(CommandState cs)
		{
			//	Ajoute une icône.
			if ( cs == null )
			{
				IconSeparator sep = new IconSeparator();
				sep.IsHorizontal = false;
				this.vToolBar.Items.Add(sep);
				return sep;
			}
			else
			{
				IconButton button = new IconButton(cs.Name, Misc.Icon(cs.IconName), cs.Name);
				this.vToolBar.Items.Add(button);
				ToolTip.Default.SetToolTip(button, Misc.GetTextWithShortcut(cs));
				return button;
			}
		}

		protected StatusField InfoAdd(string name, double width)
		{
			StatusField field = new StatusField();
			field.PreferredWidth = width;
			this.info.Items.Add(field);

			int i = this.info.Children.Count-1;
			this.info.Items[i].Name = name;
			return field;
		}

		protected IconButton InfoAdd(string command)
		{
			CommandState cs = this.commandDispatcher.GetCommandState(command);

			IconButton button = new IconButton(cs.Name, Misc.Icon(cs.IconName), cs.Name);
			button.PreferredIconSize = Misc.IconPreferredSize("Small");
			double h = this.info.DefaultHeight-3;
			button.PreferredSize = new Size(h, h);
			this.info.Items.Add(button);
			ToolTip.Default.SetToolTip(button, Misc.GetTextWithShortcut(cs));
			return button;
		}


		private void HandleRibbonPressed(object sender, MessageEventArgs e)
		{
			//	Le bouton pour activer/désactiver un ruban a été cliqué.
			RibbonButton button = sender as RibbonButton;
			Ribbons.RibbonContainer ribbon = null;
			if ( button == this.ribbonMainButton )  ribbon = this.ribbonMain;
			if ( button == this.ribbonGeomButton )  ribbon = this.ribbonGeom;
			if ( button == this.ribbonOperButton )  ribbon = this.ribbonOper;
			if ( button == this.ribbonTextButton )  ribbon = this.ribbonText;
			if ( ribbon == null )  return;

			this.ActiveRibbon(ribbon.IsVisible ? null : ribbon);
		}

		protected Ribbons.RibbonContainer GetRibbon(string name)
		{
			//	Donne le ruban correspondant à un nom.
			if ( name == this.ribbonMain.Name )  return this.ribbonMain;
			if ( name == this.ribbonGeom.Name )  return this.ribbonGeom;
			if ( name == this.ribbonOper.Name )  return this.ribbonOper;
			if ( name == this.ribbonText.Name )  return this.ribbonText;
			return null;
		}

		protected Ribbons.RibbonContainer LastRibbon(string notName)
		{
			//	Cherche le dernier ruban utilisé différent d'un nom donné.
			if ( this.ribbonList == null )  return null;

			for ( int i=this.ribbonList.Count-1 ; i>=0 ; i-- )
			{
				string name = this.ribbonList[i] as string;
				if ( name != notName )
				{
					return this.GetRibbon(name);
				}
			}
			return null;
		}

		protected void ActiveRibbon(Ribbons.RibbonContainer active)
		{
			//	Active un ruban.
			this.ribbonActive = active;

			if ( this.ribbonList == null )
			{
				this.ribbonList = new System.Collections.ArrayList();
			}

			if ( active == null )
			{
				this.ribbonList.Add("");
			}
			else
			{
				this.ribbonList.Add(active.Name);
			}

			if ( this.ribbonList.Count > 10 )
			{
				this.ribbonList.RemoveAt(0);
			}

			this.SuspendLayout();
			this.ribbonMain.Visibility = (this.ribbonMain == this.ribbonActive);
			this.ribbonGeom.Visibility = (this.ribbonGeom == this.ribbonActive);
			this.ribbonOper.Visibility = (this.ribbonOper == this.ribbonActive);
			this.ribbonText.Visibility = (this.ribbonText == this.ribbonActive);

			this.ribbonMainButton.ActiveState = (this.ribbonMain == this.ribbonActive) ? ActiveState.Yes : ActiveState.No;
			this.ribbonGeomButton.ActiveState = (this.ribbonGeom == this.ribbonActive) ? ActiveState.Yes : ActiveState.No;
			this.ribbonOperButton.ActiveState = (this.ribbonOper == this.ribbonActive) ? ActiveState.Yes : ActiveState.No;
			this.ribbonTextButton.ActiveState = (this.ribbonText == this.ribbonActive) ? ActiveState.Yes : ActiveState.No;

			double h = this.RibbonHeight;
			this.vToolBar.Margins = new Margins(0, 0, this.hToolBar.Height+h, this.info.Height);
			this.bookDocuments.Margins = new Margins(this.vToolBar.Width+1, this.panelsWidth+2, this.hToolBar.Height+h+1, this.info.Height+1);

			int total = this.bookDocuments.PageCount;
			for ( int i=0 ; i<total ; i++ )
			{
				DocumentInfo di = this.documents[i] as DocumentInfo;
				di.bookPanels.Margins = new Margins(1, 1, this.hToolBar.Height+h+1, this.info.Height+1);
			}
			this.ResumeLayout();
		}

		protected double RibbonHeight
		{
			//	Retourne la hauteur utilisée par les rubans.
			get
			{
				return (this.ribbonActive == null) ? 0 : this.ribbonHeight;
			}
		}


		private void HandleDlgClosed(object sender)
		{
			//	Un dialogue a été fermé.
			if ( sender == this.dlgGlyphs )
			{
				this.glyphsState.ActiveState = ActiveState.No;
			}

			if ( sender == this.dlgInfos )
			{
				this.infosState.ActiveState = ActiveState.No;
			}

			if ( sender == this.dlgPageStack )
			{
				this.pageStackState.ActiveState = ActiveState.No;
			}

			if ( sender == this.dlgSettings )
			{
				this.settingsState.ActiveState = ActiveState.No;
			}

			if ( sender == this.dlgReplace )
			{
				this.replaceState.ActiveState = ActiveState.No;
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

		private void HandleBookPanelsActivePageChanged(object sender)
		{
			TabBook book = sender as TabBook;
			TabPage page = book.ActivePage;
			this.CurrentDocument.Modifier.TabBookChanged(page.Name);
		}

		private void HandleStatusZoomClicked(object sender, MessageEventArgs e)
		{
			if ( !this.IsCurrentDocument )  return;
			StatusField sf = sender as StatusField;
			if ( sf == null )  return;
			Point pos = sf.MapClientToScreen(new Point(0, sf.Height));
			DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
			VMenu menu = Menus.ZoomMenu.CreateZoomMenu(context.Zoom, context.ZoomPage, null);
			menu.Host = this;
			pos.Y += menu.Height;
			menu.ShowAsContextMenu(this.Window, pos);  // -> commandes "ZoomChange"
		}

		private void HandleSliderZoomChanged(object sender)
		{
			if ( !this.IsCurrentDocument )  return;
			HSlider slider = sender as HSlider;
			if ( slider == null )  return;
			this.CurrentDocument.Modifier.ZoomValue((double) slider.Value, slider.IsInitialChange);
		}

		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();
		}


		[Command ("ToolSelect")]
		[Command ("ToolGlobal")]
		[Command ("ToolShaper")]
		[Command ("ToolEdit")]
		[Command ("ToolZoom")]
		[Command ("ToolHand")]
		[Command ("ToolPicker")]
		[Command ("ToolHotSpot")]
		[Command ("ObjectLine")]
		[Command ("ObjectRectangle")]
		[Command ("ObjectCircle")]
		[Command ("ObjectEllipse")]
		[Command ("ObjectPoly")]
		[Command ("ObjectBezier")]
		[Command ("ObjectRegular")]
		[Command ("ObjectSurface")]
		[Command ("ObjectVolume")]
		[Command ("ObjectTextLine")]
		[Command ("ObjectTextLine2")]
		[Command ("ObjectTextBox")]
		[Command ("ObjectTextBox2")]
		[Command ("ObjectArray")]
		[Command ("ObjectImage")]
		[Command ("ObjectDimension")]
		void CommandTool(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.Tool = e.CommandName;
			this.DispatchDummyMouseMoveEvent();
		}


		
		#region IO
		protected Common.Dialogs.DialogResult DialogSave(CommandDispatcher dispatcher)
		{
			//	Affiche le dialogue pour demander s'il faut enregistrer le
			//	document modifié, avant de passer à un autre document.
			if ( !this.CurrentDocument.IsDirtySerialize ||
				 this.CurrentDocument.Modifier.StatisticTotalObjects() == 0 )
			{
				return Common.Dialogs.DialogResult.None;
			}

			this.dlgSplash.Hide();

			string title = Res.Strings.Application.TitleShort;
			string icon = "manifest:Epsitec.Common.Dialogs.Images.Warning.icon";
			string shortFilename = Misc.ExtractName(this.CurrentDocument.Filename, this.CurrentDocument.IsDirtySerialize);
			string statistic = string.Format("<font size=\"80%\">{0}</font><br/>", this.CurrentDocument.Modifier.Statistic(false, false));
			string question1 = string.Format(Res.Strings.Dialog.Save.Question1, shortFilename);
			string question2 = Res.Strings.Dialog.Save.Question2;
			string message = string.Format("<font size=\"100%\">{0}</font><br/><br/>{1}{2}", question1, statistic, question2);
			Common.Dialogs.IDialog dialog = Common.Dialogs.Message.CreateYesNoCancel(title, icon, message, null, null, dispatcher);
			dialog.Owner = this.Window;
			dialog.OpenDialog();
			return dialog.Result;
		}

		protected Common.Dialogs.DialogResult DialogWarnings(CommandDispatcher dispatcher, System.Collections.ArrayList warnings)
		{
			//	Affiche le dialogue pour signaler la liste de tous les problèmes.
			if ( warnings == null || warnings.Count == 0 )  return Common.Dialogs.DialogResult.None;

			this.dlgSplash.Hide();
			warnings.Sort();

			string title = Res.Strings.Application.TitleShort;
			string icon = "manifest:Epsitec.Common.Dialogs.Images.Warning.icon";

			string chip = "<list type=\"fix\" width=\"1.5\"/>";
			System.Text.StringBuilder builder = new System.Text.StringBuilder();
			builder.Append(Res.Strings.Dialog.Warning.Text1);
			builder.Append("<br/>");
			builder.Append("<br/>");
			foreach ( string s in warnings )
			{
				builder.Append(chip);
				builder.Append(s);
				builder.Append("<br/>");
			}
			builder.Append("<br/>");
			builder.Append(Res.Strings.Dialog.Warning.Text2);
			builder.Append("<br/>");
			builder.Append(Res.Strings.Dialog.Warning.Text3);
			builder.Append("<br/>");
			string message = builder.ToString();

			Common.Dialogs.IDialog dialog = Common.Dialogs.Message.CreateOk(title, icon, message, "", dispatcher);
			dialog.Owner = this.Window;
			dialog.OpenDialog();
			return dialog.Result;
		}

		public Common.Dialogs.DialogResult DialogError(CommandDispatcher dispatcher, string error)
		{
			//	Affiche le dialogue pour signaler une erreur.
			if ( this.Window == null )  return Common.Dialogs.DialogResult.None;
			if ( error == "" )  return Common.Dialogs.DialogResult.None;

			this.dlgSplash.Hide();

			string title = Res.Strings.Application.TitleShort;
			string icon = "manifest:Epsitec.Common.Dialogs.Images.Warning.icon";
			string message = error;

			Common.Dialogs.IDialog dialog = Common.Dialogs.Message.CreateOk(title, icon, message, "", dispatcher);
			dialog.Owner = this.Window;
			dialog.OpenDialog();
			return dialog.Result;
		}

		protected static string AdjustFilename(string filename)
		{
			//	Si on a tapé "toto", mais qu'il existe le fichier "Toto",
			//	met le "vrai" nom dans filename.
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

		protected bool Open(CommandDispatcher dispatcher)
		{
			//	Demande un nom de fichier puis ouvre le fichier.
			//	Affiche l'erreur éventuelle.
			//	Retourne false si le fichier n'a pas été ouvert.
			this.dlgSplash.Hide();

			Common.Dialogs.FileOpen dialog = new Common.Dialogs.FileOpen();

			dialog.InitialDirectory = this.globalSettings.InitialDirectory;
			dialog.FileName = "";
			if ( this.type == DocumentType.Graphic )
			{
				dialog.Title = Res.Strings.Dialog.Open.TitleDoc;
				dialog.Filters.Add("crdoc", Res.Strings.Dialog.FileDoc, "*.crdoc");
			}
			else
			{
				dialog.Title = Res.Strings.Dialog.Open.TitlePic;
				dialog.Filters.Add("icon", Res.Strings.Dialog.FilePic, "*.icon");
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

		protected bool OpenModel(CommandDispatcher dispatcher)
		{
			//	Demande un nom de fichier modèle puis ouvre le fichier.
			//	Affiche l'erreur éventuelle.
			//	Retourne false si le fichier n'a pas été ouvert.
			this.dlgSplash.Hide();

			Common.Dialogs.FileOpen dialog = new Common.Dialogs.FileOpen();

			dialog.InitialDirectory = this.globalSettings.InitialDirectory;
			dialog.FileName = "";
			dialog.Title = Res.Strings.Dialog.Open.TitleMod;
			dialog.Filters.Add("crmod", Res.Strings.Dialog.FileMod, "*.crmod");
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

		public bool Open(string filename)
		{
			//	Ouvre un ficher d'après son nom.
			//	Affiche l'erreur éventuelle.
			//	Retourne false si le fichier n'a pas été ouvert.
			this.MouseShowWait();

			string err = "";
			if ( Misc.IsExtension(filename, ".crcolors") )
			{
				if ( !this.IsCurrentDocument )
				{
					this.CreateDocument();
				}
				err = this.PaletteRead(filename);
			}
			else
			{
				//	Cherche si ce nom de fichier est déjà ouvert ?
				int total = this.bookDocuments.PageCount;
				for ( int i=0 ; i<total ; i++ )
				{
					DocumentInfo di = this.documents[i] as DocumentInfo;
					if ( di.document.Filename == filename )
					{
						this.globalSettings.LastFilenameAdd(filename);
						this.BuildLastFilenamesMenu();
						this.UseDocument(i);
						this.MouseHideWait();
						return true;
					}
				}

				filename = DocumentEditor.AdjustFilename(filename);

				if ( !this.IsRecyclableDocument() )
				{
					this.CreateDocument();
				}
				err = this.CurrentDocument.Read(filename);
				this.UpdateRulers();
				if ( err == "" )
				{
					this.UpdateBookDocuments();
					this.DialogWarnings(this.commandDispatcher, this.CurrentDocument.ReadWarnings);
				}
			}
			//?this.MouseHideWait();
			this.DialogError(this.commandDispatcher, err);
			return (err == "");
		}

		protected bool Save(CommandDispatcher dispatcher, bool ask)
		{
			//	Demande un nom de fichier puis enregistre le fichier.
			//	Si le document a déjà un nom de fichier et que ask=false,
			//	l'enregistrement est fait directement avec le nom connu.
			//	Affiche l'erreur éventuelle.
			//	Retourne false si le fichier n'a pas été enregistré.
			string filename;

			if ( this.CurrentDocument.Filename == "" || ask )
			{
				this.dlgSplash.Hide();

				Common.Dialogs.FileSave dialog = new Common.Dialogs.FileSave();
			
				dialog.InitialDirectory = this.globalSettings.InitialDirectory;
				dialog.FileName = this.CurrentDocument.Filename;
				if ( this.type == DocumentType.Graphic )
				{
					dialog.Title = Res.Strings.Dialog.Save.TitleDoc;
					dialog.Filters.Add("crdoc", Res.Strings.Dialog.FileDoc, "*.crdoc");
				}
				else
				{
					dialog.Title = Res.Strings.Dialog.Save.TitlePic;
					dialog.Filters.Add("icon", Res.Strings.Dialog.FilePic, "*.icon");
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

			this.MouseShowWait();
			string err = this.CurrentDocument.Write(filename);
			if ( err == "" )
			{
				this.globalSettings.InitialDirectory = System.IO.Path.GetDirectoryName(filename);
				this.globalSettings.LastFilenameAdd(filename);
				this.BuildLastFilenamesMenu();
			}
			this.MouseHideWait();
			this.DialogError(dispatcher, err);
			return (err == "");
		}

		protected bool SaveModel(CommandDispatcher dispatcher)
		{
			//	Demande un nom de fichier modèle puis enregistre le fichier.
			//	Retourne false si le fichier n'a pas été enregistré.
			string filename;

			this.dlgSplash.Hide();

			Common.Dialogs.FileSave dialog = new Common.Dialogs.FileSave();
		
			dialog.InitialDirectory = this.globalSettings.InitialDirectory;
			dialog.FileName = "";
			dialog.Title = Res.Strings.Dialog.Save.TitleMod;
			dialog.Filters.Add("crmod", Res.Strings.Dialog.FileMod, "*.crmod");
			dialog.PromptForOverwriting = true;
			dialog.Owner = this.Window;
			dialog.OpenDialog();
			if ( dialog.Result != Common.Dialogs.DialogResult.Accept )  return false;
			filename = dialog.FileName;
			filename = DocumentEditor.AdjustFilename(filename);

			this.MouseShowWait();
			string err = this.CurrentDocument.Write(filename);
			if ( err == "" )
			{
				this.globalSettings.InitialDirectory = System.IO.Path.GetDirectoryName(filename);
				this.globalSettings.LastFilenameAdd(filename);
				this.BuildLastFilenamesMenu();
			}
			this.MouseHideWait();
			this.DialogError(dispatcher, err);
			return (err == "");
		}

		protected bool AutoSave(CommandDispatcher dispatcher)
		{
			//	Fait tout ce qu'il faut pour éventuellement sauvegarder le document
			//	avant de passer à autre chose.
			//	Retourne false si on ne peut pas continuer.
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

		protected bool AutoSaveAll(CommandDispatcher dispatcher)
		{
			//	Fait tout ce qu'il faut pour éventuellement sauvegarder tous les
			//	documents avant de passer à autre chose.
			//	Retourne false si on ne peut pas continuer.
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

		protected bool ForceSaveAll(CommandDispatcher dispatcher)
		{
			//	Sauvegarde tous les documents, même ceux qui sont à jour.
			int cd = this.currentDocument;

			int total = this.bookDocuments.PageCount;
			for ( int i=0 ; i<total ; i++ )
			{
				this.currentDocument = i;
				this.Save(dispatcher, false);
			}

			this.currentDocument = cd;
			return true;
		}
		#endregion

		[Command ("New")]
		void CommandNew(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			if ( this.globalSettings.NewDocument == "" )
			{
				this.CreateDocument();
				this.CurrentDocument.Modifier.New();
				this.CurrentDocument.Modifier.ActiveViewer.Focus();
			}
			else
			{
				this.Open(this.globalSettings.NewDocument);
				this.CurrentDocument.IsDirtySerialize = false;
				this.CurrentDocument.Modifier.ActiveViewer.Focus();
			}
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

		[Command ("OpenModel")]
		void CommandOpenModel(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.OpenModel(dispatcher);
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
		
		[Command ("SaveModel")]
		void CommandSaveModel(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.SaveModel(dispatcher);
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

		[Command ("ForceSaveAll")]
		void CommandForceSaveAll(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.ForceSaveAll(dispatcher);
		}

		[Command ("NextDocument")]
		void CommandNextDocument(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			int doc = this.currentDocument+1;
			if ( doc >= this.bookDocuments.PageCount )  doc = 0;
			this.UseDocument(doc);
		}

		[Command ("PrevDocument")]
		void CommandPrevDocument(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			int doc = this.currentDocument-1;
			if ( doc < 0 )  doc = this.bookDocuments.PageCount-1;
			this.UseDocument(doc);
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
			this.dlgSplash.Hide();

			Document document = this.CurrentDocument;
			Common.Dialogs.Print dialog = document.PrintDialog;
			dialog.Document.DocumentName = System.Utilities.XmlToText(Common.Document.Misc.FullName(document.Filename, false));
			dialog.Owner = this.Window;

			this.dlgPrint.Show();
		}
		
		[Command ("Export")]
		void CommandExport(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.dlgSplash.Hide();

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
			dialog.Title = Res.Strings.Dialog.Export.Title1;
			dialog.Filters.Add("pdf", DocumentEditor.GetRes("File.Vector.PDF"), "*.pdf");
			dialog.Filters.Add("bmp", DocumentEditor.GetRes("File.Bitmap.BMP"), "*.bmp");
			dialog.Filters.Add("tif", DocumentEditor.GetRes("File.Bitmap.TIF"), "*.tif; *.tiff");
			dialog.Filters.Add("jpg", DocumentEditor.GetRes("File.Bitmap.JPG"), "*.jpg; *.jpeg");
			dialog.Filters.Add("png", DocumentEditor.GetRes("File.Bitmap.PNG"), "*.png");
			dialog.Filters.Add("gif", DocumentEditor.GetRes("File.Bitmap.GIF"), "*.gif");
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

			if ( this.CurrentDocument.ExportFilter == 0 )  // PDF ?
			{
				this.dlgExportPDF.Show(this.CurrentDocument.ExportFilename);
			}
			else
			{
				string ext = dialog.Filters[this.CurrentDocument.ExportFilter].Name;
				this.CurrentDocument.Printer.ImageFormat = Printer.GetImageFormat(ext);

				this.dlgExport.Show(this.CurrentDocument.ExportFilename);
			}
		}
		
		[Command ("Glyphs")]
		void CommandGlyphs(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.dlgSplash.Hide();

			if ( this.glyphsState.ActiveState == ActiveState.No )
			{
				this.dlgGlyphs.Show();
				this.glyphsState.ActiveState = ActiveState.Yes;
			}
			else
			{
				this.dlgGlyphs.Hide();
				this.glyphsState.ActiveState = ActiveState.No;
			}
		}

		[Command ("Replace")]
		void CommandReplace(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.dlgSplash.Hide();

			this.dlgReplace.Show();
			this.replaceState.ActiveState = ActiveState.Yes;
		}

		[Command ("FindNext")]
		[Command ("FindPrev")]
		[Command ("FindDefNext")]
		[Command ("FindDefPrev")]
		void CommandFind(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			if ( !this.IsCurrentDocument )  return;

			bool isPrev = (e.CommandName == "FindPrev" || e.CommandName == "FindDefPrev");
			bool isDef = (e.CommandName == "FindDefNext" || e.CommandName == "FindDefPrev");

			if ( isDef )  // Ctrl-F3 ?
			{
				string word = this.CurrentDocument.Modifier.GetSelectedWord();  // mot actuellement sélectionné
				if ( word == null )  return;
				this.dlgReplace.FindText = word;  // il devient le critère de recherche
			}

			Misc.StringSearch mode = this.dlgReplace.Mode;
			mode &= ~Misc.StringSearch.EndToStart;
			if ( isPrev )  mode |= Misc.StringSearch.EndToStart;  // Shift-F3 ?
			this.dlgReplace.Mode = mode;  // modifie juste la direction de la recherche

			this.dlgReplace.MemoriseTexts();

			this.CurrentDocument.Modifier.TextReplace(this.dlgReplace.FindText, null, mode);
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
			this.dlgSplash.Hide();

			Common.Dialogs.FileOpen dialog = new Common.Dialogs.FileOpen();
			
			dialog.InitialDirectory = this.CurrentDocument.GlobalSettings.ColorCollectionDirectory;
			dialog.FileName = this.CurrentDocument.GlobalSettings.ColorCollectionFilename;
			dialog.Title = Res.Strings.Dialog.Open.TitleCol;
			dialog.Filters.Add("crcolors", Res.Strings.Dialog.FileCol, "*.crcolors");
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
			this.dlgSplash.Hide();

			Common.Dialogs.FileSave dialog = new Common.Dialogs.FileSave();
			
			dialog.InitialDirectory = this.CurrentDocument.GlobalSettings.ColorCollectionDirectory;
			dialog.FileName = this.CurrentDocument.GlobalSettings.ColorCollectionFilename;
			dialog.Title = Res.Strings.Dialog.Save.TitleCol;
			dialog.Filters.Add("crcolors", Res.Strings.Dialog.FileCol, "*.crcolors");
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

		public string PaletteRead(string filename)
		{
			//	Lit la collection de couleurs à partir d'un fichier.
			try
			{
				using ( Stream stream = File.OpenRead(filename) )
				{
					SoapFormatter formatter = new SoapFormatter();
					formatter.Binder = new VersionDeserializationBinder();
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

		public string PaletteWrite(string filename)
		{
			//	Ecrit la collection de couleurs dans un fichier.
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

		[Command ("FontBold")]
		[Command ("FontItalic")]
		[Command ("FontUnderlined")]
		[Command ("FontOverlined")]
		[Command ("FontStrikeout")]
		[Command ("FontSubscript")]
		[Command ("FontSuperscript")]
		[Command ("FontSizePlus")]
		[Command ("FontSizeMinus")]
		[Command ("FontClear")]
		[Command ("ParagraphLeadingPlus")]
		[Command ("ParagraphLeadingMinus")]
		[Command ("ParagraphIndentPlus")]
		[Command ("ParagraphIndentMinus")]
		[Command ("ParagraphClear")]
		void CommandFont(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Wrappers.ExecuteCommand(e.CommandName, null);
		}

		[Command ("ParagraphLeading")]
		[Command ("ParagraphJustif")]
		void CommandCombo(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			IconButtonCombo combo = e.Source as IconButtonCombo;
			CommandState cs = dispatcher.FindCommandState(e.CommandName);
			if ( combo != null && cs != null )
			{
				cs.AdvancedState = combo.SelectedName;
				this.CurrentDocument.Wrappers.ExecuteCommand(e.CommandName, cs.AdvancedState);
			}
			else
			{
				this.CurrentDocument.Wrappers.ExecuteCommand(e.CommandName, null);
			}
		}


		[Command ("Undo")]
		void CommandUndo(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.Undo(1);
		}

		[Command ("Redo")]
		void CommandRedo(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.Redo(1);
		}

		[Command ("UndoRedoList")]
		void CommandUndoRedoList(CommandDispatcher dispatcher, CommandEventArgs e)
		{
#if false
			Widget button = this.hToolBar.FindChild("Undo");
			if ( button == null )  return;
			Point pos = button.MapClientToScreen(new Point(0, 1));
			VMenu menu = this.CurrentDocument.Modifier.CreateUndoRedoMenu(null);
			menu.Host = this;
			menu.Accepted += new MenuEventHandler(this.HandleUndoRedoMenuAccepted);
			menu.Rejected += new EventHandler(this.HandleUndoRedoMenuRejected);
			menu.ShowAsContextMenu(this.Window, pos);
			this.WidgetUndoRedoMenuEngaged(true);
#endif
		}

#if false
		private void HandleUndoRedoMenuAccepted(object sender, MenuEventArgs e)
		{
			this.WidgetUndoRedoMenuEngaged(false);
		}

		private void HandleUndoRedoMenuRejected(object sender)
		{
			this.WidgetUndoRedoMenuEngaged(false);
		}
#endif

		protected void WidgetUndoRedoMenuEngaged(bool engaged)
		{
			Widget button;

			button = this.hToolBar.FindChild("Undo");
			if ( button != null )  button.ActiveState = engaged ? ActiveState.Yes : ActiveState.No;
			
			button = this.hToolBar.FindChild("UndoRedoList");
			if ( button != null )  button.ActiveState = engaged ? ActiveState.Yes : ActiveState.No;
			
			button = this.hToolBar.FindChild("Redo");
			if ( button != null )  button.ActiveState = engaged ? ActiveState.Yes : ActiveState.No;
		}

		[Command ("UndoRedoListDo")]
		void CommandUndoRedoListDo(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			int nb = System.Convert.ToInt32(e.CommandArgs[0]);
			if ( nb > 0 )
			{
				this.CurrentDocument.Modifier.Undo(nb);
			}
			else
			{
				this.CurrentDocument.Modifier.Redo(-nb);
			}
		}


		[Command ("OrderUpOne")]
		void CommandOrderUpOne(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.OrderUpOneSelection();
		}

		[Command ("OrderDownOne")]
		void CommandOrderDownOne(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.OrderDownOneSelection();
		}

		[Command ("OrderUpAll")]
		void CommandOrderUpAll(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.OrderUpAllSelection();
		}

		[Command ("OrderDownAll")]
		void CommandOrderDownAll(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.OrderDownAllSelection();
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

		[Command ("RotateFreeCCW")]
		void CommandRotateFreeCCW(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			double angle = this.CurrentDocument.Modifier.RotateAngle;
			this.CurrentDocument.Modifier.RotateSelection(angle);
		}

		[Command ("RotateFreeCW")]
		void CommandRotateFreeCW(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			double angle = this.CurrentDocument.Modifier.RotateAngle;
			this.CurrentDocument.Modifier.RotateSelection(-angle);
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

		[Command ("ScaleMul2")]
		void CommandScaleMul2(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.ScaleSelection(2.0);
		}

		[Command ("ScaleDiv2")]
		void CommandScaleDiv2(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.ScaleSelection(0.5);
		}

		[Command ("ScaleMulFree")]
		void CommandScaleMulFree(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			double scale = this.CurrentDocument.Modifier.ScaleFactor;
			this.CurrentDocument.Modifier.ScaleSelection(scale);
		}

		[Command ("ScaleDivFree")]
		void CommandScaleDivFree(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			double scale = this.CurrentDocument.Modifier.ScaleFactor;
			this.CurrentDocument.Modifier.ScaleSelection(1.0/scale);
		}

		[Command ("AlignLeft")]
		void CommandAlignLeft(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.AlignSelection(-1, true);
		}

		[Command ("AlignCenterX")]
		void CommandAlignCenterX(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.AlignSelection(0, true);
		}

		[Command ("AlignRight")]
		void CommandAlignRight(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.AlignSelection(1, true);
		}

		[Command ("AlignTop")]
		void CommandAlignTop(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.AlignSelection(1, false);
		}

		[Command ("AlignCenterY")]
		void CommandAlignCenterY(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.AlignSelection(0, false);
		}

		[Command ("AlignBottom")]
		void CommandAlignBottom(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.AlignSelection(-1, false);
		}

		[Command ("AlignGrid")]
		void CommandAlignGrid(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.AlignGridSelection();
		}

		[Command ("ShareLeft")]
		void CommandShareLeft(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.ShareSelection(-1, true);
		}

		[Command ("ShareCenterX")]
		void CommandShareCenterX(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.ShareSelection(0, true);
		}

		[Command ("ShareSpaceX")]
		void CommandShareSpaceX(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.SpaceSelection(true);
		}

		[Command ("ShareRight")]
		void CommandShareRight(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.ShareSelection(1, true);
		}

		[Command ("ShareTop")]
		void CommandShareTop(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.ShareSelection(1, false);
		}

		[Command ("ShareCenterY")]
		void CommandShareCenterY(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.ShareSelection(0, false);
		}

		[Command ("ShareSpaceY")]
		void CommandShareSpaceY(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.SpaceSelection(false);
		}

		[Command ("ShareBottom")]
		void CommandShareBottom(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.ShareSelection(-1, false);
		}

		[Command ("AdjustWidth")]
		void CommandAdjustWidth(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.AdjustSelection(true);
		}

		[Command ("AdjustHeight")]
		void CommandAdjustHeight(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.AdjustSelection(false);
		}

		[Command ("ColorToRgb")]
		void CommandColorToRgb(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.ColorSelection(ColorSpace.Rgb);
		}

		[Command ("ColorToCmyk")]
		void CommandColorToCmyk(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.ColorSelection(ColorSpace.Cmyk);
		}

		[Command ("ColorToGray")]
		void CommandColorToGray(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.ColorSelection(ColorSpace.Gray);
		}

		[Command ("ColorStrokeDark")]
		void CommandColorStrokeDark(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			double adjust = this.CurrentDocument.Modifier.ColorAdjust;
			this.CurrentDocument.Modifier.ColorSelection(-adjust, true);
		}

		[Command ("ColorStrokeLight")]
		void CommandColorStrokeLight(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			double adjust = this.CurrentDocument.Modifier.ColorAdjust;
			this.CurrentDocument.Modifier.ColorSelection(adjust, true);
		}

		[Command ("ColorFillDark")]
		void CommandColorFillDark(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			double adjust = this.CurrentDocument.Modifier.ColorAdjust;
			this.CurrentDocument.Modifier.ColorSelection(-adjust, false);
		}

		[Command ("ColorFillLight")]
		void CommandColorFillLight(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			double adjust = this.CurrentDocument.Modifier.ColorAdjust;
			this.CurrentDocument.Modifier.ColorSelection(adjust, false);
		}

		[Command ("Merge")]
		void CommandMerge(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.MergeSelection();
		}

		[Command ("Extract")]
		void CommandExtract(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.ExtractSelection();
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

		[Command ("ToTextBox2")]
		void CommandToTextBox2(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.ToTextBox2Selection();
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

		[Command ("ShaperHandleAdd")]
		[Command ("ShaperHandleSub")]
		[Command ("ShaperHandleToLine")]
		[Command ("ShaperHandleToCurve")]
		[Command ("ShaperHandleSym")]
		[Command ("ShaperHandleSmooth")]
		[Command ("ShaperHandleDis")]
		[Command ("ShaperHandleInline")]
		[Command ("ShaperHandleFree")]
		[Command ("ShaperHandleSimply")]
		[Command ("ShaperHandleCorner")]
		[Command ("ShaperHandleContinue")]
		void CommandShaperHandle(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.ShaperHandleCommand(e.CommandName);
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

		[Command ("TextGrid")]
		void CommandTextGrid(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
			context.TextGridShow = !context.TextGridShow;
		}

		[Command ("TextShowControlCharacters")]
		void CommandTextShowControlCharacters(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
			context.TextShowControlCharacters = !context.TextShowControlCharacters;
		}

		[Command ("TextFontFilter")]
		void CommandTextFontFilter(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
			context.TextFontFilter = !context.TextFontFilter;
		}

		[Command ("TextFontSampleAbc")]
		void CommandTextFontSampleAbc(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
			context.TextFontSampleAbc = !context.TextFontSampleAbc;
		}

		[Command ("TextInsertQuad")]
		void CommandTextInsertQuad(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			if ( !this.IsCurrentDocument )  return;
			this.CurrentDocument.Modifier.EditInsertText(Common.Text.Unicode.Code.NoBreakSpace);
		}

		[Command ("TextInsertNewFrame")]
		void CommandTextInsertNewFrame(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			if ( !this.IsCurrentDocument )  return;
			this.CurrentDocument.Modifier.EditInsertText(Common.Text.Properties.BreakProperty.NewFrame);
		}

		[Command ("TextInsertNewPage")]
		void CommandTextInsertNewPage(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			if ( !this.IsCurrentDocument )  return;
			this.CurrentDocument.Modifier.EditInsertText(Common.Text.Properties.BreakProperty.NewPage);
		}

		[Command ("Magnet")]
		void CommandMagnet(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
			context.MagnetActive = !context.MagnetActive;
		}

		[Command ("MagnetLayer")]
		void CommandMagnetLayer(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
			int rank = context.CurrentLayer;
			this.CurrentDocument.Modifier.MagnetLayerInvert(rank);
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

		[Command ("Aggregates")]
		void CommandAggregates(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
			context.AggregatesShow = !context.AggregatesShow;
		}

		[Command ("Preview")]
		void CommandPreview(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
			context.PreviewActive = !context.PreviewActive;
		}

		[Command ("DeselectAll")]
		void CommandDeselectAll(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.DeselectAllCmd();
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
			this.CurrentDocument.Modifier.Tool = "ToolSelect";
			Viewer viewer = this.CurrentDocument.Modifier.ActiveViewer;
			viewer.PartialSelect = false;
		}

		[Command ("SelectPartial")]
		void CommandSelectPartial(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.Tool = "ToolSelect";
			Viewer viewer = this.CurrentDocument.Modifier.ActiveViewer;
			viewer.PartialSelect = true;
		}

		[Command ("SelectorAuto")]
		void CommandSelectorAuto(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.Tool = "ToolSelect";
			Viewer viewer = this.CurrentDocument.Modifier.ActiveViewer;
			viewer.SelectorType = SelectorType.Auto;
		}

		[Command ("SelectorIndividual")]
		void CommandSelectorIndividual(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.Tool = "ToolSelect";
			Viewer viewer = this.CurrentDocument.Modifier.ActiveViewer;
			viewer.SelectorType = SelectorType.Individual;
		}

		[Command ("SelectorScaler")]
		void CommandSelectorScaler(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.Tool = "ToolSelect";
			Viewer viewer = this.CurrentDocument.Modifier.ActiveViewer;
			viewer.SelectorType = SelectorType.Scaler;
		}

		[Command ("SelectorStretch")]
		void CommandSelectorStretch(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.Tool = "ToolSelect";
			Viewer viewer = this.CurrentDocument.Modifier.ActiveViewer;
			viewer.SelectorType = SelectorType.Stretcher;
		}

		[Command ("SelectorStretchType")]
		void CommandSelectorStretchType(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			DocumentInfo di = this.CurrentDocumentInfo;
			HToolBar toolbar = di.containerPrincipal.SelectorToolBar;
			Widget button = toolbar.FindChild("SelectorStretch");
			if ( button == null )  return;
			Point pos = button.MapClientToScreen(new Point(0, 1));
			VMenu menu = di.containerPrincipal.CreateStretchTypeMenu(null);
			menu.Host = this;
			menu.Behavior.Accepted += new EventHandler(this.HandleStretchTypeMenuAccepted);
			menu.Behavior.Rejected += new EventHandler(this.HandleStretchTypeMenuRejected);

			ScreenInfo si = ScreenInfo.Find(pos);
			Drawing.Rectangle wa = si.WorkingArea;
			if ( pos.X+menu.Width > wa.Right )
			{
				pos.X = wa.Right-menu.Width;
			}

			menu.ShowAsComboList (this, pos, button);
			this.WidgetStretchTypeMenuEngaged(true, true);
		}

		private void HandleStretchTypeMenuAccepted(object sender)
		{
			this.WidgetStretchTypeMenuEngaged(false, true);
		}

		private void HandleStretchTypeMenuRejected(object sender)
		{
			Viewer viewer = this.CurrentDocument.Modifier.ActiveViewer;
			bool activate = (viewer.SelectorType == SelectorType.Stretcher);
			this.WidgetStretchTypeMenuEngaged(false, activate);
		}

		protected void WidgetStretchTypeMenuEngaged(bool engaged, bool activate)
		{
			Widget button;

			DocumentInfo di = this.CurrentDocumentInfo;
			HToolBar toolbar = di.containerPrincipal.SelectorToolBar;
			if ( toolbar == null )  return;

			button = toolbar.FindChild("SelectorStretch");
			if ( button != null )  button.ActiveState = activate ? ActiveState.Yes : ActiveState.No;
			
			button = toolbar.FindChild("SelectorStretchType");
			if ( button != null )  button.ActiveState = engaged ? ActiveState.Yes : ActiveState.No;
		}

		[Command ("SelectorStretchTypeDo")]
		void CommandSelectorStretchTypeDo(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			int nb = System.Convert.ToInt32(e.CommandArgs[0]);
			Viewer viewer = this.CurrentDocument.Modifier.ActiveViewer;
			viewer.SelectorTypeStretch = (SelectorTypeStretch) nb;
		}

		
		[Command ("SelectorAdaptLine")]
		void CommandSelectorAdaptLine(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			Viewer viewer = this.CurrentDocument.Modifier.ActiveViewer;
			viewer.SelectorAdaptLine = !viewer.SelectorAdaptLine;
		}

		[Command ("SelectorAdaptText")]
		void CommandSelectorAdaptText(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			Viewer viewer = this.CurrentDocument.Modifier.ActiveViewer;
			viewer.SelectorAdaptText = !viewer.SelectorAdaptText;
		}

		[Command ("HideHalf")]
		void CommandHideHalf(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.Tool = "ToolSelect";
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

		[Command ("ZoomChange")]
		void CommandZoomChange(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			double zoom = System.Convert.ToDouble(e.CommandArgs[0]);
			this.CurrentDocument.Modifier.ZoomValue(zoom);
		}

		[Command ("Object")]
		void CommandObject(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			//	Exécute une commande locale à un objet.
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
			this.dlgSplash.Hide();

			if ( this.settingsState.ActiveState == ActiveState.No )
			{
				this.dlgSettings.Show();
				this.settingsState.ActiveState = ActiveState.Yes;
			}
			else
			{
				this.dlgSettings.Hide();
				this.settingsState.ActiveState = ActiveState.No;
			}
		}

		[Command ("Infos")]
		void CommandInfos(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.dlgSplash.Hide();

			if ( this.infosState.ActiveState == ActiveState.No )
			{
				this.dlgInfos.Show();
				this.infosState.ActiveState = ActiveState.Yes;
			}
			else
			{
				this.dlgInfos.Hide();
				this.infosState.ActiveState = ActiveState.No;
			}
		}

		[Command ("PageStack")]
		void CommandPageStack(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.dlgSplash.Hide();

			if ( this.pageStackState.ActiveState == ActiveState.No )
			{
				this.dlgPageStack.Show();
				this.pageStackState.ActiveState = ActiveState.Yes;
			}
			else
			{
				this.dlgPageStack.Hide();
				this.pageStackState.ActiveState = ActiveState.No;
			}
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
			this.StartCheck(true);
			this.EndCheck(true);
		}

		[Command ("AboutApplication")]
		void CommandAboutApplication(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.dlgSplash.Hide();
			this.dlgAbout.Show();
		}


		[Command ("MoveLeftFree")]
		void CommandMoveLeftFree(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			double dx = this.CurrentDocument.Modifier.MoveDistanceH;
			this.CurrentDocument.Modifier.MoveSelection(new Point(-dx,0));
		}

		[Command ("MoveRightFree")]
		void CommandMoveRightFree(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			double dx = this.CurrentDocument.Modifier.MoveDistanceH;
			this.CurrentDocument.Modifier.MoveSelection(new Point(dx,0));
		}

		[Command ("MoveUpFree")]
		void CommandMoveUpFree(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			double dy = this.CurrentDocument.Modifier.MoveDistanceV;
			this.CurrentDocument.Modifier.MoveSelection(new Point(0,dy));
		}

		[Command ("MoveDownFree")]
		void CommandMoveDownFree(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			double dy = this.CurrentDocument.Modifier.MoveDistanceV;
			this.CurrentDocument.Modifier.MoveSelection(new Point(0,-dy));
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

		private void HandleQuickPageMenu(object sender, MessageEventArgs e)
		{
			//	Bouton "menu des pages" cliqué.
			Button button = sender as Button;
			if ( button == null )  return;
			Point pos = button.MapClientToScreen(new Point(0, button.Height));
			VMenu menu = this.CreatePagesMenu();
			menu.Host = this;
			pos.Y += menu.Height;
			menu.ShowAsComboList (this, pos, button);
		}

		[Command ("PageSelect")]
		void CommandPageSelect(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
			int sel = System.Convert.ToInt32(e.CommandArgs[0]);
			context.CurrentPage = sel;
		}

		[Command ("PageNew")]
		void CommandPageNew(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
			int rank = context.CurrentPage;
			this.CurrentDocument.Modifier.PageNew(rank+1, "");
		}

		[Command ("PageDuplicate")]
		void CommandPageDuplicate(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
			int rank = context.CurrentPage;
			this.CurrentDocument.Modifier.PageDuplicate(rank, "");
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

		private void HandleQuickLayerMenu(object sender, MessageEventArgs e)
		{
			//	Bouton "menu des calques" cliqué.
			Button button = sender as Button;
			if ( button == null )  return;
			Point pos = button.MapClientToScreen(new Point(0, button.Height));
			VMenu menu = this.CreateLayersMenu();
			menu.Host = this;
			pos.X -= menu.Width;
			menu.ShowAsComboList (this, pos, button);
		}

		[Command ("LayerSelect")]
		void CommandLayerSelect(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
			int sel = System.Convert.ToInt32(e.CommandArgs[0]);
			context.CurrentLayer = sel;
		}

		[Command ("LayerNew")]
		void CommandLayerNew(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
			int rank = context.CurrentLayer;
			this.CurrentDocument.Modifier.LayerNew(rank+1, "");
		}

		[Command ("LayerDuplicate")]
		void CommandLayerDuplicate(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
			int rank = context.CurrentLayer;
			this.CurrentDocument.Modifier.LayerDuplicate(rank, "");
		}

		[Command ("LayerNewSel")]
		void CommandLayerNewSel(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
			int rank = context.CurrentLayer;
			this.CurrentDocument.Modifier.LayerNewSel(rank, "");
		}

		[Command ("LayerMergeUp")]
		void CommandLayerMergeUp(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
			int rank = context.CurrentLayer;
			this.CurrentDocument.Modifier.LayerMerge(rank, rank+1);
		}

		[Command ("LayerMergeDown")]
		void CommandLayerMergeDown(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
			int rank = context.CurrentLayer;
			this.CurrentDocument.Modifier.LayerMerge(rank, rank-1);
		}

		[Command ("LayerUp")]
		void CommandLayerUp(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
			int rank = context.CurrentLayer;
			this.CurrentDocument.Modifier.LayerSwap(rank, rank+1);
			context.CurrentLayer = rank+1;
		}

		[Command ("LayerDown")]
		void CommandLayerDown(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
			int rank = context.CurrentLayer;
			this.CurrentDocument.Modifier.LayerSwap(rank, rank-1);
			context.CurrentLayer = rank-1;
		}

		[Command ("LayerDelete")]
		void CommandLayerDelete(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
			int rank = context.CurrentLayer;
			this.CurrentDocument.Modifier.LayerDelete(rank);
		}


		[Command ("ResDesignerBuild")]
		void CommandResDesignerBuild(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.dlgSplash.Hide();
			this.resDesignerMainWindow = new Epsitec.Common.Designer.MainWindow();
			this.resDesignerMainWindow.Show(this.Window, Epsitec.Common.Designer.DesignerMode.Build);
		}

		[Command ("ResDesignerTranslate")]
		void CommandResDesignerTranslate(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.dlgSplash.Hide();
			this.resDesignerMainWindow = new Epsitec.Common.Designer.MainWindow();
			this.resDesignerMainWindow.Show(this.Window, Epsitec.Common.Designer.DesignerMode.Translate);
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


		public void QuitApplication()
		{
			//	Quitte l'application.
			this.WritedGlobalSettings();
			Window.Quit();
		}


		public VMenu CreatePagesMenu()
		{
			//	Construit le menu pour choisir une page.
			DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
			UndoableList pages = this.CurrentDocument.GetObjects;  // liste des pages
			return Objects.Page.CreateMenu(pages, context.CurrentPage, null);
		}

		public VMenu CreateLayersMenu()
		{
			//	Construit le menu pour choisir un calque.
			DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
			Objects.Abstract page = context.RootObject(1);
			UndoableList layers = page.Objects;  // liste des calques
			return Objects.Layer.CreateMenu(layers, context.CurrentLayer, null);
		}



		protected void InitCommands()
		{
			//	Initialise toutes les commandes.
			this.toolSelectState = this.CreateCommandState("ToolSelect", "Select", "ToolSelect", KeyCode.AlphaS);
			this.toolGlobalState = this.CreateCommandState("ToolGlobal", "Global", "ToolGlobal", KeyCode.AlphaG);
			this.toolShaperState = this.CreateCommandState("ToolShaper", "Shaper", "ToolShaper", KeyCode.AlphaA);
			this.toolEditState = this.CreateCommandState("ToolEdit", "Edit", "ToolEdit", KeyCode.AlphaE);
			this.toolZoomState = this.CreateCommandState("ToolZoom", "Zoom", "ToolZoom", KeyCode.AlphaZ);
			this.toolHandState = this.CreateCommandState("ToolHand", "Hand", "ToolHand", KeyCode.AlphaH);
			this.toolPickerState = this.CreateCommandState("ToolPicker", "Picker", "ToolPicker", KeyCode.AlphaI);
			this.toolHotSpotState = this.CreateCommandState("ToolHotSpot", "HotSpot", "ToolHotSpot");
			this.toolLineState = this.CreateCommandState("ObjectLine", "ObjectLine", "ToolLine", KeyCode.AlphaL);
			this.toolRectangleState = this.CreateCommandState("ObjectRectangle", "ObjectRectangle", "ToolRectangle", KeyCode.AlphaR);
			this.toolCircleState = this.CreateCommandState("ObjectCircle", "ObjectCircle", "ToolCircle", KeyCode.AlphaC);
			this.toolEllipseState = this.CreateCommandState("ObjectEllipse", "ObjectEllipse", "ToolEllipse");
			this.toolPolyState = this.CreateCommandState("ObjectPoly", "ObjectPoly", "ToolPoly", KeyCode.AlphaP);
			this.toolBezierState = this.CreateCommandState("ObjectBezier", "ObjectBezier", "ToolBezier", KeyCode.AlphaB);
			this.toolRegularState = this.CreateCommandState("ObjectRegular", "ObjectRegular", "ToolRegular");
			this.toolSurfaceState = this.CreateCommandState("ObjectSurface", "ObjectSurface", "ToolSurface");
			this.toolVolumeState = this.CreateCommandState("ObjectVolume", "ObjectVolume", "ToolVolume");
			this.toolTextLineState = this.CreateCommandState("ObjectTextLine", "ObjectTextLine", "ToolTextLine");
			this.toolTextLine2State = this.CreateCommandState("ObjectTextLine2", "ObjectTextLine", "ToolTextLine");
			this.toolTextBoxState = this.CreateCommandState("ObjectTextBox", "ObjectTextBox", "ToolTextBox");
			this.toolTextBox2State = this.CreateCommandState("ObjectTextBox2", "ObjectTextBox", "ToolTextBox", KeyCode.AlphaT);
			this.toolArrayState = this.CreateCommandState("ObjectArray", "ObjectArray", "ToolArray");
			this.toolImageState = this.CreateCommandState("ObjectImage", "ObjectImage", "ToolImage");
			this.toolDimensionState = this.CreateCommandState("ObjectDimension", "ObjectDimension", "ToolDimension");

			this.newState = this.CreateCommandState("New", KeyCode.ModifierControl|KeyCode.AlphaN);
			this.openState = this.CreateCommandState("Open", KeyCode.ModifierControl|KeyCode.AlphaO);
			this.openModelState = this.CreateCommandState("OpenModel");
			this.saveState = this.CreateCommandState("Save", KeyCode.ModifierControl|KeyCode.AlphaS);
			this.saveAsState = this.CreateCommandState("SaveAs");
			this.saveModelState = this.CreateCommandState("SaveModel");
			this.closeState = this.CreateCommandState("Close", null, "Close", KeyCode.ModifierControl|KeyCode.FuncF4);
			this.closeAllState = this.CreateCommandState("CloseAll");
			this.forceSaveAllState = this.CreateCommandState("ForceSaveAll");
			this.nextDocState = this.CreateCommandState("NextDocument", KeyCode.ModifierControl|KeyCode.FuncF6);
			this.prevDocState = this.CreateCommandState("PrevDocument", KeyCode.ModifierControl|KeyCode.ModifierShift|KeyCode.FuncF6);
			this.printState = this.CreateCommandState("Print", KeyCode.ModifierControl|KeyCode.AlphaP);
			this.exportState = this.CreateCommandState("Export");
			this.glyphsState = this.CreateCommandState("Glyphs");
			this.glyphsInsertState = this.CreateCommandState("GlyphsInsert");
			this.textEditingState = this.CreateCommandState("TextEditing");
			this.replaceState = this.CreateCommandState("Replace", KeyCode.ModifierControl|KeyCode.AlphaF);
			this.findNextState = this.CreateCommandState("FindNext", KeyCode.FuncF3);
			this.findPrevState = this.CreateCommandState("FindPrev", KeyCode.ModifierShift|KeyCode.FuncF3);
			this.findDefNextState = this.CreateCommandState("FindDefNext", KeyCode.ModifierControl|KeyCode.FuncF3);
			this.findDefPrevState = this.CreateCommandState("FindDefPrev", KeyCode.ModifierControl|KeyCode.ModifierShift|KeyCode.FuncF3);
			this.deleteState = this.CreateCommandState("Delete", KeyCode.Delete);
			this.duplicateState = this.CreateCommandState("Duplicate", KeyCode.ModifierControl|KeyCode.AlphaD);

			this.cutState = this.CreateCommandState("Cut", KeyCode.ModifierControl|KeyCode.AlphaX);
			this.copyState = this.CreateCommandState("Copy", KeyCode.ModifierControl|KeyCode.AlphaC);
			this.pasteState = this.CreateCommandState("Paste", KeyCode.ModifierControl|KeyCode.AlphaV);
			
			this.fontBoldState = this.CreateCommandState("FontBold", true, KeyCode.ModifierControl|KeyCode.AlphaB);
			this.fontItalicState = this.CreateCommandState("FontItalic", true, KeyCode.ModifierControl|KeyCode.AlphaI);
			this.fontUnderlinedState = this.CreateCommandState("FontUnderlined", true, KeyCode.ModifierControl|KeyCode.AlphaU);
			this.fontOverlinedState = this.CreateCommandState("FontOverlined", true);
			this.fontStrikeoutState = this.CreateCommandState("FontStrikeout", true);
			this.fontSubscriptState = this.CreateCommandState("FontSubscript", true);
			this.fontSuperscriptState = this.CreateCommandState("FontSuperscript", true);
			this.fontSizePlusState = this.CreateCommandState("FontSizePlus");
			this.fontSizeMinusState = this.CreateCommandState("FontSizeMinus");
			this.fontClearState = this.CreateCommandState("FontClear");
			this.paragraphLeadingState = this.CreateCommandState("ParagraphLeading", true);
			this.paragraphLeadingPlusState = this.CreateCommandState("ParagraphLeadingPlus");
			this.paragraphLeadingMinusState = this.CreateCommandState("ParagraphLeadingMinus");
			this.paragraphIndentPlusState = this.CreateCommandState("ParagraphIndentPlus");
			this.paragraphIndentMinusState = this.CreateCommandState("ParagraphIndentMinus");
			this.paragraphJustifState = this.CreateCommandState("ParagraphJustif", null, "ParagraphJustif", true);
			this.paragraphClearState = this.CreateCommandState("ParagraphClear");
			
			this.orderUpOneState = this.CreateCommandState("OrderUpOne", KeyCode.ModifierControl|KeyCode.PageUp);
			this.orderDownOneState = this.CreateCommandState("OrderDownOne", KeyCode.ModifierControl|KeyCode.PageDown);
			this.orderUpAllState = this.CreateCommandState("OrderUpAll", KeyCode.ModifierShift|KeyCode.PageUp);
			this.orderDownAllState = this.CreateCommandState("OrderDownAll", KeyCode.ModifierShift|KeyCode.PageDown);
			
			this.moveLeftFreeState = this.CreateCommandState("MoveLeftFree", "MoveHi", "MoveLeft");
			this.moveRightFreeState = this.CreateCommandState("MoveRightFree", "MoveH", "MoveRight");
			this.moveUpFreeState = this.CreateCommandState("MoveUpFree", "MoveV", "MoveUp");
			this.moveDownFreeState = this.CreateCommandState("MoveDownFree", "MoveVi", "MoveDown");
			
			this.rotate90State = this.CreateCommandState("Rotate90");
			this.rotate180State = this.CreateCommandState("Rotate180");
			this.rotate270State = this.CreateCommandState("Rotate270");
			this.rotateFreeCCWState = this.CreateCommandState("RotateFreeCCW");
			this.rotateFreeCWState = this.CreateCommandState("RotateFreeCW");
			
			this.mirrorHState = this.CreateCommandState("MirrorH");
			this.mirrorVState = this.CreateCommandState("MirrorV");
			
			this.scaleMul2State = this.CreateCommandState("ScaleMul2");
			this.scaleDiv2State = this.CreateCommandState("ScaleDiv2");
			this.scaleMulFreeState = this.CreateCommandState("ScaleMulFree");
			this.scaleDivFreeState = this.CreateCommandState("ScaleDivFree");
			
			this.alignLeftState = this.CreateCommandState("AlignLeft");
			this.alignCenterXState = this.CreateCommandState("AlignCenterX");
			this.alignRightState = this.CreateCommandState("AlignRight");
			this.alignTopState = this.CreateCommandState("AlignTop");
			this.alignCenterYState = this.CreateCommandState("AlignCenterY");
			this.alignBottomState = this.CreateCommandState("AlignBottom");
			this.alignGridState = this.CreateCommandState("AlignGrid");
			
			this.shareLeftState = this.CreateCommandState("ShareLeft");
			this.shareCenterXState = this.CreateCommandState("ShareCenterX");
			this.shareSpaceXState = this.CreateCommandState("ShareSpaceX");
			this.shareRightState = this.CreateCommandState("ShareRight");
			this.shareTopState = this.CreateCommandState("ShareTop");
			this.shareCenterYState = this.CreateCommandState("ShareCenterY");
			this.shareSpaceYState = this.CreateCommandState("ShareSpaceY");
			this.shareBottomState = this.CreateCommandState("ShareBottom");
			
			this.adjustWidthState = this.CreateCommandState("AdjustWidth");
			this.adjustHeightState = this.CreateCommandState("AdjustHeight");
			
			this.colorToRgbState = this.CreateCommandState("ColorToRgb");
			this.colorToCmykState = this.CreateCommandState("ColorToCmyk");
			this.colorToGrayState = this.CreateCommandState("ColorToGray");
			this.colorStrokeDarkState = this.CreateCommandState("ColorStrokeDark");
			this.colorStrokeLightState = this.CreateCommandState("ColorStrokeLight");
			this.colorFillDarkState = this.CreateCommandState("ColorFillDark");
			this.colorFillLightState = this.CreateCommandState("ColorFillLight");
			
			this.mergeState = this.CreateCommandState("Merge");
			this.extractState = this.CreateCommandState("Extract");
			this.groupState = this.CreateCommandState("Group");
			this.ungroupState = this.CreateCommandState("Ungroup");
			this.insideState = this.CreateCommandState("Inside");
			this.outsideState = this.CreateCommandState("Outside");
			this.combineState = this.CreateCommandState("Combine");
			this.uncombineState = this.CreateCommandState("Uncombine");
			this.toBezierState = this.CreateCommandState("ToBezier");
			this.toPolyState = this.CreateCommandState("ToPoly");
			this.toTextBox2State = this.CreateCommandState("ToTextBox2", "ObjectTextBox", "ToTextBox2");
			this.fragmentState = this.CreateCommandState("Fragment");

			this.shaperHandleAddState = this.CreateCommandState("ShaperHandleAdd");
			this.shaperHandleSubState = this.CreateCommandState("ShaperHandleSub");
			this.shaperHandleToLineState = this.CreateCommandState("ShaperHandleToLine", true);
			this.shaperHandleToCurveState = this.CreateCommandState("ShaperHandleToCurve", true);
			this.shaperHandleSymState = this.CreateCommandState("ShaperHandleSym", true);
			this.shaperHandleSmoothState = this.CreateCommandState("ShaperHandleSmooth", true);
			this.shaperHandleDisState = this.CreateCommandState("ShaperHandleDis", true);
			this.shaperHandleInlineState = this.CreateCommandState("ShaperHandleInline", true);
			this.shaperHandleFreeState = this.CreateCommandState("ShaperHandleFree", true);
			this.shaperHandleSimplyState = this.CreateCommandState("ShaperHandleSimply", true);
			this.shaperHandleCornerState = this.CreateCommandState("ShaperHandleCorner", true);
			this.shaperHandleContinueState = this.CreateCommandState("ShaperHandleContinue");
			
			this.booleanAndState = this.CreateCommandState("BooleanAnd");
			this.booleanOrState = this.CreateCommandState("BooleanOr");
			this.booleanXorState = this.CreateCommandState("BooleanXor");
			this.booleanFrontMinusState = this.CreateCommandState("BooleanFrontMinus");
			this.booleanBackMinusState = this.CreateCommandState("BooleanBackMinus");
			
			this.undoState = this.CreateCommandState("Undo", KeyCode.ModifierControl|KeyCode.AlphaZ);
			this.redoState = this.CreateCommandState("Redo", KeyCode.ModifierControl|KeyCode.AlphaY);
			this.undoRedoListState = this.CreateCommandState("UndoRedoList");
			
			this.deselectAllState = this.CreateCommandState("DeselectAll", KeyCode.Escape);
			this.selectAllState = this.CreateCommandState("SelectAll", KeyCode.ModifierControl|KeyCode.AlphaA);
			this.selectInvertState = this.CreateCommandState("SelectInvert");
			this.selectorAutoState = this.CreateCommandState("SelectorAuto");
			this.selectorIndividualState = this.CreateCommandState("SelectorIndividual");
			this.selectorScalerState = this.CreateCommandState("SelectorScaler");
			this.selectorStretchState = this.CreateCommandState("SelectorStretch");
			this.selectorStretchTypeState = this.CreateCommandState("SelectorStretchType");
			this.selectTotalState = this.CreateCommandState("SelectTotal");
			this.selectPartialState = this.CreateCommandState("SelectPartial");
			this.selectorAdaptLine = this.CreateCommandState("SelectorAdaptLine");
			this.selectorAdaptText = this.CreateCommandState("SelectorAdaptText");
			
			this.hideHalfState = this.CreateCommandState("HideHalf", true);
			this.hideSelState = this.CreateCommandState("HideSel");
			this.hideRestState = this.CreateCommandState("HideRest");
			this.hideCancelState = this.CreateCommandState("HideCancel");
			
			this.zoomMinState = this.CreateCommandState("ZoomMin", true);
			this.zoomPageState = this.CreateCommandState("ZoomPage", true, KeyCode.ModifierControl|KeyCode.Digit0);
			this.zoomPageWidthState = this.CreateCommandState("ZoomPageWidth", true);
			this.zoomDefaultState = this.CreateCommandState("ZoomDefault", true);
			this.zoomSelState = this.CreateCommandState("ZoomSel");
			this.zoomSelWidthState = this.CreateCommandState("ZoomSelWidth");
			this.zoomPrevState = this.CreateCommandState("ZoomPrev");
			this.zoomSubState = this.CreateCommandState("ZoomSub", KeyCode.Substract);
			this.zoomAddState = this.CreateCommandState("ZoomAdd", KeyCode.Add);
			
			this.previewState = this.CreateCommandState("Preview", true);
			this.gridState = this.CreateCommandState("Grid", true);
			this.textGridState = this.CreateCommandState("TextGrid", true);
			this.textShowControlCharactersState = this.CreateCommandState("TextShowControlCharacters", true);
			this.textFontFilterState = this.CreateCommandState("TextFontFilter", true);
			this.textFontSampleAbcState = this.CreateCommandState("TextFontSampleAbc", true);
			this.textInsertQuadState = this.CreateCommandState("TextInsertQuad");
			this.textInsertNewFrameState = this.CreateCommandState("TextInsertNewFrame");
			this.textInsertNewPageState = this.CreateCommandState("TextInsertNewPage");
			this.magnetState = this.CreateCommandState("Magnet", true);
			this.magnetLayerState = this.CreateCommandState("MagnetLayer", true);
			this.rulersState = this.CreateCommandState("Rulers", true);
			this.labelsState = this.CreateCommandState("Labels", true);
			this.aggregatesState = this.CreateCommandState("Aggregates", true);

			this.arrayOutlineFrameState = this.CreateCommandState("ArrayOutlineFrame");
			this.arrayOutlineHorizState = this.CreateCommandState("ArrayOutlineHoriz");
			this.arrayOutlineVertiState = this.CreateCommandState("ArrayOutlineVerti");
			this.arrayAddColumnLeftState = this.CreateCommandState("ArrayAddColumnLeft");
			this.arrayAddColumnRightState = this.CreateCommandState("ArrayAddColumnRight");
			this.arrayAddRowTopState = this.CreateCommandState("ArrayAddRowTop");
			this.arrayAddRowBottomState = this.CreateCommandState("ArrayAddRowBottom");
			this.arrayDelColumnState = this.CreateCommandState("ArrayDelColumn");
			this.arrayDelRowState = this.CreateCommandState("ArrayDelRow");
			this.arrayAlignColumnState = this.CreateCommandState("ArrayAlignColumn");
			this.arrayAlignRowState = this.CreateCommandState("ArrayAlignRow");
			this.arraySwapColumnState = this.CreateCommandState("ArraySwapColumn");
			this.arraySwapRowState = this.CreateCommandState("ArraySwapRow");
			this.arrayLookState = this.CreateCommandState("ArrayLook");

			this.resDesignerBuildState = this.CreateCommandState("ResDesignerBuild");
			this.resDesignerTranslateState = this.CreateCommandState("ResDesignerTranslate");
			this.debugBboxThinState = this.CreateCommandState ("DebugBboxThin");
			this.debugBboxGeomState = this.CreateCommandState("DebugBboxGeom");
			this.debugBboxFullState = this.CreateCommandState("DebugBboxFull");
			this.debugDirtyState = this.CreateCommandState("DebugDirty", KeyCode.FuncF12);

			this.pagePrevState = this.CreateCommandState("PagePrev", KeyCode.PageUp);
			this.pageNextState = this.CreateCommandState("PageNext", KeyCode.PageDown);
			this.pageMenuState = this.CreateCommandState("PageMenu");
			this.pageNewState = this.CreateCommandState("PageNew");
			this.pageDuplicateState = this.CreateCommandState("PageDuplicate");
			this.pageUpState = this.CreateCommandState("PageUp");
			this.pageDownState = this.CreateCommandState("PageDown");
			this.pageDeleteState = this.CreateCommandState("PageDelete");

			this.layerPrevState = this.CreateCommandState("LayerPrev");
			this.layerNextState = this.CreateCommandState("LayerNext");
			this.layerMenuState = this.CreateCommandState("LayerMenu");
			this.layerNewState = this.CreateCommandState("LayerNew");
			this.layerDuplicateState = this.CreateCommandState("LayerDuplicate");
			this.layerNewSelState = this.CreateCommandState("LayerNewSel");
			this.layerMergeUpState = this.CreateCommandState("LayerMergeUp");
			this.layerMergeDownState = this.CreateCommandState("LayerMergeDown");
			this.layerUpState = this.CreateCommandState("LayerUp");
			this.layerDownState = this.CreateCommandState("LayerDown");
			this.layerDeleteState = this.CreateCommandState("LayerDelete");

			this.settingsState = this.CreateCommandState("Settings", KeyCode.FuncF5);
			this.infosState = this.CreateCommandState("Infos");
			this.aboutState = this.CreateCommandState("AboutApplication", "About", "About");
			this.pageStackState = this.CreateCommandState("PageStack");
			this.updateState = this.CreateCommandState("UpdateApplication", "Update", "Update");
			this.keyState = this.CreateCommandState("KeyApplication", "Key", "Key");

			this.moveLeftNormState   = this.CreateCommandState("MoveLeftNorm",   KeyCode.ArrowLeft);
			this.moveRightNormState  = this.CreateCommandState("MoveRightNorm",  KeyCode.ArrowRight);
			this.moveUpNormState     = this.CreateCommandState("MoveUpNorm",     KeyCode.ArrowUp);
			this.moveDownNormState   = this.CreateCommandState("MoveDownNorm",   KeyCode.ArrowDown);
			this.moveLeftCtrlState   = this.CreateCommandState("MoveLeftCtrl",   KeyCode.ModifierControl|KeyCode.ArrowLeft);
			this.moveRightCtrlState  = this.CreateCommandState("MoveRightCtrl",  KeyCode.ModifierControl|KeyCode.ArrowRight);
			this.moveUpCtrlState     = this.CreateCommandState("MoveUpCtrl",     KeyCode.ModifierControl|KeyCode.ArrowUp);
			this.moveDownCtrlState   = this.CreateCommandState("MoveDownCtrl",   KeyCode.ModifierControl|KeyCode.ArrowDown);
			this.moveLeftShiftState  = this.CreateCommandState("MoveLeftShift",  KeyCode.ModifierShift|KeyCode.ArrowLeft);
			this.moveRightShiftState = this.CreateCommandState("MoveRightShift", KeyCode.ModifierShift|KeyCode.ArrowRight);
			this.moveUpShiftState    = this.CreateCommandState("MoveUpShift",    KeyCode.ModifierShift|KeyCode.ArrowUp);
			this.moveDownShiftState  = this.CreateCommandState("MoveDownShift",  KeyCode.ModifierShift|KeyCode.ArrowDown);
		}

		protected CommandState CreateCommandState(string command, params Widgets.Shortcut[] shortcuts)
		{
			//	Crée un nouveau CommandState.
			CommandState cs = new CommandState(command, this.commandDispatcher, shortcuts);

			cs.IconName    = command;
			cs.LongCaption = DocumentEditor.GetRes("Action."+command);

			return cs;
		}

		protected CommandState CreateCommandState(string command, bool statefull, params Widgets.Shortcut[] shortcuts)
		{
			//	Crée un nouveau CommandState.
			CommandState cs = new CommandState(command, this.commandDispatcher, shortcuts);

			cs.IconName    = command;
			cs.LongCaption = DocumentEditor.GetRes("Action."+command);
			cs.Statefull   = statefull;

			return cs;
		}

		protected CommandState CreateCommandState(string command, string icon, string tooltip, params Widgets.Shortcut[] shortcuts)
		{
			//	Crée un nouveau CommandState.
			CommandState cs = new CommandState(command, this.commandDispatcher, shortcuts);

			cs.IconName    = icon;
			cs.LongCaption = DocumentEditor.GetRes("Action."+tooltip);

			return cs;
		}

		protected CommandState CreateCommandState(string command, string icon, string tooltip, bool statefull, params Widgets.Shortcut[] shortcuts)
		{
			//	Crée un nouveau CommandState.
			CommandState cs = new CommandState(command, this.commandDispatcher, shortcuts);

			cs.IconName    = icon;
			cs.LongCaption = DocumentEditor.GetRes("Action."+tooltip);
			cs.Statefull   = statefull;

			return cs;
		}


		protected void ConnectEvents()
		{
			//	On s'enregistre auprès du document pour tous les événements.
			this.CurrentDocument.Notifier.DocumentChanged        += new SimpleEventHandler(this.HandleDocumentChanged);
			this.CurrentDocument.Notifier.MouseChanged           += new SimpleEventHandler(this.HandleMouseChanged);
			this.CurrentDocument.Notifier.ModifChanged           += new SimpleEventHandler(this.HandleModifChanged);
			this.CurrentDocument.Notifier.OriginChanged          += new SimpleEventHandler(this.HandleOriginChanged);
			this.CurrentDocument.Notifier.ZoomChanged            += new SimpleEventHandler(this.HandleZoomChanged);
			this.CurrentDocument.Notifier.ToolChanged            += new SimpleEventHandler(this.HandleToolChanged);
			this.CurrentDocument.Notifier.SaveChanged            += new SimpleEventHandler(this.HandleSaveChanged);
			this.CurrentDocument.Notifier.SelectionChanged       += new SimpleEventHandler(this.HandleSelectionChanged);
			this.CurrentDocument.Notifier.ShaperChanged          += new SimpleEventHandler(this.HandleShaperChanged);
			this.CurrentDocument.Notifier.TextChanged            += new SimpleEventHandler(this.HandleTextChanged);
			this.CurrentDocument.Notifier.StyleChanged           += new SimpleEventHandler(this.HandleStyleChanged);
			this.CurrentDocument.Notifier.PagesChanged           += new SimpleEventHandler(this.HandlePagesChanged);
			this.CurrentDocument.Notifier.LayersChanged          += new SimpleEventHandler(this.HandleLayersChanged);
			this.CurrentDocument.Notifier.PageChanged            += new ObjectEventHandler(this.HandlePageChanged);
			this.CurrentDocument.Notifier.LayerChanged           += new ObjectEventHandler(this.HandleLayerChanged);
			this.CurrentDocument.Notifier.UndoRedoChanged        += new SimpleEventHandler(this.HandleUndoRedoChanged);
			this.CurrentDocument.Notifier.GridChanged            += new SimpleEventHandler(this.HandleGridChanged);
			this.CurrentDocument.Notifier.LabelPropertiesChanged += new SimpleEventHandler(this.HandleLabelPropertiesChanged);
			this.CurrentDocument.Notifier.MagnetChanged          += new SimpleEventHandler(this.HandleMagnetChanged);
			this.CurrentDocument.Notifier.PreviewChanged         += new SimpleEventHandler(this.HandlePreviewChanged);
			this.CurrentDocument.Notifier.SettingsChanged        += new SimpleEventHandler(this.HandleSettingsChanged);
			this.CurrentDocument.Notifier.FontsSettingsChanged   += new SimpleEventHandler(this.HandleFontsSettingsChanged);
			this.CurrentDocument.Notifier.GuidesChanged          += new SimpleEventHandler(this.HandleGuidesChanged);
			this.CurrentDocument.Notifier.HideHalfChanged        += new SimpleEventHandler(this.HandleHideHalfChanged);
			this.CurrentDocument.Notifier.DebugChanged           += new SimpleEventHandler(this.HandleDebugChanged);
			this.CurrentDocument.Notifier.PropertyChanged        += new PropertyEventHandler(this.HandlePropertyChanged);
			this.CurrentDocument.Notifier.AggregateChanged       += new AggregateEventHandler(this.HandleAggregateChanged);
			this.CurrentDocument.Notifier.TextStyleChanged       += new TextStyleEventHandler(this.HandleTextStyleChanged);
			this.CurrentDocument.Notifier.TextStyleListChanged   += new SimpleEventHandler(this.HandleTextStyleListChanged);
			this.CurrentDocument.Notifier.SelNamesChanged        += new SimpleEventHandler(this.HandleSelNamesChanged);
			this.CurrentDocument.Notifier.DrawChanged            += new RedrawEventHandler(this.HandleDrawChanged);
			this.CurrentDocument.Notifier.RibbonCommand          += new RibbonEventHandler(this.HandleRibbonCommand);
			this.CurrentDocument.Notifier.BookPanelShowPage      += new BookPanelEventHandler(this.HandleBookPanelShowPage);
			this.CurrentDocument.Notifier.SettingsShowPage       += new SettingsEventHandler(this.HandleSettingsShowPage);
		}

		private void HandleDocumentChanged()
		{
			//	Appelé par le document lorsque les informations sur le document ont changé.
			if ( this.IsCurrentDocument )
			{
				this.printState.Enable = true;
				this.exportState.Enable = true;
				this.infosState.Enable = true;
				this.pageStackState.Enable = true;

				this.CurrentDocument.Dialogs.UpdateInfos();
				this.CurrentDocument.Dialogs.UpdateFonts();
				this.CurrentDocument.Wrappers.UpdateCommands();
				this.UpdateBookDocuments();
			}
			else
			{
				this.printState.Enable = false;
				this.exportState.Enable = false;
				this.infosState.Enable = false;
				this.pageStackState.Enable = false;
			}

			this.BuildLastFilenamesMenu();
		}

		private void HandleMouseChanged()
		{
			//	Appelé par le document lorsque la position de la souris a changé.
			//	TODO: [PA] Parfois, this.info.Items est nul après avoir cliqué la case de fermeture de la fenêtre !
			
			System.Diagnostics.Debug.Assert (this.info.Items != null);
			
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

		private void HandleModifChanged()
		{
			//	Appelé par le document lorsque le texte des modifications a changé.
			//	TODO: [PA] Parfois, this.info.Items est nul après avoir cliqué la case de fermeture de la fenêtre !
			if ( this.info.Items == null )  return;

			StatusField field = this.info.Items["StatusModif"] as StatusField;
			field.Text = this.TextInfoModif;
			field.Invalidate();
		}

		private void HandleOriginChanged()
		{
			//	Appelé par le document lorsque l'origine a changé.
			this.UpdateScroller();

			if ( this.IsCurrentDocument )
			{
				DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
				this.zoomPageState.Enable = true;
				this.zoomPageWidthState.Enable = true;
				this.zoomDefaultState.Enable = true;
				this.zoomPageState.ActiveState = context.IsZoomPage ? Widgets.ActiveState.Yes : Widgets.ActiveState.No;
				this.zoomPageWidthState.ActiveState = context.IsZoomPageWidth ? Widgets.ActiveState.Yes : Widgets.ActiveState.No;
				this.zoomDefaultState.ActiveState = context.IsZoomDefault ? Widgets.ActiveState.Yes : Widgets.ActiveState.No;
			}
			else
			{
				this.zoomPageState.Enable = false;
				this.zoomPageWidthState.Enable = false;
				this.zoomDefaultState.Enable = false;
			}
		}

		private void HandleZoomChanged()
		{
			//	Appelé par le document lorsque le zoom a changé.
			this.UpdateScroller();

			if ( this.IsCurrentDocument )
			{
				DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
				this.zoomMinState.Enable = true;
				this.zoomPageState.Enable = true;
				this.zoomPageWidthState.Enable = true;
				this.zoomDefaultState.Enable = true;
				this.zoomMinState.ActiveState = ( context.Zoom <= this.CurrentDocument.Modifier.ZoomMin ) ? Widgets.ActiveState.Yes : Widgets.ActiveState.No;
				this.zoomPageState.ActiveState = context.IsZoomPage ? Widgets.ActiveState.Yes : Widgets.ActiveState.No;
				this.zoomPageWidthState.ActiveState = context.IsZoomPageWidth ? Widgets.ActiveState.Yes : Widgets.ActiveState.No;
				this.zoomDefaultState.ActiveState = context.IsZoomDefault ? Widgets.ActiveState.Yes : Widgets.ActiveState.No;
				this.zoomPrevState.Enable = ( this.CurrentDocument.Modifier.ZoomMemorizeCount > 0 );
				this.zoomSubState.Enable = ( context.Zoom > this.CurrentDocument.Modifier.ZoomMin );
				this.zoomAddState.Enable = ( context.Zoom < this.CurrentDocument.Modifier.ZoomMax );
			}
			else
			{
				this.zoomMinState.Enable = false;
				this.zoomPageState.Enable = false;
				this.zoomPageWidthState.Enable = false;
				this.zoomDefaultState.Enable = false;
				this.zoomMinState.ActiveState = Widgets.ActiveState.No;
				this.zoomPageState.ActiveState = Widgets.ActiveState.No;
				this.zoomPageWidthState.ActiveState = Widgets.ActiveState.No;
				this.zoomDefaultState.ActiveState = Widgets.ActiveState.No;
				this.zoomPrevState.Enable = false;
				this.zoomSubState.Enable = false;
				this.zoomAddState.Enable = false;
			}

			StatusField field = this.info.Items["StatusZoom"] as StatusField;
			field.Text = this.TextInfoZoom;
			field.Invalidate();

			HSlider slider = this.info.Items["StatusZoomSlider"] as HSlider;
			slider.Value = (decimal) this.ValueInfoZoom;
			slider.Enable = this.IsCurrentDocument;
		}

		protected void UpdateTool(CommandState cs, string currentTool, bool isCreating, bool enabled)
		{
			//	Met à jour une commande d'outil.
			string tool = cs.Name;

			if ( enabled )
			{
				cs.ActiveState = (tool == currentTool) ? ActiveState.Yes : ActiveState.No;;
				cs.Enable = (tool == currentTool || tool == "ToolSelect" || tool == "ToolShaper" || !isCreating);
			}
			else
			{
				cs.ActiveState = ActiveState.No;
				cs.Enable = false;
			}
		}

		private void HandleToolChanged()
		{
			//	Appelé par le document lorsque l'outil a changé.
			string tool = "";
			bool isCreating = false;
			bool enabled = false;

			if ( this.IsCurrentDocument )
			{
				tool = this.CurrentDocument.Modifier.Tool;
				isCreating = this.CurrentDocument.Modifier.ActiveViewer.IsCreating;
				enabled = true;
			}

			this.UpdateTool(this.toolSelectState, tool, isCreating, enabled);
			this.UpdateTool(this.toolGlobalState, tool, isCreating, enabled);
			this.UpdateTool(this.toolShaperState, tool, isCreating, enabled);
			this.UpdateTool(this.toolEditState, tool, isCreating, enabled);
			this.UpdateTool(this.toolZoomState, tool, isCreating, enabled);
			this.UpdateTool(this.toolHandState, tool, isCreating, enabled);
			this.UpdateTool(this.toolPickerState, tool, isCreating, enabled);
			this.UpdateTool(this.toolHotSpotState, tool, isCreating, enabled);
			this.UpdateTool(this.toolLineState, tool, isCreating, enabled);
			this.UpdateTool(this.toolRectangleState, tool, isCreating, enabled);
			this.UpdateTool(this.toolCircleState, tool, isCreating, enabled);
			this.UpdateTool(this.toolEllipseState, tool, isCreating, enabled);
			this.UpdateTool(this.toolPolyState, tool, isCreating, enabled);
			this.UpdateTool(this.toolBezierState, tool, isCreating, enabled);
			this.UpdateTool(this.toolRegularState, tool, isCreating, enabled);
			this.UpdateTool(this.toolSurfaceState, tool, isCreating, enabled);
			this.UpdateTool(this.toolVolumeState, tool, isCreating, enabled);
			this.UpdateTool(this.toolTextLineState, tool, isCreating, enabled);
			this.UpdateTool(this.toolTextLine2State, tool, isCreating, enabled);
			this.UpdateTool(this.toolTextBoxState, tool, isCreating, enabled);
			this.UpdateTool(this.toolTextBox2State, tool, isCreating, enabled);
			this.UpdateTool(this.toolArrayState, tool, isCreating, enabled);
			this.UpdateTool(this.toolImageState, tool, isCreating, enabled);
			this.UpdateTool(this.toolDimensionState, tool, isCreating, enabled);
		}

		private void HandleSaveChanged()
		{
			//	Appelé par le document lorsque l'état "enregistrer" a changé.
			if ( this.IsCurrentDocument )
			{
				bool isCreating = this.CurrentDocument.Modifier.ActiveViewer.IsCreating;
				this.saveState.Enable = !isCreating && this.CurrentDocument.IsDirtySerialize;
				this.saveAsState.Enable = !isCreating;
				this.saveModelState.Enable = !isCreating;
				this.UpdateBookDocuments();
			}
			else
			{
				this.saveState.Enable = false;
				this.saveAsState.Enable = false;
				this.saveModelState.Enable = false;
			}
		}

		private void HandleSelectionChanged()
		{
			//	Appelé par le document lorsque la sélection a changé.
			if ( this.IsCurrentDocument )
			{
				DocumentInfo di = this.CurrentDocumentInfo;

				di.containerPrincipal.SetDirtyContent();
				di.containerStyles.SetDirtyContent();

				if ( di.containerAutos != null )
				{
					di.containerAutos.SetDirtyContent();
				}

				Viewer viewer = this.CurrentDocument.Modifier.ActiveViewer;
				int totalSelected  = this.CurrentDocument.Modifier.TotalSelected;
				int totalHide      = this.CurrentDocument.Modifier.TotalHide;
				int totalPageHide  = this.CurrentDocument.Modifier.TotalPageHide;
				int totalObjects   = this.CurrentDocument.Modifier.TotalObjects;
				bool isCreating    = this.CurrentDocument.Modifier.ActiveViewer.IsCreating;
				bool isBase        = viewer.DrawingContext.RootStackIsBase;
				bool isEdit        = this.CurrentDocument.Modifier.IsToolEdit;
				SelectorType sType = viewer.SelectorType;
				Objects.Abstract one = this.CurrentDocument.Modifier.RetOnlySelectedObject();

				bool isOldText = false;
				if ( this.CurrentDocument.ContainOldText )
				{
					isOldText = this.CurrentDocument.Modifier.IsSelectedOldText();
				}

				this.newState.Enable = true;
				this.openState.Enable = true;
				this.openModelState.Enable = true;
				this.deleteState.Enable = ( totalSelected > 0 || isCreating );
				this.duplicateState.Enable = ( totalSelected > 0 && !isCreating );
				this.orderUpOneState.Enable = ( totalObjects > 1 && totalSelected > 0 && !isCreating && !isEdit );
				this.orderDownOneState.Enable = ( totalObjects > 1 && totalSelected > 0 && !isCreating && !isEdit );
				this.orderUpAllState.Enable = ( totalObjects > 1 && totalSelected > 0 && !isCreating && !isEdit );
				this.orderDownAllState.Enable = ( totalObjects > 1 && totalSelected > 0 && !isCreating && !isEdit );
				this.moveLeftFreeState.Enable = ( totalSelected > 0 && !isCreating && !isEdit );
				this.moveRightFreeState.Enable = ( totalSelected > 0 && !isCreating && !isEdit );
				this.moveUpFreeState.Enable = ( totalSelected > 0 && !isCreating && !isEdit );
				this.moveDownFreeState.Enable = ( totalSelected > 0 && !isCreating && !isEdit );
				this.rotate90State.Enable = ( totalSelected > 0 && !isCreating && !isEdit );
				this.rotate180State.Enable = ( totalSelected > 0 && !isCreating && !isEdit );
				this.rotate270State.Enable = ( totalSelected > 0 && !isCreating && !isEdit );
				this.rotateFreeCCWState.Enable = ( totalSelected > 0 && !isCreating && !isEdit );
				this.rotateFreeCWState.Enable = ( totalSelected > 0 && !isCreating && !isEdit );
				this.mirrorHState.Enable = ( totalSelected > 0 && !isCreating && !isEdit );
				this.mirrorVState.Enable = ( totalSelected > 0 && !isCreating && !isEdit );
				this.scaleMul2State.Enable = ( totalSelected > 0 && !isCreating && !isEdit );
				this.scaleDiv2State.Enable = ( totalSelected > 0 && !isCreating && !isEdit );
				this.scaleMulFreeState.Enable = ( totalSelected > 0 && !isCreating && !isEdit );
				this.scaleDivFreeState.Enable = ( totalSelected > 0 && !isCreating && !isEdit );
				this.alignLeftState.Enable = ( totalSelected > 1 && !isCreating && !isEdit );
				this.alignCenterXState.Enable = ( totalSelected > 1 && !isCreating && !isEdit );
				this.alignRightState.Enable = ( totalSelected > 1 && !isCreating && !isEdit );
				this.alignTopState.Enable = ( totalSelected > 1 && !isCreating && !isEdit );
				this.alignCenterYState.Enable = ( totalSelected > 1 && !isCreating && !isEdit );
				this.alignBottomState.Enable = ( totalSelected > 1 && !isCreating && !isEdit );
				this.alignGridState.Enable = ( totalSelected > 0 && !isCreating && !isEdit );
				this.shareLeftState.Enable = ( totalSelected > 2 && !isCreating && !isEdit );
				this.shareCenterXState.Enable = ( totalSelected > 2 && !isCreating && !isEdit );
				this.shareSpaceXState.Enable = ( totalSelected > 2 && !isCreating && !isEdit );
				this.shareRightState.Enable = ( totalSelected > 2 && !isCreating && !isEdit );
				this.shareTopState.Enable = ( totalSelected > 2 && !isCreating && !isEdit );
				this.shareCenterYState.Enable = ( totalSelected > 2 && !isCreating && !isEdit );
				this.shareSpaceYState.Enable = ( totalSelected > 2 && !isCreating && !isEdit );
				this.shareBottomState.Enable = ( totalSelected > 2 && !isCreating && !isEdit );
				this.adjustWidthState.Enable = ( totalSelected > 1 && !isCreating && !isEdit );
				this.adjustHeightState.Enable = ( totalSelected > 1 && !isCreating && !isEdit );
				this.colorToRgbState.Enable = ( totalSelected > 0 && !isCreating && !isEdit );
				this.colorToCmykState.Enable = ( totalSelected > 0 && !isCreating && !isEdit );
				this.colorToGrayState.Enable = ( totalSelected > 0 && !isCreating && !isEdit );
				this.colorStrokeDarkState.Enable = ( totalSelected > 0 && !isCreating && !isEdit );
				this.colorStrokeLightState.Enable = ( totalSelected > 0 && !isCreating && !isEdit );
				this.colorFillDarkState.Enable = ( totalSelected > 0 && !isCreating && !isEdit );
				this.colorFillLightState.Enable = ( totalSelected > 0 && !isCreating && !isEdit );
				this.mergeState.Enable = ( totalSelected > 1 && !isCreating && !isEdit );
				this.extractState.Enable = ( totalSelected > 0 && !isBase && !isCreating && !isEdit );
				this.groupState.Enable = ( totalSelected > 1 && !isCreating && !isEdit );
				this.ungroupState.Enable = ( totalSelected == 1 && one is Objects.Group && !isCreating && !isEdit );
				this.insideState.Enable = ( totalSelected == 1 && one is Objects.Group && !isCreating && !isEdit );
				this.outsideState.Enable = ( !isBase && !isCreating );
				this.combineState.Enable = ( totalSelected > 1 && !isCreating && !isEdit );
				this.uncombineState.Enable = ( totalSelected > 0 && !isCreating && !isEdit );
				this.toBezierState.Enable = ( totalSelected > 0 && !isCreating && !isEdit );
				this.toPolyState.Enable = ( totalSelected > 0 && !isCreating && !isEdit );
				this.toTextBox2State.Enable = ( totalSelected > 0 && !isCreating && !isEdit && isOldText );
				this.fragmentState.Enable = ( totalSelected > 0 && !isCreating && !isEdit );
				this.booleanAndState.Enable = ( totalSelected > 1 && !isCreating && !isEdit );
				this.booleanOrState.Enable = ( totalSelected > 1 && !isCreating && !isEdit );
				this.booleanXorState.Enable = ( totalSelected > 1 && !isCreating && !isEdit );
				this.booleanFrontMinusState.Enable = ( totalSelected > 1 && !isCreating && !isEdit );
				this.booleanBackMinusState.Enable = ( totalSelected > 1 && !isCreating && !isEdit );
				this.layerNewSelState.Enable = ( totalSelected > 0 && !isCreating );

				this.hideSelState.Enable = ( totalSelected > 0 && !isCreating );
				this.hideRestState.Enable = ( totalSelected > 0 && totalObjects-totalSelected-totalHide > 0 && !isCreating );
				this.hideCancelState.Enable = ( totalPageHide > 0 && !isCreating );

				this.zoomSelState.Enable = ( totalSelected > 0 );
				this.zoomSelWidthState.Enable = ( totalSelected > 0 );

				this.deselectAllState.Enable = ( totalSelected > 0 );
				this.selectAllState.Enable = ( totalSelected < totalObjects-totalHide );
				this.selectInvertState.Enable = ( totalSelected > 0 && totalSelected < totalObjects-totalHide );

				this.selectorAutoState.Enable        = true;
				this.selectorIndividualState.Enable  = true;
				this.selectorScalerState.Enable      = true;
				this.selectorStretchState.Enable     = true;
				this.selectorStretchTypeState.Enable = true;
				this.selectorAutoState.ActiveState       = (sType == SelectorType.Auto      ) ? ActiveState.Yes : ActiveState.No;
				this.selectorIndividualState.ActiveState = (sType == SelectorType.Individual) ? ActiveState.Yes : ActiveState.No;
				this.selectorScalerState.ActiveState     = (sType == SelectorType.Scaler    ) ? ActiveState.Yes : ActiveState.No;
				this.selectorStretchState.ActiveState    = (sType == SelectorType.Stretcher ) ? ActiveState.Yes : ActiveState.No;
				di.containerPrincipal.UpdateSelectorStretch();

				this.selectTotalState.Enable   = true;
				this.selectPartialState.Enable = true;
				this.selectTotalState.ActiveState   = !viewer.PartialSelect ? ActiveState.Yes : ActiveState.No;
				this.selectPartialState.ActiveState =  viewer.PartialSelect ? ActiveState.Yes : ActiveState.No;

				this.selectorAdaptLine.Enable = true;
				this.selectorAdaptText.Enable = true;
				this.selectorAdaptLine.ActiveState = viewer.SelectorAdaptLine ? ActiveState.Yes : ActiveState.No;
				this.selectorAdaptText.ActiveState = viewer.SelectorAdaptText ? ActiveState.Yes : ActiveState.No;

				if ( !this.CurrentDocument.Wrappers.IsWrappersAttached )  // pas édition en cours ?
				{
					this.cutState.Enable = ( totalSelected > 0 && !isCreating );
					this.copyState.Enable = ( totalSelected > 0 && !isCreating );
					this.pasteState.Enable = ( !this.CurrentDocument.Modifier.IsClipboardEmpty() && !isCreating );
				}

				this.CurrentDocument.Dialogs.UpdateInfos();
			}
			else
			{
				this.newState.Enable = true;
				this.openState.Enable = true;
				this.openModelState.Enable = true;
				this.deleteState.Enable = false;
				this.duplicateState.Enable = false;
				this.cutState.Enable = false;
				this.copyState.Enable = false;
				this.pasteState.Enable = false;
				this.glyphsState.Enable = false;
				this.glyphsInsertState.Enable = false;
				this.textEditingState.Enable = false;
				this.textShowControlCharactersState.Enable = false;
				this.textFontFilterState.Enable = false;
				this.textFontSampleAbcState.Enable = false;
				this.textInsertQuadState.Enable = false;
				this.textInsertNewFrameState.Enable = false;
				this.textInsertNewPageState.Enable = false;
				this.fontBoldState.Enable = false;
				this.fontItalicState.Enable = false;
				this.fontUnderlinedState.Enable = false;
				this.fontOverlinedState.Enable = false;
				this.fontStrikeoutState.Enable = false;
				this.fontSubscriptState.Enable = false;
				this.fontSuperscriptState.Enable = false;
				this.fontSizePlusState.Enable = false;
				this.fontSizeMinusState.Enable = false;
				this.fontClearState.Enable = false;
				this.paragraphLeadingState.Enable = false;
				this.paragraphLeadingPlusState.Enable = false;
				this.paragraphLeadingMinusState.Enable = false;
				this.paragraphIndentPlusState.Enable = false;
				this.paragraphIndentMinusState.Enable = false;
				this.paragraphJustifState.Enable = false;
				this.paragraphClearState.Enable = false;
				this.orderUpOneState.Enable = false;
				this.orderDownOneState.Enable = false;
				this.orderUpAllState.Enable = false;
				this.orderDownAllState.Enable = false;
				this.moveLeftFreeState.Enable = false;
				this.moveRightFreeState.Enable = false;
				this.moveUpFreeState.Enable = false;
				this.moveDownFreeState.Enable = false;
				this.rotate90State.Enable = false;
				this.rotate180State.Enable = false;
				this.rotate270State.Enable = false;
				this.rotateFreeCCWState.Enable = false;
				this.rotateFreeCWState.Enable = false;
				this.mirrorHState.Enable = false;
				this.mirrorVState.Enable = false;
				this.scaleMul2State.Enable = false;
				this.scaleDiv2State.Enable = false;
				this.scaleMulFreeState.Enable = false;
				this.scaleDivFreeState.Enable = false;
				this.alignLeftState.Enable = false;
				this.alignCenterXState.Enable = false;
				this.alignRightState.Enable = false;
				this.alignTopState.Enable = false;
				this.alignCenterYState.Enable = false;
				this.alignBottomState.Enable = false;
				this.alignGridState.Enable = false;
				this.shareLeftState.Enable = false;
				this.shareCenterXState.Enable = false;
				this.shareSpaceXState.Enable = false;
				this.shareRightState.Enable = false;
				this.shareTopState.Enable = false;
				this.shareCenterYState.Enable = false;
				this.shareSpaceYState.Enable = false;
				this.shareBottomState.Enable = false;
				this.adjustWidthState.Enable = false;
				this.adjustHeightState.Enable = false;
				this.colorToRgbState.Enable = false;
				this.colorToCmykState.Enable = false;
				this.colorToGrayState.Enable = false;
				this.colorStrokeDarkState.Enable = false;
				this.colorStrokeLightState.Enable = false;
				this.colorFillDarkState.Enable = false;
				this.colorFillLightState.Enable = false;
				this.mergeState.Enable = false;
				this.extractState.Enable = false;
				this.groupState.Enable = false;
				this.ungroupState.Enable = false;
				this.insideState.Enable = false;
				this.outsideState.Enable = false;
				this.combineState.Enable = false;
				this.uncombineState.Enable = false;
				this.toBezierState.Enable = false;
				this.toPolyState.Enable = false;
				this.toTextBox2State.Enable = false;
				this.fragmentState.Enable = false;
				this.booleanAndState.Enable = false;
				this.booleanOrState.Enable = false;
				this.booleanXorState.Enable = false;
				this.booleanFrontMinusState.Enable = false;
				this.booleanBackMinusState.Enable = false;
				this.layerNewSelState.Enable = false;

				this.hideSelState.Enable = false;
				this.hideRestState.Enable = false;
				this.hideCancelState.Enable = false;

				this.zoomSelState.Enable = false;
				this.zoomSelWidthState.Enable = false;

				this.deselectAllState.Enable = false;
				this.selectAllState.Enable = false;
				this.selectInvertState.Enable = false;

				this.selectorAutoState.Enable        = false;
				this.selectorIndividualState.Enable  = false;
				this.selectorScalerState.Enable      = false;
				this.selectorStretchState.Enable     = false;
				this.selectorStretchTypeState.Enable = false;
				this.selectorAutoState.ActiveState       = ActiveState.No;
				this.selectorIndividualState.ActiveState = ActiveState.No;
				this.selectorScalerState.ActiveState     = ActiveState.No;
				this.selectorStretchState.ActiveState    = ActiveState.No;

				this.selectTotalState.Enable   = false;
				this.selectPartialState.Enable = false;
				this.selectTotalState.ActiveState   = ActiveState.No;
				this.selectPartialState.ActiveState = ActiveState.No;

				this.selectorAdaptLine.Enable = false;
				this.selectorAdaptText.Enable = false;
				this.selectorAdaptLine.ActiveState = ActiveState.No;
				this.selectorAdaptText.ActiveState = ActiveState.No;
			}

			this.dlgGlyphs.SetAlternatesDirty();

			StatusField field = this.info.Items["StatusObject"] as StatusField;
			field.Text = this.TextInfoObject;
			field.Invalidate();
		}

		private void HandleShaperChanged()
		{
			//	Appelé par le document lorsque le modeleur a changé.
			if ( this.IsCurrentDocument &&
				 this.CurrentDocument.Modifier.IsToolShaper &&
				 this.CurrentDocument.Modifier.TotalSelected != 0 )
			{
				this.CurrentDocument.Modifier.ShaperHandleUpdate(this.CommandDispatcher);
			}
			else
			{
				this.shaperHandleAddState.Enable = false;
				this.shaperHandleSubState.Enable = false;
				this.shaperHandleToLineState.Enable = false;
				this.shaperHandleToCurveState.Enable = false;
				this.shaperHandleSymState.Enable = false;
				this.shaperHandleSmoothState.Enable = false;
				this.shaperHandleDisState.Enable = false;
				this.shaperHandleInlineState.Enable = false;
				this.shaperHandleFreeState.Enable = false;
				this.shaperHandleSimplyState.Enable = false;
				this.shaperHandleCornerState.Enable = false;
				this.shaperHandleContinueState.Enable = false;
			}
		}

		private void HandleTextChanged()
		{
			//	Appelé par le document lorsque le texte en édition a changé.
			this.ribbonMain.NotifyTextStylesChanged();
			this.ribbonGeom.NotifyTextStylesChanged();
			this.ribbonOper.NotifyTextStylesChanged();
			this.ribbonText.NotifyTextStylesChanged();

			this.dlgGlyphs.SetAlternatesDirty();
		}

		private void HandleStyleChanged()
		{
			//	Appelé par le document lorsqu'un style a changé.
			if ( !this.IsCurrentDocument )  return;
			DocumentInfo di = this.CurrentDocumentInfo;
			di.containerStyles.SetDirtyContent();
		}

		private void HandlePagesChanged()
		{
			//	Appelé par le document lorsque les pages ont changé.
			if ( this.IsCurrentDocument )
			{
				DocumentInfo di = this.CurrentDocumentInfo;
				di.containerPages.SetDirtyContent();

				DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
				int cp = context.CurrentPage;
				int tp = context.TotalPages();

				bool isCreating = this.CurrentDocument.Modifier.ActiveViewer.IsCreating;

				this.pagePrevState.Enable = (cp > 0 && !isCreating );
				this.pageNextState.Enable = (cp < tp-1 && !isCreating );
				this.pageMenuState.Enable = (tp > 1 && !isCreating );
				this.pageNewState.Enable = !isCreating;
				this.pageDuplicateState.Enable = !isCreating;
				this.pageUpState.Enable = (cp > 0 && !isCreating );
				this.pageDownState.Enable = (cp < tp-1 && !isCreating );
				this.pageDeleteState.Enable = (tp > 1 && !isCreating );

				Objects.Page page = this.CurrentDocument.GetObjects[cp] as Objects.Page;
				this.CurrentDocumentInfo.quickPageMenu.Text = page.ShortName;

				this.dlgPageStack.Update();
				this.dlgPrint.UpdatePages();
				this.dlgExportPDF.UpdatePages();
				this.HandleModifChanged();
			}
			else
			{
				this.pagePrevState.Enable = false;
				this.pageNextState.Enable = false;
				this.pageMenuState.Enable = false;
				this.pageNewState.Enable = false;
				this.pageDuplicateState.Enable = false;
				this.pageUpState.Enable = false;
				this.pageDownState.Enable = false;
				this.pageDeleteState.Enable = false;

				this.dlgPageStack.Update();
			}
		}

		private void HandleLayersChanged()
		{
			//	Appelé par le document lorsque les calques ont changé.
			if ( this.IsCurrentDocument )
			{
				DocumentInfo di = this.CurrentDocumentInfo;
				di.containerLayers.SetDirtyContent();

				DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
				int cl = context.CurrentLayer;
				int tl = context.TotalLayers();

				bool isCreating = this.CurrentDocument.Modifier.ActiveViewer.IsCreating;

				this.layerPrevState.Enable = (cl > 0 && !isCreating );
				this.layerNextState.Enable = (cl < tl-1 && !isCreating );
				this.layerMenuState.Enable = (tl > 1 && !isCreating );
				this.layerNewState.Enable = !isCreating ;
				this.layerDuplicateState.Enable = !isCreating ;
				this.layerMergeUpState.Enable = (cl < tl-1 && !isCreating );
				this.layerMergeDownState.Enable = (cl > 0 && !isCreating );
				this.layerUpState.Enable = (cl < tl-1 && !isCreating );
				this.layerDownState.Enable = (cl > 0 && !isCreating );
				this.layerDeleteState.Enable = (tl > 1 && !isCreating );

				bool ml = this.CurrentDocument.Modifier.MagnetLayerState(cl);
				this.magnetLayerState.Enable = true;
				this.magnetLayerState.ActiveState = ml ? ActiveState.Yes : ActiveState.No;

				this.CurrentDocumentInfo.quickLayerMenu.Text = Objects.Layer.ShortName(cl);
				this.dlgPageStack.Update();
				this.HandleModifChanged();
			}
			else
			{
				this.layerPrevState.Enable = false;
				this.layerNextState.Enable = false;
				this.layerMenuState.Enable = false;
				this.layerNewState.Enable = false;
				this.layerDuplicateState.Enable = false;
				this.layerMergeUpState.Enable = false;
				this.layerMergeDownState.Enable = false;
				this.layerUpState.Enable = false;
				this.layerDownState.Enable = false;
				this.layerDeleteState.Enable = false;

				this.magnetLayerState.Enable = false;
				this.magnetLayerState.ActiveState = ActiveState.No;
			}
		}

		private void HandlePageChanged(Objects.Abstract page)
		{
			//	Appelé par le document lorsqu'un nom de page a changé.
			if ( !this.IsCurrentDocument )  return;
			DocumentInfo di = this.CurrentDocumentInfo;
			di.containerPages.SetDirtyObject(page);
			this.HandleModifChanged();
		}

		private void HandleLayerChanged(Objects.Abstract layer)
		{
			//	Appelé par le document lorsqu'un nom de calque a changé.
			if ( !this.IsCurrentDocument )  return;
			DocumentInfo di = this.CurrentDocumentInfo;
			di.containerLayers.SetDirtyObject(layer);
			this.HandleModifChanged();
		}

		private void HandleUndoRedoChanged()
		{
			//	Appelé par le document lorsque l'état des commande undo/redo a changé.
			if ( this.IsCurrentDocument )
			{
				bool isCreating = this.CurrentDocument.Modifier.ActiveViewer.IsCreating;
				this.undoState.Enable = ( this.CurrentDocument.Modifier.OpletQueue.CanUndo && !isCreating );
				this.redoState.Enable = ( this.CurrentDocument.Modifier.OpletQueue.CanRedo && !isCreating );
				this.undoRedoListState.Enable = this.undoState.Enable|this.redoState.Enable;
			}
			else
			{
				this.undoState.Enable = false;
				this.redoState.Enable = false;
				this.undoRedoListState.Enable = false;
			}
		}

		private void HandleGridChanged()
		{
			//	Appelé par le document lorsque l'état de la grille a changé.
			if ( this.IsCurrentDocument )
			{
				DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
				
				this.gridState.Enable = true;
				this.gridState.ActiveState = context.GridActive ? ActiveState.Yes : ActiveState.No;
				
				this.textGridState.Enable = true;
				this.textGridState.ActiveState = context.TextGridShow ? ActiveState.Yes : ActiveState.No;
				
				this.rulersState.Enable = true;
				this.rulersState.ActiveState = context.RulersShow ? ActiveState.Yes : ActiveState.No;
				
				this.labelsState.Enable = true;
				this.labelsState.ActiveState = context.LabelsShow ? ActiveState.Yes : ActiveState.No;
				
				this.aggregatesState.Enable = true;
				this.aggregatesState.ActiveState = context.AggregatesShow ? ActiveState.Yes : ActiveState.No;

				this.textShowControlCharactersState.Enable = this.CurrentDocument.Wrappers.IsWrappersAttached;
				this.textShowControlCharactersState.ActiveState = context.TextShowControlCharacters ? ActiveState.Yes : ActiveState.No;
			}
			else
			{
				this.gridState.Enable = false;
				this.gridState.ActiveState = ActiveState.No;
				
				this.textGridState.Enable = false;
				this.textGridState.ActiveState = ActiveState.No;
				
				this.rulersState.Enable = false;
				this.rulersState.ActiveState = ActiveState.No;
				
				this.labelsState.Enable = false;
				this.labelsState.ActiveState = ActiveState.No;
				
				this.aggregatesState.Enable = false;
				this.aggregatesState.ActiveState = ActiveState.No;

				this.textShowControlCharactersState.Enable = false;
				this.textShowControlCharactersState.ActiveState = ActiveState.No;
			}
		}

		private void HandleLabelPropertiesChanged()
		{
			//	Appelé par le document lorsque l'état des noms d'attributs a changé.
			if ( this.IsCurrentDocument )
			{
				DocumentInfo di = this.CurrentDocumentInfo;
				di.containerPrincipal.SetDirtyContent();
				di.containerStyles.SetDirtyContent();
				di.containerPages.SetDirtyContent();
				di.containerLayers.SetDirtyContent();
			}
		}

		private void HandleMagnetChanged()
		{
			//	Appelé par le document lorsque l'état des lignes magnétiques a changé.
			if ( this.IsCurrentDocument )
			{
				DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
				System.Collections.ArrayList layers = context.MagnetLayerList;
				if ( layers.Count == 0 )
				{
					this.magnetState.Enable = false;
					this.magnetState.ActiveState = ActiveState.No;
				}
				else
				{
					this.magnetState.Enable = true;
					this.magnetState.ActiveState = context.MagnetActive ? ActiveState.Yes : ActiveState.No;
				}
			}
			else
			{
				this.magnetState.Enable = false;
				this.magnetState.ActiveState = ActiveState.No;
			}
		}

		private void HandlePreviewChanged()
		{
			//	Appelé par le document lorsque l'état de l'aperçu a changé.
			if ( this.IsCurrentDocument )
			{
				DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
				this.previewState.Enable = true;
				this.previewState.ActiveState = context.PreviewActive ? ActiveState.Yes : ActiveState.No;
			}
			else
			{
				this.previewState.Enable = false;
				this.previewState.ActiveState = ActiveState.No;
			}
		}

		private void HandleSettingsChanged()
		{
			//	Appelé par le document lorsque les réglages ont changé.
			if ( this.IsCurrentDocument )
			{
				this.CurrentDocument.Dialogs.UpdateAllSettings();
			}
		}

		private void HandleFontsSettingsChanged()
		{
			//	Appelé par le document lorsque les réglages de police ont changés.
			if ( this.IsCurrentDocument )
			{
				DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
				
				this.textFontFilterState.Enable = true;
				this.textFontFilterState.ActiveState = context.TextFontFilter ? ActiveState.Yes : ActiveState.No;
				
				this.textFontSampleAbcState.Enable = true;
				this.textFontSampleAbcState.ActiveState = context.TextFontSampleAbc ? ActiveState.Yes : ActiveState.No;

				this.CurrentDocument.Dialogs.UpdateFonts();
			}
			else
			{
				this.textFontFilterState.Enable = false;
				this.textFontFilterState.ActiveState = ActiveState.No;
				
				this.textFontSampleAbcState.Enable = false;
				this.textFontSampleAbcState.ActiveState = ActiveState.No;
			}
				
			this.ribbonMain.NotifyChanged("FontsSettingsChanged");
			this.ribbonGeom.NotifyChanged("FontsSettingsChanged");
			this.ribbonOper.NotifyChanged("FontsSettingsChanged");
			this.ribbonText.NotifyChanged("FontsSettingsChanged");
		}

		private void HandleGuidesChanged()
		{
			//	Appelé par le document lorsque les repères ont changé.
			if ( this.IsCurrentDocument )
			{
				this.CurrentDocument.Dialogs.UpdateGuides();
			}
		}

		private void HandleHideHalfChanged()
		{
			//	Appelé par le document lorsque l'état de la commande "hide half" a changé.
			if ( this.IsCurrentDocument )
			{
				DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
				this.hideHalfState.Enable = true;
				this.hideHalfState.ActiveState = context.HideHalfActive ? ActiveState.Yes : ActiveState.No;
			}
			else
			{
				this.hideHalfState.Enable = false;
				this.hideHalfState.ActiveState = ActiveState.No;
			}
		}

		private void HandleDebugChanged()
		{
			//	Appelé par le document lorsque l'état des commande de debug a changé.
			if ( this.IsCurrentDocument )
			{
				DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
				this.debugBboxThinState.Enable = true;
				this.debugBboxGeomState.Enable = true;
				this.debugBboxFullState.Enable = true;
				this.debugBboxThinState.ActiveState = context.IsDrawBoxThin ? ActiveState.Yes : ActiveState.No;
				this.debugBboxGeomState.ActiveState = context.IsDrawBoxGeom ? ActiveState.Yes : ActiveState.No;
				this.debugBboxFullState.ActiveState = context.IsDrawBoxFull ? ActiveState.Yes : ActiveState.No;
				this.debugDirtyState.Enable = true;
			}
			else
			{
				this.debugBboxThinState.Enable = false;
				this.debugBboxGeomState.Enable = false;
				this.debugBboxFullState.Enable = false;
				this.debugBboxThinState.ActiveState = ActiveState.No;
				this.debugBboxGeomState.ActiveState = ActiveState.No;
				this.debugBboxFullState.ActiveState = ActiveState.No;
				this.debugDirtyState.Enable = false;
			}
		}

		private void HandlePropertyChanged(System.Collections.ArrayList propertyList)
		{
			//	Appelé lorsqu'une propriété a changé.
			if ( this.IsCurrentDocument )
			{
				DocumentInfo di = this.CurrentDocumentInfo;
				di.containerPrincipal.SetDirtyProperties(propertyList);
				di.containerStyles.SetDirtyProperties(propertyList);
			}
		}

		private void HandleAggregateChanged(System.Collections.ArrayList aggregateList)
		{
			//	Appelé lorsqu'un agrégat a changé.
			if ( this.IsCurrentDocument )
			{
				DocumentInfo di = this.CurrentDocumentInfo;
				di.containerPrincipal.SetDirtyAggregates(aggregateList);
				di.containerStyles.SetDirtyAggregates(aggregateList);
			}
		}

		private void HandleTextStyleChanged(System.Collections.ArrayList textStyleList)
		{
			//	Appelé lorsqu'un agrégat a changé.
			if ( this.IsCurrentDocument )
			{
				DocumentInfo di = this.CurrentDocumentInfo;
				di.containerStyles.SetDirtyTextStyles(textStyleList);
			}

			this.ribbonMain.NotifyTextStylesChanged(textStyleList);
			this.ribbonGeom.NotifyTextStylesChanged(textStyleList);
			this.ribbonOper.NotifyTextStylesChanged(textStyleList);
			this.ribbonText.NotifyTextStylesChanged(textStyleList);
		}

		private void HandleTextStyleListChanged()
		{
			//	Appelé lorsqu'un style de texte a été ajouté ou supprimé.
			this.ribbonMain.NotifyChanged("TextStyleListChanged");
			this.ribbonGeom.NotifyChanged("TextStyleListChanged");
			this.ribbonOper.NotifyChanged("TextStyleListChanged");
			this.ribbonText.NotifyChanged("TextStyleListChanged");
		}

		private void HandleSelNamesChanged()
		{
			//	Appelé lorsque la sélection par noms a changé.
			if ( this.IsCurrentDocument )
			{
				DocumentInfo di = this.CurrentDocumentInfo;
				di.containerPrincipal.SetDirtySelNames();
			}
		}

		private void HandleDrawChanged(Viewer viewer, Drawing.Rectangle rect)
		{
			//	Appelé par le document lorsque le dessin a changé.
			Drawing.Rectangle box = rect;

			if ( viewer.DrawingContext.IsActive )
			{
				box.Inflate(viewer.DrawingContext.HandleRedrawSize/2);
			}

			box = viewer.InternalToScreen(box);
			this.InvalidateDraw(viewer, box);
		}

		private void HandleRibbonCommand(string name)
		{
			//	Appelé par le document lorsqu'il faut changer de ruban.
			Ribbons.RibbonContainer ribbon = this.GetRibbon(name);

			if ( name.Length > 0 && name[0] == '!' )
			{
				ribbon = this.LastRibbon(name.Substring(1));
			}

			this.ActiveRibbon(ribbon);
		}
		
		private void HandleBookPanelShowPage(string page, string sub)
		{
			//	Appelé par le document lorsqu'il faut afficher un onglet spécifique.
			this.dlgSplash.Hide();

			DocumentInfo di = this.CurrentDocumentInfo;
			if ( di == null )  return;

			foreach ( TabPage tab in di.bookPanels.Items )
			{
				if ( tab == null )  continue;
				if ( tab.Name == page )
				{
					di.bookPanels.ActivePage = tab;

					if ( page == "Styles" )
					{
						di.containerStyles.SetCategory(sub);
					}
				}
			}
		}
		
		private void HandleSettingsShowPage(string book, string tab)
		{
			//	Appelé par le document lorsqu'il faut afficher une page spécifique du dialoque des réglages.
			this.dlgSplash.Hide();

			if ( this.settingsState.ActiveState == ActiveState.No )
			{
				this.dlgSettings.Show();
				this.settingsState.ActiveState = ActiveState.Yes;
			}

			this.dlgSettings.ShowPage(book, tab);
		}
		

		protected void InvalidateDraw(Viewer viewer, Drawing.Rectangle bbox)
		{
			//	Invalide une partie de la zone de dessin d'un visualisateur.
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

		protected void UpdateScroller()
		{
			//	Met à jour les ascenseurs.
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

		protected void UpdateRulers()
		{
			//	Met à jour les règles, après les avoir montrées ou cachées.
			if ( !this.IsCurrentDocument )  return;

			Viewer viewer = this.CurrentDocument.Modifier.ActiveViewer;
			DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
			DocumentInfo di = this.CurrentDocumentInfo;
			if ( di.hRuler == null )  return;

			di.hRuler.Visibility = (context.RulersShow);
			di.vRuler.Visibility = (context.RulersShow);

			double sw = 17;  // largeur d'un ascenseur
			double sr = 13;  // largeur d'une règle
			double wm = 4;  // marges autour du viewer
			double lm = 0;
			double tm = 0;
			if ( context.RulersShow )
			{
				lm = sr-1;
				tm = sr-1;
			}
			viewer.Margins = new Margins(wm+lm, wm+sw+1, 6+wm+tm, wm+sw+1);
		}


		protected string TextInfoObject
		{
			//	Texte pour les informations.
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

					string sDeep = Res.Strings.Status.Objects.Select;
					if ( deep > 2 )
					{
						sDeep = string.Format(Res.Strings.Status.Objects.Level, deep-2);
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

		protected string TextInfoMouse
		{
			//	Texte pour les informations.
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
						doc.Modifier.ActiveViewer.DrawingContext.SnapGrid(ref mouse);
						return string.Format("x:{0} y:{1}", doc.Modifier.RealToString(mouse.X), doc.Modifier.RealToString(mouse.Y));
					}
					else
					{
						return " ";
					}
				}
			}
		}

		protected string TextInfoModif
		{
			//	Texte pour les informations.
			get
			{
				Document doc = this.CurrentDocument;
				if ( doc == null )
				{
					return " ";
				}
				else
				{
					if ( doc.Modifier.TextInfoModif == "" )
					{
						DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
						int cp = context.CurrentPage;
						int cl = context.CurrentLayer;
						Objects.Page page = this.CurrentDocument.GetObjects[cp] as Objects.Page;
						Objects.Layer layer = page.Objects[cl] as Objects.Layer;

						string sp = page.InfoName;

						string sl = layer.Name;
						if ( sl == "" )  sl = Objects.Layer.ShortName(cl);

						return string.Format(Res.Strings.Status.Modif.Default, sp, sl);
					}
					return doc.Modifier.TextInfoModif;
				}
			}
		}

		protected string TextInfoZoom
		{
			//	Texte pour les informations.
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
					return string.Format(Res.Strings.Status.Zoom.Value, (zoom*100).ToString("F0"));
				}
			}
		}

		protected double ValueInfoZoom
		{
			//	Valeur pour les informations.
			get
			{
				Document doc = this.CurrentDocument;
				if ( doc == null )
				{
					return 0;
				}
				else
				{
					DrawingContext context = doc.Modifier.ActiveViewer.DrawingContext;
					return context.Zoom;
				}
			}
		}


		protected void MouseShowWait()
		{
			//	Met le sablier.
			if ( this.Window == null )  return;

			if ( this.MouseCursor != MouseCursor.AsWait )
			{
				this.lastMouseCursor = this.MouseCursor;
			}

			this.MouseCursor = MouseCursor.AsWait;
			this.Window.MouseCursor = this.MouseCursor;
		}

		protected void MouseHideWait()
		{
			//	Enlève le sablier.
			if ( this.Window == null )  return;
			this.MouseCursor = this.lastMouseCursor;
			this.Window.MouseCursor = this.MouseCursor;
		}


		#region TabBook
		private void HandleBookDocumentsActivePageChanged(object sender)
		{
			//	L'onglet pour le document courant a été cliqué.
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

		public bool IsCurrentDocument
		{
			//	Indique s'il existe un document courant.
			get
			{
				return ( this.currentDocument >= 0 );
			}
		}

		protected DocumentInfo CurrentDocumentInfo
		{
			//	Retourne le DocumentInfo courant.
			get
			{
				if ( this.currentDocument < 0 )  return null;
				return this.documents[this.currentDocument] as DocumentInfo;
			}
		}

		public Document CurrentDocument
		{
			//	Retourne le Document courant.
			get
			{
				if ( this.currentDocument < 0 )  return null;
				return this.CurrentDocumentInfo.document;
			}
		}

		protected bool IsRecyclableDocument()
		{
			//	Indique si le document en cours est un document vide "sans titre"
			//	pouvant servir de conteneur pour ouvrir un nouveau document.
			if ( !this.IsCurrentDocument )  return false;
			if ( this.CurrentDocument.IsDirtySerialize )  return false;
			if ( this.CurrentDocument.Modifier.StatisticTotalObjects() != 0 )  return false;
			return true;
		}

		protected void CreateDocument()
		{
			//	Crée un nouveau document.
			this.PrepareCloseDocument();

			Document doc = new Document(this.type, DocumentMode.Modify, this.installType, this.debugMode, this.globalSettings, this.CommandDispatcher);
			doc.Name = "Document";
			doc.Clipboard = this.clipboard;

			DocumentInfo di = new DocumentInfo();
			di.document = doc;
			this.documents.Insert(++this.currentDocument, di);

			this.CreateDocumentLayout(this.CurrentDocument);
			this.ConnectEvents();
			this.CurrentDocument.Modifier.New();
			this.bookDocuments.ActivePage = di.tabPage;  // (*)
			this.UpdateCloseCommand();
			this.PrepareOpenDocument();

			// (*)	Le Modifier.New doit avoir été fait, car certains panneaux accèdent aux dimensions
			//		de la page. Pour cela, DrawingContext doit avoir un rootStack initialisé, ce qui
			//		est fait par Modifier.New.
		}

		protected void UseDocument(int rank)
		{
			//	Utilise un document ouvert.
			if ( this.ignoreChange )  return;

			this.PrepareCloseDocument();
			this.currentDocument = rank;
			this.PrepareOpenDocument();

			if ( rank >= 0 )
			{
				this.ignoreChange = true;
				this.bookDocuments.ActivePage = this.CurrentDocumentInfo.tabPage;
				this.ignoreChange = false;

				DocumentInfo di;
				int total = this.bookDocuments.PageCount;
				for ( int i=0 ; i<total ; i++ )
				{
					di = this.documents[i] as DocumentInfo;
					di.bookPanels.Visibility = (i == this.currentDocument);
				}

				di = this.CurrentDocumentInfo;
				this.CurrentDocument.HRuler = di.hRuler;
				this.CurrentDocument.VRuler = di.vRuler;

				this.ribbonMain.SetDocument(this.type, this.installType, this.debugMode, this.globalSettings, this.CurrentDocument);
				this.ribbonGeom.SetDocument(this.type, this.installType, this.debugMode, this.globalSettings, this.CurrentDocument);
				this.ribbonOper.SetDocument(this.type, this.installType, this.debugMode, this.globalSettings, this.CurrentDocument);
				this.ribbonText.SetDocument(this.type, this.installType, this.debugMode, this.globalSettings, this.CurrentDocument);

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
				this.ribbonMain.SetDocument(this.type, this.installType, this.debugMode, this.globalSettings, null);
				this.ribbonGeom.SetDocument(this.type, this.installType, this.debugMode, this.globalSettings, null);
				this.ribbonOper.SetDocument(this.type, this.installType, this.debugMode, this.globalSettings, null);
				this.ribbonText.SetDocument(this.type, this.installType, this.debugMode, this.globalSettings, null);

				this.HandleDocumentChanged();
				this.HandleMouseChanged();
				this.HandleOriginChanged();
				this.HandleZoomChanged();
				this.HandleToolChanged();
				this.HandleSaveChanged();
				this.HandleSelectionChanged();
				this.HandleStyleChanged();
				this.HandlePagesChanged();
				this.HandleLayersChanged();
				this.HandleUndoRedoChanged();
				this.HandleGridChanged();
				this.HandleLabelPropertiesChanged();
				this.HandleMagnetChanged();
				this.HandlePreviewChanged();
				this.HandleSettingsChanged();
				this.HandleFontsSettingsChanged();
				this.HandleGuidesChanged();
				this.HandleHideHalfChanged();
				this.HandleDebugChanged();
			}
		}

		protected void CloseDocument()
		{
			//	Ferme le document courant.
			this.PrepareCloseDocument();
			int rank = this.currentDocument;
			if ( rank < 0 )  return;

			DocumentInfo di = this.CurrentDocumentInfo;
			this.currentDocument = -1;
			this.documents.RemoveAt(rank);
			this.ignoreChange = true;
			this.bookDocuments.Items.RemoveAt(rank);
			this.ignoreChange = false;
			di.document.Dispose();
			di.Dispose();

			if ( rank >= this.bookDocuments.PageCount )
			{
				rank = this.bookDocuments.PageCount-1;
			}
			this.UseDocument(rank);
			this.UpdateCloseCommand();

			if ( this.CurrentDocument == null )
			{
				this.ActiveRibbon(this.ribbonMain);
			}
		}

		protected void UpdateCloseCommand()
		{
			//	Met à jour l'état de la commande de fermeture.
			DocumentInfo di = this.CurrentDocumentInfo;
			if ( di != null )
			{
				this.bookDocuments.ActivePage = di.tabPage;
			}

			this.closeState.Enable = (this.bookDocuments.PageCount > 0);
			this.closeAllState.Enable = (this.bookDocuments.PageCount > 0);
			this.forceSaveAllState.Enable = (this.bookDocuments.PageCount > 0);
			this.nextDocState.Enable = (this.bookDocuments.PageCount > 1);
			this.prevDocState.Enable = (this.bookDocuments.PageCount > 1);
			
			if ( di != null )
			{
				this.bookDocuments.UpdateAfterChanges();
			}
		}

		protected void UpdateBookDocuments()
		{
			//	Met à jour le nom de l'onglet des documents.
			if ( !this.IsCurrentDocument )  return;
			TabPage tab = this.bookDocuments.Items[this.currentDocument] as TabPage;
			tab.TabTitle = Misc.ExtractName(this.CurrentDocument.Filename, this.CurrentDocument.IsDirtySerialize);
			this.bookDocuments.UpdateAfterChanges();
		}

		protected void PrepareCloseDocument()
		{
			//	Préparation avant la fermeture d'un document.
			if ( !this.IsCurrentDocument )  return;
			this.CurrentDocument.Dialogs.FlushAll();
		}

		protected void PrepareOpenDocument()
		{
			//	Préparation après l'ouverture d'un document.
			this.dlgExport.Rebuild();
			this.dlgExportPDF.Rebuild();
			this.dlgGlyphs.Rebuild();
			this.dlgInfos.Rebuild();
			this.dlgPrint.Rebuild();
			this.dlgReplace.Rebuild();
			this.dlgSettings.Rebuild();
		}

		protected void CommandStateShake(CommandState state)
		{
			//	Secoue un CommandState pour le forcer à se remettre à jour.
			state.Enable = !state.Enable;
			state.Enable = !state.Enable;
		}
		#endregion


		#region GlobalSettings
		protected bool ReadGlobalSettings()
		{
			//	Lit le fichier des réglages de l'application.
			try
			{
				using ( Stream stream = File.OpenRead(this.GlobalSettingsFilename) )
				{
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

		sealed class VersionDeserializationBinder : Common.IO.GenericDeserializationBinder
		{
			public VersionDeserializationBinder()
			{
			}
		}

		protected bool WritedGlobalSettings()
		{
			//	Ecrit le fichier des réglages de l'application.
			this.globalSettings.IsFullScreen = this.Window.IsFullScreen;
			this.globalSettings.MainWindow = this.Window.WindowPlacementNormalBounds;

			this.dlgAbout.Save();
			this.dlgDownload.Save();
			this.dlgExport.Save();
			this.dlgExportPDF.Save();
			this.dlgGlyphs.Save();
			this.dlgInfos.Save();
			this.dlgKey.Save();
			this.dlgPageStack.Save();
			this.dlgPrint.Save();
			this.dlgReplace.Save();
			this.dlgSettings.Save();
			this.dlgUpdate.Save();

			this.globalSettings.Adorner = Epsitec.Common.Widgets.Adorners.Factory.ActiveName;

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

		protected string GlobalSettingsFilename
		{
			//	Retourne le nom du fichier des réglages de l'application.
			//	Le dossier est qq chose du genre:
			//	C:\Documents and Settings\Daniel Roux\Application Data\Epsitec\Crésus documents\1.0.0.0
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
					return string.Format("{0}\\{1}", dir, "CresusDocuments.data");
				}
			}
		}
		#endregion


		#region Check
		protected void StartCheck(bool always)
		{
			//	Lance le processus asynchrone qui va se connecter au site web
			//	et regarder s'il y a une version plus récente.
			if ( !always && !this.globalSettings.AutoChecker )  return;

			if ( !always )
			{
				Common.Types.Date today = Common.Types.Date.Today;
				if ( Common.Types.Date.Equals(this.globalSettings.DateChecker, today) )
				{
					return;
				}
			}

			this.checker = new VersionChecker(typeof(App.DocumentEditor.Application).Assembly);

			string url = string.Concat(Res.Strings.Dialog.Update.Web4,
				/**/				   "?software=", Res.Strings.Application.SoftName, "&version=",
				/**/				   this.checker.CurrentVersion);

			this.checker.StartCheck(url);
		}

		protected void EndCheck(bool always)
		{
			//	Attend la fin du processus de check et indique si une mise à jour est
			//	disponible.
			if ( this.checker == null )  return;

			while ( !this.checker.IsReady )
			{
				System.Threading.Thread.Sleep(100);  // attend 0.1s
			}

			this.globalSettings.DateChecker = Common.Types.Date.Today;

			if ( this.checker.FoundNewerVersion )
			{
				string version = this.checker.NewerVersion;
				string url = this.checker.NewerVersionUrl;
				
				this.dlgSplash.Hide();
				this.dlgDownload.SetInfo(version, url);
				this.dlgDownload.Show();
			}
			else if ( always )
			{
				this.dlgSplash.Hide();
				this.dlgDownload.SetInfo("", "");
				this.dlgDownload.Show();
			}

			this.checker = null;
		}
		#endregion


		#region Ressources
		public static string GetRes(string name)
		{
			//	Retourne une ressource string d'après son nom.
			//	Si elle n'est pas trouvé dans App.DocumentEditor, elle est
			//	cherchée dans Common.Document !
			string s = Res.Strings.GetString(name);
			if ( s == null )
			{
				s = Epsitec.Common.Document.Document.GetRes(name);
			}
			return s;
		}
		#endregion


		protected DocumentType					type;
		protected InstallType					installType;
		protected DebugMode						debugMode;
		protected bool							useArray;
		protected bool							firstInitialise;
		protected Document						clipboard;
		protected int							currentDocument;
		protected System.Collections.ArrayList	documents;
		protected GlobalSettings				globalSettings;
		protected bool							askKey = false;
		protected MouseCursor					lastMouseCursor = MouseCursor.AsArrow;
		protected VersionChecker				checker;
		protected Common.Designer.MainWindow	resDesignerMainWindow;

		protected CommandDispatcher				commandDispatcher;

		protected HMenu							menu;
		protected VMenu							fileMenu;
		protected HToolBar						hToolBar;
		protected RibbonButton					ribbonMainButton;
		protected RibbonButton					ribbonGeomButton;
		protected RibbonButton					ribbonOperButton;
		protected RibbonButton					ribbonTextButton;
		protected Ribbons.RibbonContainer		ribbonMain;
		protected Ribbons.RibbonContainer		ribbonGeom;
		protected Ribbons.RibbonContainer		ribbonOper;
		protected Ribbons.RibbonContainer		ribbonText;
		protected Ribbons.RibbonContainer		ribbonActive;
		protected System.Collections.ArrayList	ribbonList;
		protected VToolBar						vToolBar;
		protected StatusBar						info;
		protected ResizeKnob					resize;
		protected TabBook						bookDocuments;
		protected double						ribbonHeight = 71;
		protected double						panelsWidth = 252;
		protected bool							ignoreChange;
		protected int							tabIndex;

		protected Dialogs.About					dlgAbout;
		protected Dialogs.Download				dlgDownload;
		protected Dialogs.Export				dlgExport;
		protected Dialogs.ExportPDF				dlgExportPDF;
		protected Dialogs.Glyphs				dlgGlyphs;
		protected Dialogs.Infos					dlgInfos;
		protected Dialogs.Key					dlgKey;
		protected Dialogs.PageStack				dlgPageStack;
		protected Dialogs.Print					dlgPrint;
		protected Dialogs.Replace				dlgReplace;
		protected Dialogs.Settings				dlgSettings;
		protected Dialogs.Splash				dlgSplash;
		protected Dialogs.Update				dlgUpdate;

		protected CommandState					toolSelectState;
		protected CommandState					toolGlobalState;
		protected CommandState					toolShaperState;
		protected CommandState					toolEditState;
		protected CommandState					toolZoomState;
		protected CommandState					toolHandState;
		protected CommandState					toolPickerState;
		protected CommandState					toolHotSpotState;
		protected CommandState					toolLineState;
		protected CommandState					toolRectangleState;
		protected CommandState					toolCircleState;
		protected CommandState					toolEllipseState;
		protected CommandState					toolPolyState;
		protected CommandState					toolBezierState;
		protected CommandState					toolRegularState;
		protected CommandState					toolSurfaceState;
		protected CommandState					toolVolumeState;
		protected CommandState					toolTextLineState;
		protected CommandState					toolTextLine2State;
		protected CommandState					toolTextBoxState;
		protected CommandState					toolTextBox2State;
		protected CommandState					toolArrayState;
		protected CommandState					toolImageState;
		protected CommandState					toolDimensionState;
		protected CommandState					newState;
		protected CommandState					openState;
		protected CommandState					openModelState;
		protected CommandState					saveState;
		protected CommandState					saveAsState;
		protected CommandState					saveModelState;
		protected CommandState					closeState;
		protected CommandState					closeAllState;
		protected CommandState					forceSaveAllState;
		protected CommandState					nextDocState;
		protected CommandState					prevDocState;
		protected CommandState					printState;
		protected CommandState					exportState;
		protected CommandState					glyphsState;
		protected CommandState					glyphsInsertState;
		protected CommandState					textEditingState;
		protected CommandState					replaceState;
		protected CommandState					findNextState;
		protected CommandState					findPrevState;
		protected CommandState					findDefNextState;
		protected CommandState					findDefPrevState;
		protected CommandState					deleteState;
		protected CommandState					duplicateState;
		protected CommandState					cutState;
		protected CommandState					copyState;
		protected CommandState					pasteState;
		protected CommandState					fontBoldState;
		protected CommandState					fontItalicState;
		protected CommandState					fontUnderlinedState;
		protected CommandState					fontOverlinedState;
		protected CommandState					fontStrikeoutState;
		protected CommandState					fontSubscriptState;
		protected CommandState					fontSuperscriptState;
		protected CommandState					fontSizePlusState;
		protected CommandState					fontSizeMinusState;
		protected CommandState					fontClearState;
		protected CommandState					paragraphLeadingState;
		protected CommandState					paragraphLeadingPlusState;
		protected CommandState					paragraphLeadingMinusState;
		protected CommandState					paragraphIndentPlusState;
		protected CommandState					paragraphIndentMinusState;
		protected CommandState					paragraphJustifState;
		protected CommandState					paragraphClearState;
		protected CommandState					orderUpOneState;
		protected CommandState					orderDownOneState;
		protected CommandState					orderUpAllState;
		protected CommandState					orderDownAllState;
		protected CommandState					moveLeftFreeState;
		protected CommandState					moveRightFreeState;
		protected CommandState					moveUpFreeState;
		protected CommandState					moveDownFreeState;
		protected CommandState					rotate90State;
		protected CommandState					rotate180State;
		protected CommandState					rotate270State;
		protected CommandState					rotateFreeCCWState;
		protected CommandState					rotateFreeCWState;
		protected CommandState					mirrorHState;
		protected CommandState					mirrorVState;
		protected CommandState					scaleMul2State;
		protected CommandState					scaleDiv2State;
		protected CommandState					scaleMulFreeState;
		protected CommandState					scaleDivFreeState;
		protected CommandState					alignLeftState;
		protected CommandState					alignCenterXState;
		protected CommandState					alignRightState;
		protected CommandState					alignTopState;
		protected CommandState					alignCenterYState;
		protected CommandState					alignBottomState;
		protected CommandState					alignGridState;
		protected CommandState					shareSpaceXState;
		protected CommandState					shareLeftState;
		protected CommandState					shareCenterXState;
		protected CommandState					shareRightState;
		protected CommandState					shareSpaceYState;
		protected CommandState					shareTopState;
		protected CommandState					shareCenterYState;
		protected CommandState					shareBottomState;
		protected CommandState					adjustWidthState;
		protected CommandState					adjustHeightState;
		protected CommandState					colorToRgbState;
		protected CommandState					colorToCmykState;
		protected CommandState					colorToGrayState;
		protected CommandState					colorStrokeDarkState;
		protected CommandState					colorStrokeLightState;
		protected CommandState					colorFillDarkState;
		protected CommandState					colorFillLightState;
		protected CommandState					mergeState;
		protected CommandState					extractState;
		protected CommandState					groupState;
		protected CommandState					ungroupState;
		protected CommandState					insideState;
		protected CommandState					outsideState;
		protected CommandState					combineState;
		protected CommandState					uncombineState;
		protected CommandState					toBezierState;
		protected CommandState					toPolyState;
		protected CommandState					toTextBox2State;
		protected CommandState					fragmentState;
		protected CommandState					shaperHandleAddState;
		protected CommandState					shaperHandleSubState;
		protected CommandState					shaperHandleToLineState;
		protected CommandState					shaperHandleToCurveState;
		protected CommandState					shaperHandleSymState;
		protected CommandState					shaperHandleSmoothState;
		protected CommandState					shaperHandleDisState;
		protected CommandState					shaperHandleInlineState;
		protected CommandState					shaperHandleFreeState;
		protected CommandState					shaperHandleSimplyState;
		protected CommandState					shaperHandleCornerState;
		protected CommandState					shaperHandleContinueState;
		protected CommandState					booleanAndState;
		protected CommandState					booleanOrState;
		protected CommandState					booleanXorState;
		protected CommandState					booleanFrontMinusState;
		protected CommandState					booleanBackMinusState;
		protected CommandState					undoState;
		protected CommandState					redoState;
		protected CommandState					undoRedoListState;
		protected CommandState					deselectAllState;
		protected CommandState					selectAllState;
		protected CommandState					selectInvertState;
		protected CommandState					selectorAutoState;
		protected CommandState					selectorIndividualState;
		protected CommandState					selectorScalerState;
		protected CommandState					selectorStretchState;
		protected CommandState					selectorStretchTypeState;
		protected CommandState					selectTotalState;
		protected CommandState					selectPartialState;
		protected CommandState					selectorAdaptLine;
		protected CommandState					selectorAdaptText;
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
		protected CommandState					textGridState;
		protected CommandState					textShowControlCharactersState;
		protected CommandState					textFontFilterState;
		protected CommandState					textFontSampleAbcState;
		protected CommandState					textInsertQuadState;
		protected CommandState					textInsertNewFrameState;
		protected CommandState					textInsertNewPageState;
		protected CommandState					magnetState;
		protected CommandState					magnetLayerState;
		protected CommandState					rulersState;
		protected CommandState					labelsState;
		protected CommandState					aggregatesState;
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
		protected CommandState					resDesignerBuildState;
		protected CommandState					resDesignerTranslateState;
		protected CommandState					debugBboxThinState;
		protected CommandState					debugBboxGeomState;
		protected CommandState					debugBboxFullState;
		protected CommandState					debugDirtyState;
		protected CommandState					pagePrevState;
		protected CommandState					pageNextState;
		protected CommandState					pageMenuState;
		protected CommandState					pageNewState;
		protected CommandState					pageDuplicateState;
		protected CommandState					pageUpState;
		protected CommandState					pageDownState;
		protected CommandState					pageDeleteState;
		protected CommandState					layerPrevState;
		protected CommandState					layerNextState;
		protected CommandState					layerMenuState;
		protected CommandState					layerNewState;
		protected CommandState					layerDuplicateState;
		protected CommandState					layerNewSelState;
		protected CommandState					layerMergeUpState;
		protected CommandState					layerMergeDownState;
		protected CommandState					layerDeleteState;
		protected CommandState					layerUpState;
		protected CommandState					layerDownState;
		protected CommandState					settingsState;
		protected CommandState					infosState;
		protected CommandState					aboutState;
		protected CommandState					pageStackState;
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
			public DocWidgets.HRuler			hRuler;
			public DocWidgets.VRuler			vRuler;
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
