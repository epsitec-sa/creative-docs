//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// La structure Time est une simplification de DateTime. Elle ne
	/// représente que l'heure du jour
	/// </summary>
	public struct Time : System.IComparable, INullable
	{
		public Time(long ticks)
		{
			if ((ticks < 0) ||
				(ticks > Time.TicksPerDay))
			{
				throw new System.ArgumentException ("Ticks out of range");
			}
			
			this.time = (int)(ticks / 10000L);
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
			
			this.time = (millisecond + 1000*(second + 60*(minute + 60*hour)));
			
			System.Diagnostics.Debug.Assert (this.Ticks >= 0);
			System.Diagnostics.Debug.Assert (this.Ticks < Time.TicksPerDay);
		}
		
		public Time(object time)
		{
			if (time is System.DateTime)
			{
				System.DateTime date_time  = (System.DateTime)time;
				Time       date_time_time = new Time (date_time.Hour, date_time.Minute, date_time.Second, date_time.Millisecond);
				
				this.time = date_time_time.time;
			}
			else if (time is Time)
			{
				this.time = ((Time)time).time;
			}
			else if (time == null)
			{
				this.time = -1;
			}
			else
			{
				throw new System.ArgumentException ("Not a Time nor a DateTime");
			}
		}
		
		
		public int								Hour
		{
			get { return this.InternalTime / 60 / 60 / 1000; }
		}
		
		public int								Minute
		{
			get { return (this.InternalTime / 60 / 1000) % 60; }
		}
		
		public int								Second
		{
			get { return (this.InternalTime / 1000) % 60; }
		}
		
		public int								Millisecond
		{
			get { return (this.InternalTime) % 1000; }
		}
		
		public long								Ticks
		{
			get { return this.InternalTime * 10000L; }
		}
		
		
		public static Time						Now
		{
			get { return new Time (System.DateTime.Now); }
		}
		
		
		public const long						TicksPerDay = 1000*60*60*24*10000L;
		public const long						TicksPerSecond = 1000*10000L;
		
		
		public Time AddHour(int value)
		{
			long ticks = this.Ticks + value * 60 * 60 * Time.TicksPerSecond;
			return new Time (ticks % Time.TicksPerDay);
		}
		
		public Time AddMinute(int value)
		{
			long ticks = this.Ticks + value * 60 * Time.TicksPerSecond;
			return new Time (ticks % Time.TicksPerDay);
		}
		
		public Time AddSecond(int value)
		{
			long ticks = this.Ticks + value * Time.TicksPerSecond;
			return new Time (ticks % Time.TicksPerDay);
		}
		
		public Time AddMillisecond(int value)
		{
			long ticks = this.Ticks + value * Time.TicksPerSecond / 1000L;
			return new Time (ticks % Time.TicksPerDay);
		}
		
		
		public System.DateTime ToDateTime()
		{
			return new System.DateTime (this.Ticks);
		}
		
		
		public override bool Equals(object obj)
		{
			return this.CompareTo (obj) == 0;
		}
		
		public override int GetHashCode()
		{
			return this.time;
		}

		
		public static int Compare(Time t1, Time t2)
		{
			if (t1.time > t2.time)
			{
				return 1;
			}
			
			if (t1.time < t2.time)
			{
				return -1;
			}
			
			return 0;
		}
		
		public static bool Equals(Time t1, Time t2)
		{
			return t1.time == t2.time;
		}
		
		
		public override string ToString()
		{
			return string.Format ("{0:00}:{1:00}:{2:00}.{3:000}", this.Hour, this.Minute, this.Second, this.Millisecond);
		}
		
		
		#region INullable Members
		public bool								IsNull
		{
			get
			{
				return (this.time == -1);
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
				that_ticks = ((System.DateTime)obj).Ticks;
			}
			else if (obj is Time)
			{
				that_ticks = ((Time)obj).Ticks;
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
		
		private int								InternalTime
		{
			get
			{
				if (this.IsNull)
				{
					throw new System.NullReferenceException ("Time is Null.");
				}
				
				return this.time;
			}
		}
		
		private int								time;
	}
}
