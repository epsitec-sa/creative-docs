namespace Epsitec.Common.Widgets
{
	public enum ScrollListStyle
	{
		Normal,			// liste fixe normale
		Menu,			// menu d'un TextFieldCombo
	}

	public enum ScrollListShow
	{
		Extremity,		// déplacement minimal aux extrémités
		Middle,			// déplacement central
	}

	public enum ScrollListAdjust
	{
		MoveUp,			// déplace le haut
		MoveDown,		// déplace le bas
	}

	/// <summary>
	/// La classe ScrollList réalise une liste déroulante simple.
	/// </summary>
	public class ScrollList : Widget, Helpers.IStringCollectionHost
	{
		public ScrollList()
		{
			this.items = new Helpers.StringCollection(this);
			this.DockMargins = new Drawing.Margins(2, 2, 2, 2);
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
		}
		
		public ScrollList(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}

		
		public override void RestoreFromBundle(Epsitec.Common.Support.ObjectBundler bundler, Epsitec.Common.Support.ResourceBundle bundle)
		{
			base.RestoreFromBundle(bundler, bundle);
			
			Support.ResourceBundle items = bundle["items"].AsBundle;
			
			if ( items != null )
			{
				string[] names = items.FieldNames;
				System.Array.Sort(names);
				
				for ( int i=0 ; i<items.CountFields ; i++ )
				{
					string name = names[i];
					string item = items[name].AsString;
					
					if ( item == null )
					{
						throw new Support.ResourceException(string.Format("Item '{0}' is invalid", name));
					}
					
					this.Items.Add(Support.ResourceBundle.ExtractName(name), item);
				}
			}
		}
		
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.scroller.ValueChanged -= new Support.EventHandler(this.HandleScrollerValueChanged);
			}
			
			base.Dispose(disposing);
		}


		public Helpers.StringCollection		Items
		{
			get { return this.items; }
		}

		public ScrollListStyle				ScrollListStyle
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

		public AbstractScroller				Scroller
		{
			get { return this.scroller; }
		}


		public int							SelectedIndex
		{
			// Ligne sélectionnée, -1 si aucune.
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
					this.SetDirty();
				}
			}
		}

		public string						SelectedItem
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
		
		public string						SelectedName
		{
			// Nom de la ligne sélectionnée, null si aucune.
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

		public int							FirstVisibleLine
		{
			// Première ligne visible.
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

		
		public void ShowSelectedLine(ScrollListShow mode)
		{
			// Rend la ligne sélectionnée visible.
			
			if ( this.selectedLine == -1 ) return;
			if ( this.selectedLine >= this.firstLine && this.selectedLine <  this.firstLine+this.visibleLines ) return;
			
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

		
		public bool AdjustHeight(ScrollListAdjust mode)
		{
			// Ajuste la hauteur pour afficher pile un nombre entier de lignes.
			
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

		public bool AdjustHeightToContent(ScrollListAdjust mode, double hMin, double hMax)
		{
			// Ajuste la hauteur pour afficher exactement le nombre de lignes contenues.
			
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
			if ( h != hope )  AdjustHeight(mode);
			return true;
		}


		private void HandleScrollerValueChanged(object sender)
		{
			// Appelé lorsque l'ascenseur a bougé.
			
			this.FirstVisibleLine = (int)(this.scroller.DoubleValue + 0.5);
		}


		protected override void ProcessMessage(Message message, Drawing.Point pos)
		{
			// Gestion d'un événement.
			
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
						this.OnSelectedIndexChanged();
						this.OnValidation();
						this.mouseDown = false;
					}
					break;

				case MessageType.MouseWheel:
					if ( message.Wheel < 0 )  this.FirstVisibleLine ++;
					if ( message.Wheel > 0 )  this.FirstVisibleLine --;
					break;

				case MessageType.KeyDown:
					if ( !this.ProcessKeyDown(message.KeyCode, message.IsShiftPressed, message.IsCtrlPressed) )
					{
						return;
					}
					break;
				
				default:
					return;
			}
			
			message.Consumer = this;
		}

		protected bool MouseSelect(Drawing.Point pos)
		{
			// Sélectionne la ligne selon la souris.
			
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

		protected bool ProcessKeyDown(KeyCode key, bool isShiftPressed, bool isCtrlPressed)
		{
			// Gestion d'une touche pressée avec KeyDown dans la liste.
			
			int sel;
			switch ( key )
			{
				case KeyCode.ArrowUp:
					sel = this.SelectedIndex-1;
					if ( sel >= 0 )
					{
						this.SelectedIndex = sel;
						this.OnSelectedIndexChanged();
						this.ShowSelectedLine(ScrollListShow.Extremity);
					}
					break;

				case KeyCode.ArrowDown:
					sel = this.SelectedIndex+1;
					if ( sel < this.items.Count )
					{
						this.SelectedIndex = sel;
						this.OnSelectedIndexChanged();
						this.ShowSelectedLine(ScrollListShow.Extremity);
					}
					break;

				case KeyCode.Return:
				case KeyCode.Space:
					this.OnValidation();
					break;

				default:
					return false;
			}
			
			return true;
		}


		protected void UpdateScroller()
		{
			// Met à jour l'ascenseur en fonction de la liste.
			
			int total = this.items.Count;
			if ( total <= this.visibleLines )
			{
				if ( this.scroller.IsVisible )
				{
					this.scroller.Hide();
					this.rightMargin = 0;
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
					this.rightMargin = this.scroller.Width;
					this.scroller.Show();
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

		// Met à jour les textes.
		protected void UpdatetextLayouts()
		{
			if ( this.isDirty )
			{
				this.UpdateScroller();

				int max = System.Math.Min(this.visibleLines, this.items.Count);
				for ( int i=0 ; i<max ; i++ )
				{
					if ( this.textLayouts[i] == null )
					{
						this.textLayouts[i] = new TextLayout();
						this.textLayouts[i].BreakMode = Drawing.TextBreakMode.Ellipsis | Drawing.TextBreakMode.SingleLine;
					}
					this.textLayouts[i].Text = this.items[i+this.firstLine];
					this.textLayouts[i].Font = this.DefaultFont;
					this.textLayouts[i].FontSize = this.DefaultFontSize;
					this.textLayouts[i].LayoutSize = new Drawing.Size(this.GetTextWidth(), this.lineHeight);
				}
				this.isDirty = false;
			}
		}


		// Met à jour la géométrie de l'ascenseur de la liste.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();
			
			if ( this.lineHeight == 0 )  return;

			this.visibleLines = (int)((this.Bounds.Height-ScrollList.Margin*2)/this.lineHeight);
			if ( this.visibleLines < 1 )  this.visibleLines = 1;
			this.textLayouts = new TextLayout[this.visibleLines];
			
			this.SetDirty();

			if ( this.scroller != null )
			{
				IAdorner adorner = Widgets.Adorner.Factory.Active;
				Drawing.Rectangle rect = new Drawing.Rectangle();
				rect.Right  = this.Bounds.Width-adorner.GeometryScrollerRightMargin;
				rect.Left   = rect.Right-this.scroller.Width;
				rect.Bottom = adorner.GeometryScrollerBottomMargin;
				rect.Top    = this.Bounds.Height-adorner.GeometryScrollerTopMargin;
				this.scroller.Bounds = rect;
			}
		}

		protected override void OnAdornerChanged()
		{
			this.UpdateClientGeometry();
			base.OnAdornerChanged();
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

		// Calcule la largeur utile pour le texte.
		protected double GetTextWidth()
		{
			IAdorner adorner = Widgets.Adorner.Factory.Active;

			double width = this.Client.Width-ScrollList.Margin*2;
			if ( this.rightMargin > 0 )
			{
				width -= this.rightMargin-1+adorner.GeometryScrollerRightMargin;
			}

			return width;
		}


		protected virtual void OnSelectedIndexChanged()
		{
			// Génère un événement pour dire que la sélection dans la liste a changé.
			
			if ( this.SelectedIndexChanged != null )  // qq'un écoute ?
			{
				this.SelectedIndexChanged(this);
			}
		}

		protected virtual void OnValidation()
		{
			// Génère un événement pour dire que la sélection a été validée
			
			if ( this.Validation != null )
			{
				this.Validation(this);
			}
		}


		// Dessine la liste.
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
				adorner.PaintMenuBackground(graphics, menu, state, Direction.Down, Drawing.Rectangle.Empty, 0);
			}
			else
			{
				adorner.PaintTextFieldBackground(graphics, rect, state, TextFieldStyle.Multi, false);
			}

			Drawing.Point pos = new Drawing.Point(ScrollList.Margin, rect.Height-ScrollList.Margin-this.lineHeight);
			int max = System.Math.Min(this.visibleLines, this.items.Count);
			for ( int i=0 ; i<max ; i++ )
			{
				if ( this.textLayouts[i] == null )  break;

				if ( i+this.firstLine == this.selectedLine &&
					 (state&WidgetState.Enabled) != 0 )
				{
					Drawing.Rectangle[] rects = new Drawing.Rectangle[1];
					rects[0].Left   = ScrollList.Margin;
					rects[0].Width  = this.GetTextWidth();
					rects[0].Left  += adorner.GeometrySelectedLeftMargin;
					rects[0].Right += adorner.GeometrySelectedRightMargin;
					rects[0].Bottom = pos.Y;
					rects[0].Height = this.lineHeight;
					adorner.PaintTextSelectionBackground(graphics, rects, state);

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
				this.FirstVisibleLine = 0;
				this.SelectedIndex = -1;
			}
			
			this.SetDirty ();
		}
		#endregion


		public event Support.EventHandler		SelectedIndexChanged;
		public event Support.EventHandler		Validation;
		

		protected static readonly double		Margin = 3;
		
		protected ScrollListStyle				scrollListStyle;
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
