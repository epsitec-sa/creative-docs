//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.DataFillers;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.NodeGetters;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.Expression;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	public class ShowExpressionSimulationPopup : AbstractPopup
	{
		public ShowExpressionSimulationPopup(DataAccessor accessor, List<ExpressionSimulationNode> nodes)
		{
			this.accessor = accessor;

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


		private void UpdateController(bool crop = true)
		{
			TreeTableFiller<ExpressionSimulationNode>.FillContent (this.controller, this.dataFiller, this.visibleSelectedRow, crop);
		}


		#region Helpers
		public static void Show(Widget target, DataAccessor accessor, List<ExpressionSimulationNode> nodes)
		{
			if (target != null)
			{
				var popup = new ShowExpressionSimulationPopup (accessor, nodes);
				popup.Create (target, leftOrRight: true);
			}
		}
		#endregion


		private readonly DataAccessor							accessor;
		private readonly NavigationTreeTableController			controller;
		private readonly ExpressionSimulationNodeGetter			nodeGetter;
		private readonly ExpressionSimulationTreeTableFiller	dataFiller;

		private int visibleSelectedRow;
	}
}