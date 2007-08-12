//	Copyright © 2003-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.App.Dolphin.Dialogs
{
	/// <summary>
	/// Dialogue "à porpos de".
	/// </summary>
	public class About : Abstract
	{
		public About(DolphinApplication application) : base(application)
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
				this.window.PreventAutoClose = true;
				this.WindowInit("About", 350, 180, true);
				this.window.Text = "Dauphin";
				this.window.Owner = this.application.Window;
				this.window.Icon = Bitmap.FromManifestResource("Epsitec.App.Dolphin.Images.Application.icon", typeof(DolphinApplication).Assembly);
				this.window.WindowCloseClicked += new EventHandler(this.HandleWindowInfosCloseClicked);

				System.Text.StringBuilder builder = new System.Text.StringBuilder();
				builder.Append("<font size=\"150%\"><b>");
				builder.Append(DolphinApplication.ApplicationTitle);
				builder.Append("</b></font><br/>");
				builder.Append("<br/>");
				builder.Append("Ce logiciel a été créé par :<br/>");
				builder.Append("Daniel Roux<br/>");
				builder.Append("EPSITEC SA<br/>");
				builder.Append("ch. de la Mouette 5<br/>");
				builder.Append("CH-1092 Belmont<br/>");
				builder.Append("<a href=\"http://www.epsitec.ch\">www.epsitec.ch</a><br/>");
				builder.Append("<br/>");
				builder.Append("<b>Version ");
				builder.Append(Misc.GetVersion());
				builder.Append("</b>");

				StaticText text = new StaticText(this.window.Root);
				text.Text = builder.ToString();
				text.ContentAlignment = ContentAlignment.TopLeft;
				text.Dock = DockStyle.Fill;
				text.Margins = new Margins(10, 10, 10, 10);
				text.HypertextClicked += new MessageEventHandler(Abstract.HandleLinkHypertextClicked);

				//	Bouton de fermeture.
				Button buttonClose = new Button(this.window.Root);
				buttonClose.PreferredWidth = 75;
				buttonClose.Text = "OK";
				buttonClose.ButtonStyle = ButtonStyle.DefaultAcceptAndCancel;
				buttonClose.Anchor = AnchorStyles.BottomRight;
				buttonClose.Margins = new Margins(0, 10, 0, 10);
				buttonClose.Clicked += new MessageEventHandler(this.HandleInfosButtonCloseClicked);
				buttonClose.TabIndex = 1000;
				buttonClose.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			}

			this.window.ShowDialog();
		}

		public override void Save()
		{
			//	Enregistre la position de la fenêtre du dialogue.
			this.WindowSave("About");
		}


		private void HandleWindowInfosCloseClicked(object sender)
		{
			this.CloseWindow();
		}

		private void HandleInfosButtonCloseClicked(object sender, MessageEventArgs e)
		{
			this.CloseWindow();
		}
	}
}
