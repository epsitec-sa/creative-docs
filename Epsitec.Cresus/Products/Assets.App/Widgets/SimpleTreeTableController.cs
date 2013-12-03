//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.Server.DataFillers;
using Epsitec.Cresus.Assets.Server.NodesGetter;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	/// <summary>
	/// Ce contrôleur pilote un NavigationTreeTableController d'après un filler.
	/// </summary>
	public class SimpleTreeTableController
	{
		public SimpleTreeTableController(AbstractTreeTableFiller<GuidNode> dataFiller)
		{
			this.dataFiller = dataFiller;

			this.controller = new NavigationTreeTableController ();

			this.visibleSelectedRow = -1;
		}


		public void CreateUI(Widget parent, int rowHeight = 18, int headerHeight = 24, int footerHeight = 24)
		{
			this.controller.CreateUI (parent, rowHeight, headerHeight, footerHeight);
			TreeTableFiller<GuidNode>.FillColumns (dataFiller, this.controller);

			this.controller.RowClicked += delegate (object sender, int row)
			{
				this.visibleSelectedRow = this.controller.TopVisibleRow + row;
				this.UpdateTreeTableController ();
				this.OnRowClicked (this.visibleSelectedRow);
			};

			this.controller.ContentChanged += delegate (object sender, bool crop)
			{
				this.UpdateTreeTableController (crop);
			};
		}


		public bool AllowsMovement
		{
			get
			{
				return this.controller.AllowsMovement;
			}
			set
			{
				this.controller.AllowsMovement = value;
			}
		}

		public int SelectedRow
		{
			get
			{
				return this.visibleSelectedRow;
			}
			set
			{
				if (this.visibleSelectedRow != value)
				{
					this.visibleSelectedRow = value;
					this.UpdateTreeTableController ();
				}
			}
		}

		private void UpdateTreeTableController(bool crop = true)
		{
			TreeTableFiller<GuidNode>.FillContent (this.dataFiller, this.controller, this.visibleSelectedRow, crop);
		}


		#region Events handler
		private void OnRowClicked(int row)
		{
			this.RowClicked.Raise (this, row);
		}

		public event EventHandler<int> RowClicked;
		#endregion


		private readonly AbstractTreeTableFiller<GuidNode>	dataFiller;
		private readonly NavigationTreeTableController		controller;

		private int											visibleSelectedRow;
	}
}