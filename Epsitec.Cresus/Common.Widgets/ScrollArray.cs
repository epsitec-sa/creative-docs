//	Copyright � 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 13/02/2004

namespace Epsitec.Common.Widgets
{
	public enum ScrollShowMode
	{
		Extremity,		// d�placement minimal aux extr�mit�s
		Center,			// d�placement central
	}

	public enum ScrollAdjustMode
	{
		MoveUp,			// d�place le haut
		MoveDown,		// d�place le bas
	}

	public delegate string TextProviderCallback(int row, int column);

	/// <summary>
	///	La classe ScrollArray r�alise une liste d�roulante optimis�e � deux dimensions,
	///	ne pouvant contenir que des textes fixes.
	/// </summary>
	public class ScrollArray : Widget, Support.IStringSelection
	{
		public ScrollArray()
		{
			this.InternalState |= InternalState.AutoFocus;
			this.InternalState |= InternalState.Focusable;
			this.InternalState |= InternalState.AutoDoubleClick;
			
			IAdorner adorner = Widgets.Adorner.Factory.Active;
			
			this.frame_margins = adorner.GeometryArrayMargins;
			this.table_margins = new Drawing.Margins ();
			this.inner_margins = new Drawing.Margins ();
			this.row_height    = System.Math.Floor (this.DefaultFontHeight * 1.25 + 0.5);
			
			this.header = new Widget (this);
			this.v_scroller = new VScroller (this);
			this.h_scroller = new HScroller (this);
			this.v_scroller.IsInverted = true;
			this.v_scroller.ValueChanged += new Support.EventHandler (this.HandleVScrollerChanged);
			this.h_scroller.ValueChanged += new Support.EventHandler (this.HandleHScrollerChanged);
			
			this.is_dirty        = true;
			this.is_header_dirty = true;
		}

		public ScrollArray(Widget embedder) : this()
		{
			this.SetEmbedder (embedder);
		}

		
		public TextProviderCallback				TextProviderCallback
		{
			// Sp�cifie le d�l�gu� pour remplir les cellules.
			// En mode sans FillText, la liste est remplie � l'avance avec SetText.
			// Une copie de tous les strings est alors contenue dans this.array.
			// En mode FillText, c'est ScrollArray qui demande le contenu de chaque
			// cellule au fur et � mesure � l'aide du d�l�gu� FillText. Ce mode
			// est particuli�rement efficace pour de grandes quantit�s de donn�es.
			
			get
			{
				return this.text_provider_callback;
			}
			set
			{
				if (this.text_provider_callback != value)
				{
					if (value != null)
					{
						this.TextArrayStore = null;
					}
					
					this.text_provider_callback = value;
					
					this.Clear ();
				}
			}
		}
		
		public Support.Data.ITextArrayStore		TextArrayStore
		{
			get
			{
				return this.text_array_store;
			}
			set
			{
				if (this.text_array_store != value)
				{
					if (value != null)
					{
						this.TextProviderCallback = null;
					}
					
					if (this.text_array_store != null)
					{
						this.text_array_store.StoreChanged -= new Support.EventHandler (this.HandleStoreChanged);
					}
					
					this.text_array_store = value;
					
					if (this.text_array_store != null)
					{
						this.text_array_store.StoreChanged += new Support.EventHandler (this.HandleStoreChanged);
					}
					
					this.SyncWithTextArrayStore ();
				}
			}
		}

		public string							this[int row, int column]
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

		public char								Separator
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
		
		public int								ColumnCount
		{
			get
			{
				return this.max_columns;
			}
			set
			{
				if (this.max_columns != value)
				{
					this.max_columns = value;
					this.UpdateColumnCount ();
				}
			}
		}

		public int								RowCount
		{
			get
			{
				return this.max_rows;
			}
			set
			{
				if (this.max_rows != value)
				{
					this.max_rows = value;
					
					if ((this.text_provider_callback == null) &&
						(this.text_array_store == null))
					{
						//	Met � jour le nombre de lignes dans la table. Si la table est trop longue, on
						//	va la tronquer; si elle est trop courte, on va l'allonger.
						
						int n = this.text_array.Count;
						
						if (this.max_rows > n)
						{
							for (int i = n; i < this.max_rows; i++)
							{
								this.text_array.Add (new System.Collections.ArrayList ());
							}
						}
						else if (this.max_rows < n)
						{
							this.text_array.RemoveRange (this.max_rows, n - this.max_rows);
						}
					}
					
					this.InvalidateContents ();
				}
			}
		}
		
		public int								VisibleRowCount
		{
			get
			{
				this.Update ();
				return this.n_visible_rows;
			}
		}
		
		public int								FullyVisibleRowCount
		{
			get
			{
				this.Update ();
				return this.n_fully_visible_rows;
			}
		}
		
		public int								VirtualRowCount
		{
			get
			{
				return this.edition_row < 0 ? this.RowCount : this.RowCount + this.edition_add_rows;
			}
		}
		
		public int								FirstVisibleIndex
		{
			get
			{
				return this.FromVirtualRow (this.first_virtvis_row);
			}
			set
			{
				this.SetFirstVirtualVisibleIndex (this.ToVirtualRow (value));
			}
		}
		
		public int								EditionIndex
		{
			get
			{
				return this.edition_row;
			}
			set
			{
				if (value != -1)
				{
					value = System.Math.Max (value, 0);
					value = System.Math.Min (value, this.max_rows);
				}
				if (this.edition_row != value)
				{
					int top = this.FromVirtualRow (this.first_virtvis_row);
					
					this.edition_row       = value;
					this.first_virtvis_row = this.ToVirtualRow (top);
					
					this.InvalidateContents ();
					this.OnEditionIndexChanged ();
				}
			}
		}
		
		public int								EditionZoneHeight
		{
			get
			{
				return this.edition_add_rows + 1;
			}
			set
			{
				value--;
				if (value < 0) value = 0;
				
				if (this.edition_add_rows != value)
				{
					int top = this.FromVirtualRow (this.first_virtvis_row);
					
					this.edition_add_rows  = value;
					this.first_virtvis_row = this.ToVirtualRow (top);
					
					this.InvalidateContents ();
				}
			}
		}

		public double							HorizontalOffset
		{
			// Offset horizontal.
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

		public bool								IsSelectedVisible
		{
			get
			{
				if (this.selected_row == -1)
				{
					return true;
				}
				
				int row = this.ToVirtualRow (this.selected_row);
				
				if ((row >= this.first_virtvis_row) &&
					(row < this.first_virtvis_row + this.n_fully_visible_rows))
				{
					return true;
				}
				
				return false;
			}
		}
		
		
		public double							TitleHeight
		{
			get
			{
				return this.title_height;
			}
			set
			{
				if (this.title_height != value)
				{
					this.title_height = value;
					this.InvalidateContents ();
				}
			}
		}
		
		public Drawing.Rectangle				TitleBounds
		{
			get
			{
				this.Update ();
				
				Drawing.Rectangle bounds = new Drawing.Rectangle ();
				
				bounds.Left   = this.table_bounds.Left;
				bounds.Bottom = this.header.Top;
				bounds.Height = this.title_height;
				bounds.Right  = this.v_scroller.Right;
				
				return bounds;
			}
		}
		
		public Widget							TitleWidget
		{
			get
			{
				return this.title_widget;
			}
			set
			{
				if (this.title_widget != value)
				{
					if (this.title_widget != null)
					{
						this.title_widget.Parent = null;
					}
					
					this.title_widget = value;
					
					if (this.title_widget != null)
					{
						this.title_widget.SetEmbedder (this);
						this.UpdateTitleWidget ();
					}
				}
			}
		}
		
		
		public Tag								TagWidget
		{
			get
			{
				return this.tag_widget;
			}
			set
			{
				if (this.tag_widget != value)
				{
					if (this.tag_widget != null)
					{
						this.tag_widget.Parent = null;
					}
					
					this.tag_widget = value;
					
					if (this.tag_widget != null)
					{
						if (this.clip_widget == null)
						{
							this.clip_widget = new Widget (this);
						}
						
						this.tag_widget.SetEmbedder (this.clip_widget);
						
						this.UpdateTagWidget ();
					}
					else
					{
						if (this.clip_widget != null)
						{
							this.clip_widget.Parent = null;
							this.clip_widget = null;
						}
					}
				}
			}
		}
		
		public double							InnerTopMargin
		{
			get
			{
				return this.inner_margins.Top;
			}
			set
			{
				if (this.inner_margins.Top != value)
				{
					this.inner_margins.Top = value;
					this.InvalidateContents ();
				}
			}
		}
		
		public double							InnerBottomMargin
		{
			get
			{
				return this.inner_margins.Bottom;
			}
			set
			{
				if (this.inner_margins.Bottom != value)
				{
					this.inner_margins.Bottom = value;
					this.InvalidateContents ();
				}
			}
		}
		
		
		public double							FreeTableWidth
		{
			get
			{
				return this.inner_bounds.Width - this.total_width;
			}
		}
		
		public double							TotalTableWidth
		{
			get
			{
				return this.total_width;
			}
		}
		
		
		public int ToVirtualRow(int row)
		{
			if (this.edition_row < 0)
			{
				return row;
			}
			if (this.edition_row < row)
			{
				//	La ligne se trouve apr�s la zone d'�dition; il faut donc la d�caler vers
				//	le bas :
				
				return row + this.edition_add_rows;
			}
			
			return row;
		}
		
		public int FromVirtualRow(int row)
		{
			if (this.edition_row < 0)
			{
				return row;
			}
			if (this.edition_row + this.edition_add_rows < row)
			{
				//	La ligne se trouve apr�s la zone d'�dition; il faut donc la d�caler vers
				//	le haut :
				
				return row - this.edition_add_rows;
			}
			if (this.edition_row < row)
			{
				//	La ligne se trouve dans la zone d'�dition; il faut donc retourner le d�but
				//	de la zone d'�dition :
				
				return this.edition_row;
			}
			
			return row;
		}
		
		
		public void SetColumnWidth(int column, double width)
		{
			System.Diagnostics.Debug.Assert (this.column_widths != null);
			System.Diagnostics.Debug.Assert (column > -1);
			System.Diagnostics.Debug.Assert (column < this.column_widths.Length);
			
			width = System.Math.Floor (width);
			width = System.Math.Max (width, this.min_width);
			
			if (this.column_widths[column] != width)
			{
				this.column_widths[column] = width;
				this.UpdateTotalWidth ();
				this.UpdateScrollView ();
				this.InvalidateContents ();
			}
		}
		
		public double GetColumnWidth(int column)
		{
			System.Diagnostics.Debug.Assert (this.column_widths != null);
			System.Diagnostics.Debug.Assert (column > -1);
			System.Diagnostics.Debug.Assert (column < this.column_widths.Length);
			
			return this.column_widths[column];
		}
		
		
		public double GetColumnOffset(int column)
		{
			System.Diagnostics.Debug.Assert (this.column_widths != null);
			System.Diagnostics.Debug.Assert (column > -1);
			System.Diagnostics.Debug.Assert (column < this.column_widths.Length);
			
			double offset = 0;
			
			for (int i = 0; i < column; i++)
			{
				offset += this.column_widths[i];
			}
			
			return offset;
		}
		
		
		public virtual string GetCellText(int row, int column)
		{
			if (this.column_widths == null)
			{
				throw new System.InvalidOperationException (string.Format ("Cannot get cell [{0},{1}] in this ScrollArray.", row, column));
			}
			
			if ((row < 0) || (row >= this.max_rows))
			{
				throw new System.ArgumentOutOfRangeException ("row", row, "Row out of range.");
			}
			
			if ((column < 0) || (column >= this.max_columns))
			{
				throw new System.ArgumentOutOfRangeException ("column", column, "Column out of range.");
			}
			
			if (this.text_provider_callback != null)
			{
				return this.text_provider_callback (row, column);
			}
			if (this.text_array_store != null)
			{
				return this.text_array_store.GetCellText (row, column);
			}
			
			System.Collections.ArrayList line = this.text_array[row] as System.Collections.ArrayList;
			
			return (column >= line.Count) ? "" : line[column] as string;
		}
		
		public virtual string[] GetRowTexts(int row)
		{
			if (this.column_widths == null)
			{
				throw new System.InvalidOperationException (string.Format ("Cannot get row [{0}] in this ScrollArray.", row));
			}
			
			if ((row < 0) || (row >= this.max_rows))
			{
				throw new System.ArgumentOutOfRangeException ("row", row, "Row out of range.");
			}
			
			string[] values = new string[this.max_columns];
			 
			if (this.text_provider_callback != null)
			{
				for (int i = 0; i < this.max_columns; i++)
				{
					values[i] = this.text_provider_callback (row, i);
				}
			}
			else if (this.text_array_store != null)
			{
				for (int i = 0; i < this.max_columns; i++)
				{
					values[i] = this.text_array_store.GetCellText (row, i);
				}
			}
			else
			{
				System.Collections.ArrayList line = this.text_array[row] as System.Collections.ArrayList;
				
				for (int i = 0; i < this.max_columns; i++)
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
			if ((this.text_provider_callback != null) ||
				(this.column_widths == null))
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
			
			if (this.text_array_store != null)
			{
				this.text_array_store.SetCellText (row, column, value);
				return;
			}
			
			System.Diagnostics.Debug.Assert (this.text_array.Count == this.max_rows);
			
			bool changed = false;
			
			changed |= (row >= this.max_rows);
			changed |= (column >= this.max_columns);
			
			this.RowCount    = System.Math.Max (this.max_rows, row + 1);
			this.ColumnCount = System.Math.Max (this.max_columns, column + 1);
			
			System.Collections.ArrayList line = this.text_array[row] as System.Collections.ArrayList;
			
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
			if ((this.text_provider_callback != null) ||
				(this.column_widths == null))
			{
				throw new System.InvalidOperationException (string.Format ("Cannot set row [{0}] in this ScrollArray.", row));
			}
			
			if ((row < 0) || (row >= this.max_rows))
			{
				throw new System.ArgumentOutOfRangeException ("row", row, "Row out of range.");
			}
			
			if (values.Length > this.max_columns)
			{
				throw new System.ArgumentOutOfRangeException ("values", values, string.Format ("Too many values ({0}, expected {1}).", values.Length, this.max_columns));
			}
			
			if (this.text_array_store != null)
			{
				for (int i = 0; i < this.max_columns; i++)
				{
					this.text_array_store.SetCellText (row, i, values[i]);
				}
				
				return;
			}
			
			System.Diagnostics.Debug.Assert (this.text_array.Count == this.max_rows);
			
			bool changed = (row >= this.max_rows);
			
			this.RowCount = System.Math.Max (this.max_rows, row + 1);
			
			if (! changed)
			{
				//	V�rifie si la nouvelle ligne est identique � la version originale dans
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
					
					if (! changed)
					{
						return;
					}
				}
			}
			
			System.Collections.ArrayList line = this.text_array[row] as System.Collections.ArrayList;
			
			line.Clear ();
			line.AddRange (values);
			
			this.OnContentsChanged ();
			this.InvalidateContents ();
		}
		
		
		public Drawing.Rectangle GetRowBounds(int row)
		{
			if ((row < 0) || (row >= this.max_rows))
			{
				return Drawing.Rectangle.Empty;
			}
			
			int add_rows  = (row == this.edition_row) ? this.edition_add_rows : 0;
			int first_row = this.ToVirtualRow (row);
			int last_row  = first_row + add_rows;
			
			if ((last_row >= this.first_virtvis_row) &&
				(first_row < this.first_virtvis_row + this.n_visible_rows))
			{
				//	La ligne sp�cifi�e est (en tout cas partiellement) visible; calcule sa
				//	position dans la liste :
				
				first_row = System.Math.Max (first_row, this.first_virtvis_row);
				last_row  = System.Math.Min (last_row, this.first_virtvis_row + this.n_visible_rows - 1);
				
				double y1 = (first_row - this.first_virtvis_row) * this.row_height;
				double y2 = (last_row - this.first_virtvis_row + 1) * this.row_height;
				
				y1 = System.Math.Min (y1, this.inner_bounds.Height);
				y2 = System.Math.Min (y2, this.inner_bounds.Height);
				
				if (y2 > y1)
				{
					y1 = this.inner_bounds.Top - y1;
					y2 = this.inner_bounds.Top - y2;
					
					return new Drawing.Rectangle (this.inner_bounds.Left, y2, this.inner_bounds.Width, y1 - y2);
				}
			}
			
			return Drawing.Rectangle.Empty;
		}
		
		public Drawing.Rectangle GetCellBounds(int row, int column)
		{
			Drawing.Rectangle bounds = this.GetRowBounds (row);
			
			if (bounds.IsValid)
			{
				double x1 = 0;
				double x2 = 0;
				
				for (int i = 0; i < this.max_columns; i++)
				{
					x1 = x2;
					x2 = x1 + this.column_widths[i];
					
					if (column == i)
					{
						x1 += this.inner_bounds.Left - this.offset;
						x2 += this.inner_bounds.Left - this.offset;
						x1 = System.Math.Max (this.inner_bounds.Left, x1);
						x2 = System.Math.Min (this.inner_bounds.Right, x2);
						
						if (x1 < x2)
						{
							bounds.Left  = x1;
							bounds.Right = x2;
							
							return bounds;
						}
						
						break;
					}
				}
			}
			
			return Drawing.Rectangle.Empty;
		}
		
		public Drawing.Rectangle GetUnclippedRowBounds(int row)
		{
			if ((row < 0) || (row >= this.max_rows))
			{
				return Drawing.Rectangle.Empty;
			}
			
			int add_rows  = (row == this.edition_row) ? this.edition_add_rows : 0;
			int first_row = this.ToVirtualRow (row);
			int last_row  = first_row + add_rows;
			
			if ((last_row >= this.first_virtvis_row) &&
				(first_row < this.first_virtvis_row + this.n_visible_rows))
			{
				//	La ligne sp�cifi�e est (en tout cas partiellement) visible; calcule sa
				//	position dans la liste :
				
				first_row = System.Math.Max (first_row, this.first_virtvis_row);
				last_row  = System.Math.Min (last_row, this.first_virtvis_row + this.n_visible_rows - 1);
				
				double y1 = (first_row - this.first_virtvis_row) * this.row_height;
				double y2 = (last_row - this.first_virtvis_row + 1) * this.row_height;
				
				if (y2 > y1)
				{
					y1 = this.inner_bounds.Top - y1;
					y2 = this.inner_bounds.Top - y2;
					
					return new Drawing.Rectangle (this.inner_bounds.Left, y2, this.inner_bounds.Width, y1 - y2);
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
			x1 = 0;
			x2 = 0;
			
			for (int i = 0; i < this.max_columns; i++)
			{
				x1 = x2;
				x2 = x1 + this.column_widths[i];
				
				if (column == i)
				{
					x1 += this.inner_bounds.Left - this.offset;
					x2 += this.inner_bounds.Left - this.offset;
					
					if (x1 < x2)
					{
						return true;
					}
					
					break;
				}
			}
			
			return false;
		}
		
		
		public void Clear()
		{
			//	Purge le contenu de la table, pour autant que l'on soit en mode FillText.
			
			if ((this.text_provider_callback == null) &&
				(this.text_array_store == null))
			{
				this.text_array.Clear ();
			}
			
			this.max_rows          = 0;
			this.first_virtvis_row = 0;
			this.selected_row      = -1;
			this.edition_row       = -1;
			
			this.InvalidateContents ();
		}
		
		public virtual void InvalidateContents()
		{
			this.cache_visible_rows = -1;
			this.is_dirty = true;
			this.Invalidate ();
		}
		
		public void SyncWithTextArrayStore()
		{
			this.Clear ();
			
			if (this.text_array_store != null)
			{
				this.ColumnCount = this.text_array_store.GetColumnCount ();
				this.RowCount    = this.text_array_store.GetRowCount ();
			}
		}
		
		public bool HitTestTable(Drawing.Point pos)
		{
			return this.inner_bounds.Contains (pos);
		}
		
		public bool HitTestTable(Drawing.Point pos, out int row, out int column)
		{
			if (this.HitTestTable (pos))
			{
				double x = this.offset + pos.X - this.inner_bounds.Left;
				double y = this.inner_bounds.Top - pos.Y;
				
				int line = (int) (y / this.row_height);
				int top  = this.first_virtvis_row;
				
				if ((line < 0) ||
					(line >= this.n_visible_rows) ||
					(line + top >= this.VirtualRowCount))
				{
					goto invalid;
				}
				
				double width = 0;
				
				for (int i = 0; i < this.column_widths.Length; i++)
				{
					width += this.column_widths[i];
					
					if (x < width)
					{
						row    = this.FromVirtualRow (line + top);
						column = i;
						
						return true;
					}
				}
			}
			
invalid:	row    = -1;
			column = -1;
			
			return false;
		}
		
		
		public void SetColumnAlignment(int column, Drawing.ContentAlignment alignment)
		{
			System.Diagnostics.Debug.Assert (this.column_alignments != null);
			System.Diagnostics.Debug.Assert (column > -1);
			System.Diagnostics.Debug.Assert (column < this.column_alignments.Length);
			
			if (this.column_alignments[column] != alignment)
			{
				this.column_alignments[column] = alignment;
				this.InvalidateContents ();
			}
		}

		public Drawing.ContentAlignment GetColumnAlignment(int column)
		{
			System.Diagnostics.Debug.Assert (this.column_alignments != null);
			System.Diagnostics.Debug.Assert (column > -1);
			System.Diagnostics.Debug.Assert (column < this.column_alignments.Length);
			
			return this.column_alignments[column];
		}
		
		
		public void SetHeaderText(int column, string text)
		{
			if (column < 0 || column >= this.max_columns)
			{
				throw new System.ArgumentOutOfRangeException ("column", column, "Column index out of range");
			}
			
			this.FindButton (column).Text = text;
			this.InvalidateContents ();
		}
		
		public string GetHeaderText(int column)
		{
			if (column < 0 || column >= this.max_columns)
			{
				throw new System.ArgumentOutOfRangeException ("column", column, "Column index out of range");
			}
			
			return this.FindButton (column).Text;
		}
		
		
		public void SetSortingHeader(int column, SortMode mode)
		{
			if (column < 0 || column >= this.max_columns)
			{
				throw new System.ArgumentOutOfRangeException ("column", column, "Column index out of range");
			}
			
			if (this.FindButton (column).SortMode != mode)
			{
				for (int i = 0; i < this.max_columns; i++)
				{
					this.FindButton (i).SortMode = i == column ? mode : SortMode.None;
				}
				
				this.OnSortChanged ();
			}
		}
		
		public bool GetSortingHeader(out int column, out SortMode mode)
		{
			for (int i = 0; i < this.max_columns; i++)
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
			this.ShowRow (mode, this.selected_row);
		}
		
		public void ShowEdition(ScrollShowMode mode)
		{
			this.ShowRow (mode, this.edition_row);
		}
		
		public void ShowRow(ScrollShowMode mode, int row)
		{
			if ((row == -1) ||
				(this.is_mouse_down))
			{
				return;
			}
			
			int top    = this.first_virtvis_row;
			int first  = top;
			int num    = System.Math.Min (this.n_fully_visible_rows, this.max_rows);
			int height = (row == this.edition_row) ? this.edition_add_rows+1 : 1;
			
			row = this.ToVirtualRow (row);
			
			switch (mode)
			{
				case ScrollShowMode.Extremity:
					
					if (row < top)
					{
						//	La ligne �tait en-dessus du sommet de la liste. Utilise comme
						//	sommet la ligne s�lectionn�e...
						
						first = row;
					}
					
					if (row > top + this.n_fully_visible_rows - height)
					{
						//	La ligne �tait en-dessous du bas de la liste :
						
						first = row - (this.n_fully_visible_rows - height);
					}
					break;
				
				case ScrollShowMode.Center:
					first = System.Math.Min (row + num / 2, this.max_rows - 1);
					first = System.Math.Max (first - num + 1, 0);
					break;
			}
			
			if (this.first_virtvis_row != first)
			{
				this.first_virtvis_row = first;
				this.InvalidateContents ();
				
				Message.ResetButtonDownCounter ();
			}
		}
		
		public void ShowColumn(ScrollShowMode mode, int column)
		{
			if ((column == -1) ||
				(this.is_mouse_down))
			{
				return;
			}
			
			column = System.Math.Max (column, 0);
			column = System.Math.Min (column, this.max_columns-1);
			
			double dx = this.GetColumnWidth (column);
			double ox = this.GetColumnOffset (column);
			double x1 = ox - this.offset;
			double x2 = ox + dx - this.offset;
			double min_x = 0;
			double max_x = this.inner_bounds.Width;
			double offset = this.offset;
			
			switch (mode)
			{
				case ScrollShowMode.Extremity:
					
					if (x2 > max_x)
					{
						//	La colonne d�passe � droite, on ajuste l'offset pour que la colonne
						//	soit align�e sur son bord droit; ceci peut �tre corrig� ensuite si
						//	du coup la colonne d�passe � gauche.
						
						x2 = max_x;
						x1 = max_x - dx;
					}
					if (x1 < min_x)
					{
						//	La colonne d�passe � gauche, on ajuste l'offset pour que la colonne
						//	soit juste visible.
						
						x1 = 0;
						x2 = dx;
					}
					break;
				
				case ScrollShowMode.Center:
					
					x1 = (max_x - min_x - dx) / 2;
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

		
		public bool AdjustToMultiple(ScrollAdjustMode mode)
		{
			//	Ajuste la hauteur pour afficher pile un nombre entier de lignes.
			
			this.Update ();
			
			double height = this.inner_bounds.Height;
			int    num    = (int) (height / this.row_height);
			double adjust = height - num * this.row_height;
			
			if (adjust == 0)
			{
				return false;
			}
			
			switch (mode)
			{
				case ScrollAdjustMode.MoveUp:
					this.Top    = System.Math.Floor (this.Top - adjust);
					break;
				
				case ScrollAdjustMode.MoveDown:
					this.Bottom = System.Math.Floor (this.Bottom + adjust);
					break;
			}

			this.Invalidate ();
			
			return true;
		}

		public bool AdjustToContents(ScrollAdjustMode mode, double min_height, double max_height)
		{
			//	Ajuste la hauteur pour afficher exactement le nombre de lignes contenues.
			
			this.Update ();
			
			double height = this.row_height * this.VirtualRowCount + this.frame_margins.Height + this.table_margins.Height;
			double desire = height;
			
			height = System.Math.Max (height, min_height);
			height = System.Math.Min (height, max_height);
			
			if (height == this.Height)
			{
				return false;
			}
			
			switch (mode)
			{
				case ScrollAdjustMode.MoveUp:
					this.Top    = this.Bottom + height;
					break;
				case ScrollAdjustMode.MoveDown:
					this.Bottom = this.Top - height;
					break;
			}
			
			this.Invalidate ();
			
			if (height != desire)
			{
				this.AdjustToMultiple (mode);
			}
			
			return true;
		}
		
		public bool AdjustToRows(ScrollAdjustMode mode, int count)
		{
			//	Ajuste la hauteur pour afficher exactement le nombre de lignes sp�cifi�.
			
			this.Update ();
			
			double height = this.row_height * count + this.frame_margins.Height + this.table_margins.Height;
			
			if (height == this.Height)
			{
				return false;
			}
			
			switch (mode)
			{
				case ScrollAdjustMode.MoveUp:
					this.Top    = this.Bottom + height;
					break;
				case ScrollAdjustMode.MoveDown:
					this.Bottom = this.Top - height;
					break;
			}
			
			this.Invalidate ();
			return true;
		}

		
		protected void SetFirstVirtualVisibleIndex(int value)
		{
			int n = this.VirtualRowCount - this.n_fully_visible_rows;
			value = System.Math.Max (value, 0);
			value = System.Math.Min (value, System.Math.Max (n, 0));
			
			if (value != this.first_virtvis_row)
			{
				this.first_virtvis_row = value;
				this.UpdateScrollers ();
				
				Message.ResetButtonDownCounter ();
			}
		}
		
		
		private void HandleVScrollerChanged(object sender)
		{
			int virtual_row = (int) System.Math.Floor (this.v_scroller.DoubleValue + 0.5);
			this.SetFirstVirtualVisibleIndex (virtual_row);
			this.UpdateScrollView ();
		}

		private void HandleHScrollerChanged(object sender)
		{
			this.HorizontalOffset = System.Math.Floor (this.h_scroller.DoubleValue);
			this.UpdateScrollView ();
		}

		private void HandleHeaderButtonClicked(object sender, MessageEventArgs e)
		{
			HeaderButton button = sender as HeaderButton;
			
			int      column = button.Index;
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

		private void HandleSliderDragStarted(object sender, MessageEventArgs e)
		{
			HeaderSlider slider = sender as HeaderSlider;
			this.DragStartedColumn (slider.Index, e.Point.X);
		}

		private void HandleSliderDragMoved(object sender, MessageEventArgs e)
		{
			HeaderSlider slider = sender as HeaderSlider;
			this.DragMovedColumn (slider.Index, e.Point.X);
		}
		
		private void HandleSliderDragEnded(object sender, MessageEventArgs e)
		{
			HeaderSlider slider = sender as HeaderSlider;
			this.DragEndedColumn (slider.Index, e.Point.X);
		}

		private void HandleStoreChanged(object sender)
		{
			this.SyncWithTextArrayStore ();
		}
		
		
		protected virtual void DragStartedColumn(int column, double pos)
		{
			this.drag_index = column;
			this.drag_pos   = pos;
			this.drag_dim   = this.GetColumnWidth (column);
		}

		protected virtual void DragMovedColumn(int column, double pos)
		{
			double width = this.GetColumnWidth (column) + pos - this.drag_pos;

			this.SetColumnWidth (this.drag_index, width);
			this.InvalidateContents ();
		}

		protected virtual void DragEndedColumn(int column, double pos)
		{
			this.InvalidateContents ();
		}

		
		protected override void ProcessMessage(Message message, Drawing.Point pos)
		{
			switch (message.Type)
			{
				case MessageType.MouseDown:
					if (this.HitTestTable (pos) && message.IsLeftButton && this.is_select_enabled)
					{
						this.is_mouse_down = true;
						this.ProcessMouseSelect (pos);
						message.Consumer = this;
					}
					break;
				
				case MessageType.MouseMove:
					if (this.is_mouse_down && message.IsLeftButton && this.is_select_enabled)
					{
						this.ProcessMouseSelect (pos);
						message.Consumer = this;
					}
					break;
				
				case MessageType.MouseUp:
					if (this.is_mouse_down)
					{
						this.ProcessMouseSelect (pos);
						this.is_mouse_down = false;
						this.ShowSelected (ScrollShowMode.Extremity);
						message.Consumer = this;
					}
					break;
				
				case MessageType.MouseWheel:
					if ( message.Wheel < 0 )  this.FirstVisibleIndex ++;
					if ( message.Wheel > 0 )  this.FirstVisibleIndex --;
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

		protected virtual  void ProcessMouseSelect(Drawing.Point pos)
		{
			int row, column;
			
			if (this.HitTestTable (pos, out row, out column))
			{
				this.SelectedIndex = row;
			}
		}

		protected virtual  bool ProcessKeyEvent(Message message)
		{
			if ((message.IsAltPressed) ||
				(message.IsShiftPressed) ||
				(message.IsCtrlPressed))
			{
				return false;
			}
			
			int sel = this.SelectedIndex;
			
			switch (message.KeyCode)
			{
				case KeyCode.ArrowUp:	sel--;								break;
				case KeyCode.ArrowDown:	sel++;								break;
				case KeyCode.PageUp:	sel -= this.FullyVisibleRowCount-1;	break;
				case KeyCode.PageDown:	sel += this.FullyVisibleRowCount-1;	break;
				
				default:
					return false;
			}
			
			if (this.SelectedIndex != sel)
			{
				sel = System.Math.Max(sel, 0);
				sel = System.Math.Min(sel, this.RowCount-1);
				this.SelectedIndex = sel;
				this.ShowSelected(ScrollShowMode.Extremity);
			}
			
			return true;
		}

		
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry ();
			
			if ((this.column_widths == null) ||
				(this.v_scroller == null) ||
				(this.h_scroller == null) ||
				(this.header == null))
			{
				return;
			}
			
			this.UpdateGeometry ();
		}
		
		
		public override Drawing.Rectangle GetShapeBounds()
		{
			IAdorner          adorner = Widgets.Adorner.Factory.Active;
			Drawing.Rectangle rect    = this.Client.Bounds;
			
			rect.Inflate (adorner.GeometryListShapeBounds);
			
			return rect;
		}
		
		
		protected virtual void Update()
		{
			if (this.is_dirty)
			{
				this.UpdateClientGeometry ();
			}
		}
		
		protected virtual void UpdateScrollView()
		{
		}
		
		protected virtual void UpdateGeometry()
		{
			this.is_dirty = false;
			
			this.UpdateRowHeight ();
			this.UpdateTableBounds ();
			this.UpdateVisibleRows ();
			this.UpdateLayoutCache ();
			this.UpdateHeaderGeometry ();
			this.UpdateScrollerGeometry ();
			this.UpdateScrollers ();
			this.UpdateTitleWidget ();
		}
		
		protected virtual void UpdateColumnCount()
		{
			this.is_header_dirty   = true;
			this.column_widths     = new double[this.max_columns];
			this.column_alignments = new Drawing.ContentAlignment[this.max_columns];
					
			for (int i = 0; i < this.max_columns; i++)
			{
				this.column_widths[i] = this.def_width;
			}
					
			for (int i = 0; i < this.max_columns; i++)
			{
				this.column_alignments[i] = Drawing.ContentAlignment.MiddleLeft;
			}
					
			this.InvalidateContents ();
			this.UpdateTotalWidth ();
			this.UpdateHeaderContents ();
			this.Update ();
		}
		
		protected virtual void UpdateRowHeight()
		{
			this.row_height = System.Math.Floor (this.DefaultFontHeight * 1.25 + 0.5);
		}
		
		protected virtual void UpdateTableBounds()
		{
			this.frame_margins = Widgets.Adorner.Factory.Active.GeometryArrayMargins;
			this.table_margins = new Drawing.Margins (0, this.v_scroller.Width - 1, this.row_height + this.title_height, this.h_scroller.Height - 1);
			
			Drawing.Rectangle bounds = this.Client.Bounds;
			
			bounds.Deflate (this.frame_margins);
			bounds.Deflate (this.table_margins);
			
			this.table_bounds = bounds;
			
			bounds.Deflate (this.inner_margins);
			
			this.inner_bounds = bounds;
		}
		
		protected virtual void UpdateVisibleRows()
		{
			double v = this.inner_bounds.Height / this.row_height;

			this.n_visible_rows       = (int) System.Math.Ceiling (v);	//	compte la derni�re ligne partielle
			this.n_fully_visible_rows = (int) System.Math.Floor (v);	//	nb de lignes enti�res
		}
		
		protected virtual void UpdateLayoutCache()
		{
			//	Alloue le tableau des textes :
			
			int dx = System.Math.Max (this.n_visible_rows, 1);
			int dy = System.Math.Max (this.max_columns, 1);
			
			if ((dx != this.cache_dx) ||
				(dy != this.cache_dy))
			{
				this.layouts = new TextLayout[dx, dy];
				this.cache_dx = dx;
				this.cache_dy = dy;
				this.cache_visible_rows = -1;
			}
		}
		
		protected virtual void UpdateHeaderGeometry()
		{
			//	Positionne l'en-t�te :
			
			Drawing.Rectangle rect = this.table_bounds;
			
			rect.Bottom = this.table_bounds.Top;
			rect.Top    = this.table_bounds.Top + this.row_height;
			
			this.header.Bounds = rect;
			this.header.SuspendLayout ();
			
			//	Place les boutons dans l'en-t�te :
			
			rect.Bottom = 0;
			rect.Top    = this.header.Height;
			rect.Left   = -this.offset;
			
			for (int i = 0; i < this.max_columns; i++)
			{
				HeaderButton button = this.FindButton (i);
				
				rect.Right = rect.Left + this.GetColumnWidth (i);
				
				button.Show ();
				button.Bounds = rect;
				
				rect.Left  = rect.Right;
			}
			
			//	Place les sliders dans l'en-t�te :
			
			rect.Bottom = 0;
			rect.Top    = this.header.Height;
			rect.Left   = -this.offset;
			
			for (int i = 0; i < this.max_columns; i++)
			{
				HeaderSlider      slider = this.FindSlider (i);
				Drawing.Rectangle bounds = rect;
				
				rect.Right   = rect.Left  + this.GetColumnWidth (i);
				bounds.Left  = rect.Right - this.slider_dim / 2;
				bounds.Right = rect.Right + this.slider_dim / 2;
				rect.Left    = rect.Right;
				
				slider.Show ();
				slider.Bounds = bounds;
			}
			
			if (this.is_header_dirty)
			{
				this.header.Children.Clear ();
				this.header.Children.AddRange (this.header_buttons);
				this.header.Children.AddRange (this.header_sliders);
			}
			
			this.header.ResumeLayout ();
			this.is_header_dirty = false;
		}
		
		protected virtual void UpdateScrollerGeometry()
		{
			//	Place l'ascenseur vertical :
			
			Drawing.Rectangle rect;
			
			rect       = this.table_bounds;
			rect.Left  = this.table_bounds.Right-1;
			rect.Right = this.table_bounds.Right-1 + this.v_scroller.Width;
			
			this.v_scroller.Bounds = rect;
			
			//	Place l'ascenseur horizontal :
			
			rect        = this.table_bounds;
			rect.Bottom = this.table_bounds.Bottom+1 - this.h_scroller.Height;
			rect.Top    = this.table_bounds.Bottom+1;
			
			this.h_scroller.Bounds = rect;
		}
		
		protected virtual void UpdateScrollers()
		{
			this.UpdateTagWidget ();
			
			//	Met � jour l'ascenseur vertical :
			
			int rows = this.VirtualRowCount;
			
			if ((rows <= this.n_fully_visible_rows) ||
				(rows <= 0) ||
				(this.n_fully_visible_rows <= 0))
			{
				this.v_scroller.SetEnabled (false);
				this.v_scroller.MaxValue          = 1;
				this.v_scroller.VisibleRangeRatio = 1;
				this.v_scroller.Value             = 0;
			}
			else
			{
				this.v_scroller.SetEnabled (true);
				this.v_scroller.MaxValue          = (decimal) (rows - this.n_fully_visible_rows);
				this.v_scroller.VisibleRangeRatio = (decimal) (this.n_fully_visible_rows / (double) rows);
				this.v_scroller.Value             = (decimal) (this.first_virtvis_row);
				this.v_scroller.SmallChange       = 1;
				this.v_scroller.LargeChange       = (decimal) (this.n_fully_visible_rows / 2);
			}
			
			this.UpdateTextLayouts ();
			
			//	Met � jour l'ascenseur horizontal :
			
			double width = this.total_width;

			if ((width <= this.table_bounds.Width) ||
				(width <= 0) ||
				(this.table_bounds.Width <= 0))
			{
				this.h_scroller.SetEnabled (false);
				this.h_scroller.MaxValue          = 1;
				this.h_scroller.VisibleRangeRatio = 1;
				this.h_scroller.Value             = 0;
			}
			else
			{
				this.h_scroller.SetEnabled (true);
				this.h_scroller.MaxValue          = (decimal) (width - this.table_bounds.Width);
				this.h_scroller.VisibleRangeRatio = (decimal) (this.table_bounds.Width / width);
				this.h_scroller.Value             = (decimal) (this.offset);
				this.h_scroller.SmallChange       = 10;
				this.h_scroller.LargeChange       = (decimal) (this.table_bounds.Width / 2);
			}
			
			this.UpdateScrollView ();
			this.Invalidate ();
		}
		
		protected virtual void UpdateTextLayouts()
		{
			if (this.column_widths == null)
			{
				return;
			}
			
			int  top     = this.FromVirtualRow (this.first_virtvis_row);
			int  bottom  = this.FromVirtualRow (this.first_virtvis_row + this.n_visible_rows);
			
			top    = System.Math.Min (top,    this.max_rows);
			bottom = System.Math.Min (bottom, this.max_rows);
			
			int  height  = bottom - top;
			int  max     = System.Math.Min (height, this.max_rows);
			bool refresh = (max != this.cache_visible_rows) || (this.first_virtvis_row != this.cache_first_virtvis_row);
			
			this.cache_visible_rows      = max;
			this.cache_first_virtvis_row = this.first_virtvis_row;
			
			for (int row = 0; row < max; row++)
			{
				for (int column = 0; column < this.max_columns; column++)
				{
					if (refresh)
					{
						if (this.layouts[row, column] == null)
						{
							this.layouts[row, column] = new TextLayout ();
						}
						
						this.layouts[row, column].Text     = this[row + top, column];
						this.layouts[row, column].Font     = this.DefaultFont;
						this.layouts[row, column].FontSize = this.DefaultFontSize;
					}
					
					this.layouts[row, column].LayoutSize = new Drawing.Size (this.column_widths[column] - this.text_margin * 2, this.row_height);
					this.layouts[row, column].Alignment  = this.column_alignments[column];
					this.layouts[row, column].BreakMode  = Drawing.TextBreakMode.Ellipsis | Drawing.TextBreakMode.SingleLine;
				}
			}
			if (max > -1)
			{
				for (int row = max; row < this.cache_dx; row++)
				{
					for (int column = 0; column < this.max_columns; column++)
					{
						this.layouts[row, column] = null;
					}
				}
			}
			
			this.OnLayoutUpdated ();
		}

		protected virtual void UpdateTotalWidth()
		{
			this.total_width = 0;
			
			for (int i = 0; i < this.max_columns; i++)
			{
				this.total_width += this.column_widths[i];
			}
		}
		
		protected virtual void UpdateHeaderContents()
		{
			foreach (HeaderButton button in this.header_buttons)
			{
				button.Clicked     -= new MessageEventHandler (this.HandleHeaderButtonClicked);
			}
			foreach (HeaderSlider slider in this.header_sliders)
			{
				slider.DragStarted -= new MessageEventHandler (this.HandleSliderDragStarted);
				slider.DragMoved   -= new MessageEventHandler (this.HandleSliderDragMoved);
				slider.DragEnded   -= new MessageEventHandler (this.HandleSliderDragEnded);
			}
			
			this.header_buttons.Clear ();
			this.header_sliders.Clear ();
			
			for (int i = 0; i < this.max_columns; i++)
			{
				HeaderButton button = new HeaderButton ();
				
				button.Style    = HeaderButtonStyle.Top;
				button.Dynamic  = true;
				button.Index    = i;
				button.Clicked += new MessageEventHandler (this.HandleHeaderButtonClicked);
				
				this.header_buttons.Add (button);
			}
			for (int i = 0; i < this.max_columns; i++)
			{
				HeaderSlider slider = new HeaderSlider ();
				
				slider.Style        = HeaderSliderStyle.Top;
				slider.Index        = i;
				slider.DragStarted += new MessageEventHandler (this.HandleSliderDragStarted);
				slider.DragMoved   += new MessageEventHandler (this.HandleSliderDragMoved);
				slider.DragEnded   += new MessageEventHandler (this.HandleSliderDragEnded);
				
				this.header_sliders.Add (slider);
			}
		}

		protected virtual void UpdateTitleWidget()
		{
			if (this.title_widget != null)
			{
				this.title_widget.Bounds = this.TitleBounds;
			}
		}
		
		protected virtual void UpdateTagWidget()
		{
			if (this.tag_widget != null)
			{
				Drawing.Rectangle bounds = this.GetRowBounds (this.SelectedIndex);
				
				bounds.Inflate (0, 0, 0, 1);
				
				if ((this.h_scroller.IsVisible) &&
					(bounds.Bottom < this.h_scroller.Top))
				{
					bounds.Bottom = this.h_scroller.Top;
				}
				
				if (bounds.IsSurfaceZero)
				{
					this.clip_widget.Hide ();
				}
				else
				{
					this.clip_widget.Bounds = bounds;
					this.clip_widget.Show ();
					
					double dx = System.Math.Min (this.row_height + 1, 18);
					double dy = dx;
					double ox = bounds.Right - (dx + 1);
					double oy = bounds.Top - dy;
					double x1, x2;
					
					if (this.GetUnclippedCellX (this.max_columns-1, out x1, out x2))
					{
						x2 -= dx - 1;
						ox  = System.Math.Min (ox, x2);
					}
					
					ox -= this.clip_widget.Bounds.X;
					oy -= this.clip_widget.Bounds.Y;
					
					this.tag_widget.Bounds = new Drawing.Rectangle (ox, oy, dx, dy);
				}
			}
		}
		
		
		protected override void OnAdornerChanged()
		{
			this.UpdateClientGeometry ();
			base.OnAdornerChanged ();
		}
		
		protected virtual  void OnSelectedIndexChanged()
		{
			if (this.SelectedIndexChanged != null)
			{
				this.SelectedIndexChanged (this);
			}
		}
		
		protected virtual  void OnEditionIndexChanged()
		{
			if (this.EditionIndexChanged != null)
			{
				this.EditionIndexChanged (this);
			}
		}
		
		protected virtual  void OnContentsChanged()
		{
			System.Diagnostics.Debug.WriteLine ("Contents changed.");
			
			if (this.ContentsChanged != null)
			{
				this.ContentsChanged (this);
			}
		}

		protected virtual  void OnSortChanged()
		{
			if (this.SortChanged != null)
			{
				this.SortChanged (this);
			}
		}

		protected virtual  void OnLayoutUpdated()
		{
			if (this.LayoutUpdated != null)
			{
				this.LayoutUpdated (this);
			}
		}
		
		
		protected HeaderButton FindButton(int index)
		{
			return this.header_buttons[index] as HeaderButton;
		}

		protected HeaderSlider FindSlider(int index)
		{
			return this.header_sliders[index] as HeaderSlider;
		}

		
		public int FindRow(string[] values)
		{
			int rows = this.RowCount;
			int cols = System.Math.Min (values.Length, this.max_columns);
			
			for (int i = 0; i < rows; i++)
			{
				bool match = true;
				
				for (int j = 0; j < cols; j++)
				{
					if (values[j] != null)
					{
						if (! this[i,j].StartsWith (values[j]))
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
		
		
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clip_rect)
		{
			base.PaintBackgroundImplementation (graphics, clip_rect);
			
			if (this.column_widths == null)
			{
				return;
			}
			
			IAdorner          adorner = Widgets.Adorner.Factory.Active;
			Drawing.Rectangle rect    = this.Client.Bounds;
			WidgetState       state   = this.PaintState;
			
			adorner.PaintArrayBackground (graphics, rect, state);
			
			Drawing.Rectangle local_clip = this.MapClientToRoot (this.table_bounds);
			Drawing.Rectangle save_clip  = graphics.SaveClippingRectangle ();
			Drawing.Rectangle table_clip = this.MapClientToRoot (this.inner_bounds);
			
			graphics.SetClippingRectangle (table_clip);
			
			//	Dessine le contenu du tableau, constitu� des textes :
			
			this.Update ();
			
			int           top    = this.FromVirtualRow (this.first_virtvis_row);						//	index de la ligne en haut
			int           delta  = this.first_virtvis_row - this.ToVirtualRow (top);					//	0 si compl�tement visible, n => d�borde n 'lignes'
			Drawing.Point pos    = new Drawing.Point (this.inner_bounds.Left, this.inner_bounds.Top);
			double        limit  = this.total_width - this.offset + this.inner_bounds.Left + 1;
			double        right  = System.Math.Min (this.inner_bounds.Right, limit);
			
			//	D�termine le nombre de lignes (virtuelles) actuellement affichables. Ceci est limit�
			//	par la place disponible et par le nombre total de lignes :
			
			int virt_top    = this.first_virtvis_row;
			int virt_bottom = virt_top + this.n_visible_rows;
			int virt_end    = this.ToVirtualRow (this.max_rows);
			
			int n_rows = System.Math.Min (virt_bottom, virt_end) - virt_top;
			
			//	Peint toutes les lignes (virtuelles) en sautant celles qui correspondent � la ligne
			//	r�elle en cours d'�dition :
			
			for (int row = 0; row < n_rows; row++)
			{
				pos.X  = this.inner_bounds.Left;
				pos.Y -= this.row_height;
				
				int         row_line      = row + top;
				int         num_add_lines = (this.edition_row == row_line)  ? this.edition_add_rows - delta : 0;
				WidgetState widget_state  = state & (WidgetState.Enabled | WidgetState.Focused);
				WidgetState text_state    = state & (WidgetState.Enabled);
				
				if ((this.selected_row == row_line) &&
					(this.edition_row < 0))
				{
					widget_state |= WidgetState.Selected;
					text_state   |= WidgetState.Selected;
				}
				
				if (this.edition_row == row_line)
				{
					pos.Y -= this.row_height * num_add_lines;
					
					adorner.PaintCellBackground (graphics, new Drawing.Rectangle (pos.X, pos.Y, right - pos.X, this.row_height * (num_add_lines + 1)), widget_state);
					n_rows -= num_add_lines;
				}
				else
				{
					adorner.PaintCellBackground (graphics, new Drawing.Rectangle (pos.X, pos.Y, right - pos.X, this.row_height), widget_state);
					
					pos.X += this.text_margin - System.Math.Floor (this.offset);
					
					for (int column = 0; column < this.max_columns; column++)
					{
						double end = pos.X + this.column_widths[column];
						
						if ((pos.X < local_clip.Right) &&
							(end > local_clip.Left))
						{
							TextLayout layout = this.layouts[row, column];
							
							if (layout != null)
							{
								adorner.PaintGeneralTextLayout (graphics, pos + new Drawing.Point (0, 0.5), layout, text_state, PaintTextStyle.Array, this.BackColor);
							}
						}
						
						pos.X = end;
					}
				}
			}
			
			graphics.RestoreClippingRectangle (local_clip);
			
			n_rows = System.Math.Min (virt_bottom, virt_end) - virt_top;
			
			rect = this.table_bounds;
			rect.Inflate (-0.5, -0.5);
			graphics.LineWidth = 1;
			
			Drawing.Color color = adorner.ColorTextFieldBorder ((state & WidgetState.Enabled) != 0);
			
			//	Dessine le rectangle englobant :
			
			graphics.AddRectangle (rect);
			graphics.RenderSolid (color);
			
			{
				//	Dessine les lignes de s�paration horizontales :
				
				double x1 = this.inner_bounds.Left;
				double x2 = right - 0.5;
				double y  = this.inner_bounds.Top - 0.5;
				
				graphics.AddLine (x1, y, x2, y);
				
				for (int i = 0; i < n_rows; i++)
				{
					int row_line = i + top;
					
					if (this.edition_row == row_line)
					{
						y      -= this.row_height * (this.edition_add_rows - delta);
						n_rows -= this.edition_add_rows;
					}
					
					y -= this.row_height;
					
					graphics.AddLine (x1, y, x2, y);
				}
			}
			{
				//	Dessine les lignes de s�paration verticales :
				
				limit = this.VirtualRowCount * this.row_height;
				limit = this.inner_bounds.Top - (limit - top * this.row_height);
				
				double y1 = System.Math.Max (this.inner_bounds.Bottom, limit);
				double y2 = this.inner_bounds.Top - 0.5;
				double x  = this.inner_bounds.Left - this.offset + 0.5;
				
				for (int i = 0; i < this.max_columns; i++)
				{
					x += this.GetColumnWidth (i);
					
					if ((x < this.inner_bounds.Left) ||
						(x > this.inner_bounds.Right))
					{
						continue;
					}
					
					graphics.AddLine (x, y1, x, y2);
				}
			}

			graphics.RenderSolid (color);
			graphics.RestoreClippingRectangle (save_clip);
		}

		protected override void PaintForegroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clip_rect)
		{
			base.PaintForegroundImplementation (graphics, clip_rect);
			
			IAdorner          adorner = Widgets.Adorner.Factory.Active;
			Drawing.Rectangle rect    = this.Client.Bounds;
			WidgetState       state   = this.PaintState;
			
			adorner.PaintArrayForeground (graphics, rect, state);
		}
		
		
		#region	IStringSelection Members
		public int								SelectedIndex
		{
			get
			{
				return this.selected_row;
			}
			set
			{
				if (value != -1)
				{
					value = System.Math.Max (value, 0);
					value = System.Math.Min (value, this.max_rows);
				}
				if (value != this.selected_row)
				{
					this.selected_row = value;
					this.InvalidateContents ();
					this.OnSelectedIndexChanged ();
				}
			}
		}
		
		public string							SelectedItem
		{
			get
			{
				int row = this.SelectedIndex;
				
				if (row == -1)
				{
					return null;
				}
				
				string[] rows = new string[this.max_columns];
				
				for (int i = 0; i < rows.Length; i++)
				{
					rows[i] = this[row, i];
				}
				
				return string.Join (this.separator.ToString (), rows);
			}
			set
			{
				this.SelectedIndex = this.FindRow (value.Split (this.separator));
			}
		}
		
		public event Support.EventHandler		SelectedIndexChanged;
		#endregion
		
		public event Support.EventHandler		EditionIndexChanged;
		public event Support.EventHandler		ContentsChanged;
		public event Support.EventHandler		SortChanged;
		public event Support.EventHandler		LayoutUpdated;
		
		
		protected bool							is_dirty;
		protected bool							is_header_dirty;
		protected bool							is_mouse_down;
		
		protected int							max_rows;
		protected int							max_columns;
		
		protected System.Collections.ArrayList	text_array			= new System.Collections.ArrayList ();
		protected TextProviderCallback			text_provider_callback;
		protected Support.Data.ITextArrayStore	text_array_store;
		protected TextLayout[,]					layouts;
		
		protected double						def_width			= 100;
		protected double						min_width			= 10;
		protected double						total_width;
		protected double[]						column_widths;
		protected Drawing.ContentAlignment[]	column_alignments;
		
		protected Drawing.Margins				frame_margins;				//	marges du cadre
		protected Drawing.Margins				table_margins;				//	marges de la table interne
		protected double						text_margin			= 2;
		protected double						row_height			= 16;
		protected double						title_height		= 0;
		protected Widget						title_widget;
		protected Drawing.Margins				inner_margins;
		protected double						slider_dim			= 6;
		
		protected Tag							tag_widget;
		protected Widget						clip_widget;
		
		protected Drawing.Rectangle				table_bounds;
		protected Drawing.Rectangle				inner_bounds;
		protected Widget						header;
		protected System.Collections.ArrayList	header_buttons		= new System.Collections.ArrayList ();
		protected System.Collections.ArrayList	header_sliders		= new System.Collections.ArrayList ();
		
		protected VScroller						v_scroller;
		protected HScroller						h_scroller;
		
		protected int							n_visible_rows;
		protected int							n_fully_visible_rows;
		protected int							first_virtvis_row;
		protected double						offset;
		protected int							selected_row		= -1;
		protected bool							is_select_enabled	= true;
		
		protected int							edition_row			= -1;
		protected int							edition_add_rows	= 0;
		
		protected int							drag_index;
		protected double						drag_pos;
		protected double						drag_dim;
		
		protected int							cache_dx;
		protected int							cache_dy;
		protected int							cache_visible_rows;
		protected int							cache_first_virtvis_row;
		protected char							separator = ';';
	}
}
