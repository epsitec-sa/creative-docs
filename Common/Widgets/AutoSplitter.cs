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
    public class AutoSplitter : AbstractSplitter
    {
        public AutoSplitter() { }

        public AutoSplitter(Widget embedder)
            : this()
        {
            this.SetEmbedder(embedder);
        }

        static AutoSplitter()
        {
            var metadataWidth = Visual.PreferredWidthProperty.DefaultMetadata.Clone();
            var metadataHeight = Visual.PreferredHeightProperty.DefaultMetadata.Clone();

            metadataWidth.DefineDefaultValue(4.0);
            metadataHeight.DefineDefaultValue(4.0);

            Visual.PreferredWidthProperty.OverrideMetadata(typeof(AutoSplitter), metadataWidth);
            Visual.PreferredHeightProperty.OverrideMetadata(typeof(AutoSplitter), metadataHeight);
        }

        public override bool IsVertical
        {
            get
            {
                switch (this.Dock)
                {
                    case DockStyle.Left:
                    case DockStyle.Right:
                        return true;

                    case DockStyle.Top:
                    case DockStyle.Bottom:
                        return false;

                    default:
                        System.Diagnostics.Debug.WriteLine(
                            "IsVertical cannot derive its orientation based on the docking info"
                        );
                        return false;
                }
            }
        }
    }
}
