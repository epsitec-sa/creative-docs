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
    /// La class TabButton représente un bouton d'un onglet.
    /// </summary>
    public class TabButton : AbstractButton
    {
        public TabButton()
        {
            this.InternalState &= ~WidgetInternalState.Engageable;
            this.AutoFocus = false;
        }

        public TabButton(Widget embedder)
            : this()
        {
            this.SetEmbedder(embedder);
        }

        public TabButton(Command command)
            : this()
        {
            this.CommandObject = command;
        }

        public override Drawing.Margins GetShapeMargins()
        {
            return new Drawing.Margins(2, 2, 2, 2);
        }

        protected override void PaintBackgroundImplementation(
            Drawing.Graphics graphics,
            Drawing.Rectangle clipRect
        )
        {
            //	Dessine le bouton.
            IAdorner adorner = Widgets.Adorners.Factory.Active;

            Drawing.Rectangle rect = this.Client.Bounds;
            WidgetPaintState state = this.GetPaintState();
            Drawing.Point pos = new Drawing.Point();

            Drawing.Rectangle saveClip = graphics.SaveClippingRectangle();

            TabBook tabBook = this.Parent as TabBook;
            Drawing.Rectangle frameRect;

            if (tabBook != null)
            {
                frameRect = tabBook.TabClipRectangle;
                Drawing.Rectangle localClip = tabBook.MapClientToRoot(frameRect);
                graphics.SetClippingRectangle(localClip);
            }
            else
            {
                frameRect = this.Parent.Client.Bounds;
            }

            if (this.ActiveState == ActiveState.Yes)
            {
                rect.Bottom -= 2;
                adorner.PaintTabAboveBackground(graphics, frameRect, rect, state, Direction.Up);
                pos.Y += 1;
            }
            else
            {
                rect.Top -= 2;
                adorner.PaintTabSunkenBackground(graphics, frameRect, rect, state, Direction.Up);
                pos.Y -= 1;
            }

            if (this.Text != "")
            {
                Drawing.Size size = this.Client.Size;
                size.Width -= 4;
                this.TextLayout.LayoutSize = size;
                pos.X += 2;
                adorner.PaintButtonTextLayout(
                    graphics,
                    pos,
                    this.TextLayout,
                    state,
                    ButtonStyle.Tab
                );
            }

            graphics.RestoreClippingRectangle(saveClip);
        }
    }
}
