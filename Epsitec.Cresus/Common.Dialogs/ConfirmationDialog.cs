//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Daniel Roux

using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System;

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

		public ConfirmationDialog(string title, string header, params string[] questions)
			: this (title, header, questions, false)
		{
		}


		protected ConfirmationDialog()
		{
			this.questions = new List<string> ();
		}

		protected void DefineTitle(FormattedText text)
		{
			this.CheckMutable ();

			this.title = text.ToSimpleText ();
		}

		protected void DefineHeader(FormattedText text)
		{
			this.CheckMutable ();

			this.header = text.ToString ();
		}

		protected void AddQuestion(FormattedText text)
		{
			this.CheckMutable ();

			this.questions.Add (text.ToString ());
		}

		protected void UpdateQuestions(System.Func<string, string> updater)
		{
			this.CheckMutable ();

			this.title = updater (this.title);
			this.header = updater (this.header);

			for (int i = 0; i < this.questions.Count; i++)
			{
				this.questions[i] = updater (this.questions[i]);
			}
		}


		private void CheckMutable()
		{
			if (this.HasWindow)
            {
				throw new System.InvalidOperationException ("Cannot change dialog contents: UI already exists");
            }
		}


		protected virtual Widget CreateUI()
		{
			var adorner = Widgets.Adorners.Factory.Active;

			var container = new FrameBox ()
			{
				BackColor = adorner.ColorTextBackground,
				Padding = new Margins (ConfirmationDialog.margin),
			};

			this.CreateUIHeader (container);

			int index = 1;

			foreach (string question in this.questions)
			{
				index = this.CreateUIQuestion (index, container, question);
			}

			if (this.hasCancel)
			{
				container = this.CreateUICancelButton (container);
			}

			container.PreferredSize = new Size (this.GetDefaultWidth (), 100);

			return container;
		}

		protected virtual double GetDefaultWidth()
		{
			return ConfirmationDialog.width;
		}

		protected virtual Widget CreateUIHeader(FrameBox container)
		{
			if (string.IsNullOrEmpty (this.header))
			{
				return null;
			}
			else
			{
				return new ConfirmationStaticText (container)
				{
					Text = this.header,
					Dock = DockStyle.Top,
					Margins = new Margins (0, 0, 0, 10),
					Name = "header",
				};
			}
		}
		
		protected virtual int CreateUIQuestion(int index, FrameBox container, string question)
		{
			if (string.IsNullOrEmpty (question))
            {
				new Separator ()
				{
					Dock = DockStyle.Top,
					PreferredHeight = 1,
					IsHorizontalLine = true,
				};
            }
			else if (question.StartsWith ("#TEXT#"))
			{
				new ConfirmationStaticText (container)
				{
					Text = question.Substring (6),
					Dock = DockStyle.Top,
					Margins = new Margins (0, 0, 5, 5),
				};
			}
			else
			{
				var button = new ConfirmationButton (container)
				{
					Text = question,
					Index = index-1,
					TabIndex = index++,
					Dock = DockStyle.Top,
					Name = string.Format (System.Globalization.CultureInfo.InvariantCulture, "q{0}", index),
				};

				button.Clicked += this.HandleButtonClicked;
			}

			return index;
		}

		private FrameBox CreateUICancelButton(FrameBox container)
		{
			var buttonsFrame = container;
			var newContainer = new FrameBox ();

			buttonsFrame.SetParent (newContainer);
			buttonsFrame.Dock = DockStyle.Fill;
			buttonsFrame.TabIndex = 1;

			var footer = new FrameBox ()
			{
				Parent = newContainer,
				PreferredHeight = 38,
				Dock = DockStyle.Bottom,
				TabIndex = 2,
			};

			var button = new Button ()
			{
				Parent = footer,
				CommandId = Druid.FromLong (Dialogs.Res.CommandIds.Dialog.Generic.Cancel),
				Name = ConfirmationDialog.cancelButtonName,
				Dock = DockStyle.Right,
				Margins = new Margins (ConfirmationDialog.margin, ConfirmationDialog.margin, 8, 8),
				TabIndex = 1,
			};

			button.Clicked += this.HandleButtonClicked;
			button.Shortcuts.Add (Common.Widgets.Feel.Factory.Active.CancelShortcut);
			
			return newContainer;
		}

		protected override Window CreateWindow()
		{
			Window dialogWindow = new Window ();
			
			Widget body = this.CreateUI();
			double dx = body.PreferredWidth;
			double dy = body.PreferredHeight;

			dialogWindow.MakeFixedSizeWindow ();
			dialogWindow.MakeSecondaryWindow ();

			dialogWindow.Text = this.title;
			dialogWindow.Name = "Dialog";
			dialogWindow.ClientSize = new Size (dx+ConfirmationDialog.margin*2, dy+ConfirmationDialog.margin*2);
			dialogWindow.PreventAutoClose = true;

			CommandDispatcher.SetDispatcher (dialogWindow, new CommandDispatcher ());

			if (this.hasCancel)
			{
				dialogWindow.WindowCloseClicked += delegate
				{
					this.Result = DialogResult.Cancel;
					this.CloseDialog ();
				};
			}
			else
			{
				dialogWindow.MakeButtonlessWindow ();
			}

			this.SetupWindow (dialogWindow);

			body.SetParent (dialogWindow.Root);
			body.Dock = DockStyle.Fill;

			body.SetFocusOnTabWidget ();
			Platform.Beep.MessageBeep(Platform.Beep.MessageType.Warning);

			dialogWindow.AdjustWindowSize ();
			
			return dialogWindow;
		}

		protected virtual void SetupWindow(Window dialogWindow)
		{
		}
        
		private void HandleButtonClicked(object sender, MessageEventArgs e)
		{
			Widget button = sender as Widget;

			if (button.Name == ConfirmationDialog.cancelButtonName)
			{
				this.Result = DialogResult.Cancel;
			}
			else
			{
				int rank = button.Index;
				this.Result = (DialogResult) (DialogResult.Answer1+rank);
			}
			this.CloseDialog();
		}


		private const double width = 300;
		private const double margin = 20;
		private const string cancelButtonName = "Cancel";

		private readonly List<string>			questions;
		private string							title;
		private string							header;
		private bool							hasCancel;
	}
}
