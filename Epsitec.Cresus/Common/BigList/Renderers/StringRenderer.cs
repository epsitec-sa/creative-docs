//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.BigList.Renderers
{
	public class StringRenderer : IItemDataRenderer
	{
		public StringRenderer()
		{
			this.textFont     = Font.DefaultFont;
			this.textFontSize = Font.DefaultFontSize;
			this.textColor    = Color.FromBrightness (0);
			this.alignment    = ContentAlignment.TopLeft;
			this.lineHeight   = 18;
		}

		#region IItemDataRenderer Members

		public void Render(ItemData data, ItemState state, ItemListRow row, Graphics graphics, Rectangle bounds)
		{
			var back  = state.Selected ? Color.FromName ("Highlight") : Color.FromBrightness ((row.Index & 1) == 0 ? 1.0 : 0.9);
			var value = this.GetStringValue (data);
			var color = state.Selected ? Color.FromName ("HighlightText") : this.textColor;
			var lines = value.Split ('\n');

			graphics.AddFilledRectangle (bounds);
			graphics.RenderSolid (back);

			bounds = Rectangle.Offset (bounds, 0, -state.PaddingBefore);

			foreach (var line in lines)
			{
				graphics.AddText (bounds.X, bounds.Y, bounds.Width, bounds.Height, line, this.textFont, this.textFontSize, this.alignment);
				bounds = Rectangle.Offset (bounds, 0, -this.lineHeight);
			}

			graphics.RenderSolid (color);
		}

		#endregion

		protected virtual string GetStringValue(ItemData data)
		{
			return data.GetData<string> ();
		}


		private readonly Font textFont;
		private readonly double textFontSize;
		private readonly Color textColor;
		private readonly ContentAlignment alignment;
		private readonly int lineHeight;
	}
}
