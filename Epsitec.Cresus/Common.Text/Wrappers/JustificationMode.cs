//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Wrappers
{
	/// <summary>
	/// Mode de justification.
	/// </summary>
	public enum JustificationMode
	{
		Unknown,						//	réglage inconnu
		
		AlignLeft,						//	aligné sur la marge gauche
		AlignRight,						//	aligné sur la marge droite
		
		Center,							//	centré
		
		JustifyAlignLeft,				//	justifié, dernière ligne AlignLeft
		JustifyCenter,					//	justifié, dernière ligne Center
		JustifyAlignRight,				//	justifié, dernière ligne AlignRight
		JustifyJustfy					//	justifié, dernière ligne Justify
	}
}
