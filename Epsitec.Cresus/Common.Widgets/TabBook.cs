namespace Epsitec.Common.Widgets
{
	public enum TabBookArrows
	{
		Right,							// les 2 fl�ches � droite
		LeftRight,						// fl�ches � gauche et � droite
		Stretch,						// jamais de fl�ches
	}

	/// <summary>
	/// Summary description for TabBook.
	/// </summary>
	[Support.SuppressBundleSupport]
	public class TabBook : AbstractGroup, Helpers.IWidgetCollectionHost
	{
		public TabBook()
		{
			this.arrows = TabBookArrows.Right;
			this.items = new TabPageCollection(this);
			this.items.AutoEmbedding = true;
			
			this.InternalState &= ~InternalState.PossibleContainer;

			this.arrowLeft = new GlyphButton(this);
			this.arrowRight = new GlyphButton(this);
			this.arrowLeft.GlyphShape = GlyphShape.ArrowLeft;
			this.arrowRight.GlyphShape = GlyphShape.ArrowRight;
			this.arrowLeft.ButtonStyle = ButtonStyle.Scroller;
			this.arrowRight.ButtonStyle = ButtonStyle.Scroller;
			this.arrowLeft.Engaged += new Support.EventHandler(this.HandleScrollButton);
			this.arrowRight.Engaged += new Support.EventHandler(this.HandleScrollButton);
			this.arrowLeft.StillEngaged += new Support.EventHandler(this.HandleScrollButton);
			this.arrowRight.StillEngaged += new Support.EventHandler(this.HandleScrollButton);
			this.arrowLeft.AutoRepeatEngaged = true;
			this.arrowRight.AutoRepeatEngaged = true;

			this.buttonMenu = new GlyphButton(this);
			this.buttonMenu.GlyphShape = GlyphShape.Menu;
			this.buttonMenu.ButtonStyle = ButtonStyle.Scroller;
			this.buttonMenu.Clicked += new MessageEventHandler(this.HandleButtonMenuClicked);

			this.buttonClose = new GlyphButton(this);
			this.buttonClose.GlyphShape = GlyphShape.Close;
			this.buttonClose.ButtonStyle = ButtonStyle.Scroller;
			this.buttonClose.Clicked += new MessageEventHandler(this.HandleButtonCloseClicked);
			
			this.TabNavigation = Widget.TabNavigationMode.ForwardTabActive;
		}
		
		public TabBook(Widget embedder) : this()
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
			if ( disposing )
			{
				this.arrowLeft.Engaged -= new Support.EventHandler(this.HandleScrollButton);
				this.arrowRight.Engaged -= new Support.EventHandler(this.HandleScrollButton);
				this.arrowLeft.StillEngaged -= new Support.EventHandler(this.HandleScrollButton);
				this.arrowRight.StillEngaged -= new Support.EventHandler(this.HandleScrollButton);
				this.buttonClose.Clicked -= new MessageEventHandler(this.HandleButtonCloseClicked);
				this.buttonMenu.Clicked -= new MessageEventHandler(this.HandleButtonMenuClicked);
				
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
		
		public override Drawing.Margins		ExtraPadding
		{
			get
			{
				return new Drawing.Margins (2, 2, this.TabHeight + 2, 2);
			}
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
					this.activePage = value;
					this.UpdateVisiblePages();
					this.ShowSelectedTabButton();
					this.Invalidate();
					this.OnActivePageChanged();
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
				double max = this.Client.Width-2;
				if ( this.scrollArrow )
				{
					if ( this.Arrows == TabBookArrows.LeftRight )
					{
						max = this.Client.Width-(this.tabHeight-2);
					}
					else if ( this.Arrows == TabBookArrows.Right )
					{
						max = this.Client.Width-(this.tabHeight*2-6);
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
			// Gestion d'un �v�nement lorsqu'un bouton d'onglet est press�.
			if ( !(sender is TabButton) )  return;
			TabButton button = sender as TabButton;

			foreach ( TabPage page in this.items )
			{
				if ( page.TabButton == button )
				{
					this.ActivePage = page;
					
					if ( button.AutoFocus )
					{
						this.Focus();
					}
					
					break;
				}
			}
		}

		private void HandleScrollButton(object sender)
		{
			// Gestion d'un �v�nement lorsqu'un bouton de scroll < > est press�.
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
			// Scroll les boutons pour rendre enti�rement visible l'onglet actif.
			if ( !this.scrollArrow )  return false;

			double begin = 0;
			double end = 0;
			foreach ( TabPage page in this.items )
			{
				if ( page == this.ActivePage )  // est-ce la page active ?
				{
					Drawing.Rectangle rect = page.TabButton.Bounds;
					begin = rect.Left;
					end   = rect.Right;
				}
			}

			// Scroll si l'onglet n'est pas enti�rement visible.
			if ( begin < this.TabOffsetMin )  // d�passe � gauche ?
			{
				this.scrollOffset = this.scrollOffset+begin-this.TabOffsetMin;
				this.scrollOffset = System.Math.Floor(this.scrollOffset);
				this.UpdateButtons();
				return true;
			}
			else if ( end > this.TabOffsetMax )  // d�passe � droite ?
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
			// Scroll les boutons pour centrer l'onglet actif.
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
			// Met � jour la page visible. Toutes les autres sont cach�es.
			foreach ( TabPage page in this.items )
			{
				if ( page == this.ActivePage )  // est-ce la page active ?
				{
					page.SetVisible(true);
					page.TabButton.ActiveState = WidgetState.ActiveYes;
					page.TabButton.InheritFocus = true;
				}
				else
				{
					page.SetVisible(false);
					page.TabButton.ActiveState = WidgetState.ActiveNo;
					page.TabButton.InheritFocus = false;
				}
			}
		}

		public void UpdateButtons()
		{
			// Met � jour les boutons, en faisant dispara�tre les fl�ches si n�cessaire.
			this.UpdateTabButtons();
			this.UpdateGlyphButtons();
		}

		public void UpdateAfterChanges()
		{
			// Met � jour les boutons apr�s un changement (insertion ou suppression
			// d'un onglet, ou modification du texte d'un onglet).
			this.UpdateButtons();

			if ( this.CenterSelectedTabButton() )
			{
				this.Invalidate();
			}
		}

		protected void UpdateTabButtons()
		{
			// Met � jour tous les boutons des onglets.
			if ( this.items == null )  return;

			Drawing.Rectangle rect = this.Client.Bounds;
			rect.Bottom = rect.Top-this.TabHeight;
			rect.Left -= this.scrollOffset;

			this.scrollTotalWidth = 0;
			foreach ( TabPage page in this.items )
			{
				Drawing.Size size = page.TabSize;
				double len = System.Math.Floor(size.Width+size.Height);
				this.scrollTotalWidth += len;

				rect.Right = rect.Left+len;
				page.TabBounds = rect;
				rect.Left = rect.Right;
			}

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
		}
		
		protected void UpdateGlyphButtons()
		{
			// Met � jour les 4 boutons sp�ciaux.
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
					rect = new Drawing.Rectangle(0, this.Client.Height-this.tabHeight, this.tabHeight, this.tabHeight);
					rect.Deflate(2, 2);
					rect.Offset(-2, 0);
					this.arrowLeft.Bounds = rect;
					this.arrowLeft.SetVisible(true);
					this.arrowLeft.SetEnabled(this.scrollOffset > this.TabOffsetMin);

					double x = this.Client.Width-this.tabHeight;
					if ( this.hasMenuButton  )  x -= this.tabHeight-4;
					if ( this.hasCloseButton )  x -= this.tabHeight-4;
					rect = new Drawing.Rectangle(x, this.Client.Height-this.tabHeight, this.tabHeight, this.tabHeight);
					rect.Deflate(2, 2);
					rect.Offset(2, 0);
					this.arrowRight.Bounds = rect;
					this.arrowRight.SetVisible(true);
					this.arrowRight.SetEnabled(this.scrollOffset < this.scrollTotalWidth-this.TabOffsetMax);
				}
				else if ( this.Arrows == TabBookArrows.Right )
				{
					double x = this.Client.Width-this.tabHeight*2;
					if ( this.hasMenuButton  )  x -= this.tabHeight-4;
					if ( this.hasCloseButton )  x -= this.tabHeight-4;
					Drawing.Rectangle rect = new Drawing.Rectangle(x, this.Client.Height-this.tabHeight, this.tabHeight, this.tabHeight);
					rect.Deflate(2, 2);
					rect.Offset(6, 0);
					this.arrowLeft.Bounds = rect;
					this.arrowLeft.SetVisible(true);
					this.arrowLeft.SetEnabled(this.scrollOffset > this.TabOffsetMin);

					rect.Offset(this.tabHeight-4, 0);
					this.arrowRight.Bounds = rect;
					this.arrowRight.SetVisible(true);
					this.arrowRight.SetEnabled(this.scrollOffset < this.scrollTotalWidth-this.TabOffsetMax);
				}
			}
			else
			{
				this.arrowLeft.SetVisible(false);
				this.arrowRight.SetVisible(false);
			}

			if ( this.hasMenuButton )
			{
				double x = this.Client.Width-(this.tabHeight-2);
				if ( this.hasCloseButton )  x -= this.tabHeight-4;
				Drawing.Rectangle rect = new Drawing.Rectangle(x, this.Client.Height-this.tabHeight, this.tabHeight, this.tabHeight);
				rect.Inflate(-2, -2);
				this.buttonMenu.Bounds = rect;
				this.buttonMenu.SetVisible(true);
			}
			else
			{
				this.buttonMenu.SetVisible(false);
			}

			if ( this.hasCloseButton )
			{
				Drawing.Rectangle rect = new Drawing.Rectangle(this.Client.Width-(this.tabHeight-2), this.Client.Height-this.tabHeight, this.tabHeight, this.tabHeight);
				rect.Inflate(-2, -2);
				this.buttonClose.Bounds = rect;
				this.buttonClose.SetVisible(true);
			}
			else
			{
				this.buttonClose.SetVisible(false);
			}

			// Pour d�tecter le clic sur les fl�ches en premier.
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
			if ( this.CloseClicked != null )
			{
				this.CloseClicked(this);
			}
		}
		
		protected virtual void OnMenuClicked()
		{
			if ( this.MenuClicked != null )
			{
				this.MenuClicked(this);
			}
		}
		
		protected virtual void OnPageCountChanged()
		{
			if ( this.PageCountChanged != null )
			{
				this.PageCountChanged(this);
			}
		}
		
		protected virtual void OnActivePageChanged()
		{
			if ( this.ActivePageChanged != null )
			{
				this.ActivePageChanged(this);
			}
		}

		
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();
			
			this.UpdateOffset();
			this.UpdateButtons();
		}

		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			// Dessine le groupe d'onglets.
			IAdorner adorner = Widgets.Adorner.Factory.Active;

			Drawing.Rectangle rect  = this.Client.Bounds;
			WidgetState       state = this.PaintState;

			Drawing.Rectangle part = new Drawing.Rectangle();

			part = rect;
			part.Bottom = part.Top-this.TabHeight;
			//adorner.PaintTabBand(graphics, part, state, Direction.Down);
			
			part = rect;
			part.Top -= this.TabHeight;
			adorner.PaintTabFrame(graphics, part, state, Direction.Down);
		}
		
		protected override void ProcessMessage(Message message, Epsitec.Common.Drawing.Point pos)
		{
			if ( message.Type == MessageType.KeyDown )
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
						if ( message.IsCtrlPressed )
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
							//	nouvelle page activ�e.
							
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

		
		public override Drawing.Rectangle GetShapeBounds()
		{
			IAdorner adorner = Widgets.Adorner.Factory.Active;
			Drawing.Rectangle rect = this.Client.Bounds;
			rect.Inflate(adorner.GeometryListShapeBounds);
			return rect;
		}

		
		#region IWidgetCollectionHost Members
		Helpers.WidgetCollection Helpers.IWidgetCollectionHost.GetWidgetCollection()
		{
			return this.Items;
		}
		
		public void NotifyInsertion(Widget widget)
		{
			TabPage item    = widget as TabPage;
			TabBook oldBook = item.Book;
			
			if ((oldBook != null) &&
				(oldBook != this))
			{
				oldBook.items.Remove(item);
			}
			
//			item.Bounds = this.InnerBounds;
			item.Anchor = AnchorStyles.All;
			item.AnchorMargins = new Drawing.Margins (2, 2, this.TabHeight + 2, 2);
			
			System.Diagnostics.Debug.Assert (oldBook == this);
			
			item.TabButton.SetParent (this);
			item.TabButton.Pressed += new MessageEventHandler(this.HandleTabButton);
			item.RankChanged += new Support.EventHandler(this.HandlePageRankChanged);
			this.isRefreshNeeded = true;
			
			this.UpdateVisiblePages();
			this.UpdateButtons();
			this.OnPageCountChanged();
		}

		public void NotifyRemoval(Widget widget)
		{
			TabPage item  = widget as TabPage;
			int     index = item.Index;
			
			item.TabButton.Clicked -= new MessageEventHandler(this.HandleTabButton);
			item.RankChanged       -= new Support.EventHandler(this.HandlePageRankChanged);
			
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
		
		public void NotifyPostRemoval(Widget widget)
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
		public class TabPageCollection : Helpers.WidgetCollection
		{
			public TabPageCollection(TabBook book) : base(book)
			{
			}
			
			public new TabPage this[int index]
			{
				get
				{
					return base[index] as TabPage;
				}
			}
			
			public new TabPage this[string name]
			{
				get
				{
					return base[name] as TabPage;
				}
			}
		}
		#endregion
		
		public event Support.EventHandler	CloseClicked;
		public event Support.EventHandler	MenuClicked;
		public event Support.EventHandler	PageCountChanged;
		public event Support.EventHandler	ActivePageChanged;
		
		
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
