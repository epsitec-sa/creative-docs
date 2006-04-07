using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer
{
	/// <summary>
	/// Fen�tre principale de l'�diteur de ressources.
	/// </summary>
	public class MainWindow
	{
		static MainWindow()
		{
			Res.Initialise (typeof (MainWindow), "Designer");
		}
		
		public void Show()
		{
			//	Cr�e et montre la fen�tre de l'�diteur.
			if ( this.window == null )
			{
				this.window = new Window();
				this.window.MakeFixedSizeWindow();
				this.window.MakeSecondaryWindow();
				this.window.WindowSize = new Size(400, 400);
				this.window.Text = "Ressources Editor";
				this.window.PreventAutoClose = true;
				this.window.WindowCloseClicked += new EventHandler(this.HandleWindowAboutCloseClicked);

				this.fieldRank = new TextField(this.window.Root);
				this.fieldRank.Width = 30;
				this.fieldRank.Anchor = AnchorStyles.TopLeft;
				this.fieldRank.Margins = new Margins(10, 0, 10, 0);

				this.fieldName = new TextField(this.window.Root);
				this.fieldName.Width = 200;
				this.fieldName.Anchor = AnchorStyles.TopLeft;
				this.fieldName.Margins = new Margins(10, 0, 10+21, 0);

				this.fieldString = new TextField(this.window.Root);
				this.fieldString.Width = 200;
				this.fieldString.Anchor = AnchorStyles.TopLeft;
				this.fieldString.Margins = new Margins(10, 0, 10+21+21, 0);

				this.buttonPrev = new GlyphButton(this.window.Root);
				this.buttonPrev.Name = "Prev";
				this.buttonPrev.Width = 21;
				this.buttonPrev.Height = 21;
				this.buttonPrev.GlyphShape = GlyphShape.ArrowUp;
				this.buttonPrev.Anchor = AnchorStyles.TopLeft;
				this.buttonPrev.Margins = new Margins(10+200+5, 0, 10+21, 0);
				this.buttonPrev.Clicked += new MessageEventHandler(this.HandleButtonClicked);

				this.buttonNext = new GlyphButton(this.window.Root);
				this.buttonNext.Name = "Next";
				this.buttonNext.Width = 21;
				this.buttonNext.Height = 21;
				this.buttonNext.GlyphShape = GlyphShape.ArrowDown;
				this.buttonNext.Anchor = AnchorStyles.TopLeft;
				this.buttonNext.Margins = new Margins(10+200+5, 0, 10+21+21, 0);
				this.buttonNext.Clicked += new MessageEventHandler(this.HandleButtonClicked);

				//	Bouton de fermeture.
				Button buttonClose = new Button(this.window.Root);
				buttonClose.PreferredWidth = 75;
				buttonClose.Text = "Close";
				buttonClose.ButtonStyle = ButtonStyle.DefaultAccept;
				buttonClose.Anchor = AnchorStyles.BottomLeft;
				buttonClose.Margins = new Margins(10, 0, 0, 10);
				buttonClose.Clicked += new MessageEventHandler(this.HandleAboutButtonCloseClicked);
				buttonClose.TabIndex = 1000;
				buttonClose.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			}

			string[] modules = Resources.GetModuleNames(this.resourcePrefix);

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
#endif

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
		}


		protected void UpdateFields()
		{
			ResourceBundle.Field field = this.bundle0[this.rank];
			this.fieldRank.Text = this.rank.ToString();
			this.fieldName.Text = field.Name;
			this.fieldString.Text = field.AsString;
		}

		void HandleButtonClicked(object sender, MessageEventArgs e)
		{
			GlyphButton button = sender as GlyphButton;

			if ( button.Name == "Next" )  this.rank ++;
			if ( button.Name == "Prev" )  this.rank --;

			this.rank = System.Math.Max(this.rank, 0);
			this.rank = System.Math.Min(this.rank, this.bundle0.FieldCount-1);

			this.UpdateFields();
		}


		private void HandleWindowAboutCloseClicked(object sender)
		{
			this.window.Hide ();
		}

		private void HandleAboutButtonCloseClicked(object sender, MessageEventArgs e)
		{
			this.window.Hide ();
		}


		protected Window						window;
		protected string						resourcePrefix = "file";
		protected TextField						fieldRank;
		protected TextField						fieldName;
		protected TextField						fieldString;
		protected GlyphButton					buttonNext;
		protected GlyphButton					buttonPrev;
		protected int							rank = 0;
		protected ResourceBundle				bundle0;
		protected ResourceBundle				bundle1;
	}
}
