namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe TextFieldUpDown implémente la ligne éditable numérique.
	/// </summary>
	public class TextFieldUpDown : AbstractTextField
	{
		public TextFieldUpDown()
		{
			this.textStyle = TextFieldStyle.UpDown;

			this.arrowUp = new ArrowButton(this);
			this.arrowDown = new ArrowButton(this);
			this.arrowUp.Name = "Up";
			this.arrowDown.Name = "Down";
			this.arrowUp.GlyphType = GlyphType.ArrowUp;
			this.arrowDown.GlyphType = GlyphType.ArrowDown;
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
					if ( message.Wheel > 0 )  this.IncrementValue(this.step);
					if ( message.Wheel < 0 )  this.IncrementValue(-this.step);
					message.Consumer = this;
					return;
			}

			base.ProcessMessage(message, pos);
		}

		protected override bool ProcessKeyDown(KeyCode key, bool isShiftPressed, bool isCtrlPressed)
		{
			switch ( key )
			{
				case KeyCode.ArrowUp:
					this.IncrementValue(this.step);
					break;

				case KeyCode.ArrowDown:
					this.IncrementValue(-this.step);
					break;

				default:
					return base.ProcessKeyDown(key, isShiftPressed, isCtrlPressed);
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
		
		protected virtual void OnValueChanged()
		{
			if ( this.ValueChanged != null )  // qq'un écoute ?
			{
				this.ValueChanged(this);
			}
		}
		
		private void HandleButton(object sender)
		{
			ArrowButton button = sender as ArrowButton;

			if ( button == this.arrowUp )
			{
				this.IncrementValue(this.step);
			}
			if ( button == this.arrowDown )
			{
				this.IncrementValue(-this.step);
			}
		}

		protected void IncrementValue(double dir)
		{
			string text = this.Text;
			double number;
			try
			{
				number = System.Convert.ToDouble(text);
			}
			catch ( System.Exception )
			{
				return;
			}

			number += dir;
			number = System.Math.Max(number, this.minRange);
			number = System.Math.Min(number, this.maxRange);

			this.Value = number;
		}

		// Valeur numérique éditée.
		public virtual double Value
		{
			get
			{
				string text = this.Text;
				double number = this.minRange;
				
				if ( text != "" )
				{
					try
					{
						number = System.Convert.ToDouble(text);
					}
					catch ( System.Exception )
					{
						number = this.minRange;
					}
				}
				
				return number;
			}

			set
			{
				if ( this.Value != value || this.Text == "" )
				{
					if ( value%1.0 == 0.0 )
					{
						this.Text = System.Convert.ToString(value);
					}
					else
					{
						this.Text = value.ToString("F2");
					}
					this.SelectAll();
				}
			}
		}

		// Valeur numérique minimale possible.
		public virtual double MinRange
		{
			get { return this.minRange; }
			set { this.minRange = value; }
		}
		
		// Valeur numérique maximale possible.
		public virtual double MaxRange
		{
			get { return this.maxRange; }
			set { this.maxRange = value; }
		}
		
		// Pas pour les boutons up/down.
		public virtual double Step
		{
			get { return this.step; }
			set { this.step = value; }
		}
		
		public event Support.EventHandler		ValueChanged;
		
		protected ArrowButton					arrowUp;
		protected ArrowButton					arrowDown;
		protected double						minRange = 0;
		protected double						maxRange = 100;
		protected double						step = 1;
	}
}
