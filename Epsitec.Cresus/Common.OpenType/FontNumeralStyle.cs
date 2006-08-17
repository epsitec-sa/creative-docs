//	Copyright © 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.OpenType
{
	/// <summary>
	/// The <c>FontNumeralStyle</c> enumeration specifies how digits should
	/// be used in a font (normal, lining or old style).
	/// </summary>
	public enum FontNumeralStyle
	{
		Normal,						//	default behavior
		
		Lining,						//	digits with same height; aligned with upper case letters
		OldStyle,					//	old style digits; aligned with lower case letters
	}
}
