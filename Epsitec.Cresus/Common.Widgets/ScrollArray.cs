namespace Epsitec.Common.Widgets
{
	public enum ScrollArrayShow
	{
		Extremity,		// déplacement minimal aux extrémités
		Center,			// déplacement central
	}

	public enum ScrollArrayAdjust
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
				return this.text_provider;
			}
			set
			{
				if (this.text_provider != value)
				{
					this.text_provider = value;
					this.Clear ();
				}
			}
		}

		
		public void Clear()
		{
			//	Purge le contenu de la table, pour autant que l'on soit en mode FillText.
			
			if (this.text_provider == null)
			{
				this.text_array.Clear ();
			}
			
			this.max_rows = 0;
			this.first_visible_row = 0;
			this.selected_row = -1;
			this.InvalidateContents ();
		}
		
		public void InvalidateContents()
		{
			this.is_dirty = true;
			this.Invalidate ();
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
					
					if (this.text_provider == null)
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

		
		public string							this[int row, int column]
		{
			get
			{
				if ((this.text_provider != null) ||
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
				if (this.text_provider != null)  return;
				if (this.column_widths == null)  return;
				if (row < 0 || column < 0)  return;
				
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
				button.Clicked     -= new MessageEventHandler (this.HandleButtonClicked);
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
				button.Clicked += new MessageEventHandler (this.HandleButtonClicked);
				
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

			for (int i = 0; i < this.max_columns; i++)
			{
				this.FindButton (i).SortMode = i == column ? mode : SortMode.None;
			}
		}

		// Retourne la colonne de tri.
		public bool GetSortingHeader(out int column, out SortMode mode)
		{
			for (column = 0; column < this.max_columns; column++)
			{
				HeaderButton button = this.FindButton (column);

				mode = button.SortMode;
				if (mode != 0)  return true;
			}

			column = -1;
			mode = 0;
			return false;
		}

		// Indique si la ligne sélectionnée est visible.
		public bool IsShowSelect()
		{
			if (this.selected_row == -1)  return true;

			if (this.selected_row >= this.first_visible_row && this.selected_row < this.first_visible_row + this.n_fully_visible_rows)  return true;

			return false;
		}

		// Rend la ligne sélectionnée visible.
		public void ShowSelect(ScrollArrayShow mode)
		{
			if (this.selected_row == -1)  return;

			int fl = this.first_visible_row;

			if (mode == ScrollArrayShow.Extremity)
			{
				if (this.selected_row < this.first_visible_row)
				{
					fl = this.selected_row;
				}

				if (this.selected_row > this.first_visible_row + this.n_fully_visible_rows - 1)
				{
					fl = this.selected_row - (this.n_fully_visible_rows - 1);
				}
			}

			if (mode == ScrollArrayShow.Center)
			{
				int display = System.Math.Min (this.n_fully_visible_rows, this.max_rows);

				fl = System.Math.Min (this.selected_row + display / 2, this.max_rows - 1);
				fl = System.Math.Max (fl - display + 1, 0);
			}

			this.first_visible_row = fl;
		}

		// Ajuste la hauteur pour afficher pile un nombre entier de lignes.
		public bool AdjustToMultiple(ScrollArrayAdjust mode)
		{
			this.Update ();

			double h = this.table_bounds.Height;
			int nbLines = (int) (h / this.row_height);
			double adjust = h - nbLines * this.row_height;

			if (adjust == 0)  return false;

			if (mode == ScrollArrayAdjust.MoveUp)
			{
				this.Top = System.Math.Floor (this.Top - adjust);
			}

			if (mode == ScrollArrayAdjust.MoveDown)
			{
				this.Bottom = System.Math.Floor (this.Bottom + adjust);
			}

			this.Invalidate ();
			return true;
		}

		// Ajuste la hauteur pour afficher exactement le nombre de lignes contenues.
		public bool AdjustToContent(ScrollArrayAdjust mode, double hMin, double hMax)
		{
			this.Update ();

			double h = this.row_height * this.max_rows + this.frame_margins.Height + this.table_margins.Height;
			double hope = h;

			h = System.Math.Max (h, hMin);
			h = System.Math.Min (h, hMax);
			if (h == this.Height)  return false;

			if (mode == ScrollArrayAdjust.MoveUp)
			{
				this.Top = this.Bottom + h;
			}

			if (mode == ScrollArrayAdjust.MoveDown)
			{
				this.Bottom = this.Top - h;
			}

			this.Invalidate ();
			if (h != hope)  AdjustToMultiple (mode);

			return true;
		}

		// Appelé lorsque l'ascenseur a bougé.
		private void HandleVScrollerChanged(object sender)
		{
			this.FirstVisibleIndex = (int) System.Math.Floor (this.v_scroller.Value + 0.5);
			//this.SetFocused(true);
		}

		// Appelé lorsque l'ascenseur a bougé.
		private void HandleHScrollerChanged(object sender)
		{
			this.Offset = System.Math.Floor (this.h_scroller.Value);
			//this.SetFocused(true);
		}

		// Appelé lorsque le bouton d'en-tête a été cliqué.
		private void HandleButtonClicked(object sender, MessageEventArgs e)
		{
			foreach (HeaderButton button in this.header_buttons)
			{
				if (sender == button)
				{
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
					this.OnSortChanged ();
					return;
				}
			}
		}

		// Appelé lorsque le slider va être déplacé.
		private void HandleSliderDragStarted(object sender, MessageEventArgs e)
		{
			HeaderSlider slider = sender as HeaderSlider;

			this.DragStartedColumn (slider.Index, e.Point.X);
		}

		// Appelé lorsque le slider est déplacé.
		private void HandleSliderDragMoved(object sender, MessageEventArgs e)
		{
			HeaderSlider slider = sender as HeaderSlider;

			this.DragMovedColumn (slider.Index, e.Point.X);
		}

		// Appelé lorsque le slider est fini de déplacer.
		private void HandleSliderDragEnded(object sender, MessageEventArgs e)
		{
			HeaderSlider slider = sender as HeaderSlider;

			this.DragEndedColumn (slider.Index, e.Point.X);
		}

		// La largeur d'une colonne va être modifiée.
		protected void DragStartedColumn(int column, double pos)
		{
			this.dragRank = column;
			this.dragPos = pos;
			this.dragDim = this.GetColumnWidth (column);
		}

		// Modifie la largeur d'une colonne.
		protected void DragMovedColumn(int column, double pos)
		{
			double newWidth = this.GetColumnWidth (column) + pos - this.dragPos;

			this.SetColumnWidth (this.dragRank, newWidth);
			this.InvalidateContents ();
		}

		// La largeur d'une colonne a été modifiée.
		protected void DragEndedColumn(int column, double pos)
		{
			this.InvalidateContents ();
		}

		// Gestion d'un événement.
		protected override void ProcessMessage(Message message, Drawing.Point pos)
		{
			switch (message.Type)
			{
				case MessageType.MouseDown :
					this.is_mouse_down = true;
					this.MouseSelect (pos.Y);
					break;

				case MessageType.MouseMove :
					if (this.is_mouse_down)
					{
						this.MouseSelect (pos.Y);
					}

					break;

				case MessageType.MouseUp :
					if (this.is_mouse_down)
					{
						this.MouseSelect (pos.Y);
						this.is_mouse_down = false;
					}

					break;

				case MessageType.KeyDown :
					ProcessKeyDown (message.KeyCode, message.IsShiftPressed, message.IsCtrlPressed);
					break;
			}
			message.Consumer = this;
		}

		// Sélectionne la ligne selon la souris.
		protected bool MouseSelect(double pos)
		{
			pos = this.Client.Height - pos;

			int line = (int) ((pos - this.frame_margins.Top - this.table_margins.Top) / this.row_height);

			if (line < 0 || line >= this.n_visible_rows)  return false;

			line += this.first_visible_row;
			if (line > this.max_rows - 1)  return false;

			this.SelectedIndex = line;
			return true;
		}

		// Gestion d'une touche pressée avec KeyDown dans la liste.
		protected void ProcessKeyDown(KeyCode key, bool isShiftPressed, bool isCtrlPressed)
		{
			int sel;

			switch (key)
			{
				case KeyCode.ArrowUp :
					sel = this.SelectedIndex - 1;
					if (sel >= 0)
					{
						this.SelectedIndex = sel;
						if (!this.IsShowSelect ())  this.ShowSelect (ScrollArrayShow.Extremity);
					}

					break;

				case KeyCode.ArrowDown :
					sel = this.SelectedIndex + 1;
					if (sel < this.max_rows)
					{
						this.SelectedIndex = sel;
						if (!this.IsShowSelect ())  this.ShowSelect (ScrollArrayShow.Extremity);
					}

					break;
			}
		}

		// Demande de régénérer tout le contenu.
		public void RefreshContent()
		{
			this.lastVisibleRows = -1;
			this.InvalidateContents ();
		}

		// Met à jour la géométrie du tableau.
		protected void Update()
		{
			if (this.column_widths == null)  return;

			if (!this.is_dirty)  return;

			this.UpdateClientGeometry ();
		}

		// Met à jour la géométrie de la liste.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry ();
			
			
			if (this.column_widths == null)  return;

			if (this.v_scroller == null || this.h_scroller == null)  return;

			this.is_dirty = false;

			IAdorner adorner = Widgets.Adorner.Factory.Active;

			this.row_height    = this.DefaultFontHeight + 2;
			this.frame_margins = adorner.GeometryArrayMargins;
			this.table_margins = new Drawing.Margins (0, this.v_scroller.Width - 1, this.row_height, this.h_scroller.Height - 1);
			
			this.table_bounds = this.Client.Bounds;
			this.table_bounds.Deflate (this.frame_margins);
			this.table_bounds.Deflate (this.table_margins);
			
			double v = this.table_bounds.Height / this.row_height;

			this.n_visible_rows = (int) System.Math.Ceiling (v);  // compte la dernière ligne partielle
			this.n_fully_visible_rows = (int) System.Math.Floor (v);  // nb de lignes entières

			// Alloue le tableau des textes.
			int dx = System.Math.Max (this.n_visible_rows, 1);
			int dy = System.Math.Max (this.max_columns, 1);

			if (dx != this.lastDx || dy != this.lastDy)
			{
				this.layouts = new TextLayout[dx, dy];
				this.lastDx  = dx;
				this.lastDy  = dy;
				this.lastVisibleRows = -1;
			}

			// Place l'en-tête
			Drawing.Rectangle aRect = new Drawing.Rectangle ();

			aRect.Left   = this.table_bounds.Left;
			aRect.Right  = this.table_bounds.Right;
			aRect.Bottom = this.table_bounds.Top;
			aRect.Top    = this.table_bounds.Top + this.row_height;
			this.header.Bounds = aRect;
			if (this.is_header_dirty)  this.header.Children.Clear ();

			// Place les boutons dans l'en-tête.
			aRect.Bottom = 0;
			aRect.Top    = this.table_margins.Top;
			aRect.Left   = -this.offset;
			for (int i = 0; i < this.max_columns; i++)
			{
				aRect.Right = aRect.Left + this.GetColumnWidth (i);

				HeaderButton button = this.FindButton (i);

				button.Show ();
				button.Bounds = aRect;
				if (this.is_header_dirty)  this.header.Children.Add (button);

				aRect.Left = aRect.Right;
			}

			// Place les sliders dans l'en-tête.
			aRect.Bottom = 0;
			aRect.Top = this.table_margins.Top;
			aRect.Left = -this.offset;
			for (int i = 0; i < this.max_columns; i++)
			{
				aRect.Right = aRect.Left + this.GetColumnWidth (i);

				HeaderSlider slider = this.FindSlider (i);
				Drawing.Rectangle sRect = new Drawing.Rectangle ();

				sRect.Left   = aRect.Right - this.slider_dim / 2;
				sRect.Right  = aRect.Right + this.slider_dim / 2;
				sRect.Bottom = aRect.Bottom;
				sRect.Top    = aRect.Top;
				slider.Show ();
				slider.Bounds = sRect;
				if (this.is_header_dirty)  this.header.Children.Add (slider);

				aRect.Left = aRect.Right;
			}

			// Place l'ascenseur vertical.
			aRect.Left   = this.table_bounds.Right-1;
			aRect.Right  = this.table_bounds.Right-1 + this.v_scroller.Width;
			aRect.Bottom = this.table_bounds.Bottom;
			aRect.Top    = this.table_bounds.Top;
			this.v_scroller.Bounds = aRect;

			// Place l'ascenseur horizontal.
			aRect.Left   = this.table_bounds.Left;
			aRect.Right  = this.table_bounds.Right;
			aRect.Bottom = this.table_bounds.Bottom+1 - this.h_scroller.Height;
			aRect.Top    = this.table_bounds.Bottom+1;
			this.h_scroller.Bounds = aRect;
			this.is_header_dirty = false;
			this.UpdateScrollers ();
		}

		protected override void OnAdornerChanged()
		{
			this.UpdateClientGeometry ();
			base.OnAdornerChanged ();
		}

		public override Drawing.Rectangle GetShapeBounds()
		{
			IAdorner adorner = Widgets.Adorner.Factory.Active;
			Drawing.Rectangle rect = new Drawing.Rectangle (0, 0, this.Client.Width, this.Client.Height);

			rect.Inflate (adorner.GeometryListShapeBounds);
			return rect;
		}

		// Met à jour l'ascenseur en fonction de la liste.
		protected void UpdateScrollers()
		{
			// Met à jour l'ascenseur vertical.
			int nbRows = this.max_rows;

			if (nbRows <= this.n_fully_visible_rows || nbRows <= 0 || this.n_fully_visible_rows <= 0)
			{
				this.v_scroller.SetEnabled (false);
				this.v_scroller.Range = 1;
				this.v_scroller.VisibleRangeRatio = 1;
				this.v_scroller.Value = 0;
			}
			else
			{
				this.v_scroller.SetEnabled (true);
				this.v_scroller.Range = nbRows - this.n_fully_visible_rows;
				this.v_scroller.VisibleRangeRatio = this.n_fully_visible_rows / (double) nbRows;
				this.v_scroller.Value = this.first_visible_row;
				this.v_scroller.SmallChange = 1;
				this.v_scroller.LargeChange = this.n_fully_visible_rows / 2.0;
			}

			this.UpdateTextlayouts ();

			// Met à jour l'ascenseur horizontal.
			double width = this.total_width;

			if (width <= this.table_bounds.Width || width <= 0 || this.table_bounds.Width <= 0)
			{
				this.h_scroller.SetEnabled (false);
				this.h_scroller.Range = 1;
				this.h_scroller.VisibleRangeRatio = 1;
				this.h_scroller.Value = 0;
			}
			else
			{
				this.h_scroller.SetEnabled (true);
				this.h_scroller.Range = width - this.table_bounds.Width;
				this.h_scroller.VisibleRangeRatio = this.table_bounds.Width / width;
				this.h_scroller.Value = this.offset;
				this.h_scroller.SmallChange = 10;
				this.h_scroller.LargeChange = this.table_bounds.Width / 2.0;
			}
			
			this.Invalidate ();
		}

		// Met à jour les textes en fonction de l'ascenseur vertical.
		protected void UpdateTextlayouts()
		{
			if (this.column_widths == null)  return;

			int max = System.Math.Min (this.n_visible_rows, this.max_rows);
			bool quick = (max == this.lastVisibleRows && this.first_visible_row == this.lastFirstRow);

			this.lastVisibleRows = max;
			this.lastFirstRow = this.first_visible_row;
			for (int row = 0; row < max; row++)
			{
				for (int column = 0; column < this.max_columns; column++)
				{
					if (!quick)
					{
						if (this.layouts[row, column] == null)
						{
							this.layouts[row, column] = new TextLayout ();
						}

						if (this.text_provider == null)
						{
							this.layouts[row, column].Text = this[row + this.first_visible_row, column];
						}
						else
						{
							this.layouts[row, column].Text = this.text_provider (row + this.first_visible_row, column);
						}

						this.layouts[row, column].Font = this.DefaultFont;
						this.layouts[row, column].FontSize = this.DefaultFontSize;
					}

					this.layouts[row, column].LayoutSize = new Drawing.Size (this.column_widths[column] - this.text_margin * 2, this.row_height);
					this.layouts[row, column].Alignment = this.column_alignments[column];
					this.layouts[row, column].BreakMode = Drawing.TextBreakMode.Ellipsis | Drawing.TextBreakMode.SingleLine;
				}
			}
		}

		// Génère un événement pour dire que la sélection dans la liste a changé.
		protected virtual void OnSelectedIndexChanged()
		{
			if (this.SelectedIndexChanged != null)  // qq'un écoute ?
			{
				this.SelectedIndexChanged (this);
			}
		}
		
		protected virtual void OnContentsChanged()
		{
			if (this.ContentsChanged != null)  // qq'un écoute ?
			{
				this.ContentsChanged (this);
			}
		}

		// Génère un événement pour dire que le tri a changé.
		protected void OnSortChanged()
		{
			if (this.SortChanged != null)  // qq'un écoute ?
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

		// Dessine le tableau.
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			IAdorner adorner = Widgets.Adorner.Factory.Active;

			if (this.column_widths == null)  return;

			Drawing.Rectangle rect = new Drawing.Rectangle (0, 0, this.Client.Width, this.Client.Height);
			WidgetState state = this.PaintState;

			adorner.PaintArrayBackground (graphics, rect, state);

			Drawing.Rectangle localClip = this.MapClientToRoot (this.table_bounds);
			Drawing.Rectangle saveClip = graphics.SaveClippingRectangle ();

			graphics.SetClippingRectangle (localClip);

			// Dessine le tableau des textes.
			this.Update ();

			Drawing.Point pos = new Drawing.Point (this.table_bounds.Left, this.table_bounds.Top - this.row_height);
			double limit = this.total_width - this.offset + this.table_bounds.Left + 1;
			double maxx = System.Math.Min (this.table_bounds.Right, limit);
			int max = System.Math.Min (this.n_visible_rows, this.max_rows);

			for (int row = 0; row < max; row++)
			{
				pos.X = this.frame_margins.Left;

				WidgetState widgetState = WidgetState.Enabled;

				if (row + this.first_visible_row == this.selected_row)  // ligne sélectionnée ?
				{
					widgetState |= WidgetState.Selected;
				}

				Drawing.Rectangle rectLine = new Drawing.Rectangle ();

				rectLine.Left = this.table_bounds.Left;
				rectLine.Right = maxx;
				rectLine.Bottom = pos.Y;
				rectLine.Top = pos.Y + this.row_height;
				adorner.PaintCellBackground (graphics, rectLine, widgetState);
				pos.X += this.text_margin - System.Math.Floor (this.offset);
				for (int column = 0; column < this.max_columns; column++)
				{
					double endx = pos.X + this.column_widths[column];

					if (pos.X < localClip.Right && endx > localClip.Left)
					{
						adorner.PaintGeneralTextLayout (graphics, pos, this.layouts[row, column], widgetState, PaintTextStyle.Array, this.BackColor);
					}

					pos.X = endx;
				}

				pos.Y -= this.row_height;
			}

			rect = this.table_bounds;
			rect.Inflate (-0.5, -0.5);
			graphics.LineWidth = 1;

			Drawing.Color color = adorner.ColorTextFieldBorder ((state & WidgetState.Enabled) != 0);

			// Dessine le rectangle englobant.
			graphics.AddRectangle (rect);
			graphics.RenderSolid (color);

			// Dessine les lignes de séparation horizontales.
			if (true)
			{
				double x1 = this.table_bounds.Left;
				double x2 = maxx;
				double y = this.table_bounds.Top - 0.5;

				for (int i = 0; i < max; i++)
				{
					y -= this.row_height;
					graphics.AddLine (x1, y, x2, y);
					graphics.RenderSolid (color);
				}
			}

			// Dessine les lignes de séparation verticales.
			if (true)
			{
				limit = this.max_rows * this.row_height;
				limit = this.table_bounds.Top - (limit - this.first_visible_row * this.row_height);

				double y1 = System.Math.Max (this.table_bounds.Bottom, limit);
				double y2 = this.table_bounds.Top;
				double x = this.table_bounds.Left - this.offset + 0.5;

				for (int i = 0; i < this.max_columns; i++)
				{
					x += this.GetColumnWidth (i);
					if (x < this.table_bounds.Left || x > this.table_bounds.Right)  continue;

					graphics.AddLine (x, y1, x, y2);
					graphics.RenderSolid (color);
				}
			}

			graphics.RestoreClippingRectangle (saveClip);
		}

		protected override void PaintForegroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clip_rect)
		{
			IAdorner adorner = Widgets.Adorner.Factory.Active;
			Drawing.Rectangle rect = new Drawing.Rectangle (0, 0, this.Client.Width, this.Client.Height);
			WidgetState state = this.PaintState;

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
		protected TextProviderCallback			text_provider = null;
		protected TextLayout[,]					layouts;
		protected double						def_width = 100;	// largeur par défaut
		protected double						min_width = 10;	// largeur minimale
		protected double[]						column_widths;	// largeur des colonnes
		protected double						total_width;		// largeur totale
		protected Drawing.ContentAlignment[]	column_alignments;
		
		protected Drawing.Margins				frame_margins;			//	marges du cadre
		protected Drawing.Margins				table_margins;			//	marges de la table interne
		protected double						text_margin = 2;
		protected double						row_height = 16;
		protected double						slider_dim = 6;
		
		private Drawing.Rectangle				table_bounds = new Drawing.Rectangle ();
		protected Widget header;		// père de l'en-tête horizontale
		protected System.Collections.ArrayList header_buttons = new System.Collections.ArrayList ();
		protected System.Collections.ArrayList header_sliders = new System.Collections.ArrayList ();
		protected VScroller v_scroller;
		protected HScroller h_scroller;
		protected int n_visible_rows;
		protected int n_fully_visible_rows;
		protected int first_visible_row = 0;
		protected int selected_row = -1;
		protected double offset = 0;
		protected int dragRank;
		protected double dragPos;
		protected double dragDim;
		protected int lastDx;
		protected int lastDy;
		protected int lastVisibleRows;
		protected int lastFirstRow;
	}
}
