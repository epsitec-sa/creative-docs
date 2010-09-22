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

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Epsitec.Cresus.Core.Dialogs
{
	/// <summary>
	/// Dialogue pour choisir l'utilisateur (loggin).
	/// </summary>
	class UserManagerDialog : AbstractDialog
	{
		public UserManagerDialog(CoreApplication application)
		{
			this.application = application;

			this.users = new List<SoftwareUserEntity> ();
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
			window.Text = "Gestion des utilisateurs";
			window.MakeFixedSizeWindow ();
			window.ClientSize = new Size (540, 400);

			window.WindowCloseClicked += delegate
			{
				this.OnDialogClosed ();
				this.CloseDialog ();
			};
		}

		protected void SetupWidgets(Window window)
		{
			var topPane = new FrameBox
			{
				Parent = window.Root,
				Dock = DockStyle.Fill,
				Margins = new Margins (10, 10, 10, 10),
			};

			var leftPane = new FrameBox
			{
				Parent = topPane,
				Dock = DockStyle.Fill,
				Margins = new Margins (0, 4, 0, 0),
			};

			var rightPane = new FrameBox
			{
				Parent = topPane,
				PreferredWidth = 250,
				Dock = DockStyle.Right,
				Margins = new Margins (0, 0, 0, 0),
			};

			var footer = new FrameBox
			{
				Parent = window.Root,
				PreferredHeight = 20,
				Dock = DockStyle.Bottom,
				Margins = new Margins (10, 10, 0, 10),
			};

			//	Crée le panneau de gauche.
			new StaticText
			{
				Parent = leftPane,
				PreferredHeight = 20,
				Dock = DockStyle.Top,
				Text = "<font size=\"16\">Liste des utilisateurs</font>",
				Margins = new Margins (0, 0, 0, 10),
			};

			{
				//	Crée la toolbar.
				double buttonSize = 19;

				this.toolbar = UIBuilder.CreateMiniToolbar (leftPane, buttonSize);
				this.toolbar.Margins = new Margins (0, TileArrow.Breadth, 0, -1);
				this.toolbar.TabIndex = 1;

				this.addButton = new GlyphButton
				{
					Parent = toolbar,
					PreferredSize = new Size (buttonSize*2+1, buttonSize),
					GlyphShape = GlyphShape.Plus,
					Margins = new Margins (0, 0, 0, 0),
					Dock = DockStyle.Left,
				};

				this.removeButton = new GlyphButton
				{
					Parent = toolbar,
					PreferredSize = new Size (buttonSize, buttonSize),
					GlyphShape = GlyphShape.Minus,
					Margins = new Margins (1, 0, 0, 0),
					Dock = DockStyle.Left,
				};

				ToolTip.Default.SetToolTip (this.addButton,    "Ajoute un nouvel utilisateur");
				ToolTip.Default.SetToolTip (this.removeButton, "Supprime l'utilisateur sélectionné");
			}

			{
				//	Crée la liste.
				var tile = new ArrowedFrame
				{
					Parent = leftPane,
					ArrowDirection = Direction.Right,
					Dock = DockStyle.Fill,
					Padding = new Margins (1, TileArrow.Breadth+1, 1, 1),
					TabIndex = 2,
				};

				this.list = new ScrollList
				{
					Parent = tile,
					ScrollListStyle = Common.Widgets.ScrollListStyle.FrameLess,
					Dock = DockStyle.Fill,
					TabIndex = 1,
				};
			}

			{
				//	Crée le groupe pour le mot de passe.
				var box = new FrameBox
				{
					Parent = leftPane,
					DrawFullFrame = true,
					PreferredHeight = 60,
					Dock = DockStyle.Bottom,
					Margins = new Margins (0, TileArrow.Breadth, -1, 0),
					Padding = new Margins (10),
				};

				this.passLabel = new StaticText
				{
					Parent = box,
					Text = "Mot de passe :",
					Dock = DockStyle.Top,
					Margins = new Margins (0, 0, 0, UIBuilder.MarginUnderLabel),
				};

				this.passField = new TextField
				{
					Parent = box,
					IsPassword = true,
					PasswordReplacementCharacter = '●',
					Dock = DockStyle.Top,
					TabIndex = 2,
				};
			}

			//	Crée le panneau de droite.
			new StaticText
			{
				Parent = rightPane,
				PreferredHeight = 20,
				Dock = DockStyle.Top,
				Text = "<font size=\"16\">Paramètres d'un utilisateur</font>",
				Margins = new Margins (0, 0, 0, 10),
			};

			this.userBox = new FrameBox
			{
				Parent = rightPane,
				DrawFullFrame = true,
				PreferredWidth = 300,
				Dock = DockStyle.Fill,
				Padding = new Margins (10),
			};

			{
				new StaticText
				{
					Parent = this.userBox,
					Text = "Nom de compte :",
					Dock = DockStyle.Top,
					Margins = new Margins (0, 0, 0, UIBuilder.MarginUnderLabel),
				};

				this.loginNameField = new TextField
				{
					Parent = this.userBox,
					Dock = DockStyle.Top,
					Margins = new Margins (0, 0, 0, 10),
				};


				new StaticText
				{
					Parent = this.userBox,
					Text = "Nom complet de l'utilisateur :",
					Dock = DockStyle.Top,
					Margins = new Margins (0, 0, 0, UIBuilder.MarginUnderLabel),
				};

				this.displayNameField = new TextField
				{
					Parent = this.userBox,
					Dock = DockStyle.Top,
					Margins = new Margins (0, 0, 0, 10),
				};


				new StaticText
				{
					Parent = this.userBox,
					Text = "Niveau de l'utilisateur :",
					Dock = DockStyle.Top,
					Margins = new Margins (0, 0, 0, UIBuilder.MarginUnderLabel+2),
				};

				this.checkAdministrator = new CheckButton
				{
					Parent = this.userBox,
					Text = "Administrateur",
					Dock = DockStyle.Top,
				};

				this.checkSystem = new CheckButton
				{
					Parent = this.userBox,
					Text = "Système",
					Dock = DockStyle.Top,
				};

				this.checkDeveloper = new CheckButton
				{
					Parent = this.userBox,
					Text = "Développeur",
					Dock = DockStyle.Top,
				};

				this.checkPowerUser = new CheckButton
				{
					Parent = this.userBox,
					Text = "Utilisateur puissant",
					Dock = DockStyle.Top,
				};

				this.checkStandard = new CheckButton
				{
					Parent = this.userBox,
					Text = "Utilisateur standard",
					Dock = DockStyle.Top,
				};

				this.checkRestricted = new CheckButton
				{
					Parent = this.userBox,
					Text = "Utilisateur restreint",
					Dock = DockStyle.Top,
				};


				new StaticText
				{
					Parent = this.userBox,
					Text = "Nouveau mot de passe :",
					Dock = DockStyle.Top,
					Margins = new Margins (0, 0, 10, UIBuilder.MarginUnderLabel),
				};

				this.newPasswordField1 = new TextField
				{
					Parent = this.userBox,
					IsPassword = true,
					PasswordReplacementCharacter = '●',
					Dock = DockStyle.Top,
					Margins = new Margins (0, 0, 0, 2),
				};

				this.newPasswordField2 = new TextField
				{
					Parent = this.userBox,
					IsPassword = true,
					PasswordReplacementCharacter = '●',
					Dock = DockStyle.Top,
					Margins = new Margins (0, 0, 0, 2),
				};


				this.applyButton = new Button
				{
					Parent = this.userBox,
					Text = "Appliquer les modifications",
					Dock = DockStyle.Bottom,
					Margins = new Margins (0, 0, 10, 0),
				};
			}

			//	Crée le pied de page.
			{
				this.quitButton = new Button ()
				{
					Parent = footer,
					Text = "Quitter",
					ButtonStyle = Common.Widgets.ButtonStyle.DefaultCancel,
					Dock = DockStyle.Right,
					TabIndex = 11,
				};

				this.okButton = new Button ()
				{
					Parent = footer,
					Text = "S'identifier",
					ButtonStyle = Common.Widgets.ButtonStyle.DefaultAccept,
					Dock = DockStyle.Right,
					Margins = new Margins (0, 10, 0, 0),
					TabIndex = 10,
				};
			}

			this.UpdateList ();
			this.UpdateUser ();
			this.UpdateWidgets ();
		}

		protected void SetupEvents(Window window)
		{
			this.list.SelectionActivated += delegate
			{
				this.UpdateUser ();
				this.UpdateWidgets ();

				this.passField.SelectAll ();
				this.passField.Focus ();
			};

			this.passField.TextChanged += delegate
			{
				this.UpdateWidgets ();
			};

			this.loginNameField.TextChanged += delegate
			{
				this.dirtyContent = true;
				this.UpdateWidgets ();
			};

			this.displayNameField.TextChanged += delegate
			{
				this.dirtyContent = true;
				this.UpdateWidgets ();
			};

			this.newPasswordField1.TextChanged += delegate
			{
				this.dirtyContent = true;
				this.UpdateWidgets ();
			};

			this.newPasswordField2.TextChanged += delegate
			{
				this.dirtyContent = true;
				this.UpdateWidgets ();
			};


			this.checkAdministrator.ActiveStateChanged += delegate
			{
				this.dirtyContent = true;
				this.UpdateWidgets ();
			};

			this.checkSystem.ActiveStateChanged += delegate
			{
				this.dirtyContent = true;
				this.UpdateWidgets ();
			};

			this.checkDeveloper.ActiveStateChanged += delegate
			{
				this.dirtyContent = true;
				this.UpdateWidgets ();
			};

			this.checkPowerUser.ActiveStateChanged += delegate
			{
				this.dirtyContent = true;
				this.UpdateWidgets ();
			};

			this.checkStandard.ActiveStateChanged += delegate
			{
				this.dirtyContent = true;
				this.UpdateWidgets ();
			};

			this.checkRestricted.ActiveStateChanged += delegate
			{
				this.dirtyContent = true;
				this.UpdateWidgets ();
			};


			this.applyButton.Clicked += delegate
			{
				this.ApplyModifications ();
			};

			this.okButton.Clicked += delegate
			{
				this.Result = DialogResult.Accept;
				this.CloseDialog ();
			};

			this.quitButton.Clicked += delegate
			{
				this.Result = DialogResult.Cancel;
				this.CloseDialog ();
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
					text = user.LoginName;
				}
				else
				{
					text = TextFormatter.FormatText (user.LoginName, "(", user.DisplayName, ")");
				}

				this.list.Items.Add (text);
				this.users.Add (user);
			}
		}

		private void UpdateUser()
		{
			var user = this.SelectedUser;

			if (user == null)
			{
				this.userBox.Enable = false;

				this.loginNameField.Text = null;
				this.displayNameField.Text = null;
				this.newPasswordField1.Text = null;
				this.newPasswordField2.Text = null;
				
				this.checkAdministrator.ActiveState = ActiveState.No;
				this.checkSystem       .ActiveState = ActiveState.No;
				this.checkDeveloper    .ActiveState = ActiveState.No;
				this.checkPowerUser    .ActiveState = ActiveState.No;
				this.checkStandard     .ActiveState = ActiveState.No;
				this.checkRestricted   .ActiveState = ActiveState.No;
			}
			else
			{
				this.userBox.Enable = true;

				this.loginNameField.Text = user.LoginName;
				this.displayNameField.FormattedText = user.DisplayName;
				this.newPasswordField1.Text = null;
				this.newPasswordField2.Text = null;

				this.checkAdministrator.ActiveState = this.IsPowerLevelUsed (Business.UserManagement.UserPowerLevel.Administrator) ? ActiveState.Yes : ActiveState.No;
				this.checkSystem       .ActiveState = this.IsPowerLevelUsed (Business.UserManagement.UserPowerLevel.System       ) ? ActiveState.Yes : ActiveState.No;
				this.checkDeveloper    .ActiveState = this.IsPowerLevelUsed (Business.UserManagement.UserPowerLevel.Developer    ) ? ActiveState.Yes : ActiveState.No;
				this.checkPowerUser    .ActiveState = this.IsPowerLevelUsed (Business.UserManagement.UserPowerLevel.PowerUser    ) ? ActiveState.Yes : ActiveState.No;
				this.checkStandard     .ActiveState = this.IsPowerLevelUsed (Business.UserManagement.UserPowerLevel.Standard     ) ? ActiveState.Yes : ActiveState.No;
				this.checkRestricted   .ActiveState = this.IsPowerLevelUsed (Business.UserManagement.UserPowerLevel.Restricted   ) ? ActiveState.Yes : ActiveState.No;
			}

			this.dirtyContent = false;
		}

		private void UpdateWidgets()
		{
			int sel = this.list.SelectedItemIndex;
			var user = this.SelectedUser;
			bool hasPassword = (user != null && user.AuthenticationMethod == Business.UserManagement.UserAuthenticationMethod.Password);

			if (user != null && user.LoginName == System.Environment.UserName)
			{
				hasPassword = false;
			}

			this.passLabel.Enable = (sel != -1);
			this.passField.Enable = (sel != -1);
			this.okButton.Enable  = (sel != -1 && (!hasPassword || !string.IsNullOrEmpty (this.passField.Text)));

			this.applyButton.Enable = this.dirtyContent;

			this.passLabel.Visibility = hasPassword;
			this.passField.Visibility = hasPassword;
		}


		private void ApplyModifications()
		{
			var user = this.SelectedUser;

			if (user != null)
			{
				user.LoginName = this.loginNameField.Text;
				user.DisplayName = this.displayNameField.FormattedText;
				this.UpdateList ();
			}

			this.SetPowerLevelUsed ();
		}


		private void SetPowerLevelUsed()
		{
			var user = this.SelectedUser;

			if (user != null)
			{
				user.UserGroups.Clear ();

				if (this.checkAdministrator.ActiveState == ActiveState.Yes)
				{
					// TODO:
				}

				if (this.checkSystem.ActiveState == ActiveState.Yes)
				{
				}

				if (this.checkDeveloper.ActiveState == ActiveState.Yes)
				{
				}

				if (this.checkPowerUser.ActiveState == ActiveState.Yes)
				{
				}

				if (this.checkPowerUser.ActiveState == ActiveState.Yes)
				{
				}

				if (this.checkStandard.ActiveState == ActiveState.Yes)
				{
				}

				if (this.checkRestricted.ActiveState == ActiveState.Yes)
				{
				}
			}
		}

		private bool IsPowerLevelUsed(Business.UserManagement.UserPowerLevel level)
		{
			var user = this.SelectedUser;

			if (user != null)
			{
				foreach (var group in user.UserGroups)
				{
					if (group.UserPowerLevel == level)
					{
						return true;
					}
				}
			}

			return false;
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


		protected override void OnDialogClosed()
		{
			base.OnDialogClosed ();
		}



		private readonly CoreApplication				application;
		private readonly List<SoftwareUserEntity>		users;

		private FrameBox								toolbar;
		private GlyphButton								addButton;
		private GlyphButton								removeButton;
		private ScrollList								list;
		private StaticText								passLabel;
		private TextField								passField;
		private FrameBox								userBox;
		private TextField								loginNameField;
		private TextField								displayNameField;
		private CheckButton								checkAdministrator;
		private CheckButton								checkSystem;
		private CheckButton								checkDeveloper;
		private CheckButton								checkPowerUser;
		private CheckButton								checkStandard;
		private CheckButton								checkRestricted;
		private TextField								newPasswordField1;
		private TextField								newPasswordField2;
		private Button									applyButton;
		private bool									dirtyContent;
		private Button									okButton;
		private Button									quitButton;
	}
}
