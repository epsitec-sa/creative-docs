//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 02/02/2004

namespace Epsitec.Common.Widgets.Layouts
{
	public class LayoutInfo
	{
		public LayoutInfo(double width, double height)
		{
			this.width  = width;
			this.height = height;
		}
		
		
		public double							OriginalWidth
		{
			get { return this.width; }
		}
		
		public double							OriginalHeight
		{
			get { return this.height; }
		}
		
		
		private double							width, height;
	}
}
