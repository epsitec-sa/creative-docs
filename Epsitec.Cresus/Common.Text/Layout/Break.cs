//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Layout
{
	/// <summary>
	/// La classe Break stocke des informations sur un point de découpe dans la
	/// ligne.
	/// </summary>
	public class Break
	{
		public Break(int offset, double advance, double penalty, StretchProfile profile)
		{
			this.offset  = offset;
			this.advance = advance;
			this.penalty = penalty;
			this.profile = new StretchProfile (profile);
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
		
		public double							Penalty
		{
			get
			{
				return this.penalty;
			}
		}
		
		public StretchProfile					Profile
		{
			get
			{
				return this.profile;
			}
		}
		
		
		private int								offset;
		private double							advance;
		private double							penalty;
		private StretchProfile					profile;
	}
}
