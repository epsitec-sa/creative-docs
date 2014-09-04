//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Export;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Data.Reports;
using Epsitec.Cresus.Assets.Server.DataFillers;
using Epsitec.Cresus.Assets.Server.NodeGetters;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class AssetsReport : AbstractReport
	{
		public AssetsReport(DataAccessor accessor)
			: base (accessor)
		{
		}


		public override void Initialize(NavigationTreeTableController treeTableController)
		{
			this.treeTableController = treeTableController;
			this.visibleSelectedRow = -1;

			var groupNodeGetter  = this.accessor.GetNodeGetter (BaseType.Groups);
			var objectNodeGetter = this.accessor.GetNodeGetter (BaseType.Assets);
			this.nodeGetter = new ObjectsNodeGetter (this.accessor, groupNodeGetter, objectNodeGetter);

			this.dataFiller = new AssetsTreeTableFiller (this.accessor, this.NodeGetter);
			TreeTableFiller<SortableCumulNode>.FillColumns (this.treeTableController, this.dataFiller, "View.Report.Assets");

			this.sortingInstructions = TreeTableFiller<SortableCumulNode>.GetSortingInstructions (this.treeTableController);

			base.Initialize (treeTableController);
		}

		public override void ShowParamsPopup(Widget target)
		{
			//	Affiche le Popup pour choisir les paramètres d'un rapport.
			var popup = new AssetsReportPopup (this.accessor)
			{
				Date      = this.Params.Timestamp.Date,
				GroupGuid = this.Params.RootGuid,
				Level     = this.Params.Level,
			};

			popup.Create (target, leftOrRight: true);

			popup.ButtonClicked += delegate (object sender, string name)
			{
				if (name == "ok")
				{
					this.reportParams = new AssetsParams
					(
						new Timestamp (popup.Date.GetValueOrDefault (), 0),
						popup.GroupGuid,
						popup.Level
					);
					this.UpdateParams ();
				}
			};
		}

		public override void UpdateParams()
		{
			if (this.Params == null)
			{
				return;
			}

			this.dataFiller.Title = this.Title;
			this.dataFiller.Timestamp = this.Params.Timestamp;

			this.NodeGetter.SetParams (this.Params.Timestamp, this.Params.RootGuid, Guid.Empty, this.sortingInstructions);

			if (this.Params.Level.HasValue)
			{
				this.NodeGetter.SetLevel (this.Params.Level.Value);
			}

			this.UpdateTreeTable ();

			this.OnParamsChanged ();
			this.OnUpdateCommands ();
		}

		public override void ShowExportPopup(Widget target)
		{
			ExportHelpers<SortableCumulNode>.StartExportProcess (target, this.accessor, this.dataFiller, this.treeTableController.ColumnsState);
		}


		protected override void HandleSortingChanged(object sender)
		{
			this.sortingInstructions = TreeTableFiller<SortableCumulNode>.GetSortingInstructions (this.treeTableController);
			this.UpdateParams ();
		}


		protected override void UpdateTreeTable()
		{
			TreeTableFiller<SortableCumulNode>.FillContent (this.treeTableController, this.dataFiller, this.visibleSelectedRow, crop: true);
		}


		private AssetsParams Params
		{
			get
			{
				return this.reportParams as AssetsParams;
			}
		}

		private ObjectsNodeGetter NodeGetter
		{
			get
			{
				return this.nodeGetter as ObjectsNodeGetter;
			}
		}


		private SortingInstructions					sortingInstructions;
		private AbstractTreeTableFiller<SortableCumulNode>	dataFiller;
	}
}
