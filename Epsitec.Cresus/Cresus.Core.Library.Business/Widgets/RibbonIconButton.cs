//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Widgets
{
	/// <summary>
	/// The <c>RibbonIconButton</c> class is a variation of an <see cref="IconButton"/>
	/// which paints its icon using a different icon style when it is active.
	/// </summary>
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
				this.PreferredIconStyle = IconStyles.Active;
			}
			else
			{
				this.PreferredIconStyle = IconStyles.Default;
			}
		}

		protected override WidgetPaintState GetPaintState()
		{
			return RibbonIconButton.MaskActiveState (base.GetPaintState ());
		}

		
		private static WidgetPaintState MaskActiveState(WidgetPaintState state)
		{
			return state & ~(WidgetPaintState.ActiveYes | WidgetPaintState.ActiveMaybe);
		}
		
		
		private static class IconStyles
		{
			public const string Default = null;
			public const string Active = "Active";
		}
	}
}
