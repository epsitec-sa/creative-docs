//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
		
		//	En attachant un gestionnaire de paragraphe à un paragraphe donné, il
		//	est possible de modifier le texte pour y ajouter des caractères marqués
		//	avec la propriété AutoTextProperty.
		//
		//	NB: Le curseur pointe toujours au début du paragraphe.
		
		void AttachToParagraph(TextStory story, ICursor cursor, string[] parameters);
		void DetachFromParagraph(TextStory story, ICursor cursor, string[] parameters);
	}
}
