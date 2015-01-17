//	Copyright � 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
		public static Date? Max(Date? date1, Date? date2)
		{
			if (date1.HasValue && date2.HasValue)
			{
				if (date1.Value > date2.Value)
				{
					return date1;
				}
				else
				{
					return date2;
				}
			}
			else if (date1.HasValue)
			{
				return date1;
			}
			else if (date2.HasValue)
			{
				return date2;
			}
			else
			{
				return null;
			}
		}

		public static Date? Min(Date? date1, Date? date2)
		{
			if (date1.HasValue && date2.HasValue)
			{
				if (date1.Value < date2.Value)
				{
					return date1;
				}
				else
				{
					return date2;
				}
			}
			else if (date1.HasValue)
			{
				return date1;
			}
			else if (date2.HasValue)
			{
				return date2;
			}
			else
			{
				return null;
			}
		}


		public static int GetWeekNumber(Date date)
		{
			//	Retourne le num�ro de la semaine, selon la d�finition suivante:
			//	La semaine qui porte le num�ro 01 est celle qui contient le premier jeudi de janvier (c.f. wikipedia).
			//	Le num�ro retourn� est compris entre 0 et 53.
			var nouvelAn = new Date (date.Year, 1, 1);
			int n = 1 - (int) nouvelAn.DayOfWeek;

			if (n < -3)
			{
				n += 7;  // 1..3
			}

			var lundiSemaineUn = Dates.AddDays (nouvelAn, n);
			return Dates.NumberOfDays (date, lundiSemaineUn)/7 + 1;
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
					return Dates.GetMonthShortDescription (date1.Value) + date1.Value.Year.ToString () + " � ...";
				}
				else
				{
					return Converters.DateToString (date1) + " � ...";
				}
			}
			else if (date2.HasValue)
			{
				if (date2.Value.Day == 1)
				{
					return "... �" + Dates.GetMonthShortDescription (date2.Value) + date2.Value.Year.ToString ();
				}
				else
				{
					return "... �" + Converters.DateToString (date2);
				}
			}
			else
			{
				return FormattedText.Empty;
			}
		}

		public static FormattedText GetDescription(Date date1, Date date2)
		{
			//	Retourne un r�sum� court d'une p�riode.
			//	Par exemple:
			//	2012							-> une ann�e enti�re
			//	2012 � 2013						-> deux ann�es enti�res
			//	Mars 2012						-> un mois entier
			//	Janv. � Mars 2012				-> quelques mois entiers
			//	10.01.2012 � 25.04.2012			-> une p�riode quelconque
			//	25.07.2011 � 31.07.2011 (30)	-> une semaine enti�re
			//	09.05.2012 (me)					-> un jour entier
			FormattedText title;

			if (date1 == date2)
			{
				title = Converters.DateToString (date1) + " (" + Dates.GetDayOfWeekShortDescription (date1) + ")";
			}
			else if (Dates.NumberOfDays (date2, date1) == 7-1   &&
					 date1.DayOfWeek == System.DayOfWeek.Monday &&
					 date2.DayOfWeek == System.DayOfWeek.Sunday)  // pile une semaine enti�re ?
			{
				title = Converters.DateToString (date1) + " � " + Converters.DateToString (date2) + " (" + Dates.GetWeekNumber (date1).ToString ("00") + ")";
			}
			else if (date1.Year  == date2.Year &&
					 date1.Day   == 1  &&
					 date1.Month == 1  &&
					 date2.Day   == 31 &&
					 date2.Month == 12)  // pile une ann�e enti�re ?
			{
				title = date1.Year.ToString ();
			}
			else if (date1.Day   == 1  &&
					 date1.Month == 1  &&
					 date2.Day   == 31 &&
					 date2.Month == 12)  // pile plusieurs ann�es enti�res ?
			{
				title = date1.Year.ToString () + " � " + date2.Year.ToString ();
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
				title = Dates.GetMonthShortDescription (date1) + " � " + Dates.GetMonthShortDescription (date2) + " " + date1.Year.ToString ();
			}
			else
			{
				title = Converters.DateToString (date1) + " � " + Converters.DateToString (date2);
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
				return string.Concat (m1, "�", m2);
			}
		}

		public static string GetMonthShortDescription(Date date)
		{
			string[] months =
			{
				"Janv.",
				"F�v.",
				"Mars",
				"Avril",
				"Mai",
				"Juin",
				"Juil.",
				"Ao�t",
				"Sept.",
				"Oct.",
				"Nov.",
				"D�c."
			};

			return months[date.Month-1];
		}

		public static string GetMonthDescription(Date date)
		{
			string[] months =
			{
				"Janvier",
				"F�vrier",
				"Mars",
				"Avril",
				"Mai",
				"Juin",
				"Juillet",
				"Ao�t",
				"Septembre",
				"Octobre",
				"Novembre",
				"D�cembre"
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
