//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// L'énumération CursorAttachment définit comment un curseur est attaché
	/// au texte sous-jacent.
	/// </summary>
	public enum CursorAttachment : byte
	{
		Floating	= 0,	//	flottant (en cas de destruction du texte, ajuste
							//	..simplement la position du curseur à l'extrémité
							//	..de la zone détruite)
		
		ToNext		= 1,	//	attaché au caractère suivant
		ToPrevious	= 2,	//	attaché au caractère précédent
		
		Temporary	= 10,	//	comme Floating, mais à ignorer par les undo/redo
	}
}
