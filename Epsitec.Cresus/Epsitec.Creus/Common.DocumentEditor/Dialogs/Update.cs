using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Document;

namespace Epsitec.Common.DocumentEditor.Dialogs
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

		public override void Show()
		{
			//	Crée et montre la fenêtre du dialogue.
			if ( this.window == null )
			{
				this.window = new Window();
				this.window.MakeFixedSizeWindow();
				this.window.MakeSecondaryWindow();
				this.WindowInit("Update", 350, 210);
				this.window.Text = Res.Strings.Dialog.Update.Title;
				this.window.PreventAutoClose = true;
				this.window.Owner = this.editor.Window;
				this.window.WindowCloseClicked += this.HandleWindowUpdateCloseClicked;

				this.version = new StaticText(this.window.Root);
				this.version.PreferredHeight = this.version.PreferredHeight*1.2;
//				this.version.SetClientZoom(1.2);
				this.version.Dock = DockStyle.Top;
				this.version.Margins = new Margins (10, 10, 10, 0);

				this.limit = new StaticText(this.window.Root);
				this.limit.PreferredHeight = this.limit.PreferredHeight*1.2;
//				this.limit.SetClientZoom(1.2);
				this.limit.Dock = DockStyle.Top;
				this.limit.Margins = new Margins(10, 10, 0, 10);

				this.buy = new StaticText(this.window.Root);
				this.buy.PreferredHeight = 110;
				this.buy.Dock = DockStyle.Top;
				this.buy.Margins = new Margins(10, 10, 0, 10);

				//	Bouton de fermeture.
				Button buttonClose = new Button(this.window.Root);
				buttonClose.PreferredWidth = 75;
				buttonClose.Text = Res.Strings.Dialog.Button.Close;
				buttonClose.ButtonStyle = ButtonStyle.DefaultAcceptAndCancel;
				buttonClose.Anchor = AnchorStyles.BottomRight;
				buttonClose.Margins = new Margins(0, 10, 0, 10);
				buttonClose.Clicked += this.HandleUpdateButtonCloseClicked;
				buttonClose.TabIndex = 1000;
				buttonClose.TabNavigationMode = TabNavigationMode.ActivateOnTab;
				ToolTip.Default.SetToolTip(buttonClose, Res.Strings.Dialog.Tooltip.Close);
			}

			this.version.Text = "<font size=\"120%\">" + string.Format(Res.Strings.Dialog.Update.Version, About.GetVersion()) + "</font>";

			if ( this.editor.InstallType == InstallType.Full )
			{
				string key = SerialAlgorithm.ReadCrDocSerial();
				string date = SerialAlgorithm.GetExpirationDate(key).Subtract(new System.TimeSpan(1, 0, 0, 0)).ToShortDateString();
				this.limit.Text = "<font size=\"120%\">" + string.Format(Res.Strings.Dialog.Update.Limit, date) + "</font>";
			}
			else if ( this.editor.InstallType == InstallType.Expired )
			{
				this.limit.Text = "<font size=\"120%\">" + Res.Strings.Dialog.Update.Over + "</font>";
			}
			else
			{
				this.limit.Text = " ";
			}

			string chip = "<list type=\"fix\" width=\"1.5\"/>";
			System.Text.StringBuilder b = new System.Text.StringBuilder();
			if ( this.editor.InstallType == InstallType.Demo )
			{
				b.Append(Res.Strings.Dialog.Update.BuyDemo);
				b.Append("<br/>");
			}
			else if ( this.editor.InstallType == InstallType.Freeware )
			{
				b.Append(Res.Strings.Dialog.Update.BuyFreeware);
				b.Append("<br/>");
			}
			else
			{
				b.Append(Res.Strings.Dialog.Update.BuyFull);
				b.Append("<br/>");
			}
			b.Append("<br/>");
			b.Append(chip);
			b.Append(string.Format("{0}<tab/><a href=\"{1}\">{2}</a><br/>", Res.Strings.Dialog.Update.Web1, Res.Strings.Dialog.Update.Web2, Res.Strings.Dialog.Update.Web3));
			b.Append(chip);
			b.Append(string.Format("{0}<tab/><a href=\"{1}\">{2}</a><br/>", Res.Strings.Dialog.Update.Mail1, Res.Strings.Dialog.Update.Mail2, Res.Strings.Dialog.Update.Mail3));
			b.Append(chip);
			b.Append(string.Format("{0}<tab/>{1}<br/>", Res.Strings.Dialog.Update.Address1, Res.Strings.Dialog.Update.Address2));
			b.Append(chip);
			b.Append(string.Format("{0}<tab/>{1}<br/>", Res.Strings.Dialog.Update.Phone1, Res.Strings.Dialog.Update.Phone2));
//-			b.Append(chip);
//-			b.Append(string.Format("{0}<tab/>{1}", Res.Strings.Dialog.Update.Fax1, Res.Strings.Dialog.Update.Fax2));
			this.buy.Text = b.ToString();
			this.buy.HypertextClicked += this.HandleLinkHypertextClicked;

			TextStyle.Tab tab = new TextStyle.Tab(70, TextTabType.Left, TextTabLine.None);
			this.buy.TextLayout.TabInsert(tab);

			this.window.ShowDialog();
		}

		public override void Save()
		{
			//	Enregistre la position de la fenêtre du dialogue.
			this.WindowSave("Update");
		}


		private void HandleLinkHypertextClicked(object sender, MessageEventArgs e)
		{
			Widget widget = sender as Widget;
			System.Diagnostics.Process.Start(widget.Hypertext);
		}

		private void HandleWindowUpdateCloseClicked(object sender)
		{
			this.CloseWindow();
		}

		private void HandleUpdateButtonCloseClicked(object sender, MessageEventArgs e)
		{
			this.CloseWindow();
		}


		protected StaticText			version;
		protected StaticText			limit;
		protected StaticText			buy;
	}
}
