//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.Compta.Accessors;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Controllers
{
	/// <summary>
	/// Ce contrôleur gère la barre d'outil supérieure de recherche rapide pour la comptabilité.
	/// </summary>
	public class TopToolbarController
	{
		public TopToolbarController(BusinessContext businessContext, List<ColumnMapper> columnMappers)
		{
			this.businessContext = businessContext;
			this.columnMappers = columnMappers;

			this.searchingData = new SearchingData ();
			this.toolbarShowed = true;
		}


		public void CreateUI(FrameBox parent, System.Action showHideAction, System.Action searchStartAction, System.Action<int> searchNextAction)
		{
			this.showHideAction = showHideAction;

			this.toolbar = new FrameBox
			{
				Parent          = parent,
				PreferredHeight = TopToolbarController.toolbarHeight,
				DrawFullFrame   = true,
				BackColor       = Color.FromBrightness (0.96),  // gris très clair
				Dock            = DockStyle.Top,
				Margins         = new Margins (0, 20, 0, 6),
				Padding         = new Margins (5),
			};

			this.searchingController = new SearchingController (this.searchingData, this.columnMappers);
			this.searchingController.CreateUI (this.toolbar, searchStartAction, searchNextAction);
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


		public SearchingData SearchingData
		{
			get
			{
				return this.searchingData;
			}
		}

		public void SetSearchingCount(int dataCount, int? count)
		{
			this.searchingController.SetSearchingCount (dataCount, count);
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

		private readonly BusinessContext		businessContext;
		private readonly List<ColumnMapper>		columnMappers;
		private readonly SearchingData			searchingData;

		private FrameBox						toolbar;
		private SearchingController				searchingController;
		private GlyphButton						showHideButton;
		private bool							toolbarShowed;
		private System.Action					showHideAction;
	}
}
