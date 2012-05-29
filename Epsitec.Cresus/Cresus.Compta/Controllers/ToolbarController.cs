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
			this.box = new FrameBox
			{
				Parent = parent,
				Dock   = DockStyle.Top,
			};

			this.extendedButton = new GlyphButton
			{
				Parent          = parent,
				GlyphShape      = this.extendedMode ? GlyphShape.TriangleUp : GlyphShape.TriangleDown,
				ButtonStyle     = ButtonStyle.ToolItem,
				PreferredHeight = 20,
				PreferredWidth  = 20,
				Anchor          = AnchorStyles.TopRight,
				Margins         = new Margins (0, 2, 2, 0),
			};

			this.extendedButton.Clicked += new Common.Support.EventHandler<MessageEventArgs> (this.HandleEtendedButtonClicked);

			this.CreateModeUI ();
		}

		private void HandleEtendedButtonClicked(object sender, MessageEventArgs e)
		{
			this.extendedMode = !this.extendedMode;
			this.extendedButton.GlyphShape = this.extendedMode ? GlyphShape.TriangleUp : GlyphShape.TriangleDown;

			Application.QueueAsyncCallback
			(
				delegate
				{
					this.box.Children.Clear ();
					this.CreateModeUI ();

					this.UpdateTitle ();
					this.UpdateUser ();
					this.UpdatePériode ();
					this.UpdateTemporalFilter ();
				}
			);
		}

		public void CreateModeUI()
		{
			if (this.extendedMode)
			{
				this.CreateExtendedUI (this.box);
			}
			else
			{
				this.CreateCompactUI (this.box);
			}
		}

		public void CreateCompactUI(Widget parent)
		{
			var frame = new FrameBox
			{
				Parent              = parent,
				PreferredHeight     = 24,
				BackColor           = UIBuilder.WindowBackColor2,
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
				Dock                = DockStyle.Top,
				Padding             = new Margins (5, 20+5, 0, 0),
			};

			new Separator
			{
				Parent              = parent,
				PreferredHeight     = 1,
				IsVerticalLine      = false,
				Dock                = DockStyle.Top,
			};

			//	Partie gauche.
			this.CreateButton (frame, Res.Commands.Navigator.Prev);
			this.CreateButton (frame, Res.Commands.Navigator.Next, 10);

			this.CreateButton (frame, Res.Commands.Edit.Undo);
			this.CreateButton (frame, Res.Commands.Edit.Redo, 10);

			this.CreateButton (frame, Res.Commands.Select.Up);
			this.CreateButton (frame, Res.Commands.Select.Down);
			this.CreateButton (frame, Res.Commands.Select.Home, 10);

			new Separator
			{
				Parent         = frame,
				PreferredWidth = 1,
				IsVerticalLine = true,
				Dock           = DockStyle.Left,
				Margins        = new Margins (0, 10, 0, 0),
			};

			this.titleLabel = new StaticText
			{
				Parent           = frame,
				ContentAlignment = ContentAlignment.MiddleLeft,
				Dock             = DockStyle.Left,
			};

			new Separator
			{
				Parent         = frame,
				PreferredWidth = 1,
				IsVerticalLine = true,
				Dock           = DockStyle.Left,
				Margins        = new Margins (10, 10, 0, 0),
			};

			this.topTemporalController = new TopTemporalController (this.mainWindowController);
			this.topTemporalController.CreateUI (frame, this.extendedMode);

			//	Partie droite.
			this.userLabel = this.CreateButton (frame, "");
			this.userLabel.Dock = DockStyle.Right;
			ToolTip.Default.SetToolTip (this.userLabel, "Nom de l'utilisateur identifié");

			new Separator
			{
				Parent         = frame,
				PreferredWidth = 1,
				IsVerticalLine = true,
				Dock           = DockStyle.Right,
				Margins        = new Margins (10, 10, 0, 0),
			};

			this.userLabel.Clicked += delegate
			{
				this.mainWindowController.ShowPrésentation (ControllerType.Login);
			};
		}

		public void CreateExtendedUI(Widget parent)
		{
			var line1 = new FrameBox
			{
				Parent              = parent,
				PreferredHeight     = 24,
				BackColor           = UIBuilder.WindowBackColor2,
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
				Dock                = DockStyle.Top,
				Padding             = new Margins (5, 20+5, 0, 0),
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

			//	Partie droite de la ligne supérieure.
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
			this.topTemporalController.CreateUI (line2, this.extendedMode);
		}


		public void UpdateTitle()
		{
			if (this.extendedMode)
			{
				var n1 = Présentations.GetGroupName (this.mainWindowController.SelectedDocument);
				var n2 = this.mainWindowController.Compta.Nom;
				var n3 = "Crésus Comptabilité NG";

				var n = FormattedText.Concat (n1, " — ", n2, " — ", n3).ApplyBold ().ApplyFontSize (13.0);
				this.titleLabel.FormattedText = n;
			}
			else
			{
				var n = Présentations.GetGroupName (this.mainWindowController.SelectedDocument).ApplyBold ().ApplyFontSize (12.0);
				this.titleLabel.FormattedText = n;
				this.titleLabel.PreferredWidth = this.titleLabel.GetBestFitSize ().Width;
			}
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


		private readonly MainWindowController	mainWindowController;

		private FrameBox						box;
		private bool							extendedMode;
		private GlyphButton						extendedButton;
		private Button							userLabel;
		private StaticText						titleLabel;
		private TopTemporalController			topTemporalController;
	}
}
