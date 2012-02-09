using Epsitec.Aider.Enumerations;

using Epsitec.Common.Support.Extensions;

using System;

using System.Collections.Generic;


using System.Threading;


namespace Epsitec.Aider.Entities
{


	public partial class eCH_PersonEntity
	{


		partial void GetDateOfBirth(ref string value)
		{
			var elements = new List<string> ();

			switch (this.PersonDateOfBirthType)
			{
				case DatePrecision.YearMonthDay:
					elements.Add (this.PersonDateOfBirthDay.Value.ToString ("00"));
					goto case DatePrecision.YearMonth;

				case DatePrecision.YearMonth:
					elements.Add (this.PersonDateOfBirthMonth.Value.ToString ("00"));
					goto case DatePrecision.Year;

				case DatePrecision.Year:
					elements.Add (this.PersonDateOfBirthYear.Value.ToString ("0000"));
					break;

				case DatePrecision.None:
					break;

				default:
					throw new NotImplementedException ();
			}

			var separator = eCH_PersonEntity.GetDateSeparator ();

			value = string.Join (separator.ToString (), elements);
		}


		partial void SetDateOfBirth(string value)
		{
			var separator = eCH_PersonEntity.GetDateSeparator ();

			var elements = value.Split (new string[] { separator }, StringSplitOptions.None);
			var nbSeparator = value.CountOccurences (separator);

			if ((nbSeparator != 0 || elements.Length != 0) && nbSeparator != elements.Length - 1)
			{
				// We don't have the proper number of separators compared with the number of elements
				// ie. we have the string "1..1"

				throw new FormatException ();
			}

			string dayAsString = null;
			string monthAsString = null;
			string yearAsString = null;
			DatePrecision datePrecision;

			switch (elements.Length)
			{
				case 0:
					datePrecision = DatePrecision.None;
					break;

				case 1:
					yearAsString = elements[0];
					datePrecision = DatePrecision.Year;
					break;

				case 2:
					monthAsString = elements[0];
					yearAsString = elements[1];
					datePrecision = DatePrecision.YearMonth;
					break;

				case 3:
					dayAsString = elements[0];
					monthAsString = elements[1];
					yearAsString = elements[2];
					datePrecision = DatePrecision.YearMonthDay;
					break;

				default:
					throw new FormatException ();
			}


			int? year = null;
			bool yearIsValid = (yearAsString == null)
                || (eCH_PersonEntity.ParseInt (yearAsString, out year) &&  eCH_PersonEntity.IsYearValid (year.Value));

			int? month = null;
			bool monthIsValid = (monthAsString == null)
                || (eCH_PersonEntity.ParseInt (monthAsString, out month) && eCH_PersonEntity.IsMonthValid (month.Value));

			int? day = null;
			bool dayIsValid = (dayAsString == null)
                || (eCH_PersonEntity.ParseInt (dayAsString, out day) && eCH_PersonEntity.IsDayValid (day.Value) && eCH_PersonEntity.IsDateValid (year.Value, month.Value, day.Value));

			if (!yearIsValid || !monthIsValid || !dayIsValid)
			{
				throw new FormatException ();
			}

			this.PersonDateOfBirthYear = year;
			this.PersonDateOfBirthMonth = month;
			this.PersonDateOfBirthDay = day;
			this.PersonDateOfBirthType = datePrecision;
		}


		private static bool IsYearValid(int year)
		{
			return year >= 1900 && year <= 2100;
		}


		private static bool IsMonthValid(int month)
		{
			return month >= 1 && month <= 12;
		}


		private static bool IsDayValid(int day)
		{
			return day >= 1 && day <= 31;
		}


		private static bool ParseInt(string text, out int? value)
		{
			int result;
			bool success = int.TryParse (text, out result);

			if (success)
			{
				value = result;
			}
			else
			{
				value = null;
			}

			return success;
		}


		private static bool IsDateValid(int year, int month, int day)
		{
			try
			{
				var dateTime = new DateTime (year, month, day);

				return true;
			}
			catch
			{
				return false;
			}
		}


		private static string GetDateSeparator()
		{
			return Thread.CurrentThread.CurrentUICulture.DateTimeFormat.DateSeparator;
		}



		internal static string GetDefaultFirstName(eCH_PersonEntity person)
		{
			string[] names = person.PersonFirstNames.Split (' ');
			return names[0];
		}
	}


}
