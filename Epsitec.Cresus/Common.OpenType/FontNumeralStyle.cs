//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.OpenType
{
	/// <summary>
	/// L'énumération FontNumeralStyle détermine quelle variante de chiffres
	/// doit être utilisée dans une fonte.
	/// </summary>
	public enum FontNumeralStyle
	{
		Normal,						//	comportement par défaut pour la fonte
		
		Lining,						//	chiffres de même hauteur
		OldStyle,					//	chiffres 'anciens' alignés sur les minuscules
	}
}
