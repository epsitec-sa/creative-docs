//	Copyright © 2003-2011, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// La structure Date est une simplification de DateTime. Elle ne
	/// représente que la date du jour, sans aucune indication de
	/// l'heure.
	/// </summary>
	
	[System.Serializable]
	[System.ComponentModel.TypeConverter (typeof (Date.Converter))]
	
	public struct Date : System.IComparable, INullable, System.IEquatable<Date>, System.IFormattable
	{
		public Date(System.DateTime dateTime)
		{
			this.days = (int)(dateTime.Ticks / Time.TicksPerDay);
		}

		public Date(int year, int month, int day)
			: this (new System.DateTime (year, month, day))
		{
		}

		public Date(long ticks)
			: this (new System.DateTime (ticks))
		{
		}

		private Date(int days, bool justToMakeSure)
		{
			this.days = days;
		}
		
		
		public int								Day
		{
			get { return this.InternalDate.Day; }
		}
		
		public System.DayOfWeek					DayOfWeek
		{
			get { return this.InternalDate.DayOfWeek; }
		}

		public int								DayOfYear
		{
			get { return this.InternalDate.DayOfYear; }
		}
		
		public int								Month
		{
			get { return this.InternalDate.Month; }
		}
		
		public int								Year
		{
			get { return this.InternalDate.Year; }
		}
		
		
		public long								Ticks
		{
			get
			{
				return this.days * Time.TicksPerDay;
			}
		}
		
		
		public static Date						Today
		{
			get
			{
				return new Date (System.DateTime.Today);
			}
		}

		public static readonly Date				Null = new Date (-1, true);

		
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
		
		public bool								IsNull
		{
			get
			{
				return (this.days == -1);
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
		
		#region Converter Class
		private class Converter : Epsitec.Common.Types.AbstractStringConverter
		{
			public override object ParseString(string value, System.Globalization.CultureInfo culture)
			{
				int days = System.Int32.Parse (value, culture);
				Date date = new Date (days, true);
				return date;
			}
			
			public override string ToString(object value, System.Globalization.CultureInfo culture)
			{
				Date date = (Date) value;
				long days = date.days;
				return string.Format (culture, "{0}", days);
			}
		}
		#endregion
		
		private System.DateTime					InternalDate
		{
			get
			{
				if (this.IsNull)
				{
					throw new System.NullReferenceException ("Date is Null.");
				}
				
				return new System.DateTime (this.days * Time.TicksPerDay);
			}
		}
		
		private readonly int					days;
	}
}
