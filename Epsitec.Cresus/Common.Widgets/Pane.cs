namespace Epsitec.Common.Widgets
{
	public enum PaneStyle
	{
		LeftRight,			// 2 panneaux côte à côte (0=left, 1=right)
		BottomTop,			// 2 panneaux l'un en dessus de l'autre (0=bottom, 1=top)
	}

	/// <summary>
	/// La classe Pane implémente un double conteneur avec frontière déplaçable.
	/// </summary>
	public class Pane : AbstractGroup
	{
		public Pane()
		{
			this.panes = new Widget[2];

			this.panes[0] = new Widget();
			this.Children.Add(this.panes[0]);

			this.slider = new PaneButton();
			this.slider.PaneButtonStyle = PaneButtonStyle.Vertical;
			this.slider.DragStarted += new MessageEventHandler(this.HandleSliderDragStarted);
			this.slider.DragMoved   += new MessageEventHandler(this.HandleSliderDragMoved);
			this.slider.DragEnded   += new MessageEventHandler(this.HandleSliderDragEnded);
			this.Children.Add(this.slider);

			this.panes[1] = new Widget();
			this.Children.Add(this.panes[1]);

			SetMinDimension(0, 0);
			SetMinDimension(1, 0);
			SetMaxDimension(0, 1000000);
			SetMaxDimension(1, 1000000);
			SetDimension(0, this.Client.Width/2);
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

		// Donne un panneau.
		public Widget RetPane(int rank)
		{
			if ( rank < 0 || rank >= 2 )
			{
				throw new System.ArgumentOutOfRangeException();
			}
			return this.panes[rank];
		}

		// Spécifie une dimension.
		public void SetDimension(int rank, double dim)
		{
			if ( rank < 0 || rank >= 2 )
			{
				throw new System.ArgumentOutOfRangeException();
			}

			Drawing.Rectangle rect = this.Client.Bounds;

			dim = System.Math.Max(dim, this.minDimension[rank]);
			dim = System.Math.Min(dim, this.maxDimension[rank]);

			if ( this.paneStyle == PaneStyle.LeftRight )
			{
				double total = rect.Width;
				dim = System.Math.Min(dim, total-this.sliderDim);
				dim = System.Math.Floor(dim);

				if ( rank == 0 )  // panneau gauche ?
				{
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
					rect.Width = total-dim;
					this.panes[0].Bounds = rect;

					rect.Left = rect.Right;
					rect.Width = this.sliderDim;
					this.slider.Bounds = rect;

					rect.Left = rect.Right;
					rect.Width = dim;
					this.panes[1].Bounds = rect;
				}

				this.panes[0].SetVisible( this.panes[0].Width > 20 );
				this.panes[1].SetVisible( this.panes[1].Width > 20 );
			}

			if ( this.paneStyle == PaneStyle.BottomTop )
			{
				double total = rect.Height;
				dim = System.Math.Min(dim, total-this.sliderDim);
				dim = System.Math.Floor(dim);

				if ( rank == 0 )  // panneau inférieur ?
				{
					rect.Height = dim;
					this.panes[0].Bounds = rect;

					rect.Bottom = rect.Top;
					rect.Height = this.sliderDim;
					this.slider.Bounds = rect;

					rect.Bottom = rect.Top;
					rect.Height = total-dim-this.sliderDim;
					this.panes[1].Bounds = rect;
				}

				if ( rank == 1 )  // panneau supérieur ?
				{
					rect.Height = total-dim;
					this.panes[0].Bounds = rect;

					rect.Bottom = rect.Top;
					rect.Height = this.sliderDim;
					this.slider.Bounds = rect;

					rect.Bottom = rect.Top;
					rect.Height = dim;
					this.panes[1].Bounds = rect;
				}

				this.panes[0].SetVisible( this.panes[0].Height > 20 );
				this.panes[1].SetVisible( this.panes[1].Height > 20 );
			}
		}

		// Retourne une dimension.
		public double RetDimension(int rank)
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

		// Spécifie une dimension minimale.
		public void SetMinDimension(int rank, double dim)
		{
			if ( rank < 0 || rank >= 2 )
			{
				throw new System.ArgumentOutOfRangeException();
			}
			this.minDimension[rank] = dim;
		}

		// Spécifie une dimension maximale.
		public void SetMaxDimension(int rank, double dim)
		{
			if ( rank < 0 || rank >= 2 )
			{
				throw new System.ArgumentOutOfRangeException();
			}
			this.maxDimension[rank] = dim;
		}

		// Appelé lorsque le slider va être déplacé.
		private void HandleSliderDragStarted(object sender, MessageEventArgs e)
		{
			Widget slider = sender as Widget;
			Drawing.Point pos = slider.MapClientToParent (e.Point);
			
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
		}

		// Appelé lorsque le slider est déplacé.
		private void HandleSliderDragMoved(object sender, MessageEventArgs e)
		{
			Widget slider = sender as Widget;
			Drawing.Point pos = slider.MapClientToParent (e.Point);
			
			System.Diagnostics.Debug.Assert(this.panes.Length == 2);
			
			switch ( this.paneStyle )
			{
				case PaneStyle.LeftRight:
					this.sliderDragDim += pos.X - this.sliderDragPos;
					this.sliderDragPos  = pos.X;
					this.SetDimension(0, this.sliderDragDim);
					break;
				
				case PaneStyle.BottomTop:
					this.sliderDragDim -= pos.Y - this.sliderDragPos;
					this.sliderDragPos  = pos.Y;
					this.SetDimension(1, this.sliderDragDim);
					break;
			}
			
			this.OnDimensionChanged();
		}

		// Appelé lorsque le slider est fini de déplacer.
		private void HandleSliderDragEnded(object sender, MessageEventArgs e)
		{
		}

		// Génère un événement pour dire que la sélection dans la liste a changé.
		protected virtual void OnDimensionChanged()
		{
			if ( this.DimensionChanged != null )  // qq'un écoute ?
			{
				this.DimensionChanged(this);
			}
		}


		public event EventHandler DimensionChanged;

		protected PaneStyle					paneStyle = PaneStyle.LeftRight;
		protected Widget[]					panes;
		protected PaneButton				slider;
		protected double					sliderDim = 4;
		protected double					sliderDragPos;
		protected double					sliderDragDim;
		protected double[]					minDimension = new double[2];
		protected double[]					maxDimension = new double[2];
	}
}
