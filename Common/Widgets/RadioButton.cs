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
    /// La classe RadioButton réalise un bouton radio.
    /// </summary>
    public class RadioButton : AbstractButton
    {
        public RadioButton()
        {
            this.AutoToggle = true;
            this.AutoRadio = true;

            this.Group = "Radio";
        }

        public RadioButton(Widget embedder)
            : this()
        {
            this.SetEmbedder(embedder);
        }

        public RadioButton(Widget parent, string group, int index)
            : this()
        {
            this.SetParent(parent);
            this.Group = group;
            this.Index = index;
        }

        static RadioButton()
        {
            Types.DependencyPropertyMetadata metadataAlign =
                Visual.ContentAlignmentProperty.DefaultMetadata.Clone();
            Types.DependencyPropertyMetadata metadataDy =
                Visual.PreferredHeightProperty.DefaultMetadata.Clone();

            metadataAlign.DefineDefaultValue(Drawing.ContentAlignment.MiddleLeft);
            metadataDy.DefineDefaultValue(Widget.DefaultFontHeight + 1);

            Visual.ContentAlignmentProperty.OverrideMetadata(typeof(RadioButton), metadataAlign);
            Visual.PreferredHeightProperty.OverrideMetadata(typeof(RadioButton), metadataDy);
        }

        public Drawing.Point LabelOffset
        {
            get { return new Drawing.Point(RadioButton.RadioWidth, 0); }
        }

        public override Drawing.Margins GetShapeMargins()
        {
            if ((this.TextLayout != null) && (this.Text.Length > 0))
            {
                Drawing.Rectangle rect = this.TextLayout.StandardRectangle;
                Drawing.Rectangle bounds = this.Client.Bounds;

                rect.Offset(this.LabelOffset);
                rect.Inflate(1, 1);
                rect.Inflate(Widgets.Adorners.Factory.Active.GeometryRadioShapeMargins);

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
            if ((this.TextLayout == null) || (this.Text.Length == 0))
            {
                return new Drawing.Size(RadioButton.RadioWidth, RadioButton.RadioHeight);
            }

            Drawing.Size size = this.TextLayout.GetSingleLineSize();

            size.Width = System.Math.Ceiling(RadioButton.RadioWidth + size.Width + 3);
            size.Height = System.Math.Max(
                System.Math.Ceiling(size.Height),
                RadioButton.RadioHeight
            );

            return size;
        }

        protected override Drawing.Size GetTextLayoutSize()
        {
            Drawing.Point offset = this.LabelOffset;

            double dx = this.Client.Size.Width - offset.X;
            double dy = this.Client.Size.Height;

            return new Drawing.Size(dx, dy);
        }

        protected override void PaintBackgroundImplementation(
            Drawing.Graphics graphics,
            Drawing.Rectangle clipRect
        )
        {
            IAdorner adorner = Widgets.Adorners.Factory.Active;

            double y = (this.Client.Size.Height - RadioButton.RadioHeight) / 2;

            Drawing.Rectangle rect = new Drawing.Rectangle(
                0,
                y,
                RadioButton.RadioHeight,
                RadioButton.RadioHeight
            );
            WidgetPaintState state = this.GetPaintState();

            adorner.PaintRadio(graphics, rect, state);
            adorner.PaintGeneralTextLayout(
                graphics,
                clipRect,
                this.LabelOffset,
                this.TextLayout,
                state,
                PaintTextStyle.RadioButton,
                TextFieldDisplayMode.Default,
                this.BackColor
            );
        }

        protected static readonly double RadioHeight = 13;
        protected static readonly double RadioWidth = 20;
    }
}
