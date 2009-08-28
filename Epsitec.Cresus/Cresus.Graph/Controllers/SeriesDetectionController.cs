//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Graph;
using Epsitec.Common.Graph.Widgets;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Graph.Controllers
{
	public class SeriesDetectionController
	{
		public SeriesDetectionController(ChartView chartView, CaptionView captionView)
		{
			this.chartView   = chartView;
			this.captionView = captionView;

			if (this.chartView != null)
			{
				this.chartView.MouseMove += this.HandleChartViewMouseMove;
				this.chartView.Released  += this.HandleChartViewReleased;

				this.chartView.PaintForeground +=
					delegate (object sender, PaintEventArgs e)
					{
						Graphics graphics = e.Graphics;
						var renderer = this.chartView.Renderer;

						if ((this.hoverIndex >= 0) &&
							(renderer != null))
						{
							var series = renderer.SeriesItems[this.hoverIndex];

							using (Path path = renderer.GetDetectionPath (series, 3))
							{
								IAdorner adorner = Epsitec.Common.Widgets.Adorners.Factory.Active;
								Color    color   = adorner.ColorCaption;

								graphics.Color = Color.FromAlphaColor (0.3, color);
								graphics.PaintSurface (path);
							}
						}
					};
			}
			
			if (this.captionView != null)
			{
				this.captionView.MouseMove += this.HandleCaptionViewMouseMove;
				this.captionView.Released  += this.HandleChartViewReleased;
				
				this.captionView.BackgroundPaintCallback =
					delegate (CaptionPainter painter, int index, Rectangle bounds, IPaintPort port)
					{
						Graphics graphics = port as Graphics;

						if (graphics != null)
						{
							IAdorner adorner = Epsitec.Common.Widgets.Adorners.Factory.Active;
							Color    color   = adorner.ColorCaption;
							
							if (index == this.hoverIndex)
							{
								graphics.AddFilledRectangle (Rectangle.Inflate (bounds, 4.0, 1.0));
								graphics.RenderSolid (Color.FromAlphaColor (0.3, color));
							}
							if (index == this.selectIndex)
							{
								graphics.LineWidth = 2.0;
								graphics.LineJoin  = JoinStyle.Miter;
								graphics.AddRectangle (Rectangle.Inflate (bounds, 3.0, 1.0));
								graphics.RenderSolid (color);
							}
						}
					};
			}

			this.hoverIndex  = -1;
			this.selectIndex = -1;
		}


		private void HandleChartViewMouseMove(object sender, MessageEventArgs e)
		{
			var view = this.chartView;
			var pos = e.Point;
			var renderer = view.Renderer;

			if (renderer != null)
			{
				int index = 0;

				foreach (var series in renderer.SeriesItems)
				{
					using (Path path = renderer.GetDetectionPath (series, 4))
					{
						if (path.SurfaceContainsPoint (pos.X, pos.Y, 1))
						{
							this.NotifyHover (index);
							return;
						}
					}

					index++;
				}
			}

			this.NotifyHover (-1);
		}

		private void HandleCaptionViewMouseMove(object sender, MessageEventArgs e)
		{
			int index = this.captionView.FindCaptionIndex (e.Point);
			this.NotifyHover (index);
		}

		private void HandleChartViewReleased(object sender, MessageEventArgs e)
		{
			if ((this.hoverIndex >= 0) &&
				(this.selectIndex != this.hoverIndex))
			{
				this.selectIndex = this.hoverIndex;
				this.GetWidgets ().ForEach (x => x.Invalidate ());
			}
		}

		
		private void NotifyHover(int index)
		{
			if (this.hoverIndex != index)
			{
				this.hoverIndex = index;
				this.GetWidgets ().ForEach (x => x.Invalidate ());
				
				System.Diagnostics.Debug.WriteLine (string.Format ("Hover : {0}", index));
			}
		}


		private IEnumerable<Widget> GetWidgets()
		{
			if (this.captionView != null)
			{
				yield return this.captionView;
			}
			if (this.chartView != null)
			{
				yield return this.chartView;
			}
		}



		private readonly ChartView chartView;
		private readonly CaptionView captionView;

		private int hoverIndex;
		private int selectIndex;
	}
}
