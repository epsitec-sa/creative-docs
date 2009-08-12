//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Graph.Widgets
{
	public class ChartView : FrameBox
	{
		public ChartView()
		{
			this.items = new List<Data.ChartSeries> ();
		}


		public IList<Data.ChartSeries> Items
		{
			get
			{
				return this.items;
			}
		}


		public void DefineRenderer(Renderers.AbstractRenderer renderer)
		{
			this.renderer = renderer;
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			graphics.AddFilledRectangle (this.Client.Bounds);
			graphics.SolidRenderer.Clear (Color.FromBrightness (1));

			if (this.renderer != null)
			{
				this.renderer.BeginRender (Rectangle.Deflate (this.Client.Bounds, 4, 4));

				foreach (var item in this.items)
				{
					this.renderer.Render (graphics, item);
				}

				this.renderer.EndRender ();
			}
		}


		private Renderers.AbstractRenderer renderer;
		private readonly List<Data.ChartSeries> items;
	}
}
