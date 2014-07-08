//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Data
{
	public struct DateRange
	{
		public DateRange(System.DateTime includeFrom, System.DateTime excludeTo)
		{
			//	Typiquement, includeFrom = 01.01.2013 et excludeTo = 01.01.2014.
			this.IncludeFrom = includeFrom;
			this.ExcludeTo   = excludeTo;
		}

		public bool IsEmpty
		{
			get
			{
				return this.IncludeFrom == System.DateTime.MaxValue
					&& this.ExcludeTo   == System.DateTime.MinValue;
			}
		}

		public bool AtLeastOneTime
		{
			get
			{
				return this.IncludeFrom < this.ExcludeTo;
			}
		}

		public Timestamp FromTimestamp
		{
			get
			{
				return new Timestamp (this.IncludeFrom, 0);
			}
		}

		public Timestamp ToTimestamp
		{
			get
			{
				return new Timestamp (this.ExcludeTo, 0);
			}
		}

		public bool IsInside(System.DateTime date)
		{
			return date >= this.IncludeFrom.Date
				&& date <  this.ExcludeTo.Date;
		}


		public static bool operator ==(DateRange r1, DateRange r2)
		{
			return r1.IncludeFrom == r2.IncludeFrom
				&& r1.ExcludeTo   == r2.ExcludeTo;
		}

		public static bool operator !=(DateRange r1, DateRange r2)
		{
			return r1.IncludeFrom != r2.IncludeFrom
				|| r1.ExcludeTo   != r2.ExcludeTo;
		}


		public static DateRange Empty = new DateRange (System.DateTime.MaxValue, System.DateTime.MinValue);
		public static DateRange Full  = new DateRange (System.DateTime.MinValue, System.DateTime.MaxValue);

		public readonly System.DateTime			IncludeFrom;
		public readonly System.DateTime			ExcludeTo;
	}
}