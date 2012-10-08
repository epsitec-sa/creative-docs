//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Pierre ARNAUD

using Epsitec.Common.Dialogs;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Business.UserManagement;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Dialogs
{
	/// <summary>
	/// The <c>LoginDialog</c> displays the dialog used to log in, based on the list of
	/// users provided by the <see cref="UserManager"/>.
	/// </summary>
	internal sealed class LoginDialog : AbstractDialog
	{
		public LoginDialog(CoreApp application, SoftwareUserEntity user, bool softwareStartup)
		{
			this.application     = application;
			this.data            = this.application.GetComponent<CoreData> ();
			this.manager         = this.data.GetComponent<UserManager> ();
			this.initialUser     = user;
			this.softwareStartup = softwareStartup;
			this.users           = this.manager.GetActiveUsers ().OrderBy (x => x.DisplayName).ToList ();
			this.OwnerWindow     = this.application.Window;
		}


		public SoftwareUserEntity				SelectedUser
		{
			get
			{
				int sel = this.table.SelectedRow;

				return (sel < 0) ? null : this.users[sel];
			}
		}

		public string							SelectedUserCode
		{
			get
			{
				if (this.SelectedUser.IsNull ())
				{
					return null;
				}
				else
				{
					return this.SelectedUser.Code;
				}
			}
		}
		
		
		protected override Window CreateWindow()
		{
			Window window = this.CreateEmptyWindow ();
			
			this.CreateUI (window);
			this.CreateEventHandlers  ();

			window.AdjustWindowSize ();

			return window;
		}

		private Window CreateEmptyWindow()
		{
			Window window = new Window ()
			{
				Icon = this.application.Window.Icon,
				Text = "Identification de l'utilisateur",
				ClientSize = new Size (450, 420),
			};
			
			window.MakeFixedSizeWindow ();

			return window;
		}

		private void CreateUI(Window window)
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
					DefHeight = Library.UI.Constants.ButtonLargeWidth + 1,
					StyleH = CellArrayStyles.Separator,
					StyleV = CellArrayStyles.ScrollNorm | CellArrayStyles.Separator | CellArrayStyles.SelectLine,
					Dock = DockStyle.Fill,
					TabIndex = tabIndex++,
				};

				this.table.SetArraySize (2, 0);
				this.table.SetWidthColumn (0, Library.UI.Constants.ButtonLargeWidth + 1);
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

				this.passwordField = new TextField
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

		private void CreateEventHandlers()
		{
			this.table.SelectionChanged += delegate
			{
				this.loginErrorCounter = 0;
				this.loginErrorMessage = null;

				this.UpdateWidgets ();

				this.passwordField.Text = null;
				this.passwordField.SelectAll ();
				this.passwordField.Focus ();
			};

			this.table.DoubleClicked       += this.HandleTableDoubleClicked;
			this.passwordField.TextChanged += sender => this.UpdateWidgets ();
			this.loginButton.Clicked       += this.HandleLoginButtonClicked;
			this.manageButton.Clicked      += (sender, e) => this.ProcessManage ();
			this.cancelButton.Clicked      += (sender, e) => this.ProcessClose (cancel: true);
		}

		
		private void ProcessClose(bool cancel)
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

		private void ProcessManage()
		{
			this.ProcessClose (cancel: true);

			var dialog = new Dialogs.UserManagerDialog (this.application, this.data, this.SelectedUser);
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

		private void UpdateWidgets()
		{
			var user = this.SelectedUser;
			bool passwordRequired = (user != null) && user.IsPasswordRequired;

			this.passBoxEdit.Visibility = (user == null ||  passwordRequired);
			this.passBoxInfo.Visibility = (user != null && !passwordRequired);

			this.loginButton.Enable  = (user != null && (!passwordRequired || !string.IsNullOrEmpty (this.passwordField.Text)));

			var message = this.GetErrorMessage ();

			if (message.IsNullOrEmpty ())
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
		
		private void TableFillRow(int row)
		{
			//	Peuple une ligne de la table, si nécessaire.
			if (this.table[0, row].IsEmpty)
			{
				var button = new IconOrImageButton
				{
					CoreData = this.data,
					PreferredSize = new Size (Library.UI.Constants.ButtonLargeWidth, Library.UI.Constants.ButtonLargeWidth),
					IconUri = Misc.IconProvider.GetResourceIconUri ("UserManager"),
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
			var person = user.People.NaturalPerson;

			var button = this.table[0, row].Children[0] as IconOrImageButton;
			button.ImageEntity = person.Pictures.FirstOrDefault ();

			var text = this.table[1, row].Children[0] as StaticText;
			text.FormattedText = user.ShortDescription;

			bool sel = (user == this.initialUser);
			this.table.SelectRow (row, sel);
		}



		private void HandleTableDoubleClicked(object sender, MessageEventArgs e)
		{
			this.ProcessLoginResult (this.ProcessTableDoubleClick ());
		}

		private void HandleLoginButtonClicked(object sender, MessageEventArgs e)
		{
			this.ProcessLoginResult (this.ProcessLoginButtonClicked ());
		}

		private void ProcessLoginResult(LoginResult result)
		{
			switch (result)
			{
				case LoginResult.OK:
					this.ProcessClose (cancel: false);
					break;

				case LoginResult.Quit:
					this.ProcessClose (cancel: true);
					break;
			}
		}
		
		private LoginResult ProcessTableDoubleClick()
		{
			var user = this.SelectedUser;
			System.Diagnostics.Debug.Assert (user != null);

			if (user.IsPasswordRequired)
			{
				return LoginResult.Retry;
			}
			else
			{
				return this.ProcessLoginButtonClicked ();
			}
		}

		private LoginResult ProcessLoginButtonClicked()
		{
			var user = this.SelectedUser;
			System.Diagnostics.Debug.Assert (user != null);

			if (this.manager.CheckUserAuthentication (user, this.passwordField.Text))
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

			this.passwordField.SelectAll ();
			this.passwordField.Focus ();

			return LoginResult.Retry;
		}

		private FormattedText GetErrorMessage()
		{
			return this.loginErrorMessage;
		}

		#region LoginResult Enumeration

		private enum LoginResult
		{
			OK,
			Retry,
			Quit,
		}

		#endregion

		
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
				FormattedText = TextFormatter.FormatText (new FormattedText ("<font size=\"40\"><b>"), number, new FormattedText (".</b></font>")),
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

			var columnTitle = new StaticText (rightPart);
			
			columnTitle.SetColumnTitle (text);

			return new FrameBox
			{
				Parent = rightPart,
				Dock = DockStyle.Fill,
			};
		}


		private readonly CoreApp								application;
		private readonly CoreData								data;
		private readonly UserManager							manager;
		private readonly SoftwareUserEntity						initialUser;
		private readonly bool									softwareStartup;
		private readonly List<SoftwareUserEntity>				users;

		private CellTable										table;
		private FrameBox										passBoxInfo;
		private FrameBox										passBoxEdit;
		private TextField										passwordField;
		private StaticText										errorMessage;
		private Button											loginButton;
		private Button											manageButton;
		private Button											cancelButton;
		private int												loginErrorCounter;
		private FormattedText									loginErrorMessage;
	}
}
