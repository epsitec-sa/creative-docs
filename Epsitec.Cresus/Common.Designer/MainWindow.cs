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
	/// Fen�tre principale de l'�diteur de ressources.
	/// </summary>
	public class MainWindow : DependencyObject
	{
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
			this.moduleInfoList = new List<ModuleInfo> ();
			this.context = new PanelsContext ();
		}

		public void Show(Window parentWindow)
		{
			//	Cr�e et montre la fen�tre de l'�diteur.
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
				this.window.Name = "Application";  // utilis� pour g�n�rer "QuitApplication" !
				this.window.PreventAutoClose = true;
				
				MainWindow.SetInstance(this.window, this);  // attache l'instance de MainWindow � la fen�tre

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
				this.dlgResourceTypeType            = new Dialogs.ResourceTypeType(this);
				this.dlgResourceSelector            = new Dialogs.ResourceSelector(this);
				this.dlgResourceStructuredTypeField = new Dialogs.ResourceStructuredTypeField(this);
				this.dlgBindingSelector             = new Dialogs.BindingSelector(this);
				this.dlgTableConfiguration          = new Dialogs.TableConfiguration(this);

				this.dlgGlyphs.Closed += new EventHandler(this.HandleDlgClosed);
				this.dlgFilter.Closed += new EventHandler(this.HandleDlgClosed);
				this.dlgSearch.Closed += new EventHandler(this.HandleDlgClosed);

				this.InitCommands();
				this.CreateLayout();
			}

			//	Liste les modules, puis ouvre tous ceux qui ont �t� trouv�s.
			Resources.DefaultManager.RefreshModuleInfos(this.resourceManagerPool.DefaultPrefix);
			if (this.moduleInfoList.Count == 0)
			{
				foreach (ResourceModuleInfo item in Resources.DefaultManager.GetModuleInfos (this.resourceManagerPool.DefaultPrefix))
				{
					Module module = new Module (this, this.mode, this.resourceManagerPool.DefaultPrefix, item);

					ModuleInfo mi = new ModuleInfo ();
					mi.Module = module;
					this.moduleInfoList.Insert (++this.currentModule, mi);
					this.CreateModuleLayout ();

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
			//	Retourne la fen�tre principale de l'application.
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

			//	Cr�e le ruban principal.
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

			//	Cr�e le ruban des op�rations.
			this.ribbonOper = new RibbonPage();
			this.ribbonOper.RibbonTitle = Res.Strings.Ribbon.Oper;
			this.ribbonBook.Items.Add(this.ribbonOper);

			this.ribbonOper.Items.Add(new Ribbons.PanelShow(this));
			this.ribbonOper.Items.Add(new Ribbons.PanelSelect(this));
			this.ribbonOper.Items.Add(new Ribbons.Move(this));
			this.ribbonOper.Items.Add(new Ribbons.Align(this));
			this.ribbonOper.Items.Add(new Ribbons.Order(this));
			this.ribbonOper.Items.Add(new Ribbons.TabIndex(this));

			//	Cr�e la barre de statuts.
			this.info = new StatusBar(this.window.Root);
			this.info.Dock = DockStyle.Bottom;
			this.info.Margins = new Margins(0, 0, 0, 0);

			this.InfoAdd("InfoCurrentModule", 250);
			this.InfoAdd("InfoAccess", 250);
			this.InfoAdd("InfoViewer", 300);

			this.resize = new ResizeKnob();
			this.resize.Margins = new Margins(2, 0, 0, 0);
			this.info.Items.Add(this.resize);
			this.resize.Dock = DockStyle.Right;  // doit �tre fait apr�s le Items.Add !
			ToolTip.Default.SetToolTip(this.resize, Res.Strings.Dialog.Tooltip.Resize);

			//	Cr�e le TabBook principal pour les modules ouverts.
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
			//	Cherche le bouton utilis� pour une commande, dans tous les rubans.
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
			//	Cherche le bouton utilis� pour une commande, dans un ruban.
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

		public void UpdateViewer(MyWidgets.PanelEditor.Changing oper)
		{
			//	Met � jour le visualisateur en cours.
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
			this.dlgOpen.SetResourcePrefix (this.resourceManagerPool.DefaultPrefix);
			this.dlgOpen.Show();

			ResourceModuleInfo item = this.dlgOpen.SelectedModule;
			if (item.Name != null)
			{
				Module module = new Module (this, this.mode, this.resourceManagerPool.DefaultPrefix, item);

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

		[Command(ApplicationCommands.Id.Quit)]
		[Command("QuitApplication")]
		void CommandQuitApplication(CommandDispatcher dispatcher, CommandEventArgs e)
		{
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

		[Command("CopyToModule")]
		void CommandCopyToModule(CommandDispatcher dispatcher, CommandEventArgs e)
		{
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
		}

		protected CommandState CreateCommandState(string commandName, params Widgets.Shortcut[] shortcuts)
		{
			//	Cr�e une nouvelle commande et son command state associ�.
			
			Command command = Command.Get (commandName);

			if (command.IsReadWrite)
			{
				if (shortcuts.Length > 0)
				{
					command.Shortcuts.AddRange (shortcuts);
				}

				string iconName = commandName;
				string description = Res.Strings.GetString ("Action."+commandName);
				bool statefull = (commandName == "FontBold" || commandName == "FontItalic" || commandName == "FontUnderline" || commandName.StartsWith("PanelShow"));

				command.ManuallyDefineCommand (description, Misc.Icon(iconName), null, statefull);
			}

			return this.CommandContext.GetCommandState (command);
		}
		#endregion


		#region Modules manager
		protected void CreateModuleLayout()
		{
			ModuleInfo mi = this.CurrentModuleInfo;

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
			Viewers.Abstract viewer = Viewers.Abstract.Create(type, mi.Module, this.context, mi.Module.GetAccess(type), this);

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
			//	L'onglet pour le module courant a �t� cliqu�.
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
			//	Retourne le module pr�c�demment s�lectionn�.
			get
			{
				if ( this.lastModule < 0 || this.lastModule >= this.moduleInfoList.Count )  return null;
				if ( this.lastModule == this.currentModule )  return null;
				return this.moduleInfoList[this.lastModule].Module;
			}
		}

		public Module SearchModule(Druid druid)
		{
			//	Cherche � quel module appartient un druid.
			if (druid.IsEmpty)
			{
				return null;
			}

			return this.SearchModuleId(druid.Module);
		}

		public Module SearchModuleId(int id)
		{
			//	Cherche un module d'apr�s son identificateur.
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
			//	Fait tout ce qu'il faut pour �ventuellement sauvegarder les ressources
			//	avant de passer � autre chose.
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
			//	Fait tout ce qu'il faut pour �ventuellement sauvegarder toutes les
			//	ressources avant de passer � autre chose.
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
			//	Met � jour le nom de l'onglet des modules.
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
			//	Ouvre le dialogue pour choisir une rubrique dans une structure de donn�es.
			this.dlgBindingSelector.Initialise(baseModule, type, objectType, binding);
			this.dlgBindingSelector.Show();  // choix dans le dialogue...
			binding = this.dlgBindingSelector.SelectedBinding;
			return this.dlgBindingSelector.IsOk;
		}

		public Druid DlgResourceSelector(Module baseModule, ResourceAccess.Type type, ResourceAccess.TypeType typeType, Druid ressource, List<Druid> exclude, string includePrefix)
		{
			//	Ouvre le dialogue pour choisir une ressource (sous forme d'un Druid)
			//	d'un type � choix.
			//	Le type peut �tre inconnu ou la ressource inconnue, mais pas les deux.
			this.dlgResourceSelector.AccessOpen(baseModule, type, typeType, ressource, exclude, includePrefix);
			this.dlgResourceSelector.Show();  // choix dans le dialogue...
			return this.dlgResourceSelector.AccessClose();
		}

		public List<Druid> DlgResourceSelector(Module baseModule, ResourceAccess.Type type, ResourceAccess.TypeType typeType, List<Druid> ressources, List<Druid> exclude, string includePrefix)
		{
			//	Ouvre le dialogue pour choisir des ressources (sous forme d'une liste
			//	de Druids) d'un type � choix.
			//	Le type doit �tre connu.
			this.dlgResourceSelector.AccessOpenList(baseModule, type, typeType, ressources, exclude, includePrefix);
			this.dlgResourceSelector.Show();  // choix dans le dialogue...
			return this.dlgResourceSelector.AccessCloseList();
		}

		public string DlgResourceStructuredTypeField(StructuredType st, string field)
		{
			//	Ouvre le dialogue pour choisir un champ d'un type structur�.
			this.dlgResourceStructuredTypeField.Initialise(st, field);
			this.dlgResourceStructuredTypeField.Show();  // choix dans le dialogue...
			return this.dlgResourceStructuredTypeField.SelectedField;
		}

		public string DlgIcon(ResourceManager manager, string icon)
		{
			//	Ouvre le dialogue pour choisir une ic�ne.
			ModuleInfo mi = this.CurrentModuleInfo;
			this.dlgIcon.SetResourceManager(manager, mi.Module.ModuleInfo.Name);
			this.dlgIcon.IconValue = icon;
			this.dlgIcon.Show();
			return this.dlgIcon.IconValue;
		}

		public string DlgNewCulture(ResourceAccess access)
		{
			//	Ouvre le dialogue pour choisir la culture � cr�er.
			this.dlgNewCulture.SetAccess(access);
			this.dlgNewCulture.Show();
			return this.dlgNewCulture.Culture;
		}

		public void DlgResourceTypeType(ResourceAccess access, ref ResourceAccess.TypeType type, out System.Type stype)
		{
			//	Ouvre le dialogue pour choisir le type d'un Caption.Type.
			this.dlgResourceTypeType.ResourceAccess = access;
			this.dlgResourceTypeType.ContentType = type;
			this.dlgResourceTypeType.Show();
			type = this.dlgResourceTypeType.ContentType;
			stype = this.dlgResourceTypeType.SystemType;
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
			//	ressources modifi�es, avant de passer � d'autres ressources.
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
			//	Un dialogue a �t� ferm�.
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
			public MyWidgets.BundleType			BundleType;

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
		protected Dialogs.ResourceTypeType		dlgResourceTypeType;
		protected Dialogs.ResourceSelector		dlgResourceSelector;
		protected Dialogs.ResourceStructuredTypeField dlgResourceStructuredTypeField;
		protected Dialogs.BindingSelector		dlgBindingSelector;
		protected Dialogs.TableConfiguration	dlgTableConfiguration;
		protected PanelsContext					context;

		protected Support.ResourceManagerPool	resourceManagerPool;
		protected List<ModuleInfo>				moduleInfoList;
		protected int							lastModule = -1;
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

		public static readonly DependencyProperty InstanceProperty = DependencyProperty.RegisterAttached("Instance", typeof(MainWindow), typeof(MainWindow));
	}
}
