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


using Epsitec.Common.Widgets;

namespace Epsitec.Common.UI
{
    /// <summary>
    /// La classe AbstractPanel est la base de toutes les classes XyzPanel.
    /// </summary>
    public abstract class ObsoleteAbstractPanel : System.IDisposable
    {
        public ObsoleteAbstractPanel() { }

        public Drawing.Size Size
        {
            get { return this.size; }
        }

        public Widget Widget
        {
            get
            {
                if (this.widget == null)
                {
                    this.CreateWidget();
                }

                return this.widget;
            }
        }

        #region IDisposable Members
        public void Dispose()
        {
            this.Dispose(true);
            System.GC.SuppressFinalize(this);
        }
        #endregion

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.widget.Dispose();

                this.widget = null;
            }
        }

        protected virtual void CreateWidget()
        {
            this.widget = new Widget();

            this.widget.PreferredSize = this.Size;
            this.widget.MinSize = this.Size;
            this.widget.TabNavigationMode = TabNavigationMode.ForwardTabPassive;

            this.CreateWidgets(this.widget);
        }

        protected abstract void CreateWidgets(Widget parent);

        protected Drawing.Size size;
        protected Widget widget;
    }
}
