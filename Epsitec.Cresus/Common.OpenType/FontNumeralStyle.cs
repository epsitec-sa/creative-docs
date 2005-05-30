//	Copyright � 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.OpenType
{
	/// <summary>
	/// L'�num�ration FontNumeralStyle d�termine quelle variante de chiffres
	/// doit �tre utilis�e dans une fonte.
	/// </summary>
	public enum FontNumeralStyle
	{
		Normal,						//	comportement par d�faut pour la fonte
		
		Lining,						//	chiffres de m�me hauteur
		OldStyle,					//	chiffres 'anciens' align�s sur les minuscules
	}
}
