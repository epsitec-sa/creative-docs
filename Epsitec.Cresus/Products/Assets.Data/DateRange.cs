//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data.Helpers;

namespace Epsitec.Cresus.Assets.Data
{
	public struct DateRange
	{
		public DateRange(System.DateTime includeFrom, System.DateTime excludeTo)
		{
			//	Typiquement, includeFrom = 01.01.2013 et excludeTo = 01.01.2014.
			this.IncludeFrom = includeFrom;
			this.ExcludeTo   = excludeTo;
		}

		public bool IsEmpty
		{
			get
			{
				return this == DateRange.Empty;
			}
		}

		public bool AtLeastOneTime
		{
			get
			{
				return this.IncludeFrom < this.ExcludeTo;
			}
		}

		public Timestamp FromTimestamp
		{
			get
			{
				return new Timestamp (this.IncludeFrom, 0);
			}
		}

		public Timestamp ToTimestamp
		{
			get
			{
				return new Timestamp (this.ExcludeTo, 0);
			}
		}

		public bool IsInside(System.DateTime date)
		{
			return date >= this.IncludeFrom.Date
				&& date <  this.ExcludeTo.Date;
		}


		public DateRange ChangePeriod(int direction)
		{
			if (this.IncludeFrom.Day == 1 &&
				this.ExcludeTo.Day   == 1)
			{
				return this.ChangePeriodMonth (direction);
			}
			else
			{
				return this.ChangePeriodGeneric (direction);
			}
		}

		private DateRange ChangePeriodMonth(int direction)
		{
			if (direction > 0)
			{
				var month = this.ExcludeTo.GetTotalMonth () - this.IncludeFrom.GetTotalMonth ();
				return new DateRange (this.ExcludeTo, this.ExcludeTo.AddMonths (month));
			}
			else if (direction < 0)
			{
				var month = this.ExcludeTo.GetTotalMonth () - this.IncludeFrom.GetTotalMonth ();
				return new DateRange (this.IncludeFrom.AddMonths (-month), this.IncludeFrom);
			}
			else
			{
				return this;
			}
		}

		private DateRange ChangePeriodGeneric(int direction)
		{
			if (direction > 0)
			{
				var delta = this.ExcludeTo - this.IncludeFrom;
				return new DateRange (this.ExcludeTo, this.ExcludeTo + delta);
			}
			else if (direction < 0)
			{
				var delta = this.ExcludeTo - this.IncludeFrom;
				return new DateRange (this.IncludeFrom - delta, this.IncludeFrom);
			}
			else
			{
				return this;
			}
		}


		public static bool operator ==(DateRange a, DateRange b)
		{
			return object.Equals (a, b);
		}

		public static bool operator !=(DateRange a, DateRange b)
		{
			return !(a == b);
		}

		public override bool Equals(object obj)
		{
			var other = (DateRange) obj;

			return !object.ReferenceEquals (other, null)
				&& this.IncludeFrom == other.IncludeFrom
				&& this.ExcludeTo   == other.ExcludeTo;
		}

		public override int GetHashCode()
		{
			return this.IncludeFrom.GetHashCode ()
				 ^ this.ExcludeTo.GetHashCode ();
		}


		public override string ToString()
		{
			//	Pour le debug.
			return string.Concat ("From ", IncludeFrom.ToString ("dd.MM.yyyy"), " To ", ExcludeTo.ToString ("dd.MM.yyyy"));
		}


		public static DateRange Empty = new DateRange ();
		public static DateRange Full  = new DateRange (System.DateTime.MinValue, System.DateTime.MaxValue);

		public readonly System.DateTime			IncludeFrom;
		public readonly System.DateTime			ExcludeTo;


		static DateRange()
		{
			//	Auto-test:
			//	Un DateRange créé sans paramètres doit absolument être équivalement
			//	à un DateRange.Empty !

			System.Diagnostics.Debug.Assert (new DateRange () == DateRange.Empty);
			System.Diagnostics.Debug.Assert (new DateRange ().IsEmpty);
		}
	}
}