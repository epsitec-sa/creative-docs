//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.DataFillers;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.NodeGetters;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Data.Reports;
using Epsitec.Cresus.Assets.Server.NodeGetters;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class ReportChoiceController
	{
		public ReportChoiceController(DataAccessor accessor)
		{
			this.accessor = accessor;

			this.controller = new NavigationTreeTableController ();

			this.nodeGetter = new ReportsNodeGetter (this.accessor);
			this.Update ();
			this.dataFiller = new ReportsTreeTableFiller (this.accessor, this.nodeGetter)
			{
				Title = Res.Strings.ReportChoiceController.Title.ToString (),
				Width = ReportChoiceController.messageWidth - (int) AbstractScroller.DefaultBreadth,
			};

			//	Connexion des événements.
			this.controller.ContentChanged += delegate (object sender, bool crop)
			{
				this.UpdateController (crop);
			};

			this.controller.RowClicked += delegate (object sender, int row, int column)
			{
				this.visibleSelectedRow = this.controller.TopVisibleRow + row;
				var node = this.nodeGetter[this.visibleSelectedRow];
				var reportParams = this.accessor.Mandat.Reports[node.Guid];
				this.OnReportSelected (reportParams);
			};
		}


		public void ClearSelection()
		{
			this.visibleSelectedRow = -1;
			this.UpdateController (false);
		}

		public void CreateUI(Widget parent)
		{
			var frame = new FrameBox
			{
				Parent = parent,
				Dock   = DockStyle.Fill,
			};

			this.CreateController (frame);
		}

		public void Update()
		{
			this.nodeGetter.SetParams (this.Nodes);
		}

		private void CreateController(Widget parent)
		{
			var frame = new FrameBox
			{
				Parent         = parent,
				PreferredWidth = ReportChoiceController.messageWidth,
				Dock           = DockStyle.Fill,
			};

			this.controller.CreateUI (frame, rowHeight: ReportChoiceController.rowHeight, headerHeight: ReportChoiceController.headerHeight, footerHeight: 0);
			this.controller.AllowsMovement = false;
			this.controller.AllowsSorting  = false;

			TreeTableFiller<ReportNode>.FillColumns (this.controller, this.dataFiller, "ReportChoiceController");
		}


		private IEnumerable<Guid> Nodes
		{
			get
			{
				return this.accessor.Mandat.Reports.Select (x => x.Guid);
			}
		}

		private void UpdateController(bool crop = true)
		{
			TreeTableFiller<ReportNode>.FillContent (this.controller, this.dataFiller, this.visibleSelectedRow, crop);
		}


		#region Events handler
		protected void OnReportSelected(AbstractReportParams reportParams)
		{
			this.ReportSelected.Raise (this, reportParams);
		}

		public event EventHandler<AbstractReportParams> ReportSelected;
		#endregion


		private const int headerHeight     = 35;  // hauteur standard = 22
		private const int rowHeight        = 18;
		private const int messageWidth     = 2000;


		private readonly DataAccessor					accessor;
		private readonly NavigationTreeTableController	controller;
		private readonly ReportsNodeGetter				nodeGetter;
		private readonly ReportsTreeTableFiller			dataFiller;

		private int										visibleSelectedRow;
	}
}
