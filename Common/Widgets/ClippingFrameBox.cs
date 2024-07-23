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


using Epsitec.Common.Drawing;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

[assembly: DependencyClass(typeof(ClippingFrameBox))]

namespace Epsitec.Common.Widgets
{
    /// <summary>
    /// The <c>ClippingFrameBox</c> class is a special <see cref="FrameBox"/> which provides
    /// the ability to clip its children horizontally and/or vertically.
    /// </summary>
    public class ClippingFrameBox : FrameBox
    {
        public ClippingFrameBox() { }

        /// <summary>
        /// Gets or sets the clipping mode.
        /// </summary>
        /// <value>The clipping mode.</value>
        public ClippingMode ClippingMode
        {
            get { return this.clippingMode; }
            set
            {
                if (this.clippingMode != value)
                {
                    this.clippingMode = value;
                    Layouts.LayoutContext.AddToMeasureQueue(this);
                }
            }
        }

        protected override void MeasureMinMax(ref Size min, ref Size max)
        {
            Size oldMin = min;
            Size oldMax = max;

            base.MeasureMinMax(ref min, ref max);

            if ((this.clippingMode & Widgets.ClippingMode.ClipWidth) != 0)
            {
                //	Don't take into account the children width in the min/max measurement :

                min = new Size(oldMin.Width, min.Height);
                max = new Size(oldMax.Width, max.Height);
            }

            if ((this.clippingMode & Widgets.ClippingMode.ClipHeight) != 0)
            {
                //	Don't take into account the children height in the min/max measurement :

                min = new Size(min.Width, oldMin.Height);
                max = new Size(max.Width, oldMax.Height);
            }
        }

        private ClippingMode clippingMode;
    }
}
