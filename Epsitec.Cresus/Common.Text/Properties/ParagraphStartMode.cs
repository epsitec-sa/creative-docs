//	Copyright � 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Properties
{
	/// <summary>
	/// L'�num�ration ParagraphStartMode d�finit o� placer un d�but de paragraphe.
	/// </summary>
	public enum ParagraphStartMode
	{
		Undefined,					//	non d�fini
		
		Anywhere,					//	n'importe o�
		
		NewFrame,					//	au d�but d'un frame
		
		NewPage,					//	au d�but d'une page
		NewOddPage,					//	au d�but d'une page impaire
		NewEvenPage,				//	au d�but d'une page paire
	}
}
