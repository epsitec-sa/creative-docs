//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Widgets
{
	public class RibbonIconButton : IconButton
	{
		public RibbonIconButton()
		{
		}

		protected override void OnActiveStateChanged()
		{
			base.OnActiveStateChanged ();

			if (this.ActiveState == Common.Widgets.ActiveState.Yes)
			{
				this.PreferredIconStyle = "Active";
			}
			else
			{
				this.PreferredIconStyle = null;
			}
		}

		protected override WidgetPaintState GetPaintState()
		{
			return base.GetPaintState () & ~ (WidgetPaintState.ActiveYes | WidgetPaintState.ActiveMaybe);
		}
	}
}
