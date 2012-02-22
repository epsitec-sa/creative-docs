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

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Controllers
{
	/// <summary>
	/// Ce contrôleur gère le login de la comptabilité.
	/// </summary>
	public class LoginController : AbstractController
	{
		public LoginController(Application app, BusinessContext businessContext, MainWindowController mainWindowController)
			: base (app, businessContext, mainWindowController)
		{
		}


		protected override void UpdateTitle()
		{
			this.SetTitle ("Identification");
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
				return false;
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
				return false;
			}
		}


		protected override void CreateSpecificUI(FrameBox parent)
		{
			this.mainFrame = new FrameBox
			{
				Parent        = parent,
				DrawFullFrame = true,
//				BackColor     = Color.FromBrightness (0.95),
				BackColor     = Color.FromHexa ("e2ffe2"),  // vert clair
				Dock          = DockStyle.Fill,
				Padding       = new Margins (10),
			};

			new StaticText
			{
				Parent           = this.mainFrame,
				FormattedText    = Core.TextFormatter.FormatText("Identification").ApplyFontSize (80).ApplyBold ().ApplyFontColor (Color.FromName ("White")),
				ContentAlignment = ContentAlignment.TopLeft,
				PreferredHeight  = 120,
				Margins          = new Margins (200+10, 0, 0, 0),
				Dock             = DockStyle.Top,
			};

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
					PreferredWidth   = 200,
					Dock             = DockStyle.Left,
					Margins          = new Margins (0, 10, 0, 0),
				};

				this.currentField = new TextField
				{
					Parent         = line,
					FormattedText  = (this.mainWindowController.CurrentUser == null) ? Core.TextFormatter.FormatText ("Aucun").ApplyItalic () : this.mainWindowController.CurrentUser.Utilisateur,
					PreferredWidth = 200,
					IsReadOnly     = true,
					Dock           = DockStyle.Left,
				};
			}

			{
				var line = new FrameBox
				{
					Parent   = this.mainFrame,
					Dock     = DockStyle.Top,
					Margins  = new Margins (0, 0, 0, 2),
					TabIndex = 1,
				};

				new StaticText
				{
					Parent           = line,
					Text             = "Nouvel utilisateur",
					ContentAlignment = ContentAlignment.MiddleRight,
					PreferredWidth   = 200,
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
			}

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
					PreferredWidth   = 200,
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

			{
				var line = new FrameBox
				{
					Parent   = this.mainFrame,
					Dock     = DockStyle.Top,
					Margins  = new Margins (0, 0, 0, 10),
					TabIndex = 3,
				};

				this.loginButton = new Button
				{
					Parent         = line,
					Text           = "S'identifier",
					PreferredWidth = 200,
					Dock           = DockStyle.Left,
					Margins        = new Margins (200+10, 0, 0, 0),
					TabIndex       = 1,
				};
			}

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
					Margins         = new Margins (200+10, 0, 0, 0),
					TabIndex        = 1,
				};
			}

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

			this.UpdateWidgets ();
			this.userField.Focus ();
		}

		private void UpdateWidgets()
		{
			bool empty = string.IsNullOrEmpty (this.userField.Text) || string.IsNullOrEmpty (this.passwordField.Text);
			this.loginButton.Enable = !empty;
		}

		private void Login()
		{
			var entered = Converters.PreparingForSearh (this.userField.FormattedText);
			var utilisateur = this.comptaEntity.Utilisateurs.Where (x => Converters.PreparingForSearh (x.Utilisateur) == entered).FirstOrDefault ();

			if (utilisateur != null && utilisateur.MotDePasse == this.passwordField.Text)
			{
				this.userField.FormattedText = utilisateur.Utilisateur;
				this.mainWindowController.CurrentUser = utilisateur;
				this.SetError (false);
			}
			else
			{
				this.SetError (true);
			}
		}

		private void SetError(bool error)
		{
			if (error)
			{
				this.messageText.FormattedText = Core.TextFormatter.FormatText ("Le nom d'utilisateur ou le mot de passe sont faux").ApplyBold ().ApplyFontSize (20);
				this.mainFrame.BackColor = Color.FromHexa ("ffd6d6");  // rouge clair

				this.passwordField.Text = null;
				this.passwordField.Focus ();
			}
			else
			{
				this.messageText.FormattedText = Core.TextFormatter.FormatText ("Identification effectuée avec succès");
				this.mainFrame.BackColor = Color.FromHexa ("e2ffe2");  // vert clair
			}
		}


		private FrameBox			mainFrame;
		private TextField			currentField;
		private TextField			userField;
		private TextField			passwordField;
		private Button				loginButton;
		private StaticText			messageText;
	}
}
