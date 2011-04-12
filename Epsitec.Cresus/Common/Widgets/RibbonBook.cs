using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// Summary description for RibbonBook.
	/// </summary>
	public class RibbonBook : AbstractGroup, Collections.IWidgetCollectionHost<RibbonPage>, Collections.IWidgetCollectionHost<Button>
	{
		public RibbonBook()
		{
			this.pageItems = new RibbonPageCollection(this);
			this.buttonItems = new ButtonCollection (this);
			
			this.InternalState &= ~WidgetInternalState.PossibleContainer;

			this.TabNavigationMode = TabNavigationMode.ForwardTabActive;

			//	Partie inférieure, qui contiendra les pages.
			this.pages = new Widget(this);
			this.pages.Dock = DockStyle.Fill;

			//	Partie supérieure, qui contiendra les boutons et les commandes rapides.
			//	Créé en deuxième, pour dessiner les boutons par-dessus les pages !
			this.buttons = new Widget(this);
			this.buttons.Margins = new Margins(0, 0, 0, -1);  // -1 -> un pixel de chevauchement avec this.pages
			this.buttons.Padding = new Margins(12, 0, RibbonBook.TopMargin, 0);
			this.buttons.Dock = DockStyle.Top;
		}
		
		public RibbonBook(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}


		static RibbonBook()
		{
			Types.DependencyPropertyMetadata metadataDy = Visual.PreferredHeightProperty.DefaultMetadata.Clone ();

			metadataDy.DefineDefaultValue (RibbonBook.TabHeight);
			
			Visual.MinHeightProperty.OverrideMetadata (typeof (RibbonBook), metadataDy);
		}


		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				RibbonPage[] pages = this.Pages.ToArray ();
				Button[] buttons = this.Buttons.ToArray ();
				
				foreach (RibbonPage page in pages)
				{
					page.Dispose();
				}
				foreach (Button button in buttons)
				{
					button.Dispose ();
				}
				
				this.pageItems = null;
				this.buttonItems = null;
			}
			
			base.Dispose(disposing);
		}


		public ButtonCollection				Buttons
		{
			get
			{
				return this.buttonItems;
			}
		}
		
		public RibbonPageCollection			Pages
		{
			get
			{
				return this.pageItems;
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
				RibbonPage page = ((value >= 0) && (value < this.PageCount)) ? this.Pages[value] : null;
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
				return this.pageItems.Count;
			}
		}
		
		
		public void Clear()
		{
			this.pageItems.Clear();
		}


		public RibbonPage FindPage(int index)
		{
			return this.pageItems[index] as RibbonPage;
		}

		public int FindPage(RibbonPage page)
		{
			return this.pageItems.IndexOf(page);
		}


		private void HandleRibbonButton(object sender, MessageEventArgs e)
		{
			//	Gestion d'un événement lorsqu'un bouton d'onglet est pressé.
			if ( !(sender is RibbonButton) )  return;
			RibbonButton button = sender as RibbonButton;

			foreach (Widget widget in this.pageItems)
			{
				RibbonPage page = widget as RibbonPage;
				if (page == null)  continue;

				if (page.RibbonButton == button)
				{
					if (this.ActivePage == page)
					{
						this.ActivePage = null;
					}
					else
					{
						this.ActivePage = page;

						if (button.AutoFocus)
						{
							this.Focus();
						}
					}
					
					break;
				}
			}
		}

		
		protected void UpdateVisiblePages()
		{
			//	Met à jour la page visible. Toutes les autres sont cachées.
			foreach (Widget widget in this.pageItems)
			{
				RibbonPage page = widget as RibbonPage;
				if (page == null)  continue;

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
			var handler = this.GetUserEventHandler("PageCountChanged");
			if (handler != null)
			{
				handler(this);
			}
		}
		
		protected virtual void OnActivePageChanged()
		{
			var handler = this.GetUserEventHandler("ActivePageChanged");
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
			rect.Bottom = rect.Top-RibbonBook.TabHeight;
			adorner.PaintRibbonTabBackground (graphics, rect, this.GetPaintState ());
		}
		
		protected override void ProcessMessage(Message message, Point pos)
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


		#region IWidgetCollectionHost<RibbonPage> Members

		void Collections.IWidgetCollectionHost<RibbonPage>.NotifyInsertion(RibbonPage item)
		{
			//	Si la page à insérer est dans un autre book, on l'y enlève.
			RibbonBook oldBook = item.Book;
			if (oldBook != null && oldBook != this)
			{
				oldBook.pageItems.Remove(item);
			}

			item.SetEmbedder(this.pages);
			item.Dock = DockStyle.Fill;

			item.RibbonButton.SetEmbedder(this.buttons);
			item.RibbonButton.Dock = DockStyle.Left;
			item.RibbonButton.Pressed += this.HandleRibbonButton;
			item.RankChanged += this.HandlePageRankChanged;

			if (this.buttonItems.Count > 0)
			{
				//	Make sure that all button items are inserted after the last
				//	ribbon header button :

				foreach (Button button in this.buttonItems)
				{
					button.SetEmbedder (null);
					button.SetEmbedder (this.buttons);
				}
			}

			this.UpdateVisiblePages();
			this.OnPageCountChanged();
		}

		void Collections.IWidgetCollectionHost<RibbonPage>.NotifyRemoval(RibbonPage item)
		{
			int index = item.Index;

			item.RibbonButton.Pressed -= this.HandleRibbonButton;
			item.RankChanged -= this.HandlePageRankChanged;

			this.pages.Children.Remove(item);
			this.buttons.Children.Remove(item.RibbonButton);

			if (this.ActivePage == item)
			{
				int n = this.PageCount - 1;

				if (index >= n)
				{
					index = n - 1;
				}

				this.ActivePageIndex = index;
			}
		}

		void Collections.IWidgetCollectionHost<RibbonPage>.NotifyPostRemoval(RibbonPage item)
		{
			this.UpdateVisiblePages ();
			this.OnPageCountChanged ();
		}

		Collections.WidgetCollection<RibbonPage> Collections.IWidgetCollectionHost<RibbonPage>.GetWidgetCollection()
		{
			return this.Pages;
		}

		#endregion

		#region IWidgetCollectionHost<Button> Members

		void Collections.IWidgetCollectionHost<Button>.NotifyInsertion(Button widget)
		{
			widget.SetEmbedder (this.buttons);
			widget.Margins = new Margins (0, 0, 2, 2);
			widget.Dock = DockStyle.Left;
		}

		void Collections.IWidgetCollectionHost<Button>.NotifyRemoval(Button widget)
		{
			this.buttons.Children.Remove (widget);
		}

		void Collections.IWidgetCollectionHost<Button>.NotifyPostRemoval(Button widget)
		{
		}

		Collections.WidgetCollection<Button> Collections.IWidgetCollectionHost<Button>.GetWidgetCollection()
		{
			return this.buttonItems;
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
		public class RibbonPageCollection : Collections.WidgetCollection<RibbonPage>
		{
			public RibbonPageCollection(RibbonBook book) : base(book)
			{
			}

			public override bool AutoEmbedding
			{
				get
				{
					return false;
				}
			}
		}
		#endregion

		#region ButtonCollection Class
		public class ButtonCollection : Collections.WidgetCollection<Button>
		{
			public ButtonCollection(RibbonBook book)
				: base (book)
			{
			}

			public override bool AutoEmbedding
			{
				get
				{
					return false;
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


		protected static readonly double	TopMargin = 3;
		protected static readonly double	TabHeight = RibbonBook.TopMargin + RibbonButton.DefaultHeight - 1;

		protected Widget					buttons;
		protected Widget					pages;
		protected RibbonPageCollection		pageItems;
		protected ButtonCollection			buttonItems;
		protected RibbonPage				activePage;
	}
}
