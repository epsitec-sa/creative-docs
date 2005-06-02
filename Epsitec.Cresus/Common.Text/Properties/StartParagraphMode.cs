//	Copyright � 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Properties
{
	/// <summary>
	/// L'�num�ration StartParagraphMode d�finit o� placer un d�but de paragraphe.
	/// </summary>
	public enum StartParagraphMode
	{
		Undefined,					//	non d�fini
		
		Anywhere,					//	n'importe o�
		
		NextFrame,					//	dans le prochain cadre
		NextPage,					//	dans la prochaine page
		NextOddPage,				//	dans la prochaine page impaire
		NextEvenPage,				//	dans la prochaine page paire
	}
}
