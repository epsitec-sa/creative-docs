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
    /// La classe VToolBar permet de réaliser des tool bars verticales.
    /// </summary>
    public class VToolBar : AbstractToolBar
    {
        public VToolBar()
        {
            this.direction = Direction.Left;
        }

        public VToolBar(Widget embedder)
            : this()
        {
            this.SetEmbedder(embedder);
        }

        static VToolBar()
        {
            Types.DependencyPropertyMetadata metadataDx =
                Visual.PreferredWidthProperty.DefaultMetadata.Clone();
            Types.DependencyPropertyMetadata metadataPadding =
                Visual.PaddingProperty.DefaultMetadata.Clone();

            metadataDx.DefineDefaultValue(28.0);
            metadataPadding.DefineDefaultValue(new Drawing.Margins(3, 3, 3, 3));

            Visual.PreferredWidthProperty.OverrideMetadata(typeof(VToolBar), metadataDx);
            Visual.PaddingProperty.OverrideMetadata(typeof(VToolBar), metadataPadding);
        }

        public override DockStyle DefaultIconDockStyle
        {
            get { return DockStyle.Top; }
        }
    }
}
