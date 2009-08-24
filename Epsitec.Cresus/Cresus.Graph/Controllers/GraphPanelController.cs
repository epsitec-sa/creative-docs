//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Graph.Renderers;
using Epsitec.Common.Graph.Widgets;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Graph.Controllers
{
	internal sealed class GraphPanelController
	{
		public GraphPanelController(Widget root, GraphDocument document)
		{
			this.document = document;

			var lineChartRenderer = new LineChartRenderer ();

			lineChartRenderer.AddStyle (new Epsitec.Common.Graph.Styles.ColorStyle ("line-color") { "Red", "DeepPink", "Coral", "Tomato", "SkyBlue", "RoyalBlue", "DarkBlue", "Green", "PaleGreen", "Lime", "Yellow", "Wheat" });
			lineChartRenderer.AddAdorner (new Epsitec.Common.Graph.Adorners.CoordinateAxisAdorner ());
			
			this.chartView = new ChartView ()
			{
				Dock = DockStyle.Fill,
				Parent = root,
				Renderer = lineChartRenderer
			};

			this.captionView = new CaptionView ()
			{
				Parent = root,
				PreferredWidth = 160,
				PreferredHeight = 80,
				Captions = lineChartRenderer.Captions
			};
			
			this.splitter = new AutoSplitter ()
			{
				Parent = root
			};

			this.LayoutMode = ContainerLayoutMode.HorizontalFlow;
			
			this.detectionController= new SeriesDetectionController (this.chartView, this.captionView);

			var button = new Button ()
			{
				Anchor = AnchorStyles.TopRight,
				Margins = new Margins (0, 4, 4, 0),
				Text = "/",
				PreferredWidth = 20,
				PreferredHeight = 20,
				Parent = root
			};

			button.Clicked +=
				delegate
				{
					if (this.LayoutMode == ContainerLayoutMode.VerticalFlow)
					{
						this.LayoutMode = ContainerLayoutMode.HorizontalFlow;
					}
					else
					{
						this.LayoutMode = ContainerLayoutMode.VerticalFlow;
					}
				};
		}


		public ContainerLayoutMode LayoutMode
		{
			get
			{
				return this.layoutMode;
			}
			set
			{
				if (this.layoutMode != value)
				{
					this.layoutMode = value;

					switch (this.layoutMode)
					{
						case ContainerLayoutMode.HorizontalFlow:
							this.splitter.Dock = DockStyle.Right;
							this.captionView.Dock = DockStyle.Right;
							break;

						case ContainerLayoutMode.VerticalFlow:
							this.splitter.Dock = DockStyle.Bottom;
							this.captionView.Dock = DockStyle.Bottom;
							break;
					}
				}
			}
		}
		
		
		public void Refresh()
		{
			if ((this.document != null) &&
				(this.document.DataSet != null) &&
				(this.document.DataSet.DataTable != null))
			{
				var renderer = this.chartView.Renderer;
				
				renderer.Clear ();
				renderer.DefineValueLabels (this.document.DataSet.DataTable.ColumnLabels);
				renderer.CollectRange (this.document.ChartSeries);
				renderer.UpdateCaptions (this.document.ChartSeries);
				
				this.chartView.Invalidate ();
				this.captionView.Invalidate ();
			}
		}

		



		private readonly GraphDocument document;
		private readonly ChartView chartView;
		private readonly AutoSplitter splitter;
		private readonly CaptionView captionView;
		private readonly SeriesDetectionController detectionController;

		private ContainerLayoutMode layoutMode;
	}
}
