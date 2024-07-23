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


namespace Epsitec.Common.Widgets
{
    public class AnimatorValue<T> : AnimatorValue
        where T : struct
    {
        public AnimatorValue(T beginValue, T endValue)
        {
            this.v1 = beginValue;
            this.v2 = endValue;
        }

        public override object BeginValue
        {
            get { return this.v1; }
        }

        public override object EndValue
        {
            get { return this.v2; }
        }

        public override object Interpolate(double ratio)
        {
            System.Diagnostics.Debug.Assert(ratio >= 0.0);
            System.Diagnostics.Debug.Assert(ratio <= 1.0);

            return AnimatorValue<T>.computeFunc(ratio, this.v1, this.v2);
        }

        static AnimatorValue()
        {
            if (typeof(T) == typeof(int))
            {
                AnimatorValue<int>.computeFunc = (ratio, a, b) =>
                    (int)((1.0 - ratio) * a + ratio * b);
            }
            else if (typeof(T) == typeof(double))
            {
                AnimatorValue<double>.computeFunc = (ratio, a, b) =>
                    (double)((1.0 - ratio) * a + ratio * b);
            }
            else if (typeof(T) == typeof(decimal))
            {
                AnimatorValue<decimal>.computeFunc = (ratio, a, b) =>
                    (decimal)((1.0 - ratio) * (double)a + ratio * (double)b);
            }
            else if (typeof(T) == typeof(Drawing.Point))
            {
                AnimatorValue<Drawing.Point>.computeFunc = (ratio, a, b) =>
                    new Drawing.Point(
                        (1.0 - ratio) * a.X + ratio * b.X,
                        (1.0 - ratio) * a.Y + ratio * b.Y
                    );
            }
            else if (typeof(T) == typeof(Drawing.Size))
            {
                AnimatorValue<Drawing.Size>.computeFunc = (ratio, a, b) =>
                    new Drawing.Size(
                        (1.0 - ratio) * a.Width + ratio * b.Width,
                        (1.0 - ratio) * a.Height + ratio * b.Height
                    );
            }
            else if (typeof(T) == typeof(Drawing.Rectangle))
            {
                AnimatorValue<Drawing.Rectangle>.computeFunc = (ratio, a, b) =>
                    new Drawing.Rectangle(
                        (1.0 - ratio) * a.X + ratio * b.X,
                        (1.0 - ratio) * a.Y + ratio * b.Y,
                        (1.0 - ratio) * a.Width + ratio * b.Width,
                        (1.0 - ratio) * a.Height + ratio * b.Height
                    );
            }
            else if (typeof(T) == typeof(Drawing.Color))
            {
                AnimatorValue<Drawing.Color>.computeFunc = (ratio, a, b) =>
                    new Drawing.Color(
                        (1.0 - ratio) * a.A + ratio * b.A,
                        (1.0 - ratio) * a.R + ratio * b.R,
                        (1.0 - ratio) * a.G + ratio * b.G,
                        (1.0 - ratio) * a.B + ratio * b.B
                    );
            }
            else
            {
                throw new System.NotSupportedException(
                    "Unsupported type, cannot animate " + typeof(T).Name
                );
            }
        }

        private static System.Func<double, T, T, T> computeFunc;

        private readonly T v1;
        private readonly T v2;
    }
}
