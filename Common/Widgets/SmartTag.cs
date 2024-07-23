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


using Epsitec.Common.Support;

namespace Epsitec.Common.Widgets
{
    /// <summary>
    /// La classe SmartTag représente un Tag avec un menu associé.
    /// </summary>
    public class SmartTag : Tag
    {
        public SmartTag()
            : this(null, null) { }

        public SmartTag(Widget embedder)
            : this()
        {
            this.SetEmbedder(embedder);
        }

        public SmartTag(string command)
            : this(command, null) { }

        public SmartTag(string command, string name)
            : base(command, name) { }

        public VMenu Menu
        {
            get { return this.menu; }
            set { this.menu = value; }
        }

        public object Context
        {
            get { return this.context; }
            set { this.context = value; }
        }

        protected virtual VMenu GetMenu()
        {
            this.OnPrepareMenu();
            return this.menu;
        }

        protected override void OnClicked(MessageEventArgs e)
        {
            base.OnClicked(e);

            Drawing.Point pos = this.MapClientToScreen(new Drawing.Point(0, this.ActualHeight));
            VMenu menu = this.GetMenu();

            menu.Host = this;
            pos.X -= menu.ActualWidth;

            menu.ShowAsContextMenu(this.Window, pos);
        }

        protected virtual void OnPrepareMenu()
        {
            var handler = this.GetUserEventHandler("PrepareMenu");
            if (handler != null)
            {
                handler(this);
            }
        }

        public event EventHandler PrepareMenu
        {
            add { this.AddUserEventHandler("PrepareMenu", value); }
            remove { this.RemoveUserEventHandler("PrepareMenu", value); }
        }

        protected VMenu menu;
        protected object context;
    }
}
