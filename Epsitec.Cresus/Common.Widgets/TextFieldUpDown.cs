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
			//this.rightMargin = System.Math.Floor(rect.Height/2+1);
			this.rightMargin = System.Math.Floor(rect.Height*0.6);

			if ( this.arrowUp != null )
			{
				Drawing.Rectangle aRect = new Drawing.Rectangle(rect.Width-this.rightMargin, rect.Height/2, this.rightMargin, rect.Height/2);
				this.arrowUp.Bounds = aRect;
			}
			if ( this.arrowDown != null )
			{
				Drawing.Rectangle aRect = new Drawing.Rectangle(rect.Width-this.rightMargin, 0, this.rightMargin, rect.Height/2);
				this.arrowDown.Bounds = aRect;
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

			text = System.Convert.ToString(number);
			this.Text = text;
			this.OnTextChanged();
			this.cursorFrom = 0;
			this.cursorTo   = text.Length;
			this.Invalidate();
		}

		// Valeur numérique éditée.
		public double Value
		{
			get
			{
				string text = this.Text;
				double number = this.minRange;
				try
				{
					number = System.Convert.ToDouble(text);
				}
				catch ( System.Exception )
				{
					number = this.minRange;
				}
				return number;
			}

			set
			{
				string text = System.Convert.ToString(value);
				this.Text = text;
				this.cursorFrom = 0;
				this.cursorTo   = text.Length;
				this.CursorScroll();
				this.Invalidate();
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
			}
		}
		
		protected ArrowButton					arrowUp;
		protected ArrowButton					arrowDown;
		protected double						minRange = 0;
		protected double						maxRange = 100;
		protected double						step = 1;
	}
}
