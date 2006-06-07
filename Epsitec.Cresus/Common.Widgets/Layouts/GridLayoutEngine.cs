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

		#region ILayoutEngine Interface
		
		public void UpdateLayout(Visual container, Drawing.Rectangle rect, IEnumerable<Visual> children)
		{
			double[] x = new double[this.columnMeasures.Length];
			double[] y = new double[this.rowMeasures.Length];
			
			double dx = 0;
			double dy = 0;

			for (int i = 0; i < this.columnMeasures.Length; i++)
			{
				x[i] = dx;
				dx += this.columnMeasures[i].Desired;
			}

			for (int i = 0; i < this.rowMeasures.Length; i++)
			{
				dy += this.rowMeasures[i].Desired;
				y[i] = dy;
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

				System.Diagnostics.Debug.Assert (column < this.columnMeasures.Length);
				System.Diagnostics.Debug.Assert (row < this.rowMeasures.Length);

				LayoutMeasure columnMeasure = this.columnMeasures[column];
				LayoutMeasure rowMeasure    = this.rowMeasures[row];

				System.Diagnostics.Debug.Assert (columnMeasure != null);
				System.Diagnostics.Debug.Assert (rowMeasure != null);

				Drawing.Rectangle bounds = new Drawing.Rectangle (rect.Left+x[column], rect.Top-y[row], this.columnMeasures[column].Desired, this.rowMeasures[row].Desired);
				child.SetBounds (bounds);
			}
		}

		public void UpdateMinMax(Visual container, LayoutContext context, IEnumerable<Visual> children, ref Drawing.Size minSize, ref Drawing.Size maxSize)
		{
			List<LayoutMeasure> columnMeasureList = new List<LayoutMeasure> ();
			List<LayoutMeasure> rowMeasureList = new List<LayoutMeasure> ();
			
			int passId = context.PassId;
			int rowMax = 0;
			int columnMax = 0;
			
			foreach (Visual child in children)
			{
				int column = GridLayoutEngine.GetColumn (child);
				int row    = GridLayoutEngine.GetRow (child);

				if ((column < 0) ||
					(row < 0))
				{
					continue;
				}

				if (column > columnMax)
				{
					columnMax = column;
				}
				if (row > rowMax)
				{
					rowMax = row;
				}

				LayoutMeasure columnMeasure = this.GetColumnMeasure (columnMeasureList, passId, column);
				LayoutMeasure rowMeasure    = this.GetRowMeasure (rowMeasureList, passId, row);

				Layouts.LayoutMeasure measureDx = Layouts.LayoutMeasure.GetWidth (child);
				Layouts.LayoutMeasure measureDy = Layouts.LayoutMeasure.GetHeight (child);

				columnMeasure.UpdateMin (passId, measureDx.Desired);
				columnMeasure.UpdateMax (passId, measureDx.Max);
				columnMeasure.UpdatePassId (passId);

				rowMeasure.UpdateMin (passId, measureDy.Desired);
				rowMeasure.UpdateMax (passId, measureDy.Max);
				rowMeasure.UpdatePassId (passId);
			}

			this.columnMeasures = new LayoutMeasure[columnMax+1];
			this.rowMeasures    = new LayoutMeasure[rowMax+1];
			
			columnMeasureList.CopyTo (0, this.columnMeasures, 0, columnMax+1);
			rowMeasureList.CopyTo (0, this.rowMeasures, 0, rowMax+1);

			double minDx = 0;
			double maxDx = 0;

			for (int i = 0; i < this.columnMeasures.Length; i++)
			{
				LayoutMeasure measure = this.columnMeasures[i];

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
				LayoutMeasure measure = this.rowMeasures[i];

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

		private LayoutMeasure GetColumnMeasure(List<LayoutMeasure> list, int passId, int column)
		{
			while (column >= list.Count)
			{
				LayoutMeasure measure;
				
				if (list.Count >= this.columnMeasures.Length)
				{
					measure = new LayoutMeasure (passId);
				}
				else
				{
					measure = this.columnMeasures[list.Count];
					
					if (measure == null)
					{
						measure = new LayoutMeasure (passId);
					}
				}

				if (double.IsNaN (measure.Desired))
				{
					measure.UpdateDesired (passId, 0);
				}
				
				list.Add (measure);
			}

			return list[column];
		}

		private LayoutMeasure GetRowMeasure(List<LayoutMeasure> list, int passId, int row)
		{
			while (row >= list.Count)
			{
				LayoutMeasure measure;

				if (list.Count >= this.rowMeasures.Length)
				{
					measure = new LayoutMeasure (passId);
				}
				else
				{
					measure = this.rowMeasures[list.Count];

					if (measure == null)
					{
						measure = new LayoutMeasure (passId);
					}
				}

				if (double.IsNaN (measure.Desired))
				{
					measure.UpdateDesired (passId, 0);
				}

				list.Add (measure);
			}

			return list[row];
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

		private LayoutMeasure[] rowMeasures    = new LayoutMeasure[0];
		private LayoutMeasure[] columnMeasures = new LayoutMeasure[0];
	}
}
