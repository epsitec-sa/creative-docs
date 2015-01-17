//	Copyright � 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Properties
{
	/// <summary>
	/// L'�num�ration ApplyMode d�finit comment des propri�t�s doivent �tre
	/// appliqu�es � un texte.
	/// </summary>
	public enum ApplyMode
	{
		None,									//	ne change rien
		
		Set,									//	�crase les r�glages courants
		Clear,									//	efface les r�glages courants
		Combine,								//	combine avec les r�glages courants
	}
}
