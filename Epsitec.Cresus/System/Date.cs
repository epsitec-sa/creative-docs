namespace System
{
	/// <summary>
	/// La structure Date est une simplification de DateTime. Elle ne
	/// représente que la date du jour, sans aucune indication de
	/// l'heure.
	/// </summary>
	public struct Date : System.IComparable
	{
		public Date(int year, int month, int day)
		{
			this.date_time = new System.DateTime (year, month, day);
		}
		
		public Date(object date)
		{
			if (date is System.DateTime)
			{
				this.date_time = ((System.DateTime)date).Date;
			}
			else if (date is System.Date)
			{
				this.date_time = ((System.Date)date).date_time;
			}
			else
			{
				throw new System.ArgumentException ("Not a Date nor a DateTime");
			}
		}
		
		
		public int						Day
		{
			get { return this.date_time.Day; }
		}
		
		public System.DayOfWeek			DayOfWeek
		{
			get { return this.date_time.DayOfWeek; }
		}

		public int						DayOfYear
		{
			get { return this.date_time.DayOfYear; }
		}
		
		public int						Month
		{
			get { return this.date_time.Month; }
		}
		
		public int						Year
		{
			get { return this.date_time.Year; }
		}
		
		public long						Ticks
		{
			get { return this.date_time.Ticks; }
		}
		
		
		public static Date				Today
		{
			get { return new Date (System.DateTime.Today); }
		}
		
		
		public Date AddDays(int value)
		{
			return new Date (this.date_time.AddDays (value));
		}
		
		public Date AddMonths(int value)
		{
			return new Date (this.date_time.AddMonths (value));
		}
		
		public Date AddYears(int value)
		{
			return new Date (this.date_time.AddYears (value));
		}
		
		
		public System.DateTime ToDateTime()
		{
			return this.date_time;
		}
		
		
		public override bool Equals(object obj)
		{
			return this.CompareTo (obj) == 0;
		}
		
		public override int GetHashCode()
		{
			return this.date_time.GetHashCode ();
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
		
		
		#region IComparable Members
		
		public int CompareTo(object obj)
		{
			long this_ticks = 0;
			long that_ticks = 0;
			
			if (obj == null)
			{
				return 1;
			}
			
			if (obj is DateTime)
			{
				that_ticks = ((DateTime)obj).Ticks;
			}
			else if (obj is Date)
			{
				that_ticks = ((Date)obj).Ticks;
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
		
		private System.DateTime			date_time;
	}
}
