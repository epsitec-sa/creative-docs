//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.App.Reports;
using Epsitec.Cresus.Assets.App.Views;

namespace Epsitec.Cresus.Assets.App.Popups
{
	public class ReportPopup : SimplePopup
	{
		public ReportPopup()
		{
			foreach (var type in ReportsList.ReportTypes)
			{
				var name = ReportsList.GetReportName (type);
				this.Items.Add (name);
			}
		}


		public ReportType ReportType
		{
			get
			{
				return ReportsList.GetReportType (this.SelectedItem);
			}
			set
			{
				this.SelectedItem = ReportsList.GetReportIndex (value);
			}
		}
	}
}