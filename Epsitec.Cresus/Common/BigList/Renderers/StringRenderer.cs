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

		public void Render(ItemState state, ItemData data, Graphics graphics, Rectangle bounds)
		{
			var value = this.GetStringValue (data);
			var color = state.Selected ? Color.FromName ("HighlightText") : this.textColor;

			var lines = value.Split ('\n');

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
