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
    /// La classe VMenu implémente le menu vertical, utilisé pour tous les
    /// menus et sous-menus (sauf le menu horizontal, évidemment).
    /// </summary>
    public class VMenu : AbstractMenu
    {
        public VMenu()
            : base() { }

        public VMenu(Widget embedder)
            : this()
        {
            this.SetEmbedder(embedder);
        }

        public override MenuOrientation MenuOrientation
        {
            get { return MenuOrientation.Vertical; }
        }

        protected override void PaintBackgroundImplementation(
            Epsitec.Common.Drawing.Graphics graphics,
            Epsitec.Common.Drawing.Rectangle clipRect
        )
        {
            base.PaintBackgroundImplementation(graphics, clipRect);

            IAdorner adorner = Widgets.Adorners.Factory.Active;

            Drawing.Rectangle rect = this.Client.Bounds;
            WidgetPaintState state = this.GetPaintState();

            double iw = (this.IconWidth > 10) ? this.IconWidth + 3 : 0;
            adorner.PaintMenuBackground(
                graphics,
                rect,
                state,
                Direction.Down,
                Drawing.Rectangle.Empty,
                iw
            );
        }
    }
}
