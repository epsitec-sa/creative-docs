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
	public class TopSearchingController
	{
		public TopSearchingController(BusinessContext businessContext, List<ColumnMapper> columnMappers)
		{
			this.businessContext = businessContext;
			this.columnMappers = columnMappers;

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
				this.showPanel = value;
				this.toolbar.Visibility = this.showPanel;
			}
		}


		public void CreateUI(FrameBox parent, System.Action searchStartAction, System.Action<int> searchNextAction)
		{
			this.toolbar = new FrameBox
			{
				Parent          = parent,
				PreferredHeight = TopSearchingController.toolbarHeight,
				DrawFullFrame   = true,
				BackColor       = Color.FromBrightness (0.96),  // gris très clair
				Dock            = DockStyle.Top,
				Margins         = new Margins (0, 0, 0, 6),
				Padding         = new Margins (5),
			};

			this.searchingController = new SearchingController (this.searchingData, this.columnMappers);
			this.searchingController.CreateUI (this.toolbar, searchStartAction, searchNextAction);
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


		private static readonly double			toolbarHeight = 20;

		private readonly BusinessContext		businessContext;
		private readonly List<ColumnMapper>		columnMappers;
		private readonly SearchingData			searchingData;

		private FrameBox						toolbar;
		private SearchingController				searchingController;
		protected bool							showPanel;
	}
}
