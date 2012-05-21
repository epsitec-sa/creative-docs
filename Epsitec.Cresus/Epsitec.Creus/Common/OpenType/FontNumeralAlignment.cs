//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.OpenType
{
	/// <summary>
	/// The <c>FontNumeralAlignment</c> enumeration specifies which
	/// digits should be used in a font (normal, proportional or
	/// tabular).
	/// </summary>
	public enum FontNumeralAlignment
	{
		Normal,						//	default behavior
		
		Proportional,				//	variable width digits
		Tabular,					//	fixed width digits
	}
}
