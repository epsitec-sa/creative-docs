namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe Scroller implémente un ascenseur.
	/// </summary>
	public class Scroller : Widget
	{
		// Constructeur de l'ascenseur.
		public Scroller()
		{
			this.internalState |= InternalState.AutoEngage;
			this.internalState |= InternalState.Engageable;
			this.internalState |= InternalState.AutoRepeatEngaged;

			this.arrowUp = new ArrowButton();
			this.arrowDown = new ArrowButton();
			this.arrowUp.Direction = Direction.Up;
			this.arrowDown.Direction = Direction.Down;
			this.arrowUp.ButtonStyle = ButtonStyle.Scroller;
			this.arrowDown.ButtonStyle = ButtonStyle.Scroller;
			this.arrowUp.Engaged += new EventHandler(this.HandleButton);
			this.arrowDown.Engaged += new EventHandler(this.HandleButton);
			this.arrowUp.StillEngaged += new EventHandler(this.HandleButton);
			this.arrowDown.StillEngaged += new EventHandler(this.HandleButton);
			this.arrowUp.AutoRepeatEngaged = true;
			this.arrowDown.AutoRepeatEngaged = true;
			this.Children.Add(this.arrowUp);
			this.Children.Add(this.arrowDown);
		}


		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				System.Diagnostics.Debug.WriteLine("Dispose Scroller " + this.Text);
				
				this.arrowUp.Engaged -= new EventHandler(this.HandleButton);
				this.arrowDown.Engaged -= new EventHandler(this.HandleButton);
				this.arrowUp.StillEngaged -= new EventHandler(this.HandleButton);
				this.arrowDown.StillEngaged -= new EventHandler(this.HandleButton);
			}
			
			base.Dispose(disposing);
		}


		protected override void OnStillEngaged()
		{
			base.OnStillEngaged ();
			this.DelayPress ();
		}


		// Retourne la largeur standard d'un ascenseur.
		public static double StandardWidth
		{
			get
			{
				return Scroller.standardWidth;
			}
		}


		// Inversion du fonctionnement.
		// Ascenseur vertical:   false -> zéro en bas
		// Ascenseur vertical:   true  -> zéro en haut
		// Ascenseur horizontal: false -> zéro à gauche
		// Ascenseur horizontal: true  -> zéro à droite
		public bool Invert
		{
			get
			{
				return this.invert;
			}

			set
			{
				if ( value != this.invert )
				{
					this.invert = value;
					this.Invalidate();
				}
			}
		}
		
		// Hauteur totale représentée par l'ascenseur.
		public double Range
		{
			get
			{
				return System.Math.Max (0, this.maximum - this.minimum);
			}

			set
			{
				if ( value < 0 )  value = 0;
				
				this.Maximum = value + this.minimum;
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
		
		// Hauteur visible représentée par l'ascenseur.
		public double Display
		{
			get
			{
				return this.display;
			}

			set
			{
				if ( value < 0          )  value = 0;
				if ( value > this.Range )  value = this.Range;

				if ( value != this.display )
				{
					this.display = value;
					this.Invalidate();
				}
			}
		}

		// Valeur représentée par l'ascenseur (position).
		public double Value
		{
			get
			{
				return this.position + this.minimum;
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
					OnMoved();
				}
			}
		}
		
		// Valeur avancée par les boutons.
		public double SmallChange
		{
			get
			{
				return this.buttonStep;
			}

			set
			{
				this.buttonStep = value;
			}
		}
		
		// Valeur avancée en cliquant hors de la cabine.
		public double LargeChange
		{
			get
			{
				return this.pageStep;
			}

			set
			{
				this.pageStep = value;
			}
		}
		
		
		public ArrowButton ArrowUp
		{
			get { return this.arrowUp; }
		}
		
		public ArrowButton ArrowDown
		{
			get { return this.arrowDown; }
		}
		
		
		// Met à jour la géométrie des boutons de l'ascenseur.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			Drawing.Rectangle rect = this.Bounds;
			this.vertical = (rect.Height > rect.Width);

			if ( this.arrowUp != null )
			{
				if ( this.vertical )
				{
					Drawing.Rectangle aRect = new Drawing.Rectangle(0, rect.Height-rect.Width, rect.Width, rect.Width);
					this.arrowUp.Bounds = aRect;
					this.arrowUp.Direction = Direction.Up;
				}
				else
				{
					Drawing.Rectangle aRect = new Drawing.Rectangle(rect.Width-rect.Height, 0, rect.Height, rect.Height);
					this.arrowUp.Bounds = aRect;
					this.arrowUp.Direction = Direction.Right;
				}
			}
			if ( this.arrowDown != null )
			{
				if ( this.vertical )
				{
					Drawing.Rectangle aRect = new Drawing.Rectangle(0, 0, rect.Width, rect.Width);
					this.arrowDown.Bounds = aRect;
					this.arrowDown.Direction = Direction.Down;
				}
				else
				{
					Drawing.Rectangle aRect = new Drawing.Rectangle(0, 0, rect.Height, rect.Height);
					this.arrowDown.Bounds = aRect;
					this.arrowDown.Direction = Direction.Left;
				}
			}
		}


		// Gestion d'un événement.
		protected override void ProcessMessage(Message message, Drawing.Point pos)
		{
			switch ( message.Type )
			{
				case MessageType.MouseDown:
					if ( this.Range > 0 && this.Display > 0 )
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
				if ( pos < this.cabRect.Bottom )
				{
					this.pageScroll = -this.pageStep;
					this.DelayPress();
				}
				else if ( pos > this.cabRect.Top )
				{
					this.pageScroll = this.pageStep;
					this.DelayPress();
				}
				else
				{
					this.pageScroll = 0;
					this.cabOffset = pos-this.cabRect.Bottom;
				}
			}
			else
			{
				if ( pos < this.cabRect.Left )
				{
					this.pageScroll = -this.pageStep;
					this.DelayPress();
				}
				else if ( pos > this.cabRect.Right )
				{
					this.pageScroll = this.pageStep;
					this.DelayPress();
				}
				else
				{
					this.pageScroll = 0;
					this.cabOffset = pos-this.cabRect.Left;
				}
			}
		}

		// Appelé lorsque la souris est déplacée pour déplacer la cabine.
		protected void MovePress(double pos)
		{
			if ( this.pageScroll == 0 )
			{
				double p;
				if ( this.vertical )
				{
					p = (pos-this.cabOffset-this.sliderRect.Bottom)*this.Range/(this.sliderRect.Height-this.cabRect.Height);
				}
				else
				{
					p = (pos-this.cabOffset-this.sliderRect.Left)*this.Range/(this.sliderRect.Width-this.cabRect.Width);
				}
				if ( this.invert )  p = this.Range-p;
				this.Value = this.minimum + p;
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
			ArrowButton button = sender as ArrowButton;

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
		protected virtual void OnMoved()
		{
			if ( this.Moved != null )  // qq'un écoute ?
			{
				this.Moved(this);
			}
		}


		// Dessine l'ascenseur.
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			IAdorner adorner = Widgets.Adorner.Factory.Active;

			Drawing.Rectangle rect = new Drawing.Rectangle(0, 0, this.Client.Width, this.Client.Height);
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

			if ( this.Range > 0 && this.Display > 0 )
			{
				double pos = this.position;
				if ( this.invert )  pos = this.Range-pos;

				if ( this.vertical )
				{
					double h = this.sliderRect.Height*this.display/this.Range;
					if ( h < Scroller.minimalCab )  h = Scroller.minimalCab;
					double p = (pos/this.Range)*(this.sliderRect.Height-h);
					this.cabRect = this.sliderRect;
					this.cabRect.Bottom += p;
					this.cabRect.Height = h;

					if ( this.pageScroll < 0 )
					{
						tabRect = this.sliderRect;
						tabRect.Top = this.cabRect.Bottom;
					}
					if ( this.pageScroll > 0 )
					{
						tabRect = this.sliderRect;
						tabRect.Bottom = this.cabRect.Top;
					}
				}
				else
				{
					double h = this.sliderRect.Width*this.display/this.Range;
					if ( h < Scroller.minimalCab )  h = Scroller.minimalCab;
					double p = (pos/this.Range)*(this.sliderRect.Width-h);
					this.cabRect = this.sliderRect;
					this.cabRect.Left += p;
					this.cabRect.Width = h;

					if ( this.pageScroll < 0 )
					{
						tabRect = this.sliderRect;
						tabRect.Right = this.cabRect.Left;
					}
					if ( this.pageScroll > 0 )
					{
						tabRect = this.sliderRect;
						tabRect.Left = this.cabRect.Right;
					}
				}
			}

			// Dessine le fond.
			adorner.PaintScrollerBackground(graphics, rect, tabRect, this.PaintState, this.RootDirection);
			
			// Dessine la cabine.
			if ( this.Range > 0 && this.Display > 0 && this.IsEnabled )
			{
				Widgets.Direction dir = this.vertical ? Direction.Up : Direction.Left;
				adorner.PaintScrollerHandle(graphics, this.cabRect, Drawing.Rectangle.Empty, this.PaintState&(~WidgetState.Engaged), dir);
			}
		}


		public event EventHandler Moved;

		protected static double		standardWidth = 15;
		protected static double		minimalCab = 8;
		protected bool				vertical = true;
		protected bool				invert = false;
		protected double			minimum = 0.0;
		protected double			maximum = 1.0;
		protected double			display = 0.5;
		protected double			position = 0;
		protected double			buttonStep = 0.1;
		protected double			pageStep = 0.2;
		protected ArrowButton		arrowUp;
		protected ArrowButton		arrowDown;
		protected bool				mouseDown = false;
		protected double			cabOffset = 0;
		protected double			pageScroll = 0;
		protected Drawing.Rectangle	sliderRect = new Drawing.Rectangle();
		protected Drawing.Rectangle	cabRect = new Drawing.Rectangle();
	}
}
