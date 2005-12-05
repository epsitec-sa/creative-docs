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
		Undefined			= 0,
		
		Font				= 1,		//	style
		FontSize			= 2,		//	style
		FontKern			= 3,		//	local
		FontOffset			= 4,		//	local
		FontXscript			= 5,		//	style
		FontColor			= 6,		//	extra
		
		Conditional			= 10,		//	style
		Keep				= 11,		//	style
		Layout				= 12,		//	style
		Leading				= 13,		//	style
		Margins				= 14,		//	style
		
		ManagedParagraph	= 30,		//	style
		
		Properties			= 50,		//	style
		Styles				= 51,		//	polymorph
		Tabs				= 52,		//	local
		
		Link				= 101,		//	extra
		UserTag				= 102,		//	extra
		
		Underline			= 110,		//	extra
		Strikeout			= 111,		//	extra
		Overline			= 112,		//	extra
		TextBox				= 113,		//	extra
		TextMarker			= 114,		//	extra
		
		AutoText			= 150,		//	extra
		Generator			= 151,		//	extra
		Language			= 152,		//	extra
		
		Image				= 200,		//	local
		OpenType			= 201,		//	local
		
		Break				= 220,		//	local
		Tab					= 221,		//	local
		
		TotalCount,
		
		Other				= 1000000,
	}
}
