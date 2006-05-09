using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// Summary description for RibbonBook.
	/// </summary>
	public class RibbonBook : AbstractGroup, Collections.IWidgetCollectionHost
	{
		public RibbonBook()
		{
			this.items = new RibbonPageCollection(this);
			this.items.AutoEmbedding = true;
			
			this.InternalState &= ~InternalState.PossibleContainer;

			this.TabNavigation = Widget.TabNavigationMode.ForwardTabActive;

			this.buttons = new Widget(this);
			this.buttons.Dock = DockStyle.Top;

			this.pages = new Widget(this);
			this.pages.Dock = DockStyle.Fill;
		}
		
		public RibbonBook(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				RibbonPage[] pages = new RibbonPage[this.items.Count];
				this.items.CopyTo(pages, 0);
				
				for (int i=0; i<pages.Length; i++)
				{
					pages[i].Dispose();
				}
				
				this.items = null;
			}
			
			base.Dispose(disposing);
		}

		
		public override Margins GetInternalPadding()
		{
			return new Margins(2, 2, RibbonBook.TabHeight+2, 2);
		}


		public RibbonPageCollection			Items
		{
			get
			{
				return this.items;
			}
		}

		public int							ActivePageIndex
		{
			get
			{
				RibbonPage page = this.ActivePage;
				return (page == null) ? -1 : page.Index;
			}
			set
			{
				RibbonPage page = ((value >= 0) && (value < this.PageCount)) ? this.Items[value] : null;
				this.ActivePage = page;
			}
		}

		public RibbonPage					ActivePage
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
					this.OnActivePageChanged();
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
		
		
		public void Clear()
		{
			this.items.Clear();
		}


		public RibbonPage FindPage(int index)
		{
			return this.items[index] as RibbonPage;
		}

		public int FindPage(RibbonPage page)
		{
			return this.items.IndexOf(page);
		}


		private void HandleRibbonButton(object sender, MessageEventArgs e)
		{
			//	Gestion d'un événement lorsqu'un bouton d'onglet est pressé.
			if ( !(sender is RibbonButton) )  return;
			RibbonButton button = sender as RibbonButton;

			foreach (RibbonPage page in this.items)
			{
				if (page.RibbonButton == button)
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

		
		protected void UpdateVisiblePages()
		{
			//	Met à jour la page visible. Toutes les autres sont cachées.
			foreach (RibbonPage page in this.items)
			{
				if ( page == this.ActivePage )  // est-ce la page active ?
				{
					page.Visibility = true;
					page.RibbonButton.ActiveState = ActiveState.Yes;
					page.RibbonButton.InheritsParentFocus = true;
				}
				else
				{
					page.Visibility = false;
					page.RibbonButton.ActiveState = ActiveState.No;
					page.RibbonButton.InheritsParentFocus = false;
				}
			}
		}


		protected virtual void HandlePageRankChanged(object sender)
		{
		}
		
		
		protected virtual void OnPageCountChanged()
		{
			EventHandler handler = (EventHandler) this.GetUserEventHandler("PageCountChanged");
			if (handler != null)
			{
				handler(this);
			}
		}
		
		protected virtual void OnActivePageChanged()
		{
			EventHandler handler = (EventHandler) this.GetUserEventHandler("ActivePageChanged");
			if (handler != null)
			{
				handler(this);
			}
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			//	Dessine le groupe d'onglets.
			IAdorner adorner = Widgets.Adorners.Factory.Active;

			Rectangle rect = this.Client.Bounds;
			WidgetPaintState state = this.PaintState;

			Rectangle part = new Rectangle();

			part = rect;
			part.Bottom = part.Top-RibbonBook.TabHeight;
			//adorner.PaintTabBand(graphics, part, state, Direction.Down);
			
			part = rect;
			part.Top -= RibbonBook.TabHeight;
			adorner.PaintTabFrame(graphics, part, state, Direction.Down);
		}
		
		protected override void ProcessMessage(Message message, Point pos)
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
						index = System.Math.Max(index, 0);
						index = System.Math.Min(index, this.PageCount - 1);
					}
					
					if (this.ActivePageIndex != index)
					{
						this.ActivePageIndex = index;
						
						if (! this.IsFocused)
						{
							//	On n'a pas le focus clavier, il faut donc activer le focus de la
							//	nouvelle page activée.
							
							this.ActivePage.SetFocusOnTabWidget();
						}
					}
					
					message.Consumer  = this;
					message.Swallowed = true;
					return;
				}
			}
			
			base.ProcessMessage(message, pos);
		}


		public override Margins GetShapeMargins()
		{
			return Widgets.Adorners.Factory.Active.GeometryListShapeMargins;
		}

		
		#region IWidgetCollectionHost Members
		Collections.WidgetCollection Collections.IWidgetCollectionHost.GetWidgetCollection()
		{
			return this.Items;
		}
		
		public void NotifyInsertion(Widget widget)
		{
			RibbonPage item = widget as RibbonPage;
			RibbonBook oldBook = item.Book;
			
			if (oldBook != null && oldBook != this)
			{
				oldBook.items.Remove(item);
			}

			item.SetEmbedder(this.pages);
			item.Dock = DockStyle.Fill;
			
			System.Diagnostics.Debug.Assert(oldBook == this);

			item.RibbonButton.SetEmbedder(this.buttons);
			item.RibbonButton.Dock = DockStyle.Left;
			item.RibbonButton.Pressed += new MessageEventHandler(this.HandleRibbonButton);
			item.RankChanged += new EventHandler(this.HandlePageRankChanged);
			
			this.UpdateVisiblePages();
			this.OnPageCountChanged();
		}

		public void NotifyRemoval(Widget widget)
		{
			RibbonPage item = widget as RibbonPage;
			int index = item.Index;

			item.RibbonButton.Clicked -= new MessageEventHandler(this.HandleRibbonButton);
			item.RankChanged -= new EventHandler(this.HandlePageRankChanged);
			
			this.Children.Remove(item);
			this.Children.Remove(item.RibbonButton);
			
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
			this.OnPageCountChanged();
		}
		#endregion

		#region RibbonComparer class
		class RibbonComparer : System.Collections.IComparer
		{
			public int Compare(object x, object y)
			{
				RibbonPage page1 = x as RibbonPage;
				RibbonPage page2 = y as RibbonPage;
				
				return page1.Rank.CompareTo(page2.Rank);
			}
		}
		#endregion

		#region RibbonPageCollection Class
		public class RibbonPageCollection : Collections.WidgetCollection
		{
			public RibbonPageCollection(RibbonBook book) : base(book)
			{
				this.AutoEmbedding = false;
			}

			public new RibbonPage this[int index]
			{
				get
				{
					return base[index] as RibbonPage;
				}
			}

			public new RibbonPage this[string name]
			{
				get
				{
					return base[name] as RibbonPage;
				}
			}
		}
		#endregion
		
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

		public event EventHandler			ActivePageChanged
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


		protected static readonly double	TabHeight = 28;

		protected Widget					buttons;
		protected Widget					pages;
		private RibbonPageCollection		items;
		private RibbonPage					activePage;
	}
}
