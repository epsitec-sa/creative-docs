namespace Epsitec.Common.Widgets
{
	public enum ScrollArrayShowMode
	{
		Extremity,		// déplacement minimal aux extrémités
		Center,			// déplacement central
	}

	public enum ScrollArrayAdjustMode
	{
		MoveUp,			// déplace le haut
		MoveDown,		// déplace le bas
	}

	public delegate string TextProviderCallback(int row, int column);

	/// <summary>
	///	La classe ScrollArray réalise une liste déroulante optimisée à deux dimensions,
	///	ne pouvant contenir que des textes fixes.
	/// </summary>
	public class ScrollArray : Widget
	{
		public ScrollArray()
		{
			this.InternalState |= InternalState.AutoFocus;
			this.InternalState |= InternalState.Focusable;
			
			IAdorner adorner = Widgets.Adorner.Factory.Active;
			
			this.frame_margins = adorner.GeometryArrayMargins;
			this.table_margins = new Drawing.Margins ();
			this.row_height    = this.DefaultFontHeight + 2;
			
			this.header = new Widget (this);
			this.v_scroller = new VScroller (this);
			this.h_scroller = new HScroller (this);
			this.v_scroller.IsInverted = true;
			this.v_scroller.ValueChanged += new EventHandler (this.HandleVScrollerChanged);
			this.h_scroller.ValueChanged += new EventHandler (this.HandleHScrollerChanged);
			
			this.is_dirty        = true;
			this.is_header_dirty = true;
		}

		public ScrollArray(Widget embedder) : this()
		{
			this.SetEmbedder (embedder);
		}

		
		public TextProviderCallback				TextProviderCallback
		{
			// Spécifie le délégué pour remplir les cellules.
			// En mode sans FillText, la liste est remplie à l'avance avec SetText.
			// Une copie de tous les strings est alors contenue dans this.array.
			// En mode FillText, c'est ScrollArray qui demande le contenu de chaque
			// cellule au fur et à mesure à l'aide du délégué FillText. Ce mode
			// est particulièrement efficace pour de grandes quantités de données.
			
			get
			{
				return this.text_provider_callback;
			}
			set
			{
				if (this.text_provider_callback != value)
				{
					this.text_provider_callback = value;
					this.Clear ();
				}
			}
		}

		public string							this[int row, int column]
		{
			get
			{
				if ((this.text_provider_callback != null) ||
					(this.column_widths == null) ||
					(row < 0) ||
					(column < 0) ||
					(row >= this.text_array.Count))
				{
					return "";
				}
				
				System.Collections.ArrayList line = this.text_array[row] as System.Collections.ArrayList;
				
				return (column >= line.Count) ? "" : line[column] as string;
			}
			set
			{
				if ((this.text_provider_callback != null) ||
					(this.column_widths == null) ||
					(row < 0) ||
					(column < 0))
				{
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
					this.UpdateHeader ();
					this.Update ();
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
					
					if (this.text_provider_callback == null)
					{
						//	Met à jour le nombre de lignes dans la table. Si la table est trop longue, on
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

		public int								FirstVisibleIndex
		{
			get
			{
				return this.first_visible_row;
			}
			set
			{
				int n = this.max_rows - this.n_fully_visible_rows;
				value = System.Math.Max (value, 0);
				value = System.Math.Min (value, System.Math.Max (n, 0));
				
				if (value != this.first_visible_row)
				{
					this.first_visible_row = value;
					this.UpdateScrollers ();
				}
			}
		}

		public double							Offset
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
				
				if ((this.selected_row >= this.first_visible_row) &&
					(this.selected_row < this.first_visible_row + this.n_fully_visible_rows))
				{
					return true;
				}
				
				return false;
			}
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
		
		
		public void Clear()
		{
			//	Purge le contenu de la table, pour autant que l'on soit en mode FillText.
			
			if (this.text_provider_callback == null)
			{
				this.text_array.Clear ();
			}
			
			this.max_rows = 0;
			this.first_visible_row = 0;
			this.selected_row = -1;
			this.InvalidateContents ();
		}
		
		public void RefreshContents()
		{
			this.cache_visible_rows = -1;
			this.InvalidateContents ();
		}

		public void InvalidateContents()
		{
			this.is_dirty = true;
			this.Invalidate ();
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

		
		public void ShowSelected(ScrollArrayShowMode mode)
		{
			if (this.selected_row == -1)
			{
				return;
			}
			
			int first  = this.first_visible_row;
			int num    = System.Math.Min (this.n_fully_visible_rows, this.max_rows);
			int height = 1;
			
			switch (mode)
			{
				case ScrollArrayShowMode.Extremity:
					if (this.selected_row < this.first_visible_row)
					{
						//	La ligne était en-dessus du sommet de la liste :
						
						first = this.selected_row;
					}
					
					if (this.selected_row > this.first_visible_row + this.n_fully_visible_rows - height)
					{
						//	La ligne était en-dessous du bas de la liste :
						
						first = this.selected_row - (this.n_fully_visible_rows - height);
					}
					break;
				
				case ScrollArrayShowMode.Center:
					first = System.Math.Min (this.selected_row + num / 2, this.max_rows - 1);
					first = System.Math.Max (first - num + 1, 0);
					break;
			}

			this.first_visible_row = first;
		}

		
		public bool AdjustToMultiple(ScrollArrayAdjustMode mode)
		{
			//	Ajuste la hauteur pour afficher pile un nombre entier de lignes.
			
			this.Update ();
			
			double height = this.table_bounds.Height;
			int    num    = (int) (height / this.row_height);
			double adjust = height - num * this.row_height;
			
			if (adjust == 0)
			{
				return false;
			}
			
			switch (mode)
			{
				case ScrollArrayAdjustMode.MoveUp:
					this.Top    = System.Math.Floor (this.Top - adjust);
					break;
				
				case ScrollArrayAdjustMode.MoveDown:
					this.Bottom = System.Math.Floor (this.Bottom + adjust);
					break;
			}

			this.Invalidate ();
			
			return true;
		}

		public bool AdjustToContents(ScrollArrayAdjustMode mode, double min_height, double max_height)
		{
			//	Ajuste la hauteur pour afficher exactement le nombre de lignes contenues.
			
			this.Update ();
			
			double height = this.row_height * this.max_rows + this.frame_margins.Height + this.table_margins.Height;
			double desire = height;
			
			height = System.Math.Max (height, min_height);
			height = System.Math.Min (height, max_height);
			
			if (height == this.Height)
			{
				return false;
			}
			
			switch (mode)
			{
				case ScrollArrayAdjustMode.MoveUp:
					this.Top    = this.Bottom + height;
					break;
				case ScrollArrayAdjustMode.MoveDown:
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

		
		private void HandleVScrollerChanged(object sender)
		{
			this.FirstVisibleIndex = (int) System.Math.Floor (this.v_scroller.Value + 0.5);
		}

		private void HandleHScrollerChanged(object sender)
		{
			this.Offset = System.Math.Floor (this.h_scroller.Value);
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
				case MessageType.MouseDown :
					this.is_mouse_down = true;
					this.ProcessMouseSelect (pos.Y);
					break;

				case MessageType.MouseMove :
					if (this.is_mouse_down)
					{
						this.ProcessMouseSelect (pos.Y);
					}

					break;

				case MessageType.MouseUp :
					if (this.is_mouse_down)
					{
						this.ProcessMouseSelect (pos.Y);
						this.is_mouse_down = false;
					}

					break;

				case MessageType.KeyDown :
					this.ProcessKeyDown (message.KeyCode, message.IsShiftPressed, message.IsCtrlPressed);
					break;
			}
			
			message.Consumer = this;
		}

		protected virtual  void ProcessMouseSelect(double pos)
		{
			pos = this.Client.Height - pos;
			
			int line = (int) ((pos - this.frame_margins.Top - this.table_margins.Top) / this.row_height);
			
			if ((line < 0) ||
				(line >= this.n_visible_rows))
			{
				return;
			}
			
			line += this.first_visible_row;
			
			if (line < this.max_rows)
			{
				this.SelectedIndex = line;
			}
		}

		protected virtual  void ProcessKeyDown(KeyCode key, bool is_shift_pressed, bool is_ctrl_pressed)
		{
			int sel;

			switch (key)
			{
				case KeyCode.ArrowUp :
					sel = this.SelectedIndex - 1;
					
					if (sel >= 0)
					{
						this.SelectedIndex = sel;
						
						if (!this.IsSelectedVisible)
						{
							this.ShowSelected (ScrollArrayShowMode.Extremity);
						}
					}
					break;
				
				case KeyCode.ArrowDown :
					sel = this.SelectedIndex + 1;
					
					if (sel < this.max_rows)
					{
						this.SelectedIndex = sel;
						if (!this.IsSelectedVisible)
						{
							this.ShowSelected (ScrollArrayShowMode.Extremity);
						}
					}
					break;
			}
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
			
			this.is_dirty = false;
			
			IAdorner adorner = Widgets.Adorner.Factory.Active;

			this.row_height    = this.DefaultFontHeight + 2;
			this.frame_margins = adorner.GeometryArrayMargins;
			this.table_margins = new Drawing.Margins (0, this.v_scroller.Width - 1, this.row_height, this.h_scroller.Height - 1);
			this.table_bounds  = this.Client.Bounds;
			
			this.table_bounds.Deflate (this.frame_margins);
			this.table_bounds.Deflate (this.table_margins);
			
			double v = this.table_bounds.Height / this.row_height;

			this.n_visible_rows       = (int) System.Math.Ceiling (v);	//	compte la dernière ligne partielle
			this.n_fully_visible_rows = (int) System.Math.Floor (v);	//	nb de lignes entières
			
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
			
			//	Positionne l'en-tête :
			
			Drawing.Rectangle rect = this.table_bounds;
			
			rect.Bottom = this.table_bounds.Top;
			rect.Top    = this.table_bounds.Top + this.row_height;
			
			this.header.Bounds = rect;
			this.header.SuspendLayout ();
			
			//	Place les boutons dans l'en-tête :
			
			rect.Bottom = 0;
			rect.Top    = this.table_margins.Top;
			rect.Left   = -this.offset;
			
			for (int i = 0; i < this.max_columns; i++)
			{
				HeaderButton button = this.FindButton (i);
				
				rect.Right = rect.Left + this.GetColumnWidth (i);
				
				button.Show ();
				button.Bounds = rect;
				
				rect.Left  = rect.Right;
			}
			
			//	Place les sliders dans l'en-tête :
			
			rect.Bottom = 0;
			rect.Top    = this.table_margins.Top;
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
			
			//	Place l'ascenseur vertical :
			
			rect.Left   = this.table_bounds.Right-1;
			rect.Right  = this.table_bounds.Right-1 + this.v_scroller.Width;
			rect.Bottom = this.table_bounds.Bottom;
			rect.Top    = this.table_bounds.Top;
			
			this.v_scroller.Bounds = rect;
			
			//	Place l'ascenseur horizontal :
			
			rect.Left   = this.table_bounds.Left;
			rect.Right  = this.table_bounds.Right;
			rect.Bottom = this.table_bounds.Bottom+1 - this.h_scroller.Height;
			rect.Top    = this.table_bounds.Bottom+1;
			
			this.h_scroller.Bounds = rect;
			
			this.is_header_dirty = false;
			this.UpdateScrollers ();
		}
		
		
		public override Drawing.Rectangle GetShapeBounds()
		{
			IAdorner          adorner = Widgets.Adorner.Factory.Active;
			Drawing.Rectangle rect    = this.Client.Bounds;
			
			rect.Inflate (adorner.GeometryListShapeBounds);
			
			return rect;
		}
		
		
		protected void Update()
		{
			if (this.is_dirty)
			{
				this.UpdateClientGeometry ();
			}
		}
		
		protected void UpdateScrollers()
		{
			//	Met à jour l'ascenseur vertical :
			
			int rows = this.max_rows;
			
			if ((rows <= this.n_fully_visible_rows) ||
				(rows <= 0) ||
				(this.n_fully_visible_rows <= 0))
			{
				this.v_scroller.SetEnabled (false);
				this.v_scroller.Range             = 1;
				this.v_scroller.VisibleRangeRatio = 1;
				this.v_scroller.Value             = 0;
			}
			else
			{
				this.v_scroller.SetEnabled (true);
				this.v_scroller.Range             = rows - this.n_fully_visible_rows;
				this.v_scroller.VisibleRangeRatio = this.n_fully_visible_rows / (double) rows;
				this.v_scroller.Value             = this.first_visible_row;
				this.v_scroller.SmallChange       = 1;
				this.v_scroller.LargeChange       = this.n_fully_visible_rows / 2.0;
			}
			
			this.UpdateTextLayouts ();
			
			//	Met à jour l'ascenseur horizontal :
			
			double width = this.total_width;

			if ((width <= this.table_bounds.Width) ||
				(width <= 0) ||
				(this.table_bounds.Width <= 0))
			{
				this.h_scroller.SetEnabled (false);
				this.h_scroller.Range             = 1;
				this.h_scroller.VisibleRangeRatio = 1;
				this.h_scroller.Value             = 0;
			}
			else
			{
				this.h_scroller.SetEnabled (true);
				this.h_scroller.Range             = width - this.table_bounds.Width;
				this.h_scroller.VisibleRangeRatio = this.table_bounds.Width / width;
				this.h_scroller.Value             = this.offset;
				this.h_scroller.SmallChange       = 10;
				this.h_scroller.LargeChange       = this.table_bounds.Width / 2.0;
			}
			
			this.Invalidate ();
		}
		
		protected void UpdateTextLayouts()
		{
			if (this.column_widths == null)
			{
				return;
			}
			
			int  max     = System.Math.Min (this.n_visible_rows, this.max_rows);
			bool refresh = (max != this.cache_visible_rows) || (this.first_visible_row != this.cache_first_visible_row);

			this.cache_visible_rows      = max;
			this.cache_first_visible_row = this.first_visible_row;
			
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
						
						if (this.text_provider_callback == null)
						{
							this.layouts[row, column].Text = this[row + this.first_visible_row, column];
						}
						else
						{
							this.layouts[row, column].Text = this.text_provider_callback (row + this.first_visible_row, column);
						}
						
						this.layouts[row, column].Font     = this.DefaultFont;
						this.layouts[row, column].FontSize = this.DefaultFontSize;
					}
					
					this.layouts[row, column].LayoutSize = new Drawing.Size (this.column_widths[column] - this.text_margin * 2, this.row_height);
					this.layouts[row, column].Alignment  = this.column_alignments[column];
					this.layouts[row, column].BreakMode  = Drawing.TextBreakMode.Ellipsis | Drawing.TextBreakMode.SingleLine;
				}
			}
		}

		protected void UpdateTotalWidth()
		{
			this.total_width = 0;
			
			for (int i = 0; i < this.max_columns; i++)
			{
				this.total_width += this.column_widths[i];
			}
		}
		
		protected void UpdateHeader()
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
		
		protected virtual  void OnContentsChanged()
		{
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

		
		protected HeaderButton FindButton(int index)
		{
			return this.header_buttons[index] as HeaderButton;
		}

		protected HeaderSlider FindSlider(int index)
		{
			return this.header_sliders[index] as HeaderSlider;
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
			
			graphics.SetClippingRectangle (local_clip);
			
			//	Dessine le contenu du tableau, constitué des textes :
			
			this.Update ();
			
			Drawing.Point pos    = new Drawing.Point (this.table_bounds.Left, this.table_bounds.Top - this.row_height);
			double        limit  = this.total_width - this.offset + this.table_bounds.Left + 1;
			double        right  = System.Math.Min (this.table_bounds.Right, limit);
			int           n_rows = System.Math.Min (this.n_visible_rows, this.max_rows);
			
			for (int row = 0; row < n_rows; row++)
			{
				pos.X = this.table_bounds.Left;
				
				int         row_line     = this.first_visible_row + row;
				WidgetState widget_state = (this.selected_row == row_line) ? WidgetState.Selected : WidgetState.Enabled;
				
				adorner.PaintCellBackground (graphics, new Drawing.Rectangle (pos.X, pos.Y, right - pos.X, this.row_height), widget_state);
				
				pos.X += this.text_margin - System.Math.Floor (this.offset);
				
				for (int column = 0; column < this.max_columns; column++)
				{
					double end = pos.X + this.column_widths[column];
					
					if ((pos.X < local_clip.Right) &&
						(end > local_clip.Left))
					{
						adorner.PaintGeneralTextLayout (graphics, pos, this.layouts[row, column], widget_state, PaintTextStyle.Array, this.BackColor);
					}
					
					pos.X = end;
				}
				
				pos.Y -= this.row_height;
			}
			
			rect = this.table_bounds;
			rect.Inflate (-0.5, -0.5);
			graphics.LineWidth = 1;
			
			Drawing.Color color = adorner.ColorTextFieldBorder ((state & WidgetState.Enabled) != 0);
			
			//	Dessine le rectangle englobant :
			
			graphics.AddRectangle (rect);
			graphics.RenderSolid (color);
			
			{
				//	Dessine les lignes de séparation horizontales :
				
				double x1 = this.table_bounds.Left;
				double x2 = right;
				double y  = this.table_bounds.Top - 0.5;
				
				for (int i = 0; i < n_rows; i++)
				{
					y -= this.row_height;
					graphics.AddLine (x1, y, x2, y);
					graphics.RenderSolid (color);
				}
			}
			{
				//	Dessine les lignes de séparation verticales :
				
				limit = this.max_rows * this.row_height;
				limit = this.table_bounds.Top - (limit - this.first_visible_row * this.row_height);
				
				double y1 = System.Math.Max (this.table_bounds.Bottom, limit);
				double y2 = this.table_bounds.Top;
				double x  = this.table_bounds.Left - this.offset + 0.5;
				
				for (int i = 0; i < this.max_columns; i++)
				{
					x += this.GetColumnWidth (i);
					
					if ((x < this.table_bounds.Left) ||
						(x > this.table_bounds.Right))
					{
						continue;
					}
					
					graphics.AddLine (x, y1, x, y2);
					graphics.RenderSolid (color);
				}
			}

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
		
		
		public event EventHandler				SelectedIndexChanged;
		public event EventHandler				ContentsChanged;
		public event EventHandler				SortChanged;
		
		
		protected bool							is_dirty;
		protected bool							is_header_dirty;
		protected bool							is_mouse_down = false;
		protected int							max_rows = 0;
		protected int							max_columns = 0;
		protected System.Collections.ArrayList	text_array = new System.Collections.ArrayList ();
		protected TextProviderCallback			text_provider_callback = null;
		protected TextLayout[,]					layouts;
		protected double						def_width = 100;		//	largeur par défaut
		protected double						min_width = 10;			//	largeur minimale
		protected double[]						column_widths;			//	largeur des colonnes
		protected double						total_width;			//	largeur totale
		protected Drawing.ContentAlignment[]	column_alignments;
		
		protected Drawing.Margins				frame_margins;			//	marges du cadre
		protected Drawing.Margins				table_margins;			//	marges de la table interne
		protected double						text_margin = 2;
		protected double						row_height = 16;
		protected double						slider_dim = 6;
		
		private Drawing.Rectangle				table_bounds = new Drawing.Rectangle ();
		protected Widget						header;		// père de l'en-tête horizontale
		protected System.Collections.ArrayList	header_buttons = new System.Collections.ArrayList ();
		protected System.Collections.ArrayList	header_sliders = new System.Collections.ArrayList ();
		protected VScroller						v_scroller;
		protected HScroller						h_scroller;
		protected int							n_visible_rows;
		protected int							n_fully_visible_rows;
		protected int							first_visible_row = 0;
		protected int							selected_row = -1;
		protected double						offset = 0;
		
		protected int							drag_index;
		protected double						drag_pos;
		protected double						drag_dim;
		
		protected int							cache_dx;
		protected int							cache_dy;
		protected int							cache_visible_rows;
		protected int							cache_first_visible_row;
	}
}
