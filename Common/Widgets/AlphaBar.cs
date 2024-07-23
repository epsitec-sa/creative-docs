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
    /// <summary>
    /// La class AlphaBar représente une barre translucide pendant un drag.
    /// </summary>
    public class AlphaBar : Widget
    {
        public AlphaBar()
        {
            this.color = Drawing.Color.FromAlphaRgb(0.15, 0, 0, 0);
        }

        protected override void PaintBackgroundImplementation(
            Drawing.Graphics graphics,
            Drawing.Rectangle clipRect
        )
        {
            //	Dessine la barre.
            Drawing.Rectangle rect = this.Client.Bounds;

            graphics.AddFilledRectangle(rect);
            graphics.RenderSolid(this.color);
        }

        protected Drawing.Color color;
    }
}
