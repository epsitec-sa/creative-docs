namespace Epsitec.Common.Widgets
{
	public enum PaneBookStyle
	{
		LeftRight,			// panneaux côte à côte
		BottomTop,			// panneaux l'un en dessus de l'autre
	}

	public enum PaneBookBehaviour
	{
		Draft,				// déplace lorsque le bouton est relâché
		FollowMe,			// suit la souris
	}

	/// <summary>
	/// Summary description for PaneBook.
	/// </summary>
	[Support.SuppressBundleSupport]
	public class PaneBook : AbstractGroup, Helpers.IWidgetCollectionHost
	{
		public PaneBook()
		{
			this.items = new PanePageCollection(this);
		}
		
		public PaneBook(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		#region Interface IBundleSupport
		public override void RestoreFromBundle(Epsitec.Common.Support.ObjectBundler bundler, Epsitec.Common.Support.ResourceBundle bundle)
		{
			base.RestoreFromBundle (bundler, bundle);
			
			System.Collections.IList item_list = bundle.GetFieldBundleList("items");
			
			if ( item_list != null )
			{
				//	Notre bundle contient une liste de sous-bundles contenant les descriptions des
				//	items composant le menu.
				foreach ( Support.ResourceBundle item_bundle in item_list )
				{
					PanePage item = bundler.CreateFromBundle(item_bundle) as PanePage;
					this.Items.Add(item);
				}
			}
		}
		#endregion
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.Clear();
			}
			
			base.Dispose(disposing);
		}


		// Comportement lorsque la frontière est déplacée.
		public PaneBookBehaviour PaneBehaviour
		{
			get
			{
				return this.paneBehaviour;
			}

			set
			{
				this.paneBehaviour = value;
			}
		}

		public PanePageCollection Items
		{
			get
			{
				return this.items;
			}
		}

		public PaneBookStyle PaneBookStyle
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
		
		public int PageCount
		{
			get
			{
				return this.items.Count;
			}
		}
		
		public Drawing.Rectangle PaneClipRectangle
		{
			get
			{
				Drawing.Rectangle rect = this.Client.Bounds;
				return rect;
			}
		}
		
		public void Clear()
		{
			this.items.Clear();
		}
		
		
		public PanePage FindPage(int index)
		{
			return this.items[index] as PanePage;
		}
		
		public int FindPage(PanePage page)
		{
			return this.items.IndexOf(page);
		}


		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();
			this.UpdateGeometryPages();
			this.UpdatePaneButtons();

			this.lastWindowSize = this.Client.Size;
		}
		
		// Adapte les panneaux après un changement de géométrie.
		protected void UpdateGeometryPages()
		{
			if ( this.items == null )  return;

			this.windowSize = this.RetWindowSize();
			this.totalRelativeSize = this.RetTotalRelativeSize();

			this.StretchPages();
			this.CheckMinMax();
		}

		// Stretch les panneaux selon leurs elasticités.
		protected void StretchPages()
		{
			if ( this.lastWindowSize.Width == 0 )  return;

			double lastSize = ( this.type == PaneBookStyle.LeftRight ) ? this.lastWindowSize.Width : this.lastWindowSize.Height;
			lastSize -= this.sliderDim*(this.items.Count-1);

			int count = this.items.Count;
			double save = this.windowSize;
			this.windowSize = lastSize;
			double totalElasticity = 0;
			for ( int i=0 ; i<count ; i++ )
			{
				PanePage page = this.items[i];
				totalElasticity += this.RetSize(i)*page.PaneElasticity;
			}
			this.windowSize = save;
			if ( totalElasticity == 0 )  return;

			double factor = (this.windowSize-(lastSize-totalElasticity))/totalElasticity;
			for ( int i=0 ; i<count ; i++ )
			{
				PanePage page = this.items[i];
				if ( page.PaneElasticity > 0 )
				{
					page.PaneRelativeSize *= factor*page.PaneElasticity;
				}
			}
		}

		// Vérifie si un ou plusieurs panneaux sont en-dessous de la taille
		// minimale, ou en dessus de la taille maximale.
		protected void CheckMinMax()
		{
			int count = this.items.Count;
			double minOverflow = 0;
			double maxOverflow = 0;
			for ( int i=0 ; i<count ; i++ )
			{
				PanePage page = this.items[i];

				double inside = page.PaneMinSize-this.RetSize(i);
				if ( inside > 0 )  // panneau trop étroit ?
				{
					minOverflow += inside;
				}

				double outside = this.RetSize(i)-page.PaneMaxSize;
				if ( outside > 0 )  // panneau trop large ?
				{
					maxOverflow += outside;
				}
			}

			if ( minOverflow == 0 && maxOverflow == 0 )  return;  // tout est ok ?

			for ( int i=0 ; i<count ; i++ )
			{
				PanePage page = this.items[i];

				double inside = page.PaneMinSize-this.RetSize(i);
				if ( inside > 0 )  // panneau trop étroit ?
				{
					this.SetSizeBase(i, page.PaneMinSize);
				}
				else
				{
					double size = this.RetSize(i);
					this.SetSizeBase(i, size-minOverflow);
					minOverflow -= size-this.RetSize(i);
				}

				double outside = this.RetSize(i)-page.PaneMaxSize;
				if ( outside > 0 )  // panneau trop large ?
				{
					this.SetSizeBase(i, page.PaneMaxSize);
				}
				else
				{
					double size = this.RetSize(i);
					this.SetSizeBase(i, size+maxOverflow);
					maxOverflow -= this.RetSize(i)-size;
				}
			}
		}

		// Met à jour la géométrie de tous les panneaux et boutons.
		protected void UpdatePaneButtons()
		{
			if ( this.items == null )  return;

			this.windowSize = this.RetWindowSize();
			this.totalRelativeSize = this.RetTotalRelativeSize();

			Drawing.Rectangle rect = new Drawing.Rectangle(0, 0, this.Client.Width, this.Client.Height);
			Drawing.Point start = new Drawing.Point(0, rect.Height);
			Drawing.Point end = new Drawing.Point();

			int count = this.items.Count;
			for ( int i=0 ; i<count ; i++ )
			{
				PanePage page = this.items[i];

				if ( this.type == PaneBookStyle.LeftRight )
				{
					end.X = start.X+this.RetSize(i)+this.sliderDim;

					rect.Left = start.X;
					rect.Width = end.X-start.X-this.sliderDim;
					this.Align(ref rect);
					page.Bounds = rect;

					rect.Left = end.X-this.sliderDim;
					rect.Width = this.sliderDim;
					this.Align(ref rect);
					page.PaneButton.Bounds = rect;

					start.X = end.X;
				}
				else
				{
					end.Y = start.Y-this.RetSize(i)-this.sliderDim;

					rect.Bottom = end.Y+this.sliderDim;
					rect.Height = start.Y-end.Y-this.sliderDim;
					this.Align(ref rect);
					page.Bounds = rect;

					rect.Bottom = end.Y;
					rect.Height = this.sliderDim;
					this.Align(ref rect);
					page.PaneButton.Bounds = rect;

					start.Y = end.Y;
				}
			}
		}

		protected void Align(ref Drawing.Rectangle rect)
		{
			rect.Left   = System.Math.Floor(rect.Left+0.5);
			rect.Right  = System.Math.Floor(rect.Right+0.5);
			rect.Bottom = System.Math.Floor(rect.Bottom+0.5);
			rect.Top    = System.Math.Floor(rect.Top+0.5);
		}


		// Appelé lorsque le slider va être déplacé.
		protected void HandleSliderDragStarted(object sender, MessageEventArgs e)
		{
			if ( !(sender is PaneButton) )  return;
			PaneButton button = sender as PaneButton;
			int index = this.SearchPage(button);
			if ( index == -1 )  return;
			this.totalRelativeSize = this.RetTotalRelativeSize();
			this.sliderDragMin = this.RetMinSize(index);
			this.sliderDragMax = this.RetMaxSize(index);
			Drawing.Point pos = button.MapClientToParent(e.Point);
			
			switch ( this.paneBehaviour )
			{
				case PaneBookBehaviour.Draft:
					this.alphaBar = new AlphaBar();
					this.alphaBar.Bounds = button.Bounds;
					this.alphaBar.Parent = this;
					this.sliderDragRect = button.Bounds;

					if ( this.type == PaneBookStyle.LeftRight )
					{
						this.sliderDragPos = pos.X;
						this.sliderDragDim = this.RetSize(index);
						this.sliderDragRect.Offset(-this.RetSize(index), 0);
					}
					else
					{
						this.sliderDragPos = pos.Y;
						this.sliderDragDim = this.RetSize(index);
						this.sliderDragRect.Offset(0, this.RetSize(index));
					}
					break;

				case PaneBookBehaviour.FollowMe:
					if ( this.type == PaneBookStyle.LeftRight )
					{
						this.sliderDragPos = pos.X;
						this.sliderDragDim = this.RetSize(index);
					}
					else
					{
						this.sliderDragPos = pos.Y;
						this.sliderDragDim = this.RetSize(index);
					}
					break;
			}
		}

		// Appelé lorsque le slider est déplacé.
		protected void HandleSliderDragMoved(object sender, MessageEventArgs e)
		{
			if ( !(sender is PaneButton) )  return;
			PaneButton button = sender as PaneButton;
			int index = this.SearchPage(button);
			if ( index == -1 )  return;
			Drawing.Point pos = button.MapClientToParent(e.Point);
			
			switch ( this.paneBehaviour )
			{
				case PaneBookBehaviour.Draft:
					if ( this.type == PaneBookStyle.LeftRight )
					{
						this.sliderDragDim += pos.X - this.sliderDragPos;
						this.sliderDragPos  = pos.X;
						this.SetSizeAlpha(index, this.sliderDragDim);
					}
					else
					{

						this.sliderDragDim -= pos.Y - this.sliderDragPos;
						this.sliderDragPos  = pos.Y;
						this.SetSizeAlpha(index, this.sliderDragDim);
					}
					break;

				case PaneBookBehaviour.FollowMe:
					if ( this.type == PaneBookStyle.LeftRight )
					{
						this.sliderDragDim += pos.X - this.sliderDragPos;
						this.sliderDragPos  = pos.X;
						this.SetSize(index, this.sliderDragDim);
					}
					else
					{

						this.sliderDragDim -= pos.Y - this.sliderDragPos;
						this.sliderDragPos  = pos.Y;
						this.SetSize(index, this.sliderDragDim);
					}
					break;
			}
			
			this.UpdatePaneButtons();
		}

		// Appelé lorsque le slider est fini de déplacer.
		protected void HandleSliderDragEnded(object sender, MessageEventArgs e)
		{
			if ( !(sender is PaneButton) )  return;
			PaneButton button = sender as PaneButton;
			int index = this.SearchPage(button);
			if ( index == -1 )  return;
			Drawing.Point pos = button.MapClientToParent(e.Point);
			
			switch ( this.paneBehaviour )
			{
				case PaneBookBehaviour.Draft:
					if ( this.type == PaneBookStyle.LeftRight )
					{
						this.sliderDragDim += pos.X - this.sliderDragPos;
						this.sliderDragPos  = pos.X;
						this.SetSize(index, this.sliderDragDim);
					}
					else
					{
						this.sliderDragDim -= pos.Y - this.sliderDragPos;
						this.sliderDragPos  = pos.Y;
						this.SetSize(index, this.sliderDragDim);
					}

					this.Children.Remove(this.alphaBar);
					this.alphaBar = null;
					break;

				case PaneBookBehaviour.FollowMe:
					break;
			}

			this.UpdatePaneButtons();
		}

		// Déplace le rectangle provisoire.
		protected void SetSizeAlpha(int index, double size)
		{
			size = System.Math.Max(size, this.sliderDragMin);
			size = System.Math.Min(size, this.sliderDragMax);

			Drawing.Rectangle rect = this.alphaBar.Bounds;

			if ( this.type == PaneBookStyle.LeftRight )
			{
				rect.Left  = size;
				rect.Width = this.sliderDim;
				rect.Offset(this.sliderDragRect.Left, 0);
			}
			else
			{
				rect.Bottom = -size;
				rect.Height = this.sliderDim;
				rect.Offset(0, this.sliderDragRect.Bottom);
			}
			this.Align(ref rect);
			this.alphaBar.Bounds = rect;
		}


		// Retourne la taille actuelle.
		protected double RetSize(int index)
		{
			PanePage page = this.items[index];
			return page.PaneRelativeSize/this.totalRelativeSize*this.windowSize;
		}

		// Modifie la taille.
		protected void SetSize(int index, double size)
		{
			PanePage page = this.items[index];
			size = System.Math.Max(size, this.sliderDragMin);
			size = System.Math.Min(size, this.sliderDragMax);

			if ( index < this.items.Count-1 )
			{
				PanePage npage = this.items[index+1];
				double move = size-this.RetSize(index);
				this.SetSizeBase(index, size);
				this.SetSizeBase(index+1, this.RetSize(index+1)-move);
			}
			else
			{
				this.SetSizeBase(index, size);
			}
		}

		// Modifie la taille.
		protected void SetSizeBase(int index, double size)
		{
			PanePage page = this.items[index];
			size = System.Math.Max(size, page.PaneMinSize);
			size = System.Math.Min(size, page.PaneMaxSize);
			page.PaneRelativeSize = size*this.totalRelativeSize/this.windowSize;
		}

		// Retourne la taille minimale possible.
		protected double RetMinSize(int index)
		{
			PanePage page = this.items[index];
			double min = page.PaneMinSize;

			if ( index < this.items.Count-1 )
			{
				PanePage npage = this.items[index+1];
				double limit = this.RetSize(index)+this.RetSize(index+1)-npage.PaneMaxSize;
				min = System.Math.Max(min, limit);
			}

			return min;
		}

		// Retourne la taille maximale possible.
		protected double RetMaxSize(int index)
		{
			PanePage page = this.items[index];
			double max = page.PaneMaxSize;

			if ( index < this.items.Count-1 )
			{
				PanePage npage = this.items[index+1];
				double limit = this.RetSize(index)+this.RetSize(index+1)-npage.PaneMinSize;
				max = System.Math.Min(max, limit);
			}

			return max;
		}

		// Cherche l'index de la page correspondant à un bouton.
		protected int SearchPage(PaneButton button)
		{
			int count = this.items.Count;
			for ( int i=0 ; i<count ; i++ )
			{
				PanePage page = this.items[i];
				if ( page.PaneButton == button )  return i;
			}
			return -1;
		}


		// Retourne la largeur ou la hauteur maximale exploitable.
		protected double RetWindowSize()
		{
			double total;
			if ( this.type == PaneBookStyle.LeftRight )
			{
				total = this.Client.Width;
			}
			else
			{
				total = this.Client.Height;
			}
			if ( this.items != null )
			{
				total -= this.sliderDim*(this.items.Count-1);
			}
			return total;
		}

		// Retourne la somme de toutes les largeurs relatives.
		protected double RetTotalRelativeSize()
		{
			if ( this.items == null )  return 1;

			int count = this.items.Count;
			if ( count == 0 )  return 1;
			double total = 0;
			for ( int i=0 ; i<count ; i++ )
			{
				PanePage page = this.items[i];
				total += page.PaneRelativeSize;
			}
			return total;
		}


		protected virtual void HandlePageRankChanged(object sender, System.EventArgs e)
		{
		}
		
		
		class PaneComparer : System.Collections.IComparer
		{
			public int Compare(object x, object y)
			{
				PanePage page1 = x as PanePage;
				PanePage page2 = y as PanePage;
				
				return page1.Rank.CompareTo(page2.Rank);
			}
		}
		
		
		// Dessine le groupe de panneaux.
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			IAdorner adorner = Widgets.Adorner.Factory.Active;
		}
		

		#region IWidgetCollectionHost Members
		public void NotifyInsertion(Widget widget)
		{
			PanePage item = widget as PanePage;

			PaneBook oldBook = item.Book;
			if ( oldBook != null )
			{
				oldBook.items.Remove(item);
			}
			
			item.Bounds = this.Inside;
			item.Anchor = AnchorStyles.LeftAndRight | AnchorStyles.TopAndBottom;
			item.PaneButton.PaneButtonStyle = ( this.type == PaneBookStyle.LeftRight ) ? PaneButtonStyle.Vertical : PaneButtonStyle.Horizontal;
			
			this.Children.Add(item);
			this.Children.Add(item.PaneButton);  // PaneButton fils de PaneBook !
			item.PaneButton.DragStarted += new MessageEventHandler(this.HandleSliderDragStarted);
			item.PaneButton.DragMoved   += new MessageEventHandler(this.HandleSliderDragMoved);
			item.PaneButton.DragEnded   += new MessageEventHandler(this.HandleSliderDragEnded);
			item.RankChanged += new System.EventHandler(this.HandlePageRankChanged);
			
			this.UpdatePaneButtons();
		}

		public void NotifyRemoval(Widget widget)
		{
			PanePage item = widget as PanePage;

			item.PaneButton.DragStarted -= new MessageEventHandler(this.HandleSliderDragStarted);
			item.PaneButton.DragMoved   -= new MessageEventHandler(this.HandleSliderDragMoved);
			item.PaneButton.DragEnded   -= new MessageEventHandler(this.HandleSliderDragEnded);
			item.RankChanged -= new System.EventHandler(this.HandlePageRankChanged);

			this.Children.Remove(item);

			this.UpdatePaneButtons();
		}
		#endregion

		#region PanePageCollection Class
		public class PanePageCollection : Helpers.WidgetCollection
		{
			public PanePageCollection(PaneBook book) : base(book)
			{
			}
			
			public new PanePage this[int index]
			{
				get
				{
					return base[index] as PanePage;
				}
			}
			
			public new PanePage this[string name]
			{
				get
				{
					return base[name] as PanePage;
				}
			}
		}
		#endregion


		protected PaneBookStyle				type = PaneBookStyle.LeftRight;
		protected PaneBookBehaviour			paneBehaviour = PaneBookBehaviour.Draft;
		protected PanePageCollection		items;
		protected Drawing.Size				lastWindowSize = new Drawing.Size(0, 0);
		protected double					windowSize;
		protected double					totalRelativeSize;
		protected double					sliderDim = 5;
		protected double					sliderDragMin;
		protected double					sliderDragMax;
		protected double					sliderDragPos;
		protected double					sliderDragDim;
		protected Drawing.Rectangle			sliderDragRect;
		protected AlphaBar					alphaBar;
	}
}
