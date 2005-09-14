using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Document;

namespace Epsitec.App.DocumentEditor.Dialogs
{
	using GlobalSettings = Common.Document.Settings.GlobalSettings;

	/// <summary>
	/// Dialogue "A propos de".
	/// </summary>
	public class About : Abstract
	{
		public About(DocumentEditor editor) : base(editor)
		{
		}

		// Crée et montre la fenêtre du dialogue.
		public override void Show()
		{
			if ( this.window == null )
			{
				this.window = new Window();
				this.window.MakeFixedSizeWindow();
				this.window.MakeSecondaryWindow();
				this.WindowInit("About", 400, 243);
				this.window.Text = Res.Strings.Dialog.About.Title;
				this.window.PreventAutoClose = true;
				this.window.Owner = this.editor.Window;
				this.window.WindowCloseClicked += new EventHandler(this.HandleWindowAboutCloseClicked);

				About.CreateWidgetSplash(this.window.Root, this.editor.InstallType);

				// Bouton de fermeture.
				Button buttonClose = new Button(this.window.Root);
				buttonClose.Width = 75;
				buttonClose.Text = Res.Strings.Dialog.Button.Close;
				buttonClose.ButtonStyle = ButtonStyle.DefaultAccept;
				buttonClose.Anchor = AnchorStyles.BottomLeft;
				buttonClose.AnchorMargins = new Margins(10, 0, 0, 10);
				buttonClose.Clicked += new MessageEventHandler(this.HandleAboutButtonCloseClicked);
				buttonClose.TabIndex = 1000;
				buttonClose.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				ToolTip.Default.SetToolTip(buttonClose, Res.Strings.Dialog.Tooltip.Close);

				StaticText www = new StaticText(this.window.Root);
				www.Width = 120;
				www.Text = string.Format("<a href=\"{0}\">{1}</a><br/>", Res.Strings.Dialog.About.Link, Res.Strings.Dialog.About.Web);
				www.Alignment = ContentAlignment.MiddleLeft;
				www.HypertextClicked += new MessageEventHandler(HandleLinkHypertextClicked);
				www.Anchor = AnchorStyles.BottomRight;
				www.AnchorMargins = new Margins(0, 0, 0, 15);
			}

			this.window.ShowDialog();
		}

		// Enregistre la position de la fenêtre du dialogue.
		public override void Save()
		{
			this.WindowSave("About");
		}


		// Crée les widgets pour l'image de bienvenue.
		public static StaticText CreateWidgetSplash(Widget parent, InstallType type)
		{
			double y = parent.Height-200;

			string res = "manifest:Epsitec.App.DocumentEditor.Images.SplashScreen.png";
			string text = string.Format("<img src=\"{0}\"/>", res);
			StaticText image = new StaticText(parent);
			image.Text = text;
			image.Location = new Point(0, y+0);
			image.Size = new Size(400, 200);

			string version = About.GetVersion();
			StaticText sv = new StaticText(parent);
			sv.Text = string.Format("<font size=\"140%\"><b>{0} {1}</b></font>    {2}", Res.Strings.Dialog.About.Version, version, Res.Strings.Dialog.About.Language);
			sv.Location = new Point(22, y+5+12*2);
			sv.Size = new Size(270, 14);
			sv.SetClientZoom(0.8);

			string sk = About.GetKey();
			if ( sk != null )
			{
				StaticText key = new StaticText(parent);
				key.Text = string.Format("{0}: {1}", Res.Strings.Dialog.About.Key, sk);
				key.Location = new Point(22, y+5+12*1);
				key.Size = new Size(270, 14);
				key.SetClientZoom(0.8);
			}

			StaticText ep = new StaticText(parent);
			ep.Text = Res.Strings.Dialog.About.Copyright;
			ep.Location = new Point(22, y+5+12*0);
			ep.Size = new Size(270, 14);
			ep.SetClientZoom(0.8);

			if ( type == InstallType.Demo )
			{
				StaticText warning = new StaticText(parent);
				warning.Text = "<b>" + Res.Strings.Dialog.About.Demo + "</b>";
				warning.Location = new Point(280, y+0);
				warning.Size = new Size(84, 40);
				warning.SetClientZoom(2.5);
			}

			if ( type == InstallType.Expired )
			{
				StaticText warning = new StaticText(parent);
				warning.Text = "<b>" + Res.Strings.Dialog.About.Expired + "</b>";
				warning.Location = new Point(280, y+0);
				warning.Size = new Size(84, 40);
				warning.SetClientZoom(2.5);
			}

			if ( type == InstallType.Freeware )
			{
				StaticText warning = new StaticText(parent);
				warning.Text = "<b>" + Res.Strings.Dialog.About.Freeware + "</b>";
				warning.Location = new Point(280, y+0);
				warning.Size = new Size(190, 40);
				warning.SetClientZoom(2.0);
			}

			return image;
		}

		// Donne le numéro de version.
		public static string GetVersion()
		{
			string version = typeof(Document).Assembly.FullName.Split(',')[1].Split('=')[1];
			if ( version.EndsWith(".0") )
			{
				version = version.Substring(0, version.Length-2);
			}
			return version;
		}

		// Lit la clé d'installation.
		protected static string GetKey()
		{
			string key = Common.Support.SerialAlgorithm.ReadSerial();

			if ( key != null && key.Length == 24 )
			{
				if ( Common.Support.SerialAlgorithm.CheckSerial(key) )
				{
					return key;
				}
			}

			return null;
		}


		private void HandleLinkHypertextClicked(object sender, MessageEventArgs e)
		{
			Widget widget = sender as Widget;
			System.Diagnostics.Process.Start(widget.Hypertext);
		}

		private void HandleWindowAboutCloseClicked(object sender)
		{
			this.editor.Window.MakeActive();
			this.window.Hide();
			this.OnClosed();
		}

		private void HandleAboutButtonCloseClicked(object sender, MessageEventArgs e)
		{
			this.editor.Window.MakeActive();
			this.window.Hide();
			this.OnClosed();
		}
	}
}
