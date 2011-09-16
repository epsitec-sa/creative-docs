//	Copyright © 2004-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	[System.Flags]
	public enum CellArrayStyles
	{
		None			= 0x00000000,		// neutre
		ScrollNorm		= 0x00000001,		// défilement avec un ascenseur
		ScrollMagic		= 0x00000002,		// défilement aux extrémités
		Stretch			= 0x00000004,		// occupe toute la place
		Header			= 0x00000010,		// en-tête
		Mobile			= 0x00000020,		// dimensions mobiles
		Separator		= 0x00000040,		// lignes de séparation
		Sort			= 0x00000080,		// choix pour tri possible
		SelectCell		= 0x00000100,		// sélectionne une cellule individuelle
		SelectLine		= 0x00000200,		// sélectionne toute la ligne
		SelectMulti		= 0x00000400,		// sélections multiples possibles avec Ctrl et Shift
	}
}
