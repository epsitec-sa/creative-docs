namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe TextFieldSlider implémente la ligne éditable numérique avec slider.
	/// </summary>
	public class TextFieldSlider : TextFieldUpDown
	{
		public TextFieldSlider()
		{
			this.slider = new Slider(this);
			this.slider.HasFrame = false;
			this.slider.ValueChanged += new EventHandler(this.HandleSliderValueChanged);
		}
		
		public TextFieldSlider(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.slider.ValueChanged -= new EventHandler(this.HandleSliderValueChanged);
				this.slider.Dispose();
				this.slider = null;
			}
			
			base.Dispose(disposing);
		}
		
		// Valeur numérique minimale possible.
		public override double MinRange
		{
			get
			{
				return base.MinRange;
			}

			set
			{
				base.MinRange = value;
				this.slider.MinRange = value;
			}
		}
		
		// Valeur numérique maximale possible.
		public override double MaxRange
		{
			get
			{
				return base.MaxRange;
			}

			set
			{
				base.MaxRange = value;
				this.slider.MaxRange = value;
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
		public override Drawing.Color BackColor
		{
			get
			{
				return this.slider.BackColor;
			}

			set
			{
				this.slider.BackColor = value;
			}
		}
		

		protected override void UpdateClientGeometry()
		{
			this.margins.Bottom = TextFieldSlider.sliderHeight-AbstractTextField.FrameMargin;
			
			base.UpdateClientGeometry();

			if ( this.arrowUp == null )  return;

			Drawing.Rectangle rect = new Drawing.Rectangle(0, 0, this.Client.Width, this.Client.Height);
			rect.Width -= System.Math.Floor(rect.Height*0.6)-1;
			rect.Height = TextFieldSlider.sliderHeight;
			this.slider.Bounds = rect;
		}

		protected override void OnValueChanged()
		{
			// Valeur numérique éditée.
			base.OnValueChanged ();
			
			if (this.Text != "")
			{
				this.slider.Value = this.Value;
			}
		}

		// Slider bougé.
		private void HandleSliderValueChanged(object sender)
		{
			this.Value = this.slider.Value;
		}


		protected Slider						slider;
		protected static readonly double		sliderHeight = 5;
	}
}
