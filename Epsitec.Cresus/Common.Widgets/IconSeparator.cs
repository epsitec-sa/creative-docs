namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe IconSeparator permet de dessiner des séparations utiles
	/// pour remplir une ToolBar.
	/// </summary>
	public class IconSeparator : Widget
	{
		public IconSeparator()
		{
		}
		
		public IconSeparator(double breadth)
		{
			this.Breadth = breadth;
		}
		
		
		public override double		DefaultWidth
		{
			// Retourne la largeur standard d'une séparation.
			get
			{
				return this.is_horizontal ? this.breadth : 22;
			}
		}

		public override double		DefaultHeight
		{
			// Retourne la hauteur standard d'une séparation.
			get
			{
				return this.is_horizontal ? 22 : this.breadth;
			}
		}

		
		public bool					IsHorizontal
		{
			get { return this.is_horizontal; }
			set
			{
				if (this.is_horizontal != value)
				{
					this.is_horizontal = value;
				}
			}
		}
		
		public double				Breadth
		{
			get
			{
				return this.breadth;
			}

			set
			{
				if (this.breadth != value)
				{
					this.breadth = value;
					this.UpdateGeometry ();
				}
			}
		}
		
		
		protected void UpdateGeometry()
		{
			Drawing.Rectangle bounds = new Drawing.Rectangle (0, 0, this.DefaultWidth, this.DefaultHeight);
			this.Bounds = this.MapClientToParent (bounds);
		}

		
		protected double				breadth = 5;
		protected bool					is_horizontal = true;
	}
}
