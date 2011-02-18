//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
		
		Font				= 1,		//	core
		FontSize			= 2,		//	core
		FontKern			= 3,		//	local
		FontOffset			= 4,		//	local
		FontXscript			= 5,		//	core
		FontColor			= 6,		//	extra
		
		Conditional			= 10,		//	core
		Keep				= 11,		//	core
		Layout				= 12,		//	core
		Leading				= 13,		//	core
		Margins				= 14,		//	core
		
		ManagedParagraph	= 30,		//	core
		ManagedInfo			= 31,		//	extra
		
		Properties			= 50,		//	core
		Styles				= 51,		//	polymorph
		Tabs				= 52,		//	extra
		
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
