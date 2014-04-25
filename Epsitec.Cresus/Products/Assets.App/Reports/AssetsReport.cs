﻿//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.DataFillers;
using Epsitec.Cresus.Assets.Server.NodeGetters;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class AssetsReport : AbstractReport
	{
		public AssetsReport(DataAccessor accessor, NavigationTreeTableController treeTableController)
			: base (accessor, treeTableController)
		{
			this.reportParams = new AssetsParams (Timestamp.Now, Guid.Empty);
		}


		public override void Initialize()
		{
			this.visibleSelectedRow = -1;

			var groupNodeGetter  = this.accessor.GetNodeGetter (BaseType.Groups);
			var objectNodeGetter = this.accessor.GetNodeGetter (BaseType.Assets);
			this.nodeGetter = new ObjectsNodeGetter (this.accessor, groupNodeGetter, objectNodeGetter);

			this.sortingInstructions = new SortingInstructions (this.accessor.GetMainStringField (BaseType.Assets), SortedType.Ascending, ObjectField.Unknown, SortedType.None);

			this.dataFiller = new AssetsTreeTableFiller (this.accessor, this.NodeGetter);
			TreeTableFiller<CumulNode>.FillColumns (this.treeTableController, this.dataFiller);

			base.Initialize ();
		}

		public override void ShowParamsPopup(Widget target)
		{
			var popup = new AssetsReportPopup (this.accessor)
			{
				Date      = this.Params.Timestamp.Date,
				GroupGuid = this.Params.RootGuid,
			};

			popup.Create (target);

			popup.ButtonClicked += delegate (object sender, string name)
			{
				if (name == "ok")
				{
					this.reportParams = new AssetsParams
					(
						new Timestamp (popup.Date.GetValueOrDefault (), 0),
						popup.GroupGuid
					);

					this.UpdateParams ();
				}
			};
		}

		protected override void UpdateParams()
		{
			this.NodeGetter.SetParams (this.Params.Timestamp, this.Params.RootGuid, this.sortingInstructions);
			this.dataFiller.Timestamp = this.Params.Timestamp;

			this.UpdateTreeTable ();
		}


		protected override void UpdateTreeTable()
		{
			TreeTableFiller<CumulNode>.FillContent (this.treeTableController, this.dataFiller, this.visibleSelectedRow, crop: true);
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
		private AbstractTreeTableFiller<CumulNode>	dataFiller;
	}
}
