﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
	/// Ce contrôleur gère la barre de titre et la première barre présente depuis le haut de la fenêtre.
	/// On y trouve:
	/// - Les icônes de quelques commandes générales.
	/// - L'identité de l'utilisateur connecté.
	/// - Le contrôle de l'exercice.
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
			var titleBar = new WindowTitle
			{
				Parent              = parent,
				PreferredHeight     = 24+1,
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
				Dock                = DockStyle.Top,
				Padding             = new Margins (0, 0, 0, 1),
			};

			this.gradientTitle = new FrameBox
			{
				Parent              = titleBar,
				Dock                = DockStyle.Fill,
				Padding             = new Margins (5, 5, 0, 0),
			};

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
			new StaticText
			{
				Parent          = this.gradientTitle,
				Text            = UIBuilder.GetIconTag ("app"),
				PreferredWidth  = 24,
				PreferredHeight = 24,
				Dock            = DockStyle.Left,
				Margins         = new Margins (0, 0, 0, 0),
			};

			new Separator
			{
				Parent         = this.gradientTitle,
				PreferredWidth = 1,
				IsVerticalLine = true,
				Dock           = DockStyle.Left,
				Margins        = new Margins (1, 4, 0, 0),
			};

			this.CreateButton (this.gradientTitle, Res.Commands.Navigator.Prev);
			this.CreateButton (this.gradientTitle, Res.Commands.Navigator.Next);
			this.navigatorMenuButton = this.CreateButton (this.gradientTitle, Res.Commands.Navigator.Menu, 10);

			this.CreateButton (this.gradientTitle, Res.Commands.Edit.Undo);
			this.CreateButton (this.gradientTitle, Res.Commands.Edit.Redo, 10);

			this.CreateButton (this.gradientTitle, Res.Commands.Select.Up);
			this.CreateButton (this.gradientTitle, Res.Commands.Select.Down);
			this.CreateButton (this.gradientTitle, Res.Commands.Select.Home);

			new Separator
			{
				Parent         = this.gradientTitle,
				PreferredWidth = 1,
				IsVerticalLine = true,
				Dock           = DockStyle.Left,
				Margins        = new Margins (4, 0, 0, 0),
			};

			this.CreateWindowManagementButtons (this.gradientTitle);

			this.userLabel = this.CreateButton (this.gradientTitle, "");
			this.userLabel.Dock = DockStyle.Right;
			ToolTip.Default.SetToolTip (this.userLabel, "Nom de l'utilisateur identifié");

			new Separator
			{
				Parent         = this.gradientTitle,
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
				Parent           = this.gradientTitle,
				ContentAlignment = ContentAlignment.MiddleCenter,
				TextBreakMode    = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine,
				Dock             = DockStyle.Fill,
			};

			this.titleFilterButton = new IconButton
			{
				Parent           = this.titleLabel,
				IconUri          = UIBuilder.GetResourceIconUri ("Filter.Warning"),
				PreferredWidth   = 24,
				PreferredHeight  = 24,
				Anchor           = AnchorStyles.BottomLeft,
				Visibility       = false,
			};

			ToolTip.Default.SetToolTip (this.titleFilterButton, "Termine tous les filtres");

			this.titleFilterButton.Clicked += delegate
			{
				this.mainWindowController.ClearFilter ();
			};

			//	Ligne inférieure.
			this.topTemporalController = new TopTemporalController (this.mainWindowController);
			this.topTemporalController.CreateUI (line2);

			this.UpdateWindow ();
		}


		public Button NavigatorMenuButton
		{
			get
			{
				return this.navigatorMenuButton;
			}
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

			if (this.windowActivated)
			{
				this.titleLabel.FormattedText = title.ApplyFontSize (13.0).ApplyBold ();
			}
			else
			{
				this.titleLabel.FormattedText = title.ApplyFontSize (13.0);
			}

			if (this.mainWindowController.HasFilter)
			{
				double x = System.Math.Floor((this.titleLabel.ActualWidth - this.titleLabel.GetBestFitSize ().Width) / 2);

				this.titleFilterButton.Margins = new Margins (x-24-4, 0, 0, 2);
				this.titleFilterButton.Visibility = true;
			}
			else
			{
				this.titleFilterButton.Visibility = false;
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

			UIBuilder.AdjustWidth (this.userLabel);
		}

		public void UpdatePériode()
		{
			this.topTemporalController.UpdatePériode ();
		}

		public void UpdateTemporalFilter()
		{
			this.topTemporalController.UpdateTemporalFilter ();
		}


		private void CreateWindowManagementButtons(Widget parent)
		{
			//	Partie droite de la ligne supérieure.
			this.closeButton = new WindowButton
			{
				Parent           = parent,
				WindowButtonType = Widgets.WindowButtonType.Close,
				IconUri          = UIBuilder.GetResourceIconUri ("Window.Close"),
				PreferredWidth   = 48,
				Dock             = DockStyle.Right,
				Margins          = new Margins (-1, 0, -1, 7),
			};

			this.maximizeButton = new WindowButton
			{
				Parent           = parent,
				WindowButtonType = Widgets.WindowButtonType.Maximize,
				IconUri          = UIBuilder.GetResourceIconUri ("Window.Maximize"),
				PreferredWidth   = 28,
				Dock             = DockStyle.Right,
				Margins          = new Margins (-1, 0, -1, 7),
			};

			this.minimizeButton = new WindowButton
			{
				Parent           = parent,
				WindowButtonType = Widgets.WindowButtonType.Minimize,
				IconUri          = UIBuilder.GetResourceIconUri ("Window.Minimize"),
				PreferredWidth   = 28,
				Dock             = DockStyle.Right,
				Margins          = new Margins (10, 0, -1, 7),
			};

			ToolTip.Default.SetToolTip (this.minimizeButton, "Réduire");
			ToolTip.Default.SetToolTip (this.maximizeButton, "Agrandir");
			ToolTip.Default.SetToolTip (this.closeButton,    "Fermer");

			this.window = parent.Window;

			this.closeButton.Clicked += delegate
			{
				this.window.SimulateCloseClick ();
			};

			this.maximizeButton.Clicked += delegate
			{
				this.window.ToggleMaximize ();
			};

			this.minimizeButton.Clicked += delegate
			{
				this.window.ToggleMinimize ();
			};

			this.window.WindowActivated += delegate
			{
				this.windowActivated = true;
				this.UpdateWindow ();
			};

			this.window.WindowDeactivated += delegate
			{
				this.windowActivated = false;
				this.UpdateWindow ();
			};

			this.window.WindowPlacementChanged += delegate
			{
				this.UpdateWindow ();
			};
		}

		private void UpdateWindow()
		{
			this.isFullScreen = this.window.IsFullScreen;

			this.maximizeButton.IconUri = UIBuilder.GetResourceIconUri (this.isFullScreen ? "Window.Restore" : "Window.Maximize");
			ToolTip.Default.SetToolTip (this.maximizeButton, this.isFullScreen ? "Niveau inf." : "Agrandir");

			if (this.windowActivated)
			{
				this.gradientTitle.BackColor = UIBuilder.TitleBarActive;
				this.closeButton.BackColor = UIBuilder.TitleBarCloseButtonBack;
			}
			else
			{
				this.gradientTitle.BackColor = UIBuilder.TitleBarDesactive;
				this.closeButton.BackColor = Color.Empty;
			}

			this.UpdateTitle ();
		}

		private Button CreateButton(FrameBox parent, FormattedText text)
		{
			var button = new Button
			{
				Parent          = parent,
				FormattedText   = text,
				ButtonStyle     = ButtonStyle.ToolItem,
				Dock            = DockStyle.Left,
				Margins         = new Margins (0, 0, 2, 2),
			};

			UIBuilder.AdjustWidth (button);

			return button;
		}

		private Button CreateButton(Widget parent, Command cmd, double rightMargin = 0)
		{
			var button = UIBuilder.CreateButton (parent, cmd, 24, 20);

			button.Dock = DockStyle.Left;
			button.Margins = new Margins (0, rightMargin, 2, 2);

			return button;
		}


		private readonly MainWindowController	mainWindowController;

		private Window							window;
		private FrameBox						gradientTitle;
		private WindowButton					closeButton;
		private WindowButton					maximizeButton;
		private WindowButton					minimizeButton;
		private Button							userLabel;
		private Button							navigatorMenuButton;
		private StaticText						titleLabel;
		private IconButton						titleFilterButton;
		private TopTemporalController			topTemporalController;
		private bool							windowActivated;
		private bool							isFullScreen;
	}
}
