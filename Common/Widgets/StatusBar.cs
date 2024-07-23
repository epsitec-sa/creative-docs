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
    /// La classe StatusBar représente une barre de statuts en bas d'une fenêtre.
    /// </summary>
    public class StatusBar : Widget, Collections.IWidgetCollectionHost<Widget>
    {
        public StatusBar()
        {
            this.items = new Collections.WidgetCollection<Widget>(this);
            this.Padding = new Drawing.Margins(1, 1, 2, 1);
        }

        public StatusBar(Widget embedder)
            : this()
        {
            this.SetEmbedder(embedder);
        }

        static StatusBar()
        {
            Types.DependencyPropertyMetadata metadataDy =
                Visual.PreferredHeightProperty.DefaultMetadata.Clone();

            metadataDy.DefineDefaultValue(Widget.DefaultFontHeight + 6);

            Visual.PreferredHeightProperty.OverrideMetadata(typeof(StatusBar), metadataDy);
        }

        public Collections.WidgetCollection<Widget> Items
        {
            get { return this.items; }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Widget[] items = new Widget[this.items.Count];
                this.items.CopyTo(items, 0);
                this.items.Clear();

                foreach (Widget item in items)
                {
                    item.Dispose();
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
            IAdorner adorner = Widgets.Adorners.Factory.Active;

            Drawing.Rectangle rect = this.Client.Bounds;
            WidgetPaintState state = this.GetPaintState();
            adorner.PaintStatusBackground(graphics, rect, state);
        }

        #region IWidgetCollectionHost Members
        Collections.WidgetCollection<Widget> Collections.IWidgetCollectionHost<Widget>.GetWidgetCollection()
        {
            return this.Items;
        }

        public void NotifyInsertion(Widget widget)
        {
            widget.Dock = DockStyle.Left;
            widget.AutoFocus = false;
            widget.SetEmbedder(this);
        }

        public void NotifyRemoval(Widget widget)
        {
            this.Children.Remove(widget);
        }

        public void NotifyPostRemoval(Widget widget) { }
        #endregion

        protected Collections.WidgetCollection<Widget> items;
    }
}
