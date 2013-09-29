//	Copyright � 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Types;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	public struct TimelineCellDate
	{
		public TimelineCellDate(Date date, bool isSelected = false, bool isError = false)
		{
			this.Date       = date;
			this.IsSelected = isSelected;
			this.IsError    = isError;
		}

		
		public bool								IsInvalid
		{
			get
			{
				return this.Date.IsNull;
			}
		}

		public bool								IsValid
		{
			get
			{
				return !this.Date.IsNull;
			}
		}

		public readonly Date					Date;
		public readonly bool					IsSelected;
		public readonly bool					IsError;

		
		public override string ToString()
		{
			var buffer = new System.Text.StringBuilder ();

			buffer.Append (this.Date.ToString ());

			if (this.IsSelected)
			{
				buffer.Append (" selected");
			}
			if (this.IsError)
			{
				buffer.Append (" error");
			}

			return buffer.ToString ();
		}


		public static bool IsSameWeeksOfYear(TimelineCellDate c1, TimelineCellDate c2)
		{
			int w1 = (c1.IsValid) ? c1.Date.WeekOfYear : -1;
			int w2 = (c2.IsValid) ? c2.Date.WeekOfYear : -1;

			return w1 == w2;
		}

		public static bool IsSameMonths(TimelineCellDate c1, TimelineCellDate c2)
		{
			int m1 = (c1.IsValid) ? c1.Date.Month : -1;
			int m2 = (c2.IsValid) ? c2.Date.Month : -1;

			return m1 == m2;
		}

		public static bool IsSameDays(TimelineCellDate c1, TimelineCellDate c2)
		{
			int d1 = (c1.IsValid) ? c1.Date.Day : -1;
			int d2 = (c2.IsValid) ? c2.Date.Day : -1;

			return d1 == d2;
		}
	}
}