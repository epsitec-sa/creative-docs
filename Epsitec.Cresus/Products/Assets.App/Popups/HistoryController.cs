//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Data;

namespace Epsitec.Cresus.Assets.App.Popups
{
	public class HistoryController
	{
		public HistoryController(HistoryAccessor accessor)
		{
			this.accessor = accessor;
		}


		public Size GetSize(Widget parent)
		{
			//	On calcule une hauteur adaptée au contenu, mais qui ne dépasse
			//	évidement pas la hauteur de la fenêtre principale.
			double h = parent.ActualHeight
					 - HistoryController.headerHeight
					 - AbstractScroller.DefaultBreadth;

			//	Utilise au maximum les 3/4 de la hauteur.
			int max = (int) (h*0.75) / HistoryController.rowHeight;

			int rows = System.Math.Min (this.accessor.RowsCount, max);
			rows = System.Math.Max (rows, 3);

			int dx = this.accessor.ColumnsWidth
				   + (int) AbstractScroller.DefaultBreadth;

			int dy = AbstractPopup.titleHeight
				   + HistoryController.headerHeight
				   + rows * HistoryController.rowHeight
				   + (int) AbstractScroller.DefaultBreadth;

			return new Size (dx, dy);
		}


		public void CreateUI(Widget parent)
		{
			if (this.accessor.RowsCount == 0)
			{
				new StaticText
				{
					Parent           = parent,
					Text             = "Ce champ n'est jamais défini.",
					ContentAlignment = ContentAlignment.MiddleCenter,
					Dock             = DockStyle.Fill,
				};
			}
			else
			{
				var treeTable = new SimpleTreeTableController (this.accessor.Filler);

				treeTable.CreateUI (parent, rowHeight: HistoryController.rowHeight, headerHeight: HistoryController.headerHeight, footerHeight: 0, treeTableName: "Popup.History");
				treeTable.AllowsMovement = false;

				//	Il faut forcer le calcul du layout pour pouvoir calculer la
				//	première ligne visible dans le TreeTable.
				parent.Window.ForceLayout ();

				treeTable.SelectedRow = this.accessor.SelectedRow;

				treeTable.RowClicked += delegate (object sender, int row)
				{
					var timestamp = this.accessor.GetTimestamp (row);
					if (timestamp.HasValue)
					{
						this.OnNavigate (timestamp.Value);
					}
				};
			}
		}


		#region Events handler
		private void OnNavigate(Timestamp timestamp)
		{
			this.Navigate.Raise (this, timestamp);
		}

		public event EventHandler<Timestamp> Navigate;
		#endregion


		private const int headerHeight     = 22;
		private const int rowHeight        = 18;

		private readonly HistoryAccessor accessor;
	}
}
