namespace Epsitec.Common.Widgets
{
	using Keys = System.Windows.Forms.Keys;
	
	public enum ScrollArrayShow
	{
		Extremity,		// déplacement minimal aux extrémités
		Middle,			// déplacement central
	}

	public enum ScrollArrayAdjust
	{
		MoveUp,			// déplace le haut
		MoveDown,		// déplace le bas
	}

	/// <summary>
	/// La classe ScrollArray réalise une liste déroulante optimisée
	/// à deux dimensions ne pouvant contenir que des textes fixes,
	/// dans le style de la liste gauche de Crésus.
	/// </summary>
	public class ScrollArray : Widget
	{
		public ScrollArray()
		{
			this.internalState |= InternalState.AutoFocus;
			this.internalState |= InternalState.Focusable;

			this.header = new Widget();
			this.Children.Add(this.header);

			this.scrollerV = new VScroller();
			this.scrollerV.IsInverted = true;
			this.scrollerV.Moved += new EventHandler(this.HandleScrollerV);
			this.Children.Add(this.scrollerV);

			this.scrollerH = new HScroller();
			this.scrollerH.Moved += new EventHandler(this.HandleScrollerH);
			this.Children.Add(this.scrollerH);

			this.rowHeight = this.DefaultFontHeight+2;
		}


		// Spécifie le délégué pour remplir les cellules.
		// En mode sans FillText, la liste est remplie à l'avance avec SetText.
		// Une copie de tous les strings est alors contenue dans this.array.
		// En mode FillText, c'est ScrollArray qui demande le contenu de chaque
		// cellule au fur et à mesure à l'aide du délégué FillText. Ce mode
		// est particulièrement efficace pour de grandes quantités de données.
		public FillText FuncFillText
		{
			set
			{
				this.funcFillText = value;
			}
		}


		// Vide toute la liste.
		public void Reset()
		{
			if ( this.funcFillText == null )
			{
				for ( int row=0 ; row<this.array.Count ; row++ )
				{
					System.Collections.ArrayList list = (System.Collections.ArrayList)this.array[row];
					list.Clear();
				}
				this.array.Clear();
			}
			this.maxRows = 0;
			this.firstRow = 0;
			this.selectedRow = -1;

			this.isDirty = true;
			this.Invalidate();
		}

		// Donne le nombre de colonnes.
		public int Columns
		{
			get
			{
				return this.maxColumns;
			}

			set
			{
				if ( this.maxColumns != value )
				{
					this.maxColumns = value;

					this.widthColumns = new double[this.maxColumns];
					this.widthTotal = 0;
					for ( int i=0 ; i<this.maxColumns ; i++ )
					{
						this.widthColumns[i] = this.defWidth;
						this.widthTotal += this.defWidth;
					}

					this.alignmentColumns = new Drawing.ContentAlignment[this.maxColumns];
					for ( int i=0 ; i<this.maxColumns ; i++ )
					{
						this.alignmentColumns[i] = Drawing.ContentAlignment.MiddleLeft;
					}

					this.headerButton.Clear();
					for ( int i=0 ; i<this.maxColumns ; i++ )
					{
						HeaderButton button = new HeaderButton();
						button.HeaderButtonStyle = HeaderButtonStyle.Top;
						button.Dynamic = true;
						button.Rank = i;
						button.Clicked += new MessageEventHandler(this.HandleButtonClicked);
						this.headerButton.Add(button);
					}

					this.headerSlider.Clear();
					for ( int i=0 ; i<this.maxColumns ; i++ )
					{
						HeaderSlider slider = new HeaderSlider();
						slider.HeaderSliderStyle = HeaderSliderStyle.Top;
						slider.Rank = i;
						slider.DragStarted += new MessageEventHandler(this.HandleSliderDragStarted);
						slider.DragMoved   += new MessageEventHandler(this.HandleSliderDragMoved);
						slider.DragEnded   += new MessageEventHandler(this.HandleSliderDragEnded);
						this.headerSlider.Add(slider);
					}

					this.isDirty = true;
					this.Update();
				}
			}
		}

		// Donne le nombre de lignes.
		public int Rows
		{
			get
			{
				return this.maxRows;
			}

			set
			{
				if ( this.funcFillText != null )
				{
					this.maxRows = value;
					this.isDirty = true;
					this.Invalidate();
				}
			}
		}

		// Ajoute un texte dans le tableau.
		public void SetText(int row, int column, string text)
		{
			if ( this.funcFillText != null )  return;
			if ( this.widthColumns == null )  return;
			if ( row < 0 || column < 0 )  return;

			if ( row >= this.array.Count )
			{
				for ( int i=this.array.Count ; i<=row ; i++ )
				{
					System.Collections.ArrayList newLine = new System.Collections.ArrayList();
					this.array.Add(newLine);
				}
			}
			this.maxRows = System.Math.Max(this.maxRows, row+1);
			System.Collections.ArrayList list = (System.Collections.ArrayList)this.array[row];

			if ( column >= list.Count )
			{
				for ( int i=list.Count ; i<=column ; i++ )
				{
					string newCell = "";
					list.Add(newCell);
				}
			}
			list[column] = text;

			this.isDirty = true;
			this.Invalidate();
		}

		// Donne un texte du tableau.
		public string GetText(int row, int column)
		{
			if ( this.funcFillText != null )  return "";
			if ( this.widthColumns == null )  return "";
			if ( row < 0 || column < 0 )  return "";

			if ( row >= this.array.Count )  return "";
			System.Collections.ArrayList list = (System.Collections.ArrayList)this.array[row];

			if ( column >= list.Count )  return "";
			return (string)list[column];
		}


		// Ligne sélectionnée, -1 si aucune.
		public int SelectedIndex
		{
			get
			{
				return this.selectedRow;
			}

			set
			{
				if ( value != -1 )
				{
					value = System.Math.Max(value, 0);
					value = System.Math.Min(value, this.maxRows);
				}
				if ( value != this.selectedRow )
				{
					this.selectedRow = value;
					this.isDirty = true;
					this.Invalidate();
					this.OnSelectedIndexChanged();
				}
			}
		}

		// Première ligne visible.
		public int FirstRow
		{
			get
			{
				return this.firstRow;
			}

			set
			{
				value = System.Math.Max(value, 0);
				value = System.Math.Min(value, System.Math.Max(this.maxRows-this.visibleRowsFull, 0));
				if ( value != this.firstRow )
				{
					this.firstRow = value;
					this.UpdateScroller();
					this.Invalidate();
				}
			}
		}

		// Offset horizontal.
		public double OffsetH
		{
			get
			{
				return this.offsetH;
			}

			set
			{
				if ( this.offsetH != value )
				{
					this.offsetH = value;
					this.isDirty = true;
					this.Invalidate();
				}
			}
		}

		// Spécifie la largeur d'une colonne.
		public void SetWidthColumn(int column, double width)
		{
			if ( this.widthColumns == null )  return;
			width = System.Math.Floor(width);
			if ( this.widthColumns[column] != width )
			{
				width = System.Math.Max(width, this.minWidth);
				this.widthColumns[column] = width;
				this.widthTotal = 0;
				for ( int i=0 ; i<this.maxColumns ; i++ )
				{
					this.widthTotal += this.widthColumns[i];
				}
				this.isDirty = true;
				this.Invalidate();
			}
		}

		// Retourne la largeur d'une colonne.
		public double RetWidthColumn(int column)
		{
			if ( this.widthColumns == null )  return 0;
			return this.widthColumns[column];
		}

		// Spécifie l'alignement d'une colonne.
		public void SetAlignmentColumn(int column, Drawing.ContentAlignment alignment)
		{
			if ( this.widthColumns == null )  return;
			if ( this.alignmentColumns[column] != alignment )
			{
				this.alignmentColumns[column] = alignment;
				this.isDirty = true;
				this.Invalidate();
			}
		}

		// Retourne l'alignement d'une colonne.
		public Drawing.ContentAlignment RetAlignmentColumn(int column)
		{
			if ( this.widthColumns == null )  return Drawing.ContentAlignment.MiddleLeft;
			return this.alignmentColumns[column];
		}

		// Choix du nom d'une colonne de l'en-tête.
		public void SetHeaderText(int rank, string text)
		{
			if ( rank < 0 || rank >= this.maxColumns )
			{
				throw new System.ArgumentOutOfRangeException();
			}
			
			HeaderButton button = this.FindButton(rank);
			button.Text = text;

			this.isDirty = true;
			this.Invalidate();
		}

		// Choix de la colonne de tri.
		public void SetHeaderSort(int rank, int mode)
		{
			if ( rank < 0 || rank >= this.maxColumns )
			{
				throw new System.ArgumentOutOfRangeException();
			}
			
			for ( int i=0 ; i<this.maxColumns ; i++ )
			{
				HeaderButton button = this.FindButton(i);
				button.SortMode = i==rank ? mode : 0;
			}
		}

		// Retourne la colonne de tri.
		public bool GetHeaderSort(out int rank, out int mode)
		{
			for ( rank=0 ; rank<this.maxColumns ; rank++ )
			{
				HeaderButton button = this.FindButton(rank);
				mode = button.SortMode;
				if ( mode != 0 )  return true;
			}

			rank = -1;
			mode = 0;
			return false;
		}


		// Indique si la ligne sélectionnée est visible.
		public bool IsShowSelect()
		{
			if ( this.selectedRow == -1 )  return true;
			if ( this.selectedRow >= this.firstRow &&
				this.selectedRow <  this.firstRow+this.visibleRowsFull )  return true;
			return false;
		}

		// Rend la ligne sélectionnée visible.
		public void ShowSelect(ScrollArrayShow mode)
		{
			if ( this.selectedRow == -1 )  return;

			int fl = this.firstRow;
			if ( mode == ScrollArrayShow.Extremity )
			{
				if ( this.selectedRow < this.firstRow )
				{
					fl = this.selectedRow;
				}
				if ( this.selectedRow > this.firstRow+this.visibleRowsFull-1 )
				{
					fl = this.selectedRow-(this.visibleRowsFull-1);
				}
			}
			if ( mode == ScrollArrayShow.Middle )
			{
				int display = System.Math.Min(this.visibleRowsFull, this.maxRows);
				fl = System.Math.Min(this.selectedRow+display/2, this.maxRows-1);
				fl = System.Math.Max(fl-display+1, 0);
			}
			this.firstRow = fl;
		}

		// Ajuste la hauteur pour afficher pile un nombre entier de lignes.
		public bool AdjustToMultiple(ScrollArrayAdjust mode)
		{
			this.Update();

			double h = this.rectInside.Height;
			int nbLines = (int)(h/this.rowHeight);
			double adjust = h - nbLines*this.rowHeight;
			if ( adjust == 0 )  return false;

			if ( mode == ScrollArrayAdjust.MoveUp )
			{
				this.Top = System.Math.Floor(this.Top-adjust);
			}
			if ( mode == ScrollArrayAdjust.MoveDown )
			{
				this.Bottom = System.Math.Floor(this.Bottom+adjust);
			}
			this.Invalidate();
			return true;
		}

		// Ajuste la hauteur pour afficher exactement le nombre de lignes contenues.
		public bool AdjustToContent(ScrollArrayAdjust mode, double hMin, double hMax)
		{
			this.Update();

			double h = this.rowHeight*this.maxRows+this.margin*2+this.topMargin+this.bottomMargin;
			double hope = h;
			h = System.Math.Max(h, hMin);
			h = System.Math.Min(h, hMax);
			if ( h == this.Height )  return false;

			if ( mode == ScrollArrayAdjust.MoveUp )
			{
				this.Top = this.Bottom+h;
			}
			if ( mode == ScrollArrayAdjust.MoveDown )
			{
				this.Bottom = this.Top-h;
			}
			this.Invalidate();
			if ( h != hope )  AdjustToMultiple(mode);
			return true;
		}


		// Appelé lorsque l'ascenseur a bougé.
		private void HandleScrollerV(object sender)
		{
			this.FirstRow = (int)System.Math.Floor(this.scrollerV.Value+0.5);
			//this.SetFocused(true);
		}

		// Appelé lorsque l'ascenseur a bougé.
		private void HandleScrollerH(object sender)
		{
			this.OffsetH = System.Math.Floor(this.scrollerH.Value);
			//this.SetFocused(true);
		}

		// Appelé lorsque le bouton d'en-tête a été cliqué.
		private void HandleButtonClicked(object sender, MessageEventArgs e)
		{
			foreach ( HeaderButton button in this.headerButton )
			{
				if ( sender == button )
				{
					int column = button.Rank;
					int mode = button.SortMode;
					switch ( mode )
					{
						case -1:  mode =  1;  break;
						case  0:  mode =  1;  break;
						case  1:  mode = -1;  break;
					}
					this.SetHeaderSort(column, mode);
					this.OnSortChanged();
					return;
				}
			}
		}

		// Appelé lorsque le slider va être déplacé.
		private void HandleSliderDragStarted(object sender, MessageEventArgs e)
		{
			HeaderSlider slider = sender as HeaderSlider;
			this.DragStartedColumn(slider.Rank, e.Point.X);
		}

		// Appelé lorsque le slider est déplacé.
		private void HandleSliderDragMoved(object sender, MessageEventArgs e)
		{
			HeaderSlider slider = sender as HeaderSlider;
			this.DragMovedColumn(slider.Rank, e.Point.X);
		}

		// Appelé lorsque le slider est fini de déplacer.
		private void HandleSliderDragEnded(object sender, MessageEventArgs e)
		{
			HeaderSlider slider = sender as HeaderSlider;
			this.DragEndedColumn(slider.Rank, e.Point.X);
		}

		// La largeur d'une colonne va être modifiée.
		protected void DragStartedColumn(int column, double pos)
		{
			this.dragRank = column;
			this.dragPos  = pos;
			this.dragDim  = this.RetWidthColumn(column);
		}

		// Modifie la largeur d'une colonne.
		protected void DragMovedColumn(int column, double pos)
		{
			double newWidth = this.RetWidthColumn(column) + pos-this.dragPos;
			this.SetWidthColumn(this.dragRank, newWidth);
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
					this.mouseDown = true;
					this.MouseSelect(pos.Y);
					break;
				
				case MessageType.MouseMove:
					if ( this.mouseDown  )
					{
						this.MouseSelect(pos.Y);
					}
					break;

				case MessageType.MouseUp:
					if ( this.mouseDown )
					{
						this.MouseSelect(pos.Y);
						this.mouseDown = false;
					}
					break;

				case MessageType.KeyDown:
					ProcessKeyDown(message.KeyCodeAsKeys, message.IsShiftPressed, message.IsCtrlPressed);
					break;
			}
			
			message.Consumer = this;
		}

		// Sélectionne la ligne selon la souris.
		protected bool MouseSelect(double pos)
		{
			pos = this.Client.Height-pos;
			int line = (int)((pos-this.margin-this.topMargin)/this.rowHeight);
			if ( line < 0 || line >= this.visibleRows )  return false;
			line += this.firstRow;
			if ( line > this.maxRows-1 )  return false;
			this.SelectedIndex = line;
			return true;
		}

		// Gestion d'une touche pressée avec KeyDown dans la liste.
		protected void ProcessKeyDown(System.Windows.Forms.Keys key, bool isShiftPressed, bool isCtrlPressed)
		{
			int		sel;

			switch ( key )
			{
				case Keys.Up:
					sel = this.SelectedIndex-1;
					if ( sel >= 0 )
					{
						this.SelectedIndex = sel;
						if ( !this.IsShowSelect() )  this.ShowSelect(ScrollArrayShow.Extremity);
					}
					break;

				case Keys.Down:
					sel = this.SelectedIndex+1;
					if ( sel < this.maxRows )
					{
						this.SelectedIndex = sel;
						if ( !this.IsShowSelect() )  this.ShowSelect(ScrollArrayShow.Extremity);
					}
					break;
			}
		}


		// Demande de régénérer tout le contenu.
		public void RefreshContent()
		{
			this.lastVisibleRows = -1;
			this.isDirty = true;
		}

		// Met à jour la géométrie du tableau.
		protected void Update()
		{
			if ( this.widthColumns == null )  return;
			if ( !this.isDirty )  return;
			this.UpdateClientGeometry();
		}

		// Met à jour la géométrie de la liste.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			if ( this.widthColumns == null )  return;
			if ( this.scrollerV == null || this.scrollerH == null )  return;
			
			this.isDirty = false;

			this.topMargin = this.rowHeight;
			this.rightMargin = this.scrollerV.Width;
			this.bottomMargin = this.scrollerH.Height;

			Drawing.Rectangle rect = new Drawing.Rectangle(0, 0, this.Width, this.Height);
			rect.Inflate(-this.margin, -this.margin);

			this.rectInside.Left   = rect.Left  +this.leftMargin;
			this.rectInside.Right  = rect.Right -this.rightMargin;
			this.rectInside.Bottom = rect.Bottom+this.bottomMargin;
			this.rectInside.Top    = rect.Top   -this.topMargin;

			double v = this.rectInside.Height/this.rowHeight;
			this.visibleRows = (int)System.Math.Ceiling(v);  // compte la dernière ligne partielle
			this.visibleRowsFull = (int)System.Math.Floor(v);  // nb de lignes entières

			// Alloue le tableau des textes.
			int dx = System.Math.Max(this.visibleRows, 1);
			int dy = System.Math.Max(this.maxColumns, 1);
			if ( dx != this.lastDx || dy != this.lastDy )
			{
				this.textLayouts = new TextLayout[dx, dy];
				this.lastDx = dx;
				this.lastDy = dy;
				this.lastVisibleRows = -1;
			}

			// Place l'en-tête
			Drawing.Rectangle aRect = new Drawing.Rectangle();
			aRect.Left   = this.rectInside.Left;
			aRect.Right  = this.rectInside.Right;
			aRect.Bottom = this.rectInside.Top;
			aRect.Top    = this.rectInside.Top+this.rowHeight;
			this.header.Bounds = aRect;
			this.header.Children.Clear();

			// Place les boutons dans l'en-tête.
			aRect.Bottom = 0;
			aRect.Top    = this.topMargin;
			aRect.Left   = -this.offsetH;
			for ( int i=0 ; i<this.maxColumns ; i++ )
			{
				aRect.Right = aRect.Left+this.RetWidthColumn(i);
				HeaderButton button = this.FindButton(i);
				button.Show();
				button.Bounds = aRect;
				this.header.Children.Add(button);
				aRect.Left = aRect.Right;
			}

			// Place les sliders dans l'en-tête.
			aRect.Bottom = 0;
			aRect.Top    = this.topMargin;
			aRect.Left   = -this.offsetH;
			for ( int i=0 ; i<this.maxColumns ; i++ )
			{
				aRect.Right = aRect.Left+this.RetWidthColumn(i);
				HeaderSlider slider = this.FindSlider(i);
				Drawing.Rectangle sRect = new Drawing.Rectangle();
				sRect.Left   = aRect.Right-this.sliderDim/2;
				sRect.Right  = aRect.Right+this.sliderDim/2;
				sRect.Bottom = aRect.Bottom;
				sRect.Top    = aRect.Top;
				slider.Show();
				slider.Bounds = sRect;
				this.header.Children.Add(slider);
				aRect.Left = aRect.Right;
			}

			// Place l'ascenseur vertical.
			aRect.Left   = this.rectInside.Right;
			aRect.Right  = this.rectInside.Right+this.rightMargin;
			aRect.Bottom = this.rectInside.Bottom;
			aRect.Top    = this.rectInside.Top;
			this.scrollerV.Bounds = aRect;

			// Place l'ascenseur horizontal.
			aRect.Left   = this.rectInside.Left;
			aRect.Right  = this.rectInside.Right;
			aRect.Bottom = this.rectInside.Bottom-this.bottomMargin;
			aRect.Top    = this.rectInside.Bottom;
			this.scrollerH.Bounds = aRect;

			this.UpdateScroller();
		}

		// Met à jour l'ascenseur en fonction de la liste.
		protected void UpdateScroller()
		{
			// Met à jour l'ascenseur vertical.
			int total = this.maxRows;
			if ( total <= this.visibleRowsFull )
			{
				this.scrollerV.SetEnabled(false);
				this.scrollerV.Range = 1;
				this.scrollerV.Display = 1;
				this.scrollerV.Value = 0;
			}
			else
			{
				this.scrollerV.SetEnabled(true);
				this.scrollerV.Range = total-this.visibleRowsFull;
				this.scrollerV.Display = this.scrollerV.Range*((double)this.visibleRowsFull/total);
				this.scrollerV.Value = this.firstRow;
				this.scrollerV.SmallChange = 1;
				this.scrollerV.LargeChange = this.visibleRowsFull/2;
			}
			this.UpdateTextlayouts();

			// Met à jour l'ascenseur horizontal.
			double width = this.widthTotal;
			if ( width <= this.rectInside.Width )
			{
				this.scrollerH.SetEnabled(false);
				this.scrollerH.Range = 1;
				this.scrollerH.Display = 1;
				this.scrollerH.Value = 0;
			}
			else
			{
				this.scrollerH.SetEnabled(true);
				this.scrollerH.Range = width-this.rectInside.Width;
				this.scrollerH.Display = this.rectInside.Width/width * this.scrollerH.Range;
				this.scrollerH.Value = this.offsetH;
				this.scrollerH.SmallChange = 10;
				this.scrollerH.LargeChange = this.rectInside.Width/2;
			}
		}

		// Met à jour les textes en fonction de l'ascenseur vertical.
		protected void UpdateTextlayouts()
		{
			if ( this.widthColumns == null )  return;

			int max = System.Math.Min(this.visibleRows, this.maxRows);
			bool quick = ( max == this.lastVisibleRows && this.firstRow == this.lastFirstRow );

			this.lastVisibleRows = max;
			this.lastFirstRow = this.firstRow;

			for ( int row=0 ; row<max ; row++ )
			{
				for ( int column=0 ; column<this.maxColumns ; column++ )
				{
					if ( !quick )
					{
						if ( this.textLayouts[row,column] == null )
						{
							this.textLayouts[row,column] = new TextLayout();
						}
						if ( this.funcFillText == null )
						{
							this.textLayouts[row,column].Text = this.GetText(row+this.firstRow, column);
						}
						else
						{
							this.textLayouts[row,column].Text = this.funcFillText(row+this.firstRow, column);
						}
						this.textLayouts[row,column].Font = this.DefaultFont;
						this.textLayouts[row,column].FontSize = this.DefaultFontSize;
					}
					this.textLayouts[row,column].LayoutSize = new Drawing.Size(this.widthColumns[column]-this.textMargin*2, this.rowHeight);
					this.textLayouts[row,column].Alignment = this.alignmentColumns[column];
					this.textLayouts[row,column].BreakMode = Drawing.TextBreakMode.Ellipsis | Drawing.TextBreakMode.SingleLine;
				}
			}
		}


		// Génère un événement pour dire que la sélection dans la liste a changé.
		protected void OnSelectedIndexChanged()
		{
			if ( this.SelectedIndexChanged != null )  // qq'un écoute ?
			{
				this.SelectedIndexChanged(this);
			}
		}

		// Génère un événement pour dire que le tri a changé.
		protected void OnSortChanged()
		{
			if ( this.SortChanged != null )  // qq'un écoute ?
			{
				this.SortChanged(this);
			}
		}


		protected HeaderButton FindButton(int index)
		{
			return this.headerButton[index] as HeaderButton;
		}

		protected HeaderSlider FindSlider(int index)
		{
			return this.headerSlider[index] as HeaderSlider;
		}

		
		// Dessine la liste.
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			IAdorner adorner = Widgets.Adorner.Factory.Active;
			if ( this.widthColumns == null )  return;

			Drawing.Rectangle rect  = new Drawing.Rectangle(0, 0, this.Client.Width, this.Client.Height);
			WidgetState       state = this.PaintState;
			Direction         dir   = this.RootDirection;
			
			adorner.PaintTextFieldBackground(graphics, rect, state, dir, TextFieldStyle.Normal, false);

			Drawing.Rectangle localClip = this.MapClientToRoot(this.rectInside);
			Drawing.Rectangle saveClip  = graphics.SaveClippingRectangle();
			graphics.SetClippingRectangle(localClip);

			// Dessine le tableau des textes.
			this.Update();
			Drawing.Point pos = new Drawing.Point(this.rectInside.Left, this.rectInside.Top-this.rowHeight);

			double limit = this.widthTotal-this.offsetH+this.rectInside.Left+1;
			double maxx = System.Math.Min(this.rectInside.Right, limit);
			
			int max = System.Math.Min(this.visibleRows, this.maxRows);
			for ( int row=0 ; row<max ; row++ )
			{
				pos.X = this.margin;
				WidgetState widgetState = WidgetState.Enabled;

				if ( row+this.firstRow == this.selectedRow )  // ligne sélectionnée ?
				{
					Drawing.Rectangle[] rects = new Drawing.Rectangle[1];
					rects[0].Left   = this.rectInside.Left;
					rects[0].Right  = maxx;
					rects[0].Bottom = pos.Y;
					rects[0].Top    = pos.Y+this.rowHeight;
					adorner.PaintTextSelectionBackground(graphics, new Drawing.Point(0,0), rects);

					widgetState |= WidgetState.Selected;
				}

				pos.X += this.textMargin-System.Math.Floor(this.offsetH);
				for ( int column=0 ; column<this.maxColumns ; column++ )
				{
					double endx = pos.X+this.widthColumns[column];
					if ( pos.X < localClip.Right && endx > localClip.Left )
					{
						adorner.PaintGeneralTextLayout(graphics, pos, this.textLayouts[row,column], widgetState, Direction.None);
					}
					pos.X = endx;
				}
				pos.Y -= this.rowHeight;
			}

			rect = this.rectInside;
			rect.Inflate(-0.5, -0.5);

			graphics.LineWidth = 1;
			Drawing.Color color;
			color = Drawing.Color.FromRGB(0.9,0.9,0.9);  // gris-clair

			// Dessine le rectangle englobant.
			graphics.AddRectangle(rect);
			graphics.RenderSolid(color);

			// Dessine les lignes de séparation horizontales.
			if ( true )
			{
				double x1 = this.rectInside.Left;
				double x2 = maxx;
				double y  = this.rectInside.Top-0.5;
				for ( int i=0 ; i<max ; i++ )
				{
					y -= this.rowHeight;
					graphics.AddLine(x1, y, x2, y);
					graphics.RenderSolid(color);
				}
			}

			// Dessine les lignes de séparation verticales.
			if ( true )
			{
				limit = this.maxRows*this.rowHeight;
				limit = this.rectInside.Top-(limit-this.firstRow*this.rowHeight);
				double y1 = System.Math.Max(this.rectInside.Bottom, limit);
				double y2 = this.rectInside.Top;
				double x  = this.rectInside.Left-this.offsetH+0.5;
				for ( int i=0 ; i<this.maxColumns ; i++ )
				{
					x += this.RetWidthColumn(i);
					if ( x < this.rectInside.Left || x > this.rectInside.Right )  continue;
					graphics.AddLine(x, y1, x, y2);
					graphics.RenderSolid(color);
				}
			}

			graphics.RestoreClippingRectangle(saveClip);
		}


		public event EventHandler SelectedIndexChanged;
		public event EventHandler SortChanged;

		public delegate string FillText(int row, int column);

		protected bool							isDirty;
		protected bool							mouseDown = false;
		protected int							maxRows = 0;
		protected int							maxColumns = 0;
		protected System.Collections.ArrayList	array = new System.Collections.ArrayList();
		protected FillText						funcFillText = null;
		protected TextLayout[,]					textLayouts;
		protected double						defWidth = 100;	// largeur par défaut
		protected double						minWidth = 10;	// largeur minimale
		protected double[]						widthColumns;	// largeur des colonnes
		protected double						widthTotal;		// largeur totale
		protected Drawing.ContentAlignment[]	alignmentColumns;
		protected double						margin = 3;
		protected double						textMargin = 2;
		protected double						rowHeight = 16;
		protected double						sliderDim = 6;
		protected double						leftMargin = 0;		// marge pour en-tête
		protected double						rightMargin = 0;	// marge pour ascenseur
		protected double						bottomMargin = 0;	// marge pour ascenseur
		protected double						topMargin = 0;		// marge pour en-tête
		protected Drawing.Rectangle				rectInside = new Drawing.Rectangle();
		protected Widget						header;		// père de l'en-tête horizontale
		protected System.Collections.ArrayList	headerButton = new System.Collections.ArrayList();
		protected System.Collections.ArrayList	headerSlider = new System.Collections.ArrayList();
		protected VScroller						scrollerV;
		protected HScroller						scrollerH;
		protected int							visibleRows;
		protected int							visibleRowsFull;
		protected int							firstRow = 0;
		protected int							selectedRow = -1;
		protected double						offsetH = 0;
		protected int							dragRank;
		protected double						dragPos;
		protected double						dragDim;
		protected int							lastDx;
		protected int							lastDy;
		protected int							lastVisibleRows;
		protected int							lastFirstRow;
	}
}
