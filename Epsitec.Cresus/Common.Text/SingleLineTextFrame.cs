//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// La classe SingleLineTextFrame permet de décrire une ligne unique dans laquelle
	/// coule du texte.
	/// </summary>
	public class SingleLineTextFrame : ITextFrame
	{
		public SingleLineTextFrame()
		{
		}
		
		public SingleLineTextFrame(double width)
		{
			this.width  = width;
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

			ox    = 0;
			oy    = 0;
			width = this.width;
			next_y_dist = -1;

			if ( y_dist == 0 )  // première ligne ?
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		
		
		public void MapToView(ref double x, ref double y)
		{
		}
		
		public void MapFromView(ref double x, ref double y)
		{
		}
		
		
		private double							width;
		private int								page_number;
	}
}
