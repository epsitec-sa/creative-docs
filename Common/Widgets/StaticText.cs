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


[assembly: Epsitec.Common.Types.DependencyClass(typeof(Epsitec.Common.Widgets.StaticText))]

namespace Epsitec.Common.Widgets
{
    /// <summary>
    /// La classe StaticText représente du texte non éditable. Ce texte
    /// peut comprendre des images et des liens hypertexte.
    /// </summary>
    public class StaticText : Widget
    {
        public StaticText() { }

        public StaticText(Widget embedder)
            : this()
        {
            this.SetEmbedder(embedder);
        }

        public StaticText(string text)
            : this()
        {
            this.Text = text;
        }

        public StaticText(Widget embedder, string text)
            : this(embedder)
        {
            this.Text = text;
        }

        static StaticText()
        {
            Types.DependencyPropertyMetadata metadataAlign =
                Visual.ContentAlignmentProperty.DefaultMetadata.Clone();
            Types.DependencyPropertyMetadata metadataDy =
                Visual.PreferredHeightProperty.DefaultMetadata.Clone();

            metadataAlign.DefineDefaultValue(Drawing.ContentAlignment.MiddleLeft);
            metadataDy.DefineDefaultValue(Widget.DefaultFontHeight);

            Visual.ContentAlignmentProperty.OverrideMetadata(typeof(StaticText), metadataAlign);
            Visual.PreferredHeightProperty.OverrideMetadata(typeof(StaticText), metadataDy);
        }

        public PaintTextStyle PaintTextStyle
        {
            get { return this.paintTextStyle; }
            set
            {
                if (this.paintTextStyle != value)
                {
                    this.paintTextStyle = value;
                    this.Invalidate();
                }
            }
        }

        public override Drawing.Size GetBestFitSize()
        {
            return StaticText.GetTextBestFitSize(this);
        }

        public static Drawing.Size GetTextBestFitSize(Widget widget)
        {
            TextLayout textLayout = widget.TextLayout;

            if (textLayout != null)
            {
                return StaticText.GetTextBestFitSize(textLayout);
            }
            else
            {
                return new Drawing.Size(
                    0,
                    Drawing.TextStyle.Default.Font.LineHeight * Drawing.TextStyle.Default.FontSize
                );
            }
        }

        public static Drawing.Size GetTextBestFitSize(TextLayout textLayout)
        {
            Drawing.Size size = textLayout.GetSingleLineSize();

            size.Width = System.Math.Ceiling(size.Width);
            size.Height = System.Math.Ceiling(size.Height);

            return size;
        }

        protected override void PaintBackgroundImplementation(
            Drawing.Graphics graphics,
            Drawing.Rectangle clipRect
        )
        {
            Drawing.Rectangle rect = this.Client.Bounds;
            WidgetPaintState state = this.GetPaintState();
            Drawing.Point pos = Drawing.Point.Zero;

            if (this.BackColor.IsVisible)
            {
                graphics.AddFilledRectangle(rect);
                graphics.RenderSolid(this.BackColor);
            }

            if (this.TextLayout != null)
            {
                IAdorner adorner = Widgets.Adorners.Factory.Active;
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

            base.PaintBackgroundImplementation(graphics, clipRect);
        }

        protected PaintTextStyle paintTextStyle = PaintTextStyle.StaticText;
    }
}
