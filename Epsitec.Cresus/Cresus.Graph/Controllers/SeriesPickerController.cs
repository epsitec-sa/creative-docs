//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Graph.Renderers;
using Epsitec.Common.Graph.Widgets;

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Graph.Data;

namespace Epsitec.Cresus.Graph.Controllers
{
	internal sealed class SeriesPickerController
	{
		public SeriesPickerController(Window owner)
		{
			this.negatedSeriesLabels = new HashSet<string> ();

			var lineChartRenderer = new LineChartRenderer ();

			lineChartRenderer.AddStyle (new Epsitec.Common.Graph.Styles.ColorStyle ("line-color") { "Red", "DeepPink", "Coral", "Tomato", "SkyBlue", "RoyalBlue", "DarkBlue", "Green", "PaleGreen", "Lime", "Yellow", "Wheat" });
			lineChartRenderer.AddAdorner (new Epsitec.Common.Graph.Adorners.CoordinateAxisAdorner () { VisibleGrid = false, VisibleLabels = false, VisibleTicks = false });


			this.window = new Window ()
			{
				Text = Res.Strings.DataPicker.Title.ToSimpleText (),
				ClientSize = new Epsitec.Common.Drawing.Size (1008, 736),
				Name = "SeriesPicker2",
				Owner = owner,
				Icon = owner.Icon,
				PreventAutoClose = true
			};

			this.window.MakeSecondaryWindow ();

			var root = this.window.Root;

			root.Padding = new Margins (0, 0, 0, 0);

			var frameTop = new FrameBox ()
			{
				Dock = DockStyle.Top,
				PreferredHeight = 150,
				Parent = root,
				Name = "top",
				BackColor = Color.FromRgb (1, 0.9, 0.9)
			};

			var frameWorkspace = new FrameBox ()
			{
				Dock = DockStyle.Fill,
				Parent = root,
				Name = "workspace",
				BackColor = Color.FromRgb (1, 1, 0.9)
			};

			var frameInputs = new FrameBox ()
			{
				Dock = DockStyle.Fill,
				Parent = frameWorkspace,
				Name = "inputs",
				BackColor = Color.FromRgb (0.9, 1, 0.9),
				PreferredHeight = 320
			};

			var frameOutputs = new FrameBox ()
			{
				Dock = DockStyle.Bottom,
				Parent = frameWorkspace,
				Name = "outputs",
				PreferredHeight = 260
			};

			var frameOutputsLeft = new FrameBox ()
			{
				Dock = DockStyle.Fill,
				Parent = frameOutputs,
				Name = "left"
			};

			var frameGroups = new FrameBox ()
			{
				Dock = DockStyle.Fill,
				Parent = frameOutputsLeft,
				Name = "groups",
				BackColor = Color.FromRgb (0.9, 0.9, 1),
				PreferredHeight = 160
			};

			var frameSelection = new FrameBox ()
			{
				Dock = DockStyle.Bottom,
				Parent = frameOutputsLeft,
				Name = "selection",
				BackColor = Color.FromRgb (1, 0.9, 0.8),
				PreferredHeight = 100
			};

			var framePreview = new FrameBox ()
			{
				Dock = DockStyle.Right,
				Parent = frameOutputs,
				Name = "preview",
				BackColor = Color.FromRgb (1, 0.85, 0.8),
				PreferredWidth = 260
			};

			this.selectionItemsController = new ItemListController (frameSelection)
			{
				ItemLayoutMode = ItemLayoutMode.Horizontal
			};

			for (int i = 0; i < 5; i++)
			{
				var view = new MiniChartView ()
				{
					Anchor = AnchorStyles.BottomLeft,
					HorizontalAlignment = HorizontalAlignment.Center,
					VerticalAlignment = VerticalAlignment.Center,
					PreferredWidth = 80,
					PreferredHeight = 80,
					Padding = new Margins (4, 4, 4, 4),
					Scale = 0.5,
					Renderer = lineChartRenderer
				};

				this.selectionItemsController.Add (view);
			}

			this.selectionItemsController.Layout ();
			
			this.chartView = new MiniChartView ()
			{
				Anchor = AnchorStyles.TopLeft,
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
				PreferredWidth = 80,
				PreferredHeight = 80,
				Padding = new Margins (4, 4, 4, 4),
				Margins = new Margins (0, 0, 0, 0),
				Scale = 0.5,
				Parent = frameInputs,
				Renderer = lineChartRenderer
			};

			this.window.WindowCloseClicked += sender => this.HideWindow ();

			GraphProgram.Application.ActiveDocumentChanged += sender => this.UpdateScrollListItems ();
		}

		public void ShowWindow()
		{
			this.window.Show ();
		}

		public void HideWindow()
		{
			this.window.Hide ();
		}

		public GraphDataSet DataSet
		{
			get
			{
				return GraphProgram.Application.Document.DataSet;
			}
		}

		public void UpdateScrollListItems()
		{
			if ((this.DataSet != null) &&
				(this.DataSet.DataTable != null))
			{
				List<string> labels = new List<string> (this.DataSet.DataTable.RowLabels);

#if false
				var selection = this.scrollList.GetSortedSelection ();

				this.scrollList.ClearSelection ();
				this.scrollList.Items.Clear ();
				this.scrollList.Items.AddRange (labels.Select (x => this.negatedSeriesLabels.Contains (x) ? string.Concat ("(", x, ")") : x));
				this.scrollList.SelectedIndex = selection.Count == 0 ? -1 : selection.First ();
				this.scrollList.ShowSelected (ScrollShowMode.Extremity);
#endif
			}
		}

		public void UpdateChartView()
		{
			var renderer = this.chartView.Renderer;

			renderer.Clear ();
			renderer.DefineValueLabels (this.DataSet.DataTable.ColumnLabels);
//-			renderer.CollectRange (this.scrollList.GetSortedSelection ().Select (x => this.GetRowSeries (x)));
			renderer.ClipRange (System.Math.Min (0, renderer.MinValue), System.Math.Max (0, renderer.MaxValue));

			this.chartView.Invalidate ();
		}

		public void SetSelectedItem(int index)
		{
		}

		public void ClearNegatedSeries()
		{
			this.negatedSeriesLabels.Clear ();
		}

		public void NegateSeries(string seriesLabel)
		{
			if (this.negatedSeriesLabels.Contains (seriesLabel))
			{
				this.negatedSeriesLabels.Remove (seriesLabel);
			}
			else
			{
				this.negatedSeriesLabels.Add (seriesLabel);
			}
		}

		public ChartSeries GetRowSeries(int index)
		{
			var  table  = this.DataSet.DataTable;
			var  series = table.GetRowSeries (index);
			bool negate = this.negatedSeriesLabels.Contains (series.Label);

			if (negate)
			{
				return new ChartSeries ("-" + series.Label, series.Values.Select (x => new ChartValue (x.Label, -x.Value)));
			}
			else
			{
				return series;
			}
		}

		
		private void OnChanged()
		{
			var handler = this.Changed;

			if (handler != null)
			{
				handler (this);
			}
		}


		public event EventHandler				Changed;
		
		public System.Action<IEnumerable<int>>	SumSeriesAction;
		public System.Action<IEnumerable<int>>	AddSeriesToGraphAction;
		public System.Action<IEnumerable<int>>	NegateSeriesAction;

		readonly private Window					window;
		readonly private ChartView				chartView;
		readonly private HashSet<string>		negatedSeriesLabels;

		readonly private ItemListController		selectionItemsController;
	}
}
