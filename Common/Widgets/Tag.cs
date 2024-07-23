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
    /// La classe Tag implémente une petite étiquette (pastille) qui peut servir
    /// à l'implémentation de "smart tags".
    /// </summary>
    public class Tag : GlyphButton
    {
        public Tag()
            : this(null, null) { }

        public Tag(Widget embedder)
            : this()
        {
            this.SetEmbedder(embedder);
        }

        public Tag(string command)
            : this(command, null) { }

        public Tag(string command, string name)
            : base(command, name)
        {
            this.ButtonStyle = ButtonStyle.Flat;
            this.GlyphShape = GlyphShape.Menu;
            this.ResetDefaultColors();
        }

        static Tag()
        {
            Types.DependencyPropertyMetadata metadataDx =
                Visual.PreferredWidthProperty.DefaultMetadata.Clone();
            Types.DependencyPropertyMetadata metadataDy =
                Visual.PreferredHeightProperty.DefaultMetadata.Clone();

            metadataDx.DefineDefaultValue(18.0);
            metadataDy.DefineDefaultValue(18.0);

            Visual.PreferredWidthProperty.OverrideMetadata(typeof(Tag), metadataDx);
            Visual.PreferredHeightProperty.OverrideMetadata(typeof(Tag), metadataDy);
        }

        public Drawing.Color Color
        {
            get { return this.color; }
            set
            {
                if (this.color != value)
                {
                    this.color = value;
                    this.Invalidate();
                }
            }
        }

        public Direction Direction
        {
            get { return this.direction; }
            set
            {
                if (this.direction != value)
                {
                    this.direction = value;
                    this.Invalidate();
                }
            }
        }

        public void ResetDefaultColors()
        {
            this.BackColor = Drawing.Color.Empty;
            this.Color = Drawing.Color.Empty;
        }

        protected override void PaintBackgroundImplementation(
            Drawing.Graphics graphics,
            Drawing.Rectangle clipRect
        )
        {
            IAdorner adorner = Widgets.Adorners.Factory.Active;
            Drawing.Rectangle rect = this.Client.Bounds;
            WidgetPaintState state = this.GetPaintState();

            adorner.PaintTagBackground(graphics, rect, state, this.color, this.direction);
            adorner.PaintGlyph(graphics, rect, state, this.GlyphShape, PaintTextStyle.Button);
        }

        private Drawing.Color color;
        private Direction direction;
    }
}
