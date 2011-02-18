//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Properties
{
	/// <summary>
	/// L'énumération ApplyMode définit comment des propriétés doivent être
	/// appliquées à un texte.
	/// </summary>
	public enum ApplyMode
	{
		None,									//	ne change rien
		
		Set,									//	écrase les réglages courants
		Clear,									//	efface les réglages courants
		ClearUniform,							//	comme Clear, mais force un paragraphe uniforme
		Combine,								//	combine avec les réglages courants
		
		Overwrite,								//	écrase les réglages et supprime en plus les autres propriétés
	}
}
