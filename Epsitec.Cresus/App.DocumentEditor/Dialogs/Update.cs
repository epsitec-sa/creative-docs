using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Document;

namespace Epsitec.App.DocumentEditor.Dialogs
{
	using GlobalSettings = Common.Document.Settings.GlobalSettings;

	/// <summary>
	/// Dialogue "mise à jour".
	/// </summary>
	public class Update : Abstract
	{
		public Update(DocumentEditor editor) : base(editor)
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
				this.WindowInit("Update", 330, 210);
				this.window.Text = "Mise à jour...";
				this.window.PreventAutoClose = true;
				this.window.Owner = this.editor.Window;
				this.window.WindowCloseClicked += new EventHandler(this.HandleWindowUpdateCloseClicked);

				this.version = new StaticText(this.window.Root);
				this.version.Height = this.version.Height*1.2;
				this.version.SetClientZoom(1.2);
				this.version.Dock = DockStyle.Top;
				this.version.DockMargins = new Margins(10, 10, 10, 0);

				this.limit = new StaticText(this.window.Root);
				this.limit.Height = this.limit.Height*1.2;
				this.limit.SetClientZoom(1.2);
				this.limit.Dock = DockStyle.Top;
				this.limit.DockMargins = new Margins(10, 10, 0, 10);

				this.buy = new StaticText(this.window.Root);
				this.buy.Height = 110;
//				this.buy.IsReadOnly = true;
				this.buy.Dock = DockStyle.Top;
				this.buy.DockMargins = new Margins(10, 10, 0, 10);

				// Bouton de fermeture.
				Button buttonClose = new Button(this.window.Root);
				buttonClose.Width = 75;
				buttonClose.Text = "Fermer";
				buttonClose.ButtonStyle = ButtonStyle.DefaultAccept;
				buttonClose.Anchor = AnchorStyles.BottomLeft;
				buttonClose.AnchorMargins = new Margins(10, 0, 0, 10);
				buttonClose.Clicked += new MessageEventHandler(this.HandleUpdateButtonCloseClicked);
				buttonClose.TabIndex = 1000;
				buttonClose.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				ToolTip.Default.SetToolTip(buttonClose, "Fermer ce dialogue");
			}

			this.version.Text = string.Format("Version <b>{0}</b>    Langue: français", About.GetVersion());

			if ( this.editor.InstallType == InstallType.Full )
			{
				string key = SerialAlgorithm.ReadSerial();
				string date = SerialAlgorithm.GetExpirationDate(key).Subtract(new System.TimeSpan(1, 0, 0, 0)).ToShortDateString();
				this.limit.Text = string.Format("Mises à jour gratuites jusqu'au <b>{0}</b>", date);
			}
			else if ( this.editor.InstallType == InstallType.Expired )
			{
				this.limit.Text = "<i>Plus de mises à jour gratuites</i>.";
			}
			else
			{
				this.limit.Text = " ";
			}

			string chip = "<list type=\"fix\" width=\"1.5\"/>";
			System.Text.StringBuilder b = new System.Text.StringBuilder();
			if ( this.editor.InstallType == InstallType.Demo )
			{
				b.Append("Vous pouvez acheter ce logiciel auprès de Epsitec SA.<br/>");
			}
			else
			{
				b.Append("Vous pouvez obtenir une mise à jour auprès de Epsitec SA.<br/>");
			}
			b.Append("<br/>");
			b.Append(chip);
			b.Append("Web:<tab/><a href=\"http://www.epsitec.ch\">www.epsitec.ch</a><br/>");
			b.Append(chip);
			b.Append("Mail:<tab/><a href=\"mailto:epsitec@epsitec.ch\">epsitec@epsitec.ch</a><br/>");
			b.Append(chip);
			b.Append("Poste:<tab/>Epsitec SA, Mouette 5, CH-1092 Belmont<br/>");
			b.Append(chip);
			b.Append("Tél:<tab/>021 728 44 83<br/>");
			b.Append(chip);
			b.Append("Fax:<tab/>021 728 44 83");
			this.buy.Text = b.ToString();
			this.buy.HypertextClicked += new MessageEventHandler(HandleLinkHypertextClicked);

			TextStyle.Tab tab = new TextStyle.Tab();
			tab.Pos = 60;
			this.buy.TextLayout.TabInsert(tab);

			this.window.ShowDialog();
		}

		// Enregistre la position de la fenêtre du dialogue.
		public override void Save()
		{
			this.WindowSave("Update");
		}


		private void HandleLinkHypertextClicked(object sender, MessageEventArgs e)
		{
			Widget widget = sender as Widget;
			System.Diagnostics.Process.Start(widget.Hypertext);
		}

		private void HandleWindowUpdateCloseClicked(object sender)
		{
			this.editor.Window.MakeActive();
			this.window.Hide();
		}

		private void HandleUpdateButtonCloseClicked(object sender, MessageEventArgs e)
		{
			this.editor.Window.MakeActive();
			this.window.Hide();
		}


		protected StaticText			version;
		protected StaticText			limit;
		protected StaticText			buy;
	}
}
