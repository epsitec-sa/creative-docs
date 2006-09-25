using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
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
	public class MainWindow
	{
		static MainWindow()
		{
			Res.Initialize(typeof(MainWindow), "Common.Designer");

			ImageProvider.Default.EnableLongLifeCache = true;
			ImageProvider.Default.PrefillManifestIconCache();
		}

		public MainWindow()
		{
			this.resourcePrefix = "file";
			this.resourceManagerPool = new ResourceManagerPool("Common.Designer");
			this.moduleInfoList = new List<ModuleInfo>();
			this.context = new PanelsContext();
		}

		public void Show(Window parent, DesignerMode mode)
		{
			//	Crée et montre la fenêtre de l'éditeur.
			this.mode = mode;

			if ( this.window == null )
			{
				this.window = new Window();

				this.window.Root.WindowStyles = WindowStyles.CanResize |
												WindowStyles.CanMinimize |
												WindowStyles.CanMaximize |
												WindowStyles.HasCloseButton;

				Point parentCenter = parent.WindowBounds.Center;
				this.window.WindowBounds = new Rectangle(parentCenter.X-1000/2, parentCenter.Y-700/2, 1000, 700);
				this.window.Root.MinSize = new Size(500, 400);
				this.window.Text = Res.Strings.Application.Title;
				this.window.Name = "Application";  // utilisé pour générer "QuitApplication" !
				this.window.PreventAutoClose = true;

				this.commandDispatcher = new CommandDispatcher("Common.Designer", CommandDispatcherLevel.Primary);
				this.commandContext = new CommandContext();
				this.commandDispatcher.RegisterController(this);
				
				CommandDispatcher.SetDispatcher(this.window, this.commandDispatcher);
				CommandContext.SetContext(this.window, this.commandContext);

				this.dlgOpen             = new Dialogs.Open(this);
				this.dlgGlyphs           = new Dialogs.Glyphs(this);
				this.dlgIcon             = new Dialogs.Icon(this);
				this.dlgFilter           = new Dialogs.Filter(this);
				this.dlgSearch           = new Dialogs.Search(this);
				this.dlgNewCulture       = new Dialogs.NewCulture(this);
				this.dlgResourceSelector = new Dialogs.ResourceSelector(this);

				this.dlgGlyphs.Closed += new EventHandler(this.HandleDlgClosed);
				this.dlgFilter.Closed += new EventHandler(this.HandleDlgClosed);
				this.dlgSearch.Closed += new EventHandler(this.HandleDlgClosed);

				this.InitCommands();
				this.CreateLayout();
			}

			//	Refait la liste des modules, puis ouvre tous ceux qui ont été trouvés.
			Resources.DefaultManager.RefreshModuleInfos(this.resourcePrefix);
			foreach (ResourceModuleInfo item in Resources.DefaultManager.GetModuleInfos(this.resourcePrefix))
			{
				Module module = new Module(this, this.mode, this.resourcePrefix, item);

				ModuleInfo mi = new ModuleInfo();
				mi.Module = module;
				this.moduleInfoList.Insert(++this.currentModule, mi);
				this.CreateModuleLayout();

				this.bookModules.ActivePage = mi.TabPage;
			}
			
			this.window.Show();

#if false
			for (int i = 0; i < modules.Length; i++)
			{
				System.Diagnostics.Debug.WriteLine (string.Format ("Module {0}: {1}", i, modules[i]));
				
				ResourceManager resourceManager = new ResourceManager ();

				resourceManager.SetupApplication (modules[i]);
				resourceManager.ActivePrefix = this.resourcePrefix;

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
			resourceManager.ActivePrefix = this.resourcePrefix;
			string[] ids = resourceManager.GetBundleIds("*", ResourceLevel.Default);

			ResourceBundleCollection bundles = new ResourceBundleCollection(resourceManager);
			bundles.LoadBundles(resourceManager.ActivePrefix, resourceManager.GetBundleIds(ids[0], ResourceLevel.All));

			this.bundle0 = bundles[0];
			this.bundle1 = bundles[1];

			this.window.ShowDialog();
			this.UpdateFields();
#endif
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
			this.ribbonMain.Items.Add(new Ribbons.Clipboard(this));
			this.ribbonMain.Items.Add(new Ribbons.Culture(this));
			if (this.mode == DesignerMode.Build)
			{
				this.ribbonMain.Items.Add(new Ribbons.Select(this));
			}
			this.ribbonMain.Items.Add(new Ribbons.Access(this));
			this.ribbonMain.Items.Add(new Ribbons.Character(this));

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
			if (module != null && module.Modifier.ActiveViewer!= null)
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
		#endregion


		#region Commands manager
		[Command("Open")]
		void CommandOpen(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.dlgOpen.SetResourcePrefix(this.resourcePrefix);
			this.dlgOpen.Show();

			ResourceModuleInfo item = this.dlgOpen.SelectedModule;
			if (item.Name != null)
			{
				Module module = new Module(this, this.mode, this.resourcePrefix, item);

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
			this.CurrentModule.Check();
		}

		[Command("Save")]
		void CommandSave(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentModule.Save();
		}

		[Command("Close")]
		void CommandClose(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			if ( !this.AutoSave(dispatcher) )  return;
			this.CloseModule();
		}

		[Command("QuitApplication")]
		void CommandQuitApplication(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			if ( !this.AutoSaveAll(dispatcher) )  return;

			this.dlgGlyphs.Hide();
			this.dlgFilter.Hide();
			this.dlgSearch.Hide();

			this.window.Hide();
	}

		[Command("Glyphs")]
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
			if ( !this.IsCurrentModule )  return;
			this.CurrentModule.Modifier.ActiveViewer.DoNewCulture();
		}

		[Command("DeleteCulture")]
		void CommandDeleteCulture(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			if ( !this.IsCurrentModule )  return;
			this.CurrentModule.Modifier.ActiveViewer.DoDeleteCulture();
		}

		[Command("Delete")]
		void CommandDelete(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			if ( !this.IsCurrentModule )  return;
			this.CurrentModule.Modifier.ActiveViewer.DoDelete();
		}

		[Command("Create")]
		void CommandCreate(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			if ( !this.IsCurrentModule )  return;
			this.CurrentModule.Modifier.ActiveViewer.DoDuplicate(false);
		}

		[Command("Duplicate")]
		void CommandDuplicate(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			if ( !this.IsCurrentModule )  return;
			this.CurrentModule.Modifier.ActiveViewer.DoDuplicate(true);
		}

		[Command("Up")]
		void CommandUp(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			if ( !this.IsCurrentModule )  return;
			this.CurrentModule.Modifier.ActiveViewer.DoMove(-1);
		}

		[Command("Down")]
		void CommandDown(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			if ( !this.IsCurrentModule )  return;
			this.CurrentModule.Modifier.ActiveViewer.DoMove(1);
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
		[Command("FontUnderlined")]
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
		[Command("ObjectButton")]
		[Command("ObjectText")]
		[Command("ObjectStatic")]
		[Command("ObjectGroup")]
		[Command("ObjectGroupBox")]
		void CommandTool(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			if ( !this.IsCurrentModule )  return;
			this.CurrentModule.Modifier.ActiveViewer.DoTool(e.Command.CommandId);
		}

		[Command("PanelDelete")]
		[Command("PanelDuplicate")]
		[Command("PanelDeselectAll")]
		[Command("PanelSelectAll")]
		[Command("PanelSelectInvert")]
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
		[Command("OrderUpAll")]
		[Command("OrderDownAll")]
		[Command("OrderUpOne")]
		[Command("OrderDownOne")]
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
			this.upState = this.CreateCommandState("Up");
			this.downState = this.CreateCommandState("Down");

			this.fontBoldState = this.CreateCommandState("FontBold", KeyCode.ModifierControl|KeyCode.AlphaB);
			this.fontItalicState = this.CreateCommandState("FontItalic", KeyCode.ModifierControl|KeyCode.AlphaI);
			this.fontUnderlinedState = this.CreateCommandState("FontUnderlined", KeyCode.ModifierControl|KeyCode.AlphaU);
			
			this.glyphsState = this.CreateCommandState("Glyphs");
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
			this.objectButtonState = this.CreateCommandState("ObjectButton", KeyCode.AlphaB);
			this.objectTextState = this.CreateCommandState("ObjectText", KeyCode.AlphaT);
			this.objectStaticState = this.CreateCommandState("ObjectStatic");
			this.objectGroupState = this.CreateCommandState("ObjectGroup");
			this.objectGroupBoxState = this.CreateCommandState("ObjectGroupBox");
			
			this.panelDeleteState = this.CreateCommandState("PanelDelete");
			this.panelDuplicateState = this.CreateCommandState("PanelDuplicate");
			this.panelDeselectAllState = this.CreateCommandState("PanelDeselectAll");
			this.panelSelectAllState = this.CreateCommandState("PanelSelectAll");
			this.panelSelectInvertState = this.CreateCommandState("PanelSelectInvert");

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

			this.orderUpAllState = this.CreateCommandState("OrderUpAll");
			this.orderDownAllState = this.CreateCommandState("OrderDownAll");
			this.orderUpOneState = this.CreateCommandState("OrderUpOne");
			this.orderDownOneState = this.CreateCommandState("OrderDownOne");

			this.tabIndexClearState = this.CreateCommandState("TabIndexClear");
			this.tabIndexFirstState = this.CreateCommandState("TabIndexFirst");
			this.tabIndexPrevState = this.CreateCommandState("TabIndexPrev");
			this.tabIndexNextState = this.CreateCommandState("TabIndexNext");
			this.tabIndexLastState = this.CreateCommandState("TabIndexLast");
			this.tabIndexRenumState = this.CreateCommandState("TabIndexRenum");
		}

		protected CommandState CreateCommandState(string commandName, params Widgets.Shortcut[] shortcuts)
		{
			//	Crée une nouvelle commande et son command state associé.
			
			Command command = Command.Get (commandName);

			if (command.IsReadWrite)
			{
				if (shortcuts.Length > 0)
				{
					command.Shortcuts.AddRange (shortcuts);
				}

				string iconName = commandName;
				string description = Res.Strings.GetString ("Action."+commandName);
				bool statefull = (commandName == "FontBold" || commandName == "FontItalic" || commandName == "FontUnderlined" || commandName.StartsWith("PanelShow"));

				command.ManuallyDefineCommand (description, Misc.Icon(iconName), null, statefull);
			}

			return this.CommandContext.GetCommandState (command);
		}
		#endregion


		#region Modules manager
		protected void CreateModuleLayout()
		{
			ModuleInfo mi = this.CurrentModuleInfo;

			//?mi.Module.CreateIds();

			mi.TabPage = new TabPage();
			mi.TabPage.TabTitle = mi.Module.ModuleInfo.Name;
			this.bookModules.Items.Insert(this.currentModule, mi.TabPage);

			mi.BundleType = new MyWidgets.BundleType(mi.TabPage);
			mi.BundleType.Dock = DockStyle.Top;
			mi.BundleType.TypeChanged += new EventHandler(this.HandleTypeChanged);

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

			ResourceAccess.Type type = mi.BundleType.CurrentType;
			Viewers.Abstract viewer = Viewers.Abstract.Create(type, mi.Module, this.context, mi.Module.GetAccess(type));

			if (viewer != null)
			{
				viewer.SetParent(mi.TabPage);
				viewer.Dock = DockStyle.Fill;
				mi.Module.Modifier.AttachViewer(viewer);
				mi.Module.Modifier.ActiveViewer = viewer;
			}
		}

		void HandleTypeChanged(object sender)
		{
			this.CreateViewerLayout();
			this.DialogSearchAdapt();
			this.CurrentModule.Modifier.ActiveViewer.UpdateCommands();
		}


		private void HandleBookModulesActivePageChanged(object sender)
		{
			//	L'onglet pour le module courant a été cliqué.
			if ( this.ignoreChange )  return;

			int total = this.bookModules.PageCount;
			for ( int i=0 ; i<total ; i++ )
			{
				ModuleInfo di = this.moduleInfoList[i];
				if ( di.TabPage == this.bookModules.ActivePage )
				{
					this.UseModule(i);
					return;
				}
			}
		}

		protected bool IsCurrentModule
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

		public List<ModuleInfo> OpeningListModule
		{
			//	Retourne la liste des modules ouverts.
			get
			{
				return this.moduleInfoList;
			}
		}

		public Module CurrentModule
		{
			//	Retourne le Module courant.
			get
			{
				if ( this.currentModule < 0 )  return null;
				return this.CurrentModuleInfo.Module;
			}
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

		protected void UseModule(int rank)
		{
			//	Utilise un module ouvert.
			if ( this.ignoreChange )  return;

			this.currentModule = rank;

			if ( rank >= 0 )
			{
				this.ignoreChange = true;
				this.bookModules.ActivePage = this.CurrentModuleInfo.TabPage;
				this.ignoreChange = false;

				this.DialogSearchAdapt();
				this.CurrentModule.Modifier.ActiveViewer.UpdateCommands();
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
				this.upState.Enable = false;
				this.downState.Enable = false;
				this.fontBoldState.Enable = false;
				this.fontItalicState.Enable = false;
				this.fontUnderlinedState.Enable = false;
				this.glyphsState.Enable = false;

				this.UpdateInfoCurrentModule();
				this.UpdateInfoAccess();
				this.UpdateInfoViewer();
			}
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
		//?public Druid DlgResourceSelector(ResourceAccess access, Druid ressource)
		public void DlgResourceSelector(ref Module module, ResourceAccess.Type type, ref Druid ressource)
		{
			//	Ouvre le dialogue pour choisir un ressource de type texte.
			this.dlgResourceSelector.SetAccess(module, type);
			this.dlgResourceSelector.Resource = ressource;

			this.dlgResourceSelector.Show();  // choix dans le dialogue...

			module = this.dlgResourceSelector.Module;
			ressource = this.dlgResourceSelector.Resource;
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
		public class ModuleInfo : System.IDisposable
		{
			public Module						Module;
			public TabPage						TabPage;
			public MyWidgets.BundleType			BundleType;

			#region IDisposable Members
			public void Dispose()
			{
				if ( this.TabPage != null )  this.TabPage.Dispose();
			}
			#endregion
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
		protected Dialogs.ResourceSelector		dlgResourceSelector;
		protected PanelsContext					context;

		protected string						resourcePrefix;
		protected Support.ResourceManagerPool	resourceManagerPool;
		protected List<ModuleInfo>				moduleInfoList;
		protected int							currentModule = -1;
		protected double						ribbonHeight = 71;
		protected bool							ignoreChange = false;
		protected double						moveHorizontal = 5;
		protected double						moveVertical = 5;

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
		protected CommandState					upState;
		protected CommandState					downState;
		protected CommandState					fontBoldState;
		protected CommandState					fontItalicState;
		protected CommandState					fontUnderlinedState;
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
		protected CommandState					objectButtonState;
		protected CommandState					objectTextState;
		protected CommandState					objectStaticState;
		protected CommandState					objectGroupState;
		protected CommandState					objectGroupBoxState;
		protected CommandState					panelDeleteState;
		protected CommandState					panelDuplicateState;
		protected CommandState					panelDeselectAllState;
		protected CommandState					panelSelectAllState;
		protected CommandState					panelSelectInvertState;
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
		protected CommandState					orderUpAllState;
		protected CommandState					orderDownAllState;
		protected CommandState					orderUpOneState;
		protected CommandState					orderDownOneState;
		protected CommandState					tabIndexClearState;
		protected CommandState					tabIndexFirstState;
		protected CommandState					tabIndexPrevState;
		protected CommandState					tabIndexNextState;
		protected CommandState					tabIndexLastState;
		protected CommandState					tabIndexRenumState;
	}
}
