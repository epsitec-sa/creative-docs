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

[assembly: Epsitec.Common.Types.DependencyClass(typeof(Epsitec.Common.Widgets.HScroller))]

namespace Epsitec.Common.Widgets
{
    /// <summary>
    /// La classe HScroller implémente l'ascenceur horizontal.
    /// </summary>
    public class HScroller : AbstractScroller
    {
        public HScroller()
            : base(false)
        {
            this.ArrowUp.Name = "Right";
            this.ArrowDown.Name = "Left";
        }

        public HScroller(Widget embedder)
            : this()
        {
            this.SetEmbedder(embedder);
        }

        static HScroller()
        {
            Types.DependencyPropertyMetadata metadataDx =
                Visual.PreferredWidthProperty.DefaultMetadata.Clone();
            Types.DependencyPropertyMetadata metadataDy =
                Visual.PreferredHeightProperty.DefaultMetadata.Clone();

            double dx = AbstractScroller.minimalThumb + 6;
            double dy = AbstractScroller.DefaultBreadth;

            metadataDx.DefineDefaultValue(dx);
            metadataDy.DefineDefaultValue(dy);

            Visual.PreferredHeightProperty.OverrideMetadata(typeof(HScroller), metadataDy);
            Visual.MinWidthProperty.OverrideMetadata(typeof(HScroller), metadataDx);
            Visual.MinHeightProperty.OverrideMetadata(typeof(HScroller), metadataDy);
        }
    }
}
