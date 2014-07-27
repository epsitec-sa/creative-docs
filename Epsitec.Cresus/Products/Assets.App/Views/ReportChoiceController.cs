//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.DataFillers;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.NodeGetters;
using Epsitec.Cresus.Assets.App.Reports;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class ReportChoiceController
	{
		public ReportChoiceController(DataAccessor accessor)
		{
			this.accessor = accessor;

			this.controller = new NavigationTreeTableController ();

			this.nodeGetter = new MessagesNodeGetter ();
			this.nodeGetter.SetParams (this.MessageNodes);

			this.dataFiller = new MessagesTreeTableFiller (this.accessor, this.nodeGetter)
			{
				Title = "Liste des rapports disponibles :",
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

				int sel = this.visibleSelectedRow;
				var array = ReportsList.ReportTypes.ToArray ();
				if (sel >= 0 && sel < array.Length)
				{
					this.OnReportSelected (array[sel]);
				}
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

			TreeTableFiller<MessageNode>.FillColumns (this.controller, this.dataFiller, "ReportChoiceController");
		}


		private IEnumerable<MessageNode> MessageNodes
		{
			get
			{
				return ReportsList.ReportTypes.Select (x => new MessageNode (ReportsList.GetReportName (x)));
			}
		}

		private void UpdateController(bool crop = true)
		{
			TreeTableFiller<MessageNode>.FillContent (this.controller, this.dataFiller, this.visibleSelectedRow, crop);
		}


		#region Events handler
		protected void OnReportSelected(ReportType reportType)
		{
			this.ReportSelected.Raise (this, reportType);
		}

		public event EventHandler<ReportType> ReportSelected;
		#endregion


		private const int headerHeight     = 40;  // hauteur standard = 22
		private const int rowHeight        = 18;
		private const int messageWidth     = 2000;


		private readonly DataAccessor					accessor;
		private readonly NavigationTreeTableController	controller;
		private readonly MessagesNodeGetter				nodeGetter;
		private readonly MessagesTreeTableFiller		dataFiller;

		private int										visibleSelectedRow;
	}
}
