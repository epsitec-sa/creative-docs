//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

#if false
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
		
		
		public override void UpdateContentsSignature(IO.IChecksum checksum)
		{
		}
		
		public override bool CompareEqualContents(object value)
		{
			return true;
		}
	}
}
#endif
