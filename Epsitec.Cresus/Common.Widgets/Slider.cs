namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe Slider implémente un curseur de réglage.
	/// </summary>
	public class Slider : Widget
	{
		public Slider()
		{
			IAdorner adorner = Widgets.Adorner.Factory.Active;
			this.color = adorner.GetColorCaption();
			this.colorBack = Drawing.Color.FromBrightness(1);
		}
		
		public Slider(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}

		// Valeur numérique représentée.
		public double Value
		{
			get
			{
				return this.sliderValue;
			}

			set
			{
				if ( this.sliderValue != value )
				{
					this.sliderValue = value;
					this.Invalidate();
				}
			}
		}

		// Valeur numérique minimale possible.
		public double MinRange
		{
			get
			{
				return this.minRange;
			}

			set
			{
				this.minRange = value;
			}
		}
		
		// Valeur numérique maximale possible.
		public double MaxRange
		{
			get
			{
				return this.maxRange;
			}

			set
			{
				this.maxRange = value;
			}
		}
		
		// Couleur du slider.
		public Drawing.Color Color
		{
			get
			{
				return this.color;
			}

			set
			{
				if ( this.color != value )
				{
					this.color = value;
					this.Invalidate();
				}
			}
		}
		
		// Couleur de fond du slider.
		public Drawing.Color ColorBack
		{
			get
			{
				return this.colorBack;
			}

			set
			{
				if ( this.colorBack != value )
				{
					this.colorBack = value;
					this.Invalidate();
				}
			}
		}
		

		// Gestion d'un événement.
		protected override void ProcessMessage(Message message, Drawing.Point pos)
		{
			switch ( message.Type )
			{
				case MessageType.MouseDown:
					this.mouseDown = true;
					this.MouseDown(pos);
					break;
				
				case MessageType.MouseMove:
					if ( this.mouseDown )
					{
						this.MouseMove(pos);
					}
					break;

				case MessageType.MouseUp:
					if ( this.mouseDown )
					{
						this.MouseUp(pos);
						this.mouseDown = false;
					}
					break;
			}
			
			message.Consumer = this;
		}

		protected void MouseDown(Drawing.Point pos)
		{
			this.Value = this.Detect(pos);
			this.OnValueChanged();
		}

		protected void MouseMove(Drawing.Point pos)
		{
			this.Value = this.Detect(pos);
			this.OnValueChanged();
		}

		protected void MouseUp(Drawing.Point pos)
		{
			this.Value = this.Detect(pos);
			this.OnValueChanged();
		}

		protected double Detect(Drawing.Point pos)
		{
			Drawing.Rectangle rect = new Drawing.Rectangle(0, 0, this.Client.Width, this.Client.Height);
			rect.Inflate(-this.margin, -this.margin);
			double val = this.minRange+(pos.X-rect.Left)*(this.maxRange-this.minRange)/rect.Width;
			val = System.Math.Max(val, this.minRange);
			val = System.Math.Min(val, this.maxRange);
			val = System.Math.Floor(val+0.5);
			return val;
		}

		// Génère un événement pour dire que le slider a changé.
		protected virtual void OnValueChanged()
		{
			if ( this.ValueChanged != null )  // qq'un écoute ?
			{
				this.ValueChanged(this);
			}
		}

		public event EventHandler ValueChanged;


		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			IAdorner adorner = Widgets.Adorner.Factory.Active;

			Drawing.Rectangle rect   = new Drawing.Rectangle(0, 0, this.Client.Width, this.Client.Height);
			WidgetState       state  = this.PaintState;
			Direction         dir    = this.RootDirection;
			
			adorner.PaintTextFieldBackground(graphics, rect, state, dir, TextFieldStyle.Normal, false);

			if ( this.IsEnabled )
			{
				rect.Inflate(-this.margin, -this.margin);
				graphics.AddFilledRectangle(rect);
				graphics.RenderSolid(this.colorBack);
				rect.Width *= (this.sliderValue-this.minRange)/(this.maxRange-this.minRange);
				graphics.AddFilledRectangle(rect);
				graphics.RenderSolid(this.color);
			}
		}

		
		protected double					sliderValue = 0;
		protected double					minRange = 0;
		protected double					maxRange = 100;
		protected Drawing.Color				color;
		protected Drawing.Color				colorBack;
		protected bool						mouseDown = false;
		protected double					margin = 1;
	}
}
