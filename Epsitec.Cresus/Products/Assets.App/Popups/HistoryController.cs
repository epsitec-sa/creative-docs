//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.NaiveEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	public class HistoryController
	{
		public HistoryController(HistoryAccessor accessor)
		{
			this.accessor = accessor;
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
				Text             = "Historique des modifications",
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


		public static readonly int TitleHeight      = 24;
		public static readonly int HeaderHeight     = 22;
		public static readonly int RowHeight        = 18;
		public static readonly int DateColumnWidth  = 80;
		public static readonly int ValueColumnWidth = 150;

		private readonly HistoryAccessor accessor;
	}
}
