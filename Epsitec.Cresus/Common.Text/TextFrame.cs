//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// La classe TextFrame permet de décrire une zone dans laquelle coule du
	/// texte.
	/// </summary>
	public class TextFrame : ITextFrame
	{
		public TextFrame()
		{
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
		
		
		private double							x, y;
		private double							width, height;
		private int								page_number;
	}
}
