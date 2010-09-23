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
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Business.UserManagement;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Epsitec.Cresus.Core.Dialogs
{
	/// <summary>
	/// Dialogue pour choisir l'utilisateur (loggin).
	/// </summary>
	class LoginDialog : AbstractDialog
	{
		public LoginDialog(CoreApplication application, SoftwareUserEntity user, bool hasQuitButton)
		{
			this.application   = application;
			this.manager       = application.UserManager;
			this.initialUser   = user;
			this.hasQuitButton = hasQuitButton;

			this.users  = this.manager.GetActiveUsers ().ToList ();
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
			window.Text = "Identification de l'utilisateur";
			window.MakeFixedSizeWindow ();
			window.ClientSize = new Size (400, 400);

			window.WindowCloseClicked += delegate
			{
				this.OnDialogClosed ();
				this.CloseDialog ();
			};
		}

		protected void SetupWidgets(Window window)
		{
			int tabIndex = 1;

			var topPane = new FrameBox
			{
				Parent = window.Root,
				Dock = DockStyle.Fill,
				Margins = new Margins (10, 10, 10, 0),
				TabIndex = tabIndex++,
			};

			var footer = new FrameBox
			{
				Parent = window.Root,
				PreferredHeight = 20,
				Dock = DockStyle.Bottom,
				Margins = new Margins (10, 10, 10, 10),
				TabIndex = tabIndex++,
			};

			new Separator
			{
				Parent = window.Root,
				PreferredHeight = 1,
				Dock = DockStyle.Bottom,
			};

			//	Crée la liste.
			{
				new StaticText
				{
					Parent = topPane,
					PreferredHeight = 30,
					Dock = DockStyle.Top,
					FormattedText = LoginDialog.GetInstruction ("1", "Identifiez-vous"),
					Margins = new Margins (0, 0, 0, 5),
				};

				this.list = new ScrollList
				{
					Parent = topPane,
					Dock = DockStyle.Fill,
					TabIndex = tabIndex++,
				};
			}

			//	Crée le groupe pour le mot de passe éditable.
			{
				this.passBoxEdit = new FrameBox
				{
					Parent = topPane,
					PreferredHeight = 72,
					Dock = DockStyle.Bottom,
					Margins = new Margins (0, 0, 10, 0),
					TabIndex = tabIndex++,
				};

				new StaticText
				{
					Parent = this.passBoxEdit,
					PreferredHeight = 30,
					Dock = DockStyle.Top,
					FormattedText = LoginDialog.GetInstruction ("2", "Donnez votre mot de passe"),
					Margins = new Margins (0, 0, 0, 5),
				};

				{
					var band = new FrameBox
					{
						Parent = this.passBoxEdit,
						Dock = DockStyle.Top,
						TabIndex = tabIndex++,
					};

					this.passField = new TextField
					{
						Parent = band,
						IsPassword = true,
						PasswordReplacementCharacter = '●',
						Dock = DockStyle.Fill,
						TabIndex = tabIndex++,
					};

					this.errorMessage = new StaticText
					{
						Parent = band,
						Visibility = false,
						PreferredWidth = 200,
						ContentAlignment = Common.Drawing.ContentAlignment.MiddleCenter,
						Dock = DockStyle.Right,
						Margins = new Margins (10, 0, 0, 0),
					};
				}
			}

			//	Crée le groupe pour le mot de passe inutile.
			{
				this.passBoxInfo = new FrameBox
				{
					Parent = topPane,
					PreferredHeight = 72,
					Dock = DockStyle.Bottom,
					Margins = new Margins (0, 0, 10, 0),
					TabIndex = tabIndex++,
				};

				new StaticText
				{
					Parent = this.passBoxInfo,
					PreferredHeight = 30,
					Dock = DockStyle.Top,
					FormattedText = LoginDialog.GetInstruction ("2", "Mot de passe"),
					Margins = new Margins (0, 0, 0, 5),
				};

				new StaticText
				{
					Parent = passBoxInfo,
					FormattedText = "<i>Il n'est pas nécessaire de donner le mot de passe, car l'utilisateur correspond à la session Windows en cours.</i>",
					TextBreakMode = Common.Drawing.TextBreakMode.Hyphenate,
					ContentAlignment = Common.Drawing.ContentAlignment.TopLeft,
					Dock = DockStyle.Fill,
					Margins = new Margins (41, 0, 0, 0),  // 41 = largeur occupée par "2. " en taille 32 (voir LoginDialog.GetInstruction) !
				};
			}

			//	Crée le pied de page.
			{
				this.cancelButton = new Button ()
				{
					Parent = footer,
					Text = this.hasQuitButton ? "Quitter" : "Annuler",
					PreferredWidth = 60,
					ButtonStyle = Common.Widgets.ButtonStyle.DefaultCancel,
					Dock = DockStyle.Right,
					TabIndex = tabIndex++,
				};

				this.manageButton = new Button ()
				{
					Parent = footer,
					Text = "Gérer les comptes",
					PreferredWidth = 100,
					Visibility = !this.hasQuitButton,
					Dock = DockStyle.Right,
					Margins = new Margins (0, 10, 0, 0),
					TabIndex = tabIndex++,
				};

				this.loginButton = new Button ()
				{
					Parent = footer,
					Text = "S'identifier",
					PreferredWidth = 100,
					ButtonStyle = Common.Widgets.ButtonStyle.DefaultAccept,
					Dock = DockStyle.Right,
					Margins = new Margins (0, 10, 0, 0),
					TabIndex = tabIndex++,
				};
			}

			this.UpdateList ();
			this.UpdateWidgets ();
		}

		protected void SetupEvents(Window window)
		{
			this.list.SelectionActivated += delegate
			{
				this.loginErrorCounter = 0;
				this.loginErrorMessage = null;

				this.UpdateWidgets ();

				this.passField.Text = null;
				this.passField.SelectAll ();
				this.passField.Focus ();
			};

			this.passField.TextChanged += delegate
			{
				this.UpdateWidgets ();
			};

			this.loginButton.Clicked += delegate
			{
				var result = this.LoginAction ();

				if (result == LoginResult.OK)
				{
					this.CloseAction (cancel: false);
				}

				if (result == LoginResult.Quit)
				{
					this.CloseAction (cancel: true);
				}
			};

			this.manageButton.Clicked += delegate
			{
				this.ManageAction ();
			};

			this.cancelButton.Clicked += delegate
			{
				this.CloseAction (cancel: true);
			};
		}

		private void CloseAction(bool cancel)
		{
			if (cancel)
			{
				this.manager.DiscardChanges ();

				this.Result = DialogResult.Cancel;
			}
			else
			{
				this.manager.SaveChanges ();

				this.Result = DialogResult.Accept;
			}

			this.CloseDialog ();
		}

		private void ManageAction()
		{
			this.CloseAction (cancel: true);

			var dialog = new Dialogs.UserManagerDialog (CoreProgram.Application, this.SelectedUser, this.hasQuitButton);
			dialog.IsModal = true;
			dialog.OpenDialog ();
		}


		private void UpdateList()
		{
			int sel = -1;
			this.list.Items.Clear ();

			foreach (var user in this.users)
			{
				if (user == this.initialUser)
				{
					sel = this.list.Items.Count;
				}

				FormattedText text;

				if (user.DisplayName == user.LoginName)
				{
					text = user.LoginName;
				}
				else
				{
					text = TextFormatter.FormatText (user.LoginName, "(", user.DisplayName, ")");
				}

				this.list.Items.Add (text);
			}

			this.list.SelectedItemIndex = sel;
		}

		private void UpdateWidgets()
		{
			var user = this.SelectedUser;
			bool hasPassword = (user != null && user.AuthenticationMethod == Business.UserManagement.UserAuthenticationMethod.Password);

			if (user != null && user.LoginName == System.Environment.UserName)
			{
				//	Si le nom du compte est le même que l'utilisateur Windows en cours, le mot de passe n'est pas requis.
				hasPassword = false;
			}

			this.passBoxEdit.Visibility = (user == null ||  hasPassword);
			this.passBoxInfo.Visibility = (user != null && !hasPassword);

			this.loginButton.Enable  = (user != null && (!hasPassword || !string.IsNullOrEmpty (this.passField.Text)));

			var message = this.GetErrorMessage ();
			if (message == null)
			{
				this.errorMessage.Visibility = false;
			}
			else
			{
				this.errorMessage.Visibility = true;
				this.errorMessage.BackColor = Color.FromName ("Gold");
				this.errorMessage.FormattedText = message;
			}
		}


		private LoginResult LoginAction()
		{
			var user = this.SelectedUser;
			System.Diagnostics.Debug.Assert (user != null);

			if (this.manager.CheckUserAuthentication (user, this.passField.Text))
			{
				return LoginResult.OK;
			}

			this.loginErrorCounter++;

			if (this.loginErrorCounter >= 3)
			{
				return LoginResult.Quit;
			}

			this.loginErrorMessage = string.Format ("Mot de passe incorrect, réessayez ({0}/3).", this.loginErrorCounter.ToString ());
			this.UpdateWidgets ();

			this.passField.SelectAll ();
			this.passField.Focus ();

			return LoginResult.Retry;
		}

		private enum LoginResult
		{
			OK,
			Retry,
			Quit,
		}


		private FormattedText GetErrorMessage()
		{
			if (!this.loginErrorMessage.IsNullOrEmpty)
			{
				return this.loginErrorMessage;
			}

			return null;
		}


		public SoftwareUserEntity SelectedUser
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


		private static FormattedText GetInstruction(string number, string text)
		{
			return TextFormatter.FormatText ("<font size=\"32\"><b>", number, ". </b></font><font size=\"16\">", text, "</font>");
		}


		protected override void OnDialogClosed()
		{
			base.OnDialogClosed ();
		}



		private readonly CoreApplication						application;
		private readonly UserManager							manager;
		private readonly SoftwareUserEntity						initialUser;
		private readonly bool									hasQuitButton;
		private readonly List<SoftwareUserEntity>				users;

		private ScrollList										list;
		private FrameBox										passBoxInfo;
		private FrameBox										passBoxEdit;
		private TextField										passField;
		private StaticText										errorMessage;
		private Button											loginButton;
		private Button											manageButton;
		private Button											cancelButton;
		private int												loginErrorCounter;
		private FormattedText									loginErrorMessage;
	}
}
