//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using Epsitec.Common.Types;

[assembly: Epsitec.Common.Types.DependencyClass (typeof (Epsitec.Common.Widgets.Layouts.GridLayoutEngine))]

namespace Epsitec.Common.Widgets.Layouts
{
	public sealed class GridLayoutEngine : DependencyObject, ILayoutEngine, Types.IListHost<ColumnDefinition>, Types.IListHost<RowDefinition>
	{
		public GridLayoutEngine()
		{
			this.columnDefinitions = new Collections.ColumnDefinitionCollection (this);
			this.rowDefinitions = new Collections.RowDefinitionCollection (this);
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

		/// <summary>
		/// Gets the column count. This property is valid only if the grid has
		/// been measured by the layout system.
		/// </summary>
		/// <value>The column count.</value>
		public int ColumnCount
		{
			get
			{
				return this.columnMeasures.Length;
			}
		}

		/// <summary>
		/// Gets the row count. This property is valid only if the grid has
		/// been measured by the layout system.
		/// </summary>
		/// <value>The row count.</value>
		public int RowCount
		{
			get
			{
				return this.rowMeasures.Length;
			}
		}

		/// <summary>
		/// Gets the index of the first column occupied by a widget.  This
		/// property is valid only if the grid has been measured by the layout
		/// system.
		/// </summary>
		/// <value>The index of the first occupied column, or <c>int.MaxValue</c> if
		/// no widget can be found in the grid.</value>
		public int MinColumnIndex
		{
			get
			{
				return this.minColumnIndex;
			}
		}

		/// <summary>
		/// Gets the index of the last column occupied by a widget.  This
		/// property is valid only if the grid has been measured by the layout
		/// system.
		/// </summary>
		/// <value>The index of the last occupied column, or <c>-1</c> if no widget
		/// can be found in the grid.</value>
		public int MaxColumnIndex
		{
			get
			{
				return this.maxColumnIndex;
			}
		}

		/// <summary>
		/// Gets the index of the first row occupied by a widget.  This
		/// property is valid only if the grid has been measured by the layout
		/// system.
		/// </summary>
		/// <value>The index of the first occupied row, or <c>int.MaxValue</c> if
		/// no widget can be found in the grid.</value>
		public int MinRowIndex
		{
			get
			{
				return this.minRowIndex;
			}
		}

		/// <summary>
		/// Gets the index of the last row occupied by a widget.  This
		/// property is valid only if the grid has been measured by the layout
		/// system.
		/// </summary>
		/// <value>The index of the last occupied row, or <c>-1</c> if no widget
		/// can be found in the grid.</value>
		public int MaxRowIndex
		{
			get
			{
				return this.maxRowIndex;
			}
		}

		#region ILayoutEngine Interface
		
		public void UpdateLayout(Visual container, Drawing.Rectangle rect, IEnumerable<Visual> children)
		{
			double[] x = new double[this.columnMeasures.Length];
			double[] y = new double[this.rowMeasures.Length];
			double[] b = new double[this.rowMeasures.Length];

			this.GenerateColumnOffsets (rect, x);
			this.GenerateRowOffsets (rect, y, b);

			this.LayoutChildren (rect, children, x, y, b);
		}

		public void UpdateMinMax(Visual container, LayoutContext context, IEnumerable<Visual> children, ref Drawing.Size minSize, ref Drawing.Size maxSize)
		{
			this.containerCache = container;
			
			int passId = context.PassId;
			
			MinMaxUpdater updater = new MinMaxUpdater (this, passId);
			
			updater.ProcessChildren (children);
			updater.ConstrainColumns ();
			updater.ConstrainRows ();
			updater.ProcessPendingColumns ();
			updater.ProcessPendingRows ();

			this.columnMeasures = new ColumnMeasure[updater.ColumnCount];
			this.rowMeasures    = new RowMeasure[updater.RowCount];

			this.minColumnIndex = updater.MinColumnIndex;
			this.maxColumnIndex = updater.MaxColumnIndex;
			this.minRowIndex = updater.MinRowIndex;
			this.maxRowIndex = updater.MaxRowIndex;

			updater.ColumnMeasureList.CopyTo (0, this.columnMeasures, 0, updater.ColumnCount);
			updater.RowMeasureList.CopyTo (0, this.rowMeasures, 0, updater.RowCount);

			double minDx = 0;
			double maxDx = 0;

			for (int i = 0; i < this.columnMeasures.Length; i++)
			{
				ColumnMeasure measure = this.columnMeasures[i];

				if (measure.PassId == passId)
				{
					minDx += this.AdjustColumnWidth (measure.Desired, i);
					maxDx += this.AdjustColumnWidth (measure.Max, i);
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
					minDy += this.AdjustRowHeight (measure.Desired, i);
					maxDy += this.AdjustRowHeight (measure.Max, i);
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

		private double AdjustColumnWidth(double width, int column)
		{
			if (column < this.columnDefinitions.Count)
			{
				ColumnDefinition def = this.columnDefinitions[column];

				width += def.LeftBorder;
				width += def.RightBorder;
			}

			return System.Math.Max (0, width);
		}

		private double AdjustRowHeight(double height, int row)
		{
			if (row < this.rowDefinitions.Count)
			{
				RowDefinition def = this.rowDefinitions[row];

				height += def.TopBorder;
				height += def.BottomBorder;
			}

			return System.Math.Max (0, height);
		}

		public LayoutMode LayoutMode
		{
			get
			{
				return LayoutMode.Grid;
			}
		}

		#endregion

		public void InvalidateMeasures()
		{
			if (this.containerCache != null)
			{
				LayoutContext.AddToMeasureQueue (this.containerCache);
				LayoutContext.AddToArrangeQueue (this.containerCache);
			}
		}

		private void GenerateColumnOffsets(Drawing.Rectangle rect, double[] x)
		{
			double dx = 0;
			double flexX = 0;

			for (int i = 0; i < this.columnMeasures.Length; i++)
			{
				if (this.columnMeasures[i] == null)
				{
					this.columnMeasures[i] = new ColumnMeasure (0);
				}

				GridLength length = (i <  this.columnDefinitions.Count) ? this.columnDefinitions[i].Width : GridLength.Auto;
				
				this.columnMeasures[i].UpdateDesired (length.IsAbsolute ? length.Value : 0);
				
				double w = this.columnMeasures[i].Desired;

				x[i] = dx;
				dx  += this.AdjustColumnWidth (w, i);
				
				if (i < this.columnDefinitions.Count)
				{
					x[i] += this.columnDefinitions[i].LeftBorder;
					
					this.columnDefinitions[i].DefineActualOffset (x[i]);
					this.columnDefinitions[i].DefineActualWidth (w);

					if (this.columnDefinitions[i].Width.IsProportional)
					{
						flexX += this.columnDefinitions[i].Width.Value;
					}
				}
			}

			double spaceX = rect.Width - dx;

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
		}

		private void GenerateRowOffsets(Drawing.Rectangle rect, double[] y, double[] b)
		{
			double dy = 0;
			double flexY = 0;

			for (int i = 0; i < this.rowMeasures.Length; i++)
			{
				if (this.rowMeasures[i] == null)
				{
					this.rowMeasures[i] = new RowMeasure (0);
				}

				GridLength length = (i < this.rowDefinitions.Count) ? this.rowDefinitions[i].Height : GridLength.Auto;

				this.rowMeasures[i].UpdateDesired (length.IsAbsolute ? length.Value : 0);

				double h1 = this.rowMeasures[i].MinH1;
				double h2 = this.rowMeasures[i].MinH2;
				double h  = this.rowMeasures[i].Desired;

				dy  += this.AdjustRowHeight (h, i);
				
				y[i] = dy;
				b[i] = (h - (h1+h2)) / 2 + h2;

				if (i < this.rowDefinitions.Count)
				{
					y[i] -= this.rowDefinitions[i].BottomBorder;
					
					this.rowDefinitions[i].DefineActualOffset (y[i]);
					this.rowDefinitions[i].DefineActualHeight (h);

					if (this.rowDefinitions[i].Height.IsProportional)
					{
						flexY += this.rowDefinitions[i].Height.Value;
					}
				}
			}

			double spaceY = rect.Height - dy;

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
		}

		private void LayoutChildren(Drawing.Rectangle rect, IEnumerable<Visual> children, double[] x, double[] y, double[] b)
		{
			foreach (Visual child in children)
			{
				int column = GridLayoutEngine.GetColumn (child);
				int row    = GridLayoutEngine.GetRow (child);

				if ((column < 0) ||
					(row < 0))
				{
					continue;
				}

				this.LayoutChild (rect, x, y, b, child, column, row);
			}
		}

		private void LayoutChild(Drawing.Rectangle rect, double[] x, double[] y, double[] b, Visual child, int column, int row)
		{
			int columnSpan = GridLayoutEngine.GetColumnSpan (child);
			int rowSpan    = GridLayoutEngine.GetRowSpan (child);

			this.LayoutChild (rect, x, y, b, child, column, row, columnSpan, rowSpan);
		}

		private void LayoutChild(Drawing.Rectangle rect, double[] x, double[] y, double[] b, Visual child, int column, int row, int columnSpan, int rowSpan)
		{
			IGridPermeable permeable = child as IGridPermeable;

			if (permeable != null)
			{
				if (permeable.UpdateGridSpan (ref columnSpan, ref rowSpan) == false)
				{
					permeable = null;
				}
			}

			System.Diagnostics.Debug.Assert (column < this.columnMeasures.Length);
			System.Diagnostics.Debug.Assert (row < this.rowMeasures.Length);
			System.Diagnostics.Debug.Assert (columnSpan > 0);
			System.Diagnostics.Debug.Assert (rowSpan > 0);

			Drawing.Margins margins = child.Margins;

			double dx = this.GetColumnSpanWidth (column, columnSpan);
			double dy = this.GetRowSpanHeight (row, rowSpan);

			double ox = rect.Left + x[column];
			double oy = rect.Top - y[row+rowSpan-1];

			Drawing.Rectangle bounds = new Drawing.Rectangle (ox, oy, dx, dy);

			bounds.Deflate (margins);
			DockLayoutEngine.SetChildBounds (child, bounds, b[row]);

			if (permeable != null)
			{
				rect = bounds;

				rect.X = -x[column];
				rect.Y =  y[row+rowSpan-1] - rect.Height;

				foreach (PermeableCell cell in permeable.GetChildren (column, row, columnSpan, rowSpan))
				{
					if (cell.Visual != null)
					{
						this.LayoutChild (rect, x, y, b, cell.Visual, cell.Column, cell.Row, cell.ColumnSpan, cell.RowSpan);
					}
				}
			}
		}

		private double GetColumnSpanWidth(int column, int columnSpan)
		{
			double dx = 0;

			for (int i = 0; i < columnSpan; i++)
			{
				System.Diagnostics.Debug.Assert (this.columnMeasures[column+i] != null);

				if ((i > 0) &&
					(column+i-1 < this.columnDefinitions.Count))
				{
					dx += this.columnDefinitions[column+i-1].LeftBorder;
				}

				if ((i+1 < columnSpan) &&
					(column+i < this.columnDefinitions.Count))
				{
					dx += this.columnDefinitions[column+i].RightBorder;
				}

				dx += this.columnMeasures[column+i].Desired;
			}

			return dx;
		}

		private double GetRowSpanHeight(int row, int rowSpan)
		{
			double dy = 0;
			
			for (int i = 0; i < rowSpan; i++)
			{
				System.Diagnostics.Debug.Assert (this.rowMeasures[row+i] != null);

				if ((i > 0) &&
					(row+i-1 < this.rowDefinitions.Count))
				{
					dy += this.rowDefinitions[row+i-1].TopBorder;
				}

				if ((i+1 < rowSpan) &&
					(row+i < this.rowDefinitions.Count))
				{
					dy += this.rowDefinitions[row+i].BottomBorder;
				}

				dy += this.rowMeasures[row+i].Desired;
			}
			
			return dy;
		}

		#region MinMaxUpdater Class

		private class MinMaxUpdater
		{
			public MinMaxUpdater(GridLayoutEngine grid, int passId)
			{
				this.grid        = grid;
				this.passId      = passId;
				this.columnCount = grid.ColumnDefinitions.Count;
				this.rowCount    = grid.RowDefinitions.Count;

				this.minColumnIndex = int.MaxValue;
				this.maxColumnIndex = -1;
				this.minRowIndex = int.MaxValue;
				this.maxRowIndex = -1;
			}

			public int ColumnCount
			{
				get
				{
					return this.columnCount;
				}
			}

			public int RowCount
			{
				get
				{
					return this.rowCount;
				}
			}

			public int MinColumnIndex
			{
				get
				{
					return this.minColumnIndex;
				}
			}

			public int MaxColumnIndex
			{
				get
				{
					return this.maxColumnIndex;
				}
			}

			public int MinRowIndex
			{
				get
				{
					return this.minRowIndex;
				}
			}

			public int MaxRowIndex
			{
				get
				{
					return this.maxRowIndex;
				}
			}
			
			public List<ColumnMeasure> ColumnMeasureList
			{
				get
				{
					return this.columnMeasureList;
				}
			}

			public List<RowMeasure> RowMeasureList
			{
				get
				{
					return this.rowMeasureList;
				}
			}

			public void ProcessChildren(IEnumerable<Visual> children)
			{
				foreach (Visual child in children)
				{
					int column = GridLayoutEngine.GetColumn (child);
					int row    = GridLayoutEngine.GetRow (child);

					if ((column < 0) ||
						(row < 0))
					{
						continue;
					}

					this.ProcessChild (child, column, row);
				}
			}

			private void ProcessChild(Visual child, int column, int row)
			{
				IGridPermeable permeable = child as IGridPermeable;

				int columnSpan = GridLayoutEngine.GetColumnSpan (child);
				int rowSpan    = GridLayoutEngine.GetRowSpan (child);

				if (permeable != null)
				{
					if (permeable.UpdateGridSpan (ref columnSpan, ref rowSpan))
					{
						foreach (PermeableCell cell in permeable.GetChildren (column, row, columnSpan, rowSpan))
						{
							if (cell.Visual != null)
							{
								this.ProcessChild (cell.Visual, cell.Column, cell.Row);
							}
						}
					}
				}

				if (column+columnSpan > this.columnCount)
				{
					this.columnCount = column+columnSpan;
				}
				if (column+columnSpan-1 > this.maxColumnIndex)
				{
					this.maxColumnIndex = column+columnSpan-1;
				}
				if (column < this.minColumnIndex)
				{
					this.minColumnIndex = column;
				}
				if (row+rowSpan > this.rowCount)
				{
					this.rowCount = row+rowSpan;
				}
				if (row+rowSpan-1 > this.maxRowIndex)
				{
					this.maxRowIndex = row+rowSpan-1;
				}
				if (row < this.minRowIndex)
				{
					this.minRowIndex = row;
				}

				Layouts.LayoutMeasure measureDx = Layouts.LayoutMeasure.GetWidth (child);
				Layouts.LayoutMeasure measureDy = Layouts.LayoutMeasure.GetHeight (child);

				Drawing.Margins margins = child.Margins;

				if (columnSpan == 1)
				{
					ColumnMeasure columnMeasure = this.GetColumnMeasure (column);
					bool auto = column < this.grid.ColumnDefinitions.Count ? this.grid.ColumnDefinitions[column].Width.IsAuto : true;

					columnMeasure.UpdateMin (this.passId, (auto ? measureDx.Desired : measureDx.Min) + margins.Width);
					columnMeasure.UpdateMax (this.passId, measureDx.Max + margins.Width);
					columnMeasure.UpdatePassId (this.passId);
				}
				else
				{
					this.pendingColumns.Add (new Info (child, measureDx, column, columnSpan));
				}

				if (rowSpan == 1)
				{
					RowMeasure rowMeasure = this.GetRowMeasure (row);
					bool auto = row < this.grid.RowDefinitions.Count ? this.grid.RowDefinitions[row].Height.IsAuto : true;

					rowMeasure.UpdateMin (this.passId, (auto ? measureDy.Desired : measureDy.Min) + margins.Height);
					rowMeasure.UpdateMax (this.passId, measureDy.Max + margins.Height);
					rowMeasure.UpdatePassId (this.passId);

					if (child.VerticalAlignment == VerticalAlignment.BaseLine)
					{
						double h1;
						double h2;

						LayoutContext.GetMeasuredBaseLine (child, out h1, out h2);

						rowMeasure.UpdateMinH1H2 (this.passId, h1, h2);
					}
				}
				else
				{
					this.pendingRows.Add (new Info (child, measureDy, row, rowSpan));
				}
			}

			public void ConstrainColumns()
			{
				int nColumns = this.grid.columnDefinitions.Count;

				if (nColumns > this.columnCount)
				{
					this.columnCount = nColumns;
				}

				for (int i = 0; i < nColumns; i++)
				{
					ColumnMeasure measure = this.GetColumnMeasure (i);
					GridLength length = this.grid.columnDefinitions[i].Width;

					measure.UpdateMin (this.passId, this.grid.columnDefinitions[i].MinWidth);
					measure.UpdateMax (this.passId, this.grid.columnDefinitions[i].MaxWidth);
					measure.UpdateDesired (length.IsAbsolute ? length.Value : 0);
					measure.UpdatePassId (this.passId);
				}
			}

			public void ConstrainRows()
			{
				int nRows = this.grid.rowDefinitions.Count;

				if (nRows > this.rowCount)
				{
					this.rowCount = nRows;
				}

				for (int i = 0; i < nRows; i++)
				{
					RowMeasure measure = this.GetRowMeasure (i);
					GridLength length = this.grid.rowDefinitions[i].Height;

					measure.UpdateMin (this.passId, this.grid.rowDefinitions[i].MinHeight);
					measure.UpdateMax (this.passId, this.grid.rowDefinitions[i].MaxHeight);
					measure.UpdateDesired (length.IsAbsolute ? length.Value : 0);
					measure.UpdatePassId (this.passId);
				}
			}

			public void ProcessPendingColumns()
			{
				if (this.pendingColumns.Count > 0)
				{
					this.pendingColumns.Sort ();

					foreach (Info info in this.pendingColumns)
					{
						double dx = this.GetColumnSpanWidth (info.Index, info.Span);

						if (dx < info.Measure.Desired)
						{
							//	The widget needs more room than what has been granted to it.
							//	Distribute the excess space evenly.

							int[] index = this.GetFlexibleColumns (info.Index, info.Span);
							double space = (info.Measure.Desired - dx) / index.Length;
							
							//	TODO: result is not perfect, since we mix UpdateMin with some
							//	columns which could define a Desired value different from the
							//	minimum.

							for (int i = 0; i < index.Length; i++)
							{
								LayoutMeasure measure = this.GetColumnMeasure (index[i]);
								measure.UpdateMin (this.passId, measure.Min + space);
							}
						}
					}
				}
			}

			public void ProcessPendingRows()
			{
				if (this.pendingRows.Count > 0)
				{
					this.pendingRows.Sort ();

					foreach (Info info in this.pendingRows)
					{
						double dy = this.GetRowSpanHeight (info.Index, info.Span);

						if (dy < info.Measure.Desired)
						{
							//	The widget needs more room than what has been granted to it.
							//	Distribute the excess space evenly.

							int[] index = this.GetFlexibleRows (info.Index, info.Span);
							double space = (info.Measure.Desired - dy) / index.Length;

							//	TODO: result is not perfect, since we mix UpdateMin with some
							//	columns which could define a Desired value different from the
							//	minimum.

							for (int i = 0; i < index.Length; i++)
							{
								LayoutMeasure measure = this.GetRowMeasure (index[i]);
								measure.UpdateMin (this.passId, measure.Min + space);
							}
						}
					}
				}
			}

			private int[] GetFlexibleColumns(int index, int span)
			{
				List<int> list = new List<int> ();

				//	We try to expand the proportional columns first, if there
				//	are any. If not, consider the automatic columns; if everything
				//	fails, just use all columns.
				
				for (int i = 0; i < span; i++)
				{
					int col = index+i;

					if (col < this.grid.ColumnDefinitions.Count)
					{
						if (this.grid.ColumnDefinitions[col].Width.IsProportional)
						{
							list.Add (col);
						}
					}
				}

				if (list.Count > 0)
				{
					return list.ToArray ();
				}

				for (int i = 0; i < span; i++)
				{
					int col = index+i;

					if (col < this.grid.ColumnDefinitions.Count)
					{
						if (this.grid.ColumnDefinitions[col].Width.IsAuto)
						{
							list.Add (col);
						}
					}
				}

				if (list.Count > 0)
				{
					return list.ToArray ();
				}

				int[] array = new int[span];

				for (int i = 0; i < span; i++)
				{
					array[i] = index+i;
				}

				return array;
			}

			private int[] GetFlexibleRows(int index, int span)
			{
				List<int> list = new List<int> ();

				//	We try to expand the proportional rows first, if there
				//	are any. If not, consider the automatic rows; if everything
				//	fails, just use all rows.
				
				for (int i = 0; i < span; i++)
				{
					int row = index+i;

					if (row < this.grid.RowDefinitions.Count)
					{
						if (this.grid.RowDefinitions[row].Height.IsProportional)
						{
							list.Add (row);
						}
					}
				}

				if (list.Count > 0)
				{
					return list.ToArray ();
				}

				for (int i = 0; i < span; i++)
				{
					int row = index+i;

					if (row < this.grid.RowDefinitions.Count)
					{
						if (this.grid.RowDefinitions[row].Height.IsAuto)
						{
							list.Add (row);
						}
					}
				}

				if (list.Count > 0)
				{
					return list.ToArray ();
				}

				int[] array = new int[span];

				for (int i = 0; i < span; i++)
				{
					array[i] = index+i;
				}

				return array;
			}

			private double GetColumnSpanWidth(int column, int span)
			{
				double dx = 0;

				for (int i = 0; i < span; i++)
				{
					if ((i > 0) &&
						(column+i-1 < this.grid.columnDefinitions.Count))
					{
						dx += this.grid.columnDefinitions[column+i-1].LeftBorder;
					}

					if ((i+1 < span) &&
						(column+i < this.grid.columnDefinitions.Count))
					{
						dx += this.grid.columnDefinitions[column+i].RightBorder;
					}
					
					dx += this.GetColumnMeasure (column+i).Desired;
				}
				
				return dx;
			}

			private double GetRowSpanHeight(int row, int span)
			{
				double dy = 0;

				for (int i = 0; i < span; i++)
				{
					if ((i > 0) &&
						(row+i-1 < this.grid.rowDefinitions.Count))
					{
						dy += this.grid.rowDefinitions[row+i-1].TopBorder;
					}

					if ((i+1 < span) &&
						(row+i < this.grid.rowDefinitions.Count))
					{
						dy += this.grid.rowDefinitions[row+i].BottomBorder;
					}

					dy += this.GetRowMeasure (row+i).Desired;
				}

				return dy;
			}

			private ColumnMeasure GetColumnMeasure(int column)
			{
				while (column >= this.columnMeasureList.Count)
				{
					ColumnMeasure measure;

					if (this.columnMeasureList.Count >= this.grid.columnMeasures.Length)
					{
						measure = new ColumnMeasure (this.passId);
					}
					else
					{
						measure = this.grid.columnMeasures[this.columnMeasureList.Count];

						if (measure == null)
						{
							measure = new ColumnMeasure (this.passId);
						}
					}

					if (double.IsNaN (measure.Desired))
					{
						measure.UpdateDesired (0);
					}

					this.columnMeasureList.Add (measure);
				}

				return this.columnMeasureList[column];
			}

			private RowMeasure GetRowMeasure(int row)
			{
				while (row >= this.rowMeasureList.Count)
				{
					RowMeasure measure;

					if (this.rowMeasureList.Count >= this.grid.rowMeasures.Length)
					{
						measure = new RowMeasure (this.passId);
					}
					else
					{
						measure = this.grid.rowMeasures[this.rowMeasureList.Count];

						if (measure == null)
						{
							measure = new RowMeasure (this.passId);
						}
					}

					if (double.IsNaN (measure.Desired))
					{
						measure.UpdateDesired (0);
					}

					this.rowMeasureList.Add (measure);
				}

				return this.rowMeasureList[row];
			}

			GridLayoutEngine grid;
			List<ColumnMeasure> columnMeasureList = new List<ColumnMeasure> ();
			List<RowMeasure> rowMeasureList = new List<RowMeasure> ();

			List<Info> pendingColumns = new List<Info> ();
			List<Info> pendingRows = new List<Info> ();

			int passId;
			int columnCount;
			int rowCount;
			int minRowIndex, maxRowIndex;
			int minColumnIndex, maxColumnIndex;
		}

		#endregion

		#region Info Structure
		
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

		#region IListHost<ColumnDefinition> Members

		Epsitec.Common.Types.Collections.HostedList<ColumnDefinition> IListHost<ColumnDefinition>.Items
		{
			get
			{
				return this.ColumnDefinitions;
			}
		}

		void IListHost<ColumnDefinition>.NotifyListInsertion(ColumnDefinition item)
		{
			this.InvalidateMeasures ();
			item.Changed += this.HandleColumnDefinitionChanged;
		}

		void IListHost<ColumnDefinition>.NotifyListRemoval(ColumnDefinition item)
		{
			item.Changed -= this.HandleColumnDefinitionChanged;
			this.InvalidateMeasures ();
		}

		#endregion

		#region IListHost<RowDefinition> Members

		Epsitec.Common.Types.Collections.HostedList<RowDefinition> IListHost<RowDefinition>.Items
		{
			get
			{
				return this.RowDefinitions;
			}
		}

		void IListHost<RowDefinition>.NotifyListInsertion(RowDefinition item)
		{
			this.InvalidateMeasures ();
			item.Changed += this.HandleRowDefinitionChanged;
		}

		void IListHost<RowDefinition>.NotifyListRemoval(RowDefinition item)
		{
			item.Changed -= this.HandleRowDefinitionChanged;
			this.InvalidateMeasures ();
		}

		#endregion

		private void HandleRowDefinitionChanged(object sender)
		{
			this.InvalidateMeasures ();
		}

		private void HandleColumnDefinitionChanged(object sender)
		{
			this.InvalidateMeasures ();
		}

		public static void SetColumn(Visual visual, int column)
		{
			if (GridLayoutEngine.GetColumn (visual) == column)
			{
				return;
			}

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
			if (GridLayoutEngine.GetRow (visual) == row)
			{
				return;
			}
			
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
			if (GridLayoutEngine.GetColumnSpan (visual) == span)
			{
				return;
			}

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
			if (GridLayoutEngine.GetRowSpan (visual) == span)
			{
				return;
			}

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

		private static void NotifyGridPropertyInvalidated(DependencyObject obj, object oldValue, object newValue)
		{
			Visual visual = obj as Visual;
			Visual grid   = visual == null ? null : visual.Parent;

			if (grid != null)
			{
				LayoutContext.AddToMeasureQueue (grid);
				LayoutContext.AddToArrangeQueue (grid);
			}
		}

		private static object GetColumnDefinitionsValue(DependencyObject obj)
		{
			GridLayoutEngine that = (GridLayoutEngine) obj;
			return that.ColumnDefinitions;
		}

		private static object GetRowDefinitionsValue(DependencyObject obj)
		{
			GridLayoutEngine that = (GridLayoutEngine) obj;
			return that.RowDefinitions;
		}

		public static readonly DependencyProperty ColumnProperty = DependencyProperty.RegisterAttached ("Column", typeof (int), typeof (GridLayoutEngine), new DependencyPropertyMetadata (-1, GridLayoutEngine.NotifyGridPropertyInvalidated));
		public static readonly DependencyProperty RowProperty = DependencyProperty.RegisterAttached ("Row", typeof (int), typeof (GridLayoutEngine), new DependencyPropertyMetadata (-1, GridLayoutEngine.NotifyGridPropertyInvalidated));
		public static readonly DependencyProperty ColumnSpanProperty = DependencyProperty.RegisterAttached ("ColumnSpan", typeof (int), typeof (GridLayoutEngine), new DependencyPropertyMetadata (1, GridLayoutEngine.NotifyGridPropertyInvalidated));
		public static readonly DependencyProperty RowSpanProperty = DependencyProperty.RegisterAttached ("RowSpan", typeof (int), typeof (GridLayoutEngine), new DependencyPropertyMetadata (1, GridLayoutEngine.NotifyGridPropertyInvalidated));
		public static readonly DependencyProperty ColumnDefinitionsProperty = DependencyProperty.RegisterReadOnly ("ColumnDefinitions", typeof (Collections.ColumnDefinitionCollection), typeof (GridLayoutEngine), new DependencyPropertyMetadata (GridLayoutEngine.GetRowDefinitionsValue).MakeReadOnlySerializable ());
		public static readonly DependencyProperty RowDefinitionsProperty = DependencyProperty.RegisterReadOnly ("RowDefinitions", typeof (Collections.RowDefinitionCollection), typeof (GridLayoutEngine), new DependencyPropertyMetadata (GridLayoutEngine.GetColumnDefinitionsValue).MakeReadOnlySerializable ());

		private ColumnMeasure[] columnMeasures = new ColumnMeasure[0];
		private RowMeasure[]	rowMeasures    = new RowMeasure[0];
		private Visual			containerCache;
		
		private int				minRowIndex, maxRowIndex;
		private int				minColumnIndex, maxColumnIndex;
		
		private Collections.ColumnDefinitionCollection columnDefinitions;
		private Collections.RowDefinitionCollection rowDefinitions;
	}
}
