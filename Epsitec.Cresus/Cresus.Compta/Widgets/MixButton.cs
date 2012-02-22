//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Widgets
{
	public class MixButton : Button
	{
		public MixButton()
		{
			this.ButtonStyle = Common.Widgets.ButtonStyle.ToolItem;
		}

		
		protected override void PaintBackgroundImplementation(Common.Drawing.Graphics graphics, Common.Drawing.Rectangle clipRect)
		{
			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;
			Rectangle rect = this.Client.Bounds;

			if (this.ActiveState == ActiveState.Yes && this.Enable && !this.BackColor.IsEmpty)
			{
				graphics.AddFilledRectangle (rect);
				graphics.RenderSolid (this.BackColor);
			}

			base.PaintBackgroundImplementation (graphics, clipRect);

			if (this.ActiveState == ActiveState.Yes && this.Enable && !this.BackColor.IsEmpty)
			{
				rect.Deflate (0.5);
				graphics.AddRectangle (rect);
				graphics.RenderSolid (adorner.ColorBorder);
			}
		}
	}
}
