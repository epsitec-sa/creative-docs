namespace Epsitec.Common.Types
{
	[DesignerVisible]
	public enum YesOrNo
	{
		Yes,
		No,
	}

	public static class YesOrNoUtils
	{
		public static YesOrNo ToYesOrNo(this bool value)
		{
			return value
				? YesOrNo.Yes
				: YesOrNo.No;
		}
	}
}
