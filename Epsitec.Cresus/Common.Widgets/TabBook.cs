namespace Epsitec.Common.Widgets
{
	public enum TabBookStyle
	{
		Right,							// les 2 flèches à droite
		LeftRight,						// flèches à gauche et à droite
	}

	/// <summary>
	/// Summary description for TabBook.
	/// </summary>
	[Support.SuppressBundleSupport]
	public class TabBook : AbstractGroup, Helpers.IWidgetCollectionHost
	{
		public TabBook()
		{
			this.type = TabBookStyle.Right;
			this.items = new TabPageCollection(this);
			
			this.InternalState &= ~InternalState.PossibleContainer;

			this.arrowLeft = new ArrowButton(this);
			this.arrowRight = new ArrowButton(this);
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
		}
		
		public TabBook(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		#region Interface IBundleSupport
		public override void RestoreFromBundle(Epsitec.Common.Support.ObjectBundler bundler, Epsitec.Common.Support.ResourceBundle bundle)
		{
			base.RestoreFromBundle (bundler, bundle);
			
			Support.ResourceBundle.FieldList item_list = bundle["items"].AsList;
			
			if (item_list != null)
			{
				//	Notre bundle contient une liste de sous-bundles contenant les descriptions des
				//	items composant le menu.
				
				foreach (Support.ResourceBundle.Field field in item_list)
				{
					Support.ResourceBundle item_bundle = field.AsBundle;
					TabPage                item_widget = bundler.CreateFromBundle (item_bundle) as TabPage;
					
					this.Items.Add (item_widget);
				}
			}
		}
		#endregion
		
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
		
		public TabPageCollection Items
		{
			get
			{
				return this.items;
			}
		}

		public Direction Direction
		{
			get
			{
				return this.direction;
			}
		}

		public TabBookStyle TabBookStyle
		{
			get
			{
				return this.type;
			}
			set
			{
				this.type = value;
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
				return this.items.Count;
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
				rect.Left  = this.TabOffsetMin;
				if ( rect.Left == 2 )  rect.Left = 0;
				rect.Right = this.TabOffsetMax;
				return rect;
			}
		}
		
		public double TabOffsetMin
		{
			get
			{
				double min = 2;
				if ( this.scrollArrow )
				{
					if ( this.type == TabBookStyle.LeftRight )
					{
						min = this.tabHeight-2;
					}
				}
				return min;
			}
		}
		
		public double TabOffsetMax
		{
			get
			{
				double max = this.Client.Width-2;
				if ( this.scrollArrow )
				{
					if ( this.type == TabBookStyle.LeftRight )
					{
						max = this.Client.Width-(this.tabHeight-2);
					}
					if ( this.type == TabBookStyle.Right )
					{
						max = this.Client.Width-(this.tabHeight*2-6);
					}
				}
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


		// Gestion d'un événement lorsqu'un bouton d'onglet est pressé.
		private void HandleTabButton(object sender, MessageEventArgs e)
		{
			if ( !(sender is TabButton) )  return;
			TabButton button = sender as TabButton;

			foreach ( TabPage page in this.items )
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

			move = System.Math.Floor(move);
			this.scrollOffset += move;
			this.UpdateOffset();
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

			this.UpdateOffset();
			this.UpdateButtons();
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

			this.scrollOffset = System.Math.Min(this.scrollOffset, this.scrollTotalWidth-this.TabOffsetMax);
			this.scrollOffset = System.Math.Max(this.scrollOffset, -this.TabOffsetMin);
		}
		
		protected void UpdateDirection(Direction dir)
		{
			this.direction = dir;
			this.Invalidate();
		}
		
		// Met à jour la page visible. Toutes les autres sont cachées.
		protected void UpdateVisiblePages()
		{
			foreach ( TabPage page in this.items )
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
			foreach ( TabPage page in this.items )
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
		
		// Met à jour les boutons, en faisant disparaître les flèches si nécessaire.
		protected void UpdateButtons()
		{
			this.UpdateTabButtons();
			this.UpdateArrowButtons();
		}

		// Met à jour tous les boutons des onglets.
		protected void UpdateTabButtons()
		{
			if ( this.items == null )  return;

			Drawing.Rectangle rect = new Drawing.Rectangle(0, 0, this.Client.Width, this.Client.Height);
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
		}
		
		// Met à jour les 2 boutons flèche < > pour scroller.
		protected void UpdateArrowButtons()
		{
			this.scrollArrow = ( this.scrollTotalWidth > this.Client.Width-4 );

			if ( this.arrowLeft == null || this.arrowRight == null )  return;

			if ( this.scrollArrow )
			{
				if ( this.type == TabBookStyle.LeftRight )
				{
					Drawing.Rectangle rect;
					rect = new Drawing.Rectangle(0, this.Client.Height-this.tabHeight, this.tabHeight, this.tabHeight);
					rect.Inflate(-2, -2);
					rect.Offset(-2, 0);
					this.arrowLeft.Bounds = rect;
					this.arrowLeft.SetVisible(true);
					this.arrowLeft.SetEnabled(this.scrollOffset > this.TabOffsetMin);

					rect = new Drawing.Rectangle(this.Client.Width-this.tabHeight, this.Client.Height-this.tabHeight, this.tabHeight, this.tabHeight);
					rect.Inflate(-2, -2);
					rect.Offset(2, 0);
					this.arrowRight.Bounds = rect;
					this.arrowRight.SetVisible(true);
					this.arrowRight.SetEnabled(this.scrollOffset < this.scrollTotalWidth-this.TabOffsetMax);
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
			if ( this.isGrimy )
			{
				this.Children.Remove(this.arrowLeft);
				this.Children.Remove(this.arrowRight);
				this.Children.Add(this.arrowLeft);
				this.Children.Add(this.arrowRight);
				this.isGrimy = false;
			}
		}


		protected virtual void HandlePageRankChanged(object sender, System.EventArgs e)
		{
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

			Drawing.Rectangle part = new Drawing.Rectangle();

			part = rect;
			part.Bottom = part.Top-this.TabHeight;
			//adorner.PaintTabBand(graphics, part, state, Direction.Down);
			
			part = rect;
			part.Top -= this.TabHeight;
			adorner.PaintTabFrame(graphics, part, state, Direction.Down);
		}
		

		#region IWidgetCollectionHost Members
		public void NotifyInsertion(Widget widget)
		{
			TabPage item = widget as TabPage;

			TabBook oldBook = item.Book;
			if ( oldBook != null )
			{
				oldBook.items.Remove(item);
			}
			
			item.Bounds = this.Inside;
			item.Anchor = AnchorStyles.LeftAndRight | AnchorStyles.TopAndBottom;
			
			this.Children.Add(item);
			this.Children.Add(item.TabButton);  // TabButton fils de TabBook !
			item.TabButton.Pressed += new MessageEventHandler(this.HandleTabButton);
			item.RankChanged += new System.EventHandler(this.HandlePageRankChanged);
			this.isGrimy = true;
			
			this.UpdateVisiblePages();
			this.UpdateButtons();
		}

		public void NotifyRemoval(Widget widget)
		{
			TabPage item = widget as TabPage;

			if ( this.activePage == item )
			{
				if ( index >= this.items.Count )
				{
					index --;
				}
				
				if ( index > 0 )
				{
					this.activePage = this.items[index] as TabPage;
				}
				else
				{
					this.activePage = null;
				}
			}

			item.TabButton.Clicked -= new MessageEventHandler(this.HandleTabButton);
			item.RankChanged -= new System.EventHandler(this.HandlePageRankChanged);

			this.Children.Remove(item);
			this.isGrimy = true;
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


		protected TabBookStyle					type;
		protected TabPageCollection				items;
		protected TabPage						activePage;
		protected Direction						direction;
		protected double						tabHeight = 20;
		protected bool							scrollArrow = false;
		protected ArrowButton					arrowLeft;
		protected ArrowButton					arrowRight;
		protected double						scrollTotalWidth;
		protected double						scrollOffset = 0;
		protected bool							isGrimy;
	}
}
