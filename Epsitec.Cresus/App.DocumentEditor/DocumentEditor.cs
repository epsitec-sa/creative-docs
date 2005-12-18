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
	/// La classe DocumentEditor repr�sente l'�diteur de document complet.
	/// </summary>
	[SuppressBundleSupport]
	public class DocumentEditor : Widgets.Widget
	{
		public DocumentEditor(DocumentType type)
		{
			// On cr�e son propre dispatcher, pour �viter de marcher sur les autres commandes.
			
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
			this.dlgSettings  = new Dialogs.Settings(this);
			this.dlgUpdate    = new Dialogs.Update(this);

			this.dlgGlyphs.Closed    += new EventHandler(this.HandleDlgClosed);
			this.dlgInfos.Closed     += new EventHandler(this.HandleDlgClosed);
			this.dlgPageStack.Closed += new EventHandler(this.HandleDlgClosed);
			this.dlgSettings.Closed  += new EventHandler(this.HandleDlgClosed);

			this.StartCheck(false);
			this.InitCommands();
			this.CreateLayout();
			
			this.dlgSplash.StartTimer();

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

		private void HandleSizeChanged()
		{
			if ( this.resize == null || this.Window == null )  return;
			this.resize.Enable = !this.Window.IsFullScreen;
		}
		
		protected override void OnSizeChanged(Epsitec.Common.Types.PropertyChangedEventArgs e)
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
			//	Toute modification de la propri�t� BackColor doit �tre r�percut�e
			//	sur le slider. Le plus simple est d'utiliser un override callback
			//	sur la propri�t� BackColor :
			
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
				// On cr�e son propre dispatcher, pour �viter de marcher sur les autres commandes.
				that.commandDispatcher = new CommandDispatcher("DocumentEditor", CommandDispatcherLevel.Primary);
				that.commandDispatcher.RegisterController(that);
			}
			
			return that.commandDispatcher;
		}
#endif


		// Appel� lorsque l'application a fini de d�marrer.
		public void Finalize()
		{
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
				this.MenuAdd(arrayMenu, "", "ArrayAddColumnLeft", "Ins�rer des colonnes � gauche", "");
				this.MenuAdd(arrayMenu, "", "ArrayAddColumnRight", "Ins�rer des colonnes � droite", "");
				this.MenuAdd(arrayMenu, "", "ArrayAddRowTop", "Ins�rer des lignes en dessus", "");
				this.MenuAdd(arrayMenu, "", "ArrayAddRowBottom", "Ins�rer des lignes en dessous", "");
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
			undoRedoList.DockMargins = new Margins(-1, 0, 0, 0);
			ToolTip.Default.SetToolTip(undoRedoList, DocumentEditor.GetRes("Action.UndoRedoList"));
			this.hToolBar.Items.Add(undoRedoList);

			Widget buttonRedo = this.HToolBarAdd("Redo");
			buttonRedo.DockMargins = new Margins(-1, 0, 0, 0);

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

			this.ribbonMainButton = new Ribbons.RibbonButton("", Res.Strings.Ribbon.Main);
			this.ribbonMainButton.Size = this.ribbonMainButton.RequiredSize;
			this.ribbonMainButton.Clicked += new MessageEventHandler(this.HandleRibbonClicked);
			this.hToolBar.Items.Add(this.ribbonMainButton);

			this.ribbonGeomButton = new Ribbons.RibbonButton("", Res.Strings.Ribbon.Geom);
			this.ribbonGeomButton.Size = this.ribbonGeomButton.RequiredSize;
			this.ribbonGeomButton.Clicked += new MessageEventHandler(this.HandleRibbonClicked);
			this.hToolBar.Items.Add(this.ribbonGeomButton);

			this.ribbonOperButton = new Ribbons.RibbonButton("", Res.Strings.Ribbon.Oper);
			this.ribbonOperButton.Size = this.ribbonOperButton.RequiredSize;
			this.ribbonOperButton.Clicked += new MessageEventHandler(this.HandleRibbonClicked);
			this.hToolBar.Items.Add(this.ribbonOperButton);

			this.ribbonTextButton = new Ribbons.RibbonButton("", Res.Strings.Ribbon.Text);
			this.ribbonTextButton.Size = this.ribbonTextButton.RequiredSize;
			this.ribbonTextButton.Clicked += new MessageEventHandler(this.HandleRibbonClicked);
			this.hToolBar.Items.Add(this.ribbonTextButton);

			this.UpdateQuickCommands();

			this.ribbonMain = new Ribbons.RibbonContainer(this);
			this.ribbonMain.Name = "Main";
			this.ribbonMain.Height = this.ribbonHeight;
			this.ribbonMain.Anchor = AnchorStyles.LeftAndRight | AnchorStyles.Top;
			this.ribbonMain.AnchorMargins = new Margins(0, 0, this.hToolBar.Height, 0);
			this.ribbonMain.Visibility = true;
			this.ribbonMain.Items.Add(new Ribbons.File());
			this.ribbonMain.Items.Add(new Ribbons.Clipboard());
			this.ribbonMain.Items.Add(new Ribbons.Undo());
			this.ribbonMain.Items.Add(new Ribbons.Select());
			this.ribbonMain.Items.Add(new Ribbons.View());
			this.ribbonMain.Items.Add(new Ribbons.Action());
#if DEBUG
			this.ribbonMain.Items.Add(new Ribbons.Debug());
#endif

			this.ribbonGeom = new Ribbons.RibbonContainer(this);
			this.ribbonGeom.Name = "Geom";
			this.ribbonGeom.Height = this.ribbonHeight;
			this.ribbonGeom.Anchor = AnchorStyles.LeftAndRight | AnchorStyles.Top;
			this.ribbonGeom.AnchorMargins = new Margins(0, 0, this.hToolBar.Height, 0);
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
			this.ribbonOper.AnchorMargins = new Margins(0, 0, this.hToolBar.Height, 0);
			this.ribbonOper.Visibility = false;
			this.ribbonOper.Items.Add(new Ribbons.Order());
			this.ribbonOper.Items.Add(new Ribbons.Group());
			this.ribbonOper.Items.Add(new Ribbons.Color());

			this.ribbonText = new Ribbons.RibbonContainer(this);
			this.ribbonText.Name = "Text";
			this.ribbonText.Height = this.ribbonHeight;
			this.ribbonText.Anchor = AnchorStyles.LeftAndRight | AnchorStyles.Top;
			this.ribbonText.AnchorMargins = new Margins(0, 0, this.hToolBar.Height, 0);
			this.ribbonText.Visibility = false;
			this.ribbonText.Items.Add(new Ribbons.Paragraph());
			this.ribbonText.Items.Add(new Ribbons.Font());
			this.ribbonText.Items.Add(new Ribbons.Clipboard());
			this.ribbonText.Items.Add(new Ribbons.Insert());

			this.ribbonActive = this.ribbonMain;

			this.info = new StatusBar(this);
			this.info.Anchor = AnchorStyles.LeftAndRight | AnchorStyles.Bottom;
			this.info.AnchorMargins = new Margins(0, 22-5, 0, 0);

			this.InfoAdd("StatusDocument", 120);

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
			slider.DockMargins = new Margins(1, 1, 1, 1);
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
			this.InfoAdd("StatusModif", 250);

			StatusBar infoMisc = new StatusBar(this);
			infoMisc.Width = 22;
			infoMisc.Anchor = AnchorStyles.BottomRight;
			infoMisc.AnchorMargins = new Margins(0, 0, 0, 0);

			IconSeparator sep = new IconSeparator(infoMisc);
			sep.Height = infoMisc.Height-1.0;
			sep.Anchor = AnchorStyles.BottomLeft;

			this.resize = new ResizeKnob(infoMisc);
			this.resize.Anchor = AnchorStyles.BottomRight;
			ToolTip.Default.SetToolTip(this.resize, Res.Strings.Dialog.Tooltip.Resize);

			this.vToolBar = new VToolBar(this);
			this.vToolBar.Anchor = AnchorStyles.TopAndBottom | AnchorStyles.Left;
			this.vToolBar.AnchorMargins = new Margins(0, 0, this.hToolBar.Height+this.RibbonHeight, this.info.Height);
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
			this.VToolBarAdd(this.toolTextLineState);
			this.VToolBarAdd(this.toolTextBoxState);
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
			this.bookDocuments.AnchorMargins = new Margins(this.vToolBar.Width+this.RibbonHeight+1, this.panelsWidth+2, this.hToolBar.Height+1, this.info.Height+1);
			this.bookDocuments.Arrows = TabBookArrows.Right;
			this.bookDocuments.HasCloseButton = true;
			this.bookDocuments.CloseButton.Command = "Close";
			this.bookDocuments.ActivePageChanged += new EventHandler(this.HandleBookDocumentsActivePageChanged);
			ToolTip.Default.SetToolTip(this.bookDocuments.CloseButton, Res.Strings.Tooltip.TabBook.Close);

			this.ActiveRibbon(this.ribbonMain);
		}

		// Met � jour toutes les ic�nes de la partie rapide, � droite des choix du ruban actif.
		public void UpdateQuickCommands()
		{
			// Supprime tous les IconButtons.
			foreach ( Widget widget in this.hToolBar.Children.Widgets )
			{
				if ( widget is Ribbons.RibbonButton )  continue;
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
					if ( first )  // premi�re ic�ne ?
					{
						this.HToolBarAdd(null);  // s�parateur au d�but
						first = false;
					}

					CommandState cs = this.commandDispatcher.GetCommandState(cmd);
					this.HToolBarAdd(cs);

					if ( sep )
					{
						this.HToolBarAdd(null);  // s�parateur apr�s l'ic�ne
					}
				}
			}
		}

		protected void CreateDocumentLayout(Document document)
		{
			DocumentInfo di = this.CurrentDocumentInfo;
			double sw = 17;  // largeur d'un ascenseur
			double sr = 13;  // largeur d'une r�gle
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
				viewer.AnchorMargins = new Margins(wm, wm+sw+1, 6+wm, wm+sw+1);
				document.Modifier.ActiveViewer = viewer;
				document.Modifier.AttachViewer(viewer);

				Viewer frame1 = new Viewer(document);
				frame1.SetParent(rightPane);
				frame1.Anchor = AnchorStyles.LeftAndRight | AnchorStyles.Top;
				frame1.AnchorMargins = new Margins(wm, wm, 6+wm, wm);
				frame1.DrawingContext.LayerDrawingMode = LayerDrawingMode.ShowInactive;
				document.Modifier.AttachViewer(frame1);

				Viewer frame2 = new Viewer(document);
				frame2.SetParent(rightPane);
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
				viewer.SetParent(mainViewParent);
				viewer.Anchor = AnchorStyles.All;
				viewer.AnchorMargins = new Margins(wm+lm, wm+sw+1, 6+wm+tm, wm+sw+1);
				document.Modifier.ActiveViewer = viewer;
				document.Modifier.AttachViewer(viewer);

				di.hRuler = new DocWidgets.HRuler(mainViewParent);
				di.hRuler.Document = document;
				di.hRuler.Anchor = AnchorStyles.LeftAndRight | AnchorStyles.Top;
				di.hRuler.AnchorMargins = new Margins(wm+lm, wm+sw+1, 6+wm, 0);
				ToolTip.Default.SetToolTip(di.hRuler, "*");

				di.vRuler = new DocWidgets.VRuler(mainViewParent);
				di.vRuler.Document = document;
				di.vRuler.Anchor = AnchorStyles.TopAndBottom | AnchorStyles.Left;
				di.vRuler.AnchorMargins = new Margins(wm, 0, 6+wm+tm, wm+sw+1);
				ToolTip.Default.SetToolTip(di.vRuler, "*");
			}

			// Bande horizontale qui contient les boutons des pages et l'ascenseur.
			Widget hBand = new Widget(mainViewParent);
			hBand.Height = sw;
			hBand.Anchor = AnchorStyles.LeftAndRight | AnchorStyles.Bottom;
			hBand.AnchorMargins = new Margins(wm+lm, wm+sw+1, 0, wm);

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
			quickPageNew.DockMargins = new Margins(2, 0, 0, 0);
			ToolTip.Default.SetToolTip(quickPageNew, DocumentEditor.GetRes("Action.PageNew"));

			di.hScroller = new HScroller(hBand);
			di.hScroller.ValueChanged += new EventHandler(this.HandleHScrollerValueChanged);
			di.hScroller.Dock = DockStyle.Fill;
			di.hScroller.DockMargins = new Margins(2, 0, 0, 0);

			// Bande verticale qui contient les boutons des calques et l'ascenseur.
			Widget vBand = new Widget(mainViewParent);
			vBand.Width = sw;
			vBand.Anchor = AnchorStyles.TopAndBottom | AnchorStyles.Right;
			vBand.AnchorMargins = new Margins(0, wm, 6+wm+tm, wm+sw+1);

			Button quickLayerNew = new Button(vBand);
			quickLayerNew.Command = "LayerNew";
			quickLayerNew.Text = "<b>+</b>";
			quickLayerNew.Width = sw;
			quickLayerNew.Height = sw;
			quickLayerNew.Dock = DockStyle.Top;
			quickLayerNew.DockMargins = new Margins(0, 0, 0, 2);
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
			di.vScroller.DockMargins = new Margins(0, 0, 2, 0);

			di.bookPanels = new TabBook(this);
			di.bookPanels.Width = this.panelsWidth;
			di.bookPanels.Anchor = AnchorStyles.TopAndBottom | AnchorStyles.Right;
			di.bookPanels.AnchorMargins = new Margins(1, 1, this.hToolBar.Height+this.RibbonHeight+1, this.info.Height+1);
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

			TabPage bookTextStyles = new TabPage();
			bookTextStyles.TabTitle = Res.Strings.TabPage.TextStyles;
			bookTextStyles.Name = "TextStyles";
			di.bookPanels.Items.Add(bookTextStyles);

#if DEBUG
			TabPage bookAutos = new TabPage();
			bookAutos.TabTitle = Res.Strings.TabPage.Autos;
			bookAutos.Name = "Autos";
			di.bookPanels.Items.Add(bookAutos);
#endif

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
			di.containerPrincipal.DockMargins = new Margins(4, 4, 10, 4);
			document.Modifier.AttachContainer(di.containerPrincipal);

			di.containerStyles = new Containers.Styles(document);
			di.containerStyles.SetParent(bookStyles);
			di.containerStyles.Dock = DockStyle.Fill;
			di.containerStyles.DockMargins = new Margins(4, 4, 10, 4);
			document.Modifier.AttachContainer(di.containerStyles);

			di.containerTextStyles = new Containers.TextStyles(document);
			di.containerTextStyles.SetParent(bookTextStyles);
			di.containerTextStyles.Dock = DockStyle.Fill;
			di.containerTextStyles.DockMargins = new Margins(4, 4, 10, 4);
			document.Modifier.AttachContainer(di.containerTextStyles);

#if DEBUG
			di.containerAutos = new Containers.Autos(document);
			di.containerAutos.SetParent(bookAutos);
			di.containerAutos.Dock = DockStyle.Fill;
			di.containerAutos.DockMargins = new Margins(4, 4, 10, 4);
			document.Modifier.AttachContainer(di.containerAutos);
#endif

			di.containerPages = new Containers.Pages(document);
			di.containerPages.SetParent(bookPages);
			di.containerPages.Dock = DockStyle.Fill;
			di.containerPages.DockMargins = new Margins(4, 4, 10, 4);
			document.Modifier.AttachContainer(di.containerPages);

			di.containerLayers = new Containers.Layers(document);
			di.containerLayers.SetParent(bookLayers);
			di.containerLayers.Dock = DockStyle.Fill;
			di.containerLayers.DockMargins = new Margins(4, 4, 10, 4);
			document.Modifier.AttachContainer(di.containerLayers);

#if false
			di.containerOperations = new Containers.Operations(document);
			di.containerOperations.Parent = bookOper;
			di.containerOperations.Dock = DockStyle.Fill;
			di.containerOperations.DockMargins = new Margins(4, 4, 10, 4);
			document.Modifier.AttachContainer(di.containerOperations);
#endif

			this.bookDocuments.ActivePage = di.tabPage;
		}

		#region LastFilenames
		// Construit le sous-menu des derniers fichiers ouverts.
		protected void BuildLastFilenamesMenu()
		{
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

		// Ajoute un sous-menu dans un menu.
		protected void MenuAddSub(VMenu menu, VMenu sub, string cmd)
		{
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

		// Ajoute une ic�ne.
		protected void MenuAdd(VMenu vmenu, string command)
		{
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
		
		// Ajoute une ic�ne.
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

		// Ajoute une ic�ne.
		protected Widget HToolBarAdd(CommandState cs)
		{
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

		// Ajoute une ic�ne.
		protected Widget VToolBarAdd(CommandState cs)
		{
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
			field.Width = width;
			this.info.Items.Add(field);

			int i = this.info.Children.Count-1;
			this.info.Items[i].Name = name;
			return field;
		}

		protected IconButton InfoAdd(string command)
		{
			CommandState cs = this.commandDispatcher.GetCommandState(command);

			IconButton button = new IconButton(cs.Name, Misc.Icon(cs.IconName, "1"), cs.Name);
			double h = this.info.DefaultHeight-3;
			button.Size = new Size(h, h);
			this.info.Items.Add(button);
			ToolTip.Default.SetToolTip(button, Misc.GetTextWithShortcut(cs));
			return button;
		}


		// Le bouton pour activer/d�sactiver un ruban a �t� cliqu�.
		private void HandleRibbonClicked(object sender, MessageEventArgs e)
		{
			Ribbons.RibbonButton button = sender as Ribbons.RibbonButton;
			Ribbons.RibbonContainer ribbon = null;
			if ( button == this.ribbonMainButton )  ribbon = this.ribbonMain;
			if ( button == this.ribbonGeomButton )  ribbon = this.ribbonGeom;
			if ( button == this.ribbonOperButton )  ribbon = this.ribbonOper;
			if ( button == this.ribbonTextButton )  ribbon = this.ribbonText;
			if ( ribbon == null )  return;

			this.ActiveRibbon(ribbon.IsVisible ? null : ribbon);
		}

		// Donne le ruban correspondant � un nom.
		protected Ribbons.RibbonContainer GetRibbon(string name)
		{
			if ( name == this.ribbonMain.Name )  return this.ribbonMain;
			if ( name == this.ribbonGeom.Name )  return this.ribbonGeom;
			if ( name == this.ribbonOper.Name )  return this.ribbonOper;
			if ( name == this.ribbonText.Name )  return this.ribbonText;
			return null;
		}

		// Cherche le dernier ruban utilis� diff�rent d'un nom donn�.
		protected Ribbons.RibbonContainer LastRibbon(string notName)
		{
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

		// Active un ruban.
		protected void ActiveRibbon(Ribbons.RibbonContainer active)
		{
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
			this.vToolBar.AnchorMargins = new Margins(0, 0, this.hToolBar.Height+h, this.info.Height);
			this.bookDocuments.AnchorMargins = new Margins(this.vToolBar.Width+1, this.panelsWidth+2, this.hToolBar.Height+h+1, this.info.Height+1);

			int total = this.bookDocuments.PageCount;
			for ( int i=0 ; i<total ; i++ )
			{
				DocumentInfo di = this.documents[i] as DocumentInfo;
				di.bookPanels.AnchorMargins = new Margins(1, 1, this.hToolBar.Height+h+1, this.info.Height+1);
			}
			this.ResumeLayout();
		}

		// Retourne la hauteur utilis�e par les rubans.
		protected double RibbonHeight
		{
			get
			{
				return (this.ribbonActive == null) ? 0 : this.ribbonHeight;
			}
		}


		// Un dialogue a �t� ferm�.
		private void HandleDlgClosed(object sender)
		{
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
		// Affiche le dialogue pour demander s'il faut enregistrer le
		// document modifi�, avant de passer � un autre document.
		protected Common.Dialogs.DialogResult DialogSave(CommandDispatcher dispatcher)
		{
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

		// Affiche le dialogue pour signaler la liste de tous les probl�mes.
		protected Common.Dialogs.DialogResult DialogWarnings(CommandDispatcher dispatcher, System.Collections.ArrayList warnings)
		{
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

		// Affiche le dialogue pour signaler une erreur.
		public Common.Dialogs.DialogResult DialogError(CommandDispatcher dispatcher, string error)
		{
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

		// Si on a tap� "toto", mais qu'il existe le fichier "Toto",
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
		// Affiche l'erreur �ventuelle.
		// Retourne false si le fichier n'a pas �t� ouvert.
		protected bool Open(CommandDispatcher dispatcher)
		{
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

		// Demande un nom de fichier mod�le puis ouvre le fichier.
		// Affiche l'erreur �ventuelle.
		// Retourne false si le fichier n'a pas �t� ouvert.
		protected bool OpenModel(CommandDispatcher dispatcher)
		{
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

		// Ouvre un ficher d'apr�s son nom.
		// Affiche l'erreur �ventuelle.
		// Retourne false si le fichier n'a pas �t� ouvert.
		public bool Open(string filename)
		{
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
				// Cherche si ce nom de fichier est d�j� ouvert ?
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

		// Demande un nom de fichier puis enregistre le fichier.
		// Si le document a d�j� un nom de fichier et que ask=false,
		// l'enregistrement est fait directement avec le nom connu.
		// Affiche l'erreur �ventuelle.
		// Retourne false si le fichier n'a pas �t� enregistr�.
		protected bool Save(CommandDispatcher dispatcher, bool ask)
		{
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

		// Demande un nom de fichier mod�le puis enregistre le fichier.
		// Retourne false si le fichier n'a pas �t� enregistr�.
		protected bool SaveModel(CommandDispatcher dispatcher)
		{
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

		// Fait tout ce qu'il faut pour �ventuellement sauvegarder le document
		// avant de passer � autre chose.
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

		// Fait tout ce qu'il faut pour �ventuellement sauvegarder tous les
		// documents avant de passer � autre chose.
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

		// Sauvegarde tous les documents, m�me ceux qui sont � jour.
		protected bool ForceSaveAll(CommandDispatcher dispatcher)
		{
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

		// Lit la collection de couleurs � partir d'un fichier.
		public string PaletteRead(string filename)
		{
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
		[Command ("ParagraphLeading08")]
		[Command ("ParagraphLeading10")]
		[Command ("ParagraphLeading15")]
		[Command ("ParagraphLeading20")]
		[Command ("ParagraphLeading30")]
		[Command ("ParagraphLeadingPlus")]
		[Command ("ParagraphLeadingMinus")]
		[Command ("ParagraphIndentPlus")]
		[Command ("ParagraphIndentMinus")]
		[Command ("ParagraphClear")]
		[Command ("JustifHLeft")]
		[Command ("JustifHCenter")]
		[Command ("JustifHRight")]
		[Command ("JustifHJustif")]
		[Command ("JustifHAll")]
		void CommandFont(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Wrappers.ExecuteCommand(e.CommandName);
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

		[Command ("ColorToRGB")]
		void CommandColorToRGB(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.ColorSelection(ColorSpace.RGB);
		}

		[Command ("ColorToCMYK")]
		void CommandColorToCMYK(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentDocument.Modifier.ColorSelection(ColorSpace.CMYK);
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
			
			menu.ShowAsContextMenu(this.Window, pos);
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

		// Ex�cute une commande locale � un objet.
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
			if ( this.installType == InstallType.Freeware )
			{
				this.StartCheck(true);
				this.EndCheck(true);
			}
			else
			{
				this.dlgSplash.Hide();
				this.dlgUpdate.Show();
			}
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

		// Bouton "menu des pages" cliqu�.
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

		// Bouton "menu des calques" cliqu�.
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
			return Objects.Page.CreateMenu(pages, context.CurrentPage, null);
		}

		// Construit le menu pour choisir un calque.
		public VMenu CreateLayersMenu()
		{
			DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
			Objects.Abstract page = context.RootObject(1);
			UndoableList layers = page.Objects;  // liste des calques
			return Objects.Layer.CreateMenu(layers, context.CurrentLayer, null);
		}



		// Initialise toutes les commandes.
		protected void InitCommands()
		{
			this.MakeIconList();

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
			this.toolTextBoxState = this.CreateCommandState("ObjectTextBox", "ObjectTextBox", "ToolTextBox");
			this.toolTextBox2State = this.CreateCommandState("ObjectTextBox2", "ObjectTextBox", "ToolTextBox", KeyCode.AlphaT);
			this.toolArrayState = this.CreateCommandState("ObjectArray", "ObjectArray", "ToolArray");
			this.toolImageState = this.CreateCommandState("ObjectImage", "ObjectImage", "ToolImage");
			this.toolDimensionState = this.CreateCommandState("ObjectDimension", "ObjectDimension", "ToolDimension");

			this.newState = this.CreateCommandState("New", KeyCode.ModifierCtrl|KeyCode.AlphaN);
			this.openState = this.CreateCommandState("Open", KeyCode.ModifierCtrl|KeyCode.AlphaO);
			this.openModelState = this.CreateCommandState("OpenModel");
			this.saveState = this.CreateCommandState("Save", KeyCode.ModifierCtrl|KeyCode.AlphaS);
			this.saveAsState = this.CreateCommandState("SaveAs");
			this.saveModelState = this.CreateCommandState("SaveModel");
			this.closeState = this.CreateCommandState("Close", null, "Close");
			this.closeAllState = this.CreateCommandState("CloseAll");
			this.forceSaveAllState = this.CreateCommandState("ForceSaveAll");
			this.nextDocState = this.CreateCommandState("NextDocument", KeyCode.ModifierCtrl|KeyCode.FuncF6);
			this.prevDocState = this.CreateCommandState("PrevDocument", KeyCode.ModifierCtrl|KeyCode.ModifierShift|KeyCode.FuncF6);
			this.printState = this.CreateCommandState("Print", KeyCode.ModifierCtrl|KeyCode.AlphaP);
			this.exportState = this.CreateCommandState("Export");
			this.glyphsState = this.CreateCommandState("Glyphs");
			this.glyphsInsertState = this.CreateCommandState("GlyphsInsert");
			this.deleteState = this.CreateCommandState("Delete", KeyCode.Delete);
			this.duplicateState = this.CreateCommandState("Duplicate", KeyCode.ModifierCtrl|KeyCode.AlphaD);

			this.cutState = this.CreateCommandState("Cut", KeyCode.ModifierCtrl|KeyCode.AlphaX);
			this.copyState = this.CreateCommandState("Copy", KeyCode.ModifierCtrl|KeyCode.AlphaC);
			this.pasteState = this.CreateCommandState("Paste", KeyCode.ModifierCtrl|KeyCode.AlphaV);
			
			this.fontBoldState = this.CreateCommandState("FontBold", true, KeyCode.ModifierCtrl|KeyCode.AlphaB);
			this.fontItalicState = this.CreateCommandState("FontItalic", true, KeyCode.ModifierCtrl|KeyCode.AlphaI);
			this.fontUnderlinedState = this.CreateCommandState("FontUnderlined", true, KeyCode.ModifierCtrl|KeyCode.AlphaU);
			this.fontOverlinedState = this.CreateCommandState("FontOverlined", true);
			this.fontStrikeoutState = this.CreateCommandState("FontStrikeout", true);
			this.fontSubscriptState = this.CreateCommandState("FontSubscript", true);
			this.fontSuperscriptState = this.CreateCommandState("FontSuperscript", true);
			this.fontSizePlusState = this.CreateCommandState("FontSizePlus");
			this.fontSizeMinusState = this.CreateCommandState("FontSizeMinus");
			this.fontClearState = this.CreateCommandState("FontClear");
			this.paragraphLeading08State = this.CreateCommandState("ParagraphLeading08", true);
			this.paragraphLeading10State = this.CreateCommandState("ParagraphLeading10", true);
			this.paragraphLeading15State = this.CreateCommandState("ParagraphLeading15", true);
			this.paragraphLeading20State = this.CreateCommandState("ParagraphLeading20", true);
			this.paragraphLeading30State = this.CreateCommandState("ParagraphLeading30", true);
			this.paragraphLeadingPlusState = this.CreateCommandState("ParagraphLeadingPlus");
			this.paragraphLeadingMinusState = this.CreateCommandState("ParagraphLeadingMinus");
			this.paragraphIndentPlusState = this.CreateCommandState("ParagraphIndentPlus");
			this.paragraphIndentMinusState = this.CreateCommandState("ParagraphIndentMinus");
			this.paragraphClearState = this.CreateCommandState("ParagraphClear");
			this.justifHLeftState = this.CreateCommandState("JustifHLeft", "JustifHLeft", "ParagraphJustifHLeft", true);
			this.justifHCenterState = this.CreateCommandState("JustifHCenter", "JustifHCenter", "ParagraphJustifHCenter", true);
			this.justifHRightState = this.CreateCommandState("JustifHRight", "JustifHRight", "ParagraphJustifHRight", true);
			this.justifHJustifState = this.CreateCommandState("JustifHJustif", "JustifHJustif", "ParagraphJustifHJustif", true);
			this.justifHAllState = this.CreateCommandState("JustifHAll", "JustifHAll", "ParagraphJustifHAll", true);
			
			this.orderUpOneState = this.CreateCommandState("OrderUpOne", KeyCode.ModifierCtrl|KeyCode.PageUp);
			this.orderDownOneState = this.CreateCommandState("OrderDownOne", KeyCode.ModifierCtrl|KeyCode.PageDown);
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
			
			this.colorToRGBState = this.CreateCommandState("ColorToRGB");
			this.colorToCMYKState = this.CreateCommandState("ColorToCMYK");
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
			
			this.undoState = this.CreateCommandState("Undo", KeyCode.ModifierCtrl|KeyCode.AlphaZ);
			this.redoState = this.CreateCommandState("Redo", KeyCode.ModifierCtrl|KeyCode.AlphaY);
			this.undoRedoListState = this.CreateCommandState("UndoRedoList");
			
			this.deselectAllState = this.CreateCommandState("DeselectAll", KeyCode.Escape);
			this.selectAllState = this.CreateCommandState("SelectAll", KeyCode.ModifierCtrl|KeyCode.AlphaA);
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
			this.zoomPageState = this.CreateCommandState("ZoomPage", true, KeyCode.ModifierCtrl|KeyCode.Digit0);
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

			this.debugBboxThinState = this.CreateCommandState("DebugBboxThin");
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
			this.moveLeftCtrlState   = this.CreateCommandState("MoveLeftCtrl",   KeyCode.ModifierCtrl|KeyCode.ArrowLeft);
			this.moveRightCtrlState  = this.CreateCommandState("MoveRightCtrl",  KeyCode.ModifierCtrl|KeyCode.ArrowRight);
			this.moveUpCtrlState     = this.CreateCommandState("MoveUpCtrl",     KeyCode.ModifierCtrl|KeyCode.ArrowUp);
			this.moveDownCtrlState   = this.CreateCommandState("MoveDownCtrl",   KeyCode.ModifierCtrl|KeyCode.ArrowDown);
			this.moveLeftShiftState  = this.CreateCommandState("MoveLeftShift",  KeyCode.ModifierShift|KeyCode.ArrowLeft);
			this.moveRightShiftState = this.CreateCommandState("MoveRightShift", KeyCode.ModifierShift|KeyCode.ArrowRight);
			this.moveUpShiftState    = this.CreateCommandState("MoveUpShift",    KeyCode.ModifierShift|KeyCode.ArrowUp);
			this.moveDownShiftState  = this.CreateCommandState("MoveDownShift",  KeyCode.ModifierShift|KeyCode.ArrowDown);
		}

		// Cr�e un nouveau CommandState.
		protected CommandState CreateCommandState(string command, params Widgets.Shortcut[] shortcuts)
		{
			CommandState cs = new CommandState(command, this.commandDispatcher, shortcuts);

			cs.IconName    = this.ExtendIcon(command);
			cs.LongCaption = DocumentEditor.GetRes("Action."+command);

			return cs;
		}

		// Cr�e un nouveau CommandState.
		protected CommandState CreateCommandState(string command, bool statefull, params Widgets.Shortcut[] shortcuts)
		{
			CommandState cs = new CommandState(command, this.commandDispatcher, shortcuts);

			cs.IconName    = this.ExtendIcon(command);
			cs.LongCaption = DocumentEditor.GetRes("Action."+command);
			cs.Statefull   = statefull;

			return cs;
		}

		// Cr�e un nouveau CommandState.
		protected CommandState CreateCommandState(string command, string icon, string tooltip, params Widgets.Shortcut[] shortcuts)
		{
			CommandState cs = new CommandState(command, this.commandDispatcher, shortcuts);

			cs.IconName    = this.ExtendIcon(icon);
			cs.LongCaption = DocumentEditor.GetRes("Action."+tooltip);

			return cs;
		}

		// Cr�e un nouveau CommandState.
		protected CommandState CreateCommandState(string command, string icon, string tooltip, bool statefull, params Widgets.Shortcut[] shortcuts)
		{
			CommandState cs = new CommandState(command, this.commandDispatcher, shortcuts);

			cs.IconName    = this.ExtendIcon(icon);
			cs.LongCaption = DocumentEditor.GetRes("Action."+tooltip);
			cs.Statefull   = statefull;

			return cs;
		}

		// Fouille s'il existe des ic�nes termin�e par "*1.icon" ou "*2.icon" pour donner un nom
		// mixte, du genre "0.Delete;2.Delete2".
		protected string ExtendIcon(string icon)
		{
			System.Diagnostics.Debug.Assert(this.iconList != null);
			if ( icon == null )  return null;
			if ( icon.IndexOf(";") != -1 )  return icon;  // d�j� un nom mixte ?

			bool size1 = this.iconList.ContainsKey(icon+"1");  // existe en petite taille ?
			bool size2 = this.iconList.ContainsKey(icon+"2");  // existe en grande taille ?

			if (  size1 && !size2 )  return Misc.Icon1(icon);   // normal + petit
			if ( !size1 &&  size2 )  return Misc.Icon2(icon);   // normal + grand
			if (  size1 &&  size2 )  return Misc.Icon12(icon);  // normal + petit + grand

			return icon;
		}

		// Construit la liste de toutes les ic�nes existantes.
		protected void MakeIconList()
		{
			System.Text.RegularExpressions.Regex regex = Common.Support.RegexFactory.FromSimpleJoker("*.icon", Common.Support.RegexFactory.Options.IgnoreCase);
			string[] list = Common.Support.ImageProvider.GetManifestResourceNames(regex);
			
			this.iconList = new System.Collections.Hashtable();

			foreach ( string icon in list )
			{
				string[] parts = icon.Split('.');  // partage "Epsitec.App.DocumentEditor.Images.Icon.icon"
				if ( parts.Length >= 2 )
				{
					string quick = parts[parts.Length-2];  // prend l'avant-derni�re partie (avant le .icon)
					if ( !this.iconList.ContainsKey(quick) )
					{
						this.iconList.Add(quick, quick);
					}
				}
			}
		}


		// On s'enregistre aupr�s du document pour tous les �v�nements.
		protected void ConnectEvents()
		{
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
			this.CurrentDocument.Notifier.TextStyleChanged       += new SimpleEventHandler(this.HandleTextStyleChanged);
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
			this.CurrentDocument.Notifier.GuidesChanged          += new SimpleEventHandler(this.HandleGuidesChanged);
			this.CurrentDocument.Notifier.HideHalfChanged        += new SimpleEventHandler(this.HandleHideHalfChanged);
			this.CurrentDocument.Notifier.DebugChanged           += new SimpleEventHandler(this.HandleDebugChanged);
			this.CurrentDocument.Notifier.PropertyChanged        += new PropertyEventHandler(this.HandlePropertyChanged);
			this.CurrentDocument.Notifier.AggregateChanged       += new AggregateEventHandler(this.HandleAggregateChanged);
			this.CurrentDocument.Notifier.SelNamesChanged        += new SimpleEventHandler(this.HandleSelNamesChanged);
			this.CurrentDocument.Notifier.DrawChanged            += new RedrawEventHandler(this.HandleDrawChanged);
			this.CurrentDocument.Notifier.TextRulerColorClicked  += new TextRulerColorEventHandler(this.HandleTextRulerColorClicked);
			this.CurrentDocument.Notifier.TextRulerColorChanged  += new TextRulerColorEventHandler(this.HandleTextRulerColorChanged);
			this.CurrentDocument.Notifier.RibbonCommand          += new RibbonEventHandler(this.HandleRibbonCommand);
			this.CurrentDocument.Notifier.SettingsShowPage       += new SettingsEventHandler(this.HandleSettingsShowPage);
		}

		// Appel� par le document lorsque les informations sur le document ont chang�.
		private void HandleDocumentChanged()
		{
			StatusField field = this.info.Items["StatusDocument"] as StatusField;
			field.Text = this.TextDocument;
			field.Invalidate();

			if ( this.IsCurrentDocument )
			{
				this.printState.Enable = true;
				this.exportState.Enable = true;
				this.infosState.Enable = true;
				this.pageStackState.Enable = true;

				this.CurrentDocument.Dialogs.UpdateInfos();
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

		// Appel� par le document lorsque la position de la souris a chang�.
		private void HandleMouseChanged()
		{
			// TODO: [PA] Parfois, this.info.Items est nul apr�s avoir cliqu� la case de fermeture de la fen�tre !
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

		// Appel� par le document lorsque le texte des modifications a chang�.
		private void HandleModifChanged()
		{
			// TODO: [PA] Parfois, this.info.Items est nul apr�s avoir cliqu� la case de fermeture de la fen�tre !
			if ( this.info.Items == null )  return;

			StatusField field = this.info.Items["StatusModif"] as StatusField;
			field.Text = this.TextInfoModif;
			field.Invalidate();
		}

		// Appel� par le document lorsque l'origine a chang�.
		private void HandleOriginChanged()
		{
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

		// Appel� par le document lorsque le zoom a chang�.
		private void HandleZoomChanged()
		{
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

		// Met � jour une commande d'outil.
		protected void UpdateTool(CommandState cs, string currentTool, bool isCreating, bool enabled)
		{
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

		// Appel� par le document lorsque l'outil a chang�.
		private void HandleToolChanged()
		{
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
			this.UpdateTool(this.toolTextBoxState, tool, isCreating, enabled);
			this.UpdateTool(this.toolTextBox2State, tool, isCreating, enabled);
			this.UpdateTool(this.toolArrayState, tool, isCreating, enabled);
			this.UpdateTool(this.toolImageState, tool, isCreating, enabled);
			this.UpdateTool(this.toolDimensionState, tool, isCreating, enabled);
		}

		// Appel� par le document lorsque l'�tat "enregistrer" a chang�.
		private void HandleSaveChanged()
		{
			if ( this.IsCurrentDocument )
			{
				this.saveState.Enable = this.CurrentDocument.IsDirtySerialize;
				this.saveAsState.Enable = true;
				this.saveModelState.Enable = true;
				this.UpdateBookDocuments();
			}
			else
			{
				this.saveState.Enable = false;
				this.saveAsState.Enable = false;
				this.saveModelState.Enable = false;
			}
		}

		// Appel� par le document lorsque la s�lection a chang�.
		private void HandleSelectionChanged()
		{
			if ( this.IsCurrentDocument )
			{
				DocumentInfo di = this.CurrentDocumentInfo;

				di.containerPrincipal.SetDirtyContent();
#if DEBUG
				di.containerAutos.SetDirtyContent();
#endif
				di.containerStyles.SetDirtyContent();
				//?di.containerOperations.SetDirtyContent();

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
				this.colorToRGBState.Enable = ( totalSelected > 0 && !isCreating && !isEdit );
				this.colorToCMYKState.Enable = ( totalSelected > 0 && !isCreating && !isEdit );
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

				if ( this.CurrentDocument.Wrappers.IsWrappersAttached )  // �dition en cours ?
				{
					//?this.cutState.Enable = true;
					//?this.copyState.Enable = true;
					//?this.pasteState.Enable = true;
				}
				else
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
				this.textShowControlCharactersState.Enable = false;
				this.textFontFilterState.Enable = false;
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
				this.paragraphLeading08State.Enable = false;
				this.paragraphLeading10State.Enable = false;
				this.paragraphLeading15State.Enable = false;
				this.paragraphLeading20State.Enable = false;
				this.paragraphLeading30State.Enable = false;
				this.paragraphLeadingPlusState.Enable = false;
				this.paragraphLeadingMinusState.Enable = false;
				this.paragraphIndentPlusState.Enable = false;
				this.paragraphIndentMinusState.Enable = false;
				this.paragraphClearState.Enable = false;
				this.justifHLeftState.Enable = false;
				this.justifHCenterState.Enable = false;
				this.justifHRightState.Enable = false;
				this.justifHJustifState.Enable = false;
				this.justifHAllState.Enable = false;
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
				this.colorToRGBState.Enable = false;
				this.colorToCMYKState.Enable = false;
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

		// Appel� par le document lorsque le modeleur a chang�.
		private void HandleShaperChanged()
		{
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

		// Appel� par le document lorsque le texte en �dition a chang�.
		private void HandleTextChanged()
		{
			this.dlgGlyphs.SetAlternatesDirty();
		}

		// Appel� par le document lorsqu'un style a chang�.
		private void HandleStyleChanged()
		{
			if ( !this.IsCurrentDocument )  return;
			DocumentInfo di = this.CurrentDocumentInfo;
			di.containerStyles.SetDirtyContent();
		}

		// Appel� par le document lorsqu'un style de texte a chang�.
		private void HandleTextStyleChanged()
		{
			if ( !this.IsCurrentDocument )  return;
			DocumentInfo di = this.CurrentDocumentInfo;
			di.containerTextStyles.SetDirtyContent();
		}

		// Appel� par le document lorsque les pages ont chang�.
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

		// Appel� par le document lorsque les calques ont chang�.
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

		// Appel� par le document lorsqu'un nom de page a chang�.
		private void HandlePageChanged(Objects.Abstract page)
		{
			if ( !this.IsCurrentDocument )  return;
			DocumentInfo di = this.CurrentDocumentInfo;
			di.containerPages.SetDirtyObject(page);
			this.HandleModifChanged();
		}

		// Appel� par le document lorsqu'un nom de calque a chang�.
		private void HandleLayerChanged(Objects.Abstract layer)
		{
			if ( !this.IsCurrentDocument )  return;
			DocumentInfo di = this.CurrentDocumentInfo;
			di.containerLayers.SetDirtyObject(layer);
			this.HandleModifChanged();
		}

		// Appel� par le document lorsque l'�tat des commande undo/redo a chang�.
		private void HandleUndoRedoChanged()
		{
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

		// Appel� par le document lorsque l'�tat de la grille a chang�.
		private void HandleGridChanged()
		{
			if ( this.IsCurrentDocument )
			{
				DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
				
				this.gridState.Enable = true;
				this.gridState.ActiveState = context.GridActive ? ActiveState.Yes : ActiveState.No;
				
				this.textGridState.Enable = true;
				this.textGridState.ActiveState = context.TextGridShow ? ActiveState.Yes : ActiveState.No;
				
				this.textFontFilterState.Enable = true;
				this.textFontFilterState.ActiveState = context.TextFontFilter ? ActiveState.Yes : ActiveState.No;
				
				this.rulersState.Enable = true;
				this.rulersState.ActiveState = context.RulersShow ? ActiveState.Yes : ActiveState.No;
				
				this.labelsState.Enable = true;
				this.labelsState.ActiveState = context.LabelsShow ? ActiveState.Yes : ActiveState.No;
				
				this.aggregatesState.Enable = true;
				this.aggregatesState.ActiveState = context.AggregatesShow ? ActiveState.Yes : ActiveState.No;
			}
			else
			{
				this.gridState.Enable = false;
				this.gridState.ActiveState = ActiveState.No;
				
				this.textGridState.Enable = false;
				this.textGridState.ActiveState = ActiveState.No;
				
				this.textFontFilterState.Enable = false;
				this.textFontFilterState.ActiveState = ActiveState.No;
				
				this.rulersState.Enable = false;
				this.rulersState.ActiveState = ActiveState.No;
				
				this.labelsState.Enable = false;
				this.labelsState.ActiveState = ActiveState.No;
				
				this.aggregatesState.Enable = false;
				this.aggregatesState.ActiveState = ActiveState.No;
			}
		}

		// Appel� par le document lorsque l'�tat des noms d'attributs a chang�.
		private void HandleLabelPropertiesChanged()
		{
			if ( this.IsCurrentDocument )
			{
				DocumentInfo di = this.CurrentDocumentInfo;
				di.containerPrincipal.SetDirtyContent();
				di.containerStyles.SetDirtyContent();
			}
		}

		// Appel� par le document lorsque l'�tat des lignes magn�tiques a chang�.
		private void HandleMagnetChanged()
		{
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

		// Appel� par le document lorsque l'�tat de l'aper�u a chang�.
		private void HandlePreviewChanged()
		{
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

		// Appel� par le document lorsque les r�glages ont chang�.
		private void HandleSettingsChanged()
		{
			if ( this.IsCurrentDocument )
			{
				this.CurrentDocument.Dialogs.UpdateAllSettings();
			}
		}

		// Appel� par le document lorsque les rep�res ont chang�.
		private void HandleGuidesChanged()
		{
			if ( this.IsCurrentDocument )
			{
				this.CurrentDocument.Dialogs.UpdateGuides();
			}
		}

		// Appel� par le document lorsque l'�tat de la commande "hide half" a chang�.
		private void HandleHideHalfChanged()
		{
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

		// Appel� par le document lorsque l'�tat des commande de debug a chang�.
		private void HandleDebugChanged()
		{
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

		// Appel� lorsqu'une propri�t� a chang�.
		private void HandlePropertyChanged(System.Collections.ArrayList propertyList)
		{
			if ( this.IsCurrentDocument )
			{
				DocumentInfo di = this.CurrentDocumentInfo;
				di.containerPrincipal.SetDirtyProperties(propertyList);
				di.containerStyles.SetDirtyProperties(propertyList);
			}
		}

		// Appel� lorsqu'un agr�gat a chang�.
		private void HandleAggregateChanged(System.Collections.ArrayList aggregateList)
		{
			if ( this.IsCurrentDocument )
			{
				DocumentInfo di = this.CurrentDocumentInfo;
				di.containerPrincipal.SetDirtyAggregates(aggregateList);
				di.containerStyles.SetDirtyAggregates(aggregateList);
			}
		}

		// Appel� lorsque la s�lection par noms a chang�.
		private void HandleSelNamesChanged()
		{
			if ( this.IsCurrentDocument )
			{
				DocumentInfo di = this.CurrentDocumentInfo;
				di.containerPrincipal.SetDirtySelNames();
			}
		}

		// Appel� par le document lorsque le dessin a chang�.
		private void HandleDrawChanged(Viewer viewer, Drawing.Rectangle rect)
		{
			Drawing.Rectangle box = rect;

			if ( viewer.DrawingContext.IsActive )
			{
				DocumentInfo di = this.CurrentDocumentInfo;

				double inflate;
				if ( di.hRuler != null && di.hRuler.Edited )  // �dition en cours ?
				{
					inflate = Objects.Abstract.EditFlowHandleSize/viewer.DrawingContext.ScaleX;
				}
				else
				{
					inflate = viewer.DrawingContext.HandleRedrawSize/2;
				}
				box.Inflate(inflate);
			}

			box = viewer.InternalToScreen(box);
			this.InvalidateDraw(viewer, box);
		}

		// Appel� par le document lorsque la couleur dans une r�gle a �t� cliqu�e.
		private void HandleTextRulerColorClicked(TextRuler ruler)
		{
			if ( !this.IsCurrentDocument )  return;

			DocumentInfo di = this.CurrentDocumentInfo;
			di.containerPrincipal.TextRulerColorClicked(ruler);
		}

		// Appel� par le document lorsque la couleur dans une r�gle a chang�.
		private void HandleTextRulerColorChanged(TextRuler ruler)
		{
			if ( !this.IsCurrentDocument )  return;

			DocumentInfo di = this.CurrentDocumentInfo;
			di.containerPrincipal.TextRulerColorChanged(ruler);
		}
		
		// Appel� par le document lorsqu'il faut changer de ruban.
		private void HandleRibbonCommand(string name)
		{
			Ribbons.RibbonContainer ribbon = this.GetRibbon(name);

			if ( name.Length > 0 && name[0] == '!' )
			{
				ribbon = this.LastRibbon(name.Substring(1));
			}

			this.ActiveRibbon(ribbon);
		}
		
		// Appel� par le document lorsqu'il faut afficher une page sp�cifique du dialoque des r�glages.
		private void HandleSettingsShowPage(string book, string tab)
		{
			this.dlgSplash.Hide();

			if ( this.settingsState.ActiveState == ActiveState.No )
			{
				this.dlgSettings.Show();
				this.settingsState.ActiveState = ActiveState.Yes;
			}

			this.dlgSettings.ShowPage(book, tab);
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

		// Met � jour les ascenseurs.
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

		// Met � jour les r�gles, apr�s les avoir montr�es ou cach�es.
		protected void UpdateRulers()
		{
			if ( !this.IsCurrentDocument )  return;

			Viewer viewer = this.CurrentDocument.Modifier.ActiveViewer;
			DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
			DocumentInfo di = this.CurrentDocumentInfo;
			if ( di.hRuler == null )  return;

			di.hRuler.Visibility = (context.RulersShow);
			di.vRuler.Visibility = (context.RulersShow);

			double sw = 17;  // largeur d'un ascenseur
			double sr = 13;  // largeur d'une r�gle
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
					return string.Format(Res.Strings.Status.Document.Format, doc.Modifier.RealToString(size.Width), doc.Modifier.RealToString(size.Height));
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
					if ( doc.Modifier.TextInfoModif == "" )
					{
						DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
						int cp = context.CurrentPage;
						int cl = context.CurrentLayer;
						Objects.Page page = this.CurrentDocument.GetObjects[cp] as Objects.Page;
						Objects.Layer layer = page.Objects[cl] as Objects.Layer;

						string sp = page.Name;
						if ( sp == "" )  sp = page.ShortName;

						string sl = layer.Name;
						if ( sl == "" )  sl = Objects.Layer.ShortName(cl);

						return string.Format(Res.Strings.Status.Modif.Default, sp, sl);
					}
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
					return string.Format(Res.Strings.Status.Zoom.Value, (zoom*100).ToString("F0"));
				}
			}
		}

		// Valeur pour les informations.
		protected double ValueInfoZoom
		{
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


		// Met le sablier.
		protected void MouseShowWait()
		{
			if ( this.MouseCursor != MouseCursor.AsWait )
			{
				this.lastMouseCursor = this.MouseCursor;
			}

			this.MouseCursor = MouseCursor.AsWait;
			this.Window.MouseCursor = this.MouseCursor;
		}

		// Enl�ve le sablier.
		protected void MouseHideWait()
		{
			this.MouseCursor = this.lastMouseCursor;
			this.Window.MouseCursor = this.MouseCursor;
		}


		#region TabBook
		// L'onglet pour le document courant a �t� cliqu�.
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

		// Indique si le document en cours est un document vide "sans titre"
		// pouvant servir de conteneur pour ouvrir un nouveau document.
		protected bool IsRecyclableDocument()
		{
			if ( !this.IsCurrentDocument )  return false;
			if ( this.CurrentDocument.IsDirtySerialize )  return false;
			if ( this.CurrentDocument.Modifier.StatisticTotalObjects() != 0 )  return false;
			return true;
		}

		// Cr�e un nouveau document.
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

				this.ribbonMain.SetDocument(this.type, this.installType, this.globalSettings, this.CurrentDocument);
				this.ribbonGeom.SetDocument(this.type, this.installType, this.globalSettings, this.CurrentDocument);
				this.ribbonOper.SetDocument(this.type, this.installType, this.globalSettings, this.CurrentDocument);
				this.ribbonText.SetDocument(this.type, this.installType, this.globalSettings, this.CurrentDocument);

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
				this.ribbonMain.SetDocument(this.type, this.installType, this.globalSettings, null);
				this.ribbonGeom.SetDocument(this.type, this.installType, this.globalSettings, null);
				this.ribbonOper.SetDocument(this.type, this.installType, this.globalSettings, null);
				this.ribbonText.SetDocument(this.type, this.installType, this.globalSettings, null);

				this.HandleDocumentChanged();
				this.HandleMouseChanged();
				this.HandleOriginChanged();
				this.HandleZoomChanged();
				this.HandleToolChanged();
				this.HandleSaveChanged();
				this.HandleSelectionChanged();
				this.HandleStyleChanged();
				this.HandleTextStyleChanged();
				this.HandlePagesChanged();
				this.HandleLayersChanged();
				this.HandleUndoRedoChanged();
				this.HandleGridChanged();
				this.HandleLabelPropertiesChanged();
				this.HandleMagnetChanged();
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

		// Met � jour l'�tat de la commande de fermeture.
		protected void UpdateCloseCommand()
		{
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

		// Met � jour le nom de l'onglet des documents.
		protected void UpdateBookDocuments()
		{
			if ( !this.IsCurrentDocument )  return;
			TabPage tab = this.bookDocuments.Items[this.currentDocument] as TabPage;
			tab.TabTitle = Misc.ExtractName(this.CurrentDocument.Filename, this.CurrentDocument.IsDirtySerialize);
			this.bookDocuments.UpdateAfterChanges();
		}

		// Pr�paration avant la fermeture d'un document.
		protected void PrepareCloseDocument()
		{
			if ( !this.IsCurrentDocument )  return;
			this.CurrentDocument.Dialogs.FlushAll();
		}

		// Pr�paration apr�s l'ouverture d'un document.
		protected void PrepareOpenDocument()
		{
			this.dlgExport.Rebuild();
			this.dlgExportPDF.Rebuild();
			this.dlgGlyphs.Rebuild();
			this.dlgInfos.Rebuild();
			this.dlgPrint.Rebuild();
			this.dlgSettings.Rebuild();
		}

		// Secoue un CommandState pour le forcer � se remettre � jour.
		protected void CommandStateShake(CommandState state)
		{
			state.Enable = !state.Enable;
			state.Enable = !state.Enable;
		}
		#endregion


		#region GlobalSettings
		// Lit le fichier des r�glages de l'application.
		protected bool ReadGlobalSettings()
		{
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

		// Ecrit le fichier des r�glages de l'application.
		protected bool WritedGlobalSettings()
		{
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

		// Retourne le nom du fichier des r�glages de l'application.
		// Le dossier est qq chose du genre:
		// C:\Documents and Settings\Daniel Roux\Application Data\Epsitec\Cr�sus documents\1.0.0.0
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
					return string.Format("{0}\\{1}", dir, "CresusDocuments.data");
				}
			}
		}
		#endregion


		#region Check
		// Lance le processus asynchrone qui va se connecter au site web
		// et regarder s'il y a une version plus r�cente.
		protected void StartCheck(bool always)
		{
			if ( this.installType != InstallType.Freeware )  return;
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

			string url = "http://www.creativedocs.net/update/check" +
						 "?software=CreativeDocs&version=" +
						 this.checker.CurrentVersion;

			this.checker.StartCheck(url);
		}

		// Attend la fin du processus de check et indique si une mise � jour est
		// disponible.
		protected void EndCheck(bool always)
		{
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
		// Retourne une ressource string d'apr�s son nom.
		// Si elle n'est pas trouv� dans App.DocumentEditor, elle est
		// cherch�e dans Common.Document !
		public static string GetRes(string name)
		{
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
		protected bool							useArray;
		protected bool							firstInitialise;
		protected Document						clipboard;
		protected int							currentDocument;
		protected System.Collections.ArrayList	documents;
		protected GlobalSettings				globalSettings;
		protected bool							askKey = false;
		protected MouseCursor					lastMouseCursor = MouseCursor.AsArrow;
		protected VersionChecker				checker;

		protected CommandDispatcher				commandDispatcher;

		protected HMenu							menu;
		protected VMenu							fileMenu;
		protected HToolBar						hToolBar;
		protected Ribbons.RibbonButton			ribbonMainButton;
		protected Ribbons.RibbonButton			ribbonGeomButton;
		protected Ribbons.RibbonButton			ribbonOperButton;
		protected Ribbons.RibbonButton			ribbonTextButton;
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
		protected Dialogs.Settings				dlgSettings;
		protected Dialogs.Splash				dlgSplash;
		protected Dialogs.Update				dlgUpdate;

		protected System.Collections.Hashtable	iconList;

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
		protected CommandState					paragraphLeading08State;
		protected CommandState					paragraphLeading10State;
		protected CommandState					paragraphLeading15State;
		protected CommandState					paragraphLeading20State;
		protected CommandState					paragraphLeading30State;
		protected CommandState					paragraphLeadingPlusState;
		protected CommandState					paragraphLeadingMinusState;
		protected CommandState					paragraphIndentPlusState;
		protected CommandState					paragraphIndentMinusState;
		protected CommandState					paragraphClearState;
		protected CommandState					justifHLeftState;
		protected CommandState					justifHCenterState;
		protected CommandState					justifHRightState;
		protected CommandState					justifHJustifState;
		protected CommandState					justifHAllState;
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
		protected CommandState					colorToRGBState;
		protected CommandState					colorToCMYKState;
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
			public Containers.TextStyles		containerTextStyles;
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
				if ( this.containerTextStyles != null )  this.containerTextStyles.Dispose();
				if ( this.containerAutos != null )  this.containerAutos.Dispose();
				if ( this.containerPages != null )  this.containerPages.Dispose();
				if ( this.containerLayers != null )  this.containerLayers.Dispose();
				if ( this.containerOperations != null )  this.containerOperations.Dispose();
			}
		}
	}
}
