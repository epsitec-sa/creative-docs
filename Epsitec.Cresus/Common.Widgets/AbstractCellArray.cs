namespace Epsitec.Common.Widgets
{
	[System.Flags] public enum CellArrayStyle
	{
		None			= 0x00000000,		// neutre
		ScrollNorm		= 0x00000001,		// d�filement avec un ascenseur
		ScrollMagic		= 0x00000002,		// d�filement aux extr�mit�s
		Stretch			= 0x00000004,		// occupe toute la place
		Header			= 0x00000010,		// en-t�te
		Mobile			= 0x00000020,		// dimensions mobiles
		Separator		= 0x00000040,		// lignes de s�paration
		Sort			= 0x00000080,		// choix pour tri possible
		SelectCell		= 0x00000100,		// s�lectionne une cellule individuelle
		SelectLine		= 0x00000200,		// s�lectionne toute la ligne
		SelectMulti		= 0x00000400,		// s�lections multiples possibles avec Ctrl
	}


	/// <summary>
	/// La classe AbstractCellArray est la classe de base pour les tableaux
	/// et les listes.
	/// </summary>
	[Support.SuppressBundleSupport]
	public abstract class AbstractCellArray : AbstractGroup
	{
		public AbstractCellArray()
		{
			if ( Support.ObjectBundler.IsBooting )
			{
				//	N'initialise rien, car cela prend passablement de temps... et de toute
				//	mani�re, on n'a pas besoin de toutes ces informations pour pouvoir
				//	utiliser IBundleSupport.
				
				return;
			}
			
			this.AutoFocus = true;
			this.AutoDoubleClick = true;
			this.InternalState |= InternalState.Focusable;
			this.InternalState &= ~InternalState.PossibleContainer;
			IAdorner adorner = Widgets.Adorners.Factory.Active;
			this.margins = adorner.GeometryArrayMargins;

			double h = this.DefaultFontHeight+4;
			this.defHeight = h;
			this.minHeight = h;
			this.headerHeight = h;

			this.container  = new Widget(this);
			this.containerV = new Widget(this);
			this.containerH = new Widget(this);
			this.scrollerV  = new VScroller(this);
			this.scrollerH  = new HScroller(this);
			
			this.container.InheritParentFocus = true;
			
			this.scrollerV.IsInverted = true;  // de haut en bas
			this.scrollerV.ValueChanged += new Support.EventHandler(this.HandleScrollerV);
			this.scrollerH.ValueChanged += new Support.EventHandler(this.HandleScrollerH);
		}
		
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				if ( this.scrollerV != null )
				{
					this.scrollerV.ValueChanged -= new Support.EventHandler(this.HandleScrollerV);
				}
				if ( this.scrollerH != null )
				{
					this.scrollerH.ValueChanged -= new Support.EventHandler(this.HandleScrollerH);
				}
				
				this.SetFocusedCell(null);
			}
			
			base.Dispose(disposing);
		}
		
		public override double				DefaultHeight
		{
			get { return 80; }
		}
		
		public override double				DefaultWidth
		{
			get { return 80; }
		}


		public Drawing.Color				HiliteColor
		{
			get { return this.hiliteColor; }
			set { this.hiliteColor = value; }
		}
		


		// Sytle pour l'en-t�te sup�rieur et l'ascenseur horizontal.
		public CellArrayStyle StyleH
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
		
		// Sytle pour l'en-t�te gauche et l'ascenseur vertical.
		public CellArrayStyle StyleV
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

		// Offset horizontal d� au scrolling.
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
		
		// Offset vertical d� au scrolling.
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

		// Choix de la hauteur par d�faut d'une ligne.
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
		
		public override Drawing.Margins InternalPadding
		{
			get
			{
				Drawing.Margins padding = new Drawing.Margins (this.margins.Left+this.leftMargin,
					/**/									   this.margins.Right+this.rightMargin,
					/**/									   this.margins.Top+this.topMargin,
					/**/									   this.margins.Bottom+this.bottomMargin);
				
				return padding;
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
//			
//			set
//			{
//				System.Diagnostics.Debug.Assert(this.array[column, row] != null);
//				if ( value == null )  value = new Cell(null);
//				
//				this.array[column, row].SetArrayRank(null, -1, -1);
//				this.array[column, row] = value;
//				this.array[column, row].SetArrayRank(this, column, row);
//				
//				this.NotifyCellChanged(value);
//			}
		}
		
		public virtual void NotifyCellChanged(Cell cell)
		{
			this.isDirty = true;
			this.Invalidate();
		}


		// Choix du nom d'une ligne de l'en-t�te vertical.
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

		// Choix du nom d'une colonne de l'en-t�te horizontal.
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
		public virtual void SetHeaderSortV(int rank, SortMode mode)
		{
			if ( rank < 0 || rank >= this.maxRows )
			{
				throw new System.ArgumentOutOfRangeException();
			}
			
			for ( int i=0 ; i<this.maxRows ; i++ )
			{
				HeaderButton button = this.FindButtonV(i);
				button.SortMode = i==rank ? mode : SortMode.None;
			}
		}

		// Choix de la colonne de tri.
		public virtual void SetHeaderSortH(int rank, SortMode mode)
		{
			if ( rank < 0 || rank >= this.maxColumns )
			{
				throw new System.ArgumentOutOfRangeException();
			}
			
			for ( int i=0 ; i<this.maxColumns ; i++ )
			{
				HeaderButton button = this.FindButtonH(i);
				button.SortMode = i==rank ? mode : SortMode.None;
			}
		}

		// Retourne la ligne de tri.
		public virtual bool GetHeaderSortV(out int rank, out SortMode mode)
		{
			for ( rank=0 ; rank<this.maxRows ; rank++ )
			{
				HeaderButton button = this.FindButtonV(rank);
				mode = button.SortMode;
				if ( mode != SortMode.None )  return true;
			}

			rank = -1;
			mode = 0;
			return false;
		}

		// Retourne la colonne de tri.
		public virtual bool GetHeaderSortH(out int rank, out SortMode mode)
		{
			for ( rank=0 ; rank<this.maxColumns ; rank++ )
			{
				HeaderButton button = this.FindButtonH(rank);
				mode = button.SortMode;
				if ( mode != SortMode.None )  return true;
			}

			rank = -1;
			mode = 0;
			return false;
		}

		
		// Les colonnes et les lignes contenues dans le tableau peuvent �tre escamot�es
		// pour permettre de r�aliser des tables avec des �l�ments miniaturisables, par
		// exemple.
		// Les appels MapToVisible et MapFromVisible permettent de convertir entre des
		// positions affich�es (colonne/ligne) et des positions dans la table interne.
		
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
			if ( (this.styleH & CellArrayStyle.Stretch) == 0 )
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
			if ( (this.styleV & CellArrayStyle.Stretch) == 0 )
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
		

		// Adapte les largeurs des colonnes � la largeur du widget.
		protected void StretchColumns()
		{
			double areaWidth = this.Width-this.margins.Width-this.leftMargin-this.rightMargin;
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
		
		// Adapte les hauteurs des lignes � la hauteur du widget.
		protected void StretchRows()
		{
			double areaHeight = this.Height-this.margins.Height-this.bottomMargin-this.topMargin;
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
		
		
		// Appel� lorsque l'ascenseur vertical a boug�.
		private void HandleScrollerV(object sender)
		{
			this.OffsetV = System.Math.Floor(this.scrollerV.DoubleValue+0.5);
		}

		// Appel� lorsque l'ascenseur horizontal a boug�.
		private void HandleScrollerH(object sender)
		{
			this.OffsetH = System.Math.Floor(this.scrollerH.DoubleValue+0.5);
		}

		// Appel� lorsque le bouton d'en-t�te vertical a �t� cliqu�.
		private void HandleButtonClicked(object sender, MessageEventArgs e)
		{
			HeaderButton button = sender as HeaderButton;

			if ( button.Style == HeaderButtonStyle.Left )
			{
				if ( (this.styleV & CellArrayStyle.Sort) == 0 )  return;

				int      row  = button.Index;
				SortMode mode = button.SortMode;
				
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
				
				this.SetHeaderSortV(row, mode);
				this.OnSortChanged();
			}

			if ( button.Style == HeaderButtonStyle.Top )
			{
				if ( (this.styleH & CellArrayStyle.Sort) == 0 )  return;

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
				
				this.SetHeaderSortH(column, mode);
				this.OnSortChanged();
			}
		}

		// Appel� lorsque le slider va �tre d�plac�.
		private void HandleSliderDragStarted(object sender, MessageEventArgs e)
		{
			HeaderSlider slider = sender as HeaderSlider;
			
			this.isDraggingSlider = true;
			this.savedTotalWidth = 0;
			
			for ( int i=0 ; i<this.maxColumns ; i++ )
			{
				this.savedTotalWidth += this.RetWidthColumn(i);
			}
			
			if ( slider.Style == HeaderSliderStyle.Left )
			{
				DragStartedRow(slider.Index, e.Message.Y);
			}

			if ( slider.Style == HeaderSliderStyle.Top )
			{
				DragStartedColumn(slider.Index, e.Message.X);
			}
		}

		// Appel� lorsque le slider est d�plac�.
		private void HandleSliderDragMoved(object sender, MessageEventArgs e)
		{
			HeaderSlider slider = sender as HeaderSlider;

			if ( slider.Style == HeaderSliderStyle.Left )
			{
				DragMovedRow(slider.Index, e.Message.Y);
			}

			if ( slider.Style == HeaderSliderStyle.Top )
			{
				DragMovedColumn(slider.Index, e.Message.X);
			}
		}

		// Appel� lorsque le slider est fini de d�placer.
		private void HandleSliderDragEnded(object sender, MessageEventArgs e)
		{
			HeaderSlider slider = sender as HeaderSlider;
			
			this.isDraggingSlider = false;
			
			if ( slider.Style == HeaderSliderStyle.Left )
			{
				DragEndedRow(slider.Index, e.Message.Y);
			}

			if ( slider.Style == HeaderSliderStyle.Top )
			{
				DragEndedColumn(slider.Index, e.Message.X);
			}
		}

		// La hauteur d'une ligne va �tre modifi�e.
		protected void DragStartedRow(int row, double pos)
		{
			this.dragRank = row;
			this.dragPos  = pos;
			this.dragDim  = this.RetHeightRow(row);
		}

		// La largeur d'une colonne va �tre modifi�e.
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

		// La hauteur d'une ligne a �t� modifi�e.
		protected void DragEndedRow(int row, double pos)
		{
			this.isDirty = true;
			this.Invalidate();
		}

		// La largeur d'une colonne a �t� modifi�e.
		protected void DragEndedColumn(int column, double pos)
		{
			this.isDirty = true;
			this.Invalidate();
		}


		// Gestion d'un �v�nement.
		protected override void ProcessMessage(Message message, Drawing.Point pos)
		{
			switch ( message.Type )
			{
				case MessageType.MouseEnter:
					break;

				case MessageType.MouseLeave:
					this.ProcessMouseLeave();
					break;
				
				case MessageType.MouseDown:
					this.ProcessMouse(pos, message.IsShiftPressed, message.IsCtrlPressed, false);
					this.mouseDown = true;
					break;
				
				case MessageType.MouseMove:
					if ( this.mouseDown )
					{
						this.ProcessMouse(pos, message.IsShiftPressed, message.IsCtrlPressed, false);
					}
					else
					{
						this.ProcessMouseMove(pos);
					}
					break;

				case MessageType.MouseUp:
					if ( this.mouseDown )
					{
						this.ProcessMouse(pos, message.IsShiftPressed, message.IsCtrlPressed, true);
						this.mouseDown = false;
					}
					break;

				case MessageType.MouseWheel:
					if ( message.Wheel < 0 )  this.OffsetV = System.Math.Min(this.OffsetV+this.defHeight, this.scrollerV.DoubleRange);
					if ( message.Wheel > 0 )  this.OffsetV = System.Math.Max(this.OffsetV-this.defHeight, 0);
					break;

				case MessageType.KeyDown:
					if ( !this.ProcessKeyDown(message.KeyCode) )
					{
						return;
					}
					break;
				
				default:
					return;
			}
			
			message.Consumer = this;
		}

		// Gestion d'une touche press�e avec KeyDown.
		protected bool ProcessKeyDown(KeyCode key)
		{
			switch ( key )
			{
				case KeyCode.ArrowLeft:
					this.SelectCellDir(-1, 0);
					return true;

				case KeyCode.ArrowRight:
					this.SelectCellDir(1, 0);
					return true;

				case KeyCode.ArrowUp:
					this.SelectCellDir(0, -1);
					return true;

				case KeyCode.ArrowDown:
					this.SelectCellDir(0, 1);
					return true;
			}
			return false;
		}

		// S�lectionne une cellule.
		protected void ProcessMouse(Drawing.Point pos, bool isShiftPressed, bool isCtrlPressed, bool isFinal)
		{
			CellArrayStyle style = this.styleV | this.styleH;
			if ( (style & CellArrayStyle.SelectCell) == 0 &&
				 (style & CellArrayStyle.SelectLine) == 0 )  return;

			if ( (style & CellArrayStyle.SelectMulti) == 0 || !isCtrlPressed )
			{
				this.DeselectAll();
				this.selectedRow = -1;
				this.selectedColumn = -1;
			}

			int row, column;
			if ( this.Detect(pos, out row, out column) )  // d�tecte la cellule vis�e par la souris
			{
				if ( !this.mouseDown )  // bouton press� ?
				{
					this.mouseState = !this.array[column, row].IsSelected;
				}
				bool state = this.mouseState;

				if ( (style & CellArrayStyle.SelectMulti) != 0 && isShiftPressed )
				{
					this.SelectZone(column, row, this.selectedColumn, this.selectedRow, state);
				}
				else
				{
					this.SelectCell(column, row, state);
					this.selectedColumn = column;
					this.selectedRow = row;
				}

				if ( (this.styleV & CellArrayStyle.SelectLine) != 0 )
				{
					this.SelectRow(row, state);
				}

				if ( (this.styleH & CellArrayStyle.SelectLine) != 0 )
				{
					this.SelectColumn(column, state);
				}
			}

			this.OnSelectionChanged();
			if ( isFinal )
			{
				this.OnFinalSelectionChanged();
			}
		}

		// Appel� lorsque la souris ne survole plus rien.
		protected void ProcessMouseLeave()
		{
			this.ProcessMouseFlyOver(-1, -1);
		}

		// Appel� lorsque la souris a boug�.
		protected void ProcessMouseMove(Drawing.Point pos)
		{
			int row, column;
			this.Detect(pos, out row, out column);  // d�tecte la cellule vis�e par la souris
			this.ProcessMouseFlyOver(row, column);
		}

		// Indique la cellule survol�e.
		protected void ProcessMouseFlyOver(int row, int column)
		{
			if ( row != this.flyOverRow || column != this.flyOverColumn )
			{
				this.flyOverRow = row;
				this.flyOverColumn = column;

				this.OnFlyOverChanged();
			}
			else
			{
				this.flyOverRow = row;
				this.flyOverColumn = column;
			}
		}

		public int FlyOverRow
		{
			get { return this.flyOverRow; }
		}

		public int FlyOverColumn
		{
			get { return this.flyOverColumn; }
		}

		// D�tecte dans quelle cellule est un point.
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

		// S�lectionne une cellule proche.
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

			if ( (this.styleV & CellArrayStyle.SelectLine) != 0 )
			{
				this.SelectRow(row, true);
			}

			if ( (this.styleH & CellArrayStyle.SelectLine) != 0 )
			{
				this.SelectColumn(column, true);
			}

			this.OnSelectionChanged();
			this.OnFinalSelectionChanged();
			this.ShowSelect();
		}

		// Des�lectionne toutes les cellules.
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

		// S�lectionne une zone rectangulaire.
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

		// S�lectionne toute une ligne.
		public void SelectRow(int row, bool state)
		{
			if ( row < 0 || row >= this.maxRows )  return;
			for ( int column=0 ; column<this.maxColumns ; column++ )
			{
				this.SelectCell(column, row, state);
			}
			if ( state )
			{
				this.selectedRow = row;
			}
		}

		// S�lectionne toute une colonne.
		public void SelectColumn(int column, bool state)
		{
			if ( column < 0 || column >= this.maxColumns )  return;
			for ( int row=0 ; row<this.maxRows ; row++ )
			{
				this.SelectCell(column, row, state);
			}
			if ( state )
			{
				this.selectedColumn = column;
			}
		}

		// S�lectionne une cellule.
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

		// Indique si une cellule est s�lectionn�e.
		public bool IsCellSelected(int row, int column)
		{
			if ( row < 0 || row >= this.maxRows )  return false;
			if ( column < 0 || column >= this.maxColumns )  return false;
			return this.array[column, row].IsSelected;
		}

		// Retourne la ligne s�lectionn�e.
		public int SelectedRow
		{
			get
			{
				return this.selectedRow;
			}
		}

		// Retourne la colonne s�lectionn�e.
		public int SelectedColumn
		{
			get
			{
				return this.selectedColumn;
			}
		}

		// G�n�re un �v�nement pour dire que la s�lection a chang�.
		protected virtual void OnSelectionChanged()
		{
			if ( this.SelectionChanged != null )  // qq'un �coute ?
			{
				this.SelectionChanged(this);
			}
		}

		// G�n�re un �v�nement pour dire que la s�lection a chang�.
		protected virtual void OnFinalSelectionChanged()
		{
			if ( this.FinalSelectionChanged != null )  // qq'un �coute ?
			{
				this.FinalSelectionChanged(this);
			}
		}

		// G�n�re un �v�nement pour dire que le tri a chang�.
		protected virtual void OnSortChanged()
		{
			if ( this.SortChanged != null )  // qq'un �coute ?
			{
				this.SortChanged(this);
			}
		}

		// G�n�re un �v�nement pour dire que la cellule survol�e � chang�.
		protected virtual void OnFlyOverChanged()
		{
			if ( this.FlyOverChanged != null )  // qq'un �coute ?
			{
				this.FlyOverChanged(this);
			}
		}


		public void ShowSelect()
		{
			//	Rend la cellule s�lectionn�e visible.
			
			this.ShowCell(this.selectedRow, this.selectedColumn);
		}
		
		public void ShowCell(int showRow, int showColumn)
		{
			Drawing.Rectangle rect = this.Client.Bounds;
			rect.Deflate(this.InternalPadding);

			if ( (this.styleV & CellArrayStyle.Stretch) == 0 &&
				 (this.styleH & CellArrayStyle.SelectLine) == 0 &&
				 showRow != -1 )
			{
				double start, end;
				start = end = 0;
				for ( int row=0 ; row<=showRow ; row++ )
				{
					start = end;
					end = start+this.heightRows[row];
				}

				if ( end > this.offsetV+rect.Height )  // s�lection trop basse ?
				{
					this.OffsetV = end-rect.Height;
				}
				if ( start < this.offsetV )  // s�lection trop haute ?
				{
					this.OffsetV = start;
				}
			}

			if ( (this.styleH & CellArrayStyle.Stretch) == 0 &&
				 (this.styleV & CellArrayStyle.SelectLine) == 0 &&
				 showColumn != -1 )
			{
				double start, end;
				start = end = 0;
				for ( int column=0 ; column<=showColumn ; column++ )
				{
					start = end;
					end = start+this.widthColumns[column];
				}

				if ( end > this.offsetH+rect.Width )  // s�lection trop � droite ?
				{
					this.OffsetH = end-rect.Width;
				}
				if ( start < this.offsetH )  // s�lection trop � gauche ?
				{
					this.OffsetH = start;
				}
			}
		}


		// Met � jour la g�om�trie du tableau.
		public void Update()
		{
			if ( !this.isDirty )  return;
			this.UpdateClientGeometry();
		}

		// Met � jour la g�om�trie du conteneur.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			if ( this.scrollerV == null || this.scrollerH == null )  return;

			this.isDirty = false;

			IAdorner adorner = Widgets.Adorners.Factory.Active;
			this.margins = adorner.GeometryArrayMargins;

			this.showScrollerV = false;
			this.showScrollerH = false;
			if ( (this.styleV & CellArrayStyle.ScrollNorm) != 0 )  this.showScrollerV = true;
			if ( (this.styleH & CellArrayStyle.ScrollNorm) != 0 )  this.showScrollerH = true;

			this.leftMargin = 0;
			this.topMargin  = 0;
			if ( (this.styleV & CellArrayStyle.Header) != 0 )  this.leftMargin = this.headerWidth;
			if ( (this.styleH & CellArrayStyle.Header) != 0 )  this.topMargin  = this.headerHeight;

			Drawing.Rectangle rect = this.Bounds;
			Drawing.Rectangle iRect = new Drawing.Rectangle(this.margins.Left, this.margins.Bottom, rect.Width-this.margins.Width, rect.Height-this.margins.Height);

			this.rightMargin  = this.showScrollerV ? this.scrollerV.Width-1 : 0;
			this.bottomMargin = this.showScrollerH ? this.scrollerH.Height-1 : 0;

			iRect.Left   += this.leftMargin;
			iRect.Right  -= this.rightMargin;
			iRect.Bottom += this.bottomMargin;
			iRect.Top    -= this.topMargin;

			if ( (this.styleH & CellArrayStyle.Stretch) != 0 )
			{
				this.StretchColumns();
			}
			if ( (this.styleV & CellArrayStyle.Stretch) != 0 )
			{
				this.StretchRows();
			}

			// Positionne l'ascenseur vertical.
			if ( this.showScrollerV )
			{
				Drawing.Rectangle sRect = iRect;
				sRect.Left  = sRect.Right-1;
				sRect.Right = sRect.Left+this.scrollerV.Width;
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
				Drawing.Rectangle sRect = iRect;
				sRect.Top    = sRect.Bottom+1;
				sRect.Bottom = sRect.Top-this.scrollerH.Height;
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
				this.UpdateArrayGeometry();
			}

			// Positionne les boutons de l'en-t�te vertical.
			if ( (this.styleV & CellArrayStyle.Header) == 0 )
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
				if ( this.isGrimy )  this.containerV.Children.Clear();

				hRect.Left  = 0;
				hRect.Right = this.leftMargin;
				hRect.Top   = iRect.Height+this.offsetV;
				for ( int i=0 ; i<this.maxRows ; i++ )
				{
					hRect.Bottom = hRect.Top-this.RetHeightRow(i);
					HeaderButton button = this.FindButtonV(i);
					button.Show();
					button.Bounds = hRect;
					button.IsDynamic = ( (this.styleV & CellArrayStyle.Sort) != 0 );
					if ( this.isGrimy )  this.containerV.Children.Add(button);
					hRect.Top = hRect.Bottom;
				}

				hRect.Left  = 0;
				hRect.Right = this.leftMargin;
				hRect.Top   = iRect.Height+this.offsetV;
				for ( int i=0 ; i<this.maxRows ; i++ )
				{
					hRect.Bottom = hRect.Top-this.RetHeightRow(i);
					HeaderSlider slider = this.FindSliderV(i);
					if ( (this.styleV & CellArrayStyle.Mobile) == 0 ||
						 ((this.styleV & CellArrayStyle.Stretch) != 0 && i == this.maxRows-1) )
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
						if ( this.isGrimy )  this.containerV.Children.Add(slider);
					}
					hRect.Top = hRect.Bottom;
				}
			}

			// Positionne les boutons de l'en-t�te horizontal.
			if ( (this.styleH & CellArrayStyle.Header) == 0 )
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
				if ( this.isGrimy )  this.containerH.Children.Clear();

				hRect.Bottom = 0;
				hRect.Top    = this.topMargin;
				hRect.Left   = -this.offsetH;
				for ( int i=0 ; i<this.maxColumns ; i++ )
				{
					hRect.Right = hRect.Left+this.RetWidthColumn(i);
					HeaderButton button = this.FindButtonH(i);
					button.Show();
					button.Bounds = hRect;
					button.IsDynamic = ( (this.styleH & CellArrayStyle.Sort) != 0 );
					if ( this.isGrimy )  this.containerH.Children.Add(button);
					hRect.Left = hRect.Right;
				}

				hRect.Bottom = 0;
				hRect.Top    = this.topMargin;
				hRect.Left   = -this.offsetH;
				for ( int i=0 ; i<this.maxColumns ; i++ )
				{
					hRect.Right = hRect.Left+this.RetWidthColumn(i);
					HeaderSlider slider = this.FindSliderH(i);
					if ( (this.styleH & CellArrayStyle.Mobile) == 0 ||
						 ((this.styleH & CellArrayStyle.Stretch) != 0 && i == this.maxColumns-1) )
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
						if ( this.isGrimy )  this.containerH.Children.Add(slider);
					}
					hRect.Left = hRect.Right;
				}
			}

			this.isGrimy = false;
			this.UpdateScrollers();
		}

		protected override void OnAdornerChanged()
		{
			this.UpdateClientGeometry();
			base.OnAdornerChanged();
		}

		public override Drawing.Rectangle GetShapeBounds()
		{
			IAdorner adorner = Widgets.Adorners.Factory.Active;
			Drawing.Rectangle rect = this.Client.Bounds;
			rect.Inflate(adorner.GeometryListShapeBounds);
			return rect;
		}

		// Met � jour les ascenseurs en fonction du tableau.
		protected void UpdateScrollers()
		{
			if ( this.scrollerV == null || this.scrollerH == null )  return;

			// Traite l'ascenseur vertical.
			if ( this.showScrollerV )
			{
				double areaHeight = this.Height-this.margins.Height-this.bottomMargin-this.topMargin;
				double totalHeight = 0;
				for ( int i=0 ; i<this.maxRows ; i++ )
				{
					totalHeight += this.RetHeightRow(i);
				}

				if ( totalHeight <= areaHeight ||
					 totalHeight <= 0          ||
					 areaHeight  <= 0          )
				{
					this.scrollerV.SetEnabled(false);
					this.scrollerV.MaxValue          = 1;
					this.scrollerV.VisibleRangeRatio = 1;
					this.scrollerV.Value             = 0;
				}
				else
				{
					this.scrollerV.SetEnabled(true);
					this.scrollerV.MaxValue          = (decimal) (totalHeight-areaHeight);
					this.scrollerV.VisibleRangeRatio = (decimal) (areaHeight/totalHeight);
					this.scrollerV.Value             = (decimal) (this.offsetV);
					this.scrollerV.SmallChange       = (decimal) (this.defHeight);
					this.scrollerV.LargeChange       = (decimal) (areaHeight/2);
				}
				this.HandleScrollerV(this.scrollerV);  // update this.offsetV
			}

			// Traite l'ascenseur horizontal.
			if ( this.showScrollerH )
			{
				double areaWidth = this.Width-this.margins.Width-this.leftMargin-this.rightMargin;
				double totalWidth = 0;
				
				if ( this.isDraggingSlider )
				{
					totalWidth = this.savedTotalWidth;
				}
				else
				{
					for ( int i=0 ; i<this.maxColumns ; i++ )
					{
						totalWidth += this.RetWidthColumn(i);
					}
				}

				if ( totalWidth <= areaWidth ||
					 totalWidth <= 0         ||
					 areaWidth  <= 0         )
				{
					this.scrollerH.SetEnabled(false);
					this.scrollerH.MaxValue          = 1;
					this.scrollerH.VisibleRangeRatio = 1;
					this.scrollerH.Value             = 0;
				}
				else
				{
					this.scrollerH.SetEnabled(true);
					this.scrollerH.MaxValue          = (decimal) (totalWidth-areaWidth);
					this.scrollerH.VisibleRangeRatio = (decimal) (areaWidth/totalWidth);
					this.scrollerH.Value             = (decimal) (this.offsetH);
					this.scrollerH.SmallChange       = (decimal) (this.defWidth);
					this.scrollerH.LargeChange       = (decimal) (areaWidth/2);
				}
				this.HandleScrollerH(this.scrollerH);  // update this.offsetH
			}
		}

		// Met � jour la g�om�trie de toutes les cellules du tableau.
		protected void UpdateArrayGeometry()
		{
			Cell focusedCell = null;
			
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
						cRect.Left += 1;
						cRect.Top  -= 1;  // laisse la place pour la grille
						this.array[x,y].Bounds = cRect;
						this.array[x,y].SetParent (this.container);
						this.array[x,y].SetVisible(true);
						this.array[x,y].SetArrayRank(this, x, y);
					}
					else if (this.array[x,y].Parent != null)
					{
						this.DetachCell (this.array[x,y], true);
					}
					px += dx;
				}
			}
			
			this.SetFocusedCell (focusedCell);
		}
		
		protected void DetachCell(Cell cell, bool keep_focus)
		{
			if ((keep_focus) &&
				(cell.ContainsKeyboardFocus))
			{
				//	La cellule qui contient le focus n'est plus visible; il faudrait donc
				//	supprimer son parent, mais ce faisant, on perdrait le focus. Dilemme !
							
				focusedCell = cell;
				focusedCell.Hide ();
			}
			else
			{
				if (this.focusedCell == cell)
				{
					this.focusedCell = null;
					cell.SetFocused (false);
					this.SetFocused (true);
				}
				
				cell.SetArrayRank (null, -1, -1);
				cell.SetParent (null);
			}
		}
		
		
		protected void SetFocusedCell(Cell cell)
		{
			if ( this.focusedCell != cell )
			{
				if ( this.focusedWidget != null )
				{
					this.focusedWidget.IsKeyboardFocusedChanged -= new Types.PropertyChangedEventHandler (this.HandleFocusedWidgetIsKeyboardFocusedChanged);
					this.focusedWidget.PreProcessing -= new MessageEventHandler(this.HandleFocusedWidgetPreProcessing);
					this.focusedWidget = null;
				}
				
				this.focusedCell = cell;
				
				if ( this.focusedCell != null )
				{
					this.focusedWidget = this.focusedCell.FindFocusedChild();
					
					System.Diagnostics.Debug.Assert( this.focusedWidget != null );
					
					this.focusedWidget.IsKeyboardFocusedChanged += new Types.PropertyChangedEventHandler (this.HandleFocusedWidgetIsKeyboardFocusedChanged);
					this.focusedWidget.PreProcessing += new MessageEventHandler(this.HandleFocusedWidgetPreProcessing);
				}
			}
		}

		
		private void HandleFocusedWidgetIsKeyboardFocusedChanged(object sender, Types.PropertyChangedEventArgs e)
		{
			bool focused = (bool) e.NewValue;
			
			if (! focused)
			{
				this.SetFocusedCell (null);
			}
		}
		
		private void HandleFocusedWidgetPreProcessing(object sender, MessageEventArgs e)
		{
			if ( e.Message.IsKeyType )
			{
				this.ShowCell(this.focusedCell.RankRow, this.focusedCell.RankColumn);
			}
		}
		
		
		// Choix des dimensions du tableau.
		public virtual void SetArraySize(int maxColumns, int maxRows)
		{
			if ((this.maxColumns == maxColumns) &&
				(this.maxRows == maxRows))
			{
				return;
			}
			
			//	Supprime les colonnes exc�dentaires.
			
			for (int col = maxColumns; col < this.maxColumns; col++)
			{
				for (int row = 0; row < this.maxRows; row++)
				{
					this.DetachCell (this.array[col,row], false);
					this.array[col,row] = null;
				}
			}
			
			int minMaxCol = System.Math.Min (maxColumns, this.maxColumns);
			int minMaxRow = System.Math.Min (maxRows, this.maxRows);
			
			//	Supprime les lignes exc�dentaires, sans parcourir une seconde
			//	fois les colonnes d�j� supprim�es.
			
			for (int row = maxRows; row < this.maxRows; row++)
			{
				for (int col = 0; col < minMaxCol; col++)
				{
					this.DetachCell (this.array[col,row], false);
					this.array[col,row] = null;
				}
			}
			
			//	Alloue un nouveau tableau, puis copie le contenu de l'ancien tableau
			//	dans le nouveau. L'ancien sera perdu.
			
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
					newArray[col,row] = new Cell(null);
					newArray[col,row].SetArrayRank(this, col, row);
				}
			}
			
			// Remplit les lignes nouvelles du tableau avec des cellules vides,
			// sans parcourir une seconde fois les colonnes d�j� initialis�es.
			for ( int row=this.maxRows ; row<maxRows ; row++ )
			{
				for ( int col=0 ; col<minMaxCol ; col++ )
				{
					System.Diagnostics.Debug.Assert(newArray[col,row] == null);
					newArray[col,row] = new Cell(null);
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
			
			// Alloue les boutons pour les noms de l'en-t�te.
			while ( this.headerButtonV.Count > 0 )
			{
				int i = this.headerButtonV.Count-1;
				HeaderButton button = this.FindButtonV(i);
				button.Clicked -= new MessageEventHandler(this.HandleButtonClicked);
				this.headerButtonV.RemoveAt(i);
			}
			for ( int i=0 ; i<this.maxRows ; i++ )
			{
				HeaderButton button = new HeaderButton(null);
				button.Style = HeaderButtonStyle.Left;
				button.Index = i;
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
				HeaderButton button = new HeaderButton(null);
				button.Style = HeaderButtonStyle.Top;
				button.Index = i;
				button.Clicked += new MessageEventHandler(this.HandleButtonClicked);
				this.headerButtonH.Add(button);
			}

			// Alloue les boutons pour les sliders de l'en-t�te.
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
				HeaderSlider slider = new HeaderSlider(null);
				slider.Style = HeaderSliderStyle.Left;
				slider.Index = i;
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
				HeaderSlider slider = new HeaderSlider(null);
				slider.Style = HeaderSliderStyle.Top;
				slider.Index = i;
				slider.DragStarted += new MessageEventHandler(this.HandleSliderDragStarted);
				slider.DragMoved   += new MessageEventHandler(this.HandleSliderDragMoved);
				slider.DragEnded   += new MessageEventHandler(this.HandleSliderDragEnded);
				this.headerSliderH.Add(slider);
			}

			this.isGrimy = true;
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
			IAdorner adorner = Widgets.Adorners.Factory.Active;

			this.Update();  // mis � jour si n�cessaire

			// Dessine le cadre et le fond du tableau.
			Drawing.Rectangle rect = this.Client.Bounds;
			WidgetState state = this.PaintState;
			adorner.PaintArrayBackground(graphics, rect, state);

#if false
			if ( this.showScrollerV && this.showScrollerH )
			{
				// Grise le carr� � l'intersection des 2 ascenseurs.
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
			IAdorner adorner = Widgets.Adorners.Factory.Active;

			WidgetState state = this.PaintState;
			Drawing.Rectangle rect = this.Client.Bounds;
			rect.Deflate(this.InternalPadding);
			rect.Inflate(-0.5, -0.5);

			graphics.LineWidth = 1;
			Drawing.Color color = adorner.ColorTextFieldBorder((state&WidgetState.Enabled) != 0);

			// Dessine le rectangle englobant.
			graphics.AddRectangle(rect);
			graphics.RenderSolid(color);

			// Dessine les lignes de s�paration horizontales.
			if ( (this.styleV & CellArrayStyle.Separator) != 0 )
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

			// Dessine les lignes de s�paration verticales.
			if ( (this.styleH & CellArrayStyle.Separator) != 0 )
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
			
			// Dessine le cadre du tableau.
			rect = this.Client.Bounds;
			adorner.PaintArrayForeground(graphics, rect, state);
		}
		

		public event Support.EventHandler SelectionChanged;
		public event Support.EventHandler FinalSelectionChanged;
		public event Support.EventHandler SortChanged;
		public event Support.EventHandler FlyOverChanged;

		protected bool							isDirty;
		protected bool							isGrimy;
		protected bool							mouseDown = false;
		protected bool							mouseState;
		protected CellArrayStyle				styleH;
		protected CellArrayStyle				styleV;
		protected int							maxColumns;		// nb total de colonnes
		protected int							maxRows;		// nb total de lignes
		protected int							visibleColumns;	// nb de colonnes visibles
		protected int							visibleRows;	// nb de lignes visibles
		protected Widget						container;		// p�re de toutes les cellules
		protected Widget						containerV;		// p�re de l'en-t�te vertical
		protected Widget						containerH;		// p�re de l'en-t�te horizontal
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
		protected Drawing.Margins				margins;
		protected double						leftMargin = 0;		// marge pour en-t�te
		protected double						rightMargin = 0;	// marge pour ascenseur
		protected double						bottomMargin = 0;	// marge pour ascenseur
		protected double						topMargin = 0;		// marge pour en-t�te
		protected bool							showScrollerV = true;
		protected bool							showScrollerH = true;
		protected double						offsetV = 0;
		protected double						offsetH = 0;
		protected VScroller						scrollerV;
		protected HScroller						scrollerH;
		protected System.Collections.ArrayList	headerButtonV = new System.Collections.ArrayList();
		protected System.Collections.ArrayList	headerButtonH = new System.Collections.ArrayList();
		protected System.Collections.ArrayList	headerSliderV = new System.Collections.ArrayList();
		protected System.Collections.ArrayList	headerSliderH = new System.Collections.ArrayList();
		protected int							selectedRow = -1;
		protected int							selectedColumn = -1;
		protected int							dragRank;
		protected double						dragPos;
		protected double						dragDim;
		protected int							flyOverRow = -1;
		protected int							flyOverColumn = -1;
		protected Drawing.Color					hiliteColor = Drawing.Color.Empty;
		
		protected bool							isDraggingSlider;
		protected double						savedTotalWidth;
		
		protected Cell							focusedCell;
		protected Widget						focusedWidget;
	}
}
