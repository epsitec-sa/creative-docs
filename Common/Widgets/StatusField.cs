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
    /// La classe StatusField représente une case d'une ligne de statuts.
    /// </summary>
    public class StatusField : Widget
    {
        public StatusField() { }

        public StatusField(Widget embedder)
            : this()
        {
            this.SetEmbedder(embedder);
        }

        public StatusField(string text)
            : this()
        {
            this.Text = text;
        }

        protected override void UpdateTextLayout()
        {
            if (this.TextLayout != null)
            {
                double dx = this.Client.Size.Width - this.margin * 2;
                double dy = this.Client.Size.Height;
                this.TextLayout.Alignment = this.ContentAlignment;
                this.TextLayout.LayoutSize = new Drawing.Size(dx, dy);
            }
        }

        static StatusField()
        {
            Types.DependencyPropertyMetadata metadataAlign =
                Visual.ContentAlignmentProperty.DefaultMetadata.Clone();
            Types.DependencyPropertyMetadata metadataDx =
                Visual.PreferredWidthProperty.DefaultMetadata.Clone();
            Types.DependencyPropertyMetadata metadataDy =
                Visual.PreferredHeightProperty.DefaultMetadata.Clone();

            metadataAlign.DefineDefaultValue(Drawing.ContentAlignment.MiddleLeft);
            metadataDx.DefineDefaultValue(120.0);
            metadataDy.DefineDefaultValue(Widget.DefaultFontHeight);

            Visual.ContentAlignmentProperty.OverrideMetadata(typeof(StatusField), metadataAlign);
            Visual.PreferredWidthProperty.OverrideMetadata(typeof(StatusField), metadataDx);
            Visual.PreferredHeightProperty.OverrideMetadata(typeof(StatusField), metadataDy);
        }

#if false	//#fix
		public override Drawing.Size PreferredSize
		{
			//	Retourne les dimensions minimales pour représenter le texte.
			get
			{
				return this.TextLayout.SingleLineSize;
			}
		}
#endif
        protected override void PaintBackgroundImplementation(
            Drawing.Graphics graphics,
            Drawing.Rectangle clipRect
        )
        {
            //	Dessine le texte.
            IAdorner adorner = Widgets.Adorners.Factory.Active;

            if (this.TextLayout == null)
                return;

            Drawing.Rectangle rect = this.Client.Bounds;
            WidgetPaintState state = this.GetPaintState();
            Drawing.Point pos = new Drawing.Point();

            if (this.BackColor.IsVisible)
            {
                graphics.AddFilledRectangle(rect);
                graphics.RenderSolid(this.BackColor);
            }

            adorner.PaintStatusItemBackground(graphics, rect, state);
            pos.X += this.margin;
            pos.Y += 0.5;
            adorner.PaintGeneralTextLayout(
                graphics,
                clipRect,
                pos,
                this.TextLayout,
                state,
                PaintTextStyle.Status,
                TextFieldDisplayMode.Default,
                this.BackColor
            );
        }

        protected double margin = 8.0;
    }
}
