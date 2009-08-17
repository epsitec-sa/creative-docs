//	Copyright © 2003-2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Support;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// The <c>ScrollList</c> class implements a simple list with a vertical scrollbar,
	/// from which the user can pick items.
	/// </summary>
	public class ScrollList : Widget, Collections.IStringCollectionHost, Support.Data.INamedStringSelection
	{
		public ScrollList()
		{
			this.items = new Collections.StringCollection (this);
			this.items.AcceptsRichText = true;

			this.Padding = new Margins (2, 2, 2, 2);
			this.AutoFocus = true;
			this.AutoDoubleClick = true;
			this.InternalState |= InternalState.Focusable;

			this.selectItemBehavior = new Behaviors.SelectItemBehavior (this.AutomaticItemSelection);
			this.autoScrollBehavior = new Behaviors.AutoScrollBehavior (this, this.AutomaticScroll);

			this.scrollListStyle = ScrollListStyle.Normal;
			this.lineHeight = Widget.DefaultFontHeight+1;

			this.scroller = new VScroller ()
			{
				IsInverted = true,
				Parent = this
			};

			this.scroller.ValueChanged += this.HandleScrollerValueChanged;
			this.scroller.Hide ();
			
			this.UpdateMargins ();
		}

		public ScrollList(Widget embedder)
			: this ()
		{
			this.SetEmbedder (embedder);
		}


		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.scroller.ValueChanged -= this.HandleScrollerValueChanged;
				this.autoScrollBehavior.Dispose ();
			}

			base.Dispose (disposing);
		}


		public ScrollListStyle ScrollListStyle
		{
			get
			{
				return this.scrollListStyle;
			}
			set
			{
				if (this.scrollListStyle != value)
				{
					this.scrollListStyle = value;
					this.Invalidate ();
				}
			}
		}


		public bool DrawFrame
		{
			//	Détermine s'il faut dessiner un cadre autour de chaque ligne de la liste.
			get
			{
				return this.drawFrame;
			}

			set
			{
				if (this.drawFrame != value)
				{
					this.drawFrame = value;
					this.Invalidate ();
				}
			}
		}

		public bool								AllLinesHaveSameWidth
		{
			//	Détermine si toutes les lignes ont la même largeur (par exemple parce qu'elles
			//	contiennent de simples icônes), pour accélérer l'ouverture.
			get
			{
				return this.allLinesHaveSameWidth;
			}

			set
			{
				this.allLinesHaveSameWidth = value;
			}
		}

		public AbstractScroller Scroller
		{
			get
			{
				return this.scroller;
			}
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

		public override Size GetBestFitSize()
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
			
			return new Size(width, height);
		}

		public Size GetBestLineSize()
		{
			//	Donne les dimensions optimales pour la liste.
			//	La largeur est la largeur la plus grande de tous les textes contenus dans Items.
			//	La hauteur est la hauteur la plus grande de tous les textes contenus dans Items.

			double dx = 0;
			double dy = 0;

			TextLayout layout = new TextLayout();
			layout.SetEmbedder (this);
			
			int max = this.allLinesHaveSameWidth ? (this.items.Count == 0 ? 0 : 1) : this.items.Count;
			for ( int i=0 ; i<max ; i++ )
			{
				layout.Text = this.items[i];
				Size size = layout.SingleLineSize;
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

			return new Size(dx, dy);
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


		public override Margins GetShapeMargins()
		{
			IAdorner adorner = Widgets.Adorners.Factory.Active;
			
			Margins margins = adorner.GeometryListShapeMargins;

			if (this.scrollListStyle == ScrollListStyle.Menu)
			{
				margins += adorner.GeometryMenuShadow;
			}
			
			return margins;
		}

		protected override void ProcessMessage(Message message, Point pos)
		{
			switch (message.MessageType)
			{
				case MessageType.MouseEnter:
				case MessageType.MouseLeave:
					break;

				case MessageType.MouseDown:
					this.MouseSelectBegin ();
					this.MouseSelect (pos);
					break;

				case MessageType.MouseMove:
					if (this.mouseDown || this.scrollListStyle == ScrollListStyle.Menu)
					{
						this.MouseSelect (pos);
					}
					break;

				case MessageType.MouseUp:
					if (this.mouseDown)
					{
						this.MouseSelect (pos);
						this.autoScrollBehavior.ProcessEvent (Point.Zero);
						this.OnSelectionActivated ();
						this.MouseSelectEnd ();
					}
					break;

				case MessageType.MouseWheel:
					if (message.Wheel < 0)
						this.FirstVisibleRow++;
					if (message.Wheel > 0)
						this.FirstVisibleRow--;
					break;

				case MessageType.KeyDown:
					if (!this.ProcessKeyDown (message))
					{
						base.ProcessMessage (message, pos);
						return;
					}
					break;

				case MessageType.KeyPress:
					if (!this.ProcessKeyPress (message))
					{
						base.ProcessMessage (message, pos);
						return;
					}
					break;

				default:
					return;
			}

			message.Consumer = this;
		}

		protected virtual void MouseSelectBegin()
		{
			this.mouseDown = true;
			this.Invalidate ();
		}

		protected virtual void MouseSelectEnd()
		{
			this.mouseDown = false;
			this.Invalidate ();
		}

		protected virtual bool MouseSelect(Point pos)
		{
			if (this.lineHeight <= 0)
			{
				return false;
			}
			
			double y = this.Client.Size.Height - pos.Y - 1 - ScrollList.TextOffsetY;
			double x = pos.X - this.margins.Left;
			double h = this.visibleLines * this.lineHeight;

			if (y < 0)
			{
				this.autoScrollBehavior.ProcessEvent (new Point (0, -1));
				return false;
			}
			else if (y >= h)
			{
				this.autoScrollBehavior.ProcessEvent (new Point (0, 1));
				return false;
			}
			else
			{
				this.autoScrollBehavior.ProcessEvent (Point.Zero);
			}

			int line = (int) (y / this.lineHeight);

			System.Diagnostics.Debug.Assert (line >= 0.0);
			System.Diagnostics.Debug.Assert (line < this.visibleLines);

			this.MouseSelectLine (this.firstLine+line);
			
			return true;
		}

		protected virtual void MouseSelectLine(int index)
		{
			this.SelectedIndex = index;
			this.ShowSelected (ScrollShowMode.Extremity);
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
						this.textLayouts[i].SetEmbedder (this);
					}
					
					string text = (i+this.firstLine < this.items.Count) ? this.items[i+this.firstLine] : "";

					this.textLayouts[i].Text       = text;
					this.textLayouts[i].LayoutSize = new Size(this.GetTextWidth(), this.lineHeight);
				}
				this.isDirty = false;
			}
		}


		protected override void SetBoundsOverride(Rectangle oldRect, Rectangle newRect)
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
				Rectangle rect = new Rectangle();
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
		
		protected void UpdateMargins()
		{
			IAdorner adorner = Widgets.Adorners.Factory.Active;
			
			this.margins = new Margins(adorner.GeometryScrollListXMargin, adorner.GeometryScrollListXMargin,
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
			int index = this.items.FindStartMatch (search, this.SelectedIndex + (continued ? 0 : 1));

			if (index < 0)
			{
				index = this.items.FindStartMatch (search);
			}

			if (index >= 0)
			{
				this.SelectedIndex = index;
				this.ShowSelected (ScrollShowMode.Extremity);
			}
		}

		protected virtual void AutomaticScroll(Point magnitude)
		{
			int index = this.SelectedIndex;

			if (index >= 0)
			{
				if (magnitude.Y < 0)
				{
					if (index > 0)
					{
						index--;
					}
				}
				else if (magnitude.Y > 0)
				{
					if (index < this.Items.Count-1)
					{
						index++;
					}
				}

				this.MouseSelectLine (index);
			}
		}
		
		protected virtual void OnSelectedIndexChanged()
		{
			//	Génère un événement pour dire que la sélection dans la liste a changé.
			EventHandler handler = (EventHandler) this.GetUserEventHandler("SelectedIndexChanged");
			if (handler != null)
			{
				handler(this);
			}
		}

		protected virtual void OnSelectionActivated()
		{
			//	Génère un événement pour dire que la sélection a été validée
			EventHandler handler = (EventHandler) this.GetUserEventHandler("SelectionActivated");
			if (handler != null)
			{
				handler(this);
			}
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			this.UpdatetextLayouts ();

			IAdorner adorner = Widgets.Adorners.Factory.Active;

			Rectangle rect  = this.Client.Bounds;
			WidgetPaintState  state = this.PaintState;

			if (this.scrollListStyle == ScrollListStyle.Menu)
			{
				Rectangle frame = Rectangle.Deflate (rect, new Margins (0, 0, ScrollList.TextOffsetY-this.margins.Top, ScrollList.TextOffsetY-this.margins.Bottom));
				adorner.PaintTextFieldBackground (graphics, frame, state, TextFieldStyle.Simple, TextFieldDisplayMode.Default, false);
			}
			else
			{
				Rectangle frame = Rectangle.Deflate (rect, new Margins (0, 0, ScrollList.TextOffsetY-this.margins.Top, ScrollList.TextOffsetY-this.margins.Bottom));
				adorner.PaintTextFieldBackground (graphics, frame, state, TextFieldStyle.Multiline, TextFieldDisplayMode.Default, false);
			}

			Point pos = new Point (ScrollList.TextOffsetX, rect.Height-ScrollList.TextOffsetY-this.lineHeight);
			int   max = System.Math.Min (this.visibleLines, this.items.Count);
			
			for (int i = 0; i < max; i++)
			{
				if (this.textLayouts[i] == null)
				{
					break;
				}

				if ((this.IsLineSelected (this.firstLine + i)) &&
					((state & WidgetPaintState.Enabled) != 0))
				{
					TextLayout.SelectedArea[] areas = new TextLayout.SelectedArea[1]
					{
						new TextLayout.SelectedArea ()
						{
							Rect = new Rectangle ()
							{
								Left   = this.margins.Left,
								Width  = this.GetTextWidth (),
								Bottom = pos.Y,
								Height = this.lineHeight
							}
						}
					};

					adorner.PaintTextSelectionBackground (graphics, areas, state, PaintTextStyle.TextField, TextFieldDisplayMode.Default);

					state |= WidgetPaintState.Selected;
				}
				else
				{
					state &= ~WidgetPaintState.Selected;
				}

				adorner.PaintButtonTextLayout (graphics, pos, this.textLayouts[i], state, ButtonStyle.ListItem);

				if (this.drawFrame)  // dessine les cadres ?
				{
					Rectangle frame = new Rectangle (this.margins.Left, pos.Y, this.GetTextWidth (), this.lineHeight);

					if (i < max-1)
					{
						frame.Bottom -= 1;
					}
					
					frame.Deflate (0.5);
					
					graphics.AddRectangle (frame);
					graphics.RenderSolid (adorner.ColorBorder);
				}

				pos = new Point (pos.X, pos.Y - this.lineHeight);
			}
		}

		protected virtual bool IsLineSelected(int index)
		{
			return index == this.selectedLine;
		}
		
		
		#region IStringCollectionHost Members
		public void NotifyStringCollectionChanged()
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
		
		
		public event EventHandler				SelectedIndexChanged
		{
			add
			{
				this.AddUserEventHandler("SelectedIndexChanged", value);
			}
			remove
			{
				this.RemoveUserEventHandler("SelectedIndexChanged", value);
			}
		}
		#endregion
		
		private void HandleScrollerValueChanged(object sender)
		{
			//	Appelé lorsque l'ascenseur a bougé.
			
			this.FirstVisibleRow = (int)(this.scroller.DoubleValue + 0.5);
		}


		public event EventHandler				SelectionActivated
		{
			add
			{
				this.AddUserEventHandler("SelectionActivated", value);
			}
			remove
			{
				this.RemoveUserEventHandler("SelectionActivated", value);
			}
		}

		
		protected const double					TextOffsetX = 3;
		protected const double					TextOffsetY = 2;

		readonly Behaviors.SelectItemBehavior	selectItemBehavior;
		readonly Behaviors.AutoScrollBehavior	autoScrollBehavior;

		protected ScrollListStyle				scrollListStyle;
		protected bool							isDirty;
		protected bool							drawFrame;
		protected bool							mouseDown;
		readonly Collections.StringCollection	items;
		protected TextLayout[]					textLayouts;
		protected bool							allLinesHaveSameWidth;
		
		protected Margins				margins;
		
		protected double						lineHeight;
		readonly VScroller						scroller;
		protected int							visibleLines;
		protected int							firstLine = 0;
		protected int							selectedLine = -1;
	}
}
