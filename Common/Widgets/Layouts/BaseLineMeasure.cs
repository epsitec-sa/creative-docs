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


namespace Epsitec.Common.Widgets.Layouts
{
    public class BaseLineMeasure : Types.DependencyObject
    {
        internal BaseLineMeasure(int passId)
        {
            this.h1 = 0;
            this.h2 = 0;
            this.passId = passId;
        }

        public double H1
        {
            get { return this.h1; }
        }
        public double H2
        {
            get { return this.h2; }
        }

        public bool HasChanged
        {
            get { return this.hasChanged; }
        }

        public bool IsEmpty
        {
            get { return this.passId == 0; }
        }

        public bool SamePassIdAsLayoutContext(Visual visual)
        {
            LayoutContext context = Helpers.VisualTree.FindLayoutContext(visual);

            if (context == null)
            {
                return false;
            }

            return context.PassId == this.passId;
        }

        internal void ClearHasChanged()
        {
            this.hasChanged = false;
        }

        internal void UpdatePassId(int id)
        {
            this.passId = id;
        }

        internal void UpdateBaseLine(int passId, double h1, double h2)
        {
            double oldH1 = this.h1;
            double oldH2 = this.h2;

            if (this.passId == passId)
            {
                this.h1 = System.Math.Max(oldH1, h1);
                this.h2 = System.Math.Max(oldH2, h2);
            }
            else
            {
                this.h1 = h1;
                this.h2 = h2;
            }

            if ((this.h1 != oldH1) || (this.h2 != oldH2))
            {
                this.hasChanged = true;
            }
        }

        public static BaseLineMeasure GetBaseLineValue(Visual visual)
        {
            return visual.GetValue(BaseLineMeasure.BaseLineProperty) as BaseLineMeasure;
        }

        public static void SetBaseLineValue(Visual visual, BaseLineMeasure measure)
        {
            visual.SetValue(BaseLineMeasure.BaseLineProperty, measure);
        }

        public static Types.DependencyProperty BaseLineProperty =
            Types.DependencyProperty.RegisterAttached(
                "BaseLine",
                typeof(BaseLineMeasure),
                typeof(BaseLineMeasure),
                new Types.DependencyPropertyMetadata().MakeNotSerializable()
            );

        private double h1,
            h2;
        private int passId;
        private bool hasChanged;
    }
}
