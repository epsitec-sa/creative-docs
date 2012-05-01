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
		public static string GetMonthShortDescription(Date date1, Date date2)
		{
			if (date1.Month == date2.Month)
			{
				return Dates.GetMonthShortDescription (date1);
			}
			else
			{
				var m1 = Dates.GetMonthShortDescription (date1);
				var m2 = Dates.GetMonthShortDescription (date2);
				return string.Concat (m1, "—", m2);
			}
		}

		public static string GetMonthShortDescription(Date date)
		{
			string[] months =
			{
				"Janv.",
				"Fév.",
				"Mars",
				"Avril",
				"Mai",
				"Juin",
				"Juil.",
				"Août",
				"Sept.",
				"Oct.",
				"Nov.",
				"Déc."
			};

			return months[date.Month-1];
		}

		public static string GetMonthDescription(Date date)
		{
			string[] months =
			{
				"Janvier",
				"Février",
				"Mars",
				"Avril",
				"Mai",
				"Juin",
				"Juillet",
				"Août",
				"Septembre",
				"Octobre",
				"Novembre",
				"Décembre"
			};

			return months[date.Month-1];
		}


		public static int NumberOfDays(Date d1, Date d2)
		{
			return (int) (d1.Ticks/Time.TicksPerDay - d2.Ticks/Time.TicksPerDay);
		}

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
