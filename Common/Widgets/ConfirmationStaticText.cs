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
    /// La class ConfirmationStaticText représente un bouton pour le dialogue ConfirmationDialog.
    /// </summary>
    public class ConfirmationStaticText : StaticText
    {
        public ConfirmationStaticText()
        {
            this.ContentAlignment = Drawing.ContentAlignment.MiddleLeft;
        }

        public ConfirmationStaticText(string text)
        {
            this.Text = text;
        }

        public ConfirmationStaticText(Widget embedder)
            : this()
        {
            this.SetEmbedder(embedder);
        }

        protected override Drawing.Size GetTextLayoutSize()
        {
            Drawing.Size size = this.IsActualGeometryValid ? this.Client.Size : this.PreferredSize;
            size.Width -= ConfirmationStaticText.marginX * 2;
            size.Height -= ConfirmationStaticText.marginY * 2;
            return size;
        }

        protected override Drawing.Point GetTextLayoutOffset()
        {
            return new Drawing.Point(
                ConfirmationStaticText.marginX,
                ConfirmationStaticText.marginY
            );
        }

        protected override void OnSizeChanged(Drawing.Size oldValue, Drawing.Size newValue)
        {
            base.OnSizeChanged(oldValue, newValue);

            if (oldValue.Width != newValue.Width) // largeur changée ?
            {
                double h = this.TextLayout.FindTextHeight();
                this.PreferredHeight = h + ConfirmationStaticText.marginY * 2 + 2; // TODO: pourquoi +2 ?
            }
        }

        protected static readonly double marginX = 0;
        protected static readonly double marginY = 0;
    }
}
