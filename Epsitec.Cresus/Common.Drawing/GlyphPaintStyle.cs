//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Drawing
{
	public enum GlyphPaintStyle
	{
		Invalid		= -1,
		
		Normal		= 0,	//	texte/icône peints normalement
		Disabled	= 1,	//	texte/icône grisés
		Selected	= 2,	//	texte/icône sélectionnés (couleurs "inversées")
		Entered		= 3		//	texte/icône survolés par la souris
	}
}
