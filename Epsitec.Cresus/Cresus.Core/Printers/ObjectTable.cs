//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Debug;
using Epsitec.Common.Dialogs;
using Epsitec.Common.Drawing;
using Epsitec.Common.IO;
using Epsitec.Common.Printing;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Epsitec.Cresus.Core.Printers
{
	public class ObjectTable : AbstractObject
	{
		public ObjectTable() : base()
		{
			this.CellBorderWidth = 0.1;
			this.CellMargins = new Margins (1.0);

			this.content = new List<List<ObjectTextBox>> ();
			this.relativeColumnsWidth = new List<double> ();
		}


		public double CellBorderWidth
		{
			get;
			set;
		}

		public Margins CellMargins
		{
			get;
			set;
		}


		public int ColumnsCount
		{
			get
			{
				return this.columnsCount;
			}
			set
			{
				if (this.columnsCount != value)
				{
					this.columnsCount = value;
					this.UpdateContent ();
				}
			}
		}

		public int RowsCount
		{
			get
			{
				return this.rowsCount;
			}
			set
			{
				if (this.rowsCount != value)
				{
					this.rowsCount = value;
					this.UpdateContent ();
				}
			}
		}


		public double GetRelativeColumnWidth(int column)
		{
			if (column >= 0 && column < this.columnsCount)
			{
				return this.relativeColumnsWidth[column];
			}

			return 0.0;
		}

		public void SetRelativeColumWidth(int column, double value)
		{
			if (column >= 0 && column < this.columnsCount)
			{
				this.relativeColumnsWidth[column] = value;
			}
		}


		public string GetText(int column, int row)
		{
			ObjectTextBox textBox = this.GetTextBox (column, row);
			return textBox.Text;
		}

		public void SetText(int column, int row, string value)
		{
			ObjectTextBox textBox = this.GetTextBox (column, row);
			textBox.Text = value;
		}


		public ObjectTextBox GetTextBox(int column, int row)
		{
			if (column >= 0 && column < this.columnsCount && row >= 0 && row < this.rowsCount)
			{
				return this.content[row][column];
			}

			return null;
		}

		public int CurrentRow
		{
			get;
			set;
		}

		public int FirstLine
		{
			get;
			set;
		}


		public override void Paint(IPaintPort port)
		{
			double y = this.Bounds.Top;

			for (int row = 0; row < this.rowsCount; row++)
			{
				double height = 0;
				for (int column = 0; column < this.columnsCount; column++)
				{
					ObjectTextBox textBox = this.GetTextBox (column, row);
					double width = this.GetAbsoluteColumnWidth (column);
					textBox.Bounds = new Rectangle (0, 0, width, 0);

					height = System.Math.Max (height, textBox.RequiredHeight);
				}

				y -= height;

				double x = this.Bounds.Left;

				for (int column = 0; column < this.columnsCount; column++)
				{
					ObjectTextBox textBox = this.GetTextBox (column, row);
					double width = this.GetAbsoluteColumnWidth (column);
					Rectangle bounds = new Rectangle (x, y, width, height);

					Rectangle textBounds = bounds;
					textBounds.Deflate (this.CellMargins);
					textBox.Bounds = textBounds;
					textBox.Paint (port);

					port.LineWidth = this.CellBorderWidth;
					port.PaintOutline (Path.FromRectangle (bounds));

					x += width;
				}
			}
		}

		private double GetAbsoluteColumnWidth(int column)
		{
			return this.GetRelativeColumnWidth (column) * this.Bounds.Width / this.TotalRelativeColumsWidth;
		}

		private double TotalRelativeColumsWidth
		{
			get
			{
				double width = 0.0;

				for (int column = 0; column < this.columnsCount; column++)
				{
					width += this.relativeColumnsWidth[column];
				}

				return width;
			}
		}


		private void UpdateContent()
		{
			this.content.Clear ();
			this.relativeColumnsWidth.Clear ();

			for (int row = 0; row < this.rowsCount; row++)
			{
				List<ObjectTextBox> line = new List<ObjectTextBox> ();

				for (int column = 0; column < this.columnsCount; column++)
				{
					var textBox = new ObjectTextBox ();
					line.Add (textBox);
				}

				this.content.Add (line);

				this.relativeColumnsWidth.Add (1.0);
			}
		}


		private int columnsCount;
		private int rowsCount;
		private List<List<ObjectTextBox>> content;
		private List<double> relativeColumnsWidth;
	}
}
