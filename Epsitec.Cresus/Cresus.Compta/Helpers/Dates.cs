//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Controllers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Helpers
{
	public static class Dates
	{
		public static Date AddDays(Date date, int numberOfDays)
		{
			return new Date (date.Ticks + Time.TicksPerDay*numberOfDays);
		}

		public static Date AddMonths(Date date, int numberOfMonths)
		{
			var month = date.Month + numberOfMonths;
			var year = date.Year;

			while (month > 12)
			{
				year++;
				month -= 12;
			}

			return new Date (year, month, 1);
		}


		public static bool DateInRange(Date? date, Date? beginDate, Date? endDate)
		{
			if (date.HasValue)
			{
				if (beginDate.HasValue && date.Value < beginDate.Value)
				{
					return false;
				}

				if (endDate.HasValue && date.Value > endDate.Value)
				{
					return false;
				}
			}

			return true;
		}


		public static bool IsLastDayOfMonth(Date date)
		{
			var next = new Date (date.Ticks + Time.TicksPerDay);
			return next.Day == 1;
		}
	}
}
