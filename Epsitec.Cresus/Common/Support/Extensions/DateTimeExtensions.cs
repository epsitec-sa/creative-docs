//	Copyright � 2010-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Support.Extensions
{
	/// <summary>
	/// The <c>DateTimeExtensions</c> class provides extension methods for <see cref="System.DateTime"/>.
	/// </summary>
	public static class DateTimeExtensions
	{
		public static bool InRange(this System.DateTime date, System.DateTime? beginDate, System.DateTime? endDate)
		{
			var dateUtc = date.ToUniversalTime ();

			if (beginDate.HasValue && beginDate.Value.ToUniversalTime () > dateUtc)
			{
				return false;
			}
			if (endDate.HasValue && endDate.Value.ToUniversalTime () <= dateUtc)
			{
				return false;
			}

			return true;
		}
		
		public static bool InRange(this Date date, Date? beginDate, Date? endDate)
		{
			if (beginDate.HasValue && beginDate.Value > date)
			{
				return false;
			}
			if (endDate.HasValue && endDate.Value <= date)
			{
				return false;
			}

			return true;
		}

		public static System.DateTime? ToLocalTime(this System.DateTime? date)
		{
			if (date == null)
			{
				return null;
			}

			return date.Value.ToLocalTime ();
		}
	}
}
