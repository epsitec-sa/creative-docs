//	Copyright � 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
		ClearUniform,							//	comme Clear, mais force un paragraphe uniforme
		Combine,								//	combine avec les r�glages courants
		
		Overwrite,								//	�crase les r�glages et supprime en plus les autres propri�t�s
	}
}
