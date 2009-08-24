//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

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
				Dock = DockStyle.Right,
				Parent = root,
				Captions = lineChartRenderer.Captions
			};
		}


		public void ProcessDocumentChanged()
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
		private readonly CaptionView captionView;
	}
}
