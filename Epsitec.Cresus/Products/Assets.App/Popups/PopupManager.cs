//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

namespace Epsitec.Cresus.Assets.App.Popups
{
	public static class PopupManager
	{
		public static AbstractPopup CreatePopup(Widget target)
		{
			var parent = PopupManager.GetParent (target);

			var popup = new NewEventPopup ()
			{
				Parent = parent,
				Anchor = AnchorStyles.All,
				Name   = "PopupWidget",
			};

			var r1 = parent.MapClientToScreen (parent.ActualBounds);
			var r2 = target.MapClientToScreen (new Rectangle (0, 0, target.ActualWidth, target.ActualHeight));

			var x = r2.Left - r1.Left;
			var y = r2.Bottom - r1.Bottom;

			var targetRect = new Rectangle (x, y, target.ActualWidth, target.ActualHeight);

			//popup.Initialize (targetRect);

			return popup;
		}

		public static Widget GetParent(Widget widget)
		{
			Widget parent = widget;

			while (true)
			{
				if (parent.Name == "PopupParentFrame")
				{
					return parent;
				}

				parent = parent.Parent;
			}
		}
	}
}