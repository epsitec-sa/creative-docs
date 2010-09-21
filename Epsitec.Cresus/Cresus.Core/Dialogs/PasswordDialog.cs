//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Debug;
using Epsitec.Common.Dialogs;
using Epsitec.Common.Drawing;
using Epsitec.Common.IO;
using Epsitec.Common.Printing;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Printers;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Epsitec.Cresus.Core.Dialogs
{
	/// <summary>
	/// Dialogue pour donner le mot de passe d'un utilisateur.
	/// </summary>
	class PasswordDialog : AbstractDialog
	{
		public PasswordDialog(CoreApplication application, SoftwareUserEntity user)
		{
			this.application = application;
			this.user = user;
		}

		public string DefaultPassword
		{
			get
			{
				return this.passField.Text;
			}
		}


		protected override Window CreateWindow()
		{
			Window window = new Window ();

			this.SetupWindow  (window);
			this.SetupWidgets (window);
			this.SetupEvents  (window);

			window.AdjustWindowSize ();

			return window;
		}

		protected void SetupWindow(Window window)
		{
			this.OwnerWindow = this.application.Window;
			window.Icon = this.application.Window.Icon;
			window.Text = "Mot de passe";
			window.MakeFixedSizeWindow ();
			window.ClientSize = new Size (300, 170);

			window.WindowCloseClicked += delegate
			{
				this.OnDialogClosed ();
				this.CloseDialog ();
			};
		}

		protected void SetupWidgets(Window window)
		{
			new StaticText
			{
				Parent = window.Root,
				Text = "Utilisateur :",
				Dock = DockStyle.Top,
				Margins = new Margins (10, 10, 10, UIBuilder.MarginUnderLabel),
			};

			new TextField
			{
				Parent = window.Root,
				IsReadOnly = true,
				FormattedText = this.user.DisplayName,
				Dock = DockStyle.Top,
				TabIndex = 2,
				Margins = new Margins (10, 10, 0, 20),
			};


			new StaticText
			{
				Parent = window.Root,
				Text = "Mot de passe :",
				Dock = DockStyle.Top,
				Margins = new Margins (10, 10, 0, UIBuilder.MarginUnderLabel),
			};

			this.passField = new TextField
			{
				Parent = window.Root,
				IsPassword = true,
				PasswordReplacementCharacter = '●',
				Dock = DockStyle.Top,
				Margins = new Margins (10, 10, 0, 0),
				TabIndex = 1,
			};


			{
				this.footer = new FrameBox
				{
					Parent = window.Root,
					PreferredHeight = 20,
					Dock = DockStyle.Bottom,
					Margins = new Margins (10, 10, 10, 10),
				};

				this.cancelButton = new Button ()
				{
					Parent = this.footer,
					Text = "Annuler",
					ButtonStyle = Common.Widgets.ButtonStyle.DefaultCancel,
					Dock = DockStyle.Right,
					TabIndex = 11,
				};

				this.okButton = new Button ()
				{
					Parent = this.footer,
					Text = "S'identifier",
					ButtonStyle = Common.Widgets.ButtonStyle.DefaultAccept,
					Dock = DockStyle.Right,
					Margins = new Margins (0, 10, 0, 0),
					TabIndex = 10,
				};
			}

			this.UpdateWidgets ();

			this.passField.Focus ();
		}

		protected void SetupEvents(Window window)
		{
			this.passField.TextChanged += delegate
			{
				this.UpdateWidgets ();
			};

			this.okButton.Clicked += delegate
			{
				this.Result = DialogResult.Accept;
				this.CloseDialog ();
			};

			this.cancelButton.Clicked += delegate
			{
				this.Result = DialogResult.Cancel;
				this.CloseDialog ();
			};
		}


		private void UpdateWidgets()
		{
			this.okButton.Enable = !string.IsNullOrEmpty (this.passField.Text);
		}


		protected override void OnDialogClosed()
		{
			base.OnDialogClosed ();
		}



		private readonly CoreApplication				application;
		private readonly SoftwareUserEntity				user;

		private TextField								passField;
		private FrameBox								footer;
		private Button									okButton;
		private Button									cancelButton;
	}
}
