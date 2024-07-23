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


namespace Epsitec.Common.Widgets.Collections
{
    /// <summary>
    /// La classe HostedShortcutCollection est une variante de ShortcutCollection
    /// qui avertit son "host" des changements, s'il y en a.
    /// </summary>
    public class HostedShortcutCollection : ShortcutCollection
    {
        public HostedShortcutCollection(IShortcutCollectionHost host)
        {
            this.host = host;
        }

        protected override void NotifyInsertion(Shortcut item)
        {
            if (this.host != null)
            {
                this.host.NotifyShortcutsChanged(this);
            }

            base.NotifyInsertion(item);
        }

        protected override void NotifyRemoval(Shortcut item)
        {
            if (this.host != null)
            {
                this.host.NotifyShortcutsChanged(this);
            }

            base.NotifyRemoval(item);
        }

        private IShortcutCollectionHost host;
    }
}
