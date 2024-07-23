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


using Epsitec.Common.Widgets;

namespace Epsitec.Common.UI
{
    /// <summary>
    /// La classe ItemViewText représente du texte non éditable. Si un parent de cet objet
    /// est de type ItemViewWidget, le texte adopte l'aspect sélectionné de ce parent.
    /// Cet objet sert à afficher des textes dans l'objet UI.ItemTable.
    /// </summary>
    public class ItemViewText : StaticText
    {
        public ItemViewText() { }

        public ItemViewText(Widget embedder)
            : this()
        {
            this.SetEmbedder(embedder);
        }

        public ItemViewText(string text)
            : this()
        {
            this.Text = text;
        }

        public ItemViewText(Widget embedder, string text)
            : this(embedder)
        {
            this.Text = text;
        }

        protected override void PaintBackgroundImplementation(
            Drawing.Graphics graphics,
            Drawing.Rectangle clipRect
        )
        {
            Drawing.Rectangle rect = this.Client.Bounds;
            WidgetPaintState state = this.PaintState;
            Drawing.Point pos = Drawing.Point.Zero;

            if (this.BackColor.IsVisible)
            {
                graphics.AddFilledRectangle(rect);
                graphics.RenderSolid(this.BackColor);
            }

            if (this.TextLayout != null)
            {
                IAdorner adorner = Widgets.Adorners.Factory.Active;

                ItemViewWidget item = this.SearchItemViewParent();
                if (item != null && item.ItemView.IsSelected)
                {
                    state |= WidgetPaintState.Selected;
                }

                adorner.PaintGeneralTextLayout(
                    graphics,
                    clipRect,
                    pos,
                    this.TextLayout,
                    state,
                    this.paintTextStyle,
                    TextFieldDisplayMode.Default,
                    this.BackColor
                );
            }
        }

        protected ItemViewWidget SearchItemViewParent()
        {
            //	Retourne le premier parent de type ItemViewWidget.
            Widgets.Widget widget = this;

            while (widget.Parent != null)
            {
                widget = widget.Parent;

                if (widget is ItemViewWidget)
                {
                    return widget as ItemViewWidget;
                }
            }

            return null;
        }
    }
}
