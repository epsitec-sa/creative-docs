//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;

using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Widgets
{
	public class BackIconButton : IconButton
	{
		public BackIconButton()
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
			return BackIconButton.MaskActiveState (base.GetPaintState ());
		}
		
		private static WidgetPaintState MaskActiveState(WidgetPaintState state)
		{
			return state & ~(WidgetPaintState.ActiveYes | WidgetPaintState.ActiveMaybe);
		}


		protected override void PaintBackgroundImplementation(Common.Drawing.Graphics graphics, Common.Drawing.Rectangle clipRect)
		{
			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;
			Rectangle rect = this.Client.Bounds;

			if (this.ActiveState == ActiveState.Yes && !this.BackColor.IsEmpty)
			{
				graphics.AddFilledRectangle (rect);
				graphics.RenderSolid (this.BackColor);
			}

			base.PaintBackgroundImplementation (graphics, clipRect);

			if (this.ActiveState == ActiveState.Yes && !this.BackColor.IsEmpty)
			{
				rect.Deflate (0.5);
				graphics.AddRectangle (rect);
				graphics.RenderSolid (adorner.ColorBorder);
			}
		}
		
		
		private static class IconStyles
		{
			public const string Default = null;
			public const string Active = "Active";
		}
	}
}
