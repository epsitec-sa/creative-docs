//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Support;

namespace Epsitec.Common.Widgets
{
	public enum ScrollShowMode
	{
		Extremity,		// déplacement minimal aux extrémités
		Center,			// déplacement central
	}

	public enum ScrollAdjustMode
	{
		MoveTop,		// déplace le haut
		MoveBottom,		// déplace le bas
	}


	public enum ScrollInteractionMode
	{
		ReadOnly,
		Edition,
		Search
	}

	public delegate string TextProviderCallback(int row, int column);

	/// <summary>
	///	La classe ScrollArray réalise une liste déroulante optimisée à deux dimensions,
	///	ne pouvant contenir que des textes fixes.
	/// </summary>
	public class ScrollArray : Widget, Support.Data.IStringSelection
	{
		public ScrollArray()
		{
			this.columns = new ColumnDefinition[0];

			this.AutoFocus = true;
			this.AutoDoubleClick = true;
			this.InternalState |= WidgetInternalState.Focusable;

			IAdorner adorner = Widgets.Adorners.Factory.Active;

			this.frameMargins = adorner.GeometryArrayMargins;
			this.tableMargins = new Drawing.Margins ();
			this.innerMargins = new Drawing.Margins ();
			this.rowHeight    = System.Math.Floor (Widget.DefaultFontHeight * 1.25 + 0.5);

			this.header = new Widget (this);
			this.vScroller = new VScroller (this);
			this.hScroller = new HScroller (this);
			this.vScroller.IsInverted = true;
			this.vScroller.ValueChanged += this.HandleVScrollerChanged;
			this.hScroller.ValueChanged += this.HandleHScrollerChanged;

			this.isDirty = true;
		}

		public ScrollArray(Widget embedder)
			: this ()
		{
			this.SetEmbedder (embedder);
		}


		public TextProviderCallback TextProviderCallback
		{
			//	Spécifie le délégué pour remplir les cellules.
			//	En mode sans FillText, la liste est remplie à l'avance avec SetText.
			//	Une copie de tous les strings est alors contenue dans this.array.
			//	En mode FillText, c'est ScrollArray qui demande le contenu de chaque
			//	cellule au fur et à mesure à l'aide du délégué FillText. Ce mode
			//	est particulièrement efficace pour de grandes quantités de données.

			get
			{
				return this.textProviderCallback;
			}
			set
			{
				if (this.textProviderCallback != value)
				{
					if (value != null)
					{
						this.TextArrayStore = null;
					}

					this.textProviderCallback = value;

					this.Clear ();
				}
			}
		}

		public Support.Data.ITextArrayStore TextArrayStore
		{
			get
			{
				return this.textArrayStore;
			}
			set
			{
				if (this.textArrayStore != value)
				{
					if (value != null)
					{
						this.TextProviderCallback = null;
					}

					if (this.textArrayStore != null)
					{
						this.textArrayStore.StoreContentsChanged -= this.HandleStoreContentsChanged;
					}

					this.textArrayStore = value;

					if (this.textArrayStore != null)
					{
						this.textArrayStore.StoreContentsChanged += this.HandleStoreContentsChanged;
					}

					this.OnTextArrayStoreContentsChanged ();
					this.SyncWithTextArrayStore (false);
				}
			}
		}

		public string this[int row, int column]
		{
			get
			{
				return this.GetCellText (row, column);
			}
			set
			{
				this.SetCellText (row, column, value);
			}
		}

		public char Separator
		{
			get
			{
				return this.separator;
			}
			set
			{
				this.separator = value;
			}
		}

		public int ColumnCount
		{
			get
			{
				return this.maxColumns;
			}
			set
			{
				if (this.maxColumns != value)
				{
					this.maxColumns = value;
					this.UpdateColumnCount ();
				}
			}
		}

		public int RowCount
		{
			get
			{
				return this.maxRows;
			}
			set
			{
				if (this.maxRows != value)
				{
					this.maxRows = value;

					if ((this.textProviderCallback == null) &&
						(this.textArrayStore == null))
					{
						//	Met à jour le nombre de lignes dans la table. Si la table est trop longue, on
						//	va la tronquer; si elle est trop courte, on va l'allonger.

						int n = this.textArray.Count;

						if (this.maxRows > n)
						{
							for (int i = n; i < this.maxRows; i++)
							{
								this.textArray.Add (new System.Collections.ArrayList ());
							}
						}
						else if (this.maxRows < n)
						{
							this.textArray.RemoveRange (this.maxRows, n - this.maxRows);
						}
					}

					this.InvalidateContents ();
				}
			}
		}

		public int VisibleRowCount
		{
			get
			{
				this.Update ();
				return this.nVisibleRows;
			}
		}

		public int FullyVisibleRowCount
		{
			get
			{
				this.Update ();
				return this.nFullyVisibleRows;
			}
		}

		public int VirtualRowCount
		{
			get
			{
				return this.editionRow < 0 ? this.RowCount : this.RowCount + this.editionAddRows;
			}
		}

		public int FirstVisibleRow
		{
			get
			{
				return this.FromVirtualRow (this.firstVirtvisRow);
			}
			set
			{
				this.SetFirstVirtualVisibleIndex (this.ToVirtualRow (value));
			}
		}


		public int EditionZoneHeight
		{
			get
			{
				return this.editionAddRows + 1;
			}
			set
			{
				value--;
				if (value < 0)
					value = 0;

				if (this.editionAddRows != value)
				{
					int top = this.FromVirtualRow (this.firstVirtvisRow);

					this.editionAddRows  = value;
					this.firstVirtvisRow = this.ToVirtualRow (top);

					this.InvalidateContents ();
				}
			}
		}

		public double HorizontalOffset
		{
			//	Offset horizontal.
			get
			{
				return this.offset;
			}
			set
			{
				if (this.offset != value)
				{
					this.offset = value;
					this.InvalidateContents ();
				}
			}
		}

		public bool IsSelectedVisible
		{
			get
			{
				if (this.selectedRow == -1)
				{
					return true;
				}

				int row = this.ToVirtualRow (this.selectedRow);

				if ((row >= this.firstVirtvisRow) &&
					(row < this.firstVirtvisRow + this.nFullyVisibleRows))
				{
					return true;
				}

				return false;
			}
		}

		public ScrollInteractionMode InteractionMode
		{
			get
			{
				return this.interactionMode;
			}
		}


		public double TitleHeight
		{
			get
			{
				return this.titleHeight;
			}
			set
			{
				if (this.titleHeight != value)
				{
					this.titleHeight = value;
					this.InvalidateContents ();
				}
			}
		}

		public Drawing.Rectangle TitleBounds
		{
			get
			{
				this.Update ();

				Drawing.Rectangle bounds = new Drawing.Rectangle ();

				bounds.Left   = this.tableBounds.Left;
				bounds.Bottom = this.header.ActualLocation.Y + this.header.ActualHeight;
				bounds.Height = this.titleHeight;
				bounds.Right  = this.vScroller.ActualLocation.X + this.vScroller.ActualWidth;

				return bounds;
			}
		}

		public Widget TitleWidget
		{
			get
			{
				return this.titleWidget;
			}
			set
			{
				if (this.titleWidget != value)
				{
					if (this.titleWidget != null)
					{
						this.titleWidget.SetParent (null);
					}

					this.titleWidget = value;

					if (this.titleWidget != null)
					{
						this.titleWidget.SetEmbedder (this);
						this.UpdateTitleWidget ();
					}
				}
			}
		}


		public Tag TagWidget
		{
			get
			{
				return this.tagWidget;
			}
			set
			{
				if (this.tagWidget != value)
				{
					if (this.tagWidget != null)
					{
						this.tagWidget.SetParent (null);
					}

					this.tagWidget = value;

					if (this.tagWidget != null)
					{
						if (this.clipWidget == null)
						{
							this.clipWidget = new Widget (this);
						}

						this.tagWidget.SetEmbedder (this.clipWidget);

						this.UpdateTagWidget ();
					}
					else
					{
						if (this.clipWidget != null)
						{
							this.clipWidget.SetParent (null);
							this.clipWidget = null;
						}
					}
				}
			}
		}

		public double InnerTopMargin
		{
			get
			{
				return this.innerMargins.Top;
			}
			set
			{
				if (this.innerMargins.Top != value)
				{
					this.innerMargins.Top = value;
					this.InvalidateContents ();
				}
			}
		}

		public double InnerBottomMargin
		{
			get
			{
				return this.innerMargins.Bottom;
			}
			set
			{
				if (this.innerMargins.Bottom != value)
				{
					this.innerMargins.Bottom = value;
					this.InvalidateContents ();
				}
			}
		}


		public double FreeTableWidth
		{
			get
			{
				return this.innerBounds.Width - this.totalWidth;
			}
		}

		public double TotalTableWidth
		{
			get
			{
				return this.totalWidth;
			}
		}


		public HScroller HScroller
		{
			get
			{
				return this.hScroller;
			}
		}

		public VScroller VScroller
		{
			get
			{
				return this.vScroller;
			}
		}

		public ColumnDefinition[] Columns
		{
			get
			{
				return this.columns;
			}
		}


		public int ToVirtualRow(int row)
		{
			if (this.editionRow < 0)
			{
				return row;
			}
			if (this.editionRow < row)
			{
				//	La ligne se trouve après la zone d'édition; il faut donc la décaler vers
				//	le bas :

				return row + this.editionAddRows;
			}

			return row;
		}

		public int FromVirtualRow(int row)
		{
			if (this.editionRow < 0)
			{
				return row;
			}
			if (this.editionRow + this.editionAddRows < row)
			{
				//	La ligne se trouve après la zone d'édition; il faut donc la décaler vers
				//	le haut :

				return row - this.editionAddRows;
			}
			if (this.editionRow < row)
			{
				//	La ligne se trouve dans la zone d'édition; il faut donc retourner le début
				//	de la zone d'édition :

				return this.editionRow;
			}

			return row;
		}



		protected virtual void OnColumnWidthChanged(ColumnDefinition column)
		{
			this.UpdateTotalWidth ();
			this.UpdateScrollView ();
			this.InvalidateContents ();
		}

		protected virtual void OnColumnAlignmentChanged(ColumnDefinition column)
		{
			this.InvalidateContents ();
		}

		public void SetColumnWidth(int column, double width)
		{
			width = System.Math.Floor (width);
			width = System.Math.Max (width, this.minWidth);

			this.Columns[column].Width = width;
		}

		public double GetColumnWidth(int column)
		{
			return this.Columns[column].Width;
		}


		public double GetColumnOffset(int column)
		{
			return this.Columns[column].Offset;
		}


		public virtual string GetCellText(int row, int column)
		{
			if ((row < 0) || (row >= this.maxRows))
			{
				throw new System.ArgumentOutOfRangeException ("row", row, "Row out of range.");
			}

			if ((column < 0) || (column >= this.maxColumns))
			{
				throw new System.ArgumentOutOfRangeException ("column", column, "Column out of range.");
			}

			if (this.textProviderCallback != null)
			{
				return this.textProviderCallback (row, column);
			}
			if (this.textArrayStore != null)
			{
				return this.textArrayStore.GetCellText (row, column);
			}

			System.Collections.ArrayList line = this.textArray[row] as System.Collections.ArrayList;

			return (column >= line.Count) ? "" : line[column] as string;
		}

		public virtual string[] GetRowTexts(int row)
		{
			if ((row < 0) || (row >= this.maxRows))
			{
				throw new System.ArgumentOutOfRangeException ("row", row, "Row out of range.");
			}

			string[] values = new string[this.maxColumns];

			if (this.textProviderCallback != null)
			{
				for (int i = 0; i < this.maxColumns; i++)
				{
					values[i] = this.textProviderCallback (row, i);
				}
			}
			else if (this.textArrayStore != null)
			{
				for (int i = 0; i < this.maxColumns; i++)
				{
					values[i] = this.textArrayStore.GetCellText (row, i);
				}
			}
			else
			{
				System.Collections.ArrayList line = this.textArray[row] as System.Collections.ArrayList;

				for (int i = 0; i < this.maxColumns; i++)
				{
					if (i < line.Count)
					{
						values[i] = line[i] as string;
					}
					else
					{
						values[i] = "";
					}
				}
			}

			return values;
		}

		public virtual void SetCellText(int row, int column, string value)
		{
			if (this.textProviderCallback != null)
			{
				throw new System.InvalidOperationException (string.Format ("Cannot set cell [{0},{1}] in this ScrollArray.", row, column));
			}

			if (row < 0)
			{
				throw new System.ArgumentOutOfRangeException ("row", row, "Row out of range.");
			}

			if (column < 0)
			{
				throw new System.ArgumentOutOfRangeException ("column", column, "Column out of range.");
			}

			if (this.textArrayStore != null)
			{
				this.textArrayStore.SetCellText (row, column, value);
				return;
			}

			System.Diagnostics.Debug.Assert (this.textArray.Count == this.maxRows);

			bool changed = false;

			changed |= (row >= this.maxRows);
			changed |= (column >= this.maxColumns);

			this.RowCount    = System.Math.Max (this.maxRows, row + 1);
			this.ColumnCount = System.Math.Max (this.maxColumns, column + 1);

			System.Collections.ArrayList line = this.textArray[row] as System.Collections.ArrayList;

			if (column >= line.Count)
			{
				for (int i = line.Count; i <= column; i++)
				{
					line.Add ("");
				}
			}

			string text = line[column] as string;

			if (text != value)
			{
				line[column] = value;
				changed      = true;
			}

			if (changed)
			{
				this.OnContentsChanged ();
				this.InvalidateContents ();
			}
		}

		public virtual void SetRowTexts(int row, string[] values)
		{
			if (this.textProviderCallback != null)
			{
				throw new System.InvalidOperationException (string.Format ("Cannot set row [{0}] in this ScrollArray.", row));
			}

			if ((row < 0) || (row >= this.maxRows))
			{
				throw new System.ArgumentOutOfRangeException ("row", row, "Row out of range.");
			}

			if (values.Length > this.maxColumns)
			{
				throw new System.ArgumentOutOfRangeException ("values", values, string.Format ("Too many values ({0}, expected {1}).", values.Length, this.maxColumns));
			}

			if (this.textArrayStore != null)
			{
				for (int i = 0; i < this.maxColumns; i++)
				{
					this.textArrayStore.SetCellText (row, i, values[i]);
				}

				return;
			}

			System.Diagnostics.Debug.Assert (this.textArray.Count == this.maxRows);

			bool changed = (row >= this.maxRows);

			this.RowCount = System.Math.Max (this.maxRows, row + 1);

			if (!changed)
			{
				//	Vérifie si la nouvelle ligne est identique à la version originale dans
				//	la table :

				string[] original = this.GetRowTexts (row);

				if (original.Length == values.Length)
				{
					for (int i = 0; i < values.Length; i++)
					{
						if (original[i] != values[i])
						{
							changed = true;
							break;
						}
					}

					if (!changed)
					{
						return;
					}
				}
			}

			System.Collections.ArrayList line = this.textArray[row] as System.Collections.ArrayList;

			line.Clear ();
			line.AddRange (values);

			this.OnContentsChanged ();
			this.InvalidateContents ();
		}


		public Drawing.Rectangle GetRowBounds(int row)
		{
			if ((row < 0) || (row >= this.maxRows))
			{
				return Drawing.Rectangle.Empty;
			}

			int addRows  = (row == this.editionRow) ? this.editionAddRows : 0;
			int firstRow = this.ToVirtualRow (row);
			int lastRow  = firstRow + addRows;

			if ((lastRow >= this.firstVirtvisRow) &&
				(firstRow < this.firstVirtvisRow + this.nVisibleRows))
			{
				//	La ligne spécifiée est (en tout cas partiellement) visible; calcule sa
				//	position dans la liste :

				firstRow = System.Math.Max (firstRow, this.firstVirtvisRow);
				lastRow  = System.Math.Min (lastRow, this.firstVirtvisRow + this.nVisibleRows - 1);

				double y1 = (firstRow - this.firstVirtvisRow) * this.rowHeight;
				double y2 = (lastRow - this.firstVirtvisRow + 1) * this.rowHeight;

				y1 = System.Math.Min (y1, this.innerBounds.Height);
				y2 = System.Math.Min (y2, this.innerBounds.Height);

				if (y2 > y1)
				{
					y1 = this.innerBounds.Top - y1;
					y2 = this.innerBounds.Top - y2;

					return new Drawing.Rectangle (this.innerBounds.Left, y2, this.innerBounds.Width, y1 - y2);
				}
			}

			return Drawing.Rectangle.Empty;
		}

		public Drawing.Rectangle GetCellBounds(int row, int column)
		{
			Drawing.Rectangle bounds = this.GetRowBounds (row);

			if (bounds.IsValid)
			{
				ColumnDefinition def = this.Columns[column];

				double x1 = this.innerBounds.Left - this.offset + def.Offset;
				double x2 = x1 + def.Width;

				x1 = System.Math.Max (this.innerBounds.Left, x1);
				x2 = System.Math.Min (this.innerBounds.Right, x2);

				if (x1 < x2)
				{
					bounds.Left  = x1;
					bounds.Right = x2;

					return bounds;
				}
			}

			return Drawing.Rectangle.Empty;
		}

		public Drawing.Rectangle GetUnclippedRowBounds(int row)
		{
			if ((row < 0) || (row >= this.maxRows))
			{
				return Drawing.Rectangle.Empty;
			}

			int addRows  = (row == this.editionRow) ? this.editionAddRows : 0;
			int firstRow = this.ToVirtualRow (row);
			int lastRow  = firstRow + addRows;

			if ((lastRow >= this.firstVirtvisRow) &&
				(firstRow < this.firstVirtvisRow + this.nVisibleRows))
			{
				//	La ligne spécifiée est (en tout cas partiellement) visible; calcule sa
				//	position dans la liste :

				firstRow = System.Math.Max (firstRow, this.firstVirtvisRow);
				lastRow  = System.Math.Min (lastRow, this.firstVirtvisRow + this.nVisibleRows - 1);

				double y1 = (firstRow - this.firstVirtvisRow) * this.rowHeight;
				double y2 = (lastRow - this.firstVirtvisRow + 1) * this.rowHeight;

				if (y2 > y1)
				{
					y1 = this.innerBounds.Top - y1;
					y2 = this.innerBounds.Top - y2;

					return new Drawing.Rectangle (this.innerBounds.Left, y2, this.innerBounds.Width, y1 - y2);
				}
			}

			return Drawing.Rectangle.Empty;
		}

		public Drawing.Rectangle GetUnclippedCellBounds(int row, int column)
		{
			Drawing.Rectangle bounds = this.GetUnclippedRowBounds (row);

			if (bounds.IsValid)
			{
				double x1;
				double x2;

				if (this.GetUnclippedCellX (column, out x1, out x2))
				{
					bounds.Left  = x1;
					bounds.Right = x2;

					return bounds;
				}
			}

			return Drawing.Rectangle.Empty;
		}

		protected bool GetUnclippedCellX(int column, out double x1, out double x2)
		{
			ColumnDefinition def = this.Columns[column];

			x1 = this.innerBounds.Left - this.offset + def.Offset;
			x2 = x1 + def.Width;

			if (x1 < x2)
			{
				return true;
			}

			return false;
		}


		public void Clear()
		{
			//	Purge le contenu de la table, pour autant que l'on soit en mode FillText.

			if ((this.textProviderCallback == null) &&
				(this.textArrayStore == null))
			{
				this.textArray.Clear ();
			}

			this.maxRows           = 0;
			this.firstVirtvisRow   = 0;
			this.SelectedItemIndex = -1;

			this.InvalidateContents ();
		}

		public void InvalidateContents()
		{
			this.cacheVisibleRows = -1;
			this.isDirty = true;
			this.Invalidate ();
			this.OnContentsInvalidated ();
		}

		public void SyncWithTextArrayStore(bool update)
		{
			if (this.textArrayStore != null)
			{
				this.ColumnCount = this.textArrayStore.GetColumnCount ();
				this.RowCount    = this.textArrayStore.GetRowCount ();
			}

			this.InvalidateContents ();

			if (update)
			{
				this.Update ();
			}
		}

		public bool HitTestTable(Drawing.Point pos)
		{
			return this.innerBounds.Contains (pos);
		}

		public bool HitTestTable(Drawing.Point pos, out int row, out int column)
		{
			if (this.HitTestTable (pos))
			{
				double x = this.offset + pos.X - this.innerBounds.Left;
				double y = this.innerBounds.Top - pos.Y;

				int line = (int) (y / this.rowHeight);
				int top  = this.firstVirtvisRow;

				if ((line < 0) ||
					(line >= this.nVisibleRows) ||
					(line + top >= this.VirtualRowCount))
				{
					goto invalid;
				}

				double width = 0;

				for (int i = 0; i < this.Columns.Length; i++)
				{
					width += this.Columns[i].Width;

					if (x < width)
					{
						row    = this.FromVirtualRow (line + top);
						column = i;

						return true;
					}
				}
			}

		invalid:
			row    = -1;
			column = -1;

			return false;
		}


		public void SetColumnAlignment(int column, Drawing.ContentAlignment alignment)
		{
			this.Columns[column].Alignment = alignment;
		}

		public Drawing.ContentAlignment GetColumnAlignment(int column)
		{
			return this.Columns[column].Alignment;
		}


		public void SetHeaderText(int column, string text)
		{
			if (column < 0 || column >= this.maxColumns)
			{
				throw new System.ArgumentOutOfRangeException ("column", column, "Column index out of range");
			}

			this.FindButton (column).Text = text;
			this.InvalidateContents ();
		}

		public string GetHeaderText(int column)
		{
			if (column < 0 || column >= this.maxColumns)
			{
				throw new System.ArgumentOutOfRangeException ("column", column, "Column index out of range");
			}

			return this.FindButton (column).Text;
		}


		public void SetSortingHeader(int column, SortMode mode)
		{
			if (column < 0 || column >= this.maxColumns)
			{
				throw new System.ArgumentOutOfRangeException ("column", column, "Column index out of range");
			}

			if (this.FindButton (column).SortMode != mode)
			{
				for (int i = 0; i < this.maxColumns; i++)
				{
					this.FindButton (i).SortMode = i == column ? mode : SortMode.None;
				}

				this.OnSortChanged ();
			}
		}

		public bool GetSortingHeader(out int column, out SortMode mode)
		{
			for (int i = 0; i < this.maxColumns; i++)
			{
				HeaderButton button = this.FindButton (i);

				if (button.SortMode != SortMode.None)
				{
					column = i;
					mode   = button.SortMode;

					return true;
				}
			}

			column = -1;
			mode   = SortMode.None;

			return false;
		}


		public void ShowSelected(ScrollShowMode mode)
		{
			this.ShowRow (mode, this.selectedRow);
		}

		public void ShowEdition(ScrollShowMode mode)
		{
			this.ShowRow (mode, this.editionRow);
		}

		public void ShowRow(ScrollShowMode mode, int row)
		{
			if ((row == -1) ||
				(this.isMouseDown))
			{
				return;
			}

			int top    = this.firstVirtvisRow;
			int first  = top;
			int num    = System.Math.Min (this.nFullyVisibleRows, this.maxRows);
			int height = (row == this.editionRow) ? this.editionAddRows+1 : 1;

			row = this.ToVirtualRow (row);

			switch (mode)
			{
				case ScrollShowMode.Extremity:

					if (row < top)
					{
						//	La ligne était en-dessus du sommet de la liste. Utilise comme
						//	sommet la ligne sélectionnée...

						first = row;
					}

					if (row > top + this.nFullyVisibleRows - height)
					{
						//	La ligne était en-dessous du bas de la liste :

						first = row - (this.nFullyVisibleRows - height);
					}
					break;

				case ScrollShowMode.Center:
					first = System.Math.Min (row + num / 2, this.maxRows - 1);
					first = System.Math.Max (first - num + 1, 0);
					break;
			}

			if (this.firstVirtvisRow != first)
			{
				this.firstVirtvisRow = first;
				this.InvalidateContents ();

				Message.ResetButtonDownCounter ();
			}
		}

		public void ShowColumn(ScrollShowMode mode, int column)
		{
			if ((column == -1) ||
				(this.isMouseDown))
			{
				return;
			}

			column = System.Math.Max (column, 0);
			column = System.Math.Min (column, this.maxColumns-1);

			double dx = this.GetColumnWidth (column);
			double ox = this.GetColumnOffset (column);
			double x1 = ox - this.offset;
			double x2 = ox + dx - this.offset;
			double minX = 0;
			double maxX = this.innerBounds.Width;
			double offset = this.offset;

			switch (mode)
			{
				case ScrollShowMode.Extremity:

					if (x2 > maxX)
					{
						//	La colonne dépasse à droite, on ajuste l'offset pour que la colonne
						//	soit alignée sur son bord droit; ceci peut être corrigé ensuite si
						//	du coup la colonne dépasse à gauche.

						x2 = maxX;
						x1 = maxX - dx;
					}
					if (x1 < minX)
					{
						//	La colonne dépasse à gauche, on ajuste l'offset pour que la colonne
						//	soit juste visible.

						x1 = 0;
						x2 = dx;
					}
					break;

				case ScrollShowMode.Center:

					x1 = (maxX - minX - dx) / 2;
					x2 = x1 + dx;
					break;
			}

			offset = ox - x1;

			if (this.offset != offset)
			{
				this.offset = offset;
				this.InvalidateContents ();

				Message.ResetButtonDownCounter ();
			}
		}

		public void ShowCell(ScrollShowMode mode, int row, int column)
		{
			this.ShowRow (mode, row);
			this.ShowColumn (mode, column);
		}


#if false
		public bool AdjustHeight(ScrollAdjustMode mode)
		{
			//	Ajuste la hauteur pour afficher pile un nombre entier de lignes.
			
			this.Update ();
			
			double height = this.innerBounds.Height;
			int    num    = (int) (height / this.rowHeight);
			double adjust = height - num * this.rowHeight;
			
			if (adjust == 0)
			{
				return false;
			}
			
			switch (mode)
			{
				case ScrollAdjustMode.MoveTop:
					this.Top    = System.Math.Floor (this.Top - adjust);
					break;
				
				case ScrollAdjustMode.MoveBottom:
					this.Bottom = System.Math.Floor (this.Bottom + adjust);
					break;
				
				default:
					throw new System.NotSupportedException (string.Format ("Adjust mode {0} not supported.", mode));
			}

			this.Invalidate ();
			
			return true;
		}

		public bool AdjustHeightToContent(ScrollAdjustMode mode, double minHeight, double maxHeight)
		{
			//	Ajuste la hauteur pour afficher exactement le nombre de lignes contenues.
			
			this.Update ();
			
			double height = this.rowHeight * this.VirtualRowCount + this.frameMargins.Height + this.tableMargins.Height;
			double desire = height;
			
			height = System.Math.Max (height, minHeight);
			height = System.Math.Min (height, maxHeight);
			
			if (height == this.Height)
			{
				return false;
			}
			
			switch (mode)
			{
				case ScrollAdjustMode.MoveTop:
					this.Top    = this.Bottom + height;
					break;
				
				case ScrollAdjustMode.MoveBottom:
					this.Bottom = this.Top - height;
					break;
				
				default:
					throw new System.NotSupportedException (string.Format ("Adjust mode {0} not supported.", mode));
			}
			
			this.Invalidate ();
			
			if (height != desire)
			{
				this.AdjustHeight (mode);
			}
			
			return true;
		}
		
		public bool AdjustHeightToRows(ScrollAdjustMode mode, int count)
		{
			//	Ajuste la hauteur pour afficher exactement le nombre de lignes spécifié.
			
			this.Update ();
			
			double height = this.rowHeight * count + this.frameMargins.Height + this.tableMargins.Height;
			
			if (height == this.Height)
			{
				return false;
			}
			
			switch (mode)
			{
				case ScrollAdjustMode.MoveTop:
					this.Top    = this.Bottom + height;
					break;
				
				case ScrollAdjustMode.MoveBottom:
					this.Bottom = this.Top - height;
					break;
				
				default:
					throw new System.NotSupportedException (string.Format ("Adjust mode {0} not supported.", mode));
			}
			
			this.Invalidate ();
			return true;
		}
#endif

		protected void SetFirstVirtualVisibleIndex(int value)
		{
			int n = this.VirtualRowCount - this.nFullyVisibleRows;
			value = System.Math.Max (value, 0);
			value = System.Math.Min (value, System.Math.Max (n, 0));

			if (value != this.firstVirtvisRow)
			{
				this.firstVirtvisRow = value;
				this.UpdateScrollers ();

				Message.ResetButtonDownCounter ();
			}
		}

		protected void SetInteractionMode(ScrollInteractionMode value)
		{
			if (this.interactionMode != value)
			{
				this.OnInteractionModeChanging ();
				this.interactionMode = value;
				this.InvalidateContents ();
				this.OnInteractionModeChanged ();
			}
		}


		private void HandleVScrollerChanged(object sender)
		{
			int virtualRow = (int) System.Math.Floor (this.vScroller.DoubleValue + 0.5);
			this.SetFirstVirtualVisibleIndex (virtualRow);
			this.UpdateScrollView ();
		}

		private void HandleHScrollerChanged(object sender)
		{
			this.HorizontalOffset = System.Math.Floor (this.hScroller.DoubleValue);
			this.UpdateScrollView ();
		}

		private void HandleHeaderButtonClicked(object sender, MessageEventArgs e)
		{
			HeaderButton button = sender as HeaderButton;

			if (button.IsDynamic)
			{
				int column = button.Index;
				SortMode mode   = button.SortMode;

				switch (mode)
				{
					case SortMode.Up:
					case SortMode.None:
						mode = SortMode.Down;
						break;

					case SortMode.Down:
						mode = SortMode.Up;
						break;
				}

				this.SetSortingHeader (column, mode);
			}
		}

		private void HandleSliderDragStarted(object sender, MessageEventArgs e)
		{
			this.isDraggingSlider = true;

			HeaderSlider slider = sender as HeaderSlider;
			this.DragStartedColumn (slider.Index, e.Message.Cursor.X);
		}

		private void HandleSliderDragMoved(object sender, MessageEventArgs e)
		{
			HeaderSlider slider = sender as HeaderSlider;
			this.DragMovedColumn (slider.Index, e.Message.Cursor.X);
		}

		private void HandleSliderDragEnded(object sender, MessageEventArgs e)
		{
			this.isDraggingSlider = false;

			HeaderSlider slider = sender as HeaderSlider;
			this.DragEndedColumn (slider.Index, e.Message.Cursor.X);
			this.DispatchDummyMouseMoveEvent ();
		}

		private void HandleStoreContentsChanged(object sender)
		{
			this.SyncWithTextArrayStore (false);
		}


		protected virtual void DragStartedColumn(int column, double pos)
		{
			this.dragIndex = column;
			this.dragPos   = pos;
			this.dragDim   = this.GetColumnWidth (column);
		}

		protected virtual void DragMovedColumn(int column, double pos)
		{
			double width = this.dragDim + pos - this.dragPos;

			this.SetColumnWidth (this.dragIndex, width);
			this.InvalidateContents ();
		}

		protected virtual void DragEndedColumn(int column, double pos)
		{
			this.UpdateTotalWidth ();
			this.UpdateScrollView ();
			this.InvalidateContents ();
			this.Update ();
		}


		protected override void ProcessMessage(Message message, Drawing.Point pos)
		{
			int row, column;

			switch (message.MessageType)
			{
				case MessageType.MouseDown:
					if (this.HitTestTable (pos, out row, out column) && message.IsLeftButton && this.isSelectEnabled)
					{
						this.isMouseDown = true;
						this.ProcessMouseSelect (pos);
						message.Consumer = this;
					}
					break;

				case MessageType.MouseMove:
					if (this.isMouseDown && message.IsLeftButton && this.isSelectEnabled)
					{
						this.ProcessMouseSelect (pos);
						message.Consumer = this;
					}
					break;

				case MessageType.MouseUp:
					if (this.isMouseDown)
					{
						this.ProcessMouseSelect (pos);
						this.isMouseDown = false;
						this.ShowSelected (ScrollShowMode.Extremity);
						message.Consumer = this;
					}
					break;

				case MessageType.MouseWheel:
					if (message.Wheel < 0)
						this.FirstVisibleRow++;
					if (message.Wheel > 0)
						this.FirstVisibleRow--;
					message.Consumer = this;
					break;

				case MessageType.KeyDown:
					if (this.ProcessKeyEvent (message))
					{
						message.Consumer = this;
					}
					break;
			}
		}

		protected virtual void ProcessMouseSelect(Drawing.Point pos)
		{
			int row, column;

			if ((this.HitTestTable (pos, out row, out column)) &&
				(this.CheckChangeSelectedIndexTo (row)))
			{
				this.SelectedItemIndex = row;
			}
		}

		protected virtual bool ProcessKeyEvent(Message message)
		{
			if ((message.IsAltPressed) ||
				(message.IsShiftPressed) ||
				(message.IsControlPressed))
			{
				return false;
			}

			int sel = this.SelectedItemIndex;

			switch (message.KeyCode)
			{
				case KeyCode.ArrowUp:
					sel--;
					break;
				case KeyCode.ArrowDown:
					sel++;
					break;
				case KeyCode.PageUp:
					sel -= this.FullyVisibleRowCount-1;
					break;
				case KeyCode.PageDown:
					sel += this.FullyVisibleRowCount-1;
					break;

				default:
					return false;
			}

			if (this.SelectedItemIndex != sel)
			{
				sel = System.Math.Max (sel, 0);
				sel = System.Math.Min (sel, this.RowCount-1);

				if (this.CheckChangeSelectedIndexTo (sel))
				{
					this.SelectedItemIndex = sel;
					this.ShowSelected (ScrollShowMode.Extremity);
				}
			}

			return true;
		}

		protected virtual bool CheckChangeSelectedIndexTo(int index)
		{
			return true;
		}

		protected override void SetBoundsOverride(Drawing.Rectangle oldRect, Drawing.Rectangle newRect)
		{
			base.SetBoundsOverride (oldRect, newRect);
			this.UpdateGeometry ();
		}


		public override Drawing.Margins GetShapeMargins()
		{
			return Widgets.Adorners.Factory.Active.GeometryListShapeMargins;
		}


		protected virtual void Update()
		{
			if (this.isDirty)
			{
				this.UpdateGeometry ();
				this.DispatchDummyMouseMoveEvent ();
			}
		}

		protected virtual void UpdateScrollView()
		{
		}

		protected virtual void UpdateGeometry()
		{
			if ((this.vScroller == null) ||
				(this.hScroller == null) ||
				(this.header == null))
			{
				return;
			}

			this.isDirty = false;

			this.UpdateRowHeight ();
			this.UpdateTableBounds ();
			this.UpdateTotalWidth ();
			this.UpdateVisibleRows ();
			this.UpdateLayoutCache ();
			this.UpdateHeaderGeometry ();
			this.UpdateScrollerGeometry ();
			this.UpdateScrollers ();
			this.UpdateTitleWidget ();
		}

		protected virtual void UpdateColumnCount()
		{
			ColumnDefinition[] oldColumns = this.columns;
			ColumnDefinition[] newColumns = new ColumnDefinition[this.maxColumns];

			int n = System.Math.Min (oldColumns.Length, newColumns.Length);

			for (int i = 0; i < n; i++)
			{
				newColumns[i] = oldColumns[i];
				oldColumns[i] = null;
			}
			for (int i = n; i < newColumns.Length; i++)
			{
				newColumns[i] = new ColumnDefinition (this, i, this.defWidth, Drawing.ContentAlignment.MiddleLeft);
			}
			for (int i = n; i < oldColumns.Length; i++)
			{
				oldColumns[i].Dispose ();
				oldColumns[i] = null;
			}

			this.columns = newColumns;

			this.InvalidateContents ();
			this.UpdateTotalWidth ();
			this.Update ();
		}

		protected virtual void UpdateRowHeight()
		{
			this.rowHeight = System.Math.Floor (Widget.DefaultFontHeight * 1.25 + 0.5);
		}

		protected virtual void UpdateTableBounds()
		{
			this.frameMargins = Widgets.Adorners.Factory.Active.GeometryArrayMargins;
			this.tableMargins = new Drawing.Margins (0, this.vScroller.PreferredWidth - 1, this.rowHeight + this.titleHeight, this.hScroller.PreferredHeight - 1);

			if (this.vScroller.Visibility == false)
			{
				this.tableMargins.Right = 0;
			}

			if (this.hScroller.Visibility == false)
			{
				this.tableMargins.Bottom = 0;
			}

			Drawing.Rectangle bounds = this.Client.Bounds;

			bounds.Deflate (this.frameMargins);
			bounds.Deflate (this.tableMargins);

			this.tableBounds = bounds;

			bounds.Deflate (this.innerMargins);

			this.innerBounds = bounds;
		}

		protected virtual void UpdateVisibleRows()
		{
			double v = this.innerBounds.Height / this.rowHeight;

			this.nVisibleRows       = (int) System.Math.Ceiling (v);	//	compte la dernière ligne partielle
			this.nFullyVisibleRows = (int) System.Math.Floor (v);	//	nb de lignes entières
		}

		protected virtual void UpdateLayoutCache()
		{
			//	Alloue le tableau des textes :

			int dx = System.Math.Max (this.nVisibleRows, 1);
			int dy = System.Math.Max (this.maxColumns, 1);

			if ((dx != this.cacheDx) ||
				(dy != this.cacheDy))
			{
				this.layouts = new TextLayout[dx, dy];
				this.cacheDx = dx;
				this.cacheDy = dy;
				this.cacheVisibleRows = -1;
			}
		}

		protected virtual void UpdateHeaderGeometry()
		{
			//	Positionne l'en-tête :

			Drawing.Rectangle rect = this.tableBounds;

			rect.Bottom = this.tableBounds.Top;
			rect.Top    = this.tableBounds.Top + this.rowHeight;

			this.header.SetManualBounds (rect);

			//	Place les boutons dans l'en-tête :

			rect.Bottom = 0;
			rect.Top    = this.header.ActualHeight;
			rect.Left   = -this.offset;

			for (int i = 0; i < this.maxColumns; i++)
			{
				HeaderButton button = this.FindButton (i);

				rect.Right = rect.Left + this.GetColumnWidth (i);

				button.SetManualBounds (rect);
				button.Show ();

				rect.Left  = rect.Right;
			}

			//	Place les sliders dans l'en-tête :

			rect.Bottom = 0;
			rect.Top    = this.header.ActualHeight;
			rect.Left   = -this.offset;

			for (int i = 0; i < this.maxColumns; i++)
			{
				HeaderSlider slider = this.FindSlider (i);
				Drawing.Rectangle bounds = rect;

				rect.Right   = rect.Left  + this.GetColumnWidth (i);
				bounds.Left  = rect.Right - this.sliderDim / 2;
				bounds.Right = rect.Right + this.sliderDim / 2;
				rect.Left    = rect.Right;

				slider.ZOrder = i;
				slider.SetManualBounds (bounds);
				slider.Visibility = (this.columns[i].Elasticity == 0);
			}
		}

		protected virtual void UpdateScrollerGeometry()
		{
			//	Place l'ascenseur vertical :

			Drawing.Rectangle rect;

			rect       = this.tableBounds;
			rect.Left  = this.tableBounds.Right-1;
			rect.Right = this.tableBounds.Right-1 + this.vScroller.PreferredWidth;

			this.vScroller.SetManualBounds (rect);

			//	Place l'ascenseur horizontal :

			rect        = this.tableBounds;
			rect.Bottom = this.tableBounds.Bottom+1 - this.hScroller.PreferredHeight;
			rect.Top    = this.tableBounds.Bottom+1;

			this.hScroller.SetManualBounds (rect);
		}

		protected virtual void UpdateScrollers()
		{
			this.UpdateTagWidget ();

			//	Met à jour l'ascenseur vertical :

			int rows = this.VirtualRowCount;

			if ((rows <= this.nFullyVisibleRows) ||
				(rows <= 0) ||
				(this.nFullyVisibleRows <= 0))
			{
				this.vScroller.Enable            = false;
				this.vScroller.MaxValue          = 1;
				this.vScroller.VisibleRangeRatio = 1;
				this.vScroller.Value             = 0;
			}
			else
			{
				this.vScroller.Enable            = true;
				this.vScroller.MaxValue          = (decimal) (rows - this.nFullyVisibleRows);
				this.vScroller.VisibleRangeRatio = (decimal) (this.nFullyVisibleRows / (double) rows);
				this.vScroller.Value             = (decimal) (this.firstVirtvisRow);
				this.vScroller.SmallChange       = 1;
				this.vScroller.LargeChange       = (decimal) (this.nFullyVisibleRows / 2);
			}

			this.UpdateTextLayouts ();

			//	Met à jour l'ascenseur horizontal :

			double width = this.totalWidth;

			if ((width <= this.tableBounds.Width) ||
				(width <= 0) ||
				(this.tableBounds.Width <= 0))
			{
				this.hScroller.Enable            = false;
				this.hScroller.MaxValue          = 1;
				this.hScroller.VisibleRangeRatio = 1;
				this.hScroller.Value             = 0;
			}
			else
			{
				this.hScroller.Enable            = true;
				this.hScroller.MaxValue          = (decimal) (width - this.tableBounds.Width);
				this.hScroller.VisibleRangeRatio = (decimal) (this.tableBounds.Width / width);
				this.hScroller.Value             = (decimal) (this.offset);
				this.hScroller.SmallChange       = 10;
				this.hScroller.LargeChange       = (decimal) (this.tableBounds.Width / 2);
			}

			this.UpdateScrollView ();
			this.Invalidate ();
		}

		protected virtual void UpdateTextLayouts()
		{
			if (this.columns.Length == 0)
			{
				return;
			}

			int top     = this.FromVirtualRow (this.firstVirtvisRow);
			int bottom  = this.FromVirtualRow (this.firstVirtvisRow + this.nVisibleRows);

			top    = System.Math.Min (top, this.maxRows);
			bottom = System.Math.Min (bottom, this.maxRows);

			int height  = bottom - top;
			int max     = System.Math.Min (height, this.maxRows);
			bool refresh = (max != this.cacheVisibleRows) || (this.firstVirtvisRow != this.cacheFirstVirtvisRow);

			this.cacheVisibleRows      = max;
			this.cacheFirstVirtvisRow = this.firstVirtvisRow;

			Support.ResourceManager manager = Helpers.VisualTree.GetResourceManager (this);

			for (int row = 0; row < max; row++)
			{
				for (int column = 0; column < this.maxColumns; column++)
				{
					if (refresh)
					{
						if (this.layouts[row, column] == null)
						{
							this.layouts[row, column] = new TextLayout ();
							this.layouts[row, column].SetEmbedder (this);
						}

						string text = this[row + top, column];

						this.layouts[row, column].Text = text;
					}

					this.layouts[row, column].LayoutSize = new Drawing.Size (this.Columns[column].Width - this.textMargin * 2, this.rowHeight);
					this.layouts[row, column].Alignment  = this.Columns[column].Alignment;
				}
			}
			if (max > -1)
			{
				for (int row = max; row < this.cacheDx; row++)
				{
					for (int column = 0; column < this.maxColumns; column++)
					{
						this.layouts[row, column] = null;
					}
				}
			}

			this.OnLayoutUpdated ();
		}

		protected virtual void UpdateTotalWidth()
		{
			if (this.isDraggingSlider)
			{
				return;
			}

			double e = 0;
			double w = 0;

			for (int i = 0; i < this.maxColumns; i++)
			{
				double elasticity = this.columns[i].Elasticity;

				if (elasticity != 0)
				{
					e += elasticity;
				}
				else
				{
					w += this.columns[i].Width;
				}
			}

			if (e > 0)
			{
				double dw = this.tableBounds.Width - w;

				if (dw != 0)
				{
					for (int i = 0; i < this.maxColumns; i++)
					{
						if (this.columns[i].Elasticity != 0)
						{
							this.columns[i].AdjustWidth (dw * this.columns[i].Elasticity / e);
						}
					}
				}
			}


			double x = 0;
			this.totalWidth = 0;

			for (int i = 0; i < this.maxColumns; i++)
			{
				this.columns[i].DefineOffset (x);
				x += this.columns[i].Width;
			}

			this.totalWidth = x;
		}


		protected virtual void UpdateTitleWidget()
		{
			if (this.titleWidget != null)
			{
				this.titleWidget.SetManualBounds (this.TitleBounds);
			}
		}

		protected virtual void UpdateTagWidget()
		{
			if (this.tagWidget != null)
			{
				Drawing.Rectangle bounds = this.GetRowBounds (this.SelectedItemIndex);

				bounds.Inflate (0, 0, 0, 1);

				if ((this.hScroller.Visibility) &&
					(bounds.Bottom < this.hScroller.ActualBounds.Top))
				{
					bounds.Bottom = this.hScroller.ActualBounds.Top;
				}

				if (bounds.IsSurfaceZero)
				{
					this.clipWidget.Hide ();
				}
				else
				{
					this.clipWidget.SetManualBounds (bounds);
					this.clipWidget.Show ();

					double dx = System.Math.Min (this.rowHeight + 1, 18);
					double dy = dx;
					double ox = bounds.Right - (dx + 1);
					double oy = bounds.Top - dy;
					double x1, x2;

					if (this.GetUnclippedCellX (this.maxColumns-1, out x1, out x2))
					{
						x2 -= dx - 1;
						ox  = System.Math.Min (ox, x2);
					}

					ox -= this.clipWidget.ActualLocation.X;
					oy -= this.clipWidget.ActualLocation.Y;

					this.tagWidget.SetManualBounds (new Drawing.Rectangle (ox, oy, dx, dy));
				}
			}
		}


		protected override void OnAdornerChanged()
		{
			this.UpdateGeometry ();
			base.OnAdornerChanged ();
		}

		protected virtual void OnSelectedItemChanging()
		{
			var handler = this.GetUserEventHandler ("SelectedItemChanging");
			if (handler != null)
			{
				handler (this);
			}
		}

		protected virtual void OnSelectedItemChanged()
		{
			var handler = this.GetUserEventHandler ("SelectedItemChanged");
			if (handler != null)
			{
				handler (this);
			}
		}

		protected virtual void OnContentsChanged()
		{
			var handler = this.GetUserEventHandler ("ContentsChanged");
			if (handler != null)
			{
				handler (this);
			}
		}

		protected virtual void OnContentsInvalidated()
		{
			var handler = this.GetUserEventHandler ("ContentsInvalidated");
			if (handler != null)
			{
				handler (this);
			}
		}

		protected virtual void OnSortChanged()
		{
			var handler = this.GetUserEventHandler ("SortChanged");
			if (handler != null)
			{
				handler (this);
			}
		}

		protected virtual void OnLayoutUpdated()
		{
			var handler = this.GetUserEventHandler ("LayoutUpdated");
			if (handler != null)
			{
				handler (this);
			}
		}

		protected virtual void OnTextArrayStoreContentsChanged()
		{
			var handler = this.GetUserEventHandler ("TextArrayStoreContentsChanged");
			if (handler != null)
			{
				handler (this);
			}
		}

		protected virtual void OnInteractionModeChanging()
		{
			var handler = this.GetUserEventHandler ("InteractionModeChanging");
			if (handler != null)
			{
				handler (this);
			}
		}

		protected virtual void OnInteractionModeChanged()
		{
			var handler = this.GetUserEventHandler ("InteractionModeChanged");
			if (handler != null)
			{
				handler (this);
			}
		}


		protected HeaderButton FindButton(int index)
		{
			return this.Columns[index].HeaderButton;
		}

		protected HeaderSlider FindSlider(int index)
		{
			return this.Columns[index].HeaderSlider;
		}


		public int FindRow(string[] values)
		{
			int rows = this.RowCount;
			int cols = System.Math.Min (values.Length, this.maxColumns);

			for (int i = 0; i < rows; i++)
			{
				bool match = true;

				for (int j = 0; j < cols; j++)
				{
					if (values[j] != null)
					{
						if (!this[i, j].StartsWith (values[j]))
						{
							match = false;
							break;
						}
					}
				}

				if (match)
				{
					return i;
				}
			}

			return -1;
		}


		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			base.PaintBackgroundImplementation (graphics, clipRect);

			if (this.Columns.Length == 0)
			{
				return;
			}

			IAdorner adorner = Widgets.Adorners.Factory.Active;
			Drawing.Rectangle rect    = this.Client.Bounds;
			WidgetPaintState state   = this.GetPaintState ();

			adorner.PaintArrayBackground (graphics, rect, state);

			Drawing.Rectangle localClip = this.MapClientToRoot (this.tableBounds);
			Drawing.Rectangle saveClip  = graphics.SaveClippingRectangle ();
			Drawing.Rectangle tableClip = this.MapClientToRoot (this.innerBounds);

			graphics.SetClippingRectangle (tableClip);

			//	Dessine le contenu du tableau, constitué des textes :

			this.Update ();

			double totalWidth = this.totalWidth;

			if (this.isDraggingSlider)
			{
				totalWidth = 0;

				for (int i = 0; i < this.maxColumns; i++)
				{
					totalWidth += this.Columns[i].Width;
				}
			}

			int top    = this.FromVirtualRow (this.firstVirtvisRow);						//	index de la ligne en haut
			int delta  = this.firstVirtvisRow - this.ToVirtualRow (top);					//	0 si complètement visible, n => déborde n 'lignes'
			Drawing.Point pos    = new Drawing.Point (this.innerBounds.Left, this.innerBounds.Top);
			double limit  = totalWidth - this.offset + this.innerBounds.Left + 1;
			double right  = System.Math.Min (this.innerBounds.Right, limit);

			//	Détermine le nombre de lignes (virtuelles) actuellement affichables. Ceci est limité
			//	par la place disponible et par le nombre total de lignes :

			int virtTop    = this.firstVirtvisRow;
			int virtBottom = virtTop + this.nVisibleRows;
			int virtEnd    = this.ToVirtualRow (this.maxRows);

			int nRows = System.Math.Min (virtBottom, virtEnd) - virtTop;

			//	Peint toutes les lignes (virtuelles) en sautant celles qui correspondent à la ligne
			//	réelle en cours d'édition :

			for (int row = 0; row < nRows; row++)
			{
				pos.X  = this.innerBounds.Left;
				pos.Y -= this.rowHeight;

				int rowLine      = row + top;
				int numAddLines = (this.editionRow == rowLine)  ? this.editionAddRows - delta : 0;
				WidgetPaintState widgetState  = state & (WidgetPaintState.Enabled | WidgetPaintState.Focused);
				WidgetPaintState textState    = state & (WidgetPaintState.Enabled);

				if ((this.selectedRow == rowLine) &&
					(this.editionRow < 0))
				{
					widgetState |= WidgetPaintState.Selected;
					textState   |= WidgetPaintState.Selected;
				}

				if (this.editionRow == rowLine)
				{
					pos.Y  -= this.rowHeight * numAddLines;
					nRows -= numAddLines;

					Drawing.Rectangle bounds = new Drawing.Rectangle (pos.X, pos.Y, right - pos.X, this.rowHeight * (1 + numAddLines));

					this.PaintRowBackground (row, rowLine, graphics, adorner, bounds, widgetState);
				}
				else
				{
					Drawing.Rectangle bounds = new Drawing.Rectangle (pos.X, pos.Y, right - pos.X, this.rowHeight);

					this.PaintRowBackground (row, rowLine, graphics, adorner, bounds, widgetState);
					//					this.PaintRowContents (row, rowLine, graphics, adorner, bounds, textState, localClip);
					this.PaintRowContents (row, rowLine, graphics, adorner, bounds, textState, this.tableBounds);  // DR: correction du 12.10.04
				}
			}

			graphics.RestoreClippingRectangle (localClip);

			nRows = System.Math.Min (virtBottom, virtEnd) - virtTop;

			rect = this.tableBounds;
			rect.Inflate (-0.5, -0.5);
			graphics.LineWidth = 1;

			Drawing.Color color = adorner.ColorTextFieldBorder ((state & WidgetPaintState.Enabled) != 0);

			//	Dessine le rectangle englobant :

			graphics.AddRectangle (rect);
			graphics.RenderSolid (color);

			{
				//	Dessine les lignes de séparation horizontales :

				double x1 = this.innerBounds.Left;
				double x2 = right - 0.5;
				double y  = this.innerBounds.Top - 0.5;

				graphics.AddLine (x1, y, x2, y);

				for (int i = 0; i < nRows; i++)
				{
					int rowLine = i + top;

					if (this.editionRow == rowLine)
					{
						y      -= this.rowHeight * (this.editionAddRows - delta);
						nRows -= this.editionAddRows;
					}

					y -= this.rowHeight;

					graphics.AddLine (x1, y, x2, y);
				}
			}
			{
				//	Dessine les lignes de séparation verticales :

				limit = this.VirtualRowCount * this.rowHeight;
				limit = this.innerBounds.Top - (limit - top * this.rowHeight);

				double y1 = System.Math.Max (this.innerBounds.Bottom, limit);
				double y2 = this.innerBounds.Top - 0.5;
				double x  = this.innerBounds.Left - this.offset + 0.5;

				for (int i = 0; i < this.maxColumns; i++)
				{
					x += this.GetColumnWidth (i);

					if ((x < this.innerBounds.Left) ||
						(x > this.innerBounds.Right))
					{
						continue;
					}

					graphics.AddLine (x, y1, x, y2);
				}
			}

			graphics.RenderSolid (color);
			graphics.RestoreClippingRectangle (saveClip);
		}

		protected override void PaintForegroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			base.PaintForegroundImplementation (graphics, clipRect);

			IAdorner adorner = Widgets.Adorners.Factory.Active;
			Drawing.Rectangle rect    = this.Client.Bounds;
			WidgetPaintState state   = this.GetPaintState ();

			adorner.PaintArrayForeground (graphics, rect, state);
		}


		protected virtual void PaintRowBackground(int row, int rowLine, Drawing.Graphics graphics, IAdorner adorner, Drawing.Rectangle bounds, WidgetPaintState state)
		{
			adorner.PaintCellBackground (graphics, bounds, state);
		}

		protected virtual void PaintRowContents(int row, int rowLine, Drawing.Graphics graphics, IAdorner adorner, Drawing.Rectangle bounds, WidgetPaintState state, Drawing.Rectangle clip)
		{
			double x1 = bounds.X;
			double y1 = bounds.Y + 0.5;

			x1 += this.textMargin;
			x1 -= System.Math.Floor (this.offset);

			for (int column = 0; column < this.maxColumns; column++)
			{
				double x2 = x1 + this.columns[column].Width;

				if ((x1 < clip.Right) &&
					(x2 > clip.Left))
				{
					TextLayout layout = this.layouts[row, column];

					if ((layout != null) &&
						(layout.Text.Length > 0))
					{
						this.PaintCellContents (rowLine, column, graphics, adorner, new Drawing.Point (x1, y1), state, layout);
					}
				}

				x1 = x2;
			}
		}

		protected virtual void PaintCellContents(int rowLine, int column, Drawing.Graphics graphics, IAdorner adorner, Drawing.Point pos, WidgetPaintState state, TextLayout layout)
		{
			adorner.PaintGeneralTextLayout (graphics, Drawing.Rectangle.MaxValue, pos, layout, state, PaintTextStyle.Array, TextFieldDisplayMode.Default, this.BackColor);
		}


		#region	IStringSelection Members
		public int SelectedItemIndex
		{
			get
			{
				return this.selectedRow;
			}
			set
			{
				if (value != -1)
				{
					value = System.Math.Max (value, 0);
					value = System.Math.Min (value, this.maxRows);
				}
				if (value != this.selectedRow)
				{
					this.OnSelectedItemChanging ();
					this.selectedRow = value;
					this.InvalidateContents ();
					this.OnSelectedItemChanged ();
				}
			}
		}

		public string SelectedItem
		{
			get
			{
				int row = this.SelectedItemIndex;

				if (row == -1)
				{
					return null;
				}

				string[] rows = new string[this.maxColumns];

				for (int i = 0; i < rows.Length; i++)
				{
					rows[i] = this[row, i];
				}

				return string.Join (this.separator.ToString (), rows);
			}
			set
			{
				this.SelectedItemIndex = this.FindRow (value.Split (this.separator));
			}
		}
		#endregion

		public event EventHandler InteractionModeChanging
		{
			add
			{
				this.AddUserEventHandler ("InteractionModeChanging", value);
			}
			remove
			{
				this.RemoveUserEventHandler ("InteractionModeChanging", value);
			}
		}

		public event EventHandler InteractionModeChanged
		{
			add
			{
				this.AddUserEventHandler ("InteractionModeChanged", value);
			}
			remove
			{
				this.RemoveUserEventHandler ("InteractionModeChanged", value);
			}
		}

		public event EventHandler ContentsChanged
		{
			add
			{
				this.AddUserEventHandler ("ContentsChanged", value);
			}
			remove
			{
				this.RemoveUserEventHandler ("ContentsChanged", value);
			}
		}

		public event EventHandler ContentsInvalidated
		{
			add
			{
				this.AddUserEventHandler ("ContentsInvalidated", value);
			}
			remove
			{
				this.RemoveUserEventHandler ("ContentsInvalidated", value);
			}
		}

		public event EventHandler SortChanged
		{
			add
			{
				this.AddUserEventHandler ("SortChanged", value);
			}
			remove
			{
				this.RemoveUserEventHandler ("SortChanged", value);
			}
		}

		public event EventHandler LayoutUpdated
		{
			add
			{
				this.AddUserEventHandler ("LayoutUpdated", value);
			}
			remove
			{
				this.RemoveUserEventHandler ("LayoutUpdated", value);
			}
		}

		public event EventHandler TextArrayStoreContentsChanged
		{
			add
			{
				this.AddUserEventHandler ("TextArrayStoreContentsChanged", value);
			}
			remove
			{
				this.RemoveUserEventHandler ("TextArrayStoreContentsChanged", value);
			}
		}

		public event EventHandler SelectedItemChanging
		{
			add
			{
				this.AddUserEventHandler ("SelectedItemChanging", value);
			}
			remove
			{
				this.RemoveUserEventHandler ("SelectedItemChanging", value);
			}
		}

		public event EventHandler SelectedItemChanged
		{
			add
			{
				this.AddUserEventHandler ("SelectedItemChanged", value);
			}
			remove
			{
				this.RemoveUserEventHandler ("SelectedItemChanged", value);
			}
		}



		#region ColumnDefinition Class
		public class ColumnDefinition : System.IDisposable, Types.IReadOnly
		{
			internal ColumnDefinition(ScrollArray host, int column, double width, Drawing.ContentAlignment alignment)
			{
				this.host      = host;
				this.column    = column;
				this.width     = width;
				this.alignment = alignment;

				this.headerButton = new HeaderButton ();

				this.headerButton.Style     = HeaderButtonStyle.Top;
				this.headerButton.IsDynamic = false;
				this.headerButton.Index     = this.column;
				this.headerButton.Clicked  += this.host.HandleHeaderButtonClicked;
				this.headerButton.SetEmbedder (this.host.header);

				this.headerSlider = new HeaderSlider ();

				this.headerSlider.Style        = HeaderSliderStyle.Top;
				this.headerSlider.Index        = this.column;
				this.headerSlider.DragStarted += this.host.HandleSliderDragStarted;
				this.headerSlider.DragMoved   += this.host.HandleSliderDragMoved;
				this.headerSlider.DragEnded   += this.host.HandleSliderDragEnded;
				this.headerSlider.SetEmbedder (this.host.header);

				this.editionWidgetType = typeof (TextField);
			}


			public double Width
			{
				get
				{
					return this.width;
				}
				set
				{
					if (this.width != value)
					{
						this.width = value;
						this.host.OnColumnWidthChanged (this);
					}
				}
			}

			public double Offset
			{
				get
				{
					return this.offset;
				}
			}

			public Drawing.ContentAlignment Alignment
			{
				get
				{
					return this.alignment;
				}
				set
				{
					if (this.alignment != value)
					{
						this.alignment = value;
						this.host.OnColumnAlignmentChanged (this);
					}
				}
			}

			public string HeaderText
			{
				get
				{
					return this.headerButton.Text;
				}
				set
				{
					this.headerButton.Text = value;
				}
			}

			public HeaderButton HeaderButton
			{
				get
				{
					return this.headerButton;
				}
			}

			public HeaderSlider HeaderSlider
			{
				get
				{
					return this.headerSlider;
				}
			}

			public bool IsReadOnly
			{
				get
				{
					return this.isReadOnly;
				}
				set
				{
					this.isReadOnly = value;
				}
			}

			public Widget EditionWidget
			{
				get
				{
					return this.editionWidget;
				}
				set
				{
					this.editionWidget = value;
				}
			}

			public System.Type EditionWidgetType
			{
				get
				{
					if ((this.editionWidgetType == null) &&
						(this.editionWidgetModel != null))
					{
						return this.editionWidgetModel.GetType ();
					}

					return this.editionWidgetType;
				}
				set
				{
					this.editionWidgetType = value;
				}
			}

			public Widget EditionWidgetModel
			{
				get
				{
					return this.editionWidgetModel;
				}
				set
				{
					if (this.editionWidgetModel != value)
					{
						this.editionWidgetModel = value;

						if (value != null)
						{
							this.editionWidgetType = null;
						}
					}
				}
			}

			public double Elasticity
			{
				get
				{
					return this.elasticity;
				}
				set
				{
					if (this.elasticity != value)
					{
						this.elasticity = value;
						this.host.OnColumnWidthChanged (this);
					}
				}
			}

			internal void AdjustWidth(double width)
			{
				this.width = width;
			}

			internal void DefineColumnIndex(int index)
			{
				if (this.column != index)
				{
					this.headerButton.Index = index;
					this.headerSlider.Index = index;
				}
			}

			internal void DefineOffset(double offset)
			{
				this.offset = offset;
			}


			#region IDisposable Members
			public void Dispose()
			{
				this.Dispose (true);
			}
			#endregion

			#region IReadOnly Members
			bool Types.IReadOnly.IsReadOnly
			{
				get
				{
					return this.IsReadOnly;
				}
			}
			#endregion

			protected virtual void Dispose(bool isDisposing)
			{
				if (isDisposing)
				{
					if (this.headerButton != null)
					{
						this.headerButton.Clicked -= this.host.HandleHeaderButtonClicked;
						this.headerButton.SetParent (null);
						this.headerButton = null;
					}
					if (this.headerSlider != null)
					{
						this.headerSlider.DragStarted -= this.host.HandleSliderDragStarted;
						this.headerSlider.DragMoved   -= this.host.HandleSliderDragMoved;
						this.headerSlider.DragEnded   -= this.host.HandleSliderDragEnded;
						this.headerSlider.SetParent (null);
						this.headerSlider = null;
					}
				}
			}



			private ScrollArray host;
			private int column;
			private bool isReadOnly;
			private double width;
			private double offset;
			private double elasticity;
			private Drawing.ContentAlignment alignment;

			private HeaderButton headerButton;
			private HeaderSlider headerSlider;
			private System.Type editionWidgetType;
			private Widget editionWidgetModel;
			private Widget editionWidget;
		}
		#endregion


		protected bool isDirty;
		protected bool isMouseDown;

		protected int maxRows;
		protected int maxColumns;

		protected System.Collections.ArrayList textArray			= new System.Collections.ArrayList ();
		protected TextProviderCallback textProviderCallback;
		protected Support.Data.ITextArrayStore textArrayStore;
		protected TextLayout[,] layouts;

		protected double defWidth			= 100;
		protected double minWidth			= 10;
		protected double totalWidth;

		private ColumnDefinition[] columns;

		protected Drawing.Margins frameMargins;				//	marges du cadre
		protected Drawing.Margins tableMargins;				//	marges de la table interne
		protected double textMargin			= 2;
		protected double rowHeight			= 16;
		protected double titleHeight		= 0;
		protected Widget titleWidget;
		protected Drawing.Margins innerMargins;
		protected double sliderDim			= 6;

		protected Tag tagWidget;
		protected Widget clipWidget;

		protected Drawing.Rectangle tableBounds;
		protected Drawing.Rectangle innerBounds;
		protected Widget header;

		protected VScroller vScroller;
		protected HScroller hScroller;

		protected int nVisibleRows;
		protected int nFullyVisibleRows;
		protected int firstVirtvisRow;
		protected double offset;
		private int selectedRow		= -1;
		protected bool isSelectEnabled	= true;
		protected bool isDraggingSlider	= false;

		private int editionRow			= -1;
		protected int editionAddRows	= 0;

		protected int dragIndex;
		protected double dragPos;
		protected double dragDim;

		protected int cacheDx;
		protected int cacheDy;
		protected int cacheVisibleRows;
		protected int cacheFirstVirtvisRow;
		protected char separator = ';';

		private ScrollInteractionMode interactionMode = ScrollInteractionMode.ReadOnly;
	}
}
