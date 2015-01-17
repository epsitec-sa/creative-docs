//	Copyright � 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Properties
{
	/// <summary>
	/// L'�num�ration PropertyAffinity d�finit l'affinit� d'une propri�t�, �
	/// savoir comment elle est rattach�e au texte.
	/// </summary>
	public enum PropertyAffinity
	{
		Invalid	= 0,
		
		Text	= 1,					//	propri�t� "normale"
		Symbol	= 2,					//	propri�t� rattach�e � un symbole
	}
	
	//	Les propri�t�s dont l'affinit� est PropertyAffinity.Symbol sont tr�s
	//	particuli�res : elles d�finissent des informations vitales sans les-
	//	quelles le symbole n'aurait pas de signification.
	//
	//	Exemples :
	//
	//	- ImageProperty : d�finit quelle image peindre en lieu et place du
	//	  symbole Unicode.Code.ObjectReplacement.
	//
	//	- OpenTypeProperty : d�finit quel glyphe particulier utiliser pour
	//	  le symbole correspondant.
	//
	//	- TabProperty : d�finit � quel tabulateur le symbole HorizontalTab
	//	  se rattache.
}
