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
	/// Dialogue pour choisir l'utilisateur (loggin).
	/// </summary>
	class SelectUserDialog : AbstractDialog
	{
		public SelectUserDialog(CoreApplication application)
		{
			this.application = application;

			this.users = new List<SoftwareUserEntity> ();
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
			window.Text = "Choix de l'utilisateur";
			window.MakeFixedSizeWindow ();
			window.ClientSize = new Size (300, 300);

			window.WindowCloseClicked += delegate
			{
				this.OnDialogClosed ();
				this.CloseDialog ();
			};
		}

		protected void SetupWidgets(Window window)
		{
			this.list = new ScrollList
			{
				Parent = window.Root,
				Dock = DockStyle.Fill,
				Margins = new Margins (10, 10, 10, 10),
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

				this.quitButton = new Button ()
				{
					Parent = this.footer,
					Text = "Quitter",
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

			new Separator
			{
				Parent = window.Root,
				PreferredHeight = 1,
				Dock = DockStyle.Bottom,
			};

			{
				var passBox = new FrameBox
				{
					Parent = window.Root,
					PreferredHeight = 20,
					Dock = DockStyle.Bottom,
					Margins = new Margins (10, 10, 0, 10),
				};

				this.passLabel = new StaticText
				{
					Parent = passBox,
					Text = "Mot de passe",
					PreferredWidth = 80,
					Dock = DockStyle.Left,
				};

				this.passField = new TextField
				{
					Parent = passBox,
					IsPassword = true,
					PasswordReplacementCharacter = '●',
					Dock = DockStyle.Fill,
					TabIndex = 2,
				};
			}

			this.UpdateList ();
			this.UpdateWidgets ();
		}

		protected void SetupEvents(Window window)
		{
			this.list.SelectionActivated += delegate
			{
				this.UpdateWidgets ();

				this.passField.SelectAll ();
				this.passField.Focus ();
			};

			this.passField.TextChanged += delegate
			{
				this.UpdateWidgets ();
			};

			this.okButton.Clicked += delegate
			{
				if (this.CheckPassword ())
				{
					this.CloseDialog ();
				}
				else
				{
					FormattedText description = "Mot de passe incorrect";
					MessageDialog.CreateOk ("Erreur", DialogIcon.Warning, description.ToString ()).OpenDialog ();
				}
			};

			this.quitButton.Clicked += delegate
			{
				// TODO:
			};
		}


		private void UpdateList()
		{
			this.list.Items.Clear ();
			this.users.Clear ();

			var users = this.application.UserManager.GetActiveUsers ();

			foreach (var user in users)
			{
				if (user.IsArchive)
				{
					continue;
				}

				FormattedText text;

				if (user.DisplayName == user.LoginName)
				{
					text = user.DisplayName;
				}
				else
				{
					text = TextFormatter.FormatText (user.DisplayName, "(", user.LoginName, ")");
				}

				this.list.Items.Add (text);
				this.users.Add (user);
			}
		}

		private void UpdateWidgets()
		{
			int sel = this.list.SelectedItemIndex;
			var user = this.SelectedUser;
			bool hasPassword = (user != null && !string.IsNullOrEmpty (user.LoginPasswordHash));

			this.passLabel.Enable = (sel != -1);
			this.passField.Enable = (sel != -1);
			this.okButton.Enable  = (sel != -1 && (!hasPassword || !string.IsNullOrEmpty (this.passField.Text)));

			this.passLabel.Visibility = hasPassword;
			this.passField.Visibility = hasPassword;
		}


		private bool CheckPassword()
		{
			var user = this.SelectedUser;

			if (user == null)
			{
				return false;
			}

			if (string.IsNullOrEmpty (user.LoginPasswordHash))  // pas de mot de passe ?
			{
				return true;
			}

			return user.CheckPassword (this.passField.Text);
		}

		private SoftwareUserEntity SelectedUser
		{
			get
			{
				int sel = this.list.SelectedItemIndex;

				if (sel == -1)
				{
					return null;
				}

				return this.users[sel];
			}
		}


		protected override void OnDialogClosed()
		{
			base.OnDialogClosed ();
		}



		private readonly CoreApplication				application;
		private readonly List<SoftwareUserEntity>		users;

		private ScrollList								list;
		private StaticText								passLabel;
		private TextField								passField;
		private FrameBox								footer;
		private Button									okButton;
		private Button									quitButton;
	}
}
