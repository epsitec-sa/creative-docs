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


		public void CreateUI(Widget parent, bool extendedMode)
		{
			this.extendedMode = extendedMode;

			this.mainFrame = new FrameBox
			{
				Parent          = parent,
				PreferredHeight = TopTemporalController.toolbarHeight,
				Dock            = DockStyle.Fill,
				Margins         = new Margins (0, 0, 0, 0),
			};

			this.CreatePériodeUI (this.mainFrame);
			this.CreateTemporalFilterUI (this.mainFrame);
		}

		private void CreatePériodeUI(FrameBox parent)
		{
			new GlyphButton
			{
				Parent          = parent,
				CommandObject   = Res.Commands.Compta.PériodePrécédente,
				GlyphShape      = GlyphShape.ArrowLeft,
				ButtonStyle     = ButtonStyle.ToolItem,
				PreferredHeight = TopTemporalController.toolbarHeight,
				PreferredWidth  = TopTemporalController.toolbarHeight,
				Dock            = DockStyle.Left,
			};

			this.périodeLabel = new StaticText
			{
				Parent          = parent,
				PreferredHeight = TopTemporalController.toolbarHeight,
				Dock            = DockStyle.Left,
				Margins         = new Margins (5, 5, 0, 0),
			};

			new GlyphButton
			{
				Parent          = parent,
				CommandObject   = Res.Commands.Compta.PériodeSuivante,
				GlyphShape      = GlyphShape.ArrowRight,
				ButtonStyle     = ButtonStyle.ToolItem,
				PreferredHeight = TopTemporalController.toolbarHeight,
				PreferredWidth  = TopTemporalController.toolbarHeight,
				Dock            = DockStyle.Left,
				Margins         = new Margins (0, 10, 0, 0),
			};
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
			this.temporalController.CreateUI (frame, this.extendedMode, this.GetPériode, this.mainWindowController.TemporalDataChanged);
		}

		private ComptaPériodeEntity GetPériode()
		{
			return this.mainWindowController.Période;
		}


		public void UpdatePériode()
		{
			if (this.mainWindowController.Période != null)
			{
				this.périodeLabel.FormattedText = this.mainWindowController.Période.ShortTitle.ApplyFontSize (12.0);
				this.périodeLabel.PreferredWidth = this.périodeLabel.GetBestFitSize ().Width;
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

		private bool									extendedMode;
		private FrameBox								mainFrame;
		private TemporalController						temporalController;
		private StaticText								périodeLabel;
	}
}
