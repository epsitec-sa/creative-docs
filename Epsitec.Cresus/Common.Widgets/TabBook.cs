namespace Epsitec.Common.Widgets
{
	public enum TabBookStyle
	{
		Normal,							// flèches à gauche et à droite
		Right,							// les 2 flèches à droite
		Ellipsis,						// ajoute ... pour résumer
	}

	/// <summary>
	/// Summary description for TabBook.
	/// </summary>
	[Support.SuppressBundleSupport]
	public class TabBook : AbstractGroup
	{
		public TabBook(TabBookStyle type)
		{
			this.type = type;

			this.arrowLeft = new ArrowButton();
			this.arrowRight = new ArrowButton();
			this.arrowLeft.Direction = Direction.Left;
			this.arrowRight.Direction = Direction.Right;
			this.arrowLeft.ButtonStyle = ButtonStyle.Scroller;
			this.arrowRight.ButtonStyle = ButtonStyle.Scroller;
			this.arrowLeft.Engaged += new EventHandler(this.HandleScrollButton);
			this.arrowRight.Engaged += new EventHandler(this.HandleScrollButton);
			this.arrowLeft.StillEngaged += new EventHandler(this.HandleScrollButton);
			this.arrowRight.StillEngaged += new EventHandler(this.HandleScrollButton);
			this.arrowLeft.AutoRepeatEngaged = true;
			this.arrowRight.AutoRepeatEngaged = true;
			this.Children.Add(this.arrowLeft);
			this.Children.Add(this.arrowRight);
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.arrowLeft.Engaged -= new EventHandler(this.HandleScrollButton);
				this.arrowRight.Engaged -= new EventHandler(this.HandleScrollButton);
				this.arrowLeft.StillEngaged -= new EventHandler(this.HandleScrollButton);
				this.arrowRight.StillEngaged -= new EventHandler(this.HandleScrollButton);
				this.arrowLeft = null;
				this.arrowRight = null;

				this.Clear();
			}
			
			base.Dispose(disposing);
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
					this.ShowSelectedTabButton();
					this.Invalidate();
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

		public Drawing.Rectangle TabClipRectangle
		{
			get
			{
				Drawing.Rectangle rect = this.Client.Bounds;
				if ( this.scrollArrow )
				{
					if ( this.type == TabBookStyle.Normal )
					{
						if ( this.arrowLeft.IsVisible  )  rect.Left  += this.tabHeight-2;
						if ( this.arrowRight.IsVisible )  rect.Right -= this.tabHeight-2;
					}
					if ( this.type == TabBookStyle.Right )
					{
						rect.Right -= this.tabHeight*2-6;
					}
				}
				
				return rect;
			}
		}
		
		public double TabOffsetMin
		{
			get
			{
				double min = 0;
				if ( this.scrollArrow )
				{
					if ( this.type == TabBookStyle.Normal )
					{
						if ( this.arrowLeft.IsVisible  )  min += this.tabHeight-2;
						//min += this.tabHeight-2;
					}
				}
				return min;
			}
		}
		
		public double TabOffsetMax
		{
			get
			{
				double max = this.Client.Width;
				if ( this.scrollArrow )
				{
					if ( this.type == TabBookStyle.Normal )
					{
						if ( this.arrowRight.IsVisible )  max -= this.tabHeight-2;
						//max -= this.tabHeight-2;
					}
					if ( this.type == TabBookStyle.Right )
					{
						max -= this.tabHeight*2-6;
					}
				}
				return max;
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
			page.TabButton.Pressed += new MessageEventHandler(this.HandleTabButton);
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


		// Gestion d'un événement lorsqu'un bouton d'onglet est pressé.
		private void HandleTabButton(object sender, MessageEventArgs e)
		{
			if ( !(sender is TabButton) )  return;
			TabButton button = sender as TabButton;

			foreach ( TabPage page in this.tabPages )
			{
				if ( page.TabButton == button )
				{
					this.ActivePage = page;
				}
			}
		}

		// Gestion d'un événement lorsqu'un bouton de scroll < > est pressé.
		private void HandleScrollButton(object sender)
		{
			ArrowButton button = sender as ArrowButton;

			double move = 0;
			if ( button == this.arrowLeft )
			{
				move = -this.TabClipRectangle.Width/8;
			}
			else if ( button == this.arrowRight )
			{
				move = this.TabClipRectangle.Width/8;
			}

			this.scrollOffset += move;
			this.AdjustScrollOffset();
			this.UpdateButtons();
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
		
		protected void UpdateDirection(Direction dir)
		{
			this.direction = dir;
			this.Invalidate();
		}
		
		// Met à jour la page visible. Toutes les autres sont cachées.
		protected void UpdateVisiblePages()
		{
			foreach ( TabPage page in this.tabPages )
			{
				if ( page == this.activePage )  // est-ce la page active ?
				{
					page.SetVisible(true);
					page.TabButton.ActiveState = WidgetState.ActiveYes;
				}
				else
				{
					page.SetVisible(false);
					page.TabButton.ActiveState = WidgetState.ActiveNo;
				}
			}
		}

		// Scroll les boutons pour rendre entièrement visible l'onglet actif.
		protected bool ShowSelectedTabButton()
		{
			if ( !this.scrollArrow )  return false;

			double begin = 0;
			double end = 0;
			foreach ( TabPage page in this.tabPages )
			{
				if ( page == this.activePage )  // est-ce la page active ?
				{
					Drawing.Rectangle rect = page.TabButton.Bounds;
					begin = rect.Left;
					end   = rect.Right;
				}
			}

			// Scroll si l'onglet n'est pas entièrement visible.
			if ( begin < this.TabOffsetMin )  // dépasse à gauche ?
			{
				this.scrollOffset = this.scrollOffset+begin-this.TabOffsetMin;
				this.UpdateButtons();
				return true;
			}
			else if ( end > this.TabOffsetMax )  // dépasse à droite ?
			{
				this.scrollOffset = this.scrollOffset+end-this.TabOffsetMax;
				this.UpdateButtons();
				return true;
			}

			return false;
		}
		
		// Met à jour les boutons, en faisant disparaître les flèches si nécessaire.
		protected void UpdateButtons()
		{
			this.UpdateTabButtons();
			this.UpdateArrowButtons();

			if ( this.AdjustScrollOffset() )
			{
				this.UpdateTabButtons();
			}
		}

		// Ajuste l'offset pour le scroll. Retourne true s'il y a eu ajustement.
		protected bool AdjustScrollOffset()
		{
			double offset = this.scrollOffset;
			offset = System.Math.Max(offset, -this.TabOffsetMin);
			offset = System.Math.Min(offset, this.scrollTotalWidth-this.TabOffsetMax);
			if ( offset == this.scrollOffset )  return false;
			this.scrollOffset = offset;
			return true;
		}

		// Met à jour tous les boutons des onglets.
		protected void UpdateTabButtons()
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
		protected void UpdateArrowButtons()
		{
			this.scrollArrow = ( this.scrollTotalWidth > this.Client.Width );

			if ( this.arrowLeft == null || this.arrowRight == null )  return;

			if ( this.scrollArrow )
			{
				if ( this.type == TabBookStyle.Normal )
				{
					Drawing.Rectangle rect;
					rect = new Drawing.Rectangle(0, this.Client.Height-this.tabHeight, this.tabHeight, this.tabHeight);
					rect.Inflate(-2, -2);
					rect.Offset(-2, 0);
					this.arrowLeft.Bounds = rect;
					this.arrowLeft.SetVisible(this.scrollOffset > this.TabOffsetMin);
					//this.arrowLeft.SetVisible(true);
					//this.arrowLeft.SetEnabled(this.scrollOffset > this.TabOffsetMin);

					rect = new Drawing.Rectangle(this.Client.Width-this.tabHeight, this.Client.Height-this.tabHeight, this.tabHeight, this.tabHeight);
					rect.Inflate(-2, -2);
					rect.Offset(2, 0);
					this.arrowRight.Bounds = rect;
					this.arrowRight.SetVisible(this.scrollOffset < this.scrollTotalWidth-this.TabOffsetMax);
					//this.arrowRight.SetVisible(true);
					//this.arrowRight.SetEnabled(this.scrollOffset < this.scrollTotalWidth-this.TabOffsetMax);
				}

				if ( this.type == TabBookStyle.Right )
				{
					Drawing.Rectangle rect = new Drawing.Rectangle(this.Client.Width-this.tabHeight*2, this.Client.Height-this.tabHeight, this.tabHeight, this.tabHeight);
					rect.Inflate(-2, -2);
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
		

		protected TabBookStyle					type = TabBookStyle.Normal;
		protected System.Collections.ArrayList	tabPages = new System.Collections.ArrayList();
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
