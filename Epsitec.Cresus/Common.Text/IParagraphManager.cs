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
		
		//	En attachant un gestionnaire de paragraphe � un paragraphe donn�, il
		//	est possible de modifier le texte pour y ajouter des caract�res marqu�s
		//	avec la propri�t� AutoTextProperty.
		//
		//	NB: Le curseur pointe toujours au d�but du paragraphe.
		
		void AttachToParagraph(TextStory story, ICursor cursor, string[] parameters);
		void DetachFromParagraph(TextStory story, ICursor cursor, string[] parameters);
	}
}
