//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Graph.Widgets;
using Epsitec.Common.Graph.Renderers;
using Epsitec.Common.UI;

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
			this.graphCommands = new GraphCommands (this);
			this.graphDataSet = new GraphDataSet ();
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
			Actions.Factory.New (this.LoadDataSet).Invoke ();
		}

		internal void SetupInterface()
		{
			Window window = new Window ();

			window.Text = this.ShortWindowTitle;
			window.ClientSize = new Epsitec.Common.Drawing.Size (824, 400);
			window.Root.Padding = new Margins (4, 8, 8, 4);

			FrameBox bar = new FrameBox ()
			{
				Dock = DockStyle.Top,
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
				Margins = new Margins (0, 0, 0, 4),
				Parent = window.Root
			};

			new MetaButton ()
			{
				ButtonClass = ButtonClass.FlatButton,
				Dock = DockStyle.Stacked,
				CommandObject = ApplicationCommands.Undo,
				PreferredWidth = 32,
				Embedder = bar
			};

			new MetaButton ()
			{
				ButtonClass = ButtonClass.FlatButton,
				Dock = DockStyle.Stacked,
				CommandObject = ApplicationCommands.Redo,
				PreferredWidth = 32,
				Embedder = bar
			};

			new IconButton ()
			{
				Dock = DockStyle.Stacked,
				CommandObject = ApplicationCommands.Save,
				PreferredWidth = 32,
				Embedder = bar
			};

			this.SetEnable (ApplicationCommands.Save, false);
			

			FrameBox frame = new FrameBox ()
			{
				Dock = DockStyle.Fill,
				Parent = window.Root
			};

			this.seriesPickerController = new Controllers.SeriesPickerController (frame, this.graphDataSet);
			this.seriesPickerController.SumSeriesAction = Actions.Factory.New (this.SumRows);
			this.seriesPickerController.NegateSeriesAction = Actions.Factory.New (this.NegateRows);
			this.seriesPickerController.AddSeriesToGraphAction = Actions.Factory.New (this.AddToChart);

			
			this.Window = window;
			this.IsReady = true;
		}

		
		private void SumRows(IEnumerable<int> rows)
		{
			var sum = this.graphDataSet.DataTable.SumRows (rows, this.seriesPickerController.GetRowSeries);

			if (sum == null)
			{
				return;
			}

			this.graphDataSet.DataTable.RemoveRows (rows);
			this.graphDataSet.DataTable.Insert (rows.First (), sum.Label.Replace ("+-", "-"), sum.Values);

			this.seriesPickerController.UpdateScrollListItems ();
			this.seriesPickerController.UpdateChartView ();
		}

		private void NegateRows(IEnumerable<int> rows)
		{
			foreach (int row in rows)
			{
				var series = this.graphDataSet.DataTable.GetRowSeries (row);
				this.seriesPickerController.NegateSeries (series.Label);
			}

			this.seriesPickerController.UpdateScrollListItems ();
			this.seriesPickerController.UpdateChartView ();
		}

		private void AddToChart(IEnumerable<int> rows)
		{
		}

		private void LoadDataSet()
		{
			this.graphDataSet.LoadDataTable ();

			if (this.seriesPickerController != null)
			{
				this.seriesPickerController.UpdateScrollListItems ();
			}
		}


		private Controllers.SeriesPickerController seriesPickerController;

		private GraphDataSet graphDataSet;
		private GraphCommands graphCommands;
	}
}
