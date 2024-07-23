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
    /// La classe HSlider implémente le potentiomètre linéaire horizontal.
    /// </summary>
    public class HSlider : AbstractSlider
    {
        public HSlider()
            : base(false, true)
        {
            this.ArrowUp.Name = "Right";
            this.ArrowDown.Name = "Left";
        }

        public HSlider(Widget embedder)
            : this()
        {
            this.SetEmbedder(embedder);
        }

        static HSlider()
        {
            Types.DependencyPropertyMetadata metadataMinDx =
                Visual.MinWidthProperty.DefaultMetadata.Clone();
            Types.DependencyPropertyMetadata metadataMinDy =
                Visual.MinHeightProperty.DefaultMetadata.Clone();
            Types.DependencyPropertyMetadata metadataDy =
                Visual.PreferredHeightProperty.DefaultMetadata.Clone();

            double minDx = AbstractSlider.minimalThumb + 6;
            double minDy = AbstractSlider.defaultBreadth / 2;
            double dy = AbstractSlider.defaultBreadth;

            metadataMinDx.DefineDefaultValue(minDx);
            metadataMinDy.DefineDefaultValue(minDy);
            metadataDy.DefineDefaultValue(dy);

            Visual.MinWidthProperty.OverrideMetadata(typeof(HSlider), metadataMinDx);
            Visual.MinHeightProperty.OverrideMetadata(typeof(HSlider), metadataMinDy);
            Visual.PreferredHeightProperty.OverrideMetadata(typeof(HSlider), metadataDy);
        }
    }
}
