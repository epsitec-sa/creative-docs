namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe AbstractScroller implémente la classe de base des ascenseurs
	/// HScroller et VScroller.
	/// </summary>
	[Support.SuppressBundleSupport]
	public abstract class AbstractScroller : Widget
	{
		// Constructeur de l'ascenseur.
		protected AbstractScroller(bool vertical)
		{
			this.vertical = vertical;
			
			this.InternalState |= InternalState.AutoEngage;
			this.InternalState |= InternalState.Engageable;
			this.InternalState |= InternalState.AutoRepeatEngaged;

			this.arrowUp = new GlyphButton(this);
			this.arrowDown = new GlyphButton(this);
			this.arrowUp.GlyphType = GlyphType.ArrowUp;
			this.arrowDown.GlyphType = GlyphType.ArrowDown;
			this.arrowUp.ButtonStyle = ButtonStyle.Scroller;
			this.arrowDown.ButtonStyle = ButtonStyle.Scroller;
			this.arrowUp.Engaged += new Support.EventHandler(this.HandleButton);
			this.arrowDown.Engaged += new Support.EventHandler(this.HandleButton);
			this.arrowUp.StillEngaged += new Support.EventHandler(this.HandleButton);
			this.arrowDown.StillEngaged += new Support.EventHandler(this.HandleButton);
			this.arrowUp.AutoRepeatEngaged = true;
			this.arrowDown.AutoRepeatEngaged = true;
		}
		
		protected AbstractScroller(Widget embedder, bool vertical) : this(vertical)
		{
			this.SetEmbedder(embedder);
		}


		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.arrowUp.Engaged -= new Support.EventHandler(this.HandleButton);
				this.arrowDown.Engaged -= new Support.EventHandler(this.HandleButton);
				this.arrowUp.StillEngaged -= new Support.EventHandler(this.HandleButton);
				this.arrowDown.StillEngaged -= new Support.EventHandler(this.HandleButton);
			}
			
			base.Dispose(disposing);
		}


		protected override void OnStillEngaged()
		{
			base.OnStillEngaged ();
			this.DelayPress ();
		}

		protected override void SetBounds(double x1, double y1, double x2, double y2)
		{
			double dx = x2-x1;
			double dy = y2-y1;
			
			this.sizeOk = (dx >= this.MinSize.Width) && (dy >= this.MinSize.Height);
			
			base.SetBounds (x1, y1, x2, y2);
		}


		public bool IsInverted
		{
			// Inversion du fonctionnement.
			// Ascenseur vertical:   false -> zéro en bas
			// Ascenseur vertical:   true  -> zéro en haut
			// Ascenseur horizontal: false -> zéro à gauche
			// Ascenseur horizontal: true  -> zéro à droite
			
			get
			{
				return this.invert;
			}

			set
			{
				if (this.invert != value)
				{
					this.invert = value;
					this.Invalidate();
				}
			}
		}
		
		
		public double Range
		{
			//	Hauteur totale représentée par l'ascenseur (unités quelconques).
			
			get
			{
				return System.Math.Max (0, this.maximum - this.minimum);
			}

			set
			{
				System.Diagnostics.Debug.Assert (value >= 0);
				
				this.Maximum = this.Minimum + value;
			}
		}
		
		public double VisibleRangeRatio
		{
			//	Hauteur visible représentée par l'ascenseur (de 0 à 1).
			
			get
			{
				return this.display;
			}

			set
			{
				System.Diagnostics.Debug.Assert (value >= 0.0);
				System.Diagnostics.Debug.Assert (value <= 1.0);
				
				if (this.display != value)
				{
					this.display = value;
					this.Invalidate();
				}
			}
		}

		public double Minimum
		{
			get
			{
				return this.minimum;
			}
			set
			{
				if (this.minimum != value)
				{
					this.minimum = value;
					this.Invalidate ();
				}
			}
		}
		
		public double Maximum
		{
			get
			{
				return this.maximum;
			}
			set
			{
				if (this.maximum != value)
				{
					this.maximum = value;
					this.Invalidate ();
				}
			}
		}
		
		public double Value
		{
			// Valeur représentée par l'ascenseur (position).
			get
			{
				double pos = this.position + this.minimum;
				
				pos = System.Math.Min (pos, this.maximum);
				pos = System.Math.Max (pos, this.Minimum);
				
				return pos;
			}

			set
			{
				if ( value < this.minimum )  value = this.minimum;
				if ( value > this.maximum )  value = this.maximum;
				
				value -= this.minimum;
				
				if ( value != this.position )
				{
					this.position = value;
					this.Invalidate();
					this.OnValueChanged();
				}
			}
		}
		
		public double SmallChange
		{
			// Valeur avancée par les boutons.
			get
			{
				return this.buttonStep;
			}

			set
			{
				this.buttonStep = value;
			}
		}
		
		public double LargeChange
		{
			// Valeur avancée en cliquant hors de la cabine.
			get
			{
				return this.pageStep;
			}

			set
			{
				this.pageStep = value;
			}
		}
		
		
		public GlyphButton ArrowUp
		{
			get { return this.arrowUp; }
		}
		
		public GlyphButton ArrowDown
		{
			get { return this.arrowDown; }
		}
		
		
		// Met à jour la géométrie des boutons de l'ascenseur.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			Drawing.Rectangle rect = this.Bounds;

			if ( this.arrowUp != null )
			{
				if ( this.vertical )
				{
					Drawing.Rectangle aRect = new Drawing.Rectangle(0, rect.Height-rect.Width, rect.Width, rect.Width);
					this.arrowUp.Bounds = aRect;
					this.arrowUp.GlyphType = GlyphType.ArrowUp;
				}
				else
				{
					Drawing.Rectangle aRect = new Drawing.Rectangle(rect.Width-rect.Height, 0, rect.Height, rect.Height);
					this.arrowUp.Bounds = aRect;
					this.arrowUp.GlyphType = GlyphType.ArrowRight;
				}
			}
			if ( this.arrowDown != null )
			{
				if ( this.vertical )
				{
					Drawing.Rectangle aRect = new Drawing.Rectangle(0, 0, rect.Width, rect.Width);
					this.arrowDown.Bounds = aRect;
					this.arrowDown.GlyphType = GlyphType.ArrowDown;
				}
				else
				{
					Drawing.Rectangle aRect = new Drawing.Rectangle(0, 0, rect.Height, rect.Height);
					this.arrowDown.Bounds = aRect;
					this.arrowDown.GlyphType = GlyphType.ArrowLeft;
				}
			}
		}


		// Gestion d'un événement.
		protected override void ProcessMessage(Message message, Drawing.Point pos)
		{
			if ( !this.sizeOk ) return;

			switch ( message.Type )
			{
				case MessageType.MouseDown:
					if ( this.Range > 0 && this.VisibleRangeRatio > 0 )
					{
						this.mouseDown = true;
						this.BeginPress(this.vertical ? pos.Y : pos.X);
					}
					break;
				
				case MessageType.MouseMove:
					if ( this.mouseDown )
					{
						this.MovePress(this.vertical ? pos.Y : pos.X);
					}
					break;

				case MessageType.MouseUp:
					if ( this.mouseDown )
					{
						this.EndPress(this.vertical ? pos.Y : pos.X);
						this.mouseDown = false;
					}
					break;
			}
			
			message.Consumer = this;
		}

		// Appelé lorsque le bouton de la souris est pressé pour déplacer la cabine.
		protected void BeginPress(double pos)
		{
			if ( this.vertical )
			{
				if ( pos < this.thumbRect.Bottom )
				{
					this.pageScroll = -this.pageStep;
					this.DelayPress();
				}
				else if ( pos > this.thumbRect.Top )
				{
					this.pageScroll = this.pageStep;
					this.DelayPress();
				}
				else
				{
					this.pageScroll = 0;
					this.thumbOffset = pos-this.thumbRect.Bottom;
				}
			}
			else
			{
				if ( pos < this.thumbRect.Left )
				{
					this.pageScroll = -this.pageStep;
					this.DelayPress();
				}
				else if ( pos > this.thumbRect.Right )
				{
					this.pageScroll = this.pageStep;
					this.DelayPress();
				}
				else
				{
					this.pageScroll = 0;
					this.thumbOffset = pos-this.thumbRect.Left;
				}
			}
		}

		// Appelé lorsque la souris est déplacée pour déplacer la cabine.
		protected void MovePress(double pos)
		{
			if ( this.pageScroll == 0 )
			{
				double offset;
				double length;
				
				if ( this.vertical )
				{
					offset = pos-this.thumbOffset-this.sliderRect.Bottom;
					length = this.sliderRect.Height-this.thumbRect.Height;
				}
				else
				{
					offset = pos-this.thumbOffset-this.sliderRect.Left;
					length = this.sliderRect.Width-this.thumbRect.Width;
				}
				
				if ( length > 0 )
				{
					double new_pos = offset / length;
					
					if ( this.invert )
					{
						new_pos = 1.0 - new_pos;
					}
					
					this.Value = new_pos * this.Range + this.Minimum;
				}
			}
		}

		// Appelé lorsque la souris est maintenue pressée.
		// TODO: appeler cette méthode régulièrement ...
		protected void DelayPress()
		{
			System.Diagnostics.Debug.Assert (this.mouseDown);
			
			if ( this.pageScroll != 0 )
			{
				if ( this.invert )  this.Value -= this.pageScroll;
				else                this.Value += this.pageScroll;
			}
		}

		// Appelé lorsque le bouton de la souris est relâché pour déplacer la cabine.
		protected void EndPress(double pos)
		{
			this.MovePress(pos);
			this.pageScroll = 0;
		}

		// Gestion d'un événement lorsqu'un bouton est pressé.
		private void HandleButton(object sender)
		{
			GlyphButton button = sender as GlyphButton;

			if ( button == this.arrowUp )
			{
				if ( this.invert )  this.Value -= this.buttonStep;
				else                this.Value += this.buttonStep;
				this.Invalidate();
			}
			else if ( button == this.arrowDown )
			{
				if ( this.invert )  this.Value += this.buttonStep;
				else                this.Value -= this.buttonStep;
				this.Invalidate();
			}
		}


		// Génère un événement pour dire que l'ascenseur a bougé.
		protected virtual void OnValueChanged()
		{
			if ( this.ValueChanged != null )  // qq'un écoute ?
			{
				this.ValueChanged(this);
			}
		}


		// Dessine l'ascenseur.
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			if ( !this.sizeOk ) return;
			
			IAdorner adorner = Widgets.Adorner.Factory.Active;

			Drawing.Rectangle rect = this.Client.Bounds;
			this.sliderRect = rect;
			if ( this.vertical )
			{
				this.sliderRect.Inflate(0, -this.sliderRect.Width);
			}
			else
			{
				this.sliderRect.Inflate(-this.sliderRect.Height, 0);
			}

			Drawing.Rectangle tabRect = Drawing.Rectangle.Empty;

			if ( this.Range > 0 && this.VisibleRangeRatio > 0 )
			{
				double pos = this.position;
				if ( this.invert )  pos = this.Range-pos;

				if ( this.vertical )
				{
					double h = this.sliderRect.Height*this.display;
					if ( h < AbstractScroller.minimalThumb )  h = AbstractScroller.minimalThumb;
					double p = (pos/this.Range)*(this.sliderRect.Height-h);
					this.thumbRect = this.sliderRect;
					this.thumbRect.Bottom += p;
					this.thumbRect.Height = h;

					if ( this.pageScroll < 0 )
					{
						tabRect = this.sliderRect;
						tabRect.Top = this.thumbRect.Bottom;
					}
					if ( this.pageScroll > 0 )
					{
						tabRect = this.sliderRect;
						tabRect.Bottom = this.thumbRect.Top;
					}
				}
				else
				{
					double h = this.sliderRect.Width*this.display;
					if ( h < AbstractScroller.minimalThumb )  h = AbstractScroller.minimalThumb;
					double p = (pos/this.Range)*(this.sliderRect.Width-h);
					this.thumbRect = this.sliderRect;
					this.thumbRect.Left += p;
					this.thumbRect.Width = h;

					if ( this.pageScroll < 0 )
					{
						tabRect = this.sliderRect;
						tabRect.Right = this.thumbRect.Left;
					}
					if ( this.pageScroll > 0 )
					{
						tabRect = this.sliderRect;
						tabRect.Left = this.thumbRect.Right;
					}
				}
			}

			Widgets.Direction dir = this.vertical ? Direction.Up : Direction.Left;

			// Dessine le fond.
			adorner.PaintScrollerBackground(graphics, rect, this.thumbRect, tabRect, this.PaintState, dir);
			
			// Dessine la cabine.
			if ( this.Range > 0 && this.VisibleRangeRatio > 0 && this.IsEnabled )
			{
				rect = this.thumbRect;
				graphics.Align(ref rect);
				adorner.PaintScrollerHandle(graphics, rect, Drawing.Rectangle.Empty, this.PaintState&(~WidgetState.Engaged), dir);
			}
		}


		public event Support.EventHandler ValueChanged;

		
		protected static readonly double	defaultBreadth = 17;
		protected static readonly double	minimalThumb = 8;
		
		protected bool						vertical = true;
		protected bool						sizeOk = false;
		protected bool						invert = false;
		protected double					minimum = 0.0;
		protected double					maximum = 1.0;
		protected double					display = 0.5;
		protected double					position = 0;
		protected double					buttonStep = 0.1;
		protected double					pageStep = 0.2;
		protected GlyphButton				arrowUp;
		protected GlyphButton				arrowDown;
		protected bool						mouseDown;
		protected double					thumbOffset;
		protected double					pageScroll;
		protected Drawing.Rectangle			sliderRect;
		protected Drawing.Rectangle			thumbRect;
	}
}
