//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Controllers
{
	/// <summary>
	/// Ce contrôleur gère les utilisateurs de la comptabilité.
	/// </summary>
	public class UtilisateursController : AbstractController
	{
		public UtilisateursController(Application app, BusinessContext businessContext, MainWindowController mainWindowController)
			: base (app, businessContext, mainWindowController)
		{
			this.dataAccessor = new UtilisateursDataAccessor (this);

			this.memoryList = this.mainWindowController.GetMemoryList ("Présentation.Utilisateurs.Memory");

			this.checkButtons = new List<CheckButton> ();

			this.initialUser = this.mainWindowController.CurrentUser;
			//?this.authenticatedUser = this.manager.AuthenticatedUser;
		}


		public override void Dispose()
		{
			this.RemoveErrors ();
			base.Dispose ();
		}


		protected override void UpdateTitle()
		{
			this.SetTitle ("Utilisateurs");
		}


		public override bool AcceptPériodeChanged
		{
			get
			{
				return false;
			}
		}


		public override bool HasArray
		{
			get
			{
				return false;
			}
		}

		public override bool HasShowSearchPanel
		{
			get
			{
				return true;
			}
		}

		public override bool HasShowFilterPanel
		{
			get
			{
				return false;
			}
		}

		public override bool HasShowOptionsPanel
		{
			get
			{
				return false;
			}
		}

		public override bool HasShowInfoPanel
		{
			get
			{
				return true;
			}
		}


#if false
		protected override int ArrayLineHeight
		{
			get
			{
				return 20;
			}
		}

		protected override FormattedText GetArrayText(int row, ColumnType columnType)
		{
			//	Retourne le texte contenu dans une cellule.
			return this.dataAccessor.GetText (row, columnType);
		}


		protected override void CreateFooter(FrameBox parent)
		{
			this.footerController = new UtilisateursFooterController (this);
			this.footerController.CreateUI (parent, this.UpdateArrayContent);
			this.footerController.ShowInfoPanel = this.mainWindowController.ShowInfoPanel;
		}


		protected override IEnumerable<ColumnMapper> InitialColumnMappers
		{
			get
			{
				yield return new ColumnMapper (ColumnType.Utilisateur, 1.00, ContentAlignment.MiddleLeft, "Utilisateur",          "Nom de l'utilisateur utilisé pour l'identification");
				yield return new ColumnMapper (ColumnType.NomComplet,  1.00, ContentAlignment.MiddleLeft, "Nom complet",          "Nom complet (facultatif)");
				yield return new ColumnMapper (ColumnType.DateDébut,   0.50, ContentAlignment.MiddleLeft, "Date début",           "Date de début de validité (facultatif)");
				yield return new ColumnMapper (ColumnType.DateFin,     0.50, ContentAlignment.MiddleLeft, "Date Fin",             "Date de fin de validité (facultatif)");
				yield return new ColumnMapper (ColumnType.MotDePasse,  0.60, ContentAlignment.MiddleLeft, "Mot de passe",         "Mot de passe de l'utilisateur (facultatif)");
				yield return new ColumnMapper (ColumnType.Pièce,       0.80, ContentAlignment.MiddleLeft, "Générateur de pièces", "Générateur pour les numéros de pièces (facultatif)");
				yield return new ColumnMapper (ColumnType.Résumé,      1.00, ContentAlignment.MiddleLeft, "Résumé");
			}
		}
#endif


		protected override void CreateSpecificUI(FrameBox parent)
		{
			int tabIndex = 1;

			var mainFrame = new FrameBox
			{
				Parent        = parent,
				DrawFullFrame = true,
				Dock          = DockStyle.Fill,
				Padding       = new Margins (10),
				TabIndex      = tabIndex++,
			};

			var topFrame = new FrameBox
			{
				Parent         = mainFrame,
				Dock           = DockStyle.Fill,
				TabIndex       = tabIndex++,
			};

			var leftPane = new FrameBox
			{
				Parent         = topFrame,
				Dock           = DockStyle.Fill,
				TabIndex       = tabIndex++,
			};

			var rightPane = new FrameBox
			{
				Parent         = topFrame,
				PreferredWidth = 340,
				Dock           = DockStyle.Right,
				TabIndex       = tabIndex++,
			};

			var middlePane = new FrameBox
			{
				Parent         = topFrame,
				PreferredWidth = 210,
				Dock           = DockStyle.Right,
				Margins        = new Margins (10, -1, 0, 0),
				TabIndex       = tabIndex++,
			};

			var footer = new FrameBox
			{
				Parent          = parent,
				PreferredHeight = 20,
				Dock            = DockStyle.Bottom,
				TabIndex        = tabIndex++,
			};

			//	Crée le panneau de gauche.
			{
				var columnTitle = new StaticText (leftPane);
				columnTitle.SetColumnTitle ("Liste des utilisateurs");
			}

			{
				//	Crée la toolbar.
				double buttonSize = 19;

				this.toolbar = UIBuilder.CreateMiniToolbar (leftPane, buttonSize);
				this.toolbar.Margins = new Margins (0, 0, 0, -1);
				this.toolbar.TabIndex = tabIndex++;

				this.addButton = new GlyphButton
				{
					Parent        = this.toolbar,
					PreferredSize = new Size (buttonSize*2+1, buttonSize),
					GlyphShape    = GlyphShape.Plus,
					Margins       = new Margins (0, 0, 0, 0),
					Dock          = DockStyle.Left,
				};

				this.removeButton = new GlyphButton
				{
					Parent        = this.toolbar,
					PreferredSize = new Size (buttonSize, buttonSize),
					GlyphShape    = GlyphShape.Minus,
					Margins       = new Margins (1, 0, 0, 0),
					Dock          = DockStyle.Left,
				};

				ToolTip.Default.SetToolTip (this.addButton,    "Crée un nouvel utilisateur");
				ToolTip.Default.SetToolTip (this.removeButton, "Supprime l'utilisateur sélectionné");
			}

			{
				//	Crée la liste.
				var tile = new FrameBox
				{
					Parent   = leftPane,
					Dock     = DockStyle.Fill,
					TabIndex = tabIndex++,
				};

				this.table = new CellTable
				{
					Parent    = tile,
					DefHeight = 20,
					StyleH    = CellArrayStyles.Separator,
					StyleV    = CellArrayStyles.ScrollNorm | CellArrayStyles.Separator | CellArrayStyles.SelectLine,
					Dock      = DockStyle.Fill,
					TabIndex  = tabIndex++,
				};

				this.table.SetArraySize (2, 0);
				this.table.SetWidthColumn (0, 250);
				this.table.SetWidthColumn (1, 900);
			}

			//	Crée le panneau du milieu.
			{
				var columnTitle = new StaticText (middlePane);
				columnTitle.SetColumnTitle ("Paramètres de l'utilisateur");
			}

			this.userBox = new FrameBox
			{
				Parent         = middlePane,
				DrawFullFrame  = true,
				PreferredWidth = 300,
				Dock           = DockStyle.Fill,
				Padding        = new Margins (10),
				TabIndex       = tabIndex++,
			};

			{
				new StaticText
				{
					Parent  = this.userBox,
					Text    = "Identité de l'utilisateur :",
					Dock    = DockStyle.Top,
					Margins = new Margins (0, 0, 0, 2),
				};

				this.utilisateurField = new TextFieldEx
				{
					Parent                       = this.userBox,
					Dock                         = DockStyle.Top,
					Margins                      = new Margins (0, 0, 0, 10),
					DefocusAction                = DefocusAction.AutoAcceptOrRejectEdition,
					SwallowEscapeOnRejectEdition = true,
					SwallowReturnOnAcceptEdition = true,
					TabIndex                     = tabIndex++,
				};

				ToolTip.Default.SetToolTip (this.utilisateurField, "Nom court servant à identifier l'utilisateur.<br/>Exemples: \"Gérard\", \"secrétariat\" ou \"Silver25\"");


				new StaticText
				{
					Parent  = this.userBox,
					Text    = "Nom complet de l'utilisateur :",
					Dock    = DockStyle.Top,
					Margins = new Margins (0, 0, 0, 2),
				};

				this.nomCompletField = new TextFieldEx
				{
					Parent                       = this.userBox,
					Dock                         = DockStyle.Top,
					Margins                      = new Margins (0, 0, 0, 10),
					DefocusAction                = DefocusAction.AutoAcceptOrRejectEdition,
					SwallowEscapeOnRejectEdition = true,
					SwallowReturnOnAcceptEdition = true,
					TabIndex                     = tabIndex++,
				};

				ToolTip.Default.SetToolTip (this.nomCompletField, "Prénom et nom de l'utilisateur.<br/>Exemples: \"Jean-Paul van Decker\" ou \"Sophie Duval\"");


				this.identitéWindowsCheckButton = new CheckButton
				{
					Parent     = this.userBox,
					AutoToggle = false,
					Text       = "Utilise l'identité Windows",
					Dock       = DockStyle.Top,
					Margins    = new Margins (0, 0, 0, 0),
					TabIndex   = tabIndex++,
				};

				ToolTip.Default.SetToolTip (this.identitéWindowsCheckButton, "Si le nom de l'utilisateur correspond à la session Windows en cours,<br/>le mot de passe ne sera pas demandé.");

				this.désactivéCheckButton = new CheckButton
				{
					Parent     = this.userBox,
					AutoToggle = false,
					Text       = "Utilisateur désactivé",
					Dock       = DockStyle.Top,
					Margins    = new Margins (0, 0, 0, 10),
					TabIndex   = tabIndex++,
				};

				ToolTip.Default.SetToolTip (this.désactivéCheckButton, "Cet utilisateur ne sera plus proposé lors de l'identification.");


				new StaticText
				{
					Parent  = this.userBox,
					Text    = "Dates de début et de fin de validité :",
					Dock    = DockStyle.Top,
					Margins = new Margins (0, 0, 0, 2),
				};

				{
					var box = new FrameBox
					{
						Parent   = this.userBox,
						Dock     = DockStyle.Top,
						Margins  = new Margins (0, 0, 0, 10),
						TabIndex = tabIndex++,
					};

					this.dateDébutField = new TextFieldEx
					{
						Parent                       = box,
						PreferredWidth               = 90,
						Dock                         = DockStyle.Left,
						Margins                      = new Margins (0, 10, 0, 0),
						DefocusAction                = DefocusAction.AutoAcceptOrRejectEdition,
						SwallowEscapeOnRejectEdition = true,
						SwallowReturnOnAcceptEdition = true,
						TabIndex                     = tabIndex++,
					};

					this.dateFinField = new TextFieldEx
					{
						Parent                       = box,
						PreferredWidth               = 90,
						Dock                         = DockStyle.Left,
						DefocusAction                = DefocusAction.AutoAcceptOrRejectEdition,
						SwallowEscapeOnRejectEdition = true,
						SwallowReturnOnAcceptEdition = true,
						TabIndex                     = tabIndex++,
					};

					ToolTip.Default.SetToolTip (this.dateDébutField, "Date de début (inclue)");
					ToolTip.Default.SetToolTip (this.dateFinField,   "Date de fin (inclue)");
				}


				this.newPasswordLabel = new StaticText
				{
					Parent  = this.userBox,
					Text    = "Pour changer le mot de passe :",
					Dock    = DockStyle.Top,
					Margins = new Margins (0, 0, 10, 2),
				};

				this.newPasswordField1 = new TextFieldEx
				{
					Parent                       = this.userBox,
					IsPassword                   = true,
					PasswordReplacementCharacter = '●',
					Dock                         = DockStyle.Top,
					Margins                      = new Margins (0, 0, 0, -1),
					DefocusAction                = DefocusAction.AutoAcceptOrRejectEdition,
					SwallowEscapeOnRejectEdition = true,
					SwallowReturnOnAcceptEdition = true,
					TabIndex                     = tabIndex++,
				};

				this.newPasswordField2 = new TextFieldEx
				{
					Parent                       = this.userBox,
					IsPassword                   = true,
					PasswordReplacementCharacter = '●',
					Dock                         = DockStyle.Top,
					DefocusAction                = DefocusAction.AutoAcceptOrRejectEdition,
					SwallowEscapeOnRejectEdition = true,
					SwallowReturnOnAcceptEdition = true,
					TabIndex                     = tabIndex++,
				};

				ToolTip.Default.SetToolTip (this.newPasswordField1, "Entrez ici une première fois le mot de passe");
				ToolTip.Default.SetToolTip (this.newPasswordField2, "Entrez ici une deuxième fois le mot de passe");
			}

			//	Crée le panneau de gauche.
			{
				var columnTitle = new StaticText (rightPane);
				columnTitle.SetColumnTitle ("Présentations accessibles");
			}

			this.présentationsBox = new FrameBox
			{
				Parent         = rightPane,
				DrawFullFrame  = true,
				PreferredWidth = 300,
				Dock           = DockStyle.Fill,
				Padding        = new Margins (10),
				TabIndex       = tabIndex++,
			};

			this.CreatePrésentationButtonsUI (this.présentationsBox);

			//	Crée le pied de page.
			{
				this.errorMessage = new StaticText
				{
					Parent           = footer,
					Visibility       = false,
					ContentAlignment = Common.Drawing.ContentAlignment.MiddleCenter,
					Dock             = DockStyle.Fill,
				};
			}

			this.UpdateTable ();
			this.UpdateUserWidgets ();
			this.UpdateWidgets ();

			this.CreateConnexions ();
		}

		protected void CreateConnexions()
		{
			this.addButton.Clicked += delegate
			{
				this.AddAction ();
			};

			this.removeButton.Clicked += delegate
			{
				this.RemoveAction ();
			};

			this.table.SelectionChanged += delegate
			{
				this.UpdateUserWidgets ();
				this.UpdateWidgets ();

				this.utilisateurField.SetFocusOnTabWidget ();

				//?this.loginNameField.SelectAll ();
				//?this.loginNameField.Focus ();
			};


			this.utilisateurField.EditionAccepted += delegate
			{
				this.ActionUtilisateurChanged ();
			};

			this.nomCompletField.EditionAccepted += delegate
			{
				this.ActionNomCompletChanged ();
			};

			this.newPasswordField1.EditionAccepted += delegate
			{
				this.ActionPasswordChanged ();
			};

			this.newPasswordField2.EditionAccepted += delegate
			{
				this.ActionPasswordChanged ();
			};

			this.dateDébutField.EditionAccepted += delegate
			{
				this.ActionDateDébutChanged ();
			};

			this.dateFinField.EditionAccepted += delegate
			{
				this.ActionDateFinChanged ();
			};


			this.identitéWindowsCheckButton.Clicked += delegate
			{
				UtilisateursController.ToggleCheckButton (this.identitéWindowsCheckButton);
				this.ActionIdentitéWindowsChanged ();
			};

			this.désactivéCheckButton.Clicked += delegate
			{
				UtilisateursController.ToggleCheckButton (this.désactivéCheckButton);
				this.ActionDésactivéChanged ();
			};
		}


		private void CreatePrésentationButtonsUI(Widget parent)
		{
			this.checkButtons.Clear ();

			parent.ContainerLayoutMode = ContainerLayoutMode.VerticalFlow;

			var topFrame = new FrameBox
			{
				Parent  = parent,
				Dock    = DockStyle.Fill,
			};

			var columns = new List<FrameBox> ();

			for (int i = 0; i < 2; i++)
			{
				var column = new FrameBox
				{
					Parent         = topFrame,
					PreferredWidth = 170,
					Dock           = DockStyle.Left,
				};

				columns.Add (column);
			}

			int rank = 0;
			foreach (var cmd in Converters.PrésentationCommands)
			{
				if (cmd == Res.Commands.Présentation.Login)  // cette présentation est toujours accessible !
				{
					continue;
				}

				int column = rank++/16;  // 16 boutons par colonne
				if (column >= columns.Count)
				{
					break;
				}

				this.CreatePrésentationButton (columns[column], cmd);
			}

			{
				var footer = new FrameBox
				{
					Parent          = parent,
					PreferredHeight = 20,
					Dock            = DockStyle.Bottom,
					Margins         = new Margins (0, 0, 5, 0),
				};

				this.zeroPrésentationButton = new Button
				{
					Parent          = footer,
					Text            = "Aucune",
					PreferredWidth  = 80,
					PreferredHeight = 20,
					Dock            = DockStyle.Left,
					Margins         = new Margins (0, 10, 0, 0),
				};

				this.allPrésentationButton = new Button
				{
					Parent          = footer,
					Text            = "Toutes",
					PreferredWidth  = 80,
					PreferredHeight = 20,
					Dock            = DockStyle.Left,
					Margins         = new Margins (0, 10, 0, 0),
				};

				ToolTip.Default.SetToolTip (this.zeroPrésentationButton, "Interdit l'accès à toutes les présentations");
				ToolTip.Default.SetToolTip (this.allPrésentationButton,  "Autorise l'accès à toutes les présentations");
			}

			this.zeroPrésentationButton.Clicked += delegate
			{
				foreach (var button in this.checkButtons)
				{
					button.ActiveState = ActiveState.No;
				}
			};

			this.allPrésentationButton.Clicked += delegate
			{
				foreach (var button in this.checkButtons)
				{
					button.ActiveState = ActiveState.Yes;
				}
			};
		}

		private void CreatePrésentationButton(Widget parent, Command cmd)
		{
			var icon = UIBuilder.GetTextIconUri ("Présentation." + Converters.PrésentationCommandToString (cmd), iconSize: 20);

			var button = new CheckButton
			{
				Parent          = parent,
				FormattedText   = icon + " " + Converters.GetPrésentationCommandDescription (cmd),
				Name            = Converters.PrésentationCommandToString (cmd),
				PreferredHeight = 20,
				Dock            = DockStyle.Top,
			};

			this.checkButtons.Add (button);

			button.ActiveStateChanged += delegate
			{
				if (this.ignoreChanges.IsZero)
				{
					var user = this.SelectedUser;

					string s = user.Présentations;
					Converters.SetPrésentationCommand (ref s, cmd, button.ActiveState == ActiveState.Yes);
					user.Présentations = s;

					this.UpdateTable ();
					this.UpdateWidgets ();
					this.mainWindowController.SetDirty ();
				}
			};
		}

		private void UpdatePrésentationButtons()
		{
			var user = this.SelectedUser;

			using (this.ignoreChanges.Enter ())
			{
				bool admin = (user == null || user.Admin);
				bool zero  = true;
				bool all   = true;

				foreach (var button in this.checkButtons)
				{
					var cmd = Converters.StringToPrésentationCommand (button.Name);

					string s = (user == null) ? null : user.Présentations;
					var state = Converters.ContainsPrésentationCommand (s, cmd);

					button.ActiveState = state || admin ? ActiveState.Yes : ActiveState.No;
					button.Enable = !admin;

					if (button.ActiveState == ActiveState.Yes)
					{
						zero = false;
					}
					else
					{
						all = false;
					}
				}

				this.zeroPrésentationButton.Enable = !admin && !zero;
				this.allPrésentationButton.Enable = !admin && !all;
			}
		}


		private void UpdateTable(int? sel = null)
		{
			//	Met à jour le contenu de la table.
			int rows = this.comptaEntity.Utilisateurs.Count;
			this.table.SetArraySize (2, rows);

			if (sel == null)
			{
				sel = this.table.SelectedRow;
			}

			for (int row=0; row<rows; row++)
			{
				var user = this.comptaEntity.Utilisateurs[row];

				if (this.initialUser != null && user.Utilisateur == this.initialUser.Utilisateur)
				{
					sel = row;
				}

				this.TableFillRow (row);
				this.TableUpdateRow (row);
			}

			sel = System.Math.Min (sel.Value, rows-1);
			if (sel != -1)
			{
				this.table.SelectRow (sel.Value, true);
			}

			this.initialUser = null;  // ne sert que la 1ère fois !
		}

		private void TableFillRow(int row)
		{
			//	Peuple une ligne de la table, si nécessaire.
			if (this.table[0, row].IsEmpty)
			{
				var text = new StaticText
				{
					ContentAlignment = ContentAlignment.MiddleLeft,
					TextBreakMode    = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine,
					Dock             = DockStyle.Fill,
					Margins          = new Margins (4, 4, 0, 0),
				};

				this.table[0, row].Insert (text);
			}

			if (this.table[1, row].IsEmpty)
			{
				var text = new StaticText
				{
					ContentAlignment = ContentAlignment.MiddleLeft,
					TextBreakMode    = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine,
					Dock             = DockStyle.Fill,
					Margins          = new Margins (4, 4, 0, 0),
				};

				this.table[1, row].Insert (text);
			}
		}

		private void TableUpdateRow(int row)
		{
			//	Met à jour le contenu d'une ligne de la table.
			var user = this.comptaEntity.Utilisateurs[row];

			//	Colonne 1.
			{
				FormattedText description = user.ShortDescription;

				if (this.authenticatedUser == user)
				{
					description = Core.TextFormatter.FormatText (description).ApplyBold ();
				}

				var text = this.table[0, row].Children[0] as StaticText;
				text.FormattedText = description;
			}

			//	Colonne 2.
			{
				var text = this.table[1, row].Children[0] as StaticText;
				text.FormattedText = user.GetAccessSummary ();
			}

			this.table.SelectRow (row, false);
		}


		private void UpdateUserWidgets()
		{
			var user = this.SelectedUser;

			if (user == null)
			{
				this.userBox.Enable = false;

				this.utilisateurField.Text = null;
				this.nomCompletField.Text = null;
				this.newPasswordField1.Text = null;
				this.newPasswordField2.Text = null;
				this.dateDébutField.Text = null;
				this.dateFinField.Text = null;

				this.identitéWindowsCheckButton.ActiveState = ActiveState.No;
				this.désactivéCheckButton.ActiveState = ActiveState.No;
			}
			else
			{
				this.userBox.Enable = true;

				this.utilisateurField.FormattedText = user.Utilisateur;
				this.nomCompletField.FormattedText = user.NomComplet;
				this.newPasswordField1.Text = null;
				this.newPasswordField2.Text = null;
				this.dateDébutField.Text = Converters.DateToString (user.DateDébut);
				this.dateFinField.Text = Converters.DateToString (user.DateFin);

				this.identitéWindowsCheckButton.ActiveState = user.IdentitéWindows ? ActiveState.Yes : ActiveState.No;
				this.désactivéCheckButton.ActiveState = user.Désactivé ? ActiveState.Yes : ActiveState.No;
			}

			this.UpdatePrésentationButtons ();
		}

		private void UpdateWidgets()
		{
			var user = this.SelectedUser;
			bool hasPassword = (user != null && !string.IsNullOrEmpty (user.MotDePasse));
			bool admin = (user == null || user.Admin || this.authenticatedUser == user);

			this.removeButton.Enable = !admin;
			this.désactivéCheckButton.Enable = (this.authenticatedUser == null || user == null || this.authenticatedUser != user);
			this.newPasswordLabel.Text = hasPassword ? "Pour changer le mot de passe :" : "Mot de passe de l'utilisateur :";

			this.identitéWindowsCheckButton.Enable = !admin;
			this.désactivéCheckButton.Enable = !admin;
			this.dateDébutField.Enable = !admin;
			this.dateFinField.Enable = !admin;

			var message = this.GetErrorMessage ();
			if (message == null)
			{
				this.errorMessage.Visibility = false;
			}
			else
			{
				this.errorMessage.Visibility = true;
				this.errorMessage.BackColor = UIBuilder.ErrorColor;
				this.errorMessage.FormattedText = message;
			}
		}


		private void AddAction()
		{
			var newUser = this.CreateUtilisateur ();
			this.comptaEntity.Utilisateurs.Add (newUser);

			this.UpdateTable (this.comptaEntity.Utilisateurs.Count-1);
			this.UpdateUserWidgets ();
			this.UpdateWidgets ();
			this.mainWindowController.SetDirty ();

			this.utilisateurField.Focus ();
		}

		private void RemoveAction()
		{
			var user = this.SelectedUser;
			System.Diagnostics.Debug.Assert (user != null);

			this.DeleteUtilisateur (user);
			this.comptaEntity.Utilisateurs.RemoveAt (this.table.SelectedRow);

			this.UpdateTable ();
			this.UpdateUserWidgets ();
			this.UpdateWidgets ();
			this.mainWindowController.SetDirty ();
		}

		private void ActionUtilisateurChanged()
		{
			var user = this.SelectedUser;
			System.Diagnostics.Debug.Assert (user != null);

			user.Utilisateur = this.utilisateurField.Text.Trim ();

			this.UpdateTable ();
			this.UpdateWidgets ();
			this.mainWindowController.SetDirty ();
		}

		private void ActionNomCompletChanged()
		{
			var user = this.SelectedUser;
			System.Diagnostics.Debug.Assert (user != null);

			user.NomComplet = this.nomCompletField.FormattedText;

			this.UpdateTable ();
			this.UpdateWidgets ();
			this.mainWindowController.SetDirty ();
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
					user.MotDePasse = Strings.ComputeMd5Hash (this.newPasswordField1.Text);

					this.UpdateTable ();
					this.UpdateWidgets ();

					//	Affiche un message sur fond vert, qui sera effacé au prochain UpdateWidgets.
					this.errorMessage.Visibility = true;
					this.errorMessage.BackColor = Color.FromName ("LightGreen");
					this.errorMessage.FormattedText = string.Format ("Le mot de passe de l'utilisateur \"{0}\" est défini.", user.Utilisateur);

					return;
				}
			}

			this.UpdateTable ();
			this.UpdateWidgets ();
			this.mainWindowController.SetDirty ();
		}

		private void ActionDateDébutChanged()
		{
			var user = this.SelectedUser;
			System.Diagnostics.Debug.Assert (user != null);

			Date? date = Converters.ParseDate (this.dateDébutField.Text);

			if (date == null)
			{
				user.DateDébut = null;
			}
			else
			{
				user.DateDébut = Date.Today;
			}

			this.dateDébutField.Text = Converters.DateToString (user.DateDébut);

			this.UpdateTable ();
			this.UpdateWidgets ();
			this.mainWindowController.SetDirty ();
		}

		private void ActionDateFinChanged()
		{
			var user = this.SelectedUser;
			System.Diagnostics.Debug.Assert (user != null);

			Date? date = Converters.ParseDate (this.dateFinField.Text);

			if (date == null)
			{
				user.DateFin = null;
			}
			else
			{
				user.DateFin = Date.Today;
			}

			this.dateFinField.Text = Converters.DateToString (user.DateFin);

			this.UpdateTable ();
			this.UpdateWidgets ();
			this.mainWindowController.SetDirty ();
		}

		private void ActionIdentitéWindowsChanged()
		{
			var user = this.SelectedUser;
			System.Diagnostics.Debug.Assert (user != null);

			user.IdentitéWindows = this.identitéWindowsCheckButton.ActiveState == ActiveState.Yes;

			this.UpdateTable ();
			this.UpdateWidgets ();
			this.mainWindowController.SetDirty ();
		}

		private void ActionDésactivéChanged()
		{
			var user = this.SelectedUser;
			System.Diagnostics.Debug.Assert (user != null);

			user.Désactivé = this.désactivéCheckButton.ActiveState == ActiveState.Yes;

			this.UpdateTable ();
			this.UpdateWidgets ();
			this.mainWindowController.SetDirty ();
		}


		private static void ToggleCheckButton(CheckButton button)
		{
			button.ActiveState = (button.ActiveState == ActiveState.Yes) ? ActiveState.No : ActiveState.Yes;
		}


		private void RemoveErrors()
		{
			int i = 0;
			while (i < this.comptaEntity.Utilisateurs.Count)
			{
				var user = this.comptaEntity.Utilisateurs[i];

				if (!user.Admin && !user.IsEntityValid)
				{
					this.DeleteUtilisateur (user);
					this.comptaEntity.Utilisateurs.RemoveAt (i);
				}
				else
				{
					i++;
				}
			}
		}


		private FormattedText GetErrorMessage()
		{
			var user = this.SelectedUser;

			if (user != null)
			{
				if (string.IsNullOrWhiteSpace (this.utilisateurField.Text))
				{
					return "Vous devez donner une identité à l'utilisateur.";
				}

				if (this.NamesCount != 0)
				{
					return string.Format ("Les noms \"{0}\" et \"{1}\" sont déjà utilisés.", this.utilisateurField.Text.Trim (), this.nomCompletField.Text);
				}

				if (string.IsNullOrWhiteSpace (this.nomCompletField.Text))
				{
					return "Vous devez donner un nom complet à l'utilisateur.";
				}

				FormattedText pwm = this.GetPasswordMessage ();
				if (pwm != null)
				{
					return pwm;
				}
			}

			for (int i = 0; i < this.comptaEntity.Utilisateurs.Count; i++)
			{
				if (this.comptaEntity.Utilisateurs[i].IsEntityEmpty)
				{
					return string.Format ("L'utilisateur en position {0} n'est pas défini.", (i+1).ToString ());
				}

				if (this.comptaEntity.Utilisateurs[i].IsEntityValid == false)
				{
					return string.Format ("L'utilisateur en position {0} n'est pas complètement défini.", (i+1).ToString ());
				}
			}

			return null;
		}

		private FormattedText GetPasswordMessage()
		{
			if (!string.IsNullOrEmpty (this.newPasswordField1.Text))
			{
				string err = UtilisateursController.CheckPassword (this.newPasswordField1.Text);
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

		private int NamesCount
		{
			get
			{
				var sel = this.table.SelectedRow;
				var loginName = this.utilisateurField.Text.Trim ();
				var displayName = this.nomCompletField.FormattedText;

				int count = 0;

				for (int i=0; i<this.comptaEntity.Utilisateurs.Count; i++)
				{
					var user = this.comptaEntity.Utilisateurs[i];

					if (i != sel && user.Utilisateur == loginName && user.NomComplet == displayName)
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


		private ComptaUtilisateurEntity CreateUtilisateur()
		{
			this.mainWindowController.SetDirty ();

			ComptaUtilisateurEntity utilisateur;

			if (this.businessContext == null)
			{
				utilisateur = new ComptaUtilisateurEntity ();
			}
			else
			{
				utilisateur = this.businessContext.CreateEntity<ComptaUtilisateurEntity> ();
			}

			string présentations = null;
			Converters.SetPrésentationCommand (ref présentations, Res.Commands.Présentation.Open, true);
			Converters.SetPrésentationCommand (ref présentations, Res.Commands.Présentation.Save, true);
			Converters.SetPrésentationCommand (ref présentations, Res.Commands.Présentation.Print, true);
			Converters.SetPrésentationCommand (ref présentations, Res.Commands.Présentation.Journal, true);
			Converters.SetPrésentationCommand (ref présentations, Res.Commands.Présentation.Extrait, true);
			Converters.SetPrésentationCommand (ref présentations, Res.Commands.Présentation.Balance, true);
			utilisateur.Présentations = présentations;

			return utilisateur;
		}

		private void DeleteUtilisateur(ComptaUtilisateurEntity utilisateur)
		{
			this.mainWindowController.SetDirty ();

			if (this.businessContext == null)
			{
				// rien à faire
			}
			else
			{
				this.businessContext.DeleteEntity (utilisateur);
			}
		}

		public ComptaUtilisateurEntity SelectedUser
		{
			get
			{
				int sel = this.table.SelectedRow;

				if (sel == -1)
				{
					return null;
				}

				return this.comptaEntity.Utilisateurs[sel];
			}
		}


		private readonly List<CheckButton>					checkButtons;

		private ComptaUtilisateurEntity						initialUser;
		private ComptaUtilisateurEntity						authenticatedUser;
		private FrameBox									toolbar;
		private GlyphButton									addButton;
		private GlyphButton									removeButton;
		private CellTable									table;

		private FrameBox									userBox;
		private TextFieldEx									utilisateurField;
		private TextFieldEx									nomCompletField;
		private CheckButton									désactivéCheckButton;
		private CheckButton									identitéWindowsCheckButton;
		private TextFieldEx									dateDébutField;
		private TextFieldEx									dateFinField;
		private StaticText									newPasswordLabel;
		private TextFieldEx									newPasswordField1;
		private TextFieldEx									newPasswordField2;

		private FrameBox									présentationsBox;
		private Button										zeroPrésentationButton;
		private Button										allPrésentationButton;

		private StaticText									errorMessage;
	}
}
