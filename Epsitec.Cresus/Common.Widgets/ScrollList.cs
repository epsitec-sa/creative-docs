//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe ScrollList réalise une liste déroulante simple.
	/// </summary>
	public class ScrollList : Widget, Collections.IStringCollectionHost, Support.Data.INamedStringSelection
	{
		public ScrollList()
		{
			this.items = new Collections.StringCollection(this);
			this.items.AcceptsRichText = true;
			
			this.Padding = new Drawing.Margins(2, 2, 2, 2);
			this.AutoFocus = true;
			this.AutoDoubleClick = true;
			this.InternalState |= InternalState.Focusable;

			
			this.selectItemBehavior = new Behaviors.SelectItemBehavior(new Behaviors.SelectItemCallback(this.AutomaticItemSelection));
			
			this.scrollListStyle = ScrollListStyle.Normal;
			this.lineHeight = Widget.DefaultFontHeight+1;
			this.scroller = new VScroller(null);
			this.scroller.IsInverted = true;
			this.scroller.SetParent(this);
			this.scroller.ValueChanged += new Support.EventHandler(this.HandleScrollerValueChanged);
			this.scroller.Hide();
			this.UpdateMargins();
		}
		
		public ScrollList(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}

		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				if ( this.scroller != null )
				{
					this.scroller.ValueChanged -= new Support.EventHandler(this.HandleScrollerValueChanged);
				}
			}
			
			base.Dispose(disposing);
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

		public bool								DrawFrame
		{
			//	Détermine s'il faut dessiner un cadre autour de chaque ligne de la liste.
			get
			{
				return this.drawFrame;
			}

			set
			{
				if ( this.drawFrame != value )
				{
					this.drawFrame = value;
					this.Invalidate();
				}
			}
		}

		public bool								AllLinesWidthSameWidth
		{
			//	Détermine si toutes les lignes ont la même largeur (par exemple parce qu'elles
			//	contiennent de simples icônes), pour accélérer l'ouverture.
			get
			{
				return this.allLinesWidthSameWidth;
			}

			set
			{
				this.allLinesWidthSameWidth = value;
			}
		}

		public AbstractScroller					Scroller
		{
			get { return this.scroller; }
		}

		public int								FirstVisibleRow
		{
			//	Première ligne visible.
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
			//	Rend la ligne sélectionnée visible.

			Layouts.LayoutContext.SyncArrange (this);
			
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

		
		public double LineHeight
		{
			//	Hauteur d'une ligne.

			get
			{
				return this.lineHeight;
			}

			set
			{
				if ( this.lineHeight != value )
				{
					this.lineHeight = value;
				}
			}
		}

		public override Drawing.Size GetBestFitSize()
		{
			double margin = ScrollList.TextOffsetY * 2;
			double height = System.Math.Min (this.lineHeight * this.items.Count, this.MaxHeight);
			double width  = this.PreferredWidth;
			
			double dy = height;
			
			dy += margin;
			dy  = System.Math.Max (dy, this.MinHeight);
			dy  = System.Math.Min (dy, this.MaxHeight);
			dy -= margin;
			
			int n = (int) (dy / this.lineHeight);
			
			height = this.lineHeight * n + margin;
			
			return new Drawing.Size(width, height);
		}

		public Drawing.Size GetBestLineSize()
		{
			//	Donne les dimensions optimales pour la liste.
			//	La largeur est la largeur la plus grande de tous les textes contenus dans Items.
			//	La hauteur est la hauteur la plus grande de tous les textes contenus dans Items.

			double dx = 0;
			double dy = 0;

			TextLayout layout = new TextLayout();
			layout.ResourceManager = this.ResourceManager;
			layout.DefaultFont     = this.DefaultFont;
			layout.DefaultFontSize = this.DefaultFontSize;

			int max = this.allLinesWidthSameWidth ? 1 : this.items.Count;
			for ( int i=0 ; i<max ; i++ )
			{
				layout.Text = this.items[i];
				Drawing.Size size = layout.SingleLineSize;
				dx = System.Math.Max(dx, size.Width);
				dy = System.Math.Max(dy, size.Height);
			}

			IAdorner adorner = Widgets.Adorners.Factory.Active;
			dx += adorner.GeometryScrollerRightMargin;
			dx += this.margins.Left;
			dx += this.margins.Right;
			dx += 2;  // ch'tite marge pour respirer
			dx = System.Math.Ceiling(dx);

			dy = System.Math.Ceiling(dy);

			return new Drawing.Size(dx, dy);
		}

#if false
		public bool AdjustHeight(ScrollAdjustMode mode)
		{
			//	Ajuste la hauteur pour afficher pile un nombre entier de lignes.
			
			double h = this.Client.Size.Height-ScrollList.TextOffsetY*2;
			int count = (int)(h/this.lineHeight);
			
			return this.AdjustHeightToRows(mode, count);
		}

		bool AdjustHeightToContent(ScrollAdjustMode mode, double min_height, double max_height)
		{
			//	Ajuste la hauteur pour afficher exactement le nombre de lignes contenues.
			
			double h = this.lineHeight*this.items.Count+ScrollList.TextOffsetY*2;
			double hope = h;
			h = System.Math.Max(h, min_height);
			h = System.Math.Min(h, max_height);
			
			if ( h == this.Height )
			{
				return false;
			}

			switch ( mode )
			{
				case ScrollAdjustMode.MoveTop:
					this.Top = this.Bottom + h;
					break;
				
				case ScrollAdjustMode.MoveBottom:
					this.Bottom = this.Top - h;
					break;
				
				default:
					throw new System.NotSupportedException(string.Format("Adjust mode {0} not supported.", mode));
			}
			
			if ( h == hope )
			{
				this.Invalidate();
			}
			else
			{
				this.AdjustHeight(mode);
			}
			return true;
		}
		
		public bool AdjustHeightToRows(ScrollAdjustMode mode, int count)
		{
			//	Ajuste la hauteur pour afficher exactement le nombre de lignes spécifié.
			
			double h = this.Client.Height-ScrollList.TextOffsetY*2;
			double adjust = h - count*this.lineHeight;
			
			if ( adjust == 0 )
			{
				return false;
			}
			
			switch ( mode )
			{
				case ScrollAdjustMode.MoveTop:
					this.Top = System.Math.Floor(this.Top - adjust);
					break;
				
				case ScrollAdjustMode.MoveBottom:
					this.Bottom = System.Math.Floor(this.Bottom + adjust);
					break;
				
				default:
					throw new System.NotSupportedException(string.Format("Adjust mode {0} not supported.", mode));
			}
			
			this.Invalidate();
			return true;
		}
#endif


		public override Drawing.Margins GetShapeMargins()
		{
			IAdorner adorner = Widgets.Adorners.Factory.Active;
			
			Drawing.Margins margins = adorner.GeometryListShapeMargins;

			if (this.scrollListStyle == ScrollListStyle.Menu)
			{
				margins += adorner.GeometryMenuShadow;
			}
			
			return margins;
		}

		protected override void ProcessMessage(Message message, Drawing.Point pos)
		{
			//	Gestion d'un événement.
			
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
					if ( !this.ProcessKeyDown(message) )
					{
						base.ProcessMessage(message, pos);
						return;
					}
					break;
					
				case MessageType.KeyPress:
					if ( !this.ProcessKeyPress(message) )
					{
						base.ProcessMessage(message, pos);
						return;
					}
					break;
				
				default:
					return;
			}
			
			message.Consumer = this;
		}

		protected virtual bool MouseSelect(Drawing.Point pos)
		{
			//	Sélectionne la ligne selon la souris.
			
			double y = this.Client.Size.Height-pos.Y-1-ScrollList.TextOffsetY;
			double x = pos.X-this.margins.Left;
			
			if ( y < 0 ) return false;
			if ( y >= this.visibleLines*this.lineHeight ) return false;
			if ( x < 0 ) return false;
			if ( x >= this.Client.Size.Width-this.margins.Width ) return false;
			
			int line = (int)(y/this.lineHeight);
			
			System.Diagnostics.Debug.Assert(line >= 0.0);
			System.Diagnostics.Debug.Assert(line < this.visibleLines);
			
			this.SelectedIndex = this.firstLine+line;
			return true;
		}

		
		protected virtual bool ProcessKeyPress(Message message)
		{
			return this.selectItemBehavior.ProcessKeyPress(message);
		}
		
		protected virtual bool ProcessKeyDown(Message message)
		{
			if ( message.IsAltPressed     ||
				 message.IsShiftPressed   ||
				 message.IsControlPressed )
			{
				return false;
			}
			
			//	Gestion d'une touche pressée avec KeyDown dans la liste.
			
			int sel = this.SelectedIndex;
			
			switch ( message.KeyCode )
			{
				case KeyCode.Back:		sel = 0;							break;
				case KeyCode.Home:		sel = 0;							break;
				case KeyCode.End:		sel = this.RowCount-1;				break;
				case KeyCode.ArrowUp:	sel--;								break;
				case KeyCode.ArrowDown:	sel++;								break;
				case KeyCode.PageUp:	sel -= this.FullyVisibleRowCount-1;	break;
				case KeyCode.PageDown:	sel += this.FullyVisibleRowCount-1;	break;
				
				default:
					if ( Feel.Factory.Active.TestSelectItemKey(message) )
					{
						this.OnSelectionActivated();
						return true;
					}
					return false;
			}
			
			if ( this.SelectedIndex != sel )
			{
				this.selectItemBehavior.ClearSearch();
				
				sel = System.Math.Max(sel, 0);
				sel = System.Math.Min(sel, this.RowCount-1);
				
				this.SelectedIndex = sel;
				this.ShowSelected(ScrollShowMode.Extremity);
			}
			
			return true;
		}


		protected void UpdateScroller()
		{
			//	Met à jour l'ascenseur en fonction de la liste.
			
			int total = this.items.Count;
			if ( total <= this.visibleLines )
			{
				if (this.scroller.Visibility)
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

				if (!this.scroller.Visibility)
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
			//	Met à jour les textes.
			
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
					
					string text = (i+this.firstLine < this.items.Count) ? this.items[i+this.firstLine] : "";
					
					this.textLayouts[i].ResourceManager = this.ResourceManager;
					this.textLayouts[i].Text            = text;	//@	this.AutoResolveResRef ? this.ResourceManager.ResolveTextRef(text) : text;
					this.textLayouts[i].DefaultFont     = this.DefaultFont;
					this.textLayouts[i].DefaultFontSize = this.DefaultFontSize;
					this.textLayouts[i].LayoutSize      = new Drawing.Size(this.GetTextWidth(), this.lineHeight);
				}
				this.isDirty = false;
			}
		}


		protected override void SetBoundsOverride(Drawing.Rectangle oldRect, Drawing.Rectangle newRect)
		{
			base.SetBoundsOverride(oldRect, newRect);
			this.UpdateGeometry ();
		}
		
		protected void UpdateGeometry()
		{
			//	Met à jour la géométrie de l'ascenseur de la liste.
			
			if ( this.lineHeight == 0 )  return;

			this.visibleLines = (int)((this.ActualHeight-ScrollList.TextOffsetY*2)/this.lineHeight);
			if ( this.visibleLines < 1 )  this.visibleLines = 1;
			this.textLayouts = new TextLayout[this.visibleLines];
			
			this.SetDirty();

			if ( this.scroller != null )
			{
				this.UpdateMargins();
				IAdorner adorner = Widgets.Adorners.Factory.Active;
				Drawing.Rectangle rect = new Drawing.Rectangle();
				rect.Right  = this.Client.Size.Width-adorner.GeometryScrollerRightMargin;
				rect.Left   = rect.Right-this.scroller.PreferredWidth;
				rect.Bottom = adorner.GeometryScrollerBottomMargin+ScrollList.TextOffsetY-this.margins.Bottom;
				rect.Top    = this.Client.Size.Height-adorner.GeometryScrollerTopMargin-ScrollList.TextOffsetY+this.margins.Top;
				this.scroller.SetManualBounds(rect);
			}
		}

		protected override void OnAdornerChanged()
		{
			this.UpdateGeometry ();
			this.UpdateMargins();
			base.OnAdornerChanged();
		}
		
		protected override void OnResourceManagerChanged()
		{
			base.OnResourceManagerChanged();
			
			Support.ResourceManager resourceManager = this.ResourceManager;
			
			for ( int i=0 ; i<this.textLayouts.GetLength(0) ; i++ )
			{
				TextLayout layout = this.textLayouts[i];
				
				if ( layout != null )
				{
					layout.ResourceManager = resourceManager;
				}
			}
			
			this.Invalidate();
		}

		protected void UpdateMargins()
		{
			IAdorner adorner = Widgets.Adorners.Factory.Active;
			
			this.margins = new Drawing.Margins(adorner.GeometryScrollListXMargin, adorner.GeometryScrollListXMargin,
				/**/                           adorner.GeometryScrollListYMargin, adorner.GeometryScrollListYMargin);
			
			if ( this.scroller != null   &&
				 this.scroller.Visibility )
			{
				this.margins.Right = this.Client.Size.Width - this.scroller.ActualLocation.X;
			}
		}
		
		protected double GetTextWidth()
		{
			//	Calcule la largeur utile pour le texte.
			
			return this.Client.Size.Width - this.margins.Width;
		}


		protected virtual void AutomaticItemSelection(string search, bool continued)
		{
			int index = this.items.FindStartMatch(search, this.SelectedIndex + (continued ? 0 : 1));
			
			if ( index < 0 )
			{
				index = this.items.FindStartMatch(search);
			}
			
			if ( index >= 0 )
			{
				this.SelectedIndex = index;
			}
		}
		
		protected virtual void OnSelectedIndexChanged()
		{
			//	Génère un événement pour dire que la sélection dans la liste a changé.
			
			if ( this.SelectedIndexChanged != null )  // qq'un écoute ?
			{
				this.SelectedIndexChanged(this);
			}
		}

		protected virtual void OnSelectionActivated()
		{
			//	Génère un événement pour dire que la sélection a été validée
			
			if ( this.SelectionActivated != null )
			{
				this.SelectionActivated(this);
			}
		}


		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			this.UpdatetextLayouts();
			
			IAdorner adorner = Widgets.Adorners.Factory.Active;

			Drawing.Rectangle rect  = this.Client.Bounds;
			WidgetPaintState       state = this.PaintState;
			
			if ( this.scrollListStyle == ScrollListStyle.Menu )
			{
				Drawing.Rectangle menu = rect;
				menu.Deflate(0, 0, ScrollList.TextOffsetY-this.margins.Top, ScrollList.TextOffsetY-this.margins.Bottom);
				adorner.PaintTextFieldBackground(graphics, menu, state, TextFieldStyle.Simple, TextDisplayMode.Default, false);
			}
			else
			{
				Drawing.Rectangle menu = rect;
				menu.Deflate(0, 0, ScrollList.TextOffsetY-this.margins.Top, ScrollList.TextOffsetY-this.margins.Bottom);
				adorner.PaintTextFieldBackground(graphics, menu, state, TextFieldStyle.Multi, TextDisplayMode.Default, false);
			}

			Drawing.Point pos = new Drawing.Point(ScrollList.TextOffsetX, rect.Height-ScrollList.TextOffsetY-this.lineHeight);
			int max = System.Math.Min(this.visibleLines, this.items.Count);
			for ( int i=0 ; i<max ; i++ )
			{
				if ( this.textLayouts[i] == null )  break;

				if ( i+this.firstLine == this.selectedLine &&
					 (state&WidgetPaintState.Enabled) != 0 )
				{
					TextLayout.SelectedArea[] areas = new TextLayout.SelectedArea[1];
					areas[0] = new TextLayout.SelectedArea();
					areas[0].Rect.Left   = this.margins.Left;
					areas[0].Rect.Width  = this.GetTextWidth();
					areas[0].Rect.Bottom = pos.Y;
					areas[0].Rect.Height = this.lineHeight;
					adorner.PaintTextSelectionBackground(graphics, areas, state, PaintTextStyle.TextField, TextDisplayMode.Default);

					state |= WidgetPaintState.Selected;
				}
				else
				{
					state &= ~WidgetPaintState.Selected;
				}

				adorner.PaintButtonTextLayout(graphics, pos, this.textLayouts[i], state, ButtonStyle.ListItem);

				if ( this.drawFrame )  // dessine les cadres ?
				{
					Drawing.Rectangle frame = new Drawing.Rectangle();
					frame.Left   = this.margins.Left;
					frame.Width  = this.GetTextWidth();
					frame.Bottom = pos.Y;
					frame.Height = this.lineHeight;
					if ( i < max-1 )
					{
						frame.Bottom -= 1;
					}
					frame.Deflate(0.5);
					graphics.AddRectangle(frame);
					graphics.RenderSolid(adorner.ColorBorder);
				}

				pos.Y -= this.lineHeight;
			}
		}
		
		
		#region IStringCollectionHost Members
		public void StringCollectionChanged()
		{
			if ( this.items.Count == 0 )
			{
				this.FirstVisibleRow = 0;
				this.SelectedIndex   = -1;
			}
			
			this.SetDirty();
		}
		
		
		public Collections.StringCollection			Items
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
				//	-1 => pas de ligne sélectionnée
				
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
				this.SelectedIndex = this.Items.IndexOf(value);
			}
		}
		
		public string							SelectedName
		{
			//	Nom de la ligne sélectionnée, null si aucune.
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
			//	Appelé lorsque l'ascenseur a bougé.
			
			this.FirstVisibleRow = (int)(this.scroller.DoubleValue + 0.5);
		}


		public event Support.EventHandler		SelectionActivated;
		
		protected const double					TextOffsetX = 3;
		protected const double					TextOffsetY = 2;

		private Behaviors.SelectItemBehavior	selectItemBehavior;
		protected ScrollListStyle				scrollListStyle;
		protected bool							isDirty;
		protected bool							drawFrame = false;
		protected bool							mouseDown = false;
		protected Collections.StringCollection	items;
		protected TextLayout[]					textLayouts;
		protected bool							allLinesWidthSameWidth;
		
		protected Drawing.Margins				margins;
		
		protected double						lineHeight;
		protected VScroller						scroller;
		protected int							visibleLines;
		protected int							firstLine = 0;
		protected int							selectedLine = -1;
	}
}
