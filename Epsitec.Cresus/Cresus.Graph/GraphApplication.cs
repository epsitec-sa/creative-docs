//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Graph.Widgets;
using Epsitec.Common.Graph.Renderers;

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Graph
{
	public class GraphApplication : Application
	{
		public GraphApplication()
		{
		}

		public bool IsReady
		{
			get;
			private set;
		}

		public override string ShortWindowTitle
		{
			get
			{
				return "Crésus Graphe";
			}
		}

		internal void SetupDataSet()
		{
			this.graphDataSet = new GraphDataSet ();
		}
		

		internal void SetupInterface()
		{
			Window window = new Window ();

			window.Text = this.ShortWindowTitle;
			window.ClientSize = new Epsitec.Common.Drawing.Size (824, 400);

			FrameBox frame = new FrameBox ()
			{
				Margins = new Margins (8, 8, 8, 8),
				Dock = DockStyle.Fill,
				Parent = window.Root
			};

			this.scrollList = new ScrollListMultiSelect ()
			{
				Dock = DockStyle.Left,
				PreferredWidth = 300,
				Parent = frame,
				RowHeight = 24,
				ScrollListStyle = ScrollListStyle.AlternatingRows
			};

			VSplitter splitter = new VSplitter ()
			{
				Dock = DockStyle.Left,
				PreferredWidth = 8,
				Parent = frame
			};

			this.chartView = new ChartView ()
			{
				Dock = DockStyle.Fill,
				PreferredWidth = 300,
				Parent = frame
			};

			var lineChartRenderer = new LineChartRenderer ();

			lineChartRenderer.DefineValueLabels (this.graphDataSet.DataTable.ColumnLabels);
			lineChartRenderer.AddStyle (new Epsitec.Common.Graph.Styles.ColorStyle ("line-color") { "Red", "DeepPink", "Coral", "Tomato", "SkyBlue", "RoyalBlue", "DarkBlue", "Green", "PaleGreen", "Lime", "Yellow", "Wheat" });
			lineChartRenderer.AddAdorner (new Epsitec.Common.Graph.Adorners.CoordinateAxisAdorner ());

			this.chartView.DefineRenderer (lineChartRenderer);

			this.UpdateScrollListItems ();

			this.scrollListController = new Controllers.MainScrollListController (this.scrollList);
			this.scrollListController.Changed += sender => this.UpdateChartView ();
			this.scrollListController.SumSeries = selection => this.SumRows (selection);
			
			this.Window = window;
			this.IsReady = true;
		}

		private void SumRows(IEnumerable<int> rows)
		{
			var sum = this.graphDataSet.DataTable.SumRows (rows);

			if (sum == null)
			{
				return;
			}

			this.graphDataSet.DataTable.RemoveRows (rows);
			this.graphDataSet.DataTable.Insert (rows.First (), sum.Label, sum.Values);

			this.UpdateScrollListItems ();
		}

		private void UpdateScrollListItems()
		{
			this.scrollList.ClearSelection ();
			this.scrollList.Items.Clear ();
			this.scrollList.Items.AddRange (graphDataSet.DataTable.RowLabels);
		}

		private void UpdateChartView()
		{
			var renderer = this.chartView.Renderer;
			
			renderer.Clear ();
			renderer.CollectRange (this.scrollList.GetSortedSelection ().Select (x => this.graphDataSet.DataTable.GetRowSeries (x)));
			renderer.ClipRange (System.Math.Min (0, renderer.MinValue), System.Math.Max (0, renderer.MaxValue));
			
			this.chartView.Invalidate ();
		}


		private ScrollListMultiSelect scrollList;
		private Controllers.MainScrollListController scrollListController;
		private ChartView chartView;

		private GraphDataSet graphDataSet;
	}
}
