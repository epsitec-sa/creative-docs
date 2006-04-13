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
			Res.Initialise(typeof(MainWindow), "Designer");

			ImageProvider.Default.EnableLongLifeCache = true;
			ImageProvider.Default.PrefillManifestIconCache();
		}

		public MainWindow()
		{
			this.resourcePrefix = "file";
			this.moduleInfoList = new List<ModuleInfo>();
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
				this.window.WindowBounds = new Rectangle(parentCenter.X-600/2, parentCenter.Y-500/2, 600, 500);
				this.window.Root.MinSize = new Size(500, 400);
				this.window.Text = Res.Strings.Application.Title;
				this.window.PreventAutoClose = true;
				this.window.AsyncNotification += new EventHandler(this.HandleWindowAsyncNotification);
				this.window.WindowCloseClicked += new EventHandler(this.HandleWindowCloseClicked);

				this.commandDispatcher = new CommandDispatcher("ResDesigner", CommandDispatcherLevel.Primary);
				this.commandDispatcher.RegisterController(this);
				this.commandDispatcher.Focus();
				this.window.Root.AttachCommandDispatcher(this.commandDispatcher);
				this.window.AttachCommandDispatcher(this.commandDispatcher);

				this.dlgGlyphs = new Dialogs.Glyphs(this);
				this.dlgFilter = new Dialogs.Filter(this);
				this.dlgSearch = new Dialogs.Search(this);

				this.dlgGlyphs.Closed += new EventHandler(this.HandleDlgClosed);
				this.dlgFilter.Closed += new EventHandler(this.HandleDlgClosed);
				this.dlgSearch.Closed += new EventHandler(this.HandleDlgClosed);

				this.InitCommands();
				this.CreateLayout();
			}

			//	Ouvre tous les modules trouvés.
			string[] modules = Resources.GetModuleNames(this.resourcePrefix);

			for ( int i=0 ; i<modules.Length ; i++ )
			{
				Module module = new Module(this.resourcePrefix, modules[i]);

				ModuleInfo mi = new ModuleInfo();
				mi.Module = module;
				this.moduleInfoList.Insert(++this.currentModule, mi);
				this.CreateModuleLayout();
				this.ConnectEvents();

				this.bookModules.ActivePage = mi.TabPage;
			}

			this.window.ShowDialog();

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


		protected void CreateLayout()
		{
			this.hToolBar = new HToolBar(this.window.Root);
			this.hToolBar.Anchor = AnchorStyles.LeftAndRight | AnchorStyles.Top;

			this.ribbonMainButton = new RibbonButton("", Res.Strings.Ribbon.Main);
			this.ribbonMainButton.Size = this.ribbonMainButton.RequiredSize;
			this.ribbonMainButton.Pressed += new MessageEventHandler(this.HandleRibbonPressed);
			this.hToolBar.Items.Add(this.ribbonMainButton);

			this.ribbonOperButton = new RibbonButton("", Res.Strings.Ribbon.Oper);
			this.ribbonOperButton.Size = this.ribbonOperButton.RequiredSize;
			this.ribbonOperButton.Pressed += new MessageEventHandler(this.HandleRibbonPressed);
			//?this.hToolBar.Items.Add(this.ribbonOperButton);

			this.ribbonTextButton = new RibbonButton("", Res.Strings.Ribbon.Text);
			this.ribbonTextButton.Size = this.ribbonTextButton.RequiredSize;
			this.ribbonTextButton.Pressed += new MessageEventHandler(this.HandleRibbonPressed);
			this.hToolBar.Items.Add(this.ribbonTextButton);

			this.ribbonMain = new RibbonContainer(this.window.Root);
			this.ribbonMain.Name = "Main";
			this.ribbonMain.Height = this.ribbonHeight;
			this.ribbonMain.Anchor = AnchorStyles.LeftAndRight | AnchorStyles.Top;
			this.ribbonMain.Margins = new Margins(0, 0, this.hToolBar.Height, 0);
			this.ribbonMain.Visibility = true;
			this.ribbonMain.Items.Add(new Ribbons.File());
			this.ribbonMain.Items.Add(new Ribbons.Access());
			this.ribbonMain.Items.Add(new Ribbons.Select());
			this.ribbonMain.Items.Add(new Ribbons.Clipboard());

			this.ribbonOper = new RibbonContainer(this.window.Root);
			this.ribbonOper.Name = "Oper";
			this.ribbonOper.Height = this.ribbonHeight;
			this.ribbonOper.Anchor = AnchorStyles.LeftAndRight | AnchorStyles.Top;
			this.ribbonOper.Margins = new Margins(0, 0, this.hToolBar.Height, 0);
			this.ribbonOper.Visibility = true;

			this.ribbonText = new RibbonContainer(this.window.Root);
			this.ribbonText.Name = "Text";
			this.ribbonText.Height = this.ribbonHeight;
			this.ribbonText.Anchor = AnchorStyles.LeftAndRight | AnchorStyles.Top;
			this.ribbonText.Margins = new Margins(0, 0, this.hToolBar.Height, 0);
			this.ribbonText.Visibility = true;
			this.ribbonText.Items.Add(new Ribbons.Character());
			this.ribbonText.Items.Add(new Ribbons.Clipboard());

			this.info = new StatusBar(this.window.Root);
			this.info.Anchor = AnchorStyles.LeftAndRight | AnchorStyles.Bottom;
			this.info.Margins = new Margins(0, 22-5, 0, 0);

			this.InfoAdd("InfoCurrentModule", 200);
			this.InfoAdd("InfoAccess", 120);

			StatusBar infoMisc = new StatusBar(this.window.Root);
			infoMisc.Width = 22;
			infoMisc.Anchor = AnchorStyles.BottomRight;
			infoMisc.Margins = new Margins(0, 0, 0, 0);

			IconSeparator sep = new IconSeparator(infoMisc);
			sep.Height = infoMisc.Height-1.0;
			sep.Anchor = AnchorStyles.BottomLeft;

			this.resize = new ResizeKnob(infoMisc);
			this.resize.Anchor = AnchorStyles.BottomRight;
			ToolTip.Default.SetToolTip(this.resize, Res.Strings.Dialog.Tooltip.Resize);

			this.bookModules = new TabBook(this.window.Root);
			this.bookModules.Anchor = AnchorStyles.All;
			this.bookModules.Margins = new Margins(0, 0, this.hToolBar.Height+1, this.info.Height+1);
			this.bookModules.Arrows = TabBookArrows.Right;
			this.bookModules.HasCloseButton = true;
			this.bookModules.CloseButton.Command = "Close";
			this.bookModules.ActivePageChanged += new EventHandler(this.HandleBookModulesActivePageChanged);
			ToolTip.Default.SetToolTip(this.bookModules.CloseButton, Res.Strings.Action.Close);

			this.ribbonActive = this.ribbonMain;
			this.ActiveRibbon(this.ribbonActive);
		}


		#region Info manager
		protected StatusField InfoAdd(string name, double width)
		{
			StatusField field = new StatusField();
			field.Width = width;
			this.info.Items.Add(field);

			int i = this.info.Children.Count-1;
			this.info.Items[i].Name = name;
			return field;
		}

		protected void UpdateInfoCurrentModule()
		{
			System.Text.StringBuilder builder = new System.Text.StringBuilder();

			ModuleInfo mi = this.CurrentModuleInfo;
			if ( mi != null )
			{
				ResourceBundleCollection bundles = mi.Module.Bundles;

				for ( int b=0 ; b<bundles.Count ; b++ )
				{
					ResourceBundle bundle = bundles[b];

					if ( b > 0 )
					{
						builder.Append(", ");
					}

					builder.Append(bundle.Culture.Name);
					builder.Append(":");
					builder.Append(bundle.FieldCount);
				}
			}

			StatusField field = this.info.Items["InfoCurrentModule"] as StatusField;
			field.Text = builder.ToString();
			field.Invalidate();
		}

		protected void UpdateInfoAccess()
		{
			string text = "";
			Module module = this.CurrentModule;
			if (module != null)
			{
				text = module.Modifier.ActiveViewer.InfoAccessText;
			}
			StatusField field = this.info.Items["InfoAccess"] as StatusField;
			field.Text = text;
			field.Invalidate();
		}
		#endregion


		protected void ConnectEvents()
		{
			//	On s'enregistre auprès du module pour tous les événements.
			this.CurrentModule.Notifier.ModeChanged       += new SimpleEventHandler(this.HandleModeChanged);
			this.CurrentModule.Notifier.SaveChanged       += new SimpleEventHandler(this.HandleSaveChanged);
			this.CurrentModule.Notifier.InfoAccessChanged += new SimpleEventHandler(this.HandleInfoAccessChanged);
		}

		private void HandleModeChanged()
		{
			//	Appelé lorsque le mode de fonctionnement a changé.
			if (this.IsCurrentModule)
			{
				bool build = (this.mode == DesignerMode.Build);
				this.deleteState.Enable = build;
				this.duplicateState.Enable = build;
				this.upState.Enable = build;
				this.downState.Enable = build;
			}
			else
			{
				this.deleteState.Enable = false;
				this.duplicateState.Enable = false;
				this.upState.Enable = false;
				this.downState.Enable = false;
			}
		}

		private void HandleSaveChanged()
		{
			//	Appelé par le module lorsque l'état "enregistrer" a changé.
			if (this.IsCurrentModule)
			{
				bool isDirty = this.CurrentModule.Modifier.IsDirty;
				this.saveState.Enable = isDirty;
				this.saveAsState.Enable = isDirty;
				this.UpdateBookModules();
			}
			else
			{
				this.saveState.Enable = false;
				this.saveAsState.Enable = false;
			}
		}

		private void HandleInfoAccessChanged()
		{
			//	Appelé par le module lorsque l'information sur l'accès a changé.
			this.UpdateInfoAccess();
		}


		#region Commands manager
		[Command("Save")]
		void CommandSave(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.CurrentModule.Save();
		}

		[Command("Close")]
		void CommandClose(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			//?if ( !this.AutoSave(dispatcher) )  return;
			this.CloseModule();
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

		[Command("AccessFirst")]
		[Command("AccessPrev")]
		[Command("AccessNext")]
		[Command("AccessLast")]
		void CommandAccess(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			if ( !this.IsCurrentModule )  return;
			this.CurrentModule.Modifier.ActiveViewer.DoAccess(e.CommandName);
		}

		[Command("Delete")]
		void CommandDelete(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			if ( !this.IsCurrentModule )  return;
			this.CurrentModule.Modifier.ActiveViewer.DoDelete();
		}

		[Command("Duplicate")]
		void CommandDuplicate(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			if ( !this.IsCurrentModule )  return;
			this.CurrentModule.Modifier.ActiveViewer.DoDuplicate();
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

		protected void InitCommands()
		{
			this.newState = this.CreateCommandState("New", KeyCode.ModifierControl|KeyCode.AlphaN);
			this.openState = this.CreateCommandState("Open", KeyCode.ModifierControl|KeyCode.AlphaO);
			this.saveState = this.CreateCommandState("Save", KeyCode.ModifierControl|KeyCode.AlphaS);
			this.saveAsState = this.CreateCommandState("SaveAs");
			this.closeState = this.CreateCommandState("Close", KeyCode.ModifierControl|KeyCode.FuncF4);
			this.cutState = this.CreateCommandState("Cut", KeyCode.ModifierControl|KeyCode.AlphaX);
			this.copyState = this.CreateCommandState("Copy", KeyCode.ModifierControl|KeyCode.AlphaC);
			this.pasteState = this.CreateCommandState("Paste", KeyCode.ModifierControl|KeyCode.AlphaV);
			this.deleteState = this.CreateCommandState("Delete", KeyCode.Delete);
			this.duplicateState = this.CreateCommandState("Duplicate", KeyCode.ModifierControl|KeyCode.AlphaD);
			this.upState = this.CreateCommandState("Up");
			this.downState = this.CreateCommandState("Down");
			this.fontBoldState = this.CreateCommandState("FontBold", KeyCode.ModifierControl|KeyCode.AlphaB);
			this.fontItalicState = this.CreateCommandState("FontItalic", KeyCode.ModifierControl|KeyCode.AlphaI);
			this.fontUnderlinedState = this.CreateCommandState("FontUnderlined", KeyCode.ModifierControl|KeyCode.AlphaU);
			this.glyphsState = this.CreateCommandState("Glyphs");
			this.filterState = this.CreateCommandState("Filter");
			this.searchState = this.CreateCommandState("Search");
			this.accessFirstState = this.CreateCommandState("AccessFirst");
			this.accessPrevState = this.CreateCommandState("AccessPrev");
			this.accessNextState = this.CreateCommandState("AccessNext");
			this.accessLastState = this.CreateCommandState("AccessLast");
		}

		protected CommandState CreateCommandState(string command, params Widgets.Shortcut[] shortcuts)
		{
			//	Crée un nouveau CommandState.
			CommandState cs = new CommandState(command, this.commandDispatcher, shortcuts);

			cs.IconName    = command;
			cs.LongCaption = Res.Strings.GetString("Action."+command);

			return cs;
		}
		#endregion


		#region Modules manager
		protected void CreateModuleLayout()
		{
			ModuleInfo mi = this.CurrentModuleInfo;

			mi.TabPage = new TabPage();
			mi.TabPage.TabTitle = mi.Module.Name;
			this.bookModules.Items.Insert(this.currentModule, mi.TabPage);

			Viewer viewer = new Viewer(mi.Module);
			viewer.SetParent(mi.TabPage);
			viewer.Anchor = AnchorStyles.All;
			mi.Module.Modifier.AttachViewer(viewer);
			mi.Module.Modifier.ActiveViewer = viewer;
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

		public Module CurrentModule
		{
			//	Retourne le Module courant.
			get
			{
				if ( this.currentModule < 0 )  return null;
				return this.CurrentModuleInfo.Module;
			}
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

				this.UpdateInfoCurrentModule();
				this.CurrentModule.Notifier.NotifyAllChanged();
			}
			else
			{
				this.UpdateInfoCurrentModule();
				this.HandleSaveChanged();
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
				this.ActiveRibbon(this.ribbonMain);
			}
		}

		protected void UpdateBookModules()
		{
			//	Met à jour le nom de l'onglet des modules.
			if ( !this.IsCurrentModule )  return;
			TabPage tab = this.bookModules.Items[this.currentModule] as TabPage;
			tab.TabTitle = Misc.ExtractName(this.CurrentModule.Name, this.CurrentModule.Modifier.IsDirty);
			this.bookModules.UpdateAfterChanges();
		}
		#endregion


		#region Ribbons manager
		protected void ActiveRibbon(RibbonContainer active)
		{
			//	Active un ruban.
			this.ribbonActive = active;

			//?this.SuspendLayout();
			this.ribbonMain.Visibility = (this.ribbonMain == this.ribbonActive);
			this.ribbonOper.Visibility = (this.ribbonOper == this.ribbonActive);
			this.ribbonText.Visibility = (this.ribbonText == this.ribbonActive);

			this.ribbonMainButton.ActiveState = (this.ribbonMain == this.ribbonActive) ? ActiveState.Yes : ActiveState.No;
			this.ribbonOperButton.ActiveState = (this.ribbonOper == this.ribbonActive) ? ActiveState.Yes : ActiveState.No;
			this.ribbonTextButton.ActiveState = (this.ribbonText == this.ribbonActive) ? ActiveState.Yes : ActiveState.No;

			double h = this.RibbonHeight;
			this.bookModules.Margins = new Margins(1, 2, this.hToolBar.Height+h+1, this.info.Height+1);

			//?this.ResumeLayout();
		}

		protected double RibbonHeight
		{
			//	Retourne la hauteur utilisée par les rubans.
			get
			{
				return (this.ribbonActive == null) ? 0 : this.ribbonHeight;
			}
		}

		private void HandleRibbonPressed(object sender, MessageEventArgs e)
		{
			//	Le bouton pour activer/désactiver un ruban a été cliqué.
			RibbonButton button = sender as RibbonButton;
			RibbonContainer ribbon = null;
			if ( button == this.ribbonMainButton )  ribbon = this.ribbonMain;
			if ( button == this.ribbonOperButton )  ribbon = this.ribbonOper;
			if ( button == this.ribbonTextButton )  ribbon = this.ribbonText;
			if ( ribbon == null )  return;

			this.ActiveRibbon(ribbon.IsVisible ? null : ribbon);
		}
		#endregion


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

		private void HandleWindowAsyncNotification(object sender)
		{
			if ( this.currentModule < 0 )  return;
			this.CurrentModule.Notifier.GenerateEvents();
		}

		private void HandleWindowCloseClicked(object sender)
		{
			this.dlgGlyphs.Hide();
			this.dlgFilter.Hide();
			this.dlgSearch.Hide();

			this.window.Hide();
		}


		#region ModuleInfo class
		protected class ModuleInfo
		{
			public Module						Module;
			public TabPage						TabPage;

			public void Dispose()
			{
				//?if ( this.TabPage != null )  this.TabPage.Dispose();
			}
		}
		#endregion


		protected DesignerMode					mode;
		protected Window						window;
		protected CommandDispatcher				commandDispatcher;
		protected HToolBar						hToolBar;
		protected RibbonButton					ribbonMainButton;
		protected RibbonButton					ribbonOperButton;
		protected RibbonButton					ribbonTextButton;
		protected RibbonContainer				ribbonMain;
		protected RibbonContainer				ribbonOper;
		protected RibbonContainer				ribbonText;
		protected RibbonContainer				ribbonActive;
		protected TabBook						bookModules;
		protected StatusBar						info;
		protected ResizeKnob					resize;
		protected Dialogs.Glyphs				dlgGlyphs;
		protected Dialogs.Filter				dlgFilter;
		protected Dialogs.Search				dlgSearch;

		protected string						resourcePrefix;
		protected List<ModuleInfo>				moduleInfoList;
		protected int							currentModule = -1;
		protected double						ribbonHeight = 71;
		protected bool							ignoreChange = false;

		protected CommandState					newState;
		protected CommandState					openState;
		protected CommandState					saveState;
		protected CommandState					saveAsState;
		protected CommandState					closeState;
		protected CommandState					cutState;
		protected CommandState					copyState;
		protected CommandState					pasteState;
		protected CommandState					deleteState;
		protected CommandState					duplicateState;
		protected CommandState					upState;
		protected CommandState					downState;
		protected CommandState					fontBoldState;
		protected CommandState					fontItalicState;
		protected CommandState					fontUnderlinedState;
		protected CommandState					glyphsState;
		protected CommandState					filterState;
		protected CommandState					searchState;
		protected CommandState					accessFirstState;
		protected CommandState					accessPrevState;
		protected CommandState					accessNextState;
		protected CommandState					accessLastState;
	}
}
