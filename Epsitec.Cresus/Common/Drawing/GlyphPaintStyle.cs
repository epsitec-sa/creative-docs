//	Copyright � 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Drawing
{
	public enum GlyphPaintStyle
	{
		Invalid		= -1,
		
		Normal		= 0,	//	texte/ic�ne peints normalement
		Disabled	= 1,	//	texte/ic�ne gris�s
		Selected	= 2,	//	texte/ic�ne s�lectionn�s (couleurs "invers�es")
		Entered		= 3,	//	texte/ic�ne survol�s par la souris
		Shadow		= 4,	//	texte/ic�ne ombre sp�ciale pour certains adorners
	}
}
