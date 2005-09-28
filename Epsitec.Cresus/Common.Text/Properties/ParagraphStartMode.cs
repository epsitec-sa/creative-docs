//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Properties
{
	/// <summary>
	/// L'énumération ParagraphStartMode définit où placer un début de paragraphe.
	/// </summary>
	public enum ParagraphStartMode
	{
		Undefined			= 0,		//	non défini
		
		Anywhere			= 1,		//	n'importe où
		
		NewFrame			= 2,		//	au début d'un frame
		
		NewPage				= 3,		//	au début d'une page
		NewOddPage			= 4,		//	au début d'une page impaire
		NewEvenPage			= 5,		//	au début d'une page paire
	}
}
