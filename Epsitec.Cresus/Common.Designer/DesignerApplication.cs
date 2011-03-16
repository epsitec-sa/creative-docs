using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer
{
	public enum DesignerMode
	{
		Build,
		Translate,
	}

	/// <summary>
	/// Fenêtre principale de l'éditeur de ressources.
	/// </summary>
	public class DesignerApplication : Application
	{
		public enum DisplayMode
		{
			Horizontal,
			Vertical,
			FullScreen,
			Window,
		}


		static DesignerApplication()
		{
			Res.Initialize();

			ImageProvider.Default.EnableLongLifeCache = true;
			ImageProvider.Default.PrefillManifestIconCache();
		}

		public DesignerApplication() : this(new ResourceManagerPool("Common.Designer"))
		{
			this.resourceManagerPool.DefaultPrefix = "file";
			this.resourceManagerPool.SetupDefaultRootPaths();
		}

		public DesignerApplication(ResourceManagerPool pool)
		{
			this.resourceManagerPool = pool;
			this.LocatorInit();
			this.moduleInfoList = new List<ModuleInfo>();
			this.settings = Epsitec.Common.Designer.Settings.Default;
			this.context = new PanelsContext(this);
			
			Widgets.CommandDispatcher.CommandDispatching += this.HandleCommandDispatcherCommandDispatching;
		}

		public void Show(Window parentWindow)
		{
			//	Crée et montre la fenêtre de l'éditeur.
			if (this.Window == null)
			{
				string path = System.IO.Path.Combine (Globals.Directories.ExecutableRoot, "app.ico");
				this.icon = Epsitec.Common.Drawing.Bitmap.FromNativeIcon (path, 48, 48);

				Window window = new Window ();
				this.Window = window;
				window.Root.WindowStyles = WindowStyles.DefaultDocumentWindow;
				window.Icon = this.icon;

				Point parentCenter;
				Rectangle windowBounds;

				if (parentWindow == null)
				{
					Rectangle area = ScreenInfo.GlobalArea;
					parentCenter = area.Center;
				}
				else
				{
					parentCenter = parentWindow.WindowBounds.Center;
				}

				windowBounds = new Rectangle(parentCenter.X-1000/2, parentCenter.Y-700/2, 1000, 700);
				windowBounds = ScreenInfo.FitIntoWorkingArea(windowBounds);

				window.WindowBounds = windowBounds;
				window.Root.MinSize = new Size(500, 400);
				window.Text = Res.Strings.Application.Title;
				window.Name = "Application";  // utilisé pour générer "QuitApplication" !
				window.PreventAutoClose = true;
				
				DesignerApplication.SetInstance(window, this);  // attache l'instance de DesignerApplication à la fenêtre

				this.dlgNew              = new Dialogs.NewDialog(this);
				this.dlgOpen             = new Dialogs.OpenDialog(this);
				this.dlgGlyphs           = new Dialogs.GlyphsDialog(this);
				this.dlgIcon             = new Dialogs.IconDialog(this);
				this.dlgSearch           = new Dialogs.SearchDialog(this);
				this.dlgNewCulture       = new Dialogs.NewCultureDialog(this);
				this.dlgResourceTypeCode = new Dialogs.ResourceTypeCodeDialog(this);
				this.dlgResourceSelector = new Dialogs.ResourceSelectorDialog(this);
				this.dlgBindingSelector  = new Dialogs.BindingSelectorDialog(this);
				this.dlgFieldName        = new Dialogs.ResourceNameDialog(this);
				this.dlgEntityField      = new Dialogs.EntityFieldDialog(this);
				this.dlgLabelReplacement = new Dialogs.LabelReplacementDialog(this);
				this.dlgEntityComment    = new Dialogs.EntityCommentDialog (this);
				this.dlgEntityParameters = new Dialogs.EntityParametersDialog (this);
				this.dlgEntityCreation   = new Dialogs.EntityCreationDialog (this);
				this.dlgEntityExpression = new Dialogs.EntityExpressionDialog(this);
				this.dlgInitialMessage   = new Dialogs.InitialMessageDialog (this);
				this.dlgModuleInfo       = new Dialogs.ModuleInfoDialog (this);
				this.dlgSaveAllImages    = new Dialogs.SaveAllImagesDialog (this);

				this.dlgGlyphs.Closed         += this.HandleDlgClosed;
				this.dlgSearch.Closed         += this.HandleDlgClosed;
				this.dlgInitialMessage.Closed += this.HandleDlgClosed;

				//	Les réglages doivent être lus avant de créer l'interface graphique.
				this.settings.Read();

				this.InitCommands();
				this.CreateLayout();
			}

			this.ReadSettings();
			this.Window.Show();

			this.UpdateInitialMessage();
			this.ShowInitialMessage();  // s'il existe, affiche le message initial
		}

		internal void Hide()
		{
			this.Window.Hide();
		}

		public override string ShortWindowTitle
		{
			get
			{
				return Res.Strings.Application.Title;
			}
		}

		public override string ApplicationIdentifier
		{
			get
			{
				return "EpDesignerApp";
			}
		}

		public bool Standalone
		{
			//	Standalone = true signifie que Designer est une application à
			//	part entière : quand on ferme la fenêtre, on doit réellement
			//	quitter l'application.
			get
			{
				return this.standalone;
			}
			set
			{
				this.standalone = value;
			}
		}

		public Settings Settings
		{
			get
			{
				return this.settings;
			}
		}
		
		public DesignerMode Mode
		{
			//	Retourne le mode de fonctionnement de l'application.
			get
			{
				return this.mode;
			}
			set
			{
				this.mode = value;
			}
		}

		public new ResourceManagerPool ResourceManagerPool
		{
			get
			{
				return this.resourceManagerPool;
			}
		}
		
		public CommandState GetCommandState(string command)
		{
			CommandContext context = this.CommandContext;
			CommandState state = context.GetCommandState (Command.Get (command));

			return state;
		}

		public double MoveHorizontal
		{
			get
			{
				return this.moveHorizontal;
			}
			set
			{
				this.moveHorizontal = value;
			}
		}

		public double MoveVertical
		{
			get
			{
				return this.moveVertical;
			}
			set
			{
				this.moveVertical = value;
			}
		}

		public Image Icon
		{
			get
			{
				return this.icon;
			}
		}

		private void CreateLayout()
		{
			this.ribbonBook = new RibbonBook(this.Window.Root);
			this.ribbonBook.Dock = DockStyle.Top;

			//	Crée le ruban principal.
			this.ribbonMain = new RibbonPage();
			this.ribbonMain.RibbonTitle = Res.Strings.Ribbon.Main;
			this.ribbonBook.Pages.Add(this.ribbonMain);

			this.ribbonMain.Items.Add(new Ribbons.IdentityRibbon(this));
			this.ribbonMain.Items.Add(new Ribbons.FileRibbon(this));
			this.ribbonMain.Items.Add(new Ribbons.ClipboardRibbon(this));
			if (this.mode == DesignerMode.Build)
			{
				this.ribbonMain.Items.Add(new Ribbons.UndoRibbon(this));
				this.ribbonMain.Items.Add(new Ribbons.EditRibbon(this));
			}
			this.ribbonMain.Items.Add(new Ribbons.AccessRibbon(this));
			this.ribbonMain.Items.Add(new Ribbons.DisplayRibbon(this));
			this.ribbonMain.Items.Add(new Ribbons.LocatorRibbon(this));

			//	Crée le ruban des opérations.
			this.ribbonOper = new RibbonPage();
			this.ribbonOper.RibbonTitle = Res.Strings.Ribbon.Oper;
			this.ribbonBook.Pages.Add(this.ribbonOper);

			this.ribbonOper.Items.Add(new Ribbons.CultureRibbon(this));
			this.ribbonOper.Items.Add(new Ribbons.CharacterRibbon(this));
			this.ribbonOper.Items.Add(new Ribbons.PanelShowRibbon(this));
			this.ribbonOper.Items.Add(new Ribbons.PanelSelectRibbon(this));
			this.ribbonOper.Items.Add(new Ribbons.MoveRibbon(this));
			this.ribbonOper.Items.Add(new Ribbons.AlignRibbon(this));
			this.ribbonOper.Items.Add(new Ribbons.OrderRibbon(this));
			this.ribbonOper.Items.Add(new Ribbons.TabIndexRibbon(this));

			//	Crée la barre de statuts.
			this.info = new StatusBar(this.Window.Root);
			this.info.Dock = DockStyle.Bottom;
			this.info.Margins = new Margins(0, 0, 0, 0);

			this.InfoAdd("InfoCurrentModule", 120);
			this.InfoAdd("InfoAccess", 250);
			this.InfoAdd("InfoViewer", 300);

			this.resize = new ResizeKnob();
			this.resize.Margins = new Margins(2, 0, 0, 0);
			this.info.Items.Add(this.resize);
			this.resize.Dock = DockStyle.Right;  // doit être fait après le Items.Add !
			ToolTip.Default.SetToolTip(this.resize, Res.Strings.Dialog.Tooltip.Resize);

			//	Crée le TabBook principal pour les modules ouverts.
			this.bookModules = new TabBook(this.Window.Root);
			this.bookModules.Dock = DockStyle.Fill;
			this.bookModules.Margins = new Margins(0, 0, 3, 0);
			this.bookModules.Arrows = TabBookArrows.Right;
			this.bookModules.HasCloseButton = true;
			this.bookModules.CloseButton.CommandObject = Command.Get("Close");
			this.bookModules.ActivePageChanged += new EventHandler<CancelEventArgs>(this.HandleBookModulesActivePageChanged);
			ToolTip.Default.SetToolTip(this.bookModules.CloseButton, Res.Strings.Action.Close);

			this.ribbonActive = this.ribbonMain;
			this.ribbonBook.ActivePage = this.ribbonMain;
		}

		protected override void ExecuteQuit(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			//	Evite que cette commande ne soit exécutée par Widgets.Application,
			//	car cela provoquerait la fin du programme, quelle que soit la
			//	réponse donnée par l'utilisateur au dialogue affiché par DocumentEditor.
		}

		public void ActiveButton(string command, bool active)
		{
			//	Active un bouton dans un ruban.
			IconButton button = this.SearchIconButton(command);
			if (button != null)
			{
				//this.GetCommandState(command).ActiveState = active ? ActiveState.Yes : ActiveState.No;
				button.PreferredIconStyle = active ? "Active" : null;
			}
		}

		private void StyleButton(string command, string style)
		{
			//	Modifie le style d'un bouton dans un ruban. L'icône affichée change en fonction des noms
			//	de style dans les "pages" définis depuis CrPicto.
			IconButton button = this.SearchIconButton(command);
			if (button != null)
			{
				button.PreferredIconStyle = style;
			}
		}

		private IconButton SearchIconButton(string command)
		{
			//	Cherche le bouton utilisé pour une commande, dans tous les rubans.
			IconButton button;

			button = this.SearchIconButton(this.ribbonMain, command);
			if (button != null)
			{
				return button;
			}

			button = this.SearchIconButton(this.ribbonOper, command);
			if (button != null)
			{
				return button;
			}

			return null;
		}

		private IconButton SearchIconButton(RibbonPage page, string command)
		{
			//	Cherche le bouton utilisé pour une commande, dans un ruban.
			foreach (Widget widget in page.Items)
			{
				Ribbons.AbstractRibbon section = widget as Ribbons.AbstractRibbon;
				if (section != null)
				{
					IconButton button = section.SearchIconButton(command);
					if (button != null)
					{
						return button;
					}
				}
			}

			return null;
		}

		private void HandleCommandDispatcherCommandDispatching(object sender, CommandEventArgs e)
		{
			AbstractTextField focusedWidget = this.Window.FocusedWidget as AbstractTextField;

			if ((focusedWidget != null) &&
				(focusedWidget.IsEditing))
			{
				this.Window.ClearFocusedWidget();
				this.Window.FocusWidget(focusedWidget);
			}
		}


		#region Info manager
		private StatusField InfoAdd(string name, double width)
		{
			StatusField field = new StatusField();
			field.PreferredWidth = width;
			this.info.Items.Add(field);

			int i = this.info.Children.Count-1;
			this.info.Items[i].Name = name;
			return field;
		}

		public void UpdateInfoCurrentModule()
		{
			StatusField field = this.info.Items["InfoCurrentModule"] as StatusField;
			field.Text = this.IsReadonly ? Res.Strings.Application.Mode.Lock : Res.Strings.Application.Mode.Unlock;
		}

		public void UpdateInfoAccess()
		{
			string text = "";
			Module module = this.CurrentModule;
			if (module != null && module.Modifier.ActiveViewer != null)
			{
				text = module.Modifier.ActiveViewer.InfoAccessText;
			}

			StatusField field = this.info.Items["InfoAccess"] as StatusField;

			if (field.Text != text)
			{
				field.Text = text;
				field.Invalidate();
			}
		}

		public void UpdateInfoViewer()
		{
			string text = "";
			Module module = this.CurrentModule;
			if (module != null && module.Modifier.ActiveViewer!= null)
			{
				text = module.Modifier.ActiveViewer.InfoViewerText;
			}

			StatusField field = this.info.Items["InfoViewer"] as StatusField;

			if (field.Text != text)
			{
				field.Text = text;
				field.Invalidate();
			}
		}

		public void UpdateViewer(Viewers.Changing oper)
		{
			//	Met à jour le visualisateur en cours.
			Module module = this.CurrentModule;
			if (module != null && module.Modifier.ActiveViewer!= null)
			{
				module.Modifier.ActiveViewer.UpdateViewer(oper);
			}
		}
		#endregion


		#region Commands manager
		[Command("Open")]
		void CommandOpen(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CloseInitialMessage();

			if (!this.Terminate())
			{
				return;
			}

			this.dlgOpen.SetResourcePrefix(this.resourceManagerPool.DefaultPrefix);
			this.dlgOpen.Show();

			ResourceModuleId item = this.dlgOpen.SelectedModule;
			if (item.Name != null)
			{

				var mi = this.OpenModule (item);
				this.bookModules.ActivePage = mi.TabPage;

				//	Affiche l'éventuel message initial.
				this.UpdateInitialMessage();
				if (this.CurrentModule.InitialMessage != null)  // message initial existe ?
				{
					this.ShowInitialMessage();  // affiche le message initial
				}
			}
		}

		[Command("New")]
		void CommandNew(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CloseInitialMessage();

			if (!this.Terminate())
			{
				return;
			}

			string actualModuleName  = null;
			string rootDirectoryPath = ResourceManagerPool.SymbolicNames.Custom;
			string moduleName        = "";
			string sourceNamespace   = "";

			if (this.IsCurrentModule)
			{
				actualModuleName = this.CurrentModuleInfo.Module.ModuleId.Name;
				ResourceModuleInfo info = this.CurrentModuleInfo.Module.ResourceManager.DefaultModuleInfo;
				string[] namespaceElements = (info.SourceNamespaceRes ?? "").Split ('.');
				sourceNamespace = string.Join(".", Common.Types.Collection.StripLast(namespaceElements));
			}

			this.dlgNew.Initialize(actualModuleName, rootDirectoryPath, moduleName, sourceNamespace, ResourceModuleLayer.Application);
			this.dlgNew.Show();  // montre le dialogue et attend...

			rootDirectoryPath = this.dlgNew.RootDirectoryPath;
			moduleName        = this.dlgNew.ModuleName;
			sourceNamespace   = this.dlgNew.SourceNamespace;

			if (!string.IsNullOrEmpty(rootDirectoryPath))
			{
				ModuleSupport.ModuleStore store = new ModuleSupport.ModuleStore(this.resourceManagerPool);
				ResourceModuleInfo info = null;

				if (this.dlgNew.IsPatch)
				{
					Common.Dialogs.WorkInProgressDialog.Execute(Res.Strings.Dialog.New.Progress.Title, ProgressIndicatorStyle.UnknownDuration,
						progress =>
						{
							info = store.CreatePatchModule(rootDirectoryPath, this.CurrentModuleInfo.Module.ResourceManager.DefaultModuleInfo, this.settings.IdentityCard);
						},
						this.Window);
				}
				else
				{
					Common.Dialogs.WorkInProgressDialog.Execute(Res.Strings.Dialog.New.Progress.Title, ProgressIndicatorStyle.UnknownDuration,
						progress =>
						{
							info = store.CreateReferenceModule(rootDirectoryPath, moduleName, sourceNamespace, this.dlgNew.ResourceModuleLayer, this.settings.IdentityCard);
						},
						this.Window);
				}

				if (info == null)
				{
					this.DialogError(Res.Strings.Dialog.New.Error.Create);
				}
				else
				{
					// Ouvre le module d'on vient de créer.
					if (this.dlgNew.IsPatch)
					{
						this.CloseModule();  // ferme le module initial de référence
					}

					Module module = new Module (this, this.mode, info.FullId, this.MissingModuleResolver);

					ModuleInfo mi = new ModuleInfo();
					mi.Module = module;
					this.moduleInfoList.Insert(++this.currentModule, mi);
					this.CreateModuleLayout();

					this.bookModules.ActivePage = mi.TabPage;
				}
			}
		}

		[Command("Recycle")]
		void CommandRecycle(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CloseInitialMessage();

			if (!this.IsCurrentModule || !this.Terminate())
			{
				return;
			}

			string name = this.CurrentModuleInfo.Module.ModuleId.Name;
			string question = string.Format(Res.Strings.Dialog.Recycle.Question, name);
			if (this.DialogQuestion(question) == Common.Dialogs.DialogResult.Yes)
			{
				ModuleSupport.ModuleStore store = new ModuleSupport.ModuleStore(this.resourceManagerPool);
				ResourceModuleInfo info = this.CurrentModuleInfo.Module.ResourceManager.DefaultModuleInfo;

				bool ok = false;
				
				try
				{
					FileOperationMode mode = new FileOperationMode(this.Window);

					if (FileManager.DeleteFile(mode, info.FullId.Path))
					{
						if (info != null && (info.IsPatchModule || info.PatchDepth > 0))
						{
							ok = true;
						}
						else
						{
							Common.Dialogs.WorkInProgressDialog.Execute(Res.Strings.Dialog.Recycle.Progress.Title, ProgressIndicatorStyle.UnknownDuration,
								progress =>
								{
									ok = store.RecycleModule(info, this.settings.IdentityCard);
								},
								this.Window);
						}
					}
				}
				catch
				{
				}
				
				if (ok)
				{
					string message = string.Format(Res.Strings.Dialog.Recycle.Message.Ok, name);
					this.DialogMessage(message);

					this.CloseModule();  // ferme le module qu'on vient de recycler
				}
				else
				{
					string message = string.Format(Res.Strings.Dialog.Recycle.Error.Recycle, name);
					this.DialogError(message);
				}
			}
		}

		[Command("Check")]
		void CommandCheck(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			if (!this.Terminate())
			{
				return;
			}

			this.CurrentModule.Check();
		}

		[Command ("InitialMessage")]
		void CommandInitialMessage(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.ShowInitialMessage();
		}

		[Command("Save")]
		void CommandSave(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			if (!this.Terminate())
			{
				return;
			}

			this.CurrentModule.Save();
		}

		[Command("Close")]
		void CommandClose(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CloseInitialMessage();

			if (!this.Terminate())
			{
				return;
			}

			if (!this.AutoSave(dispatcher))
			{
				return;
			}

			this.CloseModule();
			this.UpdateAfterTypeChanged();
		}

		[Command(ApplicationCommands.Id.Quit)]
		[Command("QuitApplication")]
		void CommandQuitApplication(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			e.Executed = true;

			if (!this.Terminate())
			{
				return;
			}

			if (!this.AutoSaveAll(dispatcher))
			{
				return;
			}

			this.WriteSettings();

			this.dlgGlyphs.Hide();
			this.dlgSearch.Hide();

			if (this.standalone)
			{
				Window.Quit();
			}
			else
			{
				this.Window.Hide();
			}
		}

		[Command("DesignerGlyphs")]
		void CommandGlyphs(CommandDispatcher dispatcher, CommandEventArgs e)
		{
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

		[Command("Search")]
		void CommandSearch(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			if (this.searchState.ActiveState == ActiveState.No)
			{
				this.dlgSearch.Show();
				this.searchState.ActiveState = ActiveState.Yes;
			}
			else
			{
				this.dlgSearch.Hide();
				this.searchState.ActiveState = ActiveState.No;
			}
		}

		[Command("SearchPrev")]
		[Command("SearchNext")]
		void CommandSearchDirect(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			string searching = this.dlgSearch.Searching;
			Searcher.SearchingMode mode = this.dlgSearch.Mode;
			List<int> filter = this.dlgSearch.FilterList;

			if (e.Command.CommandId == "SearchPrev")
			{
				mode |= Searcher.SearchingMode.Reverse;
			}

			this.CurrentModule.Modifier.ActiveViewer.DoSearch(searching, mode, filter);
		}

		[Command("AccessFirst")]
		[Command("AccessPrev")]
		[Command("AccessNext")]
		[Command("AccessLast")]
		void CommandAccess(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			if ( !this.IsCurrentModule )  return;
			this.CurrentModule.Modifier.ActiveViewer.DoAccess(e.Command.CommandId);
		}

		[Command("ModificationAll")]
		[Command("ModificationClear")]
		[Command("ModificationPrev")]
		[Command("ModificationNext")]
		void CommandModification(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			if ( !this.IsCurrentModule )  return;
			this.CurrentModule.Modifier.ActiveViewer.DoModification(e.Command.CommandId);
		}

		[Command("NewCulture")]
		void CommandNewCulture(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			if (!this.Terminate())
			{
				return;
			}

			if (!this.IsCurrentModule)
			{
				return;
			}

			this.CurrentModule.Modifier.ActiveViewer.DoNewCulture();
		}

		[Command("DeleteCulture")]
		void CommandDeleteCulture(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			if (!this.Terminate())
			{
				return;
			}

			if (!this.IsCurrentModule)
			{
				return;
			}

			this.CurrentModule.Modifier.ActiveViewer.DoDeleteCulture();
		}

		[Command("EditLocked")]
		void CommandEditLocked(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			if (!this.Terminate())
			{
				return;
			}

			this.IsEditLocked = !this.IsEditLocked;
		}

		[Command("EditOk")]
		void CommandEditOk(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			if (!this.IsCurrentModule)
			{
				return;
			}

			this.CurrentModule.Modifier.ActiveViewer.Terminate(false);
		}

		[Command("EditCancel")]
		void CommandEditCancel(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			if (!this.IsCurrentModule)
			{
				return;
			}

			this.CurrentModule.Modifier.ActiveViewer.RevertChanges();
		}

		[Command("Delete")]
		void CommandDelete(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			if (!this.Terminate())
			{
				return;
			}

			if (!this.IsCurrentModule)
			{
				return;
			}

			this.CurrentModule.Modifier.ActiveViewer.DoDelete();
		}

		[Command("Create")]
		void CommandCreate(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			if (!this.Terminate())
			{
				return;
			}

			if (!this.IsCurrentModule)
			{
				return;
			}

			this.CurrentModule.Modifier.ActiveViewer.DoDuplicate(false);
		}

		[Command("Duplicate")]
		void CommandDuplicate(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			if (!this.Terminate())
			{
				return;
			}

			if (!this.IsCurrentModule)
			{
				return;
			}

			this.CurrentModule.Modifier.ActiveViewer.DoDuplicate(true);
		}

		[Command("CopyToModule")]
		void CommandCopyToModule(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			if (!this.Terminate())
			{
				return;
			}

			if (!this.IsCurrentModule)
			{
				return;
			}

			this.CurrentModule.Modifier.ActiveViewer.DoCopyToModule(this.LastModule);
		}

		[Command("Cut")]
		[Command("Copy")]
		[Command("Paste")]
		void CommandClipboard(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			if (!this.IsCurrentModule)
			{
				return;
			}

			this.CurrentModule.Modifier.ActiveViewer.DoClipboard(e.Command.CommandId);
		}

		[Command("FontBold")]
		[Command("FontItalic")]
		[Command("FontUnderline")]
		void CommandFont(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			if (!this.IsCurrentModule)
			{
				return;
			}

			this.CurrentModule.Modifier.ActiveViewer.DoFont(e.Command.CommandId);
		}

		[Command("ToolSelect")]
		[Command("ToolGlobal")]
		[Command("ToolGrid")]
		[Command("ToolEdit")]
		[Command("ToolZoom")]
		[Command("ToolHand")]
		[Command("ObjectHLine")]
		[Command("ObjectVLine")]
		[Command("ObjectSquareButton")]
		[Command("ObjectRectButton")]
		[Command("ObjectTable")]
		[Command("ObjectText")]
		[Command("ObjectStatic")]
		[Command("ObjectGroup")]
		[Command("ObjectGroupFrame")]
		[Command("ObjectGroupBox")]
		[Command("ObjectPanel")]
		void CommandTool(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			if (!this.IsCurrentModule)
			{
				return;
			}

			this.CurrentModule.Modifier.ActiveViewer.DoTool(e.Command.CommandId);
		}

		[Command("PanelDeselectAll")]
		[Command("PanelSelectAll")]
		[Command("PanelSelectInvert")]
		[Command("PanelSelectRoot")]
		[Command("PanelSelectParent")]
		[Command("PanelShowGrid")]
		[Command("PanelShowZOrder")]
		[Command("PanelShowTabIndex")]
		[Command("PanelShowConstrain")]
		[Command("PanelShowExpand")]
		[Command("PanelShowAttachment")]
		[Command("PanelRun")]
		[Command("AlignLeft")]
		[Command("AlignCenterX")]
		[Command("AlignRight")]
		[Command("AlignTop")]
		[Command("AlignCenterY")]
		[Command("AlignBottom")]
		[Command("AlignBaseLine")]
		[Command("AdjustWidth")]
		[Command("AdjustHeight")]
		[Command("AlignGrid")]
		[Command("MoveLeft")]
		[Command("MoveRight")]
		[Command("MoveDown")]
		[Command("MoveUp")]
		[Command("PanelOrderUpAll")]
		[Command("PanelOrderDownAll")]
		[Command("PanelOrderUpOne")]
		[Command("PanelOrderDownOne")]
		[Command("TabIndexClear")]
		[Command("TabIndexFirst")]
		[Command("TabIndexPrev")]
		[Command("TabIndexNext")]
		[Command("TabIndexLast")]
		[Command("TabIndexRenum")]
		[Command("FormFieldsShowPrefix")]
		[Command("FormFieldsShowGuid")]
		[Command("FormFieldsShowColumn1")]
		[Command("FormFieldsShowColumn2")]
		[Command("FormFieldsClearDelta")]
		[Command("ShowBothCulture")]
		[Command("ShowPrimaryCulture")]
		[Command("ShowSecondaryCulture")]
		void CommandCommand(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			if (!this.IsCurrentModule)
			{
				return;
			}

			this.CurrentModule.Modifier.ActiveViewer.DoCommand(e.Command.CommandId);
		}

		[Command("Undo")]
		void CommandUndo(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.Undo();
		}

		[Command("Redo")]
		void CommandRedo(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.Redo();
		}

		[Command("UndoRedoListDo")]
		void CommandUndoRedoListDo(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			string value = e.CommandState.AdvancedState;
			int i = System.Convert.ToInt32(value);
			this.UndoRedoMenuGoto(i);
		}

		[Command("LocatorPrev")]
		void CommandLocatorPrev(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.LocatorPrev();
		}

		[Command("LocatorNext")]
		void CommandLocatorNext(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.LocatorNext();
		}

		[Command ("LocatorListDo")]
		void CommandLocatorListDo(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			string value = e.CommandState.AdvancedState;
			int i = System.Convert.ToInt32(value);
			this.LocatorMenuGoto(i);
		}

		[Command("DisplayHorizontal")]
		void CommandDisplayHorizontal(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.DisplayModeState = DisplayMode.Horizontal;
		}

		[Command("DisplayVertical")]
		void CommandDisplayVertical(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.DisplayModeState = DisplayMode.Vertical;
		}

		[Command("DisplayFullScreen")]
		void CommandDisplayFullScreen(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.DisplayModeState = DisplayMode.FullScreen;
		}

		[Command("DisplayWindow")]
		void CommandDisplayWindow(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.DisplayModeState = DisplayMode.Window;
		}

		[Command("ZoomChange")]
		void CommandZoomChange(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			string value = e.CommandState.AdvancedState;
			double zoom = System.Convert.ToDouble(value);

			Module module = this.CurrentModule;
			Viewers.Entities ve = module.Modifier.ActiveViewer as Viewers.Entities;
			if (module != null && ve != null)
			{
				ve.Zoom = zoom;
			}
		}

		private void InitCommands()
		{
			this.newState = this.CreateCommandState("New", KeyCode.ModifierControl|KeyCode.AlphaN);
			this.recycleState = this.CreateCommandState("Recycle");
			this.openState = this.CreateCommandState("Open", KeyCode.ModifierControl|KeyCode.AlphaO);
			this.saveState = this.CreateCommandState("Save", KeyCode.ModifierControl|KeyCode.AlphaS);
			this.saveAsState = this.CreateCommandState("SaveAs");
			this.initialMessageState = this.CreateCommandState("InitialMessage");
			this.checkState = this.CreateCommandState ("Check");
			this.closeState = this.CreateCommandState("Close", KeyCode.ModifierControl|KeyCode.FuncF4);
			this.cutState = this.CreateCommandState("Cut", KeyCode.ModifierControl|KeyCode.AlphaX);
			this.copyState = this.CreateCommandState("Copy", KeyCode.ModifierControl|KeyCode.AlphaC);
			this.pasteState = this.CreateCommandState("Paste", KeyCode.ModifierControl|KeyCode.AlphaV);
			this.editLockedState = this.CreateCommandState("EditLocked", KeyCode.ModifierControl|KeyCode.AlphaL);
			this.editOkState = this.CreateCommandState("EditOk", KeyCode.FuncF12);
			this.editCancelState = this.CreateCommandState("EditCancel");
			this.deleteState = this.CreateCommandState("Delete", KeyCode.ModifierControl|KeyCode.Delete);
			this.createState = this.CreateCommandState("Create", KeyCode.ModifierAlt|KeyCode.AlphaN);
			this.duplicateState = this.CreateCommandState("Duplicate");
			this.copyToModuleState = this.CreateCommandState("CopyToModule", KeyCode.ModifierControl|KeyCode.AlphaM);

			this.fontBoldState = this.CreateCommandState("FontBold", KeyCode.ModifierControl|KeyCode.AlphaB);
			this.fontItalicState = this.CreateCommandState("FontItalic", KeyCode.ModifierControl|KeyCode.AlphaI);
			this.fontUnderlineState = this.CreateCommandState("FontUnderline", KeyCode.ModifierControl|KeyCode.AlphaU);

			this.glyphsState = this.CreateCommandState("DesignerGlyphs");
			this.filterState = this.CreateCommandState("Filter");
			
			this.searchState = this.CreateCommandState("Search", KeyCode.ModifierControl|KeyCode.AlphaE);
			this.searchPrevState = this.CreateCommandState("SearchPrev", KeyCode.FuncF3|KeyCode.ModifierShift);
			this.searchNextState = this.CreateCommandState("SearchNext", KeyCode.FuncF3);
			
			this.accessFirstState = this.CreateCommandState("AccessFirst");
			this.accessPrevState = this.CreateCommandState("AccessPrev", KeyCode.FuncF6|KeyCode.ModifierShift);
			this.accessNextState = this.CreateCommandState("AccessNext", KeyCode.FuncF6);
			this.accessLastState = this.CreateCommandState("AccessLast");
			
			this.modificationAllState = this.CreateCommandState("ModificationAll");
			this.modificationClearState = this.CreateCommandState("ModificationClear", KeyCode.FuncF8);
			this.modificationPrevState = this.CreateCommandState("ModificationPrev", KeyCode.FuncF7|KeyCode.ModifierShift);
			this.modificationNextState = this.CreateCommandState("ModificationNext", KeyCode.FuncF7);
			
			this.newCultureState = this.CreateCommandState("NewCulture");
			this.deleteCultureState = this.CreateCommandState("DeleteCulture");
			
			this.toolSelectState = this.CreateCommandState("ToolSelect", KeyCode.AlphaS);
			this.toolGlobalState = this.CreateCommandState("ToolGlobal", KeyCode.AlphaG);
			this.toolGridState = this.CreateCommandState("ToolGrid", KeyCode.AlphaA);
			this.toolEditState = this.CreateCommandState("ToolEdit", KeyCode.AlphaE);
			this.toolZoomState = this.CreateCommandState("ToolZoom", KeyCode.AlphaZ);
			this.toolHandState = this.CreateCommandState("ToolHand", KeyCode.AlphaH);

			this.objectHLineState = this.CreateCommandState("ObjectHLine", KeyCode.AlphaL);
			this.objectVLineState = this.CreateCommandState("ObjectVLine");
			this.objectSquareButtonState = this.CreateCommandState("ObjectSquareButton");
			this.objectRectButtonState = this.CreateCommandState("ObjectRectButton", KeyCode.AlphaB);
			this.objectTableState = this.CreateCommandState("ObjectTable");
			this.objectTextState = this.CreateCommandState("ObjectText", KeyCode.AlphaT);
			this.objectStaticState = this.CreateCommandState("ObjectStatic");
			this.objectGroupState = this.CreateCommandState("ObjectGroup");
			this.objectGroupFrameState = this.CreateCommandState("ObjectGroupFrame");
			this.objectGroupBoxState = this.CreateCommandState("ObjectGroupBox");
			this.objectPanelState = this.CreateCommandState("ObjectPanel");
			
			this.panelDeselectAllState = this.CreateCommandState("PanelDeselectAll", KeyCode.Escape);
			this.panelSelectAllState = this.CreateCommandState("PanelSelectAll");
			this.panelSelectInvertState = this.CreateCommandState("PanelSelectInvert");
			this.panelSelectRootState = this.CreateCommandState("PanelSelectRoot");
			this.panelSelectParentState = this.CreateCommandState("PanelSelectParent", KeyCode.AlphaP);

			this.panelShowGridState = this.CreateCommandState("PanelShowGrid");
			this.panelShowZOrderState = this.CreateCommandState("PanelShowZOrder");
			this.panelShowTabIndexState = this.CreateCommandState("PanelShowTabIndex");
			this.panelShowExpandState = this.CreateCommandState("PanelShowExpand");
			this.panelShowConstrainState = this.CreateCommandState("PanelShowConstrain");
			this.panelShowAttachmentState = this.CreateCommandState("PanelShowAttachment");
			this.panelRunState = this.CreateCommandState("PanelRun");

			this.alignLeftState = this.CreateCommandState("AlignLeft");
			this.alignCenterXState = this.CreateCommandState("AlignCenterX");
			this.alignRightState = this.CreateCommandState("AlignRight");
			this.alignTopState = this.CreateCommandState("AlignTop");
			this.alignCenterYState = this.CreateCommandState("AlignCenterY");
			this.alignBottomState = this.CreateCommandState("AlignBottom");
			this.alignBaseLineState = this.CreateCommandState("AlignBaseLine");
			this.adjustWidthState = this.CreateCommandState("AdjustWidth");
			this.adjustHeightState = this.CreateCommandState("AdjustHeight");
			this.alignGridState = this.CreateCommandState("AlignGrid");

			this.moveLeftState = this.CreateCommandState("MoveLeft");
			this.moveRightState = this.CreateCommandState("MoveRight");
			this.moveDownState = this.CreateCommandState("MoveDown");
			this.moveUpState = this.CreateCommandState("MoveUp");

			this.panelOrderUpAllState = this.CreateCommandState("PanelOrderUpAll");
			this.panelOrderDownAllState = this.CreateCommandState("PanelOrderDownAll");
			this.panelOrderUpOneState = this.CreateCommandState("PanelOrderUpOne");
			this.panelOrderDownOneState = this.CreateCommandState("PanelOrderDownOne");

			this.tabIndexClearState = this.CreateCommandState("TabIndexClear");
			this.tabIndexFirstState = this.CreateCommandState("TabIndexFirst");
			this.tabIndexPrevState = this.CreateCommandState("TabIndexPrev");
			this.tabIndexNextState = this.CreateCommandState("TabIndexNext");
			this.tabIndexLastState = this.CreateCommandState("TabIndexLast");
			this.tabIndexRenumState = this.CreateCommandState("TabIndexRenum");

			this.undoState = this.CreateCommandState("Undo", KeyCode.AlphaZ|KeyCode.ModifierControl);
			this.redoState = this.CreateCommandState("Redo", KeyCode.AlphaY|KeyCode.ModifierControl);
			this.undoRedoListState = this.CreateCommandState("UndoRedoList");

			this.locatorPrevState = this.CreateCommandState("LocatorPrev", KeyCode.ArrowLeft|KeyCode.ModifierAlt);
			this.locatorNextState = this.CreateCommandState("LocatorNext", KeyCode.ArrowRight|KeyCode.ModifierAlt);

			this.displayHorizontalState = this.CreateCommandState("DisplayHorizontal");
			this.displayVerticalState = this.CreateCommandState("DisplayVertical");
			this.displayFullScreenState = this.CreateCommandState("DisplayFullScreen");
			this.displayWindowState = this.CreateCommandState("DisplayWindow");
			this.displayHorizontalState.ActiveState = ActiveState.Yes;
		}

		private CommandState CreateCommandState(string commandName, params Widgets.Shortcut[] shortcuts)
		{
			//	Crée une nouvelle commande et son command state associé.
			Command command = Command.Get(commandName);

			if (command.IsReadWrite)
			{
				if (shortcuts.Length > 0)
				{
					command.Shortcuts.AddRange(shortcuts);
				}

				string iconName = commandName;
				string description = Res.Strings.GetString("Action."+commandName);
				bool statefull = (commandName == "EditLocked" || 
								  commandName == "FontBold" || 
								  commandName == "FontItalic" ||
								  commandName == "FontUnderline" ||
								  commandName.StartsWith("PanelShow") ||
								  commandName == "DisplayHorizontal" ||
								  commandName == "DisplayVertical" ||
								  commandName == "DisplayFullScreen" ||
								  commandName == "DisplayWindow");

				command.ManuallyDefineCommand(description, Misc.Icon(iconName), null, statefull);
			}

			return this.CommandContext.GetCommandState(command);
		}
		#endregion


		#region DisplayMode & ViewersWindow
		public DisplayMode DisplayModeState
		{
			//	Disposition de l'affichage (horizontal, vertical ou autre).
			get
			{
				return this.displayMode;
			}

			set
			{
				if (this.displayMode != value)
				{
					if (!this.Terminate(true))
					{
						return;
					}

					this.displayMode = value;

					this.displayHorizontalState.ActiveState = (this.displayMode == DisplayMode.Horizontal) ? ActiveState.Yes : ActiveState.No;
					this.displayVerticalState.ActiveState   = (this.displayMode == DisplayMode.Vertical  ) ? ActiveState.Yes : ActiveState.No;
					this.displayFullScreenState.ActiveState = (this.displayMode == DisplayMode.FullScreen) ? ActiveState.Yes : ActiveState.No;
					this.displayWindowState.ActiveState     = (this.displayMode == DisplayMode.Window    ) ? ActiveState.Yes : ActiveState.No;

					if (this.displayMode == DisplayMode.Window)  // mode avec fenêtre supplémentaire ?
					{
						if (this.viewersWindow == null)  // fenêtre pas encore créée ?
						{
							Rectangle bounds = new Rectangle(this.Window.WindowLocation, this.Window.WindowSize);
							bounds = new Rectangle(bounds.Center, bounds.Center);
							bounds.Inflate(640/2, 480/2);  // fenêtre initiale de 640x480

							this.viewersWindow = new Window();
							this.viewersWindow.Icon = this.icon;
							this.viewersWindow.MakeSecondaryWindow();
							this.viewersWindow.Root.BackColor = Color.FromBrightness(1);  // fond blanc
							this.viewersWindow.WindowBounds = bounds;
							this.viewersWindow.Owner = this.Window;
							this.viewersWindow.Root.MinSize = new Size(400, 300);
							this.viewersWindow.Root.WindowStyles = WindowStyles.HasCloseButton | WindowStyles.CanResize | WindowStyles.CanMaximize;
							this.viewersWindow.PreventAutoClose = true;
							this.viewersWindow.WindowCloseClicked += this.HandleViewersWindowCloseClicked;
						}
					}
					else  // mode normal (sans fenêtre supplémentaire) ?
					{
						if (this.viewersWindow != null)
						{
							this.viewersWindow.Hide();  // si la fenêtre supplémentaire existe, ferme-la sans la supprimer
						}
					}

					this.UpdateAfterTypeChanged();
				}
			}
		}

		private void HandleViewersWindowCloseClicked(object sender)
		{
			//	Le bouton de fermeture de la fenêtre supplémentaire a été cliqué.
			//	Il faut juste cacher la fenêtre sans la supprimer et remettre le mode DisplayMode.Horizontal.
			this.viewersWindow.Hide();  // cache la fenêtre supplémentaire
			this.DisplayModeState = DisplayMode.Horizontal;
		}

		public Window ViewersWindow
		{
			//	Retourne la fenêtre à utiliser pour étendre l'interface en mode DisplayMode.Window.
			//	Si le mode n'est pas DisplayMode.Window, retourne null.
			get
			{
				if (this.displayMode == DisplayMode.Window)
				{
					return this.viewersWindow;
				}
				else
				{
					return null;
				}
			}
		}

		public void ViewersWindowUpdate(string title, bool useWindow)
		{
			//	Met à jour le titre de la fenêtre supplémentaire et détermine si elle est visible.
			this.viewersWindow.Text = title;

			if (useWindow)
			{
				this.viewersWindow.Show();  // montre la fenêtre supplémentaire
			}
			else
			{
				this.viewersWindow.Hide();  // cache la fenêtre supplémentaire
			}
		}

		private void ViewersWindowClear()
		{
			//	Supprime tous les widgets contenus dans la fenêtre supplémentaire.
			if (this.viewersWindow != null)
			{
				this.viewersWindow.Root.Children.Clear();  // vide tout le contenu
				this.viewersWindow.Text = "";  // plus de titre
			}
		}
		#endregion


		#region InitialMessage
		private void UpdateInitialMessage()
		{
			//	Met à jour la commande du message initial pour l'ensemble des modules ouverts.
			this.initialMessageState.Enable = this.IsInitialMessageExisting;
		}

		private void ShowInitialMessage()
		{
			//	Affiche le message initial pour l'ensemble des modules ouverts.
			if (this.initialMessageState.ActiveState == ActiveState.No)
			{
				if (!this.IsInitialMessageExisting)
				{
					return;
				}

				this.initialMessageState.ActiveState = ActiveState.Yes;

				System.Text.StringBuilder builder = new System.Text.StringBuilder();

				builder.Append("<font size=\"120%\">");
				builder.Append(Res.Strings.Error.CreateMissingValueItems.Title);
				builder.Append("</font><br/>");

				List<Module> modules = this.OpeningListModule;
				foreach (Module module in modules)
				{
					if (module.InitialMessage != null)  // message initial existe ?
					{
						builder.Append(module.InitialMessage);
					}
				}

				builder.Append(" ");  // à cause d'un bug dans TextLayout !

				this.dlgInitialMessage.Initialise(builder.ToString());
				this.dlgInitialMessage.Show();  // affiche le message initial
			}
			else
			{
				this.CloseInitialMessage();
			}
		}

		private void CloseInitialMessage()
		{
			//	Ferme le dialogue du message initial, si nécessaire.
			if (this.initialMessageState.ActiveState == ActiveState.Yes)
			{
				this.initialMessageState.ActiveState = ActiveState.No;
				this.dlgInitialMessage.Hide();
			}
		}

		private bool IsInitialMessageExisting
		{
			//	Indique s'il existe un message initial pour l'ensemble des modules ouverts.
			get
			{
				List<Module> modules = this.OpeningListModule;
				foreach (Module module in modules)
				{
					if (module.InitialMessage != null)  // message initial existe ?
					{
						return true;
					}
				}

				return false;
			}
		}
		#endregion


		#region Navigate
		public bool NavigateToString(Druid stringId)
		{
			//	Sélectionne la string correspondante dans la liste du bon module.
			Module module = this.SearchModule(stringId);
			if (module == null)
			{
				return false;
			}

			Viewers.Locator locator = new Viewers.Locator(module.ModuleId.Name, ResourceAccess.Type.Strings, -1, stringId, null, -1);
			this.LocatorGoto(locator);
			return true;
		}

		public bool NavigateToCaption(Druid captionId)
		{
			//	Sélectionne la ressource correspondante dans la liste du bon module.
			Module module = this.SearchModule(captionId);
			if (module == null)
			{
				return false;
			}

			ResourceAccess.Type type = module.GetCaptionType(captionId);
			if (type == ResourceAccess.Type.Unknown)
			{
				return false;
			}

			Viewers.Locator locator = new Viewers.Locator(module.ModuleId.Name, type, -1, captionId, null, -1);
			this.LocatorGoto(locator);
			return true;
		}

		public bool NavigateToEntityField(Druid entityId, Druid fieldId)
		{
			//	Sélectionne l'entité correspondante dans la liste du bon module puis édite l'expression d'un champ.
			Module module = this.SearchModule(entityId);
			if (module == null)
			{
				return false;
			}

			Viewers.Locator locator = new Viewers.Locator(module.ModuleId.Name, ResourceAccess.Type.Entities, -1, entityId, null, -1);
			this.LocatorGoto(locator);

			Viewers.Entities viewer = this.CurrentModuleInfo.Module.Modifier.ActiveViewer as Viewers.Entities;
			if (viewer == null)
			{
				return false;
			}

			return viewer.EditExpression(fieldId);
		}
		#endregion


		#region UndoRedo
		private void Undo()
		{
			this.CurrentModuleInfo.Module.Modifier.ActiveViewer.Undo();
		}

		private void Redo()
		{
			this.CurrentModuleInfo.Module.Modifier.ActiveViewer.Redo();
		}

		public VMenu UndoRedoCreateMenu(Support.EventHandler<MessageEventArgs> message)
		{
			return this.CurrentModuleInfo.Module.Modifier.ActiveViewer.UndoRedoCreateMenu(message);
		}

		private void UndoRedoMenuGoto(int index)
		{
			this.CurrentModuleInfo.Module.Modifier.ActiveViewer.UndoRedoGoto(index);
		}
		#endregion


		#region Locator
		private void LocatorInit()
		{
			//	Initialise la liste des localisations.
			this.locators = new List<Viewers.Locator>();
			this.locatorIndex = -1;
			this.locatorIgnore = false;
		}

		public void LocatorFix()
		{
			//	Fixe une vue que l'on vient d'atteindre. Si on est déjà exactement sur cette même
			//	localisation, on ne mémorise rien. Ceci est primordial, car on appelle LocatorFix
			//	lorsqu'on retourne à une position localisation mémorisée, et il ne faudrait surtout
			//	pas modifier alors la liste des localisations !
			System.Diagnostics.Debug.Assert(this.locators != null);
			if (this.locatorIgnore)
			{
				return;
			}

			ModuleInfo mi = this.CurrentModuleInfo;
			if (mi == null)
			{
				return;
			}

			string moduleName = mi.Module.ModuleId.Name;
			ResourceAccess.Type viewerType = mi.BundleTypeWidget.CurrentType;

			int subView = -1;
			Viewers.Abstract viewer = mi.Module.Modifier.ActiveViewer;
			if (viewer is Viewers.Entities)
			{
				Viewers.Entities ev = viewer as Viewers.Entities;
				subView = ev.SubView;
			}

			Druid resource = Druid.Empty;
			ResourceAccess access = mi.Module.GetAccess(viewerType);
			int sel = access.AccessIndex;
			if (sel != -1)
			{
				resource = access.AccessDruid(sel);
			}

			Widget widgetFocused = this.Window.FocusedWidget;
			int lineSelected = -1;
			this.LocatorAdjustWidgetFocused(ref widgetFocused, ref lineSelected);

			Viewers.Locator locator = new Viewers.Locator(moduleName, viewerType, subView, resource, widgetFocused, lineSelected);

			if (this.locatorIndex >= 0 && this.locatorIndex < this.locators.Count)
			{
				if (locator == this.locators[this.locatorIndex])  // localisation déjà fixée ?
				{
					return;  // si oui, il ne faut surtout rien fixer
				}
			}

			//	Supprime les localisations après la localisation courante.
			while (this.locators.Count-1 > this.locatorIndex)
			{
				this.locators.RemoveAt(this.locators.Count-1);
			}

			this.locators.Add(locator);
			this.locatorIndex++;

			this.LocatorUpdateCommand();
		}

		public VMenu LocatorCreateMenu(Support.EventHandler<MessageEventArgs> message)
		{
			//	Construit le menu des localisations visitées.
			int all = this.locators.Count;
			int total = System.Math.Min(all, 20);
			int start = this.locatorIndex;
			start += total/2;  if ( start > all-1 )  start = all-1;
			start -= total-1;  if ( start < 0     )  start = 0;

			List<MenuItem> list = new List<MenuItem>();

			//	Met éventuellement la première localisation où aller.
			if (start > 0)
			{
				string action = this.LocatorGetNiceText(0);
				this.LocatorCreateMenu(list, message, "ActiveNo", 0, action);

				if (start > 1)
				{
					list.Add(new MenuSeparator());
				}
			}

			//	Met les localisations où aller.
			for (int i=start; i<start+total; i++)
			{
				if (i <= this.locatorIndex)  // prev ?
				{
					string action = this.LocatorGetNiceText(i);
					string icon = "ActiveNo";
					if (i == this.locatorIndex)
					{
						icon = "ActiveCurrent";
						action = Misc.Bold(action);
					}
					this.LocatorCreateMenu(list, message, icon, i, action);
				}
				else	// next ?
				{
					if (i == this.locatorIndex+1)
					{
						list.Add(new MenuSeparator());
					}

					string action = this.LocatorGetNiceText(i);
					action = Misc.Italic(action);
					this.LocatorCreateMenu(list, message, "", i, action);
				}
			}

			//	Met éventuellement la dernière localisation où aller.
			if (start+total < all)
			{
				if (start+total < all-1)
				{
					list.Add(new MenuSeparator());
				}

				string action = this.LocatorGetNiceText(all-1);
				action = Misc.Italic(action);
				this.LocatorCreateMenu(list, message, "", all-1, action);
			}

			//	Génère le menu.
			VMenu menu = new VMenu();
			menu.Items.AddRange (list);
			menu.AdjustSize();
			return menu;
		}

		private void LocatorCreateMenu(List<MenuItem> list, Support.EventHandler<MessageEventArgs> message, string icon, int rank, string action)
		{
			//	Crée une case du menu des localisations.
			if (icon != "")
			{
				icon = Misc.Icon(icon);
			}

			string name = string.Format("{0}: {1}", (rank+1).ToString(), action);
			string cmd = "LocatorListDo";
			Misc.CreateStructuredCommandWithName(cmd);

			MenuItem item = new MenuItem(cmd, icon, name, "", rank.ToString());

			if ( message != null )
			{
				item.Pressed += message;
			}

			list.Add(item);
		}

		private string LocatorGetNiceText(int index)
		{
			//	Retourne le joli texte correspondant à une localisations.
			string moduleName              = this.locators[index].ModuleName;
			ResourceAccess.Type viewerType = this.locators[index].ViewerType;
			int subView                    = this.locators[index].SubView;
			Druid resource                 = this.locators[index].Resource;

			string typeText = ResourceAccess.TypeDisplayName(viewerType);

			string resourceText = resource.ToString();  // affiche le Druid si on ne trouve rien de mieux
			int moduleRank = this.SearchModuleRank(moduleName);
			if (moduleRank != -1)
			{
				ResourceAccess access = this.moduleInfoList[moduleRank].Module.GetAccess(viewerType);
				if (access != null)
				{
					resourceText = access.DirectGetName(resource);
				}
			}

			if (subView == -1)
			{
				return System.String.Format("{0}/{1}: {2}", moduleName, typeText, resourceText);
			}
			else
			{
				return System.String.Format("{0}/{1}/{2}: {3}", moduleName, typeText, Viewers.Entities.SubViewName(subView), resourceText);
			}
		}

		private void LocatorClose(string moduleName)
		{
			//	Suite à la fermeture d'un module, supprime toutes les localisations en rapport avec ce module.
			int i = 0;
			while (i<this.locators.Count)
			{
				if (this.locators[i].ModuleName == moduleName)
				{
					this.locators.RemoveAt(i);
				}
				else
				{
					i++;
				}
			}

			this.locatorIndex = this.locators.Count-1;
			this.LocatorUpdateCommand();
		}

		private bool LocatorPrevIsEnable
		{
			//	Donne l'état de la commande "LocatorPrev".
			get
			{
				System.Diagnostics.Debug.Assert(this.locators != null);
				return this.locatorIndex > 0;
			}
		}

		private bool LocatorNextIsEnable
		{
			//	Donne l'état de la commande "LocatorNext".
			get
			{
				System.Diagnostics.Debug.Assert(this.locators != null);
				return this.locatorIndex < this.locators.Count-1;
			}
		}

		private void LocatorPrev()
		{
			//	Action de la commande "LocatorPrev".
			System.Diagnostics.Debug.Assert(this.LocatorPrevIsEnable);
			if (!this.Terminate())
			{
				return;
			}

			Viewers.Locator locator = this.locators[--this.locatorIndex];
			this.LocatorGoto(locator);
		}

		private void LocatorNext()
		{
			//	Action de la commande "LocatorNext".
			System.Diagnostics.Debug.Assert(this.LocatorNextIsEnable);
			if (!this.Terminate())
			{
				return;
			}

			Viewers.Locator locator = this.locators[++this.locatorIndex];
			this.LocatorGoto(locator);
		}

		private void LocatorMenuGoto(int index)
		{
			//	Revient à une location choisie dans le menu.
			if (!this.Terminate())
			{
				return;
			}

			this.locatorIndex = index;
			Viewers.Locator locator = this.locators[this.locatorIndex];
			this.LocatorGoto(locator);
		}

		public void LocatorGoto(string moduleName, ResourceAccess.Type viewerType, int subView, Druid resource, Widget widgetFocused)
		{
			//	Va sur une ressource d'une vue d'un module quelconque.
			if (!this.Terminate())
			{
				return;
			}

			int lineSelected = -1;
			this.LocatorAdjustWidgetFocused(ref widgetFocused, ref lineSelected);
			Viewers.Locator locator = new Viewers.Locator(moduleName, viewerType, subView, resource, widgetFocused, lineSelected);
			this.LocatorGoto(locator);
		}

		private void LocatorGoto(Viewers.Locator locator)
		{
			//	Va sur une ressource définie par une localisation.
			ModuleInfo mi = this.CurrentModuleInfo;

			if (mi.Module.ModuleId.Name != locator.ModuleName)
			{
				int rank = this.SearchModuleRank(locator.ModuleName);
				if (rank == -1)
				{
					return;
				}

				this.locatorIgnore = true;
				this.UseModule(rank);
				this.locatorIgnore = false;

				this.UpdateAfterTypeChanged();

				mi = this.CurrentModuleInfo;
			}

			this.locatorIgnore = true;
			mi.BundleTypeWidget.CurrentType = locator.ViewerType;
			this.locatorIgnore = false;

			Viewers.Abstract viewer = mi.Module.Modifier.ActiveViewer;
			if (locator.SubView != -1 && viewer is Viewers.Entities)
			{
				Viewers.Entities ev = viewer as Viewers.Entities;
				this.locatorIgnore = true;
				ev.SubView = locator.SubView;
				this.locatorIgnore = false;
			}

			if (!locator.Resource.IsEmpty)
			{
				ResourceAccess access = mi.Module.GetAccess(locator.ViewerType);

				int sel = access.AccessIndexOfDruid(locator.Resource);
				if (sel == -1)
				{
					access.SetFilter("", Searcher.SearchingMode.None);
				}

				sel = access.AccessIndexOfDruid(locator.Resource);
				if (sel != -1)
				{
					access.AccessIndex = sel;

					if (viewer != null)
					{
						this.locatorIgnore = true;
						viewer.Window.ForceLayout();  // StringArray.LineCount nécessite que la hauteur du widget soit juste !
						viewer.Update();
						viewer.ShowSelectedRow();
						this.locatorIgnore = false;
					}
				}
			}

#if false
			//	Remet le widget dans le même état. Cela ne semble pas fonctionner, à cause de problèmes
			//	dans StringArray !
			if (locator.WidgetFocused != null)
			{
				if (locator.WidgetFocused != null && locator.WidgetFocused.Parent is MyWidgets.StringArray)
				{
					MyWidgets.StringArray array = locator.WidgetFocused.Parent as MyWidgets.StringArray;
					array.SelectedRow = locator.LineSelected;
				}

				locator.WidgetFocused.Focus();
			}
#endif

			this.LocatorFix();
			this.LocatorUpdateCommand();
		}

		private void LocatorAdjustWidgetFocused(ref Widget widgetFocused, ref int lineSelected)
		{
			//	Si le focus est dans une StringList, cherche la sélection dans le StringArray parent.
			if (widgetFocused != null && widgetFocused.Parent is MyWidgets.StringArray)
			{
				MyWidgets.StringArray array = widgetFocused.Parent as MyWidgets.StringArray;
				lineSelected = array.SelectedRow;
			}
		}

		private void LocatorUpdateCommand()
		{
			//	Met à jour les commandes du navigateur.
			this.locatorPrevState.Enable = this.LocatorPrevIsEnable;
			this.locatorNextState.Enable = this.LocatorNextIsEnable;
		}
		#endregion


		#region Modules manager
		private void CreateModuleLayout()
		{
			ModuleInfo mi = this.CurrentModuleInfo;

			mi.TabPage = new TabPage ();
			mi.TabPage.TabTitle = Misc.GetModuleName (mi.Module.ModuleId.Name, mi.Module.IsGlobalDirty, mi.Module.IsPatch);

			this.bookModules.Items.Insert (this.currentModule, mi.TabPage);

			//	Bande horizontale pour le nom du module.
			{
				double margin = 3;
				double size = 20;

				mi.ModuleFrame = new FrameBox
				{
					Parent = mi.TabPage,
					Visibility = this.showModuleTitle,
					PreferredHeight = margin+size+margin,
					Dock = DockStyle.Top,
					Padding = new Margins (0, 0, margin, margin),
				};

				var accessButton = new GlyphButton
				{
					Parent = mi.ModuleFrame,
					GlyphShape = GlyphShape.TriangleRight,
					ButtonStyle = ButtonStyle.ToolItem,
					PreferredSize = new Size (size, size),
					Dock = DockStyle.Left,
					Margins = new Margins (0, 6, 0, 0),
				};

				ToolTip.Default.SetToolTip (accessButton, "Accès aux informations du module");

				accessButton.Clicked += delegate
				{
					if (!this.Terminate ())
					{
						return;
					}

					this.CurrentModule.Info ();
				};

				mi.ModuleTitle = new StaticText
				{
					Parent = mi.ModuleFrame,
					Text = Misc.GetModuleDescription (this, mi.Module),
					PreferredHeight = 20,
					ContentAlignment = ContentAlignment.MiddleLeft,
					Dock = DockStyle.Fill,
				};
			}

			//	Bande horizontale pour les boutons d'accès aux différents types de ressources.
			{
				mi.BundleTypeWidget = new MyWidgets.BundleType
				{
					Parent = mi.TabPage,
					Dock = DockStyle.Top,
				};
			}

			//	Bouton pour monter/cacher le titre du module.
			{
				mi.ShowHideButton = new GlyphButton
				{
					Parent = mi.TabPage,
					GlyphShape = this.showModuleTitle ? GlyphShape.TriangleUp : GlyphShape.TriangleDown,
					ButtonStyle = Widgets.ButtonStyle.Slider,
					PreferredSize = new Size (19, 19),
					Anchor = AnchorStyles.TopRight,
					Margins = new Margins (0, 3, 3, 0),
				};

				ToolTip.Default.SetToolTip (mi.ShowHideButton, "Montre ou cache le titre du module");

				mi.ShowHideButton.Clicked += delegate
				{
					this.showModuleTitle = !this.showModuleTitle;
					this.UpdateModuleTitle ();
				};
			}

			mi.BundleTypeWidget.TypeChanged += new EventHandler<CancelEventArgs> (this.HandleTypeChanged);

			this.CreateViewerLayout ();
		}

		private void CreateViewerLayout()
		{
			ModuleInfo mi = this.CurrentModuleInfo;

			Viewers.Abstract actual = mi.Module.Modifier.ActiveViewer;
			if (actual != null)
			{
				mi.Module.Modifier.DetachViewer(actual);
				mi.TabPage.Children.Remove(actual);
				mi.Module.Modifier.ActiveViewer = null;
			}

			ResourceAccess.Type type = mi.BundleTypeWidget.CurrentType;
			Viewers.Abstract viewer = Viewers.Abstract.Create(type, mi.Module, this.context, mi.Module.GetAccess(type), this);

			if (viewer != null)
			{
				viewer.SetParent(mi.TabPage);
				viewer.Dock = DockStyle.Fill;
				mi.Module.Modifier.AttachViewer(viewer);
				mi.Module.Modifier.ActiveViewer = viewer;

				//	Montre la ressource sélectionnée. Il faut le faire très tard, lorsque le tableau UI.ItemTable
				//	est correctement dimensionné.
				viewer.Window.ForceLayout();
				viewer.ShowSelectedRow();
			}
		}

		private void UpdateModuleTitle()
		{
			ModuleInfo mi = this.CurrentModuleInfo;

			mi.ShowHideButton.GlyphShape = this.showModuleTitle ? GlyphShape.TriangleUp : GlyphShape.TriangleDown;
			mi.ModuleFrame.Visibility = this.showModuleTitle;
		}

		public bool Terminate()
		{
			//	Termine le travail sur une ressource, avant de passer à une autre.
			//	Retourne false si l'utilisateur a choisi "annuler".
			return this.Terminate(false);
		}

		public bool Terminate(bool soft)
		{
			//	Termine le travail sur une ressource, avant de passer à une autre.
			//	Si soft = true, on sérialise temporairement sans poser de question.
			//	Retourne false si l'utilisateur a choisi "annuler".
			if (this.IsCurrentModule && this.CurrentModule.Modifier.ActiveViewer != null)
			{
				return this.CurrentModule.Modifier.ActiveViewer.Terminate(soft);
			}

			return true;
		}

		private void UpdateAfterTypeChanged()
		{
			//	Mise à jour après avoir changé le type de ressource.
			this.ViewersWindowClear();

			if (this.IsCurrentModule)
			{
				this.CreateViewerLayout();
				this.DialogSearchAdapt();
				this.LocatorFix();
				this.CurrentModule.Modifier.ActiveViewer.UpdateCommands();
			}
		}

		private void HandleTypeChanged(object sender, CancelEventArgs e)
		{
			//	Appelé lorsque le type de vue a changé.
			if (!this.Terminate())
			{
				e.Cancel = true;  // revient à la sélection précédente
				return;
			}

			this.UpdateAfterTypeChanged();
		}

		private void HandleBookModulesActivePageChanged(object sender, CancelEventArgs e)
		{
			//	L'onglet pour le module courant a été cliqué.
			if (this.ignoreChange)
			{
				return;
			}

			if (!this.Terminate())
			{
				e.Cancel = true;  // revient à la sélection précédente
				return;
			}

			int total = this.bookModules.PageCount;
			for (int i=0; i<total; i++)
			{
				ModuleInfo mi = this.moduleInfoList[i];
				if (mi.TabPage == this.bookModules.ActivePage)
				{
					this.UseModule(i);
					this.UpdateAfterTypeChanged();
					return;
				}
			}
		}

		public bool IsCurrentModule
		{
			//	Indique s'il existe un module courant.
			get
			{
				return (this.currentModule >= 0);
			}
		}

		private ModuleInfo CurrentModuleInfo
		{
			//	Retourne le ModuleInfo courant.
			get
			{
				if (this.currentModule < 0)
				{
					return null;
				}

				return this.moduleInfoList[this.currentModule];
			}
		}

		public List<Module> OpeningListModule
		{
			//	Retourne la liste des modules ouverts.
			get
			{
				List<Module> list = new List<Module>();

				foreach (ModuleInfo info in this.moduleInfoList)
				{
					list.Add(info.Module);
				}

				return list;
			}
		}

		public Module CurrentModule
		{
			//	Retourne le module courant.
			get
			{
				if (this.currentModule < 0)
				{
					return null;
				}

				return this.CurrentModuleInfo.Module;
			}
		}

		private Module LastModule
		{
			//	Retourne le module précédemment sélectionné.
			get
			{
				if (this.lastModule < 0 || this.lastModule >= this.moduleInfoList.Count)
				{
					return null;
				}

				if (this.lastModule == this.currentModule)
				{
					return null;
				}

				return this.moduleInfoList[this.lastModule].Module;
			}
		}

		public Module SearchModule(string name)
		{
			//	Cherche un module d'après son nom.
			foreach (ModuleInfo info in this.moduleInfoList)
			{
				if (name == info.Module.ModuleId.Name)
				{
					return info.Module;
				}
			}

			return null;
		}

		public Module SearchModule(Druid druid)
		{
			//	Cherche à quel module appartient un druid.
			if (druid.IsEmpty)
			{
				return null;
			}

			return this.SearchModuleId(druid.Module);
		}

		private Module SearchModuleId(int id)
		{
			//	Cherche un module d'après son identificateur.
			foreach (ModuleInfo info in this.moduleInfoList)
			{
				if (info.Module.ModuleId.Id == id)
				{
					return info.Module;
				}
			}

			return this.OpenModule (id);
		}

		public Module SearchModuleId(ResourceModuleId id)
		{
			//	Cherche un module d'après son identificateur.
			foreach (ModuleInfo info in this.moduleInfoList)
			{
				if (info.Module.ModuleId.Equals(id))
				{
					return info.Module;
				}
			}
			
			return null;
		}

		public List<Module> Modules
		{
			//	Retourne la liste de tous les modules.
			get
			{
				List<Module> list = new List<Module>();
				
				foreach (ModuleInfo info in this.moduleInfoList)
				{
					list.Add(info.Module);
				}
				
				return list;
			}
		}

		private int SearchModuleRank(string moduleName)
		{
			//	Cherche le rang d'un module d'après son nom.
			for (int i=0; i<this.moduleInfoList.Count; i++)
			{
				ModuleInfo mi = this.moduleInfoList[i];
				if (mi.Module.ModuleId.Name == moduleName)
				{
					return i;
				}
			}

			return -1;
		}

		private void UseModule(int rank)
		{
			//	Utilise un module ouvert.
			if (this.ignoreChange)
			{
				return;
			}

			this.lastModule = this.currentModule;
			this.currentModule = rank;

			this.UpdateModuleTitle ();
			this.UpdateCommandEditLocked();

			if ( rank >= 0 )
			{
				this.ignoreChange = true;
				
				//	Force le défocus du widget courant, qui peut être une ligne éditable
				//	TextFieldEx avec DefocusAction et remet le focus sur le 1er widget du
				//	nouvel onglet :
				
				this.Window.ClearFocusedWidget();
				this.bookModules.ActivePage = this.CurrentModuleInfo.TabPage;
				this.bookModules.ActivePage.SetFocusOnTabWidget();
				
				this.ignoreChange = false;

				this.DialogSearchAdapt();
				this.CurrentModule.Modifier.ActiveViewer.UpdateWhenModuleUsed();

				this.recycleState.Enable = true;
				this.checkState.Enable = true;
			}
			else
			{
				this.recycleState.Enable = false;
				this.saveState.Enable = false;
				this.saveAsState.Enable = false;
				this.checkState.Enable = false;
				this.cutState.Enable = false;
				this.copyState.Enable = false;
				this.pasteState.Enable = false;
				this.newCultureState.Enable = false;
				this.deleteCultureState.Enable = false;
				this.filterState.Enable = false;
				this.searchState.Enable = false;
				this.searchPrevState.Enable = false;
				this.searchNextState.Enable = false;
				this.accessFirstState.Enable = false;
				this.accessLastState.Enable = false;
				this.accessPrevState.Enable = false;
				this.accessNextState.Enable = false;
				this.modificationPrevState.Enable = false;
				this.modificationNextState.Enable = false;
				this.modificationAllState.Enable = false;
				this.modificationClearState.Enable = false;
				this.editOkState.Enable = false;
				this.editCancelState.Enable = false;
				this.deleteState.Enable = false;
				this.createState.Enable = false;
				this.duplicateState.Enable = false;
				this.copyToModuleState.Enable = false;
				this.fontBoldState.Enable = false;
				this.fontItalicState.Enable = false;
				this.fontUnderlineState.Enable = false;
				this.glyphsState.Enable = false;
				this.displayHorizontalState.Enable = false;
				this.displayVerticalState.Enable = false;
				this.displayFullScreenState.Enable = false;
				this.displayWindowState.Enable = false;

				this.UpdateInfoCurrentModule();
				this.UpdateInfoAccess();
				this.UpdateInfoViewer();
			}

			this.LocatorFix();
			this.LocatorUpdateCommand();
		}

		private void CloseModule()
		{
			//	Ferme le module courant.
			int rank = this.currentModule;
			if ( rank < 0 )  return;

			ModuleInfo mi = this.CurrentModuleInfo;
			this.currentModule = -1;
			this.moduleInfoList.RemoveAt(rank);
			this.ignoreChange = true;
			this.bookModules.Items.RemoveAt(rank);
			this.ignoreChange = false;
			this.LocatorClose(mi.Module.ModuleId.Name);
			mi.Module.Dispose();
			mi.Dispose();

			if ( rank >= this.bookModules.PageCount )
			{
				rank = this.bookModules.PageCount-1;
			}
			this.UseModule(rank);
			//?this.UpdateCloseCommand();

			if ( this.CurrentModule == null )
			{
				this.ribbonActive = this.ribbonMain;
				this.ribbonBook.ActivePage = this.ribbonMain;
			}

			this.UpdateInitialMessage();
		}


		private Module OpenModule(int id)
		{
			//	Ouvre automatiquement un module.
#if false
			List<ResourceModuleInfo> list = Collection.ToList (this.ResourceManagerPool.Modules);

			foreach (var moduleInfo in list)
			{
				if (moduleInfo.FullId.Id == id)
				{
					var result = DesignerApplication.IsOriginalModule (moduleInfo.FullId);

					if (result == -1)
					{
						continue;
					}

					if (result == 1)
					{
						int c = this.currentModule;
						var mi = this.OpenModule (moduleInfo.FullId);
						this.currentModule = c;

						return mi.Module;
					}
				}
			}
#endif

			return null;
		}

		public static int IsOriginalModule(ResourceModuleId moduleInfo)
		{
			//	Méthode "magique" pour déterminer si un module est l'original !
			//	Retourne -1 -> module à rejeter toujours
			//	Retourne  0 -> module secondaire
			//	Retourne  1 -> module original
			var path = moduleInfo.Path.Replace ('\\', '/');
			var name = moduleInfo.Name;

			if (path.Contains ("/bin/"))
			{
				return -1;  // module rejeté
			}

			var paths = path.Split ('/');
			var names = name.Split ('.');

			for (int i=0; i<paths.Length-1; i++)
			{
				if (paths[i] == name)
				{
					return 1;  // module original
				}
			}

			for (int i=0; i<paths.Length-1; i++)
			{
				foreach (var n in names)
				{
					if (paths[i] == n)
					{
						return 1;  // module original
					}
				}
			}

			return 0;  // module secondaire
		}

		private ModuleInfo OpenModule(ResourceModuleId item)
		{
			Module module = new Module (this, this.mode, item, this.MissingModuleResolver);

			ModuleInfo mi = new ModuleInfo ();
			mi.Module = module;
			this.moduleInfoList.Insert (++this.currentModule, mi);
			this.CreateModuleLayout ();

			return mi;
		}

		private ResourceManager MissingModuleResolver(int id)
		{
			Module module = this.OpenModule (id);

			if (module != null)
			{
				return module.ResourceManager;
			}

			return null;
		}


		public void UpdateCommandEditLocked()
		{
			//	Met à jour la commande "EditLocked".
			this.editLockedState.Enable = (this.CurrentModule != null && this.settings.IdentityCard != null);
			this.StyleButton("EditLocked", this.IsReadonly ? null : "Unlock");  // ouvre ou ferme le cadenas
		}

		private bool AutoSave(CommandDispatcher dispatcher)
		{
			//	Fait tout ce qu'il faut pour éventuellement sauvegarder les ressources
			//	avant de passer à autre chose.
			//	Retourne false si on ne peut pas continuer.
			var result = this.DialogSave(dispatcher);
			if (result == Common.Dialogs.DialogResult.Yes)
			{
				this.CurrentModule.Save();
				return true;
			}
			if (result == Common.Dialogs.DialogResult.Cancel)
			{
				return false;
			}
			return true;
		}

		private bool AutoSaveAll(CommandDispatcher dispatcher)
		{
			//	Fait tout ce qu'il faut pour éventuellement sauvegarder toutes les
			//	ressources avant de passer à autre chose.
			//	Retourne false si on ne peut pas continuer.
			int cm = this.currentModule;

			int total = this.bookModules.PageCount;
			for ( int i=0 ; i<total ; i++ )
			{
				this.currentModule = i;
				if (!this.AutoSave(dispatcher))
				{
					this.currentModule = cm;
					return false;
				}
			}

			this.currentModule = cm;
			return true;
		}

		private void ReadSettings()
		{
			//	Reprend tous les réglages globaux.
			if (!this.settings.Read() || this.settings.Modules.Count == 0)
			{
				this.UseModule(-1);  // grise toutes les commandes puisqu'aucun module n'est ouvert
			}
			else
			{
				foreach (ResourceModuleId id in this.settings.Modules)
				{
					Module module = new Module (this, this.mode, id, this.MissingModuleResolver);

					ModuleInfo mi = new ModuleInfo();
					mi.Module = module;
					this.moduleInfoList.Insert(++this.currentModule, mi);
					this.CreateModuleLayout();

					this.bookModules.ActivePage = mi.TabPage;
				}
			}
		}

		private bool WriteSettings()
		{
			//	Sauve tous les réglages globaux.
			this.settings.Modules.Clear();

			foreach (ModuleInfo info in this.moduleInfoList)
			{
				this.settings.Modules.Add(info.Module.ModuleId);
			}
			
			return this.settings.Write();  // enregistre les réglages globaux

		}

		public bool IsReadonly
		{
			//	Indique si Designer est en mode "consultation", lorsque l'identificateur est anonyme
			//	ou lorsqu'on est en mode "bloqué".
			get
			{
				return this.settings.IdentityCard == null || this.IsEditLocked;
			}
		}

		private bool IsEditLocked
		{
			get
			{
				ModuleInfo mi = this.CurrentModuleInfo;
				if (mi == null)
				{
					return true;
				}
				else
				{
					return mi.Module.IsEditLocked;
				}
			}
			set
			{
				if (this.IsEditLocked != value)
				{
					ModuleInfo mi = this.CurrentModuleInfo;
					mi.Module.IsEditLocked = value;

					this.UpdateCommandEditLocked();

					if (mi != null)
					{
						Viewers.Abstract viewer = mi.Module.Modifier.ActiveViewer;
						viewer.Update();
					}
				}
			}
		}

		public void UpdateBookModules()
		{
			//	Met à jour les noms des l'onglets des modules.
			//	Il faut passer en revue tous les modules, car on peut très bien avoir rendu 'dirty' un autre
			//	module que le module courant.
			bool changed = false;

			foreach (var mi in this.moduleInfoList)
			{
				TabPage tab = mi.TabPage;
				string name = Misc.GetModuleName (mi.Module.ModuleId.Name, mi.Module.IsGlobalDirty, mi.Module.IsPatch);

				if (tab.TabTitle != name)
				{
					tab.TabTitle = name;
					changed = true;
				}

				mi.ModuleTitle.Text = Misc.GetModuleDescription (this, mi.Module);
			}

			if (changed)
			{
				this.bookModules.UpdateAfterChanges ();
			}
		}
		#endregion


		#region Dialogs
		public bool DlgBindingSelector(Module baseModule, StructuredType type, PanelEditor.ObjectModifier.ObjectType objectType, ref Binding binding)
		{
			//	Ouvre le dialogue pour choisir une rubrique dans une structure de données.
			this.dlgBindingSelector.Initialise(baseModule, type, objectType, binding);

			this.dlgBindingSelector.Show();  // choix dans le dialogue...
			
			binding = this.dlgBindingSelector.SelectedBinding;
			
			return this.dlgBindingSelector.IsOk;
		}

		public Common.Dialogs.DialogResult DlgResourceSelector(Dialogs.ResourceSelectorDialog.Operation operation, Module baseModule, ResourceAccess.Type type, ref StructuredTypeClass typeClass, ref Druid resource, ref bool isNullable, List<Druid> exclude, Druid typeId)
		{
			//	Ouvre le dialogue pour choisir une ressource (sous forme d'un Druid) d'un type à choix.
			this.dlgResourceSelector.AccessOpen(operation, baseModule, type, resource, isNullable, exclude, typeId);
			this.dlgResourceSelector.StructuredTypeClass = typeClass;

			this.dlgResourceSelector.Show();  // choix dans le dialogue...
			
			typeClass = this.dlgResourceSelector.StructuredTypeClass;
			resource = this.dlgResourceSelector.SelectedResource;
			isNullable = this.dlgResourceSelector.IsNullable;
			
			return this.dlgResourceSelector.AccessClose();
		}

		public string DlgIcon(ResourceManager manager, string icon)
		{
			//	Ouvre le dialogue pour choisir une icône.
			ModuleInfo mi = this.CurrentModuleInfo;
			this.dlgIcon.SetResourceManager(manager, mi.Module.ModuleId.Name);
			this.dlgIcon.IconValue = icon;

			this.dlgIcon.Show();
			
			return this.dlgIcon.IconValue;
		}

		public string DlgNewCulture(ResourceAccess access)
		{
			//	Ouvre le dialogue pour choisir la culture à créer.
			this.dlgNewCulture.SetAccess(access);
			this.dlgNewCulture.Show();
			return this.dlgNewCulture.Culture;
		}

		public void DlgResourceTypeCode(ResourceAccess access, ref TypeCode type, out System.Type stype)
		{
			//	Ouvre le dialogue pour choisir le type d'un Caption.Type.
			this.dlgResourceTypeCode.ResourceAccess = access;
			this.dlgResourceTypeCode.ContentType = type;

			this.dlgResourceTypeCode.Show();
			
			type = this.dlgResourceTypeCode.ContentType;
			stype = this.dlgResourceTypeCode.SystemType;
		}

		public string DlgResourceName(Dialogs.ResourceNameDialog.Operation operation, Dialogs.ResourceNameDialog.Type type, string name)
		{
			//	Ouvre le dialogue pour choisir le nom d'un champ.
			this.dlgFieldName.Initialise(operation, type, name);

			this.dlgFieldName.Show();  // choix dans le dialogue...
			
			return this.dlgFieldName.SelectedName;
		}

		public Common.Dialogs.DialogResult DlgEntityField(Module baseModule, ResourceAccess.Type type, string prefix, ref string fieldName, ref Druid resource, ref bool isNullable, ref bool isCollection, ref bool isPrivate, ref bool isIndexAscending, ref bool isIndexDescending)
		{
			//	Ouvre le dialogue pour choisir les paramètres d'un champ d'une entité.
			this.dlgEntityField.AccessOpen(baseModule, type, prefix, fieldName, resource, isNullable, isCollection, isPrivate, isIndexAscending, isIndexDescending);

			this.dlgEntityField.Show();  // choix dans le dialogue...
			
			fieldName = this.dlgEntityField.FieldName;
			isNullable = this.dlgEntityField.IsNullable;
			isCollection = this.dlgEntityField.IsCollection;
			isPrivate = this.dlgEntityField.IsPrivate;
			isIndexAscending = this.dlgEntityField.IsIndexAscending;
			isIndexDescending = this.dlgEntityField.IsIndexDescending;
			
			return this.dlgEntityField.AccessClose(out resource);
		}

		public Common.Dialogs.DialogResult DlgLabelReplacement(string nameToCreate, ref Druid resource)
		{
			//	Ouvre le dialogue pour choisir le Caption de remplacement d'une champ dans un Form.
			this.dlgLabelReplacement.AccessOpen(nameToCreate, resource);
			this.dlgLabelReplacement.Show();  // choix dans le dialogue...
			return this.dlgLabelReplacement.AccessClose(out resource);
		}

		public string DlgEntityComment(string text)
		{
			//	Ouvre le dialogue pour choisir le texte d'un commentaire.
			this.dlgEntityComment.Initialise (text);
			this.dlgEntityComment.Show ();  // choix dans le dialogue...
			return this.dlgEntityComment.SelectedText;
		}

		public Common.Dialogs.DialogResult DlgEntityParameters(EntitiesEditor.ObjectBox objectBox, ref DataLifetimeExpectancy lifetime, ref StructuredTypeFlags flags)
		{
			//	Ouvre le dialogue pour choisir les paramètres d'une entité.
			this.dlgEntityParameters.ObjectBox              = objectBox;
			this.dlgEntityParameters.DataLifetimeExpectancy = lifetime;
			this.dlgEntityParameters.StructuredTypeFlags    = flags;

			this.dlgEntityParameters.Show ();  // choix dans le dialogue...

			if (this.dlgEntityParameters.IsEditOk)
			{
				lifetime = this.dlgEntityParameters.DataLifetimeExpectancy;
				flags    = this.dlgEntityParameters.StructuredTypeFlags;

				return Common.Dialogs.DialogResult.Accept;
			}
			else
			{
				return Common.Dialogs.DialogResult.Cancel;
			}
		}

		public Common.Dialogs.DialogResult DlgEntityCreation(Module module, ref string name, ref StructuredTypeClass typeClass, ref Druid resource, ref DataLifetimeExpectancy lifetime, ref StructuredTypeFlags flags)
		{
			//	Ouvre le dialogue (en 3 volets) pour créer une entité.
			this.dlgEntityCreation.ResourceName.Initialise (Dialogs.ResourceNameDialog.Operation.Create, Dialogs.ResourceNameDialog.Type.Entity, name);

			this.dlgEntityCreation.ResourceSelector.AccessOpen (Dialogs.ResourceSelectorDialog.Operation.InheritEntities, module, Common.Designer.ResourceAccess.Type.Entities, resource, false, null, Druid.Empty);
			this.dlgEntityCreation.ResourceSelector.StructuredTypeClass = typeClass;

			this.dlgEntityCreation.EntityParameters.DataLifetimeExpectancy = lifetime;
			this.dlgEntityCreation.EntityParameters.StructuredTypeFlags    = flags;

			this.dlgEntityCreation.Show ();  // choix dans le dialogue...

			if (this.dlgEntityCreation.IsEditOk)
			{
				name = this.dlgEntityCreation.ResourceName.SelectedName;

				typeClass  = this.dlgEntityCreation.ResourceSelector.StructuredTypeClass;
				resource   = this.dlgEntityCreation.ResourceSelector.SelectedResource;

				lifetime = this.dlgEntityCreation.EntityParameters.DataLifetimeExpectancy;
				flags    = this.dlgEntityCreation.EntityParameters.StructuredTypeFlags;

				return Common.Dialogs.DialogResult.Accept;
			}
			else
			{
				return Common.Dialogs.DialogResult.Cancel;
			}
		}

		public bool DlgEntityExpression(bool isInterface, bool isPatchModule, string deepExpression, ref string expression)
		{
			//	Ouvre le dialogue pour éditer une expression.
			this.dlgEntityExpression.Initialise(this.IsReadonly, isInterface, isPatchModule, deepExpression, expression);
			
			this.dlgEntityExpression.Show();  // choix dans le dialogue...
			
			expression = this.dlgEntityExpression.Expression;
			
			return this.dlgEntityExpression.IsEditOk;
		}

		public bool DlgModuleInfo(Module module)
		{
			//	Ouvre le dialogue pour éditer les informations d'un module.
			this.dlgModuleInfo.Module = module;
			
			this.dlgModuleInfo.Show ();
			
			return this.dlgModuleInfo.IsEditOk;
		}

		public System.Windows.Forms.DialogResult DlgSaveAllImages(List<EntitiesEditor.EntitySample> entitySamples, List<string> selectedEntityNames, ref string folder, ref string extension, ref EntitiesEditor.BitmapParameters bitmapParameters)
		{
			//	Ouvre le dialogue pour éditer les informations d'un module.
			this.dlgSaveAllImages.EntitySamples.Clear ();
			this.dlgSaveAllImages.EntitySamples.AddRange (entitySamples);

			this.dlgSaveAllImages.SelectedEntityNames.Clear ();
			this.dlgSaveAllImages.SelectedEntityNames.AddRange (selectedEntityNames);

			this.dlgSaveAllImages.Folder = folder;
			this.dlgSaveAllImages.Extension = extension;
			this.dlgSaveAllImages.BitmapParameters = bitmapParameters;

			this.dlgSaveAllImages.Show ();

			folder = this.dlgSaveAllImages.Folder;
			extension = this.dlgSaveAllImages.Extension;
			bitmapParameters = this.dlgSaveAllImages.BitmapParameters;

			selectedEntityNames.Clear ();
			selectedEntityNames.AddRange (this.dlgSaveAllImages.SelectedEntityNames);
			
			return this.dlgSaveAllImages.Result;
		}

		public Dialogs.SearchDialog DialogSearch
		{
			get
			{
				return this.dlgSearch;
			}
		}

		private void DialogSearchAdapt()
		{
			//	Adapte le dialogue de recherche en fonction du type du viewer actif.
			ModuleInfo mi = this.CurrentModuleInfo;
			if (mi == null)
			{
				this.dlgSearch.Adapt(ResourceAccess.Type.Unknown);
			}
			else
			{
				this.dlgSearch.Adapt(mi.Module.Modifier.ActiveResourceType);
			}
		}

		private Common.Dialogs.DialogResult DialogSave(CommandDispatcher dispatcher)
		{
			//	Affiche le dialogue pour demander s'il faut enregistrer les
			//	ressources modifiées, avant de passer à d'autres ressources.
			if (this.CurrentModule == null || !this.CurrentModule.IsGlobalDirty)
			{
				return Common.Dialogs.DialogResult.None;
			}

			string title = Res.Strings.Application.Title;
			string icon = "manifest:Epsitec.Common.Dialogs.Images.Question.icon";

			string question1 = string.Format(Res.Strings.Dialog.Save.Question1, this.CurrentModule.ModuleId.Name);
			string question2 = Res.Strings.Dialog.Save.Question2;
			string warning = this.CurrentModule.CheckMessage();
			string message;

			if (string.IsNullOrEmpty(warning))
			{
				message = string.Format("{0}<br/>{1}", question1, question2);
			}
			else
			{
				string color = Color.ToHexa(Misc.WarningColor);
				string line = "------------------------------";
				message = string.Format("{0}<br/><br/><font color=\"#{3}\">{2}<br/>{4}{2}</font><br/><br/>{1}", question1, question2, line, color, warning);
				icon = "manifest:Epsitec.Common.Dialogs.Images.Warning.icon";
			}

			Common.Dialogs.IDialog dialog = Common.Dialogs.MessageDialog.CreateYesNoCancel(title, icon, message, null, null, this.CommandDispatcher);
			dialog.OwnerWindow = this.Window;
			dialog.OpenDialog();
			return dialog.Result;
		}

		public Common.Dialogs.DialogResult DialogQuestion(string question)
		{
			//	Affiche le dialogue pour poser une question oui/non.
			if ( this.Window == null )  return Common.Dialogs.DialogResult.None;

			string title = Res.Strings.Application.Title;
			string icon = "manifest:Epsitec.Common.Dialogs.Images.Question.icon";
			string message = question;

			Common.Dialogs.IDialog dialog = Common.Dialogs.MessageDialog.CreateYesNo(title, icon, message, "", "", this.CommandDispatcher);
			dialog.OwnerWindow = this.Window;
			dialog.OpenDialog();
			return dialog.Result;
		}

		public Common.Dialogs.DialogResult DialogQuestion(string question, string yes, string no, string cancel)
		{
			//	Affiche le dialogue pour poser une question oui/non.
			if ( this.Window == null )  return Common.Dialogs.DialogResult.None;

			string title = Res.Strings.Application.Title;
			string icon = "manifest:Epsitec.Common.Dialogs.Images.Question.icon";
			string message = question;

			Common.Dialogs.IDialog dialog = Common.Dialogs.MessageDialog.CreateYesNoCancel(title, yes, no, cancel, icon, message, "", "", this.CommandDispatcher);
			dialog.OwnerWindow = this.Window;
			dialog.OpenDialog();
			return dialog.Result;
		}

		public Common.Dialogs.DialogResult DialogMessage(string message)
		{
			//	Affiche le dialogue pour afficher un message neutre.
			if ( this.Window == null )  return Common.Dialogs.DialogResult.None;

			string title = Res.Strings.Application.Title;
			string icon = "manifest:Epsitec.Common.Dialogs.Images.Information.icon";

			Common.Dialogs.IDialog dialog = Common.Dialogs.MessageDialog.CreateOk(title, icon, message, "", this.CommandDispatcher);
			dialog.OwnerWindow = this.Window;
			dialog.OpenDialog();
			return dialog.Result;
		}

		public Common.Dialogs.DialogResult DialogError(string error)
		{
			//	Affiche le dialogue pour signaler une erreur.
			if ( this.Window == null )  return Common.Dialogs.DialogResult.None;
			if ( error == "" )  return Common.Dialogs.DialogResult.None;

			string title = Res.Strings.Application.Title;
			string icon = "manifest:Epsitec.Common.Dialogs.Images.Warning.icon";
			//?string message = TextLayout.ConvertToTaggedText(error);  // surtout pas, à cause des textes mis en page avec des <b>, etc.
			string message = error;

			Common.Dialogs.IDialog dialog = Common.Dialogs.MessageDialog.CreateOk(title, icon, message, "", this.CommandDispatcher);
			dialog.OwnerWindow = this.Window;
			dialog.OpenDialog();
			return dialog.Result;
		}

		public Common.Dialogs.DialogResult DialogConfirmation(string header, List<string> questions, bool hasCancelButton)
		{
			//	Affiche le dialogue pour demander une confirmation.
			if ( this.Window == null )  return Common.Dialogs.DialogResult.None;

			string title = Res.Strings.Application.Title;
			Common.Dialogs.IDialog dialog = Common.Dialogs.MessageDialog.CreateConfirmation(title, header, questions, hasCancelButton);
			dialog.OwnerWindow = this.Window;
			dialog.OpenDialog();
			return dialog.Result;
		}

		private void HandleDlgClosed(object sender)
		{
			//	Un dialogue a été fermé.
			if (sender == this.dlgGlyphs)
			{
				this.glyphsState.ActiveState = ActiveState.No;
			}

			if (sender == this.dlgSearch)
			{
				this.searchState.ActiveState = ActiveState.No;
			}

			if (sender == this.dlgInitialMessage)
			{
				this.initialMessageState.ActiveState = ActiveState.No;
			}
		}
		#endregion


		#region ModuleInfo class
		private class ModuleInfo : System.IDisposable
		{
			public Module						Module;
			public TabPage						TabPage;
			public FrameBox						ModuleFrame;
			public StaticText					ModuleTitle;
			public MyWidgets.BundleType			BundleTypeWidget;
			public GlyphButton					ShowHideButton;

			#region IDisposable Members
			public void Dispose()
			{
				if ( this.TabPage != null )  this.TabPage.Dispose();
			}
			#endregion
		}
		#endregion


		#region Instance
		public static DesignerApplication GetInstance(DependencyObject obj)
		{
			return (DesignerApplication) obj.GetValue(DesignerApplication.InstanceProperty);
		}

		public static void SetInstance(DependencyObject obj, DesignerApplication value)
		{
			obj.SetValue(DesignerApplication.InstanceProperty, value);
		}
		#endregion


		private DesignerMode					mode;
		private bool							standalone;
		private RibbonBook						ribbonBook;
		private RibbonPage						ribbonMain;
		private RibbonPage						ribbonOper;
		private RibbonPage						ribbonActive;
		private TabBook							bookModules;
		private StatusBar						info;
		private ResizeKnob						resize;
		private Dialogs.NewDialog				dlgNew;
		private Dialogs.OpenDialog				dlgOpen;
		private Dialogs.GlyphsDialog			dlgGlyphs;
		private Dialogs.IconDialog				dlgIcon;
		private Dialogs.SearchDialog			dlgSearch;
		private Dialogs.NewCultureDialog		dlgNewCulture;
		private Dialogs.ResourceTypeCodeDialog	dlgResourceTypeCode;
		private Dialogs.ResourceSelectorDialog	dlgResourceSelector;
		private Dialogs.BindingSelectorDialog	dlgBindingSelector;
		private Dialogs.ResourceNameDialog		dlgFieldName;
		private Dialogs.EntityFieldDialog		dlgEntityField;
		private Dialogs.LabelReplacementDialog	dlgLabelReplacement;
		private Dialogs.EntityCommentDialog		dlgEntityComment;
		private Dialogs.EntityParametersDialog	dlgEntityParameters;
		private Dialogs.EntityCreationDialog	dlgEntityCreation;
		private Dialogs.EntityExpressionDialog	dlgEntityExpression;
		private Dialogs.InitialMessageDialog	dlgInitialMessage;
		private Dialogs.ModuleInfoDialog		dlgModuleInfo;
		private Dialogs.SaveAllImagesDialog		dlgSaveAllImages;
		private PanelsContext					context;
		private DisplayMode						displayMode;
		private Window							viewersWindow;
		private Image							icon;

		private Support.ResourceManagerPool		resourceManagerPool;
		private List<ModuleInfo>				moduleInfoList;
		private Settings						settings;
		private int								lastModule = -1;
		private int								currentModule = -1;
		private bool							ignoreChange = false;
		private double							moveHorizontal = 5;
		private double							moveVertical = 5;
		private bool							showModuleTitle = true;

		private List<Viewers.Locator>			locators;
		private int								locatorIndex;
		private bool							locatorIgnore;

		private CommandState					newState;
		private CommandState					recycleState;
		private CommandState					openState;
		private CommandState					saveState;
		private CommandState					saveAsState;
		private CommandState					initialMessageState;
		private CommandState					checkState;
		private CommandState					closeState;
		private CommandState					cutState;
		private CommandState					copyState;
		private CommandState					pasteState;
		private CommandState					editLockedState;
		private CommandState					editOkState;
		private CommandState					editCancelState;
		private CommandState					deleteState;
		private CommandState					createState;
		private CommandState					duplicateState;
		private CommandState					copyToModuleState;
		private CommandState					fontBoldState;
		private CommandState					fontItalicState;
		private CommandState					fontUnderlineState;
		private CommandState					glyphsState;
		private CommandState					filterState;
		private CommandState					searchState;
		private CommandState					searchPrevState;
		private CommandState					searchNextState;
		private CommandState					accessFirstState;
		private CommandState					accessPrevState;
		private CommandState					accessNextState;
		private CommandState					accessLastState;
		private CommandState					modificationAllState;
		private CommandState					modificationClearState;
		private CommandState					modificationPrevState;
		private CommandState					modificationNextState;
		private CommandState					newCultureState;
		private CommandState					deleteCultureState;
		private CommandState					toolSelectState;
		private CommandState					toolGlobalState;
		private CommandState					toolGridState;
		private CommandState					toolEditState;
		private CommandState					toolZoomState;
		private CommandState					toolHandState;
		private CommandState					objectHLineState;
		private CommandState					objectVLineState;
		private CommandState					objectSquareButtonState;
		private CommandState					objectRectButtonState;
		private CommandState					objectTableState;
		private CommandState					objectTextState;
		private CommandState					objectStaticState;
		private CommandState					objectGroupState;
		private CommandState					objectGroupFrameState;
		private CommandState					objectGroupBoxState;
		private CommandState					objectPanelState;
		private CommandState					panelDeselectAllState;
		private CommandState					panelSelectAllState;
		private CommandState					panelSelectInvertState;
		private CommandState					panelSelectRootState;
		private CommandState					panelSelectParentState;
		private CommandState					panelShowGridState;
		private CommandState					panelShowZOrderState;
		private CommandState					panelShowTabIndexState;
		private CommandState					panelShowExpandState;
		private CommandState					panelShowConstrainState;
		private CommandState					panelShowAttachmentState;
		private CommandState					panelRunState;
		private CommandState					alignLeftState;
		private CommandState					alignCenterXState;
		private CommandState					alignRightState;
		private CommandState					alignTopState;
		private CommandState					alignCenterYState;
		private CommandState					alignBottomState;
		private CommandState					alignBaseLineState;
		private CommandState					adjustWidthState;
		private CommandState					adjustHeightState;
		private CommandState					alignGridState;
		private CommandState					moveLeftState;
		private CommandState					moveRightState;
		private CommandState					moveDownState;
		private CommandState					moveUpState;
		private CommandState					panelOrderUpAllState;
		private CommandState					panelOrderDownAllState;
		private CommandState					panelOrderUpOneState;
		private CommandState					panelOrderDownOneState;
		private CommandState					tabIndexClearState;
		private CommandState					tabIndexFirstState;
		private CommandState					tabIndexPrevState;
		private CommandState					tabIndexNextState;
		private CommandState					tabIndexLastState;
		private CommandState					tabIndexRenumState;
		private CommandState					undoState;
		private CommandState					redoState;
		private CommandState					undoRedoListState;
		private CommandState					locatorPrevState;
		private CommandState					locatorNextState;
		private CommandState					displayHorizontalState;
		private CommandState					displayVerticalState;
		private CommandState					displayFullScreenState;
		private CommandState					displayWindowState;

		public static readonly DependencyProperty InstanceProperty = DependencyProperty.RegisterAttached("Instance", typeof(DesignerApplication), typeof(DesignerApplication));
	}
}
