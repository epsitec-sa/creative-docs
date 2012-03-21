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
		#region IItemDataRenderer Members

		public void Render(ItemData data, Graphics graphics, Rectangle bounds)
		{
			var value = this.GetStringValue (data);

			graphics.AddText (bounds.X, bounds.Y, bounds.Width, bounds.Height, value, Font.DefaultFont, Font.DefaultFontSize, ContentAlignment.TopLeft);
			graphics.RenderSolid (Color.FromBrightness (0));
		}

		#endregion

		protected virtual string GetStringValue(ItemData data)
		{
			return data.GetData<string> ();
		}
	}

	public class StringRenderer<TData> : StringRenderer
	{
		public StringRenderer(System.Func<TData, string> converter)
		{
			this.converter = converter;
		}

		public StringRenderer(System.Func<TData, FormattedText> converter)
		{
			this.converter = x => converter (x).ToSimpleText ();
		}

		protected override string GetStringValue(ItemData data)
		{
			return this.converter (data.GetData<TData> ());
		}


		private readonly System.Func<TData, string> converter;
	}
}
