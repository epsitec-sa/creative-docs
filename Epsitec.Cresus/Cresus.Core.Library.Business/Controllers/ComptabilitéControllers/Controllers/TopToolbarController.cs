//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

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
	/// Ce contrôleur gère la barre d'outil supérieure de recherche rapide pour la comptabilité.
	/// </summary>
	public class TopToolbarController
	{
		public TopToolbarController(TileContainer tileContainer)
		{
			this.tileContainer = tileContainer;
			this.toolbarShowed = true;
		}


		public void CreateUI(FrameBox parent, System.Action importAction, System.Action showHideAction)
		{
			this.showHideAction = showHideAction;

			this.toolbar = new FrameBox
			{
				Parent          = parent,
				PreferredHeight = TopToolbarController.toolbarHeight,
				DrawFullFrame   = true,
				BackColor       = Color.FromBrightness (0.96),  // gris très clair
				Dock            = DockStyle.Top,
				Margins         = new Margins (0, 20, 2, 6),
				Padding         = new Margins (5),
			};

			// TODO: Pour l'instant, tous les widgets pour la recherche rapide sont factices !
			{
				new StaticText
				{
					Parent         = this.toolbar,
					Text           = "Rechercher",
					PreferredWidth = 64,
					Dock           = DockStyle.Left,
				};

				new TextField
				{
					Parent          = this.toolbar,
					PreferredWidth  = 100,
					PreferredHeight = TopToolbarController.toolbarHeight,
					Dock            = DockStyle.Left,
					Margins         = new Margins (0, 10, 0, 0),
				};

				new Button
				{
					Parent          = this.toolbar,
					Text            = "Suivant",
					PreferredWidth  = 60,
					PreferredHeight = TopToolbarController.toolbarHeight,
					Dock            = DockStyle.Left,
					Enable          = false,
					Margins         = new Margins (0, 1, 0, 0),
				};

				new Button
				{
					Parent          = this.toolbar,
					Text            = "Précédent",
					PreferredWidth  = 60,
					PreferredHeight = TopToolbarController.toolbarHeight,
					Dock            = DockStyle.Left,
					Enable          = false,
					Margins         = new Margins (0, 10, 0, 0),
				};

				new Button
				{
					Parent          = this.toolbar,
					Text            = "Options...",
					PreferredWidth  = 60,
					PreferredHeight = TopToolbarController.toolbarHeight,
					Dock            = DockStyle.Left,
					Enable          = false,
					Margins         = new Margins (0, 10, 0, 0),
				};

				new CheckButton
				{
					Parent          = this.toolbar,
					Text            = "Filtre instantané",
					PreferredWidth  = 100,
					PreferredHeight = TopToolbarController.toolbarHeight,
					Dock            = DockStyle.Left,
					Enable          = false,
				};
			}

			//	<--|
			this.importButton = new IconButton
			{
				Parent          = this.toolbar,
				IconUri         = Misc.GetResourceIconUri ("Import"),
				AutoFocus       = false,
				Visibility      = false,
				PreferredHeight = TopToolbarController.toolbarHeight,
				Dock            = DockStyle.Right,
				Margins         = new Margins (0, 0, 0, 0),
			};

			this.importButton.Clicked += delegate
			{
				importAction ();
			};

			ToolTip.Default.SetToolTip (this.importButton, "Importe le plan comptable de l'année courante");
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
				this.showHideAction ();
			};

			this.UpdateShowHideButton ();
		}

		public double TopOffset
		{
			get
			{
				return this.toolbarShowed ? 38 : 0;
			}
		}


		public bool ImportEnable
		{
			get
			{
				return this.importButton.Visibility;
			}
			set
			{
				this.importButton.Visibility = value;
			}
		}


		private void UpdateShowHideButton()
		{
			//	Met à jour le bouton pour montrer/cacher la barre d'icône.
			this.showHideButton.GlyphShape = this.toolbarShowed ? GlyphShape.ArrowUp : GlyphShape.ArrowDown;
			this.showHideButton.Margins = new Margins (0, 0, this.toolbarShowed ? 9 : 0, 0);

			ToolTip.Default.SetToolTip (this.showHideButton, this.toolbarShowed ? "Cache la barre de recherche" : "Montre la barre de recherche");

			this.toolbar.Visibility   = this.toolbarShowed;
		}


		private static readonly double			toolbarHeight = 20;

		private readonly TileContainer			tileContainer;

		private FrameBox						toolbar;
		private IconButton						importButton;
		private GlyphButton						showHideButton;
		private bool							toolbarShowed;
		private System.Action					showHideAction;
	}
}
