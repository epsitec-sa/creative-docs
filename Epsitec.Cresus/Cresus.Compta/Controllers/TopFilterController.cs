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
		public TopFilterController(AbstractController controller)
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
						this.filterController.SetFocus ();
					}
					else
					{
						//	Il ne faut pas remettre à zéro le filtre lorsqu'on ferme la panneau du filtre,
						//	contrairement au panneau des recherches !
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
				Margins         = new Margins (0, 0, 0, 5),
				Visibility      = false,
			};

			this.filterController = new SearchController (this.controller, this.dataAccessor.FilterData, true);
			this.filterController.CreateUI (this.toolbar, searchStartAction, searchNextAction);
		}


		public void UpdateColumns()
		{
			//	Met à jour les widgets en fonction de la liste des colonnes présentes.
			this.filterController.UpdateColumns ();
		}


		public void SetFilterCount(int dataCount, int count, int allCount)
		{
			this.filterController.SetFilterCount (dataCount, count, allCount);
		}


		private static readonly double			toolbarHeight = 20;

		private readonly AbstractController		controller;
		private readonly ComptaEntity			comptaEntity;
		private readonly BusinessContext		businessContext;
		private readonly AbstractDataAccessor	dataAccessor;

		private FrameBox						toolbar;
		private SearchController				filterController;
		protected bool							showPanel;
	}
}
