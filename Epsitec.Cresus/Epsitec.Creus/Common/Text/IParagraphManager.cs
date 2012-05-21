//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// L'interface IParagraphManager décrit les fonctions qu'un gestionnaire de
	/// paragraphe doit mettre à disposition.
	/// du texte.
	/// </summary>
	public interface IParagraphManager
	{
		string	Name	{ get; }
		
		void AttachToParagraph(TextStory story, ICursor cursor, Properties.ManagedParagraphProperty property);
		void DetachFromParagraph(TextStory story, ICursor cursor, Properties.ManagedParagraphProperty property);
		void RefreshParagraph(TextStory story, ICursor cursor, Properties.ManagedParagraphProperty property);
		
		//	En attachant un gestionnaire de paragraphe à un paragraphe donné, il
		//	est possible de modifier le texte pour y ajouter des caractères marqués
		//	avec la propriété AutoTextProperty.
		//
		//	NB: Le curseur pointe toujours au début du paragraphe.
		
	}
}
