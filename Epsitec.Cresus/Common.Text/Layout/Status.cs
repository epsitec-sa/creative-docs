//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
		OkHiddenFitEnded,
		
		SwitchLayout,
		
		RestartLineLayout,					//	relance le layout de la ligne en cours
		RestartParagraphLayout,				//	relance le layout du paragraphe entier
		
		RewindParagraphAndRestartLayout,	//	remonte au paragraphe précédent et relance
		
		ErrorNeedMoreText,
		ErrorNeedMoreRoom,
		ErrorCannotFit,
	}
}
