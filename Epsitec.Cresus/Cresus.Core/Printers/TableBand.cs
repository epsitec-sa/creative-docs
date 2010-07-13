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
	public class TableBand : AbstractBand
	{
		public TableBand() : base()
		{
			this.CellBorderWidth = 0.1;
			this.CellMargins = new Margins (1.0);

			this.content = new List<List<TextBand>> ();
			this.relativeColumnsWidth = new List<double> ();
			this.sectionsInfo = new List<SectionInfo> ();
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

		public bool DebugPaintFrame
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
			TextBand textBox = this.GetTextBox (column, row);
			return textBox.Text;
		}

		public void SetText(int column, int row, string value)
		{
			TextBand textBox = this.GetTextBox (column, row);
			textBox.Text = value;
		}


		public TextBand GetTextBox(int column, int row)
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


		/// <summary>
		/// Effectue la justification verticale pour découper le tableau en sections.
		/// </summary>
		/// <param name="width">Largeur pour toutes les sections</param>
		/// <param name="initialHeight">Hauteur de la première section</param>
		/// <param name="middleheight">Hauteur des sections suivantes</param>
		/// <param name="finalHeight">Hauteur de la dernière section</param>
		/// <returns>Retourne false s'il n'a pas été possible de mettre tout le contenu</returns>
		public override bool BuildSections(double width, double initialHeight, double middleheight, double finalHeight)
		{
			//	initialHeight et finalHeight doivent être plus petit ou égal à middleheight.
			System.Diagnostics.Debug.Assert (initialHeight <= middleheight);
			System.Diagnostics.Debug.Assert (finalHeight   <= middleheight);

			this.width = width;
			this.sectionsInfo.Clear ();

			for (int i = 0; i < this.rowsCount; i++)
			{
				this.JustifInitialize (i);
			}

			int section = 0;
			int row = 0;
			bool first = true;
			bool ending;
			do
			{
				double heightAvailable = first ? initialHeight : middleheight;
				int sectionCount = this.sectionsInfo.Count;
				ending = this.JustifOneSection (section++, ref row, heightAvailable);

				if (sectionCount == this.sectionsInfo.Count && !first)
				{
					return false;
				}

				first = false;
			}
			while (!ending);

			return true;
		}

		private void JustifInitialize(int row)
		{
			double horizontalMargin = this.CellMargins.Left + this.CellMargins.Right;

			for (int column = 0; column < this.columnsCount; column++)
			{
				TextBand textBox = this.GetTextBox (column, row);
				double width = this.GetAbsoluteColumnWidth (column);

				textBox.JustifInitialize (width-horizontalMargin);
			}
		}

		private bool JustifOneSection(int section, ref int row, double height)
		{
			//	Essaie de mettre un maximum de cellules sur une section donnée.
			//	Retourne true s'il y a assez de place pour tout mettre (donc jusqu'à la fin).
			var newSection = new SectionInfo (section, row);

			//	Cherche la liste de CellInfo précédente.
			List<CellInfo> lastCellsInfo = null;

			var lastSectionIndex = section-1;

			if (lastSectionIndex >= 0 && lastSectionIndex < this.sectionsInfo.Count)
			{
				var lastSectionInfo = this.sectionsInfo[lastSectionIndex];

				var lastRowIndex = lastSectionInfo.RowsInfo.Count-1;

				if (lastRowIndex >= 0 && lastRowIndex < lastSectionInfo.RowsInfo.Count)
				{
					var lastRowsInfo = lastSectionInfo.RowsInfo[lastRowIndex];
					lastCellsInfo = lastRowsInfo.CellsInfo;
				}

				bool allEnding = true;
				for (int column = 0; column < this.columnsCount; column++)
				{
					if (!lastCellsInfo[column].Ending)
					{
						allEnding = false;
					}
				}

				if (allEnding)
				{
					lastCellsInfo = null;
				}
			}

			double verticalMargin = this.CellMargins.Bottom + this.CellMargins.Top;

			int	rowCount = 0;
			double sectionHeight = 0;
			bool ending = false;

			while (height-verticalMargin > 0)
			{
				var rowInfo = new RowInfo (row);
				double maxRowHeight = 0;
				bool rowEnding = true;
				bool tooSmall = false;

				for (int column = 0; column < this.columnsCount; column++)
				{
					TextBand textBox = this.GetTextBox (column, row);

					int textSection = 0;
					int line = 0;
					bool lastEnding = false;
					if (lastCellsInfo != null)
					{
						textSection = lastCellsInfo[column].TextSection + 1;
						line = lastCellsInfo[column].FirstLine + lastCellsInfo[column].LineCount;
						lastEnding = lastCellsInfo[column].Ending;
					}

					if (lastEnding)
					{
						CellInfo newCell = new CellInfo (textSection, -1, 0, true, 0);
						rowInfo.CellsInfo.Add (newCell);
					}
					else
					{
						bool cellEnding = textBox.JustifOneSection (ref line, height-verticalMargin);

						double cellHeight = textBox.LastHeight + verticalMargin;

						CellInfo newCell = new CellInfo (textSection, textBox.LastFirstLine, textBox.LastLineCount, cellEnding, cellHeight);
						rowInfo.CellsInfo.Add (newCell);

						maxRowHeight = System.Math.Max (maxRowHeight, cellHeight);

						if (cellEnding == false)
						{
							rowEnding = false;
						}

						if (textBox.LastLineCount == 0 && !string.IsNullOrEmpty(textBox.Text))
						{
							//	Si une seule colonne non vide n'arrive pas à caser au moins une ligne,
							//	il faut rejeter cette 'row' et essayer sur une nouvelle section.
							tooSmall = true;
						}
					}
				}

				if (tooSmall)
				{
					for (int column = 0; column < this.columnsCount; column++)
					{
						TextBand textBox = this.GetTextBox (column, row);
						textBox.JustifRemoveLastSection ();
					}

					break;
				}

				rowInfo.Height = maxRowHeight;
				newSection.RowsInfo.Add (rowInfo);

				rowCount++;
				sectionHeight += maxRowHeight;
				height -= maxRowHeight;

				if (rowEnding == false)
				{
					break;
				}

				lastCellsInfo = null;
				row++;

				if (row >= this.rowsCount)
				{
					ending = true;
					break;
				}
			}

			if (rowCount > 0)
			{
				newSection.RowCount = rowCount;
				newSection.Height = sectionHeight;

				this.sectionsInfo.Add (newSection);
			}

			return ending;
		}


		public override int SectionCount
		{
			get
			{
				return this.sectionsInfo.Count;
			}
		}

		/// <summary>
		/// Retourne la hauteur que l'objet occupe dans une section.
		/// </summary>
		/// <param name="section"></param>
		/// <returns></returns>
		public override double GetSectionHeight(int section)
		{
			if (section >= 0 && section < this.sectionsInfo.Count)
			{
				return this.sectionsInfo[section].Height;
			}

			return 0;
		}

		/// <summary>
		/// Dessine une section de l'objet à une position donnée.
		/// </summary>
		/// <param name="port">Port graphique</param>
		/// <param name="section">Rang de la section à dessiner</param>
		/// <param name="topLeft">Coin supérieur gauche</param>
		/// <returns>Retourne false si le contenu est trop grand et n'a pas pu être dessiné</returns>
		public override bool Paint(IPaintPort port, int section, Point topLeft)
		{
			if (section < 0 || section >= this.sectionsInfo.Count)
			{
				return true;
			}

			var sectionInfo = this.sectionsInfo[section];
			double y = topLeft.Y;
			var ok = true;

			for (int row = sectionInfo.FirstRow; row < sectionInfo.FirstRow+sectionInfo.RowCount; row++)
			{
				var rowInfo = sectionInfo.RowsInfo[row-sectionInfo.FirstRow];

				double x = topLeft.X;

				for (int column = 0; column < this.columnsCount; column++)
				{
					var cellInfo = rowInfo.CellsInfo[column];
					double width = this.GetAbsoluteColumnWidth (column);

					if (cellInfo.FirstLine != -1)
					{
						TextBand textBox = this.GetTextBox (column, row);

						if (!textBox.Paint (port, cellInfo.TextSection, new Point (x+this.CellMargins.Left, y-this.CellMargins.Top)))
						{
							ok = false;
						}
					}

					//	Dessine le cadre de la cellule.
					port.LineWidth = this.CellBorderWidth;
					port.Color = Color.FromBrightness (0);
					port.PaintOutline (Path.FromRectangle (new Rectangle (x, y-rowInfo.Height, width, rowInfo.Height)));

					x += width;
				}

				y -= rowInfo.Height;
			}

			if (this.DebugPaintFrame)
			{
				Rectangle rect = new Rectangle (topLeft.X, topLeft.Y-sectionInfo.Height, this.width, sectionInfo.Height);

				port.LineWidth = 0.1;
				port.Color = Color.FromName (ok ? "Green" : "Red");
				port.PaintOutline (Path.FromRectangle (rect));
			}

			return ok;
		}


		private double GetRowHeight(int row)
		{
			//	Calcule la hauteur nécessaire pour la ligne, qui est celle de la plus haute cellule.
			double height = 0;
			for (int column = 0; column < this.columnsCount; column++)
			{
				TextBand textBox = this.GetTextBox (column, row);

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
				List<TextBand> line = new List<TextBand> ();

				for (int column = 0; column < this.columnsCount; column++)
				{
					var textBox = new TextBand ();
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


		private class SectionInfo
		{
			public SectionInfo(int section, int firstRow)
			{
				this.Section  = section;
				this.FirstRow = firstRow;
				this.RowsInfo = new List<RowInfo> ();
			}

			public int				Section;
			public int				FirstRow;
			public int				RowCount;
			public double			Height;
			public List<RowInfo>	RowsInfo;
		}

		private class RowInfo
		{
			public RowInfo(int row)
			{
				this.Row = row;
				this.CellsInfo = new List<CellInfo> ();
			}

			public int				Row;
			public double			Height;
			public List<CellInfo>	CellsInfo;
		}

		private class CellInfo
		{
			public CellInfo(int textSection, int firstLine, int lineCount, bool ending, double height)
			{
				this.TextSection = textSection;
				this.FirstLine   = firstLine;
				this.LineCount   = lineCount;
				this.Ending      = ending;
				this.Height      = height;
			}

			public int		TextSection;
			public int		FirstLine;
			public int		LineCount;
			public bool		Ending;
			public double	Height;
		}


		private double						width;
		private int							columnsCount;
		private int							rowsCount;
		private List<List<TextBand>>		content;
		private List<double>				relativeColumnsWidth;
		private List<SectionInfo>				sectionsInfo;
	}
}
