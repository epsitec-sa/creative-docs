//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// La classe SimpleTextFrame permet de décrire une zone dans laquelle
	/// coule du texte.
	/// </summary>
	public class SimpleTextFrame : ITextFrame
	{
		public SimpleTextFrame()
		{
		}
		
		public SimpleTextFrame(double width, double height)
		{
			this.width  = width;
			this.height = height;
		}
		
		public SimpleTextFrame(double width, double height, double grid_step)
		{
			this.width     = width;
			this.height    = height;
			this.grid_step = grid_step;
		}
		
		
		public double							X
		{
			get
			{
				return this.x;
			}
			set
			{
				if (this.x != value)
				{
					this.x = value;
				}
			}
		}
		
		public double							Y
		{
			get
			{
				return this.y;
			}
			set
			{
				if (this.y != value)
				{
					this.y = value;
				}
			}
		}
		
		public double							Width
		{
			get
			{
				return this.width;
			}
			set
			{
				if (this.width != value)
				{
					this.width = value;
				}
			}
		}
		
		public double							Height
		{
			get
			{
				return this.height;
			}
			set
			{
				if (this.height != value)
				{
					this.height = value;
				}
			}
		}
		
		public double							GridStep
		{
			get
			{
				return this.grid_step;
			}
			set
			{
				if (this.grid_step != value)
				{
					this.grid_step = value;
				}
			}
		}
		
		
		public int								PageNumber
		{
			get
			{
				return this.page_number;
			}
			set
			{
				if (this.page_number != value)
				{
					this.page_number = value;
				}
			}
		}
		
		
		public bool ConstrainLineBox(double y_dist, double ascender, double descender, double height, double leading, bool sync_to_grid, out double ox, out double oy, out double width, out double next_y_dist)
		{
			//	A partir d'une position suggérée :
			//
			//	  - distance verticale depuis le sommet du cadre,
			//	  - hauteurs au-dessus/en-dessous de la ligne de base,
			//	  - hauteur de la ligne y compris interligne,
			//
			//	détermine la position exacte de la ligne ainsi que la largeur
			//	disponible et la position suggérée pour la prochaine ligne.
			
			double line_dy = leading;
			double text_dy = ascender - descender;
			double filler  = line_dy - text_dy;
			
			double y_top = y_dist;
			double y_bot = y_top - line_dy;
			
			ox    = 0;
			oy    = y_top - ascender - filler / 2;
			width = this.width;
			
			if ((sync_to_grid) &&
				(this.grid_step != 0))
			{
				//	Les coordonnées verticales (oy) commencent à zéro au sommet du cadre
				//	et vont vers des valeurs négatives, plus on descend. L'arrondi doit
				//	donc toujours se faire vers une valeur plus basse :
				
				oy = System.Math.Floor ((oy + 0.5) / this.grid_step) * this.grid_step;
				
				y_top  = oy + ascender + filler / 2;
				y_bot  = y_top - line_dy;
				y_dist = y_top;
			}
			
			if (y_bot < -this.height)
			{
				next_y_dist = y_dist;
				return false;
			}
			else
			{
				next_y_dist = y_dist - line_dy;
				return true;
			}
		}
		
		
		public void MapToView(ref double x, ref double y)
		{
			x += this.x;
			y += this.y + this.height;
		}
		
		public void MapFromView(ref double x, ref double y)
		{
			x -= this.x;
			y -= this.y + this.height;
		}
		
		
		private double							x, y;
		private double							width, height;
		private int								page_number;
		private double							grid_step;
	}
}
