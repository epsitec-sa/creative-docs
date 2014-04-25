//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class ReportsViewState : AbstractViewState
	{
		public ReportType						ReportType;
		public AbstractParams					ReportParams;


		public override bool ApproximatelyEquals(AbstractViewState other)
		{
			var o = other as ReportsViewState;
			if (o == null)
			{
				return false;
			}

			return this.ViewType   == o.ViewType
				&& this.ReportType == o.ReportType;
		}

		public override bool StrictlyEquals(AbstractViewState other)
		{
			var o = other as ReportsViewState;
			if (o == null)
			{
				return false;
			}

			if (this.ViewType   != o.ViewType  ||
				this.ReportType != o.ReportType)
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
			return ReportPopup.GetReportName (this.ReportType);
		}
	}
}
