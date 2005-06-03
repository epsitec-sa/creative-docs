//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Properties
{
	/// <summary>
	/// L'énumération ParagraphStartMode définit où placer un début de paragraphe.
	/// </summary>
	public enum ParagraphStartMode
	{
		Undefined,					//	non défini
		
		Anywhere,					//	n'importe où
		
		NewFrame,					//	au début d'un frame
		
		NewPage,					//	au début d'une page
		NewOddPage,					//	au début d'une page impaire
		NewEvenPage,				//	au début d'une page paire
	}
}
