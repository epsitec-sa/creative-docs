using Epsitec.Common.Support;

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
	public class PaneBook : AbstractGroup, Collections.IWidgetCollectionHost<PanePage>
	{
		public PaneBook()
		{
			this.items = new PanePageCollection(this);
			
			this.InternalState &= ~WidgetInternalState.PossibleContainer;
		}
		
		public PaneBook(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.Clear();
			}
			
			base.Dispose(disposing);
		}


		public PaneBookBehaviour PaneBehaviour
		{
			//	Comportement lorsque la frontière est déplacée.
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


		protected override void SetBoundsOverride(Drawing.Rectangle oldRect, Drawing.Rectangle newRect)
		{
			base.SetBoundsOverride(oldRect, newRect);
			this.UpdateGeometry ();
		}
		
		protected void UpdateGeometry()
		{
			if ( this.items == null )  return;
			this.UpdateGeometryPages();
			this.UpdatePaneButtons();
			this.lastWindowSize = this.Client.Size;
			this.IsDirty = false;
		}

		public void Update()
		{
			if ( !this.IsDirty )  return;

			this.UpdateGeometryPages();
			this.UpdatePaneButtons();
			this.IsDirty = false;
		}
		
		protected void UpdateGeometryPages()
		{
			//	Adapte les panneaux après un changement de géométrie.
			if ( this.items == null )  return;

			this.windowSize = this.RetWindowSize();
			this.totalRelativeSize = this.RetTotalRelativeSize();

			while ( this.AbsoluteUpdate() );
			this.StretchPages();
			this.CheckMinMax();
			this.UpdateAbsoluteSizes();
		}

		protected bool AbsoluteUpdate()
		{
			//	Assigne les tailles absolues.
			int count = this.items.Count;
			for ( int i=0 ; i<count ; i++ )
			{
				PanePage page = this.items[i];
				double abs = page.AbsoluteOrder;
				if ( System.Double.IsNaN(abs) )  continue;

				double delta = page.PaneRelativeSize;
				this.SetSizeBase(i, abs);
				delta -= page.PaneRelativeSize;

				for ( int j=0 ; j<count ; j++ )
				{
					if ( j == i )  continue;
					page = this.items[j];
					double dim = page.PaneRelativeSize;
					dim += delta;
					if ( dim < 0 )  dim = 0;
					delta -= System.Math.Abs(page.PaneRelativeSize-dim);
					page.PaneRelativeSize = dim;
				}
				return true;
			}
			return false;
		}

		protected void StretchPages()
		{
			//	Stretch les panneaux selon leurs élasticités.
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
			this.totalRelativeSize = this.RetTotalRelativeSize();
		}

		protected void CheckMinMax()
		{
			//	Vérifie si un ou plusieurs panneaux sont en-dessous de la taille
			//	minimale, ou en dessus de la taille maximale.
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

		protected void UpdatePaneButtons()
		{
			//	Met à jour la géométrie de tous les panneaux et boutons.
			if ( this.items == null )  return;

			this.windowSize = this.RetWindowSize();
			this.totalRelativeSize = this.RetTotalRelativeSize();

			Drawing.Rectangle rect = this.Client.Bounds;
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
					page.SetManualBounds(rect);
					page.Visibility = (rect.Width >= page.PaneHideSize);

					rect.Left = end.X-this.sliderDim;
					rect.Width = this.sliderDim;
					this.Align(ref rect);
					page.PaneButton.SetManualBounds(rect);

					if ( page.PaneToggle )
					{
						page.GlyphButton.Show();

						Drawing.Rectangle arect = rect;
						arect.Left  -= 3;
						arect.Right += 3;
						arect.Top   = arect.Bottom+12;
						page.GlyphButton.SetManualBounds(arect);

						if ( this.RetSize(i) < (this.RetMinSize(i)+this.RetMaxSize(i))/2 )
						{
							page.GlyphButton.GlyphShape = GlyphShape.ArrowRight;
						}
						else
						{
							page.GlyphButton.GlyphShape = GlyphShape.ArrowLeft;
						}
					}
					else
					{
						page.GlyphButton.Hide();
					}

					start.X = end.X;
				}
				else
				{
					end.Y = start.Y-this.RetSize(i)-this.sliderDim;

					rect.Bottom = end.Y+this.sliderDim;
					rect.Height = start.Y-end.Y-this.sliderDim;
					this.Align(ref rect);
					page.SetManualBounds(rect);

					rect.Bottom = end.Y;
					rect.Height = this.sliderDim;
					this.Align(ref rect);
					page.PaneButton.SetManualBounds(rect);

					if ( page.PaneToggle )
					{
						page.GlyphButton.Show();

						Drawing.Rectangle arect = rect;
						arect.Left   = arect.Right-12;
						arect.Bottom -= 3;
						arect.Top    += 3;
						page.GlyphButton.SetManualBounds(arect);

						if ( this.RetSize(i) < (this.RetMinSize(i)+this.RetMaxSize(i))/2 )
						{
							page.GlyphButton.GlyphShape = GlyphShape.ArrowDown;
						}
						else
						{
							page.GlyphButton.GlyphShape = GlyphShape.ArrowUp;
						}
					}
					else
					{
						page.GlyphButton.Hide();
					}

					start.Y = end.Y;
				}
			}

			this.OnPaneSizeChanged();
		}

		protected void UpdateAbsoluteSizes()
		{
			//	Met à jour les tailles absolues en fonction des tailles relatives.
			int count = this.items.Count;
			for ( int i=0 ; i<count ; i++ )
			{
				PanePage page = this.items[i];
				double size = this.RetSize(i);
				page.AbsoluteSize = size;
			}
			this.Invalidate ();
		}


		protected void HandleSliderDragStarted(object sender, MessageEventArgs e)
		{
			//	Appelé lorsque le slider va être déplacé.
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
					this.alphaBar.SetManualBounds(button.ActualBounds);
					this.alphaBar.SetParent (this);
					this.sliderDragRect = button.ActualBounds;

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

		protected void HandleSliderDragMoved(object sender, MessageEventArgs e)
		{
			//	Appelé lorsque le slider est déplacé.
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
						this.SetSizeMinMax(index, this.sliderDragDim);
					}
					else
					{

						this.sliderDragDim -= pos.Y - this.sliderDragPos;
						this.sliderDragPos  = pos.Y;
						this.SetSizeMinMax(index, this.sliderDragDim);
					}
					this.UpdatePaneButtons();
					break;
			}
		}

		protected void HandleSliderDragEnded(object sender, MessageEventArgs e)
		{
			//	Appelé lorsque le slider est fini de déplacer.
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
						this.SetSizeMinMax(index, this.sliderDragDim);
					}
					else
					{
						this.sliderDragDim -= pos.Y - this.sliderDragPos;
						this.sliderDragPos  = pos.Y;
						this.SetSizeMinMax(index, this.sliderDragDim);
					}

					this.Children.Remove(this.alphaBar);
					this.alphaBar = null;

					this.UpdatePaneButtons();
					break;

				case PaneBookBehaviour.FollowMe:
					break;
			}

			this.UpdateAbsoluteSizes();
		}

		private void HandleGlyphButtonClicked(object sender, MessageEventArgs e)
		{
			//	Bouton flèche cliqué.
			if ( !(sender is GlyphButton) )  return;
			GlyphButton button = sender as GlyphButton;
			int index = this.SearchPage(button);
			if ( index == -1 )  return;
			PanePage page = this.items[index];

			if ( this.RetSize(index) < (this.RetMinSize(index)+this.RetMaxSize(index))/2 )
			{
				this.SetSize(index, this.RetMaxSize(index));
			}
			else
			{
				this.SetSize(index, this.RetMinSize(index));
			}
			this.UpdatePaneButtons();
			this.UpdateAbsoluteSizes();
		}

		protected void SetSizeAlpha(int index, double size)
		{
			//	Déplace le rectangle provisoire.
			size = System.Math.Max(size, this.sliderDragMin);
			size = System.Math.Min(size, this.sliderDragMax);

			Drawing.Rectangle rect = this.alphaBar.ActualBounds;

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
			this.alphaBar.SetManualBounds(rect);
		}

		protected void Align(ref Drawing.Rectangle rect)
		{
			rect.Left   = System.Math.Floor(rect.Left+0.5);
			rect.Right  = System.Math.Floor(rect.Right+0.5);
			rect.Bottom = System.Math.Floor(rect.Bottom+0.5);
			rect.Top    = System.Math.Floor(rect.Top+0.5);
		}


		protected double RetSize(int index)
		{
			//	Retourne la taille actuelle.
			PanePage page = this.items[index];
			return page.PaneRelativeSize/this.totalRelativeSize*this.windowSize;
		}

		protected void SetSizeMinMax(int index, double size)
		{
			//	Modifie la taille.
			size = System.Math.Max(size, this.sliderDragMin);
			size = System.Math.Min(size, this.sliderDragMax);
			this.SetSize(index, size);
		}

		protected void SetSize(int index, double size)
		{
			//	Modifie la taille.
			PanePage page = this.items[index];

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

		protected void SetSizeBase(int index, double size)
		{
			//	Modifie la taille.
			PanePage page = this.items[index];
			size = System.Math.Max(size, page.PaneMinSize);
			size = System.Math.Min(size, page.PaneMaxSize);
			page.PaneRelativeSize = size*this.totalRelativeSize/this.windowSize;
		}

		protected double RetMinSize(int index)
		{
			//	Retourne la taille minimale possible.
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

		protected double RetMaxSize(int index)
		{
			//	Retourne la taille maximale possible.
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

		protected int SearchPage(PaneButton button)
		{
			//	Cherche l'index de la page correspondant à un bouton.
			int count = this.items.Count;
			for ( int i=0 ; i<count ; i++ )
			{
				PanePage page = this.items[i];
				if ( page.PaneButton == button )  return i;
			}
			return -1;
		}

		protected int SearchPage(GlyphButton button)
		{
			//	Cherche l'index de la page correspondant à un bouton.
			int count = this.items.Count;
			for ( int i=0 ; i<count ; i++ )
			{
				PanePage page = this.items[i];
				if ( page.GlyphButton == button )  return i;
			}
			return -1;
		}


		protected double RetWindowSize()
		{
			//	Retourne la largeur ou la hauteur maximale exploitable.
			double total;
			if ( this.type == PaneBookStyle.LeftRight )
			{
				total = this.Client.Size.Width;
			}
			else
			{
				total = this.Client.Size.Height;
			}
			if ( this.items != null )
			{
				total -= this.sliderDim*(this.items.Count-1);
			}
			return total;
		}

		protected double RetTotalRelativeSize()
		{
			//	Retourne la somme de toutes les largeurs relatives.
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


		protected virtual void HandlePageRankChanged(object sender)
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
		
		protected virtual void OnPaneSizeChanged()
		{
			//	Génère un événement pour dire qu'une taille a changé.
			EventHandler handler = (EventHandler) this.GetUserEventHandler("PaneSizeChanged");
			if (handler != null)
			{
				handler(this);
			}
		}


		public event EventHandler			PaneSizeChanged
		{
			add
			{
				this.AddUserEventHandler("PaneSizeChanged", value);
			}
			remove
			{
				this.RemoveUserEventHandler("PaneSizeChanged", value);
			}
		}


		
		//	Dessine le groupe de panneaux.
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			IAdorner adorner = Widgets.Adorners.Factory.Active;

			this.Update();
		}


		public bool IsDirty
		{
			//	Indique si le PaneBook doit être recalculé.
			get
			{
				if ( this.items == null )  return false;
				foreach ( PanePage page in this.items )
				{
					if ( page.IsDirty )  return true;
				}
				return false;
			}

			set
			{
				if ( this.items == null )  return;
				foreach ( PanePage page in this.items )
				{
					page.IsDirty = value;
				}
			}
		}


		#region IWidgetCollectionHost Members
		Collections.WidgetCollection<PanePage> Collections.IWidgetCollectionHost<PanePage>.GetWidgetCollection()
		{
			return this.Items;
		}
		
		public void NotifyInsertion(PanePage item)
		{
			PaneBook oldBook = item.Book;
			
			if ((oldBook != null) &&
				(oldBook != this))
			{
				oldBook.items.Remove(item);
			}
			
			System.Diagnostics.Debug.Assert (oldBook == this);

			Drawing.Rectangle rect = this.Client.Bounds;
			rect.Deflate (this.GetInternalPadding ());

			item.SetManualBounds(rect);
			item.PaneButton.PaneButtonStyle = ( this.type == PaneBookStyle.LeftRight ) ? PaneButtonStyle.Vertical : PaneButtonStyle.Horizontal;
			
			item.PaneButton.SetParent (this);
			item.PaneButton.DragStarted += this.HandleSliderDragStarted;
			item.PaneButton.DragMoved   += this.HandleSliderDragMoved;
			item.PaneButton.DragEnded   += this.HandleSliderDragEnded;
			item.RankChanged += this.HandlePageRankChanged;
			
			item.GlyphButton.SetParent (this);
			item.GlyphButton.Clicked += this.HandleGlyphButtonClicked;

			this.UpdatePaneButtons();
		}

		public void NotifyRemoval(PanePage item)
		{
			item.PaneButton.DragStarted -= this.HandleSliderDragStarted;
			item.PaneButton.DragMoved   -= this.HandleSliderDragMoved;
			item.PaneButton.DragEnded   -= this.HandleSliderDragEnded;
			item.RankChanged -= this.HandlePageRankChanged;

			item.GlyphButton.Clicked -= this.HandleGlyphButtonClicked;

			this.Children.Remove(item);

			this.UpdatePaneButtons();
		}
		
		public void NotifyPostRemoval(PanePage item)
		{
		}
		#endregion

		#region PanePageCollection Class
		public class PanePageCollection : Collections.WidgetCollection<PanePage>
		{
			public PanePageCollection(PaneBook book) : base(book)
			{
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
