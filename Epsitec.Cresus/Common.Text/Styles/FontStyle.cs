//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Styles
{
	/// <summary>
	/// Summary description for FontStyle.
	/// </summary>
	public class FontStyle : BaseStyle
	{
		public FontStyle()
		{
		}
		
		
		public override bool					IsSpecialStyle
		{
			get
			{
				return false;
			}
		}
		
		
		protected override int ComputeContentsSignature()
		{
			return 0;
		}
	}
}
