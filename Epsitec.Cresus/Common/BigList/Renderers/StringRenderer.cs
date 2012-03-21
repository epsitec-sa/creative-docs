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
		}

		#region IItemDataRenderer Members

		public void Render(ItemData data, Graphics graphics, Rectangle bounds)
		{
			var value = this.GetStringValue (data);

			graphics.AddText (bounds.X, bounds.Y, bounds.Width, bounds.Height, value, this.textFont, this.textFontSize, this.alignment);
			graphics.RenderSolid (this.textColor);
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
	}
}
