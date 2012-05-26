//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Support;

using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Helpers;
using Epsitec.Cresus.Compta.Search.Data;
using Epsitec.Cresus.Compta.Fields.Controllers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Search.Controllers
{
	/// <summary>
	/// Ce contrôleur gère la barre d'outil supérieure de filtre pour la comptabilité.
	/// </summary>
	public class TopTemporalController
	{
		public TopTemporalController(MainWindowController mainWindowController)
		{
			this.mainWindowController = mainWindowController;

			this.compta = this.mainWindowController.Compta;
			this.data   = this.mainWindowController.TemporalData;
		}


		public void CreateUI(Widget parent, System.Action searchStartAction)
		{
			this.searchStartAction = searchStartAction;

			this.mainFrame = new FrameBox
			{
				Parent          = parent,
				PreferredHeight = TopTemporalController.toolbarHeight,
				Dock            = DockStyle.Fill,
				Margins         = new Margins (0, 0, 0, 0),
			};

			new GlyphButton
			{
				Parent          = this.mainFrame,
				CommandObject   = Res.Commands.Compta.PériodePrécédente,
				GlyphShape      = GlyphShape.ArrowLeft,
				ButtonStyle     = ButtonStyle.ToolItem,
				PreferredHeight = TopTemporalController.toolbarHeight,
				PreferredWidth  = TopTemporalController.toolbarHeight,
				Dock            = DockStyle.Left,
			};

			this.périodeLabel = new StaticText
			{
				Parent          = this.mainFrame,
				PreferredHeight = TopTemporalController.toolbarHeight,
				Dock            = DockStyle.Left,
				Margins         = new Margins (5, 5, 0, 0),
			};

			new GlyphButton
			{
				Parent          = this.mainFrame,
				CommandObject   = Res.Commands.Compta.PériodeSuivante,
				GlyphShape      = GlyphShape.ArrowRight,
				ButtonStyle     = ButtonStyle.ToolItem,
				PreferredHeight = TopTemporalController.toolbarHeight,
				PreferredWidth  = TopTemporalController.toolbarHeight,
				Dock            = DockStyle.Left,
			};

			this.CreateSeparator (this.mainFrame);

			this.CreateButton (this.mainFrame, "J", "01: Janvier",   15);
			this.CreateButton (this.mainFrame, "F", "02: Février",   15);
			this.CreateButton (this.mainFrame, "M", "03: Mars",      15);
			this.CreateButton (this.mainFrame, "A", "04: Avril",     15);
			this.CreateButton (this.mainFrame, "M", "05: Mai",       15);
			this.CreateButton (this.mainFrame, "J", "06: Juin",      15);
			this.CreateButton (this.mainFrame, "J", "07: Juillet",   15);
			this.CreateButton (this.mainFrame, "A", "08: Août",      15);
			this.CreateButton (this.mainFrame, "S", "09: Septembre", 15);
			this.CreateButton (this.mainFrame, "O", "10: Octobre",   15);
			this.CreateButton (this.mainFrame, "N", "11: Novembre",  15);
			this.CreateButton (this.mainFrame, "D", "12: Décembre",  15);

			this.CreateSeparator (this.mainFrame);

			this.CreateButton (this.mainFrame, "T1", "Premier trimestre");
			this.CreateButton (this.mainFrame, "T2", "Deuxième trimestre");
			this.CreateButton (this.mainFrame, "T3", "Troisième trimestre");
			this.CreateButton (this.mainFrame, "T4", "Quatrième trimestre");

			this.CreateSeparator (this.mainFrame);

			this.CreateButton (this.mainFrame, "Année", "Toute l'année");

			this.CreateSeparator (this.mainFrame);

			this.CreateButton (this.mainFrame, "Autres...", "Choix d'une période quelconque");
		}

		public void UpdatePériode()
		{
			if (this.mainWindowController.Période != null)
			{
				this.périodeLabel.FormattedText = this.mainWindowController.Période.ShortTitle;
				this.périodeLabel.PreferredWidth = this.périodeLabel.GetBestFitSize ().Width;
			}
		}

		private void UpdateWidgets()
		{
		}

		private Button CreateButton(FrameBox parent, FormattedText text, FormattedText tooltip, double? width = null)
		{
			var button = new Button
			{
				Parent          = parent,
				FormattedText   = text,
				ButtonStyle     = ButtonStyle.ToolItem,
				PreferredHeight = TopTemporalController.toolbarHeight,
				Dock            = DockStyle.Left,
			};

			if (width.HasValue)
			{
				button.PreferredWidth = width.Value;
			}
			else
			{
				button.PreferredWidth = button.GetBestFitSize ().Width;
			}

			ToolTip.Default.SetToolTip (button, tooltip);

			return button;
		}

		private void CreateSeparator(FrameBox parent)
		{
			new Separator
			{
				Parent         = parent,
				PreferredWidth = 1,
				IsVerticalLine = true,
				Dock           = DockStyle.Left,
				Margins        = new Margins (10, 10, 0, 0),
			};
		}


		private static readonly double					toolbarHeight = 24;

		private readonly MainWindowController			mainWindowController;
		private readonly ComptaEntity					compta;
		private readonly TemporalData					data;

		private System.Action							searchStartAction;
		private FrameBox								mainFrame;
		private StaticText								périodeLabel;
	}
}
