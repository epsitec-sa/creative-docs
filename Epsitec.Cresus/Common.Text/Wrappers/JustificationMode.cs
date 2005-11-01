//	Copyright � 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Wrappers
{
	/// <summary>
	/// Mode de justification.
	/// </summary>
	public enum JustificationMode
	{
		Unknown,						//	r�glage inconnu
		
		AlignLeft,						//	align� sur la marge gauche
		AlignRight,						//	align� sur la marge droite
		
		Center,							//	centr�
		
		JustifyAlignLeft,				//	justifi�, derni�re ligne AlignLeft
	}
}
