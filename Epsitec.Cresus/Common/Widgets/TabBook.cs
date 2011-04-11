using Epsitec.Common.Support;

namespace Epsitec.Common.Widgets
{
	public enum TabBookArrows
	{
		Right,							// les 2 flèches à droite
		LeftRight,						// flèches à gauche et à droite
		Stretch,						// jamais de flèches
	}

	/// <summary>
	/// Summary description for TabBook.
	/// </summary>
	public class TabBook : AbstractGroup, Collections.IWidgetCollectionHost<TabPage>
	{
		public TabBook()
		{
			this.arrows = TabBookArrows.Right;
			this.items = new TabPageCollection(this);
			
			this.InternalState &= ~WidgetInternalState.PossibleContainer;
			this.InternalPadding = new Drawing.Margins (2, 2, 2, 2);

			this.arrowLeft = new GlyphButton(this);
			this.arrowRight = new GlyphButton(this);
			this.arrowLeft.GlyphShape = GlyphShape.ArrowLeft;
			this.arrowRight.GlyphShape = GlyphShape.ArrowRight;
			this.arrowLeft.ButtonStyle = ButtonStyle.Scroller;
			this.arrowRight.ButtonStyle = ButtonStyle.Scroller;
			this.arrowLeft.Engaged       += this.HandleScrollButton;
			this.arrowRight.Engaged      += this.HandleScrollButton;
			this.arrowLeft.StillEngaged  += this.HandleScrollButton;
			this.arrowRight.StillEngaged += this.HandleScrollButton;
			this.arrowLeft.AutoRepeat = true;
			this.arrowRight.AutoRepeat = true;

			this.buttonMenu = new GlyphButton(this);
			this.buttonMenu.GlyphShape = GlyphShape.Menu;
			this.buttonMenu.ButtonStyle = ButtonStyle.Scroller;
			this.buttonMenu.Clicked += this.HandleButtonMenuClicked;

			this.buttonClose = new GlyphButton(this);
			this.buttonClose.GlyphShape = GlyphShape.Close;
			this.buttonClose.ButtonStyle = ButtonStyle.Scroller;
			this.buttonClose.Clicked += this.HandleButtonCloseClicked;
			
			this.TabNavigationMode = TabNavigationMode.ForwardTabActive;
		}
		
		public TabBook(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.arrowLeft.Engaged       -= this.HandleScrollButton;
				this.arrowRight.Engaged      -= this.HandleScrollButton;
				this.arrowLeft.StillEngaged  -= this.HandleScrollButton;
				this.arrowRight.StillEngaged -= this.HandleScrollButton;
				this.buttonClose.Clicked     -= this.HandleButtonCloseClicked;
				this.buttonMenu.Clicked      -= this.HandleButtonMenuClicked;
				
				TabPage[] pages = new TabPage[this.items.Count];
				this.items.CopyTo (pages, 0);
				
				for (int i = 0; i < pages.Length; i++)
				{
					pages[i].Dispose ();
				}
				
				this.items = null;
				
				this.arrowLeft = null;
				this.arrowRight = null;
				this.buttonMenu = null;
				this.buttonClose = null;
			}
			
			base.Dispose(disposing);
		}

		
		public bool							HasMenuButton
		{
			get
			{
				return this.hasMenuButton;
			}

			set
			{
				if ( this.hasMenuButton != value )
				{
					this.hasMenuButton = value;
					this.UpdateGlyphButtons();
				}
			}
		}

		public bool							HasCloseButton
		{
			get
			{
				return this.hasCloseButton;
			}

			set
			{
				if ( this.hasCloseButton != value )
				{
					this.hasCloseButton = value;
					this.UpdateGlyphButtons();
				}
			}
		}

		public GlyphButton					CloseButton
		{
			get
			{
				if ( this.hasCloseButton )
				{
					return this.buttonClose;
				}
				return null;
			}
		}
		
		public GlyphButton					MenuButton
		{
			get
			{
				if ( this.HasMenuButton )
				{
					return this.buttonMenu;
				}
				return null;
			}
		}

		public Drawing.Margins				InternalPadding
		{
			get;
			set;
		}
		
		public override Drawing.Margins GetInternalPadding()
		{
			var padding = this.InternalPadding;
			return new Drawing.Margins (padding.Left, padding.Right, this.TabHeight + padding.Top, padding.Bottom);
		}
		
		public TabPageCollection			Items
		{
			get
			{
				return this.items;
			}
		}

		public Direction					Direction
		{
			get
			{
				return this.direction;
			}
		}

		public TabBookArrows				Arrows
		{
			get
			{
				return this.arrows;
			}
			set
			{
				this.arrows = value;
			}
		}
		
		public int							ActivePageIndex
		{
			get
			{
				TabPage page = this.ActivePage;
				return (page == null) ? -1 : page.Index;
			}
			set
			{
				TabPage page = ((value >= 0) && (value < this.PageCount)) ? this.Items[value] : null;
				this.ActivePage = page;
			}
		}
		
		public TabPage						ActivePage
		{
			get
			{
				return this.activePage;
			}

			set
			{
				if ( this.activePage != value )
				{
					TabPage oldPage = this.activePage;
					this.activePage = value;
					this.UpdateVisiblePages();
					this.ShowSelectedTabButton();
					this.Invalidate();
					this.OnActivePageChanged(oldPage);
				}
				else
				{
					if ( this.ShowSelectedTabButton() )
					{
						this.Invalidate();
					}
				}
			}
		}
		
		public int							PageCount
		{
			get
			{
				return this.items.Count;
			}
		}
		
		public virtual double				TabHeight
		{
			get
			{
				return this.tabHeight;
			}
		}

		public Drawing.Rectangle			TabClipRectangle
		{
			get
			{
				Drawing.Rectangle rect = this.Client.Bounds;
				rect.Left  = this.TabOffsetMin;
				if ( rect.Left == 2 )  rect.Left = 0;
				rect.Right = this.TabOffsetMax;
				return rect;
			}
		}
		
		public double						TabOffsetMin
		{
			get
			{
				double min = 2;
				if ( this.scrollArrow )
				{
					if ( this.Arrows == TabBookArrows.LeftRight )
					{
						min = this.tabHeight-2;
					}
				}
				return min;
			}
		}
		
		public double						TabOffsetMax
		{
			get
			{
				double max = this.Client.Size.Width-2;
				if ( this.scrollArrow )
				{
					if ( this.Arrows == TabBookArrows.LeftRight )
					{
						max = this.Client.Size.Width-(this.tabHeight-2);
					}
					else if ( this.Arrows == TabBookArrows.Right )
					{
						max = this.Client.Size.Width-(this.tabHeight*2-6);
					}
				}
				if ( this.hasMenuButton  )  max -= this.tabHeight-4;
				if ( this.hasCloseButton )  max -= this.tabHeight-4;
				return max;
			}
		}

		
		public void Clear()
		{
			this.items.Clear();
		}
		
		
		public TabPage FindPage(int index)
		{
			return this.items[index] as TabPage;
		}
		
		public int FindPage(TabPage page)
		{
			return this.items.IndexOf(page);
		}

		
		private void HandleTabButton(object sender, MessageEventArgs e)
		{
			//	Gestion d'un événement lorsqu'un bouton d'onglet est pressé.
			if (!(sender is TabButton))
			{
				return;
			}

			TabButton button = sender as TabButton;

			foreach (TabPage page in this.items)
			{
				if (page.TabButton == button)
				{
					this.ActivePage = page;
					
					if (button.AutoFocus)
					{
						this.Focus();
					}
					
					break;
				}
			}
		}

		private void HandleScrollButton(object sender)
		{
			//	Gestion d'un événement lorsqu'un bouton de scroll < > est pressé.
			GlyphButton button = sender as GlyphButton;

			double move = 0;
			if ( button == this.arrowLeft )
			{
				move = -this.TabClipRectangle.Width/8;
			}
			else if ( button == this.arrowRight )
			{
				move = this.TabClipRectangle.Width/8;
			}

			move = System.Math.Floor(move);
			this.scrollOffset += move;
			this.UpdateOffset();
			this.UpdateButtons();
			this.Invalidate();
		}

		private void HandleButtonMenuClicked(object sender, MessageEventArgs e)
		{
			this.OnMenuClicked();
		}

		private void HandleButtonCloseClicked(object sender, MessageEventArgs e)
		{
			this.OnCloseClicked();
		}

		
		protected bool ShowSelectedTabButton()
		{
			//	Scroll les boutons pour rendre entièrement visible l'onglet actif.
			if ( !this.scrollArrow )  return false;

			double begin = 0;
			double end = 0;
			foreach ( TabPage page in this.items )
			{
				if ( page == this.ActivePage )  // est-ce la page active ?
				{
					Drawing.Rectangle rect = page.TabButton.ActualBounds;
					begin = rect.Left;
					end   = rect.Right;
				}
			}

			//	Scroll si l'onglet n'est pas entièrement visible.
			if ( begin < this.TabOffsetMin )  // dépasse à gauche ?
			{
				this.scrollOffset = this.scrollOffset+begin-this.TabOffsetMin;
				this.scrollOffset = System.Math.Floor(this.scrollOffset);
				this.UpdateButtons();
				return true;
			}
			else if ( end > this.TabOffsetMax )  // dépasse à droite ?
			{
				this.scrollOffset = this.scrollOffset+end-this.TabOffsetMax;
				this.scrollOffset = System.Math.Floor(this.scrollOffset);
				this.UpdateButtons();
				return true;
			}

			return false;
		}
		
		protected bool CenterSelectedTabButton()
		{
			//	Scroll les boutons pour centrer l'onglet actif.
			double pos = 0;

			if ( this.scrollArrow )
			{
				double min = this.TabOffsetMin;
				double max = this.TabOffsetMax;

				foreach ( TabPage page in this.items )
				{
					Drawing.Size size = page.TabSize;
					double len = System.Math.Floor(size.Width+size.Height);
					pos += len;
					if ( page == this.ActivePage )  break;
				}

				pos += (max-min)/2.0;
				pos = System.Math.Min(pos, this.scrollTotalWidth);

				pos -= max-min;
				pos = System.Math.Max(pos, 0.0);
			}

			if ( this.scrollOffset == pos )  return false;
			this.scrollOffset = pos;
			return true;
		}
		
		protected void UpdateOffset()
		{
			if ( this.items == null )  return;

			this.scrollTotalWidth = 0;
			foreach ( TabPage page in this.items )
			{
				Drawing.Size size = page.TabSize;
				double len = System.Math.Floor(size.Width+size.Height);
				this.scrollTotalWidth += len;
			}

			if ( this.Arrows == TabBookArrows.Stretch )
			{
				this.scrollOffset = 0;
			}
			else
			{
				this.scrollOffset = System.Math.Min(this.scrollOffset, this.scrollTotalWidth-this.TabOffsetMax);
				this.scrollOffset = System.Math.Max(this.scrollOffset, -this.TabOffsetMin);
			}
		}
		
		protected void UpdateDirection(Direction dir)
		{
			this.direction = dir;
			this.Invalidate();
		}
		
		protected void UpdateVisiblePages()
		{
			//	Met à jour la page visible. Toutes les autres sont cachées.
			foreach ( TabPage page in this.items )
			{
				if ( page == this.ActivePage )  // est-ce la page active ?
				{
					page.Visibility = true;
					page.TabButton.ActiveState = ActiveState.Yes;
					page.TabButton.InheritsParentFocus = true;
				}
				else
				{
					page.Visibility = false;
					page.TabButton.ActiveState = ActiveState.No;
					page.TabButton.InheritsParentFocus = false;
				}
			}
		}

		public void UpdateButtons()
		{
			//	Met à jour les boutons, en faisant disparaître les flèches si nécessaire.
			this.UpdateTabButtons();
			this.UpdateGlyphButtons();
		}

		public void UpdateAfterChanges()
		{
			//	Met à jour les boutons après un changement (insertion ou suppression
			//	d'un onglet, ou modification du texte d'un onglet).
			this.UpdateButtons();

			if ( this.CenterSelectedTabButton() )
			{
				this.Invalidate();
			}
		}

		protected void UpdateTabButtons()
		{
			//	Met à jour tous les boutons des onglets.
			
			this.scrollTotalWidth = 0;
			
			if ( this.items == null )  return;
			if ( this.items.Count == 0)  return;
			
			foreach ( TabPage page in this.items )
			{
				Drawing.Size size = page.TabSize;
				double len = System.Math.Floor(size.Width+size.Height);
				this.scrollTotalWidth += len;
			}
			
			System.Diagnostics.Debug.Assert(this.scrollTotalWidth > 0);
			
			Drawing.Rectangle rect = this.Client.Bounds;
			rect.Bottom = rect.Top-this.TabHeight;
			rect.Left -= this.scrollOffset;
			
			if ( this.Arrows == TabBookArrows.Stretch )
			{
				double width = this.Client.Bounds.Width-3;
				rect.Left = 0;
				foreach ( TabPage page in this.items )
				{
					Drawing.Size size = page.TabSize;
					double len = size.Width+size.Height;
					len *= width/this.scrollTotalWidth;
					len = System.Math.Floor(len);

					rect.Right = rect.Left+len;
					page.TabBounds = rect;
					rect.Left = rect.Right;
				}
			}
			else
			{
				foreach ( TabPage page in this.items )
				{
					Drawing.Size size = page.TabSize;
					double len = System.Math.Floor(size.Width+size.Height);

					rect.Right = rect.Left+len;
					page.TabBounds = rect;
					rect.Left = rect.Right;
				}
			}
		}
		
		protected void UpdateGlyphButtons()
		{
			//	Met à jour les 4 boutons spéciaux.
			if ( this.Arrows == TabBookArrows.Stretch )
			{
				this.scrollArrow = false;
			}
			else
			{
				this.scrollArrow = true;
				double width = this.TabOffsetMax-this.TabOffsetMin;
				this.scrollArrow = ( this.scrollTotalWidth > width );
			}

			if ( this.arrowLeft == null )  return;

			if ( this.scrollArrow )
			{
				if ( this.Arrows == TabBookArrows.LeftRight )
				{
					Drawing.Rectangle rect;
					rect = new Drawing.Rectangle(0, this.Client.Size.Height-this.tabHeight, this.tabHeight, this.tabHeight);
					rect.Deflate(2, 2);
					rect.Offset(-2, 0);
					this.arrowLeft.SetManualBounds(rect);
					this.arrowLeft.Visibility = true;
					this.arrowLeft.Enable = (this.scrollOffset > this.TabOffsetMin);

					double x = this.Client.Size.Width-this.tabHeight;
					if ( this.hasMenuButton  )  x -= this.tabHeight-4;
					if ( this.hasCloseButton )  x -= this.tabHeight-4;
					rect = new Drawing.Rectangle(x, this.Client.Size.Height-this.tabHeight, this.tabHeight, this.tabHeight);
					rect.Deflate(2, 2);
					rect.Offset(2, 0);
					this.arrowRight.SetManualBounds(rect);
					this.arrowRight.Visibility = true;
					this.arrowRight.Enable = (this.scrollOffset < this.scrollTotalWidth-this.TabOffsetMax);
				}
				else if ( this.Arrows == TabBookArrows.Right )
				{
					double x = this.Client.Size.Width-this.tabHeight*2;
					if ( this.hasMenuButton  )  x -= this.tabHeight-4;
					if ( this.hasCloseButton )  x -= this.tabHeight-4;
					Drawing.Rectangle rect = new Drawing.Rectangle(x, this.Client.Size.Height-this.tabHeight, this.tabHeight, this.tabHeight);
					rect.Deflate(2, 2);
					rect.Offset(6, 0);
					this.arrowLeft.SetManualBounds(rect);
					this.arrowLeft.Visibility = true;
					this.arrowLeft.Enable = (this.scrollOffset > this.TabOffsetMin);

					rect.Offset(this.tabHeight-4, 0);
					this.arrowRight.SetManualBounds(rect);
					this.arrowRight.Visibility = true;
					this.arrowRight.Enable = (this.scrollOffset < this.scrollTotalWidth-this.TabOffsetMax);
				}
			}
			else
			{
				this.arrowLeft.Visibility = false;
				this.arrowRight.Visibility = false;
			}

			if ( this.hasMenuButton )
			{
				double x = this.Client.Size.Width-(this.tabHeight-2);
				if ( this.hasCloseButton )  x -= this.tabHeight-4;
				Drawing.Rectangle rect = new Drawing.Rectangle(x, this.Client.Size.Height-this.tabHeight, this.tabHeight, this.tabHeight);
				rect.Inflate(-2, -2);
				this.buttonMenu.SetManualBounds(rect);
				this.buttonMenu.Visibility = true;
			}
			else
			{
				this.buttonMenu.Visibility = false;
			}

			if ( this.hasCloseButton )
			{
				Drawing.Rectangle rect = new Drawing.Rectangle(this.Client.Size.Width-(this.tabHeight-2), this.Client.Size.Height-this.tabHeight, this.tabHeight, this.tabHeight);
				rect.Inflate(-2, -2);
				this.buttonClose.SetManualBounds(rect);
				this.buttonClose.Visibility = true;
			}
			else
			{
				this.buttonClose.Visibility = false;
			}

			//	Pour détecter le clic sur les flèches en premier.
			if ( this.isRefreshNeeded )
			{
				this.Children.Remove(this.arrowLeft);
				this.Children.Remove(this.arrowRight);
				this.Children.Remove(this.buttonMenu);
				this.Children.Remove(this.buttonClose);
				this.Children.Add(this.arrowLeft);
				this.Children.Add(this.arrowRight);
				this.Children.Add(this.buttonMenu);
				this.Children.Add(this.buttonClose);
				this.isRefreshNeeded = false;
			}
		}

		
		protected virtual void HandlePageRankChanged(object sender)
		{
		}
		
		
		protected virtual void OnCloseClicked()
		{
			EventHandler handler = (EventHandler) this.GetUserEventHandler("CloseClicked");
			if (handler != null)
			{
				handler(this);
			}
		}
		
		protected virtual void OnMenuClicked()
		{
			EventHandler handler = (EventHandler) this.GetUserEventHandler("MenuClicked");
			if (handler != null)
			{
				handler(this);
			}
		}
		
		protected virtual void OnPageCountChanged()
		{
			EventHandler handler = (EventHandler) this.GetUserEventHandler("PageCountChanged");
			if (handler != null)
			{
				handler(this);
			}
		}
		
		protected virtual void OnActivePageChanged(TabPage oldPage)
		{
			EventHandler<CancelEventArgs> handler = this.GetUserEventHandler<CancelEventArgs> ("ActivePageChanged");
			if (handler != null)
			{
				CancelEventArgs e = new CancelEventArgs();
				handler(this, e);

				if (e.Cancel)  // annule le changement ?
				{
					this.activePage = oldPage;
					this.UpdateVisiblePages();
					this.ShowSelectedTabButton();
					this.Invalidate();
				}
			}
		}


		protected override void SetBoundsOverride(Drawing.Rectangle oldRect, Drawing.Rectangle newRect)
		{
			base.SetBoundsOverride(oldRect, newRect);
			this.UpdateGeometry ();
		}
		
		protected void UpdateGeometry()
		{
			this.UpdateOffset();
			this.UpdateButtons();
		}

		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			//	Dessine le groupe d'onglets.
			IAdorner          adorner = Widgets.Adorners.Factory.Active;
			WidgetPaintState  state   = this.GetPaintState ();
			Drawing.Rectangle part    = Drawing.Rectangle.Deflate (this.Client.Bounds, new Drawing.Margins (0, 0, this.TabHeight, 0));
			adorner.PaintTabFrame(graphics, part, state, Direction.Down);
		}
		
		protected override void ProcessMessage(Message message, Epsitec.Common.Drawing.Point pos)
		{
			if ( message.MessageType == MessageType.KeyDown )
			{
				int dir = 0;
				bool cycle = false;
				
				switch ( message.KeyCode )
				{
					case KeyCode.ArrowLeft:
						if ( this.IsFocused )
						{
							dir = -1;
						}
						break;
					
					case KeyCode.ArrowRight:
						if ( this.IsFocused )
						{
							dir = 1;
						}
						break;
						
					case KeyCode.Tab:
						if ( message.IsControlPressed )
						{
							dir   = message.IsShiftPressed ? -1 : 1;
							cycle = true;
						}
						break;
				}
				
				if ( dir != 0 )
				{
					int index = this.ActivePageIndex + dir;
					
					if (cycle)
					{
						if (index < 0)
						{
							index = this.PageCount - 1;
						}
						else if (index >= this.PageCount)
						{
							index = 0;
						}
					}
					else
					{
						index = System.Math.Max (index, 0);
						index = System.Math.Min (index, this.PageCount - 1);
					}
					
					if (this.ActivePageIndex != index)
					{
						this.ActivePageIndex = index;
						
						if (! this.IsFocused)
						{
							//	On n'a pas le focus clavier, il faut donc activer le focus de la
							//	nouvelle page activée.
							
							this.ActivePage.SetFocusOnTabWidget ();
						}
					}
					
					message.Consumer  = this;
					message.Swallowed = true;
					return;
				}
			}
			
			base.ProcessMessage(message, pos);
		}


		public override Drawing.Margins GetShapeMargins()
		{
			return Widgets.Adorners.Factory.Active.GeometryListShapeMargins;
		}

		
		#region IWidgetCollectionHost Members
		Collections.WidgetCollection<TabPage> Collections.IWidgetCollectionHost<TabPage>.GetWidgetCollection()
		{
			return this.Items;
		}
		
		public void NotifyInsertion(TabPage item)
		{
			TabBook oldBook = item.Book;
			
			if ((oldBook != null) &&
				(oldBook != this))
			{
				oldBook.items.Remove(item);
			}
			
			item.Anchor = AnchorStyles.All;
			item.Margins = new Drawing.Margins (0, 0, 0, 0);
			
			System.Diagnostics.Debug.Assert (oldBook == this);
			
			item.TabButton.SetParent (this);
			item.TabButton.Pressed += this.HandleTabButton;
			item.RankChanged += this.HandlePageRankChanged;
			this.isRefreshNeeded = true;
			
			this.UpdateVisiblePages();
			this.UpdateButtons();
			this.OnPageCountChanged();
		}

		public void NotifyRemoval(TabPage item)
		{
			int index = item.Index;

			item.TabButton.Pressed -= this.HandleTabButton;
			item.RankChanged -= this.HandlePageRankChanged;
			
			this.Children.Remove(item);
			this.Children.Remove(item.TabButton);
			this.isRefreshNeeded = true;
			
			if ( this.ActivePage == item )
			{
				int n = this.PageCount - 1;
				
				if ( index >= n )
				{
					index = n - 1;
				}
				
				this.ActivePageIndex = index;
			}
		}

		public void NotifyPostRemoval(TabPage item)
		{
			this.UpdateVisiblePages();
			this.UpdateButtons();
			this.OnPageCountChanged();
		}
		#endregion
		
		#region TabComparer class
		class TabComparer : System.Collections.IComparer
		{
			public int Compare(object x, object y)
			{
				TabPage page1 = x as TabPage;
				TabPage page2 = y as TabPage;
				
				return page1.Rank.CompareTo(page2.Rank);
			}
		}
		#endregion
		
		#region TabPageCollection Class
		public class TabPageCollection : Collections.WidgetCollection<TabPage>
		{
			public TabPageCollection(TabBook book) : base(book)
			{
			}
		}
		#endregion
		
		public event EventHandler			CloseClicked
		{
			add
			{
				this.AddUserEventHandler("CloseClicked", value);
			}
			remove
			{
				this.RemoveUserEventHandler("CloseClicked", value);
			}
		}

		public event EventHandler			MenuClicked
		{
			add
			{
				this.AddUserEventHandler("MenuClicked", value);
			}
			remove
			{
				this.RemoveUserEventHandler("MenuClicked", value);
			}
		}

		public event EventHandler			PageCountChanged
		{
			add
			{
				this.AddUserEventHandler("PageCountChanged", value);
			}
			remove
			{
				this.RemoveUserEventHandler("PageCountChanged", value);
			}
		}

		public event EventHandler<CancelEventArgs> ActivePageChanged
		{
			add
			{
				this.AddUserEventHandler("ActivePageChanged", value);
			}
			remove
			{
				this.RemoveUserEventHandler("ActivePageChanged", value);
			}
		}

		
		
		private TabBookArrows				arrows;
		private TabPageCollection			items;
		private TabPage						activePage;
		protected Direction					direction;
		protected double					tabHeight = 20;
		protected bool						scrollArrow = false;
		protected bool						hasMenuButton = false;
		protected bool						hasCloseButton = false;
		protected GlyphButton				arrowLeft;
		protected GlyphButton				arrowRight;
		protected GlyphButton				buttonMenu;
		protected GlyphButton				buttonClose;
		protected double					scrollTotalWidth;
		protected double					scrollOffset = 0;
		protected bool						isRefreshNeeded;
	}
}
