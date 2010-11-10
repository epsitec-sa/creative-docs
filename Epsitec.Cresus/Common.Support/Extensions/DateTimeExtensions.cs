//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Support.Extensions
{
	public static class DateTimeExtensions
	{
		public static bool InRange(this System.DateTime date, System.DateTime? beginDate, System.DateTime? endDate)
		{
			if (beginDate.HasValue && beginDate.Value > date)
			{
				return false;
			}
			if (endDate.HasValue && endDate.Value < date)
			{
				return false;
			}

			return true;
		}
	}
}
