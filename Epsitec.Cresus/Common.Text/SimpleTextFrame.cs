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
		
		
		public bool ConstrainLineBox(double y_dist, double ascender, double descender, double height, out double ox, out double oy, out double width, out double next_y_dist)
		{
			double line_dy = height;
			double text_dy = ascender - descender;
			double leading = line_dy - text_dy;
			
			double y_top = y_dist;
			double y_bot = y_top - line_dy;
			
			ox    = 0;
			oy    = y_top - ascender - leading / 2;
			width = this.width;
			
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
		
		
		private double							x, y;
		private double							width, height;
		private int								page_number;
	}
}
