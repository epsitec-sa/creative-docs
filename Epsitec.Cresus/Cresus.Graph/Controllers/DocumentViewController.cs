//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Graph.Renderers;
using Epsitec.Common.Graph.Widgets;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Graph.Widgets;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.UI;

namespace Epsitec.Cresus.Graph.Controllers
{
	/// <summary>
	/// The <c>DocumentViewController</c> class creates and manages one view of a document,
	/// usually inside a tab page.
	/// </summary>
	internal sealed class DocumentViewController
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DocumentViewController"/> class.
		/// </summary>
		/// <param name="container">The container.</param>
		/// <param name="document">The document.</param>
		/// <param name="showContainer">The callback used to make the container visible.</param>
		public DocumentViewController(Widget container, GraphDocument document, System.Action<DocumentViewController> showContainer)
		{
			this.document = document;
			this.showContainerCallback = showContainer;

			var lineChartRenderer = new LineChartRenderer ();

			lineChartRenderer.AddStyle (new Epsitec.Common.Graph.Styles.ColorStyle ("line-color") { "Red", "DeepPink", "Coral", "Tomato", "SkyBlue", "RoyalBlue", "DarkBlue", "Green", "PaleGreen", "Lime", "Yellow", "Wheat" });
			lineChartRenderer.AddAdorner (new Epsitec.Common.Graph.Adorners.CoordinateAxisAdorner ());

			var frame = new FrameBox ()
			{
				Dock = DockStyle.Fill,
				Parent = container
			};

			this.commandBar = new CommandSelectionBar ()
			{
				Dock = DockStyle.Top,
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
				Parent = container,
				BackColor = container.BackColor
			};

			this.commandBar.Items.Add (Res.Commands.GraphType.UseLineChart);
			this.commandBar.Items.Add (Res.Commands.GraphType.UseBarChartVertical);
			this.commandBar.Items.Add (Res.Commands.GraphType.UseBarChartHorizontal);

			this.commandBar.ItemSize = new Size (64, 40);
			this.commandBar.SelectedItem = Res.Commands.GraphType.UseLineChart;



			this.chartView = new ChartView ()
			{
				Dock = DockStyle.Fill,
				Parent = frame,
				Renderer = lineChartRenderer
			};

			this.captionView = new CaptionView ()
			{
				Parent = frame,
				Padding = new Margins(4, 4, 2, 2),
				PreferredWidth = 160,
				PreferredHeight = 80,
				Captions = lineChartRenderer.Captions
			};
			
			this.splitter = new AutoSplitter ()
			{
				Parent = frame
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
				Parent = frame
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

		public void MakeVisible()
		{
			if (this.showContainerCallback != null)
			{
				this.showContainerCallback (this);
			}
		}
		



		private readonly GraphDocument			document;
		private readonly CommandSelectionBar	commandBar;
		private readonly ChartView				chartView;
		private readonly AutoSplitter			splitter;
		private readonly CaptionView			captionView;
		private readonly SeriesDetectionController detectionController;
		private readonly System.Action<DocumentViewController> showContainerCallback;

		private ContainerLayoutMode				layoutMode;
	}
}
