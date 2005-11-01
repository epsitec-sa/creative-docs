//	Copyright � 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Wrappers
{
	/// <summary>
	/// Mode de justification.
	/// </summary>
	public enum JustificationMode
	{
		Default,						//	r�glage par d�faut
		Other,							//	r�glage autre (non reconnu)
		
		AlignLeft,						//	align� sur la marge gauche
		AlignRight,						//	align� sur la marge droite
		
		Center,							//	centr�
		
		JustifyAlignLeft,				//	justifi�, derni�re ligne AlignLeft
	}
}
