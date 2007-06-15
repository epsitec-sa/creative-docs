//	Copyright © 2004-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Daniel Roux

using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.Dialogs
{
	/// <summary>
	/// Summary description for ConfirmationDialog.
	/// </summary>
	public class ConfirmationDialog : AbstractMessageDialog
	{
		public ConfirmationDialog(string title, string header, List<string> questions)
		{
			this.title = title;
			this.header = header;
			this.questions = questions;
		}


		protected Widget CreateBodyWidget()
		{
			IAdorner adorner = Widgets.Adorners.Factory.Active;

			StaticText container = new StaticText();
			container.BackColor = adorner.ColorTextBackground;

			ConfirmationStaticText header = new ConfirmationStaticText(container);
			header.Text = this.header;
			header.Dock = DockStyle.Top;
			header.Margins = new Drawing.Margins(0, 0, 0, 10);

			int index = 0;
			foreach (string question in this.questions)
			{
				Button button = new ConfirmationButton(container);
				button.Text = question;
				button.Name = index.ToString(System.Globalization.CultureInfo.InvariantCulture);
				button.TabIndex = index++;
				button.Dock = DockStyle.Top;
				button.Clicked += new MessageEventHandler(this.HandleButtonClicked);
			}

			container.PreferredSize = new Drawing.Size(ConfirmationDialog.width, 250);

			return container;
		}

		protected override void CreateWindow()
		{
			this.window = new Window();
			
			Widget body = this.CreateBodyWidget();
			double dx = body.PreferredWidth;
			double dy = body.PreferredHeight;
			
			this.window.Text = this.title;
			this.window.Name = "Dialog";
			this.window.ClientSize = new Drawing.Size(dx+ConfirmationDialog.margin*2, dy+ConfirmationDialog.margin*2);
			this.window.PreventAutoClose = true;

			this.window.MakeFixedSizeWindow();
			this.window.MakeButtonlessWindow();
			this.window.MakeSecondaryWindow();
			
			body.SetParent(this.window.Root);
			body.Dock = DockStyle.Fill;
			body.Padding = new Drawing.Margins(ConfirmationDialog.margin);
			body.TabIndex = 1;
			body.TabNavigationMode = TabNavigationMode.ForwardTabPassive;
			
			//?this.window.FocusedWidget = body.FindTabWidget(TabNavigationDir.Forwards, TabNavigationMode.ActivateOnTab);
			Platform.Beep.MessageBeep(Platform.Beep.MessageType.Warning);

			this.window.AdjustWindowSize ();
		}

		
		private void HandleButtonClicked(object sender, MessageEventArgs e)
		{
			ConfirmationButton button = sender as ConfirmationButton;
			int rank = int.Parse(button.Name);
			this.result = (DialogResult) (DialogResult.Answer1+rank);
			this.CloseDialog();
		}


		protected static readonly double width = 300;
		protected static readonly double margin = 20;
		
		protected string						title;
		protected string						header;
		protected List<string>					questions;
	}
}
