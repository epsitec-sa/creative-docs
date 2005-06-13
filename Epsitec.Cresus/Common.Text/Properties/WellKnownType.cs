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
