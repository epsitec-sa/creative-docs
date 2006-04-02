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

		public override void Show()
		{
			//	Crée et montre la fenêtre du dialogue.
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

				//	Bouton de fermeture.
				Button buttonClose = new Button(this.window.Root);
				buttonClose.Width = 75;
				buttonClose.Text = Res.Strings.Dialog.Button.Close;
				buttonClose.ButtonStyle = ButtonStyle.DefaultAccept;
				buttonClose.Anchor = AnchorStyles.BottomLeft;
				buttonClose.Margins = new Margins(10, 0, 0, 10);
				buttonClose.Clicked += new MessageEventHandler(this.HandleAboutButtonCloseClicked);
				buttonClose.TabIndex = 1000;
				buttonClose.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				ToolTip.Default.SetToolTip(buttonClose, Res.Strings.Dialog.Tooltip.Close);

				StaticText www = new StaticText(this.window.Root);
				www.Width = 400-280;
				www.Text = string.Format("<a href=\"{0}\">{1}</a><br/>", Res.Strings.Dialog.About.Link, Res.Strings.Dialog.About.Web);
				www.Alignment = ContentAlignment.MiddleLeft;
				www.HypertextClicked += new MessageEventHandler(HandleLinkHypertextClicked);
				www.Anchor = AnchorStyles.BottomRight;
				www.Margins = new Margins(0, 0, 0, 15);
			}

			this.window.ShowDialog();
		}

		public override void Save()
		{
			//	Enregistre la position de la fenêtre du dialogue.
			this.WindowSave("About");
		}


		public static StaticText CreateWidgetSplash(Widget parent, InstallType type)
		{
			//	Crée les widgets pour l'image de bienvenue.
			double y = parent.Height-200;

			string res;
			if ( type == InstallType.Freeware )
			{
				res = "manifest:Epsitec.App.CreativeDocs.SplashScreen.png";
			}
			else
			{
				res = "manifest:Epsitec.App.CresusDocuments.SplashScreen.png";
			}
			string text = string.Format("<img src=\"{0}\"/>", res);
			StaticText image = new StaticText(parent);
			image.Text = text;
			image.Location = new Point(0, y+0);
			image.Size = new Size(400, 200);

			if ( type == InstallType.Freeware )
			{
				string version = About.GetVersion();
				StaticText sv = new StaticText(parent);
				sv.Text = string.Format("<font size=\"80%\"><font size=\"140%\"><b>{0} {1}</b></font>    {2}</font>", Res.Strings.Dialog.About.Version, version, Res.Strings.Dialog.About.Language);
				sv.Location = new Point(10, y+5+12*1);
				sv.Size = new Size(270, 14);
//-				sv.SetClientZoom(0.8);

				StaticText ep = new StaticText(parent);
				ep.Text = @"<font size=""80%"">" + Res.Strings.Dialog.About.Copyright + "</font>";
				ep.Location = new Point(10, y+5+12*0);
				ep.Size = new Size(270, 14);
//-				ep.SetClientZoom(0.8);
			}
			else
			{
				string version = About.GetVersion();
				StaticText sv = new StaticText(parent);
				sv.Text = string.Format("<font size=\"80%\"><font size=\"140%\"><b>{0} {1}</b></font>    {2}</font>", Res.Strings.Dialog.About.Version, version, Res.Strings.Dialog.About.Language);
				sv.Location = new Point(22, y+5+12*2);
				sv.Size = new Size(270, 14);
//				sv.SetClientZoom(0.8);

				string sk = About.GetKey();
				if ( sk != null )
				{
					StaticText key = new StaticText(parent);
					key.Text = string.Format(@"<font size=""80%"">{0}: {1}</font>", Res.Strings.Dialog.About.Key, sk);
					key.Location = new Point(22, y+5+12*1);
					key.Size = new Size(270, 14);
//-					key.SetClientZoom(0.8);
				}

				StaticText ep = new StaticText(parent);
				ep.Text = @"<font size=""80%"">" + Res.Strings.Dialog.About.Copyright + "</font>";
				ep.Location = new Point(22, y+5+12*0);
				ep.Size = new Size(270, 14);
//-				ep.SetClientZoom(0.8);
			}

			if ( type == InstallType.Demo )
			{
				StaticText warning = new StaticText(parent);
				warning.Text = "<font size=\"250%\"><b>" + Res.Strings.Dialog.About.Demo + "</b></font>";
				warning.Location = new Point(280, y+0);
				warning.Size = new Size(84, 40);
//-				warning.SetClientZoom(2.5);
			}

			if ( type == InstallType.Expired )
			{
				StaticText warning = new StaticText(parent);
				warning.Text = "<font size=\"250%\"><b>" + Res.Strings.Dialog.About.Expired + "</b></font>";
				warning.Location = new Point(280, y+0);
				warning.Size = new Size(84, 40);
//-				warning.SetClientZoom(2.5);
			}

			if ( type == InstallType.Freeware )
			{
				StaticText warning = new StaticText(parent);
				warning.Text = "<font size=\"180%\"><b>" + Res.Strings.Dialog.About.Freeware + "</b></font>";
				warning.Location = new Point(280, y+0);
				warning.Size = new Size(120, 40);
//-				warning.SetClientZoom(1.8);
			}

			return image;
		}

		public static string GetVersion()
		{
			//	Donne le numéro de version.
			string version = typeof(Document).Assembly.FullName.Split(',')[1].Split('=')[1];
			if ( version.EndsWith(".0") )
			{
				version = version.Substring(0, version.Length-2);
			}
			return version;
		}

		protected static string GetKey()
		{
			//	Lit la clé d'installation.
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
