namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe TextFieldSlider impl�mente la ligne �ditable num�rique avec slider.
	/// </summary>
	public class TextFieldSlider : TextFieldUpDown
	{
		public TextFieldSlider()
		{
			this.slider = new Slider(this);
			this.slider.HasFrame = false;
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
				this.slider.ValueChanged -= new EventHandler(this.SliderValueChanged);
				this.slider.Dispose();
				this.slider = null;
			}
			
			base.Dispose(disposing);
		}
		
		// Valeur num�rique �dit�e.
		public override double Value
		{
			get
			{
				return base.Value;
			}

			set
			{
				base.Value = value;
				this.slider.Value = value;
			}
		}

		// Valeur num�rique minimale possible.
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
		
		// Valeur num�rique maximale possible.
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
			base.UpdateClientGeometry();

			if ( this.arrowUp == null )  return;

			this.margins.Bottom = this.sliderHeight-AbstractTextField.FrameMargin;
			
			Drawing.Rectangle rect = new Drawing.Rectangle(0, 0, this.Client.Width, this.Client.Height);
			rect.Width -= System.Math.Floor(rect.Height*0.6)-1;
			rect.Height = this.sliderHeight;
			this.slider.Bounds = rect;
		}

		// G�n�re un �v�nement pour dire que le texte a chang� (ajout ou suppression).
		protected override void OnTextChanged()
		{
			base.OnTextChanged();
			this.slider.Value = base.Value;
		}

		// Slider boug�.
		private void SliderValueChanged(object sender)
		{
			base.Value = this.slider.Value;
			this.OnTextChanged();
		}


		protected Slider						slider;
		protected double						sliderHeight = 5;
	}
}
