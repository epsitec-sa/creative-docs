//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.BigList.Renderers
{
	public class MarkRenderer : IItemMarkRenderer
	{
		public MarkRenderer()
		{
			this.markColor = Color.FromName ("Orange");
		}

		#region IItemMarkRenderer Members

		public void Render(ItemListMark mark, ItemListMarkOffset offset, Graphics graphics, Rectangle bounds)
		{
			graphics.AddFilledRectangle (bounds);
			graphics.RenderSolid (this.markColor);
		}

		#endregion

		private readonly Color markColor;
	}
}
