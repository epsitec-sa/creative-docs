﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
	class UserManagerDialog : AbstractDialog
	{
		public UserManagerDialog(CoreApplication application, SoftwareUserEntity user, bool hasQuitButton)
		{
			this.application   = application;
			this.manager       = application.UserManager;
			this.initialUser   = user;
			this.hasQuitButton = hasQuitButton;

			this.users  = this.manager.GetActiveUsers ().ToList ();
			this.groups = this.manager.GetActiveUserGroups ().ToList ();

			this.checkButtonGroups = new List<CheckButton> ();
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
			window.Text = "Gestion des comptes utilisateurs";
			window.MakeFixedSizeWindow ();
			window.ClientSize = new Size (560, 400);

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
				Margins = new Margins (10, 10, 10, 10),
				TabIndex = tabIndex++,
			};

			var leftPane = new FrameBox
			{
				Parent = topPane,
				Dock = DockStyle.Fill,
				Margins = new Margins (0, 4, 0, 0),
				TabIndex = tabIndex++,
			};

			var rightPane = new FrameBox
			{
				Parent = topPane,
				PreferredWidth = 260,
				Dock = DockStyle.Right,
				Margins = new Margins (0, 0, 0, 0),
				TabIndex = tabIndex++,
			};

			var footer = new FrameBox
			{
				Parent = window.Root,
				PreferredHeight = 20,
				Dock = DockStyle.Bottom,
				Margins = new Margins (10, 10, 0, 10),
				TabIndex = tabIndex++,
			};

			//	Crée le panneau de gauche.
			new StaticText
			{
				Parent = leftPane,
				PreferredHeight = 20,
				Dock = DockStyle.Top,
				Text = "<font size=\"16\">Liste des comptes utilisateurs</font>",
				Margins = new Margins (0, 0, 0, 10),
			};

			{
				//	Crée la toolbar.
				double buttonSize = 19;

				this.toolbar = UIBuilder.CreateMiniToolbar (leftPane, buttonSize);
				this.toolbar.Margins = new Margins (0, TileArrow.Breadth, 0, -1);
				this.toolbar.TabIndex = tabIndex++;

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

				ToolTip.Default.SetToolTip (this.addButton,    "Crée un nouveau compte utilisateur");
				ToolTip.Default.SetToolTip (this.removeButton, "Supprime le compte utilisateur sélectionné");
			}

			{
				//	Crée la liste.
				var tile = new ArrowedFrame
				{
					Parent = leftPane,
					ArrowDirection = Direction.Right,
					Dock = DockStyle.Fill,
					Padding = new Margins (1, TileArrow.Breadth+1, 1, 1),
					TabIndex = tabIndex++,
				};

				this.list = new ScrollList
				{
					Parent = tile,
					ScrollListStyle = Common.Widgets.ScrollListStyle.FrameLess,
					Dock = DockStyle.Fill,
					TabIndex = tabIndex++,
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
					TabIndex = tabIndex++,
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
					TabIndex = tabIndex++,
				};
			}

			//	Crée le panneau de droite.
			new StaticText
			{
				Parent = rightPane,
				PreferredHeight = 20,
				Dock = DockStyle.Top,
				Text = "<font size=\"16\">Paramètres du compte sélectionné</font>",
				Margins = new Margins (0, 0, 0, 10),
			};

			this.userBox = new FrameBox
			{
				Parent = rightPane,
				DrawFullFrame = true,
				PreferredWidth = 300,
				Dock = DockStyle.Fill,
				Padding = new Margins (10),
				TabIndex = tabIndex++,
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
					TabIndex = tabIndex++,
				};

				ToolTip.Default.SetToolTip (this.loginNameField, "Nom court servant à identifier le compte.<br/>Exemples: \"Gérard\" ou \"Silver25\"");


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
					TabIndex = tabIndex++,
				};

				ToolTip.Default.SetToolTip (this.displayNameField, "Prénom et nom du possesseur du compte.<br/>Exemples: \"Jean-Paul van Decker\" ou \"Sophie Duval\"");


				new StaticText
				{
					Parent = this.userBox,
					Text = "L'utilisateur fait partie des groupes suivants :",
					Dock = DockStyle.Top,
					Margins = new Margins (0, 0, 0, UIBuilder.MarginUnderLabel+2),
				};

				this.checkButtonGroups.Clear ();
				foreach (var group in this.groups)
				{
					var button = new CheckButton
					{
						Parent = this.userBox,
						FormattedText = group.Name,
						Dock = DockStyle.Top,
						TabIndex = tabIndex++,
					};

					this.checkButtonGroups.Add (button);
				}


				this.newPasswordLabel = new StaticText
				{
					Parent = this.userBox,
					Text = "Pour le changer le mot de passe :",
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
					TabIndex = tabIndex++,
				};

				this.newPasswordField2 = new TextField
				{
					Parent = this.userBox,
					IsPassword = true,
					PasswordReplacementCharacter = '●',
					Dock = DockStyle.Top,
					Margins = new Margins (0, 0, 0, 2),
					TabIndex = tabIndex++,
				};

				ToolTip.Default.SetToolTip (this.newPasswordField1, "Entrez ici une première fois le mot de passe");
				ToolTip.Default.SetToolTip (this.newPasswordField2, "Entrez ici une deuxième fois le mot de passe");


				this.applyButton = new Button
				{
					Parent = this.userBox,
					Text = "Appliquer les modifications",
					Dock = DockStyle.Bottom,
					Margins = new Margins (0, 0, 10, 0),
					TabIndex = tabIndex++,
				};
			}

			//	Crée le pied de page.
			{
				this.errorMessage = new StaticText
				{
					Parent = footer,
					Visibility = false,
					ContentAlignment = Common.Drawing.ContentAlignment.MiddleCenter,
					Dock = DockStyle.Fill,
					Margins = new Margins (0, 10, 0, 0),
				};

				this.cancelButton = new Button ()
				{
					Parent = footer,
					Text = this.hasQuitButton ? "Quitter" : "Annuler",
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

			this.UpdateList ();
			this.UpdateUser ();
			this.UpdateWidgets ();
		}

		protected void SetupEvents(Window window)
		{
			this.addButton.Clicked += delegate
			{
				this.AddAction ();
			};

			this.removeButton.Clicked += delegate
			{
				this.RemoveAction ();
			};

			this.list.SelectionActivated += delegate
			{
				this.loginErrorCounter = 0;
				this.loginErrorMessage = null;
				this.isUserCreation = false;

				this.UpdateUser ();
				this.UpdateWidgets ();

				this.passField.Text = null;
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


			foreach (var button in this.checkButtonGroups)
			{
				button.ActiveStateChanged += delegate
				{
					this.dirtyContent = true;
					this.UpdateWidgets ();
				};
			}


			this.applyButton.Clicked += delegate
			{
				this.ApplyModifications ();
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


		private void UpdateList(int? sel = null)
		{
			if (sel == null)
			{
				sel = this.list.SelectedItemIndex;
			}

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
					if (string.IsNullOrEmpty (user.LoginName))
					{
						text = "<i>Nouveau compte</i>";
					}
					else
					{
						text = user.LoginName;
					}
				}
				else
				{
					text = TextFormatter.FormatText (user.LoginName, "(", user.DisplayName, ")");
				}

				this.list.Items.Add (text);
			}

			sel = System.Math.Min (sel.Value, this.list.Items.Count-1);
			this.list.SelectedItemIndex = sel.Value;

			this.initialUser = null;  // ne sert que la 1ère fois !
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

				foreach (var button in this.checkButtonGroups)
				{
					button.ActiveState = ActiveState.No;
				}
			}
			else
			{
				this.userBox.Enable = true;

				this.loginNameField.Text = user.LoginName;
				this.displayNameField.FormattedText = user.DisplayName;
				this.newPasswordField1.Text = null;
				this.newPasswordField2.Text = null;

				for (int i=0; i<this.checkButtonGroups.Count; i++)
				{
					var button = this.checkButtonGroups[i];

					if (user.UserGroups.Contains (this.groups[i]))
					{
						button.ActiveState = ActiveState.Yes;
					}
					else
					{
						button.ActiveState = ActiveState.No;
					}
				}
			}

			this.dirtyContent = false;
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

			this.removeButton.Enable = (user != null);

			this.passLabel.Enable = (user != null);
			this.passField.Enable = (user != null);
			this.loginButton.Enable  = (user != null && (!hasPassword || !string.IsNullOrEmpty (this.passField.Text)));

			this.passLabel.Visibility = hasPassword;
			this.passField.Visibility = hasPassword;

			this.newPasswordLabel.Text = this.isUserCreation ? "Mot de passe du compte :" : "Pour le changer le mot de passe :";

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

			this.applyButton.Enable = this.dirtyContent && message == null;
		}


		private void AddAction()
		{
			var newUser = this.manager.CreateNewUser ();
			newUser.AuthenticationMethod = Business.UserManagement.UserAuthenticationMethod.Password;

			this.users.Add (newUser);

			this.isUserCreation = true;

			this.UpdateList (this.users.Count-1);
			this.UpdateUser ();
			this.UpdateWidgets ();

			this.loginNameField.Focus ();
		}

		private void RemoveAction()
		{
			var user = this.SelectedUser;
			System.Diagnostics.Debug.Assert (user != null);

			user.IsArchive = true;
			this.users.RemoveAt (this.list.SelectedItemIndex);

			this.UpdateList ();
			this.UpdateWidgets ();
		}

		private void ApplyModifications()
		{
			var user = this.SelectedUser;
			System.Diagnostics.Debug.Assert (user != null);

			//	Met à jour les noms.
			user.LoginName   = this.loginNameField.Text.Trim ();
			user.DisplayName = this.displayNameField.FormattedText;

			//	Met à jour le mot de passe, si l'utilisateur l'a donné.
			if (!string.IsNullOrEmpty (this.newPasswordField1.Text))
			{
				user.SetPassword (this.newPasswordField1.Text);
			}

			//	Met à jour la liste des groupes.
			user.UserGroups.Clear ();

			for (int i=0; i<this.checkButtonGroups.Count; i++)
			{
				var button = this.checkButtonGroups[i];

				if (button.ActiveState == ActiveState.Yes)
				{
					user.UserGroups.Add (this.groups[i]);
				}
			}

			this.dirtyContent = false;
			this.isUserCreation = false;

			this.UpdateList ();
			this.UpdateWidgets ();

			//	Ce message disparaîtra lors du prochain UpdateWidgets !
			this.errorMessage.Visibility = true;
			this.errorMessage.BackColor = Color.FromName ("LightGreen");
			this.errorMessage.FormattedText = string.Format ("Le compte {0} a été modifié avec succès.", user.LoginName);
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

			if (this.dirtyContent)
			{
				if (string.IsNullOrWhiteSpace (this.loginNameField.Text))
				{
					return "Vous devez donner un nom de compte.";
				}

				if (string.IsNullOrWhiteSpace (this.displayNameField.Text))
				{
					return "Vous devez donner un nom complet d'utilisateur.";
				}

				if ((!string.IsNullOrEmpty (this.newPasswordField1.Text) || !string.IsNullOrEmpty (this.newPasswordField2.Text)) &&
					this.newPasswordField1.Text != this.newPasswordField2.Text)
				{
					return "Les deux mots de passe ne sont pas identiques.";
				}

				if (this.LoginNameCount != 0)
				{
					return string.Format ("Le nom de compte \"{0}\" est déjà utilisé.", this.loginNameField.Text.Trim ());
				}

				int activeGroupCount = this.checkButtonGroups.Where (button => button.ActiveState == ActiveState.Yes).Count ();
				if (activeGroupCount == 0)
				{
					return "Vous devez spécifier au moins un groupe.";
				}

				var user = this.SelectedUser;
				if (user != null &&
					string.IsNullOrEmpty (user.LoginPasswordHash) &&
					string.IsNullOrEmpty (this.newPasswordField1.Text) &&
					string.IsNullOrEmpty (this.newPasswordField2.Text))
				{
					return "Vous devez donner un mot de passe.";
				}
			}

			return null;
		}

		private int LoginNameCount
		{
			get
			{
				var user = this.SelectedUser;
				var name = this.loginNameField.Text.Trim ();

				return this.users.Where (x => x.LoginName != user.LoginName && x.LoginName == name).Count ();
			}
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



		private readonly CoreApplication						application;
		private readonly UserManager							manager;
		private readonly bool									hasQuitButton;
		private readonly List<SoftwareUserEntity>				users;
		private readonly List<SoftwareUserGroupEntity>			groups;
		private readonly List<CheckButton>						checkButtonGroups;

		private SoftwareUserEntity								initialUser;
		private FrameBox										toolbar;
		private GlyphButton										addButton;
		private GlyphButton										removeButton;
		private ScrollList										list;
		private StaticText										passLabel;
		private TextField										passField;
		private FrameBox										userBox;
		private TextField										loginNameField;
		private TextField										displayNameField;
		private StaticText										newPasswordLabel;
		private TextField										newPasswordField1;
		private TextField										newPasswordField2;
		private Button											applyButton;
		private bool											dirtyContent;
		private StaticText										errorMessage;
		private Button											loginButton;
		private Button											cancelButton;
		private int												loginErrorCounter;
		private FormattedText									loginErrorMessage;
		private bool											isUserCreation;
	}
}
