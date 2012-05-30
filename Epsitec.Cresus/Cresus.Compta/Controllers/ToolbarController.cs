//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Widgets;
using Epsitec.Cresus.Compta.Helpers;
using Epsitec.Cresus.Compta.Search.Controllers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Controllers
{
	/// <summary>
	/// Ce contrôleur gère la première barre présente depuis le haut de la fenêtre.
	/// On y trouve:
	/// - L'identité de l'utilisateur connecté.
	/// - Les icônes de quelques commandes générales.
	/// - Le filtre temporel.
	/// </summary>
	public class ToolbarController
	{
		public ToolbarController(MainWindowController mainWindowController)
		{
			this.mainWindowController = mainWindowController;
		}


		public void CreateUI(Widget parent)
		{
			var line1 = new WindowTitle
			{
				Parent              = parent,
				PreferredHeight     = 26,
				BackColor           = Color.FromHexa ("6fc3ff"), //("a3ccef"),  // bleu
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
				Dock                = DockStyle.Top,
				Padding             = new Margins (5, 5, 0, 0),
			};

#if false
			new Separator
			{
				Parent              = parent,
				PreferredHeight     = 1,
				IsVerticalLine      = false,
				Dock                = DockStyle.Top,
			};
#endif

			var line2 = new FrameBox
			{
				Parent              = parent,
				PreferredHeight     = 24,
				BackColor           = UIBuilder.WindowBackColor2,
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
				Dock                = DockStyle.Top,
				Padding             = new Margins (5, 5, 0, 0),
			};

			new Separator
			{
				Parent              = parent,
				PreferredHeight     = 1,
				IsVerticalLine      = false,
				Dock                = DockStyle.Top,
			};

			//	Partie gauche de la ligne supérieure.
			this.CreateButton (line1, Res.Commands.Navigator.Prev);
			this.CreateButton (line1, Res.Commands.Navigator.Next, 10);

			this.CreateButton (line1, Res.Commands.Edit.Undo);
			this.CreateButton (line1, Res.Commands.Edit.Redo, 10);

			this.CreateButton (line1, Res.Commands.Select.Up);
			this.CreateButton (line1, Res.Commands.Select.Down);
			this.CreateButton (line1, Res.Commands.Select.Home, 10);

			this.CreateWindowManagementButtons (line1);

			this.userLabel = this.CreateButton (line1, "");
			this.userLabel.Dock = DockStyle.Right;
			ToolTip.Default.SetToolTip (this.userLabel, "Nom de l'utilisateur identifié");

			new Separator
			{
				Parent         = line1,
				PreferredWidth = 1,
				IsVerticalLine = true,
				Dock           = DockStyle.Right,
				Margins        = new Margins (10, 10, 0, 0),
			};

			this.userLabel.Clicked += delegate
			{
				this.mainWindowController.ShowPrésentation (ControllerType.Login);
			};

			//	Partie centrale de la ligne supérieure.
			this.titleLabel = new StaticText
			{
				Parent           = line1,
				ContentAlignment = ContentAlignment.MiddleCenter,
				Dock             = DockStyle.Fill,
			};

			//	Ligne inférieure.
			this.topTemporalController = new TopTemporalController (this.mainWindowController);
			this.topTemporalController.CreateUI (line2, false);
		}


		public void UpdateTitle()
		{
			FormattedText title;

			var n1 = Présentations.GetGroupName (this.mainWindowController.SelectedDocument);
			var n3 = "Crésus Comptabilité NG";

			if (this.mainWindowController.Compta == null)
			{
				title = FormattedText.Concat (n1, " — ", n3);
			}
			else
			{
				var n2 = this.mainWindowController.Compta.Nom;
				title = FormattedText.Concat (n1, " — ", n2, " — ", n3);
			}

			//?this.titleLabel.FormattedText = title.ApplyBold ().ApplyFontSize (13.0);
			this.titleLabel.FormattedText = title.ApplyBold ().ApplyFontSize (13.5).ApplyFontColor (Color.FromBrightness (1));
		}

		public void UpdateUser()
		{
			var user = this.mainWindowController.CurrentUser;
			if (user == null)  // déconnecté ?
			{
				this.userLabel.FormattedText = FormattedText.Concat ("Déconnecté").ApplyItalic ();
			}
			else
			{
				this.userLabel.FormattedText = user.Utilisateur;
			}

			this.userLabel.PreferredWidth = this.userLabel.GetBestFitSize ().Width;
		}

		public void UpdatePériode()
		{
			this.topTemporalController.UpdatePériode ();
		}

		public void UpdateTemporalFilter()
		{
			this.topTemporalController.UpdateTemporalFilter ();
		}


		private Button CreateButton(FrameBox parent, FormattedText text)
		{
			var button = new Button
			{
				Parent          = parent,
				FormattedText   = text,
				ButtonStyle     = ButtonStyle.ToolItem,
				PreferredHeight = 24,
				Dock            = DockStyle.Left,
			};

			button.PreferredWidth = button.GetBestFitSize ().Width;

			return button;
		}

		private void CreateButton(Widget parent, Command cmd, double rightMargin = 0)
		{
			var button = UIBuilder.CreateButton (parent, cmd, 24, 20);
			button.Dock = DockStyle.Left;

			if (rightMargin != 0)
			{
				button.Margins = new Margins (0, rightMargin, 0, 0);
			}
		}


		private void CreateWindowManagementButtons(WindowTitle line1)
		{
			//	Partie droite de la ligne supérieure.
			this.closeButton = new GlyphButton
			{
				Parent         = line1,
				GlyphShape     = GlyphShape.Close,
				PreferredWidth = 48,
				Dock           = DockStyle.Right,
				Margins        = new Margins (-1, 0, -1, 7),
			};

			this.maximizeButton = new GlyphButton
			{
				Parent         = line1,
				GlyphShape     = GlyphShape.Plus,
				PreferredWidth = 28,
				Dock           = DockStyle.Right,
				Margins        = new Margins (-1, 0, -1, 7),
			};

			this.minimizeButton = new GlyphButton
			{
				Parent         = line1,
				GlyphShape     = GlyphShape.Minus,
				PreferredWidth = 28,
				Dock           = DockStyle.Right,
				Margins        = new Margins (10, 0, -1, 7),
			};

			var window = line1.Window;

			this.closeButton.Clicked += delegate
			{
				window.SimulateCloseClick ();
			};

			this.maximizeButton.Clicked += delegate
			{
				window.ToggleMaximize ();
			};

			this.minimizeButton.Clicked += delegate
			{
				window.ToggleMinimize ();
			};

			window.WindowActivated += delegate
			{
				line1.BackColor = Color.FromHexa ("6fc3ff"); //("a3ccef");  // bleu
			};

			window.WindowDeactivated += delegate
			{
				line1.BackColor = Color.FromHexa ("cccccc");
			};
		}
		
		private readonly MainWindowController	mainWindowController;

		private GlyphButton						closeButton;
		private GlyphButton						maximizeButton;
		private GlyphButton						minimizeButton;
		private Button							userLabel;
		private StaticText						titleLabel;
		private TopTemporalController			topTemporalController;
	}
}
