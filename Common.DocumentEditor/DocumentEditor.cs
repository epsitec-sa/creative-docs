using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Soap;
using Epsitec.Common.Document;
using Epsitec.Common.Drawing;
using Epsitec.Common.IO;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
using Epsitec.Common.Widgets.Platform;

namespace Epsitec.Common.DocumentEditor
{
    using Containers = Common.Document.Containers;
    using Document = Common.Document.Document;
    using DocWidgets = Common.Document.Widgets;
    using Drawing = Common.Drawing;
    using GlobalSettings = Common.Document.Settings.GlobalSettings;
    using Menus = Common.Document.Menus;
    using Objects = Common.Document.Objects;
    using Ribbons = Common.Document.Ribbons;
    using Settings = Common.Document.Settings;

    /// <summary>
    /// La classe DocumentEditor représente l'éditeur de document complet.
    /// </summary>
    public class DocumentEditor : Widget
    {
        public DocumentEditor(DocumentType type)
            : this(
                type,
                new CommandDispatcher("Common.DocumentEditor", CommandDispatcherLevel.Primary),
                new CommandContext(),
                null
            ) { }

        public DocumentEditor(
            DocumentType type,
            CommandDispatcher dispatcher,
            CommandContext context,
            Window window
        )
        {
            this.commandDispatcher = dispatcher;
            this.commandContext = context;

            this.commandDispatcher.RegisterController(this);

            CommandDispatcher.SetDispatcher(this, this.commandDispatcher);

            this.documentType = type;
            this.useArray = false;

            if (this.documentType == DocumentType.Pictogram)
            {
                this.installType = InstallType.Full;
            }
            else
            {
                if (Application.Mode == "F")
                {
                    this.askKey = false;
                    this.installType = InstallType.Freeware;
                }
                else
                {
#if DEBUG
                    this.installType = InstallType.Full;
#else
                    string key = Common.Support.SerialAlgorithm.ReadCrDocSerial();
                    this.askKey = (key == null);

                    if (string.IsNullOrEmpty(key) || key == "demo")
                    {
                        this.installType = InstallType.Demo;
                    }
                    else if (Common.Support.SerialAlgorithm.CheckSerial(key, 40))
                    {
                        this.installType = InstallType.Full;
                    }
                    else
                    {
                        this.installType = InstallType.Expired;
                    }
#endif
                }
            }

            if (Epsitec.Common.Support.Globals.IsDebugBuild)
            {
                this.debugMode = DebugMode.DebugCommands;
                //?this.debugMode = this.documentType == Common.Document.DocumentType.Pictogram ? DebugMode.DebugCommands : DebugMode.Release;
            }
            else
            {
                this.debugMode = DebugMode.Release;
            }

            if (!this.ReadGlobalSettings())
            {
                this.globalSettings = new GlobalSettings();
                this.globalSettings.Initialize(this.documentType);
            }

            Epsitec.Common.Widgets.Adorners.Factory.SetActive(this.globalSettings.Adorner);

            this.dlgSplash = new Dialogs.Splash(this);

            if (this.documentType != DocumentType.Pictogram && this.globalSettings.SplashScreen)
            {
                this.dlgSplash.Show();
            }

            ImageManager.InitializeDefaultCache();

            //	Crée les associations internes pour la lecture des miniatures des fichiers
            //	*.crdoc et *.crmod (pour le cache des documents).
            this.defaultDocumentManager = new DocumentManager();
            this.defaultDocumentManager.Associate(".crdoc", Document.GetDocumentInfo);
            this.defaultDocumentManager.Associate(".crmod", Document.GetDocumentInfo);

            DocumentCache.CreateDefaultImageAssociations(this.defaultDocumentManager);

            this.dlgAbout = new Dialogs.About(this);
            this.dlgDownload = new Dialogs.Download(this);
            this.dlgExportType = new Dialogs.ExportType(this);
            this.dlgExport = new Dialogs.Export(this);
            this.dlgExportPDF = new Dialogs.ExportPDF(this);
            this.dlgExportICO = new Dialogs.ExportICO(this);
            this.dlgGlyphs = new Dialogs.Glyphs(this);
            this.dlgInfos = new Dialogs.Infos(this);
            this.dlgFileExport = new Dialogs.FileExport(this);
            this.dlgFileNew = new Dialogs.FileNew(this);
            this.dlgFileOpen = new Dialogs.FileOpen(this);
            this.dlgFileOpenModel = new Dialogs.FileOpenModel(this);
            this.dlgFileSave = new Dialogs.FileSave(this);
            this.dlgFileSaveModel = new Dialogs.FileSaveModel(this);
            this.dlgPageStack = new Dialogs.PageStack(this);
            this.dlgPrint = new Dialogs.Print(this);
            this.dlgReplace = new Dialogs.Replace(this);
            this.dlgSettings = new Dialogs.Settings(this);
            this.dlgUpdate = new Dialogs.Update(this);

            this.dlgGlyphs.Closed += this.HandleDlgClosed;
            this.dlgInfos.Closed += this.HandleDlgClosed;
            this.dlgPageStack.Closed += this.HandleDlgClosed;
            this.dlgReplace.Closed += this.HandleDlgClosed;
            this.dlgSettings.Closed += this.HandleDlgClosed;

            this.InitCommands();
            this.CreateLayout();

            Misc.GetFontList(false); // mise en cache des polices

            this.dlgSplash.StartTimer();

            this.clipboard = new Document(
                this.documentType,
                DocumentMode.Clipboard,
                this.installType,
                this.debugMode,
                this.globalSettings,
                this.CommandDispatcher,
                this.CommandContext,
                window
            );
            this.clipboard.Name = "Clipboard";

            this.documents = new List<DocumentInfo>();
            this.currentDocument = -1;

            string[] args = System.Environment.GetCommandLineArgs();
            if (args.Length >= 2)
            {
                this.CreateDocument(window);
                string filename = args[1];
                string err = "";
                if (Misc.IsExtension(filename, ".crcolors"))
                {
                    err = this.PaletteRead(filename);
                }
                else
                {
                    err = this.CurrentDocument.Read(filename);
                    this.UpdateAfterRead();
                }
                this.UpdateRulers();
                if (err == "")
                {
                    this.DialogWarnings(this.CurrentDocument.ReadWarnings);
                }
                this.DialogError(err);
            }
            else
            {
                if (this.globalSettings.FirstAction == Settings.FirstAction.OpenNewDocument)
                {
                    this.CreateDocument(window);
                }

                if (
                    this.globalSettings.FirstAction == Settings.FirstAction.OpenLastModel
                    && this.globalSettings.LastModelCount > 0
                )
                {
                    this.CreateDocument(window);
                    string filename = this.globalSettings.LastModelGet(0);
                    string err = this.CurrentDocument.Read(filename);
                    this.UpdateAfterRead();
                    this.UpdateRulers();
                    if (err == "")
                    {
                        this.DialogWarnings(this.CurrentDocument.ReadWarnings);
                    }
                }

                if (
                    this.globalSettings.FirstAction == Settings.FirstAction.OpenLastFile
                    && this.globalSettings.LastFilenameCount > 0
                )
                {
                    this.CreateDocument(window);
                    string filename = this.globalSettings.LastFilenameGet(0);
                    string err = this.CurrentDocument.Read(filename);
                    this.UpdateAfterRead();
                    this.UpdateRulers();
                    if (err == "")
                    {
                        this.DialogWarnings(this.CurrentDocument.ReadWarnings);
                    }
                }
            }

            if (this.HasCurrentDocument)
            {
                this.initializationInProgress = true;
                this.CurrentDocument.InitializationInProgress = true;
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
            if (this.resize == null || this.Window == null)
                return;
            this.resize.Enable = !this.Window.IsFullScreen;
        }

        protected override void SetBoundsOverride(Rectangle oldRect, Rectangle newRect)
        {
            base.SetBoundsOverride(oldRect, newRect);
            this.HandleSizeChanged();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            ImageManager.ShutDownDefaultCache();
        }

        public DocumentType DocumentType
        {
            get { return this.documentType; }
        }

        public InstallType InstallType
        {
            get { return this.installType; }
            set
            {
                this.installType = value;

                int total = this.documents.Count;
                for (int i = 0; i < total; i++)
                {
                    DocumentInfo di = this.documents[i];
                    di.document.InstallType = this.installType;
                }
            }
        }

        public DebugMode DebugMode
        {
            get { return this.debugMode; }
            set
            {
                this.debugMode = value;

                int total = this.documents.Count;
                for (int i = 0; i < total; i++)
                {
                    DocumentInfo di = this.documents[i];
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

        public CommandContext CommandContext
        {
            get { return this.commandContext; }
        }

        public void MakeReadyToRun()
        {
            //	Appelé lorsque l'application a fini de démarrer.
            if (this.askKey)
            {
                this.askKey = false;
                this.dlgSplash.Hide();
            }

            if (this.HasCurrentDocument)
            {
                this.CurrentDocument.Notifier.NotifyOriginChanged();
            }
        }

        public void AsyncNotify()
        {
            if (this.currentDocument < 0)
                return;
            this.CurrentDocument.Notifier.GenerateEvents();

            if (this.initializationInProgress)
            {
                //	Initialisation en cours. Cette période dure depuis la création d'un
                //	nouveau document, jusqu'au premier AsyncNotify effectué lorsque tout
                //	existe de façon certaine. Lorsque ce mode est true, le document n'est
                //	pas affiché, pour éviter de voir apparaître brièvement un document
                //	avec un zoom faux. Ceci est nécessaire à cause de ZoomPageAndCenter
                //	pour lequel la fenêtre doit exister dans sa taille définitive !
                this.initializationInProgress = false;

                int total = this.documents.Count;
                for (int i = 0; i < total; i++)
                {
                    DocumentInfo di = this.documents[i];
                    if (di.document.InitializationInProgress)
                    {
                        di.document.InitializationInProgress = false;
                        DrawingContext context = di.document.Modifier.ActiveViewer.DrawingContext;
                        context.ZoomPageAndCenter();
                    }
                }

                if (this.HasCurrentDocument)
                {
                    //?DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
                    //?context.ZoomPageAndCenter();
                    this.CurrentDocument.Modifier.ActiveViewer.Focus();
                }
            }
        }

        protected void CreateLayout()
        {
            ToolTip.Default.Behaviour = ToolTipBehaviour.Normal;

            //	Crée le RibbonBook unique qui remplace le traditionnel menu.
            this.ribbonBook = new RibbonBook(this);
            this.ribbonBook.Dock = DockStyle.Top;
            this.ribbonBook.ActivePageChanged += this.HandleRibbonBookActivePageChanged;

            //	Crée les différentes pages du ruban.
            this.ribbonMain = new RibbonPage();
            this.ribbonMain.Name = "Main";
            this.ribbonMain.RibbonTitle = Res.Strings.Ribbon.Main;
            this.ribbonBook.Pages.Add(this.ribbonMain);

            this.ribbonGeom = new RibbonPage();
            this.ribbonGeom.Name = "Geom";
            this.ribbonGeom.RibbonTitle = Res.Strings.Ribbon.Geom;
            this.ribbonBook.Pages.Add(this.ribbonGeom);

            this.ribbonOper = new RibbonPage();
            this.ribbonOper.Name = "Oper";
            this.ribbonOper.RibbonTitle = Res.Strings.Ribbon.Oper;
            this.ribbonBook.Pages.Add(this.ribbonOper);

            this.ribbonText = new RibbonPage();
            this.ribbonText.Name = "Text";
            this.ribbonText.RibbonTitle = Res.Strings.Ribbon.Text;
            this.ribbonBook.Pages.Add(this.ribbonText);

            //	Peuble les pages des rubans par les sections.
            this.ribbonMain.Items.Add(
                new Ribbons.File(this.documentType, this.installType, this.debugMode)
            );
            this.ribbonMain.Items.Add(
                new Ribbons.Clipboard(this.documentType, this.installType, this.debugMode)
            );
            this.ribbonMain.Items.Add(
                new Ribbons.Undo(this.documentType, this.installType, this.debugMode)
            );
            this.ribbonMain.Items.Add(
                new Ribbons.Select(this.documentType, this.installType, this.debugMode)
            );
            this.ribbonMain.Items.Add(
                new Ribbons.View(this.documentType, this.installType, this.debugMode)
            );
            this.ribbonMain.Items.Add(
                new Ribbons.Action(this.documentType, this.installType, this.debugMode)
            );
            if (this.debugMode == DebugMode.DebugCommands)
            {
                this.ribbonMain.Items.Add(
                    new Ribbons.Debug(this.documentType, this.installType, this.debugMode)
                );
            }

            this.ribbonGeom.Items.Add(
                new Ribbons.Move(this.documentType, this.installType, this.debugMode)
            );
            this.ribbonGeom.Items.Add(
                new Ribbons.Rotate(this.documentType, this.installType, this.debugMode)
            );
            this.ribbonGeom.Items.Add(
                new Ribbons.Scale(this.documentType, this.installType, this.debugMode)
            );
            this.ribbonGeom.Items.Add(
                new Ribbons.Align(this.documentType, this.installType, this.debugMode)
            );
            this.ribbonGeom.Items.Add(
                new Ribbons.Bool(this.documentType, this.installType, this.debugMode)
            );
            this.ribbonGeom.Items.Add(
                new Ribbons.Geom(this.documentType, this.installType, this.debugMode)
            );

            this.ribbonOper.Items.Add(
                new Ribbons.Order(this.documentType, this.installType, this.debugMode)
            );
            this.ribbonOper.Items.Add(
                new Ribbons.Group(this.documentType, this.installType, this.debugMode)
            );
            this.ribbonOper.Items.Add(
                new Ribbons.Color(this.documentType, this.installType, this.debugMode)
            );

            this.ribbonText.Items.Add(
                new Ribbons.TextStyles(this.documentType, this.installType, this.debugMode)
            );
            this.ribbonText.Items.Add(
                new Ribbons.Paragraph(this.documentType, this.installType, this.debugMode)
            );
            this.ribbonText.Items.Add(
                new Ribbons.Font(this.documentType, this.installType, this.debugMode)
            );
            this.ribbonText.Items.Add(
                new Ribbons.Clipboard(this.documentType, this.installType, this.debugMode)
            );
            this.ribbonText.Items.Add(
                new Ribbons.Undo(this.documentType, this.installType, this.debugMode)
            );
            this.ribbonText.Items.Add(
                new Ribbons.Replace(this.documentType, this.installType, this.debugMode)
            );
            this.ribbonText.Items.Add(
                new Ribbons.Insert(this.documentType, this.installType, this.debugMode)
            );

            this.UpdateQuickCommands();

            //	Crée la barre de status.
            this.info = new StatusBar(this);
            this.info.Dock = DockStyle.Bottom;

            this.InfoAdd(Common.Widgets.Res.Commands.DeselectAll.CommandId);
            this.InfoAdd(Common.Widgets.Res.Commands.SelectAll.CommandId);
            this.InfoAdd("SelectInvert");
            this.InfoAdd("HideSel");
            this.InfoAdd("HideRest");
            this.InfoAdd("HideCancel");

            this.InfoAdd("StatusObject", 120);

            //?this.InfoAdd("ZoomMin");
            if (this.documentType != DocumentType.Pictogram)
            {
                this.InfoAdd("ZoomPage");
                this.InfoAdd("ZoomPageWidth");
            }
            this.InfoAdd("ZoomDefault");
            this.InfoAdd("ZoomSel");
            if (this.documentType != DocumentType.Pictogram)
            {
                this.InfoAdd("ZoomSelWidth");
            }
            this.InfoAdd("ZoomPrev");

            StatusField sf = this.InfoAdd("StatusZoom", 55);
            sf.Clicked += this.HandleStatusZoomClicked;
            ToolTip.Default.SetToolTip(sf, Res.Strings.Status.Zoom.Menu);

            AbstractSlider slider = new HSlider();
            slider.Name = "StatusZoomSlider";
            slider.PreferredWidth = 100;
            slider.Margins = new Margins(1, 1, 1, 1);
            if (this.documentType == DocumentType.Pictogram)
            {
                slider.MinValue = 0.5M;
                slider.MaxValue = 8.0M;
                slider.SmallChange = 0.1M;
                slider.LargeChange = 0.5M;
            }
            else
            {
                slider.MinValue = 0.1M;
                slider.MaxValue = 16.0M;
                slider.SmallChange = 0.1M;
                slider.LargeChange = 0.5M;
            }
            slider.Resolution = 0.0M;
            slider.LogarithmicDivisor = 3.0M;
            slider.ValueChanged += this.HandleSliderZoomChanged;
            this.info.Items.Add(slider);
            ToolTip.Default.SetToolTip(slider, Res.Strings.Status.Zoom.Slider);

            this.InfoAdd("StatusMouse", 110);
            this.InfoAdd("StatusModif", 300);

            this.info.Items["StatusModif"].Dock = DockStyle.Fill;

            this.resize = new ResizeKnob();
            this.resize.Margins = new Margins(2, 0, 0, 0);
            this.info.Items.Add(this.resize);
            this.resize.Dock = DockStyle.Right; // doit être fait après le Items.Add !
            ToolTip.Default.SetToolTip(this.resize, Res.Strings.Dialog.Tooltip.Resize);

            //	Crée la barre d'outils verticale gauche.
            this.vToolBar = new VToolBar(this);
            this.vToolBar.Dock = DockStyle.Left;

            this.VToolBarAdd(this.toolSelectState.Command);
            this.VToolBarAdd(this.toolGlobalState.Command);
            this.VToolBarAdd(this.toolShaperState.Command);
            this.VToolBarAdd(this.toolEditState.Command);
            this.VToolBarAdd(this.toolZoomState.Command);
            this.VToolBarAdd(this.toolHandState.Command);
            this.VToolBarAdd(this.toolPickerState.Command);
            if (this.documentType == DocumentType.Pictogram)
            {
                this.VToolBarAdd(this.toolHotSpotState.Command);
            }
            this.VToolBarAdd(null);
            this.VToolBarAdd(this.toolLineState.Command);
            this.VToolBarAdd(this.toolRectangleState.Command);
            this.VToolBarAdd(this.toolCircleState.Command);
            this.VToolBarAdd(this.toolEllipseState.Command);
            this.VToolBarAdd(this.toolPolyState.Command);
            this.VToolBarAdd(this.toolFreeState.Command);
            this.VToolBarAdd(this.toolBezierState.Command);
            this.VToolBarAdd(this.toolRegularState.Command);
            this.VToolBarAdd(this.toolSurfaceState.Command);
            this.VToolBarAdd(this.toolVolumeState.Command);
            //this.VToolBarAdd(this.toolTextLineState.Command);
            //this.VToolBarAdd(this.toolTextBoxState.Command);
            this.VToolBarAdd(this.toolTextLine2State.Command);
            this.VToolBarAdd(this.toolTextBox2State.Command);
            if (this.useArray)
            {
                this.VToolBarAdd(this.toolArrayState.Command);
            }
            this.VToolBarAdd(this.toolImageState.Command);
            if (this.documentType != DocumentType.Pictogram)
            {
                this.VToolBarAdd(this.toolDimensionState.Command);
            }
            this.VToolBarAdd(null);

            //	Crée la partie pour les documents.
            this.bookDocuments = new TabBook(this);
            this.bookDocuments.PreferredWidth = this.panelsWidth;
            this.bookDocuments.Dock = DockStyle.Fill;
            this.bookDocuments.Margins = new Margins(1, 0, 3, 1);
            this.bookDocuments.Arrows = TabBookArrows.Right;
            this.bookDocuments.HasCloseButton = true;
            this.bookDocuments.CloseButton.CommandObject = Command.Get("Close");
            this.bookDocuments.ActivePageChanged += new EventHandler<CancelEventArgs>(
                this.HandleBookDocumentsActivePageChanged
            );
            ToolTip.Default.SetToolTip(
                this.bookDocuments.CloseButton,
                Res.Strings.Tooltip.TabBook.Close
            );

            this.SetActiveRibbon(this.ribbonMain);
        }

        public void UpdateQuickCommands()
        {
            //	Met à jour toutes les icônes de la partie rapide, à droite des choix du ruban actif.
            //	Supprime tous les IconButtons.
            this.ribbonBook.Buttons.Clear();

            bool first = true;
            foreach (string xcmd in this.globalSettings.QuickCommands)
            {
                bool used = GlobalSettings.QuickUsed(xcmd);
                bool sep = GlobalSettings.QuickSep(xcmd);
                string cmd = GlobalSettings.QuickCmd(xcmd);

                if (used)
                {
                    if (first) // première icône ?
                    {
                        this.RibbonAdd(null); // séparateur au début
                        first = false;
                    }

                    Command c = Common.Widgets.Command.Get(cmd);
                    this.RibbonAdd(c);

                    if (sep)
                    {
                        this.RibbonAdd(null); // séparateur après l'icône
                    }
                }
            }
        }

        protected void CreateDocumentLayout(Document document)
        {
            DocumentInfo di = this.CurrentDocumentInfo;
            double sw = 17; // largeur d'un ascenseur
            double sr = 13; // largeur d'une règle
            double wm = 4; // marges autour du viewer
            double lm = sr - 1;
            double tm = sr - 1;

            di.tabPage = new TabPage();
            this.bookDocuments.Items.Insert(this.currentDocument, di.tabPage);

            FrameBox topPane = new FrameBox(di.tabPage);
            topPane.Dock = DockStyle.Fill;

            di.pagePane = new FrameBox(di.tabPage);
            di.pagePane.Dock = DockStyle.Bottom;
            di.pagePane.Padding = new Margins(wm, wm, 0, wm);
            di.pagePane.Visibility = false;

            di.mainPane = new FrameBox(topPane);
            di.mainPane.Dock = DockStyle.Fill;

            di.layerPane = new FrameBox(topPane);
            di.layerPane.Dock = DockStyle.Right;
            di.layerPane.Padding = new Margins(0, wm, wm, wm);
            di.layerPane.Visibility = false;

            //	Partie gauche principale.
            Widget mainViewParent = di.mainPane;
            Viewer viewer = new Viewer(document);
            viewer.SetParent(mainViewParent);
            viewer.Anchor = AnchorStyles.All;
            viewer.Margins = new Margins(wm + lm, wm + sw + 1, 6 + tm, wm + sw + 1);
            document.Modifier.ActiveViewer = viewer;
            document.Modifier.AttachViewer(viewer);

            di.hRuler = new DocWidgets.HRuler(mainViewParent);
            di.hRuler.Document = document;
            di.hRuler.Anchor = AnchorStyles.LeftAndRight | AnchorStyles.Top;
            di.hRuler.Margins = new Margins(wm + lm, wm + sw + 1, 6, 0);
            ToolTip.Default.SetToolTip(di.hRuler, "*");

            di.vRuler = new DocWidgets.VRuler(mainViewParent);
            di.vRuler.Document = document;
            di.vRuler.Anchor = AnchorStyles.TopAndBottom | AnchorStyles.Left;
            di.vRuler.Margins = new Margins(wm, 0, 6 + tm, wm + sw + 1);
            ToolTip.Default.SetToolTip(di.vRuler, "*");

            //	Partie basse pour les pages miniatures.
            di.pageMiniatures = new Containers.PageMiniatures(document);
            document.PageMiniatures = di.pageMiniatures;
            di.pageMiniatures.CreateInterface(di.pagePane);

            //	Partie droite pour les calques miniatures.
            di.layerMiniatures = new Containers.LayerMiniatures(document);
            document.LayerMiniatures = di.layerMiniatures;
            di.layerMiniatures.CreateInterface(di.layerPane);

            //	Bande horizontale qui contient les boutons des pages et l'ascenseur.
            Widget hBand = new Widget(mainViewParent);
            hBand.PreferredHeight = sw;
            hBand.Anchor = AnchorStyles.LeftAndRight | AnchorStyles.Bottom;
            hBand.Margins = new Margins(wm, wm + sw + 1, 0, wm);

            di.quickPageMiniatures = new GlyphButton("PageMiniatures");
            di.quickPageMiniatures.SetParent(hBand);
            di.quickPageMiniatures.GlyphShape = GlyphShape.ArrowUp;
            di.quickPageMiniatures.PreferredWidth = sw;
            di.quickPageMiniatures.PreferredHeight = sw;
            di.quickPageMiniatures.Dock = DockStyle.Left;
            di.quickPageMiniatures.Margins = new Margins(0, 2, 0, 0);
            ToolTip.Default.SetToolTip(
                di.quickPageMiniatures,
                DocumentEditor.GetRes("Action.PageMiniatures")
            );

            GlyphButton quickPagePrev = new GlyphButton("PagePrev");
            quickPagePrev.SetParent(hBand);
            quickPagePrev.GlyphShape = GlyphShape.ArrowLeft;
            quickPagePrev.PreferredWidth = sw;
            quickPagePrev.PreferredHeight = sw;
            quickPagePrev.Dock = DockStyle.Left;
            ToolTip.Default.SetToolTip(quickPagePrev, DocumentEditor.GetRes("Action.PagePrev"));

            di.quickPageMenu = new Button(hBand);
            di.quickPageMenu.ButtonStyle = ButtonStyle.Icon;
            di.quickPageMenu.CommandObject = Command.Get("PageMenu");
            di.quickPageMenu.Clicked += this.HandleQuickPageMenu;
            di.quickPageMenu.PreferredWidth = System.Math.Floor(sw * 2.0);
            di.quickPageMenu.PreferredHeight = sw;
            di.quickPageMenu.Dock = DockStyle.Left;
            ToolTip.Default.SetToolTip(di.quickPageMenu, DocumentEditor.GetRes("Action.PageMenu"));

            GlyphButton quickPageNext = new GlyphButton("PageNext");
            quickPageNext.SetParent(hBand);
            quickPageNext.GlyphShape = GlyphShape.ArrowRight;
            quickPageNext.PreferredWidth = sw;
            quickPageNext.PreferredHeight = sw;
            quickPageNext.Dock = DockStyle.Left;
            quickPageNext.Margins = new Margins(0, 2, 0, 0);
            ToolTip.Default.SetToolTip(quickPageNext, DocumentEditor.GetRes("Action.PageNext"));

            GlyphButton quickPageNew = new GlyphButton("PageNew");
            quickPageNew.SetParent(hBand);
            quickPageNew.GlyphShape = GlyphShape.Plus;
            quickPageNew.PreferredWidth = sw;
            quickPageNew.PreferredHeight = sw;
            quickPageNew.Dock = DockStyle.Left;
            quickPageNew.Margins = new Margins(0, 2, 0, 0);
            ToolTip.Default.SetToolTip(quickPageNew, DocumentEditor.GetRes("Action.PageNew"));

            di.hScroller = new HScroller(hBand);
            di.hScroller.ValueChanged += this.HandleHScrollerValueChanged;
            di.hScroller.Dock = DockStyle.Fill;

            //	Bande verticale qui contient les boutons des calques et l'ascenseur.
            Widget vBand = new Widget(mainViewParent);
            vBand.PreferredWidth = sw;
            vBand.Anchor = AnchorStyles.TopAndBottom | AnchorStyles.Right;
            vBand.Margins = new Margins(0, wm, 6, wm + sw + 1);

            di.quickLayerMiniatures = new GlyphButton("LayerMiniatures");
            di.quickLayerMiniatures.SetParent(vBand);
            di.quickLayerMiniatures.GlyphShape = GlyphShape.ArrowLeft;
            di.quickLayerMiniatures.PreferredWidth = sw;
            di.quickLayerMiniatures.PreferredHeight = sw;
            di.quickLayerMiniatures.Dock = DockStyle.Top;
            di.quickLayerMiniatures.Margins = new Margins(0, 0, 0, 2);
            ToolTip.Default.SetToolTip(
                di.quickLayerMiniatures,
                DocumentEditor.GetRes("Action.LayerMiniatures")
            );

            GlyphButton quickLayerNext = new GlyphButton("LayerNext");
            quickLayerNext.SetParent(vBand);
            quickLayerNext.GlyphShape = GlyphShape.ArrowUp;
            quickLayerNext.PreferredWidth = sw;
            quickLayerNext.PreferredHeight = sw;
            quickLayerNext.Dock = DockStyle.Top;
            ToolTip.Default.SetToolTip(quickLayerNext, DocumentEditor.GetRes("Action.LayerNext"));

            di.quickLayerMenu = new Button(vBand);
            di.quickLayerMenu.ButtonStyle = ButtonStyle.Icon;
            di.quickLayerMenu.CommandObject = Command.Get("LayerMenu");
            di.quickLayerMenu.Clicked += this.HandleQuickLayerMenu;
            di.quickLayerMenu.PreferredWidth = sw;
            di.quickLayerMenu.PreferredHeight = sw;
            di.quickLayerMenu.Dock = DockStyle.Top;
            ToolTip.Default.SetToolTip(
                di.quickLayerMenu,
                DocumentEditor.GetRes("Action.LayerMenu")
            );

            GlyphButton quickLayerPrev = new GlyphButton("LayerPrev");
            quickLayerPrev.SetParent(vBand);
            quickLayerPrev.GlyphShape = GlyphShape.ArrowDown;
            quickLayerPrev.PreferredWidth = sw;
            quickLayerPrev.PreferredHeight = sw;
            quickLayerPrev.Dock = DockStyle.Top;
            quickLayerPrev.Margins = new Margins(0, 0, 0, 2);
            ToolTip.Default.SetToolTip(quickLayerPrev, DocumentEditor.GetRes("Action.LayerPrev"));

            GlyphButton quickLayerNew = new GlyphButton("LayerNew");
            quickLayerNew.SetParent(vBand);
            quickLayerNew.GlyphShape = GlyphShape.Plus;
            quickLayerNew.PreferredWidth = sw;
            quickLayerNew.PreferredHeight = sw;
            quickLayerNew.Dock = DockStyle.Top;
            quickLayerNew.Margins = new Margins(0, 0, 0, 2);
            ToolTip.Default.SetToolTip(quickLayerNew, DocumentEditor.GetRes("Action.LayerNew"));

            di.vScroller = new VScroller(vBand);
            di.vScroller.ValueChanged += this.HandleVScrollerValueChanged;
            di.vScroller.Dock = DockStyle.Fill;

            di.bookPanels = new TabBook(this);
            di.bookPanels.PreferredWidth = this.panelsWidth;
            di.bookPanels.Dock = DockStyle.Right;
            di.bookPanels.Margins = new Margins(1, 1, 3, 1);
            di.bookPanels.Arrows = TabBookArrows.Stretch;
            di.bookPanels.ActivePageChanged += new EventHandler<CancelEventArgs>(
                this.HandleBookPanelsActivePageChanged
            );

            TabPage bookPrincipal = new TabPage();
            bookPrincipal.TabTitle = Res.Strings.TabPage.Principal;
            bookPrincipal.Name = "Principal";
            di.bookPanels.Items.Add(bookPrincipal);

            TabPage bookStyles = new TabPage();
            bookStyles.TabTitle = Res.Strings.TabPage.Styles;
            bookStyles.Name = "Styles";
            di.bookPanels.Items.Add(bookStyles);

            TabPage bookAutos = null;
            if (this.debugMode == DebugMode.DebugCommands)
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

            if (bookAutos != null)
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

        public HMenu GetMenu()
        {
            return this.menu;
        }

        protected void MenuAddSub(VMenu menu, VMenu sub, string cmd)
        {
            //	Ajoute un sous-menu dans un menu.
            for (int i = 0; i < menu.Items.Count; i++)
            {
                MenuItem item = menu.Items[i] as MenuItem;
                if (item.CommandObject.CommandId == cmd)
                {
                    item.Submenu = sub;
                    //?					item.CommandObject = Command.Get(cmd);
                    return;
                }
            }
            System.Diagnostics.Debug.Assert(false, "MenuAddSub: submenu not found");
        }

        protected void MenuAdd(VMenu vmenu, string command)
        {
            //	Ajoute une icône.
            if (command == null)
            {
                vmenu.Items.Add(new MenuSeparator());
            }
            else
            {
                Command c = Common.Widgets.Command.Get(command);

                MenuItem item = new MenuItem(
                    c.CommandId,
                    c.Icon,
                    c.Description,
                    Misc.GetShortcut(c),
                    c.CommandId
                );
                vmenu.Items.Add(item);
            }
        }

        protected void MenuAdd(
            VMenu vmenu,
            string icon,
            string command,
            string text,
            string shortcut
        )
        {
            //	Ajoute une icône.
            this.MenuAdd(vmenu, icon, command, text, shortcut, command);
        }

        protected void MenuAdd(
            VMenu vmenu,
            string icon,
            string command,
            string text,
            string shortcut,
            string name
        )
        {
            if (text == "")
            {
                vmenu.Items.Add(new MenuSeparator());
            }
            else
            {
                MenuItem item;

                if (icon == "y/n")
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

        protected Widget VToolBarAdd(Command command)
        {
            //	Ajoute une icône.
            if (command == null)
            {
                IconSeparator sep = new IconSeparator();
                sep.IsHorizontal = false;
                this.vToolBar.Items.Add(sep);
                return sep;
            }
            else
            {
                IconButton button = new IconButton(command);
                this.vToolBar.Items.Add(button);
                ToolTip.Default.SetToolTip(button, command.GetDescriptionWithShortcut());
                return button;
            }
        }

        protected StatusField InfoAdd(string name, double width)
        {
            StatusField field = new StatusField();
            field.PreferredWidth = width;
            this.info.Items.Add(field);

            int i = this.info.Children.Count - 1;
            this.info.Items[i].Name = name;
            return field;
        }

        protected IconButton InfoAdd(string commandName)
        {
            Command command = Common.Widgets.Command.Get(commandName);

            IconButton button = new IconButton(command);
            button.PreferredIconSize = Misc.IconPreferredSize("Small");
            double h = this.info.PreferredHeight - 3;
            button.PreferredSize = new Size(h, h);
            this.info.Items.Add(button);
            ToolTip.Default.SetToolTip(button, command.GetDescriptionWithShortcut());
            return button;
        }

        #region Ribbons
        void HandleRibbonBookActivePageChanged(object sender)
        {
            //	Le ruban actif a été changé (nouvelle page visible).
            RibbonPage page = this.ribbonBook.ActivePage;
            this.SetActiveRibbon(page);
        }

        protected RibbonPage GetRibbon(string name)
        {
            //	Donne le ruban correspondant à un nom.
            if (name == this.ribbonMain.Name)
                return this.ribbonMain;
            if (name == this.ribbonGeom.Name)
                return this.ribbonGeom;
            if (name == this.ribbonOper.Name)
                return this.ribbonOper;
            if (name == this.ribbonText.Name)
                return this.ribbonText;
            return null;
        }

        protected RibbonPage LastRibbon(string notName)
        {
            //	Cherche le dernier ruban utilisé différent d'un nom donné.
            if (this.ribbonList == null)
                return null;

            for (int i = this.ribbonList.Count - 1; i >= 0; i--)
            {
                string name = this.ribbonList[i] as string;
                if (name != notName)
                {
                    return this.GetRibbon(name);
                }
            }
            return null;
        }

        protected void SetActiveRibbon(RibbonPage active)
        {
            //	Active un ruban.
            this.ribbonActive = active;
            this.ribbonBook.ActivePage = active;

            if (this.ribbonList == null)
            {
                this.ribbonList = new System.Collections.ArrayList();
            }

            if (active == null)
            {
                this.ribbonList.Add("");
            }
            else
            {
                this.ribbonList.Add(active.Name);
            }

            if (this.ribbonList.Count > 10)
            {
                this.ribbonList.RemoveAt(0);
            }
        }

        protected void RibbonsNotifyChanged(string changed)
        {
            //	Passe en revue toutes les sections de toutes les pages.
            foreach (Widget widget in this.ribbonBook.Pages)
            {
                RibbonPage page = widget as RibbonPage;
                if (page == null)
                    continue;

                foreach (Ribbons.Abstract section in page.Items)
                {
                    section.NotifyChanged(changed);
                }
            }
        }

        protected void RibbonsNotifyTextStylesChanged(System.Collections.ArrayList textStyleList)
        {
            //	Passe en revue toutes les sections de toutes les pages.
            foreach (Widget widget in this.ribbonBook.Pages)
            {
                RibbonPage page = widget as RibbonPage;
                if (page == null)
                    continue;

                foreach (Ribbons.Abstract section in page.Items)
                {
                    section.NotifyTextStylesChanged(textStyleList);
                }
            }
        }

        protected void RibbonsNotifyTextStylesChanged()
        {
            //	Passe en revue toutes les sections de toutes les pages.
            foreach (Widget widget in this.ribbonBook.Pages)
            {
                RibbonPage page = widget as RibbonPage;
                if (page == null)
                    continue;

                foreach (Ribbons.Abstract section in page.Items)
                {
                    section.NotifyTextStylesChanged();
                }
            }
        }

        protected void RibbonsSetDocument(Settings.GlobalSettings gs, Document document)
        {
            //	Passe en revue toutes les sections de toutes les pages.
            foreach (Widget widget in this.ribbonBook.Pages)
            {
                RibbonPage page = widget as RibbonPage;
                if (page == null)
                    continue;

                foreach (Ribbons.Abstract section in page.Items)
                {
                    section.SetDocument(gs, document);
                }
            }
        }

        protected Widget RibbonAdd(Command command)
        {
            //	Ajoute une icône.
            if (command == null)
            {
                IconSeparator sep = new IconSeparator();
                sep.IsHorizontal = true;
                this.ribbonBook.Buttons.Add(sep);
                return sep;
            }
            else
            {
                IconButton button = new IconButton(command);
                this.ribbonBook.Buttons.Add(button);
                ToolTip.Default.SetToolTip(button, command.GetDescriptionWithShortcut());
                return button;
            }
        }
        #endregion


        private void HandleDlgClosed(object sender)
        {
            //	Un dialogue a été fermé.
            if (sender == this.dlgGlyphs)
            {
                this.glyphsState.ActiveState = ActiveState.No;
            }

            if (sender == this.dlgInfos)
            {
                this.infosState.ActiveState = ActiveState.No;
            }

            if (sender == this.dlgPageStack)
            {
                this.pageStackState.ActiveState = ActiveState.No;
            }

            if (sender == this.dlgSettings)
            {
                this.settingsState.ActiveState = ActiveState.No;
            }

            if (sender == this.dlgReplace)
            {
                this.replaceState.ActiveState = ActiveState.No;
            }
        }

        private void HandleHScrollerValueChanged(object sender)
        {
            HScroller scroller = sender as HScroller;
            Viewer viewer = this.CurrentDocument.Modifier.ActiveViewer;
            viewer.DrawingContext.OriginX = (double)-scroller.Value;
        }

        private void HandleVScrollerValueChanged(object sender)
        {
            VScroller scroller = sender as VScroller;
            Viewer viewer = this.CurrentDocument.Modifier.ActiveViewer;
            viewer.DrawingContext.OriginY = (double)-scroller.Value;
        }

        private void HandleBookPanelsActivePageChanged(object sender, CancelEventArgs e)
        {
            TabBook book = sender as TabBook;
            TabPage page = book.ActivePage;
            this.CurrentDocument.Modifier.TabBookChanged(page.Name);
        }

        private void HandleStatusZoomClicked(object sender, MessageEventArgs e)
        {
            if (!this.HasCurrentDocument)
                return;
            StatusField sf = sender as StatusField;
            if (sf == null)
                return;
            DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
            VMenu menu = Menus.ZoomMenu.CreateZoomMenu(context.Zoom, context.ZoomPage, null);
            menu.Host = sf.Window;
            TextFieldCombo.AdjustComboSize(sf, menu, false);
            menu.ShowAsComboList(sf, Point.Zero, sf);
        }

        private void HandleSliderZoomChanged(object sender)
        {
            if (!this.HasCurrentDocument)
                return;
            AbstractSlider slider = sender as AbstractSlider;
            if (slider == null)
                return;
            this.CurrentDocument.Modifier.ZoomValue((double)slider.Value, slider.IsInitialChange);
        }

        protected override void UpdateClientGeometry()
        {
            base.UpdateClientGeometry();
        }

        [Command("ToolSelect")]
        [Command("ToolGlobal")]
        [Command("ToolShaper")]
        [Command("ToolEdit")]
        [Command("ToolZoom")]
        [Command("ToolHand")]
        [Command("ToolPicker")]
        [Command("ToolHotSpot")]
        [Command("ObjectLine")]
        [Command("ObjectRectangle")]
        [Command("ObjectCircle")]
        [Command("ObjectEllipse")]
        [Command("ObjectPoly")]
        [Command("ObjectFree")]
        [Command("ObjectBezier")]
        [Command("ObjectRegular")]
        [Command("ObjectSurface")]
        [Command("ObjectVolume")]
        [Command("ObjectTextLine")]
        [Command("ObjectTextLine2")]
        [Command("ObjectTextBox")]
        [Command("ObjectTextBox2")]
        [Command("ObjectArray")]
        [Command("ObjectImage")]
        [Command("ObjectDimension")]
        void CommandTool(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.Tool = e.Command.CommandId;
            this.DispatchDummyMouseMoveEvent();
        }

        #region IO
        protected Common.Dialogs.DialogResult DialogSave()
        {
            //	Affiche le dialogue pour demander s'il faut enregistrer le
            //	document modifié, avant de passer à un autre document.
            if (
                !this.CurrentDocument.IsDirtySerialize
                || this.CurrentDocument.Modifier.StatisticTotalObjects() == 0
            )
            {
                return Common.Dialogs.DialogResult.None;
            }

            this.dlgSplash.Hide();

            string title = Res.Strings.Application.TitleShort;
            string icon = "manifest:Epsitec.Common.Dialogs.Images.Question.icon";
            string shortFilename = Misc.ExtractName(
                this.CurrentDocument.Filename,
                this.CurrentDocument.IsDirtySerialize
            );
            string statistic = string.Format(
                "<font size=\"80%\">{0}</font><br/>",
                this.CurrentDocument.Modifier.Statistic(false, false)
            );
            string question1 = string.Format(Res.Strings.Dialog.Question.Save.Part1, shortFilename);
            string question2 = Res.Strings.Dialog.Question.Save.Part2;
            string message = string.Format(
                "<font size=\"100%\">{0}</font><br/><br/>{1}{2}",
                question1,
                statistic,
                question2
            );
            Common.Dialogs.IDialog dialog = Common.Dialogs.MessageDialog.CreateYesNoCancel(
                title,
                icon,
                message,
                null,
                null,
                this.commandDispatcher
            );
            dialog.OwnerWindow = this.Window;
            dialog.OpenDialog();
            return dialog.Result;
        }

        protected Common.Dialogs.DialogResult DialogWarnings(System.Collections.ArrayList warnings)
        {
            //	Affiche le dialogue pour signaler la liste de tous les problèmes.
            if (warnings == null || warnings.Count == 0)
                return Common.Dialogs.DialogResult.None;

            this.dlgSplash.Hide();
            warnings.Sort();

            string title = Res.Strings.Application.TitleShort;
            string icon = "manifest:Epsitec.Common.Dialogs.Images.Warning.icon";

            string chip = "<list type=\"fix\" width=\"1.5\"/>";
            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            builder.Append(Res.Strings.Dialog.Warning.Text1);
            builder.Append("<br/>");
            builder.Append("<br/>");
            foreach (string s in warnings)
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

            Common.Dialogs.IDialog dialog = Common.Dialogs.MessageDialog.CreateOk(
                title,
                icon,
                message,
                "",
                this.commandDispatcher
            );
            dialog.OwnerWindow = this.Window;
            dialog.OpenDialog();
            return dialog.Result;
        }

        protected void DialogWarningRedirection()
        {
            //	Affiche l'avertissement de changement 'Exemples originaux' vers 'Mes exemples'.
            string message = string.Format(
                Res.Strings.Dialog.Warning.Redirection,
                Document.OriginalSamplesDisplayName,
                Document.MySamplesDisplayName
            ); // TODO: mettre dans les ressources !
            this.DialogError(message, escapeErrorMessage: false);
        }

        public string ShortWindowTitle
        {
            get { return Res.Strings.Application.TitleShort; }
        }

        public Common.Dialogs.DialogResult DialogError(string error, bool escapeErrorMessage = true)
        {
            //	Affiche le dialogue pour signaler une erreur.
            if (this.Window == null)
                return Common.Dialogs.DialogResult.None;
            if (error == "")
                return Common.Dialogs.DialogResult.None;

            this.dlgSplash.Hide();

            FormattedText message = escapeErrorMessage
                ? Common.Dialogs.Helpers.MessageBuilder.FormatMessage(error)
                : new FormattedText(error);
            return Common.Dialogs.MessageDialog.ShowError(message, this.Window);
        }

        public Common.Dialogs.DialogResult DialogQuestion(string message)
        {
            //	Affiche le dialogue pour poser une question oui/non.
            if (this.Window == null)
                return Common.Dialogs.DialogResult.None;

            this.dlgSplash.Hide();

            string title = Res.Strings.Application.TitleShort;
            string icon = "manifest:Epsitec.Common.Dialogs.Images.Question.icon";

            Common.Dialogs.IDialog dialog = Common.Dialogs.MessageDialog.CreateYesNo(
                title,
                icon,
                message,
                "",
                "",
                this.commandDispatcher
            );
            dialog.OwnerWindow = this.Window;
            dialog.OpenDialog();
            return dialog.Result;
        }

        protected bool Open()
        {
            //	Demande un nom de fichier puis ouvre le fichier.
            //	Affiche l'erreur éventuelle.
            //	Retourne false si le fichier n'a pas été ouvert.
            this.dlgSplash.Hide();

            this.dlgFileOpen.Title =
                (this.documentType == DocumentType.Pictogram)
                    ? Res.Strings.Dialog.Open.TitlePic
                    : Res.Strings.Dialog.Open.TitleDoc;

            this.dlgFileOpen.InitialDirectory = this.globalSettings.InitialDirectory;
            this.dlgFileOpen.InitialFileName = "";

            this.dlgFileOpen.ShowDialog(); // choix d'un fichier...
            if (this.dlgFileOpen.Result != Common.Dialogs.DialogResult.Accept)
            {
                return false;
            }

            this.globalSettings.InitialDirectory = this.dlgFileOpen.InitialDirectory;

            string[] names = this.dlgFileOpen.FileNames;
            for (int i = 0; i < names.Length; i++)
            {
                this.Open(names[i]);
            }
            return true;
        }

        protected bool OpenModel()
        {
            //	Demande un nom de fichier modèle puis ouvre le fichier.
            //	Affiche l'erreur éventuelle.
            //	Retourne false si le fichier n'a pas été ouvert.
            this.dlgSplash.Hide();

            this.dlgFileOpenModel.InitialDirectory = this.globalSettings.NewDocument;
            this.dlgFileOpenModel.InitialFileName = "";

            this.dlgFileOpenModel.ShowDialog(); // choix d'un fichier...
            if (this.dlgFileOpenModel.Result != Common.Dialogs.DialogResult.Accept)
            {
                return false;
            }

            this.globalSettings.NewDocument = this.dlgFileOpenModel.InitialDirectory;

            string[] names = this.dlgFileOpenModel.FileNames;
            for (int i = 0; i < names.Length; i++)
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
            if (Misc.IsExtension(filename, ".crcolors"))
            {
                if (!this.HasCurrentDocument)
                {
                    this.CreateDocument(this.Window);
                }
                err = this.PaletteRead(filename);
            }
            else
            {
                //	Cherche si ce nom de fichier est déjà ouvert ?
                int total = this.bookDocuments.PageCount;
                for (int i = 0; i < total; i++)
                {
                    DocumentInfo di = this.documents[i];
                    if (di.document.Filename == filename)
                    {
                        if (Misc.IsExtension(filename, ".crmod"))
                        {
                            this.globalSettings.LastModelAdd(filename);
                        }
                        else
                        {
                            this.globalSettings.LastFilenameAdd(filename);
                        }
                        this.UseDocument(i);
                        this.MouseHideWait();
                        return true;
                    }
                }

                if (!this.IsRecyclableDocument())
                {
                    this.CreateDocument(this.Window);
                }
                err = this.CurrentDocument.Read(filename);
                this.initializationInProgress = true;
                this.CurrentDocument.InitializationInProgress = true;
                this.UpdateAfterRead();
                this.UpdateRulers();
                if (err == "")
                {
                    this.UpdateBookDocuments();
                    this.DialogWarnings(this.CurrentDocument.ReadWarnings);
                }
            }
            //?this.MouseHideWait();
            this.DialogError(err);
            return (err == "");
        }

        protected bool Save(bool ask)
        {
            //	Demande un nom de fichier puis enregistre le fichier.
            //	Si le document a déjà un nom de fichier et que ask=false,
            //	l'enregistrement est fait directement avec le nom connu.
            //	Affiche l'erreur éventuelle.
            //	Retourne false si le fichier n'a pas été enregistré.
            string filename = this.CurrentDocument.Filename;
            if (Document.RedirectPath(ref filename))
            {
                this.DialogWarningRedirection();
                ask = true;
            }

            if (filename == "" || ask)
            {
                this.dlgSplash.Hide();

                this.dlgFileSave.Title =
                    (this.documentType == DocumentType.Pictogram)
                        ? Res.Strings.Dialog.Save.TitlePic
                        : Res.Strings.Dialog.Save.TitleDoc;

                if (filename == "")
                {
                    this.dlgFileSave.InitialDirectory = this.globalSettings.InitialDirectory;
                    this.dlgFileSave.InitialFileName = "";
                }
                else
                {
                    this.dlgFileSave.InitialDirectory = System.IO.Path.GetDirectoryName(filename);
                    this.dlgFileSave.InitialFileName = filename;
                }

                if (
                    this.dlgFileSave.IsDirectoryRedirected
                    && this.dlgFileSave.InitialFileName != ""
                )
                {
                    this.DialogWarningRedirection();
                }

                this.dlgFileSave.ShowDialog(); // choix d'un fichier...
                if (this.dlgFileSave.Result != Common.Dialogs.DialogResult.Accept)
                {
                    return false;
                }

                filename = this.dlgFileSave.FileName;
            }

            if (Document.RedirectPath(ref filename))
            {
                this.DialogWarningRedirection();
            }

            this.MouseShowWait();
            string err = this.CurrentDocument.Write(filename);
            if (err == "")
            {
                this.globalSettings.InitialDirectory = System.IO.Path.GetDirectoryName(filename);
                this.globalSettings.LastFilenameAdd(filename);
            }
            this.MouseHideWait();
            this.DialogError(err);
            return (err == "");
        }

        protected bool SaveModel()
        {
            //	Demande un nom de fichier modèle puis enregistre le fichier.
            //	Retourne false si le fichier n'a pas été enregistré.
            string filename;

            this.dlgSplash.Hide();

            string newDocument = this.globalSettings.NewDocument;

            if (Misc.IsExtension(newDocument, ".crmod")) // ancienne définition ?
            {
                newDocument = System.IO.Path.GetDirectoryName(newDocument);
            }

            this.dlgFileSaveModel.InitialDirectory = newDocument;
            this.dlgFileSaveModel.InitialFileName = "";

            this.dlgFileSaveModel.ShowDialog(); // choix d'un fichier...
            if (this.dlgFileSaveModel.Result != Common.Dialogs.DialogResult.Accept)
            {
                return false;
            }

            filename = this.dlgFileSaveModel.FileName;

            if (Document.RedirectPath(ref filename))
            {
                this.DialogWarningRedirection();
            }

            this.MouseShowWait();
            string err = this.CurrentDocument.Write(filename);
            if (err == "")
            {
                this.globalSettings.NewDocument = this.dlgFileSaveModel.InitialDirectory;
                this.globalSettings.LastModelAdd(filename);
            }
            this.MouseHideWait();
            this.DialogError(err);
            return (err == "");
        }

        protected bool AutoSave()
        {
            //	Fait tout ce qu'il faut pour éventuellement sauvegarder le document
            //	avant de passer à autre chose.
            //	Retourne false si on ne peut pas continuer.
            Common.Dialogs.DialogResult result = this.DialogSave();
            if (result == Common.Dialogs.DialogResult.Yes)
            {
                return this.Save(false);
            }
            if (result == Common.Dialogs.DialogResult.Cancel)
            {
                return false;
            }
            return true;
        }

        protected bool AutoSaveAll()
        {
            //	Fait tout ce qu'il faut pour éventuellement sauvegarder tous les
            //	documents avant de passer à autre chose.
            //	Retourne false si on ne peut pas continuer.
            int cd = this.currentDocument;

            int total = this.bookDocuments.PageCount;
            for (int i = 0; i < total; i++)
            {
                this.currentDocument = i;
                if (!this.AutoSave())
                {
                    this.currentDocument = cd;
                    return false;
                }
            }

            this.currentDocument = cd;
            return true;
        }

        protected bool ForceSaveAll()
        {
            //	Sauvegarde tous les documents, même ceux qui sont à jour.
            int cd = this.currentDocument;

            int total = this.bookDocuments.PageCount;
            for (int i = 0; i < total; i++)
            {
                this.currentDocument = i;
                this.Save(false);
            }

            this.currentDocument = cd;
            return true;
        }

        protected bool OverwriteAll()
        {
            /*
            //	Ouvre, réécrit et ferme plusieurs fichiers.
            this.dlgSplash.Hide();

            Common.Dialogs.FileOpenDialog dialog = new Common.Dialogs.FileOpenDialog();

            dialog.InitialDirectory = this.globalSettings.InitialDirectory;
            dialog.FileName = "";
            if (this.documentType == DocumentType.Graphic)
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
            dialog.OwnerWindow = this.PlatformWindow;
            dialog.OpenDialog();
            if (dialog.Result != Common.Dialogs.DialogResult.Accept)
            {
                return false;
            }

            string[] names = dialog.FileNames;
            for (int i = 0; i < names.Length; i++)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("Open {0}", names[i]));
                string err = this.CurrentDocument.Read(names[i]);
                if (string.IsNullOrEmpty(err))
                {
                    this.CurrentDocument.Write(names[i]);
                }
            }
            return true;
            */
            throw new System.NotImplementedException();
            return false;
        }
        #endregion

        [Command("New")]
        void CommandNew(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.dlgSplash.Hide();

            if (this.documentType == DocumentType.Graphic) // CrDoc ?
            {
                string newDocument = this.globalSettings.NewDocument;

                if (Misc.IsExtension(newDocument, ".crmod")) // ancienne définition ?
                {
                    newDocument = System.IO.Path.GetDirectoryName(newDocument);
                }

                this.dlgFileNew.InitialDirectory = newDocument;

                this.dlgFileNew.ShowDialog(); // choix d'un fichier...
                if (this.dlgFileNew.Result != Common.Dialogs.DialogResult.Accept)
                {
                    return;
                }

                this.globalSettings.NewDocument = this.dlgFileNew.InitialDirectory;

                string filename = this.dlgFileNew.FileName;
                if (filename == Epsitec.Common.Dialogs.AbstractFileDialog.NewEmptyDocument) // nouveau document vide ?
                {
                    this.CreateDocument(this.Window);
                    this.CurrentDocument.Modifier.New();
                }
                else
                {
                    this.Open(filename);
                    this.CurrentDocument.ClearDirtySerialize();
                }
                this.globalSettings.LastModelAdd(filename);
                this.initializationInProgress = true;
                this.CurrentDocument.InitializationInProgress = true;
            }
            else // CrPicto ?
            {
                this.CreateDocument(this.Window);
                this.CurrentDocument.Modifier.New();
                this.initializationInProgress = true;
                this.CurrentDocument.InitializationInProgress = true;
            }
        }

        [Command("Open")]
        void CommandOpen(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.Open();
            if (this.HasCurrentDocument)
            {
                this.CurrentDocument.Modifier.ActiveViewer.Focus();
            }
        }

        [Command("OpenModel")]
        void CommandOpenModel(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.OpenModel();
            if (this.HasCurrentDocument)
            {
                this.CurrentDocument.Modifier.ActiveViewer.Focus();
            }
        }

        [Command("Save")]
        void CommandSave(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.Save(false);
        }

        [Command("SaveAs")]
        void CommandSaveAs(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.Save(true);
        }

        [Command("SaveModel")]
        void CommandSaveModel(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.SaveModel();
        }

        [Command("Close")]
        void CommandClose(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            if (!this.AutoSave())
                return;
            this.CloseDocument();
        }

        [Command("CloseAll")]
        void CommandCloseAll(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            if (!this.AutoSaveAll())
                return;
            while (this.HasCurrentDocument)
            {
                this.CloseDocument();
            }
        }

        [Command("ForceSaveAll")]
        void CommandForceSaveAll(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.ForceSaveAll();
        }

        [Command("OverwriteAll")]
        void CommandOverwriteAll(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.OverwriteAll();
        }

        [Command("NextDocument")]
        void CommandNextDocument(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            int doc = this.currentDocument + 1;
            if (doc >= this.bookDocuments.PageCount)
                doc = 0;
            this.UseDocument(doc);
        }

        [Command("PrevDocument")]
        void CommandPrevDocument(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            int doc = this.currentDocument - 1;
            if (doc < 0)
                doc = this.bookDocuments.PageCount - 1;
            this.UseDocument(doc);
        }

        [Command("LastFile")]
        void CommandLastFile(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            string value = e.CommandState.AdvancedState;
            this.Open(value);
        }

        [Command("LastModel")]
        void CommandLastModel(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            string value = e.CommandState.AdvancedState;
            if (value == Epsitec.Common.Dialogs.AbstractFileDialog.NewEmptyDocument) // nouveau document vide ?
            {
                this.CreateDocument(this.Window);
                this.CurrentDocument.Modifier.New();
                this.globalSettings.LastModelAdd(value);
                this.initializationInProgress = true;
                this.CurrentDocument.InitializationInProgress = true;
            }
            else
            {
                this.Open(value);
            }
        }

        [Command("QuitApplication")]
        [Command(ApplicationCommands.Id.Quit)]
        void CommandQuitApplication(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            if (!this.AutoSaveAll())
                return;
            this.QuitApplication();
        }

        [Command("Print")]
        void CommandPrint(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            /*
            this.dlgSplash.Hide();

            Document document = this.CurrentDocument;
            Common.Dialogs.PrintDialog dialog = document.PrintDialog;
            dialog.Document.DocumentName = Common.Support.Utilities.XmlToText(
                Common.Document.Misc.FullName(document.Filename, false)
            );
            dialog.Owner = this.PlatformWindow;

            this.dlgPrint.Show();
            */
            // bl-net8-cross printing
            throw new System.NotImplementedException();
        }

        [Command("Export")]
        void CommandExport(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.dlgSplash.Hide();

            //	Choix du type de fichier à exporter.
            if (string.IsNullOrEmpty(this.CurrentDocument.ExportFilename))
            {
                this.dlgExportType.FileType =
                    (this.documentType == DocumentType.Pictogram) ? ".png" : ".pdf";
            }
            else
            {
                this.dlgExportType.FileType = System
                    .IO.Path.GetExtension(this.CurrentDocument.ExportFilename)
                    .ToLowerInvariant();
            }

            this.dlgExportType.Show(); // choix du type de fichier...
            if (!this.dlgExportType.IsOKclicked) // annuler ?
            {
                return;
            }

            //	Si le nom du fichier du document à exporter n'existe pas, on prend celui du
            //	document en cours, ce qui rend service la plupart du temps.
            if (
                string.IsNullOrEmpty(this.CurrentDocument.ExportFilename)
                && !string.IsNullOrEmpty(this.CurrentDocument.Filename)
                && !string.IsNullOrEmpty(this.dlgExportType.FileType)
            )
            {
                var d = System.IO.Path.GetDirectoryName(this.CurrentDocument.Filename);
                var n = System.IO.Path.GetFileNameWithoutExtension(this.CurrentDocument.Filename);
                this.CurrentDocument.ExportFilename = System.IO.Path.Combine(
                    d,
                    n + this.dlgExportType.FileType
                );
            }

            //	Choix du fichier.
            if (this.dlgExportType.FileType == ".pdf")
            {
                this.dlgFileExport.Title = Res.Strings.Dialog.Export.Title1;
                this.dlgFileExport.Filters.Clear();
                this.dlgFileExport.Filters.Add(
                    new Epsitec.Common.Dialogs.FilterItem(
                        "x",
                        Res.Strings.Dialog.File.Document,
                        this.dlgExportType.FileType
                    )
                );
            }
            else
            {
                this.dlgFileExport.Title = Res.Strings.Dialog.Export.Title;
                this.dlgFileExport.Filters.Clear();
                this.dlgFileExport.Filters.Add(
                    new Epsitec.Common.Dialogs.FilterItem(
                        "x",
                        Res.Strings.Dialog.File.Image,
                        this.dlgExportType.FileType
                    )
                );
            }

            if (this.CurrentDocument.ExportDirectory == "")
            {
                if (this.CurrentDocument.Filename == "")
                {
                    this.dlgFileExport.InitialDirectory = this.globalSettings.InitialDirectory;
                }
                else
                {
                    this.dlgFileExport.InitialDirectory = System.IO.Path.GetDirectoryName(
                        this.CurrentDocument.Filename
                    );
                }
            }
            else
            {
                this.dlgFileExport.InitialDirectory = this.CurrentDocument.ExportDirectory;
            }

            this.dlgFileExport.InitialFileName = this.CurrentDocument.ExportFilename;

            this.dlgFileExport.ShowDialog(); // choix d'un fichier...
            if (this.dlgFileExport.Result != Common.Dialogs.DialogResult.Accept)
            {
                return;
            }

            this.CurrentDocument.ExportDirectory = this.dlgFileExport.InitialDirectory;
            this.CurrentDocument.ExportFilename = this.dlgFileExport.FileName;

            //	Choix des options d'exportation.
            if (this.dlgExportType.FileType == ".pdf")
            {
                this.dlgExportPDF.Show(this.CurrentDocument.ExportFilename);
            }
            else if (this.dlgExportType.FileType == ".ico")
            {
                this.dlgExportICO.Show(this.CurrentDocument.ExportFilename);
            }
            else
            {
                this.CurrentDocument.Printer.ImageFormat = Printer.GetImageFormat(
                    this.dlgExportType.FileType
                );
                this.dlgExport.Show(this.CurrentDocument.ExportFilename);
            }
        }

        [Command("QuickExport")]
        void CommandQuickExport(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            if (string.IsNullOrEmpty(this.CurrentDocument.Filename))
            {
                return;
            }

            var d = System.IO.Path.GetDirectoryName(this.CurrentDocument.Filename);
            var n = System.IO.Path.GetFileNameWithoutExtension(this.CurrentDocument.Filename);

            string ext = ".xxx";
            var compression = ImageCompression.None;
            int depth = 24;

            switch (this.globalSettings.QuickExportFormat)
            {
                case ImageFormat.Jpeg:
                    ext = ".jpg";
                    break;
                case ImageFormat.Gif:
                    ext = ".gif";
                    depth = 8;
                    break;
                case ImageFormat.Tiff:
                    ext = ".tif";
                    compression = ImageCompression.Lzw;
                    depth = 32;
                    break;
                case ImageFormat.Png:
                    ext = ".png";
                    compression = ImageCompression.Lzw;
                    depth = 32;
                    break;
                case ImageFormat.Bmp:
                    ext = ".bmp";
                    break;
            }
            string filename = System.IO.Path.Combine(d, n + ext);

            if (!string.IsNullOrEmpty(filename))
            {
                this.CurrentDocument.Printer.ImageOnlySelected = false;
                this.CurrentDocument.Printer.ImageCrop = ExportImageCrop.Page;
                this.CurrentDocument.Printer.ImageFormat = this.globalSettings.QuickExportFormat;
                this.CurrentDocument.Printer.ImageDpi = this.globalSettings.QuickExportDpi;
                this.CurrentDocument.Printer.ImageCompression = compression;
                this.CurrentDocument.Printer.ImageDepth = depth;
                this.CurrentDocument.Printer.ImageQuality = 1.0;
                this.CurrentDocument.Printer.ImageAA = 1.0;
                this.CurrentDocument.Printer.ImageAlphaCorrect = true;
                this.CurrentDocument.Printer.ImageAlphaPremultiplied = true;

                string err = this.CurrentDocument.Export(filename);
            }
        }

        [Command("Glyphs")]
        void CommandGlyphs(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.dlgSplash.Hide();

            if (this.glyphsState.ActiveState == ActiveState.No)
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

        [Command("Replace")]
        void CommandReplace(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.dlgSplash.Hide();

            this.dlgReplace.Show();
            this.replaceState.ActiveState = ActiveState.Yes;
        }

        [Command("FindNext")]
        [Command("FindPrev")]
        [Command("FindDefNext")]
        [Command("FindDefPrev")]
        void CommandFind(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            if (!this.HasCurrentDocument)
                return;

            string commandName = e.Command.CommandId;

            bool isPrev = (commandName == "FindPrev" || commandName == "FindDefPrev");
            bool isDef = (commandName == "FindDefNext" || commandName == "FindDefPrev");

            if (isDef) // Ctrl-F3 ?
            {
                string word = this.CurrentDocument.Modifier.GetSelectedWord(); // mot actuellement sélectionné
                if (word == null)
                    return;
                this.dlgReplace.FindText = word; // il devient le critère de recherche
            }

            Misc.StringSearch mode = this.dlgReplace.Mode;
            mode &= ~Misc.StringSearch.EndToStart;
            if (isPrev)
                mode |= Misc.StringSearch.EndToStart; // Shift-F3 ?
            this.dlgReplace.Mode = mode; // modifie juste la direction de la recherche

            this.dlgReplace.MemorizeTexts();

            this.CurrentDocument.Modifier.TextReplace(this.dlgReplace.FindText, null, mode);
        }

        #region PaletteIO
        [Command(Common.Widgets.Res.CommandIds.ColorPalette.SelectDefaultColors)]
        void CommandNewPaletteDefault()
        {
            this.CurrentDocument.GlobalSettings.ColorCollection.CopyFrom(
                new ColorCollection(ColorCollectionType.Default)
            );
        }

        [Command(Common.Widgets.Res.CommandIds.ColorPalette.SelectRainbowColors)]
        void NewPaletteRainbow()
        {
            this.CurrentDocument.GlobalSettings.ColorCollection.CopyFrom(
                new ColorCollection(ColorCollectionType.Rainbow)
            );
        }

        [Command(Common.Widgets.Res.CommandIds.ColorPalette.SelectLightColors)]
        void NewPaletteLight()
        {
            this.CurrentDocument.GlobalSettings.ColorCollection.CopyFrom(
                new ColorCollection(ColorCollectionType.Light)
            );
        }

        [Command(Common.Widgets.Res.CommandIds.ColorPalette.SelectDarkColors)]
        void NewPaletteDark()
        {
            this.CurrentDocument.GlobalSettings.ColorCollection.CopyFrom(
                new ColorCollection(ColorCollectionType.Dark)
            );
        }

        [Command(Common.Widgets.Res.CommandIds.ColorPalette.SelectGrayColors)]
        void CommandNewPaletteGray()
        {
            this.CurrentDocument.GlobalSettings.ColorCollection.CopyFrom(
                new ColorCollection(ColorCollectionType.Gray)
            );
        }

        [Command(Common.Widgets.Res.CommandIds.ColorPalette.Load)]
        void CommandOpenPalette()
        {
            /*
            this.dlgSplash.Hide();

            Common.Dialogs.FileOpenDialog dialog = new Common.Dialogs.FileOpenDialog();

            dialog.InitialDirectory = this.CurrentDocument.GlobalSettings.ColorCollectionDirectory;
            dialog.FileName = this.CurrentDocument.GlobalSettings.ColorCollectionFilename;
            dialog.Title = Res.Strings.Dialog.Open.TitleCol;
            dialog.Filters.Add("crcolors", Res.Strings.Dialog.FileCol, "*.crcolors");
            dialog.FilterIndex = 0;
            dialog.OwnerWindow = this.PlatformWindow;
            dialog.OpenDialog();
            if (dialog.Result != Common.Dialogs.DialogResult.Accept)
                return;

            this.CurrentDocument.GlobalSettings.ColorCollectionDirectory =
                System.IO.Path.GetDirectoryName(dialog.FileName);
            this.CurrentDocument.GlobalSettings.ColorCollectionFilename =
                System.IO.Path.GetFileName(dialog.FileName);

            string err = this.PaletteRead(dialog.FileName);
            this.DialogError(err);
            */
            throw new System.NotImplementedException();
        }

        [Command(Common.Widgets.Res.CommandIds.ColorPalette.Save)]
        void CommandSavePalette()
        {
            /*
            this.dlgSplash.Hide();

            Common.Dialogs.FileSaveDialog dialog = new Common.Dialogs.FileSaveDialog();

            dialog.InitialDirectory = this.CurrentDocument.GlobalSettings.ColorCollectionDirectory;
            dialog.FileName = this.CurrentDocument.GlobalSettings.ColorCollectionFilename;
            dialog.Title = Res.Strings.Dialog.Save.TitleCol;
            dialog.Filters.Add("crcolors", Res.Strings.Dialog.FileCol, "*.crcolors");
            dialog.FilterIndex = 0;
            dialog.PromptForOverwriting = true;
            dialog.OwnerWindow = this.PlatformWindow;
            dialog.OpenDialog();
            if (dialog.Result != Common.Dialogs.DialogResult.Accept)
                return;

            this.CurrentDocument.GlobalSettings.ColorCollectionDirectory =
                System.IO.Path.GetDirectoryName(dialog.FileName);
            this.CurrentDocument.GlobalSettings.ColorCollectionFilename =
                System.IO.Path.GetFileName(dialog.FileName);

            string err = this.PaletteWrite(dialog.FileName);
            this.DialogError(err);
            */
            throw new System.NotImplementedException();
        }

        public string PaletteRead(string filename)
        {
            //	Lit la collection de couleurs à partir d'un fichier.
            try
            {
                using (Stream stream = File.OpenRead(filename))
                {
                    SoapFormatter formatter = new SoapFormatter();
                    formatter.Binder = new GenericDeserializationBinder();
                    ColorCollection cc = (ColorCollection)formatter.Deserialize(stream);
                    cc.CopyTo(this.CurrentDocument.GlobalSettings.ColorCollection);
                }
            }
            catch (System.Exception e)
            {
                return e.Message;
            }

            return ""; // ok
        }

        public string PaletteWrite(string filename)
        {
            //	Ecrit la collection de couleurs dans un fichier.
            if (File.Exists(filename))
            {
                File.Delete(filename);
            }

            try
            {
                using (Stream stream = File.OpenWrite(filename))
                {
                    SoapFormatter formatter = new SoapFormatter();
                    formatter.Serialize(
                        stream,
                        this.CurrentDocument.GlobalSettings.ColorCollection
                    );
                }
            }
            catch (System.Exception e)
            {
                return e.Message;
            }

            return ""; // ok
        }
        #endregion


        [Command("Delete")]
        void CommandDelete()
        {
            this.CurrentDocument.Modifier.DeleteSelection();
        }

        [Command("Duplicate")]
        void CommandDuplicate(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            Point move = this.CurrentDocument.Modifier.EffectiveDuplicateMove;
            this.CurrentDocument.Modifier.DuplicateSelection(move);
        }

        [Command("Cut")]
        void CommandCut(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.CutSelection();
        }

        [Command("Copy")]
        void CommandCopy(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.CopySelection();
        }

        [Command("Paste")]
        void CommandPaste(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.Paste();
        }

        [Command(Common.Document.Res.CommandIds.FontBold)]
        [Command(Common.Document.Res.CommandIds.FontItalic)]
        [Command(Common.Document.Res.CommandIds.FontUnderline)]
        [Command(Commands.FontOverline)]
        [Command(Commands.FontStrikeout)]
        [Command(Commands.FontSubscript)]
        [Command(Commands.FontSuperscript)]
        [Command(Commands.FontSizePlus)]
        [Command(Commands.FontSizeMinus)]
        [Command(Commands.FontClear)]
        [Command("ParagraphLeadingPlus")]
        [Command("ParagraphLeadingMinus")]
        [Command("ParagraphIndentPlus")]
        [Command("ParagraphIndentMinus")]
        [Command("ParagraphClear")]
        void CommandFont(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Wrappers.ExecuteCommand(e.Command.CommandId, null);
        }

        [Command("ParagraphLeading")]
        [Command("ParagraphJustif")]
        void CommandCombo(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            IconButtonCombo combo = e.Source as IconButtonCombo;
            CommandState cs = this.commandContext.GetCommandState(e.Command);
            if (combo != null && cs != null)
            {
                cs.AdvancedState = combo.SelectedName;
                this.CurrentDocument.Wrappers.ExecuteCommand(e.Command.CommandId, cs.AdvancedState);
            }
            else
            {
                this.CurrentDocument.Wrappers.ExecuteCommand(e.Command.CommandId, null);
            }
        }

        [Command("Undo")]
        void CommandUndo(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.Undo(1);
        }

        [Command("Redo")]
        void CommandRedo(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.Redo(1);
        }

        [Command("UndoRedoList")]
        void CommandUndoRedoList(CommandDispatcher dispatcher, CommandEventArgs e)
        {
#if false
			Widget button = this.hToolBar.FindChild("Undo");
			if ( button == null )  return;
			Point pos = button.MapClientToScreen(new Point(0, 1));
			VMenu menu = this.CurrentDocument.Modifier.CreateUndoRedoMenu(null);
			menu.Host = this;
			menu.Accepted += this.HandleUndoRedoMenuAccepted;
			menu.Rejected += this.HandleUndoRedoMenuRejected;
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

#if false
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
#endif

        [Command("UndoRedoListDo")]
        void CommandUndoRedoListDo(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            string value = e.CommandState.AdvancedState;

            int nb = System.Convert.ToInt32(value);
            if (nb > 0)
            {
                this.CurrentDocument.Modifier.Undo(nb);
            }
            else
            {
                this.CurrentDocument.Modifier.Redo(-nb);
            }
        }

        [Command("OrderUpOne")]
        void CommandOrderUpOne(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.OrderUpOneSelection();
        }

        [Command("OrderDownOne")]
        void CommandOrderDownOne(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.OrderDownOneSelection();
        }

        [Command("OrderUpAll")]
        void CommandOrderUpAll(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.OrderUpAllSelection();
        }

        [Command("OrderDownAll")]
        void CommandOrderDownAll(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.OrderDownAllSelection();
        }

        [Command("Rotate90")]
        void CommandRotate90(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.RotateSelection(90);
        }

        [Command("Rotate180")]
        void CommandRotate180(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.RotateSelection(180);
        }

        [Command("Rotate270")]
        void CommandRotate270(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.RotateSelection(270);
        }

        [Command("RotateFreeCCW")]
        void CommandRotateFreeCCW(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            double angle = this.CurrentDocument.Modifier.RotateAngle;
            this.CurrentDocument.Modifier.RotateSelection(angle);
        }

        [Command("RotateFreeCW")]
        void CommandRotateFreeCW(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            double angle = this.CurrentDocument.Modifier.RotateAngle;
            this.CurrentDocument.Modifier.RotateSelection(-angle);
        }

        [Command("MirrorH")]
        void CommandMirrorH(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.MirrorSelection(true);
        }

        [Command("MirrorV")]
        void CommandMirrorV(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.MirrorSelection(false);
        }

        [Command("ScaleMul2")]
        void CommandScaleMul2(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.ScaleSelection(2.0);
        }

        [Command("ScaleDiv2")]
        void CommandScaleDiv2(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.ScaleSelection(0.5);
        }

        [Command("ScaleMulFree")]
        void CommandScaleMulFree(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            double scale = this.CurrentDocument.Modifier.ScaleFactor;
            this.CurrentDocument.Modifier.ScaleSelection(scale);
        }

        [Command("ScaleDivFree")]
        void CommandScaleDivFree(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            double scale = this.CurrentDocument.Modifier.ScaleFactor;
            this.CurrentDocument.Modifier.ScaleSelection(1.0 / scale);
        }

        [Command("AlignLeft")]
        void CommandAlignLeft(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.AlignSelection(-1, true);
        }

        [Command("AlignCenterX")]
        void CommandAlignCenterX(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.AlignSelection(0, true);
        }

        [Command("AlignRight")]
        void CommandAlignRight(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.AlignSelection(1, true);
        }

        [Command("AlignTop")]
        void CommandAlignTop(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.AlignSelection(1, false);
        }

        [Command("AlignCenterY")]
        void CommandAlignCenterY(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.AlignSelection(0, false);
        }

        [Command("AlignBottom")]
        void CommandAlignBottom(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.AlignSelection(-1, false);
        }

        [Command("AlignGrid")]
        void CommandAlignGrid(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.AlignGridSelection();
        }

        [Command("Reset")]
        void CommandReset(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.ResetSelection();
        }

        [Command("ShareLeft")]
        void CommandShareLeft(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.ShareSelection(-1, true);
        }

        [Command("ShareCenterX")]
        void CommandShareCenterX(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.ShareSelection(0, true);
        }

        [Command("ShareSpaceX")]
        void CommandShareSpaceX(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.SpaceSelection(true);
        }

        [Command("ShareRight")]
        void CommandShareRight(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.ShareSelection(1, true);
        }

        [Command("ShareTop")]
        void CommandShareTop(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.ShareSelection(1, false);
        }

        [Command("ShareCenterY")]
        void CommandShareCenterY(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.ShareSelection(0, false);
        }

        [Command("ShareSpaceY")]
        void CommandShareSpaceY(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.SpaceSelection(false);
        }

        [Command("ShareBottom")]
        void CommandShareBottom(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.ShareSelection(-1, false);
        }

        [Command("AdjustWidth")]
        void CommandAdjustWidth(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.AdjustSelection(true);
        }

        [Command("AdjustHeight")]
        void CommandAdjustHeight(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.AdjustSelection(false);
        }

        [Command("ColorToRgb")]
        void CommandColorToRgb(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.ColorSelection(ColorSpace.Rgb);
        }

        [Command("ColorToCmyk")]
        void CommandColorToCmyk(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.ColorSelection(ColorSpace.Cmyk);
        }

        [Command("ColorToGray")]
        void CommandColorToGray(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.ColorSelection(ColorSpace.Gray);
        }

        [Command("ColorStrokeDark")]
        void CommandColorStrokeDark(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            double adjust = this.CurrentDocument.Modifier.ColorAdjust;
            this.CurrentDocument.Modifier.ColorSelection(-adjust, true);
        }

        [Command("ColorStrokeLight")]
        void CommandColorStrokeLight(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            double adjust = this.CurrentDocument.Modifier.ColorAdjust;
            this.CurrentDocument.Modifier.ColorSelection(adjust, true);
        }

        [Command("ColorFillDark")]
        void CommandColorFillDark(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            double adjust = this.CurrentDocument.Modifier.ColorAdjust;
            this.CurrentDocument.Modifier.ColorSelection(-adjust, false);
        }

        [Command("ColorFillLight")]
        void CommandColorFillLight(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            double adjust = this.CurrentDocument.Modifier.ColorAdjust;
            this.CurrentDocument.Modifier.ColorSelection(adjust, false);
        }

        [Command("Merge")]
        void CommandMerge(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.MergeSelection();
        }

        [Command("Extract")]
        void CommandExtract(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.ExtractSelection();
        }

        [Command("Group")]
        void CommandGroup(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.GroupSelection();
        }

        [Command("Ungroup")]
        void CommandUngroup(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.UngroupSelection();
        }

        [Command("Inside")]
        void CommandInside(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.InsideSelection();
        }

        [Command("Outside")]
        void CommandOutside(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.OutsideSelection();
        }

        [Command("Combine")]
        void CommandCombine(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.CombineSelection();
        }

        [Command("Uncombine")]
        void CommandUncombine(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.UncombineSelection();
        }

        [Command("ToBezier")]
        void CommandToBezier(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.ToBezierSelection();
        }

        [Command("ToPoly")]
        void CommandToPoly(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.ToPolySelection();
        }

        [Command("ToTextBox2")]
        void CommandToTextBox2(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.ToTextBox2Selection();
        }

        [Command("ToSimplest")]
        void CommandToSimplest(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.ToSimplestSelection();
        }

        [Command("Fragment")]
        void CommandFragment(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.FragmentSelection();
        }

        [Command("ShaperHandleAdd")]
        [Command("ShaperHandleSub")]
        [Command("ShaperHandleToLine")]
        [Command("ShaperHandleToCurve")]
        [Command("ShaperHandleSym")]
        [Command("ShaperHandleSmooth")]
        [Command("ShaperHandleDis")]
        [Command("ShaperHandleInline")]
        [Command("ShaperHandleFree")]
        [Command("ShaperHandleSimply")]
        [Command("ShaperHandleCorner")]
        [Command("ShaperHandleContinue")]
        [Command("ShaperHandleSharp")]
        [Command("ShaperHandleRound")]
        void CommandShaperHandle(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.ShaperHandleCommand(e.Command.CommandId);
        }

        [Command("BooleanOr")]
        void CommandBooleanOr(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.BooleanSelection(Drawing.PathOperation.Or);
        }

        [Command("BooleanAnd")]
        void CommandBooleanAnd(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.BooleanSelection(Drawing.PathOperation.And);
        }

        [Command("BooleanXor")]
        void CommandBooleanXor(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.BooleanSelection(Drawing.PathOperation.Xor);
        }

        [Command("BooleanFrontMinus")]
        void CommandBooleanFrontMinus(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.BooleanSelection(Drawing.PathOperation.AMinusB);
        }

        [Command("BooleanBackMinus")]
        void CommandBooleanBackMinus(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.BooleanSelection(Drawing.PathOperation.BMinusA);
        }

        [Command("Grid")]
        void CommandGrid(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
            context.GridActive = !context.GridActive;
            context.GridShow = context.GridActive;
        }

        [Command("TextGrid")]
        void CommandTextGrid(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
            context.TextGridShow = !context.TextGridShow;
        }

        [Command(Commands.TextShowControlCharacters)]
        void CommandTextShowControlCharacters(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
            context.TextShowControlCharacters = !context.TextShowControlCharacters;
        }

        [Command("TextFontFilter")]
        void CommandTextFontFilter(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
            context.TextFontFilter = !context.TextFontFilter;
        }

        [Command("TextFontSampleAbc")]
        void CommandTextFontSampleAbc(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
            context.TextFontSampleAbc = !context.TextFontSampleAbc;
        }

        [Command("TextInsertQuad")]
        void CommandTextInsertQuad(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            if (!this.HasCurrentDocument)
                return;
            this.CurrentDocument.Modifier.EditInsertText(Common.Text.Unicode.Code.NoBreakSpace);
        }

        [Command("TextInsertNewFrame")]
        void CommandTextInsertNewFrame(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            if (!this.HasCurrentDocument)
                return;
            this.CurrentDocument.Modifier.EditInsertText(
                Common.Text.Properties.BreakProperty.NewFrame
            );
        }

        [Command("TextInsertNewPage")]
        void CommandTextInsertNewPage(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            if (!this.HasCurrentDocument)
                return;
            this.CurrentDocument.Modifier.EditInsertText(
                Common.Text.Properties.BreakProperty.NewPage
            );
        }

        [Command("Constrain")]
        void CommandConstrain(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
            context.ConstrainActive = !context.ConstrainActive;
        }

        [Command("Magnet")]
        void CommandMagnet(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
            context.MagnetActive = !context.MagnetActive;
        }

        [Command("MagnetLayer")]
        void CommandMagnetLayer(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
            int rank = context.CurrentLayer;
            this.CurrentDocument.Modifier.MagnetLayerInvert(rank);
        }

        [Command("Rulers")]
        void CommandRulers(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
            context.RulersShow = !context.RulersShow;
            this.UpdateRulers();
        }

        [Command("Labels")]
        void CommandLabels(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
            context.LabelsShow = !context.LabelsShow;
        }

        [Command("Aggregates")]
        void CommandAggregates(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
            context.AggregatesShow = !context.AggregatesShow;
        }

        [Command("Preview")]
        void CommandPreview(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
            context.PreviewActive = !context.PreviewActive;
        }

        //		[Command ("DeselectAll")]
        [Command(Common.Widgets.Res.CommandIds.DeselectAll)]
        void CommandDeselectAll(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.DeselectAllCmd();
        }

        //		[Command ("SelectAll")]
        [Command(Common.Widgets.Res.CommandIds.SelectAll)]
        void CommandSelectAll(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.SelectAll();
        }

        [Command("SelectInvert")]
        void CommandSelectInvert(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.InvertSelection();
        }

        [Command("SelectTotal")]
        void CommandSelectTotal(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.Tool = "ToolSelect";
            Viewer viewer = this.CurrentDocument.Modifier.ActiveViewer;
            viewer.PartialSelect = false;
        }

        [Command("SelectPartial")]
        void CommandSelectPartial(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.Tool = "ToolSelect";
            Viewer viewer = this.CurrentDocument.Modifier.ActiveViewer;
            viewer.PartialSelect = true;
        }

        [Command("SelectorAuto")]
        void CommandSelectorAuto(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.Tool = "ToolSelect";
            Viewer viewer = this.CurrentDocument.Modifier.ActiveViewer;
            viewer.SelectorType = SelectorType.Auto;
        }

        [Command("SelectorIndividual")]
        void CommandSelectorIndividual(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.Tool = "ToolSelect";
            Viewer viewer = this.CurrentDocument.Modifier.ActiveViewer;
            viewer.SelectorType = SelectorType.Individual;
        }

        [Command("SelectorScaler")]
        void CommandSelectorScaler(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.Tool = "ToolSelect";
            Viewer viewer = this.CurrentDocument.Modifier.ActiveViewer;
            viewer.SelectorType = SelectorType.Scaler;
        }

        [Command("SelectorStretch")]
        void CommandSelectorStretch(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.Tool = "ToolSelect";
            Viewer viewer = this.CurrentDocument.Modifier.ActiveViewer;
            viewer.SelectorType = SelectorType.Stretcher;
        }

        [Command("SelectorStretchType")]
        void CommandSelectorStretchType(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            DocumentInfo di = this.CurrentDocumentInfo;
            HToolBar toolbar = di.containerPrincipal.SelectorToolBar;
            Widget button = toolbar.FindChild("SelectorStretch");
            Widget type = toolbar.FindChild("SelectorStretchType");
            if (button == null)
                return;
            VMenu menu = di.containerPrincipal.CreateStretchTypeMenu(null);
            menu.Host = this;
            menu.Behavior.Accepted += this.HandleStretchTypeMenuAccepted;
            menu.Behavior.Rejected += this.HandleStretchTypeMenuRejected;
            menu.MinWidth = button.ActualWidth + type.ActualWidth;
            TextFieldCombo.AdjustComboSize(button, menu, false);
            menu.ShowAsComboList(button, Point.Zero, type);
            this.WidgetStretchTypeMenuEngaged(true);
        }

        private void HandleStretchTypeMenuAccepted(object sender)
        {
            this.WidgetStretchTypeMenuEngaged(true);
        }

        private void HandleStretchTypeMenuRejected(object sender)
        {
            Viewer viewer = this.CurrentDocument.Modifier.ActiveViewer;
            bool activate = (viewer.SelectorType == SelectorType.Stretcher);
            this.WidgetStretchTypeMenuEngaged(activate);
        }

        protected void WidgetStretchTypeMenuEngaged(bool activate)
        {
            Widget button;

            DocumentInfo di = this.CurrentDocumentInfo;
            HToolBar toolbar = di.containerPrincipal.SelectorToolBar;
            if (toolbar == null)
                return;

            button = toolbar.FindChild("SelectorStretch");
            if (button != null)
                button.ActiveState = activate ? ActiveState.Yes : ActiveState.No;
        }

        [Command("SelectorStretchTypeDo")]
        void CommandSelectorStretchTypeDo(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            string value = e.CommandState.AdvancedState;
            int nb = System.Convert.ToInt32(value);
            Viewer viewer = this.CurrentDocument.Modifier.ActiveViewer;
            viewer.SelectorTypeStretch = (SelectorTypeStretch)nb;
        }

        [Command("SelectorAdaptLine")]
        void CommandSelectorAdaptLine(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            Viewer viewer = this.CurrentDocument.Modifier.ActiveViewer;
            viewer.SelectorAdaptLine = !viewer.SelectorAdaptLine;
        }

        [Command("SelectorAdaptText")]
        void CommandSelectorAdaptText(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            Viewer viewer = this.CurrentDocument.Modifier.ActiveViewer;
            viewer.SelectorAdaptText = !viewer.SelectorAdaptText;
        }

        [Command("HideHalf")]
        void CommandHideHalf(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.Tool = "ToolSelect";
            DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
            context.HideHalfActive = !context.HideHalfActive;
        }

        [Command("HideSel")]
        void CommandHideSel(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.HideSelection();
        }

        [Command("HideRest")]
        void CommandHideRest(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.HideRest();
        }

        [Command("HideCancel")]
        void CommandHideCancel(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.HideCancel();
        }

        [Command("ZoomMin")]
        void CommandZoomMin(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.ZoomChange(0.0001);
        }

        [Command("ZoomPage")]
        void CommandZoomPage(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
            this.CurrentDocument.Modifier.ZoomMemorize();
            context.ZoomPageAndCenter();
        }

        [Command("ZoomPageWidth")]
        void CommandZoomPageWidth(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
            this.CurrentDocument.Modifier.ZoomMemorize();
            context.ZoomPageWidthAndCenter();
        }

        [Command("ZoomDefault")]
        void CommandZoomDefault(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
            this.CurrentDocument.Modifier.ZoomMemorize();
            context.ZoomDefaultAndCenter();
        }

        [Command("ZoomSel")]
        void CommandZoomSel(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.ZoomSel();
        }

        [Command("ZoomSelWidth")]
        void CommandZoomSelWidth(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.ZoomSelWidth();
        }

        [Command("ZoomPrev")]
        void CommandZoomPrev(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.ZoomPrev();
        }

        [Command("ZoomSub")]
        void CommandZoomSub(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.ZoomChange(1.0 / 1.2);
        }

        [Command("ZoomAdd")]
        void CommandZoomAdd(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.ZoomChange(1.2);
        }

        [Command("ZoomChange")]
        void CommandZoomChange(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            string value = e.CommandState.AdvancedState;
            double zoom = System.Convert.ToDouble(value);
            this.CurrentDocument.Modifier.ZoomValue(zoom);
        }

        [Command("Object")]
        new void CommandObject(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            //	Exécute une commande locale à un objet.
            Widget widget = e.Source as Widget;
            this.CurrentDocument.Modifier.ActiveViewer.ExecuteObjectCommand(widget.Name);
        }

        [Command("ArrayOutlineFrame")]
        [Command("ArrayOutlineHoriz")]
        [Command("ArrayOutlineVerti")]
        [Command("ArrayAddColumnLeft")]
        [Command("ArrayAddColumnRight")]
        [Command("ArrayAddRowTop")]
        [Command("ArrayAddRowBottom")]
        [Command("ArrayDelColumn")]
        [Command("ArrayDelRow")]
        [Command("ArrayAlignColumn")]
        [Command("ArrayAlignRow")]
        [Command("ArraySwapColumn")]
        [Command("ArraySwapRow")]
        [Command("ArrayLook")]
        void CommandArray(CommandDispatcher dispatcher, CommandEventArgs e) { }

        [Command("Settings")]
        void CommandSettings(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.dlgSplash.Hide();

            if (this.settingsState.ActiveState == ActiveState.No)
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

        [Command("Infos")]
        void CommandInfos(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.dlgSplash.Hide();

            if (this.infosState.ActiveState == ActiveState.No)
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

        [Command("PageStack")]
        void CommandPageStack(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.dlgSplash.Hide();

            if (this.pageStackState.ActiveState == ActiveState.No)
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

        [Command("UpdateApplication")]
        void CommandUpdateApplication(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.StartCheck(true);
            this.EndCheck(true);
        }

        [Command("AboutApplication")]
        void CommandAboutApplication(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.dlgSplash.Hide();
            this.dlgAbout.Show();
        }

        [Command("MoveLeftFree")]
        void CommandMoveLeftFree(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            double dx = this.CurrentDocument.Modifier.MoveDistanceH;
            this.CurrentDocument.Modifier.MoveSelection(new Point(-dx, 0));
        }

        [Command("MoveRightFree")]
        void CommandMoveRightFree(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            double dx = this.CurrentDocument.Modifier.MoveDistanceH;
            this.CurrentDocument.Modifier.MoveSelection(new Point(dx, 0));
        }

        [Command("MoveUpFree")]
        void CommandMoveUpFree(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            double dy = this.CurrentDocument.Modifier.MoveDistanceV;
            this.CurrentDocument.Modifier.MoveSelection(new Point(0, dy));
        }

        [Command("MoveDownFree")]
        void CommandMoveDownFree(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            double dy = this.CurrentDocument.Modifier.MoveDistanceV;
            this.CurrentDocument.Modifier.MoveSelection(new Point(0, -dy));
        }

        [Command("MoveLeftNorm")]
        void CommandMoveLeftNorm(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.MoveSelection(new Point(-1, 0), 0);
        }

        [Command("MoveRightNorm")]
        void CommandMoveRightNorm(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.MoveSelection(new Point(1, 0), 0);
        }

        [Command("MoveUpNorm")]
        void CommandMoveUpNorm(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.MoveSelection(new Point(0, 1), 0);
        }

        [Command("MoveDownNorm")]
        void CommandMoveDownNorm(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.MoveSelection(new Point(0, -1), 0);
        }

        [Command("MoveLeftCtrl")]
        void CommandMoveLeftCtrl(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.MoveSelection(new Point(-1, 0), -1);
        }

        [Command("MoveRightCtrl")]
        void CommandMoveRightCtrl(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.MoveSelection(new Point(1, 0), -1);
        }

        [Command("MoveUpCtrl")]
        void CommandMoveUpCtrl(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.MoveSelection(new Point(0, 1), -1);
        }

        [Command("MoveDownCtrl")]
        void CommandMoveDownCtrl(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.MoveSelection(new Point(0, -1), -1);
        }

        [Command("MoveLeftShift")]
        void CommandMoveLeftShift(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.MoveSelection(new Point(-1, 0), 1);
        }

        [Command("MoveRightShift")]
        void CommandMoveRightShift(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.MoveSelection(new Point(1, 0), 1);
        }

        [Command("MoveUpShift")]
        void CommandMoveUpShift(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.MoveSelection(new Point(0, 1), 1);
        }

        [Command("MoveDownShift")]
        void CommandMoveDownShift(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.MoveSelection(new Point(0, -1), 1);
        }

        [Command("PagePrev")]
        void CommandPagePrev(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
            if (context.CurrentPage > 0)
            {
                context.CurrentPage = context.CurrentPage - 1;
            }
        }

        [Command("PageNext")]
        void CommandPageNext(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
            if (context.CurrentPage < context.TotalPages() - 1)
            {
                context.CurrentPage = context.CurrentPage + 1;
            }
        }

        private void HandleQuickPageMenu(object sender, MessageEventArgs e)
        {
            //	Bouton "menu des pages" cliqué.
            Button button = sender as Button;
            if (button == null)
                return;
            VMenu menu = this.CreatePagesMenu(null);
            menu.Host = button.Window;
            TextFieldCombo.AdjustComboSize(button, menu, false);
            menu.ShowAsComboList(button, Point.Zero, button);
        }

        [Command("PageSelect")]
        void CommandPageSelect(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
            string value = e.CommandState.AdvancedState;
            int sel = System.Convert.ToInt32(value);
            context.CurrentPage = sel;
        }

        [Command("PageMiniatures")]
        void CommandPageMiniatures(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            DocumentInfo di = this.CurrentDocumentInfo;
            di.document.PageMiniatures.IsPanelShowed = !di.document.PageMiniatures.IsPanelShowed;
            di.pagePane.Visibility = di.document.PageMiniatures.IsPanelShowed;
            di.quickPageMiniatures.GlyphShape = di.document.PageMiniatures.IsPanelShowed
                ? GlyphShape.ArrowDown
                : GlyphShape.ArrowUp;
            di.pageMiniatures.UpdatePageAfterChanging();
        }

        [Command("PageNew")]
        void CommandPageNew(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
            int rank = context.CurrentPage;
            this.CurrentDocument.Modifier.PageNew(rank + 1, "");
        }

        [Command("PageDuplicate")]
        void CommandPageDuplicate(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
            int rank = context.CurrentPage;
            this.CurrentDocument.Modifier.PageDuplicate(rank, rank + 1, "");
        }

        [Command("PageDelete")]
        void CommandPageDelete(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
            int rank = context.CurrentPage;
            this.CurrentDocument.Modifier.PageDelete(rank);
        }

        [Command("PageUp")]
        void CommandPageUp(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
            int rank = context.CurrentPage;
            this.CurrentDocument.Modifier.PageSwap(rank, rank - 1);
            context.CurrentPage = rank - 1;
        }

        [Command("PageDown")]
        void CommandPageDown(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
            int rank = context.CurrentPage;
            this.CurrentDocument.Modifier.PageSwap(rank, rank + 1);
            context.CurrentPage = rank + 1;
        }

        [Command("LayerPrev")]
        void CommandLayerPrev(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
            if (context.CurrentLayer > 0)
            {
                context.CurrentLayer = context.CurrentLayer - 1;
            }
        }

        [Command("LayerNext")]
        void CommandLayerNext(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
            if (context.CurrentLayer < context.TotalLayers() - 1)
            {
                context.CurrentLayer = context.CurrentLayer + 1;
            }
        }

        private void HandleQuickLayerMenu(object sender, MessageEventArgs e)
        {
            //	Bouton "menu des calques" cliqué.
            Button button = sender as Button;
            if (button == null)
                return;
            VMenu menu = this.CreateLayersMenu(null);
            menu.Host = button.Window;
            TextFieldCombo.AdjustComboSize(button, menu, false);
            menu.ShowAsComboList(button, Point.Zero, button);
        }

        [Command("LayerSelect")]
        void CommandLayerSelect(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
            string value = e.CommandState.AdvancedState;
            int sel = System.Convert.ToInt32(value);
            context.CurrentLayer = sel;
        }

        [Command("LayerMiniatures")]
        void CommandLayerMiniatures(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            DocumentInfo di = this.CurrentDocumentInfo;
            di.document.LayerMiniatures.IsPanelShowed = !di.document.LayerMiniatures.IsPanelShowed;
            di.layerPane.Visibility = di.document.LayerMiniatures.IsPanelShowed;
            di.quickLayerMiniatures.GlyphShape = di.document.LayerMiniatures.IsPanelShowed
                ? GlyphShape.ArrowRight
                : GlyphShape.ArrowLeft;
            di.layerMiniatures.UpdateLayerAfterChanging();
        }

        [Command("LayerNew")]
        void CommandLayerNew(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
            int rank = context.CurrentLayer;
            this.CurrentDocument.Modifier.LayerNew(rank + 1, "");
        }

        [Command("LayerDuplicate")]
        void CommandLayerDuplicate(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
            int rank = context.CurrentLayer;
            this.CurrentDocument.Modifier.LayerDuplicate(rank, rank + 1, "");
        }

        [Command("LayerNewSel")]
        void CommandLayerNewSel(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
            int rank = context.CurrentLayer;
            this.CurrentDocument.Modifier.LayerNewSel(rank, "");
        }

        [Command("LayerMergeUp")]
        void CommandLayerMergeUp(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
            int rank = context.CurrentLayer;
            this.CurrentDocument.Modifier.LayerMerge(rank, rank + 1);
        }

        [Command("LayerMergeDown")]
        void CommandLayerMergeDown(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
            int rank = context.CurrentLayer;
            this.CurrentDocument.Modifier.LayerMerge(rank, rank - 1);
        }

        [Command("LayerUp")]
        void CommandLayerUp(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
            int rank = context.CurrentLayer;
            this.CurrentDocument.Modifier.LayerSwap(rank, rank + 1);
            context.CurrentLayer = rank + 1;
        }

        [Command("LayerDown")]
        void CommandLayerDown(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
            int rank = context.CurrentLayer;
            this.CurrentDocument.Modifier.LayerSwap(rank, rank - 1);
            context.CurrentLayer = rank - 1;
        }

        [Command("LayerDelete")]
        void CommandLayerDelete(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
            int rank = context.CurrentLayer;
            this.CurrentDocument.Modifier.LayerDelete(rank);
        }

        [Command("DebugBboxThin")]
        void CommandDebugBboxThin(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
            context.IsDrawBoxThin = !context.IsDrawBoxThin;
        }

        [Command("DebugBboxGeom")]
        void CommandDebugBboxGeom(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
            context.IsDrawBoxGeom = !context.IsDrawBoxGeom;
        }

        [Command("DebugBboxFull")]
        void CommandDebugBboxFull(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
            context.IsDrawBoxFull = !context.IsDrawBoxFull;
        }

        [Command("DebugDirty")]
        void CommandDebugDirty(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            this.CurrentDocument.Modifier.ActiveViewer.DirtyAllViews();
        }

        [Command("DebugCopyFonts")]
        void CommandCopyFonts(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            ClipboardWriteData data;
            data = new ClipboardWriteData();
            data.WriteText(Common.OpenType.FontCollection.Default.DebugGetFullFontList());
            Clipboard.SetData(data);
        }

        public void QuitApplication()
        {
            //	Quitte l'application.
            this.WritedGlobalSettings();
            this.DeleteTemporaryFiles();

            foreach (DocumentInfo di in this.documents)
            {
                di.Dispose();
            }

            this.documents.Clear();
            this.currentDocument = -1;

            Window.Quit();
        }

        protected VMenu CreatePagesMenu(Support.EventHandler<MessageEventArgs> message)
        {
            //	Construit le menu pour choisir une page.
            DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
            UndoableList pages = this.CurrentDocument.DocumentObjects; // liste des pages
            return Objects.Page.CreateMenu(pages, context.CurrentPage, "PageSelect", message);
        }

        protected VMenu CreateLayersMenu(Support.EventHandler<MessageEventArgs> message)
        {
            //	Construit le menu pour choisir un calque.
            DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
            Objects.Abstract page = context.RootObject(1);
            UndoableList layers = page.Objects; // liste des calques
            return Objects.Layer.CreateMenu(layers, context.CurrentLayer, "LayerSelect", message);
        }

        protected void InitCommands()
        {
            //	Initialise toutes les commandes.
            this.toolSelectState = this.CreateCommandState(
                "ToolSelect",
                "ToolSelect",
                "ToolSelect",
                KeyCode.AlphaS
            );
            this.toolGlobalState = this.CreateCommandState(
                "ToolGlobal",
                "ToolGlobal",
                "ToolGlobal",
                KeyCode.AlphaG
            );
            this.toolShaperState = this.CreateCommandState(
                "ToolShaper",
                "ToolShaper",
                "ToolShaper",
                KeyCode.AlphaA
            );
            this.toolEditState = this.CreateCommandState(
                "ToolEdit",
                "ToolEdit",
                "ToolEdit",
                KeyCode.AlphaE
            );
            this.toolZoomState = this.CreateCommandState(
                "ToolZoom",
                "ToolZoom",
                "ToolZoom",
                KeyCode.AlphaZ
            );
            this.toolHandState = this.CreateCommandState(
                "ToolHand",
                "ToolHand",
                "ToolHand",
                KeyCode.AlphaH
            );
            this.toolPickerState = this.CreateCommandState(
                "ToolPicker",
                "ToolPicker",
                "ToolPicker",
                KeyCode.AlphaI
            );
            this.toolHotSpotState = this.CreateCommandState(
                "ToolHotSpot",
                "ToolHotSpot",
                "ToolHotSpot"
            );
            this.toolLineState = this.CreateCommandState(
                "ObjectLine",
                "ObjectLine",
                "ToolLine",
                KeyCode.AlphaL
            );
            this.toolRectangleState = this.CreateCommandState(
                "ObjectRectangle",
                "ObjectRectangle",
                "ToolRectangle",
                KeyCode.AlphaR
            );
            this.toolCircleState = this.CreateCommandState(
                "ObjectCircle",
                "ObjectCircle",
                "ToolCircle",
                KeyCode.AlphaC
            );
            this.toolEllipseState = this.CreateCommandState(
                "ObjectEllipse",
                "ObjectEllipse",
                "ToolEllipse"
            );
            this.toolPolyState = this.CreateCommandState(
                "ObjectPoly",
                "ObjectPoly",
                "ToolPoly",
                KeyCode.AlphaP
            );
            this.toolFreeState = this.CreateCommandState(
                "ObjectFree",
                "ObjectFree",
                "ToolFree",
                KeyCode.AlphaM
            );
            this.toolBezierState = this.CreateCommandState(
                "ObjectBezier",
                "ObjectBezier",
                "ToolBezier",
                KeyCode.AlphaB
            );
            this.toolRegularState = this.CreateCommandState(
                "ObjectRegular",
                "ObjectRegular",
                "ToolRegular"
            );
            this.toolSurfaceState = this.CreateCommandState(
                "ObjectSurface",
                "ObjectSurface",
                "ToolSurface"
            );
            this.toolVolumeState = this.CreateCommandState(
                "ObjectVolume",
                "ObjectVolume",
                "ToolVolume"
            );
            this.toolTextLineState = this.CreateCommandState(
                "ObjectTextLine",
                "ObjectTextLine",
                "ToolTextLine"
            );
            this.toolTextLine2State = this.CreateCommandState(
                "ObjectTextLine2",
                "ObjectTextLine",
                "ToolTextLine"
            );
            this.toolTextBoxState = this.CreateCommandState(
                "ObjectTextBox",
                "ObjectTextBox",
                "ToolTextBox"
            );
            this.toolTextBox2State = this.CreateCommandState(
                "ObjectTextBox2",
                "ObjectTextBox",
                "ToolTextBox",
                KeyCode.AlphaT
            );
            this.toolArrayState = this.CreateCommandState(
                "ObjectArray",
                "ObjectArray",
                "ToolArray"
            );
            this.toolImageState = this.CreateCommandState(
                "ObjectImage",
                "ObjectImage",
                "ToolImage"
            );
            this.toolDimensionState = this.CreateCommandState(
                "ObjectDimension",
                "ObjectDimension",
                "ToolDimension"
            );

            this.newState = this.CreateCommandState(
                "New",
                "New",
                KeyCode.ModifierControl | KeyCode.AlphaN
            );
            this.openState = this.CreateCommandState(
                "Open",
                "Open",
                KeyCode.ModifierControl | KeyCode.AlphaO
            );
            this.openModelState = this.CreateCommandState(
                "OpenModel",
                "OpenModel",
                KeyCode.ModifierShift | KeyCode.ModifierControl | KeyCode.AlphaO
            );
            this.saveState = this.CreateCommandState(
                "Save",
                "Save",
                KeyCode.ModifierControl | KeyCode.AlphaS
            );
            this.saveAsState = this.CreateCommandState("SaveAs", "SaveAs");
            this.saveModelState = this.CreateCommandState("SaveModel", "SaveModel");
            this.closeState = this.CreateCommandState(
                "Close",
                null,
                "Close",
                KeyCode.ModifierControl | KeyCode.FuncF4
            );
            this.closeAllState = this.CreateCommandState("CloseAll", "CloseAll");
            this.forceSaveAllState = this.CreateCommandState("ForceSaveAll");
            this.overwriteAllState = this.CreateCommandState("OverwriteAll");
            this.nextDocState = this.CreateCommandState(
                "NextDocument",
                KeyCode.ModifierControl | KeyCode.FuncF6
            );
            this.prevDocState = this.CreateCommandState(
                "PrevDocument",
                KeyCode.ModifierControl | KeyCode.ModifierShift | KeyCode.FuncF6
            );
            this.printState = this.CreateCommandState(
                "Print",
                "Print",
                KeyCode.ModifierControl | KeyCode.AlphaP
            );
            this.exportState = this.CreateCommandState("Export", "Export");
            this.quickExportState = this.CreateCommandState("QuickExport", "QuickExport");
            this.glyphsState = this.CreateCommandState("Glyphs", "Glyphs", KeyCode.FuncF7);
            this.glyphsInsertState = this.CreateCommandState("GlyphsInsert");
            this.textEditingState = this.CreateCommandState("TextEditing");
            this.replaceState = this.CreateCommandState(
                "Replace",
                "Replace",
                KeyCode.ModifierControl | KeyCode.AlphaF
            );
            this.findNextState = this.CreateCommandState("FindNext", KeyCode.FuncF3);
            this.findPrevState = this.CreateCommandState(
                "FindPrev",
                KeyCode.ModifierShift | KeyCode.FuncF3
            );
            this.findDefNextState = this.CreateCommandState(
                "FindDefNext",
                KeyCode.ModifierControl | KeyCode.FuncF3
            );
            this.findDefPrevState = this.CreateCommandState(
                "FindDefPrev",
                KeyCode.ModifierControl | KeyCode.ModifierShift | KeyCode.FuncF3
            );
            this.deleteState = this.CreateCommandState("Delete", "Delete", KeyCode.Delete);
            this.duplicateState = this.CreateCommandState(
                "Duplicate",
                "Duplicate",
                KeyCode.ModifierControl | KeyCode.AlphaD
            );

            this.cutState = this.CreateCommandState(
                "Cut",
                "Cut",
                KeyCode.ModifierControl | KeyCode.AlphaX
            );
            this.copyState = this.CreateCommandState(
                "Copy",
                "Copy",
                KeyCode.ModifierControl | KeyCode.AlphaC
            );
            this.pasteState = this.CreateCommandState(
                "Paste",
                "Paste",
                KeyCode.ModifierControl | KeyCode.AlphaV
            );

            this.fontBoldState = this.CreateCommandState(
                Common.Document.Res.Commands.FontBold.CommandId,
                "FontBold",
                true,
                KeyCode.ModifierControl | KeyCode.AlphaB
            );
            this.fontItalicState = this.CreateCommandState(
                Common.Document.Res.Commands.FontItalic.CommandId,
                "FontItalic",
                true,
                KeyCode.ModifierControl | KeyCode.AlphaI
            );
            this.fontUnderlineState = this.CreateCommandState(
                Common.Document.Res.Commands.FontUnderline.CommandId,
                "FontUnderline",
                true,
                KeyCode.ModifierControl | KeyCode.AlphaU
            );
            this.fontOverlineState = this.CreateCommandState(
                Commands.FontOverline,
                "FontOverline",
                true
            );
            this.fontStrikeoutState = this.CreateCommandState(
                Commands.FontStrikeout,
                "FontStrikeout",
                true
            );
            this.fontSubscriptState = this.CreateCommandState(
                Commands.FontSubscript,
                "FontSubscript",
                true,
                KeyCode.ModifierControl | KeyCode.AlphaG
            );
            this.fontSuperscriptState = this.CreateCommandState(
                Commands.FontSuperscript,
                "FontSuperscript",
                true,
                KeyCode.ModifierControl | KeyCode.AlphaT
            );
            this.fontSizePlusState = this.CreateCommandState(Commands.FontSizePlus, "FontSizePlus");
            this.fontSizeMinusState = this.CreateCommandState(
                Commands.FontSizeMinus,
                "FontSizeMinus"
            );
            this.fontClearState = this.CreateCommandState(Commands.FontClear, "FontClear");
            this.paragraphLeadingState = this.CreateCommandState(
                "ParagraphLeading",
                "ParagraphLeading",
                true
            );
            this.paragraphLeadingPlusState = this.CreateCommandState(
                "ParagraphLeadingPlus",
                "ParagraphLeadingPlus"
            );
            this.paragraphLeadingMinusState = this.CreateCommandState(
                "ParagraphLeadingMinus",
                "ParagraphLeadingMinus"
            );
            this.paragraphIndentPlusState = this.CreateCommandState(
                "ParagraphIndentPlus",
                "ParagraphIndentPlus"
            );
            this.paragraphIndentMinusState = this.CreateCommandState(
                "ParagraphIndentMinus",
                "ParagraphIndentMinus"
            );
            this.paragraphJustifState = this.CreateCommandState(
                "ParagraphJustif",
                null,
                "ParagraphJustif",
                true
            );
            this.paragraphClearState = this.CreateCommandState("ParagraphClear", "ParagraphClear");

            this.orderUpOneState = this.CreateCommandState(
                "OrderUpOne",
                "OrderUpOne",
                KeyCode.ModifierControl | KeyCode.PageUp
            );
            this.orderDownOneState = this.CreateCommandState(
                "OrderDownOne",
                "OrderDownOne",
                KeyCode.ModifierControl | KeyCode.PageDown
            );
            this.orderUpAllState = this.CreateCommandState(
                "OrderUpAll",
                "OrderUpAll",
                KeyCode.ModifierShift | KeyCode.PageUp
            );
            this.orderDownAllState = this.CreateCommandState(
                "OrderDownAll",
                "OrderDownAll",
                KeyCode.ModifierShift | KeyCode.PageDown
            );

            this.moveLeftFreeState = this.CreateCommandState("MoveLeftFree", "MoveHi", "MoveLeft");
            this.moveRightFreeState = this.CreateCommandState(
                "MoveRightFree",
                "MoveH",
                "MoveRight"
            );
            this.moveUpFreeState = this.CreateCommandState("MoveUpFree", "MoveV", "MoveUp");
            this.moveDownFreeState = this.CreateCommandState("MoveDownFree", "MoveVi", "MoveDown");

            this.rotate90State = this.CreateCommandState("Rotate90", "Rotate90");
            this.rotate180State = this.CreateCommandState("Rotate180", "Rotate180");
            this.rotate270State = this.CreateCommandState("Rotate270", "Rotate270");
            this.rotateFreeCCWState = this.CreateCommandState("RotateFreeCCW", "RotateFreeCCW");
            this.rotateFreeCWState = this.CreateCommandState("RotateFreeCW", "RotateFreeCW");

            this.mirrorHState = this.CreateCommandState("MirrorH", "MirrorH");
            this.mirrorVState = this.CreateCommandState("MirrorV", "MirrorV");

            this.scaleMul2State = this.CreateCommandState("ScaleMul2", "ScaleMul2");
            this.scaleDiv2State = this.CreateCommandState("ScaleDiv2", "ScaleDiv2");
            this.scaleMulFreeState = this.CreateCommandState("ScaleMulFree", "ScaleMulFree");
            this.scaleDivFreeState = this.CreateCommandState("ScaleDivFree", "ScaleDivFree");

            this.alignLeftState = this.CreateCommandState("AlignLeft", "AlignLeft");
            this.alignCenterXState = this.CreateCommandState("AlignCenterX", "AlignCenterX");
            this.alignRightState = this.CreateCommandState("AlignRight", "AlignRight");
            this.alignTopState = this.CreateCommandState("AlignTop", "AlignTop");
            this.alignCenterYState = this.CreateCommandState("AlignCenterY", "AlignCenterY");
            this.alignBottomState = this.CreateCommandState("AlignBottom", "AlignBottom");
            this.alignGridState = this.CreateCommandState("AlignGrid", "AlignGrid");

            this.resetState = this.CreateCommandState("Reset", "Reset");

            this.shareLeftState = this.CreateCommandState("ShareLeft", "ShareLeft");
            this.shareCenterXState = this.CreateCommandState("ShareCenterX", "ShareCenterX");
            this.shareSpaceXState = this.CreateCommandState("ShareSpaceX", "ShareSpaceX");
            this.shareRightState = this.CreateCommandState("ShareRight", "ShareRight");
            this.shareTopState = this.CreateCommandState("ShareTop", "ShareTop");
            this.shareCenterYState = this.CreateCommandState("ShareCenterY", "ShareCenterY");
            this.shareSpaceYState = this.CreateCommandState("ShareSpaceY", "ShareSpaceY");
            this.shareBottomState = this.CreateCommandState("ShareBottom", "ShareBottom");

            this.adjustWidthState = this.CreateCommandState("AdjustWidth", "AdjustWidth");
            this.adjustHeightState = this.CreateCommandState("AdjustHeight", "AdjustHeight");

            this.colorToRgbState = this.CreateCommandState("ColorToRgb", "ColorToRgb");
            this.colorToCmykState = this.CreateCommandState("ColorToCmyk", "ColorToCmyk");
            this.colorToGrayState = this.CreateCommandState("ColorToGray", "ColorToGray");
            this.colorStrokeDarkState = this.CreateCommandState(
                "ColorStrokeDark",
                "ColorStrokeDark"
            );
            this.colorStrokeLightState = this.CreateCommandState(
                "ColorStrokeLight",
                "ColorStrokeLight"
            );
            this.colorFillDarkState = this.CreateCommandState("ColorFillDark", "ColorFillDark");
            this.colorFillLightState = this.CreateCommandState("ColorFillLight", "ColorFillLight");

            this.mergeState = this.CreateCommandState("Merge", "Merge");
            this.extractState = this.CreateCommandState("Extract", "Extract");
            this.groupState = this.CreateCommandState("Group", "Group");
            this.ungroupState = this.CreateCommandState("Ungroup", "Ungroup");
            this.insideState = this.CreateCommandState("Inside", "Inside");
            this.outsideState = this.CreateCommandState("Outside", "Outside");
            this.combineState = this.CreateCommandState("Combine", "Combine");
            this.uncombineState = this.CreateCommandState("Uncombine", "Uncombine");
            this.toBezierState = this.CreateCommandState("ToBezier", "ToBezier");
            this.toPolyState = this.CreateCommandState("ToPoly", "ToPoly");
            this.toTextBox2State = this.CreateCommandState(
                "ToTextBox2",
                "ObjectTextBox",
                "ToTextBox2"
            );
            this.fragmentState = this.CreateCommandState("Fragment", "Fragment");

            this.shaperHandleAddState = this.CreateCommandState(
                "ShaperHandleAdd",
                "ShaperHandleAdd"
            );
            this.shaperHandleSubState = this.CreateCommandState(
                "ShaperHandleSub",
                "ShaperHandleSub"
            );
            this.shaperHandleToLineState = this.CreateCommandState(
                "ShaperHandleToLine",
                "ShaperHandleToLine",
                true
            );
            this.shaperHandleToCurveState = this.CreateCommandState(
                "ShaperHandleToCurve",
                "ShaperHandleToCurve",
                true
            );
            this.shaperHandleSymState = this.CreateCommandState(
                "ShaperHandleSym",
                "ShaperHandleSym",
                true
            );
            this.shaperHandleSmoothState = this.CreateCommandState(
                "ShaperHandleSmooth",
                "ShaperHandleSmooth",
                true
            );
            this.shaperHandleDisState = this.CreateCommandState(
                "ShaperHandleDis",
                "ShaperHandleDis",
                true
            );
            this.shaperHandleInlineState = this.CreateCommandState(
                "ShaperHandleInline",
                "ShaperHandleInline",
                true
            );
            this.shaperHandleFreeState = this.CreateCommandState(
                "ShaperHandleFree",
                "ShaperHandleFree",
                true
            );
            this.shaperHandleSimplyState = this.CreateCommandState(
                "ShaperHandleSimply",
                "ShaperHandleSimply",
                true
            );
            this.shaperHandleCornerState = this.CreateCommandState(
                "ShaperHandleCorner",
                "ShaperHandleCorner",
                true
            );
            this.shaperHandleContinueState = this.CreateCommandState(
                "ShaperHandleContinue",
                "ShaperHandleContinue"
            );
            this.shaperHandleSharpState = this.CreateCommandState(
                "ShaperHandleSharp",
                "ShaperHandleSharp",
                true
            );
            this.shaperHandleRoundState = this.CreateCommandState(
                "ShaperHandleRound",
                "ShaperHandleRound",
                true
            );

            this.booleanAndState = this.CreateCommandState("BooleanAnd", "BooleanAnd");
            this.booleanOrState = this.CreateCommandState("BooleanOr", "BooleanOr");
            this.booleanXorState = this.CreateCommandState("BooleanXor", "BooleanXor");
            this.booleanFrontMinusState = this.CreateCommandState(
                "BooleanFrontMinus",
                "BooleanFrontMinus"
            );
            this.booleanBackMinusState = this.CreateCommandState(
                "BooleanBackMinus",
                "BooleanBackMinus"
            );

            this.undoState = this.CreateCommandState(
                "Undo",
                "Undo",
                KeyCode.ModifierControl | KeyCode.AlphaZ
            );
            this.redoState = this.CreateCommandState(
                "Redo",
                "Redo",
                KeyCode.ModifierControl | KeyCode.AlphaY
            );
            this.undoRedoListState = this.CreateCommandState("UndoRedoList");

            this.deselectAllState = this.CreateCommandState(
                Common.Widgets.Res.Commands.DeselectAll.CommandId,
                "DeselectAll",
                KeyCode.Escape
            );
            this.selectAllState = this.CreateCommandState(
                Common.Widgets.Res.Commands.SelectAll.CommandId,
                "SelectAll",
                KeyCode.ModifierControl | KeyCode.AlphaA
            );
            this.selectInvertState = this.CreateCommandState("SelectInvert", "SelectInvert");
            this.selectorAutoState = this.CreateCommandState("SelectorAuto", "SelectorAuto");
            this.selectorIndividualState = this.CreateCommandState(
                "SelectorIndividual",
                "SelectorIndividual"
            );
            this.selectorScalerState = this.CreateCommandState("SelectorScaler", "SelectorScaler");
            this.selectorStretchState = this.CreateCommandState(
                "SelectorStretch",
                "SelectorStretch"
            );
            this.selectorStretchTypeState = this.CreateCommandState("SelectorStretchType");
            this.selectTotalState = this.CreateCommandState("SelectTotal", "SelectTotal");
            this.selectPartialState = this.CreateCommandState("SelectPartial", "SelectPartial");
            this.selectorAdaptLine = this.CreateCommandState(
                "SelectorAdaptLine",
                "SelectorAdaptLine"
            );
            this.selectorAdaptText = this.CreateCommandState(
                "SelectorAdaptText",
                "SelectorAdaptText"
            );

            this.hideHalfState = this.CreateCommandState("HideHalf", "HideHalf", true);
            this.hideSelState = this.CreateCommandState("HideSel", "HideSel");
            this.hideRestState = this.CreateCommandState("HideRest", "HideRest");
            this.hideCancelState = this.CreateCommandState("HideCancel", "HideCancel");

            this.zoomMinState = this.CreateCommandState("ZoomMin", "ZoomMin", true);
            this.zoomPageState = this.CreateCommandState(
                "ZoomPage",
                "ZoomPage",
                true,
                KeyCode.ModifierControl | KeyCode.Digit0
            );
            this.zoomPageWidthState = this.CreateCommandState(
                "ZoomPageWidth",
                "ZoomPageWidth",
                true
            );
            this.zoomDefaultState = this.CreateCommandState("ZoomDefault", "ZoomDefault", true);
            this.zoomSelState = this.CreateCommandState("ZoomSel", "ZoomSel");
            this.zoomSelWidthState = this.CreateCommandState("ZoomSelWidth", "ZoomSelWidth");
            this.zoomPrevState = this.CreateCommandState("ZoomPrev", "ZoomPrev");
            this.zoomSubState = this.CreateCommandState(
                "ZoomSub",
                "ZoomSub",
                KeyCode.NumericSubtract
            );
            this.zoomAddState = this.CreateCommandState("ZoomAdd", "ZoomAdd", KeyCode.NumericAdd);

            this.previewState = this.CreateCommandState("Preview", "Preview", true);
            this.gridState = this.CreateCommandState("Grid", "Grid", true);
            this.textGridState = this.CreateCommandState("TextGrid", "TextGrid", true);
            this.textShowControlCharactersState = this.CreateCommandState(
                Commands.TextShowControlCharacters,
                "TextShowControlCharacters",
                true
            );
            this.textFontFilterState = this.CreateCommandState(
                "TextFontFilter",
                "TextFontFilter",
                true
            );
            this.textFontSampleAbcState = this.CreateCommandState(
                "TextFontSampleAbc",
                "TextFontSampleAbc",
                true
            );
            this.textInsertQuadState = this.CreateCommandState("TextInsertQuad", "TextInsertQuad");
            this.textInsertNewFrameEdges = this.CreateCommandState(
                "TextInsertNewFrame",
                "TextInsertNewFrame"
            );
            this.textInsertNewPageState = this.CreateCommandState(
                "TextInsertNewPage",
                "TextInsertNewPage"
            );
            this.constrainState = this.CreateCommandState("Constrain", "Constrain", true);
            this.magnetState = this.CreateCommandState("Magnet", "Magnet", true);
            this.magnetLayerState = this.CreateCommandState("MagnetLayer", "MagnetLayer", true);
            this.rulersState = this.CreateCommandState("Rulers", "Rulers", true);
            this.labelsState = this.CreateCommandState("Labels", "Labels", true);
            this.aggregatesState = this.CreateCommandState("Aggregates", "Aggregates", true);

            this.arrayOutlineFrameEdges = this.CreateCommandState("ArrayOutlineFrame");
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

#if false
			this.resDesignerBuildState = this.CreateCommandState("ResDesignerBuild");
			this.resDesignerTranslateState = this.CreateCommandState("ResDesignerTranslate");
#endif
            this.debugBboxThinState = this.CreateCommandState("DebugBboxThin");
            this.debugBboxGeomState = this.CreateCommandState("DebugBboxGeom");
            this.debugBboxFullState = this.CreateCommandState("DebugBboxFull");
            this.debugDirtyState = this.CreateCommandState("DebugDirty", KeyCode.FuncF12);
            this.debugCopyFontsState = this.CreateCommandState(
                "DebugCopyFonts",
                KeyCode.ModifierControl | KeyCode.ModifierShift | KeyCode.AlphaF
            );

            this.pagePrevState = this.CreateCommandState("PagePrev", KeyCode.PageUp);
            this.pageNextState = this.CreateCommandState("PageNext", KeyCode.PageDown);
            this.pageMenuState = this.CreateCommandState("PageMenu");
            this.pageMiniaturesState = this.CreateCommandState("PageMiniatures");
            this.pageNewState = this.CreateCommandState("PageNew", "PageNew");
            this.pageDuplicateState = this.CreateCommandState("PageDuplicate");
            this.pageUpState = this.CreateCommandState("PageUp");
            this.pageDownState = this.CreateCommandState("PageDown");
            this.pageDeleteState = this.CreateCommandState("PageDelete");

            this.layerPrevState = this.CreateCommandState("LayerPrev");
            this.layerNextState = this.CreateCommandState("LayerNext");
            this.layerMenuState = this.CreateCommandState("LayerMenu");
            this.layerMiniaturesState = this.CreateCommandState("LayerMiniatures");
            this.layerNewState = this.CreateCommandState("LayerNew", "LayerNew");
            this.layerDuplicateState = this.CreateCommandState("LayerDuplicate");
            this.layerNewSelState = this.CreateCommandState("LayerNewSel", "LayerNewSel");
            this.layerMergeUpState = this.CreateCommandState("LayerMergeUp", "LayerMergeUp");
            this.layerMergeDownState = this.CreateCommandState("LayerMergeDown", "LayerMergeDown");
            this.layerUpState = this.CreateCommandState("LayerUp");
            this.layerDownState = this.CreateCommandState("LayerDown");
            this.layerDeleteState = this.CreateCommandState("LayerDelete");

            this.settingsState = this.CreateCommandState("Settings", "Settings", KeyCode.FuncF5);
            this.infosState = this.CreateCommandState("Infos", "Infos");
            this.aboutState = this.CreateCommandState("AboutApplication", "About", "About");
            this.pageStackState = this.CreateCommandState("PageStack", "PageStack");
            this.updateState = this.CreateCommandState("UpdateApplication", "Update", "Update");
            this.keyState = this.CreateCommandState("KeyApplication", "Key", "Key");

            this.moveLeftNormState = this.CreateCommandState("MoveLeftNorm", KeyCode.ArrowLeft);
            this.moveRightNormState = this.CreateCommandState("MoveRightNorm", KeyCode.ArrowRight);
            this.moveUpNormState = this.CreateCommandState("MoveUpNorm", KeyCode.ArrowUp);
            this.moveDownNormState = this.CreateCommandState("MoveDownNorm", KeyCode.ArrowDown);
            this.moveLeftCtrlState = this.CreateCommandState(
                "MoveLeftCtrl",
                KeyCode.ModifierControl | KeyCode.ArrowLeft
            );
            this.moveRightCtrlState = this.CreateCommandState(
                "MoveRightCtrl",
                KeyCode.ModifierControl | KeyCode.ArrowRight
            );
            this.moveUpCtrlState = this.CreateCommandState(
                "MoveUpCtrl",
                KeyCode.ModifierControl | KeyCode.ArrowUp
            );
            this.moveDownCtrlState = this.CreateCommandState(
                "MoveDownCtrl",
                KeyCode.ModifierControl | KeyCode.ArrowDown
            );
            this.moveLeftShiftState = this.CreateCommandState(
                "MoveLeftShift",
                KeyCode.ModifierShift | KeyCode.ArrowLeft
            );
            this.moveRightShiftState = this.CreateCommandState(
                "MoveRightShift",
                KeyCode.ModifierShift | KeyCode.ArrowRight
            );
            this.moveUpShiftState = this.CreateCommandState(
                "MoveUpShift",
                KeyCode.ModifierShift | KeyCode.ArrowUp
            );
            this.moveDownShiftState = this.CreateCommandState(
                "MoveDownShift",
                KeyCode.ModifierShift | KeyCode.ArrowDown
            );
        }

        protected CommandState CreateCommandState(string commandName, params Shortcut[] shortcuts)
        {
            return this.CreateCommandState(commandName, null, commandName, shortcuts);
        }

        protected CommandState CreateCommandState(
            string commandName,
            bool statefull,
            params Shortcut[] shortcuts
        )
        {
            return this.CreateCommandState(commandName, null, commandName, statefull, shortcuts);
        }

        protected CommandState CreateCommandState(
            string commandName,
            string icon,
            bool statefull,
            params Shortcut[] shortcuts
        )
        {
            return this.CreateCommandState(commandName, icon, commandName, statefull, shortcuts);
        }

        protected CommandState CreateCommandState(
            string commandName,
            string icon,
            params Shortcut[] shortcuts
        )
        {
            return this.CreateCommandState(commandName, icon, commandName, false, shortcuts);
        }

        protected CommandState CreateCommandState(
            string commandName,
            string icon,
            string tooltip,
            params Shortcut[] shortcuts
        )
        {
            return this.CreateCommandState(commandName, icon, tooltip, false, shortcuts);
        }

        protected CommandState CreateCommandState(
            string commandName,
            string icon,
            string tooltip,
            bool statefull,
            params Shortcut[] shortcuts
        )
        {
            //	Crée un nouveau Command + CommandState.
            Command command = Epsitec.Common.Widgets.Command.Get(commandName);

            if (command.IsReadWrite)
            {
                if (shortcuts.Length > 0)
                {
                    command.Shortcuts.AddRange(shortcuts);
                }

                string description = DocumentEditor.GetRes("Action." + tooltip);

                command.ManuallyDefineCommand(description, Misc.Icon(icon), null, statefull);
            }

            return this.CommandContext.GetCommandState(command);
        }

        protected void ConnectEvents()
        {
            //	On s'enregistre auprès du document pour tous les événements.
            this.CurrentDocument.Notifier.DocumentChanged += this.HandleDocumentChanged;
            this.CurrentDocument.Notifier.MouseChanged += this.HandleMouseChanged;
            this.CurrentDocument.Notifier.ModifChanged += this.HandleModifChanged;
            this.CurrentDocument.Notifier.OriginChanged += this.HandleOriginChanged;
            this.CurrentDocument.Notifier.ZoomChanged += this.HandleZoomChanged;
            this.CurrentDocument.Notifier.ToolChanged += this.HandleToolChanged;
            this.CurrentDocument.Notifier.SaveChanged += this.HandleSaveChanged;
            this.CurrentDocument.Notifier.SelectionChanged += this.HandleSelectionChanged;
            this.CurrentDocument.Notifier.GeometryChanged += this.HandleGeometryChanged;
            this.CurrentDocument.Notifier.ShaperChanged += this.HandleShaperChanged;
            this.CurrentDocument.Notifier.TextChanged += this.HandleTextChanged;
            this.CurrentDocument.Notifier.StyleChanged += this.HandleStyleChanged;
            this.CurrentDocument.Notifier.PagesChanged += this.HandlePagesChanged;
            this.CurrentDocument.Notifier.LayersChanged += this.HandleLayersChanged;
            this.CurrentDocument.Notifier.PageChanged += this.HandlePageChanged;
            this.CurrentDocument.Notifier.LayerChanged += this.HandleLayerChanged;
            this.CurrentDocument.Notifier.UndoRedoChanged += this.HandleUndoRedoChanged;
            this.CurrentDocument.Notifier.GridChanged += this.HandleGridChanged;
            this.CurrentDocument.Notifier.LabelPropertiesChanged +=
                this.HandleLabelPropertiesChanged;
            this.CurrentDocument.Notifier.ConstrainChanged += this.HandleConstrainChanged;
            this.CurrentDocument.Notifier.MagnetChanged += this.HandleMagnetChanged;
            this.CurrentDocument.Notifier.PreviewChanged += this.HandlePreviewChanged;
            this.CurrentDocument.Notifier.SettingsChanged += this.HandleSettingsChanged;
            this.CurrentDocument.Notifier.FontsSettingsChanged += this.HandleFontsSettingsChanged;
            this.CurrentDocument.Notifier.GuidesChanged += this.HandleGuidesChanged;
            this.CurrentDocument.Notifier.HideHalfChanged += this.HandleHideHalfChanged;
            this.CurrentDocument.Notifier.DebugChanged += this.HandleDebugChanged;
            this.CurrentDocument.Notifier.PropertyChanged += this.HandlePropertyChanged;
            this.CurrentDocument.Notifier.AggregateChanged += this.HandleAggregateChanged;
            this.CurrentDocument.Notifier.TextStyleChanged += this.HandleTextStyleChanged;
            this.CurrentDocument.Notifier.TextStyleListChanged += this.HandleTextStyleListChanged;
            this.CurrentDocument.Notifier.SelNamesChanged += this.HandleSelNamesChanged;
            this.CurrentDocument.Notifier.DrawChanged += this.HandleDrawChanged;
            this.CurrentDocument.Notifier.RibbonCommand += this.HandleRibbonCommand;
            this.CurrentDocument.Notifier.BookPanelShowPage += this.HandleBookPanelShowPage;
            this.CurrentDocument.Notifier.SettingsShowPage += this.HandleSettingsShowPage;
        }

        private void HandleDocumentChanged()
        {
            //	Appelé par le document lorsque les informations sur le document ont changé.
            if (this.HasCurrentDocument)
            {
                this.printState.Enable = true;
                this.exportState.Enable = true;
                this.quickExportState.Enable = true;
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
                this.quickExportState.Enable = false;
                this.infosState.Enable = false;
                this.pageStackState.Enable = false;
            }
        }

        private void HandleMouseChanged()
        {
            //	Appelé par le document lorsque la position de la souris a changé.

            if (this.info.Items == null)
            {
                //	Quand on tue l'application par ALT-F4, il se peut que l'on reçoive encore
                //	des événements souris fantômes, alors que nous sommes déjà "morts".

                return;
            }

            StatusField field = this.info.Items["StatusMouse"] as StatusField;
            field.Text = this.TextInfoMouse;
            field.Invalidate();

            if (this.HasCurrentDocument)
            {
                DocumentInfo di = this.CurrentDocumentInfo;
                if (di.hRuler != null && di.hRuler.IsVisible)
                {
                    Point mouse;
                    if (this.CurrentDocument.Modifier.ActiveViewer.MousePos(out mouse))
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
            if (this.info.Items == null)
                return;

            StatusField field = this.info.Items["StatusModif"] as StatusField;
            field.Text = this.TextInfoModif;
            field.Invalidate();
        }

        private void HandleOriginChanged()
        {
            //	Appelé par le document lorsque l'origine a changé.
            this.UpdateScroller();

            if (this.HasCurrentDocument)
            {
                DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
                this.zoomPageState.Enable = true;
                this.zoomPageWidthState.Enable = true;
                this.zoomDefaultState.Enable = true;
                this.zoomPageState.ActiveState = context.IsZoomPage
                    ? Common.Widgets.ActiveState.Yes
                    : Common.Widgets.ActiveState.No;
                this.zoomPageWidthState.ActiveState = context.IsZoomPageWidth
                    ? Common.Widgets.ActiveState.Yes
                    : Common.Widgets.ActiveState.No;
                this.zoomDefaultState.ActiveState = context.IsZoomDefault
                    ? Common.Widgets.ActiveState.Yes
                    : Common.Widgets.ActiveState.No;
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

            if (this.HasCurrentDocument)
            {
                DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
                this.zoomMinState.Enable = true;
                this.zoomPageState.Enable = true;
                this.zoomPageWidthState.Enable = true;
                this.zoomDefaultState.Enable = true;
                this.zoomMinState.ActiveState =
                    (context.Zoom <= this.CurrentDocument.Modifier.ZoomMin)
                        ? Common.Widgets.ActiveState.Yes
                        : Common.Widgets.ActiveState.No;
                this.zoomPageState.ActiveState = context.IsZoomPage
                    ? Common.Widgets.ActiveState.Yes
                    : Common.Widgets.ActiveState.No;
                this.zoomPageWidthState.ActiveState = context.IsZoomPageWidth
                    ? Common.Widgets.ActiveState.Yes
                    : Common.Widgets.ActiveState.No;
                this.zoomDefaultState.ActiveState = context.IsZoomDefault
                    ? Common.Widgets.ActiveState.Yes
                    : Common.Widgets.ActiveState.No;
                this.zoomPrevState.Enable = (this.CurrentDocument.Modifier.ZoomMemorizeCount > 0);
                this.zoomSubState.Enable = (context.Zoom > this.CurrentDocument.Modifier.ZoomMin);
                this.zoomAddState.Enable = (context.Zoom < this.CurrentDocument.Modifier.ZoomMax);
            }
            else
            {
                this.zoomMinState.Enable = false;
                this.zoomPageState.Enable = false;
                this.zoomPageWidthState.Enable = false;
                this.zoomDefaultState.Enable = false;
                this.zoomMinState.ActiveState = Common.Widgets.ActiveState.No;
                this.zoomPageState.ActiveState = Common.Widgets.ActiveState.No;
                this.zoomPageWidthState.ActiveState = Common.Widgets.ActiveState.No;
                this.zoomDefaultState.ActiveState = Common.Widgets.ActiveState.No;
                this.zoomPrevState.Enable = false;
                this.zoomSubState.Enable = false;
                this.zoomAddState.Enable = false;
            }

            StatusField field = this.info.Items["StatusZoom"] as StatusField;
            field.Text = this.TextInfoZoom;
            field.Invalidate();

            AbstractSlider slider = this.info.Items["StatusZoomSlider"] as AbstractSlider;
            slider.Value = (decimal)this.ValueInfoZoom;
            slider.Enable = this.HasCurrentDocument;
        }

        protected void UpdateTool(
            CommandState cs,
            string currentTool,
            bool isCreating,
            bool enabled
        )
        {
            //	Met à jour une commande d'outil.
            Command command = cs.Command;
            string tool = command.CommandId;

            if (enabled)
            {
                cs.ActiveState = (tool == currentTool) ? ActiveState.Yes : ActiveState.No;
                ;
                cs.Enable = (
                    tool == currentTool
                    || tool == "ToolSelect"
                    || tool == "ToolShaper"
                    || !isCreating
                );
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

            if (this.HasCurrentDocument)
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
            this.UpdateTool(this.toolFreeState, tool, isCreating, enabled);
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
            if (this.HasCurrentDocument)
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
            if (this.HasCurrentDocument)
            {
                DocumentInfo di = this.CurrentDocumentInfo;

                di.containerPrincipal.SetDirtyContent();
                di.containerStyles.SetDirtyContent();

                if (di.containerAutos != null)
                {
                    di.containerAutos.SetDirtyContent();
                }

                Viewer viewer = this.CurrentDocument.Modifier.ActiveViewer;
                int totalSelected = this.CurrentDocument.Modifier.TotalSelected;
                int totalHide = this.CurrentDocument.Modifier.TotalHide;
                int totalPageHide = this.CurrentDocument.Modifier.TotalPageHide;
                int totalObjects = this.CurrentDocument.Modifier.TotalObjects;
                bool isCreating = this.CurrentDocument.Modifier.ActiveViewer.IsCreating;
                bool isBase = viewer.DrawingContext.RootStackIsBase;
                bool isEdit = this.CurrentDocument.Modifier.IsToolEdit;
                SelectorType sType = viewer.SelectorType;
                Objects.Abstract one = this.CurrentDocument.Modifier.RetOnlySelectedObject();

                bool isOldText = false;
                if (this.CurrentDocument.ContainOldText)
                {
                    isOldText = this.CurrentDocument.Modifier.IsSelectedOldText();
                }

                this.newState.Enable = true;
                this.openState.Enable = true;
                this.openModelState.Enable = true;
                this.deleteState.Enable = (totalSelected > 0 || isCreating);
                this.duplicateState.Enable = (totalSelected > 0 && !isCreating);
                this.orderUpOneState.Enable = (
                    totalObjects > 1 && totalSelected > 0 && !isCreating && !isEdit
                );
                this.orderDownOneState.Enable = (
                    totalObjects > 1 && totalSelected > 0 && !isCreating && !isEdit
                );
                this.orderUpAllState.Enable = (
                    totalObjects > 1 && totalSelected > 0 && !isCreating && !isEdit
                );
                this.orderDownAllState.Enable = (
                    totalObjects > 1 && totalSelected > 0 && !isCreating && !isEdit
                );
                this.moveLeftFreeState.Enable = (totalSelected > 0 && !isCreating && !isEdit);
                this.moveRightFreeState.Enable = (totalSelected > 0 && !isCreating && !isEdit);
                this.moveUpFreeState.Enable = (totalSelected > 0 && !isCreating && !isEdit);
                this.moveDownFreeState.Enable = (totalSelected > 0 && !isCreating && !isEdit);
                this.rotate90State.Enable = (totalSelected > 0 && !isCreating && !isEdit);
                this.rotate180State.Enable = (totalSelected > 0 && !isCreating && !isEdit);
                this.rotate270State.Enable = (totalSelected > 0 && !isCreating && !isEdit);
                this.rotateFreeCCWState.Enable = (totalSelected > 0 && !isCreating && !isEdit);
                this.rotateFreeCWState.Enable = (totalSelected > 0 && !isCreating && !isEdit);
                this.mirrorHState.Enable = (totalSelected > 0 && !isCreating && !isEdit);
                this.mirrorVState.Enable = (totalSelected > 0 && !isCreating && !isEdit);
                this.scaleMul2State.Enable = (totalSelected > 0 && !isCreating && !isEdit);
                this.scaleDiv2State.Enable = (totalSelected > 0 && !isCreating && !isEdit);
                this.scaleMulFreeState.Enable = (totalSelected > 0 && !isCreating && !isEdit);
                this.scaleDivFreeState.Enable = (totalSelected > 0 && !isCreating && !isEdit);
                this.alignLeftState.Enable = (totalSelected > 1 && !isCreating && !isEdit);
                this.alignCenterXState.Enable = (totalSelected > 1 && !isCreating && !isEdit);
                this.alignRightState.Enable = (totalSelected > 1 && !isCreating && !isEdit);
                this.alignTopState.Enable = (totalSelected > 1 && !isCreating && !isEdit);
                this.alignCenterYState.Enable = (totalSelected > 1 && !isCreating && !isEdit);
                this.alignBottomState.Enable = (totalSelected > 1 && !isCreating && !isEdit);
                this.alignGridState.Enable = (totalSelected > 0 && !isCreating && !isEdit);
                this.resetState.Enable = (totalSelected > 0 && !isCreating && !isEdit);
                this.shareLeftState.Enable = (totalSelected > 2 && !isCreating && !isEdit);
                this.shareCenterXState.Enable = (totalSelected > 2 && !isCreating && !isEdit);
                this.shareSpaceXState.Enable = (totalSelected > 2 && !isCreating && !isEdit);
                this.shareRightState.Enable = (totalSelected > 2 && !isCreating && !isEdit);
                this.shareTopState.Enable = (totalSelected > 2 && !isCreating && !isEdit);
                this.shareCenterYState.Enable = (totalSelected > 2 && !isCreating && !isEdit);
                this.shareSpaceYState.Enable = (totalSelected > 2 && !isCreating && !isEdit);
                this.shareBottomState.Enable = (totalSelected > 2 && !isCreating && !isEdit);
                this.adjustWidthState.Enable = (totalSelected > 1 && !isCreating && !isEdit);
                this.adjustHeightState.Enable = (totalSelected > 1 && !isCreating && !isEdit);
                this.colorToRgbState.Enable = (totalSelected > 0 && !isCreating && !isEdit);
                this.colorToCmykState.Enable = (totalSelected > 0 && !isCreating && !isEdit);
                this.colorToGrayState.Enable = (totalSelected > 0 && !isCreating && !isEdit);
                this.colorStrokeDarkState.Enable = (totalSelected > 0 && !isCreating && !isEdit);
                this.colorStrokeLightState.Enable = (totalSelected > 0 && !isCreating && !isEdit);
                this.colorFillDarkState.Enable = (totalSelected > 0 && !isCreating && !isEdit);
                this.colorFillLightState.Enable = (totalSelected > 0 && !isCreating && !isEdit);
                this.mergeState.Enable = (totalSelected > 1 && !isCreating && !isEdit);
                this.extractState.Enable = (totalSelected > 0 && !isBase && !isCreating && !isEdit);
                this.groupState.Enable = (totalSelected > 1 && !isCreating && !isEdit);
                this.ungroupState.Enable = (
                    totalSelected == 1 && one is Objects.Group && !isCreating && !isEdit
                );
                this.insideState.Enable = (
                    totalSelected == 1 && one is Objects.Group && !isCreating && !isEdit
                );
                this.outsideState.Enable = (!isBase && !isCreating);
                this.combineState.Enable = (totalSelected > 1 && !isCreating && !isEdit);
                this.uncombineState.Enable = (totalSelected > 0 && !isCreating && !isEdit);
                this.toBezierState.Enable = (totalSelected > 0 && !isCreating && !isEdit);
                this.toPolyState.Enable = (totalSelected > 0 && !isCreating && !isEdit);
                this.toTextBox2State.Enable = (
                    totalSelected > 0 && !isCreating && !isEdit && isOldText
                );
                this.fragmentState.Enable = (totalSelected > 0 && !isCreating && !isEdit);
                this.booleanAndState.Enable = (totalSelected > 1 && !isCreating && !isEdit);
                this.booleanOrState.Enable = (totalSelected > 1 && !isCreating && !isEdit);
                this.booleanXorState.Enable = (totalSelected > 1 && !isCreating && !isEdit);
                this.booleanFrontMinusState.Enable = (totalSelected > 1 && !isCreating && !isEdit);
                this.booleanBackMinusState.Enable = (totalSelected > 1 && !isCreating && !isEdit);
                this.layerNewSelState.Enable = (totalSelected > 0 && !isCreating);

                this.hideSelState.Enable = (totalSelected > 0 && !isCreating);
                this.hideRestState.Enable = (
                    totalSelected > 0 && totalObjects - totalSelected - totalHide > 0 && !isCreating
                );
                this.hideCancelState.Enable = (totalPageHide > 0 && !isCreating);

                this.zoomSelState.Enable = (totalSelected > 0);
                this.zoomSelWidthState.Enable = (totalSelected > 0);

                this.deselectAllState.Enable = (totalSelected > 0);
                this.selectAllState.Enable = (totalSelected < totalObjects - totalHide);
                this.selectInvertState.Enable = (
                    totalSelected > 0 && totalSelected < totalObjects - totalHide
                );

                this.selectorAutoState.Enable = true;
                this.selectorIndividualState.Enable = true;
                this.selectorScalerState.Enable = true;
                this.selectorStretchState.Enable = true;
                this.selectorStretchTypeState.Enable = true;
                this.selectorAutoState.ActiveState =
                    (sType == SelectorType.Auto) ? ActiveState.Yes : ActiveState.No;
                this.selectorIndividualState.ActiveState =
                    (sType == SelectorType.Individual) ? ActiveState.Yes : ActiveState.No;
                this.selectorScalerState.ActiveState =
                    (sType == SelectorType.Scaler) ? ActiveState.Yes : ActiveState.No;
                this.selectorStretchState.ActiveState =
                    (sType == SelectorType.Stretcher) ? ActiveState.Yes : ActiveState.No;
                di.containerPrincipal.UpdateSelectorStretch();

                this.selectTotalState.Enable = true;
                this.selectPartialState.Enable = true;
                this.selectTotalState.ActiveState = !viewer.PartialSelect
                    ? ActiveState.Yes
                    : ActiveState.No;
                this.selectPartialState.ActiveState = viewer.PartialSelect
                    ? ActiveState.Yes
                    : ActiveState.No;

                this.selectorAdaptLine.Enable = true;
                this.selectorAdaptText.Enable = true;
                this.selectorAdaptLine.ActiveState = viewer.SelectorAdaptLine
                    ? ActiveState.Yes
                    : ActiveState.No;
                this.selectorAdaptText.ActiveState = viewer.SelectorAdaptText
                    ? ActiveState.Yes
                    : ActiveState.No;

                if (!this.CurrentDocument.Wrappers.IsWrappersAttached) // pas édition en cours ?
                {
                    this.cutState.Enable = (totalSelected > 0 && !isCreating);
                    this.copyState.Enable = (totalSelected > 0 && !isCreating);
                    //?this.pasteState.Enable = (!this.CurrentDocument.Modifier.IsClipboardEmpty() && !isCreating);
                    this.pasteState.Enable = !isCreating;
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
                this.textInsertNewFrameEdges.Enable = false;
                this.textInsertNewPageState.Enable = false;
                this.fontBoldState.Enable = false;
                this.fontItalicState.Enable = false;
                this.fontUnderlineState.Enable = false;
                this.fontOverlineState.Enable = false;
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
                this.resetState.Enable = false;
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

                this.selectorAutoState.Enable = false;
                this.selectorIndividualState.Enable = false;
                this.selectorScalerState.Enable = false;
                this.selectorStretchState.Enable = false;
                this.selectorStretchTypeState.Enable = false;
                this.selectorAutoState.ActiveState = ActiveState.No;
                this.selectorIndividualState.ActiveState = ActiveState.No;
                this.selectorScalerState.ActiveState = ActiveState.No;
                this.selectorStretchState.ActiveState = ActiveState.No;

                this.selectTotalState.Enable = false;
                this.selectPartialState.Enable = false;
                this.selectTotalState.ActiveState = ActiveState.No;
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

        private void HandleGeometryChanged()
        {
            //	Appelé par le document lorsque la géométrie d'un objet a changé.
            if (this.HasCurrentDocument)
            {
                DocumentInfo di = this.CurrentDocumentInfo;

                di.containerPrincipal.SetDirtyContent();
            }
        }

        private void HandleShaperChanged()
        {
            //	Appelé par le document lorsque le modeleur a changé.
            if (
                this.HasCurrentDocument
                && this.CurrentDocument.Modifier.IsToolShaper
                && this.CurrentDocument.Modifier.TotalSelected != 0
            )
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
                this.shaperHandleSharpState.Enable = false;
                this.shaperHandleRoundState.Enable = false;
            }
        }

        private void HandleTextChanged()
        {
            //	Appelé par le document lorsque le texte en édition a changé.
            this.RibbonsNotifyTextStylesChanged();

            this.dlgGlyphs.SetAlternatesDirty();

            if (this.HasCurrentDocument)
            {
                DocumentInfo di = this.CurrentDocumentInfo;
                di.document.SetDirtySerialize(CacheBitmapChanging.Local); // (*REM1*)

                // (*REM1*)	Il faut appeler document.SetDirtySerialize pour mettre à jour les miniatures
                //			(de façon asynchrone) pendant la frappe du texte.
            }
        }

        private void HandleStyleChanged()
        {
            //	Appelé par le document lorsqu'un style a changé.
            if (!this.HasCurrentDocument)
                return;
            DocumentInfo di = this.CurrentDocumentInfo;
            di.containerStyles.SetDirtyContent();
        }

        private void HandlePagesChanged()
        {
            //	Appelé par le document lorsque les pages ont changé.
            if (this.HasCurrentDocument)
            {
                DocumentInfo di = this.CurrentDocumentInfo;
                di.containerPages.SetDirtyContent();

                DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
                int cp = context.CurrentPage;
                int tp = context.TotalPages();

                bool isCreating = this.CurrentDocument.Modifier.ActiveViewer.IsCreating;

                this.pagePrevState.Enable = (cp > 0 && !isCreating);
                this.pageNextState.Enable = (cp < tp - 1 && !isCreating);
                this.pageMenuState.Enable = (tp > 1 && !isCreating);
                this.pageNewState.Enable = !isCreating;
                this.pageDuplicateState.Enable = !isCreating;
                this.pageUpState.Enable = (cp > 0 && !isCreating);
                this.pageDownState.Enable = (cp < tp - 1 && !isCreating);
                this.pageDeleteState.Enable = (tp > 1 && !isCreating);

                Objects.Page page = this.CurrentDocument.DocumentObjects[cp] as Objects.Page;
                this.CurrentDocumentInfo.quickPageMenu.Text = page.ShortName;
                this.CurrentDocument.ImageLockInPage(cp);

                if (di.pageMiniatures != null)
                {
                    di.pageMiniatures.UpdatePageAfterChanging();
                }

                if (di.layerMiniatures != null)
                {
                    di.layerMiniatures.UpdateLayerAfterChanging();
                }

                this.dlgPageStack.Update();
                this.dlgPrint.UpdatePages();
                this.dlgExportPDF.UpdatePages();
                this.dlgExportICO.UpdatePages();
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
            if (this.HasCurrentDocument)
            {
                DocumentInfo di = this.CurrentDocumentInfo;
                di.containerLayers.SetDirtyContent();

                DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
                int cl = context.CurrentLayer;
                int tl = context.TotalLayers();

                bool isCreating = this.CurrentDocument.Modifier.ActiveViewer.IsCreating;

                this.layerPrevState.Enable = (cl > 0 && !isCreating);
                this.layerNextState.Enable = (cl < tl - 1 && !isCreating);
                this.layerMenuState.Enable = (tl > 1 && !isCreating);
                this.layerNewState.Enable = !isCreating;
                this.layerDuplicateState.Enable = !isCreating;
                this.layerMergeUpState.Enable = (cl < tl - 1 && !isCreating);
                this.layerMergeDownState.Enable = (cl > 0 && !isCreating);
                this.layerUpState.Enable = (cl < tl - 1 && !isCreating);
                this.layerDownState.Enable = (cl > 0 && !isCreating);
                this.layerDeleteState.Enable = (tl > 1 && !isCreating);

                bool ml = this.CurrentDocument.Modifier.MagnetLayerState(cl);
                this.magnetLayerState.Enable = true;
                this.magnetLayerState.ActiveState = ml ? ActiveState.Yes : ActiveState.No;

                if (di.layerMiniatures != null)
                {
                    di.layerMiniatures.UpdateLayerAfterChanging();
                }

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
            if (!this.HasCurrentDocument)
                return;
            DocumentInfo di = this.CurrentDocumentInfo;
            di.containerPages.SetDirtyObject(page);
            this.HandleModifChanged();
        }

        private void HandleLayerChanged(Objects.Abstract layer)
        {
            //	Appelé par le document lorsqu'un nom de calque a changé.
            if (!this.HasCurrentDocument)
                return;
            DocumentInfo di = this.CurrentDocumentInfo;
            di.containerLayers.SetDirtyObject(layer);
            this.HandleModifChanged();
        }

        private void HandleUndoRedoChanged()
        {
            //	Appelé par le document lorsque l'état des commande undo/redo a changé.
            if (this.HasCurrentDocument)
            {
                bool isCreating = this.CurrentDocument.Modifier.ActiveViewer.IsCreating;
                this.undoState.Enable = (
                    this.CurrentDocument.Modifier.OpletQueue.CanUndo && !isCreating
                );
                this.redoState.Enable = (
                    this.CurrentDocument.Modifier.OpletQueue.CanRedo && !isCreating
                );
                this.undoRedoListState.Enable = this.undoState.Enable | this.redoState.Enable;
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
            if (this.HasCurrentDocument)
            {
                DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;

                this.gridState.Enable = true;
                this.gridState.ActiveState = context.GridActive ? ActiveState.Yes : ActiveState.No;

                this.textGridState.Enable = true;
                this.textGridState.ActiveState = context.TextGridShow
                    ? ActiveState.Yes
                    : ActiveState.No;

                this.rulersState.Enable = true;
                this.rulersState.ActiveState = context.RulersShow
                    ? ActiveState.Yes
                    : ActiveState.No;

                this.labelsState.Enable = true;
                this.labelsState.ActiveState = context.LabelsShow
                    ? ActiveState.Yes
                    : ActiveState.No;

                this.aggregatesState.Enable = true;
                this.aggregatesState.ActiveState = context.AggregatesShow
                    ? ActiveState.Yes
                    : ActiveState.No;

                this.textShowControlCharactersState.Enable = this.CurrentDocument
                    .Wrappers
                    .IsWrappersAttached;
                this.textShowControlCharactersState.ActiveState = context.TextShowControlCharacters
                    ? ActiveState.Yes
                    : ActiveState.No;
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
            if (this.HasCurrentDocument)
            {
                DocumentInfo di = this.CurrentDocumentInfo;
                di.containerPrincipal.SetDirtyContent();
                di.containerStyles.SetDirtyContent();
                di.containerPages.SetDirtyContent();
                di.containerLayers.SetDirtyContent();
            }
        }

        private void HandleConstrainChanged()
        {
            //	Appelé par le document lorsque l'état des constructions a changé.
            if (this.HasCurrentDocument)
            {
                DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
                this.constrainState.Enable = true;
                this.constrainState.ActiveState = context.ConstrainActive
                    ? ActiveState.Yes
                    : ActiveState.No;
            }
            else
            {
                this.constrainState.Enable = false;
                this.constrainState.ActiveState = ActiveState.No;
            }
        }

        private void HandleMagnetChanged()
        {
            //	Appelé par le document lorsque l'état des lignes magnétiques a changé.
            if (this.HasCurrentDocument)
            {
                DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
                IList<Objects.Layer> layers = context.MagnetLayerList;
                if (layers.Count == 0)
                {
                    this.magnetState.Enable = false;
                    this.magnetState.ActiveState = ActiveState.No;
                }
                else
                {
                    this.magnetState.Enable = true;
                    this.magnetState.ActiveState = context.MagnetActive
                        ? ActiveState.Yes
                        : ActiveState.No;
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
            if (this.HasCurrentDocument)
            {
                DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
                this.previewState.Enable = true;
                this.previewState.ActiveState = context.PreviewActive
                    ? ActiveState.Yes
                    : ActiveState.No;
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
            if (this.HasCurrentDocument)
            {
                this.CurrentDocument.Dialogs.UpdateAllSettings();
            }
        }

        private void HandleFontsSettingsChanged()
        {
            //	Appelé par le document lorsque les réglages de police ont changés.
            if (this.HasCurrentDocument)
            {
                DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;

                this.textFontFilterState.Enable = true;
                this.textFontFilterState.ActiveState = context.TextFontFilter
                    ? ActiveState.Yes
                    : ActiveState.No;

                this.textFontSampleAbcState.Enable = true;
                this.textFontSampleAbcState.ActiveState = context.TextFontSampleAbc
                    ? ActiveState.Yes
                    : ActiveState.No;

                this.CurrentDocument.Dialogs.UpdateFonts();
            }
            else
            {
                this.textFontFilterState.Enable = false;
                this.textFontFilterState.ActiveState = ActiveState.No;

                this.textFontSampleAbcState.Enable = false;
                this.textFontSampleAbcState.ActiveState = ActiveState.No;
            }

            this.RibbonsNotifyChanged("FontsSettingsChanged");
        }

        private void HandleGuidesChanged()
        {
            //	Appelé par le document lorsque les repères ont changé.
            if (this.HasCurrentDocument)
            {
                this.CurrentDocument.Dialogs.UpdateGuides();
            }
        }

        private void HandleHideHalfChanged()
        {
            //	Appelé par le document lorsque l'état de la commande "hide half" a changé.
            if (this.HasCurrentDocument)
            {
                DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
                this.hideHalfState.Enable = true;
                this.hideHalfState.ActiveState = context.HideHalfActive
                    ? ActiveState.Yes
                    : ActiveState.No;
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
            if (this.HasCurrentDocument)
            {
                DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
                this.debugBboxThinState.Enable = true;
                this.debugBboxGeomState.Enable = true;
                this.debugBboxFullState.Enable = true;
                this.debugBboxThinState.ActiveState = context.IsDrawBoxThin
                    ? ActiveState.Yes
                    : ActiveState.No;
                this.debugBboxGeomState.ActiveState = context.IsDrawBoxGeom
                    ? ActiveState.Yes
                    : ActiveState.No;
                this.debugBboxFullState.ActiveState = context.IsDrawBoxFull
                    ? ActiveState.Yes
                    : ActiveState.No;
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
            if (this.HasCurrentDocument)
            {
                DocumentInfo di = this.CurrentDocumentInfo;
                di.containerPrincipal.SetDirtyProperties(propertyList);
                di.containerStyles.SetDirtyProperties(propertyList);
            }
        }

        private void HandleAggregateChanged(System.Collections.ArrayList aggregateList)
        {
            //	Appelé lorsqu'un agrégat a changé.
            if (this.HasCurrentDocument)
            {
                DocumentInfo di = this.CurrentDocumentInfo;
                di.containerPrincipal.SetDirtyAggregates(aggregateList);
                di.containerStyles.SetDirtyAggregates(aggregateList);
            }
        }

        private void HandleTextStyleChanged(System.Collections.ArrayList textStyleList)
        {
            //	Appelé lorsqu'un agrégat a changé.
            if (this.HasCurrentDocument)
            {
                DocumentInfo di = this.CurrentDocumentInfo;
                di.containerStyles.SetDirtyTextStyles(textStyleList);
            }

            this.RibbonsNotifyTextStylesChanged(textStyleList);
        }

        private void HandleTextStyleListChanged()
        {
            //	Appelé lorsqu'un style de texte a été ajouté ou supprimé.
            if (this.HasCurrentDocument)
            {
                DocumentInfo di = this.CurrentDocumentInfo;
                di.containerStyles.SetDirtyContent();
            }

            this.RibbonsNotifyChanged("TextStyleListChanged");
        }

        private void HandleSelNamesChanged()
        {
            //	Appelé lorsque la sélection par noms a changé.
            if (this.HasCurrentDocument)
            {
                DocumentInfo di = this.CurrentDocumentInfo;
                di.containerPrincipal.SetDirtySelNames();
            }
        }

        private void HandleDrawChanged(Viewer viewer, Drawing.Rectangle rect)
        {
            //	Appelé par le document lorsque le dessin a changé.
            Drawing.Rectangle box = rect;

            if (viewer.DrawingContext.IsActive)
            {
                box.Inflate(viewer.DrawingContext.HandleRedrawSize / 2);
            }

            box = viewer.InternalToClient(box);
            this.InvalidateDraw(viewer, box);
        }

        private void HandleRibbonCommand(string name)
        {
            //	Appelé par le document lorsqu'il faut changer de ruban.
            RibbonPage ribbon = this.GetRibbon(name);

            if (name.Length > 0 && name[0] == '!')
            {
                ribbon = this.LastRibbon(name.Substring(1));
            }

            this.SetActiveRibbon(ribbon);
        }

        private void HandleBookPanelShowPage(string page, string sub)
        {
            //	Appelé par le document lorsqu'il faut afficher un onglet spécifique.
            this.dlgSplash.Hide();

            DocumentInfo di = this.CurrentDocumentInfo;
            if (di == null)
                return;

            foreach (TabPage tab in di.bookPanels.Items)
            {
                if (tab == null)
                    continue;
                if (tab.Name == page)
                {
                    di.bookPanels.ActivePage = tab;

                    if (page == "Styles")
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

            if (this.settingsState.ActiveState == ActiveState.No)
            {
                this.dlgSettings.Show();
                this.settingsState.ActiveState = ActiveState.Yes;
            }

            this.dlgSettings.ShowPage(book, tab);
        }

        protected void InvalidateDraw(Viewer viewer, Drawing.Rectangle bbox)
        {
            //	Invalide une partie de la zone de dessin d'un visualisateur.
            if (bbox.IsEmpty)
                return;

            if (bbox.IsInfinite)
            {
                viewer.SyncPaint = true;
                viewer.Invalidate();
                viewer.SyncPaint = false;
            }
            else
            {
                viewer.SyncPaint = true;
                viewer.Invalidate(bbox);
                viewer.SyncPaint = false;
            }
        }

        protected void UpdateScroller()
        {
            //	Met à jour les ascenseurs.
            if (!this.HasCurrentDocument)
                return;

            DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
            Size area = this.CurrentDocument.Modifier.SizeArea;
            Point scale = context.Scale;
            Size cs = context.ContainerSize;
            if (cs.Width < 0.0 || cs.Height < 0.0)
                return;
            double ratioH = cs.Width / scale.X / area.Width;
            double ratioV = cs.Height / scale.Y / area.Height;
            ratioH = System.Math.Min(ratioH, 1.0);
            ratioV = System.Math.Min(ratioV, 1.0);

            DocumentInfo di = this.CurrentDocumentInfo;
            double min,
                max;

            min = context.MinOriginX;
            max = context.MaxOriginX;
            max = System.Math.Max(min, max);
            di.hScroller.MinValue = (decimal)min;
            di.hScroller.MaxValue = (decimal)max;
            di.hScroller.VisibleRangeRatio = (decimal)ratioH;
            di.hScroller.Value = (decimal)(-context.OriginX);
            di.hScroller.SmallChange = (decimal)((cs.Width * 0.1) / scale.X);
            di.hScroller.LargeChange = (decimal)((cs.Width * 0.9) / scale.X);

            min = context.MinOriginY;
            max = context.MaxOriginY;
            max = System.Math.Max(min, max);
            di.vScroller.MinValue = (decimal)min;
            di.vScroller.MaxValue = (decimal)max;
            di.vScroller.VisibleRangeRatio = (decimal)ratioV;
            di.vScroller.Value = (decimal)(-context.OriginY);
            di.vScroller.SmallChange = (decimal)((cs.Height * 0.1) / scale.Y);
            di.vScroller.LargeChange = (decimal)((cs.Height * 0.9) / scale.Y);

            if (di.hRuler != null && di.hRuler.IsVisible)
            {
                di.hRuler.PPM = this.CurrentDocument.Modifier.RealScale;
                di.vRuler.PPM = this.CurrentDocument.Modifier.RealScale;

                Rectangle rect = this.CurrentDocument.Modifier.ActiveViewer.RectangleDisplayed;

                di.hRuler.Starting = rect.Left;
                di.hRuler.Ending = rect.Right;

                di.vRuler.Starting = rect.Bottom;
                di.vRuler.Ending = rect.Top;
            }
        }

        protected void UpdateRulers()
        {
            //	Met à jour les règles, après les avoir montrées ou cachées.
            if (!this.HasCurrentDocument)
                return;

            Viewer viewer = this.CurrentDocument.Modifier.ActiveViewer;
            DrawingContext context = this.CurrentDocument.Modifier.ActiveViewer.DrawingContext;
            DocumentInfo di = this.CurrentDocumentInfo;
            if (di.hRuler == null)
                return;

            di.hRuler.Visibility = (context.RulersShow);
            di.vRuler.Visibility = (context.RulersShow);

            double sw = 17; // largeur d'un ascenseur
            double sr = 13; // largeur d'une règle
            double wm = 4; // marges autour du viewer
            double lm = 0;
            double tm = 0;
            if (context.RulersShow)
            {
                lm = sr - 1;
                tm = sr - 1;
            }
            viewer.Margins = new Margins(wm + lm, wm + sw + 1, 6 + tm, wm + sw + 1);
        }

        protected void UpdateAfterRead()
        {
            //	Effectue une mise à jour après avoir ouvert un fichier.
            if (this.HasCurrentDocument)
            {
                //	Il faudra refaire la liste des polices connues, ce qui est
                //	nécessaire si le document ouvert contenait des polices non
                //	installées.
                Misc.ClearFontList();
                this.CurrentDocument.Dialogs.UpdateFontsAdded();
            }
        }

        protected string TextInfoObject
        {
            //	Texte pour les informations.
            get
            {
                Document doc = this.CurrentDocument;
                if (doc == null)
                {
                    return " ";
                }
                else
                {
                    int sel = doc.Modifier.TotalSelected;
                    int hide = doc.Modifier.TotalHide;
                    int total = doc.Modifier.TotalObjects;
                    int deep = doc.Modifier.ActiveViewer.DrawingContext.RootStackDeep;

                    string sDeep = Res.Strings.Status.Objects.Select;
                    if (deep > 2)
                    {
                        sDeep = string.Format(Res.Strings.Status.Objects.Level, deep - 2);
                    }

                    string sHide = "";
                    if (hide > 0)
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
                if (doc == null)
                {
                    return " ";
                }
                else
                {
                    Point mouse;
                    if (doc.Modifier.ActiveViewer.MousePos(out mouse))
                    {
                        doc.Modifier.ActiveViewer.DrawingContext.SnapGrid(ref mouse);
                        return string.Format(
                            "x:{0} y:{1}",
                            doc.Modifier.RealToString(mouse.X),
                            doc.Modifier.RealToString(mouse.Y)
                        );
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
                if (doc == null)
                {
                    return " ";
                }
                else
                {
                    if (doc.Modifier.TextInfoModif == "")
                    {
                        DrawingContext context = this.CurrentDocument
                            .Modifier
                            .ActiveViewer
                            .DrawingContext;
                        int cp = context.CurrentPage;
                        int cl = context.CurrentLayer;
                        Objects.Page page =
                            this.CurrentDocument.DocumentObjects[cp] as Objects.Page;
                        Objects.Layer layer = page.Objects[cl] as Objects.Layer;

                        string sp = page.InfoName;

                        string sl = layer.Name;
                        if (sl == "")
                            sl = Objects.Layer.ShortName(cl);

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
                if (doc == null)
                {
                    return " ";
                }
                else
                {
                    DrawingContext context = doc.Modifier.ActiveViewer.DrawingContext;
                    double zoom = context.Zoom;
                    return string.Format(
                        Res.Strings.Status.Zoom.Value,
                        (zoom * 100).ToString("F0")
                    );
                }
            }
        }

        protected double ValueInfoZoom
        {
            //	Valeur pour les informations.
            get
            {
                Document doc = this.CurrentDocument;
                if (doc == null)
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
            if (this.Window == null)
                return;

            if (this.MouseCursor != MouseCursor.AsWait)
            {
                this.lastMouseCursor = this.MouseCursor;
            }

            this.MouseCursor = MouseCursor.AsWait;
            this.Window.MouseCursor = this.MouseCursor;
        }

        protected void MouseHideWait()
        {
            //	Enlève le sablier.
            if (this.Window == null)
                return;
            this.MouseCursor = this.lastMouseCursor;
            this.Window.MouseCursor = this.MouseCursor;
        }

        #region TabBook
        private void HandleBookDocumentsActivePageChanged(object sender, CancelEventArgs e)
        {
            //	L'onglet pour le document courant a été cliqué.
            if (this.ignoreChange)
                return;

            int total = this.bookDocuments.PageCount;
            for (int i = 0; i < total; i++)
            {
                DocumentInfo di = this.documents[i];
                if (di.tabPage == this.bookDocuments.ActivePage)
                {
                    this.UseDocument(i);
                    return;
                }
            }
        }

        public bool HasCurrentDocument
        {
            //	Indique s'il existe un document courant.
            get { return (this.currentDocument >= 0); }
        }

        protected DocumentInfo CurrentDocumentInfo
        {
            //	Retourne le DocumentInfo courant.
            get
            {
                if (this.currentDocument < 0)
                    return null;
                return this.documents[this.currentDocument];
            }
        }

        public Document CurrentDocument
        {
            //	Retourne le Document courant.
            get
            {
                if (this.currentDocument < 0)
                    return null;
                return this.CurrentDocumentInfo.document;
            }
        }

        protected bool IsRecyclableDocument()
        {
            //	Indique si le document en cours est un document vide "sans titre"
            //	pouvant servir de conteneur pour ouvrir un nouveau document.
            if (!this.HasCurrentDocument)
                return false;
            if (this.CurrentDocument.IsDirtySerialize)
                return false;
            if (this.CurrentDocument.Modifier.StatisticTotalObjects() != 0)
                return false;
            return true;
        }

        protected void CreateDocument(Window window)
        {
            //	Crée un nouveau document.
            this.PrepareCloseDocument();

            Document doc = new Document(
                this.documentType,
                DocumentMode.Modify,
                this.installType,
                this.debugMode,
                this.globalSettings,
                this.CommandDispatcher,
                this.CommandContext,
                window
            );
            doc.Name = "Document";
            doc.Clipboard = this.clipboard;

            DocumentInfo di = new DocumentInfo();
            di.document = doc;
            this.documents.Insert(++this.currentDocument, di);

            this.CreateDocumentLayout(this.CurrentDocument);
            this.ConnectEvents();
            this.CurrentDocument.Modifier.New();
            this.bookDocuments.ActivePage = di.tabPage; // (*)
            this.UpdateCloseCommand();
            this.PrepareOpenDocument();

            // (*)	Le Modifier.New doit avoir été fait, car certains panneaux accèdent aux dimensions
            //		de la page. Pour cela, DrawingContext doit avoir un rootStack initialisé, ce qui
            //		est fait par Modifier.New.
        }

        protected void UseDocument(int rank)
        {
            //	Utilise un document ouvert.
            if (this.ignoreChange)
                return;

            this.PrepareCloseDocument();
            this.currentDocument = rank;
            this.PrepareOpenDocument();

            GlobalImageCache.UnlockAll(); // libère toutes les images

            if (rank >= 0)
            {
                this.ignoreChange = true;
                this.bookDocuments.ActivePage = this.CurrentDocumentInfo.tabPage;
                this.ignoreChange = false;

                DocumentInfo di;
                int total = this.bookDocuments.PageCount;
                for (int i = 0; i < total; i++)
                {
                    di = this.documents[i];
                    di.bookPanels.Visibility = (i == this.currentDocument);
                }

                di = this.CurrentDocumentInfo;
                this.CurrentDocument.HRuler = di.hRuler;
                this.CurrentDocument.VRuler = di.vRuler;

                this.RibbonsSetDocument(this.globalSettings, this.CurrentDocument);

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
                this.RibbonsSetDocument(this.globalSettings, null);

                this.HandleDocumentChanged();
                this.HandleMouseChanged();
                this.HandleOriginChanged();
                this.HandleZoomChanged();
                this.HandleToolChanged();
                this.HandleSaveChanged();
                this.HandleSelectionChanged();
                this.HandleGeometryChanged();
                this.HandleStyleChanged();
                this.HandlePagesChanged();
                this.HandleLayersChanged();
                this.HandleUndoRedoChanged();
                this.HandleGridChanged();
                this.HandleLabelPropertiesChanged();
                this.HandleConstrainChanged();
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
            if (rank < 0)
                return;

            DocumentInfo di = this.CurrentDocumentInfo;
            this.currentDocument = -1;
            this.documents.RemoveAt(rank);
            this.ignoreChange = true;
            this.bookDocuments.Items.RemoveAt(rank);
            this.ignoreChange = false;
            di.Dispose();

            if (rank >= this.bookDocuments.PageCount)
            {
                rank = this.bookDocuments.PageCount - 1;
            }
            this.UseDocument(rank);
            this.UpdateCloseCommand();

            if (this.CurrentDocument == null)
            {
                this.SetActiveRibbon(this.ribbonMain);
            }
        }

        protected void UpdateCloseCommand()
        {
            //	Met à jour l'état de la commande de fermeture.
            DocumentInfo di = this.CurrentDocumentInfo;
            if (di != null)
            {
                this.bookDocuments.ActivePage = di.tabPage;
            }

            this.closeState.Enable = (this.bookDocuments.PageCount > 0);
            this.closeAllState.Enable = (this.bookDocuments.PageCount > 0);
            this.forceSaveAllState.Enable = (this.bookDocuments.PageCount > 0);
            this.overwriteAllState.Enable = (this.bookDocuments.PageCount > 0);
            this.nextDocState.Enable = (this.bookDocuments.PageCount > 1);
            this.prevDocState.Enable = (this.bookDocuments.PageCount > 1);

            if (di != null)
            {
                this.bookDocuments.UpdateAfterChanges();
            }
        }

        protected void UpdateBookDocuments()
        {
            //	Met à jour le nom de l'onglet des documents.
            if (!this.HasCurrentDocument)
                return;
            TabPage tab = this.bookDocuments.Items[this.currentDocument] as TabPage;
            tab.TabTitle = Misc.ExtractName(
                this.CurrentDocument.Filename,
                this.CurrentDocument.IsDirtySerialize
            );
            this.bookDocuments.UpdateAfterChanges();
        }

        protected void PrepareCloseDocument()
        {
            //	Préparation avant la fermeture d'un document.
            if (!this.HasCurrentDocument)
                return;
            this.CurrentDocument.Dialogs.FlushAll();
            this.CurrentDocument.Close();
        }

        protected void PrepareOpenDocument()
        {
            //	Préparation après l'ouverture d'un document.
            this.dlgExport.Rebuild();
            this.dlgExportPDF.Rebuild();
            this.dlgExportICO.Rebuild();
            this.dlgGlyphs.Rebuild();
            this.dlgInfos.Rebuild();
            this.dlgPrint.Rebuild();
            this.dlgReplace.Rebuild();
            this.dlgSettings.Rebuild();
        }

        protected void CommandStateShake(CommandState state)
        {
            //	Secoue un Command pour le forcer à se remettre à jour.
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
                using (Stream stream = File.OpenRead(this.GlobalSettingsFilename))
                {
                    SoapFormatter formatter = new SoapFormatter();
                    formatter.Binder = new GenericDeserializationBinder();

                    try
                    {
                        this.globalSettings = (GlobalSettings)formatter.Deserialize(stream);
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

        protected bool WritedGlobalSettings()
        {
            //	Ecrit le fichier des réglages de l'application.
            this.globalSettings.IsFullScreen = this.Window.IsFullScreen;
            this.globalSettings.MainWindowBounds = this.Window.WindowPlacementBounds;

            this.dlgAbout.Save();
            this.dlgDownload.Save();
            this.dlgExportType.Save();
            this.dlgExport.Save();
            this.dlgExportPDF.Save();
            this.dlgExportICO.Save();
            this.dlgGlyphs.Save();
            this.dlgInfos.Save();
            this.dlgPageStack.Save();
            this.dlgPrint.Save();
            this.dlgReplace.Save();
            this.dlgSettings.Save();
            this.dlgUpdate.Save();

            this.globalSettings.Adorner = Epsitec.Common.Widgets.Adorners.Factory.ActiveName;

            try
            {
                using (Stream stream = File.OpenWrite(this.GlobalSettingsFilename))
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
            //	C:\Documents and Settings\Daniel Roux\Application Data\Epsitec\Crésus documents
            get
            {
                string dir = Common.Support.Globals.Directories.UserAppData;

                if (this.documentType == DocumentType.Pictogram)
                {
                    return string.Concat(dir, "\\CresusPicto2.data"); // (*)
                }
                else
                {
                    return string.Concat(dir, "\\CresusDocuments2.data"); // (*)
                }

                //	(*)	Le nom a passé de 'CresusDocuments.data' à 'CresusDocuments2.data'
                //		afin de repartir d'un fichier neuf, ce qui est nécessaire vu
                //		notamment les changements de dossier 'Samples' en 'Exemples originaux'.
            }
        }
        #endregion


        protected void DeleteTemporaryFiles()
        {
            //	Supprime toutes les images dans le dossier temporaire, issue des opérations "coller".
            try
            {
                if (System.IO.Directory.Exists(Modifier.TemporaryDirectory))
                {
                    string filter = string.Concat(
                        Modifier.ClipboardFilenamePrefix,
                        "*",
                        Modifier.ClipboardFilenamePostfix
                    );
                    string[] list = System.IO.Directory.GetFiles(
                        Modifier.TemporaryDirectory,
                        filter
                    );
                    foreach (string filename in list)
                    {
                        try
                        {
                            System.IO.File.Delete(filename);
                        }
                        catch { }
                    }
                    System.IO.Directory.Delete(Modifier.TemporaryDirectory);
                }
            }
            catch { }
        }

        #region Check
        protected void StartCheck(bool always)
        {
            //	Lance le processus asynchrone qui va se connecter au site web
            //	et regarder s'il y a une version plus récente.
            //	Chaque exécution (qui débouche sur une vérification) incrémente le compteur
            //	'/counter/check/Cresus_Documents' dans la base MySQL (pour la version française).
            if (!always && !this.globalSettings.AutoChecker)
                return;

            if (!always)
            {
                Common.Types.Date today = Common.Types.Date.Today;
                if (Common.Types.Date.Equals(this.globalSettings.DateChecker, today))
                {
                    return;
                }
            }

            //	En français, l'url est "http://www.epsitec.ch/dynamics/check.php?software=Cresus_Documents&version=2.4.0.832".

            this.checker = VersionChecker.CheckUpdate(
                typeof(Common.DocumentEditor.Application).Assembly,
                string.Concat(Res.Strings.Dialog.Update.Web4, "?software={0}&version={1}"),
                Res.Strings.Application.SoftName
            );
        }

        protected void EndCheck(bool always)
        {
            //	Attend la fin du processus de check et indique si une mise à jour est
            //	disponible.
            if (this.checker == null)
            {
                return;
            }

            int attempts = 50;

            while (!this.checker.IsReady && attempts-- > 0)
            {
                System.Threading.Thread.Sleep(100); // attend 0.1s
            }

            this.globalSettings.DateChecker = Common.Types.Date.Today;

            if (this.checker.FoundNewerVersion)
            {
                string version = this.checker.NewerVersion;
                string url = this.checker.NewerVersionUrl;

                this.dlgSplash.Hide();
                this.dlgDownload.SetInfo(version, url);
                this.dlgDownload.Show();
            }
            else if (always)
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
            //	Si elle n'est pas trouvé dans Common.DocumentEditor, elle est
            //	cherchée dans Common.Document !
            string s = Res.Strings.GetString(name);
            if (s == null)
            {
                s = Epsitec.Common.Document.Document.GetRes(name);
            }
            return s;
        }
        #endregion


        protected DocumentType documentType;
        protected InstallType installType;
        protected DebugMode debugMode;
        protected bool useArray;
        protected bool initializationInProgress;
        protected Document clipboard;
        protected DocumentManager defaultDocumentManager;
        protected int currentDocument;
        protected List<DocumentInfo> documents;
        protected GlobalSettings globalSettings;
        protected bool askKey = false;
        protected MouseCursor lastMouseCursor = MouseCursor.AsArrow;
        protected VersionChecker checker;

        protected CommandDispatcher commandDispatcher;
        protected CommandContext commandContext;

        protected HMenu menu;
        protected VMenu fileMenu;
        protected RibbonBook ribbonBook;
        protected RibbonPage ribbonMain;
        protected RibbonPage ribbonGeom;
        protected RibbonPage ribbonOper;
        protected RibbonPage ribbonText;
        protected RibbonPage ribbonActive;
        protected System.Collections.ArrayList ribbonList;
        protected VToolBar vToolBar;
        protected StatusBar info;
        protected ResizeKnob resize;
        protected TabBook bookDocuments;
        protected double panelsWidth = 252;
        protected bool ignoreChange;
        protected int tabIndex;

        protected Dialogs.About dlgAbout;
        protected Dialogs.Download dlgDownload;
        protected Dialogs.ExportType dlgExportType;
        protected Dialogs.Export dlgExport;
        protected Dialogs.ExportPDF dlgExportPDF;
        protected Dialogs.ExportICO dlgExportICO;
        protected Dialogs.Glyphs dlgGlyphs;
        protected Dialogs.Infos dlgInfos;
        protected Dialogs.FileExport dlgFileExport;
        protected Dialogs.FileNew dlgFileNew;
        protected Dialogs.FileOpen dlgFileOpen;
        protected Dialogs.FileOpenModel dlgFileOpenModel;
        protected Dialogs.FileSave dlgFileSave;
        protected Dialogs.FileSaveModel dlgFileSaveModel;
        protected Dialogs.PageStack dlgPageStack;
        protected Dialogs.Print dlgPrint;
        protected Dialogs.Replace dlgReplace;
        protected Dialogs.Settings dlgSettings;
        protected Dialogs.Splash dlgSplash;
        protected Dialogs.Update dlgUpdate;

        protected CommandState toolSelectState;
        protected CommandState toolGlobalState;
        protected CommandState toolShaperState;
        protected CommandState toolEditState;
        protected CommandState toolZoomState;
        protected CommandState toolHandState;
        protected CommandState toolPickerState;
        protected CommandState toolHotSpotState;
        protected CommandState toolLineState;
        protected CommandState toolRectangleState;
        protected CommandState toolCircleState;
        protected CommandState toolEllipseState;
        protected CommandState toolPolyState;
        protected CommandState toolFreeState;
        protected CommandState toolBezierState;
        protected CommandState toolRegularState;
        protected CommandState toolSurfaceState;
        protected CommandState toolVolumeState;
        protected CommandState toolTextLineState;
        protected CommandState toolTextLine2State;
        protected CommandState toolTextBoxState;
        protected CommandState toolTextBox2State;
        protected CommandState toolArrayState;
        protected CommandState toolImageState;
        protected CommandState toolDimensionState;
        protected CommandState newState;
        protected CommandState openState;
        protected CommandState openModelState;
        protected CommandState saveState;
        protected CommandState saveAsState;
        protected CommandState saveModelState;
        protected CommandState closeState;
        protected CommandState closeAllState;
        protected CommandState forceSaveAllState;
        protected CommandState overwriteAllState;
        protected CommandState nextDocState;
        protected CommandState prevDocState;
        protected CommandState printState;
        protected CommandState exportState;
        protected CommandState quickExportState;
        protected CommandState glyphsState;
        protected CommandState glyphsInsertState;
        protected CommandState textEditingState;
        protected CommandState replaceState;
        protected CommandState findNextState;
        protected CommandState findPrevState;
        protected CommandState findDefNextState;
        protected CommandState findDefPrevState;
        protected CommandState deleteState;
        protected CommandState duplicateState;
        protected CommandState cutState;
        protected CommandState copyState;
        protected CommandState pasteState;
        protected CommandState fontBoldState;
        protected CommandState fontItalicState;
        protected CommandState fontUnderlineState;
        protected CommandState fontOverlineState;
        protected CommandState fontStrikeoutState;
        protected CommandState fontSubscriptState;
        protected CommandState fontSuperscriptState;
        protected CommandState fontSizePlusState;
        protected CommandState fontSizeMinusState;
        protected CommandState fontClearState;
        protected CommandState paragraphLeadingState;
        protected CommandState paragraphLeadingPlusState;
        protected CommandState paragraphLeadingMinusState;
        protected CommandState paragraphIndentPlusState;
        protected CommandState paragraphIndentMinusState;
        protected CommandState paragraphJustifState;
        protected CommandState paragraphClearState;
        protected CommandState orderUpOneState;
        protected CommandState orderDownOneState;
        protected CommandState orderUpAllState;
        protected CommandState orderDownAllState;
        protected CommandState moveLeftFreeState;
        protected CommandState moveRightFreeState;
        protected CommandState moveUpFreeState;
        protected CommandState moveDownFreeState;
        protected CommandState rotate90State;
        protected CommandState rotate180State;
        protected CommandState rotate270State;
        protected CommandState rotateFreeCCWState;
        protected CommandState rotateFreeCWState;
        protected CommandState mirrorHState;
        protected CommandState mirrorVState;
        protected CommandState scaleMul2State;
        protected CommandState scaleDiv2State;
        protected CommandState scaleMulFreeState;
        protected CommandState scaleDivFreeState;
        protected CommandState alignLeftState;
        protected CommandState alignCenterXState;
        protected CommandState alignRightState;
        protected CommandState alignTopState;
        protected CommandState alignCenterYState;
        protected CommandState alignBottomState;
        protected CommandState alignGridState;
        protected CommandState resetState;
        protected CommandState shareSpaceXState;
        protected CommandState shareLeftState;
        protected CommandState shareCenterXState;
        protected CommandState shareRightState;
        protected CommandState shareSpaceYState;
        protected CommandState shareTopState;
        protected CommandState shareCenterYState;
        protected CommandState shareBottomState;
        protected CommandState adjustWidthState;
        protected CommandState adjustHeightState;
        protected CommandState colorToRgbState;
        protected CommandState colorToCmykState;
        protected CommandState colorToGrayState;
        protected CommandState colorStrokeDarkState;
        protected CommandState colorStrokeLightState;
        protected CommandState colorFillDarkState;
        protected CommandState colorFillLightState;
        protected CommandState mergeState;
        protected CommandState extractState;
        protected CommandState groupState;
        protected CommandState ungroupState;
        protected CommandState insideState;
        protected CommandState outsideState;
        protected CommandState combineState;
        protected CommandState uncombineState;
        protected CommandState toBezierState;
        protected CommandState toPolyState;
        protected CommandState toTextBox2State;
        protected CommandState fragmentState;
        protected CommandState shaperHandleAddState;
        protected CommandState shaperHandleSubState;
        protected CommandState shaperHandleToLineState;
        protected CommandState shaperHandleToCurveState;
        protected CommandState shaperHandleSymState;
        protected CommandState shaperHandleSmoothState;
        protected CommandState shaperHandleDisState;
        protected CommandState shaperHandleInlineState;
        protected CommandState shaperHandleFreeState;
        protected CommandState shaperHandleSimplyState;
        protected CommandState shaperHandleCornerState;
        protected CommandState shaperHandleContinueState;
        protected CommandState shaperHandleSharpState;
        protected CommandState shaperHandleRoundState;
        protected CommandState booleanAndState;
        protected CommandState booleanOrState;
        protected CommandState booleanXorState;
        protected CommandState booleanFrontMinusState;
        protected CommandState booleanBackMinusState;
        protected CommandState undoState;
        protected CommandState redoState;
        protected CommandState undoRedoListState;
        protected CommandState deselectAllState;
        protected CommandState selectAllState;
        protected CommandState selectInvertState;
        protected CommandState selectorAutoState;
        protected CommandState selectorIndividualState;
        protected CommandState selectorScalerState;
        protected CommandState selectorStretchState;
        protected CommandState selectorStretchTypeState;
        protected CommandState selectTotalState;
        protected CommandState selectPartialState;
        protected CommandState selectorAdaptLine;
        protected CommandState selectorAdaptText;
        protected CommandState hideHalfState;
        protected CommandState hideSelState;
        protected CommandState hideRestState;
        protected CommandState hideCancelState;
        protected CommandState zoomMinState;
        protected CommandState zoomPageState;
        protected CommandState zoomPageWidthState;
        protected CommandState zoomDefaultState;
        protected CommandState zoomSelState;
        protected CommandState zoomSelWidthState;
        protected CommandState zoomPrevState;
        protected CommandState zoomSubState;
        protected CommandState zoomAddState;
        protected CommandState previewState;
        protected CommandState gridState;
        protected CommandState textGridState;
        protected CommandState textShowControlCharactersState;
        protected CommandState textFontFilterState;
        protected CommandState textFontSampleAbcState;
        protected CommandState textInsertQuadState;
        protected CommandState textInsertNewFrameEdges;
        protected CommandState textInsertNewPageState;
        protected CommandState constrainState;
        protected CommandState magnetState;
        protected CommandState magnetLayerState;
        protected CommandState rulersState;
        protected CommandState labelsState;
        protected CommandState aggregatesState;
        protected CommandState arrayOutlineFrameEdges;
        protected CommandState arrayOutlineHorizState;
        protected CommandState arrayOutlineVertiState;
        protected CommandState arrayAddColumnLeftState;
        protected CommandState arrayAddColumnRightState;
        protected CommandState arrayAddRowTopState;
        protected CommandState arrayAddRowBottomState;
        protected CommandState arrayDelColumnState;
        protected CommandState arrayDelRowState;
        protected CommandState arrayAlignColumnState;
        protected CommandState arrayAlignRowState;
        protected CommandState arraySwapColumnState;
        protected CommandState arraySwapRowState;
        protected CommandState arrayLookState;
#if false
		protected CommandState					resDesignerBuildState;
		protected CommandState					resDesignerTranslateState;
#endif
        protected CommandState debugBboxThinState;
        protected CommandState debugBboxGeomState;
        protected CommandState debugBboxFullState;
        protected CommandState debugDirtyState;
        protected CommandState debugCopyFontsState;
        protected CommandState pagePrevState;
        protected CommandState pageNextState;
        protected CommandState pageMenuState;
        protected CommandState pageNewState;
        protected CommandState pageMiniaturesState;
        protected CommandState pageDuplicateState;
        protected CommandState pageUpState;
        protected CommandState pageDownState;
        protected CommandState pageDeleteState;
        protected CommandState layerPrevState;
        protected CommandState layerNextState;
        protected CommandState layerMenuState;
        protected CommandState layerNewState;
        protected CommandState layerMiniaturesState;
        protected CommandState layerDuplicateState;
        protected CommandState layerNewSelState;
        protected CommandState layerMergeUpState;
        protected CommandState layerMergeDownState;
        protected CommandState layerDeleteState;
        protected CommandState layerUpState;
        protected CommandState layerDownState;
        protected CommandState settingsState;
        protected CommandState infosState;
        protected CommandState aboutState;
        protected CommandState pageStackState;
        protected CommandState updateState;
        protected CommandState keyState;
        protected CommandState moveLeftNormState;
        protected CommandState moveRightNormState;
        protected CommandState moveUpNormState;
        protected CommandState moveDownNormState;
        protected CommandState moveLeftCtrlState;
        protected CommandState moveRightCtrlState;
        protected CommandState moveUpCtrlState;
        protected CommandState moveDownCtrlState;
        protected CommandState moveLeftShiftState;
        protected CommandState moveRightShiftState;
        protected CommandState moveUpShiftState;
        protected CommandState moveDownShiftState;

        protected class DocumentInfo
        {
            public Document document;
            public TabPage tabPage;
            public DocWidgets.HRuler hRuler;
            public DocWidgets.VRuler vRuler;
            public HScroller hScroller;
            public VScroller vScroller;
            public GlyphButton quickPageMiniatures;
            public Button quickPageMenu;
            public GlyphButton quickLayerMiniatures;
            public Button quickLayerMenu;
            public TabBook bookPanels;
            public Containers.Principal containerPrincipal;
            public Containers.Styles containerStyles;
            public Containers.Autos containerAutos;
            public Containers.Pages containerPages;
            public Containers.Layers containerLayers;
            public Containers.Operations containerOperations;
            public FrameBox pagePane;
            public FrameBox mainPane;
            public FrameBox layerPane;
            public Containers.PageMiniatures pageMiniatures;
            public Containers.LayerMiniatures layerMiniatures;

            public void Dispose()
            {
                this.document.Dispose();
                if (this.tabPage != null)
                    this.tabPage.Dispose();
                if (this.hRuler != null)
                    this.hRuler.Dispose();
                if (this.vRuler != null)
                    this.vRuler.Dispose();
                if (this.hScroller != null)
                    this.hScroller.Dispose();
                if (this.vScroller != null)
                    this.vScroller.Dispose();
                if (this.quickPageMiniatures != null)
                    this.quickPageMiniatures.Dispose();
                if (this.quickPageMenu != null)
                    this.quickPageMenu.Dispose();
                if (this.quickLayerMiniatures != null)
                    this.quickLayerMiniatures.Dispose();
                if (this.quickLayerMenu != null)
                    this.quickLayerMenu.Dispose();
                if (this.bookPanels != null)
                    this.bookPanels.Dispose();
                if (this.containerPrincipal != null)
                    this.containerPrincipal.Dispose();
                if (this.containerStyles != null)
                    this.containerStyles.Dispose();
                if (this.containerAutos != null)
                    this.containerAutos.Dispose();
                if (this.containerPages != null)
                    this.containerPages.Dispose();
                if (this.containerLayers != null)
                    this.containerLayers.Dispose();
                if (this.containerOperations != null)
                    this.containerOperations.Dispose();
            }
        }
    }
}
