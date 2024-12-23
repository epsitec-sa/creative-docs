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


using Epsitec.Common.Types;

[assembly: DependencyClass(typeof(Epsitec.Common.Widgets.Layouts.LayoutMeasure))]

namespace Epsitec.Common.Widgets.Layouts
{
    /// <summary>
    /// The <c>LayoutMeasure</c> class is used to represent the result of a measure in
    /// the layout engine.
    /// </summary>
    public class LayoutMeasure : DependencyObject
    {
        internal LayoutMeasure(int passId)
        {
            this.Reset(passId);
        }

        public double Min
        {
            get { return this.min; }
        }

        public double Max
        {
            get { return this.max; }
        }

        public double Desired
        {
            get
            {
                if (this.desired.IsSafeNaN())
                {
                    return this.desired;
                }
                else
                {
                    double value;
                    value = System.Math.Min(this.max, this.desired);
                    value = System.Math.Max(this.min, value);
                    return value;
                }
            }
        }

        public bool HasChanged
        {
            get { return this.hasChanged; }
        }

        public int PassId
        {
            get { return this.passId; }
        }

        public override string ToString()
        {
            return string.Format(
                "{0} in [{1}:{2}], pass={3}, desired={4}",
                this.Desired,
                this.Min,
                this.Max,
                this.passId,
                this.desired
            );
        }

        internal void Reset(int passId)
        {
            this.min = 0;
            this.max = double.PositiveInfinity;
            this.desired = double.NaN;
            this.passId = passId;
        }

        internal void ClearHasChanged()
        {
            this.hasChanged = false;
        }

        internal void UpdatePassId(int id)
        {
            this.passId = id;
        }

        internal void UpdateMin(int passId, double value, bool forceChange = false)
        {
            double old = this.min;

            if (this.passId == passId)
            {
                this.min = System.Math.Max(old, value);
            }
            else
            {
                this.min = value;
            }

            if ((this.min != old) || (forceChange))
            {
                this.hasChanged = true;
            }
        }

        internal void UpdateMax(int passId, double value)
        {
            double old = this.max;

            if (this.passId == passId)
            {
                this.max = System.Math.Min(old, value);
            }
            else
            {
                this.max = value;
            }

            if (this.max != old)
            {
                this.hasChanged = true;
            }
        }

        internal void UpdateDesired(double value)
        {
            if (this.desired.IsSafeNaN())
            {
                if (value.IsSafeNaN())
                {
                    return;
                }

                this.desired = value;
                this.hasChanged = true;
            }
            else if (this.desired != value)
            {
                this.desired = value;
                this.hasChanged = true;
            }
        }

        public static LayoutMeasure GetWidth(Visual visual)
        {
            return visual.GetValue(LayoutMeasure.WidthProperty) as LayoutMeasure;
        }

        public static LayoutMeasure GetHeight(Visual visual)
        {
            return visual.GetValue(LayoutMeasure.HeightProperty) as LayoutMeasure;
        }

        public static DependencyProperty WidthProperty =
            DependencyProperty<LayoutMeasure>.RegisterAttached<LayoutMeasure>(
                "Width",
                new DependencyPropertyMetadata().MakeNotSerializable()
            );
        public static DependencyProperty HeightProperty =
            DependencyProperty<LayoutMeasure>.RegisterAttached<LayoutMeasure>(
                "Height",
                new DependencyPropertyMetadata().MakeNotSerializable()
            );

        private double min;
        private double max;
        private int passId;
        private double desired;
        private bool hasChanged;
    }
}
