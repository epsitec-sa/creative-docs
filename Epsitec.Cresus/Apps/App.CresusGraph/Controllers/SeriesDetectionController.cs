//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using Epsitec.Common.Drawing;
using Epsitec.Common.Graph;
using Epsitec.Common.Graph.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

namespace Epsitec.Cresus.Graph.Controllers
{
	public sealed class SeriesDetectionController
	{
		public SeriesDetectionController(ChartView chartView, SummaryCaptionsView summaryCaptionsView, SeriesCaptionsView seriesCaptionsView)
		{
			this.chartView   = chartView;
			this.summaryCaptionsView = summaryCaptionsView;
            this.seriesCaptionsView = seriesCaptionsView;

			if (this.chartView != null)
            {
                this.chartView.MouseMove += this.HandleChartViewMouseMove;
                this.chartView.Released += this.HandleChartViewReleased;
                this.chartView.Exited += (sender, e) => this.HoverIndex = -1;

                this.seriesCaptionsView.MouseMove += this.HandleChartViewMouseMove;
                this.seriesCaptionsView.Released += this.HandleChartViewReleased;
                this.seriesCaptionsView.Exited += (sender, e) => this.HoverIndex = -1;

				this.chartView.PaintForeground +=
					delegate (object sender, PaintEventArgs e)
					{
						Graphics graphics = e.Graphics;
						var renderer = this.chartView.Renderer;

						if ((this.hoverIndex >= 0) &&
							(renderer != null))
						{
							var series = renderer.SeriesItems[this.hoverIndex];

							using (Path path = renderer.GetDetectionPath (series, this.hoverIndex, 3))
							{
								IAdorner adorner = Epsitec.Common.Widgets.Adorners.Factory.Active;
								Color    color   = adorner.ColorCaption;

								graphics.Color = Color.FromAlphaColor (0.3, color);
								graphics.PaintSurface (path);
							}
						}
					};
			}
			
			if (this.summaryCaptionsView != null)
			{
				this.summaryCaptionsView.MouseMove += this.HandleCaptionViewMouseMove;
				this.summaryCaptionsView.Released  += this.HandleChartViewReleased;
				this.summaryCaptionsView.Exited    += (sender, e) => this.HoverIndex = -1;
				
				this.summaryCaptionsView.BackgroundPaintCallback =
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
							if (index == this.ActiveIndex)
							{
								graphics.LineWidth = 2.0;
								graphics.LineJoin  = JoinStyle.Miter;
								graphics.AddRectangle (Rectangle.Inflate (bounds, 3.0, 1.0));
								graphics.RenderSolid (color);
							}
						}
					};
			}

			this.HoverIndex  = -1;
			this.ActiveIndex = -1;
		}


		public int HoverIndex
		{
			get
			{
				return this.hoverIndex;
			}
			set
			{
				if (this.hoverIndex != value)
				{
					var oldValue = this.hoverIndex;
					var newValue = value;

					this.hoverIndex = value;

					this.OnHoverIndexChanged (oldValue, newValue);
					this.GetWidgets ().ForEach (widget => widget.Invalidate ());
				}
			}
		}

		public int ActiveIndex
		{
			get
			{
				return this.activeIndex;
			}
			set
			{
				if (this.activeIndex != value)
				{
					var oldValue = this.activeIndex;
					var newValue = value;

					this.activeIndex = value;

					this.OnActiveIndexChanged (oldValue, newValue);
					this.GetWidgets ().ForEach (widget => widget.Invalidate ());
				}
			}
		}

		public Rectangle ActiveCaptionBounds
		{
			get
			{
				if (this.ActiveIndex < 0)
				{
					return Rectangle.Empty;
				}
				else
				{
					return Rectangle.Inflate (this.summaryCaptionsView.GetCaptionBounds (this.ActiveIndex), 2, 2);
				}
			}
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
					using (Path path = renderer.GetDetectionPath (series, index, 4))
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
			int index = this.summaryCaptionsView.FindCaptionIndex (e.Point);
			this.NotifyHover (index);
		}

		private void HandleChartViewReleased(object sender, MessageEventArgs e)
		{
			if (this.hoverIndex >= 0)
			{
				this.ActiveIndex = this.hoverIndex;
			}
		}

		
		private void NotifyHover(int index)
		{
			this.HoverIndex = index;
		}


		private IEnumerable<Widget> GetWidgets()
		{
			if (this.summaryCaptionsView != null)
			{
				yield return this.summaryCaptionsView;
			}
			if (this.chartView != null)
			{
				yield return this.chartView;
			}
		}

		private void OnHoverIndexChanged(int oldIndex, int newIndex)
		{
			var handler = this.HoverIndexChanged;

			if (handler != null)
			{
				handler (this, new DependencyPropertyChangedEventArgs ("HoverIndex", oldIndex, newIndex));
			}
		}

		private void OnActiveIndexChanged(int oldIndex, int newIndex)
		{
			var handler = this.ActiveIndexChanged;

			if (handler != null)
			{
				handler (this, new DependencyPropertyChangedEventArgs ("ActiveIndex", oldIndex, newIndex));
			}
		}


		public event EventHandler<DependencyPropertyChangedEventArgs> HoverIndexChanged;
		public event EventHandler<DependencyPropertyChangedEventArgs> ActiveIndexChanged;

        private readonly ChartView chartView;
        private readonly SummaryCaptionsView summaryCaptionsView;
        private readonly SeriesCaptionsView seriesCaptionsView;

		private int hoverIndex;
		private int activeIndex;
	}
}
