//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Reports;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class ReportChoiceController
	{
		public ReportChoiceController(DataAccessor accessor)
		{
			this.accessor = accessor;
		}


		public void ClearSelection()
		{
			this.list.SelectedItemIndex = -1;
		}

		public void CreateUI(Widget parent)
		{
			var frame = new FrameBox
			{
				Parent         = parent,
				Dock           = DockStyle.Fill,
				Margins        = new Margins (10),
			};

			var label = "Choisissez un rapport";

			new StaticText
			{
				Parent           = frame,
				Text             = label,
				PreferredWidth   = label.GetTextWidth () + 10,
				ContentAlignment = ContentAlignment.TopLeft,
				Dock             = DockStyle.Left,
				Margins          = new Margins (0, 0, 5, 0),
			};

			this.list = new ScrollList
			{
				Parent         = frame,
				PreferredWidth = 300,
				Dock           = DockStyle.Left,
				RowHeight      = 20,    
			};

			this.UpdateList ();

			this.list.SelectedItemChanged += delegate
			{
				var array = ReportsList.ReportTypes.ToArray ();
				int sel = this.list.SelectedItemIndex;
				if (sel >= 0 && sel < array.Length)
				{
					this.OnReportSelected (array[sel]);
				}
			};
		}


		private void UpdateList()
		{
			this.list.Items.Clear ();

			foreach (var type in ReportsList.ReportTypes)
			{
				var name = "  " + ReportsList.GetReportName (type);
				this.list.Items.Add (name);
			}
		}


		#region Events handler
		protected void OnReportSelected(ReportType reportType)
		{
			this.ReportSelected.Raise (this, reportType);
		}

		public event EventHandler<ReportType> ReportSelected;
		#endregion


		private readonly DataAccessor			accessor;
		private ScrollList						list;
	}
}
