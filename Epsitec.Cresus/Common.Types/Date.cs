//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// La structure Date est une simplification de DateTime. Elle ne
	/// représente que la date du jour, sans aucune indication de
	/// l'heure.
	/// </summary>
	
	[System.Serializable]
	[System.ComponentModel.TypeConverter (typeof (Date.Converter))]
	
	public struct Date : System.IComparable, INullable
	{
		public Date(int year, int month, int day) : this()
		{
			this.Ticks = (new System.DateTime (year, month, day)).Ticks;
		}
		
		public Date(object date) : this()
		{
			if (date is System.DateTime)
			{
				this.Ticks = ((System.DateTime) date).Date.Ticks;
			}
			else if (date is Date)
			{
				this.days = ((Date) date).days;
			}
			else if (date == null)
			{
				this.days = -1;
			}
			else
			{
				throw new System.ArgumentException ("Neither a Date nor a DateTime");
			}
		}
		
		public Date(long ticks) : this()
		{
			this.Ticks = ticks;
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
			set
			{
				this.days = (int) (value / Time.TicksPerDay);
			}
		}
		
		
		public static Date						Today
		{
			get
			{
				return new Date (System.DateTime.Today);
			}
		}

		public static readonly Date				Null = new Date (null);

		
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
			return this.CompareTo (obj) == 0;
		}
		
		public override int GetHashCode()
		{
			return this.days.GetHashCode ();
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
		
		public override string ToString()
		{
			return this.InternalDate.ToString ("d", System.Globalization.CultureInfo.CurrentCulture);
		}

		public string ToString(System.IFormatProvider provider)
		{
			return this.InternalDate.ToString ("d", provider);
		}
		
		
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
			long this_ticks = this.Ticks;
			long that_ticks = 0;
			
			if (obj == null)
			{
				return 1;
			}
			
			if (obj is System.DateTime)
			{
				that_ticks = ((System.DateTime) obj).Ticks;
			}
			else if (obj is Date)
			{
				that_ticks = ((Date) obj).Ticks;
			}
			else
			{
				throw new System.ArgumentException ("Invalid argument");
			}
			
			if (this_ticks > that_ticks)
			{
				return 1;
			}
			if (this_ticks < that_ticks)
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
				Date date = new Date ();
				date.days = days;
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
		
		private int								days;
	}
}
