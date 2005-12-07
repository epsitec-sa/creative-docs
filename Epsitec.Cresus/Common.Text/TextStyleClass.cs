//	Copyright � 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// L'�num�ration TextStyleClass d�finit les cat�gories auxquelles les
	/// styles TextStyle peuvent appartenir.
	/// </summary>
	public enum TextStyleClass
	{
		Invalid			= 0,			//	pas valide
		
		Abstract		= 1,			//	style abstrait, sert uniquement de base aux autres
		Paragraph		= 2,			//	style de paragraphe, appliqu� au paragraphe entier
		Text			= 3,			//	style de texte, appliqu� � un passage de texte local
		Symbol			= 4,			//	style de symboles, appliqu� � un caract�re unique
		
		MetaProperty	= 5,			//	style se comportant comme une propri�t�
	}
}
