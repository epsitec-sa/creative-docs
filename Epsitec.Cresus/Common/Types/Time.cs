//	Copyright © 2003-2011, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// La structure Time est une simplification de DateTime. Elle ne
	/// représente que l'heure du jour
	/// </summary>
	
	[System.Serializable]
	[System.ComponentModel.TypeConverter (typeof (Time.Converter))]
	
	public struct Time : System.IComparable, INullable, System.IEquatable<Time>, System.IFormattable
	{
		public Time(long ticks)
		{
			if ((ticks < 0) ||
				(ticks > Time.TicksPerDay))
			{
				throw new System.ArgumentException ("Ticks out of range");
			}

			this.milliseconds = (int) (ticks / Time.TicksPerMillisecond);
		}

		public Time(System.DateTime dateTime)
			: this (dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond)
		{
		}

		private Time(int milliseconds, bool justToMakeSure)
		{
			this.milliseconds = milliseconds;
		}
		
		public Time(int hour, int minute, int second) : this (hour, minute, second, 0)
		{
		}
		
		public Time(int hour, int minute, int second, int millisecond)
		{
			if ((hour > 23) || (hour < 0) ||
				(minute > 59) || (minute < 0) ||
				(second > 59) || (second < 0) ||
				(millisecond > 1000) || (millisecond < 0))
			{
				throw new System.ArgumentException ("Time argument out of range");
			}
			
			this.milliseconds = (millisecond + 1000*(second + 60*(minute + 60*hour)));
			
			System.Diagnostics.Debug.Assert (this.Ticks >= 0);
			System.Diagnostics.Debug.Assert (this.Ticks < Time.TicksPerDay);
		}
		
		
		public int								Hour
		{
			get
			{
				return this.InternalTime / 60 / 60 / 1000;
			}
		}
		
		public int								Minute
		{
			get
			{
				return (this.InternalTime / 60 / 1000) % 60;
			}
		}
		
		public int								Second
		{
			get
			{
				return (this.InternalTime / 1000) % 60;
			}
		}
		
		public int								Millisecond
		{
			get
			{
				return (this.InternalTime) % 1000;
			}
		}
		
		public long								Ticks
		{
			get
			{
				return this.milliseconds * Time.TicksPerMillisecond;
			}
		}
		
		public static Time						Now
		{
			get
			{
				return new Time (System.DateTime.Now);
			}
		}

		public static readonly Time				Null = new Time (-1, true);
		
		public const long						TicksPerDay = 1000*60*60*24*10000L;
		public const long						TicksPerSecond = 1000*10000L;
		public const long						TicksPerMillisecond = 10000L;
		
		
		public Time AddHour(int value)
		{
			long ticks = this.Ticks + (long) value * 60 * 60 * Time.TicksPerSecond;
			return new Time (ticks % Time.TicksPerDay);
		}
		
		public Time AddMinute(int value)
		{
			long ticks = this.Ticks + (long) value * 60 * Time.TicksPerSecond;
			return new Time (ticks % Time.TicksPerDay);
		}
		
		public Time AddSecond(int value)
		{
			long ticks = this.Ticks + (long) value * Time.TicksPerSecond;
			return new Time (ticks % Time.TicksPerDay);
		}
		
		public Time AddMillisecond(int value)
		{
			long ticks = this.Ticks + (long) value * Time.TicksPerMillisecond;
			return new Time (ticks % Time.TicksPerDay);
		}
		
		
		public System.DateTime ToDateTime()
		{
			if (this.IsNull)
			{
				throw new System.NullReferenceException ("Date is Null.");
			}

			return new System.DateTime (this.Ticks);
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
			else if (obj is Time)
			{
				thatTicks = ((Time) obj).Ticks;
			}
			else
			{
				return false;
			}

			return thisTicks == thatTicks;
		}
		
		public override int GetHashCode()
		{
			return this.milliseconds;
		}


		public static Time FromObject(object value)
		{
			if (value is System.DateTime)
			{
				System.DateTime dateTime  = (System.DateTime) value;
				return new Time (dateTime);
			}
			else if (value is Time)
			{
				return (Time) value;
			}
			else if (value is System.TimeSpan)
			{
				System.TimeSpan timeSpan = (System.TimeSpan) value;
				return new Time (timeSpan.Ticks);
			}
			else if (value == null)
			{
				return Time.Null;
			}
			else
			{
				throw new System.ArgumentException ("Not a Time nor a DateTime");
			}
		}
		
		public static int Compare(Time t1, Time t2)
		{
			if (t1.milliseconds > t2.milliseconds)
			{
				return 1;
			}
			
			if (t1.milliseconds < t2.milliseconds)
			{
				return -1;
			}
			
			return 0;
		}
		
		public static bool Equals(Time t1, Time t2)
		{
			return t1.milliseconds == t2.milliseconds;
		}

		public static bool operator==(Time t1, Time t2)
		{
			return t1.milliseconds == t2.milliseconds;
		}
		public static bool operator!=(Time t1, Time t2)
		{
			return t1.milliseconds != t2.milliseconds;
		}
		public static bool operator<(Time t1, Time t2)
		{
			return t1.milliseconds < t2.milliseconds;
		}
		public static bool operator>(Time t1, Time t2)
		{
			return t1.milliseconds > t2.milliseconds;
		}
		public static bool operator<=(Time t1, Time t2)
		{
			return t1.milliseconds <= t2.milliseconds;
		}
		public static bool operator>=(Time t1, Time t2)
		{
			return t1.milliseconds >= t2.milliseconds;
		}
		
		
		public override string ToString()
		{
			if (this.IsNull)
			{
				return "<null>";
			}
			else
			{
				var hour   = this.Hour;
				var minute = this.Minute;
				var second = this.Second;
				var milli  = this.Millisecond;

				if (milli == 0)
				{
					return string.Format (System.Globalization.CultureInfo.CurrentCulture, "{0:00}:{1:00}:{2:00}", hour, minute, second);
				}
				else
				{
					return string.Format (System.Globalization.CultureInfo.CurrentCulture, "{0:00}:{1:00}:{2:00}.{3:000}", hour, minute, second, milli);
				}
			}
		}
		
		
		public static System.TimeSpan operator - (Time t1, Time t2)
		{
			return new System.TimeSpan (t1.Ticks - t2.Ticks);
		}

		#region IFormattable Members

		public string ToString(string format, System.IFormatProvider formatProvider)
		{
			return this.ToDateTime ().ToString (format, formatProvider);
		}

		#endregion
		

		#region IEquatable<Time> Members

		public bool Equals(Time other)
		{
			return this.milliseconds == other.milliseconds;
		}

		#endregion
		
		#region INullable Members
		
		public bool								IsNull
		{
			get
			{
				return (this.milliseconds == -1);
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
				thatTicks = ((System.DateTime)obj).Ticks;
			}
			else if (obj is Time)
			{
				thatTicks = ((Time)obj).Ticks;
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
				int milliseconds = System.Int32.Parse (value, culture);
				Time time = new Time (milliseconds, true);
				return time;
			}
			
			public override string ToString(object value, System.Globalization.CultureInfo culture)
			{
				Time time = (Time) value;
				int milliseconds = time.milliseconds;
				return string.Format (culture, "{0}", milliseconds);
			}
		}
		#endregion
		
		private int								InternalTime
		{
			get
			{
				if (this.IsNull)
				{
					throw new System.NullReferenceException ("Time is Null");
				}
				
				return this.milliseconds;
			}
		}
		
		private readonly int					milliseconds;

	}
}
