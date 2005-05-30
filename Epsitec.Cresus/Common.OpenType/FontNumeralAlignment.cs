//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.OpenType
{
	/// <summary>
	/// L'énumération FontNumeralAlignment détermine quelle variante de chiffres
	/// doit être utilisée dans une fonte.
	/// </summary>
	public enum FontNumeralAlignment
	{
		Normal,						//	comportement par défaut pour la fonte
		
		Proportional,				//	chiffres avec largeur proportionnelle
		Tabular,					//	chiffres avec largeur fixe
	}
}
