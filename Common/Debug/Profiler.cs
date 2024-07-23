/*
This file is part of CreativeDocs.

Copyright © 2003-2024, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland

CreativeDocs is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
any later version.

CreativeDocs is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/


using System.Diagnostics;

namespace Epsitec.Common.Debug
{
    public static class Profiler
    {
        public static T ElapsedMilliseconds<T>(System.Func<T> func, out long time)
        {
            long microseconds;
            var result = Profiler.ElapsedMicroseconds(func, out microseconds);
            time = microseconds / 1000;
            return result;
        }

        public static T ElapsedMicroseconds<T>(System.Func<T> func, out long time)
        {
            T result = default(T);

            time = Profiler.ElapsedMicroseconds(() => result = func());

            return result;
        }

        public static long ElapsedMilliseconds(System.Action action)
        {
            return Profiler.ElapsedMicroseconds(action) / 1000;
        }

        public static long ElapsedMicroseconds(System.Action action)
        {
            var watch = new Stopwatch();

            watch.Start();
            action();
            watch.Stop();

            return watch.ElapsedTicks * 1000L * 1000L / Stopwatch.Frequency;
        }

        public static System.IDisposable MeasureMicroseconds(string debugOutputText)
        {
            return new MeasureTime(debugOutputText, 1000L * 1000L, "µs");
        }

        public static System.IDisposable MeasureMilliseconds(string debugOutputText)
        {
            return new MeasureTime(debugOutputText, 1000L, "ms");
        }

        private class MeasureTime : System.IDisposable
        {
            public MeasureTime(string outputFormat, long multiplier, string suffix)
            {
                this.watch = new Stopwatch();
                this.format = (outputFormat ?? "").Replace("{0}", "{0}" + (suffix ?? ""));
                this.multiplier = multiplier;

                this.watch.Start();
            }

            #region IDisposable Members

            public void Dispose()
            {
                this.watch.Stop();

                if (string.IsNullOrEmpty(this.format))
                {
                    return;
                }

                long ticks = this.watch.ElapsedTicks - MeasureTime.offset;

                System.Diagnostics.Debug.WriteLine(
                    string.Format(this.format, ticks * this.multiplier / Stopwatch.Frequency)
                );
            }

            #endregion

            static MeasureTime()
            {
                MeasureTime measure = null;

                for (int i = 0; i < 10; i++)
                {
                    using (measure = new MeasureTime(null, 1L, null))
                    {
                        //	Warm-up and then a few real measurements. Probably, this will result
                        //	in a zero tick count with the watch resolution (as of June 2012).
                    }
                }

                MeasureTime.offset = measure.watch.ElapsedTicks;
            }

            private static readonly long offset;

            private readonly Stopwatch watch;
            private readonly long multiplier;
            private readonly string format;
        }
    }
}
