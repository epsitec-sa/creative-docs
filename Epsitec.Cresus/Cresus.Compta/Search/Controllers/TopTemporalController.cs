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


		public void CreateUI(Widget parent)
		{
			this.mainFrame = new FrameBox
			{
				Parent          = parent,
				PreferredHeight = TopTemporalController.toolbarHeight,
				Dock            = DockStyle.Fill,
			};

			this.CreatePériodeUI (this.mainFrame);

			new Separator
			{
				Parent         = this.mainFrame,
				PreferredWidth = 1,
				IsVerticalLine = true,
				Dock           = DockStyle.Left,
				Margins        = new Margins (10, 10, 0, 0),
			};

			this.CreateTemporalFilterUI (this.mainFrame);
		}

		private void CreatePériodeUI(FrameBox parent)
		{
			var frame = new FrameBox
			{
				Parent         = parent,
				PreferredWidth = 20,
				Dock           = DockStyle.Left,
				Padding        = new Margins (0, 0, 2, 2),
			};

			var prev = new IconButton
			{
				Parent          = frame,
				CommandObject   = Res.Commands.Compta.PériodePrécédente,
				ButtonStyle     = ButtonStyle.ToolItem,
				PreferredHeight = TopTemporalController.toolbarHeight-4,
				PreferredWidth  = TopTemporalController.toolbarHeight-4,
				Dock            = DockStyle.Left,
			};

			this.périodeLabel = new StaticText
			{
				Parent          = frame,
				PreferredHeight = TopTemporalController.toolbarHeight-4,
				Dock            = DockStyle.Left,
				Margins         = new Margins (5, 5, 0, 0),
			};

			var next = new IconButton
			{
				Parent          = frame,
				CommandObject   = Res.Commands.Compta.PériodeSuivante,
				ButtonStyle     = ButtonStyle.ToolItem,
				PreferredHeight = TopTemporalController.toolbarHeight-4,
				PreferredWidth  = TopTemporalController.toolbarHeight-4,
				Dock            = DockStyle.Left,
			};

			ToolTip.Default.SetToolTip (this.périodeLabel, "Exercice comptable en cours");
		}

		private void CreateTemporalFilterUI(FrameBox parent)
		{
			var frame = new FrameBox
			{
				Parent  = parent,
				Dock    = DockStyle.Fill,
				Padding = new Margins (0, 0, 2, 2),
			};

			this.temporalController = new TemporalController (this.data);
			this.temporalController.HasColorizedHilite = true;
			this.temporalController.CreateUI (frame, this.GetPériode, this.mainWindowController.TemporalDataChanged);
		}

		private TemporalData GetPériode()
		{
			if (this.mainWindowController.Période == null)
			{
				return null;
			}
			else
			{
				return new TemporalData
				{
					BeginDate = this.mainWindowController.Période.DateDébut,
					EndDate   = this.mainWindowController.Période.DateFin,
				};
			}
		}


		public void UpdatePériode()
		{
			if (this.mainWindowController.Période != null)
			{
				this.périodeLabel.FormattedText = this.mainWindowController.Période.ShortTitle.ApplyFontSize (12.0);
				this.périodeLabel.PreferredWidth = this.périodeLabel.GetBestFitSize ().Width;

				this.temporalController.UpdatePériode ();
			}
		}

		public void UpdateTemporalFilter()
		{
			this.temporalController.Update ();
		}


		private static readonly double					toolbarHeight = 24;

		private readonly MainWindowController			mainWindowController;
		private readonly ComptaEntity					compta;
		private readonly TemporalData					data;

		private FrameBox								mainFrame;
		private TemporalController						temporalController;
		private StaticText								périodeLabel;
	}
}
