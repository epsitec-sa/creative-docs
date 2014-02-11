//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	public struct DateRange
	{
		public DateRange(System.DateTime includeFrom, System.DateTime includeTo)
		{
			//	Typiquement, includeFrom = 01.01.2013 et includeTo = 31.12.2013.
			this.IncludeFrom = includeFrom;
			this.IncludeTo   = includeTo;
		}

		public bool IsEmpty
		{
			get
			{
				return this.IncludeFrom == System.DateTime.MaxValue
					&& this.IncludeTo   == System.DateTime.MinValue;
			}
		}

		public System.DateTime ExcludeTo
		{
			get
			{
				return this.IncludeTo.AddDays (1);
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
				return new Timestamp (this.IncludeTo, 0);
			}
		}

		public bool IsInside(System.DateTime date)
		{
			return date >= this.IncludeFrom.Date
				&& date <= this.IncludeTo.Date;
		}

		public static DateRange Empty = new DateRange (System.DateTime.MaxValue, System.DateTime.MinValue);
		public static DateRange Full  = new DateRange (System.DateTime.MinValue, System.DateTime.MaxValue);

		public readonly System.DateTime			IncludeFrom;
		public readonly System.DateTime			IncludeTo;
	}
}