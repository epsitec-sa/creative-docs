namespace Epsitec.Common.Widgets
{
	[System.Flags] public enum AbstractCellArrayStyle
	{
		None			= 0x00000000,		// neutre
		ScrollNorm		= 0x00000001,		// défilement avec un ascenseur
		ScrollMagic		= 0x00000002,		// défilement aux extrémités
		Stretch			= 0x00000004,		// occupe toute la place
		Header			= 0x00000010,		// en-tête
		Mobile			= 0x00000020,		// dimensions mobiles
		Separator		= 0x00000040,		// lignes de séparation
		Sort			= 0x00000080,		// choix pour tri possible
		SelectCell		= 0x00000100,		// sélectionne une cellule individuelle
		SelectLine		= 0x00000200,		// sélectionne toute la ligne
		SelectMulti		= 0x00000400,		// sélections multiples possibles avec Ctrl
	}


	/// <summary>
	/// La classe AbstractCellArray est la classe de base pour les tableaux
	/// et les listes.
	/// </summary>
	public abstract class AbstractCellArray : AbstractGroup
	{
		public AbstractCellArray()
		{
			this.internalState |= InternalState.AutoFocus;
			this.internalState |= InternalState.Focusable;

			double h = this.DefaultFontHeight+4;
			this.defHeight = h;
			this.minHeight = h;
			this.headerHeight = h;

			this.container = new Widget();
			this.Children.Add(this.container);

			this.containerV = new Widget();
			this.Children.Add(this.containerV);

			this.containerH = new Widget();
			this.Children.Add(this.containerH);

			this.scrollerV = new Scroller();
			this.scrollerV.Invert = true;  // de haut en bas
			this.scrollerV.Moved += new EventHandler(this.HandleScrollerV);
			this.Children.Add(this.scrollerV);

			this.scrollerH = new Scroller();
			this.scrollerH.Moved += new EventHandler(this.HandleScrollerH);
			this.Children.Add(this.scrollerH);
		}
		
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.scrollerV.Moved -= new EventHandler(this.HandleScrollerV);
				this.scrollerH.Moved -= new EventHandler(this.HandleScrollerH);
			}
			
			base.Dispose(disposing);
		}


		// Sytle pour l'en-tête supérieure et l'ascenseur horizontal.
		public AbstractCellArrayStyle StyleH
		{
			get
			{
				return this.styleH;
			}

			set
			{
				if ( value != this.styleH )
				{
					this.styleH = value;
					this.isDirty = true;
					this.Invalidate();
				}
			}
		}
		
		// Sytle pour l'en-tête gauche et l'ascenseur vertical.
		public AbstractCellArrayStyle StyleV
		{
			get
			{
				return this.styleV;
			}

			set
			{
				if ( value != this.styleV )
				{
					this.styleV = value;
					this.isDirty = true;
					this.Invalidate();
				}
			}
		}

		// Offset horizontal dû au scrolling.
		public double OffsetH
		{
			get
			{
				return this.offsetH;
			}

			set
			{
				if ( value != this.offsetH )
				{
					this.offsetH = value;
					this.isDirty = true;
					this.Invalidate();
				}
			}
		}
		
		// Offset vertical dû au scrolling.
		public double OffsetV
		{
			get
			{
				return this.offsetV;
			}

			set
			{
				if ( value != this.offsetV )
				{
					this.offsetV = value;
					this.isDirty = true;
					this.Invalidate();
				}
			}
		}

		// Choix de la hauteur par défaut d'une ligne.
		public double DefHeight
		{
			get
			{
				return this.defHeight;
			}

			set
			{
				this.defHeight = value;
				this.minHeight = value;
				this.headerHeight = value;
			}
		}
		
		// Donne le nombre total de colonnes.
		public int Columns
		{
			get
			{
				return this.maxColumns;
			}
		}
		
		// Donne le nombre total de lignes.
		public int Rows
		{
			get
			{
				return this.maxRows;
			}
		}
		
		// Donne le nombre de colonnes visibles.
		public int VisibleColumns
		{
			get
			{
				return this.visibleColumns;
			}
		}
		
		// Donne le nombre de lignes visibles.
		public int VisibleRows
		{
			get
			{
				return this.visibleRows;
			}
		}
		
		// Donne le rectangle intérieur.
		public override Drawing.Rectangle Inside
		{
			get
			{
				Drawing.Rectangle rect = new Drawing.Rectangle();
				rect.Left   = this.margin+this.leftMargin;
				rect.Right  = this.Width-this.margin-this.rightMargin;
				rect.Bottom = this.margin+this.bottomMargin;
				rect.Top    = this.Height-this.margin-this.topMargin;
				return rect;
			}
		}

		// Objet occupant une cellule.		
		public virtual Cell this[int column, int row]
		{
			get
			{
				System.Diagnostics.Debug.Assert(this.array[column, row] != null);
				return this.array[column, row];
			}
			
			set
			{
				System.Diagnostics.Debug.Assert(this.array[column, row] != null);
				if ( value == null )  value = new Cell();
				
				this.array[column, row].SetArrayRank(null, -1, -1);
				this.array[column, row] = value;
				this.array[column, row].SetArrayRank(this, column, row);
				
				this.NotifyCellChanged(value);
			}
		}
		
		public virtual void NotifyCellChanged(Cell cell)
		{
			this.isDirty = true;
			this.Invalidate();
		}


		// Choix du nom d'une ligne de l'en-tête verticale.
		public virtual void SetHeaderTextV(int rank, string text)
		{
			if ( rank < 0 || rank >= this.maxRows )
			{
				throw new System.ArgumentOutOfRangeException();
			}
			
			HeaderButton button = this.FindButtonV(rank);
			button.Text = text;

			this.isDirty = true;
			this.Invalidate();
		}

		// Choix du nom d'une colonne de l'en-tête horizontale.
		public virtual void SetHeaderTextH(int rank, string text)
		{
			if ( rank < 0 || rank >= this.maxColumns )
			{
				throw new System.ArgumentOutOfRangeException();
			}
			
			HeaderButton button = this.FindButtonH(rank);
			button.Text = text;

			this.isDirty = true;
			this.Invalidate();
		}

		// Choix de la ligne de tri.
		public virtual void SetHeaderSortV(int rank, int mode)
		{
			if ( rank < 0 || rank >= this.maxRows )
			{
				throw new System.ArgumentOutOfRangeException();
			}
			
			for ( int i=0 ; i<this.maxRows ; i++ )
			{
				HeaderButton button = this.FindButtonV(i);
				button.SortMode = i==rank ? mode : 0;
			}
		}

		// Choix de la colonne de tri.
		public virtual void SetHeaderSortH(int rank, int mode)
		{
			if ( rank < 0 || rank >= this.maxColumns )
			{
				throw new System.ArgumentOutOfRangeException();
			}
			
			for ( int i=0 ; i<this.maxColumns ; i++ )
			{
				HeaderButton button = this.FindButtonH(i);
				button.SortMode = i==rank ? mode : 0;
			}
		}

		// Retourne la ligne de tri.
		public virtual bool GetHeaderSortV(out int rank, out int mode)
		{
			for ( rank=0 ; rank<this.maxRows ; rank++ )
			{
				HeaderButton button = this.FindButtonV(rank);
				mode = button.SortMode;
				if ( mode != 0 )  return true;
			}

			rank = -1;
			mode = 0;
			return false;
		}

		// Retourne la colonne de tri.
		public virtual bool GetHeaderSortH(out int rank, out int mode)
		{
			for ( rank=0 ; rank<this.maxColumns ; rank++ )
			{
				HeaderButton button = this.FindButtonH(rank);
				mode = button.SortMode;
				if ( mode != 0 )  return true;
			}

			rank = -1;
			mode = 0;
			return false;
		}

		
		// Les colonnes et les lignes contenues dans le tableau peuvent être escamotées
		// pour permettre de réaliser des tables avec des éléments miniaturisables, par
		// exemple.
		// Les appels MapToVisible et MapFromVisible permettent de convertir entre des
		// positions affichées (colonne/ligne) et des positions dans la table interne.
		
		public virtual double RetWidthColumn(int rank)
		{
			if ( rank < 0 || rank >= this.maxColumns )
			{
				throw new System.ArgumentOutOfRangeException();
			}
			
			return System.Math.Floor(this.widthColumns[rank]+0.5);
		}

		public virtual void SetWidthColumn(int rank, double width)
		{
			if ( rank < 0 || rank >= this.maxColumns )
			{
				throw new System.ArgumentOutOfRangeException();
			}
			
			width = System.Math.Max(width, this.minWidth);
			if ( (this.styleH & AbstractCellArrayStyle.Stretch) == 0 )
			{
				this.widthColumns[rank] = width;
			}
			else
			{
				if ( rank+1 < this.maxColumns )
				{
					width = System.Math.Min(width, this.widthColumns[rank]+this.widthColumns[rank+1]-this.minWidth);
					this.widthColumns[rank+1] += this.widthColumns[rank]-width;
				}
				this.widthColumns[rank] = width;
			}

			this.isDirty = true;
			this.Invalidate();
		}
		
		public virtual void HideColumns(int start, int count)
		{
			if ( start+count > this.maxColumns )
			{
				throw new System.ArgumentOutOfRangeException();
			}
			
			for ( int i=0 ; i<this.maxColumns ; i++ )
			{
				if ( i >= start && i < start+count )
				{
					this.widthColumns[i] = 0;
				}
			}
			this.isDirty = true;
			this.Invalidate();
		}
		
		public virtual void ShowColumns(int start, int count)
		{
			if ( start+count > this.maxColumns )
			{
				throw new System.ArgumentOutOfRangeException();
			}
			
			for ( int i=0 ; i<this.maxColumns ; i++ )
			{
				if ( i >= start && i < start+count )
				{
					this.widthColumns[i] = this.defWidth;
				}
			}
			this.isDirty = true;
			this.Invalidate();
		}
		
		public virtual double RetHeightRow(int rank)
		{
			if ( rank < 0 || rank >= this.maxRows )
			{
				throw new System.ArgumentOutOfRangeException();
			}
			
			return System.Math.Floor(this.heightRows[rank]+0.5);
		}

		public virtual void SetHeightRow(int rank, double height)
		{
			if ( rank < 0 || rank >= this.maxRows )
			{
				throw new System.ArgumentOutOfRangeException();
			}
			
			height = System.Math.Max(height, this.minHeight);
			if ( (this.styleV & AbstractCellArrayStyle.Stretch) == 0 )
			{
				this.heightRows[rank] = height;
			}
			else
			{
				if ( rank+1 < this.maxRows )
				{
					height = System.Math.Min(height, this.heightRows[rank]+this.heightRows[rank+1]-this.minHeight);
					this.heightRows[rank+1] += this.heightRows[rank]-height;
				}
				this.heightRows[rank] = height;
			}

			this.isDirty = true;
			this.Invalidate();
		}
		
		public virtual void HideRows(int start, int count)
		{
			if ( start+count > this.maxRows )
			{
				throw new System.ArgumentOutOfRangeException();
			}
			
			for ( int i=0 ; i<this.maxRows ; i++ )
			{
				if ( i >= start && i < start+count )
				{
					this.heightRows[i] = 0;
				}
			}
			this.isDirty = true;
			this.Invalidate();
		}
		
		public virtual void ShowRows(int start, int count)
		{
			if ( start+count > this.maxRows )
			{
				throw new System.ArgumentOutOfRangeException();
			}
			
			for ( int i=0 ; i<this.maxRows ; i++ )
			{
				if ( i >= start && i < start+count )
				{
					this.heightRows[i] = this.defHeight;
				}
			}
			this.isDirty = true;
			this.Invalidate();
		}
		

		// Adapte les largeurs des colonnes à la largeur du widget.
		protected void StretchColumns()
		{
			double areaWidth = this.Width-this.margin*2-this.leftMargin-this.rightMargin;
			double totalWidth = 0;
			for ( int i=0 ; i<this.maxColumns ; i++ )
			{
				totalWidth += this.widthColumns[i];
			}
			if ( totalWidth == 0 || this.maxColumns == 0 )  return;
			//if ( System.Math.Abs(totalWidth-areaWidth) < 0.5 )  return;

			for ( int i=0 ; i<this.maxColumns ; i++ )
			{
				this.widthColumns[i] *= areaWidth/totalWidth;
			}
		}
		
		// Adapte les hauteurs des lignes à la hauteur du widget.
		protected void StretchRows()
		{
			double areaHeight = this.Height-this.margin*2-this.bottomMargin-this.topMargin;
			double totalHeight = 0;
			for ( int i=0 ; i<this.maxRows ; i++ )
			{
				totalHeight += this.heightRows[i];
			}
			if ( totalHeight == 0 || this.maxRows == 0 )  return;
			//if ( System.Math.Abs(totalHeight-areaHeight) < 0.5 )  return;

			for ( int i=0 ; i<this.maxRows ; i++ )
			{
				this.heightRows[i] *= areaHeight/totalHeight;
			}
		}
		
		
		public virtual bool MapToVisible(ref int column, ref int row)
		{
			if ( column < 0 || column >= this.visibleColumns ||
				 row    < 0 || row    >= this.visibleRows    )
			{
				throw new System.ArgumentOutOfRangeException();
			}
			
			bool foundColumn = false;
			bool foundRow    = false;
			
			int rankColumn = 0;
			for ( int i=0 ; i<this.maxColumns ; i++ )
			{
				if ( column == rankColumn )
				{
					column = i;
					foundColumn = true;
					break;
				}
				if ( this.widthColumns[i] != 0 )  rankColumn ++;
			}

			int rankRow = 0;
			for ( int i=0 ; i<this.maxRows ; i++ )
			{
				if ( row == rankRow )
				{
					row = i;
					foundRow = true;
					break;
				}
				if ( this.heightRows[i] != 0 )  rankRow ++;
			}
			
			if ( !foundColumn ) column = -1;
			if ( !foundRow    ) row    = -1;
			
			return ( foundColumn & foundRow );
		}
		
		public virtual bool MapFromVisible(ref int column, ref int row)
		{
			if ( column < 0 || column >= this.maxColumns ||
				 row    < 0 || row    >= this.maxRows    )
			{
				throw new System.ArgumentOutOfRangeException();
			}
			
			int rankColumn = 0;
			for ( int i=0 ; i<this.maxColumns ; i++ )
			{
				if ( column == i )
				{
					if ( this.widthColumns[i] == 0 )
					{
						column = -1;
					}
					else
					{
						column = rankColumn;
					}
					break;
				}
				if ( this.widthColumns[i] != 0 )  rankColumn ++;
			}

			int rankRow = 0;
			for ( int i=0 ; i<this.maxRows ; i++ )
			{
				if ( row == i )
				{
					if ( this.heightRows[i] == 0 )
					{
						row = -1;
					}
					else
					{
						row = rankRow;
					}
					break;
				}
				if ( this.heightRows[i] != 0 )  rankRow ++;
			}
			
			return ( column != -1 && row != -1 );
		}
		
		
		// Appelé lorsque l'ascenseur vertical a bougé.
		private void HandleScrollerV(object sender)
		{
			//this.OffsetV = System.Math.Floor(this.scrollerV.Range-this.scrollerV.Position+0.5);
			this.OffsetV = System.Math.Floor(this.scrollerV.Position+0.5);
			//this.SetFocused(true);
		}

		// Appelé lorsque l'ascenseur horizontal a bougé.
		private void HandleScrollerH(object sender)
		{
			this.OffsetH = System.Math.Floor(this.scrollerH.Position+0.5);
			//this.SetFocused(true);
		}

		// Appelé lorsque le bouton d'en-tête vertical a été cliqué.
		private void HandleButtonClicked(object sender, MessageEventArgs e)
		{
			HeaderButton button = sender as HeaderButton;

			if ( button.HeaderButtonStyle == HeaderButtonStyle.Left )
			{
				if ( (this.styleV & AbstractCellArrayStyle.Sort) == 0 )  return;

				int row = button.Rank;
				int mode = button.SortMode;
				switch ( mode )
				{
					case -1:  mode =  1;  break;
					case  0:  mode =  1;  break;
					case  1:  mode = -1;  break;
				}
				this.SetHeaderSortV(row, mode);
				this.OnSortChanged();
			}

			if ( button.HeaderButtonStyle == HeaderButtonStyle.Top )
			{
				if ( (this.styleH & AbstractCellArrayStyle.Sort) == 0 )  return;

				int column = button.Rank;
				int mode = button.SortMode;
				switch ( mode )
				{
					case -1:  mode =  1;  break;
					case  0:  mode =  1;  break;
					case  1:  mode = -1;  break;
				}
				this.SetHeaderSortH(column, mode);
				this.OnSortChanged();
			}
		}

		// Appelé lorsque le slider va être déplacé.
		private void HandleSliderDragStarted(object sender, MessageEventArgs e)
		{
			HeaderSlider slider = sender as HeaderSlider;

			if ( slider.HeaderSliderStyle == HeaderSliderStyle.Left )
			{
				DragStartedRow(slider.Rank, e.Message.Y);
			}

			if ( slider.HeaderSliderStyle == HeaderSliderStyle.Top )
			{
				DragStartedColumn(slider.Rank, e.Message.X);
			}
		}

		// Appelé lorsque le slider est déplacé.
		private void HandleSliderDragMoved(object sender, MessageEventArgs e)
		{
			HeaderSlider slider = sender as HeaderSlider;

			if ( slider.HeaderSliderStyle == HeaderSliderStyle.Left )
			{
				DragMovedRow(slider.Rank, e.Message.Y);
			}

			if ( slider.HeaderSliderStyle == HeaderSliderStyle.Top )
			{
				DragMovedColumn(slider.Rank, e.Message.X);
			}
		}

		// Appelé lorsque le slider est fini de déplacer.
		private void HandleSliderDragEnded(object sender, MessageEventArgs e)
		{
			HeaderSlider slider = sender as HeaderSlider;

			if ( slider.HeaderSliderStyle == HeaderSliderStyle.Left )
			{
				DragEndedRow(slider.Rank, e.Message.Y);
			}

			if ( slider.HeaderSliderStyle == HeaderSliderStyle.Top )
			{
				DragEndedColumn(slider.Rank, e.Message.X);
			}
		}

		// La hauteur d'une ligne va être modifiée.
		protected void DragStartedRow(int row, double pos)
		{
			this.dragRank = row;
			this.dragPos  = pos;
			this.dragDim  = this.RetHeightRow(row);
		}

		// La largeur d'une colonne va être modifiée.
		protected void DragStartedColumn(int column, double pos)
		{
			this.dragRank = column;
			this.dragPos  = pos;
			this.dragDim  = this.RetWidthColumn(column);
		}

		// Modifie la hauteur d'une ligne.
		protected void DragMovedRow(int row, double pos)
		{
			this.SetHeightRow(this.dragRank, this.dragDim+(this.dragPos-pos));
			this.isDirty = true;
			this.Invalidate();
		}

		// Modifie la largeur d'une colonne.
		protected void DragMovedColumn(int column, double pos)
		{
			this.SetWidthColumn(this.dragRank, this.dragDim+(pos-this.dragPos));
			this.isDirty = true;
			this.Invalidate();
		}

		// La hauteur d'une ligne a été modifiée.
		protected void DragEndedRow(int row, double pos)
		{
			this.isDirty = true;
			this.Invalidate();
		}

		// La largeur d'une colonne a été modifiée.
		protected void DragEndedColumn(int column, double pos)
		{
			this.isDirty = true;
			this.Invalidate();
		}


		// Gestion d'un événement.
		protected override void ProcessMessage(Message message, Drawing.Point pos)
		{
			switch ( message.Type )
			{
				case MessageType.MouseDown:
					this.ProcessMouse(pos, message.IsShiftPressed, message.IsCtrlPressed);
					this.mouseDown = true;
					break;
				
				case MessageType.MouseMove:
					if ( this.mouseDown )
					{
						this.ProcessMouse(pos, message.IsShiftPressed, message.IsCtrlPressed);
					}
					break;

				case MessageType.MouseUp:
					if ( this.mouseDown )
					{
						this.ProcessMouse(pos, message.IsShiftPressed, message.IsCtrlPressed);
						this.mouseDown = false;
					}
					break;

				case MessageType.KeyDown:
					this.ProcessKeyDown(message.KeyCode);
					break;
			}
			
			message.Consumer = this;
		}

		// Gestion d'une touche pressée avec KeyDown.
		protected void ProcessKeyDown(int key)
		{
			switch ( key )
			{
				case (int)System.Windows.Forms.Keys.Left:
					this.SelectCellDir(-1, 0);
					break;

				case (int)System.Windows.Forms.Keys.Right:
					this.SelectCellDir(1, 0);
					break;

				case (int)System.Windows.Forms.Keys.Up:
					this.SelectCellDir(0, -1);
					break;

				case (int)System.Windows.Forms.Keys.Down:
					this.SelectCellDir(0, 1);
					break;
			}
		}

		// Sélectionne une cellule.
		protected void ProcessMouse(Drawing.Point pos, bool isShiftPressed, bool isCtrlPressed)
		{
			AbstractCellArrayStyle style = this.styleV | this.styleH;
			if ( (style & AbstractCellArrayStyle.SelectCell) == 0 &&
				 (style & AbstractCellArrayStyle.SelectLine) == 0 )  return;

			if ( (style & AbstractCellArrayStyle.SelectMulti) == 0 || !isCtrlPressed )
			{
				this.DeselectAll();
			}

			int row, column;
			if ( this.Detect(pos, out row, out column) )  // détecte la cellule visée par la souris
			{
				if ( !this.mouseDown )  // bouton pressé ?
				{
					this.mouseState = !this.array[column, row].IsSelected;
				}
				bool state = this.mouseState;

				if ( (style & AbstractCellArrayStyle.SelectMulti) != 0 && isShiftPressed )
				{
					this.SelectZone(column, row, this.selectedColumn, this.selectedRow, state);
				}
				else
				{
					this.SelectCell(column, row, state);
					this.selectedColumn = column;
					this.selectedRow = row;
				}
				this.OnSelectionChanged();

				if ( (this.styleV & AbstractCellArrayStyle.SelectLine) != 0 )
				{
					this.SelectRow(row, state);
				}

				if ( (this.styleH & AbstractCellArrayStyle.SelectLine) != 0 )
				{
					this.SelectColumn(column, state);
				}
			}
		}

		// Détecte dans quelle cellule est un point.
		protected bool Detect(Drawing.Point pos, out int row, out int column)
		{
			Drawing.Rectangle rect = this.container.Bounds;

			row = column = -1;
			if ( !rect.Contains(pos) )  return false;

			double py = rect.Top+this.offsetV;
			for ( int y=0 ; y<this.maxRows ; y++ )
			{
				double dy = this.RetHeightRow(y);
				py -= dy;
				if ( pos.Y >= py )
				{
					row = y;
					break;
				}
			}

			double px = rect.Left-this.offsetH;
			for ( int x=0 ; x<this.maxColumns ; x++ )
			{
				double dx = this.RetWidthColumn(x);
				px += dx;
				if ( pos.X <= px )
				{
					column = x;
					break;
				}
			}

			if ( row == -1 || column == -1 )  return false;
			return true;
		}

		// Sélectionne une cellule proche.
		protected void SelectCellDir(int dirColumn, int dirRow)
		{
			int column = this.selectedColumn+dirColumn;
			column = System.Math.Max(column, 0);
			column = System.Math.Min(column, this.maxColumns-1);

			int row = this.selectedRow+dirRow;
			row = System.Math.Max(row, 0);
			row = System.Math.Min(row, this.maxRows-1);

			if ( column == this.selectedColumn && row == this.selectedRow )  return;

			this.DeselectAll();

			this.SelectCell(column, row, true);
			this.selectedColumn = column;
			this.selectedRow = row;
			this.OnSelectionChanged();

			if ( (this.styleV & AbstractCellArrayStyle.SelectLine) != 0 )
			{
				this.SelectRow(row, true);
			}

			if ( (this.styleH & AbstractCellArrayStyle.SelectLine) != 0 )
			{
				this.SelectColumn(column, true);
			}

			this.ShowSelect();
		}

		// Desélectionne toutes les cellules.
		public void DeselectAll()
		{
			for ( int row=0 ; row<this.maxRows ; row++ )
			{
				for ( int column=0 ; column<this.maxColumns ; column++ )
				{
					this.SelectCell(column, row, false);
				}
			}
		}

		// Sélectionne une zone rectangulaire.
		protected void SelectZone(int column1, int row1, int column2, int row2, bool state)
		{
			int sc = System.Math.Min(column1, column2);
			int ec = System.Math.Max(column1, column2);
			int sr = System.Math.Min(row1, row2);
			int er = System.Math.Max(row1, row2);

			for ( int column=sc ; column<=ec ; column++ )
			{
				for ( int row=sr ; row<=er ; row ++ )
				{
					this.SelectCell(column, row, state);
				}
			}
		}

		// Sélectionne toute une ligne.
		public void SelectRow(int row, bool state)
		{
			if ( row < 0 || row >= this.maxRows )  return;
			for ( int column=0 ; column<this.maxColumns ; column++ )
			{
				this.SelectCell(column, row, state);
			}
			this.selectedRow = row;
		}

		// Sélectionne toute une colonne.
		public void SelectColumn(int column, bool state)
		{
			if ( column < 0 || column >= this.maxColumns )  return;
			for ( int row=0 ; row<this.maxRows ; row++ )
			{
				this.SelectCell(column, row, state);
			}
			this.selectedColumn = column;
		}

		// Sélectionne une cellule.
		public void SelectCell(int column, int row, bool state)
		{
			if ( row < 0 || row >= this.maxRows )  return;
			if ( column < 0 || column >= this.maxColumns )  return;
			this.array[column, row].SetSelected(state);
			foreach (Widget fils in this.array[column, row].Children)
			{
				fils.SetSelected(state);
			}
		}

		// Indique si une cellule est sélectionnée.
		public bool IsCellSelected(int row, int column)
		{
			if ( row < 0 || row >= this.maxRows )  return false;
			if ( column < 0 || column >= this.maxColumns )  return false;
			return this.array[column, row].IsSelected;
		}

		// Retourne la ligne sélectionnée.
		public int SelectedRow
		{
			get
			{
				return this.selectedRow;
			}
		}

		// Retourne la colonne sélectionnée.
		public int SelectedColumn
		{
			get
			{
				return this.selectedColumn;
			}
		}

		// Génère un événement pour dire que la sélection a changé.
		protected virtual void OnSelectionChanged()
		{
			if ( this.SelectionChanged != null )  // qq'un écoute ?
			{
				this.SelectionChanged(this);
			}
		}

		// Génère un événement pour dire que le tri a changé.
		protected virtual void OnSortChanged()
		{
			if ( this.SortChanged != null )  // qq'un écoute ?
			{
				this.SortChanged(this);
			}
		}


		// Rend la cellule sélectionnée visible.
		public void ShowSelect()
		{
			Drawing.Rectangle rect = this.Inside;

			if ( (this.styleV & AbstractCellArrayStyle.Stretch) == 0 &&
				 (this.styleH & AbstractCellArrayStyle.SelectLine) == 0 &&
				 this.selectedRow != -1 )
			{
				double start, end;
				start = end = 0;
				for ( int row=0 ; row<=this.selectedRow ; row++ )
				{
					start = end;
					end = start+this.heightRows[row];
				}

				if ( end > this.offsetV+rect.Height )  // sélection trop basse ?
				{
					this.OffsetV = end-rect.Height;
				}
				if ( start < this.offsetV )  // sélection trop haute ?
				{
					this.OffsetV = start;
				}
			}

			if ( (this.styleH & AbstractCellArrayStyle.Stretch) == 0 &&
				 (this.styleV & AbstractCellArrayStyle.SelectLine) == 0 &&
				 this.selectedColumn != -1 )
			{
				double start, end;
				start = end = 0;
				for ( int column=0 ; column<=this.selectedColumn ; column++ )
				{
					start = end;
					end = start+this.widthColumns[column];
				}

				if ( end > this.offsetH+rect.Width )  // sélection trop à droite ?
				{
					this.OffsetH = end-rect.Width;
				}
				if ( start < this.offsetH )  // sélection trop à gauche ?
				{
					this.OffsetH = start;
				}
			}
		}


		// Met à jour la géométrie du tableau.
		protected void Update()
		{
			if ( !this.isDirty )  return;
			this.UpdateClientGeometry();
		}

		// Met à jour la géométrie du conteneur.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			if ( this.scrollerV == null || this.scrollerH == null )  return;

			this.isDirty = false;

			this.showScrollerV = false;
			this.showScrollerH = false;
			if ( (this.styleV & AbstractCellArrayStyle.ScrollNorm) != 0 )  this.showScrollerV = true;
			if ( (this.styleH & AbstractCellArrayStyle.ScrollNorm) != 0 )  this.showScrollerH = true;

			this.leftMargin = 0;
			this.topMargin  = 0;
			if ( (this.styleV & AbstractCellArrayStyle.Header) != 0 )  this.leftMargin = this.headerWidth;
			if ( (this.styleH & AbstractCellArrayStyle.Header) != 0 )  this.topMargin  = this.headerHeight;

			Drawing.Rectangle rect = this.Bounds;
			Drawing.Rectangle iRect = new Drawing.Rectangle(this.margin, this.margin, rect.Width-this.margin*2, rect.Height-this.margin*2);

			this.rightMargin  = this.showScrollerV ? Scroller.StandardWidth : 0;
			this.bottomMargin = this.showScrollerH ? Scroller.StandardWidth : 0;

			iRect.Left   += this.leftMargin;
			iRect.Right  -= this.rightMargin;
			iRect.Bottom += this.bottomMargin;
			iRect.Top    -= this.topMargin;

			if ( (this.styleH & AbstractCellArrayStyle.Stretch) != 0 )
			{
				this.StretchColumns();
			}
			if ( (this.styleV & AbstractCellArrayStyle.Stretch) != 0 )
			{
				this.StretchRows();
			}

			// Positionne l'ascenseur vertical.
			if ( this.showScrollerV )
			{
				Drawing.Rectangle sRect = new Drawing.Rectangle();
				sRect = iRect;
				sRect.Left  = sRect.Right;
				sRect.Right = sRect.Left+Scroller.StandardWidth;
				this.scrollerV.Bounds = sRect;
				this.scrollerV.Show();
			}
			else
			{
				this.scrollerV.Hide();
			}

			// Positionne l'ascenseur horizontal.
			if ( this.showScrollerH )
			{
				Drawing.Rectangle sRect = new Drawing.Rectangle();
				sRect = iRect;
				sRect.Top    = sRect.Bottom;
				sRect.Bottom = sRect.Top-Scroller.StandardWidth;
				this.scrollerH.Bounds = sRect;
				this.scrollerH.Show();
			}
			else
			{
				this.scrollerH.Hide();
			}

			// Positionne le container.
			if ( this.container != null )
			{
				this.container.Bounds = iRect;
				UpdateArrayGeometry();
			}

			// Positionne les boutons de l'en-tête verticale.
			if ( (this.styleV & AbstractCellArrayStyle.Header) == 0 )
			{
				this.containerV.Hide();
			}
			else
			{
				Drawing.Rectangle hRect = new Drawing.Rectangle();
				hRect.Left   = iRect.Left-this.leftMargin;
				hRect.Right  = iRect.Left;
				hRect.Top    = iRect.Top;
				hRect.Bottom = iRect.Bottom;
				this.containerV.Bounds = hRect;
				this.containerV.Show();
				this.containerV.Children.Clear();

				hRect.Left  = 0;
				hRect.Right = this.leftMargin;
				hRect.Top   = iRect.Height+this.offsetV;
				for ( int i=0 ; i<this.maxRows ; i++ )
				{
					hRect.Bottom = hRect.Top-this.RetHeightRow(i);
					HeaderButton button = this.FindButtonV(i);
					button.Show();
					button.Bounds = hRect;
					button.Dynamic = ( (this.styleV & AbstractCellArrayStyle.Sort) != 0 );
					this.containerV.Children.Add(button);
					hRect.Top = hRect.Bottom;
				}

				hRect.Left  = 0;
				hRect.Right = this.leftMargin;
				hRect.Top   = iRect.Height+this.offsetV;
				for ( int i=0 ; i<this.maxRows ; i++ )
				{
					hRect.Bottom = hRect.Top-this.RetHeightRow(i);
					HeaderSlider slider = this.FindSliderV(i);
					if ( (this.styleV & AbstractCellArrayStyle.Mobile) == 0 ||
						 ((this.styleV & AbstractCellArrayStyle.Stretch) != 0 && i == this.maxRows-1) )
					{
						slider.Hide();
					}
					else
					{
						Drawing.Rectangle sRect = new Drawing.Rectangle();
						sRect.Left   = hRect.Left;
						sRect.Right  = hRect.Right;
						sRect.Bottom = hRect.Bottom-this.sliderDim/2;
						sRect.Top    = hRect.Bottom+this.sliderDim/2;
						slider.Show();
						slider.Bounds = sRect;
						this.containerV.Children.Add(slider);
					}
					hRect.Top = hRect.Bottom;
				}
			}

			// Positionne les boutons de l'en-tête horizontale.
			if ( (this.styleH & AbstractCellArrayStyle.Header) == 0 )
			{
				this.containerH.Hide();
			}
			else
			{
				Drawing.Rectangle hRect = new Drawing.Rectangle();
				hRect.Bottom = iRect.Top;
				hRect.Top    = iRect.Top+this.topMargin;
				hRect.Left   = iRect.Left;
				hRect.Right  = iRect.Right;
				this.containerH.Bounds = hRect;
				this.containerH.Show();
				this.containerH.Children.Clear();

				hRect.Bottom = 0;
				hRect.Top    = this.topMargin;
				hRect.Left   = -this.offsetH;
				for ( int i=0 ; i<this.maxColumns ; i++ )
				{
					hRect.Right = hRect.Left+this.RetWidthColumn(i);
					HeaderButton button = this.FindButtonH(i);
					button.Show();
					button.Bounds = hRect;
					button.Dynamic = ( (this.styleH & AbstractCellArrayStyle.Sort) != 0 );
					this.containerH.Children.Add(button);
					hRect.Left = hRect.Right;
				}

				hRect.Bottom = 0;
				hRect.Top    = this.topMargin;
				hRect.Left   = -this.offsetH;
				for ( int i=0 ; i<this.maxColumns ; i++ )
				{
					hRect.Right = hRect.Left+this.RetWidthColumn(i);
					HeaderSlider slider = this.FindSliderH(i);
					if ( (this.styleH & AbstractCellArrayStyle.Mobile) == 0 ||
						 ((this.styleH & AbstractCellArrayStyle.Stretch) != 0 && i == this.maxColumns-1) )
					{
						slider.Hide();
					}
					else
					{
						Drawing.Rectangle sRect = new Drawing.Rectangle();
						sRect.Left   = hRect.Right-this.sliderDim/2;
						sRect.Right  = hRect.Right+this.sliderDim/2;
						sRect.Bottom = hRect.Bottom;
						sRect.Top    = hRect.Top;
						slider.Show();
						slider.Bounds = sRect;
						this.containerH.Children.Add(slider);
					}
					hRect.Left = hRect.Right;
				}
			}

			this.UpdateScrollers();
		}

		// Met à jour les ascenseurs en fonction du tableau.
		protected void UpdateScrollers()
		{
			if ( this.scrollerV == null || this.scrollerH == null )  return;

			// Traite l'ascenseur vertical.
			if ( this.showScrollerV )
			{
				double areaHeight = this.Height-this.margin*2-this.bottomMargin-this.topMargin;
				double totalHeight = 0;
				for ( int i=0 ; i<this.maxRows ; i++ )
				{
					totalHeight += this.RetHeightRow(i);
				}

				if ( totalHeight <= areaHeight )
				{
					this.scrollerV.SetEnabled(false);
					this.scrollerV.Range = 1;
					this.scrollerV.Display = 1;
					this.scrollerV.Position = 0;
				}
				else
				{
					this.scrollerV.SetEnabled(true);
					this.scrollerV.Range = totalHeight-areaHeight;
					this.scrollerV.Display = areaHeight/totalHeight * this.scrollerV.Range;
					this.scrollerV.Position = this.offsetV;
					this.scrollerV.ButtonStep = this.defHeight;
					this.scrollerV.PageStep = areaHeight/2;
				}
				this.HandleScrollerV(this.scrollerV);  // update this.offsetV
			}

			// Traite l'ascenseur horizontal.
			if ( this.showScrollerH )
			{
				double areaWidth = this.Width-this.margin*2-this.leftMargin-this.rightMargin;
				double totalWidth = 0;
				for ( int i=0 ; i<this.maxColumns ; i++ )
				{
					totalWidth += this.RetWidthColumn(i);
				}

				if ( totalWidth <= areaWidth )
				{
					this.scrollerH.SetEnabled(false);
					this.scrollerH.Range = 1;
					this.scrollerH.Display = 1;
					this.scrollerH.Position = 0;
				}
				else
				{
					this.scrollerH.SetEnabled(true);
					this.scrollerH.Range = totalWidth-areaWidth;
					this.scrollerH.Display = areaWidth/totalWidth * this.scrollerH.Range;
					this.scrollerH.Position = this.offsetH;
					this.scrollerH.ButtonStep = this.defWidth;
					this.scrollerH.PageStep = areaWidth/2;
				}
				this.HandleScrollerH(this.scrollerH);  // update this.offsetH
			}
		}

		// Met à jour la géométrie de toutes les cellules du tableau.
		protected void UpdateArrayGeometry()
		{
			this.container.Children.Clear();
			Drawing.Rectangle rect = this.container.Bounds;
			double py = rect.Height+this.offsetV;
			for ( int y=0 ; y<this.maxRows ; y++ )
			{
				double dy = this.RetHeightRow(y);
				py -= dy;
				double px = -this.offsetH;
				for ( int x=0 ; x<this.maxColumns ; x++ )
				{
					double dx = this.RetWidthColumn(x);
					Drawing.Rectangle cRect = new Drawing.Rectangle(px, py, dx, dy);
					if ( cRect.Right > 0 && cRect.Left   < rect.Width  &&
						 cRect.Top   > 0 && cRect.Bottom < rect.Height )
					{
						this.array[x,y].Bounds = cRect;
						this.array[x,y].Parent = this.container;
					}
					else
					{
						System.Diagnostics.Debug.Assert(this.array[x,y].Parent == null);
					}
					px += dx;
				}
			}
		}


		// Choix des dimensions du tableau.
		public virtual void SetArraySize(int maxColumns, int maxRows)
		{
			if ( this.maxColumns == maxColumns &&
				 this.maxRows    == maxRows    )
			{
				return;
			}
			
			// Supprime les colonnes excédentaires.
			for ( int col=maxColumns ; col<this.maxColumns ; col++ )
			{
				for ( int row=0 ; row<this.maxRows ; row++ )
				{
					this.array[col,row].SetArrayRank(null, -1, -1);
					this.array[col,row] = null;
				}
			}
			
			int minMaxCol = System.Math.Min(maxColumns, this.maxColumns);
			int minMaxRow = System.Math.Min(maxRows,    this.maxRows   );
			
			// Supprime les lignes excédentaires, sans parcourir une seconde
			// fois les colonnes déjà supprimées.
			for ( int row=maxRows ; row<this.maxRows ; row++ )
			{
				for ( int col=0 ; col<minMaxCol ; col++ )
				{
					array[col,row].SetArrayRank(null, -1, -1);
					this.array[col,row] = null;
				}
			}
			
			// Alloue un nouveau tableau, puis copie le contenu de l'ancien tableau
			// dans le nouveau. L'ancien sera perdu.
			Cell[,] newArray = (maxColumns*maxRows > 0) ? new Cell[maxColumns, maxRows] : null;
			
			for ( int col=0 ; col<minMaxCol ; col++ )
			{
				for ( int row=0 ; row<minMaxRow ; row++ )
				{
					System.Diagnostics.Debug.Assert(newArray[col,row] == null);
					newArray[col,row] = this.array[col,row];
				}
			}
			
			// Remplit les colonnes nouvelles du tableau avec des cellules vides.
			for ( int col=this.maxColumns ; col<maxColumns ; col++ )
			{
				for ( int row=0 ; row<maxRows ; row++ )
				{
					System.Diagnostics.Debug.Assert(newArray[col,row] == null);
					newArray[col,row] = new Cell();
					newArray[col,row].SetArrayRank(this, col, row);
				}
			}
			
			// Remplit les lignes nouvelles du tableau avec des cellules vides,
			// sans parcourir une seconde fois les colonnes déjà initialisées.
			for ( int row=this.maxRows ; row<maxRows ; row++ )
			{
				for ( int col=0 ; col<minMaxCol ; col++ )
				{
					System.Diagnostics.Debug.Assert(newArray[col,row] == null);
					newArray[col,row] = new Cell();
					newArray[col,row].SetArrayRank(this, col, row);
				}
			}
			
			
			this.array = newArray;
			
			if ( maxColumns != this.maxColumns )
			{
				this.maxColumns = maxColumns;
				this.widthColumns = new double[this.maxColumns];
				for ( int i=0 ; i<this.maxColumns ; i++ )
				{
					this.widthColumns[i] = this.defWidth;
				}
				this.visibleColumns = this.maxColumns;
			}
			
			if ( maxRows != this.maxRows )
			{
				this.maxRows = maxRows;
				this.heightRows = new double[this.maxRows];
				for ( int i=0 ; i<this.maxRows ; i++ )
				{
					this.heightRows[i] = this.defHeight;
				}
				this.visibleRows = this.maxRows;
			}
			
			// Alloue les boutons pour les noms de l'en-tête.
			while ( this.headerButtonV.Count > 0 )
			{
				int i = this.headerButtonV.Count-1;
				HeaderButton button = this.FindButtonV(i);
				button.Clicked -= new MessageEventHandler(this.HandleButtonClicked);
				this.headerButtonV.RemoveAt(i);
			}
			for ( int i=0 ; i<this.maxRows ; i++ )
			{
				HeaderButton button = new HeaderButton();
				button.HeaderButtonStyle = HeaderButtonStyle.Left;
				button.Rank = i;
				button.Clicked += new MessageEventHandler(this.HandleButtonClicked);
				this.headerButtonV.Add(button);
			}

			while ( this.headerButtonH.Count > 0 )
			{
				int i = this.headerButtonH.Count-1;
				HeaderButton button = this.FindButtonH(i);
				button.Clicked -= new MessageEventHandler(this.HandleButtonClicked);
				this.headerButtonH.RemoveAt(i);
			}
			for ( int i=0 ; i<this.maxColumns ; i++ )
			{
				HeaderButton button = new HeaderButton();
				button.HeaderButtonStyle = HeaderButtonStyle.Top;
				button.Rank = i;
				button.Clicked += new MessageEventHandler(this.HandleButtonClicked);
				this.headerButtonH.Add(button);
			}

			// Alloue les boutons pour les sliders de l'en-tête.
			while ( this.headerSliderV.Count > 0 )
			{
				int i = this.headerSliderV.Count-1;
				HeaderSlider slider = this.FindSliderV(i);
				slider.DragStarted -= new MessageEventHandler(this.HandleSliderDragStarted);
				slider.DragMoved   -= new MessageEventHandler(this.HandleSliderDragMoved);
				slider.DragEnded   -= new MessageEventHandler(this.HandleSliderDragEnded);
				this.headerSliderV.RemoveAt(i);
			}
			for ( int i=0 ; i<this.maxRows ; i++ )
			{
				HeaderSlider slider = new HeaderSlider();
				slider.HeaderSliderStyle = HeaderSliderStyle.Left;
				slider.Rank = i;
				slider.DragStarted += new MessageEventHandler(this.HandleSliderDragStarted);
				slider.DragMoved   += new MessageEventHandler(this.HandleSliderDragMoved);
				slider.DragEnded   += new MessageEventHandler(this.HandleSliderDragEnded);
				this.headerSliderV.Add(slider);
			}

			while ( this.headerSliderH.Count > 0 )
			{
				int i = this.headerSliderH.Count-1;
				HeaderSlider slider = this.FindSliderH(i);
				slider.DragStarted -= new MessageEventHandler(this.HandleSliderDragStarted);
				slider.DragMoved   -= new MessageEventHandler(this.HandleSliderDragMoved);
				slider.DragEnded   -= new MessageEventHandler(this.HandleSliderDragEnded);
				this.headerSliderH.RemoveAt(i);
			}
			for ( int i=0 ; i<this.maxColumns ; i++ )
			{
				HeaderSlider slider = new HeaderSlider();
				slider.HeaderSliderStyle = HeaderSliderStyle.Top;
				slider.Rank = i;
				slider.DragStarted += new MessageEventHandler(this.HandleSliderDragStarted);
				slider.DragMoved   += new MessageEventHandler(this.HandleSliderDragMoved);
				slider.DragEnded   += new MessageEventHandler(this.HandleSliderDragEnded);
				this.headerSliderH.Add(slider);
			}
		}


		protected HeaderButton FindButtonV(int index)
		{
			return this.headerButtonV[index] as HeaderButton;
		}

		protected HeaderButton FindButtonH(int index)
		{
			return this.headerButtonH[index] as HeaderButton;
		}

		protected HeaderSlider FindSliderV(int index)
		{
			return this.headerSliderV[index] as HeaderSlider;
		}

		protected HeaderSlider FindSliderH(int index)
		{
			return this.headerSliderH[index] as HeaderSlider;
		}


		// Dessine le tableau.
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			IAdorner adorner = Widgets.Adorner.Factory.Active;

			this.Update();  // mis à jour si nécessaire

			Drawing.Rectangle rect  = new Drawing.Rectangle(0, 0, this.Client.Width, this.Client.Height);
			WidgetState       state = this.PaintState;
			Direction         dir   = this.RootDirection;
			
			// Dessine le cadre et le fond du tableau.
			adorner.PaintTextFieldBackground(graphics, rect, state, dir, TextFieldStyle.Normal);

#if false
			if ( this.showScrollerV && this.showScrollerH )
			{
				// Grise le carré à l'intersection des 2 ascenseurs.
				rect.Left   = rect.Right-this.margin-this.rightMargin;
				rect.Width  = this.rightMargin;
				rect.Bottom = rect.Bottom+this.margin;
				rect.Height = this.bottomMargin;
				adorner.PaintButtonBackground(graphics, rect, WidgetState.None, dir, ButtonStyle.Flat);
			}
#endif
		}

		// Dessine la grille par-dessus.
		protected override void PaintForegroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clip_rect)
		{
			IAdorner adorner = Widgets.Adorner.Factory.Active;

			Drawing.Rectangle rect = this.Inside;
			rect.Inflate(-0.5, -0.5);

			graphics.LineWidth = 1;
			Drawing.Color color = Drawing.Color.FromRGB(0.9,0.9,0.9);  // gris-clair

			// Dessine le rectangle englobant.
			graphics.AddRectangle(rect);
			graphics.RenderSolid(color);

			// Dessine les lignes de séparation horizontales.
			if ( (this.styleV & AbstractCellArrayStyle.Separator) != 0 )
			{
				double limit = 0;
				for ( int i=0 ; i<this.maxColumns ; i++ )
				{
					limit += this.RetWidthColumn(i);
				}
				limit = limit-this.offsetH+rect.Left;
				double x1 = rect.Left;
				double x2 = System.Math.Min(rect.Right, limit);
				double y  = rect.Top+this.offsetV;
				for ( int i=0 ; i<this.maxRows ; i++ )
				{
					y -= this.RetHeightRow(i);
					if ( y < rect.Bottom || y > rect.Top )  continue;
					graphics.AddLine(x1, y, x2, y);
					graphics.RenderSolid(color);
				}
			}

			// Dessine les lignes de séparation verticales.
			if ( (this.styleH & AbstractCellArrayStyle.Separator) != 0 )
			{
				double limit = 0;
				for ( int i=0 ; i<this.maxRows ; i++ )
				{
					limit += this.RetHeightRow(i);
				}
				limit = rect.Top-(limit-this.offsetV);
				double y1 = System.Math.Max(rect.Bottom, limit);
				double y2 = rect.Top;
				double x  = rect.Left-this.offsetH;
				for ( int i=0 ; i<this.maxColumns ; i++ )
				{
					x += this.RetWidthColumn(i);
					if ( x < rect.Left || x > rect.Right )  continue;
					graphics.AddLine(x, y1, x, y2);
					graphics.RenderSolid(color);
				}
			}
		}
		

		public event EventHandler SelectionChanged;
		public event EventHandler SortChanged;

		protected bool							isDirty;
		protected bool							mouseDown = false;
		protected bool							mouseState;
		protected AbstractCellArrayStyle		styleH;
		protected AbstractCellArrayStyle		styleV;
		protected int							maxColumns;		// nb total de colonnes
		protected int							maxRows;		// nb total de lignes
		protected int							visibleColumns;	// nb de colonnes visibles
		protected int							visibleRows;	// nb de lignes visibles
		protected Widget						container;		// père de toutes les cellules
		protected Widget						containerV;		// père de l'en-tête verticale
		protected Widget						containerH;		// père de l'en-tête horizontale
		protected Cell[,]						array;			// tableau des cellules
		protected double						defWidth = 100;
		protected double						defHeight = 18;
		protected double						minWidth = 10;
		protected double						minHeight = 18;
		protected double						headerWidth = 30;
		protected double						headerHeight = 18;
		protected double[]						widthColumns;	// largeurs des colonnes
		protected double[]						heightRows;		// hauteurs des lignes
		protected double						sliderDim = 6;
		protected double						margin = 3;
		protected double						leftMargin = 0;		// marge pour en-tête
		protected double						rightMargin = 0;	// marge pour ascenseur
		protected double						bottomMargin = 0;	// marge pour ascenseur
		protected double						topMargin = 0;		// marge pour en-tête
		protected bool							showScrollerV = true;
		protected bool							showScrollerH = true;
		protected double						offsetV = 0;
		protected double						offsetH = 0;
		protected Scroller						scrollerV;
		protected Scroller						scrollerH;
		protected System.Collections.ArrayList	headerButtonV = new System.Collections.ArrayList();
		protected System.Collections.ArrayList	headerButtonH = new System.Collections.ArrayList();
		protected System.Collections.ArrayList	headerSliderV = new System.Collections.ArrayList();
		protected System.Collections.ArrayList	headerSliderH = new System.Collections.ArrayList();
		protected int							selectedRow = -1;
		protected int							selectedColumn = -1;
		protected int							dragRank;
		protected double						dragPos;
		protected double						dragDim;
	}
}
