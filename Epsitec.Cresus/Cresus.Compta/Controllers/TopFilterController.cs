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
using Epsitec.Cresus.Compta.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Controllers
{
	/// <summary>
	/// Ce contrôleur gère la barre d'outil supérieure de filtre pour la comptabilité.
	/// </summary>
	public class TopFilterController
	{
		public TopFilterController(ComptaEntity comptaEntity, BusinessContext businessContext, AbstractDataAccessor dataAccessor)
		{
			this.comptaEntity    = comptaEntity;
			this.businessContext = businessContext;
			this.dataAccessor    = dataAccessor;
		}


		public bool ShowPanel
		{
			get
			{
				return this.showPanel;
			}
			set
			{
				if (this.showPanel != value)
				{
					this.showPanel = value;
					this.toolbar.Visibility = this.showPanel;

					if (this.showPanel)
					{
						this.searchController.SetFocus ();
					}
					else
					{
						this.searchController.SearchClear ();
					}
				}
			}
		}


		public void CreateUI(FrameBox parent, System.Action searchStartAction, System.Action<int> searchNextAction)
		{
			this.toolbar = new FrameBox
			{
				Parent          = parent,
				PreferredHeight = TopFilterController.toolbarHeight,
				DrawFullFrame   = true,
				BackColor       = Color.FromHexa ("ccffcc"),  // vert pastel
				Dock            = DockStyle.Top,
				Margins         = new Margins (0, 0, 0, 6),
				Padding         = new Margins (5),
				Visibility      = false,
			};

			this.searchController = new SearchController (this.comptaEntity, this.dataAccessor.FilterData, this.dataAccessor.ColumnMappers, true);
			this.searchController.CreateUI (this.toolbar, searchStartAction, searchNextAction);
		}


		public void UpdateColumns()
		{
			//	Met à jour les widgets en fonction de la liste des colonnes présentes.
			this.searchController.UpdateColumns ();
		}


		public void SetFilterCount(int dataCount, int count, int allCount)
		{
			this.searchController.SetFilterCount (dataCount, count, allCount);
		}


		private static readonly double			toolbarHeight = 20;

		private readonly ComptaEntity			comptaEntity;
		private readonly BusinessContext		businessContext;
		private readonly AbstractDataAccessor	dataAccessor;

		private FrameBox						toolbar;
		private SearchController				searchController;
		protected bool							showPanel;
	}
}
