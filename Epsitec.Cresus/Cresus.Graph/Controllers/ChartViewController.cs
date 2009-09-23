//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Graph.Renderers;
using Epsitec.Common.Graph.Widgets;

using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Graph.Data;
using Epsitec.Cresus.Graph.Widgets;
using Epsitec.Common.Graph.Styles;
using Epsitec.Common.Graph;

namespace Epsitec.Cresus.Graph.Controllers
{
	internal sealed class ChartViewController
	{
		public ChartViewController(GraphApplication application)
		{
			this.application = application;
		}

		public Command GraphType
		{
			get
			{
				return this.graphType;
			}
			set
			{
				if (this.graphType != value)
				{
					this.graphType = value;
					//-					this.commandBar.SelectedItem = this.graphType;
					this.Refresh (this.document);
				}
			}
		}

		public ColorStyle ColorStyle
		{
			get
			{
				return this.colorStyle;
			}
			set
			{
				if (this.colorStyle != value)
				{
					this.colorStyle = value;
					this.Refresh (this.document);
				}
			}
		}
		

		public void SetupUI(Widget container)
		{
			this.container = container;
			
			this.chartView = new ChartView ()
			{
				Dock = DockStyle.Fill,
				Parent = this.container,
				Padding = new Margins (16, 24, 24, 16),
			};
		}

		public void Refresh(GraphDocument document)
		{
			this.document = document;

			if (this.document == null)
			{
				return;
			}

			var renderer = this.CreateRenderer ();

			if (renderer == null)
			{
				this.chartView.Renderer   = null;
				//-					this.captionView.Captions = null;
			}
			else
			{
				List<ChartSeries> series = new List<ChartSeries> (this.GetDocumentChartSeries ());

				bool stackValues = false;

				renderer.Clear ();
				renderer.ChartSeriesRenderingMode = stackValues ? ChartSeriesRenderingMode.Stacked : ChartSeriesRenderingMode.Separate;
				renderer.DefineValueLabels (this.document.ChartColumnLabels);
				renderer.CollectRange (series);
				renderer.UpdateCaptions (series);
				renderer.AlwaysIncludeZero = true;

				//-					Size size = renderer.Captions.GetCaptionLayoutSize (Size.MaxValue) + this.captionView.Padding.Size;

				var layoutMode = ContainerLayoutMode.None;

				switch (layoutMode)
				{
					case ContainerLayoutMode.HorizontalFlow:
						//-							this.captionView.PreferredHeight = size.Height;
						break;

					case ContainerLayoutMode.VerticalFlow:
						//-							this.captionView.PreferredWidth = size.Width;
						break;
				}

				this.chartView.Renderer = renderer;
				//-					this.captionView.Captions = renderer.Captions;
			}

			this.chartView.Invalidate ();
			//-				this.captionView.Invalidate ();
		}

		private AbstractRenderer CreateRenderer()
		{
			AbstractRenderer renderer = null;
			bool stackValues = false;

			if (this.GraphType == Res.Commands.GraphType.UseLineChart)
			{
				renderer = new LineChartRenderer ()
				{
					SurfaceAlpha = stackValues ? 1.0 : 0.0
				};
			}
			else if (this.GraphType == Res.Commands.GraphType.UseBarChartVertical)
			{
				renderer = new BarChartRenderer ();
			}

			if (renderer != null)
			{
				var adorner = new Epsitec.Common.Graph.Adorners.CoordinateAxisAdorner ()
				{
					GridColor = Color.FromBrightness (0.8),
					VisibleGrid = true,
					VisibleLabels = false,
					VisibleTicks = true,
				};

				renderer.AddStyle (this.colorStyle);
				renderer.AddAdorner (adorner);
			}

			return renderer;
		}

		private IEnumerable<ChartSeries> GetDocumentChartSeries()
		{
			bool accumulateValues = false;
			foreach (var series in this.document.OutputSeries.Select (x => x.ChartSeries))
			{
				yield return new ChartSeries (series.Label, accumulateValues ? ChartViewController.Accumulate (series.Values) : series.Values);
			}
		}

		private static IEnumerable<ChartValue> Accumulate(IEnumerable<ChartValue> collection)
		{
			double accumulation = 0.0;

			foreach (var value in collection)
			{
				accumulation += value.Value;
				yield return new ChartValue (value.Label, accumulation);
			}
		}

		

		private readonly GraphApplication	application;
		private Widget container;
		private ChartView				chartView;
		private GraphDocument document;
		private Command graphType;
		private ColorStyle colorStyle;

	}
}
