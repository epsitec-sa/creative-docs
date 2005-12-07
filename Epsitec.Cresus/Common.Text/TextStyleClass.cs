//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// L'énumération TextStyleClass définit les catégories auxquelles les
	/// styles TextStyle peuvent appartenir.
	/// </summary>
	public enum TextStyleClass
	{
		Invalid			= 0,			//	pas valide
		
		Abstract		= 1,			//	style abstrait, sert uniquement de base aux autres
		Paragraph		= 2,			//	style de paragraphe, appliqué au paragraphe entier
		Text			= 3,			//	style de texte, appliqué à un passage de texte local
		Symbol			= 4,			//	style de symboles, appliqué à un caractère unique
		
		MetaProperty	= 5,			//	style se comportant comme une propriété
	}
}
