namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe TextFieldUpDown impl�mente la ligne �ditable num�rique.
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
			this.arrowUp.Direction = Direction.Up;
			this.arrowDown.Direction = Direction.Down;
			this.arrowUp.ButtonStyle = ButtonStyle.UpDown;
			this.arrowDown.ButtonStyle = ButtonStyle.UpDown;
			this.arrowUp.Engaged += new EventHandler(this.HandleButton);
			this.arrowDown.Engaged += new EventHandler(this.HandleButton);
			this.arrowUp.StillEngaged += new EventHandler(this.HandleButton);
			this.arrowDown.StillEngaged += new EventHandler(this.HandleButton);
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
				this.arrowUp.Engaged -= new EventHandler(this.HandleButton);
				this.arrowDown.Engaged -= new EventHandler(this.HandleButton);
				this.arrowUp.StillEngaged -= new EventHandler(this.HandleButton);
				this.arrowDown.StillEngaged -= new EventHandler(this.HandleButton);
				
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

		protected override void OnAdornerChanged()
		{
			this.UpdateClientGeometry();
			base.OnAdornerChanged();
		}

		protected override void OnTextChanged()
		{
			base.OnTextChanged ();
			this.OnValueChanged ();
		}
		
		protected virtual void OnValueChanged()
		{
			if ( this.ValueChanged != null )  // qq'un �coute ?
			{
				this.ValueChanged(this);
			}
		}
		
		private void HandleButton(object sender)
		{
			ArrowButton button = sender as ArrowButton;

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

			if ( button == this.arrowUp )
			{
				number += this.step;
			}
			else if ( button == this.arrowDown )
			{
				number -= this.step;
			}
			number = System.Math.Max(number, this.minRange);
			number = System.Math.Min(number, this.maxRange);

			this.Value = number;
		}

		// Valeur num�rique �dit�e.
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
				if ((this.Value != value) ||
					(this.Text == ""))
				{
					this.Text = System.Convert.ToString(value);
					this.SelectAll();
				}
			}
		}

		// Valeur num�rique minimale possible.
		public virtual double MinRange
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
		
		// Valeur num�rique maximale possible.
		public virtual double MaxRange
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
		
		// Pas pour les boutons up/down.
		public virtual double Step
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
		
		public event EventHandler				ValueChanged;
		
		protected ArrowButton					arrowUp;
		protected ArrowButton					arrowDown;
		protected double						minRange = 0;
		protected double						maxRange = 100;
		protected double						step = 1;
	}
}
