//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Properties
{
	/// <summary>
	/// L'énumération WellKnownType liste les propriétés les plus connues
	/// afin d'éviter de devoir passer par Object.GetType pour déterminer leur
	/// type. L'énumération sert aussi de critère de tri pour les propriétés.
	/// </summary>
	public enum WellKnownType
	{
		Font			= 1,			//	style
		FontSize,						//	style
		
		Keep,							//	style
		Layout,							//	style
		Leading,						//	style
		Margins,						//	style
		
		Properties,						//	style
		Styles,							//	style
		
		Color,							//	extra
		Underline,						//	extra
		
		AutoText,						//	extra
		Generator,						//	extra
		Language,						//	extra
		Tab,							//	extra
		
		OpenType,						//	local
		Image,							//	local
		
		Other			= 1000000,
	}
}
