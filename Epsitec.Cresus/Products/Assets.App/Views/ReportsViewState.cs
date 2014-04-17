//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class ReportsViewState : AbstractViewState
	{
		public string							SelectedReportId;
		public AbstractParams					ReportParams;


		public override bool AreApproximatelyEquals(AbstractViewState other)
		{
			var o = other as ReportsViewState;
			if (o == null)
			{
				return false;
			}

			return this.ViewType         == o.ViewType
				&& this.SelectedReportId == o.SelectedReportId;
		}

		public override bool AreStrictlyEquals(AbstractViewState other)
		{
			var o = other as ReportsViewState;
			if (o == null)
			{
				return false;
			}

			if (this.ViewType         != o.ViewType        ||
				this.SelectedReportId != o.SelectedReportId)
			{
				return false;
			}

			if (this.ReportParams == null && o.ReportParams == null)
			{
				return true;
			}
			else if (this.ReportParams == null || o.ReportParams == null)
			{
				return false;
			}
			else
			{
				return this.ReportParams.AreStrictlyEquals (o.ReportParams);
			}
		}


		protected override string GetDescription(DataAccessor accessor)
		{
			if (!string.IsNullOrEmpty (this.SelectedReportId))
			{
				return ReportsView.GetReportName (this.SelectedReportId);
			}

			return null;
		}
	}
}
