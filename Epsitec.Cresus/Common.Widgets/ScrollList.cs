namespace Epsitec.Common.Widgets
{
	public enum ScrollListStyle
	{
		Flat,							// pas de cadre, ni de relief
		Normal,							// bouton normal
		Simple,							// cadre tout simple
	}

	public enum ScrollListShow
	{
		Extremity,		// d�placement minimal aux extr�mit�s
		Middle,			// d�placement central
	}

	public enum ScrollListAdjust
	{
		MoveUp,			// d�place le haut
		MoveDown,		// d�place le bas
	}

	/// <summary>
	/// La classe ScrollList r�alise une liste d�roulante simple.
	/// </summary>
	public class ScrollList : Widget, Helpers.IStringCollectionHost
	{
		public ScrollList()
		{
			this.items = new Helpers.StringCollection (this);
			this.DockMargins = new Drawing.Margins (2, 2, 2, 2);
			this.internalState |= InternalState.AutoFocus;
			this.internalState |= InternalState.Focusable;

			this.scrollListStyle = ScrollListStyle.Normal;
			this.lineHeight = this.DefaultFontHeight+1;
			this.scroller = new VScroller();
			this.scroller.IsInverted = true;
			this.scroller.Parent = this;
			this.scroller.Dock = DockStyle.Right;
			this.scroller.Moved += new EventHandler(this.HandleScroller);
			this.scroller.Hide ();
		}

		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				System.Diagnostics.Debug.WriteLine("Dispose ScrollList " + this.Text);
				
				this.scroller.Moved -= new EventHandler(this.HandleScroller);
			}
			
			base.Dispose(disposing);
		}

		public Helpers.StringCollection Items
		{
			get { return this.items; }
		}

		public ScrollListStyle ScrollListStyle
		{
			get
			{
				return this.scrollListStyle;
			}

			set
			{
				if ( this.scrollListStyle != value )
				{
					this.scrollListStyle = value;
					this.Invalidate();
				}
			}
		}

		public bool ComboMode
		{
			set
			{
				this.isComboList = value;
			}

			get
			{
				return this.isComboList;
			}
		}
		
		public AbstractScroller Scroller
		{
			get { return this.scroller; }
		}


		// Donne un texte de la liste.
		public string GetText(int index)
		{
			if ( index < 0 || index >= this.items.Count )  return "";
			return this.items[index];
		}

		// Ligne s�lectionn�e, -1 si aucune.
		public int SelectedIndex
		{
			get
			{
				return this.selectedLine;
			}

			set
			{
				if ( value != -1 )
				{
					value = System.Math.Max(value, 0);
					value = System.Math.Min(value, this.items.Count-1);
				}
				if ( value != this.selectedLine )
				{
					this.selectedLine = value;
					this.isDirty = true;
					this.Invalidate();
					if ( !this.isComboList )
					{
						this.OnSelectedIndexChanged();
					}
				}
			}
		}

		// Premi�re ligne visible.
		public int FirstVisibleLine
		{
			get
			{
				return this.firstLine;
			}

			set
			{
				value = System.Math.Max(value, 0);
				value = System.Math.Min(value, System.Math.Max(this.items.Count-this.visibleLines, 0));
				if ( value != this.firstLine )
				{
					this.firstLine = value;
					this.isDirty = true;
					this.Invalidate();
				}
			}
		}

		// Indique si la ligne s�lectionn�e est visible.
		public bool IsShowSelect()
		{
			if ( this.selectedLine == -1 )  return true;
			if ( this.selectedLine >= this.firstLine &&
				 this.selectedLine <  this.firstLine+this.visibleLines )  return true;
			return false;
		}

		// Rend la ligne s�lectionn�e visible.
		public void ShowSelect(ScrollListShow mode)
		{
			if ( this.selectedLine == -1 )  return;

			int fl = this.FirstVisibleLine;
			if ( mode == ScrollListShow.Extremity )
			{
				if ( this.selectedLine < this.firstLine )
				{
					fl = this.selectedLine;
				}
				if ( this.selectedLine > this.firstLine+this.visibleLines-1 )
				{
					fl = this.selectedLine-(this.visibleLines-1);
				}
			}
			if ( mode == ScrollListShow.Middle )
			{
				int display = System.Math.Min(this.visibleLines, this.items.Count);
				fl = System.Math.Min(this.selectedLine+display/2, this.items.Count-1);
				fl = System.Math.Max(fl-display+1, 0);
			}
			this.FirstVisibleLine = fl;
		}

		// Ajuste la hauteur pour afficher pile un nombre entier de lignes.
		public bool AdjustToMultiple(ScrollListAdjust mode)
		{
			double h = this.Height-ScrollList.Margin*2;
			int nbLines = (int)(h/this.lineHeight);
			double adjust = h - nbLines*this.lineHeight;
			if ( adjust == 0 )  return false;

			if ( mode == ScrollListAdjust.MoveUp )
			{
				this.Top = System.Math.Floor(this.Top-adjust);
			}
			if ( mode == ScrollListAdjust.MoveDown )
			{
				this.Bottom = System.Math.Floor(this.Bottom+adjust);
			}
			this.Invalidate();
			return true;
		}

		// Ajuste la hauteur pour afficher exactement le nombre de lignes contenues.
		public bool AdjustToContent(ScrollListAdjust mode, double hMin, double hMax)
		{
			double h = this.lineHeight*this.items.Count+ScrollList.Margin*2;
			double hope = h;
			h = System.Math.Max(h, hMin);
			h = System.Math.Min(h, hMax);
			if ( h == this.Height )  return false;

			if ( mode == ScrollListAdjust.MoveUp )
			{
				this.Top = this.Bottom+h;
			}
			if ( mode == ScrollListAdjust.MoveDown )
			{
				this.Bottom = this.Top-h;
			}
			this.Invalidate();
			if ( h != hope )  AdjustToMultiple(mode);
			return true;
		}


		// Appel� lorsque l'ascenseur a boug�.
		private void HandleScroller(object sender)
		{
			this.FirstVisibleLine = (int)this.scroller.Value;
			//this.SetFocused(true);
		}


		// Gestion d'un �v�nement.
		protected override void ProcessMessage(Message message, Drawing.Point pos)
		{
			switch ( message.Type )
			{
				case MessageType.MouseDown:
					this.mouseDown = true;
					this.MouseSelect(pos);
					break;
				
				case MessageType.MouseMove:
					if ( this.mouseDown || this.isComboList )
					{
						this.MouseSelect(pos);
					}
					break;

				case MessageType.MouseUp:
					if ( this.mouseDown )
					{
						this.MouseSelect(pos);
						if ( this.isComboList )
						{
							this.OnSelectedIndexChanged();
						}
						this.mouseDown = false;
					}
					break;

				case MessageType.KeyDown:
					//System.Diagnostics.Debug.WriteLine("KeyDown "+message.KeyChar+" "+message.KeyCode);
					ProcessKeyDown(message.KeyCode, message.IsShiftPressed, message.IsCtrlPressed);
					break;
			}
			
			message.Consumer = this;
		}

		// S�lectionne la ligne selon la souris.
		protected bool MouseSelect(Drawing.Point pos)
		{
			double y = this.Client.Height-pos.Y-1-ScrollList.Margin;
			double x = pos.X-ScrollList.Margin;
			
			if (y < 0) return false;
			if (y >= this.visibleLines*this.lineHeight) return false;
			if (x < 0) return false;
			if (x >= this.Client.Width-ScrollList.Margin*2-this.rightMargin) return false;
			
			int line = (int)(y/this.lineHeight);
			
			System.Diagnostics.Debug.Assert(line >= 0.0);
			System.Diagnostics.Debug.Assert(line < this.visibleLines);
			
			this.SelectedIndex = this.firstLine+line;
			return true;
		}

		// Gestion d'une touche press�e avec KeyDown dans la liste.
		protected void ProcessKeyDown(int key, bool isShiftPressed, bool isCtrlPressed)
		{
			int		sel;

			switch ( key )
			{
				case (int)System.Windows.Forms.Keys.Up:
					sel = this.SelectedIndex-1;
					if ( sel >= 0 )
					{
						this.SelectedIndex = sel;
						if ( !this.IsShowSelect() )  this.ShowSelect(ScrollListShow.Extremity);
					}
					break;

				case (int)System.Windows.Forms.Keys.Down:
					sel = this.SelectedIndex+1;
					if ( sel < this.items.Count )
					{
						this.SelectedIndex = sel;
						if ( !this.IsShowSelect() )  this.ShowSelect(ScrollListShow.Extremity);
					}
					break;

				case (int)System.Windows.Forms.Keys.Return:
				case (int)System.Windows.Forms.Keys.Space:
					if ( this.isComboList )
					{
						this.OnSelectedIndexChanged();
					}
					break;
			}
		}


		// Met � jour l'ascenseur en fonction de la liste.
		protected void UpdateScroller()
		{
			int total = this.items.Count;
			if ( total <= this.visibleLines )
			{
				if (this.scroller.IsVisible)
				{
					this.scroller.Hide();
					this.UpdateClientGeometry();
				}
			}
			else
			{
				this.scroller.Range = total-this.visibleLines;
				this.scroller.Display = this.scroller.Range*((double)this.visibleLines/total);
				this.scroller.Value = this.firstLine;
				this.scroller.SmallChange = 1;
				this.scroller.LargeChange = this.visibleLines/2;
				
				if (this.scroller.IsVisible == false)
				{
					this.scroller.Show();
					this.UpdateClientGeometry();
				}
			}
		}

		// Met � jour les textes.
		protected void UpdateTextLayouts()
		{
			if (this.isDirty)
			{
				this.UpdateScroller ();
				int max = System.Math.Min(this.visibleLines, this.items.Count);
				for ( int i=0 ; i<max ; i++ )
				{
					if ( this.textLayouts[i] == null )
					{
						this.textLayouts[i] = new TextLayout();
					}
					this.textLayouts[i].Text = this.items[i+this.firstLine];
					this.textLayouts[i].Font = this.DefaultFont;
					this.textLayouts[i].FontSize = this.DefaultFontSize;
					this.textLayouts[i].LayoutSize = new Drawing.Size(this.Client.Width-ScrollList.Margin*2-this.rightMargin, this.lineHeight);
				}
				this.isDirty = false;
			}
		}


		// Met � jour la g�om�trie de l'ascenseur de la liste.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();
			
			if ( this.lineHeight == 0 )  return;

			Drawing.Rectangle rect = this.Bounds;
			rect.Inflate(-ScrollList.Margin, -ScrollList.Margin);

			this.visibleLines = (int)(rect.Height/this.lineHeight);
			this.textLayouts = new TextLayout[this.visibleLines];

			if ( this.scroller.IsVisible )
			{
				this.rightMargin = this.scroller.Width;
			}
			else
			{
				this.rightMargin = 0;
			}
			
			this.isDirty = true;
			this.Invalidate ();
		}


		// G�n�re un �v�nement pour dire que la s�lection dans la liste a chang�.
		protected virtual void OnSelectedIndexChanged()
		{
			if ( this.SelectedIndexChanged != null )  // qq'un �coute ?
			{
				this.SelectedIndexChanged(this);
			}
		}


		// Dessine la liste.
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			this.UpdateTextLayouts ();
			
			IAdorner adorner = Widgets.Adorner.Factory.Active;

			Drawing.Rectangle rect  = new Drawing.Rectangle(0, 0, this.Client.Width, this.Client.Height);
			WidgetState       state = this.PaintState;
			Direction         dir   = this.RootDirection;
			
			TextFieldStyle style = TextFieldStyle.Normal;
			switch ( this.scrollListStyle )
			{
				case ScrollListStyle.Flat:    style = TextFieldStyle.Flat;    break;
				case ScrollListStyle.Simple:  style = TextFieldStyle.Simple;  break;
			}
			adorner.PaintTextFieldBackground(graphics, rect, state, dir, style, false);

			Drawing.Point pos = new Drawing.Point(ScrollList.Margin, rect.Height-ScrollList.Margin-this.lineHeight);
			int max = System.Math.Min(this.visibleLines, this.items.Count);
			for ( int i=0 ; i<max ; i++ )
			{
				if ( this.textLayouts[i] == null )  break;

				Drawing.Color color = Drawing.Color.Empty;

				if ( i+this.firstLine == this.selectedLine &&
					(state&WidgetState.Enabled) != 0 )
				{
					Drawing.Rectangle[] rects = new Drawing.Rectangle[1];
					rects[0].Left   = ScrollList.Margin;
					rects[0].Width  = this.Client.Width-2*ScrollList.Margin-this.rightMargin;
					rects[0].Bottom = pos.Y;
					rects[0].Height = this.lineHeight;
					adorner.PaintTextSelectionBackground(graphics, new Drawing.Point(0,0), rects);

					color = Drawing.Color.FromName("ActiveCaptionText");
					state |= WidgetState.Selected;
				}
				else
				{
					state &= ~WidgetState.Selected;
				}

				//this.textLayouts[i].Paint(pos, graphics, Drawing.Rectangle.Empty, color);
				adorner.PaintButtonTextLayout(graphics, pos, this.textLayouts[i], state, dir, ButtonStyle.ListItem);
				pos.Y -= this.lineHeight;
			}
		}
		
		
		#region IStringCollectionHost Members
		public void StringCollectionChanged()
		{
			if (this.items.Count == 0)
			{
				this.FirstVisibleLine = 0;
				this.SelectedIndex = -1;
			}
			
			this.isDirty = true;
			this.Invalidate();
		}
		#endregion


		public event EventHandler SelectedIndexChanged;

		protected static readonly double		Margin = 3;
		
		protected ScrollListStyle				scrollListStyle;
		protected bool							isComboList = false;
		protected bool							isDirty;
		protected bool							mouseDown = false;
		protected Helpers.StringCollection		items;
		protected TextLayout[]					textLayouts;
		protected double						rightMargin;
		protected double						lineHeight;
		protected VScroller						scroller;
		protected int							visibleLines;
		protected int							firstLine = 0;
		protected int							selectedLine = -1;
	}
}
