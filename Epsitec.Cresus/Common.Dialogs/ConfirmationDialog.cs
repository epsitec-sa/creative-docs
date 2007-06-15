//	Copyright © 2004-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
		/// <param name="asCancel">Présente optionnelle d'un bouton "Annuler" dans une bande grise en bas.</param>
		public ConfirmationDialog(string title, string header, List<string> questions, bool asCancel)
		{
			this.title = title;
			this.header = header;
			this.questions = questions;
			this.asCancel = asCancel;
		}


		protected Widget CreateBodyWidget()
		{
			IAdorner adorner = Widgets.Adorners.Factory.Active;

			StaticText container = new StaticText();
			container.BackColor = adorner.ColorTextBackground;
			container.Padding = new Drawing.Margins(ConfirmationDialog.margin);

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

			if (this.asCancel)  // bouton Cancel dans une bande grise en bas ?
			{
				Widget group = new Widget();

				container.SetParent(group);
				container.Dock = DockStyle.Fill;

				Widget footer = new Widget(group);
				footer.PreferredHeight = 38;
				footer.Dock = DockStyle.Bottom;

				Button button = new Button(footer);
				button.Text = Widgets.Res.Strings.Dialog.Button.Cancel;
				button.Name = "Cancel";
				button.Dock = DockStyle.Right;
				button.Margins = new Drawing.Margins(ConfirmationDialog.margin, ConfirmationDialog.margin, 8, 8);
				button.Clicked += new MessageEventHandler(this.HandleButtonClicked);

				group.PreferredSize = new Drawing.Size(ConfirmationDialog.width, 250);
				return group;
			}
			else
			{
				container.PreferredSize = new Drawing.Size(ConfirmationDialog.width, 250);
				return container;
			}
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
			body.TabIndex = 1;
			body.TabNavigationMode = TabNavigationMode.ForwardTabPassive;
			
			//?this.window.FocusedWidget = body.FindTabWidget(TabNavigationDir.Forwards, TabNavigationMode.ActivateOnTab);
			Platform.Beep.MessageBeep(Platform.Beep.MessageType.Warning);

			this.window.AdjustWindowSize ();
		}

		
		private void HandleButtonClicked(object sender, MessageEventArgs e)
		{
			Widget button = sender as Widget;

			if (button.Name == "Cancel")
			{
				this.result = DialogResult.Cancel;
			}
			else
			{
				int rank = int.Parse(button.Name);
				this.result = (DialogResult) (DialogResult.Answer1+rank);
			}
			this.CloseDialog();
		}


		protected static readonly double width = 300;
		protected static readonly double margin = 20;
		
		protected string						title;
		protected string						header;
		protected List<string>					questions;
		protected bool							asCancel;
	}
}
