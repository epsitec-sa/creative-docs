//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Styles
{
	/// <summary>
	/// Summary description for SpecialStyle.
	/// </summary>
	public class SpecialStyle : BaseStyle
	{
		public SpecialStyle()
		{
		}
		
		
		public override bool					IsSpecialStyle
		{
			get
			{
				return true;
			}
		}
		
		
		protected override int ComputeContentsSignature()
		{
			return 0;
		}
	}
}
