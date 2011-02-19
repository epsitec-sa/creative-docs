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
	internal sealed class SeriesPickerControllerOriginal
	{
		public SeriesPickerControllerOriginal(Window owner)
		{
			this.negatedSeriesLabels = new HashSet<string> ();

			var lineChartRenderer = new LineChartRenderer ();

			lineChartRenderer.AddStyle (new Epsitec.Common.Graph.Styles.ColorStyle ("line-color") { "Red", "DeepPink", "Coral", "Tomato", "SkyBlue", "RoyalBlue", "DarkBlue", "Green", "PaleGreen", "Lime", "Yellow", "Wheat" });
			lineChartRenderer.AddAdorner (new Epsitec.Common.Graph.Adorners.CoordinateAxisAdorner () { VisibleGrid = false, VisibleLabels = false, VisibleTicks = false });


			this.window = new Window ()
			{
				Text = Res.Strings.DataPicker.Title.ToSimpleText (),
				ClientSize = new Epsitec.Common.Drawing.Size (824, 400),
				Name = "SeriesPicker",
				Owner = owner,
				Icon = owner.Icon,
				PreventAutoClose = true
			};

			this.window.MakeSecondaryWindow ();

			var root = this.window.Root;

			root.Padding = new Margins (4, 8, 8, 4);

			
			this.scrollList = new ScrollListMultiSelect ()
			{
				Dock = DockStyle.Left,
				PreferredWidth = 300,
				Parent = root,
				RowHeight = 24,
				ScrollListStyle = ScrollListStyle.AlternatingRows
			};

			VSplitter splitter = new VSplitter ()
			{
				Dock = DockStyle.Left,
				PreferredWidth = 8,
				Parent = root
			};

			var view = new FrameBox ()
			{
				Dock = DockStyle.Fill,
				Parent = root,
				PreferredWidth = 300,
				BackColor = Color.FromBrightness (1.0)
			};

			this.chartView = new MiniChartView ()
			{
				Dock = DockStyle.Fill,
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
				PreferredWidth = 80,
				PreferredHeight = 80,
				Padding = new Margins (4, 4, 4, 4),
				Scale = 0.5,
				Parent = view,
				Renderer = lineChartRenderer
			};

			this.hotRowIndex = -1;
			this.visibleQuickButtons = new List<Button> ();

			this.quickButtonNegate = new IconButton ()
			{
				PreferredWidth = 22,
				PreferredHeight = 22,
				ButtonStyle = ButtonStyle.Icon,
				IconUri = "manifest:Epsitec.Cresus.Graph.Images.Glyph.PlusMinus.icon",
				Parent = this.scrollList.Parent,
				Anchor = AnchorStyles.BottomLeft
			};

			this.quickButtonAddToGraph = new IconButton ()
			{
				PreferredWidth = 22,
				PreferredHeight = 22,
				ButtonStyle = ButtonStyle.Icon,
				IconUri = "manifest:Epsitec.Cresus.Graph.Images.Glyph.Pick.icon",
				Parent = this.scrollList.Parent,
				Anchor = AnchorStyles.BottomLeft
			};

			this.quickButtonSum = new IconButton ()
			{
				PreferredWidth = 22,
				PreferredHeight = 22,
				ButtonStyle = ButtonStyle.Icon,
				IconUri = "manifest:Epsitec.Cresus.Graph.Images.Glyph.Sum.icon",
				Parent = this.scrollList.Parent,
				Anchor = AnchorStyles.BottomLeft
			};

			this.window.WindowCloseClicked += sender => this.HideWindow ();
			
			this.quickButtonNegate.Clicked     += (sender, e) => this.ProcessQuickButton (this.NegateSeriesAction);
			this.quickButtonAddToGraph.Clicked += (sender, e) => this.ProcessQuickButton (this.AddSeriesToGraphAction);
			this.quickButtonSum.Clicked        += (sender, e) => this.ProcessQuickButton (this.SumSeriesAction);

			this.scrollList.DragMultiSelectionStarted += this.HandleDragMultiSelectionStarted;
			this.scrollList.DragMultiSelectionEnded   += this.HandleDragMultiSelectionEnded;
			this.scrollList.MultiSelectionChanged     += this.HandleMultiSelectionChanged;
			this.scrollList.MouseMove                 += this.HandleMouseMove;
			this.scrollList.Exited                    += this.HandleMouseExited;

			this.scrollList.PaintForeground += this.HandlePaintForeground;

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
				return null; //-	GraphProgram.Application.Document.DataSet;
			}
		}

		public void UpdateScrollListItems()
		{
			if ((this.scrollList != null) &&
				(this.DataSet != null) &&
				(this.DataSet.DataTable != null))
			{
				List<string> labels = new List<string> (this.DataSet.DataTable.RowLabels);

				var selection = this.scrollList.GetSortedSelection ();

				this.scrollList.ClearSelection ();
				this.scrollList.Items.Clear ();
				this.scrollList.Items.AddRange (labels.Select (x => this.negatedSeriesLabels.Contains (x) ? string.Concat ("(", x, ")") : x));
				this.scrollList.SelectedItemIndex = selection.Count == 0 ? -1 : selection.First ();
				this.scrollList.ShowSelected (ScrollShowMode.Extremity);
			}
		}

		public void UpdateChartView()
		{
			var renderer = this.chartView.Renderer;

			renderer.Clear ();
			renderer.DefineValueLabels (this.DataSet.DataTable.ColumnLabels);
			renderer.CollectRange (this.scrollList.GetSortedSelection ().Select (x => this.GetRowSeries (x)));
			renderer.ClipRange (System.Math.Min (0, renderer.MinValue), System.Math.Max (0, renderer.MaxValue));

			this.chartView.Invalidate ();
		}

		public void SetSelectedItem(int index)
		{
			if (this.scrollList != null)
			{
				this.scrollList.SelectedItemIndex = index;
				this.scrollList.ShowSelected (ScrollShowMode.Extremity);
			}
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

			this.scrollList.Invalidate ();
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

		
		private void HandleDragMultiSelectionStarted(object sender, MultiSelectEventArgs e)
		{
			if (Message.CurrentState.IsControlPressed == false)
			{
				this.scrollList.ClearSelection ();
			}

			this.selection = null;
			this.UpdateVisibleButtons ();
		}

		private void HandleDragMultiSelectionEnded(object sender, MultiSelectEventArgs e)
		{
			if ((e.Count == 1) &&
				(this.scrollList.IsItemSelected (e.BeginIndex)))
			{
				this.scrollList.RemoveSelection (Enumerable.Range (e.BeginIndex, e.Count));
			}
			else
			{
				this.scrollList.AddSelection (Enumerable.Range (e.BeginIndex, e.Count));
				this.selection = e;
				this.UpdateVisibleButtons ();
			}
		}

		private void HandleMultiSelectionChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			int index = this.scrollList.SelectedItemIndex;

			if ((this.scrollList.IsItemSelected (index)) &&
				(this.scrollList.SelectionCount == 1))
			{
				this.selection = new MultiSelectEventArgs (index, index);
			}
			else
			{
				this.selection = null;
			}

			this.UpdateVisibleButtons ();
			this.UpdateChartView ();
			
			this.OnChanged ();
		}

		private void HandleMouseExited(object sender, MessageEventArgs e)
		{
			this.NotifyHotRow (-1);
		}

		private void HandleMouseMove(object sender, MessageEventArgs e)
		{
			int count = this.scrollList.VisibleRowCount;
			int first = this.scrollList.FirstVisibleRow;

			for (int i = 0; i < count; i++)
			{
				int index = first + i;
				var frame = this.scrollList.GetRowBounds (index);

				if (frame.Contains (e.Point))
				{
					this.NotifyHotRow (index);
					return;
				}
			}
		}

		private void HandlePaintForeground(object sender, PaintEventArgs e)
		{
			int index = this.scrollList.SelectedItemIndex;

			if ((index >= 0) &&
				(this.selection != null) &&
				(this.selection.Count > 0))
			{
				var graphics  = e.Graphics;
				var bounds    = this.scrollList.GetRowBounds (index);

				if ((bounds.IsEmpty) ||
					(this.visibleQuickButtons.Count == 0))
				{
					this.HideQuickButtons ();
				}
				else
				{
					var rectangle = this.selection == null ? bounds : this.scrollList.GetRowBounds (this.selection.BeginIndex, this.selection.Count);

					double arrowLength = 6;
					double buttWidth = this.visibleQuickButtons.Count * 24;
					double zoneWidth = arrowLength + 2 + buttWidth;

					using (Path path = new Path ())
					{
						double ox = bounds.Right - zoneWidth + 1;
						double oy = rectangle.Bottom;
						double cy = bounds.Center.Y;

						path.MoveTo (ox, oy);
						path.LineTo (ox, cy-arrowLength);
						path.LineTo (ox+arrowLength, cy);
						path.LineTo (ox, cy+arrowLength);
						path.LineTo (ox, rectangle.Top);
						path.LineTo (rectangle.TopRight);
						path.LineTo (rectangle.BottomRight);
						path.Close ();

						Color background = Color.FromName ("ActiveCaption");

						graphics.Color = Color.Mix (background, Color.FromBrightness (1), 0.20);

						graphics.PaintSurface (path);
						graphics.RenderSolid ();
					}

					using (Path path = new Path ())
					{
						double ox = bounds.Right - zoneWidth + 0.5;
						double oy = rectangle.Bottom;
						double cy = bounds.Center.Y;

						path.MoveTo (ox, oy);
						path.LineTo (ox, cy-arrowLength);
						path.LineTo (ox+arrowLength, cy);
						path.LineTo (ox, cy+arrowLength);
						path.LineTo (ox, rectangle.Top);

						graphics.LineWidth = 1.0;
						graphics.LineCap = CapStyle.Butt;
						graphics.LineJoin = JoinStyle.Miter;
						graphics.Color = Color.FromBrightness (1);

						graphics.PaintOutline (path);
						graphics.RenderSolid ();
					}

					this.UpdateQuickButtons (new Rectangle (bounds.Right - buttWidth, bounds.Bottom, buttWidth, bounds.Height));
				}
			}
			else
			{
				this.HideQuickButtons ();
			}
		}

		private void UpdateQuickButtons(Rectangle frame)
		{
			System.Diagnostics.Debug.Assert (frame.IsEmpty == false);

			if (this.quickButtonsFrame == frame)
			{
				return;
			}

			this.quickButtonsFrame = frame;
			this.UpdateQuickButtons ();
		}

		private void HideQuickButtons()
		{
			this.quickButtonsFrame = Rectangle.Empty;
			this.visibleQuickButtons.Clear ();
			this.UpdateQuickButtons ();
		}

		private void UpdateQuickButtons()
		{
			double x = this.quickButtonsFrame.Left + this.scrollList.ActualLocation.X - this.scrollList.Parent.Padding.Left;
			double y = this.quickButtonsFrame.Bottom + this.scrollList.ActualLocation.Y - this.scrollList.Parent.Padding.Bottom;

			double h = this.scrollList.RowHeight;

			foreach (var button in this.QuickButtons)
			{
				if (this.visibleQuickButtons.Contains (button))
				{
					button.Enable = true;
					button.Show ();
					button.Margins = new Margins (x, 0, 0, System.Math.Floor (y + (h - button.PreferredHeight) / 2));

					x += button.PreferredWidth + 2;
				}
				else
				{
					if (button.IsFocused)
					{
						this.scrollList.Focus ();
					}

					button.Enable = false;
					button.Hide ();
				}
			}

			this.scrollList.Invalidate ();
		}

		private void UpdateVisibleButtons()
		{
			int selectionCount = (this.selection == null) ? 0 : this.scrollList.SelectionCount;

			if (this.selectionCount != selectionCount)
			{
				this.selectionCount = selectionCount;
				this.visibleQuickButtons.Clear ();

				if (selectionCount == 1)
				{
					this.visibleQuickButtons.Add (this.quickButtonNegate);
					this.visibleQuickButtons.Add (this.quickButtonAddToGraph);
				}
				else if (selectionCount > 1)
				{
					this.visibleQuickButtons.Add (this.quickButtonSum);
				}
			}
		}


		private void ProcessQuickButton(System.Action<IEnumerable<int>> action)
		{
			this.selection = null;
			this.HideQuickButtons ();
			
			if (action != null)
			{
				action (this.scrollList.GetSortedSelection ());
				
				this.scrollList.Focus ();
				this.scrollList.Invalidate ();
			}
		}

		
		private void NotifyHotRow(int index)
		{
			if (this.hotRowIndex != index)
			{
				//	TODO: handle hover over row in scroll list

				this.hotRowIndex = index;
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


		private IEnumerable<Button>				QuickButtons
		{
			get
			{
				yield return this.quickButtonNegate;
				yield return this.quickButtonAddToGraph;
				yield return this.quickButtonSum;
			}
		}


		public event EventHandler				Changed;
		
		public System.Action<IEnumerable<int>>	SumSeriesAction;
		public System.Action<IEnumerable<int>>	AddSeriesToGraphAction;
		public System.Action<IEnumerable<int>>	NegateSeriesAction;

		readonly private Window					window;
		readonly private ScrollListMultiSelect	scrollList;
		readonly private ChartView				chartView;
		readonly private HashSet<string>		negatedSeriesLabels;
		
		readonly private Button					quickButtonNegate;
		readonly private Button					quickButtonAddToGraph;
		readonly private Button					quickButtonSum;
		readonly private List<Button>			visibleQuickButtons;
		
		private int								hotRowIndex;
		private MultiSelectEventArgs			selection;
		private Rectangle						quickButtonsFrame;
		private int								selectionCount;
	}
}
