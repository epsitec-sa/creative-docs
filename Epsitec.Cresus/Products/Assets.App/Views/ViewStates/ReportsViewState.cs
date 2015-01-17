//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data.Reports;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views.ViewStates
{
	public class ReportsViewState : AbstractViewState
	{
		public AbstractReportParams				ReportParams;


		public override bool StrictlyEquals(AbstractViewState other)
		{
			var o = other as ReportsViewState;
			if (o == null)
			{
				return false;
			}

			if (this.ViewType != o.ViewType)
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
				return this.ReportParams == o.ReportParams;
			}
		}


		protected override string GetDescription(DataAccessor accessor)
		{
			return ReportParamsHelper.GetTitle (accessor, this.ReportParams, ReportTitleType.Full);
		}
	}
}
