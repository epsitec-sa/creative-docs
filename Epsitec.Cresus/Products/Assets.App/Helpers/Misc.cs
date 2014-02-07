//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;

namespace Epsitec.Cresus.Assets.App.Helpers
{
	public static class Misc
	{
		public static ActiveState GetActiveState(bool state)
		{
			return state ? ActiveState.Yes : ActiveState.No;
		}


		public static string GetRichTextImg(string iconName, double verticalOffset, Size iconSize = default (Size))
		{
			return Misc.IconProvider.GetRichTextImg (iconName, verticalOffset, iconSize);
		}

		public static string GetResourceIconUri(string iconName)
		{
			return Misc.IconProvider.GetResourceIconUri (iconName);
		}


		private static readonly IconProvider IconProvider = new IconProvider ("Epsitec.Cresus.Assets.App");
	}
}
