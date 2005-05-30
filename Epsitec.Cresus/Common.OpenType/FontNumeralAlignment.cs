//	Copyright � 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.OpenType
{
	/// <summary>
	/// L'�num�ration FontNumeralAlignment d�termine quelle variante de chiffres
	/// doit �tre utilis�e dans une fonte.
	/// </summary>
	public enum FontNumeralAlignment
	{
		Normal,						//	comportement par d�faut pour la fonte
		
		Proportional,				//	chiffres avec largeur proportionnelle
		Tabular,					//	chiffres avec largeur fixe
	}
}
