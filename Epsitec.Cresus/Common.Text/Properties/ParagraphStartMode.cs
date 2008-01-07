//	Copyright � 2005-2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Properties
{
	/// <summary>
	/// L'�num�ration ParagraphStartMode d�finit o� placer un d�but de paragraphe.
	/// </summary>
	public enum ParagraphStartMode
	{
		Undefined			= 0,		//	non d�fini
		
		Anywhere			= 1,		//	n'importe o�
		
		NewFrame			= 2,		//	au d�but d'un frame
		
		NewPage				= 3,		//	au d�but d'une page
		NewOddPage			= 4,		//	au d�but d'une page impaire
		NewEvenPage			= 5,		//	au d�but d'une page paire
	}
}
