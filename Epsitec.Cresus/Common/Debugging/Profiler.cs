//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Debugging
{
	public static class Profiler
	{
		public static T ElapsedMilliseconds<T>(System.Func<T> func, out long time)
		{
			long microseconds;
			var result = Profiler.ElapsedMicroseconds (func, out microseconds);
			time = microseconds / 1000;
			return result;
		}

		public static T ElapsedMicroseconds<T>(System.Func<T> func, out long time)
		{
			T result = default (T);

			time = Profiler.ElapsedMicroseconds (() => result = func ());

			return result;
		}

		public static long ElapsedMilliseconds(System.Action action)
		{
			return Profiler.ElapsedMicroseconds (action) / 1000;
		}

		public static long ElapsedMicroseconds(System.Action action)
		{
			var watch = new System.Diagnostics.Stopwatch ();

			watch.Start ();
			action ();
			watch.Stop ();

			return watch.ElapsedTicks * 1000L * 1000L / System.Diagnostics.Stopwatch.Frequency;
		}
	}
}
