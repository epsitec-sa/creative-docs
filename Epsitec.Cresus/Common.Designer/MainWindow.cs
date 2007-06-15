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
	public class MainWindow : DependencyObject
	{
		public enum DisplayMode
		{
			Horizontal,
			Vertical,
			FullScreen,
		}


		static MainWindow()
		{
			Res.Initialize();

			ImageProvider.Default.EnableLongLifeCache = true;
			ImageProvider.Default.PrefillManifestIconCache();
		}

		public MainWindow()
			: this (new ResourceManagerPool("Common.Designer"))
		{
			this.resourceManagerPool.DefaultPrefix = "file";
		}

		internal MainWindow(ResourceManagerPool pool)
		{
			this.resourceManagerPool = pool;
			this.LocatorInit();
			this.moduleInfoList = new List<ModuleInfo>();
			this.context = new PanelsContext();
		}

		public void Show(Window parentWindow)
		{
			//	Crée et montre la fenêtre de l'éditeur.
			if ( this.window == null )
			{
				this.window = new Window();

				this.window.Root.WindowStyles = WindowStyles.DefaultDocumentWindow;

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

				windowBounds = new Rectangle (parentCenter.X-1000/2, parentCenter.Y-700/2, 1000, 700);
				windowBounds = ScreenInfo.FitIntoWorkingArea (windowBounds);

				this.window.WindowBounds = windowBounds;
				this.window.Root.MinSize = new Size(500, 400);
				this.window.Text = Res.Strings.Application.Title;
				this.window.Name = "Application";  // utilisé pour générer "QuitApplication" !
				this.window.PreventAutoClose = true;
				
				MainWindow.SetInstance(this.window, this);  // attache l'instance de MainWindow à la fenêtre

				this.commandDispatcher = new CommandDispatcher("Common.Designer", CommandDispatcherLevel.Primary);
				this.commandContext = new CommandContext();
				this.commandDispatcher.RegisterController(this);
				
				CommandDispatcher.SetDispatcher(this.window, this.commandDispatcher);
				CommandContext.SetContext(this.window, this.commandContext);

				this.dlgOpen                        = new Dialogs.Open(this);
				this.dlgGlyphs                      = new Dialogs.Glyphs(this);
				this.dlgIcon                        = new Dialogs.Icon(this);
				this.dlgFilter                      = new Dialogs.Filter(this);
				this.dlgSearch                      = new Dialogs.Search(this);
				this.dlgNewCulture                  = new Dialogs.NewCulture(this);
				this.dlgResourceTypeCode            = new Dialogs.ResourceTypeCode(this);
				this.dlgResourceSelector            = new Dialogs.ResourceSelector(this);
				this.dlgResourceStructuredTypeField = new Dialogs.ResourceStructuredTypeField(this);
				this.dlgBindingSelector             = new Dialogs.BindingSelector(this);
				this.dlgTableConfiguration          = new Dialogs.TableConfiguration(this);
				this.dlgFieldName                   = new Dialogs.FieldName(this);
				this.dlgEntityComment               = new Dialogs.EntityComment(this);

				this.dlgGlyphs.Closed += new EventHandler(this.HandleDlgClosed);
				this.dlgFilter.Closed += new EventHandler(this.HandleDlgClosed);
				this.dlgSearch.Closed += new EventHandler(this.HandleDlgClosed);

				this.InitCommands();
				this.CreateLayout();
			}

			//	Liste les modules, puis ouvre tous ceux qui ont été trouvés.
			Resources.DefaultManager.RefreshModuleInfos(this.resourceManagerPool.DefaultPrefix);
			if (this.moduleInfoList.Count == 0)
			{
				foreach (ResourceModuleInfo item in Resources.DefaultManager.GetModuleInfos(this.resourceManagerPool.DefaultPrefix))
				{
					Module module = new Module(this, this.mode, this.resourceManagerPool.DefaultPrefix, item);

					ModuleInfo mi = new ModuleInfo();
					mi.Module = module;
					this.moduleInfoList.Insert(++this.currentModule, mi);
					this.CreateModuleLayout();

					this.bookModules.ActivePage = mi.TabPage;
				}
			}
			
			this.window.Show();

#if false
			for (int i = 0; i < modules.Length; i++)
			{
				System.Diagnostics.Debug.WriteLine (string.Format ("Module {0}: {1}", i, modules[i]));
				
				ResourceManager resourceManager = new ResourceManager ();

				resourceManager.SetupApplication (modules[i]);
				resourceManager.ActivePrefix = this.resourceManagerPool.DefaultPrefix;

				string[] ids = resourceManager.GetBundleIds ("*", ResourceLevel.Default);

				for (int j = 0; j < ids.Length; j++)
				{
					System.Diagnostics.Debug.WriteLine (string.Format ("  Bundle {0}: {1}", j, ids[j]));

					ResourceBundleCollection bundles = new ResourceBundleCollection (resourceManager);
					bundles.LoadBundles (resourceManager.ActivePrefix, resourceManager.GetBundleIds (ids[j], ResourceLevel.All));

					for (int k = 0; k < bundles.Count; k++)
					{
						ResourceBundle bundle = bundles[k];
						bool needsSave = false;
						
						System.Diagnostics.Debug.WriteLine (string.Format ("  Culture {0}, name '{1}'", bundle.Culture.Name, bundle.Name ?? "<null>"));
						System.Diagnostics.Debug.WriteLine (string.Format ("    About: '{0}'", bundle.About ?? "<null>"));
						System.Diagnostics.Debug.WriteLine (string.Format ("    {0} fields", bundle.FieldCount));

						foreach (ResourceBundle.Field field in bundle.Fields)
						{
							if (field.ModificationId > 0)
							{
								System.Diagnostics.Debug.WriteLine (string.Format ("      {0}: modif. id {1}, about: {2}", field.Name, field.ModificationId, field.About ?? "<null>"));

								field.SetModificationId (field.ModificationId+1);
								needsSave = true;
							}
						}
						
						if (needsSave)
						{
							resourceManager.SetBundle (bundle, ResourceSetMode.UpdateOnly);
						}
					}
				}
			}

			this.window.ShowDialog();
#endif

#if false
			ResourceManager resourceManager = new ResourceManager();
			resourceManager.SetupApplication(modules[0]);
			resourceManager.ActivePrefix = this.resourceManagerPool.DefaultPrefix;
			string[] ids = resourceManager.GetBundleIds("*", ResourceLevel.Default);

			ResourceBundleCollection bundles = new ResourceBundleCollection(resourceManager);
			bundles.LoadBundles(resourceManager.ActivePrefix, resourceManager.GetBundleIds(ids[0], ResourceLevel.All));

			this.bundle0 = bundles[0];
			this.bundle1 = bundles[1];

			this.window.ShowDialog();
			this.UpdateFields();
#endif
		}

		internal void Hide()
		{
			this.window.Hide ();
		}

		public Window Window
		{
			//	Retourne la fenêtre principale de l'application.
			get
			{
				return this.window;
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

		public ResourceManagerPool ResourceManagerPool
		{
			get
			{
				return this.resourceManagerPool;
			}
		}

		public CommandContext CommandContext
		{
			get
			{
				return this.commandContext;
			}
		}

		public CommandDispatcher CommandDispatcher
		{
			get
			{
				return this.commandDispatcher;
			}
		}

		public CommandState GetCommandState(string command)
		{
			CommandContext context = this.CommandContext;
			CommandState state = context.GetCommandState (command);

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

		protected void CreateLayout()
		{
			this.ribbonBook = new RibbonBook(this.window.Root);
			this.ribbonBook.Dock = DockStyle.Top;

			//	Crée le ruban principal.
			this.ribbonMain = new RibbonPage();
			this.ribbonMain.RibbonTitle = Res.Strings.Ribbon.Main;
			this.ribbonBook.Items.Add(this.ribbonMain);

			this.ribbonMain.Items.Add(new Ribbons.File(this));
			this.ribbonMain.Items.Add(new Ribbons.Culture(this));
			this.ribbonMain.Items.Add(new Ribbons.Clipboard(this));
			if (this.mode == DesignerMode.Build)
			{
				this.ribbonMain.Items.Add(new Ribbons.Edit(this));
			}
			this.ribbonMain.Items.Add(new Ribbons.Access(this));
			this.ribbonMain.Items.Add(new Ribbons.Character(this));
			this.ribbonMain.Items.Add(new Ribbons.Display(this));
			this.ribbonMain.Items.Add(new Ribbons.Locator(this));

			//	Crée le ruban des opérations.
			this.ribbonOper = new RibbonPage();
			this.ribbonOper.RibbonTitle = Res.Strings.Ribbon.Oper;
			this.ribbonBook.Items.Add(this.ribbonOper);

			this.ribbonOper.Items.Add(new Ribbons.PanelShow(this));
			this.ribbonOper.Items.Add(new Ribbons.PanelSelect(this));
			this.ribbonOper.Items.Add(new Ribbons.Move(this));
			this.ribbonOper.Items.Add(new Ribbons.Align(this));
			this.ribbonOper.Items.Add(new Ribbons.Order(this));
			this.ribbonOper.Items.Add(new Ribbons.TabIndex(this));

			//	Crée la barre de statuts.
			this.info = new StatusBar(this.window.Root);
			this.info.Dock = DockStyle.Bottom;
			this.info.Margins = new Margins(0, 0, 0, 0);

			this.InfoAdd("InfoCurrentModule", 250);
			this.InfoAdd("InfoAccess", 250);
			this.InfoAdd("InfoViewer", 300);

			this.resize = new ResizeKnob();
			this.resize.Margins = new Margins(2, 0, 0, 0);
			this.info.Items.Add(this.resize);
			this.resize.Dock = DockStyle.Right;  // doit être fait après le Items.Add !
			ToolTip.Default.SetToolTip(this.resize, Res.Strings.Dialog.Tooltip.Resize);

			//	Crée le TabBook principal pour les modules ouverts.
			this.bookModules = new TabBook(this.window.Root);
			this.bookModules.Dock = DockStyle.Fill;
			this.bookModules.Margins = new Margins(0, 0, 3, 0);
			this.bookModules.Arrows = TabBookArrows.Right;
			this.bookModules.HasCloseButton = true;
			this.bookModules.CloseButton.CommandObject = Command.Get("Close");
			this.bookModules.ActivePageChanged += new EventHandler(this.HandleBookModulesActivePageChanged);
			ToolTip.Default.SetToolTip(this.bookModules.CloseButton, Res.Strings.Action.Close);

			this.ribbonActive = this.ribbonMain;
			this.ribbonBook.ActivePage = this.ribbonMain;
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

		protected IconButton SearchIconButton(string command)
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

		protected IconButton SearchIconButton(RibbonPage page, string command)
		{
			//	Cherche le bouton utilisé pour une commande, dans un ruban.
			foreach (Widget widget in page.Items)
			{
				Ribbons.Abstract section = widget as Ribbons.Abstract;
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


		#region Info manager
		protected StatusField InfoAdd(string name, double width)
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
			System.Text.StringBuilder builder = new System.Text.StringBuilder();

			ModuleInfo mi = this.CurrentModuleInfo;
			if ( mi != null )
			{
				ResourceAccess access;

				access = mi.Module.AccessStrings;
				builder.Append(Res.Strings.BundleType.Strings);
				builder.Append(":");
				builder.Append(access.TotalCount);
				builder.Append(", ");

				access = mi.Module.AccessCaptions;
				builder.Append(Res.Strings.BundleType.Captions);
				builder.Append(":");
				builder.Append(access.TotalCount);
				builder.Append(", ");

				access = mi.Module.AccessPanels;
				builder.Append(Res.Strings.BundleType.Panels);
				builder.Append(":");
				builder.Append(access.TotalCount);
			}

			StatusField field = this.info.Items["InfoCurrentModule"] as StatusField;
			string text = builder.ToString();

			if (field.Text != text)
			{
				field.Text = text;
				field.Invalidate();
			}
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

		public void UpdateViewer(MyWidgets.PanelEditor.Changing oper)
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
			this.Terminate();
			this.dlgOpen.SetResourcePrefix(this.resourceManagerPool.DefaultPrefix);
			this.dlgOpen.Show();

			ResourceModuleInfo item = this.dlgOpen.SelectedModule;
			if (item.Name != null)
			{
				Module module = new Module(this, this.mode, this.resourceManagerPool.DefaultPrefix, item);

				ModuleInfo mi = new ModuleInfo();
				mi.Module = module;
				this.moduleInfoList.Insert(++this.currentModule, mi);
				this.CreateModuleLayout();

				this.bookModules.ActivePage = mi.TabPage;
			}
		}

		[Command("Check")]
		void CommandCheck(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.Terminate();
			this.CurrentModule.Check();
		}

		[Command("Save")]
		void CommandSave(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.Terminate();
			this.CurrentModule.Save();
		}

		[Command("Close")]
		void CommandClose(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.Terminate();
			if ( !this.AutoSave(dispatcher) )  return;
			this.CloseModule();
		}

		[Command(ApplicationCommands.Id.Quit)]
		[Command("QuitApplication")]
		void CommandQuitApplication(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.Terminate();
			if ( !this.AutoSaveAll(dispatcher) )  return;

			this.dlgGlyphs.Hide();
			this.dlgFilter.Hide();
			this.dlgSearch.Hide();

			this.window.Hide();
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

		[Command("Filter")]
		void CommandFilter(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			if (this.filterState.ActiveState == ActiveState.No)
			{
				this.dlgFilter.Show();
				this.filterState.ActiveState = ActiveState.Yes;
			}
			else
			{
				this.dlgFilter.Hide();
				this.filterState.ActiveState = ActiveState.No;
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
			this.Terminate();
			if ( !this.IsCurrentModule )  return;
			this.CurrentModule.Modifier.ActiveViewer.DoNewCulture();
		}

		[Command("DeleteCulture")]
		void CommandDeleteCulture(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.Terminate();
			if ( !this.IsCurrentModule )  return;
			this.CurrentModule.Modifier.ActiveViewer.DoDeleteCulture();
		}

		[Command("Delete")]
		void CommandDelete(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.Terminate();
			if ( !this.IsCurrentModule )  return;
			this.CurrentModule.Modifier.ActiveViewer.DoDelete();
		}

		[Command("Create")]
		void CommandCreate(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.Terminate();
			if ( !this.IsCurrentModule )  return;
			this.CurrentModule.Modifier.ActiveViewer.DoDuplicate(false);
		}

		[Command("Duplicate")]
		void CommandDuplicate(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.Terminate();
			if ( !this.IsCurrentModule )  return;
			this.CurrentModule.Modifier.ActiveViewer.DoDuplicate(true);
		}

		[Command("CopyToModule")]
		void CommandCopyToModule(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.Terminate();
			if ( !this.IsCurrentModule )  return;
			this.CurrentModule.Modifier.ActiveViewer.DoCopyToModule(this.LastModule);
		}

		[Command("Cut")]
		[Command("Copy")]
		[Command("Paste")]
		void CommandClipboard(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			if ( !this.IsCurrentModule )  return;
			this.CurrentModule.Modifier.ActiveViewer.DoClipboard(e.Command.CommandId);
		}

		[Command("FontBold")]
		[Command("FontItalic")]
		[Command("FontUnderline")]
		void CommandFont(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			if ( !this.IsCurrentModule )  return;
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
			if ( !this.IsCurrentModule )  return;
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
		void CommandCommand(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			if ( !this.IsCurrentModule )  return;
			this.CurrentModule.Modifier.ActiveViewer.DoCommand(e.Command.CommandId);
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
			string value = StructuredCommand.GetFieldValue(e.CommandState, "Name") as string;
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

		[Command ("ZoomChange")]
		void CommandZoomChange(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			string value = StructuredCommand.GetFieldValue(e.CommandState, "Name") as string;
			double zoom = System.Convert.ToDouble(value);

			Module module = this.CurrentModule;
			Viewers.Entities ve = module.Modifier.ActiveViewer as Viewers.Entities;
			if (module != null && ve != null)
			{
				ve.Zoom = zoom;
			}
		}

		protected void InitCommands()
		{
			this.newState = this.CreateCommandState("New", KeyCode.ModifierControl|KeyCode.AlphaN);
			this.openState = this.CreateCommandState("Open", KeyCode.ModifierControl|KeyCode.AlphaO);
			this.saveState = this.CreateCommandState("Save", KeyCode.ModifierControl|KeyCode.AlphaS);
			this.saveAsState = this.CreateCommandState("SaveAs");
			this.checkState = this.CreateCommandState("Check");
			this.closeState = this.CreateCommandState("Close", KeyCode.ModifierControl|KeyCode.FuncF4);
			this.cutState = this.CreateCommandState("Cut", KeyCode.ModifierControl|KeyCode.AlphaX);
			this.copyState = this.CreateCommandState("Copy", KeyCode.ModifierControl|KeyCode.AlphaC);
			this.pasteState = this.CreateCommandState("Paste", KeyCode.ModifierControl|KeyCode.AlphaV);
			this.deleteState = this.CreateCommandState("Delete");
			this.createState = this.CreateCommandState("Create");
			this.duplicateState = this.CreateCommandState("Duplicate");
			this.copyToModuleState = this.CreateCommandState("CopyToModule", KeyCode.ModifierControl|KeyCode.AlphaM);

			this.fontBoldState = this.CreateCommandState("FontBold", KeyCode.ModifierControl|KeyCode.AlphaB);
			this.fontItalicState = this.CreateCommandState("FontItalic", KeyCode.ModifierControl|KeyCode.AlphaI);
			this.fontUnderlineState = this.CreateCommandState("FontUnderline", KeyCode.ModifierControl|KeyCode.AlphaU);

			this.glyphsState = this.CreateCommandState("DesignerGlyphs");
			this.filterState = this.CreateCommandState("Filter");
			
			this.searchState = this.CreateCommandState("Search");
			this.searchPrevState = this.CreateCommandState("SearchPrev");
			this.searchNextState = this.CreateCommandState("SearchNext");
			
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

			this.locatorPrevState = this.CreateCommandState("LocatorPrev", KeyCode.ArrowLeft|KeyCode.ModifierAlt);
			this.locatorNextState = this.CreateCommandState("LocatorNext", KeyCode.ArrowRight|KeyCode.ModifierAlt);

			this.displayHorizontalState = this.CreateCommandState("DisplayHorizontal");
			this.displayVerticalState = this.CreateCommandState("DisplayVertical");
			this.displayFullScreenState = this.CreateCommandState("DisplayFullScreen");
			this.displayHorizontalState.ActiveState = ActiveState.Yes;
		}

		protected CommandState CreateCommandState(string commandName, params Widgets.Shortcut[] shortcuts)
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
				bool statefull = (commandName == "FontBold" || 
								  commandName == "FontItalic" ||
								  commandName == "FontUnderline" ||
								  commandName.StartsWith("PanelShow") ||
								  commandName == "DisplayHorizontal" ||
								  commandName == "DisplayVertical" ||
								  commandName == "DisplayFullScreen");

				command.ManuallyDefineCommand(description, Misc.Icon(iconName), null, statefull);
			}

			return this.CommandContext.GetCommandState(command);
		}
		#endregion


		public DisplayMode DisplayModeState
		{
			//	Disposition de l'affichage (horizontal ou vertical).
			get
			{
				if (this.displayHorizontalState.ActiveState == ActiveState.Yes)  return DisplayMode.Horizontal;
				if (this.displayVerticalState.ActiveState == ActiveState.Yes)  return DisplayMode.Vertical;
				return DisplayMode.FullScreen;
			}
			set
			{
				if (this.DisplayModeState != value)
				{
					this.displayHorizontalState.ActiveState = (value == DisplayMode.Horizontal) ? ActiveState.Yes : ActiveState.No;
					this.displayVerticalState.ActiveState   = (value == DisplayMode.Vertical  ) ? ActiveState.Yes : ActiveState.No;
					this.displayFullScreenState.ActiveState = (value == DisplayMode.FullScreen) ? ActiveState.Yes : ActiveState.No;
					this.HandleTypeChanged(null);
				}
			}
		}


		#region Locator
		protected void LocatorInit()
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

			string moduleName = mi.Module.ModuleInfo.Name;
			ResourceAccess.Type viewerType = mi.BundleTypeWidget.CurrentType;

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

			Viewers.Locator locator = new Viewers.Locator(moduleName, viewerType, resource, widgetFocused, lineSelected);

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

		public VMenu LocatorCreateMenu(MessageEventHandler message)
		{
			//	Construit le menu des localisations visitées.
			int all = this.locators.Count;
			int total = System.Math.Min(all, 20);
			int start = this.locatorIndex;
			start += total/2;  if ( start > all-1 )  start = all-1;
			start -= total-1;  if ( start < 0     )  start = 0;

			List<Widget> list = new List<Widget>();

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
			for (int i=0; i<list.Count; i++)
			{
				menu.Items.Add(list[i]);
			}
			menu.AdjustSize();
			return menu;
		}

		protected void LocatorCreateMenu(List<Widget> list, MessageEventHandler message, string icon, int rank, string action)
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

		protected string LocatorGetNiceText(int index)
		{
			//	Retourne le joli texte correspondant à une localisations.
			string moduleName              = this.locators[index].ModuleName;
			ResourceAccess.Type viewerType = this.locators[index].ViewerType;
			Druid resource                 = this.locators[index].Resource;

			string typeText = ResourceAccess.TypeDisplayName(viewerType);

			string resourceText = resource.ToString();  // affiche le Druid si on ne trouve rien de mieux
			int moduleRank = this.SearchModuleRank(moduleName);
			if (moduleRank != -1)
			{
				ResourceAccess access = this.moduleInfoList[moduleRank].Module.GetAccess(viewerType);
				if (access != null)
				{
					resourceText = access.DirectGetDisplayName(resource);
				}
			}

			return System.String.Format("{0}/{1}: {2}", moduleName, typeText, resourceText);
		}

		protected void LocatorClose(string moduleName)
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

		protected bool LocatorPrevIsEnable
		{
			//	Donne l'état de la commande "LocatorPrev".
			get
			{
				System.Diagnostics.Debug.Assert(this.locators != null);
				return this.locatorIndex > 0;
			}
		}

		protected bool LocatorNextIsEnable
		{
			//	Donne l'état de la commande "LocatorNext".
			get
			{
				System.Diagnostics.Debug.Assert(this.locators != null);
				return this.locatorIndex < this.locators.Count-1;
			}
		}

		protected void LocatorPrev()
		{
			//	Action de la commande "LocatorPrev".
			System.Diagnostics.Debug.Assert(this.LocatorPrevIsEnable);
			Viewers.Locator locator = this.locators[--this.locatorIndex];
			this.LocatorGoto(locator);
		}

		protected void LocatorNext()
		{
			//	Action de la commande "LocatorNext".
			System.Diagnostics.Debug.Assert(this.LocatorNextIsEnable);
			Viewers.Locator locator = this.locators[++this.locatorIndex];
			this.LocatorGoto(locator);
		}

		protected void LocatorMenuGoto(int index)
		{
			//	Revient à une location choisie dans le menu.
			this.locatorIndex = index;
			Viewers.Locator locator = this.locators[this.locatorIndex];
			this.LocatorGoto(locator);
		}

		public void LocatorGoto(string moduleName, ResourceAccess.Type viewerType, Druid resource, Widget widgetFocused)
		{
			//	Va sur une ressource d'une vue d'un module quelconque.
			int lineSelected = -1;
			this.LocatorAdjustWidgetFocused(ref widgetFocused, ref lineSelected);
			Viewers.Locator locator = new Viewers.Locator(moduleName, viewerType, resource, widgetFocused, lineSelected);
			this.LocatorGoto(locator);
		}

		protected void LocatorGoto(Viewers.Locator locator)
		{
			//	Va sur une ressource définie par une localisation.
			this.Terminate();
			ModuleInfo mi = this.CurrentModuleInfo;

			if (mi.Module.ModuleInfo.Name != locator.ModuleName)
			{
				int rank = this.SearchModuleRank(locator.ModuleName);
				if (rank == -1)
				{
					return;
				}

				this.locatorIgnore = true;
				this.UseModule(rank);
				this.locatorIgnore = false;

				mi = this.CurrentModuleInfo;
			}

			this.locatorIgnore = true;
			mi.BundleTypeWidget.CurrentType = locator.ViewerType;
			this.locatorIgnore = false;

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
					Viewers.Abstract viewer = mi.Module.Modifier.ActiveViewer;
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

		protected void LocatorAdjustWidgetFocused(ref Widget widgetFocused, ref int lineSelected)
		{
			//	Si le focus est dans une StringList, cherche la sélection dans le StringArray parent.
			if (widgetFocused != null && widgetFocused.Parent is MyWidgets.StringArray)
			{
				MyWidgets.StringArray array = widgetFocused.Parent as MyWidgets.StringArray;
				lineSelected = array.SelectedRow;
			}
		}

		protected void LocatorUpdateCommand()
		{
			//	Met à jour les commandes du navigateur.
			this.locatorPrevState.Enable = this.LocatorPrevIsEnable;
			this.locatorNextState.Enable = this.LocatorNextIsEnable;
		}
		#endregion


		#region Modules manager
		protected void CreateModuleLayout()
		{
			ModuleInfo mi = this.CurrentModuleInfo;

			mi.TabPage = new TabPage();
			mi.TabPage.TabTitle = mi.Module.ModuleInfo.Name;
			this.bookModules.Items.Insert(this.currentModule, mi.TabPage);

			mi.BundleTypeWidget = new MyWidgets.BundleType(mi.TabPage);
			mi.BundleTypeWidget.Dock = DockStyle.Top;
			mi.BundleTypeWidget.TypeChanged += new EventHandler(this.HandleTypeChanged);

			this.CreateViewerLayout();
		}

		protected void CreateViewerLayout()
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

		public void Terminate()
		{
			//	Termine le travail sur une ressource, avant de passer à une autre.
			this.Terminate(false);
		}

		public void Terminate(bool soft)
		{
			//	Termine le travail sur une ressource, avant de passer à une autre.
			//	Si soft = true, on sérialise temporairement sans poser de question.
			if (this.IsCurrentModule && this.CurrentModule.Modifier.ActiveViewer != null)
			{
				this.CurrentModule.Modifier.ActiveViewer.Terminate(soft);
			}
		}

		private void HandleTypeChanged(object sender)
		{
			//	Appelé lorsque le type de vue a changé.
			this.Terminate(true);
			this.CreateViewerLayout();
			this.DialogSearchAdapt();
			this.LocatorFix();
			this.CurrentModule.Modifier.ActiveViewer.UpdateCommands();
		}


		private void HandleBookModulesActivePageChanged(object sender)
		{
			//	L'onglet pour le module courant a été cliqué.
			if ( this.ignoreChange )  return;

			int total = this.bookModules.PageCount;
			for (int i=0; i<total; i++)
			{
				ModuleInfo di = this.moduleInfoList[i];
				if ( di.TabPage == this.bookModules.ActivePage )
				{
					this.UseModule(i);
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

		protected ModuleInfo CurrentModuleInfo
		{
			//	Retourne le ModuleInfo courant.
			get
			{
				if ( this.currentModule < 0 )  return null;
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
				if ( this.currentModule < 0 )  return null;
				return this.CurrentModuleInfo.Module;
			}
		}

		protected Module LastModule
		{
			//	Retourne le module précédemment sélectionné.
			get
			{
				if ( this.lastModule < 0 || this.lastModule >= this.moduleInfoList.Count )  return null;
				if ( this.lastModule == this.currentModule )  return null;
				return this.moduleInfoList[this.lastModule].Module;
			}
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

		public Module SearchModuleId(int id)
		{
			//	Cherche un module d'après son identificateur.
			foreach (ModuleInfo info in this.moduleInfoList)
			{
				if (info.Module.ModuleInfo.Id == id)
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

		protected int SearchModuleRank(string moduleName)
		{
			//	Cherche le rang d'un module d'après son nom.
			for (int i=0; i<this.moduleInfoList.Count; i++)
			{
				ModuleInfo mi = this.moduleInfoList[i];
				if (mi.Module.ModuleInfo.Name == moduleName)
				{
					return i;
				}
			}

			return -1;
		}

		protected void UseModule(int rank)
		{
			//	Utilise un module ouvert.
			if ( this.ignoreChange )  return;

			this.lastModule = this.currentModule;
			this.currentModule = rank;

			if ( rank >= 0 )
			{
				this.ignoreChange = true;
				this.bookModules.ActivePage = this.CurrentModuleInfo.TabPage;
				this.ignoreChange = false;

				this.DialogSearchAdapt();
				this.CurrentModule.Modifier.ActiveViewer.UpdateWhenModuleUsed();
			}
			else
			{
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
				this.deleteState.Enable = false;
				this.createState.Enable = false;
				this.duplicateState.Enable = false;
				this.copyToModuleState.Enable = false;
				this.fontBoldState.Enable = false;
				this.fontItalicState.Enable = false;
				this.fontUnderlineState.Enable = false;
				this.glyphsState.Enable = false;

				this.UpdateInfoCurrentModule();
				this.UpdateInfoAccess();
				this.UpdateInfoViewer();
			}

			this.LocatorFix();
			this.LocatorUpdateCommand();
		}

		protected void CloseModule()
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
			this.LocatorClose(mi.Module.ModuleInfo.Name);
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
		}

		protected bool AutoSave(CommandDispatcher dispatcher)
		{
			//	Fait tout ce qu'il faut pour éventuellement sauvegarder les ressources
			//	avant de passer à autre chose.
			//	Retourne false si on ne peut pas continuer.
			Common.Dialogs.DialogResult result = this.DialogSave(dispatcher);
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

		protected bool AutoSaveAll(CommandDispatcher dispatcher)
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

		public void UpdateBookModules()
		{
			//	Met à jour le nom de l'onglet des modules.
			if ( !this.IsCurrentModule )  return;
			TabPage tab = this.bookModules.Items[this.currentModule] as TabPage;
			tab.TabTitle = Misc.ExtractName(this.CurrentModule.ModuleInfo.Name, this.CurrentModule.IsDirty);
			this.bookModules.UpdateAfterChanges();
		}
		#endregion


		#region Dialogs
		public List<UI.ItemTableColumn> DlgTableConfiguration(Module baseModule, StructuredType structuredType, List<UI.ItemTableColumn> columns)
		{
			//	Ouvre le dialogue pour choisir les rubriques d'une table.
			this.dlgTableConfiguration.Initialise(baseModule, structuredType, columns);
			this.dlgTableConfiguration.Show();  // choix dans le dialogue...
			return this.dlgTableConfiguration.Columns;
		}

		public bool DlgBindingSelector(Module baseModule, StructuredType type, ObjectModifier.ObjectType objectType, ref Binding binding)
		{
			//	Ouvre le dialogue pour choisir une rubrique dans une structure de données.
			this.dlgBindingSelector.Initialise(baseModule, type, objectType, binding);
			this.dlgBindingSelector.Show();  // choix dans le dialogue...
			binding = this.dlgBindingSelector.SelectedBinding;
			return this.dlgBindingSelector.IsOk;
		}

		public Druid DlgResourceSelector(Module baseModule, ResourceAccess.Type type, TypeCode typeCode, Druid ressource, List<Druid> exclude, string includePrefix)
		{
			//	Ouvre le dialogue pour choisir une ressource (sous forme d'un Druid)
			//	d'un type à choix.
			//	Le type peut être inconnu ou la ressource inconnue, mais pas les deux.
			this.dlgResourceSelector.AccessOpen(baseModule, type, typeCode, ressource, exclude, includePrefix);
			this.dlgResourceSelector.Show();  // choix dans le dialogue...
			return this.dlgResourceSelector.AccessClose();
		}

		public List<Druid> DlgResourceSelector(Module baseModule, ResourceAccess.Type type, TypeCode typeCode, List<Druid> ressources, List<Druid> exclude, string includePrefix)
		{
			//	Ouvre le dialogue pour choisir des ressources (sous forme d'une liste
			//	de Druids) d'un type à choix.
			//	Le type doit être connu.
			this.dlgResourceSelector.AccessOpenList(baseModule, type, typeCode, ressources, exclude, includePrefix);
			this.dlgResourceSelector.Show();  // choix dans le dialogue...
			return this.dlgResourceSelector.AccessCloseList();
		}

		public string DlgResourceStructuredTypeField(StructuredType st, string field)
		{
			//	Ouvre le dialogue pour choisir un champ d'un type structuré.
			this.dlgResourceStructuredTypeField.Initialise(st, field);
			this.dlgResourceStructuredTypeField.Show();  // choix dans le dialogue...
			return this.dlgResourceStructuredTypeField.SelectedField;
		}

		public string DlgIcon(ResourceManager manager, string icon)
		{
			//	Ouvre le dialogue pour choisir une icône.
			ModuleInfo mi = this.CurrentModuleInfo;
			this.dlgIcon.SetResourceManager(manager, mi.Module.ModuleInfo.Name);
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

		public string DlgFieldName(string name)
		{
			//	Ouvre le dialogue pour choisir le nom d'un champ.
			this.dlgFieldName.Initialise(name);
			this.dlgFieldName.Show();  // choix dans le dialogue...
			return this.dlgFieldName.SelectedName;
		}

		public string DlgEntityComment(string text)
		{
			//	Ouvre le dialogue pour choisir le texte d'un commentaire.
			this.dlgEntityComment.Initialise(text);
			this.dlgEntityComment.Show();  // choix dans le dialogue...
			return this.dlgEntityComment.SelectedText;
		}

		public Dialogs.Search DialogSearch
		{
			get
			{
				return this.dlgSearch;
			}
		}

		protected void DialogSearchAdapt()
		{
			//	Adapte le dialogue de recherche en fonction du type du viewer actif.
			ModuleInfo mi = this.CurrentModuleInfo;
			if (mi == null)
			{
				this.dlgSearch.Adapt(ResourceAccess.Type.Unknow);
			}
			else
			{
				this.dlgSearch.Adapt(mi.Module.Modifier.ActiveResourceType);
			}
		}

		protected Common.Dialogs.DialogResult DialogSave(CommandDispatcher dispatcher)
		{
			//	Affiche le dialogue pour demander s'il faut enregistrer les
			//	ressources modifiées, avant de passer à d'autres ressources.
			if (!this.CurrentModule.IsDirty)
			{
				return Common.Dialogs.DialogResult.None;
			}

			string title = Res.Strings.Application.Title;
			string icon = "manifest:Epsitec.Common.Dialogs.Images.Question.icon";

			string question1 = string.Format(Res.Strings.Dialog.Save.Question1, this.CurrentModule.ModuleInfo.Name);
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
				string line = "____________________";
				message = string.Format("{0}<br/><br/><font color=\"#{3}\">{2}<br/><br/>{4}<br/>{2}</font><br/><br/>{1}", question1, question2, line, color, warning);
				icon = "manifest:Epsitec.Common.Dialogs.Images.Warning.icon";
			}

			Common.Dialogs.IDialog dialog = Common.Dialogs.Message.CreateYesNoCancel(title, icon, message, null, null, this.commandDispatcher);
			dialog.Owner = this.Window;
			dialog.OpenDialog();
			return dialog.Result;
		}

		public Common.Dialogs.DialogResult DialogQuestion(string question)
		{
			//	Affiche le dialogue pour signaler une erreur.
			if ( this.Window == null )  return Common.Dialogs.DialogResult.None;

			string title = Res.Strings.Application.Title;
			string icon = "manifest:Epsitec.Common.Dialogs.Images.Question.icon";
			string message = question;

			Common.Dialogs.IDialog dialog = Common.Dialogs.Message.CreateYesNo(title, icon, message, "", "", this.commandDispatcher);
			dialog.Owner = this.Window;
			dialog.OpenDialog();
			return dialog.Result;
		}

		public Common.Dialogs.DialogResult DialogQuestion(string question, string yes, string no, string cancel)
		{
			//	Affiche le dialogue pour signaler une erreur.
			if ( this.Window == null )  return Common.Dialogs.DialogResult.None;

			string title = Res.Strings.Application.Title;
			string icon = "manifest:Epsitec.Common.Dialogs.Images.Question.icon";
			string message = question;

			Common.Dialogs.IDialog dialog = Common.Dialogs.Message.CreateYesNoCancel(title, yes, no, cancel, icon, message, "", "", this.commandDispatcher);
			dialog.Owner = this.Window;
			dialog.OpenDialog();
			return dialog.Result;
		}

		public Common.Dialogs.DialogResult DialogMessage(string message)
		{
			//	Affiche le dialogue pour signaler une erreur.
			if ( this.Window == null )  return Common.Dialogs.DialogResult.None;

			string title = Res.Strings.Application.Title;
			string icon = "manifest:Epsitec.Common.Dialogs.Images.Information.icon";

			Common.Dialogs.IDialog dialog = Common.Dialogs.Message.CreateOk(title, icon, message, "", this.commandDispatcher);
			dialog.Owner = this.Window;
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
			string message = error;

			Common.Dialogs.IDialog dialog = Common.Dialogs.Message.CreateOk(title, icon, message, "", this.commandDispatcher);
			dialog.Owner = this.Window;
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

			if (sender == this.dlgFilter)
			{
				this.filterState.ActiveState = ActiveState.No;
			}
			
			if (sender == this.dlgSearch)
			{
				this.searchState.ActiveState = ActiveState.No;
			}
		}
		#endregion


		#region ModuleInfo class
		protected class ModuleInfo : System.IDisposable
		{
			public Module						Module;
			public TabPage						TabPage;
			public MyWidgets.BundleType			BundleTypeWidget;

			#region IDisposable Members
			public void Dispose()
			{
				if ( this.TabPage != null )  this.TabPage.Dispose();
			}
			#endregion
		}
		#endregion


		#region Instance
		public static MainWindow GetInstance(DependencyObject obj)
		{
			return (MainWindow) obj.GetValue(MainWindow.InstanceProperty);
		}

		public static void SetInstance(DependencyObject obj, MainWindow value)
		{
			obj.SetValue(MainWindow.InstanceProperty, value);
		}
		#endregion


		protected DesignerMode					mode;
		protected Window						window;
		protected CommandDispatcher				commandDispatcher;
		protected CommandContext				commandContext;
		protected RibbonBook					ribbonBook;
		protected RibbonPage					ribbonMain;
		protected RibbonPage					ribbonOper;
		protected RibbonPage					ribbonActive;
		protected TabBook						bookModules;
		protected StatusBar						info;
		protected ResizeKnob					resize;
		protected Dialogs.Open					dlgOpen;
		protected Dialogs.Glyphs				dlgGlyphs;
		protected Dialogs.Icon					dlgIcon;
		protected Dialogs.Filter				dlgFilter;
		protected Dialogs.Search				dlgSearch;
		protected Dialogs.NewCulture			dlgNewCulture;
		protected Dialogs.ResourceTypeCode		dlgResourceTypeCode;
		protected Dialogs.ResourceSelector		dlgResourceSelector;
		protected Dialogs.ResourceStructuredTypeField dlgResourceStructuredTypeField;
		protected Dialogs.BindingSelector		dlgBindingSelector;
		protected Dialogs.TableConfiguration	dlgTableConfiguration;
		protected Dialogs.FieldName				dlgFieldName;
		protected Dialogs.EntityComment			dlgEntityComment;
		protected PanelsContext					context;

		protected Support.ResourceManagerPool	resourceManagerPool;
		protected List<ModuleInfo>				moduleInfoList;
		protected int							lastModule = -1;
		protected int							currentModule = -1;
		protected double						ribbonHeight = 71;
		protected bool							ignoreChange = false;
		protected double						moveHorizontal = 5;
		protected double						moveVertical = 5;

		protected List<Viewers.Locator>			locators;
		protected int							locatorIndex;
		protected bool							locatorIgnore;

		protected CommandState					newState;
		protected CommandState					openState;
		protected CommandState					saveState;
		protected CommandState					saveAsState;
		protected CommandState					checkState;
		protected CommandState					closeState;
		protected CommandState					cutState;
		protected CommandState					copyState;
		protected CommandState					pasteState;
		protected CommandState					deleteState;
		protected CommandState					createState;
		protected CommandState					duplicateState;
		protected CommandState					copyToModuleState;
		protected CommandState					fontBoldState;
		protected CommandState					fontItalicState;
		protected CommandState					fontUnderlineState;
		protected CommandState					glyphsState;
		protected CommandState					filterState;
		protected CommandState					searchState;
		protected CommandState					searchPrevState;
		protected CommandState					searchNextState;
		protected CommandState					accessFirstState;
		protected CommandState					accessPrevState;
		protected CommandState					accessNextState;
		protected CommandState					accessLastState;
		protected CommandState					modificationAllState;
		protected CommandState					modificationClearState;
		protected CommandState					modificationPrevState;
		protected CommandState					modificationNextState;
		protected CommandState					newCultureState;
		protected CommandState					deleteCultureState;
		protected CommandState					toolSelectState;
		protected CommandState					toolGlobalState;
		protected CommandState					toolGridState;
		protected CommandState					toolEditState;
		protected CommandState					toolZoomState;
		protected CommandState					toolHandState;
		protected CommandState					objectHLineState;
		protected CommandState					objectVLineState;
		protected CommandState					objectSquareButtonState;
		protected CommandState					objectRectButtonState;
		protected CommandState					objectTableState;
		protected CommandState					objectTextState;
		protected CommandState					objectStaticState;
		protected CommandState					objectGroupState;
		protected CommandState					objectGroupFrameState;
		protected CommandState					objectGroupBoxState;
		protected CommandState					objectPanelState;
		protected CommandState					panelDeselectAllState;
		protected CommandState					panelSelectAllState;
		protected CommandState					panelSelectInvertState;
		protected CommandState					panelSelectRootState;
		protected CommandState					panelSelectParentState;
		protected CommandState					panelShowGridState;
		protected CommandState					panelShowZOrderState;
		protected CommandState					panelShowTabIndexState;
		protected CommandState					panelShowExpandState;
		protected CommandState					panelShowConstrainState;
		protected CommandState					panelShowAttachmentState;
		protected CommandState					panelRunState;
		protected CommandState					alignLeftState;
		protected CommandState					alignCenterXState;
		protected CommandState					alignRightState;
		protected CommandState					alignTopState;
		protected CommandState					alignCenterYState;
		protected CommandState					alignBottomState;
		protected CommandState					alignBaseLineState;
		protected CommandState					adjustWidthState;
		protected CommandState					adjustHeightState;
		protected CommandState					alignGridState;
		protected CommandState					moveLeftState;
		protected CommandState					moveRightState;
		protected CommandState					moveDownState;
		protected CommandState					moveUpState;
		protected CommandState					panelOrderUpAllState;
		protected CommandState					panelOrderDownAllState;
		protected CommandState					panelOrderUpOneState;
		protected CommandState					panelOrderDownOneState;
		protected CommandState					tabIndexClearState;
		protected CommandState					tabIndexFirstState;
		protected CommandState					tabIndexPrevState;
		protected CommandState					tabIndexNextState;
		protected CommandState					tabIndexLastState;
		protected CommandState					tabIndexRenumState;
		protected CommandState					locatorPrevState;
		protected CommandState					locatorNextState;
		protected CommandState					displayHorizontalState;
		protected CommandState					displayVerticalState;
		protected CommandState					displayFullScreenState;

		public static readonly DependencyProperty InstanceProperty = DependencyProperty.RegisterAttached("Instance", typeof(MainWindow), typeof(MainWindow));
	}
}
