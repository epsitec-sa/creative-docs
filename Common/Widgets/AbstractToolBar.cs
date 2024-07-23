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
    /// La classe AbstractToolBar implémente ce qui est commun à HToolBar et à VToolBar.
    /// </summary>
    public abstract class AbstractToolBar : Widget, Collections.IWidgetCollectionHost<Widget>
    {
        public AbstractToolBar()
        {
            this.iconDockStyle = this.DefaultIconDockStyle;

            this.items = new Collections.WidgetCollection<Widget>(this);
        }

        public Collections.WidgetCollection<Widget> Items
        {
            get { return this.items; }
        }

        public abstract DockStyle DefaultIconDockStyle { get; }

        public DockStyle OppositeIconDockStyle
        {
            get { return AbstractToolBar.GetOpposite(this.DefaultIconDockStyle); }
        }

        public static DockStyle GetOpposite(DockStyle style)
        {
            switch (style)
            {
                case DockStyle.Left:
                    style = DockStyle.Right;
                    break;
                case DockStyle.Right:
                    style = DockStyle.Left;
                    break;
                case DockStyle.Top:
                    style = DockStyle.Bottom;
                    break;
                case DockStyle.Bottom:
                    style = DockStyle.Top;
                    break;
            }

            return style;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Widget[] items = new Widget[this.items.Count];

                this.items.CopyTo(items, 0);
                this.items.Clear();

                for (int i = 0; i < items.Length; i++)
                {
                    items[i].Dispose();
                }

                this.items = null;
            }

            base.Dispose(disposing);
        }

        protected override void PaintBackgroundImplementation(
            Drawing.Graphics graphics,
            Drawing.Rectangle clipRect
        )
        {
            //	Dessine la barre.
            IAdorner adorner = Widgets.Adorners.Factory.Active;

            Drawing.Rectangle rect = this.Client.Bounds;
            adorner.PaintToolBackground(graphics, rect, WidgetPaintState.None, this.direction);
        }

        #region IWidgetCollectionHost Members
        Collections.WidgetCollection<Widget> Collections.IWidgetCollectionHost<Widget>.GetWidgetCollection()
        {
            return this.Items;
        }

        public void NotifyInsertion(Widget widget)
        {
            if (widget.Dock == DockStyle.None)
            {
                widget.Dock = this.iconDockStyle;
            }

            widget.AutoFocus = false;
            widget.SetEmbedder(this);
            this.OnItemsChanged();
        }

        public void NotifyRemoval(Widget widget)
        {
            this.Children.Remove(widget);
        }

        public void NotifyPostRemoval(Widget widget)
        {
            this.OnItemsChanged();
        }
        #endregion

        protected virtual void OnItemsChanged()
        {
            var handler = this.GetUserEventHandler("ItemsChanged");
            if (handler != null)
            {
                handler(this);
            }
        }

        public event EventHandler ItemsChanged
        {
            add { this.AddUserEventHandler("ItemsChanged", value); }
            remove { this.RemoveUserEventHandler("ItemsChanged", value); }
        }

        private DockStyle iconDockStyle;
        protected Direction direction;
        protected Collections.WidgetCollection<Widget> items;
    }
}
