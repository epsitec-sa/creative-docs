//	Copyright � 2005-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// L'�num�ration TabPositionMode d�finit comment la position d'un tabulateur
	/// est d�finie.
	/// </summary>
	public enum TabPositionMode
	{
		Absolute				= 0,			//	position absolue
		AbsoluteIndent			= 1,			//	position absolue, indente toutes les lignes
		
		LeftRelative			= 2,			//	position relative � la marge de gauche
		LeftRelativeIndent		= 3,			//	position relative, indente toutes les lignes
		
		Force					= 4,			//	position absolue (sans changement de ligne)
		ForceIndent				= 5,			//	position absolue, indente toutes les lignes
	}
}
