//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;


namespace Epsitec.Common.Graph.Widgets
{
	public class ChartView : FrameBox
	{
		public ChartView()
		{
			this.Padding = new Margins (60, 30, 30, 30);
			this.scale = 1.0;

            this.Clicked += this.OnClicked;
		}


		public Renderers.AbstractRenderer Renderer
		{
			get
			{
				return this.renderer;
			}
			set
			{
				if (this.renderer != value)
				{
					this.renderer = value;
					this.Invalidate ();
				}
			}
		}

		public double Scale
		{
			get
			{
				return this.scale;
			}
			set
			{
				if (this.scale != value)
				{
					this.scale = value;
					this.Invalidate ();
				}
			}
		}

        /// <summary>
        /// Method called by <see cref="SeriesDetectionController"/> when the mouse is over the graph.
        /// Allows the renderer to be noticed and it can change its appearance based on the mouse location.
        /// Called only when <c>seriesItem</c> changes.
        /// </summary>
        /// <param name="oldValue">Old seriesItem</param>
        /// <param name="newValue">New seriesItem</param>
        public void HoverIndexChanged(object oldValue, object newValue)
        {
            renderer.HoverIndexChanged(oldValue, newValue);
        }

		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			graphics.AddFilledRectangle (this.Client.Bounds);
			graphics.RenderSolid (Color.FromBrightness (1));

			if (this.renderer != null)
			{
				var transform = graphics.Transform;
				graphics.ScaleTransform (this.scale, this.scale, 0, 0);
				Rectangle paint = Rectangle.Deflate (this.Client.Bounds, this.Padding);
                this.renderer.Render (graphics, Rectangle.Scale (this.Client.Bounds, 1 / this.scale), Rectangle.Scale (paint, 1 / this.scale));
				graphics.Transform = transform;
			}
		}

        /// <summary>
        /// Method called by <see cref="ChartViewController"/> when there is a click on the graph.
        /// Allows the renderer to be noticed and it can change its appearance based on the click.
        /// </summary>
        /// <param name="sender">Object throwing the click</param>
        /// <param name="e">Event data</param>
        public void OnClicked (object sender, MessageEventArgs e)
        {
            renderer.OnClicked (sender, e);
        }


		private Renderers.AbstractRenderer renderer;
		private double scale;
	}
}
