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
			var frame = new FrameBox
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

			this.userLabel = this.CreateButton (frame, "");
			ToolTip.Default.SetToolTip (this.userLabel, "Nom de l'utilisateur identifié");

			this.userLabel.Clicked += delegate
			{
				this.mainWindowController.ShowPrésentation (ControllerType.Login);
			};

			new Separator
			{
				Parent         = frame,
				PreferredWidth = 1,
				IsVerticalLine = true,
				Dock           = DockStyle.Left,
				Margins        = new Margins (10, 10, 0, 0),
			};

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

			this.topTemporalController = new TopTemporalController (this.mainWindowController);
			this.topTemporalController.CreateUI (frame);
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

		private Button							userLabel;
		private TopTemporalController			topTemporalController;
	}
}
