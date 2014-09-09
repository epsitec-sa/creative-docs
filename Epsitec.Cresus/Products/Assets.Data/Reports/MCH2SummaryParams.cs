//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Data.Reports
{
	public class MCH2SummaryParams : AbstractReportParams, System.IEquatable<MCH2SummaryParams>
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
			return a.Equals (b);
		}

		public static bool operator !=(MCH2SummaryParams a, MCH2SummaryParams b)
		{
			return !a.Equals (b);
		}

		public override bool Equals(object obj)
		{
			if (obj is MCH2SummaryParams)
			{
				return this.Equals ((MCH2SummaryParams) obj);
			}
			else
			{
				return false;
			}
		}

		public bool Equals(MCH2SummaryParams other)
		{
			//	Il ne faut surtout pas comparer les Guid !
			if (!(other is MCH2SummaryParams))
			{
				return false;
			}

			return this.CustomTitle == other.CustomTitle
				&& this.DateRange   == other.DateRange
				&& this.RootGuid    == other.RootGuid
				&& this.Level       == other.Level
				&& this.FilterGuid  == other.FilterGuid;
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


		public readonly DateRange				DateRange;
		public readonly Guid					RootGuid;
		public readonly int?					Level;
		public readonly Guid					FilterGuid;
	}
}
