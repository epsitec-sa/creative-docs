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
			this.internal_state |= InternalState.AutoEngage;
			this.internal_state |= InternalState.Engageable;

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


		// Retourne la largeur standard d'un ascenseur.
		public static double StandardWidth
		{
			get
			{
				return Scroller.standardWidth;
			}
		}


		// Hauteur totale représentée par l'ascenseur.
		public double Range
		{
			get
			{
				return this.range;
			}

			set
			{
				if ( value < 0 )  value = 0;

				if ( value != this.range )
				{
					this.range = value;
					this.Invalidate();
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
				if ( value > this.range )  value = this.range;

				if ( value != this.display )
				{
					this.display = value;
					this.Invalidate();
				}
			}
		}

		// Position représentée par l'ascenseur.
		public double Position
		{
			get
			{
				return this.position;
			}

			set
			{
				if ( value < 0          )  value = 0;
				if ( value > this.range )  value = this.range;

				if ( value != this.position )
				{
					this.position = value;
					this.Invalidate();
					OnMoved();
				}
			}
		}
		
		// Position avancée par les boutons.
		public double ButtonStep
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
		
		// Position avancée en cliquant hors de la cabine.
		public double PageStep
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
					if ( this.range > 0 && this.display > 0 )
					{
						this.mouseDown = true;
						BeginPress(this.vertical ? pos.Y : pos.X);
					}
					break;
				
				case MessageType.MouseMove:
					if ( this.mouseDown )
					{
						MovePress(this.vertical ? pos.Y : pos.X);
					}
					break;

				case MessageType.MouseUp:
					if ( this.mouseDown )
					{
						EndPress(this.vertical ? pos.Y : pos.X);
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
				if ( this.vertical )
				{
					this.Position = (pos-this.cabOffset-this.sliderRect.Bottom)*this.range/(this.sliderRect.Height-this.cabRect.Height);
				}
				else
				{
					this.Position = (pos-this.cabOffset-this.sliderRect.Left)*this.range/(this.sliderRect.Width-this.cabRect.Width);
				}
			}
		}

		// Appelé lorsque la souris est maintenue pressée.
		// TODO: appeler cette méthode régulièrement ...
		protected void DelayPress()
		{
			if ( this.mouseDown && this.pageScroll != 0 )
			{
				this.Position += this.pageScroll;
			}
		}

		// Appelé lorsque le bouton de la souris est relâché pour déplacer la cabine.
		protected void EndPress(double pos)
		{
			MovePress(pos);
			this.pageScroll = 0;
		}

		// Gestion d'un événement lorsqu'un bouton est pressé.
		private void HandleButton(object sender)
		{
			ArrowButton button = sender as ArrowButton;

			if ( button == this.arrowUp )
			{
				this.Position += this.buttonStep;
				this.Invalidate();
			}
			else if ( button == this.arrowDown )
			{
				this.Position -= this.buttonStep;
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

			if ( this.range > 0 && this.display > 0 )
			{
				if ( this.vertical )
				{
					double h = this.sliderRect.Height*this.display/this.range;
					if ( h < Scroller.minimalCab )  h = Scroller.minimalCab;
					double p = (this.position/this.range)*(this.sliderRect.Height-h);
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
					double h = this.sliderRect.Width*this.display/this.range;
					if ( h < Scroller.minimalCab )  h = Scroller.minimalCab;
					double p = (this.position/this.range)*(this.sliderRect.Width-h);
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
			if ( this.range > 0 && this.display > 0 )
			{
				Widgets.Direction dir = this.vertical ? Direction.Up : Direction.Left;
				adorner.PaintScrollerHandle(graphics, this.cabRect, Drawing.Rectangle.Empty, this.PaintState&(~WidgetState.Engaged), dir);
			}
		}


		public event EventHandler Moved;

		protected static double		standardWidth = 15;
		protected static double		minimalCab = 8;
		protected bool				vertical = true;
		protected double			range = 1;
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
