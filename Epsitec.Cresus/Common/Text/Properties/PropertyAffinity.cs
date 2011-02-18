//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Properties
{
	/// <summary>
	/// L'énumération PropertyAffinity définit l'affinité d'une propriété, à
	/// savoir comment elle est rattachée au texte.
	/// </summary>
	public enum PropertyAffinity
	{
		Invalid	= 0,
		
		Text	= 1,					//	propriété "normale"
		Symbol	= 2,					//	propriété rattachée à un symbole
	}
	
	//	Les propriétés dont l'affinité est PropertyAffinity.Symbol sont très
	//	particulières : elles définissent des informations vitales sans les-
	//	quelles le symbole n'aurait pas de signification.
	//
	//	Exemples :
	//
	//	- ImageProperty : définit quelle image peindre en lieu et place du
	//	  symbole Unicode.Code.ObjectReplacement.
	//
	//	- OpenTypeProperty : définit quel glyphe particulier utiliser pour
	//	  le symbole correspondant.
	//
	//	- TabProperty : définit à quel tabulateur le symbole HorizontalTab
	//	  se rattache.
}
