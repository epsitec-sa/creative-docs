//	Copyright � 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// L'�num�ration CursorAttachment d�finit comment un curseur est attach�
	/// au texte sous-jacent.
	/// </summary>
	public enum CursorAttachment : byte
	{
		Floating	= 0,	//	flottant (en cas de destruction du texte, ajuste
							//	..simplement la position du curseur � l'extr�mit�
							//	..de la zone d�truite)
		
		ToNext		= 1,	//	attach� au caract�re suivant
		ToPrevious	= 2,	//	attach� au caract�re pr�c�dent
		
		Temporary	= 10,	//	comme Floating, mais � ignorer par les undo/redo
	}
}
