//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

namespace Epsitec.Common.Types
{
	/// <summary>
	/// La structure Date est une simplification de DateTime. Elle ne
	/// représente que la date du jour, sans aucune indication de
	/// l'heure.
	/// </summary>
	public struct Date : System.IComparable, INullable
	{
		public Date(int year, int month, int day)
		{
			this.ticks = (new System.DateTime (year, month, day)).Ticks;
		}
		
		public Date(object date)
		{
			if (date is System.DateTime)
			{
				this.ticks = ((System.DateTime)date).Date.Ticks;
			}
			else if (date is Date)
			{
				this.ticks = ((Date)date).ticks;
			}
			else if (date == null)
			{
				this.ticks = -1;
			}
			else
			{
				throw new System.ArgumentException ("Not a Date nor a DateTime");
			}
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
			get { return this.ticks; }
		}
		
		
		public static Date						Today
		{
			get { return new Date (System.DateTime.Today); }
		}
		
		
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
			return this.ticks.GetHashCode ();
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

		
		public override string ToString()
		{
			return this.InternalDate.ToString ("d");
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
				return (this.ticks == -1L);
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
		
		private System.DateTime					InternalDate
		{
			get
			{
				if (this.IsNull)
				{
					throw new System.NullReferenceException ("Date is Null.");
				}
				
				return new System.DateTime (this.ticks);
			}
		}
		
		private long							ticks;
	}
}
