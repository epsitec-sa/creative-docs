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
		public Break(int offset, double advance, double space_penalty, double break_penalty, StretchProfile profile)
		{
			this.offset        = offset;
			this.advance       = advance;
			this.space_penalty = space_penalty;
			this.break_penalty = break_penalty;
			this.profile       = new StretchProfile (profile);
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
		
		public double							SpacePenalty
		{
			get
			{
				return this.space_penalty;
			}
		}
		
		public double							BreakPenalty
		{
			get
			{
				return this.break_penalty;
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
		private double							space_penalty;
		private double							break_penalty;
		private StretchProfile					profile;
	}
}
