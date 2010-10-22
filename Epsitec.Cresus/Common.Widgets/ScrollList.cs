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
	public class ScrollList : Widget, Collections.IStringCollectionHost, Support.Data.IKeyedStringSelection
	{
		public ScrollList()
		{
			this.items = new Collections.StringCollection (this);
			this.items.AcceptsRichText = true;

			this.Padding = new Margins (2, 2, 2, 2);
			this.AutoFocus = true;
			this.AutoDoubleClick = true;
			this.InternalState |= WidgetInternalState.Focusable;
			this.AutomaticScrollEnable = true;

			this.selectItemBehavior = new Behaviors.SelectItemBehavior (this.AutomaticItemSelection);
			this.autoScrollBehavior = new Behaviors.AutoScrollBehavior (this, this.AutomaticScroll);

			this.scrollListStyle = ScrollListStyle.Standard;
			this.rowHeight = Widget.DefaultFontHeight+1;
			this.selectedRow = -1;

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


		public ScrollListStyle					ScrollListStyle
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

		public bool								DrawFrame
		{
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

		public bool								AllRowsHaveSameWidth
		{
			//	Détermine si toutes les lignes ont la même largeur (par exemple parce qu'elles
			//	contiennent de simples icônes), pour accélérer l'ouverture.
			get
			{
				return this.allRowsHaveSameWidth;
			}

			set
			{
				this.allRowsHaveSameWidth = value;
			}
		}

		public bool								AutomaticScrollEnable
		{
			get;
			set;
		}

		public AbstractScroller					Scroller
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
				return this.firstRow;
			}

			set
			{
				value = System.Math.Max(value, 0);
				value = System.Math.Min(value, System.Math.Max(this.items.Count-this.visibleRows, 0));
				
				if ( value != this.firstRow )
				{
					this.firstRow = value;
					this.SetDirty();
					this.Invalidate();
				}
			}
		}
		
		public int								VisibleRowCount
		{
			get
			{
				return this.visibleRows;
			}
		}

		public int								FullyVisibleRowCount
		{
			get
			{
				return this.visibleRows;
			}
		}

		public int								RowCount
		{
			get
			{
				return this.Items.Count;
			}
		}

		public double							RowHeight
		{
			get
			{
				return this.rowHeight;
			}
			set
			{
				if (this.rowHeight != value)
				{
					this.rowHeight = value;
					this.SetDirty ();
				}
			}
		}

		
		public void ShowSelected(ScrollShowMode mode)
		{
			//	Rend la ligne sélectionnée visible.

			Layouts.LayoutContext.SyncArrange (this);
			
			if ( this.selectedRow == -1 ) return;
			if ( this.selectedRow >= this.firstRow && this.selectedRow <  this.firstRow+this.visibleRows ) return;
			
			int fl = this.FirstVisibleRow;
			if ( mode == ScrollShowMode.Extremity )
			{
				if ( this.selectedRow < this.firstRow )
				{
					fl = this.selectedRow;
				}
				if ( this.selectedRow > this.firstRow+this.visibleRows-1 )
				{
					fl = this.selectedRow-(this.visibleRows-1);
				}
			}
			if ( mode == ScrollShowMode.Center )
			{
				int display = System.Math.Min(this.visibleRows, this.items.Count);
				fl = System.Math.Min(this.selectedRow+display/2, this.items.Count-1);
				fl = System.Math.Max(fl-display+1, 0);
			}
			this.FirstVisibleRow = fl;
		}

		public Rectangle GetRowBounds(int index)
		{
			index -= this.firstRow;

			if ((index < 0) ||
				(index >= this.textFrames.Length))
			{
				return Rectangle.Empty;
			}
			else
			{
				return this.textFrames[index];
			}
		}

		public Rectangle GetRowBounds(int index, int count)
		{
			int beginIndex = index - this.firstRow;
			int endIndex   = index + count-1 - this.firstRow;

			if ((endIndex < 0) ||
				(beginIndex >= this.textFrames.Length))
			{
				return Rectangle.Empty;
			}

			if (beginIndex < 0)
			{
				beginIndex = 0;
			}
			if (endIndex >= this.textFrames.Length)
			{
				endIndex = this.textFrames.Length-1;
			}

			return Rectangle.Union (this.textFrames[beginIndex], this.textFrames[endIndex]);
		}

		public override Size GetBestFitSize()
		{
			double margin = ScrollList.TextOffsetY * 2;
			double height = System.Math.Min (this.rowHeight * this.items.Count, this.MaxHeight);
			double width  = this.PreferredWidth;
			
			double dy = height;
			
			dy += margin;
			dy  = System.Math.Max (dy, this.MinHeight);
			dy  = System.Math.Min (dy, this.MaxHeight);
			dy -= margin;
			
			int n = (int) (dy / this.rowHeight);
			
			height = this.rowHeight * n + margin;
			
			return new Size(width, height);
		}

		public Size GetBestFitSizeBasedOnContent()
		{
			//	Donne les dimensions optimales pour la liste.
			//	La largeur est la largeur la plus grande de tous les textes contenus dans Items.
			//	La hauteur est la hauteur la plus grande de tous les textes contenus dans Items.

			double dx = 0;
			double dy = 0;

			TextLayout layout = this.CreateRowTextLayout ();
			
			int max = this.allRowsHaveSameWidth ? (this.items.Count == 0 ? 0 : 1) : this.items.Count;
			
			for (int i = 0; i < max; i++)
			{
				layout.Text = this.items[i];
				Size size = layout.SingleLineSize;
				dx = System.Math.Max (dx, size.Width);
				dy = System.Math.Max (dy, size.Height);
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

		public bool InvalidateTextLayouts()
		{
			if (this.isDirty)
			{
				return false;
			}
			else
			{
				this.SetDirty ();
				return true;
			}
		}


		public virtual bool IsItemSelected(int index)
		{
			return index == this.selectedRow;
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

						if (this.AutomaticScrollEnable)
						{
							this.autoScrollBehavior.ProcessEvent (Point.Zero);
						}

						this.OnSelectionActivated ();
						this.MouseSelectEnd ();
					}
					break;

				case MessageType.MouseWheel:
					if (message.Wheel < 0)
					{
						this.FirstVisibleRow++;
					}
					if (message.Wheel > 0)
					{
						this.FirstVisibleRow--;
					}
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
			if (this.rowHeight <= 0)
			{
				return false;
			}
			
			double y = this.Client.Size.Height - pos.Y - 1 - ScrollList.TextOffsetY;
			double x = pos.X - this.margins.Left;
			double h = this.visibleRows * this.rowHeight;

			if (this.AutomaticScrollEnable)
			{
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
			}

			int line = (int) (y / this.rowHeight);

			line = System.Math.Max (line, 0);
			line = System.Math.Min (line, this.visibleRows-1);

			this.MouseSelectRow (this.firstRow+line);
			
			return true;
		}

		protected virtual void MouseSelectRow(int index)
		{
			this.SelectedItemIndex = index;
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
			
			int sel = this.SelectedItemIndex;
			
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
			
			if ( this.SelectedItemIndex != sel )
			{
				this.selectItemBehavior.ClearSearch();
				
				sel = System.Math.Max(sel, 0);
				sel = System.Math.Min(sel, this.RowCount-1);
				
				this.SelectedItemIndex = sel;
				this.ShowSelected(ScrollShowMode.Extremity);
			}
			
			return true;
		}


		private void UpdateScroller()
		{
			//	Met à jour l'ascenseur en fonction de la liste.
			
			int total = this.items.Count;
			if ( total <= this.visibleRows )
			{
				if (this.scroller.Visibility)
				{
					this.scroller.Hide();
					this.UpdateMargins();
				}
			}
			else
			{
				this.scroller.MaxValue          = (decimal) (total-this.visibleRows);
				this.scroller.VisibleRangeRatio = (decimal) ((double)this.visibleRows/total);
				this.scroller.Value             = (decimal) (this.firstRow);
				this.scroller.SmallChange       = 1;
				this.scroller.LargeChange       = (decimal) (this.visibleRows/2.0);

				if (!this.scroller.Visibility)
				{
					this.scroller.Show();
					this.UpdateMargins();
				}
			}
		}

		private void SetDirty()
		{
			if ( this.isDirty == false )
			{
				this.isDirty = true;
				this.Invalidate();
			}
		}

		private void UpdateTextLayouts()
		{
			//	Met à jour les textes.

			if (this.isDirty)
			{
				this.UpdateScroller ();

				Rectangle rect  = this.Client.Bounds;
				Point     pos   = new Point (this.margins.Left, rect.Height-ScrollList.TextOffsetY);
				double    width = this.GetTextWidth ();
				
				int max = System.Math.Min (this.visibleRows, this.items.Count);
				
				for (int i = 0; i < max; i++)
				{
					double    h     = this.rowHeight;
					Rectangle frame = new Rectangle (pos.X, pos.Y - h, width, h);

					if (this.textLayouts[i] == null)
					{
						this.textLayouts[i] = this.CreateRowTextLayout ();
					}

					string text = (i+this.firstRow < this.items.Count) ? this.items[i+this.firstRow] : "";

					this.textLayouts[i].Text       = text;
					this.textLayouts[i].LayoutSize = frame.Size;
					this.textFrames[i]             = frame;

					pos = frame.BottomLeft;
				}

				this.isDirty = false;
			}
		}


		protected override void SetBoundsOverride(Rectangle oldRect, Rectangle newRect)
		{
			base.SetBoundsOverride(oldRect, newRect);
			this.UpdateGeometry ();
		}

		protected override void OnAdornerChanged()
		{
			this.UpdateGeometry ();
			this.UpdateMargins ();
			base.OnAdornerChanged ();
		}

		private void UpdateGeometry()
		{
			//	Met à jour la géométrie de l'ascenseur de la liste.

			if (this.rowHeight == 0)
			{
				return;
			}

			this.visibleRows = (int) ((this.ActualHeight-ScrollList.TextOffsetY*2)/this.rowHeight);
			
			if (this.visibleRows < 1)
			{
				this.visibleRows = 1;
			}

			this.textLayouts = new TextLayout[this.visibleRows];
			this.textFrames  = new Rectangle[this.visibleRows];

			this.SetDirty ();

			if (this.scroller != null)
			{
				this.UpdateMargins ();
				IAdorner adorner = Widgets.Adorners.Factory.Active;
				Rectangle rect = new Rectangle ();
				rect.Right  = this.Client.Size.Width-adorner.GeometryScrollerRightMargin;
				rect.Left   = rect.Right-this.scroller.PreferredWidth;
				rect.Bottom = adorner.GeometryScrollerBottomMargin+ScrollList.TextOffsetY-this.margins.Bottom;
				rect.Top    = this.Client.Size.Height-adorner.GeometryScrollerTopMargin-ScrollList.TextOffsetY+this.margins.Top;
				this.scroller.SetManualBounds (rect);
			}
		}

		private void UpdateMargins()
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

		private double GetTextWidth()
		{
			//	Calcule la largeur utile pour le texte.
			
			return this.Client.Size.Width - this.margins.Width;
		}


		protected virtual void AutomaticItemSelection(string search, bool continued)
		{
			int index = this.items.FindIndexByValueStartMatch (search, this.SelectedItemIndex + (continued ? 0 : 1));

			if (index < 0)
			{
				index = this.items.FindIndexByValueStartMatch (search);
			}

			if (index >= 0)
			{
				this.SelectedItemIndex = index;
				this.ShowSelected (ScrollShowMode.Extremity);
			}
		}

		protected virtual void AutomaticScroll(Point magnitude)
		{
			int index = this.SelectedItemIndex;

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

				this.MouseSelectRow (index);
			}
		}

		protected virtual void OnSelectedItemChanging()
		{
			var handler = (EventHandler) this.GetUserEventHandler ("SelectedItemChanging");
			if (handler != null)
			{
				handler (this);
			}
		}
		protected virtual void OnSelectedItemChanged()
		{
			//	Génère un événement pour dire que la sélection dans la liste a changé.
			EventHandler handler = (EventHandler) this.GetUserEventHandler ("SelectedItemChanged");
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
			this.UpdateTextLayouts ();

			IAdorner adorner = Widgets.Adorners.Factory.Active;

			Rectangle rect  = this.Client.Bounds;
			WidgetPaintState  state = this.GetPaintState ();

			if (this.scrollListStyle == ScrollListStyle.Menu)
			{
				Rectangle frame = Rectangle.Deflate (rect, new Margins (0, 0, ScrollList.TextOffsetY-this.margins.Top, ScrollList.TextOffsetY-this.margins.Bottom));
				adorner.PaintTextFieldBackground (graphics, frame, state, TextFieldStyle.Simple, TextFieldDisplayMode.Default, false, false);
			}
			else if (this.scrollListStyle != Widgets.ScrollListStyle.FrameLess)
			{
				Rectangle frame = Rectangle.Deflate (rect, new Margins (0, 0, ScrollList.TextOffsetY-this.margins.Top, ScrollList.TextOffsetY-this.margins.Bottom));
				adorner.PaintTextFieldBackground (graphics, frame, state, TextFieldStyle.Multiline, TextFieldDisplayMode.Default, false, false);
			}

			int max = System.Math.Min (this.visibleRows, this.items.Count);
			
			for (int i = 0; i < max; i++)
			{
				if (this.textLayouts[i] == null)
				{
					break;
				}

				Rectangle frame = this.textFrames[i];

				if ((this.IsItemSelected (this.firstRow + i)) &&
					((state & WidgetPaintState.Enabled) != 0))
				{
					TextLayout.SelectedArea[] areas = new TextLayout.SelectedArea[1]
					{
						new TextLayout.SelectedArea (frame)
					};

					adorner.PaintTextSelectionBackground (graphics, areas, state, PaintTextStyle.TextField, TextFieldDisplayMode.Default);

					state |= WidgetPaintState.Selected;
				}
				else
				{
					if (this.scrollListStyle == ScrollListStyle.AlternatingRows)
					{
						int lineIndex = this.firstRow + i;

						if (lineIndex % 2 == 1)
						{
							graphics.AddFilledRectangle (frame);
							graphics.RenderSolid (Color.FromAlphaRgb (0.1, 0, 0, 0));
						}
					}

					state &= ~WidgetPaintState.Selected;
				}

				adorner.PaintButtonTextLayout (graphics, frame.BottomLeft, this.textLayouts[i], state, ButtonStyle.ListItem);

				if (this.drawFrame)  // dessine les cadres ?
				{
					if (i < max-1)
					{
						frame.Bottom -= 1;
					}
					
					frame.Deflate (0.5);
					
					graphics.AddRectangle (frame);
					graphics.RenderSolid (adorner.ColorBorder);
				}
			}
		}

		
		#region IStringCollectionHost Members
		public void NotifyStringCollectionChanged()
		{
			int count = this.items.Count;

			if (this.firstRow + this.visibleRows > count)
			{
				this.FirstVisibleRow = System.Math.Max (0, count - this.visibleRows);
			}

			if (this.selectedRow >= count)
			{
				this.SelectedItemIndex = count - 1;
			}
			
			this.SetDirty ();
		}
		
		
		public Collections.StringCollection			Items
		{
			get
			{
				return this.items;
			}
		}
		#endregion

		#region IKeyedStringSelection Members

		public int								SelectedItemIndex
		{
			get
			{
				//	-1 => pas de ligne sélectionnée
				
				return this.selectedRow;
			}

			set
			{
				if ( value != -1 )
				{
					value = System.Math.Max(value, 0);
					value = System.Math.Min(value, this.items.Count-1);
				}
				if ( value != this.selectedRow )
				{
					this.OnSelectedItemChanging ();
					this.selectedRow = value;
					this.SetDirty();
					this.OnSelectedItemChanged ();
				}
			}
		}

		public string							SelectedItem
		{
			get
			{
				int index = this.SelectedItemIndex;
				if ( index < 0 )  return null;
				return this.Items[index];
			}
			
			set
			{
				this.SelectedItemIndex = this.Items.IndexOf(value);
			}
		}
		
		public string							SelectedKey
		{
			//	Nom de la ligne sélectionnée, null si aucune.
			get
			{
				if ( this.selectedRow == -1 )
				{
					return null;
				}
				
				return this.items.GetKey(this.selectedRow);
			}

			set
			{
				if ( this.SelectedKey != value )
				{
					int index = -1;
					
					if ( value != null )
					{
						index = this.items.FindIndexByKey(value);
					
						if ( index < 0 )
						{
							throw new System.ArgumentException(string.Format("No element named '{0}' in list", value));
						}
					}
					
					this.SelectedItemIndex = index;
				}
			}
		}


		public event EventHandler SelectedItemChanging
		{
			add
			{
				this.AddUserEventHandler ("SelectedItemChanging", value);
			}
			remove
			{
				this.RemoveUserEventHandler ("SelectedItemChanging", value);
			}
		}

		public event EventHandler SelectedItemChanged
		{
			add
			{
				this.AddUserEventHandler("SelectedItemChanged", value);
			}
			remove
			{
				this.RemoveUserEventHandler("SelectedItemChanged", value);
			}
		}
		#endregion

		private TextLayout CreateRowTextLayout()
		{
			TextLayout layout = new TextLayout ()
			{
				BreakMode = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine,
				Alignment = ContentAlignment.MiddleLeft
			};

			layout.SetEmbedder (this);

			return layout;
		}

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


		private const double					TextOffsetX = 3;
		private const double					TextOffsetY = 2;

		readonly Behaviors.SelectItemBehavior	selectItemBehavior;
		readonly Behaviors.AutoScrollBehavior	autoScrollBehavior;

		private ScrollListStyle					scrollListStyle;
		private bool							isDirty;
		private bool							drawFrame;
		private bool							mouseDown;
		readonly Collections.StringCollection	items;
		private TextLayout[]					textLayouts;
		private Rectangle[]						textFrames;

		private bool							allRowsHaveSameWidth;

		private Margins							margins;

		private double							rowHeight;
		readonly VScroller						scroller;
		private int								visibleRows;
		private int								firstRow;
		private int								selectedRow;
	}
}
