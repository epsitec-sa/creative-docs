namespace Epsitec.Common.Widgets
{
	public enum ScrollListStyle
	{
		Normal,			// liste fixe normale
		Menu,			// menu d'un TextFieldCombo
	}

	/// <summary>
	/// La classe ScrollList r�alise une liste d�roulante simple.
	/// </summary>
	public class ScrollList : Widget, Helpers.IStringCollectionHost, Support.Data.INamedStringSelection
	{
		public ScrollList()
		{
			if ( Support.ObjectBundler.IsBooting )
			{
				//	N'initialise rien, car cela prend passablement de temps... et de toute
				//	mani�re, on n'a pas besoin de toutes ces informations pour pouvoir
				//	utiliser IBundleSupport.
				
				return;
			}
			
			this.items = new Helpers.StringCollection(this);
			this.DockPadding = new Drawing.Margins(2, 2, 2, 2);
			this.InternalState |= InternalState.AutoFocus;
			this.InternalState |= InternalState.AutoDoubleClick;
			this.InternalState |= InternalState.Focusable;

			this.scrollListStyle = ScrollListStyle.Normal;
			this.lineHeight = this.DefaultFontHeight+1;
			this.scroller = new VScroller(null);
			this.scroller.IsInverted = true;
			this.scroller.Parent = this;
			this.scroller.ValueChanged += new Support.EventHandler(this.HandleScrollerValueChanged);
			this.scroller.Hide();
			this.UpdateMargins();
		}
		
		public ScrollList(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}

		
		#region Interface IBundleSupport
		public override void RestoreFromBundle(Epsitec.Common.Support.ObjectBundler bundler, Epsitec.Common.Support.ResourceBundle bundle)
		{
			base.RestoreFromBundle (bundler, bundle);
			this.items.RestoreFromBundle ("items", bundler, bundle);
		}
		
		public override void SerializeToBundle(Support.ObjectBundler bundler, Support.ResourceBundle bundle)
		{
			base.SerializeToBundle (bundler, bundle);
			this.items.SerializeToBundle ("items", bundler, bundle);
		}
		#endregion
		
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if ( this.scroller != null )
				{
					this.scroller.ValueChanged -= new Support.EventHandler(this.HandleScrollerValueChanged);
				}
			}
			
			base.Dispose (disposing);
		}


		public ScrollListStyle					ScrollListStyle
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

		public AbstractScroller					Scroller
		{
			get { return this.scroller; }
		}

		public int								FirstVisibleRow
		{
			// Premi�re ligne visible.
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
					this.SetDirty();
					this.Invalidate();
				}
			}
		}
		
		public int								VisibleRowCount
		{
			get
			{
				return this.visibleLines;
			}
		}

		public int								FullyVisibleRowCount
		{
			get
			{
				return this.visibleLines;
			}
		}

		public int								RowCount
		{
			get
			{
				return this.Items.Count;
			}
		}
		
		
		public void ShowSelected(ScrollShowMode mode)
		{
			// Rend la ligne s�lectionn�e visible.
			
			if ( this.selectedLine == -1 ) return;
			if ( this.selectedLine >= this.firstLine && this.selectedLine <  this.firstLine+this.visibleLines ) return;
			
			int fl = this.FirstVisibleRow;
			if ( mode == ScrollShowMode.Extremity )
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
			if ( mode == ScrollShowMode.Center )
			{
				int display = System.Math.Min(this.visibleLines, this.items.Count);
				fl = System.Math.Min(this.selectedLine+display/2, this.items.Count-1);
				fl = System.Math.Max(fl-display+1, 0);
			}
			this.FirstVisibleRow = fl;
		}

		
		public bool AdjustHeight(ScrollAdjustMode mode)
		{
			// Ajuste la hauteur pour afficher pile un nombre entier de lignes.
			
			double h = this.Client.Height-ScrollList.TextOffsetY*2;
			int count = (int)(h/this.lineHeight);
			
			return this.AdjustHeightToRows (mode, count);
		}

		public bool AdjustHeightToContent(ScrollAdjustMode mode, double min_height, double max_height)
		{
			// Ajuste la hauteur pour afficher exactement le nombre de lignes contenues.
			
			double h = this.lineHeight*this.items.Count+ScrollList.TextOffsetY*2;
			double hope = h;
			h = System.Math.Max(h, min_height);
			h = System.Math.Min(h, max_height);
			
			if (h == this.Height)
			{
				return false;
			}

			switch (mode)
			{
				case ScrollAdjustMode.MoveTop:
					this.Top = this.Bottom + h;
					break;
				
				case ScrollAdjustMode.MoveBottom:
					this.Bottom = this.Top - h;
					break;
				
				default:
					throw new System.NotSupportedException (string.Format ("Adjust mode {0} not supported.", mode));
			}
			
			if (h == hope)
			{
				this.Invalidate ();
			}
			else
			{
				this.AdjustHeight (mode);
			}
			return true;
		}
		
		public bool AdjustHeightToRows(ScrollAdjustMode mode, int count)
		{
			//	Ajuste la hauteur pour afficher exactement le nombre de lignes sp�cifi�.
			
			double h = this.Client.Height-ScrollList.TextOffsetY*2;
			double adjust = h - count*this.lineHeight;
			
			if (adjust == 0)
			{
				return false;
			}
			
			switch (mode)
			{
				case ScrollAdjustMode.MoveTop:
					this.Top = System.Math.Floor (this.Top - adjust);
					break;
				
				case ScrollAdjustMode.MoveBottom:
					this.Bottom = System.Math.Floor (this.Bottom + adjust);
					break;
				
				default:
					throw new System.NotSupportedException (string.Format ("Adjust mode {0} not supported.", mode));
			}
			
			this.Invalidate();
			return true;
		}
		
		
		public override Drawing.Rectangle GetShapeBounds()
		{
			IAdorner adorner = Widgets.Adorner.Factory.Active;
			Drawing.Rectangle rect = this.Client.Bounds;
			rect.Inflate(adorner.GeometryListShapeBounds);
			if ( this.scrollListStyle == ScrollListStyle.Menu )
			{
				rect.Inflate(adorner.GeometryMenuShadow);
			}
			return rect;
		}

		protected override void ProcessMessage(Message message, Drawing.Point pos)
		{
			// Gestion d'un �v�nement.
			
			switch ( message.Type )
			{
				case MessageType.MouseEnter:
				case MessageType.MouseLeave:
					break;
				
				case MessageType.MouseDown:
					this.mouseDown = true;
					this.MouseSelect(pos);
					break;
				
				case MessageType.MouseMove:
					if ( this.mouseDown || this.scrollListStyle == ScrollListStyle.Menu )
					{
						this.MouseSelect(pos);
					}
					break;

				case MessageType.MouseUp:
					if ( this.mouseDown )
					{
						this.MouseSelect(pos);
						this.OnSelectionActivated();
						this.mouseDown = false;
					}
					break;

				case MessageType.MouseWheel:
					if ( message.Wheel < 0 )  this.FirstVisibleRow ++;
					if ( message.Wheel > 0 )  this.FirstVisibleRow --;
					break;

				case MessageType.KeyDown:
					if (!this.ProcessKeyEvent (message))
					{
						return;
					}
					break;
				
				default:
					return;
			}
			
			message.Consumer = this;
		}

		protected virtual  bool MouseSelect(Drawing.Point pos)
		{
			// S�lectionne la ligne selon la souris.
			
			double y = this.Client.Height-pos.Y-1-ScrollList.TextOffsetY;
			double x = pos.X-this.margins.Left;
			
			if (y < 0) return false;
			if (y >= this.visibleLines*this.lineHeight) return false;
			if (x < 0) return false;
			if (x >= this.Client.Width-this.margins.Width) return false;
			
			int line = (int)(y/this.lineHeight);
			
			System.Diagnostics.Debug.Assert(line >= 0.0);
			System.Diagnostics.Debug.Assert(line < this.visibleLines);
			
			this.SelectedIndex = this.firstLine+line;
			return true;
		}

		protected virtual bool ProcessKeyEvent(Message message)
		{
			if ((message.IsAltPressed) ||
				(message.IsShiftPressed) ||
				(message.IsCtrlPressed))
			{
				return false;
			}
			
			// Gestion d'une touche press�e avec KeyDown dans la liste.
			
			int sel = this.SelectedIndex;
			
			switch (message.KeyCode)
			{
				case KeyCode.ArrowUp:	sel--;								break;
				case KeyCode.ArrowDown:	sel++;								break;
				case KeyCode.PageUp:	sel -= this.FullyVisibleRowCount-1;	break;
				case KeyCode.PageDown:	sel += this.FullyVisibleRowCount-1;	break;
				
				default:
					if (Feel.Factory.Active.TestSelectItemKey (message))
					{
						this.OnSelectionActivated();
						return true;
					}
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


		protected void UpdateScroller()
		{
			// Met � jour l'ascenseur en fonction de la liste.
			
			int total = this.items.Count;
			if ( total <= this.visibleLines )
			{
				if ( this.scroller.IsVisible )
				{
					this.scroller.Hide();
					this.UpdateMargins();
				}
			}
			else
			{
				this.scroller.MaxValue          = (decimal) (total-this.visibleLines);
				this.scroller.VisibleRangeRatio = (decimal) ((double)this.visibleLines/total);
				this.scroller.Value             = (decimal) (this.firstLine);
				this.scroller.SmallChange       = 1;
				this.scroller.LargeChange       = (decimal) (this.visibleLines/2.0);
				
				if ( !this.scroller.IsVisible )
				{
					this.scroller.Show();
					this.UpdateMargins();
				}
			}
		}
		
		protected void SetDirty()
		{
			if ( this.isDirty == false )
			{
				this.isDirty = true;
				this.Invalidate();
			}
		}

		protected void UpdatetextLayouts()
		{
			//	Met � jour les textes.
			
			if ( this.isDirty )
			{
				this.UpdateScroller();

				int max = System.Math.Min(this.visibleLines, this.items.Count);
				for ( int i=0 ; i<max ; i++ )
				{
					if ( this.textLayouts[i] == null )
					{
						this.textLayouts[i] = new TextLayout();
					}
					
					string text = this.items[i+this.firstLine];
					
					this.textLayouts[i].ResourceManager = this.ResourceManager;
					this.textLayouts[i].Text            = this.AutoResolveResRef ? this.ResourceManager.ResolveTextRef (text) : text;
					this.textLayouts[i].DefaultFont     = this.DefaultFont;
					this.textLayouts[i].DefaultFontSize = this.DefaultFontSize;
					this.textLayouts[i].LayoutSize      = new Drawing.Size(this.GetTextWidth(), this.lineHeight);
				}
				this.isDirty = false;
			}
		}


		protected override void UpdateClientGeometry()
		{
			//	Met � jour la g�om�trie de l'ascenseur de la liste.
			
			base.UpdateClientGeometry();
			
			if ( this.lineHeight == 0 )  return;

			this.visibleLines = (int)((this.Bounds.Height-ScrollList.TextOffsetY*2)/this.lineHeight);
			if ( this.visibleLines < 1 )  this.visibleLines = 1;
			this.textLayouts = new TextLayout[this.visibleLines];
			
			this.SetDirty();

			if ( this.scroller != null )
			{
				this.UpdateMargins ();
				IAdorner adorner = Widgets.Adorner.Factory.Active;
				Drawing.Rectangle rect = new Drawing.Rectangle();
				rect.Right  = this.Client.Width-adorner.GeometryScrollerRightMargin;
				rect.Left   = rect.Right-this.scroller.Width;
				rect.Bottom = adorner.GeometryScrollerBottomMargin+ScrollList.TextOffsetY-this.margins.Bottom;
				rect.Top    = this.Client.Height-adorner.GeometryScrollerTopMargin-ScrollList.TextOffsetY+this.margins.Top;
				this.scroller.Bounds = rect;
			}
		}

		protected override void OnAdornerChanged()
		{
			this.UpdateClientGeometry ();
			this.UpdateMargins ();
			base.OnAdornerChanged ();
		}
		
		protected override void OnResourceManagerChanged()
		{
			base.OnResourceManagerChanged ();
			
			Support.ResourceManager resource_manager = this.ResourceManager;
			
			for (int i = 0; i < this.textLayouts.GetLength (0); i++)
			{
				TextLayout layout = this.textLayouts[i];
				
				if (layout != null)
				{
					layout.ResourceManager = resource_manager;
				}
			}
			
			this.Invalidate ();
		}

		protected void UpdateMargins()
		{
			IAdorner adorner = Widgets.Adorner.Factory.Active;
			
			this.margins = new Drawing.Margins (adorner.GeometryScrollListXMargin, adorner.GeometryScrollListXMargin,
				/**/                            adorner.GeometryScrollListYMargin, adorner.GeometryScrollListYMargin);
			
			if ((this.scroller != null) &&
				(this.scroller.IsVisible))
			{
				this.margins.Right = this.Client.Width - this.scroller.Left;
			}
		}
		
		protected double GetTextWidth()
		{
			//	Calcule la largeur utile pour le texte.
			
			return this.Client.Width - this.margins.Width;
		}


		protected virtual void OnSelectedIndexChanged()
		{
			// G�n�re un �v�nement pour dire que la s�lection dans la liste a chang�.
			
			if ( this.SelectedIndexChanged != null )  // qq'un �coute ?
			{
				this.SelectedIndexChanged(this);
			}
		}

		protected virtual void OnSelectionActivated()
		{
			// G�n�re un �v�nement pour dire que la s�lection a �t� valid�e
			
			if ( this.SelectionActivated != null )
			{
				this.SelectionActivated(this);
			}
		}


		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			this.UpdatetextLayouts();
			
			IAdorner adorner = Widgets.Adorner.Factory.Active;

			Drawing.Rectangle rect  = this.Client.Bounds;
			WidgetState       state = this.PaintState;
			
			if ( this.scrollListStyle == ScrollListStyle.Menu )
			{
				Drawing.Rectangle menu = rect;
				menu.Inflate(adorner.GeometryMenuShadow);
				menu.Deflate(0, 0, ScrollList.TextOffsetY-this.margins.Top, ScrollList.TextOffsetY-this.margins.Bottom);
				adorner.PaintMenuBackground(graphics, menu, state, Direction.Down, Drawing.Rectangle.Empty, 0);
			}
			else
			{
				Drawing.Rectangle menu = rect;
				menu.Deflate(0, 0, ScrollList.TextOffsetY-this.margins.Top, ScrollList.TextOffsetY-this.margins.Bottom);
				adorner.PaintTextFieldBackground(graphics, menu, state, TextFieldStyle.Multi, false);
			}

			Drawing.Point pos = new Drawing.Point(ScrollList.TextOffsetX, rect.Height-ScrollList.TextOffsetY-this.lineHeight);
			int max = System.Math.Min(this.visibleLines, this.items.Count);
			for ( int i=0 ; i<max ; i++ )
			{
				if ( this.textLayouts[i] == null )  break;

				if ( i+this.firstLine == this.selectedLine &&
					 (state&WidgetState.Enabled) != 0 )
				{
					TextLayout.SelectedArea[] areas = new TextLayout.SelectedArea[1];
					areas[0] = new TextLayout.SelectedArea();
					areas[0].Rect.Left   = this.margins.Left;
					areas[0].Rect.Width  = this.GetTextWidth();
					areas[0].Rect.Bottom = pos.Y;
					areas[0].Rect.Height = this.lineHeight;
					adorner.PaintTextSelectionBackground(graphics, areas, state);

					state |= WidgetState.Selected;
				}
				else
				{
					state &= ~WidgetState.Selected;
				}

				adorner.PaintButtonTextLayout(graphics, pos, this.textLayouts[i], state, ButtonStyle.ListItem);
				pos.Y -= this.lineHeight;
			}
		}
		
		
		#region IStringCollectionHost Members
		public void StringCollectionChanged()
		{
			if (this.items.Count == 0)
			{
				this.FirstVisibleRow = 0;
				this.SelectedIndex   = -1;
			}
			
			this.SetDirty ();
		}
		
		
		public Helpers.StringCollection			Items
		{
			get
			{
				return this.items;
			}
		}
		#endregion
		
		#region INamedStringSelection Members
		public int								SelectedIndex
		{
			get
			{
				//	-1 => pas de ligne s�lectionn�e
				
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
					this.SetDirty();
					this.OnSelectedIndexChanged();
				}
			}
		}

		public string							SelectedItem
		{
			get
			{
				int index = this.SelectedIndex;
				if ( index < 0 )  return null;
				return this.Items[index];
			}
			
			set
			{
				this.SelectedIndex = this.Items.IndexOf (value);
			}
		}
		
		public string							SelectedName
		{
			// Nom de la ligne s�lectionn�e, null si aucune.
			get
			{
				if ( this.selectedLine == -1 )
				{
					return null;
				}
				
				return this.items.GetName(this.selectedLine);
			}

			set
			{
				if ( this.SelectedName != value )
				{
					int index = -1;
					
					if ( value != null )
					{
						index = this.items.FindNameIndex(value);
					
						if ( index < 0 )
						{
							throw new System.ArgumentException(string.Format("No element named '{0}' in list", value));
						}
					}
					
					this.SelectedIndex = index;
				}
			}
		}
		
		
		public event Support.EventHandler		SelectedIndexChanged;
		#endregion
		
		private void HandleScrollerValueChanged(object sender)
		{
			// Appel� lorsque l'ascenseur a boug�.
			
			this.FirstVisibleRow = (int)(this.scroller.DoubleValue + 0.5);
		}


		public event Support.EventHandler		SelectionActivated;
		
		protected const double					TextOffsetX = 3;
		protected const double					TextOffsetY = 3;

		protected ScrollListStyle				scrollListStyle;
		protected bool							isDirty;
		protected bool							mouseDown = false;
		protected Helpers.StringCollection		items;
		protected TextLayout[]					textLayouts;
		
		protected Drawing.Margins				margins;
		
		protected double						lineHeight;
		protected VScroller						scroller;
		protected int							visibleLines;
		protected int							firstLine = 0;
		protected int							selectedLine = -1;
	}
}
