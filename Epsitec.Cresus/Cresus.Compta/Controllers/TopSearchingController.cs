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
	/// Ce contrôleur gère la barre d'outil supérieure de recherche rapide pour la comptabilité.
	/// </summary>
	public class TopSearchingController
	{
		public TopSearchingController(ComptaEntity comptaEntity, BusinessContext businessContext, List<ColumnMapper> columnMappers)
		{
			this.comptaEntity    = comptaEntity;
			this.businessContext = businessContext;
			this.columnMappers   = columnMappers;

			this.searchingData = new SearchingData ();
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
						this.searchingController.SetFocus ();
					}
					else
					{
						this.searchingController.SearchClear ();
					}
				}
			}
		}


		public void CreateUI(FrameBox parent, System.Action searchStartAction, System.Action<int> searchNextAction)
		{
			this.toolbar = new FrameBox
			{
				Parent          = parent,
				PreferredHeight = TopSearchingController.toolbarHeight,
				DrawFullFrame   = true,
				BackColor       = Color.FromHexa ("ffffdd"),  // jaune pastel
				Dock            = DockStyle.Top,
				Margins         = new Margins (0, 0, 0, 6),
				Padding         = new Margins (5),
				Visibility      = false,
			};

			this.searchingController = new SearchingController (this.comptaEntity, this.searchingData, this.columnMappers);
			this.searchingController.CreateUI (this.toolbar, searchStartAction, searchNextAction);
		}


		public void UpdateColumns(List<ColumnMapper> columnMappers)
		{
			//	Met à jour les widgets en fonction de la liste des colonnes présentes.
			this.columnMappers = columnMappers;

			this.searchingController.UpdateColumns (columnMappers);
		}


		public SearchingData SearchingData
		{
			get
			{
				return this.searchingData;
			}
		}

		public void SetSearchingCount(int dataCount, int? count, int? locator)
		{
			this.searchingController.SetSearchingCount (dataCount, count, locator);
		}


		private static readonly double			toolbarHeight = 20;

		private readonly ComptaEntity			comptaEntity;
		private readonly BusinessContext		businessContext;
		private readonly SearchingData			searchingData;

		private List<ColumnMapper>				columnMappers;
		private FrameBox						toolbar;
		private SearchingController				searchingController;
		protected bool							showPanel;
	}
}
