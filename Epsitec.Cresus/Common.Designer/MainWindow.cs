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
			Res.Initialise (typeof (MainWindow), "Designer");
		}
		
		public void Show()
		{
			//	Crée et montre la fenêtre de l'éditeur.
			if (this.window == null)
			{
				this.window = new Window ();
				this.window.MakeFixedSizeWindow ();
				this.window.MakeSecondaryWindow ();
				this.window.WindowSize = new Size (400, 400);
				this.window.Text = "Ressources Editor";
				this.window.PreventAutoClose = true;
				this.window.WindowCloseClicked += new EventHandler (this.HandleWindowAboutCloseClicked);

				//	Bouton de fermeture.
				Button buttonClose = new Button (this.window.Root);
				buttonClose.PreferredWidth = 75;
				buttonClose.Text = "Close";
				buttonClose.ButtonStyle = ButtonStyle.DefaultAccept;
				buttonClose.Anchor = AnchorStyles.BottomLeft;
				buttonClose.Margins = new Margins (10, 0, 0, 10);
				buttonClose.Clicked += new MessageEventHandler (this.HandleAboutButtonCloseClicked);
				buttonClose.TabIndex = 1000;
				buttonClose.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			}

			string[] modules = Resources.GetModuleNames (this.resourcePrefix);

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
						
						System.Diagnostics.Debug.WriteLine (string.Format ("  Culture {0}, name '{1}'", bundle.Culture.Name, bundle.Name ?? "<null>"));
						System.Diagnostics.Debug.WriteLine (string.Format ("    About: '{0}'", bundle.About ?? "<null>"));
						System.Diagnostics.Debug.WriteLine (string.Format ("    {0} fields", bundle.FieldCount));
					}
				}
			}

			this.window.ShowDialog ();
		}


		private void HandleWindowAboutCloseClicked(object sender)
		{
			this.window.Hide ();
		}

		private void HandleAboutButtonCloseClicked(object sender, MessageEventArgs e)
		{
			this.window.Hide ();
		}


		protected Window window;
		protected string resourcePrefix = "file";
	}
}
