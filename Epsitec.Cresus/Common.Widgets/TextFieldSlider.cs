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
			//	Couleur du slider.
			get
			{
				return this.slider.Color;
			}

			set
			{
				this.slider.Color = value;
			}
		}
		
		static TextFieldSlider()
		{
			//	Toute modification de la propriété BackColor doit être répercutée
			//	sur le slider. Le plus simple est d'utiliser un override callback
			//	sur la propriété BackColor :
			
			Helpers.VisualPropertyMetadata metadata = new Helpers.VisualPropertyMetadata (Drawing.Color.Empty, Helpers.VisualPropertyMetadataOptions.AffectsDisplay);
			
			metadata.SetValueOverride = new Epsitec.Common.Types.SetValueOverrideCallback (TextFieldSlider.SetBackColorValue);
			
			Visual.BackColorProperty.OverrideMetadata (typeof (TextFieldSlider), metadata);
		}
		
		
		private static void SetBackColorValue(Types.DependencyObject o, object value)
		{
			TextFieldSlider that = o as TextFieldSlider;
			that.slider.BackColor = (Drawing.Color) value;
			that.SetValueBase (Visual.BackColorProperty, value);
		}

		
		public decimal						Logarithmic
		{
			get
			{
				return this.slider.Logarithmic;
			}

			set
			{
				this.slider.Logarithmic = value;
			}
		}

		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				if ( this.slider != null )
				{
					this.slider.ValueChanged -= new Support.EventHandler(this.HandleSliderValueChanged);
					this.slider.Dispose();
				}
				this.slider = null;
			}
			
			base.Dispose(disposing);
		}
		
		protected override void UpdateGeometry()
		{
			base.UpdateGeometry ();

			if ( this.arrowUp == null )  return;

			IAdorner adorner = Widgets.Adorners.Factory.Active;
			Drawing.Rectangle rect = this.Client.Bounds;
			rect.Width -= System.Math.Floor(rect.Height*adorner.GeometryUpDownWidthFactor)-1;
			rect.Height = TextFieldSlider.sliderHeight;
			this.slider.SetManualBounds(rect);
		}

		protected override void OnValueChanged()
		{
			//	Valeur numérique éditée.
			base.OnValueChanged ();
			
			if (this.Text != "")
			{
				this.slider.Value = this.Value;
			}
		}

		protected override void InitializeMargins()
		{
			base.InitializeMargins ();
			this.margins.Bottom = TextFieldSlider.sliderHeight-AbstractTextField.FrameMargin;
		}

		
		private void HandleSliderValueChanged(object sender)
		{
			this.SetValue(this.slider.Value);
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
