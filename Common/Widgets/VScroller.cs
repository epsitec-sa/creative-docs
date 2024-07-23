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


[assembly: Epsitec.Common.Types.DependencyClass(typeof(Epsitec.Common.Widgets.VScroller))]

namespace Epsitec.Common.Widgets
{
    /// <summary>
    /// La classe VScroller implémente l'ascenceur vertical.
    /// </summary>
    public class VScroller : AbstractScroller
    {
        public VScroller()
            : base(true)
        {
            this.ArrowUp.Name = "Up";
            this.ArrowDown.Name = "Down";
        }

        public VScroller(Widget embedder)
            : this()
        {
            this.SetEmbedder(embedder);
        }

        static VScroller()
        {
            Types.DependencyPropertyMetadata metadataDx =
                Visual.PreferredWidthProperty.DefaultMetadata.Clone();
            Types.DependencyPropertyMetadata metadataDy =
                Visual.PreferredHeightProperty.DefaultMetadata.Clone();

            double dx = AbstractScroller.DefaultBreadth;
            double dy = AbstractScroller.minimalThumb + 6;

            metadataDx.DefineDefaultValue(dx);
            metadataDy.DefineDefaultValue(dy);

            Visual.PreferredWidthProperty.OverrideMetadata(typeof(VScroller), metadataDx);
            Visual.MinWidthProperty.OverrideMetadata(typeof(VScroller), metadataDx);
            Visual.MinHeightProperty.OverrideMetadata(typeof(VScroller), metadataDy);
        }
    }
}
