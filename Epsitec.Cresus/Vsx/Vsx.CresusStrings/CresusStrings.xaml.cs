using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace Epsitec.Cresus.Strings
{
	public partial class CresusStrings
	{
		public CresusStrings()
		{
			this.InitializeComponent ();
		}

		private void ListView_TargetUpdated(object sender, DataTransferEventArgs e)
		{
			var listView = sender as ListView;
			var gridView = listView.View as GridView;
			CresusStrings.AutoResizeGridViewColumns (gridView);
		}

		static void AutoResizeGridViewColumns(GridView view)
		{
			if (view == null || view.Columns.Count < 1)
				return;
			// Simulates column auto sizing
			foreach (var column in view.Columns)
			{
				// Forcing change
				if (double.IsNaN (column.Width))
					column.Width = 1;
				column.Width = double.NaN;
			}
		}
	}
}
