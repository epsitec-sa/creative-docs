//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
		
		public SimpleTextFrame(double width, double height, double gridStep)
		{
			this.width     = width;
			this.height    = height;
			this.gridStep = gridStep;
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
		
		public double							OriginX
		{
			get
			{
				return this.originX;
			}
			set
			{
				if (this.originX != value)
				{
					this.originX = value;
				}
			}
		}
		
		public double							OriginY
		{
			get
			{
				return this.originY;
			}
			set
			{
				if (this.originY != value)
				{
					this.originY = value;
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
				return this.gridStep;
			}
			set
			{
				if (this.gridStep != value)
				{
					this.gridStep = value;
				}
			}
		}
		
		public double							GridOffset
		{
			get
			{
				return this.gridOffset;
			}
			set
			{
				if (this.gridOffset != value)
				{
					this.gridOffset = value;
				}
			}
		}
		
		
		public int								PageNumber
		{
			get
			{
				return this.pageNumber;
			}
			set
			{
				if (this.pageNumber != value)
				{
					this.pageNumber = value;
				}
			}
		}
		
		
		public bool ConstrainLineBox(double yDist, double ascender, double descender, double height, double leading, bool syncToGrid, out double ox, out double oy, out double width, out double nextYDist)
		{
			//	A partir d'une position suggérée :
			//
			//	  - distance verticale depuis le sommet du cadre,
			//	  - hauteurs au-dessus/en-dessous de la ligne de base,
			//	  - hauteur de la ligne y compris interligne,
			//
			//	détermine la position exacte de la ligne ainsi que la largeur
			//	disponible et la position suggérée pour la prochaine ligne.
			
			double lineDy = leading;
			double textDy = ascender - descender;
			double filler = lineDy - textDy;
			
			double yTop = yDist;
			double yBot = yTop - lineDy;
			
			ox    = 0;
			oy    = yTop - ascender - filler / 2;
			width = this.width;
			
			if ((syncToGrid) &&
				(this.gridStep != 0))
			{
				//	Les coordonnées verticales (oy) commencent à zéro au sommet du cadre
				//	et vont vers des valeurs négatives, plus on descend. L'arrondi doit
				//	donc toujours se faire vers une valeur plus basse :
				
				oy += this.originY;
				oy  = System.Math.Floor ((oy - this.gridOffset + 0.5) / this.gridStep) * this.gridStep + this.gridOffset;
				oy -= this.originY;
				
				yTop  = oy + ascender + filler / 2;
				yBot  = yTop - lineDy;
				yDist = yTop;
			}
			
			if (yBot < -this.height)
			{
				nextYDist = yDist;
				return false;
			}
			else
			{
				nextYDist = yDist - lineDy;
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
		private double							originX, originY;
		private double							width, height;
		private int								pageNumber;
		private double							gridStep;
		private double							gridOffset;
	}
}
