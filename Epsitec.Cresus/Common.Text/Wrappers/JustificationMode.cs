//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Wrappers
{
	/// <summary>
	/// Mode de justification.
	/// </summary>
	public enum JustificationMode
	{
		Default,						//	réglage par défaut
		Other,							//	réglage autre (non reconnu)
		
		AlignLeft,						//	aligné sur la marge gauche
		AlignRight,						//	aligné sur la marge droite
		
		Center,							//	centré
		
		JustifyAlignLeft,				//	justifié, dernière ligne AlignLeft
	}
}
