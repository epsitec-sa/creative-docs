namespace Epsitec.Common.Widgets
{
	public enum PaneStyle
	{
		LeftRight,			// 2 panneaux c�te � c�te (0=left, 1=right)
		BottomTop,			// 2 panneaux l'un en dessus de l'autre (0=bottom, 1=top)
	}

	public enum PaneBehaviour
	{
		Draft,				// d�place lorsque le bouton est rel�ch�
		FollowMe,			// suit la souris
	}

	/// <summary>
	/// La classe Pane impl�mente un double conteneur avec fronti�re d�pla�able.
	/// </summary>
	public class Pane : AbstractGroup
	{
		public Pane()
		{
			this.panes = new Widget[2];

			this.panes[0] = new Widget(this);
			this.panes[1] = new Widget(this);
			
			this.slider = new PaneButton(this);
			this.slider.PaneButtonStyle = PaneButtonStyle.Vertical;
			this.slider.DragStarted += new MessageEventHandler(this.HandleSliderDragStarted);
			this.slider.DragMoved   += new MessageEventHandler(this.HandleSliderDragMoved);
			this.slider.DragEnded   += new MessageEventHandler(this.HandleSliderDragEnded);
			
			this.button = new ArrowButton(this);
			this.button.Hide();
			this.button.Clicked += new MessageEventHandler(this.HandleButtonClicked);
			
			SetHideSize(0, 20);
			SetHideSize(1, 20);
			SetMinSize(0, 0);
			SetMinSize(1, 0);
			SetMaxSize(0, 1000000);
			SetMaxSize(1, 1000000);
			SetSize(0, this.Client.Width/2);
		}
		
		public Pane(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}

		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.slider.DragStarted -= new MessageEventHandler(this.HandleSliderDragStarted);
				this.slider.DragMoved   -= new MessageEventHandler(this.HandleSliderDragMoved);
				this.slider.DragEnded   -= new MessageEventHandler(this.HandleSliderDragEnded);
				
				this.button.Clicked -= new MessageEventHandler(this.HandleButtonClicked);
			}
			
			base.Dispose(disposing);
		}

		
		// Disposition des panneaux.
		public PaneStyle PaneStyle
		{
			get
			{
				return this.paneStyle;
			}

			set
			{
				this.paneStyle = value;

				if ( this.paneStyle == PaneStyle.LeftRight )
				{
					this.slider.PaneButtonStyle = PaneButtonStyle.Vertical;
				}
				else
				{
					this.slider.PaneButtonStyle = PaneButtonStyle.Horizontal;
				}
			}
		}

		// Comportement lorsque la fronti�re est d�plac�e.
		public PaneBehaviour PaneBehaviour
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

		// Mode avec ou sans bouton.
		// Avec PaneStyle.LeftRight, il faut sp�cifier les min/max du panneau gauche (0).
		// Avec PaneStyle.BottomTop, il faut sp�cifier les min/max du panneau sup�rieur (1).
		public bool FlipFlop
		{
			get
			{
				return this.flipFlop;
			}

			set
			{
				this.flipFlop = value;
			}
		}

		// Donne un panneau.
		public Widget RetPane(int rank)
		{
			if ( rank < 0 || rank >= 2 )
			{
				throw new System.ArgumentOutOfRangeException();
			}
			return this.panes[rank];
		}

		// Sp�cifie une dimension.
		public void SetSize(int rank, double dim)
		{
			if ( rank < 0 || rank >= 2 )
			{
				throw new System.ArgumentOutOfRangeException();
			}

			Drawing.Rectangle rect = this.Client.Bounds;

			dim = System.Math.Max(dim, this.minSize[rank]);
			dim = System.Math.Min(dim, this.maxSize[rank]);

			if ( this.paneStyle == PaneStyle.LeftRight )
			{
				double total = rect.Width;
				dim = System.Math.Min(dim, total-this.sliderDim);
				dim = System.Math.Floor(dim);

				if ( rank == 0 )  // panneau gauche ?
				{
					if ( dim < this.hideSize[0] )  dim = 0;

					rect.Width = dim;
					this.panes[0].Bounds = rect;

					rect.Left = rect.Right;
					rect.Width = this.sliderDim;
					this.slider.Bounds = rect;

					rect.Left = rect.Right;
					rect.Width = total-dim-this.sliderDim;
					this.panes[1].Bounds = rect;
				}

				if ( rank == 1 )  // panneau droite ?
				{
					if ( dim < this.hideSize[1] )  dim = 0;

					rect.Width = total-dim;
					this.panes[0].Bounds = rect;

					rect.Left = rect.Right;
					rect.Width = this.sliderDim;
					this.slider.Bounds = rect;

					rect.Left = rect.Right;
					rect.Width = dim;
					this.panes[1].Bounds = rect;
				}

				this.panes[0].SetVisible( this.panes[0].Width > this.hideSize[0] );
				this.panes[1].SetVisible( this.panes[1].Width > this.hideSize[1] );

				if ( this.flipFlop )
				{
					this.button.Show();
					rect.Left   = this.slider.Left-3;
					rect.Right  = this.slider.Right+3;
					rect.Bottom = this.slider.Bottom;
					rect.Top    = this.slider.Bottom+12;
					this.button.Bounds = rect;

					if ( this.RetSize(0) == this.minSize[0] )
					{
						this.button.Direction = Direction.Right;
					}
					else
					{
						this.button.Direction = Direction.Left;
					}
				}
				else
				{
					this.button.Hide();
				}
			}

			if ( this.paneStyle == PaneStyle.BottomTop )
			{
				double total = rect.Height;
				dim = System.Math.Min(dim, total-this.sliderDim);
				dim = System.Math.Floor(dim);

				if ( rank == 0 )  // panneau inf�rieur ?
				{
					if ( dim < this.hideSize[0] )  dim = 0;

					rect.Height = dim;
					this.panes[0].Bounds = rect;

					rect.Bottom = rect.Top;
					rect.Height = this.sliderDim;
					this.slider.Bounds = rect;

					rect.Bottom = rect.Top;
					rect.Height = total-dim-this.sliderDim;
					this.panes[1].Bounds = rect;
				}

				if ( rank == 1 )  // panneau sup�rieur ?
				{
					if ( dim < this.hideSize[1] )  dim = 0;

					rect.Height = total-dim;
					this.panes[0].Bounds = rect;

					rect.Bottom = rect.Top;
					rect.Height = this.sliderDim;
					this.slider.Bounds = rect;

					rect.Bottom = rect.Top;
					rect.Height = dim;
					this.panes[1].Bounds = rect;
				}

				this.panes[0].SetVisible( this.panes[0].Height > this.hideSize[0] );
				this.panes[1].SetVisible( this.panes[1].Height > this.hideSize[1] );

				if ( this.flipFlop )
				{
					this.button.Show();
					rect.Right  = this.slider.Right;
					rect.Left   = this.slider.Right-12;
					rect.Bottom = this.slider.Bottom-3;
					rect.Top    = this.slider.Top+3;
					this.button.Bounds = rect;

					if ( this.RetSize(1) == this.minSize[1] )
					{
						this.button.Direction = Direction.Down;
					}
					else
					{
						this.button.Direction = Direction.Up;
					}
				}
				else
				{
					this.button.Hide();
				}
			}
		}

		// Retourne une dimension.
		public double RetSize(int rank)
		{
			if ( rank < 0 || rank >= 2 )
			{
				throw new System.ArgumentOutOfRangeException();
			}

			if ( this.paneStyle == PaneStyle.LeftRight )
			{
				return this.panes[rank].Width;
			}

			if ( this.paneStyle == PaneStyle.BottomTop )
			{
				return this.panes[rank].Height;
			}

			return 0;
		}

		// Sp�cifie une dimension en dessous de laquelle la panneau est cach�.
		public void SetHideSize(int rank, double dim)
		{
			if ( rank < 0 || rank >= 2 )
			{
				throw new System.ArgumentOutOfRangeException();
			}
			this.hideSize[rank] = dim;
		}

		// Sp�cifie une dimension minimale.
		public void SetMinSize(int rank, double dim)
		{
			if ( rank < 0 || rank >= 2 )
			{
				throw new System.ArgumentOutOfRangeException();
			}
			this.minSize[rank] = dim;
		}

		// Sp�cifie une dimension maximale.
		public void SetMaxSize(int rank, double dim)
		{
			if ( rank < 0 || rank >= 2 )
			{
				throw new System.ArgumentOutOfRangeException();
			}
			this.maxSize[rank] = dim;
		}

		// Appel� lorsque le slider va �tre d�plac�.
		private void HandleSliderDragStarted(object sender, MessageEventArgs e)
		{
			if ( this.flipFlop )  return;

			Widget slider = sender as Widget;
			Drawing.Point pos = slider.MapClientToParent(e.Point);
			
			switch ( this.paneBehaviour )
			{
				case PaneBehaviour.Draft:
					this.alphaBar = new AlphaBar();
					this.alphaBar.Bounds = this.slider.Bounds;
					this.alphaBar.Parent = this.Window.Root;
					this.sliderDragRect = this.slider.Bounds;

					switch ( this.paneStyle )
					{
						case PaneStyle.LeftRight:
							this.sliderDragPos = pos.X;
							this.sliderDragDim = this.panes[0].Width;
							break;
						case PaneStyle.BottomTop:
							this.sliderDragPos = pos.Y;
							this.sliderDragDim = this.panes[1].Height;
							break;
					}
					break;

				case PaneBehaviour.FollowMe:
					switch ( this.paneStyle )
					{
						case PaneStyle.LeftRight:
							this.sliderDragPos = pos.X;
							this.sliderDragDim = this.panes[0].Width;
							break;
						case PaneStyle.BottomTop:
							this.sliderDragPos = pos.Y;
							this.sliderDragDim = this.panes[1].Height;
							break;
					}
					break;
			}
		}

		// Appel� lorsque le slider est d�plac�.
		private void HandleSliderDragMoved(object sender, MessageEventArgs e)
		{
			if ( this.flipFlop )  return;

			Widget slider = sender as Widget;
			Drawing.Point pos = slider.MapClientToParent(e.Point);
			Drawing.Rectangle rect = this.sliderDragRect;
			
			System.Diagnostics.Debug.Assert(this.panes.Length == 2);
			
			switch ( this.paneBehaviour )
			{
				case PaneBehaviour.Draft:
					switch ( this.paneStyle )
					{
						case PaneStyle.LeftRight:
							rect.Offset(pos.X-this.sliderDragPos, 0);
							this.alphaBar.Bounds = rect;
							break;
						
						case PaneStyle.BottomTop:
							rect.Offset(0, pos.Y-this.sliderDragPos);
							this.alphaBar.Bounds = rect;
							break;
					}
					break;

				case PaneBehaviour.FollowMe:
					switch ( this.paneStyle )
					{
						case PaneStyle.LeftRight:
							this.sliderDragDim += pos.X - this.sliderDragPos;
							this.sliderDragPos  = pos.X;
							this.SetSize(0, this.sliderDragDim);
							break;
					
						case PaneStyle.BottomTop:
							this.sliderDragDim -= pos.Y - this.sliderDragPos;
							this.sliderDragPos  = pos.Y;
							this.SetSize(1, this.sliderDragDim);
							break;
					}
					break;
			}
			
			this.OnSizeChanged();
		}

		// Appel� lorsque le slider est fini de d�placer.
		private void HandleSliderDragEnded(object sender, MessageEventArgs e)
		{
			if ( this.flipFlop )  return;

			Widget slider = sender as Widget;
			Drawing.Point pos = slider.MapClientToParent(e.Point);
			
			switch ( this.paneBehaviour )
			{
				case PaneBehaviour.Draft:
					switch ( this.paneStyle )
					{
						case PaneStyle.LeftRight:
							this.sliderDragDim += pos.X - this.sliderDragPos;
							this.sliderDragPos  = pos.X;
							this.SetSize(0, this.sliderDragDim);
							break;
						
						case PaneStyle.BottomTop:
							this.sliderDragDim -= pos.Y - this.sliderDragPos;
							this.sliderDragPos  = pos.Y;
							this.SetSize(1, this.sliderDragDim);
							break;
					}

					this.Window.Root.Children.Remove(this.alphaBar);
					this.alphaBar = null;
					break;

				case PaneBehaviour.FollowMe:
					break;
			}

			this.OnSizeChanged();
		}

		// Bouton flip-flop cliqu�.
		private void HandleButtonClicked(object sender, MessageEventArgs e)
		{
			if ( this.paneStyle == PaneStyle.LeftRight )
			{
				if ( this.RetSize(0) == this.minSize[0] )
				{
					this.SetSize(0, this.maxSize[0]);
					OnSizeChanged();
				}
				else
				{
					this.SetSize(0, this.minSize[0]);
					OnSizeChanged();
				}
			}

			if ( this.paneStyle == PaneStyle.BottomTop )
			{
				if ( this.RetSize(1) == this.minSize[1] )
				{
					this.SetSize(1, this.maxSize[1]);
					OnSizeChanged();
				}
				else
				{
					this.SetSize(1, this.minSize[1]);
					OnSizeChanged();
				}
			}
		}

		// G�n�re un �v�nement pour dire que la s�lection dans la liste a chang�.
		protected virtual void OnSizeChanged()
		{
			if ( this.SizeChanged != null )  // qq'un �coute ?
			{
				this.SizeChanged(this);
			}
		}


		public event Support.EventHandler	SizeChanged;

		protected PaneStyle					paneStyle = PaneStyle.LeftRight;
		protected PaneBehaviour				paneBehaviour = PaneBehaviour.Draft;
		protected bool						flipFlop = false;
		protected Widget[]					panes;
		protected ArrowButton				button;
		protected PaneButton				slider;
		protected double					sliderDim = 5;
		protected double					sliderDragPos;
		protected double					sliderDragDim;
		protected Drawing.Rectangle			sliderDragRect;
		protected double[]					hideSize = new double[2];
		protected double[]					minSize = new double[2];
		protected double[]					maxSize = new double[2];
		protected AlphaBar					alphaBar;
	}
}
