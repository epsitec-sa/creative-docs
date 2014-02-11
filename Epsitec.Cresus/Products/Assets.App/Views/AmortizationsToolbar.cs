//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Widgets;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class AmortizationsToolbar : AbstractCommandToolbar
	{
		public override FrameBox CreateUI(Widget parent)
		{
			this.toolbar = new FrameBox
			{
				Parent          = parent,
				Dock            = DockStyle.Top,
				PreferredHeight = AbstractCommandToolbar.secondaryToolbarHeight,
				BackColor       = ColorManager.ToolbarBackgroundColor,
			};

			this.buttonPreview   = this.CreateCommandButton (DockStyle.Left, ToolbarCommand.AmortizationsPreview,   "Amortizations.Preview",   "Générer l'aperçu des amortissements");
			this.buttonFix       = this.CreateCommandButton (DockStyle.Left, ToolbarCommand.AmortizationsFix,       "Amortizations.Fix",       "Fixer l'aperçu des amortissements");
			this.buttonUnpreview = this.CreateCommandButton (DockStyle.Left, ToolbarCommand.AmortizationsUnpreview, "Amortizations.Unpreview", "Supprimer l'aperçu des amortissements");
			this.buttonDelete    = this.CreateCommandButton (DockStyle.Left, ToolbarCommand.AmortizationsDelete,    "Amortizations.Delete",    "Supprimer des amortissements automatiques");
			this.buttonInfo      = this.CreateCommandButton (DockStyle.Left, ToolbarCommand.AmortizationsInfo,      "Amortizations.Info",      "Montre le résultat de la dernière opération d'amortissement");

			return this.toolbar;
		}


		private IconButton						buttonPreview;
		private IconButton						buttonFix;
		private IconButton						buttonUnpreview;
		private IconButton						buttonDelete;
		private IconButton						buttonInfo;
	}
}
