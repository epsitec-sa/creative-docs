//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Helpers;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Search.Data;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Search.Controllers
{
	/// <summary>
	/// Ce contrôleur gère la barre d'outil supérieure de recherche rapide pour la comptabilité.
	/// </summary>
	public class TopSearchController
	{
		public TopSearchController(AbstractController controller)
		{
			this.controller = controller;

			this.comptaEntity    = this.controller.ComptaEntity;
			this.dataAccessor    = this.controller.DataAccessor;
			this.businessContext = this.controller.BusinessContext;
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

		public void SearchClear()
		{
			this.searchController.SearchClear ();
		}


		public void CreateUI(FrameBox parent, System.Action searchStartAction, System.Action<int> searchNextAction)
		{
			this.toolbar = new FrameBox
			{
				Parent          = parent,
				PreferredHeight = TopSearchController.toolbarHeight,
				DrawFullFrame   = true,
				BackColor       = UIBuilder.SearchBackColor,
				Dock            = DockStyle.Top,
				Margins         = new Margins (0, 0, 0, 5),
				Visibility      = false,
			};

			this.searchController = new SearchController (this.controller, this.dataAccessor.SearchData, false);
			this.searchController.CreateUI (this.toolbar, searchStartAction, searchNextAction);
		}

		public void UpdateContent()
		{
			this.searchController.UpdateContent ();
		}


		public void UpdateColumns()
		{
			//	Met à jour les widgets en fonction de la liste des colonnes présentes.
			this.searchController.UpdateColumns ();
		}


		public void SetSearchCount(int dataCount, int? count, int? locator)
		{
			this.searchController.SetSearchCount (dataCount, count, locator);
		}


		private static readonly double			toolbarHeight = 20;

		private readonly AbstractController		controller;
		private readonly ComptaEntity			comptaEntity;
		private readonly BusinessContext		businessContext;
		private readonly AbstractDataAccessor	dataAccessor;

		private FrameBox						toolbar;
		private SearchController				searchController;
		protected bool							showPanel;
	}
}
