//	Copyright � 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Properties
{
	/// <summary>
	/// L'�num�ration WellKnownType liste les propri�t�s les plus connues
	/// afin d'�viter de devoir passer par Object.GetType pour d�terminer leur
	/// type. L'�num�ration sert aussi de crit�re de tri pour les propri�t�s.
	/// </summary>
	public enum WellKnownType
	{
		Font				= 1,		//	style
		FontSize			= 2,		//	style
		FontKern			= 3,		//	local
		FontOffset			= 4,		//	local
		FontBold			= 5,		//	style
		FontItalic			= 6,		//	style
		
		Conditional			= 10,		//	style
		Keep				= 11,		//	style
		Layout				= 12,		//	style
		Leading				= 13,		//	style
		Margins				= 14,		//	style
		
		ManagedParagraph	= 30,		//	style
		
		Properties			= 50,		//	style
		Styles				= 51,		//	style
		Tabs				= 52,		//	style
		
		Color				= 100,		//	extra
		Link				= 101,		//	extra
		Meta				= 102,		//	extra
		Underline			= 103,		//	extra
		
		AutoText			= 150,		//	extra
		Generator			= 151,		//	extra
		Language			= 152,		//	extra
		
		Image				= 200,		//	local
		OpenType			= 201,		//	local
		
		Break				= 220,		//	local
		Tab					= 221,		//	local
		
		Other				= 1000000,
	}
}
