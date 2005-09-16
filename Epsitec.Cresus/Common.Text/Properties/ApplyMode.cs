//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
		Combine,								//	combine avec les réglages courants
	}
}
