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
		public LoginDialog(CoreApplication application, SoftwareUserEntity user, bool softwareStartup)
		{
			this.application     = application;
			this.manager         = application.UserManager;
			this.initialUser     = user;
			this.softwareStartup = softwareStartup;

			this.users = this.manager.GetActiveUsers ().OrderBy (x => x.DisplayName).ToList ();
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
			window.ClientSize = new Size (450, 420);

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
				Margins = new Margins (10, 10, 0, 0),
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
				var part = new FrameBox
				{
					Parent = topPane,
					Dock = DockStyle.Fill,
				};

				var container = LoginDialog.CreateContainer (part, "1", "Identifiez-vous");

				this.table = new CellTable
				{
					Parent = container,
					DefHeight = Misc.GetButtonWidth () + 1,
					StyleH = CellArrayStyles.Separator,
					StyleV = CellArrayStyles.ScrollNorm | CellArrayStyles.Separator | CellArrayStyles.SelectLine,
					Dock = DockStyle.Fill,
					TabIndex = tabIndex++,
				};

				this.table.SetArraySize (2, 0);
				this.table.SetWidthColumn (0, Misc.GetButtonWidth () + 1);
				this.table.SetWidthColumn (1, 400);
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

				var container = LoginDialog.CreateContainer (this.passBoxEdit, "2", "Donnez votre mot de passe");

				var box = new FrameBox
				{
					Parent = container,
					Dock = DockStyle.Top,
					TabIndex = tabIndex++,
				};

				this.passField = new TextField
				{
					Parent = box,
					IsPassword = true,
					PasswordReplacementCharacter = '●',
					Dock = DockStyle.Fill,
					TabIndex = tabIndex++,
				};

				this.errorMessage = new StaticText
				{
					Parent = box,
					Visibility = false,
					PreferredWidth = 200,
					ContentAlignment = Common.Drawing.ContentAlignment.MiddleCenter,
					Dock = DockStyle.Right,
					Margins = new Margins (10, 0, 0, 0),
				};
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

				var container = LoginDialog.CreateContainer (this.passBoxInfo, "2", "Mot de passe");

				new StaticText
				{
					Parent = container,
					FormattedText = "<i>Il n'est pas nécessaire de donner le mot de passe, car l'utilisateur correspond à la session Windows en cours.</i>",
					TextBreakMode = Common.Drawing.TextBreakMode.Hyphenate,
					ContentAlignment = Common.Drawing.ContentAlignment.TopLeft,
					Dock = DockStyle.Fill,
				};
			}

			//	Crée le pied de page.
			{
				this.manageButton = new Button ()
				{
					Parent = footer,
					ButtonStyle = ButtonStyle.ToolItem,  // boutons sans cadre, pour le différencier des 2 autres
					Text = "Gérer les comptes...",
					PreferredWidth = 120,
					Visibility = !this.softwareStartup && this.manager.IsUserAtPowerLevel (this.initialUser, UserPowerLevel.Administrator),
					Dock = DockStyle.Left,
					TabIndex = tabIndex++,
				};

				this.cancelButton = new Button ()
				{
					Parent = footer,
					Text = this.softwareStartup ? "Quitter" : "Annuler",
					ButtonStyle = Common.Widgets.ButtonStyle.DefaultCancel,
					Dock = DockStyle.Right,
					TabIndex = tabIndex++,
				};

				this.loginButton = new Button ()
				{
					Parent = footer,
					Text = "S'identifier",
					ButtonStyle = Common.Widgets.ButtonStyle.DefaultAccept,
					Dock = DockStyle.Right,
					Margins = new Margins (0, 10, 0, 0),
					TabIndex = tabIndex++,
				};
			}

			this.UpdateTable ();
			this.UpdateWidgets ();
		}

		protected void SetupEvents(Window window)
		{
			this.table.SelectionChanged += delegate
			{
				this.loginErrorCounter = 0;
				this.loginErrorMessage = null;

				this.UpdateWidgets ();

				this.passField.Text = null;
				this.passField.SelectAll ();
				this.passField.Focus ();
			};

			this.table.DoubleClicked += delegate
			{
				var result = this.DoubleClickedAction ();

				if (result == LoginResult.OK)
				{
					this.CloseAction (cancel: false);
				}

				if (result == LoginResult.Quit)
				{
					this.CloseAction (cancel: true);
				}
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
				this.manager.DiscardChangesAndDisposeBusinessContext ();

				this.Result = DialogResult.Cancel;
			}
			else
			{
				this.manager.SaveChangesAndDisposeBusinessContext ();

				this.Result = DialogResult.Accept;
			}

			this.CloseDialog ();
		}

		private void ManageAction()
		{
			this.CloseAction (cancel: true);

			var dialog = new Dialogs.UserManagerDialog (CoreProgram.Application, this.SelectedUser);
			dialog.IsModal = true;
			dialog.OpenDialog ();
		}


		private void UpdateTable()
		{
			//	Met à jour le contenu de la table.
			int rows = this.users.Count;
			this.table.SetArraySize (2, rows);

			for (int row=0; row<rows; row++)
			{
				this.TableFillRow (row);
				this.TableUpdateRow (row);
			}
		}

		private void TableFillRow(int row)
		{
			//	Peuple une ligne de la table, si nécessaire.
			if (this.table[0, row].IsEmpty)
			{
				var button = new IconOrImageButton
				{
					CoreData = this.manager.CoreData,
					PreferredSize = new Size (Misc.GetButtonWidth (), Misc.GetButtonWidth ()),
					IconUri = Misc.GetResourceIconUri ("UserManager"),
					IconPreferredSize = new Size (31, 31),
					Enable = false,
					Dock = DockStyle.Fill,
					Margins = new Margins (0, 0, 0, 0),
				};

				this.table[0, row].Insert (button);
			}

			if (this.table[1, row].IsEmpty)
			{
				var text = new StaticText
				{
					ContentAlignment = ContentAlignment.MiddleLeft,
					Dock = DockStyle.Fill,
					Margins = new Margins (4, 4, 0, 0),
				};

				this.table[1, row].Insert (text);
			}
		}

		private void TableUpdateRow(int row)
		{
			//	Met à jour le contenu d'une ligne de la table.
			var user = this.users[row];

			var button = this.table[0, row].Children[0] as IconOrImageButton;
			button.ImageEntity = user.Person.Pictures.FirstOrDefault ();

			var text = this.table[1, row].Children[0] as StaticText;
			text.FormattedText = user.ShortDescription;

			bool sel = (user == this.initialUser);
			this.table.SelectRow (row, sel);
		}


		private void UpdateWidgets()
		{
			var user = this.SelectedUser;
			bool passwordRequired = UserManager.IsPasswordRequired (user);

			this.passBoxEdit.Visibility = (user == null ||  passwordRequired);
			this.passBoxInfo.Visibility = (user != null && !passwordRequired);

			this.loginButton.Enable  = (user != null && (!passwordRequired || !string.IsNullOrEmpty (this.passField.Text)));

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


		private LoginResult DoubleClickedAction()
		{
			var user = this.SelectedUser;
			System.Diagnostics.Debug.Assert (user != null);

			if (UserManager.IsPasswordRequired (user))
			{
				return LoginResult.Retry;
			}
			else
			{
				return this.LoginAction ();
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
				int sel = this.table.SelectedRow;

				if (sel == -1)
				{
					return null;
				}

				return this.users[sel];
			}
		}



		private static FrameBox CreateContainer(Widget parent, FormattedText number, FormattedText text)
		{
			//	Crée un container de cette forme:
			//
			//	+---+------------+
			//	| n | text       |
			//	|   +------------+
			//	|   |            |
			//	|   | FrameBox   |
			//	|   | returned   |
			//	|   |            |
			//	+---+------------+

			new StaticText
			{
				Parent = parent,
				FormattedText = TextFormatter.FormatText ("<font size=\"40\"><b>", number, ".</b></font>"),
				ContentAlignment = Common.Drawing.ContentAlignment.TopLeft,
				PreferredWidth = 50,
				Dock = DockStyle.Left,
			};

			var rightPart = new FrameBox
			{
				Parent = parent,
				Dock = DockStyle.Fill,
				Margins = new Margins (0, 0, 10, 0),
			};

			new StaticText
			{
				Parent = rightPart,
				FormattedText = TextFormatter.FormatText ("<font size=\"16\">", text, "</font>"),
				ContentAlignment = Common.Drawing.ContentAlignment.TopLeft,
				PreferredHeight = 20,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 0, 5),
			};

			return new FrameBox
			{
				Parent = rightPart,
				Dock = DockStyle.Fill,
			};
		}


		protected override void OnDialogClosed()
		{
			base.OnDialogClosed ();
		}



		private readonly CoreApplication						application;
		private readonly UserManager							manager;
		private readonly SoftwareUserEntity						initialUser;
		private readonly bool									softwareStartup;
		private readonly List<SoftwareUserEntity>				users;

		private CellTable										table;
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
