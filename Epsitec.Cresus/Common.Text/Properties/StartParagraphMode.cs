//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Properties
{
	/// <summary>
	/// L'énumération StartParagraphMode définit où placer un début de paragraphe.
	/// </summary>
	public enum StartParagraphMode
	{
		Undefined,					//	non défini
		
		Anywhere,					//	n'importe où
		
		NextFrame,					//	dans le prochain cadre
		NextPage,					//	dans la prochaine page
		NextOddPage,				//	dans la prochaine page impaire
		NextEvenPage,				//	dans la prochaine page paire
	}
}
