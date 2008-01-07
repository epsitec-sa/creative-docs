//	Copyright � 2005-2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
		JustifyCenter,					//	justifi�, derni�re ligne Center
		JustifyAlignRight,				//	justifi�, derni�re ligne AlignRight
		JustifyJustfy					//	justifi�, derni�re ligne Justify
	}
}
