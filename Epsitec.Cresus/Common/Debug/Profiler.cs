//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Epsitec.Common.Debug
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
			var watch = new Stopwatch ();

			watch.Start ();
			action ();
			watch.Stop ();

			return watch.ElapsedTicks * 1000L * 1000L / Stopwatch.Frequency;
		}

		
		public static System.IDisposable MeasureMicroseconds(string debugOutputText)
		{
			return new MeasureTime (debugOutputText, 1000L*1000L, "µs");
		}

		public static System.IDisposable MeasureMilliseconds(string debugOutputText)
		{
			return new MeasureTime (debugOutputText, 1000L, "ms");
		}



		private class MeasureTime : System.IDisposable
		{
			public MeasureTime(string outputFormat, long multiplier, string suffix)
			{
				this.watch      = new Stopwatch ();
				this.format     = (outputFormat ?? "").Replace ("{0}", "{0}" + (suffix ?? ""));
				this.multiplier = multiplier;

				this.watch.Start ();
			}

			#region IDisposable Members

			public void Dispose()
			{
				this.watch.Stop ();

				if (string.IsNullOrEmpty (this.format))
				{
					return;
				}

				long ticks = this.watch.ElapsedTicks - MeasureTime.offset;

				System.Diagnostics.Debug.WriteLine (string.Format (this.format, ticks * this.multiplier / Stopwatch.Frequency));
			}

			#endregion

			static MeasureTime()
			{
				MeasureTime measure = null;

				for (int i = 0; i < 10; i++)
				{
					using (measure = new MeasureTime (null, 1L, null))
					{
						//	Warm-up and then a few real measurements. Probably, this will result
						//	in a zero tick count with the watch resolution (as of June 2012).
					}
				}

				MeasureTime.offset = measure.watch.ElapsedTicks;
			}

			private static readonly long		offset;

			private readonly Stopwatch			watch;
			private readonly long				multiplier;
			private readonly string				format;
		}
	}
}
