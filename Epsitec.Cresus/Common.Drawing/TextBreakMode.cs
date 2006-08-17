//	Copyright © 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Drawing
{
	[System.Flags]
	public enum TextBreakMode
	{
		None		= 0x0000,

		Hyphenate	= 0x0001,		//	césure des mots, si possible
		Ellipsis	= 0x0002,		//	ajoute une ellipse (...) si le dernier mot est tronqué
		Overhang	= 0x0004,		//	permet de dépasser la largeur si on ne peut pas faire autrement
		Split		= 0x0008,		//	coupe brutalement si on ne peut pas faire autrement

		SingleLine	= 0x0100,		//	force tout sur une ligne (utile avec Ellipsis, Overhang et Split)
	}
}
