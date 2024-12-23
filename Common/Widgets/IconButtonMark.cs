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

namespace Epsitec.Common.Widgets
{
    /// <summary>
    /// La classe IconButtonMark est un IconButton avec une marque triangulaire sur un côté.
    /// </summary>
    public class IconButtonMark : IconButton
    {
        public IconButtonMark() { }

        public IconButtonMark(Widget embedder)
            : this()
        {
            this.SetEmbedder(embedder);
        }

        public ButtonMarkDisposition MarkDisposition
        {
            //	Emplacement de la marque.
            get { return this.markDisposition; }
            set
            {
                if (this.markDisposition != value)
                {
                    this.markDisposition = value;
                    this.Invalidate();
                }
            }
        }

        public double MarkLength
        {
            //	Dimension de la marque.
            get { return this.markLength; }
            set
            {
                if (this.markLength != value)
                {
                    this.markLength = value;
                    this.Invalidate();
                }
            }
        }

        public Color BulletColor
        {
            //	Couleur de la puce éventuelle (si différent de Color.Empty).
            get { return this.bulletColor; }
            set
            {
                if (this.bulletColor != value)
                {
                    this.bulletColor = value;
                    this.Invalidate();
                }
            }
        }

        public static Rectangle GetFrameBounds(
            Rectangle bounds,
            ButtonMarkDisposition markDisposition,
            double markLength
        )
        {
            switch (markDisposition)
            {
                case ButtonMarkDisposition.Below:
                    bounds.Bottom += markLength;
                    break;

                case ButtonMarkDisposition.Above:
                    bounds.Top -= markLength;
                    break;

                case ButtonMarkDisposition.Left:
                    bounds.Left += markLength;
                    break;

                case ButtonMarkDisposition.Right:
                    bounds.Right -= markLength;
                    break;
            }

            return bounds;
        }

        protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
        {
            //	Dessine le bouton.
            IAdorner adorner = Common.Widgets.Adorners.Factory.Active;

            Rectangle rect = this.Client.Bounds;
            WidgetPaintState state = this.GetPaintState();

            bool enable = ((state & WidgetPaintState.Enabled) != 0);
            if (!enable)
            {
                state &= ~WidgetPaintState.Focused;
                state &= ~WidgetPaintState.Entered;
                state &= ~WidgetPaintState.Engaged;
            }

            if (this.ActiveState == ActiveState.Yes) // dessine la marque triangulaire ?
            {
                adorner.PaintButtonMark(
                    graphics,
                    rect,
                    state,
                    this.markDisposition,
                    this.markLength
                );
            }

            rect = IconButtonMark.GetFrameBounds(rect, this.markDisposition, this.markLength);
            state &= ~WidgetPaintState.Selected;
            adorner.PaintButtonBackground(graphics, rect, state, Direction.Down, this.ButtonStyle);
            adorner.PaintButtonBullet(graphics, rect, state, this.bulletColor);

            if (!this.bulletColor.IsEmpty)
            {
                rect.Left += rect.Height;
            }

            if (this.innerZoom != 1.0)
            {
                double zoom = (this.innerZoom - 1) / 2 + 1;
                this.TextLayout.LayoutSize = rect.Size / this.innerZoom;
                Drawing.Transform transform = graphics.Transform;
                graphics.ScaleTransform(zoom, zoom, 0, -this.Client.Size.Height * zoom);
                adorner.PaintButtonTextLayout(
                    graphics,
                    rect.BottomLeft,
                    this.TextLayout,
                    state,
                    this.ButtonStyle
                );
                graphics.Transform = transform;
            }
            else
            {
                this.TextLayout.LayoutSize = rect.Size;
                adorner.PaintButtonTextLayout(
                    graphics,
                    rect.BottomLeft,
                    this.TextLayout,
                    state,
                    this.ButtonStyle
                );
            }
        }

        protected ButtonMarkDisposition markDisposition = ButtonMarkDisposition.Below;
        protected double markLength = 8;
        protected Color bulletColor = Drawing.Color.Empty;
    }
}
