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
    /// La classe HToolBar permet de réaliser des tool bars horizontales.
    /// </summary>
    public class HToolBar : AbstractToolBar
    {
        public HToolBar()
        {
            this.direction = Direction.Up;
        }

        public HToolBar(Widget embedder)
            : this()
        {
            this.SetEmbedder(embedder);
        }

        static HToolBar()
        {
            Types.DependencyPropertyMetadata metadataDy =
                Visual.PreferredHeightProperty.DefaultMetadata.Clone();
            Types.DependencyPropertyMetadata metadataPadding =
                Visual.PaddingProperty.DefaultMetadata.Clone();

            metadataDy.DefineDefaultValue(28.0);
            metadataPadding.DefineDefaultValue(new Drawing.Margins(3, 3, 3, 3));

            Visual.PreferredHeightProperty.OverrideMetadata(typeof(HToolBar), metadataDy);
            Visual.PaddingProperty.OverrideMetadata(typeof(HToolBar), metadataPadding);
        }

        public override DockStyle DefaultIconDockStyle
        {
            get { return DockStyle.Left; }
        }
    }
}
