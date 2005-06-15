//	Copyright � 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// L'interface IParagraphManager d�crit les fonctions qu'un gestionnaire de
	/// paragraphe doit mettre � disposition.
	/// du texte.
	/// </summary>
	public interface IParagraphManager
	{
		string	Name	{ get; }
		
		void AttachToParagraph(TextStory story, ICursor cursor, string[] parameters);
		void DetachFromParagraph(TextStory story, ICursor cursor, string[] parameters);
	}
}
