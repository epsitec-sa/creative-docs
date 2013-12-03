//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.App.Helpers
{
	/// <summary>
	/// Diverses méthodes d'extension autour de System.DateTime.
	/// Voir http://msdn.microsoft.com/en-us/library/8kb3ddd4.aspx
	/// </summary>
	public static class DateTime
	{
		public static string ToFull(this System.DateTime? date, bool weekOfYear = false)
		{
			//	Retourne une description complète sous la forme "lundi 2 décembre 2013".
			if (date.HasValue)
			{
				return date.Value.ToFull (weekOfYear);
			}
			else
			{
				return null;
			}
		}

		public static string ToFull(this System.DateTime date, bool weekOfYear = false)
		{
			//	Retourne une description complète sous la forme "lundi 2 décembre 2013"
			//	ou "lundi 2 décembre 2013 (sem. 49)"
			var text = date.ToString ("dddd d MMMM yyyy");

			if (weekOfYear)
			{
				text += string.Format (" (sem. {0})", date.ToWeekOfYear ());
			}

			return text;
		}

		public static string ToYear(this System.DateTime date, int detailLevel = 1)
		{
			//	Retourne l'année sous une forme plus ou moins détaillée.
			//	detailLevel = 1 retourne "2014"
			//	detailLevel = 0 retourne "14"
			switch (detailLevel)
			{
				case 1:
					return date.ToString ("yyyy", System.Globalization.DateTimeFormatInfo.CurrentInfo);

				case 0:
					return date.ToString ("yy", System.Globalization.DateTimeFormatInfo.CurrentInfo);

				default:
					return null;
			}
		}

		public static string ToMonthYear(this System.DateTime date, int detailLevel = 4)
		{
			//	Retourne le mois sous une forme plus ou moins détaillée.
			//	detailLevel = 4 retourne "Septembre 2013"
			//	detailLevel = 3 retourne "Sept. 2013"
			//	detailLevel = 2 retourne "Septembre"
			//	detailLevel = 1 retourne "Sept."
			//	detailLevel = 0 retourne "9"
			switch (detailLevel)
			{
				case 4:
					return date.ToString ("MMMM yyyy", System.Globalization.DateTimeFormatInfo.CurrentInfo);

				case 3:
					return date.ToString ("MMM yyyy", System.Globalization.DateTimeFormatInfo.CurrentInfo);

				case 2:
					return date.ToString ("MMMM", System.Globalization.DateTimeFormatInfo.CurrentInfo);

				case 1:
					return date.ToString ("MMM", System.Globalization.DateTimeFormatInfo.CurrentInfo);

				case 0:
					return date.Month.ToString (System.Globalization.DateTimeFormatInfo.CurrentInfo);

				default:
					return null;
			}
		}

		public static string ToDayOfWeek(this System.DateTime date)
		{
			//	Retourne le jour sous la forme "lu" ou "ma".
			var text = date.ToString ("ddd", System.Globalization.DateTimeFormatInfo.CurrentInfo);

			if (text.Length > 2)
			{
				return text.Substring (0, 2);  // seulement les 2 premières lettres
			}
			else
			{
				return text;
			}
		}

		public static string ToDay(this System.DateTime date)
		{
			//	Retourne le jour sous la forme "1" ou "31".
			return date.Day.ToString (System.Globalization.DateTimeFormatInfo.CurrentInfo);
		}

		public static string ToDayMonth(this System.DateTime date)
		{
			//	Retourne le jour et le mois.
			//	Par exemple "28.03", "29.03", "30.03", "31.03".
			return date.ToString ("dd.MM", System.Globalization.DateTimeFormatInfo.CurrentInfo);
		}

		public static string ToWeekOfYear(this System.DateTime date)
		{
			//	Retourne le numéro de la semaine sous la forme "1" ou "52".
			return date.GetWeekOfYear ().ToString (System.Globalization.DateTimeFormatInfo.CurrentInfo);
		}


		public static int GetWeekOfYear(this System.DateTime date)
		{
			var ci = new System.Globalization.CultureInfo ("fr-CH");  // French (Switzerland)
			return ci.Calendar.GetWeekOfYear (date, System.Globalization.CalendarWeekRule.FirstFourDayWeek, System.DayOfWeek.Monday);
		}
	}
}
