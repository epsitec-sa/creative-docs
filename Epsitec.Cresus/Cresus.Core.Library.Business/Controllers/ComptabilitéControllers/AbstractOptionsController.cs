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
	/// Ce contrôleur gère les options d'affichage génériques de la comptabilité.
	/// </summary>
	public abstract class AbstractOptionsController<Entity, Options>
		where Entity : class
		where Options : class
	{
		public AbstractOptionsController(TileContainer tileContainer, ComptabilitéEntity comptabilitéEntity, Options options)
		{
			this.tileContainer      = tileContainer;
			this.comptabilitéEntity = comptabilitéEntity;
			this.options            = options;

			this.toolbarShowed = true;
		}


		public virtual void CreateUI(FrameBox parent, System.Action optionsChanged)
		{
		}

		public void FinalizeUI(FrameBox parent)
		{
			//	Widgets créés en dernier, pour être par-dessus tout le reste.
			this.showHideButton = new GlyphButton
			{
				Parent        = parent,
				Anchor        = AnchorStyles.TopRight,
				PreferredSize = new Size (16, 16),
				ButtonStyle   = ButtonStyle.Slider,
			};

			this.showHideButton.Clicked += delegate
			{
				this.toolbarShowed = !this.toolbarShowed;
				this.UpdateShowHideButton ();
			};

			this.UpdateShowHideButton ();
		}

		public double TopOffset
		{
			set
			{
				if (this.topOffset != value)
				{
					this.topOffset = value;
					this.UpdateShowHideButton ();
				}
			}
		}


		private void UpdateShowHideButton()
		{
			//	Met à jour le bouton pour montrer/cacher la barre d'icône.
			this.showHideButton.GlyphShape = this.toolbarShowed ? GlyphShape.ArrowUp : GlyphShape.ArrowDown;
			this.showHideButton.Margins = new Margins (0, 0, this.toolbarShowed ? this.topOffset+20 : this.topOffset+0, 0);

			ToolTip.Default.SetToolTip (this.showHideButton, this.toolbarShowed ? "Cache les options" : "Montre les options");

			this.toolbar.Visibility   = this.toolbarShowed;
		}


		protected readonly TileContainer						tileContainer;
		protected readonly ComptabilitéEntity					comptabilitéEntity;
		protected readonly Options								options;

		protected FrameBox										toolbar;
		protected GlyphButton									showHideButton;
		protected bool											toolbarShowed;
		protected double										topOffset;
	}
}
