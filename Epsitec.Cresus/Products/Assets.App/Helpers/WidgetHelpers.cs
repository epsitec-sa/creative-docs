//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;

namespace Epsitec.Cresus.Assets.App.Helpers
{
	public static class WidgetHelpers
	{
		public static FrameBox GetMainFrameBox(this Widget widget)
		{
			//	Retourne le widget 'root' qui occupe toute la fenêtre.
			var parent = widget.Window.Root.Children[0] as FrameBox;
			System.Diagnostics.Debug.Assert (parent != null);
			System.Diagnostics.Debug.Assert (parent.Name == "PopupParentFrame");
			return parent;
		}
	}
}
