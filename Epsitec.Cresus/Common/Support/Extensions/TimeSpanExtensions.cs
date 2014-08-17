//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Support.Extensions
{
	public static class TimeSpanExtensions
	{
		public static string GetSimpleFormattedTime(this System.TimeSpan span)
		{
			var milli = span.TotalMilliseconds;
			var sec   = span.TotalSeconds;
			var min   = span.TotalMinutes;
			var hour  = span.TotalHours;

			if (hour > 2)
			{
				return string.Format ("{0:0} h", hour);
			}
			if (min > 2)
			{
				return string.Format ("{0:0} min", min);
			}
			if (sec > 2)
			{
				return string.Format ("{0:0} s", sec);
			}
			if (milli > 100)
			{
				return string.Format ("{0:0.0} s", milli / 1000);
			}

			return string.Format ("{0:0} ms", milli);
		}
	}
}

