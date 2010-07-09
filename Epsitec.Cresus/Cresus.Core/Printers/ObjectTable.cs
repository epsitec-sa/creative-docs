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
			this.pagesInfo = new List<PageInfo> ();
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


		public override double RequiredHeight(double width)
		{
			this.width = width;

			double height = 0;

			for (int row = 0; row < this.rowsCount; row++)
			{
				height += this.GetRowHeight (row);
			}

			return height;
		}


		public override void InitializePages(double width, double initialHeight, double middleheight, double finalHeight)
		{
			this.width = width;
			this.pagesInfo.Clear ();

			//	Essaie de tout mettre sur la première page.
			this.ComputeRow (0, initialHeight, middleheight, finalHeight);


		}

		private bool ComputeRow(int startRow, double initialHeight, double middleheight, double finalHeight)
		{
			//	Essaie de mettre un maximum de lignes sur une page donnée.
			//	Retourne true s'il y a assez de place pour tout mettre (donc jusquà la fin).
			int pageCount = 0;

			for (int column = 0; column < this.columnsCount; column++)
			{
				ObjectTextBox textBox = this.GetTextBox (column, startRow);
				double width = this.GetAbsoluteColumnWidth (column);

				textBox.InitializePages (width, initialHeight, middleheight, finalHeight);

				pageCount = System.Math.Max (pageCount, textBox.PageCount);
			}

			return true;
		}

		public override int PageCount
		{
			get
			{
				return 1;
			}
		}

		public override void Paint(IPaintPort port, int page, Point topLeft)
		{
			if (page < 0 || page >= this.pagesInfo.Count)
			{
				return;
			}

			var pageInfo = this.pagesInfo[page];
			double y = topLeft.Y;

			for (int row = pageInfo.FirstRow; row < pageInfo.FirstRow+pageInfo.RowCount; row++)
			{
				double x = topLeft.X;
				double height = 0;

				for (int column = 0; column < this.columnsCount; column++)
				{
					var cellInfo = pageInfo.CellsInfo[row-pageInfo.FirstRow][column];
					ObjectTextBox textBox = this.GetTextBox (column, row);
					double width = this.GetAbsoluteColumnWidth (column);

					textBox.Paint (port, cellInfo.FirstLine, new Point (x+this.CellMargins.Left, y-this.CellMargins.Top));

					port.LineWidth = this.CellBorderWidth;
					port.Color = Color.FromBrightness (0);
					port.PaintOutline (Path.FromRectangle (new Rectangle (x, y-cellInfo.Height, width, cellInfo.Height)));

					x += width;
					height = System.Math.Max (height, cellInfo.Height);
				}

				y -= height;
			}
		}


		private double GetRowHeight(int row)
		{
			//	Calcule la hauteur nécessaire pour la ligne, qui est celle de la plus haute cellule.
			double height = 0;
			for (int column = 0; column < this.columnsCount; column++)
			{
				ObjectTextBox textBox = this.GetTextBox (column, row);

				double width = this.GetAbsoluteColumnWidth (column);
				width -= this.CellMargins.Left;
				width -= this.CellMargins.Right;

				height = System.Math.Max (height, textBox.RequiredHeight (width));
			}

			height += this.CellMargins.Top;
			height += this.CellMargins.Bottom;

			return height;
		}

		private double GetAbsoluteColumnWidth(int column)
		{
			return this.GetRelativeColumnWidth (column) * this.width / this.TotalRelativeColumsWidth;
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
			for (int row = 0; row < this.rowsCount; row++)
			{
				List<ObjectTextBox> line = new List<ObjectTextBox> ();

				for (int column = 0; column < this.columnsCount; column++)
				{
					var textBox = new ObjectTextBox ();
					line.Add (textBox);
				}

				this.content.Add (line);
			}

			this.relativeColumnsWidth.Clear ();
			for (int column = 0; column < this.columnsCount; column++)
			{
				this.relativeColumnsWidth.Add (1.0);
			}
		}


		private class PageInfo
		{
			public PageInfo(int firstRow, int rowCount, double height)
			{
				this.FirstRow  = firstRow;
				this.RowCount  = rowCount;
				this.Height    = height;
				this.CellsInfo = new List<List<CellInfo>> ();
			}

			public int					FirstRow;
			public int					RowCount;
			public double				Height;
			public List<List<CellInfo>>	CellsInfo;
		}

		private class CellInfo
		{
			public CellInfo(int firstLine, int lineCount, double height)
			{
				this.FirstLine = firstLine;
				this.LineCount = lineCount;
				this.Height    = height;
			}

			public int		FirstLine;
			public int		LineCount;
			public double	Height;
		}


		private double						width;
		private int							columnsCount;
		private int							rowsCount;
		private List<List<ObjectTextBox>>	content;
		private List<double>				relativeColumnsWidth;
		private List<PageInfo>				pagesInfo;
	}
}
