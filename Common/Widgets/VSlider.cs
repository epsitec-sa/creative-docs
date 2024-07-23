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
    /// La classe VSlider implémente le potentiomètre linéaire vertical.
    /// </summary>
    public class VSlider : AbstractSlider
    {
        public VSlider()
            : base(true, true)
        {
            this.ArrowUp.Name = "Up";
            this.ArrowDown.Name = "Down";
        }

        public VSlider(Widget embedder)
            : this()
        {
            this.SetEmbedder(embedder);
        }

        static VSlider()
        {
            Types.DependencyPropertyMetadata metadataMinDx =
                Visual.MinWidthProperty.DefaultMetadata.Clone();
            Types.DependencyPropertyMetadata metadataMinDy =
                Visual.MinHeightProperty.DefaultMetadata.Clone();
            Types.DependencyPropertyMetadata metadataDx =
                Visual.PreferredWidthProperty.DefaultMetadata.Clone();

            double minDx = AbstractSlider.defaultBreadth / 2;
            double minDy = AbstractSlider.minimalThumb + 6;
            double dx = AbstractSlider.defaultBreadth;

            metadataMinDx.DefineDefaultValue(minDx);
            metadataMinDy.DefineDefaultValue(minDy);
            metadataDx.DefineDefaultValue(dx);

            Visual.MinWidthProperty.OverrideMetadata(typeof(VSlider), metadataMinDx);
            Visual.MinHeightProperty.OverrideMetadata(typeof(VScroller), metadataMinDy);
            Visual.PreferredWidthProperty.OverrideMetadata(typeof(VSlider), metadataDx);
        }
    }
}
