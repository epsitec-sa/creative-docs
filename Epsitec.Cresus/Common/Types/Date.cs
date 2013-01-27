//	Copyright © 2003-2013, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>Date</c> structure is a simplified version of <see cref="System.DateTime"/>
	/// where only the date of the day is preserved; the date is always considered to be in
	/// the local time zone (not UTC).
	/// </summary>

	[System.Serializable]
	[System.ComponentModel.TypeConverter (typeof (Date.Converter))]

	public struct Date : System.IComparable, INullable, System.IEquatable<Date>, System.IFormattable
	{
		public Date(System.DateTime dateTime)
		{
			long dateTimeTicks = dateTime.ToLocalTime ().Ticks;

			if (dateTimeTicks == 0)
			{
				this.days = 0;
			}
			else
			{
				this.days = ((int) (dateTimeTicks / Time.TicksPerDay) << 4) | Date.FlagDayDefined | Date.FlagMonthDefined | Date.FlagYearDefined;
			}
		}

		public Date(int year, int month, int day)
		{
			int flags = 0;

			if (year == 0)
			{
				year = 1;
			}
			else
			{
				flags |= Date.FlagYearDefined;
			}
			if (month == 0)
			{
				month = 1;
			}
			else
			{
				flags |= Date.FlagMonthDefined;
			}
			if (day == 0)
			{
				day = 1;
			}
			else
			{
				flags |= Date.FlagDayDefined;
			}

			if (flags == 0)
			{
				this.days = 0;
			}
			else
			{
				var days = (int) (new System.DateTime (year, month, day).Ticks / Time.TicksPerDay);

				this.days = (days << 4) | flags;
			}
		}

		public Date(long ticks)
			: this (new System.DateTime (ticks, System.DateTimeKind.Local))
		{
		}

		private Date(int binary, bool justToMakeSure)
		{
			this.days = binary;
		}


		public int								Day
		{
			get
			{
				return this.HasDay ? this.InternalDate.Day : 0;
			}
		}

		public System.DayOfWeek					DayOfWeek
		{
			get
			{
				return this.InternalDate.DayOfWeek;
			}
		}

		public int								DayOfYear
		{
			get
			{
				return this.InternalDate.DayOfYear;
			}
		}

		public int								Month
		{
			get
			{
				return this.HasMonth ? this.InternalDate.Month : 0;
			}
		}

		public int								Year
		{
			get
			{
				return this.HasYear ? this.InternalDate.Year : 0;
			}
		}

		public long								Ticks
		{
			get
			{
				return this.InternalDays * Time.TicksPerDay;
			}
		}

		public bool								HasDay
		{
			get
			{
				return (this.days & Date.FlagDayDefined) != 0;
			}
		}

		public bool								HasMonth
		{
			get
			{
				return (this.days & Date.FlagMonthDefined) != 0;
			}
		}

		public bool								HasYear
		{
			get
			{
				return (this.days & Date.FlagYearDefined) != 0;
			}
		}


		public static Date						Today
		{
			get
			{
				return new Date (System.DateTime.Today);
			}
		}

		public static readonly Date				Null = new Date (0, true);


		public Date AddDays(int value)
		{
			return new Date (this.InternalDate.AddDays (value));
		}

		public Date AddMonths(int value)
		{
			return new Date (this.InternalDate.AddMonths (value));
		}

		public Date AddYears(int value)
		{
			return new Date (this.InternalDate.AddYears (value));
		}


		public System.DateTime ToDateTime()
		{
			return this.InternalDate;
		}


		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}

			long thisTicks = this.Ticks;
			long thatTicks;

			if (obj is System.DateTime)
			{
				thatTicks = ((System.DateTime) obj).Ticks;
			}
			else if (obj is Date)
			{
				thatTicks = ((Date) obj).Ticks;
			}
			else
			{
				return false;
			}

			return thisTicks == thatTicks;
		}

		public override int GetHashCode()
		{
			return this.days.GetHashCode ();
		}


		public int? ComputeAge()
		{
			return this.ComputeAge (Date.Today);
		}

		public int? ComputeAge(Date today)
		{
			if (this.IsNull)
			{
				return null;
			}

			if ((today.Month >= this.Month) &&
				(today.Day >= this.Day))
			{
				return today.Year - this.Year;
			}
			else
			{
				return today.Year - this.Year - 1;
			}
		}

		public static Date FromObject(object value)
		{
			if (value is System.DateTime)
			{
				return new Date ((System.DateTime) value);
			}
			else if (value is Date)
			{
				return (Date) value;
			}
			else if (value == null)
			{
				return Date.Null;
			}
			else
			{
				throw new System.ArgumentException ("Neither a Date nor a DateTime");
			}
		}

		public static int Compare(Date t1, Date t2)
		{
			if (t1.Ticks > t2.Ticks)
			{
				return 1;
			}

			if (t1.Ticks < t2.Ticks)
			{
				return -1;
			}

			return 0;
		}

		public static bool Equals(Date t1, Date t2)
		{
			return t1.Ticks == t2.Ticks;
		}


		public static bool operator==(Date t1, Date t2)
		{
			return t1.days == t2.days;
		}
		public static bool operator!=(Date t1, Date t2)
		{
			return t1.days != t2.days;
		}
		public static bool operator<(Date t1, Date t2)
		{
			return t1.days < t2.days;
		}
		public static bool operator>(Date t1, Date t2)
		{
			return t1.days > t2.days;
		}
		public static bool operator<=(Date t1, Date t2)
		{
			return t1.days <= t2.days;
		}
		public static bool operator>=(Date t1, Date t2)
		{
			return t1.days >= t2.days;
		}

		public static int operator-(Date t1, Date t2)
		{
			return (t2.ToDateTime () - t1.ToDateTime ()).Days;
		}

		public override string ToString()
		{
			if (this.IsNull)
			{
				return "<null>";
			}
			else
			{
				return this.ToString ("d", System.Globalization.CultureInfo.CurrentCulture);
			}
		}

		#region IFormattable Members

		public string ToString(string format, System.IFormatProvider formatProvider)
		{
			return this.InternalDate.ToString (format, formatProvider);
		}

		#endregion

		#region IEquatable<Date> Members

		public bool Equals(Date other)
		{
			return this.days == other.days;
		}

		#endregion

		#region INullable Members

		public bool IsNull
		{
			get
			{
				return (this.days == 0);
			}
		}

		#endregion

		#region IComparable Members
		public int CompareTo(object obj)
		{
			long thisTicks = this.Ticks;
			long thatTicks = 0;

			if (obj == null)
			{
				return 1;
			}

			if (obj is System.DateTime)
			{
				thatTicks = ((System.DateTime) obj).Ticks;
			}
			else if (obj is Date)
			{
				thatTicks = ((Date) obj).Ticks;
			}
			else
			{
				throw new System.ArgumentException ("Invalid argument");
			}

			if (thisTicks > thatTicks)
			{
				return 1;
			}
			if (thisTicks < thatTicks)
			{
				return -1;
			}

			return 0;
		}
		#endregion

		private int ToBinary()
		{
			return this.days;
		}

		#region Converter Class
		private class Converter : Epsitec.Common.Types.AbstractStringConverter
		{
			public override object ParseString(string value, System.Globalization.CultureInfo culture)
			{
				int binary = System.Int32.Parse (value, System.Globalization.NumberStyles.HexNumber, culture);
				Date date = new Date (binary, true);
				return date;
			}

			public override string ToString(object value, System.Globalization.CultureInfo culture)
			{
				Date date = (Date) value;
				long binary = date.ToBinary ();
				return string.Format (culture, "{0:X6}", binary);
			}
		}
		#endregion

		private System.DateTime					InternalDate
		{
			get
			{
				return new System.DateTime (this.InternalDays * Time.TicksPerDay, System.DateTimeKind.Local);
			}
		}

		private int								InternalDays
		{
			get
			{
				if (this.IsNull)
				{
					return 0;
				}
				else
				{
					return (this.days >> 4) & 0x000fffff;
				}
			}
		}


		private const int						FlagYearDefined  = 0x0004;
		private const int						FlagMonthDefined = 0x0002;
		private const int						FlagDayDefined   = 0x0001;

		private readonly int					days;
	}
}
