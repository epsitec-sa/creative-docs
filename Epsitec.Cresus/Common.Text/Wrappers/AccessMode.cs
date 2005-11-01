//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Wrappers
{
	/// <summary>
	/// Une classe dérivée AbstractState peut être mise en lecture seule ou
	/// supporter un modèle Read & Write.
	/// </summary>
	public enum AccessMode
	{
		ReadOnly,								//	accessible en lecture seulement
		ReadWrite								//	lecture & écriture (modifiable)
	}
}
