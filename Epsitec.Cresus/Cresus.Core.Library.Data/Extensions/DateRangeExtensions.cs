//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Extensions
{
	public static class DateRangeExtensions
	{
		public static bool InRange(this System.DateTime date, IDateTimeRange range)
		{
			DateRangeExtensions.Check (date, range.BeginDate);
			DateRangeExtensions.Check (date, range.EndDate);
			DateRangeExtensions.Check (range.BeginDate, range.EndDate);

			return date.InRange (range.BeginDate, range.EndDate);
		}

		public static bool InRange(this System.DateTime dateTime, IDateRange range)
		{
			Date date = new Date (dateTime);
			return date.InRange (range.BeginDate, range.EndDate);
		}

		public static bool Overlaps(this IDateRange range, IDateRange other)
		{
			if (range.EndDate.HasValue &&
				other.BeginDate.HasValue &&
				range.EndDate.Value < other.BeginDate.Value)
			{
				//	The range ends before the other one.
				return false;
			}

			if (range.BeginDate.HasValue &&
				other.EndDate.HasValue &&
				range.BeginDate.Value > other.EndDate.Value)
			{
				//	The range begins after the other one.
				return false;
			}

			return true;
		}

		public static IDateRange GetIntersection(this IDateRange range, IDateRange other)
		{
			Date? beginDate;
			Date? endDate;

			if (range.BeginDate.HasValue &&
				other.BeginDate.HasValue)
			{
				beginDate = range.BeginDate.Value < other.BeginDate.Value ? other.BeginDate : range.BeginDate;
			}
			else
			{
				beginDate = range.BeginDate ?? other.BeginDate;
			}

			if (range.EndDate.HasValue &&
				other.EndDate.HasValue)
			{
				endDate = range.EndDate.Value > other.EndDate.Value ? other.EndDate : range.EndDate;
			}
			else
			{
				endDate = range.EndDate ?? other.EndDate;
			}

			return new DateRange
			{
				BeginDate = beginDate,
				EndDate = endDate
			};
		}

		public static int GetDuration(this IDateRange range)
		{
			if (range.BeginDate.HasValue &&
				range.EndDate.HasValue)
			{
				return range.EndDate.Value - range.BeginDate.Value + 1;
			}

			throw new System.ArgumentException ("Infinite date range; cannot compute duration");
		}


		private static void Check(System.DateTime? dt1, System.DateTime? dt2)
		{
			if (dt1.HasValue &&
				dt2.HasValue &&
				dt1.Value.Kind != dt2.Value.Kind)
			{
				throw new System.ArgumentException ("DateTime are different kinds");
			}
		}


		private class DateRange : IDateRange
		{
			#region IDateRange Members

			public Date? BeginDate
			{
				get;
				set;
			}

			public Date? EndDate
			{
				get;
				set;
			}

			#endregion
		}
	}
}
