//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;
using Epsitec.Common.Types;

namespace Epsitec.Common.Widgets.Layouts
{
	public sealed class GridLayoutEngine : DependencyObject, ILayoutEngine
	{
		public GridLayoutEngine()
		{
		}

		public Collections.ColumnDefinitionCollection ColumnDefinitions
		{
			get
			{
				return this.columnDefinitions;
			}
		}

		public Collections.RowDefinitionCollection RowDefinitions
		{
			get
			{
				return this.rowDefinitions;
			}
		}

		#region ILayoutEngine Interface
		
		public void UpdateLayout(Visual container, Drawing.Rectangle rect, IEnumerable<Visual> children)
		{
			double[] x = new double[this.columnMeasures.Length];
			double[] y = new double[this.rowMeasures.Length];
			double[] b = new double[this.rowMeasures.Length];
			
			double dx = 0;
			double flexX = 0;
			double dy = 0;
			double flexY = 0;

			for (int i = 0; i < this.columnMeasures.Length; i++)
			{
				this.columnMeasures[i].UpdateDesired (0);
				
				double w = this.columnMeasures[i].Desired;

				x[i] = dx;
				dx  += w;

				if (i < this.columnDefinitions.Count)
				{
					this.columnDefinitions[i].DefineActualOffset (x[i]);
					this.columnDefinitions[i].DefineActualWidth (w);
					
					if (this.columnDefinitions[i].Width.IsProportional)
					{
						flexX += this.columnDefinitions[i].Width.Value;
					}
				}
			}

			for (int i = 0; i < this.rowMeasures.Length; i++)
			{
				this.rowMeasures[i].UpdateDesired (0);
				
				double h1 = this.rowMeasures[i].MinH1;
				double h2 = this.rowMeasures[i].MinH2;
				double h  = this.rowMeasures[i].Desired;

				dy  += h;
				
				y[i] = dy;
				b[i] = (h - (h1+h2)) / 2 + h2;
				
				if (i < this.rowDefinitions.Count)
				{
					this.rowDefinitions[i].DefineActualOffset (y[i]);
					this.rowDefinitions[i].DefineActualHeight (h);

					if (this.rowDefinitions[i].Height.IsProportional)
					{
						flexY += this.rowDefinitions[i].Height.Value;
					}
				}
			}

			double spaceX = rect.Width - dx;
			double spaceY = rect.Height - dy;

			if ((spaceX > 0) &&
				(flexX > 0))
			{
				double move = 0;

				for (int i = 0; i < this.columnDefinitions.Count; i++)
				{
					x[i] += move;
					this.columnDefinitions[i].DefineActualOffset (x[i]);
					
					if (this.columnDefinitions[i].Width.IsProportional)
					{
						double d = this.columnDefinitions[i].Width.Value * spaceX / flexX;
						double w = this.columnDefinitions[i].ActualWidth + d;

						this.columnMeasures[i].UpdateDesired (w);
						this.columnDefinitions[i].DefineActualWidth (w);

						move += d;
					}
				}

				for (int i = this.columnDefinitions.Count; i < x.Length; i++)
				{
					x[i] += move;
				}

				dx += move;
			}
			
			if ((spaceY > 0) &&
				(flexY > 0))
			{
				double move = 0;
				
				for (int i = 0; i < this.rowDefinitions.Count; i++)
				{
					if (this.rowDefinitions[i].Height.IsProportional)
					{
						double d = this.rowDefinitions[i].Height.Value * spaceY / flexY;
						double h = this.rowDefinitions[i].ActualHeight + d;
						
						this.rowMeasures[i].UpdateDesired (h);
						this.rowDefinitions[i].DefineActualHeight (h);
						
						move += d;
					}

					y[i] += move;
					this.rowDefinitions[i].DefineActualOffset (y[i]);
				}
				
				for (int i = this.rowDefinitions.Count; i < y.Length; i++)
				{
					y[i] += move;
				}

				dy += move;
			}
			
			foreach (Visual child in children)
			{
				int column = GridLayoutEngine.GetColumn (child);
				int row    = GridLayoutEngine.GetRow (child);

				if ((column < 0) ||
					(row < 0))
				{
					continue;
				}

				int columnSpan = GridLayoutEngine.GetColumnSpan (child);
				int rowSpan = GridLayoutEngine.GetRowSpan (child);

				System.Diagnostics.Debug.Assert (column < this.columnMeasures.Length);
				System.Diagnostics.Debug.Assert (row < this.rowMeasures.Length);
				System.Diagnostics.Debug.Assert (columnSpan > 0);
				System.Diagnostics.Debug.Assert (rowSpan > 0);

				Drawing.Margins margins = child.Margins;

				dx = 0;
				dy = 0;

				for (int i = 0; i < columnSpan; i++)
				{
					System.Diagnostics.Debug.Assert (this.columnMeasures[column+i] != null);
					dx += this.columnMeasures[column+i].Desired;
				}
				for (int i = 0; i < rowSpan; i++)
				{
					System.Diagnostics.Debug.Assert (this.rowMeasures[row+i] != null);
					dy += this.rowMeasures[row+i].Desired;
				}

				Drawing.Rectangle bounds = new Drawing.Rectangle (rect.Left+x[column], rect.Top-y[row+rowSpan-1], dx, dy);
				
				bounds.Deflate (margins);
				DockLayoutEngine.SetChildBounds (child, bounds, b[row]);
			}
		}

		public void UpdateMinMax(Visual container, LayoutContext context, IEnumerable<Visual> children, ref Drawing.Size minSize, ref Drawing.Size maxSize)
		{
			List<ColumnMeasure> columnMeasureList = new List<ColumnMeasure> ();
			List<RowMeasure>    rowMeasureList = new List<RowMeasure> ();

			List<Info> pendingColumns = new List<Info> ();
			List<Info> pendingRows = new List<Info> ();
			
			int passId = context.PassId;
			int columnCount = 0;
			int rowCount = 0;
			
			foreach (Visual child in children)
			{
				int column = GridLayoutEngine.GetColumn (child);
				int row    = GridLayoutEngine.GetRow (child);

				if ((column < 0) ||
					(row < 0))
				{
					continue;
				}

				int columnSpan = GridLayoutEngine.GetColumnSpan (child);
				int rowSpan = GridLayoutEngine.GetRowSpan (child);

				if (column+columnSpan > columnCount)
				{
					columnCount = column+columnSpan;
				}
				if (row+rowSpan > rowCount)
				{
					rowCount = row+rowSpan;
				}

				Layouts.LayoutMeasure measureDx = Layouts.LayoutMeasure.GetWidth (child);
				Layouts.LayoutMeasure measureDy = Layouts.LayoutMeasure.GetHeight (child);

				Drawing.Margins margins = child.Margins;

				if (columnSpan == 1)
				{
					ColumnMeasure columnMeasure = this.GetColumnMeasure (columnMeasureList, passId, column);

					columnMeasure.UpdateMin (passId, measureDx.Desired + margins.Width);
					columnMeasure.UpdateMax (passId, measureDx.Max + margins.Width);
					columnMeasure.UpdatePassId (passId);
				}
				else
				{
					pendingColumns.Add (new Info (child, measureDx, column, columnSpan));
				}

				if (rowSpan == 1)
				{
					RowMeasure rowMeasure = this.GetRowMeasure (rowMeasureList, passId, row);

					rowMeasure.UpdateMin (passId, measureDy.Desired + margins.Height);
					rowMeasure.UpdateMax (passId, measureDy.Max + margins.Height);
					rowMeasure.UpdatePassId (passId);

					if (child.VerticalAlignment == VerticalAlignment.BaseLine)
					{
						double h1;
						double h2;

						LayoutContext.GetMeasuredBaseLine (child, out h1, out h2);

						rowMeasure.UpdateMinH1H2 (passId, h1, h2);
					}
				}
				else
				{
					pendingRows.Add (new Info (child, measureDy, row, rowSpan));
				}
			}

			int nColumns = System.Math.Min (this.columnDefinitions.Count, columnCount);
			int nRows = System.Math.Min (this.rowDefinitions.Count, rowCount);

			for (int i = 0; i < nColumns; i++)
			{
				ColumnMeasure measure = this.GetColumnMeasure (columnMeasureList, passId, i);

				measure.UpdateMin (passId, this.columnDefinitions[i].MinWidth);
				measure.UpdateMax (passId, this.columnDefinitions[i].MaxWidth);
				measure.UpdateDesired (0);
				measure.UpdatePassId (passId);
			}

			for (int i = 0; i < nRows; i++)
			{
				RowMeasure measure = this.GetRowMeasure (rowMeasureList, passId, i);

				measure.UpdateMin (passId, this.rowDefinitions[i].MinHeight);
				measure.UpdateMax (passId, this.rowDefinitions[i].MaxHeight);
				measure.UpdateDesired (0);
				measure.UpdatePassId (passId);
			}

			if (pendingColumns.Count > 0)
			{
				pendingColumns.Sort ();

				foreach (Info info in pendingColumns)
				{
					double dx = 0;
					
					for (int i = 0; i < info.Span; i++)
					{
						dx += this.GetColumnMeasure (columnMeasureList, passId, info.Index + i).Desired;
					}

					if (dx < info.Measure.Desired)
					{
						//	The widget needs more room than what has been granted to it.
						//	Distribute the excess space evenly.

						double space = (info.Measure.Desired - dx) / info.Span;

						for (int i = 0; i < info.Span; i++)
						{
							LayoutMeasure measure = this.GetColumnMeasure (columnMeasureList, passId, info.Index + i);
							measure.UpdateMin (passId, measure.Min + space);
						}
					}
				}
			}

			if (pendingRows.Count > 0)
			{
				pendingRows.Sort ();

				foreach (Info info in pendingRows)
				{
					double dy = 0;

					for (int i = 0; i < info.Span; i++)
					{
						dy += this.GetRowMeasure (rowMeasureList, passId, info.Index + i).Desired;
					}

					if (dy < info.Measure.Desired)
					{
						//	The widget needs more room than what has been granted to it.
						//	Distribute the excess space evenly.

						double space = (info.Measure.Desired - dy) / info.Span;

						for (int i = 0; i < info.Span; i++)
						{
							LayoutMeasure measure = this.GetRowMeasure (rowMeasureList, passId, info.Index + i);
							measure.UpdateMin (passId, measure.Min + space);
						}
					}
				}
			}

			
			
			this.columnMeasures = new ColumnMeasure[columnCount];
			this.rowMeasures    = new RowMeasure[rowCount];
			
			columnMeasureList.CopyTo (0, this.columnMeasures, 0, columnCount);
			rowMeasureList.CopyTo (0, this.rowMeasures, 0, rowCount);

			double minDx = 0;
			double maxDx = 0;

			for (int i = 0; i < this.columnMeasures.Length; i++)
			{
				ColumnMeasure measure = this.columnMeasures[i];

				if (measure.PassId == passId)
				{
					minDx += measure.Desired;
					maxDx += measure.Max;
				}
				else
				{
					this.columnMeasures[i] = null;
				}
			}

			double minDy = 0;
			double maxDy = 0;

			for (int i = 0; i < this.rowMeasures.Length; i++)
			{
				RowMeasure measure = this.rowMeasures[i];

				if (measure.PassId == passId)
				{
					minDy += measure.Desired;
					maxDy += measure.Max;
				}
				else
				{
					this.rowMeasures[i] = null;
				}
			}

			minSize.Width  = System.Math.Max (minSize.Width,  minDx);
			minSize.Height = System.Math.Max (minSize.Height, minDy);
			maxSize.Width  = System.Math.Min (maxSize.Width,  maxDx);
			maxSize.Height = System.Math.Min (maxSize.Height, maxDy);
		}

		private struct Info : System.IComparable<Info>
		{
			public Info(Visual visual, LayoutMeasure measure, int index, int span)
			{
				this.visual = visual;
				this.measure = measure;
				this.index = index;
				this.span = span;
			}

			public Visual Visual
			{
				get
				{
					return this.visual;
				}
			}

			public LayoutMeasure Measure
			{
				get
				{
					return this.measure;
				}
			}

			public int Index
			{
				get
				{
					return this.index;
				}
			}

			public int Span
			{
				get
				{
					return this.span;
				}
			}
			
			Visual visual;
			LayoutMeasure measure;
			int index;
			int span;

			#region IComparable<Info> Members

			int System.IComparable<Info>.CompareTo(Info other)
			{
				if (this.span < other.span)
				{
					return -1;
				}
				if (this.span > other.span)
				{
					return 1;
				}
				if (this.index < other.index)
				{
					return -1;
				}
				if (this.index > other.index)
				{
					return 1;
				}

				return 0;
			}

			#endregion
		}

		private ColumnMeasure GetColumnMeasure(List<ColumnMeasure> list, int passId, int column)
		{
			while (column >= list.Count)
			{
				ColumnMeasure measure;
				
				if (list.Count >= this.columnMeasures.Length)
				{
					measure = new ColumnMeasure (passId);
				}
				else
				{
					measure = this.columnMeasures[list.Count];
					
					if (measure == null)
					{
						measure = new ColumnMeasure (passId);
					}
				}

				if (double.IsNaN (measure.Desired))
				{
					measure.UpdateDesired (0);
				}
				
				list.Add (measure);
			}

			return list[column];
		}

		private RowMeasure GetRowMeasure(List<RowMeasure> list, int passId, int row)
		{
			while (row >= list.Count)
			{
				RowMeasure measure;

				if (list.Count >= this.rowMeasures.Length)
				{
					measure = new RowMeasure (passId);
				}
				else
				{
					measure = this.rowMeasures[list.Count];

					if (measure == null)
					{
						measure = new RowMeasure (passId);
					}
				}

				if (double.IsNaN (measure.Desired))
				{
					measure.UpdateDesired (0);
				}

				list.Add (measure);
			}

			return list[row];
		}

		#endregion

		#region ColumnMeasure Class

		private class ColumnMeasure : LayoutMeasure
		{
			public ColumnMeasure(int passId) : base (passId)
			{
			}
		}

		#endregion

		#region RowMeasure Class

		private class RowMeasure : LayoutMeasure
		{
			public RowMeasure(int passId) : base (passId)
			{
			}

			public double MinH1
			{
				get
				{
					return this.minH1;
				}
			}

			public double MinH2
			{
				get
				{
					return this.minH2;
				}
			}

			public void UpdateMinH1H2(int passId, double h1, double h2)
			{
				double oldH1 = this.minH1;
				double oldH2 = this.minH2;

				if (this.PassId == passId)
				{
					this.minH1 = System.Math.Max (oldH1, h1);
					this.minH2 = System.Math.Max (oldH2, h2);
				}
				else
				{
					this.minH1 = h1;
					this.minH2 = h2;
				}

				if ((this.minH1 != oldH1) ||
					(this.minH2 != oldH2))
				{
					this.UpdateMin (passId, this.minH1 + this.minH2);
					this.SetHasChanged ();
				}
			}
			
			double minH1;
			double minH2;
		}

		#endregion

		public static void SetColumn(Visual visual, int column)
		{
			if (column == -1)
			{
				visual.ClearValue (GridLayoutEngine.ColumnProperty);
			}
			else
			{
				visual.SetValue (GridLayoutEngine.ColumnProperty, column);
			}
		}

		public static void SetRow(Visual visual, int row)
		{
			if (row == -1)
			{
				visual.ClearValue (GridLayoutEngine.RowProperty);
			}
			else
			{
				visual.SetValue (GridLayoutEngine.RowProperty, row);
			}
		}

		public static void SetColumnSpan(Visual visual, int span)
		{
			if (span == 1)
			{
				visual.ClearValue (GridLayoutEngine.ColumnSpanProperty);
			}
			else
			{
				visual.SetValue (GridLayoutEngine.ColumnSpanProperty, span);
			}
		}

		public static void SetRowSpan(Visual visual, int span)
		{
			if (span == 1)
			{
				visual.ClearValue (GridLayoutEngine.RowSpanProperty);
			}
			else
			{
				visual.SetValue (GridLayoutEngine.RowSpanProperty, span);
			}
		}

		public static int GetColumn(Visual visual)
		{
			object value;

			if (visual.TryGetLocalValue (GridLayoutEngine.ColumnProperty, out value))
			{
				return (int) value;
			}
			else
			{
				return -1;
			}
		}

		public static int GetRow(Visual visual)
		{
			object value;

			if (visual.TryGetLocalValue (GridLayoutEngine.RowProperty, out value))
			{
				return (int) value;
			}
			else
			{
				return -1;
			}
		}

		public static int GetColumnSpan(Visual visual)
		{
			object value;

			if (visual.TryGetLocalValue (GridLayoutEngine.ColumnSpanProperty, out value))
			{
				return (int) value;
			}
			else
			{
				return 1;
			}
		}

		public static int GetRowSpan(Visual visual)
		{
			object value;

			if (visual.TryGetLocalValue (GridLayoutEngine.RowSpanProperty, out value))
			{
				return (int) value;
			}
			else
			{
				return 1;
			}
		}

		public static readonly DependencyProperty ColumnProperty = DependencyProperty.RegisterAttached ("Column", typeof (int), typeof (GridLayoutEngine), new DependencyPropertyMetadata (-1));
		public static readonly DependencyProperty RowProperty = DependencyProperty.RegisterAttached ("Row", typeof (int), typeof (GridLayoutEngine), new DependencyPropertyMetadata (-1));
		public static readonly DependencyProperty ColumnSpanProperty = DependencyProperty.RegisterAttached ("ColumnSpan", typeof (int), typeof (GridLayoutEngine), new DependencyPropertyMetadata (1));
		public static readonly DependencyProperty RowSpanProperty = DependencyProperty.RegisterAttached ("RowSpan", typeof (int), typeof (GridLayoutEngine), new DependencyPropertyMetadata (1));

		private ColumnMeasure[] columnMeasures = new ColumnMeasure[0];
		private RowMeasure[]	rowMeasures    = new RowMeasure[0];

		private Collections.ColumnDefinitionCollection columnDefinitions = new Collections.ColumnDefinitionCollection ();
		private Collections.RowDefinitionCollection rowDefinitions = new Collections.RowDefinitionCollection ();
	}
}
