using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Dialogs
{
	/// <summary>
	/// Dialogue permettant d'afficher le message initial contenant des liens hypertexte.
	/// </summary>
	public class InitialMessageDialog : AbstractDialog
	{
		public InitialMessageDialog(DesignerApplication designerApplication) : base(designerApplication)
		{
		}

		public override void Show()
		{
			//	Crée et montre la fenêtre du dialogue.
			if ( this.window == null )
			{
				this.window = new Window();
				this.window.Icon = this.designerApplication.Icon;
				this.window.MakeSecondaryWindow ();
				this.window.MakeFixedSizeWindow();
				this.window.MakeToolWindow();
				this.window.PreventAutoClose = true;
				this.WindowInit("Message", 200, 100, true);
				this.window.Text = Res.Strings.Dialog.InitialMessage.Title;
				this.window.Owner = this.parentWindow;
				this.window.WindowCloseClicked += this.HandleWindowCloseClicked;
				this.window.Root.Padding = new Margins(8, 8, 8, 8);

				Widget container = new Widget(this.window.Root);
				container.Dock = DockStyle.Fill;

				this.widgetIcon = new StaticImage(container);
				this.widgetIcon.ImageName = "manifest:Epsitec.Common.Dialogs.Images.Information.icon";
				this.widgetIcon.PreferredSize = new Size(48, 48);
				this.widgetIcon.Dock = DockStyle.Left;
				this.widgetIcon.Margins = new Margins(0, 0, 0, 0);

				this.widgetText = new StaticText(container);
				this.widgetText.TextBreakMode = TextBreakMode.Hyphenate;
				this.widgetText.Dock = DockStyle.Fill;
				this.widgetText.Margins = new Margins(8, 0, 0, 0);
				this.widgetText.HypertextClicked += this.HandleHypertextClicked;

				//	Boutons de fermeture.
				Widget footer = new Widget(this.window.Root);
				footer.PreferredHeight = 22;
				footer.Margins = new Margins(0, 0, 8, 0);
				footer.Dock = DockStyle.Bottom;

				this.buttonClose = new Button(footer);
				this.buttonClose.PreferredWidth = 75;
				this.buttonClose.Text = Res.Strings.Dialog.Button.Close;
				this.buttonClose.ButtonStyle = ButtonStyle.DefaultAccept;
				this.buttonClose.Dock = DockStyle.Right;
				this.buttonClose.Clicked += this.HandleButtonCloseClicked;
				this.buttonClose.TabIndex = 1;
				this.buttonClose.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			}

			this.Update();
			this.window.Show();
		}


		public void Initialise(string message)
		{
			this.message = message;
		}

		protected void Update()
		{
			this.widgetText.Text = this.message;

			TextLayout textLayout = this.widgetText.TextLayout;
			Size layoutSize = textLayout.LayoutSize;
			double minWidth = System.Math.Min(400, System.Math.Ceiling(textLayout.SingleLineSize.Width)+4);
			textLayout.LayoutSize = new Size(minWidth, TextLayout.Infinite);

			double width  = textLayout.LayoutSize.Width;
			double height = System.Math.Ceiling(textLayout.TotalRectangle.Height+4);

			textLayout.LayoutSize = layoutSize;
			this.widgetText.PreferredSize = new Size(width, height);
			this.window.ClientSize = new Size(8+48+8+width+8, 8+height+8+22+8);
		}


		private void HandleHypertextClicked(object sender, MessageEventArgs e)
		{
			//	Appelé lorsqu'un hyperlien est cliqué.
			StaticText text = sender as StaticText;

			Druid druid = Druid.Parse(text.Hypertext);
			this.designerApplication.NavigateToCaption(druid);
		}

		private void HandleWindowCloseClicked(object sender)
		{
			this.parentWindow.MakeActive();
			this.window.Hide();
			this.OnClosed();
		}

		private void HandleButtonCloseClicked(object sender, MessageEventArgs e)
		{
			this.parentWindow.MakeActive();
			this.window.Hide();
			this.OnClosed();
		}


		protected string						message;
		protected StaticImage					widgetIcon;
		protected StaticText					widgetText;
		protected Button						buttonClose;
	}
}
