//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Styles
{
	/// <summary>
	/// Summary description for RichStyle.
	/// </summary>
	public class RichStyle : BaseStyle
	{
		public RichStyle()
		{
		}
		
		
		public override bool					IsRichStyle
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
