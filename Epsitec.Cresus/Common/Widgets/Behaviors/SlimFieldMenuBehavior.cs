//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Widgets.Behaviors
{
	public sealed class SlimFieldMenuBehavior
	{
		public SlimFieldMenuBehavior(SlimField host)
		{
			this.host = host;
			this.host.MouseMove += this.HandleHostMouseMove;
			this.host.Entered += this.HandleHostEntered;
			this.host.Exited  += this.HandleHostExited;
		}

		private void HandleHostMouseMove(object sender, MessageEventArgs e)
		{
			if (this.host.DisplayMode == SlimFieldDisplayMode.Menu)
			{
				this.UpdateMenuItemHilite (e.Point);
			}
		}

		private void HandleHostEntered(object sender, MessageEventArgs e)
		{
			this.host.DisplayMode = SlimFieldDisplayMode.Menu;
			this.host.UpdatePreferredSize ();
		}

		private void HandleHostExited(object sender, MessageEventArgs e)
		{
			this.host.DisplayMode = SlimFieldDisplayMode.Text;
			this.host.UpdatePreferredSize ();
			this.UpdateMenuItemHilite (null);
		}

		private void UpdateMenuItemHilite(Point pos)
		{
			this.UpdateMenuItemHilite (this.host.DetectMenuItem (pos));
		}

		private void UpdateMenuItemHilite(SlimFieldMenuItem hitItem)
		{
			var changed = false;

			foreach (var item in this.host.MenuItems)
			{
				if ((item == hitItem) &&
					(item.Hilite != SlimFieldMenuItemHilite.Underline))
				{
					item.Hilite = SlimFieldMenuItemHilite.Underline;
					changed = true;
				}
				
				if ((item != hitItem) &&
					(item.Hilite == SlimFieldMenuItemHilite.Underline))
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

				this.host.Invalidate ();
			}
		}



		private readonly SlimField host;
	}
}
