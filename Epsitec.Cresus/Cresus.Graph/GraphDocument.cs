//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Graph.Data;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Graph.Controllers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Graph
{
	public class GraphDocument
	{
		public GraphDocument(GraphDataSet dataSet)
		{
			this.graphPanels = new List<GraphPanelController> ();
			this.chartSeries = new List<ChartSeries> ();
			this.dataSet = dataSet;
			this.dataSet.Changed += x => this.Clear ();

			this.CreateUI ();
			this.ProcessDocumentChanged ();
		}


		public GraphDataSet DataSet
		{
			get
			{
				return this.dataSet;
			}
		}

		public IEnumerable<ChartSeries> ChartSeries
		{
			get
			{
				return this.chartSeries;
			}
		}

		public void Add(ChartSeries series)
		{
			this.chartSeries.Add (series);
			this.ProcessDocumentChanged ();
		}

		public void Clear()
		{
			this.chartSeries.Clear ();
			this.ProcessDocumentChanged ();
		}


		private void ProcessDocumentChanged()
		{
			foreach (var panel in this.graphPanels)
			{
				panel.ProcessDocumentChanged ();
			}
		}


		private void CreateUI()
		{
			if (this.window == null)
			{
				this.window = new Window ()
				{
					Text = "Document",
					ClientSize = new Size (960, 600)
				};

				var frame = new FrameBox ()
				{
					Dock = DockStyle.Fill,
					Margins = new Margins (4, 4, 8, 8),
					Parent = this.window.Root
				};

				var panel = new GraphPanelController (frame, this);

				this.graphPanels.Add (panel);
				this.window.Show ();
			}
		}

		
		private readonly List<GraphPanelController> graphPanels;
		private readonly List<ChartSeries> chartSeries;
		private readonly GraphDataSet dataSet;

		private Window window;
	}
}
