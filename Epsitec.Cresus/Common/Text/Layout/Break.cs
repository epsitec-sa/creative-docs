//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Layout
{
	/// <summary>
	/// La classe Break stocke des informations sur un point de découpe dans la
	/// ligne.
	/// </summary>
	public class Break
	{
		public Break(int offset, double advance, double spacePenalty, double breakPenalty, StretchProfile profile)
		{
			this.offset        = offset;
			this.advance       = advance;
			this.spacePenalty  = spacePenalty;
			this.breakPenalty  = breakPenalty;
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
				return this.spacePenalty;
			}
		}
		
		public double							BreakPenalty
		{
			get
			{
				return this.breakPenalty;
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
		private double							spacePenalty;
		private double							breakPenalty;
		private StretchProfile					profile;
	}
}
