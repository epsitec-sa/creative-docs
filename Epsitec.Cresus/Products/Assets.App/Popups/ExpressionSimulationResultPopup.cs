//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Cresus.Assets.App.DataFillers;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.NodeGetters;
using Epsitec.Cresus.Assets.App.Views.CommandToolbars;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.Expression;
using Epsitec.Cresus.Assets.Server.SimpleEngine;
using Epsitec.Cresus.Assets.App.Export;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.DataFillers;

namespace Epsitec.Cresus.Assets.App.Popups
{
	public class ExpressionSimulationResultPopup : AbstractPopup
	{
		private ExpressionSimulationResultPopup(DataAccessor accessor, CommandContext commandContext, List<ExpressionSimulationNode> nodes)
		{
			this.accessor = accessor;

			this.toolbar = new ExpressionSimulationResultToolbar (this.accessor, commandContext);

			this.visibleSelectedRow = -1;
			this.controller = new NavigationTreeTableController (this.accessor);

			this.nodeGetter = new ExpressionSimulationNodeGetter (nodes);
			this.dataFiller = new ExpressionSimulationTreeTableFiller (this.accessor, this.nodeGetter);

			//	Connexion des événements.
			this.controller.ContentChanged += delegate (object sender, bool crop)
			{
				this.UpdateController (crop);
			};
		}


		protected override Size DialogSize
		{
			get
			{
				return new Size
				(
					ExpressionSimulationTreeTableFiller.Width + AbstractScroller.DefaultBreadth,
					550
				);
			}
		}

		protected override void CreateUI()
		{
			this.CreateTitle (Res.Strings.Popup.ShowExpressionSimulation.Title.ToString ());
			this.CreateCloseButton ();

			this.toolbar.CreateUI (this.mainFrameBox);
			this.ConnectSearch ();

			this.CreateController ();
		}

		private void CreateController()
		{
			var frame = new FrameBox
			{
				Parent = this.mainFrameBox,
				Dock   = DockStyle.Fill,
			};

			this.controller.CreateUI (frame, headerHeight: 18, footerHeight: 0);
			this.controller.AllowsMovement = false;
			this.controller.AllowsSorting  = false;

			this.controller.RowClicked += delegate (object sender, int row, int column)
			{
				this.visibleSelectedRow = this.controller.TopVisibleRow + row;
				this.UpdateController ();
			};

			TreeTableFiller<ExpressionSimulationNode>.FillColumns (this.controller, this.dataFiller, "Popup.ShowExpressionSimulation");
		}


		private void ConnectSearch()
		{
			this.toolbar.Search += delegate (object sender, SearchDefinition definition, int direction)
			{
				this.Search (definition, direction);
			};
		}

		private void Search(SearchDefinition definition, int direction)
		{
			var row = FillerSearchEngine<ExpressionSimulationNode>.Search (this.accessor, this.nodeGetter, this.dataFiller, definition, this.visibleSelectedRow, direction);

			if (row != -1)
			{
				this.visibleSelectedRow = row;
				this.UpdateController ();
			}
		}


		[Command (Res.CommandIds.ExpressionSimulationResultToolbar.Export)]
		protected void OnExport(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			var target = this.toolbar.GetTarget (e);

			ExportHelpers<ExpressionSimulationNode>.StartExportProcess (target, this.accessor, this.dataFiller, this.controller.ColumnsState);
		}


		private void UpdateController(bool crop = true)
		{
			TreeTableFiller<ExpressionSimulationNode>.FillContent (this.controller, this.dataFiller, this.visibleSelectedRow, crop);
		}


		#region Helpers
		public static void Show(Widget target, DataAccessor accessor, CommandContext commandContext, List<ExpressionSimulationNode> nodes)
		{
			if (target != null)
			{
				var popup = new ExpressionSimulationResultPopup (accessor, commandContext, nodes);
				popup.Create (target, leftOrRight: true);
			}
		}
		#endregion


		private readonly DataAccessor							accessor;
		private readonly ExpressionSimulationResultToolbar		toolbar;
		private readonly NavigationTreeTableController			controller;
		private readonly ExpressionSimulationNodeGetter			nodeGetter;
		private readonly ExpressionSimulationTreeTableFiller	dataFiller;

		private int visibleSelectedRow;
	}
}