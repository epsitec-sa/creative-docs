//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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

			ox    = 0;
			oy    = 0;
			width = this.width;
			nextYDist = -1;

			if ( yDist == 0 )  // première ligne ?
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
		private int								pageNumber;
	}
}
