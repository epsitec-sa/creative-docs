//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Styles
{
	/// <summary>
	/// Summary description for SimpleStyle.
	/// </summary>
	internal sealed class SimpleStyle : BaseStyle
	{
		public SimpleStyle()
		{
		}
		
		
		public override bool					IsRichStyle
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
