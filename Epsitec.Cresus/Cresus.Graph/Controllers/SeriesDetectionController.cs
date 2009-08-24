//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Graph.Widgets;
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
				this.chartView.MouseMove += this.chartView_MouseMove;
			}
			if (this.captionView != null)
			{
				this.captionView.MouseMove += this.captionView_MouseMove;
			}

			this.hoverIndex = -1;
		}

		
		void chartView_MouseMove(object sender, MessageEventArgs e)
		{
			var view = this.chartView;
			var pos = e.Point;
			var renderer = view.Renderer;

			int index = 0;

			foreach (var series in renderer.SeriesItems)
			{
				using (Path path = renderer.GetDetectionPath (series, 4))
				{
					if (path.SurfaceContainsPoint (pos.X, pos.Y, 1))
					{
						//	Inside the given series

						this.NotifyHover (index);
						return;
					}
				}

				index++;
			}

			this.NotifyHover (-1);
		}

		void captionView_MouseMove(object sender, MessageEventArgs e)
		{
			int index = this.captionView.Captions.DetectCaption (this.captionView.Client.Bounds, e.Point);
			this.NotifyHover (index);
		}

		
		private void NotifyHover(int index)
		{
			if (this.hoverIndex != index)
			{
				this.hoverIndex = index;
				System.Diagnostics.Debug.WriteLine (string.Format ("Hover : {0}", index));
			}
		}




		private readonly ChartView chartView;
		private readonly CaptionView captionView;

		private int hoverIndex;
	}
}
