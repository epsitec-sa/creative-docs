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
		protected override void CreateCommands()
		{
			this.SetCommand (ToolbarCommand.AmortizationsPreview,   "Amortizations.Preview",   "Générer les préamortissements");
			this.SetCommand (ToolbarCommand.AmortizationsFix,       "Amortizations.Fix",       "Fixer les préamortissements");
			this.SetCommand (ToolbarCommand.AmortizationsToExtra,   "Amortizations.ToExtra",   "Transformer l'amortissement ordinaire en extraordinaire");
			this.SetCommand (ToolbarCommand.AmortizationsUnpreview, "Amortizations.Unpreview", "Supprimer les préamortissements");
			this.SetCommand (ToolbarCommand.AmortizationsDelete,    "Amortizations.Delete",    "Supprimer des amortissements ordinaires");
			this.SetCommand (ToolbarCommand.AmortizationsInfo,      "Amortizations.Info",      "Montrer le résultat de la dernière opération d'amortissement");
		}


		public override FrameBox CreateUI(Widget parent)
		{
			this.toolbar = new FrameBox
			{
				Parent          = parent,
				Dock            = DockStyle.Top,
				PreferredHeight = AbstractCommandToolbar.secondaryToolbarHeight,
				BackColor       = ColorManager.ToolbarBackgroundColor,
			};

			this.CreateCommandButton (DockStyle.Left, ToolbarCommand.AmortizationsPreview);
			this.CreateCommandButton (DockStyle.Left, ToolbarCommand.AmortizationsFix);
			this.CreateCommandButton (DockStyle.Left, ToolbarCommand.AmortizationsToExtra);
			this.CreateCommandButton (DockStyle.Left, ToolbarCommand.AmortizationsUnpreview);
			this.CreateCommandButton (DockStyle.Left, ToolbarCommand.AmortizationsDelete);
			this.CreateCommandButton (DockStyle.Left, ToolbarCommand.AmortizationsInfo);

			return this.toolbar;
		}
	}
}
