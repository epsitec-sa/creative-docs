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


using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Widgets.Platform;

namespace Epsitec.Common.Widgets.Behaviors
{
    public sealed class SlimFieldMenuBehavior : SlimFieldBehavior, System.IDisposable
    {
        public SlimFieldMenuBehavior(SlimField host)
            : base(host)
        {
            this.host.MouseMove += this.HandleHostMouseMove;
            this.host.Entered += this.HandleHostEntered;
            this.host.Exited += this.HandleHostExited;
            this.host.Clicked += this.HandleHostClicked;
        }

        #region IDisposable Members

        public void Dispose()
        {
            this.host.MouseMove -= this.HandleHostMouseMove;
            this.host.Entered -= this.HandleHostEntered;
            this.host.Exited -= this.HandleHostExited;
            this.host.Clicked -= this.HandleHostClicked;
        }

        #endregion


        public void Select(SlimFieldMenuItem item)
        {
            bool changed = false;

            if (item.ExecuteCommand(this))
            {
                return;
            }

            if ((item != null) && (item.Active != ActiveState.Yes))
            {
                item.Active = ActiveState.Yes;
                changed = true;
            }

            foreach (var x in this.host.MenuItems)
            {
                if ((x != item) && (x.Active == ActiveState.Yes))
                {
                    x.Active = ActiveState.No;
                    changed = true;
                }
            }

            if (changed)
            {
                this.host.FieldText = item.Texts.FirstOrDefault();
                this.OnSelected();
                this.host.UpdatePreferredSize();
                this.host.Invalidate();
            }
        }

        public void Clear()
        {
            this.host.FieldText = null;
            this.host.MenuItems.ForEach(x => x.Active = ActiveState.No);
            this.host.UpdatePreferredSize();
            this.host.Invalidate();
        }

        public SlimFieldMenuItem GetSelectedItem()
        {
            return this.host.MenuItems.FirstOrDefault(x => x.Active == ActiveState.Yes);
        }

        private void OnSelected()
        {
            this.Selected.Raise(this);
        }

        private void HandleHostMouseMove(object sender, MessageEventArgs e)
        {
            if (this.host.DisplayMode == SlimFieldDisplayMode.Menu)
            {
                this.UpdateMenuItemHilite(e.Point);
            }
        }

        private void HandleHostEntered(object sender, MessageEventArgs e)
        {
            if (this.host.IsReadOnly)
            {
                return;
            }

            this.host.MinWidth = System.Math.Ceiling(
                this.host.MeasureWidth(this.host.GetActiveDisplayMode())
            );
            this.host.DisplayMode = SlimFieldDisplayMode.Menu;
            this.host.UpdatePreferredSize();
        }

        private void HandleHostExited(object sender, MessageEventArgs e)
        {
            this.host.ClearValue(Visual.MinWidthProperty);
            this.host.DisplayMode = string.IsNullOrEmpty(this.host.FieldText)
                ? SlimFieldDisplayMode.Label
                : SlimFieldDisplayMode.Text;
            this.host.UpdatePreferredSize();
            this.UpdateMenuItemHilite(null);
        }

        private void HandleHostClicked(object sender, MessageEventArgs e)
        {
            var item = this.host.MenuItems.FirstOrDefault(x =>
                x.Hilite == SlimFieldMenuItemHilite.Underline
            );

            if (item != null)
            {
                this.Select(item);
            }

            this.host.Focus();
        }

        private void UpdateMenuItemHilite(Point pos)
        {
            this.UpdateMenuItemHilite(this.host.DetectMenuItem(pos));
        }

        private void UpdateMenuItemHilite(SlimFieldMenuItem hitItem)
        {
            var changed = false;

            foreach (var item in this.host.MenuItems)
            {
                if (item.Enable != EnableState.Enabled)
                {
                    continue;
                }

                if ((item == hitItem) && (item.Hilite != SlimFieldMenuItemHilite.Underline))
                {
                    item.Hilite = SlimFieldMenuItemHilite.Underline;
                    changed = true;
                }

                if ((item != hitItem) && (item.Hilite == SlimFieldMenuItemHilite.Underline))
                {
                    item.Hilite = SlimFieldMenuItemHilite.None;
                    changed = true;
                }
            }

            if (changed)
            {
                if (hitItem == null)
                {
                    this.host.MouseCursor = MouseCursor.AsArrow;
                }
                else
                {
                    this.host.MouseCursor = MouseCursor.AsHand;
                }

                this.host.Invalidate();
            }
        }

        public event EventHandler Selected;
    }
}
