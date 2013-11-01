﻿//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

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
					 - HistoryController.HeaderHeight
					 - AbstractScroller.DefaultBreadth;

			//	Utilise au maximum les 3/4 de la hauteur.
			int max = (int) (h*0.75) / HistoryController.RowHeight;

			int rows = System.Math.Min (this.accessor.RowsCount, max);
			rows = System.Math.Max (rows, 3);

			int dx = this.accessor.ColumnsWidth
				   + (int) AbstractScroller.DefaultBreadth;

			int dy = HistoryController.TitleHeight
				   + HistoryController.HeaderHeight
				   + rows * HistoryController.RowHeight
				   + (int) AbstractScroller.DefaultBreadth;

			return new Size (dx, dy);
		}


		public void CreateUI(Widget parent)
		{
			this.CreateTitle (parent);

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
				var treeTable = new SimpleTreeTableController ();

				treeTable.CreateUI (parent, rowHeight: HistoryController.RowHeight, headerHeight: HistoryController.HeaderHeight, footerHeight: 0);
				treeTable.AllowsMovement = false;
				treeTable.SetColumns (this.accessor.Columns);
				treeTable.SetContent (this.accessor.Content);

				//	Il faut forcer le calcul du layout pour pouvoir calculer la
				//	première ligne visible dans le TreeTable.
				parent.Window.ForceLayout ();

				treeTable.SelectedRow = this.accessor.SelectedRow;
				treeTable.ShowSelection ();

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

		private void CreateTitle(Widget parent)
		{
			new StaticText
			{
				Parent           = parent,
				Text             = "Historique",
				ContentAlignment = ContentAlignment.MiddleCenter,
				Dock             = DockStyle.Top,
				PreferredHeight  = HistoryController.TitleHeight - 4,
				BackColor        = ColorManager.SelectionColor,
			};

			new StaticText
			{
				Parent           = parent,
				Dock             = DockStyle.Top,
				PreferredHeight  = 4,
				BackColor        = ColorManager.SelectionColor,
			};
		}


		#region Events handler
		private void OnNavigate(Timestamp timestamp)
		{
			if (this.Navigate != null)
			{
				this.Navigate (this, timestamp);
			}
		}

		public delegate void NavigateEventHandler(object sender, Timestamp timestamp);
		public event NavigateEventHandler Navigate;
		#endregion


		private static readonly int TitleHeight      = 24;
		private static readonly int HeaderHeight     = 22;
		private static readonly int RowHeight        = 18;

		private readonly HistoryAccessor accessor;
	}
}
