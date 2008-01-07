//	Copyright © 2004-2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Daniel Roux

using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.Dialogs
{
	/// <summary>
	/// Dialogue pour demander confirmation avec plusieurs gros boutons. Chaque bouton peut contenir
	/// un sous-titre et un long texte explicatif.
	/// </summary>
	public class ConfirmationDialog : AbstractMessageDialog
	{
		/// <summary>
		/// Constructeur du dialogue pour demander confirmation avec plusieurs gros boutons.
		/// </summary>
		/// <param name="title">Titre du dialogue, dans la barre de titre de la fenêtre.</param>
		/// <param name="header">Question posée en haut du dialogue.</param>
		/// <param name="questions">Liste de questions, formatées avec ConfirmationButton.FormatContent().</param>
		/// <param name="hasCancelButton">Présente optionnelle d'un bouton "Annuler" dans une bande grise en bas.</param>
		public ConfirmationDialog(string title, string header, IEnumerable<string> questions, bool hasCancelButton)
		{
			this.title = title;
			this.header = header;
			this.questions = new List<string> (questions);
			this.hasCancel = hasCancelButton;
		}


		protected Widget CreateBodyWidget()
		{
			IAdorner adorner = Widgets.Adorners.Factory.Active;

			FrameBox container = new FrameBox ();
			container.BackColor = adorner.ColorTextBackground;
			container.Padding = new Drawing.Margins(ConfirmationDialog.margin);

			ConfirmationStaticText header = new ConfirmationStaticText(container);
			header.Text = this.header;
			header.Dock = DockStyle.Top;
			header.Margins = new Drawing.Margins(0, 0, 0, 10);

			int index = 1;
			foreach (string question in this.questions)
			{
				Button button = new ConfirmationButton(container);
				button.Text = question;
				button.Index = index-1;
				button.TabIndex = index++;
				button.Dock = DockStyle.Top;
				button.Clicked += new MessageEventHandler(this.HandleButtonClicked);
			}

			if (this.hasCancel)  // bouton Cancel dans une bande grise en bas ?
			{
				FrameBox buttons = container;
				
				container = new FrameBox ();

				buttons.SetParent(container);
				buttons.Dock = DockStyle.Fill;
				buttons.TabIndex = 1;

				FrameBox footer = new FrameBox(container);
				footer.PreferredHeight = 38;
				footer.Dock = DockStyle.Bottom;
				footer.TabIndex = 2;

				Button button = new Button(footer);
				button.Text = Widgets.Res.Strings.Dialog.Button.Cancel;
				button.Name = ConfirmationDialog.cancelButtonName;
				button.Dock = DockStyle.Right;
				button.Margins = new Drawing.Margins(ConfirmationDialog.margin, ConfirmationDialog.margin, 8, 8);
				button.Clicked += new MessageEventHandler(this.HandleButtonClicked);
				button.TabIndex = 1;
				button.Shortcuts.Add (Common.Widgets.Feel.Factory.Active.CancelShortcut);
			}
			
			container.PreferredSize = new Drawing.Size(ConfirmationDialog.width, 100);
			return container;
		}

		protected override Window CreateWindow()
		{
			Window dialogWindow = new Window ();
			
			Widget body = this.CreateBodyWidget();
			double dx = body.PreferredWidth;
			double dy = body.PreferredHeight;

			dialogWindow.Text = this.title;
			dialogWindow.Name = "Dialog";
			dialogWindow.ClientSize = new Drawing.Size (dx+ConfirmationDialog.margin*2, dy+ConfirmationDialog.margin*2);
			dialogWindow.PreventAutoClose = true;

			dialogWindow.MakeFixedSizeWindow ();
			dialogWindow.MakeSecondaryWindow ();

			CommandDispatcher.SetDispatcher (dialogWindow, new CommandDispatcher ());

			if (this.hasCancel)
			{
				dialogWindow.WindowCloseClicked += delegate
				{
					this.DialogResult = DialogResult.Cancel;
					this.CloseDialog ();
				};
			}
			else
			{
				dialogWindow.MakeButtonlessWindow ();
			}

			body.SetParent (dialogWindow.Root);
			body.Dock = DockStyle.Fill;

			body.SetFocusOnTabWidget ();
			Platform.Beep.MessageBeep(Platform.Beep.MessageType.Warning);

			dialogWindow.AdjustWindowSize ();

			return dialogWindow;
		}

		
		private void HandleButtonClicked(object sender, MessageEventArgs e)
		{
			Widget button = sender as Widget;

			if (button.Name == ConfirmationDialog.cancelButtonName)
			{
				this.DialogResult = DialogResult.Cancel;
			}
			else
			{
				int rank = button.Index;
				this.DialogResult = (DialogResult) (DialogResult.Answer1+rank);
			}
			this.CloseDialog();
		}


		private const double width = 300;
		private const double margin = 20;
		private const string cancelButtonName = "Cancel";
		
		private string							title;
		private string							header;
		private List<string>					questions;
		private bool							hasCancel;
	}
}
