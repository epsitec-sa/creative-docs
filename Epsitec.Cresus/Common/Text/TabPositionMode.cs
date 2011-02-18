//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// L'énumération TabPositionMode définit comment la position d'un tabulateur
	/// est définie.
	/// </summary>
	public enum TabPositionMode
	{
		Absolute				= 0,			//	position absolue
		AbsoluteIndent			= 1,			//	position absolue, indente toutes les lignes
		
		LeftRelative			= 2,			//	position relative à la marge de gauche
		LeftRelativeIndent		= 3,			//	position relative, indente toutes les lignes
		
		Force					= 4,			//	position absolue (sans changement de ligne)
		ForceIndent				= 5,			//	position absolue, indente toutes les lignes
	}
}
