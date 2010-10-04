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
		public UserManagerDialog(CoreApplication application, SoftwareUserEntity user)
		{
			this.application = application;
			this.manager     = application.UserManager;

			this.initialUser = user;
			this.authenticatedUser = this.manager.AuthenticatedUser;

			this.users  = this.manager.GetAllUsers ().OrderBy (x => x.DisplayName).ToList ();
			this.groups = this.manager.GetAllUserGroups ().Where (x => x.UserPowerLevel != UserPowerLevel.System).OrderBy (x => x.UserPowerLevel).ToList ();

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
			window.ClientSize = new Size (560, 445);

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
					Text = "Nom du compte :",
					Dock = DockStyle.Top,
					Margins = new Margins (0, 0, 0, UIBuilder.MarginUnderLabel),
				};

				this.loginNameField = new TextFieldEx
				{
					Parent = this.userBox,
					Dock = DockStyle.Top,
					Margins = new Margins (0, 0, 0, 10),
					DefocusAction = DefocusAction.AutoAcceptOrRejectEdition,
					SwallowEscapeOnRejectEdition = true,
					SwallowReturnOnAcceptEdition = true,
					TabIndex = tabIndex++,
				};

				ToolTip.Default.SetToolTip (this.loginNameField, "Nom court servant à identifier le compte.<br/>Exemples: \"Gérard\", \"secrétariat\" ou \"Silver25\"");


				new StaticText
				{
					Parent = this.userBox,
					Text = "Nom complet de l'utilisateur :",
					Dock = DockStyle.Top,
					Margins = new Margins (0, 0, 0, UIBuilder.MarginUnderLabel),
				};

				this.displayNameField = new TextFieldEx
				{
					Parent = this.userBox,
					Dock = DockStyle.Top,
					Margins = new Margins (0, 0, 0, 10),
					DefocusAction = DefocusAction.AutoAcceptOrRejectEdition,
					SwallowEscapeOnRejectEdition = true,
					SwallowReturnOnAcceptEdition = true,
					TabIndex = tabIndex++,
				};

				ToolTip.Default.SetToolTip (this.displayNameField, "Prénom et nom du possesseur du compte.<br/>Exemples: \"Jean-Paul van Decker\" ou \"Sophie Duval\"");


				this.authenticationMethodCheckButton = new CheckButton
				{
					Parent = this.userBox,
					AutoToggle = false,
					Text = "Utilise l'identité Windows",
					Dock = DockStyle.Top,
					Margins = new Margins (0, 0, 0, 0),
					TabIndex = tabIndex++,
				};

				ToolTip.Default.SetToolTip (this.authenticationMethodCheckButton, "Si le nom du compte correspond à la session Windows en cours,<br/>le mot de passe ne sera pas demandé.");

				this.disableUserCheckButton = new CheckButton
				{
					Parent = this.userBox,
					AutoToggle = false,
					Text = "Utilisateur désactivé",
					Dock = DockStyle.Top,
					Margins = new Margins (0, 0, 0, 10),
					TabIndex = tabIndex++,
				};

				ToolTip.Default.SetToolTip (this.disableUserCheckButton, "Ce compte ne sera plus proposé lors de l'identification.");


				new StaticText
				{
					Parent = this.userBox,
					Text = "Dates de début et de fin de validité du compte :",
					Dock = DockStyle.Top,
					Margins = new Margins (0, 0, 0, UIBuilder.MarginUnderLabel),
				};

				{
					var box = new FrameBox
					{
						Parent = this.userBox,
						Dock = DockStyle.Top,
						Margins = new Margins (0, 0, 0, 10),
						TabIndex = tabIndex++,
					};

					this.beginDateField = new TextFieldEx
					{
						Parent = box,
						PreferredWidth = 100,
						Dock = DockStyle.Left,
						Margins = new Margins (0, 10, 0, 0),
						DefocusAction = DefocusAction.AutoAcceptOrRejectEdition,
						SwallowEscapeOnRejectEdition = true,
						SwallowReturnOnAcceptEdition = true,
						TabIndex = tabIndex++,
					};

					this.endDateField = new TextFieldEx
					{
						Parent = box,
						PreferredWidth = 100,
						Dock = DockStyle.Left,
						Margins = new Margins (0, 10, 0, 0),
						DefocusAction = DefocusAction.AutoAcceptOrRejectEdition,
						SwallowEscapeOnRejectEdition = true,
						SwallowReturnOnAcceptEdition = true,
						TabIndex = tabIndex++,
					};

					ToolTip.Default.SetToolTip (this.beginDateField, "Date de début (inclue)");
					ToolTip.Default.SetToolTip (this.endDateField,   "Date de fin (inclue)");
				}


				new StaticText
				{
					Parent = this.userBox,
					Text = "L'utilisateur fait partie des groupes suivants :",
					Dock = DockStyle.Top,
					Margins = new Margins (0, 0, 0, UIBuilder.MarginUnderLabel+2),
				};

				double scrollableHeight = 15 * System.Math.Min (this.groups.Count, 5);

				var scrollable = new Scrollable
				{
					Parent = this.userBox,
					Dock = DockStyle.Top,
					PreferredHeight = scrollableHeight,
					HorizontalScrollerMode = ScrollableScrollerMode.HideAlways,
					VerticalScrollerMode = ScrollableScrollerMode.Auto,
					PaintViewportFrame = false,
					TabIndex = tabIndex++,
				};

				scrollable.Viewport.IsAutoFitting = true;

				this.checkButtonGroups.Clear ();
				foreach (var group in this.groups)
				{
					var button = new CheckButton
					{
						Parent = scrollable.Viewport,
						AutoToggle = false,
						FormattedText = group.Name,
						Dock = DockStyle.Top,
						TabIndex = tabIndex++,
					};

					this.checkButtonGroups.Add (button);
				}


				this.newPasswordLabel = new StaticText
				{
					Parent = this.userBox,
					Text = "Pour changer le mot de passe :",
					Dock = DockStyle.Top,
					Margins = new Margins (0, 0, 10, UIBuilder.MarginUnderLabel),
				};

				this.newPasswordField1 = new TextFieldEx
				{
					Parent = this.userBox,
					IsPassword = true,
					PasswordReplacementCharacter = '●',
					Dock = DockStyle.Top,
					Margins = new Margins (0, 0, 0, -1),
					DefocusAction = DefocusAction.AutoAcceptOrRejectEdition,
					SwallowEscapeOnRejectEdition = true,
					SwallowReturnOnAcceptEdition = true,
					TabIndex = tabIndex++,
				};

				this.newPasswordField2 = new TextFieldEx
				{
					Parent = this.userBox,
					IsPassword = true,
					PasswordReplacementCharacter = '●',
					Dock = DockStyle.Top,
					DefocusAction = DefocusAction.AutoAcceptOrRejectEdition,
					SwallowEscapeOnRejectEdition = true,
					SwallowReturnOnAcceptEdition = true,
					TabIndex = tabIndex++,
				};

				ToolTip.Default.SetToolTip (this.newPasswordField1, "Entrez ici une première fois le mot de passe");
				ToolTip.Default.SetToolTip (this.newPasswordField2, "Entrez ici une deuxième fois le mot de passe");
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
					Text = "Annuler",
					ButtonStyle = Common.Widgets.ButtonStyle.DefaultCancel,
					Dock = DockStyle.Right,
					TabIndex = tabIndex++,
				};

				this.acceptButton = new Button ()
				{
					Parent = footer,
					Text = "Valider",
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

			this.list.SelectedItemChanging += delegate
			{
				this.loginNameField.DefocusAndAcceptOrReject ();
				this.displayNameField.DefocusAndAcceptOrReject ();
				this.newPasswordField1.DefocusAndAcceptOrReject ();
				this.newPasswordField2.DefocusAndAcceptOrReject ();
				this.beginDateField.DefocusAndAcceptOrReject ();
				this.endDateField.DefocusAndAcceptOrReject ();
			};

			this.list.SelectionActivated += delegate
			{
				this.UpdateUser ();
				this.UpdateWidgets ();

				this.loginNameField.SetFocusOnTabWidget ();

				//?this.loginNameField.SelectAll ();
				//?this.loginNameField.Focus ();
			};


			this.loginNameField.EditionAccepted += delegate
			{
				this.ActionLoginNameChanged ();
			};

			this.loginNameField.EditionStarted += delegate
			{
				this.editionStarted = true;
				this.UpdateWidgets ();
			};

			this.loginNameField.EditionAccepted += delegate
			{
				this.editionStarted = false;
				this.UpdateWidgets ();
			};

			this.loginNameField.EditionRejected += delegate
			{
				this.editionStarted = false;
				this.UpdateWidgets ();
			};


			this.displayNameField.EditionAccepted += delegate
			{
				this.ActionDisplayNameChanged ();
			};

			this.displayNameField.EditionStarted += delegate
			{
				this.editionStarted = true;
				this.UpdateWidgets ();
			};

			this.displayNameField.EditionAccepted += delegate
			{
				this.editionStarted = false;
				this.UpdateWidgets ();
			};

			this.displayNameField.EditionRejected += delegate
			{
				this.editionStarted = false;
				this.UpdateWidgets ();
			};


			this.newPasswordField1.EditionAccepted += delegate
			{
				this.ActionPasswordChanged ();
			};

			this.newPasswordField1.EditionStarted += delegate
			{
				this.editionStarted = true;
				this.UpdateWidgets ();
			};

			this.newPasswordField1.EditionAccepted += delegate
			{
				this.editionStarted = false;
				this.UpdateWidgets ();
			};

			this.newPasswordField1.EditionRejected += delegate
			{
				this.editionStarted = false;
				this.UpdateWidgets ();
			};


			this.newPasswordField2.EditionAccepted += delegate
			{
				this.ActionPasswordChanged ();
			};

			this.newPasswordField2.EditionStarted += delegate
			{
				this.editionStarted = true;
				this.UpdateWidgets ();
			};

			this.newPasswordField2.EditionAccepted += delegate
			{
				this.editionStarted = false;
				this.UpdateWidgets ();
			};

			this.newPasswordField2.EditionRejected += delegate
			{
				this.editionStarted = false;
				this.UpdateWidgets ();
			};


			this.beginDateField.EditionAccepted += delegate
			{
				this.ActionBeginDateChanged ();
			};

			this.beginDateField.EditionStarted += delegate
			{
				this.editionStarted = true;
				this.UpdateWidgets ();
			};

			this.beginDateField.EditionAccepted += delegate
			{
				this.editionStarted = false;
				this.UpdateWidgets ();
			};

			this.beginDateField.EditionRejected += delegate
			{
				this.editionStarted = false;
				this.UpdateWidgets ();
			};


			this.endDateField.EditionAccepted += delegate
			{
				this.ActionEndDateChanged ();
			};

			this.endDateField.EditionStarted += delegate
			{
				this.editionStarted = true;
				this.UpdateWidgets ();
			};

			this.endDateField.EditionAccepted += delegate
			{
				this.editionStarted = false;
				this.UpdateWidgets ();
			};

			this.endDateField.EditionRejected += delegate
			{
				this.editionStarted = false;
				this.UpdateWidgets ();
			};


			this.authenticationMethodCheckButton.Clicked += delegate
			{
				UserManagerDialog.ToggleCheckButton (this.authenticationMethodCheckButton);
				this.ActionAuthenticationMethodChanged ();
			};

			this.disableUserCheckButton.Clicked += delegate
			{
				UserManagerDialog.ToggleCheckButton (this.disableUserCheckButton);
				this.ActionEnableUserChanged ();
			};


			foreach (var button in this.checkButtonGroups)
			{
				button.Clicked += new EventHandler<MessageEventArgs> (this.HandleButtonClicked);
			}


			this.acceptButton.Clicked += delegate
			{
				this.CloseAction (cancel: false);
			};

			this.cancelButton.Clicked += delegate
			{
				this.CloseAction (cancel: true);
			};
		}

		void HandleButtonClicked(object sender, MessageEventArgs e)
		{
			var button = sender as CheckButton;
			this.ActionGroupChanged (button);
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
				this.manager.UpdateAuthenticate ();
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
				if (this.initialUser != null && user.LoginName == this.initialUser.LoginName)
				{
					sel = this.list.Items.Count;
				}

				FormattedText description = user.ShortDescription;

				if (this.authenticatedUser != null && this.authenticatedUser.Code == user.Code)
				{
					description = TextFormatter.FormatText ("<b>", description, "</b>");
				}

				this.list.Items.Add (description);
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
				this.beginDateField.Text = null;
				this.endDateField.Text = null;

				this.authenticationMethodCheckButton.ActiveState = ActiveState.No;
				this.disableUserCheckButton.ActiveState = ActiveState.No;

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
				this.beginDateField.Text = Misc.GetDateTimeShortDescription (user.BeginDate);
				this.endDateField.Text = Misc.GetDateTimeShortDescription (user.EndDate);

				this.authenticationMethodCheckButton.ActiveState = user.AuthenticationMethod == Business.UserManagement.UserAuthenticationMethod.System ? ActiveState.Yes : ActiveState.No;
				this.disableUserCheckButton.ActiveState = user.Disabled ? ActiveState.Yes : ActiveState.No;

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
		}

		private void UpdateWidgets()
		{
			var user = this.SelectedUser;
			bool hasPassword = (user != null && !string.IsNullOrEmpty (user.LoginPasswordHash));

			this.removeButton.Enable = (user != null && (this.authenticatedUser == null || this.authenticatedUser.Code != user.Code));
			this.disableUserCheckButton.Enable = (this.authenticatedUser == null || user == null || this.authenticatedUser.Code != user.Code);
			this.newPasswordLabel.Text = hasPassword ? "Pour changer le mot de passe :" : "Mot de passe du compte :";

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

			this.acceptButton.Enable = message == null && this.editionStarted == false;
		}


		private void AddAction()
		{
			var newUser = this.manager.CreateNewUser ();
			newUser.AuthenticationMethod = Business.UserManagement.UserAuthenticationMethod.Password;

			this.users.Add (newUser);

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
			this.UpdateUser ();
			this.UpdateWidgets ();
		}

		private void ActionLoginNameChanged()
		{
			var user = this.SelectedUser;
			System.Diagnostics.Debug.Assert (user != null);

			user.LoginName = this.loginNameField.Text.Trim ();

			this.UpdateList ();
			this.UpdateWidgets ();
		}

		private void ActionDisplayNameChanged()
		{
			var user = this.SelectedUser;
			System.Diagnostics.Debug.Assert (user != null);

			user.DisplayName = this.displayNameField.FormattedText;

			this.UpdateList ();
			this.UpdateWidgets ();
		}

		private void ActionPasswordChanged()
		{
			var user = this.SelectedUser;
			System.Diagnostics.Debug.Assert (user != null);

			if (!string.IsNullOrEmpty (this.newPasswordField1.Text) || !string.IsNullOrEmpty (this.newPasswordField2.Text))
			{
				FormattedText pwm = this.GetPasswordMessage ();

				if (pwm.IsNullOrEmpty)  // ok ?
				{
					user.SetPassword (this.newPasswordField1.Text);

					this.UpdateList ();
					this.UpdateWidgets ();

					//	Affiche un message sur fond vert, qui sera effacé au prochain UpdateWidgets.
					this.errorMessage.Visibility = true;
					this.errorMessage.BackColor = Color.FromName ("LightGreen");
					this.errorMessage.FormattedText = string.Format ("Le mot de passe du compte \"{0}\" est défini.", user.LoginName);

					return;
				}
			}

			this.UpdateList ();
			this.UpdateWidgets ();
		}

		private void ActionBeginDateChanged()
		{
			var user = this.SelectedUser;
			System.Diagnostics.Debug.Assert (user != null);

			Date? date = Misc.GetDateFromString (this.beginDateField.Text);

			if (date == null)
			{
				user.BeginDate = null;
			}
			else
			{
				user.BeginDate = new System.DateTime (date.Value.Year, date.Value.Month, date.Value.Day, 0, 0, 0);
			}

			this.beginDateField.Text = Misc.GetDateTimeShortDescription (user.BeginDate);

			this.UpdateList ();
			this.UpdateWidgets ();
		}

		private void ActionEndDateChanged()
		{
			var user = this.SelectedUser;
			System.Diagnostics.Debug.Assert (user != null);

			Date? date = Misc.GetDateFromString (this.endDateField.Text);

			if (date == null)
			{
				user.EndDate = null;
			}
			else
			{
				user.EndDate = new System.DateTime (date.Value.Year, date.Value.Month, date.Value.Day, 23, 59, 59);
			}

			this.endDateField.Text = Misc.GetDateTimeShortDescription (user.EndDate);

			this.UpdateList ();
			this.UpdateWidgets ();
		}

		private void ActionAuthenticationMethodChanged()
		{
			var user = this.SelectedUser;
			System.Diagnostics.Debug.Assert (user != null);

			user.AuthenticationMethod = this.authenticationMethodCheckButton.ActiveState == ActiveState.Yes ? Business.UserManagement.UserAuthenticationMethod.System : Business.UserManagement.UserAuthenticationMethod.Password;

			this.UpdateList ();
			this.UpdateWidgets ();
		}

		private void ActionEnableUserChanged()
		{
			var user = this.SelectedUser;
			System.Diagnostics.Debug.Assert (user != null);

			user.Disabled = this.disableUserCheckButton.ActiveState == ActiveState.Yes;

			this.UpdateList ();
			this.UpdateWidgets ();
		}

		private void ActionGroupChanged(CheckButton button)
		{
			UserManagerDialog.ToggleCheckButton (button);

			//	Met à jour la liste des groupes.
			var user = this.SelectedUser;
			System.Diagnostics.Debug.Assert (user != null);

			user.UserGroups.Clear ();

			for (int i=0; i<this.checkButtonGroups.Count; i++)
			{
				var b = this.checkButtonGroups[i];

				if (b.ActiveState == ActiveState.Yes)
				{
					user.UserGroups.Add (this.groups[i]);
				}
			}

			this.UpdateList ();
			this.UpdateWidgets ();
		}


		private static void ToggleCheckButton(CheckButton button)
		{
			button.ActiveState = (button.ActiveState == ActiveState.Yes) ? ActiveState.No : ActiveState.Yes;
		}


		private FormattedText GetErrorMessage()
		{
			var user = this.SelectedUser;

			if (string.IsNullOrWhiteSpace (this.loginNameField.Text))
			{
				return "Vous devez donner un nom de compte.";
			}

			if (this.NamesCount != 0)
			{
				return string.Format ("Les noms \"{0}\" et \"{1}\" sont déjà utilisés.", this.loginNameField.Text.Trim (), this.displayNameField.Text);
			}

			if (string.IsNullOrWhiteSpace (this.displayNameField.Text))
			{
				return "Vous devez donner un nom complet d'utilisateur.";
			}

			FormattedText pwm = this.GetPasswordMessage ();
			if (pwm != null)
			{
				return pwm;
			}

			int activeGroupCount = this.checkButtonGroups.Where (button => button.ActiveState == ActiveState.Yes).Count ();
			if (activeGroupCount == 0)
			{
				return "Vous devez spécifier au moins un groupe.";
			}

			if (user != null &&
				string.IsNullOrEmpty (user.LoginPasswordHash) &&
				string.IsNullOrEmpty (this.newPasswordField1.Text) &&
				string.IsNullOrEmpty (this.newPasswordField2.Text))
			{
				return "Vous devez donner un mot de passe.";
			}

			if (this.AdminEternallyCount == 0)
			{
				return "Il doit exister au moins un compte administrateur valable éternellement.";
			}

			for (int i = 0; i < this.users.Count; i++)
			{
				if (this.users[i].IsEntityEmpty)
				{
					return string.Format ("Le compte en position {0} n'est pas défini.", (i+1).ToString ());
				}

				if (this.users[i].IsEntityValid == false)
				{
					return string.Format ("Le compte en position {0} n'est pas complètement défini.", (i+1).ToString ());
				}
			}

			return null;
		}

		private FormattedText GetPasswordMessage()
		{
			if (!string.IsNullOrEmpty (this.newPasswordField1.Text))
			{
				string err = UserManagerDialog.CheckPassword (this.newPasswordField1.Text);
				if (err != null)
				{
					return err;
				}
			}

			if (!string.IsNullOrEmpty (this.newPasswordField1.Text) && string.IsNullOrEmpty (this.newPasswordField2.Text))
			{
				return "Vous devez entrer une deuxième fois le mot de passe.";
			}

			if ((!string.IsNullOrEmpty (this.newPasswordField1.Text) || !string.IsNullOrEmpty (this.newPasswordField2.Text)) &&
				this.newPasswordField1.Text != this.newPasswordField2.Text)
			{
				return "Les deux mots de passe ne sont pas identiques.";
			}

			return null;
		}

		private int AdminEternallyCount
		{
			get
			{
				int count = 0;

				foreach (var user in this.users)
				{
					if ((user.BeginDate == null || user.BeginDate.Value <= System.DateTime.Now) && 
						user.EndDate == null &&
						user.Disabled == false)
					{
						foreach (var group in user.UserGroups)
						{
							if (group.UserPowerLevel == UserPowerLevel.Administrator)
							{
								count++;
							}
						}
					}
				}

				return count;
			}
		}

		private int NamesCount
		{
			get
			{
				var sel = this.list.SelectedItemIndex;
				var loginName = this.loginNameField.Text.Trim ();
				var displayName = this.displayNameField.FormattedText;

				int count = 0;

				for (int i=0; i<this.users.Count; i++)
				{
					var user = this.users[i];

					if (i != sel && user.LoginName == loginName && user.DisplayName == displayName)
					{
						count++;
					}
				}

				return count;
			}
		}

		private static string CheckPassword(string pw)
		{
			if (string.IsNullOrWhiteSpace (pw))
			{
				return "Vous devez donner un mot de passe.";
			}

			if (pw.Length < 5)
			{
				return "Le mot de passe est trop court.";
			}

			// TODO: On pourrait faire mieux !
			for (int i = 1; i < pw.Length; i++)
			{
				if (pw[0] != pw[i])
				{
					return null;  // ok
				}
			}

			return "Le mot de passe est trop simple.";
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



		private readonly CoreApplication					application;
		private readonly UserManager						manager;
		private readonly List<SoftwareUserEntity>			users;
		private readonly List<SoftwareUserGroupEntity>		groups;
		private readonly List<CheckButton>					checkButtonGroups;

		private SoftwareUserEntity							initialUser;
		private SoftwareUserEntity							authenticatedUser;
		private FrameBox									toolbar;
		private GlyphButton									addButton;
		private GlyphButton									removeButton;
		private ScrollList									list;
		private FrameBox									userBox;
		private TextFieldEx									loginNameField;
		private TextFieldEx									displayNameField;
		private CheckButton									disableUserCheckButton;
		private CheckButton									authenticationMethodCheckButton;
		private TextFieldEx									beginDateField;
		private TextFieldEx									endDateField;
		private StaticText									newPasswordLabel;
		private TextFieldEx									newPasswordField1;
		private TextFieldEx									newPasswordField2;
		private StaticText									errorMessage;
		private Button										acceptButton;
		private Button										cancelButton;
		private bool										editionStarted;
	}
}
