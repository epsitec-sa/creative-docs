namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// Summary description for TabBook.
	/// </summary>
	public class TabBook : AbstractGroup
	{
		public TabBook()
		{
			this.arrowLeft = new ArrowButton();
			this.arrowRight = new ArrowButton();
			this.arrowLeft.Direction = Direction.Left;
			this.arrowRight.Direction = Direction.Right;
			this.arrowLeft.ButtonStyle = ButtonStyle.Scroller;
			this.arrowRight.ButtonStyle = ButtonStyle.Scroller;
			this.arrowLeft.Engaged += new System.EventHandler(this.HandleScrollButton);
			this.arrowRight.Engaged += new System.EventHandler(this.HandleScrollButton);
			this.arrowLeft.StillEngaged += new System.EventHandler(this.HandleScrollButton);
			this.arrowRight.StillEngaged += new System.EventHandler(this.HandleScrollButton);
			this.arrowLeft.AutoRepeatEngaged = true;
			this.arrowRight.AutoRepeatEngaged = true;
			this.Children.Add(this.arrowLeft);
			this.Children.Add(this.arrowRight);
		}
		
		public override Drawing.Rectangle Inside
		{
			get
			{
				double width  = this.Client.Width;
				double height = this.Client.Height - this.TabHeight;
				
				Drawing.Rectangle rect = new Drawing.Rectangle(0, 0, width, height);
				rect.Inflate(-2, -2);
				return rect;
			}
		}
		
		public Direction Direction
		{
			get
			{
				return this.direction;
			}
		}

		public bool Sorted
		{
			get
			{
				return this.sorted;
			}

			set
			{
				this.sorted = value;
			}
		}
		
		public TabPage[] Pages
		{
			get
			{
				TabPage[] pages = new TabPage[this.tabPages.Count];
				this.tabPages.CopyTo(pages);
				return pages;
			}
		}
		
		public TabPage ActivePage
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
					this.Invalidate();
				}
			}
		}
		
		public int PageCount
		{
			get
			{
				return this.tabPages.Count;
			}
		}
		
		public virtual double TabHeight
		{
			get
			{
				return this.tabHeight;
			}
		}

		public Drawing.Rectangle TabRectangle
		{
			get
			{
				Drawing.Rectangle rect = this.Bounds;
				if ( this.scrollArrow )
				{
					rect.Right -= this.tabHeight*2-6;
				}
				return rect;
			}
		}
		
		
		public void Add(TabPage page)
		{
			System.Diagnostics.Debug.Assert(page != null);
			
			TabBook oldBook = page.Book;
			
			if ( oldBook != null )
			{
				oldBook.Remove(page);
			}
			
			page.Bounds = this.Inside;
			page.Anchor = AnchorStyles.LeftAndRight | AnchorStyles.TopAndBottom;
			
			this.tabPages.Add(page);
			this.Children.Add(page);
			this.Children.Add(page.TabButton);  // TabButton fils de TabBook !
			page.TabButton.Clicked += new MessageEventHandler(this.HandleTabButton);
			page.RankChanged += new System.EventHandler(this.HandlePageRankChanged);
			
			this.SortPages();
			this.UpdateVisiblePages();
			this.UpdateTabButtons();
			this.UpdateArrowButtons();
		}

		public void Remove(TabPage page)
		{
			int index = this.tabPages.IndexOf(page);
			
			if ( index < 0 )
			{
				throw new System.IndexOutOfRangeException("Page not found");
			}
			
			this.RemoveAt(index);
		}
		
		public void RemoveAt(int index)
		{
			TabPage page = this.tabPages[index] as TabPage;
			
			this.tabPages.RemoveAt(index);
			this.Children.Remove(page);
			
			if ( this.activePage == page )
			{
				if ( index >= this.tabPages.Count )
				{
					index --;
				}
				
				if ( index > 0 )
				{
					this.activePage = this.tabPages[index] as TabPage;
				}
				else
				{
					this.activePage = null;
				}
			}
			
			page.TabButton.Clicked -= new MessageEventHandler(this.HandleTabButton);
			page.RankChanged -= new System.EventHandler(this.HandlePageRankChanged);
		}
		
		public void Clear()
		{
			while ( this.tabPages.Count > 0 )
			{
				this.RemoveAt(0);
			}
		}
		
		
		public TabPage FindPage(int index)
		{
			return this.tabPages[index] as TabPage;
		}
		
		public int FindPage(TabPage page)
		{
			return this.tabPages.IndexOf(page);
		}


		// Gestion d'un événement lorsqu'un bouton est pressé.
		private void HandleTabButton(object sender, MessageEventArgs e)
		{
			TabButton button = sender as TabButton;

			foreach ( TabPage page in this.tabPages )
			{
				if ( page.TabButton == button )
				{
					this.ActivePage = page;
				}
			}
		}

		// Gestion d'un événement lorsqu'un bouton < > est pressé.
		private void HandleScrollButton(object sender, System.EventArgs e)
		{
			ArrowButton button = sender as ArrowButton;

			double move = 0;
			if ( button == this.arrowLeft )
			{
				move = -this.TabRectangle.Width/4;
			}
			else if ( button == this.arrowRight )
			{
				move = this.TabRectangle.Width/4;
			}

			this.scrollOffset += move;
			this.scrollOffset = System.Math.Max(this.scrollOffset, 0);
			this.scrollOffset = System.Math.Min(this.scrollOffset, this.scrollTotalWidth-this.TabRectangle.Width);

			this.UpdateTabButtons();
			this.UpdateArrowButtons();
			this.Invalidate();
		}


		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry ();
			
			Direction newDir = this.RootDirection;
			Direction oldDir = this.direction;
			
			if ( oldDir != newDir )
			{
				this.UpdateDirection(newDir);
			}

			this.UpdateTabButtons();
			this.UpdateArrowButtons();
		}
		
		protected virtual void UpdateDirection(Direction dir)
		{
			this.direction = dir;
			this.Invalidate();
		}
		
		// Met à jour la page visible. Toutes les autres sont cachées.
		protected virtual void UpdateVisiblePages()
		{
			double begin = 0;
			double end = 0;
			foreach ( TabPage page in this.tabPages )
			{
				if ( page == this.activePage )  // est-ce la page active ?
				{
					page.SetVisible(true);
					page.TabButton.ActiveState = WidgetState.ActiveYes;

					Drawing.Rectangle rect = page.TabButton.Bounds;
					begin = rect.Left;
					end   = rect.Right;
				}
				else
				{
					page.SetVisible(false);
					page.TabButton.ActiveState = WidgetState.ActiveNo;
				}
			}

			// Scroll si l'onglet n'est pas entièrement visible.
			if ( this.scrollArrow )
			{
				if ( end > this.TabRectangle.Width )  // dépasse à droite ?
				{
					this.scrollOffset = this.scrollOffset+end-this.TabRectangle.Width;
					UpdateTabButtons();
					UpdateArrowButtons();
				}
				if ( begin < 0 )  // dépasse à gauche ?
				{
					this.scrollOffset = this.scrollOffset+begin;
					UpdateTabButtons();
					UpdateArrowButtons();
				}
			}
		}
		
		// Met à jour tous les boutons des onglets.
		protected virtual void UpdateTabButtons()
		{
			Drawing.Rectangle rect = new Drawing.Rectangle(0, 0, this.Client.Width, this.Client.Height);
			rect.Bottom = rect.Top-this.TabHeight;
			rect.Left -= this.scrollOffset;

			this.scrollTotalWidth = 0;
			foreach ( TabPage page in this.tabPages )
			{
				Drawing.Size size = page.TabSize;
				double len = System.Math.Floor(size.Width+size.Height);
				this.scrollTotalWidth += len;

				rect.Right = rect.Left+len;
				page.TabBounds = rect;
				rect.Left = rect.Right;
			}
		}
		
		// Met à jour les 2 boutons flèche < > pour scroller.
		protected virtual void UpdateArrowButtons()
		{
			this.scrollArrow = ( this.scrollTotalWidth > this.Client.Width );

			if ( this.arrowLeft == null || this.arrowRight == null )  return;

			if ( this.scrollArrow )
			{
				Drawing.Rectangle rect = new Drawing.Rectangle(this.Client.Width-this.tabHeight*2, this.Client.Height-this.tabHeight, this.tabHeight, this.tabHeight);
				rect.Inflate(-2, -2);
				rect.Offset(6, 0);
				this.arrowLeft.Bounds = rect;
				this.arrowLeft.SetVisible(true);
				this.arrowLeft.SetEnabled(this.scrollOffset > 0);

				rect.Offset(this.tabHeight-4, 0);
				this.arrowRight.Bounds = rect;
				this.arrowRight.SetVisible(true);
				this.arrowRight.SetEnabled(this.scrollOffset < this.scrollTotalWidth-this.TabRectangle.Width);
			}
			else
			{
				this.arrowLeft.SetVisible(false);
				this.arrowRight.SetVisible(false);
			}

			// Pour détecter le clic sur les flèches en premier.
			this.Children.Remove(this.arrowLeft);
			this.Children.Remove(this.arrowRight);
			this.Children.Add(this.arrowLeft);
			this.Children.Add(this.arrowRight);
		}

		protected virtual void SortPages()
		{
			if ( this.sorted )
			{
				this.tabPages.Sort(new TabComparer());
				this.Invalidate();
			}
		}
		
		protected virtual void HandlePageRankChanged(object sender, System.EventArgs e)
		{
			this.SortPages();
		}
		
		
		
		
		class TabComparer : System.Collections.IComparer
		{
			public int Compare(object x, object y)
			{
				TabPage page1 = x as TabPage;
				TabPage page2 = y as TabPage;
				
				return page1.Rank.CompareTo(page2.Rank);
			}
		}
		
		
		// Dessine le groupe d'onglets.
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			IAdorner adorner = Widgets.Adorner.Factory.Active;

			Drawing.Rectangle rect  = new Drawing.Rectangle(0, 0, this.Client.Width, this.Client.Height);
			WidgetState       state = this.PaintState;
			Direction         dir   = this.RootDirection;

			Drawing.Rectangle part = new Drawing.Rectangle();

			part = rect;
			part.Bottom = part.Top-this.TabHeight;
			//adorner.PaintTabBand(graphics, part, state, dir);
			
			part = rect;
			part.Top -= this.TabHeight;
			adorner.PaintTabFrame(graphics, part, state, dir);
		}
		
		
		protected System.Collections.ArrayList	tabPages = new System.Collections.ArrayList ();
		protected TabPage						activePage;
		protected Direction						direction;
		protected double						tabHeight = 20;
		protected bool							sorted = false;
		protected bool							scrollArrow = false;
		protected ArrowButton					arrowLeft;
		protected ArrowButton					arrowRight;
		protected double						scrollTotalWidth;
		protected double						scrollOffset = 0;
	}
}
