namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe TextFieldSlider implémente la ligne éditable numérique avec slider.
	/// </summary>
	public class TextFieldSlider : Widget
	{
		public TextFieldSlider()
		{
			this.field = new TextFieldUpDown(this);
			this.field.TextChanged += new EventHandler(this.FieldTextChanged);

			this.slider = new Slider(this);
			this.slider.Frame = false;
			this.slider.ValueChanged += new EventHandler(this.SliderValueChanged);
		}
		
		public TextFieldSlider(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.field.TextChanged -= new EventHandler(this.FieldTextChanged);
				this.slider.ValueChanged -= new EventHandler(this.SliderValueChanged);
				
				this.field.Dispose();
				this.slider.Dispose();
				this.field = null;
				this.slider = null;
			}
			
			base.Dispose(disposing);
		}
		
		// Valeur numérique éditée.
		public double Value
		{
			get
			{
				return this.editValue;
			}

			set
			{
				if ( this.editValue != value )
				{
					this.editValue = value;
					this.field.Value = value;
					this.slider.Value = value;
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
				this.field.MinRange = value;
				this.slider.MinRange = value;
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
				this.field.MaxRange = value;
				this.slider.MaxRange = value;
			}
		}
		
		// Pas pour les boutons up/down.
		public double Step
		{
			get
			{
				return this.step;
			}

			set
			{
				this.step = value;
				this.field.Step = value;
			}
		}

		// Couleur du slider.
		public Drawing.Color Color
		{
			get
			{
				return this.slider.Color;
			}

			set
			{
				this.slider.Color = value;
			}
		}

		// Couleur de fond du slider.
		public Drawing.Color ColorBack
		{
			get
			{
				return this.slider.ColorBack;
			}

			set
			{
				this.slider.ColorBack = value;
			}
		}
		
		public override double DefaultHeight
		{
			get
			{
				return this.DefaultFontHeight + 2*AbstractTextField.Margin;
			}
		}


		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			if ( this.field == null )  return;

			this.field.BottomMargin = this.sliderHeight-AbstractTextField.FrameMargin;
			
			Drawing.Rectangle rect = new Drawing.Rectangle(0, 0, this.Client.Width, this.Client.Height);
			this.field.Bounds = rect;
			
			rect = new Drawing.Rectangle(0, 0, this.Client.Width, this.Client.Height);
			rect.Width -= System.Math.Floor(rect.Height*0.6)-1;
			rect.Height = this.sliderHeight;
			this.slider.Bounds = rect;
		}

		// Texte changé.
		private void FieldTextChanged(object sender)
		{
			this.editValue = this.field.Value;
			this.slider.Value = this.editValue;
			this.OnTextChanged();
		}

		// Slider bougé.
		private void SliderValueChanged(object sender)
		{
			this.editValue = this.slider.Value;
			this.field.Value = this.editValue;
			this.OnTextChanged();
		}

		// Génère un événement pour dire que la valeur a changé.
		protected virtual void OnTextChanged()
		{
			if ( this.TextChanged != null )  // qq'un écoute ?
			{
				this.TextChanged(this);
			}
		}

		public event EventHandler TextChanged;


		protected TextFieldUpDown				field;
		protected Slider						slider;
		protected double						editValue = -9999999;
		protected double						minRange = 0;
		protected double						maxRange = 100;
		protected double						step = 1;
		protected double						sliderHeight = 5;
	}
}
