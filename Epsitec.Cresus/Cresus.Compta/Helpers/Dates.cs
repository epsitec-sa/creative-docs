//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Search.Data;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Helpers
{
	public static class Dates
	{
		public static int GetWeekNumber(Date date)
		{
			//	Retourne le numéro de la semaine, selon la définition suivante:
			//	La semaine qui porte le numéro 01 est celle qui contient le premier jeudi de janvier (c.f. wikipedia).
			//	Le numéro retourné est compris entre 0 et 53.
			var nouvelAn = new Date (date.Year, 1, 1);
			int n = 1 - (int) nouvelAn.DayOfWeek;

			if (n < -3)
			{
				n += 7;  // 1..3
			}

			var lundiSemaineUn = Dates.AddDays (nouvelAn, n);
			return Dates.NumberOfDays (date, lundiSemaineUn)/7 + 1;
		}


		public static int GetDescriptionBestFitWidth(TemporalDataDuration duration)
		{
			switch (duration)
			{
				case TemporalDataDuration.Daily:
					return 88;  // 09.05.2012 (me)

				case TemporalDataDuration.Weekly:
					return 163;  // 25.07.2011 — 31.07.2011 (30)

				case TemporalDataDuration.Monthly:
					return 64;  // Mars 2012

				case TemporalDataDuration.Quarterly:
					return 110;  // Janv. — Mars 2012

				case TemporalDataDuration.Biannual:
					return 110;  // Janv. — Mars 2012

				case TemporalDataDuration.Annual:
					return 38;  // 2012

				default:
					return 100;
			}
		}

		public static FormattedText GetDescription(Date? date1, Date? date2)
		{
			if (date1.HasValue && date2.HasValue)
			{
				return Dates.GetDescription (date1.Value, date2.Value);
			}
			else if (date1.HasValue)
			{
				if (date1.Value.Day == 1)
				{
					return Dates.GetMonthShortDescription (date1.Value) + date1.Value.Year.ToString () + " — ...";
				}
				else
				{
					return Converters.DateToString (date1) + " — ...";
				}
			}
			else if (date2.HasValue)
			{
				if (date2.Value.Day == 1)
				{
					return "... —" + Dates.GetMonthShortDescription (date2.Value) + date2.Value.Year.ToString ();
				}
				else
				{
					return "... —" + Converters.DateToString (date2);
				}
			}
			else
			{
				return FormattedText.Empty;
			}
		}

		public static FormattedText GetDescription(Date date1, Date date2)
		{
			//	Retourne un résumé court d'une période.
			//	Par exemple:
			//	2012							-> une année entière
			//	2012 — 2013						-> deux années entières
			//	Mars 2012						-> un mois entier
			//	Janv. — Mars 2012				-> quelques mois entiers
			//	10.01.2012 — 25.04.2012			-> une période quelconque
			//	25.07.2011 — 31.07.2011 (30)	-> une semaine entière
			//	09.05.2012 (me)					-> un jour entier
			FormattedText title;

			if (date1 == date2)
			{
				title = Converters.DateToString (date1) + " (" + Dates.GetDayOfWeekShortDescription (date1) + ")";
			}
			else if (Dates.NumberOfDays (date2, date1) == 7-1   &&
					 date1.DayOfWeek == System.DayOfWeek.Monday &&
					 date2.DayOfWeek == System.DayOfWeek.Sunday)  // pile une semaine entière ?
			{
				title = Converters.DateToString (date1) + " — " + Converters.DateToString (date2) + " (" + Dates.GetWeekNumber (date1).ToString ("00") + ")";
			}
			else if (date1.Year  == date2.Year &&
					 date1.Day   == 1  &&
					 date1.Month == 1  &&
					 date2.Day   == 31 &&
					 date2.Month == 12)  // pile une année entière ?
			{
				title = date1.Year.ToString ();
			}
			else if (date1.Day   == 1  &&
					 date1.Month == 1  &&
					 date2.Day   == 31 &&
					 date2.Month == 12)  // pile plusieurs années entières ?
			{
				title = date1.Year.ToString () + " — " + date2.Year.ToString ();
			}
			else if (date1.Year  == date2.Year &&
					 date1.Month == date2.Month &&
					 date1.Day   == 1  &&
					 Dates.IsLastDayOfMonth (date2))  // pile un mois entier ?
			{
				title = Dates.GetMonthShortDescription (date1) + " " + date1.Year.ToString ();
			}
			else if (date1.Year  == date2.Year &&
					 date1.Day   == 1  &&
					 Dates.IsLastDayOfMonth (date2))  // pile quelques mois entiers ?
			{
				title = Dates.GetMonthShortDescription (date1) + " — " + Dates.GetMonthShortDescription (date2) + " " + date1.Year.ToString ();
			}
			else
			{
				title = Converters.DateToString (date1) + " — " + Converters.DateToString (date2);
			}

			return title;
		}


		public static string GetDayOfWeekShortDescription(Date date)
		{
			string[] dow =
			{
				"di",
				"lu",
				"ma",
				"me",
				"je",
				"ve",
				"sa",
			};

			return dow[(int) date.DayOfWeek];
		}

		public static string GetDayOfWeekDescription(Date date)
		{
			string[] dow =
			{
				"dimanche",
				"lundi",
				"mardi",
				"mercredi",
				"jeudi",
				"vendredi",
				"samedi",
			};

			return dow[(int) date.DayOfWeek];
		}


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

			while (month <= 0)
			{
				year--;
				month += 12;
			}

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
