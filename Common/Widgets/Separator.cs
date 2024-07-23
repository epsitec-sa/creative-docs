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

[assembly: Epsitec.Common.Types.DependencyClass(typeof(Epsitec.Common.Widgets.Separator))]

namespace Epsitec.Common.Widgets
{
    /// <summary>
    /// La classe Separator permet de dessiner des séparations.
    /// </summary>
    public class Separator : Widget
    {
        public Separator() { }

        public Separator(Widget embedder)
            : this()
        {
            this.SetEmbedder(embedder);
        }

        public Drawing.Color Color
        {
            get { return this.color; }
            set
            {
                if (this.color != value)
                {
                    this.color = value;
                    this.Invalidate();
                }
            }
        }

        public double Alpha
        {
            get { return this.alpha; }
            set
            {
                if (this.alpha != value)
                {
                    this.alpha = value;
                    this.Invalidate();
                }
            }
        }

        public bool IsHorizontalLine
        {
            //	Force un trait horizontal.
            //	Si IsHorizontalLine=false et IsVerticalLine=false, tout le rectangle est dessiné.
            get { return this.isHorizontalLine; }
            set
            {
                if (this.isHorizontalLine != value)
                {
                    this.isHorizontalLine = value;
                }
            }
        }

        public bool IsVerticalLine
        {
            //	Force un trait vertical.
            //	Si IsHorizontalLine=false et IsVerticalLine=false, tout le rectangle est dessiné.
            get { return this.isVerticalLine; }
            set
            {
                if (this.isVerticalLine != value)
                {
                    this.isVerticalLine = value;
                }
            }
        }

        protected override void PaintBackgroundImplementation(
            Drawing.Graphics graphics,
            Drawing.Rectangle clipRect
        )
        {
            if (!this.IsEnabled || this.alpha == 0)
                return;

            IAdorner adorner = Widgets.Adorners.Factory.Active;
            Drawing.Rectangle rect = this.Client.Bounds;
            double w = this.DrawFrameWidth;

            if (this.isHorizontalLine)
            {
                rect.Bottom = System.Math.Floor(rect.Center.Y) - (w - 1) / 2;
                rect.Height = w;
            }

            if (this.isVerticalLine)
            {
                rect.Left = System.Math.Floor(rect.Center.X) - (w - 1) / 2;
                rect.Width = w;
            }

            graphics.AddFilledRectangle(rect);

            Drawing.Color color = adorner.ColorBorder;
            if (!this.color.IsEmpty)
            {
                color = this.color;
            }

            graphics.RenderSolid(Drawing.Color.FromAlphaColor(this.alpha, color));
        }

        protected Drawing.Color color = Drawing.Color.Empty;
        protected double alpha = 1.0;
        protected bool isHorizontalLine = false;
        protected bool isVerticalLine = false;
    }
}
