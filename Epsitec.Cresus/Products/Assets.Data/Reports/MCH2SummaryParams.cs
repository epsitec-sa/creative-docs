//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Data.Reports
{
	public class MCH2SummaryParams : AbstractReportParams, System.IEquatable<AbstractReportParams>
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


		public static bool operator ==(MCH2SummaryParams a, MCH2SummaryParams b)
		{
			if (!(a is MCH2SummaryParams) || !(b is MCH2SummaryParams))
			{
				return false;
			}

			return a.Equals (b);
		}

		public static bool operator !=(MCH2SummaryParams a, MCH2SummaryParams b)
		{
			if (!(a is MCH2SummaryParams) || !(b is MCH2SummaryParams))
			{
				return true;
			}

			return !a.Equals (b);
		}

		public override bool Equals(AbstractReportParams other)
		{
			//	Il ne faut surtout pas comparer les Guid !
			if (!(other is MCH2SummaryParams))
			{
				return false;
			}

			var o = other as MCH2SummaryParams;

			return this.CustomTitle == o.CustomTitle
				&& this.DateRange   == o.DateRange
				&& this.RootGuid    == o.RootGuid
				&& this.Level       == o.Level
				&& this.FilterGuid  == o.FilterGuid;
		}

		public override int GetHashCode()
		{
			return this.CustomTitle.GetHashCode ()
				^  this.DateRange.GetHashCode ()
				^  this.RootGuid.GetHashCode ()
				^  this.Level.GetHashCode ()
				^  this.FilterGuid.GetHashCode ();
		}


		public override AbstractReportParams ChangePeriod(int direction)
		{
			return new MCH2SummaryParams (this.CustomTitle, this.DateRange.ChangePeriod (direction), this.RootGuid, this.Level, this.FilterGuid);
		}

		public override AbstractReportParams ChangeCustomTitle(string customTitle)
		{
			return new MCH2SummaryParams (customTitle, this.DateRange, this.RootGuid, this.Level, this.FilterGuid);
		}


		public readonly DateRange				DateRange;
		public readonly Guid					RootGuid;
		public readonly int?					Level;
		public readonly Guid					FilterGuid;
	}
}
