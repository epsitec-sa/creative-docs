//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.ComptabilitéControllers
{
	/// <summary>
	/// Ce contrôleur gère les options d'affichage de la balance de vérification de la comptabilité.
	/// </summary>
	public class BalanceOptionsController : AbstractOptionsController<BalanceData, BalanceOptions>
	{
		public BalanceOptionsController(TileContainer tileContainer, ComptabilitéEntity comptabilitéEntity, BalanceOptions options)
			: base (tileContainer, comptabilitéEntity, options)
		{
		}


		public override void CreateUI(FrameBox parent, System.Action optionsChanged)
		{
			this.toolbar = new FrameBox
			{
				Parent              = parent,
				DrawFullFrame       = true,
				BackColor           = Color.FromBrightness (0.96),  // gris très clair
				ContainerLayoutMode = Common.Widgets.ContainerLayoutMode.VerticalFlow,
				PreferredHeight     = 40,
				Dock                = DockStyle.Top,
				Margins             = new Margins (0, 20, 0, 6),
				Padding             = new Margins (5),
			};

			var button = new CheckButton
			{
				Parent         = this.toolbar,
				FormattedText  = "Affiche les comptes dont le solde est nul",
				PreferredWidth = 300,
				AutoToggle     = false,
				Dock           = DockStyle.Top,
			};

			button.Clicked += delegate
			{
				this.options.ComptesNuls = !this.options.ComptesNuls;
				button.ActiveState = this.options.ComptesNuls ? ActiveState.Yes : ActiveState.No;
				optionsChanged ();
			};
		}
	}
}
