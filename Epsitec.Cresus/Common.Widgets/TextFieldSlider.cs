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
			this.slider.ValueChanged += new Support.EventHandler(this.HandleSliderValueChanged);
			
			this.range.Changed += new Support.EventHandler(this.HandleRangeChanged);
			this.UpdateSliderRange();
		}
		
		public TextFieldSlider(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		
		public Drawing.Color				Color
		{
			// Couleur du slider.
			get
			{
				return this.slider.Color;
			}

			set
			{
				this.slider.Color = value;
			}
		}

		public override Drawing.Color		BackColor
		{
			// Couleur de fond du slider.
			get
			{
				return this.slider.BackColor;
			}

			set
			{
				this.slider.BackColor = value;
			}
		}
		
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.slider.ValueChanged -= new Support.EventHandler(this.HandleSliderValueChanged);
				this.slider.Dispose();
				this.slider = null;
			}
			
			base.Dispose(disposing);
		}
		
		protected override void UpdateClientGeometry()
		{
			this.margins.Bottom = TextFieldSlider.sliderHeight-AbstractTextField.FrameMargin;
			
			base.UpdateClientGeometry();

			if ( this.arrowUp == null )  return;

			Drawing.Rectangle rect = this.Client.Bounds;
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

		
		private void HandleSliderValueChanged(object sender)
		{
			this.Value = this.slider.Value;
		}
		
		private void HandleRangeChanged(object sender)
		{
			this.UpdateSliderRange();
		}
		
		
		protected virtual void UpdateSliderRange()
		{
			this.slider.MinValue   = this.MinValue;
			this.slider.MaxValue   = this.MaxValue;
			this.slider.Resolution = this.Resolution;
		}

		
		protected Slider					slider;
		protected static readonly double	sliderHeight = 5;
	}
}
