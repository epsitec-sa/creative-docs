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

namespace Epsitec.Common.BigList.Renderers
{
    public class StringRenderer : IItemDataRenderer
    {
        public StringRenderer()
        {
            this.textFont = Font.DefaultFont;
            this.textFontSize = Font.DefaultFontSize;
            this.textColor = Color.FromBrightness(0);
            this.alignment = ContentAlignment.TopLeft;
            this.lineHeight = 18;
        }

        public bool AlternateBackgroundColor { get; set; }

        #region IItemDataRenderer Members

        public void Render(
            ItemData data,
            ItemState state,
            ItemListRow row,
            Graphics graphics,
            Rectangle bounds
        )
        {
            var back = state.Selected ? Color.FromName("Highlight") : this.GetRowColor(row);
            var value = this.GetStringValue(data);
            var color = state.Selected ? Color.FromName("HighlightText") : this.textColor;
            var lines = value.Split('\n');

            graphics.AddFilledRectangle(bounds);
            graphics.RenderSolid(back);

            bounds = Rectangle.Offset(bounds, 0, -state.PaddingBefore);

            foreach (var line in lines)
            {
                graphics.AddText(
                    bounds.X,
                    bounds.Y,
                    bounds.Width,
                    bounds.Height,
                    line,
                    this.textFont,
                    this.textFontSize,
                    this.alignment
                );
                bounds = Rectangle.Offset(bounds, 0, -this.lineHeight);
            }

            graphics.RenderSolid(color);
        }

        #endregion

        protected virtual string GetStringValue(ItemData data)
        {
            return data.GetData<string>();
        }

        protected virtual Color GetRowColor(ItemListRow row)
        {
            return Color.FromBrightness(this.IsEvenRow(row) ? 1.0 : 0.9);
        }

        private bool IsEvenRow(ItemListRow row)
        {
            if (this.AlternateBackgroundColor)
            {
                return row.IsEven;
            }
            else
            {
                return true;
            }
        }

        private readonly Font textFont;
        private readonly double textFontSize;
        private readonly Color textColor;
        private readonly ContentAlignment alignment;
        private readonly int lineHeight;
    }
}
