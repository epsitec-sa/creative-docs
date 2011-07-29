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

namespace Epsitec.Cresus.Core.Print.Bands
{
	public class TableBand : AbstractBand
	{
		public TableBand() : base()
		{
			this.CellBorder = CellBorder.Default;
			this.CellMargins = new Margins (1.0);

			this.content = new List<List<TextBand>> ();
			this.relativeColumnsWidth = new List<double> ();
			this.sectionsInfo = new List<SectionInfo> ();
		}


		public CellBorder CellBorder
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
					this.unbreakableRows = new bool[this.columnsCount];
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


		public bool GetUnbreakableRow(int row)
		{
			if (this.unbreakableRows != null && row >= 0 && row < this.unbreakableRows.Length)
			{
				return this.unbreakableRows[row];
			}

			return false;
		}

		public void SetUnbreakableRow(int row, bool value)
		{
			if (this.unbreakableRows != null && row >= 0 && row < this.unbreakableRows.Length)
			{
				this.unbreakableRows[row] = value;
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


		public FormattedText GetText(int column, int row)
		{
			TextBand textBand = this.GetTextBand (column, row);

			if (textBand == null)
			{
				return FormattedText.Null;
			}
			else
			{
				return textBand.Text;
			}
		}

		public void SetText(int column, int row, string value)
		{
			this.SetText (column, row, FormattedText.FromSimpleText (value));
		}

		public void SetText(int column, int row, FormattedText value)
		{
			TextBand textBand = this.GetTextBand (column, row);

			if (textBand != null)
			{
				textBand.Text = value;
			}
		}


		public void SetText(int column, int row, string value, double fontSize)
		{
			this.SetText (column, row, FormattedText.FromSimpleText (value), fontSize);
		}

		public void SetText(int column, int row, FormattedText value, double fontSize)
		{
			TextBand textBand = this.GetTextBand (column, row);

			if (textBand != null)
			{
				textBand.Text = value;
				textBand.FontSize = fontSize;
			}
		}


		public Font GetFont(int column, int row)
		{
			TextBand textBand = this.GetTextBand (column, row);

			if (textBand == null)
			{
				return null;
			}
			else
			{
				return textBand.Font;
			}
		}

		public void SetFont(int column, int row, Font value)
		{
			TextBand textBand = this.GetTextBand (column, row);

			if (textBand != null)
			{
				textBand.Font = value;
			}
		}


		public double GetFontSize(int column, int row)
		{
			TextBand textBand = this.GetTextBand (column, row);

			if (textBand == null)
			{
				return 0;
			}
			else
			{
				return textBand.FontSize;
			}
		}

		public void SetFontSize(int column, int row, double value)
		{
			TextBand textBand = this.GetTextBand (column, row);

			if (textBand != null)
			{
				textBand.FontSize = value;
			}
		}


		public ContentAlignment GetAlignment(int column, int row)
		{
			TextBand textBand = this.GetTextBand (column, row);

			if (textBand == null)
			{
				return ContentAlignment.None;
			}
			else
			{
				return textBand.Alignment;
			}
		}

		public void SetAlignment(int column, int row, ContentAlignment value)
		{
			TextBand textBand = this.GetTextBand (column, row);

			if (textBand != null)
			{
				textBand.Alignment = value;
			}
		}


		public TextJustifMode GetJustif(int column, int row)
		{
			TextBand textBand = this.GetTextBand (column, row);

			if (textBand == null)
			{
				return TextJustifMode.None;
			}
			else
			{
				return textBand.Justif;
			}
		}

		public void SetJustif(int column, int row, TextJustifMode value)
		{
			TextBand textBand = this.GetTextBand (column, row);

			if (textBand != null)
			{
				textBand.Justif = value;
			}
		}


		public Margins GetCellMargins(int column, int row)
		{
			TextBand textBand = this.GetTextBand (column, row);

			if (textBand == null)
			{
				return Margins.Zero;
			}
			else
			{
				return textBand.TableCellMargins;
			}
		}

		public void SetCellMargins(int column, int row, Margins value)
		{
			TextBand textBand = this.GetTextBand (column, row);

			if (textBand != null)
			{
				textBand.TableCellMargins = value;
			}
		}

		public void SetCellMargins(int row, Margins value)
		{
			//	Spécifie les marges pour toute une ligne.
			for (int column = 0; column < this.columnsCount; column++)
			{
				this.SetCellMargins (column, row, value);
			}
		}


		public CellBorder GetCellBorder(int column, int row)
		{
			TextBand textBand = this.GetTextBand (column, row);

			if (textBand == null)
			{
				return CellBorder.Empty;
			}
			else
			{
				return textBand.TableCellBorder;
			}
		}

		public void SetCellBorder(int column, int row, CellBorder value)
		{
			TextBand textBand = this.GetTextBand (column, row);

			if (textBand != null)
			{
				textBand.TableCellBorder = value;
			}
		}

		public void SetCellBorder(int row, CellBorder value)
		{
			//	Spécifie les bordures pour toute une ligne.
			for (int column = 0; column < this.columnsCount; column++)
			{
				this.SetCellBorder (column, row, value);
			}
		}


		public Color GetBackground(int column, int row)
		{
			TextBand textBand = this.GetTextBand (column, row);

			if (textBand == null)
			{
				return Color.Empty;
			}
			else
			{
				return textBand.TableCellBackground;
			}
		}

		public void SetBackground(int column, int row, Color value)
		{
			TextBand textBand = this.GetTextBand (column, row);

			if (textBand != null)
			{
				textBand.TableCellBackground = value;
			}
		}


		public int GetColumnSpan(int column, int row)
		{
			TextBand textBand = this.GetTextBand (column, row);

			if (textBand == null)
			{
				return 1;
			}
			else
			{
				return textBand.TableCellColumnSpan;
			}
		}

		public void SetColumnSpan(int column, int row, int span)
		{
			TextBand textBand = this.GetTextBand (column, row);

			if (textBand != null)
			{
				textBand.TableCellColumnSpan = span;
			}
		}


		public TextBreakMode GetBreakMode(int column, int row)
		{
			TextBand textBand = this.GetTextBand (column, row);

			if (textBand == null)
			{
				return TextBreakMode.None;
			}
			else
			{
				return textBand.BreakMode;
			}
		}

		public void SetBreakMode(int column, int row, TextBreakMode value)
		{
			TextBand textBand = this.GetTextBand (column, row);

			if (textBand != null)
			{
				textBand.BreakMode = value;
			}
		}


		public TextBand GetTextBand(int column, int row)
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
				height += this.ComputeRowHeight (row);
			}

			return height;
		}

		public double RequiredColumnWidth(int column)
		{
			//	Retourne la largeur requise pour une colonne donnée si les textes sont mis sur une seule ligne.
			double width = 0;

			for (int row = 0; row < this.rowsCount; row++)
			{
				TextBand textBand = this.GetTextBand (column, row);

				width = System.Math.Max (width, textBand.RequiredWidth ());
			}

			return width;
		}


		/// <summary>
		/// Effectue la justification verticale pour découper le tableau en sections.
		/// </summary>
		/// <param name="width">Largeur pour toutes les sections</param>
		/// <param name="initialHeight">Hauteur de la première section</param>
		/// <param name="middleheight">Hauteur des sections suivantes</param>
		/// <param name="finalHeight">Hauteur de la dernière section</param>
		public override void BuildSections(double width, double initialHeight, double middleheight, double finalHeight)
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
					return;
				}

				first = false;
			}
			while (!ending);
		}

		private void JustifInitialize(int row)
		{
			for (int column = 0; column < this.columnsCount; column++)
			{
				TextBand textBand = this.GetTextBand (column, row);
				double width = this.GetAbsoluteSpanedColumnWidth (ref column, row);
				var margins = this.GetMargins (textBand);

				textBand.JustifInitialize (width - margins.Left - margins.Right);
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

			int	rowCount = 0;
			double sectionHeight = 0;
			bool ending = false;

			double maxVerticalMargin = 0;
			for (int column = 0; column < this.columnsCount; column++)
			{
				TextBand textBand = this.GetTextBand (column, row);
				var margins = this.GetMargins (textBand);

				maxVerticalMargin = System.Math.Max (maxVerticalMargin, margins.Bottom + margins.Top);
			}

			while (height-maxVerticalMargin > 0)
			{
				var rowInfo = new RowInfo (row);
				double maxRowHeight = 0;
				bool rowEnding = true;
				bool tooSmall = false;

				TextBand firstTextBand = this.GetTextBand (0, row);
				var topGap = (firstTextBand == null) ? 0 : firstTextBand.TableCellBorder.TopGap;

				for (int column = 0; column < this.columnsCount; column++)
				{
					TextBand textBand = this.GetTextBand (column, row);
					var margins = this.GetMargins (textBand);

					int  textSection = 0;
					int  line        = 0;
					bool lastEnding  = false;

					if (lastCellsInfo != null)
					{
						textSection = lastCellsInfo[column].TextSection + 1;
						line        = lastCellsInfo[column].FirstLine + lastCellsInfo[column].LineCount;
						lastEnding  = lastCellsInfo[column].Ending;
					}

					if (lastEnding)
					{
						CellInfo newCell = new CellInfo (textSection, -1, 0, true, 0);
						rowInfo.CellsInfo.Add (newCell);
					}
					else
					{
						double verticalMargin = margins.Bottom + margins.Top;

						bool cellEnding = textBand.JustifOneSection (ref line, height-topGap-verticalMargin);

						double cellHeight = textBand.LastHeight + verticalMargin;

						CellInfo newCell = new CellInfo (textSection, textBand.LastFirstLine, textBand.LastLineCount, cellEnding, cellHeight);
						rowInfo.CellsInfo.Add (newCell);

						maxRowHeight = System.Math.Max (maxRowHeight, cellHeight);

						if (cellEnding == false)
						{
							rowEnding = false;
						}

						if (textBand.LastLineCount == 0 && !textBand.Text.IsNullOrEmpty)
						{
							//	Si une seule colonne non vide n'arrive pas à caser au moins une ligne,
							//	il faut rejeter cette 'row' et essayer dans une nouvelle section.
							tooSmall = true;
						}
					}
				}

				if (tooSmall || (!rowEnding && this.GetUnbreakableRow (row)))
				{
					for (int column = 0; column < this.columnsCount; column++)
					{
						TextBand textBand = this.GetTextBand (column, row);
						textBand.JustifRemoveLastSection ();
					}

					break;
				}

				rowInfo.TopGap = topGap;
				rowInfo.Height = maxRowHeight;
				newSection.RowsInfo.Add (rowInfo);

				maxRowHeight += topGap;
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
		/// <param name="section">index de la section</param>
		/// <returns>hauteur</returns>
		public override double GetSectionHeight(int section)
		{
			if (section >= 0 && section < this.sectionsInfo.Count)
			{
				return this.sectionsInfo[section].Height;
			}

			return 0;
		}

		public double GetRowHeight(int row)
		{
			foreach (var section in this.sectionsInfo)
			{
				foreach (var info in section.RowsInfo)
				{
					if (info.Row == row)
					{
						return info.TopGap + info.Height;
					}
				}
			}

			return 0;
		}

		/// <summary>
		/// Retourne les index des dernières lignes de chaque section. Comme un montant est toujours
		/// imprimé en haut d'une cellule, cela permet de calculer les reports.
		/// </summary>
		/// <returns>Liste des numéros de lignes</returns>
		public int[] GetLastRowForEachSection()
		{
			int[] rows = new int[this.sectionsInfo.Count+1];
			int index = 0;

			foreach (var section in this.sectionsInfo)
			{
				rows[index++] = section.FirstRow + section.RowCount - 1;
			}

			return rows;
		}


		/// <summary>
		/// Dessine une section de l'objet à une position donnée.
		/// </summary>
		/// <param name="port">Port graphique</param>
		/// <param name="section">Rang de la section à dessiner</param>
		/// <param name="topLeft">Coin supérieur gauche</param>
		/// <returns>Retourne false si le contenu est trop grand et n'a pas pu être dessiné</returns>
		public override bool PaintForeground(IPaintPort port, PreviewMode previewMode, int section, Point topLeft)
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
				y -= rowInfo.TopGap;

				double x = topLeft.X;

				for (int column = 0; column < this.columnsCount; column++)
				{
					int c = column;
					var cellInfo = rowInfo.CellsInfo[column];
					double width = this.GetAbsoluteSpanedColumnWidth (ref column, row);
					Rectangle cellBounds = new Rectangle (x, y-rowInfo.Height, width, rowInfo.Height);

					CellBorder cellBorder = CellBorder.Empty;

					if (cellInfo.FirstLine != -1)
					{
						TextBand textBand = this.GetTextBand (c, row);

						if (textBand.TableCellBackground.IsValid)
						{
							port.Color = textBand.TableCellBackground;
							port.PaintSurface (Path.FromRectangle (cellBounds));
						}

						var margins = this.GetMargins (textBand);
						if (!textBand.PaintForeground (port, previewMode, cellInfo.TextSection, new Point (x+margins.Left, y-margins.Top)))
						{
							ok = false;
						}

						cellBorder = textBand.TableCellBorder;
					}

					//	Dessine le cadre de la cellule.
					if (cellBorder.IsEmpty)
					{
						cellBorder = this.CellBorder;
					}

					if (cellBorder.IsValid)
					{
						port.Color = cellBorder.Color;

						if (cellBorder.IsConstantWidth)
						{
							port.LineWidth = cellBorder.LeftWidth;
							port.PaintOutline (Path.FromRectangle (cellBounds));
						}
						else
						{
							if (cellBorder.LeftWidth > 0)
							{
								port.LineWidth = cellBorder.LeftWidth;
								port.PaintOutline (Path.FromLine (cellBounds.BottomLeft, cellBounds.TopLeft));
							}

							if (cellBorder.RightWidth > 0)
							{
								port.LineWidth = cellBorder.RightWidth;
								port.PaintOutline (Path.FromLine (cellBounds.BottomRight, cellBounds.TopRight));
							}

							if (cellBorder.BottomWidth > 0)
							{
								port.LineWidth = cellBorder.BottomWidth;
								port.PaintOutline (Path.FromLine (cellBounds.BottomLeft, cellBounds.BottomRight));
							}

							if (cellBorder.TopWidth > 0)
							{
								port.LineWidth = cellBorder.TopWidth;
								port.PaintOutline (Path.FromLine (cellBounds.TopLeft, cellBounds.TopRight));
							}
						}
					}

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


		private double ComputeRowHeight(int row)
		{
			//	Calcule la hauteur nécessaire pour la ligne, qui est celle de la plus haute cellule.
			double maxHeight = 0;

			for (int column = 0; column < this.columnsCount; column++)
			{
				TextBand textBand = this.GetTextBand (column, row);
				var margins = this.GetMargins (textBand);

				double width = this.GetAbsoluteSpanedColumnWidth (ref column, row);
				width -= margins.Left;
				width -= margins.Right;

				double height = textBand.RequiredHeight (width);
				height += margins.Top;
				height += margins.Bottom;

				maxHeight = System.Math.Max (maxHeight, height);
			}

			TextBand firstTextBand = this.GetTextBand (0, row);
			maxHeight += firstTextBand.TableCellBorder.TopGap;

			return maxHeight;
		}

		private double GetAbsoluteSpanedColumnWidth(ref int column, int row)
		{
			double width = this.GetAbsoluteColumnWidth (column);

			int span = this.GetColumnSpan (column, row);
			if (span > 1)
			{
				for (int i = 1; i < span; i++)
				{
					width += this.GetAbsoluteColumnWidth (column+i);
				}

				column += span-1;
			}

			return width;
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
					var textBand = new TextBand ();
					textBand.Font = this.Font;
					textBand.FontSize = this.FontSize;

					line.Add (textBand);
				}

				this.content.Add (line);
			}

			this.relativeColumnsWidth.Clear ();
			for (int column = 0; column < this.columnsCount; column++)
			{
				this.relativeColumnsWidth.Add (1.0);
			}
		}


		private Margins GetMargins(TextBand textBand)
		{
			//	Retourne les marges à utiliser pour une cellule donnée.
			if (textBand.TableCellMargins.Left   == 0 &&
				textBand.TableCellMargins.Right  == 0 &&
				textBand.TableCellMargins.Bottom == 0 &&
				textBand.TableCellMargins.Top    == 0)
			{
				return this.CellMargins;
			}
			else
			{
				return textBand.TableCellMargins;
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
			public double			TopGap;
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
		private List<SectionInfo>			sectionsInfo;
		private bool[]						unbreakableRows;
	}
}
