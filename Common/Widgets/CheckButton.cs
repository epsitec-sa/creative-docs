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
    using ContentAlignment = Drawing.ContentAlignment;

    /// <summary>
    /// La classe CheckButton réalise un bouton cochable.
    /// </summary>
    public class CheckButton : AbstractButton
    {
        public CheckButton()
        {
            this.AutoToggle = true;
        }

        public CheckButton(Widget embedder)
            : this()
        {
            this.SetEmbedder(embedder);
        }

        static CheckButton()
        {
            double height = Widget.DefaultFontHeight + 1;

            Visual.ContentAlignmentProperty.OverrideMetadataDefaultValue<CheckButton>(
                ContentAlignment.MiddleLeft
            );
            Visual.PreferredHeightProperty.OverrideMetadataDefaultValue<CheckButton>(height);
        }

        public Drawing.Point LabelOffset
        {
            get { return new Drawing.Point(CheckButton.CheckWidth, 0); }
        }

        public override Drawing.Margins GetShapeMargins()
        {
            if ((this.TextLayout != null) && (this.Text.Length > 0))
            {
                Drawing.Rectangle rect = this.TextLayout.StandardRectangle;
                Drawing.Rectangle bounds = this.Client.Bounds;

                rect.Offset(this.LabelOffset);
                rect.Inflate(1, 1);

                rect.MergeWith(bounds);

                return new Drawing.Margins(
                    bounds.Left - rect.Left,
                    rect.Right - bounds.Right,
                    rect.Top - bounds.Top,
                    bounds.Bottom - rect.Bottom
                );
            }
            else
            {
                return base.GetShapeMargins();
            }
        }

        public override Drawing.Size GetBestFitSize()
        {
            Drawing.Size size =
                (this.TextLayout == null)
                    ? Drawing.Size.Empty
                    : this.TextLayout.GetSingleLineSize();

            size.Width = System.Math.Ceiling(size.Width + CheckButton.CheckWidth + 3);
            size.Height = System.Math.Max(
                System.Math.Ceiling(size.Height),
                CheckButton.CheckHeight
            );

            return size;
        }

        protected override Epsitec.Common.Drawing.Size GetTextLayoutSize()
        {
            Drawing.Size size = base.GetTextLayoutSize();
            Drawing.Point offset = this.LabelOffset;

            double dx = size.Width - offset.X;
            double dy = size.Height;

            return new Drawing.Size(dx, dy);
        }

        protected override void PaintBackgroundImplementation(
            Drawing.Graphics graphics,
            Drawing.Rectangle clipRect
        )
        {
            IAdorner adorner = Widgets.Adorners.Factory.Active;

            Drawing.Rectangle rect = new Drawing.Rectangle(
                0,
                (this.Client.Size.Height - CheckButton.CheckHeight) / 2,
                CheckButton.CheckHeight,
                CheckButton.CheckHeight
            );
            WidgetPaintState state = this.GetPaintState();

            adorner.PaintCheck(graphics, rect, state);
            adorner.PaintGeneralTextLayout(
                graphics,
                clipRect,
                this.LabelOffset,
                this.TextLayout,
                state,
                PaintTextStyle.CheckButton,
                TextFieldDisplayMode.Default,
                this.BackColor
            );
        }

        protected static readonly double CheckHeight = 13;
        protected static readonly double CheckWidth = 20;
    }
}
