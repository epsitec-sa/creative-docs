namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe TextFieldUpDown implémente la ligne éditable numérique.
	/// </summary>
	public class TextFieldUpDown : AbstractTextField, Support.INumValue
	{
		public TextFieldUpDown()
		{
			this.textFieldStyle = TextFieldStyle.UpDown;
			this.TextNavigator.IsNumeric = true;
			this.range = new Epsitec.Common.Converters.DecimalRange(0, 100, 1);

			this.arrowUp = new GlyphButton(this);
			this.arrowDown = new GlyphButton(this);
			this.arrowUp.Name = "Up";
			this.arrowDown.Name = "Down";
			this.arrowUp.GlyphShape = GlyphShape.ArrowUp;
			this.arrowDown.GlyphShape = GlyphShape.ArrowDown;
			this.arrowUp.ButtonStyle = ButtonStyle.UpDown;
			this.arrowDown.ButtonStyle = ButtonStyle.UpDown;
			this.arrowUp.Engaged += new Support.EventHandler(this.HandleButton);
			this.arrowDown.Engaged += new Support.EventHandler(this.HandleButton);
			this.arrowUp.StillEngaged += new Support.EventHandler(this.HandleButton);
			this.arrowDown.StillEngaged += new Support.EventHandler(this.HandleButton);
			this.arrowUp.AutoRepeatEngaged = true;
			this.arrowDown.AutoRepeatEngaged = true;
		}
		
		public TextFieldUpDown(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		
		#region INumValue Members
		public virtual decimal				Value
		{
			get
			{
				string  text  = this.Text;
				decimal value = this.DefaultValue;
				
				if ( text != "" )
				{
					try
					{
						value = System.Convert.ToDecimal(text);
					}
					catch
					{
						//	ignore l'erreur
					}
				}
				
				return value;
			}
			set
			{
				value = this.range.Constrain (value);
				
				if ( this.Text == "" || this.Value != value )
				{
					this.Text = this.range.ConvertToString(value);
					this.SelectAll();
				}
			}
		}

		public decimal						MinValue
		{
			get
			{
				return this.range.Minimum;
			}
			set
			{
				this.range.Minimum = value;
			}
		}
		
		public decimal						MaxValue
		{
			get
			{
				return this.range.Maximum;
			}
			set
			{
				this.range.Maximum = value;
			}
		}

		public decimal						Resolution
		{
			get
			{
				return this.range.Resolution;
			}
			set
			{
				if ( this.range.Resolution != value )
				{
					this.range.Resolution = value;
					this.Text = this.range.Constrain(this.Value).ToString();
					this.SelectAll();
				}
			}
		}

		public decimal Range
		{
			get
			{
				return this.MaxValue-this.MinValue;
			}
		}
		
		public event Support.EventHandler		ValueChanged;
		#endregion
		
		public virtual decimal					DefaultValue
		{
			get
			{
				return this.defaultValue;
			}
			set
			{
				this.defaultValue = value;
			}
		}
		
		public virtual decimal					Step
		{
			get
			{
				return this.step;
			}
			set
			{
				this.step = value;
			}
		}
		
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.arrowUp.Engaged -= new Support.EventHandler(this.HandleButton);
				this.arrowDown.Engaged -= new Support.EventHandler(this.HandleButton);
				this.arrowUp.StillEngaged -= new Support.EventHandler(this.HandleButton);
				this.arrowDown.StillEngaged -= new Support.EventHandler(this.HandleButton);
				
				this.arrowUp.Dispose();
				this.arrowDown.Dispose();
				this.arrowUp = null;
				this.arrowDown = null;
			}
			
			base.Dispose(disposing);
		}
		
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();
			
			Drawing.Rectangle rect = this.Bounds;
			double width = System.Math.Floor(rect.Height*0.6);
			this.margins.Right = width - AbstractTextField.FrameMargin;

			if ( this.arrowUp   != null &&
				 this.arrowDown != null )
			{
				Drawing.Rectangle aRect = new Drawing.Rectangle();

				aRect.Left   = rect.Width-width;
				aRect.Width  = width;

				aRect.Bottom = 0;
				aRect.Height = System.Math.Floor(rect.Height/2)+1;
				this.arrowDown.Bounds = aRect;

				aRect.Bottom += aRect.Height-1;
				aRect.Top    = rect.Height;
				this.arrowUp.Bounds = aRect;
			}
		}

		
		protected override void ProcessMessage(Message message, Drawing.Point pos)
		{
			switch ( message.Type )
			{
				case MessageType.MouseWheel:
					if ( message.Wheel > 0 )  this.IncrementValue(1);
					if ( message.Wheel < 0 )  this.IncrementValue(-1);
					message.Consumer = this;
					return;
			}

			base.ProcessMessage(message, pos);
		}

		protected override bool ProcessKeyDown(Message message, Drawing.Point pos)
		{
			switch ( message.KeyCode )
			{
				case KeyCode.ArrowUp:
					this.IncrementValue(1);
					break;

				case KeyCode.ArrowDown:
					this.IncrementValue(-1);
					break;

				default:
					return base.ProcessKeyDown(message, pos);
			}
			
			return true;
		}
		
		
		protected override void OnAdornerChanged()
		{
			this.UpdateClientGeometry();
			base.OnAdornerChanged();
		}

		protected override void OnTextChanged()
		{
			base.OnTextChanged();
			this.OnValueChanged();
		}
		
		protected virtual  void OnValueChanged()
		{
			if ( this.ValueChanged != null )  // qq'un écoute ?
			{
				this.ValueChanged(this);
			}
		}
		
		
		protected virtual void IncrementValue(decimal delta)
		{
			Converters.DecimalRange range = new Epsitec.Common.Converters.DecimalRange (this.MinValue, this.MaxValue, this.Step);
			
			decimal orgValue   = this.Value;
			decimal roundValue = range.ConstrainToZero(orgValue);
			
			if ( orgValue == roundValue )
			{
				// La valeur d'origine était déjà parfaitement alignée sur une frontière (step),
				// on peut donc simplement passer au pas suivant :
				roundValue += delta * this.Step;
			}
			else
			{
				// L'arrondi vers zéro suffit dans les cas suivants :
				//  o  13 =>  10,  13 - 10 =>  10   orgValue > 0, delta < 0
				//  o -13 => -10, -13 + 10 => -10   orgValue < 0, delta > 0
				// en supposant un pas de 10.
				
				if ( (orgValue < 0 && delta > 0) ||
					 (orgValue > 0 && delta < 0) )
				{
					// La valeur arrondie fait l'affaire.
				}
				else
				{
					// Il faut encore ajouter l'incrément.
					roundValue += delta * this.Step;
				}
			}
			
			this.Value = roundValue;
		}
		
		
		private void HandleButton(object sender)
		{
			if ( sender == this.arrowUp )
			{
				this.IncrementValue(1);
			}
			else if ( sender == this.arrowDown )
			{
				this.IncrementValue(-1);
			}
		}

		
		protected Converters.DecimalRange		range;
		
		protected GlyphButton					arrowUp;
		protected GlyphButton					arrowDown;
		protected decimal						defaultValue = 0;
		protected decimal						step = 1;
	}
}
