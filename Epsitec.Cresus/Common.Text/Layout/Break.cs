//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Layout
{
	/// <summary>
	/// Summary description for Break.
	/// </summary>
	public class Break
	{
		public Break(int offset, double advance)
		{
			this.offset  = offset;
			this.advance = advance;
		}
		
		
		public int								Offset
		{
			get
			{
				return this.offset;
			}
		}
		
		public double							Advance
		{
			get
			{
				return this.advance;
			}
		}
		
		
		private int								offset;
		private double							advance;
	}
}
