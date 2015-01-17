//	Copyright � 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
		
		void AttachToParagraph(TextStory story, ICursor cursor, Properties.ManagedParagraphProperty property);
		void DetachFromParagraph(TextStory story, ICursor cursor, Properties.ManagedParagraphProperty property);
		void RefreshParagraph(TextStory story, ICursor cursor, Properties.ManagedParagraphProperty property);
		
		//	En attachant un gestionnaire de paragraphe � un paragraphe donn�, il
		//	est possible de modifier le texte pour y ajouter des caract�res marqu�s
		//	avec la propri�t� AutoTextProperty.
		//
		//	NB: Le curseur pointe toujours au d�but du paragraphe.
		
	}
}
