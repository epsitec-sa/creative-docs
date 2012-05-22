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
using Epsitec.Cresus.Compta.Settings.Data;
using Epsitec.Cresus.Compta.Settings.Controllers;
using Epsitec.Cresus.Compta.ViewSettings.Data;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Controllers
{
	/// <summary>
	/// Ce contrôleur gère le login de la comptabilité.
	/// </summary>
	public class LoginController : AbstractController
	{
		public LoginController(ComptaApplication app, BusinessContext businessContext, MainWindowController mainWindowController)
			: base (app, businessContext, mainWindowController)
		{
		}


		protected override ViewSettingsList DirectViewSettingsList
		{
			get
			{
				return this.mainWindowController.GetViewSettingsList ("Présentation.Login.ViewSettings");
			}
		}


		protected override ControllerType ControllerType
		{
			get
			{
				return Controllers.ControllerType.Login;
			}
		}

		protected override void UpdateTitle()
		{
			this.SetTitle ();
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

		public override bool HasSearchPanel
		{
			get
			{
				return false;
			}
		}

		public override bool HasFilterPanel
		{
			get
			{
				return false;
			}
		}

		public override bool HasOptionsPanel
		{
			get
			{
				return false;
			}
		}

		public override bool HasInfoPanel
		{
			get
			{
				return false;
			}
		}


		protected override void CreateSpecificUI(FrameBox parent)
		{
			this.mainFrame = new FrameBox
			{
				Parent        = parent,
				DrawFullFrame = true,
				BackColor     = (this.mainWindowController.CurrentUser == null) ? Color.FromBrightness (0.95) : Color.FromHexa ("e2ffe2"),  // gris ou vert clair
				Dock          = DockStyle.Fill,
				Padding       = new Margins (10),
			};

			new StaticText
			{
				Parent           = this.mainFrame,
				FormattedText    = Core.TextFormatter.FormatText("Identification").ApplyFontSize (80).ApplyBold ().ApplyFontColor (Color.FromName ("White")),
				ContentAlignment = ContentAlignment.TopLeft,
				PreferredHeight  = 120,
				Margins          = new Margins (LoginController.leftMargin+10, 0, 0, 0),
				Dock             = DockStyle.Top,
			};

			//	Ligne 1.
			{
				var line = new FrameBox
				{
					Parent   = this.mainFrame,
					Dock     = DockStyle.Top,
					Margins  = new Margins (0, 0, 0, 10),
				};

				new StaticText
				{
					Parent           = line,
					Text             = "Utilisateur actuel",
					ContentAlignment = ContentAlignment.MiddleRight,
					PreferredWidth   = LoginController.leftMargin,
					Dock             = DockStyle.Left,
					Margins          = new Margins (0, 10, 0, 0),
				};

				this.currentField = new TextField
				{
					Parent         = line,
					PreferredWidth = 200,
					IsReadOnly     = true,
					Dock           = DockStyle.Left,
				};

				this.currentInfo = new StaticText
				{
					Parent         = line,
					Dock           = DockStyle.Fill,
					Margins        = new Margins (10, 0, 0, 0),
				};
			}

			//	Ligne 2.
			{
				var line = new FrameBox
				{
					Parent   = this.mainFrame,
					Dock     = DockStyle.Top,
					Margins  = new Margins (0, 0, 0, -1),
					TabIndex = 1,
				};

				new StaticText
				{
					Parent           = line,
					Text             = "Nouvel utilisateur",
					ContentAlignment = ContentAlignment.MiddleRight,
					PreferredWidth   = LoginController.leftMargin,
					Dock             = DockStyle.Left,
					Margins          = new Margins (0, 10, 0, 0),
				};

				this.userField = new TextField
				{
					Parent         = line,
					PreferredWidth = 200,
					Dock           = DockStyle.Left,
					TabIndex       = 1,
				};

				this.userInfo = new StaticText
				{
					Parent         = line,
					Dock           = DockStyle.Fill,
					Margins        = new Margins (10, 0, 0, 0),
				};
			}

			//	Ligne 3.
			{
				var line = new FrameBox
				{
					Parent   = this.mainFrame,
					Dock     = DockStyle.Top,
					Margins  = new Margins (0, 0, 0, 10),
					TabIndex = 2,
				};

				new StaticText
				{
					Parent           = line,
					Text             = "Mot de passe",
					ContentAlignment = ContentAlignment.MiddleRight,
					PreferredWidth   = LoginController.leftMargin,
					Dock             = DockStyle.Left,
					Margins          = new Margins (0, 10, 0, 0),
				};

				this.passwordField = new TextField
				{
					Parent                       = line,
					IsPassword                   = true,
					PasswordReplacementCharacter = '●',
					PreferredWidth               = 200,
					Dock                         = DockStyle.Left,
					TabIndex                     = 1,
				};
			}

			//	Ligne 4.
			{
				var line = new FrameBox
				{
					Parent   = this.mainFrame,
					Dock     = DockStyle.Top,
					Margins  = new Margins (0, 0, 0, 2),
					TabIndex = 3,
				};

				this.loginButton = new Button
				{
					Parent         = line,
					Text           = "S'identifier",
					PreferredWidth = 200,
					Dock           = DockStyle.Left,
					Margins        = new Margins (LoginController.leftMargin+10, 0, 0, 0),
					TabIndex       = 1,
				};
			}

			//	Ligne 5.
			{
				var line = new FrameBox
				{
					Parent   = this.mainFrame,
					Dock     = DockStyle.Top,
					Margins  = new Margins (0, 0, 0, 10),
					TabIndex = 4,
				};

				this.logoutButton = new Button
				{
					Parent         = line,
					Text           = "Se déconnecter",
					PreferredWidth = 200,
					Dock           = DockStyle.Left,
					Margins        = new Margins (LoginController.leftMargin+10, 0, 0, 0),
					TabIndex       = 1,
				};
			}

			//	Ligne 6.
			{
				var line = new FrameBox
				{
					Parent          = this.mainFrame,
					Dock            = DockStyle.Top,
					PreferredHeight = 40,
					Margins         = new Margins (0, 0, 0, 2),
					TabIndex        = 3,
				};

				this.messageText = new StaticText
				{
					Parent          = line,
					Dock            = DockStyle.Fill,
					Margins         = new Margins (LoginController.leftMargin+10, 0, 0, 0),
					TabIndex        = 1,
				};
			}

			//	Connexions.
			this.userField.TextChanged += delegate
			{
				this.UpdateWidgets ();
			};

			this.passwordField.TextChanged += delegate
			{
				this.UpdateWidgets ();
			};

			this.loginButton.Clicked += delegate
			{
				this.Login ();
			};

			this.logoutButton.Clicked += delegate
			{
				this.Logout ();
			};

			this.UpdateWidgets ();
			this.userField.Focus ();
		}


		public override void AcceptAction()
		{
			this.Login ();
		}


		private void UpdateWidgets()
		{
			if (this.mainWindowController.CurrentUser == null)
			{
				this.currentField.FormattedText = Core.TextFormatter.FormatText ("Aucun (déconnecté)").ApplyItalic ();
				this.currentInfo.FormattedText = null;
			}
			else
			{
				this.currentField.FormattedText = this.mainWindowController.CurrentUser.Utilisateur;
				this.currentInfo.FormattedText = this.mainWindowController.CurrentUser.NomComplet;
			}

			var utilisateur = this.EnteredUser;

			if (utilisateur == null)
			{
				this.userInfo.FormattedText = null;
			}
			else
			{
				this.userInfo.FormattedText = utilisateur.NomComplet;
			}

			bool empty = string.IsNullOrEmpty (this.userField.Text);

			if (this.mainWindowController.CurrentUser != null && this.userField.Text == this.mainWindowController.CurrentUser.Utilisateur)
			{
				empty = true;
			}

			this.loginButton.Enable = !empty;
			this.SetCommandEnable (Res.Commands.Edit.Accept, !empty);

			this.logoutButton.Enable = this.mainWindowController.CurrentUser != null;
		}

		private void Login()
		{
			var utilisateur = this.EnteredUser;
			var md5 = Strings.ComputeMd5Hash (this.passwordField.Text);

			FormattedText error = FormattedText.Null;

			if (utilisateur == null ||
				(string.IsNullOrEmpty (utilisateur.MotDePasse) && !string.IsNullOrEmpty (md5)) ||
				(!string.IsNullOrEmpty (utilisateur.MotDePasse) && utilisateur.MotDePasse != md5))
			{
				error = "Le nom d'utilisateur ou le mot de passe sont faux";
			}
			else if (utilisateur.Désactivé)
			{
				error = "Cet utilisateur est désactivé";
			}
			else if (!Dates.DateInRange (Date.Today, utilisateur.DateDébut, utilisateur.DateFin))
			{
				error = "Cet utilisateur n'est plus valide aujourd'hui";
			}

			if (error.IsNullOrEmpty)  // ok
			{
				this.userField.FormattedText = utilisateur.Utilisateur;
				this.mainWindowController.CurrentUser = utilisateur;
				this.SetError (Result.LoginOK);
			}
			else
			{
				this.mainWindowController.CurrentUser = null;
				this.SetError (Result.LoginError, error);
			}

			this.UpdateWidgets ();
		}

		private void Logout()
		{
			this.mainWindowController.CurrentUser = null;
			this.SetError (Result.LogoutOK);
			this.UpdateWidgets ();
		}

		private ComptaUtilisateurEntity EnteredUser
		{
			get
			{
				var entered = Strings.PreparingForSearh (this.userField.FormattedText);
				return this.compta.Utilisateurs.Where (x => Strings.PreparingForSearh (x.Utilisateur) == entered).FirstOrDefault ();
			}
		}


		private enum Result
		{
			LoginOK,
			LoginError,
			LogoutOK,
		}

		private void SetError(Result result, FormattedText? error = null)
		{
			if (result == Result.LoginError && error.HasValue)
			{
				this.messageText.FormattedText = error.Value.ApplyBold ().ApplyFontSize (20);
				this.mainFrame.BackColor = Color.FromHexa ("ffd6d6");  // rouge clair

				this.passwordField.Text = null;
				this.passwordField.Focus ();
			}

			if (result == Result.LoginOK)
			{
				this.messageText.FormattedText = Core.TextFormatter.FormatText ("Identification effectuée avec succès");
				this.mainFrame.BackColor = Color.FromHexa ("e2ffe2");  // vert clair
			}

			if (result == Result.LogoutOK)
			{
				this.messageText.FormattedText = Core.TextFormatter.FormatText ("Déconnexion effectuée avec succès");
				this.mainFrame.BackColor = Color.FromBrightness (0.95);  // gris clair

				this.userField.FormattedText = null;
				this.passwordField.FormattedText = null;
				this.userField.Focus ();
			}
		}


		private static readonly double leftMargin = 150;

		private FrameBox			mainFrame;
		private TextField			currentField;
		private StaticText			currentInfo;
		private TextField			userField;
		private StaticText			userInfo;
		private TextField			passwordField;
		private Button				loginButton;
		private Button				logoutButton;
		private StaticText			messageText;
	}
}
