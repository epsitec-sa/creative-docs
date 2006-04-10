using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer
{
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
			this.moduleList = new System.Collections.ArrayList();
		}
		
		public void Show()
		{
			//	Crée et montre la fenêtre de l'éditeur.
			if ( this.window == null )
			{
				this.window = new Window();

				this.window.Root.WindowStyles = WindowStyles.CanResize |
												WindowStyles.CanMinimize |
												WindowStyles.CanMaximize |
												WindowStyles.HasCloseButton;
				
				this.window.WindowSize = new Size(600, 400);
				this.window.Root.MinSize = new Size(400, 250);
				this.window.Text = Res.Strings.Application.Title;
				this.window.PreventAutoClose = true;
				this.window.WindowCloseClicked += new EventHandler(this.HandleWindowAboutCloseClicked);

#if false
				this.commandDispatcher = new CommandDispatcher("ResDesigner", CommandDispatcherLevel.Primary);
				this.commandDispatcher.RegisterController(this);
				this.commandDispatcher.Focus();
				this.window.AttachCommandDispatcher(this.commandDispatcher);
#endif

				this.hToolBar = new HToolBar(this.window.Root);
				this.hToolBar.Anchor = AnchorStyles.LeftAndRight | AnchorStyles.Top;

				this.ribbonMainButton = new RibbonButton("", Res.Strings.Ribbon.Main);
				this.ribbonMainButton.Size = this.ribbonMainButton.RequiredSize;
				this.ribbonMainButton.Pressed += new MessageEventHandler(this.HandleRibbonPressed);
				this.hToolBar.Items.Add(this.ribbonMainButton);

				this.ribbonOperButton = new RibbonButton("", Res.Strings.Ribbon.Oper);
				this.ribbonOperButton.Size = this.ribbonOperButton.RequiredSize;
				this.ribbonOperButton.Pressed += new MessageEventHandler(this.HandleRibbonPressed);
				this.hToolBar.Items.Add(this.ribbonOperButton);

				this.ribbonMain = new RibbonContainer(this.window.Root);
				this.ribbonMain.Name = "Main";
				this.ribbonMain.Height = this.ribbonHeight;
				this.ribbonMain.Anchor = AnchorStyles.LeftAndRight | AnchorStyles.Top;
				this.ribbonMain.Margins = new Margins(0, 0, this.hToolBar.Height, 0);
				this.ribbonMain.Visibility = true;
				this.ribbonMain.Items.Add(new Ribbons.File());
				this.ribbonMain.Items.Add(new Ribbons.Clipboard());

				this.ribbonOper = new RibbonContainer(this.window.Root);
				this.ribbonOper.Name = "Oper";
				this.ribbonOper.Height = this.ribbonHeight;
				this.ribbonOper.Anchor = AnchorStyles.LeftAndRight | AnchorStyles.Top;
				this.ribbonOper.Margins = new Margins(0, 0, this.hToolBar.Height, 0);
				this.ribbonOper.Visibility = true;

				this.ribbonActive = this.ribbonMain;
				this.ActiveRibbon(this.ribbonActive);
			}

			string[] modules = Resources.GetModuleNames(this.resourcePrefix);

			for ( int i=0 ; i<modules.Length ; i++ )
			{
				Module module = new Module(this.resourcePrefix, modules[i]);

				Viewer viewer = new Viewer(module);
				module.Modifier.AttachViewer(viewer);
				module.Modifier.ActiveViewer = viewer;
				
				this.moduleList.Add(module);
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


		protected void ActiveRibbon(RibbonContainer active)
		{
			//	Active un ruban.
			this.ribbonActive = active;

			//?this.SuspendLayout();
			this.ribbonMain.Visibility = (this.ribbonMain == this.ribbonActive);
			this.ribbonOper.Visibility = (this.ribbonOper == this.ribbonActive);

			this.ribbonMainButton.ActiveState = (this.ribbonMain == this.ribbonActive) ? ActiveState.Yes : ActiveState.No;
			this.ribbonOperButton.ActiveState = (this.ribbonOper == this.ribbonActive) ? ActiveState.Yes : ActiveState.No;

			double h = this.RibbonHeight;
			//?this.vToolBar.Margins = new Margins(0, 0, this.hToolBar.Height+h, this.info.Height);
			//?this.bookDocuments.Margins = new Margins(this.vToolBar.Width+1, this.panelsWidth+2, this.hToolBar.Height+h+1, this.info.Height+1);

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
			if ( ribbon == null )  return;

			this.ActiveRibbon(ribbon.IsVisible ? null : ribbon);
		}

		private void HandleWindowAboutCloseClicked(object sender)
		{
			this.window.Hide ();
		}


		protected Window						window;
		protected CommandDispatcher				commandDispatcher;
		protected HToolBar						hToolBar;
		protected RibbonButton					ribbonMainButton;
		protected RibbonButton					ribbonOperButton;
		protected RibbonContainer				ribbonMain;
		protected RibbonContainer				ribbonOper;
		protected RibbonContainer				ribbonActive;

		protected string						resourcePrefix;
		protected System.Collections.ArrayList	moduleList;
		protected double						ribbonHeight = 71;
	}
}
