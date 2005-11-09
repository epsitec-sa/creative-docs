//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// L'énumération TabStatus définit comment/si un tabulateur est défini et
	/// référencé dans le texte.
	/// </summary>
	public enum TabStatus
	{
		None				= 0,
		
		Definition			= 1,				//	tab défini mais pas utilisé
		Live				= 2,				//	tab défini et utilisé
		Zombie				= 3					//	tab pas défini mais utilisé
	}
}
