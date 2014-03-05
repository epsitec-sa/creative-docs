//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.DataFillers;
using Epsitec.Cresus.Assets.Server.NodeGetters;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public abstract class AbstractToolbarTreeTableController<T> : AbstractToolbarTreeController<T>
		where T : struct
	{
		public AbstractToolbarTreeTableController(DataAccessor accessor, BaseType baseType)
			: base (accessor, baseType)
		{
		}


		protected override void CreateControllerUI(Widget parent)
		{
			this.selectedRow = -1;

			this.treeTableFrame = new FrameBox
			{
				Parent = parent,
				Dock   = DockStyle.Fill,
			};

			this.graphicFrame = new FrameBox
			{
				Parent = parent,
				Dock   = DockStyle.Fill,
			};

			this.CreateTreeTableControllerUI (this.treeTableFrame);
			this.CreateGraphicControllerUI (this.graphicFrame);

			this.UpdateGraphicMode ();
		}

		private void CreateTreeTableControllerUI(Widget parent)
		{
			this.treeTableController = new NavigationTreeTableController ();

			var frame = new FrameBox
			{
				Parent = parent,
				Dock   = DockStyle.Fill,
			};

			this.treeTableController.CreateUI (frame, footerHeight: 0);
			this.CreateNodeFiller ();
			this.UpdateController ();

			//	Pour que le calcul du nombre de lignes visibles soit correct.
			parent.Window.ForceLayout ();

			//	Connexion des événements.
			this.treeTableController.ContentChanged += delegate (object sender, bool crop)
			{
				this.UpdateController (crop);
			};

			this.treeTableController.SortingChanged += delegate
			{
				using (new SaveSelectedGuid (this))
				{
					this.UpdateSorting ();
				}
			};

			this.treeTableController.RowClicked += delegate (object sender, int row, int column)
			{
				this.VisibleSelectedRow = this.treeTableController.TopVisibleRow + row;
			};

			this.treeTableController.RowDoubleClicked += delegate (object sender, int row)
			{
				this.VisibleSelectedRow = this.treeTableController.TopVisibleRow + row;
				this.OnRowDoubleClicked (this.VisibleSelectedRow);
			};

			this.treeTableController.TreeButtonClicked += delegate (object sender, int row, NodeType type)
			{
				this.OnCompactOrExpand (this.treeTableController.TopVisibleRow + row);
			};
		}

		private void CreateGraphicControllerUI(Widget parent)
		{
			this.CreateGraphicControllerUI();

			if (this.graphicController == null)
			{
				return;
			}

			this.graphicController.CreateUI (parent);

			this.graphicController.TileClicked += delegate (object sender, Guid guid)
			{
				this.SelectedGuid = guid;
			};

			this.graphicController.TileDoubleClicked += delegate (object sender, Guid guid)
			{
				this.SelectedGuid = guid;
				this.OnRowDoubleClicked (this.VisibleSelectedRow);
			};

			this.graphicController.TileCompactOrExpand += delegate (object sender, Guid guid)
			{
				this.SelectedGuid = guid;
				this.OnCompactOrExpand (this.VisibleSelectedRow);
			};
		}

		protected virtual void CreateGraphicControllerUI()
		{
		}


		protected override void UpdateGraphicMode()
		{
			this.treeTableFrame.Visibility = !this.showGraphic;
			this.graphicFrame.Visibility   =  this.showGraphic;

			this.UpdateToolbar ();
		}


		private void UpdateSorting()
		{
			//	Met à jour les instructions de tri des getters en fonction des choix
			//	effectués dans le TreeTable.
			this.sortingInstructions = TreeTableFiller<T>.GetSortingInstructions (this.treeTableController, this.dataFiller);
			this.UpdateData ();
		}


		protected override void UpdateController(bool crop = true)
		{
			if (this.dataFiller != null)
			{
				TreeTableFiller<T>.FillContent (this.treeTableController, this.dataFiller, this.VisibleSelectedRow, crop);
			}

			if (this.graphicController != null)
			{
				this.graphicController.UpdateController (this.nodeGetter, this.SelectedGuid, crop);
			}
		}



		protected FrameBox						treeTableFrame;
		protected FrameBox						graphicFrame;
		protected AbstractTreeTableFiller<T>	dataFiller;
		protected NavigationTreeTableController	treeTableController;
		protected AbstractTreeGraphicController<T> graphicController;
	}
}
