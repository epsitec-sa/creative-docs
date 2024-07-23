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
    /// La classe IconSeparator permet de dessiner des séparations utiles
    /// pour remplir une ToolBar.
    /// </summary>
    public class IconSeparator : Button
    {
        public IconSeparator() { }

        public IconSeparator(Widget embedder)
            : this()
        {
            this.SetEmbedder(embedder);
        }

        public IconSeparator(double breadth)
        {
            this.Breadth = breadth;
        }

        public IconSeparator(DockStyle dock)
        {
            this.Dock = dock;
        }

        static IconSeparator()
        {
            Types.DependencyPropertyMetadata metadataDx =
                Visual.PreferredWidthProperty.DefaultMetadata.Clone();
            Types.DependencyPropertyMetadata metadataDy =
                Visual.PreferredHeightProperty.DefaultMetadata.Clone();

            metadataDx.DefineDefaultValue(12.0);
            metadataDy.DefineDefaultValue(12.0);

            Visual.PreferredWidthProperty.OverrideMetadata(typeof(IconSeparator), metadataDx);
            Visual.PreferredHeightProperty.OverrideMetadata(typeof(IconSeparator), metadataDy);
        }

        public bool IsHorizontal
        {
            //	Si IsHorizontal=true, on est dans une ToolBar horizontale, et le trait de séparation
            //	est donc vertical. Mais attention, selon l'adorner utilisé, le trait de séparation
            //	n'est pas dessiné !
            get { return this.isHorizontal; }
            set
            {
                if (this.isHorizontal != value)
                {
                    this.isHorizontal = value;
                }
            }
        }

        public double Breadth
        {
            get { return this.breadth; }
            set
            {
                if (this.breadth != value)
                {
                    this.breadth = value;
                    this.PreferredWidth = value;
                    this.PreferredHeight = value;
                }
            }
        }

        protected override void PaintBackgroundImplementation(
            Drawing.Graphics graphics,
            Drawing.Rectangle clipRect
        )
        {
            IAdorner adorner = Widgets.Adorners.Factory.Active;

            Drawing.Rectangle rect = this.Client.Bounds;
            WidgetPaintState state = this.GetPaintState();

            if (this.isHorizontal)
            {
                adorner.PaintSeparatorBackground(graphics, rect, state, Direction.Right, true);
            }
            else
            {
                adorner.PaintSeparatorBackground(graphics, rect, state, Direction.Down, true);
            }
        }

        protected double breadth = 12;
        protected bool isHorizontal = true;
    }
}
