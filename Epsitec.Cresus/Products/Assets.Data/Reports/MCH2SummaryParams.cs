//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Data.Reports
{
	public class MCH2SummaryParams : AbstractReportParams
	{
		public MCH2SummaryParams(string customTitle, DateRange dateRange, Guid rootGuid, int? level, Guid filterGuid)
			: base (customTitle)
		{
			this.DateRange  = dateRange;
			this.RootGuid   = rootGuid;
			this.Level      = level;
			this.FilterGuid = filterGuid;
		}


		public override string					Title
		{
			get
			{
				return Res.Strings.Reports.MCH2Summary.DefaultTitle.ToString ();
			}
		}


		public override bool StrictlyEquals(AbstractReportParams other)
		{
			if (other is MCH2SummaryParams)
			{
				var o = other as MCH2SummaryParams;

				return this.CustomTitle == o.CustomTitle
					&& this.DateRange   == o.DateRange
					&& this.RootGuid    == o.RootGuid
					&& this.Level       == o.Level
					&& this.FilterGuid  == o.FilterGuid;
			}

			return false;
		}

		public override AbstractReportParams ChangePeriod(int direction)
		{
			return new MCH2SummaryParams (this.CustomTitle, this.DateRange.ChangePeriod (direction), this.RootGuid, this.Level, this.FilterGuid);
		}


		public readonly DateRange				DateRange;
		public readonly Guid					RootGuid;
		public readonly int?					Level;
		public readonly Guid					FilterGuid;
	}
}
