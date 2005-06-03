//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Layout
{
	/// <summary>
	/// Summary description for Status.
	/// </summary>
	public enum Status
	{
		Undefined,
		
		Ok,
		OkFitEnded,
		OkTabReached,
		
		SwitchLayout,
		
		RestartLineLayout,					//	relance le layout de la ligne en cours
		RestartParagraphLayout,				//	relance le layout du paragraphe entier
		
		RewindParagraphAndRestartLayout,	//	remonte au paragraphe précédent et relance
		
		ErrorNeedMoreText,
		ErrorNeedMoreRoom,
		ErrorCannotFit,
	}
}
