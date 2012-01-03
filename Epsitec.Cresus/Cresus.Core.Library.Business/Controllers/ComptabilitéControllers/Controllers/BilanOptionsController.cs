﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
	/// Ce contrôleur gère les options d'affichage du bilan de la comptabilité.
	/// </summary>
	public class BilanOptionsController : AbstractOptionsController<BilanData>
	{
		public BilanOptionsController(TileContainer tileContainer, ComptabilitéEntity comptabilitéEntity, BilanOptions options)
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
				PreferredHeight     = 55,
				Dock                = DockStyle.Top,
				Margins             = new Margins (0, 20, 0, 6),
				Padding             = new Margins (5),
			};

			this.CreateCheckUI (this.toolbar, optionsChanged);
			this.CreateDateUI (this.toolbar, optionsChanged);
		}

		protected void CreateCheckUI(FrameBox parent, System.Action optionsChanged)
		{
			var frame = new FrameBox
			{
				Parent          = parent,
				PreferredHeight = 20,
				Dock            = DockStyle.Top,
				TabIndex        = ++this.tabIndex,
			};

			var button = new CheckButton
			{
				Parent         = frame,
				FormattedText  = "Affiche les comptes dont le solde est nul",
				PreferredWidth = 300,
				AutoToggle     = false,
				Dock           = DockStyle.Left,
				TabIndex        = ++this.tabIndex,
			};

			button.Clicked += delegate
			{
				this.Options.ComptesNuls = !this.Options.ComptesNuls;
				button.ActiveState = this.Options.ComptesNuls ? ActiveState.Yes : ActiveState.No;
				optionsChanged ();
			};
		}

		private BilanOptions Options
		{
			get
			{
				return this.options as BilanOptions;
			}
		}
	}
}
